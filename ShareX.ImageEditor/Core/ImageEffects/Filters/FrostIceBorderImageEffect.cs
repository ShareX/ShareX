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

public sealed class FrostIceBorderImageEffect : ImageEffectBase
{
    public override string Id => "frost_ice_border";
    public override string Name => "Frost ice border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.snowflake;
    public override string Description => "Adds a frosted ice crystal border with rime buildup and icy blue highlights.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<FrostIceBorderImageEffect>("border_size", "Border size", 10, 300, 55, (e, v) => e.BorderSize = v),
        EffectParameters.Color<FrostIceBorderImageEffect>("ice_color", "Ice color", new SKColor(200, 225, 255), (e, v) => e.IceColor = v),
        EffectParameters.FloatSlider<FrostIceBorderImageEffect>("crystallization", "Crystallization", 0, 100, 65, (e, v) => e.Crystallization = v),
        EffectParameters.FloatSlider<FrostIceBorderImageEffect>("transparency", "Transparency", 0, 80, 25, (e, v) => e.Transparency = v),
        EffectParameters.Bool<FrostIceBorderImageEffect>("sparkle", "Ice sparkle", true, (e, v) => e.Sparkle = v)
    ];

    public int BorderSize { get; set; } = 55;
    public SKColor IceColor { get; set; } = new SKColor(200, 225, 255);
    public float Crystallization { get; set; } = 65f;
    public float Transparency { get; set; } = 25f;
    public bool Sparkle { get; set; } = true;

    private const int Seed = 4519;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 300);
        float crystal = Math.Clamp(Crystallization, 0f, 100f) / 100f;
        float transparency = Math.Clamp(Transparency, 0f, 80f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        float iceR = IceColor.Red / 255f;
        float iceG = IceColor.Green / 255f;
        float iceB = IceColor.Blue / 255f;

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

                // Cross ratio: distance from inner edge (0=inner, 1=outer)
                float crossRatio = ComputeCrossRatio(x, y, border, newWidth, newHeight, source.Width, source.Height);

                // Ice buildup: thicker near the edge, thin transition near image
                float iceThickness = ProceduralEffectHelper.SmoothStep(0f, 0.5f, crossRatio);

                // Crystal noise (sharp, angular)
                float coarseNoise = ProceduralEffectHelper.FractalNoise(x * 0.02f, y * 0.02f, 4, 2.5f, 0.45f, Seed);
                float fineNoise = ProceduralEffectHelper.FractalNoise(x * 0.08f, y * 0.08f, 3, 2.0f, 0.5f, Seed ^ 0x3F);

                // Sharp crystal pattern: threshold the noise
                float crystalPattern = coarseNoise + fineNoise * 0.5f;
                float crystallized = crystal > 0f
                    ? MathF.Pow(Math.Clamp(crystalPattern, 0f, 1f), 1f - crystal * 0.7f)
                    : crystalPattern;

                // Color: white highlights, blue base
                float highlight = ProceduralEffectHelper.SmoothStep(0.55f, 0.75f, crystallized);

                float r = ProceduralEffectHelper.Lerp(iceR, 1f, highlight * 0.6f);
                float g = ProceduralEffectHelper.Lerp(iceG, 1f, highlight * 0.5f);
                float b = ProceduralEffectHelper.Lerp(iceB, 1f, highlight * 0.35f);

                // Darker veins in deeper parts
                float vein = ProceduralEffectHelper.SmoothStep(0.2f, 0.35f, crystalPattern);
                r -= vein * 0.12f;
                g -= vein * 0.06f;

                // Alpha diminishes near inner edge for icy fade effect
                float alpha = ProceduralEffectHelper.Lerp(1f - transparency, 1f, iceThickness);
                alpha *= (0.85f + crystallized * 0.15f);
                alpha = Math.Clamp(alpha, 0f, 1f);

                dstPixels[(y * newWidth) + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    ProceduralEffectHelper.ClampToByte(alpha * 255f));
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, border, border);

        // Ice sparkle dots
        if (Sparkle)
        {
            DrawSparkles(canvas, newWidth, newHeight, border, source.Width, source.Height);
        }

        return result;
    }

    private static void DrawSparkles(SKCanvas canvas, int newWidth, int newHeight, int border, int srcW, int srcH)
    {
        using SKPaint sparklePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, 200)
        };

        int sparkleCount = (newWidth + newHeight) / 12;
        for (int i = 0; i < sparkleCount; i++)
        {
            float hx = ProceduralEffectHelper.Hash01(i, Seed ^ 0xAA, Seed);
            float hy = ProceduralEffectHelper.Hash01(i, Seed ^ 0xBB, Seed ^ 0x11);
            float hs = ProceduralEffectHelper.Hash01(i, Seed ^ 0xCC, Seed ^ 0x22);

            int sx = (int)(hx * newWidth);
            int sy = (int)(hy * newHeight);

            // Only place in border area
            bool inBorder = sx < border || sx >= border + srcW || sy < border || sy >= border + srcH;
            if (!inBorder) continue;

            float radius = 0.8f + hs * 2.2f;
            sparklePaint.Color = new SKColor(255, 255, 255, (byte)(140 + hs * 115));
            canvas.DrawCircle(sx, sy, radius, sparklePaint);

            // Tiny cross-star
            if (hs > 0.5f)
            {
                float starLen = radius * 2.5f;
                using SKPaint starPaint = new()
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 0.5f,
                    Color = new SKColor(255, 255, 255, (byte)(80 + hs * 80))
                };
                canvas.DrawLine(sx - starLen, sy, sx + starLen, sy, starPaint);
                canvas.DrawLine(sx, sy - starLen, sx, sy + starLen, starPaint);
            }
        }
    }

    private static float ComputeCrossRatio(int x, int y, int border, int newW, int newH, int srcW, int srcH)
    {
        float innerL = border;
        float innerT = border;
        float innerR = border + srcW;
        float innerB = border + srcH;

        float clampX = Math.Clamp((float)x, innerL, innerR - 1);
        float clampY = Math.Clamp((float)y, innerT, innerB - 1);

        float dist = MathF.Sqrt((x - clampX) * (x - clampX) + (y - clampY) * (y - clampY));
        return Math.Clamp(dist / border, 0f, 1f);
    }
}