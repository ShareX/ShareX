#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace HistoryLib.CustomControls
{
    public class ObjectListView : MyListView
    {
        public enum ObjectType
        {
            Fields,
            Properties
        }

        public ObjectType SetObjectType { get; set; }

        public object SelectedObject
        {
            set
            {
                SelectObject(value);
            }
        }

        public ObjectListView()
        {
            SetObjectType = ObjectType.Properties;
            MultiSelect = false;
            Columns.Add("Name", 125);
            Columns.Add("Value", 300);
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Copy name").Click += PropertyListView_Click_Name;
            contextMenu.MenuItems.Add("Copy value").Click += PropertyListView_Click_Value;
            ContextMenu = contextMenu;
        }

        private void PropertyListView_Click_Name(object sender, EventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                string text = SelectedItems[0].Text;
                if (!string.IsNullOrEmpty(text))
                {
                    ClipboardHelpers.CopyText(text);
                }
            }
        }

        private void PropertyListView_Click_Value(object sender, EventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                string text = SelectedItems[0].SubItems[1].Text;
                if (!string.IsNullOrEmpty(text))
                {
                    ClipboardHelpers.CopyText(text);
                }
            }
        }

        public void SelectObject(object obj)
        {
            Items.Clear();

            if (obj != null)
            {
                Type type = obj.GetType();

                if (SetObjectType == ObjectType.Fields)
                {
                    foreach (FieldInfo property in type.GetFields())
                    {
                        AddObject(property.GetValue(obj), property.Name);
                    }
                }
                else if (SetObjectType == ObjectType.Properties)
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        AddObject(property.GetValue(obj, null), property.Name);
                    }
                }

                FillLastColumn();
            }
        }

        private void AddObject(object obj, string name)
        {
            if (obj is HistoryItem)
            {
                SelectObject(obj);
                return;
            }

            ListViewItem lvi = new ListViewItem(name);
            lvi.Tag = obj;
            if (obj != null)
            {
                lvi.SubItems.Add(obj.ToString());
            }

            Items.Add(lvi);
        }
    }
}