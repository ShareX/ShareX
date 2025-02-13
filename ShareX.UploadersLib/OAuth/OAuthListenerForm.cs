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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class OAuthListenerForm : Form
    {
        public IOAuth2Loopback OAuth { get; private set; }
        public OAuth2Info OAuth2Info { get; private set; }
        public OAuthUserInfo UserInfo { get; private set; }

        private OAuthListener listener;

        public OAuthListenerForm(IOAuth2Loopback oauth)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            OAuth = oauth;
        }

        private async void OAuthListenerForm_Load(object sender, EventArgs e)
        {
            await ConnectAsync(OAuth);

            Close();
        }

        private void OAuthListenerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            listener?.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async Task<bool> ConnectAsync(IOAuth2Loopback oauth)
        {
            OAuth2Info = null;
            UserInfo = null;

            try
            {
                using (listener = new OAuthListener(oauth))
                {
                    bool result = await listener.ConnectAsync();

                    if (result)
                    {
                        OAuth2Info = listener.OAuth.AuthInfo;
                        UserInfo = await Task.Run(() => oauth.GetUserInfo());

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
            }

            return false;
        }
    }
}