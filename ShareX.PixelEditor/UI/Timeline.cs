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

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.PhotoEditor.UI.Theme;
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.UI;

/// <summary>The animation timeline: playback controls above a grid of layers (rows) and frames (columns) whose cells are the cels.</summary>
public sealed class Timeline : UserControl
{
    private readonly PixelEditorWindow _editor;
    private readonly TimelineGrid _grid;
    private readonly ScrollBar _horizontal = new() { Orientation = Orientation.Horizontal, Visibility = ScrollBarVisibility.Visible };
    private readonly ScrollBar _vertical = new() { Orientation = Orientation.Vertical, Visibility = ScrollBarVisibility.Visible };
    private readonly Button _play;
    private readonly NumericUpDown _duration = new() { Minimum = 1, Maximum = 65535, Increment = 10, FormatString = "0", Width = 56, VerticalAlignment = VerticalAlignment.Center };
    private readonly TextBlock _info = new() { Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0) };
    private readonly List<(Button Button, Func<bool> Checked)> _toggles = [];
    private bool _updating;

    public Timeline(PixelEditorWindow editor)
    {
        _editor = editor;
        _grid = new TimelineGrid(editor, this);

        StackPanel bar = new() { Orientation = Orientation.Horizontal, Spacing = 1, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0) };
        Button Command(string icon, string commandId)
        {
            EditorCommand? command = CommandRegistry.Get(commandId);
            string tip = command?.Text.Replace("...", string.Empty) ?? commandId;
            string? gesture = CommandRegistry.GetGesture(commandId, editor.Settings);
            Button button = Ui.IconButton(icon, gesture != null ? $"{tip} ({gesture})" : tip, () => editor.Execute(commandId));
            bar.Children.Add(button);
            return button;
        }
        void Toggle(string icon, string commandId, Func<bool> isChecked) => _toggles.Add((Command(icon, commandId), isChecked));
        void Gap() => bar.Children.Add(new Border { Classes = { "vsep" } });

        Command(PixelEditorIcons.FrameFirst, "frame.first");
        Command(PixelEditorIcons.FramePrevious, "frame.previous");
        _play = Command(PixelEditorIcons.Play, "frame.play");
        Command(PixelEditorIcons.FrameNext, "frame.next");
        Command(PixelEditorIcons.FrameLast, "frame.last");
        Gap();
        Command(PixelEditorIcons.NewFrame, "frame.new");
        Command(PixelEditorIcons.NewEmptyFrame, "frame.newEmpty");
        Command(PixelEditorIcons.Trash, "frame.delete");
        Command(PixelEditorIcons.Tag, "frame.newTag");
        Gap();
        Toggle(PixelEditorIcons.OnionSkin, "frame.onionSkin", () => editor.Settings.OnionSkin);
        Toggle(PixelEditorIcons.Loop, "frame.loop", () => editor.Settings.LoopPlayback);
        Toggle(PixelEditorIcons.PlayTag, "frame.playTagOnly", () => editor.Settings.PlayTagOnly);
        Toggle(PixelEditorIcons.Thumbnails, "view.timelineThumbnails", () => editor.Settings.TimelineThumbnails);
        Gap();
        bar.Children.Add(new TextBlock { Text = Strings.Timeline_Duration, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 5, 0) });
        bar.Children.Add(_duration);
        bar.Children.Add(new TextBlock { Text = "ms", VerticalAlignment = VerticalAlignment.Center, Classes = { "dim" }, Margin = new Thickness(4, 0) });
        ToolTip.SetTip(_duration, Strings.Timeline_Duration_Tip);
        _duration.ValueChanged += (_, e) =>
        {
            if (_updating || e.NewValue is not decimal value || editor.Sprite is not Sprite sprite) return;
            EditorCommands.SetFrameDuration(editor, sprite.RangeFrom, sprite.RangeTo, (int)value);
        };

        // The frame count on the right gives way when the buttons need the room.
        Grid top = new() { Height = 30, Background = Ui.Chrome, ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        bar.ClipToBounds = true;
        Grid.SetColumn(_info, 1);
        top.Children.Add(bar);
        top.Children.Add(_info);
        top.SizeChanged += (_, _) => FitInfo(top, bar);
        _info.PropertyChanged += (_, e) =>
        {
            if (e.Property == TextBlock.TextProperty) Avalonia.Threading.Dispatcher.UIThread.Post(() => FitInfo(top, bar));
        };

        _horizontal.ValueChanged += (_, _) => _grid.InvalidateVisual();
        _vertical.ValueChanged += (_, _) => _grid.InvalidateVisual();
        Grid body = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), RowDefinitions = new RowDefinitions("*,Auto") };
        Grid.SetColumn(_vertical, 1);
        Grid.SetRow(_horizontal, 1);
        body.Children.Add(_grid);
        body.Children.Add(_vertical);
        body.Children.Add(_horizontal);

        DockPanel root = new() { Background = Ui.Deep };
        DockPanel.SetDock(top, Dock.Top);
        root.Children.Add(top);
        root.Children.Add(body);
        Content = root;
        _grid.SizeChanged += (_, _) => UpdateScrollBars();
    }

    private void FitInfo(Grid top, StackPanel bar)
    {
        double needed = bar.Margin.Left + bar.Margin.Right + bar.Children.Sum(c => c.IsVisible ? c.DesiredSize.Width : 0) + bar.Spacing * Math.Max(0, bar.Children.Count - 1);
        _info.IsVisible = top.Bounds.Width >= needed + _info.DesiredSize.Width + 4 || !_info.IsVisible && top.Bounds.Width >= needed + 160;
    }

    public double ScrollX => _horizontal.Value;

    public double ScrollY => _vertical.Value;

    public void ScrollBy(double dx, double dy)
    {
        _horizontal.Value = Math.Clamp(_horizontal.Value + dx, 0, _horizontal.Maximum);
        _vertical.Value = Math.Clamp(_vertical.Value + dy, 0, _vertical.Maximum);
    }

    private void UpdateScrollBars()
    {
        Sprite? sprite = _editor.Sprite;
        int cell = _editor.Settings.TimelineCellSize;
        double contentW = (sprite?.Frames.Count ?? 0) * cell, contentH = (sprite?.Root.Rows().Count() ?? 0) * cell;
        double viewW = Math.Max(0, _grid.Bounds.Width - TimelineGrid.HeaderWidth), viewH = Math.Max(0, _grid.Bounds.Height - TimelineGrid.TopHeight);
        _horizontal.Maximum = Math.Max(0, contentW - viewW);
        _horizontal.ViewportSize = viewW;
        _horizontal.LargeChange = Math.Max(cell, viewW - cell);
        _horizontal.SmallChange = cell;
        _vertical.Maximum = Math.Max(0, contentH - viewH);
        _vertical.ViewportSize = viewH;
        _vertical.LargeChange = Math.Max(cell, viewH - cell);
        _vertical.SmallChange = cell;
        _horizontal.Value = Math.Clamp(_horizontal.Value, 0, _horizontal.Maximum);
        _vertical.Value = Math.Clamp(_vertical.Value, 0, _vertical.Maximum);
    }

    public void ScrollToFrame(int frame)
    {
        int cell = _editor.Settings.TimelineCellSize;
        double viewW = Math.Max(cell, _grid.Bounds.Width - TimelineGrid.HeaderWidth), x = frame * cell;
        if (x < _horizontal.Value) _horizontal.Value = x;
        else if (x + cell > _horizontal.Value + viewW) _horizontal.Value = Math.Min(_horizontal.Maximum, x + cell - viewW);
    }

    public void InvalidatePlayhead() => _grid.InvalidateVisual();

    public void Refresh()
    {
        Sprite? sprite = _editor.Sprite;
        UpdateScrollBars();
        ((IconView)_play.Content!).Icon = _editor.IsPlaying ? PixelEditorIcons.Pause : PixelEditorIcons.Play;
        foreach ((Button button, Func<bool> isChecked) in _toggles) button.Classes.Set("checked", isChecked());
        _updating = true;
        _duration.IsEnabled = sprite != null;
        if (sprite != null)
        {
            _duration.Value = sprite.Frames[sprite.ActiveFrame].Duration;
            int total = sprite.TotalDuration;
            string range = sprite.RangeFrom != sprite.RangeTo ? string.Format(Strings.Timeline_Range, sprite.RangeFrom + 1, sprite.RangeTo + 1) + "   " : string.Empty;
            _info.Text = range + string.Format(Strings.Timeline_Info, sprite.Frames.Count, total / 1000.0, sprite.Frames.Count * 1000.0 / Math.Max(1, total));
        }
        else
        {
            _info.Text = string.Empty;
        }
        _updating = false;
        _grid.InvalidateVisual();
    }
}

