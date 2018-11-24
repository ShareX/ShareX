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

using Newtonsoft.Json;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Web;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class GoogleDriveFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.GoogleDrive;

        public override Icon ServiceIcon => Resources.GoogleDrive;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.GoogleDriveOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GoogleDrive(config.GoogleDriveOAuth2Info)
            {
                IsPublic = config.GoogleDriveIsPublic,
                DirectLink = config.GoogleDriveDirectLink,
                FolderID = config.GoogleDriveUseFolder ? config.GoogleDriveFolderID : null
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGoogleDrive;
    }

    public enum GoogleDrivePermissionRole
    {
        owner, reader, writer, organizer, commenter
    }

    public enum GoogleDrivePermissionType
    {
        user, group, domain, anyone
    }

    public sealed class GoogleDrive : FileUploader, IOAuth2
    {
        private GoogleOAuth2 GoogleAuth { get; set; }
        public bool IsPublic { get; set; }
        public bool DirectLink { get; set; }
        public string FolderID { get; set; }

        public GoogleDrive(OAuth2Info oauth)
        {
            GoogleAuth = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/drive"
            };
        }

        public OAuth2Info AuthInfo => GoogleAuth.AuthInfo;

        public bool RefreshAccessToken()
        {
            return GoogleAuth.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return GoogleAuth.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return GoogleAuth.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return GoogleAuth.GetAccessToken(code);
        }

        private string GetMetadata(string name, string parentID)
        {
            object metadata;

            if (!string.IsNullOrEmpty(parentID))
            {
                metadata = new
                {
                    name = name,
                    parents = new[]
                    {
                        parentID
                    }
                };
            }
            else
            {
                metadata = new
                {
                    name = name
                };
            }

            return JsonConvert.SerializeObject(metadata);
        }

        private void SetPermissions(string fileID, GoogleDrivePermissionRole role, GoogleDrivePermissionType type, bool allowFileDiscovery)
        {
            if (!CheckAuthorization()) return;

            string url = string.Format("https://www.googleapis.com/drive/v3/files/{0}/permissions", fileID);

            string json = JsonConvert.SerializeObject(new
            {
                role = role.ToString(),
                type = type.ToString(),
                allowFileDiscovery = allowFileDiscovery.ToString()
            });

            string response = SendRequest(HttpMethod.POST, url, json, UploadHelpers.ContentTypeJSON, null, GoogleAuth.GetAuthHeaders());
        }

        public List<GoogleDriveFile> GetFolders(bool trashed = false, bool writer = true)
        {
            if (!CheckAuthorization()) return null;

            string query = "mimeType = 'application/vnd.google-apps.folder'";

            if (!trashed)
            {
                query += " and trashed = false";
            }

            if (writer)
            {
                query += " and 'me' in writers";
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("q", query);
            args.Add("fields", "nextPageToken,files(id,name,description)");

            List<GoogleDriveFile> folders = new List<GoogleDriveFile>();
            string pageToken = "";

            // Make sure we get all the pages of results
            do
            {
                args["pageToken"] = pageToken;
                string response = SendRequest(HttpMethod.GET, "https://www.googleapis.com/drive/v3/files", args, GoogleAuth.GetAuthHeaders());
                pageToken = "";

                if (!string.IsNullOrEmpty(response))
                {
                    GoogleDriveFileList fileList = JsonConvert.DeserializeObject<GoogleDriveFileList>(response);

                    if (fileList != null)
                    {
                        folders.AddRange(fileList.files);
                        pageToken = fileList.nextPageToken;
                    }
                }
            }
            while (!string.IsNullOrEmpty(pageToken));

            return folders;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            string metadata = GetMetadata(fileName, FolderID);

            UploadResult result = SendRequestFile("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart&fields=id,webViewLink,webContentLink", stream, fileName,
                headers: GoogleAuth.GetAuthHeaders(), contentType: "multipart/related", metadata: metadata);

            if (!string.IsNullOrEmpty(result.Response))
            {
                GoogleDriveFile upload = JsonConvert.DeserializeObject<GoogleDriveFile>(result.Response);

                if (upload != null)
                {
                    AllowReportProgress = false;

                    if (IsPublic)
                    {
                        SetPermissions(upload.id, GoogleDrivePermissionRole.reader, GoogleDrivePermissionType.anyone, false);
                    }

                    if (DirectLink)
                    {
                        Uri webContentLink = new Uri(upload.webContentLink);

                        string leftPart = webContentLink.GetLeftPart(UriPartial.Path);

                        NameValueCollection queryString = HttpUtility.ParseQueryString(webContentLink.Query);
                        queryString.Remove("export");

                        result.URL = $"{leftPart}?{queryString}";
                    }
                    else
                    {
                        result.URL = upload.webViewLink;
                    }
                }
            }

            return result;
        }
    }

    public class GoogleDriveFile
    {
        public string id { get; set; }
        public string webViewLink { get; set; }
        public string webContentLink { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class GoogleDriveFileList
    {
        public List<GoogleDriveFile> files { get; set; }
        public string nextPageToken { get; set; }
    }
}