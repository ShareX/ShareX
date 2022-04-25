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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

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
                throw new Exception(string.Format("Optical character recognition feature is only supported with Windows version {0} or newer.",
                    SupportedVersion));
            }
        }

        public static async Task<string> OCR(string filePath, string languageTag = "en")
        {
            ThrowIfNotSupported();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await OCRInternal(fileStream, languageTag);
            }
        }

        public static async Task<string> OCR(Stream stream, string languageTag = "en")
        {
            ThrowIfNotSupported();

            return await OCRInternal(stream, languageTag);
        }

        private static async Task<string> OCRInternal(Stream stream, string languageTag)
        {
            Language language = new Language(languageTag);

            if (!OcrEngine.IsLanguageSupported(language))
            {
                throw new Exception($"{language.LanguageTag} is not supported in this system.");
            }

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());

            using (SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync())
            {
                OcrEngine engine = OcrEngine.TryCreateFromLanguage(language);
                OcrResult ocrResult = await engine.RecognizeAsync(bitmap).AsTask();

                return string.Join("\r\n", ocrResult.Lines.Select(x => x.Text));
            }
        }
    }
}