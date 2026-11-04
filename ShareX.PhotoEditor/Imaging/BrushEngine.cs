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

namespace ShareX.PhotoEditor.Imaging;

/// <summary>Per-dab callback for tools that transform existing pixels (blur, smudge, dodge, ...).</summary>
/// <param name="stroke">The stroke being painted.</param>
/// <param name="dabRect">Dab bounds in target coordinates.</param>
/// <param name="alpha">Dab coverage (row-major, dabRect size), already multiplied by flow, pressure and selection.</param>
/// <param name="center">Dab center in target coordinates.</param>
public delegate void DabProcessor(BrushStroke stroke, SKRectI dabRect, float[] alpha, SKPoint center);

public enum StrokeMode
{
    /// <summary>Paint a color or sampled source; coverage accumulates with flow and is capped by opacity.</summary>
    Accumulate,
    /// <summary>Each dab calls a processor that modifies the target directly.</summary>
    Direct
}

/// <summary>
/// Paints one brush stroke onto a BGRA surface or an 8-bit mask. Implements spacing, size/opacity pressure,
/// jitter, flow build-up capped by opacity, blend modes, lock transparency and selection clipping.
/// </summary>
public sealed class BrushStroke : IDisposable
{
    private readonly ushort[]? _coverage;
    private readonly uint[]? _baseColor;
    private readonly byte[]? _baseMask;
    private readonly Random _random = new(17);
    private SKPoint? _last;
    private float _lastPressure = 1;
    private float _distanceCarry;

    public BrushStroke(PixelSurface target, SKPointI offset, BrushPreset tip, StrokeMode mode)
    {
        Target = target;
        Offset = offset;
        Tip = tip;
        Mode = mode;
        int count = target.Width * target.Height;
        if (mode == StrokeMode.Accumulate)
        {
            _coverage = new ushort[count];
            if (target.IsMask)
            {
                _baseMask = target.GetSpan().ToArray();
            }
            else
            {
                _baseColor = new uint[count];
                Span<uint> pixels = target.GetPixelSpan();
                int stride = target.RowBytes / 4;
                for (int y = 0; y < target.Height; y++) pixels.Slice(y * stride, target.Width).CopyTo(_baseColor.AsSpan(y * target.Width, target.Width));
            }
        }
    }

    public PixelSurface Target { get; }

    /// <summary>Document position of the target's top-left pixel.</summary>
    public SKPointI Offset { get; }

    public BrushPreset Tip { get; }

    public StrokeMode Mode { get; }

    public float Size { get; set; } = 30;

    public float Opacity { get; set; } = 1;

    public float Flow { get; set; } = 1;

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    public bool LockTransparency { get; set; }

    /// <summary>Pencil mode: no anti-aliasing.</summary>
    public bool Aliased { get; set; }

    /// <summary>Constant paint color (unpremultiplied).</summary>
    public SKColor Color { get; set; } = SKColors.Black;

    /// <summary>Optional per-pixel color source (document coordinates, premultiplied result).</summary>
    public Func<int, int, uint>? Source { get; set; }

    /// <summary>Selection mask in document coordinates (null = everything).</summary>
    public PixelSurface? Selection { get; set; }

    public DabProcessor? Processor { get; set; }

    /// <summary>Accumulated changed area in document coordinates.</summary>
    public SKRectI Dirty { get; private set; } = SKRectI.Empty;

    public bool HasPainted => !Dirty.IsEmpty;

    /// <summary>Spacing override in pixels (0 = use tip spacing).</summary>
    public float SpacingPixels { get; set; }

    public void Begin(SKPoint documentPoint, float pressure)
    {
        _last = documentPoint;
        _lastPressure = pressure;
        _distanceCarry = 0;
        Dab(documentPoint, pressure);
    }

