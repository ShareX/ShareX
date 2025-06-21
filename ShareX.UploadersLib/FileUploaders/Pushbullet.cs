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
    public class PushbulletFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Pushbullet;

        public override Icon ServiceIcon => Resources.Pushbullet;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.PushbulletSettings != null && !string.IsNullOrEmpty(config.PushbulletSettings.UserAPIKey) &&
                config.PushbulletSettings.DeviceList != null && config.PushbulletSettings.DeviceList.IsValidIndex(config.PushbulletSettings.SelectedDevice);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Pushbullet(config.PushbulletSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPushbullet;
    }

    public sealed class Pushbullet : FileUploader
    {
        public PushbulletSettings Config { get; private set; }

        public Pushbullet(PushbulletSettings config)
        {
            Config = config;
        }

        private const string
            wwwPushesURL = "https://www.pushbullet.com/pushes",
            apiURL = "https://api.pushbullet.com/v2",
            apiGetDevicesURL = apiURL + "/devices",
            apiSendPushURL = apiURL + "/pushes",
            apiRequestFileUploadURL = apiURL + "/upload-request";

        public UploadResult PushFile(Stream stream, string fileName)
        {
            NameValueCollection headers = RequestHelpers.CreateAuthenticationHeader(Config.UserAPIKey, "");

            Dictionary<string, string> pushArgs, upArgs = new Dictionary<string, string>();

            upArgs.Add("file_name", fileName);

            string uploadRequest = SendRequestMultiPart(apiRequestFileUploadURL, upArgs, headers);

            if (uploadRequest == null) return null;

            PushbulletResponseFileUpload fileInfo = JsonConvert.DeserializeObject<PushbulletResponseFileUpload>(uploadRequest);

            if (fileInfo == null) return null;

            pushArgs = upArgs;

            upArgs = new Dictionary<string, string>();

            upArgs.Add("awsaccesskeyid", fileInfo.data.awsaccesskeyid);
            upArgs.Add("acl", fileInfo.data.acl);
            upArgs.Add("key", fileInfo.data.key);
            upArgs.Add("signature", fileInfo.data.signature);
            upArgs.Add("policy", fileInfo.data.policy);
            upArgs.Add("content-type", fileInfo.data.content_type);

            UploadResult uploadResult = SendRequestFile(fileInfo.upload_url, stream, fileName, "file", upArgs);

            if (uploadResult == null) return null;

            pushArgs.Add("device_iden", Config.CurrentDevice.Key);
            pushArgs.Add("type", "file");
            pushArgs.Add("file_url", fileInfo.file_url);
            pushArgs.Add("body", "Sent via ShareX");
            pushArgs.Add("file_type", fileInfo.file_type);

            string pushResult = SendRequestMultiPart(apiSendPushURL, pushArgs, headers);

            if (pushResult == null) return null;

            PushbulletResponsePush push = JsonConvert.DeserializeObject<PushbulletResponsePush>(pushResult);

            if (push != null)
                uploadResult.URL = wwwPushesURL + "?push_iden=" + push.iden;

            return uploadResult;
        }

        private string Push(string pushType, string valueType, string value, string title)
        {
            NameValueCollection headers = RequestHelpers.CreateAuthenticationHeader(Config.UserAPIKey, "");

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("device_iden", Config.CurrentDevice.Key);
            args.Add("type", pushType);
            args.Add("title", title);
            args.Add(valueType, value);

            if (valueType != "body")
            {
                if (pushType == "link")
                    args.Add("body", value);
                else
                    args.Add("body", "Sent via ShareX");
            }

            string response = SendRequestMultiPart(apiSendPushURL, args, headers);

            if (response == null) return null;

            PushbulletResponsePush push = JsonConvert.DeserializeObject<PushbulletResponsePush>(response);

            if (push != null)
                return wwwPushesURL + "?push_iden=" + push.iden;

            return null;
        }

        public string PushNote(string note, string title)
        {
            return Push("note", "body", note, title);
        }

        public string PushLink(string link, string title)
        {
            return Push("link", "url", link, title);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(Config.UserAPIKey)) throw new Exception("Missing API key.");
            if (Config.CurrentDevice == null) throw new Exception("No device set to push to.");
            if (string.IsNullOrEmpty(Config.CurrentDevice.Key)) throw new Exception("Missing device key.");

            return PushFile(stream, fileName);
        }

        public List<PushbulletDevice> GetDeviceList()
        {
            NameValueCollection headers = RequestHelpers.CreateAuthenticationHeader(Config.UserAPIKey, "");

            string response = SendRequest(HttpMethod.GET, apiGetDevicesURL, headers: headers);

            PushbulletResponseDevices devicesResponse = JsonConvert.DeserializeObject<PushbulletResponseDevices>(response);

            if (devicesResponse != null && devicesResponse.devices != null)
            {
                return devicesResponse.devices.Where(x => !string.IsNullOrEmpty(x.nickname)).Select(x1 => new PushbulletDevice { Key = x1.iden, Name = x1.nickname }).ToList();
            }

            return new List<PushbulletDevice>();
        }

        private class PushbulletResponseDevices
        {
            public List<PushbulletResponseDevice> devices { get; set; }
        }

        private class PushbulletResponseDevice
        {
            public string iden { get; set; }
            public string nickname { get; set; }
        }

        private class PushbulletResponsePush
        {
            public string iden { get; set; }
            public string device_iden { get; set; }
            public PushbulletResponsePushData data { get; set; }
            public long created { get; set; }
        }

        private class PushbulletResponsePushData
        {
            public string type { get; set; }
            public string title { get; set; }
            public string body { get; set; }
        }

        private class PushbulletResponseFileUpload
        {
            public string file_type { get; set; }
            public string file_name { get; set; }
            public string file_url { get; set; }
            public string upload_url { get; set; }
            public PushbulletResponseFileUploadData data { get; set; }
        }

        private class PushbulletResponseFileUploadData
        {
            public string awsaccesskeyid { get; set; }
            public string acl { get; set; }
            public string key { get; set; }
            public string signature { get; set; }
            public string policy { get; set; }
            [JsonProperty("content-type")]
            public string content_type { get; set; }
        }
    }

    public class PushbulletDevice
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public class PushbulletSettings
    {
        [JsonEncrypt]
        public string UserAPIKey { get; set; } = "";
        public List<PushbulletDevice> DeviceList { get; set; } = new List<PushbulletDevice>();
        public int SelectedDevice { get; set; } = 0;

        public PushbulletDevice CurrentDevice
        {
            get
            {
                if (DeviceList.IsValidIndex(SelectedDevice))
                {
                    return DeviceList[SelectedDevice];
                }

                return null;
            }
        }
    }
}