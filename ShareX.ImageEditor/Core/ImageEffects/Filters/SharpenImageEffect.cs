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

public sealed class SharpenImageEffect : ImageEffectBase
{
    public override string Id => "sharpen";
    public override string Name => "Sharpen";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.crosshair;
    public override string Description => "Sharpens the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<SharpenImageEffect>("strength", "Strength", 0, 100, 50, (effect, value) => effect.Strength = value)
    ];

    public int Strength { get; set; } = 50;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Strength <= 0) return source.Copy();

        float strength = Strength / 100f;
        float s = Math.Clamp(strength, 0f, 1f);

        // Create sharpening convolution kernel
        float center = 1 + 4 * s;
        float edge = -s;
        float[] kernel = new float[]
        {
            0, edge, 0,
            edge, center, edge,
            0, edge, 0
        };

        // Apply convolution
        using SKImageFilter sharpenFilter = SKImageFilter.CreateMatrixConvolution(
            new SKSizeI(3, 3),
            kernel,
            1f, // gain
            0f, // bias
            new SKPointI(1, 1), // kernel offset
            SKShaderTileMode.Clamp,
            true // convolve alpha
        );

        using SKPaint paint = new SKPaint { ImageFilter = sharpenFilter };

        SKBitmap sharpened = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas sharpCanvas = new SKCanvas(sharpened);
        sharpCanvas.DrawBitmap(source, 0, 0, paint);

        return sharpened;
    }
}