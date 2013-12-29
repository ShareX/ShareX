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
        public Image ToastImage { get; private set; }
        public string ToastText { get; private set; }
        public string URL { get; private set; }

        private int windowOffset = 3;
        private bool mouseInside = false;
        private bool durationEnd = false;

        public NotificationForm(int duration, Size size, Image img, string url)
        {
            InitializeComponent();

            img = ImageHelpers.ResizeImageLimit(img, size);
            img = ImageHelpers.DrawCheckers(img);
            ToastImage = img;
            URL = url;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Size = new Size(img.Width + 2, img.Height + 2);
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - Width - windowOffset, Screen.PrimaryScreen.WorkingArea.Bottom - Height - windowOffset);

            tDuration.Interval = duration;
            tDuration.Start();
        }

        private void tDuration_Tick(object sender, EventArgs e)
        {
            durationEnd = true;
            tDuration.Stop();

            if (!mouseInside)
            {
                Close();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(ToastImage, 1, 1, ToastImage.Width, ToastImage.Height);

            if (!string.IsNullOrEmpty(ToastText))
            {
                Rectangle textRect = new Rectangle(0, 0, e.ClipRectangle.Width, 45);

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                {
                    g.FillRectangle(brush, textRect);
                }

                using (Font font = new Font("Arial", 10))
                {
                    g.DrawString(ToastText, font, Brushes.Black, textRect.RectangleOffset(-5));
                }
            }

            g.DrawRectangleProper(Pens.Black, e.ClipRectangle);
        }

        public static void Show(int duration, Size size, string imagePath, string url)
        {
            if (duration > 0 && !size.IsEmpty && !string.IsNullOrEmpty(imagePath) && Helpers.IsImageFile(imagePath) && File.Exists(imagePath))
            {
                Image img = ImageHelpers.LoadImage(imagePath);
                NotificationForm form = new NotificationForm(duration, size, img, url);
                NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.ShowNoActivate);
                NativeMethods.SetWindowPos(form.Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0,
                    SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
            }
        }

        public static void Show(string imagePath, string url)
        {
            Show(5000, new Size(400, 300), imagePath, url);
        }

        private void NotificationForm_MouseClick(object sender, MouseEventArgs e)
        {
            tDuration.Stop();

            if (e.Button == MouseButtons.Left && !string.IsNullOrEmpty(URL))
            {
                Helpers.LoadBrowserAsync(URL);
            }

            Close();
        }

        private void NotificationForm_MouseEnter(object sender, EventArgs e)
        {
            mouseInside = true;

            ToastText = URL;
            Refresh();
        }

        private void NotificationForm_MouseLeave(object sender, EventArgs e)
        {
            mouseInside = false;

            if (durationEnd)
            {
                Close();
            }
        }
    }
}