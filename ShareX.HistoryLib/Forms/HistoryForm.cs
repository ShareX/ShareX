#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
            lvHistory.ContextMenuStrip = him.cmsHistory;

            pbThumbnail.Reset();
            lvHistory.FillLastColumn();

            if (Settings.SplitterDistance > 0)
            {
                scMain.SplitterDistance = Settings.SplitterDistance;
            }

            ShareXResources.ApplyTheme(this);

            Settings.WindowState.AutoHandleFormState(this);
        }

        private void ResetFilters()
        {
            tstbSearch.Text = "";
            txtFilenameFilter.ResetText();
            txtURLFilter.ResetText();
            cbDateFilter.Checked = false;
            dtpFilterFrom.ResetText();
            dtpFilterTo.ResetText();
            cbTypeFilter.Checked = false;
            if (cbTypeFilterSelection.Items.Count > 0)
            {
                cbTypeFilterSelection.SelectedIndex = 0;
            }
            cbHostFilter.Checked = false;
            cbHostFilterSelection.ResetText();
        }

        private void RefreshHistoryItems(bool mockData = false)
        {
            allHistoryItems = GetHistoryItems(mockData);
            ApplyFilterSimple();
        }

        private HistoryItem[] him_GetHistoryItems()
        {
            return lvHistory.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as HistoryItem).ToArray();
        }

        private HistoryItem[] GetHistoryItems(bool mockData = false)
        {
            if (mockData)
            {
                history = new HistoryManagerMock(HistoryPath);
            }
            else
            {
                history = new HistoryManagerJSON(HistoryPath);
            }

            List<HistoryItem> historyItems = history.GetHistoryItems();
            historyItems.Reverse();
            return historyItems.ToArray();
        }

        private void ApplyFilter(HistoryFilter filter)
        {
            if (allHistoryItems != null && allHistoryItems.Length > 0)
            {
                IEnumerable<HistoryItem> historyItems = filter.ApplyFilter(allHistoryItems);

                AddHistoryItems(historyItems.ToArray());
            }
        }

        private void ApplyFilterSimple()
        {
            HistoryFilter filter = new HistoryFilter()
            {
                Filename = tstbSearch.Text,
                MaxItemCount = Settings.MaxItemCount
            };

            ApplyFilter(filter);
        }

        private void ApplyFilterAdvanced()
        {
            HistoryFilter filter = new HistoryFilter()
            {
                Filename = txtFilenameFilter.Text,
                URL = txtURLFilter.Text,
                FilterDate = cbDateFilter.Checked,
                FromDate = dtpFilterFrom.Value.Date,
                ToDate = dtpFilterTo.Value.Date,
                FilterType = cbTypeFilter.Checked,
                Type = cbTypeFilterSelection.Text,
                FilterHost = cbHostFilter.Checked,
                Host = cbHostFilterSelection.Text,
                MaxItemCount = Settings.MaxItemCount
            };

            ApplyFilter(filter);
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
                lvi.SubItems.Add(hi.FileName);
                lvi.SubItems.Add(hi.URL);
                lvi.Tag = hi;
            }

            lvHistory.Items.AddRange(listViewItems);
            lvHistory.FillLastColumn();
            lvHistory.Focus();

            if (lvHistory.Items.Count > 0)
            {
                lvHistory.Items[0].Selected = true;
            }

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

                IEnumerable<string> types = historyItems.
                    GroupBy(x => x.Type).
                    OrderByDescending(x => x.Count()).
                    Select(x => string.Format(" - {0}: {1}", x.Key, x.Count()));

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
            HistoryItem previousHistoryItem = him.HistoryItem;
            HistoryItem historyItem = him.UpdateSelectedHistoryItem();

            if (historyItem == null)
            {
                pbThumbnail.Reset();
            }
            else if (historyItem != previousHistoryItem)
            {
                UpdatePictureBox();
            }
        }

        private void UpdatePictureBox()
        {
            pbThumbnail.Reset();

            if (him != null)
            {
                if (him.IsImageFile)
                {
                    pbThumbnail.LoadImageFromFileAsync(him.HistoryItem.FilePath);
                }
                else if (him.IsImageURL)
                {
                    pbThumbnail.LoadImageFromURLAsync(him.HistoryItem.URL);
                }
            }
        }

        private string OutputStats(HistoryItem[] historyItems)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Resources.HistoryItemCounts);
            sb.AppendLine(Resources.HistoryStats_Total + " " + historyItems.Length);

            IEnumerable<string> types = historyItems.
                GroupBy(x => x.Type).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("{0}: {1} ({2:N0}%)", x.Key, x.Count(), x.Count() / (float)historyItems.Length * 100));

            sb.AppendLine(string.Join(Environment.NewLine, types));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_YearlyUsages);

            IEnumerable<string> yearlyUsages = historyItems.
                GroupBy(x => x.DateTime.Year).
                OrderByDescending(x => x.Key).
                Select(x => string.Format("{0}: {1} ({2:N0}%)", x.Key, x.Count(), x.Count() / (float)historyItems.Length * 100));

            sb.AppendLine(string.Join(Environment.NewLine, yearlyUsages));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_FileExtensions);

            IEnumerable<string> fileExtensions = historyItems.
                Where(x => !string.IsNullOrEmpty(x.FileName) && !x.FileName.EndsWith(")")).
                Select(x => Helpers.GetFilenameExtension(x.FileName)).
                GroupBy(x => x).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("{0} ({1})", x.Key, x.Count()));

            sb.AppendLine(string.Join(Environment.NewLine, fileExtensions));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_Hosts);

            IEnumerable<string> hosts = historyItems.
                GroupBy(x => x.Host).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("{0} ({1})", x.Key, x.Count()));

            sb.Append(string.Join(Environment.NewLine, hosts));

            return sb.ToString();
        }

        #region Form events

        private void HistoryForm_Shown(object sender, EventArgs e)
        {
            Refresh();

            RefreshHistoryItems();

            if (lvHistory.Items.Count > 0)
            {
                cbTypeFilterSelection.Items.Clear();
                cbTypeFilterSelection.Items.AddRange(allHistoryItems.Select(x => x.Type).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToArray());
                cbHostFilterSelection.Items.Clear();
                cbHostFilterSelection.Items.AddRange(allHistoryItems.Select(x => x.Host).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToArray());
                ResetFilters();
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
#if DEBUG
                case Keys.Control | Keys.F5:
                    RefreshHistoryItems(true);
                    e.Handled = true;
                    break;
#endif
            }
        }

        private void scMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.SplitterDistance = scMain.SplitterDistance;
        }

        private void tstbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ApplyFilterSimple();
                tstbSearch.Focus();
            }
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            ApplyFilterSimple();
        }

        private void tsbAdvancedSearch_Click(object sender, EventArgs e)
        {
            gbAdvancedSearch.Visible = !gbAdvancedSearch.Visible;
        }

        private void tsbCopyStats_Click(object sender, EventArgs e)
        {
            string stats = OutputStats(allHistoryItems);
            ClipboardHelpers.CopyText(stats);
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            using (HistorySettingsForm form = new HistorySettingsForm(Settings))
            {
                form.ShowDialog();
            }

            RefreshHistoryItems();
        }

        private void txtFilenameFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ApplyFilterAdvanced();
                txtFilenameFilter.Focus();
            }
        }

        private void txtURLFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ApplyFilterAdvanced();
                txtURLFilter.Focus();
            }
        }

        private void btnAdvancedSearch_Click(object sender, EventArgs e)
        {
            gbAdvancedSearch.Visible = false;
            ApplyFilterAdvanced();
        }

        private void btnAdvancedSearchReset_Click(object sender, EventArgs e)
        {
            ResetFilters();
            ApplyFilterAdvanced();
        }

        private void lvHistory_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateControls();
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
                if (File.Exists(hi.FilePath))
                {
                    selection.Add(hi.FilePath);
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