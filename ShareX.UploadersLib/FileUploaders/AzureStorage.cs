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
using System.Net;
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
            return new AzureStorage(config.AzureStorageAccountName, config.AzureStorageAccountAccessKey, config.AzureStorageContainer,
                config.AzureStorageEnvironment, config.AzureStorageCustomDomain, config.AzureStorageUploadPath);
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
        public string AzureStorageUploadPath { get; private set; }

        public AzureStorage(string azureStorageAccountName, string azureStorageAccessKey, string azureStorageContainer, string azureStorageEnvironment,
            string customDomain, string uploadPath)
        {
            AzureStorageAccountName = azureStorageAccountName;
            AzureStorageAccountAccessKey = azureStorageAccessKey;
            AzureStorageContainer = azureStorageContainer;
            AzureStorageEnvironment = (!string.IsNullOrEmpty(azureStorageEnvironment)) ? azureStorageEnvironment : "blob.core.windows.net";
            AzureStorageCustomDomain = customDomain;
            AzureStorageUploadPath = uploadPath;
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

            string date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            string uploadPath = GetUploadPath(fileName);
            string url = GenerateURL(uploadPath, true);
            string contentType = UploadHelpers.GetMimeType(fileName);

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["x-ms-date"] = date;
            requestHeaders["x-ms-version"] = APIVersion;
            requestHeaders["x-ms-blob-type"] = "BlockBlob";

            string canonicalizedHeaders = $"x-ms-blob-type:BlockBlob\nx-ms-date:{date}\nx-ms-version:{APIVersion}\n";
            string canonicalizedResource = $"/{AzureStorageAccountName}/{AzureStorageContainer}/{uploadPath}";
            string stringToSign = GenerateStringToSign(canonicalizedHeaders, canonicalizedResource, stream.Length.ToString(), contentType);

            requestHeaders["Authorization"] = $"SharedKey {AzureStorageAccountName}:{stringToSign}";

            using (HttpWebResponse response = GetResponse(HttpMethod.PUT, url, stream, contentType, null, requestHeaders, null))
            {
                if (response != null && response.Headers != null)
                {
                    return new UploadResult
                    {
                        IsSuccess = true,
                        URL = GenerateURL(uploadPath)
                    };
                }
            }

            Errors.Add("Upload failed.");
            return null;
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

        private string GetUploadPath(string fileName)
        {
            string uploadPath;

            if (!string.IsNullOrEmpty(AzureStorageUploadPath))
            {
                string path = NameParser.Parse(NameParserType.FolderPath, AzureStorageUploadPath.Trim('/'));
                uploadPath = URLHelpers.CombineURL(path, fileName);
            }
            else
            {
                uploadPath = fileName;
            }

            return Uri.EscapeUriString(uploadPath);
        }

        public string GenerateURL(string uploadPath, bool isRequest = false)
        {
            string url;

            if (!isRequest && !string.IsNullOrEmpty(AzureStorageCustomDomain))
            {
                url = URLHelpers.CombineURL(AzureStorageCustomDomain, uploadPath);
                url = URLHelpers.FixPrefix(url, "https://");
            }
            else if (!isRequest && AzureStorageContainer == "$root")
            {
                url = $"https://{AzureStorageAccountName}.{AzureStorageEnvironment}/{uploadPath}";
            }
            else
            {
                url = $"https://{AzureStorageAccountName}.{AzureStorageEnvironment}/{AzureStorageContainer}/{uploadPath}";
            }

            return url;
        }

        public string GetPreviewURL()
        {
            string uploadPath = GetUploadPath("example.png");
            return GenerateURL(uploadPath);
        }
    }
}