#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.Forms
{
    public partial class PuushLoginForm : Form
    {
        public string APIKey { get; set; }

        public PuushLoginForm()
        {
            InitializeComponent();
            Icon = Resources.puush;
        }

        private bool CheckValidation()
        {
            bool result = true;

            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                txtEmail.BackColor = Color.FromArgb(255, 200, 200);
                result = false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                txtPassword.BackColor = Color.FromArgb(255, 200, 200);
                result = false;
            }

            return result;
        }

        private void llCreateAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(Puush.PuushRegisterURL);
        }

        private void llForgottenPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(Puush.PuushResetPasswordURL);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (CheckValidation())
            {
                APIKey = new Puush().Login(txtEmail.Text, txtPassword.Text);

                if (!string.IsNullOrEmpty(APIKey))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Login failed.", "Authentication failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}