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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;

namespace ShareX.Tools;

public sealed record VideoConverterCodecItem(VideoConverterCodec Codec, string DisplayName);

public sealed partial class VideoConverterViewModel : ViewModelBase, IDisposable
{
    public static IReadOnlyList<VideoConverterCodecItem> Codecs { get; } =
    [
        new(VideoConverterCodec.X264, "H.264 / x264"),
        new(VideoConverterCodec.X265, "H.265 / x265"),
        new(VideoConverterCodec.H264Nvenc, "H.264 / NVENC"),
        new(VideoConverterCodec.HevcNvenc, "HEVC / NVENC"),
        new(VideoConverterCodec.H264Amf, "H.264 / AMF"),
        new(VideoConverterCodec.HevcAmf, "HEVC / AMF"),
        new(VideoConverterCodec.H264Qsv, "H.264 / Quick Sync"),
        new(VideoConverterCodec.HevcQsv, "HEVC / Quick Sync"),
        new(VideoConverterCodec.Vp8, "VP8"),
        new(VideoConverterCodec.Vp9, "VP9"),
        new(VideoConverterCodec.Av1, "AV1"),
        new(VideoConverterCodec.Xvid, "MPEG-4 / Xvid"),
        new(VideoConverterCodec.Gif, "GIF"),
        new(VideoConverterCodec.Webp, "WebP"),
        new(VideoConverterCodec.Apng, "APNG")
    ];

    private static readonly string[] AnimationOnlyExtensions = [".gif", ".webp", ".png", ".apng"];

    private readonly VideoConverterOptions _options;
    private readonly VideoConversionHandler _conversionHandler;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _closed;

    private string _inputFilePath;
    private string _outputFolderPath;
    private string _outputFileName;
    private VideoConverterCodecItem _selectedCodec;
    private bool _useBitrate;
    private double _videoQuality;
    private decimal _videoBitrate;
    private bool _autoOpenFolder;

    [ObservableProperty]
    private bool _isEncoding;

    [ObservableProperty]
    private bool _isSelecting;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private string _statusText = Localization.Strings.VideoConverterViewModel_Choose_video;

    public VideoConverterViewModel(
        VideoConverterOptions options,
        VideoConversionHandler conversionHandler,
        string? inputFilePath = null)
    {
        _options = options;
        _conversionHandler = conversionHandler;
        _inputFilePath = options.InputFilePath ?? string.Empty;
        _outputFolderPath = options.OutputFolderPath ?? string.Empty;
        _outputFileName = options.OutputFileName ?? string.Empty;
        _selectedCodec = Codecs.First(x => (int)x.Codec == (int)options.VideoCodec);
        _useBitrate = options.VideoQualityUseBitrate;
        _videoQuality = options.VideoQuality;
        _videoBitrate = options.VideoQualityBitrate;
        _autoOpenFolder = options.AutoOpenFolder;
        _statusText = string.IsNullOrWhiteSpace(_inputFilePath)
            ? Localization.Strings.VideoConverterViewModel_Choose_video_or_animation
            : string.Empty;

        if (!string.IsNullOrWhiteSpace(inputFilePath))
        {
            LoadInput(inputFilePath);
        }
    }

    public Func<string, Task<string?>>? SelectInputFileRequested { get; set; }
    public Func<string, Task<string?>>? SelectOutputFolderRequested { get; set; }

    public bool IsClosed => _closed;
    public bool IsIdle => !_closed && !IsEncoding;
    public bool CanEdit => IsIdle && !IsSelecting;
    public bool CanStop => !_closed && IsEncoding && _cancellationTokenSource is { IsCancellationRequested: false };

    public string InputFilePath
    {
        get => _inputFilePath;
        set
        {
            if (CanEdit && SetProperty(ref _inputFilePath, value))
            {
                OnPropertyChanged(nameof(InputFileDisplay));
                OnPropertyChanged(nameof(OutputFilePath));
                SettingsValueChanged();
            }
        }
    }

    public string OutputFolderPath
    {
        get => _outputFolderPath;
        set
        {
            if (CanEdit && SetProperty(ref _outputFolderPath, value))
            {
                OnPropertyChanged(nameof(OutputFilePath));
                SettingsValueChanged();
            }
        }
    }

    public string OutputFileName
    {
        get => _outputFileName;
        set
        {
            if (CanEdit && SetProperty(ref _outputFileName, value))
            {
                OnPropertyChanged(nameof(OutputFilePath));
                SettingsValueChanged();
            }
        }
    }

