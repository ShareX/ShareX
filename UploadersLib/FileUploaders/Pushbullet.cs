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

// Credits: https://github.com/BallisticLingonberries

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;

namespace UploadersLib.FileUploaders
{
    public sealed class Pushbullet : FileUploader
    {
        public PushbulletSettings Config { get; private set; }

        public Pushbullet(PushbulletSettings config)
        {
            Config = config;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(Config.UserAPIKey)) throw new Exception("Missing API Key.");
            if (Config.CurrentDevice == null) throw new Exception("No device set to push to.");
            if (string.IsNullOrEmpty(Config.CurrentDevice.Key)) throw new Exception("Device key is empty.");

            NameValueCollection headers = CreateAuthenticationHeader(Config.UserAPIKey, "");

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("device_iden", Config.CurrentDevice.Key);
            args.Add("type", "file");

            UploadResult result = UploadData(stream, "https://api.pushbullet.com/api/pushes", fileName, arguments: args, headers: headers);

            PushbulletResponsePush response = JsonConvert.DeserializeObject<PushbulletResponsePush>(result.Response);

            if (response != null)
            {
                if (Config.ReturnPushURL)
                {
                    result.URL = "https://www.pushbullet.com/pushes?push_iden=" + response.iden;
                }
                else
                {
                    result.IsURLExpected = false;
                }
            }

            return result;
        }

        public List<PushbulletDevice> GetDeviceList()
        {
            NameValueCollection headers = CreateAuthenticationHeader(Config.UserAPIKey, "");

            string response = SendGetRequest("https://api.pushbullet.com/api/devices", headers: headers);

            PushbulletResponseDevices devicesResponse = JsonConvert.DeserializeObject<PushbulletResponseDevices>(response);

            if (devicesResponse != null && devicesResponse.devices != null)
            {
                return devicesResponse.devices.Select(x => new PushbulletDevice { Key = x.iden, Name = x.extras.nickname }).ToList();
            }

            return new List<PushbulletDevice>();
        }
    }

    public class PushbulletDevice
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public class PushbulletSettings
    {
        public string UserAPIKey = string.Empty;
        public bool ReturnPushURL = true;
        public List<PushbulletDevice> DeviceList = new List<PushbulletDevice>();
        public PushbulletDevice CurrentDevice = null; // TODO: Use int SelectedDevice
    }

    public class PushbulletResponseDevices
    {
        public List<PushbulletResponseDevice> devices { get; set; }
    }

    public class PushbulletResponseDevice
    {
        public string iden { get; set; }
        public PushbulletResponseDeviceExtras extras { get; set; }
    }

    public class PushbulletResponseDeviceExtras
    {
        public string manufacturer { get; set; }
        public string model { get; set; }
        public string android_version { get; set; }
        public string sdk_version { get; set; }
        public string app_version { get; set; }
        public string nickname { get; set; }
    }

    public class PushbulletResponsePush
    {
        public string iden { get; set; }
        public string device_iden { get; set; }
        public PushbulletResponsePushData data { get; set; }
        public long created { get; set; }
    }

    public class PushbulletResponsePushData
    {
        public string type { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}