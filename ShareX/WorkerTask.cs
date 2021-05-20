﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using HttpMethod = System.Net.Http.HttpMethod;

namespace ShareX
{
    public class WorkerTask : IDisposable
    {
        public delegate void TaskEventHandler(WorkerTask task);
        public delegate void TaskImageEventHandler(WorkerTask task, Bitmap image);
        public delegate void UploaderServiceEventHandler(IUploaderService uploaderService);

        public event TaskEventHandler StatusChanged, UploadStarted, UploadProgressChanged, UploadCompleted, TaskCompleted;
        public event TaskImageEventHandler ImageReady;
        public event UploaderServiceEventHandler UploadersConfigWindowRequested;

        public TaskInfo Info { get; private set; }
        public TaskStatus Status { get; private set; }
        public bool IsBusy => Status == TaskStatus.InQueue || IsWorking;
        public bool IsWorking => Status == TaskStatus.Preparing || Status == TaskStatus.Working || Status == TaskStatus.Stopping;
        public bool StopRequested { get; private set; }
        public bool RequestSettingUpdate { get; private set; }
        public bool EarlyURLCopied { get; private set; }
        public Stream Data { get; private set; }
        public Bitmap Image { get; private set; }
        public bool KeepImage { get; set; }
        public string Text { get; private set; }

        private ThreadWorker threadWorker;
        private GenericUploader uploader;
        private TaskReferenceHelper taskReferenceHelper;

        #region Constructors

        private WorkerTask(TaskSettings taskSettings)
        {
            Status = TaskStatus.InQueue;
            Info = new TaskInfo(taskSettings);
        }

        public static WorkerTask CreateHistoryTask(RecentTask recentTask)
        {
            WorkerTask task = new WorkerTask(null);
            task.Status = TaskStatus.History;
            task.Info.FilePath = recentTask.FilePath;
            task.Info.FileName = recentTask.FileName;
            task.Info.Result.URL = recentTask.URL;
            task.Info.Result.ThumbnailURL = recentTask.ThumbnailURL;
            task.Info.Result.DeletionURL = recentTask.DeletionURL;
            task.Info.Result.ShortenedURL = recentTask.ShortenedURL;
            task.Info.TaskEndTime = recentTask.Time;

            return task;
        }

        public static WorkerTask CreateDataUploaderTask(EDataType dataType, Stream stream, string fileName, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = TaskJob.DataUpload;
            task.Info.DataType = dataType;
            task.Info.FileName = fileName;
            task.Data = stream;
            return task;
        }

        public static WorkerTask CreateFileUploaderTask(string filePath, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.FilePath = filePath;
            task.Info.DataType = TaskHelpers.FindDataType(task.Info.FilePath, taskSettings);

            if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Helpers.GetFilenameExtension(task.Info.FilePath);
                task.Info.FileName = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            if (task.Info.TaskSettings.AdvancedSettings.ProcessImagesDuringFileUpload && task.Info.DataType == EDataType.Image)
            {
                task.Info.Job = TaskJob.Job;
                task.Image = ImageHelpers.LoadImage(task.Info.FilePath);
            }
            else
            {
                task.Info.Job = TaskJob.FileUpload;

                if (!task.LoadFileStream())
                {
                    return null;
                }
            }

            return task;
        }

        public static WorkerTask CreateImageUploaderTask(ImageInfo imageInfo, TaskSettings taskSettings, string customFileName = null)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = TaskJob.Job;
            task.Info.DataType = EDataType.Image;

            if (!string.IsNullOrEmpty(customFileName))
            {
                task.Info.FileName = Helpers.AppendExtension(customFileName, "bmp");
            }
            else
            {
                task.Info.FileName = TaskHelpers.GetFilename(taskSettings, "bmp", imageInfo);
            }

            task.Image = imageInfo.Image;
            return task;
        }

        public static WorkerTask CreateTextUploaderTask(string text, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = TaskJob.TextUpload;
            task.Info.DataType = EDataType.Text;
            task.Info.FileName = TaskHelpers.GetFilename(taskSettings, taskSettings.AdvancedSettings.TextFileExtension);
            task.Text = text;
            return task;
        }

        public static WorkerTask CreateURLShortenerTask(string url, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = TaskJob.ShortenURL;
            task.Info.DataType = EDataType.URL;
            task.Info.FileName = string.Format(Resources.UploadTask_CreateURLShortenerTask_Shorten_URL___0__, taskSettings.URLShortenerDestination.GetLocalizedDescription());
            task.Info.Result.URL = url;
            return task;
        }

        public static WorkerTask CreateShareURLTask(string url, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = TaskJob.ShareURL;
            task.Info.DataType = EDataType.URL;
            task.Info.FileName = string.Format(Resources.UploadTask_CreateShareURLTask_Share_URL___0__, taskSettings.URLSharingServiceDestination.GetLocalizedDescription());
            task.Info.Result.URL = url;
            return task;
        }

