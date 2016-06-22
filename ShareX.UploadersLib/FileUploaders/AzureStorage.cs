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

// Credits: https://github.com/0xdeafcafe

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ShareX.HelpersLib;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class AzureStorageFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.AzureStorage;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.AzureStorageSettings != null && 
                !string.IsNullOrEmpty(config.AzureStorageSettings.AccountKey) &&
                !string.IsNullOrEmpty(config.AzureStorageSettings.AccountName) && 
                !string.IsNullOrEmpty(config.AzureStorageSettings.ContainerName);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new AzureStorage(config.AzureStorageSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpAzureStorage;

        public enum UploadType
        {
            [Description("Storage Uri")]
            StorageUri,

            [Description("Shared Access Signature")]
            SharedAccessSignature
        }
    }

    public class AzureStorage : FileUploader
    {
        private AzureStorageSettings _settings { get; set; }

        public AzureStorage(AzureStorageSettings settings)
        {
            _settings = settings;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!_settings.IsDevelopmentStorageAccount)
            {
                if (string.IsNullOrEmpty(_settings.AccountKey)) Errors.Add($"'{nameof(_settings.AccountKey)}' must not be empty.");
                if (string.IsNullOrEmpty(_settings.AccountName)) Errors.Add($"'{nameof(_settings.AccountName)}' must not be empty.");
                if (string.IsNullOrEmpty(_settings.ContainerName)) Errors.Add($"'{nameof(_settings.ContainerName)}' must not be empty.");
                if (string.IsNullOrEmpty(_settings.DefaultEndpointsProtocol)) Errors.Add($"'{nameof(_settings.DefaultEndpointsProtocol)}' must not be empty.");
                if (IsError) return null;
            }

            var connectionString = _settings.IsDevelopmentStorageAccount ?
                "UseDevelopmentStorage=true" :
                $"DefaultEndpointsProtocol={_settings.DefaultEndpointsProtocol};AccountName={_settings.AccountName};AccountKey={_settings.AccountKey}";
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_settings.ContainerName);
            blobContainer.CreateIfNotExists();
            var blobFileName = GetObjectKey(fileName);
            var blobRef = blobContainer.GetBlockBlobReference(blobFileName);
            blobRef.UploadFromStream(stream);

            var uri = blobRef.Uri.ToString();

            if (_settings.UploadSecurityType == AzureStorageFileUploaderService.UploadType.SharedAccessSignature)
                uri += blobRef.GetSharedAccessSignature(new SharedAccessBlobPolicy
                    {
                        Permissions = SharedAccessBlobPermissions.Read,
                        SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5)
                    });

            return new UploadResult
            {
                IsSuccess = true,
                URL = uri
            };
        }

        private string GetObjectKey(string fileName)
        {
            var objectPrefix = NameParser.Parse(NameParserType.FolderPath, _settings.ObjectPrefix.Trim('/'));
            return URLHelpers.CombineURL(objectPrefix, fileName);
        }
    }
}
