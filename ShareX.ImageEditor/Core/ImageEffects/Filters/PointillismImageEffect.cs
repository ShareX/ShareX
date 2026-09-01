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

public sealed class PointillismImageEffect : ImageEffectBase
{
    public override string Id => "pointillism";
    public override string Name => "Pointillism";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.circle_dot;
    public override string Description => "Renders the image as a pointillist painting with colored dots.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<PointillismImageEffect>("dot_size", "Dot size", 2, 24, 7, (e, v) => e.DotSize = v),
        EffectParameters.FloatSlider<PointillismImageEffect>("density", "Density", 10f, 100f, 72f, (e, v) => e.Density = v),
        EffectParameters.FloatSlider<PointillismImageEffect>("jitter", "Jitter", 0f, 100f, 65f, (e, v) => e.Jitter = v),
        EffectParameters.FloatSlider<PointillismImageEffect>("color_boost", "Color boost", 0f, 100f, 20f, (e, v) => e.ColorBoost = v),
        EffectParameters.FloatSlider<PointillismImageEffect>("background_mix", "Background mix", 0f, 100f, 20f, (e, v) => e.BackgroundMix = v),
    ];

    public int DotSize { get; set; } = 7; // 2..24
    public float Density { get; set; } = 72f; // 10..100
    public float Jitter { get; set; } = 65f; // 0..100
    public float ColorBoost { get; set; } = 20f; // 0..100
    public float BackgroundMix { get; set; } = 20f; // 0..100
    public int Seed { get; set; } = 2026;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int dotSize = Math.Clamp(DotSize, 2, 24);
        float density = Math.Clamp(Density, 10f, 100f) / 100f;
        float jitter = Math.Clamp(Jitter, 0f, 100f) / 100f;
        float boost = Math.Clamp(ColorBoost, 0f, 100f) / 100f;
        float backgroundMix = Math.Clamp(BackgroundMix, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKBitmap result = new SKBitmap(width, height, source.ColorType, source.AlphaType);

        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);

        if (backgroundMix > 0f)
        {
            using SKPaint basePaint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(255, 255, 255, ProceduralEffectHelper.ClampToByte(backgroundMix * 255f))
            };
            canvas.DrawBitmap(source, 0, 0, basePaint);
        }

        using SKPaint dotPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        int step = Math.Max(2, dotSize);
        float jitterAmount = jitter * step * 0.6f;

        for (int gy = 0; gy < height; gy += step)
        {
            for (int gx = 0; gx < width; gx += step)
            {
                int cellX = gx / step;
                int cellY = gy / step;
                float presence = ProceduralEffectHelper.Hash01(cellX, cellY, Seed);
                if (presence > density)
                {
                    continue;
                }

                float jx = ((ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 111) * 2f) - 1f) * jitterAmount;
                float jy = ((ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 257) * 2f) - 1f) * jitterAmount;

                float cx = Math.Clamp(gx + (step * 0.5f) + jx, 0f, width - 1f);
                float cy = Math.Clamp(gy + (step * 0.5f) + jy, 0f, height - 1f);

                SKColor sample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, cx, cy);
                if (boost > 0f)
                {
                    sample = BoostSaturation(sample, boost);
                }

                float radiusJitter = 0.6f + (ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 509) * 0.9f);
                float radius = Math.Max(1f, dotSize * 0.36f * radiusJitter);

                dotPaint.Color = sample;
                canvas.DrawCircle(cx, cy, radius, dotPaint);
            }
        }

        return result;
    }

    private static SKColor BoostSaturation(SKColor color, float amount01)
    {
        float r = color.Red;
        float g = color.Green;
        float b = color.Blue;

        float gray = (r + g + b) / 3f;
        float factor = 1f + amount01;

        return new SKColor(
            ProceduralEffectHelper.ClampToByte(gray + ((r - gray) * factor)),
            ProceduralEffectHelper.ClampToByte(gray + ((g - gray) * factor)),
            ProceduralEffectHelper.ClampToByte(gray + ((b - gray) * factor)),
            color.Alpha);
    }
}