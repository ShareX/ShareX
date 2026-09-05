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
using Strings = ShareX.Tools.Localization.Strings;

namespace ShareX.Tools;

public sealed record VideoTrimmerThumbnail(double Position, Bitmap Image);

public sealed partial class VideoTrimmerViewModel : ViewModelBase, IDisposable
{
    private readonly VideoTrimmerService _service;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly SemaphoreSlim _previewGate = new(1);
    private readonly List<VideoTrimmerThumbnail> _thumbnails = [];
    private readonly LinkedList<(long Key, Bitmap Image)> _frames = [];
    private CancellationTokenSource? _loadCancellation;
    private CancellationTokenSource? _seekCancellation;
    private CancellationTokenSource? _exportCancellation;
    private bool _disposed;

    [ObservableProperty] private string _inputFilePath = string.Empty;
    [ObservableProperty] private double _duration;
    [ObservableProperty] private double _position;
    [ObservableProperty] private double _start;
    [ObservableProperty] private double _end;
    [ObservableProperty] private Bitmap? _preview;
    [ObservableProperty] private string _previewText = Strings.VideoTrimmer_PreviewHint;
    [ObservableProperty] private string _statusText = Strings.VideoTrimmer_ChooseVideo;
    [ObservableProperty] private string _outputFilePath = string.Empty;
    [ObservableProperty] private bool _precise;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isExporting;
    [ObservableProperty] private double _progress;

    public VideoTrimmerViewModel(string ffmpegPath) => _service = new(ffmpegPath);

