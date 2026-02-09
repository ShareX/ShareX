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

using FluentFTP.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.Design.AxImporter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class ImmichUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Immich;

        //public override Icon ServiceIcon => Resources.Immich;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new ImmichUploader()
            {
                APIKey = config.ImmichAPIKey,
                UploadURL = config.ImmichUploadURL,
                DeviceId = config.ImmichDeviceId
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpImmich;
    }

    public sealed class ImmichUploader : ImageUploader
    {
        public string APIKey { get; set; }
        public string UploadURL { get; set; }
        public string DeviceId { get; set; }

        public enum SharedLinkType
        {
            ALBUM,
            INDIVIDUAL
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadURL = UploadURL.TrimEnd('/');

            using HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(UploadURL + "/api/")
            };

            // https://api.immich.app/authentication
            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);

            UploadResult result = new UploadResult();

            // Upload the data
            // https://api.immich.app/endpoints/assets/uploadAsset
            var streamContent = new StreamContent(stream);
            using var uploadContent = new MultipartFormDataContent();

            uploadContent.Add(streamContent, "assetData", fileName);
            uploadContent.Add(new StringContent(fileName), "filename");
            uploadContent.Add(new StringContent(DeviceId), "deviceAssetId");
            uploadContent.Add(new StringContent(DeviceId), "deviceId");
            uploadContent.Add(new StringContent(DateTime.Now.ToString("O")), "fileCreatedAt");
            uploadContent.Add(new StringContent(DateTime.Now.ToString("O")), "fileModifiedAt");

            using HttpRequestMessage uploadRequest = new(System.Net.Http.HttpMethod.Post, UploadURL + "/api/" + "assets")
            {
                Content = uploadContent
            };
            using HttpResponseMessage uploadResult = httpClient.Send(uploadRequest);


            if (uploadResult != null)
            {
                string uploadJsonResponse = uploadResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UploadResponse uploadResponse = JsonConvert.DeserializeObject<UploadResponse>(uploadJsonResponse); // UploadResponse contains asset id

                // Immich does not automatically share the link after uploading
                // https://api.immich.app/endpoints/shared-links/createSharedLink
                var sharePayload = new
                {
                    type = SharedLinkType.INDIVIDUAL,
                    assetIds = new[] { uploadResponse.id }
                };
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };
                string shareJsonString = JsonSerializer.Serialize(sharePayload, options);
                using var shareContent = new StringContent(shareJsonString, System.Text.Encoding.UTF8, "application/json");

                using HttpRequestMessage shareRequest = new(System.Net.Http.HttpMethod.Post, UploadURL + "/api/" + "shared-links")
                {
                    Content = shareContent
                };
                using HttpResponseMessage shareResult = httpClient.Send(shareRequest);
                string shareJsonResponse = shareResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                SharedLinkResponse shareResponse = JsonConvert.DeserializeObject<SharedLinkResponse>(shareJsonResponse);

                // Get the external domain link
                using HttpRequestMessage URLRequest = new(System.Net.Http.HttpMethod.Get, UploadURL + "/api/" + "server/config");
                using HttpResponseMessage URLResult = httpClient.Send(URLRequest);
                string URLJsonResponse = URLResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                ServerConfig URLResponse = JsonConvert.DeserializeObject<ServerConfig>(URLJsonResponse);

                // Convert the link into a download
                string link = URLResponse.externalDomain + "/share/photo/" + shareResponse.key + "/" + uploadResponse.id + "/original";
                result.URL = link;
                result.IsSuccess = true;
            }

            return result;
        }

        public class UploadResponse
        {
            public string id { get; set; }

        }

        public class SharedLinkResponse
        {
            public string key { get; set; }
            public string slug { get; set; }
        }

        public class ServerConfig
        {
            public string externalDomain { get; set; }

        }

    }

    
}
