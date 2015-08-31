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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.IRCLib
{
    internal class TabManager
    {
        public List<TabInfo> Tabs { get; private set; }

        public TabInfo ActiveTab
        {
            get
            {
                TabPage tabPage = tc.SelectedTab;

                if (tabPage != null)
                {
                    TabInfo tabInfo = tabPage.Tag as TabInfo;

                    if (tabInfo != null)
                    {
                        return tabInfo;
                    }
                }

                return null;
            }
        }

        private TabControl tc;

        public TabManager(TabControl tabControl)
        {
            tc = tabControl;
            Tabs = new List<TabInfo>();
        }

        public void AddMessage(string channel, string message)
        {
            TabInfo tabInfo = AddChannel(channel);
            tabInfo.AppendText(message);
        }

        public TabInfo AddChannel(string channel)
        {
            TabInfo tabInfo = Tabs.FirstOrDefault(x => x.Name.Equals(channel, StringComparison.InvariantCultureIgnoreCase));

            if (tabInfo == null)
            {
                tabInfo = new TabInfo(channel);
                Tabs.Add(tabInfo);
                Tabs.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase));
                tc.SuspendLayout();
                TabPage selected = tc.SelectedTab;
                tc.TabPages.Clear();
                tc.TabPages.AddRange(Tabs.Select(x => x.Tab).ToArray());
                if (selected != null) tc.SelectedTab = selected;
                tc.ResumeLayout();
            }

            return tabInfo;
        }
    }
}