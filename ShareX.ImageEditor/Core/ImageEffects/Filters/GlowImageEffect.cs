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

public sealed class GlowImageEffect : ImageEffectBase
{
    public override string Id => "glow";
    public override string Name => "Glow";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.lightbulb;
    public override string Description => "Applies a glowing effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<GlowImageEffect>("size", "Size", 1, 100, 20, (effect, value) => effect.Size = value, isSnapToTickEnabled: false),
        EffectParameters.FloatSlider<GlowImageEffect>("strength", "Strength", 1, 100, 80, (effect, value) => effect.Strength = value, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0}%"),
        EffectParameters.IntSlider<GlowImageEffect>("offset_x", "Offset X", -100, 100, 0, (effect, value) => effect.OffsetX = value, isSnapToTickEnabled: false),
        EffectParameters.IntSlider<GlowImageEffect>("offset_y", "Offset Y", -100, 100, 0, (effect, value) => effect.OffsetY = value, isSnapToTickEnabled: false),
        EffectParameters.Color<GlowImageEffect>("color", "Color", SKColors.White, (effect, value) => effect.Color = value),
        EffectParameters.Bool<GlowImageEffect>("auto_resize", "Auto resize", true, (effect, value) => effect.AutoResize = value)
    ];

    public int Size { get; set; } = 20;
    public float Strength { get; set; } = 80f;
    public SKColor Color { get; set; } = SKColors.White;
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public bool AutoResize { get; set; } = true;

    public GlowImageEffect()
    {
    }

    public GlowImageEffect(int size, float strength, SKColor color, int offsetX, int offsetY, bool autoResize)
    {
        Size = size;
        Strength = strength;
        Color = color;
        OffsetX = offsetX;
        OffsetY = offsetY;
        AutoResize = autoResize;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int pad = AutoResize ? Size : 0;
        int expandLeft = AutoResize ? Math.Max(0, -OffsetX) + pad : 0;
        int expandRight = AutoResize ? Math.Max(0, OffsetX) + pad : 0;
        int expandTop = AutoResize ? Math.Max(0, -OffsetY) + pad : 0;
        int expandBottom = AutoResize ? Math.Max(0, OffsetY) + pad : 0;

        int newWidth = source.Width + expandLeft + expandRight;
        int newHeight = source.Height + expandTop + expandBottom;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        int imageX = expandLeft;
        int imageY = expandTop;
        int glowX = imageX + OffsetX;
        int glowY = imageY + OffsetY;

        SKColor glowColor = Color.WithAlpha((byte)(255 * Strength / 100f));

        using SKPaint glowPaint = new()
        {
            ColorFilter = SKColorFilter.CreateBlendMode(glowColor, SKBlendMode.SrcIn),
            ImageFilter = SKImageFilter.CreateBlur(Size, Size)
        };

        canvas.DrawBitmap(source, glowX, glowY, glowPaint);
        canvas.DrawBitmap(source, imageX, imageY);

        return result;
    }
}