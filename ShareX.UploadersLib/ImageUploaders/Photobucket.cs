#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class PhotobucketImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Photobucket;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.PhotobucketAccountInfo != null && OAuthInfo.CheckOAuth(config.PhotobucketOAuthInfo);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Photobucket(config.PhotobucketOAuthInfo, config.PhotobucketAccountInfo);
        }
    }

    public sealed class Photobucket : ImageUploader, IOAuth
    {
        private const string URLRequestToken = "http://api.photobucket.com/login/request";
        private const string URLAuthorize = "http://photobucket.com/apilogin/login";
        private const string URLAccessToken = "http://api.photobucket.com/login/access";

        public OAuthInfo AuthInfo { get; set; }
        public PhotobucketAccountInfo AccountInfo { get; set; }

        public Photobucket(OAuthInfo oauth)
        {
            AuthInfo = oauth;
            AccountInfo = new PhotobucketAccountInfo();
        }

        public Photobucket(OAuthInfo oauth, PhotobucketAccountInfo accountInfo)
        {
            AuthInfo = oauth;
            AccountInfo = accountInfo;
        }

        public Task<string> GetAuthorizationURLAsync(CancellationToken cancellationToken = default)
        {
            return GetAuthorizationURLAsync(URLRequestToken, URLAuthorize, AuthInfo, null, HttpMethod.POST, cancellationToken);
        }

        public async Task<bool> GetAccessTokenAsync(string verificationCode, CancellationToken cancellationToken = default)
        {
            AuthInfo.AuthVerifier = verificationCode;

            NameValueCollection nv = await GetAccessTokenExAsync(URLAccessToken, AuthInfo, HttpMethod.POST,
                cancellationToken).ConfigureAwait(false);

            if (nv != null)
            {
                AccountInfo.Subdomain = nv["subdomain"];
                AccountInfo.AlbumID = nv["username"];
                return !string.IsNullOrEmpty(AccountInfo.Subdomain);
            }

            return false;
        }

        public PhotobucketAccountInfo GetAccountInfo()
        {
            return AccountInfo;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            return await UploadMediaAsync(stream, fileName, AccountInfo.ActiveAlbumPath, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UploadResult> UploadMediaAsync(Stream stream, string fileName, string albumID,
            CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("id", albumID); // Album identifier.
            args.Add("type", "image"); // Media type. Options are image, video, or base64.

            /*
            // Optional
            args.Add("title", ""); // Searchable title to set on the media. Maximum 250 characters.
            args.Add("description", ""); // Searchable description to set on the media. Maximum 2048 characters.
            args.Add("scramble", "false"); // Indicates if the filename should be scrambled. Options are true or false.
            args.Add("degrees", ""); // Degrees of rotation in 90 degree increments.
            args.Add("size", ""); // Size to resize an image to. (Images can only be made smaller.)
            */

            string url = "http://api.photobucket.com/album/!/upload";
            string query = OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo);
            query = FixURL(query);

            UploadResult result = await SendRequestFileAsync(query, stream, fileName, "uploadfile",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                XDocument xd = XDocument.Parse(result.Response);
                XElement xe;

                if ((xe = xd.GetNode("response/content")) != null)
                {
                    result.URL = xe.GetElementValue("url");
                    result.ThumbnailURL = xe.GetElementValue("thumb");
                }
            }

            return result;
        }

        public async Task<bool> CreateAlbumAsync(string albumID, string albumName, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("id", albumID); // Album identifier.
            args.Add("name", albumName); // Name of result. Must be between 2 and 50 characters. Valid characters are letters, numbers, underscore ( _ ), hyphen (-), and space.

            string url = "http://api.photobucket.com/album/!";
            string query = OAuthManager.GenerateQuery(url, args, HttpMethod.POST, AuthInfo);
            query = FixURL(query);

            string response = await SendRequestMultiPartAsync(query, args, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
                XDocument xd = XDocument.Parse(response);
                XElement xe;

                if ((xe = xd.GetNode("response")) != null)
                {
                    string status = xe.GetElementValue("status");

                    return !string.IsNullOrEmpty(status) && status == "OK";
                }
            }

            return false;
        }

        private string FixURL(string url)
        {
            return url.Replace("api.photobucket.com", AccountInfo.Subdomain);
        }
    }

    public class PhotobucketAccountInfo
    {
        public string Subdomain { get; set; }

        public string AlbumID { get; set; }

        public List<string> AlbumList = new List<string>();
        public int ActiveAlbumID = 0;

        public string ActiveAlbumPath
        {
            get
            {
                return AlbumList[ActiveAlbumID];
            }
        }
    }
}
