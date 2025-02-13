#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace ShareX
{
    public static class OCRHelper
    {
        private const string SupportedVersion = "10.0.18362.0";

        public static bool IsSupported
        {
            get
            {
                return Helpers.OSVersion >= new Version(SupportedVersion);
            }
        }

        public static OCRLanguage[] AvailableLanguages
        {
            get
            {
                ThrowIfNotSupported();

                return OcrEngine.AvailableRecognizerLanguages.Select(x => new OCRLanguage(x.DisplayName, x.LanguageTag)).ToArray();
            }
        }

        public static void ThrowIfNotSupported()
        {
            if (!IsSupported)
            {
                throw new Exception(string.Format(Resources.OpticalCharacterRecognitionFeatureIsOnlyAvailableWithWindowsVersion0OrNewer, SupportedVersion));
            }
        }

        public static async Task<string> OCR(Bitmap bmp, string languageTag = "en", float scaleFactor = 1f, bool singleLine = false)
        {
            ThrowIfNotSupported();

            scaleFactor = Math.Max(scaleFactor, 1f);

            return await Task.Run(async () =>
            {
                using (Bitmap bmpScaled = ImageHelpers.ScaleImageFast(bmp, scaleFactor))
                {
                    return await OCRInternal(bmpScaled, languageTag, singleLine);
                }
            });
        }

        private static async Task<string> OCRInternal(Bitmap bmp, string languageTag, bool singleLine = false)
        {
            Language language = new Language(languageTag);

            if (!OcrEngine.IsLanguageSupported(language))
            {
                throw new Exception($"{language.DisplayName} language is not available in this system for OCR.");
            }

            OcrEngine engine = OcrEngine.TryCreateFromLanguage(language);

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                bmp.Save(stream.AsStream(), ImageFormat.Bmp);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                using (SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync())
                {
                    OcrResult ocrResult = await engine.RecognizeAsync(softwareBitmap);

                    string separator;

                    if (singleLine)
                    {
                        separator = " ";
                    }
                    else
                    {
                        separator = Environment.NewLine;
                    }

                    IEnumerable<string> lines;

                    if (language.LanguageTag.StartsWith("zh", StringComparison.OrdinalIgnoreCase) || // Chinese
                        language.LanguageTag.StartsWith("ja", StringComparison.OrdinalIgnoreCase)) // Japanese
                    {
                        // If CJK language then remove spaces between words.
                        lines = ocrResult.Lines.Select(line => string.Concat(line.Words.Select(word => word.Text)));
                    }
                    else if (language.LayoutDirection == LanguageLayoutDirection.Rtl)
                    {
                        // If RTL language then reverse order of words.
                        lines = ocrResult.Lines.Select(line => string.Join(" ", line.Words.Reverse().Select(word => word.Text)));
                    }
                    else
                    {
                        lines = ocrResult.Lines.Select(line => line.Text);
                    }

                    return string.Join(separator, lines);
                }
            }
        }
    }
}