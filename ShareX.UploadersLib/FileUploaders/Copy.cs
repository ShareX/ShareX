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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class Copy : FileUploader, IOAuth
    {
        public OAuthInfo AuthInfo { get; set; }
        public CopyAccountInfo AccountInfo { get; set; }
        public string UploadPath { get; set; }
        public CopyURLType URLType { get; set; }

        private const string APIVersion = "1";

        private const string URLAPI = "https://api.copy.com/rest";

        private const string URLAccountInfo = URLAPI + "/user";
        private const string URLFiles = URLAPI + "/files";
        private const string URLMetaData = URLAPI + "/meta/";
        private const string URLLinks = URLAPI + "/links";

        private const string URLRequestToken = "https://api.copy.com/oauth/request";
        private const string URLAuthorize = "https://www.copy.com/applications/authorize";
        private const string URLAccessToken = "https://api.copy.com/oauth/access";

        private readonly NameValueCollection APIHeaders = new NameValueCollection { { "X-Api-Version", APIVersion } };

        public Copy(OAuthInfo oauth)
        {
            AuthInfo = oauth;
        }

        public Copy(OAuthInfo oauth, CopyAccountInfo accountInfo) : this(oauth)
        {
            AccountInfo = accountInfo;
        }

        // https://developers.copy.com/documentation#authentication/oauth-handshake
        // https://developers.copy.com/console
        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("oauth_callback", Links.Callback);

            return GetAuthorizationURL(URLRequestToken, URLAuthorize, AuthInfo, args);
        }

        public bool GetAccessToken(string verificationCode = null)
        {
            AuthInfo.AuthVerifier = verificationCode;
            return GetAccessToken(URLAccessToken, AuthInfo);
        }

        #region Copy accounts

        // https://developers.copy.com/documentation#api-calls/profile
        public CopyAccountInfo GetAccountInfo()
        {
            CopyAccountInfo account = null;

            if (OAuthInfo.CheckOAuth(AuthInfo))
            {
                string query = OAuthManager.GenerateQuery(URLAccountInfo, null, HttpMethod.GET, AuthInfo);

                string response = SendRequest(HttpMethod.GET, query, null, APIHeaders);

                if (!string.IsNullOrEmpty(response))
                {
                    account = JsonConvert.DeserializeObject<CopyAccountInfo>(response);

                    if (account != null)
                    {
                        AccountInfo = account;
                    }
                }
            }

            return account;
        }

        #endregion Copy accounts

        #region Files and metadata

        // https://developers.copy.com/documentation#api-calls/filesystem - Download Raw File Conents
        // GET https://api.copy.com/rest/files/PATH/TO/FILE
        public bool DownloadFile(string path, Stream downloadStream)
        {
            if (!string.IsNullOrEmpty(path) && OAuthInfo.CheckOAuth(AuthInfo))
            {
                string url = URLHelpers.CombineURL(URLFiles, URLHelpers.URLEncode(path, true));
                string query = OAuthManager.GenerateQuery(url, null, HttpMethod.GET, AuthInfo);
                return SendRequestDownload(HttpMethod.GET, query, downloadStream);
            }

            return false;
        }

        // https://developers.copy.com/documentation#api-calls/filesystem - Create File or Directory
        // POST https://api.copy.com/rest/files/PATH/TO/FILE?overwrite=true
        public UploadResult UploadFile(Stream stream, string path, string fileName)
        {
            if (!OAuthInfo.CheckOAuth(AuthInfo))
            {
                Errors.Add("Copy login is required.");
                return null;
            }

            string url = URLHelpers.CombineURL(URLFiles, URLHelpers.URLEncode(path, true));

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("overwrite", "true");

            string query = OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo);

            // There's a 1GB and 5 hour(max time for a single upload) limit to all uploads through the API.
            UploadResult result = SendRequestFile(query, stream, fileName, "file", headers: APIHeaders);

            if (result.IsSuccess)
            {
                CopyUploadInfo content = JsonConvert.DeserializeObject<CopyUploadInfo>(result.Response);

                if (content != null && content.objects != null && content.objects.Length > 0)
                {
                    AllowReportProgress = false;
                    result.URL = CreatePublicURL(content.objects[0].path, URLType);
                }
            }

            return result;
        }

        // https://developers.copy.com/documentation#api-calls/filesystem - Read Root Directory
        // GET https://api.copy.com/rest/meta/copy
        public CopyContentInfo GetMetadata(string path)
        {
            CopyContentInfo contentInfo = null;

            if (OAuthInfo.CheckOAuth(AuthInfo))
            {
                string url = URLHelpers.CombineURL(URLMetaData, URLHelpers.URLEncode(path, true));

                string query = OAuthManager.GenerateQuery(url, null, HttpMethod.GET, AuthInfo);

                string response = SendRequest(HttpMethod.GET, query);

                if (!string.IsNullOrEmpty(response))
                {
                    contentInfo = JsonConvert.DeserializeObject<CopyContentInfo>(response);
                }
            }

            return contentInfo;
        }

        #endregion Files and metadata

        public override UploadResult Upload(Stream stream, string fileName)
        {
            return UploadFile(stream, UploadPath, fileName);
        }

        public string GetLinkURL(CopyLinksInfo link, string path, CopyURLType urlType = CopyURLType.Default)
        {
            string fileName = URLHelpers.URLEncode(URLHelpers.GetFileName(path));

            switch (urlType)
            {
                default:
                case CopyURLType.Default:
                    return string.Format("https://www.copy.com/s/{0}/{1}", link.id, fileName);
                case CopyURLType.Shortened:
                    return string.Format("https://copy.com/{0}", link.id);
                case CopyURLType.Direct:
                    return string.Format("https://copy.com/{0}/{1}", link.id, fileName);
            }
        }

        public string CreatePublicURL(string path, CopyURLType urlType = CopyURLType.Default)
        {
            path = path.Trim('/');

            string url = URLHelpers.CombineURL(URLLinks, URLHelpers.URLEncode(path, true));

            string query = OAuthManager.GenerateQuery(url, null, HttpMethod.POST, AuthInfo);

            CopyLinkRequest publicLink = new CopyLinkRequest();
            publicLink.@public = true;
            publicLink.name = "ShareX";
            publicLink.paths = new string[] { path };

            string content = JsonConvert.SerializeObject(publicLink);

            string response = SendRequest(HttpMethod.POST, query, content, headers: APIHeaders);

            if (!string.IsNullOrEmpty(response))
            {
                CopyLinksInfo link = JsonConvert.DeserializeObject<CopyLinksInfo>(response);

                return GetLinkURL(link, path, urlType);
            }

            return "";
        }

        public string GetPublicURL(string path, CopyURLType urlType = CopyURLType.Default)
        {
            path = path.Trim('/');

            CopyContentInfo fileInfo = GetMetadata(path);
            foreach (CopyLinksInfo link in fileInfo.links)
            {
                if (!link.expired && link.@public)
                {
                    return GetLinkURL(link, path, urlType);
                }
            }

            return "";
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

    public enum CopyURLType
    {
        Default,
        Shortened,
        Direct
    }

    public class CopyAccountInfo
    {
        public long id { get; set; } // The user's unique Copy ID.
        public string first_name { get; set; } // The user's first name.
        public string last_name { get; set; } // The user's last name.
        public CopyStorageInfo storage { get; set; }
        public string email { get; set; }
    }

    public class CopyStorageInfo
    {
        public long used { get; set; } // The user's used quota outside of shared folders (bytes).
        public long quota { get; set; } // The user's total quota allocation (bytes).
        public long saved { get; set; } // The user's saved quota through shared folders (bytes).
    }

    public class CopyLinkRequest
    {
        public bool @public { get; set; }
        public string name { get; set; }
        public string[] paths { get; set; }
    }

    public class CopyLinksInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool @public { get; set; }
        public bool expires { get; set; }
        public bool expired { get; set; }
        public string url { get; set; }
        public string url_short { get; set; }
        public string creator_id { get; set; }
        public string company_id { get; set; }
        public bool confirmation_required { get; set; }
        public string status { get; set; }
        public string permissions { get; set; }
    }

    public class CopyContentInfo // https://api.copy.com/rest/meta also works on 'rest/files'
    {
        public string id { get; set; } // Internal copy name
        public string path { get; set; } // file path
        public string name { get; set; } // Human readable (Filesystem) folder name
        public string type { get; set; } // "inbox", "root", "copy", "dir", "file"?
        public bool stub { get; set; } // 'The stub attribute you see on all of the nodes represents if the specified node is incomplete, that is, if the children have not all been delivered to you. Basically, they will always be a stub, unless you are looking at that item directly.'
        public long? size { get; set; } // Filesizes (size attributes) are measured in bytes. If an item displayed in the filesystem is a directory or an otherwise special location which doesn't represent a file, the size attribute will be null.
        public long date_last_synced { get; set; }
        public bool @public { get; set; } // is available to public; isnt everything private but shared in copy???
        public string url { get; set; } // web access url (private)
        public long revision_id { get; set; } // Revision of content
        public CopyLinksInfo[] links { get; set; } // links array
        public CopyContentInfo[] children { get; set; } // Children
    }

    public class CopyUploadInfo
    {
        public CopyContentInfo[] objects { get; set; }
    }
}