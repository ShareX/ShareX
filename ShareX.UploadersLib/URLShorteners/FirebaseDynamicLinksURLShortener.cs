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

    public class FirebaseResponse
    {
        public string shortLink { get; set; }
        public string previewLink { get; set; }
    }

    public class FirebaseRequest
    {
        public string longDynamicLink { get; set; }
        public FirebaseRequestSuffix suffix { get; set; }
    }

    public class FirebaseRequestSuffix
    {
        public string option { get; set; }
    }

    public sealed class FirebaseDynamicLinksURLShortener : URLShortener
    {
        public string FirebaseWebAPIKey { get; set; }
        public string DynamicLinkDomain { get; set; }
        public bool IsShort { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            string RequestUrl = URLHelpers.ForcePrefix("firebasedynamiclinks.googleapis.com/v1/shortLinks?key=" + FirebaseWebAPIKey);
            string longDynamicLink = URLHelpers.ForcePrefix(DynamicLinkDomain + ".app.goo.gl/?link=" + url);
            string option;

            if (IsShort)
            {
                option = "SHORT";
            }
            else
            {
                option = "UNGUESSABLE";
            }

            FirebaseRequestSuffix suffix = new FirebaseRequestSuffix
            {
                option = option
            };

            FirebaseRequest request = new FirebaseRequest
            {
                longDynamicLink = longDynamicLink,
                suffix = suffix
            };

            string json = JsonConvert.SerializeObject(request);
            result.Response = SendRequest(HttpMethod.POST, RequestUrl, json, ContentTypeJSON);
            result.ShortenedURL = JsonConvert.DeserializeObject<FirebaseResponse>(result.Response).shortLink;

            return result;
        }
    }
}