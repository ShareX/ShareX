#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
        public OutputBox(string text, string title)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Text = "ShareX - " + title;
            txtText.Text = text;
        }

        private void OutputBox_Shown(object sender, EventArgs e)
        {
            txtText.SelectionStart = txtText.TextLength;
            txtText.ScrollToCaret();
        }

        private void txtText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}