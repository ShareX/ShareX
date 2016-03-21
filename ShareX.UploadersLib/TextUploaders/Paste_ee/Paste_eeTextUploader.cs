using System.Collections.Generic;

namespace ShareX.UploadersLib.TextUploaders.Paste_ee
{
    internal sealed class Paste_eeTextUploader : TextUploader
    {
        public string APIKey { get; private set; }

        public Paste_eeTextUploader()
        {
            APIKey = "public";
        }

        public Paste_eeTextUploader(string apiKey)
        {
            APIKey = apiKey;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                if (string.IsNullOrEmpty(APIKey))
                {
                    APIKey = "public";
                }

                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("key", APIKey);
                arguments.Add("description", string.Empty);
                arguments.Add("paste", text);
                arguments.Add("format", "simple");
                arguments.Add("return", "link");

                ur.Response = SendRequest(HttpMethod.POST, "http://paste.ee/api", arguments);

                if (!string.IsNullOrEmpty(ur.Response) && ur.Response.StartsWith("error"))
                {
                    Errors.Add(ur.Response);
                }
                else
                {
                    ur.URL = ur.Response;
                }
            }

            return ur;
        }
    }
}