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
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class OCRWindow : Window
{
    private readonly OCRViewModel _viewModel;
    private readonly OCRRegionCaptureHandler _selectRegion;

    public string Result => _viewModel.Result;

    public OCRWindow()
        : this([], [], new OCRWindowOptions(), (_, _, _, _) => Task.FromResult(string.Empty), () => Task.FromResult<byte[]?>(null))
    {
    }

    public OCRWindow(
        byte[] imageData,
        IEnumerable<OCRLanguageOption> languages,
        OCRWindowOptions options,
        OCRRecognitionHandler recognize,
        OCRRegionCaptureHandler selectRegion,
        Action<OCRWindowOptions>? optionsChanged = null,
        Action? openHelp = null)
    {
        _selectRegion = selectRegion;
        _viewModel = new OCRViewModel(imageData, languages, options, recognize, optionsChanged, openHelp);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectRegionRequested = SelectRegionAsync;
        _viewModel.SelectImageRequested = SelectImageAsync;
        _viewModel.CopyTextRequested = CopyTextAsync;
        _viewModel.CloseRequested = Close;
        Opened += OnOpened;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await _viewModel.StartAsync();
    }

    private async Task<byte[]?> SelectRegionAsync()
    {
        WindowState previousState = WindowState;

        try
        {
            WindowState = WindowState.Minimized;
            await Task.Delay(250);
            return await _selectRegion();
        }
        finally
        {
            WindowState = previousState;
            Activate();
        }
    }

    private async Task CopyTextAsync(string text)
    {
        if (Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select image",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        if (files.Count == 0)
        {
            return null;
        }

        await using Stream stream = await files[0].OpenReadAsync();
        using MemoryStream memoryStream = new();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
