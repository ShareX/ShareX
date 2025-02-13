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

using System;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class OutputBox : Form
    {
        public bool ScrollToEnd { get; private set; }

        private OutputBox(string text, string title, bool scrollToEnd = false)
        {
            InitializeComponent();
            rtbText.AddContextMenu();
            ShareXResources.ApplyTheme(this, true);

            Text = "ShareX - " + title;
            rtbText.Text = text;
            ScrollToEnd = scrollToEnd;
        }

        public static void Show(string text, string title, bool scrollToEnd = false)
        {
            using (OutputBox outputBox = new OutputBox(text, title, scrollToEnd))
            {
                outputBox.ShowDialog();
            }
        }

        private void OutputBox_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();

            rtbText.SelectionStart = rtbText.TextLength;

            if (ScrollToEnd)
            {
                NativeMethods.SendMessage(rtbText.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_BOTTOM, 0);
            }
            else
            {
                NativeMethods.SendMessage(rtbText.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_TOP, 0);
            }
        }
    }
}