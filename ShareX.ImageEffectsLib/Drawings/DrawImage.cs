#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
                    Point imagePosition = Helpers.GetPosition(Placement, Offset, img.Size, img2.Size);
                    Rectangle imageRectangle = new Rectangle(imagePosition, img2.Size);

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
    }
}