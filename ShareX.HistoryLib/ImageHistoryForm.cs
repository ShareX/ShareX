#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

using Manina.Windows.Forms;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using View = Manina.Windows.Forms.View;

namespace ShareX.HistoryLib
{
    public partial class ImageHistoryForm : Form
    {
        public string HistoryPath { get; private set; }
        public int MaxItemCount { get; set; }
        public int ViewMode { get; set; }
        public Size ThumbnailSize { get; set; }

        private HistoryManager history;
        private HistoryItemManager him;
        private HistoryItem[] historyItems;

        public ImageHistoryForm(string historyPath, int viewMode, Size thumbnailSize, int maxItemCount)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            HistoryPath = historyPath;

            ViewMode = viewMode.Between(0, 3);
            ilvImages.View = (View)ViewMode;

            switch (ilvImages.View)
            {
                default:
                case View.Thumbnails:
                    tsmiViewModeThumbnails.RadioCheck();
                    break;
                case View.Gallery:
                    tsmiViewModeGallery.RadioCheck();
                    break;
                case View.Pane:
                    tsmiViewModePane.RadioCheck();
                    break;
            }

            ThumbnailSize = thumbnailSize;
            ilvImages.ThumbnailSize = ThumbnailSize;

            switch (ThumbnailSize.Width)
            {
                case 75:
                    tsmiThumbnailSize75.RadioCheck();
                    break;
                default:
                case 100:
                    tsmiThumbnailSize100.RadioCheck();
                    break;
                case 150:
                    tsmiThumbnailSize150.RadioCheck();
                    break;
                case 200:
                    tsmiThumbnailSize200.RadioCheck();
                    break;
                case 250:
                    tsmiThumbnailSize250.RadioCheck();
                    break;
            }

            MaxItemCount = maxItemCount;

            if (MaxItemCount <= 0)
            {
                tsmiMaxImageLimit0.RadioCheck();
            }
            else if (MaxItemCount <= 100)
            {
                tsmiMaxImageLimit100.RadioCheck();
            }
            else if (MaxItemCount <= 250)
            {
                tsmiMaxImageLimit250.RadioCheck();
            }
            else
            {
                tsmiMaxImageLimit1000.RadioCheck();
            }

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
            List<HistoryItem> result = new List<HistoryItem>();

            List<HistoryItem> allHistoryItems = history.GetHistoryItems();

            int itemCount = 0;

            for (int i = allHistoryItems.Count - 1; i >= 0; i--)
            {
                HistoryItem hi = allHistoryItems[i];

                if (!string.IsNullOrEmpty(hi.Filepath) && Helpers.IsImageFile(hi.Filepath) && File.Exists(hi.Filepath))
                {
                    result.Add(hi);

                    itemCount++;

                    if (MaxItemCount > 0 && itemCount >= MaxItemCount)
                    {
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        private HistoryItem[] him_GetHistoryItems()
        {
            return ilvImages.SelectedItems.Select(x => x.Tag as HistoryItem).ToArray();
        }

        #region Form events

        private void ImageHistoryForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            this.ForceActivate();
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
            ViewMode = (int)ilvImages.View;
        }

        private void tsmiViewModeGallery_Click(object sender, EventArgs e)
        {
            tsmiViewModeGallery.RadioCheck();
            ilvImages.View = View.Gallery;
            ViewMode = (int)ilvImages.View;
        }

        private void tsmiViewModePane_Click(object sender, EventArgs e)
        {
            tsmiViewModePane.RadioCheck();
            ilvImages.View = View.Pane;
            ViewMode = (int)ilvImages.View;
        }

        private void tsmiThumbnailSize75_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize75.RadioCheck();
            ilvImages.ThumbnailSize = new Size(75, 75);
            ThumbnailSize = ilvImages.ThumbnailSize;
        }

        private void tsmiThumbnailSize100_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize100.RadioCheck();
            ilvImages.ThumbnailSize = new Size(100, 100);
            ThumbnailSize = ilvImages.ThumbnailSize;
        }

        private void tsmiThumbnailSize150_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize150.RadioCheck();
            ilvImages.ThumbnailSize = new Size(150, 150);
            ThumbnailSize = ilvImages.ThumbnailSize;
        }

        private void tsmiThumbnailSize200_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize200.RadioCheck();
            ilvImages.ThumbnailSize = new Size(200, 200);
            ThumbnailSize = ilvImages.ThumbnailSize;
        }

        private void tsmiThumbnailSize250_Click(object sender, EventArgs e)
        {
            tsmiThumbnailSize250.RadioCheck();
            ilvImages.ThumbnailSize = new Size(250, 250);
            ThumbnailSize = ilvImages.ThumbnailSize;
        }

        private void tsmiMaxImageLimit100_Click(object sender, EventArgs e)
        {
            tsmiMaxImageLimit100.RadioCheck();
            MaxItemCount = 100;
            RefreshHistoryItems();
        }

        private void tsmiMaxImageLimit250_Click(object sender, EventArgs e)
        {
            tsmiMaxImageLimit250.RadioCheck();
            MaxItemCount = 250;
            RefreshHistoryItems();
        }

        private void tsmiMaxImageLimit1000_Click(object sender, EventArgs e)
        {
            tsmiMaxImageLimit1000.RadioCheck();
            MaxItemCount = 1000;
            RefreshHistoryItems();
        }

        private void tsmiMaxImageLimit0_Click(object sender, EventArgs e)
        {
            tsmiMaxImageLimit0.RadioCheck();
            MaxItemCount = 0;
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