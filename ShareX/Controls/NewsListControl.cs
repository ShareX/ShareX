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

namespace ShareX
{
    public partial class NewsListControl : UserControl
    {
        public List<NewsItem> NewsItems = new List<NewsItem>();

        public NewsListControl()
        {
            InitializeComponent();

            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX released on Windows Store!" });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.8.0 released." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "We now have a Discord server!" });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.7.0 released." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.6.0 released." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.5.0 released.\nMulti line test.\nTest." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.4.0 released." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.3.0 released.\n7 Long text test. 6 Long text test. 5 Long text test. 4 Long text test. 3 Long text test. 2 Long text test. 1 Long text test.\nMulti line test.\nTest." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.2.0 released." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.1.0 released." });
            AddNewsItem(new NewsItem() { DateTimeUTC = DateTime.Now, Text = "ShareX 1.0.0 released." });
        }

        public void AddNewsItem(NewsItem item)
        {
            NewsItems.Add(item);
            Label lblNewsItem = new Label()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
                Font = new Font("Arial", 10),
                Margin = new Padding(0),
                Padding = new Padding(5, 8, 5, 8),
                Text = item.DateTimeUTC.ToLocalTime().ToShortDateString() + " - " + item.Text
            };
            lblNewsItem.BackColor = NewsItems.Count.IsEvenNumber() ? Color.FromArgb(250, 250, 250) : Color.FromArgb(245, 245, 245);
            AddRow(lblNewsItem);
        }

        private void AddRow(Label label)
        {
            RowStyle style = new RowStyle(SizeType.AutoSize);
            tlpMain.RowStyles.Add(style);
            int index = tlpMain.RowCount++ - 1;
            tlpMain.Controls.Add(label, 0, index);
        }
    }
}