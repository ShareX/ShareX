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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders
{
    public class UpasteTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Upaste;

        public override Icon ServiceIcon => Resources.Upaste;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Upaste(config.UpasteUserKey)
            {
                IsPublic = config.UpasteIsPublic
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpUpaste;
    }

    public sealed class Upaste : TextUploader
    {
        private const string APIURL = "http://upaste.me/api";

        public string UserKey { get; private set; }
        public bool IsPublic { get; set; }

        public Upaste(string userKey)
        {
            UserKey = userKey;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(UserKey))
                {
                    arguments.Add("api_key", UserKey);
                }
                arguments.Add("paste", text);
                //arguments.Add("syntax", "");
                //arguments.Add("name", "");
                arguments.Add("privacy", IsPublic ? "0" : "1"); // 0 public 1 private
                arguments.Add("expire", "0");
                arguments.Add("json", "true");

                ur.Response = SendRequestMultiPart(APIURL, arguments);

                if (!string.IsNullOrEmpty(ur.Response))
                {
                    UpasteResponse response = JsonConvert.DeserializeObject<UpasteResponse>(ur.Response);

                    if (response != null)
                    {
                        if (response.status.Equals("success", StringComparison.OrdinalIgnoreCase))
                        {
                            ur.URL = response.paste.link;
                        }
                        else
                        {
                            Errors.Add(response.error);
                        }
                    }
                }
            }

            return ur;
        }

        public class UpastePaste
        {
            public string id { get; set; }
            public string link { get; set; }
            public string raw { get; set; }
            public string download { get; set; }
        }

        public class UpasteResponse
        {
            public UpastePaste paste { get; set; }
            public int errorcode { get; set; }
            public string error { get; set; }
            public string status { get; set; }
        }
    }
}