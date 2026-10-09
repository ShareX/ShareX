// SPDX-License-Identifier: GPL-3.0-or-later
#nullable enable
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.ScreenCaptureLib;
using ShareX.ScreenRecordingLib;
using ShareX.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX;

internal partial class ScreenRecorderBarWindow : Window
{
    private readonly ScreenRecorderBarSettings _draft;
    private readonly List<Source> _sources = [];
    private readonly TaskCompletionSource<bool> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Button _recordButton;
    private ToggleButton _startDelayToggle = null!;
    private ScrollingCaptureRegionWindow? _regionWindow;
    private string? _activePanel;
    private bool _closed;

    public DrawingRectangle RecordingRegion { get; private set; }
    public IntPtr CaptureWindow { get; }
    private TaskSettingsCapture Capture => _draft.Capture;

    public ScreenRecorderBarWindow(TaskSettings settings, DrawingRectangle region, IntPtr window)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _draft = new(settings);
        RecordingRegion = region;
        CaptureWindow = window;
        _recordButton = ActionButton(Strings.ScreenRecorderBar_Record, LucideIcons.circle, RequestRecord, toolbar: true);
        _recordButton.Classes.Add("recorder-record");

        AddSource(Strings.ScreenRecorderBar_SystemAudio, LucideIcons.volume_2,
            () => Capture.ScreenRecordSystemAudio, value => Capture.ScreenRecordSystemAudio = value,
            () => Capture.ScreenRecordSystemAudioDeviceId, value => Capture.ScreenRecordSystemAudioDeviceId = value,
            () => AudioCaptureDevices.GetSystemAudioDevices().Select(device => new DeviceChoice(device.Id, device.Name)).ToArray(),
            () => AudioCaptureDevices.GetDefaultSystemAudioDevice()?.Name,
            Strings.TaskSettingsWindow_NativeRecorderDefaultSystemAudioDevice, Strings.TaskSettingsWindow_NativeRecorderUnavailableSystemAudioDevice);
        AddSource(Strings.ScreenRecorderBar_Microphone, LucideIcons.mic,
            () => Capture.ScreenRecordMicrophone, value => Capture.ScreenRecordMicrophone = value,
            () => Capture.ScreenRecordMicrophoneDeviceId, value => Capture.ScreenRecordMicrophoneDeviceId = value,
            () => AudioCaptureDevices.GetMicrophones().Select(device => new DeviceChoice(device.Id, device.Name)).ToArray(),
            () => AudioCaptureDevices.GetDefaultMicrophone()?.Name,
            Strings.TaskSettingsWindow_NativeRecorderDefaultMicrophone, Strings.TaskSettingsWindow_NativeRecorderUnavailableMicrophone);
        AddSource(Strings.ScreenRecorderBar_Camera, LucideIcons.video,
            () => Capture.ScreenRecordCamera, value => Capture.ScreenRecordCamera = value,
            () => Capture.ScreenRecordCameraDeviceId, value => Capture.ScreenRecordCameraDeviceId = value,
            () => CameraCaptureDevices.GetCameras().Select(device => new DeviceChoice(device.Id, device.Name)).ToArray(), null,
            Strings.TaskSettingsWindow_NativeRecorderCamera_DefaultDevice, Strings.TaskSettingsWindow_NativeRecorderCamera_UnavailableDevice);

