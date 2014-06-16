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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UploadersLib.FileUploaders;
using UploadersLib.HelperClasses;
using UploadersLib.ImageUploaders;
using UploadersLib.Properties;

namespace UploadersLib
{
    public partial class UploadersConfigForm : Form
    {
        public UploadersConfig Config { get; private set; }

        public UploadersConfigForm(UploadersConfig uploadersConfig)
        {
            Config = uploadersConfig;
            InitializeComponent();
            string title = "ShareX - Destination settings";
            if (!string.IsNullOrEmpty(Config.FilePath))
            {
                title += " - " + Config.FilePath;
            }
            Text = title;
            Icon = ShareXResources.Icon;
        }

        private void UploadersConfigForm_Shown(object sender, EventArgs e)
        {
            FormSettings();
            LoadSettings(Config);
        }

        private void UploadersConfigForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FormSettings()
        {
            ImageList uploadersImageList = new ImageList();
            uploadersImageList.ColorDepth = ColorDepth.Depth32Bit;
            uploadersImageList.Images.Add("ImageShack", Resources.ImageShack);
            uploadersImageList.Images.Add("TinyPic", Resources.TinyPic);
            uploadersImageList.Images.Add("Imgur", Resources.Imgur);
            uploadersImageList.Images.Add("Flickr", Resources.Flickr);
            uploadersImageList.Images.Add("Photobucket", Resources.Photobucket);
            uploadersImageList.Images.Add("Picasa", Resources.Picasa);
            uploadersImageList.Images.Add("Dropbox", Resources.Dropbox);
            uploadersImageList.Images.Add("Copy", Resources.Copy);
            uploadersImageList.Images.Add("GoogleDrive", Resources.GoogleDrive);
            uploadersImageList.Images.Add("Box", Resources.Box);
            uploadersImageList.Images.Add("Minus", Resources.Minus);
            uploadersImageList.Images.Add("FTP", Resources.folder_network);
            uploadersImageList.Images.Add("RapidShare", Resources.RapidShare);
            uploadersImageList.Images.Add("SendSpace", Resources.SendSpace);
            uploadersImageList.Images.Add("Gett", Resources.Gett);
            uploadersImageList.Images.Add("Hostr", Resources.Hostr);
            uploadersImageList.Images.Add("CustomUploader", Resources.globe_network);
            uploadersImageList.Images.Add("SharedFolders", Resources.server_network);
            uploadersImageList.Images.Add("Email", Resources.mail);
            uploadersImageList.Images.Add("Jira", Resources.jira);
            uploadersImageList.Images.Add("Mega", Resources.Mega);
            uploadersImageList.Images.Add("AmazonS3", Resources.AmazonS3);
            uploadersImageList.Images.Add("Pushbullet", Resources.Pushbullet);
            uploadersImageList.Images.Add("Pastebin", Resources.Pastebin);
            uploadersImageList.Images.Add("Pasteee", Resources.page_white_text);
            uploadersImageList.Images.Add("Gist", Resources.GitHub);
            uploadersImageList.Images.Add("Upaste", Resources.Upaste);
            uploadersImageList.Images.Add("Google", Resources.Google);
            uploadersImageList.Images.Add("Bitly", Resources.Bitly);
            uploadersImageList.Images.Add("Yourls", Resources.Yourls);
            uploadersImageList.Images.Add("Twitter", Resources.Twitter);

            tpImageShack.ImageKey = "ImageShack";
            tpTinyPic.ImageKey = "TinyPic";
            tpImgur.ImageKey = "Imgur";
            tpFlickr.ImageKey = "Flickr";
            tpPhotobucket.ImageKey = "Photobucket";
            tpPicasa.ImageKey = "Picasa";
            tpDropbox.ImageKey = "Dropbox";
            tpCopy.ImageKey = "Copy";
            tpGoogleDrive.ImageKey = "GoogleDrive";
            tpBox.ImageKey = "Box";
            tpMinus.ImageKey = "Minus";
            tpFTP.ImageKey = "FTP";
            tpRapidShare.ImageKey = "RapidShare";
            tpSendSpace.ImageKey = "SendSpace";
            tpSharedFolder.ImageKey = "SharedFolders";
            tpEmail.ImageKey = "Email";
            tpJira.ImageKey = "Jira";
            tpGe_tt.ImageKey = "Gett";
            tpHostr.ImageKey = "Hostr";
            tpCustomUploaders.ImageKey = "CustomUploader";
            tpPastebin.ImageKey = "Pastebin";
            tpPaste_ee.ImageKey = "Pasteee";
            tpPushbullet.ImageKey = "Pushbullet";
            tpGoogleURLShortener.ImageKey = "Google";
            tpBitly.ImageKey = "Bitly";
            tpYourls.ImageKey = "Yourls";
            tpTwitter.ImageKey = "Twitter";
            tpMega.ImageKey = "Mega";
            tpGist.ImageKey = "Gist";
            tpUpaste.ImageKey = "Upaste";
            tpAmazonS3.ImageKey = "AmazonS3";

            ttlvMain.ImageList = uploadersImageList;
            ttlvMain.MainTabControl = tcUploaders;
            ttlvMain.FocusListView();

            NameParser.CreateCodesMenu(txtDropboxPath, ReplacementVariables.n, ReplacementVariables.t, ReplacementVariables.pn);
            NameParser.CreateCodesMenu(txtAmazonS3ObjectPrefix, ReplacementVariables.n, ReplacementVariables.t, ReplacementVariables.pn);
            NameParser.CreateCodesMenu(txtCustomUploaderArgValue, ReplacementVariables.n);

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

            // Twitter
            ucTwitterAccounts.btnAdd.Click += TwitterAccountAddButton_Click;
            ucTwitterAccounts.btnRemove.Click += TwitterAccountRemoveButton_Click;
            ucTwitterAccounts.btnDuplicate.Click += TwitterAccountDuplicateButton_Click;
            ucTwitterAccounts.btnTest.Text = "Authorize";
            ucTwitterAccounts.btnTest.Click += TwitterAccountAuthButton_Click;
            ucTwitterAccounts.lbAccounts.SelectedIndexChanged += TwitterAccountSelectedIndexChanged;

            eiFTP.ObjectType = typeof(FTPAccount);
            eiCustomUploaders.ObjectType = typeof(CustomUploaderItem);
        }

