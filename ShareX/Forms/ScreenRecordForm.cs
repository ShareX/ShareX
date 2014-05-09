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
            if (TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.FFmpegNet && !FFmpegCache.HasDependencies())
            {
                if (MessageBox.Show("FFmpeg files are not present. Would you like to download and install them?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    GitHubUpdateChecker updateChecker = new GitHubUpdateChecker("McoreD", "FFmpegNet");
                    updateChecker.Proxy = ProxyInfo.Current.GetWebProxy();
                    string downloadUrl = updateChecker.GetLatestDownloadURL();
                    if (!string.IsNullOrEmpty(downloadUrl))
                    {
                        UpdateInfo updateInfo = new UpdateInfo() { DownloadURL = downloadUrl };
                        using (UpdaterForm form = new UpdaterForm(updateInfo) { Proxy = ProxyInfo.Current.GetWebProxy() })
                        {
                            form.ShowDialog();
                        }
                    }
                }
                return;
            }

            if (TaskSettings.CaptureSettings.RunScreencastCLI)
            {
                if (!Program.Settings.VideoEncoders.IsValidIndex(TaskSettings.CaptureSettings.VideoEncoderSelected))
                {
                    MessageBox.Show("There is no valid CLI video encoder selected.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (!Program.Settings.VideoEncoders[TaskSettings.CaptureSettings.VideoEncoderSelected].IsValid())
                {
                    MessageBox.Show("CLI video encoder file not exist: " + Program.Settings.VideoEncoders[TaskSettings.CaptureSettings.VideoEncoderSelected].Path,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            SelectRegion();
            Screenshot.CaptureCursor = TaskSettings.CaptureSettings.ShowCursor;

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
                        if (TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.AVI ||
                            TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.FFmpegNet)
                        {
                            path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, "avi"));
                        }
                        else if (TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.FFmpegCLI)
                        {
                            path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, "mp4"));
                        }
                        else
                        {
                            path = Program.ScreenRecorderCacheFilePath;
                        }

                        ScreencastOptions options = new ScreencastOptions()
                        {
                            Size = CaptureRectangle.Size,
                            FPS = TaskSettings.CaptureSettings.ScreenRecordFPS,
                            OutputPath = path,
                            Duration = TaskSettings.CaptureSettings.ScreenRecordFixedDuration ? TaskSettings.CaptureSettings.ScreenRecordDuration : 0,
                            AVI = TaskSettings.CaptureSettings.AVIOptions,
                            FFmpegCLI = TaskSettings.CaptureSettings.FFmpegCLIOptions,
                            FFmpegNet = TaskSettings.CaptureSettings.FFmpegNetOptions
                        };

                        screenRecorder = new ScreenRecorder(options, CaptureRectangle, TaskSettings.CaptureSettings.ScreenRecordOutput);

                        int delay = (int)(TaskSettings.CaptureSettings.ScreenRecordStartDelay * 1000);

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

                        if (TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.GIF)
                        {
                            if (TaskSettings.CaptureSettings.RunScreencastCLI)
                            {
                                sourceFilePath = Path.ChangeExtension(Program.ScreenRecorderCacheFilePath, "gif");
                            }
                            else
                            {
                                sourceFilePath = path = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, "gif"));
                            }
                            screenRecorder.SaveAsGIF(sourceFilePath, TaskSettings.ImageSettings.ImageGIFQuality);
                        }

                        if (TaskSettings.CaptureSettings.RunScreencastCLI)
                        {
                            VideoEncoder encoder = Program.Settings.VideoEncoders[TaskSettings.CaptureSettings.VideoEncoderSelected];
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
                    if (TaskSettings.CaptureSettings.RunScreencastCLI &&
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
                if (TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.UploadImageToHost))
                {
                    UploadManager.UploadFile(path, TaskSettings);
                }
                else
                {
                    if (TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyFilePathToClipboard))
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