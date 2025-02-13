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
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class VgymeImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Vgyme;

        public override Icon ServiceIcon => Resources.Vgyme;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new VgymeUploader()
            {
                UserKey = config.VgymeUserKey
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpVgyme;
    }

    public sealed class VgymeUploader : ImageUploader
    {
        public string UserKey { get; set; }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(UserKey)) args.Add("userkey", UserKey);

            UploadResult result = SendRequestFile("https://vgy.me/upload", stream, fileName, "file", args);

            if (result.IsSuccess)
            {
                VgymeResponse response = JsonConvert.DeserializeObject<VgymeResponse>(result.Response);

                if (response != null && !response.Error)
                {
                    result.URL = response.Image;
                    result.DeletionURL = response.Delete;
                }
            }

            return result;
        }

        private class VgymeResponse
        {
            public bool Error { get; set; }
            public string URL { get; set; }
            public string Image { get; set; }
            public long Size { get; set; }
            public string Filename { get; set; }
            public string Ext { get; set; }
            public string Delete { get; set; }
        }
    }
}