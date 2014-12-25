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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.HelperClasses;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class OneDrive : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public string FolderID { get; set; }

        public OneDrive(OAuth2Info authInfo)
        {
            AuthInfo = authInfo;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("scope", "wl.offline_access wl.basic wl.skydrive");
            args.Add("response_type", "code");
            args.Add("redirect_uri", Links.URL_CALLBACK);

            return CreateQuery("https://login.live.com/oauth20_authorize.srf", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("code", code);
            args.Add("grant_type", "authorization_code");

            string response = SendRequestURLEncoded("https://login.live.com/oauth20_token.srf", args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public bool RefreshAccessToken()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("redirect_uri", Links.URL_CALLBACK);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("grant_type", "refresh_token");

                string response = SendRequestURLEncoded("https://login.live.com/oauth20_token.srf", args);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                    if (token != null && !string.IsNullOrEmpty(token.access_token))
                    {
                        token.UpdateExpireDate();
                        string refresh_token = AuthInfo.Token.refresh_token;
                        AuthInfo.Token = token;
                        AuthInfo.Token.refresh_token = refresh_token;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckAuthorization()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (AuthInfo.Token.IsExpired && !RefreshAccessToken())
                {
                    Errors.Add("Refresh access token failed.");
                    return false;
                }
            }
            else
            {
                Errors.Add("Login is required.");
                return false;
            }

            return true;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("access_token", AuthInfo.Token.access_token);
            args.Add("overwrite", "true");
            args.Add("downsize_photo_uploads", "false");

            string url = CreateQuery("https://apis.live.net/v5.0/me/skydrive/files", args);

            UploadResult result = UploadData(stream, url, fileName);

            if (result.IsSuccess)
            {
                OneDriveUploadInfo uploadInfo = JsonConvert.DeserializeObject<OneDriveUploadInfo>(result.Response);
                result.URL = uploadInfo.source;
            }

            return result;
        }

        private class OneDriveUploadInfo
        {
            public string id { get; set; }
            public string name { get; set; }
            public string source { get; set; }
        }
    }
}