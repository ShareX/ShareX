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

namespace ShareX.PixelEditor.UI.Panels;

/// <summary>The sprite's palette: click a color for the foreground, right-click for the background.</summary>
public sealed class PalettePanel : EditorPanel
{
    private const double SwatchSize = 20;

    private readonly WrapPanel _swatches = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(6) };
    private readonly TextBlock _count = new() { Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0) };
    private readonly List<(Border Swatch, uint Color)> _items = [];
    private IReadOnlyList<uint>? _shown;

    public PalettePanel(PixelEditorWindow editor) : base(editor)
    {
        Button menu = Ui.IconButton(PixelEditorIcons.PanelMenu, Strings.Palette_Menu, () => { }, 12);
        menu.Click += (_, _) => BuildMenu().Open(menu);
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 1 };
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.Plus, Strings.Palette_AddColor, () => Editor.Execute("palette.addForeground"), 12));
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.Minus, Strings.Palette_RemoveColor, () => Editor.Execute("palette.removeForeground"), 12));
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.Ramp, Strings.Palette_Ramp, () => Editor.Execute("palette.ramp"), 12));
        buttons.Children.Add(menu);
        DockPanel bar = new() { Height = 26, Background = Ui.Deep };
        DockPanel.SetDock(buttons, Dock.Right);
        bar.Children.Add(buttons);
        bar.Children.Add(_count);
        DockPanel root = new();
        DockPanel.SetDock(bar, Dock.Bottom);
        root.Children.Add(bar);
        root.Children.Add(new ScrollViewer { Content = _swatches, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled });
        Content = root;
    }

    public override string Id => "palette";

    public override string Title => Strings.Panel_Palette;

    private IReadOnlyList<uint> Colors => Sprite?.Palette ?? (IReadOnlyList<uint>)Palettes.Default;

    private ContextMenu BuildMenu()
    {
        ContextMenu menu = new();
        MenuItem presets = new() { Header = Strings.Palette_Presets };
        foreach (PalettePreset preset in Palettes.Presets)
        {
            MenuItem item = new() { Header = $"{preset.Name} ({preset.Colors.Length})" };
            PalettePreset captured = preset;
            item.Click += (_, _) => Commands.EditorCommands.SetPalette(Editor, captured.Colors, Strings.History_LoadPalette);
            presets.Items.Add(item);
        }
        menu.Items.Add(presets);
        void Add(string commandId)
        {
            Commands.EditorCommand? command = Commands.CommandRegistry.Get(commandId);
            if (command == null) return;
            MenuItem item = new() { Header = command.Text, IsEnabled = command.CanExecute?.Invoke(Editor) ?? true };
            if (command.IsChecked != null)
            {
                item.ToggleType = MenuItemToggleType.CheckBox;
                item.IsChecked = command.IsChecked(Editor);
            }
            item.Click += (_, _) => Editor.Execute(commandId);
            menu.Items.Add(item);
        }
        Add("palette.load");
        Add("palette.save");
        menu.Items.Add(new Separator());
        Add("palette.fromSprite");
        Add("palette.remapSprite");
        menu.Items.Add(new Separator());
        Add("palette.sortHue");
        Add("palette.sortBrightness");
        Add("palette.sortSaturation");
        Add("palette.reverse");
        menu.Items.Add(new Separator());
        Add("palette.ramp");
        Add("palette.removeUnused");
        return menu;
    }

    private ContextMenu SwatchMenu(int index)
    {
        ContextMenu menu = new();
        void Add(string header, Action click)
        {
            MenuItem item = new() { Header = header, IsEnabled = Sprite != null };
            item.Click += (_, _) => click();
            menu.Items.Add(item);
        }
        Add(Strings.Palette_EditColor, () => Commands.EditorCommands.EditPaletteColor(Editor, index));
        Add(Strings.Palette_ReplaceWithForeground, () => Commands.EditorCommands.ReplacePaletteColor(Editor, index, Editor.ForegroundColor));
        Add(Strings.Palette_SelectPixels, () => Commands.EditorCommands.SelectByColor(Editor, Colors[index]));
        menu.Items.Add(new Separator());
        Add(Strings.Palette_RemoveColor, () => Commands.EditorCommands.RemovePaletteColor(Editor, index));
        return menu;
    }

    public override void Refresh()
    {
        IReadOnlyList<uint> colors = Colors;
        if (_shown == null || !_shown.SequenceEqual(colors))
        {
            _shown = [.. colors];
            _swatches.Children.Clear();
            _items.Clear();
            for (int i = 0; i < colors.Count; i++)
            {
                uint color = colors[i];
                int index = i;
                Border swatch = new()
                {
                    Width = SwatchSize,
                    Height = SwatchSize,
                    Background = Argb.A(color) == 255 ? Ui.Brush(color) : Ui.Checker(0xFFFFFFFF, 0xFFC8C8C8, 4),
                    BorderThickness = new Thickness(1),
                    Child = new Border { Background = Argb.A(color) == 255 ? null : Ui.Brush(color), BorderThickness = new Thickness(1) }
                };
                ToolTip.SetTip(swatch, $"{index}: #{Argb.ToHex(color)}");
                swatch.PointerPressed += (_, e) =>
                {
                    PointerPointProperties properties = e.GetCurrentPoint(swatch).Properties;
                    if (properties.IsLeftButtonPressed)
                    {
                        if (e.ClickCount == 2 && Sprite != null) Commands.EditorCommands.EditPaletteColor(Editor, index);
                        else Editor.ForegroundColor = color;
                    }
                    else if (properties.IsRightButtonPressed && e.KeyModifiers.HasFlag(KeyModifiers.Control)) SwatchMenu(index).Open(swatch);
                    else if (properties.IsRightButtonPressed) Editor.BackgroundColor = color;
                    else if (properties.IsMiddleButtonPressed) SwatchMenu(index).Open(swatch);
                    e.Handled = true;
                };
                _items.Add((swatch, color));
                _swatches.Children.Add(swatch);
            }
        }
        _count.Text = string.Format(Strings.Palette_Count, colors.Count);
        RefreshSelection();
    }

    /// <summary>Marks the swatches of the foreground (white frame) and background (dark frame with white corners) colors.</summary>
    public void RefreshSelection()
    {
        foreach ((Border swatch, uint color) in _items)
        {
            bool fg = color == Editor.ForegroundColor, bg = color == Editor.BackgroundColor;
            swatch.BorderBrush = fg ? Brushes.White : bg ? Brushes.Black : Ui.Gutter;
            if (swatch.Child is Border inner) inner.BorderBrush = fg ? Brushes.Black : bg ? Brushes.White : Brushes.Transparent;
        }
    }
}

