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
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.PixelEditor.Tools;
using Path = Avalonia.Controls.Shapes.Path;

namespace ShareX.PixelEditor.UI;

/// <summary>Left vertical toolbar with grouped tools, flyouts and the foreground and background colors.</summary>
public sealed class ToolbarControl : UserControl
{
    private const double ButtonWidth = 34;
    private const double ButtonHeight = 31;
    private const double ColorWidgetWidth = 46;
    private static readonly IBrush SwatchOutline = new SolidColorBrush(Color.FromRgb(0x18, 0x18, 0x18));

    private readonly PixelEditorWindow _editor;
    private readonly StackPanel _sections = new() { Spacing = 0, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Dictionary<ToolGroup, Button> _buttons = [];
    private readonly Button _columnsToggle;
    private Border? _foreground, _background;

    public ToolbarControl(PixelEditorWindow editor)
    {
        _editor = editor;
        _columnsToggle = new Button { Classes = { "icon" }, Padding = new Thickness(6, 2), HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left };
        _columnsToggle.Click += (_, _) => ToggleColumns();

        StackPanel root = new() { Spacing = 0, Margin = new Thickness(0, 0, 0, 6) };
        root.Children.Add(new Border { Child = _columnsToggle, Padding = new Thickness(2, 1), Margin = new Thickness(0, 0, 0, 4) });
        root.Children.Add(_sections);
        root.Children.Add(new Border { Height = 9 });
        root.Children.Add(BuildColorWidget());
        Content = new ScrollViewer { Content = root, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        ApplyColumns();
        Rebuild();
        _editor.Tools.ActiveChanged += RefreshActive;
    }

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
        Width = two ? ButtonWidth * 2 + 8 : Math.Max(ButtonWidth + 6, ColorWidgetWidth + 2);
        _columnsToggle.Content = new IconView(two ? PixelEditorIcons.ToolbarCollapse : PixelEditorIcons.ToolbarExpand, 12);
        ToolTip.SetTip(_columnsToggle, two ? Strings.Toolbar_Collapse : Strings.Toolbar_Expand);
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
            // Two columns flow continuously so a section with an odd tool count leaves no empty cell; one column keeps section gaps.
            int groupSection = two ? 0 : group.Section;
            if (panel == null || groupSection != section)
            {
                if (panel != null) _sections.Children.Add(new Border { Height = 9 });
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
        content.Children.Add(new ContentControl { Content = new IconView(group.Current.Icon, 20), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
        if (group.Tools.Count > 1)
        {
            // Small corner triangle marks tools that have a flyout; stepped so every edge sits on whole pixels.
            content.Children.Add(new Path
            {
                Data = Geometry.Parse("M3 0 H4 V4 H0 V3 H1 V2 H2 V1 H3 Z"),
                Fill = new SolidColorBrush(Color.FromRgb(0xB8, 0xB8, 0xB8)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 3, 3)
            });
        }
        Button button = new() { Classes = { "tool" }, Content = content, Width = 32, Height = 29, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
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
        StackPanel tip = new() { Spacing = 2, MaxWidth = 300 };
        tip.Children.Add(new TextBlock { Text = $"{tool.Name} ({tool.Shortcut})", FontWeight = FontWeight.SemiBold, Foreground = Brushes.Black });
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
            bool current = tool == group.Current;
            Grid header = new() { ColumnDefinitions = new ColumnDefinitions("10,22,*,Auto"), MinWidth = 200 };
            if (current) header.Children.Add(new Border { Width = 5, Height = 5, CornerRadius = new CornerRadius(3), Background = Brushes.White, VerticalAlignment = VerticalAlignment.Center });
            Control icon = new IconView(tool.Icon, 16);
            Grid.SetColumn(icon, 1);
            TextBlock name = new() { Text = tool.Name, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0, 16, 0) };
            Grid.SetColumn(name, 2);
            TextBlock shortcut = new() { Text = tool.Shortcut.ToString(), Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
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
        foreach ((ToolGroup group, Button button) in _buttons)
        {
            button.Classes.Set("checked", group.Tools.Contains(_editor.Tools.Active));
            if (button.Content is Grid grid && grid.Children[0] is ContentControl icon) icon.Content = new IconView(group.Current.Icon, 20);
            UpdateTip(button, group.Current);
        }
        RefreshColors();
    }

    /// <summary>Foreground and background swatches: overlapping squares with the switch arrow at the top right and the default colors icon at the bottom left.</summary>
    private Control BuildColorWidget()
    {
        Border backgroundSwatch = CreateSwatch(out _background, Strings.Toolbar_BackgroundColor);
        Border foregroundSwatch = CreateSwatch(out _foreground, Strings.Toolbar_ForegroundColor);
        foregroundSwatch.PointerPressed += async (_, _) =>
        {
            uint? c = await Ui.PickColorAsync(_editor, _editor.ForegroundColor, Strings.Toolbar_ForegroundColor);
            if (c is uint color) _editor.ForegroundColor = color;
        };
        backgroundSwatch.PointerPressed += async (_, _) =>
        {
            uint? c = await Ui.PickColorAsync(_editor, _editor.BackgroundColor, Strings.Toolbar_BackgroundColor);
            if (c is uint color) _editor.BackgroundColor = color;
        };
        Canvas canvas = new() { Width = ColorWidgetWidth, Height = 45, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 4, 0, 4) };
        Place(canvas, backgroundSwatch, 17, 17);
        Place(canvas, foregroundSwatch, 2, 1);
        Place(canvas, CreateWidgetButton(CreateSwapIcon(), Strings.Toolbar_SwitchColors, () => _editor.Execute("color.swap")), 35, 0);
        Place(canvas, CreateWidgetButton(CreateDefaultColorsIcon(), Strings.Toolbar_DefaultColors, () => _editor.Execute("color.default")), 0, 33);
        RefreshColors();
        return canvas;
    }

    private static void Place(Canvas canvas, Control control, double left, double top)
    {
        Canvas.SetLeft(control, left);
        Canvas.SetTop(control, top);
        canvas.Children.Add(control);
    }

    private static Border CreateSwatch(out Border color, string tip)
    {
        color = new Border { Width = 22, Height = 22 };
        Border swatch = new()
        {
            BorderBrush = SwatchOutline,
            BorderThickness = new Thickness(1),
            Child = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Background = Ui.Checker(0xFFFFFFFF, 0xFFC8C8C8, 4), Child = color },
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        ToolTip.SetTip(swatch, tip);
        return swatch;
    }

    private static Button CreateWidgetButton(Control icon, string tip, Action click)
    {
        Button button = new() { Classes = { "swatch-icon" }, Content = icon, Padding = new Thickness(0), Width = icon.Width, Height = icon.Height };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) => click();
        return button;
    }

    /// <summary>Curved arrow with heads on both ends, drawn on an 11 px grid.</summary>
    private static Control CreateSwapIcon()
    {
        Canvas icon = new() { Width = 11, Height = 11 };
        void Add(string data, bool stroked)
        {
            Path path = new() { Data = Geometry.Parse(data), StrokeThickness = stroked ? 1.8 : 0 };
            if (stroked) path.Stroke = Ui.TextBrush;
            else path.Fill = Ui.TextBrush;
            icon.Children.Add(path);
        }
        Add("M-0.4 3 L3 -0.2 L3 6.2 Z", false);
        Add("M4.8 8 L11.2 8 L8 11.4 Z", false);
        Add("M2.5 3 H5 Q8 3 8 6 V8.5", true);
        return icon;
    }

    /// <summary>Miniature black-over-white swatch pair sharing one outline, drawn on a 12 px grid.</summary>
    private static Control CreateDefaultColorsIcon()
    {
        Canvas icon = new() { Width = 12, Height = 12 };
        IBrush outline = new SolidColorBrush(Color.FromRgb(0xB6, 0xB6, 0xB6));
        void Rect(double x, double y, double size, IBrush fill)
        {
            Rectangle rect = new() { Width = size, Height = size, Fill = fill };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            icon.Children.Add(rect);
        }
        Rect(0, 0, 9, outline);
        Rect(3, 3, 9, outline);
        Rect(4, 4, 7, Brushes.Black);
        Rect(5, 5, 5, Brushes.White);
        Rect(1, 1, 7, Brushes.Black);
        return icon;
    }

    public void RefreshColors()
    {
        if (_foreground != null) _foreground.Background = Ui.Brush(_editor.ForegroundColor);
        if (_background != null) _background.Background = Ui.Brush(_editor.BackgroundColor);
    }
}
