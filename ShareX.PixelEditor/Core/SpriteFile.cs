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
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ShareX.HelpersLib;

namespace ShareX.PixelEditor.Core;

public enum SheetLayout
{
    Horizontal,
    Vertical,
    Grid,
    /// <summary>Packs frames of different sizes tightly; used with trimming.</summary>
    Packed
}

public sealed class ExportOptions
{
    public int Scale { get; set; } = 1;

    /// <summary>The frames to export, in playback order.</summary>
    public List<int> Frames { get; set; } = [];

    public bool Loop { get; set; } = true;

    public SheetLayout Layout { get; set; } = SheetLayout.Horizontal;

    public int Columns { get; set; } = 8;

    public int Padding { get; set; }

    public int Spacing { get; set; }

    public bool WriteMetadata { get; set; } = true;

    /// <summary>Crops every frame on a sheet to its visible pixels.</summary>
    public bool Trim { get; set; }

    /// <summary>Frames that look the same share one image on a sheet.</summary>
    public bool MergeDuplicates { get; set; }

    /// <summary>Layers to include; null exports every visible layer.</summary>
    public Func<Layer, bool>? LayerFilter { get; set; }
}

/// <summary>Reads and writes sprites: the project format, images, animations and sprite sheets.</summary>
public static class SpriteFile
{
    public const string ProjectExtension = ".sxpixel";

    public static readonly string[] ImageExtensions = [".png", ".gif", ".webp", ".jpg", ".jpeg", ".bmp", ".ico"];

    public static bool IsProject(string path) => Path.GetExtension(path).Equals(ProjectExtension, StringComparison.OrdinalIgnoreCase);

    public static bool IsSupported(string path) => IsProject(path) || ImageExtensions.Contains(Path.GetExtension(path).ToLowerInvariant());

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    // ------------------------------------------------------------------ project format

    private sealed class ProjectDto
    {
        public int Version { get; set; } = 1;
        public int Width { get; set; }
        public int Height { get; set; }
        public List<int> Frames { get; set; } = [];
        public List<TagDto> Tags { get; set; } = [];
        public List<SliceDto> Slices { get; set; } = [];
        public List<string> Palette { get; set; } = [];
        public List<ImageDto> Images { get; set; } = [];
        public List<LayerDto> Layers { get; set; } = [];
        public int ActiveFrame { get; set; }
    }

    private sealed class TagDto
    {
        public string Name { get; set; } = string.Empty;
        public int From { get; set; }
        public int To { get; set; }
        public string Color { get; set; } = "4A90D9";
        public TagDirection Direction { get; set; }
        public int Repeat { get; set; }
    }

    private sealed class SliceDto
    {
        public string Name { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Color { get; set; } = "3FB950";
        public int[]? Center { get; set; }
        public int[]? Pivot { get; set; }
    }

    private sealed class ImageDto
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    private sealed class CelDto
    {
        public int Image { get; set; }
        public int Opacity { get; set; } = 255;
        public int[]? Map { get; set; }
    }

    private sealed class LayerDto
    {
        public string Name { get; set; } = string.Empty;
        public bool Group { get; set; }
        public bool Visible { get; set; } = true;
        public bool Locked { get; set; }
        public bool Reference { get; set; }
        public bool Collapsed { get; set; }
        public bool Active { get; set; }
        public int Opacity { get; set; } = 255;
        public BlendMode Blend { get; set; }
        public List<CelDto?>? Cels { get; set; }
        public List<LayerDto>? Children { get; set; }
        public int? TileWidth { get; set; }
        public int? TileHeight { get; set; }
        public int? Tileset { get; set; }
        public int? TileCount { get; set; }
    }

