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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class ArtDecoBorderImageEffect : ImageEffectBase
{
    public override string Id => "art_deco_border";
    public override string Name => "Art deco border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.trophy;
    public override string Description => "Adds a bold Art Deco border with geometric fan motifs, stepped lines, and metallic gold accents.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ArtDecoBorderImageEffect>("border_size", "Border size", 20, 300, 64, (e, v) => e.BorderSize = v),
        EffectParameters.Color<ArtDecoBorderImageEffect>("bg_color", "Background color", new SKColor(15, 15, 25), (e, v) => e.BgColor = v),
        EffectParameters.Color<ArtDecoBorderImageEffect>("accent_color", "Accent color", new SKColor(212, 175, 55), (e, v) => e.AccentColor = v),
        EffectParameters.FloatSlider<ArtDecoBorderImageEffect>("fan_spacing", "Fan spacing", 30, 200, 80, (e, v) => e.FanSpacing = v),
        EffectParameters.IntSlider<ArtDecoBorderImageEffect>("fan_rays", "Fan rays", 3, 12, 7, (e, v) => e.FanRays = v),
        EffectParameters.Bool<ArtDecoBorderImageEffect>("inner_lines", "Inner stepped lines", true, (e, v) => e.InnerLines = v)
    ];

    public int BorderSize { get; set; } = 64;
    public SKColor BgColor { get; set; } = new SKColor(15, 15, 25);
    public SKColor AccentColor { get; set; } = new SKColor(212, 175, 55);
    public float FanSpacing { get; set; } = 80f;
    public int FanRays { get; set; } = 7;
    public bool InnerLines { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 20, 300);
        float spacing = Math.Clamp(FanSpacing, 30f, 200f);
        int rays = Math.Clamp(FanRays, 3, 12);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType);

        using SKCanvas canvas = new SKCanvas(result);

        // Fill background
        canvas.Clear(BgColor);

        using SKPaint accentPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f,
            Color = AccentColor
        };

        using SKPaint accentFillPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = new SKColor(AccentColor.Red, AccentColor.Green, AccentColor.Blue, 40)
        };

        float fanRadius = border * 0.65f;

        // Draw fans along top edge
        DrawFanRow(canvas, accentPaint, accentFillPaint, spacing, fanRadius, rays,
            border, border * 0.5f, newWidth - border, false);

        // Draw fans along bottom edge
        DrawFanRow(canvas, accentPaint, accentFillPaint, spacing, fanRadius, rays,
            border, newHeight - border * 0.5f, newWidth - border, true);

        // Draw fans along left edge (vertical)
        DrawFanColumn(canvas, accentPaint, accentFillPaint, spacing, fanRadius, rays,
            border * 0.5f, border, newHeight - border, false);

        // Draw fans along right edge (vertical)
        DrawFanColumn(canvas, accentPaint, accentFillPaint, spacing, fanRadius, rays,
            newWidth - border * 0.5f, border, newHeight - border, true);

        // Outer and inner border lines
        using SKPaint linePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = AccentColor
        };

        // Outer double line
        canvas.DrawRect(2, 2, newWidth - 4, newHeight - 4, linePaint);
        canvas.DrawRect(6, 6, newWidth - 12, newHeight - 12, linePaint);

        // Inner border line
        canvas.DrawRect(border - 2, border - 2, source.Width + 4, source.Height + 4, linePaint);

        if (InnerLines)
        {
            // Stepped lines near inner border
            using SKPaint thinPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f,
                Color = new SKColor(AccentColor.Red, AccentColor.Green, AccentColor.Blue, 100)
            };

            float inset = border * 0.25f;
            canvas.DrawRect(inset, inset, newWidth - inset * 2, newHeight - inset * 2, thinPaint);

            float inset2 = border * 0.75f;
            canvas.DrawRect(inset2, inset2, newWidth - inset2 * 2, newHeight - inset2 * 2, thinPaint);
        }

        // Corner ornaments - small diamond shapes
        DrawCornerDiamond(canvas, accentPaint, 4, 4, border * 0.3f);
        DrawCornerDiamond(canvas, accentPaint, newWidth - 4, 4, border * 0.3f);
        DrawCornerDiamond(canvas, accentPaint, 4, newHeight - 4, border * 0.3f);
        DrawCornerDiamond(canvas, accentPaint, newWidth - 4, newHeight - 4, border * 0.3f);

        // Draw source image
        canvas.DrawBitmap(source, border, border);

        return result;
    }

    private static void DrawFanRow(SKCanvas canvas, SKPaint strokePaint, SKPaint fillPaint,
        float spacing, float radius, int rays, float startX, float centerY, float endX, bool flip)
    {
        float angle0 = flip ? 0f : MathF.PI;
        float sweep = MathF.PI;

        for (float cx = startX; cx <= endX; cx += spacing)
        {
            using SKPath fanPath = new();
            for (int i = 0; i <= rays; i++)
            {
                float a = angle0 + sweep * i / rays;
                float ex = cx + MathF.Cos(a) * radius;
                float ey = centerY + MathF.Sin(a) * radius;
                fanPath.MoveTo(cx, centerY);
                fanPath.LineTo(ex, ey);
            }

            // Arc outline
            SKRect arcRect = new(cx - radius, centerY - radius, cx + radius, centerY + radius);
            fanPath.AddArc(arcRect, flip ? 0f : 180f, 180f);

            canvas.DrawPath(fanPath, fillPaint);
            canvas.DrawPath(fanPath, strokePaint);
        }
    }

    private static void DrawFanColumn(SKCanvas canvas, SKPaint strokePaint, SKPaint fillPaint,
        float spacing, float radius, int rays, float centerX, float startY, float endY, bool flip)
    {
        float angle0 = flip ? -MathF.PI / 2 : MathF.PI / 2;
        float sweep = MathF.PI;

        for (float cy = startY; cy <= endY; cy += spacing)
        {
            using SKPath fanPath = new();
            for (int i = 0; i <= rays; i++)
            {
                float a = angle0 + sweep * i / rays;
                float ex = centerX + MathF.Cos(a) * radius;
                float ey = cy + MathF.Sin(a) * radius;
                fanPath.MoveTo(centerX, cy);
                fanPath.LineTo(ex, ey);
            }

            SKRect arcRect = new(centerX - radius, cy - radius, centerX + radius, cy + radius);
            fanPath.AddArc(arcRect, flip ? 270f : 90f, 180f);

            canvas.DrawPath(fanPath, fillPaint);
            canvas.DrawPath(fanPath, strokePaint);
        }
    }

    private static void DrawCornerDiamond(SKCanvas canvas, SKPaint paint, float cx, float cy, float size)
    {
        using SKPath diamond = new();
        diamond.MoveTo(cx, cy - size);
        diamond.LineTo(cx + size, cy);
        diamond.LineTo(cx, cy + size);
        diamond.LineTo(cx - size, cy);
        diamond.Close();
        canvas.DrawPath(diamond, paint);
    }
}