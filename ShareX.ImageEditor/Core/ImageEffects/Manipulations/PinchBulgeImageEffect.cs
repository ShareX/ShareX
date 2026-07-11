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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class PinchBulgeImageEffect : ImageEffectBase
{
    public override string Id => "pinch_bulge";
    public override string Name => "Pinch / bulge";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.maximize;
    public override string Description => "Applies a pinch or bulge distortion to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<PinchBulgeImageEffect>("strength", "Strength", -100f, 100f, 35f, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<PinchBulgeImageEffect>("radius_percentage", "Radius %", 1f, 100f, 50f, (e, v) => e.RadiusPercentage = v),
        EffectParameters.FloatSlider<PinchBulgeImageEffect>("center_x_percentage", "Center X %", 0f, 100f, 50f, (e, v) => e.CenterXPercentage = v),
        EffectParameters.FloatSlider<PinchBulgeImageEffect>("center_y_percentage", "Center Y %", 0f, 100f, 50f, (e, v) => e.CenterYPercentage = v)
    ];

    public float Strength { get; set; } = 35f; // -100 pinch, +100 bulge
    public float RadiusPercentage { get; set; } = 50f;
    public float CenterXPercentage { get; set; } = 50f;
    public float CenterYPercentage { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        float radius = Math.Max(1f, Math.Min(width, height) * Math.Clamp(RadiusPercentage, 1f, 100f) / 100f);
        float cx = Math.Clamp(CenterXPercentage, 0f, 100f) / 100f * right;
        float cy = Math.Clamp(CenterYPercentage, 0f, 100f) / 100f * bottom;
        float strength = Math.Clamp(Strength, -100f, 100f) / 100f;

        if (Math.Abs(strength) < 0.0001f)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = MathF.Sqrt(dx * dx + dy * dy);

                if (dist >= radius || dist <= 0.0001f)
                {
                    dstPixels[row + x] = srcPixels[row + x];
                    continue;
                }

                float normalized = dist / radius;
                float amount = 1f + strength * (1f - normalized * normalized);
                if (amount < 0.05f) amount = 0.05f;

                float srcDist = dist / amount;
                float scale = srcDist / dist;

                int sampleX = Clamp((int)MathF.Round(cx + dx * scale), 0, right);
                int sampleY = Clamp((int)MathF.Round(cy + dy * scale), 0, bottom);

                dstPixels[row + x] = srcPixels[sampleY * width + sampleX];
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType) { Pixels = dstPixels };
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}