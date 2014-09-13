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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HelpersLib
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
                });

            foreach (var variable in variables)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = string.Format("{0} - {1}", variable.Name, variable.Description), Tag = variable.Name };
                tsmi.Click += (sender, e) =>
                {
                    string text = ((ToolStripMenuItem)sender).Tag.ToString();
                    tb.AppendTextToSelection(text);
                };
                cms.Items.Add(tsmi);
            }

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem("Close");
            tsmiClose.Click += (sender, e) => cms.Close();
            cms.Items.Add(tsmiClose);

            tb.MouseDown += (sender, e) =>
            {
                if (cms.Items.Count > 0) cms.Show(tb, new Point(tb.Width + 1, 0));
            };

            tb.Leave += (sender, e) =>
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
    }
}