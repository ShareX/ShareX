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

using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class CoinURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.CoinURL;

        public override Icon ServiceIcon => Resources.CoinURL;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.CoinURLUUID);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new CoinURLShortener
            {
                UUID = config.CoinURLUUID
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpCoinURL;
    }

    public sealed class CoinURLShortener : URLShortener
    {
        private const string API_ENDPOINT = "https://coinurl.com/api.php";
        public string UUID { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("uuid", UUID);
            args.Add("url", url);

            string response = SendRequest(HttpMethod.GET, API_ENDPOINT, args);

            if (!string.IsNullOrEmpty(response) && response != "error")
            {
                result.ShortenedURL = response;
            }

            return result;
        }
    }
}