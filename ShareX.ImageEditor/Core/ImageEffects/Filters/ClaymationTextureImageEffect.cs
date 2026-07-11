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

public sealed class ClaymationTextureImageEffect : ImageEffectBase
{
    public override string Id => "claymation_texture";
    public override string Name => "Claymation texture";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.paint_roller;
    public override string Description => "Gives the image a sculpted claymation look with chunky quantization and relief texture.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ClaymationTextureImageEffect>("chunkSize", "Chunk size", 3, 18, 8, (e, v) => e.ChunkSize = v),
        EffectParameters.FloatSlider<ClaymationTextureImageEffect>("smoothness", "Smoothness", 0f, 100f, 55f, (e, v) => e.Smoothness = v),
        EffectParameters.FloatSlider<ClaymationTextureImageEffect>("relief", "Relief", 0f, 100f, 35f, (e, v) => e.Relief = v),
        EffectParameters.FloatSlider<ClaymationTextureImageEffect>("saturation", "Saturation", -100f, 100f, 20f, (e, v) => e.Saturation = v),
        EffectParameters.FloatSlider<ClaymationTextureImageEffect>("textureGrain", "Texture grain", 0f, 100f, 30f, (e, v) => e.TextureGrain = v),
    ];

    public int ChunkSize { get; set; } = 8; // 3..18
    public float Smoothness { get; set; } = 55f; // 0..100
    public float Relief { get; set; } = 35f; // 0..100
    public float Saturation { get; set; } = 20f; // -100..100
    public float TextureGrain { get; set; } = 30f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int chunkSize = Math.Clamp(ChunkSize, 3, 18);
        float smoothness = Math.Clamp(Smoothness, 0f, 100f) / 100f;
        float relief = Math.Clamp(Relief, 0f, 100f) / 100f;
        float saturation = Math.Clamp(Saturation, -100f, 100f) / 100f;
        float textureGrain = Math.Clamp(TextureGrain, 0f, 100f) / 100f;
        int seed = Random.Shared.Next(1, int.MaxValue);

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;
        int blurRadius = Math.Clamp(chunkSize / 4, 1, 4);
        float smoothMix = smoothness * 0.9f;
        float satFactor = 1f + saturation;
        float cellSize = chunkSize * 1.85f;
        int levels = Math.Clamp(20 - chunkSize, 5, 18);
        float quantStep = 255f / (levels - 1);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            int yUp = y - blurRadius;
            int yDown = y + blurRadius;
            if (yUp < 0) yUp = 0;
            if (yDown > bottom) yDown = bottom;

            for (int x = 0; x < width; x++)
            {
                int xLeft = x - blurRadius;
                int xRight = x + blurRadius;
                if (xLeft < 0) xLeft = 0;
                if (xRight > right) xRight = right;

                SKColor center = srcPixels[row + x];
                SKColor north = srcPixels[(yUp * width) + x];
                SKColor south = srcPixels[(yDown * width) + x];
                SKColor west = srcPixels[row + xLeft];
                SKColor east = srcPixels[row + xRight];
                SKColor nw = srcPixels[(yUp * width) + xLeft];
                SKColor se = srcPixels[(yDown * width) + xRight];

                float avgR = ((center.Red * 3f) + (north.Red * 1.5f) + (south.Red * 1.5f) + (west.Red * 1.5f) + (east.Red * 1.5f) + nw.Red + se.Red) / 10f;
                float avgG = ((center.Green * 3f) + (north.Green * 1.5f) + (south.Green * 1.5f) + (west.Green * 1.5f) + (east.Green * 1.5f) + nw.Green + se.Green) / 10f;
                float avgB = ((center.Blue * 3f) + (north.Blue * 1.5f) + (south.Blue * 1.5f) + (west.Blue * 1.5f) + (east.Blue * 1.5f) + nw.Blue + se.Blue) / 10f;

                float r = ProceduralEffectHelper.Lerp(center.Red, avgR, smoothMix);
                float g = ProceduralEffectHelper.Lerp(center.Green, avgG, smoothMix);
                float b = ProceduralEffectHelper.Lerp(center.Blue, avgB, smoothMix);

                r = Quantize(r, quantStep);
                g = Quantize(g, quantStep);
                b = Quantize(b, quantStep);

                float fx = x / cellSize;
                float fy = y / cellSize;
                int cx = (int)MathF.Floor(fx);
                int cy = (int)MathF.Floor(fy);
                float tx = fx - cx;
                float ty = fy - cy;

                float n00 = ProceduralEffectHelper.Hash01(cx, cy, seed);
                float n10 = ProceduralEffectHelper.Hash01(cx + 1, cy, seed);
                float n01 = ProceduralEffectHelper.Hash01(cx, cy + 1, seed);
                float n11 = ProceduralEffectHelper.Hash01(cx + 1, cy + 1, seed);
                float coarse = ProceduralEffectHelper.Lerp(
                    ProceduralEffectHelper.Lerp(n00, n10, tx),
                    ProceduralEffectHelper.Lerp(n01, n11, tx),
                    ty);

                float ridgeA = MathF.Sin(((x + (coarse * 13f)) / cellSize) * MathF.PI * 2f);
                float ridgeB = MathF.Cos(((y - (coarse * 11f)) / cellSize) * MathF.PI * 2f);
                float ridge = ridgeA * ridgeB;

                float fine = ((ProceduralEffectHelper.Hash01(x, y, seed ^ 919) * 2f) - 1f) * textureGrain;
                float reliefOffset = (((coarse - 0.5f) * 2f) + (ridge * 0.65f)) * relief * 23f;
                float grainOffset = fine * (4f + (relief * 7f));
                float offset = reliefOffset + grainOffset;

                r += offset + (relief * 2f);
                g += offset;
                b += offset - (relief * 2.5f);

                float gray = (0.2126f * r) + (0.7152f * g) + (0.0722f * b);
                r = gray + ((r - gray) * satFactor);
                g = gray + ((g - gray) * satFactor);
                b = gray + ((b - gray) * satFactor);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    center.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float Quantize(float value, float step)
    {
        return MathF.Round(value / step) * step;
    }
}