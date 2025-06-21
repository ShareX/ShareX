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
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class StreamableFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Streamable;

        public override Icon ServiceIcon => Resources.Streamable;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.StreamableUsername) && !string.IsNullOrEmpty(config.StreamablePassword);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Streamable(config.StreamableUsername, config.StreamablePassword)
            {
                UseDirectURL = config.StreamableUseDirectURL
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpStreamable;
    }

    public class Streamable : FileUploader
    {
        private const string Host = "https://api.streamable.com";

        public string Email { get; private set; }
        public string Password { get; private set; }
        public bool UseDirectURL { get; set; }

        public Streamable(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            NameValueCollection headers = null;

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                headers = RequestHelpers.CreateAuthenticationHeader(Email, Password);
            }

            string url = URLHelpers.CombineURL(Host, "upload");
            UploadResult result = SendRequestFile(url, stream, fileName, "file", headers: headers);

            TranscodeFile(result);

            return result;
        }

        private void TranscodeFile(UploadResult result)
        {
            StreamableTranscodeResponse transcodeResponse = JsonConvert.DeserializeObject<StreamableTranscodeResponse>(result.Response);

            if (!string.IsNullOrEmpty(transcodeResponse.Shortcode))
            {
                ProgressManager progress = new ProgressManager(100);
                OnProgressChanged(progress);

                while (!StopUploadRequested)
                {
                    string statusJson = SendRequest(HttpMethod.GET, URLHelpers.CombineURL(Host, "videos", transcodeResponse.Shortcode));
                    StreamableStatusResponse response = JsonConvert.DeserializeObject<StreamableStatusResponse>(statusJson);

                    if (response.status > 2)
                    {
                        Errors.Add(response.message);
                        result.IsSuccess = false;
                        break;
                    }
                    else if (response.status == 2)
                    {
                        progress.UpdateProgress(100 - progress.Position);
                        OnProgressChanged(progress);

                        result.IsSuccess = true;

                        if (UseDirectURL && response.files != null && response.files.mp4 != null && !string.IsNullOrEmpty(response.files.mp4.url))
                        {
                            result.URL = URLHelpers.ForcePrefix(response.files.mp4.url);
                        }
                        else
                        {
                            result.URL = URLHelpers.ForcePrefix(response.url);
                        }

                        break;
                    }

                    progress.UpdateProgress(response.percent - progress.Position);
                    OnProgressChanged(progress);

                    Thread.Sleep(1000);
                }
            }
            else
            {
                Errors.Add("Could not create video");
                result.IsSuccess = false;
            }
        }
    }

    public class StreamableTranscodeResponse
    {
        public string Shortcode { get; set; }
        public int Status { get; set; }
    }

    public class StreamableStatusResponse
    {
        public int status { get; set; }
        public StreamableStatusResponseFiles files { get; set; }
        //public string url_root { get; set; }
        public string thumbnail_url { get; set; }
        //public string[] formats { get; set; }
        public string url { get; set; }
        public string message { get; set; }
        public string title { get; set; }
        public long percent { get; set; }
    }

    public class StreamableStatusResponseFiles
    {
        public StreamableStatusResponseVideo mp4 { get; set; }
    }

    public class StreamableStatusResponseVideo
    {
        public int status { get; set; }
        public string url { get; set; }
        public int framerate { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public long bitrate { get; set; }
        public long size { get; set; }
    }
}