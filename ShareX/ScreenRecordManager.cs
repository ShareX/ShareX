#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System.Windows.Forms;

namespace ShareX
{
    public static class ScreenRecordManager
    {

        public static bool IsRecording { get; private set; }

        private static ScreenRecorder screenRecorder;
        private static ScreenRecordForm recordForm;
        private static TaskSettings taskSettings;
        private static ScreenRecordOutput outputType;
        private static ScreenRecordStartMethod startMethod;
        private static Rectangle captureRectangle;
        private static string path;
        private static bool abortRequested;
        private static float duration;

        public static void StartStopRecording(ScreenRecordOutput outputType, ScreenRecordStartMethod startMethod, TaskSettings taskSettings)
        {
            if (IsRecording)
            {
                if (recordForm != null && !recordForm.IsDisposed)
                {
                    recordForm.StartStopRecording();
                }
            }
            else
            {
                StartRecording(outputType, taskSettings, startMethod);
            }
        }

        private static void StopRecording()
        {
            if (IsRecording && screenRecorder != null)
            {
                screenRecorder.StopRecording();
            }
        }

        private static void StartRecording(ScreenRecordOutput recordOutputType, TaskSettings recordTaskSettings, ScreenRecordStartMethod recordStartMethod = ScreenRecordStartMethod.Region)
        {
            taskSettings = recordTaskSettings;
            outputType = recordOutputType;
            startMethod = recordStartMethod;

            showDebugMessage();

            if (!InitOutputType()) {
                return;
            }

            captureRectangle = GetCaptureStartMethod();
            captureRectangle = CaptureHelpers.EvenRectangleSize(captureRectangle);

            if (IsRecording || !captureRectangle.IsValid() || screenRecorder != null)
            {
                return;
            }

            PerformRecording();
        }

        private static void showDebugMessage()
        {
            string debugText;

            if (outputType == ScreenRecordOutput.FFmpeg)
            {
                debugText = string.Format("Starting FFmpeg recording. Video encoder: \"{0}\", Audio encoder: \"{1}\", FPS: {2}",
                    taskSettings.CaptureSettings.FFmpegOptions.VideoCodec.GetDescription(), taskSettings.CaptureSettings.FFmpegOptions.AudioCodec.GetDescription(),
                    taskSettings.CaptureSettings.ScreenRecordFPS);
            }
            else
            {
                debugText = string.Format("Starting Animated GIF recording. GIF encoding: \"{0}\", FPS: {1}",
                    taskSettings.CaptureSettings.GIFEncoding.GetDescription(), taskSettings.CaptureSettings.GIFFPS);
            }

            DebugHelper.WriteLine(debugText);
        }

