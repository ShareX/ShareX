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
using System;
using System.Drawing;
using System.IO;

namespace ShareX.HelpersLib;

public enum ImageFileFormat { Png, Jpeg, Gif, Bmp, Webp }

/// <summary>Image operations that own their pixels independently of the source file or stream.</summary>
public static partial class SkiaImageHelpers
{
    public static readonly SKSamplingOptions HighQualitySampling = new(SKCubicResampler.Mitchell);

    public static SKSamplingOptions GetSampling(ImageInterpolationMode mode) => mode switch
    {
        ImageInterpolationMode.NearestNeighbor => new(SKFilterMode.Nearest),
        ImageInterpolationMode.Bilinear or ImageInterpolationMode.HighQualityBilinear => new(SKFilterMode.Linear),
        _ => HighQualitySampling
    };

    public static SKBitmap CreateBitmap(int width, int height)
    {
        SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        bitmap.Erase(SKColors.Transparent);
        return bitmap;
    }

    public static SKColor ToSKColor(this Color color) => new(color.R, color.G, color.B, color.A);

    public static Size GetSize(this SKBitmap bitmap) => new(bitmap.Width, bitmap.Height);

    public static Color ToDrawingColor(this SKColor color) => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

    public static SKBitmap CropBitmap(SKBitmap source, Rectangle rectangle)
    {
        if (source == null || rectangle.Width <= 0 || rectangle.Height <= 0 ||
            !new Rectangle(0, 0, source.Width, source.Height).Contains(rectangle)) return null;
        SKBitmap result = CreateBitmap(rectangle.Width, rectangle.Height);
        using SKCanvas canvas = new(result);
        canvas.DrawBitmap(source, -rectangle.X, -rectangle.Y);
        return result;
    }

    public static SKBitmap ResizeImage(SKBitmap source, int width, int height,
        ImageSamplingMode interpolationMode = ImageSamplingMode.HighQualityBicubic)
    {
        if (width < 1 || height < 1 || source.Width == width && source.Height == height) return source;
        SKBitmap result = CreateBitmap(width, height);
        using (source)
        using (SKCanvas canvas = new(result))
        using (SKImage image = SKImage.FromBitmap(source))
        {
            SKSamplingOptions sampling = interpolationMode switch
            {
                ImageSamplingMode.NearestNeighbor => new(SKFilterMode.Nearest),
                ImageSamplingMode.Bilinear or ImageSamplingMode.HighQualityBilinear => new(SKFilterMode.Linear),
                _ => HighQualitySampling
            };
            canvas.DrawImage(image, new SKRect(0, 0, width, height), sampling);
        }
        return result;
    }

    public static SKBitmap ResizeImage(SKBitmap source, Size size,
        ImageSamplingMode interpolationMode = ImageSamplingMode.HighQualityBicubic)
        => ResizeImage(source, size.Width, size.Height, interpolationMode);

    public static SKBitmap ResizeImage(SKBitmap source, int width, int height, bool allowEnlarge,
        bool centerImage = true) => ResizeImage(source, width, height, allowEnlarge, centerImage, Color.Transparent);

    public static SKBitmap ResizeImage(SKBitmap source, Size size, bool allowEnlarge, bool centerImage = true)
        => ResizeImage(source, size.Width, size.Height, allowEnlarge, centerImage);

    public static SKBitmap ResizeImage(SKBitmap source, int width, int height, bool allowEnlarge,
        bool centerImage, Color backColor)
    {
        double scale = Math.Min(width / (double)source.Width, height / (double)source.Height);
        if (!allowEnlarge) scale = Math.Min(1, scale);
        int resizedWidth = Math.Max(1, (int)(source.Width * scale));
        int resizedHeight = Math.Max(1, (int)(source.Height * scale));
        int x = centerImage ? (width - resizedWidth) / 2 : 0;
        int y = centerImage ? (height - resizedHeight) / 2 : 0;
        SKBitmap result = CreateBitmap(width, height);
        using SKCanvas canvas = new(result);
        canvas.Clear(backColor.ToSKColor());
        canvas.DrawImage(source, x, y, resizedWidth, resizedHeight);
        return result;
    }

