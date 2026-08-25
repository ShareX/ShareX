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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

namespace ShareX.UploadersLib.FileUploaders
{
    public class ImmichFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Immich;

        public override bool CheckConfig(UploadersConfig config)
        {
            return Uri.TryCreate(config.ImmichURL, UriKind.Absolute, out Uri uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                !string.IsNullOrWhiteSpace(config.ImmichAPIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Immich(config.ImmichURL, config.ImmichAPIKey);
        }
    }

    public sealed class Immich : FileUploader
    {
        public string URL { get; }
        public string APIKey { get; }

        public Immich(string url, string apiKey)
        {
            URL = url;
            APIKey = apiKey;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            string timestamp = DateTimeOffset.UtcNow.ToString("O");
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                ["deviceAssetId"] = $"ShareX-{Guid.NewGuid():N}",
                ["deviceId"] = "ShareX",
                ["fileCreatedAt"] = timestamp,
                ["fileModifiedAt"] = timestamp,
                ["isFavorite"] = "false",
                ["filename"] = fileName
            };
            NameValueCollection headers = new NameValueCollection
            {
                ["Accept"] = "application/json",
                ["x-api-key"] = APIKey
            };

            UploadResult result = await SendRequestFileAsync(GetUploadURL(URL), stream, fileName, "assetData", arguments, headers,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                ImmichUploadResponse response = JsonConvert.DeserializeObject<ImmichUploadResponse>(result.Response);
                if (!string.IsNullOrWhiteSpace(response?.Id))
                {
                    result.URL = URLHelpers.CombineURL(GetWebURL(URL), "photos", response.Id);
                }
                else
                {
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        internal static string GetUploadURL(string url)
        {
            string normalizedURL = url.TrimEnd('/');
            if (normalizedURL.EndsWith("/api/assets", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedURL;
            }
            if (normalizedURL.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                return URLHelpers.CombineURL(normalizedURL, "assets");
            }
            return URLHelpers.CombineURL(normalizedURL, "api", "assets");
        }

        internal static string GetWebURL(string url)
        {
            string normalizedURL = url.TrimEnd('/');
            if (normalizedURL.EndsWith("/api/assets", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedURL[..^11];
            }
            if (normalizedURL.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedURL[..^4];
            }
            return normalizedURL;
        }
    }

    public sealed class ImmichUploadResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public bool Duplicate { get; set; }
    }
}
