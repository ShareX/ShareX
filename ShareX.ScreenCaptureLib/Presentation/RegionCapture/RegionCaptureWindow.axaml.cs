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
using DrawingColor = System.Drawing.Color;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

public partial class RegionCaptureWindow : Window
{
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
    private LayoutTransformControl _captureToolbarScale = null!;
    private const int ToolbarButtonSizeWheelStep = 4;
    private double _captureToolbarScaleFactor = 1;
    // See GetMonitorScaleFactor: corrects the toolbar for the scaling of the monitor it is on.
    private double _captureToolbarMonitorScale = 1;
    private double _appliedCaptureToolbarScale = 1;
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
    // Regions already added in multi region mode. The region being drawn or edited is the overlay selection.
    private readonly List<CaptureRegion> _regions = [];
    private SimpleWindowInfo? _hoverCandidate;
    private SimpleWindowInfo? _selectedCandidate;
    private RegionInteraction _interaction;
    private SelectionResizeNodeKind? _resizeNode;
    private Point _pressPoint;
    private Point _lastPointerPoint;
    private Point _lastCreationPoint;
    private CaptureRegion _interactionStartRegion;
    // The shape the region tool selects. Ellipse and freehand regions are hidden toolbar items by default.
    private RegionShape _regionShape = RegionShape.Rectangle;
    // Points of the freehand region being drawn; frozen into the selection when the button is released.
    private List<Point>? _freehandPoints;
    private readonly TranslateTransform _magnifierTransform = new();
    private readonly ScaleTransform _magnifierScale = new();
    private readonly ScaleTransform _selectionInfoScale = new();
    // Scaling of the monitor the custom cursors were last created for.
    private double _cursorScaling = 1;
    private double _captureToolbarCenterX = double.NaN;
    private double _captureToolbarTop = double.NaN;
    private Size _positionedCaptureToolbarSize;
    private Size _positionedCaptureToolbarClientSize;
    private bool _draggingCaptureToolbar;
    private Point _captureToolbarDragOffset;
    // Until the user interacts with the capture, the toolbar follows the pointer to whichever monitor it is on.
    private bool _captureToolbarFollowsPointer = true;
    private PixelRect _captureToolbarScreenBounds;
    private int _imageWidth;
    private int _imageHeight;
    private bool _regionToolActive = true;
    // Scrolling the magnifier away only hides it for this capture and never changes the saved setting.
    private bool _magnifierHiddenByWheel;
    private bool _keyboardInputEnabled;
    private bool _isMovingSelectionDuringCreation;
    private bool _wasControlHeldDuringCreation;
    private bool _suppressNextRightButtonReleaseAction;
    private bool _annotationMiddleButtonPressed;
    private bool _annotationRightButtonPressed;
    private bool _workspaceOwnsScreenshot;
    private bool _closing;
    private Exception? _startupException;
    private double _appliedPixelScaling;

