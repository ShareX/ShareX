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
using ShareX.ImageEditor.Hosting;
using System.Text;

namespace ShareX.ImageEditor.Presentation.ViewModels;

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

    private readonly VideoConverterSettings _settings;
    private readonly VideoConversionHandler _conversionHandler;
    private readonly Action<VideoConverterSettings>? _settingsChanged;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutputFilePath))]
    [NotifyPropertyChangedFor(nameof(InputFileDisplay))]
    private string _inputFilePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutputFilePath))]
    private string _outputFolderPath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutputFilePath))]
    private string _outputFileName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutputFilePath))]
    [NotifyPropertyChangedFor(nameof(ShowsQualityControls))]
    [NotifyPropertyChangedFor(nameof(CanChooseRateControl))]
    [NotifyPropertyChangedFor(nameof(ShowsQualitySlider))]
    [NotifyPropertyChangedFor(nameof(ShowsBitrate))]
    [NotifyPropertyChangedFor(nameof(QualityMinimum))]
    [NotifyPropertyChangedFor(nameof(QualityMaximum))]
    private VideoConverterCodecItem _selectedCodec;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowsQualitySlider))]
    [NotifyPropertyChangedFor(nameof(ShowsBitrate))]
    private bool _useBitrate;

    [ObservableProperty]
    private double _videoQuality;

    [ObservableProperty]
    private decimal _videoBitrate;

    [ObservableProperty]
    private bool _autoOpenFolder;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    private bool _isEncoding;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private string _statusText = "Choose a video to get started.";

    public VideoConverterViewModel(
        VideoConverterSettings settings,
        VideoConversionHandler conversionHandler,
        Action<VideoConverterSettings>? settingsChanged = null,
        string? inputFilePath = null)
    {
        _settings = settings;
        _conversionHandler = conversionHandler;
        _settingsChanged = settingsChanged;
        _inputFilePath = settings.InputFilePath ?? string.Empty;
        _outputFolderPath = settings.OutputFolderPath ?? string.Empty;
        _outputFileName = settings.OutputFileName ?? string.Empty;
        _selectedCodec = Codecs.First(x => x.Codec == settings.VideoCodec);
        _useBitrate = settings.VideoQualityUseBitrate;
        _videoQuality = settings.VideoQuality;
        _videoBitrate = settings.VideoQualityBitrate;
        _autoOpenFolder = settings.AutoOpenFolder;

        if (!string.IsNullOrWhiteSpace(inputFilePath))
        {
            LoadInput(inputFilePath);
        }
    }

    public Func<string, Task<string?>>? SelectInputFileRequested { get; set; }
    public Func<string, Task<string?>>? SelectOutputFolderRequested { get; set; }

    public bool IsIdle => !IsEncoding;
    public string InputFileDisplay => string.IsNullOrWhiteSpace(InputFilePath) ? "No file selected" : InputFilePath;
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

    [RelayCommand]
    private async Task BrowseInputAsync()
    {
        if (IsEncoding || SelectInputFileRequested == null)
        {
            return;
        }

        string? filePath = await SelectInputFileRequested("Select video or animation");
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            LoadInput(filePath);
        }
    }

    [RelayCommand]
    private async Task BrowseOutputFolderAsync()
    {
        if (IsEncoding || SelectOutputFolderRequested == null)
        {
            return;
        }

        string? folderPath = await SelectOutputFolderRequested("Select output folder");
        if (!string.IsNullOrWhiteSpace(folderPath))
        {
            OutputFolderPath = folderPath;
        }
    }

    public void LoadInput(string filePath)
    {
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

    [RelayCommand]
    private async Task StartEncodingAsync()
    {
        if (IsEncoding)
        {
            return;
        }

        if (!File.Exists(InputFilePath))
        {
            StatusText = "Select an existing input file.";
            return;
        }

        if (string.IsNullOrWhiteSpace(OutputFolderPath) || !Directory.Exists(OutputFolderPath))
        {
            StatusText = "Select an existing output folder.";
            return;
        }

        if (string.IsNullOrWhiteSpace(OutputFileName))
        {
            StatusText = "Enter an output file name.";
            return;
        }

        PersistSettings();
        IsEncoding = true;
        Progress = 0;
        StatusText = "Converting video...";
        _cancellationTokenSource = new CancellationTokenSource();
        IProgress<double> progress = new Progress<double>(value => Progress = Math.Clamp(value, 0, 100));

        try
        {
            VideoConversionRequest request = new(BuildArguments(), OutputFilePath, AutoOpenFolder);
            VideoConversionResult result = await _conversionHandler(request, progress, _cancellationTokenSource.Token);

            if (result.Succeeded && !result.WasCancelled)
            {
                Progress = 100;
                StatusText = $"Conversion complete: {OutputFilePath}";
            }
            else if (result.WasCancelled)
            {
                Progress = 0;
                StatusText = "Conversion stopped.";
            }
            else
            {
                Progress = 0;
                StatusText = string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "Video conversion failed."
                    : $"Video conversion failed: {result.ErrorMessage}";
            }
        }
        catch (OperationCanceledException)
        {
            Progress = 0;
            StatusText = "Conversion stopped.";
        }
        catch (Exception ex)
        {
            Progress = 0;
            StatusText = $"Video conversion failed: {ex.Message}";
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsEncoding = false;
        }
    }

    [RelayCommand]
    private void StopEncoding()
    {
        if (IsEncoding)
        {
            StatusText = "Stopping conversion...";
            _cancellationTokenSource?.Cancel();
        }
    }

    partial void OnInputFilePathChanged(string value) => SettingsValueChanged();
    partial void OnOutputFolderPathChanged(string value) => SettingsValueChanged();
    partial void OnOutputFileNameChanged(string value) => SettingsValueChanged();

    partial void OnSelectedCodecChanged(VideoConverterCodecItem value)
    {
        VideoQuality = Math.Clamp(VideoQuality, QualityMinimum, QualityMaximum);
        SettingsValueChanged();
    }

    partial void OnUseBitrateChanged(bool value) => SettingsValueChanged();
    partial void OnVideoQualityChanged(double value) => SettingsValueChanged();
    partial void OnVideoBitrateChanged(decimal value) => SettingsValueChanged();
    partial void OnAutoOpenFolderChanged(bool value) => SettingsValueChanged();
    private void SettingsValueChanged()
    {
        PersistSettings();
        StartEncodingCommand.NotifyCanExecuteChanged();
    }

    private void PersistSettings()
    {
        _settings.InputFilePath = InputFilePath;
        _settings.OutputFolderPath = OutputFolderPath;
        _settings.OutputFileName = OutputFileName;
        _settings.VideoCodec = SelectedCodec.Codec;
        _settings.VideoQualityUseBitrate = UseBitrate;
        _settings.VideoQuality = (int)Math.Round(VideoQuality);
        _settings.VideoQualityBitrate = (int)VideoBitrate;
        _settings.AutoOpenFolder = AutoOpenFolder;
        _settingsChanged?.Invoke(_settings);
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
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
