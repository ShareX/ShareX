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

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ShareX.UploadersLib.ImageUploaders
{
    public sealed class ImglandUploader : ImageUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            string uploadUrl = "http://www.imgland.net/process.php?subAPI=mainsite";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("usubmit", "true");

            UploadResult result = UploadData(stream, uploadUrl, fileName, "imagefile[]", arguments);

            if (result.IsSuccess)
            {
                ImglandResponse response = JsonConvert.DeserializeObject<ImglandResponse>(result.Response);

                if (!string.IsNullOrEmpty(response.Url))
                {
                    result.URL = response.Url;
                }
                else
                {
                    Errors.Add(string.IsNullOrEmpty(response.Status) ? "Unknown error." : response.Status);
                }
            }

            return result;
        }
    }

    public class ImglandResponse
    {
        public string Status { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}