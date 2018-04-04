/* https://github.com/matthewburnett */

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class FirebaseDynamicLinksURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.FirebaseDynamicLinks;

        public override Icon ServiceIcon => Resources.Firebase;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new FirebaseDynamicLinksURLShortener
            {
                FirebaseWebAPIKey = config.FirebaseWebAPIKey,
                DynamicLinkDomain = config.FirebaseDynamicLinkDomain,
                IsShort = config.FirebaseIsShort
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpFirebaseDynamicLinks;
    }

    public class FirebaseDynamicLinksURLShortenerServiceResponse
    {
        public string shortLink { get; set; }
        public string previewLink { get; set; }
    }

    public sealed class FirebaseDynamicLinksURLShortener : URLShortener
    {
        public string FirebaseWebAPIKey { get; set; }
        public string DynamicLinkDomain { get; set; }
        public bool IsShort { get; set; }
        private string option;
        private string RequestUrl = "https://firebasedynamiclinks.googleapis.com/v1/shortLinks?key=";

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            RequestUrl = RequestUrl + FirebaseWebAPIKey;
            string longDynamicLink = BrowserProtocol.https + DynamicLinkDomain + ".app.goo.gl/?link=" + url;

            if (IsShort)
            {
                option = "SHORT";
            }
            else
            {
                option = "UNGUESSABLE";
            }

            var FDLObject = new
            {
                longDynamicLink,

                suffix = new
                {
                    option
                }
            };

            string json = JsonConvert.SerializeObject(FDLObject);
            string response = SendRequest(HttpMethod.POST, RequestUrl, json, ContentTypeJSON);
            result.ShortenedURL = JsonConvert.DeserializeObject<FirebaseDynamicLinksURLShortenerServiceResponse>(response).shortLink;

            return result;
        }
    }
}