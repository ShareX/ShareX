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

public sealed class RainbowBorderImageEffect : ImageEffectBase
{
    public override string Id => "rainbow_border";
    public override string Name => "Rainbow border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.rainbow;
    public override string Description => "Adds a vivid rainbow gradient border that cycles through the full color spectrum.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<RainbowBorderImageEffect>("border_size", "Border size", 2, 300, 24, (e, v) => e.BorderSize = v),
        EffectParameters.FloatSlider<RainbowBorderImageEffect>("saturation", "Saturation", 0, 100, 100, (e, v) => e.Saturation = v),
        EffectParameters.FloatSlider<RainbowBorderImageEffect>("brightness", "Brightness", 0, 100, 95, (e, v) => e.Brightness = v),
        EffectParameters.FloatSlider<RainbowBorderImageEffect>("hue_offset", "Hue offset", 0, 360, 0, (e, v) => e.HueOffset = v),
        EffectParameters.FloatSlider<RainbowBorderImageEffect>("glow_strength", "Glow", 0, 100, 40, (e, v) => e.GlowStrength = v),
        EffectParameters.Bool<RainbowBorderImageEffect>("inner_line", "Inner line", true, (e, v) => e.InnerLine = v)
    ];

    public int BorderSize { get; set; } = 24;
    public float Saturation { get; set; } = 100f;
    public float Brightness { get; set; } = 95f;
    public float HueOffset { get; set; } = 0f;
    public float GlowStrength { get; set; } = 40f;
    public bool InnerLine { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 2, 300);
        float sat = Math.Clamp(Saturation, 0f, 100f);
        float brightness = Math.Clamp(Brightness, 0f, 100f);
        float hueOff = HueOffset % 360f;
        float glow = Math.Clamp(GlowStrength, 0f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        float centerX = newWidth * 0.5f;
        float centerY = newHeight * 0.5f;

        // Total perimeter for linear rainbow mapping
        float perimW = source.Width + border * 2f;
        float perimH = source.Height + border * 2f;
        float totalPerim = 2f * (perimW + perimH - 4f);

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

                // Find position along the outer perimeter, mapped to hue
                float hue = ComputePerimeterHue(x, y, border, newWidth, newHeight, totalPerim, perimW, perimH, hueOff);

                // Cross-ratio: 0 = inner edge, 1 = outer edge
                float crossRatio = ComputeCrossRatio(x, y, border, newWidth, newHeight, source.Width, source.Height);

                // Brightness falls off near the outer edge slightly for a glow look
                float brightnessScale = glow > 0f
                    ? 1f - ProceduralEffectHelper.SmoothStep(0.6f, 1f, crossRatio) * glow * 0.5f
                    : 1f;

                dstPixels[(y * newWidth) + x] = SKColor.FromHsv(hue, sat, brightness * brightnessScale);
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
            using SKPaint linePaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color = new SKColor(255, 255, 255, 120)
            };
            canvas.DrawRect(border - 0.75f, border - 0.75f, source.Width + 1.5f, source.Height + 1.5f, linePaint);
        }

        return result;
    }

    private static float ComputePerimeterHue(int x, int y, int border, int newW, int newH,
        float totalPerim, float perimW, float perimH, float hueOffset)
    {
        // Project to the outer perimeter of the full canvas
        float clampX = Math.Clamp((float)x, 0, newW - 1);
        float clampY = Math.Clamp((float)y, 0, newH - 1);

        // Project to nearest outer edge
        float along;
        float distTop = clampY;
        float distBottom = (newH - 1) - clampY;
        float distLeft = clampX;
        float distRight = (newW - 1) - clampX;

        float minDist = Math.Min(Math.Min(distTop, distBottom), Math.Min(distLeft, distRight));

        if (minDist == distTop)
        {
            along = clampX;
        }
        else if (minDist == distRight)
        {
            along = perimW + clampY;
        }
        else if (minDist == distBottom)
        {
            along = perimW + perimH + (newW - 1 - clampX);
        }
        else
        {
            along = perimW + perimH + (newW - 1) + (newH - 1 - clampY);
        }

        float t = along / totalPerim;
        return (t * 360f + hueOffset) % 360f;
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
}