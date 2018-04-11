#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class GoogleURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.Google;

        public override Icon ServiceIcon => Resources.Google;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.GoogleURLShortenerAccountType == AccountType.Anonymous || OAuth2Info.CheckOAuth(config.GoogleURLShortenerOAuth2Info);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GoogleURLShortener(config.GoogleURLShortenerAccountType, APIKeys.GoogleAPIKey, config.GoogleURLShortenerOAuth2Info);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGoogleURLShortener;
    }

    public class GoogleURLShortener : URLShortener, IOAuth2
    {
        private GoogleOAuth2 GoogleAuth { get; set; }
        public AccountType UploadMethod { get; set; }
        public string AnonymousKey { get; set; }

        public GoogleURLShortener(AccountType uploadMethod, string anonymousKey, OAuth2Info oauth)
        {
            UploadMethod = uploadMethod;
            AnonymousKey = anonymousKey;
            GoogleAuth = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/urlshortener"
            };
        }

        public GoogleURLShortener(string anonymousKey) : this(AccountType.Anonymous, anonymousKey, null)
        {
        }

        public GoogleURLShortener(OAuth2Info oauth) : this(AccountType.User, null, oauth)
        {
        }

        public OAuth2Info AuthInfo => GoogleAuth.AuthInfo;

        public bool RefreshAccessToken()
        {
            return GoogleAuth.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return GoogleAuth.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return GoogleAuth.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return GoogleAuth.GetAccessToken(code);
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

                string json = JsonConvert.SerializeObject(new { longUrl = url });

                result.Response = SendRequest(HttpMethod.POST, query, json, ContentTypeJSON);

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