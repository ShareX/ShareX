#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UploadersLib.HelperClasses;
using UploadersLib.Properties;
using UploadersLib.SocialServices;

namespace UploadersLib
{
    public partial class TwitterMsg : Form
    {
        public string ActiveAccountName { get; set; }

        public string Message
        {
            get
            {
                return txtTweet.Text;
            }
            set
            {
                txtTweet.Text = value;
            }
        }

        public OAuthInfo AuthInfo { get; set; }
        public TwitterClientSettings Config { get; set; }

        public TwitterMsg(OAuthInfo oauth, string title)
        {
            InitializeComponent();
            AuthInfo = oauth;
            Text = title;
            Icon = Resources.Twitter;
            Config = new TwitterClientSettings();
        }

        public TwitterMsg(OAuthInfo oauth)
            : this(oauth, "Update Twitter status")
        {
        }

        public TwitterMsg(string title)
            : this(null, title)
        {
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTweet.Text))
            {
                DialogResult = DialogResult.OK;

                if (AuthInfo != null && !string.IsNullOrEmpty(Message))
                {
                    Hide();

                    try
                    {
                        TweetStatus status = new Twitter(AuthInfo).TweetMessage(Message);
                        if (status != null && !string.IsNullOrEmpty(status.in_reply_to_screen_name))
                        {
                            Config.AddUser(status.in_reply_to_screen_name);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Tweet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtTweet_TextChanged(object sender, EventArgs e)
        {
            lblCount.Text = (140 - txtTweet.Text.Length).ToString();
        }

        private void lbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbUsers.SelectedItem != null)
            {
                txtTweet.Text = txtTweet.Text.Insert(txtTweet.SelectionStart, "@" + lbUsers.SelectedItem + " ");
                txtTweet.SelectionStart = txtTweet.Text.Length;
                txtTweet.Focus();
            }
        }

        private void TwitterMsg_Load(object sender, EventArgs e)
        {
            foreach (string user in Config.Addressees)
            {
                lbUsers.Items.Add(user);
            }
        }

        private void lbUsers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                for (int i = lbUsers.Items.Count - 1; i >= 0; i--)
                {
                    lbUsers.SetSelected(i, true);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (lbUsers.SelectedIndex != -1)
                {
                    List<string> temp = lbUsers.SelectedItems.Cast<string>().ToList();

                    foreach (string hi in temp)
                    {
                        lbUsers.Items.Remove(hi);
                    }

                    if (lbUsers.Items.Count > 0)
                    {
                        lbUsers.SelectedIndex = 0;
                    }
                }
            }
        }

        private void TwitterMsg_Shown(object sender, EventArgs e)
        {
            txtTweet.Focus();
        }
    }

    public class TwitterClientSettings
    {
        public List<string> Addressees { get; set; }

        public TwitterClientSettings()
        {
            Addressees = new List<string>();
        }

        public void AddUser(string user)
        {
            if (!string.IsNullOrEmpty(user) && !Addressees.Contains(user))
            {
                Addressees.Add(user);
            }
        }
    }
}