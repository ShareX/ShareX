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
using ShareX.Platform;
using Bitmap = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX.Tools;

/// <summary>OCR through <see cref="IOcrService"/>: Windows.Media.Ocr on Windows, Tesseract on Linux.</summary>
public static class OCRHelper
{
    private static IOcrService Service => PlatformServices.Current.Ocr;

    public static bool IsSupported => Service.Support.IsSupported;

    public static OCRLanguageOption[] AvailableLanguages
    {
        get
        {
            ThrowIfNotSupported();
            return Service.GetLanguages()
                .Select(x => new OCRLanguageOption(x.DisplayName, x.Tag))
                .ToArray();
        }
    }

    public static void ThrowIfNotSupported()
    {
        FeatureSupport support = Service.Support;

        if (!support.IsSupported)
        {
            throw new PlatformNotSupportedException(support.Reason);
        }
    }

    public static async Task<string> OCR(Bitmap bitmap, string languageTag = "en", float scaleFactor = 1f,
        bool singleLine = false)
    {
        ThrowIfNotSupported();
        scaleFactor = Math.Max(scaleFactor, 1f);

        // Small text is recognised better after enlarging it.
        byte[] png = await Task.Run(() =>
        {
            using Bitmap scaledBitmap = SkiaImageHelpers.ScaleImageFast(bitmap, scaleFactor);
            using SkiaSharp.SKData data = scaledBitmap.Encode(ImageFormat.Png, 100);
            return data.ToArray();
        });

        return await Service.RecognizeAsync(png, languageTag, singleLine);
    }
}
