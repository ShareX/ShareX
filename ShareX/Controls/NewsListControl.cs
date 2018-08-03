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
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class NewsListControl : UserControl
    {
        public NewsManager NewsManager { get; private set; }

        public event EventHandler NewsLoaded;

        private ToolTip tooltip;

        public NewsListControl()
        {
            InitializeComponent();

            tooltip = new ToolTip()
            {
                AutoPopDelay = 10000,
                InitialDelay = 500
            };

            tlpMain.CellPaint += TlpMain_CellPaint;
            tlpMain.Layout += TlpMain_Layout;
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
                    tlpMain.SuspendLayout();

                    foreach (NewsItem item in NewsManager.NewsItems)
                    {
                        if (item != null)
                        {
                            AddNewsItem(item);
                        }
                    }

                    tlpMain.ResumeLayout();

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
        }

        private async void TlpMain_Layout(object sender, LayoutEventArgs e)
        {
            await Task.Delay(1);

            if (tlpMain.HorizontalScroll.Visible)
            {
                tlpMain.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
            }
            else
            {
                tlpMain.Padding = new Padding(0);
            }
        }

        private void TlpMain_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            Color color;

            if (e.Row.IsEvenNumber())
            {
                color = SystemColors.Window;
            }
            else
            {
                color = ColorHelpers.DarkerColor(SystemColors.Window, 0.02f);
            }

            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            if (NewsManager != null && NewsManager.NewsItems != null && NewsManager.NewsItems.IsValidIndex(e.Row) && NewsManager.NewsItems[e.Row].IsUnread && e.Column == 0)
            {
                e.Graphics.FillRectangle(Brushes.LimeGreen, new Rectangle(e.CellBounds.X, e.CellBounds.Y, 5, e.CellBounds.Height));
            }

            using (Pen pen = new Pen(ProfessionalColors.SeparatorDark))
            {
                e.Graphics.DrawLine(pen, new Point(e.CellBounds.X, e.CellBounds.Bottom - 1), new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1));
            }
        }

        public void AddNewsItem(NewsItem item)
        {
            RowStyle style = new RowStyle(SizeType.AutoSize);
            tlpMain.RowStyles.Add(style);
            int index = tlpMain.RowCount++ - 1;

            Label lblDateTime = new Label()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 10),
                Margin = new Padding(0),
                Padding = new Padding(10, 8, 5, 8),
                Text = item.DateTime.ToShortDateString()
            };

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

            tooltip.SetToolTip(lblDateTime, dateTimeTooltip);

            tlpMain.Controls.Add(lblDateTime, 0, index);

            Label lblText = new Label()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 10),
                Margin = new Padding(0),
                Padding = new Padding(5, 8, 5, 8),
                Text = item.Text
            };

            if (URLHelpers.IsValidURL(item.URL))
            {
                tooltip.SetToolTip(lblText, item.URL);
                lblText.Cursor = Cursors.Hand;
                lblText.MouseEnter += (sender, e) => lblText.ForeColor = SystemColors.HotTrack;
                lblText.MouseLeave += (sender, e) => lblText.ForeColor = SystemColors.ControlText;
                lblText.MouseClick += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        URLHelpers.OpenURL(item.URL);
                    }
                };
            }

            tlpMain.Controls.Add(lblText, 1, index);
        }
    }
}