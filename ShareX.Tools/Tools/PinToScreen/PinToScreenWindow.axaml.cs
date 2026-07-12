#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;

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
        ApplyOptions(resize: false);
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        _opened = true;
        ResizeWindow(keepCenter: false);

        System.Drawing.Point location;
        if (_requestedLocation.HasValue)
        {
            int inset = GetOuterInset();
            location = new System.Drawing.Point(_requestedLocation.Value.X - inset, _requestedLocation.Value.Y - inset);
        }
        else
        {
            System.Drawing.Size size = new((int)Math.Ceiling(Width), (int)Math.Ceiling(Height));
            location = Helpers.GetPosition(Options.Placement, Options.PlacementOffset,
                CaptureHelpers.GetActiveScreenWorkingArea(), size);
        }

        Position = new PixelPoint(location.X, location.Y);
    }

    private void ApplyOptions(bool resize = true)
    {
        Topmost = Options.TopMost;
        Opacity = _minimized ? 1 : _imageOpacity / 100d;
        _imageBorder.Background = new SolidColorBrush(ToAvalonia(Options.BackgroundColor));
        _imageBorder.BorderBrush = new SolidColorBrush(ToAvalonia(Options.BorderColor));
        _imageBorder.BorderThickness = Options.Border ? new Thickness(Math.Clamp(Options.BorderSize, 1, 30)) : new Thickness(0);
        _imageBorder.Margin = Options.Shadow ? new Thickness(ShadowMargin) : new Thickness(0);
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

    private void ResizeWindow(bool keepCenter)
    {
        PixelPoint previousPosition = Position;
        Avalonia.Size previousSize = ClientSize;
        Avalonia.Size imageSize = GetDisplayedImageSize();
        double border = Options.Border ? Math.Clamp(Options.BorderSize, 1, 30) * 2 : 0;
        double shadow = Options.Shadow ? ShadowMargin * 2 : 0;
        double width = Math.Max(1, imageSize.Width + border + shadow);
        double height = Math.Max(1, imageSize.Height + border + shadow);

        _pinnedImage.Width = imageSize.Width;
        _pinnedImage.Height = imageSize.Height;
        Width = width;
        Height = height;
        MinWidth = width;
        MaxWidth = width;
        MinHeight = height;
        MaxHeight = height;

        if (keepCenter && previousSize.Width > 0 && previousSize.Height > 0)
        {
            Position = new PixelPoint(
                previousPosition.X + (int)Math.Round((previousSize.Width - width) / 2),
                previousPosition.Y + (int)Math.Round((previousSize.Height - height) / 2));
        }

        _scaleText.Text = $"{_imageScale}%";
        UpdateToolbarVisibility(IsPointerOver);
    }

    private Avalonia.Size GetDisplayedImageSize()
    {
        if (_minimized)
        {
            return new Avalonia.Size(Math.Max(1, Options.MinimizeSize.Width), Math.Max(1, Options.MinimizeSize.Height));
        }

        double scale = _imageScale / 100d;
        return new Avalonia.Size(Math.Max(1, Math.Round(_bitmap.PixelSize.Width * scale)),
            Math.Max(1, Math.Round(_bitmap.PixelSize.Height * scale)));
    }

    private int GetOuterInset() => (Options.Border ? Math.Clamp(Options.BorderSize, 1, 30) : 0) +
        (Options.Shadow ? ShadowMargin : 0);

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
                BeginMoveDrag(e);
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
