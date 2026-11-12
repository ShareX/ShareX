#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

namespace ShareX.PixelEditor.Core;

/// <summary>How a painted color is combined with the pixel under it.</summary>
public enum Ink
{
    /// <summary>Composites the color over the pixel.</summary>
    Normal,
    /// <summary>Writes the color and its alpha as they are.</summary>
    Replace,
    /// <summary>Recolors existing pixels and keeps their alpha; transparent pixels stay transparent.</summary>
    LockAlpha,
    /// <summary>Paints only where the layer is still transparent or translucent.</summary>
    Behind,
    Erase,
    /// <summary>Moves a pixel to the next lighter color of the palette.</summary>
    Lighten,
    /// <summary>Moves a pixel to the next darker color of the palette.</summary>
    Darken,
    /// <summary>Replaces only pixels of the secondary color.</summary>
    ReplaceColor
}

public enum BrushPattern
{
    Solid,
    Checker,
    Sparse,
    Dense,
    HorizontalLines,
    VerticalLines,
    Diagonal,
    Dots
}

public enum SymmetryMode
{
    None,
    Horizontal,
    Vertical,
    Both
}

public enum DitherMode
{
    None,
    Bayer4,
    Bayer8
}

/// <summary>
/// Paints on a cel buffer. Every pixel is computed from the buffer as it was before the stroke, so overlapping
/// parts of one stroke never build up and a shape preview can be undone by restoring an area.
/// </summary>
public sealed class Painter
{
    private readonly PixelBuffer _target;
    private readonly PixelBuffer _original;
    private readonly int _width, _height;
    private SKRectI _dirty = SKRectI.Empty;
    private SKRectI _total = SKRectI.Empty;

    public Painter(PixelBuffer target, PixelBuffer original)
    {
        _target = target;
        _original = original;
        _width = target.Width;
        _height = target.Height;
    }

    public PixelBuffer Target => _target;

    public PixelBuffer Original => _original;

    public uint Color { get; set; } = Argb.Black;

    /// <summary>The color the ReplaceColor ink looks for.</summary>
    public uint SecondaryColor { get; set; }

    public Ink Ink { get; set; }

    public int Opacity { get; set; } = 255;

    public SKPointI[] Tip { get; set; } = [new SKPointI(0, 0)];

    public BrushPattern Pattern { get; set; }

    public SelectionMask? Clip { get; set; }

    public SymmetryMode Symmetry { get; set; }

    /// <summary>Twice the position of the mirror axes, so an axis can sit on a pixel edge or through a pixel center.</summary>
    public int AxisX2 { get; set; }

    public int AxisY2 { get; set; }

    public bool TiledX { get; set; }

    public bool TiledY { get; set; }

    public IReadOnlyList<uint>? Palette { get; set; }

    public int Tolerance { get; set; }

    /// <summary>Returns the area changed since the last call and starts a new one.</summary>
    public SKRectI TakeDirty()
    {
        SKRectI result = _dirty;
        _dirty = SKRectI.Empty;
        return result;
    }

    /// <summary>Everything this painter has touched.</summary>
    public SKRectI TotalDirty => _total;

    private void MarkDirty(int x, int y)
    {
        if (_dirty.IsEmpty) _dirty = new SKRectI(x, y, x + 1, y + 1);
        else
        {
            if (x < _dirty.Left) _dirty.Left = x;
            if (x >= _dirty.Right) _dirty.Right = x + 1;
            if (y < _dirty.Top) _dirty.Top = y;
            if (y >= _dirty.Bottom) _dirty.Bottom = y + 1;
        }
        if (_total.IsEmpty) _total = new SKRectI(x, y, x + 1, y + 1);
        else
        {
            if (x < _total.Left) _total.Left = x;
            if (x >= _total.Right) _total.Right = x + 1;
            if (y < _total.Top) _total.Top = y;
            if (y >= _total.Bottom) _total.Bottom = y + 1;
        }
    }

