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
using System.Drawing;

namespace ShareX.Tools;

public enum ImageWatermarkType
{
    Text,
    Image
}

public enum ImageWatermarkPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

public enum ImageWatermarkOutputFormat
{
    Png,
    Jpeg
}

public sealed record ImageWatermarkOptions(
    ImageWatermarkType Type,
    string Text,
    ImageWatermarkPosition Position,
    int Margin,
    int Opacity,
    float TextSize,
    Color TextColor,
    int ImageScale,
    float Rotation);

public readonly record struct ImageWatermarkPreview(byte[] Data, int Width, int Height);

public static class ImageWatermarkService
{
    private const int MaxPreviewDimension = 1600;

    public static ImageWatermarkPreview CreatePreview(string filePath, string watermarkImagePath,
        ImageWatermarkOptions options, ImageWatermarkOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        using SKBitmap? source = SkiaImageHelpers.LoadImage(filePath);
        if (source == null)
        {
            return new ImageWatermarkPreview([], 0, 0);
        }

        Size previewSize = GetPreviewSize(source.GetSize());
        double previewScale = previewSize.Width / (double)source.Width;
        using SKBitmap previewSource = Resize(source, previewSize);
        using SKBitmap? watermarkImage = options.Type == ImageWatermarkType.Image
            ? SkiaImageHelpers.LoadImage(watermarkImagePath)
            : null;
        ImageWatermarkOptions previewOptions = options with
        {
            Margin = Math.Max(0, (int)Math.Round(options.Margin * previewScale)),
            TextSize = Math.Max(1, (float)(options.TextSize * previewScale))
        };
        using SKBitmap output = Apply(previewSource, watermarkImage, previewOptions);
        using MemoryStream stream = new();
        Save(output, stream, format, jpegQuality, backgroundColor);
        return new ImageWatermarkPreview(stream.ToArray(), source.Width, source.Height);
    }

    public static SKBitmap Apply(SKBitmap source, SKBitmap? watermarkImage, ImageWatermarkOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        SKBitmap output = source.Copy();
        using SKCanvas canvas = new(output);
        using SKBitmap? watermark = options.Type == ImageWatermarkType.Text
            ? CreateTextWatermark(options)
            : CreateImageWatermark(watermarkImage, source.GetSize(), options);
        if (watermark == null) return output;
        using SKBitmap rotatedWatermark = Rotate(watermark, options.Rotation);
        Point location = GetLocation(source.GetSize(), rotatedWatermark.GetSize(), options.Position, options.Margin);
        canvas.DrawBitmap(rotatedWatermark, location.X, location.Y);
        return output;
    }

    public static void Save(SKBitmap image, string filePath, ImageWatermarkOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        Save(image, stream, format, jpegQuality, backgroundColor);
    }

    private static void Save(SKBitmap image, Stream stream, ImageWatermarkOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        if (format == ImageWatermarkOutputFormat.Jpeg)
        {
            using SKBitmap flattened = SkiaImageHelpers.FillBackground(image, backgroundColor);
            SkiaImageHelpers.Save(flattened, stream, SKEncodedImageFormat.Jpeg, jpegQuality);
        }
        else
        {
            SkiaImageHelpers.Save(image, stream, SKEncodedImageFormat.Png);
        }
    }

