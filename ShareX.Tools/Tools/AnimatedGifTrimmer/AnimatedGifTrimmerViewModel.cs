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
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Tools.Localization;

namespace ShareX.Tools;

public sealed record AnimatedGifTrimmerThumbnail(double Position, Bitmap Image);

public sealed partial class AnimatedGifTrimmerViewModel : ViewModelBase, IDisposable
{
    private readonly Action? _playNotificationSound;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly DispatcherTimer _playTimer = new();
    private readonly List<AnimatedGifTrimmerThumbnail> _thumbnails = [];
    private AnimatedGifTrimmerDocument? _document;
    private CancellationTokenSource? _loadCancellation;
    private CancellationTokenSource? _seekCancellation;
    private CancellationTokenSource? _exportCancellation;
    private int _positionIndex;
    private int _startIndex;
    private int _endIndex;
    private long _previewRequestVersion;
    private long _displayedPreviewRequestVersion;
    private bool _disposed;

    [ObservableProperty] private string _inputFilePath = string.Empty;
    [ObservableProperty] private string _outputFilePath = string.Empty;
    [ObservableProperty] private Bitmap? _preview;
    [ObservableProperty] private string _statusText = Strings.AnimatedGifTrimmer_ChooseGif;
    [ObservableProperty] private string _previewText = string.Empty;
    [ObservableProperty] private bool _reencode;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isExporting;
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private double _progress;

    public AnimatedGifTrimmerViewModel(Action? playNotificationSound = null)
    {
        _playNotificationSound = playNotificationSound;
        _playTimer.Tick += OnPlayTimerTick;
    }

    public Func<Task<string?>>? SelectInputRequested { get; set; }
    public Func<string, Task<string?>>? SelectOutputRequested { get; set; }
    public Action<string>? ShowErrorRequested { get; set; }
    public IReadOnlyList<AnimatedGifTrimmerThumbnail> Thumbnails => _thumbnails;
    public bool HasGif => _document != null;
    public bool CanBrowse => !IsExporting;
    public bool CanEdit => HasGif && !IsLoading && !IsExporting;
    public bool CanPlay => CanEdit && _endIndex - _startIndex > 1;
    public bool CanTrim => CanEdit && (_startIndex > 0 || _endIndex < _document!.FrameCount);
    public bool IsWorking => IsLoading || IsExporting;
    public bool HasOutput => !string.IsNullOrEmpty(OutputFilePath);
    public bool CanUsePrimaryAction => IsExporting || CanTrim;
    public System.Windows.Input.ICommand PrimaryActionCommand => IsExporting ? CancelCommand : ExportCommand;
    public string PrimaryActionText => IsExporting ? Strings.VideoTrimmer_Cancel : Strings.AnimatedGifTrimmer_Export;
    public bool Lossless
    {
        get => !Reencode;
        set { if (value) Reencode = false; }
    }
    public string PlayActionText => IsPlaying ? Strings.AnimatedGifTrimmer_Pause : Strings.AnimatedGifTrimmer_Play;
    public double Duration => _document?.Duration ?? 0;
    public double Position => _document?.FrameStart(_positionIndex) ?? 0;
    public double Start => _document?.FrameStart(_startIndex) ?? 0;
    public double End => _document?.FrameStart(_endIndex) ?? 0;
    public string PositionText => VideoTrimmerViewModel.FormatTime(Position);
    public string DurationText => VideoTrimmerViewModel.FormatTime(Duration);
    public string StartTimeText => VideoTrimmerViewModel.FormatTime(Start);
    public string EndTimeText => VideoTrimmerViewModel.FormatTime(End);
    public string SelectionDurationText => VideoTrimmerViewModel.FormatTime(End - Start);
    public string FrameText => _document == null ? string.Empty :
        string.Format(Strings.AnimatedGifTrimmer_Frame, _positionIndex + 1, _document.FrameCount);
    public string InputDisplay => string.IsNullOrEmpty(InputFilePath) ? Strings.AnimatedGifTrimmer_ChooseGif : InputFilePath;

    public void Seek(double seconds)
    {
        if (_document != null)
        {
            StopPlayback();
            _ = SetPositionIndex(_document.FrameAt(seconds));
        }
    }

