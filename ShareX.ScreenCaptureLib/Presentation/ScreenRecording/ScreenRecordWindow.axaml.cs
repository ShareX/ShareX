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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Platform;
using ShareX.ScreenCaptureLib.Localization;
using System;
using System.Diagnostics;
using System.Threading;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;
using SkiaSharp;

namespace ShareX.ScreenCaptureLib;

public partial class ScreenRecordWindow : Window, IDisposable
{
    private const int BorderPixels = 1;
    private const int ToolbarGapPixels = 3;
    private const double TimerWidth = 112;
    private const double ActionButtonWidth = 86;
    private const double CompactActionButtonWidth = 40;
    private const int ActionButtonCount = 4;
    private const double ToolbarWidth = 461;
    private const double ToolbarHeight = 42;

    private readonly int _captureWidth;
    private readonly int _captureHeight;
    private readonly DispatcherTimer _refreshTimer;
    // Avalonia's tray icon: the notification area on Windows, the menu bar on macOS, StatusNotifierItem on Linux.
    private readonly NativeMenu _trayMenu;
    private readonly NativeMenuItem _trayStartItem;
    private readonly NativeMenuItem _trayPauseItem;
    private readonly NativeMenuItem _trayRestartItem;
    private readonly NativeMenuItem _trayAbortItem;
    private readonly TrayIcon _trayIcon;

    private volatile ScreenRecordingStatus _status;
    private volatile bool _disposed;
    private bool _dragging;
    private PixelPoint _dragPointerOrigin;
    private PixelPoint _dragWindowOrigin;
    private double _windowScaling = 1;
    private int _frameLeftPixels;
    private int _toolbarLeftPixels;
    private int _lastIconStatus = -1;
    private int _restartRequested;
    private bool _configuringGeometry;
    private bool _activateWindow = true;
    private bool _showRecordingTimer = true;
    private bool _showRecordingButtonLabels = true;

    public event Action? StopRequested;

    public ScreenRecordingStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    public TimeSpan Countdown { get; set; }
    public bool IsCountdown { get; private set; }
    public Stopwatch Timer { get; } = new();
    public ManualResetEvent RecordResetEvent { get; } = new(false);

    public bool ActivateWindow
    {
        get => _activateWindow;
        set
        {
            _activateWindow = value;
            ShowActivated = value;
        }
    }

    public float Duration { get; set; }
    public bool AskConfirmationOnAbort { get; set; }
    public bool IsDisposed => _disposed;

    public bool ShowRecordingTimer
    {
        get => _showRecordingTimer;
        set
        {
            if (_showRecordingTimer == value)
            {
                return;
            }

            _showRecordingTimer = value;
            TimerDragHandle.IsVisible = value;
            RecorderControls.ColumnDefinitions[0].Width = new GridLength(value ? TimerWidth : 0);
            Toolbar.Width = CurrentToolbarWidth;
            ConfigureGeometry(_windowScaling);
        }
    }

    private double CurrentToolbarWidth => ToolbarWidth - TimerWidth +
        (ShowRecordingTimer ? TimerWidth : 0) -
        (ShowRecordingButtonLabels ? 0 : (ActionButtonWidth - CompactActionButtonWidth) * ActionButtonCount);

    private double DisplayedToolbarWidth => AbortConfirmation.IsVisible ? ToolbarWidth : CurrentToolbarWidth;

    public bool ShowRecordingButtonLabels
    {
        get => _showRecordingButtonLabels;
        set
        {
            if (_showRecordingButtonLabels == value)
            {
                return;
            }

            _showRecordingButtonLabels = value;
            StartText.IsVisible = value;
            PauseText.IsVisible = value;
            RestartText.IsVisible = value;
            AbortText.IsVisible = value;
            UpdateActionToolTips();

            double buttonWidth = value ? ActionButtonWidth : CompactActionButtonWidth;
            foreach (Button button in new[] { StartButton, PauseButton, RestartButton, AbortButton })
            {
                button.Width = buttonWidth;
                button.MinWidth = buttonWidth;
            }

            Toolbar.Width = DisplayedToolbarWidth;
            ConfigureGeometry(_windowScaling);
        }
    }

