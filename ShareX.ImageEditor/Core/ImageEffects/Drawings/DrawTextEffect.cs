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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawTextEffect : ImageEffectBase
{
    public override string Id => "draw_text";
    public override string Name => "Text";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.type;
    public override string Description => "Draws text on the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Text<DrawTextEffect>("text", "Text", "Text", (e, v) => e.Text = v),
        EffectParameters.Enum<DrawTextEffect, DrawingPlacement>(
            "placement", "Placement", DrawingPlacement.TopLeft, (e, v) => e.Placement = v,
            new (string Label, DrawingPlacement Value)[]
            {
                ("Top left", DrawingPlacement.TopLeft),
                ("Top center", DrawingPlacement.TopCenter),
                ("Top right", DrawingPlacement.TopRight),
                ("Middle left", DrawingPlacement.MiddleLeft),
                ("Middle center", DrawingPlacement.MiddleCenter),
                ("Middle right", DrawingPlacement.MiddleRight),
                ("Bottom left", DrawingPlacement.BottomLeft),
                ("Bottom center", DrawingPlacement.BottomCenter),
                ("Bottom right", DrawingPlacement.BottomRight)
            }),
        EffectParameters.IntNumeric<DrawTextEffect>("offset_x", "Offset X", -10000, 10000, 0, (e, v) => e.Offset = new SKPointI(v, e.Offset.Y)),
        EffectParameters.IntNumeric<DrawTextEffect>("offset_y", "Offset Y", -10000, 10000, 0, (e, v) => e.Offset = new SKPointI(e.Offset.X, v)),
        EffectParameters.IntNumeric<DrawTextEffect>("angle", "Angle", -360, 360, 0, (e, v) => e.Angle = v),
        EffectParameters.Bool<DrawTextEffect>("auto_hide", "Auto hide", false, (e, v) => e.AutoHide = v),
        EffectParameters.Text<DrawTextEffect>("font_family", "Font family", "Arial", (e, v) => e.FontFamily = v),
        EffectParameters.FloatSlider<DrawTextEffect>("font_size", "Font size", 1, 500, 36, (e, v) => e.FontSize = v),
        EffectParameters.Bool<DrawTextEffect>("bold", "Bold", false, (e, v) => e.Bold = v),
        EffectParameters.Bool<DrawTextEffect>("italic", "Italic", false, (e, v) => e.Italic = v),
        EffectParameters.Color<DrawTextEffect>("color", "Color", new SKColor(235, 235, 235), (e, v) => e.Color = v),
        EffectParameters.Bool<DrawTextEffect>("outline", "Outline", false, (e, v) => e.Outline = v),
        EffectParameters.IntNumeric<DrawTextEffect>("outline_size", "Outline size", 1, 100, 5, (e, v) => e.OutlineSize = v),
        EffectParameters.Color<DrawTextEffect>("outline_color", "Outline color", new SKColor(235, 0, 0), (e, v) => e.OutlineColor = v),
        EffectParameters.Bool<DrawTextEffect>("shadow", "Shadow", false, (e, v) => e.Shadow = v),
        EffectParameters.IntNumeric<DrawTextEffect>("shadow_offset_x", "Shadow offset X", -1000, 1000, 0, (e, v) => e.ShadowOffset = new SKPointI(v, e.ShadowOffset.Y)),
        EffectParameters.IntNumeric<DrawTextEffect>("shadow_offset_y", "Shadow offset Y", -1000, 1000, 5, (e, v) => e.ShadowOffset = new SKPointI(e.ShadowOffset.X, v)),
        EffectParameters.Color<DrawTextEffect>("shadow_color", "Shadow color", new SKColor(0, 0, 0, 125), (e, v) => e.ShadowColor = v)
    ];

    public string Text { get; set; } = "Text";

    public DrawingPlacement Placement { get; set; } = DrawingPlacement.TopLeft;

    public SKPointI Offset { get; set; } = new SKPointI(0, 0);

    public int Angle { get; set; }

    public bool AutoHide { get; set; }

    public string FontFamily { get; set; } = "Arial";

    public float FontSize { get; set; } = 36f;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public SKColor Color { get; set; } = new SKColor(235, 235, 235);

    public bool Outline { get; set; }

    public int OutlineSize { get; set; } = 5;

    public SKColor OutlineColor { get; set; } = new SKColor(235, 0, 0);

    public bool Shadow { get; set; }

    public SKPointI ShadowOffset { get; set; } = new SKPointI(0, 5);

    public SKColor ShadowColor { get; set; } = new SKColor(0, 0, 0, 125);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (string.IsNullOrWhiteSpace(Text) || FontSize < 1f)
        {
            return source.Copy();
        }

        SKFontStyleWeight weight = Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
        SKFontStyleSlant slant = Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
        using SKTypeface? typeface = SKTypeface.FromFamilyName(FontFamily, weight, SKFontStyleWidth.Normal, slant);
        using SKFont textFont = new SKFont(typeface, FontSize);

        using SKPath textPath = CreateTextPath(Text, textFont);
        if (textPath.IsEmpty)
        {
            return source.Copy();
        }

        if (Angle != 0)
        {
            SKMatrix rotation = SKMatrix.CreateRotationDegrees(Angle);
            textPath.Transform(rotation);
        }

        SKRect pathRect = textPath.Bounds;
        if (pathRect.IsEmpty)
        {
            return source.Copy();
        }

        SKSizeI textSize = new SKSizeI(
            (int)Math.Ceiling(pathRect.Width) + 1,
            (int)Math.Ceiling(pathRect.Height) + 1);

        SKPointI textPosition = DrawingEffectHelpers.GetPosition(
            Placement,
            Offset,
            new SKSizeI(source.Width, source.Height),
            textSize);

        SKRectI textRectangle = new SKRectI(
            textPosition.X,
            textPosition.Y,
            textPosition.X + textSize.Width,
            textPosition.Y + textSize.Height);

        if (AutoHide && !DrawingEffectHelpers.Contains(new SKRectI(0, 0, source.Width, source.Height), textRectangle))
        {
            return source.Copy();
        }

        SKMatrix translation = SKMatrix.CreateTranslation(textRectangle.Left - pathRect.Left, textRectangle.Top - pathRect.Top);
        textPath.Transform(translation);

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new SKCanvas(result);

        if (Shadow && ShadowColor.Alpha > 0)
        {
            using SKPath shadowPath = new SKPath(textPath);
            SKMatrix shadowTranslation = SKMatrix.CreateTranslation(ShadowOffset.X, ShadowOffset.Y);
            shadowPath.Transform(shadowTranslation);

            if (Outline && OutlineSize > 0)
            {
                DrawStroke(
                    canvas,
                    shadowPath,
                    OutlineSize,
                    ShadowColor);
            }
            else
            {
                DrawFill(
                    canvas,
                    shadowPath,
                    ShadowColor);
            }
        }

        if (Outline && OutlineSize > 0)
        {
            DrawStroke(
                canvas,
                textPath,
                OutlineSize,
                OutlineColor);
        }

        DrawFill(
            canvas,
            textPath,
            Color);

        return result;
    }

    private static void DrawStroke(
        SKCanvas canvas,
        SKPath path,
        int strokeSize,
        SKColor color)
    {
        if (color.Alpha == 0)
        {
            return;
        }

        using SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Math.Max(1, strokeSize),
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true,
            Color = color
        };

        canvas.DrawPath(path, paint);
    }

    private static void DrawFill(
        SKCanvas canvas,
        SKPath path,
        SKColor color)
    {
        if (color.Alpha == 0)
        {
            return;
        }

        using SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            Color = color
        };

        canvas.DrawPath(path, paint);
    }

    private static SKPath CreateTextPath(string text, SKFont textFont)
    {
        SKPath result = new SKPath { FillType = SKPathFillType.Winding };
        string[] lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        if (lines.Length == 0)
        {
            return result;
        }

        SKFontMetrics metrics = textFont.Metrics;
        float lineHeight = Math.Max(metrics.Descent - metrics.Ascent + metrics.Leading, textFont.Size);
        float baselineOffset = -metrics.Ascent;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Length == 0)
            {
                continue;
            }

            using SKPath linePath = textFont.GetTextPath(line, new SKPoint(0f, baselineOffset + (i * lineHeight)));
            result.AddPath(linePath);
        }

        return result;
    }

}