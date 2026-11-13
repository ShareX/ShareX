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

using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ShareX.PixelEditor.UI;

/// <summary>The view state of an open sprite: zoom, scroll position and the composited image shown on the canvas.</summary>
public sealed class SpriteView : IDisposable
{
    public static readonly double[] ZoomLevels = [0.125, 0.25, 0.5, 1, 2, 3, 4, 5, 6, 8, 10, 12, 16, 20, 24, 32, 48, 64];

    private PixelBuffer _composite;
    private PixelBuffer? _onion;
    private (int Frame, int Version, int Previous, int Next, int Opacity, bool Tint) _onionKey;
    private SKRectI _dirty = SKRectI.Empty;
    private bool _dirtyAll = true;
    private int _shownFrame = -1;
    private bool _shownOnion;

    public SpriteView(Sprite sprite)
    {
        Sprite = sprite;
        _composite = new PixelBuffer(sprite.Width, sprite.Height);
        AxisX2 = sprite.Width;
        AxisY2 = sprite.Height;
    }

    public Sprite Sprite { get; }

    public double Zoom { get; private set; } = 1;

    /// <summary>Canvas position of the sprite's top-left corner.</summary>
    public double PanX { get; private set; }

    public double PanY { get; private set; }

    public Size Viewport { get; set; }

    public bool NeedsFit { get; set; } = true;

    /// <summary>Twice the positions of the symmetry axes.</summary>
    public int AxisX2 { get; set; }

    public int AxisY2 { get; set; }

    public int AntsPhase { get; set; }

    /// <summary>The frame shown while the animation plays; null shows the active frame.</summary>
    public int? PlayFrame { get; set; }

    public int ShownFrame => Math.Clamp(PlayFrame ?? Sprite.ActiveFrame, 0, Sprite.Frames.Count - 1);

    public PixelBuffer Composite => _composite;

    public WriteableBitmap? Display { get; private set; }

    public Matrix Matrix => Matrix.CreateScale(Zoom, Zoom) * Matrix.CreateTranslation(PanX, PanY);

    public Point ToScreen(double x, double y) => new(x * Zoom + PanX, y * Zoom + PanY);

    public (double X, double Y) ToDocument(Point p) => ((p.X - PanX) / Zoom, (p.Y - PanY) / Zoom);

    public void MarkDirty(SKRectI? area)
    {
        if (area is SKRectI r)
        {
            _dirty = _dirty.IsEmpty ? r : SKRectI.Union(_dirty, r);
        }
        else
        {
            _dirtyAll = true;
        }
    }

