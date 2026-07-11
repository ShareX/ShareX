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

public sealed class CartoonStickerCutoutImageEffect : ImageEffectBase
{
    public override string Id => "cartoon_sticker_cutout";
    public override string Name => "Cartoon sticker cutout";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sticker;
    public override string Description => "Applies a cartoon sticker cutout effect with quantized colors and ink edges.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<CartoonStickerCutoutImageEffect>("colorLevels", "Color levels", 2, 20, 8, (e, v) => e.ColorLevels = v),
        EffectParameters.IntSlider<CartoonStickerCutoutImageEffect>("edgeThreshold", "Edge threshold", 5, 120, 32, (e, v) => e.EdgeThreshold = v),
        EffectParameters.FloatSlider<CartoonStickerCutoutImageEffect>("inkStrength", "Ink strength", 0f, 100f, 85f, (e, v) => e.InkStrength = v),
        EffectParameters.IntSlider<CartoonStickerCutoutImageEffect>("stickerBorder", "Sticker border", 0, 10, 3, (e, v) => e.StickerBorder = v),
        EffectParameters.FloatSlider<CartoonStickerCutoutImageEffect>("borderStrength", "Border strength", 0f, 100f, 90f, (e, v) => e.BorderStrength = v),
    ];

    public int ColorLevels { get; set; } = 8; // 2..20
    public int EdgeThreshold { get; set; } = 32; // 5..120
    public float InkStrength { get; set; } = 85f; // 0..100
    public int StickerBorder { get; set; } = 3; // 0..10
    public float BorderStrength { get; set; } = 90f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int levels = Math.Clamp(ColorLevels, 2, 20);
        int edgeThreshold = Math.Clamp(EdgeThreshold, 5, 120);
        float inkStrength = Math.Clamp(InkStrength, 0f, 100f) / 100f;
        int borderRadius = Math.Clamp(StickerBorder, 0, 10);
        float borderStrength = Math.Clamp(BorderStrength, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] quantized = new SKColor[srcPixels.Length];
        byte[] edgeMask = new byte[srcPixels.Length];

        float step = 255f / (levels - 1);

        Parallel.For(0, srcPixels.Length, i =>
        {
            SKColor c = srcPixels[i];
            quantized[i] = new SKColor(
                Quantize(c.Red, step),
                Quantize(c.Green, step),
                Quantize(c.Blue, step),
                c.Alpha);
        });

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            int yUp = y > 0 ? y - 1 : 0;
            int yDown = y < bottom ? y + 1 : bottom;

            for (int x = 0; x < width; x++)
            {
                int xLeft = x > 0 ? x - 1 : 0;
                int xRight = x < right ? x + 1 : right;

                SKColor left = quantized[row + xLeft];
                SKColor rightColor = quantized[row + xRight];
                SKColor up = quantized[(yUp * width) + x];
                SKColor down = quantized[(yDown * width) + x];

                int gradX = Math.Abs(rightColor.Red - left.Red)
                    + Math.Abs(rightColor.Green - left.Green)
                    + Math.Abs(rightColor.Blue - left.Blue);

                int gradY = Math.Abs(down.Red - up.Red)
                    + Math.Abs(down.Green - up.Green)
                    + Math.Abs(down.Blue - up.Blue);

                int edge = (gradX + gradY) / 6; // ~0..255
                int normalized = (edge - edgeThreshold) * 255 / Math.Max(1, 255 - edgeThreshold);
                edgeMask[row + x] = normalized <= 0 ? (byte)0 : (byte)Math.Min(255, normalized);
            }
        });

        byte[]? borderMask = null;
        if (borderRadius > 0 && borderStrength > 0.001f)
        {
            borderMask = new byte[edgeMask.Length];
            int radiusSq = borderRadius * borderRadius;

            Parallel.For(0, height, y =>
            {
                int row = y * width;

                for (int x = 0; x < width; x++)
                {
                    int max = 0;
                    for (int ky = -borderRadius; ky <= borderRadius; ky++)
                    {
                        int sy = y + ky;
                        if (sy < 0) sy = 0;
                        else if (sy > bottom) sy = bottom;

                        int srcRow = sy * width;

                        for (int kx = -borderRadius; kx <= borderRadius; kx++)
                        {
                            if ((kx * kx) + (ky * ky) > radiusSq)
                            {
                                continue;
                            }

                            int sx = x + kx;
                            if (sx < 0) sx = 0;
                            else if (sx > right) sx = right;

                            int value = edgeMask[srcRow + sx];
                            if (value > max)
                            {
                                max = value;
                                if (max == 255) goto Done;
                            }
                        }
                    }

                Done:
                    borderMask[row + x] = (byte)max;
                }
            });
        }

        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        Parallel.For(0, srcPixels.Length, i =>
        {
            SKColor baseColor = quantized[i];
            float r = baseColor.Red;
            float g = baseColor.Green;
            float b = baseColor.Blue;

            float edge = edgeMask[i] / 255f;
            float inkMix = edge * inkStrength;

            if (borderMask != null)
            {
                float border = borderMask[i] / 255f;
                float borderOnly = MathF.Max(0f, border - (edge * 0.65f));
                float borderMix = borderOnly * borderStrength;

                if (borderMix > 0.001f)
                {
                    r = ProceduralEffectHelper.Lerp(r, 255f, borderMix);
                    g = ProceduralEffectHelper.Lerp(g, 255f, borderMix);
                    b = ProceduralEffectHelper.Lerp(b, 255f, borderMix);
                }
            }

            if (inkMix > 0.001f)
            {
                r = ProceduralEffectHelper.Lerp(r, 0f, inkMix);
                g = ProceduralEffectHelper.Lerp(g, 0f, inkMix);
                b = ProceduralEffectHelper.Lerp(b, 0f, inkMix);
            }

            dstPixels[i] = new SKColor(
                ProceduralEffectHelper.ClampToByte(r),
                ProceduralEffectHelper.ClampToByte(g),
                ProceduralEffectHelper.ClampToByte(b),
                baseColor.Alpha);
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static byte Quantize(byte value, float step)
    {
        float q = MathF.Round(value / step) * step;
        return ProceduralEffectHelper.ClampToByte(q);
    }
}