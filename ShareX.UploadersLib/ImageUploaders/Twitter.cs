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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class TwitterImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Twitter;

        public override Icon ServiceIcon => Resources.Twitter;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.TwitterOAuthInfoList != null && config.TwitterOAuthInfoList.IsValidIndex(config.TwitterSelectedAccount) &&
                OAuthInfo.CheckOAuth(config.TwitterOAuthInfoList[config.TwitterSelectedAccount]);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            OAuthInfo twitterOAuth = config.TwitterOAuthInfoList.ReturnIfValidIndex(config.TwitterSelectedAccount);

            return new Twitter(twitterOAuth)
            {
                SkipMessageBox = config.TwitterSkipMessageBox,
                DefaultMessage = config.TwitterDefaultMessage ?? ""
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpTwitter;
    }

    public class Twitter : ImageUploader, IOAuth
    {
        private const string APIVersion = "1.1";
        private const int characters_reserved_per_media = 23;

        public const int MessageLimit = 280;
        public const int MessageMediaLimit = MessageLimit - characters_reserved_per_media;

        public OAuthInfo AuthInfo { get; set; }
        public bool SkipMessageBox { get; set; }
        public string DefaultMessage { get; set; }

        public Twitter(OAuthInfo oauth)
        {
            AuthInfo = oauth;
        }

        public string GetAuthorizationURL()
        {
            return GetAuthorizationURL("https://api.twitter.com/oauth/request_token", "https://api.twitter.com/oauth/authorize", AuthInfo);
        }

        public bool GetAccessToken(string verificationCode)
        {
            AuthInfo.AuthVerifier = verificationCode;
            return GetAccessToken("https://api.twitter.com/oauth/access_token", AuthInfo);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string message = DefaultMessage;

            if (!SkipMessageBox)
            {
                using (TwitterTweetForm twitterMsg = new TwitterTweetForm())
                {
                    twitterMsg.MediaMode = true;
                    twitterMsg.Message = DefaultMessage;

                    if (twitterMsg.ShowDialog() != DialogResult.OK)
                    {
                        return new UploadResult() { IsURLExpected = false };
                    }

                    message = twitterMsg.Message;
                }
            }

            return TweetMessageWithMedia(message, stream, fileName);
        }

        public TwitterStatusResponse TweetMessage(string message)
        {
            if (message.Length > MessageLimit)
            {
                message = message.Remove(MessageLimit);
            }

            string url = string.Format("https://api.twitter.com/{0}/statuses/update.json", APIVersion);
            string query = OAuthManager.GenerateQuery(url, null, HttpMethod.POST, AuthInfo);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("status", message);

            string response = SendRequestMultiPart(query, args);

            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<TwitterStatusResponse>(response);
            }

            return null;
        }

        public UploadResult TweetMessageWithMedia(string message, Stream stream, string fileName)
        {
            if (message.Length > MessageMediaLimit)
            {
                message = message.Remove(MessageMediaLimit);
            }

            string url = string.Format("https://api.twitter.com/{0}/statuses/update_with_media.json", APIVersion);
            string query = OAuthManager.GenerateQuery(url, null, HttpMethod.POST, AuthInfo);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("status", message);

            UploadResult result = SendRequestFile(query, stream, fileName, "media[]", args);

            if (!string.IsNullOrEmpty(result.Response))
            {
                TwitterStatusResponse status = JsonConvert.DeserializeObject<TwitterStatusResponse>(result.Response);

                if (status != null && status.user != null)
                {
                    result.URL = status.GetTweetURL();
                }
            }

            return result;
        }

        private string GetConfiguration()
        {
            string url = string.Format("https://api.twitter.com/{0}/help/configuration.json", APIVersion);
            string query = OAuthManager.GenerateQuery(url, null, HttpMethod.GET, AuthInfo);
            string response = SendRequest(HttpMethod.GET, query);
            return response;
        }
    }

    public class TwitterStatusResponse
    {
        public long id { get; set; }
        public string text { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public TwitterStatusUser user { get; set; }

        public string GetTweetURL()
        {
            return string.Format("https://twitter.com/{0}/status/{1}", user.screen_name, id);
        }
    }

    public class TwitterStatusUser
    {
        public long id { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
    }
}