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

namespace ShareX.PhotoEditor.Core;

public enum LayerKind
{
    Raster,
    Text,
    Shape,
    Group,
    Adjustment,
    Fill,
    SmartObject,
    Frame
}

public sealed class LayerStyle
{
    public bool DropShadow { get; set; }
    public SKColor ShadowColor { get; set; } = SKColors.Black;
    public float ShadowOpacity { get; set; } = 0.75f;
    public float ShadowAngle { get; set; } = 120;
    public float ShadowDistance { get; set; } = 5;
    public float ShadowSize { get; set; } = 5;

    public bool Stroke { get; set; }
    public SKColor StrokeColor { get; set; } = SKColors.Black;
    public float StrokeSize { get; set; } = 3;
    public float StrokeOpacity { get; set; } = 1;
    public StrokePosition StrokePosition { get; set; } = StrokePosition.Outside;

    public bool OuterGlow { get; set; }
    public SKColor GlowColor { get; set; } = new(255, 255, 190);
    public float GlowOpacity { get; set; } = 0.75f;
    public float GlowSize { get; set; } = 10;

    public bool ColorOverlay { get; set; }
    public SKColor OverlayColor { get; set; } = SKColors.Red;
    public float OverlayOpacity { get; set; } = 1;

    public bool InnerShadow { get; set; }
    public SKColor InnerShadowColor { get; set; } = SKColors.Black;
    public float InnerShadowOpacity { get; set; } = 0.75f;
    public float InnerShadowDistance { get; set; } = 5;
    public float InnerShadowSize { get; set; } = 5;

    public bool HasAny => DropShadow || Stroke || OuterGlow || ColorOverlay || InnerShadow;

    public float Extent => Math.Max(DropShadow ? ShadowDistance + ShadowSize * 3 : 0,
        Math.Max(Stroke ? StrokeSize + 2 : 0, OuterGlow ? GlowSize * 3 : 0));

    public LayerStyle Clone() => (LayerStyle)MemberwiseClone();
}

public enum StrokePosition
{
    Outside,
    Inside,
    Center
}

/// <summary>Base class for every layer type in a document.</summary>
public abstract class Layer
{
    private static int _nextId;

    protected Layer(string name)
    {
        Id = Interlocked.Increment(ref _nextId);
        Name = name;
    }

    /// <summary>Stable identity, shared by history copies of the same layer.</summary>
    public int Id { get; private set; }

    public string Name { get; set; }

    public abstract LayerKind Kind { get; }

    public bool Visible { get; set; } = true;

    public float Opacity { get; set; } = 1;

    /// <summary>Fill opacity scales layer content but not layer style effects.</summary>
    public float FillOpacity { get; set; } = 1;

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    /// <summary>The layer is clipped to the nearest unclipped layer below it.</summary>
    public bool IsClipped { get; set; }

    public bool LockTransparency { get; set; }

    public bool LockPixels { get; set; }

    public bool LockPosition { get; set; }

    public bool LockAll { get; set; }

    public bool IsBackground { get; set; }

    /// <summary>Layer mask in document coordinates. 255 reveals, 0 hides.</summary>
    public PixelSurface? Mask { get; set; }

    public bool MaskEnabled { get; set; } = true;

    public LayerStyle? Style { get; set; }

    public GroupLayer? Parent { get; internal set; }

    public SKColor? LabelColor { get; set; }

    public bool CanEditPixels => !LockAll && !LockPixels;

    public bool CanMove => !LockAll && !LockPosition;

    public virtual IEnumerable<PixelSurface> Surfaces => Mask != null ? [Mask] : [];

    /// <summary>Bounds of the visible content in document coordinates.</summary>
    public abstract SKRect GetContentBounds();

    /// <summary>Draws the layer content (without opacity, mask or style) in document coordinates.</summary>
    public abstract void RenderContent(SKCanvas canvas, RenderContext context);

    protected abstract Layer CreateShallowCopy();

    /// <summary>Copies layer properties while sharing pixel surfaces. Used by history snapshots.</summary>
    public Layer CloneShared()
    {
        Layer copy = CreateShallowCopy();
        CopyBaseTo(copy);
        copy.Id = Id;
        return copy;
    }

