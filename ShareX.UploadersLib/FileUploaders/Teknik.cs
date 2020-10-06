#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class TeknikFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Teknik;

        public override Icon ServiceIcon => Resources.Teknik;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.TeknikUploadAPIUrl) &&
                   !string.IsNullOrEmpty(config.TeknikAuthUrl) &&
                   OAuth2Info.CheckOAuth(config.TeknikOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TeknikUploader(config.TeknikOAuth2Info, config.TeknikAuthUrl)
            {
                APIUrl = config.TeknikUploadAPIUrl,
                ExpirationUnit = config.TeknikExpirationUnit,
                ExpirationLength = config.TeknikExpirationLength,
                Encryption = config.TeknikEncryption,
                GenerateDeletionKey = config.TeknikGenerateDeletionKey
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpTeknik;
    }

    public enum TeknikExpirationUnit
    {
        Never,
        Views,
        Minutes,
        Hours,
        Days,
        Months,
        Years
    }

    public class Teknik : Uploader, IOAuth2
    {
        public const string DefaultUploadAPIURL = "https://api.teknik.io/v1/Upload";
        public const string DefaultPasteAPIURL = "https://api.teknik.io/v1/Paste";
        public const string DefaultUrlShortenerAPIURL = "https://api.teknik.io/v1/Shorten";
        public const string DefaultAuthURL = "https://auth.teknik.io";

        public OAuth2Info AuthInfo { get; set; }
        public string AuthUrl { get; set; }

        public Teknik(OAuth2Info oauth, string authUrl)
        {
            AuthInfo = oauth;
            AuthUrl = authUrl;
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

                string response = SendRequestMultiPart(AuthUrl + "/connect/token", args);

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
                Errors.Add("Teknik login is required.");
                return false;
            }

            return true;
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "authorization_code");
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("code", code);

            string response = SendRequestMultiPart(AuthUrl + "/connect/token", args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("scope", "openid teknik-api.write offline_access");
            args.Add("client_id", AuthInfo.Client_ID);

            return URLHelpers.CreateQueryString(AuthUrl + "/connect/authorize", args);
        }

        public NameValueCollection GetAuthHeaders()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                NameValueCollection headers = new NameValueCollection();
                headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
                return headers;
            }
            return null;
        }
    }

    public sealed class TeknikUploader : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public string APIUrl { get; set; }
        public TeknikExpirationUnit ExpirationUnit { get; set; }
        public int ExpirationLength { get; set; }
        public bool Encryption { get; set; }
        public bool GenerateDeletionKey { get; set; }

        private Teknik teknik;

        public TeknikUploader(OAuth2Info oauth, string authUrl)
        {
            teknik = new Teknik(oauth, authUrl);
            AuthInfo = teknik.AuthInfo;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("encrypt", (!Encryption).ToString());
            args.Add("expirationUnit", ExpirationUnit.ToString());
            args.Add("expirationLength", ExpirationLength.ToString());
            args.Add("saveKey", (!Encryption).ToString());
            args.Add("keySize", "256");
            args.Add("blockSize", "128");
            args.Add("genDeletionKey", GenerateDeletionKey.ToString());

            UploadResult result = SendRequestFile(APIUrl, stream, fileName, "file", args, teknik.GetAuthHeaders());

            if (result.IsSuccess)
            {
                TeknikUploadResponseWrapper response = JsonConvert.DeserializeObject<TeknikUploadResponseWrapper>(result.Response);

                if (response.Result != null && response.Error == null)
                {
                    result.URL = response.Result.Url;
                }
            }

            return result;
        }

        public string GetAuthorizationURL()
        {
            return teknik.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return teknik.GetAccessToken(code);
        }

        public bool RefreshAccessToken()
        {
            return teknik.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return teknik.CheckAuthorization();
        }
    }

    public class TeknikErrorResponse
    {
        public string Message { get; set; }
    }

    public class TeknikUploadResponseWrapper
    {
        public TeknikUploadResponse Result { get; set; }
        public TeknikErrorResponse Error { get; set; }
    }

    public class TeknikUploadResponse
    {
        public string Url { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string Key { get; set; }
        public int KeySize { get; set; }
        public string IV { get; set; }
        public int BlockSize { get; set; }
        public string DeletionKey { get; set; }
    }
}