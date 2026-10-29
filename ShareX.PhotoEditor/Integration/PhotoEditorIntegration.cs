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

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.PhotoEditor.UI;
using ShareX.PhotoEditor.UI.Commands;

namespace ShareX.PhotoEditor.Integration;

/// <summary>Entry point used by ShareX to open the Photo editor. A single window hosts all open documents.</summary>
public static class PhotoEditorIntegration
{
    private static PhotoEditorWindow? _window;

    /// <summary>Opens the Photo editor, optionally with files or an image. Safe to call from any thread.</summary>
    public static void Show(PhotoEditorServices services, IEnumerable<string>? files = null, SKBitmap? image = null, string? imageTitle = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        List<string> paths = files?.Where(File.Exists).ToList() ?? [];
        SKBitmap? copy = image?.Copy();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new PhotoEditorWindow(services);
                _window.Closed += (_, _) => _window = null;
                _window.Show();
            }
            else
            {
                if (!_window.IsVisible) _window.Show();
                _window.Activate();
            }
            foreach (string path in paths) EditorCommands.OpenFile(_window, path);
            if (copy != null)
            {
                using (copy) EditorCommands.OpenBitmap(_window, copy, imageTitle ?? "Screenshot", imageTitle != null && File.Exists(imageTitle) ? imageTitle : null);
            }
        });
    }
}
