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
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using Bitmap = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX.Tools;

public static class OCRHelper
{
    private const string SupportedVersion = "10.0.18362.0";

    public static bool IsSupported => Helpers.OSVersion >= new Version(SupportedVersion);

    public static OCRLanguageOption[] AvailableLanguages
    {
        get
        {
            ThrowIfNotSupported();
            return OcrEngine.AvailableRecognizerLanguages
                .Select(x => new OCRLanguageOption(x.DisplayName, x.LanguageTag))
                .ToArray();
        }
    }

    public static void ThrowIfNotSupported()
    {
        if (!IsSupported)
        {
            throw new InvalidOperationException(
                string.Format(Localization.Strings.OCRHelper_Requires_Windows_version, SupportedVersion));
        }
    }

    public static async Task<string> OCR(Bitmap bitmap, string languageTag = "en", float scaleFactor = 1f,
        bool singleLine = false)
    {
        ThrowIfNotSupported();
        scaleFactor = Math.Max(scaleFactor, 1f);

        return await Task.Run(async () =>
        {
            float maxScale = GetMaxScaleFactor(bitmap);

            if (bitmap.Height >= SmallImageHeight)
            {
                return await OCRScaled(bitmap, Math.Min(scaleFactor, maxScale), languageTag, singleLine);
            }

            // Small text is read differently at different sizes, so read it at several sizes and keep the result
            // they agree on most. Each pass is fast because the image is small.
            List<string> results = [];

            IEnumerable<float> scales = SmallImageTargetHeights
                .Select(targetHeight => Math.Min(Math.Max(scaleFactor, (float)targetHeight / bitmap.Height), maxScale))
                .Distinct();

            foreach (float scale in scales)
            {
                string result = await OCRScaled(bitmap, scale, languageTag, singleLine);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    results.Add(result);
                }
            }

            return results
                .GroupBy(result => result)
                .OrderByDescending(group => group.Count())
                .ThenByDescending(group => group.Key.Length)
                .Select(group => group.Key)
                .FirstOrDefault() ?? string.Empty;
        });
    }

    private static async Task<string> OCRScaled(Bitmap bitmap, float scaleFactor, string languageTag, bool singleLine)
    {
        using Bitmap preparedBitmap = PrepareImage(bitmap, scaleFactor);
        return await OCRInternal(preparedBitmap, languageTag, singleLine);
    }

    // Images shorter than this are treated as small text (a button, a label, a single line).
    private const int SmallImageHeight = 64;
    private static readonly int[] SmallImageTargetHeights = [32, 40, 48, 64, 96];
    // Windows OCR misses text that touches the image edges, so a border of background color is added.
    private const int ImagePadding = 24;

    private static float GetMaxScaleFactor(Bitmap bitmap)
    {
        int maxDimension = (int)OcrEngine.MaxImageDimension - (ImagePadding * 2);
        float maxScale = (float)maxDimension / Math.Max(bitmap.Width, bitmap.Height);
        return Math.Max(maxScale, 1f);
    }

    private static Bitmap PrepareImage(Bitmap bitmap, float scaleFactor)
    {
        int width = Math.Max(1, (int)Math.Round(bitmap.Width * scaleFactor));
        int height = Math.Max(1, (int)Math.Round(bitmap.Height * scaleFactor));

        Bitmap result = SkiaImageHelpers.CreateBitmap(width + (ImagePadding * 2), height + (ImagePadding * 2));
        using SKCanvas canvas = new(result);
        using SKImage image = SKImage.FromBitmap(bitmap);
        canvas.Clear(GetBackgroundColor(bitmap));
        canvas.DrawImage(image, SKRect.Create(ImagePadding, ImagePadding, width, height), SkiaImageHelpers.HighQualitySampling);
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

    private static async Task<string> OCRInternal(Bitmap bitmap, string languageTag, bool singleLine)
    {
        Language language = new(languageTag);
        if (!OcrEngine.IsLanguageSupported(language))
        {
            throw new InvalidOperationException(string.Format(Localization.Strings.OCRHelper_Language_unavailable, language.DisplayName));
        }

        OcrEngine engine = OcrEngine.TryCreateFromLanguage(language);
        using InMemoryRandomAccessStream stream = new();
        // AsStream() is buffered, so flush it or small images never reach the underlying stream.
        Stream writeStream = stream.AsStream();
        bitmap.Save(writeStream, ImageFormat.Png);
        writeStream.Flush();
        stream.Seek(0);
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        using SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
        OcrResult result = await engine.RecognizeAsync(softwareBitmap);

        IEnumerable<string> lines;
        if (language.LanguageTag.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ||
            language.LanguageTag.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
        {
            lines = result.Lines.Select(line => string.Concat(line.Words.Select(word => word.Text)));
        }
        else if (language.LayoutDirection == LanguageLayoutDirection.Rtl)
        {
            lines = result.Lines.Select(line => string.Join(" ", line.Words.Reverse().Select(word => word.Text)));
        }
        else
        {
            lines = result.Lines.Select(line => line.Text);
        }

        return string.Join(singleLine ? " " : Environment.NewLine, lines);
    }
}
