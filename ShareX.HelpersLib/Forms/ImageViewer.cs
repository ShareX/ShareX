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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ImageViewer : Form
    {
        public Image CurrentImage { get; private set; }
        public string CurrentImageFilePath { get; private set; }
        public bool SupportWrap { get; set; }
        public bool CanNavigate => Images != null && Images.Length > 1;
        public bool CanNavigateLeft => CanNavigate && (SupportWrap || CurrentImageIndex > 0);
        public bool CanNavigateRight => CanNavigate && (SupportWrap || CurrentImageIndex < Images.Length - 1);
        public string[] Images { get; private set; }
        public int CurrentImageIndex { get; private set; }
        public int NavigationButtonWidth { get; set; } = 100;
        public string Status { get; private set; }

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
            if (Images != null && Images.Length > 0)
            {
                CurrentImageIndex = CurrentImageIndex.Clamp(0, Images.Length - 1);
                CurrentImageFilePath = Images[CurrentImageIndex];
                Image img = ImageHelpers.LoadImage(CurrentImageFilePath);
                LoadImage(img);
            }

            UpdateStatus();
        }

        private void NavigateImage(int position)
        {
            if (CanNavigate)
            {
                int nextImageIndex = CurrentImageIndex + position;

                if (SupportWrap)
                {
                    if (nextImageIndex > Images.Length - 1)
                    {
                        nextImageIndex = 0;
                    }
                    else if (nextImageIndex < 0)
                    {
                        nextImageIndex = Images.Length - 1;
                    }
                }

                nextImageIndex = nextImageIndex.Clamp(0, Images.Length - 1);

                if (CurrentImageIndex != nextImageIndex)
                {
                    CurrentImageIndex = nextImageIndex;
                    LoadCurrentImage();
                }
            }
        }

        private void FilterImageFiles()
        {
            List<string> filteredImages = new List<string>();

            for (int i = 0; i < Images.Length; i++)
            {
                string imageFilePath = Images[i];

                bool isImageFile = !string.IsNullOrEmpty(imageFilePath) && FileHelpers.IsImageFile(imageFilePath);

                if (i == CurrentImageIndex)
                {
                    if (isImageFile)
                    {
                        CurrentImageIndex = filteredImages.Count;
                    }
                    else
                    {
                        CurrentImageIndex = Math.Max(filteredImages.Count - 1, 0);
                    }
                }

                if (isImageFile)
                {
                    filteredImages.Add(imageFilePath);
                }
            }

            Images = filteredImages.ToArray();
        }

        private void UpdateStatus()
        {
            Status = "";

            if (CanNavigate)
            {
                AppendStatus($"{CurrentImageIndex + 1} / {Images.Length}");
            }

            string fileName = FileHelpers.GetFileNameSafe(CurrentImageFilePath);

            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Truncate(128, "...");
                AppendStatus(fileName);
            }

            if (CurrentImage != null)
            {
                AppendStatus($"{CurrentImage.Width} x {CurrentImage.Height}");
            }

            lblStatus.Visible = !string.IsNullOrEmpty(Status);
            lblStatus.Text = Status;
            lblStatus.Location = new Point((ClientSize.Width - lblStatus.Width) / 2, 0);
        }

        private void AppendStatus(string text)
        {
            if (!string.IsNullOrEmpty(Status))
            {
                Status += " │ ";
            }

            Status += text;
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

        public static void ShowImage(string[] files, int imageIndex = 0)
        {
            if (files != null && files.Length > 0)
            {
                using (ImageViewer viewer = new ImageViewer(files, imageIndex))
                {
                    viewer.ShowDialog();
                }
            }
        }

        private void ImageViewer_Shown(object sender, EventArgs e)
        {
            UpdateStatus();

            this.ForceActivate();
        }

        private void ImageViewer_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void lblLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NavigateImage(-1);
                lblLeft.Visible = CanNavigateLeft;
            }
        }

        private void lblRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NavigateImage(1);
                lblRight.Visible = CanNavigateRight;
            }
        }

        private void pbPreview_MouseClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void pbPreview_MouseMove(object sender, MouseEventArgs e)
        {
            lblStatus.Visible = !string.IsNullOrEmpty(Status) && !new Rectangle(lblStatus.Location, lblStatus.Size).Contains(e.Location);
            lblLeft.Visible = CanNavigateLeft && new Rectangle(lblLeft.Location, lblLeft.Size).Contains(e.Location);
            lblRight.Visible = CanNavigateRight && new Rectangle(lblRight.Location, lblRight.Size).Contains(e.Location);
        }

        private void pbPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            if (CanNavigateLeft && e.Delta > 0)
            {
                NavigateImage(-1);
            }
            else if (CanNavigateRight && e.Delta < 0)
            {
                NavigateImage(1);
            }
        }

        private void pbPreview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    NavigateImage(-1);
                    break;
                case Keys.Right:
                    NavigateImage(1);
                    break;
            }
        }

        private void pbPreview_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Enter:
                case Keys.Space:
                    Close();
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

        private void lblStatus_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Visible = false;
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
            lblLeft = new Label();
            lblRight = new Label();
            SuspendLayout();

            BackColor = SystemColors.Window;
            Bounds = CaptureHelpers.GetActiveScreenBounds();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Text = Resources.ShareXImageViewer;
            TopMost = true;
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;

            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Arial", 13f);
            lblStatus.Padding = new Padding(5);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(lblStatus);

            lblLeft.Cursor = Cursors.Hand;
            lblLeft.Font = new Font("Arial", 50f, FontStyle.Bold);
            lblLeft.Location = new Point(0, 0);
            lblLeft.Text = "‹";
            lblLeft.TextAlign = ContentAlignment.MiddleCenter;
            lblLeft.Size = new Size(NavigationButtonWidth, Bounds.Height);
            lblLeft.MouseDown += lblLeft_MouseDown;
            Controls.Add(lblLeft);

            lblRight.Cursor = Cursors.Hand;
            lblRight.Font = new Font("Arial", 50f, FontStyle.Bold);
            lblRight.Location = new Point(Bounds.Width - NavigationButtonWidth, 0);
            lblRight.Text = "›";
            lblRight.TextAlign = ContentAlignment.MiddleCenter;
            lblRight.Size = new Size(NavigationButtonWidth, Bounds.Height);
            lblRight.MouseDown += lblRight_MouseDown;
            Controls.Add(lblRight);

            pbPreview.Dock = DockStyle.Fill;
            pbPreview.DrawCheckeredBackground = true;
            pbPreview.Location = new Point(0, 0);
            pbPreview.Size = new Size(100, 100);
            pbPreview.TabIndex = 0;
            Controls.Add(pbPreview);

            Shown += ImageViewer_Shown;
            Deactivate += ImageViewer_Deactivate;
            pbPreview.MouseClick += pbPreview_MouseClick;
            pbPreview.MouseMove += pbPreview_MouseMove;
            pbPreview.MouseWheel += pbPreview_MouseWheel;
            pbPreview.KeyDown += pbPreview_KeyDown;
            pbPreview.KeyUp += pbPreview_KeyUp;
            pbPreview.PreviewKeyDown += pbPreview_PreviewKeyDown;
            lblStatus.MouseEnter += lblStatus_MouseEnter;

            ResumeLayout(false);
        }

        private MyPictureBox pbPreview;
        private Label lblStatus;
        private Label lblLeft;
        private Label lblRight;

        #endregion Windows Form Designer generated code
    }
}