    public static SKBitmap ScaleImageFast(SKBitmap source, double scale) => ScaleImageFast(source, scale, scale);
    public static SKBitmap ScaleImageFast(SKBitmap source, double scaleX, double scaleY)
        => Resize(source, Math.Max(1, (int)Math.Round(source.Width * scaleX)), Math.Max(1, (int)Math.Round(source.Height * scaleY)));

    public static SKBitmap AddReflection(SKBitmap source, int percentage, int maxAlpha, int minAlpha)
    {
        int height = Math.Max(1, (int)(source.Height * Math.Clamp(percentage, 1, 100) / 100f));
        SKBitmap result = CreateBitmap(source.Width, height);
        using (SKCanvas canvas = new(result))
        {
            canvas.Translate(0, source.Height);
            canvas.Scale(1, -1);
            canvas.DrawBitmap(source, 0, 0);
        }
        using SkiaPixelBuffer pixels = new(result, true);
        for (int y = 0; y < height; y++)
            for (int x = 0; x < result.Width; x++)
            {
                ColorBgra color = pixels.GetPixel(x, y);
                int alpha = (int)(maxAlpha - (maxAlpha - minAlpha) * y / (float)Math.Max(1, height - 1));
                color.Alpha = (byte)Math.Min(color.Alpha, Math.Clamp(alpha, 0, 255));
                pixels.SetPixel(x, y, color);
            }
        return result;
    }

    public static bool CompareImages(SKBitmap first, SKBitmap second)
    {
        if (first == null || second == null) return false;
        using SkiaPixelBuffer firstPixels = new(first);
        using SkiaPixelBuffer secondPixels = new(second);
        return SkiaPixelBuffer.Compare(firstPixels, secondPixels);
    }

    public static bool IsImageTransparent(SKBitmap bitmap)
    {
        using SkiaPixelBuffer pixels = new(bitmap, true, PixelAccess.ReadOnly);
        return pixels.IsTransparent();
    }

    public static MemoryStream SaveJPEG(SKBitmap bitmap, int quality,
        SKJpegEncoderDownsample subsampling = SKJpegEncoderDownsample.Downsample420)
    {
        MemoryStream stream = new();
        SaveJPEG(bitmap, stream, quality, subsampling);
        return stream;
    }

    public static IndexedImage Quantize(SKBitmap bitmap, GIFQuality quality = GIFQuality.Default)
    {
        SkiaQuantizer quantizer = quality switch
        {
            GIFQuality.Grayscale => new SkiaGrayscaleQuantizer(),
            GIFQuality.Bit4 => new SkiaOctreeQuantizer(15, 4),
            _ => new SkiaOctreeQuantizer(255, 4)
        };
        return quantizer.Quantize(bitmap);
    }

    public static MemoryStream SaveGIF(SKBitmap bitmap, GIFQuality quality)
    {
        MemoryStream stream = new();
        SaveGIF(bitmap, stream, quality);
        return stream;
    }

    public static void SaveGIF(SKBitmap bitmap, Stream stream, GIFQuality quality)
        => Quantize(bitmap, quality).SaveGif(stream);

    public static SKBitmap LoadImage(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return null;

        try
        {
            // As in v22: anything that is not an existing image file (a recorded video, for example) quietly has no image.
            filePath = FileHelpers.GetAbsolutePath(filePath);
            if (string.IsNullOrEmpty(filePath) || !FileHelpers.IsImageFile(filePath) || !File.Exists(filePath)) return null;

            using FileStream stream = File.OpenRead(filePath);
            return Decode(stream, HelpersOptions.RotateImageByExifOrientationData);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            return null;
        }
    }

