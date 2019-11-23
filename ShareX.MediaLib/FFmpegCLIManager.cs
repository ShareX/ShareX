#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareX.MediaLib
{
    public class FFmpegCLIManager : ExternalCLIManager
    {
        public delegate void EncodeProgressChangedEventHandler(float percentage);
        public event EncodeProgressChangedEventHandler EncodeProgressChanged;

        public string FFmpegPath { get; private set; }
        public StringBuilder Output { get; private set; }
        public bool ShowError { get; set; }
        public bool TrackEncodeProgress { get; set; }
        public TimeSpan VideoDuration { get; set; }
        public TimeSpan EncodeTime { get; set; }
        public float EncodePercentage { get; set; }

        public FFmpegCLIManager(string ffmpegPath)
        {
            FFmpegPath = ffmpegPath;
            Output = new StringBuilder();
            OutputDataReceived += FFmpeg_DataReceived;
            ErrorDataReceived += FFmpeg_DataReceived;
            Helpers.CreateDirectoryFromFilePath(FFmpegPath);
        }

        public bool Run(string args)
        {
            return Run(FFmpegPath, args);
        }

        private bool Run(string path, string args)
        {
            int errorCode = Open(path, args);
            bool result = errorCode == 0;
            if (!result && ShowError)
            {
                // TODO: Translate
                new OutputBox(Output.ToString(), "FFmpeg error").ShowDialog();
            }
            return result;
        }

        private void FFmpeg_DataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (this)
            {
                string data = e.Data;

                if (!string.IsNullOrEmpty(data))
                {
                    Output.AppendLine(data);

                    if (TrackEncodeProgress)
                    {
                        UpdateEncodeProgress(data);
                    }
                }
            }
        }

        private void UpdateEncodeProgress(string data)
        {
            if (VideoDuration.Ticks == 0)
            {
                //  Duration: 00:00:15.32, start: 0.000000, bitrate: 1095 kb/s
                Match match = Regex.Match(data, @"Duration:\s*(\d+:\d+:\d+\.\d+),\s*start:", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    TimeSpan duration;

                    if (TimeSpan.TryParse(match.Groups[1].Value, out duration))
                    {
                        VideoDuration = duration;
                    }
                }
            }
            else
            {
                //frame=  942 fps=187 q=35.0 size=    3072kB time=00:00:38.10 bitrate= 660.5kbits/s speed=7.55x
                Match match = Regex.Match(data, @"time=\s*(\d+:\d+:\d+\.\d+)\s*bitrate=", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    TimeSpan time;

                    if (TimeSpan.TryParse(match.Groups[1].Value, out time))
                    {
                        EncodeTime = time;
                        EncodePercentage = ((float)EncodeTime.Ticks / VideoDuration.Ticks) * 100;

                        OnEncodeProgressChanged(EncodePercentage);
                    }
                }
            }
        }

        protected void OnEncodeProgressChanged(float percentage)
        {
            EncodeProgressChanged?.Invoke(percentage);
        }

        public VideoInfo GetVideoInfo(string videoPath)
        {
            VideoInfo videoInfo = new VideoInfo();
            videoInfo.FilePath = videoPath;

            if (Run($"-i \"{videoPath}\""))
            {
                string output = Output.ToString();

                Match matchInput = Regex.Match(output, @"Duration: (?<Duration>\d{2}:\d{2}:\d{2}\.\d{2}),.+?start: (?<Start>\d+\.\d+),.+?bitrate: (?<Bitrate>\d+) kb/s",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (matchInput.Success)
                {
                    videoInfo.Duration = TimeSpan.Parse(matchInput.Groups["Duration"].Value);
                    //videoInfo.Start = TimeSpan.Parse(match.Groups["Start"].Value);
                    videoInfo.Bitrate = int.Parse(matchInput.Groups["Bitrate"].Value);
                }
                else
                {
                    return null;
                }

                Match matchVideoStream = Regex.Match(output, @"Stream #\d+:\d+(?:\(.+?\))?: Video: (?<Codec>.+?) \(.+?,.+?, (?<Width>\d+)x(?<Height>\d+).+?, (?<FPS>\d+(?:\.\d+)?) fps",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (matchVideoStream.Success)
                {
                    videoInfo.VideoCodec = matchVideoStream.Groups["Codec"].Value;
                    videoInfo.VideoResolution = new Size(int.Parse(matchVideoStream.Groups["Width"].Value), int.Parse(matchVideoStream.Groups["Height"].Value));
                    videoInfo.VideoFPS = double.Parse(matchVideoStream.Groups["FPS"].Value, CultureInfo.InvariantCulture);
                }

                Match matchAudioStream = Regex.Match(output, @"Stream #\d+:\d+(?:\(.+?\))?: Audio: (?<Codec>.+?)(?: \(|,)",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (matchAudioStream.Success)
                {
                    videoInfo.AudioCodec = matchAudioStream.Groups["Codec"].Value;
                }
            }

            return videoInfo;
        }
    }
}