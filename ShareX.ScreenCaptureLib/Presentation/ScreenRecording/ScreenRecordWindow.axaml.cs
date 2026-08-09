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
using ShareX.ScreenCaptureLib.Localization;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using DrawingColor = System.Drawing.Color;
using DrawingIcon = System.Drawing.Icon;
using DrawingRectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;

namespace ShareX.ScreenCaptureLib;

public partial class ScreenRecordWindow : Window, IDisposable
{
    private const int BorderPixels = 1;
    private const int ToolbarGapPixels = 3;
    private const double ToolbarWidth = 386;
    private const double ToolbarHeight = 48;
    private const int RegionOr = 2;
    private const int RegionDiff = 4;

    private readonly int _captureWidth;
    private readonly int _captureHeight;
    private readonly DispatcherTimer _refreshTimer;
    private readonly WinForms.ContextMenuStrip _trayMenu;
    private readonly WinForms.ToolStripMenuItem _trayStartItem;
    private readonly WinForms.ToolStripMenuItem _trayPauseItem;
    private readonly WinForms.ToolStripMenuItem _trayAbortItem;
    private readonly WinForms.NotifyIcon _trayIcon;

    private volatile ScreenRecordingStatus _status;
    private volatile bool _disposed;
    private DrawingIcon? _ownedTrayIcon;
    private bool _dragging;
    private PixelPoint _dragPointerOrigin;
    private PixelPoint _dragWindowOrigin;
    private double _windowScaling = 1;
    private int _lastIconStatus = -1;
    private bool _activateWindow = true;

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