/// <summary>Draws the timeline grid and handles clicks on cels, frame numbers, tags and layer rows.</summary>
public sealed class TimelineGrid : Control
{
    public const double HeaderWidth = 176;
    private const double TagHeight = 16, NumberHeight = 20;
    public const double TopHeight = TagHeight + NumberHeight;

    private static readonly Typeface Font = new("Segoe UI");
    private static readonly Typeface BoldFont = new("Segoe UI", FontStyle.Normal, FontWeight.SemiBold);
    private static readonly IBrush RowLine = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
    private static readonly IBrush CellEven = new SolidColorBrush(Color.FromRgb(0x2B, 0x2B, 0x2B));
    private static readonly IBrush CellOdd = new SolidColorBrush(Color.FromRgb(0x2F, 0x2F, 0x2F));
    private static readonly IBrush HeaderBrush = new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32));
    private static readonly IBrush RangeBrush = new SolidColorBrush(Color.FromRgb(0x45, 0x45, 0x45));
    private static readonly IBrush CelBrush = new SolidColorBrush(Color.FromRgb(0xC8, 0xC8, 0xC8));
    private static readonly IBrush EmptyBrush = new SolidColorBrush(Color.FromRgb(0x4A, 0x4A, 0x4A));

    private enum DragMode
    {
        None,
        Range,
        Frames,
        Cels
    }

    private const double DragThreshold = 4;

    private static readonly IBrush ThumbBackground = new SolidColorBrush(Color.FromRgb(0x3C, 0x3C, 0x3C));
    private readonly Dictionary<PixelBuffer, WriteableBitmap> _thumbs = new(ReferenceEqualityComparer.Instance);
    private Sprite? _thumbSprite;
    private object? _thumbState;

    private readonly PixelEditorWindow _editor;
    private readonly Timeline _timeline;
    private DragMode _drag;
    private Point _pressPoint;
    private bool _dragMoved;
    private int _pressFrame, _grabOffset, _dropFrame, _dropRow;

    public TimelineGrid(PixelEditorWindow editor, Timeline timeline)
    {
        _editor = editor;
        _timeline = timeline;
        ClipToBounds = true;
        Focusable = false;
    }

    private int Cell => _editor.Settings.TimelineCellSize;

    private static FormattedText Text(string text, IBrush brush, double size = 11, bool bold = false) =>
        new(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, bold ? BoldFont : Font, size, brush);

    public override void Render(DrawingContext context)
    {
        Rect bounds = new(Bounds.Size);
        context.FillRectangle(Ui.Deep, bounds);
        Sprite? sprite = _editor.Sprite;
        if (sprite == null) return;
        int cell = Cell;
        double scrollX = _timeline.ScrollX, scrollY = _timeline.ScrollY;
        List<Layer> rows = sprite.Root.Rows().ToList();
        int firstFrame = Math.Max(0, (int)(scrollX / cell)), lastFrame = Math.Min(sprite.Frames.Count - 1, (int)((scrollX + bounds.Width - HeaderWidth) / cell));
        int shown = _editor.View?.ShownFrame ?? sprite.ActiveFrame;
        IBrush accent = PhotoEditorTheme.AccentBorderBrush;
        bool thumbnails = _editor.Settings.TimelineThumbnails && cell >= 18;
        if (thumbnails) PruneThumbnails(sprite);

        // Cels
        using (context.PushClip(new Rect(HeaderWidth, TopHeight, Math.Max(0, bounds.Width - HeaderWidth), Math.Max(0, bounds.Height - TopHeight))))
        {
            for (int r = 0; r < rows.Count; r++)
            {
                double y = TopHeight + r * cell - scrollY;
                if (y + cell < TopHeight || y > bounds.Height) continue;
                Layer layer = rows[r];
                bool activeRow = layer == sprite.ActiveLayer;
                for (int f = firstFrame; f <= lastFrame; f++)
                {
                    double x = HeaderWidth + f * cell - scrollX;
                    Rect rect = new(x, y, cell - 1, cell - 1);
                    bool inRange = f >= sprite.RangeFrom && f <= sprite.RangeTo;
                    context.FillRectangle(activeRow && inRange ? RangeBrush : f % 2 == 0 ? CellEven : CellOdd, rect);
                    Point center = rect.Center;
                    if (layer is PixelLayer pixels)
                    {
                        Cel? cel = pixels.Cels[f];
                        if (cel == null)
                        {
                            context.DrawEllipse(null, new Pen(EmptyBrush, 1), center, 2.5, 2.5);
                        }
                        else if (thumbnails)
                        {
                            // A picture of the cel; linked cels are joined by a bar under their pictures.
                            bool linkPrevious = f > 0 && pixels.Cels[f - 1]?.Image == cel.Image, linkNext = f < pixels.Cels.Count - 1 && pixels.Cels[f + 1]?.Image == cel.Image;
                            Rect box = rect.Deflate(3);
                            context.FillRectangle(ThumbBackground, box);
                            WriteableBitmap thumb = Thumbnail(sprite, cel.Buffer, (int)box.Width, (int)box.Height);
                            Rect image = new(box.X + (box.Width - thumb.PixelSize.Width) / 2, box.Y + (box.Height - thumb.PixelSize.Height) / 2, thumb.PixelSize.Width, thumb.PixelSize.Height);
                            using (context.PushOpacity(layer.IsEffectivelyVisible ? 1 : 0.4))
                            using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = BitmapInterpolationMode.None }))
                            {
                                context.DrawImage(thumb, image);
                            }
                            if (linkPrevious) context.FillRectangle(CelBrush, new Rect(x - 1, rect.Bottom - 2, cell / 2.0 + 1, 2));
                            if (linkNext) context.FillRectangle(CelBrush, new Rect(center.X, rect.Bottom - 2, cell / 2.0, 2));
                        }
                        else
                        {
                            // Linked cels are joined by a line through their dots.
                            bool linkPrevious = f > 0 && pixels.Cels[f - 1]?.Image == cel.Image, linkNext = f < pixels.Cels.Count - 1 && pixels.Cels[f + 1]?.Image == cel.Image;
                            IBrush dot = layer.IsEffectivelyVisible ? CelBrush : EmptyBrush;
                            if (linkPrevious) context.FillRectangle(dot, new Rect(x - 1, center.Y - 1, cell / 2.0 + 1, 2));
                            if (linkNext) context.FillRectangle(dot, new Rect(center.X, center.Y - 1, cell / 2.0, 2));
                            context.DrawEllipse(dot, null, center, 4.5, 4.5);
                        }
                    }
                    else
                    {
                        context.FillRectangle(EmptyBrush, new Rect(x + 3, center.Y - 0.5, cell - 7, 1));
                    }
                    if (activeRow && f == sprite.ActiveFrame) context.DrawRectangle(null, new Pen(accent, 2), rect.Deflate(1));
                }
            }
            // Where dragged cels will land.
            if (_drag == DragMode.Cels && _dragMoved && _dropRow >= 0)
            {
                int count = sprite.RangeTo - sprite.RangeFrom + 1;
                double y = TopHeight + _dropRow * cell - scrollY;
                double x = HeaderWidth + _dropFrame * cell - scrollX;
                bool valid = _dropRow < rows.Count && rows[_dropRow] is PixelLayer;
                context.DrawRectangle(new SolidColorBrush(PhotoEditorTheme.AccentBrush.Color, 0.25), new Pen(valid ? accent : Brushes.IndianRed, 2), new Rect(x + 1, y + 1, count * cell - 3, cell - 3));
            }
            // Playhead while the animation runs.
            if (_editor.IsPlaying)
            {
                double x = HeaderWidth + shown * cell - scrollX;
                context.FillRectangle(new SolidColorBrush(PhotoEditorTheme.AccentBrush.Color, 0.25), new Rect(x, TopHeight, cell - 1, bounds.Height));
            }
        }

        // Frame numbers and tags
        using (context.PushClip(new Rect(HeaderWidth, 0, Math.Max(0, bounds.Width - HeaderWidth), TopHeight)))
        {
            context.FillRectangle(HeaderBrush, new Rect(HeaderWidth, 0, bounds.Width, TopHeight));
            for (int f = firstFrame; f <= lastFrame; f++)
            {
                double x = HeaderWidth + f * cell - scrollX;
                bool active = f == sprite.ActiveFrame, inRange = f >= sprite.RangeFrom && f <= sprite.RangeTo;
                if (inRange) context.FillRectangle(active ? accent : RangeBrush, new Rect(x, TagHeight, cell - 1, NumberHeight - 1));
                if (_editor.IsPlaying && f == shown) context.FillRectangle(new SolidColorBrush(PhotoEditorTheme.AccentBrush.Color, 0.5), new Rect(x, TagHeight, cell - 1, NumberHeight - 1));
                FormattedText number = Text((f + 1).ToString(CultureInfo.CurrentCulture), active ? Brushes.White : Ui.DimBrush, 11, active);
                context.DrawText(number, new Point(x + (cell - 1 - number.Width) / 2, TagHeight + (NumberHeight - number.Height) / 2));
            }
            foreach (Tag tag in sprite.Tags)
            {
                double x0 = HeaderWidth + tag.From * cell - scrollX, x1 = HeaderWidth + (tag.To + 1) * cell - scrollX - 1;
                Color color = Argb.ToAvalonia(tag.Color);
                Rect band = new(x0, 2, Math.Max(4, x1 - x0), TagHeight - 3);
                context.DrawRectangle(new SolidColorBrush(color), null, new RoundedRect(band, 2));
                FormattedText name = Text(tag.Name, Argb.Luma(tag.Color) > 150 ? Brushes.Black : Brushes.White, 10);
                using (context.PushClip(band))
                {
                    context.DrawText(name, new Point(Math.Max(band.X, HeaderWidth) + 4, band.Y + (band.Height - name.Height) / 2));
                }
            }
        }

        // Layer names
        using (context.PushClip(new Rect(0, TopHeight, HeaderWidth, Math.Max(0, bounds.Height - TopHeight))))
        {
            for (int r = 0; r < rows.Count; r++)
            {
                double y = TopHeight + r * cell - scrollY;
                if (y + cell < TopHeight || y > bounds.Height) continue;
                Layer layer = rows[r];
                bool active = layer == sprite.ActiveLayer;
                context.FillRectangle(active ? Ui.RowSelected : HeaderBrush, new Rect(0, y, HeaderWidth - 1, cell - 1));
                double middle = y + (cell - 1) / 2.0;
                // Visibility: a filled dot when shown, a ring when hidden. Lock: a small padlock when locked.
                if (layer.Visible) context.DrawEllipse(Ui.TextBrush, null, new Point(11, middle), 3.5, 3.5);
                else context.DrawEllipse(null, new Pen(EmptyBrush, 1), new Point(11, middle), 3.5, 3.5);
                if (layer.Locked)
                {
                    context.FillRectangle(Ui.TextBrush, new Rect(24, middle - 1, 8, 6));
                    context.DrawEllipse(null, new Pen(Ui.TextBrush, 1.4), new Point(28, middle - 1.5), 2.3, 3);
                }
                else
                {
                    context.DrawRectangle(null, new Pen(EmptyBrush, 1), new Rect(24.5, middle - 0.5, 7, 5));
                }
                double indent = 42 + layer.Depth * 10;
                string label = layer is GroupLayer group ? (group.Collapsed ? "▸ " : "▾ ") + layer.Name : layer.Name;
                FormattedText name = Text(label, active ? Brushes.White : layer.IsEffectivelyVisible ? Ui.TextBrush : Ui.DimBrush, 12);
                using (context.PushClip(new Rect(indent, y, HeaderWidth - indent - 6, cell)))
                {
                    context.DrawText(name, new Point(indent, middle - name.Height / 2));
                }
            }
        }
        context.FillRectangle(HeaderBrush, new Rect(0, 0, HeaderWidth - 1, TopHeight));
        FormattedText corner = Text(Strings.Timeline_Layers, Ui.DimBrush, 11);
        context.DrawText(corner, new Point(10, TagHeight + (NumberHeight - corner.Height) / 2));
        context.FillRectangle(RowLine, new Rect(HeaderWidth - 1, 0, 1, bounds.Height));
        context.FillRectangle(RowLine, new Rect(0, TopHeight - 1, bounds.Width, 1));
        // Where dragged frames will be inserted.
        if (_drag == DragMode.Frames && _dragMoved)
        {
            double x = HeaderWidth + _dropFrame * cell - scrollX - 1;
            if (x >= HeaderWidth - 2) context.FillRectangle(accent, new Rect(x - 1, TagHeight, 3, bounds.Height - TagHeight));
        }
    }

    /// <summary>Drops cached cel pictures once the sprite changes; they are made again as cels are drawn.</summary>
    private void PruneThumbnails(Sprite sprite)
    {
        object? state = sprite.History.CurrentState;
        if (ReferenceEquals(sprite, _thumbSprite) && ReferenceEquals(state, _thumbState) && _thumbs.Count < 2000) return;
        _thumbs.Clear();
        _thumbSprite = sprite;
        _thumbState = state;
    }

    private WriteableBitmap Thumbnail(Sprite sprite, PixelBuffer buffer, int width, int height)
    {
        if (_thumbs.TryGetValue(buffer, out WriteableBitmap? cached)) return cached;
        double scale = Math.Min(width / (double)sprite.Width, height / (double)sprite.Height);
        int w = Math.Clamp((int)Math.Round(sprite.Width * scale), 1, Math.Max(1, width)), h = Math.Clamp((int)Math.Round(sprite.Height * scale), 1, Math.Max(1, height));
        WriteableBitmap bitmap = Ui.ToBitmap(Shrink(buffer, w, h));
        _thumbs[buffer] = bitmap;
        return bitmap;
    }

    /// <summary>Scales a cel down for a thumbnail. Each thumbnail pixel takes the most opaque pixel of its block, so single-pixel details stay visible.</summary>
    private static PixelBuffer Shrink(PixelBuffer source, int width, int height)
    {
        if (width >= source.Width || height >= source.Height || (long)source.Width * source.Height > (long)width * height * 64) return source.ScaleNearest(width, height);
        PixelBuffer result = new(width, height);
        for (int y = 0; y < height; y++)
        {
            int y0 = y * source.Height / height, y1 = Math.Max(y0 + 1, (y + 1) * source.Height / height);
            for (int x = 0; x < width; x++)
            {
                int x0 = x * source.Width / width, x1 = Math.Max(x0 + 1, (x + 1) * source.Width / width);
                uint best = 0;
                for (int sy = y0; sy < y1; sy++)
                {
                    for (int sx = x0; sx < x1; sx++)
                    {
                        uint p = source.Pixels[sy * source.Width + sx];
                        if (p >> 24 > best >> 24) best = p;
                    }
                }
                result.Pixels[y * width + x] = best;
            }
        }
        return result;
    }

    private (int Row, int Frame) Hit(Point p)
    {
        int cell = Cell;
        int row = p.Y < TopHeight ? -1 : (int)((p.Y - TopHeight + _timeline.ScrollY) / cell);
        int frame = p.X < HeaderWidth ? -1 : (int)((p.X - HeaderWidth + _timeline.ScrollX) / cell);
        return (row, frame);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Sprite? sprite = _editor.Sprite;
        if (sprite == null || _editor.IsBusy) return;
        Point p = e.GetPosition(this);
        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        (int row, int frame) = Hit(p);
        List<Layer> rows = sprite.Root.Rows().ToList();
        Layer? layer = row >= 0 && row < rows.Count ? rows[row] : null;
        bool validFrame = frame >= 0 && frame < sprite.Frames.Count;
        bool right = properties.IsRightButtonPressed, shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

        if (p.X < HeaderWidth)
        {
            if (layer == null) return;
            if (p.X < 21 && !right)
            {
                EditorCommands.ToggleLayerVisible(_editor, layer);
                return;
            }
            if (p.X < 38 && !right)
            {
                EditorCommands.ToggleLayerLocked(_editor, layer);
                return;
            }
            _editor.SetActiveLayer(layer);
            if (layer is GroupLayer group && e.ClickCount == 2)
            {
                group.Collapsed = !group.Collapsed;
                _editor.RefreshPanels();
            }
            else if (e.ClickCount == 2) Dispatcher.UIThread.Post(() => _editor.Execute("layer.properties"));
            else if (right) Dispatcher.UIThread.Post(() => OpenMenu(["layer.properties", "", "layer.new", "layer.newGroup", "layer.duplicate", "layer.delete", "", "layer.mergeDown", "layer.flatten"]));
            return;
        }
        if (!validFrame) return;

        if (p.Y < TagHeight)
        {
            Tag? tag = sprite.Tags.Where(t => frame >= t.From && frame <= t.To).OrderBy(t => t.To - t.From).FirstOrDefault();
            if (tag == null) return;
            _editor.CommitFloating();
            sprite.SetRange(tag.From, tag.To, Math.Clamp(sprite.ActiveFrame, tag.From, tag.To));
            _editor.GoToFrame(sprite.ActiveFrame, false);
            sprite.SetRange(tag.From, tag.To, sprite.ActiveFrame);
            _editor.RefreshTimeline();
            if (e.ClickCount == 2) Dispatcher.UIThread.Post(() => _editor.Execute("frame.tagProperties"));
            else if (right) Dispatcher.UIThread.Post(() => OpenMenu(["frame.tagProperties", "frame.removeTag", "", "frame.reverse", "file.export"]));
            return;
        }

        bool insideRange = frame >= sprite.RangeFrom && frame <= sprite.RangeTo;
        bool onHeader = p.Y < TopHeight;
        // Pressing on the selected frames, or on a cel of them in the active layer, may start dragging them; a click without a drag still selects.
        bool canDrag = !right && !shift && insideRange && (onHeader || layer is PixelLayer pixels && layer == sprite.ActiveLayer && pixels.Cels[frame] != null);
        if (layer != null && !canDrag) _editor.SetActiveLayer(layer);
        if (canDrag)
        {
            _drag = onHeader ? DragMode.Frames : DragMode.Cels;
            _pressPoint = p;
            _pressFrame = frame;
            _grabOffset = frame - sprite.RangeFrom;
            _dragMoved = false;
            _dropRow = -1;
            e.Pointer.Capture(this);
            return;
        }
        // A right-click inside the selected range keeps the range for the menu.
        if (!(right && insideRange)) _editor.GoToFrame(frame, shift);
        if (right)
        {
            string[] items = p.Y < TopHeight
                ? ["frame.new", "frame.newEmpty", "frame.delete", "", "frame.copy", "frame.paste", "", "frame.properties", "frame.reverse", "frame.moveLeft", "frame.moveRight", "", "frame.newTag"]
                : ["frame.new", "frame.newEmpty", "frame.delete", "", "cel.clear", "cel.link", "cel.unlink", "cel.properties", "", "frame.properties", "frame.newTag"];
            Dispatcher.UIThread.Post(() => OpenMenu(items));
            return;
        }
        if (e.ClickCount == 2 && p.Y < TopHeight)
        {
            Dispatcher.UIThread.Post(() => _editor.Execute("frame.properties"));
            return;
        }
        _drag = DragMode.Range;
        e.Pointer.Capture(this);
    }

    private void OpenMenu(string[] ids)
    {
        ContextMenu menu = new() { Placement = PlacementMode.Pointer };
        foreach (string id in ids)
        {
            if (id.Length == 0)
            {
                menu.Items.Add(new Separator());
                continue;
            }
            EditorCommand? command = CommandRegistry.Get(id);
            if (command == null) continue;
            MenuItem item = new() { Header = command.Text, IsEnabled = command.CanExecute?.Invoke(_editor) ?? true };
            string? gesture = CommandRegistry.GetGesture(id, _editor.Settings);
            if (gesture != null) item.InputGesture = CommandRegistry.ParseGesture(gesture);
            item.Click += (_, _) => _editor.Execute(id);
            menu.Items.Add(item);
        }
        menu.Open(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        Sprite? sprite = _editor.Sprite;
        if (_drag == DragMode.None || sprite == null) return;
        Point p = e.GetPosition(this);
        if (_drag == DragMode.Range)
        {
            (_, int frame) = Hit(p);
            frame = Math.Clamp(frame, 0, sprite.Frames.Count - 1);
            if (frame != sprite.ActiveFrame) _editor.GoToFrame(frame, true);
            return;
        }
        if (!_dragMoved && Math.Abs(p.X - _pressPoint.X) < DragThreshold && Math.Abs(p.Y - _pressPoint.Y) < DragThreshold) return;
        _dragMoved = true;
        Cursor = new Cursor(StandardCursorType.DragMove);
        int cell = Cell;
        if (_drag == DragMode.Frames)
        {
            // The gap between frames nearest to the pointer.
            _dropFrame = Math.Clamp((int)Math.Round((p.X - HeaderWidth + _timeline.ScrollX) / cell), 0, sprite.Frames.Count);
        }
        else
        {
            (int row, int frame) = Hit(p);
            int count = sprite.RangeTo - sprite.RangeFrom + 1;
            _dropFrame = Math.Clamp(frame - _grabOffset, 0, Math.Max(0, sprite.Frames.Count - count));
            _dropRow = Math.Clamp(row, 0, Math.Max(0, sprite.Root.Rows().Count() - 1));
        }
        AutoScroll(p);
        InvalidateVisual();
    }

    /// <summary>Scrolls the timeline while a drag nears its edges.</summary>
    private void AutoScroll(Point p)
    {
        double dx = p.X < HeaderWidth + 8 ? -Cell / 2.0 : p.X > Bounds.Width - 8 ? Cell / 2.0 : 0;
        double dy = p.Y < TopHeight + 4 && p.Y > TopHeight - 2 ? -Cell / 2.0 : p.Y > Bounds.Height - 6 ? Cell / 2.0 : 0;
        if (dx != 0 || dy != 0) _timeline.ScrollBy(dx, dy);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        DragMode mode = _drag;
        bool moved = _dragMoved;
        _drag = DragMode.None;
        _dragMoved = false;
        Cursor = null;
        e.Pointer.Capture(null);
        Sprite? sprite = _editor.Sprite;
        if (sprite == null || mode is DragMode.None or DragMode.Range) return;
        if (!moved)
        {
            // A click on the selected frames without dragging selects just that frame.
            _editor.GoToFrame(_pressFrame, false);
            InvalidateVisual();
            return;
        }
        bool copy = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (mode == DragMode.Frames) DropFrames(sprite, copy);
        else DropCels(sprite, copy);
        InvalidateVisual();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (_drag is DragMode.Frames or DragMode.Cels)
        {
            _drag = DragMode.None;
            _dragMoved = false;
            Cursor = null;
            InvalidateVisual();
        }
        else _drag = DragMode.None;
    }

    private void DropFrames(Sprite sprite, bool copy)
    {
        int from = sprite.RangeFrom, to = sprite.RangeTo, count = to - from + 1, gap = _dropFrame;
        _editor.CommitFloating();
        _editor.StopPlayback();
        if (copy)
        {
            SpriteOps.CopyFrames(sprite, from, to, gap);
            _editor.GoToFrame(sprite.ActiveFrame, false);
            sprite.SetRange(gap, gap + count - 1, gap);
            _editor.Commit(Strings.History_CopyFrames);
            return;
        }
        if (gap >= from && gap <= to + 1) return;
        sprite.MoveFrames(from, to, gap > to ? gap - count : gap);
        int start = sprite.RangeFrom;
        _editor.GoToFrame(start, false);
        sprite.SetRange(start, start + count - 1, start);
        _editor.Commit(Strings.History_MoveFrames);
    }

    private void DropCels(Sprite sprite, bool copy)
    {
        List<Layer> rows = sprite.Root.Rows().ToList();
        if (sprite.ActiveLayer is not PixelLayer source || _dropRow < 0 || _dropRow >= rows.Count || rows[_dropRow] is not PixelLayer target) return;
        if (target == source && _dropFrame == sprite.RangeFrom) return;
        if (target.IsEffectivelyLocked || !copy && source.IsEffectivelyLocked)
        {
            _editor.SetStatus(Strings.Status_LayerLocked);
            return;
        }
        int from = sprite.RangeFrom, to = sprite.RangeTo, count = to - from + 1;
        _editor.CommitFloating();
        _editor.StopPlayback();
        SpriteOps.MoveCels(sprite, source, from, to, target, _dropFrame, copy);
        int start = Math.Clamp(_dropFrame, 0, sprite.Frames.Count - 1);
        _editor.SetActiveLayer(target);
        _editor.GoToFrame(start, false);
        sprite.SetRange(start, Math.Min(sprite.Frames.Count - 1, start + count - 1), start);
        _editor.Commit(copy ? Strings.History_CopyCels : Strings.History_MoveCels);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) _timeline.ScrollBy(-e.Delta.Y * Cell * 2, 0);
        else _timeline.ScrollBy(-e.Delta.X * Cell * 2, -e.Delta.Y * Cell);
        e.Handled = true;
    }
}
