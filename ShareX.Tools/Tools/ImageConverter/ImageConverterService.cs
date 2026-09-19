#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ShareX.Tools;

public enum ImageConverterOutputFormat
{
    Png,
    Jpeg
}

public readonly record struct ImageConverterPreview(byte[] Data, int Width, int Height);

public static class ImageConverterService
{
    private const int MaxPreviewDimension = 1600;

    public static ImageConverterPreview CreatePreview(string filePath, ImageConverterOutputFormat format,
        int jpegQuality, Color backgroundColor)
    {
        using Bitmap? source = ImageHelpers.LoadImage(filePath);
        if (source == null)
        {
            return new ImageConverterPreview([], 0, 0);
        }

        Size previewSize = GetPreviewSize(source.Size);
        using Bitmap preview = CreatePreviewBitmap(source, previewSize);
        using MemoryStream stream = new();
        Save(preview, stream, format, jpegQuality, backgroundColor);
        return new ImageConverterPreview(stream.ToArray(), source.Width, source.Height);
    }

    public static void Save(Bitmap image, string filePath, ImageConverterOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        Save(image, stream, format, jpegQuality, backgroundColor);
    }

    private static void Save(Bitmap image, Stream stream, ImageConverterOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        if (format == ImageConverterOutputFormat.Jpeg)
        {
            using Bitmap flattened = ImageHelpers.FillBackground(image, backgroundColor);
            ImageHelpers.SaveJPEG(flattened, stream, jpegQuality);
        }
        else
        {
            image.Save(stream, ImageFormat.Png);
        }
    }

    private static Bitmap CreatePreviewBitmap(Bitmap source, Size size)
    {
        Bitmap output = new(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(output);
        graphics.Clear(Color.Transparent);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;

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
}
