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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ShareX.PhotoEditor.Integration;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI;

public sealed partial class PhotoEditorWindow
{
    private static bool IsTextInput(object? source)
    {
        for (StyledElement? e = source as StyledElement; e != null; e = e.Parent)
        {
            if (e is TextBox || e is NumericUpDown || e is ComboBox { IsDropDownOpen: true }) return true;
        }
        return false;
    }

    private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
    {
        LastModifiers = e.KeyModifiers;
        if (IsTextEditing)
        {
            if (e.Key == Key.Escape) { EndTextEditing(false); e.Handled = true; }
            else if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Control)) { EndTextEditing(true); e.Handled = true; }
            return;
        }
        if (IsTextInput(e.Source)) return;
        if (IsBusy)
        {
            e.Handled = true;
            return;
        }
        // While a stroke or drag is in progress only modifier keys, Escape and Enter reach the tool; commands wait for the release.
        if (Canvas.IsToolPointerDown && e.Key is not (Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or Key.Space or Key.Escape or Key.Enter))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Space && !Canvas.SpaceHeld)
        {
            Canvas.SpaceHeld = true;
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Tab && e.KeyModifiers == KeyModifiers.None)
        {
            TogglePanels();
            e.Handled = true;
            return;
        }
        // Alt temporarily switches painting tools to the Eyedropper, like the reference application.
        if ((e.Key == Key.LeftAlt || e.Key == Key.RightAlt) && _springReturn == null && Tools.Active is BrushTool or PencilTool or GradientTool or PaintBucketTool or ColorReplacementTool)
        {
            _springReturn = Tools.Active;
            Tools.Select(ToolId.Eyedropper);
            e.Handled = true;
            return;
        }

        if (Tools.Active.KeyDown(e.Key, e.KeyModifiers))
        {
            e.Handled = true;
            return;
        }
        EditorCommand? command = CommandRegistry.Match(e.Key, e.KeyModifiers, Settings);
        if (command != null)
        {
            Execute(command.Id);
            e.Handled = true;
            return;
        }
        if (Tools.HandleShortcut(e.Key, e.KeyModifiers)) e.Handled = true;
    }

    private void OnKeyUpTunnel(object? sender, KeyEventArgs e)
    {
        LastModifiers = e.KeyModifiers;
        if (e.Key == Key.Space && Canvas.SpaceHeld)
        {
            Canvas.SpaceHeld = false;
            e.Handled = true;
        }
        if ((e.Key == Key.LeftAlt || e.Key == Key.RightAlt) && _springReturn != null)
        {
            Tools.Select(_springReturn);
            _springReturn = null;
            e.Handled = true;
        }
        Tools.Active.KeyUp(e.Key, e.KeyModifiers);
    }

    // ------------------------------------------------------------------ drag and drop

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        List<string> paths = e.DataTransfer.TryGetFiles()?.Select(f => f.TryGetLocalPath()).Where(p => !string.IsNullOrEmpty(p)).Select(p => p!).ToList() ?? [];
        foreach (string path in paths.Where(ImageIO.IsSupported))
        {
            if (Document != null && !ImageIO.IsProject(path) && !e.KeyModifiers.HasFlag(KeyModifiers.Shift)) EditorCommands.PlaceFile(this, path);
            else EditorCommands.OpenFile(this, path);
        }
    }
}