    /// <summary>Continues the stroke to a point, stamping dabs at the configured spacing.</summary>
    public void LineTo(SKPoint documentPoint, float pressure)
    {
        if (_last is not SKPoint last)
        {
            Begin(documentPoint, pressure);
            return;
        }
        float distance = SKPoint.Distance(last, documentPoint);
        float spacing = SpacingPixels > 0 ? SpacingPixels : Math.Max(0.5f, EffectiveSize(pressure) * Math.Max(0.01f, Tip.Spacing));
        float travelled = spacing - _distanceCarry;
        while (travelled <= distance)
        {
            float t = distance <= 0 ? 1 : travelled / distance;
            SKPoint p = new(last.X + (documentPoint.X - last.X) * t, last.Y + (documentPoint.Y - last.Y) * t);
            Dab(p, _lastPressure + (pressure - _lastPressure) * t);
            travelled += spacing;
        }
        _distanceCarry = distance - (travelled - spacing);
        _last = documentPoint;
        _lastPressure = pressure;
    }

    private float EffectiveSize(float pressure) => Math.Max(1, Size * (Tip.PressureSize ? Math.Max(0.05f, pressure) : 1));

    /// <summary>Stamps one dab centered at a document position.</summary>
    public void Dab(SKPoint documentPoint, float pressure)
    {
        float size = EffectiveSize(pressure);
        if (Tip.SizeJitter > 0) size *= 1 - _random.NextSingle() * Tip.SizeJitter;
        SKPoint center = new(documentPoint.X - Offset.X, documentPoint.Y - Offset.Y);
        if (Tip.Scatter > 0)
        {
            float angle = _random.NextSingle() * MathF.PI * 2;
            float distance = _random.NextSingle() * Tip.Scatter * size;
            center += new SKPoint(MathF.Cos(angle) * distance, MathF.Sin(angle) * distance);
        }
        float flow = Flow * (Tip.PressureOpacity ? Math.Clamp(pressure, 0.05f, 1) : 1);

        float radius = size / 2;
        int left = (int)MathF.Floor(center.X - radius - 1), top = (int)MathF.Floor(center.Y - radius - 1);
        int right = (int)MathF.Ceiling(center.X + radius + 1), bottom = (int)MathF.Ceiling(center.Y + radius + 1);
        SKRectI rect = new(left, top, right, bottom);
        rect.Intersect(new SKRectI(0, 0, Target.Width, Target.Height));
        if (rect.IsEmpty) return;

        float[] alpha = ComputeDab(rect, center, size, flow);
        ApplySelection(rect, alpha);

        if (Mode == StrokeMode.Direct)
        {
            Processor?.Invoke(this, rect, alpha, center);
        }
        else
        {
            Accumulate(rect, alpha);
        }
        SKRectI documentRect = rect;
        documentRect.Offset(Offset.X, Offset.Y);
        Dirty = Dirty.IsEmpty ? documentRect : SKRectI.Union(Dirty, documentRect);
    }

