using System.Collections.Generic;
using System.IO;

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
                APIKey = "null"
            };
        }
    }

    public sealed class GoogleCloudStorage : FileUploader
    {
        public string APIKey { get; set; }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "key", APIKey }
            };

            return null;
        }
    }
}