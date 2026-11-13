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
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Theme;

namespace ShareX.PixelEditor.UI.Panels;

/// <summary>The layer stack with visibility, lock, blend mode and opacity.</summary>
public sealed class LayersPanel : EditorPanel
{
    private const int ThumbSize = 28;

    private readonly StackPanel _list = new();
    private readonly ComboBox _blend;
    private readonly Slider _opacity = new() { Minimum = 0, Maximum = 255, MinWidth = 60, VerticalAlignment = VerticalAlignment.Center };
    private readonly TextBlock _opacityText = new() { Width = 34, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Right, Classes = { "dim" } };
    private bool _updating, _opacityChanged;

    public LayersPanel(PixelEditorWindow editor) : base(editor)
    {
        _blend = DialogBuilder.Combo(EnumNames.BlendModes, BlendMode.Normal, mode =>
        {
            if (_updating || Sprite?.ActiveLayer is not Layer layer || layer.Blend == mode) return;
            layer.Blend = mode;
            Sprite.Touch();
            Editor.Commit(Strings.History_BlendMode);
        });
        _blend.MinWidth = 96;
        _opacity.ValueChanged += (_, e) =>
        {
            if (_updating || Sprite?.ActiveLayer is not Layer layer) return;
            layer.Opacity = (int)Math.Round(e.NewValue);
            _opacityText.Text = $"{layer.Opacity * 100 / 255}%";
            _opacityChanged = true;
            Sprite.Touch();
            Editor.InvalidateCanvas();
        };
        // One history state per drag, recorded when the slider is released.
        _opacity.AddHandler(PointerReleasedEvent, (_, _) => CommitOpacity(), Avalonia.Interactivity.RoutingStrategies.Tunnel | Avalonia.Interactivity.RoutingStrategies.Bubble, true);
        _opacity.LostFocus += (_, _) => CommitOpacity();
        _opacity.KeyUp += (_, _) => CommitOpacity();

        Grid top = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 6, Margin = new Thickness(6, 5) };
        Grid.SetColumn(_opacity, 1);
        Grid.SetColumn(_opacityText, 2);
        top.Children.Add(_blend);
        top.Children.Add(_opacity);
        top.Children.Add(_opacityText);

        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 1, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 4, 0) };
        void Add(string icon, string commandId)
        {
            Commands.EditorCommand? command = Commands.CommandRegistry.Get(commandId);
            buttons.Children.Add(Ui.IconButton(icon, command?.Text.Replace("...", string.Empty) ?? commandId, () => Editor.Execute(commandId)));
        }
        Add(PixelEditorIcons.NewLayer, "layer.new");
        Add(PixelEditorIcons.NewGroup, "layer.newGroup");
        Add(PixelEditorIcons.Duplicate, "layer.duplicate");
        Add(PixelEditorIcons.MergeDown, "layer.mergeDown");
        Add(PixelEditorIcons.ArrowUp, "layer.moveUp");
        Add(PixelEditorIcons.ArrowDown, "layer.moveDown");
        Add(PixelEditorIcons.Trash, "layer.delete");
        Border bar = new() { Height = 28, Background = Ui.Deep, Child = buttons };

        DockPanel root = new();
        DockPanel.SetDock(top, Dock.Top);
        DockPanel.SetDock(bar, Dock.Bottom);
        root.Children.Add(top);
        root.Children.Add(bar);
        // The drop line of a layer drag floats over the list.
        Canvas overlay = new() { IsHitTestVisible = false };
        overlay.Children.Add(_dropLine);
        Grid listHost = new();
        listHost.Children.Add(_list);
        listHost.Children.Add(overlay);
        root.Children.Add(new ScrollViewer { Content = listHost, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, Background = Ui.Deep });
        Content = root;
        _list.Background = Brushes.Transparent;
        _list.PointerMoved += (_, e) =>
        {
            if (_dragLayerId == 0 || !ReferenceEquals(e.Pointer.Captured, _list)) return;
            double y = e.GetPosition(_list).Y;
            if (!_dragging && Math.Abs(y - _dragStartY) < 6) return;
            _dragging = true;
            _dropLine.IsVisible = true;
            _dropLine.Width = _list.Bounds.Width;
            Canvas.SetTop(_dropLine, DropSlot(y) * RowHeight - 1);
        };
        _list.PointerReleased += (_, e) =>
        {
            e.Pointer.Capture(null);
            bool dragged = _dragging;
            int id = _dragLayerId;
            _dragging = false;
            _dragLayerId = 0;
            _dropLine.IsVisible = false;
            if (dragged) DropLayer(id, DropSlot(e.GetPosition(_list).Y));
        };
        _list.PointerCaptureLost += (_, _) =>
        {
            _dragging = false;
            _dragLayerId = 0;
            _dropLine.IsVisible = false;
        };
    }

    private const double RowHeight = 37;
    private readonly Border _dropLine = new() { Height = 2, Background = PhotoEditorTheme.AccentBorderBrush, IsVisible = false };
    private int _dragLayerId;
    private double _dragStartY;
    private bool _dragging;

    private int DropSlot(double y) => Math.Clamp((int)Math.Round(y / RowHeight), 0, _list.Children.Count);

    /// <summary>Moves a dragged layer to a slot between the listed rows: above the row at that slot, or below everything.</summary>
    private void DropLayer(int layerId, int slot)
    {
        Sprite? sprite = Sprite;
        if (sprite?.FindLayer(layerId) is not Layer layer) return;
        List<Layer> rows = sprite.Root.Rows().ToList();
        Layer? target = slot < rows.Count ? rows[slot] : null;
        if (target == layer || target != null && layer is GroupLayer dragged && target.IsDescendantOf(dragged)) return;
        // Dropping right below the dragged layer leaves it where it is.
        if (slot > 0 && rows[slot - 1] == layer && (target == null || target.Parent == layer.Parent)) return;
        Editor.CommitFloating();
        sprite.RemoveLayer(layer);
        if (target == null) sprite.InsertLayer(layer, sprite.Root, 0);
        else
        {
            GroupLayer parent = target.Parent ?? sprite.Root;
            sprite.InsertLayer(layer, parent, parent.Children.IndexOf(target) + 1);
        }
        sprite.ActiveLayer = layer;
        Editor.Commit(Strings.History_ReorderLayer);
    }

    public override string Id => "layers";

    public override string Title => Strings.Panel_Layers;

    private void CommitOpacity()
    {
        if (!_opacityChanged) return;
        _opacityChanged = false;
        Editor.Commit(Strings.History_Opacity);
    }

    public override void Refresh()
    {
        _list.Children.Clear();
        Sprite? sprite = Sprite;
        _updating = true;
        _blend.IsEnabled = _opacity.IsEnabled = sprite?.ActiveLayer != null;
        if (sprite?.ActiveLayer is Layer active)
        {
            _blend.SelectedIndex = Math.Max(0, EnumNames.BlendModes.ToList().FindIndex(b => b.Item2 == active.Blend));
            _opacity.Value = active.Opacity;
            _opacityText.Text = $"{active.Opacity * 100 / 255}%";
        }
        else
        {
            _opacityText.Text = string.Empty;
        }
        _updating = false;
        if (sprite == null) return;
        foreach (Layer layer in sprite.Root.Rows()) _list.Children.Add(BuildRow(sprite, layer));
    }

    private Control BuildRow(Sprite sprite, Layer layer)
    {
        bool active = layer == sprite.ActiveLayer;
        Button eye = Ui.IconButton(layer.Visible ? PixelEditorIcons.Eye : PixelEditorIcons.EyeOff, Strings.Layers_ToggleVisibility, () => Commands.EditorCommands.ToggleLayerVisible(Editor, layer));
        eye.Opacity = layer.Visible ? 1 : 0.45;
        Button lockButton = Ui.IconButton(layer.Locked ? PixelEditorIcons.Lock : PixelEditorIcons.LockOpen, Strings.Layers_ToggleLock, () => Commands.EditorCommands.ToggleLayerLocked(Editor, layer), 12);
        lockButton.Opacity = layer.Locked ? 1 : 0.3;

        Control thumb;
        if (layer is GroupLayer group)
        {
            Button fold = Ui.IconButton(group.Collapsed ? PixelEditorIcons.ChevronRight : PixelEditorIcons.ChevronDown, Strings.Layers_ToggleGroup, () =>
            {
                group.Collapsed = !group.Collapsed;
                Editor.RefreshPanels();
            }, 10);
            thumb = Ui.Row(0, fold, new IconView(PixelEditorIcons.Folder, 16) { VerticalAlignment = VerticalAlignment.Center });
        }
        else
        {
            PixelBuffer? buffer = sprite.GetBuffer((PixelLayer)layer, sprite.ActiveFrame);
            Border frame = new() { Width = ThumbSize, Height = ThumbSize, Background = Ui.Checker(0xFF8C8C8C, 0xFFA6A6A6, 4), BorderBrush = active ? Brushes.White : Ui.Gutter, BorderThickness = new Thickness(1), ClipToBounds = true };
            if (buffer != null)
            {
                Image image = new() { Source = Thumbnail(buffer, ThumbSize - 2), Stretch = Stretch.Uniform };
                RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.None);
                frame.Child = image;
            }
            thumb = frame;
        }

        TextBlock name = new() { Text = layer.Name, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis, Foreground = active ? Brushes.White : Ui.TextBrush, Margin = new Thickness(8, 0, 4, 0), FontStyle = layer is PixelLayer { IsReference: true } ? FontStyle.Italic : FontStyle.Normal };
        StackPanel badges = new() { Orientation = Orientation.Horizontal, Spacing = 2, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
        if (layer is PixelLayer pixels)
        {
            if (pixels.IsReference) badges.Children.Add(Badge(PixelEditorIcons.Reference, Strings.Layers_ReferenceBadge));
            if (sprite.IsLinked(pixels, sprite.ActiveFrame)) badges.Children.Add(Badge(PixelEditorIcons.Link, Strings.Layers_LinkedBadge));
        }
        if (layer.Opacity < 255) badges.Children.Add(new TextBlock { Text = $"{layer.Opacity * 100 / 255}%", FontSize = 10, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center });

        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,Auto,Auto,*,Auto,Auto"), Height = 36 };
        Border indent = new() { Width = layer.Depth * 14 };
        Grid.SetColumn(indent, 1);
        Grid.SetColumn(thumb, 2);
        Grid.SetColumn(name, 3);
        Grid.SetColumn(badges, 4);
        Grid.SetColumn(lockButton, 5);
        thumb.VerticalAlignment = VerticalAlignment.Center;
        grid.Children.Add(eye);
        grid.Children.Add(indent);
        grid.Children.Add(thumb);
        grid.Children.Add(name);
        grid.Children.Add(badges);
        grid.Children.Add(lockButton);

        Border row = new()
        {
            Child = grid,
            Background = active ? Ui.RowSelected : Brushes.Transparent,
            BorderBrush = Ui.Gutter,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(2, 0)
        };
        row.PointerPressed += (_, e) =>
        {
            PointerPointProperties properties = e.GetCurrentPoint(row).Properties;
            int id = layer.Id;
            double startY = e.GetPosition(_list).Y;
            Editor.SetActiveLayer(layer);
            if (properties.IsLeftButtonPressed && e.ClickCount == 1)
            {
                // The list rebuilds when the active layer changes, so the list itself tracks the drag.
                _dragLayerId = id;
                _dragStartY = startY;
                _dragging = false;
                e.Pointer.Capture(_list);
            }
            if (properties.IsLeftButtonPressed && e.ClickCount == 2) Dispatcher.UIThread.Post(() => Editor.Execute("layer.properties"));
            else if (properties.IsRightButtonPressed) Dispatcher.UIThread.Post(() => BuildMenu().Open(Editor.Canvas));
        };
        row.PointerEntered += (_, _) =>
        {
            if (!active) row.Background = Ui.Hover;
        };
        row.PointerExited += (_, _) =>
        {
            if (!active) row.Background = Brushes.Transparent;
        };
        return row;
    }

    private static Control Badge(string icon, string tip)
    {
        IconView view = new(icon, 12) { Foreground = PhotoEditorTheme.AccentTextBrush, VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(view, tip);
        return view;
    }

    private ContextMenu BuildMenu()
    {
        ContextMenu menu = new() { Placement = PlacementMode.Pointer };
        foreach (string? id in new[] { "layer.properties", null, "layer.new", "layer.newGroup", "layer.duplicate", "layer.delete", null, "layer.mergeDown", "layer.flatten", null, "layer.reference", "layer.moveToTop", "layer.moveToBottom" })
        {
            if (id == null)
            {
                menu.Items.Add(new Separator());
                continue;
            }
            Commands.EditorCommand? command = Commands.CommandRegistry.Get(id);
            if (command == null) continue;
            MenuItem item = new() { Header = command.Text, IsEnabled = command.CanExecute?.Invoke(Editor) ?? true };
            if (command.IsChecked != null)
            {
                item.ToggleType = MenuItemToggleType.CheckBox;
                item.IsChecked = command.IsChecked(Editor);
            }
            string captured = id;
            item.Click += (_, _) => Editor.Execute(captured);
            menu.Items.Add(item);
        }
        return menu;
    }

    /// <summary>A small nearest-neighbor copy of a buffer for layer and cel thumbnails.</summary>
    public static WriteableBitmap Thumbnail(PixelBuffer buffer, int maxSize)
    {
        // Small sprites keep every pixel (the image control scales them); only large ones are reduced.
        maxSize = Math.Max(maxSize, 128);
        double scale = Math.Min(1, Math.Min(maxSize / (double)buffer.Width, maxSize / (double)buffer.Height));
        int w = Math.Max(1, (int)Math.Round(buffer.Width * scale)), h = Math.Max(1, (int)Math.Round(buffer.Height * scale));
        return Ui.ToBitmap(w == buffer.Width && h == buffer.Height ? buffer : buffer.ScaleNearest(w, h));
    }
}

