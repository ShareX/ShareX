#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using System;
using System.Collections.Generic;

namespace UploadersLib.URLShorteners
{
    public sealed class YourlsURLShortener : URLShortener
    {
        public string APIURL { get; set; }
        public string Signature { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            if (!string.IsNullOrEmpty(url))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(Signature))
                {
                    arguments.Add("signature", Signature);
                }
                else if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    arguments.Add("username", Username);
                    arguments.Add("password", Password);
                }
                else
                {
                    throw new Exception("Signature or Username/Password is missing.");
                }

                arguments.Add("action", "shorturl");
                arguments.Add("url", url);
                //arguments.Add("keyword", "");
                //arguments.Add("title", "");
                arguments.Add("format", "simple");

                result.Response = SendGetRequest(APIURL, arguments);
                result.ShortenedURL = result.Response;
            }

            return result;
        }
    }
}