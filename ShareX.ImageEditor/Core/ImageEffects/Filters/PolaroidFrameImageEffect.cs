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

public sealed class PolaroidFrameImageEffect : ImageEffectBase
{
    public override string Id => "polaroid_frame";
    public override string Name => "Polaroid frame";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.camera;
    public override string Description => "Wraps the image in a Polaroid-style instant photo frame.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<PolaroidFrameImageEffect>("border_size", "Border size", 5, 100, 30, (e, v) => e.BorderSize = v),
        EffectParameters.IntSlider<PolaroidFrameImageEffect>("bottom_size", "Bottom size", 20, 200, 80, (e, v) => e.BottomSize = v),
        EffectParameters.FloatSlider<PolaroidFrameImageEffect>("rotation", "Rotation", -30, 30, 0, (e, v) => e.Rotation = v),
        EffectParameters.Color<PolaroidFrameImageEffect>("frame_color", "Frame color", new SKColor(250, 249, 245), (e, v) => e.FrameColor = v),
        EffectParameters.Text<PolaroidFrameImageEffect>("caption", "Caption", string.Empty, (e, v) => e.Caption = v),
        EffectParameters.IntSlider<PolaroidFrameImageEffect>("shadow_size", "Shadow size", 0, 60, 15, (e, v) => e.ShadowSize = v)
    ];

    public int BorderSize { get; set; } = 30;
    public int BottomSize { get; set; } = 80;
    public float Rotation { get; set; }
    public SKColor FrameColor { get; set; } = new SKColor(250, 249, 245);
    public string Caption { get; set; } = string.Empty;
    public int ShadowSize { get; set; } = 15;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 5, 100);
        int bottom = Math.Clamp(BottomSize, 20, 200);
        int shadowSize = Math.Clamp(ShadowSize, 0, 60);
        float rotation = Math.Clamp(Rotation, -30f, 30f);

        int polaroidWidth = source.Width + border * 2;
        int polaroidHeight = source.Height + border + bottom;

        // Account for rotation when computing canvas size
        float radians = MathF.Abs(rotation) * MathF.PI / 180f;
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);
        int rotatedWidth = (int)MathF.Ceiling(polaroidWidth * cos + polaroidHeight * sin);
        int rotatedHeight = (int)MathF.Ceiling(polaroidWidth * sin + polaroidHeight * cos);

        int canvasWidth = Math.Max(rotatedWidth, polaroidWidth) + shadowSize * 2 + 20;
        int canvasHeight = Math.Max(rotatedHeight, polaroidHeight) + shadowSize * 2 + 20;

        SKBitmap result = new(canvasWidth, canvasHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        float cx = canvasWidth / 2f;
        float cy = canvasHeight / 2f;

        canvas.Save();
        canvas.Translate(cx, cy);
        canvas.RotateDegrees(rotation);
        canvas.Translate(-polaroidWidth / 2f, -polaroidHeight / 2f);

        // Draw shadow
        if (shadowSize > 0)
        {
            using SKPaint shadowPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 50),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, shadowSize / 2f)
            };
            canvas.DrawRect(0, 0, polaroidWidth, polaroidHeight, shadowPaint);
        }

        // Draw frame
        using (SKPaint framePaint = new() { IsAntialias = true, Color = FrameColor, Style = SKPaintStyle.Fill })
        {
            canvas.DrawRect(0, 0, polaroidWidth, polaroidHeight, framePaint);
        }

        // Subtle frame edge shadow for realism
        using (SKPaint edgePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = new SKColor(200, 198, 190)
        })
        {
            canvas.DrawRect(0, 0, polaroidWidth, polaroidHeight, edgePaint);
        }

        // Draw the image
        canvas.DrawBitmap(source, border, border);

        // Subtle inner shadow on the photo area
        using (SKPaint innerShadow = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = new SKColor(180, 175, 165, 100)
        })
        {
            canvas.DrawRect(border, border, source.Width, source.Height, innerShadow);
        }

        // Draw caption
        if (!string.IsNullOrWhiteSpace(Caption))
        {
            float fontSize = Math.Max(12, bottom * 0.3f);
            using SKTypeface captionTypeface = SKTypeface.FromFamilyName("Segoe Script", SKFontStyle.Normal)
                                               ?? SKTypeface.FromFamilyName("Comic Sans MS", SKFontStyle.Normal)
                                               ?? SKTypeface.Default;
            using SKFont captionFont = new SKFont(captionTypeface, fontSize);
            using SKPaint textPaint = new()
            {
                IsAntialias = true,
                Color = new SKColor(60, 55, 50)
            };

            float textX = polaroidWidth / 2f;
            float textY = source.Height + border + (bottom / 2f) + (fontSize / 3f);
            canvas.DrawText(Caption, textX, textY, SKTextAlign.Center, captionFont, textPaint);
        }

        canvas.Restore();
        return result;
    }
}