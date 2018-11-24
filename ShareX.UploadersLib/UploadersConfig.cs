#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.UploadersLib.URLShorteners;
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

        #endregion Imgur

        #region ImageShack

        public ImageShackOptions ImageShackSettings = new ImageShackOptions();

        #endregion ImageShack

        #region TinyPic

        public AccountType TinyPicAccountType = AccountType.Anonymous;
        public string TinyPicRegistrationCode = "";
        public string TinyPicUsername = "";
        public string TinyPicPassword = "";

        #endregion TinyPic

        #region Flickr

        public OAuthInfo FlickrOAuthInfo = null;
        public FlickrSettings FlickrSettings = new FlickrSettings();

        #endregion Flickr

        #region Photobucket

        public OAuthInfo PhotobucketOAuthInfo = null;
        public PhotobucketAccountInfo PhotobucketAccountInfo = null;

        #endregion Photobucket

        #region Google Photos

        public OAuth2Info PicasaOAuth2Info = null;
        public string PicasaAlbumID = "";

        #endregion Google Photos

        #region Chevereto

        public CheveretoUploader CheveretoUploader = new CheveretoUploader("http://ultraimg.com/api/1/upload", "3374fa58c672fcaad8dab979f7687397");
        public bool CheveretoDirectURL = true;

        #endregion Chevereto

        #region vgy.me

        public string VgymeUserKey = "";

        #endregion vgy.me

        #endregion Image uploaders

        #region Text uploaders

        #region Pastebin

        public PastebinSettings PastebinSettings = new PastebinSettings();

        #endregion Pastebin

        #region Paste.ee

        public string Paste_eeUserKey = "";
        public bool Paste_eeEncryptPaste = false;

        #endregion Paste.ee

        #region Gist

        public OAuth2Info GistOAuth2Info = null;
        public bool GistPublishPublic = false;
        public bool GistRawURL = false;
        public string GistCustomURL = "";

        #endregion Gist

        #region uPaste

        public string UpasteUserKey = "";
        public bool UpasteIsPublic = false;

        #endregion uPaste

        #region Hastebin

        public string HastebinCustomDomain = "https://hastebin.com";
        public string HastebinSyntaxHighlighting = "hs";
        public bool HastebinUseFileExtension = true;

        #endregion Hastebin

        #region OneTimeSecret

        public string OneTimeSecretAPIKey = "";
        public string OneTimeSecretAPIUsername = "";

        #endregion OneTimeSecret

        #region Pastie

        public bool PastieIsPublic = false;

        #endregion Pastie

        #endregion Text uploaders

        #region File uploaders

        #region Dropbox

        public OAuth2Info DropboxOAuth2Info = null;
        public string DropboxUploadPath = "ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink = true;
        public bool DropboxUseDirectLink = false;

        // TEMP: For backward compatibility
        public DropboxURLType DropboxURLType = DropboxURLType.Default;

        #endregion Dropbox

        #region FTP

        public List<FTPAccount> FTPAccountList = new List<FTPAccount>();
        public int FTPSelectedImage = 0;
        public int FTPSelectedText = 0;
        public int FTPSelectedFile = 0;

        #endregion FTP

        #region OneDrive

        public OAuth2Info OneDriveV2OAuth2Info = null;
        public OneDriveFileInfo OneDriveV2SelectedFolder = OneDrive.RootFolder;
        public bool OneDriveAutoCreateShareableLink = true;

        #endregion OneDrive

        #region Gfycat

        public OAuth2Info GfycatOAuth2Info = null;
        public AccountType GfycatAccountType = AccountType.Anonymous;
        public bool GfycatIsPublic = false;

        #endregion Gfycat

        #region Google Drive

        public OAuth2Info GoogleDriveOAuth2Info = null;
        public bool GoogleDriveIsPublic = true;
        public bool GoogleDriveDirectLink = false;
        public bool GoogleDriveUseFolder = false;
        public string GoogleDriveFolderID = "";

        #endregion Google Drive

        #region puush

        public string PuushAPIKey = "";

        #endregion puush

        #region SendSpace

        public AccountType SendSpaceAccountType = AccountType.Anonymous;
        public string SendSpaceUsername = "";
        public string SendSpacePassword = "";

        #endregion SendSpace

        #region Box

        public OAuth2Info BoxOAuth2Info = null;
        public BoxFileEntry BoxSelectedFolder = Box.RootFolder;
        public bool BoxShare = true;

        #endregion Box

        #region Ge.tt

        public Ge_ttLogin Ge_ttLogin = null;

        #endregion Ge.tt

        #region Localhostr

        public string LocalhostrEmail = "";
        public string LocalhostrPassword = "";
        public bool LocalhostrDirectURL = true;

        #endregion Localhostr

        #region Shared folder

        public List<LocalhostAccount> LocalhostAccountList = new List<LocalhostAccount>();
        public int LocalhostSelectedImages = 0;
        public int LocalhostSelectedText = 0;
        public int LocalhostSelectedFiles = 0;

        #endregion Shared folder

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

        #endregion Email

        #region Jira

        public string JiraHost = "http://";
        public string JiraIssuePrefix = "PROJECT-";
        public OAuthInfo JiraOAuthInfo = null;

        #endregion Jira

        #region Mega

        public MegaApiClient.AuthInfos MegaAuthInfos = null;
        public string MegaParentNodeId = null;

        #endregion Mega

        #region Amazon S3

        public AmazonS3Settings AmazonS3Settings = new AmazonS3Settings()
        {
            ObjectPrefix = "ShareX/%y/%mo"
        };

        #endregion Amazon S3

        #region ownCloud / Nextcloud

        public string OwnCloudHost = "";
        public string OwnCloudUsername = "";
        public string OwnCloudPassword = "";
        public string OwnCloudPath = "/";
        public int OwnCloudExpiryTime = 7;
        public bool OwnCloudCreateShare = true;
        public bool OwnCloudDirectLink = false;
        public bool OwnCloud81Compatibility = true;
        public bool OwnCloudUsePreviewLinks = false;
        public bool OwnCloudAutoExpire = false;

        #endregion ownCloud / Nextcloud

        #region MediaFire

        public string MediaFireUsername = "";
        public string MediaFirePassword = "";
        public string MediaFirePath = "";
        public bool MediaFireUseLongLink = false;

        #endregion MediaFire

        #region Pushbullet

        public PushbulletSettings PushbulletSettings = new PushbulletSettings();

        #endregion Pushbullet

        #region Lambda

        public LambdaSettings LambdaSettings = new LambdaSettings();

        #endregion Lambda

        #region Lithiio

        public LithiioSettings LithiioSettings = new LithiioSettings();

        #endregion Lithiio

        #region Pomf

        public PomfUploader PomfUploader = new PomfUploader("https://mixtape.moe/upload.php");

        #endregion Pomf

        #region s-ul

        public string SulAPIKey = "";

        #endregion s-ul

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

        #endregion Seafile

        #region Streamable

        public bool StreamableAnonymous = true;
        public string StreamableUsername = "";
        public string StreamablePassword = "";
        public bool StreamableUseDirectURL = false;

        #endregion Streamable

        #region Azure Storage

        public string AzureStorageAccountName = "";
        public string AzureStorageAccountAccessKey = "";
        public string AzureStorageContainer = "";
        public string AzureStorageEnvironment = "blob.core.windows.net";
        public string AzureStorageCustomDomain = "";
        public string AzureStorageUploadPath = "";

        #endregion Azure Storage

        #region Backblaze B2

        public string B2ApplicationKeyId = "";
        public string B2ApplicationKey = "";
        public string B2BucketName = "";
        public string B2UploadPath = "ShareX/%y/%mo/";
        public bool B2UseCustomUrl = false;
        public string B2CustomUrl = "https://example.com/";

        #endregion Backblaze B2

        #region Plik

        public PlikSettings PlikSettings = new PlikSettings();

        #endregion Plik

        #region YouTube

        public OAuth2Info YouTubeOAuth2Info = null;
        public YouTubeVideoPrivacy YouTubePrivacyType = YouTubeVideoPrivacy.Public;
        public bool YouTubeUseShortenedLink = false;

        #endregion YouTube

        #region Google Cloud Storage

        public OAuth2Info GoogleCloudStorageOAuth2Info = null;
        public string GoogleCloudStorageBucket = "";
        public string GoogleCloudStorageDomain = "";
        public string GoogleCloudStorageObjectPrefix = "ShareX/%y/%mo";
        public bool GoogleCloudStorageRemoveExtensionImage = false;
        public bool GoogleCloudStorageRemoveExtensionVideo = false;
        public bool GoogleCloudStorageRemoveExtensionText = false;

        #endregion Google Cloud Storage

        #endregion File uploaders

        #region URL shorteners

        #region bit.ly

        public OAuth2Info BitlyOAuth2Info = null;
        public string BitlyDomain = "";

        #endregion bit.ly

        #region yourls.org

        public string YourlsAPIURL = "http://yoursite.com/yourls-api.php";
        public string YourlsSignature = "";
        public string YourlsUsername = "";
        public string YourlsPassword = "";

        #endregion yourls.org

        #region adf.ly

        public string AdFlyAPIKEY = "";
        public string AdFlyAPIUID = "";

        #endregion adf.ly

        #region polr

        public string PolrAPIHostname = "";
        public string PolrAPIKey = "";
        public bool PolrIsSecret = false;
        public bool PolrUseAPIv1 = false;

        #endregion polr

        #region Firebase Dynamic Links

        public string FirebaseWebAPIKey = "";
        public string FirebaseDynamicLinkDomain = "";
        public bool FirebaseIsShort = false;

        #endregion Firebase Dynamic Links

        #region Kutt

        public KuttSettings KuttSettings = new KuttSettings();

        #endregion Kutt

        #endregion URL shorteners

        #region Other uploaders

        #region Twitter

        public List<OAuthInfo> TwitterOAuthInfoList = new List<OAuthInfo>();
        public int TwitterSelectedAccount = 0;
        public bool TwitterSkipMessageBox = false;
        public string TwitterDefaultMessage = "";

        #endregion Twitter

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