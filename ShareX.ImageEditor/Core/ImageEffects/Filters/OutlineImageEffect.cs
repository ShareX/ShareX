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

public sealed class OutlineImageEffect : ImageEffectBase
{
    public override string Id => "outline";
    public override string Name => "Outline";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.square_dashed;
    public override string Description => "Applies an outline effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<OutlineImageEffect>("size", "Size", 1, 50, 3, (effect, value) => effect.Size = value, isSnapToTickEnabled: false),
        EffectParameters.IntSlider<OutlineImageEffect>("padding", "Padding", 0, 100, 0, (effect, value) => effect.Padding = value, isSnapToTickEnabled: false),
        EffectParameters.Bool<OutlineImageEffect>("outline_only", "Outline only", false, (effect, value) => effect.OutlineOnly = value),
        EffectParameters.Color<OutlineImageEffect>("color", "Color", SKColors.Black, (effect, value) => effect.Color = value)
    ];

    public int Size { get; set; } = 3;
    public int Padding { get; set; }
    public bool OutlineOnly { get; set; }
    public SKColor Color { get; set; } = SKColors.Black;

    public OutlineImageEffect()
    {
    }

    public OutlineImageEffect(int size, int padding, bool outlineOnly, SKColor color)
    {
        Size = size;
        Padding = padding;
        OutlineOnly = outlineOnly;
        Color = color;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Size <= 0) return source.Copy();

        int totalExpand = Size + Padding;
        int newWidth = source.Width + totalExpand * 2;
        int newHeight = source.Height + totalExpand * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Create the full outline footprint first, then carve the inner gap if needed.
        using SKImageFilter outerDilate = SKImageFilter.CreateDilate(Size + Padding, Size + Padding);
        using SKImageFilter tintFilter = SKImageFilter.CreateColorFilter(
            SKColorFilter.CreateBlendMode(Color, SKBlendMode.SrcIn));
        using SKImageFilter outlineFilter = SKImageFilter.CreateCompose(tintFilter, outerDilate);
        using SKPaint outlinePaint = new() { ImageFilter = outlineFilter };
        canvas.DrawBitmap(source, totalExpand, totalExpand, outlinePaint);

        if (Padding > 0)
        {
            using SKImageFilter gapDilate = SKImageFilter.CreateDilate(Padding, Padding);
            using SKPaint erasePaint = new()
            {
                ImageFilter = gapDilate,
                BlendMode = SKBlendMode.DstOut
            };
            canvas.DrawBitmap(source, totalExpand, totalExpand, erasePaint);
        }

        if (OutlineOnly)
        {
            using SKPaint holeErasePaint = new() { BlendMode = SKBlendMode.DstOut };
            canvas.DrawBitmap(source, totalExpand, totalExpand, holeErasePaint);
        }
        else
        {
            canvas.DrawBitmap(source, totalExpand, totalExpand);
        }

        return result;
    }
}