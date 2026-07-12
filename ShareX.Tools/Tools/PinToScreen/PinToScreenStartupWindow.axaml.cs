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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class PinToScreenStartupWindow : Window
{
    private readonly PinToScreenStartupViewModel _viewModel;

    public PinToScreenStartupWindow() : this(new PinToScreenServices
    {
        CaptureRegionAsync = () => Task.FromResult<PinToScreenSource?>(null),
        GetClipboardImageAsync = () => Task.FromResult<PinToScreenSource?>(null),
        SelectImageFileAsync = () => Task.FromResult<PinToScreenSource?>(null),
        CopyImage = _ => { }
    }, new PinToScreenOptions())
    {
    }

    public PinToScreenStartupWindow(PinToScreenServices services, PinToScreenOptions options)
    {
        _viewModel = new PinToScreenStartupViewModel(services);
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.CloseRequested = Close;
        _viewModel.RegionCaptureStarted = Hide;
        _viewModel.RegionCaptureFinished = () =>
        {
            if (IsVisible == false && _viewModel.HasError)
            {
                Show();
            }
        };
        _viewModel.SourceSelected = source =>
        {
            PinToScreenManager.Pin(source.ImageData, options, services.CopyImage, source.Location);
            services.ImagePinned?.Invoke();
            Close();
        };
    }
}
