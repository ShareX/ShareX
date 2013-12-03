#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UploadersLib.FileUploaders;
using UploadersLib.HelperClasses;
using UploadersLib.ImageUploaders;
using UploadersLib.Properties;
using UploadersLib.SocialServices;

namespace UploadersLib
{
    public partial class UploadersConfigForm : Form
    {
        private ContextMenuStrip cmsCustomUploaderArgValue;

        private void ControlSettings()
        {
            ImageList imageUploadersImageList = new ImageList();
            imageUploadersImageList.ColorDepth = ColorDepth.Depth32Bit;
            imageUploadersImageList.Images.Add("ImageShack", Resources.ImageShack);
            imageUploadersImageList.Images.Add("TinyPic", Resources.TinyPic);
            imageUploadersImageList.Images.Add("Imgur", Resources.Imgur);
            imageUploadersImageList.Images.Add("Flickr", Resources.Flickr);
            imageUploadersImageList.Images.Add("Photobucket", Resources.Photobucket);
            imageUploadersImageList.Images.Add("Picasa", Resources.Picasa);
            imageUploadersImageList.Images.Add("TwitPic", Resources.TwitPic);
            imageUploadersImageList.Images.Add("TwitSnaps", Resources.TwitSnaps);
            imageUploadersImageList.Images.Add("YFrog", Resources.YFrog);
            tcImageUploaders.ImageList = imageUploadersImageList;

            ImageList fileUploadersImageList = new ImageList();
            fileUploadersImageList.ColorDepth = ColorDepth.Depth32Bit;
            fileUploadersImageList.Images.Add("Dropbox", Resources.Dropbox);
            fileUploadersImageList.Images.Add("GoogleDrive", Resources.GoogleDrive);
            fileUploadersImageList.Images.Add("Box", Resources.Box);
            fileUploadersImageList.Images.Add("Minus", Resources.Minus);
            fileUploadersImageList.Images.Add("FTP", Resources.folder_network);
            fileUploadersImageList.Images.Add("RapidShare", Resources.RapidShare);
            fileUploadersImageList.Images.Add("SendSpace", Resources.SendSpace);
            fileUploadersImageList.Images.Add("Gett", Resources.Gett);
            fileUploadersImageList.Images.Add("Localhostr", Resources.Localhostr);
            fileUploadersImageList.Images.Add("CustomUploader", Resources.globe_network);
            fileUploadersImageList.Images.Add("SharedFolders", Resources.server_network);
            fileUploadersImageList.Images.Add("Email", Resources.mail);
            fileUploadersImageList.Images.Add("Jira", Resources.jira);
            fileUploadersImageList.Images.Add("Mega", Resources.Mega);
            tcFileUploaders.ImageList = fileUploadersImageList;

            ImageList textUploadersImageList = new ImageList();
            textUploadersImageList.ColorDepth = ColorDepth.Depth32Bit;
            textUploadersImageList.Images.Add("Pastebin", Resources.Pastebin);
            textUploadersImageList.Images.Add("Gist", Resources.Gist);
            tcTextUploaders.ImageList = textUploadersImageList;

            ImageList urlShortenersImageList = new ImageList();
            urlShortenersImageList.ColorDepth = ColorDepth.Depth32Bit;
            urlShortenersImageList.Images.Add("Google", Resources.Google);
            tcURLShorteners.ImageList = urlShortenersImageList;

            ImageList socialNetworkingServicesImageList = new ImageList();
            socialNetworkingServicesImageList.ColorDepth = ColorDepth.Depth32Bit;
            socialNetworkingServicesImageList.Images.Add("Twitter", Resources.Twitter);
            tcSocialNetworkingServices.ImageList = socialNetworkingServicesImageList;

            tpImageShack.ImageKey = "ImageShack";
            tpTinyPic.ImageKey = "TinyPic";
            tpImgur.ImageKey = "Imgur";
            tpFlickr.ImageKey = "Flickr";
            tpPhotobucket.ImageKey = "Photobucket";
            tpPicasa.ImageKey = "Picasa";
            tpTwitPic.ImageKey = "TwitPic";
            tpTwitSnaps.ImageKey = "TwitSnaps";
            tpYFrog.ImageKey = "YFrog";
            tpDropbox.ImageKey = "Dropbox";
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
            tpHostr.ImageKey = "Localhostr";
            tpCustomUploaders.ImageKey = "CustomUploader";
            tpPastebin.ImageKey = "Pastebin";
            tpGoogleURLShortener.ImageKey = "Google";
            tpTwitter.ImageKey = "Twitter";
            tpMega.ImageKey = "Mega";
            tpGist.ImageKey = "Gist";

            cmsCustomUploaderArgValue = NameParser.CreateCodesMenu(txtCustomUploaderArgValue, ReplacementVariables.n);

            txtCustomUploaderLog.AddContextMenu();
        }

