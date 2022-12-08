#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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

using ShareX.UploadersLib.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class OAuthLoopbackControl : UserControl
    {
        public event Action ConnectButtonClicked;
        public event Action DisconnectButtonClicked;

        private bool connected;

        [DefaultValue(false)]
        public bool Connected
        {
            get
            {
                return connected;
            }
            set
            {
                if (connected != value)
                {
                    connected = value;
                    UpdateStatus();
                }
            }
        }

        private OAuthUserInfo userInfo;

        public OAuthUserInfo UserInfo
        {
            get
            {
                return userInfo;
            }
            set
            {
                userInfo = value;
                UpdateStatus();
            }
        }

        public OAuthLoopbackControl()
        {
            InitializeComponent();
            UpdateStatus();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (Connected)
            {
                DisconnectButtonClicked?.Invoke();

                Connected = false;
                UserInfo = null;
            }
            else
            {
                ConnectButtonClicked?.Invoke();
            }
        }

        private void UpdateStatus()
        {
            if (Connected)
            {
                // TODO: Translate
                btnConnect.Text = "Disconnect";
                if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.name))
                {
                    lblStatusValue.Text = string.Format(Resources.LoggedInAs0, UserInfo.name);
                }
                else
                {
                    lblStatusValue.Text = Resources.OAuthControl_Status_LoggedIn;
                }
                lblStatusValue.ForeColor = Color.FromArgb(0, 180, 0);
            }
            else
            {
                btnConnect.Text = "Connect...";
                lblStatusValue.Text = Resources.OAuthControl_Status_NotLoggedIn;
                lblStatusValue.ForeColor = Color.FromArgb(220, 0, 0);
            }
        }
    }
}