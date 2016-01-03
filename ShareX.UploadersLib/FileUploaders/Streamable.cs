#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.UploadersLib.HelperClasses;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

namespace ShareX.UploadersLib.FileUploaders
{
    public class Streamable : FileUploader
    {
        private const string Host = "https://api.streamable.com";

        public string Email { get; private set; }
        public string Password { get; private set; }

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

            UploadResult result = UploadData(stream, URLHelpers.CombineURL(Host, "upload"), fileName, headers: headers);

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

                    if (response.Status > 2)
                    {
                        result.Errors.Add(response.Message);
                        result.IsSuccess = false;
                        break;
                    }
                    else if (response.Status == 2)
                    {
                        if (AllowReportProgress)
                        {
                            long delta = 100 - progress.Position;
                            progress.UpdateProgress(delta);
                            OnProgressChanged(progress);
                        }

                        result.IsSuccess = true;
                        result.URL = URLHelpers.CombineURL("https://streamable.com", transcodeResponse.Shortcode);
                        break;
                    }

                    if (AllowReportProgress)
                    {
                        long delta = response.Percent - progress.Position;
                        progress.UpdateProgress(delta);
                        OnProgressChanged(progress);
                    }

                    Thread.Sleep(100);
                }
            }
            else
            {
                result.Errors.Add("Could not create video");
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
        public int Status { get; set; }
        public string Message { get; set; }
        public long Percent { get; set; }
    }
}