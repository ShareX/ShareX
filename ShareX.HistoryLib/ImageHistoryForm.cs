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
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using View = Manina.Windows.Forms.View;

namespace HistoryLib
{
    public partial class ImageHistoryForm : Form
    {
        public string HistoryPath { get; private set; }
        public int MaxItemCount { get; set; }

        public int ViewMode
        {
            get
            {
                return (int)ilvImages.View;
            }
            set
            {
                if (value.IsBetween(0, 3))
                {
                    ilvImages.View = (View)value;
                }
            }
        }

        public Size ThumbnailSize
        {
            get
            {
                return ilvImages.ThumbnailSize;
            }
            set
            {
                ilvImages.ThumbnailSize = value;
            }
        }

        private HistoryManager history;
        private HistoryItemManager him;
        private HistoryItem[] historyItems;

        public ImageHistoryForm(string historyPath, int viewMode, Size thumbnailSize, int maxItemCount = -1)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Text = "ShareX - " + string.Format("Image history: {0}", historyPath);

            HistoryPath = historyPath;
            MaxItemCount = maxItemCount;
            ViewMode = viewMode;
            ThumbnailSize = thumbnailSize;

            tsbQuickList.Checked = MaxItemCount > -1;

            him = new HistoryItemManager();
            him.GetHistoryItems += him_GetHistoryItems;
        }

        private void RefreshHistoryItems()
        {
            if (history == null)
            {
                history = new HistoryManager(HistoryPath);
            }

            historyItems = GetHistoryItems();

            ilvImages.Items.Clear();
            ImageListViewItem[] ilvItems = historyItems.Select(historyItem => new ImageListViewItem(historyItem.Filepath) { Tag = historyItem }).ToArray();
            ilvImages.Items.AddRange(ilvItems);
        }

        private HistoryItem[] GetHistoryItems()
        {
            IEnumerable<HistoryItem> tempHistoryItems = history.GetHistoryItems().Where(x => !string.IsNullOrEmpty(x.Filepath) &&
                                                                                             Helpers.IsImageFile(x.Filepath) && File.Exists(x.Filepath));

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

        private HistoryItem[] him_GetHistoryItems()
        {
            return ilvImages.SelectedItems.Select(x => x.Tag as HistoryItem).ToArray();
        }

        #region Form events

        private void ImageHistoryForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            this.ShowActivate();
            RefreshHistoryItems();
        }

        private void ImageHistoryForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    RefreshHistoryItems();
                    e.Handled = true;
                    break;
            }
        }

        private void ilvImages_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                him.cmsHistory.Show(ilvImages, e.X + 1, e.Y + 1);
            }
        }

        private void ilvImages_SelectionChanged(object sender, EventArgs e)
        {
            him.RefreshInfo();
        }

        private void ilvImages_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            him.ShowImagePreview();
        }

        private void tsmiViewModeThumbnails_Click(object sender, EventArgs e)
        {
            tsmiViewModeThumbnails.RadioCheck();
            ilvImages.View = View.Thumbnails;
        }

        private void tsmiViewModeGallery_Click(object sender, EventArgs e)
        {
            tsmiViewModeGallery.RadioCheck();
            ilvImages.View = View.Gallery;
        }

        private void tsmiViewModePane_Click(object sender, EventArgs e)
        {
            tsmiViewModePane.RadioCheck();
            ilvImages.View = View.Pane;
        }

        private void tsmiThumbnailSize75_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize75.RadioCheck();
            ilvImages.ThumbnailSize = new Size(75, 75);
        }

        private void tsmiThumbnailSize100_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize100.RadioCheck();
            ilvImages.ThumbnailSize = new Size(100, 100);
        }

        private void tsmiThumbnailSize150_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize150.RadioCheck();
            ilvImages.ThumbnailSize = new Size(150, 150);
        }

        private void tsmiThumbnailSize200_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize200.RadioCheck();
            ilvImages.ThumbnailSize = new Size(200, 200);
        }

        private void tsmiThumbnailSize250_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize250.RadioCheck();
            ilvImages.ThumbnailSize = new Size(250, 250);
        }

        private void tsbQuickList_Click(object sender, EventArgs e)
        {
            if (tsbQuickList.Checked)
            {
                MaxItemCount = 100;
            }
            else
            {
                MaxItemCount = -1;
            }

            RefreshHistoryItems();
        }

        private void ilvImages_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                default:
                    return;
                case Keys.Enter:
                    him.OpenURL();
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

        #endregion Form events
    }
}