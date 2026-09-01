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
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class StainedGlassBorderImageEffect : ImageEffectBase
{
    public override string Id => "stained_glass_border";
    public override string Name => "Stained glass border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.palette;
    public override string Description => "Adds a stained glass border with jewel-toned glass panels divided by dark lead came lines.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<StainedGlassBorderImageEffect>("border_size", "Border size", 20, 300, 60, (e, v) => e.BorderSize = v),
        EffectParameters.FloatSlider<StainedGlassBorderImageEffect>("panel_width", "Panel width", 10, 120, 40, (e, v) => e.PanelWidth = v),
        EffectParameters.FloatSlider<StainedGlassBorderImageEffect>("lead_thickness", "Lead thickness", 1, 12, 3.5f, (e, v) => e.LeadThickness = v),
        EffectParameters.FloatSlider<StainedGlassBorderImageEffect>("glass_brightness", "Glass brightness", 30, 100, 75, (e, v) => e.GlassBrightness = v),
        EffectParameters.Bool<StainedGlassBorderImageEffect>("glow", "Glass glow", true, (e, v) => e.Glow = v),
        EffectParameters.Bool<StainedGlassBorderImageEffect>("inner_line", "Inner line", true, (e, v) => e.InnerLine = v)
    ];

    public int BorderSize { get; set; } = 60;
    public float PanelWidth { get; set; } = 40f;
    public float LeadThickness { get; set; } = 3.5f;
    public float GlassBrightness { get; set; } = 75f;
    public bool Glow { get; set; } = true;
    public bool InnerLine { get; set; } = true;

    // Classic jewel-tone palette for stained glass
    private static readonly float[] JewelHues = [0f, 30f, 60f, 120f, 180f, 210f, 270f, 300f, 340f];

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 20, 300);
        float panelW = Math.Clamp(PanelWidth, 10f, 120f);
        float leadT = Math.Clamp(LeadThickness, 1f, 12f);
        float brightness = Math.Clamp(GlassBrightness, 30f, 100f);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Fill border bands with dark lead color first
        SKColor leadColor = new SKColor(14, 10, 8);
        using SKPaint leadFill = new() { Color = leadColor, Style = SKPaintStyle.Fill, IsAntialias = false };
        canvas.DrawRect(0, 0, newWidth, border, leadFill);
        canvas.DrawRect(0, newHeight - border, newWidth, border, leadFill);
        canvas.DrawRect(0, border, border, source.Height, leadFill);
        canvas.DrawRect(newWidth - border, border, border, source.Height, leadFill);

        // Draw glass panels on each side
        DrawGlassPanels(canvas, 0, 0, newWidth, border, true, panelW, leadT, brightness, 0x1A);
        DrawGlassPanels(canvas, 0, newHeight - border, newWidth, border, true, panelW, leadT, brightness, 0x2B);
        DrawGlassPanels(canvas, 0, border, border, source.Height, false, panelW, leadT, brightness, 0x3C);
        DrawGlassPanels(canvas, newWidth - border, border, border, source.Height, false, panelW, leadT, brightness, 0x4D);

        // Source image
        canvas.DrawBitmap(source, border, border);

        // Inner edge - thick lead line
        if (InnerLine)
        {
            using SKPaint innerLead = new()
            {
                IsAntialias = false,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = leadT * 1.5f,
                Color = leadColor
            };
            canvas.DrawRect(border, border, source.Width, source.Height, innerLead);
        }

        return result;
    }

    private void DrawGlassPanels(SKCanvas canvas, float rx, float ry, float rw, float rh,
        bool horizontal, float panelW, float leadT, float brightness, int seed)
    {
        float primaryLen = horizontal ? rw : rh;
        float crossLen = horizontal ? rh : rw;

        // Draw rows of panels across the border depth
        // Each row is one panel tall, panels repeat along the primary axis
        float numCrossRows = Math.Max(1f, crossLen / panelW);
        float actualPanelH = crossLen / numCrossRows;

        canvas.Save();
        canvas.ClipRect(new SKRect(rx, ry, rx + rw, ry + rh));

        int panelIndex = 0;

        for (float cr = 0; cr < numCrossRows; cr++)
        {
            float crossStart = cr * actualPanelH;
            float crossEnd = crossStart + actualPanelH;

            float along = 0f;
            while (along < primaryLen)
            {
                // Vary panel width slightly per panel
                float widVar = ProceduralEffectHelper.Hash01(panelIndex, seed, seed ^ 0x7F);
                float thisPanelLen = panelW * (0.65f + widVar * 0.75f);
                float panelEnd = Math.Min(along + thisPanelLen, primaryLen);

                // Pick a jewel hue for this panel
                float hueHash = ProceduralEffectHelper.Hash01(panelIndex ^ (panelIndex * 7), seed ^ 0x33, seed);
                int hueIdx = (int)(hueHash * JewelHues.Length) % JewelHues.Length;
                float hue = JewelHues[hueIdx];

                // Saturation: high for stained glass
                float saturation = 70f + ProceduralEffectHelper.Hash01(panelIndex, seed ^ 0x55, seed) * 30f;

                // Build panel rect in local (horizontal) coords
                float px1, py1, px2, py2;
                if (horizontal)
                {
                    px1 = rx + along + leadT * 0.5f;
                    py1 = ry + crossStart + leadT * 0.5f;
                    px2 = rx + panelEnd - leadT * 0.5f;
                    py2 = ry + crossEnd - leadT * 0.5f;
                }
                else
                {
                    px1 = rx + crossStart + leadT * 0.5f;
                    py1 = ry + along + leadT * 0.5f;
                    px2 = rx + crossEnd - leadT * 0.5f;
                    py2 = ry + panelEnd - leadT * 0.5f;
                }

                if (px2 > px1 + 1f && py2 > py1 + 1f)
                {
                    // Base glass color
                    SKColor glassColor = SKColor.FromHsv(hue, saturation, brightness);

                    // Glass fill with internal gradient for translucency effect
                    DrawGlassPanel(canvas, px1, py1, px2, py2, glassColor, leadT, Glow);
                }

                along = panelEnd;
                panelIndex++;
            }
        }

        canvas.Restore();
    }

    private static void DrawGlassPanel(SKCanvas canvas, float x1, float y1, float x2, float y2,
        SKColor baseColor, float leadT, bool glow)
    {
        float w = x2 - x1;
        float h = y2 - y1;

        if (w < 1f || h < 1f) return;

        // Glow bloom layer behind glass
        if (glow)
        {
            using SKPaint glowPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = baseColor.WithAlpha(60),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, leadT * 1.8f)
            };
            canvas.DrawRect(x1, y1, w, h, glowPaint);
        }

        // Main glass fill with slight gradient (light from top-left)
        SKColor lightColor = LightenColor(baseColor, 0.25f).WithAlpha(245);
        SKColor darkColor = DarkenColor(baseColor, 0.15f).WithAlpha(235);

        using SKPaint glassPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateLinearGradient(
                new SKPoint(x1, y1),
                new SKPoint(x2, y2),
                [lightColor, darkColor],
                SKShaderTileMode.Clamp)
        };
        canvas.DrawRect(x1, y1, w, h, glassPaint);

        // Specular highlight in top-left corner
        using SKPaint specPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = SKShader.CreateRadialGradient(
                new SKPoint(x1 + w * 0.2f, y1 + h * 0.2f),
                Math.Min(w, h) * 0.4f,
                [new SKColor(255, 255, 255, 90), SKColors.Transparent],
                SKShaderTileMode.Clamp)
        };
        canvas.DrawRect(x1, y1, w, h, specPaint);
    }

    private static SKColor LightenColor(SKColor c, float amount)
    {
        return new SKColor(
            (byte)Math.Min(255, c.Red + (int)(amount * 255)),
            (byte)Math.Min(255, c.Green + (int)(amount * 255)),
            (byte)Math.Min(255, c.Blue + (int)(amount * 255)),
            c.Alpha);
    }

    private static SKColor DarkenColor(SKColor c, float amount)
    {
        return new SKColor(
            (byte)Math.Max(0, c.Red - (int)(amount * 255)),
            (byte)Math.Max(0, c.Green - (int)(amount * 255)),
            (byte)Math.Max(0, c.Blue - (int)(amount * 255)),
            c.Alpha);
    }
}