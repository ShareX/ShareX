using Newtonsoft.Json;
using System;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class SkynetNewFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Skynet;

        public override bool CheckConfig(UploadersConfig config)
        {
            return true;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Skynet();
        }
    }
    public sealed class Skynet : FileUploader
    {
        string SkynetDomain = "siasky.net";

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string SkynetEndpoint = $"https://{SkynetDomain}/skynet/skyfile/{Guid.NewGuid().ToString()}";

            UploadResult result = SendRequestFile(SkynetEndpoint, stream, fileName, "file");

            SkynetResponse response = JsonConvert.DeserializeObject<SkynetResponse>(result.Response);

            result.URL = $"https://{SkynetDomain}/{response.skylink}";

            return result;
        }

        private class SkynetResponse
        {
            public string skylink { get; set; }
        }
    }
}
