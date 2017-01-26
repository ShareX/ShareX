using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class AzureStorage : FileUploader
    {
        private string azureStorageAccountName;
        private string azureStorageAccountAccessKey;
        private string azureStorageContainer;
        private const string apiVersion = "2016-05-31";
        private string uri;
        private string date;

        public AzureStorage(string asAccountName, string asAccessKey, string asContainer)
        {
            azureStorageAccountName = asAccountName;
            azureStorageAccountAccessKey = asAccessKey;
            azureStorageContainer = asContainer;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(azureStorageAccountName)) { Errors.Add("'Account Name' must not be empty"); }
            if (string.IsNullOrEmpty(azureStorageAccountAccessKey)) { Errors.Add("'Access key' must not be empty"); }
            if (string.IsNullOrEmpty(azureStorageContainer)) { Errors.Add("'Container' must not be empty"); }

            if (IsError)
            {
                return null;
            }

            date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            CreateContainerIfNotExists();

            uri = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", azureStorageAccountName, azureStorageContainer, fileName);

            HttpWebRequest request = GenerateBasicWebRequest(uri);
            request.ContentLength = stream.Length;
            request.Headers.Add("x-ms-blob-type", "BlockBlob");

            var canonicalizedHeaders = string.Format("x-ms-blob-type:BlockBlob\nx-ms-date:{0}\nx-ms-version:{1}\n", date, apiVersion);
            var canonicalizedResource = string.Format("/{0}/{1}/{2}", azureStorageAccountName, azureStorageContainer, fileName);

            var StringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource, request.ContentLength.ToString());

            request.Headers.Add("Authorization", string.Format("SharedKey {0}:{1}", azureStorageAccountName, HashRequest(StringToSign)));

            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);

            var requestStream = request.GetRequestStream();
            requestStream.Write(byteArray, 0, (int)stream.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if ((int)response.StatusCode == 201)
                {
                    return new UploadResult { IsSuccess = true, URL = uri };
                }
                else
                {
                    Errors.Add("An error has occured. Please try again later.");
                    return null;
                }
            }
            catch (WebException ex)
            {
                if ((int)((HttpWebResponse)ex.Response).StatusCode == 403)
                {
                    Errors.Add("Unauthorized. Check your credentials.");
                    return null;
                }

                Errors.Add("An error has occured. Please try again later.");
                return null;
            }
        }

        private void CreateContainerIfNotExists()
        {
            uri = string.Format("https://{0}.blob.core.windows.net/{1}?restype=container", azureStorageAccountName, azureStorageContainer);

            HttpWebRequest request = GenerateBasicWebRequest(uri);
            request.ContentLength = 0;

            var canonicalizedHeaders = string.Format("x-ms-date:{0}\nx-ms-version:{1}\n", date, apiVersion);
            var canonicalizedResource = string.Format("/{0}/{1}\nrestype:container", azureStorageAccountName, azureStorageContainer);

            var StringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource);

            request.Headers.Add("Authorization", string.Format("SharedKey {0}:{1}", azureStorageAccountName, HashRequest(StringToSign)));

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                SetContainerACL();
            }
            catch (WebException ex)
            {
                if ((int)((HttpWebResponse)ex.Response).StatusCode == 409)
                {
                    return;
                }

                Errors.Add("An error has occured. Please try again later.");
            }
        }

        private void SetContainerACL()
        {
            uri = string.Format("https://{0}.blob.core.windows.net/{1}?restype=container&comp=acl", azureStorageAccountName, azureStorageContainer);

            HttpWebRequest request = GenerateBasicWebRequest(uri);
            request.ContentLength = 0;
            request.Headers.Add("x-ms-blob-public-access", "container");

            var canonicalizedHeaders = string.Format("x-ms-blob-public-access:container\nx-ms-date:{0}\nx-ms-version:{1}\n", date, apiVersion);
            var canonicalizedResource = string.Format("/{0}/{1}\ncomp:acl\nrestype:container", azureStorageAccountName, azureStorageContainer);

            var StringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource);

            request.Headers.Add("Authorization", string.Format("SharedKey {0}:{1}", azureStorageAccountName, HashRequest(StringToSign)));

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                Errors.Add("An error has occured. Please try again later.");
            }
        }

        private string HashRequest(string stringToSign)
        {
            HashAlgorithm hashAlgorithm = new HMACSHA256(Convert.FromBase64String(azureStorageAccountAccessKey));
            byte[] messageBuffer = Encoding.UTF8.GetBytes(stringToSign);
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(messageBuffer));
        }

        private HttpWebRequest GenerateBasicWebRequest(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "PUT";
            request.Headers.Add("x-ms-date", date);
            request.Headers.Add("x-ms-version", apiVersion);

            return request;
        }

        private string GenerateStringToSign(string canonicalizedHeaders, string canonicalizedResource, string contentLength = "")
        {
            var stringToSign = "PUT" + "\n" +
                "\n" +
                "\n" +
                (string.IsNullOrEmpty(contentLength) ? string.Empty : contentLength) + "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                canonicalizedHeaders +
                canonicalizedResource;

            return stringToSign;
        }
    }

    public class AzureStorageUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.AzureStorage;

        public override Image ServiceImage => Resources.AzureStorage;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.AzureStorageAccountName) &&
                !string.IsNullOrEmpty(config.AzureStorageAccountAccessKey) &&
                !string.IsNullOrEmpty(config.AzureStorageContainer);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new AzureStorage(config.AzureStorageAccountName, config.AzureStorageAccountAccessKey, config.AzureStorageContainer);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpAzureStorage;
    }
}