    public DrawingRectangle RecordingRegion => new(
        Position.X + _frameLeftPixels + BorderPixels,
        Position.Y + BorderPixels,
        _captureWidth,
        _captureHeight);

    public ScreenRecordWindow()
        : this(new DrawingRectangle(0, 0, 640, 360))
    {
    }

    public ScreenRecordWindow(DrawingRectangle regionRectangle)
    {
        _captureWidth = regionRectangle.Width;
        _captureHeight = regionRectangle.Height;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Position = new PixelPoint(regionRectangle.X - BorderPixels, regionRectangle.Y - BorderPixels);

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _refreshTimer.Tick += (_, _) => UpdateTimer();

        _trayStartItem = new NativeMenuItem(Strings.ScreenRecordForm_Start);
        _trayPauseItem = new NativeMenuItem(Strings.Pause);
        _trayRestartItem = new NativeMenuItem(Strings.ScreenRecordWindow_Restart);
        _trayAbortItem = new NativeMenuItem(Strings.ScreenRecordWindow_Abort);
        _trayStartItem.Click += (_, _) => RunOnUIThread(StartStopRecording);
        _trayPauseItem.Click += (_, _) => RunOnUIThread(PauseResumeRecording);
        _trayRestartItem.Click += (_, _) => RunOnUIThread(RestartRecording);
        _trayAbortItem.Click += (_, _) => RunOnUIThread(RequestAbortRecording);

        SetTrayMenuIcon(_trayStartItem, LucideIcons.circle_play);
        SetTrayMenuIcon(_trayPauseItem, LucideIcons.pause);
        SetTrayMenuIcon(_trayRestartItem, LucideIcons.rotate_ccw);
        SetTrayMenuIcon(_trayAbortItem, LucideIcons.x);

        _trayMenu = new NativeMenu();
        _trayMenu.Items.Add(_trayStartItem);
        _trayMenu.Items.Add(_trayPauseItem);
        _trayMenu.Items.Add(_trayRestartItem);
        _trayMenu.Items.Add(_trayAbortItem);
        _trayMenu.Opening += (_, _) => RefreshTrayMenuIcons();

        _trayIcon = new TrayIcon
        {
            Menu = _trayMenu,
            ToolTipText = "ShareX",
            IsVisible = false
        };
        _trayIcon.Clicked += OnTrayIconClicked;

        StartButton.Click += (_, _) => StartStopRecording();
        PauseButton.Click += (_, _) => PauseResumeRecording();
        RestartButton.Click += (_, _) => RestartRecording();
        AbortButton.Click += (_, _) => RequestAbortRecording();
        CancelAbortButton.Click += (_, _) => ShowAbortConfirmation(false);
        ConfirmAbortButton.Click += (_, _) => AbortRecording();

        TimerDragHandle.PointerPressed += OnTimerPointerPressed;
        TimerDragHandle.PointerMoved += OnTimerPointerMoved;
        TimerDragHandle.PointerReleased += OnTimerPointerReleased;
        TimerDragHandle.PointerCaptureLost += (_, _) => _dragging = false;

        Opened += OnOpened;
        PositionChanged += OnPositionChanged;
        Closed += OnClosed;

        ConfigureGeometry(1);
        UpdateTimer();
        ChangeState(ScreenRecordState.Waiting);
    }

    public void StartStopRecording()
    {
        if (Status == ScreenRecordingStatus.Working)
        {
            AbortRecording();
        }
        else if (Status == ScreenRecordingStatus.Recording)
        {
            Status = ScreenRecordingStatus.Stopped;
            OnStopRequested();
        }
        else if (Status == ScreenRecordingStatus.Paused)
        {
            Status = ScreenRecordingStatus.Stopped;
            RecordResetEvent.Set();
        }
        else
        {
            RecordResetEvent.Set();
        }
    }

    public void PauseResumeRecording()
    {
        if (Status == ScreenRecordingStatus.Recording)
        {
            Status = ScreenRecordingStatus.Paused;
            RecordResetEvent.Reset();
            OnStopRequested();
        }
        else
        {
            RecordResetEvent.Set();
        }
    }

    public void AbortRecording()
    {
        ShowAbortConfirmation(false);
        Status = ScreenRecordingStatus.Aborted;
        OnStopRequested();
        RecordResetEvent.Set();
    }

