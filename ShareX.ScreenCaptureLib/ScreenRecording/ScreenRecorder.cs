#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Text;
using System.Threading;

namespace ShareX.ScreenCaptureLib
{
    public class ScreenRecorder : IDisposable
    {
        public bool IsRecording { get; private set; }

        public int FPS
        {
            get
            {
                return fps;
            }
            set
            {
                if (!IsRecording)
                {
                    fps = value;
                    UpdateInfo();
                }
            }
        }

        public float DurationSeconds
        {
            get
            {
                return durationSeconds;
            }
            set
            {
                if (!IsRecording)
                {
                    durationSeconds = value;
                    UpdateInfo();
                }
            }
        }

        public Rectangle CaptureRectangle
        {
            get
            {
                return captureRectangle;
            }
            private set
            {
                if (!IsRecording)
                {
                    captureRectangle = value;
                }
            }
        }

        public string CachePath { get; private set; }

        public ScreenRecordOutput OutputType { get; private set; }

        public ScreenRecordingOptions Options { get; set; }

        public event Action RecordingStarted;

        public delegate void ProgressEventHandler(int progress);
        public event ProgressEventHandler EncodingProgressChanged;

        private int fps, delay, frameCount, previousProgress;
        private float durationSeconds;
        private Rectangle captureRectangle;
        private FFmpegCLIManager ffmpeg;

        public ScreenRecorder(ScreenRecordOutput outputType, ScreenRecordingOptions options, Rectangle captureRectangle)
        {
            if (string.IsNullOrEmpty(options.OutputPath))
            {
                throw new Exception("Screen recorder cache path is empty.");
            }

            FPS = options.FPS;
            DurationSeconds = options.Duration;
            CaptureRectangle = captureRectangle;
            CachePath = options.OutputPath;
            OutputType = outputType;

            Options = options;

            FileHelpers.CreateDirectoryFromFilePath(Options.OutputPath);
            ffmpeg = new FFmpegCLIManager(Options.FFmpeg.FFmpegPath);
            ffmpeg.ShowError = true;
            ffmpeg.EncodeStarted += OnRecordingStarted;
            ffmpeg.EncodeProgressChanged += OnEncodingProgressChanged;
        }

        private void UpdateInfo()
        {
            delay = 1000 / fps;
            frameCount = (int)(fps * durationSeconds);
        }

        public void StartRecording()
        {
            if (!IsRecording)
            {
                IsRecording = true;

                ffmpeg.Run(Options.GetFFmpegCommands());
            }

            IsRecording = false;
        }

        public void StopRecording()
        {
            if (ffmpeg != null)
            {
                ffmpeg.Close();
            }
        }

        public bool FFmpegEncodeVideo(string input, string output)
        {
            FileHelpers.CreateDirectoryFromFilePath(output);

            Options.IsRecording = false;
            Options.IsLossless = false;
            Options.InputPath = input;
            Options.OutputPath = output;

            try
            {
                ffmpeg.TrackEncodeProgress = true;

                return ffmpeg.Run(Options.GetFFmpegCommands());
            }
            finally
            {
                ffmpeg.TrackEncodeProgress = false;
            }
        }

        public bool FFmpegEncodeAsGIF(string input, string output)
        {
            FileHelpers.CreateDirectoryFromFilePath(output);

            try
            {
                ffmpeg.TrackEncodeProgress = true;

                StringBuilder args = new StringBuilder();

                args.Append($"-i \"{input}\" ");

                // https://ffmpeg.org/ffmpeg-filters.html#palettegen-1
                args.Append($"-lavfi \"palettegen=stats_mode={Options.FFmpeg.GIFStatsMode}[palette],");

                // https://ffmpeg.org/ffmpeg-filters.html#paletteuse
                args.Append($"[0:v][palette]paletteuse=dither={Options.FFmpeg.GIFDither}");

                if (Options.FFmpeg.GIFDither == FFmpegPaletteUseDither.bayer)
                {
                    args.Append($":bayer_scale={Options.FFmpeg.GIFBayerScale}");
                }

                if (Options.FFmpeg.GIFStatsMode == FFmpegPaletteGenStatsMode.single)
                {
                    args.Append(":new=1");
                }

                args.Append("\" ");
                args.Append("-y ");
                args.Append($"\"{output}\"");

                return ffmpeg.Run(args.ToString());
            }
            finally
            {
                ffmpeg.TrackEncodeProgress = false;
            }
        }

        protected void OnRecordingStarted()
        {
            RecordingStarted?.Invoke();
        }

        protected void OnEncodingProgressChanged(float progress)
        {
            int currentProgress = (int)progress;

            if (EncodingProgressChanged != null && currentProgress != previousProgress)
            {
                EncodingProgressChanged(currentProgress);
                previousProgress = currentProgress;
            }
        }

        public void Dispose()
        {
            if (ffmpeg != null)
            {
                ffmpeg.Dispose();
            }
        }
    }
}
