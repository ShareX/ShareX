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
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class HistoryPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();

    public HistoryPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 2, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("new-document", "Create new document from current state", () => EditorCommands.NewDocumentFromState(Editor)));
        footer.Children.Add(IconButton("snapshot", "Create new snapshot", () =>
        {
            Doc?.History.CreateSnapshot();
            Refresh();
        }));
        footer.Children.Add(IconButton("trash", "Delete the states after the current one", () =>
        {
            Doc?.History.DiscardRedo();
            Refresh();
        }));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _list, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled });
        Content = root;
    }

    public override string Id => "history";

    public override string Title => "History";

    /// <summary>Picks an icon for a history state from the name of the operation.</summary>
    private static string HistoryIcon(string name)
    {
        string n = name.ToLowerInvariant();
        if (n.Contains("open") || n.Contains("new document") || n.Contains("duplicate image")) return "new-document";
        if (n.Contains("brush") || n.Contains("pencil") || n.Contains("paint") || n.Contains("stamp") || n.Contains("healing") || n.Contains("smudge") || n.Contains("blur") || n.Contains("dodge") || n.Contains("burn") || n.Contains("sponge")) return "brush";
        if (n.Contains("eras")) return "eraser";
        if (n.Contains("select") || n.Contains("marquee") || n.Contains("lasso") || n.Contains("wand") || n.Contains("deselect")) return "rectangularmarquee";
        if (n.Contains("type") || n.Contains("text")) return "horizontaltype";
        if (n.Contains("move") || n.Contains("nudge") || n.Contains("align")) return "move";
        if (n.Contains("crop") || n.Contains("trim")) return "crop";
        if (n.Contains("transform") || n.Contains("rotate") || n.Contains("flip") || n.Contains("scale")) return "freetransform";
        if (n.Contains("gradient")) return "gradient";
        if (n.Contains("fill") || n.Contains("bucket")) return "paintbucket";
        if (n.Contains("layer") || n.Contains("merge") || n.Contains("flatten") || n.Contains("group")) return "new-layer";
        if (n.Contains("shape") || n.Contains("rectangle") || n.Contains("ellipse") || n.Contains("polygon")) return "rectangle";
        if (n.Contains("path") || n.Contains("pen") || n.Contains("anchor")) return "pen";
        return "adjust";
    }

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        History history = doc.History;
        foreach (HistoryEntry snapshot in history.Snapshots) _list.Children.Add(CreateRow(doc, snapshot, -1, true));
        if (history.Snapshots.Count > 0) _list.Children.Add(new Border { Height = 1, Background = Brushes.Gray, Margin = new Thickness(0, 2) });
        Control? current = null;
        for (int i = 0; i < history.Entries.Count; i++)
        {
            Control row = CreateRow(doc, history.Entries[i], i, false);
            if (i == history.Index) current = row;
            _list.Children.Add(row);
        }
        // Keep the current state visible, like the reference panel.
        if (current != null) Avalonia.Threading.Dispatcher.UIThread.Post(() => current.BringIntoView(), Avalonia.Threading.DispatcherPriority.Background);
    }

    private Control CreateRow(PhotoDocument doc, HistoryEntry entry, int index, bool snapshot)
    {
        History history = doc.History;
        // Left column: the history brush source marker, like the reference panel.
        bool isSource = history.Source == entry;
        Border source = new()
        {
            Width = 22,
            Height = 22,
            Background = Brushes.Transparent,
            Child = isSource ? ToolIcons.Create("historybrush", 14) : null,
            Cursor = new Cursor(StandardCursorType.Hand),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)),
            BorderThickness = new Thickness(0, 0, 1, 0)
        };
        ToolTip.SetTip(source, "Set the source for the history brush");
        source.PointerPressed += (_, e) =>
        {
            history.Source = entry;
            e.Handled = true;
            Refresh();
        };
        TextBlock name = Text(entry.Name);
        name.Margin = new Thickness(4, 0, 0, 0);
        bool future = !snapshot && index > history.Index;
        if (future) name.Opacity = 0.45;
        Control icon = snapshot ? ToolIcons.Create("snapshot", 16) : ToolIcons.Create(HistoryIcon(entry.Name), 14);
        icon.Opacity = future ? 0.45 : 0.85;
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
        content.Children.Add(source);
        content.Children.Add(icon);
        content.Children.Add(name);
        Border row = new()
        {
            Height = snapshot ? 30 : 24,
            Background = !snapshot && index == history.Index ? SelectedBrush : Brushes.Transparent,
            Child = content
        };
        row.PointerPressed += (_, e) =>
        {
            if (snapshot) history.RestoreSnapshot(entry);
            else history.JumpTo(index);
            Editor.AfterHistoryChange();
        };
        if (snapshot)
        {
            ContextMenu menu = new();
            MenuItem rename = new() { Header = "Rename Snapshot..." };
            rename.Click += async (_, _) =>
            {
                string? text = await DialogBuilder.PromptAsync(Editor, "Rename Snapshot", "Name:", entry.Name);
                if (!string.IsNullOrWhiteSpace(text)) { entry.Name = text; Refresh(); }
            };
            MenuItem delete = new() { Header = "Delete Snapshot" };
            delete.Click += (_, _) => { history.DeleteSnapshot(entry); Refresh(); };
            menu.Items.Add(rename);
            menu.Items.Add(delete);
            row.ContextMenu = menu;
        }
        return row;
    }
}
