#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class PlikFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Plik;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Plik(config.PlikSettings);
        }

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.PlikSettings.URL) && !string.IsNullOrEmpty(config.PlikSettings.APIKey);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPlik;

        public override Icon ServiceIcon => Resources.Plik;
    }

    public sealed class Plik : FileUploader
    {
        public PlikSettings Settings { get; private set; }

        public Plik(PlikSettings settings)
        {
            Settings = settings;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(Settings.URL))
            {
                throw new Exception("Plik Host is empty.");
            }
            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders["X-PlikToken"] = Settings.APIKey;
            UploadMetadataRequest metaDataReq = new UploadMetadataRequest();
            metaDataReq.Files = new UploadMetadataRequestFile0();
            metaDataReq.Files.File0 = new UploadMetadataRequestFile();
            metaDataReq.Files.File0.FileName = fileName;
            metaDataReq.Files.File0.FileType = MimeTypes.GetMimeTypeFromFileName(fileName);
            metaDataReq.Files.File0.FileSize = Convert.ToInt32(stream.Length);
            metaDataReq.Removable = Settings.Removable;
            metaDataReq.OneShot = Settings.OneShot;
            if (Settings.TTLUnit != 3) // everything except the expire time -1
            {
                metaDataReq.Ttl = Convert.ToInt32(GetMultiplyIndex(2, Settings.TTLUnit) * Settings.TTL * 60);
            }
            else
            {
                metaDataReq.Ttl = -1;
            }
            if (Settings.HasComment)
            {
                metaDataReq.Comment = Settings.Comment;
            }
            if (Settings.IsSecured)
            {
                metaDataReq.Login = Settings.Login;
                metaDataReq.Password = Settings.Password;
            }
            string metaDataResp = SendRequest(HttpMethod.POST, Settings.URL + "/upload", JsonConvert.SerializeObject(metaDataReq), headers: requestHeaders);
            UploadMetadataResponse metaData = JsonConvert.DeserializeObject<UploadMetadataResponse>(metaDataResp);
            requestHeaders["x-uploadtoken"] = metaData.uploadToken;
            string url = $"{Settings.URL}/file/{metaData.id}/{metaData.files.First().Value.id}/{fileName}";
            UploadResult FileDatReq = SendRequestFile(url, stream, fileName, "file", headers: requestHeaders);

            return ConvertResult(metaData, FileDatReq);
        }

        private UploadResult ConvertResult(UploadMetadataResponse metaData, UploadResult fileDataReq)
        {
            UploadResult result = new UploadResult(fileDataReq.Response);
            //UploadMetadataResponse fileData = JsonConvert.DeserializeObject<UploadMetadataResponse>(fileDataReq.Response);
            UploadMetadataResponseFile actFile = metaData.files.First().Value;
            result.URL = $"{Settings.URL}/file/{metaData.id}/{actFile.id}/{URLHelpers.URLEncode(actFile.fileName)}";
            return result;
        }

        internal static void CalculateTTLValue(NumericUpDown nudTTL, int newUnit, int oldUnit)
        {
            if (newUnit != 3)
            {
                if (nudTTL.Value == -1)
                {
                    nudTTL.SetValue(1);
                }

                nudTTL.SetValue(nudTTL.Value * GetMultiplyIndex(newUnit, oldUnit));
                nudTTL.ReadOnly = false;
            }
            else
            {
                nudTTL.SetValue(-1);
                nudTTL.ReadOnly = true;
            }
        }

        internal static decimal GetMultiplyIndex(int newUnit, int oldUnit)
        {
            decimal multiplyValue = 1m;
            switch (newUnit)
            {
                case 0: // days
                    switch (oldUnit)
                    {
                        case 1: // hours
                            multiplyValue = 1m / 24m;
                            break;
                        case 2: // minutes
                            multiplyValue = 1m / 24m / 60m;
                            break;
                    }
                    break;
                case 1: // hours
                    switch (oldUnit)
                    {
                        case 0: // days
                            multiplyValue = 24m;
                            break;
                        case 2: // minutes
                            multiplyValue = 1m / 60m;
                            break;
                    }
                    break;
                case 2: // minutes
                    switch (oldUnit)
                    {
                        case 0: // days
                            multiplyValue = 60m * 24m;
                            break;
                        case 1: // hours
                            multiplyValue = 60m;
                            break;
                    }
                    break;
            }
            return multiplyValue;
        }
    }

    public class UploadMetadataRequestFile
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("fileType")]
        public string FileType { get; set; }
        [JsonProperty("fileSize")]
        public int FileSize { get; set; }
    }

    public class UploadMetadataRequestFile0
    {
        [JsonProperty("0")]
        public UploadMetadataRequestFile File0 { get; set; }
    }

    public class UploadMetadataRequest
    {
        [JsonProperty("ttl")]
        public int Ttl { get; set; }
        [JsonProperty("removable")]
        public bool Removable { get; set; }
        [JsonProperty("oneShot")]
        public bool OneShot { get; set; }
        [JsonProperty("comments")]
        public string Comment { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("files")]
        public UploadMetadataRequestFile0 Files { get; set; }
    }

    public class UploadMetadataResponseFile
    {
        public string id { get; set; }
        public string fileName { get; set; }
        public string fileMd5 { get; set; }
        public string status { get; set; }
        public string fileType { get; set; }
        public int fileUploadDate { get; set; }
        public int fileSize { get; set; }
        public string reference { get; set; }
    }

    public class UploadMetadataResponse
    {
        public string id { get; set; }
        public int uploadDate { get; set; }
        public int ttl { get; set; }
        public string shortUrl { get; set; }
        public string downloadDomain { get; set; }
        public string comments { get; set; }
        public Dictionary<string, UploadMetadataResponseFile> files { get; set; }
        public string uploadToken { get; set; }
        public bool admin { get; set; }
        public bool stream { get; set; }
        public bool oneShot { get; set; }
        public bool removable { get; set; }
        public bool protectedByPassword { get; set; }
        public bool protectedByYubikey { get; set; }
    }
}