#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class DropboxFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Dropbox;

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

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            return await UploadFileAsync(stream, UploadPath, fileName, AutoCreateShareableLink, UseDirectLink,
                cancellationToken).ConfigureAwait(false);
        }

        public Task<string> GetAuthorizationURLAsync(CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);

            return Task.FromResult(URLHelpers.CreateQueryString(URLOAuth2Authorize, args));
        }

        public async Task<bool> GetAccessTokenAsync(string code, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "authorization_code");
            args.Add("code", code);

            string response = await SendRequestMultiPartAsync(URLOAuth2Token, args,
                cancellationToken: cancellationToken).ConfigureAwait(false);

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

        public async Task<DropboxAccount> GetCurrentAccountAsync(CancellationToken cancellationToken = default)
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                string response = await SendRequestAsync(HttpMethod.POST, URLGetCurrentAccount, "null", RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<DropboxAccount>(response);
                }
            }

            return null;
        }

        public async Task<bool> DownloadFileAsync(string path, Stream downloadStream, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path)
                });

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("arg", json);

                return await SendRequestDownloadAsync(HttpMethod.POST, URLDownload, downloadStream, args, GetAuthHeaders(), null,
                    RequestHelpers.ContentTypeJSON, cancellationToken).ConfigureAwait(false);
            }

            return false;
        }

        public async Task<UploadResult> UploadFileAsync(Stream stream, string path, string fileName, bool createShareableLink = false,
            bool useDirectLink = false, CancellationToken cancellationToken = default)
        {
            if (stream.Length > 150000000)
            {
                Errors.Add(Localization.Strings.Dropbox_API_upload_limit);
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

            string response = await SendRequestAsync(HttpMethod.POST, URLUpload, stream, RequestHelpers.ContentTypeOctetStream, args, GetAuthHeaders(),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            UploadResult ur = new UploadResult(response);

            if (!string.IsNullOrEmpty(ur.Response))
            {
                DropboxMetadata metadata = JsonConvert.DeserializeObject<DropboxMetadata>(ur.Response);

                if (metadata != null)
                {
                    if (createShareableLink)
                    {
                        AllowReportProgress = false;

                        ur.URL = await CreateShareableLinkAsync(metadata.path_display, useDirectLink, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        ur.IsURLExpected = false;
                    }
                }
            }

            return ur;
        }

        public async Task<DropboxMetadata> GetMetadataAsync(string path, CancellationToken cancellationToken = default)
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

                string response = await SendRequestAsync(HttpMethod.POST, URLGetMetadata, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public async Task<bool> IsExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            DropboxMetadata metadata = await GetMetadataAsync(path, cancellationToken).ConfigureAwait(false);

            return metadata != null && !metadata.tag.Equals("deleted", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> CreateShareableLinkAsync(string path, bool directLink, CancellationToken cancellationToken = default)
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

                string response = await SendRequestAsync(HttpMethod.POST, URLCreateSharedLink, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                DropboxLinkMetadata linkMetadata = null;

                if (!string.IsNullOrEmpty(response))
                {
                    linkMetadata = JsonConvert.DeserializeObject<DropboxLinkMetadata>(response);
                }
                else if (IsError && Errors.Errors[Errors.Count - 1].Text.Contains("\"shared_link_already_exists\"")) // Ugly workaround
                {
                    DropboxListSharedLinksResult result = await ListSharedLinksAsync(path, true, cancellationToken).ConfigureAwait(false);

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

        public async Task<DropboxListSharedLinksResult> ListSharedLinksAsync(string path, bool directOnly = false,
            CancellationToken cancellationToken = default)
        {
            DropboxListSharedLinksResult result = null;

            if (path != null && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path),
                    direct_only = directOnly
                });

                string response = await SendRequestAsync(HttpMethod.POST, URLListSharedLinks, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    result = JsonConvert.DeserializeObject<DropboxListSharedLinksResult>(response);
                }
            }

            return result;
        }

        public async Task<DropboxMetadata> CopyAsync(string fromPath, string toPath, CancellationToken cancellationToken = default)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(fromPath) && !string.IsNullOrEmpty(toPath) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    from_path = VerifyPath(fromPath),
                    to_path = VerifyPath(toPath)
                });

                string response = await SendRequestAsync(HttpMethod.POST, URLCopy, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public async Task<DropboxMetadata> CreateFolderAsync(string path, CancellationToken cancellationToken = default)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path)
                });

                string response = await SendRequestAsync(HttpMethod.POST, URLCreateFolder, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public async Task<DropboxMetadata> DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    path = VerifyPath(path)
                });

                string response = await SendRequestAsync(HttpMethod.POST, URLDelete, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    metadata = JsonConvert.DeserializeObject<DropboxMetadata>(response);
                }
            }

            return metadata;
        }

        public async Task<DropboxMetadata> MoveAsync(string fromPath, string toPath, CancellationToken cancellationToken = default)
        {
            DropboxMetadata metadata = null;

            if (!string.IsNullOrEmpty(fromPath) && !string.IsNullOrEmpty(toPath) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    from_path = VerifyPath(fromPath),
                    to_path = VerifyPath(toPath)
                });

                string response = await SendRequestAsync(HttpMethod.POST, URLMove, json, RequestHelpers.ContentTypeJSON, null, GetAuthHeaders(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);

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
