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

using ShareX.HelpersLib.Properties;
using System;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class UpdateMessageBox : Form
    {
        public static bool IsOpen { get; private set; }

        public bool ActivateWindow { get; private set; }

        protected override bool ShowWithoutActivation => !ActivateWindow;

        public UpdateMessageBox(UpdateChecker updateChecker, bool activateWindow = true)
        {
            ActivateWindow = activateWindow;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            if (!ActivateWindow)
            {
                WindowState = FormWindowState.Minimized;
                NativeMethods.FlashWindowEx(this, 10);
            }

            Text = Resources.UpdateMessageBox_UpdateMessageBox_update_is_available;

            StringBuilder sbText = new StringBuilder();

            if (updateChecker.IsPortable)
            {
                sbText.AppendLine(Helpers.SafeStringFormat(Resources.UpdateMessageBox_UpdateMessageBox_Portable, Application.ProductName));
            }
            else
            {
                sbText.AppendLine(Helpers.SafeStringFormat(Resources.UpdateMessageBox_UpdateMessageBox_, Application.ProductName));
            }

            sbText.AppendLine();
            sbText.Append(Resources.UpdateMessageBox_UpdateMessageBox_CurrentVersion);
            sbText.Append(": ");
            sbText.Append(updateChecker.CurrentVersion);
            sbText.AppendLine();
            sbText.Append(Resources.UpdateMessageBox_UpdateMessageBox_LatestVersion);
            sbText.Append(": ");
            sbText.Append(updateChecker.LatestVersion);
            if (updateChecker.IsDev) sbText.Append(" Dev");
            if (updateChecker is GitHubUpdateChecker githubUpdateChecker && githubUpdateChecker.IsPreRelease) sbText.Append(" (Pre-release)");

            lblText.Text = sbText.ToString();

            lblViewChangelog.Visible = !updateChecker.IsDev;
        }

        public static DialogResult Start(UpdateChecker updateChecker, bool activateWindow = true)
        {
            DialogResult result = DialogResult.None;

            if (updateChecker != null && updateChecker.Status == UpdateStatus.UpdateAvailable)
            {
                IsOpen = true;

                try
                {
                    using (UpdateMessageBox messageBox = new UpdateMessageBox(updateChecker, activateWindow))
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

        private void UpdateMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult = DialogResult.No;
            }
        }

        private void lblViewChangelog_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.Changelog);
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