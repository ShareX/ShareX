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

public sealed class KaleidoscopeImageEffect : ImageEffectBase
{
    public override string Id => "kaleidoscope";
    public override string Name => "Kaleidoscope";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.loader_pinwheel;
    public override string Description => "Creates a kaleidoscope mirror pattern.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<KaleidoscopeImageEffect>("segments", "Segments", 2, 24, 8, (e, v) => e.Segments = v),
        EffectParameters.FloatSlider<KaleidoscopeImageEffect>("rotation", "Rotation", 0f, 360f, 0f, (e, v) => e.Rotation = v),
        EffectParameters.FloatSlider<KaleidoscopeImageEffect>("zoom", "Zoom", 20f, 300f, 100f, (e, v) => e.Zoom = v),
        EffectParameters.FloatSlider<KaleidoscopeImageEffect>("center_x_percentage", "Center X percentage", 0f, 100f, 50f, (e, v) => e.CenterXPercentage = v),
        EffectParameters.FloatSlider<KaleidoscopeImageEffect>("center_y_percentage", "Center Y percentage", 0f, 100f, 50f, (e, v) => e.CenterYPercentage = v)
    ];

    public int Segments { get; set; } = 8;
    public float Rotation { get; set; } = 0f;
    public float Zoom { get; set; } = 100f;
    public float CenterXPercentage { get; set; } = 50f;
    public float CenterYPercentage { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int segmentCount = Math.Clamp(Segments, 2, 24);
        float zoom = Math.Clamp(Zoom, 20f, 300f) / 100f;
        float rotation = Rotation * (MathF.PI / 180f);
        float centerX = DistortionEffectHelper.PercentageToX(source.Width, CenterXPercentage);
        float centerY = DistortionEffectHelper.PercentageToY(source.Height, CenterYPercentage);
        float segmentAngle = (MathF.PI * 2f) / segmentCount;

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float dx = (x - centerX) / zoom;
                float dy = (y - centerY) / zoom;
                float distance = MathF.Sqrt((dx * dx) + (dy * dy));
                float angle = DistortionEffectHelper.WrapAngle(MathF.Atan2(dy, dx) - rotation);

                int segmentIndex = (int)MathF.Floor(angle / segmentAngle);
                float localAngle = angle - (segmentIndex * segmentAngle);
                if ((segmentIndex & 1) == 1)
                {
                    localAngle = segmentAngle - localAngle;
                }

                float sampleAngle = localAngle + rotation;
                float sampleX = centerX + (MathF.Cos(sampleAngle) * distance);
                float sampleY = centerY + (MathF.Sin(sampleAngle) * distance);

                dstPixels[row + x] = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}