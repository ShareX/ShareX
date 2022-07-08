#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System;
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
                // TODO: Translate
                throw new Exception(string.Format("Optical character recognition feature is only available with Windows version {0} or newer.",
                    SupportedVersion));
            }
        }

        public static async Task<string> OCR(Bitmap bmp, string languageTag = "en", float scaleFactor = 1f)
        {
            ThrowIfNotSupported();

            scaleFactor = Math.Max(scaleFactor, 1f);

            return await Task.Run(async () =>
            {
                using (Bitmap bmpClone = (Bitmap)bmp.Clone())
                using (Bitmap bmpScaled = ImageHelpers.ResizeImage(bmpClone, (int)(bmpClone.Width * scaleFactor), (int)(bmpClone.Height * scaleFactor)))
                {
                    return await OCRInternal(bmpScaled, languageTag);
                }
            });
        }

        private static async Task<string> OCRInternal(Bitmap bmp, string languageTag)
        {
            Language language = new Language(languageTag);

            if (!OcrEngine.IsLanguageSupported(language))
            {
                throw new Exception($"{language.LanguageTag} is not supported in this system.");
            }

            OcrEngine engine = OcrEngine.TryCreateFromLanguage(language);

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                bmp.Save(stream.AsStream(), ImageFormat.Bmp);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                using (SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync())
                {
                    OcrResult ocrResult = await engine.RecognizeAsync(softwareBitmap);

                    bool isCJK = languageTag.StartsWith("zh", StringComparison.OrdinalIgnoreCase) || // Chinese
                        languageTag.StartsWith("ja", StringComparison.OrdinalIgnoreCase) || // Japanese
                        languageTag.StartsWith("ko", StringComparison.OrdinalIgnoreCase); // Korean

                    if (isCJK)
                    {
                        return string.Join("\r\n", ocrResult.Lines.Select(line => string.Concat(line.Words.Select(word => word.Text))));
                    }
                    else
                    {
                        return string.Join("\r\n", ocrResult.Lines.Select(line => line.Text));
                    }
                }
            }
        }
    }
}