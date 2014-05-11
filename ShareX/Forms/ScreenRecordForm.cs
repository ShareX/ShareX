#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using ScreenCaptureLib;
using ShareX.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public class ScreenRecordForm : TrayForm
    {
        public Rectangle CaptureRectangle { get; private set; }
        public bool IsRecording { get; private set; }

        private static ScreenRecordForm instance;

        public static ScreenRecordForm Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new ScreenRecordForm();
                    instance.Show();
                }

                return instance;
            }
        }

        private ScreenRecorder screenRecorder;

        private ScreenRecordForm()
        {
            TrayIcon.Text = "ShareX - Screen recording";
            TrayIcon.MouseClick += TrayIcon_MouseClick;
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StopRecording();
            }
        }

        private void SelectRegion()
        {
            Rectangle rect;
            if (TaskHelpers.SelectRegion(out rect) && !rect.IsEmpty)
            {
                CaptureRectangle = CaptureHelpers.EvenRectangleSize(rect);
            }
        }

        public async void StartRecording(TaskSettings TaskSettings)
        {
            if (TaskSettings.SafeCaptureSettings.RunScreencastCLI)
            {
                if (!Program.Settings.VideoEncoders.IsValidIndex(TaskSettings.SafeCaptureSettings.VideoEncoderSelected))
                {
                    MessageBox.Show("There is no valid CLI video encoder selected.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (!Program.Settings.VideoEncoders[TaskSettings.SafeCaptureSettings.VideoEncoderSelected].IsValid())
                {
                    MessageBox.Show("CLI video encoder file does not exist: " + Program.Settings.VideoEncoders[TaskSettings.SafeCaptureSettings.VideoEncoderSelected].Path,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (TaskSettings.SafeCaptureSettings.ScreenRecordOutput == ScreenRecordOutput.FFmpeg && !File.Exists(TaskSettings.SafeCaptureSettings.FFmpegOptions.CLIPath))
            {
                if (MessageBox.Show(TaskSettings.SafeCaptureSettings.FFmpegOptions.CLIPath + " does not exist." + Environment.NewLine + Environment.NewLine + "Would you like to automatically download it?",
                    Application.ProductName + " - Missing ffmpeg.exe", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    TaskSettings.SafeCaptureSettings.FFmpegOptions.CLIPath = Path.Combine(Program.ToolsFolder, "ffmpeg.exe");

                    using (FFmpegOptionsForm form = new FFmpegOptionsForm(TaskSettings.SafeCaptureSettings.FFmpegOptions))
                    {
                        if (form.DownloadFFmpeg(false) == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            SelectRegion();
            Screenshot.CaptureCursor = TaskSettings.SafeCaptureSettings.ShowCursor;

            if (IsRecording || CaptureRectangle.IsEmpty || screenRecorder != null)
            {
                return;
            }

            IsRecording = true;

            TrayIcon.Icon = Resources.control_record_yellow.ToIcon();
            TrayIcon.Visible = true;

            string path = "";

            try
            {
                using (ScreenRegionManager screenRegionManager = new ScreenRegionManager())
                {
                    screenRegionManager.Start(CaptureRectangle);

                    await TaskEx.Run(() =>
                    {
                        if (TaskSettings.SafeCaptureSettings.ScreenRecordOutput == ScreenRecordOutput.AVI)
                        {
                            path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, "avi"));
                        }
                        else if (TaskSettings.SafeCaptureSettings.ScreenRecordOutput == ScreenRecordOutput.FFmpeg)
                        {
                            path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, TaskSettings.SafeCaptureSettings.FFmpegOptions.Extension));
                        }
                        else
                        {
                            path = Program.ScreenRecorderCacheFilePath;
                        }

                        ScreencastOptions options = new ScreencastOptions()
                        {
                            CaptureArea = CaptureRectangle,
                            FPS = TaskSettings.SafeCaptureSettings.ScreenRecordFPS,
                            OutputPath = path,
                            Duration = TaskSettings.SafeCaptureSettings.ScreenRecordFixedDuration ? TaskSettings.SafeCaptureSettings.ScreenRecordDuration : 0,
                            AVI = TaskSettings.SafeCaptureSettings.AVIOptions,
                            FFmpeg = TaskSettings.SafeCaptureSettings.FFmpegOptions,
                            DrawCursor = TaskSettings.SafeCaptureSettings.ShowCursor
                        };

                        screenRecorder = new ScreenRecorder(options, CaptureRectangle, TaskSettings.SafeCaptureSettings.ScreenRecordOutput);

                        int delay = (int)(TaskSettings.SafeCaptureSettings.ScreenRecordStartDelay * 1000);

                        if (delay > 0)
                        {
                            Thread.Sleep(delay);
                        }

                        screenRegionManager.ChangeColor();

                        this.InvokeSafe(() => TrayIcon.Icon = Resources.control_record.ToIcon());

                        screenRecorder.StartRecording();
                    });
                }

                if (screenRecorder != null)
                {
                    TrayIcon.Icon = Resources.camcorder__pencil.ToIcon();

                    await TaskEx.Run(() =>
                    {
                        string sourceFilePath = path;

                        if (TaskSettings.SafeCaptureSettings.ScreenRecordOutput == ScreenRecordOutput.GIF)
                        {
                            if (TaskSettings.SafeCaptureSettings.RunScreencastCLI)
                            {
                                sourceFilePath = Path.ChangeExtension(Program.ScreenRecorderCacheFilePath, "gif");
                            }
                            else
                            {
                                sourceFilePath = path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, "gif"));
                            }
                            screenRecorder.SaveAsGIF(sourceFilePath, TaskSettings.SafeImageSettings.ImageGIFQuality);
                        }

                        if (TaskSettings.SafeCaptureSettings.RunScreencastCLI)
                        {
                            VideoEncoder encoder = Program.Settings.VideoEncoders[TaskSettings.SafeCaptureSettings.VideoEncoderSelected];
                            path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, encoder.OutputExtension));
                            screenRecorder.EncodeUsingCommandLine(encoder, sourceFilePath, path);
                        }
                    });
                }
            }
            finally
            {
                if (screenRecorder != null)
                {
                    if (TaskSettings.SafeCaptureSettings.RunScreencastCLI &&
                        !string.IsNullOrEmpty(screenRecorder.CachePath) && File.Exists(screenRecorder.CachePath))
                    {
                        File.Delete(screenRecorder.CachePath);
                    }

                    screenRecorder.Dispose();
                    screenRecorder = null;
                }

                if (TrayIcon.Visible)
                {
                    TrayIcon.Visible = false;
                }
            }

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                if (TaskSettings.SafeAfterTasks.AfterCaptureJob.HasFlag(AfterCaptureTasks.UploadImageToHost))
                {
                    UploadManager.UploadFile(path, TaskSettings);
                }
                else
                {
                    if (TaskSettings.SafeAfterTasks.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyFilePathToClipboard))
                    {
                        ClipboardHelpers.CopyText(path);
                    }

                    TaskHelpers.ShowResultNotifications(path, TaskSettings, path);
                }
            }

            IsRecording = false;
        }

        public void StopRecording()
        {
            if (IsRecording && screenRecorder != null)
            {
                screenRecorder.StopRecording();
            }
        }
    }
}