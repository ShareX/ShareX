#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class BitlyURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.BITLY;

        public override Icon ServiceIcon => Resources.Bitly;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.BitlyOAuth2Info);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            if (config.BitlyOAuth2Info == null)
            {
                config.BitlyOAuth2Info = new OAuth2Info(APIKeys.BitlyClientID, APIKeys.BitlyClientSecret);
            }

            return new BitlyURLShortener(config.BitlyOAuth2Info)
            {
                Domain = config.BitlyDomain
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpBitly;
    }

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

            string response = SendRequestMultiPart(URLAccessToken, args);

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