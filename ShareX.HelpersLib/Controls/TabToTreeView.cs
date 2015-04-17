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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class TabToTreeView : UserControl
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
                FillTreeView(tvMain.Nodes, mainTabControl);
                tvMain.ExpandAll();
            }
        }

        private int treeViewSize;

        public int TreeViewSize
        {
            get
            {
                return treeViewSize;
            }
            set
            {
                treeViewSize = value;
                scMain.SplitterDistance = treeViewSize;
            }
        }

        public Font TreeViewFont
        {
            get
            {
                return tvMain.Font;
            }
            set
            {
                tvMain.Font = value;
            }
        }

        public ImageList ImageList
        {
            get
            {
                return tvMain.ImageList;
            }
            set
            {
                tvMain.ImageList = value;
            }
        }

        public TabToTreeView()
        {
            InitializeComponent();
            TreeViewSize = 150;
        }

        private void FillTreeView(TreeNodeCollection nodeCollection, TabControl tab, TreeNode parent = null)
        {
            if (nodeCollection != null && tab != null)
            {
                foreach (TabPage tabPage in tab.TabPages)
                {
                    if (parent != null && string.IsNullOrEmpty(tabPage.Text))
                    {
                        parent.Tag = tabPage;
                        continue;
                    }

                    TreeNode treeNode = new TreeNode(tabPage.Text);
                    if (!string.IsNullOrEmpty(tabPage.ImageKey))
                    {
                        treeNode.ImageKey = treeNode.SelectedImageKey = tabPage.ImageKey;
                    }
                    treeNode.Tag = tabPage;
                    nodeCollection.Add(treeNode);

                    foreach (Control control in tabPage.Controls)
                    {
                        if (control is TabControl)
                        {
                            FillTreeView(treeNode.Nodes, control as TabControl, treeNode);
                            break;
                        }
                    }
                }
            }
        }

        private void tvMain_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void tvMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TabPage tabPage = e.Node.Tag as TabPage;

            if (tabPage != null)
            {
                tvMain.BeginUpdate();
                tcMain.Visible = true;
                tcMain.TabPages.Clear();
                tcMain.TabPages.Add(tabPage);
                tvMain.Focus();
                tvMain.EndUpdate();
            }
        }
    }
}