    public void RestartRecording()
    {
        if (Status is not (ScreenRecordingStatus.Recording or ScreenRecordingStatus.Paused))
        {
            return;
        }

        bool stopActiveRecording = Status == ScreenRecordingStatus.Recording;

        Interlocked.Exchange(ref _restartRequested, 1);
        Status = ScreenRecordingStatus.Waiting;
        Timer.Reset();
        IsCountdown = false;
        StopRecordingTimer();
        UpdateUI();

        if (stopActiveRecording)
        {
            OnStopRequested();
        }

        RecordResetEvent.Set();
    }

    public bool ConsumeRestartRequest() => Interlocked.Exchange(ref _restartRequested, 0) != 0;

    public bool RestartRequested => Volatile.Read(ref _restartRequested) != 0;

    public void StartCountdown(int milliseconds)
    {
        IsCountdown = true;
        Countdown = TimeSpan.FromMilliseconds(milliseconds);
        Timer.Start();
        _refreshTimer.Start();
        UpdateTimer();
    }

    public void StartRecordingTimer()
    {
        if (IsCountdown)
        {
            Timer.Reset();
            IsCountdown = false;
        }

        if (Duration > 0)
        {
            IsCountdown = true;
            Countdown = TimeSpan.FromSeconds(Duration);
        }

        SetRecordingAccent(Brushes.Red);
        Timer.Start();
        _refreshTimer.Start();
        UpdateTimer();
    }

    public void StopRecordingTimer()
    {
        Timer.Stop();
        _refreshTimer.Stop();
        UpdateTimer();
    }

    public void ChangeState(ScreenRecordState state)
    {
        InvokeSafe(() =>
        {
            switch (state)
            {
                case ScreenRecordState.Waiting:
                    SetTrayText("ShareX - " + Strings.ScreenRecordForm_StartRecording_Waiting___);
                    SetTrayIcon(LucideIcons.video_off, SKColors.Gold);
                    SetTrayMenuEnabled(false);
                    _trayIcon.IsVisible = true;
                    break;
                case ScreenRecordState.BeforeStart:
                    SetTrayMenuEnabled(true);
                    UpdateUI();
                    break;
                case ScreenRecordState.AfterStart:
                    _dragging = false;
                    Status = ScreenRecordingStatus.Working;
                    UpdateUI();
                    break;
                case ScreenRecordState.AfterRecordingStart:
                    Status = ScreenRecordingStatus.Recording;
                    StartRecordingTimer();
                    UpdateUI();
                    break;
                case ScreenRecordState.RecordingEnd:
                    StopRecordingTimer();
                    UpdateUI();
                    break;
                case ScreenRecordState.Encoding:
                    Hide();
                    SetTrayMenuEnabled(false);
                    SetTrayText("ShareX - " + Strings.ScreenRecordForm_StartRecording_Encoding___);
                    SetTrayIcon(LucideIcons.file_video_camera, GetTrayGlyphColor());
                    break;
            }
        });
    }

    public void ChangeStateProgress(int progress)
    {
        InvokeSafe(() =>
        {
            SetTrayText($"ShareX - {Strings.ScreenRecordForm_StartRecording_Encoding___} {progress}%");

            if (!_trayIcon.IsVisible || _lastIconStatus == progress)
            {
                return;
            }

            if (progress >= 0)
            {
                SetTrayIcon(TrayIconRenderer.CreateProgressWindowIcon(progress, new SKColor(140, 0, 36)));
            }
            else
            {
                SetTrayIcon(LucideIcons.file_video_camera, GetTrayGlyphColor());
            }

            _lastIconStatus = progress;
        });
    }

