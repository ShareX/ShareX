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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class RippleRefractionImageEffect : ImageEffectBase
{
    public override string Id => "ripple_refraction";
    public override string Name => "Ripple refraction";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.waves;
    public override string Description => "Applies a ripple refraction distortion effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<RippleRefractionImageEffect>("amplitude", "Amplitude", 0f, 80f, 12f, (e, v) => e.Amplitude = v),
        EffectParameters.FloatSlider<RippleRefractionImageEffect>("wavelength", "Wavelength", 4f, 400f, 32f, (e, v) => e.Wavelength = v),
        EffectParameters.FloatSlider<RippleRefractionImageEffect>("phase", "Phase", -360f, 360f, 0f, (e, v) => e.Phase = v),
        EffectParameters.FloatSlider<RippleRefractionImageEffect>("refraction", "Refraction", 0f, 100f, 35f, (e, v) => e.Refraction = v),
        EffectParameters.FloatSlider<RippleRefractionImageEffect>("center_x_percentage", "Center X %", 0f, 100f, 50f, (e, v) => e.CenterXPercentage = v),
        EffectParameters.FloatSlider<RippleRefractionImageEffect>("center_y_percentage", "Center Y %", 0f, 100f, 50f, (e, v) => e.CenterYPercentage = v)
    ];

    public float Amplitude { get; set; } = 12f;
    public float Wavelength { get; set; } = 32f;
    public float Phase { get; set; } = 0f;
    public float Refraction { get; set; } = 35f;
    public float CenterXPercentage { get; set; } = 50f;
    public float CenterYPercentage { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amplitude = Math.Clamp(Amplitude, 0f, 80f);
        if (amplitude <= 0.01f)
        {
            return source.Copy();
        }

        float wavelength = Math.Clamp(Wavelength, 4f, 400f);
        float phaseRadians = Phase * (MathF.PI / 180f);
        float refraction01 = Math.Clamp(Refraction, 0f, 100f) / 100f;
        float centerX = DistortionEffectHelper.PercentageToX(source.Width, CenterXPercentage);
        float centerY = DistortionEffectHelper.PercentageToY(source.Height, CenterYPercentage);
        float waveScale = (MathF.PI * 2f) / wavelength;

        int width = source.Width;
        int height = source.Height;
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

                if (distance <= 0.0001f)
                {
                    dstPixels[row + x] = srcPixels[row + x];
                    continue;
                }

                float wavePhase = (distance * waveScale) + phaseRadians;
                float radialOffset = MathF.Sin(wavePhase) * amplitude;
                float refractedOffset = MathF.Cos(wavePhase) * amplitude * refraction01 * 0.35f;
                float sampleDistance = distance + radialOffset + refractedOffset;
                float scale = sampleDistance / distance;

                float sampleX = centerX + (dx * scale);
                float sampleY = centerY + (dy * scale);

                dstPixels[row + x] = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}