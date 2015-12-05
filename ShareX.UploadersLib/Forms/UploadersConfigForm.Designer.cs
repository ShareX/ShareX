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
            this.cbAmazonS3UseRRS = new System.Windows.Forms.CheckBox();
            this.cbAmazonS3CustomCNAME = new System.Windows.Forms.CheckBox();
            this.tpOtherUploaders = new System.Windows.Forms.TabPage();
            this.tcOtherUploaders = new System.Windows.Forms.TabControl();
            this.tpTwitter = new System.Windows.Forms.TabPage();
            this.btnTwitterNameUpdate = new System.Windows.Forms.Button();
            this.lbTwitterAccounts = new System.Windows.Forms.ListBox();
            this.lblTwitterDefaultMessage = new System.Windows.Forms.Label();
            this.txtTwitterDefaultMessage = new System.Windows.Forms.TextBox();
            this.cbTwitterSkipMessageBox = new System.Windows.Forms.CheckBox();
            this.txtTwitterDescription = new System.Windows.Forms.TextBox();
            this.lblTwitterDescription = new System.Windows.Forms.Label();
            this.btnTwitterRemove = new System.Windows.Forms.Button();
            this.btnTwitterAdd = new System.Windows.Forms.Button();
            this.tpCustomUploaders = new System.Windows.Forms.TabPage();
            this.tcCustomUploaderResponseParse = new System.Windows.Forms.TabControl();
            this.tpCustomUploaderRegexParse = new System.Windows.Forms.TabPage();
            this.btnCustomUploaderRegexHelp = new System.Windows.Forms.Button();
            this.btnCustomUploaderRegexAddSyntax = new System.Windows.Forms.Button();
            this.txtCustomUploaderRegexp = new System.Windows.Forms.TextBox();
            this.btnCustomUploaderRegexpUpdate = new System.Windows.Forms.Button();
            this.btnCustomUploaderRegexpAdd = new System.Windows.Forms.Button();
            this.btnCustomUploaderRegexpRemove = new System.Windows.Forms.Button();
            this.lvCustomUploaderRegexps = new ShareX.HelpersLib.MyListView();
            this.lvRegexpsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.btnCustomUploaderExamples = new System.Windows.Forms.Button();
            this.btnCustomUploaderHelp = new System.Windows.Forms.Button();
            this.lblCustomUploaderImageUploader = new System.Windows.Forms.Label();
            this.btnCustomUploaderFileUploaderTest = new System.Windows.Forms.Button();
            this.lblCustomUploaderFileUploader = new System.Windows.Forms.Label();
            this.btnCustomUploaderImageUploaderTest = new System.Windows.Forms.Button();
            this.lblCustomUploaderTestResult = new System.Windows.Forms.Label();
            this.txtCustomUploaderDeletionURL = new System.Windows.Forms.TextBox();
            this.cbCustomUploaderFileUploader = new System.Windows.Forms.ComboBox();
            this.lblCustomUploaderDeletionURL = new System.Windows.Forms.Label();
            this.btnCustomUploaderShowLastResponse = new System.Windows.Forms.Button();
            this.lblCustomUploaderResponseType = new System.Windows.Forms.Label();
            this.cbCustomUploaderURLShortener = new System.Windows.Forms.ComboBox();
            this.gbCustomUploaders = new System.Windows.Forms.GroupBox();
            this.btnCustomUploaderClearUploaders = new System.Windows.Forms.Button();
            this.eiCustomUploaders = new ShareX.HelpersLib.ExportImportControl();
            this.lbCustomUploaderList = new System.Windows.Forms.ListBox();
            this.btnCustomUploaderClear = new System.Windows.Forms.Button();
            this.btnCustomUploaderRemove = new System.Windows.Forms.Button();
            this.btnCustomUploaderUpdate = new System.Windows.Forms.Button();
            this.txtCustomUploaderName = new System.Windows.Forms.TextBox();
            this.btnCustomUploaderAdd = new System.Windows.Forms.Button();
            this.lblCustomUploaderTextUploader = new System.Windows.Forms.Label();
            this.lblCustomUploaderRequestURL = new System.Windows.Forms.Label();
            this.btnCustomUploaderURLShortenerTest = new System.Windows.Forms.Button();
            this.cbCustomUploaderTextUploader = new System.Windows.Forms.ComboBox();
            this.txtCustomUploaderThumbnailURL = new System.Windows.Forms.TextBox();
            this.lblCustomUploaderURLShortener = new System.Windows.Forms.Label();
            this.cbCustomUploaderResponseType = new System.Windows.Forms.ComboBox();
            this.btnCustomUploaderTextUploaderTest = new System.Windows.Forms.Button();
            this.txtCustomUploaderURL = new System.Windows.Forms.TextBox();
            this.cbCustomUploaderImageUploader = new System.Windows.Forms.ComboBox();
            this.txtCustomUploaderRequestURL = new System.Windows.Forms.TextBox();
            this.txtCustomUploaderLog = new System.Windows.Forms.RichTextBox();
            this.lblCustomUploaderThumbnailURL = new System.Windows.Forms.Label();
            this.lblCustomUploaderFileForm = new System.Windows.Forms.Label();
            this.lblCustomUploaderRequestType = new System.Windows.Forms.Label();
            this.cbCustomUploaderRequestType = new System.Windows.Forms.ComboBox();
            this.txtCustomUploaderFileForm = new System.Windows.Forms.TextBox();
            this.lblCustomUploaderURL = new System.Windows.Forms.Label();
            this.tpURLShorteners = new System.Windows.Forms.TabPage();
            this.tcURLShorteners = new System.Windows.Forms.TabControl();
            this.tpBitly = new System.Windows.Forms.TabPage();
            this.txtBitlyDomain = new System.Windows.Forms.TextBox();
            this.lblBitlyDomain = new System.Windows.Forms.Label();
            this.tpGoogleURLShortener = new System.Windows.Forms.TabPage();
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
            this.txtPolrAPIKey = new System.Windows.Forms.TextBox();
            this.lblPolrAPIKey = new System.Windows.Forms.Label();
            this.txtPolrAPIHostname = new System.Windows.Forms.TextBox();
            this.lblPolrAPIHostname = new System.Windows.Forms.Label();
            this.tpFileUploaders = new System.Windows.Forms.TabPage();
            this.tcFileUploaders = new System.Windows.Forms.TabControl();
            this.tpFTP = new System.Windows.Forms.TabPage();
            this.eiFTP = new ShareX.HelpersLib.ExportImportControl();
            this.btnFtpClient = new System.Windows.Forms.Button();
            this.lblFtpFiles = new System.Windows.Forms.Label();
            this.lblFtpText = new System.Windows.Forms.Label();
            this.lblFtpImages = new System.Windows.Forms.Label();
            this.cboFtpImages = new System.Windows.Forms.ComboBox();
            this.cboFtpFiles = new System.Windows.Forms.ComboBox();
            this.cboFtpText = new System.Windows.Forms.ComboBox();
            this.tpDropbox = new System.Windows.Forms.TabPage();
            this.cbDropboxURLType = new System.Windows.Forms.ComboBox();
            this.cbDropboxAutoCreateShareableLink = new System.Windows.Forms.CheckBox();
            this.btnDropboxShowFiles = new System.Windows.Forms.Button();
            this.pbDropboxLogo = new System.Windows.Forms.PictureBox();
            this.lblDropboxStatus = new System.Windows.Forms.Label();
            this.lblDropboxPathTip = new System.Windows.Forms.Label();
            this.lblDropboxPath = new System.Windows.Forms.Label();
            this.txtDropboxPath = new System.Windows.Forms.TextBox();
            this.tpOneDrive = new System.Windows.Forms.TabPage();
            this.tvOneDrive = new System.Windows.Forms.TreeView();
            this.lblOneDriveFolderID = new System.Windows.Forms.Label();
            this.cbOneDriveCreateShareableLink = new System.Windows.Forms.CheckBox();
            this.tpGoogleDrive = new System.Windows.Forms.TabPage();
            this.cbGoogleDriveUseFolder = new System.Windows.Forms.CheckBox();
            this.txtGoogleDriveFolderID = new System.Windows.Forms.TextBox();
            this.lblGoogleDriveFolderID = new System.Windows.Forms.Label();
            this.lvGoogleDriveFoldersList = new ShareX.HelpersLib.MyListView();
            this.chGoogleDriveTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGoogleDriveDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnGoogleDriveRefreshFolders = new System.Windows.Forms.Button();
            this.cbGoogleDriveIsPublic = new System.Windows.Forms.CheckBox();
            this.tpBox = new System.Windows.Forms.TabPage();
            this.lblBoxFolderTip = new System.Windows.Forms.Label();
            this.cbBoxShare = new System.Windows.Forms.CheckBox();
            this.lvBoxFolders = new ShareX.HelpersLib.MyListView();
            this.chBoxFoldersName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblBoxFolderID = new System.Windows.Forms.Label();
            this.btnBoxRefreshFolders = new System.Windows.Forms.Button();
            this.tpCopy = new System.Windows.Forms.TabPage();
            this.pbCopyLogo = new System.Windows.Forms.PictureBox();
            this.lblCopyURLType = new System.Windows.Forms.Label();
            this.cbCopyURLType = new System.Windows.Forms.ComboBox();
            this.lblCopyStatus = new System.Windows.Forms.Label();
            this.lblCopyPath = new System.Windows.Forms.Label();
            this.txtCopyPath = new System.Windows.Forms.TextBox();
            this.tpAmazonS3 = new System.Windows.Forms.TabPage();
            this.txtAmazonS3CustomDomain = new System.Windows.Forms.TextBox();
            this.lblAmazonS3PathPreviewLabel = new System.Windows.Forms.Label();
            this.lblAmazonS3PathPreview = new System.Windows.Forms.Label();
            this.btnAmazonS3BucketNameOpen = new System.Windows.Forms.Button();
            this.btnAmazonS3AccessKeyOpen = new System.Windows.Forms.Button();
            this.cbAmazonS3Endpoint = new System.Windows.Forms.ComboBox();
            this.lblAmazonS3BucketName = new System.Windows.Forms.Label();
            this.txtAmazonS3BucketName = new System.Windows.Forms.TextBox();
            this.lblAmazonS3Endpoint = new System.Windows.Forms.Label();
            this.txtAmazonS3ObjectPrefix = new System.Windows.Forms.TextBox();
            this.lblAmazonS3ObjectPrefix = new System.Windows.Forms.Label();
            this.txtAmazonS3SecretKey = new System.Windows.Forms.TextBox();
            this.lblAmazonS3SecretKey = new System.Windows.Forms.Label();
            this.lblAmazonS3AccessKey = new System.Windows.Forms.Label();
            this.txtAmazonS3AccessKey = new System.Windows.Forms.TextBox();
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
            this.cbOwnCloud81Compatibility = new System.Windows.Forms.CheckBox();
            this.cbOwnCloudIgnoreInvalidCert = new System.Windows.Forms.CheckBox();
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
            this.tpMinus = new System.Windows.Forms.TabPage();
            this.lblMinusURLType = new System.Windows.Forms.Label();
            this.cbMinusURLType = new System.Windows.Forms.ComboBox();
            this.gbMinusUserPass = new System.Windows.Forms.GroupBox();
            this.lblMinusAuthStatus = new System.Windows.Forms.Label();
            this.btnMinusRefreshAuth = new System.Windows.Forms.Button();
            this.lblMinusPassword = new System.Windows.Forms.Label();
            this.lblMinusUsername = new System.Windows.Forms.Label();
            this.txtMinusPassword = new System.Windows.Forms.TextBox();
            this.txtMinusUsername = new System.Windows.Forms.TextBox();
            this.btnMinusAuth = new System.Windows.Forms.Button();
            this.gbMinusUpload = new System.Windows.Forms.GroupBox();
            this.btnMinusReadFolderList = new System.Windows.Forms.Button();
            this.chkMinusPublic = new System.Windows.Forms.CheckBox();
            this.btnMinusFolderAdd = new System.Windows.Forms.Button();
            this.btnMinusFolderRemove = new System.Windows.Forms.Button();
            this.cboMinusFolders = new System.Windows.Forms.ComboBox();
            this.tpJira = new System.Windows.Forms.TabPage();
            this.txtJiraIssuePrefix = new System.Windows.Forms.TextBox();
            this.lblJiraIssuePrefix = new System.Windows.Forms.Label();
            this.gpJiraServer = new System.Windows.Forms.GroupBox();
            this.txtJiraConfigHelp = new System.Windows.Forms.TextBox();
            this.txtJiraHost = new System.Windows.Forms.TextBox();
            this.lblJiraHost = new System.Windows.Forms.Label();
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
            this.tpUp1 = new System.Windows.Forms.TabPage();
            this.txtUp1Key = new System.Windows.Forms.TextBox();
            this.txtUp1Host = new System.Windows.Forms.TextBox();
            this.lblUp1Key = new System.Windows.Forms.Label();
            this.lblUp1Host = new System.Windows.Forms.Label();
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
            this.txtStreamablePassword = new System.Windows.Forms.TextBox();
            this.txtStreamableUsername = new System.Windows.Forms.TextBox();
            this.lblStreamableUsername = new System.Windows.Forms.Label();
            this.lblStreamablePassword = new System.Windows.Forms.Label();
            this.cbStreamableAnonymous = new System.Windows.Forms.CheckBox();
            this.tpEmail = new System.Windows.Forms.TabPage();
            this.chkEmailConfirm = new System.Windows.Forms.CheckBox();
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
            this.tpSharedFolder = new System.Windows.Forms.TabPage();
            this.lblSharedFolderFiles = new System.Windows.Forms.Label();
            this.lblSharedFolderText = new System.Windows.Forms.Label();
            this.cboSharedFolderFiles = new System.Windows.Forms.ComboBox();
            this.lblSharedFolderImages = new System.Windows.Forms.Label();
            this.cboSharedFolderText = new System.Windows.Forms.ComboBox();
            this.cboSharedFolderImages = new System.Windows.Forms.ComboBox();
            this.btnCopyShowFiles = new System.Windows.Forms.Button();
            this.tpTextUploaders = new System.Windows.Forms.TabPage();
            this.tcTextUploaders = new System.Windows.Forms.TabControl();
            this.tpPastebin = new System.Windows.Forms.TabPage();
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
            this.lblPaste_eeUserAPIKey = new System.Windows.Forms.Label();
            this.txtPaste_eeUserAPIKey = new System.Windows.Forms.TextBox();
            this.tpGist = new System.Windows.Forms.TabPage();
            this.chkGistPublishPublic = new System.Windows.Forms.CheckBox();
            this.tpUpaste = new System.Windows.Forms.TabPage();
            this.cbUpasteIsPublic = new System.Windows.Forms.CheckBox();
            this.lblUpasteUserKey = new System.Windows.Forms.Label();
            this.txtUpasteUserKey = new System.Windows.Forms.TextBox();
            this.tpHastebin = new System.Windows.Forms.TabPage();
            this.txtHastebinSyntaxHighlighting = new System.Windows.Forms.TextBox();
            this.txtHastebinCustomDomain = new System.Windows.Forms.TextBox();
            this.lblHastebinSyntaxHighlighting = new System.Windows.Forms.Label();
            this.lblHastebinCustomDomain = new System.Windows.Forms.Label();
            this.tpOneTimeSecret = new System.Windows.Forms.TabPage();
            this.lblOneTimeSecretAPIKey = new System.Windows.Forms.Label();
            this.lblOneTimeSecretEmail = new System.Windows.Forms.Label();
            this.txtOneTimeSecretAPIKey = new System.Windows.Forms.TextBox();
            this.txtOneTimeSecretEmail = new System.Windows.Forms.TextBox();
            this.tpImageUploaders = new System.Windows.Forms.TabPage();
            this.tcImageUploaders = new System.Windows.Forms.TabControl();
            this.tpImgur = new System.Windows.Forms.TabPage();
            this.cbImgurUseGIFV = new System.Windows.Forms.CheckBox();
            this.cbImgurUploadSelectedAlbum = new System.Windows.Forms.CheckBox();
            this.cbImgurDirectLink = new System.Windows.Forms.CheckBox();
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
            this.btnTinyPicLogin = new System.Windows.Forms.Button();
            this.txtTinyPicPassword = new System.Windows.Forms.TextBox();
            this.lblTinyPicPassword = new System.Windows.Forms.Label();
            this.txtTinyPicUsername = new System.Windows.Forms.TextBox();
            this.lblTinyPicUsername = new System.Windows.Forms.Label();
            this.btnTinyPicOpenMyImages = new System.Windows.Forms.Button();
            this.tpFlickr = new System.Windows.Forms.TabPage();
            this.btnFlickrOpenImages = new System.Windows.Forms.Button();
            this.pgFlickrAuthInfo = new System.Windows.Forms.PropertyGrid();
            this.pgFlickrSettings = new System.Windows.Forms.PropertyGrid();
            this.btnFlickrCheckToken = new System.Windows.Forms.Button();
            this.btnFlickrCompleteAuth = new System.Windows.Forms.Button();
            this.btnFlickrOpenAuthorize = new System.Windows.Forms.Button();
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
            this.tpPicasa = new System.Windows.Forms.TabPage();
            this.txtPicasaAlbumID = new System.Windows.Forms.TextBox();
            this.lblPicasaAlbumID = new System.Windows.Forms.Label();
            this.lvPicasaAlbumList = new System.Windows.Forms.ListView();
            this.chPicasaID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPicasaName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPicasaDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnPicasaRefreshAlbumList = new System.Windows.Forms.Button();
            this.tpChevereto = new System.Windows.Forms.TabPage();
            this.cbCheveretoDirectURL = new System.Windows.Forms.CheckBox();
            this.lblCheveretoWebsiteTip = new System.Windows.Forms.Label();
            this.lblCheveretoWebsite = new System.Windows.Forms.Label();
            this.txtCheveretoWebsite = new System.Windows.Forms.TextBox();
            this.txtCheveretoAPIKey = new System.Windows.Forms.TextBox();
            this.lblCheveretoAPIKey = new System.Windows.Forms.Label();
            this.tcUploaders = new System.Windows.Forms.TabControl();
            this.lblWidthHint = new System.Windows.Forms.Label();
            this.ttlvMain = new ShareX.HelpersLib.TabToListView();
            this.atcImgurAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.oauth2Imgur = new ShareX.UploadersLib.OAuthControl();
            this.atcTinyPicAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.oauth2Picasa = new ShareX.UploadersLib.OAuthControl();
            this.oAuth2Gist = new ShareX.UploadersLib.OAuthControl();
            this.atcGistAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.ucFTPAccounts = new ShareX.UploadersLib.AccountsControl();
            this.oauth2Dropbox = new ShareX.UploadersLib.OAuthControl();
            this.oAuth2OneDrive = new ShareX.UploadersLib.OAuthControl();
            this.oauth2GoogleDrive = new ShareX.UploadersLib.OAuthControl();
            this.oauth2Box = new ShareX.UploadersLib.OAuthControl();
            this.oAuthCopy = new ShareX.UploadersLib.OAuthControl();
            this.atcSendSpaceAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.oAuthJira = new ShareX.UploadersLib.OAuthControl();
            this.oauthTwitter = new ShareX.UploadersLib.OAuthControl();
            this.oauth2Bitly = new ShareX.UploadersLib.OAuthControl();
            this.oauth2GoogleURLShortener = new ShareX.UploadersLib.OAuthControl();
            this.atcGoogleURLShortenerAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.ucLocalhostAccounts = new ShareX.UploadersLib.AccountsControl();
            this.actRapidShareAccountType = new ShareX.UploadersLib.AccountTypeControl();
            this.tpOtherUploaders.SuspendLayout();
            this.tcOtherUploaders.SuspendLayout();
            this.tpTwitter.SuspendLayout();
            this.tpCustomUploaders.SuspendLayout();
            this.tcCustomUploaderResponseParse.SuspendLayout();
            this.tpCustomUploaderRegexParse.SuspendLayout();
            this.tpCustomUploaderJsonParse.SuspendLayout();
            this.tpCustomUploaderXmlParse.SuspendLayout();
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
            this.tpDropbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropboxLogo)).BeginInit();
            this.tpOneDrive.SuspendLayout();
            this.tpGoogleDrive.SuspendLayout();
            this.tpBox.SuspendLayout();
            this.tpCopy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCopyLogo)).BeginInit();
            this.tpAmazonS3.SuspendLayout();
            this.tpMega.SuspendLayout();
            this.tpOwnCloud.SuspendLayout();
            this.tpMediaFire.SuspendLayout();
            this.tpPushbullet.SuspendLayout();
            this.tpSendSpace.SuspendLayout();
            this.tpGe_tt.SuspendLayout();
            this.tpHostr.SuspendLayout();
            this.tpMinus.SuspendLayout();
            this.gbMinusUserPass.SuspendLayout();
            this.gbMinusUpload.SuspendLayout();
            this.tpJira.SuspendLayout();
            this.gpJiraServer.SuspendLayout();
            this.tpLambda.SuspendLayout();
            this.tpPomf.SuspendLayout();
            this.tpUp1.SuspendLayout();
            this.tpSeafile.SuspendLayout();
            this.grpSeafileShareSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeafileExpireDays)).BeginInit();
            this.grpSeafileAccInfo.SuspendLayout();
            this.grpSeafileObtainAuthToken.SuspendLayout();
            this.tpStreamable.SuspendLayout();
            this.tpEmail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmailSmtpPort)).BeginInit();
            this.tpSharedFolder.SuspendLayout();
            this.tpTextUploaders.SuspendLayout();
            this.tcTextUploaders.SuspendLayout();
            this.tpPastebin.SuspendLayout();
            this.tpPaste_ee.SuspendLayout();
            this.tpGist.SuspendLayout();
            this.tpUpaste.SuspendLayout();
            this.tpHastebin.SuspendLayout();
            this.tpOneTimeSecret.SuspendLayout();
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
            this.tpPicasa.SuspendLayout();
            this.tpChevereto.SuspendLayout();
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
            this.ttHelpTip.BackColor = System.Drawing.Color.White;
            this.ttHelpTip.InitialDelay = 500;
            this.ttHelpTip.IsBalloon = true;
            this.ttHelpTip.ReshowDelay = 100;
            this.ttHelpTip.UseAnimation = false;
            this.ttHelpTip.UseFading = false;
            // 
            // cbAmazonS3UseRRS
            // 
            resources.ApplyResources(this.cbAmazonS3UseRRS, "cbAmazonS3UseRRS");
            this.cbAmazonS3UseRRS.Name = "cbAmazonS3UseRRS";
            this.ttHelpTip.SetToolTip(this.cbAmazonS3UseRRS, resources.GetString("cbAmazonS3UseRRS.ToolTip"));
            this.cbAmazonS3UseRRS.UseVisualStyleBackColor = true;
            this.cbAmazonS3UseRRS.CheckedChanged += new System.EventHandler(this.cbAmazonS3UseRRS_CheckedChanged);
            // 
            // cbAmazonS3CustomCNAME
            // 
            resources.ApplyResources(this.cbAmazonS3CustomCNAME, "cbAmazonS3CustomCNAME");
            this.cbAmazonS3CustomCNAME.Name = "cbAmazonS3CustomCNAME";
            this.ttHelpTip.SetToolTip(this.cbAmazonS3CustomCNAME, resources.GetString("cbAmazonS3CustomCNAME.ToolTip"));
            this.cbAmazonS3CustomCNAME.UseVisualStyleBackColor = true;
            this.cbAmazonS3CustomCNAME.CheckedChanged += new System.EventHandler(this.cbAmazonS3CustomCNAME_CheckedChanged);
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
            this.tpCustomUploaders.Controls.Add(this.tcCustomUploaderResponseParse);
            this.tpCustomUploaders.Controls.Add(this.tcCustomUploaderArguments);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderExamples);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderHelp);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderImageUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderFileUploaderTest);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderFileUploader);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderImageUploaderTest);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderTestResult);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderDeletionURL);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderFileUploader);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderDeletionURL);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderShowLastResponse);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderResponseType);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderURLShortener);
            this.tpCustomUploaders.Controls.Add(this.gbCustomUploaders);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderTextUploader);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderRequestURL);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderURLShortenerTest);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderTextUploader);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderThumbnailURL);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderURLShortener);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderResponseType);
            this.tpCustomUploaders.Controls.Add(this.btnCustomUploaderTextUploaderTest);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderURL);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderImageUploader);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderRequestURL);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderLog);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderThumbnailURL);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderFileForm);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderRequestType);
            this.tpCustomUploaders.Controls.Add(this.cbCustomUploaderRequestType);
            this.tpCustomUploaders.Controls.Add(this.txtCustomUploaderFileForm);
            this.tpCustomUploaders.Controls.Add(this.lblCustomUploaderURL);
            resources.ApplyResources(this.tpCustomUploaders, "tpCustomUploaders");
            this.tpCustomUploaders.Name = "tpCustomUploaders";
            this.tpCustomUploaders.UseVisualStyleBackColor = true;
            // 
            // tcCustomUploaderResponseParse
            // 
            this.tcCustomUploaderResponseParse.Controls.Add(this.tpCustomUploaderRegexParse);
            this.tcCustomUploaderResponseParse.Controls.Add(this.tpCustomUploaderJsonParse);
            this.tcCustomUploaderResponseParse.Controls.Add(this.tpCustomUploaderXmlParse);
            resources.ApplyResources(this.tcCustomUploaderResponseParse, "tcCustomUploaderResponseParse");
            this.tcCustomUploaderResponseParse.Name = "tcCustomUploaderResponseParse";
            this.tcCustomUploaderResponseParse.SelectedIndex = 0;
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
            // txtCustomUploaderDeletionURL
            // 
            resources.ApplyResources(this.txtCustomUploaderDeletionURL, "txtCustomUploaderDeletionURL");
            this.txtCustomUploaderDeletionURL.Name = "txtCustomUploaderDeletionURL";
            // 
            // cbCustomUploaderFileUploader
            // 
            this.cbCustomUploaderFileUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderFileUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderFileUploader, "cbCustomUploaderFileUploader");
            this.cbCustomUploaderFileUploader.Name = "cbCustomUploaderFileUploader";
            this.cbCustomUploaderFileUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderFileUploader_SelectedIndexChanged);
            // 
            // lblCustomUploaderDeletionURL
            // 
            resources.ApplyResources(this.lblCustomUploaderDeletionURL, "lblCustomUploaderDeletionURL");
            this.lblCustomUploaderDeletionURL.Name = "lblCustomUploaderDeletionURL";
            // 
            // btnCustomUploaderShowLastResponse
            // 
            resources.ApplyResources(this.btnCustomUploaderShowLastResponse, "btnCustomUploaderShowLastResponse");
            this.btnCustomUploaderShowLastResponse.Name = "btnCustomUploaderShowLastResponse";
            this.btnCustomUploaderShowLastResponse.UseVisualStyleBackColor = true;
            this.btnCustomUploaderShowLastResponse.Click += new System.EventHandler(this.btnCustomUploaderShowLastResponse_Click);
            // 
            // lblCustomUploaderResponseType
            // 
            resources.ApplyResources(this.lblCustomUploaderResponseType, "lblCustomUploaderResponseType");
            this.lblCustomUploaderResponseType.Name = "lblCustomUploaderResponseType";
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
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderClearUploaders);
            this.gbCustomUploaders.Controls.Add(this.eiCustomUploaders);
            this.gbCustomUploaders.Controls.Add(this.lbCustomUploaderList);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderClear);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderRemove);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderUpdate);
            this.gbCustomUploaders.Controls.Add(this.txtCustomUploaderName);
            this.gbCustomUploaders.Controls.Add(this.btnCustomUploaderAdd);
            resources.ApplyResources(this.gbCustomUploaders, "gbCustomUploaders");
            this.gbCustomUploaders.Name = "gbCustomUploaders";
            this.gbCustomUploaders.TabStop = false;
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
            // btnCustomUploaderClear
            // 
            resources.ApplyResources(this.btnCustomUploaderClear, "btnCustomUploaderClear");
            this.btnCustomUploaderClear.Name = "btnCustomUploaderClear";
            this.btnCustomUploaderClear.UseVisualStyleBackColor = true;
            this.btnCustomUploaderClear.Click += new System.EventHandler(this.btnCustomUploaderClear_Click);
            // 
            // btnCustomUploaderRemove
            // 
            resources.ApplyResources(this.btnCustomUploaderRemove, "btnCustomUploaderRemove");
            this.btnCustomUploaderRemove.Name = "btnCustomUploaderRemove";
            this.btnCustomUploaderRemove.UseVisualStyleBackColor = true;
            this.btnCustomUploaderRemove.Click += new System.EventHandler(this.btnCustomUploaderRemove_Click);
            // 
            // btnCustomUploaderUpdate
            // 
            resources.ApplyResources(this.btnCustomUploaderUpdate, "btnCustomUploaderUpdate");
            this.btnCustomUploaderUpdate.Name = "btnCustomUploaderUpdate";
            this.btnCustomUploaderUpdate.UseVisualStyleBackColor = true;
            this.btnCustomUploaderUpdate.Click += new System.EventHandler(this.btnCustomUploaderUpdate_Click);
            // 
            // txtCustomUploaderName
            // 
            resources.ApplyResources(this.txtCustomUploaderName, "txtCustomUploaderName");
            this.txtCustomUploaderName.Name = "txtCustomUploaderName";
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
            // lblCustomUploaderRequestURL
            // 
            resources.ApplyResources(this.lblCustomUploaderRequestURL, "lblCustomUploaderRequestURL");
            this.lblCustomUploaderRequestURL.Name = "lblCustomUploaderRequestURL";
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
            // txtCustomUploaderThumbnailURL
            // 
            resources.ApplyResources(this.txtCustomUploaderThumbnailURL, "txtCustomUploaderThumbnailURL");
            this.txtCustomUploaderThumbnailURL.Name = "txtCustomUploaderThumbnailURL";
            // 
            // lblCustomUploaderURLShortener
            // 
            resources.ApplyResources(this.lblCustomUploaderURLShortener, "lblCustomUploaderURLShortener");
            this.lblCustomUploaderURLShortener.Name = "lblCustomUploaderURLShortener";
            // 
            // cbCustomUploaderResponseType
            // 
            this.cbCustomUploaderResponseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderResponseType.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderResponseType, "cbCustomUploaderResponseType");
            this.cbCustomUploaderResponseType.Name = "cbCustomUploaderResponseType";
            // 
            // btnCustomUploaderTextUploaderTest
            // 
            resources.ApplyResources(this.btnCustomUploaderTextUploaderTest, "btnCustomUploaderTextUploaderTest");
            this.btnCustomUploaderTextUploaderTest.Name = "btnCustomUploaderTextUploaderTest";
            this.btnCustomUploaderTextUploaderTest.UseVisualStyleBackColor = true;
            this.btnCustomUploaderTextUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderTextUploaderTest_Click);
            // 
            // txtCustomUploaderURL
            // 
            resources.ApplyResources(this.txtCustomUploaderURL, "txtCustomUploaderURL");
            this.txtCustomUploaderURL.Name = "txtCustomUploaderURL";
            // 
            // cbCustomUploaderImageUploader
            // 
            this.cbCustomUploaderImageUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderImageUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderImageUploader, "cbCustomUploaderImageUploader");
            this.cbCustomUploaderImageUploader.Name = "cbCustomUploaderImageUploader";
            this.cbCustomUploaderImageUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderImageUploader_SelectedIndexChanged);
            // 
            // txtCustomUploaderRequestURL
            // 
            resources.ApplyResources(this.txtCustomUploaderRequestURL, "txtCustomUploaderRequestURL");
            this.txtCustomUploaderRequestURL.Name = "txtCustomUploaderRequestURL";
            // 
            // txtCustomUploaderLog
            // 
            resources.ApplyResources(this.txtCustomUploaderLog, "txtCustomUploaderLog");
            this.txtCustomUploaderLog.Name = "txtCustomUploaderLog";
            this.txtCustomUploaderLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtCustomUploaderLog_LinkClicked);
            // 
            // lblCustomUploaderThumbnailURL
            // 
            resources.ApplyResources(this.lblCustomUploaderThumbnailURL, "lblCustomUploaderThumbnailURL");
            this.lblCustomUploaderThumbnailURL.Name = "lblCustomUploaderThumbnailURL";
            // 
            // lblCustomUploaderFileForm
            // 
            resources.ApplyResources(this.lblCustomUploaderFileForm, "lblCustomUploaderFileForm");
            this.lblCustomUploaderFileForm.Name = "lblCustomUploaderFileForm";
            // 
            // lblCustomUploaderRequestType
            // 
            resources.ApplyResources(this.lblCustomUploaderRequestType, "lblCustomUploaderRequestType");
            this.lblCustomUploaderRequestType.Name = "lblCustomUploaderRequestType";
            // 
            // cbCustomUploaderRequestType
            // 
            this.cbCustomUploaderRequestType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaderRequestType.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaderRequestType, "cbCustomUploaderRequestType");
            this.cbCustomUploaderRequestType.Name = "cbCustomUploaderRequestType";
            this.cbCustomUploaderRequestType.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderRequestType_SelectedIndexChanged);
            // 
            // txtCustomUploaderFileForm
            // 
            resources.ApplyResources(this.txtCustomUploaderFileForm, "txtCustomUploaderFileForm");
            this.txtCustomUploaderFileForm.Name = "txtCustomUploaderFileForm";
            // 
            // lblCustomUploaderURL
            // 
            resources.ApplyResources(this.lblCustomUploaderURL, "lblCustomUploaderURL");
            this.lblCustomUploaderURL.Name = "lblCustomUploaderURL";
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
            // tpGoogleURLShortener
            // 
            this.tpGoogleURLShortener.Controls.Add(this.oauth2GoogleURLShortener);
            this.tpGoogleURLShortener.Controls.Add(this.atcGoogleURLShortenerAccountType);
            resources.ApplyResources(this.tpGoogleURLShortener, "tpGoogleURLShortener");
            this.tpGoogleURLShortener.Name = "tpGoogleURLShortener";
            this.tpGoogleURLShortener.UseVisualStyleBackColor = true;
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
            this.txtCoinURLUUID.TextChanged += new System.EventHandler(this.txtCoinURLUUID_TextChanged);
            // 
            // lblCoinURLUUID
            // 
            resources.ApplyResources(this.lblCoinURLUUID, "lblCoinURLUUID");
            this.lblCoinURLUUID.Name = "lblCoinURLUUID";
            // 
            // tpPolr
            // 
            this.tpPolr.Controls.Add(this.txtPolrAPIKey);
            this.tpPolr.Controls.Add(this.lblPolrAPIKey);
            this.tpPolr.Controls.Add(this.txtPolrAPIHostname);
            this.tpPolr.Controls.Add(this.lblPolrAPIHostname);
            resources.ApplyResources(this.tpPolr, "tpPolr");
            this.tpPolr.Name = "tpPolr";
            this.tpPolr.UseVisualStyleBackColor = true;
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
            this.tcFileUploaders.Controls.Add(this.tpBox);
            this.tcFileUploaders.Controls.Add(this.tpCopy);
            this.tcFileUploaders.Controls.Add(this.tpAmazonS3);
            this.tcFileUploaders.Controls.Add(this.tpMega);
            this.tcFileUploaders.Controls.Add(this.tpOwnCloud);
            this.tcFileUploaders.Controls.Add(this.tpMediaFire);
            this.tcFileUploaders.Controls.Add(this.tpPushbullet);
            this.tcFileUploaders.Controls.Add(this.tpSendSpace);
            this.tcFileUploaders.Controls.Add(this.tpGe_tt);
            this.tcFileUploaders.Controls.Add(this.tpHostr);
            this.tcFileUploaders.Controls.Add(this.tpMinus);
            this.tcFileUploaders.Controls.Add(this.tpJira);
            this.tcFileUploaders.Controls.Add(this.tpLambda);
            this.tcFileUploaders.Controls.Add(this.tpPomf);
            this.tcFileUploaders.Controls.Add(this.tpUp1);
            this.tcFileUploaders.Controls.Add(this.tpSeafile);
            this.tcFileUploaders.Controls.Add(this.tpStreamable);
            this.tcFileUploaders.Controls.Add(this.tpEmail);
            this.tcFileUploaders.Controls.Add(this.tpSharedFolder);
            resources.ApplyResources(this.tcFileUploaders, "tcFileUploaders");
            this.tcFileUploaders.Multiline = true;
            this.tcFileUploaders.Name = "tcFileUploaders";
            this.tcFileUploaders.SelectedIndex = 0;
            // 
            // tpFTP
            // 
            this.tpFTP.Controls.Add(this.eiFTP);
            this.tpFTP.Controls.Add(this.btnFtpClient);
            this.tpFTP.Controls.Add(this.lblFtpFiles);
            this.tpFTP.Controls.Add(this.lblFtpText);
            this.tpFTP.Controls.Add(this.lblFtpImages);
            this.tpFTP.Controls.Add(this.cboFtpImages);
            this.tpFTP.Controls.Add(this.cboFtpFiles);
            this.tpFTP.Controls.Add(this.cboFtpText);
            this.tpFTP.Controls.Add(this.ucFTPAccounts);
            resources.ApplyResources(this.tpFTP, "tpFTP");
            this.tpFTP.Name = "tpFTP";
            this.tpFTP.UseVisualStyleBackColor = true;
            // 
            // eiFTP
            // 
            resources.ApplyResources(this.eiFTP, "eiFTP");
            this.eiFTP.Name = "eiFTP";
            this.eiFTP.ObjectType = null;
            this.eiFTP.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.eiFTP_ExportRequested);
            this.eiFTP.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.eiFTP_ImportRequested);
            // 
            // btnFtpClient
            // 
            resources.ApplyResources(this.btnFtpClient, "btnFtpClient");
            this.btnFtpClient.Name = "btnFtpClient";
            this.btnFtpClient.UseVisualStyleBackColor = true;
            this.btnFtpClient.Click += new System.EventHandler(this.btnFtpClient_Click);
            // 
            // lblFtpFiles
            // 
            resources.ApplyResources(this.lblFtpFiles, "lblFtpFiles");
            this.lblFtpFiles.Name = "lblFtpFiles";
            // 
            // lblFtpText
            // 
            resources.ApplyResources(this.lblFtpText, "lblFtpText");
            this.lblFtpText.Name = "lblFtpText";
            // 
            // lblFtpImages
            // 
            resources.ApplyResources(this.lblFtpImages, "lblFtpImages");
            this.lblFtpImages.Name = "lblFtpImages";
            // 
            // cboFtpImages
            // 
            this.cboFtpImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFtpImages.FormattingEnabled = true;
            resources.ApplyResources(this.cboFtpImages, "cboFtpImages");
            this.cboFtpImages.Name = "cboFtpImages";
            this.cboFtpImages.SelectedIndexChanged += new System.EventHandler(this.cboFtpImages_SelectedIndexChanged);
            // 
            // cboFtpFiles
            // 
            this.cboFtpFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFtpFiles.FormattingEnabled = true;
            resources.ApplyResources(this.cboFtpFiles, "cboFtpFiles");
            this.cboFtpFiles.Name = "cboFtpFiles";
            this.cboFtpFiles.SelectedIndexChanged += new System.EventHandler(this.cboFtpFiles_SelectedIndexChanged);
            // 
            // cboFtpText
            // 
            this.cboFtpText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFtpText.FormattingEnabled = true;
            resources.ApplyResources(this.cboFtpText, "cboFtpText");
            this.cboFtpText.Name = "cboFtpText";
            this.cboFtpText.SelectedIndexChanged += new System.EventHandler(this.cboFtpText_SelectedIndexChanged);
            // 
            // tpDropbox
            // 
            this.tpDropbox.Controls.Add(this.oauth2Dropbox);
            this.tpDropbox.Controls.Add(this.cbDropboxURLType);
            this.tpDropbox.Controls.Add(this.cbDropboxAutoCreateShareableLink);
            this.tpDropbox.Controls.Add(this.btnDropboxShowFiles);
            this.tpDropbox.Controls.Add(this.pbDropboxLogo);
            this.tpDropbox.Controls.Add(this.lblDropboxStatus);
            this.tpDropbox.Controls.Add(this.lblDropboxPathTip);
            this.tpDropbox.Controls.Add(this.lblDropboxPath);
            this.tpDropbox.Controls.Add(this.txtDropboxPath);
            resources.ApplyResources(this.tpDropbox, "tpDropbox");
            this.tpDropbox.Name = "tpDropbox";
            this.tpDropbox.UseVisualStyleBackColor = true;
            // 
            // cbDropboxURLType
            // 
            this.cbDropboxURLType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDropboxURLType.FormattingEnabled = true;
            resources.ApplyResources(this.cbDropboxURLType, "cbDropboxURLType");
            this.cbDropboxURLType.Name = "cbDropboxURLType";
            this.cbDropboxURLType.SelectedIndexChanged += new System.EventHandler(this.cbDropboxURLType_SelectedIndexChanged);
            // 
            // cbDropboxAutoCreateShareableLink
            // 
            resources.ApplyResources(this.cbDropboxAutoCreateShareableLink, "cbDropboxAutoCreateShareableLink");
            this.cbDropboxAutoCreateShareableLink.Name = "cbDropboxAutoCreateShareableLink";
            this.cbDropboxAutoCreateShareableLink.UseVisualStyleBackColor = true;
            this.cbDropboxAutoCreateShareableLink.CheckedChanged += new System.EventHandler(this.cbDropboxAutoCreateShareableLink_CheckedChanged);
            // 
            // btnDropboxShowFiles
            // 
            resources.ApplyResources(this.btnDropboxShowFiles, "btnDropboxShowFiles");
            this.btnDropboxShowFiles.Name = "btnDropboxShowFiles";
            this.btnDropboxShowFiles.UseVisualStyleBackColor = true;
            this.btnDropboxShowFiles.Click += new System.EventHandler(this.btnDropboxShowFiles_Click);
            // 
            // pbDropboxLogo
            // 
            this.pbDropboxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbDropboxLogo, "pbDropboxLogo");
            this.pbDropboxLogo.Name = "pbDropboxLogo";
            this.pbDropboxLogo.TabStop = false;
            this.pbDropboxLogo.Click += new System.EventHandler(this.pbDropboxLogo_Click);
            // 
            // lblDropboxStatus
            // 
            resources.ApplyResources(this.lblDropboxStatus, "lblDropboxStatus");
            this.lblDropboxStatus.Name = "lblDropboxStatus";
            // 
            // lblDropboxPathTip
            // 
            resources.ApplyResources(this.lblDropboxPathTip, "lblDropboxPathTip");
            this.lblDropboxPathTip.Name = "lblDropboxPathTip";
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
            // tpGoogleDrive
            // 
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
            // tpCopy
            // 
            this.tpCopy.Controls.Add(this.pbCopyLogo);
            this.tpCopy.Controls.Add(this.lblCopyURLType);
            this.tpCopy.Controls.Add(this.cbCopyURLType);
            this.tpCopy.Controls.Add(this.lblCopyStatus);
            this.tpCopy.Controls.Add(this.lblCopyPath);
            this.tpCopy.Controls.Add(this.txtCopyPath);
            this.tpCopy.Controls.Add(this.oAuthCopy);
            resources.ApplyResources(this.tpCopy, "tpCopy");
            this.tpCopy.Name = "tpCopy";
            this.tpCopy.UseVisualStyleBackColor = true;
            // 
            // pbCopyLogo
            // 
            this.pbCopyLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbCopyLogo, "pbCopyLogo");
            this.pbCopyLogo.Name = "pbCopyLogo";
            this.pbCopyLogo.TabStop = false;
            this.pbCopyLogo.Click += new System.EventHandler(this.pbCopyLogo_Click);
            // 
            // lblCopyURLType
            // 
            resources.ApplyResources(this.lblCopyURLType, "lblCopyURLType");
            this.lblCopyURLType.Name = "lblCopyURLType";
            // 
            // cbCopyURLType
            // 
            this.cbCopyURLType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCopyURLType.FormattingEnabled = true;
            resources.ApplyResources(this.cbCopyURLType, "cbCopyURLType");
            this.cbCopyURLType.Name = "cbCopyURLType";
            this.cbCopyURLType.SelectedIndexChanged += new System.EventHandler(this.cbCopyURLType_SelectedIndexChanged);
            // 
            // lblCopyStatus
            // 
            resources.ApplyResources(this.lblCopyStatus, "lblCopyStatus");
            this.lblCopyStatus.Name = "lblCopyStatus";
            // 
            // lblCopyPath
            // 
            resources.ApplyResources(this.lblCopyPath, "lblCopyPath");
            this.lblCopyPath.Name = "lblCopyPath";
            // 
            // txtCopyPath
            // 
            resources.ApplyResources(this.txtCopyPath, "txtCopyPath");
            this.txtCopyPath.Name = "txtCopyPath";
            this.txtCopyPath.TextChanged += new System.EventHandler(this.txtCopyPath_TextChanged);
            // 
            // tpAmazonS3
            // 
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3CustomDomain);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3PathPreviewLabel);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3PathPreview);
            this.tpAmazonS3.Controls.Add(this.btnAmazonS3BucketNameOpen);
            this.tpAmazonS3.Controls.Add(this.btnAmazonS3AccessKeyOpen);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3CustomCNAME);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3Endpoint);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3BucketName);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3BucketName);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3Endpoint);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3ObjectPrefix);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3ObjectPrefix);
            this.tpAmazonS3.Controls.Add(this.cbAmazonS3UseRRS);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3SecretKey);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3SecretKey);
            this.tpAmazonS3.Controls.Add(this.lblAmazonS3AccessKey);
            this.tpAmazonS3.Controls.Add(this.txtAmazonS3AccessKey);
            resources.ApplyResources(this.tpAmazonS3, "tpAmazonS3");
            this.tpAmazonS3.Name = "tpAmazonS3";
            this.tpAmazonS3.UseVisualStyleBackColor = true;
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
            // cbAmazonS3Endpoint
            // 
            this.cbAmazonS3Endpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAmazonS3Endpoint.FormattingEnabled = true;
            resources.ApplyResources(this.cbAmazonS3Endpoint, "cbAmazonS3Endpoint");
            this.cbAmazonS3Endpoint.Name = "cbAmazonS3Endpoint";
            this.cbAmazonS3Endpoint.SelectionChangeCommitted += new System.EventHandler(this.cbAmazonS3Endpoint_SelectionChangeCommitted);
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
            // lblAmazonS3Endpoint
            // 
            resources.ApplyResources(this.lblAmazonS3Endpoint, "lblAmazonS3Endpoint");
            this.lblAmazonS3Endpoint.Name = "lblAmazonS3Endpoint";
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
            this.tpOwnCloud.Controls.Add(this.cbOwnCloud81Compatibility);
            this.tpOwnCloud.Controls.Add(this.cbOwnCloudIgnoreInvalidCert);
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
            // cbOwnCloud81Compatibility
            // 
            resources.ApplyResources(this.cbOwnCloud81Compatibility, "cbOwnCloud81Compatibility");
            this.cbOwnCloud81Compatibility.Name = "cbOwnCloud81Compatibility";
            this.cbOwnCloud81Compatibility.UseVisualStyleBackColor = true;
            this.cbOwnCloud81Compatibility.CheckedChanged += new System.EventHandler(this.cbOwnCloud81Compatibility_CheckedChanged);
            // 
            // cbOwnCloudIgnoreInvalidCert
            // 
            resources.ApplyResources(this.cbOwnCloudIgnoreInvalidCert, "cbOwnCloudIgnoreInvalidCert");
            this.cbOwnCloudIgnoreInvalidCert.Name = "cbOwnCloudIgnoreInvalidCert";
            this.cbOwnCloudIgnoreInvalidCert.UseVisualStyleBackColor = true;
            this.cbOwnCloudIgnoreInvalidCert.CheckedChanged += new System.EventHandler(this.cbOwnCloudIgnoreInvalidCert_CheckedChanged);
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
            // tpMinus
            // 
            this.tpMinus.Controls.Add(this.lblMinusURLType);
            this.tpMinus.Controls.Add(this.cbMinusURLType);
            this.tpMinus.Controls.Add(this.gbMinusUserPass);
            this.tpMinus.Controls.Add(this.gbMinusUpload);
            resources.ApplyResources(this.tpMinus, "tpMinus");
            this.tpMinus.Name = "tpMinus";
            this.tpMinus.UseVisualStyleBackColor = true;
            // 
            // lblMinusURLType
            // 
            resources.ApplyResources(this.lblMinusURLType, "lblMinusURLType");
            this.lblMinusURLType.Name = "lblMinusURLType";
            // 
            // cbMinusURLType
            // 
            this.cbMinusURLType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMinusURLType.FormattingEnabled = true;
            resources.ApplyResources(this.cbMinusURLType, "cbMinusURLType");
            this.cbMinusURLType.Name = "cbMinusURLType";
            this.cbMinusURLType.SelectedIndexChanged += new System.EventHandler(this.cbMinusURLType_SelectedIndexChanged);
            // 
            // gbMinusUserPass
            // 
            this.gbMinusUserPass.Controls.Add(this.lblMinusAuthStatus);
            this.gbMinusUserPass.Controls.Add(this.btnMinusRefreshAuth);
            this.gbMinusUserPass.Controls.Add(this.lblMinusPassword);
            this.gbMinusUserPass.Controls.Add(this.lblMinusUsername);
            this.gbMinusUserPass.Controls.Add(this.txtMinusPassword);
            this.gbMinusUserPass.Controls.Add(this.txtMinusUsername);
            this.gbMinusUserPass.Controls.Add(this.btnMinusAuth);
            resources.ApplyResources(this.gbMinusUserPass, "gbMinusUserPass");
            this.gbMinusUserPass.Name = "gbMinusUserPass";
            this.gbMinusUserPass.TabStop = false;
            // 
            // lblMinusAuthStatus
            // 
            resources.ApplyResources(this.lblMinusAuthStatus, "lblMinusAuthStatus");
            this.lblMinusAuthStatus.Name = "lblMinusAuthStatus";
            // 
            // btnMinusRefreshAuth
            // 
            resources.ApplyResources(this.btnMinusRefreshAuth, "btnMinusRefreshAuth");
            this.btnMinusRefreshAuth.Name = "btnMinusRefreshAuth";
            this.btnMinusRefreshAuth.UseVisualStyleBackColor = true;
            this.btnMinusRefreshAuth.Click += new System.EventHandler(this.btnAuthRefresh_Click);
            // 
            // lblMinusPassword
            // 
            resources.ApplyResources(this.lblMinusPassword, "lblMinusPassword");
            this.lblMinusPassword.Name = "lblMinusPassword";
            // 
            // lblMinusUsername
            // 
            resources.ApplyResources(this.lblMinusUsername, "lblMinusUsername");
            this.lblMinusUsername.Name = "lblMinusUsername";
            // 
            // txtMinusPassword
            // 
            resources.ApplyResources(this.txtMinusPassword, "txtMinusPassword");
            this.txtMinusPassword.Name = "txtMinusPassword";
            this.txtMinusPassword.UseSystemPasswordChar = true;
            // 
            // txtMinusUsername
            // 
            resources.ApplyResources(this.txtMinusUsername, "txtMinusUsername");
            this.txtMinusUsername.Name = "txtMinusUsername";
            // 
            // btnMinusAuth
            // 
            resources.ApplyResources(this.btnMinusAuth, "btnMinusAuth");
            this.btnMinusAuth.Name = "btnMinusAuth";
            this.btnMinusAuth.UseVisualStyleBackColor = true;
            this.btnMinusAuth.Click += new System.EventHandler(this.btnMinusAuth_Click);
            // 
            // gbMinusUpload
            // 
            this.gbMinusUpload.Controls.Add(this.btnMinusReadFolderList);
            this.gbMinusUpload.Controls.Add(this.chkMinusPublic);
            this.gbMinusUpload.Controls.Add(this.btnMinusFolderAdd);
            this.gbMinusUpload.Controls.Add(this.btnMinusFolderRemove);
            this.gbMinusUpload.Controls.Add(this.cboMinusFolders);
            resources.ApplyResources(this.gbMinusUpload, "gbMinusUpload");
            this.gbMinusUpload.Name = "gbMinusUpload";
            this.gbMinusUpload.TabStop = false;
            // 
            // btnMinusReadFolderList
            // 
            resources.ApplyResources(this.btnMinusReadFolderList, "btnMinusReadFolderList");
            this.btnMinusReadFolderList.Name = "btnMinusReadFolderList";
            this.btnMinusReadFolderList.UseVisualStyleBackColor = true;
            this.btnMinusReadFolderList.Click += new System.EventHandler(this.btnMinusReadFolderList_Click);
            // 
            // chkMinusPublic
            // 
            resources.ApplyResources(this.chkMinusPublic, "chkMinusPublic");
            this.chkMinusPublic.Name = "chkMinusPublic";
            this.chkMinusPublic.UseVisualStyleBackColor = true;
            // 
            // btnMinusFolderAdd
            // 
            resources.ApplyResources(this.btnMinusFolderAdd, "btnMinusFolderAdd");
            this.btnMinusFolderAdd.Name = "btnMinusFolderAdd";
            this.btnMinusFolderAdd.UseVisualStyleBackColor = true;
            this.btnMinusFolderAdd.Click += new System.EventHandler(this.btnMinusFolderAdd_Click);
            // 
            // btnMinusFolderRemove
            // 
            resources.ApplyResources(this.btnMinusFolderRemove, "btnMinusFolderRemove");
            this.btnMinusFolderRemove.Name = "btnMinusFolderRemove";
            this.btnMinusFolderRemove.UseVisualStyleBackColor = true;
            this.btnMinusFolderRemove.Click += new System.EventHandler(this.btnMinusFolderRemove_Click);
            // 
            // cboMinusFolders
            // 
            this.cboMinusFolders.FormattingEnabled = true;
            resources.ApplyResources(this.cboMinusFolders, "cboMinusFolders");
            this.cboMinusFolders.Name = "cboMinusFolders";
            this.cboMinusFolders.SelectedIndexChanged += new System.EventHandler(this.cboMinusFolders_SelectedIndexChanged);
            // 
            // tpJira
            // 
            this.tpJira.Controls.Add(this.txtJiraIssuePrefix);
            this.tpJira.Controls.Add(this.lblJiraIssuePrefix);
            this.tpJira.Controls.Add(this.gpJiraServer);
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
            // gpJiraServer
            // 
            this.gpJiraServer.Controls.Add(this.txtJiraConfigHelp);
            this.gpJiraServer.Controls.Add(this.txtJiraHost);
            this.gpJiraServer.Controls.Add(this.lblJiraHost);
            resources.ApplyResources(this.gpJiraServer, "gpJiraServer");
            this.gpJiraServer.Name = "gpJiraServer";
            this.gpJiraServer.TabStop = false;
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
            // tpUp1
            // 
            this.tpUp1.Controls.Add(this.txtUp1Key);
            this.tpUp1.Controls.Add(this.txtUp1Host);
            this.tpUp1.Controls.Add(this.lblUp1Key);
            this.tpUp1.Controls.Add(this.lblUp1Host);
            resources.ApplyResources(this.tpUp1, "tpUp1");
            this.tpUp1.Name = "tpUp1";
            this.tpUp1.UseVisualStyleBackColor = true;
            // 
            // txtUp1Key
            // 
            resources.ApplyResources(this.txtUp1Key, "txtUp1Key");
            this.txtUp1Key.Name = "txtUp1Key";
            this.txtUp1Key.TextChanged += new System.EventHandler(this.txtUp1Key_TextChanged);
            // 
            // txtUp1Host
            // 
            resources.ApplyResources(this.txtUp1Host, "txtUp1Host");
            this.txtUp1Host.Name = "txtUp1Host";
            this.txtUp1Host.TextChanged += new System.EventHandler(this.txtUp1Host_TextChanged);
            // 
            // lblUp1Key
            // 
            resources.ApplyResources(this.lblUp1Key, "lblUp1Key");
            this.lblUp1Key.Name = "lblUp1Key";
            // 
            // lblUp1Host
            // 
            resources.ApplyResources(this.lblUp1Host, "lblUp1Host");
            this.lblUp1Host.Name = "lblUp1Host";
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
            this.tpStreamable.Controls.Add(this.txtStreamablePassword);
            this.tpStreamable.Controls.Add(this.txtStreamableUsername);
            this.tpStreamable.Controls.Add(this.lblStreamableUsername);
            this.tpStreamable.Controls.Add(this.lblStreamablePassword);
            this.tpStreamable.Controls.Add(this.cbStreamableAnonymous);
            resources.ApplyResources(this.tpStreamable, "tpStreamable");
            this.tpStreamable.Name = "tpStreamable";
            this.tpStreamable.UseVisualStyleBackColor = true;
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
            // tpEmail
            // 
            this.tpEmail.Controls.Add(this.chkEmailConfirm);
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
            // chkEmailConfirm
            // 
            resources.ApplyResources(this.chkEmailConfirm, "chkEmailConfirm");
            this.chkEmailConfirm.Name = "chkEmailConfirm";
            this.chkEmailConfirm.UseVisualStyleBackColor = true;
            this.chkEmailConfirm.CheckedChanged += new System.EventHandler(this.chkEmailConfirm_CheckedChanged);
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
            resources.ApplyResources(this.tcTextUploaders, "tcTextUploaders");
            this.tcTextUploaders.Name = "tcTextUploaders";
            this.tcTextUploaders.SelectedIndex = 0;
            // 
            // tpPastebin
            // 
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
            this.tpPaste_ee.Controls.Add(this.lblPaste_eeUserAPIKey);
            this.tpPaste_ee.Controls.Add(this.txtPaste_eeUserAPIKey);
            resources.ApplyResources(this.tpPaste_ee, "tpPaste_ee");
            this.tpPaste_ee.Name = "tpPaste_ee";
            this.tpPaste_ee.UseVisualStyleBackColor = true;
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
            this.tpGist.Controls.Add(this.chkGistPublishPublic);
            this.tpGist.Controls.Add(this.oAuth2Gist);
            this.tpGist.Controls.Add(this.atcGistAccountType);
            resources.ApplyResources(this.tpGist, "tpGist");
            this.tpGist.Name = "tpGist";
            this.tpGist.UseVisualStyleBackColor = true;
            // 
            // chkGistPublishPublic
            // 
            resources.ApplyResources(this.chkGistPublishPublic, "chkGistPublishPublic");
            this.chkGistPublishPublic.Name = "chkGistPublishPublic";
            this.chkGistPublishPublic.UseVisualStyleBackColor = true;
            this.chkGistPublishPublic.CheckedChanged += new System.EventHandler(this.chkGistPublishPublic_CheckedChanged);
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
            this.tpHastebin.Controls.Add(this.txtHastebinSyntaxHighlighting);
            this.tpHastebin.Controls.Add(this.txtHastebinCustomDomain);
            this.tpHastebin.Controls.Add(this.lblHastebinSyntaxHighlighting);
            this.tpHastebin.Controls.Add(this.lblHastebinCustomDomain);
            resources.ApplyResources(this.tpHastebin, "tpHastebin");
            this.tpHastebin.Name = "tpHastebin";
            this.tpHastebin.UseVisualStyleBackColor = true;
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
            this.tcImageUploaders.Controls.Add(this.tpPicasa);
            this.tcImageUploaders.Controls.Add(this.tpChevereto);
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
            this.tpFlickr.Controls.Add(this.btnFlickrOpenImages);
            this.tpFlickr.Controls.Add(this.pgFlickrAuthInfo);
            this.tpFlickr.Controls.Add(this.pgFlickrSettings);
            this.tpFlickr.Controls.Add(this.btnFlickrCheckToken);
            this.tpFlickr.Controls.Add(this.btnFlickrCompleteAuth);
            this.tpFlickr.Controls.Add(this.btnFlickrOpenAuthorize);
            resources.ApplyResources(this.tpFlickr, "tpFlickr");
            this.tpFlickr.Name = "tpFlickr";
            this.tpFlickr.UseVisualStyleBackColor = true;
            // 
            // btnFlickrOpenImages
            // 
            resources.ApplyResources(this.btnFlickrOpenImages, "btnFlickrOpenImages");
            this.btnFlickrOpenImages.Name = "btnFlickrOpenImages";
            this.btnFlickrOpenImages.UseVisualStyleBackColor = true;
            this.btnFlickrOpenImages.Click += new System.EventHandler(this.btnFlickrOpenImages_Click);
            // 
            // pgFlickrAuthInfo
            // 
            this.pgFlickrAuthInfo.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgFlickrAuthInfo.CommandsVisibleIfAvailable = false;
            resources.ApplyResources(this.pgFlickrAuthInfo, "pgFlickrAuthInfo");
            this.pgFlickrAuthInfo.Name = "pgFlickrAuthInfo";
            this.pgFlickrAuthInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgFlickrAuthInfo.ToolbarVisible = false;
            // 
            // pgFlickrSettings
            // 
            this.pgFlickrSettings.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgFlickrSettings.CommandsVisibleIfAvailable = false;
            resources.ApplyResources(this.pgFlickrSettings, "pgFlickrSettings");
            this.pgFlickrSettings.Name = "pgFlickrSettings";
            this.pgFlickrSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgFlickrSettings.ToolbarVisible = false;
            // 
            // btnFlickrCheckToken
            // 
            resources.ApplyResources(this.btnFlickrCheckToken, "btnFlickrCheckToken");
            this.btnFlickrCheckToken.Name = "btnFlickrCheckToken";
            this.btnFlickrCheckToken.UseVisualStyleBackColor = true;
            this.btnFlickrCheckToken.Click += new System.EventHandler(this.btnFlickrCheckToken_Click);
            // 
            // btnFlickrCompleteAuth
            // 
            resources.ApplyResources(this.btnFlickrCompleteAuth, "btnFlickrCompleteAuth");
            this.btnFlickrCompleteAuth.Name = "btnFlickrCompleteAuth";
            this.btnFlickrCompleteAuth.UseVisualStyleBackColor = true;
            this.btnFlickrCompleteAuth.Click += new System.EventHandler(this.btnFlickrCompleteAuth_Click);
            // 
            // btnFlickrOpenAuthorize
            // 
            resources.ApplyResources(this.btnFlickrOpenAuthorize, "btnFlickrOpenAuthorize");
            this.btnFlickrOpenAuthorize.Name = "btnFlickrOpenAuthorize";
            this.btnFlickrOpenAuthorize.UseVisualStyleBackColor = true;
            this.btnFlickrOpenAuthorize.Click += new System.EventHandler(this.btnFlickrOpenAuthorize_Click);
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
            // tpPicasa
            // 
            this.tpPicasa.Controls.Add(this.txtPicasaAlbumID);
            this.tpPicasa.Controls.Add(this.lblPicasaAlbumID);
            this.tpPicasa.Controls.Add(this.lvPicasaAlbumList);
            this.tpPicasa.Controls.Add(this.btnPicasaRefreshAlbumList);
            this.tpPicasa.Controls.Add(this.oauth2Picasa);
            resources.ApplyResources(this.tpPicasa, "tpPicasa");
            this.tpPicasa.Name = "tpPicasa";
            this.tpPicasa.UseVisualStyleBackColor = true;
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
            // tpChevereto
            // 
            this.tpChevereto.Controls.Add(this.cbCheveretoDirectURL);
            this.tpChevereto.Controls.Add(this.lblCheveretoWebsiteTip);
            this.tpChevereto.Controls.Add(this.lblCheveretoWebsite);
            this.tpChevereto.Controls.Add(this.txtCheveretoWebsite);
            this.tpChevereto.Controls.Add(this.txtCheveretoAPIKey);
            this.tpChevereto.Controls.Add(this.lblCheveretoAPIKey);
            resources.ApplyResources(this.tpChevereto, "tpChevereto");
            this.tpChevereto.Name = "tpChevereto";
            this.tpChevereto.UseVisualStyleBackColor = true;
            // 
            // cbCheveretoDirectURL
            // 
            resources.ApplyResources(this.cbCheveretoDirectURL, "cbCheveretoDirectURL");
            this.cbCheveretoDirectURL.Name = "cbCheveretoDirectURL";
            this.cbCheveretoDirectURL.UseVisualStyleBackColor = true;
            this.cbCheveretoDirectURL.CheckedChanged += new System.EventHandler(this.cbCheveretoDirectURL_CheckedChanged);
            // 
            // lblCheveretoWebsiteTip
            // 
            resources.ApplyResources(this.lblCheveretoWebsiteTip, "lblCheveretoWebsiteTip");
            this.lblCheveretoWebsiteTip.Name = "lblCheveretoWebsiteTip";
            // 
            // lblCheveretoWebsite
            // 
            resources.ApplyResources(this.lblCheveretoWebsite, "lblCheveretoWebsite");
            this.lblCheveretoWebsite.Name = "lblCheveretoWebsite";
            // 
            // txtCheveretoWebsite
            // 
            resources.ApplyResources(this.txtCheveretoWebsite, "txtCheveretoWebsite");
            this.txtCheveretoWebsite.Name = "txtCheveretoWebsite";
            this.txtCheveretoWebsite.TextChanged += new System.EventHandler(this.txtCheveretoWebsite_TextChanged);
            // 
            // txtCheveretoAPIKey
            // 
            resources.ApplyResources(this.txtCheveretoAPIKey, "txtCheveretoAPIKey");
            this.txtCheveretoAPIKey.Name = "txtCheveretoAPIKey";
            this.txtCheveretoAPIKey.TextChanged += new System.EventHandler(this.txtCheveretoAPIKey_TextChanged);
            // 
            // lblCheveretoAPIKey
            // 
            resources.ApplyResources(this.lblCheveretoAPIKey, "lblCheveretoAPIKey");
            this.lblCheveretoAPIKey.Name = "lblCheveretoAPIKey";
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
            this.lblWidthHint.BackColor = System.Drawing.Color.Black;
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
            // atcTinyPicAccountType
            // 
            resources.ApplyResources(this.atcTinyPicAccountType, "atcTinyPicAccountType");
            this.atcTinyPicAccountType.Name = "atcTinyPicAccountType";
            this.atcTinyPicAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcTinyPicAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcTinyPicAccountType_AccountTypeChanged);
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
            // ucFTPAccounts
            // 
            resources.ApplyResources(this.ucFTPAccounts, "ucFTPAccounts");
            this.ucFTPAccounts.Name = "ucFTPAccounts";
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
            // oAuth2OneDrive
            // 
            resources.ApplyResources(this.oAuth2OneDrive, "oAuth2OneDrive");
            this.oAuth2OneDrive.Name = "oAuth2OneDrive";
            this.oAuth2OneDrive.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oAuth2OneDrive_OpenButtonClicked);
            this.oAuth2OneDrive.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oAuth2OneDrive_CompleteButtonClicked);
            this.oAuth2OneDrive.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oAuth2OneDrive_ClearButtonClicked);
            this.oAuth2OneDrive.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oAuth2OneDrive_RefreshButtonClicked);
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
            // oauth2Box
            // 
            resources.ApplyResources(this.oauth2Box, "oauth2Box");
            this.oauth2Box.Name = "oauth2Box";
            this.oauth2Box.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauth2Box_OpenButtonClicked);
            this.oauth2Box.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauth2Box_CompleteButtonClicked);
            this.oauth2Box.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauth2Box_ClearButtonClicked);
            this.oauth2Box.RefreshButtonClicked += new ShareX.UploadersLib.OAuthControl.RefreshButtonClickedEventHandler(this.oauth2Box_RefreshButtonClicked);
            // 
            // oAuthCopy
            // 
            this.oAuthCopy.IsRefreshable = false;
            resources.ApplyResources(this.oAuthCopy, "oAuthCopy");
            this.oAuthCopy.Name = "oAuthCopy";
            this.oAuthCopy.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oAuthCopy_OpenButtonClicked);
            this.oAuthCopy.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oAuthCopy_CompleteButtonClicked);
            this.oAuthCopy.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oAuthCopy_ClearButtonClicked);
            // 
            // atcSendSpaceAccountType
            // 
            resources.ApplyResources(this.atcSendSpaceAccountType, "atcSendSpaceAccountType");
            this.atcSendSpaceAccountType.Name = "atcSendSpaceAccountType";
            this.atcSendSpaceAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            this.atcSendSpaceAccountType.AccountTypeChanged += new ShareX.UploadersLib.AccountTypeControl.AccountTypeChangedEventHandler(this.atcSendSpaceAccountType_AccountTypeChanged);
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
            // oauthTwitter
            // 
            resources.ApplyResources(this.oauthTwitter, "oauthTwitter");
            this.oauthTwitter.IsRefreshable = false;
            this.oauthTwitter.Name = "oauthTwitter";
            this.oauthTwitter.OpenButtonClicked += new ShareX.UploadersLib.OAuthControl.OpenButtonClickedEventHandler(this.oauthTwitter_OpenButtonClicked);
            this.oauthTwitter.CompleteButtonClicked += new ShareX.UploadersLib.OAuthControl.CompleteButtonClickedEventHandler(this.oauthTwitter_CompleteButtonClicked);
            this.oauthTwitter.ClearButtonClicked += new ShareX.UploadersLib.OAuthControl.ClearButtonclickedEventHandler(this.oauthTwitter_ClearButtonClicked);
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
            // ucLocalhostAccounts
            // 
            resources.ApplyResources(this.ucLocalhostAccounts, "ucLocalhostAccounts");
            this.ucLocalhostAccounts.Name = "ucLocalhostAccounts";
            // 
            // actRapidShareAccountType
            // 
            resources.ApplyResources(this.actRapidShareAccountType, "actRapidShareAccountType");
            this.actRapidShareAccountType.Name = "actRapidShareAccountType";
            this.actRapidShareAccountType.SelectedAccountType = ShareX.UploadersLib.AccountType.Anonymous;
            // 
            // UploadersConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
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
            this.tcCustomUploaderResponseParse.ResumeLayout(false);
            this.tpCustomUploaderRegexParse.ResumeLayout(false);
            this.tpCustomUploaderRegexParse.PerformLayout();
            this.tpCustomUploaderJsonParse.ResumeLayout(false);
            this.tpCustomUploaderJsonParse.PerformLayout();
            this.tpCustomUploaderXmlParse.ResumeLayout(false);
            this.tpCustomUploaderXmlParse.PerformLayout();
            this.tcCustomUploaderArguments.ResumeLayout(false);
            this.tpCustomUploaderArguments.ResumeLayout(false);
            this.tpCustomUploaderArguments.PerformLayout();
            this.tpCustomUploaderHeaders.ResumeLayout(false);
            this.tpCustomUploaderHeaders.PerformLayout();
            this.gbCustomUploaders.ResumeLayout(false);
            this.gbCustomUploaders.PerformLayout();
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
            this.tpDropbox.ResumeLayout(false);
            this.tpDropbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropboxLogo)).EndInit();
            this.tpOneDrive.ResumeLayout(false);
            this.tpOneDrive.PerformLayout();
            this.tpGoogleDrive.ResumeLayout(false);
            this.tpGoogleDrive.PerformLayout();
            this.tpBox.ResumeLayout(false);
            this.tpBox.PerformLayout();
            this.tpCopy.ResumeLayout(false);
            this.tpCopy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCopyLogo)).EndInit();
            this.tpAmazonS3.ResumeLayout(false);
            this.tpAmazonS3.PerformLayout();
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
            this.tpMinus.ResumeLayout(false);
            this.tpMinus.PerformLayout();
            this.gbMinusUserPass.ResumeLayout(false);
            this.gbMinusUserPass.PerformLayout();
            this.gbMinusUpload.ResumeLayout(false);
            this.gbMinusUpload.PerformLayout();
            this.tpJira.ResumeLayout(false);
            this.tpJira.PerformLayout();
            this.gpJiraServer.ResumeLayout(false);
            this.gpJiraServer.PerformLayout();
            this.tpLambda.ResumeLayout(false);
            this.tpLambda.PerformLayout();
            this.tpPomf.ResumeLayout(false);
            this.tpPomf.PerformLayout();
            this.tpUp1.ResumeLayout(false);
            this.tpUp1.PerformLayout();
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
            this.tpEmail.ResumeLayout(false);
            this.tpEmail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmailSmtpPort)).EndInit();
            this.tpSharedFolder.ResumeLayout(false);
            this.tpSharedFolder.PerformLayout();
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
            this.tpImageUploaders.ResumeLayout(false);
            this.tcImageUploaders.ResumeLayout(false);
            this.tpImgur.ResumeLayout(false);
            this.tpImgur.PerformLayout();
            this.tpImageShack.ResumeLayout(false);
            this.tpImageShack.PerformLayout();
            this.tpTinyPic.ResumeLayout(false);
            this.tpTinyPic.PerformLayout();
            this.tpFlickr.ResumeLayout(false);
            this.tpPhotobucket.ResumeLayout(false);
            this.gbPhotobucketAlbumPath.ResumeLayout(false);
            this.gbPhotobucketAlbumPath.PerformLayout();
            this.gbPhotobucketAlbums.ResumeLayout(false);
            this.gbPhotobucketAlbums.PerformLayout();
            this.gbPhotobucketUserAccount.ResumeLayout(false);
            this.gbPhotobucketUserAccount.PerformLayout();
            this.tpPicasa.ResumeLayout(false);
            this.tpPicasa.PerformLayout();
            this.tpChevereto.ResumeLayout(false);
            this.tpChevereto.PerformLayout();
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
        private System.Windows.Forms.TabPage tpTwitter;
        private System.Windows.Forms.TabPage tpCustomUploaders;
        private System.Windows.Forms.Button btnCustomUploaderHelp;
        private System.Windows.Forms.Label lblCustomUploaderImageUploader;
        internal System.Windows.Forms.Button btnCustomUploaderFileUploaderTest;
        private System.Windows.Forms.Label lblCustomUploaderFileUploader;
        internal System.Windows.Forms.Button btnCustomUploaderImageUploaderTest;
        private System.Windows.Forms.Label lblCustomUploaderTestResult;
        internal System.Windows.Forms.TextBox txtCustomUploaderDeletionURL;
        private System.Windows.Forms.ComboBox cbCustomUploaderFileUploader;
        internal System.Windows.Forms.Label lblCustomUploaderDeletionURL;
        private System.Windows.Forms.Button btnCustomUploaderShowLastResponse;
        private System.Windows.Forms.Label lblCustomUploaderResponseType;
        private System.Windows.Forms.ComboBox cbCustomUploaderURLShortener;
        internal System.Windows.Forms.GroupBox gbCustomUploaders;
        internal System.Windows.Forms.ListBox lbCustomUploaderList;
        internal System.Windows.Forms.Button btnCustomUploaderRemove;
        internal System.Windows.Forms.Button btnCustomUploaderClear;
        internal System.Windows.Forms.Button btnCustomUploaderUpdate;
        internal System.Windows.Forms.TextBox txtCustomUploaderName;
        internal System.Windows.Forms.Button btnCustomUploaderAdd;
        private System.Windows.Forms.Label lblCustomUploaderTextUploader;
        internal System.Windows.Forms.Label lblCustomUploaderRequestURL;
        internal System.Windows.Forms.Button btnCustomUploaderURLShortenerTest;
        internal System.Windows.Forms.Button btnCustomUploaderRegexpUpdate;
        internal System.Windows.Forms.TextBox txtCustomUploaderRegexp;
        internal ShareX.HelpersLib.MyListView lvCustomUploaderRegexps;
        internal System.Windows.Forms.ColumnHeader lvRegexpsColumn;
        internal System.Windows.Forms.Button btnCustomUploaderRegexpRemove;
        internal System.Windows.Forms.Button btnCustomUploaderRegexpAdd;
        private System.Windows.Forms.ComboBox cbCustomUploaderTextUploader;
        internal System.Windows.Forms.TextBox txtCustomUploaderThumbnailURL;
        private System.Windows.Forms.Label lblCustomUploaderURLShortener;
        private System.Windows.Forms.ComboBox cbCustomUploaderResponseType;
        internal System.Windows.Forms.Button btnCustomUploaderTextUploaderTest;
        internal System.Windows.Forms.TextBox txtCustomUploaderURL;
        private System.Windows.Forms.ComboBox cbCustomUploaderImageUploader;
        internal System.Windows.Forms.TextBox txtCustomUploaderRequestURL;
        internal System.Windows.Forms.RichTextBox txtCustomUploaderLog;
        internal System.Windows.Forms.Label lblCustomUploaderThumbnailURL;
        internal System.Windows.Forms.Label lblCustomUploaderFileForm;
        private System.Windows.Forms.Label lblCustomUploaderRequestType;
        private System.Windows.Forms.ComboBox cbCustomUploaderRequestType;
        internal System.Windows.Forms.TextBox txtCustomUploaderFileForm;
        internal System.Windows.Forms.Label lblCustomUploaderURL;
        internal System.Windows.Forms.Button btnCustomUploaderArgUpdate;
        internal System.Windows.Forms.TextBox txtCustomUploaderArgValue;
        internal System.Windows.Forms.Button btnCustomUploaderArgRemove;
        internal ShareX.HelpersLib.MyListView lvCustomUploaderArguments;
        internal System.Windows.Forms.ColumnHeader chCustomUploaderArgumentsName;
        internal System.Windows.Forms.ColumnHeader chCustomUploaderArgumentsValue;
        internal System.Windows.Forms.Button btnCustomUploaderArgAdd;
        internal System.Windows.Forms.TextBox txtCustomUploaderArgName;
        private System.Windows.Forms.TabPage tpURLShorteners;
        private System.Windows.Forms.TabControl tcURLShorteners;
        private System.Windows.Forms.TabPage tpBitly;
        private OAuthControl oauth2Bitly;
        private System.Windows.Forms.TabPage tpGoogleURLShortener;
        private OAuthControl oauth2GoogleURLShortener;
        private AccountTypeControl atcGoogleURLShortenerAccountType;
        private System.Windows.Forms.TabPage tpYourls;
        private System.Windows.Forms.TextBox txtYourlsPassword;
        private System.Windows.Forms.TextBox txtYourlsUsername;
        private System.Windows.Forms.TextBox txtYourlsSignature;
        private System.Windows.Forms.Label lblYourlsNote;
        private System.Windows.Forms.Label lblYourlsPassword;
        private System.Windows.Forms.Label lblYourlsUsername;
        private System.Windows.Forms.Label lblYourlsSignature;
        private System.Windows.Forms.TextBox txtYourlsAPIURL;
        private System.Windows.Forms.Label lblYourlsAPIURL;
        public System.Windows.Forms.TabPage tpFileUploaders;
        public System.Windows.Forms.TabControl tcFileUploaders;
        private System.Windows.Forms.TabPage tpDropbox;
        private System.Windows.Forms.ComboBox cbDropboxURLType;
        private System.Windows.Forms.CheckBox cbDropboxAutoCreateShareableLink;
        private System.Windows.Forms.Button btnDropboxShowFiles;
        private System.Windows.Forms.PictureBox pbDropboxLogo;
        private System.Windows.Forms.Label lblDropboxStatus;
        private System.Windows.Forms.Label lblDropboxPathTip;
        private System.Windows.Forms.Label lblDropboxPath;
        private System.Windows.Forms.TextBox txtDropboxPath;
        private System.Windows.Forms.TabPage tpCopy;
        private System.Windows.Forms.Label lblCopyURLType;
        private System.Windows.Forms.ComboBox cbCopyURLType;
        private OAuthControl oAuthCopy;
        private System.Windows.Forms.Button btnCopyShowFiles;
        private System.Windows.Forms.PictureBox pbCopyLogo;
        private System.Windows.Forms.Label lblCopyStatus;
        private System.Windows.Forms.Label lblCopyPath;
        private System.Windows.Forms.TextBox txtCopyPath;
        public System.Windows.Forms.TabPage tpFTP;
        private System.Windows.Forms.Button btnFtpClient;
        private System.Windows.Forms.Label lblFtpFiles;
        private System.Windows.Forms.Label lblFtpText;
        private System.Windows.Forms.Label lblFtpImages;
        private System.Windows.Forms.ComboBox cboFtpImages;
        private System.Windows.Forms.ComboBox cboFtpFiles;
        private System.Windows.Forms.ComboBox cboFtpText;
        private AccountsControl ucFTPAccounts;
        private System.Windows.Forms.TabPage tpMega;
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
        private System.Windows.Forms.TabPage tpAmazonS3;
        private System.Windows.Forms.TextBox txtAmazonS3CustomDomain;
        private System.Windows.Forms.Label lblAmazonS3PathPreviewLabel;
        private System.Windows.Forms.Label lblAmazonS3PathPreview;
        private System.Windows.Forms.Button btnAmazonS3BucketNameOpen;
        private System.Windows.Forms.Button btnAmazonS3AccessKeyOpen;
        private System.Windows.Forms.CheckBox cbAmazonS3CustomCNAME;
        private System.Windows.Forms.ComboBox cbAmazonS3Endpoint;
        private System.Windows.Forms.Label lblAmazonS3BucketName;
        private System.Windows.Forms.TextBox txtAmazonS3BucketName;
        private System.Windows.Forms.Label lblAmazonS3Endpoint;
        private System.Windows.Forms.TextBox txtAmazonS3ObjectPrefix;
        private System.Windows.Forms.Label lblAmazonS3ObjectPrefix;
        private System.Windows.Forms.CheckBox cbAmazonS3UseRRS;
        private System.Windows.Forms.TextBox txtAmazonS3SecretKey;
        private System.Windows.Forms.Label lblAmazonS3SecretKey;
        private System.Windows.Forms.Label lblAmazonS3AccessKey;
        private System.Windows.Forms.TextBox txtAmazonS3AccessKey;
        private System.Windows.Forms.TabPage tpPushbullet;
        private System.Windows.Forms.Label lblPushbulletDevices;
        private System.Windows.Forms.ComboBox cboPushbulletDevices;
        private System.Windows.Forms.Button btnPushbulletGetDeviceList;
        private System.Windows.Forms.Label lblPushbulletUserKey;
        private System.Windows.Forms.TextBox txtPushbulletUserKey;
        private System.Windows.Forms.TabPage tpGoogleDrive;
        private System.Windows.Forms.CheckBox cbGoogleDriveIsPublic;
        private OAuthControl oauth2GoogleDrive;
        private System.Windows.Forms.TabPage tpBox;
        private System.Windows.Forms.Label lblBoxFolderTip;
        private System.Windows.Forms.CheckBox cbBoxShare;
        private ShareX.HelpersLib.MyListView lvBoxFolders;
        private System.Windows.Forms.ColumnHeader chBoxFoldersName;
        private System.Windows.Forms.Label lblBoxFolderID;
        private System.Windows.Forms.Button btnBoxRefreshFolders;
        private OAuthControl oauth2Box;
        private System.Windows.Forms.TabPage tpSendSpace;
        private System.Windows.Forms.Button btnSendSpaceRegister;
        private System.Windows.Forms.Label lblSendSpacePassword;
        private System.Windows.Forms.Label lblSendSpaceUsername;
        private System.Windows.Forms.TextBox txtSendSpacePassword;
        private System.Windows.Forms.TextBox txtSendSpaceUserName;
        private AccountTypeControl atcSendSpaceAccountType;
        private System.Windows.Forms.TabPage tpGe_tt;
        private System.Windows.Forms.Label lblGe_ttStatus;
        private System.Windows.Forms.Label lblGe_ttPassword;
        private System.Windows.Forms.Label lblGe_ttEmail;
        private System.Windows.Forms.Button btnGe_ttLogin;
        private System.Windows.Forms.TextBox txtGe_ttPassword;
        private System.Windows.Forms.TextBox txtGe_ttEmail;
        private System.Windows.Forms.TabPage tpHostr;
        private System.Windows.Forms.CheckBox cbLocalhostrDirectURL;
        private System.Windows.Forms.Label lblLocalhostrPassword;
        private System.Windows.Forms.Label lblLocalhostrEmail;
        private System.Windows.Forms.TextBox txtLocalhostrPassword;
        private System.Windows.Forms.TextBox txtLocalhostrEmail;
        private System.Windows.Forms.TabPage tpMinus;
        private System.Windows.Forms.Label lblMinusURLType;
        private System.Windows.Forms.ComboBox cbMinusURLType;
        private System.Windows.Forms.GroupBox gbMinusUserPass;
        private System.Windows.Forms.Label lblMinusAuthStatus;
        private System.Windows.Forms.Button btnMinusRefreshAuth;
        private System.Windows.Forms.Label lblMinusPassword;
        private System.Windows.Forms.Label lblMinusUsername;
        private System.Windows.Forms.TextBox txtMinusPassword;
        private System.Windows.Forms.TextBox txtMinusUsername;
        private System.Windows.Forms.Button btnMinusAuth;
        private System.Windows.Forms.GroupBox gbMinusUpload;
        private System.Windows.Forms.Button btnMinusReadFolderList;
        private System.Windows.Forms.CheckBox chkMinusPublic;
        private System.Windows.Forms.Button btnMinusFolderAdd;
        private System.Windows.Forms.Button btnMinusFolderRemove;
        private System.Windows.Forms.ComboBox cboMinusFolders;
        private System.Windows.Forms.TabPage tpJira;
        private System.Windows.Forms.TextBox txtJiraIssuePrefix;
        private System.Windows.Forms.Label lblJiraIssuePrefix;
        private System.Windows.Forms.GroupBox gpJiraServer;
        private System.Windows.Forms.TextBox txtJiraConfigHelp;
        private System.Windows.Forms.TextBox txtJiraHost;
        private System.Windows.Forms.Label lblJiraHost;
        private OAuthControl oAuthJira;
        private System.Windows.Forms.TabPage tpEmail;
        private System.Windows.Forms.CheckBox chkEmailConfirm;
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
        private System.Windows.Forms.TabPage tpSharedFolder;
        internal AccountsControl ucLocalhostAccounts;
        private System.Windows.Forms.Label lblSharedFolderFiles;
        private System.Windows.Forms.Label lblSharedFolderText;
        private System.Windows.Forms.Label lblSharedFolderImages;
        private System.Windows.Forms.ComboBox cboSharedFolderFiles;
        private System.Windows.Forms.ComboBox cboSharedFolderText;
        private System.Windows.Forms.ComboBox cboSharedFolderImages;
        private System.Windows.Forms.TabPage tpTextUploaders;
        private System.Windows.Forms.TabControl tcTextUploaders;
        private System.Windows.Forms.TabPage tpPastebin;
        private System.Windows.Forms.Button btnPastebinLogin;
        private System.Windows.Forms.TabPage tpPaste_ee;
        private System.Windows.Forms.Label lblPaste_eeUserAPIKey;
        private System.Windows.Forms.TextBox txtPaste_eeUserAPIKey;
        private System.Windows.Forms.TabPage tpGist;
        private System.Windows.Forms.CheckBox chkGistPublishPublic;
        private OAuthControl oAuth2Gist;
        private AccountTypeControl atcGistAccountType;
        private System.Windows.Forms.TabPage tpUpaste;
        private System.Windows.Forms.CheckBox cbUpasteIsPublic;
        private System.Windows.Forms.Label lblUpasteUserKey;
        private System.Windows.Forms.TextBox txtUpasteUserKey;
        private System.Windows.Forms.TabPage tpImageUploaders;
        private System.Windows.Forms.TabControl tcImageUploaders;
        private System.Windows.Forms.TabPage tpImgur;
        private OAuthControl oauth2Imgur;
        private System.Windows.Forms.ListView lvImgurAlbumList;
        private System.Windows.Forms.ColumnHeader chImgurID;
        private System.Windows.Forms.ColumnHeader chImgurTitle;
        private System.Windows.Forms.ColumnHeader chImgurDescription;
        private System.Windows.Forms.Button btnImgurRefreshAlbumList;
        private System.Windows.Forms.ComboBox cbImgurThumbnailType;
        private System.Windows.Forms.Label lblImgurThumbnailType;
        private AccountTypeControl atcImgurAccountType;
        private System.Windows.Forms.TabPage tpImageShack;
        private System.Windows.Forms.Button btnImageShackLogin;
        internal System.Windows.Forms.Button btnImageShackOpenPublicProfile;
        private System.Windows.Forms.CheckBox cbImageShackIsPublic;
        internal System.Windows.Forms.Button btnImageShackOpenMyImages;
        internal System.Windows.Forms.Label lblImageShackUsername;
        private System.Windows.Forms.TextBox txtImageShackUsername;
        private System.Windows.Forms.TextBox txtImageShackPassword;
        internal System.Windows.Forms.Label lblImageShackPassword;
        private System.Windows.Forms.TabPage tpTinyPic;
        private AccountTypeControl atcTinyPicAccountType;
        private System.Windows.Forms.Button btnTinyPicLogin;
        private System.Windows.Forms.TextBox txtTinyPicPassword;
        private System.Windows.Forms.Label lblTinyPicPassword;
        private System.Windows.Forms.TextBox txtTinyPicUsername;
        private System.Windows.Forms.Label lblTinyPicUsername;
        internal System.Windows.Forms.Button btnTinyPicOpenMyImages;
        private System.Windows.Forms.TabPage tpFlickr;
        private System.Windows.Forms.Button btnFlickrOpenImages;
        private System.Windows.Forms.PropertyGrid pgFlickrAuthInfo;
        private System.Windows.Forms.PropertyGrid pgFlickrSettings;
        private System.Windows.Forms.Button btnFlickrCheckToken;
        private System.Windows.Forms.Button btnFlickrCompleteAuth;
        private System.Windows.Forms.Button btnFlickrOpenAuthorize;
        private System.Windows.Forms.TabPage tpPhotobucket;
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
        private System.Windows.Forms.TabPage tpPicasa;
        private System.Windows.Forms.TextBox txtPicasaAlbumID;
        private System.Windows.Forms.Label lblPicasaAlbumID;
        private System.Windows.Forms.ListView lvPicasaAlbumList;
        private System.Windows.Forms.ColumnHeader chPicasaID;
        private System.Windows.Forms.ColumnHeader chPicasaName;
        private System.Windows.Forms.ColumnHeader chPicasaDescription;
        private System.Windows.Forms.Button btnPicasaRefreshAlbumList;
        private OAuthControl oauth2Picasa;
        public System.Windows.Forms.TabControl tcUploaders;
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
        private System.Windows.Forms.TabPage tpOwnCloud;
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
        private System.Windows.Forms.TabPage tpAdFly;
        private System.Windows.Forms.TextBox txtAdflyAPIUID;
        private System.Windows.Forms.Label lblAdflyAPIUID;
        private System.Windows.Forms.TextBox txtAdflyAPIKEY;
        private System.Windows.Forms.Label lblAdflyAPIKEY;
        private System.Windows.Forms.LinkLabel llAdflyLink;
        private System.Windows.Forms.CheckBox cbImgurDirectLink;
        private System.Windows.Forms.TabPage tpMediaFire;
        private System.Windows.Forms.TextBox txtMediaFirePassword;
        private System.Windows.Forms.TextBox txtMediaFireEmail;
        private System.Windows.Forms.Label lblMediaFirePassword;
        private System.Windows.Forms.Label lblMediaFireEmail;
        private System.Windows.Forms.TextBox txtMediaFirePath;
        private System.Windows.Forms.Label lblMediaFirePath;
        private System.Windows.Forms.CheckBox cbMediaFireUseLongLink;
        private System.Windows.Forms.TabPage tpOneDrive;
        private OAuthControl oAuth2OneDrive;
        private System.Windows.Forms.CheckBox cbOwnCloudIgnoreInvalidCert;
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
        private System.Windows.Forms.TabPage tpChevereto;
        private System.Windows.Forms.Label lblCheveretoWebsiteTip;
        private System.Windows.Forms.Label lblCheveretoWebsite;
        private System.Windows.Forms.TextBox txtCheveretoWebsite;
        private System.Windows.Forms.TextBox txtCheveretoAPIKey;
        private System.Windows.Forms.Label lblCheveretoAPIKey;
        private System.Windows.Forms.CheckBox cbCheveretoDirectURL;
        private System.Windows.Forms.ComboBox cbPastebinSyntax;
        private System.Windows.Forms.TabPage tpHastebin;
        private System.Windows.Forms.TextBox txtHastebinSyntaxHighlighting;
        private System.Windows.Forms.TextBox txtHastebinCustomDomain;
        private System.Windows.Forms.Label lblHastebinSyntaxHighlighting;
        private System.Windows.Forms.Label lblHastebinCustomDomain;
        private System.Windows.Forms.CheckBox cbOneDriveCreateShareableLink;
        private System.Windows.Forms.Label lblOneDriveFolderID;
        private System.Windows.Forms.TreeView tvOneDrive;
        private System.Windows.Forms.TabPage tpLambda;
        private System.Windows.Forms.Label lblLambdaApiKey;
        private System.Windows.Forms.TextBox txtLambdaApiKey;
        private System.Windows.Forms.Label lblLambdaInfo;
        private System.Windows.Forms.Label lblLambdaUploadURL;
        private System.Windows.Forms.ComboBox cbLambdaUploadURL;
        private OAuthControl oauthTwitter;
        private System.Windows.Forms.TextBox txtTwitterDescription;
        private System.Windows.Forms.Label lblTwitterDescription;
        private System.Windows.Forms.Button btnTwitterRemove;
        private System.Windows.Forms.Button btnTwitterAdd;
        private System.Windows.Forms.Label lblTwitterDefaultMessage;
        private System.Windows.Forms.TextBox txtTwitterDefaultMessage;
        private System.Windows.Forms.CheckBox cbTwitterSkipMessageBox;
        private System.Windows.Forms.TabPage tpUp1;
        private System.Windows.Forms.TextBox txtUp1Key;
        private System.Windows.Forms.TextBox txtUp1Host;
        private System.Windows.Forms.Label lblUp1Key;
        private System.Windows.Forms.Label lblUp1Host;
        private System.Windows.Forms.TabPage tpCoinURL;
        private System.Windows.Forms.TextBox txtCoinURLUUID;
        private System.Windows.Forms.Label lblCoinURLUUID;
        private System.Windows.Forms.CheckBox cbOwnCloud81Compatibility;
        private System.Windows.Forms.Button btnCustomUploaderClearUploaders;
        private System.Windows.Forms.TabPage tpOneTimeSecret;
        private System.Windows.Forms.Label lblOneTimeSecretAPIKey;
        private System.Windows.Forms.Label lblOneTimeSecretEmail;
        private System.Windows.Forms.TextBox txtOneTimeSecretAPIKey;
        private System.Windows.Forms.TextBox txtOneTimeSecretEmail;
        private System.Windows.Forms.TabPage tpPolr;
        private System.Windows.Forms.TextBox txtPolrAPIKey;
        private System.Windows.Forms.Label lblPolrAPIKey;
        private System.Windows.Forms.TextBox txtPolrAPIHostname;
        private System.Windows.Forms.Label lblPolrAPIHostname;
        private System.Windows.Forms.CheckBox cbImgurUseGIFV;
        private System.Windows.Forms.ListBox lbTwitterAccounts;
        private System.Windows.Forms.Button btnTwitterNameUpdate;
        private System.Windows.Forms.TabPage tpPomf;
        private System.Windows.Forms.Label lblPomfResultURL;
        private System.Windows.Forms.Label lblPomfUploadURL;
        private System.Windows.Forms.Label lblPomfUploaders;
        private System.Windows.Forms.ComboBox cbPomfUploaders;
        private System.Windows.Forms.TextBox txtPomfUploadURL;
        private System.Windows.Forms.TextBox txtPomfResultURL;
        private System.Windows.Forms.TabPage tpSeafile;
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
        internal System.Windows.Forms.Button btnCustomUploaderHeaderUpdate;
        internal System.Windows.Forms.TextBox txtCustomUploaderHeaderName;
        internal System.Windows.Forms.TextBox txtCustomUploaderHeaderValue;
        internal System.Windows.Forms.Button btnCustomUploaderHeaderAdd;
        internal System.Windows.Forms.Button btnCustomUploaderHeaderRemove;
        internal HelpersLib.MyListView lvCustomUploaderHeaders;
        internal System.Windows.Forms.ColumnHeader chCustomUploaderHeadersName;
        internal System.Windows.Forms.ColumnHeader chCustomUploaderHeadersValue;
        private System.Windows.Forms.Button btnPomfTest;
        private System.Windows.Forms.TabPage tpStreamable;
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
    }
}