    public VideoConverterCodecItem SelectedCodec
    {
        get => _selectedCodec;
        set
        {
            if (CanEdit && value != null && SetProperty(ref _selectedCodec, value))
            {
                OnPropertyChanged(nameof(OutputFilePath));
                OnPropertyChanged(nameof(ShowsQualityControls));
                OnPropertyChanged(nameof(CanChooseRateControl));
                OnPropertyChanged(nameof(ShowsQualitySlider));
                OnPropertyChanged(nameof(ShowsBitrate));
                OnPropertyChanged(nameof(QualityMinimum));
                OnPropertyChanged(nameof(QualityMaximum));
                VideoQuality = Math.Clamp(VideoQuality, QualityMinimum, QualityMaximum);
                SettingsValueChanged();
            }
        }
    }

    public bool UseBitrate
    {
        get => _useBitrate;
        set
        {
            if (CanEdit && SetProperty(ref _useBitrate, value))
            {
                OnPropertyChanged(nameof(ShowsQualitySlider));
                OnPropertyChanged(nameof(ShowsBitrate));
                SettingsValueChanged();
            }
        }
    }

    public double VideoQuality
    {
        get => _videoQuality;
        set { if (CanEdit && SetProperty(ref _videoQuality, value)) SettingsValueChanged(); }
    }

    public decimal VideoBitrate
    {
        get => _videoBitrate;
        set { if (CanEdit && SetProperty(ref _videoBitrate, value)) SettingsValueChanged(); }
    }

    public bool AutoOpenFolder
    {
        get => _autoOpenFolder;
        set { if (CanEdit && SetProperty(ref _autoOpenFolder, value)) SettingsValueChanged(); }
    }

    public string InputFileDisplay => string.IsNullOrWhiteSpace(InputFilePath) ? Localization.Strings.VideoConverterViewModel_No_file_selected : InputFilePath;
    public bool ShowsQualityControls => SelectedCodec.Codec is not (VideoConverterCodec.Gif or VideoConverterCodec.Webp or VideoConverterCodec.Apng);
    public bool CanChooseRateControl => SelectedCodec.Codec is VideoConverterCodec.X264 or VideoConverterCodec.X265 or VideoConverterCodec.Vp8
        or VideoConverterCodec.Vp9 or VideoConverterCodec.Av1 or VideoConverterCodec.Xvid;
    public bool ShowsQualitySlider => CanChooseRateControl && !UseBitrate;
    public bool ShowsBitrate => ShowsQualityControls && (!CanChooseRateControl || UseBitrate);

    public double QualityMinimum => SelectedCodec.Codec switch
    {
        VideoConverterCodec.Vp8 => 4,
        VideoConverterCodec.Xvid => 1,
        _ => 0
    };

    public double QualityMaximum => SelectedCodec.Codec switch
    {
        VideoConverterCodec.Vp8 or VideoConverterCodec.Vp9 or VideoConverterCodec.Av1 => 63,
        VideoConverterCodec.Xvid => 31,
        _ => 51
    };

