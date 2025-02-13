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
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class FlickrImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Flickr;

        public override Icon ServiceIcon => Resources.Flickr;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuthInfo.CheckOAuth(config.FlickrOAuthInfo);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new FlickrUploader(config.FlickrOAuthInfo, config.FlickrSettings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpFlickr;
    }

    public class FlickrUploader : ImageUploader, IOAuth
    {
        public OAuthInfo AuthInfo { get; set; }
        public FlickrSettings Settings { get; set; } = new FlickrSettings();

        public FlickrUploader(OAuthInfo oauth)
        {
            AuthInfo = oauth;
        }

        public FlickrUploader(OAuthInfo oauth, FlickrSettings settings)
        {
            AuthInfo = oauth;
            Settings = settings;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("oauth_callback", Links.Callback);

            string url = GetAuthorizationURL("https://www.flickr.com/services/oauth/request_token", "https://www.flickr.com/services/oauth/authorize", AuthInfo, args);

            return url + "&perms=write";
        }

        public bool GetAccessToken(string verificationCode = null)
        {
            AuthInfo.AuthVerifier = verificationCode;
            return GetAccessToken("https://www.flickr.com/services/oauth/access_token", AuthInfo);
        }

        public FlickrPhotosGetSizesResponse PhotosGetSizes(string photoid)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("nojsoncallback", "1");
            args.Add("format", "json");
            args.Add("method", "flickr.photos.getSizes");
            args.Add("photo_id", photoid);

            string query = OAuthManager.GenerateQuery("https://api.flickr.com/services/rest", args, HttpMethod.POST, AuthInfo);

            string response = SendRequest(HttpMethod.GET, query);

            return JsonConvert.DeserializeObject<FlickrPhotosGetSizesResponse>(response);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string url = "https://up.flickr.com/services/upload/";

            Dictionary<string, string> args = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(Settings.Title)) args.Add("title", Settings.Title);
            if (!string.IsNullOrEmpty(Settings.Description)) args.Add("description", Settings.Description);
            if (!string.IsNullOrEmpty(Settings.Tags)) args.Add("tags", Settings.Tags);
            if (!string.IsNullOrEmpty(Settings.IsPublic)) args.Add("is_public", Settings.IsPublic);
            if (!string.IsNullOrEmpty(Settings.IsFriend)) args.Add("is_friend", Settings.IsFriend);
            if (!string.IsNullOrEmpty(Settings.IsFamily)) args.Add("is_family", Settings.IsFamily);
            if (!string.IsNullOrEmpty(Settings.SafetyLevel)) args.Add("safety_level", Settings.SafetyLevel);
            if (!string.IsNullOrEmpty(Settings.ContentType)) args.Add("content_type", Settings.ContentType);
            if (!string.IsNullOrEmpty(Settings.Hidden)) args.Add("hidden", Settings.Hidden);

            OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo, out Dictionary<string, string> parameters);

            UploadResult result = SendRequestFile(url, stream, fileName, "photo", parameters);

            if (result.IsSuccess)
            {
                XElement xele = ParseResponse(result.Response, "photoid");

                if (xele != null)
                {
                    string photoid = xele.Value;
                    FlickrPhotosGetSizesResponse photos = PhotosGetSizes(photoid);
                    if (photos != null && photos.sizes != null && photos.sizes.size != null && photos.sizes.size.Length > 0)
                    {
                        FlickrPhotosGetSizesSize photo = photos.sizes.size[photos.sizes.size.Length - 1];

                        if (Settings.DirectLink)
                        {
                            result.URL = photo.source;
                        }
                        else
                        {
                            result.URL = photo.url;
                        }
                    }
                }
            }

            return result;
        }

        private XElement ParseResponse(string response, string field)
        {
            if (!string.IsNullOrEmpty(response))
            {
                XDocument xdoc = XDocument.Parse(response);
                XElement xele = xdoc.Element("rsp");

                if (xele != null)
                {
                    switch (xele.GetAttributeFirstValue("status", "stat"))
                    {
                        case "ok":
                            return xele.Element(field);
                        case "fail":
                            XElement err = xele.Element("err");
                            //string code = err.GetAttributeValue("code");
                            string msg = err.GetAttributeValue("msg");
                            Errors.Add(msg);
                            break;
                    }
                }
            }

            return null;
        }
    }

    public class FlickrSettings
    {
        public bool DirectLink { get; set; } = true;

        [Description("The title of the photo.")]
        public string Title { get; set; }

        [Description("A description of the photo. May contain some limited HTML.")]
        public string Description { get; set; }

        [Description("A space-seperated list of tags to apply to the photo.")]
        public string Tags { get; set; }

        [Description("Set to 0 for no, 1 for yes. Specifies who can view the photo.")]
        public string IsPublic { get; set; }

        [Description("Set to 0 for no, 1 for yes. Specifies who can view the photo.")]
        public string IsFriend { get; set; }

        [Description("Set to 0 for no, 1 for yes. Specifies who can view the photo.")]
        public string IsFamily { get; set; }

        [Description("Set to 1 for Safe, 2 for Moderate, or 3 for Restricted.")]
        public string SafetyLevel { get; set; }

        [Description("Set to 1 for Photo, 2 for Screenshot, or 3 for Other.")]
        public string ContentType { get; set; }

        [Description("Set to 1 to keep the photo in global search results, 2 to hide from public searches.")]
        public string Hidden { get; set; }
    }

    public class FlickrPhotosGetSizesResponse
    {
        public FlickrPhotosGetSizesSizes sizes { get; set; }
        public string stat { get; set; }
    }

    public class FlickrPhotosGetSizesSizes
    {
        public int canblog { get; set; }
        public bool canprint { get; set; }
        public int candownload { get; set; }
        public FlickrPhotosGetSizesSize[] size { get; set; }
    }

    public class FlickrPhotosGetSizesSize
    {
        public string label { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string source { get; set; }
        public string url { get; set; }
        public string media { get; set; }
    }
}