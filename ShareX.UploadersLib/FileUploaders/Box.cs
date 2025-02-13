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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class BoxFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Box;

        public override Icon ServiceIcon => Resources.Box;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.BoxOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Box(config.BoxOAuth2Info)
            {
                FolderID = config.BoxSelectedFolder.id,
                Share = config.BoxShare,
                ShareAccessLevel = config.BoxShareAccessLevel
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpBox;
    }

    public sealed class Box : FileUploader, IOAuth2
    {
        public static BoxFileEntry RootFolder = new BoxFileEntry
        {
            type = "folder",
            id = "0",
            name = "Root folder"
        };

        public OAuth2Info AuthInfo { get; set; }
        public string FolderID { get; set; }
        public bool Share { get; set; }
        public BoxShareAccessLevel ShareAccessLevel { get; set; }

        public Box(OAuth2Info oauth)
        {
            AuthInfo = oauth;
            FolderID = "0";
            Share = true;
            ShareAccessLevel = BoxShareAccessLevel.Open;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);

            return URLHelpers.CreateQueryString("https://www.box.com/api/oauth2/authorize", args);
        }

        public bool GetAccessToken(string pin)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("grant_type", "authorization_code");
            args.Add("code", pin);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);

            string response = SendRequestMultiPart("https://www.box.com/api/oauth2/token", args);

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
                args.Add("grant_type", "refresh_token");
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);

                string response = SendRequestMultiPart("https://www.box.com/api/oauth2/token", args);

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
            }

            return false;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
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
                Errors.Add("Box login is required.");
                return false;
            }

            return true;
        }

        public BoxFileInfo GetFiles(BoxFileEntry folder)
        {
            return GetFiles(folder.id);
        }

        public BoxFileInfo GetFiles(string id)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            string url = string.Format("https://api.box.com/2.0/folders/{0}/items", id);

            string response = SendRequest(HttpMethod.GET, url, headers: GetAuthHeaders());

            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<BoxFileInfo>(response);
            }

            return null;
        }

        public string CreateSharedLink(string id, BoxShareAccessLevel accessLevel)
        {
            string response = SendRequest(HttpMethod.PUT, "https://api.box.com/2.0/files/" + id, "{\"shared_link\": {\"access\": \"" + accessLevel.ToString().ToLower() + "\"}}", headers: GetAuthHeaders());

            if (!string.IsNullOrEmpty(response))
            {
                BoxFileEntry fileEntry = JsonConvert.DeserializeObject<BoxFileEntry>(response);

                if (fileEntry != null && fileEntry.shared_link != null)
                {
                    return fileEntry.shared_link.url;
                }
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            if (string.IsNullOrEmpty(FolderID))
            {
                FolderID = "0";
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("parent_id", FolderID);

            UploadResult result = SendRequestFile("https://upload.box.com/api/2.0/files/content", stream, fileName, "filename", args, GetAuthHeaders());

            if (result.IsSuccess)
            {
                BoxFileInfo fileInfo = JsonConvert.DeserializeObject<BoxFileInfo>(result.Response);

                if (fileInfo != null && fileInfo.entries != null && fileInfo.entries.Length > 0)
                {
                    BoxFileEntry fileEntry = fileInfo.entries[0];

                    if (Share)
                    {
                        AllowReportProgress = false;
                        result.URL = CreateSharedLink(fileEntry.id, ShareAccessLevel);
                    }
                    else
                    {
                        result.URL = string.Format("https://app.box.com/files/0/f/{0}/1/f_{1}", fileEntry.parent.id, fileEntry.id);
                    }
                }
            }

            return result;
        }
    }

    public class BoxFileInfo
    {
        public BoxFileEntry[] entries { get; set; }
    }

    public class BoxFileEntry
    {
        public string type { get; set; }
        public string id { get; set; }
        public string sequence_id { get; set; }
        public string etag { get; set; }
        public string name { get; set; }
        public BoxFileSharedLink shared_link { get; set; }
        public BoxFileEntry parent { get; set; }
    }

    public class BoxFileSharedLink
    {
        public string url { get; set; }
    }

    public class BoxFolder
    {
        public string ID;
        public string Name;
        public string User_id;
        public string Description;
        public string Shared;
        public string Shared_link;
        public string Permissions;

        //public List<BoxTag> Tags;
        //public List<BoxFile> Files;
        public List<BoxFolder> Folders = new List<BoxFolder>();
    }
}