#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ImageViewer : Form
    {
        public Image CurrentImage { get; private set; }
        public string CurrentImageFilePath { get; private set; }
        public bool SupportsImageNavigation => Images != null && Images.Length > 0;
        public string[] Images { get; private set; }
        public int CurrentImageIndex { get; private set; }

        private ImageViewer(Image img)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            LoadImage(img);
        }

        private ImageViewer(string[] images, int currentImageIndex = 0)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            Images = images;
            CurrentImageIndex = currentImageIndex;
            FilterImageFiles();
            LoadCurrentImage();
        }

        private void LoadImage(Image img)
        {
            CurrentImage?.Dispose();
            CurrentImage = img;
            pbPreview.LoadImage(CurrentImage);
        }

        private void LoadCurrentImage()
        {
            if (!SupportsImageNavigation) return;

            CurrentImageIndex = CurrentImageIndex.Clamp(0, Images.Length - 1);
            CurrentImageFilePath = Images[CurrentImageIndex];
            Image img = ImageHelpers.LoadImage(CurrentImageFilePath);
            LoadImage(img);
            UpdateIndexLabel();
        }

        private void NavigateImage(int position)
        {
            if (!SupportsImageNavigation || Images.Length < 2) return;

            int nextImageIndex = CurrentImageIndex + position;

            if (nextImageIndex > Images.Length - 1)
            {
                nextImageIndex = 0;
            }
            else if (nextImageIndex < 0)
            {
                nextImageIndex = Images.Length - 1;
            }

            if (CurrentImageIndex != nextImageIndex)
            {
                CurrentImageIndex = nextImageIndex;
                LoadCurrentImage();
            }
        }

        private void FilterImageFiles()
        {
            List<string> filteredImages = new List<string>();

            for (int i = 0; i < Images.Length; i++)
            {
                string imageFilePath = Images[i];

                bool isImageFile = !string.IsNullOrEmpty(imageFilePath) && Helpers.IsImageFile(imageFilePath);

                if (i == CurrentImageIndex)
                {
                    if (isImageFile)
                    {
                        CurrentImageIndex = filteredImages.Count;
                    }
                    else
                    {
                        CurrentImageIndex = 0;
                    }
                }

                if (isImageFile)
                {
                    filteredImages.Add(imageFilePath);
                }
            }

            Images = filteredImages.ToArray();
        }

        private void UpdateIndexLabel()
        {
            if (!SupportsImageNavigation || Images.Length < 2) return;

            string status = CurrentImageIndex + 1 + " / " + Images.Length;
            string fileName = Helpers.GetFileNameSafe(CurrentImageFilePath);
            if (!string.IsNullOrEmpty(fileName))
            {
                status += "  " + fileName;
            }
            lblStatus.Text = status;
            lblStatus.Visible = true;
            lblStatus.Location = new Point((ClientSize.Width - lblStatus.Width) / 2, -1);
        }

        public static void ShowImage(Image img)
        {
            if (img != null)
            {
                using (Image tempImage = img.CloneSafe())
                {
                    if (tempImage != null)
                    {
                        using (ImageViewer viewer = new ImageViewer(tempImage))
                        {
                            viewer.ShowDialog();
                        }
                    }
                }
            }
        }

        public static void ShowImage(string filePath)
        {
            using (Bitmap bmp = ImageHelpers.LoadImage(filePath))
            {
                if (bmp != null)
                {
                    using (ImageViewer viewer = new ImageViewer(bmp))
                    {
                        viewer.ShowDialog();
                    }
                }
            }
        }

        public static void ShowImage(string[] images, int currentImageIndex = 0)
        {
            if (images != null && images.Length > 0)
            {
                using (ImageViewer viewer = new ImageViewer(images, currentImageIndex))
                {
                    viewer.ShowDialog();
                }
            }
        }

        private void ImageViewer_Shown(object sender, EventArgs e)
        {
            UpdateIndexLabel();

            this.ForceActivate();
        }

        private void ImageViewer_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void pbPreview_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void pbPreview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Enter:
                case Keys.Space:
                    Close();
                    break;
                case Keys.Left:
                    NavigateImage(-1);
                    break;
                case Keys.Right:
                    NavigateImage(1);
                    break;
            }
        }

        private void pbPreview_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            CurrentImage?.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pbPreview = new MyPictureBox();
            lblStatus = new Label();
            SuspendLayout();

            BackColor = SystemColors.Window;
            Bounds = CaptureHelpers.GetActiveScreenBounds();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            // TODO: Translate
            Text = "ShareX - Image viewer";
            TopMost = true;
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;

            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Arial", 14f);
            lblStatus.Padding = new Padding(5);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblStatus.Visible = false;
            Controls.Add(lblStatus);

            pbPreview.Cursor = Cursors.Hand;
            pbPreview.Dock = DockStyle.Fill;
            pbPreview.DrawCheckeredBackground = true;
            pbPreview.FullscreenOnClick = false;
            pbPreview.Location = new Point(0, 0);
            pbPreview.Name = "pbPreview";
            pbPreview.ShowImageSizeLabel = true;
            pbPreview.Size = new Size(96, 100);
            pbPreview.TabIndex = 0;
            Controls.Add(pbPreview);

            Shown += ImageViewer_Shown;
            Deactivate += ImageViewer_Deactivate;
            pbPreview.MouseDown += pbPreview_MouseDown;
            pbPreview.KeyDown += pbPreview_KeyDown;
            pbPreview.PreviewKeyDown += pbPreview_PreviewKeyDown;

            ResumeLayout(false);
        }

        private MyPictureBox pbPreview;
        private Label lblStatus;

        #endregion Windows Form Designer generated code
    }
}