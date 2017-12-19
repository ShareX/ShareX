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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadersConfigForm));
            this.txtRapidSharePremiumUserName = new System.Windows.Forms.TextBox();
            this.ttHelpTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbAmazonS3CustomCNAME = new System.Windows.Forms.CheckBox();
            this.mbCustomUploaderDestinationType = new ShareX.HelpersLib.MenuButton();
            this.cmsCustomUploaderDestinationType = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tpOtherUploaders = new System.Windows.Forms.TabPage();
            this.tcOtherUploaders = new System.Windows.Forms.TabControl();
            this.tpTwitter = new System.Windows.Forms.TabPage();
            this.btnTwitterNameUpdate = new System.Windows.Forms.Button();
            this.lbTwitterAccounts = new System.Windows.Forms.ListBox();
            this.lblTwitterDefaultMessage = new System.Windows.Forms.Label();
            this.txtTwitterDefaultMessage = new System.Windows.Forms.TextBox();
            this.cbTwitterSkipMessageBox = new System.Windows.Forms.CheckBox();
            this.oauthTwitter = new ShareX.UploadersLib.OAuthControl();
            this.txtTwitterDescription = new System.Windows.Forms.TextBox();
            this.lblTwitterDescription = new System.Windows.Forms.Label();
            this.btnTwitterRemove = new System.Windows.Forms.Button();
            this.btnTwitterAdd = new System.Windows.Forms.Button();
            this.tpCustomUploaders = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderURLSharingServiceTest = new System.Windows.Forms.Button();
            this.cbCustomUploaderURLSharingService = new System.Windows.Forms.ComboBox();
            this.lblCustomUploaderURLSharingService = new System.Windows.Forms.Label();
            this.pCustomUploader = new System.Windows.Forms.Panel();
            this.lblCustomUploaderName = new System.Windows.Forms.Label();
            this.cbCustomUploaderRequestType = new System.Windows.Forms.ComboBox();
            this.tcCustomUploaderResponseParse = new System.Windows.Forms.TabControl();
            this.tpCustomUploaderJsonParse = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderJsonAddSyntax = new System.Windows.Forms.Button();
            this.btnCustomUploadJsonPathHelp = new System.Windows.Forms.Button();
            this.lblCustomUploaderJsonPathExample = new System.Windows.Forms.Label();
            this.lblCustomUploaderJsonPath = new System.Windows.Forms.Label();
            this.txtCustomUploaderJsonPath = new System.Windows.Forms.TextBox();
            this.tpCustomUploaderXmlParse = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderXmlSyntaxAdd = new System.Windows.Forms.Button();
            this.btnCustomUploaderXPathHelp = new System.Windows.Forms.Button();
            this.lblCustomUploaderXPathExample = new System.Windows.Forms.Label();
            this.lblCustomUploaderXPath = new System.Windows.Forms.Label();
            this.txtCustomUploaderXPath = new System.Windows.Forms.TextBox();
            this.tpCustomUploaderRegexParse = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderRegexHelp = new System.Windows.Forms.Button();
            this.btnCustomUploaderRegexAddSyntax = new System.Windows.Forms.Button();
            this.txtCustomUploaderRegexp = new System.Windows.Forms.TextBox();
            this.btnCustomUploaderRegexpUpdate = new System.Windows.Forms.Button();
            this.btnCustomUploaderRegexpAdd = new System.Windows.Forms.Button();
            this.btnCustomUploaderRegexpRemove = new System.Windows.Forms.Button();
            this.lvCustomUploaderRegexps = new ShareX.HelpersLib.MyListView();
            this.lvRegexpsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblCustomUploaderURL = new System.Windows.Forms.Label();
            this.tcCustomUploaderArguments = new System.Windows.Forms.TabControl();
            this.tpCustomUploaderArguments = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderArgUpdate = new System.Windows.Forms.Button();
            this.txtCustomUploaderArgName = new System.Windows.Forms.TextBox();
            this.txtCustomUploaderArgValue = new System.Windows.Forms.TextBox();
            this.btnCustomUploaderArgAdd = new System.Windows.Forms.Button();
            this.btnCustomUploaderArgRemove = new System.Windows.Forms.Button();
            this.lvCustomUploaderArguments = new ShareX.HelpersLib.MyListView();
            this.chCustomUploaderArgumentsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCustomUploaderArgumentsValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpCustomUploaderHeaders = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderHeaderUpdate = new System.Windows.Forms.Button();
            this.txtCustomUploaderHeaderName = new System.Windows.Forms.TextBox();
            this.txtCustomUploaderHeaderValue = new System.Windows.Forms.TextBox();
            this.btnCustomUploaderHeaderAdd = new System.Windows.Forms.Button();
            this.btnCustomUploaderHeaderRemove = new System.Windows.Forms.Button();
            this.lvCustomUploaderHeaders = new ShareX.HelpersLib.MyListView();
            this.chCustomUploaderHeadersName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCustomUploaderHeadersValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtCustomUploaderFileForm = new System.Windows.Forms.TextBox();
            this.lblCustomUploaderRequestType = new System.Windows.Forms.Label();
            this.lblCustomUploaderFileForm = new System.Windows.Forms.Label();
            this.txtCustomUploaderName = new System.Windows.Forms.TextBox();
            this.lblCustomUploaderThumbnailURL = new System.Windows.Forms.Label();
            this.txtCustomUploaderRequestURL = new System.Windows.Forms.TextBox();
            this.txtCustomUploaderURL = new System.Windows.Forms.TextBox();
            this.cbCustomUploaderResponseType = new System.Windows.Forms.ComboBox();
            this.txtCustomUploaderThumbnailURL = new System.Windows.Forms.TextBox();
            this.txtCustomUploaderDeletionURL = new System.Windows.Forms.TextBox();
            this.lblCustomUploaderRequestURL = new System.Windows.Forms.Label();
            this.lblCustomUploaderResponseType = new System.Windows.Forms.Label();
            this.lblCustomUploaderDeletionURL = new System.Windows.Forms.Label();
            this.btnCustomUploaderExamples = new System.Windows.Forms.Button();
            this.btnCustomUploaderHelp = new System.Windows.Forms.Button();
            this.lblCustomUploaderImageUploader = new System.Windows.Forms.Label();
            this.btnCustomUploaderFileUploaderTest = new System.Windows.Forms.Button();
            this.lblCustomUploaderFileUploader = new System.Windows.Forms.Label();
            this.btnCustomUploaderImageUploaderTest = new System.Windows.Forms.Button();
            this.lblCustomUploaderTestResult = new System.Windows.Forms.Label();
            this.cbCustomUploaderFileUploader = new System.Windows.Forms.ComboBox();
            this.btnCustomUploaderShowLastResponse = new System.Windows.Forms.Button();
            this.cbCustomUploaderURLShortener = new System.Windows.Forms.ComboBox();
            this.gbCustomUploaders = new System.Windows.Forms.GroupBox();
            this.btnCustomUploaderDuplicate = new System.Windows.Forms.Button();
            this.btnCustomUploadersExportAll = new System.Windows.Forms.Button();
            this.btnCustomUploaderClearUploaders = new System.Windows.Forms.Button();
            this.eiCustomUploaders = new ShareX.HelpersLib.ExportImportControl();
            this.lbCustomUploaderList = new System.Windows.Forms.ListBox();
            this.btnCustomUploaderRemove = new System.Windows.Forms.Button();
            this.btnCustomUploaderAdd = new System.Windows.Forms.Button();
            this.lblCustomUploaderTextUploader = new System.Windows.Forms.Label();
            this.btnCustomUploaderURLShortenerTest = new System.Windows.Forms.Button();
            this.cbCustomUploaderTextUploader = new System.Windows.Forms.ComboBox();
            this.lblCustomUploaderURLShortener = new System.Windows.Forms.Label();
            this.btnCustomUploaderTextUploaderTest = new System.Windows.Forms.Button();
            this.cbCustomUploaderImageUploader = new System.Windows.Forms.ComboBox();
            this.txtCustomUploaderLog = new System.Windows.Forms.RichTextBox();
            this.tpURLShorteners = new System.Windows.Forms.TabPage();
            this.tcURLShorteners = new System.Windows.Forms.TabControl();
            this.tpBitly = new System.Windows.Forms.TabPage();
            this.txtBitlyDomain = new System.Windows.Forms.TextBox();
            this.lblBitlyDomain = new System.Windows.Forms.Label();
            this.oauth2Bitly = new ShareX.UploadersLib.OAuthControl();
            this.tpGoogleURLShortener = new System.Windows.Forms.TabPage();
            this.oauth2GoogleURLShortener = new ShareX.UploadersLib.OAuthControl();
            this.atcGoogleURLShortenerAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.tpYourls = new System.Windows.Forms.TabPage();
            this.txtYourlsPassword = new System.Windows.Forms.TextBox();
            this.txtYourlsUsername = new System.Windows.Forms.TextBox();
            this.txtYourlsSignature = new System.Windows.Forms.TextBox();
            this.lblYourlsNote = new System.Windows.Forms.Label();
            this.lblYourlsPassword = new System.Windows.Forms.Label();
            this.lblYourlsUsername = new System.Windows.Forms.Label();
            this.lblYourlsSignature = new System.Windows.Forms.Label();
            this.txtYourlsAPIURL = new System.Windows.Forms.TextBox();
            this.lblYourlsAPIURL = new System.Windows.Forms.Label();
            this.tpAdFly = new System.Windows.Forms.TabPage();
            this.llAdflyLink = new System.Windows.Forms.LinkLabel();
            this.txtAdflyAPIUID = new System.Windows.Forms.TextBox();
            this.lblAdflyAPIUID = new System.Windows.Forms.Label();
            this.txtAdflyAPIKEY = new System.Windows.Forms.TextBox();
            this.lblAdflyAPIKEY = new System.Windows.Forms.Label();
            this.tpCoinURL = new System.Windows.Forms.TabPage();
            this.txtCoinURLUUID = new System.Windows.Forms.TextBox();
            this.lblCoinURLUUID = new System.Windows.Forms.Label();
            this.tpPolr = new System.Windows.Forms.TabPage();
            this.cbPolrUseAPIv1 = new System.Windows.Forms.CheckBox();
            this.cbPolrIsSecret = new System.Windows.Forms.CheckBox();
            this.txtPolrAPIKey = new System.Windows.Forms.TextBox();
            this.lblPolrAPIKey = new System.Windows.Forms.Label();
            this.txtPolrAPIHostname = new System.Windows.Forms.TextBox();
            this.lblPolrAPIHostname = new System.Windows.Forms.Label();
            this.tpFileUploaders = new System.Windows.Forms.TabPage();
            this.tcFileUploaders = new System.Windows.Forms.TabControl();
            this.tpFTP = new System.Windows.Forms.TabPage();
            this.gbFTPAccount = new System.Windows.Forms.GroupBox();
            this.gbSFTP = new System.Windows.Forms.GroupBox();
            this.txtSFTPKeyPassphrase = new System.Windows.Forms.TextBox();
            this.btnSFTPKeyLocationBrowse = new System.Windows.Forms.Button();
            this.lblSFTPKeyPassphrase = new System.Windows.Forms.Label();
            this.txtSFTPKeyLocation = new System.Windows.Forms.TextBox();
            this.lblSFTPKeyLocation = new System.Windows.Forms.Label();
            this.cbFTPAppendRemoteDirectory = new System.Windows.Forms.CheckBox();
            this.btnFTPTest = new System.Windows.Forms.Button();
            this.lblFTPProtocol = new System.Windows.Forms.Label();
            this.lblFTPName = new System.Windows.Forms.Label();
            this.cbFTPRemoveFileExtension = new System.Windows.Forms.CheckBox();
            this.txtFTPName = new System.Windows.Forms.TextBox();
            this.lblFTPHost = new System.Windows.Forms.Label();
            this.eiFTP = new ShareX.HelpersLib.ExportImportControl();
            this.pFTPTransferMode = new System.Windows.Forms.Panel();
            this.rbFTPTransferModeActive = new System.Windows.Forms.RadioButton();
            this.rbFTPTransferModePassive = new System.Windows.Forms.RadioButton();
            this.btnFTPClient = new System.Windows.Forms.Button();
            this.txtFTPHost = new System.Windows.Forms.TextBox();
            this.pFTPProtocol = new System.Windows.Forms.Panel();
            this.rbFTPProtocolFTP = new System.Windows.Forms.RadioButton();
            this.rbFTPProtocolFTPS = new System.Windows.Forms.RadioButton();
            this.rbFTPProtocolSFTP = new System.Windows.Forms.RadioButton();
            this.lblFTPPort = new System.Windows.Forms.Label();
            this.lblFTPTransferMode = new System.Windows.Forms.Label();
            this.nudFTPPort = new System.Windows.Forms.NumericUpDown();
            this.lblFTPURLPreviewValue = new System.Windows.Forms.Label();
            this.lblFTPUsername = new System.Windows.Forms.Label();
            this.lblFTPURLPreview = new System.Windows.Forms.Label();
            this.txtFTPUsername = new System.Windows.Forms.TextBox();
            this.cbFTPURLPathProtocol = new System.Windows.Forms.ComboBox();
            this.lblFTPPassword = new System.Windows.Forms.Label();
            this.txtFTPURLPath = new System.Windows.Forms.TextBox();
            this.txtFTPPassword = new System.Windows.Forms.TextBox();
            this.lblFTPURLPath = new System.Windows.Forms.Label();
            this.lblFTPRemoteDirectory = new System.Windows.Forms.Label();
            this.txtFTPRemoteDirectory = new System.Windows.Forms.TextBox();
            this.gbFTPS = new System.Windows.Forms.GroupBox();
            this.btnFTPSCertificateLocationBrowse = new System.Windows.Forms.Button();
            this.txtFTPSCertificateLocation = new System.Windows.Forms.TextBox();
            this.lblFTPSCertificateLocation = new System.Windows.Forms.Label();
            this.cbFTPSEncryption = new System.Windows.Forms.ComboBox();
            this.lblFTPSEncryption = new System.Windows.Forms.Label();
            this.btnFTPDuplicate = new System.Windows.Forms.Button();
            this.btnFTPRemove = new System.Windows.Forms.Button();
            this.btnFTPAdd = new System.Windows.Forms.Button();
            this.cbFTPAccounts = new System.Windows.Forms.ComboBox();
            this.lblFTPAccounts = new System.Windows.Forms.Label();
            this.lblFTPFile = new System.Windows.Forms.Label();
            this.lblFTPText = new System.Windows.Forms.Label();
            this.lblFTPImage = new System.Windows.Forms.Label();
            this.cbFTPImage = new System.Windows.Forms.ComboBox();
            this.cbFTPFile = new System.Windows.Forms.ComboBox();
            this.cbFTPText = new System.Windows.Forms.ComboBox();
            this.tpDropbox = new System.Windows.Forms.TabPage();
            this.cbDropboxUseDirectLink = new System.Windows.Forms.CheckBox();
            this.cbDropboxAutoCreateShareableLink = new System.Windows.Forms.CheckBox();
            this.pbDropboxLogo = new System.Windows.Forms.PictureBox();
            this.lblDropboxPath = new System.Windows.Forms.Label();
            this.txtDropboxPath = new System.Windows.Forms.TextBox();
            this.oauth2Dropbox = new ShareX.UploadersLib.OAuthControl();
            this.tpOneDrive = new System.Windows.Forms.TabPage();
            this.tvOneDrive = new System.Windows.Forms.TreeView();
            this.lblOneDriveFolderID = new System.Windows.Forms.Label();
            this.cbOneDriveCreateShareableLink = new System.Windows.Forms.CheckBox();
            this.oAuth2OneDrive = new ShareX.UploadersLib.OAuthControl();
            this.tpGoogleDrive = new System.Windows.Forms.TabPage();
            this.cbGoogleDriveDirectLink = new System.Windows.Forms.CheckBox();
            this.cbGoogleDriveUseFolder = new System.Windows.Forms.CheckBox();
            this.txtGoogleDriveFolderID = new System.Windows.Forms.TextBox();
            this.lblGoogleDriveFolderID = new System.Windows.Forms.Label();
            this.lvGoogleDriveFoldersList = new ShareX.HelpersLib.MyListView();
            this.chGoogleDriveTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGoogleDriveDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnGoogleDriveRefreshFolders = new System.Windows.Forms.Button();
            this.cbGoogleDriveIsPublic = new System.Windows.Forms.CheckBox();
            this.oauth2GoogleDrive = new ShareX.UploadersLib.OAuthControl();
            this.tpPuush = new System.Windows.Forms.TabPage();
            this.pbPuush = new System.Windows.Forms.PictureBox();
            this.lblPuushAPIKey = new System.Windows.Forms.Label();
            this.txtPuushAPIKey = new System.Windows.Forms.TextBox();
            this.llPuushForgottenPassword = new System.Windows.Forms.LinkLabel();
            this.btnPuushLogin = new System.Windows.Forms.Button();
            this.txtPuushPassword = new System.Windows.Forms.TextBox();
            this.txtPuushEmail = new System.Windows.Forms.TextBox();
            this.lblPuushEmail = new System.Windows.Forms.Label();
            this.lblPuushPassword = new System.Windows.Forms.Label();
            this.tpBox = new System.Windows.Forms.TabPage();
            this.lblBoxFolderTip = new System.Windows.Forms.Label();
            this.cbBoxShare = new System.Windows.Forms.CheckBox();
            this.lvBoxFolders = new ShareX.HelpersLib.MyListView();
            this.chBoxFoldersName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblBoxFolderID = new System.Windows.Forms.Label();
            this.btnBoxRefreshFolders = new System.Windows.Forms.Button();
            this.oauth2Box = new ShareX.UploadersLib.OAuthControl();
            this.tpAmazonS3 = new System.Windows.Forms.TabPage();
            this.btnAmazonS3StorageClassHelp = new System.Windows.Forms.Button();
            this.lblAmazonS3StorageClass = new System.Windows.Forms.Label();
            this.cbAmazonS3StorageClass = new System.Windows.Forms.ComboBox();
            this.cbAmazonS3UsePathStyle = new System.Windows.Forms.CheckBox();
            this.lblAmazonS3Endpoint = new System.Windows.Forms.Label();
            this.txtAmazonS3Endpoint = new System.Windows.Forms.TextBox();
            this.lblAmazonS3Region = new System.Windows.Forms.Label();
            this.txtAmazonS3Region = new System.Windows.Forms.TextBox();
            this.txtAmazonS3CustomDomain = new System.Windows.Forms.TextBox();
            this.lblAmazonS3PathPreviewLabel = new System.Windows.Forms.Label();
            this.lblAmazonS3PathPreview = new System.Windows.Forms.Label();
            this.btnAmazonS3BucketNameOpen = new System.Windows.Forms.Button();
            this.btnAmazonS3AccessKeyOpen = new System.Windows.Forms.Button();
            this.cbAmazonS3Endpoints = new System.Windows.Forms.ComboBox();
            this.lblAmazonS3BucketName = new System.Windows.Forms.Label();
            this.txtAmazonS3BucketName = new System.Windows.Forms.TextBox();
            this.lblAmazonS3Endpoints = new System.Windows.Forms.Label();
            this.txtAmazonS3ObjectPrefix = new System.Windows.Forms.TextBox();
            this.lblAmazonS3ObjectPrefix = new System.Windows.Forms.Label();
            this.txtAmazonS3SecretKey = new System.Windows.Forms.TextBox();
            this.lblAmazonS3SecretKey = new System.Windows.Forms.Label();
            this.lblAmazonS3AccessKey = new System.Windows.Forms.Label();
            this.txtAmazonS3AccessKey = new System.Windows.Forms.TextBox();
            this.tpAzureStorage = new System.Windows.Forms.TabPage();
            this.cbAzureStorageEnvironment = new System.Windows.Forms.ComboBox();
            this.lblAzureStorageEnvironment = new System.Windows.Forms.Label();
            this.btnAzureStoragePortal = new System.Windows.Forms.Button();
            this.txtAzureStorageContainer = new System.Windows.Forms.TextBox();
            this.lblAzureStorageContainer = new System.Windows.Forms.Label();
            this.txtAzureStorageAccessKey = new System.Windows.Forms.TextBox();
            this.lblAzureStorageAccessKey = new System.Windows.Forms.Label();
            this.txtAzureStorageAccountName = new System.Windows.Forms.TextBox();
            this.lblAzureStorageAccountName = new System.Windows.Forms.Label();
            this.txtAzureStorageCustomDomain = new System.Windows.Forms.TextBox();
            this.lblAzureStorageCustomDomain = new System.Windows.Forms.Label();
            this.tpGfycat = new System.Windows.Forms.TabPage();
            this.cbGfycatIsPublic = new System.Windows.Forms.CheckBox();
            this.atcGfycatAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.oauth2Gfycat = new ShareX.UploadersLib.OAuthControl();
            this.tpMega = new System.Windows.Forms.TabPage();
            this.btnMegaRefreshFolders = new System.Windows.Forms.Button();
            this.lblMegaStatus = new System.Windows.Forms.Label();
            this.btnMegaRegister = new System.Windows.Forms.Button();
            this.lblMegaFolder = new System.Windows.Forms.Label();
            this.lblMegaStatusTitle = new System.Windows.Forms.Label();
            this.cbMegaFolder = new System.Windows.Forms.ComboBox();
            this.lblMegaEmail = new System.Windows.Forms.Label();
            this.btnMegaLogin = new System.Windows.Forms.Button();
            this.txtMegaEmail = new System.Windows.Forms.TextBox();
            this.txtMegaPassword = new System.Windows.Forms.TextBox();
            this.lblMegaPassword = new System.Windows.Forms.Label();
            this.tpOwnCloud = new System.Windows.Forms.TabPage();
            this.lblOwnCloudHostExample = new System.Windows.Forms.Label();
            this.cbOwnCloud81Compatibility = new System.Windows.Forms.CheckBox();
            this.cbOwnCloudDirectLink = new System.Windows.Forms.CheckBox();
            this.cbOwnCloudCreateShare = new System.Windows.Forms.CheckBox();
            this.txtOwnCloudPath = new System.Windows.Forms.TextBox();
            this.txtOwnCloudPassword = new System.Windows.Forms.TextBox();
            this.txtOwnCloudUsername = new System.Windows.Forms.TextBox();
            this.txtOwnCloudHost = new System.Windows.Forms.TextBox();
            this.lblOwnCloudPath = new System.Windows.Forms.Label();
            this.lblOwnCloudPassword = new System.Windows.Forms.Label();
            this.lblOwnCloudUsername = new System.Windows.Forms.Label();
            this.lblOwnCloudHost = new System.Windows.Forms.Label();
            this.tpMediaFire = new System.Windows.Forms.TabPage();
            this.cbMediaFireUseLongLink = new System.Windows.Forms.CheckBox();
            this.txtMediaFirePath = new System.Windows.Forms.TextBox();
            this.lblMediaFirePath = new System.Windows.Forms.Label();
            this.txtMediaFirePassword = new System.Windows.Forms.TextBox();
            this.txtMediaFireEmail = new System.Windows.Forms.TextBox();
            this.lblMediaFirePassword = new System.Windows.Forms.Label();
            this.lblMediaFireEmail = new System.Windows.Forms.Label();
            this.tpPushbullet = new System.Windows.Forms.TabPage();
            this.lblPushbulletDevices = new System.Windows.Forms.Label();
            this.cboPushbulletDevices = new System.Windows.Forms.ComboBox();
            this.btnPushbulletGetDeviceList = new System.Windows.Forms.Button();
            this.lblPushbulletUserKey = new System.Windows.Forms.Label();
            this.txtPushbulletUserKey = new System.Windows.Forms.TextBox();
            this.tpSendSpace = new System.Windows.Forms.TabPage();
            this.btnSendSpaceRegister = new System.Windows.Forms.Button();
            this.lblSendSpacePassword = new System.Windows.Forms.Label();
            this.lblSendSpaceUsername = new System.Windows.Forms.Label();
            this.txtSendSpacePassword = new System.Windows.Forms.TextBox();
            this.txtSendSpaceUserName = new System.Windows.Forms.TextBox();
            this.atcSendSpaceAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.tpGe_tt = new System.Windows.Forms.TabPage();
            this.lblGe_ttStatus = new System.Windows.Forms.Label();
            this.lblGe_ttPassword = new System.Windows.Forms.Label();
            this.lblGe_ttEmail = new System.Windows.Forms.Label();
            this.btnGe_ttLogin = new System.Windows.Forms.Button();
            this.txtGe_ttPassword = new System.Windows.Forms.TextBox();
            this.txtGe_ttEmail = new System.Windows.Forms.TextBox();
            this.tpHostr = new System.Windows.Forms.TabPage();
            this.cbLocalhostrDirectURL = new System.Windows.Forms.CheckBox();
            this.lblLocalhostrPassword = new System.Windows.Forms.Label();
            this.lblLocalhostrEmail = new System.Windows.Forms.Label();
            this.txtLocalhostrPassword = new System.Windows.Forms.TextBox();
            this.txtLocalhostrEmail = new System.Windows.Forms.TextBox();
            this.tpJira = new System.Windows.Forms.TabPage();
            this.txtJiraIssuePrefix = new System.Windows.Forms.TextBox();
            this.lblJiraIssuePrefix = new System.Windows.Forms.Label();
            this.gbJiraServer = new System.Windows.Forms.GroupBox();
            this.txtJiraConfigHelp = new System.Windows.Forms.TextBox();
            this.txtJiraHost = new System.Windows.Forms.TextBox();
            this.lblJiraHost = new System.Windows.Forms.Label();
            this.oAuthJira = new ShareX.UploadersLib.OAuthControl();
            this.tpLambda = new System.Windows.Forms.TabPage();
            this.lblLambdaInfo = new System.Windows.Forms.Label();
            this.lblLambdaApiKey = new System.Windows.Forms.Label();
            this.txtLambdaApiKey = new System.Windows.Forms.TextBox();
            this.lblLambdaUploadURL = new System.Windows.Forms.Label();
            this.cbLambdaUploadURL = new System.Windows.Forms.ComboBox();
            this.tpPomf = new System.Windows.Forms.TabPage();
            this.btnPomfTest = new System.Windows.Forms.Button();
            this.txtPomfResultURL = new System.Windows.Forms.TextBox();
            this.txtPomfUploadURL = new System.Windows.Forms.TextBox();
            this.lblPomfResultURL = new System.Windows.Forms.Label();
            this.lblPomfUploadURL = new System.Windows.Forms.Label();
            this.lblPomfUploaders = new System.Windows.Forms.Label();
            this.cbPomfUploaders = new System.Windows.Forms.ComboBox();
            this.tpSeafile = new System.Windows.Forms.TabPage();
            this.cbSeafileAPIURL = new System.Windows.Forms.ComboBox();
            this.grpSeafileShareSettings = new System.Windows.Forms.GroupBox();
            this.txtSeafileSharePassword = new System.Windows.Forms.TextBox();
            this.lblSeafileSharePassword = new System.Windows.Forms.Label();
            this.nudSeafileExpireDays = new System.Windows.Forms.NumericUpDown();
            this.lblSeafileDaysToExpire = new System.Windows.Forms.Label();
            this.btnSeafileLibraryPasswordValidate = new System.Windows.Forms.Button();
            this.txtSeafileLibraryPassword = new System.Windows.Forms.TextBox();
            this.lblSeafileLibraryPassword = new System.Windows.Forms.Label();
            this.lvSeafileLibraries = new ShareX.HelpersLib.MyListView();
            this.colSeafileLibraryName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSeafileLibrarySize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSeafileLibraryEncrypted = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSeafilePathValidate = new System.Windows.Forms.Button();
            this.txtSeafileDirectoryPath = new System.Windows.Forms.TextBox();
            this.lblSeafileWritePermNotif = new System.Windows.Forms.Label();
            this.lblSeafilePath = new System.Windows.Forms.Label();
            this.txtSeafileUploadLocationRefresh = new System.Windows.Forms.Button();
            this.lblSeafileSelectLibrary = new System.Windows.Forms.Label();
            this.grpSeafileAccInfo = new System.Windows.Forms.GroupBox();
            this.btnRefreshSeafileAccInfo = new System.Windows.Forms.Button();
            this.txtSeafileAccInfoUsage = new System.Windows.Forms.TextBox();
            this.txtSeafileAccInfoEmail = new System.Windows.Forms.TextBox();
            this.lblSeafileAccInfoEmail = new System.Windows.Forms.Label();
            this.lblSeafileAccInfoUsage = new System.Windows.Forms.Label();
            this.btnSeafileCheckAuthToken = new System.Windows.Forms.Button();
            this.btnSeafileCheckAPIURL = new System.Windows.Forms.Button();
            this.grpSeafileObtainAuthToken = new System.Windows.Forms.GroupBox();
            this.btnSeafileGetAuthToken = new System.Windows.Forms.Button();
            this.txtSeafilePassword = new System.Windows.Forms.TextBox();
            this.txtSeafileUsername = new System.Windows.Forms.TextBox();
            this.lblSeafileUsername = new System.Windows.Forms.Label();
            this.lblSeafilePassword = new System.Windows.Forms.Label();
            this.cbSeafileIgnoreInvalidCert = new System.Windows.Forms.CheckBox();
            this.cbSeafileCreateShareableURL = new System.Windows.Forms.CheckBox();
            this.txtSeafileAuthToken = new System.Windows.Forms.TextBox();
            this.lblSeafileAuthToken = new System.Windows.Forms.Label();
            this.lblSeafileAPIURL = new System.Windows.Forms.Label();
            this.tpStreamable = new System.Windows.Forms.TabPage();
            this.cbStreamableUseDirectURL = new System.Windows.Forms.CheckBox();
            this.txtStreamablePassword = new System.Windows.Forms.TextBox();
            this.txtStreamableUsername = new System.Windows.Forms.TextBox();
            this.lblStreamableUsername = new System.Windows.Forms.Label();
            this.lblStreamablePassword = new System.Windows.Forms.Label();
            this.cbStreamableAnonymous = new System.Windows.Forms.CheckBox();
            this.tpSul = new System.Windows.Forms.TabPage();
            this.btnSulGetAPIKey = new System.Windows.Forms.Button();
            this.txtSulAPIKey = new System.Windows.Forms.TextBox();
            this.lblSulAPIKey = new System.Windows.Forms.Label();
            this.tpLithiio = new System.Windows.Forms.TabPage();
            this.btnLithiioFetchAPIKey = new System.Windows.Forms.Button();
            this.txtLithiioPassword = new System.Windows.Forms.TextBox();
            this.txtLithiioEmail = new System.Windows.Forms.TextBox();
            this.lblLithiioPassword = new System.Windows.Forms.Label();
            this.lblLithiioEmail = new System.Windows.Forms.Label();
            this.btnLithiioGetAPIKey = new System.Windows.Forms.Button();
            this.lblLithiioApiKey = new System.Windows.Forms.Label();
            this.txtLithiioApiKey = new System.Windows.Forms.TextBox();
            this.tpPlik = new System.Windows.Forms.TabPage();
            this.gbPlikSettings = new System.Windows.Forms.GroupBox();
            this.cbPlikOneShot = new System.Windows.Forms.CheckBox();
            this.txtPlikComment = new System.Windows.Forms.TextBox();
            this.cbPlikComment = new System.Windows.Forms.CheckBox();
            this.cbPlikRemovable = new System.Windows.Forms.CheckBox();
            this.gbPlikLoginCredentials = new System.Windows.Forms.GroupBox();
            this.nudPlikTTL = new System.Windows.Forms.NumericUpDown();
            this.cbxPlikTTLUnit = new System.Windows.Forms.ComboBox();
            this.lblPlikTTL = new System.Windows.Forms.Label();
            this.txtPlikURL = new System.Windows.Forms.TextBox();
            this.lblPlikURL = new System.Windows.Forms.Label();
            this.cbPlikIsSecured = new System.Windows.Forms.CheckBox();
            this.lblPlikAPIKey = new System.Windows.Forms.Label();
            this.txtPlikAPIKey = new System.Windows.Forms.TextBox();
            this.lblPlikPassword = new System.Windows.Forms.Label();
            this.lblPlikUsername = new System.Windows.Forms.Label();
            this.txtPlikPassword = new System.Windows.Forms.TextBox();
            this.txtPlikLogin = new System.Windows.Forms.TextBox();
            this.tpSharedFolder = new System.Windows.Forms.TabPage();
            this.lblSharedFolderFiles = new System.Windows.Forms.Label();
            this.lblSharedFolderText = new System.Windows.Forms.Label();
            this.cboSharedFolderFiles = new System.Windows.Forms.ComboBox();
            this.lblSharedFolderImages = new System.Windows.Forms.Label();
            this.cboSharedFolderText = new System.Windows.Forms.ComboBox();
            this.cboSharedFolderImages = new System.Windows.Forms.ComboBox();
            this.ucLocalhostAccounts = new ShareX.UploadersLib.AccountsControl();
            this.tpEmail = new System.Windows.Forms.TabPage();
            this.txtEmailAutomaticSendTo = new System.Windows.Forms.TextBox();
            this.cbEmailAutomaticSend = new System.Windows.Forms.CheckBox();
            this.lblEmailSmtpServer = new System.Windows.Forms.Label();
            this.lblEmailPassword = new System.Windows.Forms.Label();
            this.cbEmailRememberLastTo = new System.Windows.Forms.CheckBox();
            this.txtEmailFrom = new System.Windows.Forms.TextBox();
            this.txtEmailPassword = new System.Windows.Forms.TextBox();
            this.txtEmailDefaultBody = new System.Windows.Forms.TextBox();
            this.lblEmailFrom = new System.Windows.Forms.Label();
            this.txtEmailSmtpServer = new System.Windows.Forms.TextBox();
            this.lblEmailDefaultSubject = new System.Windows.Forms.Label();
            this.lblEmailDefaultBody = new System.Windows.Forms.Label();
            this.nudEmailSmtpPort = new System.Windows.Forms.NumericUpDown();
            this.lblEmailSmtpPort = new System.Windows.Forms.Label();
            this.txtEmailDefaultSubject = new System.Windows.Forms.TextBox();
            this.btnCopyShowFiles = new System.Windows.Forms.Button();
            this.tpTextUploaders = new System.Windows.Forms.TabPage();
            this.tcTextUploaders = new System.Windows.Forms.TabControl();
            this.tpPastebin = new System.Windows.Forms.TabPage();
            this.cbPastebinRaw = new System.Windows.Forms.CheckBox();
            this.cbPastebinSyntax = new System.Windows.Forms.ComboBox();
            this.btnPastebinRegister = new System.Windows.Forms.Button();
            this.lblPastebinSyntax = new System.Windows.Forms.Label();
            this.lblPastebinExpiration = new System.Windows.Forms.Label();
            this.lblPastebinPrivacy = new System.Windows.Forms.Label();
            this.lblPastebinTitle = new System.Windows.Forms.Label();
            this.lblPastebinPassword = new System.Windows.Forms.Label();
            this.lblPastebinUsername = new System.Windows.Forms.Label();
            this.cbPastebinExpiration = new System.Windows.Forms.ComboBox();
            this.cbPastebinPrivacy = new System.Windows.Forms.ComboBox();
            this.txtPastebinTitle = new System.Windows.Forms.TextBox();
            this.txtPastebinPassword = new System.Windows.Forms.TextBox();
            this.txtPastebinUsername = new System.Windows.Forms.TextBox();
            this.lblPastebinLoginStatus = new System.Windows.Forms.Label();
            this.btnPastebinLogin = new System.Windows.Forms.Button();
            this.tpPaste_ee = new System.Windows.Forms.TabPage();
            this.btnPaste_eeGetUserKey = new System.Windows.Forms.Button();
            this.lblPaste_eeUserAPIKey = new System.Windows.Forms.Label();
            this.txtPaste_eeUserAPIKey = new System.Windows.Forms.TextBox();
            this.tpGist = new System.Windows.Forms.TabPage();
            this.lblGistCustomURLExample = new System.Windows.Forms.Label();
            this.lblGistOAuthInfo = new System.Windows.Forms.Label();
            this.lblGistCustomURL = new System.Windows.Forms.Label();
            this.txtGistCustomURL = new System.Windows.Forms.TextBox();
            this.cbGistUseRawURL = new System.Windows.Forms.CheckBox();
            this.cbGistPublishPublic = new System.Windows.Forms.CheckBox();
            this.oAuth2Gist = new ShareX.UploadersLib.OAuthControl();
            this.atcGistAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.tpUpaste = new System.Windows.Forms.TabPage();
            this.cbUpasteIsPublic = new System.Windows.Forms.CheckBox();
            this.lblUpasteUserKey = new System.Windows.Forms.Label();
            this.txtUpasteUserKey = new System.Windows.Forms.TextBox();
            this.tpHastebin = new System.Windows.Forms.TabPage();
            this.cbHastebinUseFileExtension = new System.Windows.Forms.CheckBox();
            this.txtHastebinSyntaxHighlighting = new System.Windows.Forms.TextBox();
            this.txtHastebinCustomDomain = new System.Windows.Forms.TextBox();
            this.lblHastebinSyntaxHighlighting = new System.Windows.Forms.Label();
            this.lblHastebinCustomDomain = new System.Windows.Forms.Label();
            this.tpOneTimeSecret = new System.Windows.Forms.TabPage();
            this.lblOneTimeSecretAPIKey = new System.Windows.Forms.Label();
            this.lblOneTimeSecretEmail = new System.Windows.Forms.Label();
            this.txtOneTimeSecretAPIKey = new System.Windows.Forms.TextBox();
            this.txtOneTimeSecretEmail = new System.Windows.Forms.TextBox();
            this.tpPastie = new System.Windows.Forms.TabPage();
            this.cbPastieIsPublic = new System.Windows.Forms.CheckBox();
            this.tpImageUploaders = new System.Windows.Forms.TabPage();
            this.tcImageUploaders = new System.Windows.Forms.TabControl();
            this.tpImgur = new System.Windows.Forms.TabPage();
            this.cbImgurUseGIFV = new System.Windows.Forms.CheckBox();
            this.cbImgurUploadSelectedAlbum = new System.Windows.Forms.CheckBox();
            this.cbImgurDirectLink = new System.Windows.Forms.CheckBox();
            this.atcImgurAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.oauth2Imgur = new ShareX.UploadersLib.OAuthControl();
            this.lvImgurAlbumList = new System.Windows.Forms.ListView();
            this.chImgurID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chImgurTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chImgurDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnImgurRefreshAlbumList = new System.Windows.Forms.Button();
            this.cbImgurThumbnailType = new System.Windows.Forms.ComboBox();
            this.lblImgurThumbnailType = new System.Windows.Forms.Label();
            this.tpImageShack = new System.Windows.Forms.TabPage();
            this.btnImageShackLogin = new System.Windows.Forms.Button();
            this.btnImageShackOpenPublicProfile = new System.Windows.Forms.Button();
            this.cbImageShackIsPublic = new System.Windows.Forms.CheckBox();
            this.btnImageShackOpenMyImages = new System.Windows.Forms.Button();
            this.lblImageShackUsername = new System.Windows.Forms.Label();
            this.txtImageShackUsername = new System.Windows.Forms.TextBox();
            this.txtImageShackPassword = new System.Windows.Forms.TextBox();
            this.lblImageShackPassword = new System.Windows.Forms.Label();
            this.tpTinyPic = new System.Windows.Forms.TabPage();
            this.atcTinyPicAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.btnTinyPicLogin = new System.Windows.Forms.Button();
            this.txtTinyPicPassword = new System.Windows.Forms.TextBox();
            this.lblTinyPicPassword = new System.Windows.Forms.Label();
            this.txtTinyPicUsername = new System.Windows.Forms.TextBox();
            this.lblTinyPicUsername = new System.Windows.Forms.Label();
            this.btnTinyPicOpenMyImages = new System.Windows.Forms.Button();
            this.tpFlickr = new System.Windows.Forms.TabPage();
            this.cbFlickrDirectLink = new System.Windows.Forms.CheckBox();
            this.oauthFlickr = new ShareX.UploadersLib.OAuthControl();
            this.tpPhotobucket = new System.Windows.Forms.TabPage();
            this.gbPhotobucketAlbumPath = new System.Windows.Forms.GroupBox();
            this.btnPhotobucketAddAlbum = new System.Windows.Forms.Button();
            this.btnPhotobucketRemoveAlbum = new System.Windows.Forms.Button();
            this.cboPhotobucketAlbumPaths = new System.Windows.Forms.ComboBox();
            this.gbPhotobucketAlbums = new System.Windows.Forms.GroupBox();
            this.lblPhotobucketNewAlbumName = new System.Windows.Forms.Label();
            this.lblPhotobucketParentAlbumPath = new System.Windows.Forms.Label();
            this.txtPhotobucketNewAlbumName = new System.Windows.Forms.TextBox();
            this.txtPhotobucketParentAlbumPath = new System.Windows.Forms.TextBox();
            this.btnPhotobucketCreateAlbum = new System.Windows.Forms.Button();
            this.gbPhotobucketUserAccount = new System.Windows.Forms.GroupBox();
            this.lblPhotobucketDefaultAlbumName = new System.Windows.Forms.Label();
            this.btnPhotobucketAuthOpen = new System.Windows.Forms.Button();
            this.txtPhotobucketDefaultAlbumName = new System.Windows.Forms.TextBox();
            this.lblPhotobucketVerificationCode = new System.Windows.Forms.Label();
            this.btnPhotobucketAuthComplete = new System.Windows.Forms.Button();
            this.txtPhotobucketVerificationCode = new System.Windows.Forms.TextBox();
            this.lblPhotobucketAccountStatus = new System.Windows.Forms.Label();
            this.tpGooglePhotos = new System.Windows.Forms.TabPage();
            this.txtPicasaAlbumID = new System.Windows.Forms.TextBox();
            this.lblPicasaAlbumID = new System.Windows.Forms.Label();
            this.lvPicasaAlbumList = new System.Windows.Forms.ListView();
            this.chPicasaID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPicasaName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPicasaDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnPicasaRefreshAlbumList = new System.Windows.Forms.Button();
            this.oauth2Picasa = new ShareX.UploadersLib.OAuthControl();
            this.tpChevereto = new System.Windows.Forms.TabPage();
            this.btnCheveretoTestAll = new System.Windows.Forms.Button();
            this.lblCheveretoUploadURLExample = new System.Windows.Forms.Label();
            this.lblCheveretoUploaders = new System.Windows.Forms.Label();
            this.cbCheveretoUploaders = new System.Windows.Forms.ComboBox();
            this.cbCheveretoDirectURL = new System.Windows.Forms.CheckBox();
            this.lblCheveretoUploadURL = new System.Windows.Forms.Label();
            this.txtCheveretoUploadURL = new System.Windows.Forms.TextBox();
            this.txtCheveretoAPIKey = new System.Windows.Forms.TextBox();
            this.lblCheveretoAPIKey = new System.Windows.Forms.Label();
            this.tpVgyme = new System.Windows.Forms.TabPage();
            this.llVgymeAccountDetailsPage = new System.Windows.Forms.LinkLabel();
            this.txtVgymeUserKey = new System.Windows.Forms.TextBox();
            this.lvlVgymeUserKey = new System.Windows.Forms.Label();
            this.tcUploaders = new System.Windows.Forms.TabControl();
            this.lblWidthHint = new System.Windows.Forms.Label();
            this.ttlvMain = new ShareX.HelpersLib.TabToListView();
            this.actRapidShareAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.cbAmazonS3PublicACL = new System.Windows.Forms.CheckBox();
            this.tpOtherUploaders.SuspendLayout();
            this.tcOtherUploaders.SuspendLayout();
            this.tpTwitter.SuspendLayout();
            this.tpCustomUploaders.SuspendLayout();
            this.pCustomUploader.SuspendLayout();
            this.tcCustomUploaderResponseParse.SuspendLayout();
            this.tpCustomUploaderJsonParse.SuspendLayout();
            this.tpCustomUploaderXmlParse.SuspendLayout();
            this.tpCustomUploaderRegexParse.SuspendLayout();
            this.tcCustomUploaderArguments.SuspendLayout();
            this.tpCustomUploaderArguments.SuspendLayout();
            this.tpCustomUploaderHeaders.SuspendLayout();
            this.gbCustomUploaders.SuspendLayout();
            this.tpURLShorteners.SuspendLayout();
            this.tcURLShorteners.SuspendLayout();
            this.tpBitly.SuspendLayout();
            this.tpGoogleURLShortener.SuspendLayout();
            this.tpYourls.SuspendLayout();
            this.tpAdFly.SuspendLayout();
            this.tpCoinURL.SuspendLayout();
            this.tpPolr.SuspendLayout();
            this.tpFileUploaders.SuspendLayout();
            this.tcFileUploaders.SuspendLayout();
            this.tpFTP.SuspendLayout();
            this.gbFTPAccount.SuspendLayout();
            this.gbSFTP.SuspendLayout();
            this.pFTPTransferMode.SuspendLayout();
            this.pFTPProtocol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFTPPort)).BeginInit();
            this.gbFTPS.SuspendLayout();
            this.tpDropbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropboxLogo)).BeginInit();
            this.tpOneDrive.SuspendLayout();
            this.tpGoogleDrive.SuspendLayout();
            this.tpPuush.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPuush)).BeginInit();
            this.tpBox.SuspendLayout();
            this.tpAmazonS3.SuspendLayout();
            this.tpAzureStorage.SuspendLayout();
            this.tpGfycat.SuspendLayout();
            this.tpMega.SuspendLayout();
            this.tpOwnCloud.SuspendLayout();
            this.tpMediaFire.SuspendLayout();
            this.tpPushbullet.SuspendLayout();
            this.tpSendSpace.SuspendLayout();
            this.tpGe_tt.SuspendLayout();
            this.tpHostr.SuspendLayout();
            this.tpJira.SuspendLayout();
            this.gbJiraServer.SuspendLayout();
            this.tpLambda.SuspendLayout();
            this.tpPomf.SuspendLayout();
            this.tpSeafile.SuspendLayout();
            this.grpSeafileShareSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeafileExpireDays)).BeginInit();
            this.grpSeafileAccInfo.SuspendLayout();
            this.grpSeafileObtainAuthToken.SuspendLayout();
            this.tpStreamable.SuspendLayout();
            this.tpSul.SuspendLayout();
            this.tpLithiio.SuspendLayout();
            this.tpPlik.SuspendLayout();
            this.gbPlikSettings.SuspendLayout();
            this.gbPlikLoginCredentials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlikTTL)).BeginInit();
            this.tpSharedFolder.SuspendLayout();
            this.tpEmail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmailSmtpPort)).BeginInit();
            this.tpTextUploaders.SuspendLayout();
            this.tcTextUploaders.SuspendLayout();
            this.tpPastebin.SuspendLayout();
            this.tpPaste_ee.SuspendLayout();
            this.tpGist.SuspendLayout();
            this.tpUpaste.SuspendLayout();
            this.tpHastebin.SuspendLayout();
            this.tpOneTimeSecret.SuspendLayout();
            this.tpPastie.SuspendLayout();
            this.tpImageUploaders.SuspendLayout();
            this.tcImageUploaders.SuspendLayout();
            this.tpImgur.SuspendLayout();
            this.tpImageShack.SuspendLayout();
            this.tpTinyPic.SuspendLayout();
            this.tpFlickr.SuspendLayout();
            this.tpPhotobucket.SuspendLayout();
            this.gbPhotobucketAlbumPath.SuspendLayout();
            this.gbPhotobucketAlbums.SuspendLayout();
            this.gbPhotobucketUserAccount.SuspendLayout();
            this.tpGooglePhotos.SuspendLayout();
            this.tpChevereto.SuspendLayout();
            this.tpVgyme.SuspendLayout();
            this.tcUploaders.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtRapidSharePremiumUserName
            // 
            resources.ApplyResources(this.txtRapidSharePremiumUserName, "txtRapidSharePremiumUserName");
            this.txtRapidSharePremiumUserName.Name = "txtRapidSharePremiumUserName";
            // 
            // ttHelpTip
            // 
            this.ttHelpTip.AutomaticDelay = 0;
            this.ttHelpTip.AutoPopDelay = 30000;
            this.ttHelpTip.BackColor = System.Drawing.SystemColors.Window;
            this.ttHelpTip.InitialDelay = 500;
            this.ttHelpTip.IsBalloon = true;
            this.ttHelpTip.ReshowDelay = 100;
            this.ttHelpTip.UseAnimation = false;
            this.ttHelpTip.UseFading = false;
            // 
            // cbAmazonS3CustomCNAME
            // 
            resources.ApplyResources(this.cbAmazonS3CustomCNAME, "cbAmazonS3CustomCNAME");
            this.cbAmazonS3CustomCNAME.Name = "cbAmazonS3CustomCNAME";
            this.ttHelpTip.SetToolTip(this.cbAmazonS3CustomCNAME, resources.GetString("cbAmazonS3CustomCNAME.ToolTip"));
            this.cbAmazonS3CustomCNAME.UseVisualStyleBackColor = true;
            this.cbAmazonS3CustomCNAME.CheckedChanged += new System.EventHandler(this.cbAmazonS3CustomCNAME_CheckedChanged);
            // 
            // mbCustomUploaderDestinationType
            // 
            resources.ApplyResources(this.mbCustomUploaderDestinationType, "mbCustomUploaderDestinationType");
            this.mbCustomUploaderDestinationType.Menu = this.cmsCustomUploaderDestinationType;
            this.mbCustomUploaderDestinationType.Name = "mbCustomUploaderDestinationType";
            this.ttHelpTip.SetToolTip(this.mbCustomUploaderDestinationType, resources.GetString("mbCustomUploaderDestinationType.ToolTip"));
            this.mbCustomUploaderDestinationType.UseVisualStyleBackColor = true;
            // 
            // cmsCustomUploaderDestinationType
            // 
            this.cmsCustomUploaderDestinationType.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsCustomUploaderDestinationType.Name = "cmsCustomUploaderDestinationType";
            resources.ApplyResources(this.cmsCustomUploaderDestinationType, "cmsCustomUploaderDestinationType");
            // 
            // tpOtherUploaders
            // 
            this.tpOtherUploaders.Controls.Add(this.tcOtherUploaders);
            resources.ApplyResources(this.tpOtherUploaders, "tpOtherUploaders");
            this.tpOtherUploaders.Name = "tpOtherUploaders";
            this.tpOtherUploaders.UseVisualStyleBackColor = true;
            // 
            // tcOtherUploaders
            // 
            this.tcOtherUploaders.Controls.Add(this.tpTwitter);
            this.tcOtherUploaders.Controls.Add(this.tpCustomUploaders);
            resources.ApplyResources(this.tcOtherUploaders, "tcOtherUploaders");
            this.tcOtherUploaders.Name = "tcOtherUploaders";
            this.tcOtherUploaders.SelectedIndex = 0;
            // 
            // tpTwitter
            // 
            this.tpTwitter.Controls.Add(this.btnTwitterNameUpdate);
            this.tpTwitter.Controls.Add(this.lbTwitterAccounts);
            this.tpTwitter.Controls.Add(this.lblTwitterDefaultMessage);
            this.tpTwitter.Controls.Add(this.txtTwitterDefaultMessage);
            this.tpTwitter.Controls.Add(this.cbTwitterSkipMessageBox);
            this.tpTwitter.Controls.Add(this.oauthTwitter);
            this.tpTwitter.Controls.Add(this.txtTwitterDescription);
            this.tpTwitter.Controls.Add(this.lblTwitterDescription);
            this.tpTwitter.Controls.Add(this.btnTwitterRemove);
            this.tpTwitter.Controls.Add(this.btnTwitterAdd);
            resources.ApplyResources(this.tpTwitter, "tpTwitter");
            this.tpTwitter.Name = "tpTwitter";
            this.tpTwitter.UseVisualStyleBackColor = true;
            // 
            // btnTwitterNameUpdate
            // 
            resources.ApplyResources(this.btnTwitterNameUpdate, "btnTwitterNameUpdate");
            this.btnTwitterNameUpdate.Name = "btnTwitterNameUpdate";
            this.btnTwitterNameUpdate.UseVisualStyleBackColor = true;
            this.btnTwitterNameUpdate.Click += new System.EventHandler(this.btnTwitterNameUpdate_Click);
            // 
            // lbTwitterAccounts
            // 
            this.lbTwitterAccounts.FormattingEnabled = true;
            resources.ApplyResources(this.lbTwitterAccounts, "lbTwitterAccounts");
            this.lbTwitterAccounts.Name = "lbTwitterAccounts";
            this.lbTwitterAccounts.SelectedIndexChanged += new System.EventHandler(this.lbTwitterAccounts_SelectedIndexChanged);
            // 
            // lblTwitterDefaultMessage
            // 
            resources.ApplyResources(this.lblTwitterDefaultMessage, "lblTwitterDefaultMessage");
            this.lblTwitterDefaultMessage.Name = "lblTwitterDefaultMessage";
            // 
            // txtTwitterDefaultMessage
            // 
            resources.ApplyResources(this.txtTwitterDefaultMessage, "txtTwitterDefaultMessage");
            this.txtTwitterDefaultMessage.Name = "txtTwitterDefaultMessage";
            this.txtTwitterDefaultMessage.TextChanged += new System.EventHandler(this.txtTwitterDefaultMessage_TextChanged);
            // 
            // cbTwitterSkipMessageBox
            // 
            resources.ApplyResources(this.cbTwitterSkipMessageBox, "cbTwitterSkipMessageBox");
            this.cbTwitterSkipMessageBox.Name = "cbTwitterSkipMessageBox";
            this.cbTwitterSkipMessageBox.UseVisualStyleBackColor = true;
            this.cbTwitterSkipMessageBox.CheckedChanged += new System.EventHandler(this.cbTwitterSkipMessageBox_CheckedChanged);
            // 
            // oauthTwitter
            // 
            resources.ApplyResources(this.oauthTwitter, "oauthTwitter");
            this.oauthTwitter.IsRefreshable = false;
            this.oauthTwitter.Name = "oauthTwitter";
            this.oauthTwitter.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauthTwitter_OpenButtonClicked);
            this.oauthTwitter.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauthTwitter_CompleteButtonClicked);
            this.oauthTwitter.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauthTwitter_ClearButtonClicked);
            // 
            // txtTwitterDescription
            // 
            resources.ApplyResources(this.txtTwitterDescription, "txtTwitterDescription");
            this.txtTwitterDescription.Name = "txtTwitterDescription";
            // 
            // lblTwitterDescription
            // 
            resources.ApplyResources(this.lblTwitterDescription, "lblTwitterDescription");
            this.lblTwitterDescription.Name = "lblTwitterDescription";
            // 
            // btnTwitterRemove
            // 
            resources.ApplyResources(this.btnTwitterRemove, "btnTwitterRemove");
            this.btnTwitterRemove.Name = "btnTwitterRemove";
            this.btnTwitterRemove.UseVisualStyleBackColor = true;
            this.btnTwitterRemove.Click += new System.EventHandler(this.btnTwitterRemove_Click);
            // 
            // btnTwitterAdd
            // 
            resources.ApplyResources(this.btnTwitterAdd, "btnTwitterAdd");
            this.btnTwitterAdd.Name = "btnTwitterAdd";
            this.btnTwitterAdd.UseVisualStyleBackColor = true;
            this.btnTwitterAdd.Click += new System.EventHandler(this.btnTwitterAdd_Click);
            // 
            // tpCustomUploaders
            // 
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderURLSharingServiceTest);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderURLSharingService);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderURLSharingService);
            this.tpCustomUploaders.Controls.Add(this.pCustomUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderExamples);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderHelp);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderImageUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderFileUploaderTest);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderFileUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderImageUploaderTest);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderTestResult);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderFileUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderShowLastResponse);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderURLShortener);
            this.tpCustomUploaders.Controls.Add(this.gbCustomUploaders);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderTextUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderURLShortenerTest);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderTextUploader);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderURLShortener);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderTextUploaderTest);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderImageUploader);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderLog);
            resources.ApplyResources(this.tpCustomUploaders, "tpCustomUploaders");
            this.tpCustomUploaders.Name = "tpCustomUploaders";
            this.tpCustomUploaders.UseVisualStyleBackColor = true;
            // 
            // btnCustomUploaderURLSharingServiceTest
            // 
            resources.ApplyResources(this.btnCustomUploaderURLSharingServiceTest, "btnCustomUploaderURLSharingServiceTest");
            this.btnCustomUploaderURLSharingServiceTest.Name = "btnCustomUploaderURLSharingServiceTest";
            this.btnCustomUploaderURLSharingServiceTest.UseVisualStyleBackColor = true;
            this.btnCustomUploaderURLSharingServiceTest.Click += new System.EventHandler(this.btnCustomUploaderURLSharingServiceTest_Click);
            // 
            // cbCustomUploaderURLSharingService
            // 
            this.cbCustomUploaderURLSharingService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderURLSharingService.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderURLSharingService, "cbCustomUploaderURLSharingService");
            this.cbCustomUploaderURLSharingService.Name = "cbCustomUploaderURLSharingService";
            this.cbCustomUploaderURLSharingService.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderURLSharingService_SelectedIndexChanged);
            // 
            // lblCustomUploaderURLSharingService
            // 
            resources.ApplyResources(this.lblCustomUploaderURLSharingService, "lblCustomUploaderURLSharingService");
            this.lblCustomUploaderURLSharingService.Name = "lblCustomUploaderURLSharingService";
            // 
            // pCustomUploader
            // 
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderName);
            this.pCustomUploader.Controls.Add(this.mbCustomUploaderDestinationType);
            this.pCustomUploader.Controls.Add(this.cbCustomUploaderRequestType);
            this.pCustomUploader.Controls.Add(this.tcCustomUploaderResponseParse);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderURL);
            this.pCustomUploader.Controls.Add(this.tcCustomUploaderArguments);
            this.pCustomUploader.Controls.Add(this.txtCustomUploaderFileForm);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderRequestType);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderFileForm);
            this.pCustomUploader.Controls.Add(this.txtCustomUploaderName);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderThumbnailURL);
            this.pCustomUploader.Controls.Add(this.txtCustomUploaderRequestURL);
            this.pCustomUploader.Controls.Add(this.txtCustomUploaderURL);
            this.pCustomUploader.Controls.Add(this.cbCustomUploaderResponseType);
            this.pCustomUploader.Controls.Add(this.txtCustomUploaderThumbnailURL);
            this.pCustomUploader.Controls.Add(this.txtCustomUploaderDeletionURL);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderRequestURL);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderResponseType);
            this.pCustomUploader.Controls.Add(this.lblCustomUploaderDeletionURL);
            resources.ApplyResources(this.pCustomUploader, "pCustomUploader");
            this.pCustomUploader.Name = "pCustomUploader";
            // 
            // lblCustomUploaderName
            // 
            resources.ApplyResources(this.lblCustomUploaderName, "lblCustomUploaderName");
            this.lblCustomUploaderName.Name = "lblCustomUploaderName";
            // 
            // cbCustomUploaderRequestType
            // 
            this.cbCustomUploaderRequestType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderRequestType.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderRequestType, "cbCustomUploaderRequestType");
            this.cbCustomUploaderRequestType.Name = "cbCustomUploaderRequestType";
            this.cbCustomUploaderRequestType.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderRequestType_SelectedIndexChanged);
            // 
            // tcCustomUploaderResponseParse
            // 
            this.tcCustomUploaderResponseParse.Controls.Add(this.tpCustomUploaderJsonParse);
            this.tcCustomUploaderResponseParse.Controls.Add(this.tpCustomUploaderXmlParse);
            this.tcCustomUploaderResponseParse.Controls.Add(this.tpCustomUploaderRegexParse);
            resources.ApplyResources(this.tcCustomUploaderResponseParse, "tcCustomUploaderResponseParse");
            this.tcCustomUploaderResponseParse.Name = "tcCustomUploaderResponseParse";
            this.tcCustomUploaderResponseParse.SelectedIndex = 0;
            // 
            // tpCustomUploaderJsonParse
            // 
            this.tpCustomUploaderJsonParse.Controls.Add(this.btnCustomUploaderJsonAddSyntax);
            this.tpCustomUploaderJsonParse.Controls.Add(this.btnCustomUploadJsonPathHelp);
            this.tpCustomUploaderJsonParse.Controls.Add(this.lblCustomUploaderJsonPathExample);
            this.tpCustomUploaderJsonParse.Controls.Add(this.lblCustomUploaderJsonPath);
            this.tpCustomUploaderJsonParse.Controls.Add(this.txtCustomUploaderJsonPath);
            resources.ApplyResources(this.tpCustomUploaderJsonParse, "tpCustomUploaderJsonParse");
            this.tpCustomUploaderJsonParse.Name = "tpCustomUploaderJsonParse";
            this.tpCustomUploaderJsonParse.UseVisualStyleBackColor = true;
            // 
            // btnCustomUploaderJsonAddSyntax
            // 
            resources.ApplyResources(this.btnCustomUploaderJsonAddSyntax, "btnCustomUploaderJsonAddSyntax");
            this.btnCustomUploaderJsonAddSyntax.Name = "btnCustomUploaderJsonAddSyntax";
            this.btnCustomUploaderJsonAddSyntax.UseVisualStyleBackColor = true;
            this.btnCustomUploaderJsonAddSyntax.Click += new System.EventHandler(this.btnCustomUploaderJsonAddSyntax_Click);
            // 
            // btnCustomUploadJsonPathHelp
            // 
            resources.ApplyResources(this.btnCustomUploadJsonPathHelp, "btnCustomUploadJsonPathHelp");
            this.btnCustomUploadJsonPathHelp.Name = "btnCustomUploadJsonPathHelp";
            this.btnCustomUploadJsonPathHelp.UseVisualStyleBackColor = true;
            this.btnCustomUploadJsonPathHelp.Click += new System.EventHandler(this.btnCustomUploadJsonPathHelp_Click);
            // 
            // lblCustomUploaderJsonPathExample
            // 
            resources.ApplyResources(this.lblCustomUploaderJsonPathExample, "lblCustomUploaderJsonPathExample");
            this.lblCustomUploaderJsonPathExample.Name = "lblCustomUploaderJsonPathExample";
            // 
            // lblCustomUploaderJsonPath
            // 
            resources.ApplyResources(this.lblCustomUploaderJsonPath, "lblCustomUploaderJsonPath");
            this.lblCustomUploaderJsonPath.Name = "lblCustomUploaderJsonPath";
            // 
            // txtCustomUploaderJsonPath
            // 
            resources.ApplyResources(this.txtCustomUploaderJsonPath, "txtCustomUploaderJsonPath");
            this.txtCustomUploaderJsonPath.Name = "txtCustomUploaderJsonPath";
            this.txtCustomUploaderJsonPath.TextChanged += new System.EventHandler(this.txtCustomUploaderJsonPath_TextChanged);
            // 
            // tpCustomUploaderXmlParse
            // 
            this.tpCustomUploaderXmlParse.Controls.Add(this.btnCustomUploaderXmlSyntaxAdd);
            this.tpCustomUploaderXmlParse.Controls.Add(this.btnCustomUploaderXPathHelp);
            this.tpCustomUploaderXmlParse.Controls.Add(this.lblCustomUploaderXPathExample);
            this.tpCustomUploaderXmlParse.Controls.Add(this.lblCustomUploaderXPath);
            this.tpCustomUploaderXmlParse.Controls.Add(this.txtCustomUploaderXPath);
            resources.ApplyResources(this.tpCustomUploaderXmlParse, "tpCustomUploaderXmlParse");
            this.tpCustomUploaderXmlParse.Name = "tpCustomUploaderXmlParse";
            this.tpCustomUploaderXmlParse.UseVisualStyleBackColor = true;
            // 
            // btnCustomUploaderXmlSyntaxAdd
            // 
            resources.ApplyResources(this.btnCustomUploaderXmlSyntaxAdd, "btnCustomUploaderXmlSyntaxAdd");
            this.btnCustomUploaderXmlSyntaxAdd.Name = "btnCustomUploaderXmlSyntaxAdd";
            this.btnCustomUploaderXmlSyntaxAdd.UseVisualStyleBackColor = true;
            this.btnCustomUploaderXmlSyntaxAdd.Click += new System.EventHandler(this.btnCustomUploaderXmlSyntaxAdd_Click);
            // 
            // btnCustomUploaderXPathHelp
            // 
            resources.ApplyResources(this.btnCustomUploaderXPathHelp, "btnCustomUploaderXPathHelp");
            this.btnCustomUploaderXPathHelp.Name = "btnCustomUploaderXPathHelp";
            this.btnCustomUploaderXPathHelp.UseVisualStyleBackColor = true;
            this.btnCustomUploaderXPathHelp.Click += new System.EventHandler(this.btnCustomUploaderXPathHelp_Click);
            // 
            // lblCustomUploaderXPathExample
            // 
            resources.ApplyResources(this.lblCustomUploaderXPathExample, "lblCustomUploaderXPathExample");
            this.lblCustomUploaderXPathExample.Name = "lblCustomUploaderXPathExample";
            // 
            // lblCustomUploaderXPath
            // 
            resources.ApplyResources(this.lblCustomUploaderXPath, "lblCustomUploaderXPath");
            this.lblCustomUploaderXPath.Name = "lblCustomUploaderXPath";
            // 
            // txtCustomUploaderXPath
            // 
            resources.ApplyResources(this.txtCustomUploaderXPath, "txtCustomUploaderXPath");
            this.txtCustomUploaderXPath.Name = "txtCustomUploaderXPath";
            this.txtCustomUploaderXPath.TextChanged += new System.EventHandler(this.txtCustomUploaderXPath_TextChanged);
            // 
            // tpCustomUploaderRegexParse
            // 
            this.tpCustomUploaderRegexParse.Controls.Add(this.btnCustomUploaderRegexHelp);
            this.tpCustomUploaderRegexParse.Controls.Add(this.btnCustomUploaderRegexAddSyntax);
            this.tpCustomUploaderRegexParse.Controls.Add(this.txtCustomUploaderRegexp);
            this.tpCustomUploaderRegexParse.Controls.Add(this.btnCustomUploaderRegexpUpdate);
            this.tpCustomUploaderRegexParse.Controls.Add(this.btnCustomUploaderRegexpAdd);
            this.tpCustomUploaderRegexParse.Controls.Add(this.btnCustomUploaderRegexpRemove);
            this.tpCustomUploaderRegexParse.Controls.Add(this.lvCustomUploaderRegexps);
            resources.ApplyResources(this.tpCustomUploaderRegexParse, "tpCustomUploaderRegexParse");
            this.tpCustomUploaderRegexParse.Name = "tpCustomUploaderRegexParse";
            this.tpCustomUploaderRegexParse.UseVisualStyleBackColor = true;
            // 
            // btnCustomUploaderRegexHelp
            // 
            resources.ApplyResources(this.btnCustomUploaderRegexHelp, "btnCustomUploaderRegexHelp");
            this.btnCustomUploaderRegexHelp.Name = "btnCustomUploaderRegexHelp";
            this.btnCustomUploaderRegexHelp.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRegexHelp.Click += new System.EventHandler(this.btnCustomUploaderRegexHelp_Click);
            // 
            // btnCustomUploaderRegexAddSyntax
            // 
            resources.ApplyResources(this.btnCustomUploaderRegexAddSyntax, "btnCustomUploaderRegexAddSyntax");
            this.btnCustomUploaderRegexAddSyntax.Name = "btnCustomUploaderRegexAddSyntax";
            this.btnCustomUploaderRegexAddSyntax.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRegexAddSyntax.Click += new System.EventHandler(this.btnCustomUploaderRegexAddSyntax_Click);
            // 
            // txtCustomUploaderRegexp
            // 
            resources.ApplyResources(this.txtCustomUploaderRegexp, "txtCustomUploaderRegexp");
            this.txtCustomUploaderRegexp.Name = "txtCustomUploaderRegexp";
            this.txtCustomUploaderRegexp.TextChanged += new System.EventHandler(this.txtCustomUploaderRegexp_TextChanged);
            // 
            // btnCustomUploaderRegexpUpdate
            // 
            resources.ApplyResources(this.btnCustomUploaderRegexpUpdate, "btnCustomUploaderRegexpUpdate");
            this.btnCustomUploaderRegexpUpdate.Name = "btnCustomUploaderRegexpUpdate";
            this.btnCustomUploaderRegexpUpdate.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRegexpUpdate.Click += new System.EventHandler(this.btnCustomUploaderRegexpEdit_Click);
            // 
            // btnCustomUploaderRegexpAdd
            // 
            resources.ApplyResources(this.btnCustomUploaderRegexpAdd, "btnCustomUploaderRegexpAdd");
            this.btnCustomUploaderRegexpAdd.Name = "btnCustomUploaderRegexpAdd";
            this.btnCustomUploaderRegexpAdd.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRegexpAdd.Click += new System.EventHandler(this.btnCustomUploaderRegexpAdd_Click);
            // 
            // btnCustomUploaderRegexpRemove
            // 
            resources.ApplyResources(this.btnCustomUploaderRegexpRemove, "btnCustomUploaderRegexpRemove");
            this.btnCustomUploaderRegexpRemove.Name = "btnCustomUploaderRegexpRemove";
            this.btnCustomUploaderRegexpRemove.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRegexpRemove.Click += new System.EventHandler(this.btnCustomUploaderRegexpRemove_Click);
            // 
            // lvCustomUploaderRegexps
            // 
            this.lvCustomUploaderRegexps.AllowDrop = true;
            this.lvCustomUploaderRegexps.AllowItemDrag = true;
            this.lvCustomUploaderRegexps.AutoFillColumn = true;
            this.lvCustomUploaderRegexps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvRegexpsColumn});
            this.lvCustomUploaderRegexps.FullRowSelect = true;
            this.lvCustomUploaderRegexps.GridLines = true;
            this.lvCustomUploaderRegexps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvCustomUploaderRegexps.HideSelection = false;
            resources.ApplyResources(this.lvCustomUploaderRegexps, "lvCustomUploaderRegexps");
            this.lvCustomUploaderRegexps.MultiSelect = false;
            this.lvCustomUploaderRegexps.Name = "lvCustomUploaderRegexps";
            this.lvCustomUploaderRegexps.UseCompatibleStateImageBehavior = false;
            this.lvCustomUploaderRegexps.View = System.Windows.Forms.View.Details;
            this.lvCustomUploaderRegexps.SelectedIndexChanged += new System.EventHandler(this.lvCustomUploaderRegexps_SelectedIndexChanged);
            // 
            // lvRegexpsColumn
            // 
            resources.ApplyResources(this.lvRegexpsColumn, "lvRegexpsColumn");
            // 
            // lblCustomUploaderURL
            // 
            resources.ApplyResources(this.lblCustomUploaderURL, "lblCustomUploaderURL");
            this.lblCustomUploaderURL.Name = "lblCustomUploaderURL";
            // 
            // tcCustomUploaderArguments
            // 
            this.tcCustomUploaderArguments.Controls.Add(this.tpCustomUploaderArguments);
            this.tcCustomUploaderArguments.Controls.Add(this.tpCustomUploaderHeaders);
            resources.ApplyResources(this.tcCustomUploaderArguments, "tcCustomUploaderArguments");
            this.tcCustomUploaderArguments.Name = "tcCustomUploaderArguments";
            this.tcCustomUploaderArguments.SelectedIndex = 0;
            // 
            // tpCustomUploaderArguments
            // 
            this.tpCustomUploaderArguments.Controls.Add(this.btnCustomUploaderArgUpdate);
            this.tpCustomUploaderArguments.Controls.Add(this.txtCustomUploaderArgName);
            this.tpCustomUploaderArguments.Controls.Add(this.txtCustomUploaderArgValue);
            this.tpCustomUploaderArguments.Controls.Add(this.btnCustomUploaderArgAdd);
            this.tpCustomUploaderArguments.Controls.Add(this.btnCustomUploaderArgRemove);
            this.tpCustomUploaderArguments.Controls.Add(this.lvCustomUploaderArguments);
            resources.ApplyResources(this.tpCustomUploaderArguments, "tpCustomUploaderArguments");
            this.tpCustomUploaderArguments.Name = "tpCustomUploaderArguments";
            this.tpCustomUploaderArguments.UseVisualStyleBackColor = true;
            // 
            // btnCustomUploaderArgUpdate
            // 
            resources.ApplyResources(this.btnCustomUploaderArgUpdate, "btnCustomUploaderArgUpdate");
            this.btnCustomUploaderArgUpdate.Name = "btnCustomUploaderArgUpdate";
            this.btnCustomUploaderArgUpdate.UseVisualStyleBackColor = true;
            this.btnCustomUploaderArgUpdate.Click += new System.EventHandler(this.btnCustomUploaderArgUpdate_Click);
            // 
            // txtCustomUploaderArgName
            // 
            resources.ApplyResources(this.txtCustomUploaderArgName, "txtCustomUploaderArgName");
            this.txtCustomUploaderArgName.Name = "txtCustomUploaderArgName";
            this.txtCustomUploaderArgName.TextChanged += new System.EventHandler(this.txtCustomUploaderArgName_TextChanged);
            // 
            // txtCustomUploaderArgValue
            // 
            resources.ApplyResources(this.txtCustomUploaderArgValue, "txtCustomUploaderArgValue");
            this.txtCustomUploaderArgValue.Name = "txtCustomUploaderArgValue";
            // 
            // btnCustomUploaderArgAdd
            // 
            resources.ApplyResources(this.btnCustomUploaderArgAdd, "btnCustomUploaderArgAdd");
            this.btnCustomUploaderArgAdd.Name = "btnCustomUploaderArgAdd";
            this.btnCustomUploaderArgAdd.UseVisualStyleBackColor = true;
            this.btnCustomUploaderArgAdd.Click += new System.EventHandler(this.btnCustomUploaderArgAdd_Click);
            // 
            // btnCustomUploaderArgRemove
            // 
            resources.ApplyResources(this.btnCustomUploaderArgRemove, "btnCustomUploaderArgRemove");
            this.btnCustomUploaderArgRemove.Name = "btnCustomUploaderArgRemove";
            this.btnCustomUploaderArgRemove.UseVisualStyleBackColor = true;
            this.btnCustomUploaderArgRemove.Click += new System.EventHandler(this.btnCustomUploaderArgRemove_Click);
            // 
            // lvCustomUploaderArguments
            // 
            this.lvCustomUploaderArguments.AllowDrop = true;
            this.lvCustomUploaderArguments.AllowItemDrag = true;
            this.lvCustomUploaderArguments.AutoFillColumn = true;
            this.lvCustomUploaderArguments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chCustomUploaderArgumentsName,
            this.chCustomUploaderArgumentsValue});
            this.lvCustomUploaderArguments.FullRowSelect = true;
            this.lvCustomUploaderArguments.GridLines = true;
            this.lvCustomUploaderArguments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvCustomUploaderArguments.HideSelection = false;
            resources.ApplyResources(this.lvCustomUploaderArguments, "lvCustomUploaderArguments");
            this.lvCustomUploaderArguments.MultiSelect = false;
            this.lvCustomUploaderArguments.Name = "lvCustomUploaderArguments";
            this.lvCustomUploaderArguments.UseCompatibleStateImageBehavior = false;
            this.lvCustomUploaderArguments.View = System.Windows.Forms.View.Details;
            this.lvCustomUploaderArguments.SelectedIndexChanged += new System.EventHandler(this.lvCustomUploaderArguments_SelectedIndexChanged);
            // 
            // chCustomUploaderArgumentsName
            // 
            resources.ApplyResources(this.chCustomUploaderArgumentsName, "chCustomUploaderArgumentsName");
            // 
            // chCustomUploaderArgumentsValue
            // 
            resources.ApplyResources(this.chCustomUploaderArgumentsValue, "chCustomUploaderArgumentsValue");
            // 
            // tpCustomUploaderHeaders
            // 
            this.tpCustomUploaderHeaders.Controls.Add(this.btnCustomUploaderHeaderUpdate);
            this.tpCustomUploaderHeaders.Controls.Add(this.txtCustomUploaderHeaderName);
            this.tpCustomUploaderHeaders.Controls.Add(this.txtCustomUploaderHeaderValue);
            this.tpCustomUploaderHeaders.Controls.Add(this.btnCustomUploaderHeaderAdd);
            this.tpCustomUploaderHeaders.Controls.Add(this.btnCustomUploaderHeaderRemove);
            this.tpCustomUploaderHeaders.Controls.Add(this.lvCustomUploaderHeaders);
            resources.ApplyResources(this.tpCustomUploaderHeaders, "tpCustomUploaderHeaders");
            this.tpCustomUploaderHeaders.Name = "tpCustomUploaderHeaders";
            this.tpCustomUploaderHeaders.UseVisualStyleBackColor = true;
            // 
            // btnCustomUploaderHeaderUpdate
            // 
            resources.ApplyResources(this.btnCustomUploaderHeaderUpdate, "btnCustomUploaderHeaderUpdate");
            this.btnCustomUploaderHeaderUpdate.Name = "btnCustomUploaderHeaderUpdate";
            this.btnCustomUploaderHeaderUpdate.UseVisualStyleBackColor = true;
            this.btnCustomUploaderHeaderUpdate.Click += new System.EventHandler(this.btnCustomUploaderHeaderUpdate_Click);
            // 
            // txtCustomUploaderHeaderName
            // 
            resources.ApplyResources(this.txtCustomUploaderHeaderName, "txtCustomUploaderHeaderName");
            this.txtCustomUploaderHeaderName.Name = "txtCustomUploaderHeaderName";
            this.txtCustomUploaderHeaderName.TextChanged += new System.EventHandler(this.txtCustomUploaderHeaderName_TextChanged);
            // 
            // txtCustomUploaderHeaderValue
            // 
            resources.ApplyResources(this.txtCustomUploaderHeaderValue, "txtCustomUploaderHeaderValue");
            this.txtCustomUploaderHeaderValue.Name = "txtCustomUploaderHeaderValue";
            // 
            // btnCustomUploaderHeaderAdd
            // 
            resources.ApplyResources(this.btnCustomUploaderHeaderAdd, "btnCustomUploaderHeaderAdd");
            this.btnCustomUploaderHeaderAdd.Name = "btnCustomUploaderHeaderAdd";
            this.btnCustomUploaderHeaderAdd.UseVisualStyleBackColor = true;
            this.btnCustomUploaderHeaderAdd.Click += new System.EventHandler(this.btnCustomUploaderHeaderAdd_Click);
            // 
            // btnCustomUploaderHeaderRemove
            // 
            resources.ApplyResources(this.btnCustomUploaderHeaderRemove, "btnCustomUploaderHeaderRemove");
            this.btnCustomUploaderHeaderRemove.Name = "btnCustomUploaderHeaderRemove";
            this.btnCustomUploaderHeaderRemove.UseVisualStyleBackColor = true;
            this.btnCustomUploaderHeaderRemove.Click += new System.EventHandler(this.btnCustomUploaderHeaderRemove_Click);
            // 
            // lvCustomUploaderHeaders
            // 
            this.lvCustomUploaderHeaders.AllowDrop = true;
            this.lvCustomUploaderHeaders.AllowItemDrag = true;
            this.lvCustomUploaderHeaders.AutoFillColumn = true;
            this.lvCustomUploaderHeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chCustomUploaderHeadersName,
            this.chCustomUploaderHeadersValue});
            this.lvCustomUploaderHeaders.FullRowSelect = true;
            this.lvCustomUploaderHeaders.GridLines = true;
            this.lvCustomUploaderHeaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvCustomUploaderHeaders.HideSelection = false;
            resources.ApplyResources(this.lvCustomUploaderHeaders, "lvCustomUploaderHeaders");
            this.lvCustomUploaderHeaders.MultiSelect = false;
            this.lvCustomUploaderHeaders.Name = "lvCustomUploaderHeaders";
            this.lvCustomUploaderHeaders.UseCompatibleStateImageBehavior = false;
            this.lvCustomUploaderHeaders.View = System.Windows.Forms.View.Details;
            this.lvCustomUploaderHeaders.SelectedIndexChanged += new System.EventHandler(this.lvCustomUploaderHeaders_SelectedIndexChanged);
            // 
            // chCustomUploaderHeadersName
            // 
            resources.ApplyResources(this.chCustomUploaderHeadersName, "chCustomUploaderHeadersName");
            // 
            // chCustomUploaderHeadersValue
            // 
            resources.ApplyResources(this.chCustomUploaderHeadersValue, "chCustomUploaderHeadersValue");
            // 
            // txtCustomUploaderFileForm
            // 
            resources.ApplyResources(this.txtCustomUploaderFileForm, "txtCustomUploaderFileForm");
            this.txtCustomUploaderFileForm.Name = "txtCustomUploaderFileForm";
            this.txtCustomUploaderFileForm.TextChanged += new System.EventHandler(this.txtCustomUploaderFileForm_TextChanged);
            // 
            // lblCustomUploaderRequestType
            // 
            resources.ApplyResources(this.lblCustomUploaderRequestType, "lblCustomUploaderRequestType");
            this.lblCustomUploaderRequestType.Name = "lblCustomUploaderRequestType";
            // 
            // lblCustomUploaderFileForm
            // 
            resources.ApplyResources(this.lblCustomUploaderFileForm, "lblCustomUploaderFileForm");
            this.lblCustomUploaderFileForm.Name = "lblCustomUploaderFileForm";
            // 
            // txtCustomUploaderName
            // 
            resources.ApplyResources(this.txtCustomUploaderName, "txtCustomUploaderName");
            this.txtCustomUploaderName.Name = "txtCustomUploaderName";
            this.txtCustomUploaderName.TextChanged += new System.EventHandler(this.txtCustomUploaderName_TextChanged);
            // 
            // lblCustomUploaderThumbnailURL
            // 
            resources.ApplyResources(this.lblCustomUploaderThumbnailURL, "lblCustomUploaderThumbnailURL");
            this.lblCustomUploaderThumbnailURL.Name = "lblCustomUploaderThumbnailURL";
            // 
            // txtCustomUploaderRequestURL
            // 
            resources.ApplyResources(this.txtCustomUploaderRequestURL, "txtCustomUploaderRequestURL");
            this.txtCustomUploaderRequestURL.Name = "txtCustomUploaderRequestURL";
            this.txtCustomUploaderRequestURL.TextChanged += new System.EventHandler(this.txtCustomUploaderRequestURL_TextChanged);
            // 
            // txtCustomUploaderURL
            // 
            resources.ApplyResources(this.txtCustomUploaderURL, "txtCustomUploaderURL");
            this.txtCustomUploaderURL.Name = "txtCustomUploaderURL";
            this.txtCustomUploaderURL.TextChanged += new System.EventHandler(this.txtCustomUploaderURL_TextChanged);
            this.txtCustomUploaderURL.Enter += new System.EventHandler(this.txtCustomUploaderURL_Enter);
            // 
            // cbCustomUploaderResponseType
            // 
            this.cbCustomUploaderResponseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderResponseType.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderResponseType, "cbCustomUploaderResponseType");
            this.cbCustomUploaderResponseType.Name = "cbCustomUploaderResponseType";
            this.cbCustomUploaderResponseType.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderResponseType_SelectedIndexChanged);
            // 
            // txtCustomUploaderThumbnailURL
            // 
            resources.ApplyResources(this.txtCustomUploaderThumbnailURL, "txtCustomUploaderThumbnailURL");
            this.txtCustomUploaderThumbnailURL.Name = "txtCustomUploaderThumbnailURL";
            this.txtCustomUploaderThumbnailURL.TextChanged += new System.EventHandler(this.txtCustomUploaderThumbnailURL_TextChanged);
            this.txtCustomUploaderThumbnailURL.Enter += new System.EventHandler(this.txtCustomUploaderThumbnailURL_Enter);
            // 
            // txtCustomUploaderDeletionURL
            // 
            resources.ApplyResources(this.txtCustomUploaderDeletionURL, "txtCustomUploaderDeletionURL");
            this.txtCustomUploaderDeletionURL.Name = "txtCustomUploaderDeletionURL";
            this.txtCustomUploaderDeletionURL.TextChanged += new System.EventHandler(this.txtCustomUploaderDeletionURL_TextChanged);
            this.txtCustomUploaderDeletionURL.Enter += new System.EventHandler(this.txtCustomUploaderDeletionURL_Enter);
            // 
            // lblCustomUploaderRequestURL
            // 
            resources.ApplyResources(this.lblCustomUploaderRequestURL, "lblCustomUploaderRequestURL");
            this.lblCustomUploaderRequestURL.Name = "lblCustomUploaderRequestURL";
            // 
            // lblCustomUploaderResponseType
            // 
            resources.ApplyResources(this.lblCustomUploaderResponseType, "lblCustomUploaderResponseType");
            this.lblCustomUploaderResponseType.Name = "lblCustomUploaderResponseType";
            // 
            // lblCustomUploaderDeletionURL
            // 
            resources.ApplyResources(this.lblCustomUploaderDeletionURL, "lblCustomUploaderDeletionURL");
            this.lblCustomUploaderDeletionURL.Name = "lblCustomUploaderDeletionURL";
            // 
            // btnCustomUploaderExamples
            // 
            resources.ApplyResources(this.btnCustomUploaderExamples, "btnCustomUploaderExamples");
            this.btnCustomUploaderExamples.Name = "btnCustomUploaderExamples";
            this.btnCustomUploaderExamples.UseVisualStyleBackColor = true;
            this.btnCustomUploaderExamples.Click += new System.EventHandler(this.btnCustomUploaderExamples_Click);
            // 
            // btnCustomUploaderHelp
            // 
            resources.ApplyResources(this.btnCustomUploaderHelp, "btnCustomUploaderHelp");
            this.btnCustomUploaderHelp.Name = "btnCustomUploaderHelp";
            this.btnCustomUploaderHelp.UseVisualStyleBackColor = true;
            this.btnCustomUploaderHelp.Click += new System.EventHandler(this.btnCustomUploaderHelp_Click);
            // 
            // lblCustomUploaderImageUploader
            // 
            resources.ApplyResources(this.lblCustomUploaderImageUploader, "lblCustomUploaderImageUploader");
            this.lblCustomUploaderImageUploader.Name = "lblCustomUploaderImageUploader";
            // 
            // btnCustomUploaderFileUploaderTest
            // 
            resources.ApplyResources(this.btnCustomUploaderFileUploaderTest, "btnCustomUploaderFileUploaderTest");
            this.btnCustomUploaderFileUploaderTest.Name = "btnCustomUploaderFileUploaderTest";
            this.btnCustomUploaderFileUploaderTest.UseVisualStyleBackColor = true;
            this.btnCustomUploaderFileUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderFileUploaderTest_Click);
            // 
            // lblCustomUploaderFileUploader
            // 
            resources.ApplyResources(this.lblCustomUploaderFileUploader, "lblCustomUploaderFileUploader");
            this.lblCustomUploaderFileUploader.Name = "lblCustomUploaderFileUploader";
            // 
            // btnCustomUploaderImageUploaderTest
            // 
            resources.ApplyResources(this.btnCustomUploaderImageUploaderTest, "btnCustomUploaderImageUploaderTest");
            this.btnCustomUploaderImageUploaderTest.Name = "btnCustomUploaderImageUploaderTest";
            this.btnCustomUploaderImageUploaderTest.UseVisualStyleBackColor = true;
            this.btnCustomUploaderImageUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderImageUploaderTest_Click);
            // 
            // lblCustomUploaderTestResult
            // 
            resources.ApplyResources(this.lblCustomUploaderTestResult, "lblCustomUploaderTestResult");
            this.lblCustomUploaderTestResult.Name = "lblCustomUploaderTestResult";
            // 
            // cbCustomUploaderFileUploader
            // 
            this.cbCustomUploaderFileUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderFileUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderFileUploader, "cbCustomUploaderFileUploader");
            this.cbCustomUploaderFileUploader.Name = "cbCustomUploaderFileUploader";
            this.cbCustomUploaderFileUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderFileUploader_SelectedIndexChanged);
            // 
            // btnCustomUploaderShowLastResponse
            // 
            resources.ApplyResources(this.btnCustomUploaderShowLastResponse, "btnCustomUploaderShowLastResponse");
            this.btnCustomUploaderShowLastResponse.Name = "btnCustomUploaderShowLastResponse";
            this.btnCustomUploaderShowLastResponse.UseVisualStyleBackColor = true;
            this.btnCustomUploaderShowLastResponse.Click += new System.EventHandler(this.btnCustomUploaderShowLastResponse_Click);
            // 
            // cbCustomUploaderURLShortener
            // 
            this.cbCustomUploaderURLShortener.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderURLShortener.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderURLShortener, "cbCustomUploaderURLShortener");
            this.cbCustomUploaderURLShortener.Name = "cbCustomUploaderURLShortener";
            this.cbCustomUploaderURLShortener.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderURLShortener_SelectedIndexChanged);
            // 
            // gbCustomUploaders
            // 
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderDuplicate);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploadersExportAll);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderClearUploaders);
            this.gbCustomUploaders.Controls.Add(this.eiCustomUploaders);
            this.gbCustomUploaders.Controls.Add(this.lbCustomUploaderList);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderRemove);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderAdd);
            resources.ApplyResources(this.gbCustomUploaders, "gbCustomUploaders");
            this.gbCustomUploaders.Name = "gbCustomUploaders";
            this.gbCustomUploaders.TabStop = false;
            // 
            // btnCustomUploaderDuplicate
            // 
            resources.ApplyResources(this.btnCustomUploaderDuplicate, "btnCustomUploaderDuplicate");
            this.btnCustomUploaderDuplicate.Name = "btnCustomUploaderDuplicate";
            this.btnCustomUploaderDuplicate.UseVisualStyleBackColor = true;
            this.btnCustomUploaderDuplicate.Click += new System.EventHandler(this.btnCustomUploaderDuplicate_Click);
            // 
            // btnCustomUploadersExportAll
            // 
            resources.ApplyResources(this.btnCustomUploadersExportAll, "btnCustomUploadersExportAll");
            this.btnCustomUploadersExportAll.Name = "btnCustomUploadersExportAll";
            this.btnCustomUploadersExportAll.UseVisualStyleBackColor = true;
            this.btnCustomUploadersExportAll.Click += new System.EventHandler(this.btnCustomUploadersExportAll_Click);
            // 
            // btnCustomUploaderClearUploaders
            // 
            resources.ApplyResources(this.btnCustomUploaderClearUploaders, "btnCustomUploaderClearUploaders");
            this.btnCustomUploaderClearUploaders.Name = "btnCustomUploaderClearUploaders";
            this.btnCustomUploaderClearUploaders.UseVisualStyleBackColor = true;
            this.btnCustomUploaderClearUploaders.Click += new System.EventHandler(this.btnCustomUploaderClearUploaders_Click);
            // 
            // eiCustomUploaders
            // 
            this.eiCustomUploaders.CustomFilter = "ShareX custom uploader (*.sxcu)|*.sxcu";
            this.eiCustomUploaders.ExportIgnoreDefaultValue = true;
            this.eiCustomUploaders.ExportIgnoreNull = true;
            resources.ApplyResources(this.eiCustomUploaders, "eiCustomUploaders");
            this.eiCustomUploaders.Name = "eiCustomUploaders";
            this.eiCustomUploaders.ObjectType = null;
            this.eiCustomUploaders.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.eiCustomUploaders_ExportRequested);
            this.eiCustomUploaders.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.eiCustomUploaders_ImportRequested);
            // 
            // lbCustomUploaderList
            // 
            this.lbCustomUploaderList.FormattingEnabled = true;
            resources.ApplyResources(this.lbCustomUploaderList, "lbCustomUploaderList");
            this.lbCustomUploaderList.Name = "lbCustomUploaderList";
            this.lbCustomUploaderList.SelectedIndexChanged += new System.EventHandler(this.lbCustomUploaderList_SelectedIndexChanged);
            // 
            // btnCustomUploaderRemove
            // 
            resources.ApplyResources(this.btnCustomUploaderRemove, "btnCustomUploaderRemove");
            this.btnCustomUploaderRemove.Name = "btnCustomUploaderRemove";
            this.btnCustomUploaderRemove.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRemove.Click += new System.EventHandler(this.btnCustomUploaderRemove_Click);
            // 
            // btnCustomUploaderAdd
            // 
            resources.ApplyResources(this.btnCustomUploaderAdd, "btnCustomUploaderAdd");
            this.btnCustomUploaderAdd.Name = "btnCustomUploaderAdd";
            this.btnCustomUploaderAdd.UseVisualStyleBackColor = true;
            this.btnCustomUploaderAdd.Click += new System.EventHandler(this.btnCustomUploaderAdd_Click);
            // 
            // lblCustomUploaderTextUploader
            // 
            resources.ApplyResources(this.lblCustomUploaderTextUploader, "lblCustomUploaderTextUploader");
            this.lblCustomUploaderTextUploader.Name = "lblCustomUploaderTextUploader";
            // 
            // btnCustomUploaderURLShortenerTest
            // 
            resources.ApplyResources(this.btnCustomUploaderURLShortenerTest, "btnCustomUploaderURLShortenerTest");
            this.btnCustomUploaderURLShortenerTest.Name = "btnCustomUploaderURLShortenerTest";
            this.btnCustomUploaderURLShortenerTest.UseVisualStyleBackColor = true;
            this.btnCustomUploaderURLShortenerTest.Click += new System.EventHandler(this.btnCustomUploaderURLShortenerTest_Click);
            // 
            // cbCustomUploaderTextUploader
            // 
            this.cbCustomUploaderTextUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderTextUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderTextUploader, "cbCustomUploaderTextUploader");
            this.cbCustomUploaderTextUploader.Name = "cbCustomUploaderTextUploader";
            this.cbCustomUploaderTextUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderTextUploader_SelectedIndexChanged);
            // 
            // lblCustomUploaderURLShortener
            // 
            resources.ApplyResources(this.lblCustomUploaderURLShortener, "lblCustomUploaderURLShortener");
            this.lblCustomUploaderURLShortener.Name = "lblCustomUploaderURLShortener";
            // 
            // btnCustomUploaderTextUploaderTest
            // 
            resources.ApplyResources(this.btnCustomUploaderTextUploaderTest, "btnCustomUploaderTextUploaderTest");
            this.btnCustomUploaderTextUploaderTest.Name = "btnCustomUploaderTextUploaderTest";
            this.btnCustomUploaderTextUploaderTest.UseVisualStyleBackColor = true;
            this.btnCustomUploaderTextUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderTextUploaderTest_Click);
            // 
            // cbCustomUploaderImageUploader
            // 
            this.cbCustomUploaderImageUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderImageUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderImageUploader, "cbCustomUploaderImageUploader");
            this.cbCustomUploaderImageUploader.Name = "cbCustomUploaderImageUploader";
            this.cbCustomUploaderImageUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderImageUploader_SelectedIndexChanged);
            // 
            // txtCustomUploaderLog
            // 
            resources.ApplyResources(this.txtCustomUploaderLog, "txtCustomUploaderLog");
            this.txtCustomUploaderLog.Name = "txtCustomUploaderLog";
            this.txtCustomUploaderLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtCustomUploaderLog_LinkClicked);
            // 
            // tpURLShorteners
            // 
            this.tpURLShorteners.Controls.Add(this.tcURLShorteners);
            resources.ApplyResources(this.tpURLShorteners, "tpURLShorteners");
            this.tpURLShorteners.Name = "tpURLShorteners";
            this.tpURLShorteners.UseVisualStyleBackColor = true;
            // 
            // tcURLShorteners
            // 
            this.tcURLShorteners.Controls.Add(this.tpBitly);
            this.tcURLShorteners.Controls.Add(this.tpGoogleURLShortener);
            this.tcURLShorteners.Controls.Add(this.tpYourls);
            this.tcURLShorteners.Controls.Add(this.tpAdFly);
            this.tcURLShorteners.Controls.Add(this.tpCoinURL);
            this.tcURLShorteners.Controls.Add(this.tpPolr);
            resources.ApplyResources(this.tcURLShorteners, "tcURLShorteners");
            this.tcURLShorteners.Name = "tcURLShorteners";
            this.tcURLShorteners.SelectedIndex = 0;
            // 
            // tpBitly
            // 
            this.tpBitly.Controls.Add(this.txtBitlyDomain);
            this.tpBitly.Controls.Add(this.lblBitlyDomain);
            this.tpBitly.Controls.Add(this.oauth2Bitly);
            resources.ApplyResources(this.tpBitly, "tpBitly");
            this.tpBitly.Name = "tpBitly";
            this.tpBitly.UseVisualStyleBackColor = true;
            // 
            // txtBitlyDomain
            // 
            resources.ApplyResources(this.txtBitlyDomain, "txtBitlyDomain");
            this.txtBitlyDomain.Name = "txtBitlyDomain";
            this.txtBitlyDomain.TextChanged += new System.EventHandler(this.txtBitlyDomain_TextChanged);
            // 
            // lblBitlyDomain
            // 
            resources.ApplyResources(this.lblBitlyDomain, "lblBitlyDomain");
            this.lblBitlyDomain.Name = "lblBitlyDomain";
            // 
            // oauth2Bitly
            // 
            this.oauth2Bitly.IsRefreshable = false;
            resources.ApplyResources(this.oauth2Bitly, "oauth2Bitly");
            this.oauth2Bitly.Name = "oauth2Bitly";
            this.oauth2Bitly.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Bitly_OpenButtonClicked);
            this.oauth2Bitly.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Bitly_CompleteButtonClicked);
            this.oauth2Bitly.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Bitly_ClearButtonClicked);
            // 
            // tpGoogleURLShortener
            // 
            this.tpGoogleURLShortener.Controls.Add(this.oauth2GoogleURLShortener);
            this.tpGoogleURLShortener.Controls.Add(this.atcGoogleURLShortenerAccountType);
            resources.ApplyResources(this.tpGoogleURLShortener, "tpGoogleURLShortener");
            this.tpGoogleURLShortener.Name = "tpGoogleURLShortener";
            this.tpGoogleURLShortener.UseVisualStyleBackColor = true;
            // 
            // oauth2GoogleURLShortener
            // 
            resources.ApplyResources(this.oauth2GoogleURLShortener, "oauth2GoogleURLShortener");
            this.oauth2GoogleURLShortener.Name = "oauth2GoogleURLShortener";
            this.oauth2GoogleURLShortener.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2GoogleURLShortener_OpenButtonClicked);
            this.oauth2GoogleURLShortener.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2GoogleURLShortener_CompleteButtonClicked);
            this.oauth2GoogleURLShortener.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2GoogleURLShortener_ClearButtonClicked);
            this.oauth2GoogleURLShortener.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2GoogleURLShortener_RefreshButtonClicked);
            // 
            // atcGoogleURLShortenerAccountType
            // 
            resources.ApplyResources(this.atcGoogleURLShortenerAccountType, "atcGoogleURLShortenerAccountType");
            this.atcGoogleURLShortenerAccountType.Name = "atcGoogleURLShortenerAccountType";
            this.atcGoogleURLShortenerAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcGoogleURLShortenerAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcGoogleURLShortenerAccountType_AccountTypeChanged);
            // 
            // tpYourls
            // 
            this.tpYourls.Controls.Add(this.txtYourlsPassword);
            this.tpYourls.Controls.Add(this.txtYourlsUsername);
            this.tpYourls.Controls.Add(this.txtYourlsSignature);
            this.tpYourls.Controls.Add(this.lblYourlsNote);
            this.tpYourls.Controls.Add(this.lblYourlsPassword);
            this.tpYourls.Controls.Add(this.lblYourlsUsername);
            this.tpYourls.Controls.Add(this.lblYourlsSignature);
            this.tpYourls.Controls.Add(this.txtYourlsAPIURL);
            this.tpYourls.Controls.Add(this.lblYourlsAPIURL);
            resources.ApplyResources(this.tpYourls, "tpYourls");
            this.tpYourls.Name = "tpYourls";
            this.tpYourls.UseVisualStyleBackColor = true;
            // 
            // txtYourlsPassword
            // 
            resources.ApplyResources(this.txtYourlsPassword, "txtYourlsPassword");
            this.txtYourlsPassword.Name = "txtYourlsPassword";
            this.txtYourlsPassword.UseSystemPasswordChar = true;
            this.txtYourlsPassword.TextChanged += new System.EventHandler(this.txtYourlsPassword_TextChanged);
            // 
            // txtYourlsUsername
            // 
            resources.ApplyResources(this.txtYourlsUsername, "txtYourlsUsername");
            this.txtYourlsUsername.Name = "txtYourlsUsername";
            this.txtYourlsUsername.TextChanged += new System.EventHandler(this.txtYourlsUsername_TextChanged);
            // 
            // txtYourlsSignature
            // 
            resources.ApplyResources(this.txtYourlsSignature, "txtYourlsSignature");
            this.txtYourlsSignature.Name = "txtYourlsSignature";
            this.txtYourlsSignature.UseSystemPasswordChar = true;
            this.txtYourlsSignature.TextChanged += new System.EventHandler(this.txtYourlsSignature_TextChanged);
            // 
            // lblYourlsNote
            // 
            resources.ApplyResources(this.lblYourlsNote, "lblYourlsNote");
            this.lblYourlsNote.Name = "lblYourlsNote";
            // 
            // lblYourlsPassword
            // 
            resources.ApplyResources(this.lblYourlsPassword, "lblYourlsPassword");
            this.lblYourlsPassword.Name = "lblYourlsPassword";
            // 
            // lblYourlsUsername
            // 
            resources.ApplyResources(this.lblYourlsUsername, "lblYourlsUsername");
            this.lblYourlsUsername.Name = "lblYourlsUsername";
            // 
            // lblYourlsSignature
            // 
            resources.ApplyResources(this.lblYourlsSignature, "lblYourlsSignature");
            this.lblYourlsSignature.Name = "lblYourlsSignature";
            // 
            // txtYourlsAPIURL
            // 
            resources.ApplyResources(this.txtYourlsAPIURL, "txtYourlsAPIURL");
            this.txtYourlsAPIURL.Name = "txtYourlsAPIURL";
            this.txtYourlsAPIURL.TextChanged += new System.EventHandler(this.txtYourlsAPIURL_TextChanged);
            // 
            // lblYourlsAPIURL
            // 
            resources.ApplyResources(this.lblYourlsAPIURL, "lblYourlsAPIURL");
            this.lblYourlsAPIURL.Name = "lblYourlsAPIURL";
            // 
            // tpAdFly
            // 
            this.tpAdFly.Controls.Add(this.llAdflyLink);
            this.tpAdFly.Controls.Add(this.txtAdflyAPIUID);
            this.tpAdFly.Controls.Add(this.lblAdflyAPIUID);
            this.tpAdFly.Controls.Add(this.txtAdflyAPIKEY);
            this.tpAdFly.Controls.Add(this.lblAdflyAPIKEY);
            resources.ApplyResources(this.tpAdFly, "tpAdFly");
            this.tpAdFly.Name = "tpAdFly";
            this.tpAdFly.UseVisualStyleBackColor = true;
            // 
            // llAdflyLink
            // 
            resources.ApplyResources(this.llAdflyLink, "llAdflyLink");
            this.llAdflyLink.Name = "llAdflyLink";
            this.llAdflyLink.TabStop = true;
            this.llAdflyLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAdflyLink_LinkClicked);
            // 
            // txtAdflyAPIUID
            // 
            resources.ApplyResources(this.txtAdflyAPIUID, "txtAdflyAPIUID");
            this.txtAdflyAPIUID.Name = "txtAdflyAPIUID";
            this.txtAdflyAPIUID.UseSystemPasswordChar = true;
            this.txtAdflyAPIUID.TextChanged += new System.EventHandler(this.txtAdflyAPIUID_TextChanged);
            // 
            // lblAdflyAPIUID
            // 
            resources.ApplyResources(this.lblAdflyAPIUID, "lblAdflyAPIUID");
            this.lblAdflyAPIUID.Name = "lblAdflyAPIUID";
            // 
            // txtAdflyAPIKEY
            // 
            resources.ApplyResources(this.txtAdflyAPIKEY, "txtAdflyAPIKEY");
            this.txtAdflyAPIKEY.Name = "txtAdflyAPIKEY";
            this.txtAdflyAPIKEY.TextChanged += new System.EventHandler(this.txtAdflyAPIKEY_TextChanged);
            // 
            // lblAdflyAPIKEY
            // 
            resources.ApplyResources(this.lblAdflyAPIKEY, "lblAdflyAPIKEY");
            this.lblAdflyAPIKEY.Name = "lblAdflyAPIKEY";
            // 
            // tpCoinURL
            // 
            this.tpCoinURL.Controls.Add(this.txtCoinURLUUID);
            this.tpCoinURL.Controls.Add(this.lblCoinURLUUID);
            resources.ApplyResources(this.tpCoinURL, "tpCoinURL");
            this.tpCoinURL.Name = "tpCoinURL";
            this.tpCoinURL.UseVisualStyleBackColor = true;
            // 
            // txtCoinURLUUID
            // 
            resources.ApplyResources(this.txtCoinURLUUID, "txtCoinURLUUID");
            this.txtCoinURLUUID.Name = "txtCoinURLUUID";
            this.txtCoinURLUUID.UseSystemPasswordChar = true;
            this.txtCoinURLUUID.TextChanged += new System.EventHandler(this.txtCoinURLUUID_TextChanged);
            // 
            // lblCoinURLUUID
            // 
            resources.ApplyResources(this.lblCoinURLUUID, "lblCoinURLUUID");
            this.lblCoinURLUUID.Name = "lblCoinURLUUID";
            // 
            // tpPolr
            // 
            this.tpPolr.Controls.Add(this.cbPolrUseAPIv1);
            this.tpPolr.Controls.Add(this.cbPolrIsSecret);
            this.tpPolr.Controls.Add(this.txtPolrAPIKey);
            this.tpPolr.Controls.Add(this.lblPolrAPIKey);
            this.tpPolr.Controls.Add(this.txtPolrAPIHostname);
            this.tpPolr.Controls.Add(this.lblPolrAPIHostname);
            resources.ApplyResources(this.tpPolr, "tpPolr");
            this.tpPolr.Name = "tpPolr";
            this.tpPolr.UseVisualStyleBackColor = true;
            // 
            // cbPolrUseAPIv1
            // 
            resources.ApplyResources(this.cbPolrUseAPIv1, "cbPolrUseAPIv1");
            this.cbPolrUseAPIv1.Name = "cbPolrUseAPIv1";
            this.cbPolrUseAPIv1.UseVisualStyleBackColor = true;
            this.cbPolrUseAPIv1.CheckedChanged += new System.EventHandler(this.cbPolrUseAPIv1_CheckedChanged);
            // 
            // cbPolrIsSecret
            // 
            resources.ApplyResources(this.cbPolrIsSecret, "cbPolrIsSecret");
            this.cbPolrIsSecret.Name = "cbPolrIsSecret";
            this.cbPolrIsSecret.UseVisualStyleBackColor = true;
            this.cbPolrIsSecret.CheckedChanged += new System.EventHandler(this.cbPolrIsSecret_CheckedChanged);
            // 
            // txtPolrAPIKey
            // 
            resources.ApplyResources(this.txtPolrAPIKey, "txtPolrAPIKey");
            this.txtPolrAPIKey.Name = "txtPolrAPIKey";
            this.txtPolrAPIKey.UseSystemPasswordChar = true;
            this.txtPolrAPIKey.TextChanged += new System.EventHandler(this.txtPolrAPIKey_TextChanged);
            // 
            // lblPolrAPIKey
            // 
            resources.ApplyResources(this.lblPolrAPIKey, "lblPolrAPIKey");
            this.lblPolrAPIKey.Name = "lblPolrAPIKey";
            // 
            // txtPolrAPIHostname
            // 
            resources.ApplyResources(this.txtPolrAPIHostname, "txtPolrAPIHostname");
            this.txtPolrAPIHostname.Name = "txtPolrAPIHostname";
            this.txtPolrAPIHostname.TextChanged += new System.EventHandler(this.txtPolrAPIHostname_TextChanged);
            // 
            // lblPolrAPIHostname
            // 
            resources.ApplyResources(this.lblPolrAPIHostname, "lblPolrAPIHostname");
            this.lblPolrAPIHostname.Name = "lblPolrAPIHostname";
            // 
            // tpFileUploaders
            // 
            this.tpFileUploaders.Controls.Add(this.tcFileUploaders);
            resources.ApplyResources(this.tpFileUploaders, "tpFileUploaders");
            this.tpFileUploaders.Name = "tpFileUploaders";
            this.tpFileUploaders.UseVisualStyleBackColor = true;
            // 
            // tcFileUploaders
            // 
            this.tcFileUploaders.Controls.Add(this.tpFTP);
            this.tcFileUploaders.Controls.Add(this.tpDropbox);
            this.tcFileUploaders.Controls.Add(this.tpOneDrive);
            this.tcFileUploaders.Controls.Add(this.tpGoogleDrive);
            this.tcFileUploaders.Controls.Add(this.tpPuush);
            this.tcFileUploaders.Controls.Add(this.tpBox);
            this.tcFileUploaders.Controls.Add(this.tpAmazonS3);
            this.tcFileUploaders.Controls.Add(this.tpAzureStorage);
            this.tcFileUploaders.Controls.Add(this.tpGfycat);
            this.tcFileUploaders.Controls.Add(this.tpMega);
            this.tcFileUploaders.Controls.Add(this.tpOwnCloud);
            this.tcFileUploaders.Controls.Add(this.tpMediaFire);
            this.tcFileUploaders.Controls.Add(this.tpPushbullet);
            this.tcFileUploaders.Controls.Add(this.tpSendSpace);
            this.tcFileUploaders.Controls.Add(this.tpGe_tt);
            this.tcFileUploaders.Controls.Add(this.tpHostr);
            this.tcFileUploaders.Controls.Add(this.tpJira);
            this.tcFileUploaders.Controls.Add(this.tpLambda);
            this.tcFileUploaders.Controls.Add(this.tpPomf);
            this.tcFileUploaders.Controls.Add(this.tpSeafile);
            this.tcFileUploaders.Controls.Add(this.tpStreamable);
            this.tcFileUploaders.Controls.Add(this.tpSul);
            this.tcFileUploaders.Controls.Add(this.tpLithiio);
            this.tcFileUploaders.Controls.Add(this.tpPlik);
            this.tcFileUploaders.Controls.Add(this.tpSharedFolder);
            this.tcFileUploaders.Controls.Add(this.tpEmail);
            resources.ApplyResources(this.tcFileUploaders, "tcFileUploaders");
            this.tcFileUploaders.Multiline = true;
            this.tcFileUploaders.Name = "tcFileUploaders";
            this.tcFileUploaders.SelectedIndex = 0;
            // 
            // tpFTP
            // 
            this.tpFTP.Controls.Add(this.gbFTPAccount);
            this.tpFTP.Controls.Add(this.btnFTPDuplicate);
            this.tpFTP.Controls.Add(this.btnFTPRemove);
            this.tpFTP.Controls.Add(this.btnFTPAdd);
            this.tpFTP.Controls.Add(this.cbFTPAccounts);
            this.tpFTP.Controls.Add(this.lblFTPAccounts);
            this.tpFTP.Controls.Add(this.lblFTPFile);
            this.tpFTP.Controls.Add(this.lblFTPText);
            this.tpFTP.Controls.Add(this.lblFTPImage);
            this.tpFTP.Controls.Add(this.cbFTPImage);
            this.tpFTP.Controls.Add(this.cbFTPFile);
            this.tpFTP.Controls.Add(this.cbFTPText);
            resources.ApplyResources(this.tpFTP, "tpFTP");
            this.tpFTP.Name = "tpFTP";
            this.tpFTP.UseVisualStyleBackColor = true;
            // 
            // gbFTPAccount
            // 
            this.gbFTPAccount.Controls.Add(this.gbSFTP);
            this.gbFTPAccount.Controls.Add(this.cbFTPAppendRemoteDirectory);
            this.gbFTPAccount.Controls.Add(this.btnFTPTest);
            this.gbFTPAccount.Controls.Add(this.lblFTPProtocol);
            this.gbFTPAccount.Controls.Add(this.lblFTPName);
            this.gbFTPAccount.Controls.Add(this.cbFTPRemoveFileExtension);
            this.gbFTPAccount.Controls.Add(this.txtFTPName);
            this.gbFTPAccount.Controls.Add(this.lblFTPHost);
            this.gbFTPAccount.Controls.Add(this.eiFTP);
            this.gbFTPAccount.Controls.Add(this.pFTPTransferMode);
            this.gbFTPAccount.Controls.Add(this.btnFTPClient);
            this.gbFTPAccount.Controls.Add(this.txtFTPHost);
            this.gbFTPAccount.Controls.Add(this.pFTPProtocol);
            this.gbFTPAccount.Controls.Add(this.lblFTPPort);
            this.gbFTPAccount.Controls.Add(this.lblFTPTransferMode);
            this.gbFTPAccount.Controls.Add(this.nudFTPPort);
            this.gbFTPAccount.Controls.Add(this.lblFTPURLPreviewValue);
            this.gbFTPAccount.Controls.Add(this.lblFTPUsername);
            this.gbFTPAccount.Controls.Add(this.lblFTPURLPreview);
            this.gbFTPAccount.Controls.Add(this.txtFTPUsername);
            this.gbFTPAccount.Controls.Add(this.cbFTPURLPathProtocol);
            this.gbFTPAccount.Controls.Add(this.lblFTPPassword);
            this.gbFTPAccount.Controls.Add(this.txtFTPURLPath);
            this.gbFTPAccount.Controls.Add(this.txtFTPPassword);
            this.gbFTPAccount.Controls.Add(this.lblFTPURLPath);
            this.gbFTPAccount.Controls.Add(this.lblFTPRemoteDirectory);
            this.gbFTPAccount.Controls.Add(this.txtFTPRemoteDirectory);
            this.gbFTPAccount.Controls.Add(this.gbFTPS);
            resources.ApplyResources(this.gbFTPAccount, "gbFTPAccount");
            this.gbFTPAccount.Name = "gbFTPAccount";
            this.gbFTPAccount.TabStop = false;
            // 
            // gbSFTP
            // 
            this.gbSFTP.Controls.Add(this.txtSFTPKeyPassphrase);
            this.gbSFTP.Controls.Add(this.btnSFTPKeyLocationBrowse);
            this.gbSFTP.Controls.Add(this.lblSFTPKeyPassphrase);
            this.gbSFTP.Controls.Add(this.txtSFTPKeyLocation);
            this.gbSFTP.Controls.Add(this.lblSFTPKeyLocation);
            resources.ApplyResources(this.gbSFTP, "gbSFTP");
            this.gbSFTP.Name = "gbSFTP";
            this.gbSFTP.TabStop = false;
            // 
            // txtSFTPKeyPassphrase
            // 
            resources.ApplyResources(this.txtSFTPKeyPassphrase, "txtSFTPKeyPassphrase");
            this.txtSFTPKeyPassphrase.Name = "txtSFTPKeyPassphrase";
            this.txtSFTPKeyPassphrase.UseSystemPasswordChar = true;
            this.txtSFTPKeyPassphrase.TextChanged += new System.EventHandler(this.txtSFTPKeyPassphrase_TextChanged);
            // 
            // btnSFTPKeyLocationBrowse
            // 
            resources.ApplyResources(this.btnSFTPKeyLocationBrowse, "btnSFTPKeyLocationBrowse");
            this.btnSFTPKeyLocationBrowse.Name = "btnSFTPKeyLocationBrowse";
            this.btnSFTPKeyLocationBrowse.UseVisualStyleBackColor = true;
            this.btnSFTPKeyLocationBrowse.Click += new System.EventHandler(this.btnSFTPKeyLocationBrowse_Click);
            // 
            // lblSFTPKeyPassphrase
            // 
            resources.ApplyResources(this.lblSFTPKeyPassphrase, "lblSFTPKeyPassphrase");
            this.lblSFTPKeyPassphrase.Name = "lblSFTPKeyPassphrase";
            // 
            // txtSFTPKeyLocation
            // 
            resources.ApplyResources(this.txtSFTPKeyLocation, "txtSFTPKeyLocation");
            this.txtSFTPKeyLocation.Name = "txtSFTPKeyLocation";
            this.txtSFTPKeyLocation.TextChanged += new System.EventHandler(this.txtSFTPKeyLocation_TextChanged);
            // 
            // lblSFTPKeyLocation
            // 
            resources.ApplyResources(this.lblSFTPKeyLocation, "lblSFTPKeyLocation");
            this.lblSFTPKeyLocation.Name = "lblSFTPKeyLocation";
            // 
            // cbFTPAppendRemoteDirectory
            // 
            resources.ApplyResources(this.cbFTPAppendRemoteDirectory, "cbFTPAppendRemoteDirectory");
            this.cbFTPAppendRemoteDirectory.Name = "cbFTPAppendRemoteDirectory";
            this.cbFTPAppendRemoteDirectory.UseVisualStyleBackColor = true;
            this.cbFTPAppendRemoteDirectory.CheckedChanged += new System.EventHandler(this.cbFTPAppendRemoteDirectory_CheckedChanged);
            // 
            // btnFTPTest
            // 
            resources.ApplyResources(this.btnFTPTest, "btnFTPTest");
            this.btnFTPTest.Name = "btnFTPTest";
            this.btnFTPTest.UseVisualStyleBackColor = true;
            this.btnFTPTest.Click += new System.EventHandler(this.btnFTPTest_Click);
            // 
            // lblFTPProtocol
            // 
            resources.ApplyResources(this.lblFTPProtocol, "lblFTPProtocol");
            this.lblFTPProtocol.Name = "lblFTPProtocol";
            // 
            // lblFTPName
            // 
            resources.ApplyResources(this.lblFTPName, "lblFTPName");
            this.lblFTPName.Name = "lblFTPName";
            // 
            // cbFTPRemoveFileExtension
            // 
            resources.ApplyResources(this.cbFTPRemoveFileExtension, "cbFTPRemoveFileExtension");
            this.cbFTPRemoveFileExtension.Name = "cbFTPRemoveFileExtension";
            this.cbFTPRemoveFileExtension.UseVisualStyleBackColor = true;
            this.cbFTPRemoveFileExtension.CheckedChanged += new System.EventHandler(this.cbFTPRemoveFileExtension_CheckedChanged);
            // 
            // txtFTPName
            // 
            resources.ApplyResources(this.txtFTPName, "txtFTPName");
            this.txtFTPName.Name = "txtFTPName";
            this.txtFTPName.TextChanged += new System.EventHandler(this.txtFTPName_TextChanged);
            // 
            // lblFTPHost
            // 
            resources.ApplyResources(this.lblFTPHost, "lblFTPHost");
            this.lblFTPHost.Name = "lblFTPHost";
            // 
            // eiFTP
            // 
            resources.ApplyResources(this.eiFTP, "eiFTP");
            this.eiFTP.Name = "eiFTP";
            this.eiFTP.ObjectType = null;
            this.eiFTP.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.eiFTP_ExportRequested);
            this.eiFTP.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.eiFTP_ImportRequested);
            // 
            // pFTPTransferMode
            // 
            resources.ApplyResources(this.pFTPTransferMode, "pFTPTransferMode");
            this.pFTPTransferMode.Controls.Add(this.rbFTPTransferModeActive);
            this.pFTPTransferMode.Controls.Add(this.rbFTPTransferModePassive);
            this.pFTPTransferMode.Name = "pFTPTransferMode";
            // 
            // rbFTPTransferModeActive
            // 
            resources.ApplyResources(this.rbFTPTransferModeActive, "rbFTPTransferModeActive");
            this.rbFTPTransferModeActive.Name = "rbFTPTransferModeActive";
            this.rbFTPTransferModeActive.UseVisualStyleBackColor = true;
            this.rbFTPTransferModeActive.CheckedChanged += new System.EventHandler(this.rbFTPTransferModeActive_CheckedChanged);
            // 
            // rbFTPTransferModePassive
            // 
            resources.ApplyResources(this.rbFTPTransferModePassive, "rbFTPTransferModePassive");
            this.rbFTPTransferModePassive.Checked = true;
            this.rbFTPTransferModePassive.Name = "rbFTPTransferModePassive";
            this.rbFTPTransferModePassive.TabStop = true;
            this.rbFTPTransferModePassive.UseVisualStyleBackColor = true;
            this.rbFTPTransferModePassive.CheckedChanged += new System.EventHandler(this.rbFTPTransferModePassive_CheckedChanged);
            // 
            // btnFTPClient
            // 
            resources.ApplyResources(this.btnFTPClient, "btnFTPClient");
            this.btnFTPClient.Name = "btnFTPClient";
            this.btnFTPClient.UseVisualStyleBackColor = true;
            this.btnFTPClient.Click += new System.EventHandler(this.btnFTPClient_Click);
            // 
            // txtFTPHost
            // 
            resources.ApplyResources(this.txtFTPHost, "txtFTPHost");
            this.txtFTPHost.Name = "txtFTPHost";
            this.txtFTPHost.TextChanged += new System.EventHandler(this.txtFTPHost_TextChanged);
            // 
            // pFTPProtocol
            // 
            resources.ApplyResources(this.pFTPProtocol, "pFTPProtocol");
            this.pFTPProtocol.Controls.Add(this.rbFTPProtocolFTP);
            this.pFTPProtocol.Controls.Add(this.rbFTPProtocolFTPS);
            this.pFTPProtocol.Controls.Add(this.rbFTPProtocolSFTP);
            this.pFTPProtocol.Name = "pFTPProtocol";
            // 
            // rbFTPProtocolFTP
            // 
            resources.ApplyResources(this.rbFTPProtocolFTP, "rbFTPProtocolFTP");
            this.rbFTPProtocolFTP.Checked = true;
            this.rbFTPProtocolFTP.Name = "rbFTPProtocolFTP";
            this.rbFTPProtocolFTP.TabStop = true;
            this.rbFTPProtocolFTP.UseVisualStyleBackColor = true;
            this.rbFTPProtocolFTP.CheckedChanged += new System.EventHandler(this.rbFTPProtocolFTP_CheckedChanged);
            // 
            // rbFTPProtocolFTPS
            // 
            resources.ApplyResources(this.rbFTPProtocolFTPS, "rbFTPProtocolFTPS");
            this.rbFTPProtocolFTPS.Name = "rbFTPProtocolFTPS";
            this.rbFTPProtocolFTPS.UseVisualStyleBackColor = true;
            this.rbFTPProtocolFTPS.CheckedChanged += new System.EventHandler(this.rbFTPProtocolFTPS_CheckedChanged);
            // 
            // rbFTPProtocolSFTP
            // 
            resources.ApplyResources(this.rbFTPProtocolSFTP, "rbFTPProtocolSFTP");
            this.rbFTPProtocolSFTP.Name = "rbFTPProtocolSFTP";
            this.rbFTPProtocolSFTP.UseVisualStyleBackColor = true;
            this.rbFTPProtocolSFTP.CheckedChanged += new System.EventHandler(this.rbFTPProtocolSFTP_CheckedChanged);
            // 
            // lblFTPPort
            // 
            resources.ApplyResources(this.lblFTPPort, "lblFTPPort");
            this.lblFTPPort.Name = "lblFTPPort";
            // 
            // lblFTPTransferMode
            // 
            resources.ApplyResources(this.lblFTPTransferMode, "lblFTPTransferMode");
            this.lblFTPTransferMode.Name = "lblFTPTransferMode";
            // 
            // nudFTPPort
            // 
            resources.ApplyResources(this.nudFTPPort, "nudFTPPort");
            this.nudFTPPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudFTPPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFTPPort.Name = "nudFTPPort";
            this.nudFTPPort.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.nudFTPPort.ValueChanged += new System.EventHandler(this.nudFTPPort_ValueChanged);
            // 
            // lblFTPURLPreviewValue
            // 
            resources.ApplyResources(this.lblFTPURLPreviewValue, "lblFTPURLPreviewValue");
            this.lblFTPURLPreviewValue.Name = "lblFTPURLPreviewValue";
            // 
            // lblFTPUsername
            // 
            resources.ApplyResources(this.lblFTPUsername, "lblFTPUsername");
            this.lblFTPUsername.Name = "lblFTPUsername";
            // 
            // lblFTPURLPreview
            // 
            resources.ApplyResources(this.lblFTPURLPreview, "lblFTPURLPreview");
            this.lblFTPURLPreview.Name = "lblFTPURLPreview";
            // 
            // txtFTPUsername
            // 
            resources.ApplyResources(this.txtFTPUsername, "txtFTPUsername");
            this.txtFTPUsername.Name = "txtFTPUsername";
            this.txtFTPUsername.TextChanged += new System.EventHandler(this.txtFTPUsername_TextChanged);
            // 
            // cbFTPURLPathProtocol
            // 
            this.cbFTPURLPathProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPURLPathProtocol.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPURLPathProtocol, "cbFTPURLPathProtocol");
            this.cbFTPURLPathProtocol.Name = "cbFTPURLPathProtocol";
            this.cbFTPURLPathProtocol.SelectedIndexChanged += new System.EventHandler(this.cbFTPURLPathProtocol_SelectedIndexChanged);
            // 
            // lblFTPPassword
            // 
            resources.ApplyResources(this.lblFTPPassword, "lblFTPPassword");
            this.lblFTPPassword.Name = "lblFTPPassword";
            // 
            // txtFTPURLPath
            // 
            resources.ApplyResources(this.txtFTPURLPath, "txtFTPURLPath");
            this.txtFTPURLPath.Name = "txtFTPURLPath";
            this.txtFTPURLPath.TextChanged += new System.EventHandler(this.txtFTPURLPath_TextChanged);
            // 
            // txtFTPPassword
            // 
            resources.ApplyResources(this.txtFTPPassword, "txtFTPPassword");
            this.txtFTPPassword.Name = "txtFTPPassword";
            this.txtFTPPassword.UseSystemPasswordChar = true;
            this.txtFTPPassword.TextChanged += new System.EventHandler(this.txtFTPPassword_TextChanged);
            // 
            // lblFTPURLPath
            // 
            resources.ApplyResources(this.lblFTPURLPath, "lblFTPURLPath");
            this.lblFTPURLPath.Name = "lblFTPURLPath";
            // 
            // lblFTPRemoteDirectory
            // 
            resources.ApplyResources(this.lblFTPRemoteDirectory, "lblFTPRemoteDirectory");
            this.lblFTPRemoteDirectory.Name = "lblFTPRemoteDirectory";
            // 
            // txtFTPRemoteDirectory
            // 
            resources.ApplyResources(this.txtFTPRemoteDirectory, "txtFTPRemoteDirectory");
            this.txtFTPRemoteDirectory.Name = "txtFTPRemoteDirectory";
            this.txtFTPRemoteDirectory.TextChanged += new System.EventHandler(this.txtFTPRemoteDirectory_TextChanged);
            // 
            // gbFTPS
            // 
            this.gbFTPS.Controls.Add(this.btnFTPSCertificateLocationBrowse);
            this.gbFTPS.Controls.Add(this.txtFTPSCertificateLocation);
            this.gbFTPS.Controls.Add(this.lblFTPSCertificateLocation);
            this.gbFTPS.Controls.Add(this.cbFTPSEncryption);
            this.gbFTPS.Controls.Add(this.lblFTPSEncryption);
            resources.ApplyResources(this.gbFTPS, "gbFTPS");
            this.gbFTPS.Name = "gbFTPS";
            this.gbFTPS.TabStop = false;
            // 
            // btnFTPSCertificateLocationBrowse
            // 
            resources.ApplyResources(this.btnFTPSCertificateLocationBrowse, "btnFTPSCertificateLocationBrowse");
            this.btnFTPSCertificateLocationBrowse.Name = "btnFTPSCertificateLocationBrowse";
            this.btnFTPSCertificateLocationBrowse.UseVisualStyleBackColor = true;
            this.btnFTPSCertificateLocationBrowse.Click += new System.EventHandler(this.btnFTPSCertificateLocationBrowse_Click);
            // 
            // txtFTPSCertificateLocation
            // 
            resources.ApplyResources(this.txtFTPSCertificateLocation, "txtFTPSCertificateLocation");
            this.txtFTPSCertificateLocation.Name = "txtFTPSCertificateLocation";
            this.txtFTPSCertificateLocation.TextChanged += new System.EventHandler(this.txtFTPSCertificateLocation_TextChanged);
            // 
            // lblFTPSCertificateLocation
            // 
            resources.ApplyResources(this.lblFTPSCertificateLocation, "lblFTPSCertificateLocation");
            this.lblFTPSCertificateLocation.Name = "lblFTPSCertificateLocation";
            // 
            // cbFTPSEncryption
            // 
            this.cbFTPSEncryption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPSEncryption.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPSEncryption, "cbFTPSEncryption");
            this.cbFTPSEncryption.Name = "cbFTPSEncryption";
            this.cbFTPSEncryption.SelectedIndexChanged += new System.EventHandler(this.cbFTPSEncryption_SelectedIndexChanged);
            // 
            // lblFTPSEncryption
            // 
            resources.ApplyResources(this.lblFTPSEncryption, "lblFTPSEncryption");
            this.lblFTPSEncryption.Name = "lblFTPSEncryption";
            // 
            // btnFTPDuplicate
            // 
            resources.ApplyResources(this.btnFTPDuplicate, "btnFTPDuplicate");
            this.btnFTPDuplicate.Name = "btnFTPDuplicate";
            this.btnFTPDuplicate.UseVisualStyleBackColor = true;
            this.btnFTPDuplicate.Click += new System.EventHandler(this.btnFTPDuplicate_Click);
            // 
            // btnFTPRemove
            // 
            resources.ApplyResources(this.btnFTPRemove, "btnFTPRemove");
            this.btnFTPRemove.Name = "btnFTPRemove";
            this.btnFTPRemove.UseVisualStyleBackColor = true;
            this.btnFTPRemove.Click += new System.EventHandler(this.btnFTPRemove_Click);
            // 
            // btnFTPAdd
            // 
            resources.ApplyResources(this.btnFTPAdd, "btnFTPAdd");
            this.btnFTPAdd.Name = "btnFTPAdd";
            this.btnFTPAdd.UseVisualStyleBackColor = true;
            this.btnFTPAdd.Click += new System.EventHandler(this.btnFTPAdd_Click);
            // 
            // cbFTPAccounts
            // 
            this.cbFTPAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPAccounts.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPAccounts, "cbFTPAccounts");
            this.cbFTPAccounts.Name = "cbFTPAccounts";
            this.cbFTPAccounts.SelectedIndexChanged += new System.EventHandler(this.cbFTPAccounts_SelectedIndexChanged);
            // 
            // lblFTPAccounts
            // 
            resources.ApplyResources(this.lblFTPAccounts, "lblFTPAccounts");
            this.lblFTPAccounts.Name = "lblFTPAccounts";
            // 
            // lblFTPFile
            // 
            resources.ApplyResources(this.lblFTPFile, "lblFTPFile");
            this.lblFTPFile.Name = "lblFTPFile";
            // 
            // lblFTPText
            // 
            resources.ApplyResources(this.lblFTPText, "lblFTPText");
            this.lblFTPText.Name = "lblFTPText";
            // 
            // lblFTPImage
            // 
            resources.ApplyResources(this.lblFTPImage, "lblFTPImage");
            this.lblFTPImage.Name = "lblFTPImage";
            // 
            // cbFTPImage
            // 
            this.cbFTPImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPImage.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPImage, "cbFTPImage");
            this.cbFTPImage.Name = "cbFTPImage";
            this.cbFTPImage.SelectedIndexChanged += new System.EventHandler(this.cbFTPImage_SelectedIndexChanged);
            // 
            // cbFTPFile
            // 
            this.cbFTPFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPFile.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPFile, "cbFTPFile");
            this.cbFTPFile.Name = "cbFTPFile";
            this.cbFTPFile.SelectedIndexChanged += new System.EventHandler(this.cbFTPFile_SelectedIndexChanged);
            // 
            // cbFTPText
            // 
            this.cbFTPText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPText.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPText, "cbFTPText");
            this.cbFTPText.Name = "cbFTPText";
            this.cbFTPText.SelectedIndexChanged += new System.EventHandler(this.cbFTPText_SelectedIndexChanged);
            // 
            // tpDropbox
            // 
            this.tpDropbox.Controls.Add(this.cbDropboxUseDirectLink);
            this.tpDropbox.Controls.Add(this.cbDropboxAutoCreateShareableLink);
            this.tpDropbox.Controls.Add(this.pbDropboxLogo);
            this.tpDropbox.Controls.Add(this.lblDropboxPath);
            this.tpDropbox.Controls.Add(this.txtDropboxPath);
            this.tpDropbox.Controls.Add(this.oauth2Dropbox);
            resources.ApplyResources(this.tpDropbox, "tpDropbox");
            this.tpDropbox.Name = "tpDropbox";
            this.tpDropbox.UseVisualStyleBackColor = true;
            // 
            // cbDropboxUseDirectLink
            // 
            resources.ApplyResources(this.cbDropboxUseDirectLink, "cbDropboxUseDirectLink");
            this.cbDropboxUseDirectLink.Name = "cbDropboxUseDirectLink";
            this.cbDropboxUseDirectLink.UseVisualStyleBackColor = true;
            this.cbDropboxUseDirectLink.CheckedChanged += new System.EventHandler(this.cbDropboxUseDirectLink_CheckedChanged);
            // 
            // cbDropboxAutoCreateShareableLink
            // 
            resources.ApplyResources(this.cbDropboxAutoCreateShareableLink, "cbDropboxAutoCreateShareableLink");
            this.cbDropboxAutoCreateShareableLink.Name = "cbDropboxAutoCreateShareableLink";
            this.cbDropboxAutoCreateShareableLink.UseVisualStyleBackColor = true;
            this.cbDropboxAutoCreateShareableLink.CheckedChanged += new System.EventHandler(this.cbDropboxAutoCreateShareableLink_CheckedChanged);
            // 
            // pbDropboxLogo
            // 
            this.pbDropboxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbDropboxLogo, "pbDropboxLogo");
            this.pbDropboxLogo.Name = "pbDropboxLogo";
            this.pbDropboxLogo.TabStop = false;
            this.pbDropboxLogo.Click += new System.EventHandler(this.pbDropboxLogo_Click);
            // 
            // lblDropboxPath
            // 
            resources.ApplyResources(this.lblDropboxPath, "lblDropboxPath");
            this.lblDropboxPath.Name = "lblDropboxPath";
            // 
            // txtDropboxPath
            // 
            resources.ApplyResources(this.txtDropboxPath, "txtDropboxPath");
            this.txtDropboxPath.Name = "txtDropboxPath";
            this.txtDropboxPath.TextChanged += new System.EventHandler(this.txtDropboxPath_TextChanged);
            // 
            // oauth2Dropbox
            // 
            this.oauth2Dropbox.IsRefreshable = false;
            resources.ApplyResources(this.oauth2Dropbox, "oauth2Dropbox");
            this.oauth2Dropbox.Name = "oauth2Dropbox";
            this.oauth2Dropbox.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Dropbox_OpenButtonClicked);
            this.oauth2Dropbox.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Dropbox_CompleteButtonClicked);
            this.oauth2Dropbox.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Dropbox_ClearButtonClicked);
            // 
            // tpOneDrive
            // 
            this.tpOneDrive.Controls.Add(this.tvOneDrive);
            this.tpOneDrive.Controls.Add(this.lblOneDriveFolderID);
            this.tpOneDrive.Controls.Add(this.cbOneDriveCreateShareableLink);
            this.tpOneDrive.Controls.Add(this.oAuth2OneDrive);
            resources.ApplyResources(this.tpOneDrive, "tpOneDrive");
            this.tpOneDrive.Name = "tpOneDrive";
            this.tpOneDrive.UseVisualStyleBackColor = true;
            // 
            // tvOneDrive
            // 
            resources.ApplyResources(this.tvOneDrive, "tvOneDrive");
            this.tvOneDrive.Name = "tvOneDrive";
            this.tvOneDrive.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvOneDrive_AfterExpand);
            this.tvOneDrive.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvOneDrive_AfterSelect);
            // 
            // lblOneDriveFolderID
            // 
            resources.ApplyResources(this.lblOneDriveFolderID, "lblOneDriveFolderID");
            this.lblOneDriveFolderID.Name = "lblOneDriveFolderID";
            // 
            // cbOneDriveCreateShareableLink
            // 
            resources.ApplyResources(this.cbOneDriveCreateShareableLink, "cbOneDriveCreateShareableLink");
            this.cbOneDriveCreateShareableLink.Name = "cbOneDriveCreateShareableLink";
            this.cbOneDriveCreateShareableLink.UseVisualStyleBackColor = true;
            this.cbOneDriveCreateShareableLink.CheckedChanged += new System.EventHandler(this.cbOneDriveCreateShareableLink_CheckedChanged);
            // 
            // oAuth2OneDrive
            // 
            resources.ApplyResources(this.oAuth2OneDrive, "oAuth2OneDrive");
            this.oAuth2OneDrive.Name = "oAuth2OneDrive";
            this.oAuth2OneDrive.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oAuth2OneDrive_OpenButtonClicked);
            this.oAuth2OneDrive.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oAuth2OneDrive_CompleteButtonClicked);
            this.oAuth2OneDrive.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oAuth2OneDrive_ClearButtonClicked);
            this.oAuth2OneDrive.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oAuth2OneDrive_RefreshButtonClicked);
            // 
            // tpGoogleDrive
            // 
            this.tpGoogleDrive.Controls.Add(this.cbGoogleDriveDirectLink);
            this.tpGoogleDrive.Controls.Add(this.cbGoogleDriveUseFolder);
            this.tpGoogleDrive.Controls.Add(this.txtGoogleDriveFolderID);
            this.tpGoogleDrive.Controls.Add(this.lblGoogleDriveFolderID);
            this.tpGoogleDrive.Controls.Add(this.lvGoogleDriveFoldersList);
            this.tpGoogleDrive.Controls.Add(this.btnGoogleDriveRefreshFolders);
            this.tpGoogleDrive.Controls.Add(this.cbGoogleDriveIsPublic);
            this.tpGoogleDrive.Controls.Add(this.oauth2GoogleDrive);
            resources.ApplyResources(this.tpGoogleDrive, "tpGoogleDrive");
            this.tpGoogleDrive.Name = "tpGoogleDrive";
            this.tpGoogleDrive.UseVisualStyleBackColor = true;
            // 
            // cbGoogleDriveDirectLink
            // 
            resources.ApplyResources(this.cbGoogleDriveDirectLink, "cbGoogleDriveDirectLink");
            this.cbGoogleDriveDirectLink.Name = "cbGoogleDriveDirectLink";
            this.cbGoogleDriveDirectLink.UseVisualStyleBackColor = true;
            this.cbGoogleDriveDirectLink.CheckedChanged += new System.EventHandler(this.cbGoogleDriveDirectLink_CheckedChanged);
            // 
            // cbGoogleDriveUseFolder
            // 
            resources.ApplyResources(this.cbGoogleDriveUseFolder, "cbGoogleDriveUseFolder");
            this.cbGoogleDriveUseFolder.Name = "cbGoogleDriveUseFolder";
            this.cbGoogleDriveUseFolder.UseVisualStyleBackColor = true;
            this.cbGoogleDriveUseFolder.CheckedChanged += new System.EventHandler(this.cbGoogleDriveUseFolder_CheckedChanged);
            // 
            // txtGoogleDriveFolderID
            // 
            resources.ApplyResources(this.txtGoogleDriveFolderID, "txtGoogleDriveFolderID");
            this.txtGoogleDriveFolderID.Name = "txtGoogleDriveFolderID";
            this.txtGoogleDriveFolderID.TextChanged += new System.EventHandler(this.txtGoogleDriveFolderID_TextChanged);
            // 
            // lblGoogleDriveFolderID
            // 
            resources.ApplyResources(this.lblGoogleDriveFolderID, "lblGoogleDriveFolderID");
            this.lblGoogleDriveFolderID.Name = "lblGoogleDriveFolderID";
            // 
            // lvGoogleDriveFoldersList
            // 
            this.lvGoogleDriveFoldersList.AutoFillColumn = true;
            this.lvGoogleDriveFoldersList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chGoogleDriveTitle,
            this.chGoogleDriveDescription});
            this.lvGoogleDriveFoldersList.FullRowSelect = true;
            resources.ApplyResources(this.lvGoogleDriveFoldersList, "lvGoogleDriveFoldersList");
            this.lvGoogleDriveFoldersList.MultiSelect = false;
            this.lvGoogleDriveFoldersList.Name = "lvGoogleDriveFoldersList";
            this.lvGoogleDriveFoldersList.UseCompatibleStateImageBehavior = false;
            this.lvGoogleDriveFoldersList.View = System.Windows.Forms.View.Details;
            this.lvGoogleDriveFoldersList.SelectedIndexChanged += new System.EventHandler(this.lvGoogleDriveFoldersList_SelectedIndexChanged);
            // 
            // chGoogleDriveTitle
            // 
            resources.ApplyResources(this.chGoogleDriveTitle, "chGoogleDriveTitle");
            // 
            // chGoogleDriveDescription
            // 
            resources.ApplyResources(this.chGoogleDriveDescription, "chGoogleDriveDescription");
            // 
            // btnGoogleDriveRefreshFolders
            // 
            resources.ApplyResources(this.btnGoogleDriveRefreshFolders, "btnGoogleDriveRefreshFolders");
            this.btnGoogleDriveRefreshFolders.Name = "btnGoogleDriveRefreshFolders";
            this.btnGoogleDriveRefreshFolders.UseVisualStyleBackColor = true;
            this.btnGoogleDriveRefreshFolders.Click += new System.EventHandler(this.btnGoogleDriveRefreshFolders_Click);
            // 
            // cbGoogleDriveIsPublic
            // 
            resources.ApplyResources(this.cbGoogleDriveIsPublic, "cbGoogleDriveIsPublic");
            this.cbGoogleDriveIsPublic.Name = "cbGoogleDriveIsPublic";
            this.cbGoogleDriveIsPublic.UseVisualStyleBackColor = true;
            this.cbGoogleDriveIsPublic.CheckedChanged += new System.EventHandler(this.cbGoogleDriveIsPublic_CheckedChanged);
            // 
            // oauth2GoogleDrive
            // 
            resources.ApplyResources(this.oauth2GoogleDrive, "oauth2GoogleDrive");
            this.oauth2GoogleDrive.Name = "oauth2GoogleDrive";
            this.oauth2GoogleDrive.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2GoogleDrive_OpenButtonClicked);
            this.oauth2GoogleDrive.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2GoogleDrive_CompleteButtonClicked);
            this.oauth2GoogleDrive.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2GoogleDrive_ClearButtonClicked);
            this.oauth2GoogleDrive.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2GoogleDrive_RefreshButtonClicked);
            // 
            // tpPuush
            // 
            this.tpPuush.Controls.Add(this.pbPuush);
            this.tpPuush.Controls.Add(this.lblPuushAPIKey);
            this.tpPuush.Controls.Add(this.txtPuushAPIKey);
            this.tpPuush.Controls.Add(this.llPuushForgottenPassword);
            this.tpPuush.Controls.Add(this.btnPuushLogin);
            this.tpPuush.Controls.Add(this.txtPuushPassword);
            this.tpPuush.Controls.Add(this.txtPuushEmail);
            this.tpPuush.Controls.Add(this.lblPuushEmail);
            this.tpPuush.Controls.Add(this.lblPuushPassword);
            resources.ApplyResources(this.tpPuush, "tpPuush");
            this.tpPuush.Name = "tpPuush";
            this.tpPuush.UseVisualStyleBackColor = true;
            // 
            // pbPuush
            // 
            this.pbPuush.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbPuush.Image = global::ShareX.UploadersLib.Properties.Resources.puush_256;
            resources.ApplyResources(this.pbPuush, "pbPuush");
            this.pbPuush.Name = "pbPuush";
            this.pbPuush.TabStop = false;
            this.pbPuush.Click += new System.EventHandler(this.pbPuush_Click);
            // 
            // lblPuushAPIKey
            // 
            resources.ApplyResources(this.lblPuushAPIKey, "lblPuushAPIKey");
            this.lblPuushAPIKey.Name = "lblPuushAPIKey";
            // 
            // txtPuushAPIKey
            // 
            resources.ApplyResources(this.txtPuushAPIKey, "txtPuushAPIKey");
            this.txtPuushAPIKey.Name = "txtPuushAPIKey";
            this.txtPuushAPIKey.UseSystemPasswordChar = true;
            this.txtPuushAPIKey.TextChanged += new System.EventHandler(this.txtPuushAPIKey_TextChanged);
            // 
            // llPuushForgottenPassword
            // 
            resources.ApplyResources(this.llPuushForgottenPassword, "llPuushForgottenPassword");
            this.llPuushForgottenPassword.Name = "llPuushForgottenPassword";
            this.llPuushForgottenPassword.TabStop = true;
            this.llPuushForgottenPassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llPuushForgottenPassword_LinkClicked);
            // 
            // btnPuushLogin
            // 
            resources.ApplyResources(this.btnPuushLogin, "btnPuushLogin");
            this.btnPuushLogin.Name = "btnPuushLogin";
            this.btnPuushLogin.UseVisualStyleBackColor = true;
            this.btnPuushLogin.Click += new System.EventHandler(this.btnPuushLogin_Click);
            // 
            // txtPuushPassword
            // 
            resources.ApplyResources(this.txtPuushPassword, "txtPuushPassword");
            this.txtPuushPassword.Name = "txtPuushPassword";
            this.txtPuushPassword.UseSystemPasswordChar = true;
            // 
            // txtPuushEmail
            // 
            resources.ApplyResources(this.txtPuushEmail, "txtPuushEmail");
            this.txtPuushEmail.Name = "txtPuushEmail";
            // 
            // lblPuushEmail
            // 
            resources.ApplyResources(this.lblPuushEmail, "lblPuushEmail");
            this.lblPuushEmail.Name = "lblPuushEmail";
            // 
            // lblPuushPassword
            // 
            resources.ApplyResources(this.lblPuushPassword, "lblPuushPassword");
            this.lblPuushPassword.Name = "lblPuushPassword";
            // 
            // tpBox
            // 
            this.tpBox.Controls.Add(this.lblBoxFolderTip);
            this.tpBox.Controls.Add(this.cbBoxShare);
            this.tpBox.Controls.Add(this.lvBoxFolders);
            this.tpBox.Controls.Add(this.lblBoxFolderID);
            this.tpBox.Controls.Add(this.btnBoxRefreshFolders);
            this.tpBox.Controls.Add(this.oauth2Box);
            resources.ApplyResources(this.tpBox, "tpBox");
            this.tpBox.Name = "tpBox";
            this.tpBox.UseVisualStyleBackColor = true;
            // 
            // lblBoxFolderTip
            // 
            resources.ApplyResources(this.lblBoxFolderTip, "lblBoxFolderTip");
            this.lblBoxFolderTip.Name = "lblBoxFolderTip";
            // 
            // cbBoxShare
            // 
            resources.ApplyResources(this.cbBoxShare, "cbBoxShare");
            this.cbBoxShare.Name = "cbBoxShare";
            this.cbBoxShare.UseVisualStyleBackColor = true;
            this.cbBoxShare.CheckedChanged += new System.EventHandler(this.cbBoxShare_CheckedChanged);
            // 
            // lvBoxFolders
            // 
            this.lvBoxFolders.AutoFillColumn = true;
            this.lvBoxFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chBoxFoldersName});
            this.lvBoxFolders.FullRowSelect = true;
            resources.ApplyResources(this.lvBoxFolders, "lvBoxFolders");
            this.lvBoxFolders.Name = "lvBoxFolders";
            this.lvBoxFolders.UseCompatibleStateImageBehavior = false;
            this.lvBoxFolders.View = System.Windows.Forms.View.Details;
            this.lvBoxFolders.SelectedIndexChanged += new System.EventHandler(this.lvBoxFolders_SelectedIndexChanged);
            this.lvBoxFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvBoxFolders_MouseDoubleClick);
            // 
            // chBoxFoldersName
            // 
            resources.ApplyResources(this.chBoxFoldersName, "chBoxFoldersName");
            // 
            // lblBoxFolderID
            // 
            resources.ApplyResources(this.lblBoxFolderID, "lblBoxFolderID");
            this.lblBoxFolderID.Name = "lblBoxFolderID";
            // 
            // btnBoxRefreshFolders
            // 
            resources.ApplyResources(this.btnBoxRefreshFolders, "btnBoxRefreshFolders");
            this.btnBoxRefreshFolders.Name = "btnBoxRefreshFolders";
            this.btnBoxRefreshFolders.UseVisualStyleBackColor = true;
            this.btnBoxRefreshFolders.Click += new System.EventHandler(this.btnBoxRefreshFolders_Click);
            // 
            // oauth2Box
            // 
            resources.ApplyResources(this.oauth2Box, "oauth2Box");
            this.oauth2Box.Name = "oauth2Box";
            this.oauth2Box.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Box_OpenButtonClicked);
            this.oauth2Box.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Box_CompleteButtonClicked);
            this.oauth2Box.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Box_ClearButtonClicked);
            this.oauth2Box.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2Box_RefreshButtonClicked);
            // 
            // tpAmazonS3
            // 
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3PublicACL);
            this.tpAmazonS3.Controls.Add(this.btnAmazonS3StorageClassHelp);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3StorageClass);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3StorageClass);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3UsePathStyle);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3Endpoint);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3Endpoint);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3Region);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3Region);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3CustomDomain);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3PathPreviewLabel);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3PathPreview);
            this.tpAmazonS3.Controls.Add(this.btnAmazonS3BucketNameOpen);
            this.tpAmazonS3.Controls.Add(this.btnAmazonS3AccessKeyOpen);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3CustomCNAME);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3Endpoints);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3BucketName);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3BucketName);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3Endpoints);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3ObjectPrefix);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3ObjectPrefix);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3SecretKey);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3SecretKey);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3AccessKey);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3AccessKey);
            resources.ApplyResources(this.tpAmazonS3, "tpAmazonS3");
            this.tpAmazonS3.Name = "tpAmazonS3";
            this.tpAmazonS3.UseVisualStyleBackColor = true;
            // 
            // btnAmazonS3StorageClassHelp
            // 
            resources.ApplyResources(this.btnAmazonS3StorageClassHelp, "btnAmazonS3StorageClassHelp");
            this.btnAmazonS3StorageClassHelp.Name = "btnAmazonS3StorageClassHelp";
            this.btnAmazonS3StorageClassHelp.UseVisualStyleBackColor = true;
            this.btnAmazonS3StorageClassHelp.Click += new System.EventHandler(this.btnAmazonS3StorageClassHelp_Click);
            // 
            // lblAmazonS3StorageClass
            // 
            resources.ApplyResources(this.lblAmazonS3StorageClass, "lblAmazonS3StorageClass");
            this.lblAmazonS3StorageClass.Name = "lblAmazonS3StorageClass";
            // 
            // cbAmazonS3StorageClass
            // 
            this.cbAmazonS3StorageClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAmazonS3StorageClass.FormattingEnabled = true;
            resources.ApplyResources(this.cbAmazonS3StorageClass, "cbAmazonS3StorageClass");
            this.cbAmazonS3StorageClass.Name = "cbAmazonS3StorageClass";
            this.cbAmazonS3StorageClass.SelectedIndexChanged += new System.EventHandler(this.cbAmazonS3StorageClass_SelectedIndexChanged);
            // 
            // cbAmazonS3UsePathStyle
            // 
            resources.ApplyResources(this.cbAmazonS3UsePathStyle, "cbAmazonS3UsePathStyle");
            this.cbAmazonS3UsePathStyle.Name = "cbAmazonS3UsePathStyle";
            this.cbAmazonS3UsePathStyle.UseVisualStyleBackColor = true;
            this.cbAmazonS3UsePathStyle.CheckedChanged += new System.EventHandler(this.cbAmazonS3UsePathStyle_CheckedChanged);
            // 
            // lblAmazonS3Endpoint
            // 
            resources.ApplyResources(this.lblAmazonS3Endpoint, "lblAmazonS3Endpoint");
            this.lblAmazonS3Endpoint.Name = "lblAmazonS3Endpoint";
            // 
            // txtAmazonS3Endpoint
            // 
            resources.ApplyResources(this.txtAmazonS3Endpoint, "txtAmazonS3Endpoint");
            this.txtAmazonS3Endpoint.Name = "txtAmazonS3Endpoint";
            this.txtAmazonS3Endpoint.TextChanged += new System.EventHandler(this.txtAmazonS3Endpoint_TextChanged);
            // 
            // lblAmazonS3Region
            // 
            resources.ApplyResources(this.lblAmazonS3Region, "lblAmazonS3Region");
            this.lblAmazonS3Region.Name = "lblAmazonS3Region";
            // 
            // txtAmazonS3Region
            // 
            resources.ApplyResources(this.txtAmazonS3Region, "txtAmazonS3Region");
            this.txtAmazonS3Region.Name = "txtAmazonS3Region";
            this.txtAmazonS3Region.TextChanged += new System.EventHandler(this.txtAmazonS3Region_TextChanged);
            // 
            // txtAmazonS3CustomDomain
            // 
            resources.ApplyResources(this.txtAmazonS3CustomDomain, "txtAmazonS3CustomDomain");
            this.txtAmazonS3CustomDomain.Name = "txtAmazonS3CustomDomain";
            this.txtAmazonS3CustomDomain.TextChanged += new System.EventHandler(this.txtAmazonS3CustomDomain_TextChanged);
            // 
            // lblAmazonS3PathPreviewLabel
            // 
            resources.ApplyResources(this.lblAmazonS3PathPreviewLabel, "lblAmazonS3PathPreviewLabel");
            this.lblAmazonS3PathPreviewLabel.Name = "lblAmazonS3PathPreviewLabel";
            // 
            // lblAmazonS3PathPreview
            // 
            resources.ApplyResources(this.lblAmazonS3PathPreview, "lblAmazonS3PathPreview");
            this.lblAmazonS3PathPreview.Name = "lblAmazonS3PathPreview";
            // 
            // btnAmazonS3BucketNameOpen
            // 
            resources.ApplyResources(this.btnAmazonS3BucketNameOpen, "btnAmazonS3BucketNameOpen");
            this.btnAmazonS3BucketNameOpen.Name = "btnAmazonS3BucketNameOpen";
            this.btnAmazonS3BucketNameOpen.UseVisualStyleBackColor = true;
            this.btnAmazonS3BucketNameOpen.Click += new System.EventHandler(this.btnAmazonS3BucketNameOpen_Click);
            // 
            // btnAmazonS3AccessKeyOpen
            // 
            resources.ApplyResources(this.btnAmazonS3AccessKeyOpen, "btnAmazonS3AccessKeyOpen");
            this.btnAmazonS3AccessKeyOpen.Name = "btnAmazonS3AccessKeyOpen";
            this.btnAmazonS3AccessKeyOpen.UseVisualStyleBackColor = true;
            this.btnAmazonS3AccessKeyOpen.Click += new System.EventHandler(this.btnAmazonS3AccessKeyOpen_Click);
            // 
            // cbAmazonS3Endpoints
            // 
            this.cbAmazonS3Endpoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAmazonS3Endpoints.FormattingEnabled = true;
            resources.ApplyResources(this.cbAmazonS3Endpoints, "cbAmazonS3Endpoints");
            this.cbAmazonS3Endpoints.Name = "cbAmazonS3Endpoints";
            this.cbAmazonS3Endpoints.SelectedIndexChanged += new System.EventHandler(this.cbAmazonS3Endpoints_SelectedIndexChanged);
            // 
            // lblAmazonS3BucketName
            // 
            resources.ApplyResources(this.lblAmazonS3BucketName, "lblAmazonS3BucketName");
            this.lblAmazonS3BucketName.Name = "lblAmazonS3BucketName";
            // 
            // txtAmazonS3BucketName
            // 
            resources.ApplyResources(this.txtAmazonS3BucketName, "txtAmazonS3BucketName");
            this.txtAmazonS3BucketName.Name = "txtAmazonS3BucketName";
            this.txtAmazonS3BucketName.TextChanged += new System.EventHandler(this.txtAmazonS3BucketName_TextChanged);
            // 
            // lblAmazonS3Endpoints
            // 
            resources.ApplyResources(this.lblAmazonS3Endpoints, "lblAmazonS3Endpoints");
            this.lblAmazonS3Endpoints.Name = "lblAmazonS3Endpoints";
            // 
            // txtAmazonS3ObjectPrefix
            // 
            resources.ApplyResources(this.txtAmazonS3ObjectPrefix, "txtAmazonS3ObjectPrefix");
            this.txtAmazonS3ObjectPrefix.Name = "txtAmazonS3ObjectPrefix";
            this.txtAmazonS3ObjectPrefix.TextChanged += new System.EventHandler(this.txtAmazonS3ObjectPrefix_TextChanged);
            // 
            // lblAmazonS3ObjectPrefix
            // 
            resources.ApplyResources(this.lblAmazonS3ObjectPrefix, "lblAmazonS3ObjectPrefix");
            this.lblAmazonS3ObjectPrefix.Name = "lblAmazonS3ObjectPrefix";
            // 
            // txtAmazonS3SecretKey
            // 
            resources.ApplyResources(this.txtAmazonS3SecretKey, "txtAmazonS3SecretKey");
            this.txtAmazonS3SecretKey.Name = "txtAmazonS3SecretKey";
            this.txtAmazonS3SecretKey.UseSystemPasswordChar = true;
            this.txtAmazonS3SecretKey.TextChanged += new System.EventHandler(this.txtAmazonS3SecretKey_TextChanged);
            // 
            // lblAmazonS3SecretKey
            // 
            resources.ApplyResources(this.lblAmazonS3SecretKey, "lblAmazonS3SecretKey");
            this.lblAmazonS3SecretKey.Name = "lblAmazonS3SecretKey";
            // 
            // lblAmazonS3AccessKey
            // 
            resources.ApplyResources(this.lblAmazonS3AccessKey, "lblAmazonS3AccessKey");
            this.lblAmazonS3AccessKey.Name = "lblAmazonS3AccessKey";
            // 
            // txtAmazonS3AccessKey
            // 
            resources.ApplyResources(this.txtAmazonS3AccessKey, "txtAmazonS3AccessKey");
            this.txtAmazonS3AccessKey.Name = "txtAmazonS3AccessKey";
            this.txtAmazonS3AccessKey.TextChanged += new System.EventHandler(this.txtAmazonS3AccessKey_TextChanged);
            // 
            // tpAzureStorage
            // 
            this.tpAzureStorage.Controls.Add(this.cbAzureStorageEnvironment);
            this.tpAzureStorage.Controls.Add(this.lblAzureStorageEnvironment);
            this.tpAzureStorage.Controls.Add(this.btnAzureStoragePortal);
            this.tpAzureStorage.Controls.Add(this.txtAzureStorageContainer);
            this.tpAzureStorage.Controls.Add(this.lblAzureStorageContainer);
            this.tpAzureStorage.Controls.Add(this.txtAzureStorageAccessKey);
            this.tpAzureStorage.Controls.Add(this.lblAzureStorageAccessKey);
            this.tpAzureStorage.Controls.Add(this.txtAzureStorageAccountName);
            this.tpAzureStorage.Controls.Add(this.lblAzureStorageAccountName);
            this.tpAzureStorage.Controls.Add(this.txtAzureStorageCustomDomain);
            this.tpAzureStorage.Controls.Add(this.lblAzureStorageCustomDomain);
            resources.ApplyResources(this.tpAzureStorage, "tpAzureStorage");
            this.tpAzureStorage.Name = "tpAzureStorage";
            this.tpAzureStorage.UseVisualStyleBackColor = true;
            // 
            // cbAzureStorageEnvironment
            // 
            this.cbAzureStorageEnvironment.FormattingEnabled = true;
            this.cbAzureStorageEnvironment.Items.AddRange(new object[] {
            resources.GetString("cbAzureStorageEnvironment.Items"),
            resources.GetString("cbAzureStorageEnvironment.Items1"),
            resources.GetString("cbAzureStorageEnvironment.Items2"),
            resources.GetString("cbAzureStorageEnvironment.Items3")});
            resources.ApplyResources(this.cbAzureStorageEnvironment, "cbAzureStorageEnvironment");
            this.cbAzureStorageEnvironment.Name = "cbAzureStorageEnvironment";
            this.cbAzureStorageEnvironment.SelectedIndexChanged += new System.EventHandler(this.cbAzureStorageEnvironment_SelectedIndexChanged);
            // 
            // lblAzureStorageEnvironment
            // 
            resources.ApplyResources(this.lblAzureStorageEnvironment, "lblAzureStorageEnvironment");
            this.lblAzureStorageEnvironment.Name = "lblAzureStorageEnvironment";
            // 
            // btnAzureStoragePortal
            // 
            resources.ApplyResources(this.btnAzureStoragePortal, "btnAzureStoragePortal");
            this.btnAzureStoragePortal.Name = "btnAzureStoragePortal";
            this.btnAzureStoragePortal.UseVisualStyleBackColor = true;
            this.btnAzureStoragePortal.Click += new System.EventHandler(this.btnAzureStoragePortal_Click);
            // 
            // txtAzureStorageContainer
            // 
            resources.ApplyResources(this.txtAzureStorageContainer, "txtAzureStorageContainer");
            this.txtAzureStorageContainer.Name = "txtAzureStorageContainer";
            this.txtAzureStorageContainer.TextChanged += new System.EventHandler(this.txtAzureStorageContainer_TextChanged);
            // 
            // lblAzureStorageContainer
            // 
            resources.ApplyResources(this.lblAzureStorageContainer, "lblAzureStorageContainer");
            this.lblAzureStorageContainer.Name = "lblAzureStorageContainer";
            // 
            // txtAzureStorageAccessKey
            // 
            resources.ApplyResources(this.txtAzureStorageAccessKey, "txtAzureStorageAccessKey");
            this.txtAzureStorageAccessKey.Name = "txtAzureStorageAccessKey";
            this.txtAzureStorageAccessKey.UseSystemPasswordChar = true;
            this.txtAzureStorageAccessKey.TextChanged += new System.EventHandler(this.txtAzureStorageAccessKey_TextChanged);
            // 
            // lblAzureStorageAccessKey
            // 
            resources.ApplyResources(this.lblAzureStorageAccessKey, "lblAzureStorageAccessKey");
            this.lblAzureStorageAccessKey.Name = "lblAzureStorageAccessKey";
            // 
            // txtAzureStorageAccountName
            // 
            resources.ApplyResources(this.txtAzureStorageAccountName, "txtAzureStorageAccountName");
            this.txtAzureStorageAccountName.Name = "txtAzureStorageAccountName";
            this.txtAzureStorageAccountName.TextChanged += new System.EventHandler(this.txtAzureStorageAccountName_TextChanged);
            // 
            // lblAzureStorageAccountName
            // 
            resources.ApplyResources(this.lblAzureStorageAccountName, "lblAzureStorageAccountName");
            this.lblAzureStorageAccountName.Name = "lblAzureStorageAccountName";
            // 
            // txtAzureStorageCustomDomain
            // 
            resources.ApplyResources(this.txtAzureStorageCustomDomain, "txtAzureStorageCustomDomain");
            this.txtAzureStorageCustomDomain.Name = "txtAzureStorageCustomDomain";
            this.txtAzureStorageCustomDomain.TextChanged += new System.EventHandler(this.txtAzureStorageCustomDomain_TextChanged);
            // 
            // lblAzureStorageCustomDomain
            // 
            resources.ApplyResources(this.lblAzureStorageCustomDomain, "lblAzureStorageCustomDomain");
            this.lblAzureStorageCustomDomain.Name = "lblAzureStorageCustomDomain";
            // 
            // tpGfycat
            // 
            this.tpGfycat.Controls.Add(this.cbGfycatIsPublic);
            this.tpGfycat.Controls.Add(this.atcGfycatAccountType);
            this.tpGfycat.Controls.Add(this.oauth2Gfycat);
            resources.ApplyResources(this.tpGfycat, "tpGfycat");
            this.tpGfycat.Name = "tpGfycat";
            this.tpGfycat.UseVisualStyleBackColor = true;
            // 
            // cbGfycatIsPublic
            // 
            resources.ApplyResources(this.cbGfycatIsPublic, "cbGfycatIsPublic");
            this.cbGfycatIsPublic.Name = "cbGfycatIsPublic";
            this.cbGfycatIsPublic.UseVisualStyleBackColor = true;
            this.cbGfycatIsPublic.CheckedChanged += new System.EventHandler(this.cbGfycatIsPublic_CheckedChanged);
            // 
            // atcGfycatAccountType
            // 
            resources.ApplyResources(this.atcGfycatAccountType, "atcGfycatAccountType");
            this.atcGfycatAccountType.Name = "atcGfycatAccountType";
            this.atcGfycatAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcGfycatAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcGfycatAccountType_AccountTypeChanged);
            // 
            // oauth2Gfycat
            // 
            resources.ApplyResources(this.oauth2Gfycat, "oauth2Gfycat");
            this.oauth2Gfycat.Name = "oauth2Gfycat";
            this.oauth2Gfycat.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Gfycat_OpenButtonClicked);
            this.oauth2Gfycat.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Gfycat_CompleteButtonClicked);
            this.oauth2Gfycat.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Gfycat_ClearButtonClicked);
            this.oauth2Gfycat.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2Gfycat_RefreshButtonClicked);
            // 
            // tpMega
            // 
            this.tpMega.Controls.Add(this.btnMegaRefreshFolders);
            this.tpMega.Controls.Add(this.lblMegaStatus);
            this.tpMega.Controls.Add(this.btnMegaRegister);
            this.tpMega.Controls.Add(this.lblMegaFolder);
            this.tpMega.Controls.Add(this.lblMegaStatusTitle);
            this.tpMega.Controls.Add(this.cbMegaFolder);
            this.tpMega.Controls.Add(this.lblMegaEmail);
            this.tpMega.Controls.Add(this.btnMegaLogin);
            this.tpMega.Controls.Add(this.txtMegaEmail);
            this.tpMega.Controls.Add(this.txtMegaPassword);
            this.tpMega.Controls.Add(this.lblMegaPassword);
            resources.ApplyResources(this.tpMega, "tpMega");
            this.tpMega.Name = "tpMega";
            this.tpMega.UseVisualStyleBackColor = true;
            // 
            // btnMegaRefreshFolders
            // 
            resources.ApplyResources(this.btnMegaRefreshFolders, "btnMegaRefreshFolders");
            this.btnMegaRefreshFolders.Name = "btnMegaRefreshFolders";
            this.btnMegaRefreshFolders.UseVisualStyleBackColor = true;
            this.btnMegaRefreshFolders.Click += new System.EventHandler(this.btnMegaRefreshFolders_Click);
            // 
            // lblMegaStatus
            // 
            resources.ApplyResources(this.lblMegaStatus, "lblMegaStatus");
            this.lblMegaStatus.Name = "lblMegaStatus";
            // 
            // btnMegaRegister
            // 
            resources.ApplyResources(this.btnMegaRegister, "btnMegaRegister");
            this.btnMegaRegister.Name = "btnMegaRegister";
            this.btnMegaRegister.UseVisualStyleBackColor = true;
            this.btnMegaRegister.Click += new System.EventHandler(this.btnMegaRegister_Click);
            // 
            // lblMegaFolder
            // 
            resources.ApplyResources(this.lblMegaFolder, "lblMegaFolder");
            this.lblMegaFolder.Name = "lblMegaFolder";
            // 
            // lblMegaStatusTitle
            // 
            resources.ApplyResources(this.lblMegaStatusTitle, "lblMegaStatusTitle");
            this.lblMegaStatusTitle.Name = "lblMegaStatusTitle";
            // 
            // cbMegaFolder
            // 
            this.cbMegaFolder.DisplayMember = "DisplayName";
            this.cbMegaFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMegaFolder.FormattingEnabled = true;
            resources.ApplyResources(this.cbMegaFolder, "cbMegaFolder");
            this.cbMegaFolder.Name = "cbMegaFolder";
            this.cbMegaFolder.ValueMember = "Node";
            this.cbMegaFolder.SelectedIndexChanged += new System.EventHandler(this.cbMegaFolder_SelectedIndexChanged);
            // 
            // lblMegaEmail
            // 
            resources.ApplyResources(this.lblMegaEmail, "lblMegaEmail");
            this.lblMegaEmail.Name = "lblMegaEmail";
            // 
            // btnMegaLogin
            // 
            resources.ApplyResources(this.btnMegaLogin, "btnMegaLogin");
            this.btnMegaLogin.Name = "btnMegaLogin";
            this.btnMegaLogin.UseVisualStyleBackColor = true;
            this.btnMegaLogin.Click += new System.EventHandler(this.btnMegaLogin_Click);
            // 
            // txtMegaEmail
            // 
            resources.ApplyResources(this.txtMegaEmail, "txtMegaEmail");
            this.txtMegaEmail.Name = "txtMegaEmail";
            // 
            // txtMegaPassword
            // 
            resources.ApplyResources(this.txtMegaPassword, "txtMegaPassword");
            this.txtMegaPassword.Name = "txtMegaPassword";
            this.txtMegaPassword.UseSystemPasswordChar = true;
            // 
            // lblMegaPassword
            // 
            resources.ApplyResources(this.lblMegaPassword, "lblMegaPassword");
            this.lblMegaPassword.Name = "lblMegaPassword";
            // 
            // tpOwnCloud
            // 
            this.tpOwnCloud.Controls.Add(this.lblOwnCloudHostExample);
            this.tpOwnCloud.Controls.Add(this.cbOwnCloud81Compatibility);
            this.tpOwnCloud.Controls.Add(this.cbOwnCloudDirectLink);
            this.tpOwnCloud.Controls.Add(this.cbOwnCloudCreateShare);
            this.tpOwnCloud.Controls.Add(this.txtOwnCloudPath);
            this.tpOwnCloud.Controls.Add(this.txtOwnCloudPassword);
            this.tpOwnCloud.Controls.Add(this.txtOwnCloudUsername);
            this.tpOwnCloud.Controls.Add(this.txtOwnCloudHost);
            this.tpOwnCloud.Controls.Add(this.lblOwnCloudPath);
            this.tpOwnCloud.Controls.Add(this.lblOwnCloudPassword);
            this.tpOwnCloud.Controls.Add(this.lblOwnCloudUsername);
            this.tpOwnCloud.Controls.Add(this.lblOwnCloudHost);
            resources.ApplyResources(this.tpOwnCloud, "tpOwnCloud");
            this.tpOwnCloud.Name = "tpOwnCloud";
            this.tpOwnCloud.UseVisualStyleBackColor = true;
            // 
            // lblOwnCloudHostExample
            // 
            resources.ApplyResources(this.lblOwnCloudHostExample, "lblOwnCloudHostExample");
            this.lblOwnCloudHostExample.Name = "lblOwnCloudHostExample";
            // 
            // cbOwnCloud81Compatibility
            // 
            resources.ApplyResources(this.cbOwnCloud81Compatibility, "cbOwnCloud81Compatibility");
            this.cbOwnCloud81Compatibility.Name = "cbOwnCloud81Compatibility";
            this.cbOwnCloud81Compatibility.UseVisualStyleBackColor = true;
            this.cbOwnCloud81Compatibility.CheckedChanged += new System.EventHandler(this.cbOwnCloud81Compatibility_CheckedChanged);
            // 
            // cbOwnCloudDirectLink
            // 
            resources.ApplyResources(this.cbOwnCloudDirectLink, "cbOwnCloudDirectLink");
            this.cbOwnCloudDirectLink.Name = "cbOwnCloudDirectLink";
            this.cbOwnCloudDirectLink.UseMnemonic = false;
            this.cbOwnCloudDirectLink.UseVisualStyleBackColor = true;
            this.cbOwnCloudDirectLink.CheckedChanged += new System.EventHandler(this.cbOwnCloudDirectLink_CheckedChanged);
            // 
            // cbOwnCloudCreateShare
            // 
            resources.ApplyResources(this.cbOwnCloudCreateShare, "cbOwnCloudCreateShare");
            this.cbOwnCloudCreateShare.Name = "cbOwnCloudCreateShare";
            this.cbOwnCloudCreateShare.UseVisualStyleBackColor = true;
            this.cbOwnCloudCreateShare.CheckedChanged += new System.EventHandler(this.cbOwnCloudCreateShare_CheckedChanged);
            // 
            // txtOwnCloudPath
            // 
            resources.ApplyResources(this.txtOwnCloudPath, "txtOwnCloudPath");
            this.txtOwnCloudPath.Name = "txtOwnCloudPath";
            this.txtOwnCloudPath.TextChanged += new System.EventHandler(this.txtOwnCloudPath_TextChanged);
            // 
            // txtOwnCloudPassword
            // 
            resources.ApplyResources(this.txtOwnCloudPassword, "txtOwnCloudPassword");
            this.txtOwnCloudPassword.Name = "txtOwnCloudPassword";
            this.txtOwnCloudPassword.UseSystemPasswordChar = true;
            this.txtOwnCloudPassword.TextChanged += new System.EventHandler(this.txtOwnCloudPassword_TextChanged);
            // 
            // txtOwnCloudUsername
            // 
            resources.ApplyResources(this.txtOwnCloudUsername, "txtOwnCloudUsername");
            this.txtOwnCloudUsername.Name = "txtOwnCloudUsername";
            this.txtOwnCloudUsername.TextChanged += new System.EventHandler(this.txtOwnCloudUsername_TextChanged);
            // 
            // txtOwnCloudHost
            // 
            resources.ApplyResources(this.txtOwnCloudHost, "txtOwnCloudHost");
            this.txtOwnCloudHost.Name = "txtOwnCloudHost";
            this.txtOwnCloudHost.TextChanged += new System.EventHandler(this.txtOwnCloudHost_TextChanged);
            // 
            // lblOwnCloudPath
            // 
            resources.ApplyResources(this.lblOwnCloudPath, "lblOwnCloudPath");
            this.lblOwnCloudPath.Name = "lblOwnCloudPath";
            // 
            // lblOwnCloudPassword
            // 
            resources.ApplyResources(this.lblOwnCloudPassword, "lblOwnCloudPassword");
            this.lblOwnCloudPassword.Name = "lblOwnCloudPassword";
            // 
            // lblOwnCloudUsername
            // 
            resources.ApplyResources(this.lblOwnCloudUsername, "lblOwnCloudUsername");
            this.lblOwnCloudUsername.Name = "lblOwnCloudUsername";
            // 
            // lblOwnCloudHost
            // 
            resources.ApplyResources(this.lblOwnCloudHost, "lblOwnCloudHost");
            this.lblOwnCloudHost.Name = "lblOwnCloudHost";
            // 
            // tpMediaFire
            // 
            this.tpMediaFire.Controls.Add(this.cbMediaFireUseLongLink);
            this.tpMediaFire.Controls.Add(this.txtMediaFirePath);
            this.tpMediaFire.Controls.Add(this.lblMediaFirePath);
            this.tpMediaFire.Controls.Add(this.txtMediaFirePassword);
            this.tpMediaFire.Controls.Add(this.txtMediaFireEmail);
            this.tpMediaFire.Controls.Add(this.lblMediaFirePassword);
            this.tpMediaFire.Controls.Add(this.lblMediaFireEmail);
            resources.ApplyResources(this.tpMediaFire, "tpMediaFire");
            this.tpMediaFire.Name = "tpMediaFire";
            this.tpMediaFire.UseVisualStyleBackColor = true;
            // 
            // cbMediaFireUseLongLink
            // 
            resources.ApplyResources(this.cbMediaFireUseLongLink, "cbMediaFireUseLongLink");
            this.cbMediaFireUseLongLink.Name = "cbMediaFireUseLongLink";
            this.cbMediaFireUseLongLink.UseVisualStyleBackColor = true;
            this.cbMediaFireUseLongLink.CheckedChanged += new System.EventHandler(this.cbMediaFireUseLongLink_CheckedChanged);
            // 
            // txtMediaFirePath
            // 
            resources.ApplyResources(this.txtMediaFirePath, "txtMediaFirePath");
            this.txtMediaFirePath.Name = "txtMediaFirePath";
            this.txtMediaFirePath.TextChanged += new System.EventHandler(this.txtMediaFirePath_TextChanged);
            // 
            // lblMediaFirePath
            // 
            resources.ApplyResources(this.lblMediaFirePath, "lblMediaFirePath");
            this.lblMediaFirePath.Name = "lblMediaFirePath";
            // 
            // txtMediaFirePassword
            // 
            resources.ApplyResources(this.txtMediaFirePassword, "txtMediaFirePassword");
            this.txtMediaFirePassword.Name = "txtMediaFirePassword";
            this.txtMediaFirePassword.UseSystemPasswordChar = true;
            this.txtMediaFirePassword.TextChanged += new System.EventHandler(this.txtMediaFirePassword_TextChanged);
            // 
            // txtMediaFireEmail
            // 
            resources.ApplyResources(this.txtMediaFireEmail, "txtMediaFireEmail");
            this.txtMediaFireEmail.Name = "txtMediaFireEmail";
            this.txtMediaFireEmail.TextChanged += new System.EventHandler(this.txtMediaFireUsername_TextChanged);
            // 
            // lblMediaFirePassword
            // 
            resources.ApplyResources(this.lblMediaFirePassword, "lblMediaFirePassword");
            this.lblMediaFirePassword.Name = "lblMediaFirePassword";
            // 
            // lblMediaFireEmail
            // 
            resources.ApplyResources(this.lblMediaFireEmail, "lblMediaFireEmail");
            this.lblMediaFireEmail.Name = "lblMediaFireEmail";
            // 
            // tpPushbullet
            // 
            this.tpPushbullet.Controls.Add(this.lblPushbulletDevices);
            this.tpPushbullet.Controls.Add(this.cboPushbulletDevices);
            this.tpPushbullet.Controls.Add(this.btnPushbulletGetDeviceList);
            this.tpPushbullet.Controls.Add(this.lblPushbulletUserKey);
            this.tpPushbullet.Controls.Add(this.txtPushbulletUserKey);
            resources.ApplyResources(this.tpPushbullet, "tpPushbullet");
            this.tpPushbullet.Name = "tpPushbullet";
            this.tpPushbullet.UseVisualStyleBackColor = true;
            // 
            // lblPushbulletDevices
            // 
            resources.ApplyResources(this.lblPushbulletDevices, "lblPushbulletDevices");
            this.lblPushbulletDevices.Name = "lblPushbulletDevices";
            // 
            // cboPushbulletDevices
            // 
            this.cboPushbulletDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboPushbulletDevices, "cboPushbulletDevices");
            this.cboPushbulletDevices.FormattingEnabled = true;
            this.cboPushbulletDevices.Name = "cboPushbulletDevices";
            this.cboPushbulletDevices.SelectedIndexChanged += new System.EventHandler(this.cboPushbulletDevices_SelectedIndexChanged);
            // 
            // btnPushbulletGetDeviceList
            // 
            resources.ApplyResources(this.btnPushbulletGetDeviceList, "btnPushbulletGetDeviceList");
            this.btnPushbulletGetDeviceList.Name = "btnPushbulletGetDeviceList";
            this.btnPushbulletGetDeviceList.UseVisualStyleBackColor = true;
            this.btnPushbulletGetDeviceList.Click += new System.EventHandler(this.btnPushbulletGetDeviceList_Click);
            // 
            // lblPushbulletUserKey
            // 
            resources.ApplyResources(this.lblPushbulletUserKey, "lblPushbulletUserKey");
            this.lblPushbulletUserKey.Name = "lblPushbulletUserKey";
            // 
            // txtPushbulletUserKey
            // 
            resources.ApplyResources(this.txtPushbulletUserKey, "txtPushbulletUserKey");
            this.txtPushbulletUserKey.Name = "txtPushbulletUserKey";
            this.txtPushbulletUserKey.UseSystemPasswordChar = true;
            this.txtPushbulletUserKey.TextChanged += new System.EventHandler(this.txtPushbulletUserKey_TextChanged);
            // 
            // tpSendSpace
            // 
            this.tpSendSpace.Controls.Add(this.btnSendSpaceRegister);
            this.tpSendSpace.Controls.Add(this.lblSendSpacePassword);
            this.tpSendSpace.Controls.Add(this.lblSendSpaceUsername);
            this.tpSendSpace.Controls.Add(this.txtSendSpacePassword);
            this.tpSendSpace.Controls.Add(this.txtSendSpaceUserName);
            this.tpSendSpace.Controls.Add(this.atcSendSpaceAccountType);
            resources.ApplyResources(this.tpSendSpace, "tpSendSpace");
            this.tpSendSpace.Name = "tpSendSpace";
            this.tpSendSpace.UseVisualStyleBackColor = true;
            // 
            // btnSendSpaceRegister
            // 
            resources.ApplyResources(this.btnSendSpaceRegister, "btnSendSpaceRegister");
            this.btnSendSpaceRegister.Name = "btnSendSpaceRegister";
            this.btnSendSpaceRegister.UseVisualStyleBackColor = true;
            this.btnSendSpaceRegister.Click += new System.EventHandler(this.btnSendSpaceRegister_Click);
            // 
            // lblSendSpacePassword
            // 
            resources.ApplyResources(this.lblSendSpacePassword, "lblSendSpacePassword");
            this.lblSendSpacePassword.Name = "lblSendSpacePassword";
            // 
            // lblSendSpaceUsername
            // 
            resources.ApplyResources(this.lblSendSpaceUsername, "lblSendSpaceUsername");
            this.lblSendSpaceUsername.Name = "lblSendSpaceUsername";
            // 
            // txtSendSpacePassword
            // 
            resources.ApplyResources(this.txtSendSpacePassword, "txtSendSpacePassword");
            this.txtSendSpacePassword.Name = "txtSendSpacePassword";
            this.txtSendSpacePassword.UseSystemPasswordChar = true;
            this.txtSendSpacePassword.TextChanged += new System.EventHandler(this.txtSendSpacePassword_TextChanged);
            // 
            // txtSendSpaceUserName
            // 
            resources.ApplyResources(this.txtSendSpaceUserName, "txtSendSpaceUserName");
            this.txtSendSpaceUserName.Name = "txtSendSpaceUserName";
            this.txtSendSpaceUserName.TextChanged += new System.EventHandler(this.txtSendSpaceUserName_TextChanged);
            // 
            // atcSendSpaceAccountType
            // 
            resources.ApplyResources(this.atcSendSpaceAccountType, "atcSendSpaceAccountType");
            this.atcSendSpaceAccountType.Name = "atcSendSpaceAccountType";
            this.atcSendSpaceAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcSendSpaceAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcSendSpaceAccountType_AccountTypeChanged);
            // 
            // tpGe_tt
            // 
            this.tpGe_tt.Controls.Add(this.lblGe_ttStatus);
            this.tpGe_tt.Controls.Add(this.lblGe_ttPassword);
            this.tpGe_tt.Controls.Add(this.lblGe_ttEmail);
            this.tpGe_tt.Controls.Add(this.btnGe_ttLogin);
            this.tpGe_tt.Controls.Add(this.txtGe_ttPassword);
            this.tpGe_tt.Controls.Add(this.txtGe_ttEmail);
            resources.ApplyResources(this.tpGe_tt, "tpGe_tt");
            this.tpGe_tt.Name = "tpGe_tt";
            this.tpGe_tt.UseVisualStyleBackColor = true;
            // 
            // lblGe_ttStatus
            // 
            resources.ApplyResources(this.lblGe_ttStatus, "lblGe_ttStatus");
            this.lblGe_ttStatus.Name = "lblGe_ttStatus";
            // 
            // lblGe_ttPassword
            // 
            resources.ApplyResources(this.lblGe_ttPassword, "lblGe_ttPassword");
            this.lblGe_ttPassword.Name = "lblGe_ttPassword";
            // 
            // lblGe_ttEmail
            // 
            resources.ApplyResources(this.lblGe_ttEmail, "lblGe_ttEmail");
            this.lblGe_ttEmail.Name = "lblGe_ttEmail";
            // 
            // btnGe_ttLogin
            // 
            resources.ApplyResources(this.btnGe_ttLogin, "btnGe_ttLogin");
            this.btnGe_ttLogin.Name = "btnGe_ttLogin";
            this.btnGe_ttLogin.UseVisualStyleBackColor = true;
            this.btnGe_ttLogin.Click += new System.EventHandler(this.btnGe_ttLogin_Click);
            // 
            // txtGe_ttPassword
            // 
            resources.ApplyResources(this.txtGe_ttPassword, "txtGe_ttPassword");
            this.txtGe_ttPassword.Name = "txtGe_ttPassword";
            this.txtGe_ttPassword.UseSystemPasswordChar = true;
            // 
            // txtGe_ttEmail
            // 
            resources.ApplyResources(this.txtGe_ttEmail, "txtGe_ttEmail");
            this.txtGe_ttEmail.Name = "txtGe_ttEmail";
            // 
            // tpHostr
            // 
            this.tpHostr.Controls.Add(this.cbLocalhostrDirectURL);
            this.tpHostr.Controls.Add(this.lblLocalhostrPassword);
            this.tpHostr.Controls.Add(this.lblLocalhostrEmail);
            this.tpHostr.Controls.Add(this.txtLocalhostrPassword);
            this.tpHostr.Controls.Add(this.txtLocalhostrEmail);
            resources.ApplyResources(this.tpHostr, "tpHostr");
            this.tpHostr.Name = "tpHostr";
            this.tpHostr.UseVisualStyleBackColor = true;
            // 
            // cbLocalhostrDirectURL
            // 
            resources.ApplyResources(this.cbLocalhostrDirectURL, "cbLocalhostrDirectURL");
            this.cbLocalhostrDirectURL.Name = "cbLocalhostrDirectURL";
            this.cbLocalhostrDirectURL.UseVisualStyleBackColor = true;
            this.cbLocalhostrDirectURL.CheckedChanged += new System.EventHandler(this.cbLocalhostrDirectURL_CheckedChanged);
            // 
            // lblLocalhostrPassword
            // 
            resources.ApplyResources(this.lblLocalhostrPassword, "lblLocalhostrPassword");
            this.lblLocalhostrPassword.Name = "lblLocalhostrPassword";
            // 
            // lblLocalhostrEmail
            // 
            resources.ApplyResources(this.lblLocalhostrEmail, "lblLocalhostrEmail");
            this.lblLocalhostrEmail.Name = "lblLocalhostrEmail";
            // 
            // txtLocalhostrPassword
            // 
            resources.ApplyResources(this.txtLocalhostrPassword, "txtLocalhostrPassword");
            this.txtLocalhostrPassword.Name = "txtLocalhostrPassword";
            this.txtLocalhostrPassword.UseSystemPasswordChar = true;
            this.txtLocalhostrPassword.TextChanged += new System.EventHandler(this.txtLocalhostrPassword_TextChanged);
            // 
            // txtLocalhostrEmail
            // 
            resources.ApplyResources(this.txtLocalhostrEmail, "txtLocalhostrEmail");
            this.txtLocalhostrEmail.Name = "txtLocalhostrEmail";
            this.txtLocalhostrEmail.TextChanged += new System.EventHandler(this.txtLocalhostrEmail_TextChanged);
            // 
            // tpJira
            // 
            this.tpJira.Controls.Add(this.txtJiraIssuePrefix);
            this.tpJira.Controls.Add(this.lblJiraIssuePrefix);
            this.tpJira.Controls.Add(this.gbJiraServer);
            this.tpJira.Controls.Add(this.oAuthJira);
            resources.ApplyResources(this.tpJira, "tpJira");
            this.tpJira.Name = "tpJira";
            this.tpJira.UseVisualStyleBackColor = true;
            // 
            // txtJiraIssuePrefix
            // 
            resources.ApplyResources(this.txtJiraIssuePrefix, "txtJiraIssuePrefix");
            this.txtJiraIssuePrefix.Name = "txtJiraIssuePrefix";
            this.txtJiraIssuePrefix.TextChanged += new System.EventHandler(this.txtJiraIssuePrefix_TextChanged);
            // 
            // lblJiraIssuePrefix
            // 
            resources.ApplyResources(this.lblJiraIssuePrefix, "lblJiraIssuePrefix");
            this.lblJiraIssuePrefix.Name = "lblJiraIssuePrefix";
            // 
            // gbJiraServer
            // 
            this.gbJiraServer.Controls.Add(this.txtJiraConfigHelp);
            this.gbJiraServer.Controls.Add(this.txtJiraHost);
            this.gbJiraServer.Controls.Add(this.lblJiraHost);
            resources.ApplyResources(this.gbJiraServer, "gbJiraServer");
            this.gbJiraServer.Name = "gbJiraServer";
            this.gbJiraServer.TabStop = false;
            // 
            // txtJiraConfigHelp
            // 
            this.txtJiraConfigHelp.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.txtJiraConfigHelp, "txtJiraConfigHelp");
            this.txtJiraConfigHelp.Name = "txtJiraConfigHelp";
            this.txtJiraConfigHelp.ReadOnly = true;
            // 
            // txtJiraHost
            // 
            resources.ApplyResources(this.txtJiraHost, "txtJiraHost");
            this.txtJiraHost.Name = "txtJiraHost";
            this.txtJiraHost.TextChanged += new System.EventHandler(this.txtJiraHost_TextChanged);
            // 
            // lblJiraHost
            // 
            resources.ApplyResources(this.lblJiraHost, "lblJiraHost");
            this.lblJiraHost.Name = "lblJiraHost";
            // 
            // oAuthJira
            // 
            resources.ApplyResources(this.oAuthJira, "oAuthJira");
            this.oAuthJira.Name = "oAuthJira";
            this.oAuthJira.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oAuthJira_OpenButtonClicked);
            this.oAuthJira.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oAuthJira_CompleteButtonClicked);
            this.oAuthJira.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oAuthJira_ClearButtonClicked);
            this.oAuthJira.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oAuthJira_RefreshButtonClicked);
            // 
            // tpLambda
            // 
            this.tpLambda.Controls.Add(this.lblLambdaInfo);
            this.tpLambda.Controls.Add(this.lblLambdaApiKey);
            this.tpLambda.Controls.Add(this.txtLambdaApiKey);
            this.tpLambda.Controls.Add(this.lblLambdaUploadURL);
            this.tpLambda.Controls.Add(this.cbLambdaUploadURL);
            resources.ApplyResources(this.tpLambda, "tpLambda");
            this.tpLambda.Name = "tpLambda";
            this.tpLambda.UseVisualStyleBackColor = true;
            // 
            // lblLambdaInfo
            // 
            resources.ApplyResources(this.lblLambdaInfo, "lblLambdaInfo");
            this.lblLambdaInfo.Name = "lblLambdaInfo";
            this.lblLambdaInfo.Click += new System.EventHandler(this.lambdaInfoLabel_Click);
            // 
            // lblLambdaApiKey
            // 
            resources.ApplyResources(this.lblLambdaApiKey, "lblLambdaApiKey");
            this.lblLambdaApiKey.Name = "lblLambdaApiKey";
            // 
            // txtLambdaApiKey
            // 
            resources.ApplyResources(this.txtLambdaApiKey, "txtLambdaApiKey");
            this.txtLambdaApiKey.Name = "txtLambdaApiKey";
            this.txtLambdaApiKey.UseSystemPasswordChar = true;
            this.txtLambdaApiKey.TextChanged += new System.EventHandler(this.txtLambdaApiKey_TextChanged);
            // 
            // lblLambdaUploadURL
            // 
            resources.ApplyResources(this.lblLambdaUploadURL, "lblLambdaUploadURL");
            this.lblLambdaUploadURL.Name = "lblLambdaUploadURL";
            // 
            // cbLambdaUploadURL
            // 
            this.cbLambdaUploadURL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLambdaUploadURL.FormattingEnabled = true;
            resources.ApplyResources(this.cbLambdaUploadURL, "cbLambdaUploadURL");
            this.cbLambdaUploadURL.Name = "cbLambdaUploadURL";
            this.cbLambdaUploadURL.SelectedIndexChanged += new System.EventHandler(this.cbLambdaUploadURL_SelectedIndexChanged);
            // 
            // tpPomf
            // 
            this.tpPomf.Controls.Add(this.btnPomfTest);
            this.tpPomf.Controls.Add(this.txtPomfResultURL);
            this.tpPomf.Controls.Add(this.txtPomfUploadURL);
            this.tpPomf.Controls.Add(this.lblPomfResultURL);
            this.tpPomf.Controls.Add(this.lblPomfUploadURL);
            this.tpPomf.Controls.Add(this.lblPomfUploaders);
            this.tpPomf.Controls.Add(this.cbPomfUploaders);
            resources.ApplyResources(this.tpPomf, "tpPomf");
            this.tpPomf.Name = "tpPomf";
            this.tpPomf.UseVisualStyleBackColor = true;
            // 
            // btnPomfTest
            // 
            resources.ApplyResources(this.btnPomfTest, "btnPomfTest");
            this.btnPomfTest.Name = "btnPomfTest";
            this.btnPomfTest.UseVisualStyleBackColor = true;
            this.btnPomfTest.Click += new System.EventHandler(this.btnPomfTest_Click);
            // 
            // txtPomfResultURL
            // 
            resources.ApplyResources(this.txtPomfResultURL, "txtPomfResultURL");
            this.txtPomfResultURL.Name = "txtPomfResultURL";
            this.txtPomfResultURL.TextChanged += new System.EventHandler(this.txtPomfResultURL_TextChanged);
            // 
            // txtPomfUploadURL
            // 
            resources.ApplyResources(this.txtPomfUploadURL, "txtPomfUploadURL");
            this.txtPomfUploadURL.Name = "txtPomfUploadURL";
            this.txtPomfUploadURL.TextChanged += new System.EventHandler(this.txtPomfUploadURL_TextChanged);
            // 
            // lblPomfResultURL
            // 
            resources.ApplyResources(this.lblPomfResultURL, "lblPomfResultURL");
            this.lblPomfResultURL.Name = "lblPomfResultURL";
            // 
            // lblPomfUploadURL
            // 
            resources.ApplyResources(this.lblPomfUploadURL, "lblPomfUploadURL");
            this.lblPomfUploadURL.Name = "lblPomfUploadURL";
            // 
            // lblPomfUploaders
            // 
            resources.ApplyResources(this.lblPomfUploaders, "lblPomfUploaders");
            this.lblPomfUploaders.Name = "lblPomfUploaders";
            // 
            // cbPomfUploaders
            // 
            this.cbPomfUploaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPomfUploaders.FormattingEnabled = true;
            resources.ApplyResources(this.cbPomfUploaders, "cbPomfUploaders");
            this.cbPomfUploaders.Name = "cbPomfUploaders";
            this.cbPomfUploaders.SelectedIndexChanged += new System.EventHandler(this.cbPomfUploaders_SelectedIndexChanged);
            // 
            // tpSeafile
            // 
            this.tpSeafile.Controls.Add(this.cbSeafileAPIURL);
            this.tpSeafile.Controls.Add(this.grpSeafileShareSettings);
            this.tpSeafile.Controls.Add(this.btnSeafileLibraryPasswordValidate);
            this.tpSeafile.Controls.Add(this.txtSeafileLibraryPassword);
            this.tpSeafile.Controls.Add(this.lblSeafileLibraryPassword);
            this.tpSeafile.Controls.Add(this.lvSeafileLibraries);
            this.tpSeafile.Controls.Add(this.btnSeafilePathValidate);
            this.tpSeafile.Controls.Add(this.txtSeafileDirectoryPath);
            this.tpSeafile.Controls.Add(this.lblSeafileWritePermNotif);
            this.tpSeafile.Controls.Add(this.lblSeafilePath);
            this.tpSeafile.Controls.Add(this.txtSeafileUploadLocationRefresh);
            this.tpSeafile.Controls.Add(this.lblSeafileSelectLibrary);
            this.tpSeafile.Controls.Add(this.grpSeafileAccInfo);
            this.tpSeafile.Controls.Add(this.btnSeafileCheckAuthToken);
            this.tpSeafile.Controls.Add(this.btnSeafileCheckAPIURL);
            this.tpSeafile.Controls.Add(this.grpSeafileObtainAuthToken);
            this.tpSeafile.Controls.Add(this.cbSeafileIgnoreInvalidCert);
            this.tpSeafile.Controls.Add(this.cbSeafileCreateShareableURL);
            this.tpSeafile.Controls.Add(this.txtSeafileAuthToken);
            this.tpSeafile.Controls.Add(this.lblSeafileAuthToken);
            this.tpSeafile.Controls.Add(this.lblSeafileAPIURL);
            resources.ApplyResources(this.tpSeafile, "tpSeafile");
            this.tpSeafile.Name = "tpSeafile";
            this.tpSeafile.UseVisualStyleBackColor = true;
            // 
            // cbSeafileAPIURL
            // 
            this.cbSeafileAPIURL.FormattingEnabled = true;
            this.cbSeafileAPIURL.Items.AddRange(new object[] {
            resources.GetString("cbSeafileAPIURL.Items"),
            resources.GetString("cbSeafileAPIURL.Items1")});
            resources.ApplyResources(this.cbSeafileAPIURL, "cbSeafileAPIURL");
            this.cbSeafileAPIURL.Name = "cbSeafileAPIURL";
            this.cbSeafileAPIURL.TextChanged += new System.EventHandler(this.cbSeafileAPIURL_TextChanged);
            // 
            // grpSeafileShareSettings
            // 
            this.grpSeafileShareSettings.Controls.Add(this.txtSeafileSharePassword);
            this.grpSeafileShareSettings.Controls.Add(this.lblSeafileSharePassword);
            this.grpSeafileShareSettings.Controls.Add(this.nudSeafileExpireDays);
            this.grpSeafileShareSettings.Controls.Add(this.lblSeafileDaysToExpire);
            resources.ApplyResources(this.grpSeafileShareSettings, "grpSeafileShareSettings");
            this.grpSeafileShareSettings.Name = "grpSeafileShareSettings";
            this.grpSeafileShareSettings.TabStop = false;
            // 
            // txtSeafileSharePassword
            // 
            resources.ApplyResources(this.txtSeafileSharePassword, "txtSeafileSharePassword");
            this.txtSeafileSharePassword.Name = "txtSeafileSharePassword";
            this.txtSeafileSharePassword.UseSystemPasswordChar = true;
            this.txtSeafileSharePassword.TextChanged += new System.EventHandler(this.txtSeafileSharePassword_TextChanged);
            // 
            // lblSeafileSharePassword
            // 
            resources.ApplyResources(this.lblSeafileSharePassword, "lblSeafileSharePassword");
            this.lblSeafileSharePassword.Name = "lblSeafileSharePassword";
            // 
            // nudSeafileExpireDays
            // 
            resources.ApplyResources(this.nudSeafileExpireDays, "nudSeafileExpireDays");
            this.nudSeafileExpireDays.Maximum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.nudSeafileExpireDays.Name = "nudSeafileExpireDays";
            this.nudSeafileExpireDays.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudSeafileExpireDays.ValueChanged += new System.EventHandler(this.nudSeafileExpireDays_ValueChanged);
            // 
            // lblSeafileDaysToExpire
            // 
            resources.ApplyResources(this.lblSeafileDaysToExpire, "lblSeafileDaysToExpire");
            this.lblSeafileDaysToExpire.Name = "lblSeafileDaysToExpire";
            // 
            // btnSeafileLibraryPasswordValidate
            // 
            resources.ApplyResources(this.btnSeafileLibraryPasswordValidate, "btnSeafileLibraryPasswordValidate");
            this.btnSeafileLibraryPasswordValidate.Name = "btnSeafileLibraryPasswordValidate";
            this.btnSeafileLibraryPasswordValidate.UseVisualStyleBackColor = true;
            this.btnSeafileLibraryPasswordValidate.Click += new System.EventHandler(this.btnSeafileLibraryPasswordValidate_Click);
            // 
            // txtSeafileLibraryPassword
            // 
            resources.ApplyResources(this.txtSeafileLibraryPassword, "txtSeafileLibraryPassword");
            this.txtSeafileLibraryPassword.Name = "txtSeafileLibraryPassword";
            this.txtSeafileLibraryPassword.UseSystemPasswordChar = true;
            this.txtSeafileLibraryPassword.TextChanged += new System.EventHandler(this.txtSeafileLibraryPassword_TextChanged);
            // 
            // lblSeafileLibraryPassword
            // 
            resources.ApplyResources(this.lblSeafileLibraryPassword, "lblSeafileLibraryPassword");
            this.lblSeafileLibraryPassword.Name = "lblSeafileLibraryPassword";
            // 
            // lvSeafileLibraries
            // 
            this.lvSeafileLibraries.AllowColumnSort = true;
            this.lvSeafileLibraries.AutoFillColumn = true;
            this.lvSeafileLibraries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSeafileLibraryName,
            this.colSeafileLibrarySize,
            this.colSeafileLibraryEncrypted});
            this.lvSeafileLibraries.DisableDeselect = true;
            this.lvSeafileLibraries.FullRowSelect = true;
            this.lvSeafileLibraries.HideSelection = false;
            resources.ApplyResources(this.lvSeafileLibraries, "lvSeafileLibraries");
            this.lvSeafileLibraries.Name = "lvSeafileLibraries";
            this.lvSeafileLibraries.UseCompatibleStateImageBehavior = false;
            this.lvSeafileLibraries.View = System.Windows.Forms.View.Details;
            this.lvSeafileLibraries.SelectedIndexChanged += new System.EventHandler(this.lvSeafileLibraries_SelectedIndexChanged);
            // 
            // colSeafileLibraryName
            // 
            resources.ApplyResources(this.colSeafileLibraryName, "colSeafileLibraryName");
            // 
            // colSeafileLibrarySize
            // 
            resources.ApplyResources(this.colSeafileLibrarySize, "colSeafileLibrarySize");
            // 
            // colSeafileLibraryEncrypted
            // 
            resources.ApplyResources(this.colSeafileLibraryEncrypted, "colSeafileLibraryEncrypted");
            // 
            // btnSeafilePathValidate
            // 
            resources.ApplyResources(this.btnSeafilePathValidate, "btnSeafilePathValidate");
            this.btnSeafilePathValidate.Name = "btnSeafilePathValidate";
            this.btnSeafilePathValidate.UseVisualStyleBackColor = true;
            this.btnSeafilePathValidate.Click += new System.EventHandler(this.btnSeafilePathValidate_Click);
            // 
            // txtSeafileDirectoryPath
            // 
            resources.ApplyResources(this.txtSeafileDirectoryPath, "txtSeafileDirectoryPath");
            this.txtSeafileDirectoryPath.Name = "txtSeafileDirectoryPath";
            this.txtSeafileDirectoryPath.TextChanged += new System.EventHandler(this.txtSeafileDirectoryPath_TextChanged);
            // 
            // lblSeafileWritePermNotif
            // 
            resources.ApplyResources(this.lblSeafileWritePermNotif, "lblSeafileWritePermNotif");
            this.lblSeafileWritePermNotif.Name = "lblSeafileWritePermNotif";
            // 
            // lblSeafilePath
            // 
            resources.ApplyResources(this.lblSeafilePath, "lblSeafilePath");
            this.lblSeafilePath.Name = "lblSeafilePath";
            // 
            // txtSeafileUploadLocationRefresh
            // 
            resources.ApplyResources(this.txtSeafileUploadLocationRefresh, "txtSeafileUploadLocationRefresh");
            this.txtSeafileUploadLocationRefresh.Name = "txtSeafileUploadLocationRefresh";
            this.txtSeafileUploadLocationRefresh.UseVisualStyleBackColor = true;
            this.txtSeafileUploadLocationRefresh.Click += new System.EventHandler(this.txtSeafileUploadLocationRefresh_Click);
            // 
            // lblSeafileSelectLibrary
            // 
            resources.ApplyResources(this.lblSeafileSelectLibrary, "lblSeafileSelectLibrary");
            this.lblSeafileSelectLibrary.Name = "lblSeafileSelectLibrary";
            // 
            // grpSeafileAccInfo
            // 
            this.grpSeafileAccInfo.Controls.Add(this.btnRefreshSeafileAccInfo);
            this.grpSeafileAccInfo.Controls.Add(this.txtSeafileAccInfoUsage);
            this.grpSeafileAccInfo.Controls.Add(this.txtSeafileAccInfoEmail);
            this.grpSeafileAccInfo.Controls.Add(this.lblSeafileAccInfoEmail);
            this.grpSeafileAccInfo.Controls.Add(this.lblSeafileAccInfoUsage);
            resources.ApplyResources(this.grpSeafileAccInfo, "grpSeafileAccInfo");
            this.grpSeafileAccInfo.Name = "grpSeafileAccInfo";
            this.grpSeafileAccInfo.TabStop = false;
            // 
            // btnRefreshSeafileAccInfo
            // 
            resources.ApplyResources(this.btnRefreshSeafileAccInfo, "btnRefreshSeafileAccInfo");
            this.btnRefreshSeafileAccInfo.Name = "btnRefreshSeafileAccInfo";
            this.btnRefreshSeafileAccInfo.UseVisualStyleBackColor = true;
            this.btnRefreshSeafileAccInfo.Click += new System.EventHandler(this.btnRefreshSeafileAccInfo_Click);
            // 
            // txtSeafileAccInfoUsage
            // 
            resources.ApplyResources(this.txtSeafileAccInfoUsage, "txtSeafileAccInfoUsage");
            this.txtSeafileAccInfoUsage.Name = "txtSeafileAccInfoUsage";
            this.txtSeafileAccInfoUsage.ReadOnly = true;
            // 
            // txtSeafileAccInfoEmail
            // 
            resources.ApplyResources(this.txtSeafileAccInfoEmail, "txtSeafileAccInfoEmail");
            this.txtSeafileAccInfoEmail.Name = "txtSeafileAccInfoEmail";
            this.txtSeafileAccInfoEmail.ReadOnly = true;
            // 
            // lblSeafileAccInfoEmail
            // 
            resources.ApplyResources(this.lblSeafileAccInfoEmail, "lblSeafileAccInfoEmail");
            this.lblSeafileAccInfoEmail.Name = "lblSeafileAccInfoEmail";
            // 
            // lblSeafileAccInfoUsage
            // 
            resources.ApplyResources(this.lblSeafileAccInfoUsage, "lblSeafileAccInfoUsage");
            this.lblSeafileAccInfoUsage.Name = "lblSeafileAccInfoUsage";
            // 
            // btnSeafileCheckAuthToken
            // 
            resources.ApplyResources(this.btnSeafileCheckAuthToken, "btnSeafileCheckAuthToken");
            this.btnSeafileCheckAuthToken.Name = "btnSeafileCheckAuthToken";
            this.btnSeafileCheckAuthToken.UseVisualStyleBackColor = true;
            this.btnSeafileCheckAuthToken.Click += new System.EventHandler(this.btnSeafileCheckAuthToken_Click);
            // 
            // btnSeafileCheckAPIURL
            // 
            resources.ApplyResources(this.btnSeafileCheckAPIURL, "btnSeafileCheckAPIURL");
            this.btnSeafileCheckAPIURL.Name = "btnSeafileCheckAPIURL";
            this.btnSeafileCheckAPIURL.UseVisualStyleBackColor = true;
            this.btnSeafileCheckAPIURL.Click += new System.EventHandler(this.btnSeafileCheckAPIURL_Click);
            // 
            // grpSeafileObtainAuthToken
            // 
            this.grpSeafileObtainAuthToken.Controls.Add(this.btnSeafileGetAuthToken);
            this.grpSeafileObtainAuthToken.Controls.Add(this.txtSeafilePassword);
            this.grpSeafileObtainAuthToken.Controls.Add(this.txtSeafileUsername);
            this.grpSeafileObtainAuthToken.Controls.Add(this.lblSeafileUsername);
            this.grpSeafileObtainAuthToken.Controls.Add(this.lblSeafilePassword);
            resources.ApplyResources(this.grpSeafileObtainAuthToken, "grpSeafileObtainAuthToken");
            this.grpSeafileObtainAuthToken.Name = "grpSeafileObtainAuthToken";
            this.grpSeafileObtainAuthToken.TabStop = false;
            // 
            // btnSeafileGetAuthToken
            // 
            resources.ApplyResources(this.btnSeafileGetAuthToken, "btnSeafileGetAuthToken");
            this.btnSeafileGetAuthToken.Name = "btnSeafileGetAuthToken";
            this.btnSeafileGetAuthToken.UseVisualStyleBackColor = true;
            this.btnSeafileGetAuthToken.Click += new System.EventHandler(this.btnSeafileGetAuthToken_Click);
            // 
            // txtSeafilePassword
            // 
            resources.ApplyResources(this.txtSeafilePassword, "txtSeafilePassword");
            this.txtSeafilePassword.Name = "txtSeafilePassword";
            this.txtSeafilePassword.UseSystemPasswordChar = true;
            this.txtSeafilePassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSeafilePassword_KeyUp);
            // 
            // txtSeafileUsername
            // 
            resources.ApplyResources(this.txtSeafileUsername, "txtSeafileUsername");
            this.txtSeafileUsername.Name = "txtSeafileUsername";
            // 
            // lblSeafileUsername
            // 
            resources.ApplyResources(this.lblSeafileUsername, "lblSeafileUsername");
            this.lblSeafileUsername.Name = "lblSeafileUsername";
            // 
            // lblSeafilePassword
            // 
            resources.ApplyResources(this.lblSeafilePassword, "lblSeafilePassword");
            this.lblSeafilePassword.Name = "lblSeafilePassword";
            // 
            // cbSeafileIgnoreInvalidCert
            // 
            resources.ApplyResources(this.cbSeafileIgnoreInvalidCert, "cbSeafileIgnoreInvalidCert");
            this.cbSeafileIgnoreInvalidCert.Name = "cbSeafileIgnoreInvalidCert";
            this.cbSeafileIgnoreInvalidCert.UseVisualStyleBackColor = true;
            this.cbSeafileIgnoreInvalidCert.CheckedChanged += new System.EventHandler(this.cbSeafileIgnoreInvalidCert_CheckedChanged);
            // 
            // cbSeafileCreateShareableURL
            // 
            resources.ApplyResources(this.cbSeafileCreateShareableURL, "cbSeafileCreateShareableURL");
            this.cbSeafileCreateShareableURL.Name = "cbSeafileCreateShareableURL";
            this.cbSeafileCreateShareableURL.UseVisualStyleBackColor = true;
            this.cbSeafileCreateShareableURL.CheckedChanged += new System.EventHandler(this.cbSeafileCreateShareableURL_CheckedChanged);
            // 
            // txtSeafileAuthToken
            // 
            resources.ApplyResources(this.txtSeafileAuthToken, "txtSeafileAuthToken");
            this.txtSeafileAuthToken.Name = "txtSeafileAuthToken";
            this.txtSeafileAuthToken.TextChanged += new System.EventHandler(this.txtSeafileAuthToken_TextChanged);
            // 
            // lblSeafileAuthToken
            // 
            resources.ApplyResources(this.lblSeafileAuthToken, "lblSeafileAuthToken");
            this.lblSeafileAuthToken.Name = "lblSeafileAuthToken";
            // 
            // lblSeafileAPIURL
            // 
            resources.ApplyResources(this.lblSeafileAPIURL, "lblSeafileAPIURL");
            this.lblSeafileAPIURL.Name = "lblSeafileAPIURL";
            // 
            // tpStreamable
            // 
            this.tpStreamable.Controls.Add(this.cbStreamableUseDirectURL);
            this.tpStreamable.Controls.Add(this.txtStreamablePassword);
            this.tpStreamable.Controls.Add(this.txtStreamableUsername);
            this.tpStreamable.Controls.Add(this.lblStreamableUsername);
            this.tpStreamable.Controls.Add(this.lblStreamablePassword);
            this.tpStreamable.Controls.Add(this.cbStreamableAnonymous);
            resources.ApplyResources(this.tpStreamable, "tpStreamable");
            this.tpStreamable.Name = "tpStreamable";
            this.tpStreamable.UseVisualStyleBackColor = true;
            // 
            // cbStreamableUseDirectURL
            // 
            resources.ApplyResources(this.cbStreamableUseDirectURL, "cbStreamableUseDirectURL");
            this.cbStreamableUseDirectURL.Name = "cbStreamableUseDirectURL";
            this.cbStreamableUseDirectURL.UseVisualStyleBackColor = true;
            this.cbStreamableUseDirectURL.CheckedChanged += new System.EventHandler(this.cbStreamableUseDirectURL_CheckedChanged);
            // 
            // txtStreamablePassword
            // 
            resources.ApplyResources(this.txtStreamablePassword, "txtStreamablePassword");
            this.txtStreamablePassword.Name = "txtStreamablePassword";
            this.txtStreamablePassword.UseSystemPasswordChar = true;
            this.txtStreamablePassword.TextChanged += new System.EventHandler(this.txtStreamablePassword_TextChanged);
            // 
            // txtStreamableUsername
            // 
            resources.ApplyResources(this.txtStreamableUsername, "txtStreamableUsername");
            this.txtStreamableUsername.Name = "txtStreamableUsername";
            this.txtStreamableUsername.TextChanged += new System.EventHandler(this.txtStreamableUsername_TextChanged);
            // 
            // lblStreamableUsername
            // 
            resources.ApplyResources(this.lblStreamableUsername, "lblStreamableUsername");
            this.lblStreamableUsername.Name = "lblStreamableUsername";
            // 
            // lblStreamablePassword
            // 
            resources.ApplyResources(this.lblStreamablePassword, "lblStreamablePassword");
            this.lblStreamablePassword.Name = "lblStreamablePassword";
            // 
            // cbStreamableAnonymous
            // 
            resources.ApplyResources(this.cbStreamableAnonymous, "cbStreamableAnonymous");
            this.cbStreamableAnonymous.Name = "cbStreamableAnonymous";
            this.cbStreamableAnonymous.UseVisualStyleBackColor = true;
            this.cbStreamableAnonymous.CheckedChanged += new System.EventHandler(this.cboxStreamableAnonymous_CheckedChanged);
            // 
            // tpSul
            // 
            this.tpSul.Controls.Add(this.btnSulGetAPIKey);
            this.tpSul.Controls.Add(this.txtSulAPIKey);
            this.tpSul.Controls.Add(this.lblSulAPIKey);
            resources.ApplyResources(this.tpSul, "tpSul");
            this.tpSul.Name = "tpSul";
            this.tpSul.UseVisualStyleBackColor = true;
            // 
            // btnSulGetAPIKey
            // 
            resources.ApplyResources(this.btnSulGetAPIKey, "btnSulGetAPIKey");
            this.btnSulGetAPIKey.Name = "btnSulGetAPIKey";
            this.btnSulGetAPIKey.UseVisualStyleBackColor = true;
            this.btnSulGetAPIKey.Click += new System.EventHandler(this.btnSulGetAPIKey_Click);
            // 
            // txtSulAPIKey
            // 
            resources.ApplyResources(this.txtSulAPIKey, "txtSulAPIKey");
            this.txtSulAPIKey.Name = "txtSulAPIKey";
            this.txtSulAPIKey.UseSystemPasswordChar = true;
            this.txtSulAPIKey.TextChanged += new System.EventHandler(this.txtSulAPIKey_TextChanged);
            // 
            // lblSulAPIKey
            // 
            resources.ApplyResources(this.lblSulAPIKey, "lblSulAPIKey");
            this.lblSulAPIKey.Name = "lblSulAPIKey";
            // 
            // tpLithiio
            // 
            this.tpLithiio.Controls.Add(this.btnLithiioFetchAPIKey);
            this.tpLithiio.Controls.Add(this.txtLithiioPassword);
            this.tpLithiio.Controls.Add(this.txtLithiioEmail);
            this.tpLithiio.Controls.Add(this.lblLithiioPassword);
            this.tpLithiio.Controls.Add(this.lblLithiioEmail);
            this.tpLithiio.Controls.Add(this.btnLithiioGetAPIKey);
            this.tpLithiio.Controls.Add(this.lblLithiioApiKey);
            this.tpLithiio.Controls.Add(this.txtLithiioApiKey);
            resources.ApplyResources(this.tpLithiio, "tpLithiio");
            this.tpLithiio.Name = "tpLithiio";
            this.tpLithiio.UseVisualStyleBackColor = true;
            // 
            // btnLithiioFetchAPIKey
            // 
            resources.ApplyResources(this.btnLithiioFetchAPIKey, "btnLithiioFetchAPIKey");
            this.btnLithiioFetchAPIKey.Name = "btnLithiioFetchAPIKey";
            this.btnLithiioFetchAPIKey.UseVisualStyleBackColor = true;
            this.btnLithiioFetchAPIKey.Click += new System.EventHandler(this.btnLithiioLogin_Click);
            // 
            // txtLithiioPassword
            // 
            resources.ApplyResources(this.txtLithiioPassword, "txtLithiioPassword");
            this.txtLithiioPassword.Name = "txtLithiioPassword";
            this.txtLithiioPassword.UseSystemPasswordChar = true;
            // 
            // txtLithiioEmail
            // 
            resources.ApplyResources(this.txtLithiioEmail, "txtLithiioEmail");
            this.txtLithiioEmail.Name = "txtLithiioEmail";
            // 
            // lblLithiioPassword
            // 
            resources.ApplyResources(this.lblLithiioPassword, "lblLithiioPassword");
            this.lblLithiioPassword.Name = "lblLithiioPassword";
            // 
            // lblLithiioEmail
            // 
            resources.ApplyResources(this.lblLithiioEmail, "lblLithiioEmail");
            this.lblLithiioEmail.Name = "lblLithiioEmail";
            // 
            // btnLithiioGetAPIKey
            // 
            resources.ApplyResources(this.btnLithiioGetAPIKey, "btnLithiioGetAPIKey");
            this.btnLithiioGetAPIKey.Name = "btnLithiioGetAPIKey";
            this.btnLithiioGetAPIKey.UseVisualStyleBackColor = true;
            this.btnLithiioGetAPIKey.Click += new System.EventHandler(this.btnLithiioGetAPIKey_Click);
            // 
            // lblLithiioApiKey
            // 
            resources.ApplyResources(this.lblLithiioApiKey, "lblLithiioApiKey");
            this.lblLithiioApiKey.Name = "lblLithiioApiKey";
            // 
            // txtLithiioApiKey
            // 
            resources.ApplyResources(this.txtLithiioApiKey, "txtLithiioApiKey");
            this.txtLithiioApiKey.Name = "txtLithiioApiKey";
            this.txtLithiioApiKey.UseSystemPasswordChar = true;
            this.txtLithiioApiKey.TextChanged += new System.EventHandler(this.txtLithiioApiKey_TextChanged);
            // 
            // tpPlik
            // 
            this.tpPlik.Controls.Add(this.gbPlikSettings);
            this.tpPlik.Controls.Add(this.gbPlikLoginCredentials);
            resources.ApplyResources(this.tpPlik, "tpPlik");
            this.tpPlik.Name = "tpPlik";
            this.tpPlik.UseVisualStyleBackColor = true;
            // 
            // gbPlikSettings
            // 
            this.gbPlikSettings.Controls.Add(this.cbPlikOneShot);
            this.gbPlikSettings.Controls.Add(this.txtPlikComment);
            this.gbPlikSettings.Controls.Add(this.cbPlikComment);
            this.gbPlikSettings.Controls.Add(this.cbPlikRemovable);
            resources.ApplyResources(this.gbPlikSettings, "gbPlikSettings");
            this.gbPlikSettings.Name = "gbPlikSettings";
            this.gbPlikSettings.TabStop = false;
            // 
            // cbPlikOneShot
            // 
            resources.ApplyResources(this.cbPlikOneShot, "cbPlikOneShot");
            this.cbPlikOneShot.Name = "cbPlikOneShot";
            this.cbPlikOneShot.UseVisualStyleBackColor = true;
            this.cbPlikOneShot.CheckedChanged += new System.EventHandler(this.cbPlikOneShot_CheckedChanged);
            // 
            // txtPlikComment
            // 
            resources.ApplyResources(this.txtPlikComment, "txtPlikComment");
            this.txtPlikComment.Name = "txtPlikComment";
            this.txtPlikComment.ReadOnly = true;
            this.txtPlikComment.TextChanged += new System.EventHandler(this.txtPlikComment_TextChanged);
            // 
            // cbPlikComment
            // 
            resources.ApplyResources(this.cbPlikComment, "cbPlikComment");
            this.cbPlikComment.Name = "cbPlikComment";
            this.cbPlikComment.UseVisualStyleBackColor = true;
            this.cbPlikComment.CheckedChanged += new System.EventHandler(this.cbPlikComment_CheckedChanged);
            // 
            // cbPlikRemovable
            // 
            resources.ApplyResources(this.cbPlikRemovable, "cbPlikRemovable");
            this.cbPlikRemovable.Name = "cbPlikRemovable";
            this.cbPlikRemovable.UseVisualStyleBackColor = true;
            this.cbPlikRemovable.CheckedChanged += new System.EventHandler(this.cbPlikRemovable_CheckedChanged);
            // 
            // gbPlikLoginCredentials
            // 
            this.gbPlikLoginCredentials.Controls.Add(this.nudPlikTTL);
            this.gbPlikLoginCredentials.Controls.Add(this.cbxPlikTTLUnit);
            this.gbPlikLoginCredentials.Controls.Add(this.lblPlikTTL);
            this.gbPlikLoginCredentials.Controls.Add(this.txtPlikURL);
            this.gbPlikLoginCredentials.Controls.Add(this.lblPlikURL);
            this.gbPlikLoginCredentials.Controls.Add(this.cbPlikIsSecured);
            this.gbPlikLoginCredentials.Controls.Add(this.lblPlikAPIKey);
            this.gbPlikLoginCredentials.Controls.Add(this.txtPlikAPIKey);
            this.gbPlikLoginCredentials.Controls.Add(this.lblPlikPassword);
            this.gbPlikLoginCredentials.Controls.Add(this.lblPlikUsername);
            this.gbPlikLoginCredentials.Controls.Add(this.txtPlikPassword);
            this.gbPlikLoginCredentials.Controls.Add(this.txtPlikLogin);
            resources.ApplyResources(this.gbPlikLoginCredentials, "gbPlikLoginCredentials");
            this.gbPlikLoginCredentials.Name = "gbPlikLoginCredentials";
            this.gbPlikLoginCredentials.TabStop = false;
            // 
            // nudPlikTTL
            // 
            this.nudPlikTTL.DecimalPlaces = 2;
            resources.ApplyResources(this.nudPlikTTL, "nudPlikTTL");
            this.nudPlikTTL.Maximum = new decimal(new int[] {
            1661992960,
            1808227885,
            5,
            0});
            this.nudPlikTTL.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nudPlikTTL.Name = "nudPlikTTL";
            this.nudPlikTTL.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPlikTTL.ValueChanged += new System.EventHandler(this.nudPlikTTL_ValueChanged);
            // 
            // cbxPlikTTLUnit
            // 
            this.cbxPlikTTLUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPlikTTLUnit.FormattingEnabled = true;
            this.cbxPlikTTLUnit.Items.AddRange(new object[] {
            resources.GetString("cbxPlikTTLUnit.Items"),
            resources.GetString("cbxPlikTTLUnit.Items1"),
            resources.GetString("cbxPlikTTLUnit.Items2"),
            resources.GetString("cbxPlikTTLUnit.Items3")});
            resources.ApplyResources(this.cbxPlikTTLUnit, "cbxPlikTTLUnit");
            this.cbxPlikTTLUnit.Name = "cbxPlikTTLUnit";
            this.cbxPlikTTLUnit.SelectedIndexChanged += new System.EventHandler(this.cbxPlikTTLUnit_SelectedIndexChanged);
            // 
            // lblPlikTTL
            // 
            resources.ApplyResources(this.lblPlikTTL, "lblPlikTTL");
            this.lblPlikTTL.Name = "lblPlikTTL";
            // 
            // txtPlikURL
            // 
            resources.ApplyResources(this.txtPlikURL, "txtPlikURL");
            this.txtPlikURL.Name = "txtPlikURL";
            this.txtPlikURL.TextChanged += new System.EventHandler(this.txtPlikURL_TextChanged);
            // 
            // lblPlikURL
            // 
            resources.ApplyResources(this.lblPlikURL, "lblPlikURL");
            this.lblPlikURL.Name = "lblPlikURL";
            // 
            // cbPlikIsSecured
            // 
            resources.ApplyResources(this.cbPlikIsSecured, "cbPlikIsSecured");
            this.cbPlikIsSecured.Name = "cbPlikIsSecured";
            this.cbPlikIsSecured.UseVisualStyleBackColor = true;
            this.cbPlikIsSecured.CheckedChanged += new System.EventHandler(this.cbPlikIsSecured_CheckedChanged);
            // 
            // lblPlikAPIKey
            // 
            resources.ApplyResources(this.lblPlikAPIKey, "lblPlikAPIKey");
            this.lblPlikAPIKey.Name = "lblPlikAPIKey";
            // 
            // txtPlikAPIKey
            // 
            resources.ApplyResources(this.txtPlikAPIKey, "txtPlikAPIKey");
            this.txtPlikAPIKey.Name = "txtPlikAPIKey";
            this.txtPlikAPIKey.UseSystemPasswordChar = true;
            this.txtPlikAPIKey.TextChanged += new System.EventHandler(this.txtPlikAPIKey_TextChanged);
            // 
            // lblPlikPassword
            // 
            resources.ApplyResources(this.lblPlikPassword, "lblPlikPassword");
            this.lblPlikPassword.Name = "lblPlikPassword";
            // 
            // lblPlikUsername
            // 
            resources.ApplyResources(this.lblPlikUsername, "lblPlikUsername");
            this.lblPlikUsername.Name = "lblPlikUsername";
            // 
            // txtPlikPassword
            // 
            resources.ApplyResources(this.txtPlikPassword, "txtPlikPassword");
            this.txtPlikPassword.Name = "txtPlikPassword";
            this.txtPlikPassword.UseSystemPasswordChar = true;
            this.txtPlikPassword.TextChanged += new System.EventHandler(this.txtPlikPassword_TextChanged);
            // 
            // txtPlikLogin
            // 
            resources.ApplyResources(this.txtPlikLogin, "txtPlikLogin");
            this.txtPlikLogin.Name = "txtPlikLogin";
            this.txtPlikLogin.TextChanged += new System.EventHandler(this.txtPlikLogin_TextChanged);
            // 
            // tpSharedFolder
            // 
            this.tpSharedFolder.Controls.Add(this.lblSharedFolderFiles);
            this.tpSharedFolder.Controls.Add(this.lblSharedFolderText);
            this.tpSharedFolder.Controls.Add(this.cboSharedFolderFiles);
            this.tpSharedFolder.Controls.Add(this.lblSharedFolderImages);
            this.tpSharedFolder.Controls.Add(this.cboSharedFolderText);
            this.tpSharedFolder.Controls.Add(this.cboSharedFolderImages);
            this.tpSharedFolder.Controls.Add(this.ucLocalhostAccounts);
            resources.ApplyResources(this.tpSharedFolder, "tpSharedFolder");
            this.tpSharedFolder.Name = "tpSharedFolder";
            this.tpSharedFolder.UseVisualStyleBackColor = true;
            // 
            // lblSharedFolderFiles
            // 
            resources.ApplyResources(this.lblSharedFolderFiles, "lblSharedFolderFiles");
            this.lblSharedFolderFiles.Name = "lblSharedFolderFiles";
            // 
            // lblSharedFolderText
            // 
            resources.ApplyResources(this.lblSharedFolderText, "lblSharedFolderText");
            this.lblSharedFolderText.Name = "lblSharedFolderText";
            // 
            // cboSharedFolderFiles
            // 
            this.cboSharedFolderFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSharedFolderFiles.FormattingEnabled = true;
            resources.ApplyResources(this.cboSharedFolderFiles, "cboSharedFolderFiles");
            this.cboSharedFolderFiles.Name = "cboSharedFolderFiles";
            this.cboSharedFolderFiles.SelectedIndexChanged += new System.EventHandler(this.cboSharedFolderFiles_SelectedIndexChanged);
            // 
            // lblSharedFolderImages
            // 
            resources.ApplyResources(this.lblSharedFolderImages, "lblSharedFolderImages");
            this.lblSharedFolderImages.Name = "lblSharedFolderImages";
            // 
            // cboSharedFolderText
            // 
            this.cboSharedFolderText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSharedFolderText.FormattingEnabled = true;
            resources.ApplyResources(this.cboSharedFolderText, "cboSharedFolderText");
            this.cboSharedFolderText.Name = "cboSharedFolderText";
            this.cboSharedFolderText.SelectedIndexChanged += new System.EventHandler(this.cboSharedFolderText_SelectedIndexChanged);
            // 
            // cboSharedFolderImages
            // 
            this.cboSharedFolderImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSharedFolderImages.FormattingEnabled = true;
            resources.ApplyResources(this.cboSharedFolderImages, "cboSharedFolderImages");
            this.cboSharedFolderImages.Name = "cboSharedFolderImages";
            this.cboSharedFolderImages.SelectedIndexChanged += new System.EventHandler(this.cboSharedFolderImages_SelectedIndexChanged);
            // 
            // ucLocalhostAccounts
            // 
            resources.ApplyResources(this.ucLocalhostAccounts, "ucLocalhostAccounts");
            this.ucLocalhostAccounts.Name = "ucLocalhostAccounts";
            // 
            // tpEmail
            // 
            this.tpEmail.Controls.Add(this.txtEmailAutomaticSendTo);
            this.tpEmail.Controls.Add(this.cbEmailAutomaticSend);
            this.tpEmail.Controls.Add(this.lblEmailSmtpServer);
            this.tpEmail.Controls.Add(this.lblEmailPassword);
            this.tpEmail.Controls.Add(this.cbEmailRememberLastTo);
            this.tpEmail.Controls.Add(this.txtEmailFrom);
            this.tpEmail.Controls.Add(this.txtEmailPassword);
            this.tpEmail.Controls.Add(this.txtEmailDefaultBody);
            this.tpEmail.Controls.Add(this.lblEmailFrom);
            this.tpEmail.Controls.Add(this.txtEmailSmtpServer);
            this.tpEmail.Controls.Add(this.lblEmailDefaultSubject);
            this.tpEmail.Controls.Add(this.lblEmailDefaultBody);
            this.tpEmail.Controls.Add(this.nudEmailSmtpPort);
            this.tpEmail.Controls.Add(this.lblEmailSmtpPort);
            this.tpEmail.Controls.Add(this.txtEmailDefaultSubject);
            resources.ApplyResources(this.tpEmail, "tpEmail");
            this.tpEmail.Name = "tpEmail";
            this.tpEmail.UseVisualStyleBackColor = true;
            // 
            // txtEmailAutomaticSendTo
            // 
            resources.ApplyResources(this.txtEmailAutomaticSendTo, "txtEmailAutomaticSendTo");
            this.txtEmailAutomaticSendTo.Name = "txtEmailAutomaticSendTo";
            this.txtEmailAutomaticSendTo.TextChanged += new System.EventHandler(this.txtEmailAutomaticSendTo_TextChanged);
            // 
            // cbEmailAutomaticSend
            // 
            resources.ApplyResources(this.cbEmailAutomaticSend, "cbEmailAutomaticSend");
            this.cbEmailAutomaticSend.Name = "cbEmailAutomaticSend";
            this.cbEmailAutomaticSend.UseVisualStyleBackColor = true;
            this.cbEmailAutomaticSend.CheckedChanged += new System.EventHandler(this.cbEmailAutomaticSend_CheckedChanged);
            // 
            // lblEmailSmtpServer
            // 
            resources.ApplyResources(this.lblEmailSmtpServer, "lblEmailSmtpServer");
            this.lblEmailSmtpServer.Name = "lblEmailSmtpServer";
            // 
            // lblEmailPassword
            // 
            resources.ApplyResources(this.lblEmailPassword, "lblEmailPassword");
            this.lblEmailPassword.Name = "lblEmailPassword";
            // 
            // cbEmailRememberLastTo
            // 
            resources.ApplyResources(this.cbEmailRememberLastTo, "cbEmailRememberLastTo");
            this.cbEmailRememberLastTo.Name = "cbEmailRememberLastTo";
            this.cbEmailRememberLastTo.UseVisualStyleBackColor = true;
            this.cbEmailRememberLastTo.CheckedChanged += new System.EventHandler(this.cbRememberLastToEmail_CheckedChanged);
            // 
            // txtEmailFrom
            // 
            resources.ApplyResources(this.txtEmailFrom, "txtEmailFrom");
            this.txtEmailFrom.Name = "txtEmailFrom";
            this.txtEmailFrom.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            // 
            // txtEmailPassword
            // 
            resources.ApplyResources(this.txtEmailPassword, "txtEmailPassword");
            this.txtEmailPassword.Name = "txtEmailPassword";
            this.txtEmailPassword.UseSystemPasswordChar = true;
            this.txtEmailPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // txtEmailDefaultBody
            // 
            resources.ApplyResources(this.txtEmailDefaultBody, "txtEmailDefaultBody");
            this.txtEmailDefaultBody.Name = "txtEmailDefaultBody";
            this.txtEmailDefaultBody.TextChanged += new System.EventHandler(this.txtDefaultBody_TextChanged);
            // 
            // lblEmailFrom
            // 
            resources.ApplyResources(this.lblEmailFrom, "lblEmailFrom");
            this.lblEmailFrom.Name = "lblEmailFrom";
            // 
            // txtEmailSmtpServer
            // 
            resources.ApplyResources(this.txtEmailSmtpServer, "txtEmailSmtpServer");
            this.txtEmailSmtpServer.Name = "txtEmailSmtpServer";
            this.txtEmailSmtpServer.TextChanged += new System.EventHandler(this.txtSmtpServer_TextChanged);
            // 
            // lblEmailDefaultSubject
            // 
            resources.ApplyResources(this.lblEmailDefaultSubject, "lblEmailDefaultSubject");
            this.lblEmailDefaultSubject.Name = "lblEmailDefaultSubject";
            // 
            // lblEmailDefaultBody
            // 
            resources.ApplyResources(this.lblEmailDefaultBody, "lblEmailDefaultBody");
            this.lblEmailDefaultBody.Name = "lblEmailDefaultBody";
            // 
            // nudEmailSmtpPort
            // 
            resources.ApplyResources(this.nudEmailSmtpPort, "nudEmailSmtpPort");
            this.nudEmailSmtpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudEmailSmtpPort.Name = "nudEmailSmtpPort";
            this.nudEmailSmtpPort.Value = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudEmailSmtpPort.ValueChanged += new System.EventHandler(this.nudSmtpPort_ValueChanged);
            // 
            // lblEmailSmtpPort
            // 
            resources.ApplyResources(this.lblEmailSmtpPort, "lblEmailSmtpPort");
            this.lblEmailSmtpPort.Name = "lblEmailSmtpPort";
            // 
            // txtEmailDefaultSubject
            // 
            resources.ApplyResources(this.txtEmailDefaultSubject, "txtEmailDefaultSubject");
            this.txtEmailDefaultSubject.Name = "txtEmailDefaultSubject";
            this.txtEmailDefaultSubject.TextChanged += new System.EventHandler(this.txtDefaultSubject_TextChanged);
            // 
            // btnCopyShowFiles
            // 
            resources.ApplyResources(this.btnCopyShowFiles, "btnCopyShowFiles");
            this.btnCopyShowFiles.Name = "btnCopyShowFiles";
            // 
            // tpTextUploaders
            // 
            this.tpTextUploaders.Controls.Add(this.tcTextUploaders);
            resources.ApplyResources(this.tpTextUploaders, "tpTextUploaders");
            this.tpTextUploaders.Name = "tpTextUploaders";
            this.tpTextUploaders.UseVisualStyleBackColor = true;
            // 
            // tcTextUploaders
            // 
            this.tcTextUploaders.Controls.Add(this.tpPastebin);
            this.tcTextUploaders.Controls.Add(this.tpPaste_ee);
            this.tcTextUploaders.Controls.Add(this.tpGist);
            this.tcTextUploaders.Controls.Add(this.tpUpaste);
            this.tcTextUploaders.Controls.Add(this.tpHastebin);
            this.tcTextUploaders.Controls.Add(this.tpOneTimeSecret);
            this.tcTextUploaders.Controls.Add(this.tpPastie);
            resources.ApplyResources(this.tcTextUploaders, "tcTextUploaders");
            this.tcTextUploaders.Name = "tcTextUploaders";
            this.tcTextUploaders.SelectedIndex = 0;
            // 
            // tpPastebin
            // 
            this.tpPastebin.Controls.Add(this.cbPastebinRaw);
            this.tpPastebin.Controls.Add(this.cbPastebinSyntax);
            this.tpPastebin.Controls.Add(this.btnPastebinRegister);
            this.tpPastebin.Controls.Add(this.lblPastebinSyntax);
            this.tpPastebin.Controls.Add(this.lblPastebinExpiration);
            this.tpPastebin.Controls.Add(this.lblPastebinPrivacy);
            this.tpPastebin.Controls.Add(this.lblPastebinTitle);
            this.tpPastebin.Controls.Add(this.lblPastebinPassword);
            this.tpPastebin.Controls.Add(this.lblPastebinUsername);
            this.tpPastebin.Controls.Add(this.cbPastebinExpiration);
            this.tpPastebin.Controls.Add(this.cbPastebinPrivacy);
            this.tpPastebin.Controls.Add(this.txtPastebinTitle);
            this.tpPastebin.Controls.Add(this.txtPastebinPassword);
            this.tpPastebin.Controls.Add(this.txtPastebinUsername);
            this.tpPastebin.Controls.Add(this.lblPastebinLoginStatus);
            this.tpPastebin.Controls.Add(this.btnPastebinLogin);
            resources.ApplyResources(this.tpPastebin, "tpPastebin");
            this.tpPastebin.Name = "tpPastebin";
            this.tpPastebin.UseVisualStyleBackColor = true;
            // 
            // cbPastebinRaw
            // 
            resources.ApplyResources(this.cbPastebinRaw, "cbPastebinRaw");
            this.cbPastebinRaw.Name = "cbPastebinRaw";
            this.cbPastebinRaw.UseVisualStyleBackColor = true;
            this.cbPastebinRaw.CheckedChanged += new System.EventHandler(this.cbPastebinRaw_CheckedChanged);
            // 
            // cbPastebinSyntax
            // 
            this.cbPastebinSyntax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPastebinSyntax.FormattingEnabled = true;
            resources.ApplyResources(this.cbPastebinSyntax, "cbPastebinSyntax");
            this.cbPastebinSyntax.Name = "cbPastebinSyntax";
            this.cbPastebinSyntax.SelectedIndexChanged += new System.EventHandler(this.cbPastebinSyntax_SelectedIndexChanged);
            // 
            // btnPastebinRegister
            // 
            resources.ApplyResources(this.btnPastebinRegister, "btnPastebinRegister");
            this.btnPastebinRegister.Name = "btnPastebinRegister";
            this.btnPastebinRegister.UseVisualStyleBackColor = true;
            this.btnPastebinRegister.Click += new System.EventHandler(this.btnPastebinRegister_Click);
            // 
            // lblPastebinSyntax
            // 
            resources.ApplyResources(this.lblPastebinSyntax, "lblPastebinSyntax");
            this.lblPastebinSyntax.Name = "lblPastebinSyntax";
            // 
            // lblPastebinExpiration
            // 
            resources.ApplyResources(this.lblPastebinExpiration, "lblPastebinExpiration");
            this.lblPastebinExpiration.Name = "lblPastebinExpiration";
            // 
            // lblPastebinPrivacy
            // 
            resources.ApplyResources(this.lblPastebinPrivacy, "lblPastebinPrivacy");
            this.lblPastebinPrivacy.Name = "lblPastebinPrivacy";
            // 
            // lblPastebinTitle
            // 
            resources.ApplyResources(this.lblPastebinTitle, "lblPastebinTitle");
            this.lblPastebinTitle.Name = "lblPastebinTitle";
            // 
            // lblPastebinPassword
            // 
            resources.ApplyResources(this.lblPastebinPassword, "lblPastebinPassword");
            this.lblPastebinPassword.Name = "lblPastebinPassword";
            // 
            // lblPastebinUsername
            // 
            resources.ApplyResources(this.lblPastebinUsername, "lblPastebinUsername");
            this.lblPastebinUsername.Name = "lblPastebinUsername";
            // 
            // cbPastebinExpiration
            // 
            this.cbPastebinExpiration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPastebinExpiration.FormattingEnabled = true;
            resources.ApplyResources(this.cbPastebinExpiration, "cbPastebinExpiration");
            this.cbPastebinExpiration.Name = "cbPastebinExpiration";
            this.cbPastebinExpiration.SelectedIndexChanged += new System.EventHandler(this.cbPastebinExpiration_SelectedIndexChanged);
            // 
            // cbPastebinPrivacy
            // 
            this.cbPastebinPrivacy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPastebinPrivacy.FormattingEnabled = true;
            resources.ApplyResources(this.cbPastebinPrivacy, "cbPastebinPrivacy");
            this.cbPastebinPrivacy.Name = "cbPastebinPrivacy";
            this.cbPastebinPrivacy.SelectedIndexChanged += new System.EventHandler(this.cbPastebinPrivacy_SelectedIndexChanged);
            // 
            // txtPastebinTitle
            // 
            resources.ApplyResources(this.txtPastebinTitle, "txtPastebinTitle");
            this.txtPastebinTitle.Name = "txtPastebinTitle";
            this.txtPastebinTitle.TextChanged += new System.EventHandler(this.txtPastebinTitle_TextChanged);
            // 
            // txtPastebinPassword
            // 
            resources.ApplyResources(this.txtPastebinPassword, "txtPastebinPassword");
            this.txtPastebinPassword.Name = "txtPastebinPassword";
            this.txtPastebinPassword.UseSystemPasswordChar = true;
            this.txtPastebinPassword.TextChanged += new System.EventHandler(this.txtPastebinPassword_TextChanged);
            // 
            // txtPastebinUsername
            // 
            resources.ApplyResources(this.txtPastebinUsername, "txtPastebinUsername");
            this.txtPastebinUsername.Name = "txtPastebinUsername";
            this.txtPastebinUsername.TextChanged += new System.EventHandler(this.txtPastebinUsername_TextChanged);
            // 
            // lblPastebinLoginStatus
            // 
            resources.ApplyResources(this.lblPastebinLoginStatus, "lblPastebinLoginStatus");
            this.lblPastebinLoginStatus.Name = "lblPastebinLoginStatus";
            // 
            // btnPastebinLogin
            // 
            resources.ApplyResources(this.btnPastebinLogin, "btnPastebinLogin");
            this.btnPastebinLogin.Name = "btnPastebinLogin";
            this.btnPastebinLogin.UseVisualStyleBackColor = true;
            this.btnPastebinLogin.Click += new System.EventHandler(this.btnPastebinLogin_Click);
            // 
            // tpPaste_ee
            // 
            this.tpPaste_ee.Controls.Add(this.btnPaste_eeGetUserKey);
            this.tpPaste_ee.Controls.Add(this.lblPaste_eeUserAPIKey);
            this.tpPaste_ee.Controls.Add(this.txtPaste_eeUserAPIKey);
            resources.ApplyResources(this.tpPaste_ee, "tpPaste_ee");
            this.tpPaste_ee.Name = "tpPaste_ee";
            this.tpPaste_ee.UseVisualStyleBackColor = true;
            // 
            // btnPaste_eeGetUserKey
            // 
            resources.ApplyResources(this.btnPaste_eeGetUserKey, "btnPaste_eeGetUserKey");
            this.btnPaste_eeGetUserKey.Name = "btnPaste_eeGetUserKey";
            this.btnPaste_eeGetUserKey.UseVisualStyleBackColor = true;
            this.btnPaste_eeGetUserKey.Click += new System.EventHandler(this.btnPaste_eeGetUserKey_Click);
            // 
            // lblPaste_eeUserAPIKey
            // 
            resources.ApplyResources(this.lblPaste_eeUserAPIKey, "lblPaste_eeUserAPIKey");
            this.lblPaste_eeUserAPIKey.Name = "lblPaste_eeUserAPIKey";
            // 
            // txtPaste_eeUserAPIKey
            // 
            resources.ApplyResources(this.txtPaste_eeUserAPIKey, "txtPaste_eeUserAPIKey");
            this.txtPaste_eeUserAPIKey.Name = "txtPaste_eeUserAPIKey";
            this.txtPaste_eeUserAPIKey.UseSystemPasswordChar = true;
            this.txtPaste_eeUserAPIKey.TextChanged += new System.EventHandler(this.txtPaste_eeUserAPIKey_TextChanged);
            // 
            // tpGist
            // 
            this.tpGist.Controls.Add(this.lblGistCustomURLExample);
            this.tpGist.Controls.Add(this.lblGistOAuthInfo);
            this.tpGist.Controls.Add(this.lblGistCustomURL);
            this.tpGist.Controls.Add(this.txtGistCustomURL);
            this.tpGist.Controls.Add(this.cbGistUseRawURL);
            this.tpGist.Controls.Add(this.cbGistPublishPublic);
            this.tpGist.Controls.Add(this.oAuth2Gist);
            this.tpGist.Controls.Add(this.atcGistAccountType);
            resources.ApplyResources(this.tpGist, "tpGist");
            this.tpGist.Name = "tpGist";
            this.tpGist.UseVisualStyleBackColor = true;
            // 
            // lblGistCustomURLExample
            // 
            resources.ApplyResources(this.lblGistCustomURLExample, "lblGistCustomURLExample");
            this.lblGistCustomURLExample.Name = "lblGistCustomURLExample";
            // 
            // lblGistOAuthInfo
            // 
            resources.ApplyResources(this.lblGistOAuthInfo, "lblGistOAuthInfo");
            this.lblGistOAuthInfo.Name = "lblGistOAuthInfo";
            // 
            // lblGistCustomURL
            // 
            resources.ApplyResources(this.lblGistCustomURL, "lblGistCustomURL");
            this.lblGistCustomURL.Name = "lblGistCustomURL";
            // 
            // txtGistCustomURL
            // 
            resources.ApplyResources(this.txtGistCustomURL, "txtGistCustomURL");
            this.txtGistCustomURL.Name = "txtGistCustomURL";
            this.txtGistCustomURL.TextChanged += new System.EventHandler(this.txtGistCustomURL_TextChanged);
            // 
            // cbGistUseRawURL
            // 
            resources.ApplyResources(this.cbGistUseRawURL, "cbGistUseRawURL");
            this.cbGistUseRawURL.Name = "cbGistUseRawURL";
            this.cbGistUseRawURL.UseVisualStyleBackColor = true;
            this.cbGistUseRawURL.CheckedChanged += new System.EventHandler(this.cbGistUseRawURL_CheckedChanged);
            // 
            // cbGistPublishPublic
            // 
            resources.ApplyResources(this.cbGistPublishPublic, "cbGistPublishPublic");
            this.cbGistPublishPublic.Name = "cbGistPublishPublic";
            this.cbGistPublishPublic.UseVisualStyleBackColor = true;
            this.cbGistPublishPublic.CheckedChanged += new System.EventHandler(this.chkGistPublishPublic_CheckedChanged);
            // 
            // oAuth2Gist
            // 
            resources.ApplyResources(this.oAuth2Gist, "oAuth2Gist");
            this.oAuth2Gist.IsRefreshable = false;
            this.oAuth2Gist.Name = "oAuth2Gist";
            this.oAuth2Gist.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oAuth2Gist_OpenButtonClicked);
            this.oAuth2Gist.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oAuth2Gist_CompleteButtonClicked);
            this.oAuth2Gist.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oAuth2Gist_ClearButtonClicked);
            // 
            // atcGistAccountType
            // 
            resources.ApplyResources(this.atcGistAccountType, "atcGistAccountType");
            this.atcGistAccountType.Name = "atcGistAccountType";
            this.atcGistAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcGistAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcGistAccountType_AccountTypeChanged);
            // 
            // tpUpaste
            // 
            this.tpUpaste.Controls.Add(this.cbUpasteIsPublic);
            this.tpUpaste.Controls.Add(this.lblUpasteUserKey);
            this.tpUpaste.Controls.Add(this.txtUpasteUserKey);
            resources.ApplyResources(this.tpUpaste, "tpUpaste");
            this.tpUpaste.Name = "tpUpaste";
            this.tpUpaste.UseVisualStyleBackColor = true;
            // 
            // cbUpasteIsPublic
            // 
            resources.ApplyResources(this.cbUpasteIsPublic, "cbUpasteIsPublic");
            this.cbUpasteIsPublic.Name = "cbUpasteIsPublic";
            this.cbUpasteIsPublic.UseVisualStyleBackColor = true;
            this.cbUpasteIsPublic.CheckedChanged += new System.EventHandler(this.cbUpasteIsPublic_CheckedChanged);
            // 
            // lblUpasteUserKey
            // 
            resources.ApplyResources(this.lblUpasteUserKey, "lblUpasteUserKey");
            this.lblUpasteUserKey.Name = "lblUpasteUserKey";
            // 
            // txtUpasteUserKey
            // 
            resources.ApplyResources(this.txtUpasteUserKey, "txtUpasteUserKey");
            this.txtUpasteUserKey.Name = "txtUpasteUserKey";
            this.txtUpasteUserKey.UseSystemPasswordChar = true;
            this.txtUpasteUserKey.TextChanged += new System.EventHandler(this.txtUpasteUserKey_TextChanged);
            // 
            // tpHastebin
            // 
            this.tpHastebin.Controls.Add(this.cbHastebinUseFileExtension);
            this.tpHastebin.Controls.Add(this.txtHastebinSyntaxHighlighting);
            this.tpHastebin.Controls.Add(this.txtHastebinCustomDomain);
            this.tpHastebin.Controls.Add(this.lblHastebinSyntaxHighlighting);
            this.tpHastebin.Controls.Add(this.lblHastebinCustomDomain);
            resources.ApplyResources(this.tpHastebin, "tpHastebin");
            this.tpHastebin.Name = "tpHastebin";
            this.tpHastebin.UseVisualStyleBackColor = true;
            // 
            // cbHastebinUseFileExtension
            // 
            resources.ApplyResources(this.cbHastebinUseFileExtension, "cbHastebinUseFileExtension");
            this.cbHastebinUseFileExtension.Name = "cbHastebinUseFileExtension";
            this.cbHastebinUseFileExtension.UseVisualStyleBackColor = true;
            this.cbHastebinUseFileExtension.CheckedChanged += new System.EventHandler(this.cbHastebinUseFileExtension_CheckedChanged);
            // 
            // txtHastebinSyntaxHighlighting
            // 
            resources.ApplyResources(this.txtHastebinSyntaxHighlighting, "txtHastebinSyntaxHighlighting");
            this.txtHastebinSyntaxHighlighting.Name = "txtHastebinSyntaxHighlighting";
            this.txtHastebinSyntaxHighlighting.TextChanged += new System.EventHandler(this.txtHastebinSyntaxHighlighting_TextChanged);
            // 
            // txtHastebinCustomDomain
            // 
            resources.ApplyResources(this.txtHastebinCustomDomain, "txtHastebinCustomDomain");
            this.txtHastebinCustomDomain.Name = "txtHastebinCustomDomain";
            this.txtHastebinCustomDomain.TextChanged += new System.EventHandler(this.txtHastebinCustomDomain_TextChanged);
            // 
            // lblHastebinSyntaxHighlighting
            // 
            resources.ApplyResources(this.lblHastebinSyntaxHighlighting, "lblHastebinSyntaxHighlighting");
            this.lblHastebinSyntaxHighlighting.Name = "lblHastebinSyntaxHighlighting";
            // 
            // lblHastebinCustomDomain
            // 
            resources.ApplyResources(this.lblHastebinCustomDomain, "lblHastebinCustomDomain");
            this.lblHastebinCustomDomain.Name = "lblHastebinCustomDomain";
            // 
            // tpOneTimeSecret
            // 
            this.tpOneTimeSecret.Controls.Add(this.lblOneTimeSecretAPIKey);
            this.tpOneTimeSecret.Controls.Add(this.lblOneTimeSecretEmail);
            this.tpOneTimeSecret.Controls.Add(this.txtOneTimeSecretAPIKey);
            this.tpOneTimeSecret.Controls.Add(this.txtOneTimeSecretEmail);
            resources.ApplyResources(this.tpOneTimeSecret, "tpOneTimeSecret");
            this.tpOneTimeSecret.Name = "tpOneTimeSecret";
            this.tpOneTimeSecret.UseVisualStyleBackColor = true;
            // 
            // lblOneTimeSecretAPIKey
            // 
            resources.ApplyResources(this.lblOneTimeSecretAPIKey, "lblOneTimeSecretAPIKey");
            this.lblOneTimeSecretAPIKey.Name = "lblOneTimeSecretAPIKey";
            // 
            // lblOneTimeSecretEmail
            // 
            resources.ApplyResources(this.lblOneTimeSecretEmail, "lblOneTimeSecretEmail");
            this.lblOneTimeSecretEmail.Name = "lblOneTimeSecretEmail";
            // 
            // txtOneTimeSecretAPIKey
            // 
            resources.ApplyResources(this.txtOneTimeSecretAPIKey, "txtOneTimeSecretAPIKey");
            this.txtOneTimeSecretAPIKey.Name = "txtOneTimeSecretAPIKey";
            this.txtOneTimeSecretAPIKey.UseSystemPasswordChar = true;
            this.txtOneTimeSecretAPIKey.TextChanged += new System.EventHandler(this.txtOneTimeSecretAPIKey_TextChanged);
            // 
            // txtOneTimeSecretEmail
            // 
            resources.ApplyResources(this.txtOneTimeSecretEmail, "txtOneTimeSecretEmail");
            this.txtOneTimeSecretEmail.Name = "txtOneTimeSecretEmail";
            this.txtOneTimeSecretEmail.TextChanged += new System.EventHandler(this.txtOneTimeSecretEmail_TextChanged);
            // 
            // tpPastie
            // 
            this.tpPastie.Controls.Add(this.cbPastieIsPublic);
            resources.ApplyResources(this.tpPastie, "tpPastie");
            this.tpPastie.Name = "tpPastie";
            this.tpPastie.UseVisualStyleBackColor = true;
            // 
            // cbPastieIsPublic
            // 
            resources.ApplyResources(this.cbPastieIsPublic, "cbPastieIsPublic");
            this.cbPastieIsPublic.Name = "cbPastieIsPublic";
            this.cbPastieIsPublic.UseVisualStyleBackColor = true;
            this.cbPastieIsPublic.CheckedChanged += new System.EventHandler(this.cbPastieIsPublic_CheckedChanged);
            // 
            // tpImageUploaders
            // 
            this.tpImageUploaders.Controls.Add(this.tcImageUploaders);
            resources.ApplyResources(this.tpImageUploaders, "tpImageUploaders");
            this.tpImageUploaders.Name = "tpImageUploaders";
            this.tpImageUploaders.UseVisualStyleBackColor = true;
            // 
            // tcImageUploaders
            // 
            this.tcImageUploaders.Controls.Add(this.tpImgur);
            this.tcImageUploaders.Controls.Add(this.tpImageShack);
            this.tcImageUploaders.Controls.Add(this.tpTinyPic);
            this.tcImageUploaders.Controls.Add(this.tpFlickr);
            this.tcImageUploaders.Controls.Add(this.tpPhotobucket);
            this.tcImageUploaders.Controls.Add(this.tpGooglePhotos);
            this.tcImageUploaders.Controls.Add(this.tpChevereto);
            this.tcImageUploaders.Controls.Add(this.tpVgyme);
            resources.ApplyResources(this.tcImageUploaders, "tcImageUploaders");
            this.tcImageUploaders.Name = "tcImageUploaders";
            this.tcImageUploaders.SelectedIndex = 0;
            // 
            // tpImgur
            // 
            this.tpImgur.Controls.Add(this.cbImgurUseGIFV);
            this.tpImgur.Controls.Add(this.cbImgurUploadSelectedAlbum);
            this.tpImgur.Controls.Add(this.cbImgurDirectLink);
            this.tpImgur.Controls.Add(this.atcImgurAccountType);
            this.tpImgur.Controls.Add(this.oauth2Imgur);
            this.tpImgur.Controls.Add(this.lvImgurAlbumList);
            this.tpImgur.Controls.Add(this.btnImgurRefreshAlbumList);
            this.tpImgur.Controls.Add(this.cbImgurThumbnailType);
            this.tpImgur.Controls.Add(this.lblImgurThumbnailType);
            resources.ApplyResources(this.tpImgur, "tpImgur");
            this.tpImgur.Name = "tpImgur";
            this.tpImgur.UseVisualStyleBackColor = true;
            // 
            // cbImgurUseGIFV
            // 
            resources.ApplyResources(this.cbImgurUseGIFV, "cbImgurUseGIFV");
            this.cbImgurUseGIFV.Name = "cbImgurUseGIFV";
            this.cbImgurUseGIFV.UseVisualStyleBackColor = true;
            this.cbImgurUseGIFV.CheckedChanged += new System.EventHandler(this.cbImgurUseGIFV_CheckedChanged);
            // 
            // cbImgurUploadSelectedAlbum
            // 
            resources.ApplyResources(this.cbImgurUploadSelectedAlbum, "cbImgurUploadSelectedAlbum");
            this.cbImgurUploadSelectedAlbum.Name = "cbImgurUploadSelectedAlbum";
            this.cbImgurUploadSelectedAlbum.UseVisualStyleBackColor = true;
            this.cbImgurUploadSelectedAlbum.CheckedChanged += new System.EventHandler(this.cbImgurUploadSelectedAlbum_CheckedChanged);
            // 
            // cbImgurDirectLink
            // 
            resources.ApplyResources(this.cbImgurDirectLink, "cbImgurDirectLink");
            this.cbImgurDirectLink.Name = "cbImgurDirectLink";
            this.cbImgurDirectLink.UseVisualStyleBackColor = true;
            this.cbImgurDirectLink.CheckedChanged += new System.EventHandler(this.cbImgurDirectLink_CheckedChanged);
            // 
            // atcImgurAccountType
            // 
            resources.ApplyResources(this.atcImgurAccountType, "atcImgurAccountType");
            this.atcImgurAccountType.Name = "atcImgurAccountType";
            this.atcImgurAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcImgurAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcImgurAccountType_AccountTypeChanged);
            // 
            // oauth2Imgur
            // 
            resources.ApplyResources(this.oauth2Imgur, "oauth2Imgur");
            this.oauth2Imgur.Name = "oauth2Imgur";
            this.oauth2Imgur.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Imgur_OpenButtonClicked);
            this.oauth2Imgur.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Imgur_CompleteButtonClicked);
            this.oauth2Imgur.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Imgur_ClearButtonClicked);
            this.oauth2Imgur.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2Imgur_RefreshButtonClicked);
            // 
            // lvImgurAlbumList
            // 
            this.lvImgurAlbumList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chImgurID,
            this.chImgurTitle,
            this.chImgurDescription});
            this.lvImgurAlbumList.FullRowSelect = true;
            this.lvImgurAlbumList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvImgurAlbumList.HideSelection = false;
            resources.ApplyResources(this.lvImgurAlbumList, "lvImgurAlbumList");
            this.lvImgurAlbumList.MultiSelect = false;
            this.lvImgurAlbumList.Name = "lvImgurAlbumList";
            this.lvImgurAlbumList.UseCompatibleStateImageBehavior = false;
            this.lvImgurAlbumList.View = System.Windows.Forms.View.Details;
            this.lvImgurAlbumList.SelectedIndexChanged += new System.EventHandler(this.lvImgurAlbumList_SelectedIndexChanged);
            // 
            // chImgurID
            // 
            resources.ApplyResources(this.chImgurID, "chImgurID");
            // 
            // chImgurTitle
            // 
            resources.ApplyResources(this.chImgurTitle, "chImgurTitle");
            // 
            // chImgurDescription
            // 
            resources.ApplyResources(this.chImgurDescription, "chImgurDescription");
            // 
            // btnImgurRefreshAlbumList
            // 
            resources.ApplyResources(this.btnImgurRefreshAlbumList, "btnImgurRefreshAlbumList");
            this.btnImgurRefreshAlbumList.Name = "btnImgurRefreshAlbumList";
            this.btnImgurRefreshAlbumList.UseVisualStyleBackColor = true;
            this.btnImgurRefreshAlbumList.Click += new System.EventHandler(this.btnImgurRefreshAlbumList_Click);
            // 
            // cbImgurThumbnailType
            // 
            this.cbImgurThumbnailType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImgurThumbnailType.FormattingEnabled = true;
            resources.ApplyResources(this.cbImgurThumbnailType, "cbImgurThumbnailType");
            this.cbImgurThumbnailType.Name = "cbImgurThumbnailType";
            this.cbImgurThumbnailType.SelectedIndexChanged += new System.EventHandler(this.cbImgurThumbnailType_SelectedIndexChanged);
            // 
            // lblImgurThumbnailType
            // 
            resources.ApplyResources(this.lblImgurThumbnailType, "lblImgurThumbnailType");
            this.lblImgurThumbnailType.Name = "lblImgurThumbnailType";
            // 
            // tpImageShack
            // 
            this.tpImageShack.Controls.Add(this.btnImageShackLogin);
            this.tpImageShack.Controls.Add(this.btnImageShackOpenPublicProfile);
            this.tpImageShack.Controls.Add(this.cbImageShackIsPublic);
            this.tpImageShack.Controls.Add(this.btnImageShackOpenMyImages);
            this.tpImageShack.Controls.Add(this.lblImageShackUsername);
            this.tpImageShack.Controls.Add(this.txtImageShackUsername);
            this.tpImageShack.Controls.Add(this.txtImageShackPassword);
            this.tpImageShack.Controls.Add(this.lblImageShackPassword);
            resources.ApplyResources(this.tpImageShack, "tpImageShack");
            this.tpImageShack.Name = "tpImageShack";
            this.tpImageShack.UseVisualStyleBackColor = true;
            // 
            // btnImageShackLogin
            // 
            resources.ApplyResources(this.btnImageShackLogin, "btnImageShackLogin");
            this.btnImageShackLogin.Name = "btnImageShackLogin";
            this.btnImageShackLogin.UseVisualStyleBackColor = true;
            this.btnImageShackLogin.Click += new System.EventHandler(this.btnImageShackLogin_Click);
            // 
            // btnImageShackOpenPublicProfile
            // 
            resources.ApplyResources(this.btnImageShackOpenPublicProfile, "btnImageShackOpenPublicProfile");
            this.btnImageShackOpenPublicProfile.Name = "btnImageShackOpenPublicProfile";
            this.btnImageShackOpenPublicProfile.UseVisualStyleBackColor = true;
            this.btnImageShackOpenPublicProfile.Click += new System.EventHandler(this.btnImageShackOpenPublicProfile_Click);
            // 
            // cbImageShackIsPublic
            // 
            resources.ApplyResources(this.cbImageShackIsPublic, "cbImageShackIsPublic");
            this.cbImageShackIsPublic.Name = "cbImageShackIsPublic";
            this.cbImageShackIsPublic.UseVisualStyleBackColor = true;
            this.cbImageShackIsPublic.CheckedChanged += new System.EventHandler(this.cbImageShackIsPublic_CheckedChanged);
            // 
            // btnImageShackOpenMyImages
            // 
            resources.ApplyResources(this.btnImageShackOpenMyImages, "btnImageShackOpenMyImages");
            this.btnImageShackOpenMyImages.Name = "btnImageShackOpenMyImages";
            this.btnImageShackOpenMyImages.UseVisualStyleBackColor = true;
            this.btnImageShackOpenMyImages.Click += new System.EventHandler(this.btnImageShackOpenMyImages_Click);
            // 
            // lblImageShackUsername
            // 
            resources.ApplyResources(this.lblImageShackUsername, "lblImageShackUsername");
            this.lblImageShackUsername.Name = "lblImageShackUsername";
            // 
            // txtImageShackUsername
            // 
            resources.ApplyResources(this.txtImageShackUsername, "txtImageShackUsername");
            this.txtImageShackUsername.Name = "txtImageShackUsername";
            this.txtImageShackUsername.TextChanged += new System.EventHandler(this.txtImageShackUsername_TextChanged);
            // 
            // txtImageShackPassword
            // 
            resources.ApplyResources(this.txtImageShackPassword, "txtImageShackPassword");
            this.txtImageShackPassword.Name = "txtImageShackPassword";
            this.txtImageShackPassword.UseSystemPasswordChar = true;
            this.txtImageShackPassword.TextChanged += new System.EventHandler(this.txtImageShackPassword_TextChanged);
            // 
            // lblImageShackPassword
            // 
            resources.ApplyResources(this.lblImageShackPassword, "lblImageShackPassword");
            this.lblImageShackPassword.Name = "lblImageShackPassword";
            // 
            // tpTinyPic
            // 
            this.tpTinyPic.Controls.Add(this.atcTinyPicAccountType);
            this.tpTinyPic.Controls.Add(this.btnTinyPicLogin);
            this.tpTinyPic.Controls.Add(this.txtTinyPicPassword);
            this.tpTinyPic.Controls.Add(this.lblTinyPicPassword);
            this.tpTinyPic.Controls.Add(this.txtTinyPicUsername);
            this.tpTinyPic.Controls.Add(this.lblTinyPicUsername);
            this.tpTinyPic.Controls.Add(this.btnTinyPicOpenMyImages);
            resources.ApplyResources(this.tpTinyPic, "tpTinyPic");
            this.tpTinyPic.Name = "tpTinyPic";
            this.tpTinyPic.UseVisualStyleBackColor = true;
            // 
            // atcTinyPicAccountType
            // 
            resources.ApplyResources(this.atcTinyPicAccountType, "atcTinyPicAccountType");
            this.atcTinyPicAccountType.Name = "atcTinyPicAccountType";
            this.atcTinyPicAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcTinyPicAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcTinyPicAccountType_AccountTypeChanged);
            // 
            // btnTinyPicLogin
            // 
            resources.ApplyResources(this.btnTinyPicLogin, "btnTinyPicLogin");
            this.btnTinyPicLogin.Name = "btnTinyPicLogin";
            this.btnTinyPicLogin.UseVisualStyleBackColor = true;
            this.btnTinyPicLogin.Click += new System.EventHandler(this.btnTinyPicLogin_Click);
            // 
            // txtTinyPicPassword
            // 
            resources.ApplyResources(this.txtTinyPicPassword, "txtTinyPicPassword");
            this.txtTinyPicPassword.Name = "txtTinyPicPassword";
            this.txtTinyPicPassword.UseSystemPasswordChar = true;
            this.txtTinyPicPassword.TextChanged += new System.EventHandler(this.txtTinyPicPassword_TextChanged);
            // 
            // lblTinyPicPassword
            // 
            resources.ApplyResources(this.lblTinyPicPassword, "lblTinyPicPassword");
            this.lblTinyPicPassword.Name = "lblTinyPicPassword";
            // 
            // txtTinyPicUsername
            // 
            resources.ApplyResources(this.txtTinyPicUsername, "txtTinyPicUsername");
            this.txtTinyPicUsername.Name = "txtTinyPicUsername";
            this.txtTinyPicUsername.TextChanged += new System.EventHandler(this.txtTinyPicUsername_TextChanged);
            // 
            // lblTinyPicUsername
            // 
            resources.ApplyResources(this.lblTinyPicUsername, "lblTinyPicUsername");
            this.lblTinyPicUsername.Name = "lblTinyPicUsername";
            // 
            // btnTinyPicOpenMyImages
            // 
            resources.ApplyResources(this.btnTinyPicOpenMyImages, "btnTinyPicOpenMyImages");
            this.btnTinyPicOpenMyImages.Name = "btnTinyPicOpenMyImages";
            this.btnTinyPicOpenMyImages.UseVisualStyleBackColor = true;
            this.btnTinyPicOpenMyImages.Click += new System.EventHandler(this.btnTinyPicOpenMyImages_Click);
            // 
            // tpFlickr
            // 
            this.tpFlickr.Controls.Add(this.cbFlickrDirectLink);
            this.tpFlickr.Controls.Add(this.oauthFlickr);
            resources.ApplyResources(this.tpFlickr, "tpFlickr");
            this.tpFlickr.Name = "tpFlickr";
            this.tpFlickr.UseVisualStyleBackColor = true;
            // 
            // cbFlickrDirectLink
            // 
            resources.ApplyResources(this.cbFlickrDirectLink, "cbFlickrDirectLink");
            this.cbFlickrDirectLink.Name = "cbFlickrDirectLink";
            this.cbFlickrDirectLink.UseVisualStyleBackColor = true;
            this.cbFlickrDirectLink.CheckedChanged += new System.EventHandler(this.cbFlickrDirectLink_CheckedChanged);
            // 
            // oauthFlickr
            // 
            this.oauthFlickr.IsRefreshable = false;
            resources.ApplyResources(this.oauthFlickr, "oauthFlickr");
            this.oauthFlickr.Name = "oauthFlickr";
            this.oauthFlickr.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauthFlickr_OpenButtonClicked);
            this.oauthFlickr.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauthFlickr_CompleteButtonClicked);
            this.oauthFlickr.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauthFlickr_ClearButtonClicked);
            // 
            // tpPhotobucket
            // 
            this.tpPhotobucket.Controls.Add(this.gbPhotobucketAlbumPath);
            this.tpPhotobucket.Controls.Add(this.gbPhotobucketAlbums);
            this.tpPhotobucket.Controls.Add(this.gbPhotobucketUserAccount);
            resources.ApplyResources(this.tpPhotobucket, "tpPhotobucket");
            this.tpPhotobucket.Name = "tpPhotobucket";
            this.tpPhotobucket.UseVisualStyleBackColor = true;
            // 
            // gbPhotobucketAlbumPath
            // 
            this.gbPhotobucketAlbumPath.Controls.Add(this.btnPhotobucketAddAlbum);
            this.gbPhotobucketAlbumPath.Controls.Add(this.btnPhotobucketRemoveAlbum);
            this.gbPhotobucketAlbumPath.Controls.Add(this.cboPhotobucketAlbumPaths);
            resources.ApplyResources(this.gbPhotobucketAlbumPath, "gbPhotobucketAlbumPath");
            this.gbPhotobucketAlbumPath.Name = "gbPhotobucketAlbumPath";
            this.gbPhotobucketAlbumPath.TabStop = false;
            // 
            // btnPhotobucketAddAlbum
            // 
            resources.ApplyResources(this.btnPhotobucketAddAlbum, "btnPhotobucketAddAlbum");
            this.btnPhotobucketAddAlbum.Name = "btnPhotobucketAddAlbum";
            this.btnPhotobucketAddAlbum.UseVisualStyleBackColor = true;
            this.btnPhotobucketAddAlbum.Click += new System.EventHandler(this.btnPhotobucketAddAlbum_Click);
            // 
            // btnPhotobucketRemoveAlbum
            // 
            resources.ApplyResources(this.btnPhotobucketRemoveAlbum, "btnPhotobucketRemoveAlbum");
            this.btnPhotobucketRemoveAlbum.Name = "btnPhotobucketRemoveAlbum";
            this.btnPhotobucketRemoveAlbum.UseVisualStyleBackColor = true;
            this.btnPhotobucketRemoveAlbum.Click += new System.EventHandler(this.btnPhotobucketRemoveAlbum_Click);
            // 
            // cboPhotobucketAlbumPaths
            // 
            this.cboPhotobucketAlbumPaths.FormattingEnabled = true;
            resources.ApplyResources(this.cboPhotobucketAlbumPaths, "cboPhotobucketAlbumPaths");
            this.cboPhotobucketAlbumPaths.Name = "cboPhotobucketAlbumPaths";
            this.cboPhotobucketAlbumPaths.SelectedIndexChanged += new System.EventHandler(this.cboPhotobucketAlbumPaths_SelectedIndexChanged);
            // 
            // gbPhotobucketAlbums
            // 
            this.gbPhotobucketAlbums.Controls.Add(this.lblPhotobucketNewAlbumName);
            this.gbPhotobucketAlbums.Controls.Add(this.lblPhotobucketParentAlbumPath);
            this.gbPhotobucketAlbums.Controls.Add(this.txtPhotobucketNewAlbumName);
            this.gbPhotobucketAlbums.Controls.Add(this.txtPhotobucketParentAlbumPath);
            this.gbPhotobucketAlbums.Controls.Add(this.btnPhotobucketCreateAlbum);
            resources.ApplyResources(this.gbPhotobucketAlbums, "gbPhotobucketAlbums");
            this.gbPhotobucketAlbums.Name = "gbPhotobucketAlbums";
            this.gbPhotobucketAlbums.TabStop = false;
            // 
            // lblPhotobucketNewAlbumName
            // 
            resources.ApplyResources(this.lblPhotobucketNewAlbumName, "lblPhotobucketNewAlbumName");
            this.lblPhotobucketNewAlbumName.Name = "lblPhotobucketNewAlbumName";
            // 
            // lblPhotobucketParentAlbumPath
            // 
            resources.ApplyResources(this.lblPhotobucketParentAlbumPath, "lblPhotobucketParentAlbumPath");
            this.lblPhotobucketParentAlbumPath.Name = "lblPhotobucketParentAlbumPath";
            // 
            // txtPhotobucketNewAlbumName
            // 
            resources.ApplyResources(this.txtPhotobucketNewAlbumName, "txtPhotobucketNewAlbumName");
            this.txtPhotobucketNewAlbumName.Name = "txtPhotobucketNewAlbumName";
            // 
            // txtPhotobucketParentAlbumPath
            // 
            resources.ApplyResources(this.txtPhotobucketParentAlbumPath, "txtPhotobucketParentAlbumPath");
            this.txtPhotobucketParentAlbumPath.Name = "txtPhotobucketParentAlbumPath";
            // 
            // btnPhotobucketCreateAlbum
            // 
            resources.ApplyResources(this.btnPhotobucketCreateAlbum, "btnPhotobucketCreateAlbum");
            this.btnPhotobucketCreateAlbum.Name = "btnPhotobucketCreateAlbum";
            this.btnPhotobucketCreateAlbum.UseVisualStyleBackColor = true;
            this.btnPhotobucketCreateAlbum.Click += new System.EventHandler(this.btnPhotobucketCreateAlbum_Click);
            // 
            // gbPhotobucketUserAccount
            // 
            this.gbPhotobucketUserAccount.Controls.Add(this.lblPhotobucketDefaultAlbumName);
            this.gbPhotobucketUserAccount.Controls.Add(this.btnPhotobucketAuthOpen);
            this.gbPhotobucketUserAccount.Controls.Add(this.txtPhotobucketDefaultAlbumName);
            this.gbPhotobucketUserAccount.Controls.Add(this.lblPhotobucketVerificationCode);
            this.gbPhotobucketUserAccount.Controls.Add(this.btnPhotobucketAuthComplete);
            this.gbPhotobucketUserAccount.Controls.Add(this.txtPhotobucketVerificationCode);
            this.gbPhotobucketUserAccount.Controls.Add(this.lblPhotobucketAccountStatus);
            resources.ApplyResources(this.gbPhotobucketUserAccount, "gbPhotobucketUserAccount");
            this.gbPhotobucketUserAccount.Name = "gbPhotobucketUserAccount";
            this.gbPhotobucketUserAccount.TabStop = false;
            // 
            // lblPhotobucketDefaultAlbumName
            // 
            resources.ApplyResources(this.lblPhotobucketDefaultAlbumName, "lblPhotobucketDefaultAlbumName");
            this.lblPhotobucketDefaultAlbumName.Name = "lblPhotobucketDefaultAlbumName";
            // 
            // btnPhotobucketAuthOpen
            // 
            resources.ApplyResources(this.btnPhotobucketAuthOpen, "btnPhotobucketAuthOpen");
            this.btnPhotobucketAuthOpen.Name = "btnPhotobucketAuthOpen";
            this.btnPhotobucketAuthOpen.UseVisualStyleBackColor = true;
            this.btnPhotobucketAuthOpen.Click += new System.EventHandler(this.btnPhotobucketAuthOpen_Click);
            // 
            // txtPhotobucketDefaultAlbumName
            // 
            resources.ApplyResources(this.txtPhotobucketDefaultAlbumName, "txtPhotobucketDefaultAlbumName");
            this.txtPhotobucketDefaultAlbumName.Name = "txtPhotobucketDefaultAlbumName";
            this.txtPhotobucketDefaultAlbumName.ReadOnly = true;
            // 
            // lblPhotobucketVerificationCode
            // 
            resources.ApplyResources(this.lblPhotobucketVerificationCode, "lblPhotobucketVerificationCode");
            this.lblPhotobucketVerificationCode.Name = "lblPhotobucketVerificationCode";
            // 
            // btnPhotobucketAuthComplete
            // 
            resources.ApplyResources(this.btnPhotobucketAuthComplete, "btnPhotobucketAuthComplete");
            this.btnPhotobucketAuthComplete.Name = "btnPhotobucketAuthComplete";
            this.btnPhotobucketAuthComplete.UseVisualStyleBackColor = true;
            this.btnPhotobucketAuthComplete.Click += new System.EventHandler(this.btnPhotobucketAuthComplete_Click);
            // 
            // txtPhotobucketVerificationCode
            // 
            resources.ApplyResources(this.txtPhotobucketVerificationCode, "txtPhotobucketVerificationCode");
            this.txtPhotobucketVerificationCode.Name = "txtPhotobucketVerificationCode";
            // 
            // lblPhotobucketAccountStatus
            // 
            resources.ApplyResources(this.lblPhotobucketAccountStatus, "lblPhotobucketAccountStatus");
            this.lblPhotobucketAccountStatus.Name = "lblPhotobucketAccountStatus";
            // 
            // tpGooglePhotos
            // 
            this.tpGooglePhotos.Controls.Add(this.txtPicasaAlbumID);
            this.tpGooglePhotos.Controls.Add(this.lblPicasaAlbumID);
            this.tpGooglePhotos.Controls.Add(this.lvPicasaAlbumList);
            this.tpGooglePhotos.Controls.Add(this.btnPicasaRefreshAlbumList);
            this.tpGooglePhotos.Controls.Add(this.oauth2Picasa);
            resources.ApplyResources(this.tpGooglePhotos, "tpGooglePhotos");
            this.tpGooglePhotos.Name = "tpGooglePhotos";
            this.tpGooglePhotos.UseVisualStyleBackColor = true;
            // 
            // txtPicasaAlbumID
            // 
            resources.ApplyResources(this.txtPicasaAlbumID, "txtPicasaAlbumID");
            this.txtPicasaAlbumID.Name = "txtPicasaAlbumID";
            this.txtPicasaAlbumID.TextChanged += new System.EventHandler(this.txtPicasaAlbumID_TextChanged);
            // 
            // lblPicasaAlbumID
            // 
            resources.ApplyResources(this.lblPicasaAlbumID, "lblPicasaAlbumID");
            this.lblPicasaAlbumID.Name = "lblPicasaAlbumID";
            // 
            // lvPicasaAlbumList
            // 
            this.lvPicasaAlbumList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chPicasaID,
            this.chPicasaName,
            this.chPicasaDescription});
            this.lvPicasaAlbumList.FullRowSelect = true;
            resources.ApplyResources(this.lvPicasaAlbumList, "lvPicasaAlbumList");
            this.lvPicasaAlbumList.MultiSelect = false;
            this.lvPicasaAlbumList.Name = "lvPicasaAlbumList";
            this.lvPicasaAlbumList.UseCompatibleStateImageBehavior = false;
            this.lvPicasaAlbumList.View = System.Windows.Forms.View.Details;
            this.lvPicasaAlbumList.SelectedIndexChanged += new System.EventHandler(this.lvPicasaAlbumList_SelectedIndexChanged);
            // 
            // chPicasaID
            // 
            resources.ApplyResources(this.chPicasaID, "chPicasaID");
            // 
            // chPicasaName
            // 
            resources.ApplyResources(this.chPicasaName, "chPicasaName");
            // 
            // chPicasaDescription
            // 
            resources.ApplyResources(this.chPicasaDescription, "chPicasaDescription");
            // 
            // btnPicasaRefreshAlbumList
            // 
            resources.ApplyResources(this.btnPicasaRefreshAlbumList, "btnPicasaRefreshAlbumList");
            this.btnPicasaRefreshAlbumList.Name = "btnPicasaRefreshAlbumList";
            this.btnPicasaRefreshAlbumList.UseVisualStyleBackColor = true;
            this.btnPicasaRefreshAlbumList.Click += new System.EventHandler(this.btnPicasaRefreshAlbumList_Click);
            // 
            // oauth2Picasa
            // 
            resources.ApplyResources(this.oauth2Picasa, "oauth2Picasa");
            this.oauth2Picasa.Name = "oauth2Picasa";
            this.oauth2Picasa.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Picasa_OpenButtonClicked);
            this.oauth2Picasa.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Picasa_CompleteButtonClicked);
            this.oauth2Picasa.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Picasa_ClearButtonClicked);
            this.oauth2Picasa.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2Picasa_RefreshButtonClicked);
            // 
            // tpChevereto
            // 
            this.tpChevereto.Controls.Add(this.btnCheveretoTestAll);
            this.tpChevereto.Controls.Add(this.lblCheveretoUploadURLExample);
            this.tpChevereto.Controls.Add(this.lblCheveretoUploaders);
            this.tpChevereto.Controls.Add(this.cbCheveretoUploaders);
            this.tpChevereto.Controls.Add(this.cbCheveretoDirectURL);
            this.tpChevereto.Controls.Add(this.lblCheveretoUploadURL);
            this.tpChevereto.Controls.Add(this.txtCheveretoUploadURL);
            this.tpChevereto.Controls.Add(this.txtCheveretoAPIKey);
            this.tpChevereto.Controls.Add(this.lblCheveretoAPIKey);
            resources.ApplyResources(this.tpChevereto, "tpChevereto");
            this.tpChevereto.Name = "tpChevereto";
            this.tpChevereto.UseVisualStyleBackColor = true;
            // 
            // btnCheveretoTestAll
            // 
            resources.ApplyResources(this.btnCheveretoTestAll, "btnCheveretoTestAll");
            this.btnCheveretoTestAll.Name = "btnCheveretoTestAll";
            this.btnCheveretoTestAll.UseVisualStyleBackColor = true;
            this.btnCheveretoTestAll.Click += new System.EventHandler(this.btnCheveretoTestAll_Click);
            // 
            // lblCheveretoUploadURLExample
            // 
            resources.ApplyResources(this.lblCheveretoUploadURLExample, "lblCheveretoUploadURLExample");
            this.lblCheveretoUploadURLExample.Name = "lblCheveretoUploadURLExample";
            // 
            // lblCheveretoUploaders
            // 
            resources.ApplyResources(this.lblCheveretoUploaders, "lblCheveretoUploaders");
            this.lblCheveretoUploaders.Name = "lblCheveretoUploaders";
            // 
            // cbCheveretoUploaders
            // 
            this.cbCheveretoUploaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCheveretoUploaders.FormattingEnabled = true;
            resources.ApplyResources(this.cbCheveretoUploaders, "cbCheveretoUploaders");
            this.cbCheveretoUploaders.Name = "cbCheveretoUploaders";
            this.cbCheveretoUploaders.SelectedIndexChanged += new System.EventHandler(this.cbCheveretoUploaders_SelectedIndexChanged);
            // 
            // cbCheveretoDirectURL
            // 
            resources.ApplyResources(this.cbCheveretoDirectURL, "cbCheveretoDirectURL");
            this.cbCheveretoDirectURL.Name = "cbCheveretoDirectURL";
            this.cbCheveretoDirectURL.UseVisualStyleBackColor = true;
            this.cbCheveretoDirectURL.CheckedChanged += new System.EventHandler(this.cbCheveretoDirectURL_CheckedChanged);
            // 
            // lblCheveretoUploadURL
            // 
            resources.ApplyResources(this.lblCheveretoUploadURL, "lblCheveretoUploadURL");
            this.lblCheveretoUploadURL.Name = "lblCheveretoUploadURL";
            // 
            // txtCheveretoUploadURL
            // 
            resources.ApplyResources(this.txtCheveretoUploadURL, "txtCheveretoUploadURL");
            this.txtCheveretoUploadURL.Name = "txtCheveretoUploadURL";
            this.txtCheveretoUploadURL.TextChanged += new System.EventHandler(this.txtCheveretoWebsite_TextChanged);
            // 
            // txtCheveretoAPIKey
            // 
            resources.ApplyResources(this.txtCheveretoAPIKey, "txtCheveretoAPIKey");
            this.txtCheveretoAPIKey.Name = "txtCheveretoAPIKey";
            this.txtCheveretoAPIKey.UseSystemPasswordChar = true;
            this.txtCheveretoAPIKey.TextChanged += new System.EventHandler(this.txtCheveretoAPIKey_TextChanged);
            // 
            // lblCheveretoAPIKey
            // 
            resources.ApplyResources(this.lblCheveretoAPIKey, "lblCheveretoAPIKey");
            this.lblCheveretoAPIKey.Name = "lblCheveretoAPIKey";
            // 
            // tpVgyme
            // 
            this.tpVgyme.Controls.Add(this.llVgymeAccountDetailsPage);
            this.tpVgyme.Controls.Add(this.txtVgymeUserKey);
            this.tpVgyme.Controls.Add(this.lvlVgymeUserKey);
            resources.ApplyResources(this.tpVgyme, "tpVgyme");
            this.tpVgyme.Name = "tpVgyme";
            this.tpVgyme.UseVisualStyleBackColor = true;
            // 
            // llVgymeAccountDetailsPage
            // 
            resources.ApplyResources(this.llVgymeAccountDetailsPage, "llVgymeAccountDetailsPage");
            this.llVgymeAccountDetailsPage.Name = "llVgymeAccountDetailsPage";
            this.llVgymeAccountDetailsPage.TabStop = true;
            this.llVgymeAccountDetailsPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llVgymeAccountDetailsPage_LinkClicked);
            // 
            // txtVgymeUserKey
            // 
            resources.ApplyResources(this.txtVgymeUserKey, "txtVgymeUserKey");
            this.txtVgymeUserKey.Name = "txtVgymeUserKey";
            this.txtVgymeUserKey.UseSystemPasswordChar = true;
            this.txtVgymeUserKey.TextChanged += new System.EventHandler(this.txtVgymeUserKey_TextChanged);
            // 
            // lvlVgymeUserKey
            // 
            resources.ApplyResources(this.lvlVgymeUserKey, "lvlVgymeUserKey");
            this.lvlVgymeUserKey.Name = "lvlVgymeUserKey";
            // 
            // tcUploaders
            // 
            this.tcUploaders.Controls.Add(this.tpImageUploaders);
            this.tcUploaders.Controls.Add(this.tpTextUploaders);
            this.tcUploaders.Controls.Add(this.tpFileUploaders);
            this.tcUploaders.Controls.Add(this.tpURLShorteners);
            this.tcUploaders.Controls.Add(this.tpOtherUploaders);
            resources.ApplyResources(this.tcUploaders, "tcUploaders");
            this.tcUploaders.Name = "tcUploaders";
            this.tcUploaders.SelectedIndex = 0;
            // 
            // lblWidthHint
            // 
            this.lblWidthHint.BackColor = System.Drawing.SystemColors.Highlight;
            resources.ApplyResources(this.lblWidthHint, "lblWidthHint");
            this.lblWidthHint.Name = "lblWidthHint";
            // 
            // ttlvMain
            // 
            resources.ApplyResources(this.ttlvMain, "ttlvMain");
            this.ttlvMain.ImageList = null;
            this.ttlvMain.ListViewSize = 180;
            this.ttlvMain.MainTabControl = null;
            this.ttlvMain.Name = "ttlvMain";
            // 
            // actRapidShareAccountType
            // 
            resources.ApplyResources(this.actRapidShareAccountType, "actRapidShareAccountType");
            this.actRapidShareAccountType.Name = "actRapidShareAccountType";
            this.actRapidShareAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            // 
            // cbAmazonS3PublicACL
            // 
            resources.ApplyResources(this.cbAmazonS3PublicACL, "cbAmazonS3PublicACL");
            this.cbAmazonS3PublicACL.Name = "cbAmazonS3PublicACL";
            this.cbAmazonS3PublicACL.UseVisualStyleBackColor = true;
            this.cbAmazonS3PublicACL.CheckedChanged += new System.EventHandler(this.cbAmazonS3PublicACL_CheckedChanged);
            // 
            // UploadersConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblWidthHint);
            this.Controls.Add(this.tcUploaders);
            this.Controls.Add(this.ttlvMain);
            this.Name = "UploadersConfigForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.UploadersConfigForm_Shown);
            this.Resize += new System.EventHandler(this.UploadersConfigForm_Resize);
            this.tpOtherUploaders.ResumeLayout(false);
            this.tcOtherUploaders.ResumeLayout(false);
            this.tpTwitter.ResumeLayout(false);
            this.tpTwitter.PerformLayout();
            this.tpCustomUploaders.ResumeLayout(false);
            this.tpCustomUploaders.PerformLayout();
            this.pCustomUploader.ResumeLayout(false);
            this.pCustomUploader.PerformLayout();
            this.tcCustomUploaderResponseParse.ResumeLayout(false);
            this.tpCustomUploaderJsonParse.ResumeLayout(false);
            this.tpCustomUploaderJsonParse.PerformLayout();
            this.tpCustomUploaderXmlParse.ResumeLayout(false);
            this.tpCustomUploaderXmlParse.PerformLayout();
            this.tpCustomUploaderRegexParse.ResumeLayout(false);
            this.tpCustomUploaderRegexParse.PerformLayout();
            this.tcCustomUploaderArguments.ResumeLayout(false);
            this.tpCustomUploaderArguments.ResumeLayout(false);
            this.tpCustomUploaderArguments.PerformLayout();
            this.tpCustomUploaderHeaders.ResumeLayout(false);
            this.tpCustomUploaderHeaders.PerformLayout();
            this.gbCustomUploaders.ResumeLayout(false);
            this.tpURLShorteners.ResumeLayout(false);
            this.tcURLShorteners.ResumeLayout(false);
            this.tpBitly.ResumeLayout(false);
            this.tpBitly.PerformLayout();
            this.tpGoogleURLShortener.ResumeLayout(false);
            this.tpYourls.ResumeLayout(false);
            this.tpYourls.PerformLayout();
            this.tpAdFly.ResumeLayout(false);
            this.tpAdFly.PerformLayout();
            this.tpCoinURL.ResumeLayout(false);
            this.tpCoinURL.PerformLayout();
            this.tpPolr.ResumeLayout(false);
            this.tpPolr.PerformLayout();
            this.tpFileUploaders.ResumeLayout(false);
            this.tcFileUploaders.ResumeLayout(false);
            this.tpFTP.ResumeLayout(false);
            this.tpFTP.PerformLayout();
            this.gbFTPAccount.ResumeLayout(false);
            this.gbFTPAccount.PerformLayout();
            this.gbSFTP.ResumeLayout(false);
            this.gbSFTP.PerformLayout();
            this.pFTPTransferMode.ResumeLayout(false);
            this.pFTPTransferMode.PerformLayout();
            this.pFTPProtocol.ResumeLayout(false);
            this.pFTPProtocol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFTPPort)).EndInit();
            this.gbFTPS.ResumeLayout(false);
            this.gbFTPS.PerformLayout();
            this.tpDropbox.ResumeLayout(false);
            this.tpDropbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropboxLogo)).EndInit();
            this.tpOneDrive.ResumeLayout(false);
            this.tpOneDrive.PerformLayout();
            this.tpGoogleDrive.ResumeLayout(false);
            this.tpGoogleDrive.PerformLayout();
            this.tpPuush.ResumeLayout(false);
            this.tpPuush.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPuush)).EndInit();
            this.tpBox.ResumeLayout(false);
            this.tpBox.PerformLayout();
            this.tpAmazonS3.ResumeLayout(false);
            this.tpAmazonS3.PerformLayout();
            this.tpAzureStorage.ResumeLayout(false);
            this.tpAzureStorage.PerformLayout();
            this.tpGfycat.ResumeLayout(false);
            this.tpGfycat.PerformLayout();
            this.tpMega.ResumeLayout(false);
            this.tpMega.PerformLayout();
            this.tpOwnCloud.ResumeLayout(false);
            this.tpOwnCloud.PerformLayout();
            this.tpMediaFire.ResumeLayout(false);
            this.tpMediaFire.PerformLayout();
            this.tpPushbullet.ResumeLayout(false);
            this.tpPushbullet.PerformLayout();
            this.tpSendSpace.ResumeLayout(false);
            this.tpSendSpace.PerformLayout();
            this.tpGe_tt.ResumeLayout(false);
            this.tpGe_tt.PerformLayout();
            this.tpHostr.ResumeLayout(false);
            this.tpHostr.PerformLayout();
            this.tpJira.ResumeLayout(false);
            this.tpJira.PerformLayout();
            this.gbJiraServer.ResumeLayout(false);
            this.gbJiraServer.PerformLayout();
            this.tpLambda.ResumeLayout(false);
            this.tpLambda.PerformLayout();
            this.tpPomf.ResumeLayout(false);
            this.tpPomf.PerformLayout();
            this.tpSeafile.ResumeLayout(false);
            this.tpSeafile.PerformLayout();
            this.grpSeafileShareSettings.ResumeLayout(false);
            this.grpSeafileShareSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeafileExpireDays)).EndInit();
            this.grpSeafileAccInfo.ResumeLayout(false);
            this.grpSeafileAccInfo.PerformLayout();
            this.grpSeafileObtainAuthToken.ResumeLayout(false);
            this.grpSeafileObtainAuthToken.PerformLayout();
            this.tpStreamable.ResumeLayout(false);
            this.tpStreamable.PerformLayout();
            this.tpSul.ResumeLayout(false);
            this.tpSul.PerformLayout();
            this.tpLithiio.ResumeLayout(false);
            this.tpLithiio.PerformLayout();
            this.tpPlik.ResumeLayout(false);
            this.gbPlikSettings.ResumeLayout(false);
            this.gbPlikSettings.PerformLayout();
            this.gbPlikLoginCredentials.ResumeLayout(false);
            this.gbPlikLoginCredentials.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlikTTL)).EndInit();
            this.tpSharedFolder.ResumeLayout(false);
            this.tpSharedFolder.PerformLayout();
            this.tpEmail.ResumeLayout(false);
            this.tpEmail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmailSmtpPort)).EndInit();
            this.tpTextUploaders.ResumeLayout(false);
            this.tcTextUploaders.ResumeLayout(false);
            this.tpPastebin.ResumeLayout(false);
            this.tpPastebin.PerformLayout();
            this.tpPaste_ee.ResumeLayout(false);
            this.tpPaste_ee.PerformLayout();
            this.tpGist.ResumeLayout(false);
            this.tpGist.PerformLayout();
            this.tpUpaste.ResumeLayout(false);
            this.tpUpaste.PerformLayout();
            this.tpHastebin.ResumeLayout(false);
            this.tpHastebin.PerformLayout();
            this.tpOneTimeSecret.ResumeLayout(false);
            this.tpOneTimeSecret.PerformLayout();
            this.tpPastie.ResumeLayout(false);
            this.tpPastie.PerformLayout();
            this.tpImageUploaders.ResumeLayout(false);
            this.tcImageUploaders.ResumeLayout(false);
            this.tpImgur.ResumeLayout(false);
            this.tpImgur.PerformLayout();
            this.tpImageShack.ResumeLayout(false);
            this.tpImageShack.PerformLayout();
            this.tpTinyPic.ResumeLayout(false);
            this.tpTinyPic.PerformLayout();
            this.tpFlickr.ResumeLayout(false);
            this.tpFlickr.PerformLayout();
            this.tpPhotobucket.ResumeLayout(false);
            this.gbPhotobucketAlbumPath.ResumeLayout(false);
            this.gbPhotobucketAlbumPath.PerformLayout();
            this.gbPhotobucketAlbums.ResumeLayout(false);
            this.gbPhotobucketAlbums.PerformLayout();
            this.gbPhotobucketUserAccount.ResumeLayout(false);
            this.gbPhotobucketUserAccount.PerformLayout();
            this.tpGooglePhotos.ResumeLayout(false);
            this.tpGooglePhotos.PerformLayout();
            this.tpChevereto.ResumeLayout(false);
            this.tpChevereto.PerformLayout();
            this.tpVgyme.ResumeLayout(false);
            this.tpVgyme.PerformLayout();
            this.tcUploaders.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtRapidSharePremiumUserName;
        private AccountTypeControl actRapidShareAccountType;
        private System.Windows.Forms.ToolTip ttHelpTip;
        private ShareX.HelpersLib.TabToListView ttlvMain;
        private System.Windows.Forms.TabPage tpOtherUploaders;
        private System.Windows.Forms.TabControl tcOtherUploaders;
        private System.Windows.Forms.Button btnCustomUploaderHelp;
        private System.Windows.Forms.Label lblCustomUploaderImageUploader;
        private System.Windows.Forms.Button btnCustomUploaderFileUploaderTest;
        private System.Windows.Forms.Label lblCustomUploaderFileUploader;
        private System.Windows.Forms.Button btnCustomUploaderImageUploaderTest;
        private System.Windows.Forms.Label lblCustomUploaderTestResult;
        private System.Windows.Forms.TextBox txtCustomUploaderDeletionURL;
        private System.Windows.Forms.ComboBox cbCustomUploaderFileUploader;
        private System.Windows.Forms.Label lblCustomUploaderDeletionURL;
        private System.Windows.Forms.Button btnCustomUploaderShowLastResponse;
        private System.Windows.Forms.Label lblCustomUploaderResponseType;
        private System.Windows.Forms.ComboBox cbCustomUploaderURLShortener;
        private System.Windows.Forms.GroupBox gbCustomUploaders;
        private System.Windows.Forms.ListBox lbCustomUploaderList;
        private System.Windows.Forms.Button btnCustomUploaderRemove;
        private System.Windows.Forms.TextBox txtCustomUploaderName;
        private System.Windows.Forms.Button btnCustomUploaderAdd;
        private System.Windows.Forms.Label lblCustomUploaderTextUploader;
        private System.Windows.Forms.Label lblCustomUploaderRequestURL;
        private System.Windows.Forms.Button btnCustomUploaderURLShortenerTest;
        private System.Windows.Forms.Button btnCustomUploaderRegexpUpdate;
        private System.Windows.Forms.TextBox txtCustomUploaderRegexp;
        private ShareX.HelpersLib.MyListView lvCustomUploaderRegexps;
        private System.Windows.Forms.ColumnHeader lvRegexpsColumn;
        private System.Windows.Forms.Button btnCustomUploaderRegexpRemove;
        private System.Windows.Forms.Button btnCustomUploaderRegexpAdd;
        private System.Windows.Forms.ComboBox cbCustomUploaderTextUploader;
        private System.Windows.Forms.TextBox txtCustomUploaderThumbnailURL;
        private System.Windows.Forms.Label lblCustomUploaderURLShortener;
        private System.Windows.Forms.ComboBox cbCustomUploaderResponseType;
        private System.Windows.Forms.Button btnCustomUploaderTextUploaderTest;
        private System.Windows.Forms.TextBox txtCustomUploaderURL;
        private System.Windows.Forms.ComboBox cbCustomUploaderImageUploader;
        private System.Windows.Forms.TextBox txtCustomUploaderRequestURL;
        private System.Windows.Forms.RichTextBox txtCustomUploaderLog;
        private System.Windows.Forms.Label lblCustomUploaderThumbnailURL;
        private System.Windows.Forms.Label lblCustomUploaderFileForm;
        private System.Windows.Forms.Label lblCustomUploaderRequestType;
        private System.Windows.Forms.ComboBox cbCustomUploaderRequestType;
        private System.Windows.Forms.TextBox txtCustomUploaderFileForm;
        private System.Windows.Forms.Label lblCustomUploaderURL;
        private System.Windows.Forms.Button btnCustomUploaderArgUpdate;
        private System.Windows.Forms.TextBox txtCustomUploaderArgValue;
        private System.Windows.Forms.Button btnCustomUploaderArgRemove;
        private ShareX.HelpersLib.MyListView lvCustomUploaderArguments;
        private System.Windows.Forms.ColumnHeader chCustomUploaderArgumentsName;
        private System.Windows.Forms.ColumnHeader chCustomUploaderArgumentsValue;
        private System.Windows.Forms.Button btnCustomUploaderArgAdd;
        private System.Windows.Forms.TextBox txtCustomUploaderArgName;
        private System.Windows.Forms.TabPage tpURLShorteners;
        private System.Windows.Forms.TabControl tcURLShorteners;
        private OAuthControl oauth2Bitly;
        private OAuthControl oauth2GoogleURLShortener;
        private AccountTypeControl atcGoogleURLShortenerAccountType;
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
        private System.Windows.Forms.PictureBox pbDropboxLogo;
        private System.Windows.Forms.Label lblDropboxPath;
        private System.Windows.Forms.TextBox txtDropboxPath;
        private System.Windows.Forms.Button btnCopyShowFiles;
        internal System.Windows.Forms.TabPage tpFTP;
        private System.Windows.Forms.Button btnFTPClient;
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
        private System.Windows.Forms.ComboBox cboPushbulletDevices;
        private System.Windows.Forms.Button btnPushbulletGetDeviceList;
        private System.Windows.Forms.Label lblPushbulletUserKey;
        private System.Windows.Forms.TextBox txtPushbulletUserKey;
        private System.Windows.Forms.CheckBox cbGoogleDriveIsPublic;
        private OAuthControl oauth2GoogleDrive;
        private System.Windows.Forms.Label lblBoxFolderTip;
        private System.Windows.Forms.CheckBox cbBoxShare;
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
        private System.Windows.Forms.Label lblGe_ttStatus;
        private System.Windows.Forms.Label lblGe_ttPassword;
        private System.Windows.Forms.Label lblGe_ttEmail;
        private System.Windows.Forms.Button btnGe_ttLogin;
        private System.Windows.Forms.TextBox txtGe_ttPassword;
        private System.Windows.Forms.TextBox txtGe_ttEmail;
        private System.Windows.Forms.CheckBox cbLocalhostrDirectURL;
        private System.Windows.Forms.Label lblLocalhostrPassword;
        private System.Windows.Forms.Label lblLocalhostrEmail;
        private System.Windows.Forms.TextBox txtLocalhostrPassword;
        private System.Windows.Forms.TextBox txtLocalhostrEmail;
        private System.Windows.Forms.TextBox txtJiraIssuePrefix;
        private System.Windows.Forms.Label lblJiraIssuePrefix;
        private System.Windows.Forms.GroupBox gbJiraServer;
        private System.Windows.Forms.TextBox txtJiraConfigHelp;
        private System.Windows.Forms.TextBox txtJiraHost;
        private System.Windows.Forms.Label lblJiraHost;
        private OAuthControl oAuthJira;
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
        private AccountsControl ucLocalhostAccounts;
        private System.Windows.Forms.Label lblSharedFolderFiles;
        private System.Windows.Forms.Label lblSharedFolderText;
        private System.Windows.Forms.Label lblSharedFolderImages;
        private System.Windows.Forms.ComboBox cboSharedFolderFiles;
        private System.Windows.Forms.ComboBox cboSharedFolderText;
        private System.Windows.Forms.ComboBox cboSharedFolderImages;
        private System.Windows.Forms.TabPage tpTextUploaders;
        private System.Windows.Forms.TabControl tcTextUploaders;
        private System.Windows.Forms.Button btnPastebinLogin;
        private System.Windows.Forms.Label lblPaste_eeUserAPIKey;
        private System.Windows.Forms.TextBox txtPaste_eeUserAPIKey;
        private System.Windows.Forms.CheckBox cbGistPublishPublic;
        private OAuthControl oAuth2Gist;
        private AccountTypeControl atcGistAccountType;
        private System.Windows.Forms.CheckBox cbUpasteIsPublic;
        private System.Windows.Forms.Label lblUpasteUserKey;
        private System.Windows.Forms.TextBox txtUpasteUserKey;
        private System.Windows.Forms.TabPage tpImageUploaders;
        private System.Windows.Forms.TabControl tcImageUploaders;
        private OAuthControl oauth2Imgur;
        private System.Windows.Forms.ListView lvImgurAlbumList;
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
        private AccountTypeControl atcTinyPicAccountType;
        private System.Windows.Forms.Button btnTinyPicLogin;
        private System.Windows.Forms.TextBox txtTinyPicPassword;
        private System.Windows.Forms.Label lblTinyPicPassword;
        private System.Windows.Forms.TextBox txtTinyPicUsername;
        private System.Windows.Forms.Label lblTinyPicUsername;
        private System.Windows.Forms.Button btnTinyPicOpenMyImages;
        private System.Windows.Forms.GroupBox gbPhotobucketAlbumPath;
        private System.Windows.Forms.Button btnPhotobucketAddAlbum;
        private System.Windows.Forms.Button btnPhotobucketRemoveAlbum;
        private System.Windows.Forms.ComboBox cboPhotobucketAlbumPaths;
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
        private System.Windows.Forms.TextBox txtPicasaAlbumID;
        private System.Windows.Forms.Label lblPicasaAlbumID;
        private System.Windows.Forms.ListView lvPicasaAlbumList;
        private System.Windows.Forms.ColumnHeader chPicasaID;
        private System.Windows.Forms.ColumnHeader chPicasaName;
        private System.Windows.Forms.ColumnHeader chPicasaDescription;
        private System.Windows.Forms.Button btnPicasaRefreshAlbumList;
        private OAuthControl oauth2Picasa;
        private System.Windows.Forms.TabControl tcUploaders;
        private ShareX.HelpersLib.ExportImportControl eiCustomUploaders;
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
        private System.Windows.Forms.Label lblWidthHint;
        private System.Windows.Forms.Button btnCustomUploaderExamples;
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
        private System.Windows.Forms.TextBox txtAdflyAPIUID;
        private System.Windows.Forms.Label lblAdflyAPIUID;
        private System.Windows.Forms.TextBox txtAdflyAPIKEY;
        private System.Windows.Forms.Label lblAdflyAPIKEY;
        private System.Windows.Forms.LinkLabel llAdflyLink;
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
        private System.Windows.Forms.Label lblOneDriveFolderID;
        private System.Windows.Forms.TreeView tvOneDrive;
        private System.Windows.Forms.Label lblLambdaApiKey;
        private System.Windows.Forms.TextBox txtLambdaApiKey;
        private System.Windows.Forms.Label lblLambdaInfo;
        private System.Windows.Forms.Label lblLambdaUploadURL;
        private System.Windows.Forms.ComboBox cbLambdaUploadURL;
        private System.Windows.Forms.Label lblLithiioApiKey;
        private System.Windows.Forms.TextBox txtLithiioApiKey;
        private OAuthControl oauthTwitter;
        private System.Windows.Forms.TextBox txtTwitterDescription;
        private System.Windows.Forms.Label lblTwitterDescription;
        private System.Windows.Forms.Button btnTwitterRemove;
        private System.Windows.Forms.Button btnTwitterAdd;
        private System.Windows.Forms.Label lblTwitterDefaultMessage;
        private System.Windows.Forms.TextBox txtTwitterDefaultMessage;
        private System.Windows.Forms.CheckBox cbTwitterSkipMessageBox;
        private System.Windows.Forms.TextBox txtCoinURLUUID;
        private System.Windows.Forms.Label lblCoinURLUUID;
        private System.Windows.Forms.CheckBox cbOwnCloud81Compatibility;
        private System.Windows.Forms.Button btnCustomUploaderClearUploaders;
        private System.Windows.Forms.Label lblOneTimeSecretAPIKey;
        private System.Windows.Forms.Label lblOneTimeSecretEmail;
        private System.Windows.Forms.TextBox txtOneTimeSecretAPIKey;
        private System.Windows.Forms.TextBox txtOneTimeSecretEmail;
        private System.Windows.Forms.TextBox txtPolrAPIKey;
        private System.Windows.Forms.Label lblPolrAPIKey;
        private System.Windows.Forms.TextBox txtPolrAPIHostname;
        private System.Windows.Forms.Label lblPolrAPIHostname;
        private System.Windows.Forms.CheckBox cbImgurUseGIFV;
        private System.Windows.Forms.ListBox lbTwitterAccounts;
        private System.Windows.Forms.Button btnTwitterNameUpdate;
        private System.Windows.Forms.Label lblPomfResultURL;
        private System.Windows.Forms.Label lblPomfUploadURL;
        private System.Windows.Forms.Label lblPomfUploaders;
        private System.Windows.Forms.ComboBox cbPomfUploaders;
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
        private System.Windows.Forms.CheckBox cbSeafileIgnoreInvalidCert;
        private System.Windows.Forms.CheckBox cbSeafileCreateShareableURL;
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
        private System.Windows.Forms.TabControl tcCustomUploaderArguments;
        private System.Windows.Forms.TabPage tpCustomUploaderArguments;
        private System.Windows.Forms.TabPage tpCustomUploaderHeaders;
        private System.Windows.Forms.Button btnCustomUploaderHeaderUpdate;
        private System.Windows.Forms.TextBox txtCustomUploaderHeaderName;
        private System.Windows.Forms.TextBox txtCustomUploaderHeaderValue;
        private System.Windows.Forms.Button btnCustomUploaderHeaderAdd;
        private System.Windows.Forms.Button btnCustomUploaderHeaderRemove;
        private HelpersLib.MyListView lvCustomUploaderHeaders;
        private System.Windows.Forms.ColumnHeader chCustomUploaderHeadersName;
        private System.Windows.Forms.ColumnHeader chCustomUploaderHeadersValue;
        private System.Windows.Forms.Button btnPomfTest;
        private System.Windows.Forms.TextBox txtStreamablePassword;
        private System.Windows.Forms.TextBox txtStreamableUsername;
        private System.Windows.Forms.Label lblStreamableUsername;
        private System.Windows.Forms.Label lblStreamablePassword;
        private System.Windows.Forms.CheckBox cbStreamableAnonymous;
        private System.Windows.Forms.TabControl tcCustomUploaderResponseParse;
        private System.Windows.Forms.TabPage tpCustomUploaderRegexParse;
        private System.Windows.Forms.Button btnCustomUploaderRegexAddSyntax;
        private System.Windows.Forms.TabPage tpCustomUploaderJsonParse;
        private System.Windows.Forms.Label lblCustomUploaderJsonPathExample;
        private System.Windows.Forms.Label lblCustomUploaderJsonPath;
        private System.Windows.Forms.TextBox txtCustomUploaderJsonPath;
        private System.Windows.Forms.TabPage tpCustomUploaderXmlParse;
        private System.Windows.Forms.Button btnCustomUploaderJsonAddSyntax;
        private System.Windows.Forms.Button btnCustomUploadJsonPathHelp;
        private System.Windows.Forms.Button btnCustomUploaderXmlSyntaxAdd;
        private System.Windows.Forms.Button btnCustomUploaderXPathHelp;
        private System.Windows.Forms.Label lblCustomUploaderXPathExample;
        private System.Windows.Forms.Label lblCustomUploaderXPath;
        private System.Windows.Forms.TextBox txtCustomUploaderXPath;
        private System.Windows.Forms.Button btnCustomUploaderRegexHelp;
        private System.Windows.Forms.Label lblSulAPIKey;
        private System.Windows.Forms.TextBox txtSulAPIKey;
        private System.Windows.Forms.Button btnCustomUploadersExportAll;
        private System.Windows.Forms.TextBox txtVgymeUserKey;
        private System.Windows.Forms.Label lvlVgymeUserKey;
        private System.Windows.Forms.LinkLabel llVgymeAccountDetailsPage;
        private System.Windows.Forms.Label lblCheveretoUploaders;
        private System.Windows.Forms.ComboBox cbCheveretoUploaders;
        private System.Windows.Forms.Label lblCheveretoUploadURLExample;
        private System.Windows.Forms.Button btnCheveretoTestAll;
        private System.Windows.Forms.CheckBox cbPastebinRaw;
        private System.Windows.Forms.CheckBox cbGistUseRawURL;
        private System.Windows.Forms.CheckBox cbStreamableUseDirectURL;
        internal System.Windows.Forms.TabPage tpImgur;
        internal System.Windows.Forms.TabPage tpImageShack;
        internal System.Windows.Forms.TabPage tpTinyPic;
        internal System.Windows.Forms.TabPage tpFlickr;
        internal System.Windows.Forms.TabPage tpPhotobucket;
        internal System.Windows.Forms.TabPage tpGooglePhotos;
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
        internal System.Windows.Forms.TabPage tpGe_tt;
        internal System.Windows.Forms.TabPage tpHostr;
        internal System.Windows.Forms.TabPage tpJira;
        internal System.Windows.Forms.TabPage tpLambda;
        internal System.Windows.Forms.TabPage tpLithiio;
        internal System.Windows.Forms.TabPage tpPomf;
        internal System.Windows.Forms.TabPage tpSeafile;
        internal System.Windows.Forms.TabPage tpSul;
        internal System.Windows.Forms.TabPage tpStreamable;
        internal System.Windows.Forms.TabPage tpSharedFolder;
        internal System.Windows.Forms.TabPage tpEmail;
        internal System.Windows.Forms.TabPage tpBitly;
        internal System.Windows.Forms.TabPage tpGoogleURLShortener;
        internal System.Windows.Forms.TabPage tpYourls;
        internal System.Windows.Forms.TabPage tpAdFly;
        internal System.Windows.Forms.TabPage tpCoinURL;
        internal System.Windows.Forms.TabPage tpPolr;
        internal System.Windows.Forms.TabPage tpTwitter;
        internal System.Windows.Forms.TabPage tpCustomUploaders;
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
        private System.Windows.Forms.PictureBox pbPuush;
        private System.Windows.Forms.CheckBox cbHastebinUseFileExtension;
        private System.Windows.Forms.Label lblOwnCloudHostExample;
        internal System.Windows.Forms.TabPage tpPastie;
        private System.Windows.Forms.CheckBox cbPastieIsPublic;
        private System.Windows.Forms.CheckBox cbPolrUseAPIv1;
        private System.Windows.Forms.CheckBox cbPolrIsSecret;
        private HelpersLib.MenuButton mbCustomUploaderDestinationType;
        private System.Windows.Forms.ContextMenuStrip cmsCustomUploaderDestinationType;
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
        private System.Windows.Forms.ComboBox cbxPlikTTLUnit;
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
        private OAuthControl oauth2Gfycat;
        private AccountTypeControl atcGfycatAccountType;
        private System.Windows.Forms.CheckBox cbGfycatIsPublic;
        internal System.Windows.Forms.TabPage tpGfycat;
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
        private System.Windows.Forms.Button btnCustomUploaderDuplicate;
        private System.Windows.Forms.Panel pCustomUploader;
        private System.Windows.Forms.Button btnCustomUploaderURLSharingServiceTest;
        private System.Windows.Forms.ComboBox cbCustomUploaderURLSharingService;
        private System.Windows.Forms.Label lblCustomUploaderURLSharingService;
        private System.Windows.Forms.Label lblCustomUploaderName;
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
    }
}
