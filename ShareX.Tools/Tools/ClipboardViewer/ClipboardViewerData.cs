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
using Bitmap = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX.Tools;

public sealed class ClipboardViewerPreview
{
    public string Text { get; }
    public byte[]? ImageData { get; }
    public int ImageWidth { get; }
    public int ImageHeight { get; }

    public bool IsImage => ImageData != null;

    private ClipboardViewerPreview(string text, byte[]? imageData, int imageWidth, int imageHeight)
    {
        Text = text;
        ImageData = imageData;
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
    }

    public static ClipboardViewerPreview FromText(string? text) => new(text ?? string.Empty, null, 0, 0);

    public static ClipboardViewerPreview FromImage(Bitmap image)
    {
        using MemoryStream stream = new();
        image.Save(stream, ImageFormat.Png);
        return new ClipboardViewerPreview(string.Empty, stream.ToArray(), image.Width, image.Height);
    }
}

public sealed class ClipboardViewerData
{
    private readonly ClipboardData? _dataObject;

    public IReadOnlyList<string> Formats { get; }

    private ClipboardViewerData(ClipboardData? dataObject)
    {
        _dataObject = dataObject;
        Formats = dataObject?.GetFormats() ?? [];
    }

    public static ClipboardViewerData Capture()
    {
        return new ClipboardViewerData(ClipboardHelpers.CaptureData());
    }

    public ClipboardViewerPreview GetPreview(string format)
    {
        object? data = _dataObject?.GetData(format);
        if (data == null)
        {
            return ClipboardViewerPreview.FromText(string.Empty);
        }

        if (data is byte[] bytes)
        {
            if ((format.Equals(ClipboardHelpers.FORMAT_PNG, StringComparison.OrdinalIgnoreCase) || format.Equals("image/png", StringComparison.OrdinalIgnoreCase) || format.Equals(ClipboardDataFormats.Bitmap, StringComparison.OrdinalIgnoreCase)))
            {
                using MemoryStream imageStream = new(bytes, writable: false);
                using Bitmap source = SkiaImageHelpers.Decode(imageStream);
                return ClipboardViewerPreview.FromImage(source);
            }

            if (format.Equals(ClipboardDataFormats.Dib, StringComparison.OrdinalIgnoreCase))
            {
                using Bitmap image = ClipboardHelpers.ConvertClipboardDibToBitmap(bytes);
                return ClipboardViewerPreview.FromImage(image);
            }

            if (format.Equals(ClipboardHelpers.FORMAT_17, StringComparison.OrdinalIgnoreCase))
            {
                using Bitmap source = ClipboardHelpers.ConvertClipboardDibV5ToBitmap(bytes);
                using Bitmap image = source.Copy();
                return ClipboardViewerPreview.FromImage(image);
            }
        }

        if (data is Bitmap bitmap)
        {
            return ClipboardViewerPreview.FromImage(bitmap);
        }

        return ClipboardViewerPreview.FromText(data is string[] files ? string.Join(Environment.NewLine, files) : data is byte[] raw ? Convert.ToHexString(raw) : data.ToString());
    }
}
