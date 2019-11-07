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
        public ImageSplitterForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);
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

        private void BtnOutputFolderBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFolder(txtOutputFolder);
        }

        private async void BtnSplitImage_Click(object sender, EventArgs e)
        {
            string filePath = txtImageFilePath.Text;
            int rowCount = (int)nudRowCount.Value;
            int columnCount = (int)nudColumnCount.Value;
            string outputFolder = txtOutputFolder.Text;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && (rowCount > 1 || columnCount > 1) &&
                !string.IsNullOrEmpty(outputFolder) && Directory.Exists(outputFolder))
            {
                btnSplitImage.Enabled = false;

                try
                {
                    List<string> filePaths = await SplitImageAsync(filePath, rowCount, columnCount, outputFolder);

                    if (filePaths.Count > 0)
                    {
                        Helpers.OpenFolderWithFile(filePaths[0]);
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }

                btnSplitImage.Enabled = true;
            }
        }

        private List<string> SplitImage(string filePath, int rowCount, int columnCount, string outputFolder)
        {
            List<string> filePaths = new List<string>();

            Image img = ImageHelpers.LoadImage(filePath);

            if (img != null)
            {
                List<Image> images = ImageHelpers.SplitImage(img, rowCount, columnCount);

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

        private async Task<List<string>> SplitImageAsync(string filePath, int rowCount, int columnCount, string outputFolder)
        {
            return await Task.Run(() =>
            {
                return SplitImage(filePath, rowCount, columnCount, outputFolder);
            });
        }

        private void btnCopyDiscordEmoji_Click(object sender, EventArgs e)
        {
            string filePath = txtImageFilePath.Text;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            int rowCount = (int)nudRowCount.Value;
            int columnCount = (int)nudColumnCount.Value;

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

            Clipboard.SetText(text);
        }
    }
}