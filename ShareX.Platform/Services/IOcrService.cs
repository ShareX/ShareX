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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <param name="Tag">The identifier to pass back to <see cref="IOcrService.RecognizeAsync"/>: a BCP 47 tag on Windows, a Tesseract
/// language code (eng, deu, chi_sim) on Linux.</param>
public sealed record OcrLanguage(string Tag, string DisplayName);

/// <summary>Text recognition: Windows.Media.Ocr on Windows, Tesseract on Linux.</summary>
public interface IOcrService
{
    FeatureSupport Support { get; }

    IReadOnlyList<OcrLanguage> GetLanguages();

    /// <summary>
    /// The text in <paramref name="png"/>, one line per recognised line, or joined with spaces when <paramref name="singleLine"/>.
    /// A tag the platform does not know is matched on its language (for example "en-US" or "en" picks "eng" on Linux).
    /// </summary>
    Task<string> RecognizeAsync(byte[] png, string languageTag, bool singleLine, CancellationToken cancellationToken = default);
}
