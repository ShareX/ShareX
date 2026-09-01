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

public sealed class AutoCropImageEffect : ImageEffectBase
{
    public override string Id => "auto_crop_image";
    public override string Name => "Auto crop image";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.scan;
    public override string Description => "Automatically crops the image using tolerance on edge pixels.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<AutoCropImageEffect>("tolerance", "Tolerance", 0, 255, 0, (e, v) => e.Tolerance = v)
    ];

    private SKColor _color;
    private int _tolerance;

    // Exposed for schema-driven dialog parameter binding.
    public SKColor Color
    {
        get => _color;
        set => _color = value;
    }

    // Exposed for schema-driven dialog parameter binding.
    public int Tolerance
    {
        get => _tolerance;
        set => _tolerance = value;
    }

    public AutoCropImageEffect(SKColor color, int tolerance = 0)
    {
        _color = color;
        _tolerance = tolerance;
    }

    public AutoCropImageEffect()
    {
        _color = SKColors.Transparent;
        _tolerance = 0;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;

        // Dialog/catalog definitions historically pass `Transparent` for the match color.
        // In that case, align behavior with `EditorCore.AutoCrop()` by using the source's top-left pixel.
        SKColor matchColor = _color == SKColors.Transparent && width > 0 && height > 0
            ? source.GetPixel(0, 0)
            : _color;

        int minX = width, minY = height, maxX = 0, maxY = 0;
        bool hasContent = false;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SKColor pixel = source.GetPixel(x, y);
                if (!ImageHelpers.ColorsMatch(pixel, matchColor, _tolerance))
                {
                    hasContent = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!hasContent)
        {
            return new SKBitmap(1, 1, source.ColorType, source.AlphaType);
        }

        int cropWidth = maxX - minX + 1;
        int cropHeight = maxY - minY + 1;

        return ImageHelpers.Crop(source, minX, minY, cropWidth, cropHeight);
    }
}