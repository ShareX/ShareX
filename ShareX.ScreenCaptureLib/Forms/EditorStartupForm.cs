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
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class EditorStartupForm : Form
    {
        public RegionCaptureOptions Options { get; private set; }
        public Bitmap Image { get; private set; }
        public string ImageFilePath { get; private set; }

        public EditorStartupForm(RegionCaptureOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
        }

        private void LoadImageFile(string imageFilePath)
        {
            if (!string.IsNullOrEmpty(imageFilePath))
            {
                Image = ImageHelpers.LoadImage(imageFilePath);

                if (Image != null)
                {
                    ImageFilePath = imageFilePath;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void btnOpenImageFile_Click(object sender, EventArgs e)
        {
            string imageFilePath = ImageHelpers.OpenImageFileDialog(this);
            LoadImageFile(imageFilePath);
        }

        private void btnLoadImageFromClipboard_Click(object sender, EventArgs e)
        {
            if (ClipboardHelpers.ContainsImage())
            {
                Image = ClipboardHelpers.GetImage();

                if (Image != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else if (ClipboardHelpers.ContainsFileDropList())
            {
                string[] files = ClipboardHelpers.GetFileDropList();

                if (files != null)
                {
                    string imageFilePath = files.FirstOrDefault(x => FileHelpers.IsImageFile(x));
                    LoadImageFile(imageFilePath);
                }
            }
            else
            {
                MessageBox.Show(Resources.EditorStartupForm_ClipboardDoesNotContainAnImage, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnLoadImageFromURL_Click(object sender, EventArgs e)
        {
            string inputText = null;

            string text = ClipboardHelpers.GetText(true);

            if (URLHelpers.IsValidURL(text))
            {
                inputText = text;
            }

            string url = InputBox.Show(Resources.ImageURL, inputText);

            if (!string.IsNullOrEmpty(url))
            {
                btnOpenImageFile.Enabled = btnLoadImageFromClipboard.Enabled = btnLoadImageFromURL.Enabled = btnCreateNewImage.Enabled = false;
                Cursor = Cursors.WaitCursor;

                try
                {
                    Image = await WebHelpers.DownloadImageAsync(url);

                    if (IsDisposed)
                    {
                        Image?.Dispose();

                        return;
                    }
                    else if (Image != null)
                    {
                        DialogResult = DialogResult.OK;
                        Close();

                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }

                Cursor = Cursors.Default;
                btnOpenImageFile.Enabled = btnLoadImageFromClipboard.Enabled = btnLoadImageFromURL.Enabled = btnCreateNewImage.Enabled = true;
            }
        }

        private void btnCreateNewImage_Click(object sender, EventArgs e)
        {
            Image = NewImageForm.CreateNewImage(Options, this);

            if (Image != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}