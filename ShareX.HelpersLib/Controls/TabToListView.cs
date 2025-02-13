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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class TabToListView : UserControl
    {
        private TabControl mainTabControl;

        [Browsable(false)]
        public TabControl MainTabControl
        {
            get
            {
                return mainTabControl;
            }
            set
            {
                mainTabControl = value;
                FillListView(mainTabControl);
            }
        }

        private int listViewSize;

        public int ListViewSize
        {
            get
            {
                return listViewSize;
            }
            set
            {
                listViewSize = value;
                scMain.SplitterDistance = listViewSize;
            }
        }

        public ImageList ImageList
        {
            get
            {
                return lvMain.SmallImageList;
            }
            set
            {
                lvMain.SmallImageList = tcMain.ImageList = value;
            }
        }

        public TabToListView()
        {
            InitializeComponent();
            ListViewSize = 150;
        }

        private void FillListView(TabControl tab)
        {
            if (tab != null)
            {
                foreach (TabPage tabPage in tab.TabPages)
                {
                    ListViewGroup lvg = new ListViewGroup(tabPage.Text);
                    lvMain.Groups.Add(lvg);

                    foreach (Control control in tabPage.Controls)
                    {
                        if (control is TabControl)
                        {
                            TabControl tab2 = control as TabControl;

                            foreach (TabPage tabPage2 in tab2.TabPages)
                            {
                                ListViewItem lvi = new ListViewItem(tabPage2.Text);
                                lvi.ImageKey = tabPage2.ImageKey;
                                lvi.Tag = tabPage2;
                                lvi.Group = lvg;
                                lvMain.Items.Add(lvi);
                            }

                            tab2.TabPages.Clear();
                        }
                    }
                }

                if (lvMain.Items.Count > 0)
                {
                    lvMain.Items[0].Selected = true;
                }
            }
        }

        private void lvMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (lvMain.SelectedItems.Count == 0 && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            {
                ListViewItem lvi = lvMain.GetItemAt(e.X, e.Y);

                if (lvi == null)
                {
                    // Workaround for 1px space between items
                    lvi = lvMain.GetItemAt(e.X, e.Y - 1);
                }

                if (lvi != null)
                {
                    lvi.Selected = true;
                }
            }
        }

        private void lvMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMain.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvMain.SelectedItems[0];
                TabPage tabPage = lvi.Tag as TabPage;
                LoadTabPage(tabPage);
            }
        }

        private void LoadTabPage(TabPage tabPage)
        {
            if (tabPage != null && !tcMain.TabPages.Contains(tabPage))
            {
                tcMain.TabPages.Clear();
                tcMain.TabPages.Add(tabPage);
                // Need to set ImageKey again otherwise icon not show up
                tabPage.ImageKey = tabPage.ImageKey;
                tabPage.Refresh();
                lvMain.Focus();
            }
        }

        public void NavigateToTabPage(TabPage tabPage)
        {
            if (tabPage != null)
            {
                foreach (ListViewItem lvi in lvMain.Items)
                {
                    TabPage currentTabPage = lvi.Tag as TabPage;

                    if (currentTabPage == tabPage)
                    {
                        lvi.Selected = true;
                        return;
                    }
                }
            }
        }

        public void FocusListView()
        {
            lvMain.Focus();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            scMain.SplitterDistance = (int)Math.Round(scMain.SplitterDistance * factor.Width);
        }
    }
}