    /// <summary>Creates an independent duplicate with a new identity.</summary>
    public Layer Duplicate()
    {
        Layer copy = CloneShared();
        copy.Id = Interlocked.Increment(ref _nextId);
        copy.DetachSurfaces();
        return copy;
    }

    protected virtual void DetachSurfaces()
    {
        if (Mask != null) Mask = Mask.Clone();
    }

    private void CopyBaseTo(Layer copy)
    {
        copy.Name = Name;
        copy.Visible = Visible;
        copy.Opacity = Opacity;
        copy.FillOpacity = FillOpacity;
        copy.BlendMode = BlendMode;
        copy.IsClipped = IsClipped;
        copy.LockTransparency = LockTransparency;
        copy.LockPixels = LockPixels;
        copy.LockPosition = LockPosition;
        copy.LockAll = LockAll;
        copy.IsBackground = IsBackground;
        copy.Mask = Mask;
        copy.MaskEnabled = MaskEnabled;
        copy.Style = Style?.Clone();
        copy.LabelColor = LabelColor;
    }

    public void FreezeSurfaces()
    {
        foreach (PixelSurface surface in Surfaces) surface.Freeze();
    }

    /// <summary>Returns a writable mask, copying it first if it is shared with history.</summary>
    public PixelSurface GetWritableMask(int width, int height)
    {
        if (Mask == null) Mask = PixelSurface.CreateMask(width, height, 255);
        else if (Mask.Frozen) Mask = Mask.Clone();
        return Mask;
    }

    /// <summary>Offsets the layer content by a whole-pixel or sub-pixel amount.</summary>
    public virtual void Translate(float dx, float dy)
    {
    }

    public bool IsEffectivelyVisible
    {
        get
        {
            for (Layer? layer = this; layer != null; layer = layer.Parent)
            {
                if (!layer.Visible) return false;
            }
            return true;
        }
    }

    public override string ToString() => Name;
}

public sealed class RenderContext
{
    public RenderContext(int documentWidth, int documentHeight)
    {
        DocumentWidth = documentWidth;
        DocumentHeight = documentHeight;
    }

    public int DocumentWidth { get; }

    public int DocumentHeight { get; }

    public SKRectI Area { get; set; }

    public SKSamplingOptions Sampling { get; set; } = new(SKCubicResampler.Mitchell);
}

public sealed class RasterLayer : Layer
{
    public RasterLayer(string name, PixelSurface surface, SKPointI offset = default) : base(name)
    {
        Surface = surface;
        Offset = offset;
    }

    public override LayerKind Kind => LayerKind.Raster;

    public PixelSurface Surface { get; set; }

    /// <summary>Position of the surface's top-left pixel in document coordinates.</summary>
    public SKPointI Offset { get; set; }

    public SKRectI SurfaceBounds => SKRectI.Create(Offset.X, Offset.Y, Surface.Width, Surface.Height);

    public override IEnumerable<PixelSurface> Surfaces => Mask != null ? [Surface, Mask] : [Surface];

    public override SKRect GetContentBounds() => SurfaceBounds;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        canvas.DrawBitmap(Surface.Bitmap, Offset.X, Offset.Y);
    }

    protected override Layer CreateShallowCopy() => new RasterLayer(Name, Surface, Offset);

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        Surface = Surface.Clone();
    }

    /// <summary>Returns the surface for writing, copying it first when it is shared with history.</summary>
    public PixelSurface GetWritableSurface()
    {
        if (Surface.Frozen) Surface = Surface.Clone();
        return Surface;
    }

    /// <summary>Grows the surface so that the given document rectangle is backed by pixels.</summary>
    public void EnsureCovers(SKRectI documentRect)
    {
        SKRectI current = SurfaceBounds;
        SKRectI union = SKRectI.Union(current, documentRect);
        if (union == current)
        {
            GetWritableSurface();
            return;
        }
        PixelSurface grown = PixelSurface.CreateColor(union.Width, union.Height);
        using (SKCanvas canvas = new(grown.Bitmap))
        {
            canvas.DrawBitmap(Surface.Bitmap, current.Left - union.Left, current.Top - union.Top);
        }
        Surface = grown;
        Offset = new SKPointI(union.Left, union.Top);
    }

    public override void Translate(float dx, float dy)
    {
        Offset = new SKPointI(Offset.X + (int)MathF.Round(dx), Offset.Y + (int)MathF.Round(dy));
    }

    /// <summary>Computes the bounds of non-transparent pixels in document coordinates.</summary>
    public SKRectI GetOpaqueBounds()
    {
        SKRectI local = ImageOps.GetOpaqueBounds(Surface.Bitmap);
        if (local.IsEmpty) return SKRectI.Empty;
        local.Offset(Offset.X, Offset.Y);
        return local;
    }
}

