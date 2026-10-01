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

/// <summary>Image operations that own their pixels independently of the source file or stream.</summary>
public static partial class SkiaImageHelpers
{
    public static readonly SKSamplingOptions HighQualitySampling = new(SKCubicResampler.Mitchell);

    public static SKBitmap CreateBitmap(int width, int height)
    {
        SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        bitmap.Erase(SKColors.Transparent);
        return bitmap;
    }

    public static SKColor ToSKColor(this Color color) => new(color.R, color.G, color.B, color.A);

    public static Size GetSize(this SKBitmap bitmap) => new(bitmap.Width, bitmap.Height);

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

    public static void Save(SKBitmap bitmap, Stream stream, SKEncodedImageFormat format, int quality = 100)
    {
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(format, Math.Clamp(quality, 0, 100));
        if (data == null) throw new InvalidDataException($"Image encoding failed: {format}.");
        data.SaveTo(stream);
    }
}
