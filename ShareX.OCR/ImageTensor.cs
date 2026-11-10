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

namespace ShareX.OCR;

internal static class ImageTensor
{
    public static SKBitmap CreateBitmap(int width, int height)
    {
        return new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Opaque));
    }

    // Draws the image on white so transparent pixels read as background, in the pixel format the tensors are filled from.
    public static SKBitmap ToOpaqueBgra(SKBitmap source)
    {
        SKBitmap result = CreateBitmap(source.Width, source.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.White);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    public static SKBitmap Resize(SKImage source, int width, int height)
    {
        SKBitmap result = CreateBitmap(width, height);
        using SKCanvas canvas = new(result);
        canvas.DrawImage(source, SKRect.Create(width, height), GetSampling(width < source.Width || height < source.Height));
        return result;
    }

    // Draws the area inside a quadrilateral as an upright image of the given size. The quadrilateral is a rectangle,
    // so an affine transform from its origin corner and two edges is exact. Pixels outside the image repeat the edge.
    public static SKBitmap Crop(SKImage source, SKPoint origin, SKPoint xEdge, SKPoint yEdge, int width, int height)
    {
        SKBitmap result = CreateBitmap(width, height);
        SKMatrix targetToSource = new(xEdge.X / width, yEdge.X / height, origin.X, xEdge.Y / width, yEdge.Y / height, origin.Y, 0, 0, 1);

        if (!targetToSource.TryInvert(out SKMatrix sourceToTarget))
        {
            return result;
        }

        bool downscale = xEdge.Length > width * 1.01f || yEdge.Length > height * 1.01f;
        using SKShader shader = source.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, GetSampling(downscale), sourceToTarget);
        using SKPaint paint = new() { Shader = shader };
        using SKCanvas canvas = new(result);
        canvas.DrawRect(SKRect.Create(width, height), paint);
        return result;
    }

    // Writes a Bgra8888 bitmap into one image of an NCHW tensor in BGR channel order, which PaddleOCR models are trained on.
    // Columns past the bitmap width are left as they are (zero), which is how PaddleOCR pads recognition batches.
    public static void Fill(Span<float> tensor, int batchIndex, int tensorWidth, int tensorHeight, SKBitmap bitmap, float[] mean, float[] std)
    {
        ReadOnlySpan<byte> pixels = bitmap.GetPixelSpan();
        int rowBytes = bitmap.RowBytes;
        int planeSize = tensorWidth * tensorHeight;
        int imageOffset = batchIndex * planeSize * 3;
        int width = Math.Min(bitmap.Width, tensorWidth);
        int height = Math.Min(bitmap.Height, tensorHeight);
        float[] scale = [1f / (255f * std[0]), 1f / (255f * std[1]), 1f / (255f * std[2])];
        float[] offset = [mean[0] / std[0], mean[1] / std[1], mean[2] / std[2]];

        for (int y = 0; y < height; y++)
        {
            int rowOffset = y * rowBytes;
            int tensorRow = imageOffset + (y * tensorWidth);

            for (int x = 0; x < width; x++)
            {
                int pixel = rowOffset + (x * 4);
                int index = tensorRow + x;
                tensor[index] = (pixels[pixel] * scale[0]) - offset[0];
                tensor[index + planeSize] = (pixels[pixel + 1] * scale[1]) - offset[1];
                tensor[index + (planeSize * 2)] = (pixels[pixel + 2] * scale[2]) - offset[2];
            }
        }
    }

    private static SKSamplingOptions GetSampling(bool downscale)
    {
        return downscale ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear) : new SKSamplingOptions(SKCubicResampler.CatmullRom);
    }
}
