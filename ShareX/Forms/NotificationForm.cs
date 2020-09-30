#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public class NotificationFormConfig : IDisposable
    {
        public int Duration { get; set; }
        public int FadeDuration { get; set; }
        public ContentAlignment Placement { get; set; }
        public Size Size { get; set; }
        public bool IsValid => (Duration > 0 || FadeDuration > 0) && Size.Width > 0 && Size.Height > 0;

        public Bitmap Image { get; set; }
        public string Text { get; set; }
        public string FilePath { get; set; }
        public string URL { get; set; }
        public ToastClickAction LeftClickAction { get; set; }
        public ToastClickAction RightClickAction { get; set; }
        public ToastClickAction MiddleClickAction { get; set; }

        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
            }
        }
    }

    public class NotificationForm : Form
    {
        public NotificationFormConfig Config { get; private set; }

        private int windowOffset = 3;
        private bool isMouseInside;
        private bool isDurationEnd;
        private int fadeInterval = 50;
        private float opacityDecrement;
        private Font textFont;
        private int textPadding = 5;
        private int urlPadding = 3;
        private Size textRenderSize;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
                return createParams;
            }
        }

        private NotificationForm(NotificationFormConfig config)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            Config = config;
            textFont = new Font("Arial", 10);
            opacityDecrement = (float)fadeInterval / Config.FadeDuration;

            if (config.Image != null)
            {
                config.Image = ImageHelpers.ResizeImageLimit(config.Image, Config.Size);
                Color backgroundColor = ShareXResources.UseCustomTheme ? ShareXResources.Theme.BackgroundColor : SystemColors.Window;
                config.Image = ImageHelpers.FillBackground(config.Image, backgroundColor);
                Config.Size = new Size(config.Image.Width + 2, config.Image.Height + 2);
            }
            else if (!string.IsNullOrEmpty(config.Text))
            {
                textRenderSize = TextRenderer.MeasureText(config.Text, textFont, Config.Size.Offset(-textPadding * 2), TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                Config.Size = new Size(textRenderSize.Width + (textPadding * 2), textRenderSize.Height + (textPadding * 2) + 2);
            }

            Point position = Helpers.GetPosition(Config.Placement, windowOffset, Screen.PrimaryScreen.WorkingArea.Size, Config.Size);

            NativeMethods.SetWindowPos(Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, position.X + Screen.PrimaryScreen.WorkingArea.X,
                position.Y + Screen.PrimaryScreen.WorkingArea.Y, Config.Size.Width, Config.Size.Height, SetWindowPosFlags.SWP_NOACTIVATE);

            if (Config.Duration <= 0)
            {
                DurationEnd();
            }
            else
            {
                tDuration.Interval = Config.Duration;
                tDuration.Start();
            }
        }

        public static void Show(NotificationFormConfig config)
        {
            if (config.IsValid)
            {
                if (config.Image == null)
                {
                    config.Image = ImageHelpers.LoadImage(config.FilePath);
                }

                if (config.Image != null || !string.IsNullOrEmpty(config.Text))
                {
                    NotificationForm form = new NotificationForm(config);
                    NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.ShowNoActivate);
                }
            }
        }

        private void tDuration_Tick(object sender, EventArgs e)
        {
            DurationEnd();
        }

        private void DurationEnd()
        {
            isDurationEnd = true;
            tDuration.Stop();

            if (!isMouseInside)
            {
                StartClosing();
            }
        }

        private void StartClosing()
        {
            if (Config.FadeDuration <= 0)
            {
                Close();
            }
            else
            {
                Opacity = 1;
                tOpacity.Interval = fadeInterval;
                tOpacity.Start();
            }
        }

        private void tOpacity_Tick(object sender, EventArgs e)
        {
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

            Rectangle rect = ClientRectangle;

            if (Config.Image != null)
            {
                g.DrawImage(Config.Image, 1, 1, Config.Image.Width, Config.Image.Height);

                if (isMouseInside && !string.IsNullOrEmpty(Config.URL))
                {
                    Rectangle textRect = new Rectangle(0, 0, rect.Width, 40);

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        g.FillRectangle(brush, textRect);
                    }

                    TextRenderer.DrawText(g, Config.URL, textFont, textRect.Offset(-urlPadding), Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            else if (!string.IsNullOrEmpty(Config.Text))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(80, 80, 80), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                Rectangle textRect = new Rectangle(textPadding, textPadding, textRenderSize.Width + 2, textRenderSize.Height + 2);
                TextRenderer.DrawText(g, Config.Text, textFont, textRect, Color.Black, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                TextRenderer.DrawText(g, Config.Text, textFont, textRect.LocationOffset(1), Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            Color borderColor = ShareXResources.UseCustomTheme ? ShareXResources.Theme.BorderColor : SystemColors.ControlText;
            using (Pen borderPen = new Pen(borderColor))
            {
                g.DrawRectangleProper(borderPen, rect);
            }
        }

        private void NotificationForm_MouseClick(object sender, MouseEventArgs e)
        {
            tDuration.Stop();

            Close();

            ToastClickAction action = ToastClickAction.CloseNotification;

            if (e.Button == MouseButtons.Left)
            {
                action = Config.LeftClickAction;
            }
            else if (e.Button == MouseButtons.Right)
            {
                action = Config.RightClickAction;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                action = Config.MiddleClickAction;
            }

            ExecuteAction(action);
        }

        private void ExecuteAction(ToastClickAction action)
        {
            switch (action)
            {
                case ToastClickAction.AnnotateImage:
                    if (!string.IsNullOrEmpty(Config.FilePath) && Helpers.IsImageFile(Config.FilePath))
                        TaskHelpers.AnnotateImageFromFile(Config.FilePath);
                    break;
                case ToastClickAction.CopyImageToClipboard:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                        ClipboardHelpers.CopyImageFromFile(Config.FilePath);
                    break;
                case ToastClickAction.CopyUrl:
                    if (!string.IsNullOrEmpty(Config.URL))
                        ClipboardHelpers.CopyText(Config.URL);
                    break;
                case ToastClickAction.OpenFile:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                        Helpers.OpenFile(Config.FilePath);
                    break;
                case ToastClickAction.OpenFolder:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                        Helpers.OpenFolderWithFile(Config.FilePath);
                    break;
                case ToastClickAction.OpenUrl:
                    if (!string.IsNullOrEmpty(Config.URL))
                        URLHelpers.OpenURL(Config.URL);
                    break;
                case ToastClickAction.Upload:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                        UploadManager.UploadFile(Config.FilePath);
                    break;
            }
        }

        private void NotificationForm_MouseEnter(object sender, EventArgs e)
        {
            isMouseInside = true;
            tOpacity.Stop();

            if (!IsDisposed)
            {
                Refresh();
                Opacity = 1;
            }
        }

        private void NotificationForm_MouseLeave(object sender, EventArgs e)
        {
            isMouseInside = false;
            Refresh();

            if (isDurationEnd)
            {
                StartClosing();
            }
        }

        #region Windows Form Designer generated code

        private Timer tDuration;
        private Timer tOpacity;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (Config != null)
            {
                Config.Dispose();
            }

            if (textFont != null)
            {
                textFont.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tDuration = new Timer(components);
            tOpacity = new Timer(components);
            SuspendLayout();
            //
            // tDuration
            //
            tDuration.Tick += new EventHandler(tDuration_Tick);
            //
            // tOpacity
            //
            tOpacity.Tick += new EventHandler(tOpacity_Tick);
            //
            // NotificationForm
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(400, 300);
            Cursor = Cursors.Hand;
            FormBorderStyle = FormBorderStyle.None;
            Name = "NotificationForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "NotificationForm";
            MouseClick += new MouseEventHandler(NotificationForm_MouseClick);
            MouseEnter += new EventHandler(NotificationForm_MouseEnter);
            MouseLeave += new EventHandler(NotificationForm_MouseLeave);
            ResumeLayout(false);
        }

        #endregion Windows Form Designer generated code
    }
}