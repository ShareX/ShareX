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

public enum ImageResizeMode
{
    Fill,
    Fit,
    Stretch
}

public enum ImageResizeOutputFormat
{
    Png,
    Jpeg
}

public static class ImageResizerService
{
    private const int MaxPreviewDimension = 1600;

    public static SKBitmap Resize(SKBitmap source, int width, int height, ImageResizeMode mode)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        SKBitmap output = SkiaImageHelpers.CreateBitmap(width, height);
        using SKCanvas canvas = new(output);
        RectangleF sourceRectangle = new(0, 0, source.Width, source.Height);
        Rectangle destinationRectangle = new(0, 0, width, height);
        switch (mode)
        {
            case ImageResizeMode.Fill:
                sourceRectangle = GetFillSourceRectangle(source.GetSize(), new Size(width, height));
                break;
            case ImageResizeMode.Fit:
                destinationRectangle = GetFitDestinationRectangle(source.GetSize(), new Size(width, height));
                break;
        }
        canvas.DrawImage(source, new SKRect(sourceRectangle.Left, sourceRectangle.Top, sourceRectangle.Right, sourceRectangle.Bottom),
            new SKRect(destinationRectangle.Left, destinationRectangle.Top, destinationRectangle.Right, destinationRectangle.Bottom));
        return output;
    }

    public static byte[] CreatePreview(string filePath, int width, int height, ImageResizeMode mode,
        ImageResizeOutputFormat format, int jpegQuality, Color backgroundColor)
    {
        using SKBitmap? source = SkiaImageHelpers.LoadImage(filePath);
        if (source == null)
        {
            return [];
        }

        Size previewSize = GetPreviewSize(width, height);
        using SKBitmap output = Resize(source, previewSize.Width, previewSize.Height, mode);
        using MemoryStream stream = new();
        Save(output, stream, format, jpegQuality, backgroundColor);
        return stream.ToArray();
    }

    public static void Save(SKBitmap image, string filePath, ImageResizeOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        Save(image, stream, format, jpegQuality, backgroundColor);
    }

    private static void Save(SKBitmap image, Stream stream, ImageResizeOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        if (format == ImageResizeOutputFormat.Jpeg)
        {
            using SKBitmap flattened = SkiaImageHelpers.FillBackground(image, backgroundColor);
            SkiaImageHelpers.Save(flattened, stream, SKEncodedImageFormat.Jpeg, jpegQuality);
        }
        else
        {
            SkiaImageHelpers.Save(image, stream, SKEncodedImageFormat.Png);
        }
    }

    private static RectangleF GetFillSourceRectangle(Size source, Size destination)
    {
        double sourceAspect = source.Width / (double)source.Height;
        double destinationAspect = destination.Width / (double)destination.Height;

        if (sourceAspect > destinationAspect)
        {
            float width = (float)(source.Height * destinationAspect);
            return new RectangleF((source.Width - width) / 2f, 0, width, source.Height);
        }

        float height = (float)(source.Width / destinationAspect);
        return new RectangleF(0, (source.Height - height) / 2f, source.Width, height);
    }

    private static Rectangle GetFitDestinationRectangle(Size source, Size destination)
    {
        double scale = Math.Min(destination.Width / (double)source.Width,
            destination.Height / (double)source.Height);
        int width = Math.Max(1, (int)Math.Round(source.Width * scale));
        int height = Math.Max(1, (int)Math.Round(source.Height * scale));
        return new Rectangle((destination.Width - width) / 2,
            (destination.Height - height) / 2, width, height);
    }

    private static Size GetPreviewSize(int width, int height)
    {
        int largestDimension = Math.Max(width, height);
        if (largestDimension <= MaxPreviewDimension)
        {
            return new Size(width, height);
        }

        double scale = MaxPreviewDimension / (double)largestDimension;
        return new Size(Math.Max(1, (int)Math.Round(width * scale)),
            Math.Max(1, (int)Math.Round(height * scale)));
    }
}
