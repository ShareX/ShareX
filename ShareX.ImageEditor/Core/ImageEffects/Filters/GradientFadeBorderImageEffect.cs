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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class GradientFadeBorderImageEffect : ImageEffectBase
{
    public override string Id => "gradient_fade_border";
    public override string Name => "Gradient fade border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.blend;
    public override string Description => "Adds a multi-stop gradient border that smoothly fades between configurable colors.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<GradientFadeBorderImageEffect>("border_size", "Border size", 10, 300, 50, (e, v) => e.BorderSize = v),
        EffectParameters.Color<GradientFadeBorderImageEffect>("outer_color", "Outer color", new SKColor(10, 10, 35), (e, v) => e.OuterColor = v),
        EffectParameters.Color<GradientFadeBorderImageEffect>("mid_color", "Mid color", new SKColor(60, 20, 120), (e, v) => e.MidColor = v),
        EffectParameters.Color<GradientFadeBorderImageEffect>("inner_color", "Inner color", new SKColor(180, 80, 200), (e, v) => e.InnerColor = v),
        EffectParameters.FloatSlider<GradientFadeBorderImageEffect>("mid_position", "Mid-color position %", 10, 90, 50, (e, v) => e.MidPosition = v),
        EffectParameters.Bool<GradientFadeBorderImageEffect>("glow_edge", "Inner glow edge", true, (e, v) => e.GlowEdge = v)
    ];

    public int BorderSize { get; set; } = 50;
    public SKColor OuterColor { get; set; } = new SKColor(10, 10, 35);
    public SKColor MidColor { get; set; } = new SKColor(60, 20, 120);
    public SKColor InnerColor { get; set; } = new SKColor(180, 80, 200);
    public float MidPosition { get; set; } = 50f;
    public bool GlowEdge { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 300);
        float midT = Math.Clamp(MidPosition, 10f, 90f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        for (int y = 0; y < newHeight; y++)
        {
            bool topBand = y < border;
            bool bottomBand = y >= border + source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                bool leftBand = x < border;
                bool rightBand = x >= border + source.Width;

                if (!topBand && !bottomBand && !leftBand && !rightBand)
                    continue;

                // Distance from nearest edge of the image area (0 = at image, border = at outer edge)
                int distLeft = leftBand ? (border - x) : 0;
                int distRight = rightBand ? (x - border - source.Width + 1) : 0;
                int distTop = topBand ? (border - y) : 0;
                int distBottom = bottomBand ? (y - border - source.Height + 1) : 0;
                int dist = Math.Max(Math.Max(distLeft, distRight), Math.Max(distTop, distBottom));

                // t: 0 = inner edge (near image), 1 = outer edge
                float t = Math.Clamp(dist / (float)border, 0f, 1f);

                // Two segment interpolation: inner->mid->outer
                SKColor color;
                if (t < midT)
                {
                    float seg = t / midT;
                    seg = ProceduralEffectHelper.SmoothStep(0f, 1f, seg);
                    color = LerpColor(InnerColor, MidColor, seg);
                }
                else
                {
                    float seg = (t - midT) / (1f - midT);
                    seg = ProceduralEffectHelper.SmoothStep(0f, 1f, seg);
                    color = LerpColor(MidColor, OuterColor, seg);
                }

                dstPixels[(y * newWidth) + x] = color;
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, border, border);

        if (GlowEdge)
        {
            using SKPaint glowPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3f,
                Color = new SKColor(InnerColor.Red, InnerColor.Green, InnerColor.Blue, 120),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 4f)
            };
            canvas.DrawRect(border, border, source.Width, source.Height, glowPaint);
        }

        return result;
    }

    private static SKColor LerpColor(SKColor a, SKColor b, float t)
    {
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(a.Red + (b.Red - a.Red) * t),
            ProceduralEffectHelper.ClampToByte(a.Green + (b.Green - a.Green) * t),
            ProceduralEffectHelper.ClampToByte(a.Blue + (b.Blue - a.Blue) * t),
            ProceduralEffectHelper.ClampToByte(a.Alpha + (b.Alpha - a.Alpha) * t));
    }
}