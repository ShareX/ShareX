#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImageEffectsLib
{
    [Description("Image")]
    internal class DrawImage : ImageEffect
    {
        [DefaultValue(""), Editor(typeof(ImageFileNameEditor), typeof(UITypeEditor))]
        public string ImageLocation { get; set; }

        [DefaultValue(PositionType.Bottom_Right)]
        public PositionType Position { get; set; }

        [DefaultValue(5)]
        public int Offset { get; set; }

        [DefaultValue(false), Description("If image size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        public DrawImage()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (!string.IsNullOrEmpty(ImageLocation) && File.Exists(ImageLocation))
            {
                using (Image img2 = Helpers.GetImageFromFile(ImageLocation))
                {
                    if (AutoHide && ((img2.Width + Offset > img.Width) || (img2.Height + Offset > img.Height)))
                    {
                        return img;
                    }

                    Point pos = Helpers.GetPosition(Position, Offset, img.Size, img2.Size);

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.SetHighQuality();
                        g.DrawImage(img2, pos.X, pos.Y, img2.Width, img2.Height);
                    }
                }
            }

            return img;
        }
    }
}