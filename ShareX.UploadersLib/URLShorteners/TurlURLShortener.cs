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

using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners
{
    public class TurlURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.TURL;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TurlURLShortener();
        }
    }

    public sealed class TurlURLShortener : URLShortener
    {
        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("url", url);

                result.Response = SendRequest(HttpMethod.GET, "http://turl.ca/api.php", arguments);

                if (!string.IsNullOrEmpty(result.Response))
                {
                    if (result.Response.StartsWith("SUCCESS:"))
                    {
                        result.ShortenedURL = "http://turl.ca/" + result.Response.Substring(8);
                    }

                    if (result.Response.StartsWith("ERROR:"))
                    {
                        Errors.Add(result.Response.Substring(6));
                    }
                }
            }

            return result;
        }
    }
}