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

// Credits: https://github.com/mstojcevich

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class Lambda : FileUploader
    {
        public LambdaSettings Config { get; private set; }

        public Lambda(LambdaSettings config)
        {
            Config = config;
        }

        private const string uploadUrl = "https://lambda.sx/upload";
        private const string responseUrl = "https://λ.pw/";

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(Config.UserAPIKey))
            {
                Errors.Add("Missing API key. Set one in destination settings.");
                return null;
            }

            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("apikey", Config.UserAPIKey);
            UploadResult result = UploadData(stream, uploadUrl, fileName, "file", arguments);

            if (result.IsSuccess)
            {
                LambdaResponse response = JsonConvert.DeserializeObject<LambdaResponse>(result.Response);

                if (response.success)
                {
                    result.URL = responseUrl + response.files[0].url;
                }
                else
                {
                    foreach (string e in response.errors)
                    {
                        Errors.Add(e);
                    }
                }
            }

            return result;
        }

        internal class LambdaResponse
        {
            public bool success { get; set; }
            public List<LambdaFile> files { get; set; }
            public List<string> errors { get; set; }
        }

        internal class LambdaFile
        {
            public string url { get; set; }
        }
    }

    public class LambdaSettings
    {
        public string UserAPIKey = string.Empty;
    }
}