/// <summary>Plays the animation at a fixed zoom while you draw.</summary>
public sealed class PreviewPanel : EditorPanel
{
    private readonly Image _image = new() { Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
    private readonly Border _frame = new() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, ClipToBounds = true };
    private readonly DispatcherTimer _timer = new();
    private readonly Button _play;
    private readonly TextBlock _info = new() { Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0) };
    private List<int> _sequence = [];
    private int _index, _frameShown = -1, _versionShown = -1;
    private bool _paused;
    private WriteableBitmap? _bitmap;
    private Sprite? _spriteShown;

    public PreviewPanel(PixelEditorWindow editor) : base(editor)
    {
        RenderOptions.SetBitmapInterpolationMode(_image, BitmapInterpolationMode.None);
        _frame.Child = _image;
        _frame.Background = Ui.Checker(0xFF8C8C8C, 0xFFA6A6A6, 4);
        _timer.Tick += (_, _) => Tick();
        _play = Ui.IconButton(PixelEditorIcons.Pause, Strings.Preview_PlayPause, () =>
        {
            _paused = !_paused;
            Refresh();
        });
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 1 };
        buttons.Children.Add(_play);
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.ZoomOut, Strings.Command_ZoomOut, () => Zoom(-1)));
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.ZoomIn, Strings.Command_ZoomIn, () => Zoom(1)));
        DockPanel bar = new() { Height = 26, Background = Ui.Deep };
        DockPanel.SetDock(buttons, Dock.Right);
        bar.Children.Add(buttons);
        bar.Children.Add(_info);
        DockPanel root = new();
        DockPanel.SetDock(bar, Dock.Bottom);
        root.Children.Add(bar);
        root.Children.Add(new Border { Background = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)), Child = _frame, ClipToBounds = true });
        Content = root;
        SizeChanged += (_, _) => Layout();
    }

    public override string Id => "preview";

    public override string Title => Strings.Panel_Preview;

    public void Stop() => _timer.Stop();

    private void Zoom(int direction)
    {
        // 0 fits the panel; positive values are fixed whole zoom levels.
        Editor.Settings.PreviewZoom = Math.Clamp(Editor.Settings.PreviewZoom + direction, 0, 16);
        Layout();
    }

    private void Layout()
    {
        Sprite? sprite = Sprite;
        if (sprite == null) return;
        double available = Math.Min((Bounds.Width - 12) / sprite.Width, (Bounds.Height - 38) / sprite.Height);
        double zoom = Editor.Settings.PreviewZoom > 0 ? Editor.Settings.PreviewZoom : available >= 1 ? Math.Floor(available) : Math.Max(0.05, available);
        _image.Width = sprite.Width * zoom;
        _image.Height = sprite.Height * zoom;
        _image.Stretch = Stretch.Fill;
        _frame.Width = _image.Width;
        _frame.Height = _image.Height;
        _info.Text = Editor.Settings.PreviewZoom > 0 ? $"{zoom * 100:0}%" : string.Format(Strings.Preview_Fit, $"{zoom * 100:0}%");
    }

    public override void Refresh()
    {
        Sprite? sprite = Sprite;
        _frame.IsVisible = sprite != null;
        ((IconView)_play.Content!).Icon = _paused ? PixelEditorIcons.Play : PixelEditorIcons.Pause;
        if (sprite == null)
        {
            _timer.Stop();
            _info.Text = string.Empty;
            return;
        }
        _sequence = SpriteOps.PlaybackSequence(sprite, Editor.Settings.PlayTagOnly);
        if (_paused || sprite.Frames.Count < 2)
        {
            _timer.Stop();
            Show(sprite, sprite.ActiveFrame);
        }
        else
        {
            if (_index >= _sequence.Count) _index = 0;
            Show(sprite, _sequence[_index]);
            if (!_timer.IsEnabled)
            {
                _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(10, sprite.Frames[_sequence[_index]].Duration));
                _timer.Start();
            }
        }
        Layout();
    }

    private void Tick()
    {
        Sprite? sprite = Sprite;
        if (sprite == null || _sequence.Count == 0 || !Editor.Panels.IsPanelShown(Id) || !Editor.Panels.IsVisible)
        {
            _timer.Stop();
            return;
        }
        _index = (_index + 1) % _sequence.Count;
        int frame = Math.Clamp(_sequence[_index], 0, sprite.Frames.Count - 1);
        Show(sprite, frame);
        _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(10, sprite.Frames[frame].Duration));
    }

    private void Show(Sprite sprite, int frame)
    {
        frame = Math.Clamp(frame, 0, sprite.Frames.Count - 1);
        if (ReferenceEquals(sprite, _spriteShown) && frame == _frameShown && sprite.Version == _versionShown) return;
        _spriteShown = sprite;
        _frameShown = frame;
        _versionShown = sprite.Version;
        PixelBuffer flat = Compositor.Flatten(sprite, frame);
        WriteableBitmap bitmap = Ui.ToBitmap(flat, _bitmap);
        _bitmap = bitmap;
        // Reassigning the source makes the image pick up the new pixels.
        _image.Source = null;
        _image.Source = bitmap;
    }
}
