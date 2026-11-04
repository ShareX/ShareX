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

namespace ShareX.PhotoEditor.Core;

public enum LayerBlendMode
{
    Normal,
    Dissolve,
    Darken,
    Multiply,
    ColorBurn,
    LinearBurn,
    DarkerColor,
    Lighten,
    Screen,
    ColorDodge,
    LinearDodge,
    LighterColor,
    Overlay,
    SoftLight,
    HardLight,
    VividLight,
    LinearLight,
    PinLight,
    HardMix,
    Difference,
    Exclusion,
    Subtract,
    Divide,
    Hue,
    Saturation,
    Color,
    Luminosity,
    PassThrough,
    Behind,
    Clear
}

/// <summary>Blend mode math shared by the compositor and the brush engine.</summary>
public static class BlendModes
{
    public static readonly LayerBlendMode[][] MenuGroups =
    [
        [LayerBlendMode.Normal, LayerBlendMode.Dissolve],
        [LayerBlendMode.Darken, LayerBlendMode.Multiply, LayerBlendMode.ColorBurn, LayerBlendMode.LinearBurn, LayerBlendMode.DarkerColor],
        [LayerBlendMode.Lighten, LayerBlendMode.Screen, LayerBlendMode.ColorDodge, LayerBlendMode.LinearDodge, LayerBlendMode.LighterColor],
        [LayerBlendMode.Overlay, LayerBlendMode.SoftLight, LayerBlendMode.HardLight, LayerBlendMode.VividLight, LayerBlendMode.LinearLight, LayerBlendMode.PinLight, LayerBlendMode.HardMix],
        [LayerBlendMode.Difference, LayerBlendMode.Exclusion, LayerBlendMode.Subtract, LayerBlendMode.Divide],
        [LayerBlendMode.Hue, LayerBlendMode.Saturation, LayerBlendMode.Color, LayerBlendMode.Luminosity]
    ];

    public static IReadOnlyList<LayerBlendMode> LayerModes { get; } = MenuGroups.SelectMany(x => x).ToArray();

    /// <summary>Modes offered by painting tools, which add Behind and Clear.</summary>
    public static IReadOnlyList<LayerBlendMode> PaintModes { get; } =
        new[] { LayerBlendMode.Normal, LayerBlendMode.Dissolve, LayerBlendMode.Behind, LayerBlendMode.Clear }
        .Concat(MenuGroups.Skip(1).SelectMany(x => x)).ToArray();

    public static string GetName(LayerBlendMode mode) => mode switch
    {
        LayerBlendMode.ColorBurn => "Color Burn",
        LayerBlendMode.LinearBurn => "Linear Burn",
        LayerBlendMode.DarkerColor => "Darker Color",
        LayerBlendMode.ColorDodge => "Color Dodge",
        LayerBlendMode.LinearDodge => "Linear Dodge (Add)",
        LayerBlendMode.LighterColor => "Lighter Color",
        LayerBlendMode.SoftLight => "Soft Light",
        LayerBlendMode.HardLight => "Hard Light",
        LayerBlendMode.VividLight => "Vivid Light",
        LayerBlendMode.LinearLight => "Linear Light",
        LayerBlendMode.PinLight => "Pin Light",
        LayerBlendMode.HardMix => "Hard Mix",
        LayerBlendMode.PassThrough => "Pass Through",
        _ => mode.ToString()
    };

