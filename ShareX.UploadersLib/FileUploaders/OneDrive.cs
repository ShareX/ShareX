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

// Credits: https://github.com/l0nley

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UploadersLib.HelperClasses;

namespace UploadersLib.FileUploaders
{
    public sealed class OneDrive : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; private set; }

        public string FolderID { get; set; }

        public OneDrive(OAuth2Info authInfo)
        {
            AuthInfo = authInfo;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            if (string.IsNullOrEmpty(FolderID))
            {
                FolderID = "me/skydrive/files";
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("access_token", AuthInfo.Token.access_token);
            args.Add("overwrite", "true");

            string url = string.Format("https://apis.live.net/v5.0/{0}/{1}", FolderID, fileName);
            UploadResult result = UploadData(stream, url, string.Empty, arguments: args, method: HttpMethod.PUT);

            if (result.IsSuccess)
            {
                OneDriveUploadInfo resultJson = JsonConvert.DeserializeObject<OneDriveUploadInfo>(result.Response);
                result.URL = resultJson.source;
            }

            return result;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("scope", "wl.skydrive_update");

            return CreateQuery("https://login.live.com/oauth20_authorize.srf", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("grant_type", "authorization_code");
            args.Add("code", code);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);

            string response = SendRequest(HttpMethod.POST, "https://login.live.com/oauth20_token.srf", args);

            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

            if (token == null || string.IsNullOrEmpty(token.access_token))
            {
                return false;
            }

            token.UpdateExpireDate();
            AuthInfo.Token = token;
            return true;
        }

        public bool RefreshAccessToken()
        {
            if (!OAuth2Info.CheckOAuth(AuthInfo) || string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                return false;
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("grant_type", "refresh_token");
            args.Add("refresh_token", AuthInfo.Token.refresh_token);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);

            string response = SendRequest(HttpMethod.POST, "https://login.live.com/oauth20_token.srf", args);

            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

            if (token == null || string.IsNullOrEmpty(token.access_token))
            {
                return false;
            }

            token.UpdateExpireDate();
            AuthInfo.Token = token;
            return true;
        }

        public bool CheckAuthorization()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (!AuthInfo.Token.IsExpired || RefreshAccessToken())
                {
                    return true;
                }

                Errors.Add("Refresh access token failed.");
                return false;
            }

            Errors.Add("OneDrive login is required.");
            return false;
        }
    }

    internal sealed class OneDriveUploadInfo
    {
        public string id { get; set; }
        public string source { get; set; }
    }
}