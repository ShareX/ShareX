using Newtonsoft.Json;
using ShareX.HelpersLib;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class GoogleCloudStorageNewFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.GoogleCloudStorage;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.GoogleCloudStorageOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GoogleCloudStorage(config.YouTubeOAuth2Info)
            {
                bucket = config.GoogleCloudStorageBucket
            };
        }
    }

    public sealed class GoogleCloudStorage : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo => googleAuth.AuthInfo;

        private GoogleOAuth2 googleAuth;

        public GoogleCloudStorage(OAuth2Info oauth)
        {
            googleAuth = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/devstorage.full_control"
            };
        }

        public bool RefreshAccessToken()
        {
            return googleAuth.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return googleAuth.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return googleAuth.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return googleAuth.GetAccessToken(code);
        }

        public class ObjectACL
        {
            public string entity { get; set; }
            public string role { get; set; }
        }

        public string bucket { get; set; }
        public string domain { get; set; }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            if (string.IsNullOrEmpty(domain))
            {
                domain = "storage.googleapis.com";
            }

            string uploadurl = $"https://www.googleapis.com/upload/storage/v1/b/{bucket}/o";
            string publicurl = $"https://{domain}/{bucket}/{fileName}";
            string aclurl = $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{fileName}/acl";

            string contentType = Helpers.GetMimeType(fileName);

            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "uploadType", "media" },
                { "name", fileName }
            };

            ObjectACL requestacl = new ObjectACL
            {
                entity = "allUsers",
                role = "READER"
            };

            NameValueCollection headers = googleAuth.GetAuthHeaders();
            headers.Add("Content-Length", stream.Length.ToString());

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.POST, uploadurl, stream, contentType, args, headers);

            if (responseHeaders != null)
            {
                string requestjson = JsonConvert.SerializeObject(requestacl);
                SendRequest(HttpMethod.POST, aclurl, requestjson, ContentTypeJSON, headers: googleAuth.GetAuthHeaders());
            }

            return new UploadResult
            {
                IsSuccess = true,
                URL = publicurl
            };
        }
    }
}