public enum TextAlignment
{
    Left,
    Center,
    Right,
    Justify
}

public enum TextAntiAlias
{
    None,
    Sharp,
    Crisp,
    Strong,
    Smooth
}

public sealed class TextLayer : Layer
{
    private SKBitmap? _cache;
    private SKRectI _cacheBounds;
    private string? _cacheKey;

    public TextLayer(string name) : base(name)
    {
    }

    public override LayerKind Kind => LayerKind.Text;

    public string Text { get; set; } = string.Empty;

    public string FontFamily { get; set; } = "Segoe UI";

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool FauxBold { get; set; }

    public bool FauxItalic { get; set; }

    public bool Underline { get; set; }

    public bool Strikethrough { get; set; }

    public bool AllCaps { get; set; }

    public float FontSize { get; set; } = 48;

    public SKColor Color { get; set; } = SKColors.Black;

    public TextAlignment Alignment { get; set; } = TextAlignment.Left;

    public bool Vertical { get; set; }

    /// <summary>Top-left of the text block (point text) or of the paragraph box.</summary>
    public SKPoint Position { get; set; }

    /// <summary>Paragraph box size. Zero width means point text.</summary>
    public SKSize BoxSize { get; set; }

    /// <summary>Character tracking in thousandths of an em.</summary>
    public float Tracking { get; set; }

    /// <summary>Line spacing as a multiple of the font size.</summary>
    public float Leading { get; set; } = 1.2f;

    public TextAntiAlias AntiAlias { get; set; } = TextAntiAlias.Smooth;

    /// <summary>Non-destructive transform applied around the text block.</summary>
    public SKMatrix Transform { get; set; } = SKMatrix.Identity;

    public bool IsParagraph => BoxSize.Width > 0 && BoxSize.Height > 0;

    protected override Layer CreateShallowCopy()
    {
        TextLayer copy = new(Name);
        CopyTextPropertiesTo(copy);
        return copy;
    }

    public void CopyTextPropertiesTo(TextLayer copy)
    {
        copy.Text = Text;
        copy.FontFamily = FontFamily;
        copy.Bold = Bold;
        copy.Italic = Italic;
        copy.FauxBold = FauxBold;
        copy.FauxItalic = FauxItalic;
        copy.Underline = Underline;
        copy.Strikethrough = Strikethrough;
        copy.AllCaps = AllCaps;
        copy.FontSize = FontSize;
        copy.Color = Color;
        copy.Alignment = Alignment;
        copy.Vertical = Vertical;
        copy.Position = Position;
        copy.BoxSize = BoxSize;
        copy.Tracking = Tracking;
        copy.Leading = Leading;
        copy.AntiAlias = AntiAlias;
        copy.Transform = Transform;
    }

    public override void Translate(float dx, float dy)
    {
        Position = new SKPoint(Position.X + dx, Position.Y + dy);
        Invalidate();
    }

    public void Invalidate()
    {
        _cacheKey = null;
    }

    public SKFont CreateFont()
    {
        SKTypeface typeface = SKTypeface.FromFamilyName(FontFamily,
            Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal,
            Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright) ?? SKTypeface.Default;
        return new SKFont(typeface, FontSize)
        {
            Embolden = FauxBold,
            SkewX = FauxItalic ? -0.25f : 0,
            Edging = AntiAlias == TextAntiAlias.None ? SKFontEdging.Alias : SKFontEdging.Antialias,
            Subpixel = AntiAlias != TextAntiAlias.None,
            Hinting = AntiAlias == TextAntiAlias.Sharp || AntiAlias == TextAntiAlias.Crisp ? SKFontHinting.Full : SKFontHinting.Slight
        };
    }