    public static bool PatternHit(BrushPattern pattern, int x, int y) => pattern switch
    {
        BrushPattern.Checker => ((x + y) & 1) == 0,
        BrushPattern.Sparse => (x & 1) == 0 && (y & 1) == 0,
        BrushPattern.Dense => (x & 1) == 0 || (y & 1) == 0,
        BrushPattern.HorizontalLines => (y & 1) == 0,
        BrushPattern.VerticalLines => (x & 1) == 0,
        BrushPattern.Diagonal => ((x + y) & 3) == 0,
        BrushPattern.Dots => (x & 3) == 0 && (y & 3) == 0 || (x & 3) == 2 && (y & 3) == 2,
        _ => true
    };

    /// <summary>Paints one pixel and its mirror images.</summary>
    public void Pixel(int x, int y)
    {
        Plot(x, y);
        if (Symmetry == SymmetryMode.None) return;
        int mx = AxisX2 - 1 - x, my = AxisY2 - 1 - y;
        if (Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both) Plot(mx, y);
        if (Symmetry is SymmetryMode.Vertical or SymmetryMode.Both) Plot(x, my);
        if (Symmetry == SymmetryMode.Both) Plot(mx, my);
    }

    private void Plot(int x, int y)
    {
        if (TiledX) x = ((x % _width) + _width) % _width;
        if (TiledY) y = ((y % _height) + _height) % _height;
        if ((uint)x >= (uint)_width || (uint)y >= (uint)_height) return;
        int i = y * _width + x;
        if (Clip != null && !Clip.Data[i]) return;
        if (Pattern != BrushPattern.Solid && !PatternHit(Pattern, x, y)) return;
        uint before = _original.Pixels[i];
        uint after = Apply(before);
        if (after >> 24 == 0) after = 0;
        if (_target.Pixels[i] == after) return;
        _target.Pixels[i] = after;
        MarkDirty(x, y);
    }

    private uint Apply(uint before)
    {
        switch (Ink)
        {
            case Ink.Replace:
                return Argb.WithAlpha(Color, (Argb.A(Color) * Opacity + 127) / 255);
            case Ink.LockAlpha:
                if (before >> 24 == 0) return 0;
                return Argb.WithAlpha(Argb.Over(Argb.Opaque(before), Argb.Opaque(Color), (Argb.A(Color) * Opacity + 127) / 255), Argb.A(before));
            case Ink.Behind:
                return Argb.Over(Argb.WithAlpha(Color, (Argb.A(Color) * Opacity + 127) / 255), before);
            case Ink.Erase:
                return Opacity >= 255 ? 0 : Argb.WithAlpha(before, Argb.A(before) * (255 - Opacity) / 255);
            case Ink.Lighten:
                return Shade(before, 1);
            case Ink.Darken:
                return Shade(before, -1);
            case Ink.ReplaceColor:
                long limit = (long)Tolerance * Tolerance * 12;
                return Argb.Distance(before, SecondaryColor) <= limit ? Argb.Over(before, Color, Opacity) : before;
            default:
                return Argb.Over(before, Color, Opacity);
        }
    }

