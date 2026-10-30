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
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
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
public sealed class PhotoEditorWindow : Window, IEditorHost
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
    private readonly Menu _menu;
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

    public PhotoEditorWindow(PhotoEditorServices services)
    {
        Services = services;
        Settings = EditorSettings.Load(services.SettingsFilePath);
        _foreground = EditorSettings.ParseColor(Settings.Foreground, SKColors.Black);
        _background = EditorSettings.ParseColor(Settings.Background, SKColors.White);
        Gradient = GradientDefinition.CreateDefault(_foreground, _background);
        Patterns = PatternDefinition.CreateDefaults();
        BrushPresets = BrushPreset.CreateDefaults();
        if (Settings.Swatches.Count == 0) Settings.Swatches.AddRange(SwatchesPanel.DefaultSwatches);

        Title = "ShareX - Photo editor (Beta)";
        Width = 1500;
        Height = 920;
        MinWidth = 900;
        MinHeight = 600;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
        _menu = MenuBuilder.Build(this);

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
        DockPanel.SetDock(_menu, Dock.Top);
        DockPanel.SetDock(_optionsHost, Dock.Top);
        DockPanel.SetDock(statusHost, Dock.Bottom);
        root.Children.Add(_menu);
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

    public GradientDefinition Gradient { get; set; }

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

    // ------------------------------------------------------------------ documents

    public void OpenDocument(PhotoDocument doc, string historyName = "Open")
    {
        doc.History.MaxStates = Math.Clamp(Settings.HistoryStates, 5, 1000);
        doc.History.Initialize(historyName);
        doc.IsDirty = false;
        DocumentView view = new(doc);
        doc.StructureChanged += () => Canvas.InvalidateOutline();
        _views.Add(view);
        ActivateView(view);
    }

    public void ActivateView(DocumentView? view)
    {
        if (IsTextEditing) EndTextEditing(true);
        if (_view == view) return;
        Tools.Active.Deactivate();
        _view = view;
        Tools.Active.Activate();
        Canvas.InvalidateOutline();
        RefreshTabs();
        RefreshPanels();
        RefreshOptions();
        InvalidateCanvas();
        UpdateTitle();
        ApplyVisibility();
    }

    public async Task<bool> CloseViewAsync(DocumentView view)
    {
        if (view.Document.IsDirty)
        {
            ActivateView(view);
            bool? save = await DialogBuilder.ConfirmAsync(this, "Photo editor", $"Save changes to \"{view.Document.DisplayName.TrimEnd('*')}\" before closing?", "Save", "Don't Save");
            if (save == null) return false;
            if (save == true && !await EditorCommands.SaveAsync(this, false)) return false;
        }
        int index = _views.IndexOf(view);
        _views.Remove(view);
        view.Dispose();
        view.Document.Dispose();
        if (_view == view)
        {
            _view = null;
            DocumentView? next = _views.Count > 0 ? _views[Math.Clamp(index, 0, _views.Count - 1)] : null;
            if (next != null) ActivateView(next);
            else
            {
                // No documents left: clear panels, options and status like an empty workspace.
                RefreshPanels();
                RefreshOptions();
                RefreshStatus();
                ApplyVisibility();
                UpdateTitle();
            }
        }
        RefreshTabs();
        InvalidateCanvas();
        return true;
    }

    private void RefreshTabs()
    {
        _tabZoom = _view?.Zoom ?? 0;
        _tabs.Children.Clear();
        foreach (DocumentView view in _views)
        {
            bool active = view == _view;
            DocumentView captured = view;
            PhotoDocument doc = view.Document;
            string layer = doc.ActiveLayer?.Name ?? "Background";
            string mode = doc.EditingMask && doc.ActiveLayer?.Mask != null ? "Layer Mask/8" : doc.QuickMask ? "Quick Mask/8" : "RGB/8";
            string text = $"{doc.DisplayName.TrimEnd('*')} @ {view.Zoom * 100:0.#}% ({layer}, {mode}){(doc.IsDirty ? " *" : string.Empty)}";

            Button close = new() { Classes = { "icon" }, Content = ToolIcons.Create("close", 9), Padding = new Thickness(3), Width = 16, Height = 16, VerticalAlignment = VerticalAlignment.Center, Opacity = active ? 1 : 0.6 };
            ToolTip.SetTip(close, "Close");
            close.Click += async (_, e) =>
            {
                e.Handled = true;
                await CloseViewAsync(captured);
            };
            StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            content.Children.Add(new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = active ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E))
            });
            content.Children.Add(close);
            // Document tabs: the active tab joins the canvas area, inactive tabs sit on the darker strip.
            Border tab = new()
            {
                Child = content,
                Padding = new Thickness(12, 0, 6, 0),
                Height = 28,
                Margin = new Thickness(0, 0, 1, 0),
                Background = active ? new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32)) : new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x2A)),
                BorderBrush = active ? OverlayContext.AccentBrush : Brushes.Transparent,
                BorderThickness = new Thickness(0, active ? 2 : 0, 0, 0),
                Cursor = new Cursor(StandardCursorType.Hand)
            };
            ToolTip.SetTip(tab, doc.FilePath ?? doc.Title);
            tab.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(tab).Properties.IsMiddleButtonPressed)
                {
                    _ = CloseViewAsync(captured);
                    return;
                }
                ActivateView(captured);
            };
            _tabs.Children.Add(tab);
        }
    }

    private void UpdateTitle()
    {
        Title = Document != null ? $"{Document.DisplayName} - Photo editor (Beta) - ShareX" : "ShareX - Photo editor (Beta)";
    }

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
        presetContent.Children.Add(ToolIcons.Create(Tools.Active.Id, 20));
        presetContent.Children.Add(ToolIcons.Create("chevron-down", 9));
        Button preset = new() { Classes = { "icon" }, Content = presetContent, Padding = new Thickness(5, 3), VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(preset, Tools.Active.Name);
        ToolGroup? group = Tools.FindGroup(Tools.Active);
        if (group != null && group.Tools.Count > 1)
        {
            MenuFlyout flyout = new();
            foreach (ToolBase tool in group.Tools)
            {
                MenuItem item = new() { Header = tool.Name, Icon = ToolIcons.Create(tool.Id, 16) };
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
            builder.Label("Options unavailable: " + ex.Message);
        }
        left.Children.Add(builder.Root);
        _optionsHost.Child = new ScrollViewer { Content = left, HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled };
        _dock.Get("brushes")?.Refresh();
    }

    public void SetStatus(string? text) => _status.Text = text ?? Tools.Active.Hint;

    public SKBitmap GetSampleImage(bool allLayers)
    {
        PhotoDocument doc = Document ?? throw new InvalidOperationException("No document is open.");
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

    public async Task RunBusyAsync(string message, Func<IProgress<float>?, Task> work)
    {
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
            ShowMessage($"{message.TrimEnd('.')} failed: {ex.Message}");
        }
        finally
        {
            _busy.IsVisible = false;
        }
    }

    public void ShowMessage(string text) => _ = DialogBuilder.ShowMessageAsync(this, "Photo editor", text);

    public Task<string?> PromptTextAsync(string title, string label, string initial) => DialogBuilder.PromptAsync(this, title, label, initial);

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
        InvalidateCanvas();
        RefreshPanels();
        RefreshOptions();
    }

    public void Execute(string commandId)
    {
        EditorCommand? command = CommandRegistry.Get(commandId);
        if (command == null) return;
        if (command.CanExecute != null && !command.CanExecute(this)) return;
        try
        {
            command.Execute(this);
        }
        catch (Exception ex)
        {
            ShowMessage($"Could not complete \"{command.Text.Replace("...", string.Empty)}\": {ex.Message}");
        }
    }

    // ------------------------------------------------------------------ text editing

    public void BeginTextEditing(TextLayer layer, bool isNew)
    {
        if (IsTextEditing) EndTextEditing(true);
        _editingText = layer;
        _editingIsNew = isNew;
        _editingOriginal = (TextLayer)layer.CloneShared();
        bool inline = !layer.Vertical && layer.Transform.IsIdentity;
        _editingWasVisible = layer.Visible;
        if (inline) layer.Visible = false;
        _textEditor = new TextBox
        {
            Text = layer.Text,
            AcceptsReturn = true,
            TextWrapping = layer.IsParagraph ? TextWrapping.Wrap : TextWrapping.NoWrap,
            Background = inline ? Brushes.Transparent : new SolidColorBrush(Color.FromArgb(220, 30, 30, 30)),
            BorderBrush = OverlayContext.AccentBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(0),
            MinWidth = 20,
            CaretBrush = Brushes.DodgerBlue
        };
        _textEditor.TextChanged += (_, _) =>
        {
            if (_editingText == null) return;
            _editingText.Text = _textEditor.Text ?? string.Empty;
            _editingText.Invalidate();
            InvalidateCanvas();
        };
        _overlayLayer.Children.Add(_textEditor);
        PositionTextEditor();
        _textEditor.AttachedToVisualTree += (_, _) => _textEditor?.Focus();
        RefreshOptions();
        InvalidateCanvas();
        SetStatus("Type the text. Ctrl+Enter commits, Esc cancels.");
    }

    private void PositionTextEditor()
    {
        if (_textEditor == null || _editingText == null || View == null) return;
        TextLayer layer = _editingText;
        bool inline = !layer.Vertical && layer.Transform.IsIdentity;
        double zoom = View.Zoom;
        SKRect block = layer.GetBlockBounds();
        Point p = View.DocToScreen(new SKPoint(block.Left, block.Top));
        if (inline)
        {
            _textEditor.FontFamily = new FontFamily(layer.FontFamily);
            _textEditor.FontSize = Math.Max(4, layer.FontSize * zoom);
            _textEditor.FontWeight = layer.Bold || layer.FauxBold ? FontWeight.Bold : FontWeight.Normal;
            _textEditor.FontStyle = layer.Italic || layer.FauxItalic ? FontStyle.Italic : FontStyle.Normal;
            _textEditor.Foreground = new SolidColorBrush(Color.FromArgb(layer.Color.Alpha, layer.Color.Red, layer.Color.Green, layer.Color.Blue));
            _textEditor.TextAlignment = layer.Alignment switch { Core.TextAlignment.Center => Avalonia.Media.TextAlignment.Center, Core.TextAlignment.Right => Avalonia.Media.TextAlignment.Right, _ => Avalonia.Media.TextAlignment.Left };
            _textEditor.LineHeight = layer.FontSize * layer.Leading * zoom;
            if (layer.IsParagraph)
            {
                _textEditor.Width = layer.BoxSize.Width * zoom;
                _textEditor.MinHeight = layer.BoxSize.Height * zoom;
            }
            Avalonia.Controls.Canvas.SetLeft(_textEditor, p.X - 1);
            Avalonia.Controls.Canvas.SetTop(_textEditor, p.Y - 1);
        }
        else
        {
            _textEditor.FontSize = 14;
            _textEditor.Foreground = Brushes.White;
            _textEditor.Width = 260;
            _textEditor.MinHeight = 80;
            Point below = View.DocToScreen(new SKPoint(block.Left, block.Bottom));
            Avalonia.Controls.Canvas.SetLeft(_textEditor, below.X);
            Avalonia.Controls.Canvas.SetTop(_textEditor, below.Y + 8);
        }
    }

    public void EndTextEditing(bool commit)
    {
        TextLayer? layer = _editingText;
        if (layer == null) return;
        _editingText = null;
        if (_textEditor != null) _overlayLayer.Children.Remove(_textEditor);
        _textEditor = null;
        layer.Visible = _editingWasVisible;
        PhotoDocument? doc = Document;
        if (doc != null)
        {
            if (Tools.Active is TypeTool { IsMask: true } maskTool && maskTool.FinishMaskEditing(layer, commit))
            {
                // Type mask tools convert the text to a selection.
            }
            else if (!commit)
            {
                if (_editingIsNew) doc.RemoveLayer(layer);
                else if (_editingOriginal != null) _editingOriginal.CopyTextPropertiesTo(layer);
                layer.Invalidate();
            }
            else if (string.IsNullOrWhiteSpace(layer.Text))
            {
                if (_editingIsNew) doc.RemoveLayer(layer);
                else doc.History.Push("Edit Type Layer");
            }
            else
            {
                if (_editingIsNew || layer.Name == "Layer") layer.Name = layer.Text.Split('\n')[0].Trim() is { Length: > 0 } first ? first[..Math.Min(first.Length, 32)] : "Text";
                doc.History.Push(_editingIsNew ? "Type Tool" : "Edit Type Layer");
            }
        }
        _editingOriginal = null;
        InvalidateCanvas();
        RefreshPanels();
        RefreshOptions();
        SetStatus(null);
        Canvas.Focus();
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
        _docInfo.Text = $"{doc.Width} × {doc.Height} px ({doc.Resolution:0} ppi)   Doc: {megabytes:0.0}M" + (View.Rotation != 0 ? $"   View: {View.Rotation:0}°" : string.Empty);
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
        _menu.IsVisible = _screenMode != ScreenMode.Full;
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

    public void ToggleToolbarColumns() => _toolbar.ToggleColumns();

    public void RebuildToolbar()
    {
        Tools.ApplyLayout(Settings);
        _toolbar.Rebuild();
    }

    public void RebuildMenu()
    {
        if (Content is Grid shell && shell.Children[0] is DockPanel root)
        {
            int index = root.Children.IndexOf(_menu);
            Menu menu = MenuBuilder.Build(this);
            DockPanel.SetDock(menu, Avalonia.Controls.Dock.Top);
            root.Children[index] = menu;
        }
    }

    // ------------------------------------------------------------------ keyboard

    private static bool IsTextInput(object? source)
    {
        for (StyledElement? e = source as StyledElement; e != null; e = e.Parent)
        {
            if (e is TextBox || e is NumericUpDown || e is ComboBox { IsDropDownOpen: true }) return true;
        }
        return false;
    }

    private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
    {
        LastModifiers = e.KeyModifiers;
        if (IsTextEditing)
        {
            if (e.Key == Key.Escape) { EndTextEditing(false); e.Handled = true; }
            else if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Control)) { EndTextEditing(true); e.Handled = true; }
            return;
        }
        if (IsTextInput(e.Source)) return;
        if (_busy.IsVisible)
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Space && !Canvas.SpaceHeld)
        {
            Canvas.SpaceHeld = true;
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Tab && e.KeyModifiers == KeyModifiers.None)
        {
            TogglePanels();
            e.Handled = true;
            return;
        }
        // Alt temporarily switches painting tools to the Eyedropper, like the reference application.
        if ((e.Key == Key.LeftAlt || e.Key == Key.RightAlt) && _springReturn == null && Tools.Active is BrushTool or PencilTool or GradientTool or PaintBucketTool or ColorReplacementTool)
        {
            _springReturn = Tools.Active;
            Tools.Select(ToolId.Eyedropper);
            e.Handled = true;
            return;
        }

        if (Tools.Active.KeyDown(e.Key, e.KeyModifiers))
        {
            e.Handled = true;
            return;
        }
        EditorCommand? command = CommandRegistry.Match(e.Key, e.KeyModifiers, Settings);
        if (command != null)
        {
            Execute(command.Id);
            e.Handled = true;
            return;
        }
        if (Tools.HandleShortcut(e.Key, e.KeyModifiers)) e.Handled = true;
    }

    private void OnKeyUpTunnel(object? sender, KeyEventArgs e)
    {
        LastModifiers = e.KeyModifiers;
        if (e.Key == Key.Space && Canvas.SpaceHeld)
        {
            Canvas.SpaceHeld = false;
            e.Handled = true;
        }
        if ((e.Key == Key.LeftAlt || e.Key == Key.RightAlt) && _springReturn != null)
        {
            Tools.Select(_springReturn);
            _springReturn = null;
            e.Handled = true;
        }
        Tools.Active.KeyUp(e.Key, e.KeyModifiers);
    }

    // ------------------------------------------------------------------ drag and drop

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        List<string> paths = e.DataTransfer.TryGetFiles()?.Select(f => f.TryGetLocalPath()).Where(p => !string.IsNullOrEmpty(p)).Select(p => p!).ToList() ?? [];
        foreach (string path in paths.Where(ImageIO.IsSupported))
        {
            if (Document != null && !ImageIO.IsProject(path) && !e.KeyModifiers.HasFlag(KeyModifiers.Shift)) EditorCommands.PlaceFile(this, path);
            else EditorCommands.OpenFile(this, path);
        }
    }

    // ------------------------------------------------------------------ file pickers

    public async Task<IReadOnlyList<string>> PickOpenFilesAsync(string title, bool multiple = true)
    {
        IStorageFolder? start = Services.DefaultFolder != null ? await StorageProvider.TryGetFolderFromPathAsync(Services.DefaultFolder) : null;
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = multiple,
            SuggestedStartLocation = start,
            FileTypeFilter =
            [
                new FilePickerFileType("All supported images") { Patterns = ImageIO.Formats.SelectMany(f => f.Extensions).Select(x => "*" + x).ToArray() },
                .. ImageIO.Formats.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Extensions.Select(x => "*" + x).ToArray() }),
                FilePickerFileTypes.All
            ]
        });
        return files.Select(f => f.TryGetLocalPath()).Where(p => p != null).Select(p => p!).ToList();
    }

    public async Task<string?> PickSaveFileAsync(string title, string suggestedName, bool projectFirst)
    {
        IStorageFolder? start = Document?.FilePath != null ? await StorageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(Document.FilePath)!) :
            Services.DefaultFolder != null ? await StorageProvider.TryGetFolderFromPathAsync(Services.DefaultFolder) : null;
        IEnumerable<FileFormatInfo> formats = ImageIO.Formats.Where(f => f.CanSave);
        if (!projectFirst) formats = formats.OrderBy(f => f.Extensions[0] == ImageIO.ProjectExtension);
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedName,
            SuggestedStartLocation = start,
            DefaultExtension = projectFirst ? ImageIO.ProjectExtension : ".png",
            ShowOverwritePrompt = true,
            FileTypeChoices = formats.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Extensions.Select(x => "*" + x).ToArray() }).ToList()
        });
        return file?.TryGetLocalPath();
    }

    // ------------------------------------------------------------------ closing

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_closingConfirmed)
        {
            SaveSettings();
            return;
        }
        if (_views.Any(v => v.Document.IsDirty))
        {
            e.Cancel = true;
            foreach (DocumentView view in _views.ToList())
            {
                if (!await CloseViewAsync(view)) return;
            }
            _closingConfirmed = true;
            Close();
            return;
        }
        SaveSettings();
        foreach (DocumentView view in _views.ToList())
        {
            view.Dispose();
            view.Document.Dispose();
        }
        _views.Clear();
    }

    public void SaveSettings()
    {
        _dock.SaveLayout();
        if (_dockColumn.ActualWidth > 0) Settings.RightDockWidth = _dockColumn.ActualWidth;
        Settings.Save(Services.SettingsFilePath);
    }
}
