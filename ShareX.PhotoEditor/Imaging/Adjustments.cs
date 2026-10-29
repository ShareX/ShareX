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

public enum AdjustmentKind
{
    BrightnessContrast,
    Levels,
    Curves,
    Exposure,
    Vibrance,
    HueSaturation,
    ColorBalance,
    BlackAndWhite,
    PhotoFilter,
    ChannelMixer,
    Invert,
    Posterize,
    Threshold,
    GradientMap,
    SelectiveColor,
    Desaturate
}

/// <summary>Serializable parameters for an adjustment; used by adjustment layers, dialogs and the adjustment brush.</summary>
public sealed class AdjustmentSettings
{
    public AdjustmentSettings(AdjustmentKind kind)
    {
        Kind = kind;
        if (kind == AdjustmentKind.Curves)
        {
            for (int i = 0; i < 4; i++) Curves[i] = [new SKPoint(0, 0), new SKPoint(255, 255)];
        }
        if (kind == AdjustmentKind.BlackAndWhite)
        {
            Values["reds"] = 40; Values["yellows"] = 60; Values["greens"] = 40;
            Values["cyans"] = 60; Values["blues"] = 20; Values["magentas"] = 80;
        }
        if (kind == AdjustmentKind.ChannelMixer)
        {
            Values["r.r"] = 100; Values["g.g"] = 100; Values["b.b"] = 100;
        }
        if (kind == AdjustmentKind.PhotoFilter)
        {
            Color = new SKColor(0xEC, 0x8A, 0x00);
            Values["density"] = 25;
            Values["preserve"] = 1;
        }
        if (kind == AdjustmentKind.Posterize) Values["levels"] = 4;
        if (kind == AdjustmentKind.Threshold) Values["level"] = 128;
        if (kind == AdjustmentKind.ColorBalance) Values["preserve"] = 1;
        if (kind == AdjustmentKind.Exposure) Values["gamma"] = 1;
        if (kind == AdjustmentKind.Levels)
        {
            foreach (string ch in LevelsChannels)
            {
                Values[$"{ch}.inWhite"] = 255; Values[$"{ch}.gamma"] = 1; Values[$"{ch}.outWhite"] = 255;
            }
        }
    }

    public static readonly string[] LevelsChannels = ["rgb", "r", "g", "b"];

    public AdjustmentKind Kind { get; set; }

    public Dictionary<string, float> Values { get; set; } = [];

    /// <summary>Curves for RGB, red, green and blue in 0..255 space.</summary>
    public List<SKPoint>[] Curves { get; set; } = new List<SKPoint>[4];

    public SKColor Color { get; set; } = SKColors.Gray;

    public GradientDefinition? Gradient { get; set; }

    public float Get(string key, float defaultValue = 0) => Values.TryGetValue(key, out float value) ? value : defaultValue;

    public void Set(string key, float value) => Values[key] = value;

    public AdjustmentSettings Clone() => new(Kind)
    {
        Values = new Dictionary<string, float>(Values),
        Curves = Curves.Select(c => c?.ToList() ?? []).ToArray(),
        Color = Color,
        Gradient = Gradient?.Clone()
    };

    public static string GetName(AdjustmentKind kind) => kind switch
    {
        AdjustmentKind.BrightnessContrast => "Brightness/Contrast",
        AdjustmentKind.HueSaturation => "Hue/Saturation",
        AdjustmentKind.ColorBalance => "Color Balance",
        AdjustmentKind.BlackAndWhite => "Black & White",
        AdjustmentKind.PhotoFilter => "Photo Filter",
        AdjustmentKind.ChannelMixer => "Channel Mixer",
        AdjustmentKind.GradientMap => "Gradient Map",
        AdjustmentKind.SelectiveColor => "Selective Color",
        _ => kind.ToString()
    };
}