    private string DisplayText => AllCaps ? Text.ToUpperInvariant() : Text;

    /// <summary>Lays out glyph runs as (text, origin) pairs in untransformed document coordinates.</summary>
    public List<(string Text, SKPoint Origin, float Width)> Layout(SKFont font)
    {
        List<(string, SKPoint, float)> runs = [];
        SKFontMetrics metrics = font.Metrics;
        float lineHeight = FontSize * Leading;
        float ascent = -metrics.Ascent;
        string[] paragraphs = DisplayText.Replace("\r\n", "\n").Split('\n');

        if (Vertical)
        {
            float columnX = Position.X + (IsParagraph ? BoxSize.Width - lineHeight : 0);
            foreach (string paragraph in paragraphs)
            {
                float y = Position.Y + ascent;
                foreach (char c in paragraph)
                {
                    string s = c.ToString();
                    float w = font.MeasureText(s);
                    if (IsParagraph && y - ascent + lineHeight > Position.Y + BoxSize.Height && y > Position.Y + ascent)
                    {
                        columnX -= lineHeight;
                        y = Position.Y + ascent;
                    }
                    runs.Add((s, new SKPoint(columnX + (lineHeight - w) / 2, y), w));
                    y += FontSize * (1 + Tracking / 1000f);
                }
                columnX -= lineHeight;
            }
            return runs;
        }

        List<string> lines = [];
        foreach (string paragraph in paragraphs)
        {
            if (!IsParagraph)
            {
                lines.Add(paragraph);
                continue;
            }
            string current = string.Empty;
            foreach (string word in paragraph.Split(' '))
            {
                string candidate = current.Length == 0 ? word : current + " " + word;
                if (current.Length > 0 && MeasureLine(font, candidate) > BoxSize.Width)
                {
                    lines.Add(current);
                    current = word;
                }
                else
                {
                    current = candidate;
                }
            }
            lines.Add(current);
        }

        float blockWidth = IsParagraph ? BoxSize.Width : 0;
        float baseline = Position.Y + ascent;
        foreach (string line in lines)
        {
            float width = MeasureLine(font, line);
            float x = Alignment switch
            {
                TextAlignment.Center => IsParagraph ? Position.X + (blockWidth - width) / 2 : Position.X - width / 2,
                TextAlignment.Right => IsParagraph ? Position.X + blockWidth - width : Position.X - width,
                _ => Position.X
            };
            runs.Add((line, new SKPoint(x, baseline), width));
            baseline += lineHeight;
        }
        return runs;
    }

    public float MeasureLine(SKFont font, string line)
    {
        if (line.Length == 0) return 0;
        float width = font.MeasureText(line);
        if (Tracking != 0) width += FontSize * Tracking / 1000f * (line.Length - 1);
        return width;
    }

    private void DrawRun(SKCanvas canvas, SKFont font, string text, SKPoint origin, SKPaint paint)
    {
        if (Tracking == 0)
        {
            canvas.DrawText(text, origin.X, origin.Y, font, paint);
            return;
        }
        float x = origin.X;
        float extra = FontSize * Tracking / 1000f;
        foreach (char c in text)
        {
            string s = c.ToString();
            canvas.DrawText(s, x, origin.Y, font, paint);
            x += font.MeasureText(s) + extra;
        }
    }

    /// <summary>Returns the outline of the text in document coordinates, including the layer transform.</summary>
    public SKPath GetTextPath()
    {
        using SKFont font = CreateFont();
        SKPath result = new();
        foreach (var (text, origin, _) in Layout(font))
        {
            if (string.IsNullOrEmpty(text)) continue;
            if (Tracking == 0)
            {
                using SKPath path = font.GetTextPath(text, origin);
                result.AddPath(path);
            }
            else
            {
                float x = origin.X;
                float extra = FontSize * Tracking / 1000f;
                foreach (char c in text)
                {
                    string s = c.ToString();
                    using SKPath path = font.GetTextPath(s, new SKPoint(x, origin.Y));
                    result.AddPath(path);
                    x += font.MeasureText(s) + extra;
                }
            }
        }
        if (!Transform.IsIdentity) result.Transform(GetTransformAroundBlock());
        return result;
    }

