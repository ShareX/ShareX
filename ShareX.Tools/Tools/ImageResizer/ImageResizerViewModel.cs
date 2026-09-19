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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;
using System.Drawing;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.Tools;

public sealed partial class ImageResizerViewModel : ViewModelBase, IDisposable
{
    private readonly HashSet<string> _selectedImages = [];
    private CancellationTokenSource? _previewCancellationTokenSource;
    private int _previewVersion;
    private bool _isDisposed;

    public ObservableCollection<string> Images { get; } = [];

    [ObservableProperty]
    private string? _selectedImage;

    [ObservableProperty]
    private decimal _width = 1920;

    [ObservableProperty]
    private decimal _height = 1080;

    [ObservableProperty]
    private int _selectedResizeModeIndex = (int)ImageResizeMode.Fit;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsJpeg))]
    private int _selectedOutputFormatIndex;

    [ObservableProperty]
    private decimal _quality = 90;

    [ObservableProperty]
    private string _outputFolderPath = string.Empty;

    [ObservableProperty]
    private string _outputFileName = "$filename_resized";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private AvaloniaBitmap? _previewImage;

    [ObservableProperty]
    private bool _isPreviewLoading;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessage))]
    private string _message = string.Empty;

    public Func<Task<IReadOnlyList<string>?>>? SelectFilesRequested { get; set; }
    public Func<Task<string?>>? SelectOutputFolderRequested { get; set; }

    public IReadOnlyList<string> ResizeModeOptions { get; } =
    [
        Localization.Strings.ImageResizerWindow_Fill,
        Localization.Strings.ImageResizerWindow_Fit,
        Localization.Strings.ImageResizerWindow_Stretch
    ];

    public IReadOnlyList<string> OutputFormatOptions { get; } = ["PNG", "JPEG"];

    public bool HasImages => Images.Count > 0;
    public bool HasPreview => PreviewImage != null;
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);
    public bool IsJpeg => SelectedOutputFormatIndex == (int)ImageResizeOutputFormat.Jpeg;
    public bool CanRemove => _selectedImages.Count > 0 || SelectedImage != null;
    public bool CanResize => !IsBusy && HasImages && Width > 0 && Height > 0 &&
        Directory.Exists(OutputFolderPath) && !string.IsNullOrWhiteSpace(OutputFileName);
    public string ImageCountText => string.Format(Images.Count == 1
        ? Localization.Strings.ImageThumbnailerViewModel_One_image
        : Localization.Strings.ImageThumbnailerViewModel_Image_count, Images.Count);
    public string PreviewFilePath => SelectedImage ?? Images.FirstOrDefault() ?? string.Empty;
    public string PreviewSizeText => $"{(int)Width} × {(int)Height}";

    [RelayCommand]
    private async Task AddAsync()
    {
        if (SelectFilesRequested != null)
        {
            AddFiles(await SelectFilesRequested());
        }
    }

    public void AddFiles(IEnumerable<string>? files)
    {
        if (files == null)
        {
            return;
        }

        foreach (string file in files.Where(File.Exists))
        {
            Images.Add(file);
            if (string.IsNullOrWhiteSpace(OutputFolderPath))
            {
                OutputFolderPath = Path.GetDirectoryName(file) ?? string.Empty;
            }
        }

        if (SelectedImage == null)
        {
            SelectedImage = Images.FirstOrDefault();
        }

        NotifyCollectionChanged();
    }

    public void SetSelectedImages(IEnumerable<string> files)
    {
        _selectedImages.Clear();
        foreach (string file in files)
        {
            _selectedImages.Add(file);
        }
        OnPropertyChanged(nameof(CanRemove));
    }

    [RelayCommand]
    private void Remove()
    {
        string[] files = _selectedImages.Count > 0
            ? _selectedImages.ToArray()
            : SelectedImage == null ? [] : [SelectedImage];

        foreach (string file in files)
        {
            Images.Remove(file);
        }

        _selectedImages.Clear();
        SelectedImage = Images.FirstOrDefault();
        NotifyCollectionChanged();
    }

    [RelayCommand]
    private async Task BrowseOutputFolderAsync()
    {
        if (SelectOutputFolderRequested == null)
        {
            return;
        }

        string? folder = await SelectOutputFolderRequested();
        if (!string.IsNullOrWhiteSpace(folder))
        {
            OutputFolderPath = folder;
        }
    }

    [RelayCommand]
    private async Task ResizeAsync()
    {
        if (!CanResize)
        {
            return;
        }

        string[] imageFiles = Images.ToArray();
        int width = (int)Width;
        int height = (int)Height;
        ImageResizeMode mode = GetResizeMode();
        ImageResizeOutputFormat format = GetOutputFormat();
        int quality = (int)Quality;
        string outputFolderPath = OutputFolderPath;
        string outputFileName = OutputFileName;

        IsBusy = true;
        Message = string.Empty;
        try
        {
            List<string> outputFiles = await Task.Run(() => ResizeImages(imageFiles, width, height,
                mode, format, quality, outputFolderPath, outputFileName));
            if (outputFiles.Count > 0)
            {
                FileHelpers.OpenFolderWithFile(outputFiles[0]);
            }
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(ImageResizerViewModel), "Failed to resize images.", ex);
            Message = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedImageChanged(string? value)
    {
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(PreviewFilePath));
        QueuePreviewRefresh();
    }
    partial void OnWidthChanged(decimal value) => NotifyOptionsChanged();
    partial void OnHeightChanged(decimal value) => NotifyOptionsChanged();
    partial void OnSelectedResizeModeIndexChanged(int value) => NotifyOptionsChanged();
    partial void OnSelectedOutputFormatIndexChanged(int value) => NotifyOptionsChanged();
    partial void OnQualityChanged(decimal value) => NotifyOptionsChanged();
    partial void OnOutputFolderPathChanged(string value) => NotifyStateChanged();
    partial void OnOutputFileNameChanged(string value) => NotifyStateChanged();
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(CanResize));

    private void NotifyCollectionChanged()
    {
        OnPropertyChanged(nameof(HasImages));
        OnPropertyChanged(nameof(ImageCountText));
        OnPropertyChanged(nameof(PreviewFilePath));
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(CanResize));
        Message = string.Empty;
        QueuePreviewRefresh();
    }

    private void NotifyOptionsChanged()
    {
        OnPropertyChanged(nameof(CanResize));
        OnPropertyChanged(nameof(PreviewSizeText));
        Message = string.Empty;
        QueuePreviewRefresh();
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanResize));
        Message = string.Empty;
    }

    private void QueuePreviewRefresh()
    {
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();
        _previewCancellationTokenSource = new CancellationTokenSource();
        _ = RefreshPreviewAsync(_previewCancellationTokenSource.Token);
    }

    private async Task RefreshPreviewAsync(CancellationToken cancellationToken)
    {
        int version = ++_previewVersion;
        string? filePath = SelectedImage ?? Images.FirstOrDefault();
        if (_isDisposed || string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) || Width < 1 || Height < 1)
        {
            ReplacePreview(null);
            IsPreviewLoading = false;
            return;
        }

        IsPreviewLoading = true;
        try
        {
            await Task.Delay(100, cancellationToken);
            int width = (int)Width;
            int height = (int)Height;
            ImageResizeMode mode = GetResizeMode();
            ImageResizeOutputFormat format = GetOutputFormat();
            int quality = (int)Quality;
            byte[] previewBytes = await Task.Run(() => ImageResizerService.CreatePreview(filePath,
                width, height, mode, format, quality), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            AvaloniaBitmap? preview = null;
            if (previewBytes.Length > 0)
            {
                using MemoryStream stream = new(previewBytes, writable: false);
                preview = new AvaloniaBitmap(stream);
            }

            if (version == _previewVersion && !_isDisposed)
            {
                ReplacePreview(preview);
            }
            else
            {
                preview?.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            if (version == _previewVersion && !_isDisposed)
            {
                ReplacePreview(null);
                ToolsDiagnostics.ReportWarning(nameof(ImageResizerViewModel), "Failed to create resize preview.", ex);
            }
        }
        finally
        {
            if (version == _previewVersion && !_isDisposed)
            {
                IsPreviewLoading = false;
            }
        }
    }

    private void ReplacePreview(AvaloniaBitmap? preview)
    {
        PreviewImage?.Dispose();
        PreviewImage = preview;
    }

    private ImageResizeMode GetResizeMode() =>
        (ImageResizeMode)Math.Clamp(SelectedResizeModeIndex, 0, 2);

    private ImageResizeOutputFormat GetOutputFormat() =>
        (ImageResizeOutputFormat)Math.Clamp(SelectedOutputFormatIndex, 0, 1);

    private static List<string> ResizeImages(IEnumerable<string> imageFiles, int width, int height,
        ImageResizeMode mode, ImageResizeOutputFormat format, int quality, string outputFolderPath,
        string outputFileName)
    {
        List<string> outputFiles = [];
        string extension = format == ImageResizeOutputFormat.Jpeg ? "jpg" : "png";

        foreach (string filePath in imageFiles)
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            using Bitmap? source = ImageHelpers.LoadImage(filePath);
            if (source == null)
            {
                continue;
            }

            using Bitmap output = ImageResizerService.Resize(source, width, height, mode);
            string sourceName = Path.GetFileNameWithoutExtension(filePath);
            string outputPath = Path.Combine(outputFolderPath,
                outputFileName.Replace("$filename", sourceName, StringComparison.Ordinal));
            outputPath = Path.ChangeExtension(outputPath, extension);
            ImageResizerService.Save(output, outputPath, format, quality);
            outputFiles.Add(outputPath);
        }

        return outputFiles;
    }

    public void Dispose()
    {
        _isDisposed = true;
        _previewVersion++;
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();
        _previewCancellationTokenSource = null;
        ReplacePreview(null);
    }
}