    public static SKBitmap Decode(Stream stream, bool applyOrientation = true)
    {
        if (!stream.CanSeek)
        {
            using MemoryStream copy = new();
            stream.CopyTo(copy);
            copy.Position = 0;
            return Decode(copy, applyOrientation);
        }
        using SKManagedStream managedStream = new(stream, false);
        using SKCodec codec = SKCodec.Create(managedStream);
        if (codec == null) throw new InvalidDataException("The image format is not supported.");

        SKBitmap bitmap = CreateBitmap(codec.Info.Width, codec.Info.Height);
        SKCodecResult result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
        if (result is not SKCodecResult.Success and not SKCodecResult.IncompleteInput)
        {
            bitmap.Dispose();
            throw new InvalidDataException($"Image decoding failed: {result}.");
        }

        if (!applyOrientation || codec.EncodedOrigin == SKEncodedOrigin.TopLeft) return bitmap;

        // EXIF orientation applies to the decoded pixels, including mirrored orientations.
        bool swapDimensions = (int)codec.EncodedOrigin >= (int)SKEncodedOrigin.LeftTop;
        SKBitmap oriented = CreateBitmap(swapDimensions ? bitmap.Height : bitmap.Width,
            swapDimensions ? bitmap.Width : bitmap.Height);
        using (bitmap)
        using (SKCanvas canvas = new(oriented))
        {
            SKMatrix transform = codec.EncodedOrigin switch
            {
                SKEncodedOrigin.TopRight => new(-1, 0, bitmap.Width, 0, 1, 0, 0, 0, 1),
                SKEncodedOrigin.BottomRight => new(-1, 0, bitmap.Width, 0, -1, bitmap.Height, 0, 0, 1),
                SKEncodedOrigin.BottomLeft => new(1, 0, 0, 0, -1, bitmap.Height, 0, 0, 1),
                SKEncodedOrigin.LeftTop => new(0, 1, 0, 1, 0, 0, 0, 0, 1),
                SKEncodedOrigin.RightTop => new(0, -1, bitmap.Height, 1, 0, 0, 0, 0, 1),
                SKEncodedOrigin.RightBottom => new(0, -1, bitmap.Height, -1, 0, bitmap.Width, 0, 0, 1),
                SKEncodedOrigin.LeftBottom => new(0, 1, 0, -1, 0, bitmap.Width, 0, 0, 1),
                _ => SKMatrix.Identity
            };
            canvas.SetMatrix(transform);
            canvas.DrawBitmap(bitmap, 0, 0);
        }
        return oriented;
    }

    public static void DrawImage(this SKCanvas canvas, SKBitmap bitmap, SKRect source, SKRect destination,
        SKPaint paint = null)
    {
        using SKImage image = SKImage.FromBitmap(bitmap);
        canvas.DrawImage(image, source, destination, HighQualitySampling, paint);
    }

    public static SKBitmap Resize(SKBitmap source, int width, int height)
    {
        SKBitmap output = CreateBitmap(width, height);
        using SKCanvas canvas = new(output);
        canvas.DrawImage(source, new SKRect(0, 0, source.Width, source.Height), new SKRect(0, 0, width, height));
        return output;
    }

    public static SKBitmap FillBackground(SKBitmap source, Color color)
    {
        SKBitmap output = CreateBitmap(source.Width, source.Height);
        using SKCanvas canvas = new(output);
        canvas.Clear(color.ToSKColor());
        canvas.DrawBitmap(source, 0, 0);
        return output;
    }

    public static void Save(this SKBitmap bitmap, Stream stream, SKEncodedImageFormat format, int quality = 100)
    {
        if (format == SKEncodedImageFormat.Gif) { SaveGIF(bitmap, stream, GIFQuality.Default); return; }
        if (format == SKEncodedImageFormat.Bmp) { SaveBMP(bitmap, stream); return; }
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(format, Math.Clamp(quality, 0, 100));
        if (data == null) throw new InvalidDataException($"Image encoding failed: {format}.");
        data.SaveTo(stream);
    }

