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
using System.Drawing.Drawing2D;
using System.IO;

namespace ShareX.HelpersLib;

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
        InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic)
    {
        if (width < 1 || height < 1 || source.Width == width && source.Height == height) return source;
        SKBitmap result = CreateBitmap(width, height);
        using (source)
        using (SKCanvas canvas = new(result))
        using (SKImage image = SKImage.FromBitmap(source))
        {
            SKSamplingOptions sampling = interpolationMode switch
            {
                InterpolationMode.NearestNeighbor => new(SKFilterMode.Nearest),
                InterpolationMode.Bilinear or InterpolationMode.HighQualityBilinear => new(SKFilterMode.Linear),
                _ => HighQualitySampling
            };
            canvas.DrawImage(image, new SKRect(0, 0, width, height), sampling);
        }
        return result;
    }

    public static SKBitmap ResizeImage(SKBitmap source, Size size,
        InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic)
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

    public static MemoryStream SaveJPEG(SKBitmap bitmap, int quality)
    {
        MemoryStream stream = new();
        Save(bitmap, stream, SKEncodedImageFormat.Jpeg, quality);
        return stream;
    }

    public static SKBitmap LoadImage(string filePath)
    {
        try
        {
            using FileStream stream = File.OpenRead(filePath);
            return Decode(stream);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            return null;
        }
    }

    public static SKBitmap Decode(Stream stream)
    {
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

        if (codec.EncodedOrigin == SKEncodedOrigin.TopLeft) return bitmap;

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
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(format, Math.Clamp(quality, 0, 100));
        if (data == null) throw new InvalidDataException($"Image encoding failed: {format}.");
        data.SaveTo(stream);
    }

    public static void Save(this SKBitmap bitmap, string filePath, SKEncodedImageFormat format, int quality = 100)
    {
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        bitmap.Save(stream, format, quality);
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
