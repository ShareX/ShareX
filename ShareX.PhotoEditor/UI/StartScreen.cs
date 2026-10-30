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
using Avalonia.Media.Imaging;
using ShareX.PhotoEditor.UI.Commands;

namespace ShareX.PhotoEditor.UI;

/// <summary>Home screen shown in the canvas area while no document is open: quick actions and recent files.</summary>
public sealed class StartScreen : Border
{
    private static readonly IBrush CardBrush = new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32));
    private static readonly IBrush CardHoverBrush = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x3E));
    private static readonly IBrush ThumbBrush = new SolidColorBrush(Color.FromRgb(0x26, 0x26, 0x26));

    private readonly PhotoEditorWindow _editor;
    private readonly StackPanel _recent = new() { Spacing = 2 };
    private readonly TextBlock _recentEmpty = new() { Text = "Files you open will appear here.", Classes = { "dim" }, Margin = new Thickness(0, 8) };

    public StartScreen(PhotoEditorWindow editor)
    {
        _editor = editor;
        Background = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28));
        DragDrop.SetAllowDrop(this, true);

        StackPanel actions = new() { Spacing = 8, Width = 190 };
        actions.Children.Add(new TextBlock { Text = "Photo editor", FontSize = 22, FontWeight = FontWeight.Light, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 14) });
        actions.Children.Add(ActionButton("New file", "file.new", true));
        actions.Children.Add(ActionButton("Open", "file.open", false));
        actions.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47)), Margin = new Thickness(0, 8) });
        actions.Children.Add(LinkButton("clipboard", "New from Clipboard", "file.newFromClipboard"));
        if (editor.Services.CaptureRegion != null) actions.Children.Add(LinkButton("capture-region", "New from Screenshot (Region)", "file.newFromRegion"));
        if (editor.Services.CaptureFullscreen != null) actions.Children.Add(LinkButton("capture-screen", "New from Screenshot (Full Screen)", "file.newFromScreen"));

        StackPanel recentColumn = new() { Spacing = 6 };
        recentColumn.Children.Add(new TextBlock { Text = "Recent", FontSize = 16, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 6) });
        recentColumn.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47)) });
        recentColumn.Children.Add(new ScrollViewer { Content = _recent, MaxHeight = 460, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled });
        recentColumn.Children.Add(_recentEmpty);

        Grid layout = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,48,*"),
            MaxWidth = 860,
            Margin = new Thickness(48, 56),
            VerticalAlignment = VerticalAlignment.Top
        };
        Grid.SetColumn(recentColumn, 2);
        layout.Children.Add(actions);
        layout.Children.Add(recentColumn);
        Child = new ScrollViewer { Content = layout };
    }

    public void Refresh()
    {
        _recent.Children.Clear();
        foreach (string path in _editor.Settings.RecentFiles.Where(File.Exists).Take(12)) _recent.Children.Add(RecentRow(path));
        _recentEmpty.IsVisible = _recent.Children.Count == 0;
    }

    private Button ActionButton(string text, string commandId, bool primary)
    {
        Button button = new()
        {
            Content = text,
            Classes = { primary ? "primary" : "secondary" },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Height = 32,
            FontSize = 13
        };
        button.Click += (_, _) => _editor.Execute(commandId);
        return button;
    }

    private Button LinkButton(string icon, string text, string commandId)
    {
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        content.Children.Add(ToolIcons.Create(icon, 16));
        content.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center });
        Button button = new() { Content = content, Classes = { "icon" }, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left, Padding = new Thickness(6, 5) };
        button.Click += (_, _) => _editor.Execute(commandId);
        return button;
    }

    private Border RecentRow(string path)
    {
        Border thumb = new() { Width = 56, Height = 42, Background = ThumbBrush, CornerRadius = new CornerRadius(2), ClipToBounds = true };
        LoadThumbnail(thumb, path);
        StackPanel text = new() { VerticalAlignment = VerticalAlignment.Center, Spacing = 2 };
        text.Children.Add(new TextBlock { Text = Path.GetFileName(path), Foreground = Brushes.White, TextTrimming = TextTrimming.CharacterEllipsis });
        text.Children.Add(new TextBlock { Text = Path.GetDirectoryName(path), Classes = { "dim" }, FontSize = 11, TextTrimming = TextTrimming.CharacterEllipsis });
        TextBlock modified = new() { Text = File.GetLastWriteTime(path).ToString("g"), Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 4, 0) };
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto") };
        Grid.SetColumn(text, 1);
        Grid.SetColumn(modified, 2);
        text.Margin = new Thickness(12, 0, 0, 0);
        grid.Children.Add(thumb);
        grid.Children.Add(text);
        grid.Children.Add(modified);

        Border row = new() { Child = grid, Padding = new Thickness(6), CornerRadius = new CornerRadius(3), Background = Brushes.Transparent, Cursor = new Cursor(StandardCursorType.Hand) };
        ToolTip.SetTip(row, path);
        row.PointerEntered += (_, _) => row.Background = CardHoverBrush;
        row.PointerExited += (_, _) => row.Background = Brushes.Transparent;
        row.PointerPressed += (_, e) =>
        {
            if (!e.GetCurrentPoint(row).Properties.IsLeftButtonPressed) return;
            e.Handled = true;
            EditorCommands.OpenFile(_editor, path);
        };
        return row;
    }

    private static void LoadThumbnail(Border target, string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is not (".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp")) return;
        _ = Task.Run(() =>
        {
            try
            {
                using FileStream stream = File.OpenRead(path);
                return Bitmap.DecodeToWidth(stream, 112);
            }
            catch
            {
                return null;
            }
        }).ContinueWith(task =>
        {
            if (task.Result is Bitmap bitmap) target.Child = new Image { Source = bitmap, Stretch = Stretch.UniformToFill };
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
