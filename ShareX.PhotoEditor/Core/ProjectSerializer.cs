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

using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShareX.PhotoEditor.Core;

/// <summary>
/// Native layered project format (.sxpe): a zip archive containing document.json and PNG files for
/// layer pixels, masks, smart object sources, alpha channels and the selection.
/// </summary>
public static class ProjectSerializer
{
    public const int FormatVersion = 1;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(), new ColorConverter(), new PointConverter(), new SizeConverter(), new RectConverter(), new RectIConverter(), new MatrixConverter() }
    };

    private sealed class DocumentDto
    {
        public int Version { get; set; }
        public string Application { get; set; } = "ShareX Photo editor";
        public int Width { get; set; }
        public int Height { get; set; }
        public float Resolution { get; set; }
        public int ActiveLayerIndex { get; set; }
        public List<LayerDto> Layers { get; set; } = [];
        public string? Selection { get; set; }
        public List<ChannelDto> Channels { get; set; } = [];
        public List<Guide> Guides { get; set; } = [];
        public List<NoteItem> Notes { get; set; } = [];
        public List<CountItem> Counts { get; set; } = [];
        public List<CountGroup> CountGroups { get; set; } = [];
        public List<ColorSamplerPoint> Samplers { get; set; } = [];
        public List<SliceItem> Slices { get; set; } = [];
        public List<NamedPathDto> Paths { get; set; } = [];
        public PathDto? WorkPath { get; set; }
    }

    private sealed class SmartFilterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public SmartFilterRecipe Recipe { get; set; } = new();
    }

    private sealed class ChannelDto
    {
        public string Name { get; set; } = string.Empty;
        public string File { get; set; } = string.Empty;
    }

    private sealed class NamedPathDto
    {
        public string Name { get; set; } = string.Empty;
        public PathDto Path { get; set; } = new();
    }

    private sealed class PathDto
    {
        public SKPathFillType FillType { get; set; }
        public List<SubPathDto> SubPaths { get; set; } = [];

        public static PathDto From(VectorPath path) => new()
        {
            FillType = path.FillType,
            SubPaths = path.SubPaths.Select(s => new SubPathDto
            {
                Closed = s.Closed,
                Anchors = s.Anchors.Select(a => new[] { a.Point.X, a.Point.Y, a.In.X, a.In.Y, a.Out.X, a.Out.Y, a.Smooth ? 1f : 0f }).ToList()
            }).ToList()
        };

        public VectorPath ToPath()
        {
            VectorPath path = new() { FillType = FillType };
            foreach (SubPathDto s in SubPaths)
            {
                SubPath sub = new() { Closed = s.Closed };
                foreach (float[] a in s.Anchors.Where(a => a.Length >= 7))
                {
                    sub.Anchors.Add(new PathAnchor(new SKPoint(a[0], a[1]), new SKPoint(a[2], a[3]), new SKPoint(a[4], a[5]), a[6] > 0));
                }
                path.SubPaths.Add(sub);
            }
            return path;
        }
    }

    private sealed class SubPathDto
    {
        public bool Closed { get; set; }
        public List<float[]> Anchors { get; set; } = [];
    }

    private sealed class LayerDto
    {
        public LayerKind Kind { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public float Opacity { get; set; } = 1;
        public float FillOpacity { get; set; } = 1;
        public LayerBlendMode BlendMode { get; set; }
        public bool Clipped { get; set; }
        public bool LockTransparency { get; set; }
        public bool LockPixels { get; set; }
        public bool LockPosition { get; set; }
        public bool LockAll { get; set; }
        public int LinkId { get; set; }
        public bool Background { get; set; }
        public string? Mask { get; set; }
        public bool MaskEnabled { get; set; } = true;
        public LayerStyle? Style { get; set; }
        public SKColor? LabelColor { get; set; }

        public string? Pixels { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public string? Text { get; set; }
        public string? FontFamily { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool FauxBold { get; set; }
        public bool FauxItalic { get; set; }
        public bool Underline { get; set; }
        public bool Strikethrough { get; set; }
        public bool AllCaps { get; set; }
        public float FontSize { get; set; }
        public SKColor? Color { get; set; }
        public TextAlignment Alignment { get; set; }
        public bool Vertical { get; set; }
        public SKPoint Position { get; set; }
        public SKSize BoxSize { get; set; }
        public float Tracking { get; set; }
        public float Leading { get; set; }
        public TextAntiAlias AntiAlias { get; set; }
        public SKMatrix? Transform { get; set; }

        public PathDto? Path { get; set; }
        public SKColor? FillColor { get; set; }
        public SKColor? StrokeColor { get; set; }
        public float StrokeWidth { get; set; }
        public StrokePosition StrokeAlignment { get; set; }
        public bool Dashed { get; set; }

        public List<LayerDto>? Children { get; set; }
        public bool Expanded { get; set; }
        public SKRectI? Artboard { get; set; }
        public SKColor? ArtboardBackground { get; set; }

        public AdjustmentSettings? Adjustment { get; set; }

        public FillKind FillKind { get; set; }
        public GradientDefinition? Gradient { get; set; }
        public string? PatternName { get; set; }
        public string? Pattern { get; set; }
        public float PatternScale { get; set; } = 1;

        public string? Source { get; set; }
        public List<SmartFilterDto>? SmartFilters { get; set; }

        public SKRect Frame { get; set; }
        public bool Ellipse { get; set; }
        public string? Content { get; set; }
        public SKRect ContentRect { get; set; }
    }

    public static void Save(PhotoDocument doc, string path)
    {
        string temp = path + ".tmp";
        try
        {
            using (FileStream file = new(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            using (ZipArchive zip = new(file, ZipArchiveMode.Create))
            {
                int counter = 0;
                string WritePng(SKBitmap bitmap, string folder)
                {
                    string name = $"{folder}/{counter++:0000}.png";
                    ZipArchiveEntry entry = zip.CreateEntry(name, CompressionLevel.Fastest);
                    using Stream stream = entry.Open();
                    using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                    data.SaveTo(stream);
                    return name;
                }
                string WriteMask(PixelSurface mask, string folder)
                {
                    using SKBitmap gray = ImageOps.MaskToGray(mask);
                    return WritePng(gray, folder);
                }

                LayerDto ToDto(Layer layer)
                {
                    LayerDto dto = new()
                    {
                        Kind = layer.Kind,
                        Name = layer.Name,
                        Visible = layer.Visible,
                        Opacity = layer.Opacity,
                        FillOpacity = layer.FillOpacity,
                        BlendMode = layer.BlendMode,
                        Clipped = layer.IsClipped,
                        LockTransparency = layer.LockTransparency,
                        LockPixels = layer.LockPixels,
                        LockPosition = layer.LockPosition,
                        LockAll = layer.LockAll,
                        LinkId = layer.LinkId,
                        Background = layer.IsBackground,
                        MaskEnabled = layer.MaskEnabled,
                        Style = layer.Style,
                        LabelColor = layer.LabelColor,
                        Mask = layer.Mask != null ? WriteMask(layer.Mask, "masks") : null
                    };
                    switch (layer)
                    {
                        case RasterLayer raster:
                            dto.Pixels = WritePng(raster.Surface.Bitmap, "layers");
                            dto.OffsetX = raster.Offset.X;
                            dto.OffsetY = raster.Offset.Y;
                            break;
                        case TextLayer text:
                            dto.Text = text.Text; dto.FontFamily = text.FontFamily; dto.Bold = text.Bold; dto.Italic = text.Italic;
                            dto.FauxBold = text.FauxBold; dto.FauxItalic = text.FauxItalic; dto.Underline = text.Underline; dto.Strikethrough = text.Strikethrough;
                            dto.AllCaps = text.AllCaps; dto.FontSize = text.FontSize; dto.Color = text.Color; dto.Alignment = text.Alignment; dto.Vertical = text.Vertical;
                            dto.Position = text.Position; dto.BoxSize = text.BoxSize; dto.Tracking = text.Tracking; dto.Leading = text.Leading; dto.AntiAlias = text.AntiAlias;
                            dto.Transform = text.Transform;
                            break;
                        case ShapeLayer shape:
                            dto.Path = PathDto.From(shape.Path); dto.FillColor = shape.FillColor; dto.StrokeColor = shape.StrokeColor;
                            dto.StrokeWidth = shape.StrokeWidth; dto.StrokeAlignment = shape.StrokeAlignment; dto.Dashed = shape.Dashed;
                            break;
                        case GroupLayer group:
                            dto.Children = group.Children.Select(ToDto).ToList();
                            dto.Expanded = group.Expanded;
                            dto.Artboard = group.ArtboardBounds;
                            dto.ArtboardBackground = group.ArtboardBackground;
                            break;
                        case AdjustmentLayer adjustment:
                            dto.Adjustment = adjustment.Settings;
                            break;
                        case FillLayer fill:
                            dto.FillKind = fill.FillKind; dto.Color = fill.Color; dto.Gradient = fill.Gradient; dto.PatternScale = fill.PatternScale;
                            if (fill.Pattern != null)
                            {
                                dto.PatternName = fill.Pattern.Name;
                                dto.Pattern = WritePng(fill.Pattern.Tile, "patterns");
                            }
                            break;
                        case SmartObjectLayer smart:
                            dto.Source = WritePng(smart.Source.Bitmap, "smart");
                            dto.Transform = smart.Transform;
                            dto.SmartFilters = smart.Filters.Where(f => f.Recipe != null).Select(f => new SmartFilterDto { Name = f.Name, Key = f.Key, Enabled = f.Enabled, Recipe = f.Recipe! }).ToList();
                            break;
                        case FrameLayer frame:
                            dto.Frame = frame.Frame; dto.Ellipse = frame.Ellipse; dto.ContentRect = frame.ContentRect;
                            dto.StrokeColor = frame.StrokeColor; dto.StrokeWidth = frame.StrokeWidth;
                            if (frame.Content != null) dto.Content = WritePng(frame.Content.Bitmap, "frames");
                            break;
                    }
                    return dto;
                }

                List<Layer> flat = doc.AllLayers.ToList();
                DocumentDto document = new()
                {
                    Version = FormatVersion,
                    Width = doc.Width,
                    Height = doc.Height,
                    Resolution = doc.Resolution,
                    ActiveLayerIndex = doc.ActiveLayer != null ? flat.IndexOf(doc.ActiveLayer) : -1,
                    Layers = doc.Root.Children.Select(ToDto).ToList(),
                    Selection = doc.Selection != null ? WriteMask(doc.Selection, "selection") : null,
                    Channels = doc.Channels.Select(c => new ChannelDto { Name = c.Name, File = WriteMask(c.Mask, "channels") }).ToList(),
                    Guides = doc.Guides,
                    Notes = doc.Notes,
                    Counts = doc.Counts,
                    CountGroups = doc.CountGroups,
                    Samplers = doc.Samplers,
                    Slices = doc.Slices,
                    Paths = doc.Paths.Select(p => new NamedPathDto { Name = p.Name, Path = PathDto.From(p.Path) }).ToList(),
                    WorkPath = doc.WorkPath != null ? PathDto.From(doc.WorkPath) : null
                };
                ZipArchiveEntry json = zip.CreateEntry("document.json", CompressionLevel.Optimal);
                using Stream jsonStream = json.Open();
                JsonSerializer.Serialize(jsonStream, document, Options);
            }
            File.Move(temp, path, true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }

    public static PhotoDocument Load(string path)
    {
        using FileStream file = File.OpenRead(path);
        using ZipArchive zip = new(file, ZipArchiveMode.Read);
        ZipArchiveEntry entry = zip.GetEntry("document.json") ?? throw new InvalidDataException("The project file is missing document.json.");
        DocumentDto dto;
        using (Stream stream = entry.Open())
        {
            dto = JsonSerializer.Deserialize<DocumentDto>(stream, Options) ?? throw new InvalidDataException("The project file is damaged.");
        }
        if (dto.Version > FormatVersion) throw new InvalidDataException("The project was created by a newer version of the Photo editor.");

        SKBitmap ReadPng(string name)
        {
            ZipArchiveEntry e = zip.GetEntry(name) ?? throw new InvalidDataException($"The project file is missing {name}.");
            using Stream s = e.Open();
            using MemoryStream copy = new();
            s.CopyTo(copy);
            copy.Position = 0;
            using SKBitmap decoded = SKBitmap.Decode(copy) ?? throw new InvalidDataException($"Could not decode {name}.");
            return ImageOps.Convert(decoded);
        }
        PixelSurface ReadMask(string name)
        {
            using SKBitmap gray = ReadPng(name);
            return ImageOps.ToMask(gray, false);
        }

        Layer FromDto(LayerDto d)
        {
            Layer layer;
            switch (d.Kind)
            {
                case LayerKind.Text:
                    layer = new TextLayer(d.Name)
                    {
                        Text = d.Text ?? string.Empty, FontFamily = d.FontFamily ?? "Segoe UI", Bold = d.Bold, Italic = d.Italic,
                        FauxBold = d.FauxBold, FauxItalic = d.FauxItalic, Underline = d.Underline, Strikethrough = d.Strikethrough, AllCaps = d.AllCaps,
                        FontSize = d.FontSize > 0 ? d.FontSize : 12, Color = d.Color ?? SKColors.Black, Alignment = d.Alignment, Vertical = d.Vertical,
                        Position = d.Position, BoxSize = d.BoxSize, Tracking = d.Tracking, Leading = d.Leading > 0 ? d.Leading : 1.2f, AntiAlias = d.AntiAlias,
                        Transform = d.Transform ?? SKMatrix.Identity
                    };
                    break;
                case LayerKind.Shape:
                    layer = new ShapeLayer(d.Name, d.Path?.ToPath() ?? new VectorPath())
                    {
                        FillColor = d.FillColor, StrokeColor = d.StrokeColor, StrokeWidth = d.StrokeWidth, StrokeAlignment = d.StrokeAlignment, Dashed = d.Dashed
                    };
                    break;
                case LayerKind.Group:
                {
                    GroupLayer group = new(d.Name) { Expanded = d.Expanded, ArtboardBounds = d.Artboard, ArtboardBackground = d.ArtboardBackground ?? SKColors.White };
                    foreach (LayerDto child in d.Children ?? []) group.Add(FromDto(child));
                    layer = group;
                    break;
                }
                case LayerKind.Adjustment:
                    layer = new AdjustmentLayer(d.Name, d.Adjustment ?? new AdjustmentSettings(AdjustmentKind.BrightnessContrast));
                    break;
                case LayerKind.Fill:
                    layer = new FillLayer(d.Name)
                    {
                        FillKind = d.FillKind, Color = d.Color ?? SKColors.Gray, Gradient = d.Gradient ?? GradientDefinition.CreateDefault(SKColors.Black, SKColors.White),
                        Pattern = d.Pattern != null ? new PatternDefinition(d.PatternName ?? "Pattern", ReadPng(d.Pattern)) : null,
                        PatternScale = d.PatternScale
                    };
                    break;
                case LayerKind.SmartObject:
                    layer = new SmartObjectLayer(d.Name, new PixelSurface(ReadPng(d.Source ?? throw new InvalidDataException("Smart object source is missing."))))
                    {
                        Transform = d.Transform ?? SKMatrix.Identity
                    };
                    foreach (SmartFilterDto filterDto in d.SmartFilters ?? [])
                    {
                        SmartFilter? restored = SmartFilterFactory.Restore(filterDto.Name, filterDto.Key, filterDto.Enabled, filterDto.Recipe);
                        if (restored != null) ((SmartObjectLayer)layer).Filters.Add(restored);
                    }
                    break;
                case LayerKind.Frame:
                    layer = new FrameLayer(d.Name, d.Frame, d.Ellipse)
                    {
                        ContentRect = d.ContentRect,
                        Content = d.Content != null ? new PixelSurface(ReadPng(d.Content)) : null,
                        StrokeColor = d.StrokeColor ?? SKColors.Transparent,
                        StrokeWidth = d.StrokeWidth
                    };
                    break;
                default:
                    layer = new RasterLayer(d.Name, new PixelSurface(ReadPng(d.Pixels ?? throw new InvalidDataException("Layer pixels are missing."))), new SKPointI(d.OffsetX, d.OffsetY));
                    break;
            }
            // Group blend mode defaults to pass through, so always restore the stored value.
            layer.Visible = d.Visible;
            layer.Opacity = d.Opacity;
            layer.FillOpacity = d.FillOpacity;
            layer.BlendMode = d.BlendMode;
            layer.IsClipped = d.Clipped;
            layer.LockTransparency = d.LockTransparency;
            layer.LockPixels = d.LockPixels;
            layer.LockPosition = d.LockPosition;
            layer.LockAll = d.LockAll;
            layer.LinkId = d.LinkId;
            layer.IsBackground = d.Background;
            layer.MaskEnabled = d.MaskEnabled;
            layer.Style = d.Style;
            layer.LabelColor = d.LabelColor;
            if (d.Mask != null) layer.Mask = ReadMask(d.Mask);
            return layer;
        }

        PhotoDocument doc = new(dto.Width, dto.Height, dto.Resolution, createBackground: false);
        foreach (LayerDto layer in dto.Layers) doc.Root.Add(FromDto(layer));
        List<Layer> flat = doc.AllLayers.ToList();
        doc.ActiveLayer = dto.ActiveLayerIndex >= 0 && dto.ActiveLayerIndex < flat.Count ? flat[dto.ActiveLayerIndex] : flat.LastOrDefault();
        if (dto.Selection != null) doc.SetSelectionLive(ReadMask(dto.Selection));
        doc.Channels.AddRange(dto.Channels.Select(c => new AlphaChannel(c.Name, ReadMask(c.File))));
        doc.Guides.AddRange(dto.Guides);
        doc.Notes.AddRange(dto.Notes);
        doc.Counts.AddRange(dto.Counts);
        if (dto.CountGroups.Count > 0)
        {
            doc.CountGroups.Clear();
            doc.CountGroups.AddRange(dto.CountGroups);
        }
        doc.Samplers.AddRange(dto.Samplers);
        doc.Slices.AddRange(dto.Slices);
        doc.Paths.AddRange(dto.Paths.Select(p => new NamedPath(p.Name, p.Path.ToPath())));
        doc.WorkPath = dto.WorkPath?.ToPath();
        doc.Title = Path.GetFileName(path);
        return doc;
    }

    private sealed class ColorConverter : JsonConverter<SKColor>
    {
        public override SKColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            ColorMath.TryParseHex(reader.GetString(), out SKColor c) ? c : SKColors.Black;

        public override void Write(Utf8JsonWriter writer, SKColor value, JsonSerializerOptions options) =>
            writer.WriteStringValue("#" + ColorMath.ToHex(value, true));
    }

    private abstract class FloatArrayConverter<T> : JsonConverter<T>
    {
        protected abstract T Create(float[] values);
        protected abstract float[] Values(T value);

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            Create(JsonSerializer.Deserialize<float[]>(ref reader) ?? []);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, Values(value));
    }

    private sealed class PointConverter : FloatArrayConverter<SKPoint>
    {
        protected override SKPoint Create(float[] v) => v.Length >= 2 ? new SKPoint(v[0], v[1]) : default;
        protected override float[] Values(SKPoint p) => [p.X, p.Y];
    }

    private sealed class SizeConverter : FloatArrayConverter<SKSize>
    {
        protected override SKSize Create(float[] v) => v.Length >= 2 ? new SKSize(v[0], v[1]) : default;
        protected override float[] Values(SKSize s) => [s.Width, s.Height];
    }

    private sealed class RectConverter : FloatArrayConverter<SKRect>
    {
        protected override SKRect Create(float[] v) => v.Length >= 4 ? new SKRect(v[0], v[1], v[2], v[3]) : default;
        protected override float[] Values(SKRect r) => [r.Left, r.Top, r.Right, r.Bottom];
    }

    private sealed class RectIConverter : FloatArrayConverter<SKRectI>
    {
        protected override SKRectI Create(float[] v) => v.Length >= 4 ? new SKRectI((int)v[0], (int)v[1], (int)v[2], (int)v[3]) : default;
        protected override float[] Values(SKRectI r) => [r.Left, r.Top, r.Right, r.Bottom];
    }

    private sealed class MatrixConverter : FloatArrayConverter<SKMatrix>
    {
        protected override SKMatrix Create(float[] v) => v.Length >= 9 ? new SKMatrix(v) : SKMatrix.Identity;
        protected override float[] Values(SKMatrix m) => m.Values;
    }
}
