#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UploadersLib.HelperClasses;
using UploadersLib.ImageUploaders;
using UploadersLib.Properties;

namespace UploadersLib
{
    public partial class TwitterTweetForm : Form
    {
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

        private int length;

        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
                UpdateLength();
            }
        }

        public bool IsValidMessage
        {
            get
            {
                return !string.IsNullOrEmpty(Message) && Message.Length <= Length;
            }
        }

        public OAuthInfo AuthInfo { get; set; }

        public TwitterTweetForm()
        {
            InitializeComponent();
            Icon = Resources.Twitter;
            Length = Twitter.MessageLimit;
        }

        public TwitterTweetForm(OAuthInfo oauth)
            : this()
        {
            AuthInfo = oauth;
        }

        private void UpdateLength()
        {
            lblTweetLength.Text = (Length - Message.Length).ToString();
            btnOK.Enabled = IsValidMessage;
        }

        private void TwitterMsg_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void txtTweet_TextChanged(object sender, EventArgs e)
        {
            UpdateLength();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsValidMessage)
            {
                DialogResult = DialogResult.OK;
                SendTweet();
                Close();
            }
        }

        private void SendTweet()
        {
            if (AuthInfo != null)
            {
                Hide();

                try
                {
                    new Twitter(AuthInfo).TweetMessage(Message);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    MessageBox.Show(ex.ToString(), "ShareX - Tweet error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}