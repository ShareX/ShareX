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
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace ScreenCaptureLib
{
    public class ScreenRecorder : IDisposable
    {
        public bool IsRecording { get; private set; }
        public bool WriteCompressed { get; set; }

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

        public ScreencastOptions Options { get; set; }

        public delegate void ProgressEventHandler(int progress);

        public event ProgressEventHandler EncodingProgressChanged;

        private int fps, delay, frameCount;
        private float durationSeconds;
        private Rectangle captureRectangle;
        private ImageCache imgCache;
        private FFmpegHelper ffMpegCli;
        private bool stopRequest;

        public ScreenRecorder(ScreencastOptions options, Rectangle captureRectangle, ScreenRecordOutput outputType)
        {
            if (string.IsNullOrEmpty(options.OutputPath))
            {
                throw new Exception("Screen recorder cache path is empty.");
            }

            FPS = outputType == ScreenRecordOutput.GIF ? options.GIFFPS : options.ScreenRecordFPS;
            DurationSeconds = options.Duration;
            CaptureRectangle = captureRectangle;
            CachePath = options.OutputPath;
            OutputType = outputType;

            Options = options;

            switch (OutputType)
            {
                case ScreenRecordOutput.AVI:
                    imgCache = new AVICache(Options);
                    break;
                case ScreenRecordOutput.FFmpeg:
                    ffMpegCli = new FFmpegHelper(Options);
                    break;
                case ScreenRecordOutput.GIF:
                    imgCache = new HardDiskCache(Options);
                    break;
            }
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
                stopRequest = false;

                if (OutputType == ScreenRecordOutput.FFmpeg)
                {
                    ffMpegCli.Record();
                }
                else
                {
                    RecordUsingCache();
                }
            }

            IsRecording = false;
        }

        private void RecordUsingCache()
        {
            try
            {
                for (int i = 0; !stopRequest && (frameCount == 0 || i < frameCount); i++)
                {
                    Stopwatch timer = Stopwatch.StartNew();

                    Image img = Screenshot.CaptureRectangle(CaptureRectangle);
                    //DebugHelper.WriteLine("Screen capture: " + (int)timer.ElapsedMilliseconds);

                    imgCache.AddImageAsync(img);

                    if (!stopRequest && (frameCount == 0 || i + 1 < frameCount))
                    {
                        int sleepTime = delay - (int)timer.ElapsedMilliseconds;

                        if (sleepTime > 0)
                        {
                            Thread.Sleep(sleepTime);
                        }
                        else if (sleepTime < 0)
                        {
                            // Need to handle FPS drops
                        }
                    }
                }
            }
            finally
            {
                imgCache.Finish();
            }
        }

        public void StopRecording()
        {
            stopRequest = true;

            if (ffMpegCli != null)
            {
                ffMpegCli.Close();
            }
        }

        public void SaveAsGIF(string path, GIFQuality quality)
        {
            if (imgCache != null && imgCache is HardDiskCache && !IsRecording)
            {
                HardDiskCache hdCache = imgCache as HardDiskCache;

                using (AnimatedGifCreator gifEncoder = new AnimatedGifCreator(path, delay))
                {
                    int i = 0;
                    int count = hdCache.Count;

                    foreach (Image img in hdCache.GetImageEnumerator())
                    {
                        i++;
                        OnEncodingProgressChanged((int)((float)i / count * 100));

                        using (img)
                        {
                            gifEncoder.AddFrame(img, quality);
                        }
                    }
                }
            }
        }

        public void EncodeUsingCommandLine(VideoEncoder encoder, string sourceFilePath, string targetFilePath)
        {
            if (!string.IsNullOrEmpty(sourceFilePath) && File.Exists(sourceFilePath))
            {
                OnEncodingProgressChanged(-1);
                encoder.Encode(sourceFilePath, targetFilePath);
                OnEncodingProgressChanged(100);
            }
        }

        protected void OnEncodingProgressChanged(int progress)
        {
            if (EncodingProgressChanged != null)
            {
                EncodingProgressChanged(progress);
            }
        }

        public void Dispose()
        {
            if (imgCache != null)
            {
                imgCache.Dispose();
            }
        }
    }
}