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

using Avalonia;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Rendering;

/// <summary>
/// Helper methods for converting between Avalonia and SkiaSharp types.
/// </summary>
public static class SkiaSharpConversions
{
    public static SKPoint ToSKPoint(this Point point) => new((float)point.X, (float)point.Y);

    public static Point ToAvaloniaPoint(this SKPoint point) => new(point.X, point.Y);

    public static SKSize ToSKSize(this Size size) => new((float)size.Width, (float)size.Height);

    public static Size ToAvaloniaSize(this SKSize size) => new(size.Width, size.Height);

    public static SKRect ToSKRect(this Rect rect) => new((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);

    public static Rect ToAvaloniaRect(this SKRect rect) => new(rect.Left, rect.Top, rect.Width, rect.Height);

    public static SKBitmap? ToSKBitmap(this Bitmap? bitmap)
    {
        if (bitmap == null) return null;

        using var ms = new MemoryStream();
        bitmap.Save(ms, PngBitmapEncoderOptions.Default);
        ms.Position = 0;
        return SKBitmap.Decode(ms);
    }

    public static Bitmap? ToAvaloniaBitmap(this SKBitmap? skBitmap)
    {
        if (skBitmap == null) return null;

        using var image = SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);
        ms.Position = 0;
        return new Bitmap(ms);
    }
}
