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
using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners
{
    public class TwoGPURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.TwoGP;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TwoGPURLShortener();
        }
    }

    public sealed class TwoGPURLShortener : URLShortener
    {
        private const string API_ENDPOINT = "http://2.gp/api/short";

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("longurl", url);

            string response = SendRequest(HttpMethod.GET, API_ENDPOINT, args);

            if (!string.IsNullOrEmpty(response))
            {
                TwoGPURLShortenerResponse jsonResponse = JsonConvert.DeserializeObject<TwoGPURLShortenerResponse>(response);

                if (jsonResponse != null)
                {
                    result.ShortenedURL = jsonResponse.url;
                }
            }

            return result;
        }
    }

    public class TwoGPURLShortenerResponse
    {
        public string facebook_url { get; set; }
        public string stat_url { get; set; }
        public string twitter_url { get; set; }
        public string url { get; set; }
        public string target_host { get; set; }
        public string host { get; set; }
    }
}