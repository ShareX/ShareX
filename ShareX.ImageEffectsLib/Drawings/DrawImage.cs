#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
        [DefaultValue(ContentAlignment.BottomRight)]
        public ContentAlignment Placement { get; set; }

        private Point offset;

        [DefaultValue(typeof(Point), "5, 5")]
        public Point Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = new Point(value.X.Min(0), value.Y.Min(0));
            }
        }

        [DefaultValue(true), Description("If image watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        [DefaultValue(""), Editor(typeof(ImageFileNameEditor), typeof(UITypeEditor))]
        public string ImageLocation { get; set; }

        [DefaultValue(DrawImageSizeMode.DontResize), Description("How the image watermark should be rescaled, if at all.")]
        public DrawImageSizeMode SizeMode { get; set; }

        [DefaultValue(typeof(Size), "0, 0")]
        public Size Size { get; set; }

        public DrawImage()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (!string.IsNullOrEmpty(ImageLocation) && File.Exists(ImageLocation))
            {
                using (Image img2 = ImageHelpers.LoadImage(ImageLocation))
                {
                    //Calculate size first
                    Size imageSize = img2.Size;
                    if (SizeMode == DrawImageSizeMode.AbsoluteSize)
                    {
                        //Use Size property
                        imageSize = Size;
                    }
                    else if (SizeMode == DrawImageSizeMode.PercentageOfWatermark)
                    {
                        //Relative size (percentage of watermark)
                        imageSize = new Size((int)(img2.Width * (Size.Width / 100.0)), (int)(img2.Height * (Size.Height / 100.0)));
                    }
                    else if (SizeMode == DrawImageSizeMode.PercentageOfCanvas)
                    {
                        //Relative size (percentage of image)
                        imageSize = new Size((int)(img.Width * (Size.Width / 100.0)), (int)(img.Height * (Size.Height / 100.0)));
                    }

                    //Place the image
                    Point imagePosition = Helpers.GetPosition(Placement, Offset, img.Size, imageSize);
                    Rectangle imageRectangle = new Rectangle(imagePosition, imageSize);

                    if (AutoHide && !new Rectangle(0, 0, img.Width, img.Height).Contains(imageRectangle))
                    {
                        return img;
                    }

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.SetHighQuality();
                        g.DrawImage(img2, imageRectangle);
                    }
                }
            }

            return img;
        }

        public enum DrawImageSizeMode
        {
            DontResize,
            AbsoluteSize,
            PercentageOfWatermark,
            PercentageOfCanvas
        }
    }
}