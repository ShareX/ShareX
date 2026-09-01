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

public sealed class CrimsonBorderImageEffect : ImageEffectBase
{
    public override string Id => "crimson_border";
    public override string Name => "Crimson border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sword;
    public override string Description => "Adds a rich crimson/dark red border with an inner glow effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<CrimsonBorderImageEffect>("size", "Size", 2, 60, 12, (e, v) => e.Size = v),
        EffectParameters.Color<CrimsonBorderImageEffect>("color", "Color", new SKColor(153, 0, 18), (e, v) => e.Color = v),
        EffectParameters.FloatSlider<CrimsonBorderImageEffect>("glow_strength", "Glow strength", 0, 100, 50, (e, v) => e.GlowStrength = v),
        EffectParameters.Bool<CrimsonBorderImageEffect>("inner_line", "Inner line", true, (e, v) => e.InnerLine = v),
        EffectParameters.Bool<CrimsonBorderImageEffect>("outer_line", "Outer line", true, (e, v) => e.OuterLine = v)
    ];

    public int Size { get; set; } = 12;
    public SKColor Color { get; set; } = new SKColor(153, 0, 18);
    public float GlowStrength { get; set; } = 50f;
    public bool InnerLine { get; set; } = true;
    public bool OuterLine { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int size = Math.Clamp(Size, 2, 60);
        float glow = Math.Clamp(GlowStrength, 0f, 100f) / 100f;

        int newWidth = source.Width + size * 2;
        int newHeight = source.Height + size * 2;

        SKColor baseColor = Color;
        SKColor darkColor = DarkenColor(baseColor, 0.5f);
        SKColor lightColor = LightenColor(baseColor, 0.3f);

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Fill border area with base
        using (SKPaint basePaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill, Color = baseColor })
        {
            canvas.DrawRect(0, 0, newWidth, newHeight, basePaint);
        }

        // Inner glow from edges toward center
        if (glow > 0f)
        {
            float glowWidth = size * 0.7f;

            // Top glow (dark edge -> lighter)
            using (SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, glowWidth),
                    [darkColor.WithAlpha((byte)(200 * glow)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
            })
            { canvas.DrawRect(0, 0, newWidth, glowWidth, p); }

            // Bottom glow
            using (SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(0, newHeight), new SKPoint(0, newHeight - glowWidth),
                    [darkColor.WithAlpha((byte)(200 * glow)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
            })
            { canvas.DrawRect(0, newHeight - glowWidth, newWidth, glowWidth, p); }

            // Left glow
            using (SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(glowWidth, 0),
                    [darkColor.WithAlpha((byte)(180 * glow)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
            })
            { canvas.DrawRect(0, 0, glowWidth, newHeight, p); }

            // Right glow
            using (SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(newWidth, 0), new SKPoint(newWidth - glowWidth, 0),
                    [darkColor.WithAlpha((byte)(180 * glow)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
            })
            { canvas.DrawRect(newWidth - glowWidth, 0, glowWidth, newHeight, p); }

            // Inner highlight near the image
            float innerGlowW = size * 0.4f;
            using (SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(0, size), new SKPoint(0, size - innerGlowW),
                    [lightColor.WithAlpha((byte)(120 * glow)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
            })
            { canvas.DrawRect(size, size - innerGlowW, source.Width, innerGlowW, p); }

            using (SKPaint p = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(0, newHeight - size), new SKPoint(0, newHeight - size + innerGlowW),
                    [lightColor.WithAlpha((byte)(120 * glow)), baseColor.WithAlpha(0)], SKShaderTileMode.Clamp)
            })
            { canvas.DrawRect(size, newHeight - size, source.Width, innerGlowW, p); }
        }

        // Draw the image
        canvas.DrawBitmap(source, size, size);

        // Inner line
        if (InnerLine)
        {
            using SKPaint linePaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                Color = lightColor.WithAlpha(180)
            };
            canvas.DrawRect(size - 0.5f, size - 0.5f, source.Width + 1, source.Height + 1, linePaint);
        }

        // Outer line
        if (OuterLine)
        {
            using SKPaint linePaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color = darkColor
            };
            canvas.DrawRect(0.5f, 0.5f, newWidth - 1, newHeight - 1, linePaint);
        }

        return result;
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