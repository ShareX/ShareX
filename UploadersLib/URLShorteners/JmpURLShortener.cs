#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using System.Xml;
using UploadersLib.HelperClasses;

namespace UploadersLib.URLShorteners
{
    public sealed class JmpURLShortener : URLShortener
    {
        private const string APIURL = "http://api.j.mp/shorten";

        private string APILogin, APIKey;

        public JmpURLShortener(string login, string key)
        {
            APILogin = login;
            APIKey = key;
        }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("version", "2.0.1");
                arguments.Add("longUrl", url);
                arguments.Add("login", APILogin);
                arguments.Add("apiKey", APIKey);
                arguments.Add("format", "xml");

                result.Response = SendGetRequest(APIURL, arguments);

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(result.Response);
                XmlNode xnode = xdoc.SelectSingleNode("bitly/results/nodeKeyVal/shortUrl");
                if (xnode != null)
                {
                    result.ShortenedURL = xnode.InnerText;
                }
            }

            return result;
        }
    }
}