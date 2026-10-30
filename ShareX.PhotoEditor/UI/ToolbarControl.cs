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
    private const double ButtonWidth = 34;
    private const double ButtonHeight = 31;
    private readonly PhotoEditorWindow _editor;
    private readonly StackPanel _sections = new() { Spacing = 0, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Dictionary<ToolGroup, Button> _buttons = [];
    private readonly Button _columnsToggle;
    private Border? _foreground;
    private Border? _background;
    private Button? _quickMask;

    public ToolbarControl(PhotoEditorWindow editor)
    {
        _editor = editor;
        _columnsToggle = new Button { Classes = { "icon" }, Padding = new Thickness(0, 2), HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Center };
        _columnsToggle.Click += (_, _) => ToggleColumns();

        StackPanel root = new() { Spacing = 0, Margin = new Thickness(0, 0, 0, 6) };
        root.Children.Add(new Border { Child = _columnsToggle, Padding = new Thickness(2, 1), Margin = new Thickness(0, 0, 0, 4) });
        root.Children.Add(_sections);
        root.Children.Add(Separator());
        root.Children.Add(SmallButton("edit-toolbar", "Edit Toolbar...", () => _ = ToolbarCustomizeDialog.ShowAsync(_editor)));
        root.Children.Add(Separator());
        root.Children.Add(BuildColorWidget());
        _quickMask = SmallButton("quick-mask", "Edit in Quick Mask Mode (Q)", () => Commands.EditorCommands.ToggleQuickMask(_editor));
        root.Children.Add(_quickMask);
        root.Children.Add(SmallButton("screen-mode", "Change Screen Mode (F)", () => _editor.CycleScreenMode()));
        Content = new ScrollViewer { Content = root, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        ApplyColumns();
        Rebuild();
        _editor.Tools.ActiveChanged += RefreshActive;
    }

    private static Border Separator() => new() { Classes = { "hsep" }, Margin = new Thickness(6, 4) };

    /// <summary>Switches between the single-column and two-column toolbar layouts.</summary>
    public void ToggleColumns()
    {
        _editor.Settings.ToolbarTwoColumns = !_editor.Settings.ToolbarTwoColumns;
        ApplyColumns();
        Rebuild();
    }

    private void ApplyColumns()
    {
        bool two = _editor.Settings.ToolbarTwoColumns;
        Width = two ? ButtonWidth * 2 + 8 : ButtonWidth + 6;
        _columnsToggle.Content = ToolIcons.Create(two ? "toolbar-collapse" : "toolbar-expand", 11);
        ToolTip.SetTip(_columnsToggle, two ? "Collapse to a single column" : "Expand to two columns");
    }

    private Button SmallButton(string icon, string tip, Action click)
    {
        Button button = new()
        {
            Classes = { "tool" },
            Content = ToolIcons.Create(icon, 17),
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = ButtonWidth - 2,
            Height = 28
        };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) => click();
        return button;
    }

    /// <summary>Visual sections of the toolbar, separated by thin dividers like the reference layout.</summary>
    private static int SectionOf(ToolGroup group)
    {
        ToolId first = group.Tools[0].Id;
        return first switch
        {
            ToolId.SpotHealingBrush or ToolId.Remove or ToolId.HealingBrush or ToolId.Patch or ToolId.ContentAwareMove or ToolId.RedEye or
            ToolId.Brush or ToolId.Pencil or ToolId.ColorReplacement or ToolId.MixerBrush or ToolId.AdjustmentBrush or
            ToolId.CloneStamp or ToolId.PatternStamp or ToolId.HistoryBrush or ToolId.ArtHistoryBrush or
            ToolId.Eraser or ToolId.BackgroundEraser or ToolId.MagicEraser or ToolId.Gradient or ToolId.PaintBucket or
            ToolId.Blur or ToolId.Sharpen or ToolId.Smudge or ToolId.Dodge or ToolId.Burn or ToolId.Sponge => 1,
            ToolId.Pen or ToolId.FreeformPen or ToolId.CurvaturePen or ToolId.AddAnchorPoint or ToolId.DeleteAnchorPoint or ToolId.ConvertPoint or
            ToolId.HorizontalType or ToolId.VerticalType or ToolId.HorizontalTypeMask or ToolId.VerticalTypeMask or
            ToolId.PathSelection or ToolId.DirectSelection or
            ToolId.Rectangle or ToolId.Ellipse or ToolId.Triangle or ToolId.Polygon or ToolId.Line or ToolId.CustomShape => 2,
            ToolId.Hand or ToolId.RotateView or ToolId.Zoom => 3,
            _ => 0
        };
    }

    public void Rebuild()
    {
        _sections.Children.Clear();
        _buttons.Clear();
        bool two = _editor.Settings.ToolbarTwoColumns;
        WrapPanel? panel = null;
        int section = -1;
        foreach (ToolGroup group in _editor.Tools.Groups)
        {
            int groupSection = SectionOf(group);
            if (panel == null || groupSection != section)
            {
                if (panel != null) _sections.Children.Add(Separator());
                panel = new WrapPanel { Orientation = Orientation.Horizontal, ItemWidth = ButtonWidth, ItemHeight = ButtonHeight, Width = two ? ButtonWidth * 2 : ButtonWidth };
                _sections.Children.Add(panel);
                section = groupSection;
            }
            panel.Children.Add(CreateToolButton(group));
        }
        RefreshActive();
    }

    private Button CreateToolButton(ToolGroup group)
    {
        Grid content = new() { Width = 32, Height = 29 };
        content.Children.Add(new ContentControl { Content = ToolIcons.Create(group.Current.Id, 18), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
        if (group.Tools.Count > 1)
        {
            // Small corner triangle marks tools that have a flyout.
            content.Children.Add(new Avalonia.Controls.Shapes.Path
            {
                Data = Geometry.Parse("M0 4 L4 4 L4 0 Z"),
                Fill = new SolidColorBrush(Color.FromRgb(0xB8, 0xB8, 0xB8)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 3, 3)
            });
        }
        Button button = new() { Classes = { "tool" }, Content = content, Width = 32, Height = 29, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        UpdateTip(button, group.Current);
        DispatcherTimer? holdTimer = null;
        button.Click += (_, _) => _editor.Tools.Select(group.Current);
        button.AddHandler(PointerPressedEvent, (_, e) =>
        {
            if (e.GetCurrentPoint(button).Properties.IsRightButtonPressed)
            {
                ShowFlyout(group, button);
                e.Handled = true;
                return;
            }
            holdTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(450) };
            holdTimer.Tick += (_, _) =>
            {
                holdTimer?.Stop();
                ShowFlyout(group, button);
            };
            holdTimer.Start();
        }, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        button.AddHandler(PointerReleasedEvent, (_, _) => holdTimer?.Stop(), Avalonia.Interactivity.RoutingStrategies.Tunnel);
        _buttons[group] = button;
        return button;
    }

    private static void UpdateTip(Button button, ToolBase tool)
    {
        char? key = ToolManager.GetShortcut(tool.Id);
        StackPanel tip = new() { Spacing = 2, MaxWidth = 300 };
        tip.Children.Add(new TextBlock { Text = tool.Name + (key != null ? $" ({key})" : string.Empty), FontWeight = FontWeight.SemiBold, Foreground = Brushes.Black });
        if (tool.Hint.Length > 0) tip.Children.Add(new TextBlock { Text = tool.Hint, TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40)) });
        ToolTip.SetTip(button, tip);
        ToolTip.SetPlacement(button, PlacementMode.Right);
    }

    private void ShowFlyout(ToolGroup group, Button anchor)
    {
        if (group.Tools.Count < 2) return;
        MenuFlyout flyout = new() { Placement = PlacementMode.RightEdgeAlignedTop };
        foreach (ToolBase tool in group.Tools)
        {
            char? key = ToolManager.GetShortcut(tool.Id);
            bool current = tool == group.Current;
            Grid header = new() { ColumnDefinitions = new ColumnDefinitions("10,22,*,Auto"), MinWidth = 220 };
            if (current) header.Children.Add(new Border { Width = 5, Height = 5, CornerRadius = new CornerRadius(3), Background = Brushes.White, VerticalAlignment = VerticalAlignment.Center });
            Control icon = ToolIcons.Create(tool.Id, 16);
            Grid.SetColumn(icon, 1);
            TextBlock name = new() { Text = tool.Name, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0, 16, 0) };
            Grid.SetColumn(name, 2);
            TextBlock shortcut = new() { Text = key?.ToString() ?? string.Empty, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(shortcut, 3);
            header.Children.Add(icon);
            header.Children.Add(name);
            header.Children.Add(shortcut);
            MenuItem item = new() { Header = header, Height = 28 };
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
            button.Classes.Set("checked", group.Tools.Contains(_editor.Tools.Active));
            if (button.Content is Grid grid && grid.Children[0] is ContentControl icon)
            {
                icon.Content = ToolIcons.Create(group.Current.Id, 18);
            }
            UpdateTip(button, group.Current);
        }
        RefreshColors();
    }

    private Control BuildColorWidget()
    {
        IBrush outline = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));
        _foreground = new Border { Width = 20, Height = 20, BorderBrush = outline, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
        _background = new Border { Width = 20, Height = 20, BorderBrush = outline, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
        // A light inner ring makes dark swatches readable on the dark chrome.
        Border foregroundRing = new() { Child = _foreground, BorderBrush = new SolidColorBrush(Color.FromRgb(0xD0, 0xD0, 0xD0)), BorderThickness = new Thickness(1) };
        Border backgroundRing = new() { Child = _background, BorderBrush = new SolidColorBrush(Color.FromRgb(0xD0, 0xD0, 0xD0)), BorderThickness = new Thickness(1) };
        ToolTip.SetTip(foregroundRing, "Set foreground color");
        ToolTip.SetTip(backgroundRing, "Set background color");
        foregroundRing.PointerPressed += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(_editor, _editor.Foreground, "Foreground Color", _editor);
            if (c is SKColor color) _editor.Foreground = color;
        };
        backgroundRing.PointerPressed += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(_editor, _editor.Background, "Background Color", _editor);
            if (c is SKColor color) _editor.Background = color;
        };
        Canvas canvas = new() { Width = 34, Height = 36, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 2, 0, 4) };
        Canvas.SetLeft(backgroundRing, 12);
        Canvas.SetTop(backgroundRing, 14);
        Canvas.SetLeft(foregroundRing, 2);
        Canvas.SetTop(foregroundRing, 4);
        canvas.Children.Add(backgroundRing);
        canvas.Children.Add(foregroundRing);

        Button swap = new() { Classes = { "icon" }, Content = ToolIcons.Create("swap", 9), Padding = new Thickness(1), Width = 12, Height = 12 };
        ToolTip.SetTip(swap, "Switch Foreground and Background Colors (X)");
        swap.Click += (_, _) => Commands.EditorCommands.SwapColors(_editor);
        Canvas.SetLeft(swap, 23);
        Canvas.SetTop(swap, 0);
        canvas.Children.Add(swap);

        Button reset = new() { Classes = { "icon" }, Content = ToolIcons.Create("default-colors", 9), Padding = new Thickness(1), Width = 12, Height = 12 };
        ToolTip.SetTip(reset, "Default Foreground and Background Colors (D)");
        reset.Click += (_, _) => Commands.EditorCommands.ResetColors(_editor);
        Canvas.SetLeft(reset, 0);
        Canvas.SetTop(reset, 26);
        canvas.Children.Add(reset);
        RefreshColors();
        return canvas;
    }

    public void RefreshColors()
    {
        if (_foreground != null) _foreground.Background = DialogBuilder.ToBrush(_editor.Foreground.WithAlpha(255));
        if (_background != null) _background.Background = DialogBuilder.ToBrush(_editor.Background.WithAlpha(255));
        _quickMask?.Classes.Set("checked", _editor.Document?.QuickMask == true);
    }
}