    /// <summary>Recomposites what changed and refreshes the display bitmap.</summary>
    public unsafe void Update(EditorSettings settings)
    {
        Sprite sprite = Sprite;
        if (_composite.Width != sprite.Width || _composite.Height != sprite.Height)
        {
            _composite = new PixelBuffer(sprite.Width, sprite.Height);
            Display?.Dispose();
            Display = null;
            _dirtyAll = true;
            // Axes follow the center unless the sprite keeps its size.
            AxisX2 = sprite.Width;
            AxisY2 = sprite.Height;
        }
        int frame = ShownFrame;
        bool onion = settings.OnionSkin && PlayFrame == null && sprite.Frames.Count > 1 && (settings.OnionPrevious > 0 || settings.OnionNext > 0);
        if (frame != _shownFrame || onion != _shownOnion) _dirtyAll = true;
        if (onion)
        {
            var key = (frame, sprite.Version, settings.OnionPrevious, settings.OnionNext, settings.OnionOpacity, settings.OnionTint);
            if (_onion == null || key != _onionKey)
            {
                _onion = BuildOnion(sprite, frame, settings);
                _onionKey = key;
                _dirtyAll = true;
            }
        }
        else
        {
            _onion = null;
        }
        if (Display == null)
        {
            Display = new WriteableBitmap(new PixelSize(sprite.Width, sprite.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
            _dirtyAll = true;
        }
        if (!_dirtyAll && _dirty.IsEmpty) return;
        SKRectI area = _dirtyAll ? sprite.Bounds : _composite.Clip(_dirty);
        _dirtyAll = false;
        _dirty = SKRectI.Empty;
        _shownFrame = frame;
        _shownOnion = onion;
        if (area.IsEmpty) return;
        Compositor.Compose(sprite, frame, _composite, area);
        using ILockedFramebuffer buffer = Display.Lock();
        for (int y = area.Top; y < area.Bottom; y++)
        {
            uint* row = (uint*)((byte*)buffer.Address + y * buffer.RowBytes);
            int source = y * _composite.Width;
            for (int x = area.Left; x < area.Right; x++)
            {
                uint c = _composite.Pixels[source + x];
                if (_onion != null)
                {
                    uint under = _onion.Pixels[source + x];
                    if (under >> 24 != 0) c = Argb.Over(under, c);
                }
                row[x] = Argb.Premultiply(c);
            }
        }
    }

    /// <summary>The neighboring frames, faded and tinted (red before, blue after), drawn under the current frame.</summary>
    private static PixelBuffer BuildOnion(Sprite sprite, int frame, EditorSettings settings)
    {
        PixelBuffer result = new(sprite.Width, sprite.Height);
        void Add(int f, int distance, uint tint)
        {
            if (f < 0 || f >= sprite.Frames.Count) return;
            PixelBuffer flat = Compositor.Flatten(sprite, f);
            int opacity = settings.OnionOpacity * 255 / 100 / distance;
            for (int i = 0; i < flat.Pixels.Length; i++)
            {
                uint c = flat.Pixels[i];
                if (c >> 24 == 0) continue;
                if (settings.OnionTint) c = Argb.WithAlpha(Argb.Lerp(Argb.Opaque(c), tint, 0.55f), Argb.A(c));
                result.Pixels[i] = Argb.Over(result.Pixels[i], c, opacity);
            }
        }
        for (int d = settings.OnionPrevious; d >= 1; d--) Add(frame - d, d, 0xFFFF3B30);
        for (int d = settings.OnionNext; d >= 1; d--) Add(frame + d, d, 0xFF2F80FF);
        return result;
    }

    // ------------------------------------------------------------------ navigation

    public void PanBy(double dx, double dy)
    {
        PanX += dx;
        PanY += dy;
        ClampPan();
    }

    /// <summary>Keeps at least a corner of the sprite inside the viewport.</summary>
    private void ClampPan()
    {
        if (Viewport.Width <= 0 || Viewport.Height <= 0) return;
        double w = Sprite.Width * Zoom, h = Sprite.Height * Zoom, margin = 40;
        PanX = Math.Clamp(PanX, margin - w, Viewport.Width - margin);
        PanY = Math.Clamp(PanY, margin - h, Viewport.Height - margin);
    }

    public void ZoomTo(double zoom, Point anchor)
    {
        zoom = Math.Clamp(zoom, ZoomLevels[0], ZoomLevels[^1]);
        (double x, double y) = ToDocument(anchor);
        Zoom = zoom;
        PanX = Math.Round(anchor.X - x * zoom);
        PanY = Math.Round(anchor.Y - y * zoom);
        ClampPan();
    }

    public void ZoomStep(bool zoomIn, Point anchor)
    {
        double next = zoomIn
            ? ZoomLevels.FirstOrDefault(z => z > Zoom * 1.001, ZoomLevels[^1])
            : ZoomLevels.LastOrDefault(z => z < Zoom * 0.999, ZoomLevels[0]);
        ZoomTo(next, anchor);
    }

    public void ZoomAtDocument(double x, double y, bool zoomIn) => ZoomStep(zoomIn, ToScreen(x, y));

    public Point ViewportCenter => new(Viewport.Width / 2, Viewport.Height / 2);

    /// <summary>The largest whole zoom level that shows the entire sprite.</summary>
    public void Fit()
    {
        if (Viewport.Width <= 0 || Viewport.Height <= 0) return;
        double available = Math.Min((Viewport.Width - 48) / Sprite.Width, (Viewport.Height - 48) / Sprite.Height);
        Zoom = ZoomLevels.LastOrDefault(z => z <= available, ZoomLevels[0]);
        Center();
        NeedsFit = false;
    }

    /// <summary>Zooms so an area of the sprite fills the view and centers it.</summary>
    public void FitArea(SKRectI area)
    {
        if (Viewport.Width <= 0 || Viewport.Height <= 0 || area.Width <= 0 || area.Height <= 0) return;
        double available = Math.Min((Viewport.Width - 48) / area.Width, (Viewport.Height - 48) / area.Height);
        Zoom = ZoomLevels.LastOrDefault(z => z <= available, ZoomLevels[0]);
        PanX = Math.Round(Viewport.Width / 2 - (area.Left + area.Width / 2.0) * Zoom);
        PanY = Math.Round(Viewport.Height / 2 - (area.Top + area.Height / 2.0) * Zoom);
        ClampPan();
        NeedsFit = false;
    }

    public void Center()
    {
        PanX = Math.Round((Viewport.Width - Sprite.Width * Zoom) / 2);
        PanY = Math.Round((Viewport.Height - Sprite.Height * Zoom) / 2);
    }

    public void Dispose()
    {
        Display?.Dispose();
        Display = null;
    }
}
