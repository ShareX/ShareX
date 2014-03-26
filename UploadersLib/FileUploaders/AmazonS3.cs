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

using HelpersLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System;
using System.Text.RegularExpressions;
using UploadersLib.HelperClasses;
using System.Security.Cryptography;
using System.Linq;

namespace UploadersLib.FileUploaders
{
    public sealed class AmazonS3 : FileUploader
    {
        const string FILE_INPUT_NAME = "file";

        public AmazonS3Settings AccessKeys { get; set; }

        public AmazonS3(AmazonS3Settings accessKeys)
        {
            AccessKeys = accessKeys;
        }

        private string GetObjectStorageClass()
        {
            return AccessKeys.UseReducedRedundancyStorage ? "REDUCED_REDUNDANCY" : "STANDARD";
        }

        private string GetObjectKey(string fileName)
        {
            var parser = new NameParser(NameParserType.FileName);
            return string.Format("{0}{1}", parser.Parse(AccessKeys.ObjectPrefix), fileName);
        }

        private string GetPolicyDocument(string fileName)
        {
            var mimeType = Helpers.GetMimeType(fileName);
            var objectKey = GetObjectKey(fileName);
            var objectStorageClass = GetObjectStorageClass();

            var policyDocument = string.Format(@"{{
                'expiration': '2015-12-01T12:00:00.000Z',
                'conditions': [
                    {{'acl': 'public-read' }},
                    {{'bucket': '{0}' }},
                    {{'Content-Type': '{1}'}},
                    {{'key': '{2}'}},
                    {{'x-amz-storage-class': '{3}'}},
                ]
            }}", AccessKeys.Bucket, mimeType, objectKey, objectStorageClass);

            return Regex.Replace(policyDocument, @"\s", "");
        }

        private string GetEndpoint()
        {
            return string.Format("{0}{1}", AccessKeys.Endpoint, AccessKeys.Bucket);
        }

        // http://codeonaboat.wordpress.com/2011/04/22/uploading-a-file-to-amazon-s3-using-an-asp-net-mvc-application-directly-from-the-users-browser/
        private string CreateSignature(string secretKey, byte[] policyBytes)
        {
            var encoding = new ASCIIEncoding();
            var base64Policy = Convert.ToBase64String(policyBytes);
            var secretKeyBytes = encoding.GetBytes(secretKey);
            var hmacsha1 = new HMACSHA1(secretKeyBytes);
            var base64PolicyBytes = encoding.GetBytes(base64Policy);
            var signatureBytes = hmacsha1.ComputeHash(base64PolicyBytes);
            return Convert.ToBase64String(signatureBytes);
        }

        private Dictionary<string, string> GetParameters(string fileName, string objectKey)
        {
            var policyDocument = GetPolicyDocument(fileName);
            var policyBytes = Encoding.ASCII.GetBytes(policyDocument);
            var signature = CreateSignature(AccessKeys.SecretAccessKey, policyBytes);

            var parameters = new Dictionary<string, string>();
            parameters.Add("key", objectKey);
            parameters.Add("acl", "public-read");
            parameters.Add("content-type", Helpers.GetMimeType(fileName));
            parameters.Add("AWSAccessKeyId", AccessKeys.AccessKeyID);
            parameters.Add("policy", Convert.ToBase64String(policyBytes));
            parameters.Add("signature", signature);
            parameters.Add("x-amz-storage-class", GetObjectStorageClass());
            return parameters;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(AccessKeys.AccessKeyID)) throw new Exception("'Access Key' must not be empty.");
            if (string.IsNullOrEmpty(AccessKeys.SecretAccessKey)) throw new Exception("'Secret Access Key' must not be empty.");
            if (string.IsNullOrEmpty(AccessKeys.Endpoint)) throw new Exception("'Endpoint' must not be emoty.");
            if (string.IsNullOrEmpty(AccessKeys.Bucket)) throw new Exception("'Bucket' must not be empty.");

            var objectKey = GetObjectKey(fileName);

            var uploadResult = UploadData(stream, GetEndpoint(), fileName, FILE_INPUT_NAME, GetParameters(fileName, objectKey), null, null, ResponseType.Headers);

            if (uploadResult.IsSuccess)
            {
                if (AccessKeys.UseCustomCNAME)
                {
                    uploadResult.URL = string.Format("http://{0}/{1}", AccessKeys.Bucket, objectKey);
                }
                else
                {
                    uploadResult.URL = string.Format("{0}/{1}", GetEndpoint(), objectKey);
                }
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

    public class AmazonS3Region
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
    }
}