    /// <summary>Steps a color along the palette by brightness, or shifts its lightness when it is not a palette color.</summary>
    private uint Shade(uint color, int direction)
    {
        if (color >> 24 == 0) return color;
        if (Palette != null)
        {
            int index = -1;
            for (int i = 0; i < Palette.Count; i++)
            {
                if ((Palette[i] & 0xFFFFFF) == (color & 0xFFFFFF))
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                // The neighbor in the palette that is next in brightness, preferring colors of a similar hue.
                int luma = Argb.Luma(color);
                uint best = color;
                long bestScore = long.MaxValue;
                for (int i = 0; i < Palette.Count; i++)
                {
                    int delta = (Argb.Luma(Palette[i]) - luma) * direction;
                    if (delta <= 0 || Palette[i] >> 24 == 0) continue;
                    long score = Argb.Distance(Argb.Opaque(Palette[i]), Argb.Opaque(color)) + (long)Math.Abs(i - index) * 64;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = Palette[i];
                    }
                }
                return Argb.WithAlpha(best, Argb.A(color));
            }
        }
        (float h, float s, float l) = Argb.ToHsl(color);
        return Argb.FromHsl(h, s, l + direction * 0.08f, Argb.A(color));
    }

    /// <summary>Stamps the brush tip at a position.</summary>
    public void Stamp(int x, int y)
    {
        foreach (SKPointI p in Tip) Pixel(x + p.X, y + p.Y);
    }

    public void Line(int x0, int y0, int x1, int y1) => Raster.Line(x0, y0, x1, y1, Stamp);

    public void Rect(int x0, int y0, int x1, int y1, bool filled)
    {
        if (filled) Raster.Rect(x0, y0, x1, y1, true, Pixel);
        else Raster.Rect(x0, y0, x1, y1, false, Stamp);
    }

    public void Ellipse(int x0, int y0, int x1, int y1, bool filled)
    {
        if (filled) Raster.Ellipse(x0, y0, x1, y1, true, Pixel);
        else Raster.Ellipse(x0, y0, x1, y1, false, Stamp);
    }

    public void Polygon(IReadOnlyList<SKPointI> points, bool filled)
    {
        if (filled) Raster.FillPolygon(points, Pixel);
        else
        {
            for (int i = 0; i < points.Count; i++)
            {
                SKPointI p = points[i], q = points[(i + 1) % points.Count];
                Line(p.X, p.Y, q.X, q.Y);
            }
        }
    }

    public void FillRegion(bool[] region)
    {
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                if (region[y * _width + x]) Pixel(x, y);
    }

    /// <summary>Fills the clip area (or everything) with a gradient between two colors.</summary>
    public void Gradient(float x0, float y0, float x1, float y1, uint from, uint to, bool radial, DitherMode dither, bool[]? region = null)
    {
        float dx = x1 - x0, dy = y1 - y0, lengthSquared = dx * dx + dy * dy, length = MathF.Sqrt(lengthSquared);
        Ink ink = Ink;
        uint color = Color;
        Ink = Ink == Ink.LockAlpha ? Ink.LockAlpha : Ink.Normal;
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (region != null && !region[y * _width + x]) continue;
                float px = x + 0.5f - x0, py = y + 0.5f - y0;
                float t = lengthSquared <= 0 ? 1 : radial ? MathF.Sqrt(px * px + py * py) / length : (px * dx + py * dy) / lengthSquared;
                t = Math.Clamp(t, 0, 1);
                Color = dither == DitherMode.None ? Argb.Lerp(from, to, t) : t > Raster.BayerThreshold(x, y, dither == DitherMode.Bayer8) ? to : from;
                if (Color >> 24 != 0) Plot(x, y);
            }
        }
        Ink = ink;
        Color = color;
    }

    /// <summary>Puts the pixels of an area back to how they were before the stroke.</summary>
    public void Restore(SKRectI area)
    {
        area = _target.Clip(area);
        if (area.IsEmpty) return;
        _target.CopyFrom(_original, area);
        _dirty = _dirty.IsEmpty ? area : SKRectI.Union(_dirty, area);
    }

    /// <summary>Undoes everything painted so far, for redrawing a shape preview.</summary>
    public void RestoreAll()
    {
        if (_total.IsEmpty) return;
        SKRectI area = _total;
        Restore(area);
        _total = SKRectI.Empty;
    }

    /// <summary>Undoes one stamp of the brush tip, used by pixel-perfect strokes to remove corner pixels.</summary>
    public void RestoreStamp(int x, int y)
    {
        void One(int px, int py)
        {
            if (TiledX) px = ((px % _width) + _width) % _width;
            if (TiledY) py = ((py % _height) + _height) % _height;
            if ((uint)px >= (uint)_width || (uint)py >= (uint)_height) return;
            int i = py * _width + px;
            if (_target.Pixels[i] == _original.Pixels[i]) return;
            _target.Pixels[i] = _original.Pixels[i];
            MarkDirty(px, py);
        }
        foreach (SKPointI p in Tip)
        {
            int px = x + p.X, py = y + p.Y, mx = AxisX2 - 1 - px, my = AxisY2 - 1 - py;
            One(px, py);
            if (Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both) One(mx, py);
            if (Symmetry is SymmetryMode.Vertical or SymmetryMode.Both) One(px, my);
            if (Symmetry == SymmetryMode.Both) One(mx, my);
        }
    }
}
