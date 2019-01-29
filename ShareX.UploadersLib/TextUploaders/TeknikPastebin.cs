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

namespace ShareX.UploadersLib.TextUploaders
{
    public class TeknikTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Teknik;

        public override Icon ServiceIcon => Resources.Teknik;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.TeknikPasteAPIUrl) && !string.IsNullOrEmpty(config.TeknikAuthUrl);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TeknikPaster(config.TeknikOAuth2Info, config.TeknikAuthUrl)
            {
                APIUrl = config.TeknikPasteAPIUrl
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGist;
    }

    public sealed class TeknikPaster : TextUploader, IOAuth2Basic
    {
        private Teknik _Teknik;

        public OAuth2Info AuthInfo { get; set; }

        public string APIUrl { get; set; }

        public TeknikPaster(OAuth2Info oauth, string authUrl)
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

        public override UploadResult UploadText(string text, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", text);

            string response = SendRequestMultiPart(APIUrl, args, _Teknik.GetAuthHeaders(), null, HttpMethod.POST);
            TeknikPasteResponseWrapper apiResponse = JsonConvert.DeserializeObject<TeknikPasteResponseWrapper>(response);

            UploadResult ur = new UploadResult();
            if (apiResponse.Result != null && apiResponse.Error == null)
            {
                ur.URL = apiResponse.Result.Url;
            }

            return ur;
        }
    }

    public class TeknikPasteResponseWrapper
    {
        public TeknikPasteResponse Result { get; set; }
        public TeknikErrorResponse Error { get; set; }
    }

    public class TeknikPasteResponse
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Syntax { get; set; }
        public string Password { get; set; }
    }
}
