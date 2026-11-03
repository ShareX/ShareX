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

using Avalonia.Input;

namespace ShareX.PhotoEditor.UI.Commands;

/// <summary>A menu/keyboard command of the Photo editor.</summary>
public sealed class EditorCommand
{
    public EditorCommand(string id, string text, string? gesture, Action<PhotoEditorWindow> execute, Func<PhotoEditorWindow, bool>? canExecute = null, Func<PhotoEditorWindow, bool>? isChecked = null)
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

    public Action<PhotoEditorWindow> Execute { get; }

    public Func<PhotoEditorWindow, bool>? CanExecute { get; }

    public Func<PhotoEditorWindow, bool>? IsChecked { get; }
}

/// <summary>Menu tree node: a command, a submenu, a separator or a dynamic item list.</summary>
public sealed class MenuNode
{
    public string? Header { get; init; }
    public string? CommandId { get; init; }
    public List<MenuNode>? Children { get; init; }
    public Func<PhotoEditorWindow, IEnumerable<MenuNode>>? Dynamic { get; init; }
    public Action<PhotoEditorWindow>? Action { get; init; }
    public bool IsSeparator { get; init; }

    public static MenuNode Separator { get; } = new() { IsSeparator = true };

    public static MenuNode Cmd(string id) => new() { CommandId = id };

    public static MenuNode Sub(string header, params MenuNode[] children) => new() { Header = header, Children = children.ToList() };
}

/// <summary>Holds every command, the menu layout and the effective keyboard shortcuts.</summary>
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

    /// <summary>Returns the effective gesture text for a command, honoring user customization.</summary>
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
