#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using ShareX.Platform;

namespace ShareX.Tools;

public sealed record VideoThumbnailOutputChoice(string Name, ThumbnailLocationType Value);
public sealed record VideoThumbnailFormatChoice(string Name, EImageFormat Value);

public sealed partial class VideoThumbnailerViewModel : ViewModelBase, IDisposable
{
    private readonly string _ffmpegPath;
    private readonly VideoThumbnailOptions _options;
    private readonly Action<IReadOnlyList<VideoThumbnailInfo>>? _thumbnailsTaken;
    private readonly Func<FeatureSupport> _getSupport;
    private readonly Func<Task<IReadOnlyList<VideoThumbnailInfo>>>? _takeThumbnails;
    private bool _closed;

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
    public FeatureSupport Support => FileMediaFeatureSupport.ForUI(_getSupport());
    public string? SupportReason => _closed ? null : Support.Reason;
    public bool IsIdle => !_closed && !IsBusy && Support.IsSupported;
    public bool CanSelect => IsIdle;
    public bool CanStart => HasVideo && File.Exists(_ffmpegPath) && IsIdle;
    public bool IsCustomOutput => SelectedOutputLocation.Value == ThumbnailLocationType.CustomFolder;
    public bool CombinedOptionsEnabled => CombineScreenshots && IsIdle;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public VideoThumbnailerViewModel(string ffmpegPath, VideoThumbnailOptions options,
        Action<IReadOnlyList<VideoThumbnailInfo>>? thumbnailsTaken = null,
        Func<FeatureSupport>? getSupport = null)
        : this(ffmpegPath, options, thumbnailsTaken, getSupport, null)
    {
    }

    internal VideoThumbnailerViewModel(string ffmpegPath, VideoThumbnailOptions options,
        Action<IReadOnlyList<VideoThumbnailInfo>>? thumbnailsTaken, Func<FeatureSupport>? getSupport,
        Func<Task<IReadOnlyList<VideoThumbnailInfo>>>? takeThumbnails)
    {
        _ffmpegPath = ffmpegPath;
        _options = options;
        _thumbnailsTaken = thumbnailsTaken;
        _getSupport = getSupport ?? (() => FileMediaFeatureSupport.Get(ffmpegPath));
        _takeThumbnails = takeThumbnails;

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
        if (!IsIdle) return;
        _options.LastVideoPath = value;
        ErrorMessage = string.Empty;
    }

    partial void OnSelectedOutputLocationChanged(VideoThumbnailOutputChoice value)
    {
        if (!IsIdle) return;
        _options.OutputLocation = value.Value;
    }

