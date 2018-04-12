using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareX.HelpersLib;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class KekUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue => ImageDestination.Kek;

        public override bool CheckConfig(UploadersConfig config)
        {
            return true;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new KekUploader();
        }
    }

    public sealed class KekUploader : ImageUploader
    {
        private const string UploadURL = "https://u.kek.gg/v1/upload-to-kek";

        public override UploadResult Upload(Stream stream, string fileName)
        {
            var fileExt = Path.GetExtension(fileName);
            var mimeType = MimeTypes.GetMimeType(fileExt);

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                var fileBytes = memoryStream.ToArray();
                var fileData = Convert.ToBase64String(fileBytes);

                var content = $"data:{mimeType};base64,{fileData}";

                var result = new UploadResult();

                result.Response = SendRequest(
                    method: HttpMethod.POST,
                    url: UploadURL,
                    content: content,
                    contentType: "text/plain");

                if (result.Response != null)
                {
                    result.URL = JsonHelpers.DeserializeFromString<string>(result.Response);
                }

                return result;
            }
        }
    }
}