        BuildBar();
        ClosePanelButton.Content = Glyph(LucideIcons.x);
        ToolTip.SetTip(ClosePanelButton, Strings.ActionsToolbarWindow_Close);
        AutomationProperties.SetName(ClosePanelButton, Strings.ActionsToolbarWindow_Close);
        ClosePanelButton.Click += (_, _) => ClosePanel();
        KeyDown += (_, e) =>
        {
            if (e.Key != Key.Escape) return;
            if (SettingsPanel.IsVisible) ClosePanel(); else Cancel();
            e.Handled = true;
        };
        Deactivated += (_, _) => ClosePanel();
        Opened += (_, _) =>
        {
            IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            if (handle != IntPtr.Zero)
            {
                WindowInfo info = new(handle);
                info.ExStyle |= WindowStyles.WS_EX_TOOLWINDOW;
            }
            ShowRegion();
            Dispatcher.UIThread.Post(PositionNearRegion, DispatcherPriority.Loaded);
        };
        PositionChanged += (_, _) => Dispatcher.UIThread.Post(ClampToScreen, DispatcherPriority.Loaded);
        Closed += (_, _) =>
        {
            _closed = true;
            _regionWindow?.Close();
            _regionWindow = null;
            _completion.TrySetResult(false);
        };
    }

    public Task<bool> ShowSetupAsync()
    {
        Show();
        return _completion.Task;
    }

    public void Commit(TaskSettings settings) => _draft.Commit(settings);

    public void RequestRecord()
    {
        if (_closed) return;
        foreach (Source source in _sources) RefreshSource(source);
        if (!ValidateSources()) return;
        if (CaptureWindow != IntPtr.Zero)
        {
            WindowInfo target = new(CaptureWindow);
            if (!target.IsVisible || target.IsMinimized || !target.Rectangle.IsValid())
            {
                ShowError(Strings.ScreenRecorderBar_TargetUnavailable);
                return;
            }
            // The window may have moved or resized while its settings were being edited.
            RecordingRegion = CaptureHelpers.EvenRectangleSize(target.Rectangle);
        }
        _completion.TrySetResult(true);
        Close();
    }

    public void Cancel()
    {
        if (!_closed) Close();
    }

    private void BuildBar()
    {
        Toolbar.Children.Clear();
        StackPanel actions = new() { Orientation = Orientation.Horizontal };
        Border grip = new()
        {
            Width = 24,
            Height = 36,
            Background = Brushes.Transparent,
            Cursor = new Cursor(StandardCursorType.SizeAll),
            Child = Glyph(LucideIcons.grip_vertical)
        };
        grip.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(grip).Properties.IsLeftButtonPressed) BeginMoveDrag(e);
        };
        actions.Children.Add(grip);
        // Rebuild toolbar content when its labels change, keeping the active settings panel open.
        if (_recordButton.Parent is Panel oldParent) oldParent.Children.Remove(_recordButton);
        _recordButton.Content = ButtonContent(Strings.ScreenRecorderBar_Record, LucideIcons.circle, toolbar: true);
        actions.Children.Add(_recordButton);
        StackPanel startDelay = (StackPanel)SplitControl(Strings.ScreenRecorderBar_StartDelay, LucideIcons.timer,
            Capture.ScreenRecordStartDelayEnabled,
            value => { Capture.ScreenRecordStartDelayEnabled = value; UpdateStartDelay(); },
            ShowStartDelayMenu);
        _startDelayToggle = (ToggleButton)startDelay.Children[0];
        UpdateStartDelay();
        actions.Children.Add(startDelay);
        Toolbar.Children.Add(actions);
        StackPanel inputs = new() { Orientation = Orientation.Horizontal };
        foreach (Source source in _sources)
        {
            if (source.Control.Parent is Panel oldInputs) oldInputs.Children.Remove(source.Control);
            UpdateSource(source);
            inputs.Children.Add(source.Control);
        }
        inputs.Children.Add(ToggleControl(Strings.ScreenRecorderBar_Cursor, LucideIcons.mouse_pointer_2,
            Capture.ScreenRecordShowCursor, value => Capture.ScreenRecordShowCursor = value));
        inputs.Children.Add(SplitControl(Strings.MouseHighlighter, LucideIcons.mouse_pointer_click,
            Capture.ScreenRecordMouseHighlighter, value => Capture.ScreenRecordMouseHighlighter = value,
            () => ShowPanel("highlighter", Strings.MouseHighlighter, BuildHighlighterPanel)));
        // Keep the action buttons visible when long labels or a small work area require scrolling the sources.
        ScrollViewer sources = new()
        {
            Content = inputs,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        Grid.SetColumn(sources, 1);
        Toolbar.Children.Add(sources);
        Button options = ActionButton(Strings.ScreenRecorderBar_Options, LucideIcons.settings,
            () => ShowPanel("options", Strings.ScreenRecorderBar_Options, BuildOptionsPanel), toolbar: true);
        Grid.SetColumn(options, 2);
        Toolbar.Children.Add(options);
        Button cancel = ActionButton(Strings.TaskSettingsWindow_Cancel, LucideIcons.x, Cancel, iconOnly: true);
        cancel.Classes.Remove("recorder-setup");
        cancel.Classes.Add("recorder-close");
        cancel.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(cancel, 3);
        Toolbar.Children.Add(cancel);
        ValidateSources();
        Dispatcher.UIThread.Post(ClampToScreen, DispatcherPriority.Loaded);
    }

    private void ShowStartDelayMenu(Control target)
    {
        ClosePanel();
        List<MenuItem> items = [];
        for (int seconds = 1; seconds <= 5; seconds++)
        {
            int delay = seconds;
            MenuItem item = new()
            {
                Header = StartDelayLabel(delay),
                ToggleType = MenuItemToggleType.Radio,
                IsChecked = Capture.ScreenRecordStartDelay == delay
            };
            item.Click += (_, _) => SetStartDelay(delay);
            items.Add(item);
        }
        ContextMenu menu = new() { ItemsSource = items, Placement = PlacementMode.BottomEdgeAlignedLeft };
        menu.Open(target);
    }

    private void SetStartDelay(float delay)
    {
        Capture.ScreenRecordStartDelay = delay;
        UpdateStartDelay();
    }

    private void UpdateStartDelay()
    {
        bool enabled = Capture.ScreenRecordStartDelayEnabled;
        _startDelayToggle.IsChecked = enabled;
        _startDelayToggle.Content = ButtonContent(Strings.ScreenRecorderBar_StartDelay, LucideIcons.timer, enabled, toolbar: true);
        ToolTip.SetTip(_startDelayToggle, TwoLineToolTip(Strings.ScreenRecorderBar_StartDelay,
            enabled ? StartDelayLabel(Capture.ScreenRecordStartDelay) : Strings.ScreenRecorderBar_Off));
    }

    private static string StartDelayLabel(float delay) => delay switch
    {
        1 => Strings.ScreenRecorderBar_StartDelay_1Second,
        2 => Strings.ScreenRecorderBar_StartDelay_2Seconds,
        3 => Strings.ScreenRecorderBar_StartDelay_3Seconds,
        4 => Strings.ScreenRecorderBar_StartDelay_4Seconds,
        5 => Strings.ScreenRecorderBar_StartDelay_5Seconds,
        _ => $"{delay:0.##} {Strings.AutoCaptureWindow_Seconds}"
    };

    private void AddSource(string label, string icon, Func<bool> enabled, Action<bool> setEnabled,
        Func<string> selected, Action<string> setSelected, Func<DeviceChoice[]> enumerate,
        Func<string?>? defaultName, string defaultLabel, string unavailableLabel)
    {
        Source source = new(label, icon, enabled, setEnabled, () => selected() ?? "", setSelected, enumerate, defaultName, defaultLabel, unavailableLabel);
        source.Control = SplitControl(label, icon, enabled(), value =>
        {
            RefreshSource(source);
            source.SetEnabled(value && source.Available);
            UpdateSource(source);
            ValidateSources();
        }, () => ShowPanel(label, label, () => BuildSourcePanel(source)));
        source.Toggle = (ToggleButton)((StackPanel)source.Control).Children[0];
        _sources.Add(source);
        RefreshSource(source);
    }

    private void RefreshSource(Source source)
    {
        try
        {
            source.Devices = source.Enumerate();
            source.ResolvedDefault = source.DefaultName != null ? source.DefaultName() : source.Devices.FirstOrDefault()?.Name;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            source.Devices = [];
            source.ResolvedDefault = null;
        }
        source.Available = string.IsNullOrEmpty(source.Selected())
            ? source.ResolvedDefault != null
            : source.Devices.Any(device => device.Id == source.Selected());
        UpdateSource(source);
    }

    private void UpdateSource(Source source)
    {
        source.Toggle.IsChecked = source.Enabled();
        source.Toggle.IsEnabled = source.Enabled() || source.Available;
        source.Toggle.Content = ButtonContent(source.Label, source.Icon, source.Enabled(), toolbar: true);
        string name = source.Selected() == "" ? source.ResolvedDefault ?? source.UnavailableLabel
            : source.Devices.FirstOrDefault(device => device.Id == source.Selected())?.Name ?? source.UnavailableLabel;
        ToolTip.SetTip(source.Toggle, TwoLineToolTip(source.Label, name));
    }

    private bool ValidateSources()
    {
        Source? missing = _sources.FirstOrDefault(source => source.Enabled() && !source.Available);
        ErrorMessage.IsVisible = missing != null;
        ErrorMessage.Text = missing?.UnavailableLabel;
        _recordButton.IsEnabled = missing == null;
        return missing == null;
    }

    private Control BuildSourcePanel(Source source)
    {
        StackPanel panel = new() { Spacing = 10 };
        ComboBox devices = new() { HorizontalAlignment = HorizontalAlignment.Stretch, MinWidth = 260 };
        bool refreshing = false;
        void Refresh()
        {
            RefreshSource(source);
            string defaultLabel = source.ResolvedDefault == null
                ? ReferenceEquals(source, _sources[2]) ? Strings.TaskSettingsWindow_NativeRecorderCamera_NoDevices : $"{source.DefaultLabel} ({source.UnavailableLabel})"
                : $"{source.DefaultLabel} ({source.ResolvedDefault})";
            List<DeviceChoice> choices = [new("", defaultLabel)];
            choices.AddRange(source.Devices.OrderBy(device => device.Name, StringComparer.CurrentCultureIgnoreCase));
            if (!choices.Any(device => device.Id == source.Selected())) choices.Add(new(source.Selected(), source.UnavailableLabel));
            refreshing = true;
            devices.ItemsSource = choices;
            devices.SelectedItem = choices.First(device => device.Id == source.Selected());
            refreshing = false;
            ValidateSources();
        }
        devices.SelectionChanged += (_, _) =>
        {
            if (refreshing || devices.SelectedItem is not DeviceChoice device) return;
            source.SetSelected(device.Id);
            RefreshSource(source);
            ValidateSources();
        };
        devices.DropDownOpened += (_, _) => Refresh();
        Refresh();
        Grid deviceRow = new() { ColumnDefinitions = new("*,Auto"), ColumnSpacing = 6 };
        deviceRow.Children.Add(devices);
        Button refresh = ActionButton(Strings.ScreenRecorderBar_RefreshDevices, LucideIcons.refresh_cw, Refresh, iconOnly: true);
        Grid.SetColumn(refresh, 1);
        deviceRow.Children.Add(refresh);
        panel.Children.Add(deviceRow);
        if (ReferenceEquals(source, _sources[2])) panel.Children.Add(BuildCameraPanel());
        else
        {
            bool system = ReferenceEquals(source, _sources[0]);
            panel.Children.Add(VolumeControl(system ? Capture.ScreenRecordSystemAudioGain : Capture.ScreenRecordMicrophoneGain,
                value => { if (system) Capture.ScreenRecordSystemAudioGain = value; else Capture.ScreenRecordMicrophoneGain = value; }));
        }
        return panel;
    }

    private Control BuildCameraPanel()
    {
        CameraLayoutPreview preview = new(Capture, () => RecordingRegion) { Height = 140 };
        StackPanel overlay = new() { Spacing = 10, Margin = new Thickness(0, 10) };
        overlay.Children.Add(Hint(Strings.ScreenRecorderBar_LayoutPreview));
        overlay.Children.Add(preview);
        overlay.Children.Add(Row(Strings.TaskSettingsWindow_NativeRecorderCamera_Shape,
            Choice(Enum.GetValues<CameraOverlayShape>(), Capture.ScreenRecordCameraShape,
                value => { Capture.ScreenRecordCameraShape = value; preview.InvalidateVisual(); },
                value => value == CameraOverlayShape.Circle ? Strings.TaskSettingsWindow_NativeRecorderCamera_Circle : Strings.TaskSettingsWindow_NativeRecorderCamera_Rectangle)));
        overlay.Children.Add(Row(Strings.TaskSettingsWindow_Placement,
            Choice(Enum.GetValues<CameraOverlayPosition>(), Capture.ScreenRecordCameraPosition,
                value => { Capture.ScreenRecordCameraPosition = value; preview.InvalidateVisual(); }, value => value switch
                {
                    CameraOverlayPosition.TopLeft => Strings.ApplicationSettingsWindow_TopLeft,
                    CameraOverlayPosition.TopRight => Strings.ApplicationSettingsWindow_TopRight,
                    CameraOverlayPosition.BottomLeft => Strings.ApplicationSettingsWindow_BottomLeft,
                    _ => Strings.ApplicationSettingsWindow_BottomRight
                })));
        overlay.Children.Add(Row(Strings.TaskSettingsWindow_NativeRecorderCamera_Width,
            Number(Capture.ScreenRecordCameraWidthPercent, 5, 50, 1, value => { Capture.ScreenRecordCameraWidthPercent = (int)value; preview.InvalidateVisual(); })));
        overlay.Children.Add(Row(Strings.TaskSettingsWindow_NativeRecorderCamera_Margin,
            Number(Capture.ScreenRecordCameraMargin, 0, 1000, 1, value => { Capture.ScreenRecordCameraMargin = (int)value; preview.InvalidateVisual(); })));
        StackPanel capture = new() { Spacing = 10, Margin = new Thickness(0, 10) };
        capture.Children.Add(Row(Strings.TaskSettingsWindow_NativeRecorderCamera_Resolution,
            Choice(Enum.GetValues<CameraCaptureResolution>(), Capture.ScreenRecordCameraResolution,
                value => { Capture.ScreenRecordCameraResolution = value; preview.InvalidateVisual(); }, value => value switch
                {
                    CameraCaptureResolution.Size640x480 => "640 × 480",
                    CameraCaptureResolution.Size1920x1080 => "1920 × 1080",
                    _ => "1280 × 720"
                })));
        capture.Children.Add(Row(Strings.TaskSettingsWindow_NativeRecorderCamera_FPS,
            Number(Capture.ScreenRecordCameraFPS, 1, 60, 1, value => Capture.ScreenRecordCameraFPS = (int)value)));
        return Tabs((Strings.ScreenRecorderBar_Overlay, overlay), (Strings.ScreenRecorderBar_Capture, capture));
    }

    private Control BuildHighlighterPanel() => new MouseHighlighterSettingsControl(_draft.MouseHighlighter)
    {
        Margin = new Thickness(0, 8),
        HorizontalAlignment = HorizontalAlignment.Stretch
    };

    private Control BuildOptionsPanel()
    {
        StackPanel recording = new() { Spacing = 10, Margin = new Thickness(0, 8) };
        recording.Children.Add(Row(Strings.TaskSettingsWindow_ScreenRecordingFPS,
            Number(Capture.ScreenRecordFPS, 1, 120, 1, value => Capture.ScreenRecordFPS = (int)value)));
        recording.Children.Add(Row(Strings.TaskSettingsWindow_NativeRecorderBitrate,
            Number(Capture.ScreenRecordVideoBitrate, 100, 200000, 100, value => Capture.ScreenRecordVideoBitrate = (int)value)));
        NumericUpDown duration = Number((decimal)Capture.ScreenRecordDuration, 0.1m, 86400, 0.1m, value => Capture.ScreenRecordDuration = (float)value);
        duration.IsEnabled = Capture.ScreenRecordFixedDuration;
        recording.Children.Add(Check(Strings.TaskSettingsWindow_UseFixedDuration, Capture.ScreenRecordFixedDuration,
            value => { Capture.ScreenRecordFixedDuration = value; duration.IsEnabled = value; }));
        recording.Children.Add(Row(Strings.TaskSettingsWindow_DurationSeconds, duration));
        StackPanel bar = new() { Spacing = 10, Margin = new Thickness(0, 8) };
        bar.Children.Add(Check(Strings.ScreenRecorderBar_ShowBeforeRecording, Capture.ScreenRecordShowBar, value => Capture.ScreenRecordShowBar = value));
        bar.Children.Add(Check(Strings.TaskSettingsWindow_ShowRecordingTimer, Capture.ScreenRecordShowTimer, value => Capture.ScreenRecordShowTimer = value));
        bar.Children.Add(Check(Strings.TaskSettingsWindow_ShowRecordingButtonLabels, Capture.ScreenRecordShowButtonLabels,
            value => { Capture.ScreenRecordShowButtonLabels = value; BuildBar(); }));
        return Tabs((Strings.TaskSettingsWindow_Recording, recording), (Strings.ScreenRecorderBar_Bar, bar));
    }

    private void ShowRegion()
    {
        if (_closed) return;
        _regionWindow ??= new ScrollingCaptureRegionWindow(RecordingRegion) { Title = Title };
        _regionWindow.FindControl<RecordingRegionBorder>("RegionBorder")!.AccentBrush = Brushes.Goldenrod;
        _regionWindow.Show();
        // Keep the outline behind the interactive bar when the windows overlap.
        IntPtr barHandle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        IntPtr regionHandle = _regionWindow.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (barHandle != IntPtr.Zero && regionHandle != IntPtr.Zero)
        {
            NativeMethods.SetWindowPos(regionHandle, barHandle, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
        }
    }

    private void ShowPanel(string id, string title, Func<Control> build)
    {
        if (_activePanel == id && SettingsPanel.IsVisible) { ClosePanel(); return; }
        _activePanel = id;
        PanelTitle.Text = title;
        PanelContent.Content = build();
        SettingsPanel.IsVisible = true;
        Dispatcher.UIThread.Post(ClampToScreen, DispatcherPriority.Loaded);
    }

    private void ClosePanel()
    {
        SettingsPanel.IsVisible = false;
        PanelContent.Content = null;
        _activePanel = null;
        Dispatcher.UIThread.Post(ClampToScreen, DispatcherPriority.Loaded);
    }

    private void ShowError(string message)
    {
        ErrorMessage.Text = message;
        ErrorMessage.IsVisible = true;
    }

    private void PositionNearRegion()
    {
        if (_closed || !IsVisible) return;
        var screen = Screens.ScreenFromBounds(new PixelRect(RecordingRegion.X, RecordingRegion.Y, RecordingRegion.Width, RecordingRegion.Height)) ?? Screens.Primary;
        if (screen == null) return;
        PixelRect work = screen.WorkingArea;
        double scaling = screen.Scaling;
        int width = (int)Math.Ceiling(Bounds.Width * scaling);
        int height = (int)Math.Ceiling(Bounds.Height * scaling);
        int gap = (int)Math.Ceiling(8 * scaling);
        int top = RecordingRegion.Top - height - gap;
        // Regions at the screen's top edge keep the bar just inside that edge.
        if (top < work.Y) top = Math.Max(RecordingRegion.Top, work.Y) + gap;
        int left = Math.Max(RecordingRegion.Left, work.X);
        int right = Math.Min(RecordingRegion.Right, work.Right);
        if (right <= left) { left = work.X; right = work.Right; }
        Position = new PixelPoint(left + (right - left - width) / 2, top);
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        if (_closed || !IsVisible) return;
        var screen = Screens.ScreenFromPoint(Position) ?? Screens.Primary;
        if (screen == null) return;
        PixelRect work = screen.WorkingArea;
        double scaling = screen.Scaling;
        MaxWidth = Math.Max(280, work.Width / scaling - 16);
        PanelScroll.MaxHeight = Math.Max(100, work.Height / scaling - Toolbar.Bounds.Height - 110);
        int width = (int)Math.Ceiling(Bounds.Width * scaling);
        int height = (int)Math.Ceiling(Bounds.Height * scaling);
        PixelPoint clamped = new(Math.Clamp(Position.X, work.X, Math.Max(work.X, work.Right - width)),
            Math.Clamp(Position.Y, work.Y, Math.Max(work.Y, work.Bottom - height)));
        if (clamped != Position) Position = clamped;
    }

    private Control SplitControl(string label, string icon, bool enabled, Action<bool> changed, Action open) =>
        SplitControl(label, icon, enabled, changed, _ => open());

    private Control SplitControl(string label, string icon, bool enabled, Action<bool> changed, Action<Control> open)
    {
        StackPanel group = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(3, 2) };
        ToggleButton toggle = ToggleControl(label, icon, enabled, changed);
        toggle.CornerRadius = new(5, 0, 0, 5);
        toggle.Margin = new Thickness(0);
        Button arrow = ActionButton(string.Format(Strings.ScreenRecorderBar_SourceOptions, label), LucideIcons.chevron_down,
            () => open(group), iconOnly: true);
        arrow.CornerRadius = new(0, 5, 5, 0);
        arrow.Margin = new Thickness(0);
        arrow.Classes.Add("recorder-arrow");
        group.Children.Add(toggle);
        group.Children.Add(arrow);
        return group;
    }

    private ToggleButton ToggleControl(string label, string icon, bool enabled, Action<bool> changed)
    {
        ToggleButton toggle = new()
        {
            Content = ButtonContent(label, icon, enabled, toolbar: true),
            IsChecked = enabled,
            Margin = new Thickness(3, 2)
        };
        toggle.Classes.Add("recorder-setup");
        ToolTip.SetTip(toggle, label);
        AutomationProperties.SetName(toggle, label);
        toggle.Click += (_, _) =>
        {
            changed(toggle.IsChecked == true);
            toggle.Content = ButtonContent(label, icon, toggle.IsChecked == true, toolbar: true);
        };
        return toggle;
    }

    private Button ActionButton(string label, string icon, Action action, bool iconOnly = false, bool toolbar = false)
    {
        Button button = new() { Content = iconOnly ? Glyph(icon) : ButtonContent(label, icon, toolbar: toolbar), Margin = new Thickness(3, 2) };
        button.Classes.Add("recorder-setup");
        ToolTip.SetTip(button, label);
        AutomationProperties.SetName(button, label);
        button.Click += (_, _) => action();
        return button;
    }

    private Control ButtonContent(string label, string icon, bool? enabled = null, bool toolbar = false)
    {
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 6 };
        content.Children.Add(Glyph(icon));
        if (toolbar && !Capture.ScreenRecordShowButtonLabels) return content;
        content.Children.Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center });
        if (enabled != null) content.Children.Add(new TextBlock
        {
            Text = enabled.Value ? Strings.ScreenRecorderBar_On : Strings.ScreenRecorderBar_Off,
            FontSize = 11,
            Opacity = 0.8,
            VerticalAlignment = VerticalAlignment.Center
        });
        return content;
    }

    private static TextBlock Glyph(string glyph)
    {
        TextBlock icon = new() { Text = glyph, FontSize = 16, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
        icon.Classes.Add("icon");
        return icon;
    }

    private static TextBlock Hint(string text) => new() { Text = text, TextWrapping = TextWrapping.Wrap, FontSize = 12, Opacity = 0.75 };

    private static TextBlock TwoLineToolTip(string title, string detail) => new()
    {
        Text = $"{title}{Environment.NewLine}{detail}",
        TextAlignment = TextAlignment.Center
    };

    private static Control VolumeControl(float gain, Action<float> changed)
    {
        StackPanel panel = new() { Spacing = 4 };
        TextBlock percentage = new() { HorizontalAlignment = HorizontalAlignment.Right };
        Slider volume = new()
        {
            Minimum = 0,
            Maximum = 400,
            TickFrequency = 5,
            IsSnapToTickEnabled = true,
            SmallChange = 5,
            LargeChange = 25,
            Value = Math.Clamp(gain * 100, 0, 400),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetName(volume, Strings.ScreenRecorderBar_Volume);
        void Update()
        {
            percentage.Text = $"{volume.Value:0}%";
            changed((float)volume.Value / 100);
        }
        volume.ValueChanged += (_, _) => Update();
        percentage.Text = $"{volume.Value:0}%";
        panel.Children.Add(Row(Strings.ScreenRecorderBar_Volume, percentage));
        panel.Children.Add(volume);
        return panel;
    }

    private static CheckBox Check(string text, bool value, Action<bool> changed)
    {
        CheckBox check = new() { Content = text, IsChecked = value };
        check.IsCheckedChanged += (_, _) => changed(check.IsChecked == true);
        return check;
    }

    private static NumericUpDown Number(decimal value, decimal min, decimal max, decimal increment, Action<decimal> changed)
    {
        NumericUpDown number = new() { Minimum = min, Maximum = max, Increment = increment, Value = Math.Clamp(value, min, max), Width = 150 };
        number.ValueChanged += (_, _) => { if (number.Value is decimal current) changed(current); };
        return number;
    }

    private static ComboBox Choice<T>(T[] values, T? selected, Action<T> changed, Func<T, string> label)
    {
        ComboBox combo = new() { ItemsSource = values.Select(value => new ChoiceValue<T>(value, label(value))).ToArray(), MinWidth = 150 };
        combo.SelectedIndex = Array.IndexOf(values, selected!);
        combo.SelectionChanged += (_, _) => { if (combo.SelectedItem is ChoiceValue<T> choice) changed(choice.Value); };
        return combo;
    }

    private static Control Row(string label, Control control)
    {
        Grid row = new() { ColumnDefinitions = new("*,Auto"), ColumnSpacing = 12 };
        row.Children.Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 3) });
        Grid.SetColumn(control, 1);
        row.Children.Add(control);
        return row;
    }

    private static TabControl Tabs(params (string Title, Control Content)[] tabs) => new()
    {
        ItemsSource = tabs.Select(tab => new TabItem { Header = tab.Title, Content = tab.Content, FontSize = 14 }).ToArray()
    };

    private sealed record DeviceChoice(string Id, string Name) { public override string ToString() => Name; }
    private sealed record ChoiceValue<T>(T Value, string Label) { public override string ToString() => Label; }

    private sealed class Source(string label, string icon, Func<bool> enabled, Action<bool> setEnabled,
        Func<string> selected, Action<string> setSelected, Func<DeviceChoice[]> enumerate, Func<string?>? defaultName,
        string defaultLabel, string unavailableLabel)
    {
        public string Label { get; } = label;
        public string Icon { get; } = icon;
        public Func<bool> Enabled { get; } = enabled;
        public Action<bool> SetEnabled { get; } = setEnabled;
        public Func<string> Selected { get; } = selected;
        public Action<string> SetSelected { get; } = setSelected;
        public Func<DeviceChoice[]> Enumerate { get; } = enumerate;
        public Func<string?>? DefaultName { get; } = defaultName;
        public string DefaultLabel { get; } = defaultLabel;
        public string UnavailableLabel { get; } = unavailableLabel;
        public DeviceChoice[] Devices { get; set; } = [];
        public string? ResolvedDefault { get; set; }
        public bool Available { get; set; }
        public Control Control { get; set; } = null!;
        public ToggleButton Toggle { get; set; } = null!;
    }
}
