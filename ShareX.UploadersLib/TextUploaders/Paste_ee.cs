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
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders
{
    public class Paste_eeTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Paste_ee;

        public override Image ServiceImage => Resources.document;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            string apiKey;

            if (!string.IsNullOrEmpty(config.Paste_eeUserKey))
            {
                apiKey = config.Paste_eeUserKey;
            }
            else
            {
                apiKey = APIKeys.Paste_eeApplicationKey;
            }

            return new Paste_ee(apiKey)
            {
                EncryptPaste = config.Paste_eeEncryptPaste
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPaste_ee;
    }

    public sealed class Paste_ee : TextUploader
    {
        public string APIKey { get; private set; }
        public bool EncryptPaste { get; set; }

        public Paste_ee(string apiKey)
        {
            APIKey = apiKey;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            if (string.IsNullOrEmpty(APIKey))
            {
                throw new Exception("API key is missing.");
            }

            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                Paste_eeSubmitRequestBody requestBody = new Paste_eeSubmitRequestBody()
                {
                    encrypted = EncryptPaste,
                    description = "",
                    expiration = "never",
                    sections = new Paste_eeSubmitRequestBodySection[]
                    {
                        new Paste_eeSubmitRequestBodySection()
                        {
                            name = "",
                            syntax = "autodetect",
                            contents = text
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);

                NameValueCollection headers = new NameValueCollection();
                headers.Add("X-Auth-Token", APIKey);

                ur.Response = SendRequest(HttpMethod.POST, "https://api.paste.ee/v1/pastes", json, RequestHelpers.ContentTypeJSON, null, headers);

                if (!string.IsNullOrEmpty(ur.Response))
                {
                    Paste_eeSubmitResponse response = JsonConvert.DeserializeObject<Paste_eeSubmitResponse>(ur.Response);

                    ur.URL = response.link;
                }
            }

            return ur;
        }
    }

    public class Paste_eeSubmitRequestBody
    {
        public bool encrypted { get; set; }
        public string description { get; set; }
        public string expiration { get; set; }
        public Paste_eeSubmitRequestBodySection[] sections { get; set; }
    }

    public class Paste_eeSubmitRequestBodySection
    {
        public string name { get; set; }
        public string syntax { get; set; }
        public string contents { get; set; }
    }

    public class Paste_eeSubmitResponse
    {
        public string id { get; set; }
        public string link { get; set; }
    }
}