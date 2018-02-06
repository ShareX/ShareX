#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class StickerForm : Form
    {
        public List<StickerPackInfo> StickerPacks { get; set; }
        public string SelectedImageFile { get; set; }
        public int StickerSize { get; set; }

        private string[] imageFiles;

        public StickerForm(List<StickerPackInfo> stickerPacks, int stickerSize = 64)
        {
            StickerPacks = stickerPacks;
            StickerSize = stickerSize;

            InitializeComponent();
            Icon = ShareXResources.Icon;
            tsMain.Renderer = new CustomToolStripProfessionalRenderer();
            tsnudSize.NumericUpDownControl.Minimum = 16;
            tsnudSize.NumericUpDownControl.Maximum = 256;
            tsnudSize.NumericUpDownControl.Increment = 16;
            tsnudSize.NumericUpDownControl.TextAlign = HorizontalAlignment.Center;
            tsnudSize.NumericUpDownControl.SetValue(StickerSize);
            ilvStickers.SetRenderer(new StickerImageListViewRenderer());
            ilvStickers.ThumbnailSize = new Size(StickerSize, StickerSize);
            foreach (StickerPackInfo stickerPackInfo in StickerPacks)
            {
                tscbStickers.Items.Add(stickerPackInfo);
            }
            tscbStickers.SelectedIndex = 0;
            tstbSearch.Focus();
        }

        public void LoadImageFiles()
        {
            StickerPackInfo stickerPack = tscbStickers.SelectedItem as StickerPackInfo;

            if (stickerPack != null && Directory.Exists(stickerPack.FolderPath))
            {
                imageFiles = Directory.GetFiles(stickerPack.FolderPath).Where(x => Helpers.IsImageFile(x)).ToArray();

                UpdateImageFiles();
            }
        }

        private void UpdateImageFiles()
        {
            ilvStickers.Items.Clear();

            string search = tstbSearch.Text;

            string[] currentImageFiles = imageFiles;

            if (!string.IsNullOrEmpty(search))
            {
                currentImageFiles = imageFiles.Where(x => x.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            }

            ilvStickers.Items.AddRange(currentImageFiles);
        }

        private void StickerForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
            tstbSearch.Focus();
        }

        private void tstbSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateImageFiles();
        }

        private void tsnudSize_ValueChanged(object sender, EventArgs e)
        {
            int size = (int)tsnudSize.NumericUpDownControl.Value;
            ilvStickers.ThumbnailSize = new Size(size, size);
        }

        private void ilvStickers_ItemClick(object sender, Manina.Windows.Forms.ItemClickEventArgs e)
        {
            SelectedImageFile = e.Item.FileName;
            StickerSize = (int)tsnudSize.NumericUpDownControl.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void tscbStickers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadImageFiles();
        }
    }
}