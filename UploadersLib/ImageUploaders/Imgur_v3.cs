#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using UploadersLib.HelperClasses;

namespace UploadersLib.ImageUploaders
{
    public sealed class Imgur_v3 : ImageUploader, IOAuth2
    {
        public AccountType UploadMethod { get; set; }
        public OAuth2Info AuthInfo { get; set; }
        public ImgurThumbnailType ThumbnailType { get; set; }
        public string UploadAlbumID { get; set; }

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
                NameValueCollection headers = new NameValueCollection();
                headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
                string response = SendRequest(HttpMethod.GET, "https://api.imgur.com/3/account/me/albums", null, ResponseType.Text, headers, null);

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
            NameValueCollection headers = new NameValueCollection();

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

                headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            }
            else if (UploadMethod == AccountType.Anonymous)
            {
                headers.Add("Authorization", "Client-ID " + AuthInfo.Client_ID);
            }

            UploadResult result = UploadData(stream, "https://api.imgur.com/3/image", fileName, "image", args, null, headers);

            if (!string.IsNullOrEmpty(result.Response))
            {
                ImgurUpload upload = JsonConvert.DeserializeObject<ImgurUpload>(result.Response);

                if (upload != null)
                {
                    if (upload.success)
                    {
                        if (upload.data != null && !string.IsNullOrEmpty(upload.data.link))
                        {
                            result.URL = upload.data.link;

                            int index = result.URL.LastIndexOf('.');
                            string thumbnail;

                            if (ThumbnailType == ImgurThumbnailType.Large_Thumbnail)
                            {
                                thumbnail = "l";
                            }
                            else
                            {
                                thumbnail = "s";
                            }

                            result.ThumbnailURL = result.URL.Remove(index) + thumbnail + result.URL.Substring(index);
                            result.DeletionURL = "http://imgur.com/delete/" + upload.data.deletehash;
                        }
                    }
                    else
                    {
                        Errors.Add("Imgur upload failed. Status code: " + upload.status);
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

    public class ImgurUpload
    {
        public ImgurUploadData data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    public class ImgurAlbumData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int datetime { get; set; }
        public object cover { get; set; }
        public string account_url { get; set; }
        public string privacy { get; set; }
        public string layout { get; set; }
        public int views { get; set; }
        public string link { get; set; }
        public bool favorite { get; set; }
        public int order { get; set; }
    }

    public class ImgurAlbums
    {
        public List<ImgurAlbumData> data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }
}