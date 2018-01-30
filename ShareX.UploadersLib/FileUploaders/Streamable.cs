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
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using Shell32;
using System;
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

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            string username = "";
            string password = "";

            if (!config.StreamableAnonymous)
            {
                username = config.StreamableUsername;
                password = config.StreamablePassword;
            }

            return new Streamable(username, password)
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
                headers = CreateAuthenticationHeader(Email, Password);
            }
            UploadResult result;
            TimeSpan duration;
            using (FileStream fs = stream as FileStream)
            {
                GetDuration(fs.Name, out duration);
            }
            if (stream.Length > 1073741824 || duration > TimeSpan.FromMinutes(10))
            {
                result = new UploadResult
                {
                    IsSuccess = false,
                    Response = "There is a 10 minute limit on video duration and a maximum file size of 1GB for direct file uploads."
                };
                Errors.Add("Video size or duration over Streamable limit(1GB and 10 minutes)");
                return result;
            }
            result = SendRequestFile(URLHelpers.CombineURL(Host, "upload"), stream, fileName, headers: headers);

            TranscodeFile(result);

            return result;
        }

        private void TranscodeFile(UploadResult result)
        {
            StreamableTranscodeResponse transcodeResponse = JsonConvert.DeserializeObject<StreamableTranscodeResponse>(result.Response);

            if (!string.IsNullOrEmpty(transcodeResponse.Shortcode))
            {
                ProgressManager progress = new ProgressManager(100);

                if (AllowReportProgress)
                {
                    OnProgressChanged(progress);
                }

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
                        if (AllowReportProgress)
                        {
                            long delta = 100 - progress.Position;
                            progress.UpdateProgress(delta);
                            OnProgressChanged(progress);
                        }

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

                    if (AllowReportProgress)
                    {
                        long delta = response.percent - progress.Position;
                        progress.UpdateProgress(delta);
                        OnProgressChanged(progress);
                    }

                    Thread.Sleep(1000);
                }
            }
            else
            {
                Errors.Add("Could not create video");
                result.IsSuccess = false;
            }
        }
        private bool GetDuration(string filename, out TimeSpan duration)
        {
            try
            {
                var shl = new Shell();
                var fldr = shl.NameSpace(Path.GetDirectoryName(filename));
                var itm = fldr.ParseName(Path.GetFileName(filename));

                // Index 27 is the video duration [This may not always be the case]
                var propValue = fldr.GetDetailsOf(itm, 27);

                return TimeSpan.TryParse(propValue, out duration);
            }
            catch (Exception)
            {
                duration = new TimeSpan();
                return false;
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