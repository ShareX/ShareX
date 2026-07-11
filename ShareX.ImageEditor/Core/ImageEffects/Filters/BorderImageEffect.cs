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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class BorderImageEffect : ImageEffectBase
{
    public override string Id => "border";
    public override string Name => "Border";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.frame;
    public override string Description => "Adds a border to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<BorderImageEffect, ImageHelpers.BorderType>(
            "type",
            "Type",
            ImageHelpers.BorderType.Outside,
            (effect, value) => effect.Type = value,
            new (string Label, ImageHelpers.BorderType Value)[]
            {
                ("Outside", ImageHelpers.BorderType.Outside),
                ("Inside", ImageHelpers.BorderType.Inside)
            }),
        EffectParameters.IntSlider<BorderImageEffect>("size", "Size", 1, 100, 5, (effect, value) => effect.Size = value),
        EffectParameters.Enum<BorderImageEffect, ImageHelpers.DashStyle>(
            "dash_style",
            "Dash style",
            ImageHelpers.DashStyle.Solid,
            (effect, value) => effect.DashStyle = value,
            new (string Label, ImageHelpers.DashStyle Value)[]
            {
                ("Solid", ImageHelpers.DashStyle.Solid),
                ("Dash", ImageHelpers.DashStyle.Dash),
                ("Dot", ImageHelpers.DashStyle.Dot),
                ("Dash Dot", ImageHelpers.DashStyle.DashDot)
            }),
        EffectParameters.Color<BorderImageEffect>("color", "Color", SKColors.Black, (effect, value) => effect.Color = value)
    ];

    public ImageHelpers.BorderType Type { get; set; } = ImageHelpers.BorderType.Outside;
    public int Size { get; set; } = 5;
    public ImageHelpers.DashStyle DashStyle { get; set; } = ImageHelpers.DashStyle.Solid;
    public SKColor Color { get; set; } = SKColors.Black;

    public BorderImageEffect()
    {
    }

    public BorderImageEffect(ImageHelpers.BorderType type, int size, ImageHelpers.DashStyle dashStyle, SKColor color)
    {
        Type = type;
        Size = size;
        DashStyle = dashStyle;
        Color = color;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Size <= 0) return source.Copy();

        int newWidth = Type == ImageHelpers.BorderType.Outside ? source.Width + Size * 2 : source.Width;
        int newHeight = Type == ImageHelpers.BorderType.Outside ? source.Height + Size * 2 : source.Height;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        int offsetX = Type == ImageHelpers.BorderType.Outside ? Size : 0;
        int offsetY = Type == ImageHelpers.BorderType.Outside ? Size : 0;

        canvas.DrawBitmap(source, offsetX, offsetY);

        using SKPaint paint = new()
        {
            Color = Color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Size,
            IsAntialias = true
        };

        float[]? intervals = DashStyle switch
        {
            ImageHelpers.DashStyle.Dash => [Size * 3, Size],
            ImageHelpers.DashStyle.Dot => [Size, Size],
            ImageHelpers.DashStyle.DashDot => [Size * 3, Size, Size, Size],
            _ => null
        };

        if (intervals != null)
        {
            paint.PathEffect = SKPathEffect.CreateDash(intervals, 0);
        }

        float halfStroke = Size / 2f;
        SKRect borderRect = Type == ImageHelpers.BorderType.Outside
            ? new SKRect(halfStroke, halfStroke, newWidth - halfStroke, newHeight - halfStroke)
            : new SKRect(halfStroke, halfStroke, source.Width - halfStroke, source.Height - halfStroke);

        canvas.DrawRect(borderRect, paint);
        return result;
    }
}