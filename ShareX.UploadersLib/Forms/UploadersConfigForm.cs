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
using ShareX.UploadersLib.Properties;
using ShareX.UploadersLib.TextUploaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class UploadersConfigForm : Form
    {
        public static bool IsInstanceActive => instance != null && !instance.IsDisposed;

        private static UploadersConfigForm instance;

        public UploadersConfig Config { get; private set; }

        private ImageList uploadersImageList;
        private URLType customUploaderURLType = URLType.URL;

        private UploadersConfigForm(UploadersConfig config)
        {
            Config = config;
            InitializeComponent();
            InitializeControls();
        }

        public static UploadersConfigForm GetFormInstance(UploadersConfig config)
        {
            if (!IsInstanceActive)
            {
                instance = new UploadersConfigForm(config);
            }

            return instance;
        }

        private void UploadersConfigForm_Shown(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void UploadersConfigForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void InitializeControls()
        {
            Icon = ShareXResources.Icon;

            if (!string.IsNullOrEmpty(Config.FilePath))
            {
                Text += " - " + Config.FilePath;
            }

            AddIconToTabs();

            ttlvMain.ImageList = uploadersImageList;
            ttlvMain.MainTabControl = tcUploaders;
            ttlvMain.FocusListView();

            CodeMenu.Create<CodeMenuEntryFilename>(txtDropboxPath, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);
            CodeMenu.Create<CodeMenuEntryFilename>(txtAmazonS3ObjectPrefix, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);
            CodeMenu.Create<CodeMenuEntryFilename>(txtMediaFirePath, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);
            CodeMenu.Create<CodeMenuEntryFilename>(txtCustomUploaderArgValue, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);
            CodeMenu.Create<CodeMenuEntryFilename>(txtCustomUploaderHeaderValue, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);

            // FTP
            cbFTPURLPathProtocol.Items.AddRange(Helpers.GetEnumDescriptions<BrowserProtocol>());
            cbFTPSEncryption.Items.AddRange(Enum.GetNames(typeof(FTPSEncryption)));
            eiFTP.ObjectType = typeof(FTPAccount);

            // Localhost
            ucLocalhostAccounts.btnAdd.Click += LocalhostAccountAddButton_Click;
            ucLocalhostAccounts.btnRemove.Click += LocalhostAccountRemoveButton_Click;
            ucLocalhostAccounts.btnDuplicate.Click += LocalhostAccountDuplicateButton_Click;
            ucLocalhostAccounts.btnTest.Visible = false;
            ucLocalhostAccounts.pgSettings.PropertyValueChanged += SettingsGrid_LocalhostPropertyValueChanged;

            // Custom uploader
            txtCustomUploaderLog.AddContextMenu();
            eiCustomUploaders.ObjectType = typeof(CustomUploaderItem);
            txtCustomUploaderName.HandleCreated += (sender, e) => txtCustomUploaderName.SetWatermark(Resources.UploadersConfigForm_CustomUploader_Name);
            AddCustomUploaderDestinationTypes();
            cbCustomUploaderRequestType.Items.AddRange(Enum.GetNames(typeof(CustomUploaderRequestType)));
            cbCustomUploaderResponseType.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ResponseType>());

#if DEBUG
            btnCheveretoTestAll.Visible = true;
            btnPomfTest.Visible = true;
#endif
        }

        private void AddIconToTabs()
        {
            uploadersImageList = new ImageList();
            uploadersImageList.ColorDepth = ColorDepth.Depth32Bit;

            foreach (IUploaderService uploaderService in UploaderFactory.AllServices)
            {
                TabPage tp = uploaderService.GetUploadersConfigTabPage(this);

                if (tp != null && string.IsNullOrEmpty(tp.ImageKey))
                {
                    Icon icon = uploaderService.ServiceIcon;

                    if (icon != null)
                    {
                        uploadersImageList.Images.Add(tp.Name, icon);
                        tp.ImageKey = tp.Name;
                    }
                    else
                    {
                        Image img = uploaderService.ServiceImage;

                        if (img != null)
                        {
                            uploadersImageList.Images.Add(tp.Name, img);
                            tp.ImageKey = tp.Name;
                        }
                    }
                }
            }
        }

        public void NavigateToTabPage(TabPage tp)
        {
            if (tp != null)
            {
                ttlvMain.NavigateToTabPage(tp);
            }
        }

        public void LoadSettings()
        {
            #region Image uploaders

            // Imgur

            oauth2Imgur.Enabled = Config.ImgurAccountType == AccountType.User;

            if (OAuth2Info.CheckOAuth(Config.ImgurOAuth2Info))
            {
                oauth2Imgur.Status = OAuthLoginStatus.LoginSuccessful;
                btnImgurRefreshAlbumList.Enabled = true;
            }

            atcImgurAccountType.SelectedAccountType = Config.ImgurAccountType;
            cbImgurDirectLink.Checked = Config.ImgurDirectLink;
            cbImgurThumbnailType.Items.Clear();
            cbImgurThumbnailType.Items.AddRange(Helpers.GetEnumDescriptions<ImgurThumbnailType>());
            cbImgurThumbnailType.SelectedIndex = (int)Config.ImgurThumbnailType;
            cbImgurUseHTTPS.Checked = Config.ImgurUseHTTPS;
            cbImgurUseGIFV.Checked = Config.ImgurUseGIFV;
            cbImgurUploadSelectedAlbum.Checked = Config.ImgurUploadSelectedAlbum;
            ImgurFillAlbumList();

            // ImageShack

            txtImageShackUsername.Text = Config.ImageShackSettings.Username;
            txtImageShackPassword.Text = Config.ImageShackSettings.Password;
            cbImageShackIsPublic.Checked = Config.ImageShackSettings.IsPublic;

            // TinyPic

            atcTinyPicAccountType.SelectedAccountType = Config.TinyPicAccountType;
            txtTinyPicUsername.Text = Config.TinyPicUsername;
            txtTinyPicPassword.Text = Config.TinyPicPassword;

            // Flickr

            pgFlickrAuthInfo.SelectedObject = Config.FlickrAuthInfo;
            pgFlickrSettings.SelectedObject = Config.FlickrSettings;

            // Photobucket

            if (OAuthInfo.CheckOAuth(Config.PhotobucketOAuthInfo))
            {
                lblPhotobucketAccountStatus.Text = Resources.UploadersConfigForm_Login_successful;
                txtPhotobucketDefaultAlbumName.Text = Config.PhotobucketAccountInfo.AlbumID;
                lblPhotobucketParentAlbumPath.Text = Resources.UploadersConfigForm_LoadSettings_Parent_album_path_e_g_ + " " +
                    Config.PhotobucketAccountInfo.AlbumID + "/Personal/" + DateTime.Now.Year;
            }

            if (Config.PhotobucketAccountInfo != null)
            {
                cboPhotobucketAlbumPaths.Items.Clear();

                if (Config.PhotobucketAccountInfo.AlbumList.Count > 0)
                {
                    cboPhotobucketAlbumPaths.Items.AddRange(Config.PhotobucketAccountInfo.AlbumList.ToArray());
                    cboPhotobucketAlbumPaths.SelectedIndex = Config.PhotobucketAccountInfo.ActiveAlbumID.
                        BetweenOrDefault(0, Config.PhotobucketAccountInfo.AlbumList.Count - 1);
                }
            }

            // Picasa

            if (OAuth2Info.CheckOAuth(Config.PicasaOAuth2Info))
            {
                oauth2Picasa.Status = OAuthLoginStatus.LoginSuccessful;
                btnPicasaRefreshAlbumList.Enabled = true;
            }

            txtPicasaAlbumID.Text = Config.PicasaAlbumID;

            // Chevereto

            if (Config.CheveretoUploader == null) Config.CheveretoUploader = new CheveretoUploader();
            cbCheveretoUploaders.Items.AddRange(Chevereto.Uploaders.ToArray());
            txtCheveretoUploadURL.Text = Config.CheveretoUploader.UploadURL;
            txtCheveretoAPIKey.Text = Config.CheveretoUploader.APIKey;
            cbCheveretoDirectURL.Checked = Config.CheveretoDirectURL;

            // SomeImage

            txtSomeImageAPIKey.Text = Config.SomeImageAPIKey;
            cbSomeImageDirectURL.Checked = Config.SomeImageDirectURL;

            // vgy.me

            txtVgymeUserKey.Text = Config.VgymeUserKey;

            #endregion Image uploaders

            #region Text uploaders

            // Pastebin

            txtPastebinUsername.Text = Config.PastebinSettings.Username;
            txtPastebinPassword.Text = Config.PastebinSettings.Password;
            UpdatePastebinStatus();
            cbPastebinPrivacy.Items.AddRange(Helpers.GetEnumDescriptions<PastebinPrivacy>());
            cbPastebinPrivacy.SelectedIndex = (int)Config.PastebinSettings.Exposure;
            cbPastebinExpiration.Items.AddRange(Helpers.GetEnumDescriptions<PastebinExpiration>());
            cbPastebinExpiration.SelectedIndex = (int)Config.PastebinSettings.Expiration;
            cbPastebinSyntax.Items.AddRange(Pastebin.GetSyntaxList().ToArray());
            cbPastebinSyntax.SelectedIndex = 0;
            for (int i = 0; i < cbPastebinSyntax.Items.Count; i++)
            {
                PastebinSyntaxInfo pastebinSyntaxInfo = (PastebinSyntaxInfo)cbPastebinSyntax.Items[i];
                if (pastebinSyntaxInfo.Value.Equals(Config.PastebinSettings.TextFormat, StringComparison.InvariantCultureIgnoreCase))
                {
                    cbPastebinSyntax.SelectedIndex = i;
                    break;
                }
            }
            txtPastebinTitle.Text = Config.PastebinSettings.Title;
            cbPastebinRaw.Checked = Config.PastebinSettings.RawURL;

            // Paste.ee

            txtPaste_eeUserAPIKey.Text = Config.Paste_eeUserAPIKey;

            // Gist

            atcGistAccountType.SelectedAccountType = Config.GistAnonymousLogin ? AccountType.Anonymous : AccountType.User;

            if (OAuth2Info.CheckOAuth(Config.GistOAuth2Info))
            {
                oAuth2Gist.Status = OAuthLoginStatus.LoginSuccessful;
            }

            cbGistPublishPublic.Checked = Config.GistPublishPublic;
            cbGistUseRawURL.Checked = Config.GistRawURL;
            txtGistCustomURL.Text = Config.GistCustomURL;

            // Upaste

            txtUpasteUserKey.Text = Config.UpasteUserKey;
            cbUpasteIsPublic.Checked = Config.UpasteIsPublic;

            // Hastebin

            txtHastebinCustomDomain.Text = Config.HastebinCustomDomain;
            txtHastebinSyntaxHighlighting.Text = Config.HastebinSyntaxHighlighting;
            cbHastebinUseFileExtension.Checked = Config.HastebinUseFileExtension;

            // OneTimeSecret

            txtOneTimeSecretEmail.Text = Config.OneTimeSecretAPIUsername;
            txtOneTimeSecretAPIKey.Text = Config.OneTimeSecretAPIKey;

            // Pastie

            cbPastieIsPublic.Checked = Config.PastieIsPublic;

            #endregion Text uploaders

            #region File uploaders

            // FTP

            if (Config.FTPAccountList == null || Config.FTPAccountList.Count == 0)
            {
                FTPUpdateControls(new List<FTPAccount>());
                FTPClearFields();
            }
            else
            {
                FTPUpdateControls();
                cbFTPAccounts.SelectedIndex = cbFTPImage.SelectedIndex;
                FTPUpdateEnabledStates();
            }

            // Dropbox

            if (OAuth2Info.CheckOAuth(Config.DropboxOAuth2Info))
            {
                oauth2Dropbox.Status = OAuthLoginStatus.LoginSuccessful;
            }

            txtDropboxPath.Text = Config.DropboxUploadPath;
            cbDropboxAutoCreateShareableLink.Checked = Config.DropboxAutoCreateShareableLink;
            cbDropboxUseDirectLink.Enabled = Config.DropboxAutoCreateShareableLink;
            cbDropboxUseDirectLink.Checked = Config.DropboxUseDirectLink;

            // OneDrive

            tvOneDrive.Nodes.Clear();
            OneDriveAddFolder(OneDrive.RootFolder, null);

            if (OAuth2Info.CheckOAuth(Config.OneDriveOAuth2Info))
            {
                oAuth2OneDrive.Status = OAuthLoginStatus.LoginSuccessful;
            }

            cbOneDriveCreateShareableLink.Checked = Config.OneDriveAutoCreateShareableLink;
            lblOneDriveFolderID.Text = Resources.UploadersConfigForm_LoadSettings_Selected_folder_ + " " + Config.OneDriveSelectedFolder.name;
            tvOneDrive.CollapseAll();

            // Google Drive

            if (OAuth2Info.CheckOAuth(Config.GoogleDriveOAuth2Info))
            {
                oauth2GoogleDrive.Status = OAuthLoginStatus.LoginSuccessful;
                btnGoogleDriveRefreshFolders.Enabled = true;

                tvOneDrive.Enabled = true;
            }

            cbGoogleDriveIsPublic.Checked = Config.GoogleDriveIsPublic;
            cbGoogleDriveDirectLink.Checked = Config.GoogleDriveDirectLink;
            cbGoogleDriveUseFolder.Checked = Config.GoogleDriveUseFolder;
            txtGoogleDriveFolderID.Enabled = Config.GoogleDriveUseFolder;
            txtGoogleDriveFolderID.Text = Config.GoogleDriveFolderID;

            // puush

            txtPuushAPIKey.Text = Config.PuushAPIKey;

            // Minus

            cbMinusURLType.Items.Clear();
            cbMinusURLType.Items.AddRange(Enum.GetNames(typeof(MinusLinkType)));
            MinusUpdateControls();

            // Box

            if (OAuth2Info.CheckOAuth(Config.BoxOAuth2Info))
            {
                oauth2Box.Status = OAuthLoginStatus.LoginSuccessful;
                btnBoxRefreshFolders.Enabled = true;
            }

            cbBoxShare.Checked = Config.BoxShare;
            lblBoxFolderID.Text = Resources.UploadersConfigForm_LoadSettings_Selected_folder_ + " " + Config.BoxSelectedFolder.name;

            // Ge.tt

            if (Config.Ge_ttLogin != null && !string.IsNullOrEmpty(Config.Ge_ttLogin.AccessToken))
            {
                lblGe_ttStatus.Text = Resources.UploadersConfigForm_Login_successful;
            }

            // Localhostr

            txtLocalhostrEmail.Text = Config.LocalhostrEmail;
            txtLocalhostrPassword.Text = Config.LocalhostrPassword;
            cbLocalhostrDirectURL.Checked = Config.LocalhostrDirectURL;

            // Email

            txtEmailSmtpServer.Text = Config.EmailSmtpServer;
            nudEmailSmtpPort.SetValue(Config.EmailSmtpPort);
            txtEmailFrom.Text = Config.EmailFrom;
            txtEmailPassword.Text = Config.EmailPassword;
            cbEmailRememberLastTo.Checked = Config.EmailRememberLastTo;
            txtEmailDefaultSubject.Text = Config.EmailDefaultSubject;
            txtEmailDefaultBody.Text = Config.EmailDefaultBody;
            cbEmailAutomaticSend.Checked = Config.EmailAutomaticSend;
            txtEmailAutomaticSendTo.Enabled = Config.EmailAutomaticSend;
            txtEmailAutomaticSendTo.Text = Config.EmailAutomaticSendTo;

            // SendSpace

            atcSendSpaceAccountType.SelectedAccountType = Config.SendSpaceAccountType;
            txtSendSpacePassword.Text = Config.SendSpacePassword;
            txtSendSpaceUserName.Text = Config.SendSpaceUsername;

            // Localhost

            if (Config.LocalhostAccountList == null || Config.LocalhostAccountList.Count == 0)
            {
                LocalhostAccountsSetup(new List<LocalhostAccount>());
            }
            else
            {
                LocalhostAccountsSetup(Config.LocalhostAccountList);
                if (ucLocalhostAccounts.lbAccounts.Items.Count > 0)
                {
                    ucLocalhostAccounts.lbAccounts.SelectedIndex = 0;
                    cboSharedFolderImages.SelectedIndex = Config.LocalhostSelectedImages.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                    cboSharedFolderText.SelectedIndex = Config.LocalhostSelectedText.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                    cboSharedFolderFiles.SelectedIndex = Config.LocalhostSelectedFiles.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                }
            }

            // Jira

            txtJiraHost.Text = Config.JiraHost;
            txtJiraIssuePrefix.Text = Config.JiraIssuePrefix;

            try
            {
                txtJiraConfigHelp.Text = string.Format(@"How to configure your Jira server:

- Go to 'Administration' -> 'Add-ons'
- Select 'Application Links'

- Add a new 'Application Link' with following settings:
    - Server URL: {0}
    - Application Name: {1}
    - Application Type: Generic Application

- Now, you have to configure Incoming Authentication
        - Consumer Key: {2}
        - Consumer Name: {1}
        - Public Key (without quotes): '{3}'

- You can now authenticate to Jira", Links.URL_WEBSITE, "ShareX", APIKeys.JiraConsumerKey, Jira.PublicKey);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            if (OAuthInfo.CheckOAuth(Config.JiraOAuthInfo))
            {
                oAuthJira.Status = OAuthLoginStatus.LoginSuccessful;
            }

            // Mega

            MegaConfigureTab(false);

            //Pushbullet

            txtPushbulletUserKey.Text = Config.PushbulletSettings.UserAPIKey;

            if (Config.PushbulletSettings.DeviceList.Count > 0)
            {
                Config.PushbulletSettings.DeviceList.ForEach(x => cboPushbulletDevices.Items.Add(x.Name ?? Resources.UploadersConfigForm_LoadSettings_Invalid_device_name));

                if (Config.PushbulletSettings.DeviceList.IsValidIndex(Config.PushbulletSettings.SelectedDevice))
                {
                    cboPushbulletDevices.SelectedIndex = Config.PushbulletSettings.SelectedDevice;
                }
                else
                {
                    cboPushbulletDevices.SelectedIndex = 0;
                }
            }

            // Amazon S3

            txtAmazonS3AccessKey.Text = Config.AmazonS3Settings.AccessKeyID;
            txtAmazonS3SecretKey.Text = Config.AmazonS3Settings.SecretAccessKey;
            cbAmazonS3Endpoints.Items.AddRange(AmazonS3.Endpoints.ToArray());
            for (int i = 0; i < cbAmazonS3Endpoints.Items.Count; i++)
            {
                if (((AmazonS3Endpoint)cbAmazonS3Endpoints.Items[i]).Endpoint.Equals(Config.AmazonS3Settings.Endpoint, StringComparison.InvariantCultureIgnoreCase))
                {
                    cbAmazonS3Endpoints.SelectedIndex = i;
                    break;
                }
            }
            txtAmazonS3Endpoint.Text = Config.AmazonS3Settings.Endpoint;
            txtAmazonS3Region.Text = Config.AmazonS3Settings.Region;
            cbAmazonS3UsePathStyle.Checked = Config.AmazonS3Settings.UsePathStyle;
            txtAmazonS3BucketName.Text = Config.AmazonS3Settings.Bucket;
            txtAmazonS3ObjectPrefix.Text = Config.AmazonS3Settings.ObjectPrefix;
            cbAmazonS3CustomCNAME.Checked = Config.AmazonS3Settings.UseCustomCNAME;
            txtAmazonS3CustomDomain.Enabled = Config.AmazonS3Settings.UseCustomCNAME;
            txtAmazonS3CustomDomain.Text = Config.AmazonS3Settings.CustomDomain;
            cbAmazonS3UseRRS.Checked = Config.AmazonS3Settings.UseReducedRedundancyStorage;
            UpdateAmazonS3Status();

            // ownCloud

            txtOwnCloudHost.Text = Config.OwnCloudHost;
            txtOwnCloudUsername.Text = Config.OwnCloudUsername;
            txtOwnCloudPassword.Text = Config.OwnCloudPassword;
            txtOwnCloudPath.Text = Config.OwnCloudPath;
            cbOwnCloudCreateShare.Checked = Config.OwnCloudCreateShare;
            cbOwnCloudDirectLink.Checked = Config.OwnCloudDirectLink;
            cbOwnCloud81Compatibility.Checked = Config.OwnCloud81Compatibility;

            // MediaFire

            txtMediaFireEmail.Text = Config.MediaFireUsername;
            txtMediaFirePassword.Text = Config.MediaFirePassword;
            txtMediaFirePath.Text = Config.MediaFirePath;
            cbMediaFireUseLongLink.Checked = Config.MediaFireUseLongLink;

            // Lambda

            txtLambdaApiKey.Text = Config.LambdaSettings.UserAPIKey;
            cbLambdaUploadURL.Items.AddRange(Lambda.UploadURLs);
            cbLambdaUploadURL.SelectedItem = Config.LambdaSettings.UploadURL;

            // Lithiio

            txtLithiioApiKey.Text = Config.LithiioSettings.UserAPIKey;

            // Pomf

            if (Config.PomfUploader == null) Config.PomfUploader = new PomfUploader();
            cbPomfUploaders.Items.AddRange(Pomf.Uploaders.ToArray());
            txtPomfUploadURL.Text = Config.PomfUploader.UploadURL;
            txtPomfResultURL.Text = Config.PomfUploader.ResultURL;

            // Seafile

            cbSeafileAPIURL.Text = Config.SeafileAPIURL;
            txtSeafileAuthToken.Text = Config.SeafileAuthToken;
            txtSeafileDirectoryPath.Text = Config.SeafilePath;
            txtSeafileLibraryPassword.Text = Config.SeafileEncryptedLibraryPassword;
            txtSeafileLibraryPassword.ReadOnly = Config.SeafileIsLibraryEncrypted;
            btnSeafileLibraryPasswordValidate.Enabled = !Config.SeafileIsLibraryEncrypted;
            cbSeafileCreateShareableURL.Checked = Config.SeafileCreateShareableURL;
            cbSeafileIgnoreInvalidCert.Checked = Config.SeafileIgnoreInvalidCert;
            nudSeafileExpireDays.SetValue(Config.SeafileShareDaysToExpire);
            txtSeafileSharePassword.Text = Config.SeafileSharePassword;
            txtSeafileAccInfoEmail.Text = Config.SeafileAccInfoEmail;
            txtSeafileAccInfoUsage.Text = Config.SeafileAccInfoUsage;

            // Streamable

            cbStreamableAnonymous.Checked = Config.StreamableAnonymous;
            txtStreamablePassword.Text = Config.StreamablePassword;
            txtStreamableUsername.Text = Config.StreamableUsername;
            txtStreamableUsername.Enabled = txtStreamablePassword.Enabled = !Config.StreamableAnonymous;
            cbStreamableUseDirectURL.Checked = Config.StreamableUseDirectURL;

            // s-ul

            txtSulAPIKey.Text = Config.SulAPIKey;

            // Uplea

            txtUpleaApiKey.Text = Config.UpleaApiKey;
            txtUpleaEmailAddress.Text = Config.UpleaEmailAddress;
            cbUpleaInstantDownloadEnabled.Checked = Config.UpleaInstantDownloadEnabled;
            cbUpleaIsPremium.Checked = Config.UpleaIsPremiumMember;

            // Azure Storage

            txtAzureStorageAccountName.Text = Config.AzureStorageAccountName;
            txtAzureStorageAccessKey.Text = Config.AzureStorageAccountAccessKey;
            txtAzureStorageContainer.Text = Config.AzureStorageContainer;
            cbAzureStorageEnvironment.Text = Config.AzureStorageEnvironment;
            txtAzureStorageCustomDomain.Text = Config.AzureStorageCustomDomain;

            // Plik

            txtPlikAPIKey.Text = Config.PlikSettings.APIKey;
            txtPlikURL.Text = Config.PlikSettings.URL;
            txtPlikPassword.Text = Config.PlikSettings.Password;
            txtPlikLogin.Text = Config.PlikSettings.Login;
            txtPlikComment.Text = Config.PlikSettings.Comment;
            cbPlikComment.Checked = Config.PlikSettings.HasComment;
            cbPlikIsSecured.Checked = Config.PlikSettings.IsSecured;
            cbPlikRemovable.Checked = Config.PlikSettings.Removable;
            cbPlikOneShot.Checked = Config.PlikSettings.OneShot;
            nudPlikTTL.Value = Config.PlikSettings.TTL;
            cbxPlikTTLUnit.SelectedIndex = Config.PlikSettings.TTLUnit;
            txtPlikComment.ReadOnly = !cbPlikComment.Checked;
            txtPlikLogin.ReadOnly = !cbPlikIsSecured.Checked;
            txtPlikPassword.ReadOnly = !cbPlikIsSecured.Checked;

            // Gfycat

            atcGfycatAccountType.SelectedAccountType = Config.GfycatAccountType;

            oauth2Gfycat.Enabled = Config.GfycatAccountType == AccountType.User;

            if (OAuth2Info.CheckOAuth(Config.GfycatOAuth2Info))
            {
                oauth2Gfycat.Status = OAuthLoginStatus.LoginSuccessful;
            }

            cbGfycatIsPublic.Checked = Config.GfycatIsPublic;

            #endregion File uploaders

            #region URL shorteners

            // Google URL Shortener

            atcGoogleURLShortenerAccountType.SelectedAccountType = Config.GoogleURLShortenerAccountType;

            if (OAuth2Info.CheckOAuth(Config.GoogleURLShortenerOAuth2Info))
            {
                oauth2GoogleURLShortener.Status = OAuthLoginStatus.LoginSuccessful;
            }

            // bit.ly

            if (OAuth2Info.CheckOAuth(Config.BitlyOAuth2Info))
            {
                oauth2Bitly.Status = OAuthLoginStatus.LoginSuccessful;
            }

            txtBitlyDomain.Text = Config.BitlyDomain;

            // yourls.org

            txtYourlsAPIURL.Text = Config.YourlsAPIURL;
            txtYourlsSignature.Text = Config.YourlsSignature;
            txtYourlsUsername.Enabled = txtYourlsPassword.Enabled = string.IsNullOrEmpty(Config.YourlsSignature);
            txtYourlsUsername.Text = Config.YourlsUsername;
            txtYourlsPassword.Text = Config.YourlsPassword;

            // adf.ly

            txtAdflyAPIKEY.Text = Config.AdFlyAPIKEY;
            txtAdflyAPIUID.Text = Config.AdFlyAPIUID;

            // coinurl.com

            txtCoinURLUUID.Text = Config.CoinURLUUID;

            // Polr

            txtPolrAPIHostname.Text = Config.PolrAPIHostname;
            txtPolrAPIKey.Text = Config.PolrAPIKey;
            cbPolrIsSecret.Checked = Config.PolrIsSecret;
            cbPolrUseAPIv1.Checked = Config.PolrUseAPIv1;

            #endregion URL shorteners

            #region Other uploaders

            // Twitter

            lbTwitterAccounts.Items.Clear();

            foreach (OAuthInfo twitterOAuth in Config.TwitterOAuthInfoList)
            {
                lbTwitterAccounts.Items.Add(twitterOAuth.Description);
            }

            if (CheckTwitterAccounts())
            {
                lbTwitterAccounts.SelectedIndex = Config.TwitterSelectedAccount;
            }

            TwitterUpdateSelected();

            cbTwitterSkipMessageBox.Checked = Config.TwitterSkipMessageBox;
            txtTwitterDefaultMessage.Text = Config.TwitterDefaultMessage;

            // Custom uploaders

            LoadCustomUploaderTab();

            #endregion Other uploaders
        }

        #region Image Uploaders

        #region Imgur

        private void atcImgurAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.ImgurAccountType = accountType;
            oauth2Imgur.Enabled = Config.ImgurAccountType == AccountType.User;
        }

        private void oauth2Imgur_OpenButtonClicked()
        {
            ImgurAuthOpen();
        }

        private void oauth2Imgur_CompleteButtonClicked(string code)
        {
            ImgurAuthComplete(code);
        }

        private void oauth2Imgur_ClearButtonClicked()
        {
            Config.ImgurOAuth2Info = null;
        }

        private void oauth2Imgur_RefreshButtonClicked()
        {
            ImgurAuthRefresh();
        }

        private void cbImgurDirectLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.ImgurDirectLink = cbImgurDirectLink.Checked;
        }

        private void cbImgurThumbnailType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.ImgurThumbnailType = (ImgurThumbnailType)cbImgurThumbnailType.SelectedIndex;
        }

        private void cbImgurUseHTTPS_CheckedChanged(object sender, EventArgs e)
        {
            Config.ImgurUseHTTPS = cbImgurUseHTTPS.Checked;
        }

        private void cbImgurUseGIFV_CheckedChanged(object sender, EventArgs e)
        {
            Config.ImgurUseGIFV = cbImgurUseGIFV.Checked;
        }

        private void cbImgurUploadSelectedAlbum_CheckedChanged(object sender, EventArgs e)
        {
            Config.ImgurUploadSelectedAlbum = cbImgurUploadSelectedAlbum.Checked;
        }

        private void btnImgurRefreshAlbumList_Click(object sender, EventArgs e)
        {
            ImgurRefreshAlbumList();
        }

        private void lvImgurAlbumList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvImgurAlbumList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvImgurAlbumList.SelectedItems[0];
                if (lvi.Tag is ImgurAlbumData)
                {
                    Config.ImgurSelectedAlbum = (ImgurAlbumData)lvi.Tag;
                }
            }
            else
            {
                Config.ImgurSelectedAlbum = null;
            }
        }

        #endregion Imgur

        #region ImageShack

        private void txtImageShackUsername_TextChanged(object sender, EventArgs e)
        {
            Config.ImageShackSettings.Username = txtImageShackUsername.Text;
        }

        private void txtImageShackPassword_TextChanged(object sender, EventArgs e)
        {
            Config.ImageShackSettings.Password = txtImageShackPassword.Text;
        }

        private void btnImageShackLogin_Click(object sender, EventArgs e)
        {
            ImageShackUploader imageShackUploader = new ImageShackUploader(APIKeys.ImageShackKey, Config.ImageShackSettings);

            try
            {
                if (imageShackUploader.GetAccessToken())
                {
                    MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
                ex.ShowError();
            }
        }

        private void cbImageShackIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.ImageShackSettings.IsPublic = cbImageShackIsPublic.Checked;
        }

        private void btnImageShackOpenPublicProfile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Config.ImageShackSettings.Username))
            {
                URLHelpers.OpenURL("https://imageshack.com/user/" + Config.ImageShackSettings.Username);
            }
            else
            {
                txtImageShackUsername.Focus();
            }
        }

        private void btnImageShackOpenMyImages_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://imageshack.com/my/images");
        }

        #endregion ImageShack

        #region TinyPic

        private void atcTinyPicAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.TinyPicAccountType = accountType;
        }

        private void txtTinyPicUsername_TextChanged(object sender, EventArgs e)
        {
            if (Config.TinyPicRememberUserPass)
            {
                Config.TinyPicUsername = txtTinyPicUsername.Text;
            }
        }

        private void txtTinyPicPassword_TextChanged(object sender, EventArgs e)
        {
            if (Config.TinyPicRememberUserPass)
            {
                Config.TinyPicPassword = txtTinyPicPassword.Text;
            }
        }

        private void btnTinyPicLogin_Click(object sender, EventArgs e)
        {
            string username = txtTinyPicUsername.Text;
            string password = txtTinyPicPassword.Text;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    TinyPicUploader tpu = new TinyPicUploader(APIKeys.TinyPicID, APIKeys.TinyPicKey);
                    string registrationCode = tpu.UserAuth(username, password);

                    if (!string.IsNullOrEmpty(registrationCode))
                    {
                        Config.TinyPicRegistrationCode = registrationCode;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
            }
        }

        private void btnTinyPicOpenMyImages_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("http://tinypic.com/yourstuff.php");
        }

        #endregion TinyPic

        #region Flickr

        private void btnFlickrOpenAuthorize_Click(object sender, EventArgs e)
        {
            FlickrAuthOpen();
        }

        private void btnFlickrCompleteAuth_Click(object sender, EventArgs e)
        {
            FlickrAuthComplete();
        }

        private void btnFlickrCheckToken_Click(object sender, EventArgs e)
        {
            FlickrCheckToken();
        }

        private void btnFlickrOpenImages_Click(object sender, EventArgs e)
        {
            FlickrOpenImages();
        }

        #endregion Flickr

        #region Photobucket

        private void btnPhotobucketAuthOpen_Click(object sender, EventArgs e)
        {
            PhotobucketAuthOpen();
        }

        private void btnPhotobucketAuthComplete_Click(object sender, EventArgs e)
        {
            PhotobucketAuthComplete();
        }

        private void btnPhotobucketCreateAlbum_Click(object sender, EventArgs e)
        {
            PhotobucketCreateAlbum();
        }

        private void cboPhotobucketAlbumPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Config.PhotobucketAccountInfo != null)
            {
                Config.PhotobucketAccountInfo.ActiveAlbumID = cboPhotobucketAlbumPaths.SelectedIndex;
            }
        }

        private void btnPhotobucketAddAlbum_Click(object sender, EventArgs e)
        {
            string albumPath = cboPhotobucketAlbumPaths.Text;
            if (!Config.PhotobucketAccountInfo.AlbumList.Contains(albumPath))
            {
                Config.PhotobucketAccountInfo.AlbumList.Add(albumPath);
                cboPhotobucketAlbumPaths.Items.Add(albumPath);
            }
        }

        private void btnPhotobucketRemoveAlbum_Click(object sender, EventArgs e)
        {
            if (cboPhotobucketAlbumPaths.Items.Count > 1)
            {
                cboPhotobucketAlbumPaths.Items.RemoveAt(cboPhotobucketAlbumPaths.SelectedIndex);
                cboPhotobucketAlbumPaths.SelectedIndex = cboPhotobucketAlbumPaths.Items.Count - 1;
            }
        }

        #endregion Photobucket

        #region Picasa

        private void oauth2Picasa_OpenButtonClicked()
        {
            PicasaAuthOpen();
        }

        private void oauth2Picasa_CompleteButtonClicked(string code)
        {
            PicasaAuthComplete(code);
        }

        private void oauth2Picasa_ClearButtonClicked()
        {
            Config.PicasaOAuth2Info = null;
        }

        private void oauth2Picasa_RefreshButtonClicked()
        {
            PicasaAuthRefresh();
        }

        private void txtPicasaAlbumID_TextChanged(object sender, EventArgs e)
        {
            Config.PicasaAlbumID = txtPicasaAlbumID.Text;
        }

        private void btnPicasaRefreshAlbumList_Click(object sender, EventArgs e)
        {
            PicasaRefreshAlbumList();
        }

        private void lvPicasaAlbumList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPicasaAlbumList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvPicasaAlbumList.SelectedItems[0];
                if (lvi.Tag is PicasaAlbumInfo)
                {
                    PicasaAlbumInfo album = (PicasaAlbumInfo)lvi.Tag;
                    txtPicasaAlbumID.Text = album.ID;
                }
            }
        }

        #endregion Picasa

        #region Chevereto

        private void cbCheveretoUploaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCheveretoUploaders.SelectedIndex > -1)
            {
                CheveretoUploader uploader = cbCheveretoUploaders.SelectedItem as CheveretoUploader;

                if (uploader != null)
                {
                    txtCheveretoUploadURL.Text = uploader.UploadURL;
                    txtCheveretoAPIKey.Text = uploader.APIKey;
                }
            }
        }

        private void btnCheveretoTestAll_Click(object sender, EventArgs e)
        {
            btnCheveretoTestAll.Enabled = false;
            btnCheveretoTestAll.Text = "Testing...";
            string result = null;

            TaskEx.Run(() =>
            {
                result = Chevereto.TestUploaders();
            },
            () =>
            {
                if (!IsDisposed)
                {
                    btnCheveretoTestAll.Text = "Test all";
                    btnCheveretoTestAll.Enabled = true;

                    if (!string.IsNullOrEmpty(result))
                    {
                        MessageBox.Show(result, "Chevereto test results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            });
        }

        private void txtCheveretoWebsite_TextChanged(object sender, EventArgs e)
        {
            Config.CheveretoUploader.UploadURL = txtCheveretoUploadURL.Text;
        }

        private void txtCheveretoAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.CheveretoUploader.APIKey = txtCheveretoAPIKey.Text;
        }

        private void cbCheveretoDirectURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.CheveretoDirectURL = cbCheveretoDirectURL.Checked;
        }

        #endregion Chevereto

        #region SomeImage

        private void txtSomeImageAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.SomeImageAPIKey = txtSomeImageAPIKey.Text;
        }

        private void cbSomeImageDirectURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.SomeImageDirectURL = cbSomeImageDirectURL.Checked;
        }

        private void linkLblSomeImageAPIKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("https://someimage.com/api");
        }

        #endregion SomeImage

        #region vgy.me

        private void txtVgymeUserKey_TextChanged(object sender, EventArgs e)
        {
            Config.VgymeUserKey = txtVgymeUserKey.Text;
        }

        private void llVgymeAccountDetailsPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("http://vgy.me/account/details");
        }

        #endregion vgy.me

        #endregion Image Uploaders

        #region Text Uploaders

        #region Pastebin

        private void txtPastebinUsername_TextChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.Username = txtPastebinUsername.Text;
        }

        private void txtPastebinPassword_TextChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.Password = txtPastebinPassword.Text;
        }

        private void btnPastebinRegister_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("http://pastebin.com/signup");
        }

        private void btnPastebinLogin_Click(object sender, EventArgs e)
        {
            PastebinLogin();
        }

        private void cbPastebinPrivacy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.Exposure = (PastebinPrivacy)cbPastebinPrivacy.SelectedIndex;
        }

        private void cbPastebinExpiration_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.Expiration = (PastebinExpiration)cbPastebinExpiration.SelectedIndex;
        }

        private void cbPastebinSyntax_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.TextFormat = ((PastebinSyntaxInfo)cbPastebinSyntax.SelectedItem).Value;
        }

        private void txtPastebinTitle_TextChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.Title = txtPastebinTitle.Text;
        }

        private void cbPastebinRaw_CheckedChanged(object sender, EventArgs e)
        {
            Config.PastebinSettings.RawURL = cbPastebinRaw.Checked;
        }

        #endregion Pastebin

        #region Paste.ee

        private void txtPaste_eeUserAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.Paste_eeUserAPIKey = txtPaste_eeUserAPIKey.Text;
        }

        #endregion Paste.ee

        #region Gist

        private void atcGistAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.GistAnonymousLogin = accountType == AccountType.Anonymous;
            oAuth2Gist.Enabled = !Config.GistAnonymousLogin;
        }

        private void oAuth2Gist_OpenButtonClicked()
        {
            GistAuthOpen();
        }

        private void oAuth2Gist_CompleteButtonClicked(string code)
        {
            GistAuthComplete(code);
        }

        private void oAuth2Gist_ClearButtonClicked()
        {
            Config.GistOAuth2Info = null;
        }

        private void chkGistPublishPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.GistPublishPublic = cbGistPublishPublic.Checked;
        }

        private void cbGistUseRawURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.GistRawURL = cbGistUseRawURL.Checked;
        }

        private void txtGistCustomURL_TextChanged(object sender, EventArgs e)
        {
            Config.GistCustomURL = txtGistCustomURL.Text;
        }

        #endregion Gist

        #region uPaste

        private void txtUpasteUserKey_TextChanged(object sender, EventArgs e)
        {
            Config.UpasteUserKey = txtUpasteUserKey.Text;
        }

        private void cbUpasteIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.UpasteIsPublic = cbUpasteIsPublic.Checked;
        }

        #endregion uPaste

        #region Hastebin

        private void txtHastebinCustomDomain_TextChanged(object sender, EventArgs e)
        {
            Config.HastebinCustomDomain = txtHastebinCustomDomain.Text;
        }

        private void txtHastebinSyntaxHighlighting_TextChanged(object sender, EventArgs e)
        {
            Config.HastebinSyntaxHighlighting = txtHastebinSyntaxHighlighting.Text;
        }

        private void cbHastebinUseFileExtension_CheckedChanged(object sender, EventArgs e)
        {
            Config.HastebinUseFileExtension = cbHastebinUseFileExtension.Checked;
        }

        #endregion Hastebin

        #region OneTimeSecret

        private void txtOneTimeSecretEmail_TextChanged(object sender, EventArgs e)
        {
            Config.OneTimeSecretAPIUsername = txtOneTimeSecretEmail.Text;
        }

        private void txtOneTimeSecretAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.OneTimeSecretAPIKey = txtOneTimeSecretAPIKey.Text;
        }

        #endregion OneTimeSecret

        #region Pastie

        private void cbPastieIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.PastieIsPublic = cbPastieIsPublic.Checked;
        }

        #endregion Pastie

        #endregion Text Uploaders

        #region File Uploaders

        #region FTP

        private void cbFTPImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.FTPSelectedImage = cbFTPImage.SelectedIndex;
        }

        private void cbFTPText_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.FTPSelectedText = cbFTPText.SelectedIndex;
        }

        private void cbFTPFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.FTPSelectedFile = cbFTPFile.SelectedIndex;
        }

        private void cbFTPAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            FTPLoadSelectedAccount();
        }

        private void btnFTPAdd_Click(object sender, EventArgs e)
        {
            FTPAddAccount(new FTPAccount());

            cbFTPAccounts.SelectedIndex = cbFTPAccounts.Items.Count - 1;

            txtFTPName.Focus();
        }

        private void btnFTPRemove_Click(object sender, EventArgs e)
        {
            int selected = cbFTPAccounts.SelectedIndex;

            if (selected > -1)
            {
                cbFTPAccounts.Items.RemoveAt(selected);
                Config.FTPAccountList.RemoveAt(selected);

                if (cbFTPAccounts.Items.Count > 0)
                {
                    cbFTPAccounts.SelectedIndex = selected == cbFTPAccounts.Items.Count ? cbFTPAccounts.Items.Count - 1 : selected;
                }
                else
                {
                    FTPClearFields();

                    btnFTPAdd.Focus();
                }

                FTPUpdateControls();
            }
        }

        private void btnFTPDuplicate_Click(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();

            if (account != null)
            {
                FTPAccount clone = account.Clone();
                FTPAddAccount(clone);

                cbFTPAccounts.SelectedIndex = cbFTPAccounts.Items.Count - 1;
            }
        }

        private void txtFTPName_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Name = txtFTPName.Text;
                FTPRefreshNames();
            }
        }

        private void rbFTPProtocolFTP_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Protocol = FTPProtocol.FTP;
                FTPUpdateEnabledStates();
            }
        }

        private void rbFTPProtocolFTPS_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Protocol = FTPProtocol.FTPS;
                FTPUpdateEnabledStates();
            }
        }

        private void rbFTPProtocolSFTP_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Protocol = FTPProtocol.SFTP;
                FTPUpdateEnabledStates();
            }
        }

        private void txtFTPHost_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Host = txtFTPHost.Text;
                FTPUpdateURLPreview();
                FTPRefreshNames();
            }
        }

        private void nudFTPPort_ValueChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Port = (int)nudFTPPort.Value;
                FTPUpdateURLPreview();
            }
        }

        private void txtFTPUsername_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Username = txtFTPUsername.Text;
            }
        }

        private void txtFTPPassword_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Password = txtFTPPassword.Text;
            }
        }

        private void rbFTPTransferModePassive_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.IsActive = false;
            }
        }

        private void rbFTPTransferModeActive_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.IsActive = true;
            }
        }

        private void txtFTPRemoteDirectory_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.SubFolderPath = txtFTPRemoteDirectory.Text;
                FTPUpdateURLPreview();
            }
        }

        private void cbFTPURLPathProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.BrowserProtocol = (BrowserProtocol)cbFTPURLPathProtocol.SelectedIndex;
                FTPUpdateURLPreview();
            }
        }

        private void txtFTPURLPath_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.HttpHomePath = txtFTPURLPath.Text;
                FTPUpdateURLPreview();
            }
        }

        private void cbFTPAppendRemoteDirectory_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.HttpHomePathAutoAddSubFolderPath = cbFTPAppendRemoteDirectory.Checked;
                FTPUpdateURLPreview();
            }
        }

        private void cbFTPRemoveFileExtension_CheckedChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.HttpHomePathNoExtension = cbFTPRemoveFileExtension.Checked;
                FTPUpdateURLPreview();
            }
        }

        private void cbFTPSEncryption_SelectedIndexChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.FTPSEncryption = (FTPSEncryption)cbFTPSEncryption.SelectedIndex;
            }
        }

        private void txtFTPSCertificateLocation_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.FTPSCertificateLocation = txtFTPSCertificateLocation.Text;
            }
        }

        private void btnFTPSCertificateLocationBrowse_Click(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = Resources.CertFileNameEditor_EditValue_Browse_for_a_certificate_file___;
                    dlg.Filter = "Certificate file (*.cer)|*.cer";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtFTPSCertificateLocation.Text = dlg.FileName;
                    }
                }
            }
        }

        private void txtSFTPKeyLocation_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Keypath = txtSFTPKeyLocation.Text;
            }
        }

        private void btnSFTPKeyLocationBrowse_Click(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = Resources.KeyFileNameEditor_EditValue_Browse_for_a_key_file___;
                    dlg.Filter = "Key file (*.*)|*.*";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtSFTPKeyLocation.Text = dlg.FileName;
                    }
                }
            }
        }

        private void txtSFTPKeyPassphrase_TextChanged(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();
            if (account != null)
            {
                account.Passphrase = txtSFTPKeyPassphrase.Text;
            }
        }

        private void btnFTPTest_Click(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();

            if (account != null)
            {
                FTPTestAccountAsync(account);
            }
            else
            {
                MessageBox.Show(Resources.UploadersConfigForm_FTPOpenClient_Unable_to_find_valid_FTP_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnFTPClient_Click(object sender, EventArgs e)
        {
            FTPAccount account = FTPGetSelectedAccount();

            if (account != null)
            {
                FTPOpenClient(account);
            }
            else
            {
                MessageBox.Show(Resources.UploadersConfigForm_FTPOpenClient_Unable_to_find_valid_FTP_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private object eiFTP_ExportRequested()
        {
            return FTPGetSelectedAccount();
        }

        private void eiFTP_ImportRequested(object obj)
        {
            FTPAddAccount(obj as FTPAccount);
        }

        #endregion FTP

        #region Dropbox

        private void pbDropboxLogo_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://www.dropbox.com");
        }

        private void oauth2Dropbox_OpenButtonClicked()
        {
            DropboxAuthOpen();
        }

        private void oauth2Dropbox_CompleteButtonClicked(string code)
        {
            DropboxAuthComplete(code);
        }

        private void oauth2Dropbox_ClearButtonClicked()
        {
            Config.DropboxOAuth2Info = null;
        }

        private void txtDropboxPath_TextChanged(object sender, EventArgs e)
        {
            Config.DropboxUploadPath = txtDropboxPath.Text;
        }

        private void cbDropboxAutoCreateShareableLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.DropboxAutoCreateShareableLink = cbDropboxAutoCreateShareableLink.Checked;
            cbDropboxUseDirectLink.Enabled = Config.DropboxAutoCreateShareableLink;
        }

        private void cbDropboxUseDirectLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.DropboxUseDirectLink = cbDropboxUseDirectLink.Checked;
        }

        #endregion Dropbox

        #region OneDrive

        private void oAuth2OneDrive_OpenButtonClicked()
        {
            OneDriveAuthOpen();
        }

        private void oAuth2OneDrive_CompleteButtonClicked(string code)
        {
            OneDriveAuthComplete(code);
        }

        private void oAuth2OneDrive_RefreshButtonClicked()
        {
            OneDriveAuthRefresh();
        }

        private void oAuth2OneDrive_ClearButtonClicked()
        {
            Config.OneDriveOAuth2Info = null;
        }

        private void cbOneDriveCreateShareableLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.OneDriveAutoCreateShareableLink = cbOneDriveCreateShareableLink.Checked;
        }

        private void tvOneDrive_AfterSelect(object sender, TreeViewEventArgs e)
        {
            OneDriveFileInfo file = e.Node.Tag as OneDriveFileInfo;
            if (file != null)
            {
                lblOneDriveFolderID.Text = Resources.UploadersConfigForm_LoadSettings_Selected_folder_ + " " + file.name;
                Config.OneDriveSelectedFolder = file;
            }
        }

        private void tvOneDrive_AfterExpand(object sender, TreeViewEventArgs e)
        {
            OneDriveFileInfo file = e.Node.Tag as OneDriveFileInfo;
            if (file != null)
            {
                OneDriveListFolders(file, e.Node);
            }
        }

        #endregion OneDrive

        #region Google Drive

        private void oauth2GoogleDrive_OpenButtonClicked()
        {
            GoogleDriveAuthOpen();
        }

        private void oauth2GoogleDrive_CompleteButtonClicked(string code)
        {
            GoogleDriveAuthComplete(code);
        }

        private void oauth2GoogleDrive_RefreshButtonClicked()
        {
            GoogleDriveAuthRefresh();
        }

        private void oauth2GoogleDrive_ClearButtonClicked()
        {
            Config.GoogleDriveOAuth2Info = null;
        }

        private void cbGoogleDriveIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.GoogleDriveIsPublic = cbGoogleDriveIsPublic.Checked;
        }

        private void cbGoogleDriveDirectLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.GoogleDriveDirectLink = cbGoogleDriveDirectLink.Checked;
        }

        private void cbGoogleDriveUseFolder_CheckedChanged(object sender, EventArgs e)
        {
            Config.GoogleDriveUseFolder = cbGoogleDriveUseFolder.Checked;
            txtGoogleDriveFolderID.Enabled = Config.GoogleDriveUseFolder;
        }

        private void txtGoogleDriveFolderID_TextChanged(object sender, EventArgs e)
        {
            Config.GoogleDriveFolderID = txtGoogleDriveFolderID.Text;
        }

        private void btnGoogleDriveRefreshFolders_Click(object sender, EventArgs e)
        {
            GoogleDriveRefreshFolders();
        }

        private void lvGoogleDriveFoldersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvGoogleDriveFoldersList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvGoogleDriveFoldersList.SelectedItems[0];
                GoogleDriveFile folder = lvi.Tag as GoogleDriveFile;
                if (folder != null)
                {
                    txtGoogleDriveFolderID.Text = folder.id;
                }
            }
        }

        #endregion Google Drive

        #region puush

        private bool PuushValidationCheck()
        {
            bool result = true;

            if (string.IsNullOrEmpty(txtPuushEmail.Text))
            {
                txtPuushEmail.BackColor = Color.FromArgb(255, 200, 200);
                result = false;
            }
            else
            {
                txtPuushEmail.BackColor = SystemColors.Window;
            }

            if (string.IsNullOrEmpty(txtPuushPassword.Text))
            {
                txtPuushPassword.BackColor = Color.FromArgb(255, 200, 200);
                result = false;
            }
            else
            {
                txtPuushPassword.BackColor = SystemColors.Window;
            }

            return result;
        }

        private void pbPuush_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Puush.PuushURL);
        }

        private void llPuushForgottenPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(Puush.PuushResetPasswordURL);
        }

        private void btnPuushLogin_Click(object sender, EventArgs e)
        {
            if (PuushValidationCheck())
            {
                txtPuushAPIKey.Text = "";

                string apiKey = new Puush().Login(txtPuushEmail.Text, txtPuushPassword.Text);

                if (!string.IsNullOrEmpty(apiKey))
                {
                    txtPuushAPIKey.Text = apiKey;
                }
                else
                {
                    MessageBox.Show("Login failed.", "Authentication failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtPuushAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.PuushAPIKey = txtPuushAPIKey.Text;
        }

        #endregion puush

        #region Box

        private void oauth2Box_OpenButtonClicked()
        {
            BoxAuthOpen();
        }

        private void oauth2Box_CompleteButtonClicked(string code)
        {
            BoxAuthComplete(code);
        }

        private void oauth2Box_RefreshButtonClicked()
        {
            BoxAuthRefresh();
        }

        private void oauth2Box_ClearButtonClicked()
        {
            Config.BoxOAuth2Info = null;
        }

        private void cbBoxShare_CheckedChanged(object sender, EventArgs e)
        {
            Config.BoxShare = cbBoxShare.Checked;
        }

        private void btnBoxRefreshFolders_Click(object sender, EventArgs e)
        {
            BoxListFolders();
        }

        private void lvBoxFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvBoxFolders.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvBoxFolders.SelectedItems[0];
                BoxFileEntry file = lvi.Tag as BoxFileEntry;
                if (file != null)
                {
                    lblBoxFolderID.Text = Resources.UploadersConfigForm_LoadSettings_Selected_folder_ + " " + file.name;
                    Config.BoxSelectedFolder = file;
                }
            }
        }

        private void lvBoxFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lvBoxFolders.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvBoxFolders.SelectedItems[0];
                BoxFileEntry file = lvi.Tag as BoxFileEntry;
                if (file != null)
                {
                    lvBoxFolders.Items.Clear();
                    BoxListFolders(file);
                }
            }
        }

        #endregion Box

        #region Minus

        private void btnMinusAuth_Click(object sender, EventArgs e)
        {
            MinusAuth();
        }

        private void btnAuthRefresh_Click(object sender, EventArgs e)
        {
            MinusAuthRefresh();
        }

        private void cboMinusFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Config.MinusConfig != null)
            {
                Config.MinusConfig.FolderID = cboMinusFolders.SelectedIndex;
                MinusFolder tempMf = Config.MinusConfig.GetActiveFolder();
                cbMinusPublic.Checked = tempMf.is_public;
            }
        }

        private void btnMinusFolderAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboMinusFolders.Text) && !MinusHasFolder(cboMinusFolders.Text))
            {
                btnMinusFolderAdd.Enabled = false;

                Minus minus = new Minus(Config.MinusConfig, Config.MinusOAuth2Info);
                MinusFolder dir = minus.CreateFolder(cboMinusFolders.Text, cbMinusPublic.Checked);
                if (dir != null)
                {
                    cboMinusFolders.Items.Add(dir);
                    cboMinusFolders.SelectedIndex = cboMinusFolders.Items.Count - 1;
                }

                btnMinusFolderAdd.Enabled = true;
            }
        }

        private void btnMinusFolderRemove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboMinusFolders.Text) && MinusHasFolder(cboMinusFolders.Text))
            {
                btnMinusFolderRemove.Enabled = false;

                Minus minus = new Minus(Config.MinusConfig, Config.MinusOAuth2Info);

                int index = cboMinusFolders.SelectedIndex;

                if (minus.DeleteFolder(index))
                {
                    cboMinusFolders.Items.RemoveAt(index);

                    if (cboMinusFolders.Items.Count > 0)
                    {
                        cboMinusFolders.SelectedIndex = 0;
                    }
                }

                btnMinusFolderRemove.Enabled = true;
            }
        }

        private void btnMinusReadFolderList_Click(object sender, EventArgs e)
        {
            if (Config.MinusConfig != null)
            {
                btnMinusReadFolderList.Enabled = false;

                List<MinusFolder> tempListMf = new Minus(Config.MinusConfig, Config.MinusOAuth2Info).ReadFolderList();

                if (tempListMf.Count > 0)
                {
                    cboMinusFolders.Items.Clear();
                    cboMinusFolders.Items.AddRange(tempListMf.ToArray());
                    cboMinusFolders.SelectedIndex = Config.MinusConfig.FolderID;
                }

                btnMinusReadFolderList.Enabled = true;
            }
        }

        private void cbMinusURLType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Config.MinusConfig != null)
            {
                Config.MinusConfig.LinkType = (MinusLinkType)cbMinusURLType.SelectedIndex;
            }
        }

        #endregion Minus

        #region Email

        private void txtSmtpServer_TextChanged(object sender, EventArgs e)
        {
            Config.EmailSmtpServer = txtEmailSmtpServer.Text;
        }

        private void nudSmtpPort_ValueChanged(object sender, EventArgs e)
        {
            Config.EmailSmtpPort = (int)nudEmailSmtpPort.Value;
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            Config.EmailFrom = txtEmailFrom.Text;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            Config.EmailPassword = txtEmailPassword.Text;
        }

        private void cbRememberLastToEmail_CheckedChanged(object sender, EventArgs e)
        {
            Config.EmailRememberLastTo = cbEmailRememberLastTo.Checked;
        }

        private void txtDefaultSubject_TextChanged(object sender, EventArgs e)
        {
            Config.EmailDefaultSubject = txtEmailDefaultSubject.Text;
        }

        private void txtDefaultBody_TextChanged(object sender, EventArgs e)
        {
            Config.EmailDefaultBody = txtEmailDefaultBody.Text;
        }

        private void cbEmailAutomaticSend_CheckedChanged(object sender, EventArgs e)
        {
            Config.EmailAutomaticSend = cbEmailAutomaticSend.Checked;
            txtEmailAutomaticSendTo.Enabled = Config.EmailAutomaticSend;
        }

        private void txtEmailAutomaticSendTo_TextChanged(object sender, EventArgs e)
        {
            Config.EmailAutomaticSendTo = txtEmailAutomaticSendTo.Text;
        }

        #endregion Email

        #region SendSpace

        private void atcSendSpaceAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.SendSpaceAccountType = accountType;
        }

        private void btnSendSpaceRegister_Click(object sender, EventArgs e)
        {
            using (UserPassBox upb = SendSpaceRegister())
            {
                if (upb.Success)
                {
                    txtSendSpaceUserName.Text = upb.UserName;
                    txtSendSpacePassword.Text = upb.Password;
                    atcSendSpaceAccountType.SelectedAccountType = AccountType.User;
                }
            }
        }

        private void txtSendSpaceUserName_TextChanged(object sender, EventArgs e)
        {
            Config.SendSpaceUsername = txtSendSpaceUserName.Text;
        }

        private void txtSendSpacePassword_TextChanged(object sender, EventArgs e)
        {
            Config.SendSpacePassword = txtSendSpacePassword.Text;
        }

        #endregion SendSpace

        #region Ge.tt

        private void btnGe_ttLogin_Click(object sender, EventArgs e)
        {
            Ge_ttLogin();
        }

        #endregion Ge.tt

        #region Localhostr

        private void txtLocalhostrEmail_TextChanged(object sender, EventArgs e)
        {
            Config.LocalhostrEmail = txtLocalhostrEmail.Text;
        }

        private void txtLocalhostrPassword_TextChanged(object sender, EventArgs e)
        {
            Config.LocalhostrPassword = txtLocalhostrPassword.Text;
        }

        private void cbLocalhostrDirectURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.LocalhostrDirectURL = cbLocalhostrDirectURL.Checked;
        }

        #endregion Localhostr

        #region Jira

        private void txtJiraHost_TextChanged(object sender, EventArgs e)
        {
            Config.JiraHost = txtJiraHost.Text;
        }

        private void txtJiraIssuePrefix_TextChanged(object sender, EventArgs e)
        {
            Config.JiraIssuePrefix = txtJiraIssuePrefix.Text;
        }

        private void oAuthJira_OpenButtonClicked()
        {
            JiraAuthOpen();
        }

        private void oAuthJira_CompleteButtonClicked(string code)
        {
            JiraAuthComplete(code);
        }

        private void oAuthJira_ClearButtonClicked()
        {
            Config.JiraOAuthInfo = null;
        }

        private void oAuthJira_RefreshButtonClicked()
        {
            MessageBox.Show(Resources.UploadersConfigForm_oAuthJira_RefreshButtonClicked_Refresh_authorization_is_not_supported_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion Jira

        #region Mega

        private void MegaConfigureTab(bool tryLogin)
        {
            Color OkColor = Color.Green;
            Color NokColor = Color.DarkRed;

            tpMega.Enabled = false;

            if (Config.MegaAuthInfos != null)
            {
                txtMegaEmail.Text = Config.MegaAuthInfos.Email;
            }

            if (Config.MegaAuthInfos == null)
            {
                lblMegaStatus.Text = Resources.UploadersConfigForm_MegaConfigureTab_Not_configured;
                lblMegaStatus.ForeColor = NokColor;
            }
            else
            {
                cbMegaFolder.Items.Clear();

                Mega mega = new Mega(Config.MegaAuthInfos);
                if (!tryLogin || mega.TryLogin())
                {
                    lblMegaStatus.Text = Resources.UploadersConfigForm_MegaConfigureTab_Configured;
                    lblMegaStatus.ForeColor = OkColor;

                    if (tryLogin)
                    {
                        Mega.DisplayNode[] nodes = mega.GetDisplayNodes().ToArray();
                        cbMegaFolder.Items.AddRange(nodes);
                        cbMegaFolder.SelectedItem = nodes.FirstOrDefault(n => n.Node != null && n.Node.Id == Config.MegaParentNodeId) ?? Mega.DisplayNode.EmptyNode;
                    }
                    else
                    {
                        cbMegaFolder.Items.Add("[" + Resources.UploadersConfigForm_MegaConfigureTab_Click_refresh_button + "]");
                        cbMegaFolder.SelectedIndex = 0;
                    }
                }
                else
                {
                    lblMegaStatus.Text = Resources.UploadersConfigForm_MegaConfigureTab_Invalid_authentication;
                    lblMegaStatus.ForeColor = NokColor;
                }
            }

            tpMega.Enabled = true;
        }

        private void btnMegaLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMegaEmail.Text) || string.IsNullOrEmpty(txtMegaPassword.Text))
            {
                return;
            }

            Config.MegaAuthInfos = MegaApiClient.GenerateAuthInfos(txtMegaEmail.Text, txtMegaPassword.Text);

            MegaConfigureTab(true);
        }

        private void cbMegaFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            Mega.DisplayNode selectedNode = ((ComboBox)sender).SelectedItem as Mega.DisplayNode;
            if (selectedNode != null)
            {
                Config.MegaParentNodeId = selectedNode == Mega.DisplayNode.EmptyNode ? null : selectedNode.Node.Id;
            }
        }

        private void btnMegaRegister_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://mega.co.nz/#register");
        }

        private void btnMegaRefreshFolders_Click(object sender, EventArgs e)
        {
            MegaConfigureTab(true);
        }

        #endregion Mega

        #region Amazon S3

        private void txtAmazonS3AccessKey_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.AccessKeyID = txtAmazonS3AccessKey.Text;
        }

        private void btnAmazonS3AccessKeyOpen_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://console.aws.amazon.com/iam/home?#security_credential");
        }

        private void txtAmazonS3SecretKey_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.SecretAccessKey = txtAmazonS3SecretKey.Text;
        }

        private void cbAmazonS3Endpoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmazonS3Endpoint endpoint = cbAmazonS3Endpoints.SelectedItem as AmazonS3Endpoint;

            if (endpoint != null)
            {
                txtAmazonS3Region.Text = endpoint.Region;
                txtAmazonS3Endpoint.Text = endpoint.Endpoint;
            }
        }

        private void txtAmazonS3Endpoint_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.Endpoint = txtAmazonS3Endpoint.Text;
            UpdateAmazonS3Status();
        }

        private void txtAmazonS3Region_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.Region = txtAmazonS3Region.Text;
            UpdateAmazonS3Status();
        }

        private void cbAmazonS3UsePathStyle_CheckedChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.UsePathStyle = cbAmazonS3UsePathStyle.Checked;
        }

        private void txtAmazonS3BucketName_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.Bucket = txtAmazonS3BucketName.Text;
            UpdateAmazonS3Status();
        }

        private void btnAmazonS3BucketNameOpen_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://console.aws.amazon.com/s3/home");
        }

        private void txtAmazonS3ObjectPrefix_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.ObjectPrefix = txtAmazonS3ObjectPrefix.Text;
            UpdateAmazonS3Status();
        }

        private void cbAmazonS3CustomCNAME_CheckedChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.UseCustomCNAME = cbAmazonS3CustomCNAME.Checked;
            txtAmazonS3CustomDomain.Enabled = Config.AmazonS3Settings.UseCustomCNAME;
            UpdateAmazonS3Status();
        }

        private void txtAmazonS3CustomDomain_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.CustomDomain = txtAmazonS3CustomDomain.Text;
            UpdateAmazonS3Status();
        }

        private void cbAmazonS3UseRRS_CheckedChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.UseReducedRedundancyStorage = cbAmazonS3UseRRS.Checked;
            UpdateAmazonS3Status();
        }

        #endregion Amazon S3

        #region ownCloud

        private void txtOwnCloudHost_TextChanged(object sender, EventArgs e)
        {
            Config.OwnCloudHost = txtOwnCloudHost.Text;
        }

        private void txtOwnCloudUsername_TextChanged(object sender, EventArgs e)
        {
            Config.OwnCloudUsername = txtOwnCloudUsername.Text;
        }

        private void txtOwnCloudPassword_TextChanged(object sender, EventArgs e)
        {
            Config.OwnCloudPassword = txtOwnCloudPassword.Text;
        }

        private void txtOwnCloudPath_TextChanged(object sender, EventArgs e)
        {
            Config.OwnCloudPath = txtOwnCloudPath.Text;
        }

        private void cbOwnCloudCreateShare_CheckedChanged(object sender, EventArgs e)
        {
            Config.OwnCloudCreateShare = cbOwnCloudCreateShare.Checked;
        }

        private void cbOwnCloudDirectLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.OwnCloudDirectLink = cbOwnCloudDirectLink.Checked;
        }

        private void cbOwnCloud81Compatibility_CheckedChanged(object sender, EventArgs e)
        {
            Config.OwnCloud81Compatibility = cbOwnCloud81Compatibility.Checked;
        }

        #endregion ownCloud

        #region Pushbullet

        private void txtPushbulletUserKey_TextChanged(object sender, EventArgs e)
        {
            bool enable = !string.IsNullOrEmpty(txtPushbulletUserKey.Text.Trim());

            cboPushbulletDevices.Enabled = enable;
            btnPushbulletGetDeviceList.Enabled = enable;

            Config.PushbulletSettings.UserAPIKey = txtPushbulletUserKey.Text;
        }

        private void cboPushbulletDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.PushbulletSettings.SelectedDevice = cboPushbulletDevices.SelectedIndex;
        }

        private void btnPushbulletGetDeviceList_Click(object sender, EventArgs e)
        {
            PushbulletGetDevices();
        }

        #endregion Pushbullet

        #region Shared folder

        private void LocalhostAccountsSetup(IEnumerable<LocalhostAccount> accs)
        {
            if (accs != null)
            {
                int sel = ucLocalhostAccounts.lbAccounts.SelectedIndex;

                ucLocalhostAccounts.lbAccounts.Items.Clear();
                Config.LocalhostAccountList = new List<LocalhostAccount>();
                Config.LocalhostAccountList.AddRange(accs);

                cboSharedFolderFiles.Items.Clear();
                cboSharedFolderImages.Items.Clear();
                cboSharedFolderText.Items.Clear();

                foreach (LocalhostAccount acc in Config.LocalhostAccountList)
                {
                    ucLocalhostAccounts.lbAccounts.Items.Add(acc);
                    cboSharedFolderFiles.Items.Add(acc);
                    cboSharedFolderImages.Items.Add(acc);
                    cboSharedFolderText.Items.Add(acc);
                }

                if (ucLocalhostAccounts.lbAccounts.Items.Count > 0)
                {
                    ucLocalhostAccounts.lbAccounts.SelectedIndex = sel.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                    cboSharedFolderFiles.SelectedIndex = Config.LocalhostSelectedFiles.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                    cboSharedFolderImages.SelectedIndex = Config.LocalhostSelectedImages.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                    cboSharedFolderText.SelectedIndex = Config.LocalhostSelectedText.Between(0, ucLocalhostAccounts.lbAccounts.Items.Count - 1);
                }
            }
        }

        private void LocalhostAccountAddButton_Click(object sender, EventArgs e)
        {
            LocalhostAccount acc = new LocalhostAccount();
            Config.LocalhostAccountList.Add(acc);
            ucLocalhostAccounts.AddItem(acc);
        }

        private void LocalhostAccountRemoveButton_Click(object sender, EventArgs e)
        {
            int sel = ucLocalhostAccounts.lbAccounts.SelectedIndex;
            if (ucLocalhostAccounts.RemoveItem(sel))
            {
                Config.LocalhostAccountList.RemoveAt(sel);
            }
        }

        private void LocalhostAccountDuplicateButton_Click(object sender, EventArgs e)
        {
            LocalhostAccount src = (LocalhostAccount)ucLocalhostAccounts.lbAccounts.Items[ucLocalhostAccounts.lbAccounts.SelectedIndex];
            LocalhostAccount clone = src.Clone();
            Config.LocalhostAccountList.Add(clone);
            ucLocalhostAccounts.AddItem(clone);
        }

        private void SettingsGrid_LocalhostPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LocalhostAccountsSetup(Config.LocalhostAccountList);
        }

        private void cboSharedFolderImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.LocalhostSelectedImages = cboSharedFolderImages.SelectedIndex;
        }

        private void cboSharedFolderText_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.LocalhostSelectedText = cboSharedFolderText.SelectedIndex;
        }

        private void cboSharedFolderFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.LocalhostSelectedFiles = cboSharedFolderFiles.SelectedIndex;
        }

        #endregion Shared folder

        #region MediaFire

        private void txtMediaFireUsername_TextChanged(object sender, EventArgs e)
        {
            Config.MediaFireUsername = txtMediaFireEmail.Text;
        }

        private void txtMediaFirePassword_TextChanged(object sender, EventArgs e)
        {
            Config.MediaFirePassword = txtMediaFirePassword.Text;
        }

        private void txtMediaFirePath_TextChanged(object sender, EventArgs e)
        {
            Config.MediaFirePath = txtMediaFirePath.Text;
        }

        private void cbMediaFireUseLongLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.MediaFireUseLongLink = cbMediaFireUseLongLink.Checked;
        }

        #endregion MediaFire

        #region Lambda

        private void lambdaInfoLabel_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://lambda.sx/user/manage");
        }

        private void txtLambdaApiKey_TextChanged(object sender, EventArgs e)
        {
            Config.LambdaSettings.UserAPIKey = txtLambdaApiKey.Text;
        }

        private void cbLambdaUploadURL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLambdaUploadURL.SelectedIndex > -1)
            {
                string url = cbLambdaUploadURL.SelectedItem as string;

                if (url != null)
                {
                    Config.LambdaSettings.UploadURL = url;
                }
            }
        }

        #endregion Lambda

        #region Lithiio

        private void btnLithiioGetAPIKey_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://lithi.io/");
        }

        private void txtLithiioApiKey_TextChanged(object sender, EventArgs e)
        {
            Config.LithiioSettings.UserAPIKey = txtLithiioApiKey.Text;
        }

        #endregion Lithiio

        #region Pomf

        private void cbPomfUploaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPomfUploaders.SelectedIndex > -1)
            {
                PomfUploader uploader = cbPomfUploaders.SelectedItem as PomfUploader;

                if (uploader != null)
                {
                    txtPomfUploadURL.Text = uploader.UploadURL;
                    txtPomfResultURL.Text = uploader.ResultURL;
                }
            }
        }

        private void btnPomfTest_Click(object sender, EventArgs e)
        {
            btnPomfTest.Enabled = false;
            btnPomfTest.Text = "Testing...";
            string result = null;

            TaskEx.Run(() =>
            {
                result = Pomf.TestUploaders();
            },
            () =>
            {
                if (!IsDisposed)
                {
                    btnPomfTest.Text = "Test all";
                    btnPomfTest.Enabled = true;

                    if (!string.IsNullOrEmpty(result))
                    {
                        Debug.WriteLine("Pomf test results:\r\n\r\n" + result);
                    }
                }
            });
        }

        private void txtPomfUploadURL_TextChanged(object sender, EventArgs e)
        {
            Config.PomfUploader.UploadURL = txtPomfUploadURL.Text;
        }

        private void txtPomfResultURL_TextChanged(object sender, EventArgs e)
        {
            Config.PomfUploader.ResultURL = txtPomfResultURL.Text;
        }

        #endregion Pomf

        #region Sul

        private void txtSulAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.SulAPIKey = txtSulAPIKey.Text;
        }

        private void sulKeyLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("https://s-ul.eu/account/info");
        }

        #endregion Sul

        #region Seafile

        private void cbSeafileAPIURL_TextChanged(object sender, EventArgs e)
        {
            Config.SeafileAPIURL = cbSeafileAPIURL.Text;
        }

        private void btnSeafileCheckAPIURL_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbSeafileAPIURL.Text))
            {
                return;
            }

            Seafile sf = new Seafile(cbSeafileAPIURL.Text, null, null);
            bool checkReturned = sf.CheckAPIURL();

            if (checkReturned)
            {
                MessageBox.Show(Resources.UploadersConfigForm_TestFTPAccount_Connected_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.UploadersConfigForm_Error, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSeafileAuthToken_TextChanged(object sender, EventArgs e)
        {
            Config.SeafileAuthToken = txtSeafileAuthToken.Text;
        }

        private void btnSeafileCheckAuthToken_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSeafileAuthToken.Text) || string.IsNullOrEmpty(cbSeafileAPIURL.Text))
            {
                return;
            }

            Seafile sf = new Seafile(cbSeafileAPIURL.Text, txtSeafileAuthToken.Text, null);
            bool checkReturned = sf.CheckAuthToken();

            if (checkReturned)
            {
                MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.UploadersConfigForm_Error, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSeafilePassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                btnSeafileGetAuthToken.PerformClick();
            }
        }

        private void btnSeafileGetAuthToken_Click(object sender, EventArgs e)
        {
            string username = txtSeafileUsername.Text;
            string password = txtSeafilePassword.Text;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    Seafile sf = new Seafile(cbSeafileAPIURL.Text, null, null);
                    string authToken = sf.GetAuthToken(username, password);

                    if (!string.IsNullOrEmpty(authToken))
                    {
                        txtSeafileUsername.Text = "";
                        txtSeafilePassword.Text = "";
                        Config.SeafileAuthToken = authToken;
                        txtSeafileAuthToken.Text = authToken;
                        btnRefreshSeafileAccInfo.PerformClick();
                        Config.SeafileRepoID = sf.GetOrMakeDefaultLibrary(authToken);
                        txtSeafileUploadLocationRefresh.PerformClick();
                        MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
            }
        }

        private void cbSeafileCreateShareableURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.SeafileCreateShareableURL = cbSeafileCreateShareableURL.Checked;
        }

        private void cbSeafileIgnoreInvalidCert_CheckedChanged(object sender, EventArgs e)
        {
            Config.SeafileIgnoreInvalidCert = cbSeafileIgnoreInvalidCert.Checked;
        }

        private void btnRefreshSeafileAccInfo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSeafileAuthToken.Text) || string.IsNullOrEmpty(cbSeafileAPIURL.Text))
            {
                return;
            }

            Seafile sf = new Seafile(cbSeafileAPIURL.Text, txtSeafileAuthToken.Text, null);
            SeafileCheckAccInfoResponse SeafileCheckAccInfoResponse = sf.GetAccountInfo();

            if (SeafileCheckAccInfoResponse == null)
            {
                MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txtSeafileAccInfoEmail.Text = SeafileCheckAccInfoResponse.email;
            txtSeafileAccInfoUsage.Text = SeafileCheckAccInfoResponse.usage.ToSizeString() + " / " + SeafileCheckAccInfoResponse.total.ToSizeString();
        }

        private void txtSeafileUploadLocationRefresh_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSeafileAuthToken.Text) || string.IsNullOrEmpty(cbSeafileAPIURL.Text))
            {
                return;
            }
            lvSeafileLibraries.Items.Clear();

            Seafile sf = new Seafile(cbSeafileAPIURL.Text, txtSeafileAuthToken.Text, null);
            List<SeafileLibraryObj> SeafileLibraries = sf.GetLibraries();

            foreach (SeafileLibraryObj SeafileLibrary in SeafileLibraries)
            {
                if (SeafileLibrary.permission == "rw")
                {
                    ListViewItem libraryItem = lvSeafileLibraries.Items.Add(SeafileLibrary.name);
                    libraryItem.Name = SeafileLibrary.id;
                    libraryItem.Tag = SeafileLibrary;
                    libraryItem.SubItems.Add(SeafileLibrary.size.ToSizeString());
                    if (SeafileLibrary.encrypted)
                    {
                        libraryItem.SubItems.Add("\u221A");
                    }
                    if (SeafileLibrary.id == Config.SeafileRepoID)
                    {
                        libraryItem.Selected = true;
                    }
                }
            }
        }

        private void lvSeafileLibraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selIndex = lvSeafileLibraries.SelectedIndex;
            if (selIndex > -1)
            {
                ListViewItem selectedItem = lvSeafileLibraries.Items[selIndex];
                Config.SeafileRepoID = selectedItem.Name;
                SeafileLibraryObj SealileLibraryInfo = (SeafileLibraryObj)selectedItem.Tag;
                if (SealileLibraryInfo.encrypted)
                {
                    Config.SeafileIsLibraryEncrypted = true;
                    txtSeafileLibraryPassword.ReadOnly = false;
                    btnSeafileLibraryPasswordValidate.Enabled = true;
                }
                else
                {
                    Config.SeafileIsLibraryEncrypted = false;
                    txtSeafileLibraryPassword.ReadOnly = true;
                    txtSeafileLibraryPassword.Text = "";
                    Config.SeafileEncryptedLibraryPassword = "";
                    btnSeafileLibraryPasswordValidate.Enabled = false;
                }
            }
        }

        private void txtSeafileDirectoryPath_TextChanged(object sender, EventArgs e)
        {
            Config.SeafilePath = txtSeafileDirectoryPath.Text;
        }

        private void btnSeafilePathValidate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Config.SeafilePath) || string.IsNullOrEmpty(Config.SeafileAPIURL) || string.IsNullOrEmpty(Config.SeafileAuthToken) || string.IsNullOrEmpty(Config.SeafileRepoID))
            {
                return;
            }

            Seafile sf = new Seafile(cbSeafileAPIURL.Text, txtSeafileAuthToken.Text, Config.SeafileRepoID);
            bool checkReturned = sf.ValidatePath(txtSeafileDirectoryPath.Text);

            if (checkReturned)
            {
                MessageBox.Show(Resources.UploadersConfigForm_TestFTPAccount_Connected_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.UploadersConfigForm_Error, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSeafileLibraryPassword_TextChanged(object sender, EventArgs e)
        {
            if (Config.SeafileIsLibraryEncrypted)
            {
                Config.SeafileEncryptedLibraryPassword = txtSeafileLibraryPassword.Text;
            }
        }

        private void btnSeafileLibraryPasswordValidate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Config.SeafileEncryptedLibraryPassword) || string.IsNullOrEmpty(Config.SeafileAPIURL) || string.IsNullOrEmpty(Config.SeafileAuthToken) || string.IsNullOrEmpty(Config.SeafileRepoID))
            {
                return;
            }

            Seafile sf = new Seafile(cbSeafileAPIURL.Text, txtSeafileAuthToken.Text, Config.SeafileRepoID);
            bool checkReturned = sf.DecryptLibrary(txtSeafileLibraryPassword.Text);

            if (checkReturned)
            {
                MessageBox.Show(Resources.UploadersConfigForm_TestFTPAccount_Connected_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.UploadersConfigForm_Error, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nudSeafileExpireDays_ValueChanged(object sender, EventArgs e)
        {
            Config.SeafileShareDaysToExpire = (int)nudSeafileExpireDays.Value;
        }

        private void txtSeafileSharePassword_TextChanged(object sender, EventArgs e)
        {
            Config.SeafileSharePassword = txtSeafileSharePassword.Text;
        }

        #endregion Seafile

        #region Streamable

        private void cboxStreamableAnonymous_CheckedChanged(object sender, EventArgs e)
        {
            Config.StreamableAnonymous = cbStreamableAnonymous.Checked;
            txtStreamableUsername.Enabled = !Config.StreamableAnonymous;
            txtStreamablePassword.Enabled = !Config.StreamableAnonymous;
        }

        private void txtStreamableUsername_TextChanged(object sender, EventArgs e)
        {
            Config.StreamableUsername = txtStreamableUsername.Text;
        }

        private void txtStreamablePassword_TextChanged(object sender, EventArgs e)
        {
            Config.StreamablePassword = txtStreamablePassword.Text;
        }

        private void cbStreamableUseDirectURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.StreamableUseDirectURL = cbStreamableUseDirectURL.Checked;
        }

        #endregion Streamable

        #region Uplea

        private void btnUpleaLogin_Click(object sender, EventArgs e)
        {
            btnUpleaLogin.Enabled = false;

            Uplea uplea = new Uplea();

            txtUpleaApiKey.Text = "";
            cbUpleaIsPremium.Checked = false;
            cbUpleaInstantDownloadEnabled.Checked = false;

            try
            {
                string apiKey = uplea.GetApiKey(txtUpleaUsername.Text, txtUpleaPassword.Text);

                txtUpleaApiKey.Text = apiKey;

                if (!string.IsNullOrEmpty(apiKey))
                {
                    UpleaGetUserInformationResponse upleaUserInformation = uplea.GetUserInformation(apiKey);
                    txtUpleaEmailAddress.Text = upleaUserInformation.Result.EmailAddress;
                    cbUpleaIsPremium.Checked = upleaUserInformation.Result.IsPremiumMember;
                    cbUpleaInstantDownloadEnabled.Checked = upleaUserInformation.Result.InstantDownloadEnabled;
                }
                else
                {
                    MessageBox.Show("Unable to retrieve API key and user details from Uplea. Please check your user credentials and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnUpleaLogin.Enabled = true;
            }
        }

        private void txtUpleaApiKey_TextChanged(object sender, EventArgs e)
        {
            Config.UpleaApiKey = txtUpleaApiKey.Text;

            if (string.IsNullOrEmpty(Config.UpleaApiKey))
            {
                txtUpleaEmailAddress.Text = "";
                cbUpleaIsPremium.Checked = false;
                cbUpleaInstantDownloadEnabled.Checked = false;
            }
        }

        private void txtUpleaEmailAddress_TextChanged(object sender, EventArgs e)
        {
            Config.UpleaEmailAddress = txtUpleaEmailAddress.Text;
        }

        private void cbUpleaIsPremium_CheckedChanged(object sender, EventArgs e)
        {
            Config.UpleaIsPremiumMember = cbUpleaIsPremium.Checked;
        }

        private void cbUpleaInstantDownloadEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Config.UpleaInstantDownloadEnabled = cbUpleaInstantDownloadEnabled.Checked;
        }

        #endregion Uplea

        #region Azure Storage

        private void txtAzureStorageAccountName_TextChanged(object sender, EventArgs e)
        {
            Config.AzureStorageAccountName = txtAzureStorageAccountName.Text;
        }

        private void txtAzureStorageAccessKey_TextChanged(object sender, EventArgs e)
        {
            Config.AzureStorageAccountAccessKey = txtAzureStorageAccessKey.Text;
        }

        private void txtAzureStorageContainer_TextChanged(object sender, EventArgs e)
        {
            Config.AzureStorageContainer = txtAzureStorageContainer.Text;
        }

        private void cbAzureStorageEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.AzureStorageEnvironment = cbAzureStorageEnvironment.Text;
        }

        private void txtAzureStorageCustomDomain_TextChanged(object sender, EventArgs e)
        {
            Config.AzureStorageCustomDomain = txtAzureStorageCustomDomain.Text;
        }

        private void btnAzureStoragePortal_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://portal.azure.com/?feature.customportal=false#blade/HubsExtension/Resources/resourceType/Microsoft.Storage%2FStorageAccounts");
        }

        #endregion Azure Storage

        #region Plik

        private void txtPlikURL_TextChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.URL = txtPlikURL.Text;
        }

        private void txtPlikAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.APIKey = txtPlikAPIKey.Text;
        }

        private void txtPlikLogin_TextChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.Login = txtPlikLogin.Text;
        }

        private void txtPlikPassword_TextChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.Password = txtPlikPassword.Text;
        }

        private void cbPlikIsSecured_CheckedChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.IsSecured = cbPlikIsSecured.Checked;
            txtPlikLogin.ReadOnly = !cbPlikIsSecured.Checked;
            txtPlikPassword.ReadOnly = !cbPlikIsSecured.Checked;
        }

        private void cbPlikRemovable_CheckedChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.Removable = cbPlikRemovable.Checked;
        }

        private void cbPlikComment_CheckedChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.HasComment = cbPlikComment.Checked;
            txtPlikComment.ReadOnly = !cbPlikComment.Checked;
        }

        private void txtPlikComment_TextChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.Comment = txtPlikComment.Text;
        }

        private void cbPlikOneShot_CheckedChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.OneShot = cbPlikOneShot.Checked;
        }

        private void cbxPlikTTLUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Plik.CalculateTTLValue(nudPlikTTL, cbxPlikTTLUnit.SelectedIndex, Config.PlikSettings.TTLUnit);
            Config.PlikSettings.TTLUnit = cbxPlikTTLUnit.SelectedIndex;
        }

        private void nudPlikTTL_ValueChanged(object sender, EventArgs e)
        {
            Config.PlikSettings.TTL = nudPlikTTL.Value;
        }

        #endregion Plik

        #region Gfycat

        private void atcGfycatAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.GfycatAccountType = accountType;
            oauth2Gfycat.Enabled = Config.GfycatAccountType == AccountType.User;
        }

        private void oauth2Gfycat_OpenButtonClicked()
        {
            GfycatAuthOpen();
        }

        private void oauth2Gfycat_CompleteButtonClicked(string code)
        {
            GfycatAuthComplete(code);
        }

        private void oauth2Gfycat_ClearButtonClicked()
        {
            Config.GfycatOAuth2Info = null;
        }

        private void oauth2Gfycat_RefreshButtonClicked()
        {
            GfycatAuthRefresh();
        }

        private void cbGfycatIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.GfycatIsPublic = cbGfycatIsPublic.Checked;
        }

        #endregion Gfycat

        #endregion File Uploaders

        #region URL Shorteners

        #region bit.ly

        private void oauth2Bitly_OpenButtonClicked()
        {
            BitlyAuthOpen();
        }

        private void oauth2Bitly_CompleteButtonClicked(string code)
        {
            BitlyAuthComplete(code);
        }

        private void oauth2Bitly_ClearButtonClicked()
        {
            Config.BitlyOAuth2Info = null;
        }

        private void txtBitlyDomain_TextChanged(object sender, EventArgs e)
        {
            Config.BitlyDomain = txtBitlyDomain.Text;
        }

        #endregion bit.ly

        #region Google URL Shortener

        private void atcGoogleURLShortenerAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.GoogleURLShortenerAccountType = accountType;
        }

        private void oauth2GoogleURLShortener_OpenButtonClicked()
        {
            GoogleURLShortenerAuthOpen();
        }

        private void oauth2GoogleURLShortener_CompleteButtonClicked(string code)
        {
            GoogleURLShortenerAuthComplete(code);
        }

        private void oauth2GoogleURLShortener_RefreshButtonClicked()
        {
            GoogleURLShortenerAuthRefresh();
        }

        private void oauth2GoogleURLShortener_ClearButtonClicked()
        {
            Config.GoogleURLShortenerOAuth2Info = null;
        }

        #endregion Google URL Shortener

        #region yourls.org

        private void txtYourlsAPIURL_TextChanged(object sender, EventArgs e)
        {
            Config.YourlsAPIURL = txtYourlsAPIURL.Text;
        }

        private void txtYourlsSignature_TextChanged(object sender, EventArgs e)
        {
            Config.YourlsSignature = txtYourlsSignature.Text.Trim();
            txtYourlsUsername.Enabled = txtYourlsPassword.Enabled = string.IsNullOrEmpty(Config.YourlsSignature);
        }

        private void txtYourlsUsername_TextChanged(object sender, EventArgs e)
        {
            Config.YourlsUsername = txtYourlsUsername.Text;
        }

        private void txtYourlsPassword_TextChanged(object sender, EventArgs e)
        {
            Config.YourlsPassword = txtYourlsPassword.Text;
        }

        #endregion yourls.org

        #region adf.ly

        private void txtAdflyAPIKEY_TextChanged(object sender, EventArgs e)
        {
            Config.AdFlyAPIKEY = txtAdflyAPIKEY.Text;
        }

        private void txtAdflyAPIUID_TextChanged(object sender, EventArgs e)
        {
            Config.AdFlyAPIUID = txtAdflyAPIUID.Text;
        }

        private void llAdflyLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("https://adf.ly/publisher/tools#tools-api");
        }

        #endregion adf.ly

        #region CoinURL

        private void txtCoinURLUUID_TextChanged(object sender, EventArgs e)
        {
            Config.CoinURLUUID = txtCoinURLUUID.Text;
        }

        #endregion CoinURL

        #region Polr

        private void txtPolrAPIHostname_TextChanged(object sender, EventArgs e)
        {
            Config.PolrAPIHostname = txtPolrAPIHostname.Text;
        }

        private void txtPolrAPIKey_TextChanged(object sender, EventArgs e)
        {
            Config.PolrAPIKey = txtPolrAPIKey.Text;
        }

        private void cbPolrIsSecret_CheckedChanged(object sender, EventArgs e)
        {
            Config.PolrIsSecret = cbPolrIsSecret.Checked;
        }

        private void cbPolrUseAPIv1_CheckedChanged(object sender, EventArgs e)
        {
            Config.PolrUseAPIv1 = cbPolrUseAPIv1.Checked;
        }

        #endregion Polr

        #endregion URL Shorteners

        #region Other Uploaders

        #region Twitter

        private void btnTwitterAdd_Click(object sender, EventArgs e)
        {
            OAuthInfo oauth = new OAuthInfo();
            Config.TwitterOAuthInfoList.Add(oauth);
            lbTwitterAccounts.Items.Add(oauth.Description);
            lbTwitterAccounts.SelectedIndex = lbTwitterAccounts.Items.Count - 1;

            TwitterUpdateSelected();
        }

        private void btnTwitterRemove_Click(object sender, EventArgs e)
        {
            int selected = lbTwitterAccounts.SelectedIndex;

            if (selected > -1)
            {
                lbTwitterAccounts.Items.RemoveAt(selected);
                Config.TwitterOAuthInfoList.RemoveAt(selected);

                if (lbTwitterAccounts.Items.Count > 0)
                {
                    lbTwitterAccounts.SelectedIndex = selected >= lbTwitterAccounts.Items.Count ? lbTwitterAccounts.Items.Count - 1 : selected;
                }
            }

            TwitterUpdateSelected();
        }

        private void lbTwitterAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            TwitterUpdateSelected();
        }

        private void btnTwitterNameUpdate_Click(object sender, EventArgs e)
        {
            OAuthInfo oauth = GetSelectedTwitterAccount();

            if (oauth != null)
            {
                oauth.Description = txtTwitterDescription.Text;
                lbTwitterAccounts.Items[lbTwitterAccounts.SelectedIndex] = oauth.Description;
            }
        }

        private void oauthTwitter_OpenButtonClicked()
        {
            TwitterAuthOpen();
        }

        private void oauthTwitter_CompleteButtonClicked(string code)
        {
            TwitterAuthComplete(code);
        }

        private void oauthTwitter_ClearButtonClicked()
        {
            TwitterAuthClear();
        }

        private void cbTwitterSkipMessageBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.TwitterSkipMessageBox = cbTwitterSkipMessageBox.Checked;
        }

        private void txtTwitterDefaultMessage_TextChanged(object sender, EventArgs e)
        {
            Config.TwitterDefaultMessage = txtTwitterDefaultMessage.Text;
        }

        #endregion Twitter

        #region Custom Uploaders

        private void btnCustomUploaderAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustomUploaderName.Text))
            {
                CustomUploaderItem item = GetCustomUploaderFromFields();
                Config.CustomUploadersList.Add(item);
                lbCustomUploaderList.Items.Add(item.Name);
                lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
                PrepareCustomUploaderList();
            }
        }

        private void btnCustomUploaderRemove_Click(object sender, EventArgs e)
        {
            if (lbCustomUploaderList.SelectedIndex > -1)
            {
                int index = lbCustomUploaderList.SelectedIndex;
                Config.CustomUploadersList.RemoveAt(index);
                lbCustomUploaderList.Items.RemoveAt(index);
                CustomUploaderClearFields();
                CustomUploaderFixSelectedUploader(index);
                PrepareCustomUploaderList();
            }
        }

        private void btnCustomUploaderUpdate_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();
        }

        private void lbCustomUploaderList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbCustomUploaderList.SelectedIndex;

            if (index > -1)
            {
                LoadCustomUploader(Config.CustomUploadersList[index]);
            }
        }

        private object eiCustomUploaders_ExportRequested()
        {
            return GetSelectedCustomUploader();
        }

        private void eiCustomUploaders_ImportRequested(object obj)
        {
            AddCustomUploader(obj as CustomUploaderItem);
        }

        private void btnCustomUploadersExportAll_Click(object sender, EventArgs e)
        {
            CustomUploaderExportAll();
        }

        private void btnCustomUploaderClearUploaders_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.UploadersConfigForm_Remove_all_custom_uploaders_Confirmation, "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CustomUploaderClearUploaders();
            }
        }

        private void btnCustomUploaderClear_Click(object sender, EventArgs e)
        {
            CustomUploaderClearFields();
        }

        private void cbCustomUploaderRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCustomUploaderFileForm.Enabled = (CustomUploaderRequestType)cbCustomUploaderRequestType.SelectedIndex == CustomUploaderRequestType.POST;
        }

        private void btnCustomUploaderRegexpAdd_Click(object sender, EventArgs e)
        {
            string regexp = txtCustomUploaderRegexp.Text;

            if (!string.IsNullOrEmpty(regexp))
            {
                lvCustomUploaderRegexps.Items.Add(regexp);
                txtCustomUploaderRegexp.Text = "";
                txtCustomUploaderRegexp.Focus();
            }
        }

        private void btnCustomUploaderRegexpRemove_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderRegexps.SelectedItems.Count > 0)
            {
                lvCustomUploaderRegexps.SelectedItems[0].Remove();
            }
        }

        private void btnCustomUploaderRegexpEdit_Click(object sender, EventArgs e)
        {
            string regexp = txtCustomUploaderRegexp.Text;

            if (lvCustomUploaderRegexps.SelectedItems.Count > 0 && !string.IsNullOrEmpty(regexp))
            {
                lvCustomUploaderRegexps.SelectedItems[0].Text = regexp;
            }
        }

        private void btnCustomUploaderRegexHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("http://regexone.com");
        }

        private void lvCustomUploaderRegexps_SelectedIndexChanged(object sender, EventArgs e)
        {
            string regex = "";

            if (lvCustomUploaderRegexps.SelectedItems.Count > 0)
            {
                regex = lvCustomUploaderRegexps.SelectedItems[0].Text;
            }

            txtCustomUploaderRegexp.Text = regex;

            btnCustomUploaderRegexAddSyntax.Enabled = lvCustomUploaderRegexps.SelectedItems.Count > 0;
        }

        private void btnCustomUploaderRegexAddSyntax_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderRegexps.SelectedIndices.Count > 0)
            {
                int selectedIndex = lvCustomUploaderRegexps.SelectedIndices[0];
                string regex = lvCustomUploaderRegexps.Items[selectedIndex].Text;

                if (!string.IsNullOrEmpty(regex))
                {
                    string syntax;
                    Match match = Regex.Match(regex, @"\((?:\?<(.+?)>)?.+?\)");

                    if (match.Success)
                    {
                        if (match.Groups.Count > 1 && !string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            syntax = string.Format("$regex:{0},{1}$", selectedIndex + 1, match.Groups[1].Value);
                        }
                        else
                        {
                            syntax = string.Format("$regex:{0},1$", selectedIndex + 1);
                        }
                    }
                    else
                    {
                        syntax = string.Format("$regex:{0}$", selectedIndex + 1);
                    }

                    AddTextToActiveURLField(syntax);
                }
            }
        }

        private void txtCustomUploaderJsonPath_TextChanged(object sender, EventArgs e)
        {
            btnCustomUploaderJsonAddSyntax.Enabled = !string.IsNullOrEmpty(txtCustomUploaderJsonPath.Text);
        }

        private void btnCustomUploadJsonPathHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("http://goessner.net/articles/JsonPath/");
        }

        private void btnCustomUploaderJsonAddSyntax_Click(object sender, EventArgs e)
        {
            string syntax = txtCustomUploaderJsonPath.Text;

            if (!string.IsNullOrEmpty(syntax))
            {
                if (syntax.StartsWith("$."))
                {
                    syntax = syntax.Substring(2);
                }

                syntax = string.Format("$json:{0}$", syntax);
                AddTextToActiveURLField(syntax);
            }
        }

        private void txtCustomUploaderXPath_TextChanged(object sender, EventArgs e)
        {
            btnCustomUploaderXmlSyntaxAdd.Enabled = !string.IsNullOrEmpty(txtCustomUploaderXPath.Text);
        }

        private void btnCustomUploaderXPathHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://www.w3schools.com/xml/xpath_syntax.asp");
        }

        private void btnCustomUploaderXmlSyntaxAdd_Click(object sender, EventArgs e)
        {
            string syntax = txtCustomUploaderXPath.Text;

            if (!string.IsNullOrEmpty(syntax))
            {
                syntax = string.Format("$xml:{0}$", syntax);
                AddTextToActiveURLField(syntax);
            }
        }

        private void txtCustomUploaderURL_Enter(object sender, EventArgs e)
        {
            customUploaderURLType = URLType.URL;
        }

        private void txtCustomUploaderThumbnailURL_Enter(object sender, EventArgs e)
        {
            customUploaderURLType = URLType.ThumbnailURL;
        }

        private void txtCustomUploaderDeletionURL_Enter(object sender, EventArgs e)
        {
            customUploaderURLType = URLType.DeletionURL;
        }

        private void AddTextToActiveURLField(string text)
        {
            TextBox tb;

            switch (customUploaderURLType)
            {
                default:
                case URLType.URL:
                    tb = txtCustomUploaderURL;
                    break;
                case URLType.ThumbnailURL:
                    tb = txtCustomUploaderThumbnailURL;
                    break;
                case URLType.DeletionURL:
                    tb = txtCustomUploaderDeletionURL;
                    break;
            }

            tb.AppendText(text);
        }

        private void btnCustomUploaderArgAdd_Click(object sender, EventArgs e)
        {
            string name = txtCustomUploaderArgName.Text;

            if (!string.IsNullOrEmpty(name))
            {
                string value = txtCustomUploaderArgValue.Text;
                lvCustomUploaderArguments.Items.Add(name).SubItems.Add(value);
                txtCustomUploaderArgName.Text = "";
                txtCustomUploaderArgValue.Text = "";
                txtCustomUploaderArgName.Focus();
            }
        }

        private void btnCustomUploaderArgRemove_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderArguments.SelectedItems.Count > 0)
            {
                lvCustomUploaderArguments.SelectedItems[0].Remove();
            }
        }

        private void btnCustomUploaderArgUpdate_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderArguments.SelectedItems.Count > 0)
            {
                string name = txtCustomUploaderArgName.Text;

                if (!string.IsNullOrEmpty(name))
                {
                    string value = txtCustomUploaderArgValue.Text;
                    lvCustomUploaderArguments.SelectedItems[0].Text = name;
                    lvCustomUploaderArguments.SelectedItems[0].SubItems[1].Text = value;
                }
            }
        }

        private void lvCustomUploaderArguments_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = "";
            string value = "";

            if (lvCustomUploaderArguments.SelectedItems.Count > 0)
            {
                name = lvCustomUploaderArguments.SelectedItems[0].Text;
                value = lvCustomUploaderArguments.SelectedItems[0].SubItems[1].Text;
            }

            txtCustomUploaderArgName.Text = name;
            txtCustomUploaderArgValue.Text = value;
        }

        private void btnCustomUploaderHeaderAdd_Click(object sender, EventArgs e)
        {
            string name = txtCustomUploaderHeaderName.Text;

            if (!string.IsNullOrEmpty(name))
            {
                string value = txtCustomUploaderHeaderValue.Text;
                lvCustomUploaderHeaders.Items.Add(name).SubItems.Add(value);
                txtCustomUploaderHeaderName.Text = "";
                txtCustomUploaderHeaderValue.Text = "";
                txtCustomUploaderHeaderName.Focus();
            }
        }

        private void btnCustomUploaderHeaderRemove_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderHeaders.SelectedItems.Count > 0)
            {
                lvCustomUploaderHeaders.SelectedItems[0].Remove();
            }
        }

        private void btnCustomUploaderHeaderUpdate_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderHeaders.SelectedItems.Count > 0)
            {
                string name = txtCustomUploaderHeaderName.Text;

                if (!string.IsNullOrEmpty(name))
                {
                    string value = txtCustomUploaderHeaderValue.Text;
                    lvCustomUploaderHeaders.SelectedItems[0].Text = name;
                    lvCustomUploaderHeaders.SelectedItems[0].SubItems[1].Text = value;
                }
            }
        }

        private void lvCustomUploaderHeaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = "";
            string value = "";

            if (lvCustomUploaderHeaders.SelectedItems.Count > 0)
            {
                name = lvCustomUploaderHeaders.SelectedItems[0].Text;
                value = lvCustomUploaderHeaders.SelectedItems[0].SubItems[1].Text;
            }

            txtCustomUploaderHeaderName.Text = name;
            txtCustomUploaderHeaderValue.Text = value;
        }

        private void cbCustomUploaderImageUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomImageUploaderSelected = cbCustomUploaderImageUploader.SelectedIndex;
        }

        private void cbCustomUploaderTextUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomTextUploaderSelected = cbCustomUploaderTextUploader.SelectedIndex;
        }

        private void cbCustomUploaderFileUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomFileUploaderSelected = cbCustomUploaderFileUploader.SelectedIndex;
        }

        private void cbCustomUploaderURLShortener_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomURLShortenerSelected = cbCustomUploaderURLShortener.SelectedIndex;
        }

        private void btnCustomUploaderImageUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomImageUploaderSelected))
            {
                TestCustomUploader(CustomUploaderType.Image, Config.CustomUploadersList[Config.CustomImageUploaderSelected]);
            }
        }

        private void btnCustomUploaderTextUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomTextUploaderSelected))
            {
                TestCustomUploader(CustomUploaderType.Text, Config.CustomUploadersList[Config.CustomTextUploaderSelected]);
            }
        }

        private void btnCustomUploaderFileUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomFileUploaderSelected))
            {
                TestCustomUploader(CustomUploaderType.File, Config.CustomUploadersList[Config.CustomFileUploaderSelected]);
            }
        }

        private void btnCustomUploaderURLShortenerTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomURLShortenerSelected))
            {
                TestCustomUploader(CustomUploaderType.URL, Config.CustomUploadersList[Config.CustomURLShortenerSelected]);
            }
        }

        private void btnCustomUploaderHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_CUSTOM_UPLOADER);
        }

        private void btnCustomUploaderExamples_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_CUSTOM_UPLOADERS);
        }

        private void btnCustomUploaderShowLastResponse_Click(object sender, EventArgs e)
        {
            string response = btnCustomUploaderShowLastResponse.Tag as string;

            if (!string.IsNullOrEmpty(response))
            {
                using (ResponseForm form = new ResponseForm(response))
                {
                    form.ShowDialog();
                }
            }
        }

        private void txtCustomUploaderLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        #endregion Custom Uploaders

        #endregion Other Uploaders
    }
}