    public void InvokeSafe(Action action)
    {
        if (_disposed)
        {
            return;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.InvokeAsync(action).GetAwaiter().GetResult();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _refreshTimer.Stop();
        _trayIcon.IsVisible = false;
        _trayIcon.Clicked -= OnTrayIconClicked;
        _trayIcon.Dispose();
        DisposeTrayMenuIcons();
        RecordResetEvent.Dispose();
        GC.SuppressFinalize(this);
    }

    private void UpdateTimer()
    {
        if (_disposed)
        {
            return;
        }

        TimeSpan value;
        if (IsCountdown)
        {
            value = Countdown - Timer.Elapsed;
            if (value < TimeSpan.Zero)
            {
                value = TimeSpan.Zero;
            }
        }
        else
        {
            value = Timer.Elapsed;
        }

        TimerText.Text = value.ToString("mm\\:ss\\:ff");
    }

    private void UpdateUI()
    {
        ShowAbortConfirmation(false);

        switch (Status)
        {
            case ScreenRecordingStatus.Working:
                SetTrayText("ShareX - " + Strings.ScreenRecordForm_StartRecording_Click_tray_icon_to_stop_recording_);
                SetTrayIcon(LucideIcons.video, SKColors.Red);
                StartText.Text = Strings.ScreenRecordForm_Stop;
                StartIcon.Text = LucideIcons.square;
                _trayStartItem.Header = Strings.ScreenRecordForm_Stop;
                SetTrayMenuIcon(_trayStartItem, LucideIcons.square);
                SetTrayMenuIcon(_trayPauseItem, LucideIcons.pause);
                RestartButton.IsEnabled = false;
                _trayRestartItem.IsEnabled = false;
                SetRecordingAccent(Brushes.Goldenrod);
                break;

            case ScreenRecordingStatus.Waiting:
            case ScreenRecordingStatus.Paused:
                bool paused = Status == ScreenRecordingStatus.Paused;
                SetTrayText("ShareX - " + (paused
                    ? Strings.ScreenRecordForm_StartRecording_Click_tray_icon_to_stop_recording_
                    : Strings.ScreenRecordForm_StartRecording_Click_tray_icon_to_start_recording_));
                SetTrayIcon(LucideIcons.video_off, SKColors.Gold);
                StartText.Text = paused ? Strings.ScreenRecordForm_Stop : Strings.ScreenRecordForm_Start;
                StartIcon.Text = paused ? LucideIcons.square : LucideIcons.circle_play;
                _trayStartItem.Header = StartText.Text;
                SetTrayMenuIcon(_trayStartItem, paused ? LucideIcons.square : LucideIcons.circle_play);
                PauseText.Text = Strings.Resume;
                PauseIcon.Text = LucideIcons.play;
                _trayPauseItem.Header = Strings.Resume;
                SetTrayMenuIcon(_trayPauseItem, LucideIcons.play);
                TimerDragHandle.Cursor = new Cursor(StandardCursorType.SizeAll);
                RestartButton.IsEnabled = paused;
                _trayRestartItem.IsEnabled = paused;
                SetRecordingAccent(Brushes.Goldenrod);
                break;

            case ScreenRecordingStatus.Recording:
                SetTrayIcon(LucideIcons.video, SKColors.Red);
                StartText.Text = Strings.ScreenRecordForm_Stop;
                StartIcon.Text = LucideIcons.square;
                _trayStartItem.Header = Strings.ScreenRecordForm_Stop;
                SetTrayMenuIcon(_trayStartItem, LucideIcons.square);
                PauseText.Text = Strings.Pause;
                PauseIcon.Text = LucideIcons.pause;
                _trayPauseItem.Header = Strings.Pause;
                SetTrayMenuIcon(_trayPauseItem, LucideIcons.pause);
                TimerDragHandle.Cursor = Cursor.Default;
                RestartButton.IsEnabled = true;
                _trayRestartItem.IsEnabled = true;
                break;
        }

        UpdateActionToolTips();
    }

    private void UpdateActionToolTips()
    {
        ToolTip.SetTip(StartButton, ShowRecordingButtonLabels ? null : StartText.Text);
        ToolTip.SetTip(PauseButton, ShowRecordingButtonLabels ? null : PauseText.Text);
        ToolTip.SetTip(RestartButton, ShowRecordingButtonLabels ? null : RestartText.Text);
        ToolTip.SetTip(AbortButton, ShowRecordingButtonLabels ? null : AbortText.Text);
    }

    private void RequestAbortRecording()
    {
        if (AskConfirmationOnAbort)
        {
            ShowAbortConfirmation(true);
        }
        else
        {
            AbortRecording();
        }
    }

    private void ShowAbortConfirmation(bool show)
    {
        RecorderControls.IsVisible = !show;
        AbortConfirmation.IsVisible = show;
        Toolbar.Width = DisplayedToolbarWidth;
        ConfigureGeometry(_windowScaling);
    }

    private void OnStopRequested()
    {
        StopRequested?.Invoke();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        double scaling = GetScreenScaling(Position);
        ConfigureGeometry(scaling);
        ApplyToolWindowStyle();
        ApplyNativeWindowRegion();
        Dispatcher.UIThread.Post(ApplyNativeWindowRegion, DispatcherPriority.Loaded);

        if (ActivateWindow)
        {
            Activate();
        }
    }

    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (_disposed || _configuringGeometry)
        {
            return;
        }

        double scaling = GetScreenScaling(Position);
        if (Math.Abs(scaling - _windowScaling) > 0.001)
        {
            ConfigureGeometry(scaling);
        }
    }

