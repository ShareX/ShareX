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

public sealed class GoldenBorderImageEffect : ImageEffectBase
{
    public override string Id => "golden_border";
    public override string Name => "Golden border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.crown;
    public override string Description => "Adds a luxurious picture frame border to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<GoldenBorderImageEffect>("size", "Size", 5, 80, 25, (e, v) => e.Size = v),
        EffectParameters.FloatSlider<GoldenBorderImageEffect>("bevel_strength", "Bevel strength", 0, 100, 60, (e, v) => e.BevelStrength = v),
        EffectParameters.Color<GoldenBorderImageEffect>("color", "Color", new SKColor(212, 175, 55), (e, v) => e.Color = v),
        EffectParameters.Bool<GoldenBorderImageEffect>("inner_border", "Inner border", true, (e, v) => e.InnerBorder = v),
        EffectParameters.Bool<GoldenBorderImageEffect>("outer_border", "Outer border", true, (e, v) => e.OuterBorder = v)
    ];

    public int Size { get; set; } = 25;
    public float BevelStrength { get; set; } = 60f;
    public SKColor Color { get; set; } = new SKColor(212, 175, 55);
    public bool InnerBorder { get; set; } = true;
    public bool OuterBorder { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int size = Math.Clamp(Size, 5, 80);
        float bevel = Math.Clamp(BevelStrength, 0f, 100f) / 100f;

        // Derive color palette from the user-chosen base color
        SKColor baseColor = Color;
        SKColor darkColor = DarkenColor(baseColor, 0.55f);
        SKColor lightColor = LightenColor(baseColor, 0.35f);
        SKColor highlightColor = LightenColor(baseColor, 0.60f);

        int newWidth = source.Width + size * 2;
        int newHeight = source.Height + size * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Draw the frame
        DrawFrame(canvas, newWidth, newHeight, size, bevel, baseColor, darkColor, lightColor, highlightColor);

        // Draw the image centered
        canvas.DrawBitmap(source, size, size);

        // Inner border line
        if (InnerBorder)
        {
            using SKPaint innerPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color = darkColor
            };
            canvas.DrawRect(size - 1, size - 1, source.Width + 2, source.Height + 2, innerPaint);
        }

        // Outer border line
        if (OuterBorder)
        {
            using SKPaint outerPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color = darkColor
            };
            canvas.DrawRect(0.5f, 0.5f, newWidth - 1, newHeight - 1, outerPaint);
        }

        return result;
    }

    private static void DrawFrame(SKCanvas canvas, int width, int height, int size, float bevel,
        SKColor baseColor, SKColor darkColor, SKColor lightColor, SKColor highlightColor)
    {
        // Base fill
        using (SKPaint basePaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill, Color = baseColor })
        {
            canvas.DrawRect(0, 0, width, size, basePaint);
            canvas.DrawRect(0, height - size, width, size, basePaint);
            canvas.DrawRect(0, 0, size, height, basePaint);
            canvas.DrawRect(width - size, 0, size, height, basePaint);
        }

        if (bevel <= 0f) return;

        float bevelWidth = size * 0.35f * bevel;

        // Top highlight
        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, bevelWidth),
                [highlightColor.WithAlpha((byte)(200 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(0, 0, width, bevelWidth, p); }

        // Left highlight
        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(bevelWidth, 0),
                [highlightColor.WithAlpha((byte)(180 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(0, 0, bevelWidth, height, p); }

        // Bottom shadow
        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(0, height), new SKPoint(0, height - bevelWidth),
                [darkColor.WithAlpha((byte)(200 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(0, height - bevelWidth, width, bevelWidth, p); }

        // Right shadow
        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(width, 0), new SKPoint(width - bevelWidth, 0),
                [darkColor.WithAlpha((byte)(200 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(width - bevelWidth, 0, bevelWidth, height, p); }

        // Inner bevel
        float innerBevelWidth = size * 0.25f * bevel;

        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(0, size), new SKPoint(0, size - innerBevelWidth),
                [darkColor.WithAlpha((byte)(160 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(size, size - innerBevelWidth, width - size * 2, innerBevelWidth, p); }

        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(size, 0), new SKPoint(size - innerBevelWidth, 0),
                [darkColor.WithAlpha((byte)(140 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(size - innerBevelWidth, size, innerBevelWidth, height - size * 2, p); }

        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(0, height - size), new SKPoint(0, height - size + innerBevelWidth),
                [lightColor.WithAlpha((byte)(140 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(size, height - size, width - size * 2, innerBevelWidth, p); }

        using (SKPaint p = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(new SKPoint(width - size, 0), new SKPoint(width - size + innerBevelWidth, 0),
                [lightColor.WithAlpha((byte)(140 * bevel)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
        })
        { canvas.DrawRect(width - size, size, innerBevelWidth, height - size * 2, p); }

        // Center groove lines
        float grooveY = size * 0.5f;
        using (SKPaint gd = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1, Color = darkColor.WithAlpha((byte)(120 * bevel)) })
        {
            canvas.DrawLine(0, grooveY, width, grooveY, gd);
            canvas.DrawLine(0, height - grooveY, width, height - grooveY, gd);
            canvas.DrawLine(grooveY, 0, grooveY, height, gd);
            canvas.DrawLine(width - grooveY, 0, width - grooveY, height, gd);
        }
        using (SKPaint gl = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1, Color = highlightColor.WithAlpha((byte)(80 * bevel)) })
        {
            canvas.DrawLine(0, grooveY + 1, width, grooveY + 1, gl);
            canvas.DrawLine(0, height - grooveY + 1, width, height - grooveY + 1, gl);
            canvas.DrawLine(grooveY + 1, 0, grooveY + 1, height, gl);
            canvas.DrawLine(width - grooveY + 1, 0, width - grooveY + 1, height, gl);
        }
    }

    private static SKColor DarkenColor(SKColor c, float factor)
    {
        return new SKColor(
            (byte)(c.Red * (1f - factor)),
            (byte)(c.Green * (1f - factor)),
            (byte)(c.Blue * (1f - factor)),
            c.Alpha);
    }

    private static SKColor LightenColor(SKColor c, float factor)
    {
        return new SKColor(
            (byte)Math.Min(255, c.Red + (255 - c.Red) * factor),
            (byte)Math.Min(255, c.Green + (255 - c.Green) * factor),
            (byte)Math.Min(255, c.Blue + (255 - c.Blue) * factor),
            c.Alpha);
    }
}