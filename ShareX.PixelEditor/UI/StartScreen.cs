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
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.UI;

/// <summary>Home screen shown while no sprite is open: quick actions, size templates and recent files.</summary>
public sealed class StartScreen : Border
{
    private static readonly IBrush CardBrush = new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32));
    private static readonly IBrush CardHoverBrush = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x3E));
    private static readonly IBrush ThumbBrush = new SolidColorBrush(Color.FromRgb(0x26, 0x26, 0x26));
    private static readonly (int W, int H)[] Templates = [(16, 16), (32, 32), (64, 64), (128, 128), (320, 180)];

    private readonly PixelEditorWindow _editor;
    private readonly StackPanel _recent = new() { Spacing = 2 };
    private readonly TextBlock _recentEmpty = new() { Text = Strings.Start_NoRecent, Classes = { "dim" }, Margin = new Thickness(0, 8) };

    public StartScreen(PixelEditorWindow editor)
    {
        _editor = editor;
        Background = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28));

        StackPanel actions = new() { Spacing = 8, Width = 200 };
        actions.Children.Add(new TextBlock { Text = Strings.Window_Name, FontSize = 22, FontWeight = FontWeight.Light, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 14) });
        actions.Children.Add(ActionButton(Strings.Start_NewSprite, "file.new", true));
        actions.Children.Add(ActionButton(Strings.Start_Open, "file.open", false));
        actions.Children.Add(new Border { Height = 1, Background = Ui.Divider, Margin = new Thickness(0, 8) });
        actions.Children.Add(LinkButton(PixelEditorIcons.Clipboard, Strings.Command_NewFromClipboard, "file.newFromClipboard"));
        actions.Children.Add(LinkButton(PixelEditorIcons.ImportSheet, Strings.Command_ImportSheet, "file.importSheet"));
        actions.Children.Add(new Border { Height = 1, Background = Ui.Divider, Margin = new Thickness(0, 8) });
        actions.Children.Add(new TextBlock { Text = Strings.Start_Templates, Classes = { "dim" }, Margin = new Thickness(0, 0, 0, 2) });
        WrapPanel templates = new() { Orientation = Orientation.Horizontal };
        foreach ((int tw, int th) in Templates)
        {
            Button button = new() { Content = $"{tw}×{th}", Margin = new Thickness(0, 0, 4, 4), Padding = new Thickness(7, 2) };
            button.Click += (_, _) =>
            {
                Sprite sprite = Sprite.Create(tw, th, string.Format(Strings.Sprite_Untitled, 1), $"{Strings.Layer_DefaultName} 1");
                _editor.OpenSprite(sprite, Strings.History_New);
            };
            templates.Children.Add(button);
        }
        actions.Children.Add(templates);

        Button clear = Ui.IconButton(PixelEditorIcons.Trash, Strings.Start_ClearRecent, () =>
        {
            _editor.Settings.RecentFiles.Clear();
            _editor.SaveSettings();
            Refresh();
        });
        Grid header = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), Margin = new Thickness(0, 0, 0, 6) };
        Grid.SetColumn(clear, 1);
        header.Children.Add(new TextBlock { Text = Strings.Start_Recent, FontSize = 16, Foreground = Brushes.White, VerticalAlignment = VerticalAlignment.Center });
        header.Children.Add(clear);

        StackPanel recentColumn = new() { Spacing = 6 };
        recentColumn.Children.Add(header);
        recentColumn.Children.Add(new Border { Height = 1, Background = Ui.Divider });
        recentColumn.Children.Add(_recent);
        recentColumn.Children.Add(_recentEmpty);

        Grid layout = new() { ColumnDefinitions = new ColumnDefinitions("Auto,48,*"), Margin = new Thickness(56), VerticalAlignment = VerticalAlignment.Top, MaxWidth = 980 };
        Grid.SetColumn(recentColumn, 2);
        layout.Children.Add(actions);
        layout.Children.Add(recentColumn);
        Child = new ScrollViewer { Content = layout };
    }

    public void Refresh()
    {
        List<string> files = _editor.Settings.RecentFiles.Where(File.Exists).Take(12).ToList();
        _recent.Children.Clear();
        foreach (string path in files) _recent.Children.Add(RecentRow(path));
        _recent.IsVisible = files.Count > 0;
        _recentEmpty.IsVisible = files.Count == 0;
    }

    private Button ActionButton(string text, string commandId, bool primary)
    {
        Button button = new()
        {
            Content = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center },
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
        Grid content = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 8 };
        content.Children.Add(new IconView(icon, 16));
        TextBlock label = new() { Text = text.Replace("...", string.Empty), TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(label, 1);
        content.Children.Add(label);
        Button button = new() { Content = content, Classes = { "icon" }, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch, Padding = new Thickness(6, 5) };
        button.Click += (_, _) => _editor.Execute(commandId);
        return button;
    }

    private Border RecentRow(string path)
    {
        Border thumb = new() { Width = 44, Height = 44, Background = ThumbBrush, CornerRadius = new CornerRadius(2), ClipToBounds = true };
        LoadThumbnail(thumb, path);
        StackPanel text = new() { VerticalAlignment = VerticalAlignment.Center, Spacing = 2, Margin = new Thickness(12, 0, 0, 0) };
        text.Children.Add(new TextBlock { Text = Path.GetFileName(path), Foreground = Brushes.White, TextTrimming = TextTrimming.CharacterEllipsis });
        text.Children.Add(new TextBlock { Text = Path.GetDirectoryName(path), Classes = { "dim" }, FontSize = 11, TextTrimming = TextTrimming.CharacterEllipsis });
        TextBlock modified = new() { Text = File.GetLastWriteTime(path).ToString("g"), Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 4, 0) };
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto") };
        Grid.SetColumn(text, 1);
        Grid.SetColumn(modified, 2);
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
        _ = CardBrush;
        return row;
    }

    /// <summary>Shows the first frame of the file, with crisp pixels for small sprites.</summary>
    private static void LoadThumbnail(Border target, string path)
    {
        _ = Task.Run(() =>
        {
            try
            {
                Sprite sprite = SpriteFile.Load(path, string.Empty);
                PixelBuffer flat = Compositor.Flatten(sprite, 0);
                double scale = Math.Min(1, 88.0 / Math.Max(flat.Width, flat.Height));
                return scale < 1 ? flat.ScaleNearest(Math.Max(1, (int)(flat.Width * scale)), Math.Max(1, (int)(flat.Height * scale))) : flat;
            }
            catch
            {
                return null;
            }
        }).ContinueWith(task =>
        {
            if (task.Result is not PixelBuffer buffer) return;
            Image image = new() { Source = Ui.ToBitmap(buffer), Stretch = Stretch.Uniform, Margin = new Thickness(3) };
            RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.None);
            target.Child = image;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
