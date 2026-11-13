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

namespace ShareX.PixelEditor.Core;

/// <summary>
/// The tiles of a tilemap layer. Index 0 is always the empty tile. Tile buffers are shared with the history, so they are
/// replaced, never changed in place.
/// </summary>
public sealed class Tileset
{
    public Tileset(int tileWidth, int tileHeight)
    {
        TileWidth = Math.Clamp(tileWidth, 1, 512);
        TileHeight = Math.Clamp(tileHeight, 1, 512);
        Tiles.Add(new PixelBuffer(TileWidth, TileHeight) { Frozen = true });
    }

    private Tileset(int tileWidth, int tileHeight, IEnumerable<PixelBuffer> tiles)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Tiles.AddRange(tiles);
    }

    public int TileWidth { get; }

    public int TileHeight { get; }

    public List<PixelBuffer> Tiles { get; }  = [];

    public int Columns(int spriteWidth) => (spriteWidth + TileWidth - 1) / TileWidth;

    public int Rows(int spriteHeight) => (spriteHeight + TileHeight - 1) / TileHeight;

    public Tileset Clone()
    {
        foreach (PixelBuffer tile in Tiles) tile.Frozen = true;
        return new Tileset(TileWidth, TileHeight, Tiles);
    }

    /// <summary>The index of a tile with exactly these pixels, 0 for an empty tile, or -1 when there is none.</summary>
    public int Find(PixelBuffer tile)
    {
        if (Tilemaps.IsEmpty(tile)) return 0;
        for (int i = 1; i < Tiles.Count; i++)
            if (Tiles[i].Pixels.AsSpan().SequenceEqual(tile.Pixels)) return i;
        return -1;
    }

    /// <summary>The index of a tile with these pixels, adding it when the tileset does not have one yet.</summary>
    public int AddOrFind(PixelBuffer tile)
    {
        int index = Find(tile);
        if (index >= 0) return index;
        tile.Frozen = true;
        Tiles.Add(tile);
        return Tiles.Count - 1;
    }
}

/// <summary>Builds tilemap cels from tiles and keeps tiles and maps in step with pixels painted on the cels.</summary>
public static class Tilemaps
{
    public static bool IsEmpty(PixelBuffer tile)
    {
        foreach (uint p in tile.Pixels)
            if (p >> 24 != 0) return false;
        return true;
    }

