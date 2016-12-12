#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public class WorkerTask : IDisposable
    {
        public delegate void TaskEventHandler(WorkerTask task);
        public delegate void UploaderServiceEventHandler(IUploaderService uploaderService);

        public event TaskEventHandler StatusChanged, UploadStarted, UploadProgressChanged, UploadCompleted, TaskCompleted;
        public event UploaderServiceEventHandler UploadersConfigWindowRequested;

        public TaskInfo Info { get; private set; }
        public TaskStatus Status { get; private set; }
        public bool IsBusy => Status == TaskStatus.InQueue || IsWorking;
        public bool IsWorking => Status == TaskStatus.Preparing || Status == TaskStatus.Working || Status == TaskStatus.Stopping;
        public bool StopRequested { get; private set; }
        public bool RequestSettingUpdate { get; private set; }
        public Stream Data { get; private set; }

        private Image tempImage;
        private string tempText;
        private ThreadWorker threadWorker;
        private GenericUploader uploader;
        private TaskReferenceHelper taskReferenceHelper;

        private static string lastSaveAsFolder;

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
            task.Info.TaskEndTime = recentTask.Time.ToLocalTime();

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
                string ext = Path.GetExtension(task.Info.FilePath);
                task.Info.FileName = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            if (task.Info.TaskSettings.AdvancedSettings.ProcessImagesDuringFileUpload && task.Info.DataType == EDataType.Image)
            {
                task.Info.Job = TaskJob.Job;
                task.tempImage = ImageHelpers.LoadImage(task.Info.FilePath);
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

        public static WorkerTask CreateImageUploaderTask(Image image, TaskSettings taskSettings, string customFileName = null)
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
                task.Info.FileName = TaskHelpers.GetFilename(taskSettings, "bmp", image);
            }

            task.tempImage = image;
            return task;
        }

        public static WorkerTask CreateTextUploaderTask(string text, TaskSettings taskSettings)
        {
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = TaskJob.TextUpload;
            task.Info.DataType = EDataType.Text;
            task.Info.FileName = TaskHelpers.GetFilename(taskSettings, taskSettings.AdvancedSettings.TextFileExtension);
            task.tempText = text;
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
                string ext = Path.GetExtension(task.Info.FilePath);
                task.Info.FileName = Helpers.AppendExtension(customFileName, ext);
            }
            else if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Path.GetExtension(task.Info.FilePath);
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
            WorkerTask task = new WorkerTask(taskSettings);
            task.Info.Job = upload ? TaskJob.DownloadUpload : TaskJob.Download;
            task.Info.DataType = TaskHelpers.FindDataType(url, taskSettings);

            string filename = URLHelpers.URLDecode(url, 10);
            filename = URLHelpers.GetFileName(filename);
            filename = Helpers.GetValidFileName(filename);

            if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Path.GetExtension(filename);
                filename = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            task.Info.FileName = filename;
            task.Info.Result.URL = url;
            return task;
        }

        #endregion Constructors

        public void Start()
        {
            if (Status == TaskStatus.InQueue && !StopRequested)
            {
                Info.TaskStartTime = DateTime.UtcNow;

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

            TaskbarManager.SetProgressState(Program.MainForm, TaskbarProgressBarStatus.Indeterminate);

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

        private void ThreadDoWork()
        {
            CreateTaskReferenceHelper();

            try
            {
                StopRequested = !DoThreadJob();

                if (!StopRequested)
                {
                    if (Info.IsUploadJob && !Program.Settings.DisableUpload)
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
                Dispose();

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
            if (Program.Settings.ShowUploadWarning && MessageBox.Show(
                Resources.UploadTask_DoUploadJob_First_time_upload_warning_text,
                "ShareX - " + Resources.UploadTask_DoUploadJob_First_time_upload_warning,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                Program.Settings.ShowUploadWarning = false;
                Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.UploadImageToHost);
                RequestSettingUpdate = true;
                Stop();
            }

            if (Program.Settings.LargeFileSizeWarning > 0)
            {
                long dataSize = Program.Settings.BinaryUnits ? Program.Settings.LargeFileSizeWarning * 1024 * 1024 : Program.Settings.LargeFileSizeWarning * 1000 * 1000;
                if (Data != null && Data.Length > dataSize)
                {
                    using (MyMessageBox msgbox = new MyMessageBox(Resources.UploadTask_DoUploadJob_You_are_attempting_to_upload_a_large_file, "ShareX",
                        MessageBoxButtons.YesNo, Resources.UploadManager_IsUploadConfirmed_Don_t_show_this_message_again_))
                    {
                        msgbox.ShowDialog();
                        if (msgbox.IsChecked) Program.Settings.LargeFileSizeWarning = 0;
                        if (msgbox.DialogResult == DialogResult.No) Stop();
                    }
                }
            }

            if (!StopRequested)
            {
                Program.Settings.ShowUploadWarning = false;

                if (Program.UploadersConfig == null)
                {
                    Program.UploaderSettingsResetEvent.WaitOne();
                }

                Status = TaskStatus.Working;
                Info.Status = Resources.UploadTask_DoUploadJob_Uploading;

                TaskbarManager.SetProgressState(Program.MainForm, TaskbarProgressBarStatus.Normal);

                bool cancelUpload = false;

                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ShowBeforeUploadWindow))
                {
                    BeforeUploadForm form = new BeforeUploadForm(Info);
                    cancelUpload = form.ShowDialog() != DialogResult.OK;
                }

                if (!cancelUpload)
                {
                    OnUploadStarted();

                    bool isError = DoUpload();

                    if (isError && Program.Settings.MaxUploadFailRetry > 0)
                    {
                        DebugHelper.WriteLine("Upload failed. Retrying upload.");

                        for (int retry = 1; isError && retry <= Program.Settings.MaxUploadFailRetry; retry++)
                        {
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
            else if (Info.Job == TaskJob.TextUpload && !string.IsNullOrEmpty(tempText))
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
            if (tempImage == null)
            {
                return true;
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects))
            {
                tempImage = TaskHelpers.AddImageEffects(tempImage, Info.TaskSettings);

                if (tempImage == null)
                {
                    DebugHelper.WriteLine("Error: Applying image effects resulted empty image.");
                    return false;
                }
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AnnotateImage))
            {
                tempImage = TaskHelpers.AnnotateImageUsingGreenshot(tempImage, Info.FileName);

                if (tempImage == null)
                {
                    return false;
                }
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyImageToClipboard))
            {
                ClipboardHelpers.CopyImage(tempImage);
                DebugHelper.WriteLine("Image copied to clipboard.");
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SendImageToPrinter))
            {
                TaskHelpers.PrintImage(tempImage);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlagAny(AfterCaptureTasks.SaveImageToFile, AfterCaptureTasks.SaveImageToFileWithDialog, AfterCaptureTasks.DoOCR,
                AfterCaptureTasks.UploadImageToHost))
            {
                using (tempImage)
                {
                    ImageData imageData = TaskHelpers.PrepareImage(tempImage, Info.TaskSettings);
                    Data = imageData.ImageStream;
                    Info.FileName = Path.ChangeExtension(Info.FileName, imageData.ImageFormat.GetDescription());

                    if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SaveImageToFile))
                    {
                        string filePath = TaskHelpers.CheckFilePath(Info.TaskSettings.CaptureFolder, Info.FileName, Info.TaskSettings);

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
                            bool imageSaved;

                            do
                            {
                                if (string.IsNullOrEmpty(lastSaveAsFolder) || !Directory.Exists(lastSaveAsFolder))
                                {
                                    lastSaveAsFolder = Info.TaskSettings.CaptureFolder;
                                }

                                sfd.InitialDirectory = lastSaveAsFolder;
                                sfd.FileName = Info.FileName;
                                sfd.DefaultExt = Path.GetExtension(Info.FileName).Substring(1);
                                sfd.Filter = string.Format("*{0}|*{0}|All files (*.*)|*.*", Path.GetExtension(Info.FileName));
                                sfd.Title = Resources.UploadTask_DoAfterCaptureJobs_Choose_a_folder_to_save + " " + Path.GetFileName(Info.FileName);

                                if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                                {
                                    Info.FilePath = sfd.FileName;
                                    lastSaveAsFolder = Path.GetDirectoryName(Info.FilePath);
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
                            thumbnailFolder = Info.TaskSettings.CaptureFolder;
                        }

                        Info.ThumbnailFilePath = TaskHelpers.CreateThumbnail(tempImage, thumbnailFolder, thumbnailFilename, Info.TaskSettings);

                        if (!string.IsNullOrEmpty(Info.ThumbnailFilePath))
                        {
                            DebugHelper.WriteLine("Thumbnail saved to file: " + Info.ThumbnailFilePath);
                        }
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
                        if (Data != null)
                        {
                            Data.Dispose();
                        }

                        foreach (ExternalProgram fileAction in actions)
                        {
                            Info.FilePath = fileAction.Run(Info.FilePath);
                        }

                        LoadFileStream();
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
            }
        }

        private void DoTextJobs()
        {
            if (Info.TaskSettings.AdvancedSettings.TextTaskSaveAsFile)
            {
                string filePath = TaskHelpers.CheckFilePath(Info.TaskSettings.CaptureFolder, Info.FileName, Info.TaskSettings);

                if (!string.IsNullOrEmpty(filePath))
                {
                    Info.FilePath = filePath;
                    Helpers.CreateDirectoryFromFilePath(Info.FilePath);
                    File.WriteAllText(Info.FilePath, tempText, Encoding.UTF8);
                    DebugHelper.WriteLine("Text saved to file: " + Info.FilePath);
                }
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(tempText);
            Data = new MemoryStream(byteArray);
        }

        private void DoAfterUploadJobs()
        {
            try
            {
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
                    uploader.EarlyURLCopyRequested += url => ClipboardHelpers.CopyText(url);
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

                service.ShareURL(url, Program.UploadersConfig);

                return new UploadResult() { URL = url };
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

        private bool DownloadFromURL(bool upload)
        {
            string url = Info.Result.URL.Trim();
            Info.Result.URL = "";
            Info.FilePath = TaskHelpers.CheckFilePath(Info.TaskSettings.CaptureFolder, Info.FileName, Info.TaskSettings);

            if (!string.IsNullOrEmpty(Info.FilePath))
            {
                Info.Status = Resources.UploadTask_DownloadAndUpload_Downloading;
                OnStatusChanged();

                try
                {
                    Helpers.CreateDirectoryFromFilePath(Info.FilePath);

                    using (WebClient wc = new WebClient())
                    {
                        wc.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
                        wc.DownloadFile(url, Info.FilePath);
                    }

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
            if (Data != null && Info.DataType == EDataType.Image)
            {
                TaskHelpers.OCRImage(Data, Info.FileName);
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
                MessageBox.Show(e.Message, "ShareX - " + Resources.TaskManager_task_UploadCompleted_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Info.TaskEndTime = DateTime.UtcNow;

            Status = TaskStatus.Completed;

            if (StopRequested)
            {
                Info.Status = Resources.UploadTask_OnUploadCompleted_Stopped;
            }
            else
            {
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

            if (tempImage != null)
            {
                tempImage.Dispose();
                tempImage = null;
            }
        }
    }
}