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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class ImageSplitterForm : Form
    {
        public bool IsBusy { get; private set; }

        public ImageSplitterForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            btnSplitImage.Enabled = btnCopyChatEmoji.Enabled = !IsBusy && !string.IsNullOrEmpty(txtImageFilePath.Text) &&
                !string.IsNullOrEmpty(txtOutputFolder.Text) && (nudColumnCount.Value > 1 || nudRowCount.Value > 1);
            lblColumnRow.Text = nudColumnCount.Value + " x " + nudRowCount.Value;
        }

        private List<string> SplitImage(string filePath, int rowCount, int columnCount, string outputFolder)
        {
            List<string> filePaths = new List<string>();

            Bitmap bmp = ImageHelpers.LoadImage(filePath);

            if (bmp != null)
            {
                List<Bitmap> images = ImageHelpers.SplitImage(bmp, rowCount, columnCount);

                string originalFileName = Path.GetFileNameWithoutExtension(filePath);

                for (int i = 0; i < images.Count; i++)
                {
                    string fileName = originalFileName + (i + 1) + ".png";
                    string outputPath = Path.Combine(outputFolder, fileName);
                    images[i].Save(outputPath, ImageFormat.Png);
                    filePaths.Add(outputPath);
                }
            }

            return filePaths;
        }

        private Task<List<string>> SplitImageAsync(string filePath, int rowCount, int columnCount, string outputFolder)
        {
            return Task.Run(() =>
            {
                return SplitImage(filePath, rowCount, columnCount, outputFolder);
            });
        }

        private void txtImageFilePath_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void BtnImageFilePathBrowse_Click(object sender, EventArgs e)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            if (!string.IsNullOrEmpty(filePath))
            {
                txtImageFilePath.Text = filePath;

                if (string.IsNullOrEmpty(txtOutputFolder.Text))
                {
                    txtOutputFolder.Text = Path.GetDirectoryName(filePath);
                }
            }
        }

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void BtnOutputFolderBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtOutputFolder);
        }

        private void nudColumnCount_ValueChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void nudRowCount_ValueChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private async void BtnSplitImage_Click(object sender, EventArgs e)
        {
            string filePath = txtImageFilePath.Text;
            string outputFolder = txtOutputFolder.Text;
            int columnCount = (int)nudColumnCount.Value;
            int rowCount = (int)nudRowCount.Value;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && !string.IsNullOrEmpty(outputFolder) && Directory.Exists(outputFolder) &&
                (columnCount > 1 || rowCount > 1))
            {
                IsBusy = true;
                UpdateButtonStates();

                try
                {
                    List<string> filePaths = await SplitImageAsync(filePath, rowCount, columnCount, outputFolder);

                    if (filePaths.Count > 0)
                    {
                        FileHelpers.OpenFolderWithFile(filePaths[0]);
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }

                IsBusy = false;
                UpdateButtonStates();
            }
        }

        private void btnCopyChatEmoji_Click(object sender, EventArgs e)
        {
            string filePath = txtImageFilePath.Text;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            int columnCount = (int)nudColumnCount.Value;
            int rowCount = (int)nudRowCount.Value;

            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < columnCount; x++)
                {
                    int index = (y * columnCount) + x + 1;
                    sb.Append($":{fileName}{index}:");
                }

                if (y + 1 < rowCount)
                {
                    sb.AppendLine();
                }
            }

            string text = sb.ToString();

            ClipboardHelpers.CopyText(text);
        }
    }
}