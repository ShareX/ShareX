#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
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

        public static List<AmazonS3Endpoint> Endpoints { get; } = new List<AmazonS3Endpoint>()
        {
            new AmazonS3Endpoint("Asia Pacific (Tokyo)", "s3-ap-northeast-1.amazonaws.com", "ap-northeast-1"),
            new AmazonS3Endpoint("Asia Pacific (Seoul)", "s3.ap-northeast-2.amazonaws.com", "ap-northeast-2"),
            new AmazonS3Endpoint("Asia Pacific (Mumbai)", "s3.ap-south-1.amazonaws.com", "ap-south-1"),
            new AmazonS3Endpoint("Asia Pacific (Singapore)", "s3-ap-southeast-1.amazonaws.com", "ap-southeast-1"),
            new AmazonS3Endpoint("Asia Pacific (Sydney)", "s3-ap-southeast-2.amazonaws.com", "ap-southeast-2"),
            new AmazonS3Endpoint("Canada (Central)", "s3.ca-central-1.amazonaws.com", "ca-central-1"),
            new AmazonS3Endpoint("EU Central (Frankfurt)", "s3.eu-central-1.amazonaws.com", "eu-central-1"),
            new AmazonS3Endpoint("EU West (Ireland)", "s3-eu-west-1.amazonaws.com", "eu-west-1"),
            new AmazonS3Endpoint("EU West (London)", "s3.eu-west-2.amazonaws.com", "eu-west-2"),
            new AmazonS3Endpoint("South America (Sao Paulo)", "s3-sa-east-1.amazonaws.com", "sa-east-1"),
            new AmazonS3Endpoint("US East (Virginia)", "s3.amazonaws.com", "us-east-1"),
            new AmazonS3Endpoint("US East (Ohio)", "s3.us-east-2.amazonaws.com", "us-east-2"),
            new AmazonS3Endpoint("US West (N. California)", "s3-us-west-1.amazonaws.com", "us-west-1"),
            new AmazonS3Endpoint("US West (Oregon)", "s3-us-west-2.amazonaws.com", "us-west-2"),
            new AmazonS3Endpoint("China (Beijing)", "s3.cn-north-1.amazonaws.com.cn", "cn-north-1"),
            new AmazonS3Endpoint("US GovCloud West (Oregon)", "s3-us-gov-west-1.amazonaws.com", "us-gov-west-1"),
            new AmazonS3Endpoint("DreamObjects", "objects-us-west-1.dream.io")
        };

        private AmazonS3Settings Settings { get; set; }

        public AmazonS3(AmazonS3Settings settings)
        {
            Settings = settings;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string endpoint = URLHelpers.RemovePrefixes(Settings.Endpoint);
            string host = Settings.UsePathStyle ? endpoint : $"{Settings.Bucket}.{endpoint}";
            string algorithm = "AWS4-HMAC-SHA256";
            string credentialDate = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string region = GetRegion();
            string scope = URLHelpers.CombineURL(credentialDate, region, "s3", "aws4_request");
            string credential = URLHelpers.CombineURL(Settings.AccessKeyID, scope);
            string longDate = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
            string expiresTotalSeconds = ((long)TimeSpan.FromHours(1).TotalSeconds).ToString();
            string contentType = Helpers.GetMimeType(fileName);

            NameValueCollection headers = new NameValueCollection();
            headers["content-type"] = contentType;
            headers["host"] = host;
            headers["x-amz-acl"] = "public-read";
            headers["x-amz-storage-class"] = Settings.UseReducedRedundancyStorage ? "REDUCED_REDUNDANCY" : "STANDARD";

            string signedHeaders = GetSignedHeaders(headers);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("X-Amz-Algorithm", algorithm);
            args.Add("X-Amz-Credential", credential);
            args.Add("X-Amz-Date", longDate);
            args.Add("X-Amz-Expires", expiresTotalSeconds);
            args.Add("X-Amz-SignedHeaders", signedHeaders);

            string uploadPath = GetUploadPath(fileName);
            if (Settings.UsePathStyle) uploadPath = URLHelpers.CombineURL(Settings.Bucket, uploadPath);
            string canonicalURI = URLHelpers.AddSlash(uploadPath, SlashType.Prefix);
            canonicalURI = URLHelpers.URLPathEncode(canonicalURI);

            string canonicalQueryString = URLHelpers.CreateQuery(args);
            string canonicalHeaders = CreateCanonicalHeaders(headers);

            string canonicalRequest = "PUT" + "\n" +
                canonicalURI + "\n" +
                canonicalQueryString + "\n" +
                canonicalHeaders + "\n" +
                signedHeaders + "\n" +
                "UNSIGNED-PAYLOAD";

            string stringToSign = algorithm + "\n" +
                longDate + "\n" +
                scope + "\n" +
                Helpers.BytesToHex(Helpers.ComputeSHA256(canonicalRequest));

            byte[] dateKey = Helpers.ComputeHMACSHA256(credentialDate, "AWS4" + Settings.SecretAccessKey);
            byte[] dateRegionKey = Helpers.ComputeHMACSHA256(region, dateKey);
            byte[] dateRegionServiceKey = Helpers.ComputeHMACSHA256("s3", dateRegionKey);
            byte[] signingKey = Helpers.ComputeHMACSHA256("aws4_request", dateRegionServiceKey);
            string signature = Helpers.BytesToHex(Helpers.ComputeHMACSHA256(stringToSign, signingKey));

            args.Add("X-Amz-Signature", signature);

            headers.Remove("content-type");
            headers.Remove("host");

            string url = URLHelpers.CombineURL(host, canonicalURI);
            url = URLHelpers.CreateQuery(url, args);
            url = URLHelpers.ForcePrefix(url, "https://");

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, url, stream, contentType, null, headers);

            if (responseHeaders == null || responseHeaders.Count == 0)
            {
                Errors.Add("Upload to Amazon S3 failed. Check your access credentials.");
                return null;
            }

            if (responseHeaders["ETag"] == null)
            {
                Errors.Add("Upload to Amazon S3 failed.");
                return null;
            }

            return new UploadResult
            {
                IsSuccess = true,
                URL = GenerateURL(fileName)
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
            string path = NameParser.Parse(NameParserType.FolderPath, Settings.ObjectPrefix.Trim('/'));
            return URLHelpers.CombineURL(path, fileName);
        }

        public string GenerateURL(string fileName)
        {
            if (!string.IsNullOrEmpty(Settings.Endpoint) && !string.IsNullOrEmpty(Settings.Bucket))
            {
                string uploadPath = GetUploadPath(fileName);

                string url;

                if (Settings.UseCustomCNAME && !string.IsNullOrEmpty(Settings.CustomDomain))
                {
                    url = URLHelpers.CombineURL(Settings.CustomDomain, uploadPath);
                }
                else
                {
                    url = URLHelpers.CombineURL(Settings.Endpoint, Settings.Bucket, uploadPath);
                }

                return URLHelpers.FixPrefix(url, "https://");
            }

            return "";
        }

        private string CreateCanonicalHeaders(NameValueCollection headers)
        {
            string result = "";

            foreach (string key in headers)
            {
                result += key.ToLowerInvariant() + ":" + headers[key].Trim() + "\n";
            }

            return result;
        }

        private string GetSignedHeaders(NameValueCollection headers)
        {
            return string.Join(";", headers.AllKeys.Select(x => x.ToLowerInvariant()));
        }
    }
}