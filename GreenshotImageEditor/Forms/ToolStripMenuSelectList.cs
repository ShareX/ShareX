/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Forms
{
    /// <summary>
    /// Description of ToolStripMenuSelectList.
    /// </summary>
    public class ToolStripMenuSelectList : ToolStripMenuItem
    {
        private bool multiCheckAllowed = false;
        private bool updateInProgress = false;
        /// <summary>
        /// Occurs when one of the list's child element's Checked state changes.
        /// </summary>
        public new event EventHandler CheckedChanged;
        public Object Identifier;

        public ToolStripMenuSelectList(Object identifier, bool allowMultiCheck)
        {
            Identifier = identifier;
            CheckOnClick = false;
            multiCheckAllowed = allowMultiCheck;
        }

        public ToolStripMenuSelectList()
            : this(null, false)
        {
        }

        public ToolStripMenuSelectList(Object identifier)
            : this(identifier, false)
        {
        }

        /// <summary>
        /// gets or sets the currently checked item
        /// </summary>
        public Object CheckedItem
        {
            get
            {
                IEnumerator items = DropDownItems.GetEnumerator();
                while (items.MoveNext())
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)items.Current;
                    if (tsmi.Checked)
                    {
                        return tsmi;
                    }
                }
                return null;
            }
            set
            {
                IEnumerator items = DropDownItems.GetEnumerator();
                while (items.MoveNext())
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)items.Current;
                    if (!multiCheckAllowed && !tsmi.Equals(value))
                    {
                        tsmi.Checked = false;
                    }
                    else if (tsmi.Equals(value))
                    {
                        tsmi.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// gets or sets the currently checked items
        /// </summary>
        public ToolStripMenuSelectListItem[] CheckedItems
        {
            get
            {
                List<ToolStripMenuSelectListItem> sel = new List<ToolStripMenuSelectListItem>();
                IEnumerator items = DropDownItems.GetEnumerator();
                while (items.MoveNext())
                {
                    ToolStripMenuSelectListItem tsmi = (ToolStripMenuSelectListItem)items.Current;
                    if (tsmi.Checked)
                    {
                        sel.Add(tsmi);
                    }
                }
                return sel.ToArray();
            }
            set
            {
                if (!multiCheckAllowed)
                {
                    throw new ArgumentException("Writing to checkedItems is only allowed in multi-check mode. Either set allowMultiCheck to true or use set SelectedItem instead of SelectedItems.");
                }
                IEnumerator items = DropDownItems.GetEnumerator();
                IEnumerator sel = value.GetEnumerator();
                while (items.MoveNext())
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)items.Current;
                    while (sel.MoveNext())
                    {
                        if (tsmi.Equals(sel.Current))
                        {
                            tsmi.Checked = true;
                        }
                        else
                        {
                            tsmi.Checked = false;
                        }
                        if (!multiCheckAllowed && !tsmi.Equals(sel.Current))
                        {
                            tsmi.Checked = false;
                        }
                        else if (tsmi.Equals(value))
                        {
                            tsmi.Checked = true;
                        }
                    }
                }
            }
        }

        private void ItemCheckStateChanged(object sender, EventArgs e)
        {
            if (updateInProgress)
            {
                return;
            }
            ToolStripMenuSelectListItem tsmi = (ToolStripMenuSelectListItem)sender;
            updateInProgress = true;
            if (tsmi.Checked && !multiCheckAllowed)
            {
                UncheckAll();
                tsmi.Checked = true;
            }
            updateInProgress = false;
            if (CheckedChanged != null)
            {
                CheckedChanged(this, new ItemCheckedChangedEventArgs(tsmi));
            }
        }

        /// <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        /// <param name="image">the icon to be displayed</param>
        /// <param name="data">the data to be returned when an item is queried</param>
        /// <param name="isChecked">whether the item is initially checked</param>
        public void AddItem(string label, Image image, Object data, bool isChecked)
        {
            ToolStripMenuSelectListItem newItem = new ToolStripMenuSelectListItem();
            newItem.Text = label;
            newItem.Image = image;
            newItem.CheckOnClick = true;
            newItem.CheckStateChanged += ItemCheckStateChanged;
            newItem.Data = data;
            if (isChecked)
            {
                if (!multiCheckAllowed)
                {
                    updateInProgress = true;
                    UncheckAll();
                    updateInProgress = false;
                }
                newItem.Checked = isChecked;
            }
            DropDownItems.Add(newItem);
        }

        /// <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        /// <param name="image">the icon to be displayed</param>
        public void AddItem(string label, Image image)
        {
            AddItem(label, image, null, false);
        }

        /// <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        /// <param name="data">the data to be returned when an item is queried</param>
        public void AddItem(string label, Object data)
        {
            AddItem(label, null, data, false);
        }

        /// <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        public void AddItem(string label)
        {
            AddItem(label, null, null, false);
        }

        // <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        /// <param name="image">the icon to be displayed</param>
        /// <param name="selected">whether the item is initially selected</param>
        /// <param name="isChecked">whether the item is initially checked</param>
        public void AddItem(string label, Image image, bool isChecked)
        {
            AddItem(label, image, null, isChecked);
        }

        /// <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        /// <param name="data">the data to be returned when an item is queried</param>
        /// <param name="isChecked">whether the item is initially checked</param>
        public void AddItem(string label, Object data, bool isChecked)
        {
            AddItem(label, null, data, isChecked);
        }

        /// <summary>
        /// adds an item to the select list
        /// </summary>
        /// <param name="label">the label to be displayed</param>
        /// <param name="isChecked">whether the item is initially checked</param>
        public void AddItem(string label, bool isChecked)
        {
            AddItem(label, null, null, isChecked);
        }

        /// <summary>
        /// unchecks all items of the list
        /// </summary>
        public void UncheckAll()
        {
            IEnumerator items = DropDownItems.GetEnumerator();
            while (items.MoveNext())
            {
                ((ToolStripMenuItem)items.Current).Checked = false;
            }
        }
    }

    public class ItemCheckedChangedEventArgs : EventArgs
    {
        public ToolStripMenuSelectListItem Item;

        public ItemCheckedChangedEventArgs(ToolStripMenuSelectListItem item)
        {
            Item = item;
        }
    }

    public class ToolStripMenuSelectListItem : ToolStripMenuItem
    {
        public Object Data;
    }
}