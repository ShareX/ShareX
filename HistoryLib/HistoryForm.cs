#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HistoryLib
{
    public partial class HistoryForm : Form
    {
        public string HistoryPath { get; private set; }
        public int MaxItemCount { get; set; }

        private HistoryManager history;
        private HistoryItemManager him;
        private HistoryItem[] allHistoryItems;

        public HistoryForm(string historyPath, int maxItemCount = -1)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            HistoryPath = historyPath;
            MaxItemCount = maxItemCount;

            him = new HistoryItemManager();
            him.GetHistoryItems += him_GetHistoryItems;

            pbThumbnail.Reset();
            cbFilenameFilterMethod.SelectedIndex = 0; // Contains
            cbFilenameFilterCulture.SelectedIndex = 1; // Invariant culture
            cbTypeFilterSelection.SelectedIndex = 0; // Image
            cbFilenameFilterCulture.Items[0] = string.Format("Current culture ({0})", CultureInfo.CurrentCulture.Parent.EnglishName);
            lvHistory.FillLastColumn();
        }

        private void RefreshHistoryItems()
        {
            if (history == null)
            {
                history = new HistoryManager(HistoryPath);
            }

            allHistoryItems = GetHistoryItems();
            ApplyFiltersAndAdd();
        }

        private HistoryItem[] him_GetHistoryItems()
        {
            return lvHistory.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as HistoryItem).ToArray();
        }

        private HistoryItem[] GetHistoryItems()
        {
            IEnumerable<HistoryItem> tempHistoryItems = history.GetHistoryItems();

            if (MaxItemCount > -1)
            {
                int skip = tempHistoryItems.Count() - MaxItemCount;

                if (skip > 0)
                {
                    tempHistoryItems = tempHistoryItems.Skip(skip);
                }
            }

            return tempHistoryItems.OrderByDescending(x => x.DateTimeUtc).ToArray();
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
            IEnumerable<HistoryItem> result = historyItems.AsEnumerable();

            if (cbTypeFilter.Checked)
            {
                string type = cbTypeFilterSelection.Text;

                result = result.Where(x => x.Type == type);
            }

            if (cbHostFilter.Checked)
            {
                string host = txtHostFilter.Text;

                result = result.Where(x => x.Host.IndexOf(host, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }

            string filenameFilter = txtFilenameFilter.Text;
            if (cbFilenameFilter.Checked && !string.IsNullOrEmpty(filenameFilter))
            {
                StringComparison rule = GetStringRule();

                if (cbFilenameFilterMethod.SelectedIndex == 0) // Contains
                {
                    result = result.Where(x => x.Filename.IndexOf(filenameFilter, rule) >= 0);
                }
                else if (cbFilenameFilterMethod.SelectedIndex == 1) // Starts with
                {
                    result = result.Where(x => x.Filename.StartsWith(filenameFilter, rule));
                }
                else if (cbFilenameFilterMethod.SelectedIndex == 2) // Exact match
                {
                    result = result.Where(x => x.Filename.Equals(filenameFilter, rule));
                }
            }

            if (cbDateFilter.Checked)
            {
                DateTime fromDate = dtpFilterFrom.Value.Date;
                DateTime toDate = dtpFilterTo.Value.Date;

                result = from hi in result
                         let date = FastDateTime.ToLocalTime(hi.DateTimeUtc).Date
                         where date >= fromDate && date <= toDate
                         select hi;
            }

            return result.ToArray();
        }

        private StringComparison GetStringRule()
        {
            bool caseSensitive = cbFilenameFilterCase.Checked;

            switch (cbFilenameFilterCulture.SelectedIndex)
            {
                case 0:
                    return caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                case 1:
                    return caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
                case 3:
                    return caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            }

            return StringComparison.InvariantCultureIgnoreCase;
        }

        private void AddHistoryItems(HistoryItem[] historyItems)
        {
            UpdateItemCount(historyItems);

            lvHistory.Items.Clear();

            ListViewItem[] listViewItems = new ListViewItem[historyItems.Length];

            for (int i = 0; i < historyItems.Length; i++)
            {
                HistoryItem hi = historyItems[i];
                ListViewItem lvi = listViewItems[i] = new ListViewItem(hi.DateTimeUtc.ToLocalTime().ToString());
                lvi.SubItems.Add(hi.Filename);
                lvi.SubItems.Add(hi.Type);
                lvi.SubItems.Add(hi.Host);
                lvi.SubItems.Add(hi.URL);
                lvi.Tag = hi;
            }

            lvHistory.Items.AddRange(listViewItems);
            lvHistory.FillLastColumn();
            lvHistory.Focus();
        }

        private void UpdateItemCount(HistoryItem[] historyItems)
        {
            StringBuilder status = new StringBuilder();

            status.Append("Total: " + allHistoryItems.Length);

            if (allHistoryItems.Length > historyItems.Length)
            {
                status.Append(", Filtered: " + historyItems.Length);
            }

            var types = from hi in historyItems
                        group hi by hi.Type
                            into t
                            let count = t.Count()
                            orderby t.Key
                            select string.Format(", {0}: {1}", t.Key, count);

            foreach (string type in types)
            {
                status.Append(type);
            }

            tsslStatus.Text = status.ToString();
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
            Application.DoEvents();
            BringToFront();
            Activate();
            RefreshHistoryItems();
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

        private void lvHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.lvHistory.ListViewItemSorter = new ListViewItemComparer(e.Column);
            lvHistory.Sort();
        }

        #endregion Form events

    }

    class ListViewItemComparer : IComparer
    {
        private int col;
        public ListViewItemComparer()
        {
            col = 0;
        }
        public ListViewItemComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        }
    }
}