    public static void SaveProject(Sprite sprite, string path)
    {
        ProjectDto dto = new()
        {
            Width = sprite.Width,
            Height = sprite.Height,
            Frames = sprite.Frames.Select(f => f.Duration).ToList(),
            Tags = sprite.Tags.Select(t => new TagDto { Name = t.Name, From = t.From, To = t.To, Color = Argb.ToHex(t.Color), Direction = t.Direction, Repeat = t.Repeat }).ToList(),
            Slices = sprite.Slices.Select(s => new SliceDto
            {
                Name = s.Name,
                X = s.Bounds.Left,
                Y = s.Bounds.Top,
                Width = s.Bounds.Width,
                Height = s.Bounds.Height,
                Color = Argb.ToHex(s.Color),
                Center = s.Center is SKRectI c ? [c.Left, c.Top, c.Width, c.Height] : null,
                Pivot = s.Pivot is SKPointI p ? [p.X, p.Y] : null
            }).ToList(),
            Palette = sprite.Palette.Select(Argb.ToHex).ToList(),
            ActiveFrame = sprite.ActiveFrame
        };
        Dictionary<CelImage, int> images = [];
        List<(PixelBuffer Buffer, SKRectI Bounds)> pending = [];
        List<PixelBuffer> tilesets = [];

        LayerDto Convert(Layer layer)
        {
            LayerDto result = new() { Name = layer.Name, Visible = layer.Visible, Locked = layer.Locked, Opacity = layer.Opacity, Blend = layer.Blend, Active = layer == sprite.ActiveLayer };
            if (layer is GroupLayer group)
            {
                result.Group = true;
                result.Collapsed = group.Collapsed;
                result.Children = group.Children.Select(Convert).ToList();
            }
            else if (layer is PixelLayer pixels)
            {
                result.Reference = pixels.IsReference;
                if (pixels.Tileset is Tileset set)
                {
                    result.TileWidth = set.TileWidth;
                    result.TileHeight = set.TileHeight;
                    result.TileCount = set.Tiles.Count - 1;
                    result.Tileset = tilesets.Count;
                    tilesets.Add(Tilemaps.RenderTileset(set, Math.Max(1, set.Tiles.Count - 1)));
                }
                result.Cels = [];
                foreach (Cel? cel in pixels.Cels)
                {
                    SKRectI? bounds = cel?.Buffer.ContentBounds();
                    if (cel == null || bounds == null)
                    {
                        result.Cels.Add(null);
                        continue;
                    }
                    if (!images.TryGetValue(cel.Image, out int index))
                    {
                        index = pending.Count;
                        images[cel.Image] = index;
                        pending.Add((cel.Buffer, bounds.Value));
                        dto.Images.Add(new ImageDto { X = bounds.Value.Left, Y = bounds.Value.Top });
                    }
                    result.Cels.Add(new CelDto { Image = index, Opacity = cel.Opacity, Map = pixels.Tileset != null ? cel.Image.Map : null });
                }
            }
            return result;
        }

        dto.Layers = sprite.Root.Children.Select(Convert).ToList();

        string temp = path + ".tmp";
        using (FileStream stream = new(temp, FileMode.Create, FileAccess.Write))
        using (ZipArchive zip = new(stream, ZipArchiveMode.Create))
        {
            using (Stream entry = zip.CreateEntry("sprite.json", CompressionLevel.Optimal).Open())
            {
                JsonSerializer.Serialize(entry, dto, JsonOptions);
            }
            for (int i = 0; i < pending.Count; i++)
            {
                PixelBuffer cropped = pending[i].Buffer.Crop(pending[i].Bounds);
                // PNG data is already compressed.
                using Stream entry = zip.CreateEntry($"cels/{i}.png", CompressionLevel.NoCompression).Open();
                byte[] png = EncodeImage(cropped, ".png", 100);
                entry.Write(png);
            }
            for (int i = 0; i < tilesets.Count; i++)
            {
                using Stream entry = zip.CreateEntry($"tilesets/{i}.png", CompressionLevel.NoCompression).Open();
                entry.Write(EncodeImage(tilesets[i], ".png", 100));
            }
        }
        File.Move(temp, path, true);
    }

    /// <summary>Reads a tileset saved as one row of tiles; missing or damaged images give an empty tileset.</summary>
    private static Tileset LoadTileset(ZipArchive zip, int index, int tileWidth, int tileHeight, int count)
    {
        Tileset set = new(tileWidth, tileHeight);
        ZipArchiveEntry? entry = index >= 0 ? zip.GetEntry($"tilesets/{index}.png") : null;
        if (entry == null || count <= 0) return set;
        using MemoryStream memory = new();
        using (Stream s = entry.Open()) s.CopyTo(memory);
        using SKBitmap? bitmap = SKBitmap.Decode(memory.ToArray());
        if (bitmap == null) return set;
        PixelBuffer strip = PixelBuffer.FromBitmap(bitmap);
        for (int i = 0; i < count; i++)
        {
            PixelBuffer tile = strip.Crop(SKRectI.Create(i * tileWidth, 0, tileWidth, tileHeight));
            tile.Frozen = true;
            set.Tiles.Add(tile);
        }
        return set;
    }

