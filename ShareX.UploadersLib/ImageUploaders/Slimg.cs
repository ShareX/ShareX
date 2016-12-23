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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class SlimgImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Slimg;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            OAuth2Info oauth = new OAuth2Info(APIKeys.SlimgClientID, APIKeys.SlimgClientSecret);

            return new Slimg(oauth)
            {
                UploadMethod = AccountType.Anonymous
            };
        }
    }

    public sealed class Slimg : ImageUploader
    {
        public AccountType UploadMethod { get; set; }
        public OAuth2Info AuthInfo { get; set; }

        public Slimg(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();

            if (UploadMethod == AccountType.Anonymous)
            {
                headers.Add("Authorization", "Client-ID " + AuthInfo.Client_ID);
            }
            else if (UploadMethod == AccountType.User)
            {
                headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            }

            return headers;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("type", "binary");
            arguments.Add("shared", "false");

            UploadResult result = UploadData("https://api.sli.mg/media", stream, fileName, "data", arguments, GetAuthHeaders());

            if (result.IsSuccess)
            {
                SlimgResponse response = JsonConvert.DeserializeObject<SlimgResponse>(result.Response);

                while (!StopUploadRequested && response != null && response.success && response.data != null)
                {
                    if (response.data.status == 1)
                    {
                        result.URL = response.data.url_direct;
                        break;
                    }
                    else if (response.data.status == 20 || response.data.status == 21)
                    {
                        Thread.Sleep(200);

                        string media_response = SendRequest(HttpMethod.GET, "https://api.sli.mg/media/" + response.data.media_key, headers: GetAuthHeaders());

                        if (string.IsNullOrEmpty(media_response))
                        {
                            break;
                        }

                        response = JsonConvert.DeserializeObject<SlimgResponse>(media_response);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public class SlimgResponse
        {
            public bool success { get; set; }
            public int status { get; set; }
            public SlimgData data { get; set; }
        }

        public class SlimgData
        {
            public string media_key { get; set; }
            public string media_secret { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string username { get; set; }
            public int status { get; set; }
            public int created { get; set; }
            public string mimetype { get; set; }
            public string extension { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string url_direct { get; set; }
            public bool animated { get; set; }
            public bool gifv { get; set; }
            public string url_gifv { get; set; }
            public bool webm { get; set; }
            public string url_webm { get; set; }
            public bool mp4 { get; set; }
            public string url_mp4 { get; set; }
            public int height { get; set; }
            public int width { get; set; }
            public List<SlimgTag> tags { get; set; }
            public string album_key { get; set; }
        }

        public class SlimgTag
        {
            public string tag_key { get; set; }
            public string url { get; set; }
        }
    }
}