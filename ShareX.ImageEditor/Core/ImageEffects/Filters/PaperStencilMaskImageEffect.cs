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

public sealed class PaperStencilMaskImageEffect : ImageEffectBase
{
    public override string Id => "paper_stencil_mask";
    public override string Name => "Paper stencil mask";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.stamp;
    public override string Description => "Applies a paper stencil mask effect with adjustable threshold and edge strength.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<PaperStencilMaskImageEffect>("threshold", "Threshold", 0f, 255f, 140f, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<PaperStencilMaskImageEffect>("feather_radius", "Feather radius", 0f, 200f, 8f, (e, v) => e.FeatherRadius = v),
        EffectParameters.FloatSlider<PaperStencilMaskImageEffect>("edge_strength", "Edge strength", 0f, 100f, 70f, (e, v) => e.EdgeStrength = v),
        EffectParameters.FloatSlider<PaperStencilMaskImageEffect>("background_dim", "Background dim", 0f, 100f, 35f, (e, v) => e.BackgroundDim = v),
        EffectParameters.Bool<PaperStencilMaskImageEffect>("invert_mask", "Invert mask", false, (e, v) => e.InvertMask = v),
        EffectParameters.Color<PaperStencilMaskImageEffect>("stencil_color", "Stencil color", new SKColor(0, 0, 0, 220), (e, v) => e.StencilColor = v),
    ];

    public float Threshold { get; set; } = 140f; // 0..255
    public float FeatherRadius { get; set; } = 8f; // 0..200 heuristic
    public float EdgeStrength { get; set; } = 70f; // 0..100
    public float BackgroundDim { get; set; } = 35f; // 0..100
    public bool InvertMask { get; set; } = false;
    public int Seed { get; set; } = 1337;
    public SKColor StencilColor { get; set; } = new SKColor(0, 0, 0, 220);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float threshold01 = Math.Clamp(Threshold, 0f, 255f) / 255f;
        float featherPx = Math.Clamp(FeatherRadius, 0f, 200f);
        float feather = Math.Clamp(featherPx, 0f, 30f) / 30f; // 0..1
        float featherRange = 0.02f + 0.13f * feather; // luminance range around threshold

        float edgeStrength01 = Math.Clamp(EdgeStrength, 0f, 100f) / 100f;
        if (edgeStrength01 <= 0f && BackgroundDim <= 0f)
        {
            return source.Copy();
        }

        float bgDim01 = Math.Clamp(BackgroundDim, 0f, 100f) / 100f;
        float dimFactor = 1f - bgDim01;
        dimFactor = ProceduralEffectHelper.Clamp01(dimFactor);

        float stencilMixCap = (StencilColor.Alpha / 255f) * edgeStrength01;
        float stencilR = StencilColor.Red / 255f;
        float stencilG = StencilColor.Green / 255f;
        float stencilB = StencilColor.Blue / 255f;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                int idx = row + x;
                SKColor src = srcPixels[idx];

                float lum01 = ((0.2126f * src.Red) + (0.7152f * src.Green) + (0.0722f * src.Blue)) / 255f;

                // Optional tiny deterministic jitter to avoid banding at large feathers.
                float jitter = (ProceduralEffectHelper.Hash01(x, y, Seed ^ 0xCAFE) * 2f - 1f) * 0.003f;
                float t0 = threshold01 - featherRange + jitter;
                float t1 = threshold01 + featherRange + jitter;

                float maskAlpha = ProceduralEffectHelper.SmoothStep(t0, t1, lum01);
                if (InvertMask)
                {
                    maskAlpha = 1f - maskAlpha;
                }

                float stencilMix = maskAlpha * stencilMixCap;
                stencilMix = ProceduralEffectHelper.Clamp01(stencilMix);

                float outR = (src.Red / 255f) * dimFactor;
                float outG = (src.Green / 255f) * dimFactor;
                float outB = (src.Blue / 255f) * dimFactor;

                outR = ProceduralEffectHelper.Lerp(outR, stencilR, stencilMix);
                outG = ProceduralEffectHelper.Lerp(outG, stencilG, stencilMix);
                outB = ProceduralEffectHelper.Lerp(outB, stencilB, stencilMix);

                dstPixels[idx] = new SKColor(
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
}