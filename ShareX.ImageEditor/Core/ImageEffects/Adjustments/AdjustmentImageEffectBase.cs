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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public abstract class AdjustmentImageEffectBase : ImageEffectBase
{
    public override ImageEffectCategory Category => ImageEffectCategory.Adjustments;

    protected static SKBitmap ApplyColorMatrix(SKBitmap source, float[] matrix)
    {
        using SKColorFilter filter = SKColorFilter.CreateColorMatrix(matrix);
        return ApplyColorFilter(source, filter);
    }

    protected static SKBitmap ApplyColorFilter(SKBitmap source, SKColorFilter filter)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        SKBitmap result = new(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        using SKPaint paint = new() { ColorFilter = filter };
        canvas.DrawBitmap(source, 0, 0, paint);
        return result;
    }

    protected unsafe static SKBitmap ApplyPixelOperation(SKBitmap source, Func<SKColor, SKColor> operation)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        SKBitmap result = new(source.Width, source.Height, source.ColorType, source.AlphaType);

        if (source.ColorType == SKColorType.Bgra8888)
        {
            // Rows are addressed with each bitmap's own stride: a cropped image is an ExtractSubset view that keeps its parent's
            // row width, so its pixels are not contiguous.
            int width = source.Width, height = source.Height;
            byte* srcBase = (byte*)source.GetPixels();
            byte* dstBase = (byte*)result.GetPixels();

            for (int y = 0; y < height; y++)
            {
                SKColor* srcPtr = (SKColor*)(srcBase + (long)y * source.RowBytes);
                SKColor* dstPtr = (SKColor*)(dstBase + (long)y * result.RowBytes);

                for (int x = 0; x < width; x++)
                {
                    dstPtr[x] = operation(srcPtr[x]);
                }
            }
        }
        else
        {
            SKColor[] srcPixels = source.Pixels;
            SKColor[] dstPixels = new SKColor[srcPixels.Length];

            for (int i = 0; i < srcPixels.Length; i++)
            {
                dstPixels[i] = operation(srcPixels[i]);
            }

            result.Pixels = dstPixels;
        }

        return result;
    }
}