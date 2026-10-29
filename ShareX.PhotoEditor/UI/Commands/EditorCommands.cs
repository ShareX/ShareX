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

using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI.Commands;

/// <summary>All menu and keyboard commands. Split into partial files by menu.</summary>
public static partial class EditorCommands
{
    private static readonly Func<PhotoEditorWindow, bool> HasDoc = w => w.Document != null;
    private static readonly Func<PhotoEditorWindow, bool> HasSelection = w => w.Document?.Selection != null;
    private static readonly Func<PhotoEditorWindow, bool> HasLayer = w => w.Document?.ActiveLayer != null;

    private static void R(string id, string text, string? gesture, Action<PhotoEditorWindow> execute, Func<PhotoEditorWindow, bool>? canExecute = null, Func<PhotoEditorWindow, bool>? isChecked = null) =>
        CommandRegistry.Register(new EditorCommand(id, text, gesture, execute, canExecute, isChecked));

    internal static void RegisterAll()
    {
        RegisterFile();
        RegisterEdit();
        RegisterImage();
        RegisterLayer();
        RegisterType();
        RegisterSelect();
        RegisterFilter();
        RegisterView();
        RegisterWindow();
    }

    private static PhotoEditorWindow W(IEditorHost host) => (PhotoEditorWindow)host;

    /// <summary>Returns the active raster layer or explains why the command cannot run.</summary>
    private static RasterLayer? EditableRaster(PhotoEditorWindow w, string action)
    {
        PhotoDocument? doc = w.Document;
        if (doc?.ActiveLayer is RasterLayer raster && raster.CanEditPixels) return raster;
        if (doc?.ActiveLayer is RasterLayer) w.ShowMessage($"Could not {action} because the layer is locked.");
        else if (doc?.ActiveLayer != null) w.ShowMessage($"Could not {action} because the active layer is not a pixel layer. Rasterize it first (Layer > Rasterize).");
        else w.ShowMessage($"Could not {action} because there is no active layer.");
        return null;
    }

    private static IReadOnlyList<MenuNode>? _mainMenu;

    // Built lazily: static fields of the other partial files are not initialized in a defined order.
    public static IReadOnlyList<MenuNode> MainMenu => _mainMenu ??= BuildMenu();
}
