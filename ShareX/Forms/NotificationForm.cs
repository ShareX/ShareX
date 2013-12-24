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
        private Size maxImageSize;

        public NotificationForm(int duration, Size size, string text, Image img, string url)
        {
            InitializeComponent();
            maxImageSize = size;
            ToastText = text;

            img = ImageHelpers.ResizeImageLimit(img, size);
            img = ImageHelpers.DrawCheckers(img);
            ToastImage = img;
            URL = url;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            Rectangle rect = new Rectangle(Screen.PrimaryScreen.WorkingArea.Right - (img.Width + 2) - windowOffset,
                Screen.PrimaryScreen.WorkingArea.Bottom - (img.Height + 2) - windowOffset, img.Width + 2, img.Height + 2);

            NativeMethods.SetWindowPos(Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, rect.X, rect.Y, rect.Width, rect.Height, SetWindowPosFlags.SWP_NOACTIVATE);
            NativeMethods.AnimateWindow(Handle, 500, AnimateWindowFlags.AW_SLIDE | AnimateWindowFlags.AW_VER_NEGATIVE);
            tDuration.Interval = duration;
            tDuration.Start();
        }

        private void tDuration_Tick(object sender, EventArgs e)
        {
            tDuration.Stop();
            NativeMethods.AnimateWindow(Handle, 1000, AnimateWindowFlags.AW_HIDE | AnimateWindowFlags.AW_BLEND);
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(ToastImage, 1, 1, ToastImage.Width, ToastImage.Height);

            /*using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, e.ClipRectangle.Width, 45));
            }

            using (Font font = new Font("Arial", 10))
            {
                g.DrawString(ToastText, font, Brushes.Black, e.ClipRectangle.RectangleOffset(-5));
            }*/

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

            tDuration.Stop();
            Close();
        }
    }
}