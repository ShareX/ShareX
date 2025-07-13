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
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShareX.HistoryLib.Forms
{
    public partial class HistoryItemEditForm : Form
    {
        public HistoryItem HistoryItem { get; private set; }

        public HistoryItemEditForm(HistoryItem historyItem)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            this.HistoryItem = historyItem;

            LoadSettings();
        }

        private void LoadSettings()
        {
            txtFileName.Text = HistoryItem.FileName;
            txtFilePath.Text = HistoryItem.FilePath;
            txtDateTime.Text = HistoryItem.DateTime.ToString();
            txtType.Text = HistoryItem.Type;
            txtHost.Text = HistoryItem.Host;
            txtURL.Text = HistoryItem.URL;
            txtThumbnailURL.Text = HistoryItem.ThumbnailURL;
            txtDeletionURL.Text = HistoryItem.DeletionURL;
            txtShortenedURL.Text = HistoryItem.ShortenedURL;
            if (HistoryItem.Tags != null)
            {
                foreach (KeyValuePair<string, string> tag in HistoryItem.Tags)
                {
                    dgvTags.Rows.Add(tag.Key, tag.Value);
                }
            }
        }

        private void SaveSettings()
        {
            HistoryItem.FileName = txtFileName.Text;
            HistoryItem.FilePath = txtFilePath.Text;
            HistoryItem.Type = txtType.Text;
            HistoryItem.Host = txtHost.Text;
            HistoryItem.URL = txtURL.Text;
            HistoryItem.ThumbnailURL = txtThumbnailURL.Text;
            HistoryItem.DeletionURL = txtDeletionURL.Text;
            HistoryItem.ShortenedURL = txtShortenedURL.Text;
            HistoryItem.Tags = new Dictionary<string, string>();
            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                if (!string.IsNullOrEmpty(row.Cells[0].Value as string))
                {
                    HistoryItem.Tags[row.Cells[0].Value.ToString()] = row.Cells[1].Value?.ToString();
                }
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            SaveSettings();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}