/// <summary>Executes adjustments on premultiplied BGRA bitmaps.</summary>
public static class Adjustments
{
    /// <summary>
    /// Applies an adjustment to an area of the bitmap. The optional weight function returns 0..1 per bitmap pixel
    /// to blend the adjusted result with the original (used for masks, selections and opacity).
    /// </summary>
    public static unsafe void Apply(AdjustmentSettings settings, SKBitmap bitmap, SKRectI area, Func<int, int, float>? weight = null)
    {
        area.Intersect(new SKRectI(0, 0, bitmap.Width, bitmap.Height));
        if (area.IsEmpty) return;

        PixelFunction function = CreateFunction(settings);
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        int left = area.Left, right = area.Right;

        Parallel.For(area.Top, area.Bottom, y =>
        {
            uint* row = pixels + y * stride;
            for (int x = left; x < right; x++)
            {
                uint p = row[x];
                if ((p >> 24) == 0) continue;
                float w = weight?.Invoke(x, y) ?? 1;
                if (w <= 0) continue;
                ColorMath.Unpack(p, out float r, out float g, out float b, out float a);
                float r0 = r, g0 = g, b0 = b;
                function(ref r, ref g, ref b);
                if (w < 1)
                {
                    r = r0 + (r - r0) * w;
                    g = g0 + (g - g0) * w;
                    b = b0 + (b - b0) * w;
                }
                row[x] = ColorMath.PackUnpremul(r, g, b, a);
            }
        });
    }

    public delegate void PixelFunction(ref float r, ref float g, ref float b);

