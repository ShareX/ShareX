#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShareX.HelpersLib;

public sealed class ImageViewerViewModel : INotifyPropertyChanged, IDisposable
{
    private string[] _images = [];
    private int _currentImageIndex;
    private Bitmap? _currentImage;
    private string? _currentImageFilePath;
    private string _statusText = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Bitmap? CurrentImage
    {
        get => _currentImage;
        private set
        {
            if (ReferenceEquals(_currentImage, value))
            {
                return;
            }

            _currentImage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasImage));
        }
    }

    public string? CurrentImageFilePath
    {
        get => _currentImageFilePath;
        private set
        {
            if (_currentImageFilePath == value)
            {
                return;
            }

            _currentImageFilePath = value;
            OnPropertyChanged();
        }
    }

    public string StatusText
    {
        get => _statusText;
        private set
        {
            if (_statusText == value)
            {
                return;
            }

            _statusText = value;
            OnPropertyChanged();
        }
    }

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
        string? selectedPath = selectedIndex >= 0 && selectedIndex < filePaths.Count
            ? filePaths[selectedIndex]
            : null;

        _images = filePaths
            .Where(path => File.Exists(path) && FileHelpers.IsImageFile(path))
            .ToArray();
        if (_images.Length == 0)
        {
            return false;
        }

        _currentImageIndex = selectedPath == null
            ? 0
            : Array.FindIndex(_images,
                path => string.Equals(path, selectedPath, StringComparison.OrdinalIgnoreCase));
        if (_currentImageIndex < 0)
        {
            _currentImageIndex = Math.Clamp(selectedIndex, 0, _images.Length - 1);
        }

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
            ReplaceImage(new Bitmap(stream));
            _images = [];
            _currentImageIndex = 0;
            CurrentImageFilePath = displayName;
            UpdateStatus();
            return true;
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception, "Failed to load image viewer data.");
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
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception,
                $"Failed to load image '{CurrentImageFilePath}'.");
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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void Dispose()
    {
        CurrentImage?.Dispose();
        CurrentImage = null;
    }
}
