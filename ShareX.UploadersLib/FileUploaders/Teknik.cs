using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class TeknikFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Teknik;

        public override Icon ServiceIcon => Resources.Teknik;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.TeknikOAuth2Info) && !string.IsNullOrEmpty(config.TeknikAPIUrl) && !string.IsNullOrEmpty(config.TeknikAuthUrl);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TeknikUploader(config.TeknikOAuth2Info, config.TeknikAuthUrl)
            {
                APIUrl = config.TeknikAPIUrl,
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
        Never = 0,
        Views = 1,
        Minutes = 10,
        Hours = 11,
        Days = 12,
        Months = 13,
        Years = 14
    }

    public sealed class TeknikUploader : FileUploader, IOAuth2Basic
    {
        public const string DefaultAPIURL = "https://api.teknik.io/v1/Upload";
        public const string DefaultAuthURL = "https://auth.teknik.io";

        public OAuth2Info AuthInfo { get; set; }
        public string AuthUrl { get; set; }

        public string APIUrl { get; set; }
        public TeknikExpirationUnit ExpirationUnit { get; set; }
        public int ExpirationLength { get; set; }

        public bool Encryption { get; set; }
        public bool GenerateDeletionKey { get; set; }

        public TeknikUploader(OAuth2Info oauth, string authUrl)
        {
            AuthInfo = oauth;
            AuthUrl = authUrl;
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
            args.Add("scope", "openid role account-info teknik-api.read teknik-api.write");
            args.Add("client_id", AuthInfo.Client_ID);

            return URLHelpers.CreateQueryString(AuthUrl + "/connect/authorize", args);
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("encrypt", (!Encryption).ToString());
            args.Add("expirationUnit", ExpirationUnit.ToString());
            args.Add("expirationLength", ExpirationLength.ToString());
            args.Add("saveKey", (!Encryption).ToString());
            args.Add("keySize", "256");
            args.Add("blockSize", "128");
            args.Add("genDeletionKey", GenerateDeletionKey.ToString());

            UploadResult result = SendRequestFile(APIUrl, stream, fileName, "file", args, GetAuthHeaders());

            if (result.IsSuccess)
            {
                TeknikResponse response = JsonConvert.DeserializeObject<TeknikResponse>(result.Response);

                if (response.Result != null && response.Error == null)
                {
                    result.URL = response.Result.Url;
                }
            }

            return result;
        }
    }

    public class TeknikResponse
    {
        public TeknikUploadResponse Result { get; set; }
        public TeknikErrorResponse Error { get; set; }
    }

    public class TeknikErrorResponse
    {
        public string Message { get; set; }
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
