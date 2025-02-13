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
                FolderID = config.GoogleDriveUseFolder ? config.GoogleDriveFolderID : null,
                DriveID = config.GoogleDriveSelectedDrive?.id
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
        public GoogleOAuth2 OAuth2 { get; private set; }
        public OAuth2Info AuthInfo => OAuth2.AuthInfo;
        public bool IsPublic { get; set; }
        public bool DirectLink { get; set; }
        public string FolderID { get; set; }
        public string DriveID { get; set; }

        public static GoogleDriveSharedDrive MyDrive = new GoogleDriveSharedDrive
        {
            id = "", // empty defaults to user drive
            name = Resources.GoogleDrive_MyDrive_My_drive
        };

        public GoogleDrive(OAuth2Info oauth)
        {
            OAuth2 = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/userinfo.profile"
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

        private string GetMetadata(string name, string parentID, string driveID = "")
        {
            object metadata;

            // If there's no parent folder, the drive behaves as parent
            if (string.IsNullOrEmpty(parentID))
            {
                parentID = driveID;
            }

            if (!string.IsNullOrEmpty(parentID))
            {
                metadata = new
                {
                    name = name,
                    driveId = driveID,
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

            string url = string.Format("https://www.googleapis.com/drive/v3/files/{0}/permissions?supportsAllDrives=true", fileID);

            string json = JsonConvert.SerializeObject(new
            {
                role = role.ToString(),
                type = type.ToString(),
                allowFileDiscovery = allowFileDiscovery.ToString()
            });

            SendRequest(HttpMethod.POST, url, json, RequestHelpers.ContentTypeJSON, null, OAuth2.GetAuthHeaders());
        }

        public List<GoogleDriveFile> GetFolders(string driveID = "", bool trashed = false, bool writer = true)
        {
            if (!CheckAuthorization()) return null;

            string query = "mimeType = 'application/vnd.google-apps.folder'";

            if (!trashed)
            {
                query += " and trashed = false";
            }

            if (writer && string.IsNullOrEmpty(driveID))
            {
                query += " and 'me' in writers";
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("q", query);
            args.Add("fields", "nextPageToken,files(id,name,description)");
            if (!string.IsNullOrEmpty(driveID))
            {
                args.Add("driveId", driveID);
                args.Add("corpora", "drive");
                args.Add("supportsAllDrives", "true");
                args.Add("includeItemsFromAllDrives", "true");
            }

            List<GoogleDriveFile> folders = new List<GoogleDriveFile>();
            string pageToken = "";

            // Make sure we get all the pages of results
            do
            {
                args["pageToken"] = pageToken;
                string response = SendRequest(HttpMethod.GET, "https://www.googleapis.com/drive/v3/files", args, OAuth2.GetAuthHeaders());
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

        public List<GoogleDriveSharedDrive> GetDrives()
        {
            if (!CheckAuthorization()) return null;

            Dictionary<string, string> args = new Dictionary<string, string>();
            List<GoogleDriveSharedDrive> drives = new List<GoogleDriveSharedDrive>();
            string pageToken = "";

            // Make sure we get all the pages of results
            do
            {
                args["pageToken"] = pageToken;
                string response = SendRequest(HttpMethod.GET, "https://www.googleapis.com/drive/v3/drives", args, OAuth2.GetAuthHeaders());
                pageToken = "";

                if (!string.IsNullOrEmpty(response))
                {
                    GoogleDriveSharedDriveList driveList = JsonConvert.DeserializeObject<GoogleDriveSharedDriveList>(response);

                    if (driveList != null)
                    {
                        drives.AddRange(driveList.drives);
                        pageToken = driveList.nextPageToken;
                    }
                }
            }
            while (!string.IsNullOrEmpty(pageToken));

            return drives;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            string metadata = GetMetadata(fileName, FolderID, DriveID);

            UploadResult result = SendRequestFile("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart&fields=id,webViewLink,webContentLink&supportsAllDrives=true",
                stream, fileName, "file", headers: OAuth2.GetAuthHeaders(), contentType: "multipart/related", relatedData: metadata);

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

    public class GoogleDriveSharedDrive
    {
        public string id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class GoogleDriveSharedDriveList
    {
        public List<GoogleDriveSharedDrive> drives { get; set; }
        public string nextPageToken { get; set; }
    }
}