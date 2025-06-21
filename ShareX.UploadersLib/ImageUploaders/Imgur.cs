#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.ImageUploaders
{
    public enum ImgurThumbnailType // Localized
    {
        Small_Square,
        Big_Square,
        Small_Thumbnail,
        Medium_Thumbnail,
        Large_Thumbnail,
        Huge_Thumbnail
    }

    public class ImgurImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Imgur;

        public override Icon ServiceIcon => Resources.Imgur;

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

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpImgur;
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

            return URLHelpers.CreateQueryString("https://api.imgur.com/oauth2/authorize", args);
        }

        public bool GetAccessToken(string pin)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "pin");
            args.Add("pin", pin);

            string response = SendRequestMultiPart("https://api.imgur.com/oauth2/token", args);

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

                string response = SendRequestMultiPart("https://api.imgur.com/oauth2/token", args);

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

        public List<ImgurAlbumData> GetAlbums(int maxPage = 10, int perPage = 100)
        {
            List<ImgurAlbumData> albums = new List<ImgurAlbumData>();

            for (int i = 0; i < maxPage; i++)
            {
                List<ImgurAlbumData> tempAlbums = GetAlbumsPage(i, perPage);

                if (tempAlbums != null && tempAlbums.Count > 0)
                {
                    albums.AddRange(tempAlbums);

                    if (tempAlbums.Count < perPage)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return albums;
        }

        private List<ImgurAlbumData> GetAlbumsPage(int page, int perPage)
        {
            if (CheckAuthorization())
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("page", page.ToString()); // default: 0
                args.Add("perPage", perPage.ToString()); // default: 50, max: 100

                string response = SendRequest(HttpMethod.GET, "https://api.imgur.com/3/account/me/albums", args, GetAuthHeaders());

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

        public List<ImgurImageData> GetAlbumImages(string albumID)
        {
            if (CheckAuthorization())
            {
                string response = SendRequest(HttpMethod.GET, $"https://api.imgur.com/3/album/{albumID}/images", headers: GetAuthHeaders());

                ImgurResponse imgurResponse = JsonConvert.DeserializeObject<ImgurResponse>(response);

                if (imgurResponse != null)
                {
                    if (imgurResponse.success && imgurResponse.status == 200)
                    {
                        return ((JArray)imgurResponse.data).ToObject<List<ImgurImageData>>();
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

            ReturnResponseOnError = true;

            string fileFormName;

            if (FileHelpers.IsVideoFile(fileName))
            {
                fileFormName = "video";
            }
            else
            {
                fileFormName = "image";
            }

            UploadResult result = SendRequestFile("https://api.imgur.com/3/upload", stream, fileName, fileFormName, args, headers);

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
                                    // webm uploads returns link with dot at the end
                                    result.URL = imageData.link.TrimEnd('.');
                                }
                            }
                            else
                            {
                                result.URL = $"https://imgur.com/{imageData.id}";
                            }

                            string thumbnail = "";

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

                            result.ThumbnailURL = $"https://i.imgur.com/{imageData.id}{thumbnail}.jpg"; // Imgur thumbnails always jpg
                            result.DeletionURL = $"https://imgur.com/delete/{imageData.deletehash}";
                        }
                    }
                    else
                    {
                        ImgurErrorData errorData = ParseError(imgurResponse);

                        if (errorData != null)
                        {
                            if (UploadMethod == AccountType.User && refreshTokenOnError &&
                                ((string)errorData.error).Equals("The access token provided is invalid.", StringComparison.OrdinalIgnoreCase) &&
                                RefreshAccessToken())
                            {
                                DebugHelper.WriteLine("Imgur access token refreshed, reuploading image.");

                                return InternalUpload(stream, fileName, false);
                            }

                            Errors.AddFirst($"Imgur upload failed: ({imgurResponse.status}) {errorData.error}");
                        }
                    }
                }
            }

            return result;
        }

        private void HandleErrors(ImgurResponse response)
        {
            ImgurErrorData errorData = ParseError(response);

            if (errorData != null)
            {
                Errors.Add($"Status: {response.status}, Request: {errorData.request}, Error: {errorData.error}");
            }
        }

        private ImgurErrorData ParseError(ImgurResponse response)
        {
            ImgurErrorData errorData = ((JObject)response.data).ToObject<ImgurErrorData>();

            if (errorData != null && !(errorData.error is string))
            {
                errorData.error = ((JObject)errorData.error).ToObject<ImgurError>().message;
            }

            return errorData;
        }
    }

    internal class ImgurResponse
    {
        public object data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    internal class ImgurErrorData
    {
        public object error { get; set; }
        public string request { get; set; }
        public string method { get; set; }
    }

    internal class ImgurError
    {
        public int code { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        //public string[] exception { get; set; }
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
        public long bandwidth { get; set; }
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