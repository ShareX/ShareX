using ShareX.HelpersLib;
namespace ShareX
{
    partial class ApplicationSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationSettingsForm));
            tcSettings = new System.Windows.Forms.TabControl();
            tpGeneral = new System.Windows.Forms.TabPage();
            cbUpdateChannel = new System.Windows.Forms.ComboBox();
            lblUpdateChannel = new System.Windows.Forms.Label();
            cbAutoCheckUpdate = new System.Windows.Forms.CheckBox();
            cbUseWhiteShareXIcon = new System.Windows.Forms.CheckBox();
            btnCheckDevBuild = new System.Windows.Forms.Button();
            cbTrayMiddleClickAction = new System.Windows.Forms.ComboBox();
            lblTrayMiddleClickAction = new System.Windows.Forms.Label();
            cbTrayLeftDoubleClickAction = new System.Windows.Forms.ComboBox();
            lblTrayLeftDoubleClickAction = new System.Windows.Forms.Label();
            cbTrayLeftClickAction = new System.Windows.Forms.ComboBox();
            lblTrayLeftClickAction = new System.Windows.Forms.Label();
            btnEditQuickTaskMenu = new System.Windows.Forms.Button();
            cbShowTray = new System.Windows.Forms.CheckBox();
            cbTrayIconProgressEnabled = new System.Windows.Forms.CheckBox();
            btnLanguages = new MenuButton();
            cmsLanguages = new System.Windows.Forms.ContextMenuStrip(components);
            cbRememberMainFormPosition = new System.Windows.Forms.CheckBox();
            cbSilentRun = new System.Windows.Forms.CheckBox();
            cbTaskbarProgressEnabled = new System.Windows.Forms.CheckBox();
            cbRememberMainFormSize = new System.Windows.Forms.CheckBox();
            lblLanguage = new System.Windows.Forms.Label();
            tpTheme = new System.Windows.Forms.TabPage();
            btnThemeReset = new System.Windows.Forms.Button();
            btnThemeRemove = new System.Windows.Forms.Button();
            btnThemeAdd = new System.Windows.Forms.Button();
            cbThemes = new System.Windows.Forms.ComboBox();
            pgTheme = new System.Windows.Forms.PropertyGrid();
            eiTheme = new ExportImportControl();
            tpIntegration = new System.Windows.Forms.TabPage();
            gbFirefox = new System.Windows.Forms.GroupBox();
            cbFirefoxAddonSupport = new System.Windows.Forms.CheckBox();
            btnFirefoxOpenAddonPage = new System.Windows.Forms.Button();
            gbSteam = new System.Windows.Forms.GroupBox();
            cbSteamShowInApp = new System.Windows.Forms.CheckBox();
            gbChrome = new System.Windows.Forms.GroupBox();
            cbChromeExtensionSupport = new System.Windows.Forms.CheckBox();
            btnChromeOpenExtensionPage = new System.Windows.Forms.Button();
            gbWindows = new System.Windows.Forms.GroupBox();
            cbEditWithShareX = new System.Windows.Forms.CheckBox();
            cbStartWithWindows = new System.Windows.Forms.CheckBox();
            cbSendToMenu = new System.Windows.Forms.CheckBox();
            cbShellContextMenu = new System.Windows.Forms.CheckBox();
            tpPaths = new System.Windows.Forms.TabPage();
            txtSaveImageSubFolderPatternWindow = new System.Windows.Forms.TextBox();
            lblSaveImageSubFolderPatternWindow = new System.Windows.Forms.Label();
            btnPersonalFolderPathApply = new System.Windows.Forms.Button();
            btnOpenScreenshotsFolder = new System.Windows.Forms.Button();
            lblPreviewPersonalFolderPath = new System.Windows.Forms.Label();
            btnBrowsePersonalFolderPath = new System.Windows.Forms.Button();
            lblPersonalFolderPath = new System.Windows.Forms.Label();
            txtPersonalFolderPath = new System.Windows.Forms.TextBox();
            btnBrowseCustomScreenshotsPath = new System.Windows.Forms.Button();
            btnOpenPersonalFolderPath = new System.Windows.Forms.Button();
            txtCustomScreenshotsPath = new System.Windows.Forms.TextBox();
            cbUseCustomScreenshotsPath = new System.Windows.Forms.CheckBox();
            lblSaveImageSubFolderPattern = new System.Windows.Forms.Label();
            lblSaveImageSubFolderPatternPreview = new System.Windows.Forms.Label();
            txtSaveImageSubFolderPattern = new System.Windows.Forms.TextBox();
            tpSettings = new System.Windows.Forms.TabPage();
            cbAutomaticallyCleanupLogFiles = new System.Windows.Forms.CheckBox();
            nudCleanupKeepFileCount = new System.Windows.Forms.NumericUpDown();
            lblCleanupKeepFileCount = new System.Windows.Forms.Label();
            cbAutomaticallyCleanupBackupFiles = new System.Windows.Forms.CheckBox();
            pbExportImportNote = new System.Windows.Forms.PictureBox();
            cbExportHistory = new System.Windows.Forms.CheckBox();
            cbExportSettings = new System.Windows.Forms.CheckBox();
            lblExportImportNote = new System.Windows.Forms.Label();
            btnResetSettings = new System.Windows.Forms.Button();
            pbExportImport = new System.Windows.Forms.ProgressBar();
            btnExport = new System.Windows.Forms.Button();
            btnImport = new System.Windows.Forms.Button();
            tpMainWindow = new System.Windows.Forms.TabPage();
            gbThumbnailView = new System.Windows.Forms.GroupBox();
            btnThumbnailViewThumbnailSizeReset = new System.Windows.Forms.Button();
            lblThumbnailViewThumbnailSizeX = new System.Windows.Forms.Label();
            nudThumbnailViewThumbnailSizeHeight = new System.Windows.Forms.NumericUpDown();
            nudThumbnailViewThumbnailSizeWidth = new System.Windows.Forms.NumericUpDown();
            cbThumbnailViewThumbnailClickAction = new System.Windows.Forms.ComboBox();
            lblThumbnailViewThumbnailClickAction = new System.Windows.Forms.Label();
            lblThumbnailViewThumbnailSize = new System.Windows.Forms.Label();
            cbThumbnailViewTitleLocation = new System.Windows.Forms.ComboBox();
            lblThumbnailViewTitleLocation = new System.Windows.Forms.Label();
            cbThumbnailViewShowTitle = new System.Windows.Forms.CheckBox();
            tpClipboardFormats = new System.Windows.Forms.TabPage();
            lblClipboardFormatsTip = new System.Windows.Forms.Label();
            btnClipboardFormatEdit = new System.Windows.Forms.Button();
            btnClipboardFormatRemove = new System.Windows.Forms.Button();
            btnClipboardFormatAdd = new System.Windows.Forms.Button();
            lvClipboardFormats = new MyListView();
            chDescription = new System.Windows.Forms.ColumnHeader();
            chFormat = new System.Windows.Forms.ColumnHeader();
            tpUpload = new System.Windows.Forms.TabPage();
            gbSecondaryFileUploaders = new System.Windows.Forms.GroupBox();
            lvSecondaryFileUploaders = new MyListView();
            chSecondaryFileUploaders = new System.Windows.Forms.ColumnHeader();
            lblUploadLimit = new System.Windows.Forms.Label();
            gbSecondaryImageUploaders = new System.Windows.Forms.GroupBox();
            lvSecondaryImageUploaders = new MyListView();
            chSecondaryImageUploaders = new System.Windows.Forms.ColumnHeader();
            gbSecondaryTextUploaders = new System.Windows.Forms.GroupBox();
            lvSecondaryTextUploaders = new MyListView();
            chSecondaryTextUploaders = new System.Windows.Forms.ColumnHeader();
            nudUploadLimit = new System.Windows.Forms.NumericUpDown();
            cbUseSecondaryUploaders = new System.Windows.Forms.CheckBox();
            lblUploadLimitHint = new System.Windows.Forms.Label();
            cbIfUploadFailRetryOnce = new System.Windows.Forms.Label();
            lblBufferSize = new System.Windows.Forms.Label();
            nudRetryUpload = new System.Windows.Forms.NumericUpDown();
            cbBufferSize = new System.Windows.Forms.ComboBox();
            tpHistory = new System.Windows.Forms.TabPage();
            gbHistory = new System.Windows.Forms.GroupBox();
            cbHistoryCheckURL = new System.Windows.Forms.CheckBox();
            cbHistorySaveTasks = new System.Windows.Forms.CheckBox();
            gbRecentLinks = new System.Windows.Forms.GroupBox();
            cbRecentTasksTrayMenuMostRecentFirst = new System.Windows.Forms.CheckBox();
            lblRecentTasksMaxCount = new System.Windows.Forms.Label();
            nudRecentTasksMaxCount = new System.Windows.Forms.NumericUpDown();
            cbRecentTasksShowInTrayMenu = new System.Windows.Forms.CheckBox();
            cbRecentTasksShowInMainWindow = new System.Windows.Forms.CheckBox();
            cbRecentTasksSave = new System.Windows.Forms.CheckBox();
            tpPrint = new System.Windows.Forms.TabPage();
            lblDefaultPrinterOverride = new System.Windows.Forms.Label();
            txtDefaultPrinterOverride = new System.Windows.Forms.TextBox();
            cbPrintDontShowWindowsDialog = new System.Windows.Forms.CheckBox();
            cbDontShowPrintSettingDialog = new System.Windows.Forms.CheckBox();
            btnShowImagePrintSettings = new System.Windows.Forms.Button();
            tpProxy = new System.Windows.Forms.TabPage();
            cbProxyMethod = new System.Windows.Forms.ComboBox();
            lblProxyMethod = new System.Windows.Forms.Label();
            lblProxyHost = new System.Windows.Forms.Label();
            txtProxyHost = new System.Windows.Forms.TextBox();
            nudProxyPort = new System.Windows.Forms.NumericUpDown();
            lblProxyPort = new System.Windows.Forms.Label();
            lblProxyPassword = new System.Windows.Forms.Label();
            txtProxyPassword = new System.Windows.Forms.TextBox();
            lblProxyUsername = new System.Windows.Forms.Label();
            txtProxyUsername = new System.Windows.Forms.TextBox();
            tpAdvanced = new System.Windows.Forms.TabPage();
            pgSettings = new System.Windows.Forms.PropertyGrid();
            tttvMain = new TabToTreeView();
            tcSettings.SuspendLayout();
            tpGeneral.SuspendLayout();
            tpTheme.SuspendLayout();
            tpIntegration.SuspendLayout();
            gbFirefox.SuspendLayout();
            gbSteam.SuspendLayout();
            gbChrome.SuspendLayout();
            gbWindows.SuspendLayout();
            tpPaths.SuspendLayout();
            tpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudCleanupKeepFileCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbExportImportNote).BeginInit();
            tpMainWindow.SuspendLayout();
            gbThumbnailView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailViewThumbnailSizeHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailViewThumbnailSizeWidth).BeginInit();
            tpClipboardFormats.SuspendLayout();
            tpUpload.SuspendLayout();
            gbSecondaryFileUploaders.SuspendLayout();
            gbSecondaryImageUploaders.SuspendLayout();
            gbSecondaryTextUploaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudUploadLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRetryUpload).BeginInit();
            tpHistory.SuspendLayout();
            gbHistory.SuspendLayout();
            gbRecentLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRecentTasksMaxCount).BeginInit();
            tpPrint.SuspendLayout();
            tpProxy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudProxyPort).BeginInit();
            tpAdvanced.SuspendLayout();
            SuspendLayout();
            // 
            // tcSettings
            // 
            resources.ApplyResources(tcSettings, "tcSettings");
            tcSettings.Controls.Add(tpGeneral);
            tcSettings.Controls.Add(tpTheme);
            tcSettings.Controls.Add(tpIntegration);
            tcSettings.Controls.Add(tpPaths);
            tcSettings.Controls.Add(tpSettings);
            tcSettings.Controls.Add(tpMainWindow);
            tcSettings.Controls.Add(tpClipboardFormats);
            tcSettings.Controls.Add(tpUpload);
            tcSettings.Controls.Add(tpHistory);
            tcSettings.Controls.Add(tpPrint);
            tcSettings.Controls.Add(tpProxy);
            tcSettings.Controls.Add(tpAdvanced);
            tcSettings.Name = "tcSettings";
            tcSettings.SelectedIndex = 0;
            // 
            // tpGeneral
            // 
            tpGeneral.BackColor = System.Drawing.SystemColors.Window;
            tpGeneral.Controls.Add(cbUpdateChannel);
            tpGeneral.Controls.Add(lblUpdateChannel);
            tpGeneral.Controls.Add(cbAutoCheckUpdate);
            tpGeneral.Controls.Add(cbUseWhiteShareXIcon);
            tpGeneral.Controls.Add(btnCheckDevBuild);
            tpGeneral.Controls.Add(cbTrayMiddleClickAction);
            tpGeneral.Controls.Add(lblTrayMiddleClickAction);
            tpGeneral.Controls.Add(cbTrayLeftDoubleClickAction);
            tpGeneral.Controls.Add(lblTrayLeftDoubleClickAction);
            tpGeneral.Controls.Add(cbTrayLeftClickAction);
            tpGeneral.Controls.Add(lblTrayLeftClickAction);
            tpGeneral.Controls.Add(btnEditQuickTaskMenu);
            tpGeneral.Controls.Add(cbShowTray);
            tpGeneral.Controls.Add(cbTrayIconProgressEnabled);
            tpGeneral.Controls.Add(btnLanguages);
            tpGeneral.Controls.Add(cbRememberMainFormPosition);
            tpGeneral.Controls.Add(cbSilentRun);
            tpGeneral.Controls.Add(cbTaskbarProgressEnabled);
            tpGeneral.Controls.Add(cbRememberMainFormSize);
            tpGeneral.Controls.Add(lblLanguage);
            resources.ApplyResources(tpGeneral, "tpGeneral");
            tpGeneral.Name = "tpGeneral";
            // 
            // cbUpdateChannel
            // 
            cbUpdateChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbUpdateChannel.FormattingEnabled = true;
            resources.ApplyResources(cbUpdateChannel, "cbUpdateChannel");
            cbUpdateChannel.Name = "cbUpdateChannel";
            cbUpdateChannel.SelectedIndexChanged += cbUpdateChannel_SelectedIndexChanged;
            // 
            // lblUpdateChannel
            // 
            resources.ApplyResources(lblUpdateChannel, "lblUpdateChannel");
            lblUpdateChannel.Name = "lblUpdateChannel";
            // 
            // cbAutoCheckUpdate
            // 
            resources.ApplyResources(cbAutoCheckUpdate, "cbAutoCheckUpdate");
            cbAutoCheckUpdate.Name = "cbAutoCheckUpdate";
            cbAutoCheckUpdate.UseVisualStyleBackColor = true;
            cbAutoCheckUpdate.CheckedChanged += cbAutoCheckUpdate_CheckedChanged;
            // 
            // cbUseWhiteShareXIcon
            // 
            resources.ApplyResources(cbUseWhiteShareXIcon, "cbUseWhiteShareXIcon");
            cbUseWhiteShareXIcon.Name = "cbUseWhiteShareXIcon";
            cbUseWhiteShareXIcon.UseVisualStyleBackColor = true;
            cbUseWhiteShareXIcon.CheckedChanged += CbUseWhiteShareXIcon_CheckedChanged;
            // 
            // btnCheckDevBuild
            // 
            resources.ApplyResources(btnCheckDevBuild, "btnCheckDevBuild");
            btnCheckDevBuild.Name = "btnCheckDevBuild";
            btnCheckDevBuild.UseVisualStyleBackColor = true;
            btnCheckDevBuild.Click += btnCheckDevBuild_Click;
            // 
            // cbTrayMiddleClickAction
            // 
            cbTrayMiddleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTrayMiddleClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbTrayMiddleClickAction, "cbTrayMiddleClickAction");
            cbTrayMiddleClickAction.Name = "cbTrayMiddleClickAction";
            cbTrayMiddleClickAction.SelectedIndexChanged += cbTrayMiddleClickAction_SelectedIndexChanged;
            // 
            // lblTrayMiddleClickAction
            // 
            resources.ApplyResources(lblTrayMiddleClickAction, "lblTrayMiddleClickAction");
            lblTrayMiddleClickAction.Name = "lblTrayMiddleClickAction";
            // 
            // cbTrayLeftDoubleClickAction
            // 
            cbTrayLeftDoubleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTrayLeftDoubleClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbTrayLeftDoubleClickAction, "cbTrayLeftDoubleClickAction");
            cbTrayLeftDoubleClickAction.Name = "cbTrayLeftDoubleClickAction";
            cbTrayLeftDoubleClickAction.SelectedIndexChanged += cbTrayLeftDoubleClickAction_SelectedIndexChanged;
            // 
            // lblTrayLeftDoubleClickAction
            // 
            resources.ApplyResources(lblTrayLeftDoubleClickAction, "lblTrayLeftDoubleClickAction");
            lblTrayLeftDoubleClickAction.Name = "lblTrayLeftDoubleClickAction";
            // 
            // cbTrayLeftClickAction
            // 
            cbTrayLeftClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTrayLeftClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbTrayLeftClickAction, "cbTrayLeftClickAction");
            cbTrayLeftClickAction.Name = "cbTrayLeftClickAction";
            cbTrayLeftClickAction.SelectedIndexChanged += cbTrayLeftClickAction_SelectedIndexChanged;
            // 
            // lblTrayLeftClickAction
            // 
            resources.ApplyResources(lblTrayLeftClickAction, "lblTrayLeftClickAction");
            lblTrayLeftClickAction.Name = "lblTrayLeftClickAction";
            // 
            // btnEditQuickTaskMenu
            // 
            resources.ApplyResources(btnEditQuickTaskMenu, "btnEditQuickTaskMenu");
            btnEditQuickTaskMenu.Name = "btnEditQuickTaskMenu";
            btnEditQuickTaskMenu.UseVisualStyleBackColor = true;
            btnEditQuickTaskMenu.Click += btnEditQuickTaskMenu_Click;
            // 
            // cbShowTray
            // 
            resources.ApplyResources(cbShowTray, "cbShowTray");
            cbShowTray.Name = "cbShowTray";
            cbShowTray.UseVisualStyleBackColor = true;
            cbShowTray.CheckedChanged += cbShowTray_CheckedChanged;
            // 
            // cbTrayIconProgressEnabled
            // 
            resources.ApplyResources(cbTrayIconProgressEnabled, "cbTrayIconProgressEnabled");
            cbTrayIconProgressEnabled.Name = "cbTrayIconProgressEnabled";
            cbTrayIconProgressEnabled.UseVisualStyleBackColor = true;
            cbTrayIconProgressEnabled.CheckedChanged += cbTrayIconProgressEnabled_CheckedChanged;
            // 
            // btnLanguages
            // 
            resources.ApplyResources(btnLanguages, "btnLanguages");
            btnLanguages.Menu = cmsLanguages;
            btnLanguages.Name = "btnLanguages";
            btnLanguages.UseVisualStyleBackColor = true;
            // 
            // cmsLanguages
            // 
            cmsLanguages.Name = "cmsLanguages";
            resources.ApplyResources(cmsLanguages, "cmsLanguages");
            // 
            // cbRememberMainFormPosition
            // 
            resources.ApplyResources(cbRememberMainFormPosition, "cbRememberMainFormPosition");
            cbRememberMainFormPosition.Name = "cbRememberMainFormPosition";
            cbRememberMainFormPosition.UseVisualStyleBackColor = true;
            cbRememberMainFormPosition.CheckedChanged += cbRememberMainFormPosition_CheckedChanged;
            // 
            // cbSilentRun
            // 
            resources.ApplyResources(cbSilentRun, "cbSilentRun");
            cbSilentRun.Name = "cbSilentRun";
            cbSilentRun.UseVisualStyleBackColor = true;
            cbSilentRun.CheckedChanged += cbSilentRun_CheckedChanged;
            // 
            // cbTaskbarProgressEnabled
            // 
            resources.ApplyResources(cbTaskbarProgressEnabled, "cbTaskbarProgressEnabled");
            cbTaskbarProgressEnabled.Name = "cbTaskbarProgressEnabled";
            cbTaskbarProgressEnabled.UseVisualStyleBackColor = true;
            cbTaskbarProgressEnabled.CheckedChanged += cbTaskbarProgressEnabled_CheckedChanged;
            // 
            // cbRememberMainFormSize
            // 
            resources.ApplyResources(cbRememberMainFormSize, "cbRememberMainFormSize");
            cbRememberMainFormSize.Name = "cbRememberMainFormSize";
            cbRememberMainFormSize.UseVisualStyleBackColor = true;
            cbRememberMainFormSize.CheckedChanged += cbRememberMainFormSize_CheckedChanged;
            // 
            // lblLanguage
            // 
            resources.ApplyResources(lblLanguage, "lblLanguage");
            lblLanguage.Name = "lblLanguage";
            // 
            // tpTheme
            // 
            tpTheme.Controls.Add(btnThemeReset);
            tpTheme.Controls.Add(btnThemeRemove);
            tpTheme.Controls.Add(btnThemeAdd);
            tpTheme.Controls.Add(cbThemes);
            tpTheme.Controls.Add(pgTheme);
            tpTheme.Controls.Add(eiTheme);
            resources.ApplyResources(tpTheme, "tpTheme");
            tpTheme.Name = "tpTheme";
            tpTheme.UseVisualStyleBackColor = true;
            // 
            // btnThemeReset
            // 
            resources.ApplyResources(btnThemeReset, "btnThemeReset");
            btnThemeReset.Name = "btnThemeReset";
            btnThemeReset.UseVisualStyleBackColor = true;
            btnThemeReset.Click += BtnThemeReset_Click;
            // 
            // btnThemeRemove
            // 
            resources.ApplyResources(btnThemeRemove, "btnThemeRemove");
            btnThemeRemove.Name = "btnThemeRemove";
            btnThemeRemove.UseVisualStyleBackColor = true;
            btnThemeRemove.Click += BtnThemeRemove_Click;
            // 
            // btnThemeAdd
            // 
            resources.ApplyResources(btnThemeAdd, "btnThemeAdd");
            btnThemeAdd.Name = "btnThemeAdd";
            btnThemeAdd.UseVisualStyleBackColor = true;
            btnThemeAdd.Click += BtnThemeAdd_Click;
            // 
            // cbThemes
            // 
            cbThemes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbThemes.FormattingEnabled = true;
            resources.ApplyResources(cbThemes, "cbThemes");
            cbThemes.Name = "cbThemes";
            cbThemes.SelectedIndexChanged += CbThemes_SelectedIndexChanged;
            // 
            // pgTheme
            // 
            pgTheme.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(pgTheme, "pgTheme");
            pgTheme.Name = "pgTheme";
            pgTheme.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            pgTheme.ToolbarVisible = false;
            pgTheme.PropertyValueChanged += pgTheme_PropertyValueChanged;
            // 
            // eiTheme
            // 
            eiTheme.DefaultFileName = null;
            resources.ApplyResources(eiTheme, "eiTheme");
            eiTheme.Name = "eiTheme";
            eiTheme.ObjectType = null;
            eiTheme.SerializationBinder = null;
            eiTheme.ExportRequested += EiTheme_ExportRequested;
            eiTheme.ImportRequested += EiTheme_ImportRequested;
            // 
            // tpIntegration
            // 
            tpIntegration.BackColor = System.Drawing.SystemColors.Window;
            tpIntegration.Controls.Add(gbFirefox);
            tpIntegration.Controls.Add(gbSteam);
            tpIntegration.Controls.Add(gbChrome);
            tpIntegration.Controls.Add(gbWindows);
            resources.ApplyResources(tpIntegration, "tpIntegration");
            tpIntegration.Name = "tpIntegration";
            // 
            // gbFirefox
            // 
            gbFirefox.Controls.Add(cbFirefoxAddonSupport);
            gbFirefox.Controls.Add(btnFirefoxOpenAddonPage);
            resources.ApplyResources(gbFirefox, "gbFirefox");
            gbFirefox.Name = "gbFirefox";
            gbFirefox.TabStop = false;
            // 
            // cbFirefoxAddonSupport
            // 
            resources.ApplyResources(cbFirefoxAddonSupport, "cbFirefoxAddonSupport");
            cbFirefoxAddonSupport.Name = "cbFirefoxAddonSupport";
            cbFirefoxAddonSupport.UseVisualStyleBackColor = true;
            cbFirefoxAddonSupport.CheckedChanged += cbFirefoxAddonSupport_CheckedChanged;
            // 
            // btnFirefoxOpenAddonPage
            // 
            resources.ApplyResources(btnFirefoxOpenAddonPage, "btnFirefoxOpenAddonPage");
            btnFirefoxOpenAddonPage.Name = "btnFirefoxOpenAddonPage";
            btnFirefoxOpenAddonPage.UseVisualStyleBackColor = true;
            btnFirefoxOpenAddonPage.Click += btnFirefoxOpenAddonPage_Click;
            // 
            // gbSteam
            // 
            gbSteam.Controls.Add(cbSteamShowInApp);
            resources.ApplyResources(gbSteam, "gbSteam");
            gbSteam.Name = "gbSteam";
            gbSteam.TabStop = false;
            // 
            // cbSteamShowInApp
            // 
            resources.ApplyResources(cbSteamShowInApp, "cbSteamShowInApp");
            cbSteamShowInApp.Name = "cbSteamShowInApp";
            cbSteamShowInApp.UseVisualStyleBackColor = true;
            cbSteamShowInApp.CheckedChanged += cbSteamShowInApp_CheckedChanged;
            // 
            // gbChrome
            // 
            gbChrome.Controls.Add(cbChromeExtensionSupport);
            gbChrome.Controls.Add(btnChromeOpenExtensionPage);
            resources.ApplyResources(gbChrome, "gbChrome");
            gbChrome.Name = "gbChrome";
            gbChrome.TabStop = false;
            // 
            // cbChromeExtensionSupport
            // 
            resources.ApplyResources(cbChromeExtensionSupport, "cbChromeExtensionSupport");
            cbChromeExtensionSupport.Name = "cbChromeExtensionSupport";
            cbChromeExtensionSupport.UseVisualStyleBackColor = true;
            cbChromeExtensionSupport.CheckedChanged += cbChromeExtensionSupport_CheckedChanged;
            // 
            // btnChromeOpenExtensionPage
            // 
            resources.ApplyResources(btnChromeOpenExtensionPage, "btnChromeOpenExtensionPage");
            btnChromeOpenExtensionPage.Name = "btnChromeOpenExtensionPage";
            btnChromeOpenExtensionPage.UseVisualStyleBackColor = true;
            btnChromeOpenExtensionPage.Click += btnChromeOpenExtensionPage_Click;
            // 
            // gbWindows
            // 
            gbWindows.Controls.Add(cbEditWithShareX);
            gbWindows.Controls.Add(cbStartWithWindows);
            gbWindows.Controls.Add(cbSendToMenu);
            gbWindows.Controls.Add(cbShellContextMenu);
            resources.ApplyResources(gbWindows, "gbWindows");
            gbWindows.Name = "gbWindows";
            gbWindows.TabStop = false;
            // 
            // cbEditWithShareX
            // 
            resources.ApplyResources(cbEditWithShareX, "cbEditWithShareX");
            cbEditWithShareX.Name = "cbEditWithShareX";
            cbEditWithShareX.UseVisualStyleBackColor = true;
            cbEditWithShareX.CheckedChanged += cbEditWithShareX_CheckedChanged;
            // 
            // cbStartWithWindows
            // 
            resources.ApplyResources(cbStartWithWindows, "cbStartWithWindows");
            cbStartWithWindows.Name = "cbStartWithWindows";
            cbStartWithWindows.UseVisualStyleBackColor = true;
            cbStartWithWindows.CheckedChanged += cbStartWithWindows_CheckedChanged;
            // 
            // cbSendToMenu
            // 
            resources.ApplyResources(cbSendToMenu, "cbSendToMenu");
            cbSendToMenu.Name = "cbSendToMenu";
            cbSendToMenu.UseVisualStyleBackColor = true;
            cbSendToMenu.CheckedChanged += cbSendToMenu_CheckedChanged;
            // 
            // cbShellContextMenu
            // 
            resources.ApplyResources(cbShellContextMenu, "cbShellContextMenu");
            cbShellContextMenu.Name = "cbShellContextMenu";
            cbShellContextMenu.UseVisualStyleBackColor = true;
            cbShellContextMenu.CheckedChanged += cbShellContextMenu_CheckedChanged;
            // 
            // tpPaths
            // 
            tpPaths.BackColor = System.Drawing.SystemColors.Window;
            tpPaths.Controls.Add(txtSaveImageSubFolderPatternWindow);
            tpPaths.Controls.Add(lblSaveImageSubFolderPatternWindow);
            tpPaths.Controls.Add(btnPersonalFolderPathApply);
            tpPaths.Controls.Add(btnOpenScreenshotsFolder);
            tpPaths.Controls.Add(lblPreviewPersonalFolderPath);
            tpPaths.Controls.Add(btnBrowsePersonalFolderPath);
            tpPaths.Controls.Add(lblPersonalFolderPath);
            tpPaths.Controls.Add(txtPersonalFolderPath);
            tpPaths.Controls.Add(btnBrowseCustomScreenshotsPath);
            tpPaths.Controls.Add(btnOpenPersonalFolderPath);
            tpPaths.Controls.Add(txtCustomScreenshotsPath);
            tpPaths.Controls.Add(cbUseCustomScreenshotsPath);
            tpPaths.Controls.Add(lblSaveImageSubFolderPattern);
            tpPaths.Controls.Add(lblSaveImageSubFolderPatternPreview);
            tpPaths.Controls.Add(txtSaveImageSubFolderPattern);
            resources.ApplyResources(tpPaths, "tpPaths");
            tpPaths.Name = "tpPaths";
            // 
            // txtSaveImageSubFolderPatternWindow
            // 
            resources.ApplyResources(txtSaveImageSubFolderPatternWindow, "txtSaveImageSubFolderPatternWindow");
            txtSaveImageSubFolderPatternWindow.Name = "txtSaveImageSubFolderPatternWindow";
            txtSaveImageSubFolderPatternWindow.TextChanged += txtSaveImageSubFolderPatternWindow_TextChanged;
            // 
            // lblSaveImageSubFolderPatternWindow
            // 
            resources.ApplyResources(lblSaveImageSubFolderPatternWindow, "lblSaveImageSubFolderPatternWindow");
            lblSaveImageSubFolderPatternWindow.Name = "lblSaveImageSubFolderPatternWindow";
            // 
            // btnPersonalFolderPathApply
            // 
            resources.ApplyResources(btnPersonalFolderPathApply, "btnPersonalFolderPathApply");
            btnPersonalFolderPathApply.Name = "btnPersonalFolderPathApply";
            btnPersonalFolderPathApply.UseVisualStyleBackColor = true;
            btnPersonalFolderPathApply.Click += btnPersonalFolderPathApply_Click;
            // 
            // btnOpenScreenshotsFolder
            // 
            resources.ApplyResources(btnOpenScreenshotsFolder, "btnOpenScreenshotsFolder");
            btnOpenScreenshotsFolder.Name = "btnOpenScreenshotsFolder";
            btnOpenScreenshotsFolder.UseVisualStyleBackColor = true;
            btnOpenScreenshotsFolder.Click += btnOpenScreenshotsFolder_Click;
            // 
            // lblPreviewPersonalFolderPath
            // 
            resources.ApplyResources(lblPreviewPersonalFolderPath, "lblPreviewPersonalFolderPath");
            lblPreviewPersonalFolderPath.Name = "lblPreviewPersonalFolderPath";
            // 
            // btnBrowsePersonalFolderPath
            // 
            resources.ApplyResources(btnBrowsePersonalFolderPath, "btnBrowsePersonalFolderPath");
            btnBrowsePersonalFolderPath.Name = "btnBrowsePersonalFolderPath";
            btnBrowsePersonalFolderPath.UseVisualStyleBackColor = true;
            btnBrowsePersonalFolderPath.Click += btnBrowsePersonalFolderPath_Click;
            // 
            // lblPersonalFolderPath
            // 
            resources.ApplyResources(lblPersonalFolderPath, "lblPersonalFolderPath");
            lblPersonalFolderPath.Name = "lblPersonalFolderPath";
            // 
            // txtPersonalFolderPath
            // 
            resources.ApplyResources(txtPersonalFolderPath, "txtPersonalFolderPath");
            txtPersonalFolderPath.Name = "txtPersonalFolderPath";
            txtPersonalFolderPath.TextChanged += txtPersonalFolderPath_TextChanged;
            // 
            // btnBrowseCustomScreenshotsPath
            // 
            resources.ApplyResources(btnBrowseCustomScreenshotsPath, "btnBrowseCustomScreenshotsPath");
            btnBrowseCustomScreenshotsPath.Name = "btnBrowseCustomScreenshotsPath";
            btnBrowseCustomScreenshotsPath.UseVisualStyleBackColor = true;
            btnBrowseCustomScreenshotsPath.Click += btnBrowseCustomScreenshotsPath_Click;
            // 
            // btnOpenPersonalFolderPath
            // 
            resources.ApplyResources(btnOpenPersonalFolderPath, "btnOpenPersonalFolderPath");
            btnOpenPersonalFolderPath.Name = "btnOpenPersonalFolderPath";
            btnOpenPersonalFolderPath.UseVisualStyleBackColor = true;
            btnOpenPersonalFolderPath.Click += btnOpenPersonalFolder_Click;
            // 
            // txtCustomScreenshotsPath
            // 
            resources.ApplyResources(txtCustomScreenshotsPath, "txtCustomScreenshotsPath");
            txtCustomScreenshotsPath.Name = "txtCustomScreenshotsPath";
            txtCustomScreenshotsPath.TextChanged += txtCustomScreenshotsPath_TextChanged;
            // 
            // cbUseCustomScreenshotsPath
            // 
            resources.ApplyResources(cbUseCustomScreenshotsPath, "cbUseCustomScreenshotsPath");
            cbUseCustomScreenshotsPath.Name = "cbUseCustomScreenshotsPath";
            cbUseCustomScreenshotsPath.UseVisualStyleBackColor = true;
            cbUseCustomScreenshotsPath.CheckedChanged += cbUseCustomScreenshotsPath_CheckedChanged;
            // 
            // lblSaveImageSubFolderPattern
            // 
            resources.ApplyResources(lblSaveImageSubFolderPattern, "lblSaveImageSubFolderPattern");
            lblSaveImageSubFolderPattern.Name = "lblSaveImageSubFolderPattern";
            // 
            // lblSaveImageSubFolderPatternPreview
            // 
            resources.ApplyResources(lblSaveImageSubFolderPatternPreview, "lblSaveImageSubFolderPatternPreview");
            lblSaveImageSubFolderPatternPreview.Name = "lblSaveImageSubFolderPatternPreview";
            // 
            // txtSaveImageSubFolderPattern
            // 
            resources.ApplyResources(txtSaveImageSubFolderPattern, "txtSaveImageSubFolderPattern");
            txtSaveImageSubFolderPattern.Name = "txtSaveImageSubFolderPattern";
            txtSaveImageSubFolderPattern.TextChanged += txtSaveImageSubFolderPattern_TextChanged;
            // 
            // tpSettings
            // 
            tpSettings.BackColor = System.Drawing.SystemColors.Window;
            tpSettings.Controls.Add(cbAutomaticallyCleanupLogFiles);
            tpSettings.Controls.Add(nudCleanupKeepFileCount);
            tpSettings.Controls.Add(lblCleanupKeepFileCount);
            tpSettings.Controls.Add(cbAutomaticallyCleanupBackupFiles);
            tpSettings.Controls.Add(pbExportImportNote);
            tpSettings.Controls.Add(cbExportHistory);
            tpSettings.Controls.Add(cbExportSettings);
            tpSettings.Controls.Add(lblExportImportNote);
            tpSettings.Controls.Add(btnResetSettings);
            tpSettings.Controls.Add(pbExportImport);
            tpSettings.Controls.Add(btnExport);
            tpSettings.Controls.Add(btnImport);
            resources.ApplyResources(tpSettings, "tpSettings");
            tpSettings.Name = "tpSettings";
            // 
            // cbAutomaticallyCleanupLogFiles
            // 
            resources.ApplyResources(cbAutomaticallyCleanupLogFiles, "cbAutomaticallyCleanupLogFiles");
            cbAutomaticallyCleanupLogFiles.Name = "cbAutomaticallyCleanupLogFiles";
            cbAutomaticallyCleanupLogFiles.UseVisualStyleBackColor = true;
            cbAutomaticallyCleanupLogFiles.CheckedChanged += cbAutomaticallyCleanupLogFiles_CheckedChanged;
            // 
            // nudCleanupKeepFileCount
            // 
            resources.ApplyResources(nudCleanupKeepFileCount, "nudCleanupKeepFileCount");
            nudCleanupKeepFileCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCleanupKeepFileCount.Name = "nudCleanupKeepFileCount";
            nudCleanupKeepFileCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudCleanupKeepFileCount.ValueChanged += nudCleanupKeepFileCount_ValueChanged;
            // 
            // lblCleanupKeepFileCount
            // 
            resources.ApplyResources(lblCleanupKeepFileCount, "lblCleanupKeepFileCount");
            lblCleanupKeepFileCount.Name = "lblCleanupKeepFileCount";
            // 
            // cbAutomaticallyCleanupBackupFiles
            // 
            resources.ApplyResources(cbAutomaticallyCleanupBackupFiles, "cbAutomaticallyCleanupBackupFiles");
            cbAutomaticallyCleanupBackupFiles.Name = "cbAutomaticallyCleanupBackupFiles";
            cbAutomaticallyCleanupBackupFiles.UseVisualStyleBackColor = true;
            cbAutomaticallyCleanupBackupFiles.CheckedChanged += cbAutomaticallyCleanupBackupFiles_CheckedChanged;
            // 
            // pbExportImportNote
            // 
            pbExportImportNote.Image = Properties.Resources.exclamation;
            resources.ApplyResources(pbExportImportNote, "pbExportImportNote");
            pbExportImportNote.Name = "pbExportImportNote";
            pbExportImportNote.TabStop = false;
            // 
            // cbExportHistory
            // 
            resources.ApplyResources(cbExportHistory, "cbExportHistory");
            cbExportHistory.Checked = true;
            cbExportHistory.CheckState = System.Windows.Forms.CheckState.Checked;
            cbExportHistory.Name = "cbExportHistory";
            cbExportHistory.UseVisualStyleBackColor = true;
            cbExportHistory.CheckedChanged += cbExportHistory_CheckedChanged;
            // 
            // cbExportSettings
            // 
            resources.ApplyResources(cbExportSettings, "cbExportSettings");
            cbExportSettings.Checked = true;
            cbExportSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbExportSettings.Name = "cbExportSettings";
            cbExportSettings.UseVisualStyleBackColor = true;
            cbExportSettings.CheckedChanged += cbExportSettings_CheckedChanged;
            // 
            // lblExportImportNote
            // 
            resources.ApplyResources(lblExportImportNote, "lblExportImportNote");
            lblExportImportNote.Name = "lblExportImportNote";
            // 
            // btnResetSettings
            // 
            resources.ApplyResources(btnResetSettings, "btnResetSettings");
            btnResetSettings.Name = "btnResetSettings";
            btnResetSettings.UseVisualStyleBackColor = true;
            btnResetSettings.Click += btnResetSettings_Click;
            // 
            // pbExportImport
            // 
            resources.ApplyResources(pbExportImport, "pbExportImport");
            pbExportImport.MarqueeAnimationSpeed = 50;
            pbExportImport.Name = "pbExportImport";
            pbExportImport.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // btnExport
            // 
            resources.ApplyResources(btnExport, "btnExport");
            btnExport.Name = "btnExport";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnImport
            // 
            resources.ApplyResources(btnImport, "btnImport");
            btnImport.Name = "btnImport";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // tpMainWindow
            // 
            tpMainWindow.Controls.Add(gbThumbnailView);
            resources.ApplyResources(tpMainWindow, "tpMainWindow");
            tpMainWindow.Name = "tpMainWindow";
            tpMainWindow.UseVisualStyleBackColor = true;
            // 
            // gbThumbnailView
            // 
            gbThumbnailView.Controls.Add(btnThumbnailViewThumbnailSizeReset);
            gbThumbnailView.Controls.Add(lblThumbnailViewThumbnailSizeX);
            gbThumbnailView.Controls.Add(nudThumbnailViewThumbnailSizeHeight);
            gbThumbnailView.Controls.Add(nudThumbnailViewThumbnailSizeWidth);
            gbThumbnailView.Controls.Add(cbThumbnailViewThumbnailClickAction);
            gbThumbnailView.Controls.Add(lblThumbnailViewThumbnailClickAction);
            gbThumbnailView.Controls.Add(lblThumbnailViewThumbnailSize);
            gbThumbnailView.Controls.Add(cbThumbnailViewTitleLocation);
            gbThumbnailView.Controls.Add(lblThumbnailViewTitleLocation);
            gbThumbnailView.Controls.Add(cbThumbnailViewShowTitle);
            resources.ApplyResources(gbThumbnailView, "gbThumbnailView");
            gbThumbnailView.Name = "gbThumbnailView";
            gbThumbnailView.TabStop = false;
            // 
            // btnThumbnailViewThumbnailSizeReset
            // 
            resources.ApplyResources(btnThumbnailViewThumbnailSizeReset, "btnThumbnailViewThumbnailSizeReset");
            btnThumbnailViewThumbnailSizeReset.Name = "btnThumbnailViewThumbnailSizeReset";
            btnThumbnailViewThumbnailSizeReset.UseVisualStyleBackColor = true;
            btnThumbnailViewThumbnailSizeReset.Click += btnThumbnailViewThumbnailSizeReset_Click;
            // 
            // lblThumbnailViewThumbnailSizeX
            // 
            resources.ApplyResources(lblThumbnailViewThumbnailSizeX, "lblThumbnailViewThumbnailSizeX");
            lblThumbnailViewThumbnailSizeX.Name = "lblThumbnailViewThumbnailSizeX";
            // 
            // nudThumbnailViewThumbnailSizeHeight
            // 
            resources.ApplyResources(nudThumbnailViewThumbnailSizeHeight, "nudThumbnailViewThumbnailSizeHeight");
            nudThumbnailViewThumbnailSizeHeight.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudThumbnailViewThumbnailSizeHeight.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudThumbnailViewThumbnailSizeHeight.Name = "nudThumbnailViewThumbnailSizeHeight";
            nudThumbnailViewThumbnailSizeHeight.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudThumbnailViewThumbnailSizeHeight.ValueChanged += nudThumbnailViewThumbnailSizeHeight_ValueChanged;
            // 
            // nudThumbnailViewThumbnailSizeWidth
            // 
            resources.ApplyResources(nudThumbnailViewThumbnailSizeWidth, "nudThumbnailViewThumbnailSizeWidth");
            nudThumbnailViewThumbnailSizeWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudThumbnailViewThumbnailSizeWidth.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudThumbnailViewThumbnailSizeWidth.Name = "nudThumbnailViewThumbnailSizeWidth";
            nudThumbnailViewThumbnailSizeWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudThumbnailViewThumbnailSizeWidth.ValueChanged += nudThumbnailViewThumbnailSizeWidth_ValueChanged;
            // 
            // cbThumbnailViewThumbnailClickAction
            // 
            cbThumbnailViewThumbnailClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbThumbnailViewThumbnailClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbThumbnailViewThumbnailClickAction, "cbThumbnailViewThumbnailClickAction");
            cbThumbnailViewThumbnailClickAction.Name = "cbThumbnailViewThumbnailClickAction";
            cbThumbnailViewThumbnailClickAction.SelectedIndexChanged += cbThumbnailViewThumbnailClickAction_SelectedIndexChanged;
            // 
            // lblThumbnailViewThumbnailClickAction
            // 
            resources.ApplyResources(lblThumbnailViewThumbnailClickAction, "lblThumbnailViewThumbnailClickAction");
            lblThumbnailViewThumbnailClickAction.Name = "lblThumbnailViewThumbnailClickAction";
            // 
            // lblThumbnailViewThumbnailSize
            // 
            resources.ApplyResources(lblThumbnailViewThumbnailSize, "lblThumbnailViewThumbnailSize");
            lblThumbnailViewThumbnailSize.Name = "lblThumbnailViewThumbnailSize";
            // 
            // cbThumbnailViewTitleLocation
            // 
            cbThumbnailViewTitleLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbThumbnailViewTitleLocation.FormattingEnabled = true;
            resources.ApplyResources(cbThumbnailViewTitleLocation, "cbThumbnailViewTitleLocation");
            cbThumbnailViewTitleLocation.Name = "cbThumbnailViewTitleLocation";
            cbThumbnailViewTitleLocation.SelectedIndexChanged += cbThumbnailViewTitleLocation_SelectedIndexChanged;
            // 
            // lblThumbnailViewTitleLocation
            // 
            resources.ApplyResources(lblThumbnailViewTitleLocation, "lblThumbnailViewTitleLocation");
            lblThumbnailViewTitleLocation.Name = "lblThumbnailViewTitleLocation";
            // 
            // cbThumbnailViewShowTitle
            // 
            resources.ApplyResources(cbThumbnailViewShowTitle, "cbThumbnailViewShowTitle");
            cbThumbnailViewShowTitle.Name = "cbThumbnailViewShowTitle";
            cbThumbnailViewShowTitle.UseVisualStyleBackColor = true;
            cbThumbnailViewShowTitle.CheckedChanged += cbThumbnailViewShowTitle_CheckedChanged;
            // 
            // tpClipboardFormats
            // 
            tpClipboardFormats.Controls.Add(lblClipboardFormatsTip);
            tpClipboardFormats.Controls.Add(btnClipboardFormatEdit);
            tpClipboardFormats.Controls.Add(btnClipboardFormatRemove);
            tpClipboardFormats.Controls.Add(btnClipboardFormatAdd);
            tpClipboardFormats.Controls.Add(lvClipboardFormats);
            resources.ApplyResources(tpClipboardFormats, "tpClipboardFormats");
            tpClipboardFormats.Name = "tpClipboardFormats";
            tpClipboardFormats.UseVisualStyleBackColor = true;
            // 
            // lblClipboardFormatsTip
            // 
            resources.ApplyResources(lblClipboardFormatsTip, "lblClipboardFormatsTip");
            lblClipboardFormatsTip.Name = "lblClipboardFormatsTip";
            // 
            // btnClipboardFormatEdit
            // 
            resources.ApplyResources(btnClipboardFormatEdit, "btnClipboardFormatEdit");
            btnClipboardFormatEdit.Name = "btnClipboardFormatEdit";
            btnClipboardFormatEdit.UseVisualStyleBackColor = true;
            btnClipboardFormatEdit.Click += btnClipboardFormatEdit_Click;
            // 
            // btnClipboardFormatRemove
            // 
            resources.ApplyResources(btnClipboardFormatRemove, "btnClipboardFormatRemove");
            btnClipboardFormatRemove.Name = "btnClipboardFormatRemove";
            btnClipboardFormatRemove.UseVisualStyleBackColor = true;
            btnClipboardFormatRemove.Click += btnClipboardFormatRemove_Click;
            // 
            // btnClipboardFormatAdd
            // 
            resources.ApplyResources(btnClipboardFormatAdd, "btnClipboardFormatAdd");
            btnClipboardFormatAdd.Name = "btnClipboardFormatAdd";
            btnClipboardFormatAdd.UseVisualStyleBackColor = true;
            btnClipboardFormatAdd.Click += btnAddClipboardFormat_Click;
            // 
            // lvClipboardFormats
            // 
            resources.ApplyResources(lvClipboardFormats, "lvClipboardFormats");
            lvClipboardFormats.AutoFillColumn = true;
            lvClipboardFormats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chDescription, chFormat });
            lvClipboardFormats.FullRowSelect = true;
            lvClipboardFormats.Name = "lvClipboardFormats";
            lvClipboardFormats.UseCompatibleStateImageBehavior = false;
            lvClipboardFormats.View = System.Windows.Forms.View.Details;
            lvClipboardFormats.MouseDoubleClick += lvClipboardFormats_MouseDoubleClick;
            // 
            // chDescription
            // 
            resources.ApplyResources(chDescription, "chDescription");
            // 
            // chFormat
            // 
            resources.ApplyResources(chFormat, "chFormat");
            // 
            // tpUpload
            // 
            tpUpload.BackColor = System.Drawing.SystemColors.Window;
            tpUpload.Controls.Add(gbSecondaryFileUploaders);
            tpUpload.Controls.Add(lblUploadLimit);
            tpUpload.Controls.Add(gbSecondaryImageUploaders);
            tpUpload.Controls.Add(gbSecondaryTextUploaders);
            tpUpload.Controls.Add(nudUploadLimit);
            tpUpload.Controls.Add(cbUseSecondaryUploaders);
            tpUpload.Controls.Add(lblUploadLimitHint);
            tpUpload.Controls.Add(cbIfUploadFailRetryOnce);
            tpUpload.Controls.Add(lblBufferSize);
            tpUpload.Controls.Add(nudRetryUpload);
            tpUpload.Controls.Add(cbBufferSize);
            resources.ApplyResources(tpUpload, "tpUpload");
            tpUpload.Name = "tpUpload";
            // 
            // gbSecondaryFileUploaders
            // 
            gbSecondaryFileUploaders.Controls.Add(lvSecondaryFileUploaders);
            resources.ApplyResources(gbSecondaryFileUploaders, "gbSecondaryFileUploaders");
            gbSecondaryFileUploaders.Name = "gbSecondaryFileUploaders";
            gbSecondaryFileUploaders.TabStop = false;
            // 
            // lvSecondaryFileUploaders
            // 
            lvSecondaryFileUploaders.AllowDrop = true;
            lvSecondaryFileUploaders.AllowItemDrag = true;
            lvSecondaryFileUploaders.AutoFillColumn = true;
            lvSecondaryFileUploaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvSecondaryFileUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chSecondaryFileUploaders });
            resources.ApplyResources(lvSecondaryFileUploaders, "lvSecondaryFileUploaders");
            lvSecondaryFileUploaders.FullRowSelect = true;
            lvSecondaryFileUploaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            lvSecondaryFileUploaders.MultiSelect = false;
            lvSecondaryFileUploaders.Name = "lvSecondaryFileUploaders";
            lvSecondaryFileUploaders.UseCompatibleStateImageBehavior = false;
            lvSecondaryFileUploaders.View = System.Windows.Forms.View.Details;
            lvSecondaryFileUploaders.MouseUp += lvSecondaryUploaders_MouseUp;
            // 
            // lblUploadLimit
            // 
            resources.ApplyResources(lblUploadLimit, "lblUploadLimit");
            lblUploadLimit.Name = "lblUploadLimit";
            // 
            // gbSecondaryImageUploaders
            // 
            gbSecondaryImageUploaders.Controls.Add(lvSecondaryImageUploaders);
            resources.ApplyResources(gbSecondaryImageUploaders, "gbSecondaryImageUploaders");
            gbSecondaryImageUploaders.Name = "gbSecondaryImageUploaders";
            gbSecondaryImageUploaders.TabStop = false;
            // 
            // lvSecondaryImageUploaders
            // 
            lvSecondaryImageUploaders.AllowDrop = true;
            lvSecondaryImageUploaders.AllowItemDrag = true;
            lvSecondaryImageUploaders.AutoFillColumn = true;
            lvSecondaryImageUploaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvSecondaryImageUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chSecondaryImageUploaders });
            resources.ApplyResources(lvSecondaryImageUploaders, "lvSecondaryImageUploaders");
            lvSecondaryImageUploaders.FullRowSelect = true;
            lvSecondaryImageUploaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            lvSecondaryImageUploaders.MultiSelect = false;
            lvSecondaryImageUploaders.Name = "lvSecondaryImageUploaders";
            lvSecondaryImageUploaders.UseCompatibleStateImageBehavior = false;
            lvSecondaryImageUploaders.View = System.Windows.Forms.View.Details;
            lvSecondaryImageUploaders.MouseUp += lvSecondaryUploaders_MouseUp;
            // 
            // gbSecondaryTextUploaders
            // 
            gbSecondaryTextUploaders.Controls.Add(lvSecondaryTextUploaders);
            resources.ApplyResources(gbSecondaryTextUploaders, "gbSecondaryTextUploaders");
            gbSecondaryTextUploaders.Name = "gbSecondaryTextUploaders";
            gbSecondaryTextUploaders.TabStop = false;
            // 
            // lvSecondaryTextUploaders
            // 
            lvSecondaryTextUploaders.AllowDrop = true;
            lvSecondaryTextUploaders.AllowItemDrag = true;
            lvSecondaryTextUploaders.AutoFillColumn = true;
            lvSecondaryTextUploaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvSecondaryTextUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chSecondaryTextUploaders });
            resources.ApplyResources(lvSecondaryTextUploaders, "lvSecondaryTextUploaders");
            lvSecondaryTextUploaders.FullRowSelect = true;
            lvSecondaryTextUploaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            lvSecondaryTextUploaders.MultiSelect = false;
            lvSecondaryTextUploaders.Name = "lvSecondaryTextUploaders";
            lvSecondaryTextUploaders.UseCompatibleStateImageBehavior = false;
            lvSecondaryTextUploaders.View = System.Windows.Forms.View.Details;
            lvSecondaryTextUploaders.MouseUp += lvSecondaryUploaders_MouseUp;
            // 
            // nudUploadLimit
            // 
            resources.ApplyResources(nudUploadLimit, "nudUploadLimit");
            nudUploadLimit.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            nudUploadLimit.Name = "nudUploadLimit";
            nudUploadLimit.Value = new decimal(new int[] { 5, 0, 0, 0 });
            nudUploadLimit.ValueChanged += nudUploadLimit_ValueChanged;
            // 
            // cbUseSecondaryUploaders
            // 
            resources.ApplyResources(cbUseSecondaryUploaders, "cbUseSecondaryUploaders");
            cbUseSecondaryUploaders.Name = "cbUseSecondaryUploaders";
            cbUseSecondaryUploaders.UseVisualStyleBackColor = true;
            cbUseSecondaryUploaders.CheckedChanged += cbUseSecondaryUploaders_CheckedChanged;
            // 
            // lblUploadLimitHint
            // 
            resources.ApplyResources(lblUploadLimitHint, "lblUploadLimitHint");
            lblUploadLimitHint.Name = "lblUploadLimitHint";
            // 
            // cbIfUploadFailRetryOnce
            // 
            resources.ApplyResources(cbIfUploadFailRetryOnce, "cbIfUploadFailRetryOnce");
            cbIfUploadFailRetryOnce.Name = "cbIfUploadFailRetryOnce";
            // 
            // lblBufferSize
            // 
            resources.ApplyResources(lblBufferSize, "lblBufferSize");
            lblBufferSize.Name = "lblBufferSize";
            // 
            // nudRetryUpload
            // 
            resources.ApplyResources(nudRetryUpload, "nudRetryUpload");
            nudRetryUpload.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            nudRetryUpload.Name = "nudRetryUpload";
            nudRetryUpload.ValueChanged += nudRetryUpload_ValueChanged;
            // 
            // cbBufferSize
            // 
            cbBufferSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbBufferSize.FormattingEnabled = true;
            resources.ApplyResources(cbBufferSize, "cbBufferSize");
            cbBufferSize.Name = "cbBufferSize";
            cbBufferSize.SelectedIndexChanged += cbBufferSize_SelectedIndexChanged;
            // 
            // tpHistory
            // 
            tpHistory.BackColor = System.Drawing.SystemColors.Window;
            tpHistory.Controls.Add(gbHistory);
            tpHistory.Controls.Add(gbRecentLinks);
            resources.ApplyResources(tpHistory, "tpHistory");
            tpHistory.Name = "tpHistory";
            // 
            // gbHistory
            // 
            gbHistory.Controls.Add(cbHistoryCheckURL);
            gbHistory.Controls.Add(cbHistorySaveTasks);
            resources.ApplyResources(gbHistory, "gbHistory");
            gbHistory.Name = "gbHistory";
            gbHistory.TabStop = false;
            // 
            // cbHistoryCheckURL
            // 
            resources.ApplyResources(cbHistoryCheckURL, "cbHistoryCheckURL");
            cbHistoryCheckURL.Name = "cbHistoryCheckURL";
            cbHistoryCheckURL.UseVisualStyleBackColor = true;
            cbHistoryCheckURL.CheckedChanged += cbHistoryCheckURL_CheckedChanged;
            // 
            // cbHistorySaveTasks
            // 
            resources.ApplyResources(cbHistorySaveTasks, "cbHistorySaveTasks");
            cbHistorySaveTasks.Name = "cbHistorySaveTasks";
            cbHistorySaveTasks.UseVisualStyleBackColor = true;
            cbHistorySaveTasks.CheckedChanged += cbHistorySaveTasks_CheckedChanged;
            // 
            // gbRecentLinks
            // 
            gbRecentLinks.Controls.Add(cbRecentTasksTrayMenuMostRecentFirst);
            gbRecentLinks.Controls.Add(lblRecentTasksMaxCount);
            gbRecentLinks.Controls.Add(nudRecentTasksMaxCount);
            gbRecentLinks.Controls.Add(cbRecentTasksShowInTrayMenu);
            gbRecentLinks.Controls.Add(cbRecentTasksShowInMainWindow);
            gbRecentLinks.Controls.Add(cbRecentTasksSave);
            resources.ApplyResources(gbRecentLinks, "gbRecentLinks");
            gbRecentLinks.Name = "gbRecentLinks";
            gbRecentLinks.TabStop = false;
            // 
            // cbRecentTasksTrayMenuMostRecentFirst
            // 
            resources.ApplyResources(cbRecentTasksTrayMenuMostRecentFirst, "cbRecentTasksTrayMenuMostRecentFirst");
            cbRecentTasksTrayMenuMostRecentFirst.Name = "cbRecentTasksTrayMenuMostRecentFirst";
            cbRecentTasksTrayMenuMostRecentFirst.UseVisualStyleBackColor = true;
            cbRecentTasksTrayMenuMostRecentFirst.CheckedChanged += cbRecentTasksTrayMenuMostRecentFirst_CheckedChanged;
            // 
            // lblRecentTasksMaxCount
            // 
            resources.ApplyResources(lblRecentTasksMaxCount, "lblRecentTasksMaxCount");
            lblRecentTasksMaxCount.Name = "lblRecentTasksMaxCount";
            // 
            // nudRecentTasksMaxCount
            // 
            resources.ApplyResources(nudRecentTasksMaxCount, "nudRecentTasksMaxCount");
            nudRecentTasksMaxCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudRecentTasksMaxCount.Name = "nudRecentTasksMaxCount";
            nudRecentTasksMaxCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudRecentTasksMaxCount.ValueChanged += nudRecentTasksMaxCount_ValueChanged;
            // 
            // cbRecentTasksShowInTrayMenu
            // 
            resources.ApplyResources(cbRecentTasksShowInTrayMenu, "cbRecentTasksShowInTrayMenu");
            cbRecentTasksShowInTrayMenu.Name = "cbRecentTasksShowInTrayMenu";
            cbRecentTasksShowInTrayMenu.UseVisualStyleBackColor = true;
            cbRecentTasksShowInTrayMenu.CheckedChanged += cbRecentTasksShowInTrayMenu_CheckedChanged;
            // 
            // cbRecentTasksShowInMainWindow
            // 
            resources.ApplyResources(cbRecentTasksShowInMainWindow, "cbRecentTasksShowInMainWindow");
            cbRecentTasksShowInMainWindow.Name = "cbRecentTasksShowInMainWindow";
            cbRecentTasksShowInMainWindow.UseVisualStyleBackColor = true;
            cbRecentTasksShowInMainWindow.CheckedChanged += cbRecentTasksShowInMainWindow_CheckedChanged;
            // 
            // cbRecentTasksSave
            // 
            resources.ApplyResources(cbRecentTasksSave, "cbRecentTasksSave");
            cbRecentTasksSave.Name = "cbRecentTasksSave";
            cbRecentTasksSave.UseVisualStyleBackColor = true;
            cbRecentTasksSave.CheckedChanged += cbRecentTasksSave_CheckedChanged;
            // 
            // tpPrint
            // 
            tpPrint.BackColor = System.Drawing.SystemColors.Window;
            tpPrint.Controls.Add(lblDefaultPrinterOverride);
            tpPrint.Controls.Add(txtDefaultPrinterOverride);
            tpPrint.Controls.Add(cbPrintDontShowWindowsDialog);
            tpPrint.Controls.Add(cbDontShowPrintSettingDialog);
            tpPrint.Controls.Add(btnShowImagePrintSettings);
            resources.ApplyResources(tpPrint, "tpPrint");
            tpPrint.Name = "tpPrint";
            // 
            // lblDefaultPrinterOverride
            // 
            resources.ApplyResources(lblDefaultPrinterOverride, "lblDefaultPrinterOverride");
            lblDefaultPrinterOverride.Name = "lblDefaultPrinterOverride";
            // 
            // txtDefaultPrinterOverride
            // 
            resources.ApplyResources(txtDefaultPrinterOverride, "txtDefaultPrinterOverride");
            txtDefaultPrinterOverride.Name = "txtDefaultPrinterOverride";
            txtDefaultPrinterOverride.TextChanged += txtDefaultPrinterOverride_TextChanged;
            // 
            // cbPrintDontShowWindowsDialog
            // 
            resources.ApplyResources(cbPrintDontShowWindowsDialog, "cbPrintDontShowWindowsDialog");
            cbPrintDontShowWindowsDialog.Name = "cbPrintDontShowWindowsDialog";
            cbPrintDontShowWindowsDialog.UseVisualStyleBackColor = true;
            cbPrintDontShowWindowsDialog.CheckedChanged += cbPrintDontShowWindowsDialog_CheckedChanged;
            // 
            // cbDontShowPrintSettingDialog
            // 
            resources.ApplyResources(cbDontShowPrintSettingDialog, "cbDontShowPrintSettingDialog");
            cbDontShowPrintSettingDialog.Name = "cbDontShowPrintSettingDialog";
            cbDontShowPrintSettingDialog.UseVisualStyleBackColor = true;
            cbDontShowPrintSettingDialog.CheckedChanged += cbDontShowPrintSettingDialog_CheckedChanged;
            // 
            // btnShowImagePrintSettings
            // 
            resources.ApplyResources(btnShowImagePrintSettings, "btnShowImagePrintSettings");
            btnShowImagePrintSettings.Name = "btnShowImagePrintSettings";
            btnShowImagePrintSettings.UseVisualStyleBackColor = true;
            btnShowImagePrintSettings.Click += btnShowImagePrintSettings_Click;
            // 
            // tpProxy
            // 
            tpProxy.BackColor = System.Drawing.SystemColors.Window;
            tpProxy.Controls.Add(cbProxyMethod);
            tpProxy.Controls.Add(lblProxyMethod);
            tpProxy.Controls.Add(lblProxyHost);
            tpProxy.Controls.Add(txtProxyHost);
            tpProxy.Controls.Add(nudProxyPort);
            tpProxy.Controls.Add(lblProxyPort);
            tpProxy.Controls.Add(lblProxyPassword);
            tpProxy.Controls.Add(txtProxyPassword);
            tpProxy.Controls.Add(lblProxyUsername);
            tpProxy.Controls.Add(txtProxyUsername);
            resources.ApplyResources(tpProxy, "tpProxy");
            tpProxy.Name = "tpProxy";
            // 
            // cbProxyMethod
            // 
            cbProxyMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbProxyMethod.FormattingEnabled = true;
            resources.ApplyResources(cbProxyMethod, "cbProxyMethod");
            cbProxyMethod.Name = "cbProxyMethod";
            cbProxyMethod.SelectedIndexChanged += cbProxyMethod_SelectedIndexChanged;
            // 
            // lblProxyMethod
            // 
            resources.ApplyResources(lblProxyMethod, "lblProxyMethod");
            lblProxyMethod.Name = "lblProxyMethod";
            // 
            // lblProxyHost
            // 
            resources.ApplyResources(lblProxyHost, "lblProxyHost");
            lblProxyHost.Name = "lblProxyHost";
            // 
            // txtProxyHost
            // 
            resources.ApplyResources(txtProxyHost, "txtProxyHost");
            txtProxyHost.Name = "txtProxyHost";
            txtProxyHost.TextChanged += txtProxyHost_TextChanged;
            // 
            // nudProxyPort
            // 
            resources.ApplyResources(nudProxyPort, "nudProxyPort");
            nudProxyPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudProxyPort.Name = "nudProxyPort";
            nudProxyPort.ValueChanged += nudProxyPort_ValueChanged;
            // 
            // lblProxyPort
            // 
            resources.ApplyResources(lblProxyPort, "lblProxyPort");
            lblProxyPort.Name = "lblProxyPort";
            // 
            // lblProxyPassword
            // 
            resources.ApplyResources(lblProxyPassword, "lblProxyPassword");
            lblProxyPassword.Name = "lblProxyPassword";
            // 
            // txtProxyPassword
            // 
            resources.ApplyResources(txtProxyPassword, "txtProxyPassword");
            txtProxyPassword.Name = "txtProxyPassword";
            txtProxyPassword.UseSystemPasswordChar = true;
            txtProxyPassword.TextChanged += txtProxyPassword_TextChanged;
            // 
            // lblProxyUsername
            // 
            resources.ApplyResources(lblProxyUsername, "lblProxyUsername");
            lblProxyUsername.Name = "lblProxyUsername";
            // 
            // txtProxyUsername
            // 
            resources.ApplyResources(txtProxyUsername, "txtProxyUsername");
            txtProxyUsername.Name = "txtProxyUsername";
            txtProxyUsername.TextChanged += txtProxyUsername_TextChanged;
            // 
            // tpAdvanced
            // 
            tpAdvanced.BackColor = System.Drawing.SystemColors.Window;
            tpAdvanced.Controls.Add(pgSettings);
            resources.ApplyResources(tpAdvanced, "tpAdvanced");
            tpAdvanced.Name = "tpAdvanced";
            // 
            // pgSettings
            // 
            pgSettings.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(pgSettings, "pgSettings");
            pgSettings.Name = "pgSettings";
            pgSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            pgSettings.ToolbarVisible = false;
            // 
            // tttvMain
            // 
            resources.ApplyResources(tttvMain, "tttvMain");
            tttvMain.ImageList = null;
            tttvMain.LeftPanelBackColor = System.Drawing.SystemColors.Window;
            tttvMain.MainTabControl = null;
            tttvMain.Name = "tttvMain";
            tttvMain.SeparatorColor = System.Drawing.SystemColors.ControlDark;
            tttvMain.TreeViewFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 162);
            tttvMain.TreeViewSize = 175;
            tttvMain.TabChanged += tttvMain_TabChanged;
            // 
            // ApplicationSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(tcSettings);
            Controls.Add(tttvMain);
            Name = "ApplicationSettingsForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Shown += SettingsForm_Shown;
            Resize += SettingsForm_Resize;
            tcSettings.ResumeLayout(false);
            tpGeneral.ResumeLayout(false);
            tpGeneral.PerformLayout();
            tpTheme.ResumeLayout(false);
            tpIntegration.ResumeLayout(false);
            gbFirefox.ResumeLayout(false);
            gbFirefox.PerformLayout();
            gbSteam.ResumeLayout(false);
            gbSteam.PerformLayout();
            gbChrome.ResumeLayout(false);
            gbChrome.PerformLayout();
            gbWindows.ResumeLayout(false);
            gbWindows.PerformLayout();
            tpPaths.ResumeLayout(false);
            tpPaths.PerformLayout();
            tpSettings.ResumeLayout(false);
            tpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudCleanupKeepFileCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbExportImportNote).EndInit();
            tpMainWindow.ResumeLayout(false);
            gbThumbnailView.ResumeLayout(false);
            gbThumbnailView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailViewThumbnailSizeHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailViewThumbnailSizeWidth).EndInit();
            tpClipboardFormats.ResumeLayout(false);
            tpClipboardFormats.PerformLayout();
            tpUpload.ResumeLayout(false);
            tpUpload.PerformLayout();
            gbSecondaryFileUploaders.ResumeLayout(false);
            gbSecondaryImageUploaders.ResumeLayout(false);
            gbSecondaryTextUploaders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudUploadLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRetryUpload).EndInit();
            tpHistory.ResumeLayout(false);
            gbHistory.ResumeLayout(false);
            gbHistory.PerformLayout();
            gbRecentLinks.ResumeLayout(false);
            gbRecentLinks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRecentTasksMaxCount).EndInit();
            tpPrint.ResumeLayout(false);
            tpPrint.PerformLayout();
            tpProxy.ResumeLayout(false);
            tpProxy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudProxyPort).EndInit();
            tpAdvanced.ResumeLayout(false);
            ResumeLayout(false);

        }



        #endregion Windows Form Designer generated code

        private System.Windows.Forms.TabControl tcSettings;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.TabPage tpProxy;
        private System.Windows.Forms.CheckBox cbSendToMenu;
        private System.Windows.Forms.Button btnOpenPersonalFolderPath;
        private System.Windows.Forms.CheckBox cbShowTray;
        private System.Windows.Forms.CheckBox cbStartWithWindows;
        private System.Windows.Forms.Label lblSaveImageSubFolderPatternPreview;
        private System.Windows.Forms.TextBox txtSaveImageSubFolderPattern;
        private System.Windows.Forms.Label lblSaveImageSubFolderPattern;
        private System.Windows.Forms.CheckBox cbUseCustomScreenshotsPath;
        private System.Windows.Forms.TabPage tpPaths;
        private System.Windows.Forms.Button btnBrowseCustomScreenshotsPath;
        private System.Windows.Forms.TextBox txtCustomScreenshotsPath;
        private System.Windows.Forms.Label lblProxyHost;
        private System.Windows.Forms.TextBox txtProxyHost;
        private System.Windows.Forms.NumericUpDown nudProxyPort;
        private System.Windows.Forms.Label lblProxyPort;
        private System.Windows.Forms.Label lblProxyPassword;
        private System.Windows.Forms.TextBox txtProxyPassword;
        private System.Windows.Forms.Label lblProxyUsername;
        private System.Windows.Forms.TextBox txtProxyUsername;
        private System.Windows.Forms.CheckBox cbShellContextMenu;
        private System.Windows.Forms.ComboBox cbProxyMethod;
        private System.Windows.Forms.Label lblProxyMethod;
        private System.Windows.Forms.TabPage tpUpload;
        private System.Windows.Forms.Label cbIfUploadFailRetryOnce;
        private System.Windows.Forms.Label lblUploadLimit;
        private System.Windows.Forms.ComboBox cbBufferSize;
        private System.Windows.Forms.Label lblUploadLimitHint;
        private System.Windows.Forms.Label lblBufferSize;
        private System.Windows.Forms.NumericUpDown nudUploadLimit;
        private System.Windows.Forms.Button btnClipboardFormatRemove;
        private System.Windows.Forms.Button btnClipboardFormatAdd;
        private HelpersLib.MyListView lvClipboardFormats;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader chFormat;
        private System.Windows.Forms.Button btnClipboardFormatEdit;
        private System.Windows.Forms.TabPage tpPrint;
        private System.Windows.Forms.CheckBox cbDontShowPrintSettingDialog;
        private System.Windows.Forms.Button btnShowImagePrintSettings;
        private System.Windows.Forms.TabPage tpAdvanced;
        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.CheckBox cbTaskbarProgressEnabled;
        private System.Windows.Forms.CheckBox cbTrayIconProgressEnabled;
        private System.Windows.Forms.CheckBox cbRememberMainFormSize;
        private System.Windows.Forms.Label lblPreviewPersonalFolderPath;
        private System.Windows.Forms.Button btnBrowsePersonalFolderPath;
        private System.Windows.Forms.Label lblPersonalFolderPath;
        private System.Windows.Forms.TextBox txtPersonalFolderPath;
        private System.Windows.Forms.Button btnOpenScreenshotsFolder;
        private System.Windows.Forms.CheckBox cbSilentRun;
        private System.Windows.Forms.NumericUpDown nudRetryUpload;
        private System.Windows.Forms.GroupBox gbSecondaryImageUploaders;
        private MyListView lvSecondaryImageUploaders;
        private System.Windows.Forms.GroupBox gbSecondaryFileUploaders;
        private MyListView lvSecondaryFileUploaders;
        private System.Windows.Forms.GroupBox gbSecondaryTextUploaders;
        private MyListView lvSecondaryTextUploaders;
        private System.Windows.Forms.CheckBox cbUseSecondaryUploaders;
        private System.Windows.Forms.ColumnHeader chSecondaryImageUploaders;
        private System.Windows.Forms.ColumnHeader chSecondaryFileUploaders;
        private System.Windows.Forms.ColumnHeader chSecondaryTextUploaders;
        private System.Windows.Forms.CheckBox cbPrintDontShowWindowsDialog;
        private System.Windows.Forms.CheckBox cbRememberMainFormPosition;
        private System.Windows.Forms.Label lblLanguage;
        private TabToTreeView tttvMain;
        private MenuButton btnLanguages;
        private System.Windows.Forms.ContextMenuStrip cmsLanguages;
        private System.Windows.Forms.GroupBox gbWindows;
        private System.Windows.Forms.GroupBox gbChrome;
        private System.Windows.Forms.CheckBox cbSteamShowInApp;
        private System.Windows.Forms.TabPage tpIntegration;
        private System.Windows.Forms.GroupBox gbSteam;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ProgressBar pbExportImport;
        private System.Windows.Forms.Button btnEditQuickTaskMenu;
        private System.Windows.Forms.TabPage tpHistory;
        private System.Windows.Forms.GroupBox gbRecentLinks;
        private System.Windows.Forms.CheckBox cbRecentTasksSave;
        private System.Windows.Forms.CheckBox cbRecentTasksShowInTrayMenu;
        private System.Windows.Forms.CheckBox cbRecentTasksShowInMainWindow;
        private System.Windows.Forms.Label lblRecentTasksMaxCount;
        private System.Windows.Forms.NumericUpDown nudRecentTasksMaxCount;
        private System.Windows.Forms.CheckBox cbRecentTasksTrayMenuMostRecentFirst;
        private System.Windows.Forms.GroupBox gbHistory;
        private System.Windows.Forms.CheckBox cbHistorySaveTasks;
        private System.Windows.Forms.CheckBox cbHistoryCheckURL;
        private System.Windows.Forms.Label lblTrayMiddleClickAction;
        private System.Windows.Forms.Label lblTrayLeftDoubleClickAction;
        private System.Windows.Forms.Label lblTrayLeftClickAction;
        private System.Windows.Forms.ComboBox cbTrayMiddleClickAction;
        private System.Windows.Forms.ComboBox cbTrayLeftDoubleClickAction;
        private System.Windows.Forms.ComboBox cbTrayLeftClickAction;
        private System.Windows.Forms.Button btnChromeOpenExtensionPage;
        private System.Windows.Forms.GroupBox gbFirefox;
        private System.Windows.Forms.Button btnFirefoxOpenAddonPage;
        private System.Windows.Forms.CheckBox cbChromeExtensionSupport;
        private System.Windows.Forms.CheckBox cbFirefoxAddonSupport;
        private System.Windows.Forms.Button btnResetSettings;
        private System.Windows.Forms.CheckBox cbEditWithShareX;
        private System.Windows.Forms.Button btnCheckDevBuild;
        private System.Windows.Forms.Button btnPersonalFolderPathApply;
        private System.Windows.Forms.CheckBox cbUseWhiteShareXIcon;
        private System.Windows.Forms.TabPage tpTheme;
        private System.Windows.Forms.PropertyGrid pgTheme;
        private System.Windows.Forms.ComboBox cbThemes;
        private System.Windows.Forms.Button btnThemeRemove;
        private System.Windows.Forms.Button btnThemeAdd;
        private ExportImportControl eiTheme;
        private System.Windows.Forms.Button btnThemeReset;
        private System.Windows.Forms.Label lblExportImportNote;
        private System.Windows.Forms.CheckBox cbExportHistory;
        private System.Windows.Forms.CheckBox cbExportSettings;
        private System.Windows.Forms.PictureBox pbExportImportNote;
        private System.Windows.Forms.CheckBox cbAutomaticallyCleanupBackupFiles;
        private System.Windows.Forms.NumericUpDown nudCleanupKeepFileCount;
        private System.Windows.Forms.Label lblCleanupKeepFileCount;
        private System.Windows.Forms.CheckBox cbAutomaticallyCleanupLogFiles;
        private System.Windows.Forms.Label lblDefaultPrinterOverride;
        private System.Windows.Forms.TextBox txtDefaultPrinterOverride;
        private System.Windows.Forms.TabPage tpMainWindow;
        private System.Windows.Forms.GroupBox gbThumbnailView;
        private System.Windows.Forms.CheckBox cbThumbnailViewShowTitle;
        private System.Windows.Forms.ComboBox cbThumbnailViewTitleLocation;
        private System.Windows.Forms.Label lblThumbnailViewTitleLocation;
        private System.Windows.Forms.Label lblThumbnailViewThumbnailSize;
        private System.Windows.Forms.Label lblThumbnailViewThumbnailClickAction;
        private System.Windows.Forms.ComboBox cbThumbnailViewThumbnailClickAction;
        private System.Windows.Forms.NumericUpDown nudThumbnailViewThumbnailSizeHeight;
        private System.Windows.Forms.NumericUpDown nudThumbnailViewThumbnailSizeWidth;
        private System.Windows.Forms.Label lblThumbnailViewThumbnailSizeX;
        private System.Windows.Forms.Button btnThumbnailViewThumbnailSizeReset;
        private System.Windows.Forms.TabPage tpClipboardFormats;
        private System.Windows.Forms.Label lblClipboardFormatsTip;
        private System.Windows.Forms.TextBox txtSaveImageSubFolderPatternWindow;
        private System.Windows.Forms.Label lblSaveImageSubFolderPatternWindow;
        private System.Windows.Forms.CheckBox cbAutoCheckUpdate;
        private System.Windows.Forms.ComboBox cbUpdateChannel;
        private System.Windows.Forms.Label lblUpdateChannel;
    }
}