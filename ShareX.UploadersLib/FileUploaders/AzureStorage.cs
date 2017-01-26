using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

            return new UploadResult { IsSuccess = true, URL = "fake url" };
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