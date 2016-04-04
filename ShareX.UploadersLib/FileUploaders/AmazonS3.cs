#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ShareX.UploadersLib.FileUploaders
{
    public class AmazonS3FileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.AmazonS3;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.AmazonS3Settings != null && !string.IsNullOrEmpty(config.AmazonS3Settings.AccessKeyID) &&
                !string.IsNullOrEmpty(config.AmazonS3Settings.SecretAccessKey) && !string.IsNullOrEmpty(config.AmazonS3Settings.Bucket) &&
                AmazonS3.GetCurrentRegion(config.AmazonS3Settings) != AmazonS3.UnknownEndpoint;
        }

        public override FileUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new AmazonS3(config.AmazonS3Settings);
        }
    }

    public sealed class AmazonS3 : FileUploader
    {
        public static readonly AmazonS3Region UnknownEndpoint = new AmazonS3Region("Unknown Endpoint");
        public static readonly AmazonS3Region DreamObjectsEndpoint = new AmazonS3Region("DreamObjects", "dreamobjects", "objects.dreamhost.com");

        private static IList<AmazonS3Region> regionEndpoints = new List<AmazonS3Region>();

        public static IEnumerable<AmazonS3Region> RegionEndpoints
        {
            get
            {
                if (!regionEndpoints.Any())
                {
                    regionEndpoints.Add(UnknownEndpoint);
                    RegionEndpoint.EnumerableAllRegions.Select(r => new AmazonS3Region(r)).ForEach(regionEndpoints.Add);
                    regionEndpoints.Add(DreamObjectsEndpoint);
                }

                return regionEndpoints;
            }
        }

        private AmazonS3Settings s3Settings { get; set; }

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
            if (string.IsNullOrEmpty(s3Settings.AccessKeyID)) Errors.Add("'Access Key' must not be empty.");
            if (string.IsNullOrEmpty(s3Settings.SecretAccessKey)) Errors.Add("'Secret Access Key' must not be empty.");
            if (string.IsNullOrEmpty(s3Settings.Bucket)) Errors.Add("'Bucket' must not be empty.");
            if (GetCurrentRegion(s3Settings) == UnknownEndpoint) Errors.Add("Please select an endpoint.");

            if (IsError)
            {
                return null;
            }

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
                if (responseHeaders.Count == 0)
                {
                    Errors.Add("Upload to Amazon S3 failed. Check your access credentials.");
                    return null;
                }

                var eTag = responseHeaders.Get("ETag");
                if (eTag == null)
                {
                    Errors.Add("Upload to Amazon S3 failed.");
                    return null;
                }

                if (GetMd5Hash(stream) == eTag.Replace("\"", ""))
                {
                    return new UploadResult { IsSuccess = true, URL = GetObjectURL(putRequest.Key) };
                }

                Errors.Add("Upload to Amazon S3 failed, uploaded data did not match.");
                return null;
            }
        }
    }
}