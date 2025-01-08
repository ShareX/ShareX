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
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class UserPassBox : Form
    {
        public string FullName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public bool Success { get; set; }

        public UserPassBox(string title, string userName, string password)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Text = title;
            txtUserName.Text = userName;
            txtPassword.Text = password;
        }

        public UserPassBox(string title, string fullName, string userName, string password) : this(title, userName, password)
        {
            txtFullName.Text = fullName;
            txtFullName.Enabled = true;
        }

        public UserPassBox(string title, string fullName, string email, string userName, string password) : this(title, fullName, userName, password)
        {
            txtEmail.Text = email;
            txtEmail.Enabled = true;
        }

        private void InputBox_Shown(object sender, EventArgs e)
        {
            txtUserName.Focus();
            txtUserName.SelectionLength = txtUserName.Text.Length;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUserName.Text))
            {
                UserName = txtUserName.Text;
                Password = txtPassword.Text;
                Email = txtEmail.Text;
                FullName = txtFullName.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}