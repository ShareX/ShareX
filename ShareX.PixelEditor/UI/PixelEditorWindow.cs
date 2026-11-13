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
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Theme;
using ShareX.PixelEditor.Integration;
using ShareX.PixelEditor.Tools;
using ShareX.PixelEditor.UI.Commands;
using ShareX.PixelEditor.UI.Panels;

namespace ShareX.PixelEditor.UI;

/// <summary>The Pixel editor main window. Owns the open sprites, tools, panels, timeline and commands.</summary>
public sealed partial class PixelEditorWindow : Window
{
    private readonly List<SpriteView> _views = [];
    private SpriteView? _view;
    private uint _foreground, _background;
    private readonly Border _optionsHost = new() { Padding = new Thickness(6, 3), Height = 38 };
    private readonly StackPanel _tabs = new() { Orientation = Orientation.Horizontal };
    private readonly TextBlock _status = new() { VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis, FontSize = 11, Classes = { "dim" } };
    private readonly TextBlock _docInfo = new() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0), FontSize = 11, Classes = { "dim" } };
    private readonly TextBlock _cursorInfo = new() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0), MinWidth = 150, FontSize = 11, Classes = { "dim" } };
    private readonly TextBox _zoomBox = new() { Width = 54, MinHeight = 18, Height = 18, Padding = new Thickness(4, 0), FontSize = 11, VerticalContentAlignment = VerticalAlignment.Center, BorderThickness = new Thickness(0), Background = Brushes.Transparent };
    private readonly Border _busy;
    private readonly CommandPalette _palette;
    private readonly TextBlock _busyText = new() { Foreground = Brushes.White, FontSize = 14 };
    private readonly ProgressBar _busyProgress = new() { Width = 260, Minimum = 0, Maximum = 1 };
    private readonly ToolbarControl _toolbar;
    private readonly PanelDock _dock;
    private readonly Timeline _timeline;
    private readonly Grid _layout;
    private readonly Grid _centerGrid;
    private readonly ColumnDefinition _dockColumn;
    private readonly RowDefinition _timelineRow;
    private readonly GridSplitter _dockSplitter, _timelineSplitter;
    private readonly Border _toolbarHost, _tabHost;
    private readonly StartScreen _startScreen;
    private readonly DispatcherTimer _playTimer = new();
    private readonly DispatcherTimer _autoSaveTimer = new();
    private readonly Recovery _recovery;
    private string? _statusOverride;
    private bool _panelsHidden, _closingConfirmed;
    private double _shownZoom;
    private int _busyCount;

    public PixelEditorWindow(PixelEditorServices services)
    {
        Services = services;
        Settings = EditorSettings.Load(services.SettingsFilePath);
        _recovery = new Recovery(services.SettingsFilePath);
        _foreground = EditorSettings.ParseColor(Settings.Foreground, Argb.Black);
        _background = EditorSettings.ParseColor(Settings.Background, Argb.White);

        Title = Strings.Window_Title;
        Width = 1400;
        Height = 880;
        MinWidth = 900;
        MinHeight = 600;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ApplySavedWindowState();
        PhotoEditorTheme.Apply(this);

        Tools = new ToolManager(this);
        Tools.ActiveChanged += OnToolChanged;
        CommandRegistry.EnsureInitialized();

        Canvas = new CanvasView(this);
        _toolbar = new ToolbarControl(this);
        _timeline = new Timeline(this);
        _dock = new PanelDock(this, [new ColorPanel(this), new PalettePanel(this), new LayersPanel(this), new PreviewPanel(this), new HistoryPanel(this), new TilesPanel(this)]);
        _titleBar = CreateTitleBar();

        // Starts hidden so the first ApplyVisibility refreshes the recent files.
        _startScreen = new StartScreen(this) { IsVisible = false };
        Grid canvasStack = new();
        canvasStack.Children.Add(Canvas);
        canvasStack.Children.Add(_startScreen);

        _tabHost = new Border { Child = new ScrollViewer { Content = _tabs, HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled }, Height = 28, Background = Ui.Deep };
        _timelineRow = new RowDefinition(new GridLength(Math.Clamp(Settings.TimelineHeight, 90, 600))) { MinHeight = 80 };
        _centerGrid = new Grid { RowDefinitions = new RowDefinitions { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto), _timelineRow } };
        _timelineSplitter = new GridSplitter { Height = 2, ResizeDirection = GridResizeDirection.Rows, Background = Ui.Gutter };
        Grid.SetRow(canvasStack, 1);
        Grid.SetRow(_timelineSplitter, 2);
        Grid.SetRow(_timeline, 3);
        _centerGrid.Children.Add(_tabHost);
        _centerGrid.Children.Add(canvasStack);
        _centerGrid.Children.Add(_timelineSplitter);
        _centerGrid.Children.Add(_timeline);

        _toolbarHost = new Border { Child = _toolbar, Margin = new Thickness(0, 0, 2, 0), Background = Ui.Chrome };
        _dockColumn = new ColumnDefinition(new GridLength(Math.Clamp(Settings.RightDockWidth, 220, 600))) { MinWidth = 200 };
        _layout = new Grid { ColumnDefinitions = new ColumnDefinitions { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto), _dockColumn } };
        _dockSplitter = new GridSplitter { Width = 2, ResizeDirection = GridResizeDirection.Columns, Background = Ui.Gutter };
        Grid.SetColumn(_centerGrid, 1);
        Grid.SetColumn(_dockSplitter, 2);
        Grid.SetColumn(_dock, 3);
        _layout.Children.Add(_toolbarHost);
        _layout.Children.Add(_centerGrid);
        _layout.Children.Add(_dockSplitter);
        _layout.Children.Add(_dock);

        _optionsHost.Background = Ui.Chrome;
        _optionsHost.Margin = new Thickness(0, 0, 0, 2);

        _zoomBox.KeyDown += (_, e) =>
        {
            if (e.Key != Key.Enter || View == null) return;
            if (double.TryParse(_zoomBox.Text?.Replace("%", string.Empty).Trim(), out double percent) && percent > 0)
            {
                View.ZoomTo(percent / 100, View.ViewportCenter);
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
        Border statusHost = new() { Child = statusBar, Margin = new Thickness(0, 2, 0, 0), Background = Ui.Chrome };

        DockPanel root = new() { Background = Ui.Gutter };
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
        _palette = new CommandPalette(this);
        shell.Children.Add(root);
        shell.Children.Add(_palette);
        shell.Children.Add(_busy);
        Content = shell;

        AddHandler(KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
        AddHandler(KeyUpEvent, OnKeyUpTunnel, RoutingStrategies.Tunnel);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        DragDrop.SetAllowDrop(this, true);
        _playTimer.Tick += (_, _) => AdvancePlayback();
        _autoSaveTimer.Tick += (_, _) => BackupSprites();

        ApplyVisibility();
        RefreshOptions();
        RefreshTabs();
        RefreshStatus();
        Closing += OnClosing;
        Closed += (_, _) =>
        {
            _playTimer.Stop();
            _autoSaveTimer.Stop();
            _recovery.Dispose();
            Canvas.StopTimers();
            (_dock.Get("preview") as PreviewPanel)?.Stop();
        };
        Opened += (_, _) =>
        {
            _windowPlacementApplied = true;
            RestartAutoSave();
            Dispatcher.UIThread.Post(() => _ = RestoreRecoveredAsync(), DispatcherPriority.Background);
        };
        Resized += (_, _) => QueueWindowPlacementSave();
        PositionChanged += (_, _) => QueueWindowPlacementSave();
    }

    public PixelEditorServices Services { get; }

    public EditorSettings Settings { get; }

    public CanvasView Canvas { get; }

    public PanelDock Panels => _dock;

    public ToolManager Tools { get; }

    public IReadOnlyList<SpriteView> Views => _views;

    public SpriteView? View => _view;

    public Sprite? Sprite => _view?.Sprite;

    public KeyModifiers LastModifiers { get; private set; }

    /// <summary>A brush tip made from a selection; it replaces the round and square tips until it is cleared.</summary>
    public SKPointI[]? CustomTip { get; set; }

    public uint ForegroundColor
    {
        get => _foreground;
        set
        {
            _foreground = value;
            Settings.Foreground = "#" + Argb.ToHex(value);
            _toolbar.RefreshColors();
            _dock.Get("color")?.Refresh();
            (_dock.Get("palette") as PalettePanel)?.RefreshSelection();
        }
    }

    public uint BackgroundColor
    {
        get => _background;
        set
        {
            _background = value;
            Settings.Background = "#" + Argb.ToHex(value);
            _toolbar.RefreshColors();
            _dock.Get("color")?.Refresh();
            (_dock.Get("palette") as PalettePanel)?.RefreshSelection();
        }
    }

    private int _selectedTile = 1;

    /// <summary>The tile the Tile Stamp tool places; 0 is the empty tile.</summary>
    public int SelectedTile
    {
        get => _selectedTile;
        set
        {
            _selectedTile = Math.Max(0, value);
            (_dock.Get("tiles") as TilesPanel)?.RefreshSelection();
            if (Tools.Active.Id == ToolId.TileStamp) RefreshOptions();
        }
    }

    /// <summary>Opens the search box that runs any command, tool or panel by name.</summary>
    public void ShowCommandPalette() => _palette.Open();

    /// <summary>Remembers a color that was painted with, for the recent colors of the Color panel.</summary>
    public void AddRecentColor(uint color)
    {
        if (color >> 24 == 0) return;
        List<uint> recent = Settings.RecentColors;
        if (recent.Count > 0 && recent[0] == color) return;
        recent.Remove(color);
        recent.Insert(0, color);
        if (recent.Count > EditorSettings.MaxRecentColors) recent.RemoveRange(EditorSettings.MaxRecentColors, recent.Count - EditorSettings.MaxRecentColors);
        _dock.Get("color")?.Refresh();
    }

    public void SwapColors()
    {
        (_foreground, _background) = (_background, _foreground);
        ForegroundColor = _foreground;
        BackgroundColor = _background;
    }

    // ------------------------------------------------------------------ refreshing

    /// <summary>Redraws the canvas after pixels changed, in an area or everywhere.</summary>
    public void InvalidateCanvas(SKRectI? area = null)
    {
        if (_view != null)
        {
            if (area is SKRectI r) _view.MarkDirty(r);
            else _view.MarkDirty(null);
        }
        Canvas.InvalidateVisual();
    }

    /// <summary>Redraws overlays (selection, tool preview) and view readouts without recompositing.</summary>
    public void InvalidateOverlay()
    {
        Canvas.InvalidateVisual();
        RefreshStatus();
    }

    public void RefreshPanels()
    {
        _dock.RefreshVisible();
        _timeline.Refresh();
        _toolbar.RefreshColors();
        RefreshTabs();
        UpdateTitle();
        RefreshStatus();
        Canvas.InvalidateVisual();
    }

    public void RefreshTimeline() => _timeline.Refresh();

    public void RefreshOptions()
    {
        OptionsBuilder builder = new(this);
        StackPanel left = new() { Orientation = Orientation.Horizontal, Spacing = 4, VerticalAlignment = VerticalAlignment.Center };
        ToolBase tool = Tools.Active;
        StackPanel presetContent = new() { Orientation = Orientation.Horizontal, Spacing = 3 };
        presetContent.Children.Add(new IconView(tool.Icon, 20));
        presetContent.Children.Add(new IconView(PixelEditorIcons.ChevronDown, 10));
        Button preset = new() { Classes = { "icon" }, Content = presetContent, Padding = new Thickness(5, 3), VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(preset, tool.Name);
        MenuFlyout flyout = new();
        foreach (ToolBase other in Tools.GroupOf(tool).Tools)
        {
            MenuItem item = new() { Header = other.Name, Icon = new IconView(other.Icon, 16) };
            ToolBase captured = other;
            item.Click += (_, _) => Tools.Select(captured);
            flyout.Items.Add(item);
        }
        preset.Flyout = flyout;
        left.Children.Add(preset);
        left.Children.Add(new Border { Classes = { "vsep" } });
        try
        {
            tool.BuildOptions(builder);
        }
        catch (Exception ex)
        {
            builder.Label(ex.Message);
        }
        left.Children.Add(builder.Root);

        // View toggles stay at the right end of the bar for every tool.
        OptionsBuilder toggles = new(this);
        toggles.Toggle(PixelEditorIcons.SymmetryHorizontal, Strings.Command_SymmetryHorizontal, Settings.Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both, _ => Execute("view.symmetryHorizontal"));
        toggles.Toggle(PixelEditorIcons.SymmetryVertical, Strings.Command_SymmetryVertical, Settings.Symmetry is SymmetryMode.Vertical or SymmetryMode.Both, _ => Execute("view.symmetryVertical"));
        toggles.Toggle(PixelEditorIcons.Tiled, Strings.Command_TiledMode, Settings.Tiled != TiledMode.None, _ => Execute("view.tiled"));
        toggles.Separator();
        toggles.Toggle(PixelEditorIcons.PixelGrid, Strings.Command_PixelGrid, Settings.ShowPixelGrid, _ => Execute("view.pixelGrid"));
        toggles.Toggle(PixelEditorIcons.Grid, Strings.Command_Grid, Settings.ShowGrid, _ => Execute("view.grid"));
        toggles.Toggle(PixelEditorIcons.SnapToGrid, Strings.Command_SnapToGrid, Settings.SnapToGrid, _ => Execute("view.snapToGrid"));
        toggles.Separator();
        toggles.Toggle(PixelEditorIcons.OnionSkin, Strings.Command_OnionSkin, Settings.OnionSkin, _ => Execute("frame.onionSkin"));
        toggles.Root.Margin = new Thickness(12, 0, 0, 0);

        DockPanel bar = new();
        DockPanel.SetDock(toggles.Root, Dock.Right);
        bar.Children.Add(toggles.Root);
        bar.Children.Add(new ScrollViewer { Content = left, HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled });
        _optionsHost.Child = bar;
    }

    /// <summary>Shows a message in the status bar; null goes back to the active tool's hint.</summary>
    public void SetStatus(string? text)
    {
        _statusOverride = text;
        _status.Text = text ?? Tools.Active.Hint;
    }

    public void UpdateCursorStatus(int? x, int? y)
    {
        if (x is not int px || y is not int py)
        {
            _cursorInfo.Text = string.Empty;
            return;
        }
        uint? color = SampleColor(px, py, true);
        string text = color is uint c && c >> 24 != 0 ? $"X: {px}  Y: {py}   #{Argb.ToHex(c)}" : $"X: {px}  Y: {py}";
        if (Sprite?.Selection is SelectionMask selection && !selection.IsEmpty) text += "   " + string.Format(Strings.Status_Selection, selection.Bounds.Width, selection.Bounds.Height);
        _cursorInfo.Text = text;
    }

    public void RefreshStatus()
    {
        if (View == null)
        {
            _zoomBox.Text = string.Empty;
            _docInfo.Text = string.Empty;
            _status.Text = string.Empty;
            return;
        }
        if (!_zoomBox.IsFocused) _zoomBox.Text = $"{View.Zoom * 100:0.##}%";
        if (View.Zoom != _shownZoom) RefreshTabs();
        Sprite sprite = View.Sprite;
        _docInfo.Text = string.Format(Strings.Status_SpriteInfo, sprite.Width, sprite.Height, View.ShownFrame + 1, sprite.Frames.Count, sprite.Frames[View.ShownFrame].Duration);
        _status.Text = _statusOverride ?? Tools.Active.Hint;
    }

    private void OnToolChanged()
    {
        _statusOverride = null;
        RefreshOptions();
        _status.Text = View != null ? Tools.Active.Hint : string.Empty;
        Canvas.UpdateCursor();
        Canvas.InvalidateVisual();
    }

    // ------------------------------------------------------------------ busy, messages, commands

    public bool IsBusy => _busyCount > 0;

    public async Task RunBusyAsync(string message, Func<IProgress<float>, Task> work)
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
            ShowMessage(string.Format(Strings.Message_OperationFailed, ex.Message));
        }
        finally
        {
            _busyCount--;
            if (_busyCount == 0) _busy.IsVisible = false;
        }
    }

    public void ShowMessage(string text) => _ = DialogBuilder.ShowMessageAsync(this, Strings.Window_Name, text);

    public Task<string?> PromptTextAsync(string title, string label, string initial) => DialogBuilder.PromptAsync(this, title, label, initial);

    public void Execute(string commandId)
    {
        EditorCommand? command = CommandRegistry.Get(commandId);
        if (command == null || IsBusy) return;
        if (command.CanExecute != null && !command.CanExecute(this)) return;
        // Commands work on dropped pixels; only transform commands keep a moved selection afloat.
        if (!command.KeepsFloating)
        {
            // A half-finished tool edit (a curve waiting for its bend) is abandoned so it cannot end up in the history.
            if (Tools.Active.HasPendingEdit) Tools.Active.Cancel();
            CommitFloating();
        }
        if (command.StopsPlayback) StopPlayback();
        try
        {
            command.Execute(this);
        }
        catch (Exception ex)
        {
            ShowMessage(string.Format(Strings.Message_CommandFailed, command.Text.Replace("...", string.Empty), ex.Message));
        }
    }

    // ------------------------------------------------------------------ visibility

    public void TogglePanels()
    {
        _panelsHidden = !_panelsHidden;
        ApplyVisibility();
    }

    public void ToggleFullScreen()
    {
        WindowState = WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
        ApplyVisibility();
    }

    public void ApplyVisibility()
    {
        bool panels = !_panelsHidden;
        bool hasSprite = _view != null;
        _titleBar.IsVisible = WindowState != WindowState.FullScreen;
        _toolbarHost.IsVisible = panels && Settings.ToolbarVisible;
        _optionsHost.IsVisible = panels && Settings.OptionsVisible;
        _dock.IsVisible = panels && Settings.RightDockVisible;
        _dockSplitter.IsVisible = _dock.IsVisible;
        _dockColumn.Width = _dock.IsVisible ? new GridLength(Math.Clamp(Settings.RightDockWidth, 220, 600)) : new GridLength(0);
        _dockColumn.MinWidth = _dock.IsVisible ? 200 : 0;
        bool timeline = panels && Settings.TimelineVisible && hasSprite;
        _timeline.IsVisible = timeline;
        _timelineSplitter.IsVisible = timeline;
        _timelineRow.Height = timeline ? new GridLength(Math.Clamp(Settings.TimelineHeight, 90, 600)) : new GridLength(0);
        _timelineRow.MinHeight = timeline ? 80 : 0;
        _tabHost.IsVisible = hasSprite;
        bool showStart = !hasSprite;
        if (showStart && !_startScreen.IsVisible) _startScreen.Refresh();
        _startScreen.IsVisible = showStart;
    }

    public void RefreshStartScreen()
    {
        if (_startScreen.IsVisible) _startScreen.Refresh();
    }

    public void ToggleToolbarColumns() => _toolbar.ToggleColumns();
}
