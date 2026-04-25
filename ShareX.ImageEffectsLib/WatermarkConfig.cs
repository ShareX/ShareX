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

using System.ComponentModel;
using System.Drawing;

namespace ShareX.ImageEffectsLib
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WatermarkConfig
    {
        [Category("General"), Description("Type of watermark")]
        public WatermarkType Type { get; set; } = WatermarkType.Text;

        [Category("General"), Description("Watermark placement")]
        public ContentAlignment Placement { get; set; } = ContentAlignment.BottomRight;

        [Category("General"), Description("Watermark offset from edge")]
        public int Offset { get; set; } = 5;

        [Category("Text"), Description("The text to display as watermark")]
        public string Text
        {
            get => TextSettings.Text;
            set => TextSettings.Text = value;
        }

        [Category("Text"), Description("Detailed text watermark settings"), DisplayName("Text settings")]
        public DrawText TextSettings { get; set; } = new DrawText { DrawTextShadow = false };

        [Category("Image"), Description("The image file to use as watermark"), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ImagePath
        {
            get => ImageSettings.ImageLocation;
            set => ImageSettings.ImageLocation = value;
        }

        [Category("Image"), Description("Detailed image watermark settings"), DisplayName("Image settings")]
        public DrawImage ImageSettings { get; set; } = new DrawImage();

        public Bitmap Apply(Bitmap bmp)
        {
            TextSettings.Placement = ImageSettings.Placement = Placement;
            TextSettings.Offset = ImageSettings.Offset = new Point(Offset, Offset);

            switch (Type)
            {
                default:
                case WatermarkType.Text:
                    return TextSettings.Apply(bmp);
                case WatermarkType.Image:
                    return ImageSettings.Apply(bmp);
            }
        }
    }
}