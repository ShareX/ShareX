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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
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
            return new Dropbox(config.DropboxOAuth2Info, config.DropboxAccount)
            {
                UploadPath = NameParser.Parse(NameParserType.URL, Dropbox.TidyUploadPath(config.DropboxUploadPath)),
                AutoCreateShareableLink = config.DropboxAutoCreateShareableLink,
                ShareURLType = config.DropboxURLType
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpDropbox;
    }

    public sealed class Dropbox : FileUploader, IOAuth2Basic
    {
        public OAuth2Info AuthInfo { get; set; }
        public DropboxAccount Account { get; set; }
        public string UploadPath { get; set; }
        public bool AutoCreateShareableLink { get; set; }
        public DropboxURLType ShareURLType { get; set; }

        private const string APIVersion = "2";
        private const string Root = "dropbox";

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
        private const string URLCopy = URLAPI + "/files/copy";
        private const string URLCreateFolder = URLAPI + "/files/create_folder";
        private const string URLDelete = URLAPI + "/files/delete";
        private const string URLMove = URLAPI + "/files/move";

        private const string URLPublicDirect = "https://dl.dropboxusercontent.com/u";
        private const string URLShareDirect = "https://dl.dropboxusercontent.com/s";

        public Dropbox(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public Dropbox(OAuth2Info oauth, DropboxAccount account) : this(oauth)
        {
            Account = account;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);

            return CreateQuery(URLOAuth2Authorize, args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "authorization_code");
            args.Add("code", code);

            string response = SendRequest(HttpMethod.POST, URLOAuth2Token, args);

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

        #region Dropbox accounts

        public DropboxAccount GetAccountInfo()
        {
            DropboxAccount account = null;

            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                string response = SendRequestJSON(URLGetCurrentAccount, "null", GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    account = JsonConvert.DeserializeObject<DropboxAccount>(response);

                    if (account != null)
                    {
                        Account = account;
                    }
                }
            }

            return account;
        }

        #endregion Dropbox accounts

        #region Files and metadata

        public bool DownloadFile(string path, Stream downloadStream)
        {
            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string url = URLHelpers.CombineURL(URLDownload, URLHelpers.URLPathEncode(path));
                return SendRequest(HttpMethod.GET, downloadStream, url, headers: GetAuthHeaders());
            }

            return false;
        }

        public UploadResult UploadFile(Stream stream, string path, string fileName, bool createShareableURL = false, DropboxURLType urlType = DropboxURLType.Default)
        {
            string url = URLHelpers.CombineURL(URLUpload, URLHelpers.URLPathEncode(path));

            // There's a 150MB limit to all uploads through the API.
            UploadResult result = UploadData(stream, url, fileName, headers: GetAuthHeaders());

            if (result.IsSuccess)
            {
                DropboxContentInfo content = JsonConvert.DeserializeObject<DropboxContentInfo>(result.Response);

                if (content != null)
                {
                    if (createShareableURL)
                    {
                        AllowReportProgress = false;
                        result.URL = CreateShareableLink(content.Path, urlType);
                    }
                    else
                    {
                        result.URL = GetPublicURL(content.Path);
                    }
                }
            }

            return result;
        }

        public DropboxContentInfo GetMetadata(string path, bool list)
        {
            DropboxContentInfo contentInfo = null;

            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                string url = URLHelpers.CombineURL(URLGetMetadata, URLHelpers.URLPathEncode(path));

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("list", list ? "true" : "false");

                string response = SendRequest(HttpMethod.GET, url, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        public bool IsExists(string path)
        {
            DropboxContentInfo contentInfo = GetMetadata(path, false);
            return contentInfo != null && !contentInfo.Is_deleted;
        }

        public string CreateShareableLink(string path, DropboxURLType urlType)
        {
            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string url = URLHelpers.CombineURL(URLCreateSharedLink, URLHelpers.URLPathEncode(path));

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("short_url", urlType == DropboxURLType.Shortened ? "true" : "false");

                string response = SendRequest(HttpMethod.POST, url, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    DropboxShares shares = JsonConvert.DeserializeObject<DropboxShares>(response);

                    if (urlType == DropboxURLType.Direct)
                    {
                        Match match = Regex.Match(shares.URL, @"https?://(?:www\.)?dropbox.com/s/(?<path>\w+/.+)");
                        if (match.Success)
                        {
                            string urlPath = match.Groups["path"].Value;
                            if (!string.IsNullOrEmpty(urlPath))
                            {
                                return URLHelpers.CombineURL(URLShareDirect, urlPath);
                            }
                        }
                    }
                    else
                    {
                        return shares.URL;
                    }
                }
            }

            return null;
        }

        #endregion Files and metadata

        #region File operations

        public DropboxContentInfo Copy(string from_path, string to_path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(from_path) && !string.IsNullOrEmpty(to_path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("from_path", from_path);
                args.Add("to_path", to_path);

                string response = SendRequest(HttpMethod.POST, URLCopy, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        public DropboxContentInfo CreateFolder(string path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("path", path);

                string response = SendRequest(HttpMethod.POST, URLCreateFolder, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        public DropboxContentInfo Delete(string path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("path", path);

                string response = SendRequest(HttpMethod.POST, URLDelete, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        public DropboxContentInfo Move(string from_path, string to_path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(from_path) && !string.IsNullOrEmpty(to_path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("from_path", from_path);
                args.Add("to_path", to_path);

                string response = SendRequest(HttpMethod.POST, URLMove, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        #endregion File operations

        public override UploadResult Upload(Stream stream, string fileName)
        {
            CheckEarlyURLCopy(UploadPath, fileName);

            return UploadFile(stream, UploadPath, fileName, AutoCreateShareableLink, ShareURLType);
        }

        private void CheckEarlyURLCopy(string path, string fileName)
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !AutoCreateShareableLink)
            {
                string url = GetPublicURL(URLHelpers.CombineURL(path, fileName));
                OnEarlyURLCopyRequested(url);
            }
        }

        public string GetPublicURL(string path)
        {
            // TODO: uid
            return GetPublicURL(Account.account_id, path);
        }

        public static string GetPublicURL(string userID, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim('/');

                if (path.StartsWith("Public/", StringComparison.InvariantCultureIgnoreCase))
                {
                    path = URLHelpers.URLPathEncode(path.Substring(7));
                    return URLHelpers.CombineURL(URLPublicDirect, userID, path);
                }
            }

            return "Upload path is private. Use \"Public\" folder to get public URL.";
        }

        public static string TidyUploadPath(string uploadPath)
        {
            if (!string.IsNullOrEmpty(uploadPath))
            {
                return uploadPath.Trim().Replace('\\', '/').Trim('/') + "/";
            }

            return "";
        }
    }

    public enum DropboxURLType
    {
        Default,
        Shortened,
        Direct
    }

    public class DropboxPath
    {
        public string path { get; set; }

        public DropboxPath(string path)
        {
            this.path = path;
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
        public string tag { get; set; }
    }

    public class DropboxQuotaInfo
    {
        public long Normal { get; set; } // The user's used quota outside of shared folders (bytes).
        public long Shared { get; set; } // The user's used quota in shared folders (bytes).
        public long Quota { get; set; } // The user's total quota allocation (bytes).
    }

    public class DropboxContentInfo
    {
        public string Size { get; set; } // A human-readable description of the file size (translated by locale).
        public long Bytes { get; set; } // The file size in bytes.
        public string Path { get; set; } // Returns the canonical path to the file or directory.
        public bool Is_dir { get; set; } // Whether the given entry is a folder or not.
        public bool Is_deleted { get; set; } // Whether the given entry is deleted (only included if deleted files are being returned).
        public string Rev { get; set; } // A unique identifier for the current revision of a file. This field is the same rev as elsewhere in the API and can be used to detect changes and avoid conflicts.
        public string Hash { get; set; } // A folder's hash is useful for indicating changes to the folder's contents in later calls to /metadata. This is roughly the folder equivalent to a file's rev.
        public bool Thumb_exists { get; set; } // True if the file is an image can be converted to a thumbnail via the /thumbnails call.
        public string Icon { get; set; } // The name of the icon used to illustrate the file type in Dropbox's icon library.
        public string Modified { get; set; } // The last time the file was modified on Dropbox, in the standard date format (not included for the root folder).
        public string Client_mtime { get; set; } // For files, this is the modification time set by the desktop client when the file was added to Dropbox, in the standard date format. Since this time is not verified (the Dropbox server stores whatever the desktop client sends up), this should only be used for display purposes (such as sorting) and not, for example, to determine if a file has changed or not.
        public string Root { get; set; } // The root or top-level folder depending on your access level. All paths returned are relative to this root level. Permitted values are either dropbox or app_folder.
        public long Revision { get; set; } // A deprecated field that semi-uniquely identifies a file. Use rev instead.
        public string Mime_type { get; set; }
        public DropboxContentInfo[] Contents { get; set; }
    }

    public class DropboxShares
    {
        public string URL { get; set; }
        public string Expires { get; set; }
    }
}