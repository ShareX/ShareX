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

        public UploadersAPIKeys APIKeys { get; private set; }

        public UploadersConfigForm(UploadersConfig uploadersConfig, UploadersAPIKeys uploadersAPIKeys)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Config = uploadersConfig;
            ControlSettings();
            CreateUserControlEvents();
            LoadSettings(uploadersConfig);
            APIKeys = uploadersAPIKeys;
            Text = "ShareX - Outputs Configuration" + (string.IsNullOrEmpty(uploadersConfig.FilePath) ? string.Empty : ": " + uploadersConfig.FilePath);
        }

        private void UploadersConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void UploadersConfigForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        #region Image Uploaders

        #region ImageShack

        private void atcImageShackAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.ImageShackAccountType = accountType;
        }

        private void txtImageShackRegistrationCode_TextChanged(object sender, EventArgs e)
        {
            Config.ImageShackRegistrationCode = txtImageShackRegistrationCode.Text;
        }

        private void txtImageShackUsername_TextChanged(object sender, EventArgs e)
        {
            Config.ImageShackUsername = txtImageShackUsername.Text;
        }

        private void cbImageShackIsPublic_CheckedChanged(object sender, EventArgs e)
        {
            Config.ImageShackShowImagesInPublic = cbImageShackIsPublic.Checked;
        }

        private void btnImageShackOpenRegistrationCode_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync("http://profile.imageshack.us/prefs/");
        }

        private void btnImageShackOpenPublicProfile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Config.ImageShackUsername))
            {
                Helpers.LoadBrowserAsync("http://profile.imageshack.us/user/" + Config.ImageShackUsername);
            }
            else
            {
                txtImageShackUsername.Focus();
            }
        }

        private void btnImageShackOpenMyImages_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync("http://my.imageshack.us/v_images.php");
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
                        txtTinyPicRegistrationCode.Text = registrationCode;
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }
            }
        }

        private void cbTinyPicRememberUsernamePassword_CheckedChanged(object sender, EventArgs e)
        {
            Config.TinyPicRememberUserPass = cbTinyPicRememberUsernamePassword.Checked;

            if (Config.TinyPicRememberUserPass)
            {
                Config.TinyPicUsername = txtTinyPicUsername.Text;
                Config.TinyPicPassword = txtTinyPicPassword.Text;
            }
            else
            {
                Config.TinyPicUsername = string.Empty;
                Config.TinyPicPassword = string.Empty;
            }
        }

        private void btnTinyPicOpenMyImages_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync("http://tinypic.com/yourstuff.php");
        }

        #endregion TinyPic

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

        #region TwitPic

        private void cboTwitPicThumbnailMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.TwitPicThumbnailMode = (TwitPicThumbnailType)cboTwitPicThumbnailMode.SelectedIndex;
        }

        private void chkTwitPicShowFull_CheckedChanged(object sender, EventArgs e)
        {
            Config.TwitPicShowFull = chkTwitPicShowFull.Checked;
        }

        #endregion TwitPic

        #region YFrog

        private void txtYFrogUsername_TextChanged(object sender, EventArgs e)
        {
            Config.YFrogUsername = txtYFrogUsername.Text;
        }

        private void txtYFrogPassword_TextChanged(object sender, EventArgs e)
        {
            Config.YFrogPassword = txtYFrogPassword.Text;
        }

        #endregion YFrog

        #endregion Image Uploaders

        #region File Uploaders

        #region Dropbox

        private void pbDropboxLogo_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync("https://www.dropbox.com");
        }

        private void btnDropboxRegister_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync("http://db.tt/CtPYXvu");
        }

        private void btnDropboxAuthOpen_Click(object sender, EventArgs e)
        {
            DropboxAuthOpen();
        }

        private void btnDropboxAuthComplete_Click(object sender, EventArgs e)
        {
            DropboxAuthComplete();
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
            cbDropboxShortURL.Enabled = Config.DropboxAutoCreateShareableLink;
        }

        private void cbDropboxShortURL_CheckedChanged(object sender, EventArgs e)
        {
            Config.DropboxShortURL = cbDropboxShortURL.Checked;
        }

        #endregion Dropbox

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

        #endregion Google Drive

        #region Box

        private void btnBoxOpenAuthorize_Click(object sender, EventArgs e)
        {
            BoxAuthOpen();
        }

        private void btnBoxCompleteAuth_Click(object sender, EventArgs e)
        {
            BoxAuthComplete();
        }

        private void txtBoxFolderID_TextChanged(object sender, EventArgs e)
        {
            Config.BoxFolderID = txtBoxFolderID.Text;
        }

        private void btnBoxRefreshFolders_Click(object sender, EventArgs e)
        {
            BoxListFolders();
        }

        private void tvBoxFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is BoxFolder)
            {
                BoxFolder folderInfo = (BoxFolder)e.Node.Tag;
                txtBoxFolderID.Text = folderInfo.ID;
            }
        }

        #endregion Box

        #region Minus

        private bool HasFolder(string name)
        {
            return cboMinusFolders.Items.Cast<MinusFolder>().Any(mf => mf.name == name);
        }

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
            if (!string.IsNullOrEmpty(cboMinusFolders.Text) && !HasFolder(cboMinusFolders.Text))
            {
                Minus minus = new Minus(Config.MinusConfig, new OAuthInfo(APIKeys.MinusConsumerKey, APIKeys.MinusConsumerSecret));
                MinusFolder dir = minus.CreateFolder(cboMinusFolders.Text, chkMinusPublic.Checked);
                if (dir != null)
                {
                    cboMinusFolders.Items.Add(dir);
                }
            }
        }

        private void btnMinusFolderRemove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboMinusFolders.Text) && HasFolder(cboMinusFolders.Text))
            {
                Minus minus = new Minus(Config.MinusConfig, new OAuthInfo(APIKeys.MinusConsumerKey, APIKeys.MinusConsumerSecret));

                int id = cboMinusFolders.SelectedIndex;

                if (minus.DeleteFolder(id))
                {
                    cboMinusFolders.Items.RemoveAt(id);
                }
            }
        }

        private void btnMinusReadFolderList_Click(object sender, EventArgs e)
        {
            if (Config.MinusConfig != null)
            {
                btnMinusReadFolderList.Enabled = false;

                List<MinusFolder> tempListMf = new Minus(Config.MinusConfig,
                    new OAuthInfo(APIKeys.MinusConsumerKey, APIKeys.MinusConsumerSecret)).ReadFolderList(MinusScope.read_all);

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

        private void btnFtpHelp_Click(object sender, EventArgs e)
        {
            //Helpers.LoadBrowserAsync(Links.URL_WIKI_FTPAccounts);
        }

        private void btnFtpClient_Click(object sender, EventArgs e)
        {
            FTPOpenClient();
        }

        private void btnFTPImport_Click(object sender, EventArgs e)
        {
            FTPAccountsImport();
        }

        private void btnFTPExport_Click(object sender, EventArgs e)
        {
            FTPAccountsExport();
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

        #region Custom Uploader

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

            if (Config.CustomImageUploaderSelected > -1)
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.Image, Config.CustomUploadersList[Config.CustomImageUploaderSelected]);
            }
        }

        private void btnCustomUploaderTextUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomTextUploaderSelected > -1)
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.Text, Config.CustomUploadersList[Config.CustomTextUploaderSelected]);
            }
        }

        private void btnCustomUploaderFileUploaderTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomFileUploaderSelected > -1)
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.File, Config.CustomUploadersList[Config.CustomFileUploaderSelected]);
            }
        }

        private void btnCustomUploaderURLShortenerTest_Click(object sender, EventArgs e)
        {
            UpdateCustomUploader();

            if (Config.CustomURLShortenerSelected > -1)
            {
                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                    btnCustomUploaderFileUploaderTest.Enabled = btnCustomUploaderURLShortenerTest.Enabled = false;

                TestCustomUploader(CustomUploaderType.URL, Config.CustomUploadersList[Config.CustomURLShortenerSelected]);
            }
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
            Helpers.LoadBrowserAsync(e.LinkText);
        }

        #endregion Custom Uploader

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

            atcMegaAccountType.AccountTypeChanged -= atcMegaAccountType_AccountTypeChanged;
            atcMegaAccountType.SelectedAccountType = Config.MegaAnonymousLogin ? AccountType.Anonymous : AccountType.User;
            atcMegaAccountType.AccountTypeChanged += atcMegaAccountType_AccountTypeChanged;

            pnlMegaLogin.Enabled = !Config.MegaAnonymousLogin;

            if (Config.MegaAuthInfos != null)
            {
                txtMegaEmail.Text = Config.MegaAuthInfos.Email;
            }

            if (Config.MegaAnonymousLogin)
            {
                lblMegaStatus.Text = "Configured (anonymous)";
                lblMegaStatus.ForeColor = OkColor;
            }
            else if (Config.MegaAuthInfos == null)
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
                        cbMegaFolder.Items.Add("[Click refresh folders]");
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

        private void atcMegaAccountType_AccountTypeChanged(AccountType accountType)
        {
            Config.MegaAnonymousLogin = accountType == AccountType.Anonymous;
            Config.MegaAuthInfos = null;
            Config.MegaParentNodeId = null;
            txtMegaEmail.Text = null;
            txtMegaPassword.Text = null;
            cbMegaFolder.SelectedIndex = -1;
            MegaConfigureTab(true);
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
            Helpers.LoadBrowserAsync("https://mega.co.nz/#register");
        }

        private void btnMegaRefreshFolders_Click(object sender, EventArgs e)
        {
            MegaConfigureTab(true);
        }

        #endregion Mega

        #endregion File Uploaders

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

        #endregion Text Uploaders

        #region URL Shorteners

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

        #endregion URL Shorteners

        #region Other Services

        private void btnTwitterLogin_Click(object sender, EventArgs e)
        {
            TwitterLogin();
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
    }
}