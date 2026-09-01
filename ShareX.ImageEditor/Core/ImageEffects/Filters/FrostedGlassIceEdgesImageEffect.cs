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

public sealed class FrostedGlassIceEdgesImageEffect : ImageEffectBase
{
    public override string Id => "frosted_glass_ice_edges";
    public override string Name => "Frosted glass + ice edges";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.snowflake;
    public override string Description => "Combines frosted glass distortion with icy edge highlights and cool tinting.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<FrostedGlassIceEdgesImageEffect>("distortion", "Distortion", 0, 30, 10, (e, v) => e.Distortion = v),
        EffectParameters.FloatSlider<FrostedGlassIceEdgesImageEffect>("blur", "Blur", 0, 100, 35, (e, v) => e.Blur = v),
        EffectParameters.IntSlider<FrostedGlassIceEdgesImageEffect>("edge_threshold", "Edge threshold", 1, 120, 28, (e, v) => e.EdgeThreshold = v),
        EffectParameters.FloatSlider<FrostedGlassIceEdgesImageEffect>("ice_strength", "Ice strength", 0, 100, 75, (e, v) => e.IceStrength = v),
        EffectParameters.FloatSlider<FrostedGlassIceEdgesImageEffect>("tint", "Tint", 0, 100, 35, (e, v) => e.Tint = v)
    ];

    public float Distortion { get; set; } = 10f; // 0..30
    public float Blur { get; set; } = 35f; // 0..100
    public int EdgeThreshold { get; set; } = 28; // 1..120
    public float IceStrength { get; set; } = 75f; // 0..100
    public float Tint { get; set; } = 35f; // 0..100
    public int Seed { get; set; } = 1337;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float distortion = Math.Clamp(Distortion, 0f, 30f);
        float blurMix = Math.Clamp(Blur, 0f, 100f) / 100f;
        int edgeThreshold = Math.Clamp(EdgeThreshold, 1, 120);
        float iceStrength = Math.Clamp(IceStrength, 0f, 100f) / 100f;
        float tint = Math.Clamp(Tint, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

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
                float coarseX = (ProceduralEffectHelper.Hash01(x / 9, y / 9, Seed) * 2f) - 1f;
                float coarseY = (ProceduralEffectHelper.Hash01(x / 9, y / 9, Seed ^ 977) * 2f) - 1f;
                float fine = (ProceduralEffectHelper.Hash01(x, y, Seed ^ 4099) * 2f) - 1f;

                float sampleX = x + (((coarseX * 0.75f) + (fine * 0.25f)) * distortion);
                float sampleY = y + (((coarseY * 0.75f) - (fine * 0.25f)) * distortion);
                SKColor sample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY);

                float r = sample.Red;
                float g = sample.Green;
                float b = sample.Blue;
                float a = sample.Alpha;

                if (blurMix > 0.001f)
                {
                    SKColor s1 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX - 1f, sampleY);
                    SKColor s2 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX + 1f, sampleY);
                    SKColor s3 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY - 1f);
                    SKColor s4 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY + 1f);

                    float blurR = (s1.Red + s2.Red + s3.Red + s4.Red) * 0.25f;
                    float blurG = (s1.Green + s2.Green + s3.Green + s4.Green) * 0.25f;
                    float blurB = (s1.Blue + s2.Blue + s3.Blue + s4.Blue) * 0.25f;
                    float blurA = (s1.Alpha + s2.Alpha + s3.Alpha + s4.Alpha) * 0.25f;

                    r = ProceduralEffectHelper.Lerp(r, blurR, blurMix * 0.85f);
                    g = ProceduralEffectHelper.Lerp(g, blurG, blurMix * 0.85f);
                    b = ProceduralEffectHelper.Lerp(b, blurB, blurMix * 0.85f);
                    a = ProceduralEffectHelper.Lerp(a, blurA, blurMix * 0.6f);
                }

                int xLeft = x > 0 ? x - 1 : 0;
                int xRight = x < right ? x + 1 : right;
                float gradX = MathF.Abs(luminance[row + xRight] - luminance[row + xLeft]);
                float gradY = MathF.Abs(luminance[(yDown * width) + x] - luminance[(yUp * width) + x]);
                float edgeMetric = (gradX + gradY) * 255f;

                float edge = (edgeMetric - edgeThreshold) / Math.Max(1f, 255f - edgeThreshold);
                edge = ProceduralEffectHelper.Clamp01(edge) * iceStrength;

                float coolMix = (tint * 0.45f) + (edge * 0.55f);
                r = ProceduralEffectHelper.Lerp(r, (r * 0.82f) + 16f, coolMix);
                g = ProceduralEffectHelper.Lerp(g, (g * 0.95f) + 24f, coolMix);
                b = ProceduralEffectHelper.Lerp(b, (b * 1.08f) + 56f, coolMix);

                if (edge > 0.001f)
                {
                    float spec = MathF.Pow(ProceduralEffectHelper.Hash01(x, y, Seed ^ 12345), 18f) * edge;
                    float iceGlow = edge * (0.55f + (spec * 0.8f));
                    r += 120f * iceGlow;
                    g += 142f * iceGlow;
                    b += 175f * iceGlow;
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    ProceduralEffectHelper.ClampToByte(a));
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}