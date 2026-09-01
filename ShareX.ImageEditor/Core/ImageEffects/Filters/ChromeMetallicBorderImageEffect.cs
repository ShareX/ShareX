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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class ChromeMetallicBorderImageEffect : ImageEffectBase
{
    public override string Id => "chrome_metallic_border";
    public override string Name => "Chrome metallic border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sparkles;
    public override string Description => "Adds a shiny chrome metallic border with realistic specular highlights and reflections.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ChromeMetallicBorderImageEffect>("border_size", "Border size", 4, 300, 36, (e, v) => e.BorderSize = v),
        EffectParameters.Color<ChromeMetallicBorderImageEffect>("tint_color", "Tint color", new SKColor(200, 205, 210), (e, v) => e.TintColor = v),
        EffectParameters.FloatSlider<ChromeMetallicBorderImageEffect>("reflectivity", "Reflectivity", 0, 100, 80, (e, v) => e.Reflectivity = v),
        EffectParameters.FloatSlider<ChromeMetallicBorderImageEffect>("bevel_strength", "Bevel strength", 0, 100, 70, (e, v) => e.BevelStrength = v),
        EffectParameters.Bool<ChromeMetallicBorderImageEffect>("inner_line", "Inner line", true, (e, v) => e.InnerLine = v),
        EffectParameters.Bool<ChromeMetallicBorderImageEffect>("outer_line", "Outer line", true, (e, v) => e.OuterLine = v)
    ];

    public int BorderSize { get; set; } = 36;
    public SKColor TintColor { get; set; } = new SKColor(200, 205, 210);
    public float Reflectivity { get; set; } = 80f;
    public float BevelStrength { get; set; } = 70f;
    public bool InnerLine { get; set; } = true;
    public bool OuterLine { get; set; } = true;

    // Chrome gradient stops (value at crossRatio 0..1, outer to inner)
    // Classic chrome banding pattern
    private static readonly (float pos, float value)[] ChromeStops =
    [
        (0.00f, 0.15f),  // outer dark edge
        (0.10f, 0.80f),  // bright highlight
        (0.22f, 0.48f),  // mid silver
        (0.38f, 0.12f),  // dark band
        (0.52f, 0.72f),  // main highlight
        (0.68f, 0.90f),  // bright peak
        (0.80f, 0.55f),  // mid
        (0.90f, 0.22f),  // shadow
        (1.00f, 0.35f),  // inner edge reflection
    ];

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 4, 300);
        float reflectivity = Math.Clamp(Reflectivity, 0f, 100f) / 100f;
        float bevel = Math.Clamp(BevelStrength, 0f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        // Build tint channel ratios
        float tR = TintColor.Red / 200f;
        float tG = TintColor.Green / 200f;
        float tB = TintColor.Blue / 200f;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        for (int y = 0; y < newHeight; y++)
        {
            bool topBand = y < border;
            bool bottomBand = y >= border + source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                bool leftBand = x < border;
                bool rightBand = x >= border + source.Width;

                if (!topBand && !bottomBand && !leftBand && !rightBand)
                    continue;

                // Cross-ratio: 0 = inner edge (near image), 1 = outer edge
                float crossRatio = ComputeCrossRatio(x, y, border, newWidth, newHeight, source.Width, source.Height);

                // Evaluate chrome gradient
                float chromValue = EvaluateChromeGradient(crossRatio, reflectivity);

                // Apply a subtle directional variation based on which side
                float sideVariation = ComputeSideVariation(x, y, border, newWidth, newHeight, source.Width, source.Height, bevel);
                chromValue = Math.Clamp(chromValue + sideVariation * 0.12f, 0f, 1f);

                float brightness = chromValue * 255f;

                dstPixels[(y * newWidth) + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(brightness * tR),
                    ProceduralEffectHelper.ClampToByte(brightness * tG),
                    ProceduralEffectHelper.ClampToByte(brightness * tB),
                    255);
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, border, border);

        if (InnerLine)
        {
            using SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f,
                Color = new SKColor(40, 40, 45)
            };
            canvas.DrawRect(border - 0.5f, border - 0.5f, source.Width + 1f, source.Height + 1f, p);
        }

        if (OuterLine)
        {
            using SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color = new SKColor(20, 20, 22)
            };
            canvas.DrawRect(0.75f, 0.75f, newWidth - 1.5f, newHeight - 1.5f, p);
        }

        return result;
    }

    private static float EvaluateChromeGradient(float t, float reflectivity)
    {
        // Interpolate through chrome stops
        for (int i = 1; i < ChromeStops.Length; i++)
        {
            float p0 = ChromeStops[i - 1].pos;
            float p1 = ChromeStops[i].pos;
            if (t <= p1)
            {
                float local = (t - p0) / (p1 - p0);
                float v0 = ChromeStops[i - 1].value;
                float v1 = ChromeStops[i].value;
                float baseVal = ProceduralEffectHelper.Lerp(v0, v1, local);
                // Blend between mid-gray (0.5) and full chrome based on reflectivity
                return ProceduralEffectHelper.Lerp(0.5f, baseVal, reflectivity);
            }
        }
        return 0.5f;
    }

    private static float ComputeCrossRatio(int x, int y, int border, int newW, int newH, int srcW, int srcH)
    {
        float innerL = border;
        float innerT = border;
        float innerR = border + srcW;
        float innerB = border + srcH;

        float clampX = Math.Clamp((float)x, innerL, innerR - 1);
        float clampY = Math.Clamp((float)y, innerT, innerB - 1);

        float distInner = MathF.Sqrt((x - clampX) * (x - clampX) + (y - clampY) * (y - clampY));
        return Math.Clamp(distInner / border, 0f, 1f);
    }

    private static float ComputeSideVariation(int x, int y, int border, int newW, int newH, int srcW, int srcH, float bevel)
    {
        // Top = positive (brighter), Bottom = negative (darker), Left/Right = slight variation
        bool isTop = y < border && x >= border && x < border + srcW;
        bool isBottom = y >= border + srcH && x >= border && x < border + srcW;
        bool isLeft = x < border && y >= border && y < border + srcH;
        bool isRight = x >= border + srcW && y >= border && y < border + srcH;

        if (isTop) return bevel * 0.5f;
        if (isBottom) return bevel * -0.4f;
        if (isLeft) return bevel * 0.2f;
        if (isRight) return bevel * -0.2f;
        return 0f;
    }
}