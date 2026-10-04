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
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.ScreenCaptureLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX;

public partial class AutoCaptureWindow : Window
{
    private readonly DispatcherTimer _screenshotTimer;
    private readonly DispatcherTimer _statusTimer;
    private readonly Stopwatch _stopwatch = new();
    private readonly TrayIcon _trayIcon;
    private readonly IDisposable _trayIconBinding;
    private readonly IDisposable _trayIconRegistration;
    private bool _isLoaded;
    private int _delay;
    private int _count;
    private bool _waitUploads;
    private Rectangle _customRegion;
    private readonly AutoCaptureWindowViewModel _viewModel = new();
    private bool _closePending;
    private bool _resourcesDisposed;

    public bool IsRunning => _viewModel.IsRunning;
    public TaskSettings TaskSettings { get; set; } = TaskSettings.GetDefaultTaskSettings();

    public AutoCaptureWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _screenshotTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _screenshotTimer.Tick += OnScreenshotTimerTick;
        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _statusTimer.Tick += (_, _) => UpdateStatus();

        _trayIcon = new TrayIcon
        {
            ToolTipText = Strings.AutoCaptureWindow_Title,
            IsVisible = false
        };
        _trayIconBinding = LucideTrayIcon.Bind(_trayIcon, LucideIcons.clock);
        _trayIcon.Clicked += OnTrayIconClick;
        _trayIconRegistration = DesktopServices.RegisterTrayIcon(_trayIcon);

        _customRegion = ApplicationState.Settings.AutoCaptureRegion;
        RepeatTimeInput.Value = AutoCaptureWindowViewModel.ClampRepeatSeconds(ApplicationState.Settings.AutoCaptureRepeatTime);
        AutoMinimizeInput.IsChecked = ApplicationState.Settings.AutoCaptureMinimizeToTray;
        WaitUploadsInput.IsChecked = ApplicationState.Settings.AutoCaptureWaitUpload;
        UpdateRegion();

