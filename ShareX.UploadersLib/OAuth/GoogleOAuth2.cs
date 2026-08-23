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

#nullable enable

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ShareX.UploadersLib
{
    public class GoogleOAuth2 : IOAuth2Loopback
    {
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

        public OAuth2Info AuthInfo { get; private set; }
        private Uploader GoogleUploader { get; set; }
        public string RedirectURI { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;

        public GoogleOAuth2(OAuth2Info oauth, Uploader uploader)
        {
            AuthInfo = oauth;
            GoogleUploader = uploader;
        }

        public Task<string> GetAuthorizationURLAsync(CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", RedirectURI);
            args.Add("state", State);
            args.Add("scope", Scope);

            return Task.FromResult(URLHelpers.CreateQueryString(AuthorizationEndpoint, args));
        }

        public async Task<bool> GetAccessTokenAsync(string code, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", code);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("redirect_uri", RedirectURI);
            args.Add("grant_type", "authorization_code");

            string response = await GoogleUploader.SendRequestURLEncodedAsync(HttpMethod.POST, TokenEndpoint, args,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token? token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> RefreshAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("grant_type", "refresh_token");

                string response = await GoogleUploader.SendRequestURLEncodedAsync(HttpMethod.POST, TokenEndpoint, args,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token? token = JsonConvert.DeserializeObject<OAuth2Token>(response);

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

        public async Task<bool> CheckAuthorizationAsync(CancellationToken cancellationToken = default)
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (AuthInfo.Token.IsExpired && !await RefreshAccessTokenAsync(cancellationToken).ConfigureAwait(false))
                {
                    GoogleUploader.Errors.Add(Localization.Strings.UploaderErrors_Refresh_access_token_failed);
                    return false;
                }
            }
            else
            {
                GoogleUploader.Errors.Add(Localization.Strings.UploaderErrors_Login_is_required);
                return false;
            }

            return true;
        }

        public NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        public async Task<OAuthUserInfo?> GetUserInfoAsync(CancellationToken cancellationToken = default)
        {
            string response = await GoogleUploader.SendRequestAsync(HttpMethod.GET, UserInfoEndpoint, null, GetAuthHeaders(),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<OAuthUserInfo>(response);
            }

            return null;
        }
    }
}