    /// <summary>Draws a map with the tiles of a tileset.</summary>
    public static PixelBuffer Render(Tileset set, int[] map, int width, int height)
    {
        PixelBuffer result = new(width, height);
        int columns = set.Columns(width), rows = set.Rows(height);
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int index = row * columns + column;
                int tile = index < map.Length ? map[index] : 0;
                if (tile <= 0 || tile >= set.Tiles.Count) continue;
                result.Blit(set.Tiles[tile], column * set.TileWidth, row * set.TileHeight, false);
            }
        }
        return result;
    }

    /// <summary>Cuts the pixels of one map cell out of an image; parts past the image edge are transparent.</summary>
    public static PixelBuffer ReadTile(PixelBuffer image, Tileset set, int column, int row)
    {
        PixelBuffer tile = new(set.TileWidth, set.TileHeight);
        int x0 = column * set.TileWidth, y0 = row * set.TileHeight;
        int width = Math.Min(set.TileWidth, image.Width - x0), height = Math.Min(set.TileHeight, image.Height - y0);
        for (int y = 0; y < height; y++)
            Array.Copy(image.Pixels, (y0 + y) * image.Width + x0, tile.Pixels, y * set.TileWidth, width);
        return tile;
    }

    /// <summary>Splits an image into tiles, reusing tiles the set already has, and returns the map.</summary>
    public static int[] BuildMap(Tileset set, PixelBuffer image)
    {
        int columns = set.Columns(image.Width), rows = set.Rows(image.Height);
        int[] map = new int[columns * rows];
        for (int row = 0; row < rows; row++)
            for (int column = 0; column < columns; column++)
                map[row * columns + column] = set.AddOrFind(ReadTile(image, set, column, row));
        return map;
    }

    /// <summary>Whether two tiles look the same in the part of a cell that lies inside the sprite.</summary>
    private static bool SameVisible(PixelBuffer a, PixelBuffer b, int width, int height)
    {
        for (int y = 0; y < height; y++)
            if (!a.Pixels.AsSpan(y * a.Width, width).SequenceEqual(b.Pixels.AsSpan(y * b.Width, width))) return false;
        return true;
    }

    /// <summary>
    /// Brings the tiles and maps of every tilemap layer up to date with pixels changed since the cels were last drawn
    /// from their tiles, then redraws the cels. With <paramref name="shareEdits"/> a change to a used tile changes that
    /// tile everywhere it is used; otherwise the changed cell gets a tile of its own.
    /// </summary>
    public static void Sync(Sprite sprite, bool shareEdits)
    {
        foreach (PixelLayer layer in sprite.PixelLayers)
        {
            if (layer.Tileset is not Tileset set) continue;
            int columns = set.Columns(sprite.Width), rows = set.Rows(sprite.Height);
            List<CelImage> images = layer.Cels.Where(c => c != null).Select(c => c!.Image).Distinct().ToList();
            bool tilesChanged = false;
            List<CelImage> changed = [];
            foreach (CelImage image in images)
            {
                bool sized = image.Buffer.Width == sprite.Width && image.Buffer.Height == sprite.Height;
                if (ReferenceEquals(image.Rendered, image.Buffer) && image.Map?.Length == columns * rows && sized) continue;
                changed.Add(image);
                if (image.Map == null || image.Map.Length != columns * rows || image.Rendered == null || !sized)
                {
                    image.Map = BuildMap(set, image.Buffer);
                    continue;
                }
                int[] map = (int[])image.Map.Clone();
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        int cell = row * columns + column;
                        int visibleW = Math.Min(set.TileWidth, sprite.Width - column * set.TileWidth), visibleH = Math.Min(set.TileHeight, sprite.Height - row * set.TileHeight);
                        // Only cells painted since the last drawing count; other cells using the same tile must not undo the edit.
                        PixelBuffer tile = ReadTile(image.Buffer, set, column, row);
                        if (SameVisible(tile, ReadTile(image.Rendered, set, column, row), visibleW, visibleH)) continue;
                        int old = map[cell] > 0 && map[cell] < set.Tiles.Count ? map[cell] : 0;
                        if (shareEdits && old > 0 && !IsEmpty(tile))
                        {
                            // Keep the part of the shared tile that lies past the sprite edge for this cell.
                            PixelBuffer merged = set.Tiles[old].Clone();
                            for (int y = 0; y < visibleH; y++) Array.Copy(tile.Pixels, y * set.TileWidth, merged.Pixels, y * set.TileWidth, visibleW);
                            merged.Frozen = true;
                            set.Tiles[old] = merged;
                            tilesChanged = true;
                        }
                        else
                        {
                            map[cell] = set.AddOrFind(tile);
                        }
                    }
                }
                image.Map = map;
            }
            foreach (CelImage image in tilesChanged ? images : changed)
            {
                image.Buffer = Render(set, image.Map!, sprite.Width, sprite.Height);
                image.Rendered = image.Buffer;
            }
        }
    }

    /// <summary>Places a tile in one cell of a cel's map and redraws the cel.</summary>
    public static void SetTile(Sprite sprite, PixelLayer layer, CelImage image, int column, int row, int tile)
    {
        if (layer.Tileset is not Tileset set) return;
        int columns = set.Columns(sprite.Width), rows = set.Rows(sprite.Height);
        if ((uint)column >= (uint)columns || (uint)row >= (uint)rows) return;
        int[] map = image.Map is int[] existing && existing.Length == columns * rows ? (int[])existing.Clone() : BuildMap(set, image.Buffer);
        map[row * columns + column] = Math.Clamp(tile, 0, set.Tiles.Count - 1);
        image.Map = map;
        image.Buffer = Render(set, map, sprite.Width, sprite.Height);
        image.Rendered = image.Buffer;
    }

    /// <summary>The tile in a cell of a cel, or 0.</summary>
    public static int TileAt(Sprite sprite, PixelLayer layer, CelImage? image, int column, int row)
    {
        if (layer.Tileset is not Tileset set || image?.Map is not int[] map) return 0;
        int columns = set.Columns(sprite.Width);
        int index = row * columns + column;
        return column >= 0 && column < columns && index >= 0 && index < map.Length ? map[index] : 0;
    }

    /// <summary>Removes tiles no map uses and renumbers the rest. Returns how many were removed.</summary>
    public static int RemoveUnused(PixelLayer layer)
    {
        if (layer.Tileset is not Tileset set) return 0;
        List<CelImage> images = layer.Cels.Where(c => c?.Image.Map != null).Select(c => c!.Image).Distinct().ToList();
        HashSet<int> used = [0];
        foreach (CelImage image in images)
            foreach (int tile in image.Map!) used.Add(tile);
        int[] remap = new int[set.Tiles.Count];
        List<PixelBuffer> kept = [];
        for (int i = 0; i < set.Tiles.Count; i++)
        {
            if (!used.Contains(i)) continue;
            remap[i] = kept.Count;
            kept.Add(set.Tiles[i]);
        }
        int removed = set.Tiles.Count - kept.Count;
        if (removed == 0) return 0;
        set.Tiles.Clear();
        set.Tiles.AddRange(kept);
        foreach (CelImage image in images) image.Map = image.Map!.Select(t => t >= 0 && t < remap.Length ? remap[t] : 0).ToArray();
        return removed;
    }

    /// <summary>Lays the tiles out in rows on one image, for exporting a tileset.</summary>
    public static PixelBuffer RenderTileset(Tileset set, int columns)
    {
        int count = Math.Max(1, set.Tiles.Count - 1);
        columns = Math.Clamp(columns, 1, count);
        int rows = (count + columns - 1) / columns;
        PixelBuffer sheet = new(columns * set.TileWidth, rows * set.TileHeight);
        for (int i = 1; i < set.Tiles.Count; i++)
        {
            int n = i - 1;
            sheet.Blit(set.Tiles[i], n % columns * set.TileWidth, n / columns * set.TileHeight, false);
        }
        return sheet;
    }
}
