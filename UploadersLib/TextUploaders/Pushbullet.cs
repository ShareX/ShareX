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

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net;
using UploadersLib.HelperClasses;

namespace UploadersLib.TextUploaders
{
    public sealed class Pushbullet : TextUploader
    {
        public string APIKey { get; private set; }
        public PushbulletSettings Config { get; set; }

        public Pushbullet(string apiKey, PushbulletSettings config)
        {
            APIKey = apiKey;
            Config = config;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            if (string.IsNullOrEmpty(APIKey)) { throw new System.NullReferenceException("Missing API Key"); }
            if (string.IsNullOrEmpty(text)) { throw new System.NullReferenceException("Nothing to push"); }
            if (Config.CurrentDevice == null) { throw new System.NullReferenceException("No device set to push to"); }

            string strDeviceKey = Config.CurrentDevice.DeviceKey;

            if (string.IsNullOrEmpty(strDeviceKey)) { throw new System.NullReferenceException("For some reason, the device key was empty"); }

            UploadResult MakeURLHandlerHappy = new UploadResult();

            try
            {
                System.Net.WebRequest request = WebRequest.Create("https://api.pushbullet.com/api/pushes");

                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                request.Credentials = new NetworkCredential(APIKey, "");
                request.Method = "POST";

                string pushData = "";

                pushData += "device_iden=" + strDeviceKey + "&";
                pushData += "type=note&"; //For now set it to 'note' until it's dynamic w/ ShareX...
                pushData += "title=ShareX: " + System.DateTime.UtcNow + "&";
                pushData += "body=" + text;

                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(pushData);

                request.ContentLength = postBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                }

                string strPushURL = "";

                //Due to the nature of Pushbullet, it might not even be desirable to copy the URL
                //Plus, might as well not go through the trouble of getting it if the user doesn't want it
                if (Config.ReturnPushURL)
                {
                    WebResponse response = request.GetResponse();

                    JObject jObject = null;

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string strResponse = reader.ReadLine();

                        MakeURLHandlerHappy.Response = strResponse;

                        if (!string.IsNullOrEmpty(strResponse) && strResponse.StartsWith("error"))
                        {
                            throw new WebException(strResponse);
                        }

                        jObject = JObject.Parse(strResponse);
                    }

                    strPushURL = "https://www.pushbullet.com/pushes?push_iden=" + jObject["iden"];

                    MakeURLHandlerHappy.URL = strPushURL;
                }
                else
                {
                    MakeURLHandlerHappy.URL = System.Windows.Forms.Clipboard.GetText();
                }

                MakeURLHandlerHappy.IsSuccess = true;

                return MakeURLHandlerHappy;
            }
            catch (System.Exception ex)
            {
                Errors.Add(ex.ToString());

                MakeURLHandlerHappy.IsSuccess = false;
            }

            return null;
        }

        public List<PushbulletDevice> GetDeviceList()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://api.pushbullet.com/api/devices");

                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                request.Credentials = new NetworkCredential(APIKey, "");
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

                    foreach (Newtonsoft.Json.Linq.JObject device in jObject["devices"])
                    {
                        PushbulletDevice currDevice = new PushbulletDevice();

                        currDevice.DeviceKey = device["iden"].ToString();
                        currDevice.DeviceName = device["extras"]["nickname"].ToString();

                        deviceList.Add(currDevice);
                    }

                    return deviceList;
                }
            }
            catch (System.Exception ex)
            {
                Errors.Add(ex.ToString());

                return null;
            }
        }
    }

    public class PushbulletDevice
    {
        public string DeviceName = string.Empty;
        public string DeviceKey = string.Empty;

        public override string ToString()
        {
            return "Name:" + DeviceName + " | ID:" + DeviceKey;
        }
    }

    public class PushbulletSettings
    {
        public string UserAPIKey = string.Empty;
        public bool HideAPIChars = true;
        public bool ReturnPushURL = false;

        public PushbulletDevice CurrentDevice { get; set; }

        public List<PushbulletDevice> DeviceList { get; set; }
    }
}