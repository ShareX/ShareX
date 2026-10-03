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

#if WINDOWS10_0_17763_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace ShareX.Platform.Windows;

/// <summary>
/// Windows.Media.Ocr, the engine ShareX has always used on Windows. It is a Windows Runtime API, so it is only compiled into the
/// Windows SDK target of this project.
/// </summary>
[System.Runtime.Versioning.SupportedOSPlatform("windows10.0.18362.0")]
public sealed class WindowsOcrService : IOcrService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public IReadOnlyList<OcrLanguage> GetLanguages()
    {
        return OcrEngine.AvailableRecognizerLanguages.Select(language => new OcrLanguage(language.LanguageTag, language.DisplayName)).ToList();
    }

    public async Task<string> RecognizeAsync(byte[] png, string languageTag, bool singleLine, CancellationToken cancellationToken = default)
    {
        Language language = new Language(languageTag);

        if (!OcrEngine.IsLanguageSupported(language))
        {
            throw new InvalidOperationException($"The OCR language \"{language.DisplayName}\" is not installed. Add it in Settings > Time & language > Language.");
        }

        OcrEngine engine = OcrEngine.TryCreateFromLanguage(language);
        using InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
        await stream.WriteAsync(png.AsBuffer()).AsTask(cancellationToken).ConfigureAwait(false);
        stream.Seek(0);
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream).AsTask(cancellationToken).ConfigureAwait(false);
        using SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync().AsTask(cancellationToken).ConfigureAwait(false);
        OcrResult result = await engine.RecognizeAsync(softwareBitmap).AsTask(cancellationToken).ConfigureAwait(false);

        IEnumerable<string> lines;

        if (language.LanguageTag.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ||
            language.LanguageTag.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
        {
            // Chinese and Japanese are written without spaces between words.
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

#endif
