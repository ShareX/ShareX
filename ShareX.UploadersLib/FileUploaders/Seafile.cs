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
using System.Web;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class SeafileFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Seafile;

        public override Image ServiceImage => Resources.Seafile;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.SeafileAPIURL) && !string.IsNullOrEmpty(config.SeafileAuthToken) && !string.IsNullOrEmpty(config.SeafileRepoID);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Seafile(config.SeafileAPIURL, config.SeafileAuthToken, config.SeafileRepoID)
            {
                Path = config.SeafilePath,
                IsLibraryEncrypted = config.SeafileIsLibraryEncrypted,
                EncryptedLibraryPassword = config.SeafileEncryptedLibraryPassword,
                ShareDaysToExpire = config.SeafileShareDaysToExpire,
                SharePassword = config.SeafileSharePassword,
                CreateShareableURL = config.SeafileCreateShareableURL,
                CreateShareableURLRaw = config.SeafileCreateShareableURLRaw,
                IgnoreInvalidCert = config.SeafileIgnoreInvalidCert
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpSeafile;
    }

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
        public bool CreateShareableURLRaw { get; set; }
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
            url = URLHelpers.CombineURL(url, "auth-token/?format=json");

            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            string response = SendRequestMultiPart(url, args);

            if (!string.IsNullOrEmpty(response))
            {
                SeafileAuthResponse AuthResult = JsonConvert.DeserializeObject<SeafileAuthResponse>(response);

                return AuthResult.token;
            }

            return "";
        }

        #endregion SeafileAuth

        #region SeafileChecks

        public bool CheckAPIURL()
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "ping/?format=json");

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
            url = URLHelpers.CombineURL(url, "auth/ping/?format=json");

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
            url = URLHelpers.CombineURL(url, "account/info/?format=json");

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

        #endregion SeafileAccountInformation

        #region SeafileLibraries

        public string GetOrMakeDefaultLibrary(string authtoken = null)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "default-repo/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + (authtoken ?? AuthToken));

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

        public List<SeafileLibraryObj> GetLibraries()
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "repos/?format=json");

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
            url = URLHelpers.CombineURL(url, "repos/" + RepoID + "/dir/?p=" + path + "&format=json");

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

        #endregion SeafileLibraries

        #region SeafileEncryptedLibrary

        public bool DecryptLibrary(string libraryPassword)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "repos/" + RepoID + "/?format=json");

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

                string response = SendRequestMultiPart(url, args, headers);

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
            url = URLHelpers.CombineURL(url, "repos/" + RepoID + "/upload-link/?format=json");

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

                UploadResult result = SendRequestFile(responseURL, stream, fileName, "file", args, headers);

                if (!IsError)
                {
                    if (CreateShareableURL && !IsLibraryEncrypted)
                    {
                        AllowReportProgress = false;
                        result.URL = ShareFile(Path + fileName);

                        if (CreateShareableURLRaw)
                        {
                            UriBuilder uriBuilder = new UriBuilder(result.URL);
                            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
                            query["raw"] = "1";
                            uriBuilder.Query = query.ToString();
                            result.URL = $"{uriBuilder.Scheme}://{uriBuilder.Host}{uriBuilder.Path}{uriBuilder.Query}";
                        }
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
            url = URLHelpers.CombineURL(url, "repos", RepoID, "file/shared-link/");

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

                SendRequestURLEncoded(HttpMethod.PUT, url, args, headers);
                return LastResponseInfo.Headers["Location"];
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

    public class SeafileAuthResponse
    {
        public string token { get; set; }
    }

    public class SeafileCheckAccInfoResponse
    {
        public long usage { get; set; }
        public long total { get; set; }
        public string email { get; set; }
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

    public class SeafileDefaultLibraryObj
    {
        public string repo_id { get; set; }
        public bool exists { get; set; }
    }
}