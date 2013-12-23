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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public partial class NotificationForm : Form
    {
        public string ToastText { get; private set; }
        public Image ToastImage { get; private set; }
        public string URL { get; private set; }

        private int windowOffset = 3;

        public NotificationForm(int duration, Size size, string text, Image image, string url)
        {
            InitializeComponent();
            Size = size;
            ToastText = text;
            image = ImageHelpers.ResizeImage(image, ClientRectangle.Size.Offset(-2), true, true);
            image = ImageHelpers.DrawCheckers(image);
            ToastImage = image;
            URL = url;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - Width - windowOffset, Screen.PrimaryScreen.WorkingArea.Bottom - Height - windowOffset);
            NativeMethods.AnimateWindow(Handle, 1000, AnimateWindowFlags.AW_BLEND);
            tDuration.Interval = duration;
            tDuration.Start();
        }

        private void tDuration_Tick(object sender, EventArgs e)
        {
            tDuration.Stop();
            NativeMethods.AnimateWindow(Handle, 2000, AnimateWindowFlags.AW_HIDE | AnimateWindowFlags.AW_BLEND);
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(ToastImage, 1, 1, ToastImage.Width, ToastImage.Height);

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, e.ClipRectangle.Width, 40));
            }

            using (Font font = new Font("Arial", 10))
            {
                g.DrawString(ToastText, font, Brushes.Black, e.ClipRectangle.RectangleOffset(-5));
            }

            g.DrawRectangleProper(Pens.Black, e.ClipRectangle);
        }

        public static void ShowAsync(string text, string imagePath, string url)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                Thread thread = new Thread(() =>
                {
                    using (Image img = ImageHelpers.LoadImage(imagePath))
                    using (NotificationForm toastForm = new NotificationForm(5000, new Size(400, 300), text, img, url))
                    {
                        toastForm.ShowDialog();
                    }
                });

                thread.Start();
            }
        }

        private void NotificationForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!string.IsNullOrEmpty(URL))
                {
                    Helpers.LoadBrowserAsync(URL);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                tDuration.Stop();
                Close();
            }
        }
    }
}