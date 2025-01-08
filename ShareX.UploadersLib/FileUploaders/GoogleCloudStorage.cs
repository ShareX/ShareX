#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class GoogleCloudStorageNewFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.GoogleCloudStorage;

        public override Icon ServiceIcon => Resources.GoogleCloud;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.GoogleCloudStorageOAuth2Info) && !string.IsNullOrEmpty(config.GoogleCloudStorageBucket);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GoogleCloudStorage(config.GoogleCloudStorageOAuth2Info)
            {
                Bucket = config.GoogleCloudStorageBucket,
                Domain = config.GoogleCloudStorageDomain,
                Prefix = config.GoogleCloudStorageObjectPrefix,
                RemoveExtensionImage = config.GoogleCloudStorageRemoveExtensionImage,
                RemoveExtensionText = config.GoogleCloudStorageRemoveExtensionText,
                RemoveExtensionVideo = config.GoogleCloudStorageRemoveExtensionVideo,
                SetPublicACL = config.GoogleCloudStorageSetPublicACL
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGoogleCloudStorage;
    }

    public sealed class GoogleCloudStorage : FileUploader, IOAuth2
    {
        public GoogleOAuth2 OAuth2 { get; private set; }
        public OAuth2Info AuthInfo => OAuth2.AuthInfo;
        public string Bucket { get; set; }
        public string Domain { get; set; }
        public string Prefix { get; set; }
        public bool RemoveExtensionImage { get; set; }
        public bool RemoveExtensionText { get; set; }
        public bool RemoveExtensionVideo { get; set; }
        public bool SetPublicACL { get; set; }

        public GoogleCloudStorage(OAuth2Info oauth)
        {
            OAuth2 = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/devstorage.read_write https://www.googleapis.com/auth/userinfo.profile"
            };
        }

        public bool RefreshAccessToken()
        {
            return OAuth2.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return OAuth2.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return OAuth2.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return OAuth2.GetAccessToken(code);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            string uploadPath = GetUploadPath(fileName);

            OnEarlyURLCopyRequested(GenerateURL(uploadPath));

            GoogleCloudStorageMetadata googleCloudStorageMetadata = new GoogleCloudStorageMetadata
            {
                name = uploadPath
            };

            if (SetPublicACL)
            {
                googleCloudStorageMetadata.acl = new GoogleCloudStorageAcl[]
                {
                    new GoogleCloudStorageAcl
                    {
                        entity = "allUsers",
                        role = "READER"
                    }
                };
            }

            string serializedGoogleCloudStorageMetadata = JsonConvert.SerializeObject(googleCloudStorageMetadata);

            UploadResult result = SendRequestFile($"https://www.googleapis.com/upload/storage/v1/b/{Bucket}/o?uploadType=multipart&fields=name", stream, fileName, null, headers: OAuth2.GetAuthHeaders(), contentType: "multipart/related", relatedData: serializedGoogleCloudStorageMetadata);

            GoogleCloudStorageResponse googleCloudStorageResponse = JsonConvert.DeserializeObject<GoogleCloudStorageResponse>(result.Response);

            result.URL = GenerateURL(googleCloudStorageResponse.name);

            return result;
        }

        private string GetUploadPath(string fileName)
        {
            string uploadPath = NameParser.Parse(NameParserType.FilePath, Prefix.Trim('/'));

            if ((RemoveExtensionImage && FileHelpers.IsImageFile(fileName)) ||
                (RemoveExtensionText && FileHelpers.IsTextFile(fileName)) ||
                (RemoveExtensionVideo && FileHelpers.IsVideoFile(fileName)))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            return URLHelpers.CombineURL(uploadPath, fileName);
        }

        public string GenerateURL(string uploadPath)
        {
            if (string.IsNullOrEmpty(Bucket))
            {
                return "";
            }

            if (string.IsNullOrEmpty(Domain))
            {
                Domain = URLHelpers.CombineURL("storage.googleapis.com", Bucket);
            }

            uploadPath = URLHelpers.URLEncode(uploadPath, true, HelpersOptions.URLEncodeIgnoreEmoji);

            string url = URLHelpers.CombineURL(Domain, uploadPath);

            return URLHelpers.FixPrefix(url);
        }

        public string GetPreviewURL()
        {
            string uploadPath = GetUploadPath("example.png");
            return GenerateURL(uploadPath);
        }

        private class GoogleCloudStorageResponse
        {
            public string name { get; set; }
        }

        private class GoogleCloudStorageMetadata
        {
            public string name { get; set; }
            public GoogleCloudStorageAcl[] acl { get; set; }
        }

        private class GoogleCloudStorageAcl
        {
            public string entity { get; set; }
            public string role { get; set; }
        }
    }
}