    public static Sprite LoadProject(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using ZipArchive zip = new(stream, ZipArchiveMode.Read);
        ZipArchiveEntry json = zip.GetEntry("sprite.json") ?? throw new InvalidDataException();
        ProjectDto dto;
        using (Stream entry = json.Open())
        {
            dto = JsonSerializer.Deserialize<ProjectDto>(entry, JsonOptions) ?? throw new InvalidDataException();
        }
        if (dto.Width < 1 || dto.Height < 1 || dto.Width > Sprite.MaxSize || dto.Height > Sprite.MaxSize) throw new InvalidDataException();
        Sprite sprite = new(dto.Width, dto.Height) { Title = Path.GetFileNameWithoutExtension(path), FilePath = path };
        Dictionary<int, CelImage> images = [];

        CelImage? LoadImage(int index)
        {
            if (images.TryGetValue(index, out CelImage? image)) return image;
            if (index < 0 || index >= dto.Images.Count) return null;
            ZipArchiveEntry? entry = zip.GetEntry($"cels/{index}.png");
            if (entry == null) return null;
            using MemoryStream memory = new();
            using (Stream s = entry.Open()) s.CopyTo(memory);
            using SKBitmap? bitmap = SKBitmap.Decode(memory.ToArray());
            if (bitmap == null) return null;
            PixelBuffer buffer = new(dto.Width, dto.Height);
            buffer.Blit(PixelBuffer.FromBitmap(bitmap), dto.Images[index].X, dto.Images[index].Y, false);
            image = new CelImage(buffer);
            images[index] = image;
            return image;
        }

        Layer? active = null;
        Layer Convert(LayerDto source)
        {
            Layer layer;
            if (source.Group)
            {
                GroupLayer group = new() { Collapsed = source.Collapsed };
                foreach (LayerDto child in source.Children ?? [])
                {
                    Layer converted = Convert(child);
                    converted.Parent = group;
                    group.Children.Add(converted);
                }
                layer = group;
            }
            else
            {
                PixelLayer pixels = new() { IsReference = source.Reference };
                if (source.TileWidth is int tileWidth && source.TileHeight is int tileHeight && tileWidth > 0 && tileHeight > 0)
                {
                    pixels.Tileset = LoadTileset(zip, source.Tileset ?? -1, tileWidth, tileHeight, source.TileCount ?? 0);
                }
                foreach (CelDto? cel in source.Cels ?? [])
                {
                    CelImage? image = cel != null ? LoadImage(cel.Image) : null;
                    if (image != null && pixels.Tileset is Tileset set && cel!.Map is int[] map && map.Length == set.Columns(dto.Width) * set.Rows(dto.Height))
                    {
                        image.Map = map.Select(t => t >= 0 && t < set.Tiles.Count ? t : 0).ToArray();
                        image.Rendered = image.Buffer;
                    }
                    pixels.Cels.Add(image != null ? new Cel(image) { Opacity = Math.Clamp(cel!.Opacity, 0, 255) } : null);
                }
                layer = pixels;
            }
            layer.Name = source.Name;
            layer.Visible = source.Visible;
            layer.Locked = source.Locked;
            layer.Opacity = Math.Clamp(source.Opacity, 0, 255);
            layer.Blend = source.Blend;
            if (source.Active) active = layer;
            return layer;
        }

        GroupLayer root = new();
        foreach (LayerDto source in dto.Layers)
        {
            Layer layer = Convert(source);
            layer.Parent = root;
            root.Children.Add(layer);
        }
        List<Tag> tags = dto.Tags.Select(t => new Tag { Name = t.Name, From = t.From, To = t.To, Color = Argb.TryParseHex(t.Color, out uint c) ? c : 0xFF4A90D9, Direction = t.Direction, Repeat = t.Repeat }).ToList();
        List<Slice> slices = dto.Slices.Select(s => new Slice
        {
            Name = s.Name,
            Bounds = SKRectI.Create(s.X, s.Y, Math.Max(1, s.Width), Math.Max(1, s.Height)),
            Color = Argb.TryParseHex(s.Color, out uint c) ? c : 0xFF3FB950,
            Center = s.Center is { Length: 4 } r ? SKRectI.Create(r[0], r[1], Math.Max(1, r[2]), Math.Max(1, r[3])) : null,
            Pivot = s.Pivot is { Length: 2 } p ? new SKPointI(p[0], p[1]) : null
        }).ToList();
        int frameCount = Math.Max(1, dto.Frames.Count);
        tags.RemoveAll(t => t.From < 0 || t.To >= frameCount || t.To < t.From);
        sprite.Adopt(root, dto.Frames.Select(d => new FrameInfo { Duration = Math.Clamp(d, 1, 65535) }).ToList(), tags, slices);
        sprite.Palette = dto.Palette.Select(h => Argb.TryParseHex(h, out uint c) ? c : 0).Where(c => c != 0).ToList();
        if (active != null) sprite.ActiveLayer = active;
        sprite.SetActiveFrame(dto.ActiveFrame);
        return sprite;
    }

