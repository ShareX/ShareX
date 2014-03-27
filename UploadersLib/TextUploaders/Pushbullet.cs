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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using UploadersLib.HelperClasses;

namespace UploadersLib.TextUploaders
{
    public sealed class Pushbullet : TextUploader
    {
        public PushbulletSettings Config { get; private set; }

        public Pushbullet(PushbulletSettings config)
        {
            Config = config;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            if (string.IsNullOrEmpty(Config.UserAPIKey)) throw new Exception("Missing API Key.");
            if (string.IsNullOrEmpty(text)) throw new Exception("Nothing to push.");
            if (Config.CurrentDevice == null) throw new Exception("No device set to push to.");

            string strDeviceKey = Config.CurrentDevice.Key;

            if (string.IsNullOrEmpty(strDeviceKey)) throw new Exception("For some reason, the device key was empty.");

            UploadResult result = new UploadResult();

            try
            {
                WebRequest request = WebRequest.Create("https://api.pushbullet.com/api/pushes");

                request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
                request.Credentials = new NetworkCredential(Config.UserAPIKey, "");
                request.Method = "POST";

                string pushData = "";

                pushData += "device_iden=" + strDeviceKey + "&";
                pushData += "type=note&"; //For now set it to 'note' until it's dynamic w/ ShareX...
                pushData += "title=ShareX: " + DateTime.UtcNow + "&";
                pushData += "body=" + text;

                byte[] postBytes = Encoding.UTF8.GetBytes(pushData);

                request.ContentLength = postBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                }

                string strPushURL = "";

                if (Config.ReturnPushURL)
                {
                    WebResponse response = request.GetResponse();

                    JObject jObject = null;

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string strResponse = reader.ReadLine();

                        result.Response = strResponse;

                        if (!string.IsNullOrEmpty(strResponse) && strResponse.StartsWith("error"))
                        {
                            throw new WebException(strResponse);
                        }

                        jObject = JObject.Parse(strResponse);
                    }

                    strPushURL = "https://www.pushbullet.com/pushes?push_iden=" + jObject["iden"];

                    result.URL = strPushURL;
                }
                else
                {
                    result.IsURLExpected = false;
                }

                result.IsSuccess = true;

                return result;
            }
            catch (Exception ex)
            {
                Errors.Add(ex.ToString());

                result.IsSuccess = false;
            }

            return null;
        }

        public List<PushbulletDevice> GetDeviceList()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://api.pushbullet.com/api/devices");

                request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
                request.Credentials = new NetworkCredential(Config.UserAPIKey, "");
                request.Method = "GET";

                WebResponse response = request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string strResponse = reader.ReadLine();

                    if (!string.IsNullOrEmpty(strResponse) && strResponse.StartsWith("error"))
                    {
                        Errors.Add(strResponse);

                        return null;
                    }

                    JObject jObject = JObject.Parse(strResponse);

                    List<PushbulletDevice> deviceList = new List<PushbulletDevice>();

                    foreach (JObject device in jObject["devices"])
                    {
                        PushbulletDevice currDevice = new PushbulletDevice();

                        currDevice.Key = device["iden"].ToString();
                        currDevice.Name = device["extras"]["nickname"].ToString();

                        deviceList.Add(currDevice);
                    }

                    return deviceList;
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex.ToString());

                return null;
            }
        }
    }

    public class PushbulletDevice
    {
        public string Name = string.Empty;
        public string Key = string.Empty;

        public override string ToString()
        {
            return string.Format("Name: {0} ({1})", Name, Key);
        }
    }

    public class PushbulletSettings
    {
        public string UserAPIKey = string.Empty;
        public bool ReturnPushURL = false;

        public List<PushbulletDevice> DeviceList { get; set; }

        public PushbulletDevice CurrentDevice { get; set; } // TODO: Use int SelectedDevice
    }
}