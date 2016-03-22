#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class OneDriveFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.OneDrive;

        public override bool CheckConfig(UploadersConfig uploadersConfig)
        {
            return OAuth2Info.CheckOAuth(uploadersConfig.OneDriveOAuth2Info);
        }

        public override FileUploader CreateUploader(UploadersConfig uploadersConfig)
        {
            return new OneDrive(uploadersConfig.OneDriveOAuth2Info)
            {
                FolderID = uploadersConfig.OneDriveSelectedFolder.id,
                AutoCreateShareableLink = uploadersConfig.OneDriveAutoCreateShareableLink
            };
        }
    }

    public sealed class OneDrive : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public string FolderID { get; set; }
        public bool AutoCreateShareableLink { get; set; }

        public static OneDriveFileInfo RootFolder = new OneDriveFileInfo
        {
            id = "me/skydrive",
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
            args.Add("scope", "wl.offline_access wl.skydrive_update");
            args.Add("response_type", "code");
            args.Add("redirect_uri", Links.URL_CALLBACK);

            return CreateQuery("https://login.live.com/oauth20_authorize.srf", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("code", code);
            args.Add("grant_type", "authorization_code");

            string response = SendRequestURLEncoded("https://login.live.com/oauth20_token.srf", args);

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
                args.Add("redirect_uri", Links.URL_CALLBACK);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("grant_type", "refresh_token");

                string response = SendRequestURLEncoded("https://login.live.com/oauth20_token.srf", args);

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

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("access_token", AuthInfo.Token.access_token);
            args.Add("overwrite", "true");
            args.Add("downsize_photo_uploads", "false");

            string folderPath;

            if (!string.IsNullOrEmpty(FolderID))
            {
                folderPath = URLHelpers.CombineURL(FolderID, "files");
            }
            else
            {
                folderPath = "me/skydrive/files";
            }

            string url = CreateQuery(URLHelpers.CombineURL("https://apis.live.net/v5.0", folderPath), args);

            UploadResult result = UploadData(stream, url, fileName);

            if (result.IsSuccess)
            {
                OneDriveFileInfo uploadInfo = JsonConvert.DeserializeObject<OneDriveFileInfo>(result.Response);

                if (AutoCreateShareableLink)
                {
                    result.URL = CreateShareableLink(uploadInfo.id);
                }
                else
                {
                    result.URL = uploadInfo.source;
                }
            }

            return result;
        }

        public string CreateShareableLink(string id, OneDriveLinkType linkType = OneDriveLinkType.Read)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("access_token", AuthInfo.Token.access_token);

            string linkTypeValue;

            switch (linkType)
            {
                case OneDriveLinkType.Embed:
                    linkTypeValue = "embed";
                    break;
                default:
                case OneDriveLinkType.Read:
                    linkTypeValue = "shared_read_link";
                    break;
                case OneDriveLinkType.Edit:
                    linkTypeValue = "shared_edit_link";
                    break;
            }

            string url = CreateQuery(string.Format("https://apis.live.net/v5.0/{0}/{1}", id, linkTypeValue), args);

            string response = SendRequest(HttpMethod.GET, url);

            OneDriveShareableLinkInfo shareableLinkInfo = JsonConvert.DeserializeObject<OneDriveShareableLinkInfo>(response);

            if (shareableLinkInfo != null)
            {
                return shareableLinkInfo.link;
            }

            return null;
        }

        public OneDrivePathInfo GetPathInfo(string path)
        {
            if (!CheckAuthorization()) return null;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("access_token", AuthInfo.Token.access_token);

            if (!path.EndsWith("files")) path += "/files";

            string url = CreateQuery(URLHelpers.CombineURL("https://apis.live.net/v5.0", path), args);

            string response = SendRequest(HttpMethod.GET, url);

            if (response != null)
            {
                OneDrivePathInfo pathInfo = JsonConvert.DeserializeObject<OneDrivePathInfo>(response);
                return pathInfo;
            }

            return null;
        }
    }

    public class OneDriveFileInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string source { get; set; }
    }

    public class OneDriveShareableLinkInfo
    {
        public string link { get; set; }
    }

    public class OneDrivePathInfo
    {
        public OneDriveFileInfo[] data { get; set; }
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