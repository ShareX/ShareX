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

using ShareX.HelpersLib.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class MyPictureBox : UserControl
    {
        public Image Image
        {
            get
            {
                return pbMain.Image;
            }
            private set
            {
                pbMain.Image = value;
            }
        }

        private string text;

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Bindable(true)]
        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;

                if (string.IsNullOrEmpty(value))
                {
                    lblStatus.Visible = false;
                }
                else
                {
                    lblStatus.Text = value;
                    lblStatus.Visible = true;
                }
            }
        }

        public Color PictureBoxBackColor
        {
            get
            {
                return pbMain.BackColor;
            }
            set
            {
                pbMain.BackColor = value;
            }
        }

        private bool drawCheckeredBackground;

        [DefaultValue(false)]
        public bool DrawCheckeredBackground
        {
            get
            {
                return drawCheckeredBackground;
            }
            set
            {
                drawCheckeredBackground = value;
                UpdateCheckers();
            }
        }

        [DefaultValue(false)]
        public bool FullscreenOnClick { get; set; }

        [DefaultValue(false)]
        public bool EnableRightClickMenu { get; set; }

        [DefaultValue(false)]
        public bool ShowImageSizeLabel { get; set; }

        public new event MouseEventHandler MouseDown
        {
            add
            {
                pbMain.MouseDown += value;
                lblStatus.MouseDown += value;
            }
            remove
            {
                pbMain.MouseDown -= value;
                lblStatus.MouseDown -= value;
            }
        }

        public new event MouseEventHandler MouseUp
        {
            add
            {
                pbMain.MouseUp += value;
                lblStatus.MouseUp += value;
            }
            remove
            {
                pbMain.MouseUp -= value;
                lblStatus.MouseUp -= value;
            }
        }

        public new event MouseEventHandler MouseClick
        {
            add
            {
                pbMain.MouseClick += value;
                lblStatus.MouseClick += value;
            }
            remove
            {
                pbMain.MouseClick -= value;
                lblStatus.MouseClick -= value;
            }
        }

        public new event MouseEventHandler MouseMove
        {
            add
            {
                pbMain.MouseMove += value;
                lblStatus.MouseMove += value;
            }
            remove
            {
                pbMain.MouseMove -= value;
                lblStatus.MouseMove -= value;
            }
        }

        public bool IsValidImage
        {
            get
            {
                return !isImageLoading && pbMain.IsValidImage();
            }
        }

        private readonly object imageLoadLock = new object();

        private bool isImageLoading;

        public MyPictureBox()
        {
            InitializeComponent();
            Text = "";
            UpdateTheme();
            UpdateImageSizeLabel();
        }

        private void UpdateImageSizeLabel()
        {
            if (IsValidImage)
            {
                lblImageSize.Text = $"{Image.Width} x {Image.Height}";
                lblImageSize.Location = new Point((ClientSize.Width - lblImageSize.Width) / 2, ClientSize.Height - lblImageSize.Height + 1);
            }
        }

        public void UpdateTheme()
        {
            if (ShareXResources.UseCustomTheme)
            {
                lblImageSize.BackColor = ShareXResources.Theme.BackgroundColor;
                lblImageSize.ForeColor = ShareXResources.Theme.TextColor;

                ShareXResources.ApplyCustomThemeToContextMenuStrip(cmsMenu);
            }
            else
            {
                lblImageSize.BackColor = SystemColors.Window;
                lblImageSize.ForeColor = SystemColors.ControlText;
            }
        }

        public void UpdateCheckers(bool forceUpdate = false)
        {
            if (DrawCheckeredBackground)
            {
                if (forceUpdate || pbMain.BackgroundImage == null || pbMain.BackgroundImage.Size != pbMain.ClientSize)
                {
                    if (pbMain.BackgroundImage != null) pbMain.BackgroundImage.Dispose();

                    if (ShareXResources.Theme.CheckerSize > 0)
                    {
                        pbMain.BackgroundImage = ImageHelpers.CreateCheckerPattern(ShareXResources.Theme.CheckerSize, ShareXResources.Theme.CheckerSize,
                            ShareXResources.Theme.CheckerColor, ShareXResources.Theme.CheckerColor2);
                    }
                    else
                    {
                        pbMain.BackColor = ShareXResources.Theme.CheckerColor;
                        pbMain.BackgroundImage = null;
                    }
                }
            }
            else
            {
                if (pbMain.BackgroundImage != null) pbMain.BackgroundImage.Dispose();
                pbMain.BackgroundImage = null;
            }
        }

        public void LoadImage(Image img)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();

                    if (img != null)
                    {
                        isImageLoading = true;
                        Image = (Image)img.Clone();
                        isImageLoading = false;
                    }
                    else
                    {
                        Image = null;
                    }

                    AutoSetSizeMode();
                }
            }
        }

        public void LoadImageFromFile(string filePath)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();
                    isImageLoading = true;
                    Image = ImageHelpers.LoadImage(filePath);
                    isImageLoading = false;
                    AutoSetSizeMode();
                }
            }
        }

        public void LoadImageFromFileAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                LoadImageAsync(filePath);
            }
        }

        public void LoadImageFromURLAsync(string url)
        {
            if (!string.IsNullOrEmpty(url) && !url.StartsWith("ftp://") && !url.StartsWith("ftps://"))
            {
                LoadImageAsync(url);
            }
        }

        private void LoadImageAsync(string path)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();
                    isImageLoading = true;
                    Text = Resources.MyPictureBox_LoadImageAsync_Loading_image___;
                    lblStatus.Visible = true;

                    try
                    {
                        pbMain.LoadAsync(path);
                    }
                    catch
                    {
                        lblStatus.Visible = false;
                        isImageLoading = false;
                        Reset();
                    }
                }
            }
        }

        public void Reset()
        {
            if (!isImageLoading && Image != null)
            {
                Image temp = null;

                try
                {
                    temp = Image;
                    Image = null;
                }
                finally
                {
                    // If error happened in previous image load then PictureBox set image as error image and if we dispose it then error happens
                    if (temp != null && temp != pbMain.ErrorImage && temp != pbMain.InitialImage)
                    {
                        temp.Dispose();
                    }
                }
            }

            if (FullscreenOnClick && Cursor != Cursors.Default)
            {
                Cursor = Cursors.Default;
            }
        }

        private void AutoSetSizeMode()
        {
            if (IsValidImage)
            {
                if (Image.Width > pbMain.ClientSize.Width || Image.Height > pbMain.ClientSize.Height)
                {
                    pbMain.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pbMain.SizeMode = PictureBoxSizeMode.CenterImage;
                }

                if (FullscreenOnClick)
                {
                    Cursor = Cursors.Hand;
                }
            }

            UpdateImageSizeLabel();
        }

        private void PbMain_Resize(object sender, EventArgs e)
        {
            UpdateCheckers();
            AutoSetSizeMode();
        }

        private void PbMain_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lblStatus.Visible = false;
            isImageLoading = false;
            if (e.Error == null) AutoSetSizeMode();
        }

        private void PbMain_LoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (isImageLoading && e.ProgressPercentage < 100)
            {
                Text = string.Format(Resources.MyPictureBox_pbMain_LoadProgressChanged_Loading_image___0__, e.ProgressPercentage);
            }
        }

        private void PbMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (FullscreenOnClick && e.Button == MouseButtons.Left && IsValidImage)
            {
                pbMain.Enabled = false;
                ImageViewer.ShowImage(Image);
                pbMain.Enabled = true;
            }
        }

        private void PbMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (EnableRightClickMenu && e.Button == MouseButtons.Right && IsValidImage)
            {
                cmsMenu.Show(pbMain, e.X + 1, e.Y + 1);
            }
        }

        private void PbMain_MouseMove(object sender, MouseEventArgs e)
        {
            lblImageSize.Visible = ShowImageSizeLabel && IsValidImage && !new Rectangle(lblImageSize.Location, lblImageSize.Size).Contains(e.Location);
        }

        private void PbMain_MouseLeave(object sender, EventArgs e)
        {
            lblImageSize.Visible = false;
        }

        private void tsmiCopyImage_Click(object sender, EventArgs e)
        {
            if (IsValidImage)
            {
                ClipboardHelpers.CopyImage(Image);
            }
        }
    }
}