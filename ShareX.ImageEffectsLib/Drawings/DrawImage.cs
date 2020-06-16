#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;

namespace ShareX.ImageEffectsLib
{
    [Description("Image watermark")]
    public class DrawImage : ImageEffect
    {
        [DefaultValue(""), Editor(typeof(ImageFileNameEditor), typeof(UITypeEditor))]
        public string ImageLocation { get; set; }

        [DefaultValue(ContentAlignment.BottomRight)]
        public ContentAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "5, 5")]
        public Point Offset { get; set; }

        [DefaultValue(DrawImageSizeMode.DontResize), Description("How the image watermark should be rescaled, if at all.")]
        public DrawImageSizeMode SizeMode { get; set; }

        [DefaultValue(typeof(Size), "0, 0")]
        public Size Size { get; set; }

        [DefaultValue(true), Description("If image watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        public DrawImage()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            string imageFilePath = Helpers.ExpandFolderVariables(ImageLocation, true);

            if (!string.IsNullOrEmpty(imageFilePath) && File.Exists(imageFilePath))
            {
                using (Bitmap bmp2 = ImageHelpers.LoadImage(imageFilePath))
                {
                    if (bmp2 != null)
                    {
                        // Calculate size first
                        Size imageSize = bmp2.Size;
                        if (SizeMode == DrawImageSizeMode.AbsoluteSize)
                        {
                            // Use Size property
                            imageSize = Size;
                        }
                        else if (SizeMode == DrawImageSizeMode.PercentageOfWatermark)
                        {
                            // Relative size (percentage of watermark)
                            imageSize = new Size((int)(bmp2.Width * (Size.Width / 100.0)), (int)(bmp2.Height * (Size.Height / 100.0)));
                        }
                        else if (SizeMode == DrawImageSizeMode.PercentageOfCanvas)
                        {
                            // Relative size (percentage of image)
                            imageSize = new Size((int)(bmp.Width * (Size.Width / 100.0)), (int)(bmp.Height * (Size.Height / 100.0)));
                        }

                        // Place the image
                        Point imagePosition = Helpers.GetPosition(Placement, Offset, bmp.Size, imageSize);

                        Rectangle imageRectangle = new Rectangle(imagePosition, imageSize);

                        if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(imageRectangle))
                        {
                            return bmp;
                        }

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.SetHighQuality();
                            g.DrawImage(bmp2, imageRectangle);
                        }
                    }
                }
            }

            return bmp;
        }
    }
}