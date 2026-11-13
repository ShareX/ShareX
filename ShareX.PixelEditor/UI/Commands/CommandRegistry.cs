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
using Avalonia.Input;

namespace ShareX.PixelEditor.UI.Commands;

/// <summary>A menu and keyboard command of the Pixel editor.</summary>
public sealed class EditorCommand
{
    public EditorCommand(string id, string text, string? gesture, Action<PixelEditorWindow> execute, Func<PixelEditorWindow, bool>? canExecute = null, Func<PixelEditorWindow, bool>? isChecked = null)
    {
        Id = id;
        Text = text;
        DefaultGesture = gesture;
        Execute = execute;
        CanExecute = canExecute;
        IsChecked = isChecked;
    }

    public string Id { get; }

    public string Text { get; }

    public string? DefaultGesture { get; }

    public Action<PixelEditorWindow> Execute { get; }

    public Func<PixelEditorWindow, bool>? CanExecute { get; }

    public Func<PixelEditorWindow, bool>? IsChecked { get; }

    /// <summary>The command works with pixels that are being moved, so they are not dropped before it runs.</summary>
    public bool KeepsFloating { get; init; }

    /// <summary>The command changes the sprite, so a running animation stops first.</summary>
    public bool StopsPlayback { get; init; } = true;
}

/// <summary>Menu tree node: a command, a submenu, a separator or a dynamic item list.</summary>
public sealed class MenuNode
{
    public string? Header { get; init; }
    public string? CommandId { get; init; }
    public List<MenuNode>? Children { get; init; }
    public Func<PixelEditorWindow, IEnumerable<MenuNode>>? Dynamic { get; init; }
    public Action<PixelEditorWindow>? Action { get; init; }
    public bool IsSeparator { get; init; }
    public bool Checked { get; init; }

    public static MenuNode Separator { get; } = new() { IsSeparator = true };

    public static MenuNode Cmd(string id) => new() { CommandId = id };

    public static MenuNode Sub(string header, params MenuNode[] children) => new() { Header = header, Children = children.ToList() };
}

/// <summary>Holds every command and the effective keyboard shortcuts.</summary>
public static class CommandRegistry
{
    private static readonly Dictionary<string, EditorCommand> _commands = [];

    public static IReadOnlyDictionary<string, EditorCommand> Commands => _commands;

    public static void Register(EditorCommand command) => _commands[command.Id] = command;

    public static EditorCommand? Get(string id) => _commands.TryGetValue(id, out EditorCommand? command) ? command : null;

    static CommandRegistry()
    {
        EditorCommands.RegisterAll();
    }

    public static void EnsureInitialized()
    {
    }

    /// <summary>The effective gesture text of a command, honoring user customization.</summary>
    public static string? GetGesture(string id, EditorSettings settings)
    {
        if (settings.Shortcuts.TryGetValue(id, out string? custom)) return string.IsNullOrWhiteSpace(custom) ? null : custom;
        return Get(id)?.DefaultGesture;
    }

    public static KeyGesture? ParseGesture(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        try
        {
            return KeyGesture.Parse(text.Split(',')[0].Trim());
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>Finds the command bound to a key press.</summary>
    public static EditorCommand? Match(Key key, KeyModifiers modifiers, EditorSettings settings)
    {
        foreach (EditorCommand command in _commands.Values)
        {
            string? text = GetGesture(command.Id, settings);
            if (text == null) continue;
            foreach (string part in text.Split(','))
            {
                KeyGesture? gesture = ParseGesture(part);
                if (gesture != null && gesture.Key == key && gesture.KeyModifiers == modifiers) return command;
            }
        }
        return null;
    }
}

/// <summary>Builds the main menu bar from the command registry and menu tree.</summary>
public static class MenuBuilder
{
    public static Menu Build(PixelEditorWindow editor)
    {
        Menu menu = new();
        foreach (MenuNode node in EditorCommands.MainMenu) menu.Items.Add(CreateItem(editor, node));
        return menu;
    }

    public static object CreateItem(PixelEditorWindow editor, MenuNode node)
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
            if (node.Checked)
            {
                parent.ToggleType = MenuItemToggleType.CheckBox;
                parent.IsChecked = true;
            }
            parent.Click += (_, _) => node.Action(editor);
            return parent;
        }
        if (node.Children != null)
        {
            foreach (MenuNode child in node.Children) parent.Items.Add(CreateItem(editor, child));
        }
        int staticCount = parent.Items.Count;
        if (node.Dynamic != null) parent.Items.Add(new MenuItem { Header = Strings.Menu_Empty, IsEnabled = false });
        parent.SubmenuOpened += (_, e) =>
        {
            // SubmenuOpened bubbles up from nested submenus; only refresh when this menu itself opens,
            // otherwise rebuilding the items would close the submenu that was just opened.
            if (!ReferenceEquals(e.Source, parent)) return;
            RefreshItems(editor, parent, node, staticCount);
        };
        return parent;
    }

    /// <summary>Updates enabled and checked state and rebuilds the dynamic part of a submenu when it opens.</summary>
    private static void RefreshItems(PixelEditorWindow editor, MenuItem parent, MenuNode node, int staticCount)
    {
        if (node.Dynamic != null)
        {
            while (parent.Items.Count > staticCount) parent.Items.RemoveAt(parent.Items.Count - 1);
            List<MenuNode> dynamic = node.Dynamic(editor).ToList();
            if (dynamic.Count == 0) parent.Items.Add(new MenuItem { Header = Strings.Menu_Empty, IsEnabled = false });
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
