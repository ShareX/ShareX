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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

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
        using Bitmap? source = ImageHelpers.LoadImage(filePath);
        if (source == null)
        {
            return new ImageWatermarkPreview([], 0, 0);
        }

        Size previewSize = GetPreviewSize(source.Size);
        double previewScale = previewSize.Width / (double)source.Width;
        using Bitmap previewSource = Resize(source, previewSize);
        using Bitmap? watermarkImage = options.Type == ImageWatermarkType.Image
            ? ImageHelpers.LoadImage(watermarkImagePath)
            : null;
        ImageWatermarkOptions previewOptions = options with
        {
            Margin = Math.Max(0, (int)Math.Round(options.Margin * previewScale)),
            TextSize = Math.Max(1, (float)(options.TextSize * previewScale))
        };
        using Bitmap output = Apply(previewSource, watermarkImage, previewOptions);
        using MemoryStream stream = new();
        Save(output, stream, format, jpegQuality, backgroundColor);
        return new ImageWatermarkPreview(stream.ToArray(), source.Width, source.Height);
    }

    public static Bitmap Apply(Bitmap source, Bitmap? watermarkImage, ImageWatermarkOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);

        Bitmap output = new(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(output);
        ConfigureGraphics(graphics);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height));
        graphics.CompositingMode = CompositingMode.SourceOver;

        using Bitmap? watermark = options.Type == ImageWatermarkType.Text
            ? CreateTextWatermark(options)
            : CreateImageWatermark(watermarkImage, source.Size, options);
        if (watermark == null)
        {
            return output;
        }

        using Bitmap rotatedWatermark = Rotate(watermark, options.Rotation);
        Point location = GetLocation(source.Size, rotatedWatermark.Size, options.Position, options.Margin);
        graphics.DrawImageUnscaled(rotatedWatermark, location);
        return output;
    }

    public static void Save(Bitmap image, string filePath, ImageWatermarkOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        Save(image, stream, format, jpegQuality, backgroundColor);
    }

    private static void Save(Bitmap image, Stream stream, ImageWatermarkOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        if (format == ImageWatermarkOutputFormat.Jpeg)
        {
            using Bitmap flattened = ImageHelpers.FillBackground(image, backgroundColor);
            ImageHelpers.SaveJPEG(flattened, stream, jpegQuality);
        }
        else
        {
            image.Save(stream, ImageFormat.Png);
        }
    }

    private static Bitmap? CreateTextWatermark(ImageWatermarkOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Text))
        {
            return null;
        }

        using Font font = new(SystemFonts.DefaultFont.FontFamily, Math.Max(1, options.TextSize),
            FontStyle.Regular, GraphicsUnit.Pixel);
        using Bitmap measureBitmap = new(1, 1, PixelFormat.Format32bppArgb);
        using Graphics measureGraphics = Graphics.FromImage(measureBitmap);
        measureGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        SizeF measured = measureGraphics.MeasureString(options.Text, font, int.MaxValue,
            StringFormat.GenericTypographic);
        int width = Math.Max(1, (int)Math.Ceiling(measured.Width) + 4);
        int height = Math.Max(1, (int)Math.Ceiling(measured.Height) + 4);

        Bitmap watermark = new(width, height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(watermark);
        ConfigureGraphics(graphics);
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        int alpha = (int)Math.Round(Math.Clamp(options.Opacity, 0, 100) / 100d * 255);
        using SolidBrush brush = new(Color.FromArgb(alpha, options.TextColor));
        graphics.DrawString(options.Text, font, brush, new PointF(2, 2), StringFormat.GenericTypographic);
        return watermark;
    }

    private static Bitmap? CreateImageWatermark(Bitmap? watermarkImage, Size canvasSize,
        ImageWatermarkOptions options)
    {
        if (watermarkImage == null || watermarkImage.Width < 1 || watermarkImage.Height < 1)
        {
            return null;
        }

        double percentage = Math.Clamp(options.ImageScale, 1, 100) / 100d;
        double scale = Math.Min(canvasSize.Width * percentage / watermarkImage.Width,
            canvasSize.Height * percentage / watermarkImage.Height);
        int width = Math.Max(1, (int)Math.Round(watermarkImage.Width * scale));
        int height = Math.Max(1, (int)Math.Round(watermarkImage.Height * scale));

        Bitmap watermark = new(width, height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(watermark);
        ConfigureGraphics(graphics);
        using ImageAttributes attributes = new();
        float opacity = Math.Clamp(options.Opacity, 0, 100) / 100f;
        attributes.SetColorMatrix(new ColorMatrix { Matrix33 = opacity });
        graphics.DrawImage(watermarkImage, new Rectangle(0, 0, width, height),
            0, 0, watermarkImage.Width, watermarkImage.Height, GraphicsUnit.Pixel, attributes);
        return watermark;
    }

    private static Bitmap Rotate(Bitmap source, float angle)
    {
        angle %= 360;
        if (Math.Abs(angle) < 0.01f)
        {
            return new Bitmap(source);
        }

        double radians = angle * Math.PI / 180d;
        double sin = Math.Abs(Math.Sin(radians));
        double cos = Math.Abs(Math.Cos(radians));
        int width = Math.Max(1, (int)Math.Ceiling(source.Width * cos + source.Height * sin));
        int height = Math.Max(1, (int)Math.Ceiling(source.Width * sin + source.Height * cos));
        Bitmap output = new(width, height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(output);
        ConfigureGraphics(graphics);
        graphics.TranslateTransform(width / 2f, height / 2f);
        graphics.RotateTransform(angle);
        graphics.TranslateTransform(-source.Width / 2f, -source.Height / 2f);
        graphics.DrawImageUnscaled(source, 0, 0);
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

    private static Bitmap Resize(Bitmap source, Size size)
    {
        Bitmap output = new(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(output);
        ConfigureGraphics(graphics);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        using ImageAttributes attributes = new();
        attributes.SetWrapMode(WrapMode.TileFlipXY);
        graphics.DrawImage(source, new Rectangle(0, 0, size.Width, size.Height),
            0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
        return output;
    }

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

    private static void ConfigureGraphics(Graphics graphics)
    {
        graphics.Clear(Color.Transparent);
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
    }
}
