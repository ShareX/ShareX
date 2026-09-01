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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class LiquifyPushSmudgeImageEffect : ImageEffectBase
{
    public override string Id => "liquify_push_smudge";
    public override string Name => "Liquify push / smudge";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.droplet;
    public override string Description => "Pushes and smudges pixels in a direction.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<LiquifyPushSmudgeImageEffect>("angle", "Angle", 0f, 360f, 0f, (e, v) => e.Angle = v),
        EffectParameters.FloatSlider<LiquifyPushSmudgeImageEffect>("distance", "Distance", -200f, 200f, 40f, (e, v) => e.Distance = v),
        EffectParameters.FloatSlider<LiquifyPushSmudgeImageEffect>("radius_percentage", "Radius percentage", 1f, 100f, 25f, (e, v) => e.RadiusPercentage = v),
        EffectParameters.FloatSlider<LiquifyPushSmudgeImageEffect>("smudge", "Smudge", 0f, 100f, 35f, (e, v) => e.Smudge = v),
        EffectParameters.FloatSlider<LiquifyPushSmudgeImageEffect>("center_x_percentage", "Center X percentage", 0f, 100f, 50f, (e, v) => e.CenterXPercentage = v),
        EffectParameters.FloatSlider<LiquifyPushSmudgeImageEffect>("center_y_percentage", "Center Y percentage", 0f, 100f, 50f, (e, v) => e.CenterYPercentage = v)
    ];

    public float Angle { get; set; } = 0f;
    public float Distance { get; set; } = 40f;
    public float RadiusPercentage { get; set; } = 25f;
    public float Smudge { get; set; } = 35f;
    public float CenterXPercentage { get; set; } = 50f;
    public float CenterYPercentage { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float distance = Math.Clamp(Distance, -200f, 200f);
        if (Math.Abs(distance) <= 0.01f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float radius = Math.Max(1f, Math.Min(width, height) * Math.Clamp(RadiusPercentage, 1f, 100f) / 100f);
        float smudge01 = Math.Clamp(Smudge, 0f, 100f) / 100f;
        int samples = 1 + (int)MathF.Round(smudge01 * 5f);
        float angle = Angle * (MathF.PI / 180f);
        float directionX = MathF.Cos(angle);
        float directionY = MathF.Sin(angle);
        float centerX = DistortionEffectHelper.PercentageToX(width, CenterXPercentage);
        float centerY = DistortionEffectHelper.PercentageToY(height, CenterYPercentage);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float dx = x - centerX;
                float dy = y - centerY;
                float pointDistance = MathF.Sqrt((dx * dx) + (dy * dy));

                if (pointDistance >= radius)
                {
                    dstPixels[row + x] = srcPixels[row + x];
                    continue;
                }

                float normalized = pointDistance / radius;
                float influence = 1f - ProceduralEffectHelper.SmoothStep(0f, 1f, normalized);
                influence *= influence;

                float push = distance * influence;
                if (samples <= 1)
                {
                    dstPixels[row + x] = DistortionEffectHelper.SampleClamped(
                        srcPixels,
                        width,
                        height,
                        x - (directionX * push),
                        y - (directionY * push));
                    continue;
                }

                float sumR = 0f;
                float sumG = 0f;
                float sumB = 0f;
                float sumA = 0f;

                for (int i = 0; i < samples; i++)
                {
                    float t = samples == 1 ? 0f : i / (float)(samples - 1);
                    float offset = push * (1f - (t * smudge01));
                    SKColor sample = DistortionEffectHelper.SampleClamped(
                        srcPixels,
                        width,
                        height,
                        x - (directionX * offset),
                        y - (directionY * offset));

                    sumR += sample.Red;
                    sumG += sample.Green;
                    sumB += sample.Blue;
                    sumA += sample.Alpha;
                }

                float inv = 1f / samples;
                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(sumR * inv),
                    ProceduralEffectHelper.ClampToByte(sumG * inv),
                    ProceduralEffectHelper.ClampToByte(sumB * inv),
                    ProceduralEffectHelper.ClampToByte(sumA * inv));
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}