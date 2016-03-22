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

using CG.Web.MegaApiClient;
using ShareX.HelpersLib;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.HelperClasses;
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.TextUploaders;
using System;
using System.Collections.Generic;

namespace ShareX.UploadersLib
{
    public class UploadersConfig : SettingsBase<UploadersConfig>
    {
        #region Image uploaders

        // Imgur

        public AccountType ImgurAccountType = AccountType.Anonymous;
        public bool ImgurDirectLink = true;
        public ImgurThumbnailType ImgurThumbnailType = ImgurThumbnailType.Large_Thumbnail;
        public bool ImgurUseGIFV = true;
        public OAuth2Info ImgurOAuth2Info = null;
        public bool ImgurUploadSelectedAlbum = false;
        public ImgurAlbumData ImgurSelectedAlbum = null;
        public List<ImgurAlbumData> ImgurAlbumList = null;

        // ImageShack

        public ImageShackOptions ImageShackSettings = new ImageShackOptions();

        // TinyPic

        public AccountType TinyPicAccountType = AccountType.Anonymous;
        public string TinyPicRegistrationCode = string.Empty;
        public string TinyPicUsername = string.Empty;
        public string TinyPicPassword = string.Empty;
        public bool TinyPicRememberUserPass = false;

        // Flickr

        public FlickrAuthInfo FlickrAuthInfo = new FlickrAuthInfo();
        public FlickrSettings FlickrSettings = new FlickrSettings();

        // Photobucket

        public OAuthInfo PhotobucketOAuthInfo = null;
        public PhotobucketAccountInfo PhotobucketAccountInfo = null;

        // Picasa

        public OAuth2Info PicasaOAuth2Info = null;
        public string PicasaAlbumID = string.Empty;

        // Chevereto

        public CheveretoUploader CheveretoUploader = new CheveretoUploader("http://ultraimg.com/api/1/upload", "3374fa58c672fcaad8dab979f7687397");
        public bool CheveretoDirectURL = true;

        // SomeImage

        public string SomeImageAPIKey = string.Empty;
        public bool SomeImageDirectURL = true;

        // vgy.me

        public string VgymeUserKey = string.Empty;

        #endregion Image uploaders

        #region Text uploaders

        public string TextFormat = "";

        // Pastebin

        public PastebinSettings PastebinSettings = new PastebinSettings();

        // Paste.ee

        public string Paste_eeUserAPIKey = "public";

        // Gist

        public bool GistAnonymousLogin = true;
        public OAuth2Info GistOAuth2Info = null;
        public bool GistPublishPublic = false;
        public bool GistRawURL = false;

        // uPaste

        public string UpasteUserKey = string.Empty;
        public bool UpasteIsPublic = false;

        // Hastebin

        public string HastebinCustomDomain = "http://hastebin.com";
        public string HastebinSyntaxHighlighting = "hs";

        // OneTimeSecret

        public string OneTimeSecretAPIKey = string.Empty;
        public string OneTimeSecretAPIUsername = string.Empty;

        #endregion Text uploaders

        #region File uploaders

        // Dropbox

        public OAuth2Info DropboxOAuth2Info = null;
        public DropboxAccountInfo DropboxAccountInfo = null;
        public string DropboxUploadPath = "Public/ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink = false;
        public DropboxURLType DropboxURLType = DropboxURLType.Default;

        // OneDrive

        public OAuth2Info OneDriveOAuth2Info = null;
        public OneDriveFileInfo OneDriveSelectedFolder = OneDrive.RootFolder;
        public bool OneDriveAutoCreateShareableLink = true;

        // Copy

        public OAuthInfo CopyOAuthInfo = null;
        public CopyAccountInfo CopyAccountInfo = null;
        public string CopyUploadPath = "ShareX/%y/%mo";
        public CopyURLType CopyURLType = CopyURLType.Shortened;

        // Google Drive

        public OAuth2Info GoogleDriveOAuth2Info = null;
        public bool GoogleDriveIsPublic = true;
        public bool GoogleDriveUseFolder = false;
        public string GoogleDriveFolderID = string.Empty;

        // SendSpace

        public AccountType SendSpaceAccountType = AccountType.Anonymous;
        public string SendSpaceUsername = string.Empty;
        public string SendSpacePassword = string.Empty;

