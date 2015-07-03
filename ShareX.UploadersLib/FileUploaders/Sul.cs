#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using System.IO;
using System.Collections.Generic;
using ShareX.HelpersLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class SulUploader : FileUploader
    {

        private string APIKey { get; set; }

        public SulUploader(string apiKey)
        {
            APIKey = apiKey;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("wizard", "true");
            args.Add("key", APIKey);
            args.Add("client", "sharex-native");


            string url = "https://s-ul.eu";
            url = URLHelpers.CombineURL(url, "upload.php");

            UploadResult result = UploadData(stream, url, fileName, "file", args);

            if (result.IsSuccess)
            {
                JToken jsonResponse = JToken.Parse(result.Response);

                string protocol = "";
                string domain = "";
                string file = "";
                string extension = "";
                string error = "";

                if (jsonResponse != null)
                {
                    protocol = (string)jsonResponse.SelectToken("protocol");
                    domain = (string)jsonResponse.SelectToken("domain");
                    file = (string)jsonResponse.SelectToken("filename");
                    extension = (string)jsonResponse.SelectToken("extension");
                    error = (string)jsonResponse.SelectToken("error");
                }

                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(protocol))
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        Errors.Add("Generic error occurred, please contact support@s-ul.eu");
                    }
                    else
                    {
                        Errors.Add(error);
                    }
                }
                else
                {
                    result.URL = protocol + domain + "/" + file + extension;
                    result.DeletionURL =  "https://s-ul.eu/delete.php?key=" + APIKey + "&file=" + file;
                }
            }

            return result;
        }
    }
}