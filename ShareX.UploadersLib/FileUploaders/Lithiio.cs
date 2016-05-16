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

// Credits: https://github.com/lithium720

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class LithiioFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Lithiio;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.LithiioSettings != null;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Lithiio(config.LithiioSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpLithiio;
    }

    public sealed class Lithiio : FileUploader
    {
        public LithiioSettings Config { get; private set; }

        public Lithiio(LithiioSettings config)
        {
            Config = config;
        }

        private const string uploadUrl = "http://api.lithi.io/upload.php";

        public static string[] UploadURLs = new string[] { "https://i.lithi.io/", "https://lithi.io/i/", "https://i.mugi.io/", "https://mugi.io/i/"};

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("key", Config.UserAPIKey);
            UploadResult result = UploadData(stream, uploadUrl, fileName, "file", arguments, method: HttpMethod.POST);

            if (result.Response == null)
            {
                Errors.Add("Upload failed for unknown reason. Check your API key.");
                return result;
            }

            if (result.IsSuccess)
            {
                result.URL = Config.UploadURL + result.Response;
            }

            return result;
        }

        internal class LithiioResponse
        {
            public string url { get; set; }
            public List<string> errors { get; set; }
        }

        internal class LithiioFile
        {
            public string url { get; set; }
        }
    }

    public class LithiioSettings
    {
        public string UserAPIKey { get; set; } = string.Empty;
        public string UploadURL { get; set; } = "https://i.lithi.io/";
    }
}