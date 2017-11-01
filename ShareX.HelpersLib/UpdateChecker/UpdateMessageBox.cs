#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class UpdateMessageBox : BlackStyleForm
    {
        public static bool IsOpen { get; private set; }
        public static bool DontShow { get; private set; }

        public bool ActivateWindow { get; private set; }

        protected override bool ShowWithoutActivation => !ActivateWindow;

        public UpdateMessageBox(bool activateWindow, bool isPortable = false)
        {
            ActivateWindow = activateWindow;
            InitializeComponent();

            if (!ActivateWindow)
            {
                WindowState = FormWindowState.Minimized;
                NativeMethods.FlashWindowEx(this, 10);
            }

            Text = Resources.UpdateMessageBox_UpdateMessageBox_update_is_available;

            if (isPortable)
            {
                lblText.Text = Helpers.SafeStringFormat(Resources.UpdateMessageBox_UpdateMessageBox_Portable, Application.ProductName);
            }
            else
            {
                lblText.Text = Helpers.SafeStringFormat(Resources.UpdateMessageBox_UpdateMessageBox_, Application.ProductName);
            }
        }

        public static DialogResult Start(UpdateChecker updateChecker, bool activateWindow = true)
        {
            DialogResult result = DialogResult.None;

            if (updateChecker != null && updateChecker.Status == UpdateStatus.UpdateAvailable)
            {
                IsOpen = true;

                try
                {
                    using (UpdateMessageBox messageBox = new UpdateMessageBox(activateWindow, updateChecker.IsPortable))
                    {
                        result = messageBox.ShowDialog();
                    }

                    if (result == DialogResult.Yes)
                    {
                        updateChecker.DownloadUpdate();
                    }
                }
                finally
                {
                    IsOpen = false;
                }
            }

            return result;
        }

        private void UpdateMessageBox_Shown(object sender, EventArgs e)
        {
            if (ActivateWindow)
            {
                this.ForceActivate();
            }
        }

        private void lblViewChangelog_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_CHANGELOG);
        }

        private void cbDontShow_CheckedChanged(object sender, EventArgs e)
        {
            DontShow = cbDontShow.Checked;
        }

        private void btnYes_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnNo_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}