    public void SetStart(double seconds)
    {
        if (_document != null) SetStartIndex(Math.Min(_document.NearestBoundary(seconds, false), _endIndex - 1));
        OnPropertyChanged(nameof(StartTimeText));
    }

    public void SetEnd(double seconds)
    {
        if (_document != null) SetEndIndex(Math.Max(_document.NearestBoundary(seconds, true), _startIndex + 1));
        OnPropertyChanged(nameof(EndTimeText));
    }

    public void SetStartTime(string? text)
    {
        if (VideoTrimmerViewModel.TryParseTime(text, out double seconds)) SetStart(seconds);
        else OnPropertyChanged(nameof(StartTimeText));
    }

    public void SetEndTime(string? text)
    {
        if (VideoTrimmerViewModel.TryParseTime(text, out double seconds)) SetEnd(seconds);
        else OnPropertyChanged(nameof(EndTimeText));
    }

    public void StepPosition(int frames)
    {
        if (CanEdit && _document != null)
        {
            StopPlayback();
            _ = SetPositionIndex(Math.Clamp(_positionIndex + frames, 0, _document.FrameCount - 1));
        }
    }

    [RelayCommand] private void StepBackward() => StepPosition(-1);
    [RelayCommand] private void StepForward() => StepPosition(1);

    private Task SetPositionIndex(int index)
    {
        if (_document == null || index == _positionIndex) return Task.CompletedTask;
        _positionIndex = index;
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(PositionText));
        OnPropertyChanged(nameof(FrameText));
        return RefreshPreviewAsync();
    }

    private void SetStartIndex(int index)
    {
        if (_document == null || index == _startIndex) return;
        StopPlayback();
        _startIndex = index;
        NotifySelectionChanged();
        _ = SetPositionIndex(index);
    }

    private void SetEndIndex(int index)
    {
        if (_document == null || index == _endIndex) return;
        StopPlayback();
        _endIndex = index;
        NotifySelectionChanged();
        _ = SetPositionIndex(Math.Min(index - 1, _document.FrameCount - 1));
    }

    private void NotifySelectionChanged()
    {
        if (_document != null && !IsLoading && !IsExporting)
            StatusText = Reencode || _document.CanCopySelection(_startIndex, _endIndex)
                ? string.Empty : Strings.AnimatedGifTrimmer_UseReencode;
        OnPropertyChanged(nameof(Start));
        OnPropertyChanged(nameof(End));
        OnPropertyChanged(nameof(StartTimeText));
        OnPropertyChanged(nameof(EndTimeText));
        OnPropertyChanged(nameof(SelectionDurationText));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanPlay));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
    }

    [RelayCommand]
    private async Task BrowseAsync()
    {
        if (!CanBrowse || SelectInputRequested == null) return;
        try
        {
            string? file = await SelectInputRequested();
            if (file != null) await LoadInputAsync(file);
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
            if (!_disposed) ShowErrorRequested?.Invoke(ex.Message);
        }
    }

    public async Task LoadInputAsync(string file)
    {
        if (!CanBrowse || _disposed) return;
        _loadCancellation?.Cancel();
        _seekCancellation?.Cancel();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _loadCancellation = cancellation;
        CancellationToken token = cancellation.Token;
        StopPlayback();
        ClearDocument();
        InputFilePath = file;
        OutputFilePath = string.Empty;
        IsLoading = true;
        StatusText = Strings.AnimatedGifTrimmer_Loading;
        try
        {
            AnimatedGifTrimmerDocument document = await Task.Run(() => AnimatedGifTrimmerDocument.Open(file, token), token);
            if (token.IsCancellationRequested || _disposed)
            {
                document.Dispose();
                return;
            }
            _document = document;
            _positionIndex = _startIndex = 0;
            _endIndex = document.FrameCount;
            NotifyDocumentChanged();
            byte[] previewBytes = await Task.Run(() => document.RenderFrame(0, token, int.MaxValue), token);
            token.ThrowIfCancellationRequested();
            using (MemoryStream previewStream = new(previewBytes, writable: false))
                Preview = new Bitmap(previewStream);
            PreviewText = FrameText;
            for (int i = 0; i < 12; i++)
            {
                int index = document.FrameAt(document.Duration * i / 12);
                byte[] bytes = await Task.Run(() => document.RenderFrame(index, token), token);
                token.ThrowIfCancellationRequested();
                using MemoryStream stream = new(bytes, writable: false);
                Bitmap bitmap = new(stream);
                _thumbnails.Add(new(document.FrameStart(index), bitmap));
                OnPropertyChanged(nameof(Thumbnails));
            }
            StatusText = string.Empty;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
            {
                ClearDocument();
                StatusText = ex.Message;
                if (!_disposed) ShowErrorRequested?.Invoke(ex.Message);
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

    private async Task RefreshPreviewAsync()
    {
        _seekCancellation?.Cancel();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _seekCancellation = cancellation;
        AnimatedGifTrimmerDocument? document = _document;
        int index = _positionIndex;
        long requestVersion = ++_previewRequestVersion;
        if (document == null) return;
        try
        {
            byte[]? bytes = await Task.Run(() => document.TryRenderFrame(index, cancellation.Token));
            if (bytes == null || _disposed || !ReferenceEquals(document, _document) ||
                requestVersion < _displayedPreviewRequestVersion) return;
            using MemoryStream stream = new(bytes, writable: false);
            Bitmap bitmap = new(stream);
            if (_disposed || !ReferenceEquals(document, _document) ||
                requestVersion < _displayedPreviewRequestVersion) bitmap.Dispose();
            else
            {
                Bitmap? old = Preview;
                Preview = bitmap;
                _displayedPreviewRequestVersion = requestVersion;
                if (!_thumbnails.Any(x => ReferenceEquals(x.Image, old))) old?.Dispose();
                PreviewText = string.Format(Strings.AnimatedGifTrimmer_Frame, index + 1, document.FrameCount);
            }
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) when (cancellation.IsCancellationRequested) { }
        catch (Exception ex)
        {
            if (!cancellation.IsCancellationRequested)
            {
                PreviewText = StatusText = ex.Message;
                StopPlayback();
                if (!_disposed) ShowErrorRequested?.Invoke(ex.Message);
            }
        }
        finally { if (_seekCancellation == cancellation) _seekCancellation = null; }
    }

    [RelayCommand] private void SetStartHere() { if (CanEdit) SetStartIndex(Math.Min(_positionIndex, _endIndex - 1)); }
    [RelayCommand] private void SetEndHere() { if (CanEdit) SetEndIndex(Math.Max(_positionIndex + 1, _startIndex + 1)); }
    [RelayCommand] private void Reset() { if (CanEdit && _document != null) { SetStartIndex(0); SetEndIndex(_document.FrameCount); _ = SetPositionIndex(0); } }
    [RelayCommand] private void OpenOutput() { if (HasOutput) ShareX.HelpersLib.FileHelpers.OpenFolderWithFile(OutputFilePath); }
    [RelayCommand]
    private async Task TogglePlayAsync()
    {
        if (!CanPlay || _document == null) return;
        if (IsPlaying) StopPlayback();
        else
        {
            if (_positionIndex < _startIndex || _positionIndex >= _endIndex - 1)
                await SetPositionIndex(_startIndex);
            if (!CanPlay) return;
            IsPlaying = true;
            ScheduleNextFrame();
        }
    }

    private async void OnPlayTimerTick(object? sender, EventArgs e)
    {
        _playTimer.Stop();
        if (_document == null || !IsPlaying) return;
        if (_positionIndex + 1 >= _endIndex) { StopPlayback(); return; }
        await SetPositionIndex(_positionIndex + 1);
        if (IsPlaying) ScheduleNextFrame();
    }

    private void ScheduleNextFrame()
    {
        if (_document == null) return;
        _playTimer.Stop();
        _playTimer.Interval = TimeSpan.FromSeconds(_document.FrameEnd(_positionIndex) - Position);
        _playTimer.Start();
    }

    private void StopPlayback()
    {
        _playTimer.Stop();
        IsPlaying = false;
    }

    [RelayCommand]
    private void Cancel() => _exportCancellation?.Cancel();

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (!CanTrim || SelectOutputRequested == null || _disposed || _document == null) return;
        bool reencode = Reencode;
        if (!reencode && !_document.CanCopySelection(_startIndex, _endIndex))
        {
            ShowErrorRequested?.Invoke(Strings.AnimatedGifTrimmer_UseReencode);
            return;
        }
        string? output = await SelectOutputRequested(Path.GetFileNameWithoutExtension(InputFilePath) + "-trimmed.gif");
        if (output == null || _disposed) return;
        StopPlayback();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _exportCancellation = cancellation;
        IsExporting = true;
        Progress = 0;
        StatusText = Strings.AnimatedGifTrimmer_Exporting;
        string? error = null;
        try
        {
            AnimatedGifTrimmerDocument document = _document;
            int first = _startIndex, end = _endIndex;
            IProgress<double> progress = new Progress<double>(value =>
            {
                if (cancellation.IsCancellationRequested || _disposed) return;
                Dispatcher.UIThread.Post(() =>
                {
                    if (_exportCancellation == cancellation && !cancellation.IsCancellationRequested && !_disposed)
                        Progress = Math.Max(Progress, Math.Clamp(value, 0, 100));
                });
            });
            bool saved = await Task.Run(() => document.Export(output, first, end, reencode, progress, cancellation.Token), cancellation.Token);
            if (saved)
            {
                OutputFilePath = output;
                Progress = 100;
                StatusText = string.Format(Strings.VideoTrimmer_Saved, output);
                _playNotificationSound?.Invoke();
            }
            else
            {
                Progress = 0;
                StatusText = error = Strings.AnimatedGifTrimmer_UseReencode;
            }
        }
        catch (OperationCanceledException) { StatusText = Strings.VideoTrimmer_Cancelled; }
        catch (Exception ex) { StatusText = error = ex.Message; }
        finally
        {
            _exportCancellation = null;
            IsExporting = false;
        }
        if (error != null && !_disposed) ShowErrorRequested?.Invoke(error);
    }

    private void NotifyDocumentChanged()
    {
        OnPropertyChanged(nameof(HasGif));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanPlay));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
        OnPropertyChanged(nameof(Duration));
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(Start));
        OnPropertyChanged(nameof(End));
        OnPropertyChanged(nameof(DurationText));
        OnPropertyChanged(nameof(PositionText));
        OnPropertyChanged(nameof(FrameText));
        NotifySelectionChanged();
    }

    private void ClearDocument()
    {
        Bitmap? preview = Preview;
        bool previewIsThumbnail = _thumbnails.Any(x => ReferenceEquals(x.Image, preview));
        Preview = null;
        foreach (var thumbnail in _thumbnails) thumbnail.Image.Dispose();
        _thumbnails.Clear();
        if (!previewIsThumbnail) preview?.Dispose();
        _document?.Dispose();
        _document = null;
        _positionIndex = _startIndex = _endIndex = 0;
        OnPropertyChanged(nameof(Thumbnails));
        NotifyDocumentChanged();
    }

    partial void OnInputFilePathChanged(string value) => OnPropertyChanged(nameof(InputDisplay));
    partial void OnOutputFilePathChanged(string value) => OnPropertyChanged(nameof(HasOutput));
    partial void OnReencodeChanged(bool value)
    {
        OnPropertyChanged(nameof(Lossless));
        NotifySelectionChanged();
    }
    partial void OnIsPlayingChanged(bool value) => OnPropertyChanged(nameof(PlayActionText));
    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsWorking));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanPlay));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
    }
    partial void OnIsExportingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsWorking));
        OnPropertyChanged(nameof(CanBrowse));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanPlay));
        OnPropertyChanged(nameof(CanTrim));
        OnPropertyChanged(nameof(CanUsePrimaryAction));
        OnPropertyChanged(nameof(PrimaryActionCommand));
        OnPropertyChanged(nameof(PrimaryActionText));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StopPlayback();
        _lifetime.Cancel();
        _loadCancellation?.Cancel();
        _seekCancellation?.Cancel();
        _exportCancellation?.Cancel();
        ClearDocument();
        _lifetime.Dispose();
    }
}
