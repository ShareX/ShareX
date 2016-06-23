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
using System;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class OAuthWebForm : Form
    {
        public string AuthorizeURL { get; set; }
        public string CallbackURL { get; set; }
        public string Code { get; set; }

        public OAuthWebForm(string authorizeURL, string callbackURL)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            AuthorizeURL = authorizeURL;
            CallbackURL = callbackURL;
            tstbURL.Text = authorizeURL;
            wbMain.Navigate(AuthorizeURL);
        }

        private void wbMain_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (!IsDisposed)
            {
                tstbURL.Text = e.Url.ToString();
                CheckCallback(e.Url.ToString());
            }
        }

        private void CheckCallback(string url)
        {
            if (url.StartsWith(CallbackURL, StringComparison.InvariantCultureIgnoreCase))
            {
                NameValueCollection args = HttpUtility.ParseQueryString(url);

                if (args != null)
                {
                    Code = args["code"];

                    if (!string.IsNullOrEmpty(Code))
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
        }
    }
}