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

using System.Text.Json;
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.UI.Dialogs;

/// <summary>Creating, converting and exporting tilemap layers.</summary>
public static class TilemapDialogs
{
    private static (Func<int> Width, Func<int> Height) TileSize(Form form, EditorSettings settings)
    {
        form.Note(Strings.Dialog_Tilemap_Note);
        Func<int> width = form.Number(Strings.Dialog_TileWidth, 1, 512, Math.Clamp(settings.GridWidth, 1, 512));
        Func<int> height = form.Number(Strings.Dialog_TileHeight, 1, 512, Math.Clamp(settings.GridHeight, 1, 512));
        return (width, height);
    }

    public static async Task NewLayerAsync(PixelEditorWindow w)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null) return;
        Form form = new(w);
        (Func<int> width, Func<int> height) = TileSize(form, w.Settings);
        string title = Strings.Command_NewTilemapLayer.Replace("...", string.Empty);
        if (!await form.ShowAsync(title, 380) || !ReferenceEquals(w.Sprite, sprite)) return;
        w.CommitFloating();
        PixelLayer layer = sprite.CreatePixelLayer(EditorCommands.UniqueLayerName(sprite, Strings.Layer_TilemapName));
        layer.Tileset = new Tileset(width(), height());
        sprite.AddLayer(layer, sprite.ActiveLayer);
        sprite.ActiveLayer = layer;
        (w.Settings.GridWidth, w.Settings.GridHeight) = (width(), height());
        w.SelectedTile = 0;
        w.Panels.ShowPanel("tiles");
        w.Commit(title);
    }

    public static async Task ConvertAsync(PixelEditorWindow w)
    {
        Sprite? sprite = w.Sprite;
        if (sprite?.ActiveLayer is not PixelLayer { Tileset: null } layer) return;
        Form form = new(w);
        (Func<int> width, Func<int> height) = TileSize(form, w.Settings);
        string title = Strings.Command_ConvertToTilemap.Replace("...", string.Empty);
        if (!await form.ShowAsync(title, 380) || !ReferenceEquals(w.Sprite, sprite) || sprite.ActiveLayer != layer) return;
        w.CommitFloating();
        layer.Tileset = new Tileset(width(), height());
        foreach (Cel? cel in layer.Cels)
        {
            if (cel == null) continue;
            cel.Image.Map = null;
            cel.Image.Rendered = null;
        }
        (w.Settings.GridWidth, w.Settings.GridHeight) = (width(), height());
        w.Panels.ShowPanel("tiles");
        w.Commit(title);
        w.SelectedTile = Math.Min(1, layer.Tileset.Tiles.Count - 1);
        w.SetStatus(string.Format(Strings.Status_TilesMade, layer.Tileset.Tiles.Count - 1));
    }

    /// <summary>Writes the tileset as an image and the maps of every frame as JSON next to it.</summary>
    public static async Task ExportAsync(PixelEditorWindow w)
    {
        Sprite? sprite = w.Sprite;
        if (sprite?.ActiveLayer is not PixelLayer { Tileset: Tileset set } layer) return;
        string title = Strings.Command_ExportTilemap.Replace("...", string.Empty);
        string? path = await w.PickSaveFileAsync(title, Path.GetFileNameWithoutExtension(sprite.DisplayName) + "-" + layer.Name + "-tiles.png", ("PNG", [".png"]));
        if (path == null) return;
        try
        {
            int count = Math.Max(1, set.Tiles.Count - 1);
            int columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(count)));
            File.WriteAllBytes(path, SpriteFile.EncodeImage(Tilemaps.RenderTileset(set, columns), ".png", 100));
            int mapColumns = set.Columns(sprite.Width), mapRows = set.Rows(sprite.Height);
            var metadata = new
            {
                image = Path.GetFileName(path),
                tileWidth = set.TileWidth,
                tileHeight = set.TileHeight,
                tileCount = set.Tiles.Count - 1,
                columns,
                mapWidth = mapColumns,
                mapHeight = mapRows,
                // 0 is an empty cell; tile n is the n-th tile of the image, counting from 1, row by row.
                frames = sprite.Frames.Select((frame, index) => new
                {
                    frame = index,
                    duration = frame.Duration,
                    data = layer.Cels[index]?.Image.Map ?? new int[mapColumns * mapRows]
                }).ToList()
            };
            File.WriteAllText(Path.ChangeExtension(path, ".json"), JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));
            w.SetStatus(string.Format(Strings.Status_Exported, Path.GetFileName(path)));
        }
        catch (Exception ex)
        {
            w.ShowMessage(ex.Message);
        }
    }
}
