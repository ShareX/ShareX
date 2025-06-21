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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class OneDriveFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.OneDrive;

        public override Icon ServiceIcon => Resources.OneDrive;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.OneDriveV2OAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new OneDrive(config.OneDriveV2OAuth2Info)
            {
                FolderID = config.OneDriveV2SelectedFolder.id,
                AutoCreateShareableLink = config.OneDriveAutoCreateShareableLink,
                UseDirectLink = config.OneDriveUseDirectLink
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpOneDrive;
    }

    public sealed class OneDrive : FileUploader, IOAuth2
    {
        private const string AuthorizationEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        private const string TokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        private const int MaxSegmentSize = 64 * 1024 * 1024; // 64 MiB

        public OAuth2Info AuthInfo { get; set; }
        public string FolderID { get; set; }
        public bool AutoCreateShareableLink { get; set; }
        public bool UseDirectLink { get; set; }

        public static OneDriveFileInfo RootFolder = new OneDriveFileInfo
        {
            id = "", // empty defaults to root
            name = Resources.OneDrive_RootFolder_Root_folder
        };

        public OneDrive(OAuth2Info authInfo)
        {
            AuthInfo = authInfo;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("scope", "offline_access files.readwrite");
            args.Add("response_type", "code");
            args.Add("redirect_uri", Links.Callback);
            if (AuthInfo.Proof != null)
            {
                args.Add("code_challenge", AuthInfo.Proof.CodeChallenge);
                args.Add("code_challenge_method", AuthInfo.Proof.ChallengeMethod);
            }

            return URLHelpers.CreateQueryString(AuthorizationEndpoint, args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", Links.Callback);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("code", code);
            args.Add("grant_type", "authorization_code");
            if (AuthInfo.Proof != null)
            {
                args.Add("code_verifier", AuthInfo.Proof.CodeVerifier);
            }

            string response = SendRequestURLEncoded(HttpMethod.POST, TokenEndpoint, args);

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
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("grant_type", "refresh_token");

                string response = SendRequestURLEncoded(HttpMethod.POST, TokenEndpoint, args);

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

        private string GetFolderUrl(string folderID)
        {
            string folderPath;

            if (!string.IsNullOrEmpty(folderID))
            {
                folderPath = URLHelpers.CombineURL("me/drive/items", folderID);
            }
            else
            {
                folderPath = "me/drive/root";
            }

            return folderPath;
        }

        private string CreateSession(string fileName)
        {
            string json = JsonConvert.SerializeObject(new
            {
                item = new Dictionary<string, string>
                {
                    { "@microsoft.graph.conflictBehavior", "replace" }
                }
            });

            string folderPath = GetFolderUrl(FolderID);

            string url = URLHelpers.BuildUri("https://graph.microsoft.com", $"/v1.0/{folderPath}:/{fileName}:/createUploadSession");

            AllowReportProgress = false;
            string response = SendRequest(HttpMethod.POST, url, json, RequestHelpers.ContentTypeJSON, headers: GetAuthHeaders());
            AllowReportProgress = true;

            OneDriveUploadSession session = JsonConvert.DeserializeObject<OneDriveUploadSession>(response);

            if (session != null)
            {
                return session.uploadUrl;
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            UploadResult result;
            string sessionUrl = CreateSession(fileName);
            long position = 0;

            do
            {
                result = SendRequestFileRange(sessionUrl, stream, fileName, position, MaxSegmentSize);

                if (result.IsSuccess)
                {
                    position += MaxSegmentSize;
                }
                else
                {
                    SendRequest(HttpMethod.DELETE, sessionUrl);
                    break;
                }
            }
            while (position < stream.Length);

            if (result.IsSuccess)
            {
                OneDriveFileInfo uploadInfo = JsonConvert.DeserializeObject<OneDriveFileInfo>(result.Response);

                if (AutoCreateShareableLink)
                {
                    AllowReportProgress = false;

                    result.URL = CreateShareableLink(uploadInfo.id, UseDirectLink ? OneDriveLinkType.Embed : OneDriveLinkType.Read);
                }
                else
                {
                    result.URL = uploadInfo.webUrl;
                }
            }

            return result;
        }

        public string CreateShareableLink(string id, OneDriveLinkType linkType = OneDriveLinkType.Read)
        {
            string linkTypeValue;

            switch (linkType)
            {
                case OneDriveLinkType.Embed:
                    linkTypeValue = "embed";
                    break;
                default:
                case OneDriveLinkType.Read:
                    linkTypeValue = "view";
                    break;
                case OneDriveLinkType.Edit:
                    linkTypeValue = "edit";
                    break;
            }

            string json = JsonConvert.SerializeObject(new
            {
                type = linkTypeValue,
                scope = "anonymous"
            });

            string response = SendRequest(HttpMethod.POST, $"https://graph.microsoft.com/v1.0/me/drive/items/{id}/createLink", json, RequestHelpers.ContentTypeJSON,
                headers: GetAuthHeaders());

            OneDrivePermission permissionInfo = JsonConvert.DeserializeObject<OneDrivePermission>(response);

            if (permissionInfo != null && permissionInfo.link != null)
            {
                return permissionInfo.link.webUrl;
            }

            return null;
        }

        public OneDriveFileList GetPathInfo(string id)
        {
            if (!CheckAuthorization()) return null;

            string folderPath = GetFolderUrl(id);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("select", "id,name");
            args.Add("filter", "folder ne null");

            string response = SendRequest(HttpMethod.GET, $"https://graph.microsoft.com/v1.0/{folderPath}/children", args, GetAuthHeaders());

            if (response != null)
            {
                OneDriveFileList pathInfo = JsonConvert.DeserializeObject<OneDriveFileList>(response);
                return pathInfo;
            }

            return null;
        }
    }

    public class OneDriveFileInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
    }

    public class OneDrivePermission
    {
        public OneDriveShareableLink link { get; set; }
    }

    public class OneDriveShareableLink
    {
        public string webUrl { get; set; }
        public string webHtml { get; set; }
    }

    public class OneDriveFileList
    {
        public OneDriveFileInfo[] value { get; set; }
    }

    public class OneDriveUploadSession
    {
        public string uploadUrl { get; set; }
        public string[] nextExpectedRanges { get; set; }
    }

    public enum OneDriveLinkType
    {
        [Description("An embedded link, which is an HTML code snippet that you can insert into a webpage to provide an interactive view of the corresponding file.")]
        Embed,
        [Description("A read-only link, which is a link to a read-only version of the folder or file.")]
        Read,
        [Description("A read-write link, which is a link to a read-write version of the folder or file.")]
        Edit
    }
}