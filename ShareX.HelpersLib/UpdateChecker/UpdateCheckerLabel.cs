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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class UpdateCheckerLabel : UserControl
    {
        public bool IsBusy { get; private set; }

        private UpdateChecker updateChecker;

        public UpdateCheckerLabel()
        {
            InitializeComponent();
        }

        public async Task CheckUpdate(UpdateChecker updateChecker)
        {
            if (!IsBusy)
            {
                IsBusy = true;

                this.updateChecker = updateChecker;

                lblStatus.Visible = false;
                llblUpdateAvailable.Visible = false;

                pbLoading.Visible = true;
                lblCheckingUpdates.Visible = true;

                await CheckingUpdate();
            }
        }

        public void UpdateLoadingImage()
        {
            if (ShareXResources.IsDarkTheme)
            {
                pbLoading.Image = Resources.LoadingSmallWhite;
            }
            else
            {
                pbLoading.Image = Resources.LoadingSmallBlack;
            }
        }

        private async Task CheckingUpdate()
        {
            await updateChecker.CheckUpdateAsync();

            try
            {
                UpdateControls();
            }
            catch
            {
            }

            IsBusy = false;
        }

        private void UpdateControls()
        {
            this.InvokeSafe(() =>
            {
                pbLoading.Visible = false;
                lblCheckingUpdates.Visible = false;

                switch (updateChecker.Status)
                {
                    case UpdateStatus.UpdateCheckFailed:
                        lblStatus.Text = Resources.UpdateCheckerLabel_UpdateControls_Update_check_failed;
                        lblStatus.Visible = true;
                        break;
                    case UpdateStatus.UpdateAvailable:
                        llblUpdateAvailable.Text = string.Format(Resources.UpdateCheckerLabel_UpdateControls_A_newer_version_of_ShareX_is_available, Application.ProductName);
                        llblUpdateAvailable.Visible = true;
                        break;
                    case UpdateStatus.UpToDate:
                        lblStatus.Text = string.Format(Resources.UpdateCheckerLabel_UpdateControls_ShareX_is_up_to_date, Application.ProductName);
                        lblStatus.Visible = true;
                        break;
                }
            });
        }

        private void llblUpdateAvailable_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UpdateMessageBox.Start(updateChecker);
        }
    }
}