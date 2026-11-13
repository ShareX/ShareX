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
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.PixelEditor.Tools;
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.UI;

/// <summary>A search box over every command, tool, panel and recent file, so anything can be run by typing part of its name.</summary>
internal sealed class CommandPalette : Border
{
    private const int MaxResults = 80;
    private const int MaxRecent = 8;

    private sealed record Entry(string Key, string Title, string Path, string? Gesture, Action Run, Func<bool> IsEnabled, Func<bool>? IsChecked)
    {
        public string TitleText { get; } = Normalize(Title);
        public string SearchText { get; } = Normalize(Path + " " + Title);
    }

    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x2F, 0x5D, 0x8C));

    private readonly PixelEditorWindow _editor;
    private readonly TextBox _query;
    private readonly StackPanel _list = new();
    private readonly ScrollViewer _scroll;
    private readonly TextBlock _empty;
    private readonly List<Border> _rows = [];
    private List<Entry> _entries = [];
    private List<Entry> _results = [];
    private int _selected;
    private IInputElement? _returnFocus;

    public CommandPalette(PixelEditorWindow editor)
    {
        _editor = editor;
        IsVisible = false;
        Background = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0));

        _query = new TextBox { PlaceholderText = Strings.CommandPalette_Placeholder, FontSize = 14, Padding = new Thickness(8, 6), MinHeight = 32 };
        _query.TextChanged += (_, _) =>
        {
            _selected = 0;
            Filter();
        };
        _query.AddHandler(KeyDownEvent, OnQueryKeyDown, RoutingStrategies.Tunnel);
        _scroll = new ScrollViewer { Content = _list, MaxHeight = 400, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled, Margin = new Thickness(0, 6, 0, 0) };
        _empty = new TextBlock { Text = Strings.CommandPalette_NoResults, Classes = { "dim" }, Margin = new Thickness(8, 10), IsVisible = false };

        StackPanel content = new();
        content.Children.Add(_query);
        content.Children.Add(_empty);
        content.Children.Add(_scroll);
        Child = new Border
        {
            Width = 580,
            Margin = new Thickness(0, 64, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Background = Ui.Chrome,
            BorderBrush = Ui.Divider,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(6),
            BoxShadow = BoxShadows.Parse("0 10 28 0 #90000000"),
            Child = content
        };
        PointerPressed += (_, e) =>
        {
            // A click on the dimmed backdrop, outside the box, closes the palette.
            if (ReferenceEquals(e.Source, this)) Close();
        };
    }

    public bool IsOpen => IsVisible;

    public void Open()
    {
        if (IsVisible)
        {
            _query.SelectAll();
            _query.Focus();
            return;
        }
        _returnFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
        _entries = BuildEntries();
        _selected = 0;
        IsVisible = true;
        _query.Text = string.Empty;
        Filter();
        Dispatcher.UIThread.Post(() => _query.Focus(), DispatcherPriority.Input);
    }

    public void Close()
    {
        if (!IsVisible) return;
        IsVisible = false;
        _list.Children.Clear();
        _rows.Clear();
        if (_returnFocus is InputElement { IsEffectivelyVisible: true } element && TopLevel.GetTopLevel(element) != null) element.Focus();
        else _editor.Canvas.Focus();
        _returnFocus = null;
    }

    // ------------------------------------------------------------------ entries

    private List<Entry> BuildEntries()
    {
        List<Entry> entries = [];
        HashSet<string> listed = [];
        foreach (MenuNode node in EditorCommands.MainMenu) AddMenu(node, string.Empty, entries, listed);
        foreach (EditorCommand command in CommandRegistry.Commands.Values)
        {
            if (listed.Add(command.Id)) entries.Add(CommandEntry(command, string.Empty));
        }

        foreach (ToolBase tool in _editor.Tools.All)
        {
            ToolBase captured = tool;
            entries.Add(new Entry("tool:" + tool.Id, tool.Name, Strings.CommandPalette_Tool, tool.Shortcut == Key.None ? null : tool.Shortcut.ToString(),
                () => _editor.Tools.Select(captured), () => true, () => ReferenceEquals(_editor.Tools.Active, captured)));
        }

        string panels = CleanHeader(Strings.Menu_View) + " › " + CleanHeader(Strings.Menu_Panels);
        foreach (Panels.EditorPanel panel in _editor.Panels.Panels)
        {
            string id = panel.Id;
            entries.Add(new Entry("panel:" + id, panel.Title, panels, null, () => _editor.Panels.TogglePanel(id), () => true, () => _editor.Panels.IsPanelVisible(id)));
        }

        string recent = CleanHeader(Strings.Menu_File) + " › " + CleanHeader(Strings.Menu_OpenRecent);
        foreach (string path in _editor.Settings.RecentFiles.Where(File.Exists).Take(12))
        {
            string captured = path;
            entries.Add(new Entry("file:" + path, Path.GetFileName(path), recent, null, () => EditorCommands.OpenFile(_editor, captured), () => true, null));
        }
        return entries;
    }

    private void AddMenu(MenuNode node, string path, List<Entry> entries, HashSet<string> listed)
    {
        if (node.CommandId != null)
        {
            EditorCommand? command = CommandRegistry.Get(node.CommandId);
            if (command != null && listed.Add(command.Id)) entries.Add(CommandEntry(command, path));
            return;
        }
        if (node.Children == null || node.Header == null) return;
        string childPath = path.Length == 0 ? CleanHeader(node.Header) : path + " › " + CleanHeader(node.Header);
        foreach (MenuNode child in node.Children) AddMenu(child, childPath, entries, listed);
    }

    private Entry CommandEntry(EditorCommand command, string path)
    {
        string id = command.Id;
        Func<bool>? isChecked = command.IsChecked != null ? () => command.IsChecked(_editor) : null;
        return new Entry(id, command.Text, path, CommandRegistry.GetGesture(id, _editor.Settings), () => _editor.Execute(id),
            () => command.CanExecute?.Invoke(_editor) ?? true, isChecked);
    }

    private static string CleanHeader(string header) => header.Replace("_", string.Empty);

    private static string Normalize(string text) => text.Replace("...", string.Empty).Replace("…", string.Empty).Replace("_", string.Empty).ToLowerInvariant();

    // ------------------------------------------------------------------ filtering

    private void Filter()
    {
        string query = Normalize(_query.Text ?? string.Empty).Trim();
        if (query.Length == 0)
        {
            // Recently run entries first, then everything in menu order.
            List<string> recent = _editor.Settings.RecentCommands;
            _results = recent.Select(key => _entries.FirstOrDefault(e => e.Key == key)).OfType<Entry>()
                .Concat(_entries.Where(e => !recent.Contains(e.Key))).Take(MaxResults).ToList();
        }
        else
        {
            string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _results = _entries.Select((entry, index) => (entry, index, score: Score(entry, query, words)))
                .Where(r => r.score > 0)
                .OrderByDescending(r => r.score).ThenBy(r => r.index)
                .Take(MaxResults).Select(r => r.entry).ToList();
        }
        _selected = Math.Clamp(_selected, 0, Math.Max(0, _results.Count - 1));
        Render();
    }

    /// <summary>Ranks a match: a title that starts with the query beats words that start with each typed word, which beats plain containment and finally letters in order.</summary>
    private int Score(Entry entry, string query, string[] words)
    {
        int score = 0;
        if (entry.TitleText.StartsWith(query, StringComparison.Ordinal)) score += 1000;
        else if (entry.TitleText.Contains(query, StringComparison.Ordinal)) score += 400;
        foreach (string word in words)
        {
            int at = entry.SearchText.IndexOf(word, StringComparison.Ordinal);
            if (at < 0)
            {
                score = 0;
                break;
            }
            bool wordStart = at == 0 || !char.IsLetterOrDigit(entry.SearchText[at - 1]);
            bool inTitle = entry.TitleText.Contains(word, StringComparison.Ordinal);
            score += (wordStart ? 60 : 20) + (inTitle ? 40 : 0);
        }
        if (score == 0 && IsSubsequence(query.Replace(" ", string.Empty), entry.TitleText)) score = 5;
        if (score > 0 && _editor.Settings.RecentCommands.Contains(entry.Key)) score += 30;
        if (score > 0 && !entry.IsEnabled()) score -= 4;
        return score;
    }

    private static bool IsSubsequence(string query, string text)
    {
        int i = 0;
        foreach (char c in text)
        {
            if (i < query.Length && c == query[i]) i++;
        }
        return query.Length > 0 && i == query.Length;
    }

    // ------------------------------------------------------------------ rendering

    private void Render()
    {
        _list.Children.Clear();
        _rows.Clear();
        _empty.IsVisible = _results.Count == 0;
        _scroll.IsVisible = _results.Count > 0;
        for (int i = 0; i < _results.Count; i++)
        {
            Border row = CreateRow(_results[i], i);
            _rows.Add(row);
            _list.Children.Add(row);
        }
        UpdateSelection(false);
        _scroll.Offset = default;
    }

    private Border CreateRow(Entry entry, int index)
    {
        bool enabled = entry.IsEnabled();
        DockPanel dock = new() { LastChildFill = true };
        if (!string.IsNullOrEmpty(entry.Gesture))
        {
            Border key = new()
            {
                Child = new TextBlock { Text = FormatGesture(entry.Gesture), FontSize = 11, Foreground = Ui.DimBrush },
                Padding = new Thickness(6, 1),
                CornerRadius = new CornerRadius(3),
                Background = Ui.Deep,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0)
            };
            DockPanel.SetDock(key, Dock.Right);
            dock.Children.Add(key);
        }

        StackPanel text = new() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        TextBlock check = new() { Text = "✓", Width = 16, FontSize = 12, Foreground = Ui.TextBrush, VerticalAlignment = VerticalAlignment.Center, Opacity = entry.IsChecked?.Invoke() == true ? 1 : 0 };
        text.Children.Add(check);
        if (entry.Path.Length > 0) text.Children.Add(new TextBlock { Text = entry.Path + " › ", Foreground = Ui.DimBrush, VerticalAlignment = VerticalAlignment.Center });
        text.Children.Add(new TextBlock { Text = entry.Title.Replace("...", string.Empty), Foreground = enabled ? Brushes.White : Ui.DimBrush, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis });
        dock.Children.Add(text);

        Border row = new() { Child = dock, Padding = new Thickness(6, 5), CornerRadius = new CornerRadius(4), Background = Brushes.Transparent, Opacity = enabled ? 1 : 0.6, Cursor = new Cursor(StandardCursorType.Hand) };
        row.PointerEntered += (_, _) =>
        {
            if (index != _selected) row.Background = Ui.Hover;
        };
        row.PointerExited += (_, _) =>
        {
            if (index != _selected) row.Background = Brushes.Transparent;
        };
        row.PointerPressed += (_, e) =>
        {
            e.Handled = true;
            _selected = index;
            Run();
        };
        return row;
    }

    private static string FormatGesture(string gesture)
    {
        string first = gesture.Split(',')[0].Trim();
        KeyGesture? parsed = CommandRegistry.ParseGesture(first);
        return parsed?.ToString("p", null) ?? first;
    }

    private void UpdateSelection(bool scroll)
    {
        for (int i = 0; i < _rows.Count; i++) _rows[i].Background = i == _selected ? SelectedBrush : Brushes.Transparent;
        if (scroll && _selected >= 0 && _selected < _rows.Count) _rows[_selected].BringIntoView();
    }

    private void Move(int delta)
    {
        if (_results.Count == 0) return;
        _selected = Math.Clamp(_selected + delta, 0, _results.Count - 1);
        UpdateSelection(true);
    }

    private void OnQueryKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                Close();
                break;
            case Key.Enter:
                Run();
                break;
            case Key.Down:
                Move(1);
                break;
            case Key.Up:
                Move(-1);
                break;
            case Key.PageDown:
                Move(10);
                break;
            case Key.PageUp:
                Move(-10);
                break;
            case Key.Tab:
                Move(e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? -1 : 1);
                break;
            default:
                return;
        }
        e.Handled = true;
    }

    private void Run()
    {
        if (_selected < 0 || _selected >= _results.Count) return;
        Entry entry = _results[_selected];
        if (!entry.IsEnabled()) return;
        List<string> recent = _editor.Settings.RecentCommands;
        recent.Remove(entry.Key);
        recent.Insert(0, entry.Key);
        if (recent.Count > MaxRecent) recent.RemoveRange(MaxRecent, recent.Count - MaxRecent);
        Close();
        // Runs after the palette is gone so dialogs and focus changes start from the editor.
        Dispatcher.UIThread.Post(entry.Run);
    }
}
