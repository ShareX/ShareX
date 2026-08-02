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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.ScreenCaptureLib;
using System;
using System.Diagnostics;
using System.Drawing;
using WinFormsNotifyIcon = System.Windows.Forms.NotifyIcon;
using WinFormsMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using AppResources = ShareX.Properties.Resources;

namespace ShareX;

public partial class AutoCaptureWindow : Window
{
    private readonly DispatcherTimer _screenshotTimer;
    private readonly DispatcherTimer _statusTimer;
    private readonly Stopwatch _stopwatch = new();
    private readonly WinFormsNotifyIcon _trayIcon;
    private readonly Icon _trayIconImage;
    private bool _isLoaded;
    private int _delay;
    private int _count;
    private bool _waitUploads;
    private Rectangle _customRegion;

    public bool IsRunning { get; private set; }
    public TaskSettings TaskSettings { get; set; } = TaskSettings.GetDefaultTaskSettings();

    public AutoCaptureWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _screenshotTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _screenshotTimer.Tick += OnScreenshotTimerTick;
        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _statusTimer.Tick += (_, _) => UpdateStatus();

        _trayIconImage = AppResources.clock.ToIcon();
        _trayIcon = new WinFormsNotifyIcon
        {
            Icon = _trayIconImage,
            Text = Strings.AutoCaptureWindow_Title,
            Visible = false
        };
        _trayIcon.MouseClick += OnTrayIconClick;

        _customRegion = Program.Settings.AutoCaptureRegion;
        RepeatTimeInput.Value = Program.Settings.AutoCaptureRepeatTime;
        AutoMinimizeInput.IsChecked = Program.Settings.AutoCaptureMinimizeToTray;
        WaitUploadsInput.IsChecked = Program.Settings.AutoCaptureWaitUpload;
        UpdateRegion();

