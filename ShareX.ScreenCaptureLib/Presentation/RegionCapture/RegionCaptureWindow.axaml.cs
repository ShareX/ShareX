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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Input;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaCanvas = Avalonia.Controls.Canvas;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

public partial class RegionCaptureWindow : Window
{
    private const double MagnifierSize = 150;
    private const int MinimumMagnifierPixelSize = 6;

    private readonly TaskCompletionSource<AvaloniaRegionCaptureResult?> _completionSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private AvaloniaRegionCaptureRequest? _request;
    private AvaloniaRegionCaptureResult? _pendingResult;
    private EditorView _editorWorkspace = null!;
    private MainViewModel? _viewModel;
    private Grid _regionInputSurface = null!;
    private RegionSelectionOverlay _regionOverlay = null!;
    private AvaloniaCanvas _regionResizeNodeCanvas = null!;
    private LayoutTransformControl _regionTransform = null!;
    private Grid _captureToolbar = null!;
    private AnnotationToolbar _annotationToolbar = null!;
    private Button _regionToolButton = null!;
    private StackPanel _magnifierPanel = null!;
    private Grid _magnifierView = null!;
    private Image _magnifierImage = null!;
    private MagnifierPixelGrid _magnifierPixelGrid = null!;
    private Border _pointerInfoPanel = null!;
    private TextBlock _pointerInfoText = null!;
    private Border _selectionInfoPanel = null!;
    private TextBlock _selectionInfoText = null!;
    private WriteableBitmap? _magnifierBitmap;
    private IReadOnlyList<SimpleWindowInfo> _windows = Array.Empty<SimpleWindowInfo>();
    private readonly Dictionary<SelectionResizeNodeKind, Border> _regionResizeNodes = [];
    private SimpleWindowInfo? _hoverCandidate;
    private SimpleWindowInfo? _selectedCandidate;
    private RegionInteraction _interaction;
    private SelectionResizeNodeKind? _resizeNode;
    private Point _pressPoint;
    private Point _lastPointerPoint;
    private Point _lastCreationPoint;
    private Rect _interactionStartRectangle;
    private readonly TranslateTransform _magnifierTransform = new();
    private double _captureToolbarCenterX = double.NaN;
    private double _captureToolbarTop = double.NaN;
    private double _positionedCaptureToolbarWidth = double.NaN;
    private int _imageWidth;
    private int _imageHeight;
    private bool _regionToolActive = true;
    private bool _keyboardInputEnabled;
    private bool _isMovingSelectionDuringCreation;
    private bool _wasControlHeldDuringCreation;
    private bool _suppressNextRightButtonReleaseAction;
    private bool _annotationRightButtonPressed;
    private bool _workspaceOwnsScreenshot;
    private bool _closing;
    private Exception? _startupException;