    // ------------------------------------------------------------------ import

    /// <summary>Opens a project or an image. Animated GIF and WebP files become one frame per animation frame.</summary>
    public static Sprite Load(string path, string layerName)
    {
        if (IsProject(path)) return LoadProject(path);
        string title = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is ".gif" or ".webp")
        {
            Sprite? animated = LoadAnimation(path, title, layerName);
            if (animated != null)
            {
                animated.FilePath = path;
                return animated;
            }
        }
        using SKBitmap bitmap = SkiaImageHelpers.LoadImage(path) ?? throw new InvalidDataException();
        if (bitmap.Width > Sprite.MaxSize || bitmap.Height > Sprite.MaxSize) throw new InvalidDataException();
        Sprite sprite = Sprite.FromBuffer(PixelBuffer.FromBitmap(bitmap), title, layerName);
        sprite.FilePath = path;
        return sprite;
    }

    private static Sprite? LoadAnimation(string path, string title, string layerName)
    {
        using SKCodec? codec = SKCodec.Create(path);
        if (codec == null || codec.FrameCount <= 1) return null;
        int width = codec.Info.Width, height = codec.Info.Height;
        if (width > Sprite.MaxSize || height > Sprite.MaxSize) return null;
        SKImageInfo info = new(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        Sprite sprite = new(width, height) { Title = title };
        PixelLayer layer = sprite.CreatePixelLayer(layerName);
        List<FrameInfo> frames = [];
        SKCodecFrameInfo[] frameInfos = codec.FrameInfo;
        for (int i = 0; i < codec.FrameCount; i++)
        {
            using SKBitmap bitmap = new(info);
            SKCodecResult result = codec.GetPixels(info, bitmap.GetPixels(), new SKCodecOptions(i));
            if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput) break;
            PixelBuffer buffer = PixelBuffer.FromBitmap(bitmap);
            layer.Cels.Add(buffer.IsEmpty ? null : new Cel(new CelImage(buffer)));
            int duration = i < frameInfos.Length ? frameInfos[i].Duration : 100;
            frames.Add(new FrameInfo { Duration = duration <= 0 ? 100 : duration });
        }
        if (frames.Count == 0) return null;
        layer.Cels.RemoveAt(0);
        GroupLayer root = new();
        layer.Parent = root;
        root.Children.Add(layer);
        // The layer was created with one empty cel for the initial frame; the decoded cels replace it.
        sprite.Adopt(root, frames, []);
        return sprite;
    }

