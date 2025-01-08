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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ShareX.UploadersLib.ImageUploaders
{
    public enum TwitPicUploadType
    {
        [Description("Upload Image")]
        UPLOAD_IMAGE_ONLY,
        [Description("Upload Image and update Twitter Status")]
        UPLOAD_IMAGE_AND_TWITTER
    }

    public enum TwitPicThumbnailType
    {
        [Description("Mini Thumbnail")]
        Mini,
        [Description("Normal Thumbnail")]
        Thumb
    }

    public sealed class TwitPicUploader : ImageUploader
    {
        public string APIKey { get; private set; }
        public OAuthInfo AuthInfo { get; private set; }

        public TwitPicUploadType TwitPicUploadType { get; set; }
        public bool ShowFull { get; set; }
        public TwitPicThumbnailType TwitPicThumbnailMode { get; set; }

        private const string UploadLink = "http://api.twitpic.com/1/upload.json";
        private const string UploadAndPostLink = "http://api.twitpic.com/1/uploadAndPost.json";

        public TwitPicUploader(string key, OAuthInfo oauth)
        {
            APIKey = key;
            AuthInfo = oauth;
            TwitPicUploadType = TwitPicUploadType.UPLOAD_IMAGE_ONLY;
            ShowFull = false;
            TwitPicThumbnailMode = TwitPicThumbnailType.Thumb;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            switch (TwitPicUploadType)
            {
                case TwitPicUploadType.UPLOAD_IMAGE_ONLY:
                    return Upload(stream, fileName, UploadLink);
                case TwitPicUploadType.UPLOAD_IMAGE_AND_TWITTER:
                    using (TwitterTweetForm msgBox = new TwitterTweetForm())
                    {
                        msgBox.ShowDialog();
                        return Upload(stream, fileName, UploadAndPostLink, msgBox.Message);
                    }
            }

            return null;
        }

        private UploadResult Upload(Stream stream, string fileName, string url, string msg = "")
        {
            if (AuthInfo == null || string.IsNullOrEmpty(AuthInfo.UserToken) || string.IsNullOrEmpty(AuthInfo.UserSecret))
            {
                Errors.Add("Login is required.");
                return null;
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("key", APIKey);
            args.Add("consumer_token", AuthInfo.ConsumerKey);
            args.Add("consumer_secret", AuthInfo.ConsumerSecret);
            args.Add("oauth_token", AuthInfo.UserToken);
            args.Add("oauth_secret", AuthInfo.UserSecret);
            args.Add("message", msg);

            UploadResult result = SendRequestFile(url, stream, fileName, "media", args);

            TwitPicResponse response = JsonConvert.DeserializeObject<TwitPicResponse>(result.Response);

            if (response != null)
            {
                result.URL = response.URL;
                if (ShowFull) result.URL += "/full";
                result.ThumbnailURL = string.Format("http://twitpic.com/show/{0}/{1}.{2}", TwitPicThumbnailMode.ToString().ToLowerInvariant(), response.ID, response.Type);
            }

            return result;
        }

        public class TwitPicResponse
        {
            public string ID { get; set; }
            public string Text { get; set; }
            public string URL { get; set; }
            public string Width { get; set; }
            public string Height { get; set; }
            public string Size { get; set; }
            public string Type { get; set; }
            public string Timestamp { get; set; }

            public class User
            {
                public string ID { get; set; }
                public string Screen_Name { get; set; }
            }
        }
    }
}