        public void LoadSettings(UploadersConfig uploadersConfig)
        {
            #region Image uploaders

            // ImageShack

            atcImageShackAccountType.SelectedAccountType = Config.ImageShackAccountType;
            txtImageShackRegistrationCode.Text = Config.ImageShackRegistrationCode;
            txtImageShackUsername.Text = Config.ImageShackUsername;
            cbImageShackIsPublic.Checked = Config.ImageShackShowImagesInPublic;

            // TinyPic

            atcTinyPicAccountType.SelectedAccountType = Config.TinyPicAccountType;
            txtTinyPicUsername.Text = Config.TinyPicUsername;
            txtTinyPicPassword.Text = Config.TinyPicPassword;
            cbTinyPicRememberUsernamePassword.Checked = Config.TinyPicRememberUserPass;
            txtTinyPicRegistrationCode.Text = Config.TinyPicRegistrationCode;

            // Imgur

            atcImgurAccountType.SelectedAccountType = Config.ImgurAccountType;
            cbImgurThumbnailType.Items.Clear();
            cbImgurThumbnailType.Items.AddRange(Helpers.GetEnumDescriptions<ImgurThumbnailType>());
            cbImgurThumbnailType.SelectedIndex = (int)Config.ImgurThumbnailType;
            txtImgurAlbumID.Text = Config.ImgurAlbumID;

            if (OAuth2Info.CheckOAuth(Config.ImgurOAuth2Info))
            {
                oauth2Imgur.Status = "Login successful.";
                oauth2Imgur.LoginStatus = true;
                btnImgurRefreshAlbumList.Enabled = true;
            }

            // Photobucket

            if (OAuthInfo.CheckOAuth(Config.PhotobucketOAuthInfo))
            {
                lblPhotobucketAccountStatus.Text = "Login successful: " + Config.PhotobucketOAuthInfo.UserToken;
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
                oauth2Picasa.Status = "Login successful.";
                oauth2Picasa.LoginStatus = true;
                btnPicasaRefreshAlbumList.Enabled = true;
            }

            txtPicasaAlbumID.Text = Config.PicasaAlbumID;

            // Flickr

            pgFlickrAuthInfo.SelectedObject = Config.FlickrAuthInfo;
            pgFlickrSettings.SelectedObject = Config.FlickrSettings;

            // TwitPic

            chkTwitPicShowFull.Checked = Config.TwitPicShowFull;
            cboTwitPicThumbnailMode.Items.Clear();
            cboTwitPicThumbnailMode.Items.AddRange(Helpers.GetEnumDescriptions<TwitPicThumbnailType>());
            cboTwitPicThumbnailMode.SelectedIndex = (int)Config.TwitPicThumbnailMode;

            // YFrog

            txtYFrogUsername.Text = Config.YFrogUsername;
            txtYFrogPassword.Text = Config.YFrogPassword;

            #endregion Image uploaders

            #region Text uploaders

            // Pastebin

            pgPastebinSettings.SelectedObject = Config.PastebinSettings;

            // Paste.ee

            txtPaste_eeUserAPIKey.Text = Config.Paste_eeUserAPIKey;

            // Gist

            atcGistAccountType.SelectedAccountType = Config.GistAnonymousLogin ? AccountType.Anonymous : AccountType.User;
            chkGistPublishPublic.Checked = Config.GistPublishPublic;

            #endregion Text uploaders

            #region File uploaders

            // Dropbox

            txtDropboxPath.Text = Config.DropboxUploadPath;
            cbDropboxAutoCreateShareableLink.Checked = Config.DropboxAutoCreateShareableLink;
            cbDropboxShortURL.Enabled = Config.DropboxAutoCreateShareableLink;
            cbDropboxShortURL.Checked = Config.DropboxShortURL;
            UpdateDropboxStatus();

            // Google Drive

            if (OAuth2Info.CheckOAuth(Config.GoogleDriveOAuth2Info))
            {
                oauth2GoogleDrive.Status = "Login successful.";
                oauth2GoogleDrive.LoginStatus = true;
            }

            // Minus

            cbMinusURLType.Items.Clear();
            cbMinusURLType.Items.AddRange(Enum.GetNames(typeof(MinusLinkType)));
            MinusUpdateControls();

            // Box

            txtBoxFolderID.Text = Config.BoxFolderID;

            // Ge.tt

            lblGe_ttAccessToken.Text = "Access token:";

            if (Config.Ge_ttLogin != null && !string.IsNullOrEmpty(Config.Ge_ttLogin.AccessToken))
            {
                lblGe_ttAccessToken.Text += " " + Config.Ge_ttLogin.AccessToken;
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
                if (ucFTPAccounts.AccountsList.Items.Count > 0)
                {
                    ucFTPAccounts.AccountsList.SelectedIndex = 0;
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
                if (ucLocalhostAccounts.AccountsList.Items.Count > 0)
                {
                    ucLocalhostAccounts.AccountsList.SelectedIndex = 0;
                    cboSharedFolderImages.SelectedIndex = Config.LocalhostSelectedImages.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
                    cboSharedFolderText.SelectedIndex = Config.LocalhostSelectedText.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
                    cboSharedFolderFiles.SelectedIndex = Config.LocalhostSelectedFiles.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
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

- You can now authenticate to Jira", Links.URL_WEBSITE, Application.ProductName, ApiKeys.JiraConsumerKey, Jira.PublicKey);

            if (OAuthInfo.CheckOAuth(Config.JiraOAuthInfo))
            {
                oAuthJira.Status = "Login successful.";
                oAuthJira.LoginStatus = true;
            }

            // Mega

            MegaConfigureTab(false);

            #endregion File uploaders

            #region URL Shorteners

            atcGoogleURLShortenerAccountType.SelectedAccountType = Config.GoogleURLShortenerAccountType;

            if (OAuth2Info.CheckOAuth(Config.GoogleURLShortenerOAuth2Info))
            {
                oauth2GoogleURLShortener.Status = "Login successful.";
                oauth2GoogleURLShortener.LoginStatus = true;
            }

            #endregion URL Shorteners

            #region Other Services

            ucTwitterAccounts.AccountsList.Items.Clear();

            foreach (OAuthInfo acc in Config.TwitterOAuthInfoList)
            {
                ucTwitterAccounts.AccountsList.Items.Add(acc);
            }

            if (ucTwitterAccounts.AccountsList.Items.Count > 0)
            {
                ucTwitterAccounts.AccountsList.SelectedIndex = Config.TwitterSelectedAccount;
            }

            #endregion Other Services
        }

        private void CreateUserControlEvents()
        {
            // FTP

            ucFTPAccounts.btnAdd.Click += FTPAccountAddButton_Click;
            ucFTPAccounts.btnRemove.Click += FTPAccountRemoveButton_Click;
            ucFTPAccounts.btnTest.Click += FTPAccountTestButton_Click;
            ucFTPAccounts.btnClone.Visible = true;
            ucFTPAccounts.btnClone.Click += FTPAccountCloneButton_Click;
            ucFTPAccounts.AccountsList.SelectedIndexChanged += FTPAccountsList_SelectedIndexChanged;
            ucFTPAccounts.SettingsGrid.PropertyValueChanged += FtpAccountSettingsGrid_PropertyValueChanged;

            // Localhost

            ucLocalhostAccounts.btnAdd.Click += LocalhostAccountAddButton_Click;
            ucLocalhostAccounts.btnRemove.Click += LocalhostAccountRemoveButton_Click;
            ucLocalhostAccounts.btnTest.Visible = false;
            ucLocalhostAccounts.AccountsList.SelectedIndexChanged += LocalhostAccountsList_SelectedIndexChanged;
            ucLocalhostAccounts.SettingsGrid.PropertyValueChanged += SettingsGrid_LocalhostPropertyValueChanged;

            // Twitter

            ucTwitterAccounts.btnAdd.Text = "Add";
            ucTwitterAccounts.btnAdd.Click += TwitterAccountAddButton_Click;
            ucTwitterAccounts.btnRemove.Click += TwitterAccountRemoveButton_Click;
            ucTwitterAccounts.btnTest.Text = "Authorize";
            ucTwitterAccounts.btnTest.Click += TwitterAccountAuthButton_Click;
            ucTwitterAccounts.SettingsGrid.PropertySort = PropertySort.NoSort;
            ucTwitterAccounts.AccountsList.SelectedIndexChanged += TwitterAccountList_SelectedIndexChanged;
        }

        #region Localhost

        private void LocalhostAccountAddButton_Click(object sender, EventArgs e)
        {
            LocalhostAccount acc = new LocalhostAccount("New Account");
            Config.LocalhostAccountList.Add(acc);
            ucLocalhostAccounts.AccountsList.Items.Add(acc);
            ucLocalhostAccounts.AccountsList.SelectedIndex = ucLocalhostAccounts.AccountsList.Items.Count - 1;
        }

        private void LocalhostAccountRemoveButton_Click(object sender, EventArgs e)
        {
            int sel = ucLocalhostAccounts.AccountsList.SelectedIndex;
            if (ucLocalhostAccounts.RemoveItem(sel))
            {
                Config.LocalhostAccountList.RemoveAt(sel);
            }
        }

        private void LocalhostAccountsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sel = ucLocalhostAccounts.AccountsList.SelectedIndex;

            if (Config.LocalhostAccountList.IsValidIndex(sel))
            {
                LocalhostAccount acc = Config.LocalhostAccountList[sel];
                ucLocalhostAccounts.SettingsGrid.SelectedObject = acc;
            }
        }

        private void SettingsGrid_LocalhostPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LocalhostAccountsSetup(Config.LocalhostAccountList);
        }

        private void LocalhostAccountsSetup(IEnumerable<LocalhostAccount> accs)
        {
            if (accs != null)
            {
                int sel = ucLocalhostAccounts.AccountsList.SelectedIndex;

                ucLocalhostAccounts.AccountsList.Items.Clear();
                Config.LocalhostAccountList = new List<LocalhostAccount>();
                Config.LocalhostAccountList.AddRange(accs);

                cboSharedFolderFiles.Items.Clear();
                cboSharedFolderImages.Items.Clear();
                cboSharedFolderText.Items.Clear();

                foreach (LocalhostAccount acc in Config.LocalhostAccountList)
                {
                    ucLocalhostAccounts.AccountsList.Items.Add(acc);
                    cboSharedFolderFiles.Items.Add(acc);
                    cboSharedFolderImages.Items.Add(acc);
                    cboSharedFolderText.Items.Add(acc);
                }

                if (ucLocalhostAccounts.AccountsList.Items.Count > 0)
                {
                    ucLocalhostAccounts.AccountsList.SelectedIndex = sel.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
                    cboSharedFolderFiles.SelectedIndex = Config.LocalhostSelectedFiles.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
                    cboSharedFolderImages.SelectedIndex = Config.LocalhostSelectedImages.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
                    cboSharedFolderText.SelectedIndex = Config.LocalhostSelectedText.Between(0, ucLocalhostAccounts.AccountsList.Items.Count - 1);
                }
            }
        }

        #endregion Localhost

        #region FTP

        private void FTPSetup(IEnumerable<FTPAccount> accs)
        {
            if (accs != null)
            {
                int selFtpList = ucFTPAccounts.AccountsList.SelectedIndex;

                ucFTPAccounts.AccountsList.Items.Clear();
                ucFTPAccounts.SettingsGrid.PropertySort = PropertySort.Categorized;
                cboFtpImages.Items.Clear();
                cboFtpText.Items.Clear();
                cboFtpFiles.Items.Clear();

                Config.FTPAccountList = new List<FTPAccount>();
                Config.FTPAccountList.AddRange(accs);

                foreach (FTPAccount acc in Config.FTPAccountList)
                {
                    ucFTPAccounts.AccountsList.Items.Add(acc);
                    cboFtpImages.Items.Add(acc);
                    cboFtpText.Items.Add(acc);
                    cboFtpFiles.Items.Add(acc);
                }

                if (ucFTPAccounts.AccountsList.Items.Count > 0)
                {
                    ucFTPAccounts.AccountsList.SelectedIndex = selFtpList.Between(0, ucFTPAccounts.AccountsList.Items.Count - 1);
                    cboFtpImages.SelectedIndex = Config.FTPSelectedImage.Between(0, ucFTPAccounts.AccountsList.Items.Count - 1);
                    cboFtpText.SelectedIndex = Config.FTPSelectedText.Between(0, ucFTPAccounts.AccountsList.Items.Count - 1);
                    cboFtpFiles.SelectedIndex = Config.FTPSelectedFile.Between(0, ucFTPAccounts.AccountsList.Items.Count - 1);
                }
            }
        }

        private void FTPAccountAddButton_Click(object sender, EventArgs e)
        {
            FTPAccount acc = new FTPAccount("New Account");
            Config.FTPAccountList.Add(acc);
            ucFTPAccounts.AccountsList.Items.Add(acc);
            ucFTPAccounts.AccountsList.SelectedIndex = ucFTPAccounts.AccountsList.Items.Count - 1;
            FTPSetup(Config.FTPAccountList);
        }

        private void FTPAccountRemoveButton_Click(object sender, EventArgs e)
        {
            int sel = ucFTPAccounts.AccountsList.SelectedIndex;
            if (ucFTPAccounts.RemoveItem(sel))
            {
                Config.FTPAccountList.RemoveAt(sel);
            }
            FTPSetup(Config.FTPAccountList);
        }

        private void FTPAccountTestButton_Click(object sender, EventArgs e)
        {
            if (CheckFTPAccounts())
            {
                TestFTPAccount(Config.FTPAccountList[ucFTPAccounts.AccountsList.SelectedIndex], false);
            }
        }

        private void FTPAccountCloneButton_Click(object sender, EventArgs e)
        {
            FTPAccount src = ucFTPAccounts.AccountsList.Items[ucFTPAccounts.AccountsList.SelectedIndex] as FTPAccount;
            Config.FTPAccountList.Add(src.Clone());
            ucFTPAccounts.AccountsList.SelectedIndex = ucFTPAccounts.AccountsList.Items.Count - 1;
            FTPSetup(Config.FTPAccountList);
        }

        private void FTPAccountsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sel = ucFTPAccounts.AccountsList.SelectedIndex;

            if (Config.FTPAccountList.IsValidIndex(sel))
            {
                FTPAccount acc = Config.FTPAccountList[sel];
                ucFTPAccounts.SettingsGrid.SelectedObject = acc;
            }
        }

