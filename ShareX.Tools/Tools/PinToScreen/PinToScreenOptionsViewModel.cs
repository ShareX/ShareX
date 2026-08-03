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
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace ShareX.Tools;

public sealed record PinPlacementChoice(string Name, ContentAlignment Value)
{
    public override string ToString() => Name;
}

public sealed partial class PinToScreenOptionsViewModel : ViewModelBase
{
    private readonly PinToScreenOptions _options;

    public ObservableCollection<PinPlacementChoice> Placements { get; } =
    [
        new(Localization.Strings.PinToScreenOptionsViewModel_Top_left, ContentAlignment.TopLeft), new(Localization.Strings.PinToScreenOptionsViewModel_Top_center, ContentAlignment.TopCenter),
        new(Localization.Strings.PinToScreenOptionsViewModel_Top_right, ContentAlignment.TopRight), new(Localization.Strings.PinToScreenOptionsViewModel_Middle_left, ContentAlignment.MiddleLeft),
        new(Localization.Strings.PinToScreenOptionsViewModel_Center, ContentAlignment.MiddleCenter), new(Localization.Strings.PinToScreenOptionsViewModel_Middle_right, ContentAlignment.MiddleRight),
        new(Localization.Strings.PinToScreenOptionsViewModel_Bottom_left, ContentAlignment.BottomLeft), new(Localization.Strings.PinToScreenOptionsViewModel_Bottom_center, ContentAlignment.BottomCenter),
        new(Localization.Strings.PinToScreenOptionsViewModel_Bottom_right, ContentAlignment.BottomRight)
    ];

    [ObservableProperty] private decimal _initialScale;
    [ObservableProperty] private decimal _scaleStep;
    [ObservableProperty] private bool _highQualityScale;
    [ObservableProperty] private decimal _initialOpacity;
    [ObservableProperty] private decimal _opacityStep;
    [ObservableProperty] private PinPlacementChoice? _selectedPlacement;
    [ObservableProperty] private decimal _placementOffset;
    [ObservableProperty] private bool _topMost;
    [ObservableProperty] private bool _keepCenterLocation;
    [ObservableProperty] private Color _backgroundColor;
    [ObservableProperty] private bool _shadow;
    [ObservableProperty] private bool _border;
    [ObservableProperty] private decimal _borderSize;
    [ObservableProperty] private Color _borderColor;
    [ObservableProperty] private decimal _minimizeWidth;
    [ObservableProperty] private decimal _minimizeHeight;

    public Action<bool>? CloseRequested { get; set; }
    public IBrush BackgroundColorBrush => new SolidColorBrush(BackgroundColor);
    public IBrush BorderColorBrush => new SolidColorBrush(BorderColor);

    public PinToScreenOptionsViewModel(PinToScreenOptions options)
    {
        _options = options;
        _initialScale = options.InitialScale;
        _scaleStep = options.ScaleStep;
        _highQualityScale = options.HighQualityScale;
        _initialOpacity = options.InitialOpacity;
        _opacityStep = options.OpacityStep;
        _selectedPlacement = Placements.First(x => x.Value == options.Placement);
        _placementOffset = options.PlacementOffset;
        _topMost = options.TopMost;
        _keepCenterLocation = options.KeepCenterLocation;
        _backgroundColor = ToAvalonia(options.BackgroundColor);
        _shadow = options.Shadow;
        _border = options.Border;
        _borderSize = options.BorderSize;
        _borderColor = ToAvalonia(options.BorderColor);
        _minimizeWidth = options.MinimizeSize.Width;
        _minimizeHeight = options.MinimizeSize.Height;
    }

    [RelayCommand]
    private void Save()
    {
        _options.InitialScale = (int)InitialScale;
        _options.ScaleStep = (int)ScaleStep;
        _options.HighQualityScale = HighQualityScale;
        _options.InitialOpacity = (int)InitialOpacity;
        _options.OpacityStep = (int)OpacityStep;
        _options.Placement = SelectedPlacement?.Value ?? ContentAlignment.BottomRight;
        _options.PlacementOffset = (int)PlacementOffset;
        _options.TopMost = TopMost;
        _options.KeepCenterLocation = KeepCenterLocation;
        _options.BackgroundColor = ToDrawing(BackgroundColor);
        _options.Shadow = Shadow;
        _options.Border = Border;
        _options.BorderSize = (int)BorderSize;
        _options.BorderColor = ToDrawing(BorderColor);
        _options.MinimizeSize = new System.Drawing.Size((int)MinimizeWidth, (int)MinimizeHeight);
        CloseRequested?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(false);

    partial void OnBackgroundColorChanged(Color value) => OnPropertyChanged(nameof(BackgroundColorBrush));
    partial void OnBorderColorChanged(Color value) => OnPropertyChanged(nameof(BorderColorBrush));

    private static Color ToAvalonia(System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
    private static System.Drawing.Color ToDrawing(Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
}