    partial void OnCustomOutputDirectoryChanged(string value) { if (IsIdle) _options.CustomOutputDirectory = value; }
    partial void OnSelectedImageFormatChanged(VideoThumbnailFormatChoice value) { if (IsIdle) _options.ImageFormat = value.Value; }
    partial void OnThumbnailCountChanged(decimal value) { if (IsIdle) _options.ThumbnailCount = Math.Max((int)value, 1); }
    partial void OnFilenameSuffixChanged(string value) { if (IsIdle) _options.FilenameSuffix = value; }
    partial void OnMaxThumbnailWidthChanged(decimal value) { if (IsIdle) _options.MaxThumbnailWidth = Math.Max((int)value, 0); }
    partial void OnRandomFrameChanged(bool value) { if (IsIdle) _options.RandomFrame = value; }
    partial void OnUploadThumbnailsChanged(bool value) { if (IsIdle) _options.UploadThumbnails = value; }
    partial void OnOpenDirectoryChanged(bool value) { if (IsIdle) _options.OpenDirectory = value; }
    partial void OnCombineScreenshotsChanged(bool value) { if (IsIdle) _options.CombineScreenshots = value; }
    partial void OnKeepScreenshotsChanged(bool value) { if (IsIdle) _options.KeepScreenshots = value; }
    partial void OnColumnCountChanged(decimal value) { if (IsIdle) _options.ColumnCount = Math.Max((int)value, 1); }
    partial void OnPaddingChanged(decimal value) { if (IsIdle) _options.Padding = Math.Max((int)value, 0); }
    partial void OnSpacingChanged(decimal value) { if (IsIdle) _options.Spacing = Math.Max((int)value, 0); }
    partial void OnAddVideoInfoChanged(bool value) { if (IsIdle) _options.AddVideoInfo = value; }
    partial void OnAddTimestampChanged(bool value) { if (IsIdle) _options.AddTimestamp = value; }
    partial void OnDrawShadowChanged(bool value) { if (IsIdle) _options.DrawShadow = value; }
    partial void OnDrawBorderChanged(bool value) { if (IsIdle) _options.DrawBorder = value; }
    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private async Task SelectVideoAsync()
    {
        if (CheckSupport() && IsIdle && SelectVideoRequested != null &&
            await SelectVideoRequested() is { } filePath && CheckSupport() && IsIdle)
        {
            VideoPath = filePath;
        }
    }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private async Task SelectOutputFolderAsync()
    {
        if (CheckSupport() && IsIdle && SelectOutputFolderRequested != null &&
            await SelectOutputFolderRequested(CustomOutputDirectory) is { } folderPath && CheckSupport() && IsIdle)
        {
            CustomOutputDirectory = folderPath;
        }
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        if (!CheckSupport() || !CanStart)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        ProgressValue = 0;
        ProgressMaximum = Math.Max(ThumbnailCount, 1);

        try
        {
            if (_closed || !CheckSupport()) return;
            IReadOnlyList<VideoThumbnailInfo> thumbnails = _takeThumbnails != null
                ? await _takeThumbnails()
                : await Task.Run(() =>
            {
                if (_closed || !Support.IsSupported) return (IReadOnlyList<VideoThumbnailInfo>)Array.Empty<VideoThumbnailInfo>();
                VideoThumbnailer thumbnailer = new(_ffmpegPath, _options);
                thumbnailer.ProgressChanged += (current, length) => Dispatcher.UIThread.Post(() =>
                {
                    if (_closed) return;
                    ProgressMaximum = Math.Max(length, 1);
                    ProgressValue = current;
                });
                return thumbnailer.TakeThumbnails(VideoPath) ?? [];
            });

            if (thumbnails.Count > 0)
            {
                _thumbnailsTaken?.Invoke(thumbnails);
            }
            else if (!_closed)
            {
                ErrorMessage = Support.IsSupported
                    ? Localization.Strings.VideoThumbnailerViewModel_No_thumbnails
                    : SupportReason!;
            }
        }
        catch (Exception ex)
        {
            if (_closed) return;
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
        if (CheckSupport() && IsIdle && !string.IsNullOrWhiteSpace(filePath))
        {
            VideoPath = filePath;
        }
    }

    partial void OnIsBusyChanged(bool value) => NotifySupport();

    private bool CheckSupport()
    {
        if (_closed) return false;
        FeatureSupport support = Support;
        NotifySupport();
        if (!support.IsSupported && !_closed) ErrorMessage = support.Reason!;
        return support.IsSupported;
    }

    private void NotifySupport()
    {
        OnPropertyChanged(nameof(SupportReason));
        OnPropertyChanged(nameof(IsIdle));
        OnPropertyChanged(nameof(CanSelect));
        OnPropertyChanged(nameof(CanStart));
        OnPropertyChanged(nameof(CombinedOptionsEnabled));
        SelectVideoCommand.NotifyCanExecuteChanged();
        SelectOutputFolderCommand.NotifyCanExecuteChanged();
        StartCommand.NotifyCanExecuteChanged();
    }

    public void Dispose()
    {
        if (_closed) return;
        _closed = true;
        NotifySupport();
        // Accepted workers own their engine and output callback; only new commands and late UI updates end here.
    }
}