    public RegionCaptureWindow()
    {
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        AvaloniaXamlLoader.Load(this);
#if !DEBUG
        Topmost = true;
#endif
        ResolveControls();
        AddHandler(PointerPressedEvent, OnCaptureHostPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(PointerReleasedEvent, OnCaptureHostPointerReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    public RegionCaptureWindow(AvaloniaRegionCaptureRequest request) : this()
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
        _imageWidth = Math.Max(1, request.ScreenBounds.Width);
        _imageHeight = Math.Max(1, request.ScreenBounds.Height);
        InitializeCaptureWorkspace();
        Opened += OnOpened;
    }

    public Task<AvaloniaRegionCaptureResult?> CaptureAsync()
    {
        if (_request == null)
        {
            throw new InvalidOperationException("A capture request is required.");
        }

        ConfigureInitialPixelBounds();
        Show();
        return _completionSource.Task;
    }

    protected override void OnClosed(EventArgs e)
    {
        Exception? closeException = _startupException;

        try
        {
            _magnifierBitmap?.Dispose();
            _magnifierBitmap = null;
            _captureToolbar.LayoutUpdated -= OnCaptureToolbarLayoutUpdated;
            _editorWorkspace.DetachHostAnnotationToolbar(_annotationToolbar);

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.ToolSelectionRequested -= OnAnnotationToolSelected;
            }

            if (_workspaceOwnsScreenshot)
            {
                _editorWorkspace.DisposeWorkspace();
            }
            else
            {
                _request?.Screenshot.Dispose();
            }

            _request?.CursorBitmap?.Dispose();
        }
        catch (Exception ex)
        {
            closeException ??= ex;
        }
        finally
        {
            if (closeException != null)
            {
                _completionSource.TrySetException(closeException);
            }
            else
            {
                _completionSource.TrySetResult(_pendingResult);
            }

            base.OnClosed(e);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_keyboardInputEnabled && !_closing && _request != null &&
            e.Key == Key.Tab && e.KeyModifiers == KeyModifiers.None && _viewModel?.IsModalOpen != true)
        {
            ActivateRegionTool();
            e.Handled = true;
            return;
        }

        if (_keyboardInputEnabled && !_closing && e.Key == Key.H && e.KeyModifiers == KeyModifiers.Control)
        {
            _captureToolbar.Classes.Set("hidden", !_captureToolbar.Classes.Contains("hidden"));
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);

        if (!_keyboardInputEnabled || _closing || _request == null)
        {
            return;
        }

        if (_regionToolActive && _interaction == RegionInteraction.Creating &&
            (e.Key is Key.LeftShift or Key.RightShift))
        {
            UpdateSelectionDuringCreation(_lastCreationPoint, e.KeyModifiers | KeyModifiers.Shift);
            e.Handled = true;
            return;
        }

        if (_regionToolActive && e.Key == Key.Enter && HasValidSelection())
        {
            e.Handled = true;
            Complete(_regionOverlay.SelectionRectangle);
            return;
        }

        if (_regionToolActive && e.Key == Key.Space)
        {
            e.Handled = true;
            Complete(new Rect(0, 0, _imageWidth, _imageHeight), includeWindowInfo: false);
            return;
        }

        if (_regionToolActive && e.Key == Key.OemTilde)
        {
            e.Handled = true;
            CompleteActiveMonitor();
            return;
        }

        if (_regionToolActive && e.Key is Key.Left or Key.Right or Key.Up or Key.Down)
        {
            int distance = e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                ? RegionCaptureOptions.MoveSpeedMaximum
                : RegionCaptureOptions.MoveSpeedMinimum;
            int dx = e.Key == Key.Left ? -distance : e.Key == Key.Right ? distance : 0;
            int dy = e.Key == Key.Up ? -distance : e.Key == Key.Down ? distance : 0;
            System.Windows.Forms.Cursor.Position = System.Windows.Forms.Cursor.Position.Add(dx, dy);
            e.Handled = true;
            return;
        }

        if (_regionToolActive && e.Key == Key.Delete && HasValidSelection())
        {
            ClearSelection();
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if ((_interaction is RegionInteraction.Creating or RegionInteraction.PendingHover) &&
            (e.Key is Key.LeftCtrl or Key.RightCtrl))
        {
            _isMovingSelectionDuringCreation = false;
            _wasControlHeldDuringCreation = false;
        }

        if (_keyboardInputEnabled && !_closing && _request != null && _regionToolActive &&
            _interaction == RegionInteraction.Creating && (e.Key is Key.LeftShift or Key.RightShift))
        {
            UpdateSelectionDuringCreation(_lastCreationPoint, e.KeyModifiers & ~KeyModifiers.Shift);
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Escape || e.KeyModifiers != KeyModifiers.None || _closing)
        {
            return;
        }

        e.Handled = true;

        if (!_regionToolActive && _editorWorkspace.CancelActiveInteractionOrSelection())
        {
            return;
        }

        CancelCapture();
    }

    private void ResolveControls()
    {
        _editorWorkspace = this.FindControl<EditorView>("EditorWorkspace")!;
        _regionInputSurface = this.FindControl<Grid>("RegionInputSurface")!;
        _regionOverlay = this.FindControl<RegionSelectionOverlay>("RegionOverlay")!;
        _regionResizeNodeCanvas = this.FindControl<AvaloniaCanvas>("RegionResizeNodeCanvas")!;
        _regionTransform = this.FindControl<LayoutTransformControl>("RegionTransform")!;
        _captureToolbar = this.FindControl<Grid>("CaptureToolbar")!;
        _captureToolbar.LayoutUpdated += OnCaptureToolbarLayoutUpdated;
        _annotationToolbar = this.FindControl<AnnotationToolbar>("CaptureAnnotationToolbar")!;
        _regionToolButton = this.FindControl<Button>("RegionToolButton")!;
        _magnifierPanel = this.FindControl<StackPanel>("MagnifierPanel")!;
        _magnifierPanel.RenderTransform = _magnifierTransform;
        _magnifierView = this.FindControl<Grid>("MagnifierView")!;
        _magnifierImage = this.FindControl<Image>("MagnifierImage")!;
        _magnifierPixelGrid = this.FindControl<MagnifierPixelGrid>("MagnifierPixelGrid")!;
        _pointerInfoPanel = this.FindControl<Border>("PointerInfoPanel")!;
        _pointerInfoText = this.FindControl<TextBlock>("PointerInfoText")!;
        _selectionInfoPanel = this.FindControl<Border>("SelectionInfoPanel")!;
        _selectionInfoText = this.FindControl<TextBlock>("SelectionInfoText")!;
    }

    private void InitializeCaptureWorkspace()
    {
        if (_request == null)
        {
            return;
        }

        _viewModel = new MainViewModel(_request.EditorOptions)
        {
            ShowFileMenu = false,
            ShowOptionsButton = false,
            ShowTaskButtons = false,
            ShowBottomToolbar = false,
            ShowToolbars = false,
            ShowStartScreen = false,
            UseContinueWorkflow = false
        };
        _viewModel.SetHostToolbarFilter(item =>
            _request.EnableAnnotations &&
            item.Tool.HasValue &&
            item.Tool.Value is not EditorTool.Crop and not EditorTool.CutOut);
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _viewModel.ToolSelectionRequested += OnAnnotationToolSelected;

        _editorWorkspace.DataContext = _viewModel;
        _annotationToolbar.DataContext = _viewModel.ToolbarAdapter;
        _editorWorkspace.AttachHostAnnotationToolbar(_annotationToolbar);
        _annotationToolbar.IsVisible = _request.EnableAnnotations;
        _annotationToolbar.ShowToolOptions = false;

        _regionInputSurface.Width = _imageWidth;
        _regionInputSurface.Height = _imageHeight;
        _regionOverlay.DimAlpha = GetDimAlpha(_request.CaptureOptions);
        _regionOverlay.ShowCenterCrosshair = _request.CaptureOptions.ShowCenterCrosshair;
        _regionOverlay.ShowCursorCrosshair = _request.CaptureOptions.ShowScreenCrosshair;
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(GetInitialScaling());
        InitializeRegionResizeNodes();

        _magnifierView.IsVisible = _request.CaptureOptions.ShowMagnifier;
        ApplyMagnifierShape();
        _pointerInfoPanel.IsVisible = _request.CaptureOptions.ShowInfo;
        _magnifierPanel.IsVisible = _magnifierView.IsVisible || _pointerInfoPanel.IsVisible;
        _regionToolButton.Classes.Set("active", true);
        _viewModel.SetHostToolbarToolsActive(false);
        Title = Localization.Strings.BaseRegionForm_InitializeComponent_Region_capture;
    }

    private void ApplyMagnifierShape()
    {
        bool useSquare = _request?.CaptureOptions.UseSquareMagnifier == true;
        this.FindControl<Control>("MagnifierCircleOuter")!.IsVisible = !useSquare;
        this.FindControl<Control>("MagnifierCircleInner")!.IsVisible = !useSquare;
        this.FindControl<Control>("MagnifierSquareOuter")!.IsVisible = useSquare;
        this.FindControl<Control>("MagnifierSquareInner")!.IsVisible = useSquare;

        Grid magnifierContent = this.FindControl<Grid>("MagnifierContent")!;
        magnifierContent.Clip = useSquare
            ? null
            : new EllipseGeometry(new Rect(0, 0, MagnifierSize, MagnifierSize));
    }

    private void InitializeRegionResizeNodes()
    {
        double scaling = GetInitialScaling();
        foreach (SelectionResizeNodeKind node in SelectionResizeNode.RectangleNodes)
        {
            Border control = SelectionResizeNode.Create(
                0,
                0,
                node,
                CursorAssetLoader.GetOpenHandCursor(scaling));
            _regionResizeNodeCanvas.Children.Add(control);
            _regionResizeNodes.Add(node, control);
        }
    }

    private void SetRegionResizeNodesVisible(bool visible)
    {
        _regionResizeNodeCanvas.IsVisible = visible;
        if (visible)
        {
            UpdateRegionResizeNodePositions();
        }
    }

    private void UpdateRegionResizeNodePositions()
    {
        Rect selection = _regionOverlay.SelectionRectangle;
        if (!RegionSelectionOverlay.IsValid(selection))
        {
            return;
        }

        foreach ((SelectionResizeNodeKind node, Border control) in _regionResizeNodes)
        {
            SelectionResizeNode.SetPosition(control, SelectionResizeNode.GetPosition(selection, node));
        }
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        try
        {
            if (_request == null || _viewModel == null)
            {
                CancelCapture();
                return;
            }

            ApplyPixelSize();
            UpdatePixelTransforms();
            _editorWorkspace.ConfigureForFullscreenWorkspace();
            _editorWorkspace.LoadWorkspaceImage(_request.Screenshot);
            _workspaceOwnsScreenshot = true;

            if (_request.CursorBitmap != null)
            {
                _editorWorkspace.InsertWorkspaceImageAnnotation(
                    _request.CursorBitmap,
                    new Point(_request.CursorPosition.X, _request.CursorPosition.Y));
                ActivateRegionTool();
            }

            Activate();
            Focus();
            _regionInputSurface.Focus();

            DrawingPoint cursorPosition = System.Windows.Forms.Control.MousePosition;
            _lastPointerPoint = ClampPoint(new Point(
                cursorPosition.X - _request.ScreenBounds.X,
                cursorPosition.Y - _request.ScreenBounds.Y));
            UpdateToolbarPosition(new PixelPoint(cursorPosition.X, cursorPosition.Y));
            UpdateHud(_lastPointerPoint);

            _ = LoadWindowRegionsAsync();

            int inputDelay = Math.Max(0, _request.CaptureOptions.InputDelay);
            if (inputDelay > 0)
            {
                await Task.Delay(inputDelay);
            }

            if (!_closing)
            {
                _keyboardInputEnabled = true;
            }
        }
        catch (Exception ex)
        {
            _startupException = ex;
            CancelCapture();
        }
    }

    private async Task LoadWindowRegionsAsync()
    {
        if (_request == null || !_request.CaptureOptions.DetectWindows)
        {
            return;
        }

        try
        {
            IntPtr ignoredHandle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            WindowsRectangleList windows = new WindowsRectangleList
            {
                IncludeChildWindows = _request.CaptureOptions.DetectControls,
                Timeout = 5000
            };

            if (ignoredHandle != IntPtr.Zero)
            {
                windows.IgnoreHandleList.Add(ignoredHandle);
            }

            IReadOnlyList<SimpleWindowInfo> result = await Task.Run(windows.GetWindowInfoList);
            if (!_closing)
            {
                _windows = result;
                UpdateHover(_lastPointerPoint);
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.ActiveTool))
        {
            ActivateAnnotationTool();
        }
    }

    private void OnAnnotationToolSelected(object? sender, EditorTool tool)
    {
        ActivateAnnotationTool();
    }

    private void OnRegionToolClick(object? sender, RoutedEventArgs e)
    {
        ActivateRegionTool();
        e.Handled = true;
    }

    private void ActivateRegionTool()
    {
        _regionToolActive = true;
        _regionInputSurface.IsVisible = true;
        _regionInputSurface.IsHitTestVisible = true;
        _regionOverlay.IsVisible = true;
        SetRegionResizeNodesVisible(_request?.CaptureOptions.QuickCapture == false && HasValidSelection());
        _regionToolButton.Classes.Set("active", true);
        _viewModel?.SetHostToolbarToolsActive(false);
        _annotationToolbar.ShowToolOptions = false;
        _editorWorkspace.CancelActiveInteractionOrSelection();
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(Math.Max(1, RenderScaling));
        bool showMagnifier = _request?.CaptureOptions.ShowMagnifier == true;
        bool showInfo = _request?.CaptureOptions.ShowInfo == true;
        _magnifierView.IsVisible = showMagnifier;
        _pointerInfoPanel.IsVisible = showInfo;
        _magnifierPanel.IsVisible = showMagnifier || showInfo;
        UpdateHover(_lastPointerPoint);
        UpdateHud(_lastPointerPoint);
        _regionInputSurface.Focus();
    }

    private void ActivateAnnotationTool()
    {
        if (_request?.EnableAnnotations != true)
        {
            return;
        }

        _regionToolActive = false;
        _interaction = RegionInteraction.None;
        _regionInputSurface.IsVisible = false;
        _regionInputSurface.IsHitTestVisible = false;
        _regionOverlay.IsVisible = false;
        SetRegionResizeNodesVisible(false);
        _regionOverlay.HoverRectangle = default;
        _hoverCandidate = null;
        _regionToolButton.Classes.Set("active", false);
        _viewModel?.SetHostToolbarToolsActive(true);
        _annotationToolbar.ShowToolOptions = true;
        _magnifierPanel.IsVisible = false;
        _selectionInfoPanel.IsVisible = false;
        _editorWorkspace.Focus();
    }

    private void OnRegionPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_closing || _request == null || !_regionToolActive)
        {
            return;
        }

        Point point = ClampPoint(e.GetPosition(_regionInputSurface));
        _lastPointerPoint = point;
        PointerPointProperties properties = e.GetCurrentPoint(_regionInputSurface).Properties;

        if (TryCancelRegionCreation(e, point))
        {
            return;
        }

        if (properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            e.Handled = true;
            return;
        }

        if (properties.IsMiddleButtonPressed)
        {
            RunCaptureAction(_request.CaptureOptions.RegionCaptureActionMiddleClick);
            e.Handled = true;
            return;
        }

        if (properties.IsXButton1Pressed)
        {
            RunCaptureAction(_request.CaptureOptions.RegionCaptureActionX1Click);
            e.Handled = true;
            return;
        }

        if (properties.IsXButton2Pressed)
        {
            RunCaptureAction(_request.CaptureOptions.RegionCaptureActionX2Click);
            e.Handled = true;
            return;
        }

        if (properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed)
        {
            return;
        }

        if (e.ClickCount >= 2 && HasValidSelection() && _regionOverlay.SelectionRectangle.Contains(point))
        {
            Complete(_regionOverlay.SelectionRectangle);
            e.Handled = true;
            return;
        }

        _pressPoint = point;
        _interactionStartRectangle = _regionOverlay.SelectionRectangle;
        _resizeNode = SelectionResizeNode.TryGetKind((e.Source as Control)?.Tag, out SelectionResizeNodeKind resizeNode)
            ? resizeNode
            : null;

        if (_resizeNode.HasValue)
        {
            _interaction = RegionInteraction.Resizing;
        }
        else if (HasValidSelection() && _regionOverlay.SelectionRectangle.Contains(point))
        {
            _interaction = RegionInteraction.Moving;
        }
        else if (RegionSelectionOverlay.IsValid(_regionOverlay.HoverRectangle))
        {
            SetSelection(_regionOverlay.HoverRectangle, _hoverCandidate);
            _interactionStartRectangle = _regionOverlay.SelectionRectangle;
            _interaction = RegionInteraction.PendingHover;
        }
        else
        {
            SetSelection(default, null);
            _interaction = RegionInteraction.Creating;
        }

        if (_interaction is RegionInteraction.Creating or RegionInteraction.PendingHover)
        {
            _lastCreationPoint = point;
            _isMovingSelectionDuringCreation = false;
            _wasControlHeldDuringCreation = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        }

        SetRegionResizeNodesVisible(false);
        if (_interaction is RegionInteraction.Moving or RegionInteraction.Resizing)
        {
            _regionInputSurface.Cursor = CursorAssetLoader.GetClosedHandCursor(Math.Max(1, RenderScaling));
        }
        e.Pointer.Capture(_regionInputSurface);
        e.Handled = true;
    }

