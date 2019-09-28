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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

        private async void BtnSplitImage_Click(object sender, EventArgs e)
        {
            btnSplitImage.Enabled = false;

            string filePath = txtImageFilePath.Text;
            int rowCount = (int)nudRowCount.Value;
            int columnCount = (int)nudColumnCount.Value;
            string outputFolder = txtOutputFolder.Text;

            await SplitImageAsync(filePath, rowCount, columnCount, outputFolder);

            btnSplitImage.Enabled = true;
        }

        private void SplitImage(string filePath, int rowCount, int columnCount, string outputFolder)
        {
            Image img = ImageHelpers.LoadImage(filePath);

            if (img != null)
            {
                List<Image> images = ImageHelpers.SplitImage(img, rowCount, columnCount);

                for (int i = 0; i < images.Count; i++)
                {
                    string filename = Path.GetFileNameWithoutExtension(filePath) + (i + 1) + ".png";
                    string outputPath = Path.Combine(outputFolder, filename);
                    images[i].Save(outputPath, ImageFormat.Png);
                }
            }
        }

        private async Task SplitImageAsync(string filePath, int rowCount, int columnCount, string outputFolder)
        {
            await Task.Run(() =>
            {
                SplitImage(filePath, rowCount, columnCount, outputFolder);
            });
        }
    }
}