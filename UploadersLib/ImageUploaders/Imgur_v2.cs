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
using System.Xml.Linq;
using UploadersLib.HelperClasses;

namespace UploadersLib.ImageUploaders
{
    public enum ImgurThumbnailType
    {
        [Description("Small Square")]
        Small_Square,
        [Description("Large Thumbnail")]
        Large_Thumbnail
    }

    public sealed class Imgur_v2 : ImageUploader, IOAuth
    {
        private const string URLAnonymousUpload = "http://api.imgur.com/2/upload.xml";
        private const string URLUserUpload = "http://api.imgur.com/2/account/images.xml";

        private const string URLRequestToken = "https://api.imgur.com/oauth/request_token";
        private const string URLAuthorize = "https://api.imgur.com/oauth/authorize";
        private const string URLAccessToken = "https://api.imgur.com/oauth/access_token";

        public AccountType UploadMethod { get; set; }
        public string AnonymousKey { get; set; }
        public OAuthInfo AuthInfo { get; set; }
        public ImgurThumbnailType ThumbnailType { get; set; }

        public Imgur_v2(AccountType uploadMethod, string anonymousKey, OAuthInfo oauth)
        {
            UploadMethod = uploadMethod;
            AnonymousKey = anonymousKey;
            AuthInfo = oauth;
        }

        public Imgur_v2(string anonymousKey)
        {
            UploadMethod = AccountType.Anonymous;
            AnonymousKey = anonymousKey;
        }

        public Imgur_v2(OAuthInfo oauth)
        {
            UploadMethod = AccountType.User;
            AuthInfo = oauth;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult ur;

            switch (UploadMethod)
            {
                default:
                case AccountType.Anonymous:
                    ur = AnonymousUpload(stream, fileName);
                    break;
                case AccountType.User:
                    ur = UserUpload(stream, fileName);
                    break;
            }

            return ur;
        }

        public string GetAuthorizationURL()
        {
            return GetAuthorizationURL(URLRequestToken, URLAuthorize, AuthInfo);
        }

        public bool GetAccessToken(string verificationCode)
        {
            AuthInfo.AuthVerifier = verificationCode;
            return GetAccessToken(URLAccessToken, AuthInfo);
        }

        private UploadResult UserUpload(Stream stream, string fileName)
        {
            if (AuthInfo == null || string.IsNullOrEmpty(AuthInfo.UserToken) || string.IsNullOrEmpty(AuthInfo.UserSecret))
            {
                Errors.Add("Login is required.");
                return null;
            }

            string query = OAuthManager.GenerateQuery(URLUserUpload, null, HttpMethod.POST, AuthInfo);

            UploadResult result = UploadData(stream, query, fileName, "image");

            return ParseResponse(result);
        }

        private UploadResult AnonymousUpload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("key", AnonymousKey);

            UploadResult result = UploadData(stream, URLAnonymousUpload, fileName, "image", arguments);

            return ParseResponse(result);
        }

        private UploadResult ParseResponse(UploadResult result)
        {
            if (result.IsSuccess)
            {
                try
                {
                    XDocument xd = XDocument.Parse(result.Response);
                    XElement xe;

                    if ((xe = xd.GetNode("upload|images/links")) != null)
                    {
                        result.URL = xe.GetElementValue("original");

                        if (ThumbnailType == ImgurThumbnailType.Large_Thumbnail)
                        {
                            result.ThumbnailURL = xe.GetElementValue("large_thumbnail");
                        }
                        else
                        {
                            result.ThumbnailURL = xe.GetElementValue("small_square");
                        }

                        result.DeletionURL = xe.GetElementValue("delete_page");
                    }
                    else if ((xe = xd.GetElement("error")) != null)
                    {
                        Errors.Add("Imgur error message: " + xe.GetElementValue("message"));
                    }
                }
                catch (Exception e)
                {
                    Errors.Add(e.ToString());
                }
            }

            return result;
        }
    }
}