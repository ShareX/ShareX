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

public enum PolarWarpMode
{
    CartesianToPolar,
    PolarToCartesian
}

public sealed class PolarWarpImageEffect : ImageEffectBase
{
    public override string Id => "polar_warp";
    public override string Name => "Polar warp";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.orbit;
    public override string Description => "Converts between Cartesian and polar coordinate systems.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<PolarWarpImageEffect, PolarWarpMode>("mode", "Mode", PolarWarpMode.CartesianToPolar, (e, v) => e.Mode = v, new (string Label, PolarWarpMode Value)[] { ("Cartesian to polar", PolarWarpMode.CartesianToPolar), ("Polar to Cartesian", PolarWarpMode.PolarToCartesian) }),
        EffectParameters.FloatSlider<PolarWarpImageEffect>("rotation", "Rotation", -360f, 360f, 0f, (e, v) => e.Rotation = v),
        EffectParameters.FloatSlider<PolarWarpImageEffect>("radius_scale", "Radius scale", 20f, 200f, 100f, (e, v) => e.RadiusScale = v),
        EffectParameters.FloatSlider<PolarWarpImageEffect>("center_x_percentage", "Center X %", 0f, 100f, 50f, (e, v) => e.CenterXPercentage = v),
        EffectParameters.FloatSlider<PolarWarpImageEffect>("center_y_percentage", "Center Y %", 0f, 100f, 50f, (e, v) => e.CenterYPercentage = v)
    ];

    public PolarWarpMode Mode { get; set; } = PolarWarpMode.CartesianToPolar;
    public float Rotation { get; set; } = 0f;
    public float RadiusScale { get; set; } = 100f;
    public float CenterXPercentage { get; set; } = 50f;
    public float CenterYPercentage { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        float radiusScale = Math.Clamp(RadiusScale, 20f, 200f) / 100f;
        float rotation = Rotation * (MathF.PI / 180f);
        float centerX = DistortionEffectHelper.PercentageToX(width, CenterXPercentage);
        float centerY = DistortionEffectHelper.PercentageToY(height, CenterYPercentage);
        float maxRadius = (Math.Min(width - 1, height - 1) * 0.5f) * radiusScale;

        if (maxRadius <= 0.5f)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        float widthRange = Math.Max(1f, width - 1);
        float heightRange = Math.Max(1f, height - 1);

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                if (Mode == PolarWarpMode.CartesianToPolar)
                {
                    float polarAngle = ((x / widthRange) * MathF.PI * 2f) + rotation;
                    float radius = (y / heightRange) * maxRadius;
                    float sampleX = centerX + (MathF.Cos(polarAngle) * radius);
                    float sampleY = centerY + (MathF.Sin(polarAngle) * radius);
                    dstPixels[row + x] = DistortionEffectHelper.SampleTransparent(srcPixels, width, height, sampleX, sampleY);
                    continue;
                }

                float dx = x - centerX;
                float dy = y - centerY;
                float distance = MathF.Sqrt((dx * dx) + (dy * dy));

                if (distance > maxRadius)
                {
                    dstPixels[row + x] = SKColors.Transparent;
                    continue;
                }

                float angle = DistortionEffectHelper.WrapAngle(MathF.Atan2(dy, dx) - rotation);
                float sampleXPolar = (angle / (MathF.PI * 2f)) * widthRange;
                float sampleYPolar = (distance / maxRadius) * heightRange;

                dstPixels[row + x] = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleXPolar, sampleYPolar);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}