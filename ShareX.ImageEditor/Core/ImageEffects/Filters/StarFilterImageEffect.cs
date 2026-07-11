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

public sealed class StarFilterImageEffect : ImageEffectBase
{
    public override string Id => "star_filter";
    public override string Name => "Star filter";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.star;
    public override string Description => "Adds star-shaped light streaks to bright areas.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<StarFilterImageEffect>("threshold", "Threshold", 0f, 100f, 74f, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<StarFilterImageEffect>("length", "Length", 4f, 80f, 40f, (e, v) => e.Length = v),
        EffectParameters.FloatSlider<StarFilterImageEffect>("strength", "Strength", 0f, 100f, 58f, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<StarFilterImageEffect>("rotation", "Rotation", 0f, 360f, 0f, (e, v) => e.Rotation = v),
        EffectParameters.FloatSlider<StarFilterImageEffect>("warmth", "Warmth", 0f, 100f, 48f, (e, v) => e.Warmth = v),
    ];

    public float Threshold { get; set; } = 74f;
    public float Length { get; set; } = 40f;
    public float Strength { get; set; } = 58f;
    public float Rotation { get; set; } = 0f;
    public float Warmth { get; set; } = 48f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float threshold = Math.Clamp(Threshold, 0f, 100f) / 100f;
        float length = Math.Clamp(Length, 4f, 80f);
        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;
        float warmth = Math.Clamp(Warmth, 0f, 100f) / 100f;

        if (strength <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int sampleCount = Math.Clamp((int)MathF.Round(length / 4f), 3, 18);
        float step = Math.Max(1f, length / sampleCount);
        float baseAngle = Rotation * (MathF.PI / 180f);
        float[] angles =
        [
            baseAngle,
            baseAngle + (MathF.PI * 0.5f),
            baseAngle + (MathF.PI * 0.25f),
            baseAngle + (MathF.PI * 0.75f)
        ];
        float[] weights = [1f, 1f, 0.68f, 0.68f];

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                float streak = 0f;

                for (int d = 0; d < angles.Length; d++)
                {
                    float dirX = MathF.Cos(angles[d]);
                    float dirY = MathF.Sin(angles[d]);

                    for (int i = 1; i <= sampleCount; i++)
                    {
                        float falloff = 1f - (i / (float)(sampleCount + 1));
                        float offset = i * step;

                        SKColor positive = AnalogEffectHelper.Sample(srcPixels, width, height, x + (dirX * offset), y + (dirY * offset));
                        SKColor negative = AnalogEffectHelper.Sample(srcPixels, width, height, x - (dirX * offset), y - (dirY * offset));

                        float lumA = MathF.Max(0f, AnalogEffectHelper.Luminance01(positive) - threshold) / MathF.Max(0.0001f, 1f - threshold);
                        float lumB = MathF.Max(0f, AnalogEffectHelper.Luminance01(negative) - threshold) / MathF.Max(0.0001f, 1f - threshold);

                        streak += ((lumA * lumA) + (lumB * lumB)) * falloff * weights[d];
                    }
                }

                float star = strength * (streak / (sampleCount * 3.6f));
                SKColor src = srcPixels[row + x];
                float r = AnalogEffectHelper.Screen(src.Red / 255f, star * (0.90f + (warmth * 0.18f)));
                float g = AnalogEffectHelper.Screen(src.Green / 255f, star * (0.74f + (warmth * 0.08f)));
                float b = AnalogEffectHelper.Screen(src.Blue / 255f, star * (0.52f - (warmth * 0.16f)));

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    src.Alpha);
            }
        });

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }
}