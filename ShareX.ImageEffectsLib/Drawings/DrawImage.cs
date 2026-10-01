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

using ShareX.HelpersLib;
using SkiaSharp;
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ImageEffectsLib
{
    [Description("Image")]
    public class DrawImage : ImageEffect
    {
        [DefaultValue("")]
        public string ImageLocation { get; set; }

        [DefaultValue(ImageAlignment.TopLeft)]
        public ImageAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "0, 0")]
        public Point Offset { get; set; }

        [DefaultValue(DrawImageSizeMode.DontResize), Description("How the image watermark should be rescaled, if at all.")]
        public DrawImageSizeMode SizeMode { get; set; }

        [DefaultValue(typeof(Size), "0, 0")]
        public Size Size { get; set; }

        [DefaultValue(ImageRotateFlipType.None)]
        public ImageRotateFlipType RotateFlip { get; set; }

        [DefaultValue(false)]
        public bool Tile { get; set; }

        [DefaultValue(false), Description("If image watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        [DefaultValue(ImageInterpolationMode.HighQualityBicubic)]
        public ImageInterpolationMode InterpolationMode { get; set; }

        [DefaultValue(ImageCompositingMode.SourceOver)]
        public ImageCompositingMode CompositingMode { get; set; }

        private int opacity;

        [DefaultValue(100)]
        public int Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                opacity = value.Clamp(0, 100);
            }
        }

        public DrawImage()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override SKBitmap Apply(SKBitmap bmp)
        {
            if (Opacity < 1 || (SizeMode != DrawImageSizeMode.DontResize && Size.Width <= 0 && Size.Height <= 0))
            {
                return bmp;
            }

            if (!ImageEffectPathHelpers.TryGetSafeLocalFilePath(ImageLocation, out string imageFilePath) || !File.Exists(imageFilePath))
            {
                return bmp;
            }

            SKBitmap watermark = SkiaImageHelpers.LoadImage(imageFilePath);

            if (watermark == null)
            {
                return bmp;
            }

            if (RotateFlip != ImageRotateFlipType.None)
            {
                using SKBitmap unrotated = watermark;
                watermark = SkiaImageHelpers.RotateFlip(unrotated, (int)RotateFlip);
            }

            using (watermark)
            {
                Size imageSize;

                if (SizeMode == DrawImageSizeMode.AbsoluteSize)
                {
                    int width = Size.Width == -1 ? bmp.Width : Size.Width;
                    int height = Size.Height == -1 ? bmp.Height : Size.Height;
                    imageSize = SkiaImageHelpers.ApplyAspectRatio(width, height, watermark);
                }
                else if (SizeMode == DrawImageSizeMode.PercentageOfWatermark)
                {
                    int width = (int)Math.Round(Size.Width / 100f * watermark.Width);
                    int height = (int)Math.Round(Size.Height / 100f * watermark.Height);
                    imageSize = SkiaImageHelpers.ApplyAspectRatio(width, height, watermark);
                }
                else if (SizeMode == DrawImageSizeMode.PercentageOfCanvas)
                {
                    int width = (int)Math.Round(Size.Width / 100f * bmp.Width);
                    int height = (int)Math.Round(Size.Height / 100f * bmp.Height);
                    imageSize = SkiaImageHelpers.ApplyAspectRatio(width, height, watermark);
                }
                else
                {
                    imageSize = new Size(watermark.Width, watermark.Height);
                }

                Point imagePosition = SkiaImageHelpers.GetPosition(Placement, Offset, new Size(bmp.Width, bmp.Height), imageSize);
                Rectangle imageRectangle = new Rectangle(imagePosition, imageSize);

                if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(imageRectangle))
                {
                    return bmp;
                }

                SKRect destination = new SKRect(imageRectangle.Left, imageRectangle.Top, imageRectangle.Right, imageRectangle.Bottom);
                SKSamplingOptions sampling = GetSampling(InterpolationMode);

                using (SKCanvas canvas = new SKCanvas(bmp))
                using (SKPaint paint = new SKPaint { BlendMode = CompositingMode == ImageCompositingMode.SourceCopy ? SKBlendMode.Src : SKBlendMode.SrcOver })
                {
                    if (Tile)
                    {
                        using SKShader shader = SKShader.CreateBitmap(watermark, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat,
                            SKMatrix.CreateTranslation(imageRectangle.X, imageRectangle.Y));
                        paint.Shader = shader;
                        canvas.DrawRect(destination, paint);
                    }
                    else
                    {
                        if (Opacity < 100)
                        {
                            paint.Color = SKColors.White.WithAlpha((byte)Math.Round(Opacity / 100f * 255));
                        }

                        using SKImage image = SKImage.FromBitmap(watermark);
                        canvas.DrawImage(image, destination, sampling, paint);
                    }
                }
            }

            return bmp;
        }

        private static SKSamplingOptions GetSampling(ImageInterpolationMode mode) => mode switch
        {
            ImageInterpolationMode.NearestNeighbor => new SKSamplingOptions(SKFilterMode.Nearest),
            ImageInterpolationMode.Bilinear or ImageInterpolationMode.HighQualityBilinear => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
            _ => new SKSamplingOptions(SKCubicResampler.Mitchell)
        };

        protected override string GetSummary()
        {
            if (!string.IsNullOrEmpty(ImageLocation))
            {
                return FileHelpers.GetFileNameSafe(ImageLocation);
            }

            return null;
        }
    }
}
