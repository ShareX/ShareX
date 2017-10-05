#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
            return new FlickrUploader(config.FlickrOAuthInfo);
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
            args.Add("oauth_callback", Links.URL_CALLBACK);

            string url = GetAuthorizationURL("https://www.flickr.com/services/oauth/request_token", "https://www.flickr.com/services/oauth/authorize", AuthInfo, args);

            return url + "&perms=write";
        }

        public bool GetAccessToken(string verificationCode = null)
        {
            AuthInfo.AuthVerifier = verificationCode;
            return GetAccessToken("https://www.flickr.com/services/oauth/access_token", AuthInfo);
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

            string query = OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo, out Dictionary<string, string> parameters);

            UploadResult result = SendRequestFile(url, stream, fileName, "photo", parameters);

            if (result.IsSuccess)
            {
                XElement xele = ParseResponse(result.Response, "photoid");

                if (null != xele)
                {
                    string photoid = xele.Value;
                    //string url = URLHelpers.CombineURL(GetPhotosLink(), photoid);
                    //result.URL = URLHelpers.CombineURL(url, "sizes/o");
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
                            string code = err.GetAttributeValue("code");
                            string msg = err.GetAttributeValue("msg");
                            Errors.Add(msg);
                            break;
                    }
                }
            }

            return null;
        }
    }

    public class FlickrAuthInfo
    {
        [Description("Token string"), ReadOnly(true), PasswordPropertyText(true)]
        public string Token { get; set; }

        [Description("Permission"), ReadOnly(true)]
        public string Permission { get; set; }

        [Description("User ID that can be used in a URL")]
        public string UserID { get; set; }

        [Description("Your Flickr username"), ReadOnly(true)]
        public string Username { get; set; }

        [Description("Full name"), ReadOnly(true)]
        public string Fullname { get; set; }

        public FlickrAuthInfo()
        {
        }

        public FlickrAuthInfo(XElement element)
        {
            Token = element.GetElementValue("token");
            Permission = element.GetElementValue("perms");
            XElement user = element.Element("user");
            UserID = user.GetAttributeValue("nsid");
            Username = user.GetAttributeValue("username");
            Fullname = user.GetAttributeValue("fullname");
        }
    }

    public class FlickrSettings
    {
        /// <summary>
        /// The title of the photo.
        /// </summary>
        [Description("The title of the photo.")]
        public string Title { get; set; }

        /// <summary>
        /// A description of the photo. May contain some limited HTML.
        /// </summary>
        [Description("A description of the photo. May contain some limited HTML.")]
        public string Description { get; set; }

        /// <summary>
        /// A space-seperated list of tags to apply to the photo.
        /// </summary>
        [Description("A space-seperated list of tags to apply to the photo.")]
        public string Tags { get; set; }

        /// <summary>
        /// Set to 0 for no, 1 for yes. Specifies who can view the photo.
        /// </summary>
        [Description("Set to 0 for no, 1 for yes. Specifies who can view the photo.")]
        public string IsPublic { get; set; }

        /// <summary>
        /// Set to 0 for no, 1 for yes. Specifies who can view the photo.
        /// </summary>
        [Description("Set to 0 for no, 1 for yes. Specifies who can view the photo.")]
        public string IsFriend { get; set; }

        /// <summary>
        /// Set to 0 for no, 1 for yes. Specifies who can view the photo.
        /// </summary>
        [Description("Set to 0 for no, 1 for yes. Specifies who can view the photo.")]
        public string IsFamily { get; set; }

        /// <summary>
        /// Set to 1 for Safe, 2 for Moderate, or 3 for Restricted.
        /// </summary>
        [Description("Set to 1 for Safe, 2 for Moderate, or 3 for Restricted.")]
        public string SafetyLevel { get; set; }

        /// <summary>
        /// Set to 1 for Photo, 2 for Screenshot, or 3 for Other.
        /// </summary>
        [Description("Set to 1 for Photo, 2 for Screenshot, or 3 for Other.")]
        public string ContentType { get; set; }

        /// <summary>
        /// Set to 1 to keep the photo in global search results, 2 to hide from public searches.
        /// </summary>
        [Description("Set to 1 to keep the photo in global search results, 2 to hide from public searches.")]
        public string Hidden { get; set; }
    }

    public enum FlickrPermission
    {
        None,
        /// <summary>
        /// Permission to read private information
        /// </summary>
        Read,
        /// <summary>
        /// Permission to add, edit and delete photo metadata (includes 'read')
        /// </summary>
        Write,
        /// <summary>
        /// Permission to delete photos (includes 'write' and 'read')
        /// </summary>
        Delete
    }
}