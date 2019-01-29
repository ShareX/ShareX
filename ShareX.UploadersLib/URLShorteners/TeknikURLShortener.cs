using Newtonsoft.Json;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class TeknikUrlShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.Teknik;

        public override Icon ServiceIcon => Resources.Teknik;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.TeknikUrlShortenerAPIUrl) && !string.IsNullOrEmpty(config.TeknikAuthUrl);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TeknikUrlShortener(config.TeknikOAuth2Info, config.TeknikAuthUrl)
            {
                APIUrl = config.TeknikUrlShortenerAPIUrl
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGist;
    }

    public sealed class TeknikUrlShortener : URLShortener, IOAuth2Basic
    {
        private Teknik _Teknik;

        public OAuth2Info AuthInfo { get; set; }

        public string APIUrl { get; set; }

        public TeknikUrlShortener(OAuth2Info oauth, string authUrl)
        {
            _Teknik = new Teknik(oauth, authUrl);
            AuthInfo = _Teknik.AuthInfo;
        }

        public bool GetAccessToken(string code)
        {
            return _Teknik.GetAccessToken(code);
        }

        public string GetAuthorizationURL()
        {
            return _Teknik.GetAuthorizationURL();
        }

        public override UploadResult ShortenURL(string url)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("url", url);

            string response = SendRequestMultiPart(APIUrl, args, _Teknik.GetAuthHeaders(), null, HttpMethod.POST);
            TeknikUrlShortenerResponseWrapper apiResponse = JsonConvert.DeserializeObject<TeknikUrlShortenerResponseWrapper>(response);

            UploadResult ur = new UploadResult();
            if (apiResponse.Result != null && apiResponse.Error == null)
            {
                ur.ShortenedURL = apiResponse.Result.shortUrl;
            }

            return ur;
        }
    }

    public class TeknikUrlShortenerResponseWrapper
    {
        public TeknikUrlShortenerResponse Result { get; set; }
        public TeknikErrorResponse Error { get; set; }
    }

    public class TeknikUrlShortenerResponse
    {
        public string shortUrl { get; set; }
        public string originalUrl { get; set; }
    }
}
