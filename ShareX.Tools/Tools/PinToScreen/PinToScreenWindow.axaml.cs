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

using ShareX.AvaloniaUI.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Input;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Platform;

namespace ShareX.Tools;

public partial class PinToScreenWindow : Window
{
    private const int ShadowMargin = 10;
    private const string TransparentPixelPng = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";
    private readonly byte[] _imageData;
    private readonly Action<byte[]> _copyImage;
    private readonly System.Drawing.Point? _requestedLocation;
    private readonly Bitmap _bitmap;
    private Border _imageBorder = null!;
    private Image _pinnedImage = null!;
    private Border _toolbar = null!;
    private TextBlock _scaleText = null!;
    private int _imageScale;
    private int _imageOpacity;
    private bool _minimized;
    private bool _opened;
    private bool _dragging;
    private PixelSize _windowPixelSize;

    public PinToScreenOptions Options { get; }

    public PinToScreenWindow() : this(Convert.FromBase64String(TransparentPixelPng), new PinToScreenOptions(), _ => { })
    {
    }

    public PinToScreenWindow(byte[] imageData, PinToScreenOptions options, Action<byte[]> copyImage,
        System.Drawing.Point? location = null)
    {
        _imageData = imageData;
        _copyImage = copyImage;
        _requestedLocation = location;
        Options = options;
        _imageScale = Math.Clamp(options.InitialScale, 20, 500);
        _imageOpacity = Math.Clamp(options.InitialOpacity, 10, 100);

        using MemoryStream stream = new(imageData, writable: false);
        _bitmap = new Bitmap(stream);

        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _imageBorder = this.FindControl<Border>("ImageBorder")!;
        _pinnedImage = this.FindControl<Image>("PinnedImage")!;
        _toolbar = this.FindControl<Border>("Toolbar")!;
        _scaleText = this.FindControl<TextBlock>("ScaleText")!;
        _pinnedImage.Source = _bitmap;

        Opened += OnOpened;
        Closed += (_, _) => _bitmap.Dispose();
        ScalingChanged += OnScalingChanged;
        ApplyOptions(resize: false);
        UpdateCursor();
        ConfigureInitialPlacement();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        _opened = true;
        ConfigureInitialPlacement();
    }

    private void ConfigureInitialPlacement()
    {
        PlatformPoint? anchor = _requestedLocation.HasValue
            ? new PlatformPoint(_requestedLocation.Value.X, _requestedLocation.Value.Y)
            : PlatformServices.IsInitialized ? PlatformServices.Current.Windows.GetCursorPosition() : null;
        ScreenInfo[] screens = Screens.All.Select((screen, index) => new ScreenInfo(index.ToString(), screen.DisplayName ?? string.Empty,
            new PlatformRectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height),
            new PlatformRectangle(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height),
            screen.IsPrimary, screen.Scaling)).ToArray();
        ScreenInfo? target = PinToScreenGeometry.FindScreen(screens, anchor);

        // Like ShareX 21, 100% shows the image pixel for pixel, so sizes are worked out in physical pixels for the screen the
        // window is about to be placed on, which may not be the one it opened on.
        double scaling = target?.ScaleFactor ?? RenderScaling;
        ResizeWindow(keepCenter: false, scaling);

        if (_requestedLocation is System.Drawing.Point origin)
        {
            // The image goes exactly where it was captured, so the border and shadow sit outside it.
            int inset = (int)Math.Round(GetOuterInsetPixels(scaling) * WindowScaling.GetPositionScaling(scaling) / scaling);
            Position = new PixelPoint(origin.X - inset, origin.Y - inset);
            return;
        }

