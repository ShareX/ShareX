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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class CodeMenu : ContextMenuStrip
    {
        public Point MenuLocation
        {
            get
            {
                if (MenuLocationBottom)
                {
                    return new Point(MenuLocationOffset.X, textBoxBase.Height + MenuLocationOffset.Y + 1);
                }

                return new Point(textBoxBase.Width + MenuLocationOffset.X + 1, MenuLocationOffset.Y);
            }
        }

        public Point MenuLocationOffset { get; set; }

        public bool MenuLocationBottom { get; set; }

        private TextBoxBase textBoxBase;

        public CodeMenu(TextBoxBase tbb, CodeMenuItem[] items)
        {
            textBoxBase = tbb;

            Font = new Font("Lucida Console", 8);
            AutoClose = textBoxBase == null;
            ShowImageMargin = false;

            foreach (CodeMenuItem item in items)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = $"{item.Name} - {item.Description}", Tag = item.Name };

                tsmi.MouseUp += (sender, e) =>
                {
                    if (textBoxBase != null && e.Button == MouseButtons.Left)
                    {
                        string text = ((ToolStripMenuItem)sender).Tag.ToString();
                        textBoxBase.AppendTextToSelection(text);
                    }
                    else
                    {
                        Close();
                    }
                };

                if (string.IsNullOrWhiteSpace(item.Category))
                {
                    Items.Add(tsmi);
                }
                else
                {
                    ToolStripMenuItem tsmiParent;
                    int index = Items.IndexOfKey(item.Category);
                    if (index < 0)
                    {
                        tsmiParent = new ToolStripMenuItem { Text = item.Category, Tag = item.Category, Name = item.Category };
                        tsmiParent.HideImageMargin();
                        Items.Add(tsmiParent);
                    }
                    else
                    {
                        tsmiParent = Items[index] as ToolStripMenuItem;
                    }
                    tsmiParent.DropDownItems.Add(tsmi);
                }
            }

            Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem(Resources.CodeMenu_Create_Close);
            tsmiClose.Click += (sender, e) => Close();
            Items.Add(tsmiClose);

            if (ShareXResources.UseCustomTheme)
            {
                ShareXResources.ApplyCustomThemeToContextMenuStrip(this);
            }

            if (textBoxBase != null)
            {
                textBoxBase.MouseDown += (sender, e) =>
                {
                    if (Items.Count > 0) Show(textBoxBase, MenuLocation);
                };

                textBoxBase.GotFocus += (sender, e) =>
                {
                    if (Items.Count > 0) Show(textBoxBase, MenuLocation);
                };

                textBoxBase.LostFocus += (sender, e) =>
                {
                    if (Visible) Close();
                };

                textBoxBase.KeyDown += (sender, e) =>
                {
                    if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) && Visible)
                    {
                        Close();
                        e.SuppressKeyPress = true;
                    }
                };

                textBoxBase.Disposed += (sender, e) => Dispose();
            }
        }

        public static CodeMenu Create<TEntry>(TextBoxBase tb, TEntry[] ignoreList, CodeMenuItem[] extraItems) where TEntry : CodeMenuEntry
        {
            List<CodeMenuItem> items = new List<CodeMenuItem>();

            if (extraItems != null)
            {
                items.AddRange(extraItems);
            }

            IEnumerable<CodeMenuItem> codeMenuItems = Helpers.GetValueFields<TEntry>().Where(x => !ignoreList.Contains(x)).
                Select(x => new CodeMenuItem(x.ToPrefixString(), x.Description, x.Category));

            items.AddRange(codeMenuItems);

            return new CodeMenu(tb, items.ToArray());
        }

        public static CodeMenu Create<TEntry>(TextBoxBase tb, params TEntry[] ignoreList) where TEntry : CodeMenuEntry
        {
            return Create(tb, ignoreList, (CodeMenuItem[])null);
        }
    }
}