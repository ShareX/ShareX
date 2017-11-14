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
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class HighlightEffectShape : BaseEffectShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.EffectHighlight;

        public Color HighlightColor { get; set; }

        public override void OnConfigLoad()
        {
            HighlightColor = AnnotationOptions.HighlightColor;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.HighlightColor = HighlightColor;
        }

        public override void ApplyEffect(Bitmap bmp)
        {
            ImageHelpers.HighlightImage(bmp, HighlightColor);
        }

        public override void OnDrawOverlay(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.FromArgb(100, HighlightColor)))
            {
                g.FillRectangle(brush, Rectangle);
            }

            g.DrawCornerLines(Rectangle.Offset(1), Pens.Black, 20);

            using (Font font = new Font("Verdana", 12))
            {
                string text = "Highlight";
                Size textSize = g.MeasureString(text, font).ToSize();

                if (Rectangle.Width > textSize.Width && Rectangle.Height > textSize.Height)
                {
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(text, font, Brushes.Black, Rectangle, sf);
                    }
                }
            }
        }

        public override void OnDrawFinal(Graphics g, Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            rect.Intersect(Rectangle);

            using (Bitmap croppedImage = ImageHelpers.CropBitmap(bmp, rect))
            {
                ApplyEffect(croppedImage);

                g.DrawImage(croppedImage, rect);
            }
        }
    }
}