    public RegionCaptureWindow()
    {
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        AvaloniaXamlLoader.Load(this);
#if !DEBUG
        Topmost = true;
#endif
        ResolveControls();
        AddHandler(PointerPressedEvent, OnCaptureHostPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(PointerMovedEvent, OnCaptureHostPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(PointerReleasedEvent, OnCaptureHostPointerReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(KeyDownEvent, OnCaptureHostKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    public RegionCaptureWindow(AvaloniaRegionCaptureRequest request) : this()
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
        _imageWidth = Math.Max(1, request.ScreenBounds.Width);
        _imageHeight = Math.Max(1, request.ScreenBounds.Height);
        if (request.RegionCaptureOptions.ActiveMonitorMode)
        {
            Helpers.LockCursorToWindow(this);
        }
        InitializeCaptureWorkspace();
        Opened += OnOpened;
    }

    public Task<AvaloniaRegionCaptureResult?> CaptureAsync()
    {
        if (_request == null)
        {
            throw new InvalidOperationException("A capture request is required.");
        }

        // The overlay spans every monitor and is sized in device pixels. Keep the scale it was created with, or Windows
        // switches it between the monitors' DPIs (mixed scaling) and selections miss the screenshot.
        RegionCaptureFixedScale.Apply(TryGetPlatformHandle()?.Handle ?? IntPtr.Zero);
        ConfigureInitialPixelBounds();

        try
        {
            PrepareWorkspace();
        }
        catch (Exception ex)
        {
            // Close so OnClosed releases the workspace and bitmaps and completes the task with this exception.
            _startupException = ex;
            Close();
            return _completionSource.Task;
        }

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
                _viewModel.HostToolbarItemRequested -= OnHostToolbarItemRequested;
            }

            if (_workspaceOwnsScreenshot)
            {
                _editorWorkspace.DisposeWorkspace();
            }
            else
            {
                _request?.Screenshot.Dispose();
            }

            _viewModel?.Dispose();
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
            e.KeyModifiers == KeyModifiers.None && _viewModel?.IsModalOpen != true &&
            (e.Key == Key.Tab || (e.Key == Key.NumPad0 && !e.Handled))) // NumPad0 matches ShareX 21; a toolbar hotkey on it wins.
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

        if (e.Key == Key.Enter && HasAnyRegion() && !e.Handled &&
            _viewModel?.IsModalOpen != true && e.Source is not TextBox &&
            FocusManager?.GetFocusedElement() is not TextBox)
        {
            e.Handled = true;
            CompleteSelection();
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
            CaptureHelpers.SetCursorPosition(CaptureHelpers.GetCursorPosition().Add(dx, dy));
            e.Handled = true;
            return;
        }

        if (_regionToolActive && e.Key == Key.Delete && HasAnyRegion())
        {
            if (HasValidSelection())
            {
                ClearSelection();
            }
            else
            {
                RemoveRegion(_regions.Count - 1);
            }

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
        _captureToolbarScale = this.FindControl<LayoutTransformControl>("CaptureToolbarScale")!;
        _annotationToolbar = this.FindControl<AnnotationToolbar>("CaptureAnnotationToolbar")!;
        _regionToolButton = this.FindControl<Button>("RegionToolButton")!;
        _magnifierPanel = this.FindControl<StackPanel>("MagnifierPanel")!;
        _magnifierPanel.RenderTransformOrigin = RelativePoint.TopLeft;
        _magnifierPanel.RenderTransform = new TransformGroup { Children = { _magnifierScale, _magnifierTransform } };
        _magnifierView = this.FindControl<Grid>("MagnifierView")!;
        _magnifierImage = this.FindControl<Image>("MagnifierImage")!;
        _magnifierPixelGrid = this.FindControl<MagnifierPixelGrid>("MagnifierPixelGrid")!;
        _pointerInfoPanel = this.FindControl<Border>("PointerInfoPanel")!;
        _pointerInfoText = this.FindControl<TextBlock>("PointerInfoText")!;
        _selectionInfoPanel = this.FindControl<Border>("SelectionInfoPanel")!;
        _selectionInfoPanel.RenderTransformOrigin = RelativePoint.TopLeft;
        _selectionInfoPanel.RenderTransform = _selectionInfoScale;
        _selectionInfoText = this.FindControl<TextBlock>("SelectionInfoText")!;
    }

    private void InitializeCaptureWorkspace()
    {
        if (_request == null)
        {
            return;
        }

        _viewModel = new MainViewModel(_request.ImageEditorOptions)
        {
            ShowFileMenu = false,
            ShowTaskButtons = false,
            ShowBottomToolbar = false,
            ShowToolbars = false,
            ShowStartScreen = false,
            UseContinueWorkflow = false,
            AllowAutoExpandCanvas = false
        };
        _viewModel.SetHostToolbarItems(_request.EnableAnnotations
            ? RegionCaptureToolbar.CreateItems(_request.RegionCaptureOptions.ToolbarItems)
            : []);
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _viewModel.ToolSelectionRequested += OnAnnotationToolSelected;
        _viewModel.HostToolbarItemRequested += OnHostToolbarItemRequested;

        _editorWorkspace.DataContext = _viewModel;
        _annotationToolbar.DataContext = _viewModel.ToolbarAdapter;
        _editorWorkspace.AttachHostAnnotationToolbar(_annotationToolbar);
        _annotationToolbar.IsVisible = _request.EnableAnnotations;
        _annotationToolbar.ShowToolOptions = false;

        if (_request.RegionCaptureOptions.CompactToolbar)
        {
            _annotationToolbar.IsCompact = true;
            _annotationToolbar.MainToolbarCornerRadius = new CornerRadius(3);
        }

        _regionInputSurface.Width = _imageWidth;
        _regionInputSurface.Height = _imageHeight;
        _regionOverlay.DimAlpha = GetDimAlpha(_request.RegionCaptureOptions);
        _regionOverlay.DimAllExceptSelection = _request.RegionCaptureOptions.DimAllExceptSelection;
        _regionOverlay.ShowCenterCrosshair = _request.RegionCaptureOptions.ShowCenterCrosshair;
        _regionOverlay.ShowCursorCrosshair = _request.RegionCaptureOptions.ShowScreenCrosshair;
        _cursorScaling = GetInitialScaling();
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(_cursorScaling);
        InitializeRegionResizeNodes();

        _magnifierView.IsVisible = IsMagnifierShown;
        ApplyMagnifierShape();
        _pointerInfoPanel.IsVisible = _request.RegionCaptureOptions.ShowInfo;
        _magnifierPanel.IsVisible = _magnifierView.IsVisible || _pointerInfoPanel.IsVisible;
        _regionToolButton.Classes.Set("active", true);
        _viewModel.SetHostToolbarToolsActive(false);
        Title = Localization.Strings.BaseRegionForm_InitializeComponent_Region_capture;
    }

    private void ApplyMagnifierShape()
    {
        if (_request == null)
        {
            return;
        }

        RegionCaptureOptions options = _request.RegionCaptureOptions;
        int magnifierSize = Math.Clamp(options.MagnifierSize,
            RegionCaptureOptions.MagnifierSizeMinimum,
            RegionCaptureOptions.MagnifierSizeMaximum);
        options.MagnifierSize = magnifierSize;
        double outerSize = magnifierSize + 2;

        Control circleOuter = this.FindControl<Control>("MagnifierCircleOuter")!;
        Control circleInner = this.FindControl<Control>("MagnifierCircleInner")!;
        Control squareOuter = this.FindControl<Control>("MagnifierSquareOuter")!;
        Control squareInner = this.FindControl<Control>("MagnifierSquareInner")!;
        bool useSquare = options.UseSquareMagnifier;
        circleOuter.IsVisible = !useSquare;
        circleInner.IsVisible = !useSquare;
        squareOuter.IsVisible = useSquare;
        squareInner.IsVisible = useSquare;

        _magnifierView.Width = outerSize;
        _magnifierView.Height = outerSize;
        circleOuter.Width = outerSize;
        circleOuter.Height = outerSize;
        squareOuter.Width = outerSize;
        squareOuter.Height = outerSize;
        circleInner.Width = magnifierSize;
        circleInner.Height = magnifierSize;
        squareInner.Width = magnifierSize;
        squareInner.Height = magnifierSize;

        Grid magnifierContent = this.FindControl<Grid>("MagnifierContent")!;
        magnifierContent.Width = magnifierSize;
        magnifierContent.Height = magnifierSize;
        _magnifierImage.Width = magnifierSize;
        _magnifierImage.Height = magnifierSize;
        magnifierContent.Clip = useSquare
            ? null
            : new EllipseGeometry(new Rect(0, 0, magnifierSize, magnifierSize));
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

    /// <summary>
    /// Runs the open path on a tiny workspace without showing the window: XAML, styles, templates, view model,
    /// toolbar, icons and one layout pass. Used once at idle time so the first real capture opens as fast as later ones.
    /// </summary>
    internal void WarmUpHidden()
    {
        PrepareWorkspace();
        ApplyPixelSize();

        // A window that was never shown is invisible, so layout skips it. Measure and render its content directly.
        if (Content is Control content)
        {
            Size size = new Size(Width, Height);
            content.Measure(size);
            content.Arrange(new Rect(size));
            using RenderTargetBitmap target = new RenderTargetBitmap(
                new PixelSize(Math.Max(1, (int)Math.Ceiling(size.Width)), Math.Max(1, (int)Math.Ceiling(size.Height))));
            target.Render(content);
        }
    }

    /// <summary>
    /// Loads the screenshot before the window is shown, so the first layout pass and the first
    /// rendered frame already contain it. OnOpened only corrects the scale if the monitor DPI differs.
    /// </summary>
    private void PrepareWorkspace()
    {
        if (_request == null || _viewModel == null)
        {
            return;
        }

        UpdatePixelTransforms();
        _editorWorkspace.ConfigureForFullscreenWorkspace();
        _editorWorkspace.LoadWorkspaceImage(_request.Screenshot, _request.ScreenshotIsOpaque);
        _workspaceOwnsScreenshot = true;

        if (_request.CursorBitmap != null)
        {
            _editorWorkspace.InsertWorkspaceImageAnnotation(
                _request.CursorBitmap,
                new Point(_request.CursorPosition.X, _request.CursorPosition.Y));
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
            Helpers.EnsureWindowPosition(this, _request.ScreenBounds.X, _request.ScreenBounds.Y);
            if (GetRenderScaling() != _appliedPixelScaling)
            {
                UpdatePixelTransforms();
            }

            if (_request.CursorBitmap != null)
            {
                ActivateRegionTool();
            }

            Activate();
            Focus();
            _regionInputSurface.Focus();

            DrawingPoint cursorPosition = CaptureHelpers.GetCursorPosition();
            _lastPointerPoint = ClampPoint(new Point(
                cursorPosition.X - _request.ScreenBounds.X,
                cursorPosition.Y - _request.ScreenBounds.Y));
            UpdateToolbarPosition(new PixelPoint(cursorPosition.X, cursorPosition.Y));
            UpdateHud(_lastPointerPoint);

            _ = LoadWindowRegionsAsync();

            int inputDelay = Math.Max(0, RegionCaptureOptions.InputDelay);
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
        if (_request == null || !_request.RegionCaptureOptions.DetectWindows)
        {
            return;
        }

        try
        {
            IntPtr ignoredHandle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            WindowsRectangleList windows = new WindowsRectangleList
            {
                IncludeChildWindows = _request.RegionCaptureOptions.DetectControls,
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
        ActivateRegionTool(RegionShape.Rectangle);
        e.Handled = true;
    }

    private void OnHostToolbarItemRequested(object? sender, string id)
    {
        if (RegionCaptureToolbar.GetRegionShape(id) is RegionShape shape)
        {
            ActivateRegionTool(shape);
        }
    }

    /// <summary>Switches to the region tool, selecting <paramref name="shape"/> or else the last used shape.</summary>
    private void ActivateRegionTool(RegionShape? shape = null)
    {
        if (shape.HasValue && shape.Value != _regionShape &&
            _interaction is RegionInteraction.Creating or RegionInteraction.PendingHover)
        {
            ClearSelection();
        }

        _regionShape = shape ?? _regionShape;
        _regionOverlay.HoverShape = _regionShape == RegionShape.Ellipse ? RegionShape.Ellipse : RegionShape.Rectangle;
        _regionToolActive = true;
        _regionInputSurface.IsVisible = true;
        _regionInputSurface.IsHitTestVisible = true;
        _regionOverlay.IsVisible = true;
        _regionOverlay.ShowCursorCrosshair = _request?.RegionCaptureOptions.ShowScreenCrosshair == true;
        SetRegionResizeNodesVisible(!IsQuickCapture && CanResizeSelection());
        _regionToolButton.Classes.Set("active", _regionShape == RegionShape.Rectangle);
        _viewModel?.SetHostToolbarToolsActive(false);
        _viewModel?.SetActiveHostToolbarItem(RegionCaptureToolbar.GetItemId(_regionShape));
        _annotationToolbar.ShowToolOptions = false;
        _editorWorkspace.CancelActiveInteractionOrSelection();
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(_cursorScaling);
        bool showMagnifier = IsMagnifierShown;
        bool showInfo = _request?.RegionCaptureOptions.ShowInfo == true;
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
        SetInteraction(RegionInteraction.None);
        bool showSelection = !IsQuickCapture && HasAnyRegion();
        _regionInputSurface.IsVisible = showSelection;
        _regionInputSurface.IsHitTestVisible = false;
        _regionOverlay.IsVisible = showSelection;
        _regionOverlay.ShowCursorCrosshair = false;
        SetRegionResizeNodesVisible(false);
        _regionOverlay.HoverRectangle = default;
        _hoverCandidate = null;
        _regionToolButton.Classes.Set("active", false);
        _viewModel?.SetActiveHostToolbarItem(null);
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
            RunCaptureAction(_request.RegionCaptureOptions.RegionCaptureActionMiddleClick);
            e.Handled = true;
            return;
        }

        if (properties.IsXButton1Pressed)
        {
            RunCaptureAction(_request.RegionCaptureOptions.RegionCaptureActionX1Click);
            e.Handled = true;
            return;
        }

        if (properties.IsXButton2Pressed)
        {
            RunCaptureAction(_request.RegionCaptureOptions.RegionCaptureActionX2Click);
            e.Handled = true;
            return;
        }

        if (properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed)
        {
            return;
        }

        if (e.ClickCount >= 2 && IsPointInAnyRegion(point))
        {
            CompleteSelection();
            e.Handled = true;
            return;
        }

        _pressPoint = point;
        _interactionStartRegion = _regionOverlay.Selection;
        _resizeNode = SelectionResizeNode.TryGetKind((e.Source as Control)?.Tag, out SelectionResizeNodeKind resizeNode)
            ? resizeNode
            : null;

        if (_resizeNode.HasValue)
        {
            SetInteraction(RegionInteraction.Resizing);
        }
        else if (HasValidSelection() && _regionOverlay.Selection.Contains(point))
        {
            SetInteraction(RegionInteraction.Moving);
        }
        else if (FindRegionAt(point) is int regionIndex and >= 0)
        {
            // Multi region mode: pressing an added region makes it the one being edited.
            CaptureRegion region = _regions[regionIndex];
            _regions.RemoveAt(regionIndex);
            AddSelectionToRegions();
            _regionOverlay.Regions = [.. _regions];
            SetSelection(region, null);
            _interactionStartRegion = region;
            SetInteraction(RegionInteraction.Moving);
        }
        else if (RegionSelectionOverlay.IsValid(_regionOverlay.HoverRectangle))
        {
            AddSelectionToRegions();
            SetSelection(new CaptureRegion(_regionOverlay.HoverShape, _regionOverlay.HoverRectangle), _hoverCandidate);
            _interactionStartRegion = _regionOverlay.Selection;
            SetInteraction(RegionInteraction.PendingHover);
        }
        else
        {
            AddSelectionToRegions();

            if (_regionShape == RegionShape.Freehand)
            {
                _freehandPoints = [point];
                SetSelection(new CaptureRegion(RegionShape.Freehand, default, _freehandPoints), null);
            }
            else
            {
                SetSelection(new CaptureRegion(_regionShape, default), null);
            }

            SetInteraction(RegionInteraction.Creating);
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
            _regionInputSurface.Cursor = CursorAssetLoader.GetClosedHandCursor(_cursorScaling);
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
                    SetInteraction(RegionInteraction.Creating);
                    _selectedCandidate = null;
                    SetSelection(new CaptureRegion(_regionOverlay.HoverShape, default), null);
                    UpdateSelectionDuringCreation(point, e.KeyModifiers);
                }
                break;
            case RegionInteraction.Creating when _freehandPoints != null:
                AddFreehandPoint(point);
                break;
            case RegionInteraction.Creating:
                UpdateSelectionDuringCreation(point, e.KeyModifiers);
                break;
            case RegionInteraction.Moving:
                MoveSelection(point.X - _pressPoint.X, point.Y - _pressPoint.Y, _interactionStartRegion);
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
        SetInteraction(RegionInteraction.None);
        _resizeNode = null;
        _regionInputSurface.Cursor = CursorAssetLoader.GetCrosshairCursor(_cursorScaling);
        ResetCreationModifiers();

        if (_freehandPoints != null)
        {
            SetSelection(_regionOverlay.Selection with { Points = _freehandPoints.ToArray() }, null);
            _freehandPoints = null;
        }

        if (!HasValidSelection())
        {
            if (RegionSelectionOverlay.IsValid(_regionOverlay.HoverRectangle))
            {
                SetSelection(new CaptureRegion(_regionOverlay.HoverShape, _regionOverlay.HoverRectangle), _hoverCandidate);
            }
            else
            {
                ClearSelection();
            }
        }

        if (HasValidSelection() && IsQuickCapture)
        {
            Complete([_regionOverlay.Selection]);
        }
        else
        {
            SetRegionResizeNodesVisible(CanResizeSelection());
            UpdateHover(_lastPointerPoint);
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
            RunCaptureAction(_request.RegionCaptureOptions.RegionCaptureActionRightClick);
        }

        e.Handled = true;
        return true;
    }

    private bool IsMagnifierShown => _request?.RegionCaptureOptions.ShowMagnifier == true && !_magnifierHiddenByWheel;

    private void OnRegionPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_request == null || !_regionToolActive)
        {
            return;
        }

        e.Handled = true;

        // Horizontal-only wheel events have no vertical direction.
        int direction = Math.Sign(e.Delta.Y);
        if (direction == 0)
        {
            return;
        }

        RegionCaptureOptions options = _request.RegionCaptureOptions;
        bool scrollUp = direction > 0;

        // A magnifier turned off in task settings stays off.
        if (!options.ShowMagnifier)
        {
            return;
        }

        if (_magnifierHiddenByWheel)
        {
            // A magnifier scrolled away comes back on scroll up, at the zoom and size it had before it was hidden.
            if (scrollUp)
            {
                _magnifierHiddenByWheel = false;
                RecreateMagnifierBitmap();
                UpdateHud(_lastPointerPoint);
            }

            return;
        }

        RegionCaptureMagnifierScrollAction action = options.MagnifierScrollAction;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            action = action == RegionCaptureMagnifierScrollAction.Zoom
                ? RegionCaptureMagnifierScrollAction.Resize
                : RegionCaptureMagnifierScrollAction.Zoom;
        }

        if (action == RegionCaptureMagnifierScrollAction.Resize)
        {
            ResizeMagnifier(scrollUp);
        }
        else
        {
            ZoomMagnifier(scrollUp);
        }

        UpdateHud(_lastPointerPoint);
    }

    // Keeps the magnifier the same size and changes how many pixels it shows.
    private void ZoomMagnifier(bool zoomIn)
    {
        RegionCaptureOptions options = _request!.RegionCaptureOptions;
        options.MagnifierPixelCount = NormalizeMagnifierPixelCount(
            NormalizeMagnifierPixelCount(options.MagnifierPixelCount) + (zoomIn ? -2 : 2));
        RecreateMagnifierBitmap();
    }

    // Keeps each magnified pixel the same size and grows or shrinks the magnifier around more or fewer pixels.
    // Shrinking past the smallest size hides the magnifier.
    private void ResizeMagnifier(bool grow)
    {
        RegionCaptureOptions options = _request!.RegionCaptureOptions;
        int currentSize = Math.Clamp(options.MagnifierSize,
            RegionCaptureOptions.MagnifierSizeMinimum,
            RegionCaptureOptions.MagnifierSizeMaximum);
        int currentCount = NormalizeMagnifierPixelCount(options.MagnifierPixelCount);
        int count = currentCount + (grow ? 2 : -2);
        int size = (int)Math.Round((double)currentSize / currentCount * count);

        if (size < RegionCaptureOptions.MagnifierSizeMinimum && currentSize == RegionCaptureOptions.MagnifierSizeMinimum)
        {
            _magnifierHiddenByWheel = true;
            return;
        }

        size = Math.Clamp(size, RegionCaptureOptions.MagnifierSizeMinimum, RegionCaptureOptions.MagnifierSizeMaximum);
        if (size == currentSize)
        {
            return;
        }

        // Past the pixel count limits the frame still resizes, and the pixels scale with it.
        options.MagnifierSize = size;
        options.MagnifierPixelCount = Math.Clamp(count,
            RegionCaptureOptions.MagnifierPixelCountMinimum,
            RegionCaptureOptions.MagnifierPixelCountMaximum);
        ApplyMagnifierShape();
        RecreateMagnifierBitmap();
        _magnifierPanel.UpdateLayout();
    }

    private void UpdateToolbarPosition(PixelPoint desktopPoint)
    {
        if (_request == null)
        {
            return;
        }

        double scale = GetToolbarScale();
        DrawingPoint saved = _request.RegionCaptureOptions.ToolbarPosition;
        if (!saved.IsEmpty)
        {
            PixelPoint savedPoint = new(saved.X, saved.Y);
            if (Screens.All.Any(screen => screen.Bounds.Contains(savedPoint)))
            {
                SetCaptureToolbarAnchor(
                    (saved.X - _request.ScreenBounds.X) / scale,
                    (saved.Y - _request.ScreenBounds.Y) / scale);
                return;
            }
        }

        Screen? screen = Screens.ScreenFromPoint(desktopPoint);
        if (screen == null)
        {
            return;
        }

        _captureToolbarScreenBounds = screen.Bounds;
        double monitorCenterX = screen.Bounds.X - _request.ScreenBounds.X + screen.Bounds.Width / 2d;
        double monitorTop = screen.Bounds.Y - _request.ScreenBounds.Y;
        SetCaptureToolbarAnchor(monitorCenterX / scale, monitorTop / scale);
    }

    private double GetToolbarScale()
    {
        return GetWindowScaling();
    }

    private double GetWindowScaling()
    {
        return double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;
    }

    /// <summary>Scaling of the monitor at the given desktop point, or of the window when the point is not on a monitor.</summary>
    private double GetMonitorScaling(PixelPoint desktopPoint)
    {
        double scaling = Screens.ScreenFromPoint(desktopPoint)?.Scaling ?? 0;
        return double.IsFinite(scaling) && scaling > 0 ? scaling : GetWindowScaling();
    }

    private double GetMonitorScaling(Point imagePoint)
    {
        return _request == null
            ? GetWindowScaling()
            : GetMonitorScaling(new PixelPoint(
                _request.ScreenBounds.X + (int)Math.Floor(imagePoint.X),
                _request.ScreenBounds.Y + (int)Math.Floor(imagePoint.Y)));
    }

    /// <summary>
    /// The overlay is one window across all monitors, so Windows gives it a single scaling, from the monitor at
    /// the desktop origin (see ConfigureInitialPixelBounds). The toolbar, magnifier and info labels are scaled by
    /// this factor so they match the scaling of the monitor they are shown on in mixed DPI setups.
    /// </summary>
    private double GetMonitorScaleFactor(PixelPoint desktopPoint)
    {
        return GetMonitorScaling(desktopPoint) / GetWindowScaling();
    }

    private double GetMonitorScaleFactor(Point imagePoint)
    {
        return GetMonitorScaling(imagePoint) / GetWindowScaling();
    }

    /// <summary>Recreates the custom cursors when the pointer moves to a monitor with different scaling, so they keep their size.</summary>
    private void UpdateCursorScaling(Point imagePoint)
    {
        double scaling = GetMonitorScaling(imagePoint);
        if (Math.Abs(scaling - _cursorScaling) < 0.001)
        {
            return;
        }

        _cursorScaling = scaling;
        _regionInputSurface.Cursor = _interaction is RegionInteraction.Moving or RegionInteraction.Resizing
            ? CursorAssetLoader.GetClosedHandCursor(scaling)
            : CursorAssetLoader.GetCrosshairCursor(scaling);
        foreach (Border node in _regionResizeNodes.Values)
        {
            node.Cursor = CursorAssetLoader.GetOpenHandCursor(scaling);
        }
    }

    /// <summary>The toolbar is anchored by its top centre so it stays put when the options row changes its width.</summary>
    private void SetCaptureToolbarAnchor(double centerX, double top)
    {
        _captureToolbarCenterX = centerX;
        _captureToolbarTop = top;
        _positionedCaptureToolbarSize = default;

        if (_request != null)
        {
            double scaling = GetWindowScaling();
            _captureToolbarMonitorScale = GetMonitorScaleFactor(new PixelPoint(
                _request.ScreenBounds.X + (int)Math.Floor(centerX * scaling),
                _request.ScreenBounds.Y + (int)Math.Floor(top * scaling)));
            ApplyCaptureToolbarScale();
        }

        PositionCaptureToolbar();
    }

    private void OnCaptureToolbarLayoutUpdated(object? sender, EventArgs e)
    {
        UpdateCaptureToolbarScale();
        PositionCaptureToolbar();
    }

    /// <summary>
    /// Scales the whole toolbar so its buttons are the custom button size tall. The button is measured
    /// inside the scale transform, so its height stays unscaled and the scale settles after one pass.
    /// </summary>
    private void UpdateCaptureToolbarScale()
    {
        if (_request == null || !_request.RegionCaptureOptions.UseCustomToolbarButtonSize)
        {
            return;
        }

        double buttonHeight = _regionToolButton.Bounds.Height;
        if (!double.IsFinite(buttonHeight) || buttonHeight <= 0)
        {
            return;
        }

        int buttonSize = Math.Clamp(_request.RegionCaptureOptions.ToolbarButtonSize,
            RegionCaptureOptions.ToolbarButtonSizeMinimum, RegionCaptureOptions.ToolbarButtonSizeMaximum);
        _captureToolbarScaleFactor = buttonSize / buttonHeight;
        ApplyCaptureToolbarScale();
    }

    /// <summary>
    /// Ctrl + mouse wheel over the toolbar makes it bigger or smaller. It changes the toolbar button size
    /// option from task settings, so the size is kept for the next capture.
    /// </summary>
    private void OnCaptureToolbarPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_request == null || !e.KeyModifiers.HasFlag(KeyModifiers.Control) || e.Delta.Y == 0)
        {
            return;
        }

        RegionCaptureOptions options = _request.RegionCaptureOptions;
        int currentSize = options.UseCustomToolbarButtonSize
            ? options.ToolbarButtonSize
            : (int)Math.Round(_regionToolButton.Bounds.Height);
        int newSize = Math.Clamp(currentSize + (e.Delta.Y > 0 ? ToolbarButtonSizeWheelStep : -ToolbarButtonSizeWheelStep),
            RegionCaptureOptions.ToolbarButtonSizeMinimum, RegionCaptureOptions.ToolbarButtonSizeMaximum);

        options.UseCustomToolbarButtonSize = true;
        options.ToolbarButtonSize = newSize;
        UpdateCaptureToolbarScale();
        e.Handled = true;
    }

    /// <summary>Applies the custom button size together with the correction for the toolbar's monitor.</summary>
    private void ApplyCaptureToolbarScale()
    {
        double scale = _captureToolbarScaleFactor * _captureToolbarMonitorScale;
        if (Math.Abs(scale - _appliedCaptureToolbarScale) < 0.001)
        {
            return;
        }

        _appliedCaptureToolbarScale = scale;
        _captureToolbarScale.LayoutTransform = new ScaleTransform(scale, scale);
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
        double toolbarHeight = _captureToolbar.Bounds.Height > 0
            ? _captureToolbar.Bounds.Height
            : _captureToolbar.DesiredSize.Height;
        // The window can still be resizing right after it opens: with mixed DPI monitors Windows can change its
        // scaling after the first size was set. So also place the toolbar again when the window size changes,
        // otherwise it stays clamped to the old, smaller size, between two monitors.
        Size clientSize = ClientSize;
        if (!double.IsFinite(toolbarWidth) || toolbarWidth <= 0 ||
            (Math.Abs(toolbarWidth - _positionedCaptureToolbarSize.Width) < 0.01 &&
             Math.Abs(toolbarHeight - _positionedCaptureToolbarSize.Height) < 0.01 &&
             clientSize == _positionedCaptureToolbarClientSize))
        {
            return;
        }

        _positionedCaptureToolbarSize = new Size(toolbarWidth, toolbarHeight);
        _positionedCaptureToolbarClientSize = clientSize;
        double maxLeft = Math.Max(0, ClientSize.Width - toolbarWidth);
        double maxTop = Math.Max(0, ClientSize.Height - toolbarHeight);
        AvaloniaCanvas.SetLeft(_captureToolbar, Math.Clamp(_captureToolbarCenterX - toolbarWidth / 2d, 0, maxLeft));
        AvaloniaCanvas.SetTop(_captureToolbar, Math.Clamp(_captureToolbarTop, 0, maxTop));
    }

    private void OnToolbarDragHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control handle || _request == null ||
            !e.GetCurrentPoint(handle).Properties.IsLeftButtonPressed)
        {
            return;
        }

