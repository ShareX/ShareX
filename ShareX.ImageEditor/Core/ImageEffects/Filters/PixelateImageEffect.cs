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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class PixelateImageEffect : ImageEffectBase
{
    public override string Id => "pixelate";
    public override string Name => "Pixelate";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.grid_2x2;
    public override string Description => "Pixelates the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<PixelateImageEffect>("size", "Size", 1, 200, 10, (effect, value) => effect.Size = value)
    ];

    public int Size { get; set; } = 10;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Size <= 1) return source.Copy();

        // Downscale then upscale to create pixelation effect
        int smallWidth = Math.Max(1, source.Width / Size);
        int smallHeight = Math.Max(1, source.Height / Size);

        SKBitmap small = new SKBitmap(smallWidth, smallHeight);
        using (SKCanvas smallCanvas = new SKCanvas(small))
        {
            smallCanvas.DrawBitmap(source, new SKRect(0, 0, smallWidth, smallHeight));
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas canvas = new SKCanvas(result))
        {
            using SKPaint paint = new SKPaint();
            using SKImage smallImage = SKImage.FromBitmap(small);
            canvas.DrawImage(smallImage, new SKRect(0, 0, source.Width, source.Height), new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None), paint);
        }
        small.Dispose();
        return result;
    }
}