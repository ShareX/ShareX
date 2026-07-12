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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib;

public sealed class ClipboardViewerPreview
{
    public string Text { get; }
    public byte[] ImageData { get; }
    public int ImageWidth { get; }
    public int ImageHeight { get; }

    public bool IsImage => ImageData != null;

    private ClipboardViewerPreview(string text, byte[] imageData, int imageWidth, int imageHeight)
    {
        Text = text;
        ImageData = imageData;
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
    }

    public static ClipboardViewerPreview FromText(string text) => new(text ?? string.Empty, null, 0, 0);

    public static ClipboardViewerPreview FromImage(Bitmap image)
    {
        using MemoryStream stream = new();
        image.Save(stream, ImageFormat.Png);
        return new ClipboardViewerPreview(string.Empty, stream.ToArray(), image.Width, image.Height);
    }
}

public sealed class ClipboardViewerData
{
    private readonly IDataObject _dataObject;

    public IReadOnlyList<string> Formats { get; }

    private ClipboardViewerData(IDataObject dataObject)
    {
        _dataObject = dataObject;
        Formats = dataObject?.GetFormats() ?? [];
    }

    public static ClipboardViewerData Capture()
    {
        return new ClipboardViewerData(Clipboard.GetDataObject());
    }

    public ClipboardViewerPreview GetPreview(string format)
    {
        object data = _dataObject?.GetData(format);
        if (data == null)
        {
            return ClipboardViewerPreview.FromText(string.Empty);
        }

        if (data is MemoryStream memoryStream)
        {
            byte[] bytes = memoryStream.ToArray();

            if (format.Equals(ClipboardHelpers.FORMAT_PNG, StringComparison.OrdinalIgnoreCase))
            {
                using MemoryStream imageStream = new(bytes, writable: false);
                using Bitmap source = new(imageStream);
                return ClipboardViewerPreview.FromImage(source);
            }

            if (format.Equals(DataFormats.Dib, StringComparison.OrdinalIgnoreCase))
            {
                using Bitmap image = ClipboardHelpersEx.ImageFromClipboardDib(bytes);
                return ClipboardViewerPreview.FromImage(image);
            }

            if (format.Equals(ClipboardHelpers.FORMAT_17, StringComparison.OrdinalIgnoreCase))
            {
                using Bitmap source = ClipboardHelpersEx.DIBV5ToBitmap(bytes);
                using Bitmap image = ClipboardHelpersEx.CloneImage(source);
                return ClipboardViewerPreview.FromImage(image);
            }
        }

        if (data is Bitmap bitmap)
        {
            return ClipboardViewerPreview.FromImage(bitmap);
        }

        return ClipboardViewerPreview.FromText(data.ToString());
    }
}
