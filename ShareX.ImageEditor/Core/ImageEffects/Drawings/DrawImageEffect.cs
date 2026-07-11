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

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawImageEffect : ImageEffectBase
{
    private int _opacity = 100;

    public override string Id => "draw_image";
    public override string Name => "Image";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.image_plus;
    public override string Description => "Draws an image overlay on the source image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FilePath<DrawImageEffect>("image_location", "Image location", string.Empty, (e, v) => e.ImageLocation = v),
        EffectParameters.Enum<DrawImageEffect, DrawingPlacement>(
            "placement", "Placement", DrawingPlacement.TopLeft, (e, v) => e.Placement = v,
            new (string Label, DrawingPlacement Value)[]
            {
                ("Top left", DrawingPlacement.TopLeft),
                ("Top center", DrawingPlacement.TopCenter),
                ("Top right", DrawingPlacement.TopRight),
                ("Middle left", DrawingPlacement.MiddleLeft),
                ("Middle center", DrawingPlacement.MiddleCenter),
                ("Middle right", DrawingPlacement.MiddleRight),
                ("Bottom left", DrawingPlacement.BottomLeft),
                ("Bottom center", DrawingPlacement.BottomCenter),
                ("Bottom right", DrawingPlacement.BottomRight)
            }),
        EffectParameters.IntNumeric<DrawImageEffect>("offset_x", "Offset X", -10000, 10000, 0, (e, v) => e.Offset = new SKPointI(v, e.Offset.Y)),
        EffectParameters.IntNumeric<DrawImageEffect>("offset_y", "Offset Y", -10000, 10000, 0, (e, v) => e.Offset = new SKPointI(e.Offset.X, v)),
        EffectParameters.Enum<DrawImageEffect, DrawingImageSizeMode>(
            "size_mode", "Size mode", DrawingImageSizeMode.DontResize, (e, v) => e.SizeMode = v,
            new (string Label, DrawingImageSizeMode Value)[]
            {
                ("Don't resize", DrawingImageSizeMode.DontResize),
                ("Absolute size", DrawingImageSizeMode.AbsoluteSize),
                ("Percentage of watermark", DrawingImageSizeMode.PercentageOfWatermark),
                ("Percentage of canvas", DrawingImageSizeMode.PercentageOfCanvas)
            }),
        EffectParameters.IntNumeric<DrawImageEffect>("size_width", "Size width", -1, 10000, 0, (e, v) => e.Size = new SKSizeI(v, e.Size.Height)),
        EffectParameters.IntNumeric<DrawImageEffect>("size_height", "Size height", -1, 10000, 0, (e, v) => e.Size = new SKSizeI(e.Size.Width, v)),
        EffectParameters.Enum<DrawImageEffect, DrawingImageRotateFlipType>(
            "rotate_flip", "Rotate / flip", DrawingImageRotateFlipType.None, (e, v) => e.RotateFlip = v,
            new (string Label, DrawingImageRotateFlipType Value)[]
            {
                ("None", DrawingImageRotateFlipType.None),
                ("Rotate 90", DrawingImageRotateFlipType.Rotate90),
                ("Rotate 180", DrawingImageRotateFlipType.Rotate180),
                ("Rotate 270", DrawingImageRotateFlipType.Rotate270),
                ("Flip X", DrawingImageRotateFlipType.FlipX),
                ("Rotate 90 Flip X", DrawingImageRotateFlipType.Rotate90FlipX),
                ("Flip Y", DrawingImageRotateFlipType.FlipY),
                ("Rotate 90 Flip Y", DrawingImageRotateFlipType.Rotate90FlipY)
            }),
        EffectParameters.Bool<DrawImageEffect>("tile", "Tile", false, (e, v) => e.Tile = v),
        EffectParameters.Bool<DrawImageEffect>("auto_hide", "Auto hide", false, (e, v) => e.AutoHide = v),
        EffectParameters.Enum<DrawImageEffect, DrawingInterpolationMode>(
            "interpolation_mode", "Interpolation mode", DrawingInterpolationMode.HighQualityBicubic, (e, v) => e.InterpolationMode = v,
            new (string Label, DrawingInterpolationMode Value)[]
            {
                ("High quality bicubic", DrawingInterpolationMode.HighQualityBicubic),
                ("Bicubic", DrawingInterpolationMode.Bicubic),
                ("High quality bilinear", DrawingInterpolationMode.HighQualityBilinear),
                ("Bilinear", DrawingInterpolationMode.Bilinear),
                ("Nearest neighbor", DrawingInterpolationMode.NearestNeighbor)
            }),
        EffectParameters.Enum<DrawImageEffect, DrawingCompositingMode>(
            "compositing_mode", "Compositing mode", DrawingCompositingMode.SourceOver, (e, v) => e.CompositingMode = v,
            new (string Label, DrawingCompositingMode Value)[]
            {
                ("Source over", DrawingCompositingMode.SourceOver),
                ("Source copy", DrawingCompositingMode.SourceCopy)
            }),
        EffectParameters.IntSlider<DrawImageEffect>("opacity", "Opacity", 0, 100, 100, (e, v) => e.Opacity = v)
    ];

    public string ImageLocation { get; set; } = string.Empty;

    public DrawingPlacement Placement { get; set; } = DrawingPlacement.TopLeft;

    public SKPointI Offset { get; set; } = new SKPointI(0, 0);

    public DrawingImageSizeMode SizeMode { get; set; } = DrawingImageSizeMode.DontResize;

    public SKSizeI Size { get; set; } = new SKSizeI(0, 0);

    public DrawingImageRotateFlipType RotateFlip { get; set; } = DrawingImageRotateFlipType.None;

    public bool Tile { get; set; }

    public bool AutoHide { get; set; }

    public DrawingInterpolationMode InterpolationMode { get; set; } = DrawingInterpolationMode.HighQualityBicubic;

    public DrawingCompositingMode CompositingMode { get; set; } = DrawingCompositingMode.SourceOver;

    public int Opacity
    {
        get => _opacity;
        set => _opacity = Math.Clamp(value, 0, 100);
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (Opacity < 1 || (SizeMode != DrawingImageSizeMode.DontResize && Size.Width <= 0 && Size.Height <= 0))
        {
            return source.Copy();
        }

        string imagePath = DrawingEffectHelpers.ExpandVariables(ImageLocation);
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return source.Copy();
        }

        using SKBitmap? sourceWatermark = SKBitmap.Decode(imagePath);
        if (sourceWatermark is null || sourceWatermark.Width <= 0 || sourceWatermark.Height <= 0)
        {
            return source.Copy();
        }

        using SKBitmap watermark = DrawingEffectHelpers.RotateFlip(sourceWatermark, RotateFlip);

        SKSizeI imageSize = SizeMode switch
        {
            DrawingImageSizeMode.AbsoluteSize => DrawingEffectHelpers.ApplyAspectRatio(
                Size.Width == -1 ? source.Width : Size.Width,
                Size.Height == -1 ? source.Height : Size.Height,
                watermark),
            DrawingImageSizeMode.PercentageOfWatermark => DrawingEffectHelpers.ApplyAspectRatio(
                (int)Math.Round(Size.Width / 100f * watermark.Width),
                (int)Math.Round(Size.Height / 100f * watermark.Height),
                watermark),
            DrawingImageSizeMode.PercentageOfCanvas => DrawingEffectHelpers.ApplyAspectRatio(
                (int)Math.Round(Size.Width / 100f * source.Width),
                (int)Math.Round(Size.Height / 100f * source.Height),
                watermark),
            _ => new SKSizeI(watermark.Width, watermark.Height)
        };

        if (imageSize.Width <= 0 || imageSize.Height <= 0)
        {
            return source.Copy();
        }

        SKPointI imagePosition = DrawingEffectHelpers.GetPosition(
            Placement,
            Offset,
            new SKSizeI(source.Width, source.Height),
            imageSize);

        SKRectI imageRect = new SKRectI(
            imagePosition.X,
            imagePosition.Y,
            imagePosition.X + imageSize.Width,
            imagePosition.Y + imageSize.Height);

        if (AutoHide && !DrawingEffectHelpers.Contains(new SKRectI(0, 0, source.Width, source.Height), imageRect))
        {
            return source.Copy();
        }

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new SKCanvas(result);
        SKSamplingOptions sampling = DrawingEffectHelpers.GetSamplingOptions(InterpolationMode);
        using SKPaint paint = new SKPaint
        {
            IsAntialias = true,
            BlendMode = DrawingEffectHelpers.GetBlendMode(CompositingMode)
        };

        if (Opacity < 100)
        {
            byte alpha = (byte)Math.Round(255 * (Opacity / 100f));
            paint.ColorFilter = SKColorFilter.CreateBlendMode(new SKColor(255, 255, 255, alpha), SKBlendMode.Modulate);
        }

        if (Tile)
        {
            using SKImage watermarkImage = SKImage.FromBitmap(watermark);
            using SKShader shader = SKShader.CreateImage(
                watermarkImage,
                SKShaderTileMode.Repeat,
                SKShaderTileMode.Repeat,
                sampling);
            paint.Shader = shader;
            canvas.Save();
            canvas.Translate(imageRect.Left, imageRect.Top);
            canvas.DrawRect(0, 0, imageRect.Width, imageRect.Height, paint);
            canvas.Restore();
        }
        else
        {
            using SKImage watermarkImage = SKImage.FromBitmap(watermark);
            canvas.DrawImage(watermarkImage, new SKRect(imageRect.Left, imageRect.Top, imageRect.Right, imageRect.Bottom), sampling, paint);
        }

        return result;
    }
}