    /// <summary>Untransformed bounds of the text block.</summary>
    public SKRect GetBlockBounds()
    {
        if (IsParagraph) return SKRect.Create(Position, BoxSize);
        using SKFont font = CreateFont();
        var runs = Layout(font);
        SKFontMetrics metrics = font.Metrics;
        if (runs.Count == 0) return SKRect.Create(Position.X, Position.Y, 1, FontSize);
        float left = runs.Min(r => r.Origin.X);
        float right = runs.Max(r => r.Origin.X + Math.Max(r.Width, 1));
        float top = runs.Min(r => r.Origin.Y) + metrics.Ascent;
        float bottom = runs.Max(r => r.Origin.Y) + metrics.Descent;
        return new SKRect(left, top, right, bottom);
    }

    public SKMatrix GetTransformAroundBlock()
    {
        if (Transform.IsIdentity) return SKMatrix.Identity;
        SKRect block = GetBlockBoundsRaw();
        SKPoint center = new(block.MidX, block.MidY);
        return SKMatrix.CreateTranslation(-center.X, -center.Y).PostConcat(Transform).PostConcat(SKMatrix.CreateTranslation(center.X, center.Y));
    }

    private SKRect GetBlockBoundsRaw()
    {
        SKMatrix saved = Transform;
        Transform = SKMatrix.Identity;
        SKRect bounds = GetBlockBounds();
        Transform = saved;
        return bounds;
    }

