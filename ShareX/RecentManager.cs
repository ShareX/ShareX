#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public class RecentManager
    {
        private int maxCount = 10;

        public int MaxCount
        {
            get
            {
                return maxCount;
            }
            set
            {
                maxCount = value.Between(1, 100);

                lock (itemsLock)
                {
                    while (Items.Count > maxCount)
                    {
                        Items.Dequeue();
                    }

                    UpdateRecentMenu();
                }
            }
        }

        public Queue<RecentItem> Items { get; private set; }

        private static readonly object itemsLock = new object();

        public RecentManager()
        {
            Items = new Queue<RecentItem>();
        }

        public void Add(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                lock (itemsLock)
                {
                    while (Items.Count >= MaxCount)
                    {
                        Items.Dequeue();
                    }

                    Items.Enqueue(new RecentItem(item));

                    UpdateRecentMenu();
                }
            }
        }

        public void UpdateItems(RecentItem[] items)
        {
            if (items != null)
            {
                lock (itemsLock)
                {
                    Items = new Queue<RecentItem>(items);

                    UpdateRecentMenu();
                }
            }
        }

        private void UpdateRecentMenu()
        {
            if (Program.MainForm == null || Program.MainForm.tsmiTrayRecentItems == null || Items.Count == 0)
            {
                return;
            }

            ToolStripMenuItem tsmi = Program.MainForm.tsmiTrayRecentItems;

            if (!tsmi.Visible)
            {
                tsmi.Visible = true;
            }

            tsmi.DropDownItems.Clear();
            ToolStripMenuItem tsmiTip = new ToolStripMenuItem("Left click to copy URL to clipboard. Right click to open URL.");
            tsmiTip.Enabled = false;
            tsmi.DropDownItems.Add(tsmiTip);
            tsmi.DropDownItems.Add(new ToolStripSeparator());

            foreach (RecentItem recentItem in Items.Reverse())
            {
                string text = string.Format("[{0:HH:mm:ss}] {1}", recentItem.Time, recentItem.Text.Truncate(50, "...", false));
                ToolStripMenuItem tsmiLink = new ToolStripMenuItem(text);
                tsmiLink.ToolTipText = recentItem.Text;
                string link = recentItem.Text;
                tsmiLink.MouseUp += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        ClipboardHelpers.CopyText(link);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        URLHelpers.OpenURL(link);
                    }
                };
                tsmi.DropDownItems.Add(tsmiLink);
            }
        }
    }

    public class RecentItem
    {
        public string Text { get; set; }
        public DateTime Time { get; set; }

        public RecentItem(string text)
        {
            Text = text;
            Time = DateTime.Now;
        }
    }
}