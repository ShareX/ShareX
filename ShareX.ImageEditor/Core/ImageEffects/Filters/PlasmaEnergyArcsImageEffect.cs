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

public sealed class PlasmaEnergyArcsImageEffect : ImageEffectBase
{
    public override string Id => "plasma_energy_arcs";
    public override string Name => "Plasma energy arcs";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.zap;
    public override string Description => "Generates plasma-like energy arcs overlaid on the image.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<PlasmaEnergyArcsImageEffect>("energy", "Energy", 0f, 100f, 65f, (e, v) => e.Energy = v),
        EffectParameters.FloatSlider<PlasmaEnergyArcsImageEffect>("arc_density", "Arc density", 0f, 100f, 46f, (e, v) => e.ArcDensity = v),
        EffectParameters.FloatSlider<PlasmaEnergyArcsImageEffect>("glow", "Glow", 0f, 100f, 70f, (e, v) => e.Glow = v),
        EffectParameters.FloatSlider<PlasmaEnergyArcsImageEffect>("turbulence", "Turbulence", 0f, 100f, 52f, (e, v) => e.Turbulence = v),
        EffectParameters.FloatSlider<PlasmaEnergyArcsImageEffect>("thickness", "Thickness", 0f, 100f, 38f, (e, v) => e.Thickness = v),
    ];

    public float Energy { get; set; } = 65f; // 0..100
    public float ArcDensity { get; set; } = 46f; // 0..100
    public float Glow { get; set; } = 70f; // 0..100
    public float Turbulence { get; set; } = 52f; // 0..100
    public float Thickness { get; set; } = 38f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float energy = Math.Clamp(Energy, 0f, 100f) / 100f;
        float arcDensity = Math.Clamp(ArcDensity, 0f, 100f) / 100f;
        float glow = Math.Clamp(Glow, 0f, 100f) / 100f;
        float turbulence = Math.Clamp(Turbulence, 0f, 100f) / 100f;
        float thickness = Math.Clamp(Thickness, 0f, 100f) / 100f;

        if (energy <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float invWidth = 1f / Math.Max(1, width - 1);
        float invHeight = 1f / Math.Max(1, height - 1);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float lineWidth = 0.020f + ((1f - thickness) * 0.10f);
        float densityFreq = 16f + (arcDensity * 18f);
        float turbulenceFreq = 2f + (turbulence * 6f);

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float v = y * invHeight;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;

                float u = x * invWidth;
                float fieldA = MathF.Sin((u * densityFreq) + MathF.Sin((v * turbulenceFreq * 2.4f) + 0.7f) * (1.8f + turbulence * 4.5f) + 0.8f);
                float fieldB = MathF.Cos((v * (densityFreq * 0.92f)) - MathF.Sin((u * turbulenceFreq * 1.6f) - 1.1f) * (1.4f + turbulence * 4.2f) - 1.3f);
                float fieldC = MathF.Sin(((u + v) * (10f + arcDensity * 12f)) + 2.1f);

                float coreField = (fieldA * 0.46f) + (fieldB * 0.34f) + (fieldC * 0.20f);
                float branchField = (fieldA * 0.28f) - (fieldB * 0.44f) + (fieldC * 0.28f);

                float core = 1f - ProceduralEffectHelper.SmoothStep(lineWidth, lineWidth + 0.08f, MathF.Abs(coreField));
                float branch = 1f - ProceduralEffectHelper.SmoothStep(lineWidth * 0.70f, lineWidth * 0.70f + 0.09f, MathF.Abs(branchField));
                float arc = MathF.Max(core, branch * 0.72f);

                float edge = SampleEdge(srcPixels, width, height, x, y);
                arc *= 0.72f + (edge * 0.28f);

                float glowMask = MathF.Pow(MathF.Max(arc, 0f), 0.34f) * glow;
                float coreMask = arc * energy;

                float overlayR = (coreMask * 0.20f) + (glowMask * 0.10f);
                float overlayG = (coreMask * 0.70f) + (glowMask * 0.26f);
                float overlayB = (coreMask * 1.00f) + (glowMask * 0.34f);

                float outR = Screen(sr, overlayR);
                float outG = Screen(sg, overlayG);
                float outB = Screen(sb, overlayB);

                outR = ProceduralEffectHelper.Lerp(outR, 1f, coreMask * 0.18f);
                outG = ProceduralEffectHelper.Lerp(outG, 1f, coreMask * 0.22f);
                outB = ProceduralEffectHelper.Lerp(outB, 1f, coreMask * 0.28f);

                dstPixels[row + x] = new SKColor(
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

    private static float SampleEdge(SKColor[] pixels, int width, int height, int x, int y)
    {
        int left = Math.Max(0, x - 1);
        int right = Math.Min(width - 1, x + 1);
        int top = Math.Max(0, y - 1);
        int bottom = Math.Min(height - 1, y + 1);

        float lumLeft = GetLuminance01(pixels[(y * width) + left]);
        float lumRight = GetLuminance01(pixels[(y * width) + right]);
        float lumTop = GetLuminance01(pixels[(top * width) + x]);
        float lumBottom = GetLuminance01(pixels[(bottom * width) + x]);
        return ProceduralEffectHelper.Clamp01((MathF.Abs(lumRight - lumLeft) + MathF.Abs(lumBottom - lumTop)) * 1.1f);
    }

    private static float GetLuminance01(SKColor color)
    {
        return ((0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue)) / 255f;
    }

    private static float Screen(float source, float overlay)
    {
        overlay = ProceduralEffectHelper.Clamp01(overlay);
        return 1f - ((1f - source) * (1f - overlay));
    }
}