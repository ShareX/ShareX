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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShareX.HelpersLib;
using System.Drawing.Drawing2D;

namespace ShareX
{
    public partial class NewsListControl : UserControl
    {
        private NewsManager newsManager;
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

            newsManager = new NewsManager();
            newsManager.UpdateNews();

            foreach(NewsItem item in newsManager.NewsItems)
            {
                AddNewsItem(item);
            }
        }

        private void TlpMain_Layout(object sender, LayoutEventArgs e)
        {
            TaskEx.RunDelayed(() =>
            {
                if (tlpMain.HorizontalScroll.Visible)
                {
                    tlpMain.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
                }
                else
                {
                    tlpMain.Padding = new Padding(0);
                }
            }, 1);
        }

        private void TlpMain_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            Color color;

            if (e.Row.IsEvenNumber())
            {
                color = Color.FromArgb(250, 250, 250);
            }
            else
            {
                color = Color.FromArgb(247, 247, 247);
            }

            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            if (newsManager.NewsItems.IsValidIndex(e.Row) && newsManager.NewsItems[e.Row].IsUnread && e.Column == 0)
            {
                e.Graphics.FillRectangle(Brushes.LimeGreen, new Rectangle(e.CellBounds.X, e.CellBounds.Y, 5, e.CellBounds.Height));
            }

            e.Graphics.DrawLine(Pens.LightGray, new Point(e.CellBounds.X, e.CellBounds.Bottom - 1), new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1));
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
                Text = item.DateTime.ToLocalTime().ToShortDateString()
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
                lblText.MouseEnter += (sender, e) => lblText.ForeColor = Color.Blue;
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