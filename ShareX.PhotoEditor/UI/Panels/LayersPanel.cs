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
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Theme;

namespace ShareX.PhotoEditor.UI.Panels;

/// <summary>Layers panel: visibility, thumbnails, masks, selection, renaming, drag-and-drop ordering and layer options.</summary>
public sealed class LayersPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();
    private readonly ComboBox _blend = new() { MinWidth = 110, HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly NumericUpDown _opacity = DialogsNumber();
    private readonly NumericUpDown _fill = DialogsNumber();
    private readonly StackPanel _locks = new() { Orientation = Orientation.Horizontal, Spacing = 0 };
    private readonly Border _dropIndicator = new() { Height = 2, Background = PhotoEditorTheme.AccentBrush, IsVisible = false, IsHitTestVisible = false };
    private bool _updating;
    private Layer? _dragLayer;
    private Point _dragStart;
    private bool _dragging;
    private (Layer Target, int Position)? _drop;
    private readonly Dictionary<Layer, RowParts> _rows = [];
    private static readonly IBrush ThumbFrame = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));

    /// <summary>Thumbnails of frozen (immutable) surfaces, so unchanged layers are not re-rendered on every refresh.</summary>
    private sealed record CachedThumb((int, int, int, int) Key, Avalonia.Media.Imaging.Bitmap Image);

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<PixelSurface, CachedThumb> LayerThumbs = new();
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<PixelSurface, CachedThumb> MaskThumbs = new();

    private sealed record RowParts(Border Row, TextBlock Name, Border Thumbnail, Border? Mask);

    private static NumericUpDown DialogsNumber() => new() { Minimum = 0, Maximum = 100, Increment = 1, FormatString = "0", Width = 42, ShowButtonSpinner = false };

    public LayersPanel(PhotoEditorWindow editor) : base(editor)
    {
        foreach (LayerBlendMode mode in BlendModes.LayerModes.Prepend(LayerBlendMode.PassThrough)) _blend.Items.Add(new ComboBoxItem { Content = BlendModes.GetName(mode), Tag = mode });
        _blend.SelectionChanged += (_, _) =>
        {
            if (_updating || Doc?.ActiveLayer is not Layer layer || _blend.SelectedItem is not ComboBoxItem { Tag: LayerBlendMode mode }) return;
            if (mode == LayerBlendMode.PassThrough && layer is not GroupLayer) return;
            layer.BlendMode = mode;
            Editor.Commit(Strings.LayersPanel_BlendingChange);
        };
        _opacity.ValueChanged += (_, e) =>
        {
            if (_updating || Doc?.ActiveLayer is not Layer layer || e.NewValue is not decimal v) return;
            layer.Opacity = (float)v / 100;
            Editor.Commit(Strings.LayersPanel_OpacityChange);
        };
        _fill.ValueChanged += (_, e) =>
        {
            if (_updating || Doc?.ActiveLayer is not Layer layer || e.NewValue is not decimal v) return;
            layer.FillOpacity = (float)v / 100;
            Editor.Commit(Strings.LayersPanel_FillOpacityChange);
        };

        StackPanel header = new() { Spacing = 5, Margin = new Thickness(8, 6, 8, 6) };
        Grid blendRow = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto,Auto"), ColumnSpacing = 4 };
        TextBlock opacityLabel = Text(Strings.EditorCommands_OpacityColon);
        TextBlock opacityUnit = new() { Text = "%", Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(opacityLabel, 1);
        Grid.SetColumn(_opacity, 2);
        Grid.SetColumn(opacityUnit, 3);
        blendRow.Children.Add(_blend);
        blendRow.Children.Add(opacityLabel);
        blendRow.Children.Add(_opacity);
        blendRow.Children.Add(opacityUnit);
        header.Children.Add(blendRow);
        Grid lockRow = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,Auto,Auto"), ColumnSpacing = 4 };
        TextBlock lockLabel = Text(Strings.LayersPanel_LockColon);
        TextBlock fillLabel = Text(Strings.PropertiesPanel_FillColon);
        TextBlock fillUnit = new() { Text = "%", Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(_locks, 1);
        Grid.SetColumn(fillLabel, 2);
        Grid.SetColumn(_fill, 3);
        Grid.SetColumn(fillUnit, 4);
        lockRow.Children.Add(lockLabel);
        lockRow.Children.Add(_locks);
        lockRow.Children.Add(fillLabel);
        lockRow.Children.Add(_fill);
        lockRow.Children.Add(fillUnit);
        header.Children.Add(lockRow);

        StackPanel footer = new() { Orientation = Orientation.Horizontal, Spacing = 2, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(4, 2) };
        footer.Children.Add(IconButton(PhotoEditorIcons.Link, Strings.LayersPanel_LinkTip, () => EditorCommands.ToggleLinkLayers(Editor)));
        footer.Children.Add(IconButton(PhotoEditorIcons.Effects, Strings.LayersPanel_AddLayerStyleTip, () => _ = EditorCommands.LayerStyle(Editor)));
        footer.Children.Add(IconButton(PhotoEditorIcons.Mask, Strings.LayersPanel_AddLayerMaskTip, () => EditorCommands.AddMask(Editor, Doc?.Selection != null ? MaskMode.RevealSelection : MaskMode.RevealAll)));
        Button adjust = IconButton(PhotoEditorIcons.Adjustment, Strings.LayersPanel_NewFillOrAdjustmentTip, () => { });
        adjust.Flyout = EditorCommands.CreateAdjustmentFlyout(Editor);
        footer.Children.Add(adjust);
        footer.Children.Add(IconButton(PhotoEditorIcons.Folder, Strings.LayersPanel_NewGroupTip, () => EditorCommands.NewGroup(Editor)));
        footer.Children.Add(IconButton(PhotoEditorIcons.NewLayer, Strings.LayersPanel_NewLayerTip, () => EditorCommands.NewLayer(Editor)));
        footer.Children.Add(IconButton(PhotoEditorIcons.Trash, Strings.LayersPanel_DeleteLayerTip, () => EditorCommands.DeleteLayers(Editor)));

        Grid listHost = new();
        listHost.Children.Add(_list);
        listHost.Children.Add(new Canvas { IsHitTestVisible = false, Children = { _dropIndicator } });

        DockPanel root = new();
        Border headerHost = new() { Child = header, BorderBrush = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)), BorderThickness = new Thickness(0, 0, 0, 1) };
        Border footerHost = new() { Child = footer, BorderBrush = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)), BorderThickness = new Thickness(0, 1, 0, 0) };
        DockPanel.SetDock(headerHost, Dock.Top);
        DockPanel.SetDock(footerHost, Dock.Bottom);
        root.Children.Add(headerHost);
        root.Children.Add(footerHost);
        root.Children.Add(new ScrollViewer { Content = listHost, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, Background = new SolidColorBrush(Color.FromRgb(0x2B, 0x2B, 0x2B)) });
        Content = root;
    }

    public override string Id => "layers";

    public override string Title => Strings.EditorCommands_Layers;

    private Button LockButton(string tip, bool active, Action toggle, string icon)
    {
        Button button = new()
        {
            Classes = { "icon" },
            Content = new IconView(icon, 16),
            Width = 24,
            Height = 22,
            Padding = new Thickness(0),
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        button.Classes.Set("checked", active);
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) =>
        {
            toggle();
            Editor.Commit(Strings.LayersPanel_ChangeLayerLock);
        };
        return button;
    }

    public override void Refresh()
    {
        RefreshHeader();
        _updating = true;
        _list.Children.Clear();
        _rows.Clear();
        PhotoDocument? doc = Doc;
        if (doc != null)
        {
            foreach (var (layer, depth) in doc.GetLayerListTopDown()) _list.Children.Add(CreateRow(doc, layer, depth));
        }
        _updating = false;
    }

    /// <summary>Updates the blend, opacity, fill and lock controls for the active layer.</summary>
    private void RefreshHeader()
    {
        _updating = true;
        _locks.Children.Clear();
        PhotoDocument? doc = Doc;
        Layer? active = doc?.ActiveLayer;
        IsEnabled = doc != null;
        if (active != null)
        {
            _blend.SelectedItem = _blend.Items.OfType<ComboBoxItem>().FirstOrDefault(i => (LayerBlendMode)i.Tag! == active.BlendMode);
            _opacity.Value = (decimal)Math.Round(active.Opacity * 100);
            _fill.Value = (decimal)Math.Round(active.FillOpacity * 100);
            _locks.Children.Add(LockButton(Strings.LayersPanel_LockTransparentPixels, active.LockTransparency, () => active.LockTransparency = !active.LockTransparency, PhotoEditorIcons.LockTransparency));
            _locks.Children.Add(LockButton(Strings.LayersPanel_LockImagePixels, active.LockPixels, () => active.LockPixels = !active.LockPixels, PhotoEditorIcons.LockPixels));
            _locks.Children.Add(LockButton(Strings.LayersPanel_LockPosition, active.LockPosition, () => active.LockPosition = !active.LockPosition, PhotoEditorIcons.LockPosition));
            _locks.Children.Add(LockButton(Strings.LayersPanel_LockAll, active.LockAll, () => active.LockAll = !active.LockAll, PhotoEditorIcons.Lock));
        }
        _updating = false;
    }

    private Control CreateRow(PhotoDocument doc, Layer layer, int depth)
    {
        bool selected = layer == doc.ActiveLayer || doc.SelectedLayerIds.Contains(layer.Id);
        Border row = new()
        {
            Height = 38,
            Background = selected ? SelectedBrush : Brushes.Transparent,
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24)),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Tag = layer
        };
        Grid content = new() { ColumnDefinitions = new ColumnDefinitions("28,Auto,*,Auto") };

        // Visibility column, separated from the layer content like the reference panel.
        Border eye = new()
        {
            Width = 28,
            Background = Brushes.Transparent,
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24)),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Child = layer.Visible ? new IconView(PhotoEditorIcons.Eye, 16) : null,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        ToolTip.SetTip(eye, Strings.LayersPanel_ToggleVisibilityTip);
        eye.PointerPressed += (_, e) =>
        {
            e.Handled = true;
            if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
            {
                List<Layer> siblings = doc.AllLayers.Where(l => l.Parent == layer.Parent && l != layer).ToList();
                bool solo = siblings.All(l => !l.Visible) && layer.Visible;
                foreach (Layer other in siblings) other.Visible = solo;
                layer.Visible = true;
            }
            else
            {
                layer.Visible = !layer.Visible;
            }
            Editor.Commit(layer.Visible ? Strings.LayersPanel_ShowLayer : Strings.LayersPanel_HideLayer);
        };
        content.Children.Add(eye);

        StackPanel thumbs = new() { Orientation = Orientation.Horizontal, Spacing = 4, Margin = new Thickness(6 + depth * 16, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(thumbs, 1);
        if (layer.IsClipped) thumbs.Children.Add(new IconView(PhotoEditorIcons.ClippingMask, 12));
        if (layer is GroupLayer group)
        {
            Button expander = new() { Classes = { "icon" }, Content = new IconView(group.Expanded ? PhotoEditorIcons.ChevronDown : PhotoEditorIcons.ChevronRight, 10), Padding = new Thickness(2), VerticalAlignment = VerticalAlignment.Center };
            expander.Click += (_, _) =>
            {
                group.Expanded = !group.Expanded;
                Refresh();
            };
            thumbs.Children.Add(expander);
        }

        bool editingMask = doc.EditingMask && layer == doc.ActiveLayer;
        IBrush frame = ThumbFrame;
        Control thumbContent = layer switch
        {
            GroupLayer g => g.IsArtboard ? new IconView(PhotoEditorIcons.Artboard, 20) : new IconView(PhotoEditorIcons.Folder, 16),
            TextLayer => new IconView(PhotoEditorIcons.HorizontalType, 20),
            AdjustmentLayer => new IconView(PhotoEditorIcons.Adjustment, 16),
            _ => CreateThumbnail(doc, layer)
        };
        bool iconThumb = layer is GroupLayer or TextLayer or AdjustmentLayer;
        Border thumbnail = new()
        {
            Width = iconThumb ? 26 : 32,
            Height = iconThumb ? 26 : 32,
            BorderThickness = new Thickness(iconThumb ? 0 : 1),
            BorderBrush = selected && !editingMask ? Brushes.White : frame,
            Child = thumbContent,
            Cursor = new Cursor(StandardCursorType.Hand),
            VerticalAlignment = VerticalAlignment.Center
        };
        ToolTip.SetTip(thumbnail, "Ctrl-click loads the layer transparency as a selection");
        thumbnail.PointerPressed += (_, e) =>
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                EditorCommands.LoadLayerTransparency(Editor, layer, e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? SelectionMode.Add : e.KeyModifiers.HasFlag(KeyModifiers.Alt) ? SelectionMode.Subtract : SelectionMode.Replace);
                e.Handled = true;
                return;
            }
            doc.EditingMask = false;
        };
        thumbs.Children.Add(thumbnail);

        Border? maskBorder = null;
        if (layer.Mask != null)
        {
            thumbs.Children.Add(new IconView(PhotoEditorIcons.Link, 12));
            Border mask = new()
            {
                Width = 32,
                Height = 32,
                BorderThickness = new Thickness(1),
                BorderBrush = editingMask ? Brushes.White : frame,
                Opacity = layer.MaskEnabled ? 1 : 0.4,
                Child = new Image { Source = MaskThumbnail(layer.Mask) },
                VerticalAlignment = VerticalAlignment.Center
            };
            ToolTip.SetTip(mask, Strings.LayersPanel_MaskThumbnailTip);
            mask.PointerPressed += (_, e) =>
            {
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    layer.MaskEnabled = !layer.MaskEnabled;
                    Editor.Commit(layer.MaskEnabled ? Strings.EditorCommands_EnableLayerMask : Strings.EditorCommands_DisableLayerMask);
                }
                else if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    doc.SetSelection(layer.Mask.Clone());
                    Editor.Commit(Strings.EditorCommands_LoadSelection);
                }
                else
                {
                    doc.ActiveLayer = layer;
                    doc.EditingMask = true;
                    Editor.RefreshPanels();
                }
                e.Handled = true;
            };
            thumbs.Children.Add(mask);
            maskBorder = mask;
        }
        content.Children.Add(thumbs);

        TextBlock name = new()
        {
            Text = layer.Name,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0, 4, 0),
            FontStyle = layer.IsBackground ? FontStyle.Italic : FontStyle.Normal,
            Foreground = selected ? Brushes.White : new SolidColorBrush(Color.FromRgb(0xD0, 0xD0, 0xD0)),
            Opacity = layer.IsEffectivelyVisible ? 1 : 0.55,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        Grid.SetColumn(name, 2);
        content.Children.Add(name);

        StackPanel badges = new() { Orientation = Orientation.Horizontal, Spacing = 6, Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center };
        if (layer.LinkId != 0) badges.Children.Add(new IconView(PhotoEditorIcons.Link, 12));
        if (layer.Style?.HasAny == true) badges.Children.Add(new IconView(PhotoEditorIcons.Effects, 12));
        if (layer is SmartObjectLayer) badges.Children.Add(new IconView(PhotoEditorIcons.SmartObject, 12));
        if (layer.LockAll || layer.LockPixels || layer.LockPosition || layer.LockTransparency) badges.Children.Add(CreateLockBadge(doc, layer));
        Grid.SetColumn(badges, 3);
        content.Children.Add(badges);
        row.Child = content;
        _rows[layer] = new RowParts(row, name, thumbnail, maskBorder);

        row.PointerPressed += (_, e) =>
        {
            PointerPoint point = e.GetCurrentPoint(row);
            if (point.Properties.IsRightButtonPressed)
            {
                if (!IsSelected(doc, layer)) SelectLayer(doc, layer, KeyModifiers.None);
                ShowContextMenu(row);
                e.Handled = true;
                return;
            }
            if (e.ClickCount >= 2)
            {
                _ = Rename(layer);
                return;
            }
            SelectLayer(doc, layer, e.KeyModifiers);
            _dragLayer = layer;
            _dragStart = e.GetPosition(this);
            _dragging = false;
            e.Pointer.Capture(row);
        };
        row.PointerMoved += (_, e) =>
        {
            if (_dragLayer == null) return;
            Point p = e.GetPosition(this);
            if (!_dragging && Math.Abs(p.Y - _dragStart.Y) < 6) return;
            _dragging = true;
            UpdateDrop(e.GetPosition(_list));
        };
        row.PointerReleased += (_, e) =>
        {
            e.Pointer.Capture(null);
            if (_dragging && _dragLayer != null && _drop is (Layer target, int position)) EditorCommands.MoveLayer(Editor, _dragLayer, target, position);
            _dragLayer = null;
            _dragging = false;
            _drop = null;
            _dropIndicator.IsVisible = false;
        };
        return row;
    }

    /// <summary>Lock indicator of a layer row; clicking it on the background layer converts it to a normal layer.</summary>
    private Border CreateLockBadge(PhotoDocument doc, Layer layer)
    {
        Border badge = new() { Background = Brushes.Transparent, Child = new IconView(PhotoEditorIcons.Lock, 16) };
        ToolTip.SetTip(badge, layer.LockAll ? Strings.LayersPanel_FullyLockedTip : Strings.LayersPanel_PartiallyLockedTip);
        if (layer is RasterLayer { IsBackground: true })
        {
            badge.Cursor = new Cursor(StandardCursorType.Hand);
            badge.PointerPressed += (_, e) =>
            {
                if (!e.GetCurrentPoint(badge).Properties.IsLeftButtonPressed) return;
                e.Handled = true;
                SelectLayer(doc, layer, KeyModifiers.None);
                Editor.Execute("layer.fromBackground");
            };
        }
        return badge;
    }

    private static Avalonia.Media.Imaging.Bitmap MaskThumbnail(PixelSurface mask)
    {
        if (mask.Frozen && MaskThumbs.TryGetValue(mask, out CachedThumb? cached)) return cached.Image;
        using SKBitmap rendered = BitmapHelper.MaskThumbnail(mask, 30);
        Avalonia.Media.Imaging.Bitmap image = BitmapHelper.ToAvalonia(rendered);
        if (mask.Frozen) MaskThumbs.AddOrUpdate(mask, new CachedThumb((0, 0, 0, 0), image));
        return image;
    }

    private Control CreateThumbnail(PhotoDocument doc, Layer layer)
    {
        try
        {
            if (layer is RasterLayer { Surface.Frozen: true } raster)
            {
                (int, int, int, int) key = (raster.Offset.X, raster.Offset.Y, doc.Width, doc.Height);
                if (!LayerThumbs.TryGetValue(raster.Surface, out CachedThumb? cached) || cached.Key != key)
                {
                    using SKBitmap rendered = BitmapHelper.LayerThumbnail(doc, layer, 34);
                    cached = new CachedThumb(key, BitmapHelper.ToAvalonia(rendered));
                    LayerThumbs.AddOrUpdate(raster.Surface, cached);
                }
                return new Image { Source = cached.Image, Stretch = Stretch.Uniform };
            }
            using SKBitmap thumb = BitmapHelper.LayerThumbnail(doc, layer, 34);
            return new Image { Source = BitmapHelper.ToAvalonia(thumb), Stretch = Stretch.Uniform };
        }
        catch (Exception)
        {
            return new Border();
        }
    }

    private void UpdateDrop(Point position)
    {
        _drop = null;
        _dropIndicator.IsVisible = false;
        foreach (Control child in _list.Children)
        {
            if (child is not Border { Tag: Layer target } border || target == _dragLayer) continue;
            Rect bounds = border.Bounds;
            if (position.Y < bounds.Top || position.Y > bounds.Bottom) continue;
            double t = (position.Y - bounds.Top) / Math.Max(1, bounds.Height);
            // 0 = above, 1 = below, 2 = inside (groups only).
            int where = target is GroupLayer && t > 0.3 && t < 0.7 ? 2 : t < 0.5 ? 0 : 1;
            _drop = (target, where);
            _dropIndicator.Width = bounds.Width;
            Canvas.SetLeft(_dropIndicator, 0);
            Canvas.SetTop(_dropIndicator, where == 1 ? bounds.Bottom - 1 : where == 0 ? bounds.Top - 1 : bounds.Center.Y);
            _dropIndicator.IsVisible = true;
            return;
        }
    }

    private void SelectLayer(PhotoDocument doc, Layer layer, KeyModifiers modifiers)
    {
        if (modifiers.HasFlag(KeyModifiers.Control))
        {
            if (!doc.SelectedLayerIds.Add(layer.Id)) doc.SelectedLayerIds.Remove(layer.Id);
            if (doc.ActiveLayer != null) doc.SelectedLayerIds.Add(doc.ActiveLayer.Id);
            doc.ActiveLayer = layer;
        }
        else if (modifiers.HasFlag(KeyModifiers.Shift) && doc.ActiveLayer != null)
        {
            List<Layer> order = doc.GetLayerListTopDown().Select(x => x.Layer).ToList();
            int a = order.IndexOf(doc.ActiveLayer), b = order.IndexOf(layer);
            for (int i = Math.Min(a, b); i <= Math.Max(a, b); i++) doc.SelectedLayerIds.Add(order[i].Id);
            doc.ActiveLayer = layer;
        }
        else
        {
            doc.SelectedLayerIds.Clear();
            doc.SelectedLayerIds.Add(layer.Id);
            doc.ActiveLayer = layer;
        }
        doc.EditingMask = false;
        doc.ActiveChannel = null;
        // The rows stay alive so a press can turn into a drag-and-drop reorder.
        ApplySelectionVisuals(doc);
        RefreshHeader();
        Editor.RefreshPanelsExcept(this);
        Editor.RefreshOptions();
        Editor.Canvas.InvalidateOutline();
        Editor.InvalidateOverlay();
    }

    private static bool IsSelected(PhotoDocument doc, Layer layer) => layer == doc.ActiveLayer || doc.SelectedLayerIds.Contains(layer.Id);

    private void ApplySelectionVisuals(PhotoDocument doc)
    {
        foreach (var (layer, parts) in _rows)
        {
            bool selected = IsSelected(doc, layer);
            bool editingMask = doc.EditingMask && layer == doc.ActiveLayer;
            parts.Row.Background = selected ? SelectedBrush : Brushes.Transparent;
            parts.Name.Foreground = selected ? Brushes.White : new SolidColorBrush(Color.FromRgb(0xD0, 0xD0, 0xD0));
            parts.Thumbnail.BorderBrush = selected && !editingMask ? Brushes.White : ThumbFrame;
            if (parts.Mask != null) parts.Mask.BorderBrush = editingMask ? Brushes.White : ThumbFrame;
        }
    }

    private async Task Rename(Layer layer)
    {
        string? name = await Dialogs.DialogBuilder.PromptAsync(Editor, Strings.EditorCommands_RenameLayer, Strings.EditorCommands_NameColon, layer.Name);
        if (string.IsNullOrWhiteSpace(name)) return;
        layer.Name = name;
        Editor.Commit(Strings.EditorCommands_RenameLayer);
    }

    private void ShowContextMenu(Control anchor)
    {
        ContextMenu menu = new();
        void Add(string header, string commandId)
        {
            MenuItem item = new() { Header = header };
            item.Click += (_, _) => Editor.Execute(commandId);
            menu.Items.Add(item);
        }
        Add(Strings.LayersPanel_LayerPropertiesEllipsis, "layer.rename");
        Add(Strings.LayersPanel_BlendingOptionsEllipsis, "layer.style");
        menu.Items.Add(new Separator());
        Add(Strings.LayersPanel_DuplicateLayerEllipsis, "layer.duplicate");
        Add(Strings.EditorCommands_DeleteLayer, "layer.delete");
        Add(Strings.LayersPanel_GroupFromLayersEllipsis, "layer.group");
        menu.Items.Add(new Separator());
        Add(Strings.EditorCommands_ConvertToSmartObject, "layer.smart.convert");
        Add(Strings.EditorCommands_RasterizeLayer, "layer.rasterize");
        Add(Strings.EditorCommands_RasterizeLayerStyle, "layer.rasterizeStyle");
        menu.Items.Add(new Separator());
        Add(Strings.EditorCommands_AddLayerMask, "layer.mask.revealAll");
        Add(Strings.EditorCommands_ApplyLayerMask, "layer.mask.apply");
        Add(Strings.EditorCommands_DeleteLayerMask, "layer.mask.delete");
        Add(Strings.EditorCommands_CreateClippingMask, "layer.clip");
        menu.Items.Add(new Separator());
        Add(Strings.EditorCommands_CopyLayerStyle, "layer.style.copy");
        Add(Strings.EditorCommands_PasteLayerStyle, "layer.style.paste");
        Add(Strings.EditorCommands_ClearLayerStyle, "layer.style.clear");
        menu.Items.Add(new Separator());
        Add(Strings.EditorCommands_LinkUnlinkLayers, "layer.link");
        menu.Items.Add(new Separator());
        Add(Strings.EditorCommands_MergeDown, "layer.mergeDown");
        Add(Strings.EditorCommands_MergeVisible, "layer.mergeVisible");
        Add(Strings.EditorCommands_FlattenImage, "layer.flatten");
        menu.Open(anchor);
    }
}

public enum MaskMode
{
    RevealAll,
    HideAll,
    RevealSelection,
    HideSelection,
    FromTransparency
}
