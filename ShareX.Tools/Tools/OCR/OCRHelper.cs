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
using ShareX.OCR;
using ShareX.Platform;
using SkiaSharp;
using Bitmap = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX.Tools;

public sealed record OCRLanguageOption(string DisplayName, string LanguageTag);

/// <summary>
/// Reads text with ShareX's own PaddleOCR engine. Where its ONNX runtime has no build (Intel Macs), the system's engine is used
/// instead (Vision); it returns the text without the boxes, so the OCR window has nothing to outline there.
/// </summary>
public static class OCRHelper
{
    // OCR engines miss text that touches the image edges, so the system engine gets a border of background color.
    private const int ImagePadding = 24;

    private static IOcrService SystemService => PlatformServices.Current.Ocr;

    public static FeatureSupport Support => PaddleOCREngine.IsAvailable || !PlatformServices.IsInitialized
        ? FeatureSupport.Supported
        : SystemService.Support;

    public static bool IsSupported => Support.IsSupported;

    public static Task<OCRResult> Recognize(Bitmap bitmap)
    {
        if (PaddleOCREngine.IsAvailable)
        {
            return Task.Run(() => PaddleOCREngine.Shared.Recognize(bitmap));
        }

        return RecognizeWithSystemEngine(bitmap);
    }

    public static async Task<string> OCR(Bitmap bitmap, bool singleLine = false)
    {
        OCRResult result = await Recognize(bitmap);
        return result.GetText(singleLine);
    }

    private static async Task<OCRResult> RecognizeWithSystemEngine(Bitmap bitmap)
    {
        FeatureSupport support = SystemService.Support;

        if (!support.IsSupported)
        {
            throw new PlatformNotSupportedException(support.Reason);
        }

        OCRLanguageOption? language = SelectLanguage(
            SystemService.GetLanguages().Select(x => new OCRLanguageOption(x.DisplayName, x.Tag)).ToArray(),
            null, System.Globalization.CultureInfo.CurrentUICulture.Name);

        byte[] png = await Task.Run(() =>
        {
            using Bitmap prepared = AddPadding(bitmap);
            using SKData data = prepared.Encode(ImageFormat.Png, 100);
            return data.ToArray();
        });

        string text = await SystemService.RecognizeAsync(png, language?.LanguageTag ?? "en", singleLine: false);
        return OCRResult.FromLines(text.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    /// <summary>
    /// The saved language when the engine has it; otherwise the system's language, then any variant of the saved one, then the
    /// first. Engines name languages differently ("en-US" in Vision), so an inexact tag must not fall through to whatever sorts
    /// first (German on a French Mac).
    /// </summary>
    internal static OCRLanguageOption? SelectLanguage(IReadOnlyList<OCRLanguageOption> languages, string? saved, string? systemCulture)
    {
        static string BaseOf(string? tag) => (tag ?? string.Empty).Split('-', '_')[0];

        return languages.FirstOrDefault(x => x.LanguageTag.Equals(saved, StringComparison.OrdinalIgnoreCase))
            ?? languages.FirstOrDefault(x => x.LanguageTag.Equals(systemCulture, StringComparison.OrdinalIgnoreCase))
            ?? (string.IsNullOrEmpty(BaseOf(systemCulture)) ? null
                : languages.FirstOrDefault(x => BaseOf(x.LanguageTag).Equals(BaseOf(systemCulture), StringComparison.OrdinalIgnoreCase)))
            ?? (string.IsNullOrEmpty(BaseOf(saved)) ? null
                : languages.FirstOrDefault(x => BaseOf(x.LanguageTag).Equals(BaseOf(saved), StringComparison.OrdinalIgnoreCase)))
            ?? languages.FirstOrDefault();
    }

    private static Bitmap AddPadding(Bitmap bitmap)
    {
        Bitmap result = SkiaImageHelpers.CreateBitmap(bitmap.Width + (ImagePadding * 2), bitmap.Height + (ImagePadding * 2));
        using SKCanvas canvas = new(result);
        canvas.Clear(GetBackgroundColor(bitmap));
        canvas.DrawBitmap(bitmap, ImagePadding, ImagePadding);
        return result;
    }

    // The most common color along the image edges, which for text in a box is its background.
    private static SKColor GetBackgroundColor(Bitmap bitmap)
    {
        Dictionary<SKColor, int> counts = [];

        void Count(int x, int y)
        {
            SKColor color = bitmap.GetPixel(x, y).WithAlpha(255);
            counts[color] = counts.GetValueOrDefault(color) + 1;
        }

        for (int x = 0; x < bitmap.Width; x++)
        {
            Count(x, 0);
            Count(x, bitmap.Height - 1);
        }

        for (int y = 1; y < bitmap.Height - 1; y++)
        {
            Count(0, y);
            Count(bitmap.Width - 1, y);
        }

        return counts.MaxBy(pair => pair.Value).Key;
    }
}
