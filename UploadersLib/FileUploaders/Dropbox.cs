#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UploadersLib.HelperClasses;

namespace UploadersLib.FileUploaders
{
    public sealed class Dropbox : FileUploader, IOAuth
    {
        public OAuthInfo AuthInfo { get; set; }
        public DropboxAccountInfo AccountInfo { get; set; }

        public string UploadPath { get; set; }
        public bool AutoCreateShareableLink { get; set; }
        public bool ShortURL { get; set; }

        private const string APIVersion = "1";
        private const string Root = "dropbox"; // dropbox or sandbox

        private const string URLAPI = "https://api.dropbox.com/" + APIVersion;
        private const string URLAPIContent = "https://api-content.dropbox.com/" + APIVersion;

        private const string URLAccountInfo = URLAPI + "/account/info";
        private const string URLFiles = URLAPIContent + "/files/" + Root;
        private const string URLMetaData = URLAPI + "/metadata/" + Root;
        private const string URLShares = URLAPI + "/shares/" + Root;
        private const string URLCopy = URLAPI + "/fileops/copy";
        private const string URLCreateFolder = URLAPI + "/fileops/create_folder";
        private const string URLDelete = URLAPI + "/fileops/delete";
        private const string URLMove = URLAPI + "/fileops/move";
        private const string URLDownload = "https://dl.dropbox.com/u";

        private const string URLRequestToken = URLAPI + "/oauth/request_token";
        private const string URLAuthorize = "https://www.dropbox.com/" + APIVersion + "/oauth/authorize";
        private const string URLAccessToken = URLAPI + "/oauth/access_token";

        public Dropbox(OAuthInfo oauth)
        {
            AuthInfo = oauth;
        }

        public Dropbox(OAuthInfo oauth, DropboxAccountInfo accountInfo)
            : this(oauth)
        {
            AccountInfo = accountInfo;
        }

        // https://www.dropbox.com/developers/core/api#request-token
        // https://www.dropbox.com/developers/core/api#authorize
        public string GetAuthorizationURL()
        {
            return GetAuthorizationURL(URLRequestToken, URLAuthorize, AuthInfo);
        }

        // https://www.dropbox.com/developers/core/api#access-token
        public bool GetAccessToken(string verificationCode = null)
        {
            AuthInfo.AuthVerifier = verificationCode;
            return GetAccessToken(URLAccessToken, AuthInfo);
        }

        #region Dropbox accounts

        // https://www.dropbox.com/developers/core/api#account-info
        public DropboxAccountInfo GetAccountInfo()
        {
            DropboxAccountInfo account = null;

            if (OAuthInfo.CheckOAuth(AuthInfo))
            {
                string query = OAuthManager.GenerateQuery(URLAccountInfo, null, HttpMethod.GET, AuthInfo);

                string response = SendRequest(HttpMethod.GET, query);

                if (!string.IsNullOrEmpty(response))
                {
                    account = JsonConvert.DeserializeObject<DropboxAccountInfo>(response);

                    if (account != null)
                    {
                        AccountInfo = account;
                    }
                }
            }

            return account;
        }

        #endregion Dropbox accounts

        #region Files and metadata

        // https://www.dropbox.com/developers/core/api#files-GET
        public bool DownloadFile(string path, Stream downloadStream)
        {
            if (!string.IsNullOrEmpty(path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                string url = Helpers.CombineURL(URLFiles, Helpers.URLPathEncode(path));
                string query = OAuthManager.GenerateQuery(url, null, HttpMethod.GET, AuthInfo);
                return SendRequest(HttpMethod.GET, downloadStream, query);
            }

            return false;
        }

        // https://www.dropbox.com/developers/core/api#files-POST
        public UploadResult UploadFile(Stream stream, string path, string fileName, bool createShareableURL = false, bool shortURL = true)
        {
            if (!OAuthInfo.CheckOAuth(AuthInfo))
            {
                Errors.Add("Dropbox login is required.");
                return null;
            }

            string url = Helpers.CombineURL(URLFiles, Helpers.URLPathEncode(path));

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("file", fileName);

            string query = OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo);

            // There's a 150MB limit to all uploads through the API.
            UploadResult result = UploadData(stream, query, fileName);

            if (result.IsSuccess)
            {
                DropboxContentInfo content = JsonConvert.DeserializeObject<DropboxContentInfo>(result.Response);

                if (content != null)
                {
                    if (createShareableURL)
                    {
                        DropboxShares shares = CreateShareableLink(content.Path, shortURL);

                        if (shares != null)
                        {
                            result.URL = shares.URL;
                        }
                    }
                    else
                    {
                        result.URL = GetPublicURL(content.Path);
                    }
                }
            }

            return result;
        }

