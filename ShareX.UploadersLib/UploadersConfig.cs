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

        #region Imgur

        public AccountType ImgurAccountType = AccountType.Anonymous;
        public bool ImgurDirectLink = true;
        public ImgurThumbnailType ImgurThumbnailType = ImgurThumbnailType.Medium_Thumbnail;
        public bool ImgurUseGIFV = true;
        public OAuth2Info ImgurOAuth2Info = null;
        public bool ImgurUploadSelectedAlbum = false;
        public ImgurAlbumData ImgurSelectedAlbum = null;
        public List<ImgurAlbumData> ImgurAlbumList = null;

        #endregion

        #region ImageShack

        public ImageShackOptions ImageShackSettings = new ImageShackOptions();

        #endregion

        #region TinyPic

        public AccountType TinyPicAccountType = AccountType.Anonymous;
        public string TinyPicRegistrationCode = "";
        public string TinyPicUsername = "";
        public string TinyPicPassword = "";
        public bool TinyPicRememberUserPass = false;

        #endregion

        #region Flickr

        public OAuthInfo FlickrOAuthInfo = null;
        public FlickrSettings FlickrSettings = new FlickrSettings();

        #endregion

        #region Photobucket

        public OAuthInfo PhotobucketOAuthInfo = null;
        public PhotobucketAccountInfo PhotobucketAccountInfo = null;

        #endregion

        #region Google Photos

        public OAuth2Info PicasaOAuth2Info = null;
        public string PicasaAlbumID = "";

        #endregion

        #region Chevereto

        public CheveretoUploader CheveretoUploader = new CheveretoUploader("http://ultraimg.com/api/1/upload", "3374fa58c672fcaad8dab979f7687397");
        public bool CheveretoDirectURL = true;

        #endregion

        #region vgy.me

        public string VgymeUserKey = "";

        #endregion

        #endregion Image uploaders

        #region Text uploaders

        #region Pastebin

        public PastebinSettings PastebinSettings = new PastebinSettings();

        #endregion

        #region Paste.ee

        public string Paste_eeUserKey = "";

        #endregion

        #region Gist

        public bool GistAnonymousLogin = true;
        public OAuth2Info GistOAuth2Info = null;
        public bool GistPublishPublic = false;
        public bool GistRawURL = false;
        public string GistCustomURL = "";

        #endregion

        #region uPaste

        public string UpasteUserKey = "";
        public bool UpasteIsPublic = false;

        #endregion

        #region Hastebin

        public string HastebinCustomDomain = "https://hastebin.com";
        public string HastebinSyntaxHighlighting = "hs";
        public bool HastebinUseFileExtension = true;

        #endregion

        #region OneTimeSecret

        public string OneTimeSecretAPIKey = "";
        public string OneTimeSecretAPIUsername = "";

        #endregion

        #region Pastie

        public bool PastieIsPublic = false;

        #endregion

        #endregion Text uploaders

        #region File uploaders

        #region Dropbox

        public OAuth2Info DropboxOAuth2Info = null;
        public string DropboxUploadPath = "ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink = true;
        public bool DropboxUseDirectLink = false;

        // TEMP: For backward compatibility
        public DropboxURLType DropboxURLType = DropboxURLType.Default;

        #endregion

        #region FTP

        public List<FTPAccount> FTPAccountList = new List<FTPAccount>();
        public int FTPSelectedImage = 0;
        public int FTPSelectedText = 0;
        public int FTPSelectedFile = 0;

        #endregion

        #region OneDrive

        public OAuth2Info OneDriveOAuth2Info = null;
        public OneDriveFileInfo OneDriveSelectedFolder = OneDrive.RootFolder;
        public bool OneDriveAutoCreateShareableLink = true;

        #endregion

        #region Gfycat

        public OAuth2Info GfycatOAuth2Info = null;
        public AccountType GfycatAccountType = AccountType.Anonymous;
        public bool GfycatIsPublic = false;

        #endregion

        #region Google Drive

        public OAuth2Info GoogleDriveOAuth2Info = null;
        public bool GoogleDriveIsPublic = true;
        public bool GoogleDriveDirectLink = false;
        public bool GoogleDriveUseFolder = false;
        public string GoogleDriveFolderID = "";

        #endregion

        #region puush

        public string PuushAPIKey = "";

        #endregion

        #region SendSpace

        public AccountType SendSpaceAccountType = AccountType.Anonymous;
        public string SendSpaceUsername = "";
        public string SendSpacePassword = "";

        #endregion

        #region Box

        public OAuth2Info BoxOAuth2Info = null;
        public BoxFileEntry BoxSelectedFolder = Box.RootFolder;
        public bool BoxShare = true;

        #endregion

        #region Ge.tt

        public Ge_ttLogin Ge_ttLogin = null;

        #endregion

        #region Localhostr

        public string LocalhostrEmail = "";
        public string LocalhostrPassword = "";
        public bool LocalhostrDirectURL = true;

        #endregion

        #region Shared folder

        public List<LocalhostAccount> LocalhostAccountList = new List<LocalhostAccount>();
        public int LocalhostSelectedImages = 0;
        public int LocalhostSelectedText = 0;
        public int LocalhostSelectedFiles = 0;

        #endregion

        #region Email

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

        #endregion

        #region Jira

        public string JiraHost = "http://";
        public string JiraIssuePrefix = "PROJECT-";
        public OAuthInfo JiraOAuthInfo = null;

        #endregion

        #region Mega

        public MegaApiClient.AuthInfos MegaAuthInfos = null;
        public string MegaParentNodeId = null;

        #endregion

        #region Amazon S3

        public AmazonS3Settings AmazonS3Settings = new AmazonS3Settings()
        {
            ObjectPrefix = "ShareX/%y/%mo"
        };

        #endregion

        #region ownCloud

        public string OwnCloudHost = "";
        public string OwnCloudUsername = "";
        public string OwnCloudPassword = "";
        public string OwnCloudPath = "/";
        public bool OwnCloudCreateShare = true;
        public bool OwnCloudDirectLink = false;
        public bool OwnCloud81Compatibility = false;

        #endregion

        #region MediaFire

        public string MediaFireUsername = "";
        public string MediaFirePassword = "";
        public string MediaFirePath = "";
        public bool MediaFireUseLongLink = false;

        #endregion

        #region Pushbullet

        public PushbulletSettings PushbulletSettings = new PushbulletSettings();

        #endregion

        #region Lambda

        public LambdaSettings LambdaSettings = new LambdaSettings();

        #endregion

        #region Lithiio

        public LithiioSettings LithiioSettings = new LithiioSettings();

        #endregion

        #region Pomf

        public PomfUploader PomfUploader = new PomfUploader("https://mixtape.moe/upload.php");

        #endregion

        #region s-ul

        public string SulAPIKey = "";

        #endregion

        #region Seafile

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

        #endregion

        #region Streamable

        public bool StreamableAnonymous = true;
        public string StreamableUsername = "";
        public string StreamablePassword = "";
        public bool StreamableUseDirectURL = false;

        #endregion

        #region Azure Storage

        public string AzureStorageAccountName = "";
        public string AzureStorageAccountAccessKey = "";
        public string AzureStorageContainer = "";
        public string AzureStorageEnvironment = "blob.core.windows.net";
        public string AzureStorageCustomDomain = "";

        #endregion

        #region Plik

        public PlikSettings PlikSettings = new PlikSettings();

        #endregion

        #endregion File uploaders

        #region URL shorteners

        #region bit.ly

        public OAuth2Info BitlyOAuth2Info = null;
        public string BitlyDomain = "";

        #endregion

        #region Google URL Shortener

        public AccountType GoogleURLShortenerAccountType = AccountType.Anonymous;
        public OAuth2Info GoogleURLShortenerOAuth2Info = null;

        #endregion

        #region yourls.org

        public string YourlsAPIURL = "http://yoursite.com/yourls-api.php";
        public string YourlsSignature = "";
        public string YourlsUsername = "";
        public string YourlsPassword = "";

        #endregion

        #region adf.ly

        public string AdFlyAPIKEY = "";
        public string AdFlyAPIUID = "";

        #endregion

        #region coinurl.com

        public string CoinURLUUID = "";

        #endregion

        #region polr

        public string PolrAPIHostname = "";
        public string PolrAPIKey = "";
        public bool PolrIsSecret = false;
        public bool PolrUseAPIv1 = false;

        #endregion

        #endregion URL shorteners

        #region Other uploaders

        #region Twitter

        public List<OAuthInfo> TwitterOAuthInfoList = new List<OAuthInfo>();
        public int TwitterSelectedAccount = 0;
        public bool TwitterSkipMessageBox = false;
        public string TwitterDefaultMessage = "";

        #endregion

        #region Custom uploaders

        public List<CustomUploaderItem> CustomUploadersList = new List<CustomUploaderItem>();
        public int CustomImageUploaderSelected = 0;
        public int CustomTextUploaderSelected = 0;
        public int CustomFileUploaderSelected = 0;
        public int CustomURLShortenerSelected = 0;
        public int CustomURLSharingServiceSelected = 0;

        #endregion Custom uploaders

        #endregion Other uploaders
    }
}