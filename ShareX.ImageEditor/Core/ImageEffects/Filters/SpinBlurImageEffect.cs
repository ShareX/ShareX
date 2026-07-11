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

public sealed class SpinBlurImageEffect : ImageEffectBase
{
    public override string Id => "spin_blur";
    public override string Name => "Spin blur";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.rotate_cw;
    public override string Description => "Applies a rotational motion blur around a center point.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<SpinBlurImageEffect>("angle", "Angle", 0f, 180f, 20f, (e, v) => e.Angle = v),
        EffectParameters.IntSlider<SpinBlurImageEffect>("samples", "Samples", 4, 64, 24, (e, v) => e.Samples = v),
        EffectParameters.FloatSlider<SpinBlurImageEffect>("center_x", "Center X", 0f, 100f, 50f, (e, v) => e.CenterX = v),
        EffectParameters.FloatSlider<SpinBlurImageEffect>("center_y", "Center Y", 0f, 100f, 50f, (e, v) => e.CenterY = v),
    ];

    public float Angle { get; set; } = 20f; // 0..180
    public int Samples { get; set; } = 24; // 4..64
    public float CenterX { get; set; } = 50f; // 0..100
    public float CenterY { get; set; } = 50f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float angle = Math.Clamp(Angle, 0f, 180f);
        int sampleCount = Math.Clamp(Samples, 4, 64);
        if (angle <= 0.001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float cx = (Math.Clamp(CenterX, 0f, 100f) / 100f) * (width - 1);
        float cy = (Math.Clamp(CenterY, 0f, 100f) / 100f) * (height - 1);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float maxRadians = angle * (MathF.PI / 180f);
        float half = (sampleCount - 1) * 0.5f;
        float invHalf = half <= 0.0001f ? 1f : 1f / half;
        float[] sinValues = new float[sampleCount];
        float[] cosValues = new float[sampleCount];
        float[] weights = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (i - half) * invHalf; // -1..1
            float theta = t * maxRadians;
            sinValues[i] = MathF.Sin(theta);
            cosValues[i] = MathF.Cos(theta);
            weights[i] = 1f - (MathF.Abs(t) * 0.7f);
        }

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                if ((dx * dx) + (dy * dy) < 0.25f)
                {
                    dstPixels[row + x] = srcPixels[row + x];
                    continue;
                }

                float sumR = 0f;
                float sumG = 0f;
                float sumB = 0f;
                float sumA = 0f;
                float sumW = 0f;

                for (int i = 0; i < sampleCount; i++)
                {
                    float sin = sinValues[i];
                    float cos = cosValues[i];
                    float w = weights[i];

                    float sampleX = cx + ((dx * cos) - (dy * sin));
                    float sampleY = cy + ((dx * sin) + (dy * cos));
                    SKColor sample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY);

                    sumR += sample.Red * w;
                    sumG += sample.Green * w;
                    sumB += sample.Blue * w;
                    sumA += sample.Alpha * w;
                    sumW += w;
                }

                float inv = 1f / Math.Max(0.0001f, sumW);
                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(sumR * inv),
                    ProceduralEffectHelper.ClampToByte(sumG * inv),
                    ProceduralEffectHelper.ClampToByte(sumB * inv),
                    ProceduralEffectHelper.ClampToByte(sumA * inv));
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}