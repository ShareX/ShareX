#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using ShareX.UploadersLib.HelperClasses;
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

    public sealed class Imgur_v3 : ImageUploader, IOAuth2
    {
        public AccountType UploadMethod { get; set; }
        public OAuth2Info AuthInfo { get; set; }
        public ImgurThumbnailType ThumbnailType { get; set; }
        public string UploadAlbumID { get; set; }
        public bool DirectLink { get; set; }

        public Imgur_v3(OAuth2Info oauth)
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

                if (!string.IsNullOrEmpty(response))
                {
                    ImgurAlbums albums = JsonConvert.DeserializeObject<ImgurAlbums>(response);

                    if (albums != null)
                    {
                        if (albums.success)
                        {
                            return albums.data;
                        }

                        Errors.Add("Imgur albums failed. Status code: " + albums.status);
                    }
                }
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
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

            UploadResult result = UploadData(stream, "https://api.imgur.com/3/image", fileName, "image", args, headers);

            if (!string.IsNullOrEmpty(result.Response))
            {
                JToken jsonResponse = JToken.Parse(result.Response);

                if (jsonResponse != null)
                {
                    bool success = jsonResponse["success"].Value<bool>();
                    int status = jsonResponse["status"].Value<int>();

                    if (success && status == 200)
                    {
                        ImgurUploadData uploadData = jsonResponse["data"].ToObject<ImgurUploadData>();

                        if (uploadData != null && !string.IsNullOrEmpty(uploadData.link))
                        {
                            if (DirectLink)
                            {
                                result.URL = uploadData.link;
                            }
                            else
                            {
                                result.URL = "http://imgur.com/" + uploadData.id;
                            }

                            int index = result.URL.LastIndexOf('.');
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

                            result.ThumbnailURL = string.Format("http://i.imgur.com/{0}{1}.jpg", uploadData.id, thumbnail); // Thumbnails always jpg
                            result.DeletionURL = "http://imgur.com/delete/" + uploadData.deletehash;
                        }
                    }
                    else
                    {
                        ImgurErrorData errorData = jsonResponse["data"].ToObject<ImgurErrorData>();

                        if (errorData != null && !string.IsNullOrEmpty(errorData.error))
                        {
                            string errorMessage = string.Format("Status: {0}, Error: {1}", status, errorData.error);
                            Errors.Add(errorMessage);
                        }
                    }
                }
            }

            return result;
        }
    }

    public class ImgurUploadData
    {
        public string id { get; set; }
        public string deletehash { get; set; }
        public string link { get; set; }
    }

    public class ImgurAlbumData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        /*public int datetime { get; set; }
        public object cover { get; set; }
        public string account_url { get; set; }
        public string privacy { get; set; }
        public string layout { get; set; }
        public int views { get; set; }
        public string link { get; set; }
        public bool favorite { get; set; }
        public int order { get; set; }*/
    }

    public class ImgurErrorData
    {
        public string error { get; set; }
        public string request { get; set; }
        public string method { get; set; }
    }

    public class ImgurAlbums
    {
        public List<ImgurAlbumData> data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }
}