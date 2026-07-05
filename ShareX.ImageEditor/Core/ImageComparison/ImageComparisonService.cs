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

using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageComparison;

public sealed class ImageComparisonService
{
    public unsafe ImageComparisonResult Compare(SKBitmap image1, SKBitmap image2, int maxDiffPreviewDimension = 2048)
    {
        ArgumentNullException.ThrowIfNull(image1);
        ArgumentNullException.ThrowIfNull(image2);

        if (maxDiffPreviewDimension <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDiffPreviewDimension));
        }

        int width = Math.Max(image1.Width, image2.Width);
        int height = Math.Max(image1.Height, image2.Height);

        if (width <= 0 || height <= 0)
        {
            return new ImageComparisonResult(new SKBitmap(1, 1), 1, 1);
        }

        SKBitmap? convertedImage1 = null;
        SKBitmap? convertedImage2 = null;

        try
        {
            SKBitmap comparisonImage1 = image1;
            SKBitmap comparisonImage2 = image2;

            if (!CanCompareRawPixels(image1, image2))
            {
                convertedImage1 = ConvertToComparisonFormat(image1);
                convertedImage2 = ConvertToComparisonFormat(image2);
                comparisonImage1 = convertedImage1;
                comparisonImage2 = convertedImage2;
            }

            int commonWidth = Math.Min(comparisonImage1.Width, comparisonImage2.Width);
            int commonHeight = Math.Min(comparisonImage1.Height, comparisonImage2.Height);
            double previewScale = Math.Min(1d, maxDiffPreviewDimension / (double)Math.Max(width, height));
            int previewWidth = Math.Max(1, (int)Math.Round(width * previewScale));
            int previewHeight = Math.Max(1, (int)Math.Round(height * previewScale));

            SKBitmap diffBitmap = new(new SKImageInfo(
                previewWidth,
                previewHeight,
                SKColorType.Bgra8888,
                SKAlphaType.Opaque));
            diffBitmap.Erase(SKColors.Black);

            IntPtr pixels1 = comparisonImage1.GetPixels();
            IntPtr pixels2 = comparisonImage2.GetPixels();
            IntPtr diffPixels = diffBitmap.GetPixels();
            int rowBytes1 = comparisonImage1.RowBytes;
            int rowBytes2 = comparisonImage2.RowBytes;
            int diffRowBytes = diffBitmap.RowBytes;
            int[] previewXBySourceX = new int[commonWidth];

            for (int x = 0; x < commonWidth; x++)
            {
                previewXBySourceX[x] = (int)((long)x * previewWidth / width);
            }

            int firstExtraPreviewX = commonWidth < width
                ? (int)((long)commonWidth * previewWidth / width)
                : previewWidth;
            long[] matchingPixelsByPreviewRow = new long[previewHeight];

            Parallel.For(0, previewHeight, previewY =>
            {
                int sourceYStart = DivideRoundUp((long)previewY * height, previewHeight);
                int sourceYEnd = DivideRoundUp((long)(previewY + 1) * height, previewHeight);
                uint* diffRow = (uint*)((byte*)diffPixels + previewY * diffRowBytes);
                long rowMatchingPixels = 0;

                if (sourceYEnd > commonHeight)
                {
                    new Span<uint>(diffRow, previewWidth).Fill(uint.MaxValue);
                }
                else if (firstExtraPreviewX < previewWidth)
                {
                    new Span<uint>(diffRow + firstExtraPreviewX, previewWidth - firstExtraPreviewX).Fill(uint.MaxValue);
                }

                int comparableSourceYEnd = Math.Min(sourceYEnd, commonHeight);

                for (int sourceY = sourceYStart; sourceY < comparableSourceYEnd; sourceY++)
                {
                    uint* row1 = (uint*)((byte*)pixels1 + sourceY * rowBytes1);
                    uint* row2 = (uint*)((byte*)pixels2 + sourceY * rowBytes2);

                    for (int x = 0; x < commonWidth; x++)
                    {
                        if (row1[x] == row2[x])
                        {
                            rowMatchingPixels++;
                        }
                        else
                        {
                            diffRow[previewXBySourceX[x]] = uint.MaxValue;
                        }
                    }
                }

                matchingPixelsByPreviewRow[previewY] = rowMatchingPixels;
            });

            long matchingPixels = matchingPixelsByPreviewRow.Sum();
            long totalPixels = (long)width * height;
            return new ImageComparisonResult(diffBitmap, matchingPixels, totalPixels);
        }
        finally
        {
            convertedImage1?.Dispose();
            convertedImage2?.Dispose();
        }
    }

    private static bool CanCompareRawPixels(SKBitmap image1, SKBitmap image2)
    {
        return image1.BytesPerPixel == sizeof(uint) &&
            image2.BytesPerPixel == sizeof(uint) &&
            image1.ColorType == image2.ColorType &&
            image1.AlphaType == image2.AlphaType;
    }

    private static SKBitmap ConvertToComparisonFormat(SKBitmap source)
    {
        SKImageInfo comparisonInfo = new(
            source.Width,
            source.Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul);
        SKBitmap converted = new(comparisonInfo);

        using SKPixmap sourcePixels = source.PeekPixels();
        if (!sourcePixels.ReadPixels(comparisonInfo, converted.GetPixels(), converted.RowBytes, 0, 0))
        {
            converted.Dispose();
            throw new InvalidOperationException("Unable to convert image pixels for comparison.");
        }

        return converted;
    }

    private static int DivideRoundUp(long value, int divisor)
    {
        return (int)((value + divisor - 1) / divisor);
    }
}