    public override SKRect GetContentBounds()
    {
        SKRect bounds = GetBlockBounds();
        bounds.Inflate(FontSize * 0.25f, FontSize * 0.25f);
        return Transform.IsIdentity ? bounds : GetTransformAroundBlock().MapRect(bounds);
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        string key = $"{Text}|{FontFamily}|{Bold}|{Italic}|{FauxBold}|{FauxItalic}|{Underline}|{Strikethrough}|{AllCaps}|{FontSize}|{Color}|{Alignment}|{Vertical}|{Position}|{BoxSize}|{Tracking}|{Leading}|{AntiAlias}|{Transform.Values.Sum()}|{string.Join(",", Transform.Values)}";
        if (_cache == null || _cacheKey != key)
        {
            _cache?.Dispose();
            SKRectI bounds = SKRectI.Ceiling(GetContentBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) bounds = SKRectI.Create((int)Position.X, (int)Position.Y, 1, 1);
            _cacheBounds = bounds;
            _cache = new SKBitmap(new SKImageInfo(bounds.Width, bounds.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
            using (SKCanvas cacheCanvas = new(_cache))
            {
                cacheCanvas.Clear(SKColors.Transparent);
                cacheCanvas.Translate(-bounds.Left, -bounds.Top);
                cacheCanvas.Concat(GetTransformAroundBlock());
                DrawText(cacheCanvas);
            }
            _cacheKey = key;
        }
        canvas.DrawBitmap(_cache, _cacheBounds.Left, _cacheBounds.Top);
    }

    private void DrawText(SKCanvas canvas)
    {
        using SKFont font = CreateFont();
        using SKPaint paint = new() { Color = Color, IsAntialias = AntiAlias != TextAntiAlias.None };
        foreach (var (text, origin, width) in Layout(font))
        {
            DrawRun(canvas, font, text, origin, paint);
            if (!Vertical && (Underline || Strikethrough) && width > 0)
            {
                float thickness = Math.Max(1, FontSize / 15f);
                using SKPaint line = new() { Color = Color, StrokeWidth = thickness, IsAntialias = true, Style = SKPaintStyle.Stroke };
                if (Underline) canvas.DrawLine(origin.X, origin.Y + thickness * 2, origin.X + width, origin.Y + thickness * 2, line);
                if (Strikethrough) canvas.DrawLine(origin.X, origin.Y - FontSize * 0.3f, origin.X + width, origin.Y - FontSize * 0.3f, line);
            }
        }
    }
}

public sealed class ShapeLayer : Layer
{
    public ShapeLayer(string name, VectorPath path) : base(name)
    {
        Path = path;
    }

    public override LayerKind Kind => LayerKind.Shape;

    public VectorPath Path { get; set; }

    public SKColor? FillColor { get; set; } = SKColors.Gray;

    public SKColor? StrokeColor { get; set; }

    public float StrokeWidth { get; set; } = 3;

    public StrokePosition StrokeAlignment { get; set; } = StrokePosition.Center;

    public bool Dashed { get; set; }

    public SKStrokeCap StrokeCap { get; set; } = SKStrokeCap.Butt;

    public SKStrokeJoin StrokeJoin { get; set; } = SKStrokeJoin.Miter;

    protected override Layer CreateShallowCopy() => new ShapeLayer(Name, Path.Clone())
    {
        FillColor = FillColor,
        StrokeColor = StrokeColor,
        StrokeWidth = StrokeWidth,
        StrokeAlignment = StrokeAlignment,
        Dashed = Dashed,
        StrokeCap = StrokeCap,
        StrokeJoin = StrokeJoin
    };

    public override void Translate(float dx, float dy) => Path.Offset(new SKPoint(dx, dy));

    public override SKRect GetContentBounds()
    {
        SKRect bounds = Path.GetBounds();
        float inflate = StrokeColor != null ? StrokeWidth + 2 : 1;
        bounds.Inflate(inflate, inflate);
        return bounds;
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        using SKPath path = Path.ToSKPath();
        if (FillColor is SKColor fill)
        {
            using SKPaint paint = new() { Color = fill, IsAntialias = true, Style = SKPaintStyle.Fill };
            canvas.DrawPath(path, paint);
        }
        if (StrokeColor is SKColor stroke && StrokeWidth > 0)
        {
            float width = StrokeAlignment == StrokePosition.Center ? StrokeWidth : StrokeWidth * 2;
            using SKPaint paint = new()
            {
                Color = stroke,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = width,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                PathEffect = Dashed ? SKPathEffect.CreateDash([width * 2, width * 2], 0) : null
            };
            canvas.Save();
            if (StrokeAlignment == StrokePosition.Inside) canvas.ClipPath(path, SKClipOperation.Intersect, true);
            else if (StrokeAlignment == StrokePosition.Outside) canvas.ClipPath(path, SKClipOperation.Difference, true);
            canvas.DrawPath(path, paint);
            canvas.Restore();
        }
    }
}

public sealed class GroupLayer : Layer
{
    public GroupLayer(string name) : base(name)
    {
        BlendMode = LayerBlendMode.PassThrough;
    }

    public override LayerKind Kind => LayerKind.Group;

    public List<Layer> Children { get; } = [];

    public bool Expanded { get; set; } = true;

    /// <summary>When set, the group is an artboard that clips its content and has its own background.</summary>
    public SKRectI? ArtboardBounds { get; set; }

    public SKColor ArtboardBackground { get; set; } = SKColors.White;

    public bool IsArtboard => ArtboardBounds != null;

    public override IEnumerable<PixelSurface> Surfaces => base.Surfaces.Concat(Children.SelectMany(c => c.Surfaces));

    public void Add(Layer layer, int? index = null)
    {
        layer.Parent?.Children.Remove(layer);
        layer.Parent = this;
        if (index is int i) Children.Insert(Math.Clamp(i, 0, Children.Count), layer);
        else Children.Add(layer);
    }

    public void Remove(Layer layer)
    {
        if (Children.Remove(layer)) layer.Parent = null;
    }

    protected override Layer CreateShallowCopy()
    {
        GroupLayer copy = new(Name)
        {
            Expanded = Expanded,
            ArtboardBounds = ArtboardBounds,
            ArtboardBackground = ArtboardBackground
        };
        foreach (Layer child in Children) copy.Add(child.CloneShared());
        return copy;
    }

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        List<Layer> children = Children.ToList();
        Children.Clear();
        foreach (Layer child in children) Add(child.Duplicate());
    }

    public override void Translate(float dx, float dy)
    {
        foreach (Layer child in Children) child.Translate(dx, dy);
        if (ArtboardBounds is SKRectI bounds)
        {
            bounds.Offset((int)MathF.Round(dx), (int)MathF.Round(dy));
            ArtboardBounds = bounds;
        }
    }

    public override SKRect GetContentBounds()
    {
        if (ArtboardBounds is SKRectI artboard) return artboard;
        SKRect bounds = SKRect.Empty;
        foreach (Layer child in Children.Where(c => c.Visible && c is not AdjustmentLayer))
        {
            SKRect childBounds = child.GetContentBounds();
            bounds = bounds.IsEmpty ? childBounds : SKRect.Union(bounds, childBounds);
        }
        return bounds;
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        // Groups are rendered by the compositor so that blend modes and adjustments apply per child.
    }

    public IEnumerable<Layer> Descendants()
    {
        foreach (Layer child in Children)
        {
            yield return child;
            if (child is GroupLayer group)
            {
                foreach (Layer descendant in group.Descendants()) yield return descendant;
            }
        }
    }
}

public sealed class AdjustmentLayer : Layer
{
    public AdjustmentLayer(string name, AdjustmentSettings settings) : base(name)
    {
        Settings = settings;
    }

