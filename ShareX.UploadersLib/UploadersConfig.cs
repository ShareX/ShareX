#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

        public AccountType ImgurAccountType { get; set; } = AccountType.Anonymous;
        public bool ImgurDirectLink { get; set; } = true;
        public ImgurThumbnailType ImgurThumbnailType { get; set; } = ImgurThumbnailType.Medium_Thumbnail;
        public bool ImgurUseGIFV { get; set; } = true;
        public OAuth2Info ImgurOAuth2Info { get; set; } = null;
        public bool ImgurUploadSelectedAlbum { get; set; } = false;
        public ImgurAlbumData ImgurSelectedAlbum { get; set; } = null;
        public List<ImgurAlbumData> ImgurAlbumList { get; set; } = null;

        #endregion Imgur

        #region ImageShack

        public ImageShackOptions ImageShackSettings { get; set; } = new ImageShackOptions();

        #endregion ImageShack

        #region Flickr

        public OAuthInfo FlickrOAuthInfo { get; set; } = null;
        public FlickrSettings FlickrSettings { get; set; } = new FlickrSettings();

        #endregion Flickr

        #region Photobucket

        public OAuthInfo PhotobucketOAuthInfo { get; set; } = null;
        public PhotobucketAccountInfo PhotobucketAccountInfo { get; set; } = null;

        #endregion Photobucket

        #region Chevereto

        public CheveretoUploader CheveretoUploader { get; set; } = new CheveretoUploader();
        public bool CheveretoDirectURL { get; set; } = true;

        #endregion Chevereto

        #region vgy.me

        [JsonEncrypt]
        public string VgymeUserKey { get; set; } = "";

        #endregion vgy.me

        #endregion Image uploaders

        #region Text uploaders

        #region Pastebin

        public PastebinSettings PastebinSettings { get; set; } = new PastebinSettings();

        #endregion Pastebin

        #region Paste.ee

        [JsonEncrypt]
        public string Paste_eeUserKey { get; set; } = "";
        public bool Paste_eeEncryptPaste { get; set; } = false;

        #endregion Paste.ee

        #region Gist

        public OAuth2Info GistOAuth2Info { get; set; } = null;
        public bool GistPublishPublic { get; set; } = false;
        public bool GistRawURL { get; set; } = false;
        public string GistCustomURL { get; set; } = "";

        #endregion Gist

        #region uPaste

        [JsonEncrypt]
        public string UpasteUserKey { get; set; } = "";
        public bool UpasteIsPublic { get; set; } = false;

        #endregion uPaste

        #region Hastebin

        public string HastebinCustomDomain { get; set; } = "https://hastebin.com";
        public string HastebinSyntaxHighlighting { get; set; } = "hs";
        public bool HastebinUseFileExtension { get; set; } = true;

        #endregion Hastebin

        #region OneTimeSecret

        public string OneTimeSecretAPIUsername { get; set; } = "";
        [JsonEncrypt]
        public string OneTimeSecretAPIKey { get; set; } = "";

        #endregion OneTimeSecret

        #region Pastie

        public bool PastieIsPublic { get; set; } = false;

        #endregion Pastie

        #endregion Text uploaders

        #region File uploaders

        #region Dropbox

        public OAuth2Info DropboxOAuth2Info { get; set; } = null;
        public string DropboxUploadPath { get; set; } = "ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink { get; set; } = true;
        public bool DropboxUseDirectLink { get; set; } = false;

        #endregion Dropbox

        #region FTP

        public List<FTPAccount> FTPAccountList { get; set; } = new List<FTPAccount>();
        public int FTPSelectedImage { get; set; } = 0;
        public int FTPSelectedText { get; set; } = 0;
        public int FTPSelectedFile { get; set; } = 0;

        #endregion FTP

        #region OneDrive

        public OAuth2Info OneDriveV2OAuth2Info { get; set; } = null;
        public OneDriveFileInfo OneDriveV2SelectedFolder { get; set; } = OneDrive.RootFolder;
        public bool OneDriveAutoCreateShareableLink { get; set; } = true;
        public bool OneDriveUseDirectLink { get; set; } = false;

        #endregion OneDrive

        #region Google Drive

        public OAuth2Info GoogleDriveOAuth2Info { get; set; } = null;
        public OAuthUserInfo GoogleDriveUserInfo { get; set; } = null;
        public bool GoogleDriveIsPublic { get; set; } = true;
        public bool GoogleDriveDirectLink { get; set; } = false;
        public bool GoogleDriveUseFolder { get; set; } = false;
        public string GoogleDriveFolderID { get; set; } = "";
        public GoogleDriveSharedDrive GoogleDriveSelectedDrive { get; set; } = GoogleDrive.MyDrive;

        #endregion Google Drive

        #region puush

        [JsonEncrypt]
        public string PuushAPIKey { get; set; } = "";

        #endregion puush

        #region SendSpace

        public AccountType SendSpaceAccountType { get; set; } = AccountType.Anonymous;
        public string SendSpaceUsername { get; set; } = "";
        [JsonEncrypt]
        public string SendSpacePassword { get; set; } = "";

        #endregion SendSpace

        #region Box

        public OAuth2Info BoxOAuth2Info { get; set; } = null;
        public BoxFileEntry BoxSelectedFolder { get; set; } = Box.RootFolder;
        public bool BoxShare { get; set; } = true;
        public BoxShareAccessLevel BoxShareAccessLevel { get; set; } = BoxShareAccessLevel.Open;

        #endregion Box

        #region Localhostr

        public string LocalhostrEmail { get; set; } = "";
        [JsonEncrypt]
        public string LocalhostrPassword { get; set; } = "";
        public bool LocalhostrDirectURL { get; set; } = true;

        #endregion Localhostr

        #region Shared folder

        public List<LocalhostAccount> LocalhostAccountList { get; set; } = new List<LocalhostAccount>();
        public int LocalhostSelectedImages { get; set; } = 0;
        public int LocalhostSelectedText { get; set; } = 0;
        public int LocalhostSelectedFiles { get; set; } = 0;

        #endregion Shared folder

        #region Email

        public string EmailSmtpServer { get; set; } = "smtp.gmail.com";
        public int EmailSmtpPort { get; set; } = 587;
        public string EmailFrom { get; set; } = "...@gmail.com";
        [JsonEncrypt]
        public string EmailPassword { get; set; } = "";
        public bool EmailRememberLastTo { get; set; } = true;
        public string EmailLastTo { get; set; } = "";
        public string EmailDefaultSubject { get; set; } = "Sending email from ShareX";
        public string EmailDefaultBody { get; set; } = "Screenshot is attached.";
        public bool EmailAutomaticSend { get; set; } = false;
        public string EmailAutomaticSendTo { get; set; } = "";

        #endregion Email

        #region Jira

        public string JiraHost { get; set; } = "http://";
        public string JiraIssuePrefix { get; set; } = "PROJECT-";
        public OAuthInfo JiraOAuthInfo { get; set; } = null;

        #endregion Jira

        #region Mega

        public MegaAuthInfos MegaAuthInfos { get; set; } = null;
        public string MegaParentNodeId { get; set; } = null;

        #endregion Mega

        #region Amazon S3

        public AmazonS3Settings AmazonS3Settings { get; set; } = new AmazonS3Settings()
        {
            ObjectPrefix = "ShareX/%y/%mo"
        };

        #endregion Amazon S3

        #region ownCloud / Nextcloud

        public string OwnCloudHost { get; set; } = "";
        public string OwnCloudUsername { get; set; } = "";
        [JsonEncrypt]
        public string OwnCloudPassword { get; set; } = "";
        public string OwnCloudPath { get; set; } = "/";
        public int OwnCloudExpiryTime { get; set; } = 7;
        public bool OwnCloudCreateShare { get; set; } = true;
        public bool OwnCloudDirectLink { get; set; } = false;
        public bool OwnCloud81Compatibility { get; set; } = true;
        public bool OwnCloudUsePreviewLinks { get; set; } = false;
        public bool OwnCloudAppendFileNameToURL { get; set; } = false;
        public bool OwnCloudAutoExpire { get; set; } = false;

        #endregion ownCloud / Nextcloud

        #region MediaFire

        public string MediaFireUsername { get; set; } = "";
        [JsonEncrypt]
        public string MediaFirePassword { get; set; } = "";
        public string MediaFirePath { get; set; } = "";
        public bool MediaFireUseLongLink { get; set; } = false;

        #endregion MediaFire

        #region Pushbullet

        public PushbulletSettings PushbulletSettings { get; set; } = new PushbulletSettings();

        #endregion Pushbullet

        #region Lambda

        public LambdaSettings LambdaSettings { get; set; } = new LambdaSettings();

        #endregion Lambda

        #region LobFile

        public LobFileSettings LithiioSettings { get; set; } = new LobFileSettings();

        #endregion

        #region Pomf

        public PomfUploader PomfUploader { get; set; } = new PomfUploader();

        #endregion Pomf

        #region s-ul

        [JsonEncrypt]
        public string SulAPIKey { get; set; } = "";

        #endregion s-ul

        #region Seafile

        public string SeafileAPIURL { get; set; } = "";
        [JsonEncrypt]
        public string SeafileAuthToken { get; set; } = "";
        public string SeafileRepoID { get; set; } = "";
        public string SeafilePath { get; set; } = "/";
        public bool SeafileIsLibraryEncrypted { get; set; } = false;
        [JsonEncrypt]
        public string SeafileEncryptedLibraryPassword { get; set; } = "";
        public bool SeafileCreateShareableURL { get; set; } = true;
        public bool SeafileCreateShareableURLRaw { get; set; } = false;
        public bool SeafileIgnoreInvalidCert { get; set; } = false;
        public int SeafileShareDaysToExpire { get; set; } = 0;
        [JsonEncrypt]
        public string SeafileSharePassword { get; set; } = "";
        public string SeafileAccInfoEmail { get; set; } = "";
        public string SeafileAccInfoUsage { get; set; } = "";

        #endregion Seafile

        #region Streamable

        public string StreamableUsername { get; set; } = "";
        [JsonEncrypt]
        public string StreamablePassword { get; set; } = "";
        public bool StreamableUseDirectURL { get; set; } = false;

        #endregion Streamable

        #region Azure Storage

        public string AzureStorageAccountName { get; set; } = "";
        [JsonEncrypt]
        public string AzureStorageAccountAccessKey { get; set; } = "";
        public string AzureStorageContainer { get; set; } = "";
        public string AzureStorageEnvironment { get; set; } = "blob.core.windows.net";
        public string AzureStorageCustomDomain { get; set; } = "";
        public string AzureStorageUploadPath { get; set; } = "";
        public string AzureStorageCacheControl { get; set; } = "";

        #endregion Azure Storage

        #region Backblaze B2

        public string B2ApplicationKeyId { get; set; } = "";
        [JsonEncrypt]
        public string B2ApplicationKey { get; set; } = "";
        public string B2BucketName { get; set; } = "";
        public string B2UploadPath { get; set; } = "ShareX/%y/%mo";
        public bool B2UseCustomUrl { get; set; } = false;
        public string B2CustomUrl { get; set; } = "https://example.com";

        #endregion Backblaze B2

        #region Plik

        public PlikSettings PlikSettings { get; set; } = new PlikSettings();

        #endregion Plik

        #region YouTube

        public OAuth2Info YouTubeOAuth2Info { get; set; } = null;
        public OAuthUserInfo YouTubeUserInfo { get; set; } = null;
        public YouTubeVideoPrivacy YouTubePrivacyType { get; set; } = YouTubeVideoPrivacy.Public;
        public bool YouTubeUseShortenedLink { get; set; } = false;
        public bool YouTubeShowDialog { get; set; } = false;

        #endregion YouTube

        #region Google Cloud Storage

        public OAuth2Info GoogleCloudStorageOAuth2Info { get; set; } = null;
        public OAuthUserInfo GoogleCloudStorageUserInfo { get; set; } = null;
        public string GoogleCloudStorageBucket { get; set; } = "";
        public string GoogleCloudStorageDomain { get; set; } = "";
        public string GoogleCloudStorageObjectPrefix { get; set; } = "ShareX/%y/%mo";
        public bool GoogleCloudStorageRemoveExtensionImage { get; set; } = false;
        public bool GoogleCloudStorageRemoveExtensionVideo { get; set; } = false;
        public bool GoogleCloudStorageRemoveExtensionText { get; set; } = false;
        public bool GoogleCloudStorageSetPublicACL { get; set; } = true;

        #endregion Google Cloud Storage

        #endregion File uploaders

        #region URL shorteners

        #region bit.ly

        public OAuth2Info BitlyOAuth2Info { get; set; } = null;
        public string BitlyDomain { get; set; } = "";

        #endregion bit.ly

        #region yourls.org

        public string YourlsAPIURL { get; set; } = "http://yoursite.com/yourls-api.php";
        [JsonEncrypt]
        public string YourlsSignature { get; set; } = "";
        public string YourlsUsername { get; set; } = "";
        [JsonEncrypt]
        public string YourlsPassword { get; set; } = "";

        #endregion yourls.org

        #region polr

        public string PolrAPIHostname { get; set; } = "";
        [JsonEncrypt]
        public string PolrAPIKey { get; set; } = "";
        public bool PolrIsSecret { get; set; } = false;
        public bool PolrUseAPIv1 { get; set; } = false;

        #endregion polr

        #region Firebase Dynamic Links

        [JsonEncrypt]
        public string FirebaseWebAPIKey { get; set; } = "";
        public string FirebaseDynamicLinkDomain { get; set; } = "";
        public bool FirebaseIsShort { get; set; } = false;

        #endregion Firebase Dynamic Links

        #region Kutt

        public KuttSettings KuttSettings { get; set; } = new KuttSettings();

        #endregion Kutt

        #region Zero Width Shortener

        public string ZeroWidthShortenerURL { get; set; } = "https://api.zws.im";
        public string ZeroWidthShortenerToken { get; set; } = "";

        #endregion

        #endregion URL shorteners

        #region Other uploaders

        #region Twitter

        public List<OAuthInfo> TwitterOAuthInfoList { get; set; } = new List<OAuthInfo>();
        public int TwitterSelectedAccount { get; set; } = 0;
        public bool TwitterSkipMessageBox { get; set; } = false;
        public string TwitterDefaultMessage { get; set; } = "";

        #endregion Twitter

        #region Custom uploaders

        public List<CustomUploaderItem> CustomUploadersList { get; set; } = new List<CustomUploaderItem>();
        public int CustomImageUploaderSelected { get; set; } = 0;
        public int CustomTextUploaderSelected { get; set; } = 0;
        public int CustomFileUploaderSelected { get; set; } = 0;
        public int CustomURLShortenerSelected { get; set; } = 0;
        public int CustomURLSharingServiceSelected { get; set; } = 0;

        #endregion Custom uploaders

        #endregion Other uploaders
    }
}