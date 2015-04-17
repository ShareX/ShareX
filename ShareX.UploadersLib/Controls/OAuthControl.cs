#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

                switch (status)
                {
                    case OAuthLoginStatus.LoginRequired:
                        lblLoginStatus.Text = Resources.OAuthControl_Status_Status__Not_logged_in_;
                        break;
                    case OAuthLoginStatus.LoginSuccessful:
                        lblLoginStatus.Text = Resources.OAuthControl_Status_Status__Logged_in_;
                        break;
                    case OAuthLoginStatus.LoginFailed:
                        lblLoginStatus.Text = Resources.OAuthControl_Status_Status__Login_failed_;
                        break;
                }

                txtVerificationCode.ResetText();
                btnClearAuthorization.Enabled = btnRefreshAuthorization.Enabled = status == OAuthLoginStatus.LoginSuccessful;
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
        }

        private void btnOpenAuthorizePage_Click(object sender, EventArgs e)
        {
            if (OpenButtonClicked != null)
            {
                OpenButtonClicked();
            }
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
            if (RefreshButtonClicked != null)
            {
                RefreshButtonClicked();
            }
        }

        private void btnClearAuthorization_Click(object sender, EventArgs e)
        {
            if (ClearButtonClicked != null)
            {
                ClearButtonClicked();

                Status = OAuthLoginStatus.LoginRequired;
            }
        }
    }
}