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
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Core.Abstractions;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Core.Editor;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Emoji;
using System.Collections.ObjectModel;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public sealed class GradientPreset
        {
            public required string Name { get; init; }
            public required IBrush Brush { get; init; }

            public Color Color1 => GetGradientStopColor(0);
            public Color Color2 => GetGradientStopColor(1);

            private Color GetGradientStopColor(int index)
            {
                return Brush is LinearGradientBrush { GradientStops.Count: > 0 } linear
                    ? linear.GradientStops[Math.Min(index, linear.GradientStops.Count - 1)].Color
                    : Colors.Transparent;
            }
        }

        public enum EditorTaskResult
        {
            None,
            Continue,
            ContinueNoSave,
            Cancel
        }

        private readonly ImageEditorOptions _options;
        private readonly ObservableCollection<string> _recentImageFiles;
        public ImageEditorOptions Options => _options;
        public IAnnotationToolbarAdapter ToolbarAdapter { get; }
        public ReadOnlyObservableCollection<string> RecentImageFiles { get; }
        public bool HasRecentImageFiles => RecentImageFiles.Count > 0;

        private const string OutputRatioAuto = "Auto";

        [ObservableProperty]
        private bool _isDirty;

        private bool _isSyncingFromCore;
        private bool _zoomToFitOnNextImageLoad;

        [ObservableProperty]
        private string _windowTitle = Strings.MainViewModel_WindowTitle;

        [ObservableProperty]
        private bool _showFileMenu;

        [ObservableProperty]
        private bool _showOptionsButton = true;

        [ObservableProperty]
        private bool _showTaskButtons = true;

        [ObservableProperty]
        private bool _showBottomToolbar = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AreToolbarsHidden))]
        [NotifyPropertyChangedFor(nameof(ToggleToolbarsMenuHeader))]
        private bool _showToolbars = true;

        [ObservableProperty]
        private bool _showStartScreen = true;

        public bool AreToolbarsHidden => !ShowToolbars;

        public string ToggleToolbarsMenuHeader => ShowToolbars ? Strings.MainViewModel_HideToolbars : Strings.MainViewModel_ShowToolbars;

        // Events to signal View to perform canvas operations
        public event EventHandler? UndoRequested;
        public event EventHandler? RedoRequested;
        public event EventHandler? DeleteRequested;
        public event EventHandler? ClearAnnotationsRequested;
        public event EventHandler? FlattenRequested;
        public event EventHandler? DeselectRequested;
        public event EventHandler? CanvasFocusRequested;
        public event EventHandler? PasteRequested;
        public event EventHandler? DuplicateRequested;
        public event EventHandler? CutAnnotationRequested;
        public event EventHandler? CopyAnnotationRequested;
        public event EventHandler? ZoomToFitRequested;
        public event EventHandler? CloseRequested;
        public event EventHandler? ImageInsertionRequested;
        public event EventHandler<EmojiSelectionRequest>? EmojiInsertionRequested;

        // File menu events (Image Editor Mode)
        public event EventHandler? NewImageRequested;
        public event EventHandler? OpenImageRequested;
        public event EventHandler? StartScreenRequested;
        public event EventHandler? LoadFromClipboardRequested;
        public event EventHandler<string>? LoadFromUrlRequested;
        public event EventHandler<string>? LoadRecentFileRequested;

        [ObservableProperty]
        private bool _useContinueWorkflow;

        public string ContinueButtonTooltip => UseContinueWorkflow ? Strings.MainViewModel_ContinueEnter : Strings.MainViewModel_RunAfterCaptureTasksEnter;

        partial void OnUseContinueWorkflowChanged(bool value)
        {
            OnPropertyChanged(nameof(ContinueButtonTooltip));
        }

        [ObservableProperty]
        private EditorTaskResult _taskResult = EditorTaskResult.None;

        [RelayCommand]
        private void Continue()
        {
            TaskResult = EditorTaskResult.Continue;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose();
        }

        [RelayCommand]
        private void ToggleToolbars()
        {
            ShowToolbars = !ShowToolbars;
        }

        public void RequestClose(bool ignoreModal = false)
        {
            if (IsModalOpen && !ignoreModal)
            {
                return;
            }

            if (IsModalOpen && ignoreModal)
            {
                CloseModal();
            }

            TaskResult = EditorTaskResult.Cancel;

            if (Options.ShowExitConfirmation && IsDirty)
            {
                ShowConfirmationDialog();
            }
            else
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void CloseAfterTaskActionIfEnabled()
        {
            if (Options.AutoCloseEditorOnTask)
            {
                TaskResult = EditorTaskResult.Cancel;
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RequestCopyToClipboard()
        {
            _ = RequestCopyToClipboardAsync();
        }

        public Task RequestCopyToClipboardAsync()
        {
            return InvokeRequestedHandlersAsync(_copyRequested);
        }

        private void ShowConfirmationDialog()
        {
            if (IsModalOpen)
            {
                return;
            }

            var dialog = new ConfirmationDialogViewModel(
                onYes: () => _ = SaveAndCloseAfterConfirmationAsync(),
                onNo: () =>
                {
                    CloseModal();
                    CloseRequested?.Invoke(this, EventArgs.Empty);
                },
                onCancel: () =>
                {
                    CloseModal();
                }
            );

            ModalContent = dialog;
            IsModalOpen = true;
        }

        private async Task SaveAndCloseAfterConfirmationAsync()
        {
            await RequestSaveAsync();
            CloseModal();
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        // Export events
        private Func<Task>? _copyRequested;
        public event Func<Task>? CopyRequested
        {
            add { _copyRequested += value; CopyCommand.NotifyCanExecuteChanged(); }
            remove { _copyRequested -= value; CopyCommand.NotifyCanExecuteChanged(); }
        }
        public bool HasHostCopyHandler { get; set; }
        public bool CanCopy() => _copyRequested != null && HasPreviewImage;

        private Func<Task<string?>>? _saveRequested;
        public event Func<Task<string?>>? SaveRequested
        {
            add { _saveRequested += value; SaveCommand.NotifyCanExecuteChanged(); }
            remove { _saveRequested -= value; SaveCommand.NotifyCanExecuteChanged(); }
        }
        public bool HasHostSaveHandler { get; set; }
        public bool CanSave() => _saveRequested != null && HasPreviewImage;

        private Func<Task<string?>>? _saveAsRequested;
        public event Func<Task<string?>>? SaveAsRequested
        {
            add { _saveAsRequested += value; SaveAsCommand.NotifyCanExecuteChanged(); }
            remove { _saveAsRequested -= value; SaveAsCommand.NotifyCanExecuteChanged(); }
        }
        public bool HasHostSaveAsHandler { get; set; }
        public bool CanSaveAs() => _saveAsRequested != null && HasPreviewImage;

        private Action? _printRequested;
        public event Action? PrintRequested
        {
            add { _printRequested += value; PrintCommand.NotifyCanExecuteChanged(); }
            remove { _printRequested -= value; PrintCommand.NotifyCanExecuteChanged(); }
        }
        public bool CanPrint() => _printRequested != null && HasPreviewImage;

        private Action? _pinRequested;
        public event Action? PinRequested
        {
            add { _pinRequested += value; PinToScreenCommand.NotifyCanExecuteChanged(); }
            remove { _pinRequested -= value; PinToScreenCommand.NotifyCanExecuteChanged(); }
        }
        public bool CanPinToScreen() => _pinRequested != null && HasPreviewImage;

        private Action? _uploadRequested;
        public event Action? UploadRequested
        {
            add { _uploadRequested += value; UploadCommand.NotifyCanExecuteChanged(); }
            remove { _uploadRequested -= value; UploadCommand.NotifyCanExecuteChanged(); }
        }
        public bool CanUpload() => _uploadRequested != null && HasPreviewImage;

        private Bitmap? _previewImage;
        public Bitmap? PreviewImage
        {
            get => _previewImage;
            set
            {
                if (SetProperty(ref _previewImage, value))
                {
                    OnPreviewImageChanged(value);
                }
            }
        }

        private bool _hasPreviewImage;
        public bool HasPreviewImage
        {
            get => _hasPreviewImage;
            set
            {
                if (SetProperty(ref _hasPreviewImage, value))
                {
                    ToggleEffectsPanelCommand.NotifyCanExecuteChanged();
                    CopyCommand.NotifyCanExecuteChanged();
                    SaveCommand.NotifyCanExecuteChanged();
                    SaveAsCommand.NotifyCanExecuteChanged();
                    PrintCommand.NotifyCanExecuteChanged();
                    PinToScreenCommand.NotifyCanExecuteChanged();
                    UploadCommand.NotifyCanExecuteChanged();
                    ZoomInCommand.NotifyCanExecuteChanged();
                    ZoomOutCommand.NotifyCanExecuteChanged();
                    ResetZoomCommand.NotifyCanExecuteChanged();
                    ZoomToFitCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _hasSelectedAnnotation;
        /// <summary>
        /// Whether there is a currently selected annotation (shape). Used for Delete button CanExecute.
        /// </summary>
        public bool HasSelectedAnnotation
        {
            get => _hasSelectedAnnotation;
            set
            {
                if (SetProperty(ref _hasSelectedAnnotation, value))
                {
                    DeleteSelectedCommand.NotifyCanExecuteChanged();
                    DuplicateSelectedCommand.NotifyCanExecuteChanged();
                    BringToFrontCommand.NotifyCanExecuteChanged();
                    SendToBackCommand.NotifyCanExecuteChanged();
                    BringForwardCommand.NotifyCanExecuteChanged();
                    BringForwardCommand.NotifyCanExecuteChanged();
                    SendBackwardCommand.NotifyCanExecuteChanged();
                    CutAnnotationCommand.NotifyCanExecuteChanged();
                    CopyAnnotationCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _hasAnnotations;
        /// <summary>
        /// Whether there are any annotations on the canvas. Used for Clear All button CanExecute.
        /// </summary>
        public bool HasAnnotations
        {
            get => _hasAnnotations;
            set
            {
                if (SetProperty(ref _hasAnnotations, value))
                {
                    ClearAnnotationsCommand.NotifyCanExecuteChanged();
                    FlattenImageCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [ObservableProperty]
        private double _imageWidth;

        [ObservableProperty]
        private double _imageHeight;

        private Thickness _smartPaddingCropInsets;
        private bool _smartPaddingCacheValid;
        private bool _suppressSmartPaddingChangeHandling;

        private bool HasDetectedSmartPadding =>
            _smartPaddingCropInsets.Left > 0 ||
            _smartPaddingCropInsets.Top > 0 ||
            _smartPaddingCropInsets.Right > 0 ||
            _smartPaddingCropInsets.Bottom > 0;

        public bool CanUseBackgroundSmartPadding =>
            HasPreviewImage && (!_smartPaddingCacheValid || HasDetectedSmartPadding);

        public double SmartPaddingViewportWidth
        {
            get
            {
                double width = ImageWidth;

                if (IsSmartPaddingActive)
                {
                    width -= _smartPaddingCropInsets.Left + _smartPaddingCropInsets.Right;
                }

                return Math.Max(0, width);
            }
        }

        public double SmartPaddingViewportHeight
        {
            get
            {
                double height = ImageHeight;

                if (IsSmartPaddingActive)
                {
                    height -= _smartPaddingCropInsets.Top + _smartPaddingCropInsets.Bottom;
                }

                return Math.Max(0, height);
            }
        }

        public double SmartPaddingOffsetX
        {
            get
            {
                if (!IsSmartPaddingActive)
                {
                    return 0;
                }

                return -_smartPaddingCropInsets.Left;
            }
        }

        public double SmartPaddingOffsetY
        {
            get
            {
                if (!IsSmartPaddingActive)
                {
                    return 0;
                }

                return -_smartPaddingCropInsets.Top;
            }
        }

        private void NotifySmartPaddingStateChanged()
        {
            OnPropertyChanged(nameof(CanUseBackgroundSmartPadding));
            OnPropertyChanged(nameof(SmartPaddingColor));
            OnPropertyChanged(nameof(SmartPaddingThickness));
            OnPropertyChanged(nameof(SmartPaddingViewportWidth));
            OnPropertyChanged(nameof(SmartPaddingViewportHeight));
            OnPropertyChanged(nameof(SmartPaddingOffsetX));
            OnPropertyChanged(nameof(SmartPaddingOffsetY));
        }

        private void NotifySmartPaddingPaddingChanged()
        {
            OnPropertyChanged(nameof(SmartPaddingThickness));
        }

        private void RefreshSmartPaddingState(bool ensureCache = false, bool forceCacheRefresh = false)
        {
            if (ensureCache && AreBackgroundEffectsActive)
            {
                EnsureSmartPaddingCache(forceCacheRefresh);
            }

            UpdateCanvasProperties();
            NotifySmartPaddingStateChanged();
        }

        internal void SyncImageDimensions(double width, double height)
        {
            bool hasImage = width > 0 && height > 0;

            if (ImageWidth == width && ImageHeight == height && HasPreviewImage == hasImage)
            {
                return;
            }

            ImageWidth = width;
            ImageHeight = height;
            HasPreviewImage = hasImage;

            InvalidateSmartPaddingCache();

            if (hasImage)
            {
                RefreshSmartPaddingState(ensureCache: AreBackgroundEffectsActive, forceCacheRefresh: AreBackgroundEffectsActive);

                var fileName = GetFileNameFromPath(ImageFilePath);
                WindowTitle = BuildWindowTitle(ImageWidth, ImageHeight, fileName);
            }
            else
            {
                NotifySmartPaddingStateChanged();
            }
        }

        private void OnPreviewImageChanged(Bitmap? value)
        {
            if (value != null)
            {
                var pixelSize = value.PixelSize;
                SyncImageDimensions(pixelSize.Width, pixelSize.Height);

                if (!_isSyncingFromCore && !_isApplyingSmartPadding)
                {
                    IsDirty = true;
                }
            }
            else
            {
                SyncImageDimensions(0, 0);
            }
        }

        [ObservableProperty]
        private double _backgroundMargin = 30;

        [ObservableProperty]
        private double _backgroundPadding = 30;

        [ObservableProperty]
        private bool _backgroundSmartPadding = true;

        private bool IsSmartPaddingActive =>
            BackgroundSmartPadding && AreBackgroundEffectsActive && HasDetectedSmartPadding;

        /// <summary>
        /// ISSUE-022 fix: Recursion guard flag for smart padding event chain.
        /// Prevents infinite loop: BackgroundSmartPadding property change → ApplySmartPaddingCrop →
        /// UpdatePreview → PreviewImage changed → ApplySmartPaddingCrop (again).
        /// Set to true during ApplySmartPaddingCrop execution to break the cycle.
        /// </summary>
        private bool _isApplyingSmartPadding = false;

        /// <summary>
        /// Public accessor for _isApplyingSmartPadding. Used by EditorView to skip
        /// LoadImageFromViewModel during smart padding operations, preventing history reset.
        /// </summary>
        public bool IsSmartPaddingInProgress => _isApplyingSmartPadding;

        public Thickness SmartPaddingThickness => AreBackgroundEffectsActive ? new Thickness(BackgroundPadding) : new Thickness(0);

        public IBrush SmartPaddingColor
        {
            get
            {
                if (!AreBackgroundEffectsActive || PreviewImage == null)
                {
                    return Brushes.Transparent;
                }

                try
                {
                    var topLeftColor = SamplePixelColor(PreviewImage, 0, 0);
                    return new SolidColorBrush(topLeftColor);
                }
                catch (Exception ex)
                {
                    EditorServices.ReportWarning(nameof(MainViewModel), "Failed to sample smart padding color. Falling back to transparent.", ex);
                    return Brushes.Transparent;
                }
            }
        }

        [ObservableProperty]
        private double _backgroundRoundedCorner = 15;

        [ObservableProperty]
        private double _backgroundShadowRadius = 30;

        private const double MinZoom = 0.25;
        private const double MaxZoom = 4.0;
        private const double ZoomStep = 0.1;

        [ObservableProperty]
        private double _zoom = 1.0;

        // DPI scale reported by the host window (1.0 at 100 %, 1.5 at 150 %, etc.).
        // Set by EditorView when the window is loaded or moved to a different monitor.
        private double _dpiScale = 1.0;
        public double DpiScale
        {
            get => _dpiScale;
            set
            {
                double safe = Math.Max(0.01, value);
                if (Math.Abs(_dpiScale - safe) <= 0.0001) return;
                _dpiScale = safe;
                OnPropertyChanged(nameof(DpiScale));
                OnPropertyChanged(nameof(EffectiveZoom));
            }
        }

        /// <summary>
        /// The scale factor applied to the LayoutTransformControl.
        /// Combines the user-selected zoom with an inverse DPI compensation so that
        /// 100 % zoom always shows one image pixel per physical screen pixel,
        /// regardless of the Windows display scaling setting.
        /// </summary>
        public double EffectiveZoom => Zoom / _dpiScale;

        [ObservableProperty]
        private string _imageDimensions = Strings.MainViewModel_NoImage;

        [ObservableProperty]
        private bool _isPngFormat = true;

        [ObservableProperty]
        private string _appVersion;

        [ObservableProperty]
        private bool _isSettingsPanelOpen;

        [ObservableProperty]
        private int _numberCounter = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
        private bool _canUndo;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
        private bool _canRedo;

        private bool _isCoreUndoAvailable;
        private bool _isCoreRedoAvailable;
        private EditorCore? _editorCore;

        private bool _isPreviewingEffect;
        public bool AreBackgroundEffectsActive => IsSettingsPanelOpen && !_isPreviewingEffect;
        public IBrush EffectiveCanvasBackground => AreBackgroundEffectsActive ? CanvasBackground : Brushes.Transparent;

        partial void OnIsSettingsPanelOpenChanged(bool value)
        {
            // Toggle background effects visibility
            OnPropertyChanged(nameof(AreBackgroundEffectsActive));
            OnPropertyChanged(nameof(EffectiveCanvasBackground));
            RefreshSmartPaddingState(ensureCache: value && _originalSourceImage != null, forceCacheRefresh: value);
            RefreshToolbarItemActiveStates();
        }

        public void UpdateCoreHistoryState(bool canUndo, bool canRedo)
        {
            _isCoreUndoAvailable = canUndo;
            _isCoreRedoAvailable = canRedo;
            UpdateUndoRedoProperties();
        }

        private void UpdateUndoRedoProperties()
        {
            CanUndo = _isCoreUndoAvailable;
            CanRedo = _isCoreRedoAvailable;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PasteCommand))]
        private bool _canPaste;

        [RelayCommand(CanExecute = nameof(CanPaste))]
        private void Paste()
        {
            PasteRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void CutAnnotation()
        {
            CutAnnotationRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void CopyAnnotation()
        {
            CopyAnnotationRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void DuplicateSelected()
        {
            DuplicateRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void BringToFront()
        {
            _editorCore?.BringToFront();
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void SendToBack()
        {
            _editorCore?.SendToBack();
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void BringForward()
        {
            _editorCore?.BringForward();
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void SendBackward()
        {
            _editorCore?.SendBackward();
        }

        [ObservableProperty]
        private string _selectedOutputRatio = OutputRatioAuto;

        [ObservableProperty]
        private double? _targetOutputAspectRatio;

        [RelayCommand]
        private void ResetNumberCounter()
        {
            NumberCounter = StepStartNumber;
        }

        public void RecalculateNumberCounter(IEnumerable<Annotation> annotations)
        {
            int max = 0;
            if (annotations != null)
            {
                foreach (var ann in annotations)
                {
                    if (ann is NumberAnnotation num)
                    {
                        if (num.Number > max) max = num.Number;
                    }
                }
            }
            NumberCounter = Math.Max(max + 1, StepStartNumber);
        }

        [RelayCommand]
        private void SetOutputRatio(string ratioKey)
        {
            SelectedOutputRatio = string.IsNullOrWhiteSpace(ratioKey) ? OutputRatioAuto : ratioKey;
        }

        // Effects Panel Properties
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEffectBrowserVisible))]
        [NotifyPropertyChangedFor(nameof(IsRightEffectsSidebarVisible))]
        private bool _isEffectsPanelOpen;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEffectBrowserVisible))]
        [NotifyPropertyChangedFor(nameof(IsRightEffectsSidebarVisible))]
        private object? _effectsPanelContent;

        public bool IsEffectBrowserVisible => EffectsPanelContent == null && IsEffectsPanelOpen;

        public bool IsRightEffectsSidebarVisible => EffectsPanelContent != null && IsEffectsPanelOpen;

        private void CancelPendingEffectPreviewIfAny()
        {
            if (_preEffectImage != null)
            {
                // Ensure dialog preview is reverted when switching away from the dialog host.
                CancelEffectPreview();
            }
        }

        [RelayCommand]
        private void CloseEffectsPanel()
        {
            CancelPendingEffectPreviewIfAny();
            IsEffectsPanelOpen = false;
            EffectsPanelContent = null;
            OnPropertyChanged(nameof(IsEffectBrowserVisible));
            OnPropertyChanged(nameof(IsRightEffectsSidebarVisible));
        }

        [RelayCommand(CanExecute = nameof(HasPreviewImage))]
        private void ToggleEffectsPanel()
        {
            if (IsEffectsPanelOpen && EffectsPanelContent == null)
            {
                IsEffectsPanelOpen = false;
                OnPropertyChanged(nameof(IsEffectBrowserVisible));
                OnPropertyChanged(nameof(IsRightEffectsSidebarVisible));
            }
            else if (IsEffectsPanelOpen && EffectsPanelContent != null)
            {
                // Switching from dialog sidebar back to browser should cancel live preview.
                CancelPendingEffectPreviewIfAny();

                EffectsPanelContent = null;
                IsEffectsPanelOpen = true;
                OnPropertyChanged(nameof(IsEffectBrowserVisible));
                OnPropertyChanged(nameof(IsRightEffectsSidebarVisible));
            }
            else
            {
                EffectsPanelContent = null;
                IsEffectsPanelOpen = true;
                OnPropertyChanged(nameof(IsEffectBrowserVisible));
                OnPropertyChanged(nameof(IsRightEffectsSidebarVisible));
            }
        }

        // Modal Overlay Properties
        [ObservableProperty]
        private bool _isModalOpen;

        [ObservableProperty]
        private object? _modalContent;

        [RelayCommand]
        private void CloseModal()
        {
            IsModalOpen = false;
            ModalContent = null;
        }

        [ObservableProperty]
        private IBrush _canvasBackground;

        public ObservableCollection<GradientPreset> GradientPresets { get; }

        [ObservableProperty]
        private double _canvasCornerRadius = 0;

        [ObservableProperty]
        private Thickness _canvasPadding;

        [ObservableProperty]
        private BoxShadows _canvasShadow;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? _imageFilePath;

        public static MainViewModel Current { get; private set; } = null!;

        public MainViewModel(ImageEditorOptions? options = null)
        {
            _options = options ?? new ImageEditorOptions();
            _recentImageFiles = new ObservableCollection<string>(_options.RecentImageFiles);
            _recentImageFiles.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasRecentImageFiles));
            RecentImageFiles = new ReadOnlyObservableCollection<string>(_recentImageFiles);
            _activeTool = GetInitialAnnotationTool();

            InitializeToolbarCustomization();
            ToolbarAdapter = new EditorToolbarAdapter(this);
            Current = this;
            GradientPresets = BuildGradientPresets();
            BackgroundModeOptions = BuildBackgroundModeOptions();
            InitializeBackgroundSettingsFromOptions();
            _canvasBackground = Brushes.Transparent;
            EditorServices.StartDesktopWallpaperPrewarm(nameof(MainViewModel));

            // Initialize values from options
            _textColor = $"#{_options.TextTextColor.A:X2}{_options.TextTextColor.R:X2}{_options.TextTextColor.G:X2}{_options.TextTextColor.B:X2}";
            _fillColor = $"#{_options.FillColor.A:X2}{_options.FillColor.R:X2}{_options.FillColor.G:X2}{_options.FillColor.B:X2}";
            _selectedColor = $"#{_options.BorderColor.A:X2}{_options.BorderColor.R:X2}{_options.BorderColor.G:X2}{_options.BorderColor.B:X2}";
            _strokeWidth = _options.Thickness;
            _cornerRadius = _options.CornerRadius;
            _fontSize = _options.TextFontSize;
            _selectedFontFamily = NormalizeFontFamily(_options.TextFontFamily);
            _selectedBorderStyle = NormalizeBorderStyle(_options.BorderStyle);
            _selectedArrowStyle = NormalizeArrowStyle(_options.ArrowStyle);
            _shadowEnabled = _options.Shadow;
            _shadowColor = _options.ShadowColorHex;
            _shadowBlurRadius = _options.ShadowBlurRadius;
            _shadowOpacity = _options.ShadowOpacity;
            _shadowOffsetX = _options.ShadowOffsetX;
            _shadowOffsetY = _options.ShadowOffsetY;
            _textBold = _options.TextBold;
            _textItalic = _options.TextItalic;

            // Get version from assembly
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _appVersion = version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "v1.0.0";

            _isLoadingOptions = true;
            try
            {
                OnPropertyChanged(nameof(EffectStrengthMaximum));
                LoadOptionsForTool(_activeTool);
                UpdateToolOptionsVisibility();
            }
            finally
            {
                _isLoadingOptions = false;
            }

            ApplySelectedBackgroundMode();
            UpdateCanvasProperties();
        }

        public void AddRecentImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            _options.AddRecentImageFile(filePath);
            SyncRecentImageFiles();
        }

        public void RemoveRecentImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (_options.RecentImageFiles.Remove(filePath))
            {
                SyncRecentImageFiles();
            }
        }

        private void SyncRecentImageFiles()
        {
            _recentImageFiles.Clear();

            foreach (string filePath in _options.RecentImageFiles)
            {
                _recentImageFiles.Add(filePath);
            }
        }

        private static string BuildWindowTitle(double width, double height, string? fileName)
        {
            var sb = new System.Text.StringBuilder(Strings.MainViewModel_WindowTitle);

            if (width > 0 && height > 0)
            {
                sb.Append($" - {width}x{height}");
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                sb.Append($" - {fileName}");
            }

            return sb.ToString();
        }

        private static string? GetFileNameFromPath(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            try
            {
                return Path.GetFileName(filePath);
            }
            catch
            {
                return null;
            }
        }

        partial void OnImageFilePathChanged(string? value)
        {
            // Refresh window title when file path changes (after Save / Save As)
            var fileName = GetFileNameFromPath(value);
            WindowTitle = BuildWindowTitle(ImageWidth, ImageHeight, fileName);
        }

        public void AttachEditorCore(EditorCore editorCore)
        {
            _editorCore = editorCore;
            UpdateCoreHistoryState(editorCore.CanUndo, editorCore.CanRedo);
        }

        public void RequestZoomToFitOnNextImageLoad()
        {
            _zoomToFitOnNextImageLoad = Options.ZoomToFitOnOpen;
        }

        public bool ConsumeZoomToFitOnNextImageLoad()
        {
            if (!_zoomToFitOnNextImageLoad)
            {
                return false;
            }

            _zoomToFitOnNextImageLoad = false;
            return true;
        }

        partial void OnSelectedOutputRatioChanged(string value)
        {
            TargetOutputAspectRatio = ParseAspectRatio(value);
            UpdateCanvasProperties();
        }

        partial void OnBackgroundMarginChanged(double value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundMargin = value;
            UpdateCanvasProperties();
        }

        partial void OnBackgroundPaddingChanged(double value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundPadding = value;
            UpdateCanvasProperties();
            NotifySmartPaddingPaddingChanged();
        }

        partial void OnBackgroundSmartPaddingChanged(bool value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundSmartPadding = value;

            if (_suppressSmartPaddingChangeHandling)
            {
                return;
            }

            RefreshSmartPaddingState(ensureCache: value, forceCacheRefresh: value);
        }

        partial void OnBackgroundRoundedCornerChanged(double value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundRoundedCorner = value;
            UpdateCanvasProperties();
        }

        partial void OnBackgroundShadowRadiusChanged(double value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundShadowRadius = value;
            UpdateCanvasProperties();
        }

        partial void OnZoomChanged(double value)
        {
            var clamped = Math.Clamp(value, MinZoom, MaxZoom);
            if (Math.Abs(clamped - value) > 0.0001)
            {
                Zoom = clamped;
                return;
            }

            OnPropertyChanged(nameof(EffectiveZoom));
        }

        // Static color palette for annotation toolbar
        public static string[] ColorPalette => new[]
        {
            "#EF4444", "#F97316", "#EAB308", "#22C55E",
            "#0EA5E9", "#6366F1", "#A855F7", "#EC4899",
            "#FFFFFF", "#000000", "#64748B", "#1E293B"
        };

        // Static stroke widths
        public static int[] StrokeWidths => new[] { 2, 4, 6, 8, 10 };

        public event EventHandler<EditorTool>? ToolSelectionRequested;

        [RelayCommand]
        private void SelectTool(EditorTool tool)
        {
            ToolSelectionRequested?.Invoke(this, tool);

            if (tool == EditorTool.Image)
            {
                DeselectRequested?.Invoke(this, EventArgs.Empty);
                RequestImageInsertion();
                return;
            }

            if (tool == EditorTool.Emoji)
            {
                DeselectRequested?.Invoke(this, EventArgs.Empty);
                ShowEmojiPickerDialog();
                return;
            }

            // Re-selecting the active crop/cut-out tool should not cancel the current operation
            if (ActiveTool == tool && tool is EditorTool.Crop or EditorTool.CutOut)
            {
                CanvasFocusRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            DeselectRequested?.Invoke(this, EventArgs.Empty);

            if (tool is EditorTool.Crop or EditorTool.CutOut)
            {
                IsSettingsPanelOpen = false;
            }

            ActiveTool = tool;
        }

        private void RequestImageInsertion()
        {
            if (IsModalOpen)
            {
                return;
            }

            ImageInsertionRequested?.Invoke(this, EventArgs.Empty);
        }

        internal void ShowEmojiPickerDialog(Action<EmojiCatalogEntry>? onSelect = null)
        {
            if (IsModalOpen)
            {
                return;
            }

            var dialog = new EmojiPickerDialogViewModel(
                onSelect: entry =>
                {
                    CloseModal();

                    if (onSelect != null)
                    {
                        onSelect(entry);
                    }
                    else
                    {
                        EmojiInsertionRequested?.Invoke(this, new EmojiSelectionRequest(entry.Unicode, entry.DisplayName));
                    }
                },
                onCancel: CloseModal);

            ModalContent = dialog;
            IsModalOpen = true;
        }

        [RelayCommand]
        private void SetColor(string color)
        {
            SelectedColor = color;
        }

        [RelayCommand]
        private void SetStrokeWidth(int width)
        {
            StrokeWidth = width;
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void Undo()
        {
            UndoRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(CanRedo))]
        private void Redo()
        {
            RedoRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(HasSelectedAnnotation))]
        private void DeleteSelected()
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(HasAnnotations))]
        private void ClearAnnotations()
        {
            ClearAnnotationsRequested?.Invoke(this, EventArgs.Empty);
            ResetNumberCounter();
        }

        [RelayCommand(CanExecute = nameof(HasAnnotations))]
        private void FlattenImage()
        {
            FlattenRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void ToggleSettingsPanel()
        {
            IsSettingsPanelOpen = !IsSettingsPanelOpen;
        }

        private bool CanZoom() => HasPreviewImage;

        [RelayCommand(CanExecute = nameof(CanZoom))]
        private void ZoomIn()
        {
            Zoom = Math.Clamp(Math.Round((Zoom + ZoomStep) * 100) / 100, MinZoom, MaxZoom);
        }

        [RelayCommand(CanExecute = nameof(CanZoom))]
        private void ZoomOut()
        {
            Zoom = Math.Clamp(Math.Round((Zoom - ZoomStep) * 100) / 100, MinZoom, MaxZoom);
        }

        [RelayCommand(CanExecute = nameof(CanZoom))]
        private void ResetZoom()
        {
            Zoom = 1.0;
        }

        [RelayCommand(CanExecute = nameof(CanZoom))]
        private void ZoomToFit()
        {
            ZoomToFitRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Clear()
        {
            PreviewImage = null;

            // ISSUE-030 fix: Dispose bitmaps before clearing
            _currentSourceImage?.Dispose();
            _currentSourceImage = null;

            _originalSourceImage?.Dispose();
            _originalSourceImage = null;

            // HasPreviewImage = false; // Handled by OnPreviewImageChanged
            ImageDimensions = Strings.MainViewModel_NoImage;
            ResetNumberCounter();

            // Clear annotations as well
            ClearAnnotationsRequested?.Invoke(this, EventArgs.Empty);
        }

        private Task<string?> RequestSaveAsync()
        {
            return InvokeRequestedHandlersAsync(_saveRequested);
        }

        private Task<string?> RequestSaveAsAsync()
        {
            return InvokeRequestedHandlersAsync(_saveAsRequested);
        }

        [RelayCommand(CanExecute = nameof(CanCopy))]
        private async Task Copy()
        {
            await RequestCopyToClipboardAsync();
            ShowTaskActionNotification(Strings.MainViewModel_ImageCopiedToClipboard, EditorIcons.ActionCopy);
            CloseAfterTaskActionIfEnabled();
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            string? savedPath = await RequestSaveAsync();
            ShowSaveNotification(savedPath, EditorIcons.ActionSave);
            CloseAfterTaskActionIfEnabled();
        }

        [RelayCommand(CanExecute = nameof(CanSaveAs))]
        private async Task SaveAs()
        {
            string? savedPath = await RequestSaveAsAsync();
            ShowSaveNotification(savedPath, EditorIcons.ActionSaveAs);
            CloseAfterTaskActionIfEnabled();
        }

        [RelayCommand(CanExecute = nameof(CanPrint))]
        private void Print()
        {
            _printRequested?.Invoke();
            ShowTaskActionNotification(Strings.MainViewModel_ImagePrinted, EditorIcons.ActionPrint);
            CloseAfterTaskActionIfEnabled();
        }

        [RelayCommand(CanExecute = nameof(CanPinToScreen))]
        private void PinToScreen()
        {
            _pinRequested?.Invoke();
            ShowTaskActionNotification(Strings.MainViewModel_ImagePinnedToScreen, EditorIcons.ActionPinToScreen);
            CloseAfterTaskActionIfEnabled();
        }

        [RelayCommand(CanExecute = nameof(CanUpload))]
        private async Task Upload()
        {
            _uploadRequested?.Invoke();
            ShowTaskActionNotification(Strings.MainViewModel_ImageIsUploading, EditorIcons.ActionUpload);
            CloseAfterTaskActionIfEnabled();
        }

        [RelayCommand]
        private void NewImage()
        {
            NewImageRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void OpenImage()
        {
            OpenImageRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RequestStartScreen()
        {
            StartScreenRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RequestLoadFromClipboard()
        {
            LoadFromClipboardRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RequestLoadFromUrl(string url)
        {
            LoadFromUrlRequested?.Invoke(this, url);
        }

        public void RequestLoadRecentFile(string filePath)
        {
            LoadRecentFileRequested?.Invoke(this, filePath);
        }

        [RelayCommand]
        private void OpenRecentImage(string? filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                RequestLoadRecentFile(filePath);
            }
        }

        [RelayCommand]
        private void ExitEditor()
        {
            RequestClose();
        }
    }
}
