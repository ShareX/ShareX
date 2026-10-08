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
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Platform;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.ScreenCaptureLib;

public partial class ScrollingCaptureWindow : Window
{
    private static readonly Cursor PanCursor = new(StandardCursorType.SizeAll);

    private readonly ScrollingCaptureService _service;
    private readonly ScrollingCaptureWindowViewModel _viewModel;
    private readonly Action<SKBitmap>? _uploadRequested;
    private readonly Action? _playNotificationSound;
    private AvaloniaBitmap? _previewBitmap;
    private bool _serviceDisposed;
    private bool _closeRequested;
    private bool _isPanning;
    private Point _panStart;
    private Vector _panStartOffset;

    public bool IsCapturing => !_viewModel.IsClosed && _service.IsCapturing;

    public ScrollingCaptureWindow()
        : this(new ScrollingCaptureOptions(), null, null)
    {
    }

    public ScrollingCaptureWindow(
        ScrollingCaptureOptions options,
        Action<SKBitmap>? uploadRequested,
        Action? playNotificationSound)
    {
        _service = new ScrollingCaptureService(options);
        _viewModel = new ScrollingCaptureWindowViewModel();
        _uploadRequested = uploadRequested;
        _playNotificationSound = playNotificationSound;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        ScrollMethodInput.ItemsSource = Helpers.GetEnums<ScrollMethod>().Select(method =>
        {
            FeatureSupport support = _viewModel.GetSupport(method);
            ComboBoxItem item = new() { Content = method.GetLocalizedDescription(), IsEnabled = support.IsSupported };
            ToolTip.SetTip(item, support.IsSupported ? null : support.Reason);
            ToolTip.SetShowOnDisabled(item, true);
            return item;
        }).ToArray();
        RefreshCaptureControls();
        if (HidesInsteadOfMinimizing)
        {
            // Shown invisible, then hidden while the region is selected (see GetOutOfTheWay).
            Opacity = 0;
        }
        else
        {
            WindowState = Avalonia.Controls.WindowState.Minimized;
        }

        Opened += OnOpened;
        Activated += OnActivated;
        Closing += OnClosing;
        Closed += OnClosed;
    }

    public async Task StartStopAsync()
    {
        if (_viewModel.IsClosed) return;

        if (_service.IsCapturing)
        {
            StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Stopping_capture;
            _service.StopCapture();
            return;
        }

        if (!_viewModel.IsBusy)
        {
            await SelectWindowAsync();
        }
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await StartStopAsync();
    }

