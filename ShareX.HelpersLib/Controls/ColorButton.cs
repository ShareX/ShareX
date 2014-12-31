#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    [DefaultEvent("ColorChanged")]
    public class ColorButton : Button
    {
        public delegate void ColorChangedEventHandler(Color color);
        public event ColorChangedEventHandler ColorChanged;

        private Color color;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                Refresh();
                OnColorChanged(color);
            }
        }

        protected void OnColorChanged(Color color)
        {
            if (ColorChanged != null)
            {
                ColorChanged(color);
            }
        }

        protected override void OnMouseClick(MouseEventArgs mevent)
        {
            base.OnMouseClick(mevent);

            ShowColorDialog();
        }

        public void ShowColorDialog()
        {
            using (ColorPickerForm dialogColor = new ColorPickerForm(Color))
            {
                if (dialogColor.ShowDialog() == DialogResult.OK)
                {
                    Color = dialogColor.NewColor;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            int offset = 3;
            int boxSize = ClientRectangle.Height - offset * 2;
            Rectangle boxRectangle = new Rectangle(ClientRectangle.Width - offset - boxSize, offset, boxSize, boxSize);

            Graphics g = pevent.Graphics;

            if (Color.A < 255)
            {
                using (Image checker = ImageHelpers.CreateCheckers(boxSize, boxSize, Color.LightGray, Color.White))
                {
                    g.DrawImage(checker, boxRectangle);
                }
            }

            using (Brush brush = new SolidBrush(Color))
            {
                g.FillRectangle(brush, boxRectangle);
            }

            g.DrawRectangleProper(Pens.DarkGray, boxRectangle);
        }
    }
}