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

using CG.Web.MegaApiClient;
using HelpersLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UploadersLib.FileUploaders;
using UploadersLib.HelperClasses;
using UploadersLib.ImageUploaders;
using UploadersLib.TextUploaders;

namespace UploadersLib
{
    public class UploadersConfig : SettingsBase<UploadersConfig>
    {
        #region Image uploaders

        // Imgur

        public AccountType ImgurAccountType = AccountType.Anonymous;
        public ImgurThumbnailType ImgurThumbnailType = ImgurThumbnailType.Large_Thumbnail;
        public OAuth2Info ImgurOAuth2Info = null;
        public string ImgurAlbumID = string.Empty;

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

        // uPaste

        public string UpasteUserKey = string.Empty;
        public bool UpasteIsPublic = false;

        // Pushbullet

        public PushbulletSettings PushbulletSettings = new PushbulletSettings();

        #endregion Text uploaders

        #region File uploaders

        // Dropbox

        public OAuth2Info DropboxOAuth2Info = null;
        public DropboxAccountInfo DropboxAccountInfo = null;
        public string DropboxUploadPath = "Public/ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink = false;
        public DropboxURLType DropboxURLType = DropboxURLType.Default;

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

        // RapidShare

        public string RapidShareUsername = string.Empty;
        public string RapidSharePassword = string.Empty;
        public string RapidShareFolderID = string.Empty;

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
        public string EmailDefaultSubject = "Sending email from " + Application.ProductName;
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
        public string adflyAPIKEY = String.Empty;
        public string adflyAPIUID = String.Empty;

        #endregion URL shorteners

        #region URL sharing services

        // Twitter

        public List<OAuthInfo> TwitterOAuthInfoList = new List<OAuthInfo>();
        public int TwitterSelectedAccount = 0;

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
            switch (destination)
            {
                case ImageDestination.Imgur:
                    return ImgurAccountType == AccountType.Anonymous || OAuth2Info.CheckOAuth(ImgurOAuth2Info);
                case ImageDestination.ImageShack:
                    return ImageShackSettings != null && !string.IsNullOrEmpty(ImageShackSettings.Auth_token);
                case ImageDestination.TinyPic:
                    return TinyPicAccountType == AccountType.Anonymous || !string.IsNullOrEmpty(TinyPicRegistrationCode);
                case ImageDestination.Flickr:
                    return !string.IsNullOrEmpty(FlickrAuthInfo.Token);
                case ImageDestination.Photobucket:
                    return PhotobucketAccountInfo != null && OAuthInfo.CheckOAuth(PhotobucketOAuthInfo);
                case ImageDestination.Picasa:
                    return OAuth2Info.CheckOAuth(PicasaOAuth2Info);
                case ImageDestination.Twitter:
                    return TwitterOAuthInfoList != null && TwitterOAuthInfoList.IsValidIndex(TwitterSelectedAccount) && OAuthInfo.CheckOAuth(TwitterOAuthInfoList[TwitterSelectedAccount]);
                case ImageDestination.CustomImageUploader:
                    return CustomUploadersList != null && CustomUploadersList.IsValidIndex(CustomImageUploaderSelected);
            }

            return true;
        }

        public bool IsValid(TextDestination destination)
        {
            switch (destination)
            {
                case TextDestination.CustomTextUploader:
                    return CustomUploadersList != null && CustomUploadersList.IsValidIndex(CustomTextUploaderSelected);
            }

            return true;
        }

