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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace ShareX.UploadersLib.FileUploaders
{
    public class UpleaUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Uplea;

        //public override Icon ServiceIcon = Resources.Mega;
        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.UpleaApiKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Uplea(config);
        }
    }

    [Localizable(false)]
    public sealed class Uplea : FileUploader
    {
        private const string upleaBaseUrl = "http://api.uplea.com/api/";
        private readonly UploadersConfig upleaConfig;

        public Uplea(UploadersConfig config)
        {
            upleaConfig = config;
        }

        public UpleaUserInformation GetUserInformation(string apiKey)
        {
            UpleaUserInformation upleaUserInformation = new UpleaUserInformation();

            var upleaGetUserInformationResponse = GetUpleaResponse<UpleaGetUserInformationRequest>(new UpleaGetUserInformationRequest() { ApiKey = apiKey });           

            if (!string.IsNullOrEmpty(upleaGetUserInformationResponse))
            {
                JObject upleaGetUserInformationResponseObj = JObject.Parse(upleaGetUserInformationResponse);
                upleaUserInformation.EmailAddress = (string)upleaGetUserInformationResponseObj.SelectToken("result.mail");
                upleaUserInformation.IsPremiumMember = (bool)upleaGetUserInformationResponseObj.SelectToken("result.is_premium");
                upleaUserInformation.InstantDownloadEnabled = (bool)upleaGetUserInformationResponseObj.SelectToken("result.instant_download");
            }

            return upleaUserInformation;
        }

        private UpleaNode GetBestNode()
        {
            JObject getBestNodeResponse = JObject.Parse(SendRequest(HttpMethod.POST, upleaBaseUrl + "get-best-node"));
            return new UpleaNode((string)getBestNodeResponse.SelectToken("result.token"), (string)getBestNodeResponse.SelectToken("result.name"));
        }

        public string GetApiKey(string username, string password)
        {
            var upleaGetApiKeyResponse = GetUpleaResponse<UpleaGetApiKeyRequest>(new UpleaGetApiKeyRequest() { Username = username, Password = password });

            if (!string.IsNullOrEmpty(upleaGetApiKeyResponse))
            {
                JObject upleaGetApiKeyResponseObj = JObject.Parse(upleaGetApiKeyResponse);

                if ((string)upleaGetApiKeyResponseObj.SelectToken("status") == bool.TrueString)
                {
                    return (string)upleaGetApiKeyResponseObj.SelectToken("result.api_key");
                }
            }

            return string.Empty;
        }

        private string GetUpleaResponse<T>(T upleaRequest) where T : IUpleaRequest
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(T));

                ser.WriteObject(ms, upleaRequest);

                if (ms.CanRead)
                {
                    ms.Position = 0;

                    using (StreamReader sr = new StreamReader(ms))
                    {
                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("json", sr.ReadToEnd());

                        return SendRequestURLEncoded(upleaBaseUrl + upleaRequest.RequestType, parameters);
                    }
                }
            }

            return string.Empty;
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

        public override UploadResult Upload(Stream stream, string fileName)
        {
            throw new NotImplementedException();
        }

        public sealed class UpleaUserInformation
        {
            public string EmailAddress { get; set; }
            public bool InstantDownloadEnabled { get; set; }
            public bool IsPremiumMember { get; set; }
        }

        [DataContract]
        private sealed class UpleaGetApiKeyRequest : IUpleaRequest
        {
            [DataMember(Name = "username", Order = 0)]
            public string Username { get; set; }
            [DataMember(Name = "password", Order = 1)]
            public string Password { get; set; }

            public string RequestType { get; } = "get-my-api-key";
        }

        [DataContract]
        private sealed class UpleaGetUserInformationRequest : IUpleaRequest
        {
            [DataMember(Name = "api_key")]
            public string ApiKey { get; set; }
            public string RequestType { get; } = "get-user-info";
        }

        private interface IUpleaRequest
        {
            string RequestType { get; }
        }
    }
}