    public string OutputFilePath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(OutputFolderPath) || string.IsNullOrWhiteSpace(OutputFileName))
            {
                return string.Empty;
            }

            string path = Path.Combine(OutputFolderPath, OutputFileName);
            return Path.HasExtension(OutputFileName) ? path : Path.ChangeExtension(path, GetFileExtension());
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private async Task BrowseInputAsync()
    {
        Func<string, Task<string?>>? selectInput = SelectInputFileRequested;
        if (!CanEdit || selectInput == null)
        {
            return;
        }

        IsSelecting = true;
        try
        {
            if (_closed) return;
            string? filePath = await selectInput(Localization.Strings.VideoConverterViewModel_Select_input_dialog);
            if (!_closed && !string.IsNullOrWhiteSpace(filePath))
            {
                IsSelecting = false;
                LoadInput(filePath);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception) when (_closed) { }
        finally
        {
            IsSelecting = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private async Task BrowseOutputFolderAsync()
    {
        Func<string, Task<string?>>? selectOutput = SelectOutputFolderRequested;
        if (!CanEdit || selectOutput == null)
        {
            return;
        }

        IsSelecting = true;
        try
        {
            if (_closed) return;
            string? folderPath = await selectOutput(Localization.Strings.VideoConverterViewModel_Select_output_dialog);
            if (!_closed && !string.IsNullOrWhiteSpace(folderPath))
            {
                IsSelecting = false;
                OutputFolderPath = folderPath;
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception) when (_closed) { }
        finally
        {
            IsSelecting = false;
        }
    }

    public void LoadInput(string filePath)
    {
        if (!CanEdit) return;

        InputFilePath = filePath;

        if (string.IsNullOrWhiteSpace(OutputFolderPath))
        {
            OutputFolderPath = Path.GetDirectoryName(filePath) ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(OutputFileName))
        {
            OutputFileName = $"{Path.GetFileNameWithoutExtension(filePath)}-output";
        }

        StatusText = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private async Task StartEncodingAsync()
    {
        if (!CanEdit)
        {
            return;
        }

        if (!File.Exists(InputFilePath))
        {
            StatusText = Localization.Strings.VideoConverterViewModel_Select_existing_input;
            return;
        }

        if (string.IsNullOrWhiteSpace(OutputFolderPath) || !Directory.Exists(OutputFolderPath))
        {
            StatusText = Localization.Strings.VideoConverterViewModel_Select_existing_output;
            return;
        }

        if (string.IsNullOrWhiteSpace(OutputFileName))
        {
            StatusText = Localization.Strings.VideoConverterViewModel_Enter_output_name;
            return;
        }

        PersistSettings();
        using CancellationTokenSource cancellation = new();
        _cancellationTokenSource = cancellation;
        IsEncoding = true;
        IProgress<double> progress = new Progress<double>(value =>
        {
            // Progress can already be queued when Stop, Close or the next job runs.
            if (!_closed && IsEncoding && ReferenceEquals(_cancellationTokenSource, cancellation) &&
                !cancellation.IsCancellationRequested)
            {
                Progress = Math.Clamp(value, 0, 100);
            }
        });

        try
        {
            cancellation.Token.ThrowIfCancellationRequested();
            Progress = 0;
            StatusText = Localization.Strings.VideoConverterViewModel_Converting;
            VideoConversionRequest request = new(BuildArguments(), OutputFilePath, AutoOpenFolder);
            cancellation.Token.ThrowIfCancellationRequested();
            VideoConversionResult result = await _conversionHandler(request, progress, cancellation.Token);
            if (_closed) return;

            if (cancellation.IsCancellationRequested || result.WasCancelled)
            {
                Progress = 0;
                StatusText = Localization.Strings.VideoConverterViewModel_Conversion_stopped;
            }
            else if (result.Succeeded)
            {
                Progress = 100;
                StatusText = string.Format(Localization.Strings.VideoConverterViewModel_Conversion_complete, request.OutputFilePath);
            }
            else
            {
                Progress = 0;
                StatusText = string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? Localization.Strings.VideoConverterViewModel_Conversion_failed
                    : string.Format(Localization.Strings.VideoConverterViewModel_Conversion_failed_message, result.ErrorMessage);
            }
        }
        catch (OperationCanceledException)
        {
            if (_closed) return;
            Progress = 0;
            StatusText = Localization.Strings.VideoConverterViewModel_Conversion_stopped;
        }
        catch (Exception ex)
        {
            if (_closed) return;
            Progress = 0;
            StatusText = cancellation.IsCancellationRequested
                ? Localization.Strings.VideoConverterViewModel_Conversion_stopped
                : string.Format(Localization.Strings.VideoConverterViewModel_Conversion_failed_message, ex.Message);
        }
        finally
        {
            _cancellationTokenSource = null;
            IsEncoding = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void StopEncoding()
    {
        if (CanStop)
        {
            StatusText = Localization.Strings.VideoConverterViewModel_Stopping;
            _cancellationTokenSource?.Cancel();
            NotifyActionState();
        }
    }

    partial void OnIsEncodingChanged(bool value) => NotifyActionState();
    partial void OnIsSelectingChanged(bool value) => NotifyActionState();

    private void NotifyActionState()
    {
        OnPropertyChanged(nameof(IsIdle));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanStop));
        BrowseInputCommand.NotifyCanExecuteChanged();
        BrowseOutputFolderCommand.NotifyCanExecuteChanged();
        StartEncodingCommand.NotifyCanExecuteChanged();
        StopEncodingCommand.NotifyCanExecuteChanged();
    }

    private void SettingsValueChanged()
    {
        PersistSettings();
        StartEncodingCommand.NotifyCanExecuteChanged();
    }

    private void PersistSettings()
    {
        _options.InputFilePath = InputFilePath;
        _options.OutputFolderPath = OutputFolderPath;
        _options.OutputFileName = OutputFileName;
        _options.VideoCodec = (ConverterVideoCodecs)SelectedCodec.Codec;
        _options.VideoQualityUseBitrate = UseBitrate;
        _options.VideoQuality = (int)Math.Round(VideoQuality);
        _options.VideoQualityBitrate = (int)VideoBitrate;
        _options.AutoOpenFolder = AutoOpenFolder;
    }

    private string BuildArguments()
    {
        StringBuilder args = new();
        args.Append($"-i \"{InputFilePath}\" ");
        int quality = Math.Clamp((int)Math.Round(VideoQuality), (int)QualityMinimum, (int)QualityMaximum);
        int bitrate = Math.Max(0, (int)VideoBitrate);

        switch (SelectedCodec.Codec)
        {
            case VideoConverterCodec.X264:
                args.Append("-c:v libx264 -preset medium ");
                args.Append(UseBitrate ? $"-b:v {bitrate}k " : $"-crf {quality} ");
                args.Append("-pix_fmt yuv420p -movflags +faststart ");
                break;
            case VideoConverterCodec.X265:
                args.Append("-c:v libx265 -preset medium ");
                args.Append(UseBitrate ? $"-b:v {bitrate}k " : $"-crf {quality} ");
                break;
            case VideoConverterCodec.H264Nvenc:
                args.Append($"-c:v h264_nvenc -preset p4 -tune hq -profile:v high -b:v {bitrate}k ");
                break;
            case VideoConverterCodec.HevcNvenc:
                args.Append($"-c:v hevc_nvenc -preset p4 -tune hq -profile:v main -b:v {bitrate}k ");
                break;
            case VideoConverterCodec.H264Amf:
                args.Append($"-c:v h264_amf -usage transcoding -profile main -quality balanced -b:v {bitrate}k ");
                break;
            case VideoConverterCodec.HevcAmf:
                args.Append($"-c:v hevc_amf -usage transcoding -profile main -quality balanced -b:v {bitrate}k ");
                break;
            case VideoConverterCodec.H264Qsv:
                args.Append($"-c:v h264_qsv -preset medium -b:v {bitrate}k ");
                break;
            case VideoConverterCodec.HevcQsv:
                args.Append($"-c:v hevc_qsv -preset medium -b:v {bitrate}k ");
                break;
            case VideoConverterCodec.Vp8:
                args.Append("-c:v libvpx ");
                args.Append(UseBitrate ? $"-b:v {bitrate}k " : $"-crf {quality} -b:v 100M ");
                break;
            case VideoConverterCodec.Vp9:
                args.Append("-c:v libvpx-vp9 ");
                args.Append(UseBitrate ? $"-b:v {bitrate}k " : $"-crf {quality} -b:v 0 ");
                break;
            case VideoConverterCodec.Av1:
                args.Append("-c:v libsvtav1 ");
                args.Append(UseBitrate ? $"-b:v {bitrate}k " : $"-crf {quality} ");
                break;
            case VideoConverterCodec.Xvid:
                args.Append("-c:v libxvid ");
                args.Append(UseBitrate ? $"-b:v {bitrate}k " : $"-q:v {quality} ");
                break;
            case VideoConverterCodec.Gif:
                args.Append("-lavfi \"palettegen=stats_mode=full[palette],[0:v][palette]paletteuse=dither=sierra2_4a\" ");
                break;
            case VideoConverterCodec.Webp:
                args.Append("-c:v libwebp -lossless 0 -preset default -loop 0 ");
                break;
            case VideoConverterCodec.Apng:
                args.Append("-f apng -plays 0 ");
                break;
        }

        if (SelectedCodec.Codec is VideoConverterCodec.X265 or VideoConverterCodec.HevcNvenc
            or VideoConverterCodec.HevcAmf or VideoConverterCodec.HevcQsv)
        {
            args.Append("-tag:v hvc1 ");
        }

        bool animationOnly = AnimationOnlyExtensions.Any(extension => InputFilePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
        if (!animationOnly)
        {
            switch (SelectedCodec.Codec)
            {
                case VideoConverterCodec.X264:
                case VideoConverterCodec.X265:
                case VideoConverterCodec.H264Nvenc:
                case VideoConverterCodec.HevcNvenc:
                case VideoConverterCodec.H264Amf:
                case VideoConverterCodec.HevcAmf:
                case VideoConverterCodec.H264Qsv:
                case VideoConverterCodec.HevcQsv:
                    args.Append("-c:a aac -b:a 128k ");
                    break;
                case VideoConverterCodec.Vp8:
                case VideoConverterCodec.Vp9:
                    args.Append("-c:a libvorbis -q:a 3 ");
                    break;
                case VideoConverterCodec.Av1:
                    args.Append("-c:a libopus -b:a 128k ");
                    break;
                case VideoConverterCodec.Xvid:
                    args.Append("-c:a libmp3lame -q:a 4 ");
                    break;
            }
        }

        args.Append($"-y \"{OutputFilePath}\"");
        return args.ToString();
    }

    private string GetFileExtension() => SelectedCodec.Codec switch
    {
        VideoConverterCodec.Vp8 or VideoConverterCodec.Vp9 => "webm",
        VideoConverterCodec.Av1 => "mkv",
        VideoConverterCodec.Xvid => "avi",
        VideoConverterCodec.Gif => "gif",
        VideoConverterCodec.Webp => "webp",
        VideoConverterCodec.Apng => "apng",
        _ => "mp4"
    };

    public void Dispose()
    {
        if (_closed) return;
        _closed = true;
        OnPropertyChanged(nameof(IsClosed));
        NotifyActionState();
        // The active job keeps its token valid until its handler really completes.
        _cancellationTokenSource?.Cancel();
    }
}