    private static SKBitmap? CreateTextWatermark(ImageWatermarkOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Text)) return null;
        using SKFont font = new(SKTypeface.Default, Math.Max(1, options.TextSize));
        string[] lines = options.Text.Replace("\r", "").Split('\n');
        float width = lines.Max(line => font.MeasureText(line));
        SKFontMetrics metrics = font.Metrics;
        float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;
        SKBitmap watermark = SkiaImageHelpers.CreateBitmap(Math.Max(1, (int)Math.Ceiling(width) + 4),
            Math.Max(1, (int)Math.Ceiling(lineHeight * lines.Length) + 4));
        using SKCanvas canvas = new(watermark);
        byte alpha = (byte)Math.Round(Math.Clamp(options.Opacity, 0, 100) / 100d * 255);
        using SKPaint paint = new() { Color = options.TextColor.ToSKColor().WithAlpha(alpha), IsAntialias = true };
        for (int index = 0; index < lines.Length; index++)
            canvas.DrawText(lines[index], 2, 2 - metrics.Ascent + index * lineHeight, font, paint);
        return watermark;
    }

    private static SKBitmap? CreateImageWatermark(SKBitmap? watermarkImage, Size canvasSize,
        ImageWatermarkOptions options)
    {
        if (watermarkImage == null || watermarkImage.Width < 1 || watermarkImage.Height < 1) return null;
        double percentage = Math.Clamp(options.ImageScale, 1, 100) / 100d;
        double scale = Math.Min(canvasSize.Width * percentage / watermarkImage.Width,
            canvasSize.Height * percentage / watermarkImage.Height);
        int width = Math.Max(1, (int)Math.Round(watermarkImage.Width * scale));
        int height = Math.Max(1, (int)Math.Round(watermarkImage.Height * scale));
        SKBitmap watermark = SkiaImageHelpers.CreateBitmap(width, height);
        using SKCanvas canvas = new(watermark);
        using SKPaint paint = new() { Color = SKColors.White.WithAlpha((byte)Math.Round(Math.Clamp(options.Opacity, 0, 100) * 2.55)) };
        canvas.DrawImage(watermarkImage, new SKRect(0, 0, watermarkImage.Width, watermarkImage.Height),
            new SKRect(0, 0, width, height), paint);
        return watermark;
    }

    private static SKBitmap Rotate(SKBitmap source, float angle)
    {
        angle %= 360;
        if (Math.Abs(angle) < 0.01f) return source.Copy();
        double radians = angle * Math.PI / 180d;
        double sin = Math.Abs(Math.Sin(radians));
        double cos = Math.Abs(Math.Cos(radians));
        int width = Math.Max(1, (int)Math.Ceiling(source.Width * cos + source.Height * sin));
        int height = Math.Max(1, (int)Math.Ceiling(source.Width * sin + source.Height * cos));
        SKBitmap output = SkiaImageHelpers.CreateBitmap(width, height);
        using SKCanvas canvas = new(output);
        canvas.Translate(width / 2f, height / 2f);
        canvas.RotateDegrees(angle);
        canvas.Translate(-source.Width / 2f, -source.Height / 2f);
        canvas.DrawImage(source, new SKRect(0, 0, source.Width, source.Height),
            new SKRect(0, 0, source.Width, source.Height));
        return output;
    }

    private static Point GetLocation(Size canvas, Size watermark, ImageWatermarkPosition position, int margin)
    {
        int left = margin;
        int centerX = (canvas.Width - watermark.Width) / 2;
        int right = canvas.Width - watermark.Width - margin;
        int top = margin;
        int centerY = (canvas.Height - watermark.Height) / 2;
        int bottom = canvas.Height - watermark.Height - margin;

        return position switch
        {
            ImageWatermarkPosition.TopLeft => new Point(left, top),
            ImageWatermarkPosition.TopCenter => new Point(centerX, top),
            ImageWatermarkPosition.TopRight => new Point(right, top),
            ImageWatermarkPosition.MiddleLeft => new Point(left, centerY),
            ImageWatermarkPosition.Center => new Point(centerX, centerY),
            ImageWatermarkPosition.MiddleRight => new Point(right, centerY),
            ImageWatermarkPosition.BottomLeft => new Point(left, bottom),
            ImageWatermarkPosition.BottomCenter => new Point(centerX, bottom),
            _ => new Point(right, bottom)
        };
    }

    private static SKBitmap Resize(SKBitmap source, Size size) => SkiaImageHelpers.Resize(source, size.Width, size.Height);

    private static Size GetPreviewSize(Size source)
    {
        int largestDimension = Math.Max(source.Width, source.Height);
        if (largestDimension <= MaxPreviewDimension)
        {
            return source;
        }

        double scale = MaxPreviewDimension / (double)largestDimension;
        return new Size(Math.Max(1, (int)Math.Round(source.Width * scale)),
            Math.Max(1, (int)Math.Round(source.Height * scale)));
    }


}
