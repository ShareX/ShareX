#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
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
            return new AzureStorage(config.AzureStorageAccountName, config.AzureStorageAccountAccessKey, config.AzureStorageContainer, config.AzureStorageEnvironment, config.AzureStorageCustomDomain);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpAzureStorage;
    }

    public sealed class AzureStorage : FileUploader
    {
        private const string APIVersion = "2016-05-31";

        public string AzureStorageAccountName { get; private set; }
        public string AzureStorageAccountAccessKey { get; private set; }
        public string AzureStorageContainer { get; private set; }
        public string AzureStorageEnvironment { get; private set; }
        public string AzureStorageCustomDomain { get; private set; }

        public AzureStorage(string azureStorageAccountName, string azureStorageAccessKey, string azureStorageContainer, string azureStorageEnvironment, string customDomain)
        {
            AzureStorageAccountName = azureStorageAccountName;
            AzureStorageAccountAccessKey = azureStorageAccessKey;
            AzureStorageContainer = azureStorageContainer;
            AzureStorageEnvironment = (!string.IsNullOrEmpty(azureStorageEnvironment)) ? azureStorageEnvironment : "blob.core.windows.net";
            AzureStorageCustomDomain = customDomain;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(AzureStorageAccountName)) Errors.Add("'Account Name' must not be empty");
            if (string.IsNullOrEmpty(AzureStorageAccountAccessKey)) Errors.Add("'Access key' must not be empty");
            if (string.IsNullOrEmpty(AzureStorageContainer)) Errors.Add("'Container' must not be empty");

            if (IsError)
            {
                return null;
            }

            CreateContainerIfNotExists();

            string date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            string url = $"https://{AzureStorageAccountName}.{AzureStorageEnvironment}/{AzureStorageContainer}/{fileName}";
            string contentType = Helpers.GetMimeType(fileName);

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = APIVersion;
            requestHeaders["x-ms-blob-type"] = "BlockBlob";

            string canonicalizedHeaders = $"x-ms-blob-type:BlockBlob\nx-ms-date:{date}\nx-ms-version:{APIVersion}\n";
            string canonicalizedResource = $"/{AzureStorageAccountName}/{AzureStorageContainer}/{fileName}";
            string stringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource, stream.Length.ToString(), contentType);

            requestHeaders["Authorization"] = $"SharedKey {AzureStorageAccountName}:{stringToSign}";

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, url, stream, contentType, null, requestHeaders, null);

            if (responseHeaders != null)
            {
                string result;

                if (!string.IsNullOrEmpty(AzureStorageCustomDomain))
                {
                    result = URLHelpers.CombineURL(AzureStorageCustomDomain, AzureStorageContainer, fileName);
                    result = URLHelpers.FixPrefix(result);
                }
                else
                {
                    result = url;
                }

                return new UploadResult { IsSuccess = true, URL = result };
            }
            else
            {
                Errors.Add("Upload failed.");
                return null;
            }
        }

        private void CreateContainerIfNotExists()
        {
            string date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            string url = $"https://{AzureStorageAccountName}.{AzureStorageEnvironment}/{AzureStorageContainer}?restype=container";

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["Content-Length"] = "0";
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = APIVersion;

            string canonicalizedHeaders = $"x-ms-date:{date}\nx-ms-version:{APIVersion}\n";
            string canonicalizedResource = $"/{AzureStorageAccountName}/{AzureStorageContainer}\nrestype:container";
            string stringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource);

            requestHeaders["Authorization"] = $"SharedKey {AzureStorageAccountName}:{stringToSign}";

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, url, null, null, null, requestHeaders, null);

            if (responseHeaders != null)
            {
                SetContainerACL();
            }
            else
            {
                if (Errors.Count > 0)
                {
                    if (Errors[0].Contains("409"))
                    {
                        SetContainerACL();
                    }
                    else
                    {
                        Errors.Add("Upload to Azure storage failed.");
                    }
                }
            }
        }

        private void SetContainerACL()
        {
            string date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            string url = $"https://{AzureStorageAccountName}.{AzureStorageEnvironment}/{AzureStorageContainer}?restype=container&comp=acl";

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["Content-Length"] = "0";
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = APIVersion;
            requestHeaders["x-ms-blob-public-access"] = "container";

            string canonicalizedHeaders = $"x-ms-blob-public-access:container\nx-ms-date:{date}\nx-ms-version:{APIVersion}\n";
            string canonicalizedResource = $"/{AzureStorageAccountName}/{AzureStorageContainer}\ncomp:acl\nrestype:container";
            string stringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource);

            requestHeaders["Authorization"] = $"SharedKey {AzureStorageAccountName}:{stringToSign}";

            NameValueCollection responseHeaders = SendRequestGetHeaders(HttpMethod.PUT, url, null, null, null, requestHeaders, null);

            if (responseHeaders == null)
            {
                Errors.Add("There was an issue with setting ACL on the container.");
            }
        }

        private string GenerateStringToSign(string canonicalizedHeaders, string canonicalizedResource, string contentLength = "", string contentType = "")
        {
            string stringToSign = "PUT" + "\n" +
                "\n" +
                "\n" +
                (contentLength ?? "") + "\n" +
                "\n" +
                (contentType ?? "") + "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                "\n" +
                canonicalizedHeaders +
                canonicalizedResource;

            return HashRequest(stringToSign);
        }

        private string HashRequest(string stringToSign)
        {
            string hashedString;

            using (HashAlgorithm hashAlgorithm = new HMACSHA256(Convert.FromBase64String(AzureStorageAccountAccessKey)))
            {
                byte[] messageBuffer = Encoding.UTF8.GetBytes(stringToSign);
                hashedString = Convert.ToBase64String(hashAlgorithm.ComputeHash(messageBuffer));
            }

            return hashedString;
        }
    }
}