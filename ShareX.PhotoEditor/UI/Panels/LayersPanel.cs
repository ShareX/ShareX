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

namespace ShareX.PhotoEditor.UI.Panels;

/// <summary>Layers panel: visibility, thumbnails, masks, selection, renaming, drag-and-drop ordering and layer options.</summary>
public sealed class LayersPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromArgb(110, 62, 131, 242));
    private readonly StackPanel _list = new();
    private readonly ComboBox _blend = new() { Width = 130 };
    private readonly NumericUpDown _opacity = DialogsNumber();
    private readonly NumericUpDown _fill = DialogsNumber();
    private readonly StackPanel _locks = new() { Orientation = Orientation.Horizontal, Spacing = 2 };
    private readonly Border _dropIndicator = new() { Height = 2, Background = new SolidColorBrush(Color.FromRgb(0x3E, 0x83, 0xF2)), IsVisible = false, IsHitTestVisible = false };
    private bool _updating;
    private Layer? _dragLayer;
    private Point _dragStart;
    private bool _dragging;
    private (Layer Target, int Position)? _drop;

    private static NumericUpDown DialogsNumber() => new() { Minimum = 0, Maximum = 100, Increment = 1, FormatString = "0", Width = 64, ShowButtonSpinner = false };

    public LayersPanel(PhotoEditorWindow editor) : base(editor)
    {
        foreach (LayerBlendMode mode in BlendModes.LayerModes.Prepend(LayerBlendMode.PassThrough)) _blend.Items.Add(new ComboBoxItem { Content = BlendModes.GetName(mode), Tag = mode });
        _blend.SelectionChanged += (_, _) =>
        {
            if (_updating || Doc?.ActiveLayer is not Layer layer || _blend.SelectedItem is not ComboBoxItem { Tag: LayerBlendMode mode }) return;
            if (mode == LayerBlendMode.PassThrough && layer is not GroupLayer) return;
            layer.BlendMode = mode;
            Editor.Commit("Blending Change");
        };
        _opacity.ValueChanged += (_, e) =>
        {
            if (_updating || Doc?.ActiveLayer is not Layer layer || e.NewValue is not decimal v) return;
            layer.Opacity = (float)v / 100;
            Editor.Commit("Opacity Change");
        };
        _fill.ValueChanged += (_, e) =>
        {
            if (_updating || Doc?.ActiveLayer is not Layer layer || e.NewValue is not decimal v) return;
            layer.FillOpacity = (float)v / 100;
            Editor.Commit("Fill Opacity Change");
        };

        StackPanel header = new() { Spacing = 4, Margin = new Thickness(4) };
        header.Children.Add(Row(_blend, Text("Opacity:"), _opacity));
        header.Children.Add(Row(Text("Lock:"), _locks, Text("Fill:"), _fill));

        StackPanel footer = new() { Orientation = Orientation.Horizontal, Spacing = 2, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("link", "Link layers (select multiple layers with Ctrl-click)", () => { }));
        footer.Children.Add(IconButton("fx", "Add a layer style", () => _ = EditorCommands.LayerStyle(Editor)));
        footer.Children.Add(IconButton("mask", "Add layer mask (Alt: hide all)", () => EditorCommands.AddMask(Editor, Doc?.Selection != null ? MaskMode.RevealSelection : MaskMode.RevealAll)));
        Button adjust = IconButton("adjust", "Create new fill or adjustment layer", () => { });
        adjust.Flyout = EditorCommands.CreateAdjustmentFlyout(Editor);
        footer.Children.Add(adjust);
        footer.Children.Add(IconButton("folder", "Create a new group", () => EditorCommands.NewGroup(Editor)));
        footer.Children.Add(IconButton("new-layer", "Create a new layer", () => EditorCommands.NewLayer(Editor)));
        footer.Children.Add(IconButton("trash", "Delete layer", () => EditorCommands.DeleteLayers(Editor)));

        Grid listHost = new();
        listHost.Children.Add(_list);
        listHost.Children.Add(new Canvas { IsHitTestVisible = false, Children = { _dropIndicator } });

        DockPanel root = new();
        DockPanel.SetDock(header, Dock.Top);
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(header);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = listHost, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled });
        Content = root;
    }

    public override string Id => "layers";

    public override string Title => "Layers";

    private Button LockButton(string tip, bool active, Action toggle, string glyph)
    {
        Button button = new()
        {
            Content = new TextBlock { Text = glyph, FontSize = 11 },
            Padding = new Thickness(4, 1),
            Background = active ? SelectedBrush : Brushes.Transparent
        };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) =>
        {
            toggle();
            Editor.Commit("Change Layer Lock");
        };
        return button;
    }

    public override void Refresh()
    {
        _updating = true;
        _list.Children.Clear();
        _locks.Children.Clear();
        PhotoDocument? doc = Doc;
        Layer? active = doc?.ActiveLayer;
        IsEnabled = doc != null;
        if (active != null)
        {
            _blend.SelectedItem = _blend.Items.OfType<ComboBoxItem>().FirstOrDefault(i => (LayerBlendMode)i.Tag! == active.BlendMode);
            _opacity.Value = (decimal)Math.Round(active.Opacity * 100);
            _fill.Value = (decimal)Math.Round(active.FillOpacity * 100);
            _locks.Children.Add(LockButton("Lock transparent pixels", active.LockTransparency, () => active.LockTransparency = !active.LockTransparency, "▦"));
            _locks.Children.Add(LockButton("Lock image pixels", active.LockPixels, () => active.LockPixels = !active.LockPixels, "✎"));
            _locks.Children.Add(LockButton("Lock position", active.LockPosition, () => active.LockPosition = !active.LockPosition, "✥"));
            _locks.Children.Add(LockButton("Lock all", active.LockAll, () => active.LockAll = !active.LockAll, "🔒"));
        }
        if (doc != null)
        {
            foreach (var (layer, depth) in doc.GetLayerListTopDown()) _list.Children.Add(CreateRow(doc, layer, depth));
        }
        _updating = false;
    }

    private Control CreateRow(PhotoDocument doc, Layer layer, int depth)
    {
        bool selected = layer == doc.ActiveLayer || doc.SelectedLayerIds.Contains(layer.Id);
        Border row = new()
        {
            Padding = new Thickness(2),
            Background = selected ? SelectedBrush : Brushes.Transparent,
            BorderBrush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Tag = layer
        };
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 4 };

        Button eye = new()
        {
            Content = layer.Visible ? ToolIcons.Create("eye", 14) : new Border { Width = 14, Height = 14 },
            Padding = new Thickness(2),
            Background = Brushes.Transparent,
            VerticalAlignment = VerticalAlignment.Center
        };
        ToolTip.SetTip(eye, "Toggle visibility (Alt-click shows only this layer)");
        eye.Click += (_, _) =>
        {
            if (Editor.LastModifiers.HasFlag(KeyModifiers.Alt))
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
            Editor.Commit(layer.Visible ? "Show Layer" : "Hide Layer");
        };
        content.Children.Add(eye);
        content.Children.Add(new Border { Width = depth * 14 });

        if (layer.IsClipped) content.Children.Add(Text("↳"));
        if (layer is GroupLayer group)
        {
            Button expander = new() { Content = ToolIcons.Create(group.Expanded ? "chevron-down" : "chevron-right", 12), Padding = new Thickness(1), Background = Brushes.Transparent };
            expander.Click += (_, _) =>
            {
                group.Expanded = !group.Expanded;
                Refresh();
            };
            content.Children.Add(expander);
        }

        Border thumbnail = new()
        {
            Width = 36,
            Height = 36,
            BorderThickness = new Thickness(doc.EditingMask && layer == doc.ActiveLayer ? 0 : 1),
            BorderBrush = Brushes.Gray,
            Child = layer is GroupLayer ? ToolIcons.Create(((GroupLayer)layer).IsArtboard ? "artboard" : "folder", 22) : CreateThumbnail(doc, layer),
            Cursor = new Cursor(StandardCursorType.Hand)
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
        content.Children.Add(thumbnail);

        if (layer.Mask != null)
        {
            Border mask = new()
            {
                Width = 36,
                Height = 36,
                BorderThickness = new Thickness(doc.EditingMask && layer == doc.ActiveLayer ? 2 : 1),
                BorderBrush = doc.EditingMask && layer == doc.ActiveLayer ? Brushes.White : Brushes.Gray,
                Opacity = layer.MaskEnabled ? 1 : 0.4,
                Child = new Image { Source = BitmapHelper.ToAvalonia(BitmapHelper.MaskThumbnail(layer.Mask, 34)) }
            };
            ToolTip.SetTip(mask, "Click to edit the mask, Shift-click to disable it, Ctrl-click to load it as a selection");
            mask.PointerPressed += (_, e) =>
            {
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    layer.MaskEnabled = !layer.MaskEnabled;
                    Editor.Commit(layer.MaskEnabled ? "Enable Layer Mask" : "Disable Layer Mask");
                }
                else if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    doc.SetSelection(layer.Mask.Clone());
                    Editor.Commit("Load Selection");
                }
                else
                {
                    doc.ActiveLayer = layer;
                    doc.EditingMask = true;
                    Editor.RefreshPanels();
                }
                e.Handled = true;
            };
            content.Children.Add(mask);
        }

        TextBlock name = new()
        {
            Text = layer.Name,
            VerticalAlignment = VerticalAlignment.Center,
            FontStyle = layer.IsBackground ? FontStyle.Italic : FontStyle.Normal,
            Opacity = layer.IsEffectivelyVisible ? 1 : 0.55,
            TextTrimming = TextTrimming.CharacterEllipsis,
            MaxWidth = 140
        };
        content.Children.Add(name);
        string badges = (layer.Style?.HasAny == true ? " fx" : string.Empty) + (layer.LockAll || layer.LockPixels || layer.LockPosition || layer.LockTransparency ? " 🔒" : string.Empty)
            + (layer is SmartObjectLayer ? " ◰" : string.Empty) + (layer is TextLayer ? " T" : string.Empty);
        if (badges.Length > 0) content.Children.Add(Text(badges, 11));
        row.Child = content;

        row.PointerPressed += (_, e) =>
        {
            PointerPoint point = e.GetCurrentPoint(row);
            if (point.Properties.IsRightButtonPressed)
            {
                if (!selected) SelectLayer(doc, layer, KeyModifiers.None);
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

    private Control CreateThumbnail(PhotoDocument doc, Layer layer)
    {
        try
        {
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
        Editor.RefreshPanels();
        Editor.RefreshOptions();
    }

    private async Task Rename(Layer layer)
    {
        string? name = await Dialogs.DialogBuilder.PromptAsync(Editor, "Rename Layer", "Name:", layer.Name);
        if (string.IsNullOrWhiteSpace(name)) return;
        layer.Name = name;
        Editor.Commit("Rename Layer");
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
        Add("Layer Properties...", "layer.rename");
        Add("Blending Options...", "layer.style");
        menu.Items.Add(new Separator());
        Add("Duplicate Layer...", "layer.duplicate");
        Add("Delete Layer", "layer.delete");
        Add("Group from Layers...", "layer.group");
        menu.Items.Add(new Separator());
        Add("Convert to Smart Object", "layer.smart.convert");
        Add("Rasterize Layer", "layer.rasterize");
        Add("Rasterize Layer Style", "layer.rasterizeStyle");
        menu.Items.Add(new Separator());
        Add("Add Layer Mask", "layer.mask.revealAll");
        Add("Apply Layer Mask", "layer.mask.apply");
        Add("Delete Layer Mask", "layer.mask.delete");
        Add("Create Clipping Mask", "layer.clip");
        menu.Items.Add(new Separator());
        Add("Copy Layer Style", "layer.style.copy");
        Add("Paste Layer Style", "layer.style.paste");
        Add("Clear Layer Style", "layer.style.clear");
        menu.Items.Add(new Separator());
        Add("Merge Down", "layer.mergeDown");
        Add("Merge Visible", "layer.mergeVisible");
        Add("Flatten Image", "layer.flatten");
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
