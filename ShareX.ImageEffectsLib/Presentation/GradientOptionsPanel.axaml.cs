#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ShareX.HelpersLib;
using System.Drawing.Drawing2D;
using DrawingColor = System.Drawing.Color;
using HelperGradientStop = ShareX.HelpersLib.GradientStop;

namespace ShareX.ImageEffectsLib;

public partial class GradientOptionsPanel : UserControl
{
    private static readonly LinearGradientMode[] GradientDirections = Enum.GetValues<LinearGradientMode>();
    private readonly GradientInfo _gradient;
    private readonly Action? _changed;
    private readonly Dictionary<HelperGradientStop, Control> _stopRows = [];
    private ComboBox _direction = null!;
    private StackPanel _stopsPanel = null!;

    public GradientOptionsPanel() : this(new GradientInfo())
    {
    }

    public GradientOptionsPanel(GradientInfo gradient, Action? changed = null)
    {
        _gradient = gradient;
        _changed = changed;
        AvaloniaXamlLoader.Load(this);
        _direction = this.FindControl<ComboBox>("DirectionComboBox")!;
        _stopsPanel = this.FindControl<StackPanel>("StopsPanel")!;
        _direction.ItemsSource = GradientDirections.Select(x => Helpers.GetProperName(x.ToString())).ToArray();
        _direction.SelectedIndex = Array.IndexOf(GradientDirections, _gradient.Type);
        _direction.SelectionChanged += (_, _) =>
        {
            if (_direction.SelectedIndex >= 0)
            {
                _gradient.Type = GradientDirections[_direction.SelectedIndex];
                NotifyChanged();
            }
        };
        RebuildStops();
    }

    private void RebuildStops()
    {
        _stopsPanel.Children.Clear();
        _stopRows.Clear();
        foreach (HelperGradientStop stop in _gradient.Colors.OrderBy(x => x.Location).ToArray())
        {
            Grid row = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 8 };
            Border swatch = new()
            {
                Width = 28,
                Height = 28,
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(ToAvalonia(stop.Color))
            };
            Button colorButton = new() { Content = swatch, Width = 42, Padding = new Thickness(4) };
            Avalonia.Controls.ColorView picker = new()
            {
                MinWidth = 320,
                Color = ToAvalonia(stop.Color),
                IsAlphaVisible = true,
                IsColorPreviewVisible = true
            };
            picker.PropertyChanged += (_, e) =>
            {
                if (e.Property == Avalonia.Controls.ColorView.ColorProperty)
                {
                    stop.Color = ToDrawing(picker.Color);
                    swatch.Background = new SolidColorBrush(picker.Color);
                    NotifyChanged();
                }
            };
            colorButton.Flyout = new Flyout { Content = picker };
            row.Children.Add(colorButton);

            NumericUpDown location = new()
            {
                Minimum = 0,
                Maximum = 100,
                FormatString = "0'%'",
                Value = (decimal)stop.Location
            };
            location.ValueChanged += (_, _) =>
            {
                stop.Location = (float)(location.Value ?? 0);
                ReorderStops();
                NotifyChanged();
            };
            Grid.SetColumn(location, 1);
            row.Children.Add(location);

            Button remove = new() { Content = "Remove" };
            remove.Click += (_, _) =>
            {
                _gradient.Colors.Remove(stop);
                RebuildStops();
                NotifyChanged();
            };
            Grid.SetColumn(remove, 2);
            row.Children.Add(remove);
            _stopsPanel.Children.Add(row);
            _stopRows.Add(stop, row);
        }
    }

    private void ReorderStops()
    {
        HelperGradientStop[] orderedStops = _gradient.Colors.OrderBy(x => x.Location).ToArray();
        _gradient.Colors.Clear();
        _gradient.Colors.AddRange(orderedStops);

        for (int targetIndex = 0; targetIndex < orderedStops.Length; targetIndex++)
        {
            Control row = _stopRows[orderedStops[targetIndex]];
            int currentIndex = _stopsPanel.Children.IndexOf(row);
            if (currentIndex != targetIndex)
            {
                _stopsPanel.Children.Move(currentIndex, targetIndex);
            }
        }
    }

    private void OnAddStopClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        float location = _gradient.Colors.Count == 0 ? 0 : Math.Min(100, _gradient.Colors.Max(x => x.Location) + 10);
        _gradient.Colors.Add(new HelperGradientStop(DrawingColor.White, location));
        RebuildStops();
        NotifyChanged();
    }

    private void NotifyChanged() => _changed?.Invoke();
    private static Avalonia.Media.Color ToAvalonia(DrawingColor color) => Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    private static DrawingColor ToDrawing(Avalonia.Media.Color color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
}
