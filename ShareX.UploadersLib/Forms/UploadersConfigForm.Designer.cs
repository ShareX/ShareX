#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2021 ShareX Team

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
namespace ShareX.UploadersLib
{
    partial class UploadersConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadersConfigForm));
            txtRapidSharePremiumUserName = new System.Windows.Forms.TextBox();
            ttHelpTip = new System.Windows.Forms.ToolTip(components);
            cbAmazonS3CustomCNAME = new System.Windows.Forms.CheckBox();
            txtB2CustomUrl = new System.Windows.Forms.TextBox();
            cbB2CustomUrl = new System.Windows.Forms.CheckBox();
            txtB2Bucket = new System.Windows.Forms.TextBox();
            txtB2UploadPath = new System.Windows.Forms.TextBox();
            txtB2ApplicationKey = new System.Windows.Forms.TextBox();
            txtB2ApplicationKeyId = new System.Windows.Forms.TextBox();
            tpURLShorteners = new System.Windows.Forms.TabPage();
            tcURLShorteners = new System.Windows.Forms.TabControl();
            tpBitly = new System.Windows.Forms.TabPage();
            txtBitlyDomain = new System.Windows.Forms.TextBox();
            lblBitlyDomain = new System.Windows.Forms.Label();
            oauth2Bitly = new OAuthControl();
            tpYourls = new System.Windows.Forms.TabPage();
            txtYourlsPassword = new System.Windows.Forms.TextBox();
            txtYourlsUsername = new System.Windows.Forms.TextBox();
            txtYourlsSignature = new System.Windows.Forms.TextBox();
            lblYourlsNote = new System.Windows.Forms.Label();
            lblYourlsPassword = new System.Windows.Forms.Label();
            lblYourlsUsername = new System.Windows.Forms.Label();
            lblYourlsSignature = new System.Windows.Forms.Label();
            txtYourlsAPIURL = new System.Windows.Forms.TextBox();
            lblYourlsAPIURL = new System.Windows.Forms.Label();
            tpPolr = new System.Windows.Forms.TabPage();
            cbPolrUseAPIv1 = new System.Windows.Forms.CheckBox();
            cbPolrIsSecret = new System.Windows.Forms.CheckBox();
            txtPolrAPIKey = new System.Windows.Forms.TextBox();
            lblPolrAPIKey = new System.Windows.Forms.Label();
            txtPolrAPIHostname = new System.Windows.Forms.TextBox();
            lblPolrAPIHostname = new System.Windows.Forms.Label();
            tpFirebaseDynamicLinks = new System.Windows.Forms.TabPage();
            lblFirebaseDomainExample = new System.Windows.Forms.Label();
            lblFirebaseDomain = new System.Windows.Forms.Label();
            cbFirebaseIsShort = new System.Windows.Forms.CheckBox();
            txtFirebaseDomain = new System.Windows.Forms.TextBox();
            txtFirebaseWebAPIKey = new System.Windows.Forms.TextBox();
            lblFirebaseWebAPIKey = new System.Windows.Forms.Label();
            tpKutt = new System.Windows.Forms.TabPage();
            txtKuttDomain = new System.Windows.Forms.TextBox();
            lblKuttDomain = new System.Windows.Forms.Label();
            lblKuttPassword = new System.Windows.Forms.Label();
            txtKuttPassword = new System.Windows.Forms.TextBox();
            cbKuttReuse = new System.Windows.Forms.CheckBox();
            txtKuttAPIKey = new System.Windows.Forms.TextBox();
            txtKuttHost = new System.Windows.Forms.TextBox();
            lblKuttAPIKey = new System.Windows.Forms.Label();
            lblKuttHost = new System.Windows.Forms.Label();
            tpZeroWidthShortener = new System.Windows.Forms.TabPage();
            txtZWSToken = new System.Windows.Forms.TextBox();
            txtZWSURL = new System.Windows.Forms.TextBox();
            lblZWSToken = new System.Windows.Forms.Label();
            lblZWSURL = new System.Windows.Forms.Label();
            tpFileUploaders = new System.Windows.Forms.TabPage();
            tcFileUploaders = new System.Windows.Forms.TabControl();
            tpFTP = new System.Windows.Forms.TabPage();
            gbFTPAccount = new System.Windows.Forms.GroupBox();
            gbSFTP = new System.Windows.Forms.GroupBox();
            txtSFTPKeyPassphrase = new System.Windows.Forms.TextBox();
            btnSFTPKeyLocationBrowse = new System.Windows.Forms.Button();
            lblSFTPKeyPassphrase = new System.Windows.Forms.Label();
            txtSFTPKeyLocation = new System.Windows.Forms.TextBox();
            lblSFTPKeyLocation = new System.Windows.Forms.Label();
            cbFTPAppendRemoteDirectory = new System.Windows.Forms.CheckBox();
            lblFTPProtocol = new System.Windows.Forms.Label();
            lblFTPName = new System.Windows.Forms.Label();
            cbFTPRemoveFileExtension = new System.Windows.Forms.CheckBox();
            txtFTPName = new System.Windows.Forms.TextBox();
            lblFTPHost = new System.Windows.Forms.Label();
            pFTPTransferMode = new System.Windows.Forms.Panel();
            rbFTPTransferModeActive = new System.Windows.Forms.RadioButton();
            rbFTPTransferModePassive = new System.Windows.Forms.RadioButton();
            txtFTPHost = new System.Windows.Forms.TextBox();
            pFTPProtocol = new System.Windows.Forms.Panel();
            rbFTPProtocolFTP = new System.Windows.Forms.RadioButton();
            rbFTPProtocolFTPS = new System.Windows.Forms.RadioButton();
            rbFTPProtocolSFTP = new System.Windows.Forms.RadioButton();
            lblFTPPort = new System.Windows.Forms.Label();
            lblFTPTransferMode = new System.Windows.Forms.Label();
            nudFTPPort = new System.Windows.Forms.NumericUpDown();
            lblFTPURLPreviewValue = new System.Windows.Forms.Label();
            lblFTPUsername = new System.Windows.Forms.Label();
            lblFTPURLPreview = new System.Windows.Forms.Label();
            txtFTPUsername = new System.Windows.Forms.TextBox();
            cbFTPURLPathProtocol = new System.Windows.Forms.ComboBox();
            lblFTPPassword = new System.Windows.Forms.Label();
            txtFTPURLPath = new System.Windows.Forms.TextBox();
            txtFTPPassword = new System.Windows.Forms.TextBox();
            lblFTPURLPath = new System.Windows.Forms.Label();
            lblFTPRemoteDirectory = new System.Windows.Forms.Label();
            txtFTPRemoteDirectory = new System.Windows.Forms.TextBox();
            gbFTPS = new System.Windows.Forms.GroupBox();
            btnFTPSCertificateLocationBrowse = new System.Windows.Forms.Button();
            txtFTPSCertificateLocation = new System.Windows.Forms.TextBox();
            lblFTPSCertificateLocation = new System.Windows.Forms.Label();
            cbFTPSEncryption = new System.Windows.Forms.ComboBox();
            lblFTPSEncryption = new System.Windows.Forms.Label();
            btnFTPDuplicate = new System.Windows.Forms.Button();
            btnFTPTest = new System.Windows.Forms.Button();
            btnFTPRemove = new System.Windows.Forms.Button();
            btnFTPAdd = new System.Windows.Forms.Button();
            cbFTPAccounts = new System.Windows.Forms.ComboBox();
            lblFTPAccounts = new System.Windows.Forms.Label();
            lblFTPFile = new System.Windows.Forms.Label();
            lblFTPText = new System.Windows.Forms.Label();
            eiFTP = new ShareX.HelpersLib.ExportImportControl();
            lblFTPImage = new System.Windows.Forms.Label();
            cbFTPImage = new System.Windows.Forms.ComboBox();
            cbFTPFile = new System.Windows.Forms.ComboBox();
            cbFTPText = new System.Windows.Forms.ComboBox();
            tpDropbox = new System.Windows.Forms.TabPage();
            cbDropboxUseDirectLink = new System.Windows.Forms.CheckBox();
            cbDropboxAutoCreateShareableLink = new System.Windows.Forms.CheckBox();
            lblDropboxPath = new System.Windows.Forms.Label();
            txtDropboxPath = new System.Windows.Forms.TextBox();
            oauth2Dropbox = new OAuthControl();
            tpOneDrive = new System.Windows.Forms.TabPage();
            tvOneDrive = new System.Windows.Forms.TreeView();
            lblOneDriveFolderID = new System.Windows.Forms.Label();
            cbOneDriveCreateShareableLink = new System.Windows.Forms.CheckBox();
            cbOneDriveUseDirectLink = new System.Windows.Forms.CheckBox();
            oAuth2OneDrive = new OAuthControl();
            tpGoogleDrive = new System.Windows.Forms.TabPage();
            btnGoogleDriveFolderIDHelp = new System.Windows.Forms.Button();
            cbGoogleDriveSharedDrive = new System.Windows.Forms.ComboBox();
            cbGoogleDriveDirectLink = new System.Windows.Forms.CheckBox();
            cbGoogleDriveUseFolder = new System.Windows.Forms.CheckBox();
            txtGoogleDriveFolderID = new System.Windows.Forms.TextBox();
            lblGoogleDriveFolderID = new System.Windows.Forms.Label();
            oauth2GoogleDrive = new OAuthLoopbackControl();
            lvGoogleDriveFoldersList = new ShareX.HelpersLib.MyListView();
            chGoogleDriveTitle = new System.Windows.Forms.ColumnHeader();
            chGoogleDriveDescription = new System.Windows.Forms.ColumnHeader();
            btnGoogleDriveRefreshFolders = new System.Windows.Forms.Button();
            cbGoogleDriveIsPublic = new System.Windows.Forms.CheckBox();
            tpPuush = new System.Windows.Forms.TabPage();
            lblPuushAPIKey = new System.Windows.Forms.Label();
            txtPuushAPIKey = new System.Windows.Forms.TextBox();
            llPuushForgottenPassword = new System.Windows.Forms.LinkLabel();
            btnPuushLogin = new System.Windows.Forms.Button();
            txtPuushPassword = new System.Windows.Forms.TextBox();
            txtPuushEmail = new System.Windows.Forms.TextBox();
            lblPuushEmail = new System.Windows.Forms.Label();
            lblPuushPassword = new System.Windows.Forms.Label();
            tpBox = new System.Windows.Forms.TabPage();
            lblBoxFolderTip = new System.Windows.Forms.Label();
            cbBoxShare = new System.Windows.Forms.CheckBox();
            cbBoxShareAccessLevel = new System.Windows.Forms.ComboBox();
            lblBoxShareAccessLevel = new System.Windows.Forms.Label();
            lvBoxFolders = new ShareX.HelpersLib.MyListView();
            chBoxFoldersName = new System.Windows.Forms.ColumnHeader();
            lblBoxFolderID = new System.Windows.Forms.Label();
            btnBoxRefreshFolders = new System.Windows.Forms.Button();
            oauth2Box = new OAuthControl();
            tpAmazonS3 = new System.Windows.Forms.TabPage();
            gbAmazonS3Advanced = new System.Windows.Forms.GroupBox();
            cbAmazonS3SignedPayload = new System.Windows.Forms.CheckBox();
            lblAmazonS3StripExtension = new System.Windows.Forms.Label();
            cbAmazonS3StripExtensionText = new System.Windows.Forms.CheckBox();
            cbAmazonS3StorageClass = new System.Windows.Forms.ComboBox();
            cbAmazonS3StripExtensionVideo = new System.Windows.Forms.CheckBox();
            cbAmazonS3PublicACL = new System.Windows.Forms.CheckBox();
            cbAmazonS3StripExtensionImage = new System.Windows.Forms.CheckBox();
            cbAmazonS3UsePathStyle = new System.Windows.Forms.CheckBox();
            btnAmazonS3StorageClassHelp = new System.Windows.Forms.Button();
            lblAmazonS3StorageClass = new System.Windows.Forms.Label();
            lblAmazonS3Endpoint = new System.Windows.Forms.Label();
            txtAmazonS3Endpoint = new System.Windows.Forms.TextBox();
            lblAmazonS3Region = new System.Windows.Forms.Label();
            txtAmazonS3Region = new System.Windows.Forms.TextBox();
            txtAmazonS3CustomDomain = new System.Windows.Forms.TextBox();
            lblAmazonS3PathPreviewLabel = new System.Windows.Forms.Label();
            lblAmazonS3PathPreview = new System.Windows.Forms.Label();
            btnAmazonS3BucketNameOpen = new System.Windows.Forms.Button();
            btnAmazonS3AccessKeyOpen = new System.Windows.Forms.Button();
            cbAmazonS3Endpoints = new System.Windows.Forms.ComboBox();
            lblAmazonS3BucketName = new System.Windows.Forms.Label();
            txtAmazonS3BucketName = new System.Windows.Forms.TextBox();
            lblAmazonS3Endpoints = new System.Windows.Forms.Label();
            txtAmazonS3ObjectPrefix = new System.Windows.Forms.TextBox();
            lblAmazonS3ObjectPrefix = new System.Windows.Forms.Label();
            txtAmazonS3SecretKey = new System.Windows.Forms.TextBox();
            lblAmazonS3SecretKey = new System.Windows.Forms.Label();
            lblAmazonS3AccessKey = new System.Windows.Forms.Label();
            txtAmazonS3AccessKey = new System.Windows.Forms.TextBox();
            tpGoogleCloudStorage = new System.Windows.Forms.TabPage();
            oauth2GoogleCloudStorage = new OAuthLoopbackControl();
            gbGoogleCloudStorageAdvanced = new System.Windows.Forms.GroupBox();
            lblGoogleCloudStorageStripExtension = new System.Windows.Forms.Label();
            cbGoogleCloudStorageStripExtensionText = new System.Windows.Forms.CheckBox();
            cbGoogleCloudStorageStripExtensionVideo = new System.Windows.Forms.CheckBox();
            cbGoogleCloudStorageSetPublicACL = new System.Windows.Forms.CheckBox();
            cbGoogleCloudStorageStripExtensionImage = new System.Windows.Forms.CheckBox();
            lblGoogleCloudStoragePathPreview = new System.Windows.Forms.Label();
            lblGoogleCloudStoragePathPreviewLabel = new System.Windows.Forms.Label();
            txtGoogleCloudStorageObjectPrefix = new System.Windows.Forms.TextBox();
            lblGoogleCloudStorageObjectPrefix = new System.Windows.Forms.Label();
            lblGoogleCloudStorageDomain = new System.Windows.Forms.Label();
            txtGoogleCloudStorageDomain = new System.Windows.Forms.TextBox();
            lblGoogleCloudStorageBucket = new System.Windows.Forms.Label();
            txtGoogleCloudStorageBucket = new System.Windows.Forms.TextBox();
            tpAzureStorage = new System.Windows.Forms.TabPage();
            txtAzureStorageCacheControl = new System.Windows.Forms.TextBox();
            lblAzureStorageCacheControl = new System.Windows.Forms.Label();
            lblAzureStorageURLPreview = new System.Windows.Forms.Label();
            lblAzureStorageURLPreviewLabel = new System.Windows.Forms.Label();
            txtAzureStorageUploadPath = new System.Windows.Forms.TextBox();
            lblAzureStorageUploadPath = new System.Windows.Forms.Label();
            cbAzureStorageEnvironment = new System.Windows.Forms.ComboBox();
            lblAzureStorageEnvironment = new System.Windows.Forms.Label();
            btnAzureStoragePortal = new System.Windows.Forms.Button();
            txtAzureStorageContainer = new System.Windows.Forms.TextBox();
            lblAzureStorageContainer = new System.Windows.Forms.Label();
            txtAzureStorageAccessKey = new System.Windows.Forms.TextBox();
            lblAzureStorageAccessKey = new System.Windows.Forms.Label();
            txtAzureStorageAccountName = new System.Windows.Forms.TextBox();
            lblAzureStorageAccountName = new System.Windows.Forms.Label();
            txtAzureStorageCustomDomain = new System.Windows.Forms.TextBox();
            lblAzureStorageCustomDomain = new System.Windows.Forms.Label();
            tpBackblazeB2 = new System.Windows.Forms.TabPage();
            lblB2UrlPreview = new System.Windows.Forms.Label();
            lblB2ManageLink = new System.Windows.Forms.LinkLabel();
            lblB2UrlPreviewLabel = new System.Windows.Forms.Label();
            lblB2Bucket = new System.Windows.Forms.Label();
            lblB2UploadPath = new System.Windows.Forms.Label();
            lblB2ApplicationKey = new System.Windows.Forms.Label();
            lblB2ApplicationKeyId = new System.Windows.Forms.Label();
            tpMega = new System.Windows.Forms.TabPage();
            btnMegaRefreshFolders = new System.Windows.Forms.Button();
            lblMegaStatus = new System.Windows.Forms.Label();
            btnMegaRegister = new System.Windows.Forms.Button();
            lblMegaFolder = new System.Windows.Forms.Label();
            lblMegaStatusTitle = new System.Windows.Forms.Label();
            cbMegaFolder = new System.Windows.Forms.ComboBox();
            lblMegaEmail = new System.Windows.Forms.Label();
            btnMegaLogin = new System.Windows.Forms.Button();
            txtMegaEmail = new System.Windows.Forms.TextBox();
            txtMegaPassword = new System.Windows.Forms.TextBox();
            lblMegaPassword = new System.Windows.Forms.Label();
            tpOwnCloud = new System.Windows.Forms.TabPage();
            cbOwnCloudAppendFileNameToURL = new System.Windows.Forms.CheckBox();
            nudOwnCloudExpiryTime = new System.Windows.Forms.NumericUpDown();
            cbOwnCloudAutoExpire = new System.Windows.Forms.CheckBox();
            lblOwnCloudExpiryTime = new System.Windows.Forms.Label();
            cbOwnCloudUsePreviewLinks = new System.Windows.Forms.CheckBox();
            lblOwnCloudHostExample = new System.Windows.Forms.Label();
            cbOwnCloud81Compatibility = new System.Windows.Forms.CheckBox();
            cbOwnCloudDirectLink = new System.Windows.Forms.CheckBox();
            cbOwnCloudCreateShare = new System.Windows.Forms.CheckBox();
            txtOwnCloudPath = new System.Windows.Forms.TextBox();
            txtOwnCloudPassword = new System.Windows.Forms.TextBox();
            txtOwnCloudUsername = new System.Windows.Forms.TextBox();
            txtOwnCloudHost = new System.Windows.Forms.TextBox();
            lblOwnCloudPath = new System.Windows.Forms.Label();
            lblOwnCloudPassword = new System.Windows.Forms.Label();
            lblOwnCloudUsername = new System.Windows.Forms.Label();
            lblOwnCloudHost = new System.Windows.Forms.Label();
            tpMediaFire = new System.Windows.Forms.TabPage();
            cbMediaFireUseLongLink = new System.Windows.Forms.CheckBox();
            txtMediaFirePath = new System.Windows.Forms.TextBox();
            lblMediaFirePath = new System.Windows.Forms.Label();
            txtMediaFirePassword = new System.Windows.Forms.TextBox();
            txtMediaFireEmail = new System.Windows.Forms.TextBox();
            lblMediaFirePassword = new System.Windows.Forms.Label();
            lblMediaFireEmail = new System.Windows.Forms.Label();
            tpPushbullet = new System.Windows.Forms.TabPage();
            lblPushbulletDevices = new System.Windows.Forms.Label();
            cbPushbulletDevices = new System.Windows.Forms.ComboBox();
            btnPushbulletGetDeviceList = new System.Windows.Forms.Button();
            lblPushbulletUserKey = new System.Windows.Forms.Label();
            txtPushbulletUserKey = new System.Windows.Forms.TextBox();
            tpSendSpace = new System.Windows.Forms.TabPage();
            btnSendSpaceRegister = new System.Windows.Forms.Button();
            lblSendSpacePassword = new System.Windows.Forms.Label();
            lblSendSpaceUsername = new System.Windows.Forms.Label();
            txtSendSpacePassword = new System.Windows.Forms.TextBox();
            txtSendSpaceUserName = new System.Windows.Forms.TextBox();
            atcSendSpaceAccountType = new AccountTypeControl();
            tpHostr = new System.Windows.Forms.TabPage();
            cbLocalhostrDirectURL = new System.Windows.Forms.CheckBox();
            lblLocalhostrPassword = new System.Windows.Forms.Label();
            lblLocalhostrEmail = new System.Windows.Forms.Label();
            txtLocalhostrPassword = new System.Windows.Forms.TextBox();
            txtLocalhostrEmail = new System.Windows.Forms.TextBox();
            tpLambda = new System.Windows.Forms.TabPage();
            lblLambdaInfo = new System.Windows.Forms.Label();
            lblLambdaApiKey = new System.Windows.Forms.Label();
            txtLambdaApiKey = new System.Windows.Forms.TextBox();
            lblLambdaUploadURL = new System.Windows.Forms.Label();
            cbLambdaUploadURL = new System.Windows.Forms.ComboBox();
            tpPomf = new System.Windows.Forms.TabPage();
            txtPomfResultURL = new System.Windows.Forms.TextBox();
            txtPomfUploadURL = new System.Windows.Forms.TextBox();
            lblPomfResultURL = new System.Windows.Forms.Label();
            lblPomfUploadURL = new System.Windows.Forms.Label();
            tpSeafile = new System.Windows.Forms.TabPage();
            cbSeafileAPIURL = new System.Windows.Forms.ComboBox();
            grpSeafileShareSettings = new System.Windows.Forms.GroupBox();
            txtSeafileSharePassword = new System.Windows.Forms.TextBox();
            lblSeafileSharePassword = new System.Windows.Forms.Label();
            nudSeafileExpireDays = new System.Windows.Forms.NumericUpDown();
            lblSeafileDaysToExpire = new System.Windows.Forms.Label();
            btnSeafileLibraryPasswordValidate = new System.Windows.Forms.Button();
            txtSeafileLibraryPassword = new System.Windows.Forms.TextBox();
            lblSeafileLibraryPassword = new System.Windows.Forms.Label();
            lvSeafileLibraries = new ShareX.HelpersLib.MyListView();
            colSeafileLibraryName = new System.Windows.Forms.ColumnHeader();
            colSeafileLibrarySize = new System.Windows.Forms.ColumnHeader();
            colSeafileLibraryEncrypted = new System.Windows.Forms.ColumnHeader();
            btnSeafilePathValidate = new System.Windows.Forms.Button();
            txtSeafileDirectoryPath = new System.Windows.Forms.TextBox();
            lblSeafileWritePermNotif = new System.Windows.Forms.Label();
            lblSeafilePath = new System.Windows.Forms.Label();
            txtSeafileUploadLocationRefresh = new System.Windows.Forms.Button();
            lblSeafileSelectLibrary = new System.Windows.Forms.Label();
            grpSeafileAccInfo = new System.Windows.Forms.GroupBox();
            btnRefreshSeafileAccInfo = new System.Windows.Forms.Button();
            txtSeafileAccInfoUsage = new System.Windows.Forms.TextBox();
            txtSeafileAccInfoEmail = new System.Windows.Forms.TextBox();
            lblSeafileAccInfoEmail = new System.Windows.Forms.Label();
            lblSeafileAccInfoUsage = new System.Windows.Forms.Label();
            btnSeafileCheckAuthToken = new System.Windows.Forms.Button();
            btnSeafileCheckAPIURL = new System.Windows.Forms.Button();
            grpSeafileObtainAuthToken = new System.Windows.Forms.GroupBox();
            btnSeafileGetAuthToken = new System.Windows.Forms.Button();
            txtSeafilePassword = new System.Windows.Forms.TextBox();
            txtSeafileUsername = new System.Windows.Forms.TextBox();
            lblSeafileUsername = new System.Windows.Forms.Label();
            lblSeafilePassword = new System.Windows.Forms.Label();
            cbSeafileCreateShareableURL = new System.Windows.Forms.CheckBox();
            cbSeafileCreateShareableURLRaw = new System.Windows.Forms.CheckBox();
            txtSeafileAuthToken = new System.Windows.Forms.TextBox();
            lblSeafileAuthToken = new System.Windows.Forms.Label();
            lblSeafileAPIURL = new System.Windows.Forms.Label();
            tpStreamable = new System.Windows.Forms.TabPage();
            cbStreamableUseDirectURL = new System.Windows.Forms.CheckBox();
            txtStreamablePassword = new System.Windows.Forms.TextBox();
            txtStreamableUsername = new System.Windows.Forms.TextBox();
            lblStreamableUsername = new System.Windows.Forms.Label();
            lblStreamablePassword = new System.Windows.Forms.Label();
            tpSul = new System.Windows.Forms.TabPage();
            btnSulGetAPIKey = new System.Windows.Forms.Button();
            txtSulAPIKey = new System.Windows.Forms.TextBox();
            lblSulAPIKey = new System.Windows.Forms.Label();
            tpLithiio = new System.Windows.Forms.TabPage();
            btnLithiioFetchAPIKey = new System.Windows.Forms.Button();
            txtLithiioPassword = new System.Windows.Forms.TextBox();
            txtLithiioEmail = new System.Windows.Forms.TextBox();
            lblLithiioPassword = new System.Windows.Forms.Label();
            lblLithiioEmail = new System.Windows.Forms.Label();
            btnLithiioGetAPIKey = new System.Windows.Forms.Button();
            lblLithiioApiKey = new System.Windows.Forms.Label();
            txtLithiioApiKey = new System.Windows.Forms.TextBox();
            tpPlik = new System.Windows.Forms.TabPage();
            gbPlikSettings = new System.Windows.Forms.GroupBox();
            cbPlikOneShot = new System.Windows.Forms.CheckBox();
            txtPlikComment = new System.Windows.Forms.TextBox();
            cbPlikComment = new System.Windows.Forms.CheckBox();
            cbPlikRemovable = new System.Windows.Forms.CheckBox();
            gbPlikLoginCredentials = new System.Windows.Forms.GroupBox();
            nudPlikTTL = new System.Windows.Forms.NumericUpDown();
            cbPlikTTLUnit = new System.Windows.Forms.ComboBox();
            lblPlikTTL = new System.Windows.Forms.Label();
            txtPlikURL = new System.Windows.Forms.TextBox();
            lblPlikURL = new System.Windows.Forms.Label();
            cbPlikIsSecured = new System.Windows.Forms.CheckBox();
            lblPlikAPIKey = new System.Windows.Forms.Label();
            txtPlikAPIKey = new System.Windows.Forms.TextBox();
            lblPlikPassword = new System.Windows.Forms.Label();
            lblPlikUsername = new System.Windows.Forms.Label();
            txtPlikPassword = new System.Windows.Forms.TextBox();
            txtPlikLogin = new System.Windows.Forms.TextBox();
            tpYouTube = new System.Windows.Forms.TabPage();
            oauth2YouTube = new OAuthLoopbackControl();
            llYouTubePermissionsLink = new System.Windows.Forms.LinkLabel();
            lblYouTubePermissionsTip = new System.Windows.Forms.Label();
            cbYouTubeShowDialog = new System.Windows.Forms.CheckBox();
            cbYouTubeUseShortenedLink = new System.Windows.Forms.CheckBox();
            cbYouTubePrivacyType = new System.Windows.Forms.ComboBox();
            lblYouTubePrivacyType = new System.Windows.Forms.Label();
            tpSharedFolder = new System.Windows.Forms.TabPage();
            lbSharedFolderAccounts = new System.Windows.Forms.ListBox();
            pgSharedFolderAccount = new System.Windows.Forms.PropertyGrid();
            btnSharedFolderDuplicate = new System.Windows.Forms.Button();
            btnSharedFolderRemove = new System.Windows.Forms.Button();
            btnSharedFolderAdd = new System.Windows.Forms.Button();
            lblSharedFolderFiles = new System.Windows.Forms.Label();
            lblSharedFolderText = new System.Windows.Forms.Label();
            cbSharedFolderFiles = new System.Windows.Forms.ComboBox();
            lblSharedFolderImages = new System.Windows.Forms.Label();
            cbSharedFolderText = new System.Windows.Forms.ComboBox();
            cbSharedFolderImages = new System.Windows.Forms.ComboBox();
            tpEmail = new System.Windows.Forms.TabPage();
            txtEmailAutomaticSendTo = new System.Windows.Forms.TextBox();
            cbEmailAutomaticSend = new System.Windows.Forms.CheckBox();
            lblEmailSmtpServer = new System.Windows.Forms.Label();
            lblEmailPassword = new System.Windows.Forms.Label();
            cbEmailRememberLastTo = new System.Windows.Forms.CheckBox();
            txtEmailFrom = new System.Windows.Forms.TextBox();
            txtEmailPassword = new System.Windows.Forms.TextBox();
            txtEmailDefaultBody = new System.Windows.Forms.TextBox();
            lblEmailFrom = new System.Windows.Forms.Label();
            txtEmailSmtpServer = new System.Windows.Forms.TextBox();
            lblEmailDefaultSubject = new System.Windows.Forms.Label();
            lblEmailDefaultBody = new System.Windows.Forms.Label();
            nudEmailSmtpPort = new System.Windows.Forms.NumericUpDown();
            lblEmailSmtpPort = new System.Windows.Forms.Label();
            txtEmailDefaultSubject = new System.Windows.Forms.TextBox();
            btnCopyShowFiles = new System.Windows.Forms.Button();
            tpTextUploaders = new System.Windows.Forms.TabPage();
            tcTextUploaders = new System.Windows.Forms.TabControl();
            tpPastebin = new System.Windows.Forms.TabPage();
            cbPastebinRaw = new System.Windows.Forms.CheckBox();
            cbPastebinSyntax = new System.Windows.Forms.ComboBox();
            btnPastebinRegister = new System.Windows.Forms.Button();
            lblPastebinSyntax = new System.Windows.Forms.Label();
            lblPastebinExpiration = new System.Windows.Forms.Label();
            lblPastebinPrivacy = new System.Windows.Forms.Label();
            lblPastebinTitle = new System.Windows.Forms.Label();
            lblPastebinPassword = new System.Windows.Forms.Label();
            lblPastebinUsername = new System.Windows.Forms.Label();
            cbPastebinExpiration = new System.Windows.Forms.ComboBox();
            cbPastebinPrivacy = new System.Windows.Forms.ComboBox();
            txtPastebinTitle = new System.Windows.Forms.TextBox();
            txtPastebinPassword = new System.Windows.Forms.TextBox();
            txtPastebinUsername = new System.Windows.Forms.TextBox();
            lblPastebinLoginStatus = new System.Windows.Forms.Label();
            btnPastebinLogin = new System.Windows.Forms.Button();
            tpPaste_ee = new System.Windows.Forms.TabPage();
            btnPaste_eeGetUserKey = new System.Windows.Forms.Button();
            lblPaste_eeUserAPIKey = new System.Windows.Forms.Label();
            txtPaste_eeUserAPIKey = new System.Windows.Forms.TextBox();
            tpGist = new System.Windows.Forms.TabPage();
            lblGistCustomURLExample = new System.Windows.Forms.Label();
            lblGistOAuthInfo = new System.Windows.Forms.Label();
            lblGistCustomURL = new System.Windows.Forms.Label();
            txtGistCustomURL = new System.Windows.Forms.TextBox();
            cbGistUseRawURL = new System.Windows.Forms.CheckBox();
            cbGistPublishPublic = new System.Windows.Forms.CheckBox();
            oAuth2Gist = new OAuthControl();
            tpUpaste = new System.Windows.Forms.TabPage();
            cbUpasteIsPublic = new System.Windows.Forms.CheckBox();
            lblUpasteUserKey = new System.Windows.Forms.Label();
            txtUpasteUserKey = new System.Windows.Forms.TextBox();
            tpHastebin = new System.Windows.Forms.TabPage();
            cbHastebinUseFileExtension = new System.Windows.Forms.CheckBox();
            txtHastebinSyntaxHighlighting = new System.Windows.Forms.TextBox();
            txtHastebinCustomDomain = new System.Windows.Forms.TextBox();
            lblHastebinSyntaxHighlighting = new System.Windows.Forms.Label();
            lblHastebinCustomDomain = new System.Windows.Forms.Label();
            tpOneTimeSecret = new System.Windows.Forms.TabPage();
            lblOneTimeSecretAPIKey = new System.Windows.Forms.Label();
            lblOneTimeSecretEmail = new System.Windows.Forms.Label();
            txtOneTimeSecretAPIKey = new System.Windows.Forms.TextBox();
            txtOneTimeSecretEmail = new System.Windows.Forms.TextBox();
            tpPastie = new System.Windows.Forms.TabPage();
            cbPastieIsPublic = new System.Windows.Forms.CheckBox();
            tpImageUploaders = new System.Windows.Forms.TabPage();
            tcImageUploaders = new System.Windows.Forms.TabControl();
            tpImgur = new System.Windows.Forms.TabPage();
            cbImgurUseGIFV = new System.Windows.Forms.CheckBox();
            cbImgurUploadSelectedAlbum = new System.Windows.Forms.CheckBox();
            cbImgurDirectLink = new System.Windows.Forms.CheckBox();
            atcImgurAccountType = new AccountTypeControl();
            oauth2Imgur = new OAuthControl();
            lvImgurAlbumList = new ShareX.HelpersLib.MyListView();
            chImgurID = new System.Windows.Forms.ColumnHeader();
            chImgurTitle = new System.Windows.Forms.ColumnHeader();
            chImgurDescription = new System.Windows.Forms.ColumnHeader();
            btnImgurRefreshAlbumList = new System.Windows.Forms.Button();
            cbImgurThumbnailType = new System.Windows.Forms.ComboBox();
            lblImgurThumbnailType = new System.Windows.Forms.Label();
            tpImageShack = new System.Windows.Forms.TabPage();
            btnImageShackLogin = new System.Windows.Forms.Button();
            btnImageShackOpenPublicProfile = new System.Windows.Forms.Button();
            cbImageShackIsPublic = new System.Windows.Forms.CheckBox();
            btnImageShackOpenMyImages = new System.Windows.Forms.Button();
            lblImageShackUsername = new System.Windows.Forms.Label();
            txtImageShackUsername = new System.Windows.Forms.TextBox();
            txtImageShackPassword = new System.Windows.Forms.TextBox();
            lblImageShackPassword = new System.Windows.Forms.Label();
            tpFlickr = new System.Windows.Forms.TabPage();
            cbFlickrDirectLink = new System.Windows.Forms.CheckBox();
            oauthFlickr = new OAuthControl();
            tpPhotobucket = new System.Windows.Forms.TabPage();
            gbPhotobucketAlbumPath = new System.Windows.Forms.GroupBox();
            btnPhotobucketAddAlbum = new System.Windows.Forms.Button();
            btnPhotobucketRemoveAlbum = new System.Windows.Forms.Button();
            cbPhotobucketAlbumPaths = new System.Windows.Forms.ComboBox();
            gbPhotobucketAlbums = new System.Windows.Forms.GroupBox();
            lblPhotobucketNewAlbumName = new System.Windows.Forms.Label();
            lblPhotobucketParentAlbumPath = new System.Windows.Forms.Label();
            txtPhotobucketNewAlbumName = new System.Windows.Forms.TextBox();
            txtPhotobucketParentAlbumPath = new System.Windows.Forms.TextBox();
            btnPhotobucketCreateAlbum = new System.Windows.Forms.Button();
            gbPhotobucketUserAccount = new System.Windows.Forms.GroupBox();
            lblPhotobucketDefaultAlbumName = new System.Windows.Forms.Label();
            btnPhotobucketAuthOpen = new System.Windows.Forms.Button();
            txtPhotobucketDefaultAlbumName = new System.Windows.Forms.TextBox();
            lblPhotobucketVerificationCode = new System.Windows.Forms.Label();
            btnPhotobucketAuthComplete = new System.Windows.Forms.Button();
            txtPhotobucketVerificationCode = new System.Windows.Forms.TextBox();
            lblPhotobucketAccountStatus = new System.Windows.Forms.Label();
            tpChevereto = new System.Windows.Forms.TabPage();
            lblCheveretoUploadURLExample = new System.Windows.Forms.Label();
            cbCheveretoDirectURL = new System.Windows.Forms.CheckBox();
            lblCheveretoUploadURL = new System.Windows.Forms.Label();
            txtCheveretoUploadURL = new System.Windows.Forms.TextBox();
            txtCheveretoAPIKey = new System.Windows.Forms.TextBox();
            lblCheveretoAPIKey = new System.Windows.Forms.Label();
            tpVgyme = new System.Windows.Forms.TabPage();
            llVgymeAccountDetailsPage = new System.Windows.Forms.LinkLabel();
            txtVgymeUserKey = new System.Windows.Forms.TextBox();
            lvlVgymeUserKey = new System.Windows.Forms.Label();
            tcUploaders = new System.Windows.Forms.TabControl();
            tttvMain = new ShareX.HelpersLib.TabToTreeView();
            actRapidShareAccountType = new AccountTypeControl();
            tpURLShorteners.SuspendLayout();
            tcURLShorteners.SuspendLayout();
            tpBitly.SuspendLayout();
            tpYourls.SuspendLayout();
            tpPolr.SuspendLayout();
            tpFirebaseDynamicLinks.SuspendLayout();
            tpKutt.SuspendLayout();
            tpZeroWidthShortener.SuspendLayout();
            tpFileUploaders.SuspendLayout();
            tcFileUploaders.SuspendLayout();
            tpFTP.SuspendLayout();
            gbFTPAccount.SuspendLayout();
            gbSFTP.SuspendLayout();
            pFTPTransferMode.SuspendLayout();
            pFTPProtocol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudFTPPort).BeginInit();
            gbFTPS.SuspendLayout();
            tpDropbox.SuspendLayout();
            tpOneDrive.SuspendLayout();
            tpGoogleDrive.SuspendLayout();
            tpPuush.SuspendLayout();
            tpBox.SuspendLayout();
            tpAmazonS3.SuspendLayout();
            gbAmazonS3Advanced.SuspendLayout();
            tpGoogleCloudStorage.SuspendLayout();
            gbGoogleCloudStorageAdvanced.SuspendLayout();
            tpAzureStorage.SuspendLayout();
            tpBackblazeB2.SuspendLayout();
            tpMega.SuspendLayout();
            tpOwnCloud.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudOwnCloudExpiryTime).BeginInit();
            tpMediaFire.SuspendLayout();
            tpPushbullet.SuspendLayout();
            tpSendSpace.SuspendLayout();
            tpHostr.SuspendLayout();
            tpLambda.SuspendLayout();
            tpPomf.SuspendLayout();
            tpSeafile.SuspendLayout();
            grpSeafileShareSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudSeafileExpireDays).BeginInit();
            grpSeafileAccInfo.SuspendLayout();
            grpSeafileObtainAuthToken.SuspendLayout();
            tpStreamable.SuspendLayout();
            tpSul.SuspendLayout();
            tpLithiio.SuspendLayout();
            tpPlik.SuspendLayout();
            gbPlikSettings.SuspendLayout();
            gbPlikLoginCredentials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPlikTTL).BeginInit();
            tpYouTube.SuspendLayout();
            tpSharedFolder.SuspendLayout();
            tpEmail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudEmailSmtpPort).BeginInit();
            tpTextUploaders.SuspendLayout();
            tcTextUploaders.SuspendLayout();
            tpPastebin.SuspendLayout();
            tpPaste_ee.SuspendLayout();
            tpGist.SuspendLayout();
            tpUpaste.SuspendLayout();
            tpHastebin.SuspendLayout();
            tpOneTimeSecret.SuspendLayout();
            tpPastie.SuspendLayout();
            tpImageUploaders.SuspendLayout();
            tcImageUploaders.SuspendLayout();
            tpImgur.SuspendLayout();
            tpImageShack.SuspendLayout();
            tpFlickr.SuspendLayout();
            tpPhotobucket.SuspendLayout();
            gbPhotobucketAlbumPath.SuspendLayout();
            gbPhotobucketAlbums.SuspendLayout();
            gbPhotobucketUserAccount.SuspendLayout();
            tpChevereto.SuspendLayout();
            tpVgyme.SuspendLayout();
            tcUploaders.SuspendLayout();
            SuspendLayout();
            // 
            // txtRapidSharePremiumUserName
            // 
            resources.ApplyResources(txtRapidSharePremiumUserName, "txtRapidSharePremiumUserName");
            txtRapidSharePremiumUserName.Name = "txtRapidSharePremiumUserName";
            // 
            // ttHelpTip
            // 
            ttHelpTip.AutomaticDelay = 0;
            ttHelpTip.AutoPopDelay = 30000;
            ttHelpTip.BackColor = System.Drawing.SystemColors.Window;
            ttHelpTip.InitialDelay = 500;
            ttHelpTip.ReshowDelay = 100;
            ttHelpTip.UseAnimation = false;
            ttHelpTip.UseFading = false;
            // 
            // cbAmazonS3CustomCNAME
            // 
            resources.ApplyResources(cbAmazonS3CustomCNAME, "cbAmazonS3CustomCNAME");
            cbAmazonS3CustomCNAME.Name = "cbAmazonS3CustomCNAME";
            ttHelpTip.SetToolTip(cbAmazonS3CustomCNAME, resources.GetString("cbAmazonS3CustomCNAME.ToolTip"));
            cbAmazonS3CustomCNAME.UseVisualStyleBackColor = true;
            cbAmazonS3CustomCNAME.CheckedChanged += cbAmazonS3CustomCNAME_CheckedChanged;
            // 
            // txtB2CustomUrl
            // 
            resources.ApplyResources(txtB2CustomUrl, "txtB2CustomUrl");
            txtB2CustomUrl.Name = "txtB2CustomUrl";
            txtB2CustomUrl.TextChanged += txtB2CustomUrl_TextChanged;
            // 
            // cbB2CustomUrl
            // 
            resources.ApplyResources(cbB2CustomUrl, "cbB2CustomUrl");
            cbB2CustomUrl.Name = "cbB2CustomUrl";
            cbB2CustomUrl.UseVisualStyleBackColor = true;
            cbB2CustomUrl.CheckedChanged += cbB2CustomUrl_CheckedChanged;
            // 
            // txtB2Bucket
            // 
            resources.ApplyResources(txtB2Bucket, "txtB2Bucket");
            txtB2Bucket.Name = "txtB2Bucket";
            txtB2Bucket.TextChanged += txtB2Bucket_TextChanged;
            // 
            // txtB2UploadPath
            // 
            resources.ApplyResources(txtB2UploadPath, "txtB2UploadPath");
            txtB2UploadPath.Name = "txtB2UploadPath";
            txtB2UploadPath.TextChanged += txtB2UploadPath_TextChanged;
            // 
            // txtB2ApplicationKey
            // 
            resources.ApplyResources(txtB2ApplicationKey, "txtB2ApplicationKey");
            txtB2ApplicationKey.Name = "txtB2ApplicationKey";
            txtB2ApplicationKey.UseSystemPasswordChar = true;
            txtB2ApplicationKey.TextChanged += txtB2ApplicationKey_TextChanged;
            // 
            // txtB2ApplicationKeyId
            // 
            resources.ApplyResources(txtB2ApplicationKeyId, "txtB2ApplicationKeyId");
            txtB2ApplicationKeyId.Name = "txtB2ApplicationKeyId";
            txtB2ApplicationKeyId.TextChanged += txtB2ApplicationKeyId_TextChanged;
            // 
            // tpURLShorteners
            // 
            tpURLShorteners.BackColor = System.Drawing.SystemColors.Window;
            tpURLShorteners.Controls.Add(tcURLShorteners);
            resources.ApplyResources(tpURLShorteners, "tpURLShorteners");
            tpURLShorteners.Name = "tpURLShorteners";
            // 
            // tcURLShorteners
            // 
            tcURLShorteners.Controls.Add(tpBitly);
            tcURLShorteners.Controls.Add(tpYourls);
            tcURLShorteners.Controls.Add(tpPolr);
            tcURLShorteners.Controls.Add(tpFirebaseDynamicLinks);
            tcURLShorteners.Controls.Add(tpKutt);
            tcURLShorteners.Controls.Add(tpZeroWidthShortener);
            resources.ApplyResources(tcURLShorteners, "tcURLShorteners");
            tcURLShorteners.Name = "tcURLShorteners";
            tcURLShorteners.SelectedIndex = 0;
            // 
            // tpBitly
            // 
            tpBitly.BackColor = System.Drawing.SystemColors.Window;
            tpBitly.Controls.Add(txtBitlyDomain);
            tpBitly.Controls.Add(lblBitlyDomain);
            tpBitly.Controls.Add(oauth2Bitly);
            resources.ApplyResources(tpBitly, "tpBitly");
            tpBitly.Name = "tpBitly";
            // 
            // txtBitlyDomain
            // 
            resources.ApplyResources(txtBitlyDomain, "txtBitlyDomain");
            txtBitlyDomain.Name = "txtBitlyDomain";
            txtBitlyDomain.TextChanged += txtBitlyDomain_TextChanged;
            // 
            // lblBitlyDomain
            // 
            resources.ApplyResources(lblBitlyDomain, "lblBitlyDomain");
            lblBitlyDomain.Name = "lblBitlyDomain";
            // 
            // oauth2Bitly
            // 
            oauth2Bitly.IsRefreshable = false;
            resources.ApplyResources(oauth2Bitly, "oauth2Bitly");
            oauth2Bitly.Name = "oauth2Bitly";
            oauth2Bitly.UserInfo = null;
            oauth2Bitly.OpenButtonClicked += oauth2Bitly_OpenButtonClicked;
            oauth2Bitly.CompleteButtonClicked += oauth2Bitly_CompleteButtonClicked;
            oauth2Bitly.ClearButtonClicked += oauth2Bitly_ClearButtonClicked;
            // 
            // tpYourls
            // 
            tpYourls.BackColor = System.Drawing.SystemColors.Window;
            tpYourls.Controls.Add(txtYourlsPassword);
            tpYourls.Controls.Add(txtYourlsUsername);
            tpYourls.Controls.Add(txtYourlsSignature);
            tpYourls.Controls.Add(lblYourlsNote);
            tpYourls.Controls.Add(lblYourlsPassword);
            tpYourls.Controls.Add(lblYourlsUsername);
            tpYourls.Controls.Add(lblYourlsSignature);
            tpYourls.Controls.Add(txtYourlsAPIURL);
            tpYourls.Controls.Add(lblYourlsAPIURL);
            resources.ApplyResources(tpYourls, "tpYourls");
            tpYourls.Name = "tpYourls";
            // 
            // txtYourlsPassword
            // 
            resources.ApplyResources(txtYourlsPassword, "txtYourlsPassword");
            txtYourlsPassword.Name = "txtYourlsPassword";
            txtYourlsPassword.UseSystemPasswordChar = true;
            txtYourlsPassword.TextChanged += txtYourlsPassword_TextChanged;
            // 
            // txtYourlsUsername
            // 
            resources.ApplyResources(txtYourlsUsername, "txtYourlsUsername");
            txtYourlsUsername.Name = "txtYourlsUsername";
            txtYourlsUsername.TextChanged += txtYourlsUsername_TextChanged;
            // 
            // txtYourlsSignature
            // 
            resources.ApplyResources(txtYourlsSignature, "txtYourlsSignature");
            txtYourlsSignature.Name = "txtYourlsSignature";
            txtYourlsSignature.UseSystemPasswordChar = true;
            txtYourlsSignature.TextChanged += txtYourlsSignature_TextChanged;
            // 
            // lblYourlsNote
            // 
            resources.ApplyResources(lblYourlsNote, "lblYourlsNote");
            lblYourlsNote.Name = "lblYourlsNote";
            // 
            // lblYourlsPassword
            // 
            resources.ApplyResources(lblYourlsPassword, "lblYourlsPassword");
            lblYourlsPassword.Name = "lblYourlsPassword";
            // 
            // lblYourlsUsername
            // 
            resources.ApplyResources(lblYourlsUsername, "lblYourlsUsername");
            lblYourlsUsername.Name = "lblYourlsUsername";
            // 
            // lblYourlsSignature
            // 
            resources.ApplyResources(lblYourlsSignature, "lblYourlsSignature");
            lblYourlsSignature.Name = "lblYourlsSignature";
            // 
            // txtYourlsAPIURL
            // 
            resources.ApplyResources(txtYourlsAPIURL, "txtYourlsAPIURL");
            txtYourlsAPIURL.Name = "txtYourlsAPIURL";
            txtYourlsAPIURL.TextChanged += txtYourlsAPIURL_TextChanged;
            // 
            // lblYourlsAPIURL
            // 
            resources.ApplyResources(lblYourlsAPIURL, "lblYourlsAPIURL");
            lblYourlsAPIURL.Name = "lblYourlsAPIURL";
            // 
            // tpPolr
            // 
            tpPolr.BackColor = System.Drawing.SystemColors.Window;
            tpPolr.Controls.Add(cbPolrUseAPIv1);
            tpPolr.Controls.Add(cbPolrIsSecret);
            tpPolr.Controls.Add(txtPolrAPIKey);
            tpPolr.Controls.Add(lblPolrAPIKey);
            tpPolr.Controls.Add(txtPolrAPIHostname);
            tpPolr.Controls.Add(lblPolrAPIHostname);
            resources.ApplyResources(tpPolr, "tpPolr");
            tpPolr.Name = "tpPolr";
            // 
            // cbPolrUseAPIv1
            // 
            resources.ApplyResources(cbPolrUseAPIv1, "cbPolrUseAPIv1");
            cbPolrUseAPIv1.Name = "cbPolrUseAPIv1";
            cbPolrUseAPIv1.UseVisualStyleBackColor = true;
            cbPolrUseAPIv1.CheckedChanged += cbPolrUseAPIv1_CheckedChanged;
            // 
            // cbPolrIsSecret
            // 
            resources.ApplyResources(cbPolrIsSecret, "cbPolrIsSecret");
            cbPolrIsSecret.Name = "cbPolrIsSecret";
            cbPolrIsSecret.UseVisualStyleBackColor = true;
            cbPolrIsSecret.CheckedChanged += cbPolrIsSecret_CheckedChanged;
            // 
            // txtPolrAPIKey
            // 
            resources.ApplyResources(txtPolrAPIKey, "txtPolrAPIKey");
            txtPolrAPIKey.Name = "txtPolrAPIKey";
            txtPolrAPIKey.UseSystemPasswordChar = true;
            txtPolrAPIKey.TextChanged += txtPolrAPIKey_TextChanged;
            // 
            // lblPolrAPIKey
            // 
            resources.ApplyResources(lblPolrAPIKey, "lblPolrAPIKey");
            lblPolrAPIKey.Name = "lblPolrAPIKey";
            // 
            // txtPolrAPIHostname
            // 
            resources.ApplyResources(txtPolrAPIHostname, "txtPolrAPIHostname");
            txtPolrAPIHostname.Name = "txtPolrAPIHostname";
            txtPolrAPIHostname.TextChanged += txtPolrAPIHostname_TextChanged;
            // 
            // lblPolrAPIHostname
            // 
            resources.ApplyResources(lblPolrAPIHostname, "lblPolrAPIHostname");
            lblPolrAPIHostname.Name = "lblPolrAPIHostname";
            // 
            // tpFirebaseDynamicLinks
            // 
            tpFirebaseDynamicLinks.Controls.Add(lblFirebaseDomainExample);
            tpFirebaseDynamicLinks.Controls.Add(lblFirebaseDomain);
            tpFirebaseDynamicLinks.Controls.Add(cbFirebaseIsShort);
            tpFirebaseDynamicLinks.Controls.Add(txtFirebaseDomain);
            tpFirebaseDynamicLinks.Controls.Add(txtFirebaseWebAPIKey);
            tpFirebaseDynamicLinks.Controls.Add(lblFirebaseWebAPIKey);
            resources.ApplyResources(tpFirebaseDynamicLinks, "tpFirebaseDynamicLinks");
            tpFirebaseDynamicLinks.Name = "tpFirebaseDynamicLinks";
            tpFirebaseDynamicLinks.UseVisualStyleBackColor = true;
            // 
            // lblFirebaseDomainExample
            // 
            resources.ApplyResources(lblFirebaseDomainExample, "lblFirebaseDomainExample");
            lblFirebaseDomainExample.Name = "lblFirebaseDomainExample";
            // 
            // lblFirebaseDomain
            // 
            resources.ApplyResources(lblFirebaseDomain, "lblFirebaseDomain");
            lblFirebaseDomain.Name = "lblFirebaseDomain";
            // 
            // cbFirebaseIsShort
            // 
            resources.ApplyResources(cbFirebaseIsShort, "cbFirebaseIsShort");
            cbFirebaseIsShort.Name = "cbFirebaseIsShort";
            cbFirebaseIsShort.UseVisualStyleBackColor = true;
            cbFirebaseIsShort.CheckedChanged += cbFirebaseIsShort_CheckedChanged;
            // 
            // txtFirebaseDomain
            // 
            resources.ApplyResources(txtFirebaseDomain, "txtFirebaseDomain");
            txtFirebaseDomain.Name = "txtFirebaseDomain";
            txtFirebaseDomain.TextChanged += txtFirebaseDomain_TextChanged;
            // 
            // txtFirebaseWebAPIKey
            // 
            resources.ApplyResources(txtFirebaseWebAPIKey, "txtFirebaseWebAPIKey");
            txtFirebaseWebAPIKey.Name = "txtFirebaseWebAPIKey";
            txtFirebaseWebAPIKey.UseSystemPasswordChar = true;
            txtFirebaseWebAPIKey.TextChanged += txtFirebaseWebAPIKey_TextChanged;
            // 
            // lblFirebaseWebAPIKey
            // 
            resources.ApplyResources(lblFirebaseWebAPIKey, "lblFirebaseWebAPIKey");
            lblFirebaseWebAPIKey.Name = "lblFirebaseWebAPIKey";
            // 
            // tpKutt
            // 
            tpKutt.Controls.Add(txtKuttDomain);
            tpKutt.Controls.Add(lblKuttDomain);
            tpKutt.Controls.Add(lblKuttPassword);
            tpKutt.Controls.Add(txtKuttPassword);
            tpKutt.Controls.Add(cbKuttReuse);
            tpKutt.Controls.Add(txtKuttAPIKey);
            tpKutt.Controls.Add(txtKuttHost);
            tpKutt.Controls.Add(lblKuttAPIKey);
            tpKutt.Controls.Add(lblKuttHost);
            resources.ApplyResources(tpKutt, "tpKutt");
            tpKutt.Name = "tpKutt";
            tpKutt.UseVisualStyleBackColor = true;
            // 
            // txtKuttDomain
            // 
            resources.ApplyResources(txtKuttDomain, "txtKuttDomain");
            txtKuttDomain.Name = "txtKuttDomain";
            txtKuttDomain.TextChanged += txtKuttDomain_TextChanged;
            // 
            // lblKuttDomain
            // 
            resources.ApplyResources(lblKuttDomain, "lblKuttDomain");
            lblKuttDomain.Name = "lblKuttDomain";
            // 
            // lblKuttPassword
            // 
            resources.ApplyResources(lblKuttPassword, "lblKuttPassword");
            lblKuttPassword.Name = "lblKuttPassword";
            // 
            // txtKuttPassword
            // 
            resources.ApplyResources(txtKuttPassword, "txtKuttPassword");
            txtKuttPassword.Name = "txtKuttPassword";
            txtKuttPassword.UseSystemPasswordChar = true;
            txtKuttPassword.TextChanged += txtKuttPassword_TextChanged;
            // 
            // cbKuttReuse
            // 
            resources.ApplyResources(cbKuttReuse, "cbKuttReuse");
            cbKuttReuse.Name = "cbKuttReuse";
            cbKuttReuse.UseVisualStyleBackColor = true;
            cbKuttReuse.CheckedChanged += cbKuttReuse_CheckedChanged;
            // 
            // txtKuttAPIKey
            // 
            resources.ApplyResources(txtKuttAPIKey, "txtKuttAPIKey");
            txtKuttAPIKey.Name = "txtKuttAPIKey";
            txtKuttAPIKey.UseSystemPasswordChar = true;
            txtKuttAPIKey.TextChanged += txtKuttAPIKey_TextChanged;
            // 
            // txtKuttHost
            // 
            resources.ApplyResources(txtKuttHost, "txtKuttHost");
            txtKuttHost.Name = "txtKuttHost";
            txtKuttHost.TextChanged += txtKuttHost_TextChanged;
            // 
            // lblKuttAPIKey
            // 
            resources.ApplyResources(lblKuttAPIKey, "lblKuttAPIKey");
            lblKuttAPIKey.Name = "lblKuttAPIKey";
            // 
            // lblKuttHost
            // 
            resources.ApplyResources(lblKuttHost, "lblKuttHost");
            lblKuttHost.Name = "lblKuttHost";
            // 
            // tpZeroWidthShortener
            // 
            tpZeroWidthShortener.Controls.Add(txtZWSToken);
            tpZeroWidthShortener.Controls.Add(txtZWSURL);
            tpZeroWidthShortener.Controls.Add(lblZWSToken);
            tpZeroWidthShortener.Controls.Add(lblZWSURL);
            resources.ApplyResources(tpZeroWidthShortener, "tpZeroWidthShortener");
            tpZeroWidthShortener.Name = "tpZeroWidthShortener";
            tpZeroWidthShortener.UseVisualStyleBackColor = true;
            // 
            // txtZWSToken
            // 
            resources.ApplyResources(txtZWSToken, "txtZWSToken");
            txtZWSToken.Name = "txtZWSToken";
            txtZWSToken.UseSystemPasswordChar = true;
            txtZWSToken.TextChanged += txtZWSToken_TextChanged;
            // 
            // txtZWSURL
            // 
            resources.ApplyResources(txtZWSURL, "txtZWSURL");
            txtZWSURL.Name = "txtZWSURL";
            txtZWSURL.TextChanged += txtZWSURL_TextChanged;
            // 
            // lblZWSToken
            // 
            resources.ApplyResources(lblZWSToken, "lblZWSToken");
            lblZWSToken.Name = "lblZWSToken";
            // 
            // lblZWSURL
            // 
            resources.ApplyResources(lblZWSURL, "lblZWSURL");
            lblZWSURL.Name = "lblZWSURL";
            // 
            // tpFileUploaders
            // 
            tpFileUploaders.BackColor = System.Drawing.SystemColors.Window;
            tpFileUploaders.Controls.Add(tcFileUploaders);
            resources.ApplyResources(tpFileUploaders, "tpFileUploaders");
            tpFileUploaders.Name = "tpFileUploaders";
            // 
            // tcFileUploaders
            // 
            tcFileUploaders.Controls.Add(tpFTP);
            tcFileUploaders.Controls.Add(tpDropbox);
            tcFileUploaders.Controls.Add(tpOneDrive);
            tcFileUploaders.Controls.Add(tpGoogleDrive);
            tcFileUploaders.Controls.Add(tpPuush);
            tcFileUploaders.Controls.Add(tpBox);
            tcFileUploaders.Controls.Add(tpAmazonS3);
            tcFileUploaders.Controls.Add(tpGoogleCloudStorage);
            tcFileUploaders.Controls.Add(tpAzureStorage);
            tcFileUploaders.Controls.Add(tpBackblazeB2);
            tcFileUploaders.Controls.Add(tpMega);
            tcFileUploaders.Controls.Add(tpOwnCloud);
            tcFileUploaders.Controls.Add(tpMediaFire);
            tcFileUploaders.Controls.Add(tpPushbullet);
            tcFileUploaders.Controls.Add(tpSendSpace);
            tcFileUploaders.Controls.Add(tpHostr);
            tcFileUploaders.Controls.Add(tpLambda);
            tcFileUploaders.Controls.Add(tpPomf);
            tcFileUploaders.Controls.Add(tpSeafile);
            tcFileUploaders.Controls.Add(tpStreamable);
            tcFileUploaders.Controls.Add(tpSul);
            tcFileUploaders.Controls.Add(tpLithiio);
            tcFileUploaders.Controls.Add(tpPlik);
            tcFileUploaders.Controls.Add(tpYouTube);
            tcFileUploaders.Controls.Add(tpSharedFolder);
            tcFileUploaders.Controls.Add(tpEmail);
            resources.ApplyResources(tcFileUploaders, "tcFileUploaders");
            tcFileUploaders.Multiline = true;
            tcFileUploaders.Name = "tcFileUploaders";
            tcFileUploaders.SelectedIndex = 0;
            // 
            // tpFTP
            // 
            tpFTP.BackColor = System.Drawing.SystemColors.Window;
            tpFTP.Controls.Add(gbFTPAccount);
            tpFTP.Controls.Add(btnFTPDuplicate);
            tpFTP.Controls.Add(btnFTPTest);
            tpFTP.Controls.Add(btnFTPRemove);
            tpFTP.Controls.Add(btnFTPAdd);
            tpFTP.Controls.Add(cbFTPAccounts);
            tpFTP.Controls.Add(lblFTPAccounts);
            tpFTP.Controls.Add(lblFTPFile);
            tpFTP.Controls.Add(lblFTPText);
            tpFTP.Controls.Add(eiFTP);
            tpFTP.Controls.Add(lblFTPImage);
            tpFTP.Controls.Add(cbFTPImage);
            tpFTP.Controls.Add(cbFTPFile);
            tpFTP.Controls.Add(cbFTPText);
            resources.ApplyResources(tpFTP, "tpFTP");
            tpFTP.Name = "tpFTP";
            // 
            // gbFTPAccount
            // 
            gbFTPAccount.Controls.Add(gbSFTP);
            gbFTPAccount.Controls.Add(cbFTPAppendRemoteDirectory);
            gbFTPAccount.Controls.Add(lblFTPProtocol);
            gbFTPAccount.Controls.Add(lblFTPName);
            gbFTPAccount.Controls.Add(cbFTPRemoveFileExtension);
            gbFTPAccount.Controls.Add(txtFTPName);
            gbFTPAccount.Controls.Add(lblFTPHost);
            gbFTPAccount.Controls.Add(pFTPTransferMode);
            gbFTPAccount.Controls.Add(txtFTPHost);
            gbFTPAccount.Controls.Add(pFTPProtocol);
            gbFTPAccount.Controls.Add(lblFTPPort);
            gbFTPAccount.Controls.Add(lblFTPTransferMode);
            gbFTPAccount.Controls.Add(nudFTPPort);
            gbFTPAccount.Controls.Add(lblFTPURLPreviewValue);
            gbFTPAccount.Controls.Add(lblFTPUsername);
            gbFTPAccount.Controls.Add(lblFTPURLPreview);
            gbFTPAccount.Controls.Add(txtFTPUsername);
            gbFTPAccount.Controls.Add(cbFTPURLPathProtocol);
            gbFTPAccount.Controls.Add(lblFTPPassword);
            gbFTPAccount.Controls.Add(txtFTPURLPath);
            gbFTPAccount.Controls.Add(txtFTPPassword);
            gbFTPAccount.Controls.Add(lblFTPURLPath);
            gbFTPAccount.Controls.Add(lblFTPRemoteDirectory);
            gbFTPAccount.Controls.Add(txtFTPRemoteDirectory);
            gbFTPAccount.Controls.Add(gbFTPS);
            resources.ApplyResources(gbFTPAccount, "gbFTPAccount");
            gbFTPAccount.Name = "gbFTPAccount";
            gbFTPAccount.TabStop = false;
            // 
            // gbSFTP
            // 
            gbSFTP.Controls.Add(txtSFTPKeyPassphrase);
            gbSFTP.Controls.Add(btnSFTPKeyLocationBrowse);
            gbSFTP.Controls.Add(lblSFTPKeyPassphrase);
            gbSFTP.Controls.Add(txtSFTPKeyLocation);
            gbSFTP.Controls.Add(lblSFTPKeyLocation);
            resources.ApplyResources(gbSFTP, "gbSFTP");
            gbSFTP.Name = "gbSFTP";
            gbSFTP.TabStop = false;
            // 
            // txtSFTPKeyPassphrase
            // 
            resources.ApplyResources(txtSFTPKeyPassphrase, "txtSFTPKeyPassphrase");
            txtSFTPKeyPassphrase.Name = "txtSFTPKeyPassphrase";
            txtSFTPKeyPassphrase.UseSystemPasswordChar = true;
            txtSFTPKeyPassphrase.TextChanged += txtSFTPKeyPassphrase_TextChanged;
            // 
            // btnSFTPKeyLocationBrowse
            // 
            resources.ApplyResources(btnSFTPKeyLocationBrowse, "btnSFTPKeyLocationBrowse");
            btnSFTPKeyLocationBrowse.Name = "btnSFTPKeyLocationBrowse";
            btnSFTPKeyLocationBrowse.UseVisualStyleBackColor = true;
            btnSFTPKeyLocationBrowse.Click += btnSFTPKeyLocationBrowse_Click;
            // 
            // lblSFTPKeyPassphrase
            // 
            resources.ApplyResources(lblSFTPKeyPassphrase, "lblSFTPKeyPassphrase");
            lblSFTPKeyPassphrase.Name = "lblSFTPKeyPassphrase";
            // 
            // txtSFTPKeyLocation
            // 
            resources.ApplyResources(txtSFTPKeyLocation, "txtSFTPKeyLocation");
            txtSFTPKeyLocation.Name = "txtSFTPKeyLocation";
            txtSFTPKeyLocation.TextChanged += txtSFTPKeyLocation_TextChanged;
            // 
            // lblSFTPKeyLocation
            // 
            resources.ApplyResources(lblSFTPKeyLocation, "lblSFTPKeyLocation");
            lblSFTPKeyLocation.Name = "lblSFTPKeyLocation";
            // 
            // cbFTPAppendRemoteDirectory
            // 
            resources.ApplyResources(cbFTPAppendRemoteDirectory, "cbFTPAppendRemoteDirectory");
            cbFTPAppendRemoteDirectory.Name = "cbFTPAppendRemoteDirectory";
            cbFTPAppendRemoteDirectory.UseVisualStyleBackColor = true;
            cbFTPAppendRemoteDirectory.CheckedChanged += cbFTPAppendRemoteDirectory_CheckedChanged;
            // 
            // lblFTPProtocol
            // 
            resources.ApplyResources(lblFTPProtocol, "lblFTPProtocol");
            lblFTPProtocol.Name = "lblFTPProtocol";
            // 
            // lblFTPName
            // 
            resources.ApplyResources(lblFTPName, "lblFTPName");
            lblFTPName.Name = "lblFTPName";
            // 
            // cbFTPRemoveFileExtension
            // 
            resources.ApplyResources(cbFTPRemoveFileExtension, "cbFTPRemoveFileExtension");
            cbFTPRemoveFileExtension.Name = "cbFTPRemoveFileExtension";
            cbFTPRemoveFileExtension.UseVisualStyleBackColor = true;
            cbFTPRemoveFileExtension.CheckedChanged += cbFTPRemoveFileExtension_CheckedChanged;
            // 
            // txtFTPName
            // 
            resources.ApplyResources(txtFTPName, "txtFTPName");
            txtFTPName.Name = "txtFTPName";
            txtFTPName.TextChanged += txtFTPName_TextChanged;
            // 
            // lblFTPHost
            // 
            resources.ApplyResources(lblFTPHost, "lblFTPHost");
            lblFTPHost.Name = "lblFTPHost";
            // 
            // pFTPTransferMode
            // 
            resources.ApplyResources(pFTPTransferMode, "pFTPTransferMode");
            pFTPTransferMode.Controls.Add(rbFTPTransferModeActive);
            pFTPTransferMode.Controls.Add(rbFTPTransferModePassive);
            pFTPTransferMode.Name = "pFTPTransferMode";
            // 
            // rbFTPTransferModeActive
            // 
            resources.ApplyResources(rbFTPTransferModeActive, "rbFTPTransferModeActive");
            rbFTPTransferModeActive.Name = "rbFTPTransferModeActive";
            rbFTPTransferModeActive.UseVisualStyleBackColor = true;
            rbFTPTransferModeActive.CheckedChanged += rbFTPTransferModeActive_CheckedChanged;
            // 
            // rbFTPTransferModePassive
            // 
            resources.ApplyResources(rbFTPTransferModePassive, "rbFTPTransferModePassive");
            rbFTPTransferModePassive.Checked = true;
            rbFTPTransferModePassive.Name = "rbFTPTransferModePassive";
            rbFTPTransferModePassive.TabStop = true;
            rbFTPTransferModePassive.UseVisualStyleBackColor = true;
            rbFTPTransferModePassive.CheckedChanged += rbFTPTransferModePassive_CheckedChanged;
            // 
            // txtFTPHost
            // 
            resources.ApplyResources(txtFTPHost, "txtFTPHost");
            txtFTPHost.Name = "txtFTPHost";
            txtFTPHost.TextChanged += txtFTPHost_TextChanged;
            // 
            // pFTPProtocol
            // 
            resources.ApplyResources(pFTPProtocol, "pFTPProtocol");
            pFTPProtocol.Controls.Add(rbFTPProtocolFTP);
            pFTPProtocol.Controls.Add(rbFTPProtocolFTPS);
            pFTPProtocol.Controls.Add(rbFTPProtocolSFTP);
            pFTPProtocol.Name = "pFTPProtocol";
            // 
            // rbFTPProtocolFTP
            // 
            resources.ApplyResources(rbFTPProtocolFTP, "rbFTPProtocolFTP");
            rbFTPProtocolFTP.Checked = true;
            rbFTPProtocolFTP.Name = "rbFTPProtocolFTP";
            rbFTPProtocolFTP.TabStop = true;
            rbFTPProtocolFTP.UseVisualStyleBackColor = true;
            rbFTPProtocolFTP.CheckedChanged += rbFTPProtocolFTP_CheckedChanged;
            // 
            // rbFTPProtocolFTPS
            // 
            resources.ApplyResources(rbFTPProtocolFTPS, "rbFTPProtocolFTPS");
            rbFTPProtocolFTPS.Name = "rbFTPProtocolFTPS";
            rbFTPProtocolFTPS.UseVisualStyleBackColor = true;
            rbFTPProtocolFTPS.CheckedChanged += rbFTPProtocolFTPS_CheckedChanged;
            // 
            // rbFTPProtocolSFTP
            // 
            resources.ApplyResources(rbFTPProtocolSFTP, "rbFTPProtocolSFTP");
            rbFTPProtocolSFTP.Name = "rbFTPProtocolSFTP";
            rbFTPProtocolSFTP.UseVisualStyleBackColor = true;
            rbFTPProtocolSFTP.CheckedChanged += rbFTPProtocolSFTP_CheckedChanged;
            // 
            // lblFTPPort
            // 
            resources.ApplyResources(lblFTPPort, "lblFTPPort");
            lblFTPPort.Name = "lblFTPPort";
            // 
            // lblFTPTransferMode
            // 
            resources.ApplyResources(lblFTPTransferMode, "lblFTPTransferMode");
            lblFTPTransferMode.Name = "lblFTPTransferMode";
            // 
            // nudFTPPort
            // 
            resources.ApplyResources(nudFTPPort, "nudFTPPort");
            nudFTPPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudFTPPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudFTPPort.Name = "nudFTPPort";
            nudFTPPort.Value = new decimal(new int[] { 21, 0, 0, 0 });
            nudFTPPort.ValueChanged += nudFTPPort_ValueChanged;
            // 
            // lblFTPURLPreviewValue
            // 
            resources.ApplyResources(lblFTPURLPreviewValue, "lblFTPURLPreviewValue");
            lblFTPURLPreviewValue.Name = "lblFTPURLPreviewValue";
            // 
            // lblFTPUsername
            // 
            resources.ApplyResources(lblFTPUsername, "lblFTPUsername");
            lblFTPUsername.Name = "lblFTPUsername";
            // 
            // lblFTPURLPreview
            // 
            resources.ApplyResources(lblFTPURLPreview, "lblFTPURLPreview");
            lblFTPURLPreview.Name = "lblFTPURLPreview";
            // 
            // txtFTPUsername
            // 
            resources.ApplyResources(txtFTPUsername, "txtFTPUsername");
            txtFTPUsername.Name = "txtFTPUsername";
            txtFTPUsername.TextChanged += txtFTPUsername_TextChanged;
            // 
            // cbFTPURLPathProtocol
            // 
            cbFTPURLPathProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPURLPathProtocol.FormattingEnabled = true;
            resources.ApplyResources(cbFTPURLPathProtocol, "cbFTPURLPathProtocol");
            cbFTPURLPathProtocol.Name = "cbFTPURLPathProtocol";
            cbFTPURLPathProtocol.SelectedIndexChanged += cbFTPURLPathProtocol_SelectedIndexChanged;
            // 
            // lblFTPPassword
            // 
            resources.ApplyResources(lblFTPPassword, "lblFTPPassword");
            lblFTPPassword.Name = "lblFTPPassword";
            // 
            // txtFTPURLPath
            // 
            resources.ApplyResources(txtFTPURLPath, "txtFTPURLPath");
            txtFTPURLPath.Name = "txtFTPURLPath";
            txtFTPURLPath.TextChanged += txtFTPURLPath_TextChanged;
            // 
            // txtFTPPassword
            // 
            resources.ApplyResources(txtFTPPassword, "txtFTPPassword");
            txtFTPPassword.Name = "txtFTPPassword";
            txtFTPPassword.UseSystemPasswordChar = true;
            txtFTPPassword.TextChanged += txtFTPPassword_TextChanged;
            // 
            // lblFTPURLPath
            // 
            resources.ApplyResources(lblFTPURLPath, "lblFTPURLPath");
            lblFTPURLPath.Name = "lblFTPURLPath";
            // 
            // lblFTPRemoteDirectory
            // 
            resources.ApplyResources(lblFTPRemoteDirectory, "lblFTPRemoteDirectory");
            lblFTPRemoteDirectory.Name = "lblFTPRemoteDirectory";
            // 
            // txtFTPRemoteDirectory
            // 
            resources.ApplyResources(txtFTPRemoteDirectory, "txtFTPRemoteDirectory");
            txtFTPRemoteDirectory.Name = "txtFTPRemoteDirectory";
            txtFTPRemoteDirectory.TextChanged += txtFTPRemoteDirectory_TextChanged;
            // 
            // gbFTPS
            // 
            gbFTPS.Controls.Add(btnFTPSCertificateLocationBrowse);
            gbFTPS.Controls.Add(txtFTPSCertificateLocation);
            gbFTPS.Controls.Add(lblFTPSCertificateLocation);
            gbFTPS.Controls.Add(cbFTPSEncryption);
            gbFTPS.Controls.Add(lblFTPSEncryption);
            resources.ApplyResources(gbFTPS, "gbFTPS");
            gbFTPS.Name = "gbFTPS";
            gbFTPS.TabStop = false;
            // 
            // btnFTPSCertificateLocationBrowse
            // 
            resources.ApplyResources(btnFTPSCertificateLocationBrowse, "btnFTPSCertificateLocationBrowse");
            btnFTPSCertificateLocationBrowse.Name = "btnFTPSCertificateLocationBrowse";
            btnFTPSCertificateLocationBrowse.UseVisualStyleBackColor = true;
            btnFTPSCertificateLocationBrowse.Click += btnFTPSCertificateLocationBrowse_Click;
            // 
            // txtFTPSCertificateLocation
            // 
            resources.ApplyResources(txtFTPSCertificateLocation, "txtFTPSCertificateLocation");
            txtFTPSCertificateLocation.Name = "txtFTPSCertificateLocation";
            txtFTPSCertificateLocation.TextChanged += txtFTPSCertificateLocation_TextChanged;
            // 
            // lblFTPSCertificateLocation
            // 
            resources.ApplyResources(lblFTPSCertificateLocation, "lblFTPSCertificateLocation");
            lblFTPSCertificateLocation.Name = "lblFTPSCertificateLocation";
            // 
            // cbFTPSEncryption
            // 
            cbFTPSEncryption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPSEncryption.FormattingEnabled = true;
            resources.ApplyResources(cbFTPSEncryption, "cbFTPSEncryption");
            cbFTPSEncryption.Name = "cbFTPSEncryption";
            cbFTPSEncryption.SelectedIndexChanged += cbFTPSEncryption_SelectedIndexChanged;
            // 
            // lblFTPSEncryption
            // 
            resources.ApplyResources(lblFTPSEncryption, "lblFTPSEncryption");
            lblFTPSEncryption.Name = "lblFTPSEncryption";
            // 
            // btnFTPDuplicate
            // 
            resources.ApplyResources(btnFTPDuplicate, "btnFTPDuplicate");
            btnFTPDuplicate.Name = "btnFTPDuplicate";
            btnFTPDuplicate.UseVisualStyleBackColor = true;
            btnFTPDuplicate.Click += btnFTPDuplicate_Click;
            // 
            // btnFTPTest
            // 
            resources.ApplyResources(btnFTPTest, "btnFTPTest");
            btnFTPTest.Name = "btnFTPTest";
            btnFTPTest.UseVisualStyleBackColor = true;
            btnFTPTest.Click += btnFTPTest_Click;
            // 
            // btnFTPRemove
            // 
            resources.ApplyResources(btnFTPRemove, "btnFTPRemove");
            btnFTPRemove.Name = "btnFTPRemove";
            btnFTPRemove.UseVisualStyleBackColor = true;
            btnFTPRemove.Click += btnFTPRemove_Click;
            // 
            // btnFTPAdd
            // 
            resources.ApplyResources(btnFTPAdd, "btnFTPAdd");
            btnFTPAdd.Name = "btnFTPAdd";
            btnFTPAdd.UseVisualStyleBackColor = true;
            btnFTPAdd.Click += btnFTPAdd_Click;
            // 
            // cbFTPAccounts
            // 
            cbFTPAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPAccounts.FormattingEnabled = true;
            resources.ApplyResources(cbFTPAccounts, "cbFTPAccounts");
            cbFTPAccounts.Name = "cbFTPAccounts";
            cbFTPAccounts.SelectedIndexChanged += cbFTPAccounts_SelectedIndexChanged;
            // 
            // lblFTPAccounts
            // 
            resources.ApplyResources(lblFTPAccounts, "lblFTPAccounts");
            lblFTPAccounts.Name = "lblFTPAccounts";
            // 
            // lblFTPFile
            // 
            resources.ApplyResources(lblFTPFile, "lblFTPFile");
            lblFTPFile.Name = "lblFTPFile";
            // 
            // lblFTPText
            // 
            resources.ApplyResources(lblFTPText, "lblFTPText");
            lblFTPText.Name = "lblFTPText";
            // 
            // eiFTP
            // 
            eiFTP.DefaultFileName = null;
            resources.ApplyResources(eiFTP, "eiFTP");
            eiFTP.Name = "eiFTP";
            eiFTP.ObjectType = null;
            eiFTP.SerializationBinder = null;
            eiFTP.ExportRequested += eiFTP_ExportRequested;
            eiFTP.ImportRequested += eiFTP_ImportRequested;
            // 
            // lblFTPImage
            // 
            resources.ApplyResources(lblFTPImage, "lblFTPImage");
            lblFTPImage.Name = "lblFTPImage";
            // 
            // cbFTPImage
            // 
            cbFTPImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPImage.FormattingEnabled = true;
            resources.ApplyResources(cbFTPImage, "cbFTPImage");
            cbFTPImage.Name = "cbFTPImage";
            cbFTPImage.SelectedIndexChanged += cbFTPImage_SelectedIndexChanged;
            // 
            // cbFTPFile
            // 
            cbFTPFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPFile.FormattingEnabled = true;
            resources.ApplyResources(cbFTPFile, "cbFTPFile");
            cbFTPFile.Name = "cbFTPFile";
            cbFTPFile.SelectedIndexChanged += cbFTPFile_SelectedIndexChanged;
            // 
            // cbFTPText
            // 
            cbFTPText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPText.FormattingEnabled = true;
            resources.ApplyResources(cbFTPText, "cbFTPText");
            cbFTPText.Name = "cbFTPText";
            cbFTPText.SelectedIndexChanged += cbFTPText_SelectedIndexChanged;
            // 
            // tpDropbox
            // 
            tpDropbox.BackColor = System.Drawing.SystemColors.Window;
            tpDropbox.Controls.Add(cbDropboxUseDirectLink);
            tpDropbox.Controls.Add(cbDropboxAutoCreateShareableLink);
            tpDropbox.Controls.Add(lblDropboxPath);
            tpDropbox.Controls.Add(txtDropboxPath);
            tpDropbox.Controls.Add(oauth2Dropbox);
            resources.ApplyResources(tpDropbox, "tpDropbox");
            tpDropbox.Name = "tpDropbox";
            // 
            // cbDropboxUseDirectLink
            // 
            resources.ApplyResources(cbDropboxUseDirectLink, "cbDropboxUseDirectLink");
            cbDropboxUseDirectLink.Name = "cbDropboxUseDirectLink";
            cbDropboxUseDirectLink.UseVisualStyleBackColor = true;
            cbDropboxUseDirectLink.CheckedChanged += cbDropboxUseDirectLink_CheckedChanged;
            // 
            // cbDropboxAutoCreateShareableLink
            // 
            resources.ApplyResources(cbDropboxAutoCreateShareableLink, "cbDropboxAutoCreateShareableLink");
            cbDropboxAutoCreateShareableLink.Name = "cbDropboxAutoCreateShareableLink";
            cbDropboxAutoCreateShareableLink.UseVisualStyleBackColor = true;
            cbDropboxAutoCreateShareableLink.CheckedChanged += cbDropboxAutoCreateShareableLink_CheckedChanged;
            // 
            // lblDropboxPath
            // 
            resources.ApplyResources(lblDropboxPath, "lblDropboxPath");
            lblDropboxPath.Name = "lblDropboxPath";
            // 
            // txtDropboxPath
            // 
            resources.ApplyResources(txtDropboxPath, "txtDropboxPath");
            txtDropboxPath.Name = "txtDropboxPath";
            txtDropboxPath.TextChanged += txtDropboxPath_TextChanged;
            // 
            // oauth2Dropbox
            // 
            oauth2Dropbox.IsRefreshable = false;
            resources.ApplyResources(oauth2Dropbox, "oauth2Dropbox");
            oauth2Dropbox.Name = "oauth2Dropbox";
            oauth2Dropbox.UserInfo = null;
            oauth2Dropbox.OpenButtonClicked += oauth2Dropbox_OpenButtonClicked;
            oauth2Dropbox.CompleteButtonClicked += oauth2Dropbox_CompleteButtonClicked;
            oauth2Dropbox.ClearButtonClicked += oauth2Dropbox_ClearButtonClicked;
            // 
            // tpOneDrive
            // 
            tpOneDrive.BackColor = System.Drawing.SystemColors.Window;
            tpOneDrive.Controls.Add(tvOneDrive);
            tpOneDrive.Controls.Add(lblOneDriveFolderID);
            tpOneDrive.Controls.Add(cbOneDriveCreateShareableLink);
            tpOneDrive.Controls.Add(cbOneDriveUseDirectLink);
            tpOneDrive.Controls.Add(oAuth2OneDrive);
            resources.ApplyResources(tpOneDrive, "tpOneDrive");
            tpOneDrive.Name = "tpOneDrive";
            // 
            // tvOneDrive
            // 
            resources.ApplyResources(tvOneDrive, "tvOneDrive");
            tvOneDrive.Name = "tvOneDrive";
            tvOneDrive.AfterExpand += tvOneDrive_AfterExpand;
            tvOneDrive.AfterSelect += tvOneDrive_AfterSelect;
            // 
            // lblOneDriveFolderID
            // 
            resources.ApplyResources(lblOneDriveFolderID, "lblOneDriveFolderID");
            lblOneDriveFolderID.Name = "lblOneDriveFolderID";
            // 
            // cbOneDriveCreateShareableLink
            // 
            resources.ApplyResources(cbOneDriveCreateShareableLink, "cbOneDriveCreateShareableLink");
            cbOneDriveCreateShareableLink.Name = "cbOneDriveCreateShareableLink";
            cbOneDriveCreateShareableLink.UseVisualStyleBackColor = true;
            cbOneDriveCreateShareableLink.CheckedChanged += cbOneDriveCreateShareableLink_CheckedChanged;
            // 
            // cbOneDriveUseDirectLink
            // 
            resources.ApplyResources(cbOneDriveUseDirectLink, "cbOneDriveUseDirectLink");
            cbOneDriveUseDirectLink.Name = "cbOneDriveUseDirectLink";
            cbOneDriveUseDirectLink.UseVisualStyleBackColor = true;
            cbOneDriveUseDirectLink.CheckedChanged += cbOneDriveUseDirectLink_CheckedChanged;
            // 
            // oAuth2OneDrive
            // 
            resources.ApplyResources(oAuth2OneDrive, "oAuth2OneDrive");
            oAuth2OneDrive.Name = "oAuth2OneDrive";
            oAuth2OneDrive.UserInfo = null;
            oAuth2OneDrive.OpenButtonClicked += oAuth2OneDrive_OpenButtonClicked;
            oAuth2OneDrive.CompleteButtonClicked += oAuth2OneDrive_CompleteButtonClicked;
            oAuth2OneDrive.ClearButtonClicked += oAuth2OneDrive_ClearButtonClicked;
            oAuth2OneDrive.RefreshButtonClicked += oAuth2OneDrive_RefreshButtonClicked;
            // 
            // tpGoogleDrive
            // 
            tpGoogleDrive.BackColor = System.Drawing.SystemColors.Window;
            tpGoogleDrive.Controls.Add(btnGoogleDriveFolderIDHelp);
            tpGoogleDrive.Controls.Add(cbGoogleDriveSharedDrive);
            tpGoogleDrive.Controls.Add(cbGoogleDriveDirectLink);
            tpGoogleDrive.Controls.Add(cbGoogleDriveUseFolder);
            tpGoogleDrive.Controls.Add(txtGoogleDriveFolderID);
            tpGoogleDrive.Controls.Add(lblGoogleDriveFolderID);
            tpGoogleDrive.Controls.Add(oauth2GoogleDrive);
            tpGoogleDrive.Controls.Add(lvGoogleDriveFoldersList);
            tpGoogleDrive.Controls.Add(btnGoogleDriveRefreshFolders);
            tpGoogleDrive.Controls.Add(cbGoogleDriveIsPublic);
            resources.ApplyResources(tpGoogleDrive, "tpGoogleDrive");
            tpGoogleDrive.Name = "tpGoogleDrive";
            // 
            // btnGoogleDriveFolderIDHelp
            // 
            resources.ApplyResources(btnGoogleDriveFolderIDHelp, "btnGoogleDriveFolderIDHelp");
            btnGoogleDriveFolderIDHelp.Name = "btnGoogleDriveFolderIDHelp";
            btnGoogleDriveFolderIDHelp.UseVisualStyleBackColor = true;
            btnGoogleDriveFolderIDHelp.Click += btnGoogleDriveFolderIDHelp_Click;
            // 
            // cbGoogleDriveSharedDrive
            // 
            cbGoogleDriveSharedDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbGoogleDriveSharedDrive.FormattingEnabled = true;
            resources.ApplyResources(cbGoogleDriveSharedDrive, "cbGoogleDriveSharedDrive");
            cbGoogleDriveSharedDrive.Name = "cbGoogleDriveSharedDrive";
            cbGoogleDriveSharedDrive.SelectedIndexChanged += cbGoogleDriveSharedDrive_SelectedIndexChanged;
            // 
            // cbGoogleDriveDirectLink
            // 
            resources.ApplyResources(cbGoogleDriveDirectLink, "cbGoogleDriveDirectLink");
            cbGoogleDriveDirectLink.Name = "cbGoogleDriveDirectLink";
            cbGoogleDriveDirectLink.UseVisualStyleBackColor = true;
            cbGoogleDriveDirectLink.CheckedChanged += cbGoogleDriveDirectLink_CheckedChanged;
            // 
            // cbGoogleDriveUseFolder
            // 
            resources.ApplyResources(cbGoogleDriveUseFolder, "cbGoogleDriveUseFolder");
            cbGoogleDriveUseFolder.Name = "cbGoogleDriveUseFolder";
            cbGoogleDriveUseFolder.UseVisualStyleBackColor = true;
            cbGoogleDriveUseFolder.CheckedChanged += cbGoogleDriveUseFolder_CheckedChanged;
            // 
            // txtGoogleDriveFolderID
            // 
            resources.ApplyResources(txtGoogleDriveFolderID, "txtGoogleDriveFolderID");
            txtGoogleDriveFolderID.Name = "txtGoogleDriveFolderID";
            txtGoogleDriveFolderID.TextChanged += txtGoogleDriveFolderID_TextChanged;
            // 
            // lblGoogleDriveFolderID
            // 
            resources.ApplyResources(lblGoogleDriveFolderID, "lblGoogleDriveFolderID");
            lblGoogleDriveFolderID.Name = "lblGoogleDriveFolderID";
            // 
            // oauth2GoogleDrive
            // 
            resources.ApplyResources(oauth2GoogleDrive, "oauth2GoogleDrive");
            oauth2GoogleDrive.Name = "oauth2GoogleDrive";
            oauth2GoogleDrive.ConnectButtonClicked += oauth2GoogleDrive_ConnectButtonClicked;
            oauth2GoogleDrive.DisconnectButtonClicked += oauth2GoogleDrive_DisconnectButtonClicked;
            // 
            // lvGoogleDriveFoldersList
            // 
            lvGoogleDriveFoldersList.AutoFillColumn = true;
            lvGoogleDriveFoldersList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chGoogleDriveTitle, chGoogleDriveDescription });
            lvGoogleDriveFoldersList.FullRowSelect = true;
            resources.ApplyResources(lvGoogleDriveFoldersList, "lvGoogleDriveFoldersList");
            lvGoogleDriveFoldersList.MultiSelect = false;
            lvGoogleDriveFoldersList.Name = "lvGoogleDriveFoldersList";
            lvGoogleDriveFoldersList.UseCompatibleStateImageBehavior = false;
            lvGoogleDriveFoldersList.View = System.Windows.Forms.View.Details;
            lvGoogleDriveFoldersList.SelectedIndexChanged += lvGoogleDriveFoldersList_SelectedIndexChanged;
            // 
            // chGoogleDriveTitle
            // 
            resources.ApplyResources(chGoogleDriveTitle, "chGoogleDriveTitle");
            // 
            // chGoogleDriveDescription
            // 
            resources.ApplyResources(chGoogleDriveDescription, "chGoogleDriveDescription");
            // 
            // btnGoogleDriveRefreshFolders
            // 
            resources.ApplyResources(btnGoogleDriveRefreshFolders, "btnGoogleDriveRefreshFolders");
            btnGoogleDriveRefreshFolders.Name = "btnGoogleDriveRefreshFolders";
            btnGoogleDriveRefreshFolders.UseVisualStyleBackColor = true;
            btnGoogleDriveRefreshFolders.Click += btnGoogleDriveRefreshFolders_Click;
            // 
            // cbGoogleDriveIsPublic
            // 
            resources.ApplyResources(cbGoogleDriveIsPublic, "cbGoogleDriveIsPublic");
            cbGoogleDriveIsPublic.Name = "cbGoogleDriveIsPublic";
            cbGoogleDriveIsPublic.UseVisualStyleBackColor = true;
            cbGoogleDriveIsPublic.CheckedChanged += cbGoogleDriveIsPublic_CheckedChanged;
            // 
            // tpPuush
            // 
            tpPuush.BackColor = System.Drawing.SystemColors.Window;
            tpPuush.Controls.Add(lblPuushAPIKey);
            tpPuush.Controls.Add(txtPuushAPIKey);
            tpPuush.Controls.Add(llPuushForgottenPassword);
            tpPuush.Controls.Add(btnPuushLogin);
            tpPuush.Controls.Add(txtPuushPassword);
            tpPuush.Controls.Add(txtPuushEmail);
            tpPuush.Controls.Add(lblPuushEmail);
            tpPuush.Controls.Add(lblPuushPassword);
            resources.ApplyResources(tpPuush, "tpPuush");
            tpPuush.Name = "tpPuush";
            // 
            // lblPuushAPIKey
            // 
            resources.ApplyResources(lblPuushAPIKey, "lblPuushAPIKey");
            lblPuushAPIKey.Name = "lblPuushAPIKey";
            // 
            // txtPuushAPIKey
            // 
            resources.ApplyResources(txtPuushAPIKey, "txtPuushAPIKey");
            txtPuushAPIKey.Name = "txtPuushAPIKey";
            txtPuushAPIKey.UseSystemPasswordChar = true;
            txtPuushAPIKey.TextChanged += txtPuushAPIKey_TextChanged;
            // 
            // llPuushForgottenPassword
            // 
            resources.ApplyResources(llPuushForgottenPassword, "llPuushForgottenPassword");
            llPuushForgottenPassword.Name = "llPuushForgottenPassword";
            llPuushForgottenPassword.TabStop = true;
            llPuushForgottenPassword.LinkClicked += llPuushForgottenPassword_LinkClicked;
            // 
            // btnPuushLogin
            // 
            resources.ApplyResources(btnPuushLogin, "btnPuushLogin");
            btnPuushLogin.Name = "btnPuushLogin";
            btnPuushLogin.UseVisualStyleBackColor = true;
            btnPuushLogin.Click += btnPuushLogin_Click;
            // 
            // txtPuushPassword
            // 
            resources.ApplyResources(txtPuushPassword, "txtPuushPassword");
            txtPuushPassword.Name = "txtPuushPassword";
            txtPuushPassword.UseSystemPasswordChar = true;
            // 
            // txtPuushEmail
            // 
            resources.ApplyResources(txtPuushEmail, "txtPuushEmail");
            txtPuushEmail.Name = "txtPuushEmail";
            // 
            // lblPuushEmail
            // 
            resources.ApplyResources(lblPuushEmail, "lblPuushEmail");
            lblPuushEmail.Name = "lblPuushEmail";
            // 
            // lblPuushPassword
            // 
            resources.ApplyResources(lblPuushPassword, "lblPuushPassword");
            lblPuushPassword.Name = "lblPuushPassword";
            // 
            // tpBox
            // 
            tpBox.BackColor = System.Drawing.SystemColors.Window;
            tpBox.Controls.Add(lblBoxFolderTip);
            tpBox.Controls.Add(cbBoxShare);
            tpBox.Controls.Add(cbBoxShareAccessLevel);
            tpBox.Controls.Add(lblBoxShareAccessLevel);
            tpBox.Controls.Add(lvBoxFolders);
            tpBox.Controls.Add(lblBoxFolderID);
            tpBox.Controls.Add(btnBoxRefreshFolders);
            tpBox.Controls.Add(oauth2Box);
            resources.ApplyResources(tpBox, "tpBox");
            tpBox.Name = "tpBox";
            // 
            // lblBoxFolderTip
            // 
            resources.ApplyResources(lblBoxFolderTip, "lblBoxFolderTip");
            lblBoxFolderTip.Name = "lblBoxFolderTip";
            // 
            // cbBoxShare
            // 
            resources.ApplyResources(cbBoxShare, "cbBoxShare");
            cbBoxShare.Name = "cbBoxShare";
            cbBoxShare.UseVisualStyleBackColor = true;
            cbBoxShare.CheckedChanged += cbBoxShare_CheckedChanged;
            // 
            // cbBoxShareAccessLevel
            // 
            cbBoxShareAccessLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbBoxShareAccessLevel.FormattingEnabled = true;
            resources.ApplyResources(cbBoxShareAccessLevel, "cbBoxShareAccessLevel");
            cbBoxShareAccessLevel.Name = "cbBoxShareAccessLevel";
            cbBoxShareAccessLevel.SelectedIndexChanged += cbBoxShareAccessLevel_SelectedIndexChanged;
            // 
            // lblBoxShareAccessLevel
            // 
            resources.ApplyResources(lblBoxShareAccessLevel, "lblBoxShareAccessLevel");
            lblBoxShareAccessLevel.Name = "lblBoxShareAccessLevel";
            // 
            // lvBoxFolders
            // 
            lvBoxFolders.AutoFillColumn = true;
            lvBoxFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chBoxFoldersName });
            lvBoxFolders.FullRowSelect = true;
            resources.ApplyResources(lvBoxFolders, "lvBoxFolders");
            lvBoxFolders.Name = "lvBoxFolders";
            lvBoxFolders.UseCompatibleStateImageBehavior = false;
            lvBoxFolders.View = System.Windows.Forms.View.Details;
            lvBoxFolders.SelectedIndexChanged += lvBoxFolders_SelectedIndexChanged;
            lvBoxFolders.MouseDoubleClick += lvBoxFolders_MouseDoubleClick;
            // 
            // chBoxFoldersName
            // 
            resources.ApplyResources(chBoxFoldersName, "chBoxFoldersName");
            // 
            // lblBoxFolderID
            // 
            resources.ApplyResources(lblBoxFolderID, "lblBoxFolderID");
            lblBoxFolderID.Name = "lblBoxFolderID";
            // 
            // btnBoxRefreshFolders
            // 
            resources.ApplyResources(btnBoxRefreshFolders, "btnBoxRefreshFolders");
            btnBoxRefreshFolders.Name = "btnBoxRefreshFolders";
            btnBoxRefreshFolders.UseVisualStyleBackColor = true;
            btnBoxRefreshFolders.Click += btnBoxRefreshFolders_Click;
            // 
            // oauth2Box
            // 
            resources.ApplyResources(oauth2Box, "oauth2Box");
            oauth2Box.Name = "oauth2Box";
            oauth2Box.UserInfo = null;
            oauth2Box.OpenButtonClicked += oauth2Box_OpenButtonClicked;
            oauth2Box.CompleteButtonClicked += oauth2Box_CompleteButtonClicked;
            oauth2Box.ClearButtonClicked += oauth2Box_ClearButtonClicked;
            oauth2Box.RefreshButtonClicked += oauth2Box_RefreshButtonClicked;
            // 
            // tpAmazonS3
            // 
            tpAmazonS3.BackColor = System.Drawing.SystemColors.Window;
            tpAmazonS3.Controls.Add(gbAmazonS3Advanced);
            tpAmazonS3.Controls.Add(lblAmazonS3Endpoint);
            tpAmazonS3.Controls.Add(txtAmazonS3Endpoint);
            tpAmazonS3.Controls.Add(lblAmazonS3Region);
            tpAmazonS3.Controls.Add(txtAmazonS3Region);
            tpAmazonS3.Controls.Add(txtAmazonS3CustomDomain);
            tpAmazonS3.Controls.Add(lblAmazonS3PathPreviewLabel);
            tpAmazonS3.Controls.Add(lblAmazonS3PathPreview);
            tpAmazonS3.Controls.Add(btnAmazonS3BucketNameOpen);
            tpAmazonS3.Controls.Add(btnAmazonS3AccessKeyOpen);
            tpAmazonS3.Controls.Add(cbAmazonS3CustomCNAME);
            tpAmazonS3.Controls.Add(cbAmazonS3Endpoints);
            tpAmazonS3.Controls.Add(lblAmazonS3BucketName);
            tpAmazonS3.Controls.Add(txtAmazonS3BucketName);
            tpAmazonS3.Controls.Add(lblAmazonS3Endpoints);
            tpAmazonS3.Controls.Add(txtAmazonS3ObjectPrefix);
            tpAmazonS3.Controls.Add(lblAmazonS3ObjectPrefix);
            tpAmazonS3.Controls.Add(txtAmazonS3SecretKey);
            tpAmazonS3.Controls.Add(lblAmazonS3SecretKey);
            tpAmazonS3.Controls.Add(lblAmazonS3AccessKey);
            tpAmazonS3.Controls.Add(txtAmazonS3AccessKey);
            resources.ApplyResources(tpAmazonS3, "tpAmazonS3");
            tpAmazonS3.Name = "tpAmazonS3";
            // 
            // gbAmazonS3Advanced
            // 
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3SignedPayload);
            gbAmazonS3Advanced.Controls.Add(lblAmazonS3StripExtension);
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3StripExtensionText);
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3StorageClass);
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3StripExtensionVideo);
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3PublicACL);
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3StripExtensionImage);
            gbAmazonS3Advanced.Controls.Add(cbAmazonS3UsePathStyle);
            gbAmazonS3Advanced.Controls.Add(btnAmazonS3StorageClassHelp);
            gbAmazonS3Advanced.Controls.Add(lblAmazonS3StorageClass);
            resources.ApplyResources(gbAmazonS3Advanced, "gbAmazonS3Advanced");
            gbAmazonS3Advanced.Name = "gbAmazonS3Advanced";
            gbAmazonS3Advanced.TabStop = false;
            // 
            // cbAmazonS3SignedPayload
            // 
            resources.ApplyResources(cbAmazonS3SignedPayload, "cbAmazonS3SignedPayload");
            cbAmazonS3SignedPayload.Name = "cbAmazonS3SignedPayload";
            cbAmazonS3SignedPayload.UseVisualStyleBackColor = true;
            cbAmazonS3SignedPayload.CheckedChanged += cbAmazonS3SignedPayload_CheckedChanged;
            // 
            // lblAmazonS3StripExtension
            // 
            resources.ApplyResources(lblAmazonS3StripExtension, "lblAmazonS3StripExtension");
            lblAmazonS3StripExtension.Name = "lblAmazonS3StripExtension";
            // 
            // cbAmazonS3StripExtensionText
            // 
            resources.ApplyResources(cbAmazonS3StripExtensionText, "cbAmazonS3StripExtensionText");
            cbAmazonS3StripExtensionText.Name = "cbAmazonS3StripExtensionText";
            cbAmazonS3StripExtensionText.UseVisualStyleBackColor = true;
            cbAmazonS3StripExtensionText.CheckedChanged += cbAmazonS3StripExtensionText_CheckedChanged;
            // 
            // cbAmazonS3StorageClass
            // 
            cbAmazonS3StorageClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAmazonS3StorageClass.FormattingEnabled = true;
            resources.ApplyResources(cbAmazonS3StorageClass, "cbAmazonS3StorageClass");
            cbAmazonS3StorageClass.Name = "cbAmazonS3StorageClass";
            cbAmazonS3StorageClass.SelectedIndexChanged += cbAmazonS3StorageClass_SelectedIndexChanged;
            // 
            // cbAmazonS3StripExtensionVideo
            // 
            resources.ApplyResources(cbAmazonS3StripExtensionVideo, "cbAmazonS3StripExtensionVideo");
            cbAmazonS3StripExtensionVideo.Name = "cbAmazonS3StripExtensionVideo";
            cbAmazonS3StripExtensionVideo.UseVisualStyleBackColor = true;
            cbAmazonS3StripExtensionVideo.CheckedChanged += cbAmazonS3StripExtensionVideo_CheckedChanged;
            // 
            // cbAmazonS3PublicACL
            // 
            resources.ApplyResources(cbAmazonS3PublicACL, "cbAmazonS3PublicACL");
            cbAmazonS3PublicACL.Name = "cbAmazonS3PublicACL";
            cbAmazonS3PublicACL.UseVisualStyleBackColor = true;
            cbAmazonS3PublicACL.CheckedChanged += cbAmazonS3PublicACL_CheckedChanged;
            // 
            // cbAmazonS3StripExtensionImage
            // 
            resources.ApplyResources(cbAmazonS3StripExtensionImage, "cbAmazonS3StripExtensionImage");
            cbAmazonS3StripExtensionImage.Name = "cbAmazonS3StripExtensionImage";
            cbAmazonS3StripExtensionImage.UseVisualStyleBackColor = true;
            cbAmazonS3StripExtensionImage.CheckedChanged += cbAmazonS3StripExtensionImage_CheckedChanged;
            // 
            // cbAmazonS3UsePathStyle
            // 
            resources.ApplyResources(cbAmazonS3UsePathStyle, "cbAmazonS3UsePathStyle");
            cbAmazonS3UsePathStyle.Name = "cbAmazonS3UsePathStyle";
            cbAmazonS3UsePathStyle.UseVisualStyleBackColor = true;
            cbAmazonS3UsePathStyle.CheckedChanged += cbAmazonS3UsePathStyle_CheckedChanged;
            // 
            // btnAmazonS3StorageClassHelp
            // 
            resources.ApplyResources(btnAmazonS3StorageClassHelp, "btnAmazonS3StorageClassHelp");
            btnAmazonS3StorageClassHelp.Name = "btnAmazonS3StorageClassHelp";
            btnAmazonS3StorageClassHelp.UseVisualStyleBackColor = true;
            btnAmazonS3StorageClassHelp.Click += btnAmazonS3StorageClassHelp_Click;
            // 
            // lblAmazonS3StorageClass
            // 
            resources.ApplyResources(lblAmazonS3StorageClass, "lblAmazonS3StorageClass");
            lblAmazonS3StorageClass.Name = "lblAmazonS3StorageClass";
            // 
            // lblAmazonS3Endpoint
            // 
            resources.ApplyResources(lblAmazonS3Endpoint, "lblAmazonS3Endpoint");
            lblAmazonS3Endpoint.Name = "lblAmazonS3Endpoint";
            // 
            // txtAmazonS3Endpoint
            // 
            resources.ApplyResources(txtAmazonS3Endpoint, "txtAmazonS3Endpoint");
            txtAmazonS3Endpoint.Name = "txtAmazonS3Endpoint";
            txtAmazonS3Endpoint.TextChanged += txtAmazonS3Endpoint_TextChanged;
            // 
            // lblAmazonS3Region
            // 
            resources.ApplyResources(lblAmazonS3Region, "lblAmazonS3Region");
            lblAmazonS3Region.Name = "lblAmazonS3Region";
            // 
            // txtAmazonS3Region
            // 
            resources.ApplyResources(txtAmazonS3Region, "txtAmazonS3Region");
            txtAmazonS3Region.Name = "txtAmazonS3Region";
            txtAmazonS3Region.TextChanged += txtAmazonS3Region_TextChanged;
            // 
            // txtAmazonS3CustomDomain
            // 
            resources.ApplyResources(txtAmazonS3CustomDomain, "txtAmazonS3CustomDomain");
            txtAmazonS3CustomDomain.Name = "txtAmazonS3CustomDomain";
            txtAmazonS3CustomDomain.TextChanged += txtAmazonS3CustomDomain_TextChanged;
            // 
            // lblAmazonS3PathPreviewLabel
            // 
            resources.ApplyResources(lblAmazonS3PathPreviewLabel, "lblAmazonS3PathPreviewLabel");
            lblAmazonS3PathPreviewLabel.Name = "lblAmazonS3PathPreviewLabel";
            // 
            // lblAmazonS3PathPreview
            // 
            resources.ApplyResources(lblAmazonS3PathPreview, "lblAmazonS3PathPreview");
            lblAmazonS3PathPreview.Name = "lblAmazonS3PathPreview";
            // 
            // btnAmazonS3BucketNameOpen
            // 
            resources.ApplyResources(btnAmazonS3BucketNameOpen, "btnAmazonS3BucketNameOpen");
            btnAmazonS3BucketNameOpen.Name = "btnAmazonS3BucketNameOpen";
            btnAmazonS3BucketNameOpen.UseVisualStyleBackColor = true;
            btnAmazonS3BucketNameOpen.Click += btnAmazonS3BucketNameOpen_Click;
            // 
            // btnAmazonS3AccessKeyOpen
            // 
            resources.ApplyResources(btnAmazonS3AccessKeyOpen, "btnAmazonS3AccessKeyOpen");
            btnAmazonS3AccessKeyOpen.Name = "btnAmazonS3AccessKeyOpen";
            btnAmazonS3AccessKeyOpen.UseVisualStyleBackColor = true;
            btnAmazonS3AccessKeyOpen.Click += btnAmazonS3AccessKeyOpen_Click;
            // 
            // cbAmazonS3Endpoints
            // 
            cbAmazonS3Endpoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAmazonS3Endpoints.FormattingEnabled = true;
            resources.ApplyResources(cbAmazonS3Endpoints, "cbAmazonS3Endpoints");
            cbAmazonS3Endpoints.Name = "cbAmazonS3Endpoints";
            cbAmazonS3Endpoints.SelectedIndexChanged += cbAmazonS3Endpoints_SelectedIndexChanged;
            // 
            // lblAmazonS3BucketName
            // 
            resources.ApplyResources(lblAmazonS3BucketName, "lblAmazonS3BucketName");
            lblAmazonS3BucketName.Name = "lblAmazonS3BucketName";
            // 
            // txtAmazonS3BucketName
            // 
            resources.ApplyResources(txtAmazonS3BucketName, "txtAmazonS3BucketName");
            txtAmazonS3BucketName.Name = "txtAmazonS3BucketName";
            txtAmazonS3BucketName.TextChanged += txtAmazonS3BucketName_TextChanged;
            // 
            // lblAmazonS3Endpoints
            // 
            resources.ApplyResources(lblAmazonS3Endpoints, "lblAmazonS3Endpoints");
            lblAmazonS3Endpoints.Name = "lblAmazonS3Endpoints";
            // 
            // txtAmazonS3ObjectPrefix
            // 
            resources.ApplyResources(txtAmazonS3ObjectPrefix, "txtAmazonS3ObjectPrefix");
            txtAmazonS3ObjectPrefix.Name = "txtAmazonS3ObjectPrefix";
            txtAmazonS3ObjectPrefix.TextChanged += txtAmazonS3ObjectPrefix_TextChanged;
            // 
            // lblAmazonS3ObjectPrefix
            // 
            resources.ApplyResources(lblAmazonS3ObjectPrefix, "lblAmazonS3ObjectPrefix");
            lblAmazonS3ObjectPrefix.Name = "lblAmazonS3ObjectPrefix";
            // 
            // txtAmazonS3SecretKey
            // 
            resources.ApplyResources(txtAmazonS3SecretKey, "txtAmazonS3SecretKey");
            txtAmazonS3SecretKey.Name = "txtAmazonS3SecretKey";
            txtAmazonS3SecretKey.UseSystemPasswordChar = true;
            txtAmazonS3SecretKey.TextChanged += txtAmazonS3SecretKey_TextChanged;
            // 
            // lblAmazonS3SecretKey
            // 
            resources.ApplyResources(lblAmazonS3SecretKey, "lblAmazonS3SecretKey");
            lblAmazonS3SecretKey.Name = "lblAmazonS3SecretKey";
            // 
            // lblAmazonS3AccessKey
            // 
            resources.ApplyResources(lblAmazonS3AccessKey, "lblAmazonS3AccessKey");
            lblAmazonS3AccessKey.Name = "lblAmazonS3AccessKey";
            // 
            // txtAmazonS3AccessKey
            // 
            resources.ApplyResources(txtAmazonS3AccessKey, "txtAmazonS3AccessKey");
            txtAmazonS3AccessKey.Name = "txtAmazonS3AccessKey";
            txtAmazonS3AccessKey.TextChanged += txtAmazonS3AccessKey_TextChanged;
            // 
            // tpGoogleCloudStorage
            // 
            tpGoogleCloudStorage.Controls.Add(oauth2GoogleCloudStorage);
            tpGoogleCloudStorage.Controls.Add(gbGoogleCloudStorageAdvanced);
            tpGoogleCloudStorage.Controls.Add(lblGoogleCloudStoragePathPreview);
            tpGoogleCloudStorage.Controls.Add(lblGoogleCloudStoragePathPreviewLabel);
            tpGoogleCloudStorage.Controls.Add(txtGoogleCloudStorageObjectPrefix);
            tpGoogleCloudStorage.Controls.Add(lblGoogleCloudStorageObjectPrefix);
            tpGoogleCloudStorage.Controls.Add(lblGoogleCloudStorageDomain);
            tpGoogleCloudStorage.Controls.Add(txtGoogleCloudStorageDomain);
            tpGoogleCloudStorage.Controls.Add(lblGoogleCloudStorageBucket);
            tpGoogleCloudStorage.Controls.Add(txtGoogleCloudStorageBucket);
            resources.ApplyResources(tpGoogleCloudStorage, "tpGoogleCloudStorage");
            tpGoogleCloudStorage.Name = "tpGoogleCloudStorage";
            tpGoogleCloudStorage.UseVisualStyleBackColor = true;
            // 
            // oauth2GoogleCloudStorage
            // 
            resources.ApplyResources(oauth2GoogleCloudStorage, "oauth2GoogleCloudStorage");
            oauth2GoogleCloudStorage.Name = "oauth2GoogleCloudStorage";
            oauth2GoogleCloudStorage.ConnectButtonClicked += oauth2GoogleCloudStorage_ConnectButtonClicked;
            oauth2GoogleCloudStorage.DisconnectButtonClicked += oauth2GoogleCloudStorage_DisconnectButtonClicked;
            // 
            // gbGoogleCloudStorageAdvanced
            // 
            gbGoogleCloudStorageAdvanced.Controls.Add(lblGoogleCloudStorageStripExtension);
            gbGoogleCloudStorageAdvanced.Controls.Add(cbGoogleCloudStorageStripExtensionText);
            gbGoogleCloudStorageAdvanced.Controls.Add(cbGoogleCloudStorageStripExtensionVideo);
            gbGoogleCloudStorageAdvanced.Controls.Add(cbGoogleCloudStorageSetPublicACL);
            gbGoogleCloudStorageAdvanced.Controls.Add(cbGoogleCloudStorageStripExtensionImage);
            resources.ApplyResources(gbGoogleCloudStorageAdvanced, "gbGoogleCloudStorageAdvanced");
            gbGoogleCloudStorageAdvanced.Name = "gbGoogleCloudStorageAdvanced";
            gbGoogleCloudStorageAdvanced.TabStop = false;
            // 
            // lblGoogleCloudStorageStripExtension
            // 
            resources.ApplyResources(lblGoogleCloudStorageStripExtension, "lblGoogleCloudStorageStripExtension");
            lblGoogleCloudStorageStripExtension.Name = "lblGoogleCloudStorageStripExtension";
            // 
            // cbGoogleCloudStorageStripExtensionText
            // 
            resources.ApplyResources(cbGoogleCloudStorageStripExtensionText, "cbGoogleCloudStorageStripExtensionText");
            cbGoogleCloudStorageStripExtensionText.Name = "cbGoogleCloudStorageStripExtensionText";
            cbGoogleCloudStorageStripExtensionText.UseVisualStyleBackColor = true;
            cbGoogleCloudStorageStripExtensionText.CheckedChanged += cbGoogleCloudStorageStripExtensionText_CheckedChanged;
            // 
            // cbGoogleCloudStorageStripExtensionVideo
            // 
            resources.ApplyResources(cbGoogleCloudStorageStripExtensionVideo, "cbGoogleCloudStorageStripExtensionVideo");
            cbGoogleCloudStorageStripExtensionVideo.Name = "cbGoogleCloudStorageStripExtensionVideo";
            cbGoogleCloudStorageStripExtensionVideo.UseVisualStyleBackColor = true;
            cbGoogleCloudStorageStripExtensionVideo.CheckedChanged += cbGoogleCloudStorageStripExtensionVideo_CheckedChanged;
            // 
            // cbGoogleCloudStorageSetPublicACL
            // 
            resources.ApplyResources(cbGoogleCloudStorageSetPublicACL, "cbGoogleCloudStorageSetPublicACL");
            cbGoogleCloudStorageSetPublicACL.Name = "cbGoogleCloudStorageSetPublicACL";
            cbGoogleCloudStorageSetPublicACL.UseVisualStyleBackColor = true;
            cbGoogleCloudStorageSetPublicACL.CheckedChanged += cbGoogleCloudStorageSetPublicACL_CheckedChanged;
            // 
            // cbGoogleCloudStorageStripExtensionImage
            // 
            resources.ApplyResources(cbGoogleCloudStorageStripExtensionImage, "cbGoogleCloudStorageStripExtensionImage");
            cbGoogleCloudStorageStripExtensionImage.Name = "cbGoogleCloudStorageStripExtensionImage";
            cbGoogleCloudStorageStripExtensionImage.UseVisualStyleBackColor = true;
            cbGoogleCloudStorageStripExtensionImage.CheckedChanged += cbGoogleCloudStorageStripExtensionImage_CheckedChanged;
            // 
            // lblGoogleCloudStoragePathPreview
            // 
            resources.ApplyResources(lblGoogleCloudStoragePathPreview, "lblGoogleCloudStoragePathPreview");
            lblGoogleCloudStoragePathPreview.Name = "lblGoogleCloudStoragePathPreview";
            // 
            // lblGoogleCloudStoragePathPreviewLabel
            // 
            resources.ApplyResources(lblGoogleCloudStoragePathPreviewLabel, "lblGoogleCloudStoragePathPreviewLabel");
            lblGoogleCloudStoragePathPreviewLabel.Name = "lblGoogleCloudStoragePathPreviewLabel";
            // 
            // txtGoogleCloudStorageObjectPrefix
            // 
            resources.ApplyResources(txtGoogleCloudStorageObjectPrefix, "txtGoogleCloudStorageObjectPrefix");
            txtGoogleCloudStorageObjectPrefix.Name = "txtGoogleCloudStorageObjectPrefix";
            txtGoogleCloudStorageObjectPrefix.TextChanged += txtGoogleCloudStorageObjectPrefix_TextChanged;
            // 
            // lblGoogleCloudStorageObjectPrefix
            // 
            resources.ApplyResources(lblGoogleCloudStorageObjectPrefix, "lblGoogleCloudStorageObjectPrefix");
            lblGoogleCloudStorageObjectPrefix.Name = "lblGoogleCloudStorageObjectPrefix";
            // 
            // lblGoogleCloudStorageDomain
            // 
            resources.ApplyResources(lblGoogleCloudStorageDomain, "lblGoogleCloudStorageDomain");
            lblGoogleCloudStorageDomain.Name = "lblGoogleCloudStorageDomain";
            // 
            // txtGoogleCloudStorageDomain
            // 
            resources.ApplyResources(txtGoogleCloudStorageDomain, "txtGoogleCloudStorageDomain");
            txtGoogleCloudStorageDomain.Name = "txtGoogleCloudStorageDomain";
            txtGoogleCloudStorageDomain.TextChanged += txtGoogleCloudStorageDomain_TextChanged;
            // 
            // lblGoogleCloudStorageBucket
            // 
            resources.ApplyResources(lblGoogleCloudStorageBucket, "lblGoogleCloudStorageBucket");
            lblGoogleCloudStorageBucket.Name = "lblGoogleCloudStorageBucket";
            // 
            // txtGoogleCloudStorageBucket
            // 
            resources.ApplyResources(txtGoogleCloudStorageBucket, "txtGoogleCloudStorageBucket");
            txtGoogleCloudStorageBucket.Name = "txtGoogleCloudStorageBucket";
            txtGoogleCloudStorageBucket.TextChanged += txtGoogleCloudStorageBucket_TextChanged;
            // 
            // tpAzureStorage
            // 
            tpAzureStorage.BackColor = System.Drawing.SystemColors.Window;
            tpAzureStorage.Controls.Add(txtAzureStorageCacheControl);
            tpAzureStorage.Controls.Add(lblAzureStorageCacheControl);
            tpAzureStorage.Controls.Add(lblAzureStorageURLPreview);
            tpAzureStorage.Controls.Add(lblAzureStorageURLPreviewLabel);
            tpAzureStorage.Controls.Add(txtAzureStorageUploadPath);
            tpAzureStorage.Controls.Add(lblAzureStorageUploadPath);
            tpAzureStorage.Controls.Add(cbAzureStorageEnvironment);
            tpAzureStorage.Controls.Add(lblAzureStorageEnvironment);
            tpAzureStorage.Controls.Add(btnAzureStoragePortal);
            tpAzureStorage.Controls.Add(txtAzureStorageContainer);
            tpAzureStorage.Controls.Add(lblAzureStorageContainer);
            tpAzureStorage.Controls.Add(txtAzureStorageAccessKey);
            tpAzureStorage.Controls.Add(lblAzureStorageAccessKey);
            tpAzureStorage.Controls.Add(txtAzureStorageAccountName);
            tpAzureStorage.Controls.Add(lblAzureStorageAccountName);
            tpAzureStorage.Controls.Add(txtAzureStorageCustomDomain);
            tpAzureStorage.Controls.Add(lblAzureStorageCustomDomain);
            resources.ApplyResources(tpAzureStorage, "tpAzureStorage");
            tpAzureStorage.Name = "tpAzureStorage";
            // 
            // txtAzureStorageCacheControl
            // 
            resources.ApplyResources(txtAzureStorageCacheControl, "txtAzureStorageCacheControl");
            txtAzureStorageCacheControl.Name = "txtAzureStorageCacheControl";
            txtAzureStorageCacheControl.TextChanged += txtAzureStorageCacheControl_TextChanged;
            // 
            // lblAzureStorageCacheControl
            // 
            resources.ApplyResources(lblAzureStorageCacheControl, "lblAzureStorageCacheControl");
            lblAzureStorageCacheControl.Name = "lblAzureStorageCacheControl";
            // 
            // lblAzureStorageURLPreview
            // 
            resources.ApplyResources(lblAzureStorageURLPreview, "lblAzureStorageURLPreview");
            lblAzureStorageURLPreview.Name = "lblAzureStorageURLPreview";
            // 
            // lblAzureStorageURLPreviewLabel
            // 
            resources.ApplyResources(lblAzureStorageURLPreviewLabel, "lblAzureStorageURLPreviewLabel");
            lblAzureStorageURLPreviewLabel.Name = "lblAzureStorageURLPreviewLabel";
            // 
            // txtAzureStorageUploadPath
            // 
            resources.ApplyResources(txtAzureStorageUploadPath, "txtAzureStorageUploadPath");
            txtAzureStorageUploadPath.Name = "txtAzureStorageUploadPath";
            txtAzureStorageUploadPath.TextChanged += txtAzureStorageUploadPath_TextChanged;
            // 
            // lblAzureStorageUploadPath
            // 
            resources.ApplyResources(lblAzureStorageUploadPath, "lblAzureStorageUploadPath");
            lblAzureStorageUploadPath.Name = "lblAzureStorageUploadPath";
            // 
            // cbAzureStorageEnvironment
            // 
            cbAzureStorageEnvironment.FormattingEnabled = true;
            cbAzureStorageEnvironment.Items.AddRange(new object[] { resources.GetString("cbAzureStorageEnvironment.Items"), resources.GetString("cbAzureStorageEnvironment.Items1"), resources.GetString("cbAzureStorageEnvironment.Items2"), resources.GetString("cbAzureStorageEnvironment.Items3") });
            resources.ApplyResources(cbAzureStorageEnvironment, "cbAzureStorageEnvironment");
            cbAzureStorageEnvironment.Name = "cbAzureStorageEnvironment";
            cbAzureStorageEnvironment.SelectedIndexChanged += cbAzureStorageEnvironment_SelectedIndexChanged;
            // 
            // lblAzureStorageEnvironment
            // 
            resources.ApplyResources(lblAzureStorageEnvironment, "lblAzureStorageEnvironment");
            lblAzureStorageEnvironment.Name = "lblAzureStorageEnvironment";
            // 
            // btnAzureStoragePortal
            // 
            resources.ApplyResources(btnAzureStoragePortal, "btnAzureStoragePortal");
            btnAzureStoragePortal.Name = "btnAzureStoragePortal";
            btnAzureStoragePortal.UseVisualStyleBackColor = true;
            btnAzureStoragePortal.Click += btnAzureStoragePortal_Click;
            // 
            // txtAzureStorageContainer
            // 
            resources.ApplyResources(txtAzureStorageContainer, "txtAzureStorageContainer");
            txtAzureStorageContainer.Name = "txtAzureStorageContainer";
            txtAzureStorageContainer.TextChanged += txtAzureStorageContainer_TextChanged;
            // 
            // lblAzureStorageContainer
            // 
            resources.ApplyResources(lblAzureStorageContainer, "lblAzureStorageContainer");
            lblAzureStorageContainer.Name = "lblAzureStorageContainer";
            // 
            // txtAzureStorageAccessKey
            // 
            resources.ApplyResources(txtAzureStorageAccessKey, "txtAzureStorageAccessKey");
            txtAzureStorageAccessKey.Name = "txtAzureStorageAccessKey";
            txtAzureStorageAccessKey.UseSystemPasswordChar = true;
            txtAzureStorageAccessKey.TextChanged += txtAzureStorageAccessKey_TextChanged;
            // 
            // lblAzureStorageAccessKey
            // 
            resources.ApplyResources(lblAzureStorageAccessKey, "lblAzureStorageAccessKey");
            lblAzureStorageAccessKey.Name = "lblAzureStorageAccessKey";
            // 
            // txtAzureStorageAccountName
            // 
            resources.ApplyResources(txtAzureStorageAccountName, "txtAzureStorageAccountName");
            txtAzureStorageAccountName.Name = "txtAzureStorageAccountName";
            txtAzureStorageAccountName.TextChanged += txtAzureStorageAccountName_TextChanged;
            // 
            // lblAzureStorageAccountName
            // 
            resources.ApplyResources(lblAzureStorageAccountName, "lblAzureStorageAccountName");
            lblAzureStorageAccountName.Name = "lblAzureStorageAccountName";
            // 
            // txtAzureStorageCustomDomain
            // 
            resources.ApplyResources(txtAzureStorageCustomDomain, "txtAzureStorageCustomDomain");
            txtAzureStorageCustomDomain.Name = "txtAzureStorageCustomDomain";
            txtAzureStorageCustomDomain.TextChanged += txtAzureStorageCustomDomain_TextChanged;
            // 
            // lblAzureStorageCustomDomain
            // 
            resources.ApplyResources(lblAzureStorageCustomDomain, "lblAzureStorageCustomDomain");
            lblAzureStorageCustomDomain.Name = "lblAzureStorageCustomDomain";
            // 
            // tpBackblazeB2
            // 
            tpBackblazeB2.BackColor = System.Drawing.SystemColors.Window;
            tpBackblazeB2.Controls.Add(lblB2UrlPreview);
            tpBackblazeB2.Controls.Add(lblB2ManageLink);
            tpBackblazeB2.Controls.Add(txtB2CustomUrl);
            tpBackblazeB2.Controls.Add(lblB2UrlPreviewLabel);
            tpBackblazeB2.Controls.Add(cbB2CustomUrl);
            tpBackblazeB2.Controls.Add(lblB2Bucket);
            tpBackblazeB2.Controls.Add(txtB2Bucket);
            tpBackblazeB2.Controls.Add(txtB2UploadPath);
            tpBackblazeB2.Controls.Add(lblB2UploadPath);
            tpBackblazeB2.Controls.Add(txtB2ApplicationKey);
            tpBackblazeB2.Controls.Add(lblB2ApplicationKey);
            tpBackblazeB2.Controls.Add(lblB2ApplicationKeyId);
            tpBackblazeB2.Controls.Add(txtB2ApplicationKeyId);
            resources.ApplyResources(tpBackblazeB2, "tpBackblazeB2");
            tpBackblazeB2.Name = "tpBackblazeB2";
            // 
            // lblB2UrlPreview
            // 
            resources.ApplyResources(lblB2UrlPreview, "lblB2UrlPreview");
            lblB2UrlPreview.Name = "lblB2UrlPreview";
            // 
            // lblB2ManageLink
            // 
            resources.ApplyResources(lblB2ManageLink, "lblB2ManageLink");
            lblB2ManageLink.Name = "lblB2ManageLink";
            lblB2ManageLink.TabStop = true;
            lblB2ManageLink.VisitedLinkColor = System.Drawing.Color.Blue;
            lblB2ManageLink.LinkClicked += lblB2ManageLink_LinkClicked;
            // 
            // lblB2UrlPreviewLabel
            // 
            resources.ApplyResources(lblB2UrlPreviewLabel, "lblB2UrlPreviewLabel");
            lblB2UrlPreviewLabel.Name = "lblB2UrlPreviewLabel";
            // 
            // lblB2Bucket
            // 
            resources.ApplyResources(lblB2Bucket, "lblB2Bucket");
            lblB2Bucket.Name = "lblB2Bucket";
            // 
            // lblB2UploadPath
            // 
            resources.ApplyResources(lblB2UploadPath, "lblB2UploadPath");
            lblB2UploadPath.Name = "lblB2UploadPath";
            // 
            // lblB2ApplicationKey
            // 
            resources.ApplyResources(lblB2ApplicationKey, "lblB2ApplicationKey");
            lblB2ApplicationKey.Name = "lblB2ApplicationKey";
            // 
            // lblB2ApplicationKeyId
            // 
            resources.ApplyResources(lblB2ApplicationKeyId, "lblB2ApplicationKeyId");
            lblB2ApplicationKeyId.Name = "lblB2ApplicationKeyId";
            // 
            // tpMega
            // 
            tpMega.BackColor = System.Drawing.SystemColors.Window;
            tpMega.Controls.Add(btnMegaRefreshFolders);
            tpMega.Controls.Add(lblMegaStatus);
            tpMega.Controls.Add(btnMegaRegister);
            tpMega.Controls.Add(lblMegaFolder);
            tpMega.Controls.Add(lblMegaStatusTitle);
            tpMega.Controls.Add(cbMegaFolder);
            tpMega.Controls.Add(lblMegaEmail);
            tpMega.Controls.Add(btnMegaLogin);
            tpMega.Controls.Add(txtMegaEmail);
            tpMega.Controls.Add(txtMegaPassword);
            tpMega.Controls.Add(lblMegaPassword);
            resources.ApplyResources(tpMega, "tpMega");
            tpMega.Name = "tpMega";
            // 
            // btnMegaRefreshFolders
            // 
            resources.ApplyResources(btnMegaRefreshFolders, "btnMegaRefreshFolders");
            btnMegaRefreshFolders.Name = "btnMegaRefreshFolders";
            btnMegaRefreshFolders.UseVisualStyleBackColor = true;
            btnMegaRefreshFolders.Click += btnMegaRefreshFolders_Click;
            // 
            // lblMegaStatus
            // 
            resources.ApplyResources(lblMegaStatus, "lblMegaStatus");
            lblMegaStatus.Name = "lblMegaStatus";
            // 
            // btnMegaRegister
            // 
            resources.ApplyResources(btnMegaRegister, "btnMegaRegister");
            btnMegaRegister.Name = "btnMegaRegister";
            btnMegaRegister.UseVisualStyleBackColor = true;
            btnMegaRegister.Click += btnMegaRegister_Click;
            // 
            // lblMegaFolder
            // 
            resources.ApplyResources(lblMegaFolder, "lblMegaFolder");
            lblMegaFolder.Name = "lblMegaFolder";
            // 
            // lblMegaStatusTitle
            // 
            resources.ApplyResources(lblMegaStatusTitle, "lblMegaStatusTitle");
            lblMegaStatusTitle.Name = "lblMegaStatusTitle";
            // 
            // cbMegaFolder
            // 
            cbMegaFolder.DisplayMember = "DisplayName";
            cbMegaFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbMegaFolder.FormattingEnabled = true;
            resources.ApplyResources(cbMegaFolder, "cbMegaFolder");
            cbMegaFolder.Name = "cbMegaFolder";
            cbMegaFolder.ValueMember = "Node";
            cbMegaFolder.SelectedIndexChanged += cbMegaFolder_SelectedIndexChanged;
            // 
            // lblMegaEmail
            // 
            resources.ApplyResources(lblMegaEmail, "lblMegaEmail");
            lblMegaEmail.Name = "lblMegaEmail";
            // 
            // btnMegaLogin
            // 
            resources.ApplyResources(btnMegaLogin, "btnMegaLogin");
            btnMegaLogin.Name = "btnMegaLogin";
            btnMegaLogin.UseVisualStyleBackColor = true;
            btnMegaLogin.Click += btnMegaLogin_Click;
            // 
            // txtMegaEmail
            // 
            resources.ApplyResources(txtMegaEmail, "txtMegaEmail");
            txtMegaEmail.Name = "txtMegaEmail";
            // 
            // txtMegaPassword
            // 
            resources.ApplyResources(txtMegaPassword, "txtMegaPassword");
            txtMegaPassword.Name = "txtMegaPassword";
            txtMegaPassword.UseSystemPasswordChar = true;
            // 
            // lblMegaPassword
            // 
            resources.ApplyResources(lblMegaPassword, "lblMegaPassword");
            lblMegaPassword.Name = "lblMegaPassword";
            // 
            // tpOwnCloud
            // 
            tpOwnCloud.BackColor = System.Drawing.SystemColors.Window;
            tpOwnCloud.Controls.Add(cbOwnCloudAppendFileNameToURL);
            tpOwnCloud.Controls.Add(nudOwnCloudExpiryTime);
            tpOwnCloud.Controls.Add(cbOwnCloudAutoExpire);
            tpOwnCloud.Controls.Add(lblOwnCloudExpiryTime);
            tpOwnCloud.Controls.Add(cbOwnCloudUsePreviewLinks);
            tpOwnCloud.Controls.Add(lblOwnCloudHostExample);
            tpOwnCloud.Controls.Add(cbOwnCloud81Compatibility);
            tpOwnCloud.Controls.Add(cbOwnCloudDirectLink);
            tpOwnCloud.Controls.Add(cbOwnCloudCreateShare);
            tpOwnCloud.Controls.Add(txtOwnCloudPath);
            tpOwnCloud.Controls.Add(txtOwnCloudPassword);
            tpOwnCloud.Controls.Add(txtOwnCloudUsername);
            tpOwnCloud.Controls.Add(txtOwnCloudHost);
            tpOwnCloud.Controls.Add(lblOwnCloudPath);
            tpOwnCloud.Controls.Add(lblOwnCloudPassword);
            tpOwnCloud.Controls.Add(lblOwnCloudUsername);
            tpOwnCloud.Controls.Add(lblOwnCloudHost);
            resources.ApplyResources(tpOwnCloud, "tpOwnCloud");
            tpOwnCloud.Name = "tpOwnCloud";
            // 
            // cbOwnCloudAppendFileNameToURL
            // 
            resources.ApplyResources(cbOwnCloudAppendFileNameToURL, "cbOwnCloudAppendFileNameToURL");
            cbOwnCloudAppendFileNameToURL.Name = "cbOwnCloudAppendFileNameToURL";
            cbOwnCloudAppendFileNameToURL.UseMnemonic = false;
            cbOwnCloudAppendFileNameToURL.UseVisualStyleBackColor = true;
            cbOwnCloudAppendFileNameToURL.CheckedChanged += cbOwnCloudAppendFileNameToURL_CheckedChanged;
            // 
            // nudOwnCloudExpiryTime
            // 
            resources.ApplyResources(nudOwnCloudExpiryTime, "nudOwnCloudExpiryTime");
            nudOwnCloudExpiryTime.Maximum = new decimal(new int[] { 1410065407, 2, 0, 0 });
            nudOwnCloudExpiryTime.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudOwnCloudExpiryTime.Name = "nudOwnCloudExpiryTime";
            nudOwnCloudExpiryTime.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudOwnCloudExpiryTime.ValueChanged += nudOwnExpiryTime_TextChanged;
            // 
            // cbOwnCloudAutoExpire
            // 
            resources.ApplyResources(cbOwnCloudAutoExpire, "cbOwnCloudAutoExpire");
            cbOwnCloudAutoExpire.Name = "cbOwnCloudAutoExpire";
            cbOwnCloudAutoExpire.UseVisualStyleBackColor = true;
            cbOwnCloudAutoExpire.CheckedChanged += cbOwnCloudAutoExpire_CheckedChanged;
            // 
            // lblOwnCloudExpiryTime
            // 
            resources.ApplyResources(lblOwnCloudExpiryTime, "lblOwnCloudExpiryTime");
            lblOwnCloudExpiryTime.Name = "lblOwnCloudExpiryTime";
            // 
            // cbOwnCloudUsePreviewLinks
            // 
            resources.ApplyResources(cbOwnCloudUsePreviewLinks, "cbOwnCloudUsePreviewLinks");
            cbOwnCloudUsePreviewLinks.Name = "cbOwnCloudUsePreviewLinks";
            cbOwnCloudUsePreviewLinks.UseVisualStyleBackColor = true;
            cbOwnCloudUsePreviewLinks.CheckedChanged += cbOwnCloudUsePreviewLinks_CheckedChanged;
            // 
            // lblOwnCloudHostExample
            // 
            resources.ApplyResources(lblOwnCloudHostExample, "lblOwnCloudHostExample");
            lblOwnCloudHostExample.Name = "lblOwnCloudHostExample";
            // 
            // cbOwnCloud81Compatibility
            // 
            resources.ApplyResources(cbOwnCloud81Compatibility, "cbOwnCloud81Compatibility");
            cbOwnCloud81Compatibility.Name = "cbOwnCloud81Compatibility";
            cbOwnCloud81Compatibility.UseVisualStyleBackColor = true;
            cbOwnCloud81Compatibility.CheckedChanged += cbOwnCloud81Compatibility_CheckedChanged;
            // 
            // cbOwnCloudDirectLink
            // 
            resources.ApplyResources(cbOwnCloudDirectLink, "cbOwnCloudDirectLink");
            cbOwnCloudDirectLink.Name = "cbOwnCloudDirectLink";
            cbOwnCloudDirectLink.UseMnemonic = false;
            cbOwnCloudDirectLink.UseVisualStyleBackColor = true;
            cbOwnCloudDirectLink.CheckedChanged += cbOwnCloudDirectLink_CheckedChanged;
            // 
            // cbOwnCloudCreateShare
            // 
            resources.ApplyResources(cbOwnCloudCreateShare, "cbOwnCloudCreateShare");
            cbOwnCloudCreateShare.Name = "cbOwnCloudCreateShare";
            cbOwnCloudCreateShare.UseVisualStyleBackColor = true;
            cbOwnCloudCreateShare.CheckedChanged += cbOwnCloudCreateShare_CheckedChanged;
            // 
            // txtOwnCloudPath
            // 
            resources.ApplyResources(txtOwnCloudPath, "txtOwnCloudPath");
            txtOwnCloudPath.Name = "txtOwnCloudPath";
            txtOwnCloudPath.TextChanged += txtOwnCloudPath_TextChanged;
            // 
            // txtOwnCloudPassword
            // 
            resources.ApplyResources(txtOwnCloudPassword, "txtOwnCloudPassword");
            txtOwnCloudPassword.Name = "txtOwnCloudPassword";
            txtOwnCloudPassword.UseSystemPasswordChar = true;
            txtOwnCloudPassword.TextChanged += txtOwnCloudPassword_TextChanged;
            // 
            // txtOwnCloudUsername
            // 
            resources.ApplyResources(txtOwnCloudUsername, "txtOwnCloudUsername");
            txtOwnCloudUsername.Name = "txtOwnCloudUsername";
            txtOwnCloudUsername.TextChanged += txtOwnCloudUsername_TextChanged;
            // 
            // txtOwnCloudHost
            // 
            resources.ApplyResources(txtOwnCloudHost, "txtOwnCloudHost");
            txtOwnCloudHost.Name = "txtOwnCloudHost";
            txtOwnCloudHost.TextChanged += txtOwnCloudHost_TextChanged;
            // 
            // lblOwnCloudPath
            // 
            resources.ApplyResources(lblOwnCloudPath, "lblOwnCloudPath");
            lblOwnCloudPath.Name = "lblOwnCloudPath";
            // 
            // lblOwnCloudPassword
            // 
            resources.ApplyResources(lblOwnCloudPassword, "lblOwnCloudPassword");
            lblOwnCloudPassword.Name = "lblOwnCloudPassword";
            // 
            // lblOwnCloudUsername
            // 
            resources.ApplyResources(lblOwnCloudUsername, "lblOwnCloudUsername");
            lblOwnCloudUsername.Name = "lblOwnCloudUsername";
            // 
            // lblOwnCloudHost
            // 
            resources.ApplyResources(lblOwnCloudHost, "lblOwnCloudHost");
            lblOwnCloudHost.Name = "lblOwnCloudHost";
            // 
            // tpMediaFire
            // 
            tpMediaFire.BackColor = System.Drawing.SystemColors.Window;
            tpMediaFire.Controls.Add(cbMediaFireUseLongLink);
            tpMediaFire.Controls.Add(txtMediaFirePath);
            tpMediaFire.Controls.Add(lblMediaFirePath);
            tpMediaFire.Controls.Add(txtMediaFirePassword);
            tpMediaFire.Controls.Add(txtMediaFireEmail);
            tpMediaFire.Controls.Add(lblMediaFirePassword);
            tpMediaFire.Controls.Add(lblMediaFireEmail);
            resources.ApplyResources(tpMediaFire, "tpMediaFire");
            tpMediaFire.Name = "tpMediaFire";
            // 
            // cbMediaFireUseLongLink
            // 
            resources.ApplyResources(cbMediaFireUseLongLink, "cbMediaFireUseLongLink");
            cbMediaFireUseLongLink.Name = "cbMediaFireUseLongLink";
            cbMediaFireUseLongLink.UseVisualStyleBackColor = true;
            cbMediaFireUseLongLink.CheckedChanged += cbMediaFireUseLongLink_CheckedChanged;
            // 
            // txtMediaFirePath
            // 
            resources.ApplyResources(txtMediaFirePath, "txtMediaFirePath");
            txtMediaFirePath.Name = "txtMediaFirePath";
            txtMediaFirePath.TextChanged += txtMediaFirePath_TextChanged;
            // 
            // lblMediaFirePath
            // 
            resources.ApplyResources(lblMediaFirePath, "lblMediaFirePath");
            lblMediaFirePath.Name = "lblMediaFirePath";
            // 
            // txtMediaFirePassword
            // 
            resources.ApplyResources(txtMediaFirePassword, "txtMediaFirePassword");
            txtMediaFirePassword.Name = "txtMediaFirePassword";
            txtMediaFirePassword.UseSystemPasswordChar = true;
            txtMediaFirePassword.TextChanged += txtMediaFirePassword_TextChanged;
            // 
            // txtMediaFireEmail
            // 
            resources.ApplyResources(txtMediaFireEmail, "txtMediaFireEmail");
            txtMediaFireEmail.Name = "txtMediaFireEmail";
            txtMediaFireEmail.TextChanged += txtMediaFireUsername_TextChanged;
            // 
            // lblMediaFirePassword
            // 
            resources.ApplyResources(lblMediaFirePassword, "lblMediaFirePassword");
            lblMediaFirePassword.Name = "lblMediaFirePassword";
            // 
            // lblMediaFireEmail
            // 
            resources.ApplyResources(lblMediaFireEmail, "lblMediaFireEmail");
            lblMediaFireEmail.Name = "lblMediaFireEmail";
            // 
            // tpPushbullet
            // 
            tpPushbullet.BackColor = System.Drawing.SystemColors.Window;
            tpPushbullet.Controls.Add(lblPushbulletDevices);
            tpPushbullet.Controls.Add(cbPushbulletDevices);
            tpPushbullet.Controls.Add(btnPushbulletGetDeviceList);
            tpPushbullet.Controls.Add(lblPushbulletUserKey);
            tpPushbullet.Controls.Add(txtPushbulletUserKey);
            resources.ApplyResources(tpPushbullet, "tpPushbullet");
            tpPushbullet.Name = "tpPushbullet";
            // 
            // lblPushbulletDevices
            // 
            resources.ApplyResources(lblPushbulletDevices, "lblPushbulletDevices");
            lblPushbulletDevices.Name = "lblPushbulletDevices";
            // 
            // cbPushbulletDevices
            // 
            cbPushbulletDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(cbPushbulletDevices, "cbPushbulletDevices");
            cbPushbulletDevices.FormattingEnabled = true;
            cbPushbulletDevices.Name = "cbPushbulletDevices";
            cbPushbulletDevices.SelectedIndexChanged += cbPushbulletDevices_SelectedIndexChanged;
            // 
            // btnPushbulletGetDeviceList
            // 
            resources.ApplyResources(btnPushbulletGetDeviceList, "btnPushbulletGetDeviceList");
            btnPushbulletGetDeviceList.Name = "btnPushbulletGetDeviceList";
            btnPushbulletGetDeviceList.UseVisualStyleBackColor = true;
            btnPushbulletGetDeviceList.Click += btnPushbulletGetDeviceList_Click;
            // 
            // lblPushbulletUserKey
            // 
            resources.ApplyResources(lblPushbulletUserKey, "lblPushbulletUserKey");
            lblPushbulletUserKey.Name = "lblPushbulletUserKey";
            // 
            // txtPushbulletUserKey
            // 
            resources.ApplyResources(txtPushbulletUserKey, "txtPushbulletUserKey");
            txtPushbulletUserKey.Name = "txtPushbulletUserKey";
            txtPushbulletUserKey.UseSystemPasswordChar = true;
            txtPushbulletUserKey.TextChanged += txtPushbulletUserKey_TextChanged;
            // 
            // tpSendSpace
            // 
            tpSendSpace.BackColor = System.Drawing.SystemColors.Window;
            tpSendSpace.Controls.Add(btnSendSpaceRegister);
            tpSendSpace.Controls.Add(lblSendSpacePassword);
            tpSendSpace.Controls.Add(lblSendSpaceUsername);
            tpSendSpace.Controls.Add(txtSendSpacePassword);
            tpSendSpace.Controls.Add(txtSendSpaceUserName);
            tpSendSpace.Controls.Add(atcSendSpaceAccountType);
            resources.ApplyResources(tpSendSpace, "tpSendSpace");
            tpSendSpace.Name = "tpSendSpace";
            // 
            // btnSendSpaceRegister
            // 
            resources.ApplyResources(btnSendSpaceRegister, "btnSendSpaceRegister");
            btnSendSpaceRegister.Name = "btnSendSpaceRegister";
            btnSendSpaceRegister.UseVisualStyleBackColor = true;
            btnSendSpaceRegister.Click += btnSendSpaceRegister_Click;
            // 
            // lblSendSpacePassword
            // 
            resources.ApplyResources(lblSendSpacePassword, "lblSendSpacePassword");
            lblSendSpacePassword.Name = "lblSendSpacePassword";
            // 
            // lblSendSpaceUsername
            // 
            resources.ApplyResources(lblSendSpaceUsername, "lblSendSpaceUsername");
            lblSendSpaceUsername.Name = "lblSendSpaceUsername";
            // 
            // txtSendSpacePassword
            // 
            resources.ApplyResources(txtSendSpacePassword, "txtSendSpacePassword");
            txtSendSpacePassword.Name = "txtSendSpacePassword";
            txtSendSpacePassword.UseSystemPasswordChar = true;
            txtSendSpacePassword.TextChanged += txtSendSpacePassword_TextChanged;
            // 
            // txtSendSpaceUserName
            // 
            resources.ApplyResources(txtSendSpaceUserName, "txtSendSpaceUserName");
            txtSendSpaceUserName.Name = "txtSendSpaceUserName";
            txtSendSpaceUserName.TextChanged += txtSendSpaceUserName_TextChanged;
            // 
            // atcSendSpaceAccountType
            // 
            resources.ApplyResources(atcSendSpaceAccountType, "atcSendSpaceAccountType");
            atcSendSpaceAccountType.Name = "atcSendSpaceAccountType";
            atcSendSpaceAccountType.SelectedAccountType = AccountType.Anonymous;
            atcSendSpaceAccountType.AccountTypeChanged += atcSendSpaceAccountType_AccountTypeChanged;
            // 
            // tpHostr
            // 
            tpHostr.BackColor = System.Drawing.SystemColors.Window;
            tpHostr.Controls.Add(cbLocalhostrDirectURL);
            tpHostr.Controls.Add(lblLocalhostrPassword);
            tpHostr.Controls.Add(lblLocalhostrEmail);
            tpHostr.Controls.Add(txtLocalhostrPassword);
            tpHostr.Controls.Add(txtLocalhostrEmail);
            resources.ApplyResources(tpHostr, "tpHostr");
            tpHostr.Name = "tpHostr";
            // 
            // cbLocalhostrDirectURL
            // 
            resources.ApplyResources(cbLocalhostrDirectURL, "cbLocalhostrDirectURL");
            cbLocalhostrDirectURL.Name = "cbLocalhostrDirectURL";
            cbLocalhostrDirectURL.UseVisualStyleBackColor = true;
            cbLocalhostrDirectURL.CheckedChanged += cbLocalhostrDirectURL_CheckedChanged;
            // 
            // lblLocalhostrPassword
            // 
            resources.ApplyResources(lblLocalhostrPassword, "lblLocalhostrPassword");
            lblLocalhostrPassword.Name = "lblLocalhostrPassword";
            // 
            // lblLocalhostrEmail
            // 
            resources.ApplyResources(lblLocalhostrEmail, "lblLocalhostrEmail");
            lblLocalhostrEmail.Name = "lblLocalhostrEmail";
            // 
            // txtLocalhostrPassword
            // 
            resources.ApplyResources(txtLocalhostrPassword, "txtLocalhostrPassword");
            txtLocalhostrPassword.Name = "txtLocalhostrPassword";
            txtLocalhostrPassword.UseSystemPasswordChar = true;
            txtLocalhostrPassword.TextChanged += txtLocalhostrPassword_TextChanged;
            // 
            // txtLocalhostrEmail
            // 
            resources.ApplyResources(txtLocalhostrEmail, "txtLocalhostrEmail");
            txtLocalhostrEmail.Name = "txtLocalhostrEmail";
            txtLocalhostrEmail.TextChanged += txtLocalhostrEmail_TextChanged;
            // 
            // tpLambda
            // 
            tpLambda.BackColor = System.Drawing.SystemColors.Window;
            tpLambda.Controls.Add(lblLambdaInfo);
            tpLambda.Controls.Add(lblLambdaApiKey);
            tpLambda.Controls.Add(txtLambdaApiKey);
            tpLambda.Controls.Add(lblLambdaUploadURL);
            tpLambda.Controls.Add(cbLambdaUploadURL);
            resources.ApplyResources(tpLambda, "tpLambda");
            tpLambda.Name = "tpLambda";
            // 
            // lblLambdaInfo
            // 
            resources.ApplyResources(lblLambdaInfo, "lblLambdaInfo");
            lblLambdaInfo.Name = "lblLambdaInfo";
            lblLambdaInfo.Click += lambdaInfoLabel_Click;
            // 
            // lblLambdaApiKey
            // 
            resources.ApplyResources(lblLambdaApiKey, "lblLambdaApiKey");
            lblLambdaApiKey.Name = "lblLambdaApiKey";
            // 
            // txtLambdaApiKey
            // 
            resources.ApplyResources(txtLambdaApiKey, "txtLambdaApiKey");
            txtLambdaApiKey.Name = "txtLambdaApiKey";
            txtLambdaApiKey.UseSystemPasswordChar = true;
            txtLambdaApiKey.TextChanged += txtLambdaApiKey_TextChanged;
            // 
            // lblLambdaUploadURL
            // 
            resources.ApplyResources(lblLambdaUploadURL, "lblLambdaUploadURL");
            lblLambdaUploadURL.Name = "lblLambdaUploadURL";
            // 
            // cbLambdaUploadURL
            // 
            cbLambdaUploadURL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbLambdaUploadURL.FormattingEnabled = true;
            resources.ApplyResources(cbLambdaUploadURL, "cbLambdaUploadURL");
            cbLambdaUploadURL.Name = "cbLambdaUploadURL";
            cbLambdaUploadURL.SelectedIndexChanged += cbLambdaUploadURL_SelectedIndexChanged;
            // 
            // tpPomf
            // 
            tpPomf.BackColor = System.Drawing.SystemColors.Window;
            tpPomf.Controls.Add(txtPomfResultURL);
            tpPomf.Controls.Add(txtPomfUploadURL);
            tpPomf.Controls.Add(lblPomfResultURL);
            tpPomf.Controls.Add(lblPomfUploadURL);
            resources.ApplyResources(tpPomf, "tpPomf");
            tpPomf.Name = "tpPomf";
            // 
            // txtPomfResultURL
            // 
            resources.ApplyResources(txtPomfResultURL, "txtPomfResultURL");
            txtPomfResultURL.Name = "txtPomfResultURL";
            txtPomfResultURL.TextChanged += txtPomfResultURL_TextChanged;
            // 
            // txtPomfUploadURL
            // 
            resources.ApplyResources(txtPomfUploadURL, "txtPomfUploadURL");
            txtPomfUploadURL.Name = "txtPomfUploadURL";
            txtPomfUploadURL.TextChanged += txtPomfUploadURL_TextChanged;
            // 
            // lblPomfResultURL
            // 
            resources.ApplyResources(lblPomfResultURL, "lblPomfResultURL");
            lblPomfResultURL.Name = "lblPomfResultURL";
            // 
            // lblPomfUploadURL
            // 
            resources.ApplyResources(lblPomfUploadURL, "lblPomfUploadURL");
            lblPomfUploadURL.Name = "lblPomfUploadURL";
            // 
            // tpSeafile
            // 
            tpSeafile.BackColor = System.Drawing.SystemColors.Window;
            tpSeafile.Controls.Add(cbSeafileAPIURL);
            tpSeafile.Controls.Add(grpSeafileShareSettings);
            tpSeafile.Controls.Add(btnSeafileLibraryPasswordValidate);
            tpSeafile.Controls.Add(txtSeafileLibraryPassword);
            tpSeafile.Controls.Add(lblSeafileLibraryPassword);
            tpSeafile.Controls.Add(lvSeafileLibraries);
            tpSeafile.Controls.Add(btnSeafilePathValidate);
            tpSeafile.Controls.Add(txtSeafileDirectoryPath);
            tpSeafile.Controls.Add(lblSeafileWritePermNotif);
            tpSeafile.Controls.Add(lblSeafilePath);
            tpSeafile.Controls.Add(txtSeafileUploadLocationRefresh);
            tpSeafile.Controls.Add(lblSeafileSelectLibrary);
            tpSeafile.Controls.Add(grpSeafileAccInfo);
            tpSeafile.Controls.Add(btnSeafileCheckAuthToken);
            tpSeafile.Controls.Add(btnSeafileCheckAPIURL);
            tpSeafile.Controls.Add(grpSeafileObtainAuthToken);
            tpSeafile.Controls.Add(cbSeafileCreateShareableURL);
            tpSeafile.Controls.Add(cbSeafileCreateShareableURLRaw);
            tpSeafile.Controls.Add(txtSeafileAuthToken);
            tpSeafile.Controls.Add(lblSeafileAuthToken);
            tpSeafile.Controls.Add(lblSeafileAPIURL);
            resources.ApplyResources(tpSeafile, "tpSeafile");
            tpSeafile.Name = "tpSeafile";
            // 
            // cbSeafileAPIURL
            // 
            cbSeafileAPIURL.FormattingEnabled = true;
            cbSeafileAPIURL.Items.AddRange(new object[] { resources.GetString("cbSeafileAPIURL.Items"), resources.GetString("cbSeafileAPIURL.Items1") });
            resources.ApplyResources(cbSeafileAPIURL, "cbSeafileAPIURL");
            cbSeafileAPIURL.Name = "cbSeafileAPIURL";
            cbSeafileAPIURL.TextChanged += cbSeafileAPIURL_TextChanged;
            // 
            // grpSeafileShareSettings
            // 
            grpSeafileShareSettings.Controls.Add(txtSeafileSharePassword);
            grpSeafileShareSettings.Controls.Add(lblSeafileSharePassword);
            grpSeafileShareSettings.Controls.Add(nudSeafileExpireDays);
            grpSeafileShareSettings.Controls.Add(lblSeafileDaysToExpire);
            resources.ApplyResources(grpSeafileShareSettings, "grpSeafileShareSettings");
            grpSeafileShareSettings.Name = "grpSeafileShareSettings";
            grpSeafileShareSettings.TabStop = false;
            // 
            // txtSeafileSharePassword
            // 
            resources.ApplyResources(txtSeafileSharePassword, "txtSeafileSharePassword");
            txtSeafileSharePassword.Name = "txtSeafileSharePassword";
            txtSeafileSharePassword.UseSystemPasswordChar = true;
            txtSeafileSharePassword.TextChanged += txtSeafileSharePassword_TextChanged;
            // 
            // lblSeafileSharePassword
            // 
            resources.ApplyResources(lblSeafileSharePassword, "lblSeafileSharePassword");
            lblSeafileSharePassword.Name = "lblSeafileSharePassword";
            // 
            // nudSeafileExpireDays
            // 
            resources.ApplyResources(nudSeafileExpireDays, "nudSeafileExpireDays");
            nudSeafileExpireDays.Maximum = new decimal(new int[] { 900, 0, 0, 0 });
            nudSeafileExpireDays.Name = "nudSeafileExpireDays";
            nudSeafileExpireDays.Value = new decimal(new int[] { 7, 0, 0, 0 });
            nudSeafileExpireDays.ValueChanged += nudSeafileExpireDays_ValueChanged;
            // 
            // lblSeafileDaysToExpire
            // 
            resources.ApplyResources(lblSeafileDaysToExpire, "lblSeafileDaysToExpire");
            lblSeafileDaysToExpire.Name = "lblSeafileDaysToExpire";
            // 
            // btnSeafileLibraryPasswordValidate
            // 
            resources.ApplyResources(btnSeafileLibraryPasswordValidate, "btnSeafileLibraryPasswordValidate");
            btnSeafileLibraryPasswordValidate.Name = "btnSeafileLibraryPasswordValidate";
            btnSeafileLibraryPasswordValidate.UseVisualStyleBackColor = true;
            btnSeafileLibraryPasswordValidate.Click += btnSeafileLibraryPasswordValidate_Click;
            // 
            // txtSeafileLibraryPassword
            // 
            resources.ApplyResources(txtSeafileLibraryPassword, "txtSeafileLibraryPassword");
            txtSeafileLibraryPassword.Name = "txtSeafileLibraryPassword";
            txtSeafileLibraryPassword.UseSystemPasswordChar = true;
            txtSeafileLibraryPassword.TextChanged += txtSeafileLibraryPassword_TextChanged;
            // 
            // lblSeafileLibraryPassword
            // 
            resources.ApplyResources(lblSeafileLibraryPassword, "lblSeafileLibraryPassword");
            lblSeafileLibraryPassword.Name = "lblSeafileLibraryPassword";
            // 
            // lvSeafileLibraries
            // 
            lvSeafileLibraries.AllowColumnSort = true;
            lvSeafileLibraries.AutoFillColumn = true;
            lvSeafileLibraries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colSeafileLibraryName, colSeafileLibrarySize, colSeafileLibraryEncrypted });
            lvSeafileLibraries.DisableDeselect = true;
            lvSeafileLibraries.FullRowSelect = true;
            resources.ApplyResources(lvSeafileLibraries, "lvSeafileLibraries");
            lvSeafileLibraries.Name = "lvSeafileLibraries";
            lvSeafileLibraries.UseCompatibleStateImageBehavior = false;
            lvSeafileLibraries.View = System.Windows.Forms.View.Details;
            lvSeafileLibraries.SelectedIndexChanged += lvSeafileLibraries_SelectedIndexChanged;
            // 
            // colSeafileLibraryName
            // 
            resources.ApplyResources(colSeafileLibraryName, "colSeafileLibraryName");
            // 
            // colSeafileLibrarySize
            // 
            resources.ApplyResources(colSeafileLibrarySize, "colSeafileLibrarySize");
            // 
            // colSeafileLibraryEncrypted
            // 
            resources.ApplyResources(colSeafileLibraryEncrypted, "colSeafileLibraryEncrypted");
            // 
            // btnSeafilePathValidate
            // 
            resources.ApplyResources(btnSeafilePathValidate, "btnSeafilePathValidate");
            btnSeafilePathValidate.Name = "btnSeafilePathValidate";
            btnSeafilePathValidate.UseVisualStyleBackColor = true;
            btnSeafilePathValidate.Click += btnSeafilePathValidate_Click;
            // 
            // txtSeafileDirectoryPath
            // 
            resources.ApplyResources(txtSeafileDirectoryPath, "txtSeafileDirectoryPath");
            txtSeafileDirectoryPath.Name = "txtSeafileDirectoryPath";
            txtSeafileDirectoryPath.TextChanged += txtSeafileDirectoryPath_TextChanged;
            // 
            // lblSeafileWritePermNotif
            // 
            resources.ApplyResources(lblSeafileWritePermNotif, "lblSeafileWritePermNotif");
            lblSeafileWritePermNotif.Name = "lblSeafileWritePermNotif";
            // 
            // lblSeafilePath
            // 
            resources.ApplyResources(lblSeafilePath, "lblSeafilePath");
            lblSeafilePath.Name = "lblSeafilePath";
            // 
            // txtSeafileUploadLocationRefresh
            // 
            resources.ApplyResources(txtSeafileUploadLocationRefresh, "txtSeafileUploadLocationRefresh");
            txtSeafileUploadLocationRefresh.Name = "txtSeafileUploadLocationRefresh";
            txtSeafileUploadLocationRefresh.UseVisualStyleBackColor = true;
            txtSeafileUploadLocationRefresh.Click += txtSeafileUploadLocationRefresh_Click;
            // 
            // lblSeafileSelectLibrary
            // 
            resources.ApplyResources(lblSeafileSelectLibrary, "lblSeafileSelectLibrary");
            lblSeafileSelectLibrary.Name = "lblSeafileSelectLibrary";
            // 
            // grpSeafileAccInfo
            // 
            grpSeafileAccInfo.Controls.Add(btnRefreshSeafileAccInfo);
            grpSeafileAccInfo.Controls.Add(txtSeafileAccInfoUsage);
            grpSeafileAccInfo.Controls.Add(txtSeafileAccInfoEmail);
            grpSeafileAccInfo.Controls.Add(lblSeafileAccInfoEmail);
            grpSeafileAccInfo.Controls.Add(lblSeafileAccInfoUsage);
            resources.ApplyResources(grpSeafileAccInfo, "grpSeafileAccInfo");
            grpSeafileAccInfo.Name = "grpSeafileAccInfo";
            grpSeafileAccInfo.TabStop = false;
            // 
            // btnRefreshSeafileAccInfo
            // 
            resources.ApplyResources(btnRefreshSeafileAccInfo, "btnRefreshSeafileAccInfo");
            btnRefreshSeafileAccInfo.Name = "btnRefreshSeafileAccInfo";
            btnRefreshSeafileAccInfo.UseVisualStyleBackColor = true;
            btnRefreshSeafileAccInfo.Click += btnRefreshSeafileAccInfo_Click;
            // 
            // txtSeafileAccInfoUsage
            // 
            resources.ApplyResources(txtSeafileAccInfoUsage, "txtSeafileAccInfoUsage");
            txtSeafileAccInfoUsage.Name = "txtSeafileAccInfoUsage";
            txtSeafileAccInfoUsage.ReadOnly = true;
            // 
            // txtSeafileAccInfoEmail
            // 
            resources.ApplyResources(txtSeafileAccInfoEmail, "txtSeafileAccInfoEmail");
            txtSeafileAccInfoEmail.Name = "txtSeafileAccInfoEmail";
            txtSeafileAccInfoEmail.ReadOnly = true;
            // 
            // lblSeafileAccInfoEmail
            // 
            resources.ApplyResources(lblSeafileAccInfoEmail, "lblSeafileAccInfoEmail");
            lblSeafileAccInfoEmail.Name = "lblSeafileAccInfoEmail";
            // 
            // lblSeafileAccInfoUsage
            // 
            resources.ApplyResources(lblSeafileAccInfoUsage, "lblSeafileAccInfoUsage");
            lblSeafileAccInfoUsage.Name = "lblSeafileAccInfoUsage";
            // 
            // btnSeafileCheckAuthToken
            // 
            resources.ApplyResources(btnSeafileCheckAuthToken, "btnSeafileCheckAuthToken");
            btnSeafileCheckAuthToken.Name = "btnSeafileCheckAuthToken";
            btnSeafileCheckAuthToken.UseVisualStyleBackColor = true;
            btnSeafileCheckAuthToken.Click += btnSeafileCheckAuthToken_Click;
            // 
            // btnSeafileCheckAPIURL
            // 
            resources.ApplyResources(btnSeafileCheckAPIURL, "btnSeafileCheckAPIURL");
            btnSeafileCheckAPIURL.Name = "btnSeafileCheckAPIURL";
            btnSeafileCheckAPIURL.UseVisualStyleBackColor = true;
            btnSeafileCheckAPIURL.Click += btnSeafileCheckAPIURL_Click;
            // 
            // grpSeafileObtainAuthToken
            // 
            grpSeafileObtainAuthToken.Controls.Add(btnSeafileGetAuthToken);
            grpSeafileObtainAuthToken.Controls.Add(txtSeafilePassword);
            grpSeafileObtainAuthToken.Controls.Add(txtSeafileUsername);
            grpSeafileObtainAuthToken.Controls.Add(lblSeafileUsername);
            grpSeafileObtainAuthToken.Controls.Add(lblSeafilePassword);
            resources.ApplyResources(grpSeafileObtainAuthToken, "grpSeafileObtainAuthToken");
            grpSeafileObtainAuthToken.Name = "grpSeafileObtainAuthToken";
            grpSeafileObtainAuthToken.TabStop = false;
            // 
            // btnSeafileGetAuthToken
            // 
            resources.ApplyResources(btnSeafileGetAuthToken, "btnSeafileGetAuthToken");
            btnSeafileGetAuthToken.Name = "btnSeafileGetAuthToken";
            btnSeafileGetAuthToken.UseVisualStyleBackColor = true;
            btnSeafileGetAuthToken.Click += btnSeafileGetAuthToken_Click;
            // 
            // txtSeafilePassword
            // 
            resources.ApplyResources(txtSeafilePassword, "txtSeafilePassword");
            txtSeafilePassword.Name = "txtSeafilePassword";
            txtSeafilePassword.UseSystemPasswordChar = true;
            txtSeafilePassword.KeyUp += txtSeafilePassword_KeyUp;
            // 
            // txtSeafileUsername
            // 
            resources.ApplyResources(txtSeafileUsername, "txtSeafileUsername");
            txtSeafileUsername.Name = "txtSeafileUsername";
            // 
            // lblSeafileUsername
            // 
            resources.ApplyResources(lblSeafileUsername, "lblSeafileUsername");
            lblSeafileUsername.Name = "lblSeafileUsername";
            // 
            // lblSeafilePassword
            // 
            resources.ApplyResources(lblSeafilePassword, "lblSeafilePassword");
            lblSeafilePassword.Name = "lblSeafilePassword";
            // 
            // cbSeafileCreateShareableURL
            // 
            resources.ApplyResources(cbSeafileCreateShareableURL, "cbSeafileCreateShareableURL");
            cbSeafileCreateShareableURL.Name = "cbSeafileCreateShareableURL";
            cbSeafileCreateShareableURL.UseVisualStyleBackColor = true;
            cbSeafileCreateShareableURL.CheckedChanged += cbSeafileCreateShareableURL_CheckedChanged;
            // 
            // cbSeafileCreateShareableURLRaw
            // 
            resources.ApplyResources(cbSeafileCreateShareableURLRaw, "cbSeafileCreateShareableURLRaw");
            cbSeafileCreateShareableURLRaw.Name = "cbSeafileCreateShareableURLRaw";
            cbSeafileCreateShareableURLRaw.UseVisualStyleBackColor = true;
            cbSeafileCreateShareableURLRaw.CheckedChanged += cbSeafileCreateShareableURLRaw_CheckedChanged;
            // 
            // txtSeafileAuthToken
            // 
            resources.ApplyResources(txtSeafileAuthToken, "txtSeafileAuthToken");
            txtSeafileAuthToken.Name = "txtSeafileAuthToken";
            txtSeafileAuthToken.UseSystemPasswordChar = true;
            txtSeafileAuthToken.TextChanged += txtSeafileAuthToken_TextChanged;
            // 
            // lblSeafileAuthToken
            // 
            resources.ApplyResources(lblSeafileAuthToken, "lblSeafileAuthToken");
            lblSeafileAuthToken.Name = "lblSeafileAuthToken";
            // 
            // lblSeafileAPIURL
            // 
            resources.ApplyResources(lblSeafileAPIURL, "lblSeafileAPIURL");
            lblSeafileAPIURL.Name = "lblSeafileAPIURL";
            // 
            // tpStreamable
            // 
            tpStreamable.BackColor = System.Drawing.SystemColors.Window;
            tpStreamable.Controls.Add(cbStreamableUseDirectURL);
            tpStreamable.Controls.Add(txtStreamablePassword);
            tpStreamable.Controls.Add(txtStreamableUsername);
            tpStreamable.Controls.Add(lblStreamableUsername);
            tpStreamable.Controls.Add(lblStreamablePassword);
            resources.ApplyResources(tpStreamable, "tpStreamable");
            tpStreamable.Name = "tpStreamable";
            // 
            // cbStreamableUseDirectURL
            // 
            resources.ApplyResources(cbStreamableUseDirectURL, "cbStreamableUseDirectURL");
            cbStreamableUseDirectURL.Name = "cbStreamableUseDirectURL";
            cbStreamableUseDirectURL.UseVisualStyleBackColor = true;
            cbStreamableUseDirectURL.CheckedChanged += cbStreamableUseDirectURL_CheckedChanged;
            // 
            // txtStreamablePassword
            // 
            resources.ApplyResources(txtStreamablePassword, "txtStreamablePassword");
            txtStreamablePassword.Name = "txtStreamablePassword";
            txtStreamablePassword.UseSystemPasswordChar = true;
            txtStreamablePassword.TextChanged += txtStreamablePassword_TextChanged;
            // 
            // txtStreamableUsername
            // 
            resources.ApplyResources(txtStreamableUsername, "txtStreamableUsername");
            txtStreamableUsername.Name = "txtStreamableUsername";
            txtStreamableUsername.TextChanged += txtStreamableUsername_TextChanged;
            // 
            // lblStreamableUsername
            // 
            resources.ApplyResources(lblStreamableUsername, "lblStreamableUsername");
            lblStreamableUsername.Name = "lblStreamableUsername";
            // 
            // lblStreamablePassword
            // 
            resources.ApplyResources(lblStreamablePassword, "lblStreamablePassword");
            lblStreamablePassword.Name = "lblStreamablePassword";
            // 
            // tpSul
            // 
            tpSul.BackColor = System.Drawing.SystemColors.Window;
            tpSul.Controls.Add(btnSulGetAPIKey);
            tpSul.Controls.Add(txtSulAPIKey);
            tpSul.Controls.Add(lblSulAPIKey);
            resources.ApplyResources(tpSul, "tpSul");
            tpSul.Name = "tpSul";
            // 
            // btnSulGetAPIKey
            // 
            resources.ApplyResources(btnSulGetAPIKey, "btnSulGetAPIKey");
            btnSulGetAPIKey.Name = "btnSulGetAPIKey";
            btnSulGetAPIKey.UseVisualStyleBackColor = true;
            btnSulGetAPIKey.Click += btnSulGetAPIKey_Click;
            // 
            // txtSulAPIKey
            // 
            resources.ApplyResources(txtSulAPIKey, "txtSulAPIKey");
            txtSulAPIKey.Name = "txtSulAPIKey";
            txtSulAPIKey.UseSystemPasswordChar = true;
            txtSulAPIKey.TextChanged += txtSulAPIKey_TextChanged;
            // 
            // lblSulAPIKey
            // 
            resources.ApplyResources(lblSulAPIKey, "lblSulAPIKey");
            lblSulAPIKey.Name = "lblSulAPIKey";
            // 
            // tpLithiio
            // 
            tpLithiio.BackColor = System.Drawing.SystemColors.Window;
            tpLithiio.Controls.Add(btnLithiioFetchAPIKey);
            tpLithiio.Controls.Add(txtLithiioPassword);
            tpLithiio.Controls.Add(txtLithiioEmail);
            tpLithiio.Controls.Add(lblLithiioPassword);
            tpLithiio.Controls.Add(lblLithiioEmail);
            tpLithiio.Controls.Add(btnLithiioGetAPIKey);
            tpLithiio.Controls.Add(lblLithiioApiKey);
            tpLithiio.Controls.Add(txtLithiioApiKey);
            resources.ApplyResources(tpLithiio, "tpLithiio");
            tpLithiio.Name = "tpLithiio";
            // 
            // btnLithiioFetchAPIKey
            // 
            resources.ApplyResources(btnLithiioFetchAPIKey, "btnLithiioFetchAPIKey");
            btnLithiioFetchAPIKey.Name = "btnLithiioFetchAPIKey";
            btnLithiioFetchAPIKey.UseVisualStyleBackColor = true;
            btnLithiioFetchAPIKey.Click += btnLithiioLogin_Click;
            // 
            // txtLithiioPassword
            // 
            resources.ApplyResources(txtLithiioPassword, "txtLithiioPassword");
            txtLithiioPassword.Name = "txtLithiioPassword";
            txtLithiioPassword.UseSystemPasswordChar = true;
            // 
            // txtLithiioEmail
            // 
            resources.ApplyResources(txtLithiioEmail, "txtLithiioEmail");
            txtLithiioEmail.Name = "txtLithiioEmail";
            // 
            // lblLithiioPassword
            // 
            resources.ApplyResources(lblLithiioPassword, "lblLithiioPassword");
            lblLithiioPassword.Name = "lblLithiioPassword";
            // 
            // lblLithiioEmail
            // 
            resources.ApplyResources(lblLithiioEmail, "lblLithiioEmail");
            lblLithiioEmail.Name = "lblLithiioEmail";
            // 
            // btnLithiioGetAPIKey
            // 
            resources.ApplyResources(btnLithiioGetAPIKey, "btnLithiioGetAPIKey");
            btnLithiioGetAPIKey.Name = "btnLithiioGetAPIKey";
            btnLithiioGetAPIKey.UseVisualStyleBackColor = true;
            btnLithiioGetAPIKey.Click += btnLithiioGetAPIKey_Click;
            // 
            // lblLithiioApiKey
            // 
            resources.ApplyResources(lblLithiioApiKey, "lblLithiioApiKey");
            lblLithiioApiKey.Name = "lblLithiioApiKey";
            // 
            // txtLithiioApiKey
            // 
            resources.ApplyResources(txtLithiioApiKey, "txtLithiioApiKey");
            txtLithiioApiKey.Name = "txtLithiioApiKey";
            txtLithiioApiKey.UseSystemPasswordChar = true;
            txtLithiioApiKey.TextChanged += txtLithiioApiKey_TextChanged;
            // 
            // tpPlik
            // 
            tpPlik.BackColor = System.Drawing.SystemColors.Window;
            tpPlik.Controls.Add(gbPlikSettings);
            tpPlik.Controls.Add(gbPlikLoginCredentials);
            resources.ApplyResources(tpPlik, "tpPlik");
            tpPlik.Name = "tpPlik";
            // 
            // gbPlikSettings
            // 
            gbPlikSettings.Controls.Add(cbPlikOneShot);
            gbPlikSettings.Controls.Add(txtPlikComment);
            gbPlikSettings.Controls.Add(cbPlikComment);
            gbPlikSettings.Controls.Add(cbPlikRemovable);
            resources.ApplyResources(gbPlikSettings, "gbPlikSettings");
            gbPlikSettings.Name = "gbPlikSettings";
            gbPlikSettings.TabStop = false;
            // 
            // cbPlikOneShot
            // 
            resources.ApplyResources(cbPlikOneShot, "cbPlikOneShot");
            cbPlikOneShot.Name = "cbPlikOneShot";
            cbPlikOneShot.UseVisualStyleBackColor = true;
            cbPlikOneShot.CheckedChanged += cbPlikOneShot_CheckedChanged;
            // 
            // txtPlikComment
            // 
            resources.ApplyResources(txtPlikComment, "txtPlikComment");
            txtPlikComment.Name = "txtPlikComment";
            txtPlikComment.ReadOnly = true;
            txtPlikComment.TextChanged += txtPlikComment_TextChanged;
            // 
            // cbPlikComment
            // 
            resources.ApplyResources(cbPlikComment, "cbPlikComment");
            cbPlikComment.Name = "cbPlikComment";
            cbPlikComment.UseVisualStyleBackColor = true;
            cbPlikComment.CheckedChanged += cbPlikComment_CheckedChanged;
            // 
            // cbPlikRemovable
            // 
            resources.ApplyResources(cbPlikRemovable, "cbPlikRemovable");
            cbPlikRemovable.Name = "cbPlikRemovable";
            cbPlikRemovable.UseVisualStyleBackColor = true;
            cbPlikRemovable.CheckedChanged += cbPlikRemovable_CheckedChanged;
            // 
            // gbPlikLoginCredentials
            // 
            gbPlikLoginCredentials.Controls.Add(nudPlikTTL);
            gbPlikLoginCredentials.Controls.Add(cbPlikTTLUnit);
            gbPlikLoginCredentials.Controls.Add(lblPlikTTL);
            gbPlikLoginCredentials.Controls.Add(txtPlikURL);
            gbPlikLoginCredentials.Controls.Add(lblPlikURL);
            gbPlikLoginCredentials.Controls.Add(cbPlikIsSecured);
            gbPlikLoginCredentials.Controls.Add(lblPlikAPIKey);
            gbPlikLoginCredentials.Controls.Add(txtPlikAPIKey);
            gbPlikLoginCredentials.Controls.Add(lblPlikPassword);
            gbPlikLoginCredentials.Controls.Add(lblPlikUsername);
            gbPlikLoginCredentials.Controls.Add(txtPlikPassword);
            gbPlikLoginCredentials.Controls.Add(txtPlikLogin);
            resources.ApplyResources(gbPlikLoginCredentials, "gbPlikLoginCredentials");
            gbPlikLoginCredentials.Name = "gbPlikLoginCredentials";
            gbPlikLoginCredentials.TabStop = false;
            // 
            // nudPlikTTL
            // 
            nudPlikTTL.DecimalPlaces = 2;
            resources.ApplyResources(nudPlikTTL, "nudPlikTTL");
            nudPlikTTL.Maximum = new decimal(new int[] { 1661992960, 1808227885, 5, 0 });
            nudPlikTTL.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            nudPlikTTL.Name = "nudPlikTTL";
            nudPlikTTL.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudPlikTTL.ValueChanged += nudPlikTTL_ValueChanged;
            // 
            // cbPlikTTLUnit
            // 
            cbPlikTTLUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPlikTTLUnit.FormattingEnabled = true;
            cbPlikTTLUnit.Items.AddRange(new object[] { resources.GetString("cbPlikTTLUnit.Items"), resources.GetString("cbPlikTTLUnit.Items1"), resources.GetString("cbPlikTTLUnit.Items2"), resources.GetString("cbPlikTTLUnit.Items3") });
            resources.ApplyResources(cbPlikTTLUnit, "cbPlikTTLUnit");
            cbPlikTTLUnit.Name = "cbPlikTTLUnit";
            cbPlikTTLUnit.SelectedIndexChanged += cbPlikTTLUnit_SelectedIndexChanged;
            // 
            // lblPlikTTL
            // 
            resources.ApplyResources(lblPlikTTL, "lblPlikTTL");
            lblPlikTTL.Name = "lblPlikTTL";
            // 
            // txtPlikURL
            // 
            resources.ApplyResources(txtPlikURL, "txtPlikURL");
            txtPlikURL.Name = "txtPlikURL";
            txtPlikURL.TextChanged += txtPlikURL_TextChanged;
            // 
            // lblPlikURL
            // 
            resources.ApplyResources(lblPlikURL, "lblPlikURL");
            lblPlikURL.Name = "lblPlikURL";
            // 
            // cbPlikIsSecured
            // 
            resources.ApplyResources(cbPlikIsSecured, "cbPlikIsSecured");
            cbPlikIsSecured.Name = "cbPlikIsSecured";
            cbPlikIsSecured.UseVisualStyleBackColor = true;
            cbPlikIsSecured.CheckedChanged += cbPlikIsSecured_CheckedChanged;
            // 
            // lblPlikAPIKey
            // 
            resources.ApplyResources(lblPlikAPIKey, "lblPlikAPIKey");
            lblPlikAPIKey.Name = "lblPlikAPIKey";
            // 
            // txtPlikAPIKey
            // 
            resources.ApplyResources(txtPlikAPIKey, "txtPlikAPIKey");
            txtPlikAPIKey.Name = "txtPlikAPIKey";
            txtPlikAPIKey.UseSystemPasswordChar = true;
            txtPlikAPIKey.TextChanged += txtPlikAPIKey_TextChanged;
            // 
            // lblPlikPassword
            // 
            resources.ApplyResources(lblPlikPassword, "lblPlikPassword");
            lblPlikPassword.Name = "lblPlikPassword";
            // 
            // lblPlikUsername
            // 
            resources.ApplyResources(lblPlikUsername, "lblPlikUsername");
            lblPlikUsername.Name = "lblPlikUsername";
            // 
            // txtPlikPassword
            // 
            resources.ApplyResources(txtPlikPassword, "txtPlikPassword");
            txtPlikPassword.Name = "txtPlikPassword";
            txtPlikPassword.UseSystemPasswordChar = true;
            txtPlikPassword.TextChanged += txtPlikPassword_TextChanged;
            // 
            // txtPlikLogin
            // 
            resources.ApplyResources(txtPlikLogin, "txtPlikLogin");
            txtPlikLogin.Name = "txtPlikLogin";
            txtPlikLogin.TextChanged += txtPlikLogin_TextChanged;
            // 
            // tpYouTube
            // 
            tpYouTube.Controls.Add(oauth2YouTube);
            tpYouTube.Controls.Add(llYouTubePermissionsLink);
            tpYouTube.Controls.Add(lblYouTubePermissionsTip);
            tpYouTube.Controls.Add(cbYouTubeShowDialog);
            tpYouTube.Controls.Add(cbYouTubeUseShortenedLink);
            tpYouTube.Controls.Add(cbYouTubePrivacyType);
            tpYouTube.Controls.Add(lblYouTubePrivacyType);
            resources.ApplyResources(tpYouTube, "tpYouTube");
            tpYouTube.Name = "tpYouTube";
            tpYouTube.UseVisualStyleBackColor = true;
            // 
            // oauth2YouTube
            // 
            resources.ApplyResources(oauth2YouTube, "oauth2YouTube");
            oauth2YouTube.Name = "oauth2YouTube";
            oauth2YouTube.ConnectButtonClicked += oauth2YouTube_ConnectButtonClicked;
            oauth2YouTube.DisconnectButtonClicked += oauth2YouTube_DisconnectButtonClicked;
            // 
            // llYouTubePermissionsLink
            // 
            resources.ApplyResources(llYouTubePermissionsLink, "llYouTubePermissionsLink");
            llYouTubePermissionsLink.Name = "llYouTubePermissionsLink";
            llYouTubePermissionsLink.TabStop = true;
            llYouTubePermissionsLink.LinkClicked += llYouTubePermissionsLink_LinkClicked;
            // 
            // lblYouTubePermissionsTip
            // 
            resources.ApplyResources(lblYouTubePermissionsTip, "lblYouTubePermissionsTip");
            lblYouTubePermissionsTip.Name = "lblYouTubePermissionsTip";
            // 
            // cbYouTubeShowDialog
            // 
            resources.ApplyResources(cbYouTubeShowDialog, "cbYouTubeShowDialog");
            cbYouTubeShowDialog.Name = "cbYouTubeShowDialog";
            cbYouTubeShowDialog.UseVisualStyleBackColor = true;
            cbYouTubeShowDialog.CheckedChanged += cbYouTubeShowDialog_CheckedChanged;
            // 
            // cbYouTubeUseShortenedLink
            // 
            resources.ApplyResources(cbYouTubeUseShortenedLink, "cbYouTubeUseShortenedLink");
            cbYouTubeUseShortenedLink.Name = "cbYouTubeUseShortenedLink";
            cbYouTubeUseShortenedLink.UseVisualStyleBackColor = true;
            cbYouTubeUseShortenedLink.CheckedChanged += cbYouTubeUseShortenedLink_CheckedChanged;
            // 
            // cbYouTubePrivacyType
            // 
            cbYouTubePrivacyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbYouTubePrivacyType.FormattingEnabled = true;
            resources.ApplyResources(cbYouTubePrivacyType, "cbYouTubePrivacyType");
            cbYouTubePrivacyType.Name = "cbYouTubePrivacyType";
            cbYouTubePrivacyType.SelectedIndexChanged += cbYouTubePrivacyType_SelectedIndexChanged;
            // 
            // lblYouTubePrivacyType
            // 
            resources.ApplyResources(lblYouTubePrivacyType, "lblYouTubePrivacyType");
            lblYouTubePrivacyType.Name = "lblYouTubePrivacyType";
            // 
            // tpSharedFolder
            // 
            tpSharedFolder.BackColor = System.Drawing.SystemColors.Window;
            tpSharedFolder.Controls.Add(lbSharedFolderAccounts);
            tpSharedFolder.Controls.Add(pgSharedFolderAccount);
            tpSharedFolder.Controls.Add(btnSharedFolderDuplicate);
            tpSharedFolder.Controls.Add(btnSharedFolderRemove);
            tpSharedFolder.Controls.Add(btnSharedFolderAdd);
            tpSharedFolder.Controls.Add(lblSharedFolderFiles);
            tpSharedFolder.Controls.Add(lblSharedFolderText);
            tpSharedFolder.Controls.Add(cbSharedFolderFiles);
            tpSharedFolder.Controls.Add(lblSharedFolderImages);
            tpSharedFolder.Controls.Add(cbSharedFolderText);
            tpSharedFolder.Controls.Add(cbSharedFolderImages);
            resources.ApplyResources(tpSharedFolder, "tpSharedFolder");
            tpSharedFolder.Name = "tpSharedFolder";
            // 
            // lbSharedFolderAccounts
            // 
            lbSharedFolderAccounts.FormattingEnabled = true;
            resources.ApplyResources(lbSharedFolderAccounts, "lbSharedFolderAccounts");
            lbSharedFolderAccounts.Name = "lbSharedFolderAccounts";
            lbSharedFolderAccounts.SelectedIndexChanged += lbSharedFolderAccounts_SelectedIndexChanged;
            // 
            // pgSharedFolderAccount
            // 
            pgSharedFolderAccount.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(pgSharedFolderAccount, "pgSharedFolderAccount");
            pgSharedFolderAccount.Name = "pgSharedFolderAccount";
            pgSharedFolderAccount.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            pgSharedFolderAccount.ToolbarVisible = false;
            pgSharedFolderAccount.PropertyValueChanged += pgSharedFolderAccount_PropertyValueChanged;
            // 
            // btnSharedFolderDuplicate
            // 
            resources.ApplyResources(btnSharedFolderDuplicate, "btnSharedFolderDuplicate");
            btnSharedFolderDuplicate.Name = "btnSharedFolderDuplicate";
            btnSharedFolderDuplicate.UseVisualStyleBackColor = true;
            btnSharedFolderDuplicate.Click += btnSharedFolderDuplicate_Click;
            // 
            // btnSharedFolderRemove
            // 
            resources.ApplyResources(btnSharedFolderRemove, "btnSharedFolderRemove");
            btnSharedFolderRemove.Name = "btnSharedFolderRemove";
            btnSharedFolderRemove.UseVisualStyleBackColor = true;
            btnSharedFolderRemove.Click += btnSharedFolderRemove_Click;
            // 
            // btnSharedFolderAdd
            // 
            resources.ApplyResources(btnSharedFolderAdd, "btnSharedFolderAdd");
            btnSharedFolderAdd.Name = "btnSharedFolderAdd";
            btnSharedFolderAdd.UseVisualStyleBackColor = true;
            btnSharedFolderAdd.Click += btnSharedFolderAdd_Click;
            // 
            // lblSharedFolderFiles
            // 
            resources.ApplyResources(lblSharedFolderFiles, "lblSharedFolderFiles");
            lblSharedFolderFiles.Name = "lblSharedFolderFiles";
            // 
            // lblSharedFolderText
            // 
            resources.ApplyResources(lblSharedFolderText, "lblSharedFolderText");
            lblSharedFolderText.Name = "lblSharedFolderText";
            // 
            // cbSharedFolderFiles
            // 
            cbSharedFolderFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbSharedFolderFiles.FormattingEnabled = true;
            resources.ApplyResources(cbSharedFolderFiles, "cbSharedFolderFiles");
            cbSharedFolderFiles.Name = "cbSharedFolderFiles";
            cbSharedFolderFiles.SelectedIndexChanged += cbSharedFolderFiles_SelectedIndexChanged;
            // 
            // lblSharedFolderImages
            // 
            resources.ApplyResources(lblSharedFolderImages, "lblSharedFolderImages");
            lblSharedFolderImages.Name = "lblSharedFolderImages";
            // 
            // cbSharedFolderText
            // 
            cbSharedFolderText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbSharedFolderText.FormattingEnabled = true;
            resources.ApplyResources(cbSharedFolderText, "cbSharedFolderText");
            cbSharedFolderText.Name = "cbSharedFolderText";
            cbSharedFolderText.SelectedIndexChanged += cbSharedFolderText_SelectedIndexChanged;
            // 
            // cbSharedFolderImages
            // 
            cbSharedFolderImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbSharedFolderImages.FormattingEnabled = true;
            resources.ApplyResources(cbSharedFolderImages, "cbSharedFolderImages");
            cbSharedFolderImages.Name = "cbSharedFolderImages";
            cbSharedFolderImages.SelectedIndexChanged += cbSharedFolderImages_SelectedIndexChanged;
            // 
            // tpEmail
            // 
            tpEmail.BackColor = System.Drawing.SystemColors.Window;
            tpEmail.Controls.Add(txtEmailAutomaticSendTo);
            tpEmail.Controls.Add(cbEmailAutomaticSend);
            tpEmail.Controls.Add(lblEmailSmtpServer);
            tpEmail.Controls.Add(lblEmailPassword);
            tpEmail.Controls.Add(cbEmailRememberLastTo);
            tpEmail.Controls.Add(txtEmailFrom);
            tpEmail.Controls.Add(txtEmailPassword);
            tpEmail.Controls.Add(txtEmailDefaultBody);
            tpEmail.Controls.Add(lblEmailFrom);
            tpEmail.Controls.Add(txtEmailSmtpServer);
            tpEmail.Controls.Add(lblEmailDefaultSubject);
            tpEmail.Controls.Add(lblEmailDefaultBody);
            tpEmail.Controls.Add(nudEmailSmtpPort);
            tpEmail.Controls.Add(lblEmailSmtpPort);
            tpEmail.Controls.Add(txtEmailDefaultSubject);
            resources.ApplyResources(tpEmail, "tpEmail");
            tpEmail.Name = "tpEmail";
            // 
            // txtEmailAutomaticSendTo
            // 
            resources.ApplyResources(txtEmailAutomaticSendTo, "txtEmailAutomaticSendTo");
            txtEmailAutomaticSendTo.Name = "txtEmailAutomaticSendTo";
            txtEmailAutomaticSendTo.TextChanged += txtEmailAutomaticSendTo_TextChanged;
            // 
            // cbEmailAutomaticSend
            // 
            resources.ApplyResources(cbEmailAutomaticSend, "cbEmailAutomaticSend");
            cbEmailAutomaticSend.Name = "cbEmailAutomaticSend";
            cbEmailAutomaticSend.UseVisualStyleBackColor = true;
            cbEmailAutomaticSend.CheckedChanged += cbEmailAutomaticSend_CheckedChanged;
            // 
            // lblEmailSmtpServer
            // 
            resources.ApplyResources(lblEmailSmtpServer, "lblEmailSmtpServer");
            lblEmailSmtpServer.Name = "lblEmailSmtpServer";
            // 
            // lblEmailPassword
            // 
            resources.ApplyResources(lblEmailPassword, "lblEmailPassword");
            lblEmailPassword.Name = "lblEmailPassword";
            // 
            // cbEmailRememberLastTo
            // 
            resources.ApplyResources(cbEmailRememberLastTo, "cbEmailRememberLastTo");
            cbEmailRememberLastTo.Name = "cbEmailRememberLastTo";
            cbEmailRememberLastTo.UseVisualStyleBackColor = true;
            cbEmailRememberLastTo.CheckedChanged += cbRememberLastToEmail_CheckedChanged;
            // 
            // txtEmailFrom
            // 
            resources.ApplyResources(txtEmailFrom, "txtEmailFrom");
            txtEmailFrom.Name = "txtEmailFrom";
            txtEmailFrom.TextChanged += txtEmail_TextChanged;
            // 
            // txtEmailPassword
            // 
            resources.ApplyResources(txtEmailPassword, "txtEmailPassword");
            txtEmailPassword.Name = "txtEmailPassword";
            txtEmailPassword.UseSystemPasswordChar = true;
            txtEmailPassword.TextChanged += txtPassword_TextChanged;
            // 
            // txtEmailDefaultBody
            // 
            resources.ApplyResources(txtEmailDefaultBody, "txtEmailDefaultBody");
            txtEmailDefaultBody.Name = "txtEmailDefaultBody";
            txtEmailDefaultBody.TextChanged += txtDefaultBody_TextChanged;
            // 
            // lblEmailFrom
            // 
            resources.ApplyResources(lblEmailFrom, "lblEmailFrom");
            lblEmailFrom.Name = "lblEmailFrom";
            // 
            // txtEmailSmtpServer
            // 
            resources.ApplyResources(txtEmailSmtpServer, "txtEmailSmtpServer");
            txtEmailSmtpServer.Name = "txtEmailSmtpServer";
            txtEmailSmtpServer.TextChanged += txtSmtpServer_TextChanged;
            // 
            // lblEmailDefaultSubject
            // 
            resources.ApplyResources(lblEmailDefaultSubject, "lblEmailDefaultSubject");
            lblEmailDefaultSubject.Name = "lblEmailDefaultSubject";
            // 
            // lblEmailDefaultBody
            // 
            resources.ApplyResources(lblEmailDefaultBody, "lblEmailDefaultBody");
            lblEmailDefaultBody.Name = "lblEmailDefaultBody";
            // 
            // nudEmailSmtpPort
            // 
            resources.ApplyResources(nudEmailSmtpPort, "nudEmailSmtpPort");
            nudEmailSmtpPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudEmailSmtpPort.Name = "nudEmailSmtpPort";
            nudEmailSmtpPort.Value = new decimal(new int[] { 65535, 0, 0, 0 });
            nudEmailSmtpPort.ValueChanged += nudSmtpPort_ValueChanged;
            // 
            // lblEmailSmtpPort
            // 
            resources.ApplyResources(lblEmailSmtpPort, "lblEmailSmtpPort");
            lblEmailSmtpPort.Name = "lblEmailSmtpPort";
            // 
            // txtEmailDefaultSubject
            // 
            resources.ApplyResources(txtEmailDefaultSubject, "txtEmailDefaultSubject");
            txtEmailDefaultSubject.Name = "txtEmailDefaultSubject";
            txtEmailDefaultSubject.TextChanged += txtDefaultSubject_TextChanged;
            // 
            // btnCopyShowFiles
            // 
            resources.ApplyResources(btnCopyShowFiles, "btnCopyShowFiles");
            btnCopyShowFiles.Name = "btnCopyShowFiles";
            // 
            // tpTextUploaders
            // 
            tpTextUploaders.BackColor = System.Drawing.SystemColors.Window;
            tpTextUploaders.Controls.Add(tcTextUploaders);
            resources.ApplyResources(tpTextUploaders, "tpTextUploaders");
            tpTextUploaders.Name = "tpTextUploaders";
            // 
            // tcTextUploaders
            // 
            tcTextUploaders.Controls.Add(tpPastebin);
            tcTextUploaders.Controls.Add(tpPaste_ee);
            tcTextUploaders.Controls.Add(tpGist);
            tcTextUploaders.Controls.Add(tpUpaste);
            tcTextUploaders.Controls.Add(tpHastebin);
            tcTextUploaders.Controls.Add(tpOneTimeSecret);
            tcTextUploaders.Controls.Add(tpPastie);
            resources.ApplyResources(tcTextUploaders, "tcTextUploaders");
            tcTextUploaders.Name = "tcTextUploaders";
            tcTextUploaders.SelectedIndex = 0;
            // 
            // tpPastebin
            // 
            tpPastebin.BackColor = System.Drawing.SystemColors.Window;
            tpPastebin.Controls.Add(cbPastebinRaw);
            tpPastebin.Controls.Add(cbPastebinSyntax);
            tpPastebin.Controls.Add(btnPastebinRegister);
            tpPastebin.Controls.Add(lblPastebinSyntax);
            tpPastebin.Controls.Add(lblPastebinExpiration);
            tpPastebin.Controls.Add(lblPastebinPrivacy);
            tpPastebin.Controls.Add(lblPastebinTitle);
            tpPastebin.Controls.Add(lblPastebinPassword);
            tpPastebin.Controls.Add(lblPastebinUsername);
            tpPastebin.Controls.Add(cbPastebinExpiration);
            tpPastebin.Controls.Add(cbPastebinPrivacy);
            tpPastebin.Controls.Add(txtPastebinTitle);
            tpPastebin.Controls.Add(txtPastebinPassword);
            tpPastebin.Controls.Add(txtPastebinUsername);
            tpPastebin.Controls.Add(lblPastebinLoginStatus);
            tpPastebin.Controls.Add(btnPastebinLogin);
            resources.ApplyResources(tpPastebin, "tpPastebin");
            tpPastebin.Name = "tpPastebin";
            // 
            // cbPastebinRaw
            // 
            resources.ApplyResources(cbPastebinRaw, "cbPastebinRaw");
            cbPastebinRaw.Name = "cbPastebinRaw";
            cbPastebinRaw.UseVisualStyleBackColor = true;
            cbPastebinRaw.CheckedChanged += cbPastebinRaw_CheckedChanged;
            // 
            // cbPastebinSyntax
            // 
            cbPastebinSyntax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPastebinSyntax.FormattingEnabled = true;
            resources.ApplyResources(cbPastebinSyntax, "cbPastebinSyntax");
            cbPastebinSyntax.Name = "cbPastebinSyntax";
            cbPastebinSyntax.SelectedIndexChanged += cbPastebinSyntax_SelectedIndexChanged;
            // 
            // btnPastebinRegister
            // 
            resources.ApplyResources(btnPastebinRegister, "btnPastebinRegister");
            btnPastebinRegister.Name = "btnPastebinRegister";
            btnPastebinRegister.UseVisualStyleBackColor = true;
            btnPastebinRegister.Click += btnPastebinRegister_Click;
            // 
            // lblPastebinSyntax
            // 
            resources.ApplyResources(lblPastebinSyntax, "lblPastebinSyntax");
            lblPastebinSyntax.Name = "lblPastebinSyntax";
            // 
            // lblPastebinExpiration
            // 
            resources.ApplyResources(lblPastebinExpiration, "lblPastebinExpiration");
            lblPastebinExpiration.Name = "lblPastebinExpiration";
            // 
            // lblPastebinPrivacy
            // 
            resources.ApplyResources(lblPastebinPrivacy, "lblPastebinPrivacy");
            lblPastebinPrivacy.Name = "lblPastebinPrivacy";
            // 
            // lblPastebinTitle
            // 
            resources.ApplyResources(lblPastebinTitle, "lblPastebinTitle");
            lblPastebinTitle.Name = "lblPastebinTitle";
            // 
            // lblPastebinPassword
            // 
            resources.ApplyResources(lblPastebinPassword, "lblPastebinPassword");
            lblPastebinPassword.Name = "lblPastebinPassword";
            // 
            // lblPastebinUsername
            // 
            resources.ApplyResources(lblPastebinUsername, "lblPastebinUsername");
            lblPastebinUsername.Name = "lblPastebinUsername";
            // 
            // cbPastebinExpiration
            // 
            cbPastebinExpiration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPastebinExpiration.FormattingEnabled = true;
            resources.ApplyResources(cbPastebinExpiration, "cbPastebinExpiration");
            cbPastebinExpiration.Name = "cbPastebinExpiration";
            cbPastebinExpiration.SelectedIndexChanged += cbPastebinExpiration_SelectedIndexChanged;
            // 
            // cbPastebinPrivacy
            // 
            cbPastebinPrivacy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPastebinPrivacy.FormattingEnabled = true;
            resources.ApplyResources(cbPastebinPrivacy, "cbPastebinPrivacy");
            cbPastebinPrivacy.Name = "cbPastebinPrivacy";
            cbPastebinPrivacy.SelectedIndexChanged += cbPastebinPrivacy_SelectedIndexChanged;
            // 
            // txtPastebinTitle
            // 
            resources.ApplyResources(txtPastebinTitle, "txtPastebinTitle");
            txtPastebinTitle.Name = "txtPastebinTitle";
            txtPastebinTitle.TextChanged += txtPastebinTitle_TextChanged;
            // 
            // txtPastebinPassword
            // 
            resources.ApplyResources(txtPastebinPassword, "txtPastebinPassword");
            txtPastebinPassword.Name = "txtPastebinPassword";
            txtPastebinPassword.UseSystemPasswordChar = true;
            txtPastebinPassword.TextChanged += txtPastebinPassword_TextChanged;
            // 
            // txtPastebinUsername
            // 
            resources.ApplyResources(txtPastebinUsername, "txtPastebinUsername");
            txtPastebinUsername.Name = "txtPastebinUsername";
            txtPastebinUsername.TextChanged += txtPastebinUsername_TextChanged;
            // 
            // lblPastebinLoginStatus
            // 
            resources.ApplyResources(lblPastebinLoginStatus, "lblPastebinLoginStatus");
            lblPastebinLoginStatus.Name = "lblPastebinLoginStatus";
            // 
            // btnPastebinLogin
            // 
            resources.ApplyResources(btnPastebinLogin, "btnPastebinLogin");
            btnPastebinLogin.Name = "btnPastebinLogin";
            btnPastebinLogin.UseVisualStyleBackColor = true;
            btnPastebinLogin.Click += btnPastebinLogin_Click;
            // 
            // tpPaste_ee
            // 
            tpPaste_ee.BackColor = System.Drawing.SystemColors.Window;
            tpPaste_ee.Controls.Add(btnPaste_eeGetUserKey);
            tpPaste_ee.Controls.Add(lblPaste_eeUserAPIKey);
            tpPaste_ee.Controls.Add(txtPaste_eeUserAPIKey);
            resources.ApplyResources(tpPaste_ee, "tpPaste_ee");
            tpPaste_ee.Name = "tpPaste_ee";
            // 
            // btnPaste_eeGetUserKey
            // 
            resources.ApplyResources(btnPaste_eeGetUserKey, "btnPaste_eeGetUserKey");
            btnPaste_eeGetUserKey.Name = "btnPaste_eeGetUserKey";
            btnPaste_eeGetUserKey.UseVisualStyleBackColor = true;
            btnPaste_eeGetUserKey.Click += btnPaste_eeGetUserKey_Click;
            // 
            // lblPaste_eeUserAPIKey
            // 
            resources.ApplyResources(lblPaste_eeUserAPIKey, "lblPaste_eeUserAPIKey");
            lblPaste_eeUserAPIKey.Name = "lblPaste_eeUserAPIKey";
            // 
            // txtPaste_eeUserAPIKey
            // 
            resources.ApplyResources(txtPaste_eeUserAPIKey, "txtPaste_eeUserAPIKey");
            txtPaste_eeUserAPIKey.Name = "txtPaste_eeUserAPIKey";
            txtPaste_eeUserAPIKey.UseSystemPasswordChar = true;
            txtPaste_eeUserAPIKey.TextChanged += txtPaste_eeUserAPIKey_TextChanged;
            // 
            // tpGist
            // 
            tpGist.BackColor = System.Drawing.SystemColors.Window;
            tpGist.Controls.Add(lblGistCustomURLExample);
            tpGist.Controls.Add(lblGistOAuthInfo);
            tpGist.Controls.Add(lblGistCustomURL);
            tpGist.Controls.Add(txtGistCustomURL);
            tpGist.Controls.Add(cbGistUseRawURL);
            tpGist.Controls.Add(cbGistPublishPublic);
            tpGist.Controls.Add(oAuth2Gist);
            resources.ApplyResources(tpGist, "tpGist");
            tpGist.Name = "tpGist";
            // 
            // lblGistCustomURLExample
            // 
            resources.ApplyResources(lblGistCustomURLExample, "lblGistCustomURLExample");
            lblGistCustomURLExample.Name = "lblGistCustomURLExample";
            // 
            // lblGistOAuthInfo
            // 
            resources.ApplyResources(lblGistOAuthInfo, "lblGistOAuthInfo");
            lblGistOAuthInfo.Name = "lblGistOAuthInfo";
            // 
            // lblGistCustomURL
            // 
            resources.ApplyResources(lblGistCustomURL, "lblGistCustomURL");
            lblGistCustomURL.Name = "lblGistCustomURL";
            // 
            // txtGistCustomURL
            // 
            resources.ApplyResources(txtGistCustomURL, "txtGistCustomURL");
            txtGistCustomURL.Name = "txtGistCustomURL";
            txtGistCustomURL.TextChanged += txtGistCustomURL_TextChanged;
            // 
            // cbGistUseRawURL
            // 
            resources.ApplyResources(cbGistUseRawURL, "cbGistUseRawURL");
            cbGistUseRawURL.Name = "cbGistUseRawURL";
            cbGistUseRawURL.UseVisualStyleBackColor = true;
            cbGistUseRawURL.CheckedChanged += cbGistUseRawURL_CheckedChanged;
            // 
            // cbGistPublishPublic
            // 
            resources.ApplyResources(cbGistPublishPublic, "cbGistPublishPublic");
            cbGistPublishPublic.Name = "cbGistPublishPublic";
            cbGistPublishPublic.UseVisualStyleBackColor = true;
            cbGistPublishPublic.CheckedChanged += cbGistPublishPublic_CheckedChanged;
            // 
            // oAuth2Gist
            // 
            oAuth2Gist.IsRefreshable = false;
            resources.ApplyResources(oAuth2Gist, "oAuth2Gist");
            oAuth2Gist.Name = "oAuth2Gist";
            oAuth2Gist.UserInfo = null;
            oAuth2Gist.OpenButtonClicked += oAuth2Gist_OpenButtonClicked;
            oAuth2Gist.CompleteButtonClicked += oAuth2Gist_CompleteButtonClicked;
            oAuth2Gist.ClearButtonClicked += oAuth2Gist_ClearButtonClicked;
            // 
            // tpUpaste
            // 
            tpUpaste.BackColor = System.Drawing.SystemColors.Window;
            tpUpaste.Controls.Add(cbUpasteIsPublic);
            tpUpaste.Controls.Add(lblUpasteUserKey);
            tpUpaste.Controls.Add(txtUpasteUserKey);
            resources.ApplyResources(tpUpaste, "tpUpaste");
            tpUpaste.Name = "tpUpaste";
            // 
            // cbUpasteIsPublic
            // 
            resources.ApplyResources(cbUpasteIsPublic, "cbUpasteIsPublic");
            cbUpasteIsPublic.Name = "cbUpasteIsPublic";
            cbUpasteIsPublic.UseVisualStyleBackColor = true;
            cbUpasteIsPublic.CheckedChanged += cbUpasteIsPublic_CheckedChanged;
            // 
            // lblUpasteUserKey
            // 
            resources.ApplyResources(lblUpasteUserKey, "lblUpasteUserKey");
            lblUpasteUserKey.Name = "lblUpasteUserKey";
            // 
            // txtUpasteUserKey
            // 
            resources.ApplyResources(txtUpasteUserKey, "txtUpasteUserKey");
            txtUpasteUserKey.Name = "txtUpasteUserKey";
            txtUpasteUserKey.UseSystemPasswordChar = true;
            txtUpasteUserKey.TextChanged += txtUpasteUserKey_TextChanged;
            // 
            // tpHastebin
            // 
            tpHastebin.BackColor = System.Drawing.SystemColors.Window;
            tpHastebin.Controls.Add(cbHastebinUseFileExtension);
            tpHastebin.Controls.Add(txtHastebinSyntaxHighlighting);
            tpHastebin.Controls.Add(txtHastebinCustomDomain);
            tpHastebin.Controls.Add(lblHastebinSyntaxHighlighting);
            tpHastebin.Controls.Add(lblHastebinCustomDomain);
            resources.ApplyResources(tpHastebin, "tpHastebin");
            tpHastebin.Name = "tpHastebin";
            // 
            // cbHastebinUseFileExtension
            // 
            resources.ApplyResources(cbHastebinUseFileExtension, "cbHastebinUseFileExtension");
            cbHastebinUseFileExtension.Name = "cbHastebinUseFileExtension";
            cbHastebinUseFileExtension.UseVisualStyleBackColor = true;
            cbHastebinUseFileExtension.CheckedChanged += cbHastebinUseFileExtension_CheckedChanged;
            // 
            // txtHastebinSyntaxHighlighting
            // 
            resources.ApplyResources(txtHastebinSyntaxHighlighting, "txtHastebinSyntaxHighlighting");
            txtHastebinSyntaxHighlighting.Name = "txtHastebinSyntaxHighlighting";
            txtHastebinSyntaxHighlighting.TextChanged += txtHastebinSyntaxHighlighting_TextChanged;
            // 
            // txtHastebinCustomDomain
            // 
            resources.ApplyResources(txtHastebinCustomDomain, "txtHastebinCustomDomain");
            txtHastebinCustomDomain.Name = "txtHastebinCustomDomain";
            txtHastebinCustomDomain.TextChanged += txtHastebinCustomDomain_TextChanged;
            // 
            // lblHastebinSyntaxHighlighting
            // 
            resources.ApplyResources(lblHastebinSyntaxHighlighting, "lblHastebinSyntaxHighlighting");
            lblHastebinSyntaxHighlighting.Name = "lblHastebinSyntaxHighlighting";
            // 
            // lblHastebinCustomDomain
            // 
            resources.ApplyResources(lblHastebinCustomDomain, "lblHastebinCustomDomain");
            lblHastebinCustomDomain.Name = "lblHastebinCustomDomain";
            // 
            // tpOneTimeSecret
            // 
            tpOneTimeSecret.BackColor = System.Drawing.SystemColors.Window;
            tpOneTimeSecret.Controls.Add(lblOneTimeSecretAPIKey);
            tpOneTimeSecret.Controls.Add(lblOneTimeSecretEmail);
            tpOneTimeSecret.Controls.Add(txtOneTimeSecretAPIKey);
            tpOneTimeSecret.Controls.Add(txtOneTimeSecretEmail);
            resources.ApplyResources(tpOneTimeSecret, "tpOneTimeSecret");
            tpOneTimeSecret.Name = "tpOneTimeSecret";
            // 
            // lblOneTimeSecretAPIKey
            // 
            resources.ApplyResources(lblOneTimeSecretAPIKey, "lblOneTimeSecretAPIKey");
            lblOneTimeSecretAPIKey.Name = "lblOneTimeSecretAPIKey";
            // 
            // lblOneTimeSecretEmail
            // 
            resources.ApplyResources(lblOneTimeSecretEmail, "lblOneTimeSecretEmail");
            lblOneTimeSecretEmail.Name = "lblOneTimeSecretEmail";
            // 
            // txtOneTimeSecretAPIKey
            // 
            resources.ApplyResources(txtOneTimeSecretAPIKey, "txtOneTimeSecretAPIKey");
            txtOneTimeSecretAPIKey.Name = "txtOneTimeSecretAPIKey";
            txtOneTimeSecretAPIKey.UseSystemPasswordChar = true;
            txtOneTimeSecretAPIKey.TextChanged += txtOneTimeSecretAPIKey_TextChanged;
            // 
            // txtOneTimeSecretEmail
            // 
            resources.ApplyResources(txtOneTimeSecretEmail, "txtOneTimeSecretEmail");
            txtOneTimeSecretEmail.Name = "txtOneTimeSecretEmail";
            txtOneTimeSecretEmail.TextChanged += txtOneTimeSecretEmail_TextChanged;
            // 
            // tpPastie
            // 
            tpPastie.BackColor = System.Drawing.SystemColors.Window;
            tpPastie.Controls.Add(cbPastieIsPublic);
            resources.ApplyResources(tpPastie, "tpPastie");
            tpPastie.Name = "tpPastie";
            // 
            // cbPastieIsPublic
            // 
            resources.ApplyResources(cbPastieIsPublic, "cbPastieIsPublic");
            cbPastieIsPublic.Name = "cbPastieIsPublic";
            cbPastieIsPublic.UseVisualStyleBackColor = true;
            cbPastieIsPublic.CheckedChanged += cbPastieIsPublic_CheckedChanged;
            // 
            // tpImageUploaders
            // 
            tpImageUploaders.BackColor = System.Drawing.SystemColors.Window;
            tpImageUploaders.Controls.Add(tcImageUploaders);
            resources.ApplyResources(tpImageUploaders, "tpImageUploaders");
            tpImageUploaders.Name = "tpImageUploaders";
            // 
            // tcImageUploaders
            // 
            tcImageUploaders.Controls.Add(tpImgur);
            tcImageUploaders.Controls.Add(tpImageShack);
            tcImageUploaders.Controls.Add(tpFlickr);
            tcImageUploaders.Controls.Add(tpPhotobucket);
            tcImageUploaders.Controls.Add(tpChevereto);
            tcImageUploaders.Controls.Add(tpVgyme);
            resources.ApplyResources(tcImageUploaders, "tcImageUploaders");
            tcImageUploaders.Name = "tcImageUploaders";
            tcImageUploaders.SelectedIndex = 0;
            // 
            // tpImgur
            // 
            tpImgur.BackColor = System.Drawing.SystemColors.Window;
            tpImgur.Controls.Add(cbImgurUseGIFV);
            tpImgur.Controls.Add(cbImgurUploadSelectedAlbum);
            tpImgur.Controls.Add(cbImgurDirectLink);
            tpImgur.Controls.Add(atcImgurAccountType);
            tpImgur.Controls.Add(oauth2Imgur);
            tpImgur.Controls.Add(lvImgurAlbumList);
            tpImgur.Controls.Add(btnImgurRefreshAlbumList);
            tpImgur.Controls.Add(cbImgurThumbnailType);
            tpImgur.Controls.Add(lblImgurThumbnailType);
            resources.ApplyResources(tpImgur, "tpImgur");
            tpImgur.Name = "tpImgur";
            // 
            // cbImgurUseGIFV
            // 
            resources.ApplyResources(cbImgurUseGIFV, "cbImgurUseGIFV");
            cbImgurUseGIFV.Name = "cbImgurUseGIFV";
            cbImgurUseGIFV.UseVisualStyleBackColor = true;
            cbImgurUseGIFV.CheckedChanged += cbImgurUseGIFV_CheckedChanged;
            // 
            // cbImgurUploadSelectedAlbum
            // 
            resources.ApplyResources(cbImgurUploadSelectedAlbum, "cbImgurUploadSelectedAlbum");
            cbImgurUploadSelectedAlbum.Name = "cbImgurUploadSelectedAlbum";
            cbImgurUploadSelectedAlbum.UseVisualStyleBackColor = true;
            cbImgurUploadSelectedAlbum.CheckedChanged += cbImgurUploadSelectedAlbum_CheckedChanged;
            // 
            // cbImgurDirectLink
            // 
            resources.ApplyResources(cbImgurDirectLink, "cbImgurDirectLink");
            cbImgurDirectLink.Name = "cbImgurDirectLink";
            cbImgurDirectLink.UseVisualStyleBackColor = true;
            cbImgurDirectLink.CheckedChanged += cbImgurDirectLink_CheckedChanged;
            // 
            // atcImgurAccountType
            // 
            resources.ApplyResources(atcImgurAccountType, "atcImgurAccountType");
            atcImgurAccountType.Name = "atcImgurAccountType";
            atcImgurAccountType.SelectedAccountType = AccountType.Anonymous;
            atcImgurAccountType.AccountTypeChanged += atcImgurAccountType_AccountTypeChanged;
            // 
            // oauth2Imgur
            // 
            resources.ApplyResources(oauth2Imgur, "oauth2Imgur");
            oauth2Imgur.Name = "oauth2Imgur";
            oauth2Imgur.UserInfo = null;
            oauth2Imgur.OpenButtonClicked += oauth2Imgur_OpenButtonClicked;
            oauth2Imgur.CompleteButtonClicked += oauth2Imgur_CompleteButtonClicked;
            oauth2Imgur.ClearButtonClicked += oauth2Imgur_ClearButtonClicked;
            oauth2Imgur.RefreshButtonClicked += oauth2Imgur_RefreshButtonClicked;
            // 
            // lvImgurAlbumList
            // 
            lvImgurAlbumList.AllowColumnSort = true;
            lvImgurAlbumList.AutoFillColumn = true;
            lvImgurAlbumList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chImgurID, chImgurTitle, chImgurDescription });
            lvImgurAlbumList.FullRowSelect = true;
            resources.ApplyResources(lvImgurAlbumList, "lvImgurAlbumList");
            lvImgurAlbumList.MultiSelect = false;
            lvImgurAlbumList.Name = "lvImgurAlbumList";
            lvImgurAlbumList.UseCompatibleStateImageBehavior = false;
            lvImgurAlbumList.View = System.Windows.Forms.View.Details;
            lvImgurAlbumList.SelectedIndexChanged += lvImgurAlbumList_SelectedIndexChanged;
            // 
            // chImgurID
            // 
            resources.ApplyResources(chImgurID, "chImgurID");
            // 
            // chImgurTitle
            // 
            resources.ApplyResources(chImgurTitle, "chImgurTitle");
            // 
            // chImgurDescription
            // 
            resources.ApplyResources(chImgurDescription, "chImgurDescription");
            // 
            // btnImgurRefreshAlbumList
            // 
            resources.ApplyResources(btnImgurRefreshAlbumList, "btnImgurRefreshAlbumList");
            btnImgurRefreshAlbumList.Name = "btnImgurRefreshAlbumList";
            btnImgurRefreshAlbumList.UseVisualStyleBackColor = true;
            btnImgurRefreshAlbumList.Click += btnImgurRefreshAlbumList_Click;
            // 
            // cbImgurThumbnailType
            // 
            cbImgurThumbnailType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbImgurThumbnailType.FormattingEnabled = true;
            resources.ApplyResources(cbImgurThumbnailType, "cbImgurThumbnailType");
            cbImgurThumbnailType.Name = "cbImgurThumbnailType";
            cbImgurThumbnailType.SelectedIndexChanged += cbImgurThumbnailType_SelectedIndexChanged;
            // 
            // lblImgurThumbnailType
            // 
            resources.ApplyResources(lblImgurThumbnailType, "lblImgurThumbnailType");
            lblImgurThumbnailType.Name = "lblImgurThumbnailType";
            // 
            // tpImageShack
            // 
            tpImageShack.BackColor = System.Drawing.SystemColors.Window;
            tpImageShack.Controls.Add(btnImageShackLogin);
            tpImageShack.Controls.Add(btnImageShackOpenPublicProfile);
            tpImageShack.Controls.Add(cbImageShackIsPublic);
            tpImageShack.Controls.Add(btnImageShackOpenMyImages);
            tpImageShack.Controls.Add(lblImageShackUsername);
            tpImageShack.Controls.Add(txtImageShackUsername);
            tpImageShack.Controls.Add(txtImageShackPassword);
            tpImageShack.Controls.Add(lblImageShackPassword);
            resources.ApplyResources(tpImageShack, "tpImageShack");
            tpImageShack.Name = "tpImageShack";
            // 
            // btnImageShackLogin
            // 
            resources.ApplyResources(btnImageShackLogin, "btnImageShackLogin");
            btnImageShackLogin.Name = "btnImageShackLogin";
            btnImageShackLogin.UseVisualStyleBackColor = true;
            btnImageShackLogin.Click += btnImageShackLogin_Click;
            // 
            // btnImageShackOpenPublicProfile
            // 
            resources.ApplyResources(btnImageShackOpenPublicProfile, "btnImageShackOpenPublicProfile");
            btnImageShackOpenPublicProfile.Name = "btnImageShackOpenPublicProfile";
            btnImageShackOpenPublicProfile.UseVisualStyleBackColor = true;
            btnImageShackOpenPublicProfile.Click += btnImageShackOpenPublicProfile_Click;
            // 
            // cbImageShackIsPublic
            // 
            resources.ApplyResources(cbImageShackIsPublic, "cbImageShackIsPublic");
            cbImageShackIsPublic.Name = "cbImageShackIsPublic";
            cbImageShackIsPublic.UseVisualStyleBackColor = true;
            cbImageShackIsPublic.CheckedChanged += cbImageShackIsPublic_CheckedChanged;
            // 
            // btnImageShackOpenMyImages
            // 
            resources.ApplyResources(btnImageShackOpenMyImages, "btnImageShackOpenMyImages");
            btnImageShackOpenMyImages.Name = "btnImageShackOpenMyImages";
            btnImageShackOpenMyImages.UseVisualStyleBackColor = true;
            btnImageShackOpenMyImages.Click += btnImageShackOpenMyImages_Click;
            // 
            // lblImageShackUsername
            // 
            resources.ApplyResources(lblImageShackUsername, "lblImageShackUsername");
            lblImageShackUsername.Name = "lblImageShackUsername";
            // 
            // txtImageShackUsername
            // 
            resources.ApplyResources(txtImageShackUsername, "txtImageShackUsername");
            txtImageShackUsername.Name = "txtImageShackUsername";
            txtImageShackUsername.TextChanged += txtImageShackUsername_TextChanged;
            // 
            // txtImageShackPassword
            // 
            resources.ApplyResources(txtImageShackPassword, "txtImageShackPassword");
            txtImageShackPassword.Name = "txtImageShackPassword";
            txtImageShackPassword.UseSystemPasswordChar = true;
            txtImageShackPassword.TextChanged += txtImageShackPassword_TextChanged;
            // 
            // lblImageShackPassword
            // 
            resources.ApplyResources(lblImageShackPassword, "lblImageShackPassword");
            lblImageShackPassword.Name = "lblImageShackPassword";
            // 
            // tpFlickr
            // 
            tpFlickr.BackColor = System.Drawing.SystemColors.Window;
            tpFlickr.Controls.Add(cbFlickrDirectLink);
            tpFlickr.Controls.Add(oauthFlickr);
            resources.ApplyResources(tpFlickr, "tpFlickr");
            tpFlickr.Name = "tpFlickr";
            // 
            // cbFlickrDirectLink
            // 
            resources.ApplyResources(cbFlickrDirectLink, "cbFlickrDirectLink");
            cbFlickrDirectLink.Name = "cbFlickrDirectLink";
            cbFlickrDirectLink.UseVisualStyleBackColor = true;
            cbFlickrDirectLink.CheckedChanged += cbFlickrDirectLink_CheckedChanged;
            // 
            // oauthFlickr
            // 
            oauthFlickr.IsRefreshable = false;
            resources.ApplyResources(oauthFlickr, "oauthFlickr");
            oauthFlickr.Name = "oauthFlickr";
            oauthFlickr.UserInfo = null;
            oauthFlickr.OpenButtonClicked += oauthFlickr_OpenButtonClicked;
            oauthFlickr.CompleteButtonClicked += oauthFlickr_CompleteButtonClicked;
            oauthFlickr.ClearButtonClicked += oauthFlickr_ClearButtonClicked;
            // 
            // tpPhotobucket
            // 
            tpPhotobucket.BackColor = System.Drawing.SystemColors.Window;
            tpPhotobucket.Controls.Add(gbPhotobucketAlbumPath);
            tpPhotobucket.Controls.Add(gbPhotobucketAlbums);
            tpPhotobucket.Controls.Add(gbPhotobucketUserAccount);
            resources.ApplyResources(tpPhotobucket, "tpPhotobucket");
            tpPhotobucket.Name = "tpPhotobucket";
            // 
            // gbPhotobucketAlbumPath
            // 
            gbPhotobucketAlbumPath.Controls.Add(btnPhotobucketAddAlbum);
            gbPhotobucketAlbumPath.Controls.Add(btnPhotobucketRemoveAlbum);
            gbPhotobucketAlbumPath.Controls.Add(cbPhotobucketAlbumPaths);
            resources.ApplyResources(gbPhotobucketAlbumPath, "gbPhotobucketAlbumPath");
            gbPhotobucketAlbumPath.Name = "gbPhotobucketAlbumPath";
            gbPhotobucketAlbumPath.TabStop = false;
            // 
            // btnPhotobucketAddAlbum
            // 
            resources.ApplyResources(btnPhotobucketAddAlbum, "btnPhotobucketAddAlbum");
            btnPhotobucketAddAlbum.Name = "btnPhotobucketAddAlbum";
            btnPhotobucketAddAlbum.UseVisualStyleBackColor = true;
            btnPhotobucketAddAlbum.Click += btnPhotobucketAddAlbum_Click;
            // 
            // btnPhotobucketRemoveAlbum
            // 
            resources.ApplyResources(btnPhotobucketRemoveAlbum, "btnPhotobucketRemoveAlbum");
            btnPhotobucketRemoveAlbum.Name = "btnPhotobucketRemoveAlbum";
            btnPhotobucketRemoveAlbum.UseVisualStyleBackColor = true;
            btnPhotobucketRemoveAlbum.Click += btnPhotobucketRemoveAlbum_Click;
            // 
            // cbPhotobucketAlbumPaths
            // 
            cbPhotobucketAlbumPaths.FormattingEnabled = true;
            resources.ApplyResources(cbPhotobucketAlbumPaths, "cbPhotobucketAlbumPaths");
            cbPhotobucketAlbumPaths.Name = "cbPhotobucketAlbumPaths";
            cbPhotobucketAlbumPaths.SelectedIndexChanged += cbPhotobucketAlbumPaths_SelectedIndexChanged;
            // 
            // gbPhotobucketAlbums
            // 
            gbPhotobucketAlbums.Controls.Add(lblPhotobucketNewAlbumName);
            gbPhotobucketAlbums.Controls.Add(lblPhotobucketParentAlbumPath);
            gbPhotobucketAlbums.Controls.Add(txtPhotobucketNewAlbumName);
            gbPhotobucketAlbums.Controls.Add(txtPhotobucketParentAlbumPath);
            gbPhotobucketAlbums.Controls.Add(btnPhotobucketCreateAlbum);
            resources.ApplyResources(gbPhotobucketAlbums, "gbPhotobucketAlbums");
            gbPhotobucketAlbums.Name = "gbPhotobucketAlbums";
            gbPhotobucketAlbums.TabStop = false;
            // 
            // lblPhotobucketNewAlbumName
            // 
            resources.ApplyResources(lblPhotobucketNewAlbumName, "lblPhotobucketNewAlbumName");
            lblPhotobucketNewAlbumName.Name = "lblPhotobucketNewAlbumName";
            // 
            // lblPhotobucketParentAlbumPath
            // 
            resources.ApplyResources(lblPhotobucketParentAlbumPath, "lblPhotobucketParentAlbumPath");
            lblPhotobucketParentAlbumPath.Name = "lblPhotobucketParentAlbumPath";
            // 
            // txtPhotobucketNewAlbumName
            // 
            resources.ApplyResources(txtPhotobucketNewAlbumName, "txtPhotobucketNewAlbumName");
            txtPhotobucketNewAlbumName.Name = "txtPhotobucketNewAlbumName";
            // 
            // txtPhotobucketParentAlbumPath
            // 
            resources.ApplyResources(txtPhotobucketParentAlbumPath, "txtPhotobucketParentAlbumPath");
            txtPhotobucketParentAlbumPath.Name = "txtPhotobucketParentAlbumPath";
            // 
            // btnPhotobucketCreateAlbum
            // 
            resources.ApplyResources(btnPhotobucketCreateAlbum, "btnPhotobucketCreateAlbum");
            btnPhotobucketCreateAlbum.Name = "btnPhotobucketCreateAlbum";
            btnPhotobucketCreateAlbum.UseVisualStyleBackColor = true;
            btnPhotobucketCreateAlbum.Click += btnPhotobucketCreateAlbum_Click;
            // 
            // gbPhotobucketUserAccount
            // 
            gbPhotobucketUserAccount.Controls.Add(lblPhotobucketDefaultAlbumName);
            gbPhotobucketUserAccount.Controls.Add(btnPhotobucketAuthOpen);
            gbPhotobucketUserAccount.Controls.Add(txtPhotobucketDefaultAlbumName);
            gbPhotobucketUserAccount.Controls.Add(lblPhotobucketVerificationCode);
            gbPhotobucketUserAccount.Controls.Add(btnPhotobucketAuthComplete);
            gbPhotobucketUserAccount.Controls.Add(txtPhotobucketVerificationCode);
            gbPhotobucketUserAccount.Controls.Add(lblPhotobucketAccountStatus);
            resources.ApplyResources(gbPhotobucketUserAccount, "gbPhotobucketUserAccount");
            gbPhotobucketUserAccount.Name = "gbPhotobucketUserAccount";
            gbPhotobucketUserAccount.TabStop = false;
            // 
            // lblPhotobucketDefaultAlbumName
            // 
            resources.ApplyResources(lblPhotobucketDefaultAlbumName, "lblPhotobucketDefaultAlbumName");
            lblPhotobucketDefaultAlbumName.Name = "lblPhotobucketDefaultAlbumName";
            // 
            // btnPhotobucketAuthOpen
            // 
            resources.ApplyResources(btnPhotobucketAuthOpen, "btnPhotobucketAuthOpen");
            btnPhotobucketAuthOpen.Name = "btnPhotobucketAuthOpen";
            btnPhotobucketAuthOpen.UseVisualStyleBackColor = true;
            btnPhotobucketAuthOpen.Click += btnPhotobucketAuthOpen_Click;
            // 
            // txtPhotobucketDefaultAlbumName
            // 
            resources.ApplyResources(txtPhotobucketDefaultAlbumName, "txtPhotobucketDefaultAlbumName");
            txtPhotobucketDefaultAlbumName.Name = "txtPhotobucketDefaultAlbumName";
            txtPhotobucketDefaultAlbumName.ReadOnly = true;
            // 
            // lblPhotobucketVerificationCode
            // 
            resources.ApplyResources(lblPhotobucketVerificationCode, "lblPhotobucketVerificationCode");
            lblPhotobucketVerificationCode.Name = "lblPhotobucketVerificationCode";
            // 
            // btnPhotobucketAuthComplete
            // 
            resources.ApplyResources(btnPhotobucketAuthComplete, "btnPhotobucketAuthComplete");
            btnPhotobucketAuthComplete.Name = "btnPhotobucketAuthComplete";
            btnPhotobucketAuthComplete.UseVisualStyleBackColor = true;
            btnPhotobucketAuthComplete.Click += btnPhotobucketAuthComplete_Click;
            // 
            // txtPhotobucketVerificationCode
            // 
            resources.ApplyResources(txtPhotobucketVerificationCode, "txtPhotobucketVerificationCode");
            txtPhotobucketVerificationCode.Name = "txtPhotobucketVerificationCode";
            // 
            // lblPhotobucketAccountStatus
            // 
            resources.ApplyResources(lblPhotobucketAccountStatus, "lblPhotobucketAccountStatus");
            lblPhotobucketAccountStatus.Name = "lblPhotobucketAccountStatus";
            // 
            // tpChevereto
            // 
            tpChevereto.BackColor = System.Drawing.SystemColors.Window;
            tpChevereto.Controls.Add(lblCheveretoUploadURLExample);
            tpChevereto.Controls.Add(cbCheveretoDirectURL);
            tpChevereto.Controls.Add(lblCheveretoUploadURL);
            tpChevereto.Controls.Add(txtCheveretoUploadURL);
            tpChevereto.Controls.Add(txtCheveretoAPIKey);
            tpChevereto.Controls.Add(lblCheveretoAPIKey);
            resources.ApplyResources(tpChevereto, "tpChevereto");
            tpChevereto.Name = "tpChevereto";
            // 
            // lblCheveretoUploadURLExample
            // 
            resources.ApplyResources(lblCheveretoUploadURLExample, "lblCheveretoUploadURLExample");
            lblCheveretoUploadURLExample.Name = "lblCheveretoUploadURLExample";
            // 
            // cbCheveretoDirectURL
            // 
            resources.ApplyResources(cbCheveretoDirectURL, "cbCheveretoDirectURL");
            cbCheveretoDirectURL.Name = "cbCheveretoDirectURL";
            cbCheveretoDirectURL.UseVisualStyleBackColor = true;
            cbCheveretoDirectURL.CheckedChanged += cbCheveretoDirectURL_CheckedChanged;
            // 
            // lblCheveretoUploadURL
            // 
            resources.ApplyResources(lblCheveretoUploadURL, "lblCheveretoUploadURL");
            lblCheveretoUploadURL.Name = "lblCheveretoUploadURL";
            // 
            // txtCheveretoUploadURL
            // 
            resources.ApplyResources(txtCheveretoUploadURL, "txtCheveretoUploadURL");
            txtCheveretoUploadURL.Name = "txtCheveretoUploadURL";
            txtCheveretoUploadURL.TextChanged += txtCheveretoWebsite_TextChanged;
            // 
            // txtCheveretoAPIKey
            // 
            resources.ApplyResources(txtCheveretoAPIKey, "txtCheveretoAPIKey");
            txtCheveretoAPIKey.Name = "txtCheveretoAPIKey";
            txtCheveretoAPIKey.UseSystemPasswordChar = true;
            txtCheveretoAPIKey.TextChanged += txtCheveretoAPIKey_TextChanged;
            // 
            // lblCheveretoAPIKey
            // 
            resources.ApplyResources(lblCheveretoAPIKey, "lblCheveretoAPIKey");
            lblCheveretoAPIKey.Name = "lblCheveretoAPIKey";
            // 
            // tpVgyme
            // 
            tpVgyme.BackColor = System.Drawing.SystemColors.Window;
            tpVgyme.Controls.Add(llVgymeAccountDetailsPage);
            tpVgyme.Controls.Add(txtVgymeUserKey);
            tpVgyme.Controls.Add(lvlVgymeUserKey);
            resources.ApplyResources(tpVgyme, "tpVgyme");
            tpVgyme.Name = "tpVgyme";
            // 
            // llVgymeAccountDetailsPage
            // 
            resources.ApplyResources(llVgymeAccountDetailsPage, "llVgymeAccountDetailsPage");
            llVgymeAccountDetailsPage.Name = "llVgymeAccountDetailsPage";
            llVgymeAccountDetailsPage.TabStop = true;
            llVgymeAccountDetailsPage.LinkClicked += llVgymeAccountDetailsPage_LinkClicked;
            // 
            // txtVgymeUserKey
            // 
            resources.ApplyResources(txtVgymeUserKey, "txtVgymeUserKey");
            txtVgymeUserKey.Name = "txtVgymeUserKey";
            txtVgymeUserKey.UseSystemPasswordChar = true;
            txtVgymeUserKey.TextChanged += txtVgymeUserKey_TextChanged;
            // 
            // lvlVgymeUserKey
            // 
            resources.ApplyResources(lvlVgymeUserKey, "lvlVgymeUserKey");
            lvlVgymeUserKey.Name = "lvlVgymeUserKey";
            // 
            // tcUploaders
            // 
            tcUploaders.Controls.Add(tpImageUploaders);
            tcUploaders.Controls.Add(tpTextUploaders);
            tcUploaders.Controls.Add(tpFileUploaders);
            tcUploaders.Controls.Add(tpURLShorteners);
            resources.ApplyResources(tcUploaders, "tcUploaders");
            tcUploaders.Name = "tcUploaders";
            tcUploaders.SelectedIndex = 0;
            // 
            // tttvMain
            // 
            tttvMain.AutoSelectChild = true;
            resources.ApplyResources(tttvMain, "tttvMain");
            tttvMain.ImageList = null;
            tttvMain.LeftPanelBackColor = System.Drawing.SystemColors.Window;
            tttvMain.MainTabControl = null;
            tttvMain.Name = "tttvMain";
            tttvMain.SeparatorColor = System.Drawing.SystemColors.ControlDark;
            tttvMain.TreeViewFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 162);
            tttvMain.TreeViewSize = 230;
            // 
            // actRapidShareAccountType
            // 
            resources.ApplyResources(actRapidShareAccountType, "actRapidShareAccountType");
            actRapidShareAccountType.Name = "actRapidShareAccountType";
            actRapidShareAccountType.SelectedAccountType = AccountType.Anonymous;
            // 
            // UploadersConfigForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(tcUploaders);
            Controls.Add(tttvMain);
            Name = "UploadersConfigForm";
            Shown += UploadersConfigForm_Shown;
            Resize += UploadersConfigForm_Resize;
            tpURLShorteners.ResumeLayout(false);
            tcURLShorteners.ResumeLayout(false);
            tpBitly.ResumeLayout(false);
            tpBitly.PerformLayout();
            tpYourls.ResumeLayout(false);
            tpYourls.PerformLayout();
            tpPolr.ResumeLayout(false);
            tpPolr.PerformLayout();
            tpFirebaseDynamicLinks.ResumeLayout(false);
            tpFirebaseDynamicLinks.PerformLayout();
            tpKutt.ResumeLayout(false);
            tpKutt.PerformLayout();
            tpZeroWidthShortener.ResumeLayout(false);
            tpZeroWidthShortener.PerformLayout();
            tpFileUploaders.ResumeLayout(false);
            tcFileUploaders.ResumeLayout(false);
            tpFTP.ResumeLayout(false);
            tpFTP.PerformLayout();
            gbFTPAccount.ResumeLayout(false);
            gbFTPAccount.PerformLayout();
            gbSFTP.ResumeLayout(false);
            gbSFTP.PerformLayout();
            pFTPTransferMode.ResumeLayout(false);
            pFTPTransferMode.PerformLayout();
            pFTPProtocol.ResumeLayout(false);
            pFTPProtocol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudFTPPort).EndInit();
            gbFTPS.ResumeLayout(false);
            gbFTPS.PerformLayout();
            tpDropbox.ResumeLayout(false);
            tpDropbox.PerformLayout();
            tpOneDrive.ResumeLayout(false);
            tpOneDrive.PerformLayout();
            tpGoogleDrive.ResumeLayout(false);
            tpGoogleDrive.PerformLayout();
            tpPuush.ResumeLayout(false);
            tpPuush.PerformLayout();
            tpBox.ResumeLayout(false);
            tpBox.PerformLayout();
            tpAmazonS3.ResumeLayout(false);
            tpAmazonS3.PerformLayout();
            gbAmazonS3Advanced.ResumeLayout(false);
            gbAmazonS3Advanced.PerformLayout();
            tpGoogleCloudStorage.ResumeLayout(false);
            tpGoogleCloudStorage.PerformLayout();
            gbGoogleCloudStorageAdvanced.ResumeLayout(false);
            gbGoogleCloudStorageAdvanced.PerformLayout();
            tpAzureStorage.ResumeLayout(false);
            tpAzureStorage.PerformLayout();
            tpBackblazeB2.ResumeLayout(false);
            tpBackblazeB2.PerformLayout();
            tpMega.ResumeLayout(false);
            tpMega.PerformLayout();
            tpOwnCloud.ResumeLayout(false);
            tpOwnCloud.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudOwnCloudExpiryTime).EndInit();
            tpMediaFire.ResumeLayout(false);
            tpMediaFire.PerformLayout();
            tpPushbullet.ResumeLayout(false);
            tpPushbullet.PerformLayout();
            tpSendSpace.ResumeLayout(false);
            tpSendSpace.PerformLayout();
            tpHostr.ResumeLayout(false);
            tpHostr.PerformLayout();
            tpLambda.ResumeLayout(false);
            tpLambda.PerformLayout();
            tpPomf.ResumeLayout(false);
            tpPomf.PerformLayout();
            tpSeafile.ResumeLayout(false);
            tpSeafile.PerformLayout();
            grpSeafileShareSettings.ResumeLayout(false);
            grpSeafileShareSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudSeafileExpireDays).EndInit();
            grpSeafileAccInfo.ResumeLayout(false);
            grpSeafileAccInfo.PerformLayout();
            grpSeafileObtainAuthToken.ResumeLayout(false);
            grpSeafileObtainAuthToken.PerformLayout();
            tpStreamable.ResumeLayout(false);
            tpStreamable.PerformLayout();
            tpSul.ResumeLayout(false);
            tpSul.PerformLayout();
            tpLithiio.ResumeLayout(false);
            tpLithiio.PerformLayout();
            tpPlik.ResumeLayout(false);
            gbPlikSettings.ResumeLayout(false);
            gbPlikSettings.PerformLayout();
            gbPlikLoginCredentials.ResumeLayout(false);
            gbPlikLoginCredentials.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPlikTTL).EndInit();
            tpYouTube.ResumeLayout(false);
            tpYouTube.PerformLayout();
            tpSharedFolder.ResumeLayout(false);
            tpSharedFolder.PerformLayout();
            tpEmail.ResumeLayout(false);
            tpEmail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudEmailSmtpPort).EndInit();
            tpTextUploaders.ResumeLayout(false);
            tcTextUploaders.ResumeLayout(false);
            tpPastebin.ResumeLayout(false);
            tpPastebin.PerformLayout();
            tpPaste_ee.ResumeLayout(false);
            tpPaste_ee.PerformLayout();
            tpGist.ResumeLayout(false);
            tpGist.PerformLayout();
            tpUpaste.ResumeLayout(false);
            tpUpaste.PerformLayout();
            tpHastebin.ResumeLayout(false);
            tpHastebin.PerformLayout();
            tpOneTimeSecret.ResumeLayout(false);
            tpOneTimeSecret.PerformLayout();
            tpPastie.ResumeLayout(false);
            tpPastie.PerformLayout();
            tpImageUploaders.ResumeLayout(false);
            tcImageUploaders.ResumeLayout(false);
            tpImgur.ResumeLayout(false);
            tpImgur.PerformLayout();
            tpImageShack.ResumeLayout(false);
            tpImageShack.PerformLayout();
            tpFlickr.ResumeLayout(false);
            tpFlickr.PerformLayout();
            tpPhotobucket.ResumeLayout(false);
            gbPhotobucketAlbumPath.ResumeLayout(false);
            gbPhotobucketAlbums.ResumeLayout(false);
            gbPhotobucketAlbums.PerformLayout();
            gbPhotobucketUserAccount.ResumeLayout(false);
            gbPhotobucketUserAccount.PerformLayout();
            tpChevereto.ResumeLayout(false);
            tpChevereto.PerformLayout();
            tpVgyme.ResumeLayout(false);
            tpVgyme.PerformLayout();
            tcUploaders.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.TextBox txtRapidSharePremiumUserName;
        private AccountTypeControl actRapidShareAccountType;
        private System.Windows.Forms.ToolTip ttHelpTip;
        private System.Windows.Forms.TabPage tpURLShorteners;
        private System.Windows.Forms.TabControl tcURLShorteners;
        private OAuthControl oauth2Bitly;
        private System.Windows.Forms.TextBox txtYourlsPassword;
        private System.Windows.Forms.TextBox txtYourlsUsername;
        private System.Windows.Forms.TextBox txtYourlsSignature;
        private System.Windows.Forms.Label lblYourlsNote;
        private System.Windows.Forms.Label lblYourlsPassword;
        private System.Windows.Forms.Label lblYourlsUsername;
        private System.Windows.Forms.Label lblYourlsSignature;
        private System.Windows.Forms.TextBox txtYourlsAPIURL;
        private System.Windows.Forms.Label lblYourlsAPIURL;
        internal System.Windows.Forms.TabPage tpFileUploaders;
        private System.Windows.Forms.TabControl tcFileUploaders;
        private System.Windows.Forms.CheckBox cbDropboxAutoCreateShareableLink;
        private System.Windows.Forms.Label lblDropboxPath;
        private System.Windows.Forms.TextBox txtDropboxPath;
        private System.Windows.Forms.Button btnCopyShowFiles;
        internal System.Windows.Forms.TabPage tpFTP;
        private System.Windows.Forms.Label lblFTPFile;
        private System.Windows.Forms.Label lblFTPText;
        private System.Windows.Forms.Label lblFTPImage;
        private System.Windows.Forms.ComboBox cbFTPImage;
        private System.Windows.Forms.ComboBox cbFTPFile;
        private System.Windows.Forms.ComboBox cbFTPText;
        private System.Windows.Forms.Label lblMegaStatus;
        private System.Windows.Forms.Button btnMegaRefreshFolders;
        private System.Windows.Forms.Button btnMegaRegister;
        private System.Windows.Forms.Label lblMegaFolder;
        private System.Windows.Forms.ComboBox cbMegaFolder;
        private System.Windows.Forms.Label lblMegaEmail;
        private System.Windows.Forms.TextBox txtMegaEmail;
        private System.Windows.Forms.Label lblMegaPassword;
        private System.Windows.Forms.TextBox txtMegaPassword;
        private System.Windows.Forms.Button btnMegaLogin;
        private System.Windows.Forms.Label lblMegaStatusTitle;
        private System.Windows.Forms.TextBox txtAmazonS3CustomDomain;
        private System.Windows.Forms.Label lblAmazonS3PathPreviewLabel;
        private System.Windows.Forms.Label lblAmazonS3PathPreview;
        private System.Windows.Forms.Button btnAmazonS3BucketNameOpen;
        private System.Windows.Forms.Button btnAmazonS3AccessKeyOpen;
        private System.Windows.Forms.CheckBox cbAmazonS3CustomCNAME;
        private System.Windows.Forms.ComboBox cbAmazonS3Endpoints;
        private System.Windows.Forms.Label lblAmazonS3BucketName;
        private System.Windows.Forms.TextBox txtAmazonS3BucketName;
        private System.Windows.Forms.Label lblAmazonS3Endpoints;
        private System.Windows.Forms.TextBox txtAmazonS3ObjectPrefix;
        private System.Windows.Forms.Label lblAmazonS3ObjectPrefix;
        private System.Windows.Forms.TextBox txtAmazonS3SecretKey;
        private System.Windows.Forms.Label lblAmazonS3SecretKey;
        private System.Windows.Forms.Label lblAmazonS3AccessKey;
        private System.Windows.Forms.TextBox txtAmazonS3AccessKey;
        private System.Windows.Forms.Label lblPushbulletDevices;
        private System.Windows.Forms.ComboBox cbPushbulletDevices;
        private System.Windows.Forms.Button btnPushbulletGetDeviceList;
        private System.Windows.Forms.Label lblPushbulletUserKey;
        private System.Windows.Forms.TextBox txtPushbulletUserKey;
        private System.Windows.Forms.CheckBox cbGoogleDriveIsPublic;
        private System.Windows.Forms.Label lblBoxFolderTip;
        private System.Windows.Forms.CheckBox cbBoxShare;
        private System.Windows.Forms.ComboBox cbBoxShareAccessLevel;
        private System.Windows.Forms.Label lblBoxShareAccessLevel;
        private ShareX.HelpersLib.MyListView lvBoxFolders;
        private System.Windows.Forms.ColumnHeader chBoxFoldersName;
        private System.Windows.Forms.Label lblBoxFolderID;
        private System.Windows.Forms.Button btnBoxRefreshFolders;
        private OAuthControl oauth2Box;
        private System.Windows.Forms.Button btnSendSpaceRegister;
        private System.Windows.Forms.Label lblSendSpacePassword;
        private System.Windows.Forms.Label lblSendSpaceUsername;
        private System.Windows.Forms.TextBox txtSendSpacePassword;
        private System.Windows.Forms.TextBox txtSendSpaceUserName;
        private AccountTypeControl atcSendSpaceAccountType;
        private System.Windows.Forms.CheckBox cbLocalhostrDirectURL;
        private System.Windows.Forms.Label lblLocalhostrPassword;
        private System.Windows.Forms.Label lblLocalhostrEmail;
        private System.Windows.Forms.TextBox txtLocalhostrPassword;
        private System.Windows.Forms.TextBox txtLocalhostrEmail;
        private System.Windows.Forms.Label lblEmailSmtpServer;
        private System.Windows.Forms.Label lblEmailPassword;
        private System.Windows.Forms.CheckBox cbEmailRememberLastTo;
        private System.Windows.Forms.TextBox txtEmailFrom;
        private System.Windows.Forms.TextBox txtEmailPassword;
        private System.Windows.Forms.TextBox txtEmailDefaultBody;
        private System.Windows.Forms.Label lblEmailFrom;
        private System.Windows.Forms.TextBox txtEmailSmtpServer;
        private System.Windows.Forms.Label lblEmailDefaultSubject;
        private System.Windows.Forms.Label lblEmailDefaultBody;
        private System.Windows.Forms.NumericUpDown nudEmailSmtpPort;
        private System.Windows.Forms.Label lblEmailSmtpPort;
        private System.Windows.Forms.TextBox txtEmailDefaultSubject;
        private System.Windows.Forms.Label lblSharedFolderFiles;
        private System.Windows.Forms.Label lblSharedFolderText;
        private System.Windows.Forms.Label lblSharedFolderImages;
        private System.Windows.Forms.ComboBox cbSharedFolderFiles;
        private System.Windows.Forms.ComboBox cbSharedFolderText;
        private System.Windows.Forms.ComboBox cbSharedFolderImages;
        private System.Windows.Forms.TabPage tpTextUploaders;
        private System.Windows.Forms.TabControl tcTextUploaders;
        private System.Windows.Forms.Button btnPastebinLogin;
        private System.Windows.Forms.Label lblPaste_eeUserAPIKey;
        private System.Windows.Forms.TextBox txtPaste_eeUserAPIKey;
        private System.Windows.Forms.CheckBox cbGistPublishPublic;
        private OAuthControl oAuth2Gist;
        private System.Windows.Forms.CheckBox cbUpasteIsPublic;
        private System.Windows.Forms.Label lblUpasteUserKey;
        private System.Windows.Forms.TextBox txtUpasteUserKey;
        private System.Windows.Forms.TabPage tpImageUploaders;
        private System.Windows.Forms.TabControl tcImageUploaders;
        private OAuthControl oauth2Imgur;
        private ShareX.HelpersLib.MyListView lvImgurAlbumList;
        private System.Windows.Forms.ColumnHeader chImgurID;
        private System.Windows.Forms.ColumnHeader chImgurTitle;
        private System.Windows.Forms.ColumnHeader chImgurDescription;
        private System.Windows.Forms.Button btnImgurRefreshAlbumList;
        private System.Windows.Forms.ComboBox cbImgurThumbnailType;
        private System.Windows.Forms.Label lblImgurThumbnailType;
        private AccountTypeControl atcImgurAccountType;
        private System.Windows.Forms.Button btnImageShackLogin;
        private System.Windows.Forms.Button btnImageShackOpenPublicProfile;
        private System.Windows.Forms.CheckBox cbImageShackIsPublic;
        private System.Windows.Forms.Button btnImageShackOpenMyImages;
        private System.Windows.Forms.Label lblImageShackUsername;
        private System.Windows.Forms.TextBox txtImageShackUsername;
        private System.Windows.Forms.TextBox txtImageShackPassword;
        private System.Windows.Forms.Label lblImageShackPassword;
        private System.Windows.Forms.GroupBox gbPhotobucketAlbumPath;
        private System.Windows.Forms.Button btnPhotobucketAddAlbum;
        private System.Windows.Forms.Button btnPhotobucketRemoveAlbum;
        private System.Windows.Forms.ComboBox cbPhotobucketAlbumPaths;
        private System.Windows.Forms.GroupBox gbPhotobucketAlbums;
        private System.Windows.Forms.Label lblPhotobucketNewAlbumName;
        private System.Windows.Forms.Label lblPhotobucketParentAlbumPath;
        private System.Windows.Forms.TextBox txtPhotobucketNewAlbumName;
        private System.Windows.Forms.TextBox txtPhotobucketParentAlbumPath;
        private System.Windows.Forms.Button btnPhotobucketCreateAlbum;
        private System.Windows.Forms.GroupBox gbPhotobucketUserAccount;
        private System.Windows.Forms.Label lblPhotobucketDefaultAlbumName;
        private System.Windows.Forms.Button btnPhotobucketAuthOpen;
        private System.Windows.Forms.TextBox txtPhotobucketDefaultAlbumName;
        private System.Windows.Forms.Label lblPhotobucketVerificationCode;
        private System.Windows.Forms.Button btnPhotobucketAuthComplete;
        private System.Windows.Forms.TextBox txtPhotobucketVerificationCode;
        private System.Windows.Forms.Label lblPhotobucketAccountStatus;
        private System.Windows.Forms.TabControl tcUploaders;
        private ShareX.HelpersLib.ExportImportControl eiFTP;
        private OAuthControl oauth2Dropbox;
        private System.Windows.Forms.TextBox txtBitlyDomain;
        private System.Windows.Forms.Label lblBitlyDomain;
        private System.Windows.Forms.TextBox txtGoogleDriveFolderID;
        private System.Windows.Forms.Label lblGoogleDriveFolderID;
        private ShareX.HelpersLib.MyListView lvGoogleDriveFoldersList;
        private System.Windows.Forms.ColumnHeader chGoogleDriveTitle;
        private System.Windows.Forms.Button btnGoogleDriveRefreshFolders;
        private System.Windows.Forms.ColumnHeader chGoogleDriveDescription;
        private System.Windows.Forms.CheckBox cbGoogleDriveUseFolder;
        private System.Windows.Forms.TextBox txtOwnCloudPath;
        private System.Windows.Forms.TextBox txtOwnCloudPassword;
        private System.Windows.Forms.TextBox txtOwnCloudUsername;
        private System.Windows.Forms.TextBox txtOwnCloudHost;
        private System.Windows.Forms.Label lblOwnCloudPath;
        private System.Windows.Forms.Label lblOwnCloudPassword;
        private System.Windows.Forms.Label lblOwnCloudUsername;
        private System.Windows.Forms.Label lblOwnCloudHost;
        private System.Windows.Forms.CheckBox cbOwnCloudCreateShare;
        private System.Windows.Forms.CheckBox cbOwnCloudDirectLink;
        private System.Windows.Forms.CheckBox cbImgurDirectLink;
        private System.Windows.Forms.TextBox txtMediaFirePassword;
        private System.Windows.Forms.TextBox txtMediaFireEmail;
        private System.Windows.Forms.Label lblMediaFirePassword;
        private System.Windows.Forms.Label lblMediaFireEmail;
        private System.Windows.Forms.TextBox txtMediaFirePath;
        private System.Windows.Forms.Label lblMediaFirePath;
        private System.Windows.Forms.CheckBox cbMediaFireUseLongLink;
        private OAuthControl oAuth2OneDrive;
        private System.Windows.Forms.CheckBox cbImgurUploadSelectedAlbum;
        private System.Windows.Forms.Label lblPastebinLoginStatus;
        private System.Windows.Forms.TextBox txtPastebinTitle;
        private System.Windows.Forms.TextBox txtPastebinPassword;
        private System.Windows.Forms.TextBox txtPastebinUsername;
        private System.Windows.Forms.Label lblPastebinExpiration;
        private System.Windows.Forms.Label lblPastebinPrivacy;
        private System.Windows.Forms.Label lblPastebinTitle;
        private System.Windows.Forms.Label lblPastebinPassword;
        private System.Windows.Forms.Label lblPastebinUsername;
        private System.Windows.Forms.ComboBox cbPastebinExpiration;
        private System.Windows.Forms.ComboBox cbPastebinPrivacy;
        private System.Windows.Forms.Label lblPastebinSyntax;
        private System.Windows.Forms.Button btnPastebinRegister;
        private System.Windows.Forms.Label lblCheveretoUploadURL;
        private System.Windows.Forms.TextBox txtCheveretoUploadURL;
        private System.Windows.Forms.TextBox txtCheveretoAPIKey;
        private System.Windows.Forms.Label lblCheveretoAPIKey;
        private System.Windows.Forms.CheckBox cbCheveretoDirectURL;
        private System.Windows.Forms.ComboBox cbPastebinSyntax;
        private System.Windows.Forms.TextBox txtHastebinSyntaxHighlighting;
        private System.Windows.Forms.TextBox txtHastebinCustomDomain;
        private System.Windows.Forms.Label lblHastebinSyntaxHighlighting;
        private System.Windows.Forms.Label lblHastebinCustomDomain;
        private System.Windows.Forms.CheckBox cbOneDriveCreateShareableLink;
        private System.Windows.Forms.CheckBox cbOneDriveUseDirectLink;
        private System.Windows.Forms.Label lblOneDriveFolderID;
        private System.Windows.Forms.TreeView tvOneDrive;
        private System.Windows.Forms.Label lblLambdaApiKey;
        private System.Windows.Forms.TextBox txtLambdaApiKey;
        private System.Windows.Forms.Label lblLambdaInfo;
        private System.Windows.Forms.Label lblLambdaUploadURL;
        private System.Windows.Forms.ComboBox cbLambdaUploadURL;
        private System.Windows.Forms.Label lblLithiioApiKey;
        private System.Windows.Forms.TextBox txtLithiioApiKey;
        private System.Windows.Forms.CheckBox cbOwnCloud81Compatibility;
        private System.Windows.Forms.Label lblOneTimeSecretAPIKey;
        private System.Windows.Forms.Label lblOneTimeSecretEmail;
        private System.Windows.Forms.TextBox txtOneTimeSecretAPIKey;
        private System.Windows.Forms.TextBox txtOneTimeSecretEmail;
        private System.Windows.Forms.TextBox txtPolrAPIKey;
        private System.Windows.Forms.Label lblPolrAPIKey;
        private System.Windows.Forms.TextBox txtPolrAPIHostname;
        private System.Windows.Forms.Label lblPolrAPIHostname;
        private System.Windows.Forms.CheckBox cbImgurUseGIFV;
        private System.Windows.Forms.Label lblPomfResultURL;
        private System.Windows.Forms.Label lblPomfUploadURL;
        private System.Windows.Forms.TextBox txtPomfUploadURL;
        private System.Windows.Forms.TextBox txtPomfResultURL;
        private System.Windows.Forms.Button btnSeafileCheckAuthToken;
        private System.Windows.Forms.Button btnSeafileCheckAPIURL;
        private System.Windows.Forms.GroupBox grpSeafileObtainAuthToken;
        private System.Windows.Forms.Button btnSeafileGetAuthToken;
        private System.Windows.Forms.TextBox txtSeafilePassword;
        private System.Windows.Forms.TextBox txtSeafileUsername;
        private System.Windows.Forms.Label lblSeafileUsername;
        private System.Windows.Forms.Label lblSeafilePassword;
        private System.Windows.Forms.CheckBox cbSeafileCreateShareableURL;
        private System.Windows.Forms.CheckBox cbSeafileCreateShareableURLRaw;
        private System.Windows.Forms.TextBox txtSeafileAuthToken;
        private System.Windows.Forms.Label lblSeafileAuthToken;
        private System.Windows.Forms.Label lblSeafileAPIURL;
        private System.Windows.Forms.GroupBox grpSeafileAccInfo;
        private System.Windows.Forms.Button btnRefreshSeafileAccInfo;
        private System.Windows.Forms.TextBox txtSeafileAccInfoUsage;
        private System.Windows.Forms.TextBox txtSeafileAccInfoEmail;
        private System.Windows.Forms.Label lblSeafileAccInfoEmail;
        private System.Windows.Forms.Label lblSeafileAccInfoUsage;
        private System.Windows.Forms.Button txtSeafileUploadLocationRefresh;
        private System.Windows.Forms.Label lblSeafileSelectLibrary;
        private System.Windows.Forms.Label lblSeafileWritePermNotif;
        private HelpersLib.MyListView lvSeafileLibraries;
        private System.Windows.Forms.ColumnHeader colSeafileLibraryName;
        private System.Windows.Forms.Button btnSeafilePathValidate;
        private System.Windows.Forms.TextBox txtSeafileDirectoryPath;
        private System.Windows.Forms.Label lblSeafilePath;
        private System.Windows.Forms.ColumnHeader colSeafileLibrarySize;
        private System.Windows.Forms.ColumnHeader colSeafileLibraryEncrypted;
        private System.Windows.Forms.Button btnSeafileLibraryPasswordValidate;
        private System.Windows.Forms.TextBox txtSeafileLibraryPassword;
        private System.Windows.Forms.Label lblSeafileLibraryPassword;
        private System.Windows.Forms.GroupBox grpSeafileShareSettings;
        private System.Windows.Forms.TextBox txtSeafileSharePassword;
        private System.Windows.Forms.Label lblSeafileSharePassword;
        private System.Windows.Forms.NumericUpDown nudSeafileExpireDays;
        private System.Windows.Forms.Label lblSeafileDaysToExpire;
        private System.Windows.Forms.ComboBox cbSeafileAPIURL;
        private System.Windows.Forms.TextBox txtStreamablePassword;
        private System.Windows.Forms.TextBox txtStreamableUsername;
        private System.Windows.Forms.Label lblStreamableUsername;
        private System.Windows.Forms.Label lblStreamablePassword;
        private System.Windows.Forms.Label lblSulAPIKey;
        private System.Windows.Forms.TextBox txtSulAPIKey;
        private System.Windows.Forms.TextBox txtVgymeUserKey;
        private System.Windows.Forms.Label lvlVgymeUserKey;
        private System.Windows.Forms.LinkLabel llVgymeAccountDetailsPage;
        private System.Windows.Forms.Label lblCheveretoUploadURLExample;
        private System.Windows.Forms.CheckBox cbPastebinRaw;
        private System.Windows.Forms.CheckBox cbGistUseRawURL;
        private System.Windows.Forms.CheckBox cbStreamableUseDirectURL;
        internal System.Windows.Forms.TabPage tpImgur;
        internal System.Windows.Forms.TabPage tpImageShack;
        internal System.Windows.Forms.TabPage tpFlickr;
        internal System.Windows.Forms.TabPage tpPhotobucket;
        internal System.Windows.Forms.TabPage tpChevereto;
        internal System.Windows.Forms.TabPage tpVgyme;
        internal System.Windows.Forms.TabPage tpPastebin;
        internal System.Windows.Forms.TabPage tpPaste_ee;
        internal System.Windows.Forms.TabPage tpGist;
        internal System.Windows.Forms.TabPage tpUpaste;
        internal System.Windows.Forms.TabPage tpHastebin;
        internal System.Windows.Forms.TabPage tpOneTimeSecret;
        internal System.Windows.Forms.TabPage tpDropbox;
        internal System.Windows.Forms.TabPage tpOneDrive;
        internal System.Windows.Forms.TabPage tpGoogleDrive;
        internal System.Windows.Forms.TabPage tpBox;
        internal System.Windows.Forms.TabPage tpAmazonS3;
        internal System.Windows.Forms.TabPage tpMega;
        internal System.Windows.Forms.TabPage tpOwnCloud;
        internal System.Windows.Forms.TabPage tpMediaFire;
        internal System.Windows.Forms.TabPage tpPushbullet;
        internal System.Windows.Forms.TabPage tpSendSpace;
        internal System.Windows.Forms.TabPage tpHostr;
        internal System.Windows.Forms.TabPage tpLambda;
        internal System.Windows.Forms.TabPage tpLithiio;
        internal System.Windows.Forms.TabPage tpPomf;
        internal System.Windows.Forms.TabPage tpSeafile;
        internal System.Windows.Forms.TabPage tpSul;
        internal System.Windows.Forms.TabPage tpStreamable;
        internal System.Windows.Forms.TabPage tpSharedFolder;
        internal System.Windows.Forms.TabPage tpEmail;
        internal System.Windows.Forms.TabPage tpBitly;
        internal System.Windows.Forms.TabPage tpYourls;
        internal System.Windows.Forms.TabPage tpPolr;
        private System.Windows.Forms.TextBox txtEmailAutomaticSendTo;
        private System.Windows.Forms.CheckBox cbEmailAutomaticSend;
        private System.Windows.Forms.Button btnLithiioGetAPIKey;
        private System.Windows.Forms.CheckBox cbGoogleDriveDirectLink;
        private System.Windows.Forms.Label lblPuushAPIKey;
        private System.Windows.Forms.TextBox txtPuushAPIKey;
        private System.Windows.Forms.LinkLabel llPuushForgottenPassword;
        private System.Windows.Forms.Button btnPuushLogin;
        private System.Windows.Forms.TextBox txtPuushPassword;
        private System.Windows.Forms.TextBox txtPuushEmail;
        private System.Windows.Forms.Label lblPuushEmail;
        private System.Windows.Forms.Label lblPuushPassword;
        internal System.Windows.Forms.TabPage tpPuush;
        private System.Windows.Forms.CheckBox cbHastebinUseFileExtension;
        private System.Windows.Forms.Label lblOwnCloudHostExample;
        internal System.Windows.Forms.TabPage tpPastie;
        private System.Windows.Forms.CheckBox cbPastieIsPublic;
        private System.Windows.Forms.CheckBox cbPolrUseAPIv1;
        private System.Windows.Forms.CheckBox cbPolrIsSecret;
        internal System.Windows.Forms.TabPage tpAzureStorage;
        private System.Windows.Forms.Label lblAzureStorageAccessKey;
        private System.Windows.Forms.TextBox txtAzureStorageAccountName;
        private System.Windows.Forms.Label lblAzureStorageAccountName;
        private System.Windows.Forms.TextBox txtAzureStorageAccessKey;
        private System.Windows.Forms.TextBox txtAzureStorageContainer;
        private System.Windows.Forms.Label lblAzureStorageContainer;
        private System.Windows.Forms.Button btnAzureStoragePortal;
        private System.Windows.Forms.ComboBox cbAzureStorageEnvironment;
        private System.Windows.Forms.Label lblAzureStorageEnvironment;
        private System.Windows.Forms.TextBox txtAzureStorageCustomDomain;
        private System.Windows.Forms.Label lblAzureStorageCustomDomain;
        internal System.Windows.Forms.TabPage tpPlik;
        private System.Windows.Forms.GroupBox gbPlikSettings;
        private System.Windows.Forms.TextBox txtPlikComment;
        private System.Windows.Forms.CheckBox cbPlikComment;
        private System.Windows.Forms.CheckBox cbPlikRemovable;
        private System.Windows.Forms.GroupBox gbPlikLoginCredentials;
        private System.Windows.Forms.CheckBox cbPlikIsSecured;
        private System.Windows.Forms.Label lblPlikAPIKey;
        private System.Windows.Forms.TextBox txtPlikAPIKey;
        private System.Windows.Forms.Label lblPlikPassword;
        private System.Windows.Forms.Label lblPlikUsername;
        private System.Windows.Forms.TextBox txtPlikPassword;
        private System.Windows.Forms.TextBox txtPlikLogin;
        private System.Windows.Forms.Label lblPlikURL;
        private System.Windows.Forms.TextBox txtPlikURL;
        private System.Windows.Forms.CheckBox cbPlikOneShot;
        private System.Windows.Forms.ComboBox cbPlikTTLUnit;
        private System.Windows.Forms.NumericUpDown nudPlikTTL;
        private System.Windows.Forms.Label lblPlikTTL;
        private System.Windows.Forms.TextBox txtGistCustomURL;
        private System.Windows.Forms.Label lblGistCustomURL;
        private System.Windows.Forms.Label lblGistOAuthInfo;
        private System.Windows.Forms.Label lblGistCustomURLExample;
        private System.Windows.Forms.TextBox txtAmazonS3Region;
        private System.Windows.Forms.TextBox txtAmazonS3Endpoint;
        private System.Windows.Forms.Label lblAmazonS3Region;
        private System.Windows.Forms.Label lblAmazonS3Endpoint;
        private System.Windows.Forms.CheckBox cbDropboxUseDirectLink;
        private System.Windows.Forms.CheckBox cbAmazonS3UsePathStyle;
        private System.Windows.Forms.Panel pFTPTransferMode;
        private System.Windows.Forms.RadioButton rbFTPTransferModeActive;
        private System.Windows.Forms.RadioButton rbFTPTransferModePassive;
        private System.Windows.Forms.Panel pFTPProtocol;
        private System.Windows.Forms.RadioButton rbFTPProtocolFTP;
        private System.Windows.Forms.RadioButton rbFTPProtocolFTPS;
        private System.Windows.Forms.RadioButton rbFTPProtocolSFTP;
        private System.Windows.Forms.Label lblFTPTransferMode;
        private System.Windows.Forms.Label lblFTPURLPreviewValue;
        private System.Windows.Forms.Label lblFTPURLPreview;
        private System.Windows.Forms.ComboBox cbFTPURLPathProtocol;
        private System.Windows.Forms.TextBox txtFTPURLPath;
        private System.Windows.Forms.Label lblFTPURLPath;
        private System.Windows.Forms.TextBox txtFTPRemoteDirectory;
        private System.Windows.Forms.Label lblFTPRemoteDirectory;
        private System.Windows.Forms.Button btnFTPRemove;
        private System.Windows.Forms.Button btnFTPAdd;
        private System.Windows.Forms.ComboBox cbFTPAccounts;
        private System.Windows.Forms.Label lblFTPAccounts;
        private System.Windows.Forms.TextBox txtFTPPassword;
        private System.Windows.Forms.Label lblFTPPassword;
        private System.Windows.Forms.TextBox txtFTPUsername;
        private System.Windows.Forms.Label lblFTPUsername;
        private System.Windows.Forms.NumericUpDown nudFTPPort;
        private System.Windows.Forms.Label lblFTPPort;
        private System.Windows.Forms.TextBox txtFTPHost;
        private System.Windows.Forms.Label lblFTPHost;
        private System.Windows.Forms.TextBox txtFTPName;
        private System.Windows.Forms.Label lblFTPName;
        private System.Windows.Forms.Label lblFTPProtocol;
        private System.Windows.Forms.CheckBox cbFTPRemoveFileExtension;
        private System.Windows.Forms.CheckBox cbFTPAppendRemoteDirectory;
        private System.Windows.Forms.Button btnFTPDuplicate;
        private System.Windows.Forms.Button btnFTPTest;
        private System.Windows.Forms.GroupBox gbFTPAccount;
        private System.Windows.Forms.GroupBox gbFTPS;
        private System.Windows.Forms.TextBox txtFTPSCertificateLocation;
        private System.Windows.Forms.Label lblFTPSCertificateLocation;
        private System.Windows.Forms.ComboBox cbFTPSEncryption;
        private System.Windows.Forms.Label lblFTPSEncryption;
        private System.Windows.Forms.Button btnFTPSCertificateLocationBrowse;
        private System.Windows.Forms.GroupBox gbSFTP;
        private System.Windows.Forms.TextBox txtSFTPKeyLocation;
        private System.Windows.Forms.Label lblSFTPKeyLocation;
        private System.Windows.Forms.TextBox txtSFTPKeyPassphrase;
        private System.Windows.Forms.Button btnSFTPKeyLocationBrowse;
        private System.Windows.Forms.Label lblSFTPKeyPassphrase;
        private System.Windows.Forms.Label lblAmazonS3StorageClass;
        private System.Windows.Forms.ComboBox cbAmazonS3StorageClass;
        private System.Windows.Forms.Button btnAmazonS3StorageClassHelp;
        private System.Windows.Forms.Button btnPaste_eeGetUserKey;
        private OAuthControl oauthFlickr;
        private System.Windows.Forms.CheckBox cbFlickrDirectLink;
        private System.Windows.Forms.Button btnSulGetAPIKey;
        private System.Windows.Forms.Button btnLithiioFetchAPIKey;
        private System.Windows.Forms.TextBox txtLithiioPassword;
        private System.Windows.Forms.TextBox txtLithiioEmail;
        private System.Windows.Forms.Label lblLithiioPassword;
        private System.Windows.Forms.Label lblLithiioEmail;
        private System.Windows.Forms.CheckBox cbAmazonS3PublicACL;
        private System.Windows.Forms.GroupBox gbAmazonS3Advanced;
        private System.Windows.Forms.Label lblAmazonS3StripExtension;
        private System.Windows.Forms.CheckBox cbAmazonS3StripExtensionText;
        private System.Windows.Forms.CheckBox cbAmazonS3StripExtensionVideo;
        private System.Windows.Forms.CheckBox cbAmazonS3StripExtensionImage;
        private System.Windows.Forms.CheckBox cbOwnCloudUsePreviewLinks;
        internal System.Windows.Forms.TabPage tpFirebaseDynamicLinks;
        private System.Windows.Forms.CheckBox cbFirebaseIsShort;
        private System.Windows.Forms.TextBox txtFirebaseDomain;
        private System.Windows.Forms.TextBox txtFirebaseWebAPIKey;
        private System.Windows.Forms.Label lblFirebaseWebAPIKey;
        private System.Windows.Forms.ComboBox cbYouTubePrivacyType;
        private System.Windows.Forms.Label lblYouTubePrivacyType;
        internal System.Windows.Forms.TabPage tpYouTube;
        private System.Windows.Forms.CheckBox cbYouTubeUseShortenedLink;
        internal System.Windows.Forms.TabPage tpGoogleCloudStorage;
        private System.Windows.Forms.TextBox txtGoogleCloudStorageBucket;
        private System.Windows.Forms.Label lblGoogleCloudStorageBucket;
        private System.Windows.Forms.TextBox txtGoogleCloudStorageDomain;
        private System.Windows.Forms.Label lblGoogleCloudStorageDomain;
        private System.Windows.Forms.TextBox txtGoogleCloudStorageObjectPrefix;
        private System.Windows.Forms.Label lblGoogleCloudStorageObjectPrefix;
        private System.Windows.Forms.Button btnSharedFolderDuplicate;
        private System.Windows.Forms.Button btnSharedFolderRemove;
        private System.Windows.Forms.Button btnSharedFolderAdd;
        private System.Windows.Forms.PropertyGrid pgSharedFolderAccount;
        private System.Windows.Forms.ListBox lbSharedFolderAccounts;
        private System.Windows.Forms.Label lblGoogleCloudStoragePathPreviewLabel;
        private System.Windows.Forms.Label lblGoogleCloudStoragePathPreview;
        private System.Windows.Forms.TextBox txtAzureStorageUploadPath;
        private System.Windows.Forms.Label lblAzureStorageUploadPath;
        private System.Windows.Forms.Label lblFirebaseDomain;
        private System.Windows.Forms.Label lblAzureStorageURLPreview;
        private System.Windows.Forms.Label lblAzureStorageURLPreviewLabel;
        private System.Windows.Forms.Label lblFirebaseDomainExample;
        private System.Windows.Forms.Label lblOwnCloudExpiryTime;
        private System.Windows.Forms.CheckBox cbOwnCloudAutoExpire;
        private System.Windows.Forms.NumericUpDown nudOwnCloudExpiryTime;
        internal System.Windows.Forms.TabPage tpBackblazeB2;
        private System.Windows.Forms.TextBox txtB2CustomUrl;
        private System.Windows.Forms.Label lblB2UrlPreviewLabel;
        private System.Windows.Forms.CheckBox cbB2CustomUrl;
        private System.Windows.Forms.Label lblB2Bucket;
        private System.Windows.Forms.TextBox txtB2Bucket;
        private System.Windows.Forms.TextBox txtB2UploadPath;
        private System.Windows.Forms.Label lblB2UploadPath;
        private System.Windows.Forms.TextBox txtB2ApplicationKey;
        private System.Windows.Forms.Label lblB2ApplicationKey;
        private System.Windows.Forms.Label lblB2ApplicationKeyId;
        private System.Windows.Forms.TextBox txtB2ApplicationKeyId;
        private System.Windows.Forms.LinkLabel lblB2ManageLink;
        private System.Windows.Forms.TextBox txtKuttAPIKey;
        private System.Windows.Forms.TextBox txtKuttHost;
        private System.Windows.Forms.Label lblKuttAPIKey;
        private System.Windows.Forms.Label lblKuttHost;
        private System.Windows.Forms.CheckBox cbKuttReuse;
        private System.Windows.Forms.Label lblKuttPassword;
        private System.Windows.Forms.TextBox txtKuttPassword;
        internal System.Windows.Forms.TabPage tpKutt;
        private System.Windows.Forms.CheckBox cbAmazonS3SignedPayload;
        private System.Windows.Forms.GroupBox gbGoogleCloudStorageAdvanced;
        private System.Windows.Forms.Label lblGoogleCloudStorageStripExtension;
        private System.Windows.Forms.CheckBox cbGoogleCloudStorageStripExtensionText;
        private System.Windows.Forms.CheckBox cbGoogleCloudStorageStripExtensionVideo;
        private System.Windows.Forms.CheckBox cbGoogleCloudStorageSetPublicACL;
        private System.Windows.Forms.CheckBox cbGoogleCloudStorageStripExtensionImage;
        private System.Windows.Forms.Label lblB2UrlPreview;
        private HelpersLib.TabToTreeView tttvMain;
        private System.Windows.Forms.ComboBox cbGoogleDriveSharedDrive;
        private System.Windows.Forms.TextBox txtKuttDomain;
        private System.Windows.Forms.Label lblKuttDomain;
        internal System.Windows.Forms.TabPage tpZeroWidthShortener;
        private System.Windows.Forms.TextBox txtZWSToken;
        private System.Windows.Forms.TextBox txtZWSURL;
        private System.Windows.Forms.Label lblZWSToken;
        private System.Windows.Forms.Label lblZWSURL;
        private System.Windows.Forms.CheckBox cbOwnCloudAppendFileNameToURL;
        private System.Windows.Forms.CheckBox cbYouTubeShowDialog;
        private System.Windows.Forms.LinkLabel llYouTubePermissionsLink;
        private System.Windows.Forms.Label lblYouTubePermissionsTip;
        private OAuthLoopbackControl oauth2YouTube;
        private OAuthLoopbackControl oauth2GoogleDrive;
        private OAuthLoopbackControl oauth2GoogleCloudStorage;
        private System.Windows.Forms.TextBox txtAzureStorageCacheControl;
        private System.Windows.Forms.Label lblAzureStorageCacheControl;
        private System.Windows.Forms.Button btnGoogleDriveFolderIDHelp;
    }
}