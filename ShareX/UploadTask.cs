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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UploadersLib;
using UploadersLib.FileUploaders;
using UploadersLib.GUI;
using UploadersLib.HelperClasses;
using UploadersLib.ImageUploaders;
using UploadersLib.TextUploaders;
using UploadersLib.URLShorteners;

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
            EDataType dataType = Helpers.FindDataType(filePath);
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
                task.Data = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
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
            task.Info.FileName = "URL shorten";
            task.Info.Result.URL = url;
            return task;
        }

        public static UploadTask CreateFileJobTask(string filePath, TaskSettings taskSettings)
        {
            EDataType dataType = Helpers.FindDataType(filePath);
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
                    Info.Status = "Preparing";
                    break;
                default:
                    Info.Status = "Starting";
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
                    Info.Status = "Stopping";
                    OnStatusChanged();
                    break;
            }
        }

        private void ThreadDoWork()
        {
            Info.StartTime = DateTime.UtcNow;

            DoThreadJob();

            if (Info.IsUploadJob)
            {
                if (Program.Settings.ShowUploadWarning && MessageBox.Show(
                    "Are you sure you want to upload screenshot?\r\nYou can press 'No' for cancel current upload and disable auto uploading screenshots.",
                    "ShareX - First time upload warning",
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
                    Info.Status = "Uploading";

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

            if (!StopRequested && Info.Result != null && Info.Result.IsURLExpected && !Info.Result.IsError)
            {
                if (string.IsNullOrEmpty(Info.Result.URL))
                {
                    Info.Result.Errors.Add("URL is empty.");
                }
                else
                {
                    DoAfterUploadJobs();
                }
            }

            Info.UploadTime = DateTime.UtcNow;
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

        private void DoThreadJob()
        {
            if (Info.Job == TaskJob.Job)
            {
                if (tempImage != null)
                {
                    DoAfterCaptureJobs();
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
        }

        private void DoAfterCaptureJobs()
        {
            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects))
            {
                tempImage = TaskHelpers.AddImageEffects(tempImage, Info.TaskSettings);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddWatermark) && Info.TaskSettings.ImageSettings.WatermarkConfig != null)
            {
                Info.TaskSettings.ImageSettings.WatermarkConfig.Apply(tempImage);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AnnotateImage))
            {
                tempImage = TaskHelpers.AnnotateImage(tempImage, Info.FileName);
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyImageToClipboard))
            {
                ClipboardHelpers.CopyImage(tempImage);
                DebugHelper.WriteLine("CopyImageToClipboard");
            }

            if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.SendImageToPrinter))
            {
                if (Program.Settings.DontShowPrintSettingsDialog)
                {
                    using (PrintHelper printHelper = new PrintHelper(tempImage))
                    {
                        printHelper.Settings = Program.Settings.PrintSettings;
                        printHelper.Print();
                    }
                }
                else
                {
                    using (PrintForm printForm = new PrintForm(tempImage, Program.Settings.PrintSettings))
                    {
                        printForm.ShowDialog();
                    }
                }
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
                            sfd.Title = "Choose a folder to save " + Path.GetFileName(Info.FileName);

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
        }

        private void DoFileJobs()
        {
            if (!string.IsNullOrEmpty(Info.FilePath) && File.Exists(Info.FilePath))
            {
                if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyFileToClipboard))
                {
                    ClipboardHelpers.CopyFile(Info.FilePath);
                }
                else if (Info.TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.CopyFilePathToClipboard))
                {
                    ClipboardHelpers.CopyText(Info.FilePath);
                }

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
                            fileAction.Run(Info.FilePath);
                        }

                        Data = new FileStream(Info.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                }
            }
        }

        private void DoAfterUploadJobs()
        {
            try
            {
                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.UseURLShortener) || Info.Job == TaskJob.ShortenURL ||
                    (Info.TaskSettings.AdvancedSettings.AutoShortenURLLength > 0 && Info.Result.URL.Length > Info.TaskSettings.AdvancedSettings.AutoShortenURLLength))
                {
                    UploadResult result = ShortenURL(Info.Result.URL);

                    if (result != null)
                    {
                        Info.Result.ShortenedURL = result.ShortenedURL;
                    }
                }

                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.ShareURLToSocialNetworkingService))
                {
                    DoSocialNetworkingService();
                }

                if (Info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.SendURLWithEmail))
                {
                    using (EmailForm emailForm = new EmailForm(Program.UploadersConfig.EmailRememberLastTo ? Program.UploadersConfig.EmailLastTo : string.Empty,
                        Program.UploadersConfig.EmailDefaultSubject, Info.Result.ToString()))
                    {
                        emailForm.Icon = ShareXResources.Icon;

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
                case ImageDestination.ImageShack:
                    Program.UploadersConfig.ImageShackSettings.ThumbnailWidth = Info.TaskSettings.AdvancedSettings.ThumbnailPreferredWidth;
                    Program.UploadersConfig.ImageShackSettings.ThumbnailHeight = Info.TaskSettings.AdvancedSettings.ThumbnailPreferredHeight;
                    imageUploader = new ImageShackUploader(APIKeys.ImageShackKey, Program.UploadersConfig.ImageShackSettings);
                    break;
                case ImageDestination.TinyPic:
                    imageUploader = new TinyPicUploader(APIKeys.TinyPicID, APIKeys.TinyPicKey, Program.UploadersConfig.TinyPicAccountType, Program.UploadersConfig.TinyPicRegistrationCode);
                    break;
                case ImageDestination.Imgur:
                    if (Program.UploadersConfig.ImgurOAuth2Info == null)
                    {
                        Program.UploadersConfig.ImgurOAuth2Info = new OAuth2Info(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);
                    }

                    imageUploader = new Imgur_v3(Program.UploadersConfig.ImgurOAuth2Info)
                    {
                        UploadMethod = Program.UploadersConfig.ImgurAccountType,
                        ThumbnailType = Program.UploadersConfig.ImgurThumbnailType,
                        UploadAlbumID = Program.UploadersConfig.ImgurAlbumID
                    };
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
                    imageUploader = new Twitter(twitterOAuth);
                    break;
                case ImageDestination.CustomImageUploader:
                    if (Program.UploadersConfig.CustomUploadersList.IsValidIndex(Program.UploadersConfig.CustomImageUploaderSelected))
                    {
                        imageUploader = new CustomImageUploader(Program.UploadersConfig.CustomUploadersList[Program.UploadersConfig.CustomImageUploaderSelected]);
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
                case TextDestination.PastebinCA:
                    textUploader = new Pastebin_ca(APIKeys.PastebinCaKey, new PastebinCaSettings { TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat });
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
                case TextDestination.CustomTextUploader:
                    if (Program.UploadersConfig.CustomUploadersList.IsValidIndex(Program.UploadersConfig.CustomTextUploaderSelected))
                    {
                        textUploader = new CustomTextUploader(Program.UploadersConfig.CustomUploadersList[Program.UploadersConfig.CustomTextUploaderSelected]);
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
                case FileDestination.Copy:
                    fileUploader = new Copy(Program.UploadersConfig.CopyOAuthInfo, Program.UploadersConfig.CopyAccountInfo)
                    {
                        UploadPath = NameParser.Parse(NameParserType.URL, Copy.TidyUploadPath(Program.UploadersConfig.CopyUploadPath)),
                        URLType = Program.UploadersConfig.CopyURLType
                    };
                    break;
                case FileDestination.GoogleDrive:
                    fileUploader = new GoogleDrive(Program.UploadersConfig.GoogleDriveOAuth2Info)
                    {
                        IsPublic = Program.UploadersConfig.GoogleDriveIsPublic,
                        FolderID = Program.UploadersConfig.GoogleDriveUseFolder ? Program.UploadersConfig.GoogleDriveFolderID : null
                    };
                    break;
                case FileDestination.RapidShare:
                    fileUploader = new RapidShare(Program.UploadersConfig.RapidShareUsername, Program.UploadersConfig.RapidSharePassword,
                        Program.UploadersConfig.RapidShareFolderID);
                    break;
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
                    if (Program.UploadersConfig.IsActive(FileDestination.Ge_tt))
                    {
                        fileUploader = new Ge_tt(APIKeys.Ge_ttKey)
                        {
                            AccessToken = Program.UploadersConfig.Ge_ttLogin.AccessToken
                        };
                    }
                    break;
                case FileDestination.Localhostr:
                    fileUploader = new Hostr(Program.UploadersConfig.LocalhostrEmail, Program.UploadersConfig.LocalhostrPassword)
                    {
                        DirectURL = Program.UploadersConfig.LocalhostrDirectURL
                    };
                    break;
                case FileDestination.CustomFileUploader:
                    if (Program.UploadersConfig.CustomUploadersList.IsValidIndex(Program.UploadersConfig.CustomFileUploaderSelected))
                    {
                        fileUploader = new CustomFileUploader(Program.UploadersConfig.CustomUploadersList[Program.UploadersConfig.CustomFileUploaderSelected]);
                    }
                    break;
                case FileDestination.FTP:
                    int index;

                    if (Info.TaskSettings.OverrideFTP)
                    {
                        index = Info.TaskSettings.FTPIndex.BetweenOrDefault(0, Program.UploadersConfig.FTPAccountList.Count - 1);
                    }
                    else
                    {
                        index = Program.UploadersConfig.GetFTPIndex(Info.DataType);
                    }

                    FTPAccount account = Program.UploadersConfig.FTPAccountList.ReturnIfValidIndex(index);

                    if (account != null)
                    {
                        if (account.Protocol == FTPProtocol.FTP || account.Protocol == FTPProtocol.FTPS)
                        {
                            fileUploader = new FTP(account);
                        }
                        else if (account.Protocol == FTPProtocol.SFTP)
                        {
                            fileUploader = new SFTP(account);
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
                    fileUploader = new OwnCloud(Program.UploadersConfig.OwnCloudHost, Program.UploadersConfig.OwnCloudUsername, Program.UploadersConfig.OwnCloudPassword, Program.UploadersConfig.OwnCloudPath);
                    break;
                case FileDestination.Pushbullet:
                    fileUploader = new Pushbullet(Program.UploadersConfig.PushbulletSettings);
                    break;
                case FileDestination.MediaCrush:
                    fileUploader = new MediaCrushUploader();
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
                /*case UrlShortenerType.THREELY:
                urlShortener = new ThreelyURLShortener(Program.ThreelyKey);
                break;*/
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
                case UrlShortenerType.CustomURLShortener:
                    if (Program.UploadersConfig.CustomUploadersList.IsValidIndex(Program.UploadersConfig.CustomURLShortenerSelected))
                    {
                        urlShortener = new CustomURLShortener(Program.UploadersConfig.CustomUploadersList[Program.UploadersConfig.CustomURLShortenerSelected]);
                    }
                    break;
            }

            if (urlShortener != null)
            {
                return urlShortener.ShortenURL(url);
            }

            return null;
        }

        public void DoSocialNetworkingService()
        {
            string url = Info.Result.ToString();

            if (!string.IsNullOrEmpty(url))
            {
                switch (Info.TaskSettings.SocialNetworkingServiceDestination)
                {
                    case SocialNetworkingService.Twitter:
                        OAuthInfo twitterOAuth = Program.UploadersConfig.TwitterOAuthInfoList.ReturnIfValidIndex(Program.UploadersConfig.TwitterSelectedAccount);

                        if (twitterOAuth != null)
                        {
                            using (TwitterTweetForm twitter = new TwitterTweetForm(twitterOAuth))
                            {
                                twitter.Message = url;
                                twitter.ShowDialog();
                            }
                        }
                        break;
                    case SocialNetworkingService.Facebook:
                        URLHelpers.OpenURL("https://www.facebook.com/sharer/sharer.php?u=" + url);
                        break;
                    case SocialNetworkingService.GooglePlus:
                        // The Google+ API currently provides read-only access to public data. So sharing with API not possible yet.
                        URLHelpers.OpenURL("https://plus.google.com/share?url=" + url);
                        break;
                }
            }
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
                Info.Status = "Stopped";
            }
            else
            {
                Info.Status = "Done";
            }

            if (UploadCompleted != null)
            {
                UploadCompleted(this);
            }

            Dispose();
        }

        public void Dispose()
        {
            if (Data != null) Data.Dispose();
            if (tempImage != null) tempImage.Dispose();
        }
    }
}