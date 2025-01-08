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

using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class PolrURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.Polr;

        public override Icon ServiceIcon => Resources.Polr;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.PolrAPIHostname) && !string.IsNullOrEmpty(config.PolrAPIKey);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new PolrURLShortener
            {
                Host = config.PolrAPIHostname,
                Key = config.PolrAPIKey,
                IsSecret = config.PolrIsSecret,
                UseAPIv1 = config.PolrUseAPIv1
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPolr;
    }

    public sealed class PolrURLShortener : URLShortener
    {
        public string Host { get; set; }
        public string Key { get; set; }
        public bool IsSecret { get; set; }
        public bool UseAPIv1 { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            Host = URLHelpers.FixPrefix(Host);

            Dictionary<string, string> args = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(Key))
            {
                if (UseAPIv1)
                {
                    args.Add("apikey", Key);
                }
                else
                {
                    args.Add("key", Key);
                }
            }

            if (UseAPIv1)
            {
                args.Add("action", "shorten");
            }

            args.Add("url", url);

            if (IsSecret && !UseAPIv1)
            {
                args.Add("is_secret", "true");
            }

            string response = SendRequest(HttpMethod.GET, Host, args);

            if (!string.IsNullOrEmpty(response))
            {
                result.ShortenedURL = response;
            }

            return result;
        }
    }
}