    public override LayerKind Kind => LayerKind.Adjustment;

    public AdjustmentSettings Settings { get; set; }

    protected override Layer CreateShallowCopy() => new AdjustmentLayer(Name, Settings.Clone());

    public override SKRect GetContentBounds() => SKRect.Empty;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
    }
}

public enum FillKind
{
    SolidColor,
    Gradient,
    Pattern
}

public sealed class FillLayer : Layer
{
    public FillLayer(string name) : base(name)
    {
    }

    public override LayerKind Kind => LayerKind.Fill;

    public FillKind FillKind { get; set; }

    public SKColor Color { get; set; } = SKColors.Gray;

    public GradientDefinition Gradient { get; set; } = GradientDefinition.CreateDefault(SKColors.Black, SKColors.White);

    public PatternDefinition? Pattern { get; set; }

    public float PatternScale { get; set; } = 1;

    protected override Layer CreateShallowCopy() => new FillLayer(Name)
    {
        FillKind = FillKind,
        Color = Color,
        Gradient = Gradient.Clone(),
        Pattern = Pattern,
        PatternScale = PatternScale
    };

    public override SKRect GetContentBounds() => SKRect.Empty;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        SKRect area = SKRect.Create(0, 0, context.DocumentWidth, context.DocumentHeight);
        using SKPaint paint = new() { IsAntialias = false };
        switch (FillKind)
        {
            case FillKind.SolidColor:
                paint.Color = Color;
                break;
            case FillKind.Gradient:
                paint.Shader = Gradient.CreateShader(area);
                break;
            case FillKind.Pattern when Pattern != null:
                paint.Shader = SKShader.CreateBitmap(Pattern.Tile, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat,
                    SKMatrix.CreateScale(PatternScale, PatternScale));
                break;
        }
        canvas.DrawRect(area, paint);
    }
}

/// <summary>Embedded source pixels displayed through a non-destructive transform.</summary>
public sealed class SmartObjectLayer : Layer
{
    public SmartObjectLayer(string name, PixelSurface source) : base(name)
    {
        Source = source;
    }

    public override LayerKind Kind => LayerKind.SmartObject;

    public PixelSurface Source { get; set; }

    public SKMatrix Transform { get; set; } = SKMatrix.Identity;

    /// <summary>Non-destructive filters re-applied to the source whenever the layer is rendered.</summary>
    public List<SmartFilter> Filters { get; } = [];

    private SKBitmap? _filtered;
    private string? _filterKey;

    public override IEnumerable<PixelSurface> Surfaces => Mask != null ? [Source, Mask] : [Source];

