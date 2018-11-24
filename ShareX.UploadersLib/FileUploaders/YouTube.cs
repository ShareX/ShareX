#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class YouTubeFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.YouTube;

        public override Icon ServiceIcon => Resources.YouTube;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuth2Info.CheckOAuth(config.YouTubeOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new YouTube(config.YouTubeOAuth2Info)
            {
                PrivacyType = config.YouTubePrivacyType,
                UseShortenedLink = config.YouTubeUseShortenedLink
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpYouTube;
    }

    public sealed class YouTube : FileUploader, IOAuth2
    {
        public OAuth2Info AuthInfo => googleAuth.AuthInfo;
        public YouTubeVideoPrivacy PrivacyType { get; set; }
        public bool UseShortenedLink { get; set; }

        private GoogleOAuth2 googleAuth;

        public YouTube(OAuth2Info oauth)
        {
            googleAuth = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/youtube.upload"
            };
        }

        public bool RefreshAccessToken()
        {
            return googleAuth.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return googleAuth.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return googleAuth.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return googleAuth.GetAccessToken(code);
        }

        private string GetMetadata(string title)
        {
            object metadata = new
            {
                snippet = new
                {
                    title = title
                },
                status = new
                {
                    privacyStatus = PrivacyType.ToString()
                }
            };

            return JsonConvert.SerializeObject(metadata);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            string metadata = GetMetadata(fileName);

            UploadResult result = SendRequestFile("https://www.googleapis.com/upload/youtube/v3/videos?part=id,snippet,status", stream, fileName,
                headers: googleAuth.GetAuthHeaders(), metadata: metadata);

            if (!string.IsNullOrEmpty(result.Response))
            {
                YouTubeVideo video = JsonConvert.DeserializeObject<YouTubeVideo>(result.Response);

                if (video != null)
                {
                    if (UseShortenedLink)
                    {
                        result.URL = $"https://youtu.be/{video.id}";
                    }
                    else
                    {
                        result.URL = $"https://www.youtube.com/watch?v={video.id}";
                    }

                    switch (video.status.uploadStatus)
                    {
                        case YouTubeVideoStatus.UploadFailed:
                            Errors.Add("Upload failed: " + video.status.failureReason);
                            break;
                        case YouTubeVideoStatus.UploadRejected:
                            Errors.Add("Upload rejected: " + video.status.rejectionReason);
                            break;
                    }
                }
            }

            return result;
        }
    }

    public class YouTubeVideo
    {
        public string id { get; set; }
        public YouTubeVideoSnippet snippet { get; set; }
        public YouTubeVideoStatus status { get; set; }
    }

    public class YouTubeVideoSnippet
    {
        public string title { get; set; }
    }

    public class YouTubeVideoStatus
    {
        public const string UploadFailed = "failed";
        public const string UploadRejected = "rejected";

        public YouTubeVideoPrivacy privacyStatus { get; set; }
        public string uploadStatus { get; set; }
        public string failureReason { get; set; }
        public string rejectionReason { get; set; }
    }
}