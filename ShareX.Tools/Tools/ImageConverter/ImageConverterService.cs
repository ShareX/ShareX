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

using ShareX.HelpersLib;
using SkiaSharp;

namespace ShareX.Tools;

public enum ImageConverterOutputFormat
{
    Png,
    Jpeg,
    Webp,
    Gif,
    Bmp
}

public readonly record struct ImageConverterPreview(byte[] Data, int Width, int Height);

public static class ImageConverterService
{
    private const int MaxPreviewDimension = 1600;

    public static SKBitmap? LoadImage(string filePath)
    {
        using SKCodec? codec = SKCodec.Create(filePath);
        if (codec == null)
        {
            return null;
        }

        SKBitmap? source = SKBitmap.Decode(codec);
        if (source == null || !HelpersOptions.RotateImageByExifOrientationData ||
            codec.EncodedOrigin == SKEncodedOrigin.TopLeft)
        {
            return source;
        }

        using (source)
        {
            int width = source.Width;
            int height = source.Height;
            SKMatrix transform = codec.EncodedOrigin switch
            {
                SKEncodedOrigin.TopRight => new(-1, 0, width, 0, 1, 0, 0, 0, 1),
                SKEncodedOrigin.BottomRight => new(-1, 0, width, 0, -1, height, 0, 0, 1),
                SKEncodedOrigin.BottomLeft => new(1, 0, 0, 0, -1, height, 0, 0, 1),
                SKEncodedOrigin.LeftTop => new(0, 1, 0, 1, 0, 0, 0, 0, 1),
                SKEncodedOrigin.RightTop => new(0, -1, height, 1, 0, 0, 0, 0, 1),
                SKEncodedOrigin.RightBottom => new(0, -1, height, -1, 0, width, 0, 0, 1),
                SKEncodedOrigin.LeftBottom => new(0, 1, 0, -1, 0, width, 0, 0, 1),
                _ => SKMatrix.Identity
            };

            bool swapDimensions = codec.EncodedOrigin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
                or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
            SKBitmap output = new(new SKImageInfo(swapDimensions ? height : width,
                swapDimensions ? width : height, SKColorType.Bgra8888, SKAlphaType.Premul, source.ColorSpace));
            using SKCanvas canvas = new(output);
            canvas.Clear(SKColors.Transparent);
            canvas.SetMatrix(transform);
            canvas.DrawBitmap(source, 0, 0);
            canvas.Flush();
            return output;
        }
    }

    public static ImageConverterPreview CreatePreview(string filePath, ImageConverterOutputFormat format,
        int quality, SKColor backgroundColor, SKPngEncoderOptions? pngOptions = null,
        SKJpegEncoderDownsample jpegSubsampling = SKJpegEncoderDownsample.Downsample420,
        GIFQuality gifQuality = GIFQuality.Adaptive, PNGBitDepth pngBitDepth = PNGBitDepth.Automatic,
        BMPBitDepth bmpBitDepth = BMPBitDepth.Bit24,
        SKWebpEncoderCompression webpCompression = SKWebpEncoderCompression.Lossy)
    {
        using SKBitmap? source = LoadImage(filePath);
        if (source == null)
        {
            return new ImageConverterPreview([], 0, 0);
        }

        SKSizeI previewSize = GetPreviewSize(new SKSizeI(source.Width, source.Height));
        using SKBitmap preview = CreatePreviewBitmap(source, previewSize);
        using SKData data = Encode(preview, format, quality, backgroundColor, pngOptions, jpegSubsampling,
            gifQuality, pngBitDepth, bmpBitDepth, webpCompression);
        return new ImageConverterPreview(data.ToArray(), source.Width, source.Height);
    }

    public static string GetFileExtension(ImageConverterOutputFormat format) => format switch
    {
        ImageConverterOutputFormat.Png => "png",
        ImageConverterOutputFormat.Jpeg => "jpg",
        ImageConverterOutputFormat.Webp => "webp",
        ImageConverterOutputFormat.Gif => "gif",
        ImageConverterOutputFormat.Bmp => "bmp",
        _ => throw new ArgumentOutOfRangeException(nameof(format))
    };

