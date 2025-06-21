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

using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        public override Icon ServiceIcon => Resources.AmazonS3;

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

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpAmazonS3;
    }

    public sealed class AmazonS3 : FileUploader
    {
        private const string DefaultRegion = "us-east-1";

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

        public override UploadResult Upload(Stream stream, string fileName)
        {
            bool isPathStyleRequest = Settings.UsePathStyle;

            if (!isPathStyleRequest && Settings.Bucket.Contains("."))
            {
                isPathStyleRequest = true;
            }

            string scheme = URLHelpers.GetPrefix(Settings.Endpoint);
            string endpoint = URLHelpers.RemovePrefixes(Settings.Endpoint);
            string host = isPathStyleRequest ? endpoint : $"{Settings.Bucket}.{endpoint}";
            string algorithm = "AWS4-HMAC-SHA256";
            string credentialDate = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string region = GetRegion();
            string scope = URLHelpers.CombineURL(credentialDate, region, "s3", "aws4_request");
            string credential = URLHelpers.CombineURL(Settings.AccessKeyID, scope);
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
            string contentType = MimeTypes.GetMimeTypeFromFileName(fileName);
            string hashedPayload;

            if (Settings.SignedPayload)
            {
                hashedPayload = Helpers.BytesToHex(Helpers.ComputeSHA256(stream));
            }
            else
            {
                hashedPayload = "UNSIGNED-PAYLOAD";
            }

            string uploadPath = GetUploadPath(fileName);
            string resultURL = GenerateURL(uploadPath);

            OnEarlyURLCopyRequested(resultURL);

            NameValueCollection headers = new NameValueCollection
            {
                ["Host"] = host,
                ["Content-Length"] = stream.Length.ToString(),
                ["Content-Type"] = contentType,
                ["x-amz-date"] = timeStamp,
                ["x-amz-content-sha256"] = hashedPayload,
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

            string canonicalURI = uploadPath;
            if (isPathStyleRequest) canonicalURI = URLHelpers.CombineURL(Settings.Bucket, canonicalURI);
            canonicalURI = URLHelpers.AddSlash(canonicalURI, SlashType.Prefix);
            canonicalURI = URLHelpers.URLEncode(canonicalURI, true);
            string canonicalQueryString = "";
            string canonicalHeaders = CreateCanonicalHeaders(headers);
            string signedHeaders = GetSignedHeaders(headers);

            string canonicalRequest = "PUT" + "\n" +
                canonicalURI + "\n" +
                canonicalQueryString + "\n" +
                canonicalHeaders + "\n" +
                signedHeaders + "\n" +
                hashedPayload;

            string stringToSign = algorithm + "\n" +
                timeStamp + "\n" +
                scope + "\n" +
                Helpers.BytesToHex(Helpers.ComputeSHA256(canonicalRequest));

            byte[] dateKey = Helpers.ComputeHMACSHA256(credentialDate, "AWS4" + Settings.SecretAccessKey);
            byte[] dateRegionKey = Helpers.ComputeHMACSHA256(region, dateKey);
            byte[] dateRegionServiceKey = Helpers.ComputeHMACSHA256("s3", dateRegionKey);
            byte[] signingKey = Helpers.ComputeHMACSHA256("aws4_request", dateRegionServiceKey);

            string signature = Helpers.BytesToHex(Helpers.ComputeHMACSHA256(stringToSign, signingKey));

            headers["Authorization"] = algorithm + " " +
                "Credential=" + credential + "," +
                "SignedHeaders=" + signedHeaders + "," +
                "Signature=" + signature;

            headers.Remove("Host");
            headers.Remove("Content-Type");

            string url = URLHelpers.CombineURL(scheme + host, canonicalURI);
            url = URLHelpers.FixPrefix(url);

            SendRequest(HttpMethod.PUT, url, stream, contentType, null, headers);

            if (LastResponseInfo != null && LastResponseInfo.IsSuccess)
            {
                return new UploadResult
                {
                    IsSuccess = true,
                    URL = resultURL
                };
            }

            Errors.Add("Upload to Amazon S3 failed.");
            return null;
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

                string url;

                if (Settings.UseCustomCNAME && !string.IsNullOrEmpty(Settings.CustomDomain))
                {
                    ShareXCustomUploaderSyntaxParser parser = new ShareXCustomUploaderSyntaxParser();
                    string parsedDomain = parser.Parse(Settings.CustomDomain);
                    url = URLHelpers.CombineURL(parsedDomain, uploadPath);
                }
                else
                {
                    url = URLHelpers.CombineURL(Settings.Endpoint, Settings.Bucket, uploadPath);
                }

                return URLHelpers.FixPrefix(url);
            }

            return "";
        }

        public string GetPreviewURL()
        {
            string uploadPath = GetUploadPath("example.png");
            return GenerateURL(uploadPath);
        }

        private string CreateCanonicalHeaders(NameValueCollection headers)
        {
            return headers.AllKeys.OrderBy(key => key).Select(key => key.ToLowerInvariant() + ":" + headers[key].Trim() + "\n").
                Aggregate((result, next) => result + next);
        }

        private string GetSignedHeaders(NameValueCollection headers)
        {
            return string.Join(";", headers.AllKeys.OrderBy(key => key).Select(key => key.ToLowerInvariant()));
        }
    }
}