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

using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

public static class GdiSkiaBitmapConverter
{
    public static unsafe SKBitmap ToSKBitmap(Bitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        Bitmap source = bitmap;
        bool disposeSource = false;

        if (bitmap.PixelFormat is not PixelFormat.Format32bppArgb and not PixelFormat.Format32bppPArgb)
        {
            source = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);
            using Graphics graphics = Graphics.FromImage(source);
            graphics.DrawImageUnscaled(bitmap, 0, 0);
            disposeSource = true;
        }

        Rectangle bounds = new Rectangle(0, 0, source.Width, source.Height);
        BitmapData data = source.LockBits(bounds, ImageLockMode.ReadOnly, source.PixelFormat);

        try
        {
            SKAlphaType alphaType = source.PixelFormat == PixelFormat.Format32bppPArgb
                ? SKAlphaType.Premul
                : SKAlphaType.Unpremul;
            SKBitmap result = new SKBitmap(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, alphaType));

            byte* sourceBase = (byte*)data.Scan0;
            int sourceStride = data.Stride;
            if (sourceStride < 0)
            {
                sourceBase += sourceStride * (source.Height - 1);
                sourceStride = -sourceStride;
            }

            byte* destinationBase = (byte*)result.GetPixels();
            int rowBytes = source.Width * 4;

            for (int y = 0; y < source.Height; y++)
            {
                Buffer.MemoryCopy(
                    sourceBase + y * sourceStride,
                    destinationBase + y * result.RowBytes,
                    result.RowBytes,
                    rowBytes);
            }

            return result;
        }
        finally
        {
            source.UnlockBits(data);
            if (disposeSource)
            {
                source.Dispose();
            }
        }
    }

    public static unsafe Bitmap ToGdiBitmap(SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        SKBitmap source = bitmap;
        SKBitmap temporary = null;

        if (bitmap.ColorType != SKColorType.Bgra8888 || bitmap.AlphaType != SKAlphaType.Premul)
        {
            temporary = new SKBitmap(new SKImageInfo(
                bitmap.Width,
                bitmap.Height,
                SKColorType.Bgra8888,
                SKAlphaType.Premul));
            using SKCanvas canvas = new SKCanvas(temporary);
            canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(bitmap, 0, 0);
            source = temporary;
        }

        Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppPArgb);
        Rectangle bounds = new Rectangle(0, 0, result.Width, result.Height);
        BitmapData data = result.LockBits(bounds, ImageLockMode.WriteOnly, result.PixelFormat);

        try
        {
            byte* destinationBase = (byte*)data.Scan0;
            int destinationStride = data.Stride;
            if (destinationStride < 0)
            {
                destinationBase += destinationStride * (result.Height - 1);
                destinationStride = -destinationStride;
            }

            byte* sourceBase = (byte*)source.GetPixels();
            int rowBytes = source.Width * 4;

            for (int y = 0; y < source.Height; y++)
            {
                Buffer.MemoryCopy(
                    sourceBase + y * source.RowBytes,
                    destinationBase + y * destinationStride,
                    destinationStride,
                    rowBytes);
            }
        }
        finally
        {
            result.UnlockBits(data);
            temporary?.Dispose();
        }

        return result;
    }
}
