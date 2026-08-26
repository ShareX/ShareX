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
            return new Immich(config.ImmichURL, config.ImmichAPIKey)
            {
                AutoCreateShareableLink = config.ImmichAutoCreateShareableLink,
                ShowMetadata = config.ImmichShowMetadata
            };
        }
    }

    public sealed class Immich : FileUploader
    {
        public string URL { get; }
        public string APIKey { get; }
        public bool AutoCreateShareableLink { get; set; }
        public bool ShowMetadata { get; set; }

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
                    if (AutoCreateShareableLink)
                    {
                        result.URL = await CreateShareableLinkAsync(response.Id, headers, cancellationToken).ConfigureAwait(false);
                        result.IsSuccess = !string.IsNullOrWhiteSpace(result.URL);
                    }
                    else
                    {
                        result.URL = URLHelpers.CombineURL(GetWebURL(URL), "photos", response.Id);
                    }
                }
                else
                {
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        private async Task<string> CreateShareableLinkAsync(string assetId, NameValueCollection headers, CancellationToken cancellationToken)
        {
            string requestBody = JsonConvert.SerializeObject(new
            {
                type = "INDIVIDUAL",
                assetIds = new[] { assetId },
                allowUpload = false,
                allowDownload = true,
                showMetadata = ShowMetadata
            });
            string responseText = await SendRequestAsync(HttpMethod.POST, GetSharedLinksURL(URL), requestBody,
                RequestHelpers.ContentTypeJSON, headers: headers, cancellationToken: cancellationToken).ConfigureAwait(false);
            ImmichSharedLinkResponse response = JsonConvert.DeserializeObject<ImmichSharedLinkResponse>(responseText);

            return string.IsNullOrWhiteSpace(response?.Key)
                ? null
                : URLHelpers.CombineURL(GetWebURL(URL), "share", response.Key);
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

        internal static string GetSharedLinksURL(string url)
        {
            return URLHelpers.CombineURL(GetWebURL(url), "api", "shared-links");
        }
    }

    public sealed class ImmichUploadResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public bool Duplicate { get; set; }
    }

    public sealed class ImmichSharedLinkResponse
    {
        public string Key { get; set; }
    }
}
