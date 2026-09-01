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

public sealed class CyanotypeImageEffect : ImageEffectBase
{
    public override string Id => "cyanotype";
    public override string Name => "Cyanotype";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.swatch_book;
    public override string Description => "Simulates a cyanotype photographic printing process with blue-toned output.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<CyanotypeImageEffect>("contrast", "Contrast", 50, 200, 118, (e, v) => e.Contrast = v),
        EffectParameters.FloatSlider<CyanotypeImageEffect>("paper_texture", "Paper texture", 0, 100, 34, (e, v) => e.PaperTexture = v),
        EffectParameters.FloatSlider<CyanotypeImageEffect>("stain", "Stain", 0, 100, 28, (e, v) => e.Stain = v),
        EffectParameters.FloatSlider<CyanotypeImageEffect>("grain", "Grain", 0, 100, 12, (e, v) => e.Grain = v),
        EffectParameters.FloatSlider<CyanotypeImageEffect>("vignette", "Vignette", 0, 100, 20, (e, v) => e.Vignette = v)
    ];

    public float Contrast { get; set; } = 118f;
    public float PaperTexture { get; set; } = 34f;
    public float Stain { get; set; } = 28f;
    public float Grain { get; set; } = 12f;
    public float Vignette { get; set; } = 20f;
    public int Seed { get; set; } = 1842;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float contrast = Math.Clamp(Contrast, 50f, 200f) / 100f;
        float paperTexture = Math.Clamp(PaperTexture, 0f, 100f) / 100f;
        float stain = Math.Clamp(Stain, 0f, 100f) / 100f;
        float grain = Math.Clamp(Grain, 0f, 100f) / 100f;
        float vignette = Math.Clamp(Vignette, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        float centerX = (width - 1) * 0.5f;
        float centerY = (height - 1) * 0.5f;
        float maxDist = MathF.Sqrt((centerX * centerX) + (centerY * centerY));
        maxDist = MathF.Max(1f, maxDist);

        SKColor paper = new(241, 236, 220);
        SKColor mid = new(87, 136, 170);
        SKColor deep = new(11, 45, 92);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float lum = AnalogEffectHelper.ApplyContrast(AnalogEffectHelper.Luminance01(src), contrast);
                float ink = 1f - lum;

                SKColor tone = ink < 0.36f
                    ? AnalogEffectHelper.LerpColor(paper, mid, ink / 0.36f)
                    : AnalogEffectHelper.LerpColor(mid, deep, (ink - 0.36f) / 0.64f);

                float paperNoise = (ProceduralEffectHelper.FractalNoise(x * 0.022f, y * 0.022f, 4, 2.1f, 0.55f, Seed) - 0.5f) * paperTexture;
                float stainNoise = ProceduralEffectHelper.FractalNoise(x * 0.006f, y * 0.006f, 5, 2.0f, 0.5f, Seed ^ 901);
                float stainMask = ProceduralEffectHelper.SmoothStep(0.56f, 0.92f, stainNoise) * stain;
                float grainNoise = ((ProceduralEffectHelper.Hash01(x, y, Seed ^ 2111) * 2f) - 1f) * grain * 0.08f;

                float radial = MathF.Sqrt(MathF.Pow(x - centerX, 2f) + MathF.Pow(y - centerY, 2f)) / maxDist;
                float vignetteMask = 1f - (vignette * MathF.Pow(radial, 1.7f) * 0.24f);
                vignetteMask = ProceduralEffectHelper.Clamp01(vignetteMask);

                float r = (tone.Red / 255f);
                float g = (tone.Green / 255f);
                float b = (tone.Blue / 255f);

                r = ProceduralEffectHelper.Clamp01((r + (paperNoise * 0.08f) + (stainMask * 0.06f) + grainNoise) * vignetteMask);
                g = ProceduralEffectHelper.Clamp01((g + (paperNoise * 0.10f) + (stainMask * 0.08f) + grainNoise) * vignetteMask);
                b = ProceduralEffectHelper.Clamp01((b + (paperNoise * 0.14f) + (stainMask * 0.10f) + grainNoise) * vignetteMask);

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