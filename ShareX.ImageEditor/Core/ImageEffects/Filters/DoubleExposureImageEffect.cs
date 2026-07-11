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

public sealed class DoubleExposureImageEffect : ImageEffectBase
{
    public override string Id => "double_exposure";
    public override string Name => "Double exposure";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.images;
    public override string Description => "Blends a blurred ghost of the image over itself to simulate double exposure.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<DoubleExposureImageEffect>("blend_amount", "Blend amount", 0, 100, 62, (e, v) => e.BlendAmount = v),
        EffectParameters.IntNumeric<DoubleExposureImageEffect>("offset_x", "Offset X", -500, 500, 22, (e, v) => e.OffsetX = v),
        EffectParameters.IntNumeric<DoubleExposureImageEffect>("offset_y", "Offset Y", -500, 500, -14, (e, v) => e.OffsetY = v),
        EffectParameters.FloatSlider<DoubleExposureImageEffect>("ghost_blur", "Ghost blur", 0, 30, 8, (e, v) => e.GhostBlur = v),
        EffectParameters.FloatSlider<DoubleExposureImageEffect>("highlight_bias", "Highlight bias", 0, 100, 55, (e, v) => e.HighlightBias = v),
        EffectParameters.FloatSlider<DoubleExposureImageEffect>("contrast", "Contrast", 50, 200, 112, (e, v) => e.Contrast = v)
    ];

    public float BlendAmount { get; set; } = 62f;
    public int OffsetX { get; set; } = 22;
    public int OffsetY { get; set; } = -14;
    public float GhostBlur { get; set; } = 8f;
    public float HighlightBias { get; set; } = 55f;
    public float Contrast { get; set; } = 112f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float blend = Math.Clamp(BlendAmount, 0f, 100f) / 100f;
        float blur = Math.Clamp(GhostBlur, 0f, 30f);
        float highlightBias = Math.Clamp(HighlightBias, 0f, 100f) / 100f;
        float contrast = Math.Clamp(Contrast, 50f, 200f) / 100f;

        if (blend <= 0.0001f && blur <= 0.0001f && OffsetX == 0 && OffsetY == 0)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;

        using SKBitmap? ghostBitmap = blur > 0.01f ? AnalogEffectHelper.CreateBlurredClamp(source, blur) : null;
        SKColor[] ghostPixels = ghostBitmap?.Pixels ?? srcPixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                SKColor ghost = AnalogEffectHelper.Sample(ghostPixels, width, height, x - OffsetX, y - OffsetY);

                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;

                float ghostLum = AnalogEffectHelper.Luminance01(ghost);
                float ghostR = ProceduralEffectHelper.Lerp(ghost.Red / 255f, ghostLum * (1.12f + (highlightBias * 0.48f)), 0.68f);
                float ghostG = ProceduralEffectHelper.Lerp(ghost.Green / 255f, ghostLum * 0.95f, 0.78f);
                float ghostB = ProceduralEffectHelper.Lerp(ghost.Blue / 255f, ghostLum * 0.82f, 0.84f);

                ghostR = AnalogEffectHelper.ApplyContrast(ghostR, contrast);
                ghostG = AnalogEffectHelper.ApplyContrast(ghostG, contrast);
                ghostB = AnalogEffectHelper.ApplyContrast(ghostB, contrast);

                float localBlend = blend * (0.55f + (ghostLum * 0.45f));
                float r = ProceduralEffectHelper.Lerp(sr, AnalogEffectHelper.Screen(sr, ghostR), localBlend);
                float g = ProceduralEffectHelper.Lerp(sg, AnalogEffectHelper.Screen(sg, ghostG), localBlend * 0.92f);
                float b = ProceduralEffectHelper.Lerp(sb, AnalogEffectHelper.Screen(sb, ghostB), localBlend * 0.88f);

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