    private float[] ComputeDab(SKRectI rect, SKPoint center, float size, float flow)
    {
        int w = rect.Width, h = rect.Height;
        float[] alpha = new float[w * h];
        float radius = Math.Max(0.5f, size / 2);
        float angle = Tip.Angle * MathF.PI / 180;
        float cos = MathF.Cos(angle), sin = MathF.Sin(angle);
        float roundness = Math.Clamp(Tip.Roundness, 0.05f, 1);
        float hardness = Math.Clamp(Tip.Hardness, 0, 1);
        bool square = Tip.Shape == BrushTipShape.Square;
        SKBitmap? sampled = Tip.Shape == BrushTipShape.Sampled ? Tip.SampledTip : null;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float px = rect.Left + x + 0.5f - center.X;
                float py = rect.Top + y + 0.5f - center.Y;
                // Rotate into tip space and stretch by roundness.
                float u = px * cos + py * sin;
                float v = (-px * sin + py * cos) / roundness;
                float value;
                if (sampled != null)
                {
                    float sx = (u / radius * 0.5f + 0.5f) * sampled.Width;
                    float sy = (v / radius * 0.5f + 0.5f) * sampled.Height;
                    if (sx < 0 || sy < 0 || sx >= sampled.Width || sy >= sampled.Height) continue;
                    SKColor c = sampled.GetPixel((int)sx, (int)sy);
                    value = (255 - (c.Red + c.Green + c.Blue) / 3f) / 255f * c.Alpha / 255f;
                }
                else
                {
                    float distance = square ? Math.Max(MathF.Abs(u), MathF.Abs(v)) : MathF.Sqrt(u * u + v * v);
                    if (Aliased)
                    {
                        value = distance <= radius ? 1 : 0;
                        if (size <= 1.5f) value = MathF.Abs(px) < 0.5f && MathF.Abs(py) < 0.5f ? 1 : 0;
                    }
                    else
                    {
                        float edge = Math.Clamp(radius - distance + 0.5f, 0, 1);
                        if (hardness >= 0.999f)
                        {
                            value = edge;
                        }
                        else
                        {
                            float d = distance / radius;
                            if (d >= 1) value = 0;
                            else if (d <= hardness) value = 1;
                            else
                            {
                                float t = (d - hardness) / (1 - hardness);
                                value = 1 - t * t * (3 - 2 * t);
                            }
                            value = Math.Min(value, edge);
                        }
                    }
                }
                alpha[y * w + x] = value * flow;
            }
        }
        return alpha;
    }

    private void ApplySelection(SKRectI rect, float[] alpha)
    {
        if (Selection == null) return;
        int w = rect.Width;
        for (int y = 0; y < rect.Height; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                if (alpha[i] <= 0) continue;
                alpha[i] *= MaskOps.Get(Selection, rect.Left + x + Offset.X, rect.Top + y + Offset.Y) / 255f;
            }
        }
    }

    private unsafe void Accumulate(SKRectI rect, float[] alpha)
    {
        int tw = Target.Width;
        float sr = Color.Red / 255f, sg = Color.Green / 255f, sb = Color.Blue / 255f, sa = Color.Alpha / 255f;
        if (Target.IsMask)
        {
            byte* m = (byte*)Target.Bitmap.GetPixels();
            int stride = Target.RowBytes;
            float targetValue = BlendMode == LayerBlendMode.Clear ? 0 : ColorMath.Luminance(sr, sg, sb) * 255;
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    float a = alpha[(y - rect.Top) * rect.Width + (x - rect.Left)];
                    if (a <= 0) continue;
                    int i = y * tw + x;
                    float coverage = _coverage![i] / 65535f;
                    coverage += (1 - coverage) * a;
                    _coverage[i] = (ushort)(coverage * 65535);
                    float baseValue = _baseMask![y * stride + x];
                    float tv = targetValue;
                    if (Source != null)
                    {
                        ColorMath.Unpack(Source(x + Offset.X, y + Offset.Y), out float r, out float g, out float b, out float pa);
                        tv = ColorMath.Luminance(r, g, b) * pa * 255;
                    }
                    m[y * stride + x] = (byte)Math.Clamp(baseValue + (tv - baseValue) * coverage * Opacity * sa, 0, 255);
                }
            }
            return;
        }

        uint* pixels = (uint*)Target.Bitmap.GetPixels();
        int pixelStride = Target.RowBytes / 4;
        for (int y = rect.Top; y < rect.Bottom; y++)
        {
            for (int x = rect.Left; x < rect.Right; x++)
            {
                float a = alpha[(y - rect.Top) * rect.Width + (x - rect.Left)];
                if (a <= 0) continue;
                int i = y * tw + x;
                float coverage = _coverage![i] / 65535f;
                coverage += (1 - coverage) * a;
                _coverage[i] = (ushort)(coverage * 65535);
                uint basePixel = _baseColor![i];
                float r = sr, g = sg, b = sb, pa = sa;
                if (Source != null)
                {
                    ColorMath.Unpack(Source(x + Offset.X, y + Offset.Y), out r, out g, out b, out pa);
                }
                pixels[y * pixelStride + x] = BlendModes.CompositePixel(basePixel, r, g, b, coverage * Opacity * pa, BlendMode, LockTransparency);
            }
        }
    }

    /// <summary>Returns the stroke coverage as a document-sized mask (used by healing tools).</summary>
    public PixelSurface GetCoverageMask(int documentWidth, int documentHeight)
    {
        PixelSurface mask = PixelSurface.CreateMask(documentWidth, documentHeight);
        if (_coverage == null) return mask;
        unsafe
        {
            byte* m = (byte*)mask.Bitmap.GetPixels();
            for (int y = 0; y < Target.Height; y++)
            {
                int dy = y + Offset.Y;
                if (dy < 0 || dy >= documentHeight) continue;
                for (int x = 0; x < Target.Width; x++)
                {
                    int dx = x + Offset.X;
                    if (dx < 0 || dx >= documentWidth) continue;
                    ushort c = _coverage[y * Target.Width + x];
                    if (c > 0) m[dy * mask.RowBytes + dx] = (byte)(c >> 8);
                }
            }
        }
        return mask;
    }

    /// <summary>Returns the stroke coverage as a mask in target coordinates.</summary>
    public PixelSurface GetLocalCoverageMask(byte threshold = 0)
    {
        PixelSurface mask = PixelSurface.CreateMask(Target.Width, Target.Height);
        if (_coverage == null) return mask;
        Span<byte> span = mask.GetSpan();
        for (int y = 0; y < Target.Height; y++)
        {
            for (int x = 0; x < Target.Width; x++)
            {
                int c = _coverage[y * Target.Width + x] >> 8;
                if (c > threshold) span[y * mask.RowBytes + x] = threshold > 0 ? (byte)255 : (byte)c;
            }
        }
        return mask;
    }

    /// <summary>Creates a bitmap of the target as it was before the stroke (color targets in accumulate mode).</summary>
    public unsafe SKBitmap CreateBaseBitmap()
    {
        SKBitmap bitmap = ImageOps.CreateBitmap(Target.Width, Target.Height);
        if (_baseColor == null) return bitmap;
        uint* p = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        for (int y = 0; y < Target.Height; y++)
            for (int x = 0; x < Target.Width; x++)
                p[y * stride + x] = _baseColor[y * Target.Width + x];
        return bitmap;
    }

    /// <summary>Restores the pre-stroke pixels inside an area (target coordinates).</summary>
    public unsafe void RestoreBase(SKRectI area)
    {
        if (_baseColor == null) return;
        area.Intersect(new SKRectI(0, 0, Target.Width, Target.Height));
        uint* p = (uint*)Target.Bitmap.GetPixels();
        int stride = Target.RowBytes / 4;
        for (int y = area.Top; y < area.Bottom; y++)
            for (int x = area.Left; x < area.Right; x++)
                p[y * stride + x] = _baseColor[y * Target.Width + x];
    }

    /// <summary>Original pixel before the stroke started (accumulate mode only).</summary>
    public uint BasePixel(int x, int y) => _baseColor != null && x >= 0 && y >= 0 && x < Target.Width && y < Target.Height ? _baseColor[y * Target.Width + x] : 0;

    public void Dispose()
    {
    }
}

