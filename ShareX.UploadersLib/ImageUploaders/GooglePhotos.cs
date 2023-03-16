#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class GooglePhotosImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Picasa;

        public override Image ServiceImage => Resources.GooglePhotos;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.GooglePhotosOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GooglePhotos(config.GooglePhotosOAuth2Info)
            {
                AlbumID = config.GooglePhotosAlbumID,
                IsPublic = config.GooglePhotosIsPublic
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGooglePhotos;
    }

    public sealed class GooglePhotos : ImageUploader, IOAuth2
    {
        public GoogleOAuth2 OAuth2 { get; private set; }
        public OAuth2Info AuthInfo => OAuth2.AuthInfo;
        public string AlbumID { get; set; }
        public bool IsPublic { get; set; }

        public GooglePhotos(OAuth2Info oauth)
        {
            OAuth2 = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/photoslibrary https://www.googleapis.com/auth/photoslibrary.sharing https://www.googleapis.com/auth/userinfo.profile"
            };
        }

        public bool RefreshAccessToken()
        {
            return OAuth2.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return OAuth2.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return OAuth2.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return OAuth2.GetAccessToken(code);
        }

        public OAuthUserInfo GetUserInfo()
        {
            return OAuth2.GetUserInfo();
        }

        public GooglePhotosAlbum CreateAlbum(string albumName)
        {
            GooglePhotosNewAlbum newItemAlbum = new GooglePhotosNewAlbum
            {
                album = new GooglePhotosAlbum
                {
                    title = albumName
                }
            };

            Dictionary<string, string> newItemAlbumArgs = new Dictionary<string, string>
            {
                { "fields", "id" }
            };

            string serializedNewItemAlbum = JsonConvert.SerializeObject(newItemAlbum);
            string serializedNewItemAlbumResponse = SendRequest(HttpMethod.POST, "https://photoslibrary.googleapis.com/v1/albums", serializedNewItemAlbum, RequestHelpers.ContentTypeJSON, newItemAlbumArgs, OAuth2.GetAuthHeaders());
            GooglePhotosAlbum newItemAlbumResponse = JsonConvert.DeserializeObject<GooglePhotosAlbum>(serializedNewItemAlbumResponse);

            return newItemAlbumResponse;
        }

        public List<GooglePhotosAlbumInfo> GetAlbumList()
        {
            if (!CheckAuthorization()) return null;

            List<GooglePhotosAlbumInfo> albumList = new List<GooglePhotosAlbumInfo>();

            Dictionary<string, string> albumListArgs = new Dictionary<string, string>
            {
                { "excludeNonAppCreatedData", "true" },
                { "fields", "albums(id,title,shareInfo),nextPageToken" }
            };

            string pageToken = "";

            do
            {
                albumListArgs["pageToken"] = pageToken;
                string response = SendRequest(HttpMethod.GET, "https://photoslibrary.googleapis.com/v1/albums", albumListArgs, OAuth2.GetAuthHeaders());
                pageToken = "";

                if (!string.IsNullOrEmpty(response))
                {
                    GooglePhotosAlbums albumsResponse = JsonConvert.DeserializeObject<GooglePhotosAlbums>(response);

                    if (albumsResponse.albums != null)
                    {
                        foreach (GooglePhotosAlbum album in albumsResponse.albums)
                        {
                            GooglePhotosAlbumInfo AlbumInfo = new GooglePhotosAlbumInfo
                            {
                                ID = album.id,
                                Name = album.title
                            };

                            if (album.shareInfo == null)
                            {
                                albumList.Add(AlbumInfo);
                            }
                        }
                    }
                    pageToken = albumsResponse.nextPageToken;
                }
            }
            while (!string.IsNullOrEmpty(pageToken));

            return albumList;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            UploadResult result = new UploadResult();

            if (IsPublic)
            {
                AlbumID = CreateAlbum(fileName).id;

                Dictionary<string, string> albumOptionsResponseArgs = new Dictionary<string, string>
                {
                    { "fields", "shareInfo/shareableUrl" }
                };

                GooglePhotosAlbumOptions albumOptions = new GooglePhotosAlbumOptions();

                string serializedAlbumOptions = JsonConvert.SerializeObject(albumOptions);
                string serializedAlbumOptionsResponse = SendRequest(HttpMethod.POST, $"https://photoslibrary.googleapis.com/v1/albums/{AlbumID}:share", serializedAlbumOptions, RequestHelpers.ContentTypeJSON, albumOptionsResponseArgs, OAuth2.GetAuthHeaders());
                GooglePhotosAlbumOptionsResponse albumOptionsResponse = JsonConvert.DeserializeObject<GooglePhotosAlbumOptionsResponse>(serializedAlbumOptionsResponse);

                result.URL = albumOptionsResponse.shareInfo.shareableUrl;
            }

            NameValueCollection uploadTokenHeaders = new NameValueCollection
            {
                { "X-Goog-Upload-File-Name", fileName },
                { "X-Goog-Upload-Protocol", "raw" },
                { "Authorization", OAuth2.GetAuthHeaders()["Authorization"] }
            };

            string uploadToken = SendRequest(HttpMethod.POST, "https://photoslibrary.googleapis.com/v1/uploads", stream, RequestHelpers.ContentTypeOctetStream, null, uploadTokenHeaders);

            GooglePhotosNewMediaItemRequest newMediaItemRequest = new GooglePhotosNewMediaItemRequest
            {
                albumId = AlbumID,
                newMediaItems = new GooglePhotosNewMediaItem[]
                {
                    new  GooglePhotosNewMediaItem
                    {
                        simpleMediaItem = new GooglePhotosSimpleMediaItem
                        {
                            uploadToken = uploadToken
                        }
                    }
                }
            };

            Dictionary<string, string> newMediaItemRequestArgs = new Dictionary<string, string>
            {
                { "fields", "newMediaItemResults(mediaItem/productUrl)" }
            };

            string serializedNewMediaItemRequest = JsonConvert.SerializeObject(newMediaItemRequest);

            result.Response = SendRequest(HttpMethod.POST, "https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate", serializedNewMediaItemRequest, RequestHelpers.ContentTypeJSON, newMediaItemRequestArgs, OAuth2.GetAuthHeaders());

            GooglePhotosNewMediaItemResults newMediaItemResult = JsonConvert.DeserializeObject<GooglePhotosNewMediaItemResults>(result.Response);

            if (!IsPublic)
            {
                result.URL = newMediaItemResult.newMediaItemResults[0].mediaItem.productUrl;
            }

            return result;
        }
    }

    public class GooglePhotosAlbumInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
    }

    public class GooglePhotosAlbums
    {
        public GooglePhotosAlbum[] albums { get; set; }
        public string nextPageToken { get; set; }
    }

    public class GooglePhotosAlbum
    {
        public string id { get; set; }
        public string title { get; set; }
        public string productUrl { get; set; }
        public string coverPhotoBaseUrl { get; set; }
        public string coverPhotoMediaItemId { get; set; }
        public string isWriteable { get; set; }
        public GooglePhotosShareInfo shareInfo { get; set; }
        public string mediaItemsCount { get; set; }
    }

    public class GooglePhotosNewMediaItemRequest
    {
        public string albumId { get; set; }
        public GooglePhotosNewMediaItem[] newMediaItems { get; set; }
    }

    public class GooglePhotosNewMediaItem
    {
        public string description { get; set; }
        public GooglePhotosSimpleMediaItem simpleMediaItem { get; set; }
    }

    public class GooglePhotosSimpleMediaItem
    {
        public string uploadToken { get; set; }
    }

    public class GooglePhotosNewMediaItemResults
    {
        public GooglePhotosNewMediaItemResult[] newMediaItemResults { get; set; }
    }

    public class GooglePhotosNewMediaItemResult
    {
        public string uploadToken { get; set; }
        public GooglePhotosStatus status { get; set; }
        public GooglePhotosMediaItem mediaItem { get; set; }
    }

    public class GooglePhotosStatus
    {
        public string message { get; set; }
        public int code { get; set; }
    }

    public class GooglePhotosMediaItem
    {
        public string id { get; set; }
        public string productUrl { get; set; }
        public string description { get; set; }
        public string baseUrl { get; set; }
        public GooglePhotosMediaMetaData mediaMetadata { get; set; }
    }

    public class GooglePhotosMediaMetaData
    {
        public string width { get; set; }
        public string height { get; set; }
        public string creationTime { get; set; }
        public GooglePhotosPhoto photo { get; set; }
    }

    public class GooglePhotosPhoto
    {
    }

    public class GooglePhotosNewAlbum
    {
        public GooglePhotosAlbum album { get; set; }
    }

    public class GooglePhotosAlbumOptions
    {
        public GooglePhotosSharedAlbumOptions sharedAlbumOptions { get; set; }
    }

    public class GooglePhotosSharedAlbumOptions
    {
        public string isCollaborative { get; set; }
        public string isCommentable { get; set; }
    }

    public class GooglePhotosAlbumOptionsResponse
    {
        public GooglePhotosShareInfo shareInfo { get; set; }
    }

    public class GooglePhotosShareInfo
    {
        public GooglePhotosSharedAlbumOptions sharedAlbumOptions { get; set; }
        public string shareableUrl { get; set; }
        public string shareToken { get; set; }
        public string isJoined { get; set; }
    }
}