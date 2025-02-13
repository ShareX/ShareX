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
using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    /// <summary>
    /// A <see cref="FileUploaderService"/> implementation for the Backblaze B2 Cloud Storage API.
    /// </summary>
    public class BackblazeB2UploaderService : FileUploaderService
    {
        public override FileDestination EnumValue => FileDestination.BackblazeB2;

        public override Icon ServiceIcon => Resources.BackblazeB2;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrWhiteSpace(config.B2ApplicationKeyId) && !string.IsNullOrWhiteSpace(config.B2ApplicationKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new BackblazeB2(applicationKeyId: config.B2ApplicationKeyId,
                applicationKey: config.B2ApplicationKey,
                bucketName: config.B2BucketName,
                uploadPath: config.B2UploadPath,
                useCustomUrl: config.B2UseCustomUrl,
                customUrl: config.B2CustomUrl);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpBackblazeB2;
    }

    /// <summary>
    /// An <see cref="ImageUploader"/> implementation for the Backblaze B2 Cloud Storage API.
    /// </summary>
    [Localizable(false)]
    public sealed class BackblazeB2 : ImageUploader
    {
        private const string B2AuthorizeAccountUrl = "https://api.backblazeb2.com/b2api/v1/b2_authorize_account";

        // after we authorize, we'll get an api url that we need to prepend here
        private const string B2GetUploadUrlPath = "/b2api/v1/b2_get_upload_url";
        private const string B2ListBucketsPath = "/b2api/v1/b2_list_buckets";

        private const string ApplicationJson = "application/json; charset=utf-8";

        public string ApplicationKeyId { get; }
        public string ApplicationKey { get; }
        public string BucketName { get; }
        public string UploadPath { get; }
        public bool UseCustomUrl { get; }
        public string CustomUrl { get; }

        public BackblazeB2(string applicationKeyId, string applicationKey, string bucketName, string uploadPath, bool useCustomUrl, string customUrl)
        {
            ApplicationKeyId = applicationKeyId;
            ApplicationKey = applicationKey;
            BucketName = bucketName;
            UploadPath = uploadPath;
            UseCustomUrl = useCustomUrl;
            CustomUrl = customUrl;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string parsedUploadPath = NameParser.Parse(NameParserType.FilePath, UploadPath);
            string destinationPath = URLHelpers.CombineURL(parsedUploadPath, fileName);

            // docs: https://www.backblaze.com/b2/docs/

            // STEP 1: authorize, get auth token, api url, download url
            DebugHelper.WriteLine($"B2 uploader: Attempting to authorize as '{ApplicationKeyId}'.");
            B2Authorization auth = B2ApiAuthorize(ApplicationKeyId, ApplicationKey, out string authError);
            if (authError != null)
            {
                DebugHelper.WriteLine("B2 uploader: Failed to authorize.");
                Errors.Add($"Could not authenticate with B2: {authError}");
                return null;
            }

            DebugHelper.WriteLine($"B2 uploader: Authorized, using API server {auth.apiUrl}, download URL {auth.downloadUrl}");

            // STEP 1.25: if we have an application key, there will be a bucketId present here, but if
            //            not, we have an account key and need to find our bucket id ourselves
            string bucketId = auth.allowed?.bucketId;
            if (bucketId == null)
            {
                DebugHelper.WriteLine("B2 uploader: Key doesn't have a bucket ID set, so I'm looking for a bucket ID.");

                string newBucketId = B2ApiGetBucketId(auth, BucketName, out string getBucketError);
                if (getBucketError != null)
                {
                    DebugHelper.WriteLine($"B2 uploader: It's {newBucketId}.");
                    bucketId = newBucketId;
                }
            }

            // STEP 1.5: verify whether we can write to the bucket user wants to write to, with the given prefix
            DebugHelper.WriteLine("B2 uploader: Checking clientside whether we have permission to upload.");
            bool authCheckOk = IsAuthorizedForUpload(auth, bucketId, destinationPath, out string authCheckError);
            if (!authCheckOk)
            {
                DebugHelper.WriteLine("B2 uploader: Key is not suitable for this upload.");
                Errors.Add($"B2 upload failed: {authCheckError}");
                return null;
            }

            // STEP 1.75: start upload attempt loop
            const int maxTries = 5;
            B2UploadUrl url = null;
            for (int tries = 1; tries <= maxTries; tries++)
            {
                string newOrSameUrl = url == null ? "New URL." : "Same URL.";
                DebugHelper.WriteLine($"B2 uploader: Upload attempt {tries} of {maxTries}. {newOrSameUrl}");

                // sloppy, but we need exponential backoff somehow and we are not in async code
                // since B2Uploader should have the thread to itself, and this just occurs on rare failures,
                // this should be OK
                if (tries > 1)
                {
                    int delay = (int)Math.Pow(2, tries - 1) * 1000;
                    DebugHelper.WriteLine($"Waiting ${delay} ms for backoff.");
                    Thread.Sleep(delay);
                }

                // STEP 2: get upload url that we need to POST to in step 3
                if (url == null)
                {
                    DebugHelper.WriteLine("B2 uploader: Getting new upload URL.");
                    url = B2ApiGetUploadUrl(auth, bucketId, out string getUrlError);
                    if (getUrlError != null)
                    {
                        // this is guaranteed to be unrecoverable, so bail out
                        DebugHelper.WriteLine("B2 uploader: Got error trying to get upload URL.");
                        Errors.Add("Could not get B2 upload URL: " + getUrlError);
                        return null;
                    }
                }

                // STEP 3: upload file and see if anything went wrong
                DebugHelper.WriteLine($"B2 uploader: Uploading to URL {url.uploadUrl}");
                B2UploadResult uploadResult = B2ApiUploadFile(url, destinationPath, stream);
                HashSet<string> expiredTokenCodes = new HashSet<string>(new List<string> { "expired_auth_token", "bad_auth_token" });

                if (uploadResult.RC == -1)
                {
                    // magic number for "connection failed", should also happen when upload
                    // caps are exceeded
                    DebugHelper.WriteLine("B2 uploader: Connection failed, trying with new URL.");
                    url = null;
                    continue;
                }
                else if (uploadResult.RC == 401 && expiredTokenCodes.Contains(uploadResult.Error.code))
                {
                    // Unauthorized, our token expired
                    DebugHelper.WriteLine("B2 uploader: Upload auth token expired, trying with new URL.");
                    url = null;
                    continue;
                }
                else if (uploadResult.RC == 408)
                {
                    DebugHelper.WriteLine("B2 uploader: Request Timeout, trying with same URL.");
                    continue;
                }
                else if (uploadResult.RC == 429)
                {
                    DebugHelper.WriteLine("B2 uploader: Too Many Requests, trying with same URL.");
                    continue;
                }
                else if (uploadResult.RC == 503)
                {
                    DebugHelper.WriteLine("B2 uploader: Service Unavailable, trying with new URL.");
                    url = null;
                    continue;
                }
                else if (uploadResult.RC != 200)
                {
                    // something else happened that wasn't a success, so bail out
                    DebugHelper.WriteLine("B2 uploader: Unknown error, upload failure.");
                    Errors.Add("B2 uploader: Unknown error occurred while calling b2_upload_file().");
                    return null;
                }

                // success!
                // STEP 4: compose:
                //           the download url (e.g. "https://f567.backblazeb2.com")
                //           /file/$bucket/$uploadPath
                //         or
                //           $customUrl/$uploadPath

                string encodedFileName = URLHelpers.URLEncode(uploadResult.Upload.fileName, true);
                string remoteLocation = URLHelpers.CombineURL(auth.downloadUrl, "file", URLHelpers.URLEncode(BucketName), encodedFileName);

                DebugHelper.WriteLine($"B2 uploader: Successful upload! File should be at: {remoteLocation}");

                if (UseCustomUrl)
                {
                    remoteLocation = URLHelpers.CombineURL(CustomUrl, encodedFileName);
                    remoteLocation = URLHelpers.FixPrefix(remoteLocation);

                    DebugHelper.WriteLine($"B2 uploader: But user requested custom URL, which will be: {remoteLocation}");
                }

                return new UploadResult()
                {
                    IsSuccess = true,
                    URL = remoteLocation
                };
            }

            DebugHelper.WriteLine("B2 uploader: Ran out of attempts, aborting.");
            Errors.Add($"B2 upload failed: Could not upload file after {maxTries} attempts.");
            return null;
        }

        /// <summary>
        /// Attempts to authorize against the B2 API with the given key.
        /// </summary>
        /// <param name="keyId">The application key ID <b>or</b> account ID.</param>
        /// <param name="key">The application key <b>or</b> account master key.</param>
        /// <param name="error">Will be set to a non-null value on failure.</param>
        /// <returns>Null if an error occurs, and <c>error</c> will contain an error message. Otherwise, a <see cref="B2Authorization"/>.</returns>
        private B2Authorization B2ApiAuthorize(string keyId, string key, out string error)
        {
            NameValueCollection headers = RequestHelpers.CreateAuthenticationHeader(keyId, key);

            using (HttpWebResponse res = GetResponse(HttpMethod.GET, B2AuthorizeAccountUrl, headers: headers, allowNon2xxResponses: true))
            {
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    error = StringifyB2Error(res);
                    return null;
                }

                string body = RequestHelpers.ResponseToString(res);

                error = null;
                return JsonConvert.DeserializeObject<B2Authorization>(body);
            }
        }

        /// <summary>
        /// Gets the bucket ID for the given bucket name. Requires <c>listBuckets</c> permission.
        /// </summary>
        /// <param name="auth">The B2 API authorization.</param>
        /// <param name="bucketName">The bucket to get the ID for.</param>
        /// <param name="error">Will be set to a non-null value on failure.</param>
        /// <returns>Null if an error occurs, and <c>error</c> will contain an error message. Otherwise, the bucket ID.</returns>
        private string B2ApiGetBucketId(B2Authorization auth, string bucketName, out string error)
        {
            NameValueCollection headers = new NameValueCollection()
            {
                ["Authorization"] = auth.authorizationToken
            };

            Dictionary<string, string> reqBody = new Dictionary<string, string>
            {
                ["accountId"] = auth.accountId,
                ["bucketName"] = bucketName
            };

            using (Stream data = CreateJsonBody(reqBody))
            {
                using (HttpWebResponse res = GetResponse(HttpMethod.POST, auth.apiUrl + B2ListBucketsPath,
                    contentType: ApplicationJson, headers: headers, data: data, allowNon2xxResponses: true))
                {
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        error = StringifyB2Error(res);
                        return null;
                    }

                    string body = RequestHelpers.ResponseToString(res);

                    JObject json;

                    try
                    {
                        json = JObject.Parse(body);
                    }
                    catch (JsonException e)
                    {
                        DebugHelper.WriteLine($"B2 uploader: Could not parse b2_list_buckets response: {e}");
                        error = "B2 upload failed: Couldn't parse b2_list_buckets response.";
                        return null;
                    }

                    string bucketId = json.SelectToken("buckets")?.FirstOrDefault(b => b["bucketName"].ToString() == bucketName)?.
                        SelectToken("bucketId")?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(bucketId))
                    {
                        error = null;
                        return bucketId;
                    }

                    error = $"B2 upload failed: Couldn't find bucket {bucketName}.";
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="B2UploadUrl"/> for the given bucket. Requires <c>writeFile</c> permission.
        /// </summary>
        /// <param name="auth">The B2 API authorization.</param>
        /// <param name="bucketId">The bucket ID to get an upload URL for.</param>
        /// <param name="error">Will be set to a non-null value on failure.</param>
        /// <returns>Null if an error occurs, and <c>error</c> will contain an error message. Otherwise, a <see cref="B2UploadUrl"/></returns>
        private B2UploadUrl B2ApiGetUploadUrl(B2Authorization auth, string bucketId, out string error)
        {
            NameValueCollection headers = new NameValueCollection() { ["Authorization"] = auth.authorizationToken };

            Dictionary<string, string> reqBody = new Dictionary<string, string> { ["bucketId"] = bucketId };

            using (Stream data = CreateJsonBody(reqBody))
            {
                using (HttpWebResponse res = GetResponse(HttpMethod.POST, auth.apiUrl + B2GetUploadUrlPath,
                    contentType: ApplicationJson, headers: headers, data: data, allowNon2xxResponses: true))
                {
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        error = StringifyB2Error(res);
                        return null;
                    }

                    string body = RequestHelpers.ResponseToString(res);

                    error = null;
                    return JsonConvert.DeserializeObject<B2UploadUrl>(body);
                }
            }
        }

        /// <summary>
        /// Given a <see cref="B2UploadUrl"/> returned from the API, attempts to upload a file.
        /// </summary>
        /// <param name="b2UploadUrl">Information returned by the <c>b2_get_upload_url</c> API.</param>
        /// <param name="destinationPath">The remote path to upload to.</param>
        /// <param name="file">The file to upload.</param>
        /// <returns>
        ///     A B2UploadResult(HTTP status, B2Error, B2Upload) that can be decomposed as follows:
        ///
        ///     <ul>
        ///         <li><b>If successful:</b> <c>(200, null, B2Upload)</c></li>
        ///         <li><b>If unsuccessful:</b> <c>(HTTP status, B2Error, null)</c></li>
        ///         <li><b>If the connection failed:</b> <c>(-1, null, null)</c></li>
        ///     </ul>
        /// </returns>
        private B2UploadResult B2ApiUploadFile(B2UploadUrl b2UploadUrl, string destinationPath, Stream file)
        {
            // we want to send 'Content-Disposition: inline; filename="screenshot.png"'
            // this should display the uploaded data inline if possible, but if that fails, present a sensible filename
            // conveniently, this class will handle this for us
            ContentDisposition contentDisposition = new ContentDisposition("inline") { FileName = URLHelpers.GetFileName(destinationPath) };
            DebugHelper.WriteLine($"B2 uploader: Content disposition is '{contentDisposition}'.");

            // compute SHA1 hash without loading the file fully into memory
            string sha1Hash;
            using (SHA1CryptoServiceProvider cryptoProvider = new SHA1CryptoServiceProvider())
            {
                file.Seek(0, SeekOrigin.Begin);
                byte[] bytes = cryptoProvider.ComputeHash(file);
                sha1Hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                file.Seek(0, SeekOrigin.Begin);
            }
            DebugHelper.WriteLine($"B2 uploader: SHA1 hash is '{sha1Hash}'.");

            // it's showtime
            // https://www.backblaze.com/b2/docs/b2_upload_file.html
            NameValueCollection headers = new NameValueCollection()
            {
                ["Authorization"] = b2UploadUrl.authorizationToken,
                ["X-Bz-File-Name"] = URLHelpers.URLEncode(destinationPath),
                ["Content-Length"] = file.Length.ToString(),
                ["X-Bz-Content-Sha1"] = sha1Hash,
                ["X-Bz-Info-src_last_modified_millis"] = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                ["X-Bz-Info-b2-content-disposition"] = URLHelpers.URLEncode(contentDisposition.ToString()),
            };

            string contentType = MimeTypes.GetMimeTypeFromFileName(destinationPath);

            using (HttpWebResponse res = GetResponse(HttpMethod.POST, b2UploadUrl.uploadUrl,
                contentType: contentType, headers: headers, data: file, allowNon2xxResponses: true))
            {
                // if connection failed, res will be null, and here we -do- want to check explicitly for this
                // since the server might be down
                if (res == null)
                {
                    return new B2UploadResult(-1, null, null);
                }

                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return new B2UploadResult((int)res.StatusCode, ParseB2Error(res), null);
                }

                string body = RequestHelpers.ResponseToString(res);
                DebugHelper.WriteLine($"B2 uploader: B2ApiUploadFile() reports success! '{body}'");

                return new B2UploadResult((int)res.StatusCode, null, JsonConvert.DeserializeObject<B2Upload>(body));
            }
        }

        /// <summary>
        /// Checks whether the authorization allows uploading to the specific bucket and path (without accessing the B2 API.)
        /// </summary>
        /// <param name="auth">The authorization response.</param>
        /// <param name="bucketId">The bucket to upload to.</param>
        /// <param name="destinationPath">The path of the file that will be uploaded.</param>
        /// <param name="error">Will be set to a non-null value on failure.</param>
        /// <returns>True if we have authorization for uploading, otherwise, false. Iff false, <c>error</c> will be set
        /// to an error message describing why there is no permission.</returns>
        private static bool IsAuthorizedForUpload(B2Authorization auth, string bucketId, string destinationPath, out string error)
        {
            string allowedBucketId = auth.allowed?.bucketId;
            if (allowedBucketId != null && bucketId != allowedBucketId)
            {
                DebugHelper.WriteLine($"B2 uploader: Error, user is only allowed to access '{allowedBucketId}', " +
                    $"but user is trying to access '{bucketId}'.");

                error = "No permission to upload to this bucket. Are you using the right application key?";
                return false;
            }

            string allowedPrefix = auth.allowed?.namePrefix;
            if (allowedPrefix != null && !destinationPath.StartsWith(allowedPrefix))
            {
                DebugHelper.WriteLine($"B2 uploader: Error, key is restricted to prefix '{allowedPrefix}'.");
                error = "Your upload path conflicts with the key's name prefix setting.";
                return false;
            }

            List<string> caps = auth.allowed?.capabilities;
            if (caps != null && !caps.Contains("writeFiles"))
            {
                DebugHelper.WriteLine($"B2 uploader: No permission to write to '{bucketId}'.");
                error = "Your key does not allow uploading to this bucket.";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Tries to parse a <see cref="B2Error"/> from the given response.
        /// </summary>
        /// <param name="res">The response that contains an error.</param>
        /// <returns>
        /// The parse result, or null if the response is successful or cannot be parsed.
        /// </returns>
        /// <exception cref="IOException">If the response body cannot be read.</exception>
        private static B2Error ParseB2Error(HttpWebResponse res)
        {
            if (WebHelpers.IsSuccessStatusCode(res.StatusCode))
            {
                return null;
            }

            try
            {
                string body = RequestHelpers.ResponseToString(res);
                DebugHelper.WriteLine($"B2 uploader: ParseB2Error() got: {body}");
                B2Error err = JsonConvert.DeserializeObject<B2Error>(body);
                return err;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a user facing error message from a failed B2 request.
        /// </summary>
        /// <param name="res">A <see cref="HttpWebResponse"/> with a non-2xx status code.</param>
        /// <returns>A string describing the error.</returns>
        private static string StringifyB2Error(HttpWebResponse res)
        {
            B2Error err = ParseB2Error(res);
            if (err == null)
            {
                return $"Status {res.StatusCode}, unknown error.";
            }

            string colonSpace = string.IsNullOrWhiteSpace(err.message) ? "" : ": ";
            return $"Got status {err.status} ({err.code}){colonSpace}{err.message}";
        }

        /// <summary>
        /// Takes key-value pairs and returns a Stream of data that should be sent as body for a request with
        /// <c>Content-Type: application/json; charset=utf-8</c>.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Stream CreateJsonBody(Dictionary<string, string> args)
        {
            string body = JsonConvert.SerializeObject(args);
            return new MemoryStream(Encoding.UTF8.GetBytes(body));
        }

        /// <summary>
        /// The result of <see cref="BackblazeB2.B2ApiUploadFile(B2UploadUrl, string, Stream)"/>.
        /// </summary>
        private class B2UploadResult
        {
            /// <summary>
            /// The HTTP status code.
            /// </summary>
            public int RC { get; }

            /// <summary>
            /// If not null, a value returned by the API describing what went wrong.
            /// </summary>
            public B2Error Error { get; }

            /// <summary>
            /// If <c>Error</c> is null, then this will contain the parsed API response.
            /// </summary>
            public B2Upload Upload { get; }

            public B2UploadResult(int rc, B2Error error, B2Upload upload)
            {
                RC = rc;
                Error = error;
                Upload = upload;
            }
        }

        #region JSON responses

        /// <summary>
        /// The b2_authorize_account API's optional 'allowed' field.
        /// </summary>
        private class B2Allowed
        {
            public List<string> capabilities { get; }
            public string bucketId { get; }  // may be null!
            public string namePrefix { get; } // may be null!

            public B2Allowed(List<string> capabilities, string bucketId, string namePrefix)
            {
                this.capabilities = capabilities;
                this.bucketId = bucketId;
                this.namePrefix = namePrefix;
            }
        }

        /// <summary>
        /// A parsed JSON response from the b2_authorize_account API.
        /// </summary>
        private class B2Authorization
        {
            public string accountId { get; }
            public string apiUrl { get; }
            public string authorizationToken { get; }
            public string downloadUrl { get; }
            public int minimumPartSize { get; }
            public B2Allowed allowed { get; } // optional

            public B2Authorization(string accountId, string apiUrl, string authorizationToken, string downloadUrl, int minimumPartSize, B2Allowed allowed)
            {
                this.accountId = accountId;
                this.apiUrl = apiUrl;
                this.authorizationToken = authorizationToken;
                this.downloadUrl = downloadUrl;
                this.minimumPartSize = minimumPartSize;
                this.allowed = allowed;
            }
        }

        /// <summary>
        /// A parsed JSON response from failed B2 API calls, describing the error.
        /// </summary>
        private class B2Error
        {
            public int status { get; }
            public string code { get; }
            public string message { get; }

            public B2Error(int status, string code, string message)
            {
                this.status = status;
                this.code = code;
                this.message = message;
            }
        }

        /// <summary>
        /// A parsed JSON response from the b2_get_upload_url API.
        /// </summary>
        private class B2UploadUrl
        {
            public string bucketId { get; }
            public string uploadUrl { get; }
            public string authorizationToken { get; }

            public B2UploadUrl(string bucketId, string uploadUrl, string authorizationToken)
            {
                this.bucketId = bucketId;
                this.uploadUrl = uploadUrl;
                this.authorizationToken = authorizationToken;
            }
        }

        /// <summary>
        /// A parsed JSON response from the b2_upload_file API.
        /// </summary>
        private class B2Upload
        {
            public string fileId { get; }
            public string fileName { get; }
            public string accountId { get; }
            public string bucketId { get; }
            public int contentLength { get; }
            public string contentSha1 { get; }
            public string contentType { get; }
            public Dictionary<string, string> fileInfo { get; }

            public B2Upload(string fileId, string fileName, string accountId, string bucketId,
                int contentLength, string contentSha1, string contentType, Dictionary<string, string> fileInfo)
            {
                this.fileId = fileId;
                this.fileName = fileName;
                this.accountId = accountId;
                this.bucketId = bucketId;
                this.contentLength = contentLength;
                this.contentSha1 = contentSha1;
                this.contentType = contentType;
                this.fileInfo = fileInfo;
            }
        }

        #endregion JSON responses
    }
}