    public static PixelFunction CreateFunction(AdjustmentSettings s)
    {
        switch (s.Kind)
        {
            case AdjustmentKind.BrightnessContrast:
            {
                float brightness = s.Get("brightness") / 150f;
                float contrast = s.Get("contrast") / 100f;
                bool legacy = s.Get("legacy") > 0;
                float[] lut = new float[256];
                for (int i = 0; i < 256; i++)
                {
                    float v = i / 255f;
                    if (legacy)
                    {
                        v += brightness * 0.5f;
                    }
                    else
                    {
                        // Brightness bends the midtones like a gamma curve so that highlights are not clipped.
                        v = brightness >= 0 ? 1 - MathF.Pow(1 - v, 1 + brightness * 1.5f) : MathF.Pow(v, 1 - brightness * 1.5f);
                    }
                    float factor = contrast >= 0 ? 1 / MathF.Max(0.01f, 1 - contrast * 0.99f) : 1 + contrast;
                    v = (v - 0.5f) * factor + 0.5f;
                    lut[i] = Math.Clamp(v, 0, 1);
                }
                return LutFunction(lut, lut, lut);
            }
            case AdjustmentKind.Levels:
            {
                float[] master = LevelsLut(s, "rgb");
                float[] rl = Chain(LevelsLut(s, "r"), master);
                float[] gl = Chain(LevelsLut(s, "g"), master);
                float[] bl = Chain(LevelsLut(s, "b"), master);
                return LutFunction(rl, gl, bl);
            }
            case AdjustmentKind.Curves:
            {
                float[] master = CurveLut(s.Curves[0]);
                return LutFunction(Chain(master, CurveLut(s.Curves[1])), Chain(master, CurveLut(s.Curves[2])), Chain(master, CurveLut(s.Curves[3])));
            }
            case AdjustmentKind.Exposure:
            {
                float exposure = MathF.Pow(2, s.Get("exposure"));
                float offset = s.Get("offset");
                float gamma = Math.Max(0.01f, s.Get("gamma", 1));
                float[] lut = new float[256];
                for (int i = 0; i < 256; i++)
                {
                    float linear = MathF.Pow(i / 255f, 2.2f);
                    linear = linear * exposure + offset;
                    float v = MathF.Pow(Math.Max(0, linear), 1 / 2.2f);
                    lut[i] = Math.Clamp(MathF.Pow(Math.Clamp(v, 0, 1), 1 / gamma), 0, 1);
                }
                return LutFunction(lut, lut, lut);
            }
            case AdjustmentKind.Vibrance:
            {
                float vibrance = s.Get("vibrance") / 100f;
                float saturation = s.Get("saturation") / 100f;
                return (ref float r, ref float g, ref float b) =>
                {
                    float max = MathF.Max(r, MathF.Max(g, b));
                    float min = MathF.Min(r, MathF.Min(g, b));
                    float sat = max - min;
                    float l = ColorMath.Luminance(r, g, b);
                    // Vibrance boosts less saturated colors more and protects skin-like hues.
                    float skin = r > g && g > b ? 0.5f : 1f;
                    float amount = 1 + vibrance * (1 - sat) * skin + saturation;
                    r = l + (r - l) * amount;
                    g = l + (g - l) * amount;
                    b = l + (b - l) * amount;
                };
            }
            case AdjustmentKind.HueSaturation:
            {
                float hue = s.Get("hue");
                float saturation = s.Get("saturation") / 100f;
                float lightness = s.Get("lightness") / 100f;
                bool colorize = s.Get("colorize") > 0;
                return (ref float r, ref float g, ref float b) =>
                {
                    ColorMath.RgbToHsl(r, g, b, out float h, out float sat, out float l);
                    if (colorize)
                    {
                        h = hue < 0 ? hue + 360 : hue;
                        sat = Math.Clamp(0.25f + saturation * 0.75f, 0, 1);
                    }
                    else
                    {
                        h += hue;
                        sat = saturation >= 0 ? sat + (1 - sat) * saturation * sat * 2f : sat * (1 + saturation);
                        sat = Math.Clamp(sat, 0, 1);
                    }
                    l = lightness >= 0 ? l + (1 - l) * lightness : l * (1 + lightness);
                    ColorMath.HslToRgb(h, sat, l, out r, out g, out b);
                };
            }
            case AdjustmentKind.ColorBalance:
            {
                float[] shadows = [s.Get("shadows.cr") / 100f, s.Get("shadows.mg") / 100f, s.Get("shadows.yb") / 100f];
                float[] mid = [s.Get("midtones.cr") / 100f, s.Get("midtones.mg") / 100f, s.Get("midtones.yb") / 100f];
                float[] high = [s.Get("highlights.cr") / 100f, s.Get("highlights.mg") / 100f, s.Get("highlights.yb") / 100f];
                bool preserve = s.Get("preserve", 1) > 0;
                return (ref float r, ref float g, ref float b) =>
                {
                    float l = ColorMath.Luminance(r, g, b);
                    float ws = Math.Clamp(1 - l * 3, 0, 1);
                    float wh = Math.Clamp((l - 0.66f) * 3, 0, 1);
                    float wm = 1 - ws - wh;
                    float dr = (shadows[0] * ws + mid[0] * wm + high[0] * wh) * 0.4f;
                    float dg = (shadows[1] * ws + mid[1] * wm + high[1] * wh) * 0.4f;
                    float db = (shadows[2] * ws + mid[2] * wm + high[2] * wh) * 0.4f;
                    r += dr; g += dg; b += db;
                    r = Math.Clamp(r, 0, 1); g = Math.Clamp(g, 0, 1); b = Math.Clamp(b, 0, 1);
                    if (preserve)
                    {
                        float nl = ColorMath.Luminance(r, g, b);
                        float d = l - nl;
                        r += d; g += d; b += d;
                    }
                };
            }
            case AdjustmentKind.BlackAndWhite:
            {
                float[] weights = [s.Get("reds", 40), s.Get("yellows", 60), s.Get("greens", 40), s.Get("cyans", 60), s.Get("blues", 20), s.Get("magentas", 80)];
                bool tint = s.Get("tint") > 0;
                SKColor tintColor = s.Color;
                ColorMath.RgbToHsl(tintColor.Red / 255f, tintColor.Green / 255f, tintColor.Blue / 255f, out float th, out float ts, out _);
                return (ref float r, ref float g, ref float b) =>
                {
                    ColorMath.RgbToHsl(r, g, b, out float h, out float sat, out _);
                    float min = MathF.Min(r, MathF.Min(g, b));
                    float lum = ColorMath.Luminance(r, g, b);
                    // Blend the two hue sectors adjacent to this pixel's hue.
                    float sector = h / 60f;
                    int i0 = (int)MathF.Floor(sector) % 6;
                    int i1 = (i0 + 1) % 6;
                    float f = sector - MathF.Floor(sector);
                    float wv = (weights[i0] * (1 - f) + weights[i1] * f) / 100f;
                    float chroma = MathF.Max(r, MathF.Max(g, b)) - min;
                    float gray = Math.Clamp(min + chroma * wv * 1.0f + (lum - min) * 0.0f, 0, 1);
                    if (sat < 0.001f) gray = lum;
                    if (tint)
                    {
                        ColorMath.HslToRgb(th, ts, gray, out r, out g, out b);
                    }
                    else
                    {
                        r = g = b = gray;
                    }
                };
            }
            case AdjustmentKind.PhotoFilter:
            {
                SKColor color = s.Color;
                float density = s.Get("density", 25) / 100f;
                bool preserve = s.Get("preserve", 1) > 0;
                float cr = color.Red / 255f, cg = color.Green / 255f, cb = color.Blue / 255f;
                return (ref float r, ref float g, ref float b) =>
                {
                    float l = ColorMath.Luminance(r, g, b);
                    r = r + (r * cr * 2 - r) * density * 0.5f + (cr - r) * density * 0.25f;
                    g = g + (g * cg * 2 - g) * density * 0.5f + (cg - g) * density * 0.25f;
                    b = b + (b * cb * 2 - b) * density * 0.5f + (cb - b) * density * 0.25f;
                    r = Math.Clamp(r, 0, 1); g = Math.Clamp(g, 0, 1); b = Math.Clamp(b, 0, 1);
                    if (preserve)
                    {
                        float d = l - ColorMath.Luminance(r, g, b);
                        r += d; g += d; b += d;
                    }
                };
            }
            case AdjustmentKind.ChannelMixer:
            {
                float[] m =
                [
                    s.Get("r.r") / 100f, s.Get("r.g") / 100f, s.Get("r.b") / 100f, s.Get("r.c") / 100f,
                    s.Get("g.r") / 100f, s.Get("g.g") / 100f, s.Get("g.b") / 100f, s.Get("g.c") / 100f,
                    s.Get("b.r") / 100f, s.Get("b.g") / 100f, s.Get("b.b") / 100f, s.Get("b.c") / 100f
                ];
                bool mono = s.Get("monochrome") > 0;
                return (ref float r, ref float g, ref float b) =>
                {
                    float nr = r * m[0] + g * m[1] + b * m[2] + m[3];
                    if (mono)
                    {
                        r = g = b = nr;
                        return;
                    }
                    float ng = r * m[4] + g * m[5] + b * m[6] + m[7];
                    float nb = r * m[8] + g * m[9] + b * m[10] + m[11];
                    r = nr; g = ng; b = nb;
                };
            }
            case AdjustmentKind.Invert:
                return (ref float r, ref float g, ref float b) => { r = 1 - r; g = 1 - g; b = 1 - b; };
            case AdjustmentKind.Posterize:
            {
                int levels = Math.Clamp((int)s.Get("levels", 4), 2, 255);
                return (ref float r, ref float g, ref float b) =>
                {
                    r = MathF.Round(r * (levels - 1)) / (levels - 1);
                    g = MathF.Round(g * (levels - 1)) / (levels - 1);
                    b = MathF.Round(b * (levels - 1)) / (levels - 1);
                };
            }
            case AdjustmentKind.Threshold:
            {
                float level = s.Get("level", 128) / 255f;
                return (ref float r, ref float g, ref float b) =>
                {
                    float v = ColorMath.Luminance(r, g, b) >= level ? 1 : 0;
                    r = g = b = v;
                };
            }
            case AdjustmentKind.GradientMap:
            {
                GradientDefinition gradient = s.Gradient ?? GradientDefinition.CreateDefault(SKColors.Black, SKColors.White);
                SKColor[] lut = new SKColor[256];
                for (int i = 0; i < 256; i++) lut[i] = gradient.Sample(i / 255f);
                return (ref float r, ref float g, ref float b) =>
                {
                    SKColor c = lut[Math.Clamp((int)(ColorMath.Luminance(r, g, b) * 255), 0, 255)];
                    r = c.Red / 255f; g = c.Green / 255f; b = c.Blue / 255f;
                };
            }
            case AdjustmentKind.SelectiveColor:
            {
                string[] groups = ["reds", "yellows", "greens", "cyans", "blues", "magentas", "whites", "neutrals", "blacks"];
                float[,] cmyk = new float[groups.Length, 4];
                for (int i = 0; i < groups.Length; i++)
                {
                    cmyk[i, 0] = s.Get($"{groups[i]}.c") / 100f;
                    cmyk[i, 1] = s.Get($"{groups[i]}.m") / 100f;
                    cmyk[i, 2] = s.Get($"{groups[i]}.y") / 100f;
                    cmyk[i, 3] = s.Get($"{groups[i]}.k") / 100f;
                }
                return (ref float r, ref float g, ref float b) =>
                {
                    float max = MathF.Max(r, MathF.Max(g, b));
                    float min = MathF.Min(r, MathF.Min(g, b));
                    float chroma = max - min;
                    ColorMath.RgbToHsl(r, g, b, out float h, out _, out float l);
                    float dr = 0, dg = 0, db = 0;
                    void ApplyGroup(int index, float amount)
                    {
                        if (amount <= 0) return;
                        float k = cmyk[index, 3];
                        dr -= (cmyk[index, 0] + k) * amount;
                        dg -= (cmyk[index, 1] + k) * amount;
                        db -= (cmyk[index, 2] + k) * amount;
                    }
                    int hueGroup = (int)MathF.Round(h / 60f) % 6;
                    ApplyGroup(hueGroup, chroma);
                    ApplyGroup(6, Math.Clamp((min - 0.5f) * 2, 0, 1));
                    ApplyGroup(7, 1 - MathF.Abs(l - 0.5f) * 2 - chroma);
                    ApplyGroup(8, Math.Clamp((0.5f - max) * 2, 0, 1));
                    r = Math.Clamp(r + dr, 0, 1); g = Math.Clamp(g + dg, 0, 1); b = Math.Clamp(b + db, 0, 1);
                };
            }
            case AdjustmentKind.Desaturate:
                return (ref float r, ref float g, ref float b) =>
                {
                    float l = ColorMath.Luminance(r, g, b);
                    r = g = b = l;
                };
            default:
                return (ref float r, ref float g, ref float b) => { };
        }
    }

