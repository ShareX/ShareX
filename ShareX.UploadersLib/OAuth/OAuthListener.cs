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

        private HttpListener listener;

        public OAuthListener(IOAuth2Loopback oauth)
        {
            OAuth = oauth;
        }

        public void Dispose()
        {
            if (listener != null)
            {
                listener.Close();
                listener = null;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            Dispose();

            IPAddress ip = IPAddress.Loopback;
            int port = WebHelpers.GetRandomUnusedPort();
            string redirectURI = string.Format($"http://{ip}:{port}/");
            string state = Helpers.GetRandomAlphanumeric(32);

            OAuth.RedirectURI = redirectURI;
            OAuth.State = state;
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

            string queryCode = null;
            string queryState = null;

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(redirectURI);
                listener.Start();

                HttpListenerContext context = await listener.GetContextAsync();
                queryCode = context.Request.QueryString.Get("code");
                queryState = context.Request.QueryString.Get("state");

                using (HttpListenerResponse response = context.Response)
                {
                    string status;

                    if (queryState != state)
                    {
                        status = "Invalid state parameter.";
                    }
                    else if (!string.IsNullOrEmpty(queryCode))
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
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                Dispose();
            }

            if (queryState == state && !string.IsNullOrEmpty(queryCode))
            {
                return await Task.Run(() => OAuth.GetAccessToken(queryCode));
            }

            return false;
        }
    }
}