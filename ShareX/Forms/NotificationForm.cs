#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public partial class NotificationForm : Form
    {
        public NotificationFormConfig ToastConfig { get; private set; }

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

        public NotificationForm(int duration, ContentAlignment placement, Size size, NotificationFormConfig config)
        {
            InitializeComponent();
            ToastConfig = config;
            textFont = new Font("Arial", 10);

            if (config.Image != null)
            {
                config.Image = ImageHelpers.ResizeImageLimit(config.Image, size);
                config.Image = ImageHelpers.DrawCheckers(config.Image);
                size = new Size(config.Image.Width + 2, config.Image.Height + 2);
            }
            else if (!string.IsNullOrEmpty(config.Text))
            {
                textRenderSize = Helpers.MeasureText(config.Text, textFont, size.Width - textPadding * 2);
                size = new Size(textRenderSize.Width + textPadding * 2, textRenderSize.Height + textPadding * 2 + 2);
            }

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

            if (ToastConfig.Image != null)
            {
                g.DrawImage(ToastConfig.Image, 1, 1, ToastConfig.Image.Width, ToastConfig.Image.Height);

                if (mouseInside && !string.IsNullOrEmpty(ToastConfig.URL))
                {
                    Rectangle textRect = new Rectangle(0, 0, rect.Width, 40);

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        g.FillRectangle(brush, textRect);
                    }

                    g.DrawString(ToastConfig.URL, textFont, Brushes.White, textRect.RectangleOffset(-urlPadding));
                }
            }
            else if (!string.IsNullOrEmpty(ToastConfig.Text))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(80, 80, 80), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                Rectangle textRect = new Rectangle(textPadding, textPadding, textRenderSize.Width + 2, textRenderSize.Height + 2);
                g.DrawString(ToastConfig.Text, textFont, Brushes.Black, textRect);
                g.DrawString(ToastConfig.Text, textFont, Brushes.White, textRect.LocationOffset(1));
            }

            g.DrawRectangleProper(Pens.Black, rect);
        }

        public static void Show(int duration, ContentAlignment placement, Size size, NotificationFormConfig config)
        {
            if (duration > 0 && size.Width > 0 && size.Height > 0)
            {
                config.Image = ImageHelpers.LoadImage(config.FilePath);

                if (config.Image != null || !string.IsNullOrEmpty(config.Text))
                {
                    NotificationForm form = new NotificationForm(duration, placement, size, config);
                    NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.ShowNoActivate);
                    NativeMethods.SetWindowPos(form.Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0,
                        SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
                }
            }
        }

        private void NotificationForm_MouseClick(object sender, MouseEventArgs e)
        {
            tDuration.Stop();

            if (e.Button == MouseButtons.Left)
            {
                switch (ToastConfig.Action)
                {
                    case ToastClickAction.AnnotateImage:
                        if (!string.IsNullOrEmpty(ToastConfig.FilePath) && Helpers.IsImageFile(ToastConfig.FilePath))
                            TaskHelpers.AnnotateImage(ToastConfig.FilePath);
                        break;
                    case ToastClickAction.CopyImageToClipboard:
                        if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                            ClipboardHelpers.CopyImageFromFile(ToastConfig.FilePath);
                        break;
                    case ToastClickAction.OpenFile:
                        if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                            Helpers.OpenURL(ToastConfig.FilePath);
                        break;
                    case ToastClickAction.OpenFolder:
                        if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                            Helpers.OpenFolderWithFile(ToastConfig.FilePath);
                        break;
                    case ToastClickAction.OpenUrl:
                        if (!string.IsNullOrEmpty(ToastConfig.URL))
                            Helpers.OpenURL(ToastConfig.URL);
                        break;
                    case ToastClickAction.Upload:
                        if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                            UploadManager.UploadFile(ToastConfig.FilePath);
                        break;
                }
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

            if (ToastConfig != null)
            {
                ToastConfig.Dispose();
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

    public class NotificationFormConfig : IDisposable
    {
        public Image Image { get; set; }
        public string Text { get; set; }
        public string FilePath { get; set; }
        public string URL { get; set; }
        public ToastClickAction Action { get; set; }

        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
            }
        }
    }
}