    private void ConfigureGeometry(double scaling)
    {
        _configuringGeometry = true;

        try
        {
            int recordingLeft = Position.X + _frameLeftPixels + BorderPixels;

            _windowScaling = Math.Max(0.5, scaling);

            int frameWidthPixels = _captureWidth + BorderPixels * 2;
            int frameHeightPixels = _captureHeight + BorderPixels * 2;
            int toolbarWidthPixels = (int)Math.Ceiling(DisplayedToolbarWidth * _windowScaling);
            int contentWidthPixels = Math.Max(frameWidthPixels, toolbarWidthPixels);

            _frameLeftPixels = (contentWidthPixels - frameWidthPixels) / 2;
            _toolbarLeftPixels = (contentWidthPixels - toolbarWidthPixels) / 2;

            double frameLeft = _frameLeftPixels / _windowScaling;
            double toolbarLeft = _toolbarLeftPixels / _windowScaling;
            double frameWidth = frameWidthPixels / _windowScaling;
            double frameHeight = frameHeightPixels / _windowScaling;
            double gap = ToolbarGapPixels / _windowScaling;

            PixelPoint centeredPosition = new(
                recordingLeft - _frameLeftPixels - BorderPixels,
                Position.Y);

            if (Position != centeredPosition)
            {
                Position = centeredPosition;
            }

            Avalonia.Controls.Canvas.SetLeft(RegionBorder, frameLeft);
            RegionBorder.Width = frameWidth;
            RegionBorder.Height = frameHeight;
            Avalonia.Controls.Canvas.SetLeft(Toolbar, toolbarLeft);
            Avalonia.Controls.Canvas.SetTop(Toolbar, frameHeight + gap);

            Width = contentWidthPixels / _windowScaling;
            Height = frameHeight + gap + ToolbarHeight;

            if (IsVisible)
            {
                Dispatcher.UIThread.Post(ApplyNativeWindowRegion, DispatcherPriority.Loaded);
            }
        }
        finally
        {
            _configuringGeometry = false;
        }
    }

    private double GetScreenScaling(PixelPoint point)
    {
        return Screens.ScreenFromPoint(point)?.Scaling ?? Screens.Primary?.Scaling ?? 1;
    }

    private void ApplyToolWindowStyle()
    {
        FrameWindowShape.SetOverlayStyle(this, clickThrough: false);
    }

    private void ApplyNativeWindowRegion()
    {
        int frameWidth = _captureWidth + BorderPixels * 2;
        int frameHeight = _captureHeight + BorderPixels * 2;
        int toolbarTop = frameHeight + ToolbarGapPixels;
        int toolbarWidth = (int)Math.Ceiling(DisplayedToolbarWidth * _windowScaling);
        int toolbarHeight = (int)Math.Ceiling(ToolbarHeight * _windowScaling);

        FrameWindowShape.Apply(this, FrameWindowShape.Create(
            new PlatformRectangle(_frameLeftPixels, 0, frameWidth, frameHeight),
            BorderPixels,
            new PlatformRectangle(_toolbarLeftPixels, toolbarTop, toolbarWidth, toolbarHeight)));
    }

