#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DrawingColor = System.Drawing.Color;
using HelperGradientStop = ShareX.HelpersLib.GradientStop;

namespace ShareX.ImageEffectsLib;

public partial class GradientOptionsWindow : Window
{
    private static readonly LinearGradientMode[] GradientDirections = Enum.GetValues<LinearGradientMode>();
    private readonly GradientInfo _target;
    private readonly GradientInfo _gradient;
    private ComboBox _direction = null!;
    private StackPanel _stopsPanel = null!;
    private Image _gradientPreview = null!;

    public GradientOptionsWindow() : this(new GradientInfo())
    {
    }

    public GradientOptionsWindow(GradientInfo gradient)
    {
        _target = gradient;
        _gradient = gradient.Copy();
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _direction = this.FindControl<ComboBox>("DirectionComboBox")!;
        _stopsPanel = this.FindControl<StackPanel>("StopsPanel")!;
        _gradientPreview = this.FindControl<Image>("GradientPreview")!;
        _direction.ItemsSource = GradientDirections.Select(x => Helpers.GetProperName(x.ToString())).ToArray();
        _direction.SelectedIndex = Array.IndexOf(GradientDirections, _gradient.Type);
        _direction.SelectionChanged += (_, _) =>
        {
            if (_direction.SelectedIndex >= 0)
            {
                _gradient.Type = GradientDirections[_direction.SelectedIndex];
                UpdateGradientPreview();
            }
        };
        RebuildStops();
        Closed += (_, _) => (_gradientPreview.Source as Avalonia.Media.Imaging.Bitmap)?.Dispose();
    }

    private void RebuildStops()
    {
        _stopsPanel.Children.Clear();
        foreach (HelperGradientStop stop in _gradient.Colors.OrderBy(x => x.Location).ToArray())
        {
            Grid row = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 8 };
            Border swatch = new() { Width = 28, Height = 28, CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(ToAvalonia(stop.Color)) };
            Button colorButton = new() { Content = swatch, Width = 42, Padding = new Thickness(4) };
            Avalonia.Controls.ColorView picker = new()
            {
                MinWidth = 320,
                Color = ToAvalonia(stop.Color),
                IsAlphaVisible = true,
                IsColorPreviewVisible = true
            };
            Flyout flyout = new() { Content = picker };
            picker.PropertyChanged += (_, e) =>
            {
                if (e.Property == Avalonia.Controls.ColorView.ColorProperty)
                {
                    stop.Color = ToDrawing(picker.Color);
                    swatch.Background = new SolidColorBrush(picker.Color);
                    UpdateGradientPreview();
                }
            };
            colorButton.Flyout = flyout;
            row.Children.Add(colorButton);

            NumericUpDown location = new() { Minimum = 0, Maximum = 100, FormatString = "0'%'", Value = (decimal)stop.Location };
            location.ValueChanged += (_, _) =>
            {
                stop.Location = (float)(location.Value ?? 0);
                UpdateGradientPreview();
            };
            Grid.SetColumn(location, 1);
            row.Children.Add(location);

            Button remove = new() { Content = "Remove" };
            remove.Click += (_, _) => { _gradient.Colors.Remove(stop); RebuildStops(); };
            Grid.SetColumn(remove, 2);
            row.Children.Add(remove);
            _stopsPanel.Children.Add(row);
        }
        UpdateGradientPreview();
    }

    private void UpdateGradientPreview()
    {
        using System.Drawing.Bitmap drawingPreview = _gradient.CreateGradientPreview(320, 64, checkers: true);
        using MemoryStream stream = new();
        drawingPreview.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        Avalonia.Media.Imaging.Bitmap preview = new(stream);
        Avalonia.Media.Imaging.Bitmap? previous = _gradientPreview.Source as Avalonia.Media.Imaging.Bitmap;
        _gradientPreview.Source = preview;
        previous?.Dispose();
    }

    private void OnAddStopClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        float location = _gradient.Colors.Count == 0 ? 0 : Math.Min(100, _gradient.Colors.Max(x => x.Location) + 10);
        _gradient.Colors.Add(new HelperGradientStop(DrawingColor.White, location));
        RebuildStops();
    }

    private void OnOkClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _gradient.Sort();
        _target.Type = _gradient.Type;
        _target.Colors = _gradient.Colors.Copy();
        Close(true);
    }

    private void OnCancelClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(false);

    private static Avalonia.Media.Color ToAvalonia(DrawingColor color) => Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    private static DrawingColor ToDrawing(Avalonia.Media.Color color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
}
