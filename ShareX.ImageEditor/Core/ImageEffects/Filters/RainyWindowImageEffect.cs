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

public sealed class RainyWindowImageEffect : ImageEffectBase
{
    public override string Id => "rainy_window";
    public override string Name => "Rainy window";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cloud_rain;
    public override string Description => "Simulates looking through a rain-streaked window with distortion and mist.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<RainyWindowImageEffect>("distortion", "Distortion", 0f, 30f, 8f, (e, v) => e.Distortion = v),
        EffectParameters.FloatSlider<RainyWindowImageEffect>("streak_density", "Streak density", 0f, 100f, 45f, (e, v) => e.StreakDensity = v),
        EffectParameters.FloatSlider<RainyWindowImageEffect>("mist_amount", "Mist amount", 0f, 100f, 25f, (e, v) => e.MistAmount = v),
        EffectParameters.FloatSlider<RainyWindowImageEffect>("droplet_amount", "Droplet amount", 0f, 100f, 35f, (e, v) => e.DropletAmount = v),
    ];

    public float Distortion { get; set; } = 8f; // 0..30
    public float StreakDensity { get; set; } = 45f; // 0..100
    public float MistAmount { get; set; } = 25f; // 0..100
    public float DropletAmount { get; set; } = 35f; // 0..100
    public int Seed { get; set; } = 8723;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float distortionPx = Math.Clamp(Distortion, 0f, 30f);
        float streakDensity = Math.Clamp(StreakDensity, 0f, 100f) / 100f;
        float mist = Math.Clamp(MistAmount, 0f, 100f) / 100f;
        float droplets = Math.Clamp(DropletAmount, 0f, 100f) / 100f;

        if (distortionPx <= 0f && streakDensity <= 0f && mist <= 0f && droplets <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float dropletCellSize = MathF.Max(8f, 30f - (droplets * 22f));
        const float twoPi = MathF.PI * 2f;

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            float yWave = y * 0.09f;

            for (int x = 0; x < width; x++)
            {
                float columnPhase = ProceduralEffectHelper.Hash01(x / 8, 3, Seed) * twoPi;
                float waveA = MathF.Sin((y * 0.075f) + columnPhase);
                float waveB = MathF.Sin((x * 0.022f) + (y * 0.15f) + (columnPhase * 0.5f));
                float offsetX = distortionPx * ((0.38f * waveA) + (0.18f * waveB));
                float offsetY = distortionPx * 0.08f * MathF.Sin((x * 0.08f) + (columnPhase * 1.3f));

                float sx = x + offsetX;
                float sy = y + offsetY;

                SKColor baseColor = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx, sy);

                if (mist > 0f)
                {
                    SKColor b0 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx - 1.25f, sy);
                    SKColor b1 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx + 1.25f, sy);
                    SKColor b2 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx, sy - 1.25f);
                    SKColor b3 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx, sy + 1.25f);
                    SKColor blurColor = new SKColor(
                        (byte)((b0.Red + b1.Red + b2.Red + b3.Red) / 4),
                        (byte)((b0.Green + b1.Green + b2.Green + b3.Green) / 4),
                        (byte)((b0.Blue + b1.Blue + b2.Blue + b3.Blue) / 4),
                        (byte)((b0.Alpha + b1.Alpha + b2.Alpha + b3.Alpha) / 4));

                    baseColor = Blend(baseColor, blurColor, mist * 0.45f);
                }

                float columnNoise = ProceduralEffectHelper.Hash01(x / 6, 11, Seed);
                float streak = 0f;
                if (columnNoise > (1f - (0.72f * streakDensity)))
                {
                    float streakSignal = MathF.Sin(yWave + (columnNoise * 18f));
                    streak = MathF.Pow(MathF.Max(0f, streakSignal), 3f) * streakDensity;
                }

                float dropletMask = 0f;
                int cellX = (int)(x / dropletCellSize);
                int cellY = (int)(y / dropletCellSize);
                float cellNoise = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 0x2A31);
                float dropletThreshold = 0.93f - (0.55f * droplets);
                if (cellNoise > dropletThreshold)
                {
                    float cx = (cellX + ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 193)) * dropletCellSize;
                    float cy = (cellY + ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 431)) * dropletCellSize;
                    float radius = dropletCellSize * (0.12f + (0.24f * ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 997)));

                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = MathF.Sqrt((dx * dx) + (dy * dy));
                    float radial = dist / MathF.Max(radius, 0.001f);
                    dropletMask = 1f - ProceduralEffectHelper.SmoothStep(0.2f, 1f, radial);
                }

                float luminance = Luminance(baseColor);
                SKColor fogColor = new SKColor(186, 196, 206, baseColor.Alpha);
                float fogMix = mist * (0.2f + ((1f - luminance) * 0.8f));
                SKColor mistColor = Blend(baseColor, fogColor, fogMix);

                float wetShade = 1f - ((0.08f * mist) + (0.06f * streak));
                float highlight = (dropletMask * 0.50f) + (streak * 0.20f);

                float r = (mistColor.Red * wetShade) + (highlight * 88f);
                float g = (mistColor.Green * wetShade) + (highlight * 96f);
                float b = (mistColor.Blue * wetShade) + (highlight * 116f);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    mistColor.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static SKColor Blend(SKColor a, SKColor b, float t)
    {
        t = ProceduralEffectHelper.Clamp01(t);
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Red, b.Red, t)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Green, b.Green, t)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Blue, b.Blue, t)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Alpha, b.Alpha, t)));
    }

    private static float Luminance(SKColor c)
    {
        return ((0.2126f * c.Red) + (0.7152f * c.Green) + (0.0722f * c.Blue)) / 255f;
    }
}