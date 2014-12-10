#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ShareX.UploadersLib.ImageUploaders
{
    public sealed class Chevereto : ImageUploader
    {
        public string Website { get; set; }
        public string APIKey { get; set; }
        public bool DirectURL { get; set; }

        public Chevereto(string website, string apiKey)
        {
            APIKey = apiKey;
            Website = website;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("key", APIKey);
            args.Add("format", "json");

            string url = URLHelpers.FixPrefix(Website);
            url = URLHelpers.CombineURL(url, "api/1/upload");

            UploadResult result = UploadData(stream, url, fileName, "source", args);

            if (result.IsSuccess)
            {
                CheveretoResponse response = JsonConvert.DeserializeObject<CheveretoResponse>(result.Response);

                if (response != null && response.Image != null)
                {
                    result.URL = DirectURL ? response.Image.URL : response.Image.URL_Viewer;

                    if (response.Image.Thumb != null)
                    {
                        result.ThumbnailURL = response.Image.Thumb.URL;
                    }
                }
            }

            return result;
        }

        private class CheveretoResponse
        {
            public CheveretoImage Image { get; set; }
        }

        private class CheveretoImage
        {
            public string URL { get; set; }
            public string URL_Viewer { get; set; }
            public CheveretoThumb Thumb { get; set; }
        }

        private class CheveretoThumb
        {
            public string URL { get; set; }
        }
    }
}