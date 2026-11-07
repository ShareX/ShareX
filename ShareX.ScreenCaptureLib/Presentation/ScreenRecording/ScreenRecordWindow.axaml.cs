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

using ShareX.AvaloniaUI.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Platform;
using ShareX.Icons;
using ShareX.ScreenCaptureLib.Localization;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
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
    private readonly PixelPoint _initialPosition;
    private readonly DispatcherTimer _refreshTimer;
    // Avalonia's tray icon: the notification area on Windows, the menu bar on macOS, StatusNotifierItem on Linux.
    private readonly NativeMenu _trayMenu;
    private readonly NativeMenuItem _trayStartItem;
    private readonly NativeMenuItem _trayPauseItem;
    private readonly NativeMenuItem _trayRestartItem;
    private readonly NativeMenuItem _trayAbortItem;
    private readonly Dictionary<NativeMenuItem, string> _trayMenuIconKeys = new();
    private readonly TrayIcon _trayIcon;
    private readonly IDisposable _trayIconRegistration;

    private volatile ScreenRecordingStatus _status;
    private readonly ScreenRecordWindowViewModel _lifetime = new();
    private bool _dragging;
    private PixelPoint _dragPointerOrigin;
    private PixelPoint _dragWindowOrigin;
    private double _windowScaling = 1;
    private int _frameLeftPixels;
    private RecordingFrameLayout _frameLayout;
    private int _lastIconStatus = -1;
    private int _restartRequested;
    private bool _configuringGeometry;
    private bool _activateWindow = true;
    private bool _showRecordingTimer = true;
    private bool _showRecordingButtonLabels = true;

    public event Action? StopRequested;
    public event Action? PauseRequested;
    public event Action? ResumeRequested;
    public bool UseInProcessPause { get; set; }
    public bool ExcludeFromCapture { get; set; }

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
            if (IsDisposed) return;
            _activateWindow = value;
            ShowActivated = value;
        }
    }

    public float Duration { get; set; }
    public bool AskConfirmationOnAbort { get; set; }
    public bool IsDisposed => _lifetime.IsClosed;

    public bool ShowRecordingTimer
    {
        get => _showRecordingTimer;
        set
        {
            if (IsDisposed) return;
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
            if (IsDisposed) return;
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
        _initialPosition = new PixelPoint(regionRectangle.X - BorderPixels, regionRectangle.Y - BorderPixels);

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Position = _initialPosition;

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

        SetTrayMenuIcon(_trayStartItem, AppIcons.CirclePlay);
        SetTrayMenuIcon(_trayPauseItem, AppIcons.Pause);
        SetTrayMenuIcon(_trayRestartItem, AppIcons.Reset);
        SetTrayMenuIcon(_trayAbortItem, AppIcons.Close);

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
        // A tray icon created at run time only appears once the application knows about it (#8875).
        _trayIconRegistration = DesktopServices.RegisterTrayIcon(_trayIcon);

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
        ScalingChanged += OnScalingChanged;
        Closed += OnClosed;

        ConfigureGeometry(1);
        UpdateTimer();
        ChangeState(ScreenRecordState.Waiting);
    }

    public void StartStopRecording()
    {
        if (IsDisposed) return;
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
            if (UseInProcessPause) OnStopRequested();
            _lifetime.TryRun(() => RecordResetEvent.Set());
        }
        else
        {
            _lifetime.TryRun(() => RecordResetEvent.Set());
        }
    }

    public void PauseResumeRecording()
    {
        if (IsDisposed) return;
        if (Status == ScreenRecordingStatus.Recording)
        {
            Status = ScreenRecordingStatus.Paused;
            _lifetime.TryRun(() => RecordResetEvent.Reset());
            if (UseInProcessPause)
            {
                PauseRequested?.Invoke();
                StopRecordingTimer();
                UpdateUI();
            }
            else OnStopRequested();
        }
        else if (Status == ScreenRecordingStatus.Paused && UseInProcessPause)
        {
            ResumeRequested?.Invoke();
            Status = ScreenRecordingStatus.Recording;
            Timer.Start();
            _refreshTimer.Start();
            UpdateTimer();
            UpdateUI();
        }
        else
        {
            _lifetime.TryRun(() => RecordResetEvent.Set());
        }
    }

    public void AbortRecording()
    {
        if (IsDisposed) return;
        ShowAbortConfirmation(false);
        Status = ScreenRecordingStatus.Aborted;
        OnStopRequested();
        _lifetime.TryRun(() => RecordResetEvent.Set());
    }

    public void RestartRecording()
    {
        if (IsDisposed) return;
        if (Status is not (ScreenRecordingStatus.Recording or ScreenRecordingStatus.Paused))
        {
            return;
        }

        bool stopActiveRecording = Status == ScreenRecordingStatus.Recording || (UseInProcessPause && Status == ScreenRecordingStatus.Paused);

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

        _lifetime.TryRun(() => RecordResetEvent.Set());
    }

    public bool ConsumeRestartRequest() => Interlocked.Exchange(ref _restartRequested, 0) != 0;

    public bool RestartRequested => Volatile.Read(ref _restartRequested) != 0;

    public void StartCountdown(int milliseconds)
    {
        if (IsDisposed) return;
        IsCountdown = true;
        Countdown = TimeSpan.FromMilliseconds(milliseconds);
        Timer.Start();
        _refreshTimer.Start();
        UpdateTimer();
    }

    public void StartRecordingTimer()
    {
        if (IsDisposed) return;
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
        if (IsDisposed) return;
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
                    SetTrayIcon(TrayIconImage.CreateIconBytes(AppIcons.VideoOff, DrawingColor.Gold));
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
                    SetTrayIcon(TrayIconImage.CreateIconBytes(AppIcons.FileVideo));
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

            byte[] icon;

            if (progress >= 0)
            {
                try
                {
                    icon = Helpers.GetProgressIconBytes(progress, DrawingColor.FromArgb(140, 0, 36));
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    progress = -1;
                    if (_lastIconStatus == progress)
                    {
                        return;
                    }
                    icon = TrayIconImage.CreateIconBytes(AppIcons.FileVideo);
                }
            }
            else
            {
                icon = TrayIconImage.CreateIconBytes(AppIcons.FileVideo);
            }

            SetTrayIcon(icon);
            _lastIconStatus = progress;
        });
    }

    public void InvokeSafe(Action action)
    {
        _lifetime.Dispatch(callback =>
        {
            if (Dispatcher.UIThread.CheckAccess()) callback();
            else Dispatcher.UIThread.InvokeAsync(callback).GetAwaiter().GetResult();
        }, action);
    }

    public void Dispose()
    {
        if (!_lifetime.TryClose()) return;
        _dragging = false;
        Timer.Stop();
        _refreshTimer.Stop();
        _trayIcon.IsVisible = false;
        _trayIcon.Clicked -= OnTrayIconClicked;
        _trayIconRegistration.Dispose();
        DisposeTrayMenuIcons();
        _trayIcon.Dispose();
        RecordResetEvent.Dispose();
        GC.SuppressFinalize(this);
    }

    private void UpdateTimer()
    {
        if (IsDisposed)
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
        if (IsDisposed) return;
        ShowAbortConfirmation(false);

        switch (Status)
        {
            case ScreenRecordingStatus.Working:
                SetTrayText("ShareX - " + Strings.ScreenRecordForm_StartRecording_Click_tray_icon_to_stop_recording_);
                SetTrayIcon(TrayIconImage.CreateIconBytes(AppIcons.Video, DrawingColor.Red));
                StartText.Text = Strings.ScreenRecordForm_Stop;
                StartIcon.Icon = AppIcons.Stop;
                _trayStartItem.Header = Strings.ScreenRecordForm_Stop;
                SetTrayMenuIcon(_trayStartItem, AppIcons.Stop);
                SetTrayMenuIcon(_trayPauseItem, AppIcons.Pause);
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
                SetTrayIcon(TrayIconImage.CreateIconBytes(AppIcons.VideoOff, DrawingColor.Gold));
                StartText.Text = paused ? Strings.ScreenRecordForm_Stop : Strings.ScreenRecordForm_Start;
                StartIcon.Icon = paused ? AppIcons.Stop : AppIcons.CirclePlay;
                _trayStartItem.Header = StartText.Text;
                SetTrayMenuIcon(_trayStartItem, paused ? AppIcons.Stop : AppIcons.CirclePlay);
                PauseText.Text = Strings.Resume;
                PauseIcon.Icon = AppIcons.Play;
                _trayPauseItem.Header = Strings.Resume;
                SetTrayMenuIcon(_trayPauseItem, AppIcons.Play);
                TimerDragHandle.Cursor = new Cursor(StandardCursorType.SizeAll);
                RestartButton.IsEnabled = paused;
                _trayRestartItem.IsEnabled = paused;
                SetRecordingAccent(Brushes.Goldenrod);
                break;

            case ScreenRecordingStatus.Recording:
                SetTrayIcon(TrayIconImage.CreateIconBytes(AppIcons.Video, DrawingColor.Red));
                StartText.Text = Strings.ScreenRecordForm_Stop;
                StartIcon.Icon = AppIcons.Stop;
                _trayStartItem.Header = Strings.ScreenRecordForm_Stop;
                SetTrayMenuIcon(_trayStartItem, AppIcons.Stop);
                PauseText.Text = Strings.Pause;
                PauseIcon.Icon = AppIcons.Pause;
                _trayPauseItem.Header = Strings.Pause;
                SetTrayMenuIcon(_trayPauseItem, AppIcons.Pause);
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
        if (IsDisposed) return;
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
        if (IsDisposed) return;
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
        if (IsDisposed) return;
        double scaling = GetScreenScaling(_initialPosition);
        ConfigureGeometry(scaling);
        Helpers.EnsureWindowPosition(this, _initialPosition.X - _frameLeftPixels, _initialPosition.Y);
        ApplyToolWindowStyle();
        ApplyNativeWindowRegion();
        RunOnUIThread(ApplyNativeWindowRegion, DispatcherPriority.Loaded);

        if (ActivateWindow)
        {
            Activate();
        }
    }

    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (IsDisposed || _configuringGeometry)
        {
            return;
        }

        double scaling = GetRenderScaling();
        if (Math.Abs(scaling - _windowScaling) > 0.001)
        {
            ConfigureGeometry(scaling);
        }
    }

    private void ConfigureGeometry(double scaling)
    {
        if (IsDisposed) return;
        _configuringGeometry = true;

        try
        {
            DrawingRectangle region = RecordingRegion;
            _frameLayout = FrameWindowShape.CreateRecordingLayout(
                new PlatformRectangle(region.X, region.Y, region.Width, region.Height), scaling,
                DisplayedToolbarWidth, ToolbarHeight, BorderPixels, ToolbarGapPixels);
            _windowScaling = _frameLayout.Scaling;
            _frameLeftPixels = _frameLayout.Frame.X;

            if (Position != _frameLayout.Position)
            {
                Position = _frameLayout.Position;
            }

            var frameSize = FrameWindowShape.GetLogicalSize(_frameLayout.Frame.Width, _frameLayout.Frame.Height, _windowScaling);
            Avalonia.Controls.Canvas.SetLeft(RegionBorder, _frameLayout.Frame.X / _windowScaling);
            RegionBorder.Width = frameSize.Width;
            RegionBorder.Height = frameSize.Height;
            Avalonia.Controls.Canvas.SetLeft(Toolbar, _frameLayout.Toolbar.X / _windowScaling);
            Avalonia.Controls.Canvas.SetTop(Toolbar, _frameLayout.Toolbar.Y / _windowScaling);

            Width = _frameLayout.Width;
            Height = _frameLayout.Height;

            if (IsVisible)
            {
                RunOnUIThread(ApplyNativeWindowRegion, DispatcherPriority.Loaded);
            }
        }
        finally
        {
            _configuringGeometry = false;
        }
    }

    private double GetRenderScaling() => WindowScaling.GetPositionScaling(RenderScaling);

    /// <summary>The scale of the screen at a point (ShareX #8894), in window position units (1 on macOS, see WindowScaling).</summary>
    private double GetScreenScaling(PixelPoint point) =>
        WindowScaling.GetPositionScaling(Screens.ScreenFromPoint(point)?.Scaling ?? Screens.Primary?.Scaling ?? 1);

    private void OnScalingChanged(object? sender, EventArgs e)
    {
        // A border or wide toolbar can put the window origin on a different monitor.
        // Use the scale actually rendering this window, after the native change settles.
        RunOnUIThread(() => ConfigureGeometry(GetRenderScaling()), DispatcherPriority.Loaded);
    }

    private void ApplyToolWindowStyle()
    {
        FrameWindowShape.SetOverlayStyle(this, clickThrough: false);
        if (ExcludeFromCapture && !FrameWindowShape.ExcludeFromCapture(this))
        {
            DebugHelper.WriteLine("Could not exclude the recording controls from capture.");
        }
    }

    private void ApplyNativeWindowRegion()
    {
        if (IsDisposed) return;
        FrameWindowShape.Apply(this, FrameWindowShape.Create(_frameLayout.Frame, BorderPixels, _frameLayout.Toolbar));
    }

    private void OnTimerPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsDisposed) return;
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
        if (IsDisposed) return;
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
        if (IsDisposed) return;
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

    private void SetTrayIcon(byte[] icon)
    {
        using MemoryStream stream = new(icon, false);
        _trayIcon.Icon = new WindowIcon(stream);
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

    private void SetTrayMenuIcon(NativeMenuItem item, string icon)
    {
        DrawingColor color = ThemeManager.IsDarkTheme ? DrawingColor.White : DrawingColor.Black;
        string cacheKey = $"{icon}:{color.ToArgb()}";
        if (_trayMenuIconKeys.TryGetValue(item, out string? existing) && existing == cacheKey) return;
        using SKBitmap bitmap = TrayIconImage.CreateImage(icon, color);
        using MemoryStream stream = bitmap.GetStream();
        AvaloniaBitmap replacement = new(stream);
        AvaloniaBitmap? previous = item.Icon as AvaloniaBitmap;
        item.Icon = replacement;
        _trayMenuIconKeys[item] = cacheKey;
        previous?.Dispose();
    }

    private void RefreshTrayMenuIcons()
    {
        bool waitingOrPaused = Status is ScreenRecordingStatus.Waiting or ScreenRecordingStatus.Paused;
        SetTrayMenuIcon(_trayStartItem,
            Status is ScreenRecordingStatus.Working or ScreenRecordingStatus.Recording or ScreenRecordingStatus.Paused
                ? AppIcons.Stop
                : AppIcons.CirclePlay);
        SetTrayMenuIcon(_trayPauseItem, waitingOrPaused ? AppIcons.Play : AppIcons.Pause);
        SetTrayMenuIcon(_trayRestartItem, AppIcons.Reset);
        SetTrayMenuIcon(_trayAbortItem, AppIcons.Close);
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

    private void RunOnUIThread(Action action)
    {
        _lifetime.Dispatch(callback => Dispatcher.UIThread.Post(callback), action);
    }

    private void RunOnUIThread(Action action, DispatcherPriority priority)
    {
        _lifetime.Dispatch(callback => Dispatcher.UIThread.Post(callback, priority), action);
    }
}
