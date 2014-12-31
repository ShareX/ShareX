#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Web;

namespace ShareX.UploadersLib.URLShorteners
{
    public sealed class BitlyURLShortener : URLShortener, IOAuth2Basic
    {
        private const string URLAPI = "https://api-ssl.bitly.com/";
        private const string URLAccessToken = URLAPI + "oauth/access_token";
        private const string URLShorten = URLAPI + "v3/shorten";

        public OAuth2Info AuthInfo { get; private set; }
        public string Domain { get; set; }

        public BitlyURLShortener(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", Links.URL_CALLBACK);

            return CreateQuery("https://bitly.com/oauth/authorize", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("code", code);
            args.Add("redirect_uri", Links.URL_CALLBACK);

            string response = SendRequest(HttpMethod.POST, URLAccessToken, args);

            if (!string.IsNullOrEmpty(response))
            {
                string token = HttpUtility.ParseQueryString(response)["access_token"];

                if (!string.IsNullOrEmpty(token))
                {
                    AuthInfo.Token = new OAuth2Token { access_token = token };
                    return true;
                }
            }

            return false;
        }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("access_token", AuthInfo.Token.access_token);
                arguments.Add("longUrl", url);
                if (!string.IsNullOrEmpty(Domain)) arguments.Add("domain", Domain);

                result.Response = SendRequest(HttpMethod.GET, URLShorten, arguments);

                BitlyShortenResponse shorten = JsonConvert.DeserializeObject<BitlyShortenResponse>(result.Response);

                if (shorten != null && shorten.data != null && !string.IsNullOrEmpty(shorten.data.url))
                {
                    result.ShortenedURL = shorten.data.url;
                }
            }

            return result;
        }

        public class BitlyShortenData
        {
            public string global_hash { get; set; }
            public string hash { get; set; }
            public string long_url { get; set; }
            public int new_hash { get; set; }
            public string url { get; set; }
        }

        public class BitlyShortenResponse
        {
            public BitlyShortenData data { get; set; }
            public int status_code { get; set; }
            public string status_txt { get; set; }
        }
    }
}