        PropertyChanged += OnWindowPropertyChanged;
        Closing += OnClosing;
        Closed += OnClosed;
        _isLoaded = true;
    }

    public void ShowAndActivate()
    {
        if (_viewModel.IsClosed) return;
        if (!IsVisible)
        {
            Show();
        }

        WindowState = Avalonia.Controls.WindowState.Normal;
        Activate();
    }

    public void Execute()
    {
        if (_viewModel.IsClosed) return;
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
        if (!_viewModel.TryStart(ApplicationState.Settings.AutoCaptureRegion)) { RefreshAvailability(); return; }
        ExecuteText.Text = Strings.AutoCaptureWindow_Stop;
        ExecuteIcon.Text = LucideIcons.square;
        StatusIcon.Text = LucideIcons.timer;
        _screenshotTimer.Interval = TimeSpan.FromSeconds(1);
        _delay = AutoCaptureWindowViewModel.GetRepeatDelayMilliseconds(ApplicationState.Settings.AutoCaptureRepeatTime);
        _waitUploads = ApplicationState.Settings.AutoCaptureWaitUpload;

        _screenshotTimer.Start();
        _statusTimer.Start();
        RefreshAvailability();

        if (ApplicationState.Settings.AutoCaptureMinimizeToTray)
        {
            HideToTray();
        }
    }

    private void Stop()
    {
        _viewModel.Stop();
        _screenshotTimer.Stop();
        _statusTimer.Stop();
        _stopwatch.Reset();
        if (_viewModel.IsClosed) return;
        StatusProgress.Value = 0;
        StatusText.Text = Strings.AutoCaptureWindow_Ready;
        StatusIcon.Text = LucideIcons.timer;
        StatusIcon.Foreground = Avalonia.Media.Brushes.Gray;
        ExecuteText.Text = Strings.AutoCaptureWindow_Start;
        ExecuteIcon.Text = LucideIcons.play;
        RefreshAvailability();
    }

    private async void OnScreenshotTimerTick(object? sender, EventArgs e)
    {
        if (_viewModel.IsClosed || !IsRunning || _viewModel.IsBusy)
        {
            return;
        }

        if (!_viewModel.Support.IsSupported) { Stop(); return; }

        if (_waitUploads && TaskManager.IsBusy)
        {
            _screenshotTimer.Interval = TimeSpan.FromSeconds(1);
            return;
        }

        _stopwatch.Restart();
        _screenshotTimer.Interval = TimeSpan.FromMilliseconds(_delay);
        _count++;
        _screenshotTimer.Stop();
        Task capture = TakeScreenshotAsync();
        RefreshAvailability();
        try
        {
            await capture;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            if (!_viewModel.IsClosed) { Stop(); ex.ShowError(); }
        }
        finally
        {
            if (!_viewModel.IsClosed)
            {
                if (IsRunning) _screenshotTimer.Start();
                else Stop();
                RefreshAvailability();
            }
            if (_closePending) Close();
        }
    }

    private async Task TakeScreenshotAsync()
    {
        Rectangle rectangle = ApplicationState.Settings.AutoCaptureRegion;
        TaskSettings taskSettings = TaskSettings;
        await _viewModel.TryCaptureAsync(rectangle, region => TaskHelpers.GetScreenshot(taskSettings).CaptureRectangleAsync(region), bitmap =>
        {
            taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AnnotateImage);
            taskSettings.GeneralSettings.PlaySoundAfterUpload = false;
            taskSettings.GeneralSettings.PlaySoundAfterAction = false;
            taskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted = false;
            UploadManager.RunImageTask(bitmap, taskSettings, true, true);
        });
    }

    private void UpdateStatus()
    {
        if (_viewModel.IsClosed || !IsRunning)
        {
            return;
        }

        if (!_viewModel.Support.IsSupported) { Stop(); return; }
        RefreshAvailability();

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

    private async Task SelectRegionAsync()
    {
        if (!_viewModel.CanEdit) { RefreshAvailability(); return; }
        RegionCaptureOptions options = TaskSettings.CaptureSettings.RegionCaptureOptions;
        Task selection = _viewModel.TrySelectRegionAsync(async () =>
        {
            var result = await RegionCaptureTasks.GetRectangleRegionAsync(options);
            return result?.Rectangle;
        }, rectangle =>
        {
            ApplicationState.Settings.AutoCaptureRegion = rectangle;
            UpdateRegion();
        });
        RefreshAvailability();
        try { await selection; }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            if (!_viewModel.IsClosed) ex.ShowError();
        }
        finally
        {
            if (!_viewModel.IsClosed) RefreshAvailability();
            if (_closePending) Close();
        }
    }

    private void UpdateRegion()
    {
        Rectangle rectangle = ApplicationState.Settings.AutoCaptureRegion;
        if (_viewModel.IsClosed) return;

        RegionText.Text = rectangle.IsEmpty
            ? Strings.AutoCaptureWindow_NoRegion
            : string.Format(
                Strings.AutoCaptureWindow_Region,
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
        RefreshAvailability();
    }

    private void RefreshAvailability()
    {
        if (_viewModel.IsClosed) return;
        var support = _viewModel.Support;
        OptionsContent.IsEnabled = _viewModel.CanEdit;
        SelectRegionButton.IsEnabled = _viewModel.CanEdit && CustomRegionRadio.IsChecked == true;
        ExecuteButton.IsEnabled = IsRunning || _viewModel.CanStart(ApplicationState.Settings.AutoCaptureRegion);
        ToolTip.SetTip(OptionsAvailabilitySurface, support.IsSupported ? null : support.Reason);
        ToolTip.SetTip(ExecuteAvailabilitySurface, IsRunning || support.IsSupported ? null : support.Reason);
        if (!support.IsSupported && !IsRunning) StatusText.Text = support.Reason;
    }

    private void HideToTray()
    {
        if (_viewModel.IsClosed) return;
        Hide();
        _trayIcon.IsVisible = true;
    }

    private void RestoreFromTray()
    {
        if (_viewModel.IsClosed) return;
        _trayIcon.IsVisible = false;
        ShowAndActivate();
    }

    private void OnExecuteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Execute();

    private async void OnSelectRegionClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await SelectRegionAsync();

    private void OnFullscreenChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded && FullscreenRadio.IsChecked == true)
        {
            _viewModel.TryChange(() =>
            {
                _customRegion = ApplicationState.Settings.AutoCaptureRegion;
                ApplicationState.Settings.AutoCaptureRegion = CaptureHelpers.GetScreenBounds();
                UpdateRegion();
            });
            RefreshAvailability();
        }
    }

    private void OnCustomRegionChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded && CustomRegionRadio.IsChecked == true)
        {
            _viewModel.TryChange(() =>
            {
                ApplicationState.Settings.AutoCaptureRegion = _customRegion;
                UpdateRegion();
            });
            RefreshAvailability();
        }
    }

    private void OnRepeatTimeChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_isLoaded && RepeatTimeInput.Value is decimal value)
        {
            _viewModel.TryChange(() => ApplicationState.Settings.AutoCaptureRepeatTime = AutoCaptureWindowViewModel.ClampRepeatSeconds(value));
            RefreshAvailability();
        }
    }

    private void OnAutoMinimizeChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded)
        {
            _viewModel.TryChange(() => ApplicationState.Settings.AutoCaptureMinimizeToTray = AutoMinimizeInput.IsChecked == true);
            RefreshAvailability();
        }
    }

    private void OnWaitUploadsChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLoaded)
        {
            _viewModel.TryChange(() => ApplicationState.Settings.AutoCaptureWaitUpload = WaitUploadsInput.IsChecked == true);
            RefreshAvailability();
        }
    }

    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!_viewModel.IsClosed && e.Property == WindowStateProperty
            && ApplicationState.Settings.AutoCaptureMinimizeToTray
            && WindowState == Avalonia.Controls.WindowState.Minimized)
        {
            HideToTray();
        }
    }

    private void OnTrayIconClick(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(RestoreFromTray);
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!_viewModel.IsBusy) return;
        Stop();
        _viewModel.Close();
        _closePending = true;
        OptionsContent.IsEnabled = false;
        ExecuteButton.IsEnabled = false;
        e.Cancel = e.CloseReason != WindowCloseReason.OSShutdown;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (_resourcesDisposed) return;
        _resourcesDisposed = true;
        _viewModel.Close();
        Stop();
        PropertyChanged -= OnWindowPropertyChanged;
        _trayIcon.IsVisible = false;
        _trayIcon.Clicked -= OnTrayIconClick;
        _trayIconBinding.Dispose();
        _trayIconRegistration.Dispose();
        _trayIcon.Dispose();
    }
}
