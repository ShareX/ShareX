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

public sealed class TextWatermarkEffect : ImageEffectBase
{
    private int _cornerRadius = 4;
    private int _borderSize = 1;

    public override string Id => "text_watermark";
    public override string Name => "Text watermark";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.text_cursor;
    public override string Description => "Draws a text watermark with background on the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Text<TextWatermarkEffect>("text", "Text", "Text watermark", (e, v) => e.Text = v),
        EffectParameters.Enum<TextWatermarkEffect, DrawingPlacement>(
            "placement", "Placement", DrawingPlacement.BottomRight, (e, v) => e.Placement = v,
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
        EffectParameters.IntNumeric<TextWatermarkEffect>("offset_x", "Offset X", -10000, 10000, 5, (e, v) => e.Offset = new SKPointI(v, e.Offset.Y)),
        EffectParameters.IntNumeric<TextWatermarkEffect>("offset_y", "Offset Y", -10000, 10000, 5, (e, v) => e.Offset = new SKPointI(e.Offset.X, v)),
        EffectParameters.Bool<TextWatermarkEffect>("auto_hide", "Auto hide", false, (e, v) => e.AutoHide = v),
        EffectParameters.Text<TextWatermarkEffect>("font_family", "Font family", "Arial", (e, v) => e.FontFamily = v),
        EffectParameters.FloatSlider<TextWatermarkEffect>("font_size", "Font size", 1, 500, 15, (e, v) => e.FontSize = v),
        EffectParameters.Bool<TextWatermarkEffect>("bold", "Bold", false, (e, v) => e.Bold = v),
        EffectParameters.Bool<TextWatermarkEffect>("italic", "Italic", false, (e, v) => e.Italic = v),
        EffectParameters.Color<TextWatermarkEffect>("text_color", "Text color", new SKColor(235, 235, 235), (e, v) => e.TextColor = v),
        EffectParameters.Bool<TextWatermarkEffect>("draw_text_shadow", "Draw text shadow", false, (e, v) => e.DrawTextShadow = v),
        EffectParameters.Color<TextWatermarkEffect>("text_shadow_color", "Text shadow color", SKColors.Black, (e, v) => e.TextShadowColor = v),
        EffectParameters.IntNumeric<TextWatermarkEffect>("text_shadow_offset_x", "Text shadow offset X", -1000, 1000, -1, (e, v) => e.TextShadowOffset = new SKPointI(v, e.TextShadowOffset.Y)),
        EffectParameters.IntNumeric<TextWatermarkEffect>("text_shadow_offset_y", "Text shadow offset Y", -1000, 1000, -1, (e, v) => e.TextShadowOffset = new SKPointI(e.TextShadowOffset.X, v)),
        EffectParameters.IntNumeric<TextWatermarkEffect>("corner_radius", "Corner radius", 0, 100, 4, (e, v) => e.CornerRadius = v),
        EffectParameters.IntNumeric<TextWatermarkEffect>("padding_left", "Padding left", 0, 100, 5, (e, v) => e.PaddingLeft = v),
        EffectParameters.IntNumeric<TextWatermarkEffect>("padding_top", "Padding top", 0, 100, 5, (e, v) => e.PaddingTop = v),
        EffectParameters.IntNumeric<TextWatermarkEffect>("padding_right", "Padding right", 0, 100, 5, (e, v) => e.PaddingRight = v),
        EffectParameters.IntNumeric<TextWatermarkEffect>("padding_bottom", "Padding bottom", 0, 100, 5, (e, v) => e.PaddingBottom = v),
        EffectParameters.Bool<TextWatermarkEffect>("draw_border", "Draw border", true, (e, v) => e.DrawBorder = v),
        EffectParameters.Color<TextWatermarkEffect>("border_color", "Border color", SKColors.Black, (e, v) => e.BorderColor = v),
        EffectParameters.IntNumeric<TextWatermarkEffect>("border_size", "Border size", 0, 50, 1, (e, v) => e.BorderSize = v),
        EffectParameters.Bool<TextWatermarkEffect>("draw_background", "Draw background", true, (e, v) => e.DrawBackground = v),
        EffectParameters.Color<TextWatermarkEffect>("background_color", "Background color", new SKColor(42, 47, 56), (e, v) => e.BackgroundColor = v)
    ];

    public string Text { get; set; } = "Text watermark";

    public DrawingPlacement Placement { get; set; } = DrawingPlacement.BottomRight;

    public SKPointI Offset { get; set; } = new SKPointI(5, 5);

    public bool AutoHide { get; set; }

    public string FontFamily { get; set; } = "Arial";

    public float FontSize { get; set; } = 15f;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public SKColor TextColor { get; set; } = new SKColor(235, 235, 235);

    public bool DrawTextShadow { get; set; }

    public SKColor TextShadowColor { get; set; } = SKColors.Black;

    public SKPointI TextShadowOffset { get; set; } = new SKPointI(-1, -1);

    public int CornerRadius
    {
        get => _cornerRadius;
        set => _cornerRadius = Math.Max(0, value);
    }