    public DrawingRectangle RecordingRegion => new(
        Position.X + BorderPixels,
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

        _trayStartItem = new WinForms.ToolStripMenuItem(Strings.ScreenRecordForm_Start);
        _trayPauseItem = new WinForms.ToolStripMenuItem(Strings.Pause);
        _trayAbortItem = new WinForms.ToolStripMenuItem(Strings.ScreenRecordWindow_Abort);
        _trayStartItem.Click += (_, _) => RunOnUIThread(StartStopRecording);
        _trayPauseItem.Click += (_, _) => RunOnUIThread(PauseResumeRecording);
        _trayAbortItem.Click += (_, _) => RunOnUIThread(RequestAbortRecording);

        _trayMenu = new WinForms.ContextMenuStrip();
        _trayMenu.Items.AddRange([_trayStartItem, _trayPauseItem, _trayAbortItem]);

        _trayIcon = new WinForms.NotifyIcon
        {
            ContextMenuStrip = _trayMenu,
            Text = "ShareX",
            Visible = false
        };
        _trayIcon.MouseClick += OnTrayIconMouseClick;

        StartButton.Click += (_, _) => StartStopRecording();
        PauseButton.Click += (_, _) => PauseResumeRecording();
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

        SetRecordingAccent(Brushes.LimeGreen);
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
                    SetTrayIcon(Properties.Resources.control_record_yellow.ToIcon());
                    _trayMenu.Enabled = false;
                    _trayIcon.Visible = true;
                    break;
                case ScreenRecordState.BeforeStart:
                    _trayMenu.Enabled = true;
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
                    _trayMenu.Enabled = false;
                    SetTrayText("ShareX - " + Strings.ScreenRecordForm_StartRecording_Encoding___);
                    SetTrayIcon(Properties.Resources.camcorder__pencil.ToIcon());
                    break;
            }
        });
    }

    public void ChangeStateProgress(int progress)
    {
        InvokeSafe(() =>
        {
            SetTrayText($"ShareX - {Strings.ScreenRecordForm_StartRecording_Encoding___} {progress}%");

            if (!_trayIcon.Visible || _lastIconStatus == progress)
            {
                return;
            }

            DrawingIcon icon;
            if (progress >= 0)
            {
                try
                {
                    icon = Helpers.GetProgressIcon(progress, DrawingColor.FromArgb(140, 0, 36));
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    progress = -1;
                    if (_lastIconStatus == progress)
                    {
                        return;
                    }
                    icon = Properties.Resources.camcorder__pencil.ToIcon();
                }
            }
            else
            {
                icon = Properties.Resources.camcorder__pencil.ToIcon();
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
        _trayIcon.Visible = false;
        _trayIcon.MouseClick -= OnTrayIconMouseClick;
        _trayIcon.Dispose();
        _trayMenu.Dispose();
        _ownedTrayIcon?.Dispose();
        _ownedTrayIcon = null;
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
                SetTrayIcon(Properties.Resources.control_record.ToIcon());
                StartText.Text = Strings.ScreenRecordForm_Stop;
                StartIcon.Text = LucideIcons.square;
                _trayStartItem.Text = Strings.ScreenRecordForm_Stop;
                SetRecordingAccent(Brushes.Goldenrod);
                break;

            case ScreenRecordingStatus.Waiting:
            case ScreenRecordingStatus.Paused:
                bool paused = Status == ScreenRecordingStatus.Paused;
                SetTrayText("ShareX - " + (paused
                    ? Strings.ScreenRecordForm_StartRecording_Click_tray_icon_to_stop_recording_
                    : Strings.ScreenRecordForm_StartRecording_Click_tray_icon_to_start_recording_));
                SetTrayIcon(Properties.Resources.control_record_yellow.ToIcon());
                StartText.Text = paused ? Strings.ScreenRecordForm_Stop : Strings.ScreenRecordForm_Start;
                StartIcon.Text = paused ? LucideIcons.square : LucideIcons.circle_play;
                _trayStartItem.Text = StartText.Text;
                PauseText.Text = Strings.Resume;
                PauseIcon.Text = LucideIcons.play;
                _trayPauseItem.Text = Strings.Resume;
                TimerDragHandle.Cursor = new Cursor(StandardCursorType.SizeAll);
                SetRecordingAccent(Brushes.Goldenrod);
                break;

            case ScreenRecordingStatus.Recording:
                SetTrayIcon(Properties.Resources.control_record.ToIcon());
                StartText.Text = Strings.ScreenRecordForm_Stop;
                StartIcon.Text = LucideIcons.square;
                _trayStartItem.Text = Strings.ScreenRecordForm_Stop;
                PauseText.Text = Strings.Pause;
                PauseIcon.Text = LucideIcons.pause;
                _trayPauseItem.Text = Strings.Pause;
                TimerDragHandle.Cursor = Cursor.Default;
                break;
        }
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
        if (_disposed)
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
        _windowScaling = Math.Max(0.5, scaling);

        int frameWidthPixels = _captureWidth + BorderPixels * 2;
        int frameHeightPixels = _captureHeight + BorderPixels * 2;
        double frameWidth = frameWidthPixels / _windowScaling;
        double frameHeight = frameHeightPixels / _windowScaling;
        double gap = ToolbarGapPixels / _windowScaling;

        RegionBorder.Width = frameWidth;
        RegionBorder.Height = frameHeight;
        Avalonia.Controls.Canvas.SetTop(Toolbar, frameHeight + gap);

        Width = Math.Max(frameWidth, ToolbarWidth);
        Height = frameHeight + gap + ToolbarHeight;

        if (IsVisible)
        {
            Dispatcher.UIThread.Post(ApplyNativeWindowRegion, DispatcherPriority.Loaded);
        }
    }

    private double GetScreenScaling(PixelPoint point)
    {
        return Screens.ScreenFromPoint(point)?.Scaling ?? Screens.Primary?.Scaling ?? 1;
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
    }

    private void ApplyNativeWindowRegion()
    {
        IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        int frameWidth = _captureWidth + BorderPixels * 2;
        int frameHeight = _captureHeight + BorderPixels * 2;
        int toolbarTop = frameHeight + ToolbarGapPixels;
        int toolbarWidth = (int)Math.Ceiling(ToolbarWidth * _windowScaling);
        int toolbarHeight = (int)Math.Ceiling(ToolbarHeight * _windowScaling);

        IntPtr windowRegion = CreateRectRgn(0, 0, frameWidth, frameHeight);
        IntPtr apertureRegion = CreateRectRgn(
            BorderPixels,
            BorderPixels,
            frameWidth - BorderPixels,
            frameHeight - BorderPixels);
        IntPtr toolbarRegion = CreateRectRgn(0, toolbarTop, toolbarWidth, toolbarTop + toolbarHeight);

        if (windowRegion == IntPtr.Zero || apertureRegion == IntPtr.Zero || toolbarRegion == IntPtr.Zero)
        {
            DeleteRegion(windowRegion);
            DeleteRegion(apertureRegion);
            DeleteRegion(toolbarRegion);
            return;
        }

        CombineRgn(windowRegion, windowRegion, apertureRegion, RegionDiff);
        CombineRgn(windowRegion, windowRegion, toolbarRegion, RegionOr);
        DeleteObject(apertureRegion);
        DeleteObject(toolbarRegion);

        if (SetWindowRgn(handle, windowRegion, true) == 0)
        {
            DeleteObject(windowRegion);
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
            candidate.X + BorderPixels,
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

    private void OnTrayIconMouseClick(object? sender, WinForms.MouseEventArgs e)
    {
        if (e.Button == WinForms.MouseButtons.Left)
        {
            RunOnUIThread(StartStopRecording);
        }
    }

    private void SetRecordingAccent(IBrush brush)
    {
        RegionBorder.AccentBrush = brush;
        StatusIndicator.Background = brush;
    }

    private void SetTrayText(string text)
    {
        _trayIcon.Text = text.Truncate(63);
    }

    private void SetTrayIcon(DrawingIcon icon)
    {
        DrawingIcon? previous = _ownedTrayIcon;
        _ownedTrayIcon = icon;
        _trayIcon.Icon = icon;
        previous?.Dispose();
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

    private static void DeleteRegion(IntPtr region)
    {
        if (region != IntPtr.Zero)
        {
            DeleteObject(region);
        }
    }

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateRectRgn(int left, int top, int right, int bottom);

    [DllImport("gdi32.dll")]
    private static extern int CombineRgn(IntPtr destination, IntPtr source1, IntPtr source2, int combineMode);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr handle);

    [DllImport("user32.dll")]
    private static extern int SetWindowRgn(IntPtr windowHandle, IntPtr regionHandle, bool redraw);
}
