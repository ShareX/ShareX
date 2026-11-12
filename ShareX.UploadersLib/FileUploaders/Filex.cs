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
using System;
using System.Collections.Specialized;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class FilexFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Filex;

        public override bool CheckConfig(UploadersConfig config)
        {
            return Uri.TryCreate(config.FilexURL, UriKind.Absolute, out Uri uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                !string.IsNullOrWhiteSpace(config.FilexAPIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Filex(config.FilexURL, config.FilexAPIKey);
        }
    }

    public sealed class Filex : FileUploader
    {
        public string URL { get; }
        public string APIKey { get; }

        public Filex(string url, string apiKey)
        {
            URL = url;
            APIKey = apiKey;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            NameValueCollection headers = new NameValueCollection
            {
                ["Accept"] = "application/json",
                ["Authorization"] = "Bearer " + APIKey
            };

            UploadResult result = await SendRequestFileAsync(GetUploadURL(URL), stream, fileName, "file", headers: headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                try
                {
                    FilexUploadResponse response = JsonConvert.DeserializeObject<FilexUploadResponse>(result.Response);

                    if (!string.IsNullOrEmpty(response?.URL))
                    {
                        result.URL = response.URL;
                        result.ThumbnailURL = response.ThumbnailURL;
                        result.DeletionURL = response.DeletionURL;
                    }
                    else
                    {
                        Errors.Add(string.IsNullOrEmpty(response?.Error) ? Localization.Strings.Common_Unknown_error : response.Error);
                        result.IsSuccess = false;
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

        internal static string GetUploadURL(string url)
        {
            string normalizedURL = url.TrimEnd('/');
            if (normalizedURL.EndsWith("/api/uploads", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedURL;
            }
            return URLHelpers.CombineURL(normalizedURL, "api", "uploads");
        }
    }

    public sealed class FilexUploadResponse
    {
        [JsonProperty("url")]
        public string URL { get; set; }
        [JsonProperty("thumbnail_url")]
        public string ThumbnailURL { get; set; }
        [JsonProperty("deletion_url")]
        public string DeletionURL { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
