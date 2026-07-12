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
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class AnalyzeImageWindow : Window
{
    private readonly AnalyzeImageViewModel _viewModel;
    private readonly AnalyzeImageRegionCaptureHandler _captureRegion;
    private readonly AnalyzeImageService _service = new();
    private readonly AIOptions _options;

    public AnalyzeImageWindow()
        : this(null, new AIOptions(), () => Task.FromResult<byte[]?>(null))
    {
    }

    public AnalyzeImageWindow(string? imagePath, AIOptions options,
        AnalyzeImageRegionCaptureHandler captureRegion,
        Action? playNotificationSound = null)
    {
        _options = options;
        _captureRegion = captureRegion;
        _viewModel = new AnalyzeImageViewModel(imagePath, options)
        {
            PlayNotificationSound = playNotificationSound
        };

        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectImageRequested = SelectImageAsync;
        _viewModel.SelectRegionRequested = SelectRegionAsync;
        _viewModel.CopyTextRequested = CopyTextAsync;
        _viewModel.EditOptionsRequested = EditOptionsAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Opened += OnOpened;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await _viewModel.InitializeAsync();
    }

    private async Task<string?> SelectImageAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select image",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    private async Task<byte[]?> SelectRegionAsync()
    {
        WindowState previousState = WindowState;
        try
        {
            WindowState = WindowState.Minimized;
            await Task.Delay(250);
            return await _captureRegion();
        }
        finally
        {
            WindowState = previousState;
            Activate();
        }
    }

    private async Task EditOptionsAsync()
    {
        AnalyzeImageOptionsWindow window = new(_options, _service);
        await window.ShowDialog<bool>(this);
    }

    private async Task CopyTextAsync(string text)
    {
        if (Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _viewModel.IsBusy || !e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.None
            : DragDropEffects.Copy;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        IStorageFile? file = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault();
        if (!_viewModel.IsBusy && file != null)
        {
            _viewModel.LoadImage(file.Path.LocalPath);
            e.Handled = true;
        }
    }

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed || _viewModel.GetImageData() is not { Length: > 0 } data)
        {
            return;
        }

        new ImageViewerWindow(data, _viewModel.ImageDescription).Show(this);
        e.Handled = true;
    }
}
