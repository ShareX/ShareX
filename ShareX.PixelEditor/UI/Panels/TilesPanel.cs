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
using ShareX.PhotoEditor.UI.Theme;
using ShareX.PixelEditor.Tools;

namespace ShareX.PixelEditor.UI.Panels;

/// <summary>The tiles of the active tilemap layer; click one to stamp it with the Tile Stamp tool.</summary>
public sealed class TilesPanel : EditorPanel
{
    private const double TileBox = 34;

    private readonly WrapPanel _tiles = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(6) };
    private readonly TextBlock _info = new() { Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0), TextTrimming = TextTrimming.CharacterEllipsis };
    private readonly StackPanel _empty = new() { Margin = new Thickness(10), Spacing = 8 };
    private readonly ScrollViewer _scroll;
    private readonly List<Border> _boxes = [];
    private readonly CheckBox _shared = new() { Content = Strings.Tiles_EditsShared, Margin = new Thickness(6, 2), FontSize = 11 };
    private Tileset? _shownSet;
    private List<PixelBuffer>? _shownTiles;

    public TilesPanel(PixelEditorWindow editor) : base(editor)
    {
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 1 };
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.NewLayer, Strings.Command_NewTilemapLayer.Replace("...", string.Empty), () => Editor.Execute("layer.newTilemap"), 12));
        buttons.Children.Add(Ui.IconButton(PixelEditorIcons.Trash, Strings.Command_RemoveUnusedTiles, () => Editor.Execute("tiles.removeUnused"), 12));
        DockPanel bar = new() { Height = 26, Background = Ui.Deep };
        DockPanel.SetDock(buttons, Dock.Right);
        bar.Children.Add(buttons);
        bar.Children.Add(_info);

        _empty.Children.Add(new TextBlock { Text = Strings.Tiles_Empty, TextWrapping = TextWrapping.Wrap, Classes = { "dim" } });
        Button create = new() { Content = Strings.Command_NewTilemapLayer.Replace("...", string.Empty), HorizontalAlignment = HorizontalAlignment.Left };
        create.Click += (_, _) => Editor.Execute("layer.newTilemap");
        Button convert = new() { Content = Strings.Command_ConvertToTilemap.Replace("...", string.Empty), HorizontalAlignment = HorizontalAlignment.Left };
        convert.Click += (_, _) => Editor.Execute("layer.convertToTilemap");
        _empty.Children.Add(create);
        _empty.Children.Add(convert);

        CheckBox shared = _shared;
        shared.IsChecked = editor.Settings.TileEditsShared;
        ToolTip.SetTip(shared, Strings.Tiles_EditsShared_Tip);
        shared.IsCheckedChanged += (_, _) =>
        {
            if (Editor.Settings.TileEditsShared == (shared.IsChecked == true)) return;
            Editor.Settings.TileEditsShared = shared.IsChecked == true;
            Editor.RefreshOptions();
        };

        _scroll = new ScrollViewer { Content = _tiles, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        DockPanel root = new();
        DockPanel.SetDock(bar, Dock.Bottom);
        DockPanel.SetDock(shared, Dock.Bottom);
        root.Children.Add(bar);
        root.Children.Add(shared);
        root.Children.Add(_empty);
        root.Children.Add(_scroll);
        Content = root;
    }

    public override string Id => "tiles";

    public override string Title => Strings.Panel_Tiles;

    private Tileset? Set => (Sprite?.ActiveLayer as PixelLayer)?.Tileset;

    public override void Refresh()
    {
        Tileset? set = Set;
        _shared.IsChecked = Editor.Settings.TileEditsShared;
        _shared.IsVisible = set != null;
        _empty.IsVisible = set == null;
        _scroll.IsVisible = set != null;
        if (set == null)
        {
            _info.Text = string.Empty;
            _tiles.Children.Clear();
            _boxes.Clear();
            _shownSet = null;
            _shownTiles = null;
            return;
        }
        _info.Text = string.Format(Strings.Tiles_Info, set.Tiles.Count - 1, set.TileWidth, set.TileHeight);
        if (!ReferenceEquals(set, _shownSet) || _shownTiles == null || !_shownTiles.SequenceEqual(set.Tiles, ReferenceEqualityComparer.Instance))
        {
            _shownSet = set;
            _shownTiles = [.. set.Tiles];
            _tiles.Children.Clear();
            _boxes.Clear();
            for (int i = 0; i < set.Tiles.Count; i++) AddBox(set, i);
        }
        RefreshSelection();
    }

    private void AddBox(Tileset set, int index)
    {
        Control content;
        if (index == 0)
        {
            // The empty tile erases cells.
            content = new Avalonia.Controls.Shapes.Path { Data = Geometry.Parse("M0 0 L18 18 M18 0 L0 18"), Stroke = Ui.DimBrush, StrokeThickness = 1, Width = 18, Height = 18, Stretch = Stretch.Fill };
        }
        else
        {
            PixelBuffer tile = set.Tiles[index];
            double scale = Math.Max(1, Math.Floor((TileBox - 6) / Math.Max(tile.Width, tile.Height)));
            if (Math.Max(tile.Width, tile.Height) > TileBox - 6) scale = (TileBox - 6) / Math.Max(tile.Width, tile.Height);
            Image image = new() { Source = Ui.ToBitmap(tile), Width = tile.Width * scale, Height = tile.Height * scale };
            RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.None);
            content = image;
        }
        Border box = new()
        {
            Width = TileBox,
            Height = TileBox,
            Margin = new Thickness(0, 0, 2, 2),
            Background = index == 0 ? Ui.Deep : Ui.Checker(0xFF8C8C8C, 0xFFA6A6A6, 4),
            BorderThickness = new Thickness(2),
            Child = content,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        if (content is Layoutable layoutable)
        {
            layoutable.HorizontalAlignment = HorizontalAlignment.Center;
            layoutable.VerticalAlignment = VerticalAlignment.Center;
        }
        ToolTip.SetTip(box, index == 0 ? Strings.Tiles_EmptyTile : string.Format(Strings.Tiles_Tile, index));
        box.PointerPressed += (_, e) =>
        {
            Editor.SelectedTile = index;
            if (Editor.Tools.Active.Id != ToolId.TileStamp) Editor.Tools.Select(ToolId.TileStamp);
            e.Handled = true;
        };
        _boxes.Add(box);
        _tiles.Children.Add(box);
    }

    public void RefreshSelection()
    {
        for (int i = 0; i < _boxes.Count; i++) _boxes[i].BorderBrush = i == Editor.SelectedTile ? PhotoEditorTheme.AccentBorderBrush : Brushes.Transparent;
    }
}
