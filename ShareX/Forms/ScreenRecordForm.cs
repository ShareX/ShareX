#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public class ScreenRecordForm : TrayForm
    {
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
        private ScreenRegionForm regionForm;
        private DWMManager dwmManager;
        private bool abortRequested;

        private ScreenRecordForm()
        {
            TrayIcon.Text = "ShareX";
            TrayIcon.MouseClick += TrayIcon_MouseClick;
        }

        public void StartStopRecording()
        {
            if (regionForm != null && !regionForm.IsDisposed)
            {
                regionForm.StartStop();
            }
        }

        private void StopRecording()
        {
            if (IsRecording && screenRecorder != null)
            {
                screenRecorder.StopRecording();
            }
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StartStopRecording();
            }
        }

        public void StartRecording(ScreenRecordOutput outputType, TaskSettings taskSettings, bool skipRegionSelection = false)
        {
            if (taskSettings.CaptureSettings.RunScreencastCLI)
            {
                if (!Program.Settings.VideoEncoders.IsValidIndex(taskSettings.CaptureSettings.VideoEncoderSelected))
                {
                    MessageBox.Show(Resources.ScreenRecordForm_StartRecording_There_is_no_valid_CLI_video_encoder_selected_,
                        "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!Program.Settings.VideoEncoders[taskSettings.CaptureSettings.VideoEncoderSelected].IsValid())
                {
                    MessageBox.Show(Resources.ScreenRecordForm_StartRecording_CLI_video_encoder_file_does_not_exist__ +
                        Program.Settings.VideoEncoders[taskSettings.CaptureSettings.VideoEncoderSelected].Path,
                        "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (outputType == ScreenRecordOutput.FFmpeg)
            {
                if (!File.Exists(taskSettings.CaptureSettings.FFmpegOptions.CLIPath))
                {
                    string ffmpegText = string.IsNullOrEmpty(taskSettings.CaptureSettings.FFmpegOptions.CLIPath) ? "ffmpeg.exe" : taskSettings.CaptureSettings.FFmpegOptions.CLIPath;

                    if (MessageBox.Show(string.Format(Resources.ScreenRecordForm_StartRecording_does_not_exist, ffmpegText),
                        "ShareX - " + Resources.ScreenRecordForm_StartRecording_Missing + " ffmpeg.exe", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        if (FFmpegDownloader.DownloadFFmpeg(false, DownloaderForm_InstallRequested) == DialogResult.OK)
                        {
                            Program.DefaultTaskSettings.CaptureSettings.FFmpegOptions.CLIPath = taskSettings.TaskSettingsReference.CaptureSettings.FFmpegOptions.CLIPath =
                               taskSettings.CaptureSettings.FFmpegOptions.CLIPath = Path.Combine(Program.ToolsFolder, "ffmpeg.exe");
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (!taskSettings.CaptureSettings.FFmpegOptions.IsSourceSelected)
                {
                    MessageBox.Show(Resources.ScreenRecordForm_StartRecording_FFmpeg_video_and_audio_source_both_can_t_be__None__,
                        "ShareX - " + Resources.ScreenRecordForm_StartRecording_FFmpeg_error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            Rectangle captureRectangle;

            if (skipRegionSelection)
            {
                captureRectangle = Program.Settings.ScreenRecordRegion;
            }
            else
            {
                TaskHelpers.SelectRegion(out captureRectangle, taskSettings);
                captureRectangle = CaptureHelpers.EvenRectangleSize(captureRectangle);
            }

            if (IsRecording || !captureRectangle.IsValid() || screenRecorder != null)
            {
                return;
            }

            Program.Settings.ScreenRecordRegion = captureRectangle;

            IsRecording = true;
            Screenshot.CaptureCursor = taskSettings.CaptureSettings.ShowCursor;

            string trayText = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Waiting___;
            TrayIcon.Text = trayText.Truncate(63);
            TrayIcon.Icon = Resources.control_record_yellow.ToIcon();
            TrayIcon.Visible = true;

            string path = "";

            float duration = taskSettings.CaptureSettings.ScreenRecordFixedDuration ? taskSettings.CaptureSettings.ScreenRecordDuration : 0;

            regionForm = ScreenRegionForm.Show(captureRectangle, StopRecording, duration);
            regionForm.RecordResetEvent = new ManualResetEvent(false);

            TaskEx.Run(() =>
            {
                try
                {
                    if (taskSettings.CaptureSettings.ScreenRecordAutoDisableAero)
                    {
                        dwmManager = new DWMManager();
                        dwmManager.AutoDisable();
                    }

                    if (outputType == ScreenRecordOutput.FFmpeg)
                    {
                        path = Path.Combine(taskSettings.CaptureFolder, TaskHelpers.GetFilename(taskSettings, taskSettings.CaptureSettings.FFmpegOptions.Extension));
                    }
                    else
                    {
                        path = Program.ScreenRecorderCacheFilePath;
                    }

                    ScreencastOptions options = new ScreencastOptions()
                    {
                        FFmpeg = taskSettings.CaptureSettings.FFmpegOptions,
                        ScreenRecordFPS = taskSettings.CaptureSettings.ScreenRecordFPS,
                        GIFFPS = taskSettings.CaptureSettings.GIFFPS,
                        Duration = duration,
                        OutputPath = path,
                        CaptureArea = captureRectangle,
                        DrawCursor = taskSettings.CaptureSettings.ShowCursor
                    };

                    screenRecorder = new ScreenRecorder(outputType, options, captureRectangle);

                    if (regionForm != null && regionForm.RecordResetEvent != null)
                    {
                        trayText = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Click_tray_icon_to_start_recording_;
                        TrayIcon.Text = trayText.Truncate(63);

                        if (taskSettings.CaptureSettings.ScreenRecordAutoStart)
                        {
                            int delay = (int)(taskSettings.CaptureSettings.ScreenRecordStartDelay * 1000);

                            if (delay > 0)
                            {
                                regionForm.RecordResetEvent.WaitOne(delay);
                            }
                        }
                        else
                        {
                            regionForm.RecordResetEvent.WaitOne();
                        }

                        if (regionForm.AbortRequested)
                        {
                            abortRequested = true;
                        }
                    }

                    if (!abortRequested)
                    {
                        trayText = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Click_tray_icon_to_stop_recording_;
                        TrayIcon.Text = trayText.Truncate(63);
                        TrayIcon.Icon = Resources.control_record.ToIcon();

                        if (regionForm != null)
                        {
                            this.InvokeSafe(() => regionForm.StartTimer());
                        }

                        screenRecorder.StartRecording();

                        if (regionForm != null && regionForm.AbortRequested)
                        {
                            abortRequested = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
                finally
                {
                    if (dwmManager != null)
                    {
                        dwmManager.Dispose();
                        dwmManager = null;
                    }

                    if (regionForm != null)
                    {
                        if (regionForm.RecordResetEvent != null)
                        {
                            regionForm.RecordResetEvent.Dispose();
                        }

                        this.InvokeSafe(() => regionForm.Close());
                        regionForm = null;
                    }
                }

                try
                {
                    if (!abortRequested && screenRecorder != null)
                    {
                        TrayIcon.Text = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Encoding___;
                        TrayIcon.Icon = Resources.camcorder_pencil.ToIcon();

                        string sourceFilePath = path;

                        if (outputType == ScreenRecordOutput.GIF)
                        {
                            if (taskSettings.CaptureSettings.RunScreencastCLI)
                            {
                                sourceFilePath = Path.ChangeExtension(Program.ScreenRecorderCacheFilePath, "gif");
                            }
                            else
                            {
                                sourceFilePath = path = Path.Combine(taskSettings.CaptureFolder, TaskHelpers.GetFilename(taskSettings, "gif"));
                            }

                            Helpers.CreateDirectoryIfNotExist(sourceFilePath);
                            screenRecorder.EncodingProgressChanged += progress =>
                            {
                                TrayIcon.Text = string.Format("ShareX - {0} ({1}%)", Resources.ScreenRecordForm_StartRecording_Encoding___, progress);
                            };
                            screenRecorder.SaveAsGIF(sourceFilePath, taskSettings.ImageSettings.ImageGIFQuality);
                        }

                        if (taskSettings.CaptureSettings.RunScreencastCLI)
                        {
                            VideoEncoder encoder = Program.Settings.VideoEncoders[taskSettings.CaptureSettings.VideoEncoderSelected];
                            path = Path.Combine(taskSettings.CaptureFolder, TaskHelpers.GetFilename(taskSettings, encoder.OutputExtension));
                            screenRecorder.EncodeUsingCommandLine(encoder, sourceFilePath, path);
                        }
                    }
                }
                finally
                {
                    if (screenRecorder != null)
                    {
                        if (taskSettings.CaptureSettings.RunScreencastCLI && !string.IsNullOrEmpty(screenRecorder.CachePath) && File.Exists(screenRecorder.CachePath))
                        {
                            File.Delete(screenRecorder.CachePath);
                        }

                        screenRecorder.Dispose();
                        screenRecorder = null;

                        if (abortRequested && !string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
            },
            () =>
            {
                if (TrayIcon.Visible)
                {
                    TrayIcon.Visible = false;
                }

                if (!abortRequested && !string.IsNullOrEmpty(path) && File.Exists(path) && TaskHelpers.ShowAfterCaptureForm(taskSettings))
                {
                    UploadTask task = UploadTask.CreateFileJobTask(path, taskSettings);
                    TaskManager.Start(task);
                }

                abortRequested = false;
                IsRecording = false;
            });
        }

        private void DownloaderForm_InstallRequested(string filePath)
        {
            string extractPath = Path.Combine(Program.ToolsFolder, "ffmpeg.exe");
            bool result = FFmpegDownloader.ExtractFFmpeg(filePath, extractPath);

            if (result)
            {
                MessageBox.Show(Resources.ScreenRecordForm_DownloaderForm_InstallRequested_FFmpeg_successfully_downloaded_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.ScreenRecordForm_DownloaderForm_InstallRequested_Download_of_FFmpeg_failed_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}