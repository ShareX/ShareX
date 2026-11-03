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
