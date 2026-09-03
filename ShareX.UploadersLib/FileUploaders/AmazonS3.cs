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

using ShareX.HelpersLib;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ShareX.UploadersLib.FileUploaders
{
    public enum AmazonS3StorageClass
    {
        [Description("Amazon S3 Standard")]
        STANDARD,
        [Description("Amazon S3 Standard-Infrequent Access")]
        STANDARD_IA,
        [Description("Amazon S3 One Zone-Infrequent Access")]
        ONEZONE_IA,
        [Description("Amazon S3 Intelligent-Tiering")]
        INTELLIGENT_TIERING,
        //[Description("Amazon S3 Glacier")]
        //GLACIER,
        //[Description("Amazon S3 Glacier Deep Archive")]
        //DEEP_ARCHIVE
    }

    public class AmazonS3NewFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.AmazonS3;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.AmazonS3Settings != null && !string.IsNullOrEmpty(config.AmazonS3Settings.AccessKeyID) &&
                !string.IsNullOrEmpty(config.AmazonS3Settings.SecretAccessKey) && !string.IsNullOrEmpty(config.AmazonS3Settings.Endpoint) &&
                !string.IsNullOrEmpty(config.AmazonS3Settings.Bucket);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new AmazonS3(config.AmazonS3Settings);
        }
    }

    public sealed class AmazonS3 : FileUploader
    {
        private const string DefaultRegion = "us-east-1";
        private const string SignatureAlgorithm = "AWS4-HMAC-SHA256";
        private const string UnsignedPayload = "UNSIGNED-PAYLOAD";
        private const long DefaultPartSize = 100L * 1024 * 1024;
        private const long MinimumPartSize = 5L * 1024 * 1024;
        private const long MaximumPartSize = 5L * 1024 * 1024 * 1024;
        private const int MaximumPartCount = 10000;

        private sealed class AmazonS3RequestData
        {
            public string Scheme { get; }
            public string Host { get; }
            public string CanonicalURI { get; }

            public AmazonS3RequestData(string scheme, string host, string canonicalURI)
            {
                Scheme = scheme;
                Host = host;
                CanonicalURI = canonicalURI;
            }
        }

        private sealed class AmazonS3MultipartPart
        {
            public int PartNumber { get; }
            public string EntityTag { get; }

            public AmazonS3MultipartPart(int partNumber, string entityTag)
            {
                PartNumber = partNumber;
                EntityTag = entityTag;
            }
        }

        // http://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region
        public static List<AmazonS3Endpoint> Endpoints { get; } = new List<AmazonS3Endpoint>()
        {
            new AmazonS3Endpoint("Asia Pacific (Hong Kong)", "s3.ap-east-1.amazonaws.com", "ap-east-1"),
            new AmazonS3Endpoint("Asia Pacific (Mumbai)", "s3.ap-south-1.amazonaws.com", "ap-south-1"),
            new AmazonS3Endpoint("Asia Pacific (Seoul)", "s3.ap-northeast-2.amazonaws.com", "ap-northeast-2"),
            new AmazonS3Endpoint("Asia Pacific (Singapore)", "s3.ap-southeast-1.amazonaws.com", "ap-southeast-1"),
            new AmazonS3Endpoint("Asia Pacific (Sydney)", "s3.ap-southeast-2.amazonaws.com", "ap-southeast-2"),
            new AmazonS3Endpoint("Asia Pacific (Tokyo)", "s3.ap-northeast-1.amazonaws.com", "ap-northeast-1"),
            new AmazonS3Endpoint("Canada (Central)", "s3.ca-central-1.amazonaws.com", "ca-central-1"),
            new AmazonS3Endpoint("China (Beijing)", "s3.cn-north-1.amazonaws.com.cn", "cn-north-1"),
            new AmazonS3Endpoint("China (Ningxia)", "s3.cn-northwest-1.amazonaws.com.cn", "cn-northwest-1"),
            new AmazonS3Endpoint("EU (Frankfurt)", "s3.eu-central-1.amazonaws.com", "eu-central-1"),
            new AmazonS3Endpoint("EU (Ireland)", "s3.eu-west-1.amazonaws.com", "eu-west-1"),
            new AmazonS3Endpoint("EU (London)", "s3.eu-west-2.amazonaws.com", "eu-west-2"),
            new AmazonS3Endpoint("EU (Paris)", "s3.eu-west-3.amazonaws.com", "eu-west-3"),
            new AmazonS3Endpoint("EU (Stockholm)", "s3.eu-north-1.amazonaws.com", "eu-north-1"),
            new AmazonS3Endpoint("Middle East (Bahrain)", "s3.me-south-1.amazonaws.com", "me-south-1"),
            new AmazonS3Endpoint("South America (São Paulo)", "s3.sa-east-1.amazonaws.com", "sa-east-1"),
            new AmazonS3Endpoint("US East (N. Virginia)", "s3.amazonaws.com", "us-east-1"),
            new AmazonS3Endpoint("US East (Ohio)", "s3.us-east-2.amazonaws.com", "us-east-2"),
            new AmazonS3Endpoint("US West (N. California)", "s3.us-west-1.amazonaws.com", "us-west-1"),
            new AmazonS3Endpoint("US West (Oregon)", "s3.us-west-2.amazonaws.com", "us-west-2"),
            new AmazonS3Endpoint("DreamObjects", "objects-us-east-1.dream.io"),
            new AmazonS3Endpoint("DigitalOcean (Amsterdam)", "ams3.digitaloceanspaces.com", "ams3"),
            new AmazonS3Endpoint("DigitalOcean (New York)", "nyc3.digitaloceanspaces.com", "nyc3"),
            new AmazonS3Endpoint("DigitalOcean (San Francisco)", "sfo2.digitaloceanspaces.com", "sfo2"),
            new AmazonS3Endpoint("DigitalOcean (Singapore)", "sgp1.digitaloceanspaces.com", "sgp1"),
            new AmazonS3Endpoint("Wasabi", "s3.wasabisys.com")
        };

        private AmazonS3Settings Settings { get; set; }

        public AmazonS3(AmazonS3Settings settings)
        {
            Settings = settings;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            string contentType = MimeTypes.GetMimeTypeFromFileName(fileName);
            string uploadPath = GetUploadPath(fileName);
            string resultURL = GenerateURL(uploadPath);
            OnEarlyURLCopyRequested(resultURL);

            AmazonS3RequestData requestData = CreateRequestData(uploadPath);
            UploadResult result;

            if (Settings.UseMultipartUpload && stream.Length > 0)
            {
                result = await UploadMultipartAsync(stream, contentType, resultURL, requestData, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                result = await UploadSingleRequestAsync(stream, contentType, resultURL, requestData, cancellationToken).ConfigureAwait(false);
            }

            if (result == null)
            {
                Errors.Add(Localization.Strings.AmazonS3_Upload_failed);
            }

            return result;
        }

        public async Task<bool> DeleteObjectAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(objectKey);

            AmazonS3RequestData requestData = CreateRequestData(objectKey);

            using MemoryStream emptyStream = new MemoryStream(Array.Empty<byte>(), false);
            await SendAmazonS3RequestAsync(HttpMethod.DELETE, requestData, "", emptyStream, 0, 0, null, null,
                cancellationToken).ConfigureAwait(false);

            return LastResponseInfo != null && LastResponseInfo.IsSuccess;
        }

        private async Task<UploadResult> UploadSingleRequestAsync(Stream stream, string contentType, string resultURL,
            AmazonS3RequestData requestData, CancellationToken cancellationToken)
        {
            await SendAmazonS3RequestAsync(HttpMethod.PUT, requestData, "", stream, 0, stream.Length, contentType,
                CreateObjectHeaders(), cancellationToken).ConfigureAwait(false);

            return LastResponseInfo != null && LastResponseInfo.IsSuccess ? CreateUploadResult(resultURL) : null;
        }

        private async Task<UploadResult> UploadMultipartAsync(Stream stream, string contentType, string resultURL,
            AmazonS3RequestData requestData, CancellationToken cancellationToken)
        {
            long partSize = GetMultipartPartSize(stream.Length);

            if (partSize == 0)
            {
                return null;
            }

            string uploadID = null;
            bool isCompleted = false;

            try
            {
                using (MemoryStream emptyStream = new MemoryStream(Array.Empty<byte>(), false))
                {
                    string response = await SendAmazonS3RequestAsync(HttpMethod.POST, requestData, "uploads=", emptyStream, 0, 0,
                        contentType, CreateObjectHeaders(), cancellationToken).ConfigureAwait(false);
                    uploadID = GetUploadID(response);
                }

                if (string.IsNullOrEmpty(uploadID))
                {
                    return null;
                }

                List<AmazonS3MultipartPart> parts = new List<AmazonS3MultipartPart>();
                long position = 0;
                int partNumber = 1;

                while (position < stream.Length)
                {
                    long length = Math.Min(partSize, stream.Length - position);
                    string queryString = "partNumber=" + partNumber.ToString(CultureInfo.InvariantCulture) +
                        "&uploadId=" + URLHelpers.URLEncode(uploadID);

                    await SendAmazonS3RequestAsync(HttpMethod.PUT, requestData, queryString, stream, position, length,
                        contentType, null, cancellationToken).ConfigureAwait(false);

                    if (LastResponseInfo == null || !LastResponseInfo.IsSuccess)
                    {
                        return null;
                    }

                    string entityTag = LastResponseInfo.Headers?["ETag"];

                    if (string.IsNullOrWhiteSpace(entityTag))
                    {
                        return null;
                    }

                    parts.Add(new AmazonS3MultipartPart(partNumber, entityTag.Trim()));
                    position += length;
                    partNumber++;
                }

                byte[] completePayload = CreateCompleteMultipartUploadPayload(parts);

                using (MemoryStream completeStream = new MemoryStream(completePayload, false))
                {
                    string queryString = "uploadId=" + URLHelpers.URLEncode(uploadID);
                    string response = await SendAmazonS3RequestAsync(HttpMethod.POST, requestData, queryString, completeStream, 0,
                        completeStream.Length, "application/xml", null, cancellationToken).ConfigureAwait(false);

                    if (LastResponseInfo == null || !LastResponseInfo.IsSuccess || IsAmazonS3ErrorResponse(response))
                    {
                        return null;
                    }
                }

                isCompleted = true;
                return CreateUploadResult(resultURL);
            }
            finally
            {
                if (!isCompleted && !string.IsNullOrEmpty(uploadID))
                {
                    await AbortMultipartUploadAsync(requestData, uploadID).ConfigureAwait(false);
                }
            }
        }

        private async Task AbortMultipartUploadAsync(AmazonS3RequestData requestData, string uploadID)
        {
            ResponseInfo responseInfo = LastResponseInfo;

            try
            {
                using CancellationTokenSource abortCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using MemoryStream emptyStream = new MemoryStream(Array.Empty<byte>(), false);
                string queryString = "uploadId=" + URLHelpers.URLEncode(uploadID);
                await SendAmazonS3RequestAsync(HttpMethod.DELETE, requestData, queryString, emptyStream, 0, 0, null, null,
                    abortCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                LastResponseInfo = responseInfo;
            }
        }

        private async Task<string> SendAmazonS3RequestAsync(HttpMethod method, AmazonS3RequestData requestData,
            string canonicalQueryString, Stream stream, long position, long length, string contentType,
            NameValueCollection requestHeaders, CancellationToken cancellationToken)
        {
            string hashedPayload = GetPayloadHash(stream, position, length, cancellationToken);
            DateTime requestTime = DateTime.UtcNow;
            string credentialDate = requestTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string timeStamp = requestTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
            string region = GetRegion();
            string scope = URLHelpers.CombineURL(credentialDate, region, "s3", "aws4_request");
            string credential = URLHelpers.CombineURL(Settings.AccessKeyID, scope);
            NameValueCollection headers = requestHeaders != null
                ? new NameValueCollection(requestHeaders)
                : new NameValueCollection();

            headers["Host"] = requestData.Host;
            headers["Content-Length"] = length.ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(contentType))
            {
                headers["Content-Type"] = contentType;
            }

            headers["x-amz-date"] = timeStamp;
            headers["x-amz-content-sha256"] = hashedPayload;

            string canonicalHeaders = CreateCanonicalHeaders(headers);
            string signedHeaders = GetSignedHeaders(headers);
            string canonicalRequest = method + "\n" +
                requestData.CanonicalURI + "\n" +
                canonicalQueryString + "\n" +
                canonicalHeaders + "\n" +
                signedHeaders + "\n" +
                hashedPayload;
            string stringToSign = SignatureAlgorithm + "\n" +
                timeStamp + "\n" +
                scope + "\n" +
                Helpers.BytesToHex(Helpers.ComputeSHA256(canonicalRequest));

            byte[] dateKey = Helpers.ComputeHMACSHA256(credentialDate, "AWS4" + Settings.SecretAccessKey);
            byte[] dateRegionKey = Helpers.ComputeHMACSHA256(region, dateKey);
            byte[] dateRegionServiceKey = Helpers.ComputeHMACSHA256("s3", dateRegionKey);
            byte[] signingKey = Helpers.ComputeHMACSHA256("aws4_request", dateRegionServiceKey);
            string signature = Helpers.BytesToHex(Helpers.ComputeHMACSHA256(stringToSign, signingKey));

            headers["Authorization"] = SignatureAlgorithm + " " +
                "Credential=" + credential + "," +
                "SignedHeaders=" + signedHeaders + "," +
                "Signature=" + signature;

            headers.Remove("Host");
            headers.Remove("Content-Type");

            string url = GetRequestURL(requestData, canonicalQueryString);
            return await SendRequestAsync(method, url, stream, position, length, contentType, null, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private AmazonS3RequestData CreateRequestData(string uploadPath)
        {
            bool isPathStyleRequest = Settings.UsePathStyle || Settings.Bucket.Contains(".");
            string scheme = URLHelpers.GetPrefix(Settings.Endpoint);
            string endpoint = URLHelpers.RemovePrefixes(Settings.Endpoint).TrimEnd('/');
            string host = isPathStyleRequest ? endpoint : $"{Settings.Bucket}.{endpoint}";
            string canonicalURI = isPathStyleRequest ? URLHelpers.CombineURL(Settings.Bucket, uploadPath) : uploadPath;
            canonicalURI = URLHelpers.AddSlash(canonicalURI, SlashType.Prefix);
            canonicalURI = URLHelpers.URLEncode(canonicalURI, true);
            return new AmazonS3RequestData(scheme, host, canonicalURI);
        }

        private static string GetRequestURL(AmazonS3RequestData requestData, string canonicalQueryString)
        {
            string url = URLHelpers.CombineURL(requestData.Scheme + requestData.Host, requestData.CanonicalURI);
            url = URLHelpers.FixPrefix(url);

            if (!string.IsNullOrEmpty(canonicalQueryString))
            {
                url += "?" + canonicalQueryString;
            }

            return url;
        }

        private NameValueCollection CreateObjectHeaders()
        {
            NameValueCollection headers = new NameValueCollection
            {
                // If you don't specify, S3 Standard is the default storage class. Amazon S3 supports other storage classes.
                // Valid Values: STANDARD | REDUCED_REDUNDANCY | STANDARD_IA | ONEZONE_IA | INTELLIGENT_TIERING | GLACIER | DEEP_ARCHIVE
                // https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObject.html
                ["x-amz-storage-class"] = Settings.StorageClass.ToString()
            };

            if (Settings.SetPublicACL)
            {
                // The canned ACL to apply to the object. For more information, see Canned ACL.
                // https://docs.aws.amazon.com/AmazonS3/latest/dev/acl-overview.html#canned-acl
                headers["x-amz-acl"] = "public-read";
            }

            return headers;
        }

        private string GetPayloadHash(Stream stream, long position, long length, CancellationToken cancellationToken)
        {
            if (!Settings.SignedPayload)
            {
                return UnsignedPayload;
            }

            long originalPosition = stream.Position;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(Math.Max(1, BufferSize));

            try
            {
                stream.Position = position;
                long remaining = length;

                using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

                while (remaining > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int count = (int)Math.Min(buffer.Length, remaining);
                    int bytesRead = stream.Read(buffer, 0, count);

                    if (bytesRead == 0)
                    {
                        throw new EndOfStreamException("The upload stream ended before the payload hash was calculated.");
                    }

                    hash.AppendData(buffer, 0, bytesRead);
                    remaining -= bytesRead;
                }

                return Helpers.BytesToHex(hash.GetHashAndReset());
            }
            finally
            {
                stream.Position = originalPosition;
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private static long GetMultipartPartSize(long streamLength)
        {
            if (streamLength <= 0)
            {
                return 0;
            }

            long requiredPartSize = streamLength / MaximumPartCount;

            if (streamLength % MaximumPartCount != 0)
            {
                requiredPartSize++;
            }

            long partSize = Math.Max(DefaultPartSize, Math.Max(MinimumPartSize, requiredPartSize));
            long remainder = partSize % (1024 * 1024);

            if (remainder != 0)
            {
                partSize += 1024 * 1024 - remainder;
            }

            return partSize <= MaximumPartSize ? partSize : 0;
        }

        private static string GetUploadID(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }

            try
            {
                XDocument document = XDocument.Parse(response);
                return document.Root?.DescendantsAndSelf().FirstOrDefault(x => x.Name.LocalName == "UploadId")?.Value.Trim();
            }
            catch (XmlException)
            {
                return null;
            }
        }

        private static byte[] CreateCompleteMultipartUploadPayload(IEnumerable<AmazonS3MultipartPart> parts)
        {
            XElement document = new XElement("CompleteMultipartUpload",
                parts.Select(part => new XElement("Part",
                    new XElement("PartNumber", part.PartNumber),
                    new XElement("ETag", part.EntityTag))));
            return Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting));
        }

        private static bool IsAmazonS3ErrorResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return false;
            }

            try
            {
                XDocument document = XDocument.Parse(response);
                return document.Root?.DescendantsAndSelf().Any(x => x.Name.LocalName == "Error") == true;
            }
            catch (XmlException)
            {
                return true;
            }
        }

        private static UploadResult CreateUploadResult(string resultURL)
        {
            return new UploadResult
            {
                IsSuccess = true,
                URL = resultURL
            };
        }

        private string GetRegion()
        {
            if (!string.IsNullOrEmpty(Settings.Region))
            {
                return Settings.Region;
            }

            string url = Settings.Endpoint;

            int delimIndex = url.IndexOf("//", StringComparison.Ordinal);
            if (delimIndex >= 0)
            {
                url = url.Substring(delimIndex + 2);
            }

            if (url.EndsWith("/", StringComparison.Ordinal))
            {
                url = url.Substring(0, url.Length - 1);
            }

            int awsIndex = url.IndexOf(".amazonaws.com", StringComparison.Ordinal);
            if (awsIndex < 0)
            {
                return DefaultRegion;
            }

            string serviceAndRegion = url.Substring(0, awsIndex);
            if (serviceAndRegion.StartsWith("s3-", StringComparison.Ordinal))
            {
                serviceAndRegion = "s3." + serviceAndRegion.Substring(3);
            }

            int separatorIndex = serviceAndRegion.LastIndexOf('.');
            if (separatorIndex == -1)
            {
                return DefaultRegion;
            }

            return serviceAndRegion.Substring(separatorIndex + 1);
        }

        private string GetUploadPath(string fileName)
        {
            string path = NameParser.Parse(NameParserType.FilePath, Settings.ObjectPrefix.Trim('/'));

            if ((Settings.RemoveExtensionImage && FileHelpers.IsImageFile(fileName)) ||
                (Settings.RemoveExtensionText && FileHelpers.IsTextFile(fileName)) ||
                (Settings.RemoveExtensionVideo && FileHelpers.IsVideoFile(fileName)))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            return URLHelpers.CombineURL(path, fileName);
        }

        public string GenerateURL(string uploadPath)
        {
            if (!string.IsNullOrEmpty(Settings.Endpoint) && !string.IsNullOrEmpty(Settings.Bucket))
            {
                uploadPath = URLHelpers.URLEncode(uploadPath, true, HelpersOptions.URLEncodeIgnoreEmoji);
                return URLHelpers.CombineURL(GetObjectBaseURL(), uploadPath);
            }

            return "";
        }

        public bool TryGetObjectKey(string url, out string objectKey)
        {
            objectKey = null;

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri objectURI) ||
                !Uri.TryCreate(GetObjectBaseURL(), UriKind.Absolute, out Uri baseURI) ||
                !string.Equals(objectURI.Scheme, baseURI.Scheme, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(objectURI.IdnHost, baseURI.IdnHost, StringComparison.OrdinalIgnoreCase) ||
                objectURI.Port != baseURI.Port)
            {
                return false;
            }

            string basePath = baseURI.AbsolutePath.TrimEnd('/');
            string objectPath = objectURI.AbsolutePath;

            if (!objectPath.StartsWith(basePath + "/", StringComparison.Ordinal))
            {
                return false;
            }

            string escapedObjectKey = objectPath.Substring(basePath.Length + 1);

            if (string.IsNullOrEmpty(escapedObjectKey))
            {
                return false;
            }

            objectKey = Uri.UnescapeDataString(escapedObjectKey);
            return !string.IsNullOrEmpty(objectKey);
        }

        private string GetObjectBaseURL()
        {
            string url;

            if (Settings.UseCustomCNAME && !string.IsNullOrEmpty(Settings.CustomDomain))
            {
                ShareXCustomUploaderSyntaxParser parser = new ShareXCustomUploaderSyntaxParser();
                url = parser.Parse(Settings.CustomDomain);
            }
            else
            {
                url = URLHelpers.CombineURL(Settings.Endpoint, Settings.Bucket);
            }

            return URLHelpers.FixPrefix(url);
        }

        public string GetPreviewURL()
        {
            string uploadPath = GetUploadPath("example.png");
            return GenerateURL(uploadPath);
        }

        private string CreateCanonicalHeaders(NameValueCollection headers)
        {
            return string.Concat(headers.AllKeys.OrderBy(key => key, StringComparer.Ordinal).
                Select(key => key.ToLowerInvariant() + ":" + headers[key].Trim() + "\n"));
        }

        private string GetSignedHeaders(NameValueCollection headers)
        {
            return string.Join(";", headers.AllKeys.OrderBy(key => key, StringComparer.Ordinal).Select(key => key.ToLowerInvariant()));
        }
    }
}
