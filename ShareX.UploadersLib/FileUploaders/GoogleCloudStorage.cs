using ShareX.HelpersLib;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ShareX.UploadersLib.FileUploaders
{
    public class GoogleCloudStorageNewFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.GoogleCloudStorage;

        public override bool CheckConfig(UploadersConfig config)
        {
            return true;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GoogleCloudStorage
            {
                token = "",
                bucket = "cdn.riolu.com"
            };
        }
    }

    public sealed class GoogleCloudStorage : FileUploader
    {
        public string token { get; set; }
        public string bucket { get; set; }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string uploadurl = $"https://www.googleapis.com/upload/storage/v1/b/{bucket}/o";
            string publicurl = $"https://storage.googleapis.com/{bucket}/{fileName}";

            string contentType = Helpers.GetMimeType(fileName);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("uploadType", "media");
            args.Add("name", fileName);

            NameValueCollection headers = new NameValueCollection
            {
                ["Content-Length"] = stream.Length.ToString(),
                ["Authorization"] = "Bearer " + token
            };

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.POST, uploadurl, stream, contentType, args, headers);

            if (responseHeaders == null)
            {
                Errors.Add("Upload to Google Cloud Storage failed.");
                return null;
            }

            return new UploadResult
            {
                IsSuccess = true,
                URL = publicurl
            };
        }
    }
}