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
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class StickerPackForm : Form
    {
        public List<StickerPackInfo> Stickers { get; private set; }

        public StickerPackForm(List<StickerPackInfo> stickers)
        {
            Stickers = stickers;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            foreach (StickerPackInfo stickerPackInfo in Stickers)
            {
                cbStickers.Items.Add(stickerPackInfo);
            }

            if (cbStickers.Items.Count > 0)
            {
                cbStickers.SelectedIndex = 0;
            }

            UpdateEnabledStates();
        }

        private StickerPackInfo GetCurrentStickerPack()
        {
            if (cbStickers.SelectedIndex > -1)
            {
                return cbStickers.SelectedItem as StickerPackInfo;
            }

            return null;
        }

        private void UpdateEnabledStates()
        {
            cbStickers.Enabled = btnRemove.Enabled = txtFolder.Enabled = btnFolderBrowse.Enabled = txtName.Enabled = cbStickers.SelectedIndex > -1;
        }

        private void cbStickers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabledStates();

            StickerPackInfo stickerPackInfo = GetCurrentStickerPack();

            if (stickerPackInfo != null)
            {
                txtFolder.Text = stickerPackInfo.FolderPath;
                txtName.Text = stickerPackInfo.Name;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (FolderSelectDialog fsd = new FolderSelectDialog())
            {
                if (fsd.ShowDialog())
                {
                    StickerPackInfo stickerPackInfo = new StickerPackInfo(fsd.FileName);
                    Stickers.Add(stickerPackInfo);
                    cbStickers.Items.Add(stickerPackInfo);
                    cbStickers.SelectedIndex = cbStickers.Items.Count - 1;
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int selected = cbStickers.SelectedIndex;

            if (selected > -1)
            {
                Stickers.RemoveAt(selected);
                cbStickers.Items.RemoveAt(selected);
                cbStickers.SelectedIndex = cbStickers.Items.Count - 1;
            }

            UpdateEnabledStates();
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            StickerPackInfo stickerPackInfo = GetCurrentStickerPack();

            if (stickerPackInfo != null)
            {
                stickerPackInfo.FolderPath = txtFolder.Text;
                cbStickers.RefreshItems();
            }
        }

        private void btnFolderBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtFolder, txtFolder.Text);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            StickerPackInfo stickerPackInfo = GetCurrentStickerPack();

            if (stickerPackInfo != null)
            {
                stickerPackInfo.Name = txtName.Text;
                cbStickers.RefreshItems();
            }
        }
    }
}