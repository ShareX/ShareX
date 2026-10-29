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
using Avalonia.Threading;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI;

/// <summary>Left vertical toolbar with grouped tools, flyouts, color swatches and mode buttons.</summary>
public sealed class ToolbarControl : UserControl
{
    private static readonly IBrush ActiveBrush = new SolidColorBrush(Color.FromArgb(110, 62, 131, 242));
    private readonly PhotoEditorWindow _editor;
    private const double SingleColumnWidth = 44;
    private const double TwoColumnWidth = 80;
    private readonly WrapPanel _tools = new() { Orientation = Orientation.Horizontal, ItemWidth = 36, ItemHeight = 37, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Dictionary<ToolGroup, Button> _buttons = [];
    private readonly Button _columnsToggle;
    private Border? _foreground;
    private Border? _background;
    private Button? _quickMask;

    public ToolbarControl(PhotoEditorWindow editor)
    {
        _editor = editor;
        _columnsToggle = new Button { Padding = new Thickness(4, 1), HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Center, Background = Brushes.Transparent };
        _columnsToggle.Click += (_, _) => ToggleColumns();
        StackPanel root = new() { Spacing = 6, Margin = new Thickness(4, 2, 4, 6) };
        root.Children.Add(_columnsToggle);
        root.Children.Add(_tools);
        root.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromArgb(70, 128, 128, 128)) });
        root.Children.Add(BuildColorWidget());
        _quickMask = SmallButton("quick-mask", "Edit in Quick Mask Mode (Q)", () => Commands.EditorCommands.ToggleQuickMask(_editor));
        root.Children.Add(_quickMask);
        root.Children.Add(SmallButton("screen-mode", "Change Screen Mode (F)", () => _editor.CycleScreenMode()));
        root.Children.Add(SmallButton("edit-toolbar", "Edit Toolbar...", () => _ = ToolbarCustomizeDialog.ShowAsync(_editor)));
        Content = new ScrollViewer { Content = root, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        ApplyColumns();
        Rebuild();
        _editor.Tools.ActiveChanged += RefreshActive;
    }

    /// <summary>Switches between the single-column and two-column toolbar layouts.</summary>
    public void ToggleColumns()
    {
        _editor.Settings.ToolbarTwoColumns = !_editor.Settings.ToolbarTwoColumns;
        ApplyColumns();
    }

    private void ApplyColumns()
    {
        bool two = _editor.Settings.ToolbarTwoColumns;
        // The wrap panel lays tools out in one or two columns depending on the available width.
        Width = two ? TwoColumnWidth : SingleColumnWidth;
        _tools.Width = two ? 72 : 36;
        _columnsToggle.Content = ToolIcons.Create(two ? "toolbar-collapse" : "toolbar-expand", 12);
        ToolTip.SetTip(_columnsToggle, two ? "Single column toolbar" : "Two column toolbar");
    }

    private Button SmallButton(string icon, string tip, Action click)
    {
        Button button = new() { Content = ToolIcons.Create(icon, 18), Padding = new Thickness(6), HorizontalAlignment = HorizontalAlignment.Center, Background = Brushes.Transparent };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) => click();
        return button;
    }

    public void Rebuild()
    {
        _tools.Children.Clear();
        _buttons.Clear();
        foreach (ToolGroup group in _editor.Tools.Groups)
        {
            Grid content = new() { Width = 30, Height = 30 };
            content.Children.Add(new ContentControl { Content = ToolIcons.Create(group.Current.Id, 20), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            if (group.Tools.Count > 1)
            {
                // Small corner triangle marks tools that have a flyout.
                content.Children.Add(new Avalonia.Controls.Shapes.Path
                {
                    Data = Geometry.Parse("M0 4 L4 4 L4 0 Z"),
                    Fill = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 1, 1)
                });
            }
            Button button = new() { Content = content, Padding = new Thickness(3), Background = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Center };
            UpdateTip(button, group.Current);
            ToolGroup captured = group;
            DispatcherTimer? holdTimer = null;
            button.Click += (_, _) => _editor.Tools.Select(captured.Current);
            button.AddHandler(PointerPressedEvent, (_, e) =>
            {
                if (e.GetCurrentPoint(button).Properties.IsRightButtonPressed)
                {
                    ShowFlyout(captured, button);
                    e.Handled = true;
                    return;
                }
                holdTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(450) };
                holdTimer.Tick += (_, _) =>
                {
                    holdTimer?.Stop();
                    ShowFlyout(captured, button);
                };
                holdTimer.Start();
            }, Avalonia.Interactivity.RoutingStrategies.Tunnel);
            button.AddHandler(PointerReleasedEvent, (_, _) => holdTimer?.Stop(), Avalonia.Interactivity.RoutingStrategies.Tunnel);
            _buttons[group] = button;
            _tools.Children.Add(button);
        }
        RefreshActive();
    }

    private static void UpdateTip(Button button, ToolBase tool)
    {
        char? key = ToolManager.GetShortcut(tool.Id);
        ToolTip.SetTip(button, tool.Name + (key != null ? $" ({key})" : string.Empty) + (tool.Hint.Length > 0 ? "\n" + tool.Hint : string.Empty));
    }

    private void ShowFlyout(ToolGroup group, Button anchor)
    {
        if (group.Tools.Count < 2) return;
        MenuFlyout flyout = new() { Placement = PlacementMode.RightEdgeAlignedTop };
        foreach (ToolBase tool in group.Tools)
        {
            char? key = ToolManager.GetShortcut(tool.Id);
            MenuItem item = new()
            {
                Header = tool.Name,
                Icon = ToolIcons.Create(tool.Id, 16),
                InputGesture = key != null ? new KeyGesture(Key.A + (key.Value - 'A')) : null,
                FontWeight = tool == group.Current ? FontWeight.Bold : FontWeight.Normal
            };
            ToolBase captured = tool;
            item.Click += (_, _) => _editor.Tools.Select(captured);
            flyout.Items.Add(item);
        }
        flyout.ShowAt(anchor);
    }

    public void RefreshActive()
    {
        foreach (var (group, button) in _buttons)
        {
            bool active = group.Tools.Contains(_editor.Tools.Active);
            button.Background = active ? ActiveBrush : Brushes.Transparent;
            if (button.Content is Grid grid && grid.Children[0] is ContentControl icon)
            {
                icon.Content = ToolIcons.Create(group.Current.Id, 20);
            }
            UpdateTip(button, group.Current);
        }
        RefreshColors();
    }

    private Control BuildColorWidget()
    {
        _foreground = new Border { Width = 22, Height = 22, BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
        _background = new Border { Width = 22, Height = 22, BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
        ToolTip.SetTip(_foreground, "Set foreground color");
        ToolTip.SetTip(_background, "Set background color");
        _foreground.PointerPressed += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(_editor, _editor.Foreground, "Foreground Color", _editor);
            if (c is SKColor color) _editor.Foreground = color;
        };
        _background.PointerPressed += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(_editor, _editor.Background, "Background Color", _editor);
            if (c is SKColor color) _editor.Background = color;
        };
        Canvas canvas = new() { Width = 36, Height = 36, HorizontalAlignment = HorizontalAlignment.Center };
        Canvas.SetLeft(_background, 12);
        Canvas.SetTop(_background, 12);
        canvas.Children.Add(_background);
        canvas.Children.Add(_foreground);

        Button swap = new() { Content = ToolIcons.Create("swap", 10), Padding = new Thickness(0), Background = Brushes.Transparent, Width = 12, Height = 12 };
        ToolTip.SetTip(swap, "Switch foreground and background colors (X)");
        swap.Click += (_, _) => Commands.EditorCommands.SwapColors(_editor);
        Canvas.SetLeft(swap, 24);
        Canvas.SetTop(swap, -2);
        canvas.Children.Add(swap);

        Button reset = new() { Content = ToolIcons.Create("default-colors", 10), Padding = new Thickness(0), Background = Brushes.Transparent, Width = 12, Height = 12 };
        ToolTip.SetTip(reset, "Default foreground and background colors (D)");
        reset.Click += (_, _) => Commands.EditorCommands.ResetColors(_editor);
        Canvas.SetLeft(reset, -2);
        Canvas.SetTop(reset, 24);
        canvas.Children.Add(reset);
        RefreshColors();
        return canvas;
    }

    public void RefreshColors()
    {
        if (_foreground != null) _foreground.Background = DialogBuilder.ToBrush(_editor.Foreground.WithAlpha(255));
        if (_background != null) _background.Background = DialogBuilder.ToBrush(_editor.Background.WithAlpha(255));
        if (_quickMask != null) _quickMask.Background = _editor.Document?.QuickMask == true ? ActiveBrush : Brushes.Transparent;
    }
}