        public bool IsValid(FileDestination destination)
        {
            switch (destination)
            {
                case FileDestination.Dropbox:
                    return OAuth2Info.CheckOAuth(DropboxOAuth2Info);
                case FileDestination.Copy:
                    return OAuthInfo.CheckOAuth(CopyOAuthInfo);
                case FileDestination.GoogleDrive:
                    return OAuth2Info.CheckOAuth(GoogleDriveOAuth2Info);
                case FileDestination.RapidShare:
                    return !string.IsNullOrEmpty(RapidShareUsername) && !string.IsNullOrEmpty(RapidSharePassword);
                case FileDestination.SendSpace:
                    return SendSpaceAccountType == AccountType.Anonymous || (!string.IsNullOrEmpty(SendSpaceUsername) && !string.IsNullOrEmpty(SendSpacePassword));
                case FileDestination.Minus:
                    return MinusConfig != null && MinusConfig.MinusUser != null;
                case FileDestination.Box:
                    return OAuth2Info.CheckOAuth(BoxOAuth2Info);
                case FileDestination.Ge_tt:
                    return Ge_ttLogin != null && !string.IsNullOrEmpty(Ge_ttLogin.AccessToken);
                case FileDestination.Localhostr:
                    return !string.IsNullOrEmpty(LocalhostrEmail) && !string.IsNullOrEmpty(LocalhostrPassword);
                case FileDestination.CustomFileUploader:
                    return CustomUploadersList != null && CustomUploadersList.IsValidIndex(CustomFileUploaderSelected);
                case FileDestination.FTP:
                    return FTPAccountList != null && FTPAccountList.IsValidIndex(FTPSelectedFile);
                case FileDestination.SharedFolder:
                    return LocalhostAccountList != null && LocalhostAccountList.IsValidIndex(LocalhostSelectedFiles);
                case FileDestination.Email:
                    return !string.IsNullOrEmpty(EmailSmtpServer) && EmailSmtpPort > 0 && !string.IsNullOrEmpty(EmailFrom) && !string.IsNullOrEmpty(EmailPassword);
                case FileDestination.Jira:
                    return OAuthInfo.CheckOAuth(JiraOAuthInfo);
                case FileDestination.Mega:
                    return MegaAuthInfos != null && MegaAuthInfos.Email != null && MegaAuthInfos.Hash != null && MegaAuthInfos.PasswordAesKey != null;
                case FileDestination.Pushbullet:
                    return PushbulletSettings != null && !string.IsNullOrEmpty(PushbulletSettings.UserAPIKey) && PushbulletSettings.DeviceList != null &&
                        PushbulletSettings.DeviceList.IsValidIndex(PushbulletSettings.SelectedDevice);
                case FileDestination.OwnCloud:
                    return !string.IsNullOrEmpty(OwnCloudHost) && !string.IsNullOrEmpty(OwnCloudUsername) && !string.IsNullOrEmpty(OwnCloudPassword);
            }

            return true;
        }

        public bool IsValid(UrlShortenerType destination)
        {
            switch (destination)
            {
                case UrlShortenerType.Google:
                    return GoogleURLShortenerAccountType == AccountType.Anonymous || OAuth2Info.CheckOAuth(GoogleURLShortenerOAuth2Info);
                case UrlShortenerType.BITLY:
                    return OAuth2Info.CheckOAuth(BitlyOAuth2Info);
                case UrlShortenerType.YOURLS:
                    return !string.IsNullOrEmpty(YourlsAPIURL) && (!string.IsNullOrEmpty(YourlsSignature) || (!string.IsNullOrEmpty(YourlsUsername) && !string.IsNullOrEmpty(YourlsPassword)));
                case UrlShortenerType.adfly:
                    return !string.IsNullOrEmpty(adflyAPIKEY) && !string.IsNullOrEmpty(adflyAPIUID);
                case UrlShortenerType.CustomURLShortener:
                    return CustomUploadersList != null && CustomUploadersList.IsValidIndex(CustomURLShortenerSelected);
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
            }

            return true;
        }

        public int GetFTPIndex(EDataType dataType)
        {
            switch (dataType)
            {
                case EDataType.Image:
                    return FTPSelectedImage;
                case EDataType.Text:
                    return FTPSelectedText;
                default:
                case EDataType.File:
                    return FTPSelectedFile;
            }
        }

        public int GetLocalhostIndex(EDataType dataType)
        {
            switch (dataType)
            {
                case EDataType.Image:
                    return LocalhostSelectedImages;
                case EDataType.Text:
                    return LocalhostSelectedText;
                default:
                case EDataType.File:
                    return LocalhostSelectedFiles;
            }
        }

        #endregion Helper Methods
    }
}