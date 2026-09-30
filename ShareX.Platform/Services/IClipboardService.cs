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
}
