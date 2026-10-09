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

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Tools.Localization;

namespace ShareX.Tools;

public sealed record VideoTrimmerThumbnail(double Position, Bitmap Image);

public sealed partial class VideoTrimmerViewModel : ViewModelBase, IDisposable
{
    private readonly VideoTrimmerService _service;
    private readonly WindowsMediaPlayer _player;
    private readonly Func<string?>? _resolveFFmpegPath;
    private readonly Action? _playNotificationSound;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly List<VideoTrimmerThumbnail> _thumbnails = [];
    private CancellationTokenSource? _loadCancellation;
    private CancellationTokenSource? _exportCancellation;
    private bool _disposed;
    private bool _updatingPlaybackPosition;

    [ObservableProperty] private string _inputFilePath = string.Empty;
    [ObservableProperty] private double _duration;
    [ObservableProperty] private double _position;
    [ObservableProperty] private double _start;
    [ObservableProperty] private double _end;
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private string _statusText = Strings.VideoTrimmer_ChooseVideo;
    [ObservableProperty] private string _outputFilePath = string.Empty;
    [ObservableProperty] private bool _precise;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isExporting;
    [ObservableProperty] private double _progress;

    public VideoTrimmerViewModel(string ffmpegPath, WindowsMediaPlayer player, Action? playNotificationSound = null,
        Func<string?>? resolveFFmpegPath = null)
    {
        _service = new(ffmpegPath);
        _player = player;
        _resolveFFmpegPath = resolveFFmpegPath;
        _playNotificationSound = playNotificationSound;
        _player.PositionChanged += UpdatePlaybackPosition;
        _player.IsPlayingChanged += UpdatePlaybackState;
        _player.PlaybackFailed += HandlePlaybackFailure;
    }

    public Func<Task<string?>>? SelectInputRequested { get; set; }
    public Func<string, Task<string?>>? SelectOutputRequested { get; set; }
    public Action<string>? ShowErrorRequested { get; set; }
    public IReadOnlyList<VideoTrimmerThumbnail> Thumbnails => _thumbnails;
    public bool HasVideo => Duration > 0;
    public bool HasStatus => !string.IsNullOrEmpty(StatusText);
    public string PlayActionText => IsPlaying ? Strings.AnimatedGifTrimmer_Pause : Strings.AnimatedGifTrimmer_Play;
    public bool CanEdit => HasVideo && !IsExporting;
    public bool CanTrim => CanEdit && (Start >= 0.001 || End <= Duration - 0.001);
    public bool CanBrowse => !IsExporting;
    public bool IsWorking => IsLoading || IsExporting;
    public bool CanUsePrimaryAction => IsExporting || CanTrim;
    public bool HasOutput => !string.IsNullOrEmpty(OutputFilePath);
    public System.Windows.Input.ICommand PrimaryActionCommand => IsExporting ? CancelCommand : ExportCommand;
    public string PrimaryActionText => IsExporting ? Strings.VideoTrimmer_Cancel : Strings.VideoTrimmer_Export;
    public bool Lossless
    {
        get => !Precise;
        set { if (value) Precise = false; }
    }
    public string PositionText => FormatTime(Position);
    public string DurationText => FormatTime(Duration);
    public string StartTimeText => FormatTime(Start);
    public string EndTimeText => FormatTime(End);
    public string SelectionDurationText => FormatTime(End - Start);
    public string SelectionText => string.Format(Strings.VideoTrimmer_Selection, FormatTime(End - Start));
    public string InputDisplay => string.IsNullOrEmpty(InputFilePath) ? Strings.VideoTrimmer_ChooseVideo : InputFilePath;