/// <summary>The undo history; click a state to go back to it.</summary>
public sealed class HistoryPanel : EditorPanel
{
    private readonly StackPanel _list = new();
    private readonly ScrollViewer _scroll;

    public HistoryPanel(PixelEditorWindow editor) : base(editor)
    {
        _scroll = new ScrollViewer { Content = _list, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        Content = _scroll;
    }

    public override string Id => "history";

    public override string Title => Strings.Panel_History;

    public override void Refresh()
    {
        _list.Children.Clear();
        Sprite? sprite = Sprite;
        if (sprite == null) return;
        int index = 0, current = sprite.History.Index;
        Border? active = null;
        foreach (string name in sprite.History.Names)
        {
            int captured = index;
            bool isCurrent = index == current;
            Border row = new()
            {
                Height = 22,
                Padding = new Thickness(10, 0),
                Background = isCurrent ? Ui.RowSelected : Brushes.Transparent,
                Child = new TextBlock { Text = name, VerticalAlignment = VerticalAlignment.Center, Foreground = index > current ? new SolidColorBrush(Color.FromRgb(0x70, 0x70, 0x70)) : Ui.TextBrush, FontStyle = index > current ? FontStyle.Italic : FontStyle.Normal }
            };
            row.PointerEntered += (_, _) =>
            {
                if (!isCurrent) row.Background = Ui.Hover;
            };
            row.PointerExited += (_, _) =>
            {
                if (!isCurrent) row.Background = Brushes.Transparent;
            };
            row.PointerPressed += (_, _) =>
            {
                Editor.CommitFloating();
                if (Editor.Sprite?.History.JumpTo(captured) == true) Editor.AfterHistoryChange();
            };
            if (isCurrent) active = row;
            _list.Children.Add(row);
            index++;
        }
        if (active != null) Avalonia.Threading.Dispatcher.UIThread.Post(() => active.BringIntoView(), Avalonia.Threading.DispatcherPriority.Background);
    }
}
