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

using HelpersLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using UploadersLib.HelperClasses;

namespace UploadersLib.URLShorteners
{
    public class GoogleURLShortener : URLShortener, IOAuth2
    {
        public AccountType UploadMethod { get; set; }
        public string AnonymousKey { get; set; }
        public OAuth2Info AuthInfo { get; set; }

        public GoogleURLShortener(AccountType uploadMethod, string anonymousKey, OAuth2Info oauth)
        {
            UploadMethod = uploadMethod;
            AnonymousKey = anonymousKey;
            AuthInfo = oauth;
        }

        public GoogleURLShortener(string anonymousKey)
        {
            UploadMethod = AccountType.Anonymous;
            AnonymousKey = anonymousKey;
        }

        public GoogleURLShortener(OAuth2Info oauth)
        {
            UploadMethod = AccountType.User;
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            return string.Format("https://accounts.google.com/o/oauth2/auth?response_type={0}&client_id={1}&redirect_uri={2}&scope={3}",
                "code", AuthInfo.Client_ID, "urn:ietf:wg:oauth:2.0:oob", Helpers.URLEncode("https://www.googleapis.com/auth/urlshortener"));
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", code);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
            args.Add("grant_type", "authorization_code");

            string response = SendRequest(HttpMethod.POST, "https://accounts.google.com/o/oauth2/token", args);

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
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("grant_type", "refresh_token");

                string response = SendRequest(HttpMethod.POST, "https://accounts.google.com/o/oauth2/token", args);

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

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                string query;

                switch (UploadMethod)
                {
                    default:
                    case AccountType.Anonymous:
                        query = string.Format("https://www.googleapis.com/urlshortener/v1/url?key={0}", AnonymousKey);
                        break;
                    case AccountType.User:
                        if (!CheckAuthorization())
                        {
                            return null;
                        }

                        query = string.Format("https://www.googleapis.com/urlshortener/v1/url?access_token={0}", AuthInfo.Token.access_token);
                        break;
                }

                string json = string.Format("{{\"longUrl\":\"{0}\"}}", url);

                result.Response = SendPostRequestJSON(query, json);

                if (!string.IsNullOrEmpty(result.Response))
                {
                    GoogleURLShortenerResponse response = JsonConvert.DeserializeObject<GoogleURLShortenerResponse>(result.Response);

                    if (response != null)
                    {
                        result.ShortenedURL = response.id;
                    }
                }
            }

            return result;
        }
    }

    public class GoogleURLShortenerResponse
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string longUrl { get; set; }
        public string status { get; set; }
    }
}