        PropertyChanged += OnWindowPropertyChanged;
        Closed += OnClosed;
        _isLoaded = true;
    }

    public void ShowAndActivate()
    {
        if (!IsVisible)
        {
            Show();
        }

        WindowState = Avalonia.Controls.WindowState.Normal;
        Activate();
    }

    public void Execute()
    {
        if (IsRunning)
        {
            Stop();
        }
        else
        {
            Start();
        }
    }

    private void Start()
    {
        IsRunning = true;
        ExecuteText.Text = Strings.AutoCaptureWindow_Stop;
        ExecuteIcon.Text = LucideIcons.square;
        StatusIcon.Text = LucideIcons.timer;
        _screenshotTimer.Interval = TimeSpan.FromSeconds(1);
        _delay = (int)(Program.Settings.AutoCaptureRepeatTime * 1000);
        _waitUploads = Program.Settings.AutoCaptureWaitUpload;

        _screenshotTimer.Start();
        _statusTimer.Start();

        if (Program.Settings.AutoCaptureMinimizeToTray)
        {
            HideToTray();
        }
    }

    private void Stop()
    {
        IsRunning = false;
        _screenshotTimer.Stop();
        _statusTimer.Stop();
        _stopwatch.Reset();
        StatusProgress.Value = 0;
        StatusText.Text = Strings.AutoCaptureWindow_Ready;
        StatusIcon.Text = LucideIcons.timer;
        StatusIcon.Foreground = Avalonia.Media.Brushes.Gray;
        ExecuteText.Text = Strings.AutoCaptureWindow_Start;
        ExecuteIcon.Text = LucideIcons.play;
    }

    private void OnScreenshotTimerTick(object? sender, EventArgs e)
    {
        if (!IsRunning)
        {
            return;
        }

        if (_waitUploads && TaskManager.IsBusy)
        {
            _screenshotTimer.Interval = TimeSpan.FromSeconds(1);
            return;
        }

        _stopwatch.Restart();
        _screenshotTimer.Interval = TimeSpan.FromMilliseconds(_delay);
        _count++;
        TakeScreenshot();
    }

    private void TakeScreenshot()
    {
        Rectangle rectangle = Program.Settings.AutoCaptureRegion;

        if (rectangle.IsEmpty)
        {
            return;
        }

        Bitmap bitmap = TaskHelpers.GetScreenshot(TaskSettings).CaptureRectangle(rectangle);

        if (bitmap == null)
        {
            return;
        }

        TaskSettings.AfterCaptureJob = TaskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AnnotateImage);
        TaskSettings.GeneralSettings.PlaySoundAfterUpload = false;
        TaskSettings.GeneralSettings.PlaySoundAfterAction = false;
        TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted = false;
        UploadManager.RunImageTask(bitmap, TaskSettings, true, true);
    }

    private void UpdateStatus()
    {
        if (!IsRunning)
        {
            return;
        }

        int timeLeft = Math.Max(0, _delay - (int)_stopwatch.ElapsedMilliseconds);
        int percentage = _delay > 0 ? (int)(100 - (double)timeLeft / _delay * 100) : 100;
        string secondsLeft = (timeLeft / 1000f).ToString("0.0");
        StatusProgress.Value = Math.Clamp(percentage, 0, 100);
        StatusText.Text = string.Format(
            Strings.AutoCaptureWindow_Status,
            secondsLeft,
            percentage,
            _count);
        StatusIcon.Foreground = Avalonia.Media.Brushes.DodgerBlue;
    }

    private void SelectRegion()
    {
        if (RegionCaptureTasks.GetRectangleRegion(out Rectangle rectangle, TaskSettings.CaptureSettings.SurfaceOptions))
        {
            Program.Settings.AutoCaptureRegion = rectangle;
            UpdateRegion();
        }
    }

    private void UpdateRegion()
    {
        Rectangle rectangle = Program.Settings.AutoCaptureRegion;
        ExecuteButton.IsEnabled = !rectangle.IsEmpty;

        RegionText.Text = rectangle.IsEmpty
            ? Strings.AutoCaptureWindow_NoRegion
            : string.Format(
                Strings.AutoCaptureWindow_Region,
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
    }

    private void HideToTray()
    {
        Hide();
        _trayIcon.Visible = true;
    }

    private void RestoreFromTray()
    {
        _trayIcon.Visible = false;
        ShowAndActivate();
    }

    private void OnExecuteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Execute();

    private void OnSelectRegionClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => SelectRegion();

    private void OnFullscreenChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded && FullscreenRadio.IsChecked == true)
        {
            _customRegion = Program.Settings.AutoCaptureRegion;
            Program.Settings.AutoCaptureRegion = CaptureHelpers.GetScreenBounds();
            SelectRegionButton.IsEnabled = false;
            UpdateRegion();
        }
    }

    private void OnCustomRegionChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded && CustomRegionRadio.IsChecked == true)
        {
            Program.Settings.AutoCaptureRegion = _customRegion;
            SelectRegionButton.IsEnabled = true;
            UpdateRegion();
        }
    }

    private void OnRepeatTimeChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_isLoaded && RepeatTimeInput.Value is decimal value)
        {
            Program.Settings.AutoCaptureRepeatTime = value;
        }
    }

    private void OnAutoMinimizeChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded)
        {
            Program.Settings.AutoCaptureMinimizeToTray = AutoMinimizeInput.IsChecked == true;
        }
    }

    private void OnWaitUploadsChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded)
        {
            Program.Settings.AutoCaptureWaitUpload = WaitUploadsInput.IsChecked == true;
        }
    }

    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == WindowStateProperty
            && Program.Settings.AutoCaptureMinimizeToTray
            && WindowState == Avalonia.Controls.WindowState.Minimized)
        {
            HideToTray();
        }
    }

    private void OnTrayIconClick(object? sender, WinFormsMouseEventArgs e)
    {
        Dispatcher.UIThread.Post(RestoreFromTray);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        Stop();
        _trayIcon.Visible = false;
        _trayIcon.MouseClick -= OnTrayIconClick;
        _trayIcon.Dispose();
        _trayIconImage.Dispose();
    }
}
