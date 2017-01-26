using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public class AzureStorageSettings
    {
        public string AccountName { get; set; }
        public string AccessKey { get; set; }
        public string Container { get; set; }
    }
}