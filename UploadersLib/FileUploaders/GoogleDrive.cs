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
using UploadersLib.HelperClasses;

namespace UploadersLib.FileUploaders
{
    public sealed class GoogleDrive : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public bool IsPublic { get; set; }

        public GoogleDrive(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            return string.Format("https://accounts.google.com/o/oauth2/auth?response_type={0}&client_id={1}&redirect_uri={2}&scope={3}",
                "code", AuthInfo.Client_ID, "urn:ietf:wg:oauth:2.0:oob", Helpers.URLEncode("https://www.googleapis.com/auth/drive"));
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

        public void SetPermissions(string fileID, GoogleDrivePermissionRole role, GoogleDrivePermissionType type, string value, bool withLink)
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);

            string url = string.Format("https://www.googleapis.com/drive/v2/files/{0}/permissions", fileID);

            string json = JsonConvert.SerializeObject(new
            {
                role = role.ToString(),
                type = type.ToString(),
                value = value,
                withLink = withLink.ToString()
            });

            string response = SendPostRequestJSON(url, json, headers: headers);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("title", fileName);

            UploadResult result = UploadData(stream, "https://www.googleapis.com/upload/drive/v2/files", fileName, "file", args, headers: headers);

            if (!string.IsNullOrEmpty(result.Response))
            {
                GoogleDriveUpload upload = JsonConvert.DeserializeObject<GoogleDriveUpload>(result.Response);

                if (upload != null)
                {
                    if (IsPublic)
                    {
                        AllowReportProgress = false;
                        SetPermissions(upload.id, GoogleDrivePermissionRole.reader, GoogleDrivePermissionType.anyone, "", true);
                    }

                    result.URL = upload.alternateLink;
                }
            }

            return result;
        }

        public class GoogleDriveUpload
        {
            public string id { get; set; }
            public string alternateLink { get; set; }
        }

        public enum GoogleDrivePermissionRole
        {
            owner, reader, writer
        }

        public enum GoogleDrivePermissionType
        {
            user, group, domain, anyone
        }
    }
}