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

using HelpersLib;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace UploadersLib.FileUploaders
{
    public class FileSonic : FileUploader
    {
        public string Username { get; set; }

        public string Password { get; set; }

        private const string APIURL = "http://api.filesonic.com/upload";

        public FileSonic(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = null;

            string url = GetUploadURL();

            if (!string.IsNullOrEmpty(url))
            {
                result = UploadData(stream, url, fileName);

                if (!string.IsNullOrEmpty(result.Response))
                {
                    result.URL = result.Response;
                }
            }
            else
            {
                Errors.Add("GetUploadURL failed.");
            }

            return result;
        }

        public string GetUploadURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "getUploadUrl");
            args.Add("format", "xml");
            args.Add("u", Username);
            args.Add("p", Password);

            string response = SendGetRequest(APIURL, args);

            XDocument xd = XDocument.Parse(response);
            return xd.GetValue("FSApi_Upload/getUploadUrl/response/url");
        }
    }
}