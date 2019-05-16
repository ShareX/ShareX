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

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ImageViewer : Form
    {
        private Image screenshot;

        private ImageViewer(Image image)
        {
            screenshot = image;

            InitializeComponent();
            Icon = ShareXResources.Icon;
        }

        public static void ShowImage(Image img)
        {
            if (img != null)
            {
                using (Image tempImage = (Image)img.Clone())
                using (ImageViewer viewer = new ImageViewer(tempImage))
                {
                    viewer.ShowDialog();
                }
            }
        }

        public static void ShowImage(string filepath)
        {
            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                using (Image img = ImageHelpers.LoadImage(filepath))
                using (ImageViewer viewer = new ImageViewer(img))
                {
                    viewer.ShowDialog();
                }
            }
        }

        private void ShowScreenshot_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void ShowScreenshot_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void pbPreview_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void pbPreview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                Close();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (screenshot != null)
            {
                screenshot.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pbPreview = new MyPictureBox();
            SuspendLayout();

            BackColor = SystemColors.Window;
            Bounds = CaptureHelpers.GetActiveScreenBounds();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Text = "ShareX - Image viewer";
            TopMost = true;
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;

            pbPreview.Cursor = Cursors.Hand;
            pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            pbPreview.DrawCheckeredBackground = true;
            pbPreview.FullscreenOnClick = false;
            pbPreview.Location = new System.Drawing.Point(0, 0);
            pbPreview.Name = "pbPreview";
            pbPreview.Size = new System.Drawing.Size(96, 100);
            pbPreview.TabIndex = 0;
            pbPreview.LoadImage(screenshot);
            Controls.Add(pbPreview);

            Shown += new System.EventHandler(ShowScreenshot_Shown);
            Deactivate += new System.EventHandler(ShowScreenshot_Deactivate);
            pbPreview.MouseDown += pbPreview_MouseDown;
            pbPreview.KeyDown += pbPreview_KeyDown;

            ResumeLayout(false);
        }

        private MyPictureBox pbPreview;

        #endregion Windows Form Designer generated code
    }
}