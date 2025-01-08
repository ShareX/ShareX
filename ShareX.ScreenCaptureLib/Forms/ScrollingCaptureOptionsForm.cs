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

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureOptionsForm : Form
    {
        public ScrollingCaptureOptions Options { get; private set; }

        public ScrollingCaptureOptionsForm(ScrollingCaptureOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            LoadOptions();
        }

        private void LoadOptions()
        {
            nudStartDelay.SetValue(Options.StartDelay);
            cbAutoScrollTop.Checked = Options.AutoScrollTop;
            nudScrollDelay.SetValue(Options.ScrollDelay);
            nudScrollAmount.SetValue(Options.ScrollAmount);
            cbAutoUpload.Checked = Options.AutoUpload;
            cbShowRegion.Checked = Options.ShowRegion;
            cbAutoIgnoreBottomEdge.Checked = Options.AutoIgnoreBottomEdge;
        }

        private void SaveOptions()
        {
            Options.StartDelay = (int)nudStartDelay.Value;
            Options.AutoScrollTop = cbAutoScrollTop.Checked;
            Options.ScrollDelay = (int)nudScrollDelay.Value;
            Options.ScrollAmount = (int)nudScrollAmount.Value;
            Options.AutoUpload = cbAutoUpload.Checked;
            Options.ShowRegion = cbShowRegion.Checked;
            Options.AutoIgnoreBottomEdge = cbAutoIgnoreBottomEdge.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveOptions();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}