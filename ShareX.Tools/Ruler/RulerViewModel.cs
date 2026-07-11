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

using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using ShareX.Tools.Infrastructure;

namespace ShareX.Tools;

public sealed partial class RulerViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _positionText = "X: 0  Y: 0";

    [ObservableProperty]
    private string _sizeText = "0 × 0 px";

    [ObservableProperty]
    private string _detailsText = "Area: 0 px²  |  Perimeter: 0 px  |  Distance: 0 px  |  Angle: 0°";

    [ObservableProperty]
    private bool _hasMeasurement;

    public string ClipboardText => $"{PositionText}\n{SizeText}\n{DetailsText}";

    public void Update(Rect selection, double scaling, PixelPoint windowOrigin)
    {
        int x = windowOrigin.X + (int)Math.Round(selection.X * scaling);
        int y = windowOrigin.Y + (int)Math.Round(selection.Y * scaling);
        int width = Math.Max(0, (int)Math.Round(selection.Width * scaling));
        int height = Math.Max(0, (int)Math.Round(selection.Height * scaling));
        int right = x + width;
        int bottom = y + height;
        long area = (long)width * height;
        long perimeter = 2L * (width + height);
        double distance = Math.Sqrt((double)width * width + (double)height * height);
        double angle = Math.Atan2(height, width) * 180d / Math.PI;

        PositionText = $"X: {x}  Y: {y}  |  Right: {right}  Bottom: {bottom}";
        SizeText = $"{width} × {height} px";
        DetailsText = $"Area: {area:N0} px²  |  Perimeter: {perimeter:N0} px  |  Distance: {distance:0.00} px  |  Angle: {angle:0.00}°";
        HasMeasurement = width > 0 || height > 0;
    }

    public void Clear()
    {
        PositionText = "X: 0  Y: 0";
        SizeText = "0 × 0 px";
        DetailsText = "Area: 0 px²  |  Perimeter: 0 px  |  Distance: 0 px  |  Angle: 0°";
        HasMeasurement = false;
    }
}
