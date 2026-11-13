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

using Avalonia.Input;
using ShareX.PixelEditor.UI;

namespace ShareX.PixelEditor.Tools;

/// <summary>Places the tile chosen in the Tiles panel into the cells of a tilemap layer.</summary>
public sealed class TileStampTool : ToolBase
{
    private bool _down, _changed;
    private int _lastColumn = int.MinValue, _lastRow = int.MinValue;
    private SKRectI? _hover;

    public TileStampTool(PixelEditorWindow editor) : base(editor, ToolId.TileStamp, Strings.Tool_TileStamp, Strings.Tool_TileStamp_Hint, PixelEditorIcons.TileStamp, Key.A)
    {
    }

    public override bool HasPendingEdit => _down && _changed;

    private Tileset? Tiles => (Sprite?.ActiveLayer as PixelLayer)?.Tileset;

    private (int Column, int Row) Cell(ToolPointer p, Tileset set) =>
        ((int)Math.Floor(p.X / (double)set.TileWidth), (int)Math.Floor(p.Y / (double)set.TileHeight));

    public override void Down(ToolPointer p)
    {
        Sprite? sprite = Sprite;
        if (sprite == null) return;
        if (Tiles == null)
        {
            Editor.SetStatus(Strings.Status_SelectTilemapLayer);
            return;
        }
        if (p.Alt)
        {
            // Alt+click picks the tile under the pointer.
            PixelLayer layer = (PixelLayer)sprite.ActiveLayer!;
            (int column, int row) = Cell(p, layer.Tileset!);
            Editor.SelectedTile = Tilemaps.TileAt(sprite, layer, sprite.GetCel(layer, sprite.ActiveFrame)?.Image, column, row);
            return;
        }
        if (Editor.EditableLayer() == null) return;
        Editor.StopPlayback();
        Editor.CommitFloating();
        _down = true;
        _changed = false;
        _lastColumn = _lastRow = int.MinValue;
        Place(p);
    }

    public override void Move(ToolPointer p)
    {
        UpdateHover(p);
        if (_down) Place(p);
    }

    public override void Hover(ToolPointer p) => UpdateHover(p);

    public override void Up(ToolPointer p)
    {
        if (!_down) return;
        _down = false;
        if (_changed) Editor.Commit(Name);
    }

    public override void Cancel()
    {
        if (_down && _changed) Editor.RevertEdit();
        _down = false;
        _changed = false;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _hover = null;
    }

    private void Place(ToolPointer p)
    {
        Sprite? sprite = Sprite;
        if (sprite?.ActiveLayer is not PixelLayer { Tileset: Tileset set } layer) return;
        (int column, int row) = Cell(p, set);
        if (column == _lastColumn && row == _lastRow) return;
        _lastColumn = column;
        _lastRow = row;
        if (column < 0 || row < 0 || column >= set.Columns(sprite.Width) || row >= set.Rows(sprite.Height)) return;
        int tile = p.Right ? 0 : Math.Clamp(Editor.SelectedTile, 0, set.Tiles.Count - 1);
        Cel? cel = sprite.GetCel(layer, sprite.ActiveFrame);
        if (Tilemaps.TileAt(sprite, layer, cel?.Image, column, row) == tile && cel?.Image.Map != null) return;
        if (cel == null)
        {
            if (tile == 0) return;
            sprite.GetWritable(layer, sprite.ActiveFrame);
            cel = sprite.GetCel(layer, sprite.ActiveFrame)!;
        }
        Tilemaps.SetTile(sprite, layer, cel.Image, column, row, tile);
        _changed = true;
        Editor.InvalidateCanvas();
    }

    private void UpdateHover(ToolPointer p)
    {
        Sprite? sprite = Sprite;
        SKRectI? hover = null;
        if (sprite != null && Tiles is Tileset set)
        {
            (int column, int row) = Cell(p, set);
            if (column >= 0 && row >= 0 && column < set.Columns(sprite.Width) && row < set.Rows(sprite.Height))
                hover = SKRectI.Create(column * set.TileWidth, row * set.TileHeight, set.TileWidth, set.TileHeight);
        }
        if (hover == _hover) return;
        _hover = hover;
        Editor.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (_hover is SKRectI rect) overlay.RectOutline(rect);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (Tiles is not Tileset set)
        {
            options.Label(Strings.Status_SelectTilemapLayer);
            options.Separator();
            options.Button(Strings.Command_NewTilemapLayer.Replace("...", string.Empty), () => Editor.Execute("layer.newTilemap"));
            return;
        }
        options.Label(string.Format(Strings.Options_TileInfo, Math.Clamp(Editor.SelectedTile, 0, set.Tiles.Count - 1), set.TileWidth, set.TileHeight));
        options.Separator();
        options.Check(Strings.Tiles_EditsShared, Settings.TileEditsShared, v => Settings.TileEditsShared = v, Strings.Tiles_EditsShared_Tip);
    }
}