    private static void SaveBMP(SKBitmap bitmap, Stream stream)
    {
        int stride = checked((bitmap.Width * 3 + 3) / 4 * 4);
        using BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true);
        writer.Write((ushort)0x4D42); writer.Write(checked(54 + stride * bitmap.Height));
        writer.Write(0); writer.Write(54); writer.Write(40); writer.Write(bitmap.Width); writer.Write(bitmap.Height);
        writer.Write((ushort)1); writer.Write((ushort)24); writer.Write(0); writer.Write(checked(stride * bitmap.Height));
        writer.Write(3780); writer.Write(3780); writer.Write(0); writer.Write(0);
        using SkiaPixelBuffer pixels = new(bitmap, true, PixelAccess.ReadOnly);
        byte[] row = new byte[stride];
        for (int y = bitmap.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                ColorBgra color = pixels.GetPixel(x, y);
                row[x * 3] = color.Blue; row[x * 3 + 1] = color.Green; row[x * 3 + 2] = color.Red;
            }
            writer.Write(row);
        }
    }

    public static void Save(this SKBitmap bitmap, string filePath, SKEncodedImageFormat format, int quality = 100)
    {
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        bitmap.Save(stream, format, quality);
    }

    public static void Save(this SKBitmap bitmap, Stream stream, ImageFileFormat format, int quality = 100)
    {
        bitmap.Save(stream, format switch
        {
            ImageFileFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            ImageFileFormat.Gif => SKEncodedImageFormat.Gif,
            ImageFileFormat.Bmp => SKEncodedImageFormat.Bmp,
            ImageFileFormat.Webp => SKEncodedImageFormat.Webp,
            _ => SKEncodedImageFormat.Png
        }, quality);
    }

    public static void Save(this SKBitmap bitmap, string filePath, ImageFileFormat format, int quality = 100)
    {
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        bitmap.Save(stream, format, quality);
    }

    public static ImageFileFormat GetImageFormat(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" or ".jpe" or ".jfif" => ImageFileFormat.Jpeg,
        ".gif" => ImageFileFormat.Gif,
        ".bmp" => ImageFileFormat.Bmp,
        ".webp" => ImageFileFormat.Webp,
        _ => ImageFileFormat.Png
    };

    public static SKBitmap ByteArrayToBitmap(byte[] bytes)
    {
        using MemoryStream stream = new(bytes, false);
        return Decode(stream);
    }

    public static SKBitmap CreateThumbnail(SKBitmap source, int width, int height,
        ImageSamplingMode interpolationMode = ImageSamplingMode.HighQualityBicubic)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        float scale = Math.Max(width / (float)source.Width, height / (float)source.Height);
        float sourceWidth = width / scale, sourceHeight = height / scale;
        SKRect crop = SKRect.Create((source.Width - sourceWidth) / 2, (source.Height - sourceHeight) / 2, sourceWidth, sourceHeight);
        SKBitmap result = CreateBitmap(width, height);
        using (source)
        using (SKCanvas canvas = new(result)) canvas.DrawImage(source, crop, SKRect.Create(width, height));
        return result;
    }

    public static SKJpegEncoderOptions GetJPEGEncoderOptions(int quality, SKJpegEncoderDownsample subsampling)
        => new(Math.Clamp(quality, 0, 100), Enum.IsDefined(subsampling) ? subsampling : SKJpegEncoderDownsample.Downsample420,
            SKJpegEncoderAlphaOption.Ignore);

    public static void SaveJPEG(SKBitmap bitmap, Stream stream, int quality,
        SKJpegEncoderDownsample subsampling = SKJpegEncoderDownsample.Downsample420)
    {
        using SKPixmap pixels = bitmap.PeekPixels();
        if (!pixels.Encode(stream, GetJPEGEncoderOptions(quality, subsampling)))
            throw new InvalidDataException("Image encoding failed: Jpeg.");
    }

    public static void SaveJPEG(SKBitmap bitmap, string path, int quality,
        SKJpegEncoderDownsample subsampling = SKJpegEncoderDownsample.Downsample420)
    {
        using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        SaveJPEG(bitmap, stream, quality, subsampling);
    }
    public static MemoryStream SavePNG(SKBitmap bitmap, PNGBitDepth depth)
    {
        MemoryStream stream = new(); SavePNG(bitmap, stream, depth); return stream;
    }
    public static void SavePNG(SKBitmap bitmap, Stream stream, PNGBitDepth depth)
    {
        SavePNG(bitmap, stream, depth, long.MaxValue);
    }

    // Returns false when the encoded PNG exceeds the limit, so callers can discard it without encoding the remaining rows.
    public static bool SavePNG(SKBitmap bitmap, Stream stream, PNGBitDepth depth, long sizeLimit)
    {
        return SavePNG(bitmap, stream, depth, sizeLimit, SKPngEncoderOptions.Default);
    }

    public static bool SavePNG(SKBitmap bitmap, Stream stream, PNGBitDepth depth, long sizeLimit, SKPngEncoderOptions options)
    {
        using SizeLimitedWStream output = new(stream, sizeLimit);
        bool encoded;

        if (depth == PNGBitDepth.Bit24 || depth == PNGBitDepth.Automatic && !IsImageTransparent(bitmap))
        {
            using SKBitmap opaque = new(new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Opaque));
            using (SkiaPixelBuffer source = new(bitmap, true, PixelAccess.ReadOnly))
            using (SkiaPixelBuffer target = new(opaque, true, PixelAccess.WriteOnly))
            {
                for (int index = 0; index < source.PixelCount; index++)
                {
                    ColorBgra pixel = source.GetPixel(index); pixel.Alpha = 255; target.SetPixel(index, pixel);
                }
            }
            using SKPixmap pixels = opaque.PeekPixels();
            encoded = pixels.Encode(output, options);
        }
        else
        {
            using SKPixmap pixels = bitmap.PeekPixels();
            encoded = pixels.Encode(output, options);
        }

        if (!encoded && !output.SizeLimitExceeded) throw new InvalidDataException("Image encoding failed: Png.");
        return encoded;
    }

    private sealed class SizeLimitedWStream : SKManagedWStream
    {
        private readonly long sizeLimit;
        private long bytesWritten;
        public bool SizeLimitExceeded { get; private set; }

        public SizeLimitedWStream(Stream stream, long sizeLimit) : base(stream, false)
        {
            this.sizeLimit = Math.Max(0, sizeLimit);
        }

        protected override bool OnWrite(IntPtr buffer, IntPtr size)
        {
            long count = size.ToInt64();
            if (SizeLimitExceeded || count > sizeLimit - bytesWritten)
            {
                SizeLimitExceeded = true;
                return false;
            }

            if (!base.OnWrite(buffer, size)) return false;
            bytesWritten += count;
            return true;
        }
    }

    public static string ImageToBase64(SKBitmap bitmap, ImageFileFormat format)
    {
        using MemoryStream stream = new(); bitmap.Save(stream, format); return Convert.ToBase64String(stream.ToArray());
    }

    public static void FlipInPlace(this SKBitmap bitmap, bool horizontal, bool vertical)
    {
        using SKBitmap result = CreateBitmap(bitmap.Width, bitmap.Height);
        using (SKCanvas canvas = new(result))
        {
            canvas.Translate(horizontal ? bitmap.Width : 0, vertical ? bitmap.Height : 0);
            canvas.Scale(horizontal ? -1 : 1, vertical ? -1 : 1);
            canvas.DrawBitmap(bitmap, 0, 0);
        }
        bitmap.Reset();
        if (!bitmap.TryAllocPixels(result.Info)) throw new InvalidOperationException("Unable to allocate transformed image pixels.");
        using SKPixmap pixels = result.PeekPixels();
        if (!pixels.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes)) throw new InvalidOperationException("Unable to copy transformed image pixels.");
    }

    public static void RotateFlipInPlace(this SKBitmap bitmap, int rotateFlip)
    {
        int rotation = rotateFlip & 3;
        bool swap = rotation % 2 == 1;
        using SKBitmap result = CreateBitmap(swap ? bitmap.Height : bitmap.Width, swap ? bitmap.Width : bitmap.Height);
        using (SKCanvas canvas = new(result))
        {
            canvas.Translate(result.Width / 2f, result.Height / 2f);
            if ((rotateFlip & 4) != 0) canvas.Scale(-1, 1);
            canvas.RotateDegrees(rotation * 90);
            canvas.Translate(-bitmap.Width / 2f, -bitmap.Height / 2f);
            canvas.DrawBitmap(bitmap, 0, 0);
        }
        bitmap.Reset();
        if (!bitmap.TryAllocPixels(result.Info)) throw new InvalidOperationException("Unable to allocate transformed image pixels.");
        using SKPixmap pixels = result.PeekPixels();
        if (!pixels.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes)) throw new InvalidOperationException("Unable to copy transformed image pixels.");
    }
}
