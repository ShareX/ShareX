#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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

namespace ShareX.UploadersLib.FileUploaders
{
    public class Pomf : FileUploader
    {
        public static List<PomfUploader> Uploaders = new List<PomfUploader>()
        {
            new PomfUploader("1339.cf", "http://1339.cf/upload.php", "http://b.1339.cf"),
            new PomfUploader("bucket.pw", "https://bucket.pw/upload.php", "https://dl.bucket.pw"),
            new PomfUploader("maxfile.ro", "https://maxfile.ro/static/upload.php", "https://d.maxfile.ro"),
            new PomfUploader("mixtape.moe", "https://mixtape.moe/upload.php"),
            new PomfUploader("pantsu.cat", "https://pantsu.cat/upload.php"),
            new PomfUploader("pomf.cat", "https://pomf.cat/upload.php", "http://a.pomf.cat"),
            new PomfUploader("pomf.hummingbird.moe", "http://pomf.hummingbird.moe/upload.php", "http://a.pomf.hummingbird.moe"),
            new PomfUploader("pomf.io", "http://pomf.io/upload.php"),
            new PomfUploader("pomf.pl", "http://pomf.pl/upload.php", "http://i.pomf.pl")
            //new PomfUploader("pomf.se", "https://pomf.se/upload.php", "https://a.pomf.se")
        };

        public PomfUploader Uploader { get; set; }

        public Pomf(PomfUploader uploader)
        {
            Uploader = uploader;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = UploadData(stream, Uploader.UploadURL, fileName, "files[]");

            if (result.IsSuccess)
            {
                PomfResponse response = JsonConvert.DeserializeObject<PomfResponse>(result.Response);

                if (response.success && response.files != null && response.files.Count > 0)
                {
                    string url = response.files[0].url;

                    if (!string.IsNullOrEmpty(Uploader.ResultURL))
                    {
                        url = URLHelpers.CombineURL(Uploader.ResultURL, url);
                    }

                    result.URL = url;
                }
            }

            return result;
        }

        private class PomfResponse
        {
            public bool success { get; set; }
            public object error { get; set; }
            public List<PomfFile> files { get; set; }
        }

        private class PomfFile
        {
            public string hash { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string size { get; set; }
        }
    }
}