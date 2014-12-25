#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using ShareX.UploadersLib.HelperClasses;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class GoogleDrive : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public bool IsPublic { get; set; }
        public string FolderID { get; set; }

        public GoogleDrive(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
            args.Add("scope", "https://www.googleapis.com/auth/drive");

            return CreateQuery("https://accounts.google.com/o/oauth2/auth", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", code);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
            args.Add("grant_type", "authorization_code");

            string response = SendRequest(HttpMethod.POST, "https://accounts.google.com/o/oauth2/token", args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public bool RefreshAccessToken()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("grant_type", "refresh_token");

                string response = SendRequest(HttpMethod.POST, "https://accounts.google.com/o/oauth2/token", args);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                    if (token != null && !string.IsNullOrEmpty(token.access_token))
                    {
                        token.UpdateExpireDate();
                        string refresh_token = AuthInfo.Token.refresh_token;
                        AuthInfo.Token = token;
                        AuthInfo.Token.refresh_token = refresh_token;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckAuthorization()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (AuthInfo.Token.IsExpired && !RefreshAccessToken())
                {
                    Errors.Add("Refresh access token failed.");
                    return false;
                }
            }
            else
            {
                Errors.Add("Login is required.");
                return false;
            }

            return true;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        private void SetMetadata(string fileID, string title, string parentID = null)
        {
            string url = string.Format("https://www.googleapis.com/drive/v2/files/{0}", fileID);

            object metadata;

            if (!string.IsNullOrEmpty(parentID))
            {
                metadata = new
                {
                    title = title,
                    parents = new[]
                    {
                        new
                        {
                            id = parentID
                        }
                    }
                };
            }
            else
            {
                metadata = new
                {
                    title = title
                };
            }

            string json = JsonConvert.SerializeObject(metadata);

            string response = SendRequestJSON(url, json, GetAuthHeaders(), method: HttpMethod.PUT);
        }

        private void SetPermissions(string fileID, GoogleDrivePermissionRole role, GoogleDrivePermissionType type, string value, bool withLink)
        {
            string url = string.Format("https://www.googleapis.com/drive/v2/files/{0}/permissions", fileID);

            string json = JsonConvert.SerializeObject(new
            {
                role = role.ToString(),
                type = type.ToString(),
                value = value,
                withLink = withLink.ToString()
            });

            string response = SendRequestJSON(url, json, GetAuthHeaders());
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

            string response = SendRequest(HttpMethod.GET, "https://www.googleapis.com/drive/v2/files", args, GetAuthHeaders());

            if (!string.IsNullOrEmpty(response))
            {
                GoogleDriveFileList fileList = JsonConvert.DeserializeObject<GoogleDriveFileList>(response);

                if (fileList != null)
                {
                    return fileList.items;
                }
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            UploadResult result = UploadData(stream, "https://www.googleapis.com/upload/drive/v2/files", fileName, headers: GetAuthHeaders());

            if (!string.IsNullOrEmpty(result.Response))
            {
                GoogleDriveFile upload = JsonConvert.DeserializeObject<GoogleDriveFile>(result.Response);

                if (upload != null)
                {
                    AllowReportProgress = false;

                    SetMetadata(upload.id, fileName, FolderID);

                    if (IsPublic)
                    {
                        SetPermissions(upload.id, GoogleDrivePermissionRole.reader, GoogleDrivePermissionType.anyone, "", true);
                    }

                    result.URL = upload.alternateLink;
                }
            }

            return result;
        }

        public class GoogleDriveFile
        {
            public string id { get; set; }
            public string alternateLink { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class GoogleDriveFileList
        {
            public List<GoogleDriveFile> items { get; set; }
        }

        public enum GoogleDrivePermissionRole
        {
            owner, reader, writer
        }

        public enum GoogleDrivePermissionType
        {
            user, group, domain, anyone
        }
    }
}