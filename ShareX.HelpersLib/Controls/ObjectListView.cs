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

using ShareX.HelpersLib.Properties;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ObjectListView : MyListView
    {
        public enum ObjectType
        {
            Fields,
            Properties
        }

        [DefaultValue(ObjectType.Properties)]
        public ObjectType SetObjectType { get; set; }

        private object selectedObject;

        public object SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                selectedObject = value;

                SelectObject(selectedObject);
            }
        }

        public ObjectListView()
        {
            SetObjectType = ObjectType.Properties;
            MultiSelect = false;
            Columns.Add(Resources.ObjectListView_ObjectListView_Name, 125);
            Columns.Add(Resources.ObjectListView_ObjectListView_Value, 300);

            ContextMenuStrip cms = new ContextMenuStrip();
            cms.ShowImageMargin = false;
            cms.Items.Add(Resources.ObjectListView_ObjectListView_Copy_name).Click += PropertyListView_Click_Name;
            cms.Items.Add(Resources.ObjectListView_ObjectListView_Copy_value).Click += PropertyListView_Click_Value;
            ContextMenuStrip = cms;
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
            if (SelectedItems.Count > 0 && SelectedItems[0].SubItems.Count > 1)
            {
                string text = SelectedItems[0].SubItems[1].Text;

                if (!string.IsNullOrEmpty(text))
                {
                    ClipboardHelpers.CopyText(text);
                }
            }
        }

        private void SelectObject(object obj)
        {
            Items.Clear();

            if (obj != null)
            {
                Type type = obj.GetType();

                if (SetObjectType == ObjectType.Fields)
                {
                    foreach (FieldInfo field in type.GetFields())
                    {
                        AddObject(field.GetValue(obj), field.Name);
                    }
                }
                else if (SetObjectType == ObjectType.Properties)
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        AddObject(property.GetValue(obj, null), property.Name);
                    }
                }
            }
        }

        private void AddObject(object obj, string name)
        {
            ListViewItem lvi = new ListViewItem(name);

            if (obj != null)
            {
                lvi.Tag = obj;
                lvi.SubItems.Add(obj.ToString());
            }

            Items.Add(lvi);
        }
    }
}