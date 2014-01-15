#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
        public string ToastURL { get; private set; }

        private int windowOffset = 3;
        private bool mouseInside = false;
        private bool durationEnd = false;
        private bool closingAnimation = true;
        private int closingAnimationDuration = 2000;
        private int closingAnimationInterval = 50;
        private Font textFont;
        private int textPadding = 5;
        private int urlPadding = 3;
        private Size textRenderSize;

        public NotificationForm(int duration, ContentAlignment placement, Size size, Image img, string text, string url)
        {
            InitializeComponent();

            textFont = new Font("Arial", 10);

            if (img != null)
            {
                img = ImageHelpers.ResizeImageLimit(img, size);
                img = ImageHelpers.DrawCheckers(img);
                size = new Size(img.Width + 2, img.Height + 2);
                ToastImage = img;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                textRenderSize = Helpers.MeasureText(text, textFont, size.Width - textPadding * 2);
                size = new Size(textRenderSize.Width + textPadding * 2, textRenderSize.Height + textPadding * 2 + 2);
                ToastText = text;
            }

            ToastURL = url;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Size = size;
            Point position = Helpers.GetPosition(placement, new Point(windowOffset, windowOffset), Screen.PrimaryScreen.WorkingArea.Size, Size);
            Location = new Point(Screen.PrimaryScreen.WorkingArea.X + position.X, Screen.PrimaryScreen.WorkingArea.Y + position.Y);

            tDuration.Interval = duration;
            tDuration.Start();
        }

        private void tDuration_Tick(object sender, EventArgs e)
        {
            durationEnd = true;
            tDuration.Stop();

            if (!mouseInside)
            {
                StartClosing();
            }
        }

        private void StartClosing()
        {
            if (closingAnimation)
            {
                Opacity = 1;
                tOpacity.Interval = closingAnimationInterval;
                tOpacity.Start();
            }
            else
            {
                Close();
            }
        }

        private void tOpacity_Tick(object sender, EventArgs e)
        {
            float opacityDecrement = (float)closingAnimationInterval / closingAnimationDuration;

            if (Opacity > opacityDecrement)
            {
                Opacity -= opacityDecrement;
            }
            else
            {
                Close();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle rect = e.ClipRectangle;

            if (ToastImage != null)
            {
                g.DrawImage(ToastImage, 1, 1, ToastImage.Width, ToastImage.Height);

                if (mouseInside && !string.IsNullOrEmpty(ToastURL))
                {
                    Rectangle textRect = new Rectangle(0, 0, rect.Width, 40);

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                    {
                        g.FillRectangle(brush, textRect);
                    }

                    g.DrawString(ToastURL, textFont, Brushes.Black, textRect.RectangleOffset(-urlPadding));
                }
            }
            else if (!string.IsNullOrEmpty(ToastText))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(80, 80, 80), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                Rectangle textRect = new Rectangle(textPadding, textPadding, textRenderSize.Width + 2, textRenderSize.Height + 2);
                g.DrawString(ToastText, textFont, Brushes.Black, textRect);
                g.DrawString(ToastText, textFont, Brushes.White, textRect.LocationOffset(1));
            }

            g.DrawRectangleProper(Pens.Black, rect);
        }

        public static void Show(int duration, ContentAlignment placement, Size size, string imagePath, string text, string url)
        {
            if (duration > 0 && !size.IsEmpty)
            {
                Image img = ImageHelpers.LoadImage(imagePath);

                if (img != null || !string.IsNullOrEmpty(text))
                {
                    NotificationForm form = new NotificationForm(duration, placement, size, img, text, url);
                    NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.ShowNoActivate);
                    NativeMethods.SetWindowPos(form.Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0,
                        SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
                }
            }
        }

        public static void Show(string imagePath, string text, string url)
        {
            Show(4000, ContentAlignment.BottomRight, new Size(400, 300), imagePath, text, url);
        }

        private void NotificationForm_MouseClick(object sender, MouseEventArgs e)
        {
            tDuration.Stop();

            if (e.Button == MouseButtons.Left && !string.IsNullOrEmpty(ToastURL))
            {
                Helpers.LoadBrowserAsync(ToastURL);
            }

            Close();
        }

        private void NotificationForm_MouseEnter(object sender, EventArgs e)
        {
            mouseInside = true;
            Refresh();

            tOpacity.Stop();
            Opacity = 1;
        }

        private void NotificationForm_MouseLeave(object sender, EventArgs e)
        {
            mouseInside = false;
            Refresh();

            if (durationEnd)
            {
                StartClosing();
            }
        }

        #region Windows Form Designer generated code

        private System.Windows.Forms.Timer tDuration;
        private System.Windows.Forms.Timer tOpacity;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (ToastImage != null)
            {
                ToastImage.Dispose();
            }

            if (textFont != null)
            {
                textFont.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tDuration = new System.Windows.Forms.Timer(this.components);
            this.tOpacity = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // tDuration
            //
            this.tDuration.Tick += new System.EventHandler(this.tDuration_Tick);
            //
            // tOpacity
            //
            this.tOpacity.Tick += new System.EventHandler(this.tOpacity_Tick);
            //
            // NotificationForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotificationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NotificationForm";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotificationForm_MouseClick);
            this.MouseEnter += new System.EventHandler(this.NotificationForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.NotificationForm_MouseLeave);
            this.ResumeLayout(false);
        }

        #endregion Windows Form Designer generated code
    }
}