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

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using ShareX.UploadersLib.Properties;
using Newtonsoft.Json;

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

    [Localizable(false)]
    public sealed class Uplea : FileUploader
    {
        private const string upleaBaseUrl = "http://api.uplea.com/api/";
        public string UpleaApiKey { get; set; }

        public UpleaGetUserInformationResponse GetUserInformation(string apiKey)
        {
            var upleaGetUserInformationResponse = JsonConvert.DeserializeObject<UpleaGetUserInformationResponse>(GetUpleaResponse<UpleaGetUserInformationRequest>(new UpleaGetUserInformationRequest() { ApiKey = apiKey }));

            if (upleaGetUserInformationResponse.Status)
            {
                return upleaGetUserInformationResponse;
            }

            return new UpleaGetUserInformationResponse();
        }

        private UpleaNode GetBestNode()
        {
            var getBestNodeResponse = JsonConvert.DeserializeObject<UpleaGetBestNodeResponse>(SendRequest(HttpMethod.POST, upleaBaseUrl + "get-best-node"));
            return new UpleaNode(getBestNodeResponse.Result.Name, getBestNodeResponse.Result.Token);
        }

        public string GetApiKey(string username, string password)
        {
            var upleaGetApiKeyResponseStr = GetUpleaResponse(new UpleaGetApiKeyRequest() { Username = username, Password = password });

            if (!string.IsNullOrEmpty(upleaGetApiKeyResponseStr))
            {
                try
                {
                    var upleaGetApiKeyResponse = JsonConvert.DeserializeObject<UpleaGetApiKeyResponse>(upleaGetApiKeyResponseStr);

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

                    System.Diagnostics.Debug.WriteLine("Deserialization of UpleaGetApiKeyResponse failed: {0}", ex.Message);

                }
            }

            return string.Empty;
        }

        private string GetUpleaResponse<T>(T upleaRequest) where T : IUpleaRequest
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("json", JsonConvert.SerializeObject(upleaRequest));
            return SendRequestURLEncoded(upleaBaseUrl + upleaRequest.RequestType, parameters);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            var upleaBestNode = GetBestNode();

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("api_key", UpleaApiKey);
            args.Add("token", upleaBestNode.Token);
            args.Add("file_id[]", string.Format("{0}", Guid.NewGuid()));

            UploadResult result = UploadData(stream, string.Format("http://{0}/", upleaBestNode.Name), fileName, "files[]", args, contentType: "multipart/form-data");
            var uploadResult = JsonConvert.DeserializeObject<UpleaGetUpleaUploadResponse>(result.Response);

            result.IsURLExpected = true;
            if (uploadResult.Files.Length > 0)
            {
                result.URL = uploadResult.Files[0].Url;
            }

            return result;
        }
        
        private sealed class UpleaNode
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
        private sealed class UpleaGetUpleaUploadResponse
        {
            public class UpleaUploadResult
            {
                public string Url { get; set; }
            }
            
            public UpleaUploadResult[] Files { get; set; }
        }

        private sealed class UpleaGetBestNodeResponse
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

        private sealed class UpleaGetApiKeyResponse
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
        #endregion

        #region Uplea Requests
        private sealed class UpleaGetApiKeyRequest : IUpleaRequest
        {
            [JsonProperty(PropertyName = "username")]
            public string Username { get; set; }
            [JsonProperty(PropertyName = "password")]
            public string Password { get; set; }

            [JsonIgnore]
            public string RequestType { get; } = "get-my-api-key";
        }

        private sealed class UpleaGetUserInformationRequest : IUpleaRequest
        {
            [JsonProperty(PropertyName = "api_key")]
            public string ApiKey { get; set; }

            [JsonIgnore]
            public string RequestType { get; } = "get-user-info";
        }
        #endregion

        #region Uplea Request Interface
        private interface IUpleaRequest
        {
            string RequestType { get; }
        }
        #endregion
    }
}
