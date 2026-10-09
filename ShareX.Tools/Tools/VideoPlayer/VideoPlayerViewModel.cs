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
using ShareX.Tools.Localization;

namespace ShareX.Tools;

public sealed partial class VideoPlayerViewModel : ViewModelBase, IDisposable
{
    private readonly WindowsMediaPlayer _player;
    private readonly CancellationTokenSource _lifetime = new();
    private CancellationTokenSource? _loadCancellation;
    private bool _updatingPosition, _seeking, _resumeAfterSeek, _disposed;

    [ObservableProperty] private string _inputFilePath = string.Empty;
    [ObservableProperty] private double _duration;
    [ObservableProperty] private double _position;
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private double _volume = 1;
    [ObservableProperty] private bool _isMuted;
    [ObservableProperty] private string _statusText = Strings.VideoTrimmer_ChooseVideo;

    public VideoPlayerViewModel(WindowsMediaPlayer player)
    {
        _player = player;
        _player.PositionChanged += UpdatePosition;
        _player.IsPlayingChanged += UpdatePlaybackState;
        _player.PlaybackFailed += HandlePlaybackFailure;
    }

    public Func<Task<string?>>? SelectInputRequested { get; set; }
    public bool HasVideo => Duration > 0;
    public bool SoundOff => IsMuted || Volume == 0;
    public string PlayActionText => IsPlaying ? Strings.AnimatedGifTrimmer_Pause : Strings.AnimatedGifTrimmer_Play;
    public string MuteActionText => SoundOff ? Strings.VideoPlayer_Unmute : Strings.VideoPlayer_Mute;
    public string TimeText => $"{FormatTime(Position)} / {FormatTime(Duration)}";
    public string WindowTitle => string.IsNullOrEmpty(InputFilePath)
        ? Strings.VideoPlayer_Title : $"{Path.GetFileName(InputFilePath)} - {Strings.VideoPlayer_Title}";

    private static string FormatTime(double seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Math.Max(0, seconds));
        return time.TotalHours >= 1 ? $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}" : $"{time.Minutes:00}:{time.Seconds:00}";
    }

    [RelayCommand]
    private async Task BrowseAsync()
    {
        if (SelectInputRequested == null || _disposed) return;
        try
        {
            string? path = await SelectInputRequested();
            if (path != null && !_disposed) await LoadInputAsync(path);
        }
        catch (Exception ex) { HandlePlaybackFailure(ex); }
    }

    public async Task LoadInputAsync(string path)
    {
        if (_disposed) return;
        _loadCancellation?.Cancel();
        _seeking = _resumeAfterSeek = false;
        _player.Unload();
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _loadCancellation = cancellation;
        Duration = 0;
        UpdatePosition(0);
        InputFilePath = path;
        StatusText = Strings.VideoPlayer_Loading;
        IsLoading = true;
        try
        {
            if (!File.Exists(path)) throw new FileNotFoundException(Strings.VideoTrimmer_InvalidVideo);
            VideoPlaybackInfo info = await _player.LoadAsync(path, cancellation.Token);
            cancellation.Token.ThrowIfCancellationRequested();
            Duration = info.Duration;
            StatusText = string.Empty;
            _player.Play();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!cancellation.IsCancellationRequested) HandlePlaybackFailure(ex);
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

    [RelayCommand]
    public void TogglePlay()
    {
        if (!HasVideo) return;
        if (_player.IsPlaybackRequested) _player.Pause();
        else _player.Play();
    }

    [RelayCommand] private void StepBackward() { if (HasVideo) _player.StepFrame(false); }
    [RelayCommand] private void StepForward() { if (HasVideo) _player.StepFrame(true); }

    [RelayCommand]
    public void ToggleMute()
    {
        if (SoundOff)
        {
            if (Volume == 0) Volume = 1;
            IsMuted = false;
        }
        else IsMuted = true;
    }

    public void SeekRelative(double seconds)
    {
        if (HasVideo) Position = Math.Clamp(Position + seconds, 0, Duration);
    }

    public void BeginSeek()
    {
        if (!HasVideo || _seeking) return;
        _seeking = true;
        _resumeAfterSeek = _player.IsPlaybackRequested;
        _player.Pause();
    }

    public void EndSeek()
    {
        if (!_seeking) return;
        _seeking = false;
        _player.Seek(Position);
        if (_resumeAfterSeek) _player.Play();
        _resumeAfterSeek = false;
    }

    partial void OnPositionChanged(double value)
    {
        OnPropertyChanged(nameof(TimeText));
        if (_updatingPosition || !HasVideo) return;
        bool resume = !_seeking && _player.IsPlaybackRequested;
        _player.Seek(value);
        if (resume) _player.Play();
    }

    partial void OnDurationChanged(double value)
    {
        OnPropertyChanged(nameof(HasVideo));
        OnPropertyChanged(nameof(TimeText));
    }

    partial void OnInputFilePathChanged(string value) => OnPropertyChanged(nameof(WindowTitle));
    partial void OnIsPlayingChanged(bool value) => OnPropertyChanged(nameof(PlayActionText));

    partial void OnVolumeChanged(double value)
    {
        _player.Volume = value;
        OnPropertyChanged(nameof(SoundOff));
        OnPropertyChanged(nameof(MuteActionText));
    }

    partial void OnIsMutedChanged(bool value)
    {
        _player.IsMuted = value;
        OnPropertyChanged(nameof(SoundOff));
        OnPropertyChanged(nameof(MuteActionText));
    }

    private void UpdatePosition(double position)
    {
        if (_seeking) return;
        _updatingPosition = true;
        try { Position = position; }
        finally { _updatingPosition = false; }
    }

    private void UpdatePlaybackState(bool playing) => IsPlaying = playing;

    private void HandlePlaybackFailure(Exception error)
    {
        if (_disposed) return;
        _seeking = _resumeAfterSeek = false;
        _player.Unload();
        Duration = 0;
        UpdatePosition(0);
        StatusText = string.Format(Strings.VideoTrimmer_PlaybackError, error.Message);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lifetime.Cancel();
        _player.PositionChanged -= UpdatePosition;
        _player.IsPlayingChanged -= UpdatePlaybackState;
        _player.PlaybackFailed -= HandlePlaybackFailure;
        _player.Dispose();
        _lifetime.Dispose();
    }
}