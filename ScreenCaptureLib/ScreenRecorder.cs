#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

        public delegate void ProgressEventHandler(int progress);

        public event ProgressEventHandler EncodingProgressChanged;

        private int fps, delay, frameCount;
        private float durationSeconds;
        private Rectangle captureRectangle;
        private HardDiskCache hdCache;
        private AVICache aviCache;
        private bool stopRequest;

        public ScreenRecorder(int fps, float durationSeconds, Rectangle captureRectangle, string cachePath, ScreenRecordOutput outputType)
        {
            if (string.IsNullOrEmpty(cachePath))
            {
                throw new Exception("Screen recorder cache path is empty.");
            }

            FPS = fps;
            DurationSeconds = durationSeconds;
            CaptureRectangle = captureRectangle;
            CachePath = cachePath;
            OutputType = outputType;

            if (OutputType == ScreenRecordOutput.AVI || OutputType == ScreenRecordOutput.AVICommandLine)
            {
                bool showOptions = OutputType == ScreenRecordOutput.AVI;
                aviCache = new AVICache(CachePath, FPS, CaptureRectangle.Size, showOptions);
            }
            else if (OutputType == ScreenRecordOutput.GIF)
            {
                hdCache = new HardDiskCache(CachePath);
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

                for (int i = 0; (frameCount == 0 && !stopRequest) || i < frameCount; i++)
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    Image img = Screenshot.CaptureRectangle(CaptureRectangle);

                    if (OutputType == ScreenRecordOutput.AVI || OutputType == ScreenRecordOutput.AVICommandLine)
                    {
                        aviCache.AddImageAsync(img);
                    }
                    else if (OutputType == ScreenRecordOutput.GIF)
                    {
                        hdCache.AddImageAsync(img);
                    }

                    if ((frameCount == 0 && !stopRequest) || (i + 1 < frameCount))
                    {
                        int sleepTime = delay - (int)timer.ElapsedMilliseconds;

                        if (sleepTime > 0)
                        {
                            Thread.Sleep(sleepTime);
                        }
                        else
                        {
                            //Debug.WriteLine("FPS drop: " + sleepTime);
                        }
                    }
                }

                if (OutputType == ScreenRecordOutput.AVI || OutputType == ScreenRecordOutput.AVICommandLine)
                {
                    aviCache.Finish();
                }
                else if (OutputType == ScreenRecordOutput.GIF)
                {
                    hdCache.Finish();
                }
            }

            IsRecording = false;
        }

        public void StopRecording()
        {
            stopRequest = true;
        }

        public void SaveAsGIF(string path, GIFQuality quality)
        {
            if (!IsRecording)
            {
                using (GifCreator gifEncoder = new GifCreator(delay))
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

                    gifEncoder.Finish();
                    gifEncoder.Save(path);
                }
            }
        }

        public void EncodeUsingCommandLine(string output, string encoderPath, string encoderArguments)
        {
            if (!string.IsNullOrEmpty(CachePath) && File.Exists(CachePath) && !string.IsNullOrEmpty(encoderPath) && File.Exists(encoderPath))
            {
                OnEncodingProgressChanged(-1);
                Helpers.CreateDirectoryIfNotExist(output);

                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo(encoderPath);
                    encoderArguments = encoderArguments.Replace("%input", "\"" + CachePath + "\"").Replace("%output", "\"" + output + "\"");
                    psi.Arguments = encoderArguments;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }

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
            if (hdCache != null)
            {
                hdCache.Dispose();
            }

            if (aviCache != null)
            {
                aviCache.Dispose();
            }
        }
    }
}