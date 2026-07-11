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

public sealed class MarbleBorderImageEffect : ImageEffectBase
{
    public override string Id => "marble_border";
    public override string Name => "Marble border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.gem;
    public override string Description => "Adds a procedural marble-veined texture border around the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<MarbleBorderImageEffect>("border_size", "Border size", 2, 300, 50, (e, v) => e.BorderSize = v),
        EffectParameters.Color<MarbleBorderImageEffect>("base_color", "Base color", new SKColor(235, 230, 220), (e, v) => e.BaseColor = v),
        EffectParameters.Color<MarbleBorderImageEffect>("vein_color", "Vein color", new SKColor(90, 80, 70), (e, v) => e.VeinColor = v),
        EffectParameters.FloatSlider<MarbleBorderImageEffect>("vein_strength", "Vein strength", 0, 100, 60, (e, v) => e.VeinStrength = v),
        EffectParameters.FloatSlider<MarbleBorderImageEffect>("vein_scale", "Vein scale", 5, 100, 30, (e, v) => e.VeinScale = v),
        EffectParameters.Bool<MarbleBorderImageEffect>("inner_bevel", "Inner bevel", true, (e, v) => e.InnerBevel = v)
    ];

    public int BorderSize { get; set; } = 50;
    public SKColor BaseColor { get; set; } = new SKColor(235, 230, 220);
    public SKColor VeinColor { get; set; } = new SKColor(90, 80, 70);
    public float VeinStrength { get; set; } = 60f;
    public float VeinScale { get; set; } = 30f;
    public bool InnerBevel { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 2, 300);
        float veinStr = Math.Clamp(VeinStrength, 0f, 100f) / 100f;
        float scale = Math.Clamp(VeinScale, 5f, 100f) / 1000f;

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
                {
                    continue; // inner image pixel, skip
                }

                SKColor marble = ComputeMarbleColor(x, y, scale, veinStr, BaseColor, VeinColor);

                // Bevel shading: inner edge (near source image) gets slightly darkened
                if (InnerBevel)
                {
                    float bevelDepth = ComputeInnerBevelShade(x, y, border, newWidth, newHeight, source.Width, source.Height);
                    if (bevelDepth > 0f)
                    {
                        marble = ShadeColor(marble, -bevelDepth * 0.25f);
                    }
                }

                dstPixels[(y * newWidth) + x] = marble;
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, border, border);

        // Inner edge line
        using SKPaint linePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f,
            Color = ShadeColor(VeinColor, -0.3f)
        };
        canvas.DrawRect(border - 0.75f, border - 0.75f, source.Width + 1.5f, source.Height + 1.5f, linePaint);

        return result;
    }

    private static SKColor ComputeMarbleColor(int x, int y, float scale, float veinStr, SKColor baseColor, SKColor veinColor)
    {
        // Classic marble: turbulence perturbs a sine wave along a diagonal
        float turbulence = ProceduralEffectHelper.FractalNoise(x * scale, y * scale, 6, 2.0f, 0.5f, 42);

        // Marble value: sine of (diagonal position + turbulence)
        float diag = (x + y) * scale * 2f;
        float marble = MathF.Sin(diag + veinStr * turbulence * 20f);
        marble = (marble + 1f) * 0.5f; // 0..1

        // Smooth the veins (emphasize darker streaks)
        marble = MathF.Pow(marble, 1.5f);

        // Blend base and vein color
        float r = Lerp(baseColor.Red, veinColor.Red, marble);
        float g = Lerp(baseColor.Green, veinColor.Green, marble);
        float b = Lerp(baseColor.Blue, veinColor.Blue, marble);

        return new SKColor(
            ProceduralEffectHelper.ClampToByte(r),
            ProceduralEffectHelper.ClampToByte(g),
            ProceduralEffectHelper.ClampToByte(b),
            255);
    }

    private static float ComputeInnerBevelShade(int x, int y, int border, int newWidth, int newHeight, int srcW, int srcH)
    {
        // Distance from the inner edge (image boundary), normalized 0..1 over bevel zone
        float bevelZone = border * 0.4f;
        float innerL = border;
        float innerT = border;
        float innerR = border + srcW;
        float innerB = border + srcH;

        float dFromInner = float.MaxValue;
        if (x < innerL) dFromInner = Math.Min(dFromInner, innerL - x);
        if (x >= innerR) dFromInner = Math.Min(dFromInner, x - (innerR - 1));
        if (y < innerT) dFromInner = Math.Min(dFromInner, innerT - y);
        if (y >= innerB) dFromInner = Math.Min(dFromInner, y - (innerB - 1));

        // Closest to inner edge → highest bevel value
        float t = 1f - Math.Clamp(dFromInner / bevelZone, 0f, 1f);
        return ProceduralEffectHelper.SmoothStep(0f, 1f, t);
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static SKColor ShadeColor(SKColor c, float amount)
    {
        float r = c.Red / 255f + amount;
        float g = c.Green / 255f + amount;
        float b = c.Blue / 255f + amount;
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(r * 255f),
            ProceduralEffectHelper.ClampToByte(g * 255f),
            ProceduralEffectHelper.ClampToByte(b * 255f),
            c.Alpha);
    }
}