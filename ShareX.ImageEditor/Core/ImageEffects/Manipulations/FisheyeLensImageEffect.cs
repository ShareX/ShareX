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

public sealed class FisheyeLensImageEffect : ImageEffectBase
{
    public override string Id => "fisheye_lens";
    public override string Name => "Fisheye lens";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.circle_gauge;
    public override string Description => "Applies a fisheye lens distortion effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<FisheyeLensImageEffect>("strength", "Strength", 0f, 100f, 58f, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<FisheyeLensImageEffect>("radius_percentage", "Radius percentage", 1f, 100f, 100f, (e, v) => e.RadiusPercentage = v),
        EffectParameters.FloatSlider<FisheyeLensImageEffect>("center_x_percentage", "Center X percentage", 0f, 100f, 50f, (e, v) => e.CenterXPercentage = v),
        EffectParameters.FloatSlider<FisheyeLensImageEffect>("center_y_percentage", "Center Y percentage", 0f, 100f, 50f, (e, v) => e.CenterYPercentage = v)
    ];

    public float Strength { get; set; } = 58f;
    public float RadiusPercentage { get; set; } = 100f;
    public float CenterXPercentage { get; set; } = 50f;
    public float CenterYPercentage { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float strength01 = Math.Clamp(Strength, 0f, 100f) / 100f;
        if (strength01 <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float radius = Math.Max(1f, Math.Min(width, height) * Math.Clamp(RadiusPercentage, 1f, 100f) / 100f);
        float centerX = DistortionEffectHelper.PercentageToX(width, CenterXPercentage);
        float centerY = DistortionEffectHelper.PercentageToY(height, CenterYPercentage);
        float exponent = 1f + (strength01 * 2.4f);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float dx = x - centerX;
                float dy = y - centerY;
                float distance = MathF.Sqrt((dx * dx) + (dy * dy));

                if (distance <= 0.0001f || distance >= radius)
                {
                    dstPixels[row + x] = srcPixels[row + x];
                    continue;
                }

                float normalized = distance / radius;
                float sampleDistance = MathF.Pow(normalized, exponent) * radius;
                float scale = sampleDistance / distance;

                float sampleX = centerX + (dx * scale);
                float sampleY = centerY + (dy * scale);

                dstPixels[row + x] = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}