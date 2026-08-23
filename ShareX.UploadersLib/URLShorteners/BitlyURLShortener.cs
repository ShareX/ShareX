#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace ShareX.UploadersLib.URLShorteners
{
    public class BitlyURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.BITLY;

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
    }

    public sealed class BitlyURLShortener : URLShortener, IOAuth2Basic
    {
        private const string URLAPI = "https://api-ssl.bitly.com/";
        private const string URLAccessToken = URLAPI + "oauth/access_token";
        private const string URLShorten = URLAPI + "v4/shorten";

        public OAuth2Info AuthInfo { get; private set; }
        public string Domain { get; set; }

        public BitlyURLShortener(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public Task<string> GetAuthorizationURLAsync(CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", Links.Callback);

            return Task.FromResult(URLHelpers.CreateQueryString("https://bitly.com/oauth/authorize", args));
        }

        public async Task<bool> GetAccessTokenAsync(string code, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("code", code);
            args.Add("redirect_uri", Links.Callback);

            string response = await SendRequestURLEncodedAsync(HttpMethod.POST, URLAccessToken, args,
                cancellationToken: cancellationToken).ConfigureAwait(false);

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

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        protected override async Task<UploadResult> ShortenURLCoreAsync(string url, CancellationToken cancellationToken)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                BitlyShortenRequestBody requestBody = new BitlyShortenRequestBody();
                requestBody.long_url = url;
                if (!string.IsNullOrEmpty(Domain)) requestBody.domain = Domain;
                string json = JsonConvert.SerializeObject(requestBody);

                NameValueCollection headers = GetAuthHeaders();

                result.Response = await SendRequestAsync(HttpMethod.POST, URLShorten, json, RequestHelpers.ContentTypeJSON, null, headers,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                BitlyShortenResponse responseData = JsonConvert.DeserializeObject<BitlyShortenResponse>(result.Response);

                if (responseData != null && !string.IsNullOrEmpty(responseData.link))
                {
                    result.ShortenedURL = responseData.link;
                }
            }

            return result;
        }

        private class BitlyShortenRequestBody
        {
            public string long_url { get; set; }
            public string domain { get; set; } = "bit.ly";
        }

        private class BitlyShortenResponse
        {
            public DateTime created_at { get; set; }
            public string id { get; set; }
            public string link { get; set; }
            public string long_url { get; set; }
        }
    }
}
