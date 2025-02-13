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

using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ActionsToolbarEditForm : Form
    {
        public List<HotkeyType> Actions { get; private set; }

        public ActionsToolbarEditForm(List<HotkeyType> actions)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

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
                    img = TaskHelpers.FindMenuIcon(hotkeyType);
                }

                ilMain.Images.Add(hotkeyType.ToString(), img);
            }

            AddEnumItemsContextMenu(AddAction, cmsAction);

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

                    string text;
                    Image img;

                    if (hotkeyType == HotkeyType.None)
                    {
                        text = Resources.ActionsToolbarEditForm_Separator;
                        img = Resources.ui_splitter;
                    }
                    else
                    {
                        text = enumInfo.Description.Replace("&", "&&");
                        img = TaskHelpers.FindMenuIcon(hotkeyType);
                    }

                    ToolStripMenuItem tsmi = new ToolStripMenuItem(text);
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

        private void AddAction(HotkeyType hotkeyType)
        {
            Actions.Add(hotkeyType);
            AddActionToList(hotkeyType);
        }

        private void AddActionToList(HotkeyType hotkeyType)
        {
            string text;

            if (hotkeyType == HotkeyType.None)
            {
                text = Resources.ActionsToolbarEditForm_Separator;
            }
            else
            {
                text = hotkeyType.GetLocalizedDescription();
            }

            ListViewItem lvi = new ListViewItem()
            {
                Text = text,
                ImageKey = hotkeyType.ToString()
            };

            lvActions.Items.Add(lvi);
        }

        private void RemoveAction(int index)
        {
            Actions.RemoveAt(index);
            lvActions.Items.RemoveAt(index);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvActions.SelectedIndex >= 0)
            {
                RemoveAction(lvActions.SelectedIndex);
            }
        }

        private void lvActions_ItemMoved(object sender, int oldIndex, int newIndex)
        {
            Actions.Move(oldIndex, newIndex);
        }
    }
}