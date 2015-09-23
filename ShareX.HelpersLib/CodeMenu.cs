#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class CodeMenu
    {
        public static ContextMenuStrip Create<TEntry>(TextBox tb, params TEntry[] ignoreList)
            where TEntry : CodeMenuEntry
        {
            ContextMenuStrip cms = new ContextMenuStrip
            {
                Font = new Font("Lucida Console", 8),
                AutoClose = false,
                Opacity = 0.9,
                ShowImageMargin = false
            };

            var variables = Helpers.GetValueFields<TEntry>().Where(x => !ignoreList.Contains(x)).
                Select(x => new
                {
                    Name = x.ToPrefixString(),
                    Description = x.Description,
                    Category = x.Category
                });

            foreach (var variable in variables)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = string.Format("{0} - {1}", variable.Name, variable.Description), Tag = variable.Name };
                tsmi.Click += (sender, e) =>
                {
                    string text = ((ToolStripMenuItem)sender).Tag.ToString();
                    tb.AppendTextToSelection(text);
                };

                if (string.IsNullOrWhiteSpace(variable.Category))
                {
                    cms.Items.Add(tsmi);
                }
                else
                {
                    ToolStripMenuItem tsmiParent;
                    int index = cms.Items.IndexOfKey(variable.Category);
                    if (0 > index)
                    {
                        tsmiParent = new ToolStripMenuItem { Text = variable.Category, Tag = variable.Category, Name = variable.Category };
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

            return cms;
        }

        public static ContextMenuStrip Create<TEntry>(params TEntry[] ignoreList)
            where TEntry : CodeMenuEntry
        {
            ContextMenuStrip cms = new ContextMenuStrip
            {
                Font = new Font("Lucida Console", 8),
                AutoClose = true,
                Opacity = 0.9,
                ShowImageMargin = false
            };

            var variables = Helpers.GetValueFields<TEntry>().Where(x => !ignoreList.Contains(x)).
                Select(x => new
                {
                    Name = x.ToPrefixString(),
                    Description = x.Description
                });

            foreach (var variable in variables)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = string.Format("{0} - {1}", variable.Name, variable.Description), Tag = variable.Name };
                cms.Items.Add(tsmi);
            }

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem(Resources.CodeMenu_Create_Close);
            tsmiClose.Click += (sender, e) => cms.Close();
            cms.Items.Add(tsmiClose);

            return cms;
        }
    }
}