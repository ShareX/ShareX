#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class NewsListControl : UserControl
    {
        public event EventHandler NewsLoaded;

        public NewsManager NewsManager { get; private set; }

        public NewsListControl()
        {
            InitializeComponent();
            dgvNews.AlternatingRowsDefaultCellStyle.BackColor = dgvNews.AlternatingRowsDefaultCellStyle.SelectionBackColor =
                ColorHelpers.DarkerColor(SystemColors.Window, 0.02f);
            dgvNews.GridColor = ProfessionalColors.SeparatorDark;
            dgvNews.DoubleBuffered(true);
        }

        public void Start()
        {
            Task.Run(() =>
            {
                NewsManager = new NewsManager();
                NewsManager.LastReadDate = Program.Settings.NewsLastReadDate;
                NewsManager.UpdateNews();
                NewsManager.UpdateUnread();
            }).ContinueInCurrentContext(() =>
            {
                if (NewsManager != null && NewsManager.NewsItems != null)
                {
                    SuspendLayout();

                    foreach (NewsItem item in NewsManager.NewsItems)
                    {
                        if (item != null)
                        {
                            AddNewsItem(item);
                        }
                    }

                    UpdateUnreadStatus();

                    ResumeLayout();

                    OnNewsLoaded();
                }
            });
        }

        protected void OnNewsLoaded()
        {
            if (NewsLoaded != null)
            {
                NewsLoaded(this, EventArgs.Empty);
            }
        }

        public void MarkRead()
        {
            if (NewsManager != null && NewsManager.NewsItems != null && NewsManager.NewsItems.Count > 0)
            {
                DateTime latestDate = NewsManager.NewsItems.OrderByDescending(x => x.DateTime).First().DateTime;
                DateTime futureDate = DateTime.Now.AddMonths(1);

                if (latestDate < futureDate)
                {
                    Program.Settings.NewsLastReadDate = NewsManager.LastReadDate = latestDate;
                    NewsManager.UpdateUnread();
                }
            }

            UpdateUnreadStatus();
        }

        public void AddNewsItem(NewsItem item)
        {
            int index = dgvNews.Rows.Add();
            DataGridViewRow row = dgvNews.Rows[index];
            row.Tag = item;

            row.Cells[1].Value = item.DateTime.ToShortDateString();

            string dateTimeTooltip;
            double days = (DateTime.Now - item.DateTime).TotalDays;

            if (days < 1)
            {
                dateTimeTooltip = "Today.";
            }
            else if (days < 2)
            {
                dateTimeTooltip = "Yesterday.";
            }
            else
            {
                dateTimeTooltip = (int)days + " days ago.";
            }

            row.Cells[1].ToolTipText = dateTimeTooltip;

            row.Cells[2].Value = item.Text;

            if (URLHelpers.IsValidURL(item.URL))
            {
                row.Cells[2].ToolTipText = item.URL;
            }
        }

        private void UpdateUnreadStatus()
        {
            foreach (DataGridViewRow row in dgvNews.Rows)
            {
                NewsItem newsItem = row.Tag as NewsItem;
                if (newsItem != null && newsItem.IsUnread)
                {
                    row.Cells[0].Style.BackColor = row.Cells[0].Style.SelectionBackColor = Color.LimeGreen;
                }
                else
                {
                    row.Cells[0].Style = null;
                }
            }
        }

        private void dgvNews_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                DataGridViewRow row = dgvNews.Rows[e.RowIndex];
                NewsItem newsItem = row.Tag as NewsItem;
                if (newsItem != null && !string.IsNullOrEmpty(newsItem.URL))
                {
                    dgvNews.Cursor = Cursors.Hand;
                    row.Cells[e.ColumnIndex].Style.ForeColor = row.Cells[e.ColumnIndex].Style.SelectionForeColor = SystemColors.HotTrack;
                }
            }
        }

        private void dgvNews_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                DataGridViewRow row = dgvNews.Rows[e.RowIndex];
                NewsItem newsItem = row.Tag as NewsItem;
                if (newsItem != null && !string.IsNullOrEmpty(newsItem.URL))
                {
                    row.Cells[e.ColumnIndex].Style.ForeColor = row.Cells[e.ColumnIndex].Style.SelectionForeColor = SystemColors.ControlText;
                }
            }

            dgvNews.Cursor = Cursors.Default;
        }

        private void dgvNews_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.ColumnIndex == 2)
            {
                DataGridViewRow row = dgvNews.Rows[e.RowIndex];
                NewsItem newsItem = row.Tag as NewsItem;
                if (newsItem != null && URLHelpers.IsValidURL(newsItem.URL))
                {
                    URLHelpers.OpenURL(newsItem.URL);
                }
            }
        }
    }
}