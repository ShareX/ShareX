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
using ShareX.UploadersLib.Properties;
using ShareX.UploadersLib.TextUploaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class UploadersConfigForm
    {
        #region Imgur

        private void ImgurRefreshAlbumList()
        {
            try
            {
                lvImgurAlbumList.Items.Clear();

                if (OAuth2Info.CheckOAuth(Config.ImgurOAuth2Info))
                {
                    Config.ImgurAlbumList = new Imgur(Config.ImgurOAuth2Info).GetAlbums();
                    ImgurFillAlbumList();
                    lvImgurAlbumList.Focus();
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        private void ImgurFillAlbumList()
        {
            if (Config.ImgurAlbumList != null)
            {
                foreach (ImgurAlbumData album in Config.ImgurAlbumList)
                {
                    ListViewItem lvi = new ListViewItem(album.id ?? "");
                    lvi.SubItems.Add(album.title ?? "");
                    lvi.SubItems.Add(album.description ?? "");
                    lvi.Selected = Config.ImgurSelectedAlbum != null && !string.IsNullOrEmpty(Config.ImgurSelectedAlbum.id) &&
                        album.id.Equals(Config.ImgurSelectedAlbum.id, StringComparison.OrdinalIgnoreCase);
                    lvi.Tag = album;
                    lvImgurAlbumList.Items.Add(lvi);
                }
            }
        }

        #endregion Imgur

        #region Flickr

        private void FlickrAuthOpen()
        {
            try
            {
                OAuthInfo oauth = new OAuthInfo(APIKeys.FlickrKey, APIKeys.FlickrSecret);

                string url = new FlickrUploader(oauth).GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    Config.FlickrOAuthInfo = oauth;
                    URLHelpers.OpenURL(url);
                    DebugHelper.WriteLine("FlickrAuthOpen - Authorization URL is opened: " + url);
                }
                else
                {
                    DebugHelper.WriteLine("FlickrAuthOpen - Authorization URL is empty.");
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        private void FlickrAuthComplete(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code) && Config.FlickrOAuthInfo != null)
                {
                    bool result = new FlickrUploader(Config.FlickrOAuthInfo).GetAccessToken(code);

                    if (result)
                    {
                        oauthFlickr.Status = OAuthLoginStatus.LoginSuccessful;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        oauthFlickr.Status = OAuthLoginStatus.LoginFailed;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        #endregion Flickr

        #region Photobucket

        public void PhotobucketAuthOpen()
        {
            try
            {
                OAuthInfo oauth = new OAuthInfo(APIKeys.PhotobucketConsumerKey, APIKeys.PhotobucketConsumerSecret);

                string url = new Photobucket(oauth).GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    Config.PhotobucketOAuthInfo = oauth;
                    URLHelpers.OpenURL(url);
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        public void PhotobucketAuthComplete()
        {
            try
            {
                string verification = txtPhotobucketVerificationCode.Text;

                if (!string.IsNullOrEmpty(verification) && Config.PhotobucketOAuthInfo != null &&
                    !string.IsNullOrEmpty(Config.PhotobucketOAuthInfo.AuthToken) && !string.IsNullOrEmpty(Config.PhotobucketOAuthInfo.AuthSecret))
                {
                    Photobucket pb = new Photobucket(Config.PhotobucketOAuthInfo);
                    bool result = pb.GetAccessToken(verification);

                    if (result)
                    {
                        Config.PhotobucketAccountInfo = pb.GetAccountInfo();
                        lblPhotobucketAccountStatus.Text = Resources.UploadersConfigForm_Login_successful;
                        txtPhotobucketDefaultAlbumName.Text = Config.PhotobucketAccountInfo.AlbumID;
                        Config.PhotobucketAccountInfo.AlbumList.Add(Config.PhotobucketAccountInfo.AlbumID);
                        cbPhotobucketAlbumPaths.Items.Add(Config.PhotobucketAccountInfo.AlbumID);
                        cbPhotobucketAlbumPaths.SelectedIndex = 0;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblPhotobucketAccountStatus.Text = Resources.UploadersConfigForm_Login_failed;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        public void PhotobucketCreateAlbum()
        {
            Photobucket pb = new Photobucket(Config.PhotobucketOAuthInfo, Config.PhotobucketAccountInfo);
            if (pb.CreateAlbum(txtPhotobucketParentAlbumPath.Text, txtPhotobucketNewAlbumName.Text))
            {
                string albumPath = txtPhotobucketParentAlbumPath.Text + "/" + txtPhotobucketNewAlbumName.Text;
                Config.PhotobucketAccountInfo.AlbumList.Add(albumPath);
                cbPhotobucketAlbumPaths.Items.Add(albumPath);
                MessageBox.Show(string.Format(Resources.UploadersConfigForm_PhotobucketCreateAlbum__0__successfully_created_, albumPath), "ShareX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion Photobucket

        #region Amazon S3

        private void UpdateAmazonS3Status()
        {
            AmazonS3 s3 = new AmazonS3(Config.AmazonS3Settings);

            lblAmazonS3PathPreview.Text = s3.GetPreviewURL();
        }

        #endregion Amazon S3

        #region Google Cloud Storage

        private void UpdateGoogleCloudStorageStatus()
        {
            GoogleCloudStorage gcs = new GoogleCloudStorage(Config.GoogleCloudStorageOAuth2Info)
            {
                Bucket = Config.GoogleCloudStorageBucket,
                Domain = Config.GoogleCloudStorageDomain,
                Prefix = Config.GoogleCloudStorageObjectPrefix,
                RemoveExtensionImage = Config.GoogleCloudStorageRemoveExtensionImage,
                RemoveExtensionText = Config.GoogleCloudStorageRemoveExtensionText,
                RemoveExtensionVideo = Config.GoogleCloudStorageRemoveExtensionVideo,
                SetPublicACL = Config.GoogleCloudStorageSetPublicACL
            };

            lblGoogleCloudStoragePathPreview.Text = gcs.GetPreviewURL();
        }

        #endregion Google Cloud Storage

        #region Azure Storage

        private void UpdateAzureStorageStatus()
        {
            AzureStorage azure = new AzureStorage(Config.AzureStorageAccountName, Config.AzureStorageAccountAccessKey, Config.AzureStorageContainer,
                Config.AzureStorageEnvironment, Config.AzureStorageCustomDomain, Config.AzureStorageUploadPath, Config.AzureStorageCacheControl);

            lblAzureStorageURLPreview.Text = azure.GetPreviewURL();
        }

        #endregion Azure Storage

        #region Backblaze B2

        private void B2UpdateCustomDomainPreview()
        {
            string uploadPath = NameParser.Parse(NameParserType.FilePath, Config.B2UploadPath);
            string url;

            if (cbB2CustomUrl.Checked)
            {
                url = URLHelpers.CombineURL(Config.B2CustomUrl, uploadPath, "example.png");
                url = URLHelpers.FixPrefix(url);
            }
            else
            {
                string bucket = string.IsNullOrEmpty(Config.B2BucketName) ? "[bucket]" : URLHelpers.URLEncode(Config.B2BucketName);
                url = URLHelpers.CombineURL("https://f001.backblazeb2.com/file", bucket, uploadPath, "example.png");
            }

            lblB2UrlPreview.Text = url;
        }

        #endregion Backblaze B2

        #region Google Drive

        private void GoogleDriveRefreshFolders()
        {
            try
            {
                lvGoogleDriveFoldersList.Items.Clear();

                if (OAuth2Info.CheckOAuth(Config.GoogleDriveOAuth2Info))
                {
                    List<GoogleDriveFile> folders = new GoogleDrive(Config.GoogleDriveOAuth2Info).GetFolders(Config.GoogleDriveSelectedDrive.id);

                    if (folders != null)
                    {
                        foreach (GoogleDriveFile folder in folders)
                        {
                            ListViewItem lvi = new ListViewItem(folder.name);
                            lvi.SubItems.Add(folder.description);
                            lvi.Tag = folder;
                            lvGoogleDriveFoldersList.Items.Add(lvi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        private void GoogleDriveRefreshDrives()
        {
            try
            {
                if (OAuth2Info.CheckOAuth(Config.GoogleDriveOAuth2Info))
                {
                    List<GoogleDriveSharedDrive> drives = new GoogleDrive(Config.GoogleDriveOAuth2Info).GetDrives();

                    if (drives != null)
                    {
                        cbGoogleDriveSharedDrive.Items.Clear();
                        cbGoogleDriveSharedDrive.Items.Add(GoogleDrive.MyDrive);

                        foreach (GoogleDriveSharedDrive drive in drives)
                        {
                            cbGoogleDriveSharedDrive.Items.Add(drive);
                        }
                        GoogleDriveSelectConfigDrive();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        private void GoogleDriveSelectConfigDrive()
        {
            string driveID = Config.GoogleDriveSelectedDrive?.id;
            cbGoogleDriveSharedDrive.SelectedItem = cbGoogleDriveSharedDrive.Items
                .OfType<GoogleDriveSharedDrive>()
                .Where(x => x.id == driveID)
                .FirstOrDefault();
        }

        #endregion Google Drive

        #region Box

        public void BoxListFolders()
        {
            lvBoxFolders.Items.Clear();
            BoxAddFolder(Box.RootFolder);
            BoxListFolders(Box.RootFolder);
        }

        public void BoxListFolders(BoxFileEntry fileEntry)
        {
            if (!OAuth2Info.CheckOAuth(Config.BoxOAuth2Info))
            {
                MessageBox.Show(Resources.UploadersConfigForm_ListFolders_Authentication_required_, Resources.UploadersConfigForm_BoxListFolders_Box_refresh_folders_list_failed,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Box box = new Box(Config.BoxOAuth2Info);
                BoxFileInfo files = box.GetFiles(fileEntry);
                if (files != null && files.entries != null && files.entries.Length > 0)
                {
                    foreach (BoxFileEntry folder in files.entries.Where(x => x.type == "folder"))
                    {
                        BoxAddFolder(folder);
                    }
                }
            }
        }

        private void BoxAddFolder(BoxFileEntry folder)
        {
            ListViewItem lvi = new ListViewItem(folder.name);
            lvi.Tag = folder;
            lvBoxFolders.Items.Add(lvi);
        }

        #endregion Box

        #region OneDrive

        public void OneDriveListFolders(OneDriveFileInfo fileEntry, TreeNode tnParent)
        {
            Application.DoEvents();
            OneDrive oneDrive = new OneDrive(Config.OneDriveV2OAuth2Info);
            OneDriveFileList oneDrivePathInfo = oneDrive.GetPathInfo(fileEntry.id);
            tnParent.Nodes.Clear();
            foreach (OneDriveFileInfo folder in oneDrivePathInfo.value)
            {
                OneDriveAddFolder(folder, tnParent);
            }
        }

        private void OneDriveAddFolder(OneDriveFileInfo folder, TreeNode tnParent)
        {
            TreeNode tn = new TreeNode(folder.name);
            tn.Tag = folder;
            tn.Nodes.Add(new TreeNode(Resources.UploadersConfigForm_OneDriveAddFolder_Querying_folders___));

            if (tnParent != null)
            {
                tnParent.Nodes.Add(tn);
            }
            else
            {
                tvOneDrive.Nodes.Add(tn);
            }
        }

        #endregion OneDrive

        #region FTP

        private bool FTPCheckAccount(int index)
        {
            return Config.FTPAccountList.IsValidIndex(index);
        }

        private FTPAccount FTPGetSelectedAccount()
        {
            int index = cbFTPAccounts.SelectedIndex;

            if (FTPCheckAccount(index))
            {
                return Config.FTPAccountList[index];
            }

            return null;
        }

        private void FTPAddAccount(FTPAccount account)
        {
            if (account != null)
            {
                Config.FTPAccountList.Add(account);
                cbFTPAccounts.Items.Add(account);
                FTPUpdateControls();
            }
        }

        private void FTPUpdateControls()
        {
            int selected = cbFTPAccounts.SelectedIndex;

            cbFTPAccounts.Items.Clear();
            cbFTPImage.Items.Clear();
            cbFTPText.Items.Clear();
            cbFTPFile.Items.Clear();

            if (Config.FTPAccountList.Count > 0)
            {
                foreach (FTPAccount account in Config.FTPAccountList)
                {
                    cbFTPAccounts.Items.Add(account);
                    cbFTPImage.Items.Add(account);
                    cbFTPText.Items.Add(account);
                    cbFTPFile.Items.Add(account);
                }

                cbFTPAccounts.SelectedIndex = selected.Clamp(0, Config.FTPAccountList.Count - 1);
                cbFTPImage.SelectedIndex = Config.FTPSelectedImage.Clamp(0, Config.FTPAccountList.Count - 1);
                cbFTPText.SelectedIndex = Config.FTPSelectedText.Clamp(0, Config.FTPAccountList.Count - 1);
                cbFTPFile.SelectedIndex = Config.FTPSelectedFile.Clamp(0, Config.FTPAccountList.Count - 1);
            }

            FTPUpdateEnabledStates();
        }

        private void FTPUpdateEnabledStates()
        {
            cbFTPImage.Enabled = cbFTPText.Enabled = cbFTPFile.Enabled = cbFTPAccounts.Enabled = cbFTPAccounts.Items.Count > 0;
            btnFTPRemove.Enabled = btnFTPDuplicate.Enabled = btnFTPTest.Enabled = gbFTPAccount.Enabled = cbFTPAccounts.SelectedIndex > -1;

            FTPAccount account = FTPGetSelectedAccount();

            if (account != null)
            {
                gbFTPS.Visible = account.Protocol == FTPProtocol.FTPS;
                gbSFTP.Visible = account.Protocol == FTPProtocol.SFTP;
                pFTPTransferMode.Enabled = account.Protocol == FTPProtocol.FTP || account.Protocol == FTPProtocol.FTPS;
            }
            else
            {
                gbFTPS.Visible = gbSFTP.Visible = false;
            }
        }

        private void FTPRefreshNames()
        {
            cbFTPAccounts.RefreshItems();
            cbFTPImage.RefreshItems();
            cbFTPText.RefreshItems();
            cbFTPFile.RefreshItems();
        }

        private void FTPLoadSelectedAccount()
        {
            FTPAccount account = FTPGetSelectedAccount();

            if (account != null)
            {
                FTPLoadAccount(account);
            }
        }

        private void FTPLoadAccount(FTPAccount account)
        {
            txtFTPName.Text = account.Name;

            switch (account.Protocol)
            {
                case FTPProtocol.FTP:
                    rbFTPProtocolFTP.Checked = true;
                    break;
                case FTPProtocol.FTPS:
                    rbFTPProtocolFTPS.Checked = true;
                    break;
                case FTPProtocol.SFTP:
                    rbFTPProtocolSFTP.Checked = true;
                    break;
            }

            txtFTPHost.Text = account.Host;
            nudFTPPort.SetValue(account.Port);
            txtFTPUsername.Text = account.Username;
            txtFTPPassword.Text = account.Password;

            if (account.IsActive)
            {
                rbFTPTransferModeActive.Checked = true;
            }
            else
            {
                rbFTPTransferModePassive.Checked = true;
            }

            txtFTPRemoteDirectory.Text = account.SubFolderPath;
            cbFTPURLPathProtocol.SelectedIndex = (int)account.BrowserProtocol;
            txtFTPURLPath.Text = account.HttpHomePath;
            cbFTPAppendRemoteDirectory.Checked = account.HttpHomePathAutoAddSubFolderPath;
            cbFTPRemoveFileExtension.Checked = account.HttpHomePathNoExtension;
            lblFTPURLPreviewValue.Text = account.PreviewHttpPath;

            cbFTPSEncryption.SelectedIndex = (int)account.FTPSEncryption;
            txtFTPSCertificateLocation.Text = account.FTPSCertificateLocation;

            txtSFTPKeyLocation.Text = account.Keypath;
            txtSFTPKeyPassphrase.Text = account.Passphrase;

            FTPUpdateEnabledStates();
        }

        private void FTPClearFields()
        {
            FTPAccount account = new FTPAccount()
            {
                Name = "",
                HttpHomePathAutoAddSubFolderPath = false
            };

            FTPLoadAccount(account);
        }

        private void FTPUpdateURLPreview()
        {
            FTPAccount account = FTPGetSelectedAccount();

            if (account != null)
            {
                lblFTPURLPreviewValue.Text = account.PreviewHttpPath;
            }
        }

        private async Task FTPTestAccountAsync(FTPAccount account)
        {
            if (account != null)
            {
                btnFTPTest.Enabled = false;

                await Task.Run(() =>
                {
                    FTPTestAccount(account);
                });

                btnFTPTest.Enabled = true;
            }
        }

        private void FTPTestAccount(FTPAccount account)
        {
            string msg = "";
            string remotePath = account.GetSubFolderPath();
            List<string> directories = new List<string>();

            try
            {
                if (account.Protocol == FTPProtocol.FTP || account.Protocol == FTPProtocol.FTPS)
                {
                    using (FTP ftp = new FTP(account))
                    {
                        if (ftp.Connect())
                        {
                            if (!ftp.DirectoryExists(remotePath))
                            {
                                directories = ftp.CreateMultiDirectory(remotePath);
                            }

                            if (ftp.IsConnected)
                            {
                                if (directories.Count > 0)
                                {
                                    msg = Resources.UploadersConfigForm_TestFTPAccount_Connected_Created_folders + "\r\n" + string.Join("\r\n", directories);
                                }
                                else
                                {
                                    msg = Resources.UploadersConfigForm_TestFTPAccount_Connected_;
                                }
                            }
                        }
                    }
                }
                else if (account.Protocol == FTPProtocol.SFTP)
                {
                    using (SFTP sftp = new SFTP(account))
                    {
                        if (sftp.Connect())
                        {
                            if (!sftp.DirectoryExists(remotePath))
                            {
                                directories = sftp.CreateMultiDirectory(remotePath);
                            }

                            if (sftp.IsConnected)
                            {
                                if (directories.Count > 0)
                                {
                                    msg = Resources.UploadersConfigForm_TestFTPAccount_Connected_Created_folders + "\r\n" + string.Join("\r\n", directories);
                                }
                                else
                                {
                                    msg = Resources.UploadersConfigForm_TestFTPAccount_Connected_;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            MessageBox.Show(msg, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion FTP

        #region SendSpace

        public UserPassBox SendSpaceRegister()
        {
            UserPassBox upb = new UserPassBox(Resources.UploadersConfigForm_SendSpaceRegister_SendSpace_Registration___, "John Doe", "john.doe@gmail.com", "JohnDoe", "");

            if (upb.ShowDialog() == DialogResult.OK)
            {
                SendSpace sendSpace = new SendSpace(APIKeys.SendSpaceKey);
                upb.Success = sendSpace.AuthRegister(upb.UserName, upb.FullName, upb.Email, upb.Password);

                if (!upb.Success && sendSpace.Errors.Count > 0)
                {
                    MessageBox.Show(sendSpace.ToErrorString(), Resources.UploadersConfigForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return upb;
        }

        #endregion SendSpace

        #region Pastebin

        public void PastebinLogin()
        {
            if (Config.PastebinSettings != null)
            {
                try
                {
                    Pastebin pastebin = new Pastebin(APIKeys.PastebinKey, Config.PastebinSettings);

                    if (pastebin.Login())
                    {
                        UpdatePastebinStatus();
                        MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        UpdatePastebinStatus();
                        MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        public void UpdatePastebinStatus()
        {
            if (Config.PastebinSettings == null || string.IsNullOrEmpty(Config.PastebinSettings.UserKey))
            {
                lblPastebinLoginStatus.Text = Resources.UploadersConfigForm_UpdatePastebinStatus_NotLoggedIn;
            }
            else
            {
                lblPastebinLoginStatus.Text = Resources.UploadersConfigForm_UpdatePastebinStatus_LoggedIn;
            }
        }

        #endregion Pastebin

        #region Pushbullet

        public void PushbulletGetDevices()
        {
            cbPushbulletDevices.Items.Clear();
            cbPushbulletDevices.ResetText();

            Pushbullet pushbullet = new Pushbullet(Config.PushbulletSettings);
            Config.PushbulletSettings.DeviceList = pushbullet.GetDeviceList();

            if (Config.PushbulletSettings.DeviceList.Count > 0)
            {
                Config.PushbulletSettings.SelectedDevice = 0;

                cbPushbulletDevices.Enabled = true;

                Config.PushbulletSettings.DeviceList.ForEach(pbDevice =>
                {
                    if (!string.IsNullOrEmpty(pbDevice.Name))
                    {
                        cbPushbulletDevices.Items.Add(pbDevice.Name);
                    }
                });

                cbPushbulletDevices.SelectedIndex = 0;
            }
        }

        #endregion Pushbullet

        #region Twitter

        private OAuthInfo GetSelectedTwitterAccount()
        {
            return Config.TwitterOAuthInfoList.ReturnIfValidIndex(Config.TwitterSelectedAccount);
        }

        private bool CheckTwitterAccounts()
        {
            return Config.TwitterOAuthInfoList.IsValidIndex(Config.TwitterSelectedAccount);
        }

        private bool TwitterUpdateSelected()
        {
            Config.TwitterSelectedAccount = lbTwitterAccounts.SelectedIndex;

            if (Config.TwitterSelectedAccount > -1)
            {
                OAuthInfo oauth = Config.TwitterOAuthInfoList[Config.TwitterSelectedAccount];

                if (oauth != null)
                {
                    txtTwitterDescription.Enabled = true;
                    txtTwitterDescription.Text = oauth.Description;
                    oauthTwitter.Enabled = true;

                    if (OAuthInfo.CheckOAuth(oauth))
                    {
                        oauthTwitter.Status = OAuthLoginStatus.LoginSuccessful;
                    }
                    else
                    {
                        oauthTwitter.Status = OAuthLoginStatus.LoginRequired;
                    }

                    return true;
                }
            }

            txtTwitterDescription.Enabled = false;
            txtTwitterDescription.Text = "";
            oauthTwitter.Enabled = false;
            return false;
        }

        private void TwitterAuthOpen()
        {
            if (CheckTwitterAccounts())
            {
                try
                {
                    OAuthInfo oauth = new OAuthInfo(APIKeys.TwitterConsumerKey, APIKeys.TwitterConsumerSecret);

                    string url = new Twitter(oauth).GetAuthorizationURL();

                    if (!string.IsNullOrEmpty(url))
                    {
                        oauth.Description = Config.TwitterOAuthInfoList[Config.TwitterSelectedAccount].Description;
                        Config.TwitterOAuthInfoList[Config.TwitterSelectedAccount] = oauth;
                        URLHelpers.OpenURL(url);
                        DebugHelper.WriteLine("TwitterAuthOpen - Authorization URL is opened: " + url);
                    }
                    else
                    {
                        DebugHelper.WriteLine("TwitterAuthOpen - Authorization URL is empty.");
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        private void TwitterAuthComplete(string code)
        {
            if (CheckTwitterAccounts())
            {
                try
                {
                    OAuthInfo oauth = GetSelectedTwitterAccount();

                    if (oauth != null && !string.IsNullOrEmpty(oauth.AuthToken) && !string.IsNullOrEmpty(oauth.AuthSecret))
                    {
                        bool result = new Twitter(oauth).GetAccessToken(code);

                        if (result)
                        {
                            oauth.AuthVerifier = "";
                            oauthTwitter.Status = OAuthLoginStatus.LoginSuccessful;
                            MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            oauthTwitter.Status = OAuthLoginStatus.LoginFailed;
                            MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        private void TwitterAuthClear()
        {
            if (CheckTwitterAccounts())
            {
                OAuthInfo oauth = new OAuthInfo();

                OAuthInfo oauth2 = GetSelectedTwitterAccount();

                if (oauth2 != null)
                {
                    oauth.Description = oauth2.Description;
                }

                Config.TwitterOAuthInfoList[Config.TwitterSelectedAccount] = oauth;
            }
        }

        #endregion Twitter

        #region Jira

        public void JiraAuthOpen()
        {
            try
            {
                OAuthInfo oauth = new OAuthInfo(APIKeys.JiraConsumerKey);
                oauth.SignatureMethod = OAuthInfo.OAuthInfoSignatureMethod.RSA_SHA1;
                oauth.ConsumerPrivateKey = Jira.PrivateKey;

                string url = new Jira(Config.JiraHost, oauth).GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    Config.JiraOAuthInfo = oauth;
                    URLHelpers.OpenURL(url);
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        public void JiraAuthComplete(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code) && Config.JiraOAuthInfo != null && !string.IsNullOrEmpty(Config.JiraOAuthInfo.AuthToken) && !string.IsNullOrEmpty(Config.JiraOAuthInfo.AuthSecret))
                {
                    Jira jira = new Jira(Config.JiraHost, Config.JiraOAuthInfo);
                    bool result = jira.GetAccessToken(code);

                    if (result)
                    {
                        oAuthJira.Status = OAuthLoginStatus.LoginSuccessful;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        oAuthJira.Status = OAuthLoginStatus.LoginFailed;
                        MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        #endregion Jira

        #region Shared folder

        private void SharedFolderUpdateControls()
        {
            int selected = lbSharedFolderAccounts.SelectedIndex;

            lbSharedFolderAccounts.Items.Clear();
            cbSharedFolderImages.Items.Clear();
            cbSharedFolderText.Items.Clear();
            cbSharedFolderFiles.Items.Clear();

            if (Config.LocalhostAccountList.Count > 0)
            {
                foreach (LocalhostAccount account in Config.LocalhostAccountList)
                {
                    lbSharedFolderAccounts.Items.Add(account);
                    cbSharedFolderImages.Items.Add(account);
                    cbSharedFolderText.Items.Add(account);
                    cbSharedFolderFiles.Items.Add(account);
                }

                lbSharedFolderAccounts.SelectedIndex = selected.Clamp(0, Config.LocalhostAccountList.Count - 1);
                cbSharedFolderImages.SelectedIndex = Config.LocalhostSelectedImages.Clamp(0, Config.LocalhostAccountList.Count - 1);
                cbSharedFolderText.SelectedIndex = Config.LocalhostSelectedText.Clamp(0, Config.LocalhostAccountList.Count - 1);
                cbSharedFolderFiles.SelectedIndex = Config.LocalhostSelectedFiles.Clamp(0, Config.LocalhostAccountList.Count - 1);
            }

            SharedFolderUpdateEnabledStates();
        }

        private void SharedFolderUpdateEnabledStates()
        {
            cbSharedFolderImages.Enabled = cbSharedFolderText.Enabled = cbSharedFolderFiles.Enabled = Config.LocalhostAccountList.Count > 0;
            btnSharedFolderRemove.Enabled = btnSharedFolderDuplicate.Enabled = lbSharedFolderAccounts.SelectedIndex > -1;
        }

        private void SharedFolderAddItem(LocalhostAccount account)
        {
            Config.LocalhostAccountList.Add(account);
            lbSharedFolderAccounts.Items.Add(account);
            lbSharedFolderAccounts.SelectedIndex = lbSharedFolderAccounts.Items.Count - 1;
            SharedFolderUpdateControls();
        }

        private bool SharedFolderRemoveItem(int index)
        {
            if (index.IsBetween(0, lbSharedFolderAccounts.Items.Count - 1))
            {
                Config.LocalhostAccountList.RemoveAt(index);
                lbSharedFolderAccounts.Items.RemoveAt(index);

                if (lbSharedFolderAccounts.Items.Count > 0)
                {
                    lbSharedFolderAccounts.SelectedIndex = index == lbSharedFolderAccounts.Items.Count ? lbSharedFolderAccounts.Items.Count - 1 : index;
                    pgSharedFolderAccount.SelectedObject = lbSharedFolderAccounts.Items[lbSharedFolderAccounts.SelectedIndex];
                }
                else
                {
                    pgSharedFolderAccount.SelectedObject = null;
                }

                SharedFolderUpdateControls();

                return true;
            }

            return false;
        }

        #endregion Shared folder

        #region Generic OAuth2

        private OAuth2Info OAuth2Open(IOAuth2Basic uploader)
        {
            try
            {
                string url = uploader.GetAuthorizationURL();

                if (!string.IsNullOrEmpty(url))
                {
                    URLHelpers.OpenURL(url);
                    DebugHelper.WriteLine(uploader.ToString() + " - Authorization URL is opened: " + url);
                    return uploader.AuthInfo;
                }
                else
                {
                    DebugHelper.WriteLine(uploader.ToString() + " - Authorization URL is empty.");
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }

            return null;
        }

        private bool OAuth2Complete(IOAuth2Basic uploader, string code, OAuthControl control)
        {
            try
            {
                if (!string.IsNullOrEmpty(code) && uploader.AuthInfo != null)
                {
                    bool result = uploader.GetAccessToken(code);
                    ConfigureOAuthStatus(control, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }

            return false;
        }

        private bool OAuth2Refresh(IOAuth2 uploader, OAuthControl oauth2)
        {
            try
            {
                if (OAuth2Info.CheckOAuth(uploader.AuthInfo))
                {
                    bool result = uploader.RefreshAccessToken();
                    ConfigureOAuthStatus(oauth2, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }

            return false;
        }

        private void ConfigureOAuthStatus(OAuthControl oauth2, bool result)
        {
            if (result)
            {
                oauth2.Status = OAuthLoginStatus.LoginSuccessful;
                MessageBox.Show(Resources.UploadersConfigForm_Login_successful, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                oauth2.Status = OAuthLoginStatus.LoginFailed;
                MessageBox.Show(Resources.UploadersConfigForm_Login_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Generic OAuth2
    }
}