        public static WorkerTask CreateFileJobTask(string filePath, TaskSettings taskSettings, string customFileName = null)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.FilePath = filePath;
            task.Info.DataType = TaskHelpers.FindDataType(task.Info.FilePath, taskSettings);

            if (!string.IsNullOrEmpty(customFileName))
            {
                string ext = Helpers.GetFilenameExtension(task.Info.FilePath);
                task.Info.FileName = Helpers.AppendExtension(customFileName, ext);
            }
            else if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Helpers.GetFilenameExtension(task.Info.FilePath);
                task.Info.FileName = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            task.Info.Job = TaskJob.Job;

            if (task.Info.IsUploadJob && !task.LoadFileStream())
            {
                return null;
            }

            return task;
        }

        public static WorkerTask CreateDownloadTask(string url, bool upload, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings)
            {
                Info = {Job = upload ? TaskJob.DownloadUpload : TaskJob.Download}
            };

            string filename = URLHelpers.URLDecode(url, 10);
            filename = URLHelpers.GetFileName(filename);
            filename = Helpers.GetValidFileName(filename);

            if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Helpers.GetFilenameExtension(filename);
                filename = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            task.Info.FileName = filename;
            task.Info.DataType = TaskHelpers.FindDataType(task.Info.FileName, taskSettings);
            task.Info.Result.URL = url;
            return task;
        }

        #endregion Constructors

        public void Start()
        {
            if (Status == TaskStatus.InQueue && !StopRequested)
            {
                Info.TaskStartTime = DateTime.Now;

                threadWorker = new ThreadWorker();
                Prepare();
                threadWorker.DoWork += ThreadDoWork;
                threadWorker.Completed += ThreadCompleted;
                threadWorker.Start(ApartmentState.STA);
            }
        }

        private void Prepare()
        {
            Status = TaskStatus.Preparing;

            switch (Info.Job)
            {
                case TaskJob.Job:
                case TaskJob.TextUpload:
                    Info.Status = Resources.UploadTask_Prepare_Preparing;
                    break;
                default:
                    Info.Status = Resources.UploadTask_Prepare_Starting;
                    break;
            }

            OnStatusChanged();
        }

        public void Stop()
        {
            StopRequested = true;

            switch (Status)
            {
                case TaskStatus.InQueue:
                    OnTaskCompleted();
                    break;
                case TaskStatus.Preparing:
                case TaskStatus.Working:
                    if (uploader != null) uploader.StopUpload();
                    Status = TaskStatus.Stopping;
                    Info.Status = Resources.UploadTask_Stop_Stopping;
                    OnStatusChanged();
                    break;
            }
        }

        public void ShowErrorWindow()
        {
            if (Info != null && Info.Result != null && Info.Result.IsError)
            {
                string errors = Info.Result.ErrorsToString();

                if (!string.IsNullOrEmpty(errors))
                {
                    using (ErrorForm form = new ErrorForm(Resources.UploadInfoManager_ShowErrors_Upload_errors, errors, Program.LogsFilePath, Links.URL_ISSUES, false))
                    {
                        form.ShowDialog();
                    }
                }
            }
        }

        private void ThreadDoWork()
        {
            CreateTaskReferenceHelper();

            try
            {
                StopRequested = !DoThreadJob();

                OnImageReady();

                if (!StopRequested)
                {
                    if (Info.IsUploadJob && TaskHelpers.IsUploadAllowed())
                    {
                        DoUploadJob();
                    }
                    else
                    {
                        Info.Result.IsURLExpected = false;
                    }
                }
            }
            finally
            {
                KeepImage = Image != null && Info.TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted;

                Dispose();

                if (EarlyURLCopied && (StopRequested || Info.Result == null || string.IsNullOrEmpty(Info.Result.URL)) && ClipboardHelpers.ContainsText())
                {
                    ClipboardHelpers.Clear();
                }

                if (Info.Job == TaskJob.Job && Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.DeleteFile) && !string.IsNullOrEmpty(Info.FilePath) && File.Exists(Info.FilePath))
                {
                    File.Delete(Info.FilePath);
                }
            }

            if (!StopRequested && Info.Result != null && Info.Result.IsURLExpected && !Info.Result.IsError)
            {
                if (string.IsNullOrEmpty(Info.Result.URL))
                {
                    AddErrorMessage(Resources.UploadTask_ThreadDoWork_URL_is_empty_);
                }
                else
                {
                    DoAfterUploadJobs();
                }
            }
        }

        private void CreateTaskReferenceHelper()
        {
            taskReferenceHelper = new TaskReferenceHelper()
            {
                DataType = Info.DataType,
                OverrideFTP = Info.TaskSettings.OverrideFTP,
                FTPIndex = Info.TaskSettings.FTPIndex,
                OverrideCustomUploader = Info.TaskSettings.OverrideCustomUploader,
                CustomUploaderIndex = Info.TaskSettings.CustomUploaderIndex,
                TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat
            };
        }

        private void DoUploadJob()
        {
            if (Program.Settings.ShowUploadWarning && MessageBox.Show(Resources.UploadTask_DoUploadJob_First_time_upload_warning_text,
                "ShareX - " + Resources.UploadTask_DoUploadJob_First_time_upload_warning,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                Program.Settings.ShowUploadWarning = false;
                Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.UploadImageToHost);
                Info.TaskSettings.AfterCaptureJob = Info.TaskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.UploadImageToHost);
                Info.Result.IsURLExpected = false;
                RequestSettingUpdate = true;
                return;
            }

            if (Program.Settings.ShowLargeFileSizeWarning > 0)
            {
                long dataSize = Program.Settings.BinaryUnits ? Program.Settings.ShowLargeFileSizeWarning * 1024 * 1024 : Program.Settings.ShowLargeFileSizeWarning * 1000 * 1000;
                if (Data != null && Data.Length > dataSize)
                {
                    using (MyMessageBox msgbox = new MyMessageBox(Resources.UploadTask_DoUploadJob_You_are_attempting_to_upload_a_large_file, "ShareX",
                        MessageBoxButtons.YesNo, Resources.UploadManager_IsUploadConfirmed_Don_t_show_this_message_again_))
                    {
                        msgbox.ShowDialog();
                        if (msgbox.IsChecked) Program.Settings.ShowLargeFileSizeWarning = 0;
                        if (msgbox.DialogResult == DialogResult.No) Stop();
                    }
                }
            }

            if (!StopRequested)
            {
                Program.Settings.ShowUploadWarning = false;

                SettingManager.WaitUploadersConfig();

                Status = TaskStatus.Working;
                Info.Status = Resources.UploadTask_DoUploadJob_Uploading;

                TaskbarManager.SetProgressState(Program.MainForm, TaskbarProgressBarStatus.Normal);

                bool cancelUpload = false;

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ShowBeforeUploadWindow))
                {
                    using (BeforeUploadForm form = new BeforeUploadForm(Info))
                    {
                        cancelUpload = form.ShowDialog() != DialogResult.OK;
                    }
                }

                if (!cancelUpload)
                {
                    OnUploadStarted();

                    bool isError = DoUpload();

                    if (isError && Program.Settings.MaxUploadFailRetry > 0)
                    {
                        for (int retry = 1; !StopRequested && isError && retry <= Program.Settings.MaxUploadFailRetry; retry++)
                        {
                            DebugHelper.WriteLine("Upload failed. Retrying upload.");
                            isError = DoUpload(retry);
                        }
                    }

                    if (!isError)
                    {
                        OnUploadCompleted();
                    }
                }
                else
                {
                    Info.Result.IsURLExpected = false;
                }
            }
        }

        private bool DoUpload(int retry = 0)
        {
            bool isError = false;

            if (retry > 0)
            {
                if (Program.Settings.UseSecondaryUploaders)
                {
                    Info.TaskSettings.ImageDestination = Program.Settings.SecondaryImageUploaders[retry - 1];
                    Info.TaskSettings.ImageFileDestination = Program.Settings.SecondaryFileUploaders[retry - 1];
                    Info.TaskSettings.TextDestination = Program.Settings.SecondaryTextUploaders[retry - 1];
                    Info.TaskSettings.TextFileDestination = Program.Settings.SecondaryFileUploaders[retry - 1];
                    Info.TaskSettings.FileDestination = Program.Settings.SecondaryFileUploaders[retry - 1];
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

            SSLBypassHelper sslBypassHelper = null;

            try
            {
                if (HelpersOptions.AcceptInvalidSSLCertificates)
                {
                    sslBypassHelper = new SSLBypassHelper();
                }

                if (!CheckUploadFilters(Data, Info.FileName))
                {
                    switch (Info.UploadDestination)
                    {
                        case EDataType.Image:
                            Info.Result = UploadImage(Data, Info.FileName);
                            break;
                        case EDataType.Text:
                            Info.Result = UploadText(Data, Info.FileName);
                            break;
                        case EDataType.File:
                            Info.Result = UploadFile(Data, Info.FileName);
                            break;
                    }
                }

                StopRequested |= taskReferenceHelper.StopRequested;
            }
            catch (Exception e)
            {
                if (!StopRequested)
                {
                    DebugHelper.WriteException(e);
                    isError = true;
                    AddErrorMessage(e.ToString());
                }
            }
            finally
            {
                if (sslBypassHelper != null)
                {
                    sslBypassHelper.Dispose();
                }

                if (Info.Result == null)
                {
                    Info.Result = new UploadResult();
                }

                if (uploader != null)
                {
                    AddErrorMessage(uploader.Errors.ToArray());
                }

                isError |= Info.Result.IsError;
            }

            return isError;
        }

        private void AddErrorMessage(params string[] errorMessages)
        {
            if (Info.Result == null)
            {
                Info.Result = new UploadResult();
            }

            Info.Result.Errors.AddRange(errorMessages);
        }

        private bool DoThreadJob()
        {
            if (Info.IsUploadJob && Info.TaskSettings.AdvancedSettings.AutoClearClipboard)
            {
                ClipboardHelpers.Clear();
            }

            if (Info.Job == TaskJob.Download || Info.Job == TaskJob.DownloadUpload)
            {
                bool downloadResult = DownloadFromURL(Info.Job == TaskJob.DownloadUpload);

                if (!downloadResult)
                {
                    return false;
                }
                else if (Info.Job == TaskJob.Download)
                {
                    return true;
                }
            }

            if (Info.Job == TaskJob.Job)
            {
                if (!DoAfterCaptureJobs())
                {
                    return false;
                }

                DoFileJobs();
            }
            else if (Info.Job == TaskJob.TextUpload && !string.IsNullOrEmpty(Text))
            {
                DoTextJobs();
            }
            else if (Info.Job == TaskJob.FileUpload && Info.TaskSettings.AdvancedSettings.UseAfterCaptureTasksDuringFileUpload)
            {
                DoFileJobs();
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.DoOCR))
            {
                DoOCR();
            }

            if (Info.IsUploadJob && Data != null && Data.CanSeek)
            {
                Data.Position = 0;
            }

            return true;
        }

        private bool DoAfterCaptureJobs()
        {
            if (Image == null)
            {
                return true;
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects))
            {
                Image = TaskHelpers.ApplyImageEffects(Image, Info.TaskSettings.ImageSettingsReference);

                if (Image == null)
                {
                    DebugHelper.WriteLine("Error: Applying image effects resulted empty image.");
                    return false;
                }
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AnnotateImage))
            {
                Image = TaskHelpers.AnnotateImage(Image, null, Info.TaskSettings, true);

                if (Image == null)
                {
                    return false;
                }
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyImageToClipboard))
            {
                ClipboardHelpers.CopyImage(Image, Info.FileName);
                DebugHelper.WriteLine("Image copied to clipboard.");
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SendImageToPrinter))
            {
                TaskHelpers.PrintImage(Image);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlagAny(AfterCaptureTasks.SaveImageToFile, AfterCaptureTasks.SaveImageToFileWithDialog, AfterCaptureTasks.DoOCR,
                AfterCaptureTasks.UploadImageToHost))
            {
                ImageData imageData = TaskHelpers.PrepareImage(Image, Info.TaskSettings);
                Data = imageData.ImageStream;
                Info.FileName = Path.ChangeExtension(Info.FileName, imageData.ImageFormat.GetDescription());

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SaveImageToFile))
                {
                    string filePath = TaskHelpers.HandleExistsFile(Info.TaskSettings.GetScreenshotsFolder(), Info.FileName, Info.TaskSettings);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        Info.FilePath = filePath;
                        imageData.Write(Info.FilePath);
                        DebugHelper.WriteLine("Image saved to file: " + Info.FilePath);
                    }
                }

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SaveImageToFileWithDialog))
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        string initialDirectory = null;

                        if (!string.IsNullOrEmpty(HelpersOptions.LastSaveDirectory) && Directory.Exists(HelpersOptions.LastSaveDirectory))
                        {
                            initialDirectory = HelpersOptions.LastSaveDirectory;
                        }
                        else
                        {
                            initialDirectory = Info.TaskSettings.GetScreenshotsFolder();
                        }

                        bool imageSaved;

                        do
                        {
                            sfd.InitialDirectory = initialDirectory;
                            sfd.FileName = Info.FileName;
                            sfd.DefaultExt = Path.GetExtension(Info.FileName).Substring(1);
                            sfd.Filter = string.Format("*{0}|*{0}|All files (*.*)|*.*", Path.GetExtension(Info.FileName));
                            sfd.Title = Resources.UploadTask_DoAfterCaptureJobs_Choose_a_folder_to_save + " " + Path.GetFileName(Info.FileName);

                            if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                            {
                                Info.FilePath = sfd.FileName;
                                HelpersOptions.LastSaveDirectory = Path.GetDirectoryName(Info.FilePath);
                                imageSaved = imageData.Write(Info.FilePath);

                                if (imageSaved)
                                {
                                    DebugHelper.WriteLine("Image saved to file with dialog: " + Info.FilePath);
                                }
                            }
                            else
                            {
                                break;
                            }
                        } while (!imageSaved);
                    }
                }

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SaveThumbnailImageToFile))
                {
                    string thumbnailFilename, thumbnailFolder;

                    if (!string.IsNullOrEmpty(Info.FilePath))
                    {
                        thumbnailFilename = Path.GetFileName(Info.FilePath);
                        thumbnailFolder = Path.GetDirectoryName(Info.FilePath);
                    }
                    else
                    {
                        thumbnailFilename = Info.FileName;
                        thumbnailFolder = Info.TaskSettings.GetScreenshotsFolder();
                    }

                    Info.ThumbnailFilePath = TaskHelpers.CreateThumbnail(Image, thumbnailFolder, thumbnailFilename, Info.TaskSettings);

                    if (!string.IsNullOrEmpty(Info.ThumbnailFilePath))
                    {
                        DebugHelper.WriteLine("Thumbnail saved to file: " + Info.ThumbnailFilePath);
                    }
                }
            }

            return true;
        }

        private void DoFileJobs()
        {
            if (!string.IsNullOrEmpty(Info.FilePath) && File.Exists(Info.FilePath))
            {
                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.PerformActions) && Info.TaskSettings.ExternalPrograms != null)
                {
                    IEnumerable<ExternalProgram> actions = Info.TaskSettings.ExternalPrograms.Where(x => x.IsActive);

                    if (actions.Count() > 0)
                    {
                        bool isFileModified = false;
                        string fileName = Info.FileName;

                        foreach (ExternalProgram fileAction in actions)
                        {
                            string modifiedPath = fileAction.Run(Info.FilePath);

                            if (!string.IsNullOrEmpty(modifiedPath))
                            {
                                isFileModified = true;
                                Info.FilePath = modifiedPath;

                                if (Data != null)
                                {
                                    Data.Dispose();
                                }

                                fileAction.DeletePendingInputFile();
                            }
                        }

                        if (isFileModified)
                        {
                            string extension = Helpers.GetFilenameExtension(Info.FilePath);
                            Info.FileName = Helpers.ChangeFilenameExtension(fileName, extension);

                            LoadFileStream();
                        }
                    }
                }

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyFileToClipboard))
                {
                    ClipboardHelpers.CopyFile(Info.FilePath);
                }
                else if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyFilePathToClipboard))
                {
                    ClipboardHelpers.CopyText(Info.FilePath);
                }

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ShowInExplorer))
                {
                    Helpers.OpenFolderWithFile(Info.FilePath);
                }

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ScanQRCode) && Info.DataType == EDataType.Image)
                {
                    QRCodeForm.OpenFormDecodeFromFile(Info.FilePath).ShowDialog();
                }
            }
        }

        private void DoTextJobs()
        {
            if (Info.TaskSettings.AdvancedSettings.TextTaskSaveAsFile)
            {
                string filePath = TaskHelpers.HandleExistsFile(Info.TaskSettings.GetScreenshotsFolder(), Info.FileName, Info.TaskSettings);

                if (!string.IsNullOrEmpty(filePath))
                {
                    Info.FilePath = filePath;
                    Helpers.CreateDirectoryFromFilePath(Info.FilePath);
                    File.WriteAllText(Info.FilePath, Text, Encoding.UTF8);
                    DebugHelper.WriteLine("Text saved to file: " + Info.FilePath);
                }
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(Text);
            Data = new MemoryStream(byteArray);
        }

        private void DoAfterUploadJobs()
        {
            try
            {
                if (Info.TaskSettings.UploadSettings.URLRegexReplace)
                {
                    Info.Result.URL = Regex.Replace(Info.Result.URL, Info.TaskSettings.UploadSettings.URLRegexReplacePattern,
                        Info.TaskSettings.UploadSettings.URLRegexReplaceReplacement);
                }

                if (Info.TaskSettings.AdvancedSettings.ResultForceHTTPS)
                {
                    Info.Result.ForceHTTPS();
                }

                if (Info.Job != TaskJob.ShareURL && (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.UseURLShortener) || Info.Job == TaskJob.ShortenURL ||
                    (Info.TaskSettings.AdvancedSettings.AutoShortenURLLength > 0 && Info.Result.URL.Length > Info.TaskSettings.AdvancedSettings.AutoShortenURLLength)))
                {
                    UploadResult result = ShortenURL(Info.Result.URL);

                    if (result != null)
                    {
                        Info.Result.ShortenedURL = result.ShortenedURL;
                        Info.Result.Errors.AddRange(result.Errors);
                    }
                }

                if (Info.Job != TaskJob.ShortenURL && (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.ShareURL) || Info.Job == TaskJob.ShareURL))
                {
                    UploadResult result = ShareURL(Info.Result.ToString());

                    if (result != null)
                    {
                        Info.Result.Errors.AddRange(result.Errors);
                    }

                    if (Info.Job == TaskJob.ShareURL)
                    {
                        Info.Result.IsURLExpected = false;
                    }
                }

                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.CopyURLToClipboard))
                {
                    string txt;

                    if (!string.IsNullOrEmpty(Info.TaskSettings.AdvancedSettings.ClipboardContentFormat))
                    {
                        txt = new UploadInfoParser().Parse(Info, Info.TaskSettings.AdvancedSettings.ClipboardContentFormat);
                    }
                    else
                    {
                        txt = Info.Result.ToString();
                    }

                    if (!string.IsNullOrEmpty(txt))
                    {
                        ClipboardHelpers.CopyText(txt);
                    }
                }

                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.OpenURL))
                {
                    string result;

                    if (!string.IsNullOrEmpty(Info.TaskSettings.AdvancedSettings.OpenURLFormat))
                    {
                        result = new UploadInfoParser().Parse(Info, Info.TaskSettings.AdvancedSettings.OpenURLFormat);
                    }
                    else
                    {
                        result = Info.Result.ToString();
                    }

                    URLHelpers.OpenURL(result);
                }

                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.ShowQRCode))
                {
                    threadWorker.InvokeAsync(() => new QRCodeForm(Info.Result.ToString()).Show());
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                AddErrorMessage(e.ToString());
            }
        }

        public UploadResult UploadData(IGenericUploaderService service, Stream stream, string fileName)
        {
            if (!service.CheckConfig(Program.UploadersConfig))
            {
                return GetInvalidConfigResult(service);
            }

            uploader = service.CreateUploader(Program.UploadersConfig, taskReferenceHelper);

            if (uploader != null)
            {
                uploader.BufferSize = (int)Math.Pow(2, Program.Settings.BufferSizePower) * 1024;
                uploader.ProgressChanged += uploader_ProgressChanged;

                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.CopyURLToClipboard) && Info.TaskSettings.AdvancedSettings.EarlyCopyURL)
                {
                    uploader.EarlyURLCopyRequested += url =>
                    {
                        ClipboardHelpers.CopyText(url);
                        EarlyURLCopied = true;
                    };
                }

                fileName = URLHelpers.RemoveBidiControlCharacters(fileName);

                if (Info.TaskSettings.UploadSettings.FileUploadReplaceProblematicCharacters)
                {
                    fileName = URLHelpers.ReplaceReservedCharacters(fileName, "_");
                }

                Info.UploadDuration = Stopwatch.StartNew();

                UploadResult result = uploader.Upload(stream, fileName);

                Info.UploadDuration.Stop();

                return result;
            }

            return null;
        }

        private bool CheckUploadFilters(Stream stream, string filename)
        {
            if (Info.TaskSettings.UploadSettings.UploaderFilters != null && !string.IsNullOrEmpty(filename) && stream != null)
            {
                UploaderFilter filter = Info.TaskSettings.UploadSettings.UploaderFilters.FirstOrDefault(x => x.IsValidFilter(filename, stream));

                if (filter != null)
                {
                    IGenericUploaderService service = filter.GetUploaderService();

                    if (service != null)
                    {
                        Info.Result = UploadData(service, stream, filename);

                        return true;
                    }
                }
            }

            return false;
        }

        public UploadResult UploadImage(Stream stream, string fileName)
        {
            ImageUploaderService service = UploaderFactory.ImageUploaderServices[Info.TaskSettings.ImageDestination];

            return UploadData(service, stream, fileName);
        }

        public UploadResult UploadText(Stream stream, string fileName)
        {
            TextUploaderService service = UploaderFactory.TextUploaderServices[Info.TaskSettings.TextDestination];

            return UploadData(service, stream, fileName);
        }

        public UploadResult UploadFile(Stream stream, string fileName)
        {
            FileUploaderService service = UploaderFactory.FileUploaderServices[Info.TaskSettings.GetFileDestinationByDataType(Info.DataType)];

            return UploadData(service, stream, fileName);
        }

        public UploadResult ShortenURL(string url)
        {
            URLShortenerService service = UploaderFactory.URLShortenerServices[Info.TaskSettings.URLShortenerDestination];

            if (!service.CheckConfig(Program.UploadersConfig))
            {
                return GetInvalidConfigResult(service);
            }

            URLShortener urlShortener = service.CreateShortener(Program.UploadersConfig, taskReferenceHelper);

            if (urlShortener != null)
            {
                return urlShortener.ShortenURL(url);
            }

            return null;
        }

        public UploadResult ShareURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                URLSharingService service = UploaderFactory.URLSharingServices[Info.TaskSettings.URLSharingServiceDestination];

                if (!service.CheckConfig(Program.UploadersConfig))
                {
                    return GetInvalidConfigResult(service);
                }

                URLSharer urlSharer = service.CreateSharer(Program.UploadersConfig, taskReferenceHelper);

                if (urlSharer != null)
                {
                    return urlSharer.ShareURL(url);
                }
            }

            return null;
        }

        private UploadResult GetInvalidConfigResult(IUploaderService uploaderService)
        {
            UploadResult ur = new UploadResult();

            string message = string.Format(Resources.WorkerTask_GetInvalidConfigResult__0__configuration_is_invalid_or_missing__Please_check__Destination_settings__window_to_configure_it_,
                uploaderService.ServiceName);
            DebugHelper.WriteLine(message);
            ur.Errors.Add(message);

            OnUploadersConfigWindowRequested(uploaderService);

            return ur;
        }

        private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>() // list is on https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
        {
            {"audio/aac", "aac"},
            {"application/x-abiword", "abw"},
            {"application/x-freearc", "arc"},
            {"video/x-msvideo", "avi"},
            {"application/vnd.amazon.ebook", "azw"},
            {"application/octet-stream", "bin"},
            {"image/bmp", "bmp"},
            {"application/x-bzip", "bz"},
            {"application/x-bzip2", "bz2"},
            {"application/x-csh", "csh"},
            {"text/css", "css"},
            {"text/csv", "csv"},
            {"application/msword", "doc"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"},
            {"application/vnd.ms-fontobject", "eot"},
            {"application/epub+zip", "epub"},
            {"application/gzip", "gz"},
            {"image/gif", "gif"},
            {"text/html", "html"},
            {"image/vnd.microsoft.icon", "ico"},
            {"text/calendar", "ics"},
            {"application/java-archive", "jar"},
            {"image/jpeg", "jpg"},
            {"text/javascript", "js"},
            {"application/json", "json"},
            {"application/ld+json", "jsonld"},
            {"audio/midi", "midi"},
            {"audio/x-midi", "midi"},
            {"audio/mpeg", "mp3"},
            {"application/x-cdf", "cda"},
            {"video/mp4", "mp4"},
            {"video/mpeg", "mpeg"},
            {"application/vnd.apple.installer+xml", "mpkg"},
            {"application/vnd.oasis.opendocument.presentation", "odp"},
            {"application/vnd.oasis.opendocument.spreadsheet", "ods"},
            {"application/vnd.oasis.opendocument.text", "odt"},
            {"audio/ogg", "oga"},
            {"video/ogg", "ogv"},
            {"application/ogg", "ogx"},
            {"audio/opus", "opus"},
            {"font/otf", "otf"},
            {"image/png", "png"},
            {"application/pdf", "pdf"},
            {"application/x-httpd-php", "php"},
            {"application/vnd.ms-powerpoint", "ppt"},
            {"application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx"},
            {"application/vnd.rar", "rar"},
            {"application/rtf", "rtf"},
            {"application/x-sh", "sh"},
            {"image/svg+xml", "svg"},
            {"application/x-shockwave-flash", "swf"},
            {"application/x-tar", "tar"},
            {"image/tiff", "tif"},
            {"video/mp2t", "ts"},
            {"font/ttf", "ttf"},
            {"text/plain", "txt"},
            {"application/vnd.visio", "vsd"},
            {"audio/wav", "wav"},
            {"audio/webm", "weba"},
            {"video/webm", "webm"},
            {"image/webp", "webp"},
            {"font/woff", "woff"},
            {"font/woff2", "woff2"},
            {"application/xhtml+xml", "xthml"},
            {"application/vnd.ms-excel", "xls"},
            {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"},
            {"application/xml", "xml"},
            {"text/xml", "xml"},
            {"application/vnd.mozilla.xul+xml", "xul"},
            {"application/zip", "zip"},
            {"video/3gpp", "3gp"},
            {"audio/3gpp", "3gp"},
            {"video/3gpp2", "3g2"},
            {"audio/3gpp2", "3g2"},
            {"application/x-7z-compressed", "7z"}
        };

        private bool DownloadFromURL(bool upload)
        {
            string url = Info.Result.URL.Trim();
            Info.Result.URL = "";
            Info.FilePath = TaskHelpers.HandleExistsFile(Info.TaskSettings.GetScreenshotsFolder(), Info.FileName, Info.TaskSettings);

            if (!string.IsNullOrEmpty(Info.FilePath))
            {
                Info.Status = Resources.UploadTask_DownloadAndUpload_Downloading;
                OnStatusChanged();

                try
                {
                    Helpers.CreateDirectoryFromFilePath(Info.FilePath);

                    IWebProxy proxy = HelpersOptions.CurrentProxy.GetWebProxy();
                    using (HttpClientHandler handler = new HttpClientHandler
                    {
                        Proxy = proxy,
                        UseProxy = proxy != null
                    })
                    {
                        using (HttpClient httpClient = new HttpClient(handler))
                        {
                            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                            requestMessage.Headers.Add("user-agent", ShareXResources.UserAgent);
                            var resp = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                            IEnumerable<string> headers;
                            if (resp.Content.Headers.TryGetValues("content-disposition", out headers))
                            {
                                string fileName = headers.First();
                                if (fileName.Contains("filename="))
                                {
                                    int pos = fileName.IndexOf("filename=");
                                    if (pos != -1)
                                    {
                                        fileName = fileName.Substring(pos+9);
                                        int pos2 = fileName.IndexOf(';');
                                        if (pos2 != -1)
                                        {
                                            fileName = fileName.Substring(0, pos2);
                                        }
                                    }

                                    Info.FileName = fileName;
                                    Info.DataType = TaskHelpers.FindDataType(fileName, Info.TaskSettings);
                                }
                            }

                            if (resp.Content.Headers.TryGetValues("content-type", out headers))
                            {
                                string mimeType = headers.First();
                                if (MimeTypes.ContainsKey(mimeType))
                                {
                                    string ext = MimeTypes[mimeType];
                                    int pos = Info.FileName.LastIndexOf('.');
                                    Info.FileName = pos != -1 ? Info.FileName.Substring(0, pos) + '.' + ext : Info.FileName + '.' + ext;
                                }
                            }

                            int posF = Info.FilePath.LastIndexOf('\\');
                            if (posF != -1)
                            {
                                Info.FilePath = Info.FilePath.Substring(0, posF + 1) + Info.FileName;
                            }

                            File.WriteAllBytes(Info.FilePath, resp.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult());
                        }
                    }

                    /*
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.UserAgent, ShareXResources.UserAgent);
                        wc.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
                        wc.DownloadFile(url, Info.FilePath);
                    }
                    */

                    if (upload)
                    {
                        LoadFileStream();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    MessageBox.Show(string.Format(Resources.UploadManager_DownloadAndUploadFile_Download_failed, e), "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return false;
        }

        private void DoOCR()
        {
            if (Image != null && Info.DataType == EDataType.Image)
            {
                _ = TaskHelpers.OCRImage(Image, Info.TaskSettings);
            }
        }

        private bool LoadFileStream()
        {
            try
            {
                Data = new FileStream(Info.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception e)
            {
                e.ShowError();
                return false;
            }

            return true;
        }

        private void ThreadCompleted()
        {
            OnTaskCompleted();
        }

        private void uploader_ProgressChanged(ProgressManager progress)
        {
            if (progress != null)
            {
                Info.Progress = progress;

                OnUploadProgressChanged();
            }
        }

        private void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                threadWorker.InvokeAsync(() => StatusChanged(this));
            }
        }

        private void OnImageReady()
        {
            if (ImageReady != null)
            {
                Bitmap image = null;

                if (Program.Settings.TaskViewMode == TaskViewMode.ThumbnailView && Image != null)
                {
                    image = (Bitmap)Image.Clone();
                }

                threadWorker.InvokeAsync(() =>
                {
                    using (image)
                    {
                        ImageReady(this, image);
                    }
                });
            }
        }

        private void OnUploadStarted()
        {
            if (UploadStarted != null)
            {
                threadWorker.InvokeAsync(() => UploadStarted(this));
            }
        }

        private void OnUploadCompleted()
        {
            if (UploadCompleted != null)
            {
                threadWorker.InvokeAsync(() => UploadCompleted(this));
            }
        }

        private void OnUploadProgressChanged()
        {
            if (UploadProgressChanged != null)
            {
                threadWorker.InvokeAsync(() => UploadProgressChanged(this));
            }
        }

        private void OnTaskCompleted()
        {
            Info.TaskEndTime = DateTime.Now;

            if (StopRequested)
            {
                Status = TaskStatus.Stopped;
                Info.Status = Resources.UploadTask_OnUploadCompleted_Stopped;
            }
            else if (Info.Result.IsError)
            {
                Status = TaskStatus.Failed;
                Info.Status = Resources.TaskManager_task_UploadCompleted_Error;
            }
            else
            {
                Status = TaskStatus.Completed;
                Info.Status = Resources.UploadTask_OnUploadCompleted_Done;
            }

            if (TaskCompleted != null)
            {
                TaskCompleted(this);
            }

            Dispose();
        }

        private void OnUploadersConfigWindowRequested(IUploaderService uploaderService)
        {
            if (UploadersConfigWindowRequested != null)
            {
                threadWorker.InvokeAsync(() => UploadersConfigWindowRequested(uploaderService));
            }
        }

        public void Dispose()
        {
            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }

            if (!KeepImage && Image != null)
            {
                Image.Dispose();
                Image = null;
            }
        }
    }
}