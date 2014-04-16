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
using UploadersLib.HelperClasses;

namespace UploadersLib.SocialServices
{
    public class Twitter : Uploader, IOAuth
    {
        private const string APIVersion = "1.1";
        private const string URLRequestToken = "https://api.twitter.com/oauth/request_token";
        private const string URLAuthorize = "https://api.twitter.com/oauth/authorize";
        private const string URLAccessToken = "https://api.twitter.com/oauth/access_token";
        private const string URLTweet = "https://api.twitter.com/" + APIVersion + "/statuses/update.json";

        public OAuthInfo AuthInfo { get; set; }

        public Twitter(OAuthInfo oauth)
        {
            AuthInfo = oauth;
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

        public TweetStatus TweetMessage(string message)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("status", message);

            string query = OAuthManager.GenerateQuery(URLTweet, args, HttpMethod.POST, AuthInfo);

            string response = SendRequest(HttpMethod.POST, query);

            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<TweetStatus>(response);
            }

            return null;
        }
    }

    public class TweetStatus
    {
        public long id { get; set; }
        public string text { get; set; }
        public string in_reply_to_screen_name { get; set; }
    }
}