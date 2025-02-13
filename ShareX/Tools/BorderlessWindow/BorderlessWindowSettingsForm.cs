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
using System;
using System.Windows.Forms;

namespace ShareX
{
    public partial class BorderlessWindowSettingsForm : Form
    {
        public BorderlessWindowSettings Settings { get; private set; }

        public BorderlessWindowSettingsForm(BorderlessWindowSettings settings)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Settings = settings;
            cbRememberWindowTitle.Checked = Settings.RememberWindowTitle;
            cbAutoCloseWindow.Checked = Settings.AutoCloseWindow;
            cbExcludeTaskbarArea.Checked = Settings.ExcludeTaskbarArea;
        }

        private void cbRememberWindowTitle_CheckedChanged(object sender, EventArgs e)
        {
            Settings.RememberWindowTitle = cbRememberWindowTitle.Checked;
        }

        private void cbAutoCloseWindow_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AutoCloseWindow = cbAutoCloseWindow.Checked;
        }

        private void cbExcludeTaskbarArea_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ExcludeTaskbarArea = cbExcludeTaskbarArea.Checked;
        }
    }
}