    protected override Layer CreateShallowCopy()
    {
        SmartObjectLayer copy = new(Name, Source) { Transform = Transform };
        copy.Filters.AddRange(Filters.Select(f => f.Clone()));
        return copy;
    }

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        Source = Source.Clone();
    }

    public override void Translate(float dx, float dy) => Transform = Transform.PostConcat(SKMatrix.CreateTranslation(dx, dy));

    public SKPoint[] GetCorners()
    {
        SKPoint[] corners = [new(0, 0), new(Source.Width, 0), new(Source.Width, Source.Height), new(0, Source.Height)];
        return Transform.MapPoints(corners);
    }

    public override SKRect GetContentBounds() => Transform.MapRect(SKRect.Create(0, 0, Source.Width, Source.Height));

    public SKBitmap GetFilteredSource()
    {
        if (Filters.Count == 0) return Source.Bitmap;
        string key = string.Join("|", Filters.Select(f => f.Key)) + "|" + Source.GetHashCode();
        if (_filtered == null || _filterKey != key)
        {
            _filtered?.Dispose();
            SKBitmap current = Source.Bitmap.Copy();
            foreach (SmartFilter filter in Filters.Where(f => f.Enabled))
            {
                SKBitmap next = filter.Apply(current);
                if (!ReferenceEquals(next, current)) current.Dispose();
                current = next;
            }
            _filtered = current;
            _filterKey = key;
        }
        return _filtered;
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        canvas.Save();
        canvas.Concat(Transform);
        using SKImage image = SKImage.FromBitmap(GetFilteredSource());
        using SKPaint paint = new() { IsAntialias = true };
        canvas.DrawImage(image, 0, 0, context.Sampling, paint);
        canvas.Restore();
    }
}

/// <summary>A filter stored on a smart object and applied non-destructively.</summary>
public sealed class SmartFilter
{
    public SmartFilter(string name, string key, Func<SKBitmap, SKBitmap> apply)
    {
        Name = name;
        Key = key;
        Apply = apply;
    }

    public string Name { get; }

    public string Key { get; }

    public bool Enabled { get; set; } = true;

    public Func<SKBitmap, SKBitmap> Apply { get; }

    public SmartFilter Clone() => new(Name, Key, Apply) { Enabled = Enabled };
}

/// <summary>A placeholder frame that crops placed content to a rectangle or ellipse.</summary>
public sealed class FrameLayer : Layer
{
    public FrameLayer(string name, SKRect frame, bool ellipse) : base(name)
    {
        Frame = frame;
        Ellipse = ellipse;
    }

    public override LayerKind Kind => LayerKind.Frame;

    public SKRect Frame { get; set; }

    public bool Ellipse { get; set; }

    public PixelSurface? Content { get; set; }

    /// <summary>Content placement relative to the frame, in document coordinates.</summary>
    public SKRect ContentRect { get; set; }

    public SKColor StrokeColor { get; set; } = SKColors.Transparent;

    public float StrokeWidth { get; set; }

    public override IEnumerable<PixelSurface> Surfaces => Content != null ? base.Surfaces.Append(Content) : base.Surfaces;

    protected override Layer CreateShallowCopy() => new FrameLayer(Name, Frame, Ellipse)
    {
        Content = Content,
        ContentRect = ContentRect,
        StrokeColor = StrokeColor,
        StrokeWidth = StrokeWidth
    };

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        Content = Content?.Clone();
    }

    public override void Translate(float dx, float dy)
    {
        SKRect frame = Frame;
        frame.Offset(dx, dy);
        Frame = frame;
        SKRect content = ContentRect;
        content.Offset(dx, dy);
        ContentRect = content;
    }

    public void PlaceContent(PixelSurface content)
    {
        Content = content;
        float scale = Math.Max(Frame.Width / content.Width, Frame.Height / content.Height);
        float w = content.Width * scale, h = content.Height * scale;
        ContentRect = SKRect.Create(Frame.MidX - w / 2, Frame.MidY - h / 2, w, h);
    }

    public SKPath GetFramePath()
    {
        SKPath path = new();
        if (Ellipse) path.AddOval(Frame);
        else path.AddRect(Frame);
        return path;
    }

    public override SKRect GetContentBounds() => Frame;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        using SKPath path = GetFramePath();
        canvas.Save();
        canvas.ClipPath(path, SKClipOperation.Intersect, true);
        if (Content != null)
        {
            using SKImage image = SKImage.FromBitmap(Content.Bitmap);
            canvas.DrawImage(image, SKRect.Create(0, 0, Content.Width, Content.Height), ContentRect, context.Sampling);
        }
        canvas.Restore();
        if (StrokeWidth > 0 && StrokeColor.Alpha > 0)
        {
            using SKPaint stroke = new() { Color = StrokeColor, StrokeWidth = StrokeWidth, Style = SKPaintStyle.Stroke, IsAntialias = true };
            canvas.DrawPath(path, stroke);
        }
    }
}
