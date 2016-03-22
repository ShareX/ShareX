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

// Credits: https://github.com/DanielMcAssey

using ShareX.HelpersLib;
using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners
{
    public class PolrURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.Polr;

        public override bool CheckConfig(UploadersConfig uploadersConfig)
        {
            return !string.IsNullOrEmpty(uploadersConfig.PolrAPIKey);
        }

        public override URLShortener CreateShortener(UploadersConfig uploadersConfig, TaskReferenceHelper taskInfo)
        {
            return new PolrURLShortener
            {
                API_HOST = uploadersConfig.PolrAPIHostname,
                API_KEY = uploadersConfig.PolrAPIKey
            };
        }
    }

    public sealed class PolrURLShortener : URLShortener
    {
        public string API_HOST { get; set; }
        public string API_KEY { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (string.IsNullOrEmpty(API_HOST))
            {
                API_HOST = "https://polr.me/publicapi.php";
                API_KEY = null;
            }
            else
            {
                API_HOST = URLHelpers.FixPrefix(API_HOST);
            }

            Dictionary<string, string> args = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(API_KEY))
            {
                args.Add("apikey", API_KEY);
            }

            args.Add("action", "shorten");
            args.Add("url", url);

            string response = SendRequest(HttpMethod.GET, API_HOST, args);

            if (!string.IsNullOrEmpty(response))
            {
                result.ShortenedURL = response;
            }

            return result;
        }
    }
}