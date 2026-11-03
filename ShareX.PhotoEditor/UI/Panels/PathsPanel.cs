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
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class PathsPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();

    public PathsPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 2, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("path-fill", "Fill path with foreground color", () => EditorCommands.FillPath(Editor)));
        footer.Children.Add(IconButton("path-stroke", "Stroke path with brush", () => EditorCommands.StrokePath(Editor)));
        footer.Children.Add(IconButton("selection", "Load path as a selection", () => EditorCommands.PathToSelection(Editor)));
        footer.Children.Add(IconButton("pen", "Make work path from selection", () => EditorCommands.SelectionToPath(Editor)));
        footer.Children.Add(IconButton("mask", "Add a vector mask (shape layer from path)", () => EditorCommands.PathToShape(Editor)));
        footer.Children.Add(IconButton("new-layer", "Create new path", () => EditorCommands.NewPath(Editor)));
        footer.Children.Add(IconButton("trash", "Delete current path", () => EditorCommands.DeletePath(Editor)));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _list });
        Content = root;
    }

    public override string Id => "paths";

    public override string Title => "Paths";

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        if (doc.WorkPath != null)
        {
            bool selected = doc.ActivePath?.Path == doc.WorkPath;
            Border row = new() { Padding = new Thickness(6, 4), Background = selected ? SelectedBrush : Brushes.Transparent, Child = new TextBlock { Text = "Work Path", FontStyle = FontStyle.Italic } };
            row.PointerPressed += async (_, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    string? name = await DialogBuilder.PromptAsync(Editor, "Save Path", "Name:", $"Path {doc.Paths.Count + 1}");
                    if (string.IsNullOrWhiteSpace(name)) return;
                    NamedPath saved = new(name, doc.WorkPath!);
                    doc.Paths.Add(saved);
                    doc.WorkPath = null;
                    doc.ActivePath = saved;
                    Editor.Commit("Save Path");
                    return;
                }
                doc.ActivePath = selected ? null : new NamedPath("Work Path", doc.WorkPath!);
                Editor.RefreshPanels();
                Editor.InvalidateOverlay();
            };
            _list.Children.Add(row);
        }
        foreach (NamedPath path in doc.Paths)
        {
            bool selected = doc.ActivePath == path;
            Border row = new() { Padding = new Thickness(6, 4), Background = selected ? SelectedBrush : Brushes.Transparent, Child = Text(path.Name) };
            row.PointerPressed += async (_, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    string? name = await DialogBuilder.PromptAsync(Editor, "Rename Path", "Name:", path.Name);
                    if (!string.IsNullOrWhiteSpace(name)) { path.Name = name; Editor.Commit("Rename Path"); }
                    return;
                }
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    doc.ActivePath = path;
                    EditorCommands.PathToSelection(Editor);
                    return;
                }
                doc.ActivePath = selected ? null : path;
                Editor.RefreshPanels();
                Editor.InvalidateOverlay();
            };
            _list.Children.Add(row);
        }
        if (doc.ActiveLayer is ShapeLayer shape)
        {
            _list.Children.Add(new Border { Padding = new Thickness(6, 4), Child = new TextBlock { Text = $"{shape.Name} Shape Path", FontStyle = FontStyle.Italic, Opacity = 0.8 } });
        }
    }
}