    private void OnRegionPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_closing || _request == null || !_regionToolActive)
        {
            return;
        }

        Point point = ClampPoint(e.GetPosition(_regionInputSurface));
        _lastPointerPoint = point;

        if (TryCancelRegionCreation(e, point))
        {
            return;
        }

        if (TryHandleRightButtonRelease(e))
        {
            return;
        }

        UpdateHud(point);

        switch (_interaction)
        {
            case RegionInteraction.None:
                UpdateHover(point);
                break;
            case RegionInteraction.PendingHover:
                if (Distance(_pressPoint, point) >= 4)
                {
                    _interaction = RegionInteraction.Creating;
                    _selectedCandidate = null;
                    SetSelection(default, null);
                    UpdateSelectionDuringCreation(point, e.KeyModifiers);
                }
                break;
            case RegionInteraction.Creating:
                UpdateSelectionDuringCreation(point, e.KeyModifiers);
                break;
            case RegionInteraction.Moving:
                MoveSelection(point.X - _pressPoint.X, point.Y - _pressPoint.Y, _interactionStartRectangle);
                break;
            case RegionInteraction.Resizing:
                ResizeSelection(point);
                break;
        }

        e.Handled = true;
    }

    private void OnRegionPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_closing || _request == null || !_regionToolActive)
        {
            return;
        }

        Point point = ClampPoint(e.GetPosition(_regionInputSurface));
        _lastPointerPoint = point;

        if (TryCancelRegionCreation(e, point))
        {
            return;
        }

        PointerPointProperties properties = e.GetCurrentPoint(_regionInputSurface).Properties;
        PointerUpdateKind updateKind = properties.PointerUpdateKind;
        if (TryHandleRightButtonRelease(e))
        {
            return;
        }

        if (_interaction == RegionInteraction.None)
        {
            if (updateKind == PointerUpdateKind.LeftButtonReleased && !properties.IsRightButtonPressed)
            {
                _suppressNextRightButtonReleaseAction = false;
            }

            return;
        }

        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return;
        }

        e.Pointer.Capture(null);
        _interaction = RegionInteraction.None;
        _resizeNode = null;
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(Math.Max(1, RenderScaling));
        ResetCreationModifiers();

        Rect selection = _regionOverlay.SelectionRectangle;
        if (selection.Width < _request.CaptureOptions.MinimumSize || selection.Height < _request.CaptureOptions.MinimumSize)
        {
            if (RegionSelectionOverlay.IsValid(_regionOverlay.HoverRectangle))
            {
                SetSelection(_regionOverlay.HoverRectangle, _hoverCandidate);
            }
            else
            {
                ClearSelection();
            }
        }

        if (HasValidSelection() && _request.CaptureOptions.QuickCapture)
        {
            Complete(_regionOverlay.SelectionRectangle);
        }
        else
        {
            SetRegionResizeNodesVisible(HasValidSelection());
            UpdateHud(_lastPointerPoint);
        }

        e.Handled = true;
    }

    private bool TryCancelRegionCreation(PointerEventArgs e, Point point)
    {
        PointerUpdateKind updateKind = e.GetCurrentPoint(_regionInputSurface).Properties.PointerUpdateKind;
        if (updateKind is not PointerUpdateKind.RightButtonPressed and not PointerUpdateKind.RightButtonReleased ||
            _interaction is not (RegionInteraction.Creating or RegionInteraction.PendingHover))
        {
            return false;
        }

        _suppressNextRightButtonReleaseAction = updateKind == PointerUpdateKind.RightButtonPressed;
        e.Pointer.Capture(null);
        ClearSelection();
        UpdateHud(point);
        e.Handled = true;
        return true;
    }

    private bool TryHandleRightButtonRelease(PointerEventArgs e)
    {
        if (e.GetCurrentPoint(_regionInputSurface).Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
        {
            return false;
        }

        if (_suppressNextRightButtonReleaseAction)
        {
            _suppressNextRightButtonReleaseAction = false;
        }
        else if (_request != null)
        {
            RunCaptureAction(_request.CaptureOptions.RegionCaptureActionRightClick);
        }

        e.Handled = true;
        return true;
    }

    private void OnRegionPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_request == null || !_regionToolActive)
        {
            return;
        }

        int delta = e.Delta.Y > 0 ? -2 : 2;
        int count = NormalizeMagnifierPixelCount(_request.CaptureOptions.MagnifierPixelCount + delta);

        _request.CaptureOptions.MagnifierPixelCount = count;
        _request.CaptureOptions.ShowMagnifier = true;
        RecreateMagnifierBitmap();
        UpdateHud(_lastPointerPoint);
        e.Handled = true;
    }

    private void UpdateToolbarPosition(PixelPoint desktopPoint)
    {
        if (_request == null)
        {
            return;
        }

        Screen? screen = Screens.ScreenFromPoint(desktopPoint);
        if (screen == null)
        {
            return;
        }

        double scale = double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;
        double monitorCenterX = screen.Bounds.X - _request.ScreenBounds.X + screen.Bounds.Width / 2d;
        double monitorTop = screen.Bounds.Y - _request.ScreenBounds.Y;
        _captureToolbarCenterX = monitorCenterX / scale;
        _captureToolbarTop = monitorTop / scale;
        _positionedCaptureToolbarWidth = double.NaN;
        PositionCaptureToolbar();
    }

    private void OnCaptureToolbarLayoutUpdated(object? sender, EventArgs e)
    {
        PositionCaptureToolbar();
    }

    private void PositionCaptureToolbar()
    {
        if (!double.IsFinite(_captureToolbarCenterX) || !double.IsFinite(_captureToolbarTop))
        {
            return;
        }

        double toolbarWidth = _captureToolbar.Bounds.Width > 0
            ? _captureToolbar.Bounds.Width
            : _captureToolbar.DesiredSize.Width;
        if (!double.IsFinite(toolbarWidth) || toolbarWidth <= 0 ||
            Math.Abs(toolbarWidth - _positionedCaptureToolbarWidth) < 0.01)
        {
            return;
        }

        _positionedCaptureToolbarWidth = toolbarWidth;
        AvaloniaCanvas.SetLeft(_captureToolbar, _captureToolbarCenterX - toolbarWidth / 2d);
        AvaloniaCanvas.SetTop(_captureToolbar, _captureToolbarTop);
    }

    private void OnCaptureHostPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_regionToolActive || _closing || _request == null || !IsEditorWorkspaceSource(e.Source))
        {
            return;
        }

        if (e.GetCurrentPoint(_editorWorkspace).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            if (_viewModel?.IsModalOpen == true || IsEditorNotificationSource(e.Source))
            {
                _annotationRightButtonPressed = false;
                return;
            }

            _annotationRightButtonPressed = true;
            e.Handled = true;
        }
    }

    private void OnCaptureHostPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_annotationRightButtonPressed || _regionToolActive || _closing || _request == null)
        {
            return;
        }

        if (e.GetCurrentPoint(_editorWorkspace).Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
        {
            return;
        }

        _annotationRightButtonPressed = false;
        RunAnnotationRightClickAction(e.GetPosition(_editorWorkspace));
        e.Handled = true;
    }

    private bool IsEditorWorkspaceSource(object? source)
    {
        Visual? visual = source as Visual;
        while (visual != null)
        {
            if (ReferenceEquals(visual, _editorWorkspace))
            {
                return true;
            }

            visual = visual.GetVisualParent();
        }

        return false;
    }

    private static bool IsEditorNotificationSource(object? source)
    {
        Visual? visual = source as Visual;
        while (visual != null)
        {
            if (visual is Control { Name: "EditorNotificationHost" })
            {
                return true;
            }

            visual = visual.GetVisualParent();
        }

        return false;
    }

    private void RunAnnotationRightClickAction(Point workspacePoint)
    {
        if (_request == null)
        {
            return;
        }

        RegionCaptureAction action = _request.CaptureOptions.RegionCaptureActionRightClick;
        if (action == RegionCaptureAction.RemoveShapeCancelCapture)
        {
            if (!_editorWorkspace.DeleteWorkspaceAnnotationAt(workspacePoint))
            {
                CancelCapture();
            }
        }
        else if (action == RegionCaptureAction.RemoveShape)
        {
            _editorWorkspace.DeleteWorkspaceAnnotationAt(workspacePoint);
        }
        else
        {
            RunCaptureAction(action);
        }
    }

    private void UpdateHover(Point imagePoint)
    {
        if (_request == null || HasValidSelection() || _interaction != RegionInteraction.None)
        {
            _regionOverlay.HoverRectangle = default;
            _hoverCandidate = null;
            return;
        }

        double screenX = _request.ScreenBounds.X + Math.Round(imagePoint.X);
        double screenY = _request.ScreenBounds.Y + Math.Round(imagePoint.Y);
        SimpleWindowInfo? candidate = _windows.FirstOrDefault(window =>
            ContainsPoint(window.Rectangle, screenX, screenY));

        if (candidate == null)
        {
            _hoverCandidate = null;
            _regionOverlay.HoverRectangle = default;
            return;
        }

        DrawingRectangle candidateRectangle = candidate.Rectangle;
        double candidateLeft = (double)candidateRectangle.X - _request.ScreenBounds.X;
        double candidateTop = (double)candidateRectangle.Y - _request.ScreenBounds.Y;
        double left = Math.Max(0, candidateLeft);
        double top = Math.Max(0, candidateTop);
        double right = Math.Min(_imageWidth, candidateLeft + candidateRectangle.Width);
        double bottom = Math.Min(_imageHeight, candidateTop + candidateRectangle.Height);
        Rect hover = right > left && bottom > top
            ? new Rect(left, top, right - left, bottom - top)
            : default;

        _hoverCandidate = RegionSelectionOverlay.IsValid(hover) ? candidate : null;
        _regionOverlay.HoverRectangle = hover;
    }

    private static bool ContainsPoint(DrawingRectangle rectangle, double x, double y)
    {
        return rectangle.Width > 0 && rectangle.Height > 0 &&
            x >= rectangle.X && y >= rectangle.Y &&
            x < (double)rectangle.X + rectangle.Width &&
            y < (double)rectangle.Y + rectangle.Height;
    }

    private void SetSelection(Rect rectangle, SimpleWindowInfo? candidate)
    {
        _regionOverlay.SelectionRectangle = RegionSelectionOverlay.Intersect(
            rectangle,
            new Rect(0, 0, GetImageSize().Width, GetImageSize().Height));
        _selectedCandidate = candidate;
        if (_regionResizeNodeCanvas.IsVisible)
        {
            UpdateRegionResizeNodePositions();
        }
        UpdateSelectionInfo();
    }

    private void ClearSelection()
    {
        _interaction = RegionInteraction.None;
        ResetCreationModifiers();
        _selectedCandidate = null;
        _regionOverlay.SelectionRectangle = default;
        SetRegionResizeNodesVisible(false);
        _selectionInfoPanel.IsVisible = false;
        UpdateHover(_lastPointerPoint);
    }

    private void MoveSelection(double dx, double dy)
    {
        MoveSelection(dx, dy, _regionOverlay.SelectionRectangle);
    }

    private void MoveSelection(double dx, double dy, Rect source)
    {
        Size bounds = GetImageSize();
        double x = Math.Clamp(source.X + dx, 0, Math.Max(0, bounds.Width - source.Width));
        double y = Math.Clamp(source.Y + dy, 0, Math.Max(0, bounds.Height - source.Height));
        SetSelection(new Rect(x, y, source.Width, source.Height), null);
    }

    private void UpdateSelectionDuringCreation(Point point, KeyModifiers modifiers)
    {
        bool controlHeld = modifiers.HasFlag(KeyModifiers.Control);
        if (!controlHeld)
        {
            _isMovingSelectionDuringCreation = false;
        }
        else if (RegionSelectionOverlay.IsValid(_regionOverlay.SelectionRectangle) &&
            (_isMovingSelectionDuringCreation || !_wasControlHeldDuringCreation))
        {
            MoveSelectionDuringCreation(point);
            _isMovingSelectionDuringCreation = true;
            _lastCreationPoint = point;
            _wasControlHeldDuringCreation = true;
            return;
        }

        bool constrainToSquare = modifiers.HasFlag(KeyModifiers.Shift);
        SetSelection(CreateSelectionRectangle(_pressPoint, point, GetImageSize(), constrainToSquare), null);
        _lastCreationPoint = point;
        _wasControlHeldDuringCreation = controlHeld;
    }

    private void MoveSelectionDuringCreation(Point point)
    {
        Rect before = _regionOverlay.SelectionRectangle;
        MoveSelection(point.X - _lastCreationPoint.X, point.Y - _lastCreationPoint.Y, before);
        Rect after = _regionOverlay.SelectionRectangle;
        _pressPoint = ClampPoint(new Point(
            _pressPoint.X + after.X - before.X,
            _pressPoint.Y + after.Y - before.Y));
    }

    private static Rect CreateSelectionRectangle(Point first, Point second, Size bounds, bool constrainToSquare)
    {
        if (!constrainToSquare)
        {
            return RegionSelectionOverlay.NormalizeAndClamp(first, second, bounds);
        }

        double deltaX = second.X - first.X;
        double deltaY = second.Y - first.Y;
        double side = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
        double availableWidth = deltaX < 0 ? first.X : bounds.Width - first.X;
        double availableHeight = deltaY < 0 ? first.Y : bounds.Height - first.Y;
        side = Math.Max(0, Math.Min(side, Math.Min(availableWidth, availableHeight)));

        Point constrained = new(
            first.X + (deltaX < 0 ? -side : side),
            first.Y + (deltaY < 0 ? -side : side));
        return RegionSelectionOverlay.NormalizeAndClamp(first, constrained, bounds);
    }

    private void ResetCreationModifiers()
    {
        _isMovingSelectionDuringCreation = false;
        _wasControlHeldDuringCreation = false;
    }

    private void ResizeSelection(Point point)
    {
        if (!_resizeNode.HasValue)
        {
            return;
        }

        Vector delta = point - _pressPoint;
        Rect resized = SelectionResizeNode.Resize(_regionOverlay.SelectionRectangle, _resizeNode.Value, delta);
        SetSelection(resized, null);
        _pressPoint = point;
    }

    private void UpdateHud(Point imagePoint)
    {
        if (_request == null)
        {
            return;
        }

        _regionOverlay.CursorPosition = imagePoint;

        bool showMagnifier = _request.CaptureOptions.ShowMagnifier;
        bool showInfo = _request.CaptureOptions.ShowInfo;

        if (showMagnifier || showInfo)
        {
            if (showMagnifier)
            {
                UpdateMagnifier(imagePoint);
            }

            _magnifierView.IsVisible = showMagnifier;
            _pointerInfoPanel.IsVisible = showInfo;
            _magnifierPanel.IsVisible = true;
            _pointerInfoText.Text = $"X: {_request.ScreenBounds.X + (int)imagePoint.X} Y: {_request.ScreenBounds.Y + (int)imagePoint.Y}";

            double scale = Math.Max(1, RenderScaling);
            Point pointer = new Point(imagePoint.X / scale, imagePoint.Y / scale);
            PositionPanelNearPointer(_magnifierPanel, pointer, 18);
            _magnifierPanel.InvalidateVisual();
        }
        else
        {
            _magnifierPanel.IsVisible = false;
        }

        UpdateSelectionInfo();
    }

    private unsafe void UpdateMagnifier(Point imagePoint)
    {
        if (_request == null)
        {
            return;
        }

        EnsureMagnifierBitmap();
        if (_magnifierBitmap == null)
        {
            return;
        }

        int count = _magnifierBitmap.PixelSize.Width;
        int radius = count / 2;
        int centerX = (int)Math.Round(imagePoint.X);
        int centerY = (int)Math.Round(imagePoint.Y);

        using ILockedFramebuffer framebuffer = _magnifierBitmap.Lock();
        byte* destination = (byte*)framebuffer.Address;

        for (int y = 0; y < count; y++)
        {
            byte* row = destination + y * framebuffer.RowBytes;
            int sourceY = Math.Clamp(centerY + y - radius, 0, _imageHeight - 1);

            for (int x = 0; x < count; x++)
            {
                int sourceX = Math.Clamp(centerX + x - radius, 0, _imageWidth - 1);
                SKColor color = _editorWorkspace.GetWorkspacePixel(sourceX, sourceY);
                int offset = x * 4;
                row[offset] = color.Blue;
                row[offset + 1] = color.Green;
                row[offset + 2] = color.Red;
                row[offset + 3] = color.Alpha;
            }
        }
    }

    private void EnsureMagnifierBitmap()
    {
        if (_request == null)
        {
            return;
        }

        int count = NormalizeMagnifierPixelCount(_request.CaptureOptions.MagnifierPixelCount);

        if (_magnifierBitmap?.PixelSize == new PixelSize(count, count))
        {
            return;
        }

        RecreateMagnifierBitmap(count);
    }

    private void RecreateMagnifierBitmap(int? requestedCount = null)
    {
        if (_request == null)
        {
            return;
        }

        int count = NormalizeMagnifierPixelCount(requestedCount ?? _request.CaptureOptions.MagnifierPixelCount);
        _request.CaptureOptions.MagnifierPixelCount = count;

        if (_magnifierBitmap?.PixelSize == new PixelSize(count, count))
        {
            _magnifierPixelGrid.PixelCount = count;
            return;
        }

        _magnifierBitmap?.Dispose();
        _magnifierBitmap = new WriteableBitmap(
            new PixelSize(count, count),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);
        _magnifierImage.Source = _magnifierBitmap;
        _magnifierPixelGrid.PixelCount = count;
    }

    private int NormalizeMagnifierPixelCount(int requestedCount)
    {
        double scale = double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;
        int sizeLimitedMaximum = (int)Math.Floor(MagnifierSize * scale / MinimumMagnifierPixelSize);
        int maximum = Math.Clamp(
            sizeLimitedMaximum,
            RegionCaptureOptions.MagnifierPixelCountMinimum,
            RegionCaptureOptions.MagnifierPixelCountMaximum);

        if ((maximum & 1) == 0)
        {
            maximum--;
        }

        int count = Math.Clamp(requestedCount, RegionCaptureOptions.MagnifierPixelCountMinimum, maximum);
        if ((count & 1) == 0)
        {
            count = count < maximum ? count + 1 : count - 1;
        }

        return count;
    }

    private void UpdateSelectionInfo()
    {
        if (_request?.CaptureOptions.ShowInfo != true)
        {
            _selectionInfoPanel.IsVisible = false;
            return;
        }

        Rect selection = _regionOverlay.SelectionRectangle;
        if (!RegionSelectionOverlay.IsValid(selection))
        {
            _selectionInfoPanel.IsVisible = false;
            return;
        }

        int width = Math.Max(1, (int)Math.Round(selection.Width));
        int height = Math.Max(1, (int)Math.Round(selection.Height));
        _selectionInfoText.Text = $"{width} × {height}";
        _selectionInfoPanel.IsVisible = true;

        double scale = Math.Max(1, RenderScaling);
        Point anchor = new Point(selection.Left / scale, selection.Bottom / scale);
        double x = Math.Clamp(anchor.X, 0, Math.Max(0, Bounds.Width - 100));
        double y = anchor.Y + 8;
        if (y + 36 > Bounds.Height)
        {
            y = Math.Max(0, selection.Top / scale - 36);
        }

        AvaloniaCanvas.SetLeft(_selectionInfoPanel, x);
        AvaloniaCanvas.SetTop(_selectionInfoPanel, y);
    }

    private void PositionPanelNearPointer(Control panel, Point pointer, double offset)
    {
        double width = panel.Bounds.Width > 0 ? panel.Bounds.Width : 170;
        double height = panel.Bounds.Height > 0 ? panel.Bounds.Height : 205;
        double x = pointer.X + offset;
        double y = pointer.Y + offset;

        if (x + width > Bounds.Width)
        {
            x = pointer.X - width - offset;
        }

        if (y + height > Bounds.Height)
        {
            y = pointer.Y - height - offset;
        }

        double targetX = Math.Clamp(x, 0, Math.Max(0, Bounds.Width - width));
        double targetY = Math.Clamp(y, 0, Math.Max(0, Bounds.Height - height));

        if (ReferenceEquals(panel, _magnifierPanel))
        {
            double scale = Math.Max(1, RenderScaling);
            targetX = Math.Round(targetX * scale) / scale;
            targetY = Math.Round(targetY * scale) / scale;
            _magnifierTransform.X = targetX;
            _magnifierTransform.Y = targetY;
            _magnifierPixelGrid.InvalidateVisual();
        }
        else
        {
            AvaloniaCanvas.SetLeft(panel, targetX);
            AvaloniaCanvas.SetTop(panel, targetY);
        }
    }

    private void RunCaptureAction(RegionCaptureAction action)
    {
        if (_request == null)
        {
            return;
        }

        switch (action)
        {
            case RegionCaptureAction.None:
                break;
            case RegionCaptureAction.CancelCapture:
                CancelCapture();
                break;
            case RegionCaptureAction.RemoveShapeCancelCapture:
                if (HasValidSelection() && _regionOverlay.SelectionRectangle.Contains(_lastPointerPoint))
                {
                    ClearSelection();
                }
                else
                {
                    CancelCapture();
                }
                break;
            case RegionCaptureAction.RemoveShape:
                if (HasValidSelection() && _regionOverlay.SelectionRectangle.Contains(_lastPointerPoint))
                {
                    ClearSelection();
                }
                break;
            case RegionCaptureAction.SwapToolType:
                if (_regionToolActive)
                {
                    ActivateAnnotationTool();
                }
                else
                {
                    ActivateRegionTool();
                }
                break;
            case RegionCaptureAction.CaptureFullscreen:
                Complete(new Rect(0, 0, _imageWidth, _imageHeight), includeWindowInfo: false);
                break;
            case RegionCaptureAction.CaptureActiveMonitor:
                CompleteActiveMonitor();
                break;
            case RegionCaptureAction.CaptureLastRegion:
                CompleteLastRegion();
                break;
        }
    }

    private void CompleteLastRegion()
    {
        if (_request == null || RegionCaptureIntegration.LastRegionRectangle.IsEmpty)
        {
            return;
        }

        DrawingRectangle screenRectangle = DrawingRectangle.Intersect(
            RegionCaptureIntegration.LastRegionRectangle,
            _request.ScreenBounds);
        if (screenRectangle.IsEmpty)
        {
            return;
        }

        Complete(new Rect(
            screenRectangle.X - _request.ScreenBounds.X,
            screenRectangle.Y - _request.ScreenBounds.Y,
            screenRectangle.Width,
            screenRectangle.Height), includeWindowInfo: false);
    }

    private void CompleteActiveMonitor()
    {
        if (_request == null)
        {
            return;
        }

        PixelPoint desktopPoint = new PixelPoint(
            _request.ScreenBounds.X + (int)Math.Round(_lastPointerPoint.X),
            _request.ScreenBounds.Y + (int)Math.Round(_lastPointerPoint.Y));
        Screen? screen = Screens.ScreenFromPoint(desktopPoint);
        if (screen == null)
        {
            return;
        }

        Rect relative = new Rect(
            screen.Bounds.X - _request.ScreenBounds.X,
            screen.Bounds.Y - _request.ScreenBounds.Y,
            screen.Bounds.Width,
            screen.Bounds.Height);
        Complete(RegionSelectionOverlay.Intersect(relative,
            new Rect(0, 0, _imageWidth, _imageHeight)),
            includeWindowInfo: false);
    }

    private void Complete(Rect selection, bool includeWindowInfo = true)
    {
        if (_closing || _request == null || !RegionSelectionOverlay.IsValid(selection))
        {
            return;
        }

        int left = Math.Clamp((int)Math.Floor(selection.Left), 0, _imageWidth - 1);
        int top = Math.Clamp((int)Math.Floor(selection.Top), 0, _imageHeight - 1);
        int right = Math.Clamp((int)Math.Ceiling(selection.Right), left + 1, _imageWidth);
        int bottom = Math.Clamp((int)Math.Ceiling(selection.Bottom), top + 1, _imageHeight);

        int width = right - left;
        int height = bottom - top;
        SKBitmap? output = _editorWorkspace.GetSnapshot(new Rect(left, top, width, height));
        if (output == null)
        {
            CancelCapture();
            return;
        }

        DrawingRectangle screenRectangle = new DrawingRectangle(
            _request.ScreenBounds.X + left,
            _request.ScreenBounds.Y + top,
            width,
            height);
        WindowInfo? windowInfo = includeWindowInfo ? FindTopLevelWindowInfo(screenRectangle) : null;

        _pendingResult = new AvaloniaRegionCaptureResult(
            output,
            screenRectangle,
            windowInfo,
            _viewModel?.IsDirty == true);
        RegionCaptureIntegration.LastRegionRectangle = screenRectangle;
        _closing = true;
        Close();
    }

    private WindowInfo? FindTopLevelWindowInfo(DrawingRectangle selectedRectangle)
    {
        if (_selectedCandidate is { IsWindow: true })
        {
            return _selectedCandidate.WindowInfo;
        }

        DrawingPoint point = new DrawingPoint(
            selectedRectangle.Left + selectedRectangle.Width / 2,
            selectedRectangle.Top + selectedRectangle.Height / 2);
        return _windows.FirstOrDefault(window => window.IsWindow && window.Rectangle.Contains(point))?.WindowInfo;
    }

    private void CancelCapture()
    {
        if (_closing)
        {
            return;
        }

        _closing = true;
        _pendingResult = null;
        Close();
    }

    private bool HasValidSelection()
    {
        if (_request == null)
        {
            return false;
        }

        Rect selection = _regionOverlay.SelectionRectangle;
        return selection.Width >= _request.CaptureOptions.MinimumSize &&
            selection.Height >= _request.CaptureOptions.MinimumSize;
    }

    private Point ClampPoint(Point point)
    {
        Size size = GetImageSize();
        return new Point(
            Math.Clamp(point.X, 0, size.Width),
            Math.Clamp(point.Y, 0, size.Height));
    }

    private Size GetImageSize()
    {
        return _request == null
            ? default
            : new Size(_imageWidth, _imageHeight);
    }

    private void ConfigureInitialPixelBounds()
    {
        if (_request == null)
        {
            return;
        }

        // Set Position exactly once, while the HWND still has its small initial size, so
        // Win32 selects the DPI of the monitor at the virtual desktop origin. Reassigning
        // Position after the window spans multiple monitors can make MonitorFromWindow
        // select a different monitor and silently change Avalonia's internal scale.
        Position = new PixelPoint(_request.ScreenBounds.X, _request.ScreenBounds.Y);
        ApplyPixelSize();
    }

    private void ApplyPixelSize()
    {
        if (_request == null)
        {
            return;
        }

        double scaling = double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;
        Width = _request.ScreenBounds.Width / scaling;
        Height = _request.ScreenBounds.Height / scaling;
    }

    private void UpdatePixelTransforms()
    {
        double scaling = double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;
        _regionTransform.LayoutTransform = new ScaleTransform(1 / scaling, 1 / scaling);
        if (_viewModel != null)
        {
            _viewModel.DpiScale = scaling;
            _viewModel.Zoom = 1;
        }
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(scaling);
        foreach (Border node in _regionResizeNodes.Values)
        {
            node.Cursor = CursorAssetLoader.GetOpenHandCursor(scaling);
        }
    }

    private double GetInitialScaling()
    {
        if (_request == null)
        {
            return 1;
        }

        PixelPoint topLeft = new PixelPoint(_request.ScreenBounds.X, _request.ScreenBounds.Y);
        return Screens.ScreenFromPoint(topLeft)?.Scaling ?? Screens.Primary?.Scaling ?? 1;
    }

    private static byte GetDimAlpha(RegionCaptureOptions options)
    {
        if (options.BackgroundDimStrength <= 0)
        {
            return 0;
        }

        return (byte)Math.Clamp((int)Math.Round(options.BackgroundDimStrength / 100d * 255), 0, 255);
    }

    private static double Distance(Point first, Point second)
    {
        double dx = second.X - first.X;
        double dy = second.Y - first.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private enum RegionInteraction
    {
        None,
        PendingHover,
        Creating,
        Moving,
        Resizing
    }
}
