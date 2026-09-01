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

public sealed class ClarityImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "clarity";
    public override string Name => "Clarity";
    public override string IconKey => LucideIcons.eye;
    public override string Description => "Enhances midtone contrast for added depth and punch.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ClarityImageEffect>("amount", "Amount", -100, 100, 0, (effect, value) => effect.Amount = value)
    ];

    /// <summary>
    /// Positive values increase midtone contrast (more punch).
    /// Negative values decrease midtone contrast (softer).
    /// Range: -100 to 100.
    /// </summary>
    public float Amount { get; set; } = 0f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amount = Math.Clamp(Amount, -100f, 100f) / 100f;
        if (Math.Abs(amount) < 0.001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;

        // Step 1: Create a blurred (low-frequency) version of the image.
        // Use SkiaSharp's blur filter for the low-pass.
        float blurRadius = Math.Max(width, height) * 0.02f;
        blurRadius = Math.Max(blurRadius, 3f);

        SKBitmap blurred = new(width, height, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new(blurred))
        {
            using SKPaint blurPaint = new()
            {
                ImageFilter = SKImageFilter.CreateBlur(blurRadius, blurRadius)
            };
            blurCanvas.DrawBitmap(source, 0, 0, blurPaint);
        }

        // Step 2: Compute high-pass detail = original - blurred,
        // then add it back weighted by the amount, targeting midtones.
        SKColor[] srcPixels = source.Pixels;
        SKColor[] blurPixels = blurred.Pixels;
        blurred.Dispose();

        int count = srcPixels.Length;
        SKColor[] result = new SKColor[count];

        for (int i = 0; i < count; i++)
        {
            SKColor src = srcPixels[i];
            SKColor blur = blurPixels[i];

            float sr = src.Red / 255f;
            float sg = src.Green / 255f;
            float sb = src.Blue / 255f;

            float br = blur.Red / 255f;
            float bg = blur.Green / 255f;
            float bb = blur.Blue / 255f;

            // Luminance for midtone weighting
            float lum = 0.299f * sr + 0.587f * sg + 0.114f * sb;

            // Midtone mask: peaks at 0.5, falls off at extremes.
            // Gaussian-like bell centered at 0.5.
            float diff = lum - 0.5f;
            float midtoneMask = MathF.Exp(-8f * diff * diff);

            // High-pass detail
            float detailR = sr - br;
            float detailG = sg - bg;
            float detailB = sb - bb;

            // Apply clarity: add detail weighted by midtone mask and amount
            float strength = amount * midtoneMask * 1.5f;
            float outR = Math.Clamp(sr + detailR * strength, 0f, 1f);
            float outG = Math.Clamp(sg + detailG * strength, 0f, 1f);
            float outB = Math.Clamp(sb + detailB * strength, 0f, 1f);

            result[i] = new SKColor(
                (byte)MathF.Round(outR * 255f),
                (byte)MathF.Round(outG * 255f),
                (byte)MathF.Round(outB * 255f),
                src.Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = result
        };
    }
}