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
using System.Windows.Forms;

namespace ShareX
{
    public class NotificationFormConfig : IDisposable
    {
        public int Duration { get; set; }
        public int FadeDuration { get; set; }
        public ContentAlignment Placement { get; set; }
        public int Offset { get; set; } = 5;
        public Size Size { get; set; }
        public bool IsValid => (Duration > 0 || FadeDuration > 0) && Size.Width > 0 && Size.Height > 0;
        public Color BackgroundColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color BorderColor { get; set; } = Color.FromArgb(40, 40, 40);
        public int TextPadding { get; set; } = 10;
        public Font TextFont { get; set; } = new Font("Arial", 11);
        public Color TextColor { get; set; } = Color.FromArgb(210, 210, 210);
        public Font TitleFont { get; set; } = new Font("Arial", 11, FontStyle.Bold);
        public Color TitleColor { get; set; } = Color.FromArgb(240, 240, 240);

        public Bitmap Image { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string FilePath { get; set; }
        public string URL { get; set; }
        public ToastClickAction LeftClickAction { get; set; }
        public ToastClickAction RightClickAction { get; set; }
        public ToastClickAction MiddleClickAction { get; set; }

        public void Dispose()
        {
            if (TextFont != null)
            {
                TextFont.Dispose();
            }

            if (TitleFont != null)
            {
                TitleFont.Dispose();
            }

            if (Image != null)
            {
                Image.Dispose();
            }
        }
    }

    public class NotificationForm : Form
    {
        public NotificationFormConfig Config { get; private set; }

        private bool isMouseInside;
        private bool isDurationEnd;
        private int fadeInterval = 50;
        private float opacityDecrement;
        private int urlPadding = 3;
        private int titleSpace = 3;
        private Size titleRenderSize;
        private Size textRenderSize;
        private Size totalRenderSize;

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
            opacityDecrement = (float)fadeInterval / Config.FadeDuration;

            if (Config.Image != null)
            {
                Config.Image = ImageHelpers.ResizeImageLimit(Config.Image, Config.Size);
                Config.Size = new Size(Config.Image.Width + 2, Config.Image.Height + 2);
            }
            else if (!string.IsNullOrEmpty(Config.Text))
            {
                Size size = Config.Size.Offset(-Config.TextPadding * 2);
                textRenderSize = TextRenderer.MeasureText(Config.Text, Config.TextFont, size,
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis);
                textRenderSize = new Size(textRenderSize.Width, Math.Min(textRenderSize.Height, size.Height));
                totalRenderSize = textRenderSize;

                if (!string.IsNullOrEmpty(Config.Title))
                {
                    titleRenderSize = TextRenderer.MeasureText(Config.Title, Config.TitleFont, Config.Size.Offset(-Config.TextPadding * 2),
                        TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                    totalRenderSize = new Size(Math.Max(textRenderSize.Width, titleRenderSize.Width), titleRenderSize.Height + titleSpace + textRenderSize.Height);
                }

                Config.Size = new Size(totalRenderSize.Width + (Config.TextPadding * 2), totalRenderSize.Height + (Config.TextPadding * 2) + 2);
            }

            Point position = Helpers.GetPosition(Config.Placement, Config.Offset, Screen.PrimaryScreen.WorkingArea.Size, Config.Size);

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
            g.Clear(Config.BackgroundColor);

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

                    TextRenderer.DrawText(g, Config.URL, Config.TextFont, textRect.Offset(-urlPadding), Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            else if (!string.IsNullOrEmpty(Config.Text))
            {
                Rectangle textRect;

                if (!string.IsNullOrEmpty(Config.Title))
                {
                    Rectangle titleRect = new Rectangle(Config.TextPadding, Config.TextPadding, titleRenderSize.Width + 2, titleRenderSize.Height + 2);
                    TextRenderer.DrawText(g, Config.Title, Config.TitleFont, titleRect, Config.TitleColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                    textRect = new Rectangle(Config.TextPadding, Config.TextPadding + titleRect.Height + titleSpace, textRenderSize.Width + 2, textRenderSize.Height + 2);
                }
                else
                {
                    textRect = new Rectangle(Config.TextPadding, Config.TextPadding, textRenderSize.Width + 2, textRenderSize.Height + 2);
                }

                TextRenderer.DrawText(g, Config.Text, Config.TextFont, textRect, Config.TextColor,
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis);
            }

            using (Pen borderPen = new Pen(Config.BorderColor))
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
                    {
                        TaskHelpers.AnnotateImageFromFile(Config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyImageToClipboard:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        ClipboardHelpers.CopyImageFromFile(Config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyFile:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        ClipboardHelpers.CopyFile(Config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyFilePath:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        ClipboardHelpers.CopyText(Config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyUrl:
                    if (!string.IsNullOrEmpty(Config.URL))
                    {
                        ClipboardHelpers.CopyText(Config.URL);
                    }
                    break;
                case ToastClickAction.OpenFile:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        Helpers.OpenFile(Config.FilePath);
                    }
                    break;
                case ToastClickAction.OpenFolder:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        Helpers.OpenFolderWithFile(Config.FilePath);
                    }
                    break;
                case ToastClickAction.OpenUrl:
                    if (!string.IsNullOrEmpty(Config.URL))
                    {
                        URLHelpers.OpenURL(Config.URL);
                    }
                    break;
                case ToastClickAction.Upload:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        UploadManager.UploadFile(Config.FilePath);
                    }
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