    private void OnActivated(object? sender, EventArgs e)
    {
        if (_service.IsCapturing)
        {
            StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Stopping_capture;
            _service.StopCapture();
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_viewModel.IsBusy || _service.IsCapturing)
        {
            _closeRequested = true;
            if (_service.IsCapturing) _service.StopCapture();
            else _viewModel.Close();
            e.Cancel = e.CloseReason != WindowCloseReason.OSShutdown;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _viewModel.Close();
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        // A selector has no cancellation contract. Let it finish before disposing its service.
        if (!_viewModel.IsBusy) DisposeService();
    }

    private static bool HidesInsteadOfMinimizing => PlatformServices.IsInitialized && PlatformServices.Current.Windows.HidesInsteadOfMinimizing;

    private void GetOutOfTheWay()
    {
        if (HidesInsteadOfMinimizing)
        {
            Hide();
        }
        else
        {
            WindowState = Avalonia.Controls.WindowState.Minimized;
        }
    }

    private async Task SelectWindowAsync()
    {
        // Ask for the input permission (macOS Accessibility) first, so its prompt is not answered in the middle of a capture.
        if (PlatformServices.IsInitialized && !PlatformServices.Current.Input.RequestPermission())
        {
            SetStatus(ScrollingCaptureStatus.Failed);
            StatusText.Text = GetUnavailableReason();
            RestoreAndActivate();
            RefreshCaptureControls();
            return;
        }

        try
        {
            ScrollingCaptureWindowViewModel.StartResult result = await _viewModel.TryCaptureAsync(_service.Options,
                () =>
                {
                    OptionsOverlay.IsVisible = false;
                    GetOutOfTheWay();
                    RefreshCaptureControls();
                }, () => Task.Delay(250), _service.SelectWindowAsync, CaptureSelectedWindowAsync);

            if (_viewModel.IsClosed) return;
            if (result == ScrollingCaptureWindowViewModel.StartResult.Unavailable)
            {
                SetStatus(ScrollingCaptureStatus.Failed);
                StatusText.Text = GetUnavailableReason();
                RestoreAndActivate();
            }
            else if (result == ScrollingCaptureWindowViewModel.StartResult.Cancelled)
            {
                StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Selection_cancelled;
                RestoreAndActivate();
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            if (_viewModel.IsClosed) return;
            SetStatus(ScrollingCaptureStatus.Failed);
            StatusText.Text = ex.Message;
            RestoreAndActivate();
            ex.ShowError();
        }
        finally
        {
            if (_viewModel.IsClosed)
            {
                DisposeService();
                if (_closeRequested) Close();
            }
            else RefreshCaptureControls();
        }
    }

    private void DisposeService()
    {
        if (_serviceDisposed) return;
        _serviceDisposed = true;
        _service.Dispose();
    }

    private async Task CaptureSelectedWindowAsync()
    {
        SetCaptureControlsEnabled(false);
        ResultSizeText.Text = string.Empty;
        ResetPreview();
        StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Capturing;
        StatusIcon.Text = LucideIcons.loader_circle;
        StatusIcon.Foreground = Brushes.DodgerBlue;

        try
        {
            ScrollingCaptureStatus status = await _service.StartCaptureAsync();
            if (_viewModel.IsClosed) return;
            SetStatus(status);
            if (status == ScrollingCaptureStatus.Failed && !string.IsNullOrWhiteSpace(_service.FailureReason))
            {
                // For example a missing Accessibility permission on macOS: say so, not only in the status line.
                StatusText.Text = _service.FailureReason;
                RestoreAndActivate();
                ShareX.AvaloniaUI.MessageBox.Show(this, _service.FailureReason, "ShareX", ShareX.AvaloniaUI.MessageBoxButtons.OK, ShareX.AvaloniaUI.MessageBoxIcon.Warning);
            }
            _playNotificationSound?.Invoke();
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            if (_viewModel.IsClosed) return;
            SetStatus(ScrollingCaptureStatus.Failed);
            StatusText.Text = ex.Message;
            ex.ShowError();
        }
        finally
        {
            if (!_viewModel.IsClosed)
            {
                LoadImage(_service.Result);
                RestoreAndActivate();
            }
        }

        if (_service.Options.AutoUpload)
        {
            UploadResult();
        }

        if (_closeRequested)
        {
            Close();
        }
    }

    private void SetCaptureControlsEnabled(bool enabled)
    {
        FeatureSupport support = _viewModel.GetSupport(_service.Options.ScrollMethod, _service.Options.AutoScrollTop);
        CaptureButton.IsEnabled = enabled && support.IsSupported;
        ToolTip.SetTip(CaptureButton, support.IsSupported ? null : support.Reason);
        ToolTip.SetShowOnDisabled(CaptureButton, true);
        OptionsButton.IsEnabled = enabled;
        UploadButton.IsEnabled = enabled && _service.Result != null;
        CopyButton.IsEnabled = enabled && _service.Result != null;
    }

    private void RefreshCaptureControls()
    {
        if (_viewModel.IsClosed) return;
        SetCaptureControlsEnabled(!_viewModel.IsBusy && !_service.IsCapturing);
    }

    private void SetStatus(ScrollingCaptureStatus status)
    {
        switch (status)
        {
            case ScrollingCaptureStatus.Failed:
                StatusIcon.Text = LucideIcons.circle_x;
                StatusIcon.Foreground = Brushes.IndianRed;
                StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Capture_failed;
                break;
            case ScrollingCaptureStatus.PartiallySuccessful:
                StatusIcon.Text = LucideIcons.triangle_alert;
                StatusIcon.Foreground = Brushes.Goldenrod;
                StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Capture_partially_successful;
                break;
            case ScrollingCaptureStatus.Successful:
                StatusIcon.Text = LucideIcons.circle_check;
                StatusIcon.Foreground = Brushes.MediumSeaGreen;
                StatusText.Text = Localization.Strings.ScrollingCaptureWindow_Capture_successful;
                break;
        }
    }

    private void LoadImage(SKBitmap? bitmap)
    {
        if (bitmap == null)
        {
            return;
        }

        using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        using MemoryStream stream = new(data.ToArray());

        AvaloniaBitmap preview = new(stream);
        _previewBitmap?.Dispose();
        _previewBitmap = preview;
        PreviewImage.Source = preview;
        PreviewScrollViewer.Offset = default;
        EmptyState.IsVisible = false;
        ResultSizeText.Text = $"{bitmap.Width}x{bitmap.Height}";
        UploadButton.IsEnabled = true;
        CopyButton.IsEnabled = true;
    }

    private void ResetPreview()
    {
        PreviewImage.Source = null;
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        EmptyState.IsVisible = true;
        UploadButton.IsEnabled = false;
        CopyButton.IsEnabled = false;
    }

    private string GetUnavailableReason() =>
        _viewModel.GetSupport(_service.Options.ScrollMethod, _service.Options.AutoScrollTop).Reason is { Length: > 0 } reason
            ? reason : Localization.Strings.ScrollingCaptureWindow_Unavailable;

    private void RestoreAndActivate()
    {
        if (_viewModel.IsClosed) return;
        Opacity = 1;
        WindowState = Avalonia.Controls.WindowState.Normal;

        if (!IsVisible)
        {
            Show();
        }

        // On macOS the window only comes to the front when ShareX itself is active.
        PlatformServices.Current.Windows.ActivateOwnApplication();
        Activate();
    }

    private void UploadResult()
    {
        if (!_viewModel.IsClosed && !_service.IsCapturing && _service.Result != null)
        {
            _uploadRequested?.Invoke(_service.Result.Copy());
        }
    }

    private void CopyResult()
    {
        if (!_viewModel.IsClosed && !_service.IsCapturing && _service.Result != null)
        {
            ClipboardHelpers.CopyImage(_service.Result);
        }
    }

    private async void OnCaptureClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await StartStopAsync();

    private void OnUploadClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => UploadResult();

    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => CopyResult();

    private void OnHelpClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => URLHelpers.OpenURL(Links.DocsScrollingScreenshot);

    private void OnOptionsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_viewModel.IsClosed || _viewModel.IsBusy) return;
        LoadOptions();
        OptionsOverlay.IsVisible = true;
    }

