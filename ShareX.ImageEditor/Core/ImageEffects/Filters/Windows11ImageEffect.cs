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

public sealed class Windows11ImageEffect : ImageEffectBase
{
    public override string Id => "windows11_window";
    public override string Name => "Windows 11";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.monitor;
    public override string Description => "Wraps the image in a Windows 11-style window frame with a title bar and window controls.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Text<Windows11ImageEffect>("title", "Title", string.Empty, (e, v) => e.Title = v),
        EffectParameters.IntSlider<Windows11ImageEffect>("corner_radius", "Corner radius", 0, 16, 8, (e, v) => e.CornerRadius = v),
        EffectParameters.Bool<Windows11ImageEffect>("dark_mode", "Dark mode", false, (e, v) => e.DarkMode = v),
        EffectParameters.IntSlider<Windows11ImageEffect>("shadow_size", "Shadow size", 0, 60, 16, (e, v) => e.ShadowSize = v),
        EffectParameters.IntSlider<Windows11ImageEffect>("padding", "Padding", 0, 100, 20, (e, v) => e.Padding = v)
    ];

    public string Title { get; set; } = string.Empty;
    public int CornerRadius { get; set; } = 8;
    public bool DarkMode { get; set; }
    public int ShadowSize { get; set; } = 16;
    public int Padding { get; set; } = 20;

    private const int TitleBarHeight = 32;
    private const float ButtonWidth = 46;
    private const float ButtonHeight = 32;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cornerRadius = Math.Clamp(CornerRadius, 0, 16);
        int shadowSize = Math.Clamp(ShadowSize, 0, 60);
        int padding = Math.Clamp(Padding, 0, 100);

        int windowWidth = source.Width;
        int windowHeight = TitleBarHeight + source.Height;

        int totalWidth = windowWidth + (padding * 2) + (shadowSize * 2);
        int totalHeight = windowHeight + (padding * 2) + (shadowSize * 2);

        SKBitmap result = new(totalWidth, totalHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        float windowX = padding + shadowSize;
        float windowY = padding + shadowSize;

        SKRoundRect windowRRect = new(
            new SKRect(windowX, windowY, windowX + windowWidth, windowY + windowHeight),
            cornerRadius, cornerRadius);

        // Draw shadow
        if (shadowSize > 0)
        {
            using SKPaint shadowPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 40),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, shadowSize / 2.5f)
            };
            canvas.DrawRoundRect(windowRRect, shadowPaint);
        }

        // Clip to window shape
        canvas.Save();
        canvas.ClipRoundRect(windowRRect, SKClipOperation.Intersect, true);

        // Draw title bar
        SKColor titleBarColor = DarkMode ? new SKColor(32, 32, 32) : new SKColor(243, 243, 243);

        using (SKPaint titlePaint = new() { IsAntialias = true, Color = titleBarColor, Style = SKPaintStyle.Fill })
        {
            canvas.DrawRect(windowX, windowY, windowWidth, TitleBarHeight, titlePaint);
        }

        // Title bar bottom border (subtle)
        SKColor borderColor = DarkMode ? new SKColor(50, 50, 50) : new SKColor(229, 229, 229);
        using (SKPaint borderPaint = new() { IsAntialias = true, Color = borderColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1 })
        {
            canvas.DrawLine(windowX, windowY + TitleBarHeight, windowX + windowWidth, windowY + TitleBarHeight, borderPaint);
        }

        // Window control buttons (right side): minimize, maximize, close
        SKColor buttonTextColor = DarkMode ? new SKColor(200, 200, 200) : new SKColor(90, 90, 90);
        float controlsRight = windowX + windowWidth;

        // Close button (red hover area, X icon)
        DrawWindowButton(canvas, controlsRight - ButtonWidth, windowY,
            ButtonWidth, ButtonHeight, new SKColor(196, 43, 28), SKColors.White, DrawCloseIcon, true);

        // Maximize button
        DrawWindowButton(canvas, controlsRight - ButtonWidth * 2, windowY,
            ButtonWidth, ButtonHeight, titleBarColor, buttonTextColor, DrawMaximizeIcon, false);

        // Minimize button
        DrawWindowButton(canvas, controlsRight - ButtonWidth * 3, windowY,
            ButtonWidth, ButtonHeight, titleBarColor, buttonTextColor, DrawMinimizeIcon, false);

        // Draw title text
        if (!string.IsNullOrWhiteSpace(Title))
        {
            SKColor textColor = DarkMode ? new SKColor(220, 220, 220) : new SKColor(50, 50, 50);
            using SKTypeface titleTypeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
                                            ?? SKTypeface.Default;
            using SKFont titleFont = new SKFont(titleTypeface, 12f);
            using SKPaint textPaint = new()
            {
                IsAntialias = true,
                Color = textColor
            };

            float textX = windowX + 12;
            float textY = windowY + (TitleBarHeight / 2f) + (12f / 3f);
            canvas.DrawText(Title, textX, textY, SKTextAlign.Left, titleFont, textPaint);
        }

        // Draw the source image below the title bar
        canvas.DrawBitmap(source, windowX, windowY + TitleBarHeight);

        canvas.Restore();

        // Draw window border
        SKColor windowBorderColor = DarkMode ? new SKColor(60, 60, 60, 160) : new SKColor(200, 200, 200, 160);
        using (SKPaint windowBorderPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = windowBorderColor
        })
        {
            canvas.DrawRoundRect(windowRRect, windowBorderPaint);
        }

        // Subtle top highlight (mica-like)
        if (!DarkMode)
        {
            using SKPaint micaPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(windowX, windowY),
                    new SKPoint(windowX, windowY + 2),
                    [new SKColor(255, 255, 255, 80), new SKColor(255, 255, 255, 0)],
                    SKShaderTileMode.Clamp)
            };

            canvas.Save();
            canvas.ClipRoundRect(windowRRect, SKClipOperation.Intersect, true);
            canvas.DrawRect(windowX, windowY, windowWidth, 2, micaPaint);
            canvas.Restore();
        }

        return result;
    }

    private delegate void DrawIconDelegate(SKCanvas canvas, float cx, float cy, float size, SKColor color);

    private static void DrawWindowButton(SKCanvas canvas, float x, float y, float w, float h,
        SKColor bgColor, SKColor iconColor, DrawIconDelegate drawIcon, bool isClose)
    {
        using (SKPaint bgPaint = new() { IsAntialias = true, Color = bgColor, Style = SKPaintStyle.Fill })
        {
            canvas.DrawRect(x, y, w, h, bgPaint);
        }

        float cx = x + w / 2f;
        float cy = y + h / 2f;
        drawIcon(canvas, cx, cy, 10f, isClose ? SKColors.White : iconColor);
    }

    private static void DrawCloseIcon(SKCanvas canvas, float cx, float cy, float size, SKColor color)
    {
        float half = size / 2f;
        using SKPaint paint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            StrokeCap = SKStrokeCap.Round,
            Color = color
        };
        canvas.DrawLine(cx - half, cy - half, cx + half, cy + half, paint);
        canvas.DrawLine(cx + half, cy - half, cx - half, cy + half, paint);
    }

    private static void DrawMaximizeIcon(SKCanvas canvas, float cx, float cy, float size, SKColor color)
    {
        float half = size / 2f;
        using SKPaint paint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = color
        };
        canvas.DrawRect(cx - half, cy - half, size, size, paint);
    }

    private static void DrawMinimizeIcon(SKCanvas canvas, float cx, float cy, float size, SKColor color)
    {
        float half = size / 2f;
        using SKPaint paint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            StrokeCap = SKStrokeCap.Round,
            Color = color
        };
        canvas.DrawLine(cx - half, cy, cx + half, cy, paint);
    }
}