#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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

// Credits: https://github.com/zikeji

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class Seafile : FileUploader
    {
        public string APIURL { get; set; }
        public string AuthToken { get; set; }
        public string RepoID { get; set; }
        public string Path { get; set; }
        public bool IsLibraryEncrypted { get; set; }
        public string EncryptedLibraryPassword { get; set; }
        public int ShareDaysToExpire { get; set; }
        public string SharePassword { get; set; }
        public bool CreateShareableURL { get; set; }
        public bool IgnoreInvalidCert { get; set; }

        public Seafile(string apiurl, string authtoken, string repoid)
        {
            APIURL = apiurl;
            AuthToken = authtoken;
            RepoID = repoid;
        }

        #region SeafileAuth

        public string GetAuthToken(string username, string password)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "auth-token/?format=json");

            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            string response = SendRequest(HttpMethod.POST, url, args);

            if (!string.IsNullOrEmpty(response))
            {
                SeafileAuthResponse AuthResult = JsonConvert.DeserializeObject<SeafileAuthResponse>(response);

                return AuthResult.token;
            }

            return string.Empty;
        }

        public class SeafileAuthResponse
        {
            public string token { get; set; }
        }

        #endregion SeafileAuth

        #region SeafileChecks

        public bool CheckAPIURL()
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "ping/?format=json");

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url);

                if (!string.IsNullOrEmpty(response))
                {
                    if (response == "\"pong\"")
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        public bool CheckAuthToken()
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "auth/ping/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url, null, headers);

                if (!string.IsNullOrEmpty(response))
                {
                    if (response == "\"pong\"")
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        #endregion SeafileChecks

        #region SeafileAccountInformation

        public SeafileCheckAccInfoResponse GetAccountInfo()
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "account/info/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url, null, headers);

                if (!string.IsNullOrEmpty(response))
                {
                    SeafileCheckAccInfoResponse AccInfoResponse = JsonConvert.DeserializeObject<SeafileCheckAccInfoResponse>(response);

                    return AccInfoResponse;
                }

                return null;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        public class SeafileCheckAccInfoResponse
        {
            public long usage { get; set; }
            public long total { get; set; }
            public string email { get; set; }
        }

        #endregion SeafileAccountInformation

        #region SeafileLibraries

        public string GetOrMakeDefaultLibrary(string authtoken = null)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "default-repo/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + (authtoken == null ? AuthToken : authtoken));

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url, null, headers);

                if (!string.IsNullOrEmpty(response))
                {
                    SeafileDefaultLibraryObj JsonResponse = JsonConvert.DeserializeObject<SeafileDefaultLibraryObj>(response);

                    return JsonResponse.repo_id;
                }

                return null;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        public class SeafileDefaultLibraryObj
        {
            public string repo_id { get; set; }
            public bool exists { get; set; }
        }

        public List<SeafileLibraryObj> GetLibraries()
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "repos/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url, null, headers);

                if (!string.IsNullOrEmpty(response))
                {
                    List<SeafileLibraryObj> JsonResponse = JsonConvert.DeserializeObject<List<SeafileLibraryObj>>(response);

                    return JsonResponse;
                }

                return null;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        public bool ValidatePath(string path)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "repos/" + RepoID + "/dir/?p=" + path + "&format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url, null, headers);

                if (!string.IsNullOrEmpty(response))
                {
                    return true;
                }

                return false;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        public class SeafileLibraryObj
        {
            public string permission { get; set; }
            public bool encrypted { get; set; }
            public long mtime { get; set; }
            public string owner { get; set; }
            public string id { get; set; }
            public long size { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            [JsonProperty("virtual")]
            public string _virtual { get; set; }
            public string desc { get; set; }
            public string root { get; set; }
        }

        #endregion SeafileLibraries

        #region SeafileEncryptedLibrary

        public bool DecryptLibrary(string libraryPassword)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "repos/" + RepoID + "/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("password", libraryPassword);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.POST, url, args, headers);

                if (!string.IsNullOrEmpty(response))
                {
                    if (response == "\"success\"")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        #endregion SeafileEncryptedLibrary

        #region SeafileUpload

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(APIURL))
            {
                throw new Exception("Seafile API URL is empty.");
            }

            if (string.IsNullOrEmpty(AuthToken))
            {
                throw new Exception("Seafile Authentication Token is empty.");
            }

            if (string.IsNullOrEmpty(Path))
            {
                Path = "/";
            }
            else
            {
                char pathLast = Path[Path.Length - 1];
                if (pathLast != '/')
                {
                    Path += "/";
                }
            }

            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "repos/" + RepoID + "/upload-link/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                string response = SendRequest(HttpMethod.GET, url, null, headers);

                string responseURL = response.Trim('"');

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("filename", fileName);
                args.Add("parent_dir", Path);

                UploadResult result = UploadData(stream, responseURL, fileName, "file", args, headers);

                if (!IsError)
                {
                    if (CreateShareableURL && !IsLibraryEncrypted)
                    {
                        AllowReportProgress = false;
                        result.URL = ShareFile(Path + fileName);
                    }
                    else
                    {
                        result.IsURLExpected = false;
                    }
                }

                return result;
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        public string ShareFile(string path)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(APIURL, "repos", RepoID, "file/shared-link/");

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("p", path);
            args.Add("share_type", "download");
            if (!string.IsNullOrEmpty(SharePassword)) args.Add("password", SharePassword);
            if (ShareDaysToExpire > 0) args.Add("expire", ShareDaysToExpire.ToString());

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (IgnoreInvalidCert)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                return SendRequestURLEncoded(url, args, headers, method: HttpMethod.PUT, responseType: ResponseType.LocationHeader);
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }
            }
        }

        #endregion SeafileUpload
    }
}