    public static void Save(SKBitmap image, string filePath, ImageConverterOutputFormat format, int quality,
        SKColor backgroundColor, SKPngEncoderOptions? pngOptions = null,
        SKJpegEncoderDownsample jpegSubsampling = SKJpegEncoderDownsample.Downsample420,
        GIFQuality gifQuality = GIFQuality.Adaptive, PNGBitDepth pngBitDepth = PNGBitDepth.Automatic,
        BMPBitDepth bmpBitDepth = BMPBitDepth.Bit24,
        SKWebpEncoderCompression webpCompression = SKWebpEncoderCompression.Lossy)
    {
        using SKData data = Encode(image, format, quality, backgroundColor, pngOptions, jpegSubsampling,
            gifQuality, pngBitDepth, bmpBitDepth, webpCompression);
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        data.SaveTo(stream);
    }

    private static SKData Encode(SKBitmap bitmap, ImageConverterOutputFormat format, int quality,
        SKColor backgroundColor, SKPngEncoderOptions? pngOptions, SKJpegEncoderDownsample jpegSubsampling,
        GIFQuality gifQuality, PNGBitDepth pngBitDepth, BMPBitDepth bmpBitDepth, SKWebpEncoderCompression webpCompression)
    {
        if (format == ImageConverterOutputFormat.Png)
        {
            using SKBitmap? flattened = pngBitDepth == PNGBitDepth.Bit24 ? FlattenBackground(bitmap, backgroundColor) : null;
            using MemoryStream stream = new();
            SkiaImageHelpers.SavePNG(flattened ?? bitmap, stream, pngBitDepth, long.MaxValue,
                pngOptions ?? new SKPngEncoderOptions(SKPngEncoderFilterFlags.AllFilters, 1));
            return SKData.CreateCopy(stream.ToArray());
        }

        if (format == ImageConverterOutputFormat.Gif)
        {
            using MemoryStream stream = SkiaImageHelpers.SaveGIF(bitmap, gifQuality);
            return SKData.CreateCopy(stream.ToArray());
        }

        if (format == ImageConverterOutputFormat.Bmp)
        {
            using SKBitmap? flattened = bmpBitDepth == BMPBitDepth.Bit24 ? FlattenBackground(bitmap, backgroundColor) : null;
            using MemoryStream stream = new();
            SkiaImageHelpers.SaveBMP(flattened ?? bitmap, stream, bmpBitDepth);
            return SKData.CreateCopy(stream.ToArray());
        }

        SKData? data;
        if (format == ImageConverterOutputFormat.Jpeg)
        {
            using SKBitmap flattened = FlattenBackground(bitmap, backgroundColor);
            using SKPixmap pixels = flattened.PeekPixels();
            data = pixels.Encode(SkiaImageHelpers.GetJPEGEncoderOptions(quality, jpegSubsampling));
        }
        else if (format == ImageConverterOutputFormat.Webp)
        {
            using SKPixmap pixels = bitmap.PeekPixels();
            data = pixels.Encode(new SKWebpEncoderOptions(webpCompression, Math.Clamp(quality, 0, 100)));
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(format));
        }

        return data
            ?? throw new InvalidOperationException(string.Format(
                Localization.Strings.ImageConverterService_Failed_to_encode_image, format));
    }

    private static SKBitmap FlattenBackground(SKBitmap source, SKColor backgroundColor)
    {
        SKBitmap output = new(new SKImageInfo(source.Width, source.Height,
            SKColorType.Bgra8888, SKAlphaType.Opaque, source.ColorSpace));
        using SKCanvas canvas = new(output);
        canvas.Clear(new SKColor(backgroundColor.Red, backgroundColor.Green, backgroundColor.Blue));
        canvas.DrawBitmap(source, 0, 0);
        canvas.Flush();
        return output;
    }

    private static SKBitmap CreatePreviewBitmap(SKBitmap source, SKSizeI size)
    {
        SKBitmap output = new(new SKImageInfo(size.Width, size.Height,
            SKColorType.Bgra8888, SKAlphaType.Premul, source.ColorSpace));
        using SKCanvas canvas = new(output);
        canvas.Clear(SKColors.Transparent);
        using SKImage image = SKImage.FromBitmap(source);
        canvas.DrawImage(image, new SKRect(0, 0, size.Width, size.Height),
            new SKSamplingOptions(SKCubicResampler.CatmullRom));
        canvas.Flush();
        return output;
    }

    private static SKSizeI GetPreviewSize(SKSizeI source)
    {
        int largestDimension = Math.Max(source.Width, source.Height);
        if (largestDimension <= MaxPreviewDimension)
        {
            return source;
        }

        double scale = MaxPreviewDimension / (double)largestDimension;
        return new SKSizeI(Math.Max(1, (int)Math.Round(source.Width * scale)),
            Math.Max(1, (int)Math.Round(source.Height * scale)));
    }
}