        e.Handled = true;

        if (e.ClickCount >= 2)
        {
            _draggingCaptureToolbar = false;
            _request.RegionCaptureOptions.ToolbarPosition = DrawingPoint.Empty;
            DrawingPoint cursorPosition = CaptureHelpers.GetCursorPosition();
            UpdateToolbarPosition(new PixelPoint(cursorPosition.X, cursorPosition.Y));
            return;
        }

        // Start from where the toolbar is drawn, which may be clamped inside the window.
        double left = AvaloniaCanvas.GetLeft(_captureToolbar);
        double top = AvaloniaCanvas.GetTop(_captureToolbar);
        if (!double.IsFinite(left) || !double.IsFinite(top))
        {
            return;
        }

        _captureToolbarCenterX = left + _captureToolbar.Bounds.Width / 2d;
        _captureToolbarTop = top;

        Point pointer = e.GetPosition(this);
        _captureToolbarDragOffset = new Point(pointer.X - _captureToolbarCenterX, pointer.Y - _captureToolbarTop);
        _draggingCaptureToolbar = true;
        e.Pointer.Capture(handle);
    }

    private void OnToolbarDragHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_draggingCaptureToolbar)
        {
            return;
        }

        Point pointer = e.GetPosition(this);
        SetCaptureToolbarAnchor(pointer.X - _captureToolbarDragOffset.X, pointer.Y - _captureToolbarDragOffset.Y);
        e.Handled = true;
    }

    private void OnToolbarDragHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_draggingCaptureToolbar)
        {
            return;
        }

        e.Pointer.Capture(null);
        EndToolbarDrag();
        e.Handled = true;
    }

    private void OnToolbarDragHandlePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        EndToolbarDrag();
    }

    private void EndToolbarDrag()
    {
        if (!_draggingCaptureToolbar || _request == null)
        {
            return;
        }

        _draggingCaptureToolbar = false;

        // Save where the toolbar ended up after clamping, not where the pointer was released.
        double scale = GetToolbarScale();
        double centerX = AvaloniaCanvas.GetLeft(_captureToolbar) + _captureToolbar.Bounds.Width / 2d;
        double top = AvaloniaCanvas.GetTop(_captureToolbar);
        _captureToolbarCenterX = centerX;
        _captureToolbarTop = top;
        _request.RegionCaptureOptions.ToolbarPosition = new DrawingPoint(
            _request.ScreenBounds.X + (int)Math.Round(centerX * scale),
            _request.ScreenBounds.Y + (int)Math.Round(top * scale));
    }

    private void OnCaptureHostKeyDown(object? sender, KeyEventArgs e)
    {
        // Modifier keys alone are not an interaction, e.g. holding Shift while moving to another monitor.
        if (_keyboardInputEnabled && e.Key is not (Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or
            Key.LeftAlt or Key.RightAlt or Key.LWin or Key.RWin))
        {
            _captureToolbarFollowsPointer = false;
        }
    }

    private void FollowPointerWithCaptureToolbar(PointerEventArgs e)
    {
        // A position the user saved by dragging the toolbar always wins.
        if (!_captureToolbarFollowsPointer || _draggingCaptureToolbar || _request == null ||
            !_request.RegionCaptureOptions.ToolbarPosition.IsEmpty)
        {
            return;
        }

        double scale = GetToolbarScale();
        Point pointer = e.GetPosition(this);
        PixelPoint desktopPoint = new(
            _request.ScreenBounds.X + (int)Math.Round(pointer.X * scale),
            _request.ScreenBounds.Y + (int)Math.Round(pointer.Y * scale));
        if (!_captureToolbarScreenBounds.Contains(desktopPoint))
        {
            UpdateToolbarPosition(desktopPoint);
        }
    }

    private void OnCaptureHostPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _captureToolbarFollowsPointer = false;

        if (_regionToolActive || _closing || _request == null || !IsEditorWorkspaceSource(e.Source))
        {
            return;
        }

        PointerUpdateKind updateKind = e.GetCurrentPoint(_editorWorkspace).Properties.PointerUpdateKind;
        if (updateKind == PointerUpdateKind.MiddleButtonPressed)
        {
            if (_viewModel?.IsModalOpen == true || IsEditorNotificationSource(e.Source))
            {
                _annotationMiddleButtonPressed = false;
                return;
            }

            _annotationMiddleButtonPressed = true;
            e.Handled = true;
            RunAnnotationCaptureAction(
                _request.RegionCaptureOptions.RegionCaptureActionMiddleClick,
                e.GetPosition(_editorWorkspace));
            return;
        }

        if (updateKind == PointerUpdateKind.RightButtonPressed)
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

    private void OnCaptureHostPointerMoved(object? sender, PointerEventArgs e)
    {
        FollowPointerWithCaptureToolbar(e);

        if (!_annotationMiddleButtonPressed)
        {
            return;
        }

        if (!e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
        {
            _annotationMiddleButtonPressed = false;
            return;
        }

        e.Handled = true;
    }

    private void OnCaptureHostPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_annotationMiddleButtonPressed &&
            e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased)
        {
            _annotationMiddleButtonPressed = false;
            e.Handled = true;
            return;
        }

        if (!_annotationRightButtonPressed || _regionToolActive || _closing || _request == null)
        {
            return;
        }

        if (e.GetCurrentPoint(_editorWorkspace).Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
        {
            return;
        }

        _annotationRightButtonPressed = false;
        RunAnnotationCaptureAction(
            _request.RegionCaptureOptions.RegionCaptureActionRightClick,
            e.GetPosition(_editorWorkspace));
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

    private void RunAnnotationCaptureAction(RegionCaptureAction action, Point workspacePoint)
    {
        if (_request == null)
        {
            return;
        }

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
        UpdateHoverRectangle(imagePoint);

        // The size label also covers the hovered window, so refresh it whenever the hover changes.
        UpdateSelectionInfo();
    }

    private void UpdateHoverRectangle(Point imagePoint)
    {
        bool hoverBlocked = IsMultiRegionMode ? IsPointInAnyRegion(imagePoint) : HasValidSelection();
        if (_request == null || hoverBlocked || _interaction != RegionInteraction.None ||
            _regionShape == RegionShape.Freehand)
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

    /// <summary>Sets the bounds of the rectangle or ellipse selection.</summary>
    private void SetSelection(Rect rectangle, SimpleWindowInfo? candidate)
    {
        SetSelection(_regionOverlay.Selection with { Bounds = rectangle }, candidate);
    }

    private void SetSelection(CaptureRegion region, SimpleWindowInfo? candidate)
    {
        _regionOverlay.Selection = region with
        {
            Bounds = RegionSelectionOverlay.Intersect(
                region.Bounds,
                new Rect(0, 0, GetImageSize().Width, GetImageSize().Height))
        };
        _selectedCandidate = candidate;
        if (_regionResizeNodeCanvas.IsVisible)
        {
            UpdateRegionResizeNodePositions();
        }
        UpdateSelectionInfo();
    }

    private void SetInteraction(RegionInteraction interaction)
    {
        _interaction = interaction;
        _regionOverlay.IsSelectionPending = interaction == RegionInteraction.PendingHover;
        _regionOverlay.ShowHoverWithSelection = interaction == RegionInteraction.None && IsMultiRegionMode;
    }

    private void ClearSelection()
    {
        SetInteraction(RegionInteraction.None);
        ResetCreationModifiers();
        _selectedCandidate = null;
        _freehandPoints = null;
        _regionOverlay.Selection = default;
        SetRegionResizeNodesVisible(false);
        _selectionInfoPanel.IsVisible = false;
        UpdateHover(_lastPointerPoint);
    }

    private void MoveSelection(double dx, double dy, CaptureRegion source)
    {
        Size bounds = GetImageSize();
        Rect rectangle = source.Bounds;
        double x = Math.Clamp(rectangle.X + dx, 0, Math.Max(0, bounds.Width - rectangle.Width));
        double y = Math.Clamp(rectangle.Y + dy, 0, Math.Max(0, bounds.Height - rectangle.Height));
        SetSelection(source.Offset(x - rectangle.X, y - rectangle.Y), null);
    }

    private void AddFreehandPoint(Point point)
    {
        if (_freehandPoints == null || (_freehandPoints.Count > 0 && _freehandPoints[^1] == point))
        {
            return;
        }

        _freehandPoints.Add(point);
        SetSelection(new CaptureRegion(RegionShape.Freehand, CaptureRegion.GetBounds(_freehandPoints), _freehandPoints), null);
    }

    private void UpdateSelectionDuringCreation(Point point, KeyModifiers modifiers)
    {
        if (_freehandPoints != null)
        {
            return;
        }

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
        MoveSelection(point.X - _lastCreationPoint.X, point.Y - _lastCreationPoint.Y, _regionOverlay.Selection);
        Rect after = _regionOverlay.SelectionRectangle;
        _pressPoint = ClampPoint(new Point(
            _pressPoint.X + after.X - before.X,
            _pressPoint.Y + after.Y - before.Y));
    }

    private static Rect CreateSelectionRectangle(Point first, Point second, Size bounds, bool constrainToSquare)
    {
        if (!constrainToSquare)
        {
            return RegionSelectionOverlay.CreatePixelRectangle(first, second, bounds);
        }

        double deltaX = second.X - first.X;
        double deltaY = second.Y - first.Y;
        double side = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
        double availableWidth = deltaX < 0 ? first.X : bounds.Width - 1 - first.X;
        double availableHeight = deltaY < 0 ? first.Y : bounds.Height - 1 - first.Y;
        side = Math.Max(0, Math.Min(side, Math.Min(availableWidth, availableHeight)));

        Point constrained = new(
            first.X + (deltaX < 0 ? -side : side),
            first.Y + (deltaY < 0 ? -side : side));
        return RegionSelectionOverlay.CreatePixelRectangle(first, constrained, bounds);
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
        UpdateCursorScaling(imagePoint);

        bool showMagnifier = IsMagnifierShown;
        bool showInfo = _request.RegionCaptureOptions.ShowInfo;

        if (showMagnifier || showInfo)
        {
            if (showMagnifier)
            {
                UpdateMagnifier(imagePoint);
            }

            _magnifierView.IsVisible = showMagnifier;
            _pointerInfoPanel.IsVisible = showInfo;
            _magnifierPanel.IsVisible = true;
            _pointerInfoText.Text = GetPointerInfoText(imagePoint);

            double scale = Math.Max(1, RenderScaling);
            Point pointer = new Point(imagePoint.X / scale, imagePoint.Y / scale);
            double monitorScale = GetMonitorScaleFactor(imagePoint);
            _magnifierScale.ScaleX = monitorScale;
            _magnifierScale.ScaleY = monitorScale;
            PositionPanelNearPointer(_magnifierPanel, pointer, 18, GetMonitorBounds(imagePoint), monitorScale);
            _magnifierPanel.InvalidateVisual();
        }
        else
        {
            _magnifierPanel.IsVisible = false;
        }

        UpdateSelectionInfo();
    }

    private string GetPointerInfoText(Point imagePoint)
    {
        if (_request == null)
        {
            return string.Empty;
        }

        int imageX = Math.Clamp((int)Math.Round(imagePoint.X), 0, _imageWidth - 1);
        int imageY = Math.Clamp((int)Math.Round(imagePoint.Y), 0, _imageHeight - 1);
        DrawingPoint screenPosition = new(
            _request.ScreenBounds.X + imageX,
            _request.ScreenBounds.Y + imageY);

        RegionCaptureOptions options = _request.RegionCaptureOptions;
        if (options.UseCustomInfoText)
        {
            SKColor pixel = _editorWorkspace.GetWorkspacePixel(imageX, imageY);
            DrawingColor color = DrawingColor.FromArgb(pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue);

            if (!string.IsNullOrEmpty(options.CustomInfoText))
            {
                return CodeMenuEntryPixelInfo.Parse(options.CustomInfoText, color, screenPosition);
            }

            return $"RGB: {color.R}, {color.G}, {color.B}{Environment.NewLine}" +
                $"Hex: {ColorHelpers.ColorToHex(color)}{Environment.NewLine}" +
                $"X: {screenPosition.X} Y: {screenPosition.Y}";
        }

        return $"X: {screenPosition.X} Y: {screenPosition.Y}";
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

        int count = NormalizeMagnifierPixelCount(_request.RegionCaptureOptions.MagnifierPixelCount);

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

        int count = NormalizeMagnifierPixelCount(requestedCount ?? _request.RegionCaptureOptions.MagnifierPixelCount);
        _request.RegionCaptureOptions.MagnifierPixelCount = count;

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
        int magnifierSize = Math.Clamp(_request?.RegionCaptureOptions.MagnifierSize ?? RegionCaptureOptions.MagnifierSizeMinimum,
            RegionCaptureOptions.MagnifierSizeMinimum,
            RegionCaptureOptions.MagnifierSizeMaximum);
        int sizeLimitedMaximum = (int)Math.Floor(
            magnifierSize * scale / RegionCaptureOptions.MagnifierPixelSizeMinimum);
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
        if (_request?.RegionCaptureOptions.ShowInfo != true)
        {
            _selectionInfoPanel.IsVisible = false;
            return;
        }

        // Show the size of the selection, or of the hovered window when nothing is selected yet.
        Rect selection = _regionOverlay.ActiveRectangle;
        if (!RegionSelectionOverlay.IsValid(selection))
        {
            _selectionInfoPanel.IsVisible = false;
            return;
        }

        int width = Math.Max(1, (int)Math.Round(selection.Width));
        int height = Math.Max(1, (int)Math.Round(selection.Height));
        // Same format as ShareX 21. X and Y are screen coordinates, matching the cursor info label.
        int screenX = _request.ScreenBounds.X + (int)Math.Round(selection.X);
        int screenY = _request.ScreenBounds.Y + (int)Math.Round(selection.Y);
        _selectionInfoText.Text = $"X: {screenX} Y: {screenY} W: {width} H: {height}";
        _selectionInfoPanel.IsVisible = true;

        double scale = Math.Max(1, RenderScaling);
        double baseWidth = _selectionInfoPanel.Bounds.Width > 0 ? _selectionInfoPanel.Bounds.Width : 100;
        double baseHeight = _selectionInfoPanel.Bounds.Height > 0 ? _selectionInfoPanel.Bounds.Height : 36;
        double left = selection.Left / scale;

        // Prefer below the selection, on the monitor containing its bottom-left corner.
        Point anchor = new Point(selection.Left, Math.Max(selection.Top, selection.Bottom - 1));
        Rect area = GetMonitorBounds(anchor);
        double panelScale = GetMonitorScaleFactor(anchor);
        double panelWidth = baseWidth * panelScale;
        double panelHeight = baseHeight * panelScale;
        double y = selection.Bottom / scale + 8 * panelScale;
        if (y + panelHeight > area.Bottom)
        {
            // Otherwise place it above the selection, on the monitor containing its top-left corner.
            anchor = new Point(selection.Left, selection.Top);
            area = GetMonitorBounds(anchor);
            panelScale = GetMonitorScaleFactor(anchor);
            panelWidth = baseWidth * panelScale;
            panelHeight = baseHeight * panelScale;
            y = selection.Top / scale - panelHeight;
        }

        double x = Math.Clamp(left, area.Left, Math.Max(area.Left, area.Right - panelWidth));
        y = Math.Clamp(y, area.Top, Math.Max(area.Top, area.Bottom - panelHeight));

        _selectionInfoScale.ScaleX = panelScale;
        _selectionInfoScale.ScaleY = panelScale;
        AvaloniaCanvas.SetLeft(_selectionInfoPanel, x);
        AvaloniaCanvas.SetTop(_selectionInfoPanel, y);
    }

    /// <summary>
    /// Returns the bounds, in window coordinates (DIPs), of the monitor containing the given image point.
    /// Falls back to the whole window when the point is not on any monitor.
    /// </summary>
    private Rect GetMonitorBounds(Point imagePoint)
    {
        Rect windowBounds = new Rect(Bounds.Size);
        if (_request == null)
        {
            return windowBounds;
        }

        PixelPoint desktopPoint = new PixelPoint(
            _request.ScreenBounds.X + (int)Math.Floor(imagePoint.X),
            _request.ScreenBounds.Y + (int)Math.Floor(imagePoint.Y));
        Screen? screen = Screens.ScreenFromPoint(desktopPoint);
        if (screen == null)
        {
            return windowBounds;
        }

        double scale = Math.Max(1, RenderScaling);
        Rect monitorBounds = new Rect(
            (screen.Bounds.X - _request.ScreenBounds.X) / scale,
            (screen.Bounds.Y - _request.ScreenBounds.Y) / scale,
            screen.Bounds.Width / scale,
            screen.Bounds.Height / scale).Intersect(windowBounds);
        return monitorBounds.Width > 0 && monitorBounds.Height > 0 ? monitorBounds : windowBounds;
    }

    /// <param name="panelScale">Scale applied to the panel by its render transform, see GetMonitorScaleFactor.</param>
    private void PositionPanelNearPointer(Control panel, Point pointer, double offset, Rect area, double panelScale = 1)
    {
        bool isMagnifierPanel = ReferenceEquals(panel, _magnifierPanel);
        double fallbackWidth = isMagnifierPanel ? _magnifierView.Width : 170;
        double fallbackHeight = isMagnifierPanel ? _magnifierView.Height + 53 : 205;
        double width = (panel.Bounds.Width > 0 ? panel.Bounds.Width : fallbackWidth) * panelScale;
        double height = (panel.Bounds.Height > 0 ? panel.Bounds.Height : fallbackHeight) * panelScale;
        offset *= panelScale;
        double x = pointer.X + offset;
        double y = pointer.Y + offset;

        if (x + width > area.Right)
        {
            x = pointer.X - width - offset;
        }

        if (y + height > area.Bottom)
        {
            y = pointer.Y - height - offset;
        }

        double targetX = Math.Clamp(x, area.Left, Math.Max(area.Left, area.Right - width));
        double targetY = Math.Clamp(y, area.Top, Math.Max(area.Top, area.Bottom - height));

        if (isMagnifierPanel)
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
                if (!TryRemoveRegionAt(_lastPointerPoint))
                {
                    CancelCapture();
                }
                break;
            case RegionCaptureAction.RemoveShape:
                TryRemoveRegionAt(_lastPointerPoint);
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

        IReadOnlyList<DrawingRectangle> lastRegions = RegionCaptureIntegration.LastRegionRectangles.Count > 0
            ? RegionCaptureIntegration.LastRegionRectangles
            : [RegionCaptureIntegration.LastRegionRectangle];
        List<Rect> regions = [];

        foreach (DrawingRectangle lastRegion in lastRegions)
        {
            DrawingRectangle screenRectangle = DrawingRectangle.Intersect(lastRegion, _request.ScreenBounds);
            if (!screenRectangle.IsEmpty)
            {
                regions.Add(new Rect(
                    screenRectangle.X - _request.ScreenBounds.X,
                    screenRectangle.Y - _request.ScreenBounds.Y,
                    screenRectangle.Width,
                    screenRectangle.Height));
            }
        }

        Complete(regions, includeWindowInfo: false);
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

    private void CompleteSelection()
    {
        Complete(GetSelectedRegions());
    }

    private void Complete(Rect selection, bool includeWindowInfo = true)
    {
        Complete([CaptureRegion.FromRectangle(selection)], includeWindowInfo);
    }

    private void Complete(IReadOnlyList<Rect> selections, bool includeWindowInfo)
    {
        Complete(selections.Select(CaptureRegion.FromRectangle).ToList(), includeWindowInfo);
    }

    /// <summary>
    /// Captures the bounds of <paramref name="selections"/>. With more than one region, or an ellipse or freehand
    /// region, pixels outside every region are transparent, matching the classic region capture.
    /// </summary>
    private void Complete(IReadOnlyList<CaptureRegion> selections, bool includeWindowInfo = true)
    {
        if (_closing || _request == null)
        {
            return;
        }

        List<DrawingRectangle> regions = [];
        List<CaptureRegion> shapes = [];

        foreach (CaptureRegion region in selections)
        {
            Rect selection = region.Bounds;
            if (!RegionSelectionOverlay.IsValid(selection))
            {
                continue;
            }

            int left = Math.Clamp((int)Math.Floor(selection.Left), 0, _imageWidth - 1);
            int top = Math.Clamp((int)Math.Floor(selection.Top), 0, _imageHeight - 1);
            int right = Math.Clamp((int)Math.Ceiling(selection.Right), left + 1, _imageWidth);
            int bottom = Math.Clamp((int)Math.Ceiling(selection.Bottom), top + 1, _imageHeight);
            regions.Add(new DrawingRectangle(left, top, right - left, bottom - top));
            shapes.Add(region);
        }

        if (regions.Count == 0)
        {
            return;
        }

        DrawingRectangle bounds = regions.Aggregate(DrawingRectangle.Union);
        SKBitmap? output = _editorWorkspace.GetSnapshot(new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height));
        if (output == null)
        {
            CancelCapture();
            return;
        }

        if (shapes.Any(shape => !shape.IsRectangle))
        {
            List<SKPath> paths = shapes.Select(shape => shape.CreatePath(-bounds.X, -bounds.Y)).ToList();
            try
            {
                SKBitmap masked = RegionCaptureTasks.ApplyRegionMask(output, paths);
                output.Dispose();
                output = masked;
            }
            finally
            {
                paths.ForEach(path => path.Dispose());
            }
        }
        else if (regions.Count > 1)
        {
            List<DrawingRectangle> localRegions = regions
                .Select(region => new DrawingRectangle(region.X - bounds.X, region.Y - bounds.Y, region.Width, region.Height))
                .ToList();
            SKBitmap masked = RegionCaptureTasks.ApplyRegionMask(output, localRegions);
            output.Dispose();
            output = masked;
        }

        DrawingRectangle screenRectangle = ToScreenRectangle(bounds);
        WindowInfo? windowInfo = includeWindowInfo ? FindTopLevelWindowInfo(screenRectangle) : null;

        _pendingResult = new AvaloniaRegionCaptureResult(
            output,
            screenRectangle,
            windowInfo,
            _viewModel?.IsDirty == true);
        RegionCaptureIntegration.SetLastRegion(screenRectangle, regions.Select(ToScreenRectangle).ToArray());
        _closing = true;
        Close();
    }

    private DrawingRectangle ToScreenRectangle(DrawingRectangle imageRectangle)
    {
        return new DrawingRectangle(
            _request!.ScreenBounds.X + imageRectangle.X,
            _request.ScreenBounds.Y + imageRectangle.Y,
            imageRectangle.Width,
            imageRectangle.Height);
    }

    private WindowInfo? FindTopLevelWindowInfo(DrawingRectangle selectedRectangle)
    {
        if (_selectedCandidate is { IsWindow: true })
        {
            return _selectedCandidate.WindowInfo;
        }

        // Prefer the window under the cursor (the clicked window), because the selection center can be covered by another window.
        DrawingPoint point = new DrawingPoint(
            _request!.ScreenBounds.X + (int)_lastPointerPoint.X,
            _request.ScreenBounds.Y + (int)_lastPointerPoint.Y);

        if (!selectedRectangle.Contains(point))
        {
            point = new DrawingPoint(
                selectedRectangle.Left + selectedRectangle.Width / 2,
                selectedRectangle.Top + selectedRectangle.Height / 2);
        }

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

        CaptureRegion selection = _regionOverlay.Selection;
        return selection.Bounds.Width >= RegionCaptureOptions.MinimumSize &&
            selection.Bounds.Height >= RegionCaptureOptions.MinimumSize &&
            (selection.Shape != RegionShape.Freehand || selection.Points is { Count: >= 3 });
    }

    // A freehand region can be moved but has no resize nodes.
    private bool CanResizeSelection() => HasValidSelection() && _regionOverlay.Selection.Shape != RegionShape.Freehand;

    private bool IsMultiRegionMode => _request is { AllowMultipleRegions: true, RegionCaptureOptions.MultiRegionMode: true };

    private bool IsQuickCapture => _request?.RegionCaptureOptions.QuickCapture == true && !IsMultiRegionMode;

    private bool HasAnyRegion() => HasValidSelection() || _regions.Count > 0;

    private List<CaptureRegion> GetSelectedRegions()
    {
        List<CaptureRegion> regions = [.. _regions];
        if (HasValidSelection())
        {
            regions.Add(_regionOverlay.Selection);
        }

        return regions;
    }

    /// <summary>Returns the index of the topmost added region containing the point, or -1.</summary>
    private int FindRegionAt(Point point)
    {
        for (int i = _regions.Count - 1; i >= 0; i--)
        {
            if (_regions[i].Contains(point))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsPointInAnyRegion(Point point)
    {
        return (HasValidSelection() && _regionOverlay.Selection.Contains(point)) || FindRegionAt(point) >= 0;
    }

    /// <summary>In multi region mode, keeps the current selection as an added region before another one starts.</summary>
    private void AddSelectionToRegions()
    {
        if (IsMultiRegionMode && HasValidSelection())
        {
            _regions.Add(_regionOverlay.Selection);
            _regionOverlay.Regions = [.. _regions];
        }
    }

    private void RemoveRegion(int index)
    {
        if (index < 0 || index >= _regions.Count)
        {
            return;
        }

        _regions.RemoveAt(index);
        _regionOverlay.Regions = [.. _regions];
        UpdateHover(_lastPointerPoint);
    }

    private bool TryRemoveRegionAt(Point point)
    {
        if (HasValidSelection() && _regionOverlay.Selection.Contains(point))
        {
            ClearSelection();
            return true;
        }

        int index = FindRegionAt(point);
        if (index >= 0)
        {
            RemoveRegion(index);
            return true;
        }

        return false;
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

    private double GetRenderScaling() => double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;

    private void UpdatePixelTransforms()
    {
        double scaling = GetRenderScaling();
        _appliedPixelScaling = scaling;
        _regionTransform.LayoutTransform = new ScaleTransform(1 / scaling, 1 / scaling);
        if (_viewModel != null)
        {
            _viewModel.DpiScale = scaling;
            _viewModel.Zoom = 1;
        }
        // UpdateHud switches the cursors to the scaling of the monitor under the pointer.
        _cursorScaling = scaling;
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