        public void LoadSettings(UploadersConfig uploadersConfig)
        {
            #region Image uploaders

            // Imgur

            atcImgurAccountType.SelectedAccountType = Config.ImgurAccountType;
            cbImgurThumbnailType.Items.Clear();
            cbImgurThumbnailType.Items.AddRange(Helpers.GetEnumDescriptions<ImgurThumbnailType>());
            cbImgurThumbnailType.SelectedIndex = (int)Config.ImgurThumbnailType;
            txtImgurAlbumID.Text = Config.ImgurAlbumID;

            if (OAuth2Info.CheckOAuth(Config.ImgurOAuth2Info))
            {
                oauth2Imgur.Status = OAuthLoginStatus.LoginSuccessful;
                btnImgurRefreshAlbumList.Enabled = true;
            }

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
                lblPhotobucketAccountStatus.Text = "Login successful.";
                txtPhotobucketDefaultAlbumName.Text = Config.PhotobucketAccountInfo.AlbumID;
                lblPhotobucketParentAlbumPath.Text = "Parent album path e.g. " + Config.PhotobucketAccountInfo.AlbumID + "/Personal/" + DateTime.Now.Year;
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

            #endregion Image uploaders

            #region Text uploaders

            // Pastebin

            pgPastebinSettings.SelectedObject = Config.PastebinSettings;

            // Paste.ee

            txtPaste_eeUserAPIKey.Text = Config.Paste_eeUserAPIKey;

            // Gist

            atcGistAccountType.SelectedAccountType = Config.GistAnonymousLogin ? AccountType.Anonymous : AccountType.User;
            chkGistPublishPublic.Checked = Config.GistPublishPublic;

            if (OAuth2Info.CheckOAuth(Config.GistOAuth2Info))
            {
                oAuth2Gist.Status = OAuthLoginStatus.LoginSuccessful;
            }

            // Upaste

            txtUpasteUserKey.Text = Config.UpasteUserKey;
            cbUpasteIsPublic.Checked = Config.UpasteIsPublic;

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

            // Copy

            if (OAuthInfo.CheckOAuth(Config.CopyOAuthInfo))
            {
                oAuthCopy.Status = OAuthLoginStatus.LoginSuccessful;
            }

            txtCopyPath.Text = Config.CopyUploadPath;
            cbCopyURLType.Items.AddRange(Helpers.GetEnumNamesProper<CopyURLType>());
            cbCopyURLType.SelectedIndex = (int)Config.CopyURLType;
            UpdateCopyStatus();

            // Google Drive

            if (OAuth2Info.CheckOAuth(Config.GoogleDriveOAuth2Info))
            {
                oauth2GoogleDrive.Status = OAuthLoginStatus.LoginSuccessful;
            }

            cbGoogleDriveIsPublic.Checked = Config.GoogleDriveIsPublic;

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
            lblBoxFolderID.Text = "Selected folder: " + Config.BoxSelectedFolder.name;

            // Ge.tt

            if (Config.Ge_ttLogin != null && !string.IsNullOrEmpty(Config.Ge_ttLogin.AccessToken))
            {
                lblGe_ttStatus.Text = "Login successful.";
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
            nudEmailSmtpPort.Value = Config.EmailSmtpPort;
            txtEmailFrom.Text = Config.EmailFrom;
            txtEmailPassword.Text = Config.EmailPassword;
            chkEmailConfirm.Checked = Config.EmailConfirmSend;
            cbEmailRememberLastTo.Checked = Config.EmailRememberLastTo;
            txtEmailDefaultSubject.Text = Config.EmailDefaultSubject;
            txtEmailDefaultBody.Text = Config.EmailDefaultBody;

            // RapidShare

            txtRapidShareUsername.Text = Config.RapidShareUsername;
            txtRapidSharePassword.Text = Config.RapidSharePassword;
            txtRapidShareFolderID.Text = Config.RapidShareFolderID;

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

            cbCustomUploaderRequestType.Items.AddRange(Enum.GetNames(typeof(CustomUploaderRequestType)));
            cbCustomUploaderResponseType.Items.AddRange(Helpers.GetEnumDescriptions<ResponseType>());

            CustomUploaderClear();

            // Jira

            txtJiraHost.Text = Config.JiraHost;
            txtJiraIssuePrefix.Text = Config.JiraIssuePrefix;
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

- You can now authenticate to Jira", Links.URL_WEBSITE, Application.ProductName, APIKeys.JiraConsumerKey, Jira.PublicKey);

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
                Config.PushbulletSettings.DeviceList.ForEach(x => cboPushbulletDevices.Items.Add(x.Name ?? "Invalid device name"));

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
            cbAmazonS3Endpoint.Text = Config.AmazonS3Settings.Endpoint;
            txtAmazonS3BucketName.Text = Config.AmazonS3Settings.Bucket;
            txtAmazonS3ObjectPrefix.Text = Config.AmazonS3Settings.ObjectPrefix;
            cbAmazonS3CustomCNAME.Checked = Config.AmazonS3Settings.UseCustomCNAME;
            txtAmazonS3CustomDomain.Enabled = Config.AmazonS3Settings.UseCustomCNAME;
            txtAmazonS3CustomDomain.Text = Config.AmazonS3Settings.CustomDomain;
            cbAmazonS3UseRRS.Checked = Config.AmazonS3Settings.UseReducedRedundancyStorage;
            UpdateAmazonS3Status();

            #endregion File uploaders

            #region URL Shorteners

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

            // yourls.org

            txtYourlsAPIURL.Text = Config.YourlsAPIURL;
            txtYourlsSignature.Text = Config.YourlsSignature;
            txtYourlsUsername.Enabled = txtYourlsPassword.Enabled = string.IsNullOrEmpty(Config.YourlsSignature);
            txtYourlsUsername.Text = Config.YourlsUsername;
            txtYourlsPassword.Text = Config.YourlsPassword;

            #endregion URL Shorteners

            #region Other Services

            ucTwitterAccounts.lbAccounts.Items.Clear();

            foreach (OAuthInfo acc in Config.TwitterOAuthInfoList)
            {
                ucTwitterAccounts.lbAccounts.Items.Add(acc);
            }

            if (ucTwitterAccounts.lbAccounts.Items.Count > 0)
            {
                ucTwitterAccounts.lbAccounts.SelectedIndex = Config.TwitterSelectedAccount;
            }

            #endregion Other Services
        }

        #region Image Uploaders

        #region Imgur

        private void atcImgurAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.ImgurAccountType = accountType;
        }

