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

public sealed class ColorizeImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "colorize";
    public override string Name => "Colorize";
    public override string IconKey => LucideIcons.palette;
    public override string Description => "Colorizes the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<ColorizeImageEffect>("color", "Color", SKColors.Orange, (effect, value) => effect.Color = value),
        EffectParameters.FloatSlider<ColorizeImageEffect>("strength", "Strength", 0, 100, 50, (effect, value) => effect.Strength = value)
    ];

    public SKColor Color { get; set; } = SKColors.Red; // Default
    public float Strength { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        float strength = Strength;
        if (strength <= 0) return source.Copy();
        if (strength > 100) strength = 100;

        using var paint = new SKPaint();

        var grayscaleMatrix = new float[] {
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0,       0,       0,       1, 0
        };
        using var grayscale = SKColorFilter.CreateColorMatrix(grayscaleMatrix);
        using var tint = SKColorFilter.CreateBlendMode(Color, SKBlendMode.Modulate);
        using var composed = SKColorFilter.CreateCompose(tint, grayscale);

        paint.ColorFilter = composed;

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas canvas = new SKCanvas(result))
        {
            canvas.Clear(SKColors.Transparent);

            if (strength >= 100)
            {
                canvas.DrawBitmap(source, 0, 0, paint);
            }
            else
            {
                canvas.DrawBitmap(source, 0, 0);
                paint.Color = new SKColor(255, 255, 255, (byte)(255 * (strength / 100f)));
                canvas.DrawBitmap(source, 0, 0, paint);
            }
        }
        return result;
    }
}