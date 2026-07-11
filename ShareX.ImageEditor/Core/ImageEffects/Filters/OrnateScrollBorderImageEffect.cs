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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class OrnateScrollBorderImageEffect : ImageEffectBase
{
    public override string Id => "ornate_scroll_border";
    public override string Name => "Ornate scroll border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scroll;
    public override string Description => "Adds an elegant Victorian-style ornate border with decorative corner scrollwork and double-line detailing.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<OrnateScrollBorderImageEffect>("border_size", "Border size", 10, 200, 48, (e, v) => e.BorderSize = v),
        EffectParameters.Color<OrnateScrollBorderImageEffect>("frame_color", "Frame color", new SKColor(245, 240, 220), (e, v) => e.FrameColor = v),
        EffectParameters.Color<OrnateScrollBorderImageEffect>("ornament_color", "Ornament color", new SKColor(160, 130, 60), (e, v) => e.OrnamentColor = v),
        EffectParameters.FloatSlider<OrnateScrollBorderImageEffect>("line_thickness", "Line thickness", 0.5f, 6, 2f, (e, v) => e.LineThickness = v),
        EffectParameters.Bool<OrnateScrollBorderImageEffect>("fill_background", "Fill background", true, (e, v) => e.FillBackground = v),
        EffectParameters.Bool<OrnateScrollBorderImageEffect>("shadow", "Shadow", true, (e, v) => e.Shadow = v)
    ];

    public int BorderSize { get; set; } = 48;
    public SKColor FrameColor { get; set; } = new SKColor(245, 240, 220);
    public SKColor OrnamentColor { get; set; } = new SKColor(160, 130, 60);
    public float LineThickness { get; set; } = 2f;
    public bool FillBackground { get; set; } = true;
    public bool Shadow { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 200);
        float lineThick = Math.Clamp(LineThickness, 0.5f, 6f);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        float w = newWidth;
        float h = newHeight;
        float b = border;

        // Drop shadow
        if (Shadow)
        {
            using SKPaint shadowPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 40),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, border * 0.18f)
            };
            canvas.DrawRect(border + 3, border + 4, source.Width, source.Height, shadowPaint);
        }

        // Frame background fill
        if (FillBackground)
        {
            using SKPaint bgPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill, Color = FrameColor };
            canvas.DrawRect(0, 0, w, h, bgPaint);
        }

        // Source image
        canvas.DrawBitmap(source, border, border);

        // Subtle inner shadow over image edge
        using (SKPaint innerShadow = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(
                new SKPoint(b, b), new SKPoint(b + border * 0.15f, b + border * 0.15f),
                [new SKColor(0, 0, 0, 45), SKColors.Transparent], SKShaderTileMode.Clamp)
        })
        {
            canvas.DrawRect(b, b, source.Width, source.Height, innerShadow);
        }

        // Ornament paint
        using SKPaint ornamentPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = lineThick,
            Color = OrnamentColor,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        // Outer border line
        canvas.DrawRect(lineThick / 2f, lineThick / 2f, w - lineThick, h - lineThick, ornamentPaint);

        // Inner border line (offset inward by ~30% of border)
        float inset = border * 0.30f;
        canvas.DrawRect(inset, inset, w - inset * 2f, h - inset * 2f, ornamentPaint);

        // Second inner line (tighter)
        float inset2 = border * 0.70f;
        canvas.DrawRect(inset2, inset2, w - inset2 * 2f, h - inset2 * 2f, ornamentPaint);

        // Decorative mid-line (thinner)
        ornamentPaint.StrokeWidth = lineThick * 0.5f;
        float insetMid = border * 0.50f;
        canvas.DrawRect(insetMid, insetMid, w - insetMid * 2f, h - insetMid * 2f, ornamentPaint);

        // Restore full thickness for ornaments
        ornamentPaint.StrokeWidth = lineThick;
        ornamentPaint.Style = SKPaintStyle.Fill;

        // Corner ornaments (diamonds + scrollwork)
        float ornSize = Math.Min(border * 0.35f, 18f);
        DrawCornerOrnament(canvas, ornamentPaint, inset, inset, ornSize);
        DrawCornerOrnament(canvas, ornamentPaint, w - inset, inset, ornSize);
        DrawCornerOrnament(canvas, ornamentPaint, inset, h - inset, ornSize);
        DrawCornerOrnament(canvas, ornamentPaint, w - inset, h - inset, ornSize);

        // Mid-side ornaments (smaller)
        float ornSmall = ornSize * 0.65f;
        DrawCornerOrnament(canvas, ornamentPaint, w * 0.5f, inset, ornSmall);
        DrawCornerOrnament(canvas, ornamentPaint, w * 0.5f, h - inset, ornSmall);
        DrawCornerOrnament(canvas, ornamentPaint, inset, h * 0.5f, ornSmall);
        DrawCornerOrnament(canvas, ornamentPaint, w - inset, h * 0.5f, ornSmall);

        // Draw corner scrollwork spirals
        ornamentPaint.Style = SKPaintStyle.Stroke;
        ornamentPaint.StrokeWidth = lineThick * 0.9f;
        float scrollSize = border * 0.40f;
        DrawScrollwork(canvas, ornamentPaint, lineThick * 0.5f, lineThick * 0.5f, scrollSize, 0);
        DrawScrollwork(canvas, ornamentPaint, w - lineThick * 0.5f, lineThick * 0.5f, scrollSize, 1);
        DrawScrollwork(canvas, ornamentPaint, lineThick * 0.5f, h - lineThick * 0.5f, scrollSize, 2);
        DrawScrollwork(canvas, ornamentPaint, w - lineThick * 0.5f, h - lineThick * 0.5f, scrollSize, 3);

        return result;
    }

    private static void DrawCornerOrnament(SKCanvas canvas, SKPaint paint, float cx, float cy, float size)
    {
        // Draw a diamond shape
        using SKPath diamond = new();
        diamond.MoveTo(cx, cy - size);
        diamond.LineTo(cx + size * 0.55f, cy);
        diamond.LineTo(cx, cy + size);
        diamond.LineTo(cx - size * 0.55f, cy);
        diamond.Close();
        canvas.DrawPath(diamond, paint);

        // Small center dot
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(cx, cy, size * 0.18f, paint);
    }

    private static void DrawScrollwork(SKCanvas canvas, SKPaint paint, float anchorX, float anchorY, float size, int corner)
    {
        // Each corner has a pair of S-curves / spirals radiating from the corner inward
        // corner: 0=TL, 1=TR, 2=BL, 3=BR
        float sx = corner == 0 || corner == 2 ? 1f : -1f;   // horizontal sign
        float sy = corner == 0 || corner == 1 ? 1f : -1f;   // vertical sign

        float x = anchorX;
        float y = anchorY;

        // Horizontal scroll arm
        using SKPath hScroll = new();
        hScroll.MoveTo(x, y);
        hScroll.CubicTo(
            x + sx * size * 0.4f, y,
            x + sx * size * 0.7f, y + sy * size * 0.2f,
            x + sx * size * 0.8f, y + sy * size * 0.5f);
        hScroll.CubicTo(
            x + sx * size * 0.85f, y + sy * size * 0.7f,
            x + sx * size * 0.65f, y + sy * size * 0.85f,
            x + sx * size * 0.45f, y + sy * size * 0.75f);
        canvas.DrawPath(hScroll, paint);

        // Vertical scroll arm
        using SKPath vScroll = new();
        vScroll.MoveTo(x, y);
        vScroll.CubicTo(
            x, y + sy * size * 0.4f,
            x + sx * size * 0.2f, y + sy * size * 0.7f,
            x + sx * size * 0.5f, y + sy * size * 0.8f);
        vScroll.CubicTo(
            x + sx * size * 0.7f, y + sy * size * 0.85f,
            x + sx * size * 0.85f, y + sy * size * 0.65f,
            x + sx * size * 0.75f, y + sy * size * 0.45f);
        canvas.DrawPath(vScroll, paint);
    }
}