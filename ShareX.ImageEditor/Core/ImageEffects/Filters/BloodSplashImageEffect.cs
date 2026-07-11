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

public sealed class BloodSplashImageEffect : ImageEffectBase
{
    public override string Id => "blood_splash";
    public override string Name => "Blood splash";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.droplet;
    public override string Description => "Overlays procedural blood splash and drip effects.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<BloodSplashImageEffect>("splash_amount", "Splash amount", 0, 100, 52, (e, v) => e.SplashAmount = v),
        EffectParameters.FloatSlider<BloodSplashImageEffect>("drip_length", "Drip length", 0, 100, 48, (e, v) => e.DripLength = v),
        EffectParameters.FloatSlider<BloodSplashImageEffect>("spread", "Spread", 0, 100, 42, (e, v) => e.Spread = v),
        EffectParameters.FloatSlider<BloodSplashImageEffect>("darkness", "Darkness", 0, 100, 58, (e, v) => e.Darkness = v),
        EffectParameters.FloatSlider<BloodSplashImageEffect>("wet_shine", "Wet shine", 0, 100, 30, (e, v) => e.WetShine = v)
    ];

    public float SplashAmount { get; set; } = 52f; // 0..100
    public float DripLength { get; set; } = 48f; // 0..100
    public float Spread { get; set; } = 42f; // 0..100
    public float Darkness { get; set; } = 58f; // 0..100
    public float WetShine { get; set; } = 30f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float splashAmount = Math.Clamp(SplashAmount, 0f, 100f) / 100f;
        float dripLength = Math.Clamp(DripLength, 0f, 100f) / 100f;
        float spread = Math.Clamp(Spread, 0f, 100f) / 100f;
        float darkness = Math.Clamp(Darkness, 0f, 100f) / 100f;
        float wetShine = Math.Clamp(WetShine, 0f, 100f) / 100f;

        if (splashAmount <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int seed = Random.Shared.Next(1, int.MaxValue);

        float splashCell = 18f + ((1f - spread) * 20f);
        float splashRadius = 5.5f + (spread * 15.5f);
        float splashDensity = 0.025f + (splashAmount * 0.16f);

        float sprayCell = 10f + ((1f - spread) * 10f);
        float sprayRadius = 0.85f + (spread * 2.5f);
        float sprayDensity = 0.035f + (splashAmount * 0.22f);

        float dripCell = 16f + ((1f - spread) * 18f);
        float dripProbability = 0.08f + (splashAmount * 0.26f);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float r = src.Red;
                float g = src.Green;
                float b = src.Blue;
                float a = src.Alpha;

                float splash = SampleSplashMask(x, y, splashCell, splashRadius, splashDensity, seed ^ 991);
                float spray = SampleDropletField(x, y, sprayCell, sprayRadius, sprayDensity, seed ^ 2081);
                float drips = SampleDripMask(x, y, height, dripCell, dripProbability, dripLength, spread, seed ^ 4201);
                float mist = MathF.Pow(ProceduralEffectHelper.Hash01(x, y, seed ^ 6311), 24f)
                    * splashAmount
                    * (0.18f + (spread * 0.45f));

                float mask = Math.Clamp(
                    (splash * 0.98f) +
                    (spray * 0.58f) +
                    (drips * 0.88f) +
                    (mist * 0.32f),
                    0f,
                    1f) * (0.52f + (splashAmount * 0.48f));

                if (mask > 0.0001f)
                {
                    float clot = MathF.Max(0f, splash - 0.32f) * (0.85f + (darkness * 0.35f));
                    float pooled = MathF.Max(clot, drips * 0.28f);
                    float darkMix = Math.Clamp((darkness * 0.72f) + (pooled * 0.55f), 0f, 1f);

                    float targetR = ProceduralEffectHelper.Lerp(198f, 78f, darkMix);
                    float targetG = ProceduralEffectHelper.Lerp(24f, 6f, darkMix);
                    float targetB = ProceduralEffectHelper.Lerp(26f, 10f, darkMix);

                    float coverage = Math.Clamp(mask + (splash * 0.12f), 0f, 1f);
                    r = ProceduralEffectHelper.Lerp(r, targetR, coverage);
                    g = ProceduralEffectHelper.Lerp(g, targetG, coverage);
                    b = ProceduralEffectHelper.Lerp(b, targetB, coverage);

                    float absorb = Math.Clamp((pooled * 0.24f) + (darkness * mask * 0.16f), 0f, 0.42f);
                    r *= 1f - (absorb * 0.18f);
                    g *= 1f - (absorb * 0.34f);
                    b *= 1f - (absorb * 0.38f);

                    if (wetShine > 0.0001f)
                    {
                        float shineNoise = ProceduralEffectHelper.Hash01((int)(x * 0.28f), (int)(y * 0.28f), seed ^ 7607);
                        float shineMask = ProceduralEffectHelper.SmoothStep(0.76f, 0.985f, shineNoise)
                            * ProceduralEffectHelper.SmoothStep(0.22f, 0.82f, MathF.Max(splash, drips));

                        float shine = shineMask * wetShine * (0.25f + (0.75f * MathF.Max(splash, drips)));
                        if (shine > 0.0001f)
                        {
                            r = ProceduralEffectHelper.Lerp(r, 255f, shine * 0.24f);
                            g = ProceduralEffectHelper.Lerp(g, 132f, shine * 0.20f);
                            b = ProceduralEffectHelper.Lerp(b, 122f, shine * 0.14f);
                        }
                    }
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

    private static float SampleSplashMask(int x, int y, float cellSize, float radius, float density, int seed)
    {
        int cellX = (int)MathF.Floor(x / cellSize);
        int cellY = (int)MathF.Floor(y / cellSize);
        float strongest = 0f;

        for (int oy = -1; oy <= 1; oy++)
        {
            for (int ox = -1; ox <= 1; ox++)
            {
                int nx = cellX + ox;
                int ny = cellY + oy;

                if (ProceduralEffectHelper.Hash01(nx, ny, seed) > density)
                {
                    continue;
                }

                float centerX = (nx + ProceduralEffectHelper.Hash01(nx, ny, seed ^ 157)) * cellSize;
                float centerY = (ny + ProceduralEffectHelper.Hash01(nx, ny, seed ^ 389)) * cellSize;
                float localRadius = radius * (0.48f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 701) * 0.92f));

                strongest = MathF.Max(strongest, SampleDroplet(x, y, centerX, centerY, localRadius, seed ^ 911));

                for (int i = 0; i < 3; i++)
                {
                    float satelliteChance = ProceduralEffectHelper.Hash01((nx * 17) + i, (ny * 19) - i, seed ^ 1237);
                    if (satelliteChance > 0.82f)
                    {
                        continue;
                    }

                    float angle = ProceduralEffectHelper.Hash01((nx * 23) + i, (ny * 29) + i, seed ^ 1601) * MathF.Tau;
                    float orbit = localRadius * (0.32f + (ProceduralEffectHelper.Hash01((nx * 31) + i, (ny * 37) + i, seed ^ 1907) * 1.12f));
                    float satX = centerX + (MathF.Cos(angle) * orbit);
                    float satY = centerY + (MathF.Sin(angle) * orbit * (0.72f + (satelliteChance * 0.55f)));
                    float satRadius = localRadius * (0.16f + (satelliteChance * 0.32f));

                    strongest = MathF.Max(strongest, SampleDroplet(x, y, satX, satY, satRadius, seed ^ (2401 + i)));
                }
            }
        }

        return strongest;
    }

    private static float SampleDropletField(int x, int y, float cellSize, float radius, float density, int seed)
    {
        int cellX = (int)MathF.Floor(x / cellSize);
        int cellY = (int)MathF.Floor(y / cellSize);
        float strongest = 0f;

        for (int oy = -1; oy <= 1; oy++)
        {
            for (int ox = -1; ox <= 1; ox++)
            {
                int nx = cellX + ox;
                int ny = cellY + oy;
                if (ProceduralEffectHelper.Hash01(nx, ny, seed) > density)
                {
                    continue;
                }

                float centerX = (nx + ProceduralEffectHelper.Hash01(nx, ny, seed ^ 223)) * cellSize;
                float centerY = (ny + ProceduralEffectHelper.Hash01(nx, ny, seed ^ 521)) * cellSize;
                float localRadius = radius * (0.65f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 883) * 0.9f));

                strongest = MathF.Max(strongest, SampleDroplet(x, y, centerX, centerY, localRadius, seed ^ 1291));
            }
        }

        return strongest;
    }

    private static float SampleDroplet(float x, float y, float centerX, float centerY, float radius, int seed)
    {
        float dx = x - centerX;
        float dy = y - centerY;
        float dist = MathF.Sqrt((dx * dx) + (dy * dy));
        float edgeNoise = ProceduralEffectHelper.Hash01((int)(centerX * 0.25f), (int)(centerY * 0.25f), seed);
        float irregularRadius = radius * (0.78f + (edgeNoise * 0.34f));
        return 1f - ProceduralEffectHelper.SmoothStep(irregularRadius * 0.22f, irregularRadius, dist);
    }

    private static float SampleDripMask(int x, int y, int height, float cellWidth, float probability, float dripLength, float spread, int seed)
    {
        int column = (int)MathF.Floor(x / cellWidth);
        float strongest = 0f;

        for (int c = column - 2; c <= column + 2; c++)
        {
            if (ProceduralEffectHelper.Hash01(c, 0, seed) > probability)
            {
                continue;
            }

            float baseX = (c + ProceduralEffectHelper.Hash01(c, 1, seed ^ 191)) * cellWidth;
            float startY = ProceduralEffectHelper.Hash01(c, 2, seed ^ 487) * height * 0.58f;
            float length = (0.12f + (dripLength * 0.9f))
                * (0.36f + (ProceduralEffectHelper.Hash01(c, 3, seed ^ 761) * 0.88f))
                * height;
            float thickness = (1.15f + (spread * 3.8f))
                * (0.68f + (ProceduralEffectHelper.Hash01(c, 4, seed ^ 1033) * 0.85f));

            float dy = y - startY;
            if (dy < 0f || dy > length)
            {
                continue;
            }

            float sway = ((ProceduralEffectHelper.Hash01(c, 5, seed ^ 1327) * 2f) - 1f) * spread * 5.5f;
            float frequency = 0.012f + (ProceduralEffectHelper.Hash01(c, 6, seed ^ 1607) * 0.035f);
            float phase = ProceduralEffectHelper.Hash01(c, 7, seed ^ 1931) * MathF.Tau;
            float laneX = baseX
                + MathF.Sin((dy * frequency) + phase) * sway
                + (dy * sway * 0.018f);

            float dx = MathF.Abs(x - laneX);
            float line = 1f - ProceduralEffectHelper.SmoothStep(thickness * 0.45f, thickness, dx);
            if (line <= 0f)
            {
                continue;
            }

            float fade = 1f - ProceduralEffectHelper.SmoothStep(0f, length, dy);
            float drip = line * (0.48f + (0.52f * fade));

            float tipY = startY + length;
            float tipDx = x - laneX;
            float tipDy = y - tipY;
            float tipDist = MathF.Sqrt((tipDx * tipDx) + ((tipDy * tipDy) * 0.52f));
            float tip = 1f - ProceduralEffectHelper.SmoothStep(thickness * 0.24f, thickness * 1.75f, tipDist);

            float shoulderDx = x - baseX;
            float shoulderDy = y - startY;
            float shoulderDist = MathF.Sqrt((shoulderDx * shoulderDx) + ((shoulderDy * shoulderDy) * 0.7f));
            float shoulder = 1f - ProceduralEffectHelper.SmoothStep(thickness * 0.20f, thickness * 1.9f, shoulderDist);

            strongest = MathF.Max(strongest, MathF.Max(drip, MathF.Max(tip * 0.92f, shoulder * 0.38f)));
        }

        return strongest;
    }
}