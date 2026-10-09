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

using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Tools.Localization;
using System.ComponentModel;
using WindowState = Avalonia.Controls.WindowState;

namespace ShareX.Tools;

public partial class VideoPlayerWindow : Window
{
    private readonly VideoPlayerViewModel _viewModel;
    private readonly WindowsMediaPlayer _player;
    private readonly Popup _controlsPopup;
    private readonly Border _controls;
    private readonly Border _viewport;
    private readonly DispatcherTimer _controlsTimer;
    private WindowState _previousWindowState;
    private PixelPoint _lastPointer;
    private long _lastInteraction = Environment.TickCount64;
    private bool _seeking, _dialogOpen, _closed, _keyboardNavigation;

    public VideoPlayerWindow() : this(null) { }

    public VideoPlayerWindow(string? inputFilePath)
    {
        AvaloniaXamlLoader.Load(this);
        _player = this.FindControl<WindowsMediaPlayer>("Player")!;
        _viewModel = new(_player);
        _player.VideoClicked += OnVideoClicked;
        DataContext = _viewModel;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _controlsPopup = this.FindControl<Popup>("ControlsPopup")!;
        _controls = this.FindControl<Border>("PlaybackControls")!;
        _viewport = this.FindControl<Border>("VideoViewport")!;
        _viewModel.SelectInputRequested = SelectInputAsync;
        _viewModel.PropertyChanged += OnPlaybackPropertyChanged;
        _controlsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _controlsTimer.Tick += (_, _) => UpdateControls();
        _viewport.SizeChanged += (_, _) => PositionControls();
        Activated += (_, _) => ShowControls();
        Deactivated += (_, _) => UpdateControls();
        AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        _controls.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        _controls.AddHandler(GotFocusEvent, (_, e) =>
        {
            _keyboardNavigation = e.NavigationMethod is NavigationMethod.Tab or NavigationMethod.Directional;
            ShowControls();
        });
        _controls.AddHandler(PointerPressedEvent, (_, _) =>
        {
            _keyboardNavigation = false;
            ShowControls();
        }, RoutingStrategies.Tunnel);
        var seekSlider = this.FindControl<Slider>("SeekSlider")!;
        seekSlider.AddHandler(PointerPressedEvent, (_, e) =>
        {
            if (!e.GetCurrentPoint(seekSlider).Properties.IsLeftButtonPressed) return;
            _seeking = true;
            _viewModel.BeginSeek();
        }, RoutingStrategies.Tunnel);
        seekSlider.AddHandler(PointerReleasedEvent, (_, _) => EndSeek(), RoutingStrategies.Bubble, handledEventsToo: true);
        seekSlider.PointerCaptureLost += (_, _) => EndSeek();
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Opened += async (_, _) =>
        {
            PositionControls();
            ShowControls();
            _controlsTimer.Start();
            if (!string.IsNullOrEmpty(inputFilePath)) await _viewModel.LoadInputAsync(inputFilePath);
        };
        Closed += (_, _) =>
        {
            _closed = true;
            _controlsTimer.Stop();
            _controlsPopup.IsOpen = false;
            _player.VideoClicked -= OnVideoClicked;
            _viewModel.PropertyChanged -= OnPlaybackPropertyChanged;
            _viewModel.Dispose();
        };
    }

    private void OnVideoClicked()
    {
        _keyboardNavigation = false;
        _viewModel.TogglePlay();
        ShowControls();
    }

    private void PositionControls()
    {
        _controls.Width = Math.Max(0, Math.Min(680, _viewport.Bounds.Width - 24));
        // The nonempty anchor strip ends 12 pixels above the bottom of the viewport.
        _controlsPopup.PlacementRect = new Rect(0, Math.Max(0, _viewport.Bounds.Height - 13), _viewport.Bounds.Width, 1);
    }

    private void ShowControls()
    {
        _lastInteraction = Environment.TickCount64;
        UpdateControls();
    }