    private static PixelFunction LutFunction(float[] rl, float[] gl, float[] bl) => (ref float r, ref float g, ref float b) =>
    {
        r = rl[Math.Clamp((int)(r * 255 + 0.5f), 0, 255)];
        g = gl[Math.Clamp((int)(g * 255 + 0.5f), 0, 255)];
        b = bl[Math.Clamp((int)(b * 255 + 0.5f), 0, 255)];
    };

    private static float[] Chain(float[] first, float[] second)
    {
        float[] result = new float[256];
        for (int i = 0; i < 256; i++) result[i] = second[Math.Clamp((int)(first[i] * 255 + 0.5f), 0, 255)];
        return result;
    }

    public static float[] LevelsLut(AdjustmentSettings s, string channel)
    {
        float inBlack = s.Get($"{channel}.inBlack", 0);
        float inWhite = s.Get($"{channel}.inWhite", 255);
        float gamma = Math.Max(0.01f, s.Get($"{channel}.gamma", 1));
        float outBlack = s.Get($"{channel}.outBlack", 0);
        float outWhite = s.Get($"{channel}.outWhite", 255);
        float[] lut = new float[256];
        for (int i = 0; i < 256; i++)
        {
            float v = Math.Clamp((i - inBlack) / Math.Max(1, inWhite - inBlack), 0, 1);
            v = MathF.Pow(v, 1 / gamma);
            lut[i] = Math.Clamp((outBlack + v * (outWhite - outBlack)) / 255f, 0, 1);
        }
        return lut;
    }

