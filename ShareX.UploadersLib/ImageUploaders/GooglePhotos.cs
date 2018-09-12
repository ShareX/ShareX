#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class GooglePhotosImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Picasa;

        public override Icon ServiceIcon => Resources.GooglePhotos;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.PicasaOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new GooglePhotos(config.PicasaOAuth2Info)
            {
                AlbumID = config.PicasaAlbumID
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGooglePhotos;
    }

    public class GooglePhotos : ImageUploader, IOAuth2
    {
        private GoogleOAuth2 GoogleAuth { get; set; }
        public string AlbumID { get; set; }

        public GooglePhotos(OAuth2Info oauth)
        {
            GoogleAuth = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/photoslibrary"
            };
        }

        public OAuth2Info AuthInfo => GoogleAuth.AuthInfo;

        public bool RefreshAccessToken()
        {
            return GoogleAuth.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return GoogleAuth.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return GoogleAuth.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return GoogleAuth.GetAccessToken(code);
        }

        public List<GooglePhotosAlbumInfo> GetAlbumList()
        {
            if (!CheckAuthorization()) return null;

            List<GooglePhotosAlbumInfo> albumList = new List<GooglePhotosAlbumInfo>();

            string response = SendRequest(HttpMethod.GET, "https://photoslibrary.googleapis.com/v1/albums", headers: GoogleAuth.GetAuthHeaders());

            //if (!string.IsNullOrEmpty(response))
            //{
            //    XDocument xd = XDocument.Parse(response);

            //    if (xd != null)
            //    {
            //        foreach (XElement entry in xd.Descendants(AtomNS + "entry"))
            //        {
            //            GooglePhotosAlbumInfo album = new GooglePhotosAlbumInfo();
            //            album.ID = entry.GetElementValue(GPhotoNS + "id");
            //            album.Name = entry.GetElementValue(AtomNS + "title");
            //            album.Summary = entry.GetElementValue(AtomNS + "summary");
            //            albumList.Add(album);
            //        }
            //    }
            //}

            return albumList;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            if (string.IsNullOrEmpty(AlbumID))
            {
                AlbumID = "default";
            }

            UploadResult ur = new UploadResult();

            //string url = string.Format("https://picasaweb.google.com/data/feed/api/user/default/albumid/" + AlbumID);
            //string contentType = Helpers.GetMimeType(fileName);

            //NameValueCollection headers = GetAuthHeaders();
            //headers.Add("Slug", URLHelpers.URLEncode(fileName));

            //ur.Response = SendRequest(HttpMethod.POST, url, stream, contentType, null, headers);

            //if (ur.Response != null)
            //{
            //    XDocument xd = XDocument.Parse(ur.Response);

            //    XElement entry_element = xd.Element(AtomNS + "entry");

            //    if (entry_element != null)
            //    {
            //        XElement group_element = entry_element.Element(MediaNS + "group");

            //        if (group_element != null)
            //        {
            //            XElement content_element = group_element.Element(MediaNS + "content");

            //            if (content_element != null)
            //            {
            //                ur.ThumbnailURL = content_element.GetAttributeValue("url");

            //                int last_slash_index = ur.ThumbnailURL.LastIndexOf(@"/");

            //                ur.URL = ur.ThumbnailURL.Insert(last_slash_index, @"/s0");
            //            }
            //        }
            //    }
            //}

            throw new NotImplementedException();
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
        public Album[] albums { get; set; }
        public string nextPageToken { get; set; }
    }

    public class Album
    {
        public string id { get; set; }
        public string title { get; set; }
        public string productUrl { get; set; }
        public string coverPhotoBaseUrl { get; set; }
        public string coverPhotoMediaItemId { get; set; }
        public string isWriteable { get; set; }
        public string mediaItemsCount { get; set; }
    }

    public class GooglePhotosMediaItem
    {
        public string albumId { get; set; }
        public Newmediaitem[] newMediaItems { get; set; }
    }

    public class Newmediaitem
    {
        public string description { get; set; }
        public Simplemediaitem simpleMediaItem { get; set; }
    }

    public class Simplemediaitem
    {
        public string uploadToken { get; set; }
    }
}