#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

// Credits: https://github.com/alanedwardes

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.Collections.Specialized;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class AmazonS3 : FileUploader
    {
        private AmazonS3Settings s3Settings { get; set; }

        private static readonly AmazonS3Region UnknownEndpoint = new AmazonS3Region("Unknown Endpoint");
        private static readonly AmazonS3Region DreamObjectsEndpoint = new AmazonS3Region("DreamObjects", "dreamobjects", "objects.dreamhost.com");

        public static IEnumerable<AmazonS3Region> RegionEndpoints
        {
            get
            {
                yield return UnknownEndpoint;

                foreach (var endpoint in RegionEndpoint.EnumerableAllRegions)
                {
                    yield return new AmazonS3Region(endpoint);
                }

                yield return DreamObjectsEndpoint;
            }
        }

        public AmazonS3(AmazonS3Settings s3Settings)
        {
            this.s3Settings = s3Settings;
        }

        private string GetObjectStorageClass()
        {
            return s3Settings.UseReducedRedundancyStorage ? "REDUCED_REDUNDANCY" : "STANDARD";
        }

        public static AmazonS3Region GetCurrentRegion(AmazonS3Settings s3Settings)
        {
            return RegionEndpoints.SingleOrDefault(r => r.Identifier == s3Settings.Endpoint) ?? UnknownEndpoint;
        }

        private string GetEndpoint()
        {
            return URLHelpers.CombineURL("https://" + GetCurrentRegion(s3Settings).Hostname, s3Settings.Bucket);
        }

        private AWSCredentials GetCurrentCredentials()
        {
            return new BasicAWSCredentials(s3Settings.AccessKeyID, s3Settings.SecretAccessKey);
        }

        private string GetObjectKey(string fileName)
        {
            var objectPrefix = NameParser.Parse(NameParserType.FolderPath, s3Settings.ObjectPrefix.Trim('/'));
            return URLHelpers.CombineURL(objectPrefix, fileName);
        }

        private string GetObjectURL(string objectName)
        {
            objectName = objectName.Trim('/');
            objectName = URLHelpers.URLPathEncode(objectName);

            if (s3Settings.UseCustomCNAME)
            {
                string url;

                if (!string.IsNullOrEmpty(s3Settings.CustomDomain))
                {
                    url = URLHelpers.CombineURL(s3Settings.CustomDomain, objectName);
                }
                else
                {
                    url = URLHelpers.CombineURL(s3Settings.Bucket, objectName);
                }

                return URLHelpers.FixPrefix(url);
            }

            return URLHelpers.CombineURL(GetEndpoint(), objectName);
        }

        public string GetURL(string fileName)
        {
            return GetObjectURL(GetObjectKey(fileName));
        }

        public string GetMd5Hash(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (var md5 = MD5.Create())
            {
                return string.Concat(md5.ComputeHash(stream).Select(b => b.ToString("x2")));
            }
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(s3Settings.AccessKeyID)) throw new Exception("'Access Key' must not be empty.");
            if (string.IsNullOrEmpty(s3Settings.SecretAccessKey)) throw new Exception("'Secret Access Key' must not be empty.");
            if (string.IsNullOrEmpty(s3Settings.Bucket)) throw new Exception("'Bucket' must not be empty.");
            if (GetCurrentRegion(s3Settings) == UnknownEndpoint) throw new Exception("Please select an endpoint.");

            var region = GetCurrentRegion(s3Settings);

            var s3ClientConfig = new AmazonS3Config();

            if (region.AmazonRegion == null)
            {
                s3ClientConfig.ServiceURL = "https://" + region.Hostname;
            }
            else
            {
                s3ClientConfig.RegionEndpoint = region.AmazonRegion;
            }

            using (var client = new AmazonS3Client(GetCurrentCredentials(), s3ClientConfig))
            {
                var putRequest = new GetPreSignedUrlRequest
                {
                    BucketName = s3Settings.Bucket,
                    Key = GetObjectKey(fileName),
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    ContentType = Helpers.GetMimeType(fileName)
                };

                var requestHeaders = new NameValueCollection();
                requestHeaders["x-amz-acl"] = "public-read";
                requestHeaders["x-amz-storage-class"] = GetObjectStorageClass();

                putRequest.Headers["x-amz-acl"] = "public-read";
                putRequest.Headers["x-amz-storage-class"] = GetObjectStorageClass();

                var responseHeaders = SendRequestStreamGetHeaders(client.GetPreSignedURL(putRequest), stream, Helpers.GetMimeType(fileName), requestHeaders, method: HttpMethod.PUT);
                var eTag = responseHeaders["ETag"].Replace("\"", "");

                var uploadResult = new UploadResult();

                if (GetMd5Hash(stream) == eTag)
                {
                    uploadResult.IsSuccess = true;
                    uploadResult.URL = GetObjectURL(putRequest.Key);
                }
                else
                {
                    uploadResult.Errors = new List<string> { "Uploaded file is different." };
                }

                return uploadResult;
            }
        }
    }

    public class AmazonS3Region
    {
        public AmazonS3Region(string name)
        {
            Name = name;
        }

        public AmazonS3Region(string name, string identifier, string hostname)
        {
            Name = name;
            Identifier = identifier;
            Hostname = hostname;
        }

        public AmazonS3Region(RegionEndpoint region)
        {
            Name = region.DisplayName;
            Identifier = region.SystemName;
            AmazonRegion = region;
            Hostname = region.GetEndpointForService("s3").Hostname;
        }

        public string Name { get; private set; }
        public string Identifier { get; private set; }
        public RegionEndpoint AmazonRegion { get; private set; }
        public string Hostname { get; private set; }
    }

    public class AmazonS3Settings
    {
        public string AccessKeyID { get; set; }
        public string SecretAccessKey { get; set; }
        public string Endpoint { get; set; }
        public string Bucket { get; set; }
        public string ObjectPrefix { get; set; }
        public bool UseCustomCNAME { get; set; }
        public string CustomDomain { get; set; }
        public bool UseReducedRedundancyStorage { get; set; }
    }
}