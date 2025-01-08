#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public class ImageBeautifier : IDisposable
    {
        public ImageBeautifierOptions Options { get; set; }
        public Bitmap SourceImage { get; private set; }
        public Bitmap SourceImageCropped { get; private set; }
        public Color PaddingColor { get; private set; }

        public ImageBeautifier()
        {
        }

        public ImageBeautifier(ImageBeautifierOptions options)
        {
            Options = options;
        }

        public ImageBeautifier(Bitmap image, ImageBeautifierOptions options) : this(options)
        {
            LoadImage(image);
        }

        public void Dispose()
        {
            SourceImage?.Dispose();
            SourceImageCropped?.Dispose();
        }

        public void LoadImage(Bitmap image)
        {
            SourceImage = (Bitmap)image.Clone();
            SourceImageCropped = null;

            Rectangle source = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
            Rectangle rect = ImageHelpers.FindAutoCropRectangle(SourceImage, true, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            if (source != rect)
            {
                SourceImageCropped = ImageHelpers.CropBitmap(SourceImage, rect);
            }

            PaddingColor = SourceImage.GetPixel(0, 0);
        }

        public Bitmap Render()
        {
            Bitmap resultImage;

            if (Options.SmartPadding && SourceImageCropped != null)
            {
                resultImage = (Bitmap)SourceImageCropped.Clone();
            }
            else
            {
                resultImage = (Bitmap)SourceImage.Clone();
            }

            if (Options.Padding > 0)
            {
                Bitmap resultImageNew = ImageHelpers.AddCanvas(resultImage, Options.Padding, PaddingColor);
                resultImage.Dispose();
                resultImage = resultImageNew;
            }

            if (Options.RoundedCorner > 0)
            {
                resultImage = ImageHelpers.RoundedCorners(resultImage, Options.RoundedCorner);
            }

            if (Options.Margin > 0)
            {
                Bitmap resultImageNew = ImageHelpers.AddCanvas(resultImage, Options.Margin);
                resultImage.Dispose();
                resultImage = resultImageNew;
            }

            if (Options.ShadowOpacity > 0 && (Options.ShadowRadius > 0 || Options.ShadowDistance > 0))
            {
                float shadowOpacity = Options.ShadowOpacity / 100f;
                Point shadowOffset = (Point)MathHelpers.DegreeToVector2(Options.ShadowAngle - 90, Options.ShadowDistance);
                resultImage = ImageHelpers.AddShadow(resultImage, shadowOpacity, Options.ShadowRadius, 0f, Options.ShadowColor, shadowOffset, false);
            }

            switch (Options.BackgroundType)
            {
                case ImageBeautifierBackgroundType.Gradient:
                    if (Options.BackgroundGradient != null && Options.BackgroundGradient.IsVisible)
                    {
                        Bitmap resultImageNew = ImageHelpers.FillBackground(resultImage, Options.BackgroundGradient);
                        resultImage.Dispose();
                        resultImage = resultImageNew;
                    }
                    break;
                case ImageBeautifierBackgroundType.Color:
                    if (!Options.BackgroundColor.IsTransparent())
                    {
                        Bitmap resultImageNew = ImageHelpers.FillBackground(resultImage, Options.BackgroundColor);
                        resultImage.Dispose();
                        resultImage = resultImageNew;
                    }
                    break;
                case ImageBeautifierBackgroundType.Image:
                    resultImage = ImageHelpers.DrawBackgroundImage(resultImage, Options.BackgroundImageFilePath);
                    break;
                case ImageBeautifierBackgroundType.Desktop:
                    string desktopWallpaperFilePath = Helpers.GetDesktopWallpaperFilePath();
                    resultImage = ImageHelpers.DrawBackgroundImage(resultImage, desktopWallpaperFilePath);
                    break;
                default:
                case ImageBeautifierBackgroundType.Transparent:
                    break;
            }

            return resultImage;
        }

        public async Task<Bitmap> RenderAsync()
        {
            return await Task.Run(Render);
        }
    }
}