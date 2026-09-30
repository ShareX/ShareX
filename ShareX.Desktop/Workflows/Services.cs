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

using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop.Workflows;

public sealed record UploadOutcome(bool Success, string? Url, string? Error)
{
    public static UploadOutcome Failed(string error) => new UploadOutcome(false, null, error);
}

/// <summary>Sends a file to the destination the user configured.</summary>
public interface IUploadService
{
    /// <summary>False with a reason when no destination is configured, so the caller can tell the user what to do.</summary>
    bool IsConfigured(bool isImage, out string? reason);

    Task<UploadOutcome> UploadAsync(string fileName, byte[] data, bool isImage, CancellationToken cancellationToken);
}

/// <summary>Opens the image editor.</summary>
public interface IEditorLauncher
{
    Task OpenAsync(string? filePath, byte[]? png);
}
