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

        [DefaultValue(ImageContentAlignment.TopLeft)]
        public ImageContentAlignment Placement { get; set; }

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

            if (ImageEffectPathHelpers.TryGetSafeLocalFilePath(ImageLocation, out string imageFilePath) && File.Exists(imageFilePath))
            {
                using (SKBitmap bmpWatermark = SkiaImageHelpers.LoadImage(imageFilePath))
                {
                    if (bmpWatermark != null)
                    {
                        if (RotateFlip != ImageRotateFlipType.None)
                        {
                            bmpWatermark.RotateFlipInPlace((int)RotateFlip);
                        }

                        Size imageSize;

                        if (SizeMode == DrawImageSizeMode.AbsoluteSize)
                        {
                            int width = Size.Width == -1 ? bmp.Width : Size.Width;
                            int height = Size.Height == -1 ? bmp.Height : Size.Height;
                            imageSize = SkiaImageHelpers.ApplyAspectRatio(width, height, bmpWatermark);
                        }
                        else if (SizeMode == DrawImageSizeMode.PercentageOfWatermark)
                        {
                            int width = (int)Math.Round(Size.Width / 100f * bmpWatermark.Width);
                            int height = (int)Math.Round(Size.Height / 100f * bmpWatermark.Height);
                            imageSize = SkiaImageHelpers.ApplyAspectRatio(width, height, bmpWatermark);
                        }
                        else if (SizeMode == DrawImageSizeMode.PercentageOfCanvas)
                        {
                            int width = (int)Math.Round(Size.Width / 100f * bmp.Width);
                            int height = (int)Math.Round(Size.Height / 100f * bmp.Height);
                            imageSize = SkiaImageHelpers.ApplyAspectRatio(width, height, bmpWatermark);
                        }
                        else
                        {
                            imageSize = bmpWatermark.GetSize();
                        }

                        Point imagePosition = Helpers.GetPosition(Placement, Offset, bmp.GetSize(), imageSize);
                        Rectangle imageRectangle = new Rectangle(imagePosition, imageSize);

                        if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(imageRectangle))
                        {
                            return bmp;
                        }

                        using (SKCanvas canvas = new(bmp))
                        using (SKPaint paint = new())
                        {
                            paint.BlendMode = CompositingMode == ImageCompositingMode.SourceCopy ? SKBlendMode.Src : SKBlendMode.SrcOver;
                            if (Tile)
                            {
                                using SKPaint texture = SkiaDrawing.Texture(bmpWatermark, ImageTileMode.Tile);
                                texture.BlendMode = paint.BlendMode;
                                texture.Translate(imageRectangle.X, imageRectangle.Y);
                                canvas.FillRectangle(texture, imageRectangle);
                            }
                            else
                            {
                                if (Opacity < 100) paint.Color = SKColors.White.WithAlpha((byte)Math.Round(Opacity * 2.55));
                                using SKImage image = SKImage.FromBitmap(bmpWatermark);
                                canvas.DrawImage(image, imageRectangle.ToSKRect(), SkiaImageHelpers.GetSampling(InterpolationMode), paint);
                            }
                        }
                    }
                }
            }

            return bmp;
        }

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