    /// <summary>Returns the native Skia equivalent, or null when the mode is computed on the CPU. Linear Dodge is computed on the CPU because Skia's Plus adds the source alpha-weighted, which differs from blending then mixing at partial opacity.</summary>
    public static SKBlendMode? ToSkia(LayerBlendMode mode) => mode switch
    {
        LayerBlendMode.Normal or LayerBlendMode.PassThrough => SKBlendMode.SrcOver,
        LayerBlendMode.Darken => SKBlendMode.Darken,
        LayerBlendMode.Multiply => SKBlendMode.Multiply,
        LayerBlendMode.ColorBurn => SKBlendMode.ColorBurn,
        LayerBlendMode.Lighten => SKBlendMode.Lighten,
        LayerBlendMode.Screen => SKBlendMode.Screen,
        LayerBlendMode.ColorDodge => SKBlendMode.ColorDodge,
        LayerBlendMode.Overlay => SKBlendMode.Overlay,
        LayerBlendMode.SoftLight => SKBlendMode.SoftLight,
        LayerBlendMode.HardLight => SKBlendMode.HardLight,
        LayerBlendMode.Difference => SKBlendMode.Difference,
        LayerBlendMode.Exclusion => SKBlendMode.Exclusion,
        LayerBlendMode.Hue => SKBlendMode.Hue,
        LayerBlendMode.Saturation => SKBlendMode.Saturation,
        LayerBlendMode.Color => SKBlendMode.Color,
        LayerBlendMode.Luminosity => SKBlendMode.Luminosity,
        LayerBlendMode.Behind => SKBlendMode.DstOver,
        LayerBlendMode.Clear => SKBlendMode.DstOut,
        _ => null
    };

    private static float Lum(float r, float g, float b) => 0.3f * r + 0.59f * g + 0.11f * b;

    private static void ClipColor(ref float r, ref float g, ref float b)
    {
        float l = Lum(r, g, b);
        float n = MathF.Min(r, MathF.Min(g, b));
        float x = MathF.Max(r, MathF.Max(g, b));
        if (n < 0)
        {
            float d = l - n;
            if (d > 1e-6f) { r = l + (r - l) * l / d; g = l + (g - l) * l / d; b = l + (b - l) * l / d; }
        }
        if (x > 1)
        {
            float d = x - l;
            if (d > 1e-6f) { r = l + (r - l) * (1 - l) / d; g = l + (g - l) * (1 - l) / d; b = l + (b - l) * (1 - l) / d; }
        }
    }

    private static void SetLum(ref float r, ref float g, ref float b, float l)
    {
        float d = l - Lum(r, g, b);
        r += d; g += d; b += d;
        ClipColor(ref r, ref g, ref b);
    }

    private static float Sat(float r, float g, float b) => MathF.Max(r, MathF.Max(g, b)) - MathF.Min(r, MathF.Min(g, b));

    private static void SetSat(ref float r, ref float g, ref float b, float s)
    {
        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float range = max - min;
        r = range > 1e-6f ? (r - min) * s / range : 0;
        g = range > 1e-6f ? (g - min) * s / range : 0;
        b = range > 1e-6f ? (b - min) * s / range : 0;
    }

    public static float Separable(LayerBlendMode mode, float cb, float cs)
    {
        switch (mode)
        {
            case LayerBlendMode.Multiply: return cb * cs;
            case LayerBlendMode.Screen: return cb + cs - cb * cs;
            case LayerBlendMode.Overlay: return Separable(LayerBlendMode.HardLight, cs, cb);
            case LayerBlendMode.Darken: return MathF.Min(cb, cs);
            case LayerBlendMode.Lighten: return MathF.Max(cb, cs);
            case LayerBlendMode.ColorDodge:
                if (cb <= 0) return 0;
                return cs >= 1 ? 1 : MathF.Min(1, cb / (1 - cs));
            case LayerBlendMode.ColorBurn:
                if (cb >= 1) return 1;
                return cs <= 0 ? 0 : 1 - MathF.Min(1, (1 - cb) / cs);
            case LayerBlendMode.HardLight:
                return cs <= 0.5f ? cb * 2 * cs : Separable(LayerBlendMode.Screen, cb, 2 * cs - 1);
            case LayerBlendMode.SoftLight:
                if (cs <= 0.5f) return cb - (1 - 2 * cs) * cb * (1 - cb);
                float d = cb <= 0.25f ? ((16 * cb - 12) * cb + 4) * cb : MathF.Sqrt(cb);
                return cb + (2 * cs - 1) * (d - cb);
            case LayerBlendMode.Difference: return MathF.Abs(cb - cs);
            case LayerBlendMode.Exclusion: return cb + cs - 2 * cb * cs;
            case LayerBlendMode.LinearBurn: return MathF.Max(0, cb + cs - 1);
            case LayerBlendMode.LinearDodge: return MathF.Min(1, cb + cs);
            case LayerBlendMode.VividLight:
                return cs <= 0.5f ? Separable(LayerBlendMode.ColorBurn, cb, 2 * cs) : Separable(LayerBlendMode.ColorDodge, cb, 2 * cs - 1);
            case LayerBlendMode.LinearLight: return Math.Clamp(cb + 2 * cs - 1, 0, 1);
            case LayerBlendMode.PinLight:
                return cs <= 0.5f ? MathF.Min(cb, 2 * cs) : MathF.Max(cb, 2 * cs - 1);
            case LayerBlendMode.HardMix: return cb + cs >= 1 ? 1 : 0;
            case LayerBlendMode.Subtract: return MathF.Max(0, cb - cs);
            case LayerBlendMode.Divide: return cs <= 0 ? (cb > 0 ? 1 : 0) : MathF.Min(1, cb / cs);
            default: return cs;
        }
    }

