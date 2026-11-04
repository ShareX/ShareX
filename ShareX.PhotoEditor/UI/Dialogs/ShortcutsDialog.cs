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

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class ShortcutsDialog
{
    public static async Task ShowAsync(PhotoEditorWindow owner)
    {
        Dictionary<string, string> edits = new(owner.Settings.Shortcuts);
        TextBox filter = new() { PlaceholderText = "Search commands" };
        StackPanel list = new() { Spacing = 2 };
        void Build()
        {
            list.Children.Clear();
            string term = filter.Text ?? string.Empty;
            foreach (EditorCommand command in CommandRegistry.Commands.Values.OrderBy(c => c.Id))
            {
                if (term.Length > 0 && !command.Text.Contains(term, StringComparison.OrdinalIgnoreCase) && !command.Id.Contains(term, StringComparison.OrdinalIgnoreCase)) continue;
                string current = edits.TryGetValue(command.Id, out string? custom) ? custom : command.DefaultGesture ?? string.Empty;
                TextBox gesture = new() { Text = current, Width = 200, PlaceholderText = "none" };
                string id = command.Id;
                gesture.KeyDown += (_, e) =>
                {
                    // Record the pressed key combination.
                    if (e.Key is Key.LeftCtrl or Key.RightCtrl or Key.LeftShift or Key.RightShift or Key.LeftAlt or Key.RightAlt) return;
                    if (e.Key == Key.Back && e.KeyModifiers == KeyModifiers.None) { gesture.Text = string.Empty; edits[id] = string.Empty; e.Handled = true; return; }
                    if (e.Key == Key.Tab) return;
                    KeyGesture recorded = new(e.Key, e.KeyModifiers);
                    gesture.Text = recorded.ToString();
                    edits[id] = gesture.Text;
                    e.Handled = true;
                    // A shortcut can only trigger one command, so it moves away from any command that used it.
                    bool moved = false;
                    foreach (EditorCommand other in CommandRegistry.Commands.Values)
                    {
                        if (other.Id == id) continue;
                        string otherText = edits.TryGetValue(other.Id, out string? otherCustom) ? otherCustom : other.DefaultGesture ?? string.Empty;
                        List<string> parts = otherText.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
                        int removed = parts.RemoveAll(p => CommandRegistry.ParseGesture(p) is KeyGesture g && g.Key == recorded.Key && g.KeyModifiers == recorded.KeyModifiers);
                        if (removed == 0) continue;
                        edits[other.Id] = string.Join(", ", parts);
                        moved = true;
                    }
                    if (moved) Build();
                };
                Grid row = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
                TextBlock name = new() { Text = $"{command.Text.Replace("...", string.Empty)}  ({command.Id})", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
                Grid.SetColumn(gesture, 1);
                row.Children.Add(name);
                row.Children.Add(gesture);
                list.Children.Add(row);
            }
        }
        filter.TextChanged += (_, _) => Build();
        Build();
        Button reset = new() { Content = "Use Defaults" };
        reset.Click += (_, _) => { edits.Clear(); Build(); };
        DockPanel content = new() { Height = 520 };
        DockPanel.SetDock(filter, Dock.Top);
        content.Children.Add(filter);
        content.Children.Add(new ScrollViewer { Content = list, Margin = new Thickness(0, 8, 0, 0) });
        if (!await DialogBuilder.ShowAsync(owner, "Keyboard Shortcuts", content, 640, "OK", null, [reset])) return;
        // Store only gestures that differ from the defaults.
        owner.Settings.Shortcuts = edits.Where(e => CommandRegistry.Get(e.Key)?.DefaultGesture != e.Value).ToDictionary(e => e.Key, e => e.Value);
        owner.SaveSettings();
        owner.RebuildMenu();
    }
}