    private void UpdateControls()
    {
        if (_closed) return;
        // Pointer events over the native video HWND do not reach Avalonia.
        if (NativeMethods.GetCursorPos(out POINT cursor))
        {
            PixelPoint pointer = new(cursor.X, cursor.Y);
            if (pointer != _lastPointer && new Rect(_viewport.Bounds.Size).Contains(_viewport.PointToClient(pointer)))
                _lastInteraction = Environment.TickCount64;
            _lastPointer = pointer;
        }
        _controlsPopup.IsOpen = IsVisible && IsActive && WindowState != WindowState.Minimized && !_dialogOpen &&
            (!_viewModel.IsPlaying || _seeking || _controls.IsPointerOver || _keyboardNavigation && _controls.IsKeyboardFocusWithin ||
             Environment.TickCount64 - _lastInteraction < 3000);
    }

    private void EndSeek()
    {
        _seeking = false;
        _viewModel.EndSeek();
        ShowControls();
    }

    private void OnPlaybackPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(VideoPlayerViewModel.IsPlaying) or nameof(VideoPlayerViewModel.IsLoading)) ShowControls();
    }

    private async Task<string?> SelectInputAsync()
    {
        _dialogOpen = true;
        UpdateControls();
        try
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.VideoTrimmer_Open,
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType(Strings.VideoTrimmer_VideoFiles)
                    {
                        Patterns = ["*.mp4", "*.mkv", "*.webm", "*.mov", "*.avi", "*.m4v", "*.wmv", "*.ts", "*.mts", "*.m2ts"]
                    },
                    new FilePickerFileType(Strings.VideoTrimmer_AllFiles) { Patterns = ["*"] }
                ]
            });
            return files.FirstOrDefault()?.TryGetLocalPath();
        }
        finally
        {
            _dialogOpen = false;
            ShowControls();
        }
    }

    private void OnFullscreenClick(object? sender, RoutedEventArgs e) => ToggleFullscreen();

    private void ToggleFullscreen()
    {
        if (WindowState == WindowState.FullScreen) WindowState = _previousWindowState;
        else
        {
            _previousWindowState = WindowState == WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
            WindowState = WindowState.FullScreen;
        }
        bool fullscreen = WindowState == WindowState.FullScreen;
        var button = this.FindControl<Button>("FullscreenButton")!;
        string actionText = fullscreen ? Strings.VideoPlayer_ExitFullscreen : Strings.VideoPlayer_Fullscreen;
        ToolTip.SetTip(button, actionText);
        AutomationProperties.SetName(button, actionText);
        this.FindControl<TextBlock>("FullscreenIcon")!.Text = fullscreen ? LucideIcons.minimize : LucideIcons.maximize;
        ShowControls();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled || _dialogOpen) return;
        if (e.Key == Key.Tab)
        {
            ShowControls();
            return;
        }
        if (e.Key == Key.O && e.KeyModifiers == KeyModifiers.Control) _viewModel.BrowseCommand.Execute(null);
        else if (e.Key == Key.F11 || e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.Alt) ToggleFullscreen();
        else if (e.Key == Key.Escape && WindowState == WindowState.FullScreen) ToggleFullscreen();
        else if (e.KeyModifiers == KeyModifiers.None)
        {
            switch (e.Key)
            {
                case Key.Space: _viewModel.TogglePlay(); break;
                case Key.Left: _viewModel.SeekRelative(-5); break;
                case Key.Right: _viewModel.SeekRelative(5); break;
                case Key.Up: _viewModel.Volume = Math.Min(1, _viewModel.Volume + 0.05); break;
                case Key.Down: _viewModel.Volume = Math.Max(0, _viewModel.Volume - 0.05); break;
                case Key.M: _viewModel.ToggleMute(); break;
                default: return;
            }
        }
        else return;
        e.Handled = true;
        ShowControls();
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        string? path = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault()?.TryGetLocalPath();
        if (path == null) return;
        e.Handled = true;
        await _viewModel.LoadInputAsync(path);
    }
}