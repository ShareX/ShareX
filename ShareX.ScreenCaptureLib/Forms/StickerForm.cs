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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class StickerForm : Form
    {
        public List<StickerPackInfo> StickerPacks { get; set; }
        public int SelectedStickerPack { get; set; }
        public int StickerSize { get; set; }
        public string SelectedImageFile { get; set; }

        private string[] imageFiles;

        public StickerForm(List<StickerPackInfo> stickerPacks, int selectedStickerPack, int stickerSize = 64)
        {
            StickerPacks = stickerPacks;
            SelectedStickerPack = selectedStickerPack;
            StickerSize = stickerSize;

            InitializeComponent();
            tsMain.Renderer = new ToolStripRoundedEdgeRenderer();
            ShareXResources.ApplyTheme(this, true);

            tsnudSize.NumericUpDownControl.Minimum = 16;
            tsnudSize.NumericUpDownControl.Maximum = 256;
            tsnudSize.NumericUpDownControl.Increment = 16;
            tsnudSize.NumericUpDownControl.TextAlign = HorizontalAlignment.Center;
            tsnudSize.NumericUpDownControl.SetValue(StickerSize);
            ilvStickers.SetRenderer(new StickerImageListViewRenderer());
            ilvStickers.ThumbnailSize = new Size(StickerSize, StickerSize);
            UpdateStickerPacks();
            tstbSearch.Focus();
        }

        private void UpdateStickerPacks()
        {
            imageFiles = null;
            ilvStickers.Items.Clear();
            tscbStickers.Items.Clear();

            foreach (StickerPackInfo stickerPackInfo in StickerPacks)
            {
                tscbStickers.Items.Add(stickerPackInfo);
            }

            if (tscbStickers.Items.Count > SelectedStickerPack)
            {
                tscbStickers.SelectedIndex = SelectedStickerPack;
            }
            else if (tscbStickers.Items.Count > 0)
            {
                tscbStickers.SelectedIndex = 0;
            }
        }

        private void LoadImageFiles()
        {
            imageFiles = null;
            ilvStickers.Items.Clear();

            if (tscbStickers.SelectedItem is StickerPackInfo stickerPack && !string.IsNullOrEmpty(stickerPack.FolderPath))
            {
                string folderPath = FileHelpers.GetAbsolutePath(stickerPack.FolderPath);

                if (Directory.Exists(folderPath))
                {
                    imageFiles = Directory.GetFiles(folderPath).Where(x => FileHelpers.IsImageFile(x)).ToArray();

                    UpdateImageFiles();
                }
            }
        }

        private void UpdateImageFiles()
        {
            ilvStickers.Items.Clear();

            if (imageFiles != null && imageFiles.Length > 0)
            {
                string[] currentImageFiles = imageFiles;

                string search = tstbSearch.Text;

                if (!string.IsNullOrEmpty(search))
                {
                    currentImageFiles = imageFiles.Where(x => x.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                }

                ilvStickers.Items.AddRange(currentImageFiles);
            }
        }

        private void Close(string filePath)
        {
            SelectedImageFile = filePath;
            StickerSize = (int)tsnudSize.NumericUpDownControl.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void StickerForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
            tstbSearch.Focus();
        }

        private void tstbSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateImageFiles();

            if (ilvStickers.Items.Count > 0 && !string.IsNullOrEmpty(tstbSearch.Text))
            {
                ilvStickers.Items[0].Selected = true;
            }
        }

        private void tstbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void tstbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (ilvStickers.Items.Count > 0)
                {
                    Close(ilvStickers.Items[0].FileName);
                }
            }
        }

        private void tscbStickers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadImageFiles();

            SelectedStickerPack = tscbStickers.SelectedIndex;
        }

        private void tsbEditStickers_Click(object sender, EventArgs e)
        {
            using (StickerPackForm stickerPackForm = new StickerPackForm(StickerPacks))
            {
                stickerPackForm.ShowDialog(this);

                UpdateStickerPacks();
            }
        }

        private void tsnudSize_ValueChanged(object sender, EventArgs e)
        {
            int size = (int)tsnudSize.NumericUpDownControl.Value;
            ilvStickers.ThumbnailSize = new Size(size, size);
        }

        private void ilvStickers_ItemClick(object sender, Manina.Windows.Forms.ItemClickEventArgs e)
        {
            Close(e.Item.FileName);
        }
    }
}