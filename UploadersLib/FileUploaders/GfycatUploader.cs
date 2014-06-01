#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.ComponentModel;
using System.IO;
using UploadersLib.HelperClasses;

namespace UploadersLib.FileUploaders
{
    public class GfycatUploader : FileUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            // Magical official values from http://www.reddit.com/r/gfycat/comments/20xbth/any_word_on_allowing_uploading_a_gif_through_the/
            args.Add("key", Helpers.GetRandomAlphanumeric(10));
            args.Add("acl", "private");
            args.Add("AWSAccessKeyId", "AKIAIT4VU4B7G2LQYKZQ");
            args.Add("policy", "eyAiZXhwaXJhdGlvbiI6ICIyMDIwLTEyLTAxVDEyOjAwOjAwLjAwMFoiLAogICAgICAgICAgICAiY29uZGl0aW9ucyI6IFsKICAgICAgICAgICAgeyJidWNrZXQiOiAiZ2lmYWZmZSJ9LAogICAgICAgICAgICBbInN0YXJ0cy13aXRoIiwgIiRrZXkiLCAiIl0sCiAgICAgICAgICAgIHsiYWNsIjogInByaXZhdGUifSwKCSAgICB7InN1Y2Nlc3NfYWN0aW9uX3N0YXR1cyI6ICIyMDAifSwKICAgICAgICAgICAgWyJzdGFydHMtd2l0aCIsICIkQ29udGVudC1UeXBlIiwgIiJdLAogICAgICAgICAgICBbImNvbnRlbnQtbGVuZ3RoLXJhbmdlIiwgMCwgNTI0Mjg4MDAwXQogICAgICAgICAgICBdCiAgICAgICAgICB9");
            args.Add("success_action_status", "200");
            args.Add("signature", "mk9t/U/wRN4/uU01mXfeTe2Kcoc=");
            args.Add("Content-Type", MimeTypes.GetMimeType(Path.GetExtension(fileName).ToLower()));
            UploadResult result = UploadData(stream, "https://gifaffe.s3.amazonaws.com/", fileName, "file", args);

            if (!result.IsError)
            {
                TranscodeFile(args["key"], result);
            }

            return result;
        }

        private void TranscodeFile(string key, UploadResult result)
        {
            string transcodeJson = SendRequest(HttpMethod.GET, "https://upload.gfycat.com/transcodeRelease/" + key + "?noResize=true");
            GfycatTranscodeResponse transcodeResponse = JsonConvert.DeserializeObject<GfycatTranscodeResponse>(transcodeJson);

            if (transcodeResponse.IsOk)
            {
                ProgressManager progress = new ProgressManager(10000);
                if (AllowReportProgress)
                {
                    OnProgressChanged(progress);
                }

                while (!stopUpload)
                {
                    string statusJson = SendRequest(HttpMethod.GET, "https://upload.gfycat.com/status/" + key);
                    GfycatStatusResponse response = JsonConvert.DeserializeObject<GfycatStatusResponse>(statusJson);
                    if (response.Error != null)
                    {
                        result.Errors.Add(response.Error);
                        result.IsSuccess = false;
                        break;
                    } else if (response.GfyName != null) {
                        result.IsSuccess = true;
                        result.URL = "https://gfycat.com/" + response.GfyName;
                        break;
                    }
                    else
                    {
                        if (AllowReportProgress && progress.UpdateProgress((progress.Length - progress.Position) / response.Time))
                        {
                            OnProgressChanged(progress);
                        }
                    }
                }
            }
            else
            {
                result.Errors.Add(transcodeResponse.Error);
                result.IsSuccess = false;
            }
        }
    }

    public class GfycatTranscodeResponse
    {
        public bool IsOk { get; set; }
        public string Error { get; set; }
    }

    public class GfycatStatusResponse
    {
        public string Task { get; set; }
        public int Time { get; set; }
        public string GfyName { get; set; }
        public string Error { get; set; }
    }
}