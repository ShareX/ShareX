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
using ShareX.UploadersLib.ImageUploaders;
using System;
using System.Windows.Forms;

namespace ShareX.UploadersLib
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
                return Message != null && (MediaMode || Message.Length > 0) && Message.Length <= Length;
            }
        }

        private bool mediaMode;

        public bool MediaMode
        {
            get
            {
                return mediaMode;
            }
            set
            {
                mediaMode = value;

                if (mediaMode)
                {
                    Length = Twitter.MessageMediaLimit;
                }
                else
                {
                    Length = Twitter.MessageLimit;
                }
            }
        }

        public bool IsTweetSent { get; private set; }

        public OAuthInfo AuthInfo { get; set; }

        public TwitterTweetForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            MediaMode = false;
        }

        public TwitterTweetForm(OAuthInfo oauth) : this()
        {
            AuthInfo = oauth;
        }

        public TwitterTweetForm(OAuthInfo oauth, string message) : this(oauth)
        {
            Message = message;
        }

        private void UpdateLength()
        {
            lblTweetLength.Text = (Length - Message.Length).ToString();
            btnOK.Enabled = IsValidMessage;
        }

        private void OK()
        {
            if (IsValidMessage)
            {
                SendTweet();

                DialogResult = DialogResult.OK;
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
                    TwitterStatusResponse status = new Twitter(AuthInfo).TweetMessage(Message);
                    IsTweetSent = status != null;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
            }
        }

        private void TwitterMsg_Shown(object sender, EventArgs e)
        {
            txtTweet.Select(txtTweet.TextLength, 0);
            this.ForceActivate();
        }

        private void txtTweet_TextChanged(object sender, EventArgs e)
        {
            UpdateLength();
        }

        private void txtTweet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Enter))
            {
                OK();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OK();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}