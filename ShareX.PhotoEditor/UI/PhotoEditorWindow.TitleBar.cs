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
using Avalonia.Controls.Chrome;
using Path = Avalonia.Controls.Shapes.Path;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.PhotoEditor.UI.Commands;

namespace ShareX.PhotoEditor.UI;

public sealed partial class PhotoEditorWindow
{
    private const double TitleBarHeight = 32;

    private Border _titleBar = null!;
    private Border _menuHost = null!;
    private TextBlock _titleText = null!;
    private Button _maximizeButton = null!;
    private Path _maximizeGlyph = null!;

    /// <summary>Builds the custom title bar: icon and menu on the left, document name centered, caption buttons on the right.</summary>
    private Border CreateTitleBar()
    {
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaTitleBarHeightHint = TitleBarHeight;
        WindowDecorations = WindowDecorations.BorderOnly;

        Image icon = new() { Width = 16, Height = 16, Margin = new Thickness(10, 0, 4, 0), Source = LoadTitleBarIcon(), IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center };
        RenderOptions.SetBitmapInterpolationMode(icon, BitmapInterpolationMode.HighQuality);
        _menuHost = new Border { Child = CreateMenu(), VerticalAlignment = VerticalAlignment.Center };
        StackPanel left = new() { Orientation = Orientation.Horizontal };
        left.Children.Add(icon);
        left.Children.Add(_menuHost);

        _titleText = new TextBlock { TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0), Text = Strings.PhotoEditorWindow_PhotoEditor, Classes = { "dim" } };

        Button minimize = CreateCaptionButton(Strings.TitleBar_Minimize, "M0 5.5 H10", WindowDecorationsElementRole.MinimizeButton);
        minimize.Click += (_, _) => WindowState = WindowState.Minimized;
        Button maximize = CreateCaptionButton(Strings.TitleBar_Maximize, MaximizeGlyph, WindowDecorationsElementRole.MaximizeButton);
        _maximizeButton = maximize;
        _maximizeGlyph = (Path)maximize.Content!;
        maximize.Click += (_, _) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        Button close = CreateCaptionButton(Strings.EditorCommands_Close, "M0 0 L10 10 M10 0 L0 10", WindowDecorationsElementRole.CloseButton);
        close.Classes.Add("close");
        close.Click += (_, _) => Close();
        StackPanel right = new() { Orientation = Orientation.Horizontal };
        right.Children.Add(minimize);
        right.Children.Add(maximize);
        right.Children.Add(close);

        TitleBarPanel content = new();
        content.Children.Add(left);
        content.Children.Add(_titleText);
        content.Children.Add(right);
        Border bar = new() { Height = TitleBarHeight, Child = content };
        bar.Bind(Border.BackgroundProperty, bar.GetResourceObservable("PE.Brush.Chrome"));
        WindowDecorationProperties.SetElementRole(bar, WindowDecorationsElementRole.TitleBar);

        PropertyChanged += (_, e) =>
        {
            if (e.Property == WindowStateProperty) UpdateMaximizeGlyph();
        };
        UpdateMaximizeGlyph();
        return bar;
    }

    private const string MaximizeGlyph = "M0.5 0.5 H9.5 V9.5 H0.5 Z";
    private const string RestoreGlyph = "M0.5 2.5 H7.5 V9.5 H0.5 Z M2.5 2.5 V0.5 H9.5 V7.5 H7.5";

    private void UpdateMaximizeGlyph()
    {
        bool maximized = WindowState == WindowState.Maximized;
        _maximizeGlyph.Data = Geometry.Parse(maximized ? RestoreGlyph : MaximizeGlyph);
        ToolTip.SetTip(_maximizeButton, maximized ? Strings.TitleBar_Restore : Strings.TitleBar_Maximize);
    }

    private static Button CreateCaptionButton(string tip, string glyph, WindowDecorationsElementRole role)
    {
        // Stroke comes from the caption style so the close button can switch it on hover.
        Path path = new() { Data = Geometry.Parse(glyph), Width = 10, Height = 10, StrokeThickness = 1, Stretch = Stretch.None };
        Button button = new() { Classes = { "caption" }, Content = path, Focusable = false };
        ToolTip.SetTip(button, tip);
        WindowDecorationProperties.SetElementRole(button, role);
        return button;
    }

    private Menu CreateMenu()
    {
        Menu menu = MenuBuilder.Build(this);
        menu.Background = Brushes.Transparent;
        // The menu sits inside the title bar, so it must take its own input instead of dragging the window.
        WindowDecorationProperties.SetElementRole(menu, WindowDecorationsElementRole.User);
        return menu;
    }

    public void RebuildMenu() => _menuHost.Child = CreateMenu();

    private static Bitmap? LoadTitleBarIcon()
    {
        try
        {
            using SKBitmap bitmap = ShareX.HelpersLib.SkiaImageHelpers.ByteArrayToBitmap(ShareX.HelpersLib.ShareXResources.IconBytes);
            using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using Stream stream = data.AsStream();
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Lays out the left, center and right title bar parts, keeping the center part centered on the window when it fits.</summary>
    private sealed class TitleBarPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0, height = 0;
            foreach (Control child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }
            return new Size(double.IsInfinity(availableSize.Width) ? width : availableSize.Width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Control left = Children[0], center = Children[1], right = Children[2];
            double leftWidth = Math.Min(left.DesiredSize.Width, finalSize.Width);
            double rightWidth = Math.Min(right.DesiredSize.Width, finalSize.Width - leftWidth);
            left.Arrange(new Rect(0, 0, leftWidth, finalSize.Height));
            right.Arrange(new Rect(finalSize.Width - rightWidth, 0, rightWidth, finalSize.Height));
            double space = Math.Max(0, finalSize.Width - leftWidth - rightWidth);
            double centerWidth = Math.Min(center.DesiredSize.Width, space);
            double x = Math.Clamp((finalSize.Width - centerWidth) / 2, leftWidth, leftWidth + space - centerWidth);
            center.Arrange(new Rect(x, 0, centerWidth, finalSize.Height));
            return finalSize;
        }
    }
}
