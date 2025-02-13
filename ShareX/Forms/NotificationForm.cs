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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public class NotificationForm : LayeredForm
    {
        private static NotificationForm instance;

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
        private bool isMouseDragging;
        private Point dragStart;
        private float opacity = 255;
        private Bitmap buffer;
        private Graphics gBuffer;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
                return createParams;
            }
        }

        private NotificationForm()
        {
            InitializeComponent();
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
                    if (instance == null || instance.IsDisposed)
                    {
                        instance = new NotificationForm();
                        instance.LoadConfig(config);

                        NativeMethods.ShowWindow(instance.Handle, (int)WindowShowStyle.ShowNoActivate);
                    }
                    else
                    {
                        instance.LoadConfig(config);
                    }
                }
            }
        }

        public static void CloseActiveForm()
        {
            if (instance != null && !instance.IsDisposed)
            {
                instance.Close();
            }
        }

        public void LoadConfig(NotificationFormConfig config)
        {
            Config?.Dispose();
            buffer?.Dispose();
            gBuffer?.Dispose();

            Config = config;
            opacityDecrement = (float)fadeInterval / Config.FadeDuration * 255;

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

            buffer = new Bitmap(Config.Size.Width, Config.Size.Height);
            gBuffer = Graphics.FromImage(buffer);

            Point position = Helpers.GetPosition(Config.Placement, Config.Offset, Screen.PrimaryScreen.WorkingArea, Config.Size);

            NativeMethods.SetWindowPos(Handle, (IntPtr)NativeConstants.HWND_TOPMOST, position.X, position.Y, Config.Size.Width, Config.Size.Height,
                SetWindowPosFlags.SWP_NOACTIVATE);

            tDuration.Stop();
            tOpacity.Stop();

            opacity = 255;
            Render(true);

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

        private void UpdateBuffer()
        {
            Rectangle rect = new Rectangle(0, 0, buffer.Width, buffer.Height);

            gBuffer.Clear(Config.BackgroundColor);

            if (Config.Image != null)
            {
                gBuffer.DrawImage(Config.Image, 1, 1, Config.Image.Width, Config.Image.Height);

                if (isMouseInside && !string.IsNullOrEmpty(Config.URL))
                {
                    Rectangle textRect = new Rectangle(0, 0, rect.Width, 40);

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        gBuffer.FillRectangle(brush, textRect);
                    }

                    TextRenderer.DrawText(gBuffer, Config.URL, Config.TextFont, textRect.Offset(-urlPadding), Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            else if (!string.IsNullOrEmpty(Config.Text))
            {
                Rectangle textRect;

                if (!string.IsNullOrEmpty(Config.Title))
                {
                    Rectangle titleRect = new Rectangle(Config.TextPadding, Config.TextPadding, titleRenderSize.Width + 2, titleRenderSize.Height + 2);
                    TextRenderer.DrawText(gBuffer, Config.Title, Config.TitleFont, titleRect, Config.TitleColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                    textRect = new Rectangle(Config.TextPadding, Config.TextPadding + titleRect.Height + titleSpace, textRenderSize.Width + 2, textRenderSize.Height + 2);
                }
                else
                {
                    textRect = new Rectangle(Config.TextPadding, Config.TextPadding, textRenderSize.Width + 2, textRenderSize.Height + 2);
                }

                TextRenderer.DrawText(gBuffer, Config.Text, Config.TextFont, textRect, Config.TextColor,
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis);
            }

            using (Pen borderPen = new Pen(Config.BorderColor))
            {
                gBuffer.DrawRectangleProper(borderPen, rect);
            }
        }

        private void Render(bool updateBuffer)
        {
            if (updateBuffer)
            {
                UpdateBuffer();
            }

            SelectBitmap(buffer, (int)opacity);
        }

        private void DurationEnd()
        {
            isDurationEnd = true;
            tDuration.Stop();

            if (!isMouseInside)
            {
                StartFade();
            }
        }

        private void StartFade()
        {
            if (Config.FadeDuration <= 0)
            {
                Close();
            }
            else
            {
                opacity = 255;
                Render(false);

                tOpacity.Interval = fadeInterval;
                tOpacity.Start();
            }
        }

        private void tDuration_Tick(object sender, EventArgs e)
        {
            DurationEnd();
        }

        private void tOpacity_Tick(object sender, EventArgs e)
        {
            if (opacity > opacityDecrement)
            {
                opacity -= opacityDecrement;
                Render(false);
            }
            else
            {
                Close();
            }
        }

        private void NotificationForm_MouseEnter(object sender, EventArgs e)
        {
            isMouseInside = true;
            tOpacity.Stop();

            if (!IsDisposed)
            {
                opacity = 255;
                Render(true);
            }
        }

        private void NotificationForm_MouseLeave(object sender, EventArgs e)
        {
            isMouseInside = false;
            isMouseDragging = false;
            Render(true);

            if (isDurationEnd)
            {
                StartFade();
            }
        }

        private void NotificationForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragStart = e.Location;
                isMouseDragging = true;
            }
        }

        private void NotificationForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDragging)
            {
                int dragThreshold = 20;

                Rectangle dragThresholdRectangle = new Rectangle(dragStart.X - dragThreshold, dragStart.Y - dragThreshold, dragThreshold * 2, dragThreshold * 2);

                bool isOverThreshold = !dragThresholdRectangle.Contains(e.Location);
                if (isOverThreshold && !string.IsNullOrEmpty(Config.FilePath) && File.Exists(Config.FilePath))
                {
                    IDataObject dataObject = new DataObject(DataFormats.FileDrop, new string[] { Config.FilePath });
                    DoDragDrop(dataObject, DragDropEffects.Copy | DragDropEffects.Move);

                    isMouseDragging = false;
                }
            }
        }

        private void NotificationForm_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDragging = false;
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
                    if (!string.IsNullOrEmpty(Config.FilePath) && FileHelpers.IsImageFile(Config.FilePath))
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
                        FileHelpers.OpenFile(Config.FilePath);
                    }
                    break;
                case ToastClickAction.OpenFolder:
                    if (!string.IsNullOrEmpty(Config.FilePath))
                    {
                        FileHelpers.OpenFolderWithFile(Config.FilePath);
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
                case ToastClickAction.PinToScreen:
                    if (!string.IsNullOrEmpty(Config.FilePath) && FileHelpers.IsImageFile(Config.FilePath))
                    {
                        TaskHelpers.PinToScreen(Config.FilePath);
                    }
                    break;
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

            Config?.Dispose();
            buffer?.Dispose();
            gBuffer?.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tDuration = new Timer(components);
            tOpacity = new Timer(components);
            SuspendLayout();
            tDuration.Tick += new EventHandler(tDuration_Tick);
            tOpacity.Tick += new EventHandler(tOpacity_Tick);
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
            MouseDown += new MouseEventHandler(NotificationForm_MouseDown);
            MouseMove += new MouseEventHandler(NotificationForm_MouseMove);
            MouseUp += new MouseEventHandler(NotificationForm_MouseUp);
            ResumeLayout(false);
        }

        #endregion Windows Form Designer generated code
    }
}