/// <summary>Pixel processors used by retouching brushes in direct mode.</summary>
public static class DabProcessors
{
    private static unsafe uint* Pixels(BrushStroke s, out int stride)
    {
        stride = s.Target.RowBytes / 4;
        return (uint*)s.Target.Bitmap.GetPixels();
    }

    /// <summary>Blur: blends each pixel toward its 3x3 neighborhood average.</summary>
    public static DabProcessor Blur(float strength) => (stroke, rect, alpha, center) =>
    {
        unsafe
        {
            uint* p = Pixels(stroke, out int stride);
            int w = stroke.Target.Width, h = stroke.Target.Height;
            uint[] copy = new uint[rect.Width * rect.Height];
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    long a = 0, r = 0, g = 0, b = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            uint q = p[Math.Clamp(y + dy, 0, h - 1) * stride + Math.Clamp(x + dx, 0, w - 1)];
                            a += q >> 24; r += (q >> 16) & 0xFF; g += (q >> 8) & 0xFF; b += q & 0xFF;
                        }
                    }
                    copy[(y - rect.Top) * rect.Width + x - rect.Left] = (uint)(a / 9) << 24 | (uint)(r / 9) << 16 | (uint)(g / 9) << 8 | (uint)(b / 9);
                }
            }
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    int i = (y - rect.Top) * rect.Width + x - rect.Left;
                    float t = alpha[i] * strength;
                    if (t <= 0) continue;
                    if (stroke.LockTransparency) t *= (p[y * stride + x] >> 24) / 255f;
                    p[y * stride + x] = ColorMath.LerpPremul(p[y * stride + x], copy[i], t);
                }
            }
        }
    };

    /// <summary>Sharpen: pushes each pixel away from its neighborhood average.</summary>
    public static DabProcessor Sharpen(float strength, bool protectDetail) => (stroke, rect, alpha, center) =>
    {
        unsafe
        {
            uint* p = Pixels(stroke, out int stride);
            int w = stroke.Target.Width, h = stroke.Target.Height;
            uint[] result = new uint[rect.Width * rect.Height];
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    int i = (y - rect.Top) * rect.Width + x - rect.Left;
                    float t = alpha[i] * strength;
                    uint c = p[y * stride + x];
                    if (t <= 0 || (c >> 24) == 0) { result[i] = c; continue; }
                    float ar = 0, ag = 0, ab = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            ColorMath.Unpack(p[Math.Clamp(y + dy, 0, h - 1) * stride + Math.Clamp(x + dx, 0, w - 1)], out float nr, out float ng, out float nb, out _);
                            ar += nr; ag += ng; ab += nb;
                        }
                    }
                    ColorMath.Unpack(c, out float r, out float g, out float b, out float a);
                    float k = protectDetail ? 0.5f : 1f;
                    r += (r - ar / 9) * t * k; g += (g - ag / 9) * t * k; b += (b - ab / 9) * t * k;
                    result[i] = ColorMath.PackUnpremul(r, g, b, a);
                }
            }
            for (int y = rect.Top; y < rect.Bottom; y++)
                for (int x = rect.Left; x < rect.Right; x++)
                    p[y * stride + x] = result[(y - rect.Top) * rect.Width + x - rect.Left];
        }
    };

    /// <summary>Smudge: drags previously picked-up color along the stroke.</summary>
    public sealed class Smudge
    {
        private uint[]? _carry;
        private int _carryW, _carryH;
        private readonly float _strength;
        private readonly SKColor? _fingerColor;

        public Smudge(float strength, SKColor? fingerPaintingColor)
        {
            _strength = strength;
            _fingerColor = fingerPaintingColor;
        }

        public unsafe void Process(BrushStroke stroke, SKRectI rect, float[] alpha, SKPoint center)
        {
            uint* p = Pixels(stroke, out int stride);
            int w = rect.Width, h = rect.Height;
            int size = (int)MathF.Ceiling(stroke.Size) + 4;
            if (_carry == null)
            {
                _carryW = _carryH = size;
                _carry = new uint[size * size];
                uint init = _fingerColor is SKColor fc ? ColorMath.FromColor(fc) : 0;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        int sx = (int)(center.X - size / 2f) + x, sy = (int)(center.Y - size / 2f) + y;
                        bool inside = sx >= 0 && sy >= 0 && sx < stroke.Target.Width && sy < stroke.Target.Height;
                        _carry[y * size + x] = _fingerColor != null ? init : inside ? p[sy * stride + sx] : 0;
                    }
                }
                return;
            }
            int originX = (int)(center.X - _carryW / 2f), originY = (int)(center.Y - _carryH / 2f);
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    int cx = x - originX, cy = y - originY;
                    if (cx < 0 || cy < 0 || cx >= _carryW || cy >= _carryH) continue;
                    float t = alpha[(y - rect.Top) * w + x - rect.Left] * _strength;
                    if (t <= 0) continue;
                    uint current = p[y * stride + x];
                    uint carried = _carry[cy * _carryW + cx];
                    uint mixed = ColorMath.LerpPremul(current, carried, t);
                    if (stroke.LockTransparency) mixed = ColorMath.PackUnpremul(Ch(mixed, 16), Ch(mixed, 8), Ch(mixed, 0), (current >> 24) / 255f);
                    p[y * stride + x] = mixed;
                    // The finger picks up some of the new color.
                    _carry[cy * _carryW + cx] = ColorMath.LerpPremul(carried, mixed, 1 - _strength);
                }
            }
            _ = h;
        }

        private static float Ch(uint p, int shift)
        {
            ColorMath.Unpack(p, out float r, out float g, out float b, out _);
            return shift == 16 ? r : shift == 8 ? g : b;
        }
    }

    public enum ToneRange { Shadows, Midtones, Highlights }

    /// <summary>Dodge (lighten) or burn (darken) restricted to a tonal range.</summary>
    public static DabProcessor DodgeBurn(bool dodge, ToneRange range, float exposure, bool protectTones) => (stroke, rect, alpha, center) =>
    {
        unsafe
        {
            uint* p = Pixels(stroke, out int stride);
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    float t = alpha[(y - rect.Top) * rect.Width + x - rect.Left] * exposure;
                    if (t <= 0) continue;
                    uint c = p[y * stride + x];
                    if ((c >> 24) == 0) continue;
                    ColorMath.Unpack(c, out float r, out float g, out float b, out float a);
                    float l = ColorMath.Luminance(r, g, b);
                    float weight = range switch
                    {
                        ToneRange.Shadows => Math.Clamp(1 - l * 2, 0, 1),
                        ToneRange.Highlights => Math.Clamp(l * 2 - 1, 0, 1),
                        _ => 1 - MathF.Abs(l - 0.5f) * 2
                    };
                    float k = t * weight * 0.25f;
                    if (dodge)
                    {
                        r += (1 - r) * k; g += (1 - g) * k; b += (1 - b) * k;
                    }
                    else
                    {
                        r -= r * k; g -= g * k; b -= b * k;
                    }
                    if (protectTones)
                    {
                        // Keep the original saturation to avoid hue shifts.
                        ColorMath.RgbToHsl(r, g, b, out float nh, out _, out float nl);
                        ColorMath.RgbToHsl(ColorMath.Unpack3(c).r, ColorMath.Unpack3(c).g, ColorMath.Unpack3(c).b, out _, out float os, out _);
                        ColorMath.HslToRgb(nh, os, nl, out r, out g, out b);
                    }
                    p[y * stride + x] = ColorMath.PackUnpremul(r, g, b, a);
                }
            }
        }
    };

    /// <summary>Sponge: saturates or desaturates.</summary>
    public static DabProcessor Sponge(bool saturate, float flow, bool vibrance) => (stroke, rect, alpha, center) =>
    {
        unsafe
        {
            uint* p = Pixels(stroke, out int stride);
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    float t = alpha[(y - rect.Top) * rect.Width + x - rect.Left] * flow * 0.2f;
                    if (t <= 0) continue;
                    uint c = p[y * stride + x];
                    if ((c >> 24) == 0) continue;
                    ColorMath.Unpack(c, out float r, out float g, out float b, out float a);
                    ColorMath.RgbToHsl(r, g, b, out float h, out float s, out float l);
                    if (vibrance) t *= saturate ? 1 - s : s;
                    s = saturate ? s + (1 - s) * t : s * (1 - t);
                    ColorMath.HslToRgb(h, s, l, out r, out g, out b);
                    p[y * stride + x] = ColorMath.PackUnpremul(r, g, b, a);
                }
            }
        }
    };

    public enum ReplaceMode { Hue, Saturation, Color, Luminosity }

    /// <summary>Color replacement: recolors pixels similar to the sampled color.</summary>
    public sealed class ColorReplace
    {
        private readonly SKColor _color;
        private readonly ReplaceMode _mode;
        private readonly int _tolerance;
        private readonly bool _sampleContinuous;
        private uint? _sample;

        public ColorReplace(SKColor color, ReplaceMode mode, int tolerance, bool sampleContinuous)
        {
            _color = color;
            _mode = mode;
            _tolerance = tolerance;
            _sampleContinuous = sampleContinuous;
        }

        public unsafe void Process(BrushStroke stroke, SKRectI rect, float[] alpha, SKPoint center)
        {
            uint* p = Pixels(stroke, out int stride);
            int cx = Math.Clamp((int)center.X, 0, stroke.Target.Width - 1), cy = Math.Clamp((int)center.Y, 0, stroke.Target.Height - 1);
            if (_sample == null || _sampleContinuous) _sample = p[cy * stride + cx];
            ColorMath.RgbToHsl(_color.Red / 255f, _color.Green / 255f, _color.Blue / 255f, out float th, out float ts, out float tl);
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    float t = alpha[(y - rect.Top) * rect.Width + x - rect.Left];
                    if (t <= 0) continue;
                    uint c = p[y * stride + x];
                    int d = ColorMath.Distance(c, _sample.Value);
                    if (d > _tolerance) continue;
                    t *= Math.Clamp(1 - d / (float)Math.Max(1, _tolerance) * 0.5f, 0, 1);
                    ColorMath.Unpack(c, out float r, out float g, out float b, out float a);
                    ColorMath.RgbToHsl(r, g, b, out float h, out float s, out float l);
                    switch (_mode)
                    {
                        case ReplaceMode.Hue: h = th; break;
                        case ReplaceMode.Saturation: s = ts; break;
                        case ReplaceMode.Luminosity: l = tl; break;
                        default: h = th; s = ts; break;
                    }
                    ColorMath.HslToRgb(h, s, l, out float nr, out float ng, out float nb);
                    p[y * stride + x] = ColorMath.PackUnpremul(r + (nr - r) * t, g + (ng - g) * t, b + (nb - b) * t, a);
                }
            }
        }
    }

    /// <summary>Background eraser: erases pixels similar to the color under the brush hot spot.</summary>
    public sealed class BackgroundEraser
    {
        private readonly int _tolerance;
        private readonly bool _continuous;
        private readonly bool _contiguous;
        private readonly SKColor? _protect;
        private readonly SKColor? _fixedSample;
        private uint? _sample;

        public BackgroundEraser(int tolerance, bool continuous, bool contiguous, SKColor? protectColor, SKColor? fixedSample)
        {
            _tolerance = tolerance;
            _continuous = continuous;
            _contiguous = contiguous;
            _protect = protectColor;
            _fixedSample = fixedSample;
        }

        public unsafe void Process(BrushStroke stroke, SKRectI rect, float[] alpha, SKPoint center)
        {
            uint* p = Pixels(stroke, out int stride);
            int cx = Math.Clamp((int)center.X, 0, stroke.Target.Width - 1), cy = Math.Clamp((int)center.Y, 0, stroke.Target.Height - 1);
            if (_fixedSample is SKColor fixedColor) _sample = ColorMath.FromColor(fixedColor);
            else if (_sample == null || _continuous) _sample = p[cy * stride + cx];
            if ((_sample.Value >> 24) == 0) return;
            uint sample = _sample.Value;
            uint? protect = _protect is SKColor pc ? ColorMath.FromColor(pc) : null;

            int w = rect.Width, h = rect.Height;
            bool[] erase = new bool[w * h];
            bool Similar(int x, int y)
            {
                uint c = p[y * stride + x];
                if ((c >> 24) == 0) return false;
                if (protect != null && ColorMath.Distance(c, protect.Value) <= 8) return false;
                return ColorMath.Distance(c, sample) <= _tolerance;
            }
            if (_contiguous)
            {
                Stack<(int, int)> stack = new();
                bool[] visited = new bool[w * h];
                if (cx >= rect.Left && cy >= rect.Top && cx < rect.Right && cy < rect.Bottom) stack.Push((cx, cy));
                while (stack.Count > 0)
                {
                    var (x, y) = stack.Pop();
                    if (x < rect.Left || y < rect.Top || x >= rect.Right || y >= rect.Bottom) continue;
                    int i = (y - rect.Top) * w + x - rect.Left;
                    if (visited[i]) continue;
                    visited[i] = true;
                    if (alpha[i] <= 0 || !Similar(x, y)) continue;
                    erase[i] = true;
                    stack.Push((x + 1, y)); stack.Push((x - 1, y)); stack.Push((x, y + 1)); stack.Push((x, y - 1));
                }
            }
            else
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                    for (int x = rect.Left; x < rect.Right; x++)
                        erase[(y - rect.Top) * w + x - rect.Left] = Similar(x, y);
            }
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    int i = (y - rect.Top) * w + x - rect.Left;
                    if (!erase[i]) continue;
                    p[y * stride + x] = ImageOps.ScalePixel(p[y * stride + x], (int)((1 - Math.Clamp(alpha[i], 0, 1)) * 255));
                }
            }
            _ = h;
        }
    }

    /// <summary>Mixer brush: mixes a paint reservoir with canvas color.</summary>
    public sealed class Mixer
    {
        private float _r, _g, _b;
        private readonly float _wet, _load, _mix;
        private float _reservoir;
        private readonly bool _sampleAllLayers;

        public Mixer(SKColor load, float wet, float loadAmount, float mix, bool sampleAll)
        {
            _r = load.Red / 255f; _g = load.Green / 255f; _b = load.Blue / 255f;
            _wet = wet; _load = loadAmount; _mix = mix; _reservoir = 1;
            _sampleAllLayers = sampleAll;
        }

        public unsafe void Process(BrushStroke stroke, SKRectI rect, float[] alpha, SKPoint center)
        {
            uint* p = Pixels(stroke, out int stride);
            // Average canvas color under the dab.
            float cr = 0, cg = 0, cb = 0, weight = 0;
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    float t = alpha[(y - rect.Top) * rect.Width + x - rect.Left];
                    if (t <= 0) continue;
                    ColorMath.Unpack(p[y * stride + x], out float r, out float g, out float b, out float a);
                    float wv = t * a;
                    cr += r * wv; cg += g * wv; cb += b * wv; weight += wv;
                }
            }
            if (weight > 0)
            {
                cr /= weight; cg /= weight; cb /= weight;
                float m = _mix * _wet;
                _r += (cr - _r) * m; _g += (cg - _g) * m; _b += (cb - _b) * m;
            }
            _reservoir = Math.Max(0, _reservoir - (1 - _load) * 0.02f);
            float strength = Math.Max(0.15f, _reservoir);
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    float t = alpha[(y - rect.Top) * rect.Width + x - rect.Left] * strength;
                    if (t <= 0) continue;
                    p[y * stride + x] = BlendModes.CompositePixel(p[y * stride + x], _r, _g, _b, t * stroke.Opacity, LayerBlendMode.Normal, stroke.LockTransparency);
                }
            }
            _ = _sampleAllLayers;
        }
    }
}
