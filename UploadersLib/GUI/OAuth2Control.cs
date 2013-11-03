#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

namespace UploadersLib.GUI
{
    public partial class OAuth2Control : UserControl
    {
        public delegate void OpenButtonClickedEventHandler();

        public event OpenButtonClickedEventHandler OpenButtonClicked;

        public delegate void CompleteButtonClickedEventHandler(string code);

        public event CompleteButtonClickedEventHandler CompleteButtonClicked;

        public delegate void RefreshButtonClickedEventHandler();

        public event RefreshButtonClickedEventHandler RefreshButtonClicked;

        public string Status
        {
            get
            {
                return lblLoginStatus.Text;
            }
            set
            {
                lblLoginStatus.Text = value;
            }
        }

        private bool loginStatus;

        public bool LoginStatus
        {
            get
            {
                return loginStatus;
            }
            set
            {
                loginStatus = value;
                btnRefreshAuthorization.Enabled = loginStatus;
            }
        }

        public OAuth2Control()
        {
            InitializeComponent();
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
    }
}