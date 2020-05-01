#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public class NotificationForm : Form
    {
        public NotificationFormConfig ToastConfig { get; private set; }

        public int Duration { get; private set; }
        public int FadeDuration { get; private set; }

        private int windowOffset = 3;
        private bool isMouseInside;
        private bool isDurationEnd;
        private int fadeInterval = 50;
        private float opacityDecrement;
        private Font textFont;
        private int textPadding = 5;
        private int urlPadding = 3;
        private ContextMenuStrip cmsTray;
        private ToolStripMenuItem tsmiTrayImageEditor;
        private ToolStripMenuItem tsmiDelete;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem tsmiCloseMenu;
        private ToolStripMenuItem tsmiOpen;
        private ToolStripMenuItem tsmiOpenFile;
        private ToolStripMenuItem tsmiOpenLocation;
        private ToolStripMenuItem tsmiCopy;
        private ToolStripMenuItem tsmiCopyFile;
        private ToolStripMenuItem tsmiCopyFilePath;
        private ToolStripMenuItem tsmiCopyFileName;
        private ToolStripMenuItem tsmiCopyFileNameWithExtension;
        private ToolStripMenuItem tsmiCopyFolder;
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

        public NotificationForm(int duration, int fadeDuration, ContentAlignment placement, Size size, NotificationFormConfig config)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            Duration = duration;
            FadeDuration = fadeDuration;

            opacityDecrement = (float)fadeInterval / FadeDuration;

            ToastConfig = config;
            textFont = new Font("Arial", 10);

            if (config.Image != null)
            {
                config.Image = ImageHelpers.ResizeImageLimit(config.Image, size);
                Color backgroundColor = ShareXResources.UseCustomTheme ? ShareXResources.Theme.BackgroundColor : SystemColors.Window;
                config.Image = ImageHelpers.FillBackground(config.Image, backgroundColor);
                size = new Size(config.Image.Width + 2, config.Image.Height + 2);
            }
            else if (!string.IsNullOrEmpty(config.Text))
            {
                textRenderSize = TextRenderer.MeasureText(config.Text, textFont, size.Offset(-textPadding * 2), TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                size = new Size(textRenderSize.Width + (textPadding * 2), textRenderSize.Height + (textPadding * 2) + 2);
            }

            Point position = Helpers.GetPosition(placement, new Point(windowOffset, windowOffset), Screen.PrimaryScreen.WorkingArea.Size, size);

            NativeMethods.SetWindowPos(Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, position.X + Screen.PrimaryScreen.WorkingArea.X,
                position.Y + Screen.PrimaryScreen.WorkingArea.Y, size.Width, size.Height, SetWindowPosFlags.SWP_NOACTIVATE);

            if (Duration <= 0)
            {
                DurationEnd();
            }
            else
            {
                tDuration.Interval = Duration;
                tDuration.Start();
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
            if (FadeDuration <= 0)
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

            if (ToastConfig.Image != null)
            {
                g.DrawImage(ToastConfig.Image, 1, 1, ToastConfig.Image.Width, ToastConfig.Image.Height);

                if (isMouseInside && !string.IsNullOrEmpty(ToastConfig.URL))
                {
                    Rectangle textRect = new Rectangle(0, 0, rect.Width, 40);

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        g.FillRectangle(brush, textRect);
                    }

                    TextRenderer.DrawText(g, ToastConfig.URL, textFont, textRect.Offset(-urlPadding), Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            else if (!string.IsNullOrEmpty(ToastConfig.Text))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(80, 80, 80), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                Rectangle textRect = new Rectangle(textPadding, textPadding, textRenderSize.Width + 2, textRenderSize.Height + 2);
                TextRenderer.DrawText(g, ToastConfig.Text, textFont, textRect, Color.Black, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                TextRenderer.DrawText(g, ToastConfig.Text, textFont, textRect.LocationOffset(1), Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            Color borderColor = ShareXResources.UseCustomTheme ? ShareXResources.Theme.BorderColor : SystemColors.ControlText;
            using (Pen borderPen = new Pen(borderColor))
            {
                g.DrawRectangleProper(borderPen, rect);
            }
        }

        public static void Show(int duration, int fadeDuration, ContentAlignment placement, Size size, NotificationFormConfig config)
        {
            if ((duration > 0 || fadeDuration > 0) && size.Width > 0 && size.Height > 0)
            {
                if (config.Image == null)
                {
                    config.Image = ImageHelpers.LoadImage(config.FilePath);
                }

                if (config.Image != null || !string.IsNullOrEmpty(config.Text))
                {
                    NotificationForm form = new NotificationForm(duration, fadeDuration, placement, size, config);
                    NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.ShowNoActivate);
                }
            }
        }

        private void NotificationForm_MouseClick(object sender, MouseEventArgs e)
        {
            tDuration.Stop();

            ToastClickAction action = ToastClickAction.CloseNotification;

            if (e.Button == MouseButtons.Left)
            {
                action = ToastConfig.LeftClickAction;
            }
            else if (e.Button == MouseButtons.Right)
            {
                action = ToastConfig.RightClickAction;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                action = ToastConfig.MiddleClickAction;
            }

            if (action != ToastClickAction.ShowContextMenu)
            {
                Close();
            }

            ExecuteAction(action);
        }

        private void ExecuteAction(ToastClickAction action)
        {
            switch (action)
            {
                case ToastClickAction.AnnotateImage:
                    if (!string.IsNullOrEmpty(ToastConfig.FilePath) && Helpers.IsImageFile(ToastConfig.FilePath))
                        TaskHelpers.AnnotateImageFromFile(ToastConfig.FilePath);
                    break;
                case ToastClickAction.CopyImageToClipboard:
                    if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                        ClipboardHelpers.CopyImageFromFile(ToastConfig.FilePath);
                    break;
                case ToastClickAction.CopyUrl:
                    if (!string.IsNullOrEmpty(ToastConfig.URL))
                        ClipboardHelpers.CopyText(ToastConfig.URL);
                    break;
                case ToastClickAction.OpenFile:
                    if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                        Helpers.OpenFile(ToastConfig.FilePath);
                    break;
                case ToastClickAction.OpenFolder:
                    if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                        Helpers.OpenFolderWithFile(ToastConfig.FilePath);
                    break;
                case ToastClickAction.OpenUrl:
                    if (!string.IsNullOrEmpty(ToastConfig.URL))
                        URLHelpers.OpenURL(ToastConfig.URL);
                    break;
                case ToastClickAction.Upload:
                    if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                        UploadManager.UploadFile(ToastConfig.FilePath);
                    break;
                case ToastClickAction.ShowContextMenu:
                    if (!string.IsNullOrEmpty(ToastConfig.FilePath))
                        //UploadManager.UploadFile(ToastConfig.FilePath);
                        //MessageBox.Show("Testing");
                        cmsTray.Show(Cursor.Position);
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
            this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFilePath = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFileNameWithExtension = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCloseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsTray.SuspendLayout();
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
            // cmsTray
            // 
            this.cmsTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpen,
            this.tsmiCopy,
            this.tsmiTrayImageEditor,
            this.tsmiDelete,
            this.toolStripSeparator1,
            this.tsmiCloseMenu});
            this.cmsTray.Name = "cmsTray";
            this.cmsTray.Size = new System.Drawing.Size(151, 120);
            this.cmsTray.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.cmsTray_Closed);
            this.cmsTray.Opened += new System.EventHandler(this.cmsTray_Opened);
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenFile,
            this.tsmiOpenLocation});
            this.tsmiOpen.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.Size = new System.Drawing.Size(150, 22);
            this.tsmiOpen.Text = "Open";
            // 
            // tsmiOpenFile
            // 
            this.tsmiOpenFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            this.tsmiOpenFile.ShortcutKeyDisplayString = "";
            this.tsmiOpenFile.Size = new System.Drawing.Size(107, 22);
            this.tsmiOpenFile.Text = "File";
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiOpenLocation
            // 
            this.tsmiOpenLocation.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiOpenLocation.Name = "tsmiOpenLocation";
            this.tsmiOpenLocation.ShortcutKeyDisplayString = "";
            this.tsmiOpenLocation.Size = new System.Drawing.Size(107, 22);
            this.tsmiOpenLocation.Text = "Folder";
            this.tsmiOpenLocation.Click += new System.EventHandler(this.tsmiOpenLocation_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyFile,
            this.tsmiCopyFilePath,
            this.tsmiCopyFileName,
            this.tsmiCopyFileNameWithExtension,
            this.tsmiCopyFolder});
            this.tsmiCopy.Image = global::ShareX.Properties.Resources.document_copy;
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(150, 22);
            this.tsmiCopy.Text = "Copy";
            // 
            // tsmiCopyFile
            // 
            this.tsmiCopyFile.Image = global::ShareX.Properties.Resources.document_copy;
            this.tsmiCopyFile.Name = "tsmiCopyFile";
            this.tsmiCopyFile.ShortcutKeyDisplayString = "";
            this.tsmiCopyFile.Size = new System.Drawing.Size(205, 22);
            this.tsmiCopyFile.Text = "File/URL";
            this.tsmiCopyFile.Click += new System.EventHandler(this.tsmiCopyFile_Click);
            // 
            // tsmiCopyFilePath
            // 
            this.tsmiCopyFilePath.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            this.tsmiCopyFilePath.ShortcutKeyDisplayString = "";
            this.tsmiCopyFilePath.Size = new System.Drawing.Size(205, 22);
            this.tsmiCopyFilePath.Text = "File path";
            // 
            // tsmiCopyFileName
            // 
            this.tsmiCopyFileName.Name = "tsmiCopyFileName";
            this.tsmiCopyFileName.Size = new System.Drawing.Size(205, 22);
            this.tsmiCopyFileName.Text = "File name";
            this.tsmiCopyFileName.Click += new System.EventHandler(this.tsmiCopyFileName_Click);
            // 
            // tsmiCopyFileNameWithExtension
            // 
            this.tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            this.tsmiCopyFileNameWithExtension.Size = new System.Drawing.Size(205, 22);
            this.tsmiCopyFileNameWithExtension.Text = "File name with extension";
            this.tsmiCopyFileNameWithExtension.Click += new System.EventHandler(this.tsmiCopyFileNameWithExtension_Click);
            // 
            // tsmiCopyFolder
            // 
            this.tsmiCopyFolder.Name = "tsmiCopyFolder";
            this.tsmiCopyFolder.Size = new System.Drawing.Size(205, 22);
            this.tsmiCopyFolder.Text = "Folder";
            this.tsmiCopyFolder.Click += new System.EventHandler(this.tsmiCopyFolder_Click);
            // 
            // tsmiTrayImageEditor
            // 
            this.tsmiTrayImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiTrayImageEditor.Name = "tsmiTrayImageEditor";
            this.tsmiTrayImageEditor.Size = new System.Drawing.Size(150, 22);
            this.tsmiTrayImageEditor.Text = "Image editor...";
            this.tsmiTrayImageEditor.Click += new System.EventHandler(this.tsmiTrayImageEditor_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Image = global::ShareX.Properties.Resources.eraser;
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(150, 22);
            this.tsmiDelete.Text = "Delete...";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // tsmiCloseMenu
            // 
            this.tsmiCloseMenu.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiCloseMenu.Name = "tsmiCloseMenu";
            this.tsmiCloseMenu.Size = new System.Drawing.Size(150, 22);
            this.tsmiCloseMenu.Text = "Close Menu";
            this.tsmiCloseMenu.Click += new System.EventHandler(this.tsmiCloseMenu_Click);
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
            this.cmsTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete " + ToastConfig.URL + "?", "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if (File.Exists(ToastConfig.URL))
                {
                    File.Delete(ToastConfig.URL);
                }
            }
        }

        private void tsmiTrayImageEditor_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageEditor();
        }

        private void tsmiCloseMenu_Click(object sender, EventArgs e)
        {
            cmsTray.Close();
            tDuration.Start();
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            Process.Start(ToastConfig.URL);
        }

        private void tsmiOpenLocation_Click(object sender, EventArgs e)
        {
            if (File.Exists(ToastConfig.URL))
            {
                Process.Start("explorer.exe", Path.GetDirectoryName(ToastConfig.URL));
            }
        }

        private void cmsTray_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            tDuration.Start();
        }

        private void cmsTray_Opened(object sender, EventArgs e)
        {
            tDuration.Stop();
        }

        private void tsmiCopyFile_Click(object sender, EventArgs e)
        {
            if (Uri.IsWellFormedUriString(ToastConfig.URL, UriKind.RelativeOrAbsolute))
            {
                ClipboardHelpers.CopyText(ToastConfig.URL);
            }
            else
            {
                ClipboardHelpers.CopyFile(ToastConfig.URL);
            }
        }

        private void tsmiCopyFileName_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Path.GetFileNameWithoutExtension(ToastConfig.URL));
        }

        private void tsmiCopyFileNameWithExtension_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Path.GetFileName(ToastConfig.URL));
        }

        private void tsmiCopyFolder_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Path.GetDirectoryName(ToastConfig.URL));
        }
    }

    public class NotificationFormConfig : IDisposable
    {
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
}