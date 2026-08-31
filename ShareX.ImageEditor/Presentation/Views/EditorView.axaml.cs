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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Core.Editor;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Controllers;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Emoji;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;
using System.ComponentModel;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        public static readonly StyledProperty<bool> UseBuiltInToolbarsProperty =
            AvaloniaProperty.Register<EditorView, bool>(nameof(UseBuiltInToolbars));

        private static readonly Cursor ArrowCursor = new(StandardCursorType.Arrow);
        internal const double OverlayCanvasBleed = 24;

        private readonly EditorZoomController _zoomController;
        private readonly EditorSelectionController _selectionController;
        private readonly EditorInputController _inputController;

        internal EditorCore EditorCore => _editorCore;

        // SIP0018: Hybrid Rendering
        private SKCanvasControl? _canvasControl;
        private readonly EditorCore _editorCore;

        // Sync flags to prevent loop between VM.PreviewImage <-> Core.SourceImage
        private bool _isSyncingFromVM;
        private bool _isSyncingToVM;
        private bool _skipNextCoreImageChanged;
        private bool _isWorkspaceHostMode;
        private bool _workspaceDisposed;
        private bool _suppressNextHistoryDirtyMark;
        private bool _pendingZoomToFitOnOpen;
        private int _pendingZoomToFitRetryCount;
        private int _pendingAutoCopyImageVersion;
        private int _renderCorePending;
        private bool _overlayCanvasLayoutUpdatePending;
        private Rect? _lastOverlayCanvasRect;
        private double _lastOverlayCanvasZoom = -1;
        private double _lastRenderScaling = 1.0;
        private EffectBrowserPanel? _effectBrowserPanel;
        private ImageEditorOptions? _effectBrowserPanelOptions;
        private Cursor? _interactionCursorOverride;
        private CursorAssetLoader.CustomCursorKind? _interactionCursorAsset;
        private ContentControl _builtInToolbarsHost = null!;
        private EditorBuiltInToolbars? _builtInToolbars;

        // Window-level key handler reference (so shortcuts work regardless of focus)
        private Window? _parentWindow;

        // SIP-CLIPBOARD: Internal clipboard for shape deep-cloning
        private static Annotation? _clipboardAnnotation;

        public EditorView()
        {
            InitializeComponent();
            _builtInToolbarsHost = this.FindControl<ContentControl>("BuiltInToolbarsHost")!;

            _editorCore = new EditorCore();

            _zoomController = new EditorZoomController(this);
            _selectionController = new EditorSelectionController(this);
            _inputController = new EditorInputController(this, _selectionController, _zoomController);
            InitializeEasterEggs();

            // Subscribe to selection controller events
            _selectionController.RequestUpdateEffect += OnRequestUpdateEffect;
            _selectionController.SelectionChanged += OnSelectionChanged;
            LayoutUpdated += OnLayoutUpdated;

            // SIP0018: Subscribe to Core events
            _editorCore.InvalidateRequested += RequestRenderCore;
            _editorCore.ImageChanged += () =>
            {
                // Capture the one-shot skip synchronously so it applies to the event
                // raised by the VM->Core sync, not the next unrelated crop/cut/undo event.
                bool skipVmSync = _skipNextCoreImageChanged;
                _skipNextCoreImageChanged = false;

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    if (_canvasControl != null)
                    {
                        _canvasControl.Initialize((int)_editorCore.CanvasSize.Width, (int)_editorCore.CanvasSize.Height);
                        RenderCore();
                        if (DataContext is MainViewModel vm)
                        {
                            UpdateViewModelHistoryState(vm);
                            UpdateViewModelMetadata(vm);
                            vm.SyncImageDimensions(_editorCore.CanvasSize.Width, _editorCore.CanvasSize.Height);

                            // Sync Core image back to VM if change originated from Core (Undo/Redo, Core Crop)
                            if (!_isWorkspaceHostMode && !_isSyncingFromVM && !_isSyncingToVM && _editorCore.SourceImage != null)
                            {
                                if (skipVmSync)
                                {
                                    return;
                                }

                                try
                                {
                                    _isSyncingToVM = true;
                                    vm.UpdatePreviewImageOnly(_editorCore.SourceImage, syncSourceState: true);

                                    // Core-driven destructive image changes resize the backing bitmap
                                    // before the VM size bindings have updated the layout container.
                                    // Queue one more redraw after the render pass so the raster layer
                                    // is repainted against the settled post-resize bounds.
                                    Avalonia.Threading.Dispatcher.UIThread.Post(RenderCore, DispatcherPriority.Render);
                                }
                                finally
                                {
                                    _isSyncingToVM = false;
                                }
                            }
                        }
                    }
                });
            };
            _editorCore.AnnotationsRestored += () => Avalonia.Threading.Dispatcher.UIThread.Post(OnAnnotationsRestored);
            _editorCore.AnnotationOrderChanged += () => Avalonia.Threading.Dispatcher.UIThread.Post(OnAnnotationOrderChanged);
            _editorCore.HistoryChanged += () =>
            {
                bool suppressDirtyMark = _suppressNextHistoryDirtyMark;
                _suppressNextHistoryDirtyMark = false;

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    if (DataContext is MainViewModel vm)
                    {
                        UpdateViewModelHistoryState(vm);
                        vm.RecalculateNumberCounter(_editorCore.Annotations);

                        if (!suppressDirtyMark)
                        {
                            // Mark as dirty when history changes due to a user edit.
                            vm.IsDirty = true;
                        }

                        QueueAutoCopyImageToClipboard(vm);
                    }
                });
            };

            // Capture wheel events in tunneling phase so ScrollViewer doesn't scroll when using Ctrl+wheel zoom.
            AddHandler(PointerWheelChangedEvent, OnPreviewPointerWheelChanged, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);

            // Enable drag-and-drop for image files
            DragDrop.SetAllowDrop(this, true);
            AddHandler(DragDrop.DropEvent, OnDrop);
            AddHandler(DragDrop.DragOverEvent, OnDragOver);

        }

        public bool UseBuiltInToolbars
        {
            get => GetValue(UseBuiltInToolbarsProperty);
            set => SetValue(UseBuiltInToolbarsProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == UseBuiltInToolbarsProperty && _builtInToolbarsHost != null)
            {
                UpdateBuiltInToolbars();
            }
        }

        private void UpdateBuiltInToolbars()
        {
            if (UseBuiltInToolbars)
            {
                if (_builtInToolbars == null)
                {
                    _builtInToolbars = new EditorBuiltInToolbars();
                    _builtInToolbars.ZoomChanged += OnZoomChanged;
                    _builtInToolbars.ZoomToFitRequested += OnZoomPickerZoomToFitRequested;
                    _builtInToolbarsHost.Content = _builtInToolbars;

                    if (IsLoaded)
                    {
                        HookAnnotationToolbarEvents();
                    }
                }
            }
            else if (_builtInToolbars != null)
            {
                UnhookAnnotationToolbarEvents();
                _builtInToolbars.ZoomChanged -= OnZoomChanged;
                _builtInToolbars.ZoomToFitRequested -= OnZoomPickerZoomToFitRequested;
                _builtInToolbarsHost.Content = null;
                _builtInToolbars = null;
            }
        }

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            UpdateDpiScaleFromTopLevel();
            RequestOverlayCanvasLayoutUpdate();
        }

        /// <summary>
        /// Reads the current render scaling from the host TopLevel and propagates it to the
        /// ViewModel so that <see cref="MainViewModel.EffectiveZoom"/> can compensate for the
        /// Windows display scale factor.  Called on every layout pass so that a move to a
        /// different-DPI monitor is picked up without a dedicated event subscription.
        /// </summary>
        private void UpdateDpiScaleFromTopLevel()
        {
            double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            if (Math.Abs(scaling - _lastRenderScaling) <= 0.0001)
            {
                return;
            }

            _lastRenderScaling = scaling;
            if (DataContext is MainViewModel vm)
            {
                vm.DpiScale = scaling;
            }

            UpdateCursorForTool();

            // Force an immediate overlay canvas refresh after the DPI change so that
            // selection handles reposition correctly on the rescaled canvas.
            RequestOverlayCanvasLayoutUpdate();
        }

        private void OnCanvasScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            RequestOverlayCanvasLayoutUpdate();
        }

        private void RequestOverlayCanvasLayoutUpdate()
        {
            if (_overlayCanvasLayoutUpdatePending)
            {
                return;
            }

            _overlayCanvasLayoutUpdatePending = true;
            Dispatcher.UIThread.Post(() =>
            {
                _overlayCanvasLayoutUpdatePending = false;
                UpdateOverlayCanvasLayout();
            }, DispatcherPriority.Render);
        }

        private void UpdateOverlayCanvasLayout()
        {
            var overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");
            var overlayHost = this.FindControl<Canvas>("OverlayHost");
            var canvasContainer = this.FindControl<Grid>("CanvasContainer");
            var vm = DataContext as MainViewModel;

            if (overlayCanvas == null || overlayHost == null || canvasContainer == null)
            {
                return;
            }

            double contentWidth = canvasContainer.Bounds.Width;
            double contentHeight = canvasContainer.Bounds.Height;

            if (contentWidth <= 0 || contentHeight <= 0)
            {
                return;
            }

            var contentOrigin = canvasContainer.TranslatePoint(default, overlayHost);
            if (!contentOrigin.HasValue)
            {
                return;
            }

            double zoom = vm?.EffectiveZoom ?? (vm?.Zoom ?? 1.0);
            var overlayRect = new Rect(
                contentOrigin.Value.X - (OverlayCanvasBleed * zoom),
                contentOrigin.Value.Y - (OverlayCanvasBleed * zoom),
                contentWidth + (OverlayCanvasBleed * 2),
                contentHeight + (OverlayCanvasBleed * 2));

            Rect? previousOverlayRect = _lastOverlayCanvasRect;
            bool zoomChanged = Math.Abs(_lastOverlayCanvasZoom - zoom) >= 0.0001;

            if (previousOverlayRect == overlayRect && !zoomChanged)
            {
                return;
            }

            _lastOverlayCanvasRect = overlayRect;
            _lastOverlayCanvasZoom = zoom;

            if (!previousOverlayRect.HasValue || Math.Abs(previousOverlayRect.Value.Width - overlayRect.Width) >= 0.0001)
            {
                overlayCanvas.Width = overlayRect.Width;
            }

            if (!previousOverlayRect.HasValue || Math.Abs(previousOverlayRect.Value.Height - overlayRect.Height) >= 0.0001)
            {
                overlayCanvas.Height = overlayRect.Height;
            }

            if (!previousOverlayRect.HasValue || Math.Abs(previousOverlayRect.Value.Left - overlayRect.Left) >= 0.0001)
            {
                Canvas.SetLeft(overlayCanvas, overlayRect.Left);
            }

            if (!previousOverlayRect.HasValue || Math.Abs(previousOverlayRect.Value.Top - overlayRect.Top) >= 0.0001)
            {
                Canvas.SetTop(overlayCanvas, overlayRect.Top);
            }

            if (zoomChanged || overlayCanvas.RenderTransform is null)
            {
                overlayCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
                overlayCanvas.RenderTransform = new ScaleTransform(zoom, zoom);
            }
        }

        private void OnSelectionChanged(bool hasSelection)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.HasSelectedAnnotation = hasSelection;
                var annotation = _selectionController.SelectedShape?.Tag as Annotation;
                vm.SelectedAnnotation = annotation;

                // Sync selection to EditorCore so z-order operations work
                if (annotation != null)
                {
                    _editorCore.Select(annotation);
                }
                else
                {
                    _editorCore.Deselect();
                }

                // Sync VM properties with selected annotation to update UI
                if (vm.SelectedAnnotation != null)
                {
                    // Prevent feedback loop: UI update -> VM Property Changed -> Apply to Annotation (redundant)
                    // But Apply... methods limit damage.

                    // Don't sync stroke properties from ImageAnnotation, effect annotations,
                    // Spotlight, or SmartEraser. They use nonstandard option semantics and
                    // would clobber the shared defaults for other tools.
                    if (vm.SelectedAnnotation is not ImageAnnotation
                        && vm.SelectedAnnotation is not BaseEffectAnnotation
                        && vm.SelectedAnnotation is not SpotlightAnnotation
                        && vm.SelectedAnnotation is not SmartEraserAnnotation)
                    {
                        vm.SelectedColor = vm.SelectedAnnotation.StrokeColor;
                        vm.StrokeWidth = (int)vm.SelectedAnnotation.StrokeWidth;
                        vm.ShadowEnabled = vm.SelectedAnnotation.ShadowEnabled;
                        vm.ShadowColorValue = Avalonia.Media.Color.Parse(vm.SelectedAnnotation.ShadowColor);
                        vm.ShadowBlurRadius = vm.SelectedAnnotation.ShadowBlurRadius;
                        vm.ShadowOpacity = vm.SelectedAnnotation.ShadowOpacity;
                        vm.ShadowOffsetX = vm.SelectedAnnotation.ShadowOffsetX;
                        vm.ShadowOffsetY = vm.SelectedAnnotation.ShadowOffsetY;
                    }

                    if (vm.SelectedAnnotation is NumberAnnotation num)
                    {
                        vm.FontSize = num.FontSize;
                        vm.TextBold = num.IsBold;
                        vm.FillColor = num.FillColor;
                        vm.SpeechBalloonTail = num.TailEnabled;
                        if (!string.IsNullOrEmpty(num.TextColor))
                            vm.TextColorValue = Avalonia.Media.Color.Parse(num.TextColor);
                    }
                    else if (vm.SelectedAnnotation is TextAnnotation text)
                    {
                        vm.FontSize = text.FontSize;
                        vm.SelectedFontFamily = text.FontFamily;
                        vm.SelectedTextHorizontalAlignment = text.HorizontalAlignment;
                        vm.TextBold = text.IsBold;
                        vm.TextItalic = text.IsItalic;
                        if (!string.IsNullOrEmpty(text.TextColor))
                            vm.TextColorValue = Avalonia.Media.Color.Parse(text.TextColor);
                    }
                    else if (vm.SelectedAnnotation is SpeechBalloonAnnotation balloon)
                    {
                        vm.FontSize = balloon.FontSize;
                        vm.SelectedFontFamily = balloon.FontFamily;
                        vm.SelectedTextHorizontalAlignment = balloon.HorizontalAlignment;
                        vm.TextBold = balloon.IsBold;
                        vm.TextItalic = balloon.IsItalic;
                        vm.FillColor = balloon.FillColor;
                        vm.CornerRadius = balloon.CornerRadius;
                        vm.SpeechBalloonTail = balloon.TailEnabled;
                        if (!string.IsNullOrEmpty(balloon.TextColor))
                            vm.TextColorValue = Avalonia.Media.Color.Parse(balloon.TextColor);
                    }
                    else if (vm.SelectedAnnotation is RectangleAnnotation rect && vm.SelectedAnnotation is not SmartEraserAnnotation)
                    {
                        vm.SelectedBorderStyle = rect.BorderStyle;
                        vm.FillColor = rect.FillColor;
                        vm.CornerRadius = rect.CornerRadius;
                    }
                    else if (vm.SelectedAnnotation is LineAnnotation line)
                    {
                        vm.SelectedBorderStyle = line.BorderStyle;
                    }
                    else if (vm.SelectedAnnotation is FreehandAnnotation freehand)
                    {
                        vm.SelectedBorderStyle = freehand.BorderStyle;
                    }
                    else if (vm.SelectedAnnotation is CursorAnnotation cursor)
                    {
                        vm.SelectedCursorType = cursor.CursorType;
                    }
                    else if (vm.SelectedAnnotation is ArrowAnnotation arrow)
                    {
                        vm.SelectedArrowStyle = arrow.Style;
                    }
                    else if (vm.SelectedAnnotation is EllipseAnnotation ellipse)
                    {
                        vm.SelectedBorderStyle = ellipse.BorderStyle;
                        vm.FillColor = ellipse.FillColor;
                    }
                    else if (vm.SelectedAnnotation is SpotlightAnnotation spotlight)
                    {
                        vm.EffectStrength = (int)Math.Round(
                            spotlight.DarkenOpacity / 255f * MainViewModel.GetMaxEffectStrength(EditorTool.Spotlight),
                            MidpointRounding.AwayFromZero);
                        vm.SpotlightBlur = spotlight.BlurAmount;
                        vm.EffectEllipse = spotlight.IsEllipse;
                    }
                    else if (vm.SelectedAnnotation is BaseEffectAnnotation effect)
                    {
                        vm.EffectStrength = (int)effect.Amount;
                        if (effect is MagnifyAnnotation magnify)
                        {
                            vm.EffectEllipse = magnify.IsEllipse;
                        }
                        if (effect is HighlightAnnotation highlight)
                        {
                            vm.FillColor = highlight.FillColor;
                        }
                    }
                }
            }
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // Check clipboard initially
            _ = CheckClipboardStatus();

            // Attach key handlers to the parent Window so shortcuts work
            // regardless of which child control has focus (buttons, dropdowns, etc.).
            _parentWindow = TopLevel.GetTopLevel(this) as Window;
            if (_parentWindow != null)
            {
                _parentWindow.KeyDown += OnKeyDown;
                _parentWindow.KeyUp += OnKeyUp;
                _parentWindow.Activated += OnWindowActivated;
            }

            // Give the editor initial focus
            this.Focus();

            if (DataContext is MainViewModel vm)
            {
                vm.AttachEditorCore(_editorCore);
                _editorCore.ActiveTool = vm.ActiveTool;
                HookAnnotationToolbarEvents();

                vm.DeleteRequested += (s, args) => PerformDelete();
                vm.UndoRequested += (s, args) => PerformUndo();
                vm.RedoRequested += (s, args) => PerformRedo();
                vm.ClearAnnotationsRequested += (s, args) => ClearAllAnnotations();

                // Subscribe to new context menu events
                vm.CutAnnotationRequested += OnCutRequested;
                vm.CopyAnnotationRequested += OnCopyRequested;
                vm.PasteRequested += OnPasteRequested;
                vm.DuplicateRequested += OnDuplicateRequested;
                vm.ZoomToFitRequested += OnZoomToFitRequested;
                vm.FlattenRequested += OnFlattenRequested;
                vm.ImageInsertionRequested += OnImageInsertionRequested;
                vm.EmojiInsertionRequested += OnEmojiInsertionRequested;

                // File menu event handlers (Image Editor Mode)
                vm.NewImageRequested += OnNewImageRequested;
                vm.OpenImageRequested += OnOpenImageRequested;
                vm.StartScreenRequested += OnStartScreenRequested;
                vm.LoadFromClipboardRequested += OnLoadFromClipboardRequested;
                vm.LoadFromUrlRequested += OnLoadFromUrlRequested;
                vm.LoadRecentFileRequested += OnLoadRecentFileRequested;
                vm.CopyRequested += OnCopyImageRequested;
                vm.SaveRequested += OnSaveRequested;
                vm.SaveAsRequested += OnSaveAsRequested;
                vm.FileMenuRequested += OnFileMenuRequested;

                // Original code subscribed to vm.PropertyChanged
                vm.PropertyChanged += OnViewModelPropertyChanged;

                // Initialize zoom
                _zoomController.InitLastZoom(vm.Zoom);
                UpdateCursorForTool();

                // Wire up View interactions
                vm.DeselectRequested += OnDeselectRequested;
                vm.CanvasFocusRequested += OnCanvasFocusRequested;

                // Initial load
                if (vm.PreviewImage != null)
                {
                    bool isInitialImageLoad = _editorCore.SourceImage == null;
                    LoadImageFromViewModel(vm);
                    if (isInitialImageLoad)
                    {
                        QueueAutoCopyImageToClipboard(vm);
                    }
                }
                else if (vm.ShowStartScreen)
                {
                    // No image loaded — show the start screen dialog
                    vm.RequestStartScreen();
                }

                // Reset dirty flag after initial load — loading the image fires HistoryChanged
                // and OnPreviewImageChanged which both set IsDirty=true as a side-effect.
                vm.IsDirty = false;
            }

        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);

            if (_parentWindow != null)
            {
                _parentWindow.KeyDown -= OnKeyDown;
                _parentWindow.KeyUp -= OnKeyUp;
                _parentWindow.Activated -= OnWindowActivated;
            }

            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
                vm.DeselectRequested -= OnDeselectRequested;
                vm.ZoomToFitRequested -= OnZoomToFitRequested;
                vm.NewImageRequested -= OnNewImageRequested;
                vm.OpenImageRequested -= OnOpenImageRequested;
                vm.StartScreenRequested -= OnStartScreenRequested;
                vm.LoadFromClipboardRequested -= OnLoadFromClipboardRequested;
                vm.LoadFromUrlRequested -= OnLoadFromUrlRequested;
                vm.LoadRecentFileRequested -= OnLoadRecentFileRequested;
                vm.CopyRequested -= OnCopyImageRequested;
                vm.SaveRequested -= OnSaveRequested;
                vm.SaveAsRequested -= OnSaveAsRequested;
                vm.FileMenuRequested -= OnFileMenuRequested;
                vm.ImageInsertionRequested -= OnImageInsertionRequested;
                vm.EmojiInsertionRequested -= OnEmojiInsertionRequested;
            }

            UnhookAnnotationToolbarEvents();
            StopEasterEggs();
            _selectionController.RequestUpdateEffect -= OnRequestUpdateEffect;
            ClearEffectPreviewCache();
        }

        private void OnWindowActivated(object? sender, EventArgs e)
        {
            _ = CheckClipboardStatus();
        }

        private void OnEffectBrowserOverlayPointerPressed(object? sender, global::Avalonia.Input.PointerPressedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.IsEffectBrowserVisible)
            {
                if (vm.CloseEffectsPanelCommand.CanExecute(null))
                {
                    vm.CloseEffectsPanelCommand.Execute(null);
                }
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is MainViewModel vm)
            {
                if (e.PropertyName == nameof(MainViewModel.SelectedColor))
                {
                    ApplySelectedColor(vm.SelectedColor);
                }
                else if (e.PropertyName == nameof(MainViewModel.StrokeWidth))
                {
                    ApplySelectedStrokeWidth(vm.StrokeWidth);
                }
                else if (e.PropertyName == nameof(MainViewModel.FillColorValue))
                {
                    ApplySelectedFillColor(vm.FillColor);
                }
                else if (e.PropertyName == nameof(MainViewModel.TextColorValue))
                {
                    ApplySelectedTextColor(vm.TextColor);
                }
                else if (e.PropertyName == nameof(MainViewModel.CornerRadius))
                {
                    ApplySelectedCornerRadius(vm.CornerRadius);
                }
                else if (e.PropertyName == nameof(MainViewModel.PreviewImage))
                {
                    bool isInitialImageLoad = vm.PreviewImage != null && _editorCore.SourceImage == null;
                    ClearEffectPreviewCache();
                    _zoomController.ResetScrollViewerOffset();
                    // During smart padding, use UpdateSourceImage to preserve history and annotations
                    if (vm.IsSmartPaddingInProgress)
                    {
                        UpdateSourceImageFromViewModel(vm);
                    }
                    else
                    {
                        LoadImageFromViewModel(vm);
                    }

                    if (isInitialImageLoad)
                    {
                        QueueAutoCopyImageToClipboard(vm);
                    }
                }
                else if (e.PropertyName == nameof(MainViewModel.Zoom))
                {
                    _zoomController.HandleZoomPropertyChanged(vm);
                }
                else if (e.PropertyName == nameof(MainViewModel.ActiveTool))
                {
                    _editorCore.ActiveTool = vm.ActiveTool;

                    if (vm.ActiveTool == EditorTool.Crop)
                    {
                        if (vm.Options.QuickCrop)
                        {
                            _inputController.CancelCrop();
                        }
                        else
                        {
                            _inputController.ActivateCropToFullImage();
                        }
                        this.Focus();
                    }
                    else
                    {
                        _inputController.CancelCrop();
                    }
                    _selectionController.ClearSelection();
                    UpdateCursorForTool(); // ISSUE-018 fix: Update cursor feedback for active tool
                }
                else if (e.PropertyName == nameof(MainViewModel.IsEffectBrowserVisible))
                {
                    if (vm.IsEffectBrowserVisible)
                    {
                        EnsureEffectBrowserPanel(vm).FocusSearchBox();
                    }
                }
                else if (e.PropertyName == nameof(MainViewModel.ModalContent) &&
                    vm.ModalContent is EmojiPickerDialogViewModel)
                {
                    PositionModalOnCursorScreen();
                }
                else if (e.PropertyName == nameof(MainViewModel.IsModalOpen) && !vm.IsModalOpen)
                {
                    ResetModalContentPosition();
                }
                else if (e.PropertyName == nameof(MainViewModel.NotificationMessage) &&
                    !string.IsNullOrEmpty(vm.NotificationMessage))
                {
                    PositionNotificationOnCursorScreen();
                }
                else if (e.PropertyName == nameof(MainViewModel.StepStartNumber))
                {
                    vm.RecalculateNumberCounter(_editorCore.Annotations);
                }
                else if (e.PropertyName == nameof(MainViewModel.SelectedTextHorizontalAlignment))
                {
                    ApplySelectedTextHorizontalAlignment(vm.SelectedTextHorizontalAlignment);
                }
                else if (e.PropertyName == nameof(MainViewModel.SelectedStepType))
                {
                    ApplyStepTypeToAnnotations(vm.SelectedStepType);
                }
            }
        }

        private void OnFileMenuRequested(object? sender, EventArgs e)
        {
            _builtInToolbars?.OpenFileMenu();
        }

        private EffectBrowserPanel EnsureEffectBrowserPanel(MainViewModel vm)
        {
            if (_effectBrowserPanel == null)
            {
                _effectBrowserPanel = new EffectBrowserPanel();
                _effectBrowserPanel.EffectDialogRequested += OnEffectDialogRequested;

                var effectBrowserHost = this.FindControl<ContentControl>("EffectBrowserHost");
                if (effectBrowserHost != null)
                {
                    effectBrowserHost.Content = _effectBrowserPanel;
                }
            }

            if (!ReferenceEquals(_effectBrowserPanelOptions, vm.Options))
            {
                _effectBrowserPanel.SetOptions(vm.Options);
                _effectBrowserPanelOptions = vm.Options;
            }

            return _effectBrowserPanel;
        }

        /// <summary>
        /// ISSUE-018 fix: Updates the editor canvas cursor based on the active tool.
        /// The overlay canvas sits on top of the annotation canvas, so both must stay in sync.
        /// </summary>
        internal Cursor GetCursorForActiveTool()
        {
            if (DataContext is not MainViewModel vm)
            {
                return ArrowCursor;
            }

            return vm.ActiveTool switch
            {
                EditorTool.Select => ArrowCursor,
                EditorTool.Crop or EditorTool.CutOut => GetCrosshairCursor(),
                _ => GetCrosshairCursor()
            };
        }

        internal Cursor GetCrosshairCursor()
        {
            return CursorAssetLoader.GetCrosshairCursor(GetCurrentRenderScaling());
        }

        internal Cursor GetOpenHandCursor()
        {
            return CursorAssetLoader.GetOpenHandCursor(GetCurrentRenderScaling());
        }

        internal Cursor GetClosedHandCursor()
        {
            return CursorAssetLoader.GetClosedHandCursor(GetCurrentRenderScaling());
        }

        private Cursor GetCustomCursor(CursorAssetLoader.CustomCursorKind cursorAsset)
        {
            return cursorAsset switch
            {
                CursorAssetLoader.CustomCursorKind.ClosedHand => GetClosedHandCursor(),
                CursorAssetLoader.CustomCursorKind.Crosshair => GetCrosshairCursor(),
                _ => GetOpenHandCursor()
            };
        }

        private double GetCurrentRenderScaling()
        {
            double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? _lastRenderScaling;
            return double.IsFinite(scaling) && scaling > 0 ? scaling : 1.0;
        }

        internal void ApplyAnnotationCursor(Control? control, Cursor cursor)
        {
            if (control == null || control.Tag is not Annotation)
            {
                return;
            }

            ApplyCursorToControlTree(control, cursor);
        }

        internal void SyncAnnotationCursor(Control? control)
        {
            ApplyAnnotationCursor(control, GetCursorForActiveTool());
        }

        private void UpdateCursorForTool()
        {
            if (DataContext is not MainViewModel vm) return;

            if (_interactionCursorOverride != null)
            {
                if (_selectionController.IsInteractionActive || _zoomController.IsPanning || _inputController.IsCropInteractionActive)
                {
                    if (_interactionCursorAsset is CursorAssetLoader.CustomCursorKind interactionCursorAsset)
                    {
                        ApplyInteractionCursor(GetCustomCursor(interactionCursorAsset), interactionCursorAsset);
                    }
                    else
                    {
                        ApplyInteractionCursor(_interactionCursorOverride);
                    }
                }
                else
                {
                    _interactionCursorOverride = null;
                    _interactionCursorAsset = null;
                    HideInteractionCaptureLayer();
                }
            }

            var canvasScrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            var previewFrame = this.FindControl<Border>("PreviewFrame");
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            var overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");
            if (canvasScrollViewer == null && previewFrame == null && annotationCanvas == null && overlayCanvas == null) return;

            Cursor cursor = GetCursorForActiveTool();
            Cursor surfaceCursor = vm.HasPreviewImage ? cursor : ArrowCursor;

            if (canvasScrollViewer != null)
            {
                canvasScrollViewer.Cursor = surfaceCursor;
            }

            if (previewFrame != null)
            {
                previewFrame.Cursor = surfaceCursor;
            }

            if (annotationCanvas != null)
            {
                annotationCanvas.Cursor = cursor;
                UpdateAnnotationCanvasChildCursors(annotationCanvas, cursor);
            }

            if (overlayCanvas != null)
            {
                overlayCanvas.Cursor = cursor;
            }

            _selectionController.RefreshHoveredShapeCursor();
        }

        private void UpdateAnnotationCanvasChildCursors(Canvas annotationCanvas, Cursor cursor)
        {
            foreach (var child in annotationCanvas.Children)
            {
                if (child is Control control && control.Tag is Annotation)
                {
                    ApplyCursorToControlTree(control, cursor);
                }
            }
        }

        private static void ApplyCursorToControlTree(Control control, Cursor cursor)
        {
            control.Cursor = cursor;

            foreach (var descendant in control.GetVisualDescendants())
            {
                if (descendant is InputElement inputElement)
                {
                    inputElement.Cursor = cursor;
                }
            }
        }

        internal void ApplyInteractionCursor(Cursor cursor, CursorAssetLoader.CustomCursorKind? cursorAsset = null)
        {
            _interactionCursorOverride = cursor;
            _interactionCursorAsset = cursorAsset;
            var interactionLayer = this.FindControl<Border>("InteractionCaptureLayer");
            if (interactionLayer != null)
            {
                interactionLayer.Cursor = cursor;
                interactionLayer.IsHitTestVisible = true;
                interactionLayer.IsVisible = true;
            }
        }

        internal void ApplyInteractionCursor(CursorAssetLoader.CustomCursorKind cursorAsset)
        {
            ApplyInteractionCursor(GetCustomCursor(cursorAsset), cursorAsset);
        }

        internal void BeginInteractionCursorCapture(IPointer pointer, Cursor cursor, CursorAssetLoader.CustomCursorKind? cursorAsset = null)
        {
            ApplyInteractionCursor(cursor, cursorAsset);

            var interactionLayer = this.FindControl<Border>("InteractionCaptureLayer");
            if (interactionLayer != null)
            {
                pointer.Capture(interactionLayer);
            }
        }

        internal void BeginInteractionCursorCapture(IPointer pointer, CursorAssetLoader.CustomCursorKind cursorAsset)
        {
            ApplyInteractionCursor(cursorAsset);

            var interactionLayer = this.FindControl<Border>("InteractionCaptureLayer");
            if (interactionLayer != null)
            {
                pointer.Capture(interactionLayer);
            }
        }

        internal void RestoreEditorSurfaceCursorForActiveTool()
        {
            if (_selectionController.IsInteractionActive || _zoomController.IsPanning || _inputController.IsCropInteractionActive)
            {
                return;
            }

            _interactionCursorOverride = null;
            _interactionCursorAsset = null;
            HideInteractionCaptureLayer();
            UpdateCursorForTool();
        }

        private void HideInteractionCaptureLayer()
        {
            var interactionLayer = this.FindControl<Border>("InteractionCaptureLayer");
            if (interactionLayer != null)
            {
                interactionLayer.IsHitTestVisible = false;
                interactionLayer.IsVisible = false;
                interactionLayer.Cursor = ArrowCursor;
            }
        }

        // --- Public/Internal Methods for Controllers ---

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _canvasControl = this.FindControl<SKCanvasControl>("CanvasControl");
        }

        /// <summary>
        /// Configures the editor canvas as a pixel-aligned workspace embedded in a fullscreen host.
        /// The host supplies its own toolbars and completion controls.
        /// </summary>
        public void ConfigureForFullscreenWorkspace()
        {
            _isWorkspaceHostMode = true;

            if (this.FindControl<Grid>("EditorCanvasHost") is Grid canvasHost)
            {
                canvasHost.Margin = new Thickness(0);
            }

            if (this.FindControl<ScrollViewer>("CanvasScrollViewer") is ScrollViewer scrollViewer)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        /// <summary>
        /// Loads an owned bitmap directly into the shared workspace without creating the
        /// normal editor preview and backup copies. This is intended for very large,
        /// immutable capture backgrounds.
        /// </summary>
        public void LoadWorkspaceImage(SKBitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap);

            if (_canvasControl == null)
            {
                throw new InvalidOperationException("The editor workspace must be initialized before loading an image.");
            }

            _suppressNextHistoryDirtyMark = true;
            _canvasControl.Initialize(bitmap.Width, bitmap.Height);
            _editorCore.LoadImage(bitmap);

            if (DataContext is MainViewModel vm)
            {
                vm.SyncImageDimensions(bitmap.Width, bitmap.Height);
                vm.Zoom = 1.0;
                vm.IsDirty = false;
            }
        }

        /// <summary>Reads a pixel from the live raster owned by an embedded workspace.</summary>
        public SKColor GetWorkspacePixel(int x, int y)
        {
            SKBitmap? sourceImage = _editorCore.SourceImage;
            if (sourceImage == null ||
                x < 0 || y < 0 ||
                x >= _editorCore.CanvasSize.Width || y >= _editorCore.CanvasSize.Height)
            {
                return SKColors.Transparent;
            }

            return sourceImage.GetPixel(x, y);
        }

        /// <summary>Deletes the topmost annotation under a point supplied by an embedded host.</summary>
        public bool DeleteWorkspaceAnnotationAt(Point workspacePoint)
        {
            Canvas? canvas = this.FindControl<Canvas>("AnnotationCanvas");
            Point? canvasPoint = canvas == null ? null : this.TranslatePoint(workspacePoint, canvas);
            if (canvas == null || !canvasPoint.HasValue)
            {
                return false;
            }

            Control? shape = _selectionController.HitTestShape(canvas, canvasPoint.Value);
            if (shape == null)
            {
                return false;
            }

            _selectionController.SetSelectedShape(shape);
            PerformDelete();
            return true;
        }

        /// <summary>
        /// Gives a host the same staged Escape behavior as the editor without closing its window.
        /// </summary>
        public bool CancelActiveInteractionOrSelection()
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.IsModalOpen)
                {
                    vm.CloseModalCommand.Execute(null);
                    return true;
                }

                if (vm.IsEffectsPanelOpen)
                {
                    vm.CloseEffectsPanelCommand.Execute(null);
                    return true;
                }
            }

            if (_inputController.CancelCrop())
            {
                return true;
            }

            if (_selectionController.SelectedShape != null)
            {
                _selectionController.ClearSelection();
                return true;
            }

            return false;
        }

        /// <summary>Releases the large raster buffers owned by an embedded workspace.</summary>
        public void DisposeWorkspace()
        {
            if (_workspaceDisposed)
            {
                return;
            }

            _workspaceDisposed = true;
            ClearEffectPreviewCache();
            _canvasControl?.Dispose();
            _editorCore.Dispose();
        }

        private void LoadImageFromViewModel(MainViewModel vm)
        {
            if (vm.PreviewImage == null || _canvasControl == null) return;
            if (_isSyncingToVM) return; // Ignore updates that we just pushed to VM

            try
            {
                _isSyncingFromVM = true;

                using var skBitmap = !vm.IsEffectPreviewActive
                    ? vm.CreateSourceImageCopyForCore() ?? BitmapConversionHelpers.ToSKBitmap(vm.PreviewImage)
                    : BitmapConversionHelpers.ToSKBitmap(vm.PreviewImage);
                if (skBitmap != null)
                {
                    // We must copy because ToSKBitmap might return a disposable wrapper or we need ownership
                    // ISSUE-FIX: Use UpdateSourceImage to preserve existing history/annotations
                    // This allows VM-driven updates (Effects, Undo) to not wipe Core state.
                    // New file loads should be preceded by Clear() from the VM/Host.
                    _skipNextCoreImageChanged = true;
                    _editorCore.UpdateSourceImage(skBitmap.Copy());

                    _canvasControl.Initialize(skBitmap.Width, skBitmap.Height);
                    RenderCore();
                    QueueZoomToFitOnOpenIfNeeded(vm);
                }
            }
            finally
            {
                _isSyncingFromVM = false;
            }
        }

        /// <summary>
        /// Updates the source image in EditorCore without clearing history or annotations.
        /// Used during smart padding operations to preserve editing state.
        /// </summary>
        private void UpdateSourceImageFromViewModel(MainViewModel vm)
        {
            if (vm.PreviewImage == null || _canvasControl == null) return;

            using var skBitmap = !vm.IsEffectPreviewActive
                ? vm.CreateSourceImageCopyForCore() ?? BitmapConversionHelpers.ToSKBitmap(vm.PreviewImage)
                : BitmapConversionHelpers.ToSKBitmap(vm.PreviewImage);
            if (skBitmap != null)
            {
                _skipNextCoreImageChanged = true;
                _editorCore.UpdateSourceImage(skBitmap.Copy());
                _canvasControl.Initialize(skBitmap.Width, skBitmap.Height);
                RenderCore();
            }
        }

        private void QueueZoomToFitOnOpenIfNeeded(MainViewModel vm)
        {
            if (!vm.ConsumeZoomToFitOnNextImageLoad())
            {
                return;
            }

            _pendingZoomToFitOnOpen = true;
            _pendingZoomToFitRetryCount = 4;
            TryApplyPendingZoomToFitOnOpen();
        }

        private void TryApplyPendingZoomToFitOnOpen()
        {
            if (!_pendingZoomToFitOnOpen)
            {
                return;
            }

            if (_zoomController.ZoomToFit())
            {
                _pendingZoomToFitOnOpen = false;
                return;
            }

            if (_pendingZoomToFitRetryCount-- <= 0)
            {
                _pendingZoomToFitOnOpen = false;
                return;
            }

            Dispatcher.UIThread.Post(TryApplyPendingZoomToFitOnOpen, DispatcherPriority.Render);
        }

        private void QueueAutoCopyImageToClipboard(MainViewModel vm)
        {
            if (_isWorkspaceHostMode || !vm.Options.AutoCopyImageToClipboard || !vm.HasPreviewImage)
            {
                return;
            }

            int version = ++_pendingAutoCopyImageVersion;

            Dispatcher.UIThread.Post(async () =>
            {
                if (version != _pendingAutoCopyImageVersion)
                {
                    return;
                }

                AutoCopyImageToClipboard(vm);
            }, DispatcherPriority.Background);
        }

        private async void AutoCopyImageToClipboard(MainViewModel vm)
        {
            if (_isWorkspaceHostMode || !vm.Options.AutoCopyImageToClipboard || !vm.HasPreviewImage)
            {
                return;
            }

            try
            {
                await vm.RequestCopyToClipboardAsync();
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), "Failed to raise auto-copy image request.", ex);
            }
        }

        // --- Event Handlers Delegated to Controllers ---

        private void OnPreviewPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            _zoomController.OnPreviewPointerWheelChanged(sender, e);
        }

        private void OnScrollViewerPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _zoomController.OnScrollViewerPointerPressed(sender, e);

            if (!e.Handled)
            {
                _inputController.OnCanvasPointerPressed(sender, e);
            }
        }

        private void OnScrollViewerPointerMoved(object? sender, PointerEventArgs e)
        {
            _zoomController.OnScrollViewerPointerMoved(sender, e);
        }

        private void OnScrollViewerPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _zoomController.OnScrollViewerPointerReleased(sender, e);
        }

        private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _inputController.OnCanvasPointerPressed(sender, e);
        }

        private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
        {
            _inputController.OnCanvasPointerMoved(sender, e);
        }

        private void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _inputController.OnCanvasPointerReleased(sender, e);
        }

        private void OnNotificationPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DismissNotification();
                e.Handled = true;
            }
        }

        private void PositionNotificationOnCursorScreen()
        {
            Grid? notificationHost = this.FindControl<Grid>("EditorNotificationHost");
            if (notificationHost == null)
            {
                return;
            }

            notificationHost.RenderTransform = null;
            if (!_isWorkspaceHostMode)
            {
                return;
            }

            Point? screenCenter = GetCursorScreenCenter(this);
            if (screenCenter.HasValue)
            {
                notificationHost.RenderTransform = new TranslateTransform(
                    screenCenter.Value.X - Bounds.Width / 2,
                    0);
            }
        }

        private void OnNotificationPointerEntered(object? sender, PointerEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SetNotificationHoverState(true);
            }
        }

        private void OnNotificationPointerExited(object? sender, PointerEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SetNotificationHoverState(false);
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _inputController.OnKeyDown(sender ?? this, e);

            // Skip shortcuts when the user is typing in a text field
            if (_parentWindow?.FocusManager?.GetFocusedElement() is TextBox) return;

            // Skip shortcuts when a modal dialog is open (e.g. emoji picker search box)
            if (DataContext is MainViewModel { IsModalOpen: true }) return;

            if (HandleEasterEggKeyDown(e)) return;

            if (e.KeyModifiers.HasFlag(KeyModifiers.Control) && !_inputController.IsDrawingActive)
            {
                _selectionController.RefreshHoverFeedback(e.KeyModifiers);
            }

            if (DataContext is MainViewModel vm)
            {
                if (e.Key == Key.Delete)
                {
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        if (vm.ClearAnnotationsCommand.CanExecute(null))
                        {
                            vm.ClearAnnotationsCommand.Execute(null);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        vm.DeleteSelectedCommand.Execute(null);
                        e.Handled = true;
                    }
                }
                else if (e.KeyModifiers.HasFlag(KeyModifiers.Control | KeyModifiers.Shift))
                {
                    if (vm.TrySelectToolForToolbarHotkey(e.Key, e.KeyModifiers))
                    {
                        e.Handled = true;
                        return;
                    }

                    switch (e.Key)
                    {
                        case Key.Z: vm.RedoCommand.Execute(null); e.Handled = true; break;
                        case Key.C:
                            if (vm.CopyAnnotationCommand.CanExecute(null))
                            {
                                vm.CopyAnnotationCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                        case Key.F: vm.FlattenImageCommand.Execute(null); e.Handled = true; break;
                        case Key.P:
                            if (vm.PrintCommand.CanExecute(null))
                            {
                                vm.PrintCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                        case Key.S: vm.SaveAsCommand.Execute(null); e.Handled = true; break;
                    }
                }
                else if (e.KeyModifiers.HasFlag(KeyModifiers.Control) && !e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    if (vm.TrySelectToolForToolbarHotkey(e.Key, e.KeyModifiers))
                    {
                        e.Handled = true;
                        return;
                    }

                    switch (e.Key)
                    {
                        case Key.Z: vm.UndoCommand.Execute(null); e.Handled = true; break;
                        case Key.Y: vm.RedoCommand.Execute(null); e.Handled = true; break;
                        case Key.H:
                            if (!_isWorkspaceHostMode)
                            {
                                vm.ToggleToolbarsCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                        case Key.X: vm.CutAnnotationCommand.Execute(null); e.Handled = true; break;
                        case Key.C:
                            if (vm.CopyCommand.CanExecute(null))
                            {
                                vm.CopyCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                        case Key.V: vm.PasteCommand.Execute(null); e.Handled = true; break;
                        case Key.D: DuplicateSelectedAnnotation(); e.Handled = true; break;
                        case Key.S: vm.SaveCommand.Execute(null); e.Handled = true; break;
                        case Key.N: vm.NewImageCommand.Execute(null); e.Handled = true; break;
                        case Key.O: vm.OpenImageCommand.Execute(null); e.Handled = true; break;
                        case Key.P:
                            if (vm.PinToScreenCommand.CanExecute(null))
                            {
                                vm.PinToScreenCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                        case Key.U:
                            if (vm.UploadCommand.CanExecute(null))
                            {
                                vm.UploadCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                    }
                }
                else if (e.KeyModifiers == KeyModifiers.None || e.KeyModifiers == KeyModifiers.Shift)
                {
                    double step = e.KeyModifiers == KeyModifiers.Shift ? 10 : 1;

                    if ((e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right) && _selectionController.SelectedShape != null)
                    {
                        double dx = e.Key == Key.Left ? -step : (e.Key == Key.Right ? step : 0);
                        double dy = e.Key == Key.Up ? -step : (e.Key == Key.Down ? step : 0);
                        _selectionController.MoveSelectedShape(dx, dy);
                        e.Handled = true;
                    }
                    else if (e.KeyModifiers == KeyModifiers.Shift)
                    {
                        if (vm.TrySelectToolForToolbarHotkey(e.Key, e.KeyModifiers))
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    else if (e.KeyModifiers == KeyModifiers.None)
                    {
                        if (vm.TrySelectToolForToolbarHotkey(e.Key, e.KeyModifiers))
                        {
                            e.Handled = true;
                            return;
                        }

                        // Tool shortcuts
                        switch (e.Key)
                        {
                            case Key.Home: _editorCore.BringToFront(); e.Handled = true; break;
                            case Key.End: _editorCore.SendToBack(); e.Handled = true; break;
                            case Key.PageUp: _editorCore.BringForward(); e.Handled = true; break;
                            case Key.PageDown: _editorCore.SendBackward(); e.Handled = true; break;

                            case Key.Enter:
                                if (_inputController.TryConfirmCrop())
                                {
                                    e.Handled = true;
                                }
                                else if (vm.ShowTaskButtons)
                                {
                                    vm.ContinueCommand.Execute(null);
                                    e.Handled = true;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _inputController.OnKeyUp(sender ?? this, e);

            if (DataContext is MainViewModel vm && e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None)
            {
                // Close emoji modal dialog on Escape (before TextBox short-circuit)
                if (vm.IsModalOpen)
                {
                    vm.CloseModalCommand.Execute(null);
                    e.Handled = true;
                    return;
                }

                // Close image effects panel on Escape (before TextBox short-circuit)
                // Covers both the effects browser (EffectsPanelContent == null) and specific
                // effect dialogs (EffectsPanelContent != null) to prevent Esc from falling
                // through to the editor-close path when any effects panel state is active.
                if (vm.IsEffectsPanelOpen)
                {
                    vm.CloseEffectsPanelCommand.Execute(null);
                    e.Handled = true;
                    return;
                }
            }

            // Skip shortcuts when the user is typing in a text field
            if (_parentWindow?.FocusManager?.GetFocusedElement() is TextBox) return;

            if (e.Key is Key.LeftCtrl or Key.RightCtrl && !_inputController.IsDrawingActive)
            {
                _selectionController.RefreshHoverFeedback(e.KeyModifiers);
            }

            if (DataContext is MainViewModel vm2 && e.KeyModifiers == KeyModifiers.None)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        if (_inputController.CancelCrop())
                        {
                            e.Handled = true;
                        }
                        else if (_selectionController.SelectedShape != null)
                        {
                            _selectionController.ClearSelection();
                            e.Handled = true;
                        }
                        else if (vm2.UseContinueWorkflow)
                        {
                            vm2.CancelCommand.Execute(null);
                            e.Handled = true;
                        }
                        else
                        {
                            vm2.ExitEditorCommand.Execute(null);
                            e.Handled = true;
                        }
                        break;
                }
            }
        }

        // --- Private Helpers (Undo/Redo, Delete, etc that involve view state) ---

        private void PerformUndo()
        {
            if (_editorCore.CanUndo)
            {
                _editorCore.Undo();
                // AnnotationsRestored event will handle UI sync
            }
        }

        private void PerformRedo()
        {
            if (_editorCore.CanRedo)
            {
                _editorCore.Redo();
            }
        }

        private void OnDeselectRequested(object? sender, EventArgs e)
        {
            _inputController.CancelCrop();
            _selectionController.ClearSelection();
        }

        private void OnCanvasFocusRequested(object? sender, EventArgs e)
        {
            this.Focus();
        }

        private Color SKColorToAvalonia(SKColor color)
        {
            return Color.FromUInt32((uint)color);
        }

        private Control? CreateControlForAnnotation(Annotation annotation)
        {
            var control = AnnotationVisualFactory.CreateVisualControl(annotation, AnnotationVisualMode.Persisted);
            if (control == null)
            {
                return null;
            }

            AnnotationVisualFactory.UpdateVisualControl(
                control,
                annotation,
                AnnotationVisualMode.Persisted,
                _editorCore.CanvasSize.Width,
                _editorCore.CanvasSize.Height);

            // Effect annotations require bitmap-backed fills from current source image.
            if (annotation is BaseEffectAnnotation)
            {
                OnRequestUpdateEffect(control);
            }

            SyncAnnotationCursor(control);

            return control;
        }

        private void PerformDelete()
        {
            var selected = _selectionController.SelectedShape;
            if (selected != null)
            {
                var canvas = this.FindControl<Canvas>("AnnotationCanvas");
                if (canvas != null && canvas.Children.Contains(selected))
                {
                    // Sync with EditorCore - this creates the undo history entry
                    if (selected.Tag is Annotation annotation)
                    {
                        // Select the annotation in core so DeleteSelected knows what to remove
                        _editorCore.Select(annotation);
                        _editorCore.DeleteSelected();
                    }

                    // Dispose annotation resources before removing from view
                    (selected.Tag as IDisposable)?.Dispose();

                    canvas.Children.Remove(selected);
                    RefreshSpotlightOverlay();

                    _selectionController.ClearSelection();

                    // Update HasAnnotations state
                    UpdateHasAnnotationsState();
                }
            }
        }

        private void ClearAllAnnotations()
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas != null)
            {
                canvas.Children.Clear();
                RefreshSpotlightOverlay();
                _selectionController.ClearSelection();
                _editorCore.ClearAll(resetHistory: false);
                RenderCore();

                // Update HasAnnotations state
                if (DataContext is MainViewModel vm)
                {
                    vm.HasAnnotations = false;
                }
            }
        }

        // --- Crop and Image Insertion ---

        public void PerformCrop()
        {
            var cropOverlay = this.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
            if (cropOverlay != null && cropOverlay.IsVisible && DataContext is MainViewModel vm)
            {
                var rect = new SkiaSharp.SKRect(
                    (float)(Canvas.GetLeft(cropOverlay) - OverlayCanvasBleed),
                    (float)(Canvas.GetTop(cropOverlay) - OverlayCanvasBleed),
                    (float)(Canvas.GetLeft(cropOverlay) - OverlayCanvasBleed + cropOverlay.Width),
                    (float)(Canvas.GetTop(cropOverlay) - OverlayCanvasBleed + cropOverlay.Height));

                if (rect.Width > 0 && rect.Height > 0)
                {
                    // Canvas coordinates are already in image-pixel space (AnnotationCanvas
                    // is sized to CanvasSize = bitmap.Width/Height). No DPI scaling needed.
                    var cropX = (int)Math.Round(rect.Left);
                    var cropY = (int)Math.Round(rect.Top);
                    var cropW = (int)Math.Round(rect.Width);
                    var cropH = (int)Math.Round(rect.Height);

                    _editorCore.Crop(new SKRect(cropX, cropY, cropX + cropW, cropY + cropH));
                }
                cropOverlay.IsVisible = false;
            }
        }

        // --- Image Paste & Drag-Drop ---

        /// <summary>
        /// Inserts an image annotation from an SKBitmap at an optional drop position.
        /// Adds the annotation to both the Avalonia canvas and EditorCore, then switches to Select tool.
        /// </summary>
        /// <remarks>
        /// XIP0039 Guardrail 6: This method is public so host applications can insert image annotations
        /// directly without resorting to reflection. The previous private access required callers such as
        /// <c>MainWindow.axaml.cs</c> to use <c>BindingFlags.NonPublic</c> reflection.
        /// </remarks>
        public void InsertImageAnnotation(SKBitmap skBitmap, Point? dropPosition = null)
        {
            InsertImageAnnotationCore(skBitmap, dropPosition);
        }

        /// <summary>Inserts host-provided capture content without treating it as a user edit.</summary>
        public void InsertWorkspaceImageAnnotation(SKBitmap skBitmap, Point? position = null)
        {
            _suppressNextHistoryDirtyMark = true;
            InsertImageAnnotationCore(skBitmap, position, showNotification: false, selectAnnotation: false);

            if (DataContext is MainViewModel vm)
            {
                vm.IsDirty = false;
            }
        }

        private void InsertEmojiAnnotation(string unicodeSequence, string displayName, Point? dropPosition = null)
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null || DataContext is not MainViewModel vm)
            {
                return;
            }

            const int defaultSize = 160;

            Point? screenCenter = dropPosition.HasValue ? null : GetCursorScreenCenter(canvas);
            double centerX = screenCenter.HasValue
                ? Math.Clamp(screenCenter.Value.X, 0, _editorCore.CanvasSize.Width)
                : _editorCore.CanvasSize.Width / 2;
            double centerY = screenCenter.HasValue
                ? Math.Clamp(screenCenter.Value.Y, 0, _editorCore.CanvasSize.Height)
                : _editorCore.CanvasSize.Height / 2;
            var posX = dropPosition?.X ?? centerX - defaultSize / 2.0;
            var posY = dropPosition?.Y ?? centerY - defaultSize / 2.0;

            var annotation = new EmojiAnnotation
            {
                UnicodeSequence = unicodeSequence,
                DisplayName = displayName,
                StartPoint = new SKPoint((float)posX, (float)posY),
                EndPoint = new SKPoint((float)(posX + defaultSize), (float)(posY + defaultSize))
            };

            var control = CreateControlForAnnotation(annotation);
            if (control == null)
            {
                return;
            }

            canvas.Children.Add(control);
            _editorCore.AddAnnotation(annotation);
            vm.HasAnnotations = true;
            vm.ActiveTool = EditorTool.Select;
            _selectionController.SetSelectedShape(control);
        }

        /// <summary>
        /// Handles DragOver event to show appropriate drag cursor.
        /// </summary>
        private void OnDragOver(object? sender, DragEventArgs e)
        {
            // Keep DragOver lightweight and non-consuming; resolve concrete files in OnDrop.
            e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        /// <summary>
        /// Handles drag-and-drop of image files onto the editor canvas.
        /// </summary>
        private async void OnDrop(object? sender, DragEventArgs e)
        {
            var droppedItems = e.DataTransfer.TryGetFiles()?.ToList() ?? new List<IStorageItem>();

            // Fallback for providers that expose files only through raw items.
            if (droppedItems.Count == 0)
            {
                foreach (var item in e.DataTransfer.Items)
                {
                    if (item.TryGetRaw(DataFormat.File) is IStorageItem storageItem)
                    {
                        droppedItems.Add(storageItem);
                    }
                }
            }

            if (droppedItems.Count > 0)
            {
                foreach (var item in droppedItems)
                {
                    if (item is IStorageFile file)
                    {
                        var ext = System.IO.Path.GetExtension(file.Name)?.ToLowerInvariant();

                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif" || ext == ".webp" || ext == ".ico" || ext == ".tiff" || ext == ".tif")
                        {
                            try
                            {
                                using var stream = await file.OpenReadAsync();
                                using var memStream = new System.IO.MemoryStream();
                                await stream.CopyToAsync(memStream);
                                memStream.Position = 0;
                                var skBitmap = SKBitmap.Decode(memStream);
                                if (skBitmap != null)
                                {
                                    // If there's no base image yet (common in embedded MainWindow editor),
                                    // use the dropped file as the main preview image.
                                    if (DataContext is MainViewModel vm && !vm.HasPreviewImage)
                                    {
                                        vm.UpdatePreview(skBitmap, clearAnnotations: true);
                                        return;
                                    }

                                    await InsertExternalImageAsync(skBitmap, file.Path.LocalPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                EditorServices.ReportWarning(nameof(EditorView), $"Failed to decode dropped image '{file.Name}'.", ex);
                            }
                        }
                    }
                }
            }
        }

        private async void OnBrowseBackgroundImageClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider == null)
            {
                return;
            }

            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.EditorView_SelectBackgroundImage,
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (files.Count > 0)
            {
                vm.SetBackgroundImagePath(files[0].Path.LocalPath);
            }
        }

        private void OnGradientColor1PreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenBackgroundColorPicker(BackgroundGradientColor1Popup, BackgroundGradientColor2Popup, BackgroundColorPopup);
            e.Handled = true;
        }

        private void OnGradientColor2PreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenBackgroundColorPicker(BackgroundGradientColor2Popup, BackgroundGradientColor1Popup, BackgroundColorPopup);
            e.Handled = true;
        }

        private void OnBackgroundColorPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenBackgroundColorPicker(BackgroundColorPopup, BackgroundGradientColor1Popup, BackgroundGradientColor2Popup);
            e.Handled = true;
        }

        private static void OpenBackgroundColorPicker(Popup popupToOpen, params Popup[] popupsToClose)
        {
            foreach (Popup popupToClose in popupsToClose)
            {
                popupToClose.IsOpen = false;
            }

            popupToOpen.IsOpen = true;
        }

        private void OnNewImageRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            var dialog = new NewImageDialogViewModel(
                onOk: (result) =>
                {
                    vm.IsModalOpen = false;

                    var color = result.Transparent ? SKColors.Transparent :
                        new SKColor(result.BackgroundColor.R, result.BackgroundColor.G, result.BackgroundColor.B, result.BackgroundColor.A);

                    var skBitmap = new SKBitmap(new SKImageInfo(result.Width, result.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
                    using var canvas = new SKCanvas(skBitmap);
                    canvas.Clear(color);

                    // Clear annotation visuals
                    var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
                    annotationCanvas?.Children.Clear();
                    RefreshSpotlightOverlay();
                    _selectionController.ClearSelection();

                    // Load fresh image into core (clears history and annotations)
                    _skipNextCoreImageChanged = true;
                    _suppressNextHistoryDirtyMark = true;
                    _editorCore.LoadImage(skBitmap);

                    // Initialize canvas control
                    _canvasControl?.Initialize(skBitmap.Width, skBitmap.Height);
                    RenderCore();

                    // Sync to VM
                    try
                    {
                        _isSyncingToVM = true;
                        vm.ImageFilePath = null;
                        vm.IsDirty = false;
                        vm.HasAnnotations = false;
                        vm.UpdateCoreHistoryState(_editorCore.CanUndo, _editorCore.CanRedo);
                        vm.UpdatePreviewImageOnly(skBitmap, syncSourceState: true);
                    }
                    finally
                    {
                        _isSyncingToVM = false;
                    }

                    vm.ShowNewImageNotification(skBitmap.Width, skBitmap.Height);
                },
                onCancel: () =>
                {
                    vm.IsModalOpen = false;
                }
            );

            vm.ModalContent = dialog;
            vm.IsModalOpen = true;
        }

        private async void OnOpenImageRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider == null)
            {
                return;
            }

            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.EditorView_OpenImage,
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (files.Count > 0)
            {
                string filePath = files[0].Path.LocalPath;

                using var stream = await files[0].OpenReadAsync();
                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);
                memStream.Position = 0;

                var skBitmap = SKBitmap.Decode(memStream);
                if (skBitmap == null) return;

                LoadBitmapIntoEditor(vm, skBitmap, filePath);
                vm.ShowOpenImageNotification(filePath);
            }
        }

        private void OnStartScreenRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;

            EnsureStartScreenDialog(vm);
        }

        private StartScreenDialogViewModel EnsureStartScreenDialog(MainViewModel vm)
        {
            if (vm.ModalContent is StartScreenDialogViewModel existingDialog)
            {
                vm.IsModalOpen = true;
                return existingDialog;
            }

            StartScreenDialogViewModel? dialog = null;

            dialog = new StartScreenDialogViewModel(
                recentFiles: vm.RecentImageFiles,
                onNewImage: () =>
                {
                    vm.CloseModalCommand.Execute(null);
                    vm.NewImageCommand.Execute(null);
                },
                onOpenFile: () =>
                {
                    vm.CloseModalCommand.Execute(null);
                    vm.OpenImageCommand.Execute(null);
                },
                onLoadFromClipboard: () =>
                {
                    vm.RequestLoadFromClipboard();
                },
                onShowUrlInput: () =>
                {
                    if (dialog != null)
                    {
                        _ = PrepareStartScreenUrlInputAsync(dialog);
                    }
                },
                onSubmitUrl: url =>
                {
                    vm.RequestLoadFromUrl(url);
                },
                onClose: () =>
                {
                    vm.CloseModalCommand.Execute(null);
                },
                onExit: () =>
                {
                    vm.CloseModalCommand.Execute(null);
                    vm.ExitEditorCommand.Execute(null);
                },
                onOpenRecentFile: path =>
                {
                    vm.RequestLoadRecentFile(path);
                });

            vm.ModalContent = dialog;
            vm.IsModalOpen = true;

            return dialog;
        }

        private async Task PrepareStartScreenUrlInputAsync(StartScreenDialogViewModel dialog)
        {
            string? clipboardUrl = null;

            try
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.Clipboard != null)
                {
                    var text = await topLevel.Clipboard.TryGetTextAsync();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        text = text.Trim();
                        if (Uri.TryCreate(text, UriKind.Absolute, out var uri) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                        {
                            clipboardUrl = text;
                        }
                    }
                }
            }
            catch
            {
                // Ignore clipboard read errors while preparing the inline URL entry.
            }

            dialog.ShowUrlInput(clipboardUrl);
        }

        private void ShowStartScreenStatus(MainViewModel vm, string message)
        {
            var dialog = EnsureStartScreenDialog(vm);
            dialog.ShowStatus(message);
        }

        private async void OnLoadFromClipboardRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;

            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.Clipboard == null)
            {
                ShowStartScreenStatus(vm, Strings.EditorView_FailedToLoadImageFromClipboard);
                return;
            }

            try
            {
                var clipboard = topLevel.Clipboard;

                // Try to get bitmap from clipboard
                var clipboardBitmap = await clipboard.TryGetBitmapAsync();
                if (clipboardBitmap != null)
                {
                    using var ms = new MemoryStream();
                    clipboardBitmap.Save(ms, PngBitmapEncoderOptions.Default);
                    (clipboardBitmap as IDisposable)?.Dispose();
                    ms.Position = 0;

                    var skBitmap = SKBitmap.Decode(ms);
                    if (skBitmap != null)
                    {
                        vm.CloseModalCommand.Execute(null);
                        LoadBitmapIntoEditor(vm, skBitmap, null);
                        return;
                    }
                }

                // Try files
                var files = await clipboard.TryGetFilesAsync();
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file is not IStorageFile storageFile) continue;

                        try
                        {
                            using var stream = await storageFile.OpenReadAsync();
                            using var memStream = new MemoryStream();
                            await stream.CopyToAsync(memStream);
                            memStream.Position = 0;

                            var skBitmap = SKBitmap.Decode(memStream);
                            if (skBitmap != null)
                            {
                                vm.CloseModalCommand.Execute(null);
                                LoadBitmapIntoEditor(vm, skBitmap, storageFile.Path.LocalPath);
                                return;
                            }
                        }
                        catch
                        {
                            // Try next file
                        }
                    }
                }

                ShowStartScreenStatus(vm, Strings.EditorView_ClipboardDoesNotContainImage);
            }
            catch (Exception ex)
            {
                EditorServices.ReportError(nameof(EditorView), "Failed to load image from clipboard.", ex);
                ShowStartScreenStatus(vm, Strings.EditorView_FailedToLoadImageFromClipboard);
            }
        }

        private async void OnLoadFromUrlRequested(object? sender, string url)
        {
            if (DataContext is not MainViewModel vm) return;

            StartScreenDialogViewModel? startScreenDialog = vm.ModalContent as StartScreenDialogViewModel;
            startScreenDialog?.ClearStatus();
            startScreenDialog?.SetUrlLoading(true);

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "ShareX");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);
                memStream.Position = 0;

                var skBitmap = SKBitmap.Decode(memStream);
                if (skBitmap == null)
                {
                    startScreenDialog?.SetUrlLoading(false);
                    startScreenDialog?.ShowStatus(Strings.EditorView_UrlDoesNotPointToValidImage);
                    return;
                }

                vm.CloseModalCommand.Execute(null);
                LoadBitmapIntoEditor(vm, skBitmap, null);
            }
            catch (Exception ex)
            {
                startScreenDialog?.SetUrlLoading(false);
                startScreenDialog?.ShowStatus(string.Format(Strings.EditorView_FailedToDownloadImageFormat, ex.Message));
            }
        }

        private void OnLoadRecentFileRequested(object? sender, string filePath)
        {
            if (DataContext is not MainViewModel vm) return;

            if (!File.Exists(filePath))
            {
                vm.RemoveRecentImageFile(filePath);
                if (vm.ModalContent is StartScreenDialogViewModel startScreenDialog)
                {
                    startScreenDialog.RecentFiles.Remove(filePath);
                }
                ShowStartScreenStatus(vm, string.Format(Strings.EditorView_FileNoLongerExistsFormat, filePath));
                return;
            }

            try
            {
                using var stream = File.OpenRead(filePath);
                var skBitmap = SKBitmap.Decode(stream);
                if (skBitmap == null)
                {
                    EditorServices.ReportError(nameof(EditorView), $"Failed to decode image file '{filePath}'.");
                    ShowStartScreenStatus(vm, string.Format(Strings.EditorView_FailedToLoadImageFileFormat, filePath));
                    return;
                }

                vm.CloseModalCommand.Execute(null);
                LoadBitmapIntoEditor(vm, skBitmap, filePath);
                vm.ShowOpenImageNotification(filePath);
            }
            catch (Exception ex)
            {
                EditorServices.ReportError(nameof(EditorView), $"Failed to load image file '{filePath}'.", ex);
                ShowStartScreenStatus(vm, string.Format(Strings.EditorView_FailedToLoadImageFileFormat, filePath));
            }
        }

        private void LoadBitmapIntoEditor(MainViewModel vm, SKBitmap skBitmap, string? filePath)
        {
            // Clear annotation visuals
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            annotationCanvas?.Children.Clear();
            RefreshSpotlightOverlay();
            _selectionController.ClearSelection();

            // Load fresh image into core (clears history and annotations)
            _skipNextCoreImageChanged = true;
            _suppressNextHistoryDirtyMark = true;
            _editorCore.LoadImage(skBitmap);

            // Initialize canvas control
            _canvasControl?.Initialize(skBitmap.Width, skBitmap.Height);
            RenderCore();

            // Sync to VM
            try
            {
                _isSyncingToVM = true;
                vm.ImageFilePath = filePath;
                vm.IsDirty = false;
                vm.HasAnnotations = false;
                vm.UpdateCoreHistoryState(_editorCore.CanUndo, _editorCore.CanRedo);
                vm.UpdatePreviewImageOnly(skBitmap, syncSourceState: true);
            }
            finally
            {
                _isSyncingToVM = false;
            }

            // Track in recent files
            if (!string.IsNullOrEmpty(filePath))
            {
                vm.AddRecentImageFile(filePath);
            }
        }

        private void OnEmojiInsertionRequested(object? sender, EmojiSelectionRequest e)
        {
            try
            {
                InsertEmojiAnnotation(e.UnicodeSequence, e.DisplayName);
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), $"Failed to render emoji '{e.DisplayName}'.", ex);
            }
        }

        private async Task OnCopyImageRequested()
        {
            if (DataContext is not MainViewModel vm) return;
            if (vm.HasHostCopyHandler) return;

            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            IClipboard? clipboard = topLevel?.Clipboard;
            if (clipboard == null) return;

            using var skBitmap = GetSnapshot();
            if (skBitmap == null) return;

            using var image = SKImage.FromBitmap(skBitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            using var memStream = new System.IO.MemoryStream(encoded.ToArray());
            var bitmap = new Avalonia.Media.Imaging.Bitmap(memStream);

            DataTransfer data = new DataTransfer();
            DataTransferItem item = new DataTransferItem();
            item.SetBitmap(bitmap);
            data.Add(item);

            await clipboard.SetDataAsync(data);
        }

        private Task<string?> OnSaveRequested()
        {
            if (DataContext is not MainViewModel vm) return Task.FromResult<string?>(null);
            if (vm.HasHostSaveHandler) return Task.FromResult<string?>(null);

            if (!string.IsNullOrEmpty(vm.ImageFilePath))
            {
                SaveSnapshotToFile(vm.ImageFilePath!);
                vm.IsDirty = false;
                return Task.FromResult<string?>(vm.ImageFilePath);
            }

            return Task.FromResult<string?>(null);
        }

        private async Task<string?> OnSaveAsRequested()
        {
            if (DataContext is not MainViewModel vm) return null;
            if (vm.HasHostSaveAsHandler) return null;

            return await SaveAsAsync();
        }

        private async Task<string?> SaveAsAsync()
        {
            if (DataContext is not MainViewModel vm) return null;

            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider == null) return null;

            IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = Strings.EditorView_SaveImageAs,
                SuggestedFileName = !string.IsNullOrEmpty(vm.ImageFilePath)
                    ? System.IO.Path.GetFileName(vm.ImageFilePath)
                    : "image.png",
                FileTypeChoices =
                [
                    new FilePickerFileType("PNG") { Patterns = ["*.png"] },
                    new FilePickerFileType("JPEG") { Patterns = ["*.jpg", "*.jpeg"] },
                    new FilePickerFileType("WebP") { Patterns = ["*.webp"] }
                ]
            });

            if (file != null)
            {
                var path = file.Path.LocalPath;
                SaveSnapshotToFile(path);
                vm.ImageFilePath = path;
                vm.IsDirty = false;
                return path;
            }

            return null;
        }

        private void SaveSnapshotToFile(string path)
        {
            using var bitmap = GetSnapshot();
            if (bitmap == null) return;

            var ext = System.IO.Path.GetExtension(path)?.ToLowerInvariant();
            var format = ext switch
            {
                ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
                ".webp" => SKEncodedImageFormat.Webp,
                _ => SKEncodedImageFormat.Png
            };

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(format, format == SKEncodedImageFormat.Jpeg ? 95 : 100);
            using var stream = System.IO.File.OpenWrite(path);
            data.SaveTo(stream);
        }

        private void OnZoomToFitRequested(object? sender, EventArgs e)
        {
            _zoomController.ZoomToFit();
        }

        private void OnFlattenRequested(object? sender, EventArgs e)
        {
            var snapshot = GetSnapshot();
            if (snapshot == null) return;

            if (_editorCore.FlattenImage(snapshot))
            {
                // Clear annotation visuals from the UI canvas
                var canvas = this.FindControl<Canvas>("AnnotationCanvas");
                if (canvas != null)
                {
                    canvas.Children.Clear();
                    _selectionController.ClearSelection();
                }

                if (DataContext is MainViewModel vm)
                {
                    vm.HasAnnotations = false;
                }
            }
        }

        public void OpenContextMenu(Control target)
        {
            if (this.Resources["EditorContextMenu"] is ContextMenu menu)
            {
                menu.PlacementTarget = target;
                menu.Open(target);
            }
        }
    }
}
