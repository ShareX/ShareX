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

public sealed class ShadowImageEffect : ImageEffectBase
{
    public override string Id => "shadow";
    public override string Name => "Shadow";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cloud_moon;
    public override string Description => "Adds a drop shadow to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ShadowImageEffect>("opacity", "Opacity", 0, 100, 80, (effect, value) => effect.Opacity = value, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0}%"),
        EffectParameters.IntSlider<ShadowImageEffect>("size", "Size", 0, 100, 20, (effect, value) => effect.Size = value, isSnapToTickEnabled: false),
        EffectParameters.IntNumeric<ShadowImageEffect>("offset_x", "Offset X", -1000, 1000, 5, (effect, value) => effect.OffsetX = value),
        EffectParameters.IntNumeric<ShadowImageEffect>("offset_y", "Offset Y", -1000, 1000, 5, (effect, value) => effect.OffsetY = value),
        EffectParameters.Color<ShadowImageEffect>("color", "Color", SKColors.Black, (effect, value) => effect.Color = value),
        EffectParameters.Bool<ShadowImageEffect>("auto_resize", "Auto resize", true, (effect, value) => effect.AutoResize = value)
    ];

    public float Opacity { get; set; } = 80f;
    public int Size { get; set; } = 20;
    public SKColor Color { get; set; } = SKColors.Black;
    public int OffsetX { get; set; } = 5;
    public int OffsetY { get; set; } = 5;
    public bool AutoResize { get; set; } = true;

    public ShadowImageEffect()
    {
    }

    public ShadowImageEffect(float opacity, int size, SKColor color, int offsetX, int offsetY, bool autoResize)
    {
        Opacity = opacity;
        Size = size;
        Color = color;
        OffsetX = offsetX;
        OffsetY = offsetY;
        AutoResize = autoResize;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int blurPad = Size;
        int expandLeft = AutoResize ? Math.Max(0, -OffsetX) + blurPad : 0;
        int expandRight = AutoResize ? Math.Max(0, OffsetX) + blurPad : 0;
        int expandTop = AutoResize ? Math.Max(0, -OffsetY) + blurPad : 0;
        int expandBottom = AutoResize ? Math.Max(0, OffsetY) + blurPad : 0;

        int newWidth = source.Width + expandLeft + expandRight;
        int newHeight = source.Height + expandTop + expandBottom;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        int imageX = expandLeft;
        int imageY = expandTop;
        int shadowX = imageX + OffsetX;
        int shadowY = imageY + OffsetY;

        SKColor shadowColor = new(Color.Red, Color.Green, Color.Blue, (byte)(255 * Opacity / 100f));

        using SKPaint shadowPaint = new()
        {
            ColorFilter = SKColorFilter.CreateBlendMode(shadowColor, SKBlendMode.SrcIn),
            ImageFilter = SKImageFilter.CreateBlur(Size / 2f, Size / 2f)
        };

        canvas.DrawBitmap(source, shadowX, shadowY, shadowPaint);
        canvas.DrawBitmap(source, imageX, imageY);

        return result;
    }
}