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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System.Collections.Generic;
using System.IO;

namespace ShareX.UploadersLib.ImageUploaders
{
    public sealed class Chevereto : ImageUploader
    {
        public static List<CheveretoUploader> Uploaders = new List<CheveretoUploader>()
        {
            new CheveretoUploader("http://ultraimg.com/api/1/upload", "3374fa58c672fcaad8dab979f7687397"),
            new CheveretoUploader("http://yukle.at/api/1/upload", "ee24aee90bcd24e39cead57c65044bde"),
            new CheveretoUploader("http://img.patifile.com/api/1/upload", "8320784a9b044510e8c723fb778fe3b7"),
            new CheveretoUploader("http://boltimg.com/api/1/upload", "8dfbcb7ab9b5258a90be7cf09e361894"),
            new CheveretoUploader("http://snapie.net/myapi/1/upload", "aff7bd5bf65b7e30b675a430049894b3"),
            new CheveretoUploader("http://picgur.org/api/1/upload", "0a65553c54cf72127d11281f96518469")
        };

        public CheveretoUploader Uploader { get; private set; }

        public bool DirectURL { get; set; }

        public Chevereto(CheveretoUploader uploader)
        {
            Uploader = uploader;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("key", Uploader.APIKey);
            args.Add("format", "json");

            string url = URLHelpers.FixPrefix(Uploader.UploadURL);

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