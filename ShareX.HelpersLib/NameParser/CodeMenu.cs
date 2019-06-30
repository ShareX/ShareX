#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
    public static class CodeMenu
    {
        public static ContextMenuStrip Create<TEntry>(TextBoxBase tb, params TEntry[] ignoreList) where TEntry : CodeMenuEntry
        {
            return Create(tb, ignoreList, (CodeMenuItem[])null);
        }

        public static ContextMenuStrip Create<TEntry>(TextBoxBase tb, TEntry[] ignoreList, CodeMenuItem[] extraItems) where TEntry : CodeMenuEntry
        {
            List<CodeMenuItem> items = new List<CodeMenuItem>();

            if (extraItems != null)
            {
                items.AddRange(extraItems);
            }

            IEnumerable<CodeMenuItem> variables = Helpers.GetValueFields<TEntry>().Where(x => !ignoreList.Contains(x)).
                Select(x => new CodeMenuItem(x.ToPrefixString(), x.Description, x.Category));

            items.AddRange(variables);

            return Create(tb, items.ToArray());
        }

        public static ContextMenuStrip Create(TextBoxBase tb, CodeMenuItem[] items)
        {
            ContextMenuStrip cms = new ContextMenuStrip
            {
                Font = new Font("Lucida Console", 8),
                AutoClose = tb == null,
                Opacity = 0.9,
                ShowImageMargin = false
            };

            if (ShareXResources.ExperimentalDarkTheme)
            {
                cms.Renderer = new ToolStripDarkRenderer();
            }

            foreach (CodeMenuItem item in items)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = $"{item.Name} - {item.Description}", Tag = item.Name };
                tsmi.MouseUp += (sender, e) =>
                {
                    if (tb != null && e.Button == MouseButtons.Left)
                    {
                        string text = ((ToolStripMenuItem)sender).Tag.ToString();
                        tb.AppendTextToSelection(text);
                    }
                    else
                    {
                        cms.Close();
                    }
                };

                if (string.IsNullOrWhiteSpace(item.Category))
                {
                    cms.Items.Add(tsmi);
                }
                else
                {
                    ToolStripMenuItem tsmiParent;
                    int index = cms.Items.IndexOfKey(item.Category);
                    if (index < 0)
                    {
                        tsmiParent = new ToolStripMenuItem { Text = item.Category, Tag = item.Category, Name = item.Category };
                        tsmiParent.HideImageMargin();
                        cms.Items.Add(tsmiParent);
                    }
                    else
                    {
                        tsmiParent = cms.Items[index] as ToolStripMenuItem;
                    }
                    tsmiParent.DropDownItems.Add(tsmi);
                }
            }

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem(Resources.CodeMenu_Create_Close);
            tsmiClose.Click += (sender, e) => cms.Close();
            cms.Items.Add(tsmiClose);

            if (tb != null)
            {
                tb.MouseDown += (sender, e) =>
                {
                    if (cms.Items.Count > 0) cms.Show(tb, new Point(tb.Width + 1, 0));
                };

                tb.GotFocus += (sender, e) =>
                {
                    if (cms.Items.Count > 0) cms.Show(tb, new Point(tb.Width + 1, 0));
                };

                tb.LostFocus += (sender, e) =>
                {
                    if (cms.Visible) cms.Close();
                };

                tb.KeyDown += (sender, e) =>
                {
                    if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) && cms.Visible)
                    {
                        cms.Close();
                        e.SuppressKeyPress = true;
                    }
                };

                tb.Disposed += (sender, e) => cms.Dispose();
            }

            return cms;
        }
    }
}