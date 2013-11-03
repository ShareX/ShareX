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

using HelpersLib.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HelpersLib
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

        private readonly object ImageLoadLock = new object();

        private bool isImageLoading;

        public MyPictureBox()
        {
            InitializeComponent();
            Text = string.Empty;
            pbMain.InitialImage = Resources.Loading;
            pbMain.LoadProgressChanged += pbMain_LoadProgressChanged;
            pbMain.LoadCompleted += pbMain_LoadCompleted;
            pbMain.Resize += pbMain_Resize;
            pbMain.MouseUp += MyPictureBox_MouseUp;
            MouseDown += MyPictureBox_MouseDown;
        }

        private void pbMain_Resize(object sender, EventArgs e)
        {
            UpdateCheckers();
            AutoSetSizeMode();
        }

        private void UpdateCheckers()
        {
            if (DrawCheckeredBackground)
            {
                if (pbMain.BackgroundImage == null || pbMain.BackgroundImage.Size != pbMain.ClientSize)
                {
                    if (pbMain.BackgroundImage != null) pbMain.BackgroundImage.Dispose();
                    pbMain.BackgroundImage = ImageHelpers.CreateCheckers(8, Color.LightGray, Color.White);
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
            lock (ImageLoadLock)
            {
                if (!isImageLoading)
                {
                    isImageLoading = true;
                    Reset();
                    Image = (Image)img.Clone();
                    AutoSetSizeMode();
                    isImageLoading = false;
                }
            }
        }

        public void LoadImageFromFile(string filePath)
        {
            lock (ImageLoadLock)
            {
                if (!isImageLoading)
                {
                    isImageLoading = true;
                    Reset();
                    Image = Helpers.GetImageFromFile(filePath);
                    AutoSetSizeMode();
                    isImageLoading = false;
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
            if (!string.IsNullOrEmpty(url))
            {
                LoadImageAsync(url);
            }
        }

        private void LoadImageAsync(string path)
        {
            lock (ImageLoadLock)
            {
                if (!isImageLoading)
                {
                    isImageLoading = true;
                    Reset();
                    Text = "Loading image...";
                    lblStatus.Visible = true;
                    pbMain.LoadAsync(path);
                }
            }
        }

        public void Reset()
        {
            if (!isImageLoading)
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }
            }

            if (FullscreenOnClick && Cursor != Cursors.Default)
            {
                Cursor = Cursors.Default;
            }
        }

        private void pbMain_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lblStatus.Visible = false;
            AutoSetSizeMode();
            isImageLoading = false;
        }

        private void pbMain_LoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100)
            {
                Text = string.Format("Loading image: {0}%", e.ProgressPercentage);
            }
        }

        private void AutoSetSizeMode()
        {
            if (Image != null)
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
        }

        private void MyPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && FullscreenOnClick && !isImageLoading && Image != null)
            {
                ImageViewer.ShowImage(Image);
            }
        }

        private void MyPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && EnableRightClickMenu && !isImageLoading && Image != null)
            {
                cmsMenu.Show(pbMain, e.X + 1, e.Y + 1);
            }
        }

        private void tsmiCopyImage_Click(object sender, EventArgs e)
        {
            if (!isImageLoading && Image != null)
            {
                ClipboardHelpers.CopyImage(Image);
            }
        }
    }
}