        Position = PinToScreenGeometry.GetInitialPosition(target, new Avalonia.Size(Width, Height), null, 0,
            Options.Placement, Options.PlacementOffset);
    }

    private void OnScalingChanged(object? sender, EventArgs e)
    {
        UpdateCursor();

        if (_opened)
        {
            // Keep the image pixel for pixel when the window moves to a monitor with a different scale.
            ResizeWindow(keepCenter: false);
        }
    }

    private void ApplyOptions(bool resize = true)
    {
        Topmost = Options.TopMost;
        Opacity = _minimized ? 1 : _imageOpacity / 100d;
        _imageBorder.Background = new SolidColorBrush(ToAvalonia(Options.BackgroundColor));
        _imageBorder.BorderBrush = new SolidColorBrush(ToAvalonia(Options.BorderColor));
        _imageBorder.BoxShadow = Options.Shadow
            ? new BoxShadows(new BoxShadow { Blur = 14, OffsetY = 3, Color = Color.FromArgb(110, 0, 0, 0) })
            : new BoxShadows();
        RenderOptions.SetBitmapInterpolationMode(_pinnedImage,
            Options.HighQualityScale ? BitmapInterpolationMode.HighQuality : BitmapInterpolationMode.None);

        if (resize && _opened)
        {
            ResizeWindow(Options.KeepCenterLocation);
        }
    }

    private void ResizeWindow(bool keepCenter) => ResizeWindow(keepCenter, RenderScaling);

    private void ResizeWindow(bool keepCenter, double scaling)
    {
        if (!double.IsFinite(scaling) || scaling <= 0)
        {
            scaling = 1;
        }

        // Size every part in whole physical pixels and convert to DIPs, so layout rounding can't shift or blur the image.
        PixelPoint previousPosition = Position;
        PixelSize previousSize = _windowPixelSize;
        PixelSize imageSize = GetDisplayedImagePixelSize(scaling);
        int borderSize = GetBorderPixels(scaling);
        int shadowSize = GetShadowPixels(scaling);
        int inset = borderSize + shadowSize;
        PixelSize windowSize = new(imageSize.Width + inset * 2, imageSize.Height + inset * 2);

        _imageBorder.BorderThickness = new Thickness(borderSize / scaling);
        _imageBorder.Margin = new Thickness(shadowSize / scaling);
        _pinnedImage.Width = imageSize.Width / scaling;
        _pinnedImage.Height = imageSize.Height / scaling;

        double width = windowSize.Width / scaling;
        double height = windowSize.Height / scaling;
        Width = width;
        Height = height;
        MinWidth = width;
        MaxWidth = width;
        MinHeight = height;
        MaxHeight = height;
        _windowPixelSize = windowSize;

        if (keepCenter && previousSize.Width > 0 && previousSize.Height > 0)
        {
            // Windows are placed in device pixels, except on macOS, which places them in points.
            Position = PinToScreenGeometry.KeepCenter(previousPosition,
                new Avalonia.Size(previousSize.Width / scaling, previousSize.Height / scaling),
                new Avalonia.Size(width, height), WindowScaling.GetPositionScaling(scaling));
        }

        _scaleText.Text = $"{_imageScale}%";
        UpdateToolbarVisibility(IsPointerOver);
    }

    private PixelSize GetDisplayedImagePixelSize(double scaling)
    {
        if (_minimized)
        {
            return new PixelSize(Math.Max(1, (int)Math.Round(Options.MinimizeSize.Width * scaling)),
                Math.Max(1, (int)Math.Round(Options.MinimizeSize.Height * scaling)));
        }

        double scale = _imageScale / 100d;
        return new PixelSize(Math.Max(1, (int)Math.Round(_bitmap.PixelSize.Width * scale)),
            Math.Max(1, (int)Math.Round(_bitmap.PixelSize.Height * scale)));
    }

    private int GetBorderPixels(double scaling) =>
        Options.Border ? Math.Max(1, (int)Math.Round(Math.Clamp(Options.BorderSize, 1, 30) * scaling)) : 0;

    private int GetShadowPixels(double scaling) => Options.Shadow ? (int)Math.Round(ShadowMargin * scaling) : 0;

    private int GetOuterInsetPixels(double scaling) => GetBorderPixels(scaling) + GetShadowPixels(scaling);

    private void UpdateCursor()
    {
        Cursor = _dragging
            ? CursorAssetLoader.GetClosedHandCursor(RenderScaling)
            : CursorAssetLoader.GetOpenHandCursor(RenderScaling);
    }

    private void ChangeScale(int delta)
    {
        if (_minimized) return;
        int value = Math.Clamp(_imageScale + delta, 20, 500);
        if (value != _imageScale)
        {
            _imageScale = value;
            ResizeWindow(Options.KeepCenterLocation);
        }
    }

    private void ChangeOpacity(int delta)
    {
        if (_minimized) return;
        _imageOpacity = Math.Clamp(_imageOpacity + delta, 10, 100);
        Opacity = _imageOpacity / 100d;
    }

    private void ResetImage()
    {
        _imageScale = 100;
        _imageOpacity = 100;
        Opacity = 1;
        ResizeWindow(Options.KeepCenterLocation);
    }

    private void ToggleMinimized()
    {
        _minimized = !_minimized;
        Opacity = _minimized ? 1 : _imageOpacity / 100d;
        ResizeWindow(Options.KeepCenterLocation);
    }

    private void UpdateToolbarVisibility(bool pointerInside)
    {
        _toolbar.IsVisible = pointerInside && Width >= 155 && Height >= 55;
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) => UpdateToolbarVisibility(true);
    private void OnPointerExited(object? sender, PointerEventArgs e) => UpdateToolbarVisibility(false);

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint point = e.GetCurrentPoint(this);
        if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            if (e.ClickCount > 1)
            {
                ToggleMinimized();
            }
            else
            {
                _dragging = true;
                UpdateCursor();

                try
                {
                    // On Windows this runs the move loop and returns once the button is released.
                    BeginMoveDrag(e);
                }
                finally
                {
                    _dragging = false;
                    UpdateCursor();
                }
            }
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        PointerPoint point = e.GetCurrentPoint(this);
        if (point.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
        {
            Close();
            e.Handled = true;
        }
        else if (point.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased && !_minimized)
        {
            ResetImage();
            e.Handled = true;
        }
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_minimized) return;
        int direction = e.Delta.Y > 0 ? 1 : e.Delta.Y < 0 ? -1 : 0;
        if (direction == 0) return;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            ChangeOpacity(direction * Math.Max(1, Options.OpacityStep));
        }
        else if (e.KeyModifiers == KeyModifiers.None)
        {
            ChangeScale(direction * Math.Max(1, Options.ScaleStep));
        }
        e.Handled = true;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        int distance = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
        switch (e.Key)
        {
            case Key.Left: Position = Position.WithX(Position.X - distance); e.Handled = true; return;
            case Key.Right: Position = Position.WithX(Position.X + distance); e.Handled = true; return;
            case Key.Up: Position = Position.WithY(Position.Y - distance); e.Handled = true; return;
            case Key.Down: Position = Position.WithY(Position.Y + distance); e.Handled = true; return;
        }

        if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.C)
        {
            _copyImage(_imageData);
            e.Handled = true;
            return;
        }

        bool plus = e.Key is Key.Add or Key.OemPlus;
        bool minus = e.Key is Key.Subtract or Key.OemMinus;
        if (!_minimized && (plus || minus))
        {
            int direction = plus ? 1 : -1;
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                ChangeOpacity(direction * Math.Max(1, Options.OpacityStep));
            else
                ChangeScale(direction * Math.Max(1, Options.ScaleStep));
            e.Handled = true;
        }
    }

    private void OnToolbarPointerPressed(object? sender, PointerPressedEventArgs e) => e.Handled = true;
    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => _copyImage(_imageData);
    private void OnResetScaleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ResetImage();
    private void OnCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

    private async void OnOptionsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _toolbar.IsVisible = false;
        PinToScreenOptionsWindow window = new(Options);
        if (await window.ShowDialog<bool>(this))
        {
            ApplyOptions();
        }
    }

    private static Color ToAvalonia(System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
}

internal static class PixelPointExtensions
{
    public static PixelPoint WithX(this PixelPoint point, int x) => new(x, point.Y);
    public static PixelPoint WithY(this PixelPoint point, int y) => new(point.X, y);
}
