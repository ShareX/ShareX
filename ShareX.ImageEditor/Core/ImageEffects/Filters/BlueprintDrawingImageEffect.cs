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

public sealed class BlueprintDrawingImageEffect : ImageEffectBase
{
    public override string Id => "blueprint_drawing";
    public override string Name => "Blueprint drawing";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.drafting_compass;
    public override string Description => "Transforms the image into a blueprint-style drawing.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<BlueprintDrawingImageEffect>("line_strength", "Line strength", 0, 100, 75, (e, v) => e.LineStrength = v),
        EffectParameters.FloatSlider<BlueprintDrawingImageEffect>("detail", "Detail", 0, 100, 45, (e, v) => e.Detail = v),
        EffectParameters.FloatSlider<BlueprintDrawingImageEffect>("grid_intensity", "Grid intensity", 0, 100, 30, (e, v) => e.GridIntensity = v),
        EffectParameters.FloatSlider<BlueprintDrawingImageEffect>("texture", "Texture", 0, 100, 25, (e, v) => e.Texture = v),
        EffectParameters.FloatSlider<BlueprintDrawingImageEffect>("glow", "Glow", 0, 100, 35, (e, v) => e.Glow = v)
    ];

    public float LineStrength { get; set; } = 75f; // 0..100
    public float Detail { get; set; } = 45f; // 0..100
    public float GridIntensity { get; set; } = 30f; // 0..100
    public float Texture { get; set; } = 25f; // 0..100
    public float Glow { get; set; } = 35f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float lineStrength = Math.Clamp(LineStrength, 0f, 100f) / 100f;
        float detail = Math.Clamp(Detail, 0f, 100f) / 100f;
        float grid = Math.Clamp(GridIntensity, 0f, 100f) / 100f;
        float texture = Math.Clamp(Texture, 0f, 100f) / 100f;
        float glow = Math.Clamp(Glow, 0f, 100f) / 100f;
        int seed = Random.Shared.Next(1, int.MaxValue);

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;
        float centerX = right * 0.5f;
        float centerY = bottom * 0.5f;
        float invSpacing = 1f / (10f + ((1f - detail) * 20f));

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        float[] luminance = new float[srcPixels.Length];

        Parallel.For(0, srcPixels.Length, i =>
        {
            SKColor c = srcPixels[i];
            luminance[i] = ((0.2126f * c.Red) + (0.7152f * c.Green) + (0.0722f * c.Blue)) / 255f;
        });

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            int yUp = y > 0 ? y - 1 : 0;
            int yDown = y < bottom ? y + 1 : bottom;

            for (int x = 0; x < width; x++)
            {
                int xLeft = x > 0 ? x - 1 : 0;
                int xRight = x < right ? x + 1 : right;
                int index = row + x;
                float lum = luminance[index];

                float gradX = MathF.Abs(luminance[row + xRight] - luminance[row + xLeft]);
                float gradY = MathF.Abs(luminance[(yDown * width) + x] - luminance[(yUp * width) + x]);
                float edge = Math.Clamp((gradX + gradY) * (1.7f + (lineStrength * 2.4f)), 0f, 1f);

                float tonalLine = MathF.Pow(1f - lum, 0.65f + ((1f - detail) * 1.5f)) * (0.2f + (detail * 0.35f));
                float lineMask = Math.Clamp((edge * (0.72f + (lineStrength * 0.75f))) + tonalLine, 0f, 1f);

                float nx = x / Math.Max(1f, right);
                float ny = y / Math.Max(1f, bottom);
                float dx = (x - centerX) / Math.Max(1f, right);
                float dy = (y - centerY) / Math.Max(1f, bottom);
                float dist = MathF.Sqrt((dx * dx) + (dy * dy));
                float vignette = 1f - ProceduralEffectHelper.SmoothStep(0.35f, 0.95f, dist);
                float baseShade = 0.42f + (0.38f * vignette);

                float gridX = MathF.Abs(((x * invSpacing) - MathF.Round(x * invSpacing)));
                float gridY = MathF.Abs(((y * invSpacing) - MathF.Round(y * invSpacing)));
                float gridLine = MathF.Max(1f - (gridX * 18f), 1f - (gridY * 18f));
                gridLine = Math.Clamp(gridLine, 0f, 1f) * grid * 0.45f;

                float paperNoiseA = ((ProceduralEffectHelper.Hash01((int)(nx * 1024f), (int)(ny * 1024f), seed) * 2f) - 1f) * texture;
                float paperNoiseB = ((ProceduralEffectHelper.Hash01((int)(nx * 320f), (int)(ny * 320f), seed ^ 431) * 2f) - 1f) * texture * 0.6f;
                float paper = (paperNoiseA + paperNoiseB) * 12f;

                float br = 10f + (12f * baseShade) + (16f * gridLine) + paper;
                float bg = 52f + (46f * baseShade) + (28f * gridLine) + paper;
                float bb = 122f + (88f * baseShade) + (46f * gridLine) + paper;

                float glowMix = lineMask * (0.55f + (glow * 0.45f));
                float r = ProceduralEffectHelper.Lerp(br, 230f, glowMix);
                float g = ProceduralEffectHelper.Lerp(bg, 242f, glowMix);
                float b = ProceduralEffectHelper.Lerp(bb, 255f, glowMix);

                float cyanEdge = lineMask * glow * 0.28f;
                r = ProceduralEffectHelper.Lerp(r, 136f, cyanEdge);
                g = ProceduralEffectHelper.Lerp(g, 224f, cyanEdge);
                b = ProceduralEffectHelper.Lerp(b, 255f, cyanEdge);

                dstPixels[index] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    srcPixels[index].Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}