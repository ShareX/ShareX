#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class ImgFishFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.ImgFish;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.ImgFishSettings != null &&
                config.ImgFishSettings.FileIDLength >= ImgFishSettings.MinFileIDLength &&
                config.ImgFishSettings.FileIDLength <= ImgFishSettings.MaxFileIDLength;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new ImgFish(config.ImgFishSettings);
        }
    }

    public sealed class ImgFishSettings
    {
        public const int MinFileIDLength = 8;
        public const int MaxFileIDLength = 32;

        [JsonEncrypt]
        public string APIKey { get; set; } = "";

        public int FileIDLength { get; set; } = MinFileIDLength;
    }

    public sealed class ImgFish : FileUploader
    {
        private const string UploadURL = "https://img.fish/up";

        public ImgFishSettings Settings { get; }

        public ImgFish(ImgFishSettings settings)
        {
            Settings = settings;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                ["length"] = Settings.FileIDLength.ToString(CultureInfo.InvariantCulture)
            };
            NameValueCollection headers = new NameValueCollection
            {
                ["Accept"] = "application/json"
            };

            if (!string.IsNullOrWhiteSpace(Settings.APIKey))
            {
                headers["x-api-key"] = Settings.APIKey;
            }

            UploadResult result = await SendRequestFileAsync(UploadURL, stream, fileName, "file", args, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                try
                {
                    ImgFishResponse response = JsonConvert.DeserializeObject<ImgFishResponse>(result.Response);

                    if (!string.IsNullOrEmpty(response?.error))
                    {
                        Errors.Add(response.error);
                        result.IsSuccess = false;
                    }
                    else if (string.IsNullOrEmpty(response?.link))
                    {
                        Errors.Add(Localization.Strings.Common_Unknown_error);
                        result.IsSuccess = false;
                    }
                    else
                    {
                        result.URL = response.link;
                        result.DeletionURL = response.destroy;
                    }
                }
                catch (JsonException)
                {
                    Errors.Add(Localization.Strings.Common_Unknown_error);
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        private sealed class ImgFishResponse
        {
            public string link { get; set; }
            public string destroy { get; set; }
            public string error { get; set; }
        }
    }
}
