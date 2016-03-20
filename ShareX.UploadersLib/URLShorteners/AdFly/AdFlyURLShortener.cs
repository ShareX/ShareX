using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners.AdFly
{
    public class AdFlyURLShortener : URLShortener
    {
        public string APIKEY { get; set; }
        public string APIUID { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("key", APIKEY);
            args.Add("uid", APIUID);
            args.Add("advert_type", "int");
            args.Add("domain", "adf.ly");
            args.Add("url", url);

            string response = SendRequest(HttpMethod.GET, "http://api.adf.ly/api.php", args);

            if (!string.IsNullOrEmpty(response) && response != "error")
            {
                result.ShortenedURL = response;
            }

            return result;
        }
    }
}