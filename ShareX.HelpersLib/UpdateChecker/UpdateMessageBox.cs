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

using ShareX.HelpersLib.Properties;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class UpdateMessageBox : Form
    {
        public static bool IsOpen { get; private set; }
        public static bool DontShow { get; private set; }

        public bool ActivateWindow { get; set; }

        private Rectangle fillRect;

        public UpdateMessageBox()
        {
            InitializeComponent();

            Icon = ShareXResources.Icon;
            Text = Resources.UpdateMessageBox_UpdateMessageBox_update_is_available;
            lblText.Text = string.Format(Resources.UpdateMessageBox_UpdateMessageBox_, Application.ProductName);

            fillRect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
        }

        public static void Start(UpdateChecker updateChecker, bool activateWindow = true)
        {
            if (updateChecker != null && updateChecker.Status == UpdateStatus.UpdateAvailable)
            {
                IsOpen = true;

                try
                {
                    DialogResult result;

                    using (UpdateMessageBox messageBox = new UpdateMessageBox())
                    {
                        messageBox.ActivateWindow = activateWindow;
                        result = messageBox.ShowDialog();
                    }

                    if (result == DialogResult.Yes)
                    {
                        using (DownloaderForm updaterForm = new DownloaderForm(updateChecker))
                        {
                            updaterForm.ShowDialog();

                            if (updaterForm.Status == DownloaderFormStatus.InstallStarted)
                            {
                                Application.Exit();
                            }
                        }
                    }
                }
                finally
                {
                    IsOpen = false;
                }
            }
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return !ActivateWindow;
            }
        }

        private void UpdateMessageBox_Shown(object sender, System.EventArgs e)
        {
            if (ActivateWindow)
            {
                this.ShowActivate();
            }
        }

        private void UpdateMessageBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (LinearGradientBrush brush = new LinearGradientBrush(fillRect, Color.FromArgb(80, 80, 80), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, fillRect);
            }
        }

        private void cbDontShow_CheckedChanged(object sender, System.EventArgs e)
        {
            DontShow = cbDontShow.Checked;
        }

        private void btnYes_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnNo_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}