        private void FtpAccountSettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FTPSetup(Config.FTPAccountList);
        }

        #endregion FTP

        #region Twitter

        private void TwitterAccountAuthButton_Click(object sender, EventArgs e)
        {
            if (CheckTwitterAccounts())
            {
                OAuthInfo acc = TwitterGetActiveAccount();
                Twitter twitter = new Twitter(acc);
                string url = twitter.GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    Config.TwitterOAuthInfoList[Config.TwitterSelectedAccount] = acc;
                    Helpers.LoadBrowserAsync(url);
                    ucTwitterAccounts.SettingsGrid.SelectedObject = acc;
                }
            }
        }

        private void TwitterAccountRemoveButton_Click(object sender, EventArgs e)
        {
            int sel = ucTwitterAccounts.AccountsList.SelectedIndex;
            if (ucTwitterAccounts.RemoveItem(sel))
            {
                Config.TwitterOAuthInfoList.RemoveAt(sel);
            }
        }

        private void TwitterAccountList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sel = ucTwitterAccounts.AccountsList.SelectedIndex;
            Config.TwitterSelectedAccount = sel;

            if (CheckTwitterAccounts())
            {
                OAuthInfo acc = Config.TwitterOAuthInfoList[sel];
                ucTwitterAccounts.SettingsGrid.SelectedObject = acc;
            }
        }

        private void TwitterAccountAddButton_Click(object sender, EventArgs e)
        {
            OAuthInfo acc = new OAuthInfo(APIKeys.TwitterConsumerKey, APIKeys.TwitterConsumerSecret);
            Config.TwitterOAuthInfoList.Add(acc);
            ucTwitterAccounts.AccountsList.Items.Add(acc);
            ucTwitterAccounts.AccountsList.SelectedIndex = ucTwitterAccounts.AccountsList.Items.Count - 1;
            if (CheckTwitterAccounts())
            {
                ucTwitterAccounts.SettingsGrid.SelectedObject = acc;
            }
        }

        #endregion Twitter
    }
}