        // Minus

        public OAuth2Info MinusOAuth2Info = null;
        public MinusOptions MinusConfig = new MinusOptions();

        // Box

        public OAuth2Info BoxOAuth2Info = null;
        public BoxFileEntry BoxSelectedFolder = Box.RootFolder;
        public bool BoxShare = true;

        // Ge.tt

        public Ge_ttLogin Ge_ttLogin = null;

        // Localhostr

        public string LocalhostrEmail = string.Empty;
        public string LocalhostrPassword = string.Empty;
        public bool LocalhostrDirectURL = true;

        // FTP Server

        public List<FTPAccount> FTPAccountList = new List<FTPAccount>();
        public int FTPSelectedImage = 0;
        public int FTPSelectedText = 0;
        public int FTPSelectedFile = 0;

        // Shared Folder

        public List<LocalhostAccount> LocalhostAccountList = new List<LocalhostAccount>();
        public int LocalhostSelectedImages = 0;
        public int LocalhostSelectedText = 0;
        public int LocalhostSelectedFiles = 0;

        // Email

        public string EmailSmtpServer = "smtp.gmail.com";
        public int EmailSmtpPort = 587;
        public string EmailFrom = "...@gmail.com";
        public string EmailPassword = string.Empty;
        public bool EmailRememberLastTo = true;
        public bool EmailConfirmSend = true;
        public string EmailLastTo = string.Empty;
        public string EmailDefaultSubject = "Sending email from ShareX";
        public string EmailDefaultBody = "Screenshot is attached.";

        // Jira

        public string JiraHost = "http://";
        public string JiraIssuePrefix = "PROJECT-";
        public OAuthInfo JiraOAuthInfo = null;

        // Mega

        public MegaApiClient.AuthInfos MegaAuthInfos = null;
        public string MegaParentNodeId = null;

        // Amazon S3

        public AmazonS3Settings AmazonS3Settings = new AmazonS3Settings()
        {
            ObjectPrefix = "ShareX/%y/%mo",
            UseReducedRedundancyStorage = true
        };

        // ownCloud

        public string OwnCloudHost = "";
        public string OwnCloudUsername = "";
        public string OwnCloudPassword = "";
        public string OwnCloudPath = "/";
        public bool OwnCloudCreateShare = true;
        public bool OwnCloudDirectLink = false;
        public bool OwnCloudIgnoreInvalidCert = false;
        public bool OwnCloud81Compatibility = false;

        // MediaFire

        public string MediaFireUsername = "";
        public string MediaFirePassword = "";
        public string MediaFirePath = "";
        public bool MediaFireUseLongLink = false;

        // Pushbullet

        public PushbulletSettings PushbulletSettings = new PushbulletSettings();

        // Up1

        public string Up1Host = "https://up1.ca";
        public string Up1Key = "c61540b5ceecd05092799f936e27755f";

        // Lambda

        public LambdaSettings LambdaSettings = new LambdaSettings();

        // Pomf

        public PomfUploader PomfUploader = new PomfUploader("https://pomf.cat/upload.php", "https://a.pomf.cat");

        // s-ul

        public string SulAPIKey = string.Empty;

        // Seafile

        public string SeafileAPIURL = "";
        public string SeafileAuthToken = "";
        public string SeafileRepoID = "";
        public string SeafilePath = "/";
        public bool SeafileIsLibraryEncrypted = false;
        public string SeafileEncryptedLibraryPassword = "";
        public bool SeafileCreateShareableURL = true;
        public bool SeafileIgnoreInvalidCert = false;
        public int SeafileShareDaysToExpire = 0;
        public string SeafileSharePassword = "";
        public string SeafileAccInfoEmail = "";
        public string SeafileAccInfoUsage = "";

        // Streamable

        public string StreamableUsername = "";
        public string StreamablePassword = "";
        public bool StreamableAnonymous = true;

        // openload.co

        public string OpenloadAPILogin = string.Empty;
        public string OpenloadAPIKey = string.Empty;
        public OpenloadFolderNode OpenloadFolderTree = null;
        public bool OpenloadUploadToSelectedFolder = false;
        public string OpenloadSelectedFolderID = string.Empty;

