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

public sealed class WoodenFrameImageEffect : ImageEffectBase
{
    private enum FrameSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public override string Id => "wooden_frame";
    public override string Name => "Wooden frame";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.frame;
    public override string Description => "Adds a procedural wooden frame around the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<WoodenFrameImageEffect>("frame_width", "Frame width", 2, 300, 48, (e, v) => e.FrameWidth = v),
        EffectParameters.FloatSlider<WoodenFrameImageEffect>("grain_strength", "Grain strength", 0, 100, 60, (e, v) => e.GrainStrength = v),
        EffectParameters.FloatSlider<WoodenFrameImageEffect>("bevel_strength", "Bevel strength", 0, 100, 65, (e, v) => e.BevelStrength = v),
        EffectParameters.Color<WoodenFrameImageEffect>("wood_color", "Wood color", new SKColor(139, 94, 60), (e, v) => e.WoodColor = v)
    ];

    public int FrameWidth { get; set; } = 48;
    public float GrainStrength { get; set; } = 60f; // 0..100
    public float BevelStrength { get; set; } = 65f; // 0..100
    public SKColor WoodColor { get; set; } = new SKColor(139, 94, 60);
    public int Seed { get; set; } = 1649;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        int frameWidth = Math.Clamp(FrameWidth, 2, 300);
        float grainStrength = Math.Clamp(GrainStrength, 0f, 100f) / 100f;
        float bevelStrength = Math.Clamp(BevelStrength, 0f, 100f) / 100f;

        int newWidth = source.Width + (frameWidth * 2);
        int newHeight = source.Height + (frameWidth * 2);

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        for (int y = 0; y < newHeight; y++)
        {
            bool topBand = y < frameWidth;
            bool bottomBand = y >= frameWidth + source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                bool leftBand = x < frameWidth;
                bool rightBand = x >= frameWidth + source.Width;

                if (!topBand && !bottomBand && !leftBand && !rightBand)
                {
                    continue;
                }

                FrameSide side;
                float along;
                float across;
                float outerDistance;
                float innerDistance;
                int boardSeed;

                if (topBand)
                {
                    side = FrameSide.Top;
                    along = x;
                    across = y;
                    outerDistance = y;
                    innerDistance = (frameWidth - 1) - y;
                    boardSeed = Seed ^ 0x1C3;
                }
                else if (bottomBand)
                {
                    side = FrameSide.Bottom;
                    along = x;
                    across = y - (frameWidth + source.Height);
                    outerDistance = (newHeight - 1) - y;
                    innerDistance = y - (frameWidth + source.Height);
                    boardSeed = Seed ^ 0x29B;
                }
                else if (leftBand)
                {
                    side = FrameSide.Left;
                    along = y;
                    across = x;
                    outerDistance = x;
                    innerDistance = (frameWidth - 1) - x;
                    boardSeed = Seed ^ 0x35F;
                }
                else
                {
                    side = FrameSide.Right;
                    along = y;
                    across = x - (frameWidth + source.Width);
                    outerDistance = (newWidth - 1) - x;
                    innerDistance = x - (frameWidth + source.Width);
                    boardSeed = Seed ^ 0x41D;
                }

                float span = MathF.Max(1f, frameWidth - 1f);
                float acrossRatio = across / span;
                float boardVariation = ((ProceduralEffectHelper.Hash01(boardSeed, frameWidth, Seed) * 2f) - 1f) * 0.06f;
                float grain = ComputeGrain(along, across, grainStrength, boardSeed);
                float shade = boardVariation + ComputeBevelShade(side, acrossRatio, outerDistance, innerDistance, frameWidth, bevelStrength);
                shade -= ComputeMiterSeamShade(x, y, newWidth, newHeight, frameWidth);

                float red = WoodColor.Red * (1f + grain + shade + 0.03f);
                float green = WoodColor.Green * (1f + (grain * 0.82f) + shade);
                float blue = WoodColor.Blue * (1f + (grain * 0.48f) + shade - 0.03f);

                int index = (y * newWidth) + x;
                dstPixels[index] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(red),
                    ProceduralEffectHelper.ClampToByte(green),
                    ProceduralEffectHelper.ClampToByte(blue),
                    255);
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, frameWidth, frameWidth);

        return result;
    }

    private static float ComputeGrain(float along, float across, float grainStrength, int seed)
    {
        float coarseNoise = (ProceduralEffectHelper.Hash01((int)(along * 0.20f), (int)(across * 0.08f), seed) * 2f) - 1f;
        float mediumNoise = (ProceduralEffectHelper.Hash01((int)(along * 0.55f), (int)(across * 0.16f), seed ^ 0x57) * 2f) - 1f;
        float fineNoise = (ProceduralEffectHelper.Hash01((int)(along * 1.30f), (int)(across * 0.42f), seed ^ 0xA1) * 2f) - 1f;

        float wave1 = MathF.Sin((along * 0.075f) + (coarseNoise * 5.6f) + (across * 0.028f));
        float wave2 = MathF.Sin((along * 0.19f) - (mediumNoise * 3.1f) + (across * 0.012f));
        float ridge = MathF.Sign(wave1) * MathF.Pow(MathF.Abs(wave1), 1.75f);
        float pore = fineNoise * 0.12f;
        float crossTint = MathF.Sin((across * 0.55f) + (coarseNoise * 2.2f)) * 0.04f;

        return ((ridge * 0.20f) + (wave2 * 0.08f) + pore + crossTint) * grainStrength;
    }

    private static float ComputeBevelShade(
        FrameSide side,
        float acrossRatio,
        float outerDistance,
        float innerDistance,
        int frameWidth,
        float bevelStrength)
    {
        float bevelSpan = MathF.Max(2f, frameWidth * (0.16f + (0.10f * bevelStrength)));
        float outerMask = 1f - ProceduralEffectHelper.SmoothStep(0f, bevelSpan, outerDistance);
        float innerMask = 1f - ProceduralEffectHelper.SmoothStep(0f, bevelSpan, innerDistance);

        float bodySlope = 0.5f - acrossRatio;
        float shade = side switch
        {
            FrameSide.Top => (outerMask * 0.24f) - (innerMask * 0.28f) + (bodySlope * 0.12f),
            FrameSide.Left => (outerMask * 0.18f) - (innerMask * 0.22f) + (bodySlope * 0.10f),
            FrameSide.Right => -(outerMask * 0.20f) + (innerMask * 0.08f) - (bodySlope * 0.08f),
            _ => -(outerMask * 0.26f) + (innerMask * 0.10f) - (bodySlope * 0.12f)
        };

        return shade * bevelStrength;
    }

    private static float ComputeMiterSeamShade(int x, int y, int width, int height, int frameWidth)
    {
        float seamWidth = MathF.Max(1.5f, frameWidth * 0.08f);

        if (x < frameWidth && y < frameWidth)
        {
            return (1f - ProceduralEffectHelper.SmoothStep(0f, seamWidth, MathF.Abs((x + y) - (frameWidth - 1f)))) * 0.10f;
        }

        if (x >= width - frameWidth && y < frameWidth)
        {
            float diagonal = ((width - 1 - x) + y) - (frameWidth - 1f);
            return (1f - ProceduralEffectHelper.SmoothStep(0f, seamWidth, MathF.Abs(diagonal))) * 0.10f;
        }

        if (x < frameWidth && y >= height - frameWidth)
        {
            float diagonal = (x + (height - 1 - y)) - (frameWidth - 1f);
            return (1f - ProceduralEffectHelper.SmoothStep(0f, seamWidth, MathF.Abs(diagonal))) * 0.10f;
        }

        if (x >= width - frameWidth && y >= height - frameWidth)
        {
            float diagonal = ((width - 1 - x) + (height - 1 - y)) - (frameWidth - 1f);
            return (1f - ProceduralEffectHelper.SmoothStep(0f, seamWidth, MathF.Abs(diagonal))) * 0.10f;
        }

        return 0f;
    }
}