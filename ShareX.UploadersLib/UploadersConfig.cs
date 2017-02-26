#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.TextUploaders;
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
        public bool ImgurUseHTTPS = false;
        public bool ImgurUseGIFV = true;
        public OAuth2Info ImgurOAuth2Info = null;
        public bool ImgurUploadSelectedAlbum = false;
        public ImgurAlbumData ImgurSelectedAlbum = null;
        public List<ImgurAlbumData> ImgurAlbumList = null;

        // ImageShack

        public ImageShackOptions ImageShackSettings = new ImageShackOptions();

        // TinyPic

        public AccountType TinyPicAccountType = AccountType.Anonymous;
        public string TinyPicRegistrationCode = "";
        public string TinyPicUsername = "";
        public string TinyPicPassword = "";
        public bool TinyPicRememberUserPass = false;

        // Flickr

        public FlickrAuthInfo FlickrAuthInfo = new FlickrAuthInfo();
        public FlickrSettings FlickrSettings = new FlickrSettings();

        // Photobucket

        public OAuthInfo PhotobucketOAuthInfo = null;
        public PhotobucketAccountInfo PhotobucketAccountInfo = null;

        // Picasa

        public OAuth2Info PicasaOAuth2Info = null;
        public string PicasaAlbumID = "";

        // Chevereto

        public CheveretoUploader CheveretoUploader = new CheveretoUploader("http://ultraimg.com/api/1/upload", "3374fa58c672fcaad8dab979f7687397");
        public bool CheveretoDirectURL = true;

        // SomeImage

        public string SomeImageAPIKey = "";
        public bool SomeImageDirectURL = true;

        // vgy.me

        public string VgymeUserKey = "";

        #endregion Image uploaders

        #region Text uploaders

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

        public string UpasteUserKey = "";
        public bool UpasteIsPublic = false;

        // Hastebin

        public string HastebinCustomDomain = "https://hastebin.com";
        public string HastebinSyntaxHighlighting = "hs";
        public bool HastebinUseFileExtension = true;

        // OneTimeSecret

        public string OneTimeSecretAPIKey = "";
        public string OneTimeSecretAPIUsername = "";

        // Pastie

        public bool PastieIsPublic = false;

        #endregion Text uploaders

        #region File uploaders

        // Dropbox

        public OAuth2Info DropboxOAuth2Info = null;
        //public DropboxAccount DropboxAccount = null;
        public string DropboxUploadPath = "ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink = true;
        public DropboxURLType DropboxURLType = DropboxURLType.Default;
        public DropboxAccountInfo DropboxAccountInfo = null; // API v1

        // FTP Server

        public List<FTPAccount> FTPAccountList = new List<FTPAccount>();
        public int FTPSelectedImage = 0;
        public int FTPSelectedText = 0;
        public int FTPSelectedFile = 0;

        // OneDrive

        public OAuth2Info OneDriveOAuth2Info = null;
        public OneDriveFileInfo OneDriveSelectedFolder = OneDrive.RootFolder;
        public bool OneDriveAutoCreateShareableLink = true;

        // Google Drive

        public OAuth2Info GoogleDriveOAuth2Info = null;
        public bool GoogleDriveIsPublic = true;
        public bool GoogleDriveDirectLink = false;
        public bool GoogleDriveUseFolder = false;
        public string GoogleDriveFolderID = "";

        // puush

        public string PuushAPIKey = "";

        // SendSpace

        public AccountType SendSpaceAccountType = AccountType.Anonymous;
        public string SendSpaceUsername = "";
        public string SendSpacePassword = "";

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

        public string LocalhostrEmail = "";
        public string LocalhostrPassword = "";
        public bool LocalhostrDirectURL = true;

        // Shared Folder

        public List<LocalhostAccount> LocalhostAccountList = new List<LocalhostAccount>();
        public int LocalhostSelectedImages = 0;
        public int LocalhostSelectedText = 0;
        public int LocalhostSelectedFiles = 0;

        // Email

        public string EmailSmtpServer = "smtp.gmail.com";
        public int EmailSmtpPort = 587;
        public string EmailFrom = "...@gmail.com";
        public string EmailPassword = "";
        public bool EmailRememberLastTo = true;
        public string EmailLastTo = "";
        public string EmailDefaultSubject = "Sending email from ShareX";
        public string EmailDefaultBody = "Screenshot is attached.";
        public bool EmailAutomaticSend = false;
        public string EmailAutomaticSendTo = "";

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
        public bool OwnCloud81Compatibility = false;

        // MediaFire

        public string MediaFireUsername = "";
        public string MediaFirePassword = "";
        public string MediaFirePath = "";
        public bool MediaFireUseLongLink = false;

        // Pushbullet

        public PushbulletSettings PushbulletSettings = new PushbulletSettings();

        // Lambda

        public LambdaSettings LambdaSettings = new LambdaSettings();

        // Lithiio

        public LithiioSettings LithiioSettings = new LithiioSettings();

        // Pomf

        public PomfUploader PomfUploader = new PomfUploader("https://mixtape.moe/upload.php");

        // s-ul

        public string SulAPIKey = "";

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

        public bool StreamableAnonymous = true;
        public string StreamableUsername = "";
        public string StreamablePassword = "";
        public bool StreamableUseDirectURL = false;

        // Uplea

        public string UpleaApiKey = "";
        public string UpleaEmailAddress = "";
        public bool UpleaIsPremiumMember = false;
        public bool UpleaInstantDownloadEnabled = false;

        // Azure Storage

        public string AzureStorageAccountName = "";
        public string AzureStorageAccountAccessKey = "";
        public string AzureStorageContainer = "";

        // Plik

        public PlikSettings PlikSettings = new PlikSettings()
        {
        };

        #endregion File uploaders

        #region URL shorteners

        // bit.ly

        public OAuth2Info BitlyOAuth2Info = null;
        public string BitlyDomain = "";

        // Google URL Shortener

        public AccountType GoogleURLShortenerAccountType = AccountType.Anonymous;
        public OAuth2Info GoogleURLShortenerOAuth2Info = null;

        // yourls.org

        public string YourlsAPIURL = "http://yoursite.com/yourls-api.php";
        public string YourlsSignature = "";
        public string YourlsUsername = "";
        public string YourlsPassword = "";

        // adf.ly
        public string AdFlyAPIKEY = "";
        public string AdFlyAPIUID = "";

        // coinurl.com
        public string CoinURLUUID = "";

        // polr
        public string PolrAPIHostname = "";
        public string PolrAPIKey = "";
        public bool PolrIsSecret = false;
        public bool PolrUseAPIv1 = false;

        #endregion URL shorteners

        #region URL sharing services

        // Twitter

        public List<OAuthInfo> TwitterOAuthInfoList = new List<OAuthInfo>();
        public int TwitterSelectedAccount = 0;
        public bool TwitterSkipMessageBox = false;
        public string TwitterDefaultMessage = "";

        #endregion URL sharing services

        #region Custom Uploaders

        public List<CustomUploaderItem> CustomUploadersList = new List<CustomUploaderItem>();
        public int CustomImageUploaderSelected = 0;
        public int CustomTextUploaderSelected = 0;
        public int CustomFileUploaderSelected = 0;
        public int CustomURLShortenerSelected = 0;

        #endregion Custom Uploaders
    }
}