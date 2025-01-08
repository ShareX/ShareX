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
using ShareX.HistoryLib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public partial class HistoryForm : Form
    {
        public string HistoryPath { get; private set; }
        public HistorySettings Settings { get; private set; }

        private HistoryItemManager him;
        private HistoryItem[] allHistoryItems;
        private HistoryItem[] filteredHistoryItems;
        private string defaultTitle;
        private Dictionary<string, string> typeNamesLocaleLookup;
        private string[] allTypeNames;
        private ListViewItem[] listViewCache;
        private int listViewCacheStartIndex;

        public HistoryForm(string historyPath, HistorySettings settings, Action<string> uploadFile = null, Action<string> editImage = null, Action<string> pinToScreen = null)
        {
            HistoryPath = historyPath;
            Settings = settings;

            InitializeComponent();
            tsHistory.Renderer = new ToolStripRoundedEdgeRenderer();

            defaultTitle = Text;

            string[] typeNames = Enum.GetNames(typeof(EDataType));
            string[] typeTranslations = Helpers.GetLocalizedEnumDescriptions<EDataType>();
            typeNamesLocaleLookup = typeNames.Zip(typeTranslations, (key, val) => new { key, val }).ToDictionary(e => e.key, e => e.val);

            UpdateTitle();

            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add(Resources.image);
            il.Images.Add(Resources.notebook);
            il.Images.Add(Resources.application_block);
            il.Images.Add(Resources.globe);
            lvHistory.SmallImageList = il;

            him = new HistoryItemManager(uploadFile, editImage, pinToScreen, true);
            him.GetHistoryItems += him_GetHistoryItems;
            lvHistory.ContextMenuStrip = him.cmsHistory;

            pbThumbnail.Reset();
            lvHistory.FillLastColumn();
            scHistoryItemInfo.SplitterWidth = 7; // Because of bug must be assigned here again
            scHistoryItemInfo.Panel2Collapsed = true;

            tstbSearch.TextBox.HandleCreated += (sender, e) => tstbSearch.TextBox.SetWatermark(Resources.HistoryForm_Search_Watermark, true);

            if (Settings.RememberSearchText)
            {
                tstbSearch.Text = Settings.SearchText;
            }

            ShareXResources.ApplyTheme(this, true);

            if (Settings.RememberWindowState)
            {
                Settings.WindowState.ApplyFormState(this);

                if (Settings.SplitterDistance > 0)
                {
                    scMain.SplitterDistance = Settings.SplitterDistance;
                }
            }
        }

        private void ResetFilters()
        {
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

        private async Task RefreshHistoryItems(bool mockData = false)
        {
            allHistoryItems = await GetHistoryItems(mockData);

            cbTypeFilterSelection.Items.Clear();
            cbHostFilterSelection.Items.Clear();
            tstbSearch.AutoCompleteCustomSource.Clear();

            if (allHistoryItems.Length > 0)
            {
                allTypeNames = allHistoryItems.Select(x => x.Type).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
                cbTypeFilterSelection.Items.AddRange(allTypeNames.Select(x => typeNamesLocaleLookup.TryGetValue(x, out string value) ? value : x).ToArray());
                cbHostFilterSelection.Items.AddRange(allHistoryItems.Select(x => x.Host).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray());
                tstbSearch.AutoCompleteCustomSource.AddRange(allHistoryItems.Select(x => x.TagsProcessName).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray());
            }

            ApplyFilterSimple();

            ResetFilters();
        }

        private HistoryItem[] him_GetHistoryItems()
        {
            return lvHistory.SelectedIndices.Cast<int>().Select(i => filteredHistoryItems[i]).ToArray();
        }

        private async Task<HistoryItem[]> GetHistoryItems(bool mockData = false)
        {
            HistoryManager history;

            if (mockData)
            {
                history = new HistoryManagerMock(HistoryPath);
            }
            else
            {
                history = new HistoryManagerJSON(HistoryPath);
            }

            List<HistoryItem> historyItems = await history.GetHistoryItemsAsync();
            historyItems.Reverse();
            return historyItems.ToArray();
        }

        private void ApplyFilter(HistoryFilter filter)
        {
            if (allHistoryItems != null && allHistoryItems.Length > 0)
            {
                IEnumerable<HistoryItem> historyItems = filter.ApplyFilter(allHistoryItems);
                filteredHistoryItems = historyItems.ToArray();

                UpdateTitle(filteredHistoryItems);

                listViewCache = null;
                listViewCacheStartIndex = 0;
                lvHistory.VirtualListSize = 0;

                if (filteredHistoryItems.Length > 0)
                {
                    lvHistory.VirtualListSize = filteredHistoryItems.Length;
                    lvHistory.SelectedIndices.Add(0);
                }
            }
        }

        private void ApplyFilterSimple()
        {
            string searchText = tstbSearch.Text;

            if (Settings.RememberSearchText)
            {
                Settings.SearchText = searchText;
            }
            else
            {
                Settings.SearchText = "";
            }

            HistoryFilter filter = new HistoryFilter()
            {
                Filename = searchText
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
                FilterHost = cbHostFilter.Checked,
                Host = cbHostFilterSelection.Text
            };

            if (cbTypeFilter.Checked && allTypeNames.IsValidIndex(cbTypeFilterSelection.SelectedIndex))
            {
                filter.FilterType = true;
                filter.Type = allTypeNames[cbTypeFilterSelection.SelectedIndex];
            }

            ApplyFilter(filter);
        }

        private ListViewItem CreateListViewItem(int index)
        {
            HistoryItem hi = filteredHistoryItems[index];

            ListViewItem lvi = new ListViewItem();

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

            lvi.SubItems.Add(hi.DateTime.ToString());
            lvi.SubItems.Add(hi.FileName);
            lvi.SubItems.Add(hi.URL);

            return lvi;
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
                    Select(x => string.Format(" - {0}: {1}", typeNamesLocaleLookup.TryGetValue(x.Key, out string value) ? value : x.Key, x.Count()));

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

            pgHistoryItemInfo.SelectedObject = historyItem;
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
            string empty = "(empty)";

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
                Select(x => FileHelpers.GetFileNameExtension(x.FileName)).
                GroupBy(x => string.IsNullOrWhiteSpace(x) ? empty : x).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("[{0}] {1}", x.Count(), x.Key));

            sb.AppendLine(string.Join(Environment.NewLine, fileExtensions));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_Hosts);

            IEnumerable<string> hosts = historyItems.
                GroupBy(x => string.IsNullOrWhiteSpace(x.Host) ? empty : x.Host).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("[{0}] {1}", x.Count(), x.Key));

            sb.AppendLine(string.Join(Environment.NewLine, hosts));

            sb.AppendLine();
            sb.AppendLine(Resources.ProcessNames);

            IEnumerable<string> processNames = historyItems.
                GroupBy(x => string.IsNullOrWhiteSpace(x.TagsProcessName) ? empty : x.TagsProcessName).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("[{0}] {1}", x.Count(), x.Key));

            sb.Append(string.Join(Environment.NewLine, processNames));

            return sb.ToString();
        }

        #region Form events

        private async void HistoryForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();

            await RefreshHistoryItems();
        }

        private void HistoryForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void HistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.RememberWindowState)
            {
                Settings.WindowState.UpdateFormState(this);
                Settings.SplitterDistance = scMain.SplitterDistance;
            }
        }

        private async void HistoryForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    e.SuppressKeyPress = true;
                    await RefreshHistoryItems();
                    break;
                case Keys.Control | Keys.F5 when HelpersOptions.DevMode:
                    e.SuppressKeyPress = true;
                    await RefreshHistoryItems(true);
                    break;
            }
        }

        private void tstbSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilterSimple();
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            ApplyFilterSimple();
        }

        private void tsbAdvancedSearch_Click(object sender, EventArgs e)
        {
            bool isPanelVisible = gbAdvancedSearch.Visible;
            gbAdvancedSearch.Visible = !isPanelVisible;
            tsbAdvancedSearch.Checked = !isPanelVisible;
        }

        private void tsbToggleMoreInfo_Click(object sender, EventArgs e)
        {
            bool isPanelVisible = !scHistoryItemInfo.Panel2Collapsed;
            scHistoryItemInfo.Panel2Collapsed = isPanelVisible;
            tsbToggleMoreInfo.Checked = !isPanelVisible;
        }

        private void tsbShowStats_Click(object sender, EventArgs e)
        {
            string stats = OutputStats(allHistoryItems);
            OutputBox.Show(stats, Resources.HistoryStats);
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            using (HistorySettingsForm form = new HistorySettingsForm(Settings))
            {
                form.ShowDialog();
            }
        }

        private void AdvancedFilter_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilterAdvanced();
        }

        private void btnAdvancedSearchReset_Click(object sender, EventArgs e)
        {
            ResetFilters();
            ApplyFilterAdvanced();
        }

        private void btnAdvancedSearchClose_Click(object sender, EventArgs e)
        {
            gbAdvancedSearch.Visible = false;
            tsbAdvancedSearch.Checked = false;
        }

        private void lvHistory_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (listViewCache != null && e.ItemIndex >= listViewCacheStartIndex && e.ItemIndex < listViewCacheStartIndex + listViewCache.Length)
            {
                e.Item = listViewCache[e.ItemIndex - listViewCacheStartIndex];
            }
            else
            {
                e.Item = CreateListViewItem(e.ItemIndex);
            }
        }

        private void lvHistory_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (listViewCache != null && e.StartIndex >= listViewCacheStartIndex && e.EndIndex <= listViewCacheStartIndex + listViewCache.Length)
            {
                return;
            }

            listViewCacheStartIndex = e.StartIndex;
            int length = e.EndIndex - e.StartIndex + 1;
            listViewCache = new ListViewItem[length];

            for (int i = 0; i < length; i++)
            {
                listViewCache[i] = CreateListViewItem(e.StartIndex + i);
            }
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
            e.SuppressKeyPress = him.HandleKeyInput(e);
        }

        private void lvHistory_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<string> selection = new List<string>();

            foreach (int index in lvHistory.SelectedIndices)
            {
                HistoryItem hi = filteredHistoryItems[index];

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

        private void pbThumbnail_MouseDown(object sender, MouseEventArgs e)
        {
            int currentImageIndex = lvHistory.SelectedIndex;

            if (currentImageIndex > -1 && pbThumbnail.Image != null && filteredHistoryItems != null && filteredHistoryItems.Length > 0)
            {
                pbThumbnail.Enabled = false;

                int modifiedImageIndex = 0;
                int halfRange = 100;
                int startIndex = Math.Max(currentImageIndex - halfRange, 0);
                int endIndex = Math.Min(startIndex + (halfRange * 2) + 1, filteredHistoryItems.Length);

                List<string> filteredImages = new List<string>();

                for (int i = startIndex; i < endIndex; i++)
                {
                    string imageFilePath = filteredHistoryItems[i].FilePath;

                    if (i == currentImageIndex)
                    {
                        modifiedImageIndex = filteredImages.Count;
                    }

                    filteredImages.Add(imageFilePath);
                }

                ImageViewer.ShowImage(filteredImages.ToArray(), modifiedImageIndex);

                pbThumbnail.Enabled = true;
            }
        }

        #endregion Form events
    }
}