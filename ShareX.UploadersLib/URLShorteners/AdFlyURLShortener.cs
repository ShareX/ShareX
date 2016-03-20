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

// Credits: https://github.com/LRNAB

using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners
{
    public class AdFlyURLShortener : URLShortener
    {
        public string APIKEY { get; set; }
        public string APIUID { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("key", APIKEY);
            args.Add("uid", APIUID);
            args.Add("advert_type", "int");
            args.Add("domain", "adf.ly");
            args.Add("url", url);

            string response = SendRequest(HttpMethod.GET, "http://api.adf.ly/api.php", args);

            if (!string.IsNullOrEmpty(response) && response != "error")
            {
                result.ShortenedURL = response;
            }

            return result;
        }
    }

    public class AdFlyURLShortenerService : IURLShortenerService
    {
        public string ServiceId { get; } = "AdFly";
        public UrlShortenerType EnumValue { get; } = UrlShortenerType.AdFly;

        public IURLShortener CreateShortener(UploadersConfig config)
        {
            return new AdFlyURLShortener
            {
                APIKEY = config.AdFlyAPIKEY,
                APIUID = config.AdFlyAPIUID
            };
        }
    }
}