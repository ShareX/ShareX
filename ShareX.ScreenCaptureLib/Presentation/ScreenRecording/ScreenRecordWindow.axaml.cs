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
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Icons;
using ShareX.ScreenCaptureLib.Localization;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib;

public partial class ScreenRecordWindow : Window, IDisposable
{
    private const int BorderPixels = RegionFrame.BorderPixels;
    private const int ToolbarGapPixels = 3;
    private const double TimerWidth = 112;
    private const double ActionButtonWidth = 86;
    private const double CompactActionButtonWidth = 40;
    private const int ActionButtonCount = 4;
    private const double ToolbarWidth = 461;
    private const double ToolbarHeight = 42;

    private readonly RegionFrame _frame;
    private readonly DispatcherTimer _refreshTimer;
    private readonly NativeMenu _trayMenu;
    private readonly NativeMenuItem _trayStartItem;
    private readonly NativeMenuItem _trayPauseItem;
    private readonly NativeMenuItem _trayRestartItem;
    private readonly NativeMenuItem _trayAbortItem;
    private readonly TrayIcon _trayIcon;
    private readonly IDisposable _trayIconRegistration;

    private volatile ScreenRecordingStatus _status;
    private volatile bool _disposed;
    private readonly Dictionary<NativeMenuItem, string> _trayMenuIconKeys = new();
    private bool _dragging;
    private PixelPoint _dragPointerOrigin;
    private DrawingRectangle _dragRegionOrigin;
    private DrawingRectangle _region;
    private double _windowScaling = 1;
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

    public bool ExcludeFromCapture
    {
        get => _frame.ExcludeFromCapture;
        set => _frame.ExcludeFromCapture = value;
    }

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

    public DrawingRectangle RecordingRegion => _region;

    public ScreenRecordWindow()
        : this(new DrawingRectangle(0, 0, 640, 360))
    {
    }

    public ScreenRecordWindow(DrawingRectangle regionRectangle)
    {
        _region = regionRectangle;
        _frame = new RegionFrame(regionRectangle, Brushes.Goldenrod);

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _frame.Title = Title;

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
        foreach (NativeMenuItem item in new[] { _trayStartItem, _trayPauseItem, _trayRestartItem, _trayAbortItem }) _trayMenu.Items.Add(item);
        _trayMenu.NeedsUpdate += (_, _) => RefreshTrayMenuIcons();

        _trayIcon = new TrayIcon
        {
            Menu = _trayMenu,
            ToolTipText = "ShareX",
            IsVisible = false
        };
        _trayIcon.Clicked += OnTrayIconMouseClick;
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
            if (UseInProcessPause) OnStopRequested();
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
        _trayIcon.Clicked -= OnTrayIconMouseClick;
        _trayIconRegistration.Dispose();
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
        ConfigureGeometry(GetScreenScaling(ToolbarAnchor));
        PixelPoint position = GetToolbarPosition();
        Helpers.EnsureWindowPosition(this, position.X, position.Y);
        ApplyToolWindowStyle();
        _frame.Show();

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

        double scaling = GetScreenScaling(ToolbarAnchor);
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
            _windowScaling = Math.Max(0.5, scaling);

            // The window holds only the toolbar, centered under the region outline.
            Width = DisplayedToolbarWidth;
            Height = ToolbarHeight;

            PixelPoint position = GetToolbarPosition();
            if (Position != position)
            {
                Position = position;
            }
        }
        finally
        {
            _configuringGeometry = false;
        }
    }

    private PixelPoint ToolbarAnchor => new(_region.X + _region.Width / 2, _region.Bottom + BorderPixels + ToolbarGapPixels);

    private PixelPoint GetToolbarPosition()
    {
        int toolbarWidthPixels = (int)Math.Ceiling(DisplayedToolbarWidth * _windowScaling);
        return new PixelPoint(_region.X + (_region.Width - toolbarWidthPixels) / 2, ToolbarAnchor.Y);
    }

    private void SetRegion(DrawingRectangle region)
    {
        _region = region;
        _frame.SetRegion(region);
        // The region may have been dragged onto a monitor with a different DPI.
        ConfigureGeometry(GetScreenScaling(ToolbarAnchor));
    }

    private double GetScreenScaling(PixelPoint point)
    {
        // The toolbar can sit below the bottom edge of the screen when the region reaches it.
        PixelPoint regionCenter = new(_region.X + _region.Width / 2, _region.Y + _region.Height / 2);
        return (Screens.ScreenFromPoint(point) ?? Screens.ScreenFromPoint(regionCenter))?.Scaling ?? Screens.Primary?.Scaling ?? 1;
    }

    private void ApplyToolWindowStyle()
    {
        IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        WindowInfo info = new(handle);
        info.ExStyle |= WindowStyles.WS_EX_TOOLWINDOW;
        if (ExcludeFromCapture && !NativeMethods.SetWindowDisplayAffinity(handle, 0x11)) // WDA_EXCLUDEFROMCAPTURE
        {
            DebugHelper.WriteLine("Could not exclude the recording controls from capture. Win32 error: {0}", Marshal.GetLastWin32Error());
        }
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
        _dragRegionOrigin = _region;
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
        DrawingRectangle recordingRegion = new(
            _dragRegionOrigin.X + pointer.X - _dragPointerOrigin.X,
            _dragRegionOrigin.Y + pointer.Y - _dragPointerOrigin.Y,
            _region.Width,
            _region.Height);

        if (CaptureHelpers.GetScreenBounds().Contains(recordingRegion))
        {
            SetRegion(recordingRegion);
        }
        else
        {
            _dragPointerOrigin = pointer;
            _dragRegionOrigin = _region;
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

    private void OnTrayIconMouseClick(object? sender, EventArgs e) => RunOnUIThread(StartStopRecording);

    private void SetRecordingAccent(IBrush brush)
    {
        _frame.AccentBrush = brush;
        StatusIndicator.Background = brush;
    }

    private void SetTrayMenuEnabled(bool enabled)
    {
        foreach (NativeMenuItem item in _trayMenu.Items)
            item.IsEnabled = enabled && (!ReferenceEquals(item, _trayRestartItem) || Status is ScreenRecordingStatus.Recording or ScreenRecordingStatus.Paused);
    }

    private void SetTrayText(string text) => _trayIcon.ToolTipText = text.Truncate(63);

    private void SetTrayIcon(byte[] icon)
    {
        using MemoryStream stream = new(icon, false);
        _trayIcon.Icon = new WindowIcon(stream);
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
        foreach (NativeMenuItem item in _trayMenu.Items)
        {
            (item.Icon as IDisposable)?.Dispose();
            item.Icon = null;
        }
        _trayMenuIconKeys.Clear();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // The outline is drawn by separate windows, so it follows the toolbar when it is hidden (e.g. while encoding).
        if (change.Property == IsVisibleProperty && _frame != null)
        {
            if (change.GetNewValue<bool>())
            {
                _frame.Show();
            }
            else
            {
                _frame.Hide();
            }
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _frame.Close();

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
