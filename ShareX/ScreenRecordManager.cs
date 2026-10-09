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
using ShareX.Localization;
using ShareX.ScreenCaptureLib;
using ShareX.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using NativeScreenRecorder = ShareX.ScreenRecordingLib.ScreenRecorder;
using NativeRecordingOptions = ShareX.ScreenRecordingLib.RecordingOptions;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

namespace ShareX
{
    public static class ScreenRecordManager
    {
        public static bool IsRecording { get; private set; }

        private static ScreenRecorder screenRecorder;
        private static NativeScreenRecorder nativeRecorder;
        private static ScreenRecordWindow recordForm;
        private static ScreenRecorderBarWindow configurationBar;

        public static async void StartStopRecording(ScreenRecordOutput outputType, ScreenRecordStartMethod startMethod, TaskSettings taskSettings)
        {
            if (IsRecording)
            {
                if (configurationBar != null)
                {
                    configurationBar.RequestRecord();
                }
                else if (recordForm != null && !recordForm.IsDisposed)
                {
                    recordForm.StartStopRecording();
                }
            }
            else
            {
                await StartRecording(outputType, taskSettings, startMethod);
            }
        }

        public static void StopRecording()
        {
            configurationBar?.Cancel();
            nativeRecorder?.RequestStop();
            if (IsRecording && screenRecorder != null)
            {
                screenRecorder.StopRecording();
            }
        }

        public static void PauseScreenRecording()
        {
            if (IsRecording && recordForm != null && !recordForm.IsDisposed)
            {
                recordForm.PauseResumeRecording();
            }
        }

        public static void AbortRecording()
        {
            configurationBar?.Cancel();
            if (IsRecording && recordForm != null && !recordForm.IsDisposed)
            {
                recordForm.AbortRecording();
            }
        }

