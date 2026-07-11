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

public sealed class WetPlateCollodionImageEffect : ImageEffectBase
{
    public override string Id => "wet_plate_collodion";
    public override string Name => "Wet plate collodion";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.film;
    public override string Description => "Simulates the wet plate collodion photographic process with stains and silvering.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<WetPlateCollodionImageEffect>("contrast", "Contrast", 50, 200, 132, (e, v) => e.Contrast = v),
        EffectParameters.FloatSlider<WetPlateCollodionImageEffect>("plate_stains", "Plate stains", 0, 100, 34, (e, v) => e.PlateStains = v),
        EffectParameters.FloatSlider<WetPlateCollodionImageEffect>("scratches", "Scratches", 0, 100, 24, (e, v) => e.Scratches = v),
        EffectParameters.FloatSlider<WetPlateCollodionImageEffect>("silvering", "Silvering", 0, 100, 36, (e, v) => e.Silvering = v),
        EffectParameters.FloatSlider<WetPlateCollodionImageEffect>("vignette", "Vignette", 0, 100, 58, (e, v) => e.Vignette = v)
    ];

    public float Contrast { get; set; } = 132f;
    public float PlateStains { get; set; } = 34f;
    public float Scratches { get; set; } = 24f;
    public float Silvering { get; set; } = 36f;
    public float Vignette { get; set; } = 58f;
    public int Seed { get; set; } = 1855;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float contrast = Math.Clamp(Contrast, 50f, 200f) / 100f;
        float stains = Math.Clamp(PlateStains, 0f, 100f) / 100f;
        float scratches = Math.Clamp(Scratches, 0f, 100f) / 100f;
        float silvering = Math.Clamp(Silvering, 0f, 100f) / 100f;
        float vignette = Math.Clamp(Vignette, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        float centerX = (width - 1) * 0.5f;
        float centerY = (height - 1) * 0.5f;
        float maxDist = MathF.Sqrt((centerX * centerX) + (centerY * centerY));
        maxDist = MathF.Max(1f, maxDist);

        SKColor shadow = new(20, 16, 18);
        SKColor highlight = new(220, 213, 202);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float lum = AnalogEffectHelper.ApplyContrast(AnalogEffectHelper.Luminance01(src), contrast);
                SKColor tone = AnalogEffectHelper.LerpColor(shadow, highlight, MathF.Pow(lum, 0.88f));

                float chemNoise = ProceduralEffectHelper.FractalNoise(x * 0.008f, y * 0.008f, 5, 2.05f, 0.52f, Seed);
                float streakNoise = ProceduralEffectHelper.FractalNoise(x * 0.024f, y * 0.0026f, 4, 2.2f, 0.48f, Seed ^ 337);
                float stainMask = ProceduralEffectHelper.SmoothStep(0.54f, 0.92f, chemNoise) * stains;
                stainMask += MathF.Max(0f, streakNoise - 0.72f) * stains * 0.45f;

                float edgeDistance = Math.Min(Math.Min(x, width - 1 - x), Math.Min(y, height - 1 - y));
                float edge01 = 1f - Math.Clamp(edgeDistance / Math.Max(1f, Math.Min(width, height) * 0.16f), 0f, 1f);
                float silverMask = silvering * edge01 * MathF.Pow(MathF.Max(lum, 0.02f), 0.55f);

                float radial = MathF.Sqrt(MathF.Pow(x - centerX, 2f) + MathF.Pow(y - centerY, 2f)) / maxDist;
                float vignetteMask = 1f - (vignette * MathF.Pow(radial, 1.95f) * 0.78f);
                vignetteMask = ProceduralEffectHelper.Clamp01(vignetteMask);

                float scratchMask = 0f;
                if (ProceduralEffectHelper.Hash01(x / 3, y, Seed ^ 173) > 0.9975f)
                {
                    scratchMask += scratches * 0.55f;
                }

                if (ProceduralEffectHelper.Hash01(x, y / 5, Seed ^ 811) > 0.9984f)
                {
                    scratchMask += scratches * 0.35f;
                }

                float r = tone.Red / 255f;
                float g = tone.Green / 255f;
                float b = tone.Blue / 255f;

                r = ProceduralEffectHelper.Lerp(r, 0.88f, silverMask * 0.34f);
                g = ProceduralEffectHelper.Lerp(g, 0.84f, silverMask * 0.30f);
                b = ProceduralEffectHelper.Lerp(b, 0.92f, silverMask * 0.38f);

                r = ProceduralEffectHelper.Clamp01((r - (stainMask * 0.10f) + scratchMask) * vignetteMask);
                g = ProceduralEffectHelper.Clamp01((g - (stainMask * 0.08f) + scratchMask) * vignetteMask);
                b = ProceduralEffectHelper.Clamp01((b - (stainMask * 0.06f) + scratchMask) * vignetteMask);

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