    /// <summary>Builds a 256 entry LUT through the curve points using monotone cubic interpolation.</summary>
    public static float[] CurveLut(List<SKPoint>? points)
    {
        float[] lut = new float[256];
        if (points == null || points.Count < 2)
        {
            for (int i = 0; i < 256; i++) lut[i] = i / 255f;
            return lut;
        }
        List<SKPoint> p = points.OrderBy(x => x.X).ToList();
        int n = p.Count;
        float[] dx = new float[n - 1], slope = new float[n - 1], m = new float[n];
        for (int i = 0; i < n - 1; i++)
        {
            dx[i] = Math.Max(1e-3f, p[i + 1].X - p[i].X);
            slope[i] = (p[i + 1].Y - p[i].Y) / dx[i];
        }
        m[0] = slope[0];
        m[n - 1] = slope[n - 2];
        for (int i = 1; i < n - 1; i++)
        {
            m[i] = slope[i - 1] * slope[i] <= 0 ? 0 : (slope[i - 1] + slope[i]) / 2;
        }
        for (int i = 0; i < n - 1; i++)
        {
            if (MathF.Abs(slope[i]) < 1e-6f)
            {
                m[i] = m[i + 1] = 0;
                continue;
            }
            float a = m[i] / slope[i], b = m[i + 1] / slope[i];
            float h = a * a + b * b;
            if (h > 9)
            {
                float t = 3 / MathF.Sqrt(h);
                m[i] = t * a * slope[i];
                m[i + 1] = t * b * slope[i];
            }
        }
        for (int x = 0; x < 256; x++)
        {
            float y;
            if (x <= p[0].X) y = p[0].Y;
            else if (x >= p[n - 1].X) y = p[n - 1].Y;
            else
            {
                int i = 0;
                while (i < n - 2 && x > p[i + 1].X) i++;
                float h = dx[i];
                float t = (x - p[i].X) / h;
                float t2 = t * t, t3 = t2 * t;
                y = (2 * t3 - 3 * t2 + 1) * p[i].Y + (t3 - 2 * t2 + t) * h * m[i] + (-2 * t3 + 3 * t2) * p[i + 1].Y + (t3 - t2) * h * m[i + 1];
            }
            lut[x] = Math.Clamp(y / 255f, 0, 1);
        }
        return lut;
    }

