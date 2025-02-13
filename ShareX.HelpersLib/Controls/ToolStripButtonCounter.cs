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

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.HelpersLib
{
    public class ToolStripButtonCounter : ToolStripButtonExtraImage
    {
        private int counter;

        public int Counter
        {
            get
            {
                return counter;
            }
            set
            {
                counter = value;
                UpdateImage();
            }
        }

        private void UpdateImage()
        {
            if (counter <= 0)
            {
                ShowExtraImage = false;

                if (ExtraImage != null)
                {
                    ExtraImage.Dispose();
                }

                ExtraImage = null;
            }
            else
            {
                int size = Height - (ExtraImagePadding * 2);
                Bitmap bmp = new Bitmap(size, size);

                using (Graphics g = Graphics.FromImage(bmp))
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                using (Brush brush = new SolidBrush(Color.FromArgb(230, 0, 0)))
                using (Font font = new Font("Arial", 9, FontStyle.Bold))
                using (StringFormat stringFormat = new StringFormat())
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    g.DrawRoundedRectangle(shadowBrush, new Rectangle(0, 1, bmp.Width, bmp.Height), 3);
                    g.DrawRoundedRectangle(brush, new Rectangle(0, 0, bmp.Width, bmp.Height), 3);
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    string text;
                    if (Counter > 9)
                    {
                        text = "+";
                    }
                    else
                    {
                        text = Counter.ToString();
                    }
                    g.DrawString(text, font, Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height), stringFormat);
                }

                if (ExtraImage != null)
                {
                    ExtraImage.Dispose();
                }

                ExtraImage = bmp;
                ShowExtraImage = true;
            }
        }
    }
}