    public static string FormatTime(double seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Math.Max(0, seconds));
        return $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}.{time.Milliseconds:000}";
    }

    public void SetStartTime(string? text)
    {
        if (TryParseTime(text, out double seconds)) Start = seconds;
        OnPropertyChanged(nameof(StartTimeText));
    }

    public void SetEndTime(string? text)
    {
        if (TryParseTime(text, out double seconds)) End = seconds;
        OnPropertyChanged(nameof(EndTimeText));
    }

    internal static bool TryParseTime(string? text, out double seconds)
    {
        seconds = 0;
        string[] parts = text?.Trim().Split(':') ?? [];
        if (parts.Length != 3 ||
            !int.TryParse(parts[0], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out int hours) ||
            !int.TryParse(parts[1], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out int minutes) ||
            !double.TryParse(parts[2], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out double secondsPart) ||
            hours < 0 || minutes is < 0 or > 59 || secondsPart is < 0 or >= 60)
        {
            return false;
        }

        seconds = hours * 3600d + minutes * 60d + secondsPart;
        return double.IsFinite(seconds);
    }

    [RelayCommand]
    private async Task BrowseAsync()
    {
        if (!CanBrowse || SelectInputRequested == null) return;
        try
        {
            string? file = await SelectInputRequested();
            if (file != null && !_disposed) await LoadInputAsync(file);
        }
        catch (Exception ex) { StatusText = ex.Message; }
    }

    public async Task LoadInputAsync(string file)
    {
        if (!CanBrowse || _disposed) return;
        _loadCancellation?.Cancel();
        _player.Unload();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _loadCancellation = cancellation;
        CancellationToken token = cancellation.Token;
        Duration = 0;
        Position = Start = End = 0;
        ClearThumbnails();
        InputFilePath = file;
        OutputFilePath = string.Empty;
        IsLoading = true;
        StatusText = Strings.VideoTrimmer_Loading;
        try
        {
            if (!File.Exists(file)) throw new FileNotFoundException(Strings.VideoTrimmer_InvalidVideo);
            VideoPlaybackInfo info = await _player.LoadAsync(file, token);
            token.ThrowIfCancellationRequested();
            _updatingPlaybackPosition = true;
            try
            {
                Duration = info.Duration;
                End = info.Duration;
                Position = 0;
            }
            finally { _updatingPlaybackPosition = false; }
            // The filmstrip has its own persistent decoder. Building it cannot move the playback cursor.
            // Thumbnail failures are nonfatal; native playback may support formats the reader cannot convert.
            try
            {
                var thumbnails = await NativeVideoThumbnails.CreateAsync(file, info.Duration, token);
                if (token.IsCancellationRequested || _disposed)
                {
                    foreach (var thumbnail in thumbnails) thumbnail.Image.Dispose();
                    token.ThrowIfCancellationRequested();
                    return;
                }
                _thumbnails.AddRange(thumbnails);
                OnPropertyChanged(nameof(Thumbnails));
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                token.ThrowIfCancellationRequested();
                System.Diagnostics.Debug.WriteLine(ex);
            }

            StatusText = string.Empty;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
            {
                StatusText = string.Format(Strings.VideoTrimmer_PlaybackError, ex.Message);
                ShowErrorRequested?.Invoke(StatusText);
            }
        }
        finally
        {
            if (_loadCancellation == cancellation)
            {
                _loadCancellation = null;
                IsLoading = false;
            }
        }
    }

    partial void OnPositionChanged(double value)
    {
        if (!double.IsFinite(value) || value < 0 || value > Duration)
        {
            Position = double.IsFinite(value) ? Math.Clamp(value, 0, Duration) : 0;
            return;
        }

        OnPropertyChanged(nameof(PositionText));
        if (HasVideo && !_disposed && !_updatingPlaybackPosition) _player.Seek(value);
    }

    private void UpdatePlaybackPosition(double position)
    {
        if (_disposed || !HasVideo) return;
        bool reachedEnd = IsPlaying && position >= End;
        if (reachedEnd) _player.Pause();
        _updatingPlaybackPosition = true;
        try
        {
            Position = reachedEnd ? End : position;
        }
        finally { _updatingPlaybackPosition = false; }
        if (reachedEnd) _player.Seek(End);
    }

    private void UpdatePlaybackState(bool playing) => IsPlaying = playing;

    private void HandlePlaybackFailure(Exception error)
    {
        if (_disposed) return;
        _loadCancellation?.Cancel();
        Duration = 0;
        ClearThumbnails();
        StatusText = string.Format(Strings.VideoTrimmer_PlaybackError, error.Message);
        ShowErrorRequested?.Invoke(StatusText);
    }

    [RelayCommand]
    private void TogglePlay()
    {
        if (!CanEdit) return;
        if (IsPlaying) _player.Pause();
        else
        {
            if (Position < Start || Position >= End - 0.001) Position = Start;
            _player.Play();
        }
    }

    [RelayCommand] private void StepBackward() { if (CanEdit) _player.StepFrame(false); }
    [RelayCommand] private void StepForward() { if (CanEdit) _player.StepFrame(true); }
    partial void OnIsPlayingChanged(bool value) => OnPropertyChanged(nameof(PlayActionText));
    partial void OnStatusTextChanged(string value) => OnPropertyChanged(nameof(HasStatus));

    partial void OnStartChanged(double value)
    {
        double clamped = double.IsFinite(value) ? Math.Clamp(value, 0, Math.Max(0, End - MinimumSelection)) : 0;
        if (value != clamped) { Start = clamped; return; }
        OnPropertyChanged(nameof(StartTimeText));
        OnPropertyChanged(nameof(SelectionDurationText));
        OnPropertyChanged(nameof(SelectionText));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
        if (HasVideo) Position = Start;
    }

    partial void OnEndChanged(double value)
    {
        double clamped = double.IsFinite(value) ? Math.Clamp(value, Math.Min(Duration, Start + MinimumSelection), Duration) : Duration;
        if (value != clamped) { End = clamped; return; }
        OnPropertyChanged(nameof(EndTimeText));
        OnPropertyChanged(nameof(SelectionDurationText));
        OnPropertyChanged(nameof(SelectionText));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
        if (HasVideo) Position = End;
    }

    private double MinimumSelection => Math.Min(0.001, Duration);
    partial void OnDurationChanged(double value)
    {
        OnPropertyChanged(nameof(HasVideo));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
        OnPropertyChanged(nameof(DurationText));
    }
    partial void OnInputFilePathChanged(string value) => OnPropertyChanged(nameof(InputDisplay));
    partial void OnOutputFilePathChanged(string value) => OnPropertyChanged(nameof(HasOutput));
    partial void OnPreciseChanged(bool value)
    {
        OnPropertyChanged(nameof(Lossless));
    }
    partial void OnIsLoadingChanged(bool value) => OnPropertyChanged(nameof(IsWorking));
    partial void OnIsExportingChanged(bool value)
    {
        if (value) _player.Pause();
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanBrowse));
        OnPropertyChanged(nameof(IsWorking));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
        OnPropertyChanged(nameof(PrimaryActionCommand));
        OnPropertyChanged(nameof(PrimaryActionText));
    }

    [RelayCommand] private void SetStart() { if (CanEdit) Start = Position; }
    [RelayCommand] private void SetEnd() { if (CanEdit) End = Position; }
    [RelayCommand] private void Reset() { if (CanEdit) { Start = 0; End = Duration; Position = 0; } }
    [RelayCommand] private void OpenOutput() { if (HasOutput) ShareX.HelpersLib.FileHelpers.OpenFolderWithFile(OutputFilePath); }
    [RelayCommand]
    private void Cancel()
    {
        _loadCancellation?.Cancel();
        _exportCancellation?.Cancel();
        if (!IsExporting) StatusText = Strings.VideoTrimmer_Cancelled;
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (!CanTrim || SelectOutputRequested == null || _disposed) return;
        IsExporting = true;
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _exportCancellation = cancellation;
        string? errorMessage = null;
        try
        {
            // Playback can open without FFmpeg installed. Resolve/download the configured executable only for export.
            if (_resolveFFmpegPath != null)
            {
                string? ffmpegPath = _resolveFFmpegPath();
                if (string.IsNullOrEmpty(ffmpegPath)) return;
                _service.FFmpegPath = ffmpegPath;
            }
            string extension = Precise ? ".mp4" : Path.GetExtension(InputFilePath);
            string? output = await SelectOutputRequested(Path.GetFileNameWithoutExtension(InputFilePath) + "-trimmed" + extension);
            if (output == null) return;
            cancellation.Token.ThrowIfCancellationRequested();
            _loadCancellation?.Cancel();
            Progress = 0;
            StatusText = Strings.VideoTrimmer_Exporting;
            await _service.TrimAsync(InputFilePath, output, Start, End, Duration, Precise,
                new Progress<double>(value => { if (!cancellation.IsCancellationRequested && !_disposed) Progress = value; }), cancellation.Token);
            Progress = 100;
            OutputFilePath = output;
            StatusText = string.Format(Strings.VideoTrimmer_Saved, output);
            _playNotificationSound?.Invoke();
        }
        catch (OperationCanceledException)
        {
            Progress = 0;
            StatusText = Strings.VideoTrimmer_Cancelled;
        }
        catch (Exception ex)
        {
            Progress = 0;
            StatusText = ex.Message;
            if (!cancellation.IsCancellationRequested) errorMessage = ex.Message;
        }
        finally
        {
            _exportCancellation = null;
            IsExporting = false;
        }

        if (errorMessage != null && !_disposed) ShowErrorRequested?.Invoke(errorMessage);
    }

    private void ClearThumbnails()
    {
        foreach (var thumbnail in _thumbnails) thumbnail.Image.Dispose();
        _thumbnails.Clear();
        OnPropertyChanged(nameof(Thumbnails));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lifetime.Cancel();
        _lifetime.Dispose();
        _player.PositionChanged -= UpdatePlaybackPosition;
        _player.IsPlayingChanged -= UpdatePlaybackState;
        _player.PlaybackFailed -= HandlePlaybackFailure;
        _player.Dispose();
        ClearThumbnails();
    }
}
