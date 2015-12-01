using ShareX.HelpersLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.GUI;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public class UploadFileWorkerTask : WorkerTask
    {
        public UploadFileWorkerTask(TaskSettings settings) : base(settings)
        {
        }

        public override UploadResult PerformTask(Stream stream, string fileName)
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
                        IgnoreInvalidCert = Program.UploadersConfig.OwnCloudIgnoreInvalidCert,
                        IsCompatibility81 = Program.UploadersConfig.OwnCloud81Compatibility
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
                case FileDestination.Pomf:
                    fileUploader = new Pomf(Program.UploadersConfig.PomfUploader);
                    break;
                case FileDestination.Uguu:
                    fileUploader = new Uguu();
                    break;
                case FileDestination.Dropfile:
                    fileUploader = new Dropfile();
                    break;
                case FileDestination.Up1:
                    fileUploader = new Up1(Program.UploadersConfig.Up1Host, Program.UploadersConfig.Up1Key);
                    break;
                case FileDestination.Seafile:
                    fileUploader = new Seafile(Program.UploadersConfig.SeafileAPIURL, Program.UploadersConfig.SeafileAuthToken, Program.UploadersConfig.SeafileRepoID)
                    {
                        Path = Program.UploadersConfig.SeafilePath,
                        IsLibraryEncrypted = Program.UploadersConfig.SeafileIsLibraryEncrypted,
                        EncryptedLibraryPassword = Program.UploadersConfig.SeafileEncryptedLibraryPassword,
                        ShareDaysToExpire = Program.UploadersConfig.SeafileShareDaysToExpire,
                        SharePassword = Program.UploadersConfig.SeafileSharePassword,
                        CreateShareableURL = Program.UploadersConfig.SeafileCreateShareableURL,
                        IgnoreInvalidCert = Program.UploadersConfig.SeafileIgnoreInvalidCert
                    };
                    break;
            }

            if (fileUploader != null)
            {
                PrepareUploader(fileUploader);
                return fileUploader.Upload(stream, fileName);
            }

            return null;
        }
    }
}