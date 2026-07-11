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

public sealed class RopeBorderImageEffect : ImageEffectBase
{
    public override string Id => "rope_border";
    public override string Name => "Rope border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.link;
    public override string Description => "Adds a twisted rope/cord border with braided fiber detail and shading.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<RopeBorderImageEffect>("border_size", "Border size", 12, 200, 40, (e, v) => e.BorderSize = v),
        EffectParameters.Color<RopeBorderImageEffect>("rope_color", "Rope color", new SKColor(180, 145, 90), (e, v) => e.RopeColor = v),
        EffectParameters.Color<RopeBorderImageEffect>("bg_color", "Background color", new SKColor(60, 45, 30), (e, v) => e.BgColor = v),
        EffectParameters.FloatSlider<RopeBorderImageEffect>("twist_density", "Twist density", 10, 100, 50, (e, v) => e.TwistDensity = v),
        EffectParameters.FloatSlider<RopeBorderImageEffect>("fiber_detail", "Fiber detail", 0, 100, 55, (e, v) => e.FiberDetail = v)
    ];

    public int BorderSize { get; set; } = 40;
    public SKColor RopeColor { get; set; } = new SKColor(180, 145, 90);
    public SKColor BgColor { get; set; } = new SKColor(60, 45, 30);
    public float TwistDensity { get; set; } = 50f;
    public float FiberDetail { get; set; } = 55f;

    private const int Seed = 8271;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 12, 200);
        float twistFreq = Math.Clamp(TwistDensity, 10f, 100f) / 1000f;
        float fiber = Math.Clamp(FiberDetail, 0f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        float rR = RopeColor.Red / 255f;
        float rG = RopeColor.Green / 255f;
        float rB = RopeColor.Blue / 255f;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        float ropeRadius = border * 0.42f;

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

                // Determine position relative to rope center line
                float along, across;
                float ropeCenterCross = border * 0.5f;
                int boardSeed;

                if (topBand)
                {
                    along = x;
                    across = y - ropeCenterCross;
                    boardSeed = Seed ^ 0x1A;
                }
                else if (bottomBand)
                {
                    along = newWidth - x;
                    across = (y - (newHeight - border)) - (border - ropeCenterCross);
                    across = -across;
                    boardSeed = Seed ^ 0x2B;
                }
                else if (leftBand)
                {
                    along = newHeight - y;
                    across = x - ropeCenterCross;
                    boardSeed = Seed ^ 0x3C;
                }
                else
                {
                    along = y;
                    across = (x - (newWidth - border)) - (border - ropeCenterCross);
                    across = -across;
                    boardSeed = Seed ^ 0x4D;
                }

                // Twisted strand model: 3 strands twisted around each other
                float bestStrandDist = float.MaxValue;
                float bestStrandShade = 0f;

                for (int s = 0; s < 3; s++)
                {
                    float phase = s * (MathF.PI * 2f / 3f);
                    float strandOffset = MathF.Sin(along * twistFreq * MathF.PI * 2f + phase) * ropeRadius * 0.45f;
                    float strandDist = MathF.Abs(across - strandOffset);
                    float strandRadius = ropeRadius * 0.38f;

                    if (strandDist < strandRadius && strandDist < bestStrandDist)
                    {
                        bestStrandDist = strandDist;
                        // Cross-strand shading (rounded 3D feel)
                        float normalizedDist = strandDist / strandRadius;
                        bestStrandShade = 1f - normalizedDist * normalizedDist;
                    }
                }

                if (bestStrandDist < ropeRadius * 0.38f)
                {
                    // Rope fiber detail (fine noise along strand)
                    float fiberNoise = fiber > 0f
                        ? (ProceduralEffectHelper.Hash01((int)(along * 1.5f), (int)(across * 3f), boardSeed) * 2f - 1f) * fiber * 0.18f
                        : 0f;

                    // Coarse twist highlight variation
                    float twistHighlight = MathF.Cos(along * twistFreq * MathF.PI * 2f) * 0.08f;

                    float shade = bestStrandShade * 0.35f + twistHighlight + fiberNoise;

                    dstPixels[(y * newWidth) + x] = new SKColor(
                        ProceduralEffectHelper.ClampToByte((rR + shade) * 255f),
                        ProceduralEffectHelper.ClampToByte((rG + shade * 0.8f) * 255f),
                        ProceduralEffectHelper.ClampToByte((rB + shade * 0.5f) * 255f),
                        255);
                }
                else
                {
                    // Background
                    dstPixels[(y * newWidth) + x] = BgColor;
                }
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, border, border);

        return result;
    }
}