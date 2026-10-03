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

/// <summary>
/// The platform's own names for clipboard formats ShareX writes alongside an image, for code that puts data on the clipboard
/// through a UI toolkit (Avalonia) rather than through <see cref="IClipboardService"/>.
/// </summary>
/// <param name="Png">PNG image data: "PNG" on Windows, "image/png" on Linux, "public.png" on macOS.</param>
/// <param name="Dib">A device-independent bitmap for older Windows applications, or null where there is no such format.</param>
/// <param name="Html">HTML: "HTML Format" on Windows, "text/html" on Linux, "public.html" on macOS. Encode the content with
/// <see cref="IClipboardService.EncodeHtml"/>.</param>
public sealed record ClipboardFormatNames(string Png, string? Dib, string Html);

/// <summary>System clipboard: Win32, NSPasteboard, X11 selections or Wayland data control.</summary>
/// <remarks>Images are exchanged as PNG bytes so the interface does not depend on System.Drawing, Avalonia or SkiaSharp.</remarks>
public interface IClipboardService
{
    FeatureSupport Support { get; }

    Task<bool> SetTextAsync(string text, CancellationToken cancellationToken = default);

    Task<string?> GetTextAsync(CancellationToken cancellationToken = default);

    Task<bool> SetImageAsync(byte[] png, CancellationToken cancellationToken = default);

    /// <summary>Returns the clipboard image encoded as PNG, or null when the clipboard does not hold an image.</summary>
    Task<byte[]?> GetImageAsync(CancellationToken cancellationToken = default);

    Task<bool> SetFilesAsync(IReadOnlyList<string> paths, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default);

    Task<bool> ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Every format on the clipboard, named the way the platform names them: Windows clipboard format names ("UnicodeText",
    /// "DeviceIndependentBitmap", "PNG"), MIME types on Linux, uniform type identifiers on macOS. For the clipboard viewer.
    /// </summary>
    Task<IReadOnlyList<string>> GetFormatsAsync(CancellationToken cancellationToken = default);

    /// <summary>The raw bytes of one format from <see cref="GetFormatsAsync"/>, or null when it is gone or not a data format.</summary>
    Task<byte[]?> GetDataAsync(string format, CancellationToken cancellationToken = default);

    ClipboardFormatNames FormatNames { get; }

    /// <summary>
    /// Wraps an HTML fragment the way the platform's HTML clipboard format expects: the CF_HTML header with byte offsets on
    /// Windows, unchanged elsewhere.
    /// </summary>
    string EncodeHtml(string htmlFragment);
}
