#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
    public class KuttURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.Kutt;

        public override Image ServiceImage => Resources.Kutt;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.KuttSettings.APIKey);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new KuttURLShortener(config.KuttSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpKutt;
    }

    public sealed class KuttURLShortener : URLShortener
    {
        public KuttSettings Settings { get; set; }

        public KuttURLShortener(KuttSettings settings)
        {
            Settings = settings;
        }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };
            result.ShortenedURL = Submit(url);
            return result;
        }

        public string Submit(string url)
        {
            if (string.IsNullOrEmpty(Settings.Host))
            {
                Settings.Host = "https://kutt.it";
            }
            else
            {
                Settings.Host = URLHelpers.FixPrefix(Settings.Host);
            }

            string requestURL = URLHelpers.CombineURL(Settings.Host, "/api/url/submit");

            KuttSubmitRequest body = new KuttSubmitRequest()
            {
                target = url,
                customurl = null,
                password = Settings.Password,
                reuse = Settings.Reuse
            };

            string json = JsonConvert.SerializeObject(body);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("X-API-Key", Settings.APIKey);

            string response = SendRequest(HttpMethod.POST, requestURL, json, UploadHelpers.ContentTypeJSON, headers: headers);

            if (!string.IsNullOrEmpty(response))
            {
                KuttSubmitResponse submitResponse = JsonConvert.DeserializeObject<KuttSubmitResponse>(response);

                if (submitResponse != null)
                {
                    return submitResponse.shortUrl;
                }
            }

            return null;
        }

        private class KuttSubmitRequest
        {
            /// <summary>Original long URL to be shortened.</summary>
            public string target { get; set; }

            /// <summary>(optional) Set a custom URL.</summary>
            public string customurl { get; set; }

            /// <summary>(optional) Set a password.</summary>
            public string password { get; set; }

            /// <summary>(optional) If a URL with the specified target exists returns it, otherwise will send a new shortened URL.</summary>
            public bool reuse { get; set; }
        }

        private class KuttSubmitResponse
        {
            /// <summary>Unique ID of the URL</summary>
            public string id { get; set; }

            /// <summary>The shortened link</summary>
            public string shortUrl { get; set; }
        }
    }

    public class KuttSettings
    {
        public string APIKey { get; set; }
        public string Host { get; set; } = "https://kutt.it";
        public string Password { get; set; }
        public bool Reuse { get; set; }
    }
}