        private static bool InitOutputType()
        {
            if (taskSettings.CaptureSettings.RunScreencastCLI)
            {
                if (!IsValidRunScreenCastCLI(taskSettings))
                {
                    return false;
                }
            }

            if (outputType == ScreenRecordOutput.GIF && taskSettings.CaptureSettings.GIFEncoding == ScreenRecordGIFEncoding.FFmpeg)
            {
                outputType = ScreenRecordOutput.FFmpeg;
                taskSettings.CaptureSettings.FFmpegOptions.VideoCodec = FFmpegVideoCodec.gif;
                taskSettings.CaptureSettings.FFmpegOptions.UseCustomCommands = false;
            }

            if (outputType == ScreenRecordOutput.FFmpeg)
            {
                if (!IsValidOutputFFmpeg(taskSettings))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsValidRunScreenCastCLI(TaskSettings taskSettings)
        {
            if (!Program.Settings.VideoEncoders.IsValidIndex(taskSettings.CaptureSettings.VideoEncoderSelected))
            {
                MessageBox.Show(Resources.ScreenRecordForm_StartRecording_There_is_no_valid_CLI_video_encoder_selected_,
                    "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Program.Settings.VideoEncoders[taskSettings.CaptureSettings.VideoEncoderSelected].IsValid())
            {
                MessageBox.Show(Resources.ScreenRecordForm_StartRecording_CLI_video_encoder_file_does_not_exist__ +
                    Program.Settings.VideoEncoders[taskSettings.CaptureSettings.VideoEncoderSelected].Path,
                    "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private static bool IsValidOutputFFmpeg(TaskSettings taskSettings)
        {
            if (!TaskHelpers.CheckFFmpeg(taskSettings))
            {
                return false;
            }

            if (!taskSettings.CaptureSettings.FFmpegOptions.IsSourceSelected)
            {
                MessageBox.Show(Resources.ScreenRecordForm_StartRecording_FFmpeg_video_and_audio_source_both_can_t_be__None__,
                    "ShareX - " + Resources.ScreenRecordForm_StartRecording_FFmpeg_error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private static Rectangle GetCaptureStartMethod()
        {
            Rectangle captureRectangle = Rectangle.Empty;

            switch (startMethod)
            {
                case ScreenRecordStartMethod.Region:
                    TaskHelpers.SelectRegion(out captureRectangle, taskSettings);
                    break;
                case ScreenRecordStartMethod.ActiveWindow:
                    if (taskSettings.CaptureSettings.CaptureClientArea)
                    {
                        captureRectangle = CaptureHelpers.GetActiveWindowClientRectangle();
                    }
                    else
                    {
                        captureRectangle = CaptureHelpers.GetActiveWindowRectangle();
                    }
                    break;
                case ScreenRecordStartMethod.LastRegion:
                    captureRectangle = Program.Settings.ScreenRecordRegion;
                    break;
            }

            return captureRectangle;
        }

        private static void PerformRecording()
        {
            Program.Settings.ScreenRecordRegion = captureRectangle;

            IsRecording = true;
            Screenshot.CaptureCursor = taskSettings.CaptureSettings.ScreenRecordShowCursor;


            abortRequested = false;
            path = "";
            duration = taskSettings.CaptureSettings.ScreenRecordFixedDuration ? taskSettings.CaptureSettings.ScreenRecordDuration : 0;

            recordForm = new ScreenRecordForm(captureRectangle, startMethod == ScreenRecordStartMethod.Region, duration);
            recordForm.StopRequested += StopRecording;
            recordForm.Show();

            RunRecorderThread();
        }

        private static void RunRecorderThread()
        {
            TaskEx.Run(() =>
            {
                try
                {
                    BuildFFmpeg();
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }

                try
                {
                    BuildGif();
                }
                finally
                {
                    DisposeRecorder();
                }
            },
           () =>
           {
               CompleteRecorderThread();
           });
        }

        private static void BuildFFmpeg()
        {
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
                DrawCursor = taskSettings.CaptureSettings.ScreenRecordShowCursor
            };

            recordForm.ChangeState(ScreenRecordState.BeforeStart);

            if (taskSettings.CaptureSettings.ScreenRecordAutoStart)
            {
                int delay = (int)(taskSettings.CaptureSettings.ScreenRecordStartDelay * 1000);

                if (delay > 0)
                {
                    recordForm.InvokeSafe(() => recordForm.StartCountdown(delay));

                    recordForm.RecordResetEvent.WaitOne(delay);
                }
            }
            else
            {
                recordForm.RecordResetEvent.WaitOne();
            }

            if (recordForm.AbortRequested)
            {
                abortRequested = true;
            }

            if (!abortRequested)
            {
                screenRecorder = new ScreenRecorder(outputType, options, captureRectangle);
                screenRecorder.RecordingStarted += () => recordForm.ChangeState(ScreenRecordState.AfterRecordingStart);
                recordForm.ChangeState(ScreenRecordState.AfterStart);
                screenRecorder.StartRecording();

                if (recordForm.AbortRequested)
                {
                    abortRequested = true;
                }
            }
        }

        private static void BuildGif()
        {
            if (!abortRequested && screenRecorder != null)
            {
                recordForm.ChangeState(ScreenRecordState.AfterStop);

                if (outputType == ScreenRecordOutput.GIF)
                {
                    path = Path.Combine(taskSettings.CaptureFolder, TaskHelpers.GetFilename(taskSettings, "gif"));
                    screenRecorder.EncodingProgressChanged += progress => recordForm.ChangeStateProgress(progress);
                    GIFQuality gifQuality = taskSettings.CaptureSettings.GIFEncoding == ScreenRecordGIFEncoding.OctreeQuantizer ? GIFQuality.Bit8 : GIFQuality.Default;
                    screenRecorder.SaveAsGIF(path, gifQuality);
                }
                else if (outputType == ScreenRecordOutput.FFmpeg && taskSettings.CaptureSettings.FFmpegOptions.VideoCodec == FFmpegVideoCodec.gif)
                {
                    path = Path.Combine(taskSettings.CaptureFolder, TaskHelpers.GetFilename(taskSettings, "gif"));
                    screenRecorder.FFmpegEncodeAsGIF(path);
                }

                if (taskSettings.CaptureSettings.RunScreencastCLI)
                {
                    VideoEncoder encoder = Program.Settings.VideoEncoders[taskSettings.CaptureSettings.VideoEncoderSelected];
                    string sourceFilePath = path;
                    path = Path.Combine(taskSettings.CaptureFolder, TaskHelpers.GetFilename(taskSettings, encoder.OutputExtension));
                    screenRecorder.EncodeUsingCommandLine(encoder, sourceFilePath, path);
                }
            }
        }

        private static void DisposeRecorder()
        {
            if (recordForm != null)
            {
                recordForm.InvokeSafe(() =>
                {
                    recordForm.Close();
                    recordForm.Dispose();
                    recordForm = null;
                });
            }

            if (screenRecorder != null)
            {
                if ((outputType == ScreenRecordOutput.GIF || taskSettings.CaptureSettings.RunScreencastCLI ||
                    (outputType == ScreenRecordOutput.FFmpeg && taskSettings.CaptureSettings.FFmpegOptions.VideoCodec == FFmpegVideoCodec.gif)) &&
                    !string.IsNullOrEmpty(screenRecorder.CachePath) && File.Exists(screenRecorder.CachePath))
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

        private static void CompleteRecorderThread() {
            string customFileName;

            if (!abortRequested && !string.IsNullOrEmpty(path) && File.Exists(path) && TaskHelpers.ShowAfterCaptureForm(taskSettings, out customFileName))
            {
                WorkerTask task = WorkerTask.CreateFileJobTask(path, taskSettings, customFileName);
                TaskManager.Start(task);
            }

            abortRequested = false;
            IsRecording = false;
        }
    }
}