    public int PaddingLeft { get; set; } = 5;

    public int PaddingTop { get; set; } = 5;

    public int PaddingRight { get; set; } = 5;

    public int PaddingBottom { get; set; } = 5;

    public bool DrawBorder { get; set; } = true;

    public SKColor BorderColor { get; set; } = SKColors.Black;

    public int BorderSize
    {
        get => _borderSize;
        set => _borderSize = Math.Max(0, value);
    }

    public bool DrawBackground { get; set; } = true;

    public SKColor BackgroundColor { get; set; } = new SKColor(42, 47, 56);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (FontSize < 1f)
        {
            return source.Copy();
        }

        string text = DrawingEffectHelpers.ExpandTextVariables(Text, new SKSizeI(source.Width, source.Height));
        if (string.IsNullOrWhiteSpace(text))
        {
            return source.Copy();
        }

        int paddingLeft = Math.Max(0, PaddingLeft);
        int paddingTop = Math.Max(0, PaddingTop);
        int paddingRight = Math.Max(0, PaddingRight);
        int paddingBottom = Math.Max(0, PaddingBottom);

        SKFontStyleWeight weight = Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
        SKFontStyleSlant slant = Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

        using SKTypeface? typeface = SKTypeface.FromFamilyName(FontFamily, weight, SKFontStyleWidth.Normal, slant);
        using SKFont textFont = new SKFont(typeface, FontSize);
        using SKPaint textPaint = new SKPaint
        {
            IsAntialias = true,
            Color = TextColor
        };

        string[] lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        if (lines.Length == 0)
        {
            return source.Copy();
        }

        SKFontMetrics metrics = textFont.Metrics;
        float rawTextHeight = Math.Max(metrics.Descent - metrics.Ascent, textFont.Size);
        float lineHeight = Math.Max(rawTextHeight + metrics.Leading, textFont.Size);
        float baselineOffset = -metrics.Ascent;

        float maxTextWidth = 0f;
        foreach (string line in lines)
        {
            maxTextWidth = Math.Max(maxTextWidth, textFont.MeasureText(line));
        }

        float totalTextHeight = rawTextHeight + Math.Max(0, lines.Length - 1) * lineHeight;
        if (maxTextWidth <= 0f && totalTextHeight <= 0f)
        {
            return source.Copy();
        }

        SKSizeI watermarkSize = new SKSizeI(
            (int)Math.Ceiling(maxTextWidth + paddingLeft + paddingRight),
            (int)Math.Ceiling(totalTextHeight + paddingTop + paddingBottom));

        if (watermarkSize.Width <= 0 || watermarkSize.Height <= 0)
        {
            return source.Copy();
        }

        SKPointI watermarkPosition = DrawingEffectHelpers.GetPosition(
            Placement,
            Offset,
            new SKSizeI(source.Width, source.Height),
            watermarkSize);

        SKRectI watermarkRect = new SKRectI(
            watermarkPosition.X,
            watermarkPosition.Y,
            watermarkPosition.X + watermarkSize.Width,
            watermarkPosition.Y + watermarkSize.Height);

        if (AutoHide && !DrawingEffectHelpers.Contains(new SKRectI(0, 0, source.Width, source.Height), watermarkRect))
        {
            return source.Copy();
        }

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new SKCanvas(result);

        float radius = MathF.Min(CornerRadius, MathF.Min(watermarkRect.Width, watermarkRect.Height) / 2f);
        SKRect backgroundRect = new SKRect(watermarkRect.Left, watermarkRect.Top, watermarkRect.Right, watermarkRect.Bottom);

        if (DrawBackground && BackgroundColor.Alpha > 0)
        {
            using SKPaint backgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = BackgroundColor
            };

            canvas.DrawRoundRect(backgroundRect, radius, radius, backgroundPaint);
        }

        if (DrawBorder && BorderColor.Alpha > 0)
        {
            using SKPaint borderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = BorderColor,
                StrokeWidth = Math.Max(1, BorderSize)
            };

            canvas.DrawRoundRect(backgroundRect, radius, radius, borderPaint);
        }

        float textX = watermarkRect.Left + paddingLeft;
        float textY = watermarkRect.Top + paddingTop + baselineOffset;

        if (DrawTextShadow && TextShadowColor.Alpha > 0)
        {
            using SKPaint shadowPaint = new SKPaint
            {
                IsAntialias = true,
                Color = TextShadowColor
            };

            DrawLines(canvas, lines, textX + TextShadowOffset.X, textY + TextShadowOffset.Y, lineHeight, textFont, shadowPaint);
        }

        DrawLines(canvas, lines, textX, textY, lineHeight, textFont, textPaint);
        return result;
    }

    private static void DrawLines(SKCanvas canvas, IReadOnlyList<string> lines, float x, float baselineY, float lineHeight, SKFont font, SKPaint paint)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            if (line.Length == 0)
            {
                continue;
            }

            canvas.DrawText(line, x, baselineY + (i * lineHeight), font, paint);
        }
    }
}