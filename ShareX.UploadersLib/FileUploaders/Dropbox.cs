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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class DropboxFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Dropbox;

        public override Icon ServiceIcon => Resources.Dropbox;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.DropboxOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Dropbox(config.DropboxOAuth2Info)
            {
                UploadPath = NameParser.Parse(NameParserType.Default, Dropbox.VerifyPath(config.DropboxUploadPath)),
                AutoCreateShareableLink = config.DropboxAutoCreateShareableLink,
                UseDirectLink = config.DropboxUseDirectLink
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpDropbox;
    }

    public sealed class Dropbox : FileUploader, IOAuth2Basic
    {
        private const string APIVersion = "2";

        private const string URLWEB = "https://www.dropbox.com";
        private const string URLAPIBase = "https://api.dropboxapi.com";
        private const string URLAPI = URLAPIBase + "/" + APIVersion;
        private const string URLContent = "https://content.dropboxapi.com/" + APIVersion;
        private const string URLNotify = "https://notify.dropboxapi.com/" + APIVersion;

        private const string URLOAuth2Authorize = URLWEB + "/oauth2/authorize";
        private const string URLOAuth2Token = URLAPIBase + "/oauth2/token";

        private const string URLGetCurrentAccount = URLAPI + "/users/get_current_account";
        private const string URLDownload = URLContent + "/files/download";
        private const string URLUpload = URLContent + "/files/upload";
        private const string URLGetMetadata = URLAPI + "/files/get_metadata";
        private const string URLCreateSharedLink = URLAPI + "/sharing/create_shared_link_with_settings";
        private const string URLListSharedLinks = URLAPI + "/sharing/list_shared_links";
        private const string URLCopy = URLAPI + "/files/copy";
        private const string URLCreateFolder = URLAPI + "/files/create_folder";
        private const string URLDelete = URLAPI + "/files/delete";
        private const string URLMove = URLAPI + "/files/move";

        private const string URLShareDirect = "https://dl.dropboxusercontent.com/scl/fi/";

        public OAuth2Info AuthInfo { get; set; }
        public string UploadPath { get; set; }
        public bool AutoCreateShareableLink { get; set; }
        public bool UseDirectLink { get; set; }

        public Dropbox(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            return UploadFile(stream, UploadPath, fileName, AutoCreateShareableLink, UseDirectLink);
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);

            return URLHelpers.CreateQueryString(URLOAuth2Authorize, args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "authorization_code");
            args.Add("code", code);

            string response = SendRequestMultiPart(URLOAuth2Token, args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    AuthInfo.Token = token;
                    return true;
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

        public static string VerifyPath(string path, string fileName = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim().Replace('\\', '/').Trim('/');
                path = URLHelpers.AddSlash(path, SlashType.Prefix);

                if (!string.IsNullOrEmpty(fileName))
                {
                    path = URLHelpers.CombineURL(path, fileName);
                }

                return path;
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                return fileName;
            }

            return "";
        }

        public DropboxAccount GetCurrentAccount()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                string response = SendRequest(HttpMethod.POST, URLGetCurrentAccount, "null", RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<DropboxAccount>(response);
                }
            }

            return null;
        }

        public bool DownloadFile(string path, Stream downloadStream)
        {
            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path)
                });

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("arg", json);

                return SendRequestDownload(HttpMethod.POST, URLDownload, downloadStream, args, GetAuthHeaders(), null, RequestHelpers.ContentTypeJSON);
            }

            return false;
        }

        public UploadResult UploadFile(Stream stream, string path, string fileName, bool createShareableLink = false, bool useDirectLink = false)
        {
            if (stream.Length > 150000000)
            {
                Errors.Add("There's a 150MB limit to uploads through the API.");
                return null;
            }

            string json = JsonConvert.SerializeObject(new
            {
                path = VerifyPath(path, fileName),
                mode = "overwrite",
                autorename = false,
                mute = true
            });

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("arg", json);

            string response = SendRequest(HttpMethod.POST, URLUpload, stream, RequestHelpers.ContentTypeOctetStream, args, GetAuthHeaders());

            UploadResult ur = new UploadResult(response);

            if (!string.IsNullOrEmpty(ur.Response))
            {
                DropboxMetadata metadata = JsonConvert.DeserializeObject<DropboxMetadata>(ur.Response);

                if (metadata != null)
                {
                    if (createShareableLink)
                    {
                        AllowReportProgress = false;

                        ur.URL = CreateShareableLink(metadata.path_display, useDirectLink);
                    }
                    else
                    {
                        ur.IsURLExpected = false;
                    }
                }
            }

            return ur;
        }

        public DropboxMetadata GetMetadata(string path)
        {
            DropboxMetadata metadata = null;

            if (path != null && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path),
                    include_media_info = false,
                    include_deleted = false,
                    include_has_explicit_shared_members = false
                });

                string response = SendRequest(HttpMethod.POST, URLGetMetadata, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public bool IsExists(string path)
        {
            DropboxMetadata metadata = GetMetadata(path);

            return metadata != null && !metadata.tag.Equals("deleted", StringComparison.OrdinalIgnoreCase);
        }

        public string CreateShareableLink(string path, bool directLink)
        {
            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path),
                    settings = new
                    {
                        requested_visibility = "public" // Anyone who has received the link can access it. No login required.
                    }
                });

                string response = SendRequest(HttpMethod.POST, URLCreateSharedLink, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                DropboxLinkMetadata linkMetadata = null;

                if (!string.IsNullOrEmpty(response))
                {
                    linkMetadata = JsonConvert.DeserializeObject<DropboxLinkMetadata>(response);
                }
                else if (IsError && Errors.Errors[Errors.Count - 1].Text.Contains("\"shared_link_already_exists\"")) // Ugly workaround
                {
                    DropboxListSharedLinksResult result = ListSharedLinks(path, true);

                    if (result != null && result.links != null && result.links.Length > 0)
                    {
                        linkMetadata = result.links[0];
                    }
                }

                if (linkMetadata != null)
                {
                    if (directLink)
                    {
                        return GetDirectShareableURL(linkMetadata.url);
                    }
                    else
                    {
                        return linkMetadata.url;
                    }
                }
            }

            return null;
        }

        public DropboxListSharedLinksResult ListSharedLinks(string path, bool directOnly = false)
        {
            DropboxListSharedLinksResult result = null;

            if (path != null && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path),
                    direct_only = directOnly
                });

                string response = SendRequest(HttpMethod.POST, URLListSharedLinks, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    result = JsonConvert.DeserializeObject<DropboxListSharedLinksResult>(response);
                }
            }

            return result;
        }

        public DropboxMetadata Copy(string fromPath, string toPath)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(fromPath) && !string.IsNullOrEmpty(toPath) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    from_path = VerifyPath(fromPath),
                    to_path = VerifyPath(toPath)
                });

                string response = SendRequest(HttpMethod.POST, URLCopy, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public DropboxMetadata CreateFolder(string path)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path)
                });

                string response = SendRequest(HttpMethod.POST, URLCreateFolder, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public DropboxMetadata Delete(string path)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path)
                });

                string response = SendRequest(HttpMethod.POST, URLDelete, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public DropboxMetadata Move(string fromPath, string toPath)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(fromPath) && !string.IsNullOrEmpty(toPath) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    from_path = VerifyPath(fromPath),
                    to_path = VerifyPath(toPath)
                });

                string response = SendRequest(HttpMethod.POST, URLMove, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        private static string GetDirectShareableURL(string url)
        {
            string find = "dropbox.com/scl/fi/";
            int index = url.IndexOf(find);

            if (index > -1)
            {
                string path = url.Remove(0, index + find.Length);

                if (!string.IsNullOrEmpty(path))
                {
                    return URLHelpers.CombineURL(URLShareDirect, path);
                }
            }

            return null;
        }
    }

    public class DropboxAccount
    {
        public string account_id { get; set; }
        public DropboxAccountName name { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public bool disabled { get; set; }
        public string locale { get; set; }
        public string referral_link { get; set; }
        public bool is_paired { get; set; }
        public DropboxAccountType account_type { get; set; }
        public string profile_photo_url { get; set; }
        public string country { get; set; }
    }

    public class DropboxAccountName
    {
        public string given_name { get; set; }
        public string surname { get; set; }
        public string familiar_name { get; set; }
        public string display_name { get; set; }
    }

    public class DropboxAccountType
    {
        [JsonProperty(".tag")]
        public string tag { get; set; }
    }

    public class DropboxMetadata
    {
        [JsonProperty(".tag")]
        public string tag { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string client_modified { get; set; }
        public string server_modified { get; set; }
        public string rev { get; set; }
        public int size { get; set; }
        public string path_lower { get; set; }
        public string path_display { get; set; }
        public DropboxMetadataSharingInfo sharing_info { get; set; }
        public List<DropboxMetadataPropertyGroup> property_groups { get; set; }
        public bool has_explicit_shared_members { get; set; }
    }

    public class DropboxMetadataSharingInfo
    {
        public bool read_only { get; set; }
        public string parent_shared_folder_id { get; set; }
        public string modified_by { get; set; }
    }

    public class DropboxMetadataPropertyGroup
    {
        public string template_id { get; set; }
        public List<DropboxMetadataPropertyGroupField> fields { get; set; }
    }

    public class DropboxMetadataPropertyGroupField
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class DropboxLinkMetadata
    {
        [JsonProperty(".tag")]
        public string tag { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public DropboxLinkMetadataPermissions link_permissions { get; set; }
        public string client_modified { get; set; }
        public string server_modified { get; set; }
        public string rev { get; set; }
        public int size { get; set; }
        public string id { get; set; }
        public string path_lower { get; set; }
        public DropboxLinkMetadataTeamMemberInfo team_member_info { get; set; }
    }

    public class DropboxLinkMetadataPermissions
    {
        public bool can_revoke { get; set; }
        public DropboxLinkMetadataResolvedVisibility resolved_visibility { get; set; }
        public DropboxLinkMetadataRevokeFailureReason revoke_failure_reason { get; set; }
    }

    public class DropboxLinkMetadataResolvedVisibility
    {
        [JsonProperty(".tag")]
        public string tag { get; set; }
    }

    public class DropboxLinkMetadataRevokeFailureReason
    {
        [JsonProperty(".tag")]
        public string tag { get; set; }
    }

    public class DropboxLinkMetadataTeamMemberInfo
    {
        public DropboxLinkMetadataTeamInfo team_info { get; set; }
        public string display_name { get; set; }
        public string member_id { get; set; }
    }

    public class DropboxLinkMetadataTeamInfo
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class DropboxListSharedLinksResult
    {
        public DropboxLinkMetadata[] links { get; set; }
        public bool has_more { get; set; }
        public string cursor { get; set; }
    }
}