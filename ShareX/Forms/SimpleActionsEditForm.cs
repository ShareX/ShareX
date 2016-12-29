#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class SimpleActionsEditForm : Form
    {
        public List<HotkeyType> Actions { get; private set; }

        public SimpleActionsEditForm(List<HotkeyType> actions)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            Actions = actions;

            foreach (HotkeyType hotkeyType in Helpers.GetEnums<HotkeyType>())
            {
                Image img;

                if (hotkeyType == HotkeyType.None)
                {
                    img = Resources.ui_splitter;
                }
                else
                {
                    img = TaskHelpers.GetHotkeyTypeIcon(hotkeyType);
                }

                ilMain.Images.Add(hotkeyType.ToString(), img);
            }

            AddEnumItemsContextMenu(x =>
            {
                AddActionToList(x);
            }, cmsAction);

            foreach (HotkeyType action in Actions)
            {
                AddActionToList(action);
            }
        }

        private void AddEnumItemsContextMenu(Action<HotkeyType> selectedEnum, params ToolStripDropDown[] parents)
        {
            EnumInfo[] enums = Helpers.GetEnums<HotkeyType>().OfType<Enum>().Select(x => new EnumInfo(x)).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                foreach (EnumInfo enumInfo in enums)
                {
                    HotkeyType hotkeyType = (HotkeyType)Enum.ToObject(typeof(HotkeyType), enumInfo.Value);

                    Image img;

                    if (hotkeyType == HotkeyType.None)
                    {
                        img = Resources.ui_splitter;
                    }
                    else
                    {
                        img = TaskHelpers.GetHotkeyTypeIcon(hotkeyType);
                    }

                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enumInfo.Description.Replace("&", "&&"));
                    tsmi.Image = img;
                    tsmi.Tag = enumInfo;

                    tsmi.Click += (sender, e) =>
                    {
                        selectedEnum(hotkeyType);
                    };

                    if (!string.IsNullOrEmpty(enumInfo.Category))
                    {
                        ToolStripMenuItem tsmiParent = parent.Items.OfType<ToolStripMenuItem>().FirstOrDefault(x => x.Text == enumInfo.Category);

                        if (tsmiParent == null)
                        {
                            tsmiParent = new ToolStripMenuItem(enumInfo.Category);
                            parent.Items.Add(tsmiParent);
                        }

                        tsmiParent.DropDownItems.Add(tsmi);
                    }
                    else
                    {
                        parent.Items.Add(tsmi);
                    }
                }
            }
        }

        private void AddActionToList(HotkeyType hotkeyType)
        {
            ListViewItem lvi = new ListViewItem()
            {
                Text = hotkeyType.GetLocalizedDescription(),
                ImageKey = hotkeyType.ToString()
            };

            lvActions.Items.Add(lvi);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvActions.SelectedItems.Count > 0)
            {
                lvActions.Items.Remove(lvActions.SelectedItems[0]);
            }
        }
    }
}