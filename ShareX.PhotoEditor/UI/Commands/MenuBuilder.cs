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

using Avalonia.Controls;

namespace ShareX.PhotoEditor.UI.Commands;

/// <summary>Builds the main menu bar from the command registry and menu tree.</summary>
public static class MenuBuilder
{
    public static Menu Build(PhotoEditorWindow editor)
    {
        Menu menu = new();
        foreach (MenuNode node in EditorCommands.MainMenu) menu.Items.Add(CreateItem(editor, node));
        return menu;
    }

    private static object CreateItem(PhotoEditorWindow editor, MenuNode node)
    {
        if (node.IsSeparator) return new Separator();
        if (node.CommandId != null)
        {
            EditorCommand? command = CommandRegistry.Get(node.CommandId);
            MenuItem item = new() { Header = command?.Text ?? node.CommandId, Tag = node.CommandId };
            string? gesture = CommandRegistry.GetGesture(node.CommandId, editor.Settings);
            if (gesture != null) item.InputGesture = CommandRegistry.ParseGesture(gesture);
            if (command?.IsChecked != null) item.ToggleType = MenuItemToggleType.CheckBox;
            item.Click += (_, _) => editor.Execute(node.CommandId);
            return item;
        }
        MenuItem parent = new() { Header = node.Header };
        if (node.Action != null)
        {
            parent.Click += (_, _) => node.Action(editor);
            return parent;
        }
        if (node.Children != null)
        {
            foreach (MenuNode child in node.Children) parent.Items.Add(CreateItem(editor, child));
        }
        int staticCount = parent.Items.Count;
        if (node.Dynamic != null)
        {
            parent.Items.Add(new MenuItem { Header = "(empty)", IsEnabled = false });
        }
        parent.SubmenuOpened += (_, e) =>
        {
            // SubmenuOpened bubbles up from nested submenus; only refresh when this menu itself opens,
            // otherwise rebuilding the items would close the submenu that was just opened.
            if (!ReferenceEquals(e.Source, parent)) return;
            RefreshItems(editor, parent, node, staticCount);
        };
        return parent;
    }

    /// <summary>Updates enabled/checked state and rebuilds the dynamic part of a submenu when it opens.</summary>
    private static void RefreshItems(PhotoEditorWindow editor, MenuItem parent, MenuNode node, int staticCount)
    {
        if (node.Dynamic != null)
        {
            while (parent.Items.Count > staticCount) parent.Items.RemoveAt(parent.Items.Count - 1);
            List<MenuNode> dynamic = node.Dynamic(editor).ToList();
            if (dynamic.Count == 0) parent.Items.Add(new MenuItem { Header = "(empty)", IsEnabled = false });
            foreach (MenuNode child in dynamic) parent.Items.Add(CreateItem(editor, child));
        }
        foreach (object? item in parent.Items)
        {
            if (item is not MenuItem { Tag: string id } menuItem) continue;
            EditorCommand? command = CommandRegistry.Get(id);
            if (command == null) continue;
            menuItem.IsEnabled = command.CanExecute?.Invoke(editor) ?? true;
            if (command.IsChecked != null) menuItem.IsChecked = command.IsChecked(editor);
        }
    }
}