        private void cbImgurThumbnailType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.ImgurThumbnailType = (ImgurThumbnailType)cbImgurThumbnailType.SelectedIndex;
        }

        private void txtImgurAlbumID_TextChanged(object sender, EventArgs e)
        {
            Config.ImgurAlbumID = txtImgurAlbumID.Text;
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
                    ImgurAlbumData album = (ImgurAlbumData)lvi.Tag;
                    txtImgurAlbumID.Text = album.id;
                }
            }
        }

        private void oauth2Imgur_OpenButtonClicked()
        {
            ImgurAuthOpen();
        }

        private void oauth2Imgur_CompleteButtonClicked(string code)
        {
            ImgurAuthComplete(code);
        }

        private void oauth2Imgur_RefreshButtonClicked()
        {
            ImgurAuthRefresh();
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
                    MessageBox.Show("Login successful.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Login failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Helpers.OpenURL("https://imageshack.com/user/" + Config.ImageShackSettings.Username);
            }
            else
            {
                txtImageShackUsername.Focus();
            }
        }

        private void btnImageShackOpenMyImages_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://imageshack.com/my/images");
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
                        MessageBox.Show("Login successful.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Login failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTinyPicOpenMyImages_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("http://tinypic.com/yourstuff.php");
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

        #endregion Image Uploaders

        #region Text Uploaders

        #region Pastebin

        private void btnPastebinLogin_Click(object sender, EventArgs e)
        {
            PastebinLogin();
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

        private void chkGistPublishPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.GistPublishPublic = ((CheckBox)sender).Checked;
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

        #endregion Text Uploaders

        #region File Uploaders

        #region Dropbox

        private void pbDropboxLogo_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://www.dropbox.com");
        }

        private void btnDropboxRegister_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("http://db.tt/CtPYXvu");
        }

        private void oauth2Dropbox_OpenButtonClicked()
        {
            DropboxAuthOpen();
        }

        private void oauth2Dropbox_CompleteButtonClicked(string code)
        {
            DropboxAuthComplete(code);
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

        #region Copy

        private void pbCopyLogo_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://copy.com");
        }

        private void btnCopyRegister_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://copy.com?r=hC3DMW");
        }

        private void txtCopyPath_TextChanged(object sender, EventArgs e)
        {
            Config.CopyUploadPath = txtCopyPath.Text;
            UpdateCopyStatus();
        }

        private void oAuthCopy_OpenButtonClicked()
        {
            CopyAuthOpen();
        }

        private void oAuthCopy_CompleteButtonClicked(string code)
        {
            CopyAuthComplete(code);
        }

        private void cbCopyURLType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CopyURLType = (CopyURLType)cbCopyURLType.SelectedIndex;
        }

        #endregion Copy

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

        private void cbGoogleDriveIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.GoogleDriveIsPublic = cbGoogleDriveIsPublic.Checked;
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
                    lblBoxFolderID.Text = "Selected folder: " + file.name;
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
                chkMinusPublic.Checked = tempMf.is_public;
            }
        }

        private void btnMinusFolderAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboMinusFolders.Text) && !MinusHasFolder(cboMinusFolders.Text))
            {
                btnMinusFolderAdd.Enabled = false;

                Minus minus = new Minus(Config.MinusConfig, Config.MinusOAuth2Info);
                MinusFolder dir = minus.CreateFolder(cboMinusFolders.Text, chkMinusPublic.Checked);
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
            FTPAccount clone = (FTPAccount)src.Clone();
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
            Config.EmailConfirmSend = chkEmailConfirm.Checked;
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

        #region RapidShare

        private void txtRapidShareUsername_TextChanged(object sender, EventArgs e)
        {
            Config.RapidShareUsername = txtRapidShareUsername.Text;
        }

        private void txtRapidSharePassword_TextChanged(object sender, EventArgs e)
        {
            Config.RapidSharePassword = txtRapidSharePassword.Text;
        }

        private void txtRapidShareFolderID_TextChanged(object sender, EventArgs e)
        {
            Config.RapidShareFolderID = txtRapidShareFolderID.Text;
        }

        private void btnRapidShareRefreshFolders_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Config.RapidShareUsername) || string.IsNullOrEmpty(Config.RapidSharePassword))
            {
                MessageBox.Show("RapidShare account username or password is empty.", "RapidShare refresh folders list failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                tvRapidShareFolders.Nodes.Clear();
                RapidShareFolderInfo root = new RapidShare(Config.RapidShareUsername, Config.RapidSharePassword).GetRootFolderWithChilds();
                RapidShareRecursiveAddChilds(tvRapidShareFolders.Nodes, root);
                tvRapidShareFolders.ExpandAll();
            }
        }

        private void RapidShareRecursiveAddChilds(TreeNodeCollection treeNodes, RapidShareFolderInfo folderInfo)
        {
            TreeNode treeNode = treeNodes.Add(folderInfo.FolderName);
            treeNode.Tag = folderInfo;

            foreach (RapidShareFolderInfo folderInfo2 in folderInfo.ChildFolders)
            {
                RapidShareRecursiveAddChilds(treeNode.Nodes, folderInfo2);
            }
        }

        private void tvRapidShareFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is RapidShareFolderInfo)
            {
                RapidShareFolderInfo folderInfo = (RapidShareFolderInfo)e.Node.Tag;
                txtRapidShareFolderID.Text = folderInfo.RealFolderID;
            }
        }

        #endregion RapidShare

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

        private void oAuthJira_RefreshButtonClicked()
        {
            MessageBox.Show("Refresh authorization is not supported.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                lblMegaStatus.Text = "Not configured";
                lblMegaStatus.ForeColor = NokColor;
            }
            else
            {
                cbMegaFolder.Items.Clear();

                Mega mega = new Mega(Config.MegaAuthInfos);
                if (!tryLogin || mega.TryLogin())
                {
                    lblMegaStatus.Text = "Configured";
                    lblMegaStatus.ForeColor = OkColor;

                    if (tryLogin)
                    {
                        Mega.DisplayNode[] nodes = mega.GetDisplayNodes().ToArray();
                        cbMegaFolder.Items.AddRange(nodes);
                        cbMegaFolder.SelectedItem = nodes.FirstOrDefault(n => n.Node != null && n.Node.Id == Config.MegaParentNodeId) ?? Mega.DisplayNode.EmptyNode;
                    }
                    else
                    {
                        cbMegaFolder.Items.Add("[Click refresh button]");
                        cbMegaFolder.SelectedIndex = 0;
                    }
                }
                else
                {
                    lblMegaStatus.Text = "Invalid authentication";
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
            Helpers.OpenURL("https://mega.co.nz/#register");
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
            Helpers.OpenURL("https://console.aws.amazon.com/iam/home?#security_credential");
        }

        private void txtAmazonS3SecretKey_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.SecretAccessKey = txtAmazonS3SecretKey.Text;
        }

        private void cbAmazonS3Endpoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.Endpoint = cbAmazonS3Endpoint.Text;
        }

        private void cbAmazonS3Endpoint_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.Endpoint = cbAmazonS3Endpoint.Text;
            UpdateAmazonS3Status();
        }

        private void txtAmazonS3BucketName_TextChanged(object sender, EventArgs e)
        {
            Config.AmazonS3Settings.Bucket = txtAmazonS3BucketName.Text;
            UpdateAmazonS3Status();
        }

        private void btnAmazonS3BucketNameOpen_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://console.aws.amazon.com/s3/home");
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
            LocalhostAccount clone = (LocalhostAccount)src.Clone();
            Config.LocalhostAccountList.Add(clone);
            ucLocalhostAccounts.AddItem(clone);
        }

        private void SettingsGrid_LocalhostPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LocalhostAccountsSetup(Config.LocalhostAccountList);
        }

        #endregion Shared folder

        #endregion File Uploaders

        #region URL Shorteners

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

        #endregion Google URL Shortener

        #region bit.ly

        private void oauth2Bitly_OpenButtonClicked()
        {
            BitlyAuthOpen();
        }

        private void oauth2Bitly_CompleteButtonClicked(string code)
        {
            BitlyAuthComplete(code);
        }

        #endregion bit.ly

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

        #endregion URL Shorteners

        #region Other Services

        private void TwitterAccountAddButton_Click(object sender, EventArgs e)
        {
            OAuthInfo acc = new OAuthInfo();
            Config.TwitterOAuthInfoList.Add(acc);
            ucTwitterAccounts.AddItem(acc);
        }

        private void TwitterAccountRemoveButton_Click(object sender, EventArgs e)
        {
            int sel = ucTwitterAccounts.lbAccounts.SelectedIndex;
            if (ucTwitterAccounts.RemoveItem(sel))
            {
                Config.TwitterOAuthInfoList.RemoveAt(sel);
            }
        }

        private void TwitterAccountDuplicateButton_Click(object sender, EventArgs e)
        {
            OAuthInfo src = (OAuthInfo)ucTwitterAccounts.lbAccounts.Items[ucTwitterAccounts.lbAccounts.SelectedIndex];
            OAuthInfo clone = (OAuthInfo)src.Clone();
            Config.TwitterOAuthInfoList.Add(clone);
            ucTwitterAccounts.AddItem(clone);
        }

        private void TwitterAccountAuthButton_Click(object sender, EventArgs e)
        {
            TwitterAuthOpen();
        }

        private void btnTwitterLogin_Click(object sender, EventArgs e)
        {
            TwitterAuthComplete();
        }

        private void TwitterAccountSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ucTwitterAccounts.lbAccounts.SelectedIndex > -1)
            {
                Config.TwitterSelectedAccount = ucTwitterAccounts.lbAccounts.SelectedIndex;
            }
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

        #endregion Other Services

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
                CustomUploaderClear();
                FixSelectedUploader(index);
                PrepareCustomUploaderList();
            }
        }

        private void FixSelectedUploader(int removedIndex)
        {
            if (Config.CustomImageUploaderSelected == removedIndex) Config.CustomImageUploaderSelected = 0;
            else if (Config.CustomImageUploaderSelected > removedIndex) Config.CustomImageUploaderSelected--;

            if (Config.CustomTextUploaderSelected == removedIndex) Config.CustomTextUploaderSelected = 0;
            else if (Config.CustomTextUploaderSelected > removedIndex) Config.CustomTextUploaderSelected--;

            if (Config.CustomFileUploaderSelected == removedIndex) Config.CustomFileUploaderSelected = 0;
            else if (Config.CustomFileUploaderSelected > removedIndex) Config.CustomFileUploaderSelected--;

            if (Config.CustomURLShortenerSelected == removedIndex) Config.CustomURLShortenerSelected = 0;
            else if (Config.CustomURLShortenerSelected > removedIndex) Config.CustomURLShortenerSelected--;
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

        private void lvCustomUploaderRegexps_SelectedIndexChanged(object sender, EventArgs e)
        {
            string regex = string.Empty;

            if (lvCustomUploaderRegexps.SelectedItems.Count > 0)
            {
                regex = lvCustomUploaderRegexps.SelectedItems[0].Text;
            }

            txtCustomUploaderRegexp.Text = regex;
        }

        private void btnCustomUploaderArgAdd_Click(object sender, EventArgs e)
        {
            string name = txtCustomUploaderArgName.Text;
            string value = txtCustomUploaderArgValue.Text;

            if (!string.IsNullOrEmpty(name))
            {
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

        private void btnCustomUploaderArgEdit_Click(object sender, EventArgs e)
        {
            string name = txtCustomUploaderArgName.Text;
            string value = txtCustomUploaderArgValue.Text;

            if (lvCustomUploaderArguments.SelectedItems.Count > 0 && !string.IsNullOrEmpty(name))
            {
                lvCustomUploaderArguments.SelectedItems[0].Text = name;
                lvCustomUploaderArguments.SelectedItems[0].SubItems[1].Text = value;
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

        private void btnCustomUploaderClear_Click(object sender, EventArgs e)
        {
            CustomUploaderClear();
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
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.Image, Config.CustomUploadersList[Config.CustomImageUploaderSelected]);
            }
        }

        private void btnCustomUploaderTextUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomTextUploaderSelected))
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.Text, Config.CustomUploadersList[Config.CustomTextUploaderSelected]);
            }
        }

        private void btnCustomUploaderFileUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomFileUploaderSelected))
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.File, Config.CustomUploadersList[Config.CustomFileUploaderSelected]);
            }
        }

        private void btnCustomUploaderURLShortenerTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomUploadersList.IsValidIndex(Config.CustomURLShortenerSelected))
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.URL, Config.CustomUploadersList[Config.CustomURLShortenerSelected]);
            }
        }

        private void btnCustomUploaderHelp_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://docs.google.com/document/d/1TSttAfH-1970JNsu3i9tl6UY0a8KNbUCeGri0Fs-jcU/edit?usp=sharing");
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
            Helpers.OpenURL(e.LinkText);
        }

        #endregion Custom Uploaders
    }
}