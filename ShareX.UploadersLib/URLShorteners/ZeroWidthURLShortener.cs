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
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class ZeroWidthURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.ZeroWidthShortener;

        public override Image ServiceImage => Resources.ZeroWidthShortener;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new ZeroWidthURLShortener()
            {
                RequestURL = config.ZeroWidthShortenerURL,
                Token = config.ZeroWidthShortenerToken
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpZeroWidthShortener;
    }

    public sealed class ZeroWidthURLShortener : URLShortener
    {
        public string RequestURL { get; set; }
        public string Token { get; set; }

        private NameValueCollection GetAuthHeaders()
        {
            if (!string.IsNullOrEmpty(Token))
            {
                NameValueCollection headers = new NameValueCollection();
                headers.Add("Authorization", "Bearer " + Token);
                return headers;
            }

            return null;
        }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            string json = JsonConvert.SerializeObject(new
            {
                url = url
            });

            if (string.IsNullOrEmpty(RequestURL))
            {
                RequestURL = "https://api.zws.im";
            }

            NameValueCollection headers = GetAuthHeaders();

            string response = SendRequest(HttpMethod.POST, RequestURL, json, RequestHelpers.ContentTypeJSON, null, headers);

            if (!string.IsNullOrEmpty(response))
            {
                ZeroWidthURLShortenerResponse jsonResponse = JsonConvert.DeserializeObject<ZeroWidthURLShortenerResponse>(response);

                if (jsonResponse != null)
                {
                    if (!string.IsNullOrEmpty(jsonResponse.URL))
                    {
                        result.ShortenedURL = jsonResponse.URL;
                    }
                    else
                    {
                        result.ShortenedURL = URLHelpers.CombineURL("https://zws.im", jsonResponse.Short);
                    }
                }
            }

            return result;
        }
    }

    public class ZeroWidthURLShortenerResponse
    {
        public string Short { get; set; }
        public string URL { get; set; }
    }
}