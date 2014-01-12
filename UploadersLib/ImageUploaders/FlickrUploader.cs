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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UploadersLib.HelperClasses;

namespace UploadersLib.ImageUploaders
{
    public class FlickrUploader : ImageUploader
    {
        private string API_Key, API_Secret;

        private const string API_URL = "http://api.flickr.com/services/rest/";
        private const string API_Auth_URL = "http://www.flickr.com/services/auth/";
        private const string API_Upload_URL = "http://api.flickr.com/services/upload/";

        public FlickrAuthInfo Auth = new FlickrAuthInfo();
        public FlickrSettings Settings = new FlickrSettings();
        public string Frob;

        public FlickrUploader(string key, string secret)
        {
            API_Key = key;
            API_Secret = secret;
        }

        public FlickrUploader(string key, string secret, FlickrAuthInfo auth, FlickrSettings settings)
            : this(key, secret)
        {
            Auth = auth;
            Settings = settings;
        }

        #region Auth

        /// <summary>
        /// Returns the credentials attached to an authentication token.
        /// http://www.flickr.com/services/api/flickr.auth.checkToken.html
        /// </summary>
        public FlickrAuthInfo CheckToken(string token)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "flickr.auth.checkToken");
            args.Add("api_key", API_Key);
            args.Add("auth_token", token);
            args.Add("api_sig", GetAPISig(args));

            string response = SendPostRequest(API_URL, args);

            Auth = new FlickrAuthInfo(ParseResponse(response, "auth"));

            return Auth;
        }

        /// <summary>
        /// Returns a frob to be used during authentication.
        /// http://www.flickr.com/services/api/flickr.auth.getFrob.html
        /// </summary>
        public string GetFrob()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "flickr.auth.getFrob");
            args.Add("api_key", API_Key);
            args.Add("api_sig", GetAPISig(args));

            string response = SendPostRequest(API_URL, args);

            XElement eFrob = ParseResponse(response, "frob");

            if (eFrob == null)
            {
                string errorMessage = ToErrorString();

                if (string.IsNullOrEmpty(errorMessage))
                {
                    throw new Exception("getFrob failed.");
                }

                throw new Exception(errorMessage);
            }

            Frob = eFrob.Value;
            return Frob;
        }

        /// <summary>
        /// Get the full authentication token for a mini-token.
        /// http://www.flickr.com/services/api/flickr.auth.getFullToken.html
        /// </summary>
        public FlickrAuthInfo GetFullToken(string frob)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "flickr.auth.getFullToken");
            args.Add("api_key", API_Key);
            args.Add("mini_token", frob);
            args.Add("api_sig", GetAPISig(args));

            string response = SendPostRequest(API_URL, args);

            Auth = new FlickrAuthInfo(ParseResponse(response, "auth"));

            return Auth;
        }

        /// <summary>
        /// Returns the auth token for the given frob, if one has been attached.
        /// http://www.flickr.com/services/api/flickr.auth.getToken.html
        /// </summary>
        /// <returns></returns>
        public FlickrAuthInfo GetToken(string frob)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "flickr.auth.getToken");
            args.Add("api_key", API_Key);
            args.Add("frob", frob);
            args.Add("api_sig", GetAPISig(args));

            string response = SendPostRequest(API_URL, args);

            Auth = new FlickrAuthInfo(ParseResponse(response, "auth"));

            return Auth;
        }

        public FlickrAuthInfo GetToken()
        {
            return GetToken(Frob);
        }

        public string GetAuthLink()
        {
            return GetAuthLink(FlickrPermission.Write);
        }

        public string GetAuthLink(FlickrPermission perm)
        {
            if (!string.IsNullOrEmpty(Frob))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("api_key", API_Key);
                args.Add("perms", perm.ToString().ToLowerInvariant());
                args.Add("frob", Frob);
                args.Add("api_sig", GetAPISig(args));

                return string.Format("{0}?{1}", API_Auth_URL, string.Join("&", args.Select(x => x.Key + "=" + x.Value).ToArray()));
            }

            return null;
        }

        public string GetPhotosLink(string userID)
        {
            return Helpers.CombineURL("http://www.flickr.com/photos", userID);
        }

        public string GetPhotosLink()
        {
            return GetPhotosLink(Auth.UserID);
        }

        #endregion Auth

        #region Helpers

        private string GetAPISig(Dictionary<string, string> args)
        {
            return TranslatorHelper.TextToHash(args.OrderBy(x => x.Key).Aggregate(API_Secret, (x, x2) => x + x2.Key + x2.Value), HashType.MD5);
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
                            // throw new Exception(string.Format("Code: {0}, Message: {1}", code, msg));
                            Errors.Add(msg);
                            break;
                    }
                }
            }

            return null;
        }

        #endregion Helpers

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("api_key", API_Key);
            args.Add("auth_token", Auth.Token);

            if (!string.IsNullOrEmpty(Settings.Title)) args.Add("title", Settings.Title);
            if (!string.IsNullOrEmpty(Settings.Description)) args.Add("description", Settings.Description);
            if (!string.IsNullOrEmpty(Settings.Tags)) args.Add("tags", Settings.Tags);
            if (!string.IsNullOrEmpty(Settings.IsPublic)) args.Add("is_public", Settings.IsPublic);
            if (!string.IsNullOrEmpty(Settings.IsFriend)) args.Add("is_friend", Settings.IsFriend);
            if (!string.IsNullOrEmpty(Settings.IsFamily)) args.Add("is_family", Settings.IsFamily);
            if (!string.IsNullOrEmpty(Settings.SafetyLevel)) args.Add("safety_level", Settings.SafetyLevel);
            if (!string.IsNullOrEmpty(Settings.ContentType)) args.Add("content_type", Settings.ContentType);
            if (!string.IsNullOrEmpty(Settings.Hidden)) args.Add("hidden", Settings.Hidden);

            args.Add("api_sig", GetAPISig(args));

            UploadResult result = UploadData(stream, API_Upload_URL, fileName, "photo", args);

            if (result.IsSuccess)
            {
                XElement xele = ParseResponse(result.Response, "photoid");

                if (null != xele)
                {
                    string photoid = xele.Value;
                    string url = Helpers.CombineURL(GetPhotosLink(), photoid);
                    result.URL = Helpers.CombineURL(url, "sizes/o");
                }
            }

            return result;
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