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

namespace ShareX.PhotoEditor.Integration;

/// <summary>Callbacks the host application (ShareX) provides to the Photo editor.</summary>
public sealed class PhotoEditorServices
{
    /// <summary>Where preferences, shortcuts and the workspace layout are stored.</summary>
    public string? SettingsFilePath { get; init; }

    /// <summary>Uploads an image using the host's configured uploader.</summary>
    public Action<SKBitmap>? UploadImage { get; init; }

    /// <summary>Copies an image to the clipboard. Defaults to the ShareX clipboard helper.</summary>
    public Action<SKBitmap>? CopyImage { get; init; }

    public Action<SKBitmap>? PinImage { get; init; }

    public Action<SKBitmap>? PrintImage { get; init; }

    /// <summary>Runs the host's after-capture tasks (save, upload, copy, ...) for the image.</summary>
    public Action<SKBitmap, string?>? RunImageTask { get; init; }

    /// <summary>Lets the user capture a screen region; returns null when cancelled.</summary>
    public Func<Task<SKBitmap?>>? CaptureRegion { get; init; }

    /// <summary>Captures the full screen.</summary>
    public Func<Task<SKBitmap?>>? CaptureFullscreen { get; init; }

    /// <summary>Opens the host's screenshots folder for the file dialogs.</summary>
    public string? DefaultFolder { get; init; }
}
