#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareX.Tools;

public enum MonitorTestMode
{
    Grayscale,
    Rgb,
    Gradient,
    Pattern,
    Motion
}

public enum MonitorGradientDirection
{
    Horizontal,
    Vertical,
    ForwardDiagonal,
    BackwardDiagonal
}

public enum MonitorPattern
{
    HorizontalLines,
    VerticalLines,
    Checkerboard
}

public enum MonitorMotionDirection
{
    VerticalBars,
    HorizontalBars
}

public sealed partial class MonitorTestViewModel : ViewModelBase
{
    private bool _updatingRgb;

    public static IReadOnlyList<MonitorGradientDirection> GradientDirections { get; } = Enum.GetValues<MonitorGradientDirection>();
    public static IReadOnlyList<MonitorPattern> Patterns { get; } = Enum.GetValues<MonitorPattern>();
    public static IReadOnlyList<MonitorMotionDirection> MotionDirections { get; } = Enum.GetValues<MonitorMotionDirection>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGrayscale))]
    [NotifyPropertyChangedFor(nameof(IsRgb))]
    [NotifyPropertyChangedFor(nameof(IsGradient))]
    [NotifyPropertyChangedFor(nameof(IsPattern))]
    [NotifyPropertyChangedFor(nameof(IsMotion))]
    private MonitorTestMode _selectedMode;

    [ObservableProperty]
    private bool _controlsVisible = true;

    [ObservableProperty]
    private double _grayValue = 255;

    [ObservableProperty]
    private double _red = 255;

    [ObservableProperty]
    private double _green;

    [ObservableProperty]
    private double _blue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GradientStartBrush))]
    [NotifyPropertyChangedFor(nameof(GradientStartText))]
    private Color _gradientStart = Colors.White;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GradientEndBrush))]
    [NotifyPropertyChangedFor(nameof(GradientEndText))]
    private Color _gradientEnd = Colors.Black;

    [ObservableProperty]
    private MonitorGradientDirection _gradientDirection = MonitorGradientDirection.Vertical;

    [ObservableProperty]
    private MonitorPattern _selectedPattern;

    [ObservableProperty]
    private double _patternSize = 5;

    [ObservableProperty]
    private MonitorMotionDirection _motionDirection;

    [ObservableProperty]
    private double _motionSpeed = 500;

    public bool IsGrayscale => SelectedMode == MonitorTestMode.Grayscale;
    public bool IsRgb => SelectedMode == MonitorTestMode.Rgb;
    public bool IsGradient => SelectedMode == MonitorTestMode.Gradient;
    public bool IsPattern => SelectedMode == MonitorTestMode.Pattern;
    public bool IsMotion => SelectedMode == MonitorTestMode.Motion;

    public Color RgbColor
    {
        get => Color.FromRgb(ToByte(Red), ToByte(Green), ToByte(Blue));
        set
        {
            _updatingRgb = true;
            try
            {
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
            finally
            {
                _updatingRgb = false;
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(RgbBrush));
            OnPropertyChanged(nameof(RgbText));
        }
    }

    public IBrush RgbBrush => new SolidColorBrush(RgbColor);
    public string RgbText => $"#{RgbColor.R:X2}{RgbColor.G:X2}{RgbColor.B:X2}";
    public IBrush GradientStartBrush => new SolidColorBrush(GradientStart);
    public IBrush GradientEndBrush => new SolidColorBrush(GradientEnd);
    public string GradientStartText => $"#{GradientStart.R:X2}{GradientStart.G:X2}{GradientStart.B:X2}";
    public string GradientEndText => $"#{GradientEnd.R:X2}{GradientEnd.G:X2}{GradientEnd.B:X2}";

    partial void OnRedChanged(double value) => NotifyRgbChanged();
    partial void OnGreenChanged(double value) => NotifyRgbChanged();
    partial void OnBlueChanged(double value) => NotifyRgbChanged();

    public void SelectMode(MonitorTestMode mode)
    {
        SelectedMode = mode;
    }

    public void SelectRelativeMode(int offset)
    {
        int count = Enum.GetValues<MonitorTestMode>().Length;
        SelectedMode = (MonitorTestMode)(((int)SelectedMode + offset + count) % count);
    }

    private void NotifyRgbChanged()
    {
        if (!_updatingRgb)
        {
            OnPropertyChanged(nameof(RgbColor));
            OnPropertyChanged(nameof(RgbBrush));
            OnPropertyChanged(nameof(RgbText));
        }
    }

    private static byte ToByte(double value) => (byte)Math.Round(Math.Clamp(value, 0, 255));
}
