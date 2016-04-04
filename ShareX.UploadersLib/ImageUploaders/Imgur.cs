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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace ShareX.UploadersLib.ImageUploaders
{
    public enum ImgurThumbnailType
    {
        [Description("Small square")]
        Small_Square,
        [Description("Big square")]
        Big_Square,
        [Description("Small thumbnail")]
        Small_Thumbnail,
        [Description("Medium thumbnail")]
        Medium_Thumbnail,
        [Description("Large thumbnail")]
        Large_Thumbnail,
        [Description("Huge thumbnail")]
        Huge_Thumbnail
    }

    public class ImgurImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Imgur;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.ImgurAccountType == AccountType.Anonymous || OAuth2Info.CheckOAuth(config.ImgurOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            if (config.ImgurOAuth2Info == null)
            {
                config.ImgurOAuth2Info = new OAuth2Info(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);
            }

            string albumID = null;

            if (config.ImgurUploadSelectedAlbum && config.ImgurSelectedAlbum != null)
            {
                albumID = config.ImgurSelectedAlbum.id;
            }

            return new Imgur(config.ImgurOAuth2Info)
            {
                UploadMethod = config.ImgurAccountType,
                DirectLink = config.ImgurDirectLink,
                ThumbnailType = config.ImgurThumbnailType,
                UseGIFV = config.ImgurUseGIFV,
                UploadAlbumID = albumID
            };
        }
    }

    public sealed class Imgur : ImageUploader, IOAuth2
    {
        public AccountType UploadMethod { get; set; }
        public OAuth2Info AuthInfo { get; set; }
        public ImgurThumbnailType ThumbnailType { get; set; }
        public string UploadAlbumID { get; set; }
        public bool DirectLink { get; set; }
        public bool UseGIFV { get; set; }

        public Imgur(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("response_type", "pin");

            return CreateQuery("https://api.imgur.com/oauth2/authorize", args);
        }

        public bool GetAccessToken(string pin)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "pin");
            args.Add("pin", pin);

            string response = SendRequest(HttpMethod.POST, "https://api.imgur.com/oauth2/token", args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public bool RefreshAccessToken()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("grant_type", "refresh_token");

                string response = SendRequest(HttpMethod.POST, "https://api.imgur.com/oauth2/token", args);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                    if (token != null && !string.IsNullOrEmpty(token.access_token))
                    {
                        token.UpdateExpireDate();
                        AuthInfo.Token = token;
                        return true;
                    }
                }
            }

            return false;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        public bool CheckAuthorization()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (AuthInfo.Token.IsExpired && !RefreshAccessToken())
                {
                    Errors.Add("Refresh access token failed.");
                    return false;
                }
            }
            else
            {
                Errors.Add("Imgur login is required.");
                return false;
            }

            return true;
        }

        public List<ImgurAlbumData> GetAlbums()
        {
            if (CheckAuthorization())
            {
                string response = SendRequest(HttpMethod.GET, "https://api.imgur.com/3/account/me/albums", headers: GetAuthHeaders());

                ImgurResponse imgurResponse = JsonConvert.DeserializeObject<ImgurResponse>(response);

                if (imgurResponse != null)
                {
                    if (imgurResponse.success && imgurResponse.status == 200)
                    {
                        return ((JArray)imgurResponse.data).ToObject<List<ImgurAlbumData>>();
                    }
                    else
                    {
                        HandleErrors(imgurResponse);
                    }
                }
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            return InternalUpload(stream, fileName, true);
        }

        private UploadResult InternalUpload(Stream stream, string fileName, bool refreshTokenOnError)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            NameValueCollection headers;

            if (UploadMethod == AccountType.User)
            {
                if (!CheckAuthorization())
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(UploadAlbumID))
                {
                    args.Add("album", UploadAlbumID);
                }

                headers = GetAuthHeaders();
            }
            else
            {
                headers = new NameValueCollection();
                headers.Add("Authorization", "Client-ID " + AuthInfo.Client_ID);
            }

            WebExceptionReturnResponse = true;
            UploadResult result = UploadData(stream, "https://api.imgur.com/3/image", fileName, "image", args, headers);

            if (!string.IsNullOrEmpty(result.Response))
            {
                ImgurResponse imgurResponse = JsonConvert.DeserializeObject<ImgurResponse>(result.Response);

                if (imgurResponse != null)
                {
                    if (imgurResponse.success && imgurResponse.status == 200)
                    {
                        ImgurImageData imageData = ((JObject)imgurResponse.data).ToObject<ImgurImageData>();

                        if (imageData != null && !string.IsNullOrEmpty(imageData.link))
                        {
                            if (DirectLink)
                            {
                                if (UseGIFV && !string.IsNullOrEmpty(imageData.gifv))
                                {
                                    result.URL = imageData.gifv;
                                }
                                else
                                {
                                    result.URL = imageData.link;
                                }
                            }
                            else
                            {
                                result.URL = "http://imgur.com/" + imageData.id;
                            }

                            string thumbnail = string.Empty;

                            switch (ThumbnailType)
                            {
                                case ImgurThumbnailType.Small_Square:
                                    thumbnail = "s";
                                    break;
                                case ImgurThumbnailType.Big_Square:
                                    thumbnail = "b";
                                    break;
                                case ImgurThumbnailType.Small_Thumbnail:
                                    thumbnail = "t";
                                    break;
                                case ImgurThumbnailType.Medium_Thumbnail:
                                    thumbnail = "m";
                                    break;
                                case ImgurThumbnailType.Large_Thumbnail:
                                    thumbnail = "l";
                                    break;
                                case ImgurThumbnailType.Huge_Thumbnail:
                                    thumbnail = "h";
                                    break;
                            }

                            result.ThumbnailURL = string.Format("http://i.imgur.com/{0}{1}.jpg", imageData.id, thumbnail); // Thumbnails always jpg
                            result.DeletionURL = "http://imgur.com/delete/" + imageData.deletehash;
                        }
                    }
                    else
                    {
                        ImgurErrorData errorData = ((JObject)imgurResponse.data).ToObject<ImgurErrorData>();

                        if (errorData != null)
                        {
                            if (UploadMethod == AccountType.User && refreshTokenOnError &&
                                errorData.error.Equals("The access token provided is invalid.", StringComparison.InvariantCultureIgnoreCase) && RefreshAccessToken())
                            {
                                DebugHelper.WriteLine("Imgur access token refreshed, reuploading image.");

                                return InternalUpload(stream, fileName, false);
                            }

                            string errorMessage = string.Format("Imgur upload failed: ({0}) {1}", imgurResponse.status, errorData.error);
                            Errors.Clear();
                            Errors.Add(errorMessage);
                        }
                    }
                }
            }

            return result;
        }

        private void HandleErrors(ImgurResponse response)
        {
            ImgurErrorData errorData = ((JObject)response.data).ToObject<ImgurErrorData>();

            if (errorData != null)
            {
                string errorMessage = string.Format("Status: {0}, Request: {1}, Error: {2}", response.status, errorData.request, errorData.error);
                Errors.Add(errorMessage);
            }
        }
    }

    public class ImgurResponse
    {
        public object data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    public class ImgurErrorData
    {
        public string error { get; set; }
        public string request { get; set; }
        public string method { get; set; }
    }

    public class ImgurImageData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int datetime { get; set; }
        public string type { get; set; }
        public bool animated { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int size { get; set; }
        public int views { get; set; }
        public int bandwidth { get; set; }
        public string deletehash { get; set; }
        public string name { get; set; }
        public string section { get; set; }
        public string link { get; set; }
        public string gifv { get; set; }
        public string mp4 { get; set; }
        public string webm { get; set; }
        public bool looping { get; set; }
        public bool favorite { get; set; }
        public bool? nsfw { get; set; }
        public string vote { get; set; }
        public string comment_preview { get; set; }
    }

    public class ImgurAlbumData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int datetime { get; set; }
        public string cover { get; set; }
        public string cover_width { get; set; }
        public string cover_height { get; set; }
        public string account_url { get; set; }
        public long? account_id { get; set; }
        public string privacy { get; set; }
        public string layout { get; set; }
        public int views { get; set; }
        public string link { get; set; }
        public bool favorite { get; set; }
        public bool? nsfw { get; set; }
        public string section { get; set; }
        public int order { get; set; }
        public string deletehash { get; set; }
        public int images_count { get; set; }
        public ImgurImageData[] images { get; set; }
    }
}