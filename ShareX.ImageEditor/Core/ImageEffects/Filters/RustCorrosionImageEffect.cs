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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class RustCorrosionImageEffect : ImageEffectBase
{
    public override string Id => "rust_corrosion";
    public override string Name => "Rust / corrosion";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.brush_cleaning;
    public override string Description => "Simulates rust and corrosion effects with pitting and streaks.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<RustCorrosionImageEffect>("corrosion", "Corrosion", 0f, 100f, 58f, (e, v) => e.Corrosion = v),
        EffectParameters.FloatSlider<RustCorrosionImageEffect>("pitting", "Pitting", 0f, 100f, 44f, (e, v) => e.Pitting = v),
        EffectParameters.FloatSlider<RustCorrosionImageEffect>("streaks", "Streaks", 0f, 100f, 36f, (e, v) => e.Streaks = v),
        EffectParameters.FloatSlider<RustCorrosionImageEffect>("dirt", "Dirt", 0f, 100f, 26f, (e, v) => e.Dirt = v),
        EffectParameters.FloatSlider<RustCorrosionImageEffect>("edge_wear", "Edge wear", 0f, 100f, 52f, (e, v) => e.EdgeWear = v),
    ];

    public float Corrosion { get; set; } = 58f; // 0..100
    public float Pitting { get; set; } = 44f; // 0..100
    public float Streaks { get; set; } = 36f; // 0..100
    public float Dirt { get; set; } = 26f; // 0..100
    public float EdgeWear { get; set; } = 52f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float corrosion = Math.Clamp(Corrosion, 0f, 100f) / 100f;
        float pitting = Math.Clamp(Pitting, 0f, 100f) / 100f;
        float streaks = Math.Clamp(Streaks, 0f, 100f) / 100f;
        float dirt = Math.Clamp(Dirt, 0f, 100f) / 100f;
        float edgeWear = Math.Clamp(EdgeWear, 0f, 100f) / 100f;

        if (corrosion <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float v = y / (float)Math.Max(1, height - 1);

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float r = src.Red;
                float g = src.Green;
                float b = src.Blue;
                float edge = SampleEdge(srcPixels, width, height, x, y);

                float warpScale = 3.5f + (corrosion * 12f);
                float warpX = ((ProceduralEffectHelper.FractalNoise(x * 0.010f, y * 0.010f, 3, 2.05f, 0.55f, 541) * 2f) - 1f) * warpScale;
                float warpY = ((ProceduralEffectHelper.FractalNoise((x + 91.7f) * 0.010f, (y - 63.4f) * 0.010f, 3, 2.00f, 0.55f, 887) * 2f) - 1f) * warpScale * 0.75f;

                float sampleX = x + warpX;
                float sampleY = y + warpY;

                float coarse = ProceduralEffectHelper.FractalNoise(sampleX * 0.016f, sampleY * 0.016f, 3, 2.00f, 0.56f, 701);
                float medium = ProceduralEffectHelper.FractalNoise(sampleX * 0.040f, sampleY * 0.040f, 3, 2.15f, 0.52f, 1301);
                float fine = ProceduralEffectHelper.FractalNoise(sampleX * 0.105f, sampleY * 0.105f, 2, 2.30f, 0.60f, 1907);

                float streakSource = ProceduralEffectHelper.FractalNoise((x * 0.012f) + 17.2f, (y * 0.0035f) + 4.1f, 3, 2.00f, 0.58f, 2203);
                float streak = ProceduralEffectHelper.SmoothStep(0.70f - (streaks * 0.08f), 0.98f, streakSource) * streaks * v;
                streak *= 0.42f + (0.58f * ProceduralEffectHelper.FractalNoise((x - 41.3f) * 0.022f, (y + 18.7f) * 0.060f, 2, 2.20f, 0.62f, 3251));

                float corrosionMask = ((coarse * 0.52f) + (medium * 0.32f) + (fine * 0.16f));
                corrosionMask = ProceduralEffectHelper.SmoothStep(0.48f - (corrosion * 0.18f), 0.94f, corrosionMask);
                corrosionMask = MathF.Max(corrosionMask, edge * edgeWear * 0.72f);
                corrosionMask = ProceduralEffectHelper.Clamp01(corrosionMask * corrosion + streak);

                if (corrosionMask > 0.0001f)
                {
                    float tone = (coarse * 0.56f) + (medium * 0.32f) + (fine * 0.12f);
                    float warmth = ProceduralEffectHelper.FractalNoise((sampleX + 37.1f) * 0.030f, (sampleY - 22.4f) * 0.030f, 2, 2.10f, 0.60f, 4783);
                    float rustR = ProceduralEffectHelper.Lerp(126f, 214f, (tone * 0.72f) + (warmth * 0.28f));
                    float rustG = ProceduralEffectHelper.Lerp(44f, 108f, tone);
                    float rustB = ProceduralEffectHelper.Lerp(12f, 40f, tone * 0.88f);

                    r = ProceduralEffectHelper.Lerp(r, rustR, corrosionMask * 0.86f);
                    g = ProceduralEffectHelper.Lerp(g, rustG, corrosionMask * 0.82f);
                    b = ProceduralEffectHelper.Lerp(b, rustB, corrosionMask * 0.78f);

                    float pitNoise = ProceduralEffectHelper.FractalNoise(sampleX * 0.150f, sampleY * 0.150f, 2, 2.35f, 0.58f, 6121);
                    float pit = ProceduralEffectHelper.SmoothStep(0.72f, 0.96f, pitNoise) * pitting * corrosionMask;
                    r *= 1f - (pit * 0.26f);
                    g *= 1f - (pit * 0.34f);
                    b *= 1f - (pit * 0.38f);
                }

                if (dirt > 0.0001f)
                {
                    float grime = ((ProceduralEffectHelper.FractalNoise(x * 0.028f, y * 0.028f, 3, 2.20f, 0.55f, 4021) * 2f) - 1f) * dirt * 14f;
                    r += grime * 0.80f;
                    g += grime * 0.70f;
                    b += grime * 0.55f;
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    src.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float SampleEdge(SKColor[] pixels, int width, int height, int x, int y)
    {
        int left = Math.Max(0, x - 1);
        int right = Math.Min(width - 1, x + 1);
        int top = Math.Max(0, y - 1);
        int bottom = Math.Min(height - 1, y + 1);

        float lumLeft = GetLuminance(pixels[(y * width) + left]) / 255f;
        float lumRight = GetLuminance(pixels[(y * width) + right]) / 255f;
        float lumTop = GetLuminance(pixels[(top * width) + x]) / 255f;
        float lumBottom = GetLuminance(pixels[(bottom * width) + x]) / 255f;

        return ProceduralEffectHelper.Clamp01((MathF.Abs(lumRight - lumLeft) + MathF.Abs(lumBottom - lumTop)) * 1.1f);
    }

    private static float GetLuminance(SKColor color)
    {
        return (0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue);
    }
}