    /// <summary>Computes B(Cb, Cs) for unpremultiplied colors in the 0..1 range. The result replaces the source color.</summary>
    public static void Blend(LayerBlendMode mode, float br, float bg, float bb, ref float sr, ref float sg, ref float sb)
    {
        switch (mode)
        {
            case LayerBlendMode.Normal:
            case LayerBlendMode.Dissolve:
            case LayerBlendMode.PassThrough:
            case LayerBlendMode.Behind:
            case LayerBlendMode.Clear:
                return;
            case LayerBlendMode.DarkerColor:
                if (Lum(br, bg, bb) < Lum(sr, sg, sb)) { sr = br; sg = bg; sb = bb; }
                return;
            case LayerBlendMode.LighterColor:
                if (Lum(br, bg, bb) > Lum(sr, sg, sb)) { sr = br; sg = bg; sb = bb; }
                return;
            case LayerBlendMode.Hue:
            {
                float r = sr, g = sg, b = sb;
                SetSat(ref r, ref g, ref b, Sat(br, bg, bb));
                SetLum(ref r, ref g, ref b, Lum(br, bg, bb));
                sr = r; sg = g; sb = b;
                return;
            }
            case LayerBlendMode.Saturation:
            {
                float r = br, g = bg, b = bb;
                SetSat(ref r, ref g, ref b, Sat(sr, sg, sb));
                SetLum(ref r, ref g, ref b, Lum(br, bg, bb));
                sr = r; sg = g; sb = b;
                return;
            }
            case LayerBlendMode.Color:
            {
                float r = sr, g = sg, b = sb;
                SetLum(ref r, ref g, ref b, Lum(br, bg, bb));
                sr = r; sg = g; sb = b;
                return;
            }
            case LayerBlendMode.Luminosity:
            {
                float r = br, g = bg, b = bb;
                SetLum(ref r, ref g, ref b, Lum(sr, sg, sb));
                sr = r; sg = g; sb = b;
                return;
            }
            default:
                sr = Separable(mode, br, sr);
                sg = Separable(mode, bg, sg);
                sb = Separable(mode, bb, sb);
                return;
        }
    }

