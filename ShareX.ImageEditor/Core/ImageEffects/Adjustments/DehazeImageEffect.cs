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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class DehazeImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "dehaze";
    public override string Name => "Dehaze";
    public override string IconKey => LucideIcons.sunset;
    public override string Description => "Removes or adds atmospheric haze.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<DehazeImageEffect>("amount", "Amount", -100, 100, 50, (effect, value) => effect.Amount = value)
    ];

    /// <summary>
    /// Positive values remove haze (increase contrast and saturation in low-contrast areas).
    /// Negative values add haze (reduce contrast, lighten shadows).
    /// Range: -100 to 100.
    /// </summary>
    public float Amount { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amount = Math.Clamp(Amount, -100f, 100f) / 100f;
        if (Math.Abs(amount) < 0.001f)
        {
            return source.Copy();
        }

        // Estimate atmospheric light from the brightest region of the image.
        // Use a downscaled version to find the average of the top 0.1% brightest pixels.
        SKColor[] pixels = source.Pixels;
        int width = source.Width;
        int height = source.Height;
        int count = pixels.Length;

        // Compute per-pixel minimum channel (dark channel approximation).
        float sumAtmR = 0f, sumAtmG = 0f, sumAtmB = 0f;
        int atmCount = 0;
        int topN = Math.Max(1, count / 1000);

        // Simple approach: find average brightness, then estimate atmospheric light
        // from the brightest pixels.
        float[] brightness = new float[count];
        for (int i = 0; i < count; i++)
        {
            SKColor c = pixels[i];
            brightness[i] = (c.Red + c.Green + c.Blue) / 3f;
        }

        // Find threshold for top 0.1% brightness.
        float[] sorted = new float[count];
        Array.Copy(brightness, sorted, count);
        Array.Sort(sorted);
        float threshold = sorted[Math.Max(0, count - topN)];

        for (int i = 0; i < count; i++)
        {
            if (brightness[i] >= threshold)
            {
                sumAtmR += pixels[i].Red;
                sumAtmG += pixels[i].Green;
                sumAtmB += pixels[i].Blue;
                atmCount++;
            }
        }

        float atmR = atmCount > 0 ? sumAtmR / atmCount : 200f;
        float atmG = atmCount > 0 ? sumAtmG / atmCount : 200f;
        float atmB = atmCount > 0 ? sumAtmB / atmCount : 200f;

        SKColor[] result = new SKColor[count];

        for (int i = 0; i < count; i++)
        {
            SKColor c = pixels[i];
            float r = c.Red;
            float g = c.Green;
            float b = c.Blue;

            if (amount > 0f)
            {
                // Remove haze: recover scene radiance.
                // Estimate transmission from dark channel.
                float darkChannel = Math.Min(r / atmR, Math.Min(g / atmG, b / atmB));
                float transmission = 1f - (amount * darkChannel);
                transmission = Math.Max(transmission, 0.1f);

                r = (r - atmR * (1f - transmission)) / transmission;
                g = (g - atmG * (1f - transmission)) / transmission;
                b = (b - atmB * (1f - transmission)) / transmission;
            }
            else
            {
                // Add haze: blend toward atmospheric light.
                float hazeAmount = -amount;
                r = r + (atmR - r) * hazeAmount;
                g = g + (atmG - g) * hazeAmount;
                b = b + (atmB - b) * hazeAmount;
            }

            result[i] = new SKColor(ClampToByte(r), ClampToByte(g), ClampToByte(b), c.Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = result
        };
    }

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}