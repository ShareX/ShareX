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

public sealed class LuminanceContourLinesImageEffect : ImageEffectBase
{
    public override string Id => "luminance_contour_lines";
    public override string Name => "Luminance contour lines";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.waypoints;
    public override string Description => "Draws contour lines based on luminance quantization levels.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<LuminanceContourLinesImageEffect>("levels", "Levels", 2, 64, 12, (e, v) => e.Levels = v),
        EffectParameters.FloatSlider<LuminanceContourLinesImageEffect>("line_width", "Line width", 0, 200, 6, (e, v) => e.LineWidth = v),
        EffectParameters.FloatSlider<LuminanceContourLinesImageEffect>("line_strength", "Line strength", 0, 100, 65, (e, v) => e.LineStrength = v),
        EffectParameters.FloatSlider<LuminanceContourLinesImageEffect>("background_strength", "Background strength", 0, 100, 20, (e, v) => e.BackgroundStrength = v),
        EffectParameters.FloatSlider<LuminanceContourLinesImageEffect>("threshold", "Threshold", 0, 255, 0, (e, v) => e.Threshold = v),
        EffectParameters.Bool<LuminanceContourLinesImageEffect>("invert", "Invert", false, (e, v) => e.Invert = v),
        EffectParameters.Color<LuminanceContourLinesImageEffect>("line_color", "Line color", new SKColor(0, 0, 0, 255), (e, v) => e.LineColor = v),
    ];

    public int Levels { get; set; } = 12; // ~2..64
    public float LineWidth { get; set; } = 6f; // 0..200
    public float LineStrength { get; set; } = 65f; // 0..100
    public float BackgroundStrength { get; set; } = 20f; // 0..100
    public float Threshold { get; set; } = 0f; // 0..255 heuristic (used as luminance bias)
    public bool Invert { get; set; }
    public SKColor LineColor { get; set; } = new SKColor(0, 0, 0, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int levels = Math.Clamp(Levels, 2, 64);
        if (levels <= 0)
        {
            return source.Copy();
        }

        float backgroundStrength01 = Math.Clamp(BackgroundStrength, 0f, 100f) / 100f;
        float lineStrength01 = Math.Clamp(LineStrength, 0f, 100f) / 100f;
        if (lineStrength01 <= 0f && backgroundStrength01 <= 0f)
        {
            return source.Copy();
        }

        float thresholdBias = Math.Clamp(Threshold, 0f, 255f) / 255f;

        float lineWidth01 = Math.Clamp(LineWidth, 0f, 200f) / 200f; // 0..1
        float feather = 0.04f + (0.22f * lineWidth01); // fraction around step boundaries

        float lineColorA01 = LineColor.Alpha / 255f;
        float lineColorR = LineColor.Red / 255f;
        float lineColorG = LineColor.Green / 255f;
        float lineColorB = LineColor.Blue / 255f;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                int idx = row + x;
                SKColor src = srcPixels[idx];

                float lum01 = ((0.2126f * src.Red) + (0.7152f * src.Green) + (0.0722f * src.Blue)) / 255f;
                lum01 = ProceduralEffectHelper.Clamp01(lum01 + thresholdBias);

                float scaled = lum01 * levels;
                float qIndex = MathF.Floor(scaled);
                float frac = scaled - qIndex; // 0..1 inside the current quantization bucket

                // Quantized luminance for the background.
                float qLum = (qIndex + 0.5f) / levels;
                qLum = ProceduralEffectHelper.Clamp01(qLum);

                // Distance to the nearest bucket boundary (0 at boundary, 0.5 at center).
                float distToBoundary = MathF.Min(frac, 1f - frac);

                // Line mask appears around bucket boundaries.
                float t = ProceduralEffectHelper.SmoothStep(0f, feather, distToBoundary);
                float lineMask = 1f - t;
                lineMask = lineMask * lineMask;

                if (Invert)
                {
                    lineMask = 1f - lineMask;
                }

                float bgLum = ProceduralEffectHelper.Lerp(lum01, qLum, backgroundStrength01);
                bgLum = ProceduralEffectHelper.Clamp01(bgLum);

                float baseR = bgLum;
                float baseG = bgLum;
                float baseB = bgLum;

                float lineMix = lineMask * lineStrength01 * lineColorA01;

                float outR = ProceduralEffectHelper.Lerp(baseR, lineColorR, lineMix);
                float outG = ProceduralEffectHelper.Lerp(baseG, lineColorG, lineMix);
                float outB = ProceduralEffectHelper.Lerp(baseB, lineColorB, lineMix);

                dstPixels[idx] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(outR * 255f),
                    ProceduralEffectHelper.ClampToByte(outG * 255f),
                    ProceduralEffectHelper.ClampToByte(outB * 255f),
                    src.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}