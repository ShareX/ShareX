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
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.PhotoEditor.Integration;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI;

public enum ScreenMode
{
    Standard,
    FullWithMenu,
    Full
}

/// <summary>The Photo editor main window. Implements <see cref="IEditorHost"/> for tools and owns documents, panels and commands.</summary>
public sealed partial class PhotoEditorWindow : Window, IEditorHost
{
    private readonly List<DocumentView> _views = [];
    private DocumentView? _view;
    private SKColor _foreground = SKColors.Black;
    private SKColor _background = SKColors.White;
    private readonly Border _optionsHost = new() { Padding = new Thickness(6, 3), Height = 38 };
    private readonly StackPanel _tabs = new() { Orientation = Orientation.Horizontal };
    private readonly TextBlock _status = new() { VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis, FontSize = 11, Classes = { "dim" } };
    private readonly TextBlock _docInfo = new() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0), FontSize = 11, Classes = { "dim" } };
    private readonly TextBlock _cursorInfo = new() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0), MinWidth = 110, FontSize = 11, Classes = { "dim" } };
    private readonly TextBox _zoomBox = new() { Width = 54, MinHeight = 18, Height = 18, Padding = new Thickness(4, 0), FontSize = 11, VerticalContentAlignment = VerticalAlignment.Center, BorderThickness = new Thickness(0), Background = Brushes.Transparent };
    private readonly Canvas _overlayLayer = new();
    private readonly Border _busy;
    private readonly TextBlock _busyText = new() { Foreground = Brushes.White, FontSize = 14 };
    private readonly ProgressBar _busyProgress = new() { Width = 260, Minimum = 0, Maximum = 1 };
    private readonly ToolbarControl _toolbar;
    private readonly PanelDock _dock;
    private readonly Grid _layout;
    private readonly ColumnDefinition _dockColumn;
    private readonly GridSplitter _dockSplitter;
    private readonly Border _toolbarHost;
    private readonly Border _tabHost;
    private readonly RulerControl _hRuler;
    private readonly RulerControl _vRuler;
    private readonly Border _rulerCorner;
    private readonly StartScreen _startScreen;
    private readonly DispatcherTimer _statusTimer;
    private TextBox? _textEditor;
    private TextLayer? _editingText;
    private TextLayer? _editingOriginal;
    private bool _editingIsNew;
    private bool _editingWasVisible;
    private ToolBase? _springReturn;
    private ScreenMode _screenMode;
    private bool _panelsHidden;
    private bool _closingConfirmed;
    private double _tabZoom;
    private int _busyCount;

    public PhotoEditorWindow(PhotoEditorServices services)
    {
        Services = services;
        Settings = EditorSettings.Load(services.SettingsFilePath);
        _foreground = EditorSettings.ParseColor(Settings.Foreground, SKColors.Black);
        _background = EditorSettings.ParseColor(Settings.Background, SKColors.White);
        _gradient = GradientDefinition.CreateDefault(_foreground, _background);
        Patterns = PatternDefinition.CreateDefaults();
        BrushPresets = BrushPreset.CreateDefaults();
        if (Settings.Swatches.Count == 0) Settings.Swatches.AddRange(SwatchesPanel.DefaultSwatches);

        Title = Strings.PhotoEditorWindow_WindowTitle;
        Width = 1500;
        Height = 920;
        MinWidth = 900;
        MinHeight = 600;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ApplySavedWindowState();
        ShareX.PhotoEditor.UI.Theme.PhotoEditorTheme.Apply(this);

        Tools = new ToolManager(this);
        Tools.ApplyLayout(Settings);
        Tools.ActiveChanged += OnToolChanged;
        CommandRegistry.EnsureInitialized();

        Canvas = new CanvasView(this);
        _hRuler = new RulerControl(this, true);
        _vRuler = new RulerControl(this, false);
        _toolbar = new ToolbarControl(this);
        _dock = new PanelDock(this,
        [
            new LayersPanel(this), new HistoryPanel(this), new ColorPanel(this), new SwatchesPanel(this), new PropertiesPanel(this),
            new AdjustmentsPanel(this), new ChannelsPanel(this), new PathsPanel(this), new BrushesPanel(this), new NavigatorPanel(this), new InfoPanel(this)
        ]);
        _titleBar = CreateTitleBar();

        // Canvas area with rulers.
        Grid canvasGrid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), RowDefinitions = new RowDefinitions("Auto,*") };
        _rulerCorner = new Border { Width = 18, Height = 18, Background = new SolidColorBrush(Color.FromRgb(0x2C, 0x2C, 0x2C)) };
        Grid.SetColumn(_hRuler, 1);
        Grid.SetRow(_vRuler, 1);
        Grid canvasStack = new();
        canvasStack.Children.Add(Canvas);
        canvasStack.Children.Add(_overlayLayer);
        Grid.SetRow(canvasStack, 1);
        Grid.SetColumn(canvasStack, 1);
        canvasGrid.Children.Add(_rulerCorner);
        canvasGrid.Children.Add(_hRuler);
        canvasGrid.Children.Add(_vRuler);
        canvasGrid.Children.Add(canvasStack);
        // Starts hidden so the first ApplyVisibility refreshes the recent files.
        _startScreen = new StartScreen(this) { IsVisible = false };
        Grid.SetRowSpan(_startScreen, 2);
        Grid.SetColumnSpan(_startScreen, 2);
        canvasGrid.Children.Add(_startScreen);

        _tabHost = new Border { Child = new ScrollViewer { Content = _tabs, HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled }, Height = 28, Background = new SolidColorBrush(Color.FromRgb(0x26, 0x26, 0x26)) };
        DockPanel center = new();
        DockPanel.SetDock(_tabHost, Dock.Top);
        center.Children.Add(_tabHost);
        center.Children.Add(canvasGrid);

        _toolbarHost = new Border { Child = _toolbar, Margin = new Thickness(0, 0, 2, 0) };
        _toolbarHost.Bind(Border.BackgroundProperty, _toolbarHost.GetResourceObservable("ShareX.Brush.Background.Toolbar"));

        _dockColumn = new ColumnDefinition(new GridLength(Math.Clamp(Settings.RightDockWidth, 220, 600))) { MinWidth = 200 };
        _layout = new Grid { ColumnDefinitions = new ColumnDefinitions { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto), _dockColumn } };
        _dockSplitter = new GridSplitter { Width = 2, ResizeDirection = GridResizeDirection.Columns, Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)) };
        Grid.SetColumn(center, 1);
        Grid.SetColumn(_dockSplitter, 2);
        Grid.SetColumn(_dock, 3);
        _layout.Children.Add(_toolbarHost);
        _layout.Children.Add(center);
        _layout.Children.Add(_dockSplitter);
        _layout.Children.Add(_dock);

        _optionsHost.Bind(Border.BackgroundProperty, _optionsHost.GetResourceObservable("ShareX.Brush.Background.Toolbar"));
        _optionsHost.Margin = new Thickness(0, 0, 0, 2);

        _zoomBox.KeyDown += (_, e) =>
        {
            if (e.Key != Key.Enter || View == null) return;
            if (double.TryParse(_zoomBox.Text?.Replace("%", string.Empty).Trim(), out double percent) && percent > 0)
            {
                View.ZoomAt(percent / 100, new Point(View.Viewport.Width / 2, View.Viewport.Height / 2));
                InvalidateOverlay();
            }
            Canvas.Focus();
        };
        DockPanel statusBar = new() { Height = 22, Margin = new Thickness(4, 0) };
        StackPanel left = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        left.Children.Add(_zoomBox);
        left.Children.Add(_docInfo);
        left.Children.Add(_cursorInfo);
        DockPanel.SetDock(left, Dock.Left);
        statusBar.Children.Add(left);
        statusBar.Children.Add(_status);
        Border statusHost = new() { Child = statusBar, Margin = new Thickness(0, 2, 0, 0) };
        statusHost.Bind(Border.BackgroundProperty, statusHost.GetResourceObservable("ShareX.Brush.Background.Toolbar"));

        DockPanel root = new() { Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)) };
        DockPanel.SetDock(_titleBar, Dock.Top);
        DockPanel.SetDock(_optionsHost, Dock.Top);
        DockPanel.SetDock(statusHost, Dock.Bottom);
        root.Children.Add(_titleBar);
        root.Children.Add(_optionsHost);
        root.Children.Add(statusHost);
        root.Children.Add(_layout);

        StackPanel busyContent = new() { Spacing = 10, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        busyContent.Children.Add(_busyText);
        busyContent.Children.Add(_busyProgress);
        _busy = new Border { Background = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0)), Child = busyContent, IsVisible = false };
        Grid shell = new();
        shell.Children.Add(root);
        shell.Children.Add(_busy);
        Content = shell;

        AddHandler(KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
        AddHandler(KeyUpEvent, OnKeyUpTunnel, RoutingStrategies.Tunnel);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        DragDrop.SetAllowDrop(this, true);

        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _statusTimer.Tick += (_, _) =>
        {
            _statusTimer.Stop();
            RefreshInfo();
        };

        ApplyVisibility();
        RefreshOptions();
        RefreshTabs();
        RefreshStatus();
        Closing += OnClosing;
        Closed += (_, _) =>
        {
            _statusTimer.Stop();
            Canvas.StopTimers();
        };
        Opened += (_, _) => _windowPlacementApplied = true;
        Resized += (_, _) => QueueWindowPlacementSave();
        PositionChanged += (_, _) => QueueWindowPlacementSave();
    }

    public PhotoEditorServices Services { get; }

    public EditorSettings Settings { get; }

    public CanvasView Canvas { get; }

    public PanelDock Panels => _dock;

    public ToolManager Tools { get; }

    public Window Window => this;

    public IReadOnlyList<DocumentView> Views => _views;

    public DocumentView? View => _view;

    public PhotoDocument? Document => _view?.Document;

    private GradientDefinition _gradient;

    /// <summary>The active gradient. Foreground presets always reflect the current foreground and background colors.</summary>
    public GradientDefinition Gradient
    {
        get => _gradient.ResolveColors(_foreground, _background);
        set => _gradient = value;
    }

    public PatternDefinition? Pattern { get; set; }

    public List<PatternDefinition> Patterns { get; }

    public List<BrushPreset> BrushPresets { get; }

    public KeyModifiers LastModifiers { get; private set; }

    /// <summary>The last filter applied, for Filter &gt; Last Filter.</summary>
    public Action? RepeatLastFilter { get; set; }

    public string? LastFilterName { get; set; }

    public new SKColor Foreground
    {
        get => _foreground;
        set
        {
            _foreground = value;
            Settings.Foreground = "#" + ColorMath.ToHex(value);
            _toolbar.RefreshColors();
            _dock.Get("color")?.Refresh();
        }
    }

    public new SKColor Background
    {
        get => _background;
        set
        {
            _background = value;
            Settings.Background = "#" + ColorMath.ToHex(value);
            _toolbar.RefreshColors();
            _dock.Get("color")?.Refresh();
        }
    }

    public bool IsTextEditing => _editingText != null;

    // ------------------------------------------------------------------ IEditorHost

    public void InvalidateCanvas(SKRectI? area = null)
    {
        if (_view != null)
        {
            if (area is SKRectI r)
            {
                r.Inflate(2, 2);
                _view.MarkDirty(r);
            }
            else
            {
                _view.MarkDirty(null);
            }
        }
        Canvas.InvalidateVisual();
        _hRuler.InvalidateVisual();
        _vRuler.InvalidateVisual();
        if (area == null) Canvas.InvalidateOutline();
        _statusTimer.Stop();
        _statusTimer.Start();
    }

    public void InvalidateOverlay()
    {
        Canvas.InvalidateVisual();
        _hRuler.InvalidateVisual();
        _vRuler.InvalidateVisual();
        PositionTextEditor();
        (_dock.Get("navigator") as NavigatorPanel)?.UpdateRect();
        RefreshStatus();
    }

    public void RefreshPanels()
    {
        _dock.RefreshVisible();
        _toolbar.RefreshColors();
        RefreshTabs();
        UpdateTitle();
        RefreshStatus();
        Canvas.InvalidateOutline();
        Canvas.InvalidateVisual();
    }

    public void RefreshPanelsExcept(EditorPanel panel)
    {
        _dock.RefreshVisible(panel);
        RefreshTabs();
        UpdateTitle();
    }

    public void RefreshOptions()
    {
        OptionsBuilder builder = new(this);
        StackPanel left = new() { Orientation = Orientation.Horizontal, Spacing = 4, VerticalAlignment = VerticalAlignment.Center };
        // Tool preset button: the active tool's icon with a dropdown listing the other tools of its group.
        StackPanel presetContent = new() { Orientation = Orientation.Horizontal, Spacing = 3 };
        presetContent.Children.Add(new IconView(IconMap.ForTool(Tools.Active.Id), 20));
        presetContent.Children.Add(new IconView(PhotoEditorIcons.ChevronDown, 10));
        Button preset = new() { Classes = { "icon" }, Content = presetContent, Padding = new Thickness(5, 3), VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(preset, Tools.Active.Name);
        ToolGroup? group = Tools.FindGroup(Tools.Active);
        if (group != null && group.Tools.Count > 1)
        {
            MenuFlyout flyout = new();
            foreach (ToolBase tool in group.Tools)
            {
                MenuItem item = new() { Header = tool.Name, Icon = new IconView(IconMap.ForTool(tool.Id), 16) };
                ToolBase captured = tool;
                item.Click += (_, _) => Tools.Select(captured);
                flyout.Items.Add(item);
            }
            preset.Flyout = flyout;
        }
        left.Children.Add(preset);
        left.Children.Add(new Border { Classes = { "vsep" } });
        try
        {
            Tools.Active.BuildOptions(builder);
        }
        catch (Exception ex)
        {
            builder.Label(string.Format(Strings.PhotoEditorWindow_OptionsUnavailable, ex.Message));
        }
        left.Children.Add(builder.Root);
        _optionsHost.Child = new ScrollViewer { Content = left, HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled };
        _dock.Get("brushes")?.Refresh();
    }

    public void SetStatus(string? text) => _status.Text = text ?? Tools.Active.Hint;

    public SKBitmap GetSampleImage(bool allLayers)
    {
        PhotoDocument doc = Document ?? throw new InvalidOperationException(Strings.PhotoEditorWindow_NoDocumentOpen);
        if (allLayers)
        {
            _view!.Update(false);
            return _view.Composite!.Copy();
        }
        if (doc.ActiveLayer is RasterLayer raster)
        {
            SKBitmap result = ImageOps.CreateBitmap(doc.Width, doc.Height);
            result.Erase(SKColors.Transparent);
            using SKCanvas canvas = new(result);
            canvas.DrawBitmap(raster.Surface.Bitmap, raster.Offset.X, raster.Offset.Y);
            return result;
        }
        if (doc.ActiveLayer is Layer layer && layer is not AdjustmentLayer) return Compositor.RenderLayer(doc, layer);
        _view!.Update(false);
        return _view.Composite!.Copy();
    }

    /// <summary>True while a background operation runs; documents must not be closed or edited meanwhile.</summary>
    public bool IsBusy => _busyCount > 0;

    public async Task RunBusyAsync(string message, Func<IProgress<float>?, Task> work)
    {
        _busyCount++;
        _busyText.Text = message;
        _busyProgress.Value = 0;
        _busy.IsVisible = true;
        Progress<float> progress = new(v => _busyProgress.Value = v);
        try
        {
            await work(progress);
        }
        catch (Exception ex)
        {
            ShowMessage(string.Format(Strings.PhotoEditorWindow_OperationFailed, message.TrimEnd('.'), ex.Message));
        }
        finally
        {
            _busyCount--;
            if (_busyCount == 0) _busy.IsVisible = false;
        }
    }

    public void ShowMessage(string text) => _ = DialogBuilder.ShowMessageAsync(this, Strings.PhotoEditorWindow_PhotoEditor, text);

    public Task<string?> PromptTextAsync(string title, string label, string initial, bool multiline = false) => DialogBuilder.PromptAsync(this, title, label, initial, multiline);

    /// <summary>Records a history state after a panel or command change and refreshes the UI.</summary>
    public void Commit(string historyName)
    {
        if (Document == null) return;
        Document.History.Push(historyName);
        InvalidateCanvas();
        RefreshPanels();
    }

    public void AfterHistoryChange()
    {
        if (IsTextEditing) EndTextEditing(false);
        if (Tools.Active is TransformTool { IsActive: true } transform) transform.Abandon();
        if (Tools.Active is CropTool crop) crop.Cancel();
        InvalidateCanvas();
        RefreshPanels();
        RefreshOptions();
    }

    public void Execute(string commandId)
    {
        EditorCommand? command = CommandRegistry.Get(commandId);
        if (command == null) return;
        if (command.CanExecute != null && !command.CanExecute(this)) return;
        // Menu commands act on committed content; undo and redo discard the pending edit themselves.
        if (IsTextEditing && commandId is not ("edit.undo" or "edit.redo" or "edit.stepBackward" or "edit.stepForward")) EndTextEditing(true);
        try
        {
            command.Execute(this);
        }
        catch (Exception ex)
        {
            ShowMessage(string.Format(Strings.PhotoEditorWindow_CouldNotComplete, command.Text.Replace("...", string.Empty), ex.Message));
        }
    }

    // ------------------------------------------------------------------ status

    public void UpdateStatusForPointer(SKPoint doc)
    {
        _cursorInfo.Text = $"X: {doc.X:0}  Y: {doc.Y:0}";
        _hRuler.InvalidateVisual();
        _vRuler.InvalidateVisual();
        _statusTimer.Stop();
        _statusTimer.Start();
    }

    public void RefreshInfo()
    {
        _dock.Get("info")?.Refresh();
        if (_dock.Get("navigator") is NavigatorPanel navigator && _dock.IsPanelVisible("navigator")) navigator.Refresh();
    }

    public void RefreshStatus()
    {
        if (View == null)
        {
            _zoomBox.Text = string.Empty;
            _docInfo.Text = string.Empty;
            _status.Text = Tools.Active.Hint;
            return;
        }
        if (!_zoomBox.IsFocused) _zoomBox.Text = $"{View.Zoom * 100:0.##}%";
        if (View.Zoom != _tabZoom) RefreshTabs();
        PhotoDocument doc = View.Document;
        double megabytes = doc.AllLayers.OfType<RasterLayer>().Sum(l => (double)l.Surface.Width * l.Surface.Height * 4) / 1024 / 1024;
        _docInfo.Text = string.Format(Strings.PhotoEditorWindow_DocumentInfo, doc.Width, doc.Height, doc.Resolution, megabytes) + (View.Rotation != 0 ? "   " + string.Format(Strings.PhotoEditorWindow_ViewRotation, View.Rotation) : string.Empty);
        if (string.IsNullOrEmpty(_status.Text)) _status.Text = Tools.Active.Hint;
    }

    private void OnToolChanged()
    {
        RefreshOptions();
        _status.Text = Tools.Active.Hint;
        Canvas.UpdateCursor();
        InvalidateOverlay();
    }

    // ------------------------------------------------------------------ snapping and context menu

    /// <summary>Snaps a document point to guides, grid and document edges when snapping is enabled.</summary>
    public SKPoint Snap(SKPoint p)
    {
        if (!Settings.Snap || View == null || Document == null) return p;
        float tolerance = 6 / (float)View.Zoom;
        float SnapAxis(float value, bool horizontal)
        {
            float best = value, bestDistance = tolerance;
            void Try(float candidate)
            {
                float d = MathF.Abs(candidate - value);
                if (d < bestDistance) { bestDistance = d; best = candidate; }
            }
            if (Settings.SnapToGuides && Settings.ShowGuides)
                foreach (Guide g in Document.Guides.Where(g => g.Horizontal == horizontal)) Try(g.Position);
            if (Settings.SnapToGrid && Settings.ShowGrid)
            {
                float step = Math.Max(1, Settings.GridSpacing / Math.Max(1, Settings.GridSubdivisions));
                Try(MathF.Round(value / step) * step);
            }
            if (Settings.SnapToDocument)
            {
                Try(0);
                Try(horizontal ? Document.Height : Document.Width);
            }
            return best;
        }
        return new SKPoint(SnapAxis(p.X, false), SnapAxis(p.Y, true));
    }

    public Point CanvasOffsetInRuler(RulerControl ruler) => Canvas.TranslatePoint(new Point(0, 0), ruler) ?? default;

    public void ShowCanvasContextMenu(Control target, Point position)
    {
        ContextMenu menu = new();
        void Add(string commandId)
        {
            EditorCommand? command = CommandRegistry.Get(commandId);
            if (command == null) return;
            MenuItem item = new() { Header = command.Text };
            item.Click += (_, _) => Execute(commandId);
            menu.Items.Add(item);
        }
        if (Document?.Selection != null)
        {
            Add("select.deselect");
            Add("select.inverse");
            Add("select.modify.feather");
            Add("select.selectAndMask");
            menu.Items.Add(new Separator());
            Add("layer.newViaCopy");
            Add("layer.newViaCut");
            Add("edit.fill");
            Add("edit.stroke");
            Add("edit.contentAwareFill");
            Add("edit.freeTransform");
            Add("paths.fromSelection");
        }
        else
        {
            Add("select.all");
            Add("select.reselect");
            menu.Items.Add(new Separator());
            Add("edit.undo");
            Add("edit.redo");
            Add("edit.paste");
            menu.Items.Add(new Separator());
            Add("edit.freeTransform");
            Add("layer.duplicate");
        }
        menu.Open(target);
        _ = position;
    }

    // ------------------------------------------------------------------ screen modes and visibility

    public void CycleScreenMode()
    {
        _screenMode = (ScreenMode)(((int)_screenMode + 1) % 3);
        WindowState = _screenMode switch
        {
            ScreenMode.Full => WindowState.FullScreen,
            ScreenMode.FullWithMenu => WindowState.Maximized,
            _ => WindowState.Normal
        };
        ApplyVisibility();
    }

    public void TogglePanels()
    {
        _panelsHidden = !_panelsHidden;
        ApplyVisibility();
    }

    public void ApplyVisibility()
    {
        bool panels = !_panelsHidden && _screenMode != ScreenMode.Full;
        _titleBar.IsVisible = _screenMode != ScreenMode.Full;
        _toolbarHost.IsVisible = panels && Settings.ToolbarVisible;
        _optionsHost.IsVisible = panels && Settings.OptionsVisible;
        _dock.IsVisible = panels && Settings.RightDockVisible;
        _dockSplitter.IsVisible = _dock.IsVisible;
        _dockColumn.Width = _dock.IsVisible ? new GridLength(Math.Clamp(Settings.RightDockWidth, 220, 600)) : new GridLength(0);
        _dockColumn.MinWidth = _dock.IsVisible ? 200 : 0;
        _tabHost.IsVisible = _screenMode == ScreenMode.Standard;
        _hRuler.IsVisible = Settings.ShowRulers && _view != null;
        _vRuler.IsVisible = Settings.ShowRulers && _view != null;
        _rulerCorner.IsVisible = _hRuler.IsVisible;
        bool showStart = _view == null;
        if (showStart && !_startScreen.IsVisible) _startScreen.Refresh();
        _startScreen.IsVisible = showStart;
    }

    public void RefreshStartScreen()
    {
        if (_startScreen.IsVisible) _startScreen.Refresh();
    }

    public void ToggleToolbarColumns() => _toolbar.ToggleColumns();

    public void RebuildToolbar()
    {
        Tools.ApplyLayout(Settings);
        _toolbar.Rebuild();
    }
}
