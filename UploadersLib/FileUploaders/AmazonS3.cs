#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace UploadersLib.FileUploaders
{
    public sealed class AmazonS3 : FileUploader
    {
        private AmazonS3Settings S3Settings { get; set; }

        public AmazonS3(AmazonS3Settings accessKeys)
        {
            S3Settings = accessKeys;
        }

        private string GetObjectStorageClass()
        {
            return S3Settings.UseReducedRedundancyStorage ? "REDUCED_REDUNDANCY" : "STANDARD";
        }

        private string GetObjectKey(string fileName)
        {
            var parser = new NameParser(NameParserType.FolderPath);
            return string.Format("{0}{1}", parser.Parse(S3Settings.ObjectPrefix), fileName);
        }

        // Helper class to construct the S3 policy document (below)
        private class S3PolicyCondition : Dictionary<string, string>
        {
            public S3PolicyCondition(string key, string value)
            {
                Add(key, value);
            }
        }

        private string GetPolicyDocument(string fileName)
        {
            var policyDocument = new
            {
                expiration = DateTime.UtcNow.AddDays(2).ToString("o"), // The policy is valid for 2 days
                conditions = new List<S3PolicyCondition> {
                    new S3PolicyCondition("acl", "public-read"),
                    new S3PolicyCondition("bucket", S3Settings.Bucket),
                    new S3PolicyCondition("Content-Type", Helpers.GetMimeType(fileName)),
                    new S3PolicyCondition("key", GetObjectKey(fileName)),
                    new S3PolicyCondition("x-amz-storage-class", GetObjectStorageClass())
                }
            };

            return JsonConvert.SerializeObject(policyDocument);
        }

        private string GetEndpoint()
        {
            return string.Format("{0}{1}", S3Settings.Endpoint, S3Settings.Bucket);
        }

        // http://codeonaboat.wordpress.com/2011/04/22/uploading-a-file-to-amazon-s3-using-an-asp-net-mvc-application-directly-from-the-users-browser/
        private string CreateSignature(string secretKey, byte[] policyBytes)
        {
            var encoding = new ASCIIEncoding();
            var base64Policy = Convert.ToBase64String(policyBytes);
            var secretKeyBytes = encoding.GetBytes(secretKey);

            byte[] signatureBytes;
            using (var hmacsha1 = new HMACSHA1(secretKeyBytes))
            {
                var base64PolicyBytes = encoding.GetBytes(base64Policy);
                signatureBytes = hmacsha1.ComputeHash(base64PolicyBytes);
            }

            return Convert.ToBase64String(signatureBytes);
        }

        private string GetObjectURL(string objectName)
        {
            var urlEncodedObjectName = Helpers.URLPathEncode(objectName);
            if (S3Settings.UseCustomCNAME)
            {
                return string.Format("http://{0}/{1}", S3Settings.Bucket, urlEncodedObjectName);
            }
            else
            {
                return string.Format("{0}/{1}", GetEndpoint(), urlEncodedObjectName);
            }
        }

        private Dictionary<string, string> GetParameters(string fileName, string objectKey)
        {
            var policyDocument = GetPolicyDocument(fileName);
            var policyBytes = Encoding.ASCII.GetBytes(policyDocument);
            var signature = CreateSignature(S3Settings.SecretAccessKey, policyBytes);

            var parameters = new Dictionary<string, string>();
            parameters.Add("key", objectKey);
            parameters.Add("acl", "public-read");
            parameters.Add("content-type", Helpers.GetMimeType(fileName));
            parameters.Add("AWSAccessKeyId", S3Settings.AccessKeyID);
            parameters.Add("policy", Convert.ToBase64String(policyBytes));
            parameters.Add("signature", signature);
            parameters.Add("x-amz-storage-class", GetObjectStorageClass());
            return parameters;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(S3Settings.AccessKeyID)) throw new Exception("'Access Key' must not be empty.");
            if (string.IsNullOrEmpty(S3Settings.SecretAccessKey)) throw new Exception("'Secret Access Key' must not be empty.");
            if (string.IsNullOrEmpty(S3Settings.Endpoint)) throw new Exception("'Endpoint' must not be emoty.");
            if (string.IsNullOrEmpty(S3Settings.Bucket)) throw new Exception("'Bucket' must not be empty.");

            var objectKey = GetObjectKey(fileName);

            var uploadResult = UploadData(stream, GetEndpoint(), fileName, arguments: GetParameters(fileName, objectKey), responseType: ResponseType.Headers);

            if (uploadResult.IsSuccess)
            {
                uploadResult.URL = GetObjectURL(objectKey);
            }

            return uploadResult;
        }
    }

    public class AmazonS3Settings
    {
        public string AccessKeyID { get; set; }
        public string SecretAccessKey { get; set; }
        public bool UseReducedRedundancyStorage { get; set; }
        public bool UseCustomCNAME { get; set; }
        public string ObjectPrefix { get; set; }
        public string Bucket { get; set; }
        public string Endpoint { get; set; }
    }
}