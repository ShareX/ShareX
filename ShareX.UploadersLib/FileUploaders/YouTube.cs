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
using Newtonsoft.Json.Converters;
using ShareX.HelpersLib;
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
                PrivacyType = config.YouTubePrivacyType
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpYouTube;
    }

    public sealed class YouTube : FileUploader, IOAuth2
    {
        private GoogleOAuth2 GoogleAuth { get; set; }
        public YouTubeVideoPrivacy PrivacyType { get; set; }

        public YouTube(OAuth2Info oauth)
        {
            GoogleAuth = new GoogleOAuth2(oauth, this)
            {
                Scope = "https://www.googleapis.com/auth/youtube.upload"
            };
        }

        public OAuth2Info AuthInfo => GoogleAuth.AuthInfo;

        public bool RefreshAccessToken()
        {
            return GoogleAuth.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return GoogleAuth.CheckAuthorization();
        }

        public string GetAuthorizationURL()
        {
            return GoogleAuth.GetAuthorizationURL();
        }

        public bool GetAccessToken(string code)
        {
            return GoogleAuth.GetAccessToken(code);
        }

        private string GetMetadata(string title)
        {
            object metadata;

            metadata = new
            {
                snippet = new
                {
                    title = title
                },
                status = new
                {
                    privacyStatus = PrivacyType
                }
            };

            return JsonConvert.SerializeObject(metadata);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization()) return null;

            if (!Helpers.IsVideoFile(fileName))
            {
                Errors.Add("YouTube only supports video files");
                return null;
            }

            string metadata = GetMetadata(fileName);

            UploadResult result = SendRequestFile("https://www.googleapis.com/upload/youtube/v3/videos?part=id,snippet,status", stream, fileName,
                headers: GoogleAuth.GetAuthHeaders(), metadata: metadata);

            if (!string.IsNullOrEmpty(result.Response))
            {
                YouTubeVideo upload = JsonConvert.DeserializeObject<YouTubeVideo>(result.Response);

                if (upload != null)
                {
                    AllowReportProgress = false;

                    result.URL = "https://youtu.be/" + upload.id;

                    switch (upload.status.uploadStatus)
                    {
                        case YouTubeVideoStatus.UploadFailed:
                            Errors.Add("Upload failed: " + upload.status.failureReason);
                            break;

                        case YouTubeVideoStatus.UploadRejected:
                            Errors.Add("Upload rejected: " + upload.status.rejectionReason);
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum YouTubeVideoPrivacy // Localized
    {
        Public,
        Unlisted,
        Private
    }
}