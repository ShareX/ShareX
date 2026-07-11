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

public sealed class ZoomBlurImageEffect : ImageEffectBase
{
    public override string Id => "zoom_blur";
    public override string Name => "Zoom blur";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.zoom_in;
    public override string Description => "Applies a radial zoom blur emanating from a center point.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ZoomBlurImageEffect>("strength", "Strength", 0, 100, 35, (e, v) => e.Strength = v),
        EffectParameters.IntSlider<ZoomBlurImageEffect>("samples", "Samples", 4, 64, 24, (e, v) => e.Samples = v),
        EffectParameters.FloatSlider<ZoomBlurImageEffect>("center_x", "Center X", 0, 100, 50, (e, v) => e.CenterX = v),
        EffectParameters.FloatSlider<ZoomBlurImageEffect>("center_y", "Center Y", 0, 100, 50, (e, v) => e.CenterY = v)
    ];

    public float Strength { get; set; } = 35f; // 0..100
    public int Samples { get; set; } = 24; // 4..64
    public float CenterX { get; set; } = 50f; // 0..100
    public float CenterY { get; set; } = 50f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;
        int sampleCount = Math.Clamp(Samples, 4, 64);
        if (strength <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float cx = (Math.Clamp(CenterX, 0f, 100f) / 100f) * (width - 1);
        float cy = (Math.Clamp(CenterY, 0f, 100f) / 100f) * (height - 1);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float invSamples = sampleCount <= 1 ? 1f : 1f / (sampleCount - 1);
        float[] factors = new float[sampleCount];
        float[] weights = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i * invSamples;
            factors[i] = 1f - (t * strength);
            weights[i] = 1f - (t * 0.6f);
        }

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                float dx = x - cx;
                float dy = y - cy;

                float sumR = 0f;
                float sumG = 0f;
                float sumB = 0f;
                float sumA = 0f;
                float sumW = 0f;

                for (int i = 0; i < sampleCount; i++)
                {
                    float factor = factors[i];
                    float w = weights[i];

                    float sampleX = cx + (dx * factor);
                    float sampleY = cy + (dy * factor);
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