    public Func<Task<string?>>? SelectInputRequested { get; set; }
    public Func<string, Task<string?>>? SelectOutputRequested { get; set; }
    public IReadOnlyList<VideoTrimmerThumbnail> Thumbnails => _thumbnails;
    public bool HasVideo => Duration > 0;
    public bool CanEdit => HasVideo && !IsExporting;
    public bool CanBrowse => !IsExporting;
    public bool IsWorking => IsLoading || IsExporting;
    public bool HasOutput => !string.IsNullOrEmpty(OutputFilePath);
    public string ModeDescription => Precise ? Strings.VideoTrimmer_PreciseHint : Strings.VideoTrimmer_CopyHint;
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
        _seekCancellation?.Cancel();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _loadCancellation = cancellation;
        CancellationToken token = cancellation.Token;
        Duration = 0;
        Position = Start = End = 0;
        ClearFrames();
        InputFilePath = file;
        OutputFilePath = string.Empty;
        IsLoading = true;
        StatusText = Strings.VideoTrimmer_Loading;
        try
        {
            if (!File.Exists(file)) throw new FileNotFoundException(Strings.VideoTrimmer_InvalidVideo);
            double duration = await _service.GetDurationAsync(file, token);
            token.ThrowIfCancellationRequested();
            Duration = duration;
            End = duration;
            Position = 0;
            // A fixed number of sparse input seeks bounds work even for hours-long recordings.
            // One background worker builds the overview; a separate serialized worker refines seeks.
            for (int i = 0; i < 12; i++)
            {
                double position = duration * i / 12;
                byte[] bytes = await _service.GetFrameAsync(file, position, token);
                token.ThrowIfCancellationRequested();
                using MemoryStream stream = new(bytes);
                Bitmap bitmap = new(stream);
                _thumbnails.Add(new(position, bitmap));
                OnPropertyChanged(nameof(Thumbnails));
                if (Preview == null)
                {
                    Preview = bitmap;
                    PreviewText = Strings.VideoTrimmer_CachedPreview;
                }
            }

            StatusText = string.Empty;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested) StatusText = ex.Message;
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
        if (HasVideo && !_disposed) _ = RefreshPreviewAsync();
    }

    private async Task RefreshPreviewAsync()
    {
        _seekCancellation?.Cancel();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _seekCancellation = cancellation;
        CancellationToken token = cancellation.Token;
        // Avoid seeking exactly to EOF, where no frame exists.
        double target = Math.Min(Position, Math.Max(0, Duration - 0.1));
        long key = (long)Math.Round(target * 1000);
        try
        {
            var cached = _frames.First;
            while (cached != null && cached.Value.Key != key) cached = cached.Next;
            if (cached != null)
            {
                Preview = cached.Value.Image;
                _frames.Remove(cached);
                _frames.AddFirst(cached);
                PreviewText = string.Format(Strings.VideoTrimmer_FrameAt, FormatTime(target));
                return;
            }

            if (_thumbnails.Count > 0)
            {
                Preview = _thumbnails.MinBy(x => Math.Abs(x.Position - target))!.Image;
                PreviewText = Strings.VideoTrimmer_CachedPreview;
            }

            await Task.Delay(220, token);
            await _previewGate.WaitAsync(token);
            try
            {
                byte[] bytes = await _service.GetFrameAsync(InputFilePath, target, token);
                token.ThrowIfCancellationRequested();
                using MemoryStream stream = new(bytes);
                Bitmap bitmap = new(stream);
                Preview = bitmap;
                PreviewText = string.Format(Strings.VideoTrimmer_FrameAt, FormatTime(target));
                _frames.AddFirst((key, bitmap));
                if (_frames.Count > 48)
                {
                    _frames.Last!.Value.Image.Dispose();
                    _frames.RemoveLast();
                }
            }
            finally { _previewGate.Release(); }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested) PreviewText = ex.Message;
        }
        finally
        {
            if (_seekCancellation == cancellation) _seekCancellation = null;
        }
    }

    partial void OnStartChanged(double value)
    {
        double clamped = double.IsFinite(value) ? Math.Clamp(value, 0, Math.Max(0, End - MinimumSelection)) : 0;
        if (value != clamped) { Start = clamped; return; }
        OnPropertyChanged(nameof(StartTimeText));
        OnPropertyChanged(nameof(SelectionDurationText));
        OnPropertyChanged(nameof(SelectionText));
        if (HasVideo) Position = Start;
    }

    partial void OnEndChanged(double value)
    {
        double clamped = double.IsFinite(value) ? Math.Clamp(value, Math.Min(Duration, Start + MinimumSelection), Duration) : Duration;
        if (value != clamped) { End = clamped; return; }
        OnPropertyChanged(nameof(EndTimeText));
        OnPropertyChanged(nameof(SelectionDurationText));
        OnPropertyChanged(nameof(SelectionText));
        if (HasVideo) Position = End;
    }

    private double MinimumSelection => Math.Min(0.001, Duration);
    partial void OnDurationChanged(double value)
    {
        OnPropertyChanged(nameof(HasVideo));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(DurationText));
    }
    partial void OnInputFilePathChanged(string value) => OnPropertyChanged(nameof(InputDisplay));
    partial void OnOutputFilePathChanged(string value) => OnPropertyChanged(nameof(HasOutput));
    partial void OnPreciseChanged(bool value) => OnPropertyChanged(nameof(ModeDescription));
    partial void OnIsLoadingChanged(bool value) => OnPropertyChanged(nameof(IsWorking));
    partial void OnIsExportingChanged(bool value)
    {
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanBrowse));
        OnPropertyChanged(nameof(IsWorking));
    }

    [RelayCommand] private void SetStart() { if (CanEdit) Start = Position; }
    [RelayCommand] private void SetEnd() { if (CanEdit) End = Position; }
    [RelayCommand] private void Reset() { if (CanEdit) { Start = 0; End = Duration; Position = 0; } }
    [RelayCommand] private void OpenOutput() { if (HasOutput) ShareX.HelpersLib.FileHelpers.OpenFolderWithFile(OutputFilePath); }
    [RelayCommand]
    private void Cancel()
    {
        _loadCancellation?.Cancel();
        _seekCancellation?.Cancel();
        _exportCancellation?.Cancel();
        if (!IsExporting) StatusText = Strings.VideoTrimmer_Cancelled;
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (!CanEdit || SelectOutputRequested == null || _disposed) return;
        IsExporting = true;
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _exportCancellation = cancellation;
        try
        {
            string extension = Precise ? ".mp4" : Path.GetExtension(InputFilePath);
            string? output = await SelectOutputRequested(Path.GetFileNameWithoutExtension(InputFilePath) + "-trimmed" + extension);
            if (output == null) return;
            cancellation.Token.ThrowIfCancellationRequested();
            _loadCancellation?.Cancel();
            _seekCancellation?.Cancel();
            Progress = 0;
            StatusText = Strings.VideoTrimmer_Exporting;
            await _service.TrimAsync(InputFilePath, output, Start, End, Duration, Precise,
                new Progress<double>(value => { if (!cancellation.IsCancellationRequested && !_disposed) Progress = value; }), cancellation.Token);
            Progress = 100;
            OutputFilePath = output;
            StatusText = string.Format(Strings.VideoTrimmer_Saved, output);
        }
        catch (OperationCanceledException) { StatusText = Strings.VideoTrimmer_Cancelled; }
        catch (Exception ex) { StatusText = ex.Message; }
        finally
        {
            _exportCancellation = null;
            IsExporting = false;
        }
    }

    private void ClearFrames()
    {
        Preview = null;
        foreach (var thumbnail in _thumbnails) thumbnail.Image.Dispose();
        _thumbnails.Clear();
        foreach (var frame in _frames) frame.Image.Dispose();
        _frames.Clear();
        OnPropertyChanged(nameof(Thumbnails));
        PreviewText = Strings.VideoTrimmer_PreviewHint;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lifetime.Cancel();
        _lifetime.Dispose();
        ClearFrames();
    }
}
