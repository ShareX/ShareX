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
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public partial class ImageHistorySettingsForm : Form
    {
        public ImageHistorySettings Settings { get; private set; }

        public ImageHistorySettingsForm(ImageHistorySettings settings)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Settings = settings;
            nudThumbnailSize.SetValue(Settings.ThumbnailSize.Width);
            nudMaximumImageLimit.SetValue(Settings.MaxItemCount);
            cbFilterMissingFiles.Checked = Settings.FilterMissingFiles;
            cbRememberSearchText.Checked = Settings.RememberSearchText;
            cbRememberWindowState.Checked = Settings.RememberWindowState;
        }

        private void nudThumbnailSize_ValueChanged(object sender, EventArgs e)
        {
            Settings.ThumbnailSize = new Size((int)nudThumbnailSize.Value, (int)nudThumbnailSize.Value);
        }

        private void nudMaximumImageLimit_ValueChanged(object sender, EventArgs e)
        {
            Settings.MaxItemCount = (int)nudMaximumImageLimit.Value;
        }

        private void cbFilterMissingFiles_CheckedChanged(object sender, EventArgs e)
        {
            Settings.FilterMissingFiles = cbFilterMissingFiles.Checked;
        }

        private void cbRememberSearchText_CheckedChanged(object sender, EventArgs e)
        {
            Settings.RememberSearchText = cbRememberSearchText.Checked;
        }

        private void cbRememberWindowState_CheckedChanged(object sender, EventArgs e)
        {
            Settings.RememberWindowState = cbRememberWindowState.Checked;
        }
    }
}