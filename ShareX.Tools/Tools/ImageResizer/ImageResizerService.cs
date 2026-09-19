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

    public static Bitmap Resize(Bitmap source, int width, int height, ImageResizeMode mode)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);

        Bitmap output = new(width, height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(output);
        graphics.Clear(Color.Transparent);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;

        RectangleF sourceRectangle = new(0, 0, source.Width, source.Height);
        Rectangle destinationRectangle = new(0, 0, width, height);

        switch (mode)
        {
            case ImageResizeMode.Fill:
                sourceRectangle = GetFillSourceRectangle(source.Size, new Size(width, height));
                break;
            case ImageResizeMode.Fit:
                destinationRectangle = GetFitDestinationRectangle(source.Size, new Size(width, height));
                break;
        }

        using ImageAttributes attributes = new();
        attributes.SetWrapMode(WrapMode.TileFlipXY);
        graphics.DrawImage(source, destinationRectangle, sourceRectangle.X, sourceRectangle.Y,
            sourceRectangle.Width, sourceRectangle.Height, GraphicsUnit.Pixel, attributes);
        return output;
    }

    public static byte[] CreatePreview(string filePath, int width, int height, ImageResizeMode mode,
        ImageResizeOutputFormat format, int jpegQuality, Color backgroundColor)
    {
        using Bitmap? source = ImageHelpers.LoadImage(filePath);
        if (source == null)
        {
            return [];
        }

        Size previewSize = GetPreviewSize(width, height);
        using Bitmap output = Resize(source, previewSize.Width, previewSize.Height, mode);
        using MemoryStream stream = new();
        Save(output, stream, format, jpegQuality, backgroundColor);
        return stream.ToArray();
    }

    public static void Save(Bitmap image, string filePath, ImageResizeOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        Save(image, stream, format, jpegQuality, backgroundColor);
    }

    private static void Save(Bitmap image, Stream stream, ImageResizeOutputFormat format, int jpegQuality,
        Color backgroundColor)
    {
        if (format == ImageResizeOutputFormat.Jpeg)
        {
            using Bitmap flattened = ImageHelpers.FillBackground(image, backgroundColor);
            ImageHelpers.SaveJPEG(flattened, stream, jpegQuality);
        }
        else
        {
            image.Save(stream, ImageFormat.Png);
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
