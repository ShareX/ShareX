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
    private readonly ScrollViewer _recentScroller = new() { MaxHeight = 460, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
    private readonly TextBlock _recentEmpty = new() { Text = Strings.StartScreen_NoRecentFiles, Classes = { "dim" }, Margin = new Thickness(0, 8) };
    private readonly Button _viewModeButton = new() { Classes = { "icon" }, Width = 28, Height = 26, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center };
    private readonly Button _clearButton = new() { Classes = { "icon" }, Width = 28, Height = 26, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center };

    public StartScreen(PhotoEditorWindow editor)
    {
        _editor = editor;
        Background = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28));
        DragDrop.SetAllowDrop(this, true);

        StackPanel actions = new() { Spacing = 8, Width = 190 };
        actions.Children.Add(new TextBlock { Text = Strings.PhotoEditorWindow_PhotoEditor, FontSize = 22, FontWeight = FontWeight.Light, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 14) });
        actions.Children.Add(ActionButton(Strings.StartScreen_NewFile, "file.new", true));
        actions.Children.Add(ActionButton(Strings.EditorCommands_Open, "file.open", false));
        actions.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47)), Margin = new Thickness(0, 8) });
        actions.Children.Add(LinkButton(PhotoEditorIcons.Clipboard, Strings.EditorCommands_NewFromClipboard, "file.newFromClipboard"));
        if (editor.Services.CaptureRegion != null) actions.Children.Add(LinkButton(PhotoEditorIcons.CaptureRegion, Strings.StartScreen_NewFromScreenshotRegion, "file.newFromRegion"));
        if (editor.Services.CaptureFullscreen != null) actions.Children.Add(LinkButton(PhotoEditorIcons.CaptureScreen, Strings.EditorCommands_NewFromScreenshotFullScreen, "file.newFromScreen"));

        _clearButton.Content = new IconView(PhotoEditorIcons.Trash, 16);
        ToolTip.SetTip(_clearButton, Strings.StartScreen_ClearRecentFiles);
        _clearButton.Click += (_, _) => Commands.EditorCommands.ClearRecentFiles(_editor);
        _viewModeButton.Click += (_, _) =>
        {
            _editor.Settings.RecentTiles = !_editor.Settings.RecentTiles;
            _editor.SaveSettings();
            Refresh();
        };

        StackPanel headerButtons = new() { Orientation = Orientation.Horizontal, Spacing = 2, VerticalAlignment = VerticalAlignment.Center };
        headerButtons.Children.Add(_viewModeButton);
        headerButtons.Children.Add(_clearButton);
        Grid header = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), Margin = new Thickness(0, 0, 0, 6) };
        Grid.SetColumn(headerButtons, 1);
        header.Children.Add(new TextBlock { Text = Strings.StartScreen_Recent, FontSize = 16, Foreground = Brushes.White, VerticalAlignment = VerticalAlignment.Center });
        header.Children.Add(headerButtons);

        StackPanel recentColumn = new() { Spacing = 6 };
        recentColumn.Children.Add(header);
        recentColumn.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47)) });
        recentColumn.Children.Add(_recentScroller);
        recentColumn.Children.Add(_recentEmpty);

        Grid layout = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,48,*"),
            Margin = new Thickness(56),
            VerticalAlignment = VerticalAlignment.Top
        };
        Grid.SetColumn(recentColumn, 2);
        layout.Children.Add(actions);
        layout.Children.Add(recentColumn);
        Child = new ScrollViewer { Content = layout };
    }

    public void Refresh()
    {
        bool tiles = _editor.Settings.RecentTiles;
        List<string> files = _editor.Settings.RecentFiles.Where(File.Exists).Take(12).ToList();
        Panel panel = tiles ? new WrapPanel() : new StackPanel { Spacing = 2 };
        foreach (string path in files) panel.Children.Add(tiles ? RecentTile(path) : RecentRow(path));
        _recentScroller.Content = panel;
        _recentScroller.IsVisible = files.Count > 0;
        _recentEmpty.IsVisible = files.Count == 0;
        _clearButton.IsEnabled = _editor.Settings.RecentFiles.Count > 0;

        _viewModeButton.Content = new IconView(tiles ? PhotoEditorIcons.ListView : PhotoEditorIcons.TileView, 16);
        ToolTip.SetTip(_viewModeButton, tiles ? Strings.StartScreen_ShowAsListTip : Strings.StartScreen_ShowAsTilesTip);
    }

    private Button ActionButton(string text, string commandId, bool primary)
    {
        Button button = new()
        {
            Content = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, TextAlignment = Avalonia.Media.TextAlignment.Center },
            Classes = { primary ? "primary" : "secondary" },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            MinHeight = 32,
            FontSize = 13
        };
        button.Click += (_, _) => _editor.Execute(commandId);
        return button;
    }

    private Button LinkButton(string icon, string text, string commandId)
    {
        // Grid instead of a horizontal StackPanel so the label gets a finite width and can wrap.
        Grid content = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 8 };
        content.Children.Add(new IconView(icon, 16));
        TextBlock label = new() { Text = text, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(label, 1);
        content.Children.Add(label);
        Button button = new() { Content = content, Classes = { "icon" }, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch, Padding = new Thickness(6, 5) };
        button.Click += (_, _) => _editor.Execute(commandId);
        return button;
    }

    private Border RecentRow(string path)
    {
        Border thumb = new() { Width = 56, Height = 42, Background = ThumbBrush, CornerRadius = new CornerRadius(2), ClipToBounds = true };
        LoadThumbnail(thumb, path, 112);
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

        return Clickable(new Border { Child = grid, Padding = new Thickness(6) }, path);
    }

    private Border RecentTile(string path)
    {
        Border thumb = new() { Width = 152, Height = 104, Background = ThumbBrush, CornerRadius = new CornerRadius(2), ClipToBounds = true };
        LoadThumbnail(thumb, path, 304);
        StackPanel content = new() { Spacing = 4, Width = 152 };
        content.Children.Add(thumb);
        content.Children.Add(new TextBlock { Text = Path.GetFileName(path), Foreground = Brushes.White, TextTrimming = TextTrimming.CharacterEllipsis });
        content.Children.Add(new TextBlock { Text = File.GetLastWriteTime(path).ToString("g"), Classes = { "dim" }, FontSize = 11, TextTrimming = TextTrimming.CharacterEllipsis });
        return Clickable(new Border { Child = content, Padding = new Thickness(6), Margin = new Thickness(0, 0, 8, 8) }, path);
    }

    /// <summary>Makes a recent-file entry open its file on click and highlight on hover.</summary>
    private Border Clickable(Border row, string path)
    {
        row.CornerRadius = new CornerRadius(3);
        row.Background = Brushes.Transparent;
        row.Cursor = new Cursor(StandardCursorType.Hand);
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

    private static void LoadThumbnail(Border target, string path, int decodeWidth)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is not (".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp")) return;
        _ = Task.Run(() =>
        {
            try
            {
                using FileStream stream = File.OpenRead(path);
                return Bitmap.DecodeToWidth(stream, decodeWidth);
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
