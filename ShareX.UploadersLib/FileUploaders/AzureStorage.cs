using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        //private string date;

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

            CreateContainerIfNotExists();

            var date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            var uri = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", azureStorageAccountName, azureStorageContainer, fileName);

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = apiVersion;
            requestHeaders["x-ms-blob-type"] = "BlockBlob";

            var canonicalizedHeaders = string.Format("x-ms-blob-type:BlockBlob\nx-ms-date:{0}\nx-ms-version:{1}\n", date, apiVersion);
            var canonicalizedResource = string.Format("/{0}/{1}/{2}", azureStorageAccountName, azureStorageContainer, fileName);

            var StringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource, stream.Length.ToString());

            requestHeaders["Authorization"] = string.Format("SharedKey {0}:{1}", azureStorageAccountName, HashRequest(StringToSign));

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, uri, stream, null, null, requestHeaders, null);

            if (responseHeaders != null)
            {
                return new UploadResult { IsSuccess = true, URL = uri };
            }
            else
            {
                Errors.Add("Upload failed.");
                return null;
            }
        }

        private void CreateContainerIfNotExists()
        {
            var date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            var uri = string.Format("https://{0}.blob.core.windows.net/{1}?restype=container", azureStorageAccountName, azureStorageContainer);

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["Content-Length"] = "0";
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = apiVersion;

            var canonicalizedHeaders = string.Format("x-ms-date:{0}\nx-ms-version:{1}\n", date, apiVersion);
            var canonicalizedResource = string.Format("/{0}/{1}\nrestype:container", azureStorageAccountName, azureStorageContainer);

            var StringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource);

            requestHeaders["Authorization"] = string.Format("SharedKey {0}:{1}", azureStorageAccountName, HashRequest(StringToSign));

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, uri, null, null, null, requestHeaders, null);

            if (responseHeaders != null)
            {
                SetContainerACL();
            }
            else
            {
                if (Errors.Count != 0)
                {
                    if (Errors[0].Contains("409"))
                        SetContainerACL();
                    else
                    {
                        Errors.Add("Upload to Azure storage failed.");
                    }
                }
            }
        }

        private void SetContainerACL()
        {
            var date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            var uri = string.Format("https://{0}.blob.core.windows.net/{1}?restype=container&comp=acl", azureStorageAccountName, azureStorageContainer);

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["Content-Length"] = "0";
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = apiVersion;
            requestHeaders["x-ms-blob-public-access"] = "container";

            var canonicalizedHeaders = string.Format("x-ms-blob-public-access:container\nx-ms-date:{0}\nx-ms-version:{1}\n", date, apiVersion);
            var canonicalizedResource = string.Format("/{0}/{1}\ncomp:acl\nrestype:container", azureStorageAccountName, azureStorageContainer);

            var StringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource);

            requestHeaders["Authorization"] = string.Format("SharedKey {0}:{1}", azureStorageAccountName, HashRequest(StringToSign));

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, uri, null, null, null, requestHeaders, null);

            if (responseHeaders == null)
                Errors.Add("There was an issue with setting ACL on the container.");
        }

        private string HashRequest(string stringToSign)
        {
            HashAlgorithm hashAlgorithm = new HMACSHA256(Convert.FromBase64String(azureStorageAccountAccessKey));
            byte[] messageBuffer = Encoding.UTF8.GetBytes(stringToSign);
            var hashedString = Convert.ToBase64String(hashAlgorithm.ComputeHash(messageBuffer));
            hashAlgorithm.Clear();
            return hashedString;
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