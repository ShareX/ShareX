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
using System.Security.Cryptography;
using System.Text;
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
                !string.IsNullOrEmpty(config.AmazonS3Settings.SecretAccessKey) && !string.IsNullOrEmpty(config.AmazonS3Settings.Bucket) &&
                !string.IsNullOrEmpty(config.AmazonS3Settings.Endpoint);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new AmazonS3(config.AmazonS3Settings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpAmazonS3;
    }

    public sealed class AmazonS3 : FileUploader
    {
        private AmazonS3Settings Settings { get; set; }

        public AmazonS3(AmazonS3Settings settings)
        {
            Settings = settings;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string host = $"{Settings.Bucket}.s3.{Settings.Endpoint}.amazonaws.com";
            string algorithm = "AWS4-HMAC-SHA256";
            string credentialDate = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string scope = $"{credentialDate}/{Settings.Endpoint}/s3/aws4_request";
            string credential = $"{Settings.AccessKeyID}/{scope}";
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
            string canonicalURI = URLHelpers.AddSlash(uploadPath, SlashType.Prefix);
            canonicalURI = URLHelpers.URLPathEncode(canonicalURI);

            string canonicalQueryString = CreateQueryString(args);
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
                BytesToHex(ComputeHash(canonicalRequest));

            byte[] secretKey = Encoding.UTF8.GetBytes("AWS4" + Settings.SecretAccessKey);
            byte[] dateKey = ComputeHMAC(Encoding.UTF8.GetBytes(credentialDate), secretKey);
            byte[] dateRegionKey = ComputeHMAC(Encoding.UTF8.GetBytes(Settings.Endpoint), dateKey);
            byte[] dateRegionServiceKey = ComputeHMAC(Encoding.UTF8.GetBytes("s3"), dateRegionKey);
            byte[] signingKey = ComputeHMAC(Encoding.UTF8.GetBytes("aws4_request"), dateRegionServiceKey);
            string signature = BytesToHex(ComputeHMAC(Encoding.UTF8.GetBytes(stringToSign), signingKey));

            args.Add("X-Amz-Signature", signature);

            headers.Remove("content-type");
            headers.Remove("host");

            string url = URLHelpers.CombineURL(host, canonicalURI) + "?" + CreateQueryString(args);
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

        private string GetUploadPath(string fileName)
        {
            string path = NameParser.Parse(NameParserType.FolderPath, Settings.ObjectPrefix.Trim('/'));
            return URLHelpers.CombineURL(path, fileName);
        }

        public string GenerateURL(string fileName)
        {
            string uploadPath = GetUploadPath(fileName);
            return URLHelpers.CombineURL($"https://s3.{Settings.Endpoint}.amazonaws.com", Settings.Bucket, uploadPath);
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

        private byte[] ComputeHash(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            return hash;
        }

        private byte[] ComputeHMAC(byte[] data, byte[] key)
        {
            using (HashAlgorithm hashAlgorithm = new HMACSHA256(key))
            {
                return hashAlgorithm.ComputeHash(data);
            }
        }

        private string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in bytes)
            {
                sb.Append(string.Format("{0:x2}", x));
            }
            return sb.ToString();
        }

        private string CreateQueryString(Dictionary<string, string> args)
        {
            if (args != null && args.Count > 0)
            {
                return string.Join("&", args.Select(x => x.Key + "=" + URLHelpers.URLEncode(x.Value)).ToArray());
            }

            return "";
        }
    }
}