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
using System.Web;
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

        public class ObjectACL
        {
            public string entity { get; set; }
            public string role { get; set; }
        }

        public class GoogleCloudStorageResponse
        {
            public string name { get; set; }
        }

        public string bucket { get; set; }
        public string domain { get; set; }
        public string prefix { get; set; }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            UploadResult result = new UploadResult();

            string contentType = Helpers.GetMimeType(fileName);

            string uploadpath = GetUploadPath(fileName);

            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "uploadType", "media" },
                { "name", uploadpath }
            };

            ObjectACL publicacl = new ObjectACL
            {
                entity = "allUsers",
                role = "READER"
            };

            result.Response = SendRequest(HttpMethod.POST, $"https://www.googleapis.com/upload/storage/v1/b/{bucket}/o",
                stream, contentType, args, googleAuth.GetAuthHeaders());
            string responsename = JsonConvert.DeserializeObject<GoogleCloudStorageResponse>(result.Response).name;

            if (responsename == uploadpath)
            {
                string encodeduploadpath = HttpUtility.UrlEncode(uploadpath);
                string requestjson = JsonConvert.SerializeObject(publicacl);
                SendRequest(HttpMethod.POST, $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{encodeduploadpath}/acl",
                    requestjson, ContentTypeJSON, headers: googleAuth.GetAuthHeaders());
            }

            if (string.IsNullOrEmpty(domain))
            {
                domain = "storage.googleapis.com/{bucket}";
            }

            result.URL = $"https://{domain}/{uploadpath}";

            return result;
        }
    }
}