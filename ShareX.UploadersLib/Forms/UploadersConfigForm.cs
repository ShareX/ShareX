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
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.Properties;
using ShareX.UploadersLib.TextUploaders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class UploadersConfigForm : Form
    {
        public UploadersConfig Config { get; private set; }

        private ImageList uploadersImageList;
        private URLType urlType = URLType.URL;

        public UploadersConfigForm(UploadersConfig config)
        {
            Config = config;
            InitializeComponent();
            Icon = ShareXResources.Icon;

            if (!string.IsNullOrEmpty(Config.FilePath))
            {
                Text += " - " + Config.FilePath;
            }
        }

        private void UploadersConfigForm_Shown(object sender, EventArgs e)
        {
            FormSettings();
            LoadSettings();
        }

        private void UploadersConfigForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FormSettings()
        {
            uploadersImageList = new ImageList();
            uploadersImageList.ColorDepth = ColorDepth.Depth32Bit;

            AddIconToTab(tpAdFly, Resources.AdFly);
            AddIconToTab(tpAmazonS3, Resources.AmazonS3);
            AddIconToTab(tpBitly, Resources.Bitly);
            AddIconToTab(tpBox, Resources.Box);
            AddIconToTab(tpChevereto, Resources.Chevereto);
            AddIconToTab(tpCoinURL, Resources.CoinURL);
            AddIconToTab(tpCustomUploaders, Resources.globe_network);
            AddIconToTab(tpDropbox, Resources.Dropbox);
            AddIconToTab(tpEmail, Resources.mail);
            AddIconToTab(tpFlickr, Resources.Flickr);
            AddIconToTab(tpFTP, Resources.folder_network);
            AddIconToTab(tpGe_tt, Resources.Gett);
            AddIconToTab(tpGist, Resources.GitHub);
            AddIconToTab(tpGoogleDrive, Resources.GoogleDrive);
            AddIconToTab(tpGoogleURLShortener, Resources.Google);
            AddIconToTab(tpHastebin, Resources.Hastebin);
            AddIconToTab(tpHostr, Resources.Hostr);
            AddIconToTab(tpImageShack, Resources.ImageShack);
            AddIconToTab(tpImgur, Resources.Imgur);
            AddIconToTab(tpJira, Resources.jira);
            AddIconToTab(tpLambda, Resources.Lambda);
            AddIconToTab(tpMediaFire, Resources.MediaFire);
            AddIconToTab(tpMega, Resources.Mega);
            AddIconToTab(tpMinus, Resources.Minus);
            AddIconToTab(tpOneDrive, Resources.OneDrive);
            AddIconToTab(tpOneTimeSecret, Resources.OneTimeSecret);
            AddIconToTab(tpOpenload, Resources.OpenLoad);
            AddIconToTab(tpOwnCloud, Resources.OwnCloud);
            AddIconToTab(tpPaste_ee, Resources.page_white_text);
            AddIconToTab(tpPastebin, Resources.Pastebin);
            AddIconToTab(tpPhotobucket, Resources.Photobucket);
            AddIconToTab(tpPicasa, Resources.Picasa);
            AddIconToTab(tpPolr, Resources.Polr);
            AddIconToTab(tpPomf, Resources.Pomf);
            AddIconToTab(tpPushbullet, Resources.Pushbullet);
            AddIconToTab(tpSeafile, Resources.Seafile);
            AddIconToTab(tpSendSpace, Resources.SendSpace);
            AddIconToTab(tpSharedFolder, Resources.server_network);
            AddIconToTab(tpSomeImage, Resources.SomeImage);
            AddIconToTab(tpStreamable, Resources.Streamable);
            AddIconToTab(tpSul, Resources.Sul);
            AddIconToTab(tpTinyPic, Resources.TinyPic);
            AddIconToTab(tpTwitter, Resources.Twitter);
            AddIconToTab(tpUp1, Resources.Up1);
            AddIconToTab(tpUpaste, Resources.Upaste);
            AddIconToTab(tpVgyme, Resources.Vgyme);
            AddIconToTab(tpYourls, Resources.Yourls);

            ttlvMain.ImageList = uploadersImageList;
            ttlvMain.MainTabControl = tcUploaders;
            ttlvMain.FocusListView();

            CodeMenu.Create(txtDropboxPath, ReplCodeMenuEntry.n, ReplCodeMenuEntry.t, ReplCodeMenuEntry.pn);
            CodeMenu.Create(txtAmazonS3ObjectPrefix, ReplCodeMenuEntry.n, ReplCodeMenuEntry.t, ReplCodeMenuEntry.pn);
            CodeMenu.Create(txtMediaFirePath, ReplCodeMenuEntry.n, ReplCodeMenuEntry.t, ReplCodeMenuEntry.pn);
            CodeMenu.Create(txtCustomUploaderArgValue, ReplCodeMenuEntry.n, ReplCodeMenuEntry.t, ReplCodeMenuEntry.pn);
            CodeMenu.Create(txtCustomUploaderHeaderValue, ReplCodeMenuEntry.n, ReplCodeMenuEntry.t, ReplCodeMenuEntry.pn);

            txtCustomUploaderLog.AddContextMenu();

            // FTP
            ucFTPAccounts.btnAdd.Click += FTPAccountAddButton_Click;
            ucFTPAccounts.btnRemove.Click += FTPAccountRemoveButton_Click;
            ucFTPAccounts.btnDuplicate.Click += FTPAccountDuplicateButton_Click;
            ucFTPAccounts.btnTest.Click += FTPAccountTestButton_Click;
            ucFTPAccounts.pgSettings.PropertyValueChanged += FtpAccountSettingsGrid_PropertyValueChanged;

            // Localhost
            ucLocalhostAccounts.btnAdd.Click += LocalhostAccountAddButton_Click;
            ucLocalhostAccounts.btnRemove.Click += LocalhostAccountRemoveButton_Click;
            ucLocalhostAccounts.btnDuplicate.Click += LocalhostAccountDuplicateButton_Click;
            ucLocalhostAccounts.btnTest.Visible = false;
            ucLocalhostAccounts.pgSettings.PropertyValueChanged += SettingsGrid_LocalhostPropertyValueChanged;

            eiFTP.ObjectType = typeof(FTPAccount);
            eiCustomUploaders.ObjectType = typeof(CustomUploaderItem);

#if DEBUG
            btnCheveretoTestAll.Visible = true;
            btnPomfTest.Visible = true;
#endif
        }

        private void AddIconToTab(TabPage tp, Icon icon)
        {
            uploadersImageList.Images.Add(tp.Name, icon);
            tp.ImageKey = tp.Name;
        }

        private void AddIconToTab(TabPage tp, Bitmap bitmap)
        {
            uploadersImageList.Images.Add(tp.Name, bitmap);
            tp.ImageKey = tp.Name;
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

            // Upaste

            txtUpasteUserKey.Text = Config.UpasteUserKey;
            cbUpasteIsPublic.Checked = Config.UpasteIsPublic;

            // Hastebin

            txtHastebinCustomDomain.Text = Config.HastebinCustomDomain;
            txtHastebinSyntaxHighlighting.Text = Config.HastebinSyntaxHighlighting;

            // OneTimeSecret

            txtOneTimeSecretEmail.Text = Config.OneTimeSecretAPIUsername;
            txtOneTimeSecretAPIKey.Text = Config.OneTimeSecretAPIKey;

            #endregion Text uploaders

            #region File uploaders

            // Dropbox

            if (OAuth2Info.CheckOAuth(Config.DropboxOAuth2Info))
            {
                oauth2Dropbox.Status = OAuthLoginStatus.LoginSuccessful;
            }

            txtDropboxPath.Text = Config.DropboxUploadPath;
            cbDropboxAutoCreateShareableLink.Checked = Config.DropboxAutoCreateShareableLink;
            cbDropboxURLType.Enabled = Config.DropboxAutoCreateShareableLink;
            cbDropboxURLType.Items.AddRange(Helpers.GetEnumNamesProper<DropboxURLType>());
            cbDropboxURLType.SelectedIndex = (int)Config.DropboxURLType;
            UpdateDropboxStatus();

            // Google Drive

            if (OAuth2Info.CheckOAuth(Config.GoogleDriveOAuth2Info))
            {
                oauth2GoogleDrive.Status = OAuthLoginStatus.LoginSuccessful;
                btnGoogleDriveRefreshFolders.Enabled = true;

                tvOneDrive.Enabled = true;
            }

            cbGoogleDriveIsPublic.Checked = Config.GoogleDriveIsPublic;
            cbGoogleDriveUseFolder.Checked = Config.GoogleDriveUseFolder;
            txtGoogleDriveFolderID.Enabled = Config.GoogleDriveUseFolder;
            txtGoogleDriveFolderID.Text = Config.GoogleDriveFolderID;

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

            // FTP

            if (Config.FTPAccountList == null || Config.FTPAccountList.Count == 0)
            {
                FTPSetup(new List<FTPAccount>());
            }
            else
            {
                FTPSetup(Config.FTPAccountList);
                if (ucFTPAccounts.lbAccounts.Items.Count > 0)
                {
                    ucFTPAccounts.lbAccounts.SelectedIndex = 0;
                }
            }

            // Email

            txtEmailSmtpServer.Text = Config.EmailSmtpServer;
            nudEmailSmtpPort.SetValue(Config.EmailSmtpPort);
            txtEmailFrom.Text = Config.EmailFrom;
            txtEmailPassword.Text = Config.EmailPassword;
            cbEmailConfirm.Checked = Config.EmailConfirmSend;
            cbEmailRememberLastTo.Checked = Config.EmailRememberLastTo;
            txtEmailDefaultSubject.Text = Config.EmailDefaultSubject;
            txtEmailDefaultBody.Text = Config.EmailDefaultBody;

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
                txtJiraConfigHelp.Text = string.Format(@"Howto configure your Jira server:

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
            txtAmazonS3BucketName.Text = Config.AmazonS3Settings.Bucket;
            txtAmazonS3ObjectPrefix.Text = Config.AmazonS3Settings.ObjectPrefix;
            cbAmazonS3CustomCNAME.Checked = Config.AmazonS3Settings.UseCustomCNAME;
            txtAmazonS3CustomDomain.Enabled = Config.AmazonS3Settings.UseCustomCNAME;
            txtAmazonS3CustomDomain.Text = Config.AmazonS3Settings.CustomDomain;
            cbAmazonS3UseRRS.Checked = Config.AmazonS3Settings.UseReducedRedundancyStorage;

            cbAmazonS3Endpoint.Items.AddRange(AmazonS3.RegionEndpoints.ToArray());
            cbAmazonS3Endpoint.SelectedItem = AmazonS3.GetCurrentRegion(Config.AmazonS3Settings);
            cbAmazonS3Endpoint.DisplayMember = "Name";
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

            // Up1

            txtUp1Host.Text = Config.Up1Host;
            txtUp1Key.Text = Config.Up1Key;

            // Lambda

            txtLambdaApiKey.Text = Config.LambdaSettings.UserAPIKey;
            cbLambdaUploadURL.Items.AddRange(Lambda.UploadURLs);
            cbLambdaUploadURL.SelectedItem = Config.LambdaSettings.UploadURL;

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
            if (Config.StreamableAnonymous)
            {
                txtStreamableUsername.Enabled = false;
                txtStreamablePassword.Enabled = false;
            }

            // openload.co

            txtOpenloadApiLogin.Text = Config.OpenloadAPILogin;
            txtOpenloadApiKey.Text = Config.OpenloadAPIKey;
            cbOpenloadUploadToFolder.Checked = Config.OpenloadUploadToSelectedFolder;
            OpenloadUpdateFolderTree();

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

            lbCustomUploaderList.Items.Clear();

            if (Config.CustomUploadersList == null)
            {
                Config.CustomUploadersList = new List<CustomUploaderItem>();
            }
            else
            {
                foreach (CustomUploaderItem customUploader in Config.CustomUploadersList)
                {
                    lbCustomUploaderList.Items.Add(customUploader.Name);
                }

                PrepareCustomUploaderList();
            }

#if DEBUG
            btnCustomUploadersExportAll.Visible = true;
#endif

            cbCustomUploaderRequestType.Items.AddRange(Enum.GetNames(typeof(CustomUploaderRequestType)));
            cbCustomUploaderResponseType.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ResponseType>());

            CustomUploaderClearFields();

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
                MessageBox.Show(ex.ToString(), Resources.UploadersConfigForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show(ex.ToString(), Resources.UploadersConfigForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #endregion Text Uploaders

        #region File Uploaders

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
            UpdateDropboxStatus();
        }

        private void btnDropboxShowFiles_Click(object sender, EventArgs e)
        {
            DropboxOpenFiles();
        }

        private void cbDropboxAutoCreateShareableLink_CheckedChanged(object sender, EventArgs e)
        {
            Config.DropboxAutoCreateShareableLink = cbDropboxAutoCreateShareableLink.Checked;
            cbDropboxURLType.Enabled = Config.DropboxAutoCreateShareableLink;
        }

        private void cbDropboxURLType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.DropboxURLType = (DropboxURLType)cbDropboxURLType.SelectedIndex;
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
                var folder = lvi.Tag as GoogleDrive.GoogleDriveFile;
                if (folder != null)
                {
                    txtGoogleDriveFolderID.Text = folder.id;
                }
            }
        }

        #endregion Google Drive

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

        #region FTP

        private void cboFtpImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.FTPSelectedImage = cboFtpImages.SelectedIndex;
        }

        private void cboFtpText_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.FTPSelectedText = cboFtpText.SelectedIndex;
        }

        private void cboFtpFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.FTPSelectedFile = cboFtpFiles.SelectedIndex;
        }

        private void btnFtpClient_Click(object sender, EventArgs e)
        {
            FTPOpenClient();
        }

        private object eiFTP_ExportRequested()
        {
            return GetSelectedFTPAccount();
        }

        private void eiFTP_ImportRequested(object obj)
        {
            AddFTPAccount(obj as FTPAccount);
        }

        private void FTPSetup(IEnumerable<FTPAccount> accs)
        {
            if (accs != null)
            {
                int selFtpList = ucFTPAccounts.lbAccounts.SelectedIndex;

                ucFTPAccounts.lbAccounts.Items.Clear();
                ucFTPAccounts.pgSettings.PropertySort = PropertySort.Categorized;
                cboFtpImages.Items.Clear();
                cboFtpText.Items.Clear();
                cboFtpFiles.Items.Clear();

                Config.FTPAccountList = new List<FTPAccount>();
                Config.FTPAccountList.AddRange(accs);

                foreach (FTPAccount acc in Config.FTPAccountList)
                {
                    ucFTPAccounts.lbAccounts.Items.Add(acc);
                    cboFtpImages.Items.Add(acc);
                    cboFtpText.Items.Add(acc);
                    cboFtpFiles.Items.Add(acc);
                }

                if (ucFTPAccounts.lbAccounts.Items.Count > 0)
                {
                    ucFTPAccounts.lbAccounts.SelectedIndex = selFtpList.Between(0, ucFTPAccounts.lbAccounts.Items.Count - 1);
                    cboFtpImages.SelectedIndex = Config.FTPSelectedImage.Between(0, ucFTPAccounts.lbAccounts.Items.Count - 1);
                    cboFtpText.SelectedIndex = Config.FTPSelectedText.Between(0, ucFTPAccounts.lbAccounts.Items.Count - 1);
                    cboFtpFiles.SelectedIndex = Config.FTPSelectedFile.Between(0, ucFTPAccounts.lbAccounts.Items.Count - 1);
                }
            }
        }

        private void FTPAccountAddButton_Click(object sender, EventArgs e)
        {
            AddFTPAccount(new FTPAccount());
        }

        private void FTPAccountRemoveButton_Click(object sender, EventArgs e)
        {
            int sel = ucFTPAccounts.lbAccounts.SelectedIndex;
            if (ucFTPAccounts.RemoveItem(sel))
            {
                Config.FTPAccountList.RemoveAt(sel);
            }
            FTPSetup(Config.FTPAccountList);
        }

        private void FTPAccountDuplicateButton_Click(object sender, EventArgs e)
        {
            FTPAccount src = (FTPAccount)ucFTPAccounts.lbAccounts.Items[ucFTPAccounts.lbAccounts.SelectedIndex];
            FTPAccount clone = src.Clone();
            AddFTPAccount(clone);
        }

        private void FTPAccountTestButton_Click(object sender, EventArgs e)
        {
            TestFTPAccountAsync(GetSelectedFTPAccount());
        }

        private void FtpAccountSettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FTPSetup(Config.FTPAccountList);
        }

        #endregion FTP

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

        private void chkEmailConfirm_CheckedChanged(object sender, EventArgs e)
        {
            Config.EmailConfirmSend = cbEmailConfirm.Checked;
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

        private void cbAmazonS3Endpoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var region = cbAmazonS3Endpoint.SelectedItem as AmazonS3Region;
            if (region != null)
            {
                Config.AmazonS3Settings.Endpoint = region.Identifier;
                UpdateAmazonS3Status();
            }
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

        #region Up1

        private void txtUp1Host_TextChanged(object sender, EventArgs e)
        {
            Config.Up1Host = txtUp1Host.Text;
        }

        private void txtUp1Key_TextChanged(object sender, EventArgs e)
        {
            Config.Up1Key = txtUp1Key.Text;
        }

        #endregion Up1

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
                        MessageBox.Show(result, "Pomf test results", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show(ex.ToString(), Resources.UploadersConfigForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Seafile.SeafileCheckAccInfoResponse SeafileCheckAccInfoResponse = sf.GetAccountInfo();

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
            List<Seafile.SeafileLibraryObj> SeafileLibraries = sf.GetLibraries();

            foreach (var SeafileLibrary in SeafileLibraries)
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
                Seafile.SeafileLibraryObj SealileLibraryInfo = (Seafile.SeafileLibraryObj)selectedItem.Tag;
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

        #endregion Streamable

        #region openload.co

        private void txtOpenloadApiLogin_TextChanged(object sender, EventArgs e)
        {
            Config.OpenloadAPILogin = txtOpenloadApiLogin.Text;
        }

        private void txtOpenloadApiKey_TextChanged(object sender, EventArgs e)
        {
            Config.OpenloadAPIKey = txtOpenloadApiKey.Text;
        }

        private void lblOpenloadApiLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("https://openload.co/account");
        }

        private void btnOpenloadRefreshFolders_Click(object sender, EventArgs e)
        {
            btnOpenloadRefreshFolders.Enabled = false;

            OpenloadUploader openload = new OpenloadUploader()
            {
                APILogin = Config.OpenloadAPILogin,
                APIKey = Config.OpenloadAPIKey
            };

            Config.OpenloadFolderTree = openload.GetFolderTree(string.Empty);
            OpenloadUpdateFolderTree();

            btnOpenloadRefreshFolders.Enabled = true;
        }

        private void cbOpenloadUploadToFolder_CheckedChanged(object sender, EventArgs e)
        {
            Config.OpenloadUploadToSelectedFolder = cbOpenloadUploadToFolder.Checked;
        }

        private void trvOpenloadFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Config.OpenloadSelectedFolderID = ((OpenloadFolder)e.Node.Tag).id;
        }

        private void OpenloadUpdateFolderTree()
        {
            trvOpenloadFolders.Nodes.Clear();
            if (Config.OpenloadFolderTree != null)
            {
                foreach (OpenloadFolderNode node in Config.OpenloadFolderTree.subNodes)
                {
                    TreeNode treeNode = OpenloadToTreeNode(node);
                    if (treeNode != null)
                    {
                        trvOpenloadFolders.Nodes.Add(treeNode);
                        if (string.Compare(node.folder.id, Config.OpenloadSelectedFolderID) == 0)
                            trvOpenloadFolders.SelectedNode = treeNode;
                    }
                }
            }
        }

        private TreeNode OpenloadToTreeNode(OpenloadFolderNode node)
        {
            if (node != null && node.folder != null)
            {
                TreeNode treeNode = new TreeNode(node.folder.name);
                treeNode.Tag = node.folder;
                foreach (OpenloadFolderNode subNode in node.subNodes)
                {
                    TreeNode subTreeNode = OpenloadToTreeNode(subNode);
                    if (subTreeNode != null)
                    {
                        treeNode.Nodes.Add(subTreeNode);
                        if (string.Compare(subNode.folder.id, Config.OpenloadSelectedFolderID) == 0)
                        {
                            trvOpenloadFolders.SelectedNode = subTreeNode;
                            TreeNode parentNode = treeNode;
                            while (parentNode != null)
                            {
                                parentNode.Expand();
                                parentNode = parentNode.Parent;
                            }
                        }
                    }
                }
                return treeNode;
            }
            return null;
        }

        #endregion openload.co

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
            if (MessageBox.Show("Remove all custom uploaders?", "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                txtCustomUploaderRegexp.Text = string.Empty;
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
            string regex = string.Empty;

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
            URLHelpers.OpenURL("http://www.w3schools.com/xsl/xpath_syntax.asp");
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
            urlType = URLType.URL;
        }

        private void txtCustomUploaderThumbnailURL_Enter(object sender, EventArgs e)
        {
            urlType = URLType.ThumbnailURL;
        }

        private void txtCustomUploaderDeletionURL_Enter(object sender, EventArgs e)
        {
            urlType = URLType.DeletionURL;
        }

        private void AddTextToActiveURLField(string text)
        {
            TextBox tb;

            switch (urlType)
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
                txtCustomUploaderArgName.Text = string.Empty;
                txtCustomUploaderArgValue.Text = string.Empty;
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
            string name = string.Empty;
            string value = string.Empty;

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
                txtCustomUploaderHeaderName.Text = string.Empty;
                txtCustomUploaderHeaderValue.Text = string.Empty;
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
            string name = string.Empty;
            string value = string.Empty;

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
            URLHelpers.OpenURL("https://github.com/ShareX/ShareX/wiki/Custom-Uploader");
        }

        private void btnCustomUploaderExamples_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://github.com/ShareX/CustomUploaders");
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