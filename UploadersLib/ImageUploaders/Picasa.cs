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

using HelpersLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using UploadersLib.HelperClasses;

namespace UploadersLib.ImageUploaders
{
    public class Picasa : ImageUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public string AlbumID { get; set; }

        private static readonly XNamespace AtomNS = "http://www.w3.org/2005/Atom";
        private static readonly XNamespace MediaNS = "http://search.yahoo.com/mrss/";
        private static readonly XNamespace GPhotoNS = "http://schemas.google.com/photos/2007";

        public Picasa(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            return string.Format("https://accounts.google.com/o/oauth2/auth?response_type={0}&client_id={1}&redirect_uri={2}&scope={3}",
                "code", AuthInfo.Client_ID, "urn:ietf:wg:oauth:2.0:oob", Helpers.URLEncode("https://picasaweb.google.com/data"));
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", code);
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
            args.Add("grant_type", "authorization_code");

            string response = SendPostRequest("https://accounts.google.com/o/oauth2/token", args);

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

                string response = SendPostRequest("https://accounts.google.com/o/oauth2/token", args);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                    if (token != null && !string.IsNullOrEmpty(token.access_token))
                    {
                        token.UpdateExpireDate();
                        string refresh_token = AuthInfo.Token.refresh_token;
                        AuthInfo.Token = token;
                        AuthInfo.Token.refresh_token = refresh_token;
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
                Errors.Add("Login is required.");
                return false;
            }

            return true;
        }

        public List<PicasaAlbumInfo> GetAlbumList()
        {
            if (!CheckAuthorization()) return null;

            List<PicasaAlbumInfo> albumList = new List<PicasaAlbumInfo>();

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);

            string response = SendGetRequest("https://picasaweb.google.com/data/feed/api/user/default", null, ResponseType.Text, null, headers);

            if (!string.IsNullOrEmpty(response))
            {
                XDocument xd = XDocument.Parse(response);

                if (xd != null)
                {
                    foreach (XElement entry in xd.Descendants(AtomNS + "entry"))
                    {
                        PicasaAlbumInfo album = new PicasaAlbumInfo();
                        album.ID = entry.GetElementValue(GPhotoNS + "id");
                        album.Name = entry.GetElementValue(GPhotoNS + "name");
                        album.Summary = entry.GetElementValue(AtomNS + "summary");
                        albumList.Add(album);
                    }
                }
            }

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

            string url = string.Format("https://picasaweb.google.com/data/feed/api/user/default/albumid/" + AlbumID);
            string contentType = Helpers.GetMimeType(fileName);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            headers.Add("Slug", Helpers.URLEncode(fileName));

            ur.Response = SendPostRequestStream(url, stream, contentType, null, headers);

            XDocument xd = XDocument.Parse(ur.Response);

            XElement entry_element = xd.Element(AtomNS + "entry");

            if (entry_element != null)
            {
                XElement group_element = entry_element.Element(MediaNS + "group");

                if (group_element != null)
                {
                    XElement content_element = group_element.Element(MediaNS + "content");

                    if (content_element != null)
                    {
                        ur.ThumbnailURL = content_element.GetAttributeValue("url");

                        int last_slash_index = ur.ThumbnailURL.LastIndexOf(@"/");

                        ur.URL = ur.ThumbnailURL.Insert(last_slash_index, @"/s0");
                    }
                }
            }

            return ur;
        }
    }

    public class PicasaAlbumInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
    }
}