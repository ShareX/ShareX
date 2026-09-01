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
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class MotionBlurImageEffect : ImageEffectBase
{
    public override string Id => "motion_blur";
    public override string Name => "Motion blur";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.move_horizontal;
    public override string Description => "Applies a directional motion blur at a specified angle.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MotionBlurImageEffect>("distance", "Distance", 1, 200, 12, (e, v) => e.Distance = v),
        EffectParameters.FloatSlider<MotionBlurImageEffect>("angle", "Angle", 0, 360, 0, (e, v) => e.Angle = v),
    ];

    public int Distance { get; set; } = 12;
    public float Angle { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int distance = Math.Clamp(Distance, 1, 200);
        if (distance <= 1)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        float radians = Angle * (MathF.PI / 180f);
        float dx = MathF.Cos(radians);
        float dy = MathF.Sin(radians);

        int start = -((distance - 1) / 2);
        int end = distance / 2;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            int dstRow = y * width;

            for (int x = 0; x < width; x++)
            {
                int sumR = 0;
                int sumG = 0;
                int sumB = 0;
                int sumA = 0;
                int samples = 0;

                for (int i = start; i <= end; i++)
                {
                    int sx = Clamp((int)MathF.Round(x + (i * dx)), 0, right);
                    int sy = Clamp((int)MathF.Round(y + (i * dy)), 0, bottom);
                    SKColor c = srcPixels[(sy * width) + sx];

                    sumR += c.Red;
                    sumG += c.Green;
                    sumB += c.Blue;
                    sumA += c.Alpha;
                    samples++;
                }

                dstPixels[dstRow + x] = new SKColor(
                    (byte)(sumR / samples),
                    (byte)(sumG / samples),
                    (byte)(sumB / samples),
                    (byte)(sumA / samples));
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}