    private void OnTimerPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Status is not (ScreenRecordingStatus.Waiting or ScreenRecordingStatus.Paused) ||
            !e.GetCurrentPoint(TimerDragHandle).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _dragging = true;
        _dragPointerOrigin = this.PointToScreen(e.GetPosition(this));
        _dragWindowOrigin = Position;
        e.Pointer.Capture(TimerDragHandle);
        e.Handled = true;
    }

    private void OnTimerPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragging || e.Pointer.Captured != TimerDragHandle)
        {
            return;
        }

        PixelPoint pointer = this.PointToScreen(e.GetPosition(this));
        PixelPoint candidate = new(
            _dragWindowOrigin.X + pointer.X - _dragPointerOrigin.X,
            _dragWindowOrigin.Y + pointer.Y - _dragPointerOrigin.Y);
        DrawingRectangle recordingRegion = new(
            candidate.X + _frameLeftPixels + BorderPixels,
            candidate.Y + BorderPixels,
            _captureWidth,
            _captureHeight);

        if (CaptureHelpers.GetScreenBounds().Contains(recordingRegion))
        {
            Position = candidate;
        }
        else
        {
            _dragPointerOrigin = pointer;
            _dragWindowOrigin = Position;
        }

        e.Handled = true;
    }

    private void OnTimerPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured == TimerDragHandle)
        {
            e.Pointer.Capture(null);
        }

        _dragging = false;
        e.Handled = true;
    }

    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        RunOnUIThread(StartStopRecording);
    }

    private void SetRecordingAccent(IBrush brush)
    {
        RegionBorder.AccentBrush = brush;
        StatusIndicator.Background = brush;
    }

    private void SetTrayText(string text)
    {
        _trayIcon.ToolTipText = text.Truncate(63);
    }

    private void SetTrayIcon(string glyph, SKColor color)
    {
        SetTrayIcon(TrayIconRenderer.CreateWindowIcon(glyph, color));
    }

    private void SetTrayIcon(WindowIcon icon)
    {
        _trayIcon.Icon = icon;
    }

    private void SetTrayMenuEnabled(bool enabled)
    {
        foreach (NativeMenuItemBase item in _trayMenu.Items)
        {
            if (item is NativeMenuItem menuItem)
            {
                menuItem.IsEnabled = enabled;
            }
        }
    }

    private static SKColor GetThemeGlyphColor() => ThemeManager.IsDarkTheme ? SKColors.White : SKColors.Black;

    /// <summary>Contrasts with the task bar or panel the tray icon sits on, falling back to ShareX's theme.</summary>
    private static SKColor GetTrayGlyphColor() => PlatformServices.Current.Preferences.SystemUsesLightTheme switch
    {
        true => SKColors.Black,
        false => SKColors.White,
        null => GetThemeGlyphColor()
    };

    private static void SetTrayMenuIcon(NativeMenuItem item, string glyph)
    {
        SKColor color = GetThemeGlyphColor();
        string cacheKey = $"{glyph}:{color}";

        if (item.CommandParameter as string == cacheKey && item.Icon != null)
        {
            return;
        }

        IDisposable? previous = item.Icon as IDisposable;
        item.Icon = TrayIconRenderer.CreateMenuBitmap(glyph, color);
        item.CommandParameter = cacheKey;
        previous?.Dispose();
    }

    private void RefreshTrayMenuIcons()
    {
        bool waitingOrPaused = Status is ScreenRecordingStatus.Waiting or ScreenRecordingStatus.Paused;
        SetTrayMenuIcon(_trayStartItem,
            Status is ScreenRecordingStatus.Working or ScreenRecordingStatus.Recording or ScreenRecordingStatus.Paused
                ? LucideIcons.square
                : LucideIcons.circle_play);
        SetTrayMenuIcon(_trayPauseItem, waitingOrPaused ? LucideIcons.play : LucideIcons.pause);
        SetTrayMenuIcon(_trayRestartItem, LucideIcons.rotate_ccw);
        SetTrayMenuIcon(_trayAbortItem, LucideIcons.x);
    }

    private void DisposeTrayMenuIcons()
    {
        foreach (NativeMenuItemBase item in _trayMenu.Items)
        {
            if (item is NativeMenuItem menuItem && menuItem.Icon is IDisposable icon)
            {
                menuItem.Icon = null;
                icon.Dispose();
            }
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (Status is not (ScreenRecordingStatus.Stopped or ScreenRecordingStatus.Aborted))
        {
            AbortRecording();
        }

        Dispose();
    }

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Post(action);
    }
}
