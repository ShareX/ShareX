#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

/* https://github.com/matthewburnett */

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class GoogleCloudStorageNewFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.GoogleCloudStorage;

        public override Image ServiceImage => Resources.GoogleCloudStorage;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.GoogleCloudStorageOAuth2Info) && !string.IsNullOrEmpty(config.GoogleCloudStorageBucket);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GoogleCloudStorage(config.GoogleCloudStorageOAuth2Info)
            {
                bucket = config.GoogleCloudStorageBucket,
                domain = config.GoogleCloudStorageDomain,
                prefix = config.GoogleCloudStorageObjectPrefix
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGoogleCloudStorage;
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

        private string GetUploadPath(string fileName)
        {
            string path = NameParser.Parse(NameParserType.FolderPath, prefix.Trim('/'));
            return URLHelpers.CombineURL(path, fileName);
        }

        public class GoogleCloudStorageResponse
        {
            public string name { get; set; }
        }

        public class Metadata
        {
            public string name { get; set; }
            public Acl[] acl { get; set; }
        }

        public class Acl
        {
            public string entity { get; set; }
            public string role { get; set; }
        }

        public string bucket { get; set; }
        public string domain { get; set; }
        public string prefix { get; set; }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            string uploadpath = GetUploadPath(fileName);

            if (string.IsNullOrEmpty(domain))
            {
                domain = $"storage.googleapis.com/{bucket}";
            }

            Metadata metadata = new Metadata
            {
                name = uploadpath,
                acl = new Acl[]
                {
                    new Acl
                    {
                        entity = "allUsers",
                        role = "READER"
                    }
                }
            };

            string metadatajson = JsonConvert.SerializeObject(metadata);

            UploadResult result = SendRequestFile($"https://www.googleapis.com/upload/storage/v1/b/{bucket}/o?uploadType=multipart", stream, fileName,
                headers: googleAuth.GetAuthHeaders(), contentType: "multipart/related", metadata: metadatajson);
            GoogleCloudStorageResponse upload = JsonConvert.DeserializeObject<GoogleCloudStorageResponse>(result.Response);

            if (upload.name != uploadpath)
            {
                Errors.Add("Upload failed.");
                return null;
            }

            result.URL = $"https://{domain}/{uploadpath}";

            return result;
        }
    }
}