    /// <summary>
    /// Composites an unpremultiplied source color with alpha <paramref name="sa"/> onto a premultiplied BGRA destination pixel.
    /// </summary>
    public static uint CompositePixel(uint dst, float sr, float sg, float sb, float sa, LayerBlendMode mode, bool preserveAlpha = false)
    {
        if (sa <= 0) return dst;
        if (sa > 1) sa = 1;

        float da = (dst >> 24) / 255f;

        if (mode == LayerBlendMode.Clear)
        {
            if (preserveAlpha) return dst;
            float keep = 1 - sa;
            return Pack(((dst >> 16) & 0xFF) / 255f * keep, ((dst >> 8) & 0xFF) / 255f * keep, (dst & 0xFF) / 255f * keep, da * keep);
        }

        float dr = 0, dg = 0, db = 0;
        if (da > 0)
        {
            dr = MathF.Min(1, ((dst >> 16) & 0xFF) / 255f / da);
            dg = MathF.Min(1, ((dst >> 8) & 0xFF) / 255f / da);
            db = MathF.Min(1, (dst & 0xFF) / 255f / da);
        }

        if (mode == LayerBlendMode.Behind)
        {
            if (preserveAlpha) return dst;
            // Paint only where the destination is transparent: destination-over.
            float oa2 = da + sa * (1 - da);
            return Pack(dr * da + sr * sa * (1 - da), dg * da + sg * sa * (1 - da), db * da + sb * sa * (1 - da), oa2);
        }

        if (mode == LayerBlendMode.Dissolve)
        {
            mode = LayerBlendMode.Normal;
            sa = Random.Shared.NextSingle() < sa ? 1 : 0;
            if (sa <= 0) return dst;
        }

        float br = sr, bg = sg, bb = sb;
        if (mode != LayerBlendMode.Normal && da > 0)
        {
            Blend(mode, dr, dg, db, ref br, ref bg, ref bb);
        }

        // W3C compositing: Cs' = (1 - ab) * Cs + ab * B(Cb, Cs), then source-over.
        float mr = (1 - da) * sr + da * br;
        float mg = (1 - da) * sg + da * bg;
        float mb = (1 - da) * sb + da * bb;

        float oa = sa + da * (1 - sa);
        float pr = mr * sa + dr * da * (1 - sa);
        float pg = mg * sa + dg * da * (1 - sa);
        float pb = mb * sa + db * da * (1 - sa);

        if (preserveAlpha)
        {
            if (da <= 0) return dst;
            float k = oa > 0 ? da / oa : 0;
            pr *= k; pg *= k; pb *= k; oa = da;
        }

        return Pack(pr, pg, pb, oa);
    }

    public static uint Pack(float pr, float pg, float pb, float a)
    {
        int A = Math.Clamp((int)(a * 255f + 0.5f), 0, 255);
        int R = Math.Clamp((int)(pr * 255f + 0.5f), 0, A);
        int G = Math.Clamp((int)(pg * 255f + 0.5f), 0, A);
        int B = Math.Clamp((int)(pb * 255f + 0.5f), 0, A);
        return ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | (uint)B;
    }

    /// <summary>Blends a premultiplied source layer region onto a premultiplied destination using CPU math.</summary>
    public static unsafe void BlendBitmaps(SKBitmap destination, SKBitmap source, LayerBlendMode mode, float opacity, SKRectI area, bool preserveAlpha = false)
    {
        area.Intersect(new SKRectI(0, 0, destination.Width, destination.Height));
        area.Intersect(new SKRectI(0, 0, source.Width, source.Height));
        if (area.IsEmpty || opacity <= 0) return;

        uint* dst = (uint*)destination.GetPixels();
        uint* src = (uint*)source.GetPixels();
        int dstStride = destination.RowBytes / 4;
        int srcStride = source.RowBytes / 4;
        int left = area.Left, right = area.Right;

        Parallel.For(area.Top, area.Bottom, y =>
        {
            uint* d = dst + y * dstStride;
            uint* s = src + y * srcStride;
            for (int x = left; x < right; x++)
            {
                uint sp = s[x];
                uint sAlpha = sp >> 24;
                if (sAlpha == 0) continue;
                float a = sAlpha / 255f;
                float r = MathF.Min(1, ((sp >> 16) & 0xFF) / 255f / a);
                float g = MathF.Min(1, ((sp >> 8) & 0xFF) / 255f / a);
                float b = MathF.Min(1, (sp & 0xFF) / 255f / a);
                float sa = a * opacity;
                LayerBlendMode pixelMode = mode;
                if (mode == LayerBlendMode.Dissolve)
                {
                    // Deterministic per-pixel noise so partial re-renders do not flicker.
                    uint h = (uint)(x * 73856093) ^ (uint)(y * 19349663);
                    h ^= h >> 13;
                    h *= 0x5bd1e995;
                    sa = (h & 0xFFFF) / 65535f < sa ? 1 : 0;
                    pixelMode = LayerBlendMode.Normal;
                }
                d[x] = CompositePixel(d[x], r, g, b, sa, pixelMode, preserveAlpha);
            }
        });
    }
}
