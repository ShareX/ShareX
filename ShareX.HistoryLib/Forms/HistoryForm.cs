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
using ShareX.HistoryLib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public partial class HistoryForm : Form
    {
        public string HistoryPath { get; private set; }
        public HistorySettings Settings { get; private set; }

        private HistoryManager history;
        private HistoryItemManager him;
        private HistoryItem[] allHistoryItems;
        private string defaultTitle;

        public HistoryForm(string historyPath, HistorySettings settings, Action<string> uploadFile = null, Action<string> editImage = null)
        {
            HistoryPath = historyPath;
            Settings = settings;

            InitializeComponent();
            Icon = ShareXResources.Icon;
            defaultTitle = Text;
            UpdateTitle();

            // Mark the Date column as having a date; used for sorting
            chDateTime.Tag = new DateTime();

            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add(Resources.image);
            il.Images.Add(Resources.notebook);
            il.Images.Add(Resources.application_block);
            il.Images.Add(Resources.globe);
            lvHistory.SmallImageList = il;

            him = new HistoryItemManager(uploadFile, editImage);
            him.GetHistoryItems += him_GetHistoryItems;

            pbThumbnail.Reset();
            lvHistory.FillLastColumn();

            if (Settings.SplitterDistance > 0)
            {
                scMain.SplitterDistance = Settings.SplitterDistance;
            }

            Settings.WindowState.AutoHandleFormState(this);
        }

        private void RefreshHistoryItems()
        {
            allHistoryItems = GetHistoryItems();
            ApplyFiltersAndAdd();
        }

        private HistoryItem[] him_GetHistoryItems()
        {
            return lvHistory.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as HistoryItem).ToArray();
        }

        private HistoryItem[] GetHistoryItems()
        {
            if (history == null)
            {
                history = new HistoryManager(HistoryPath);
            }

            IEnumerable<HistoryItem> tempHistoryItems = history.GetHistoryItems();
            tempHistoryItems = tempHistoryItems.Reverse();

            if (Settings.MaxItemCount > 0)
            {
                tempHistoryItems = tempHistoryItems.Take(Settings.MaxItemCount);
            }

            return tempHistoryItems.ToArray();
        }

        private void ApplyFiltersAndAdd()
        {
            if (allHistoryItems.Length > 0)
            {
                AddHistoryItems(ApplyFilters(allHistoryItems));
            }
        }

        private HistoryItem[] ApplyFilters(HistoryItem[] historyItems)
        {
            if (!cbTypeFilter.Checked && !cbHostFilter.Checked && string.IsNullOrEmpty(txtFilenameFilter.Text) && string.IsNullOrEmpty(txtURLFilter.Text) && !cbDateFilter.Checked)
            {
                return historyItems;
            }

            IEnumerable<HistoryItem> result = historyItems.AsEnumerable();

            if (cbTypeFilter.Checked)
            {
                string type = cbTypeFilterSelection.Text;

                if (!string.IsNullOrEmpty(type))
                {
                    result = result.Where(x => !string.IsNullOrEmpty(x.Type) && x.Type.Equals(type, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            if (cbHostFilter.Checked)
            {
                string host = cbHostFilterSelection.Text;

                if (!string.IsNullOrEmpty(host))
                {
                    result = result.Where(x => !string.IsNullOrEmpty(x.Host) && x.Host.Contains(host, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            string filenameFilter = txtFilenameFilter.Text;

            if (!string.IsNullOrEmpty(filenameFilter))
            {
                result = result.Where(x => x.Filename != null && x.Filename.Contains(filenameFilter, StringComparison.InvariantCultureIgnoreCase));
            }

            string urlFilter = txtURLFilter.Text;

            if (!string.IsNullOrEmpty(urlFilter))
            {
                result = result.Where(x => x.URL != null && x.URL.Contains(urlFilter, StringComparison.InvariantCultureIgnoreCase));
            }

            if (cbDateFilter.Checked)
            {
                DateTime fromDate = dtpFilterFrom.Value.Date;
                DateTime toDate = dtpFilterTo.Value.Date;

                result = result.Where(x => x.DateTime.Date >= fromDate && x.DateTime.Date <= toDate);
            }

            return result.ToArray();
        }

        private void AddHistoryItems(HistoryItem[] historyItems)
        {
            Cursor = Cursors.WaitCursor;

            UpdateTitle(historyItems);

            lvHistory.Items.Clear();

            ListViewItem[] listViewItems = new ListViewItem[historyItems.Length];

            for (int i = 0; i < historyItems.Length; i++)
            {
                HistoryItem hi = historyItems[i];
                ListViewItem lvi = listViewItems[i] = new ListViewItem();

                if (hi.Type.Equals("Image", StringComparison.InvariantCultureIgnoreCase))
                {
                    lvi.ImageIndex = 0;
                }
                else if (hi.Type.Equals("Text", StringComparison.InvariantCultureIgnoreCase))
                {
                    lvi.ImageIndex = 1;
                }
                else if (hi.Type.Equals("File", StringComparison.InvariantCultureIgnoreCase))
                {
                    lvi.ImageIndex = 2;
                }
                else
                {
                    lvi.ImageIndex = 3;
                }

                lvi.SubItems.Add(hi.DateTime.ToString()).Tag = hi.DateTime;
                lvi.SubItems.Add(hi.Filename);
                lvi.SubItems.Add(hi.URL);
                lvi.Tag = hi;
            }

            lvHistory.Items.AddRange(listViewItems);
            lvHistory.FillLastColumn();
            lvHistory.Focus();

            Cursor = Cursors.Default;
        }

        private void UpdateTitle(HistoryItem[] historyItems = null)
        {
            string title = defaultTitle;

            if (historyItems != null)
            {
                StringBuilder status = new StringBuilder();

                status.Append(" (");
                status.AppendFormat(Resources.HistoryForm_UpdateItemCount_Total___0_, allHistoryItems.Length.ToString("N0"));

                if (allHistoryItems.Length > historyItems.Length)
                {
                    status.AppendFormat(" - " + Resources.HistoryForm_UpdateItemCount___Filtered___0_, historyItems.Length.ToString("N0"));
                }

                IEnumerable<string> types = from hi in historyItems
                                            group hi by hi.Type
                                            into t
                                            let count = t.Count()
                                            select string.Format(" - {0}: {1:N0}", t.Key, count);

                foreach (string type in types)
                {
                    status.Append(type);
                }

                status.Append(")");
                title += status.ToString();
            }

            Text = title;
        }

        private void UpdateControls()
        {
            switch (him.RefreshInfo())
            {
                case HistoryRefreshInfoResult.Success:
                    UpdatePictureBox();
                    break;
                case HistoryRefreshInfoResult.Invalid:
                    pbThumbnail.Reset();
                    break;
            }
        }

        private void UpdatePictureBox()
        {
            pbThumbnail.Reset();

            if (him != null)
            {
                if (him.IsImageFile)
                {
                    pbThumbnail.LoadImageFromFileAsync(him.HistoryItem.Filepath);
                }
                else if (him.IsImageURL)
                {
                    pbThumbnail.LoadImageFromURLAsync(him.HistoryItem.URL);
                }
            }
        }

        #region Form events

        private void HistoryForm_Shown(object sender, EventArgs e)
        {
            Refresh();

            RefreshHistoryItems();

            if (lvHistory.Items.Count > 0)
            {
                lvHistory.Items[0].Selected = true;

                cbTypeFilterSelection.Items.Clear();
                cbTypeFilterSelection.Items.AddRange(allHistoryItems.Select(x => x.Type).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToArray());

                if (cbTypeFilterSelection.Items.Count > 0)
                {
                    cbTypeFilterSelection.SelectedIndex = 0;
                }

                cbHostFilterSelection.Items.Clear();
                cbHostFilterSelection.Items.AddRange(allHistoryItems.Select(x => x.Host).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToArray());
            }

            this.ForceActivate();
        }

        private void HistoryForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void HistoryForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    RefreshHistoryItems();
                    e.Handled = true;
                    break;
            }
        }

        private void scMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.SplitterDistance = scMain.SplitterDistance;
        }

        private void btnApplyFilters_Click(object sender, EventArgs e)
        {
            ApplyFiltersAndAdd();
        }

        private void btnRemoveFilters_Click(object sender, EventArgs e)
        {
            AddHistoryItems(allHistoryItems);
        }

        private void lvHistory_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                UpdateControls();
            }
        }

        private void lvHistory_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                him.cmsHistory.Show(lvHistory, e.X + 1, e.Y + 1);
            }
        }

        private void lvHistory_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (him != null && e.Button == MouseButtons.Left)
            {
                him.TryOpen();
            }
        }

        private void lvHistory_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                default:
                    return;
                case Keys.Enter:
                    him.TryOpen();
                    break;
                case Keys.Control | Keys.Enter:
                    him.OpenFile();
                    break;
                case Keys.Control | Keys.C:
                    him.CopyURL();
                    break;
            }

            e.Handled = true;
        }

        private void lvHistory_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<string> selection = new List<string>();

            foreach (ListViewItem item in lvHistory.SelectedItems)
            {
                HistoryItem hi = (HistoryItem)item.Tag;
                if (File.Exists(hi.Filepath))
                {
                    selection.Add(hi.Filepath);
                }
            }

            if (selection.Count > 0)
            {
                DataObject data = new DataObject(DataFormats.FileDrop, selection.ToArray());
                DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        #endregion Form events
    }
}