    /// <summary>Computes 256-bin histograms for luminosity, red, green and blue.</summary>
    public static unsafe int[][] Histogram(SKBitmap bitmap, SKRectI? area = null)
    {
        int[][] result = [new int[256], new int[256], new int[256], new int[256]];
        SKRectI r = area ?? new SKRectI(0, 0, bitmap.Width, bitmap.Height);
        r.Intersect(new SKRectI(0, 0, bitmap.Width, bitmap.Height));
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        int step = Math.Max(1, (int)Math.Sqrt((double)r.Width * r.Height / 1_000_000));
        for (int y = r.Top; y < r.Bottom; y += step)
        {
            for (int x = r.Left; x < r.Right; x += step)
            {
                uint p = pixels[y * stride + x];
                if ((p >> 24) == 0) continue;
                ColorMath.Unpack(p, out float cr, out float cg, out float cb, out _);
                int ri = (int)(cr * 255), gi = (int)(cg * 255), bi = (int)(cb * 255);
                result[0][(int)(ColorMath.Luminance(cr, cg, cb) * 255)]++;
                result[1][ri]++;
                result[2][gi]++;
                result[3][bi]++;
            }
        }
        return result;
    }

    /// <summary>Creates levels settings that clip a small percentage of the darkest and brightest pixels.</summary>
    public static AdjustmentSettings AutoLevels(SKBitmap bitmap, bool perChannel, float clip = 0.001f)
    {
        AdjustmentSettings settings = new(AdjustmentKind.Levels);
        int[][] histogram = Histogram(bitmap);
        (float low, float high) Find(int[] h)
        {
            long total = h.Sum(x => (long)x);
            if (total == 0) return (0, 255);
            long cutoff = (long)(total * clip);
            long acc = 0;
            int lo = 0;
            for (; lo < 255; lo++) { acc += h[lo]; if (acc > cutoff) break; }
            acc = 0;
            int hi = 255;
            for (; hi > 0; hi--) { acc += h[hi]; if (acc > cutoff) break; }
            if (hi <= lo) return (0, 255);
            return (lo, hi);
        }
        if (perChannel)
        {
            string[] channels = ["r", "g", "b"];
            for (int i = 0; i < 3; i++)
            {
                var (lo, hi) = Find(histogram[i + 1]);
                settings.Set($"{channels[i]}.inBlack", lo);
                settings.Set($"{channels[i]}.inWhite", hi);
            }
        }
        else
        {
            int[] combined = new int[256];
            for (int i = 0; i < 256; i++) combined[i] = histogram[1][i] + histogram[2][i] + histogram[3][i];
            var (lo, hi) = Find(combined);
            settings.Set("rgb.inBlack", lo);
            settings.Set("rgb.inWhite", hi);
        }
        return settings;
    }

    /// <summary>Histogram equalization of luminosity, preserving hue.</summary>
    public static unsafe void Equalize(SKBitmap bitmap, SKRectI area, Func<int, int, float>? weight = null)
    {
        int[] histogram = Histogram(bitmap, area)[0];
        long total = histogram.Sum(x => (long)x);
        if (total == 0) return;
        float[] cdf = new float[256];
        long acc = 0;
        for (int i = 0; i < 256; i++)
        {
            acc += histogram[i];
            cdf[i] = acc / (float)total;
        }
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        for (int y = area.Top; y < area.Bottom; y++)
        {
            for (int x = area.Left; x < area.Right; x++)
            {
                uint p = pixels[y * stride + x];
                if ((p >> 24) == 0) continue;
                float w = weight?.Invoke(x, y) ?? 1;
                if (w <= 0) continue;
                ColorMath.Unpack(p, out float r, out float g, out float b, out float a);
                float l = ColorMath.Luminance(r, g, b);
                float target = cdf[Math.Clamp((int)(l * 255), 0, 255)];
                float d = (target - l) * w;
                pixels[y * stride + x] = ColorMath.PackUnpremul(r + d, g + d, b + d, a);
            }
        }
    }
}
