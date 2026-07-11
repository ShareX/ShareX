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

public sealed class MacOSWindowImageEffect : ImageEffectBase
{
    public override string Id => "macos_window";
    public override string Name => "macOS";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.monitor;
    public override string Description => "Wraps the image in a macOS-style window frame with a title bar and traffic light buttons.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Text<MacOSWindowImageEffect>("title", "Title", string.Empty, (e, v) => e.Title = v),
        EffectParameters.IntSlider<MacOSWindowImageEffect>("corner_radius", "Corner radius", 0, 30, 10, (e, v) => e.CornerRadius = v),
        EffectParameters.Bool<MacOSWindowImageEffect>("dark_mode", "Dark mode", false, (e, v) => e.DarkMode = v),
        EffectParameters.IntSlider<MacOSWindowImageEffect>("shadow_size", "Shadow size", 0, 60, 20, (e, v) => e.ShadowSize = v),
        EffectParameters.IntSlider<MacOSWindowImageEffect>("padding", "Padding", 0, 100, 20, (e, v) => e.Padding = v)
    ];

    public string Title { get; set; } = string.Empty;
    public int CornerRadius { get; set; } = 10;
    public bool DarkMode { get; set; }
    public int ShadowSize { get; set; } = 20;
    public int Padding { get; set; } = 20;

    private const int TitleBarHeight = 38;
    private const int ButtonRadius = 6;
    private const int ButtonSpacing = 20;
    private const int ButtonLeftMargin = 14;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cornerRadius = Math.Clamp(CornerRadius, 0, 30);
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
                Color = new SKColor(0, 0, 0, 60),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, shadowSize / 2f)
            };
            canvas.DrawRoundRect(windowRRect, shadowPaint);
        }

        // Clip to window shape
        canvas.Save();
        canvas.ClipRoundRect(windowRRect, SKClipOperation.Intersect, true);

        // Draw title bar
        SKColor titleBarColor = DarkMode ? new SKColor(56, 55, 57) : new SKColor(232, 232, 232);
        SKColor titleBarBorderColor = DarkMode ? new SKColor(45, 44, 46) : new SKColor(208, 208, 208);

        using (SKPaint titlePaint = new() { IsAntialias = true, Color = titleBarColor, Style = SKPaintStyle.Fill })
        {
            canvas.DrawRect(windowX, windowY, windowWidth, TitleBarHeight, titlePaint);
        }

        // Title bar bottom border
        using (SKPaint borderPaint = new() { IsAntialias = true, Color = titleBarBorderColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1 })
        {
            canvas.DrawLine(windowX, windowY + TitleBarHeight, windowX + windowWidth, windowY + TitleBarHeight, borderPaint);
        }

        // Traffic light buttons
        float buttonY = windowY + TitleBarHeight / 2f;
        float buttonX = windowX + ButtonLeftMargin + ButtonRadius;

        DrawTrafficButton(canvas, buttonX, buttonY, new SKColor(255, 95, 87));                     // close (red)
        DrawTrafficButton(canvas, buttonX + ButtonSpacing, buttonY, new SKColor(255, 189, 46));    // minimize (yellow)
        DrawTrafficButton(canvas, buttonX + ButtonSpacing * 2, buttonY, new SKColor(39, 201, 63)); // maximize (green)

        // Draw title text
        if (!string.IsNullOrWhiteSpace(Title))
        {
            SKColor textColor = DarkMode ? new SKColor(220, 220, 220) : new SKColor(74, 74, 74);
            using SKTypeface titleTypeface = SKTypeface.FromFamilyName("San Francisco", SKFontStyleWeight.Medium, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
                                            ?? SKTypeface.FromFamilyName("Segoe UI", SKFontStyleWeight.Medium, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
                                            ?? SKTypeface.Default;
            using SKFont titleFont = new SKFont(titleTypeface, 13f);
            using SKPaint textPaint = new()
            {
                IsAntialias = true,
                Color = textColor
            };

            float textY = windowY + (TitleBarHeight / 2f) + (13f / 3f);
            canvas.DrawText(Title, windowX + windowWidth / 2f, textY, SKTextAlign.Center, titleFont, textPaint);
        }

        // Draw the source image below the title bar
        canvas.DrawBitmap(source, windowX, windowY + TitleBarHeight);

        canvas.Restore();

        // Draw window border on top
        using (SKPaint windowBorderPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = DarkMode ? new SKColor(70, 70, 70, 180) : new SKColor(180, 180, 180, 180)
        })
        {
            canvas.DrawRoundRect(windowRRect, windowBorderPaint);
        }

        return result;
    }

    private static void DrawTrafficButton(SKCanvas canvas, float x, float y, SKColor color)
    {
        using SKPaint fillPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = color
        };
        canvas.DrawCircle(x, y, ButtonRadius, fillPaint);

        using SKPaint strokePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 0.5f,
            Color = new SKColor(
                (byte)(color.Red * 0.8f),
                (byte)(color.Green * 0.8f),
                (byte)(color.Blue * 0.8f),
                200)
        };
        canvas.DrawCircle(x, y, ButtonRadius, strokePaint);
    }
}