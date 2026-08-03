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

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;

namespace ShareX.Tools;

public sealed record VideoThumbnailOutputChoice(string Name, ThumbnailLocationType Value);
public sealed record VideoThumbnailFormatChoice(string Name, EImageFormat Value);

public sealed partial class VideoThumbnailerViewModel : ViewModelBase
{
    private readonly string _ffmpegPath;
    private readonly VideoThumbnailOptions _options;
    private readonly Action<IReadOnlyList<VideoThumbnailInfo>>? _thumbnailsTaken;

    public IReadOnlyList<VideoThumbnailOutputChoice> OutputLocations { get; } =
    [
        new(Localization.Strings.VideoThumbnailerViewModel_Default_screenshots_folder, ThumbnailLocationType.DefaultFolder),
        new(Localization.Strings.VideoThumbnailerViewModel_Same_folder_as_video, ThumbnailLocationType.ParentFolder),
        new(Localization.Strings.VideoThumbnailerViewModel_Custom_folder, ThumbnailLocationType.CustomFolder)
    ];

    public IReadOnlyList<VideoThumbnailFormatChoice> ImageFormats { get; } = Enum.GetValues<EImageFormat>()
        .Select(x => new VideoThumbnailFormatChoice(x.GetDescription().ToUpperInvariant(), x))
        .ToArray();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasVideo))]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    private string _videoPath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCustomOutput))]
    private VideoThumbnailOutputChoice _selectedOutputLocation;

    [ObservableProperty] private string _customOutputDirectory;
    [ObservableProperty] private VideoThumbnailFormatChoice _selectedImageFormat;
    [ObservableProperty] private decimal _thumbnailCount;
    [ObservableProperty] private string _filenameSuffix;
    [ObservableProperty] private decimal _maxThumbnailWidth;
    [ObservableProperty] private bool _randomFrame;
    [ObservableProperty] private bool _uploadThumbnails;
    [ObservableProperty] private bool _openDirectory;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CombinedOptionsEnabled))]
    private bool _combineScreenshots;

    [ObservableProperty] private bool _keepScreenshots;
    [ObservableProperty] private decimal _columnCount;
    [ObservableProperty] private decimal _padding;
    [ObservableProperty] private decimal _spacing;
    [ObservableProperty] private bool _addVideoInfo;
    [ObservableProperty] private bool _addTimestamp;
    [ObservableProperty] private bool _drawShadow;
    [ObservableProperty] private bool _drawBorder;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    private bool _isBusy;

    [ObservableProperty] private decimal _progressValue;
    [ObservableProperty] private decimal _progressMaximum = 1;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public Func<Task<string?>>? SelectVideoRequested { get; set; }
    public Func<string?, Task<string?>>? SelectOutputFolderRequested { get; set; }

    public bool HasVideo => !string.IsNullOrWhiteSpace(VideoPath) && File.Exists(VideoPath);
    public bool IsIdle => !IsBusy;
    public bool CanStart => HasVideo && File.Exists(_ffmpegPath) && !IsBusy;
    public bool IsCustomOutput => SelectedOutputLocation.Value == ThumbnailLocationType.CustomFolder;
    public bool CombinedOptionsEnabled => CombineScreenshots && !IsBusy;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public VideoThumbnailerViewModel(string ffmpegPath, VideoThumbnailOptions options,
        Action<IReadOnlyList<VideoThumbnailInfo>>? thumbnailsTaken = null)
    {
        _ffmpegPath = ffmpegPath;
        _options = options;
        _thumbnailsTaken = thumbnailsTaken;

        _videoPath = options.LastVideoPath ?? string.Empty;
        _selectedOutputLocation = OutputLocations.First(x => x.Value == options.OutputLocation);
        _customOutputDirectory = options.CustomOutputDirectory ?? string.Empty;
        _selectedImageFormat = ImageFormats.FirstOrDefault(x => x.Value == options.ImageFormat) ?? ImageFormats[0];
        _thumbnailCount = Math.Max(options.ThumbnailCount, 1);
        _filenameSuffix = options.FilenameSuffix ?? string.Empty;
        _maxThumbnailWidth = Math.Max(options.MaxThumbnailWidth, 0);
        _randomFrame = options.RandomFrame;
        _uploadThumbnails = options.UploadThumbnails;
        _openDirectory = options.OpenDirectory;
        _combineScreenshots = options.CombineScreenshots;
        _keepScreenshots = options.KeepScreenshots;
        _columnCount = Math.Max(options.ColumnCount, 1);
        _padding = Math.Max(options.Padding, 0);
        _spacing = Math.Max(options.Spacing, 0);
        _addVideoInfo = options.AddVideoInfo;
        _addTimestamp = options.AddTimestamp;
        _drawShadow = options.DrawShadow;
        _drawBorder = options.DrawBorder;
    }

    partial void OnVideoPathChanged(string value)
    {
        _options.LastVideoPath = value;
        ErrorMessage = string.Empty;
    }

    partial void OnSelectedOutputLocationChanged(VideoThumbnailOutputChoice value)
    {
        _options.OutputLocation = value.Value;
    }

    partial void OnCustomOutputDirectoryChanged(string value) => _options.CustomOutputDirectory = value;
    partial void OnSelectedImageFormatChanged(VideoThumbnailFormatChoice value) => _options.ImageFormat = value.Value;
    partial void OnThumbnailCountChanged(decimal value) => _options.ThumbnailCount = Math.Max((int)value, 1);
    partial void OnFilenameSuffixChanged(string value) => _options.FilenameSuffix = value;
    partial void OnMaxThumbnailWidthChanged(decimal value) => _options.MaxThumbnailWidth = Math.Max((int)value, 0);
    partial void OnRandomFrameChanged(bool value) => _options.RandomFrame = value;
    partial void OnUploadThumbnailsChanged(bool value) => _options.UploadThumbnails = value;
    partial void OnOpenDirectoryChanged(bool value) => _options.OpenDirectory = value;
    partial void OnCombineScreenshotsChanged(bool value) => _options.CombineScreenshots = value;
    partial void OnKeepScreenshotsChanged(bool value) => _options.KeepScreenshots = value;
    partial void OnColumnCountChanged(decimal value) => _options.ColumnCount = Math.Max((int)value, 1);
    partial void OnPaddingChanged(decimal value) => _options.Padding = Math.Max((int)value, 0);
    partial void OnSpacingChanged(decimal value) => _options.Spacing = Math.Max((int)value, 0);
    partial void OnAddVideoInfoChanged(bool value) => _options.AddVideoInfo = value;
    partial void OnAddTimestampChanged(bool value) => _options.AddTimestamp = value;
    partial void OnDrawShadowChanged(bool value) => _options.DrawShadow = value;
    partial void OnDrawBorderChanged(bool value) => _options.DrawBorder = value;
    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    [RelayCommand]
    private async Task SelectVideoAsync()
    {
        if (!IsBusy && SelectVideoRequested != null && await SelectVideoRequested() is { } filePath)
        {
            VideoPath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectOutputFolderAsync()
    {
        if (!IsBusy && SelectOutputFolderRequested != null &&
            await SelectOutputFolderRequested(CustomOutputDirectory) is { } folderPath)
        {
            CustomOutputDirectory = folderPath;
        }
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (!CanStart)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        ProgressValue = 0;
        ProgressMaximum = Math.Max(ThumbnailCount, 1);

        try
        {
            IReadOnlyList<VideoThumbnailInfo> thumbnails = await Task.Run(() =>
            {
                VideoThumbnailer thumbnailer = new(_ffmpegPath, _options);
                thumbnailer.ProgressChanged += (current, length) => Dispatcher.UIThread.Post(() =>
                {
                    ProgressMaximum = Math.Max(length, 1);
                    ProgressValue = current;
                });
                return thumbnailer.TakeThumbnails(VideoPath) ?? [];
            });

            if (thumbnails.Count > 0)
            {
                _thumbnailsTaken?.Invoke(thumbnails);
            }
            else
            {
                ErrorMessage = Localization.Strings.VideoThumbnailerViewModel_No_thumbnails;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ToolsDiagnostics.ReportWarning(nameof(VideoThumbnailerViewModel), "Failed to create video thumbnails.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void LoadVideo(string filePath)
    {
        if (!IsBusy && !string.IsNullOrWhiteSpace(filePath))
        {
            VideoPath = filePath;
        }
    }
}