    private void OnCancelOptionsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OptionsOverlay.IsVisible = false;
    }

    private void OnSaveOptionsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ScrollMethod method = (ScrollMethod)ScrollMethodInput.SelectedIndex;
        bool autoScrollTop = AutoScrollTopInput.IsChecked == true;
        if (_viewModel.TryChange(method, autoScrollTop, () =>
        {
            ScrollingCaptureOptions options = _service.Options;
            options.StartDelay = Value(StartDelayInput);
            options.AutoScrollTop = autoScrollTop;
            options.ScrollDelay = Value(ScrollDelayInput);
            options.ScrollMethod = method;
            options.ScrollAmount = Value(ScrollAmountInput);
            options.AutoUpload = AutoUploadInput.IsChecked == true;
            options.ShowRegion = ShowRegionInput.IsChecked == true;
            options.AutoIgnoreBottomEdge = AutoIgnoreBottomEdgeInput.IsChecked == true;
        })) OptionsOverlay.IsVisible = false;
        UpdateScrollAmountVisibility();
    }

    private void LoadOptions()
    {
        ScrollingCaptureOptions options = _service.Options;
        StartDelayInput.Value = options.StartDelay;
        AutoScrollTopInput.IsChecked = options.AutoScrollTop;
        ScrollDelayInput.Value = options.ScrollDelay;
        ScrollMethodInput.SelectedIndex = (int)options.ScrollMethod;
        ScrollAmountInput.Value = options.ScrollAmount;
        AutoUploadInput.IsChecked = options.AutoUpload;
        ShowRegionInput.IsChecked = options.ShowRegion;
        AutoIgnoreBottomEdgeInput.IsChecked = options.AutoIgnoreBottomEdge;
        UpdateScrollAmountVisibility();
    }

    private void OnScrollMethodChanged(object? sender, SelectionChangedEventArgs e) => UpdateScrollAmountVisibility();

    private void OnOptionsAvailabilityChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => UpdateScrollAmountVisibility();

    private void UpdateScrollAmountVisibility()
    {
        if (_viewModel.IsClosed) return;
        FeatureSupport support = _viewModel.GetSupport((ScrollMethod)ScrollMethodInput.SelectedIndex, AutoScrollTopInput.IsChecked == true);
        ToolTip.SetTip(ScrollMethodInput, support.IsSupported ? null : support.Reason);
        ToolTip.SetShowOnDisabled(ScrollMethodInput, true);
        ToolTip.SetTip(SaveOptionsButton, support.IsSupported ? null : support.Reason);
        ToolTip.SetShowOnDisabled(SaveOptionsButton, true);
        SaveOptionsButton.IsEnabled = support.IsSupported && !_viewModel.IsBusy;
        FeatureSupport autoTopSupport = _viewModel.AutoScrollTopSupport;
        ToolTip.SetTip(AutoScrollTopInput, autoTopSupport.IsSupported ? null : autoTopSupport.Reason);
        ToolTip.SetShowOnDisabled(AutoScrollTopInput, true);
        // A saved unavailable flag can be turned off without losing it just by opening options.
        AutoScrollTopInput.IsEnabled = !_viewModel.IsBusy && (autoTopSupport.IsSupported || AutoScrollTopInput.IsChecked == true);
        if (ScrollMethodInput.ItemsSource is ComboBoxItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                FeatureSupport methodSupport = _viewModel.GetSupport((ScrollMethod)i);
                items[i].IsEnabled = methodSupport.IsSupported;
                ToolTip.SetTip(items[i], methodSupport.IsSupported ? null : methodSupport.Reason);
            }
        }
        RefreshCaptureControls();

        bool isVisible = (ScrollMethod)ScrollMethodInput.SelectedIndex != ScrollMethod.PageDown;
        ScrollAmountLabel.IsVisible = isVisible;
        ScrollAmountInput.IsVisible = isVisible;
        ScrollAmountHint.IsVisible = isVisible;
    }

    private static int Value(NumericUpDown input) => (int)(input.Value ?? 0);

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (PreviewImage.Source == null || !e.GetCurrentPoint(PreviewImage).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _isPanning = true;
        _panStart = e.GetPosition(PreviewScrollViewer);
        _panStartOffset = PreviewScrollViewer.Offset;
        PreviewImage.Cursor = PanCursor;
        e.Pointer.Capture(PreviewImage);
        e.Handled = true;
    }

    private void OnPreviewPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPanning || e.Pointer.Captured != PreviewImage)
        {
            return;
        }

        Point position = e.GetPosition(PreviewScrollViewer);
        Vector delta = position - _panStart;
        PreviewScrollViewer.Offset = new Vector(
            _panStartOffset.X - delta.X,
            _panStartOffset.Y - delta.Y);
        e.Handled = true;
    }

    private void OnPreviewPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured == PreviewImage)
        {
            e.Pointer.Capture(null);
        }

        EndPanning();
    }

    private void OnPreviewPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndPanning();

    private void EndPanning()
    {
        _isPanning = false;
        PreviewImage.Cursor = null;
    }
}