        // https://www.dropbox.com/developers/core/api#metadata
        public DropboxContentInfo GetMetadata(string path, bool list)
        {
            DropboxContentInfo contentInfo = null;

            if (OAuthInfo.CheckOAuth(AuthInfo))
            {
                string url = Helpers.CombineURL(URLMetaData, Helpers.URLPathEncode(path));

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("list", list ? "true" : "false");

                string query = OAuthManager.GenerateQuery(url, args, HttpMethod.GET, AuthInfo);

                string response = SendRequest(HttpMethod.GET, query);

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

        // https://www.dropbox.com/developers/core/api#shares
        public DropboxShares CreateShareableLink(string path, bool short_url = true)
        {
            DropboxShares shares = null;

            if (!string.IsNullOrEmpty(path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                string url = Helpers.CombineURL(URLShares, Helpers.URLPathEncode(path));

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("short_url", short_url ? "true" : "false");

                string query = OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo);

                string response = SendRequest(HttpMethod.POST, query);

                if (!string.IsNullOrEmpty(response))
                {
                    shares = JsonConvert.DeserializeObject<DropboxShares>(response);
                }
            }

            return shares;
        }

        #endregion Files and metadata

        #region File operations

        // https://www.dropbox.com/developers/core/api#fileops-copy
        public DropboxContentInfo Copy(string from_path, string to_path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(from_path) && !string.IsNullOrEmpty(to_path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("from_path", from_path);
                args.Add("to_path", to_path);

                string query = OAuthManager.GenerateQuery(URLCopy, args, HttpMethod.POST, AuthInfo);

                string response = SendRequest(HttpMethod.POST, query);

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        // https://www.dropbox.com/developers/core/api#fileops-create-folder
        public DropboxContentInfo CreateFolder(string path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("path", path);

                string query = OAuthManager.GenerateQuery(URLCreateFolder, args, HttpMethod.POST, AuthInfo);

                string response = SendRequest(HttpMethod.POST, query);

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        // https://www.dropbox.com/developers/core/api#fileops-delete
        public DropboxContentInfo Delete(string path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("path", path);

                string query = OAuthManager.GenerateQuery(URLDelete, args, HttpMethod.POST, AuthInfo);

                string response = SendRequest(HttpMethod.POST, query);

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<DropboxContentInfo>(response);
                }
            }

            return contentInfo;
        }

        // https://www.dropbox.com/developers/core/api#fileops-move
        public DropboxContentInfo Move(string from_path, string to_path)
        {
            DropboxContentInfo contentInfo = null;

            if (!string.IsNullOrEmpty(from_path) && !string.IsNullOrEmpty(to_path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("root", Root);
                args.Add("from_path", from_path);
                args.Add("to_path", to_path);

                string query = OAuthManager.GenerateQuery(URLMove, args, HttpMethod.POST, AuthInfo);

                string response = SendRequest(HttpMethod.POST, query);

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
            return UploadFile(stream, UploadPath, fileName, AutoCreateShareableLink, ShortURL);
        }

        public string GetPublicURL(string path)
        {
            return GetPublicURL(AccountInfo.Uid, path);
        }

        public static string GetPublicURL(long userID, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim('/');

                if (path.StartsWith("Public/", StringComparison.InvariantCultureIgnoreCase))
                {
                    path = Helpers.URLPathEncode((path.Substring(7)));
                    return Helpers.CombineURL(URLDownload, userID.ToString(), path);
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

            return string.Empty;
        }
    }

    public class DropboxAccountInfo
    {
        public string Referral_link { get; set; } // The user's referral link.
        public string Display_name { get; set; } // The user's display name.
        public long Uid { get; set; } // The user's unique Dropbox ID.
        public string Country { get; set; } // The user's two-letter country code, if available.
        public DropboxQuotaInfo Quota_info { get; set; }
        public string Email { get; set; }
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