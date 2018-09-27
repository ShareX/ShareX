#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
            return
                !string.IsNullOrWhiteSpace(config.B2ApplicationKeyId) &&
                !string.IsNullOrWhiteSpace(config.B2ApplicationKey) &&
                !string.IsNullOrWhiteSpace(config.B2BucketName);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new BackblazeB2(
                applicationKeyId: config.B2ApplicationKeyId,
                applicationKey: config.B2ApplicationKey,
                bucketName: config.B2BucketName,
                uploadPath: config.B2UploadPath,
                useCustomUrl: config.B2UseCustomUrl,
                customUrl: config.B2CustomUrl
                );
        }
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
            var parsedUploadPath = NameParser.Parse(NameParserType.FolderPath, UploadPath);
            var destinationPath = parsedUploadPath + fileName;

            // docs: https://www.backblaze.com/b2/docs/

            // STEP 1: authorize, get auth token, api url, download url
            DebugHelper.WriteLine($"B2 uploader: Attempting to authorize as '{ApplicationKeyId}'.");
            var (authError, auth) = B2ApiAuthorize(ApplicationKeyId, ApplicationKey);
            if (authError != null)
            {
                DebugHelper.WriteLine("B2 uploader: Failed to authorize.");
                Errors.Add("Could not authenticate with B2: " + authError);
                return null;
            }

            DebugHelper.WriteLine($"B2 uploader: Authorized, using API server {auth.apiUrl}, download URL {auth.downloadUrl}");

            // STEP 1.25: if we have an application key, there will be a bucketId present here, but if
            //            not, we have an account key and need to find our bucket id ourselves
            var bucketId = auth.allowed?.bucketId;
            if (bucketId == null)
            {
                DebugHelper.WriteLine("B2 uploader: This doesn't look like an app key, so I'm looking for a bucket ID.");

                var (getBucketError, newBucketId) = B2ApiGetBucketId(auth, BucketName);
                if (getBucketError != null)
                {
                    DebugHelper.WriteLine($"B2 uploader: It's {newBucketId}.");
                    bucketId = newBucketId;
                }
            }

            // STEP 1.5: verify whether we can write to the bucket user wants to write to, with the given prefix
            DebugHelper.WriteLine("B2 uploader: Checking clientside whether we have permission to upload.");
            var (authCheckError, authCheckOk) = IsAuthorizedForUpload(auth, bucketId, destinationPath);
            if (!authCheckOk)
            {
                DebugHelper.WriteLine("B2 uploader: Key is not suitable for this upload.");
                Errors.Add("B2 upload failed: " + authCheckError);
                return null;
            }

            // STEP 1.75: start upload attempt loop
            const int maxTries = 5;
            B2UploadUrl url = null;
            for (var tries = 1; tries <= maxTries; tries++)
            {
                var newOrSameUrl = (url == null) ? "New URL." : "Same URL.";
                DebugHelper.WriteLine($"B2 uploader: Upload attempt {tries} of {maxTries}. {newOrSameUrl}");

                // Sloppy
                if (tries > 1)
                {
                    var delay = (int)Math.Pow(2, tries - 1) * 1000;
                    DebugHelper.WriteLine($"Waiting ${delay} ms for backoff.");
                    Thread.Sleep(delay);
                }

                // STEP 2: get upload url that we need to POST to in step 3
                if (url == null)
                {
                    DebugHelper.WriteLine("B2 uploader: Getting new upload URL.");
                    string getUrlError;
                    (getUrlError, url) = B2ApiGetUploadUrl(auth, bucketId);
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
                var (status, uploadError, upload) = B2ApiUploadFile(url, destinationPath, stream);
                var expiredTokenCodes = new HashSet<string>(new List<string> { "expired_auth_token", "bad_auth_token" });

                if (status == -1)
                {
                    // magic number for "connection failed", should also happen when upload
                    // caps are exceeded
                    DebugHelper.WriteLine("B2 uploader: Connection failed, trying with new URL.");
                    url = null;
                    continue;
                }
                else if (status == 401 && expiredTokenCodes.Contains(uploadError.code))
                {
                    // Unauthorized, our token expired
                    DebugHelper.WriteLine("B2 uploader: Upload auth token expired, trying with new URL.");
                    url = null;
                    continue;
                }
                else if (status == 408)
                {
                    DebugHelper.WriteLine("B2 uploader: Request Timeout, trying with same URL.");
                    continue;
                }
                else if (status == 429)
                {
                    DebugHelper.WriteLine("B2 uploader: Too Many Requests, trying with same URL.");
                    continue;
                }
                else if (status != 200)
                {
                    // something else happened that wasn't a success, so bail out
                    DebugHelper.WriteLine("B2 uploader: Unknown error, upload failure.");
                    Errors.Add("B2 uploader: Unknown error occurred while calling b2_upload_file().");
                    return null;
                }

                // success!
                // STEP 4: compose:
                //           the download url (e.g. "https://f700.backblazeb2.com")
                //           /file/$bucket/$uploadPath
                //         or
                //           $customUrl/$uploadPath

                var remoteLocation = auth.downloadUrl +
                                     "/file/" +
                                     URLHelpers.URLEncode(BucketName) +
                                     "/" +
                                     upload.fileName;

                DebugHelper.WriteLine($"B2 uploader: Successful upload! File should be at: {remoteLocation}");

                if (UseCustomUrl)
                {
                    var parsedCustomUrl = NameParser.Parse(NameParserType.FolderPath, CustomUrl);
                    remoteLocation = parsedCustomUrl + upload.fileName;
                    DebugHelper.WriteLine($"B2 uploader: But user requested custom URL, which will be: {remoteLocation}");
                }

                return new UploadResult()
                {
                    IsSuccess = true,
                    URL = remoteLocation
                };
            }

            DebugHelper.WriteLine($"B2 uploader: Ran out of attempts, aborting.");
            Errors.Add($"B2 upload failed: Could not upload file after {maxTries} attempts.");
            return null;
        }

        /// <summary>
        /// Attempts to authorize against the B2 API with the given key.
        /// </summary>
        /// <param name="keyId">The application key ID <b>or</b> account ID.</param>
        /// <param name="key">The application key <b>or</b> account master key.</param>
        /// <returns>A tuple with either an error set, or a <see cref="B2Authorization"/>.</returns>
        private (string error, B2Authorization res) B2ApiAuthorize(string keyId, string key)
        {
            var base64Key = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{key}"));
            var headers = new NameValueCollection() { ["Authorization"] = $"Basic {base64Key}" };

            using (var res = GetResponse(HttpMethod.GET, B2AuthorizeAccountUrl, headers: headers, allowNon2xxResponses: true))
            {
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return (StringifyB2Error(res), null);
                }

                var body = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                return (null, JsonConvert.DeserializeObject<B2Authorization>(body));
            }
        }

        /// <summary>
        /// Gets the bucket ID for the given bucket name. Requires <c>listBuckets</c> permission.
        /// </summary>
        /// <param name="auth">The B2 API authorization.</param>
        /// <param name="bucketName">The bucket to get the ID for.</param>
        /// <returns><c>(null, "bucket id")</c> on success, <c>("error message", null)</c> on failure.</returns>
        private (string error, string id) B2ApiGetBucketId(B2Authorization auth, string bucketName)
        {
            var headers = new NameValueCollection()
            {
                ["Authorization"] = auth.authorizationToken
            };

            var reqBody = new Dictionary<string, string> { ["bucketName"] = bucketName };

            using (var data = CreateJsonBody(reqBody))
            {
                using (var res = GetResponse(HttpMethod.POST, auth.apiUrl + B2ListBucketsPath, contentType: ApplicationJson, headers: headers, data: data, allowNon2xxResponses: true))
                {
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        return (StringifyB2Error(res), null);
                    }

                    var body = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();

                    JObject json;

                    try
                    {
                        json = JObject.Parse(body);
                    }
                    catch (JsonException e)
                    {
                        DebugHelper.WriteLine($"B2 uploader: Could not parse b2_list_buckets response: {e}");
                        return ("B2 upload failed: Couldn't parse b2_list_buckets response.", null);
                    }

                    var bucketId = json
                        .SelectToken("buckets")
                        ?.FirstOrDefault(b => b["bucketName"].ToString() == bucketName)
                        ?.SelectToken("bucketId")?.ToString() ?? "";

                    if (bucketId != "")
                    {
                        return (null, bucketId);
                    }

                    return ($"B2 upload failed: Couldn't find bucket {bucketName}.", null);
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="B2UploadUrl"/> for the given bucket. Requires <c>writeFile</c> permission.
        /// </summary>
        /// <param name="auth">The B2 API authorization.</param>
        /// <param name="bucketId">The bucket ID to get an upload URL for.</param>
        /// <returns><c>(null, B2UploadUrl)</c> on success, <c>("error message", null)</c> on failure.</returns>
        private (string error, B2UploadUrl url) B2ApiGetUploadUrl(B2Authorization auth, string bucketId)
        {
            var headers = new NameValueCollection() { ["Authorization"] = auth.authorizationToken };

            var reqBody = new Dictionary<string, string> { ["bucketId"] = bucketId };

            using (var data = CreateJsonBody(reqBody))
            {
                using (var res = GetResponse(HttpMethod.POST, auth.apiUrl + B2GetUploadUrlPath, contentType: ApplicationJson, headers: headers, data: data, allowNon2xxResponses: true))
                {
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        return (StringifyB2Error(res), null);
                    }

                    var body = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    return (null, JsonConvert.DeserializeObject<B2UploadUrl>(body));
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
        ///     <ul>
        ///         <li><b>If successful:</b> <c>(200, null, B2Upload)</c></li>
        ///         <li><b>If unsuccessful:</b> <c>(HTTP status, B2Error, null)</c></li>
        ///         <li><b>If the connection failed:</b> <c>(-1, null, null)</c></li>
        ///     </ul>
        /// </returns>
        private (int rc, B2Error error, B2Upload upload) B2ApiUploadFile(B2UploadUrl b2UploadUrl, string destinationPath, Stream file)
        {
            // we want to send 'Content-Disposition: inline; filename="screenshot.png"'
            // this should display the uploaded data inline if possible, but if that fails, present a sensible filename
            // conveniently, this class will handle this for us
            var contentDisposition = new ContentDisposition("inline") { FileName = URLHelpers.GetFileName(destinationPath) };
            DebugHelper.WriteLine($"B2 uploader: Content disposition is '{contentDisposition}'.");

            // compute SHA1 hash without loading the file fully into memory
            string sha1Hash;
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                file.Seek(0, SeekOrigin.Begin);
                var bytes = cryptoProvider.ComputeHash(file);
                sha1Hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                file.Seek(0, SeekOrigin.Begin);
            }
            DebugHelper.WriteLine($"B2 uploader: SHA1 hash is '{sha1Hash}'.");

            // it's showtime
            // https://www.backblaze.com/b2/docs/b2_upload_file.html
            var headers = new NameValueCollection()
            {
                ["Authorization"] = b2UploadUrl.authorizationToken,
                ["X-Bz-File-Name"] = URLHelpers.URLEncode(destinationPath),
                ["Content-Length"] = file.Length.ToString(),
                ["X-Bz-Content-Sha1"] = sha1Hash,
                ["X-Bz-Info-src_last_modified_millis"] = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                ["X-Bz-Info-b2-content-disposition"] = URLHelpers.URLEncode(contentDisposition.ToString()),
            };

            var contentType = Helpers.GetMimeType(destinationPath);

            using (var res = GetResponse(HttpMethod.POST, b2UploadUrl.uploadUrl, contentType: contentType, headers: headers, data: file, allowNon2xxResponses: true))
            {
                // if connection failed, res will be null, and here we -do- want to check explicitly for this
                // since the server might be down
                if (res == null)
                {
                    return (-1, null, null);
                }

                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return ((int)res.StatusCode, ParseB2Error(res), null);
                }

                var body = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                DebugHelper.WriteLine($"B2 uploader: B2ApiUploadFile() reports success! '{body}'");

                return ((int)res.StatusCode, null, JsonConvert.DeserializeObject<B2Upload>(body));
            }
        }

        /// <summary>
        /// Checks whether the authorization allows uploading to the specific bucket and path.
        /// </summary>
        /// <param name="auth">The authorization response.</param>
        /// <param name="bucket">The bucket to upload to.</param>
        /// <param name="destinationPath">The path of the file that will be uploaded.</param>
        /// <returns><c>("error message", false)</c> on failure, <c>("", true)</c> on success.</returns>
        private static (string error, bool success) IsAuthorizedForUpload(B2Authorization auth, string bucketId, string destinationPath)
        {
            var allowedBucketId = auth.allowed?.bucketId;
            if (allowedBucketId != null && bucketId != allowedBucketId)
            {
                DebugHelper.WriteLine($"B2 uploader: Error, user is only allowed to access '{allowedBucketId}', " +
                                      $"but user is trying to access '{bucketId}'.");

                return ($"No permission to upload to this bucket. Are you using the right application key?", false);
            }

            var allowedPrefix = auth?.allowed?.namePrefix;
            if (allowedPrefix != null && !destinationPath.StartsWith(allowedPrefix))
            {
                DebugHelper.WriteLine($"B2 uploader: Error, key is restricted to prefix '{allowedPrefix}'.");
                return ("Your upload path conflicts with the key's name prefix setting.", false);
            }

            var caps = auth.allowed?.capabilities;
            if (caps != null && !caps.Contains("writeFiles"))
            {
                DebugHelper.WriteLine($"B2 uploader: No permission to write to '{bucketId}'.");
                return ("Your key does not allow uploading to this bucket.", false);
            }

            return (null, true);
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
            if (Helpers.IsSuccessfulResponse(res)) return null;

            try
            {
                var body = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                DebugHelper.WriteLine($"B2 uploader: ParseB2Error() got: {body}");
                var err = JsonConvert.DeserializeObject<B2Error>(body);
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
            var err = ParseB2Error(res);
            if (err == null)
            {
                return $"Status {res.StatusCode}, unknown error.";
            }

            var colonSpace = string.IsNullOrWhiteSpace(err.message) ? "" : ": ";
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
            var body = JsonConvert.SerializeObject(args);
            return new MemoryStream(Encoding.UTF8.GetBytes(body));
        }

        /// <summary>
        /// The b2_authorize_account API's optional 'allowed' field.
        /// </summary>
        private class B2Allowed
        {
            public B2Allowed(List<string> capabilities, string bucketId, string namePrefix)
            {
                this.capabilities = capabilities;
                this.bucketId = bucketId;
                this.namePrefix = namePrefix;
            }

            public List<string> capabilities { get; }

            public string bucketId { get; }  // may be null!

            public string namePrefix { get; } // may be null!
        }

        /// <summary>
        /// A parsed JSON response from the b2_authorize_account API.
        /// </summary>
        private class B2Authorization
        {
            public B2Authorization(string accountId, string apiUrl, string authorizationToken, string downloadUrl, int minimumPartSize, B2Allowed allowed)
            {
                this.accountId = accountId;
                this.apiUrl = apiUrl;
                this.authorizationToken = authorizationToken;
                this.downloadUrl = downloadUrl;
                this.minimumPartSize = minimumPartSize;
                this.allowed = allowed;
            }

            public string accountId { get; }
            public string apiUrl { get; }
            public string authorizationToken { get; }
            public string downloadUrl { get; }
            public int minimumPartSize { get; }
            public B2Allowed allowed { get; } // optional
        }

        /// <summary>
        /// A parsed JSON response from failed B2 API calls, describing the error.
        /// </summary>
        private class B2Error
        {
            public B2Error(int status, string code, string message)
            {
                this.status = status;
                this.code = code;
                this.message = message;
            }

            public int status { get; }
            public string code { get; }
            public string message { get; }
        }

        /// <summary>
        /// A parsed JSON response from the b2_get_upload_url API.
        /// </summary>
        private class B2UploadUrl
        {
            public B2UploadUrl(string bucketId, string uploadUrl, string authorizationToken)
            {
                this.bucketId = bucketId;
                this.uploadUrl = uploadUrl;
                this.authorizationToken = authorizationToken;
            }

            public string bucketId { get; }
            public string uploadUrl { get; }
            public string authorizationToken { get; }
        }

        /// <summary>
        /// A parsed JSON response from the b2_upload_file API.
        /// </summary>
        private class B2Upload
        {
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

            public string fileId { get; }
            public string fileName { get; }
            public string accountId { get; }
            public string bucketId { get; }
            public int contentLength { get; }
            public string contentSha1 { get; }
            public string contentType { get; }
            public Dictionary<string, string> fileInfo { get; }
        }
    }
}