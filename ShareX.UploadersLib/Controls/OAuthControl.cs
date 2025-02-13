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
using ShareX.UploadersLib.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class OAuthControl : UserControl
    {
        public delegate void OpenButtonClickedEventHandler();
        public event OpenButtonClickedEventHandler OpenButtonClicked;

        public delegate void CompleteButtonClickedEventHandler(string code);
        public event CompleteButtonClickedEventHandler CompleteButtonClicked;

        public delegate void ClearButtonclickedEventHandler();
        public event ClearButtonclickedEventHandler ClearButtonClicked;

        public delegate void RefreshButtonClickedEventHandler();
        public event RefreshButtonClickedEventHandler RefreshButtonClicked;

        private OAuthLoginStatus status;

        [DefaultValue(OAuthLoginStatus.LoginRequired)]
        public OAuthLoginStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                UpdateStatusLabel();
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
                UpdateStatusLabel();
            }
        }

        private bool isRefreshable;

        [DefaultValue(true)]
        public bool IsRefreshable
        {
            get
            {
                return isRefreshable;
            }
            set
            {
                isRefreshable = value;

                if (isRefreshable)
                {
                    gbUserAccount.Size = defaultGroupBoxSize;
                }
                else
                {
                    gbUserAccount.Size = smallGroupBoxSize;
                }

                btnRefreshAuthorization.Visible = isRefreshable;
            }
        }

        private Size defaultGroupBoxSize, smallGroupBoxSize;

        public OAuthControl()
        {
            InitializeComponent();
            Status = OAuthLoginStatus.LoginRequired;
            defaultGroupBoxSize = gbUserAccount.Size;
            smallGroupBoxSize = new Size(defaultGroupBoxSize.Width, (int)(defaultGroupBoxSize.Height / 1.16f));
            IsRefreshable = true;
            txtVerificationCode.HandleCreated += (sender, e) => txtVerificationCode.SetWatermark(Resources.OAuthControl_OAuthControl_PasteVerificationCodeHere);
        }

        private void btnOpenAuthorizePage_Click(object sender, EventArgs e)
        {
            OpenButtonClicked?.Invoke();
        }

        private void txtVerificationCode_TextChanged(object sender, EventArgs e)
        {
            btnCompleteAuthorization.Enabled = !string.IsNullOrEmpty(txtVerificationCode.Text);
        }

        private void btnCompleteAuthorization_Click(object sender, EventArgs e)
        {
            string code = txtVerificationCode.Text;

            if (CompleteButtonClicked != null && !string.IsNullOrEmpty(code))
            {
                CompleteButtonClicked(code);
            }
        }

        private void btnRefreshAuthorization_Click(object sender, EventArgs e)
        {
            RefreshButtonClicked?.Invoke();
        }

        private void btnClearAuthorization_Click(object sender, EventArgs e)
        {
            UserInfo = null;
            Status = OAuthLoginStatus.LoginRequired;

            ClearButtonClicked?.Invoke();
        }

        private void UpdateStatusLabel()
        {
            switch (Status)
            {
                case OAuthLoginStatus.LoginRequired:
                    lblStatusValue.Text = Resources.OAuthControl_Status_NotLoggedIn;
                    lblStatusValue.ForeColor = Color.FromArgb(200, 0, 0);
                    break;
                case OAuthLoginStatus.LoginSuccessful:
                    if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.name))
                    {
                        lblStatusValue.Text = string.Format(Resources.LoggedInAs0, UserInfo.name);
                    }
                    else
                    {
                        lblStatusValue.Text = Resources.OAuthControl_Status_LoggedIn;
                    }
                    lblStatusValue.ForeColor = Color.FromArgb(0, 160, 0);
                    break;
                case OAuthLoginStatus.LoginFailed:
                    lblStatusValue.Text = Resources.OAuthControl_Status_LoginFailed;
                    lblStatusValue.ForeColor = Color.FromArgb(200, 0, 0);
                    break;
            }

            txtVerificationCode.ResetText();
            btnClearAuthorization.Enabled = btnRefreshAuthorization.Enabled = Status == OAuthLoginStatus.LoginSuccessful;
        }
    }
}