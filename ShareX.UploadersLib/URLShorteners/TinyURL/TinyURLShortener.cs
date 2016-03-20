using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners.TinyURL
{
    public sealed class TinyURLShortener : URLShortener
    {
        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("url", url);

                result.Response = result.ShortenedURL = SendRequest(HttpMethod.GET, "http://tinyurl.com/api-create.php", arguments);
            }

            return result;
        }
    }
}