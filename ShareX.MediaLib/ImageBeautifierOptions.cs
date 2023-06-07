#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public class ImageBeautifierOptions
    {
        public int Margin { get; set; }
        public int Padding { get; set; }
        public bool SmartPadding { get; set; }
        public int RoundedCorner { get; set; }
        public int ShadowSize { get; set; }
        public int ShadowOpacity { get; set; }
        public int ShadowDistance { get; set; }
        public int ShadowAngle { get; set; }
        public Color ShadowColor { get; set; }
        public ImageBeautifierBackgroundType BackgroundType { get; set; }
        public GradientInfo BackgroundGradient { get; set; }
        public Color BackgroundColor { get; set; }
        public string BackgroundImageFilePath { get; set; }

        public ImageBeautifierOptions()
        {
            ResetOptions();
        }

        public void ResetOptions()
        {
            Margin = 80;
            Padding = 40;
            SmartPadding = true;
            RoundedCorner = 20;
            ShadowSize = 30;
            ShadowOpacity = 100;
            ShadowDistance = 0;
            ShadowAngle = -90;
            ShadowColor = Color.Black;
            BackgroundType = ImageBeautifierBackgroundType.Gradient;
            BackgroundGradient = new GradientInfo(LinearGradientMode.ForwardDiagonal, Color.FromArgb(255, 81, 47), Color.FromArgb(221, 36, 118));
            BackgroundColor = Color.FromArgb(34, 34, 34);
            BackgroundImageFilePath = "";
        }

        public Bitmap Render(Bitmap image)
        {
            Bitmap resultImage = (Bitmap)image.Clone();

            if (SmartPadding)
            {
                resultImage = ImageHelpers.AutoCropImage(resultImage, true, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, Padding);
            }
            else if (Padding > 0)
            {
                Color color = resultImage.GetPixel(0, 0);
                Bitmap resultImageNew = ImageHelpers.AddCanvas(resultImage, Padding, color);
                resultImage.Dispose();
                resultImage = resultImageNew;
            }

            if (RoundedCorner > 0)
            {
                resultImage = ImageHelpers.RoundedCorners(resultImage, RoundedCorner);
            }

            if (Margin > 0)
            {
                Bitmap resultImageNew = ImageHelpers.AddCanvas(resultImage, Margin);
                resultImage.Dispose();
                resultImage = resultImageNew;
            }

            if (ShadowOpacity > 0 && (ShadowSize > 0 || ShadowDistance > 0))
            {
                float shadowOpacity = ShadowOpacity / 100f;
                Point shadowOffset = (Point)MathHelpers.DegreeToVector2(ShadowAngle, ShadowDistance);
                resultImage = ImageHelpers.AddShadow(resultImage, shadowOpacity, ShadowSize, 0f, ShadowColor, shadowOffset, false);
            }

            switch (BackgroundType)
            {
                case ImageBeautifierBackgroundType.Gradient:
                    if (BackgroundGradient != null && BackgroundGradient.IsVisible)
                    {
                        Bitmap resultImageNew = ImageHelpers.FillBackground(resultImage, BackgroundGradient);
                        resultImage.Dispose();
                        resultImage = resultImageNew;
                    }
                    break;
                case ImageBeautifierBackgroundType.Color:
                    if (!BackgroundColor.IsTransparent())
                    {
                        Bitmap resultImageNew = ImageHelpers.FillBackground(resultImage, BackgroundColor);
                        resultImage.Dispose();
                        resultImage = resultImageNew;
                    }
                    break;
                case ImageBeautifierBackgroundType.Image:
                    resultImage = ImageHelpers.DrawBackgroundImage(resultImage, BackgroundImageFilePath);
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

        public async Task<Bitmap> RenderAsync(Bitmap image)
        {
            return await Task.Run(() => Render(image));
        }
    }
}