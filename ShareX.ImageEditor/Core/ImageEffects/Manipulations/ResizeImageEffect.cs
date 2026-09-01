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

public enum ResizeImageEffectAspectRatioAnchor
{
    LargestDimension,
    Width,
    Height
}

public sealed class ResizeImageEffect : ImageEffectBase
{
    public override string Id => "resize_image";
    public override string Name => "Resize image";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.scaling;
    public override string Description => "Resizes the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ResizeImageEffect>("width", "Width", 0, 10000, 0, (e, v) => e.Width = v),
        EffectParameters.IntNumeric<ResizeImageEffect>("height", "Height", 0, 10000, 0, (e, v) => e.Height = v),
        EffectParameters.Bool<ResizeImageEffect>("maintain_aspect_ratio", "Maintain aspect ratio", false, (e, v) => e.MaintainAspectRatio = v)
    ];

    public int Width { get; set; }
    public int Height { get; set; }
    public bool MaintainAspectRatio { get; set; }
    public ResizeImageEffectAspectRatioAnchor AspectRatioAnchor { get; set; } = ResizeImageEffectAspectRatioAnchor.LargestDimension;

    public SKSizeI GetTargetSize(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        return ResolveTargetSize(source.Width, source.Height, Width, Height, MaintainAspectRatio, AspectRatioAnchor);
    }

    public static SKSizeI ResolveTargetSize(
        int sourceWidth,
        int sourceHeight,
        int requestedWidth,
        int requestedHeight,
        bool maintainAspectRatio,
        ResizeImageEffectAspectRatioAnchor aspectRatioAnchor = ResizeImageEffectAspectRatioAnchor.LargestDimension)
    {
        if (sourceWidth <= 0) throw new ArgumentOutOfRangeException(nameof(sourceWidth));
        if (sourceHeight <= 0) throw new ArgumentOutOfRangeException(nameof(sourceHeight));

        int width = requestedWidth > 0 ? requestedWidth : 0;
        int height = requestedHeight > 0 ? requestedHeight : 0;

        if (!maintainAspectRatio)
        {
            width = width > 0 ? width : sourceWidth;
            height = height > 0 ? height : sourceHeight;
            return new SKSizeI(width, height);
        }

        if (width <= 0 && height <= 0)
        {
            return new SKSizeI(sourceWidth, sourceHeight);
        }

        if (width <= 0)
        {
            width = Math.Max(1, (int)Math.Round((double)height / sourceHeight * sourceWidth));
            return new SKSizeI(width, height);
        }

        if (height <= 0)
        {
            height = Math.Max(1, (int)Math.Round((double)width / sourceWidth * sourceHeight));
            return new SKSizeI(width, height);
        }

        if (aspectRatioAnchor == ResizeImageEffectAspectRatioAnchor.Width)
        {
            height = Math.Max(1, (int)Math.Round((double)width / sourceWidth * sourceHeight));
            return new SKSizeI(width, height);
        }

        if (aspectRatioAnchor == ResizeImageEffectAspectRatioAnchor.Height)
        {
            width = Math.Max(1, (int)Math.Round((double)height / sourceHeight * sourceWidth));
            return new SKSizeI(width, height);
        }

        double scale = Math.Max((double)width / sourceWidth, (double)height / sourceHeight);

        return new SKSizeI(
            Math.Max(1, (int)Math.Round(sourceWidth * scale)),
            Math.Max(1, (int)Math.Round(sourceHeight * scale)));
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        SKSizeI targetSize = GetTargetSize(source);

        SKImageInfo info = new SKImageInfo(targetSize.Width, targetSize.Height, source.ColorType, source.AlphaType, source.ColorSpace);
        return source.Resize(info, new SKSamplingOptions(SKCubicResampler.CatmullRom));
    }
}