    /// <summary>Cuts a sprite sheet into frames of a fixed cell size.</summary>
    public static Sprite FromSheet(PixelBuffer sheet, int cellWidth, int cellHeight, int padding, int spacing, bool skipEmpty, string title, string layerName, int duration = 100)
    {
        cellWidth = Math.Clamp(cellWidth, 1, sheet.Width);
        cellHeight = Math.Clamp(cellHeight, 1, sheet.Height);
        Sprite sprite = new(cellWidth, cellHeight) { Title = title };
        PixelLayer layer = sprite.CreatePixelLayer(layerName);
        layer.Cels.Clear();
        List<FrameInfo> frames = [];
        for (int y = padding; y + cellHeight <= sheet.Height; y += cellHeight + spacing)
        {
            for (int x = padding; x + cellWidth <= sheet.Width; x += cellWidth + spacing)
            {
                PixelBuffer cell = sheet.Crop(SKRectI.Create(x, y, cellWidth, cellHeight));
                bool empty = cell.IsEmpty;
                if (empty && skipEmpty) continue;
                layer.Cels.Add(empty ? null : new Cel(new CelImage(cell)));
                frames.Add(new FrameInfo { Duration = duration });
            }
        }
        if (frames.Count == 0)
        {
            frames.Add(new FrameInfo());
            layer.Cels.Add(null);
        }
        GroupLayer root = new();
        layer.Parent = root;
        root.Children.Add(layer);
        sprite.Adopt(root, frames, []);
        return sprite;
    }

    // ------------------------------------------------------------------ export

