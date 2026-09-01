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

public sealed class JpegArtifactImageEffect : ImageEffectBase
{
    public override string Id => "jpeg_artifact";
    public override string Name => "JPEG artifact";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.file_image;
    public override string Description => "Simulates JPEG compression artifacts with blocking and ringing.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<JpegArtifactImageEffect>("compression", "Compression", 0, 100, 62, (e, v) => e.Compression = v),
        EffectParameters.IntSlider<JpegArtifactImageEffect>("block_size", "Block size", 4, 32, 8, (e, v) => e.BlockSize = v),
        EffectParameters.FloatSlider<JpegArtifactImageEffect>("chroma_bleed", "Chroma bleed", 0, 100, 45, (e, v) => e.ChromaBleed = v),
        EffectParameters.FloatSlider<JpegArtifactImageEffect>("ringing", "Ringing", 0, 100, 28, (e, v) => e.Ringing = v)
    ];

    public float Compression { get; set; } = 62f;
    public int BlockSize { get; set; } = 8;
    public float ChromaBleed { get; set; } = 45f;
    public float Ringing { get; set; } = 28f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amount = Math.Clamp(Compression, 0f, 100f) / 100f;
        int blockSize = Math.Clamp(BlockSize, 4, 32);
        float chromaBleed = Math.Clamp(ChromaBleed, 0f, 100f) / 100f;
        float ringing = Math.Clamp(Ringing, 0f, 100f) / 100f;

        if (amount <= 0.0001f && ringing <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] compressedPixels = new SKColor[srcPixels.Length];

        float lumaStep = 1f + (amount * 22f);
        float chromaStep = 2f + (amount * 38f);

        for (int by = 0; by < height; by += blockSize)
        {
            int y1 = Math.Min(height, by + blockSize);

            for (int bx = 0; bx < width; bx += blockSize)
            {
                int x1 = Math.Min(width, bx + blockSize);
                float avgY = 0f;
                float avgCb = 0f;
                float avgCr = 0f;
                int count = 0;

                for (int y = by; y < y1; y++)
                {
                    int row = y * width;
                    for (int x = bx; x < x1; x++)
                    {
                        ToYCbCr(srcPixels[row + x], out float yy, out float cb, out float cr);
                        avgY += yy;
                        avgCb += cb;
                        avgCr += cr;
                        count++;
                    }
                }

                if (count > 0)
                {
                    float inv = 1f / count;
                    avgY *= inv;
                    avgCb *= inv;
                    avgCr *= inv;
                }

                for (int y = by; y < y1; y++)
                {
                    int row = y * width;
                    for (int x = bx; x < x1; x++)
                    {
                        SKColor src = srcPixels[row + x];
                        ToYCbCr(src, out float yy, out float cb, out float cr);

                        float blockY = ProceduralEffectHelper.Lerp(yy, avgY, amount * 0.16f);
                        float blockCb = ProceduralEffectHelper.Lerp(cb, avgCb, 0.15f + (chromaBleed * 0.70f) + (amount * 0.08f));
                        float blockCr = ProceduralEffectHelper.Lerp(cr, avgCr, 0.15f + (chromaBleed * 0.70f) + (amount * 0.08f));

                        float quantY = Quantize(blockY, lumaStep);
                        float quantCb = Quantize(blockCb, chromaStep);
                        float quantCr = Quantize(blockCr, chromaStep);

                        compressedPixels[row + x] = FromYCbCr(quantY, quantCb, quantCr, src.Alpha);
                    }
                }
            }
        }

        if (ringing <= 0.0001f)
        {
            return AnalogEffectHelper.CreateBitmap(source, compressedPixels);
        }

        SKColor[] dstPixels = new SKColor[compressedPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor current = compressedPixels[row + x];
                float boundary = BoundaryFactor(x, y, blockSize);

                if (boundary <= 0f)
                {
                    dstPixels[row + x] = current;
                    continue;
                }

                float lumLeft = AnalogEffectHelper.Luminance(compressedPixels[row + Math.Max(0, x - 1)]);
                float lumRight = AnalogEffectHelper.Luminance(compressedPixels[row + Math.Min(width - 1, x + 1)]);
                float lumTop = AnalogEffectHelper.Luminance(compressedPixels[(Math.Max(0, y - 1) * width) + x]);
                float lumBottom = AnalogEffectHelper.Luminance(compressedPixels[(Math.Min(height - 1, y + 1) * width) + x]);

                float edge = ((lumRight - lumLeft) + (lumBottom - lumTop)) * 0.25f;
                float oscillation = MathF.Sin(((x + y) * 0.82f) + (((x / blockSize) + (y / blockSize)) * 1.7f));
                float sharpen = edge * oscillation * ringing * boundary * 0.32f;

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(current.Red + (sharpen * 1.05f)),
                    ProceduralEffectHelper.ClampToByte(current.Green + (sharpen * 0.86f)),
                    ProceduralEffectHelper.ClampToByte(current.Blue - (sharpen * 0.30f)),
                    current.Alpha);
            }
        });

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }

    private static float Quantize(float value, float step)
    {
        return MathF.Round(value / step) * step;
    }

    private static float BoundaryFactor(int x, int y, int blockSize)
    {
        int localX = x % blockSize;
        int localY = y % blockSize;

        float fx = localX <= 1 || localX >= blockSize - 2 ? 1f : 0f;
        float fy = localY <= 1 || localY >= blockSize - 2 ? 1f : 0f;
        return Math.Max(fx, fy);
    }

    private static void ToYCbCr(SKColor color, out float y, out float cb, out float cr)
    {
        float r = color.Red;
        float g = color.Green;
        float b = color.Blue;

        y = (0.299f * r) + (0.587f * g) + (0.114f * b);
        cb = 128f - (0.168736f * r) - (0.331264f * g) + (0.5f * b);
        cr = 128f + (0.5f * r) - (0.418688f * g) - (0.081312f * b);
    }

    private static SKColor FromYCbCr(float y, float cb, float cr, byte alpha)
    {
        float r = y + (1.402f * (cr - 128f));
        float g = y - (0.344136f * (cb - 128f)) - (0.714136f * (cr - 128f));
        float b = y + (1.772f * (cb - 128f));

        return new SKColor(
            ProceduralEffectHelper.ClampToByte(r),
            ProceduralEffectHelper.ClampToByte(g),
            ProceduralEffectHelper.ClampToByte(b),
            alpha);
    }
}