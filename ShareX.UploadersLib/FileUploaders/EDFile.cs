#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

namespace ShareX.UploadersLib.FileUploaders
{
    public class EDFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.EDFile;

        public override Icon ServiceIcon => Resources.EDFile;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.EDFileSettings != null; //&& !string.IsNullOrEmpty(config.EDFileSettings.UserUploadToken);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new EDFile(config.EDFileSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpEDFile;
    }

    public sealed class EDFile : FileUploader
    {
        public EDFileSettings Config { get; private set; }

        public EDFile(EDFileSettings config)
        {
            Config = config;
        }

        private const string uploadUrl = "https://edfile.pro/upload/archive";

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("userkey", Config.UserUploadToken);
            UploadResult result = SendRequestFile(uploadUrl, stream, fileName, "files[]", arguments);

            EDFileResponse response = JsonConvert.DeserializeObject<EDFileResponse>(result.Response);

            if (response.success && response.files != null && response.files.Count > 0)
            {
                if (Config.ImageDirect && !string.IsNullOrEmpty(response.files[0].direct))
                {
                    result.URL = response.files[0].direct;
                }
                else
                {
                    result.URL = response.files[0].url;
                }
            }

            return result;
        }

        public class EDFileResponse
        {
            public bool success { get; set; }
            public object error { get; set; }
            public List<EDFileFile> files { get; set; }
        }

        public class EDFileFile
        {
            public string hash { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string direct { get; set; }
            public string size { get; set; }
        }
    }

    public class EDFileSettings
    {
        public string UserUploadToken { get; set; } = "";
        public bool ImageDirect { get; set; } = false;
    }
}