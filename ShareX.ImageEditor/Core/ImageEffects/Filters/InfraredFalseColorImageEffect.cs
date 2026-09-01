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

public sealed class InfraredFalseColorImageEffect : ImageEffectBase
{
    public override string Id => "infrared_false_color";
    public override string Name => "Infrared false color";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.flame;
    public override string Description => "Simulates infrared false color photography.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<InfraredFalseColorImageEffect>("intensity", "Intensity", 0, 100, 68, (e, v) => e.Intensity = v),
        EffectParameters.FloatSlider<InfraredFalseColorImageEffect>("foliage_shift", "Foliage shift", 0, 100, 78, (e, v) => e.FoliageShift = v),
        EffectParameters.FloatSlider<InfraredFalseColorImageEffect>("sky_darkening", "Sky darkening", 0, 100, 44, (e, v) => e.SkyDarkening = v),
        EffectParameters.FloatSlider<InfraredFalseColorImageEffect>("glow", "Glow", 0, 100, 20, (e, v) => e.Glow = v),
        EffectParameters.FloatSlider<InfraredFalseColorImageEffect>("contrast", "Contrast", 50, 200, 118, (e, v) => e.Contrast = v)
    ];

    public float Intensity { get; set; } = 68f;
    public float FoliageShift { get; set; } = 78f;
    public float SkyDarkening { get; set; } = 44f;
    public float Glow { get; set; } = 20f;
    public float Contrast { get; set; } = 118f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity = Math.Clamp(Intensity, 0f, 100f) / 100f;
        float foliageShift = Math.Clamp(FoliageShift, 0f, 100f) / 100f;
        float skyDarkening = Math.Clamp(SkyDarkening, 0f, 100f) / 100f;
        float glow = Math.Clamp(Glow, 0f, 100f) / 100f;
        float contrast = Math.Clamp(Contrast, 50f, 200f) / 100f;

        if (intensity <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        using SKBitmap? glowBitmap = glow > 0.01f ? AnalogEffectHelper.CreateBlurredClamp(source, 6f + (glow * 12f)) : null;
        SKColor[] glowPixels = glowBitmap?.Pixels ?? srcPixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                src.ToHsl(out float h, out float s, out float l);

                float nh = h;
                float ns = s;
                float nl = l;

                bool isFoliage = h >= 45f && h <= 170f && s >= 18f;
                bool isSky = h >= 170f && h <= 260f && s >= 12f;

                if (isFoliage)
                {
                    nh = ProceduralEffectHelper.Lerp(h, 8f + (l * 0.16f), foliageShift * (0.55f + (intensity * 0.35f)));
                    ns = Math.Min(100f, s * (1f + (0.18f * intensity) + (0.32f * foliageShift)));
                    nl = Math.Min(100f, l + (intensity * 14f) + (foliageShift * 10f));
                }
                else if (isSky)
                {
                    nh = ProceduralEffectHelper.Lerp(h, 196f, 0.22f + (intensity * 0.28f));
                    ns = Math.Min(100f, s * (1.03f + (0.10f * intensity)));
                    nl = Math.Max(0f, l * (1f - (skyDarkening * 0.46f)));
                }
                else
                {
                    nh = ProceduralEffectHelper.Lerp(h, h + 6f, intensity * 0.10f);
                    ns = Math.Min(100f, s * (1f + (intensity * 0.06f)));
                    nl = Math.Clamp(l + (intensity * 3f), 0f, 100f);
                }

                SKColor mapped = AnalogEffectHelper.FromHsl(nh, ns, nl, src.Alpha);
                float r = AnalogEffectHelper.ApplyContrast(mapped.Red / 255f, contrast);
                float g = AnalogEffectHelper.ApplyContrast(mapped.Green / 255f, contrast);
                float b = AnalogEffectHelper.ApplyContrast(mapped.Blue / 255f, contrast);

                if (glow > 0.0001f)
                {
                    float glowMask = MathF.Max(0f, (AnalogEffectHelper.Luminance01(glowPixels[row + x]) - 0.52f) / 0.48f);
                    glowMask = MathF.Pow(glowMask, 1.55f) * glow;
                    r = AnalogEffectHelper.Screen(r, glowMask * 0.80f);
                    g = AnalogEffectHelper.Screen(g, glowMask * 0.18f);
                    b = AnalogEffectHelper.Screen(b, glowMask * 0.42f);
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    src.Alpha);
            }
        });

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }
}