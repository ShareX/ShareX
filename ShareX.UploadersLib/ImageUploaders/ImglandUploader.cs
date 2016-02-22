using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ShareX.UploadersLib.ImageUploaders
{
    public sealed class ImglandUploader : ImageUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            string uploadUrl = "http://www.imgland.net/process.php?subAPI=mainsite";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("usubmit", "true");

            UploadResult result = UploadData(stream, uploadUrl, fileName, "imagefile[]", arguments);

            if (result.IsSuccess)
            {
                ImglandResponse response = JsonConvert.DeserializeObject<ImglandResponse>(result.Response);

                if (!string.IsNullOrEmpty(response.Url))
                {
                    result.URL = response.Url;
                }
                else
                {
                    Errors.Add(string.IsNullOrEmpty(response.Status) ? "Unknown error." : response.Status);
                }
            }

            return result;
        }
    }

    public class ImglandResponse
    {
        public string Status { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