    public static byte[] EncodeImage(PixelBuffer buffer, string extension, int quality)
    {
        using SKBitmap bitmap = buffer.ToBitmap();
        SKEncodedImageFormat format = extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            ".webp" => SKEncodedImageFormat.Webp,
            _ => SKEncodedImageFormat.Png
        };
        if (format == SKEncodedImageFormat.Jpeg)
        {
            // JPEG has no alpha: flatten on white.
            using SKBitmap opaque = new(new SKImageInfo(buffer.Width, buffer.Height, SKColorType.Bgra8888, SKAlphaType.Opaque));
            using (SKCanvas canvas = new(opaque))
            {
                canvas.Clear(SKColors.White);
                canvas.DrawBitmap(bitmap, 0, 0);
            }
            using SKData jpeg = opaque.Encode(format, quality);
            return jpeg.ToArray();
        }
        using SKData data = bitmap.Encode(format, quality);
        return data.ToArray();
    }

    public static PixelBuffer RenderFrame(Sprite sprite, int frame, ExportOptions options)
    {
        PixelBuffer flat = Compositor.Flatten(sprite, frame, false, options.LayerFilter);
        return options.Scale > 1 ? flat.ScaleNearest(sprite.Width * options.Scale, sprite.Height * options.Scale) : flat;
    }

    public static void ExportImage(Sprite sprite, string path, int frame, ExportOptions options, int quality = 95)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        PixelBuffer image = RenderFrame(sprite, frame, options);
        if (extension == ".gif")
        {
            using FileStream stream = File.Create(path);
            ToIndexed(image).Image.SaveGif(stream);
            return;
        }
        File.WriteAllBytes(path, EncodeImage(image, extension, quality));
    }

    /// <summary>Converts a frame to a palette image for GIF. Up to 255 colors are kept exactly; more are reduced.</summary>
    private static (IndexedImage Image, int Transparent) ToIndexed(PixelBuffer buffer)
    {
        List<uint> colors = Palettes.FromImage(buffer, 255);
        Dictionary<uint, byte> lookup = [];
        for (int i = 0; i < colors.Count; i++) lookup[colors[i]] = (byte)(i + 1);
        byte[] pixels = new byte[buffer.Width * buffer.Height];
        bool exact = true;
        for (int i = 0; i < pixels.Length; i++)
        {
            uint p = buffer.Pixels[i];
            if (Argb.A(p) < 128) continue;
            uint key = Argb.Opaque(p);
            if (!lookup.TryGetValue(key, out byte index))
            {
                exact = false;
                index = (byte)(Palettes.Nearest(colors, key) + 1);
                lookup[key] = index;
            }
            pixels[i] = index;
        }
        _ = exact;
        System.Drawing.Color[] palette = new System.Drawing.Color[colors.Count + 1];
        palette[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);
        for (int i = 0; i < colors.Count; i++) palette[i + 1] = System.Drawing.Color.FromArgb(255, Argb.R(colors[i]), Argb.G(colors[i]), Argb.B(colors[i]));
        return (new IndexedImage(buffer.Width, buffer.Height, pixels, palette), 0);
    }

    public static void ExportGif(Sprite sprite, string path, ExportOptions options, IProgress<float>? progress = null)
    {
        using AnimatedGifCreator gif = new(path, 100, 0, options.Loop);
        for (int i = 0; i < options.Frames.Count; i++)
        {
            int frame = options.Frames[i];
            (IndexedImage image, int transparent) = ToIndexed(RenderFrame(sprite, frame, options));
            gif.AddFrame(image, sprite.Frames[frame].Duration, 0, 0, true, transparent);
            progress?.Report((i + 1) / (float)options.Frames.Count);
        }
    }

    /// <summary>Writes an animated PNG, which keeps full color and alpha.</summary>
    public static void ExportApng(Sprite sprite, string path, ExportOptions options, IProgress<float>? progress = null)
    {
        int width = sprite.Width * Math.Max(1, options.Scale), height = sprite.Height * Math.Max(1, options.Scale);
        using FileStream stream = File.Create(path);
        stream.Write([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);
        byte[] header = new byte[13];
        WriteInt(header, 0, width);
        WriteInt(header, 4, height);
        header[8] = 8;
        header[9] = 6;
        WriteChunk(stream, "IHDR", header);
        byte[] control = new byte[8];
        WriteInt(control, 0, options.Frames.Count);
        WriteInt(control, 4, options.Loop ? 0 : 1);
        WriteChunk(stream, "acTL", control);
        int sequence = 0;
        for (int i = 0; i < options.Frames.Count; i++)
        {
            int frame = options.Frames[i];
            PixelBuffer image = RenderFrame(sprite, frame, options);
            byte[] frameControl = new byte[26];
            WriteInt(frameControl, 0, sequence++);
            WriteInt(frameControl, 4, width);
            WriteInt(frameControl, 8, height);
            int duration = Math.Clamp(sprite.Frames[frame].Duration, 1, 65535);
            frameControl[20] = (byte)(duration >> 8);
            frameControl[21] = (byte)duration;
            frameControl[22] = 1000 >> 8;
            frameControl[23] = 1000 & 0xFF;
            // Dispose to background and replace, so every frame is independent.
            frameControl[24] = 1;
            frameControl[25] = 0;
            WriteChunk(stream, "fcTL", frameControl);
            byte[] data = CompressScanlines(image);
            if (i == 0) WriteChunk(stream, "IDAT", data);
            else
            {
                byte[] frameData = new byte[data.Length + 4];
                WriteInt(frameData, 0, sequence++);
                data.CopyTo(frameData, 4);
                WriteChunk(stream, "fdAT", frameData);
            }
            progress?.Report((i + 1) / (float)options.Frames.Count);
        }
        WriteChunk(stream, "IEND", []);
    }

    private static byte[] CompressScanlines(PixelBuffer image)
    {
        using MemoryStream memory = new();
        using (ZLibStream zlib = new(memory, CompressionLevel.Optimal, true))
        {
            byte[] row = new byte[1 + image.Width * 4];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    uint p = image.Pixels[y * image.Width + x];
                    row[1 + x * 4] = (byte)Argb.R(p);
                    row[2 + x * 4] = (byte)Argb.G(p);
                    row[3 + x * 4] = (byte)Argb.B(p);
                    row[4 + x * 4] = (byte)Argb.A(p);
                }
                zlib.Write(row);
            }
        }
        return memory.ToArray();
    }

    private static void WriteInt(byte[] buffer, int offset, int value)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }

    private static readonly uint[] CrcTable = CreateCrcTable();

    private static uint[] CreateCrcTable()
    {
        uint[] table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            table[n] = c;
        }
        return table;
    }

    private static void WriteChunk(Stream stream, string type, byte[] data)
    {
        byte[] length = new byte[4];
        WriteInt(length, 0, data.Length);
        stream.Write(length);
        byte[] name = Encoding.ASCII.GetBytes(type);
        stream.Write(name);
        stream.Write(data);
        uint crc = 0xFFFFFFFF;
        foreach (byte b in name) crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        foreach (byte b in data) crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        byte[] check = new byte[4];
        WriteInt(check, 0, (int)(crc ^ 0xFFFFFFFF));
        stream.Write(check);
    }

    /// <summary>Writes numbered images, one per frame, next to the given path (name_0001.png, ...).</summary>
    public static List<string> ExportSequence(Sprite sprite, string path, ExportOptions options)
    {
        string directory = Path.GetDirectoryName(path) ?? string.Empty, name = Path.GetFileNameWithoutExtension(path), extension = Path.GetExtension(path);
        List<string> files = [];
        int digits = Math.Max(2, options.Frames.Count.ToString(CultureInfo.InvariantCulture).Length);
        for (int i = 0; i < options.Frames.Count; i++)
        {
            string file = Path.Combine(directory, $"{name}_{(i + 1).ToString(CultureInfo.InvariantCulture).PadLeft(digits, '0')}{extension}");
            File.WriteAllBytes(file, EncodeImage(RenderFrame(sprite, options.Frames[i], options), extension, 95));
            files.Add(file);
        }
        return files;
    }

    /// <summary>Where every frame goes on a sprite sheet: frames that look the same can share one image, and trimmed frames remember their place in the full frame.</summary>
    public sealed class SheetPlan
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<PixelBuffer> Images { get; } = [];
        public List<SKPointI> Positions { get; } = [];
        public List<SheetCell> Cells { get; } = [];
    }

    /// <summary>One frame on a sheet: its rectangle on the sheet and the part of the full frame it holds.</summary>
    public sealed record SheetCell(SKRectI Frame, SKRectI Source, int SourceWidth, int SourceHeight);

    public static SheetPlan PlanSheet(Sprite sprite, ExportOptions options)
    {
        int scale = Math.Max(1, options.Scale), fullW = sprite.Width * scale, fullH = sprite.Height * scale;
        SheetPlan plan = new();
        List<int> imageOf = [];
        List<SKRectI> sources = [];
        foreach (int frame in options.Frames)
        {
            PixelBuffer image = RenderFrame(sprite, frame, options);
            SKRectI source = SKRectI.Create(0, 0, fullW, fullH);
            if (options.Trim)
            {
                source = image.ContentBounds() ?? SKRectI.Create(0, 0, 1, 1);
                image = image.Crop(source);
            }
            int index = options.MergeDuplicates ? plan.Images.FindIndex(i => i.Width == image.Width && i.Height == image.Height && i.Pixels.AsSpan().SequenceEqual(image.Pixels)) : -1;
            if (index < 0)
            {
                index = plan.Images.Count;
                plan.Images.Add(image);
            }
            imageOf.Add(index);
            sources.Add(source);
        }
        int count = Math.Max(1, plan.Images.Count), padding = options.Padding, spacing = options.Spacing;
        if (options.Layout == SheetLayout.Packed)
        {
            // Shelf packing: tallest images first, filling rows up to a width that keeps the sheet roughly square.
            long area = plan.Images.Sum(i => (long)(i.Width + spacing) * (i.Height + spacing));
            int widest = plan.Images.Count > 0 ? plan.Images.Max(i => i.Width) : 1;
            int rowWidth = Math.Max(widest, (int)Math.Ceiling(Math.Sqrt(area)));
            SKPointI[] positions = new SKPointI[plan.Images.Count];
            int x = 0, y = 0, shelf = 0, usedW = 0;
            foreach (int i in Enumerable.Range(0, plan.Images.Count).OrderByDescending(i => plan.Images[i].Height).ThenByDescending(i => plan.Images[i].Width))
            {
                PixelBuffer image = plan.Images[i];
                if (x > 0 && x + image.Width > rowWidth)
                {
                    y += shelf + spacing;
                    x = 0;
                    shelf = 0;
                }
                positions[i] = new SKPointI(padding + x, padding + y);
                x += image.Width + spacing;
                usedW = Math.Max(usedW, x - spacing);
                shelf = Math.Max(shelf, image.Height);
            }
            plan.Positions.AddRange(positions);
            plan.Width = padding * 2 + Math.Max(1, usedW);
            plan.Height = padding * 2 + Math.Max(1, y + shelf);
        }
        else
        {
            int cellW = options.Trim && plan.Images.Count > 0 ? plan.Images.Max(i => i.Width) : fullW;
            int cellH = options.Trim && plan.Images.Count > 0 ? plan.Images.Max(i => i.Height) : fullH;
            int columns = options.Layout switch
            {
                SheetLayout.Horizontal => count,
                SheetLayout.Vertical => 1,
                _ => Math.Clamp(options.Columns, 1, count)
            };
            int rows = (count + columns - 1) / columns;
            for (int i = 0; i < plan.Images.Count; i++) plan.Positions.Add(new SKPointI(padding + i % columns * (cellW + spacing), padding + i / columns * (cellH + spacing)));
            plan.Width = padding * 2 + columns * cellW + (columns - 1) * spacing;
            plan.Height = padding * 2 + rows * cellH + (rows - 1) * spacing;
        }
        for (int i = 0; i < imageOf.Count; i++)
        {
            PixelBuffer image = plan.Images[imageOf[i]];
            SKPointI at = plan.Positions[imageOf[i]];
            plan.Cells.Add(new SheetCell(SKRectI.Create(at.X, at.Y, image.Width, image.Height), sources[i], fullW, fullH));
        }
        return plan;
    }

    /// <summary>Lays the frames out on one image.</summary>
    public static PixelBuffer RenderSheet(Sprite sprite, ExportOptions options, out List<SheetCell> cells)
    {
        SheetPlan plan = PlanSheet(sprite, options);
        PixelBuffer sheet = new(Math.Max(1, plan.Width), Math.Max(1, plan.Height));
        for (int i = 0; i < plan.Images.Count; i++) sheet.Blit(plan.Images[i], plan.Positions[i].X, plan.Positions[i].Y, false);
        cells = plan.Cells;
        return sheet;
    }
    /// <summary>Writes a sprite sheet image and, optionally, a JSON file describing its frames, tags and slices.</summary>
    public static void ExportSheet(Sprite sprite, string path, ExportOptions options)
    {
        PixelBuffer sheet = RenderSheet(sprite, options, out List<SheetCell> cells);
        File.WriteAllBytes(path, EncodeImage(sheet, Path.GetExtension(path), 95));
        if (!options.WriteMetadata) return;
        int scale = Math.Max(1, options.Scale);
        string name = Path.GetFileNameWithoutExtension(path);
        var metadata = new
        {
            frames = cells.Select((c, i) => new
            {
                filename = $"{name} {i}",
                frame = new { x = c.Frame.Left, y = c.Frame.Top, w = c.Frame.Width, h = c.Frame.Height },
                rotated = false,
                trimmed = options.Trim,
                spriteSourceSize = new { x = c.Source.Left, y = c.Source.Top, w = c.Source.Width, h = c.Source.Height },
                sourceSize = new { w = c.SourceWidth, h = c.SourceHeight },
                duration = sprite.Frames[options.Frames[i]].Duration
            }).ToList(),
            meta = new
            {
                app = "ShareX Pixel Editor",
                image = Path.GetFileName(path),
                format = "RGBA8888",
                size = new { w = sheet.Width, h = sheet.Height },
                scale = scale.ToString(CultureInfo.InvariantCulture),
                frameTags = sprite.Tags.Select(t => new { name = t.Name, from = t.From, to = t.To, direction = t.Direction.ToString().ToLowerInvariant(), color = "#" + Argb.ToHex(t.Color) }).ToList(),
                slices = sprite.Slices.Select(s => new
                {
                    name = s.Name,
                    color = "#" + Argb.ToHex(s.Color),
                    keys = new[]
                    {
                        new
                        {
                            frame = 0,
                            bounds = new { x = s.Bounds.Left * scale, y = s.Bounds.Top * scale, w = s.Bounds.Width * scale, h = s.Bounds.Height * scale },
                            center = s.Center is SKRectI c ? new { x = c.Left * scale, y = c.Top * scale, w = c.Width * scale, h = c.Height * scale } : null,
                            pivot = s.Pivot is SKPointI p ? new { x = p.X * scale, y = p.Y * scale } : null
                        }
                    }
                }).ToList()
            }
        };
        File.WriteAllText(Path.ChangeExtension(path, ".json"), JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
    }
}
