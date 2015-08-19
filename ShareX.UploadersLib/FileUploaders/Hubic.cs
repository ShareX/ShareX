#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class Hubic : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public HubicOpenstackAuthInfo HubicOpenstackAuthInfo { get; set; }
        public HubicFolderInfo SelectedFolder { get; set; }
        public bool Publish { get; set; }
        public static HubicFolderInfo RootFolder = new HubicFolderInfo
        {
            name = ""
        };
        public const string Scope = @"usage.r,account.r,getAllLinks.r,credentials.r,sponsorCode.r,activate.w,sponsored.r,links.drw";
        public const string URLApi = @"https://api.hubic.com/1.0/account/credentials/";

        public Hubic(OAuth2Info oauth, HubicOpenstackAuthInfo osauth)
        {
            AuthInfo = oauth;
            HubicOpenstackAuthInfo = osauth;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            //Hubic only accepts https callback URL
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("scope", Scope);
            args.Add("response_type", "code");

            return CreateQuery("https://api.hubic.com/oauth/auth/", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", code);
            //Hubic only accepts https callback URL
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("grant_type", "authorization_code");

            string response = SendRequest(HttpMethod.POST, "https://api.hubic.com/oauth/token/", args, GetAuthHeaders("Basic"));

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    GetHubicOpenStackAuthInfo();
                    return true;
                }
            }

            return false;
        }

        private NameValueCollection GetAuthHeaders(string headerType)
        {
            NameValueCollection headers = new NameValueCollection();
            switch (headerType)
            {
                case "Basic":
                    string secretInBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthInfo.Client_ID + ":" + AuthInfo.Client_Secret));
                    headers["Authorization"] = headerType + " " + secretInBase64;
                    break;
                case "Bearer":
                    headers["Authorization"] = headerType + " " + AuthInfo.Token.access_token;
                    break;
                case "X-Auth-Token":
                    headers["X-Auth-Token"] = " " + HubicOpenstackAuthInfo.token;
                    headers["X-Detect-Content-Type"] = " " + "true";
                    break;
            }
            return headers;
        }

        public bool RefreshAccessToken()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("grant_type", "refresh_token");
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);

                string response = SendRequest(HttpMethod.POST, "https://api.hubic.com/oauth/token", args);

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
                Errors.Add("Hubic login is required.");
                return false;
            }

            return true;
        }

        public void GetHubicOpenStackAuthInfo()
        {
            string response = SendRequest(HttpMethod.GET, URLApi, headers: GetAuthHeaders("Bearer"));
            if (!string.IsNullOrEmpty(response))
            {
                HubicOpenstackAuthInfo resp = JsonConvert.DeserializeObject<HubicOpenstackAuthInfo>(response);
                HubicOpenstackAuthInfo.endpoint = resp.endpoint;
                HubicOpenstackAuthInfo.expires = resp.expires;
                HubicOpenstackAuthInfo.token = resp.token;
            }
        }

        public List<HubicFolderInfo> GetFiles(HubicFolderInfo fileInfo)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            string response = SendRequest(HttpMethod.GET, HubicOpenstackAuthInfo.endpoint + "/default" + "/?path=" + fileInfo.name + "&format=json", headers: GetAuthHeaders("X-Auth-Token"));

            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<List<HubicFolderInfo>>(response);
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            string url = URLHelpers.CombineURL(HubicOpenstackAuthInfo.endpoint, "default", SelectedFolder.path, fileName);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("X-Auth-Token", HubicOpenstackAuthInfo.token);
            headers.Add("X-Detect-Content-Type", "true");

            UploadResult result = UploadData(stream, url, fileName, headers: headers, method: HttpMethod.PUT);

            if (Publish)
            {
                AllowReportProgress = false;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("comment", fileName);
                args.Add("container", "default");
                args.Add("mode", "ro");
                args.Add("ttl", "30");
                args.Add("type", "file");
                args.Add("uri", "/" + SelectedFolder.path + "/" + fileName);

                string response = SendRequest(HttpMethod.POST, "https://api.hubic.com/1.0/account/links", args, GetAuthHeaders("Bearer"));

                if (!string.IsNullOrEmpty(response))
                {
                    HubicPublishURLResponse resp = JsonConvert.DeserializeObject<HubicPublishURLResponse>(response);
                    string respURL = resp.indirectUrl;
                    result.IsURLExpected = true;
                    result.URL = respURL;
                }
            }

            return result;
        }
    }

    public class HubicOpenstackAuthInfo
    {
        public string token { get; set; }
        public string endpoint { get; set; }
        public string expires { get; set; }
    }

    public class HubicFolderInfo
    {
        private string _name;
        public string hash { get; set; }
        public string last_modified { get; set; }
        public int bytes { get; set; }
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                //split the folder path and jsut store the folder name
                path = value;
                string[] temp = value.Split('/');
                _name = temp[temp.Length - 1];
            }
        }
        public string content_type { get; set; }
        public string path { get; set; }
    }

    public class HubicPublishURLResponse
    {
        public string indirectUrl { get; set; }
        public string mode { get; set; }
        public string container { get; set; }
        public string expirationDate { get; set; }
        public string creationDate { get; set; }
        public string comment { get; set; }
        public string type { get; set; }
    }
}