        private static async Task StartRecording(ScreenRecordOutput outputType, TaskSettings taskSettings, ScreenRecordStartMethod startMethod = ScreenRecordStartMethod.Region)
        {
            bool useNative = taskSettings.CaptureSettings.ScreenRecordUseNative && outputType != ScreenRecordOutput.GIF;
            if (outputType == ScreenRecordOutput.GIF)
            {
                taskSettings.CaptureSettings.FFmpegOptions.VideoCodec = FFmpegVideoCodec.gif;
            }

            if (!useNative && taskSettings.CaptureSettings.FFmpegOptions.IsAnimatedImage)
            {
                taskSettings.CaptureSettings.ScreenRecordTwoPassEncoding = true;
            }

            int fps;

            if (!useNative && taskSettings.CaptureSettings.FFmpegOptions.VideoCodec == FFmpegVideoCodec.gif)
            {
                fps = taskSettings.CaptureSettings.GIFFPS;
            }
            else
            {
                fps = taskSettings.CaptureSettings.ScreenRecordFPS;
            }

            if (useNative)
                DebugHelper.WriteLine("Starting native screen recording. H.264/AAC MP4, FPS: {0}", fps);
            else
                DebugHelper.WriteLine("Starting screen recording. Video encoder: \"{0}\", Audio encoder: \"{1}\", FPS: {2}",
                    taskSettings.CaptureSettings.FFmpegOptions.VideoCodec.GetDescription(), taskSettings.CaptureSettings.FFmpegOptions.AudioCodec.GetDescription(), fps);

            if (!useNative && !TaskHelpers.CheckFFmpeg(taskSettings))
            {
                return;
            }

            if (!useNative && !taskSettings.CaptureSettings.FFmpegOptions.IsSourceSelected)
            {
                MessageBox.Show(Strings.FFmpeg_FFmpeg_video_and_audio_source_both_can_t_be__None__,
                    "ShareX - " + Strings.FFmpeg_FFmpeg_error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (taskSettings.GeneralSettings.ToastWindowAutoHide)
            {
                NotificationWindow.CloseActiveWindow();
            }

            Rectangle captureRectangle = Rectangle.Empty;
            IntPtr captureWindow = IntPtr.Zero;
            TaskMetadata metadata = new TaskMetadata();

            switch (startMethod)
            {
                case ScreenRecordStartMethod.Region:
                    var selection = await RegionCaptureTasks.GetRectangleRegionAsync(
                        taskSettings.CaptureSettings.RegionCaptureOptions);
                    if (selection != null)
                    {
                        captureRectangle = selection.Value.Rectangle;
                        metadata.UpdateInfo(selection.Value.WindowInfo);
                    }
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

                    IntPtr handle = NativeMethods.GetForegroundWindow();
                    if (!taskSettings.CaptureSettings.CaptureClientArea) captureWindow = handle;
                    WindowInfo activeWindowInfo = new WindowInfo(handle);
                    metadata.UpdateInfo(activeWindowInfo);
                    break;
                case ScreenRecordStartMethod.CustomRegion:
                    captureRectangle = taskSettings.CaptureSettings.CaptureCustomRegion;
                    break;
                case ScreenRecordStartMethod.LastRegion:
                    captureRectangle = ApplicationState.Settings.ScreenRecordRegion;
                    break;
            }

            Rectangle screenRectangle = CaptureHelpers.GetScreenBounds();
            captureRectangle = Rectangle.Intersect(captureRectangle, screenRectangle);

            if (useNative || taskSettings.CaptureSettings.FFmpegOptions.IsEvenSizeRequired)
            {
                captureRectangle = CaptureHelpers.EvenRectangleSize(captureRectangle);
            }

            if (IsRecording || !captureRectangle.IsValid() || screenRecorder != null || nativeRecorder != null)
            {
                return;
            }

            IsRecording = true;

            bool startedFromBar = useNative && taskSettings.CaptureSettings.ScreenRecordShowBar;
            if (startedFromBar)
            {
                try
                {
                    configurationBar = new ScreenRecorderBarWindow(taskSettings, captureRectangle, captureWindow);
                    bool record = await configurationBar.ShowSetupAsync();
                    configurationBar.Commit(taskSettings);
                    SettingManager.SaveApplicationConfigAsync();
                    SettingManager.SaveHotkeysConfigAsync();
                    if (!record)
                    {
                        IsRecording = false;
                        return;
                    }
                    captureRectangle = configurationBar.RecordingRegion;
                    captureWindow = configurationBar.CaptureWindow;
                    fps = taskSettings.CaptureSettings.ScreenRecordFPS;
                }
                catch (Exception ex)
                {
                    IsRecording = false;
                    DebugHelper.WriteException(ex);
                    MessageBox.Show(ex.Message, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    configurationBar?.Cancel();
                    configurationBar = null;
                }
            }

            ApplicationState.Settings.ScreenRecordRegion = captureRectangle;

            string path = "";
            string concatPath = "";
            string tempPath = "";
            bool abortRequested = false;

            float duration = taskSettings.CaptureSettings.ScreenRecordFixedDuration ? taskSettings.CaptureSettings.ScreenRecordDuration : 0;

            recordForm = new ScreenRecordWindow(captureRectangle)
            {
                ActivateWindow = startMethod == ScreenRecordStartMethod.Region,
                Duration = duration,
                AskConfirmationOnAbort = taskSettings.CaptureSettings.ScreenRecordAskConfirmationOnAbort,
                ShowRecordingTimer = taskSettings.CaptureSettings.ScreenRecordShowTimer,
                ShowRecordingButtonLabels = taskSettings.CaptureSettings.ScreenRecordShowButtonLabels
            };
            recordForm.UseInProcessPause = useNative;
            recordForm.ExcludeFromCapture = useNative;
            recordForm.PauseRequested += () => nativeRecorder?.Pause();
            recordForm.ResumeRequested += () =>
            {
                if (captureWindow == IntPtr.Zero) nativeRecorder?.Resume(recordForm.RecordingRegion);
                else nativeRecorder?.Resume();
            };

            recordForm.StopRequested += StopRecording;
            recordForm.Show();

            _ = Task.Run(async () =>
            {
                try
                {
                    string extension;
                    if (useNative || taskSettings.CaptureSettings.ScreenRecordTwoPassEncoding)
                    {
                        extension = "mp4";
                    }
                    else
                    {
                        extension = taskSettings.CaptureSettings.FFmpegOptions.Extension;
                    }
                    string screenshotsFolder = TaskHelpers.GetScreenshotsFolder(taskSettings, metadata);
                    string fileName = TaskHelpers.GetFileName(taskSettings, extension, metadata);
                    string requestedPath = Path.Combine(screenshotsFolder, fileName);
                    bool nativeOverwriteCandidate = useNative && File.Exists(requestedPath);
                    path = TaskHelpers.HandleExistsFile(requestedPath, taskSettings);
                    bool replaceNativeOutput = nativeOverwriteCandidate && string.Equals(path, requestedPath, StringComparison.OrdinalIgnoreCase);

                    if (string.IsNullOrEmpty(path))
                    {
                        abortRequested = true;
                    }
                    else if (!useNative)
                    {
                        concatPath = FileHelpers.AppendTextToFileName(path, "-concat");
                        FileHelpers.DeleteFile(concatPath);
                        tempPath = FileHelpers.AppendTextToFileName(path, "-temp");
                        FileHelpers.DeleteFile(tempPath);
                    }

                    if (useNative && !abortRequested)
                    {
                        abortRequested = await RecordNativeAsync(path, captureWindow, taskSettings, replaceNativeOutput, startedFromBar);
                    }

                    while (!useNative && !abortRequested && (recordForm.Status == ScreenRecordingStatus.Waiting || recordForm.Status == ScreenRecordingStatus.Paused))
                    {
                        recordForm.ChangeState(ScreenRecordState.BeforeStart);

                        if (recordForm.Status == ScreenRecordingStatus.Paused || !taskSettings.CaptureSettings.ScreenRecordAutoStart)
                        {
                            recordForm.RecordResetEvent.WaitOne();
                        }
                        else
                        {
                            int delay = (int)(taskSettings.CaptureSettings.ScreenRecordStartDelay * 1000);

                            if (delay > 0)
                            {
                                recordForm.InvokeSafe(() => recordForm.StartCountdown(delay));

                                recordForm.RecordResetEvent.WaitOne(delay);
                            }
                        }

                        if (recordForm.Status == ScreenRecordingStatus.Aborted)
                        {
                            abortRequested = true;
                        }

                        if (recordForm.ConsumeRestartRequest())
                        {
                            screenRecorder?.Dispose();
                            screenRecorder = null;
                            FileHelpers.DeleteFile(path);
                            FileHelpers.DeleteFile(concatPath);
                            FileHelpers.DeleteFile(tempPath);
                        }

                        if (recordForm.Status == ScreenRecordingStatus.Waiting || recordForm.Status == ScreenRecordingStatus.Paused)
                        {
                            if (recordForm.Status == ScreenRecordingStatus.Paused && File.Exists(path))
                            {
                                FileHelpers.RenameFile(path, concatPath);
                            }

                            recordForm.ChangeState(ScreenRecordState.AfterStart);

                            captureRectangle = recordForm.RecordingRegion;

                            ScreenRecordingOptions options = new ScreenRecordingOptions()
                            {
                                IsRecording = true,
                                IsLossless = taskSettings.CaptureSettings.ScreenRecordTwoPassEncoding,
                                FFmpeg = taskSettings.CaptureSettings.FFmpegOptions,
                                FPS = fps,
                                Duration = duration,
                                OutputPath = path,
                                CaptureArea = captureRectangle,
                                DrawCursor = taskSettings.CaptureSettings.ScreenRecordShowCursor
                            };

                            Screenshot screenshot = TaskHelpers.GetScreenshot(taskSettings);
                            screenshot.CaptureCursor = taskSettings.CaptureSettings.ScreenRecordShowCursor;

                            screenRecorder?.Dispose();
                            screenRecorder = new ScreenRecorder(ScreenRecordOutput.FFmpeg, options, screenshot, captureRectangle);
                            screenRecorder.RecordingStarted += ScreenRecorder_RecordingStarted;
                            screenRecorder.EncodingProgressChanged += ScreenRecorder_EncodingProgressChanged;
                            using (IDisposable highlighter = taskSettings.CaptureSettings.ScreenRecordMouseHighlighter
                                ? await MouseHighlighterManager.BeginRecordingAsync(taskSettings.ToolsSettingsReference.MouseHighlighterOptions)
                                : null)
                            {
                                if (recordForm.Status != ScreenRecordingStatus.Aborted && recordForm.Status != ScreenRecordingStatus.Stopped)
                                {
                                    screenRecorder.StartRecording();
                                }
                            }
                            recordForm.ChangeState(ScreenRecordState.RecordingEnd);

                            if (recordForm.Status == ScreenRecordingStatus.Aborted)
                            {
                                abortRequested = true;
                            }

                            if (recordForm.RestartRequested)
                            {
                                continue;
                            }
                        }

                        TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);

                        if (File.Exists(concatPath))
                        {
                            using (FFmpegCLIManager ffmpeg = new FFmpegCLIManager(taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath))
                            {
                                ffmpeg.ShowError = true;
                                ffmpeg.ConcatenateVideos(new string[] { concatPath, path }, tempPath, true);
                                FileHelpers.RenameFile(tempPath, path);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    if (useNative)
                    {
                        abortRequested = true;
                        recordForm.InvokeSafe(() => MessageBox.Show(e.Message, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    }
                }

                if (!useNative && taskSettings.CaptureSettings.ScreenRecordTwoPassEncoding && !abortRequested && screenRecorder != null && File.Exists(path))
                {
                    recordForm.ChangeState(ScreenRecordState.Encoding);

                    path = ProcessTwoPassEncoding(path, metadata, taskSettings);
                }

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
                    screenRecorder.Dispose();
                    screenRecorder = null;
                }
                if (nativeRecorder != null)
                {
                    await nativeRecorder.DisposeAsync();
                    nativeRecorder = null;
                }

                if (abortRequested && !useNative)
                {
                    FileHelpers.DeleteFile(path);
                }

                FileHelpers.DeleteFile(concatPath);
                FileHelpers.DeleteFile(tempPath);
            }).ContinueInCurrentContext(() =>
            {
                void FinishRecording(AfterCaptureWindowResult result)
                {
                    if (result.Accepted)
                    {
                        string customFileName = result.FileName;

                        if (!string.IsNullOrEmpty(customFileName))
                        {
                            string currentFileName = Path.GetFileNameWithoutExtension(path);
                            string ext = Path.GetExtension(path);

                            if (!currentFileName.Equals(customFileName, StringComparison.OrdinalIgnoreCase))
                            {
                                path = FileHelpers.RenameFile(path, customFileName + ext);
                            }
                        }

                        WorkerTask task = WorkerTask.CreateFileJobTask(path, metadata, taskSettings, customFileName);
                        TaskManager.Start(task);
                    }

                    abortRequested = false;
                    IsRecording = false;
                }

                if (!abortRequested && !string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    TaskHelpers.ShowAfterCaptureWindow(taskSettings, FinishRecording, null, path);
                }
                else
                {
                    abortRequested = false;
                    IsRecording = false;
                }
            });
        }

        private static async Task<bool> RecordNativeAsync(string path, IntPtr captureWindow, TaskSettings settings, bool replaceExisting, bool startedFromBar)
        {
            // HandleExistsFile has already applied the user's overwrite policy. Commit a completed
            // recording afterward so aborts, restarts and startup errors preserve an existing file.
            string recordingPath = Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(path)}-recording-{Guid.NewGuid():N}.mp4");
            while (true)
            {
                bool completed = false;
                NativeRecordingOptions options = new()
                {
                    OutputPath = recordingPath,
                    Region = recordForm.RecordingRegion,
                    WindowHandle = captureWindow,
                    FramesPerSecond = settings.CaptureSettings.ScreenRecordFPS,
                    VideoBitrate = settings.CaptureSettings.ScreenRecordVideoBitrate * 1000,
                    CaptureSystemAudio = settings.CaptureSettings.ScreenRecordSystemAudio,
                    SystemAudioDeviceId = settings.CaptureSettings.ScreenRecordSystemAudioDeviceId,
                    SystemAudioGain = settings.CaptureSettings.ScreenRecordSystemAudioGain,
                    CaptureMicrophone = settings.CaptureSettings.ScreenRecordMicrophone,
                    MicrophoneDeviceId = settings.CaptureSettings.ScreenRecordMicrophoneDeviceId,
                    MicrophoneGain = settings.CaptureSettings.ScreenRecordMicrophoneGain,
                    CaptureCamera = settings.CaptureSettings.ScreenRecordCamera,
                    CameraDeviceId = settings.CaptureSettings.ScreenRecordCameraDeviceId,
                    CameraResolution = settings.CaptureSettings.ScreenRecordCameraResolution,
                    CameraFramesPerSecond = settings.CaptureSettings.ScreenRecordCameraFPS,
                    CameraPosition = settings.CaptureSettings.ScreenRecordCameraPosition,
                    CameraShape = settings.CaptureSettings.ScreenRecordCameraShape,
                    CameraWidthPercent = settings.CaptureSettings.ScreenRecordCameraWidthPercent,
                    CameraMargin = settings.CaptureSettings.ScreenRecordCameraMargin,
                    RequireHardwareEncoder = settings.CaptureSettings.ScreenRecordRequireHardwareEncoder,
                    IncludeCursor = settings.CaptureSettings.ScreenRecordShowCursor,
                    Duration = settings.CaptureSettings.ScreenRecordFixedDuration ? TimeSpan.FromSeconds(settings.CaptureSettings.ScreenRecordDuration) : TimeSpan.Zero
                };
                nativeRecorder = new NativeScreenRecorder(options);
                nativeRecorder.Diagnostic += message => DebugHelper.WriteLine("Native recorder: " + message);
                try
                {
                    recordForm.ChangeState(ScreenRecordState.BeforeStart);
                    // Initialize the GPU, codecs and audio endpoints before manual start or the countdown.
                    // Prepare does not start video or audio capture.
                    await nativeRecorder.PrepareAsync();
                    if (!startedFromBar && !settings.CaptureSettings.ScreenRecordAutoStart)
                    {
                        recordForm.RecordResetEvent.WaitOne();
                    }
                    else
                    {
                        int delay = (int)(settings.CaptureSettings.ScreenRecordStartDelay * 1000);
                        if (delay > 0)
                        {
                            recordForm.InvokeSafe(() => recordForm.StartCountdown(delay));
                            recordForm.RecordResetEvent.WaitOne(delay);
                        }
                    }
                    recordForm.ConsumeRestartRequest();
                    if (recordForm.Status == ScreenRecordingStatus.Aborted) return true;
                    if (recordForm.Status == ScreenRecordingStatus.Stopped) return true;
                    // A waiting recording region can be dragged. Rebuild for its final physical coordinates.
                    if (captureWindow == IntPtr.Zero && options.Region != recordForm.RecordingRegion)
                    {
                        await nativeRecorder.DisposeAsync();
                        nativeRecorder = new NativeScreenRecorder(options with { Region = recordForm.RecordingRegion });
                        nativeRecorder.Diagnostic += message => DebugHelper.WriteLine("Native recorder: " + message);
                        await nativeRecorder.PrepareAsync();
                    }
                    recordForm.ChangeState(ScreenRecordState.AfterStart);
                    using (IDisposable highlighter = settings.CaptureSettings.ScreenRecordMouseHighlighter
                        ? await MouseHighlighterManager.BeginRecordingAsync(settings.ToolsSettings.MouseHighlighterOptions) : null)
                    {
                        await nativeRecorder.StartAsync();
                        if (recordForm.Status == ScreenRecordingStatus.Aborted || recordForm.Status == ScreenRecordingStatus.Stopped || recordForm.RestartRequested)
                            nativeRecorder.RequestStop();
                        else ScreenRecorder_RecordingStarted();
                        var result = await nativeRecorder.Completion;
                        completed = true;
                        DebugHelper.WriteLine("Native screen recording completed. Encoder: {0}, hardware: {1}, frames: {2}, dropped: {3}, audio discontinuities: {4}",
                            result.Encoder.Name, result.Encoder.IsHardwareAccelerated, result.VideoFrames, result.DroppedVideoFrames, result.AudioDiscontinuities);
                    }
                }
                catch (OperationCanceledException) when (recordForm.Status == ScreenRecordingStatus.Aborted || recordForm.Status == ScreenRecordingStatus.Stopped || recordForm.RestartRequested)
                {
                    // A stop during preparation or before the first frame removes the incomplete file.
                }
                finally
                {
                    await nativeRecorder.DisposeAsync();
                    nativeRecorder = null;
                    recordForm.ChangeState(ScreenRecordState.RecordingEnd);
                }
                if (recordForm.RestartRequested)
                {
                    FileHelpers.DeleteFile(recordingPath);
                    continue;
                }
                TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted, settings);
                bool aborted = recordForm.Status == ScreenRecordingStatus.Aborted;
                if (aborted) FileHelpers.DeleteFile(recordingPath);
                if (!aborted && completed)
                {
                    try { File.Move(recordingPath, path, replaceExisting); }
                    catch (Exception ex)
                    {
                        throw new IOException($"The recording finished, but its output could not be moved to '{path}'. The completed recording is available at '{recordingPath}'.", ex);
                    }
                }
                return aborted || !completed;
            }
        }

        private static void ScreenRecorder_RecordingStarted()
        {
            recordForm.ChangeState(ScreenRecordState.AfterRecordingStart);
        }

        private static void ScreenRecorder_EncodingProgressChanged(int progress)
        {
            recordForm.ChangeStateProgress(progress);
        }

        private static string ProcessTwoPassEncoding(string input, TaskMetadata metadata, TaskSettings taskSettings, bool deleteInputFile = true)
        {
            string screenshotsFolder = TaskHelpers.GetScreenshotsFolder(taskSettings, metadata);
            string fileName = TaskHelpers.GetFileName(taskSettings, taskSettings.CaptureSettings.FFmpegOptions.Extension, metadata);
            string output = Path.Combine(screenshotsFolder, fileName);

            try
            {
                if (taskSettings.CaptureSettings.FFmpegOptions.VideoCodec == FFmpegVideoCodec.gif)
                {
                    screenRecorder.FFmpegEncodeAsGIF(input, output);
                }
                else
                {
                    screenRecorder.FFmpegEncodeVideo(input, output);
                }
            }
            finally
            {
                if (deleteInputFile && !input.Equals(output, StringComparison.OrdinalIgnoreCase) && File.Exists(input))
                {
                    File.Delete(input);
                }
            }

            return output;
        }
    }
}
