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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib
{
    public partial class TabToListView : UserControl
    {
        private TabControl mainTabControl;

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
                lvMain.SmallImageList = value;
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
                        }
                    }
                }

                if (lvMain.Items.Count > 0)
                {
                    lvMain.Items[0].Selected = true;
                }
            }
        }

        private void lvMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMain.SelectedItems != null && lvMain.SelectedItems.Count > 0)
            {
                TabPage tabPage = lvMain.SelectedItems[0].Tag as TabPage;

                if (tabPage != null)
                {
                    tcMain.Visible = true;
                    tcMain.TabPages.Clear();
                    tcMain.TabPages.Add(tabPage);
                }
            }
        }

        public void FocusListView()
        {
            lvMain.Focus();
        }
    }
}