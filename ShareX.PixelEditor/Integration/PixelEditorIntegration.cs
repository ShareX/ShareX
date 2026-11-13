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
using ShareX.PixelEditor.UI;
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.Integration;

/// <summary>Callbacks the host application (ShareX) provides to the Pixel editor.</summary>
public sealed class PixelEditorServices
{
    /// <summary>Where preferences, shortcuts and the workspace layout are stored.</summary>
    public string? SettingsFilePath { get; init; }

    /// <summary>Uploads an image using the host's configured uploader.</summary>
    public Action<SKBitmap>? UploadImage { get; init; }

    /// <summary>The folder the file dialogs start in.</summary>
    public string? DefaultFolder { get; init; }
}

/// <summary>Entry point used by ShareX to open the Pixel editor. A single window hosts all open sprites.</summary>
public static class PixelEditorIntegration
{
    private static PixelEditorWindow? _window;

    /// <summary>Opens the Pixel editor, optionally with files or an image. Safe to call from any thread.</summary>
    public static void Show(PixelEditorServices services, IEnumerable<string>? files = null, SKBitmap? image = null, string? imageTitle = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        List<string> paths = files?.Where(File.Exists).ToList() ?? [];
        SKBitmap? copy = image?.Copy();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new PixelEditorWindow(services);
                _window.Closed += (_, _) => _window = null;
                DesktopServices.PlaceOnMainWindowScreen(_window);
                _window.Show();
            }
            else
            {
                _window.BringToFront();
            }
            foreach (string path in paths) EditorCommands.OpenFile(_window, path);
            if (copy != null)
            {
                using (copy) EditorCommands.OpenBitmap(_window, copy, imageTitle ?? Localization.Strings.Sprite_Clipboard);
            }
        });
    }
}
