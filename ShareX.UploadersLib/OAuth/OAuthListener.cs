#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public class OAuthListener : IDisposable
    {
        public IOAuth2Loopback OAuth { get; private set; }
        public string Code { get; private set; }

        private HttpListener listener;

        public OAuthListener(IOAuth2Loopback oauth)
        {
            OAuth = oauth;
        }

        public void Dispose()
        {
            listener?.Close();
            listener = null;
        }

        public async Task<bool> ConnectAsync()
        {
            Dispose();
            Code = null;

            IPAddress ip = IPAddress.Loopback;
            int port = URLHelpers.GetRandomUnusedPort();
            string redirectURI = string.Format($"http://{ip}:{port}/");

            OAuth.RedirectURI = redirectURI;
            string url = OAuth.GetAuthorizationURL();

            if (!string.IsNullOrEmpty(url))
            {
                URLHelpers.OpenURL(url);
                DebugHelper.WriteLine("Authorization URL is opened: " + url);
            }
            else
            {
                DebugHelper.WriteLine("Authorization URL is empty.");
                return false;
            }

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(redirectURI);
                listener.Start();

                HttpListenerContext context = await listener.GetContextAsync();
                Code = context.Request.QueryString.Get("code");

                using (HttpListenerResponse response = context.Response)
                {
                    string status;

                    if (!string.IsNullOrEmpty(Code))
                    {
                        status = "Authorization completed successfully.";
                    }
                    else
                    {
                        status = "Authorization did not succeed.";
                    }

                    string responseText = Resources.OAuthCallbackPage.Replace("{0}", status);
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    response.ContentLength64 = buffer.Length;
                    response.KeepAlive = false;

                    using (Stream responseOutput = response.OutputStream)
                    {
                        await responseOutput.WriteAsync(buffer, 0, buffer.Length);
                        await responseOutput.FlushAsync();
                    }
                }
            }
            finally
            {
                Dispose();
            }

            if (!string.IsNullOrEmpty(Code))
            {
                return await Task.Run(() => OAuth.GetAccessToken(Code));
            }

            return false;
        }
    }
}