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
using ShareX.UploadersLib;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.GUI;
using ShareX.UploadersLib.HelperClasses;
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.TextUploaders;
using ShareX.UploadersLib.URLShorteners;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public class UploadTask : IDisposable
    {
        public delegate void TaskEventHandler(UploadTask task);

        public event TaskEventHandler StatusChanged;
        public event TaskEventHandler UploadStarted;
        public event TaskEventHandler UploadProgressChanged;
        public event TaskEventHandler UploadCompleted;

        public TaskInfo Info { get; private set; }

        public TaskStatus Status { get; private set; }

        public bool IsWorking
        {
            get
            {
                return Status != TaskStatus.InQueue && Status != TaskStatus.Completed;
            }
        }

        public bool StopRequested { get; private set; }
        public bool RequestSettingUpdate { get; private set; }

        public Stream Data { get; set; }

        private Image tempImage;
        private string tempText;
        private ThreadWorker threadWorker;
        private Uploader uploader;

        private static string lastSaveAsFolder;

        #region Constructors

        private UploadTask(TaskSettings taskSettings)
        {
            Status = TaskStatus.InQueue;
            Info = new TaskInfo(taskSettings);
        }

        public static UploadTask CreateDataUploaderTask(EDataType dataType, Stream stream, string fileName, TaskSettings taskSettings)
        {
            UploadTask task = new UploadTask(taskSettings);
            task.Info.Job = TaskJob.DataUpload;
            task.Info.DataType = dataType;
            task.Info.FileName = fileName;
            task.Data = stream;
            return task;
        }

        public static UploadTask CreateFileUploaderTask(string filePath, TaskSettings taskSettings)
        {
            EDataType dataType = TaskHelpers.FindDataType(filePath, taskSettings);
            UploadTask task = new UploadTask(taskSettings);

            task.Info.DataType = dataType;
            task.Info.FilePath = filePath;

            if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Path.GetExtension(task.Info.FilePath);
                task.Info.FileName = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            if (task.Info.TaskSettings.AdvancedSettings.ProcessImagesDuringFileUpload && dataType == EDataType.Image)
            {
                task.Info.Job = TaskJob.Job;
                task.tempImage = ImageHelpers.LoadImage(filePath);
            }
            else
            {
                task.Info.Job = TaskJob.FileUpload;

                try
                {
                    task.Data = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception e)
                {
                    MessageBox.Show("ShareX - " + Resources.TaskManager_task_UploadCompleted_Error, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }

            return task;
        }

        public static UploadTask CreateImageUploaderTask(Image image, TaskSettings taskSettings)
        {
            UploadTask task = new UploadTask(taskSettings);

            task.Info.Job = TaskJob.Job;
            task.Info.DataType = EDataType.Image;
            task.Info.FileName = TaskHelpers.GetImageFilename(taskSettings, image);
            task.tempImage = image;
            return task;
        }

        public static UploadTask CreateTextUploaderTask(string text, TaskSettings taskSettings)
        {
            UploadTask task = new UploadTask(taskSettings);
            task.Info.Job = TaskJob.TextUpload;
            task.Info.DataType = EDataType.Text;
            task.Info.FileName = TaskHelpers.GetFilename(taskSettings, taskSettings.AdvancedSettings.TextFileExtension);
            task.tempText = text;
            return task;
        }

        public static UploadTask CreateURLShortenerTask(string url, TaskSettings taskSettings)
        {
            UploadTask task = new UploadTask(taskSettings);
            task.Info.Job = TaskJob.ShortenURL;
            task.Info.DataType = EDataType.URL;
            task.Info.FileName = string.Format(Resources.UploadTask_CreateURLShortenerTask_Shorten_URL___0__, taskSettings.URLShortenerDestination.GetLocalizedDescription());
            task.Info.Result.URL = url;
            return task;
        }

        public static UploadTask CreateShareURLTask(string url, TaskSettings taskSettings)
        {
            UploadTask task = new UploadTask(taskSettings);
            task.Info.Job = TaskJob.ShareURL;
            task.Info.DataType = EDataType.URL;
            task.Info.FileName = string.Format(Resources.UploadTask_CreateShareURLTask_Share_URL___0__, taskSettings.URLSharingServiceDestination.GetLocalizedDescription());
            task.Info.Result.URL = url;
            return task;
        }

        public static UploadTask CreateFileJobTask(string filePath, TaskSettings taskSettings)
        {
            EDataType dataType = TaskHelpers.FindDataType(filePath, taskSettings);
            UploadTask task = new UploadTask(taskSettings);

            task.Info.DataType = dataType;
            task.Info.FilePath = filePath;

            if (task.Info.TaskSettings.UploadSettings.FileUploadUseNamePattern)
            {
                string ext = Path.GetExtension(task.Info.FilePath);
                task.Info.FileName = TaskHelpers.GetFilename(task.Info.TaskSettings, ext);
            }

            task.Info.Job = TaskJob.Job;

            if (task.Info.IsUploadJob)
            {
                task.Data = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            return task;
        }

        #endregion Constructors

        public void Start()
        {
            if (Status == TaskStatus.InQueue && !StopRequested)
            {
                Prepare();
                threadWorker = new ThreadWorker();
                threadWorker.DoWork += ThreadDoWork;
                threadWorker.Completed += ThreadCompleted;
                threadWorker.Start(ApartmentState.STA);
            }
        }

        public void StartSync()
        {
            if (Status == TaskStatus.InQueue && !StopRequested)
            {
                Prepare();
                ThreadDoWork();
                ThreadCompleted();
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
                    OnUploadCompleted();
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
            Info.StartTime = DateTime.UtcNow;

            try
            {
                StopRequested = !DoThreadJob();

                if (!StopRequested)
                {
                    DoUploadJob();
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
                    Info.Result.Errors.Add(Resources.UploadTask_ThreadDoWork_URL_is_empty_);
                }
                else
                {
                    DoAfterUploadJobs();
                }
            }

            Info.UploadTime = DateTime.UtcNow;
        }

        private void DoUploadJob()
        {
            if (Info.IsUploadJob)
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
                else
                {
                    Program.Settings.ShowUploadWarning = false;

                    if (Program.UploadersConfig == null)
                    {
                        Program.UploaderSettingsResetEvent.WaitOne();
                    }

                    Status = TaskStatus.Working;
                    Info.Status = Resources.UploadTask_DoUploadJob_Uploading;

                    TaskbarManager.SetProgressState(Program.MainForm, TaskbarProgressBarStatus.Normal);

                    DialogResult beforeUploadResult = DialogResult.OK;

                    if (Info.TaskSettings.GeneralSettings.ShowBeforeUploadForm)
                    {
                        BeforeUploadForm form = new BeforeUploadForm(Info);
                        beforeUploadResult = form.ShowDialog();
                    }

                    if (beforeUploadResult == DialogResult.OK)
                    {
                        if (threadWorker != null)
                        {
                            threadWorker.InvokeAsync(OnUploadStarted);
                        }
                        else
                        {
                            OnUploadStarted();
                        }

                        bool isError = DoUpload();

                        if (isError && Program.Settings.MaxUploadFailRetry > 0)
                        {
                            DebugHelper.WriteLine("Upload failed. Retrying upload.");

                            for (int retry = 1; isError && retry <= Program.Settings.MaxUploadFailRetry; retry++)
                            {
                                isError = DoUpload(retry);
                            }
                        }
                    }
                    else if (beforeUploadResult == DialogResult.Cancel)
                    {
                        Info.Result.IsURLExpected = false;
                    }
                }
            }
            else
            {
                Info.Result.IsURLExpected = false;
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

            try
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
            catch (Exception e)
            {
                if (!StopRequested)
                {
                    DebugHelper.WriteException(e);
                    isError = true;
                    if (Info.Result == null) Info.Result = new UploadResult();
                    Info.Result.Errors.Add(e.ToString());
                }
            }
            finally
            {
                if (Info.Result == null) Info.Result = new UploadResult();
                if (uploader != null) Info.Result.Errors.AddRange(uploader.Errors);
                isError |= Info.Result.IsError;
            }

            return isError;
        }

        private bool DoThreadJob()
        {
            if (Info.IsUploadJob && Info.TaskSettings.AdvancedSettings.AutoClearClipboard)
            {
                ClipboardHelpers.Clear();
            }

            if (Info.Job == TaskJob.Job)
            {
                if (tempImage != null && !DoAfterCaptureJobs())
                {
                    return false;
                }

                DoFileJobs();
            }
            else if (Info.Job == TaskJob.TextUpload && !string.IsNullOrEmpty(tempText))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(tempText);
                Data = new MemoryStream(byteArray);
            }

            if (Info.IsUploadJob && Data != null && Data.CanSeek)
            {
                Data.Position = 0;
            }

            return true;
        }

        private bool DoAfterCaptureJobs()
        {
            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects))
            {
                tempImage = TaskHelpers.AddImageEffects(tempImage, Info.TaskSettings);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AnnotateImage))
            {
                tempImage = TaskHelpers.AnnotateImage(tempImage, Info.FileName);

                if (tempImage == null)
                {
                    return false;
                }
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyImageToClipboard))
            {
                ClipboardHelpers.CopyImage(tempImage);
                DebugHelper.WriteLine("CopyImageToClipboard");
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SendImageToPrinter))
            {
                TaskHelpers.PrintImage(tempImage);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlagAny(AfterCaptureTasks.SaveImageToFile, AfterCaptureTasks.SaveImageToFileWithDialog, AfterCaptureTasks.UploadImageToHost))
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
                            DebugHelper.WriteLine("SaveImageToFile: " + Info.FilePath);
                        }
                    }

                    if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SaveImageToFileWithDialog))
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog())
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
                                imageData.Write(Info.FilePath);
                                DebugHelper.WriteLine("SaveImageToFileWithDialog: " + Info.FilePath);
                            }
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
                            DebugHelper.WriteLine("SaveThumbnailImageToFile: " + Info.ThumbnailFilePath);
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
                    var actions = Info.TaskSettings.ExternalPrograms.Where(x => x.IsActive);

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

                        Data = new FileStream(Info.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
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
            }
        }

        private void DoAfterUploadJobs()
        {
            try
            {
                if (Info.TaskSettings.AdvancedSettings.ResultForceHTTPS)
                {
                    Info.Result.URL = URLHelpers.ForceHTTPS(Info.Result.URL);
                    Info.Result.ThumbnailURL = URLHelpers.ForceHTTPS(Info.Result.ThumbnailURL);
                    Info.Result.DeletionURL = URLHelpers.ForceHTTPS(Info.Result.DeletionURL);
                }

                if (Info.Job != TaskJob.ShareURL && (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.UseURLShortener) || Info.Job == TaskJob.ShortenURL ||
                    (Info.TaskSettings.AdvancedSettings.AutoShortenURLLength > 0 && Info.Result.URL.Length > Info.TaskSettings.AdvancedSettings.AutoShortenURLLength)))
                {
                    UploadResult result = ShortenURL(Info.Result.URL);

                    if (result != null)
                    {
                        Info.Result.ShortenedURL = result.ShortenedURL;
                    }
                }

                if (Info.Job != TaskJob.ShortenURL && (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.ShareURL) || Info.Job == TaskJob.ShareURL))
                {
                    ShareURL(Info.Result.ToString());
                    if (Info.Job == TaskJob.ShareURL) Info.Result.IsURLExpected = false;
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
                if (Info.Result == null) Info.Result = new UploadResult();
                Info.Result.Errors.Add(e.ToString());
            }
        }

        public UploadResult UploadImage(Stream stream, string fileName)
        {
            ImageUploader imageUploader = null;

            switch (Info.TaskSettings.ImageDestination)
            {
                case ImageDestination.Imgur:
                    if (Program.UploadersConfig.ImgurOAuth2Info == null)
                    {
                        Program.UploadersConfig.ImgurOAuth2Info = new OAuth2Info(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);
                    }

                    string albumID = null;

                    if (Program.UploadersConfig.ImgurUploadSelectedAlbum && Program.UploadersConfig.ImgurSelectedAlbum != null)
                    {
                        albumID = Program.UploadersConfig.ImgurSelectedAlbum.id;
                    }

                    imageUploader = new Imgur(Program.UploadersConfig.ImgurOAuth2Info)
                    {
                        UploadMethod = Program.UploadersConfig.ImgurAccountType,
                        DirectLink = Program.UploadersConfig.ImgurDirectLink,
                        ThumbnailType = Program.UploadersConfig.ImgurThumbnailType,
                        UploadAlbumID = albumID
                    };
                    break;
                case ImageDestination.ImageShack:
                    Program.UploadersConfig.ImageShackSettings.ThumbnailWidth = Info.TaskSettings.AdvancedSettings.ThumbnailPreferredWidth;
                    Program.UploadersConfig.ImageShackSettings.ThumbnailHeight = Info.TaskSettings.AdvancedSettings.ThumbnailPreferredHeight;
                    imageUploader = new ImageShackUploader(APIKeys.ImageShackKey, Program.UploadersConfig.ImageShackSettings);
                    break;
                case ImageDestination.TinyPic:
                    imageUploader = new TinyPicUploader(APIKeys.TinyPicID, APIKeys.TinyPicKey, Program.UploadersConfig.TinyPicAccountType, Program.UploadersConfig.TinyPicRegistrationCode);
                    break;
                case ImageDestination.Flickr:
                    imageUploader = new FlickrUploader(APIKeys.FlickrKey, APIKeys.FlickrSecret, Program.UploadersConfig.FlickrAuthInfo, Program.UploadersConfig.FlickrSettings);
                    break;
                case ImageDestination.Photobucket:
                    imageUploader = new Photobucket(Program.UploadersConfig.PhotobucketOAuthInfo, Program.UploadersConfig.PhotobucketAccountInfo);
                    break;
                case ImageDestination.Picasa:
                    imageUploader = new Picasa(Program.UploadersConfig.PicasaOAuth2Info)
                    {
                        AlbumID = Program.UploadersConfig.PicasaAlbumID
                    };
                    break;
                case ImageDestination.Twitter:
                    OAuthInfo twitterOAuth = Program.UploadersConfig.TwitterOAuthInfoList.ReturnIfValidIndex(Program.UploadersConfig.TwitterSelectedAccount);
                    imageUploader = new Twitter(twitterOAuth)
                    {
                        SkipMessageBox = Program.UploadersConfig.TwitterSkipMessageBox,
                        DefaultMessage = Program.UploadersConfig.TwitterDefaultMessage ?? string.Empty
                    };
                    break;
                case ImageDestination.Chevereto:
                    imageUploader = new Chevereto(Program.UploadersConfig.CheveretoWebsite, Program.UploadersConfig.CheveretoAPIKey)
                    {
                        DirectURL = Program.UploadersConfig.CheveretoDirectURL
                    };
                    break;
                case ImageDestination.HizliResim:
                    imageUploader = new HizliResim()
                    {
                        DirectURL = true
                    };
                    break;
                case ImageDestination.Vgyme:
                    imageUploader = new VgymeUploader();
                    break;
                case ImageDestination.CustomImageUploader:
                    CustomUploaderItem customUploader = GetCustomUploader(Program.UploadersConfig.CustomImageUploaderSelected);
                    if (customUploader != null)
                    {
                        imageUploader = new CustomImageUploader(customUploader);
                    }
                    break;
            }

            if (imageUploader != null)
            {
                PrepareUploader(imageUploader);
                return imageUploader.Upload(stream, fileName);
            }

            return null;
        }

        public UploadResult UploadText(Stream stream, string fileName)
        {
            TextUploader textUploader = null;

            switch (Info.TaskSettings.TextDestination)
            {
                case TextDestination.Pastebin:
                    PastebinSettings settings = Program.UploadersConfig.PastebinSettings;
                    if (string.IsNullOrEmpty(settings.TextFormat))
                    {
                        settings.TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat;
                    }
                    textUploader = new Pastebin(APIKeys.PastebinKey, settings);
                    break;
                case TextDestination.Paste2:
                    textUploader = new Paste2(new Paste2Settings { TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat });
                    break;
                case TextDestination.Slexy:
                    textUploader = new Slexy(new SlexySettings { TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat });
                    break;
                case TextDestination.Pastee:
                    textUploader = new Pastee { Lexer = Info.TaskSettings.AdvancedSettings.TextFormat };
                    break;
                case TextDestination.Paste_ee:
                    textUploader = new Paste_ee(Program.UploadersConfig.Paste_eeUserAPIKey);
                    break;
                case TextDestination.Gist:
                    textUploader = Program.UploadersConfig.GistAnonymousLogin ? new Gist(Program.UploadersConfig.GistPublishPublic) :
                        new Gist(Program.UploadersConfig.GistPublishPublic, Program.UploadersConfig.GistOAuth2Info);
                    break;
                case TextDestination.Upaste:
                    textUploader = new Upaste(Program.UploadersConfig.UpasteUserKey)
                    {
                        IsPublic = Program.UploadersConfig.UpasteIsPublic
                    };
                    break;
                case TextDestination.Hastebin:
                    textUploader = new Hastebin()
                    {
                        CustomDomain = Program.UploadersConfig.HastebinCustomDomain,
                        SyntaxHighlighting = Program.UploadersConfig.HastebinSyntaxHighlighting
                    };
                    break;
                case TextDestination.CustomTextUploader:
                    CustomUploaderItem customUploader = GetCustomUploader(Program.UploadersConfig.CustomTextUploaderSelected);
                    if (customUploader != null)
                    {
                        textUploader = new CustomTextUploader(customUploader);
                    }
                    break;
            }

            if (textUploader != null)
            {
                PrepareUploader(textUploader);
                return textUploader.UploadText(stream, fileName);
            }

            return null;
        }

        public UploadResult UploadFile(Stream stream, string fileName)
        {
            FileUploader fileUploader = null;

            FileDestination fileDestination;

            switch (Info.DataType)
            {
                case EDataType.Image:
                    fileDestination = Info.TaskSettings.ImageFileDestination;
                    break;
                case EDataType.Text:
                    fileDestination = Info.TaskSettings.TextFileDestination;
                    break;
                default:
                case EDataType.File:
                    fileDestination = Info.TaskSettings.FileDestination;
                    break;
            }

            switch (fileDestination)
            {
                case FileDestination.Dropbox:
                    fileUploader = new Dropbox(Program.UploadersConfig.DropboxOAuth2Info, Program.UploadersConfig.DropboxAccountInfo)
                    {
                        UploadPath = NameParser.Parse(NameParserType.URL, Dropbox.TidyUploadPath(Program.UploadersConfig.DropboxUploadPath)),
                        AutoCreateShareableLink = Program.UploadersConfig.DropboxAutoCreateShareableLink,
                        ShareURLType = Program.UploadersConfig.DropboxURLType
                    };
                    break;
                case FileDestination.OneDrive:
                    fileUploader = new OneDrive(Program.UploadersConfig.OneDriveOAuth2Info)
                    {
                        FolderID = Program.UploadersConfig.OneDriveSelectedFolder.id,
                        AutoCreateShareableLink = Program.UploadersConfig.OneDriveAutoCreateShareableLink
                    };
                    break;
                case FileDestination.GoogleDrive:
                    fileUploader = new GoogleDrive(Program.UploadersConfig.GoogleDriveOAuth2Info)
                    {
                        IsPublic = Program.UploadersConfig.GoogleDriveIsPublic,
                        FolderID = Program.UploadersConfig.GoogleDriveUseFolder ? Program.UploadersConfig.GoogleDriveFolderID : null
                    };
                    break;
                case FileDestination.Copy:
                    fileUploader = new Copy(Program.UploadersConfig.CopyOAuthInfo, Program.UploadersConfig.CopyAccountInfo)
                    {
                        UploadPath = NameParser.Parse(NameParserType.URL, Copy.TidyUploadPath(Program.UploadersConfig.CopyUploadPath)),
                        URLType = Program.UploadersConfig.CopyURLType
                    };
                    break;
                /*case FileDestination.Hubic:
                    fileUploader = new Hubic(Program.UploadersConfig.HubicOAuth2Info, Program.UploadersConfig.HubicOpenstackAuthInfo)
                    {
                        SelectedFolder = Program.UploadersConfig.HubicSelectedFolder,
                        Publish = Program.UploadersConfig.HubicPublish
                    };
                    break;*/
                case FileDestination.SendSpace:
                    fileUploader = new SendSpace(APIKeys.SendSpaceKey);
                    switch (Program.UploadersConfig.SendSpaceAccountType)
                    {
                        case AccountType.Anonymous:
                            SendSpaceManager.PrepareUploadInfo(APIKeys.SendSpaceKey);
                            break;
                        case AccountType.User:
                            SendSpaceManager.PrepareUploadInfo(APIKeys.SendSpaceKey, Program.UploadersConfig.SendSpaceUsername, Program.UploadersConfig.SendSpacePassword);
                            break;
                    }
                    break;
                case FileDestination.Minus:
                    fileUploader = new Minus(Program.UploadersConfig.MinusConfig, Program.UploadersConfig.MinusOAuth2Info);
                    break;
                case FileDestination.Box:
                    fileUploader = new Box(Program.UploadersConfig.BoxOAuth2Info)
                    {
                        FolderID = Program.UploadersConfig.BoxSelectedFolder.id,
                        Share = Program.UploadersConfig.BoxShare
                    };
                    break;
                case FileDestination.Gfycat:
                    fileUploader = new GfycatUploader();
                    break;
                case FileDestination.Ge_tt:
                    fileUploader = new Ge_tt(APIKeys.Ge_ttKey)
                    {
                        AccessToken = Program.UploadersConfig.Ge_ttLogin.AccessToken
                    };
                    break;
                case FileDestination.Localhostr:
                    fileUploader = new Hostr(Program.UploadersConfig.LocalhostrEmail, Program.UploadersConfig.LocalhostrPassword)
                    {
                        DirectURL = Program.UploadersConfig.LocalhostrDirectURL
                    };
                    break;
                case FileDestination.CustomFileUploader:
                    CustomUploaderItem customUploader = GetCustomUploader(Program.UploadersConfig.CustomFileUploaderSelected);
                    if (customUploader != null)
                    {
                        fileUploader = new CustomFileUploader(customUploader);
                    }
                    break;
                case FileDestination.FTP:
                    FTPAccount ftpAccount = GetFTPAccount(Program.UploadersConfig.GetFTPIndex(Info.DataType));
                    if (ftpAccount != null)
                    {
                        if (ftpAccount.Protocol == FTPProtocol.FTP || ftpAccount.Protocol == FTPProtocol.FTPS)
                        {
                            fileUploader = new FTP(ftpAccount);
                        }
                        else if (ftpAccount.Protocol == FTPProtocol.SFTP)
                        {
                            fileUploader = new SFTP(ftpAccount);
                        }
                    }
                    break;
                case FileDestination.SharedFolder:
                    int idLocalhost = Program.UploadersConfig.GetLocalhostIndex(Info.DataType);
                    if (Program.UploadersConfig.LocalhostAccountList.IsValidIndex(idLocalhost))
                    {
                        fileUploader = new SharedFolderUploader(Program.UploadersConfig.LocalhostAccountList[idLocalhost]);
                    }
                    break;
                case FileDestination.Email:
                    using (EmailForm emailForm = new EmailForm(Program.UploadersConfig.EmailRememberLastTo ? Program.UploadersConfig.EmailLastTo : string.Empty,
                        Program.UploadersConfig.EmailDefaultSubject, Program.UploadersConfig.EmailDefaultBody))
                    {
                        emailForm.Icon = ShareXResources.Icon;

                        if (emailForm.ShowDialog() == DialogResult.OK)
                        {
                            if (Program.UploadersConfig.EmailRememberLastTo)
                            {
                                Program.UploadersConfig.EmailLastTo = emailForm.ToEmail;
                            }

                            fileUploader = new Email
                            {
                                SmtpServer = Program.UploadersConfig.EmailSmtpServer,
                                SmtpPort = Program.UploadersConfig.EmailSmtpPort,
                                FromEmail = Program.UploadersConfig.EmailFrom,
                                Password = Program.UploadersConfig.EmailPassword,
                                ToEmail = emailForm.ToEmail,
                                Subject = emailForm.Subject,
                                Body = emailForm.Body
                            };
                        }
                        else
                        {
                            StopRequested = true;
                        }
                    }
                    break;
                case FileDestination.Jira:
                    fileUploader = new Jira(Program.UploadersConfig.JiraHost, Program.UploadersConfig.JiraOAuthInfo, Program.UploadersConfig.JiraIssuePrefix);
                    break;
                case FileDestination.Mega:
                    fileUploader = new Mega(Program.UploadersConfig.MegaAuthInfos, Program.UploadersConfig.MegaParentNodeId);
                    break;
                case FileDestination.AmazonS3:
                    fileUploader = new AmazonS3(Program.UploadersConfig.AmazonS3Settings);
                    break;
                case FileDestination.OwnCloud:
                    fileUploader = new OwnCloud(Program.UploadersConfig.OwnCloudHost, Program.UploadersConfig.OwnCloudUsername, Program.UploadersConfig.OwnCloudPassword)
                    {
                        Path = Program.UploadersConfig.OwnCloudPath,
                        CreateShare = Program.UploadersConfig.OwnCloudCreateShare,
                        DirectLink = Program.UploadersConfig.OwnCloudDirectLink,
                        IgnoreInvalidCert = Program.UploadersConfig.OwnCloudIgnoreInvalidCert
                    };
                    break;
                case FileDestination.Pushbullet:
                    fileUploader = new Pushbullet(Program.UploadersConfig.PushbulletSettings);
                    break;
                case FileDestination.MediaFire:
                    fileUploader = new MediaFire(APIKeys.MediaFireAppId, APIKeys.MediaFireApiKey, Program.UploadersConfig.MediaFireUsername, Program.UploadersConfig.MediaFirePassword)
                    {
                        UploadPath = NameParser.Parse(NameParserType.URL, Program.UploadersConfig.MediaFirePath),
                        UseLongLink = Program.UploadersConfig.MediaFireUseLongLink
                    };
                    break;
                case FileDestination.Lambda:
                    fileUploader = new Lambda(Program.UploadersConfig.LambdaSettings);
                    break;
                case FileDestination.Imgrush:
                    fileUploader = new MediaCrushUploader("https://imgrush.com");
                    break;
                case FileDestination.VideoBin:
                    fileUploader = new VideoBin();
                    break;
                case FileDestination.MaxFile:
                    fileUploader = new MaxFile();
                    break;
                case FileDestination.Dropfile:
                    fileUploader = new Dropfile();
                    break;
                case FileDestination.Up1:
                    fileUploader = new Up1(Program.UploadersConfig.Up1Host, Program.UploadersConfig.Up1Key);
                    break;
            }

            if (fileUploader != null)
            {
                PrepareUploader(fileUploader);
                return fileUploader.Upload(stream, fileName);
            }

            return null;
        }

        public UploadResult ShortenURL(string url)
        {
            URLShortener urlShortener = null;

            switch (Info.TaskSettings.URLShortenerDestination)
            {
                case UrlShortenerType.BITLY:
                    if (Program.UploadersConfig.BitlyOAuth2Info == null)
                    {
                        Program.UploadersConfig.BitlyOAuth2Info = new OAuth2Info(APIKeys.BitlyClientID, APIKeys.BitlyClientSecret);
                    }

                    urlShortener = new BitlyURLShortener(Program.UploadersConfig.BitlyOAuth2Info)
                    {
                        Domain = Program.UploadersConfig.BitlyDomain
                    };
                    break;
                case UrlShortenerType.Google:
                    urlShortener = new GoogleURLShortener(Program.UploadersConfig.GoogleURLShortenerAccountType, APIKeys.GoogleAPIKey,
                        Program.UploadersConfig.GoogleURLShortenerOAuth2Info);
                    break;
                case UrlShortenerType.ISGD:
                    urlShortener = new IsgdURLShortener();
                    break;
                case UrlShortenerType.VGD:
                    urlShortener = new VgdURLShortener();
                    break;
                case UrlShortenerType.TINYURL:
                    urlShortener = new TinyURLShortener();
                    break;
                case UrlShortenerType.TURL:
                    urlShortener = new TurlURLShortener();
                    break;
                case UrlShortenerType.YOURLS:
                    urlShortener = new YourlsURLShortener
                    {
                        APIURL = Program.UploadersConfig.YourlsAPIURL,
                        Signature = Program.UploadersConfig.YourlsSignature,
                        Username = Program.UploadersConfig.YourlsUsername,
                        Password = Program.UploadersConfig.YourlsPassword
                    };
                    break;
                case UrlShortenerType.NLCM:
                    urlShortener = new NlcmURLShortener();
                    break;
                case UrlShortenerType.AdFly:
                    urlShortener = new AdFlyURLShortener
                    {
                        APIKEY = Program.UploadersConfig.AdFlyAPIKEY,
                        APIUID = Program.UploadersConfig.AdFlyAPIUID
                    };
                    break;
				case UrlShortenerType.LnkU:
					urlShortener = new LnkUURLShortener
					{
						API_KEY = Program.UploadersConfig.LnkUAPIKEY
					};
					break;
                case UrlShortenerType.CustomURLShortener:
                    CustomUploaderItem customUploader = GetCustomUploader(Program.UploadersConfig.CustomURLShortenerSelected);
                    if (customUploader != null)
                    {
                        urlShortener = new CustomURLShortener(customUploader);
                    }
                    break;
            }

            if (urlShortener != null)
            {
                return urlShortener.ShortenURL(url);
            }

            return null;
        }

        public void ShareURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string encodedUrl = URLHelpers.URLEncode(url);

                switch (Info.TaskSettings.URLSharingServiceDestination)
                {
                    case URLSharingServices.Email:
                        if (Program.UploadersConfig.IsValid(URLSharingServices.Email))
                        {
                            using (EmailForm emailForm = new EmailForm(Program.UploadersConfig.EmailRememberLastTo ? Program.UploadersConfig.EmailLastTo : string.Empty,
                                Program.UploadersConfig.EmailDefaultSubject, url))
                            {
                                if (emailForm.ShowDialog() == DialogResult.OK)
                                {
                                    if (Program.UploadersConfig.EmailRememberLastTo)
                                    {
                                        Program.UploadersConfig.EmailLastTo = emailForm.ToEmail;
                                    }

                                    Email email = new Email
                                    {
                                        SmtpServer = Program.UploadersConfig.EmailSmtpServer,
                                        SmtpPort = Program.UploadersConfig.EmailSmtpPort,
                                        FromEmail = Program.UploadersConfig.EmailFrom,
                                        Password = Program.UploadersConfig.EmailPassword
                                    };

                                    email.Send(emailForm.ToEmail, emailForm.Subject, emailForm.Body);
                                }
                            }
                        }
                        else
                        {
                            URLHelpers.OpenURL("mailto:?body=" + encodedUrl);
                        }
                        break;
                    case URLSharingServices.Twitter:
                        if (Program.UploadersConfig.IsValid(URLSharingServices.Twitter))
                        {
                            OAuthInfo twitterOAuth = Program.UploadersConfig.TwitterOAuthInfoList[Program.UploadersConfig.TwitterSelectedAccount];

                            if (Program.UploadersConfig.TwitterSkipMessageBox)
                            {
                                try
                                {
                                    new Twitter(twitterOAuth).TweetMessage(url);
                                }
                                catch (Exception ex)
                                {
                                    DebugHelper.WriteException(ex);
                                }
                            }
                            else
                            {
                                using (TwitterTweetForm twitter = new TwitterTweetForm(twitterOAuth, url))
                                {
                                    twitter.ShowDialog();
                                }
                            }
                        }
                        else
                        {
                            //URLHelpers.OpenURL("https://twitter.com/intent/tweet?text=" + encodedUrl);
                            MessageBox.Show(Resources.TaskHelpers_TweetMessage_Unable_to_find_valid_Twitter_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                    case URLSharingServices.Facebook:
                        URLHelpers.OpenURL("https://www.facebook.com/sharer/sharer.php?u=" + encodedUrl);
                        break;
                    case URLSharingServices.GooglePlus:
                        URLHelpers.OpenURL("https://plus.google.com/share?url=" + encodedUrl);
                        break;
                    case URLSharingServices.Reddit:
                        URLHelpers.OpenURL("http://www.reddit.com/submit?url=" + encodedUrl);
                        break;
                    case URLSharingServices.Pinterest:
                        URLHelpers.OpenURL(string.Format("http://pinterest.com/pin/create/button/?url={0}&media={0}", encodedUrl));
                        break;
                    case URLSharingServices.Tumblr:
                        URLHelpers.OpenURL("https://www.tumblr.com/share?v=3&u=" + encodedUrl);
                        break;
                    case URLSharingServices.LinkedIn:
                        URLHelpers.OpenURL("https://www.linkedin.com/shareArticle?url=" + encodedUrl);
                        break;
                    case URLSharingServices.StumbleUpon:
                        URLHelpers.OpenURL("http://www.stumbleupon.com/submit?url=" + encodedUrl);
                        break;
                    case URLSharingServices.Delicious:
                        URLHelpers.OpenURL("https://delicious.com/save?v=5&url=" + encodedUrl);
                        break;
                    case URLSharingServices.VK:
                        URLHelpers.OpenURL("http://vk.com/share.php?url=" + encodedUrl);
                        break;
                    case URLSharingServices.Pushbullet:
                        new Pushbullet(Program.UploadersConfig.PushbulletSettings).PushLink(url, "ShareX: URL Share");
                        break;
                }
            }
        }

        private FTPAccount GetFTPAccount(int index)
        {
            if (Info.TaskSettings.OverrideFTP)
            {
                index = Info.TaskSettings.FTPIndex.BetweenOrDefault(0, Program.UploadersConfig.FTPAccountList.Count - 1);
            }

            return Program.UploadersConfig.FTPAccountList.ReturnIfValidIndex(index);
        }

        private CustomUploaderItem GetCustomUploader(int index)
        {
            if (Info.TaskSettings.OverrideCustomUploader)
            {
                index = Info.TaskSettings.CustomUploaderIndex.BetweenOrDefault(0, Program.UploadersConfig.CustomUploadersList.Count - 1);
            }

            return Program.UploadersConfig.CustomUploadersList.ReturnIfValidIndex(index);
        }

        private void ThreadCompleted()
        {
            OnUploadCompleted();
        }

        private void PrepareUploader(Uploader currentUploader)
        {
            uploader = currentUploader;
            uploader.BufferSize = (int)Math.Pow(2, Program.Settings.BufferSizePower) * 1024;
            uploader.ProgressChanged += uploader_ProgressChanged;
        }

        private void uploader_ProgressChanged(ProgressManager progress)
        {
            if (progress != null)
            {
                Info.Progress = progress;

                if (threadWorker != null)
                {
                    threadWorker.InvokeAsync(OnUploadProgressChanged);
                }
                else
                {
                    OnUploadProgressChanged();
                }
            }
        }

        private void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                if (threadWorker != null)
                {
                    threadWorker.InvokeAsync(() => StatusChanged(this));
                }
                else
                {
                    StatusChanged(this);
                }
            }
        }

        private void OnUploadStarted()
        {
            if (UploadStarted != null)
            {
                UploadStarted(this);
            }
        }

        private void OnUploadProgressChanged()
        {
            if (UploadProgressChanged != null)
            {
                UploadProgressChanged(this);
            }
        }

        private void OnUploadCompleted()
        {
            Status = TaskStatus.Completed;

            if (StopRequested)
            {
                Info.Status = Resources.UploadTask_OnUploadCompleted_Stopped;
            }
            else
            {
                Info.Status = Resources.UploadTask_OnUploadCompleted_Done;
            }

            if (UploadCompleted != null)
            {
                UploadCompleted(this);
            }

            Dispose();
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