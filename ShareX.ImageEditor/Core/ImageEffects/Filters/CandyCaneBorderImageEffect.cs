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

public sealed class CandyCaneBorderImageEffect : ImageEffectBase
{
    public override string Id => "candy_cane_border";
    public override string Name => "Candy Cane Border";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.candy_cane;
    public override string Description => "Adds a candy cane striped border around the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<CandyCaneBorderImageEffect>("border_width", "Border Width", 1, 200, 10, (e, v) => e.BorderWidth = v),
        EffectParameters.FloatSlider<CandyCaneBorderImageEffect>("stripe_width", "Stripe Width", 2, 60, 12, (e, v) => e.StripeWidth = v),
        EffectParameters.Color<CandyCaneBorderImageEffect>("color_a", "Color A", new SKColor(220, 20, 20, 255), (e, v) => e.ColorA = v),
        EffectParameters.Color<CandyCaneBorderImageEffect>("color_b", "Color B", new SKColor(255, 255, 255, 255), (e, v) => e.ColorB = v),
        EffectParameters.FloatSlider<CandyCaneBorderImageEffect>("gloss", "Gloss", 0, 100, 50, (e, v) => e.Gloss = v),
        EffectParameters.Bool<CandyCaneBorderImageEffect>("rounded_corners", "Rounded Corners", true, (e, v) => e.RoundedCorners = v)
    ];

    public int BorderWidth { get; set; } = 32;
    public float StripeWidth { get; set; } = 12f;
    public SKColor ColorA { get; set; } = new SKColor(220, 20, 20, 255);
    public SKColor ColorB { get; set; } = new SKColor(255, 255, 255, 255);
    public float Gloss { get; set; } = 50f;
    public bool RoundedCorners { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int bw = Math.Clamp(BorderWidth, 1, 500);
        float sw = Math.Clamp(StripeWidth, 1f, 200f);
        float glossAmount = Math.Clamp(Gloss, 0f, 100f) / 100f;

        int newWidth = source.Width + bw * 2;
        int newHeight = source.Height + bw * 2;

        float aR = ColorA.Red / 255f, aG = ColorA.Green / 255f, aB = ColorA.Blue / 255f;
        float bR = ColorB.Red / 255f, bG = ColorB.Green / 255f, bB = ColorB.Blue / 255f;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        float cornerRadius = RoundedCorners ? bw * 0.4f : 0f;

        for (int y = 0; y < newHeight; y++)
        {
            bool inTop = y < bw;
            bool inBottom = y >= bw + source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                bool inLeft = x < bw;
                bool inRight = x >= bw + source.Width;

                if (!inTop && !inBottom && !inLeft && !inRight)
                    continue;

                // Use the closest point on the inner rectangle to compute a
                // consistent perimeter position and cross-axis ratio that
                // transitions smoothly through corners.

                // Inner rectangle bounds (the image area).
                float innerL = bw;
                float innerT = bw;
                float innerR = bw + source.Width - 1;
                float innerB = bw + source.Height - 1;

                // Closest point on the inner rect perimeter to (x, y).
                float clampedX = Math.Clamp((float)x, innerL, innerR);
                float clampedY = Math.Clamp((float)y, innerT, innerB);

                // Cross-axis: distance from pixel to its closest inner-edge point,
                // normalized by border width. 0 = inner edge, 1 = outer edge.
                float distToInner = MathF.Sqrt((x - clampedX) * (x - clampedX) + (y - clampedY) * (y - clampedY));
                float crossRatio = Math.Clamp(distToInner / bw, 0f, 1f);

                // Along-axis: perimeter position of the closest inner-edge point,
                // measured clockwise from the top-left corner.
                // Top side:    0 .. W
                // Right side:  W .. W+H
                // Bottom side: W+H .. 2W+H
                // Left side:   2W+H .. 2W+2H
                float imgW = source.Width;
                float imgH = source.Height;
                float along;

                if (clampedY == innerT && clampedX > innerL)
                {
                    // Top edge
                    along = clampedX - innerL;
                }
                else if (clampedX == innerR && clampedY > innerT)
                {
                    // Right edge (excluding top-right corner point)
                    along = imgW + (clampedY - innerT);
                }
                else if (clampedY == innerB && clampedX < innerR)
                {
                    // Bottom edge
                    along = imgW + imgH + (innerR - clampedX);
                }
                else if (clampedX == innerL && clampedY < innerB)
                {
                    // Left edge
                    along = imgW * 2f + imgH + (innerB - clampedY);
                }
                else if (clampedX == innerR && clampedY == innerT)
                {
                    // Top-right corner point
                    along = imgW;
                }
                else if (clampedX == innerR && clampedY == innerB)
                {
                    // Bottom-right corner point
                    along = imgW + imgH;
                }
                else if (clampedX == innerL && clampedY == innerB)
                {
                    // Bottom-left corner point
                    along = imgW * 2f + imgH;
                }
                else
                {
                    // Top-left corner point
                    along = 0f;
                }

                // Rounded corner masking.
                if (RoundedCorners && cornerRadius > 0)
                {
                    bool inCornerX = (x < cornerRadius) || (x > newWidth - 1 - cornerRadius);
                    bool inCornerY = (y < cornerRadius) || (y > newHeight - 1 - cornerRadius);

                    if (inCornerX && inCornerY)
                    {
                        float outerCx = x < cornerRadius ? cornerRadius : newWidth - 1 - cornerRadius;
                        float outerCy = y < cornerRadius ? cornerRadius : newHeight - 1 - cornerRadius;
                        float distToOuter = MathF.Sqrt((x - outerCx) * (x - outerCx) + (y - outerCy) * (y - outerCy));

                        if (distToOuter > cornerRadius)
                        {
                            dstPixels[y * newWidth + x] = SKColors.Transparent;
                            continue;
                        }
                    }
                }

                // Diagonal stripe pattern.
                float stripePhase = (along + crossRatio * bw) / sw;
                float stripeT = (MathF.Sin(stripePhase * MathF.PI * 2f) + 1f) * 0.5f;

                // Smoothstep to get sharper stripes.
                stripeT = stripeT < 0.5f
                    ? ProceduralEffectHelper.SmoothStep(0.15f, 0.45f, stripeT)
                    : 1f - ProceduralEffectHelper.SmoothStep(0.55f, 0.85f, 1f - stripeT);

                float r = ProceduralEffectHelper.Lerp(aR, bR, stripeT);
                float g = ProceduralEffectHelper.Lerp(aG, bG, stripeT);
                float b = ProceduralEffectHelper.Lerp(aB, bB, stripeT);

                // Cylindrical gloss: bright highlight near center, darker at edges.
                if (glossAmount > 0.001f)
                {
                    float centered = (crossRatio - 0.5f) * 2f; // -1 to 1
                    float glossHighlight = MathF.Exp(-3f * centered * centered);
                    float darkEdge = 1f - 0.15f * (1f - glossHighlight);
                    float combinedGloss = darkEdge + glossHighlight * 0.25f * glossAmount;

                    r = Math.Clamp(r * combinedGloss, 0f, 1f);
                    g = Math.Clamp(g * combinedGloss, 0f, 1f);
                    b = Math.Clamp(b * combinedGloss, 0f, 1f);
                }

                dstPixels[y * newWidth + x] = new SKColor(
                    (byte)MathF.Round(r * 255f),
                    (byte)MathF.Round(g * 255f),
                    (byte)MathF.Round(b * 255f),
                    255);
            }
        }

        SKBitmap result = new(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new(result);
        canvas.DrawBitmap(source, bw, bw);

        return result;
    }
}