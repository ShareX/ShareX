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

namespace ShareX.UploadersLib.FileUploaders
{
    public class LambdaFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Lambda;

        public override Icon ServiceIcon => Resources.Lambda;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.LambdaSettings != null && !string.IsNullOrEmpty(config.LambdaSettings.UserAPIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            // Correct old URLs
            if (config.LambdaSettings != null && config.LambdaSettings.UploadURL == "https://λ.pw/")
            {
                config.LambdaSettings.UploadURL = "https://lbda.net/";
            }

            return new Lambda(config.LambdaSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpLambda;
    }

    public sealed class Lambda : FileUploader
    {
        public LambdaSettings Config { get; private set; }

        public Lambda(LambdaSettings config)
        {
            Config = config;
        }

        private const string uploadUrl = "https://lbda.net/api/upload";

        public static string[] UploadURLs = new string[] { "https://lbda.net/", "https://lambda.sx/" };

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("api_key", Config.UserAPIKey);
            UploadResult result = SendRequestFile(uploadUrl, stream, fileName, "file", arguments, method: HttpMethod.PUT);

            if (result.Response == null)
            {
                Errors.Add("Upload failed for unknown reason. Check your API key.");
                return result;
            }

            LambdaResponse response = JsonConvert.DeserializeObject<LambdaResponse>(result.Response);
            if (result.IsSuccess)
            {
                result.URL = Config.UploadURL + response.url;
            }
            else
            {
                foreach (string e in response.errors)
                {
                    Errors.Add(e);
                }
            }

            return result;
        }

        internal class LambdaResponse
        {
            public string url { get; set; }
            public List<string> errors { get; set; }
        }

        internal class LambdaFile
        {
            public string url { get; set; }
        }
    }

    public class LambdaSettings
    {
        [JsonEncrypt]
        public string UserAPIKey { get; set; } = "";
        public string UploadURL { get; set; } = "https://lbda.net/";
    }
}