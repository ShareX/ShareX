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

public sealed class SurfaceBlurImageEffect : ImageEffectBase
{
    public override string Id => "surface_blur";
    public override string Name => "Surface blur";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.waves;
    public override string Description => "Blurs while preserving edges based on color similarity.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<SurfaceBlurImageEffect>("radius", "Radius", 1, 8, 3, (e, v) => e.Radius = v),
        EffectParameters.IntSlider<SurfaceBlurImageEffect>("threshold", "Threshold", 1, 100, 24, (e, v) => e.Threshold = v),
    ];

    public int Radius { get; set; } = 3; // 1..8
    public int Threshold { get; set; } = 24; // 1..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int radius = Math.Clamp(Radius, 1, 8);
        int threshold = Math.Clamp(Threshold, 1, 100);
        int thresholdSum = threshold * 3;
        float thresholdWeightScale = 0.65f / Math.Max(1, thresholdSum);

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        int diameter = (radius * 2) + 1;
        int maxNeighbors = (diameter * diameter) - 1;
        int[] offsetX = new int[maxNeighbors];
        int[] offsetY = new int[maxNeighbors];
        int offsetCount = 0;

        for (int ky = -radius; ky <= radius; ky++)
        {
            for (int kx = -radius; kx <= radius; kx++)
            {
                if (kx == 0 && ky == 0)
                {
                    continue;
                }

                offsetX[offsetCount] = kx;
                offsetY[offsetCount] = ky;
                offsetCount++;
            }
        }

        Parallel.For(0, height, y =>
        {
            int dstRow = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor center = srcPixels[dstRow + x];

                float sumR = center.Red;
                float sumG = center.Green;
                float sumB = center.Blue;
                float sumA = center.Alpha;
                float sumW = 1f;

                for (int i = 0; i < offsetCount; i++)
                {
                    int sx = x + offsetX[i];
                    if (sx < 0) sx = 0;
                    else if (sx > right) sx = right;

                    int sy = y + offsetY[i];
                    if (sy < 0) sy = 0;
                    else if (sy > bottom) sy = bottom;

                    SKColor c = srcPixels[(sy * width) + sx];

                    int colorDelta = Math.Abs(c.Red - center.Red)
                        + Math.Abs(c.Green - center.Green)
                        + Math.Abs(c.Blue - center.Blue);

                    if (colorDelta > thresholdSum)
                    {
                        continue;
                    }

                    float w = 1f - (colorDelta * thresholdWeightScale);
                    sumR += c.Red * w;
                    sumG += c.Green * w;
                    sumB += c.Blue * w;
                    sumA += c.Alpha * w;
                    sumW += w;
                }

                float inv = 1f / Math.Max(0.0001f, sumW);
                dstPixels[dstRow + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(sumR * inv),
                    ProceduralEffectHelper.ClampToByte(sumG * inv),
                    ProceduralEffectHelper.ClampToByte(sumB * inv),
                    ProceduralEffectHelper.ClampToByte(sumA * inv));
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}