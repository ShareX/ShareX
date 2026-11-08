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
using SkiaSharp;
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
        byte[] png;

        using (Bitmap preparedBitmap = PrepareImage(bitmap, scaleFactor))
        using (SKData data = preparedBitmap.Encode(ImageFormat.Png, 100))
        {
            png = data.ToArray();
        }

        return await Service.RecognizeAsync(png, languageTag, singleLine);
    }

    // Images shorter than this are treated as small text (a button, a label, a single line).
    private const int SmallImageHeight = 64;
    private static readonly int[] SmallImageTargetHeights = [32, 40, 48, 64, 96];
    // OCR engines miss text that touches the image edges, so a border of background color is added.
    private const int ImagePadding = 24;

    private static float GetMaxScaleFactor(Bitmap bitmap)
    {
        long maxDimension = (long)Service.MaxImageDimension - (ImagePadding * 2);
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
}
