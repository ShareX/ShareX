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
using ShareX.MediaLib.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class ImageThumbnailerForm : Form
    {
        public ImageThumbnailerForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
        }

        private void UpdateEnabled()
        {
            btnRemove.Enabled = lvImages.SelectedItems.Count > 0;
            btnGenerate.Enabled = lvImages.Items.Count > 0 && nudWidth.Value > 0 && nudHeight.Value > 0 && !string.IsNullOrEmpty(txtOutputFolder.Text) &&
                !string.IsNullOrEmpty(txtOutputFilename.Text);
        }

        private void AddFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                lvImages.Items.Add(filePath);

                if (string.IsNullOrEmpty(txtOutputFolder.Text))
                {
                    txtOutputFolder.Text = Path.GetDirectoryName(filePath);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string[] images = ImageHelpers.OpenImageFileDialog(true);

            if (images != null)
            {
                foreach (string image in images)
                {
                    AddFile(image);
                }

                UpdateEnabled();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvImages.SelectedItems)
                {
                    lvImages.Items.Remove(lvi);
                }
            }
        }

        private void lvImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void lvImages_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void lvImages_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files)
            {
                foreach (string file in files)
                {
                    AddFile(file);
                }

                UpdateEnabled();
            }
        }

        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void btnOutputFolder_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtOutputFolder);
        }

        private void txtOutputFilename_TextChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (lvImages.Items.Count > 0)
            {
                int width = (int)nudWidth.Value;
                int height = (int)nudHeight.Value;
                int quality = (int)nudQuality.Value;
                string outputFolder = txtOutputFolder.Text;
                string outputFileName = txtOutputFilename.Text;

                Cursor = Cursors.WaitCursor;

                try
                {
                    foreach (ListViewItem lvi in lvImages.Items)
                    {
                        string filePath = lvi.Text;

                        if (File.Exists(filePath))
                        {
                            using (Bitmap bmp = ImageHelpers.LoadImage(filePath))
                            {
                                if (bmp != null)
                                {
                                    using (Bitmap thumbnail = ImageHelpers.CreateThumbnail(bmp, width, height))
                                    {
                                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                                        string outputPath = Path.Combine(outputFolder, outputFileName.Replace("$filename", fileName));
                                        outputPath = Path.ChangeExtension(outputPath, "jpg");

                                        using (Bitmap newImage = ImageHelpers.FillBackground(thumbnail, Color.White))
                                        {
                                            ImageHelpers.SaveJPEG(newImage, outputPath, quality);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Cursor = Cursors.Default;
                    MessageBox.Show(Resources.ThumbnailsSuccessfullyGenerated, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    Cursor = Cursors.Default;
                    ex.ShowError();
                }
            }
        }
    }
}