        #endregion File uploaders

        #region URL shorteners

        // bit.ly

        public OAuth2Info BitlyOAuth2Info = null;
        public string BitlyDomain = string.Empty;

        // Google URL Shortener

        public AccountType GoogleURLShortenerAccountType = AccountType.Anonymous;
        public OAuth2Info GoogleURLShortenerOAuth2Info = null;

        // yourls.org

        public string YourlsAPIURL = "http://yoursite.com/yourls-api.php";
        public string YourlsSignature = string.Empty;
        public string YourlsUsername = string.Empty;
        public string YourlsPassword = string.Empty;

        // adf.ly
        public string AdFlyAPIKEY = String.Empty;
        public string AdFlyAPIUID = String.Empty;

        // coinurl.com
        public string CoinURLUUID = string.Empty;

        // polr
        public string PolrAPIHostname = string.Empty;
        public string PolrAPIKey = string.Empty;

        #endregion URL shorteners

        #region URL sharing services

        // Twitter

        public List<OAuthInfo> TwitterOAuthInfoList = new List<OAuthInfo>();
        public int TwitterSelectedAccount = 0;
        public bool TwitterSkipMessageBox = false;
        public string TwitterDefaultMessage = string.Empty;

        #endregion URL sharing services

        #region Custom Uploaders

        public List<CustomUploaderItem> CustomUploadersList = new List<CustomUploaderItem>();
        public int CustomImageUploaderSelected = 0;
        public int CustomTextUploaderSelected = 0;
        public int CustomFileUploaderSelected = 0;
        public int CustomURLShortenerSelected = 0;

        #endregion Custom Uploaders

        #region Helper Methods

        public bool IsValid<T>(int index)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);

            if (destination is ImageDestination)
            {
                return IsValid((ImageDestination)destination);
            }

            if (destination is TextDestination)
            {
                return IsValid((TextDestination)destination);
            }

            if (destination is FileDestination)
            {
                return IsValid((FileDestination)destination);
            }

            if (destination is UrlShortenerType)
            {
                return IsValid((UrlShortenerType)destination);
            }

            if (destination is URLSharingServices)
            {
                return IsValid((URLSharingServices)destination);
            }

            return true;
        }

        public bool IsValid(ImageDestination destination)
        {
            ImageUploaderService service = UploaderFactory.GetImageUploaderServiceByEnum(destination);

            if (service != null)
            {
                return service.CheckConfig(this);
            }

            return true;
        }

        public bool IsValid(TextDestination destination)
        {
            TextUploaderService service = UploaderFactory.GetTextUploaderServiceByEnum(destination);

            if (service != null)
            {
                return service.CheckConfig(this);
            }

            return true;
        }

        public bool IsValid(FileDestination destination)
        {
            FileUploaderService service = UploaderFactory.GetFileUploaderServiceByEnum(destination);

            if (service != null)
            {
                return service.CheckConfig(this);
            }

            return true;
        }

        public bool IsValid(UrlShortenerType destination)
        {
            URLShortenerService service = UploaderFactory.GetURLShortenerServiceByEnum(destination);

            if (service != null)
            {
                return service.CheckConfig(this);
            }

            return true;
        }

        public bool IsValid(URLSharingServices destination)
        {
            switch (destination)
            {
                case URLSharingServices.Email:
                    return !string.IsNullOrEmpty(EmailSmtpServer) && EmailSmtpPort > 0 && !string.IsNullOrEmpty(EmailFrom) && !string.IsNullOrEmpty(EmailPassword);
                case URLSharingServices.Twitter:
                    return TwitterOAuthInfoList != null && TwitterOAuthInfoList.IsValidIndex(TwitterSelectedAccount) && OAuthInfo.CheckOAuth(TwitterOAuthInfoList[TwitterSelectedAccount]);
                case URLSharingServices.Pushbullet:
                    return PushbulletSettings != null && !string.IsNullOrEmpty(PushbulletSettings.UserAPIKey) && PushbulletSettings.DeviceList != null &&
                        PushbulletSettings.DeviceList.IsValidIndex(PushbulletSettings.SelectedDevice);
            }

            return true;
        }

        #endregion Helper Methods
    }
}