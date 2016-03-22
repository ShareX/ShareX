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

using Newtonsoft.Json;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class HostrFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Localhostr;

        public override bool CheckConfig(UploadersConfig uploadersConfig)
        {
            return !string.IsNullOrEmpty(uploadersConfig.LocalhostrEmail) && !string.IsNullOrEmpty(uploadersConfig.LocalhostrPassword);
        }

        public override FileUploader CreateUploader(UploadersConfig uploadersConfig, TaskReferenceHelper taskInfo)
        {
            return new Hostr(uploadersConfig.LocalhostrEmail, uploadersConfig.LocalhostrPassword)
            {
                DirectURL = uploadersConfig.LocalhostrDirectURL
            };
        }
    }

    public sealed class Hostr : FileUploader
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool DirectURL { get; set; }

        public Hostr(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = null;

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                NameValueCollection headers = CreateAuthenticationHeader(Email, Password);
                result = UploadData(stream, "https://api.hostr.co/file", fileName, headers: headers);

                if (result.IsSuccess)
                {
                    HostrFileUploadResponse response = JsonConvert.DeserializeObject<HostrFileUploadResponse>(result.Response);

                    if (response != null)
                    {
                        if (DirectURL && response.direct != null)
                        {
                            result.URL = string.Format("http://hostr.co/file/{0}/{1}", response.id, response.name);
                            result.ThumbnailURL = response.direct.direct_150x;
                        }
                        else
                        {
                            result.URL = response.href;
                        }
                    }
                }
            }

            return result;
        }

        public class HostrFileUploadResponse
        {
            public string added { get; set; }
            public string name { get; set; }
            public string href { get; set; }
            public int size { get; set; }
            public string type { get; set; }
            public HostrFileUploadResponseDirect direct { get; set; }
            public string id { get; set; }
        }

        public class HostrFileUploadResponseDirect
        {
            [JsonProperty("150x")]
            public string direct_150x { get; set; }

            [JsonProperty("930x")]
            public string direct_930x { get; set; }
        }
    }
}