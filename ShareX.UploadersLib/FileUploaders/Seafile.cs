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
using System.Web;

namespace ShareX.UploadersLib.FileUploaders
{
    public class SeafileFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Seafile;

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
                CreateShareableURLRaw = config.SeafileCreateShareableURLRaw
            };
        }
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

        public Seafile(string apiurl, string authtoken, string repoid)
        {
            APIURL = apiurl;
            AuthToken = authtoken;
            RepoID = repoid;
        }

        #region SeafileAuth

        public async Task<string> GetAuthTokenAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "auth-token/?format=json");

            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            string response = await SendRequestMultiPartAsync(url, args,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                SeafileAuthResponse AuthResult = JsonConvert.DeserializeObject<SeafileAuthResponse>(response);

                return AuthResult.token;
            }

            return "";
        }

        #endregion SeafileAuth

        #region SeafileChecks

        public async Task<bool> CheckAPIURLAsync(CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "ping/?format=json");

            string response = await SendRequestAsync(HttpMethod.GET, url,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                if (response == "\"pong\"")
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CheckAuthTokenAsync(CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "auth/ping/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            string response = await SendRequestAsync(HttpMethod.GET, url, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                if (response == "\"pong\"")
                {
                    return true;
                }
            }

            return false;
        }

        #endregion SeafileChecks

        #region SeafileAccountInformation

        public async Task<SeafileCheckAccInfoResponse> GetAccountInfoAsync(CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "account/info/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            string response = await SendRequestAsync(HttpMethod.GET, url, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                SeafileCheckAccInfoResponse AccInfoResponse = JsonConvert.DeserializeObject<SeafileCheckAccInfoResponse>(response);

                return AccInfoResponse;
            }

            return null;
        }

        #endregion SeafileAccountInformation

        #region SeafileLibraries

        public async Task<string> GetOrMakeDefaultLibraryAsync(string authtoken = null, CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "default-repo/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + (authtoken ?? AuthToken));

            string response = await SendRequestAsync(HttpMethod.GET, url, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                SeafileDefaultLibraryObj JsonResponse = JsonConvert.DeserializeObject<SeafileDefaultLibraryObj>(response);

                return JsonResponse.repo_id;
            }

            return null;
        }

        public async Task<List<SeafileLibraryObj>> GetLibrariesAsync(CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "repos/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            string response = await SendRequestAsync(HttpMethod.GET, url, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                List<SeafileLibraryObj> JsonResponse = JsonConvert.DeserializeObject<List<SeafileLibraryObj>>(response);

                return JsonResponse;
            }

            return null;
        }

        public async Task<bool> ValidatePathAsync(string path, CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "repos/" + RepoID + "/dir/?p=" + path + "&format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            string response = await SendRequestAsync(HttpMethod.GET, url, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                return true;
            }

            return false;
        }

        #endregion SeafileLibraries

        #region SeafileEncryptedLibrary

        public async Task<bool> DecryptLibraryAsync(string libraryPassword, CancellationToken cancellationToken = default)
        {
            string url = URLHelpers.FixPrefix(APIURL);
            url = URLHelpers.CombineURL(url, "repos/" + RepoID + "/?format=json");

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Token " + AuthToken);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("password", libraryPassword);

            string response = await SendRequestMultiPartAsync(url, args, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

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

        #endregion SeafileEncryptedLibrary

        #region SeafileUpload

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(APIURL))
            {
                throw new Exception(Localization.Strings.Seafile_API_URL_is_empty);
            }

            if (string.IsNullOrEmpty(AuthToken))
            {
                throw new Exception(Localization.Strings.Seafile_Authentication_token_is_empty);
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

            string response = await SendRequestAsync(HttpMethod.GET, url, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            string responseURL = response.Trim('"');

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("filename", fileName);
            args.Add("parent_dir", Path);

            UploadResult result = await SendRequestFileAsync(responseURL, stream, fileName, "file", args, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!IsError)
            {
                if (CreateShareableURL && !IsLibraryEncrypted)
                {
                    AllowReportProgress = false;
                    result.URL = await ShareFileAsync(Path + fileName, cancellationToken).ConfigureAwait(false);

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

        public async Task<string> ShareFileAsync(string path, CancellationToken cancellationToken = default)
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

            await SendRequestURLEncodedAsync(HttpMethod.PUT, url, args, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return LastResponseInfo?.Headers?["Location"];
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
