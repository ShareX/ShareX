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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class CropImageEffect : ImageEffectBase
{
    public override string Id => "crop_image";
    public override string Name => "Crop image";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.crop;
    public override string Description => "Crops the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<CropImageEffect>("x", "X", 0, 100000, 0, (e, v) => e.X = v),
        EffectParameters.IntNumeric<CropImageEffect>("y", "Y", 0, 100000, 0, (e, v) => e.Y = v),
        EffectParameters.IntNumeric<CropImageEffect>("width", "Width", 1, 100000, 100, (e, v) => e.Width = v),
        EffectParameters.IntNumeric<CropImageEffect>("height", "Height", 1, 100000, 100, (e, v) => e.Height = v)
    ];

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cropX = Math.Clamp(X, 0, source.Width);
        int cropY = Math.Clamp(Y, 0, source.Height);
        int cropW = Math.Clamp(Width, 0, source.Width - cropX);
        int cropH = Math.Clamp(Height, 0, source.Height - cropY);

        if (cropW <= 0 || cropH <= 0)
        {
            return source.Copy();
        }

        SKBitmap result = new SKBitmap(cropW, cropH, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(source, new SKRect(cropX, cropY, cropX + cropW, cropY + cropH), new SKRect(0, 0, cropW, cropH));
        return result;
    }
}