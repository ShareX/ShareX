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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class CheveretoImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Chevereto;

        public override Image ServiceImage => Resources.Chevereto;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CheveretoUploader != null && !string.IsNullOrEmpty(config.CheveretoUploader.UploadURL) &&
                !string.IsNullOrEmpty(config.CheveretoUploader.APIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Chevereto(config.CheveretoUploader)
            {
                DirectURL = config.CheveretoDirectURL
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpChevereto;
    }

    public sealed class Chevereto : ImageUploader
    {
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

            UploadResult result = SendRequestFile(url, stream, fileName, "source", args);

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