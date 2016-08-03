#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

namespace ShareX.ScreenCaptureLib
{
    public class BlurEffectShape : BaseEffectShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingBlur;

        public int BlurRadius { get; set; }

        public override void OnConfigLoad()
        {
            BlurRadius = AnnotationOptions.BlurRadius;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.BlurRadius = BlurRadius;
        }

        public override void OnDraw(Graphics g)
        {
            if (BlurRadius > 1)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(150, Color.Black)))
                {
                    g.FillRectangle(brush, Rectangle);
                }

                g.DrawCornerLines(Rectangle.Offset(1), Pens.White, 20);

                using (Font font = new Font("Verdana", 12))
                {
                    string text = $"Blur ({BlurRadius})";
                    Size textSize = g.MeasureString(text, font).ToSize();

                    if (Rectangle.Width > textSize.Width && Rectangle.Height > textSize.Height)
                    {
                        using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(text, font, Brushes.White, Rectangle, sf);
                        }
                    }
                }
            }
        }

        public override void OnDrawFinal(Graphics g, Bitmap bmp)
        {
            if (BlurRadius > 1)
            {
                using (Bitmap croppedImage = ImageHelpers.CropBitmap(bmp, Rectangle))
                {
                    ImageHelpers.Blur(croppedImage, BlurRadius);

                    g.DrawImage(croppedImage, Rectangle);
                }
            }
        }
    }
}