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

// Credits: https://github.com/osfancy

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class UpleaUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Uplea;

        public override Icon ServiceIcon => Resources.Uplea;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.UpleaApiKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Uplea() { UpleaApiKey = config.UpleaApiKey };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpUplea;
    }

    public sealed class Uplea : FileUploader
    {
        private const string upleaBaseUrl = "http://api.uplea.com/api/";

        public string UpleaApiKey { get; set; }

        public UpleaGetUserInformationResponse GetUserInformation(string apiKey)
        {
            UpleaGetUserInformationResponse upleaGetUserInformationResponse = JsonConvert.DeserializeObject<UpleaGetUserInformationResponse>(
                GetUpleaResponse<UpleaGetUserInformationRequest>(new UpleaGetUserInformationRequest() { ApiKey = apiKey }));

            if (upleaGetUserInformationResponse.Status)
            {
                return upleaGetUserInformationResponse;
            }

            return new UpleaGetUserInformationResponse();
        }

        private UpleaNode GetBestNode()
        {
            string response = SendRequest(HttpMethod.POST, upleaBaseUrl + "get-best-node");
            UpleaGetBestNodeResponse getBestNodeResponse = JsonConvert.DeserializeObject<UpleaGetBestNodeResponse>(response);
            return new UpleaNode(getBestNodeResponse.Result.Name, getBestNodeResponse.Result.Token);
        }

        public string GetApiKey(string username, string password)
        {
            string upleaGetApiKeyResponseStr = GetUpleaResponse(new UpleaGetApiKeyRequest() { Username = username, Password = password });

            if (!string.IsNullOrEmpty(upleaGetApiKeyResponseStr))
            {
                try
                {
                    UpleaGetApiKeyResponse upleaGetApiKeyResponse = JsonConvert.DeserializeObject<UpleaGetApiKeyResponse>(upleaGetApiKeyResponseStr);

                    if (upleaGetApiKeyResponse.Status)
                    {
                        return upleaGetApiKeyResponse.Result.ApiKey;
                    }
                }
                catch (JsonSerializationException ex)
                {
                    // For some reason the Uplea API is supposed to return a single object in the result property of the response, but when
                    // there is an error it returns an empty array. This is causing deserialziation to fail. Do we want to just query the JSON
                    // status before trying to deserialize?

                    DebugHelper.WriteLine("Deserialization of UpleaGetApiKeyResponse failed: {0}", ex.Message);
                }
            }

            return "";
        }

        private string GetUpleaResponse<T>(T upleaRequest) where T : IUpleaRequest
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("json", JsonConvert.SerializeObject(upleaRequest));
            return SendRequestURLEncoded(HttpMethod.POST, upleaBaseUrl + upleaRequest.RequestType, parameters);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UpleaNode upleaBestNode = GetBestNode();

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("api_key", UpleaApiKey);
            args.Add("token", upleaBestNode.Token);
            args.Add("file_id[]", Guid.NewGuid().ToString());

            UploadResult result = UploadData(string.Format("http://{0}/", upleaBestNode.Name), stream, fileName, "files[]", args);
            UpleaGetUpleaUploadResponse uploadResult = JsonConvert.DeserializeObject<UpleaGetUpleaUploadResponse>(result.Response);

            if (uploadResult.Files.Length > 0)
            {
                result.URL = uploadResult.Files[0].Url;
            }

            return result;
        }
    }

    internal sealed class UpleaNode
    {
        public UpleaNode(string name, string token)
        {
            Name = name;
            Token = token;
        }

        public string Name { get; private set; }
        public string Token { get; private set; }
    }

    #region Uplea Responses

    internal sealed class UpleaGetUpleaUploadResponse
    {
        public class UpleaUploadResult
        {
            public string Url { get; set; }
        }

        public UpleaUploadResult[] Files { get; set; }
    }

    internal sealed class UpleaGetBestNodeResponse
    {
        [JsonObject]
        public class UpleaGetBestNodeResult
        {
            public string Name { get; set; }
            public string Token { get; set; }
        }

        public UpleaGetBestNodeResult Result { get; set; }
        public bool Status { get; set; }
    }

    internal sealed class UpleaGetApiKeyResponse
    {
        public class UpleaGetApiKeyResult
        {
            [JsonProperty(PropertyName = "api_key")]
            public string ApiKey { get; set; }
        }

        public UpleaGetApiKeyResult Result { get; set; }
        public bool Status { get; set; }
    }

    public sealed class UpleaGetUserInformationResponse
    {
        public class UpleaUserInformationResult
        {
            [JsonProperty(PropertyName = "mail")]
            public string EmailAddress { get; set; }
            [JsonProperty(PropertyName = "instant_download")]
            public bool InstantDownloadEnabled { get; set; }
            [JsonProperty(PropertyName = "is_premium")]
            public bool IsPremiumMember { get; set; }
        }

        public UpleaUserInformationResult Result { get; set; }
        public bool Status { get; set; }
    }

    #endregion Uplea Responses

    #region Uplea Requests

    internal sealed class UpleaGetApiKeyRequest : IUpleaRequest
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonIgnore]
        public string RequestType { get; } = "get-my-api-key";
    }

    internal sealed class UpleaGetUserInformationRequest : IUpleaRequest
    {
        [JsonProperty(PropertyName = "api_key")]
        public string ApiKey { get; set; }

        [JsonIgnore]
        public string RequestType { get; } = "get-user-info";
    }

    #endregion Uplea Requests

    #region Uplea Request Interface

    internal interface IUpleaRequest
    {
        string RequestType { get; }
    }

    #endregion Uplea Request Interface
}