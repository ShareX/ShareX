#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.HelpersLib;

public partial class ImageViewerWindow : Window
{
    private readonly ImageViewerViewModel _viewModel = new();
    private bool _closeOnDeactivate;

    public ImageViewerWindow()
    {
        Initialize();
        Opened += OnOpened;
    }

    public ImageViewerWindow(string filePath)
    {
        Initialize();
        _closeOnDeactivate = _viewModel.LoadFile(filePath);
    }

    public ImageViewerWindow(IReadOnlyList<string> filePaths, int selectedIndex)
    {
        Initialize();
        _closeOnDeactivate = _viewModel.LoadFiles(filePaths, selectedIndex);
    }

    public ImageViewerWindow(byte[] imageData, string? displayName = null)
    {
        Initialize();
        _closeOnDeactivate = _viewModel.LoadEncodedImage(imageData, displayName);
    }

    private void Initialize()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        System.Drawing.Rectangle activeScreen = CaptureHelpers.GetActiveScreenBounds();
        WindowStartupLocation = WindowStartupLocation.Manual;
        Position = new PixelPoint(activeScreen.X, activeScreen.Y);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Title = Localization.Strings.ImageViewerWindow_Title;
        KeyDown += OnKeyDown;
        Deactivated += OnDeactivated;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        if (!await SelectImageAsync())
        {
            Close();
        }
    }

    private async Task<bool> SelectImageAsync()
    {
        _closeOnDeactivate = false;
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.ImageViewerWindow_Open_image,
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        string? filePath = files.FirstOrDefault()?.Path.LocalPath;
        bool loaded = !string.IsNullOrWhiteSpace(filePath) && _viewModel.LoadFile(filePath);
        _closeOnDeactivate = _viewModel.HasImage;
        Activate();
        Focus();
        return loaded;
    }

    private void OnPreviousClick(object? sender, RoutedEventArgs e) => _viewModel.Navigate(-1);
    private void OnNextClick(object? sender, RoutedEventArgs e) => _viewModel.Navigate(1);

    private void OnPreviewPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
    }

    private void OnPreviewPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Delta.Y > 0 && _viewModel.CanNavigateLeft)
        {
            _viewModel.Navigate(-1);
            e.Handled = true;
        }
        else if (e.Delta.Y < 0 && _viewModel.CanNavigateRight)
        {
            _viewModel.Navigate(1);
            e.Handled = true;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                _viewModel.Navigate(-1);
                e.Handled = true;
                break;
            case Key.Right:
                _viewModel.Navigate(1);
                e.Handled = true;
                break;
            case Key.Escape:
            case Key.Enter:
            case Key.Space:
                Close();
                e.Handled = true;
                break;
        }
    }

    private void OnDeactivated(object? sender, EventArgs e)
    {
        if (_closeOnDeactivate)
        {
            Close();
        }
    }
}
