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

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ShareX.HelpersLib;
using ShareX.Tools.Infrastructure;

namespace ShareX.Tools;

public sealed partial class ImageViewerViewModel : ViewModelBase, IDisposable
{
    private string[] _images = [];
    private int _currentImageIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    private Bitmap? _currentImage;

    [ObservableProperty]
    private string? _currentImageFilePath;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public bool SupportWrap { get; set; }
    public bool HasImage => CurrentImage != null;
    public bool CanNavigate => _images.Length > 1;
    public bool CanNavigateLeft => CanNavigate && (SupportWrap || _currentImageIndex > 0);
    public bool CanNavigateRight => CanNavigate && (SupportWrap || _currentImageIndex < _images.Length - 1);

    public bool LoadFile(string filePath)
    {
        if (!File.Exists(filePath) || !FileHelpers.IsImageFile(filePath))
        {
            return false;
        }

        string? folder = Path.GetDirectoryName(filePath);
        _images = !string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder)
            ? Directory.GetFiles(folder).Where(FileHelpers.IsImageFile).ToArray()
            : [filePath];

        _currentImageIndex = Array.FindIndex(_images,
            path => string.Equals(path, filePath, StringComparison.OrdinalIgnoreCase));
        if (_currentImageIndex < 0)
        {
            _images = [filePath];
            _currentImageIndex = 0;
        }

        return LoadCurrentImage();
    }

    public bool LoadFiles(IReadOnlyList<string> filePaths, int selectedIndex)
    {
        _images = filePaths.Where(path => File.Exists(path) && FileHelpers.IsImageFile(path)).ToArray();
        if (_images.Length == 0)
        {
            return false;
        }

        _currentImageIndex = Math.Clamp(selectedIndex, 0, _images.Length - 1);
        return LoadCurrentImage();
    }

    public bool LoadEncodedImage(byte[] imageData, string? displayName = null)
    {
        if (imageData.Length == 0)
        {
            return false;
        }

        try
        {
            using MemoryStream stream = new(imageData, writable: false);
            Bitmap image = new(stream);
            ReplaceImage(image);
            _images = [];
            _currentImageIndex = 0;
            CurrentImageFilePath = displayName;
            UpdateStatus();
            return true;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(ImageViewerViewModel), "Failed to load encoded image.", ex);
            return false;
        }
    }

    public void Navigate(int offset)
    {
        if (!CanNavigate)
        {
            return;
        }

        int nextIndex = _currentImageIndex + offset;
        if (SupportWrap)
        {
            nextIndex = (nextIndex + _images.Length) % _images.Length;
        }
        else
        {
            nextIndex = Math.Clamp(nextIndex, 0, _images.Length - 1);
        }

        if (nextIndex != _currentImageIndex)
        {
            _currentImageIndex = nextIndex;
            LoadCurrentImage();
        }
    }

    private bool LoadCurrentImage()
    {
        if (_images.Length == 0)
        {
            return false;
        }

        try
        {
            CurrentImageFilePath = _images[_currentImageIndex];
            ReplaceImage(new Bitmap(CurrentImageFilePath));
            UpdateStatus();
            return true;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(ImageViewerViewModel),
                $"Failed to load image '{CurrentImageFilePath}'.", ex);
            return false;
        }
    }

    private void ReplaceImage(Bitmap image)
    {
        CurrentImage?.Dispose();
        CurrentImage = image;
        NotifyNavigationChanged();
    }

    private void UpdateStatus()
    {
        List<string> parts = [];
        if (CanNavigate)
        {
            parts.Add($"{_currentImageIndex + 1} / {_images.Length}");
        }

        if (!string.IsNullOrWhiteSpace(CurrentImageFilePath))
        {
            string fileName = Path.GetFileName(CurrentImageFilePath);
            parts.Add(fileName.Length > 128 ? $"{fileName[..125]}..." : fileName);
        }

        if (CurrentImage != null)
        {
            parts.Add($"{CurrentImage.PixelSize.Width} × {CurrentImage.PixelSize.Height}");
        }

        StatusText = string.Join("  |  ", parts);
        NotifyNavigationChanged();
    }

    private void NotifyNavigationChanged()
    {
        OnPropertyChanged(nameof(CanNavigate));
        OnPropertyChanged(nameof(CanNavigateLeft));
        OnPropertyChanged(nameof(CanNavigateRight));
    }

    public void Dispose()
    {
        CurrentImage?.Dispose();
    }
}
