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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationSettingsForm));
            this.tcSettings = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.cbCheckPreReleaseUpdates = new System.Windows.Forms.CheckBox();
            this.cbTrayMiddleClickAction = new System.Windows.Forms.ComboBox();
            this.lblTrayMiddleClickAction = new System.Windows.Forms.Label();
            this.cbTrayLeftDoubleClickAction = new System.Windows.Forms.ComboBox();
            this.lblTrayLeftDoubleClickAction = new System.Windows.Forms.Label();
            this.cbTrayLeftClickAction = new System.Windows.Forms.ComboBox();
            this.lblTrayLeftClickAction = new System.Windows.Forms.Label();
            this.btnEditQuickTaskMenu = new System.Windows.Forms.Button();
            this.cbShowTray = new System.Windows.Forms.CheckBox();
            this.cbTrayIconProgressEnabled = new System.Windows.Forms.CheckBox();
            this.btnLanguages = new ShareX.HelpersLib.MenuButton();
            this.cmsLanguages = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cbRememberMainFormPosition = new System.Windows.Forms.CheckBox();
            this.cbSilentRun = new System.Windows.Forms.CheckBox();
            this.cbTaskbarProgressEnabled = new System.Windows.Forms.CheckBox();
            this.cbRememberMainFormSize = new System.Windows.Forms.CheckBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.tpIntegration = new System.Windows.Forms.TabPage();
            this.gbSteam = new System.Windows.Forms.GroupBox();
            this.cbSteamShowInApp = new System.Windows.Forms.CheckBox();
            this.gbChrome = new System.Windows.Forms.GroupBox();
            this.gbWindows = new System.Windows.Forms.GroupBox();
            this.cbStartWithWindows = new System.Windows.Forms.CheckBox();
            this.cbSendToMenu = new System.Windows.Forms.CheckBox();
            this.cbShellContextMenu = new System.Windows.Forms.CheckBox();
            this.tpPaths = new System.Windows.Forms.TabPage();
            this.lblNotePersonalFolderPath = new System.Windows.Forms.Label();
            this.btnOpenScreenshotsFolder = new System.Windows.Forms.Button();
            this.lblPreviewPersonalFolderPath = new System.Windows.Forms.Label();
            this.btnBrowsePersonalFolderPath = new System.Windows.Forms.Button();
            this.lblPersonalFolderPath = new System.Windows.Forms.Label();
            this.txtPersonalFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseCustomScreenshotsPath = new System.Windows.Forms.Button();
            this.btnOpenPersonalFolderPath = new System.Windows.Forms.Button();
            this.txtCustomScreenshotsPath = new System.Windows.Forms.TextBox();
            this.cbUseCustomScreenshotsPath = new System.Windows.Forms.CheckBox();
            this.lblSaveImageSubFolderPattern = new System.Windows.Forms.Label();
            this.lblSaveImageSubFolderPatternPreview = new System.Windows.Forms.Label();
            this.txtSaveImageSubFolderPattern = new System.Windows.Forms.TextBox();
            this.tpExportImport = new System.Windows.Forms.TabPage();
            this.cbExportLogs = new System.Windows.Forms.CheckBox();
            this.cbExportHistory = new System.Windows.Forms.CheckBox();
            this.cbExportSettings = new System.Windows.Forms.CheckBox();
            this.pbExportImport = new System.Windows.Forms.ProgressBar();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.tpUpload = new System.Windows.Forms.TabPage();
            this.tcUpload = new System.Windows.Forms.TabControl();
            this.tpPerformance = new System.Windows.Forms.TabPage();
            this.lblUploadLimit = new System.Windows.Forms.Label();
            this.nudUploadLimit = new System.Windows.Forms.NumericUpDown();
            this.lblUploadLimitHint = new System.Windows.Forms.Label();
            this.cbBufferSize = new System.Windows.Forms.ComboBox();
            this.lblBufferSize = new System.Windows.Forms.Label();
            this.tpUploadResults = new System.Windows.Forms.TabPage();
            this.gbClipboardFormats = new System.Windows.Forms.GroupBox();
            this.btnClipboardFormatEdit = new System.Windows.Forms.Button();
            this.btnClipboardFormatRemove = new System.Windows.Forms.Button();
            this.btnClipboardFormatAdd = new System.Windows.Forms.Button();
            this.lvClipboardFormats = new ShareX.HelpersLib.MyListView();
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpUploadRetry = new System.Windows.Forms.TabPage();
            this.chkUseSecondaryUploaders = new System.Windows.Forms.CheckBox();
            this.tlpBackupDestinations = new System.Windows.Forms.TableLayoutPanel();
            this.gbSecondaryImageUploaders = new System.Windows.Forms.GroupBox();
            this.lvSecondaryImageUploaders = new ShareX.HelpersLib.MyListView();
            this.chSecondaryImageUploaders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSecondaryFileUploaders = new System.Windows.Forms.GroupBox();
            this.lvSecondaryFileUploaders = new ShareX.HelpersLib.MyListView();
            this.chSecondaryFileUploaders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSecondaryTextUploaders = new System.Windows.Forms.GroupBox();
            this.lvSecondaryTextUploaders = new ShareX.HelpersLib.MyListView();
            this.chSecondaryTextUploaders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbIfUploadFailRetryOnce = new System.Windows.Forms.Label();
            this.nudRetryUpload = new System.Windows.Forms.NumericUpDown();
            this.tpHistory = new System.Windows.Forms.TabPage();
            this.gbHistory = new System.Windows.Forms.GroupBox();
            this.cbHistoryCheckURL = new System.Windows.Forms.CheckBox();
            this.cbHistorySaveTasks = new System.Windows.Forms.CheckBox();
            this.gbRecentLinks = new System.Windows.Forms.GroupBox();
            this.cbRecentTasksTrayMenuMostRecentFirst = new System.Windows.Forms.CheckBox();
            this.lblRecentTasksMaxCount = new System.Windows.Forms.Label();
            this.nudRecentTasksMaxCount = new System.Windows.Forms.NumericUpDown();
            this.cbRecentTasksShowInTrayMenu = new System.Windows.Forms.CheckBox();
            this.cbRecentTasksShowInMainWindow = new System.Windows.Forms.CheckBox();
            this.cbRecentTasksSave = new System.Windows.Forms.CheckBox();
            this.tpPrint = new System.Windows.Forms.TabPage();
            this.cbPrintDontShowWindowsDialog = new System.Windows.Forms.CheckBox();
            this.cbDontShowPrintSettingDialog = new System.Windows.Forms.CheckBox();
            this.btnShowImagePrintSettings = new System.Windows.Forms.Button();
            this.tpProxy = new System.Windows.Forms.TabPage();
            this.cbProxyMethod = new System.Windows.Forms.ComboBox();
            this.lblProxyMethod = new System.Windows.Forms.Label();
            this.lblProxyHost = new System.Windows.Forms.Label();
            this.txtProxyHost = new System.Windows.Forms.TextBox();
            this.nudProxyPort = new System.Windows.Forms.NumericUpDown();
            this.lblProxyPort = new System.Windows.Forms.Label();
            this.lblProxyPassword = new System.Windows.Forms.Label();
            this.txtProxyPassword = new System.Windows.Forms.TextBox();
            this.lblProxyUsername = new System.Windows.Forms.Label();
            this.txtProxyUsername = new System.Windows.Forms.TextBox();
            this.tpAdvanced = new System.Windows.Forms.TabPage();
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.tttvMain = new ShareX.HelpersLib.TabToTreeView();
            this.btnChromeEnableSupport = new System.Windows.Forms.Button();
            this.btnChromeDisableSupport = new System.Windows.Forms.Button();
            this.btnChromeOpenExtensionPage = new System.Windows.Forms.Button();
            this.gpFirefox = new System.Windows.Forms.GroupBox();
            this.btnFirefoxOpenAddonPage = new System.Windows.Forms.Button();
            this.btnFirefoxDisableSupport = new System.Windows.Forms.Button();
            this.btnFirefoxEnableSupport = new System.Windows.Forms.Button();
            this.tcSettings.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.tpIntegration.SuspendLayout();
            this.gbSteam.SuspendLayout();
            this.gbChrome.SuspendLayout();
            this.gbWindows.SuspendLayout();
            this.tpPaths.SuspendLayout();
            this.tpExportImport.SuspendLayout();
            this.tpUpload.SuspendLayout();
            this.tcUpload.SuspendLayout();
            this.tpPerformance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUploadLimit)).BeginInit();
            this.tpUploadResults.SuspendLayout();
            this.gbClipboardFormats.SuspendLayout();
            this.tpUploadRetry.SuspendLayout();
            this.tlpBackupDestinations.SuspendLayout();
            this.gbSecondaryImageUploaders.SuspendLayout();
            this.gbSecondaryFileUploaders.SuspendLayout();
            this.gbSecondaryTextUploaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryUpload)).BeginInit();
            this.tpHistory.SuspendLayout();
            this.gbHistory.SuspendLayout();
            this.gbRecentLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentTasksMaxCount)).BeginInit();
            this.tpPrint.SuspendLayout();
            this.tpProxy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudProxyPort)).BeginInit();
            this.tpAdvanced.SuspendLayout();
            this.gpFirefox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcSettings
            // 
            resources.ApplyResources(this.tcSettings, "tcSettings");
            this.tcSettings.Controls.Add(this.tpGeneral);
            this.tcSettings.Controls.Add(this.tpIntegration);
            this.tcSettings.Controls.Add(this.tpPaths);
            this.tcSettings.Controls.Add(this.tpExportImport);
            this.tcSettings.Controls.Add(this.tpUpload);
            this.tcSettings.Controls.Add(this.tpHistory);
            this.tcSettings.Controls.Add(this.tpPrint);
            this.tcSettings.Controls.Add(this.tpProxy);
            this.tcSettings.Controls.Add(this.tpAdvanced);
            this.tcSettings.Name = "tcSettings";
            this.tcSettings.SelectedIndex = 0;
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.cbCheckPreReleaseUpdates);
            this.tpGeneral.Controls.Add(this.cbTrayMiddleClickAction);
            this.tpGeneral.Controls.Add(this.lblTrayMiddleClickAction);
            this.tpGeneral.Controls.Add(this.cbTrayLeftDoubleClickAction);
            this.tpGeneral.Controls.Add(this.lblTrayLeftDoubleClickAction);
            this.tpGeneral.Controls.Add(this.cbTrayLeftClickAction);
            this.tpGeneral.Controls.Add(this.lblTrayLeftClickAction);
            this.tpGeneral.Controls.Add(this.btnEditQuickTaskMenu);
            this.tpGeneral.Controls.Add(this.cbShowTray);
            this.tpGeneral.Controls.Add(this.cbTrayIconProgressEnabled);
            this.tpGeneral.Controls.Add(this.btnLanguages);
            this.tpGeneral.Controls.Add(this.cbRememberMainFormPosition);
            this.tpGeneral.Controls.Add(this.cbSilentRun);
            this.tpGeneral.Controls.Add(this.cbTaskbarProgressEnabled);
            this.tpGeneral.Controls.Add(this.cbRememberMainFormSize);
            this.tpGeneral.Controls.Add(this.lblLanguage);
            resources.ApplyResources(this.tpGeneral, "tpGeneral");
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // cbCheckPreReleaseUpdates
            // 
            resources.ApplyResources(this.cbCheckPreReleaseUpdates, "cbCheckPreReleaseUpdates");
            this.cbCheckPreReleaseUpdates.Name = "cbCheckPreReleaseUpdates";
            this.cbCheckPreReleaseUpdates.UseVisualStyleBackColor = true;
            this.cbCheckPreReleaseUpdates.CheckedChanged += new System.EventHandler(this.cbCheckPreReleaseUpdates_CheckedChanged);
            // 
            // cbTrayMiddleClickAction
            // 
            this.cbTrayMiddleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrayMiddleClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbTrayMiddleClickAction, "cbTrayMiddleClickAction");
            this.cbTrayMiddleClickAction.Name = "cbTrayMiddleClickAction";
            this.cbTrayMiddleClickAction.SelectedIndexChanged += new System.EventHandler(this.cbTrayMiddleClickAction_SelectedIndexChanged);
            // 
            // lblTrayMiddleClickAction
            // 
            resources.ApplyResources(this.lblTrayMiddleClickAction, "lblTrayMiddleClickAction");
            this.lblTrayMiddleClickAction.Name = "lblTrayMiddleClickAction";
            // 
            // cbTrayLeftDoubleClickAction
            // 
            this.cbTrayLeftDoubleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrayLeftDoubleClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbTrayLeftDoubleClickAction, "cbTrayLeftDoubleClickAction");
            this.cbTrayLeftDoubleClickAction.Name = "cbTrayLeftDoubleClickAction";
            this.cbTrayLeftDoubleClickAction.SelectedIndexChanged += new System.EventHandler(this.cbTrayLeftDoubleClickAction_SelectedIndexChanged);
            // 
            // lblTrayLeftDoubleClickAction
            // 
            resources.ApplyResources(this.lblTrayLeftDoubleClickAction, "lblTrayLeftDoubleClickAction");
            this.lblTrayLeftDoubleClickAction.Name = "lblTrayLeftDoubleClickAction";
            // 
            // cbTrayLeftClickAction
            // 
            this.cbTrayLeftClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrayLeftClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbTrayLeftClickAction, "cbTrayLeftClickAction");
            this.cbTrayLeftClickAction.Name = "cbTrayLeftClickAction";
            this.cbTrayLeftClickAction.SelectedIndexChanged += new System.EventHandler(this.cbTrayLeftClickAction_SelectedIndexChanged);
            // 
            // lblTrayLeftClickAction
            // 
            resources.ApplyResources(this.lblTrayLeftClickAction, "lblTrayLeftClickAction");
            this.lblTrayLeftClickAction.Name = "lblTrayLeftClickAction";
            // 
            // btnEditQuickTaskMenu
            // 
            resources.ApplyResources(this.btnEditQuickTaskMenu, "btnEditQuickTaskMenu");
            this.btnEditQuickTaskMenu.Name = "btnEditQuickTaskMenu";
            this.btnEditQuickTaskMenu.UseVisualStyleBackColor = true;
            this.btnEditQuickTaskMenu.Click += new System.EventHandler(this.btnEditQuickTaskMenu_Click);
            // 
            // cbShowTray
            // 
            resources.ApplyResources(this.cbShowTray, "cbShowTray");
            this.cbShowTray.Name = "cbShowTray";
            this.cbShowTray.UseVisualStyleBackColor = true;
            this.cbShowTray.CheckedChanged += new System.EventHandler(this.cbShowTray_CheckedChanged);
            // 
            // cbTrayIconProgressEnabled
            // 
            resources.ApplyResources(this.cbTrayIconProgressEnabled, "cbTrayIconProgressEnabled");
            this.cbTrayIconProgressEnabled.Name = "cbTrayIconProgressEnabled";
            this.cbTrayIconProgressEnabled.UseVisualStyleBackColor = true;
            this.cbTrayIconProgressEnabled.CheckedChanged += new System.EventHandler(this.cbTrayIconProgressEnabled_CheckedChanged);
            // 
            // btnLanguages
            // 
            resources.ApplyResources(this.btnLanguages, "btnLanguages");
            this.btnLanguages.Menu = this.cmsLanguages;
            this.btnLanguages.Name = "btnLanguages";
            this.btnLanguages.UseVisualStyleBackColor = true;
            // 
            // cmsLanguages
            // 
            this.cmsLanguages.Name = "cmsLanguages";
            resources.ApplyResources(this.cmsLanguages, "cmsLanguages");
            // 
            // cbRememberMainFormPosition
            // 
            resources.ApplyResources(this.cbRememberMainFormPosition, "cbRememberMainFormPosition");
            this.cbRememberMainFormPosition.Name = "cbRememberMainFormPosition";
            this.cbRememberMainFormPosition.UseVisualStyleBackColor = true;
            this.cbRememberMainFormPosition.CheckedChanged += new System.EventHandler(this.cbRememberMainFormPosition_CheckedChanged);
            // 
            // cbSilentRun
            // 
            resources.ApplyResources(this.cbSilentRun, "cbSilentRun");
            this.cbSilentRun.Name = "cbSilentRun";
            this.cbSilentRun.UseVisualStyleBackColor = true;
            this.cbSilentRun.CheckedChanged += new System.EventHandler(this.cbSilentRun_CheckedChanged);
            // 
            // cbTaskbarProgressEnabled
            // 
            resources.ApplyResources(this.cbTaskbarProgressEnabled, "cbTaskbarProgressEnabled");
            this.cbTaskbarProgressEnabled.Name = "cbTaskbarProgressEnabled";
            this.cbTaskbarProgressEnabled.UseVisualStyleBackColor = true;
            this.cbTaskbarProgressEnabled.CheckedChanged += new System.EventHandler(this.cbTaskbarProgressEnabled_CheckedChanged);
            // 
            // cbRememberMainFormSize
            // 
            resources.ApplyResources(this.cbRememberMainFormSize, "cbRememberMainFormSize");
            this.cbRememberMainFormSize.Name = "cbRememberMainFormSize";
            this.cbRememberMainFormSize.UseVisualStyleBackColor = true;
            this.cbRememberMainFormSize.CheckedChanged += new System.EventHandler(this.cbRememberMainFormSize_CheckedChanged);
            // 
            // lblLanguage
            // 
            resources.ApplyResources(this.lblLanguage, "lblLanguage");
            this.lblLanguage.Name = "lblLanguage";
            // 
            // tpIntegration
            // 
            this.tpIntegration.Controls.Add(this.gpFirefox);
            this.tpIntegration.Controls.Add(this.gbSteam);
            this.tpIntegration.Controls.Add(this.gbChrome);
            this.tpIntegration.Controls.Add(this.gbWindows);
            resources.ApplyResources(this.tpIntegration, "tpIntegration");
            this.tpIntegration.Name = "tpIntegration";
            this.tpIntegration.UseVisualStyleBackColor = true;
            // 
            // gbSteam
            // 
            this.gbSteam.Controls.Add(this.cbSteamShowInApp);
            resources.ApplyResources(this.gbSteam, "gbSteam");
            this.gbSteam.Name = "gbSteam";
            this.gbSteam.TabStop = false;
            // 
            // cbSteamShowInApp
            // 
            resources.ApplyResources(this.cbSteamShowInApp, "cbSteamShowInApp");
            this.cbSteamShowInApp.Name = "cbSteamShowInApp";
            this.cbSteamShowInApp.UseVisualStyleBackColor = true;
            this.cbSteamShowInApp.CheckedChanged += new System.EventHandler(this.cbSteamShowInApp_CheckedChanged);
            // 
            // gbChrome
            // 
            this.gbChrome.Controls.Add(this.btnChromeOpenExtensionPage);
            this.gbChrome.Controls.Add(this.btnChromeDisableSupport);
            this.gbChrome.Controls.Add(this.btnChromeEnableSupport);
            resources.ApplyResources(this.gbChrome, "gbChrome");
            this.gbChrome.Name = "gbChrome";
            this.gbChrome.TabStop = false;
            // 
            // gbWindows
            // 
            this.gbWindows.Controls.Add(this.cbStartWithWindows);
            this.gbWindows.Controls.Add(this.cbSendToMenu);
            this.gbWindows.Controls.Add(this.cbShellContextMenu);
            resources.ApplyResources(this.gbWindows, "gbWindows");
            this.gbWindows.Name = "gbWindows";
            this.gbWindows.TabStop = false;
            // 
            // cbStartWithWindows
            // 
            resources.ApplyResources(this.cbStartWithWindows, "cbStartWithWindows");
            this.cbStartWithWindows.Name = "cbStartWithWindows";
            this.cbStartWithWindows.UseVisualStyleBackColor = true;
            this.cbStartWithWindows.CheckedChanged += new System.EventHandler(this.cbStartWithWindows_CheckedChanged);
            // 
            // cbSendToMenu
            // 
            resources.ApplyResources(this.cbSendToMenu, "cbSendToMenu");
            this.cbSendToMenu.Name = "cbSendToMenu";
            this.cbSendToMenu.UseVisualStyleBackColor = true;
            this.cbSendToMenu.CheckedChanged += new System.EventHandler(this.cbSendToMenu_CheckedChanged);
            // 
            // cbShellContextMenu
            // 
            resources.ApplyResources(this.cbShellContextMenu, "cbShellContextMenu");
            this.cbShellContextMenu.Name = "cbShellContextMenu";
            this.cbShellContextMenu.UseVisualStyleBackColor = true;
            this.cbShellContextMenu.CheckedChanged += new System.EventHandler(this.cbShellContextMenu_CheckedChanged);
            // 
            // tpPaths
            // 
            this.tpPaths.Controls.Add(this.lblNotePersonalFolderPath);
            this.tpPaths.Controls.Add(this.btnOpenScreenshotsFolder);
            this.tpPaths.Controls.Add(this.lblPreviewPersonalFolderPath);
            this.tpPaths.Controls.Add(this.btnBrowsePersonalFolderPath);
            this.tpPaths.Controls.Add(this.lblPersonalFolderPath);
            this.tpPaths.Controls.Add(this.txtPersonalFolderPath);
            this.tpPaths.Controls.Add(this.btnBrowseCustomScreenshotsPath);
            this.tpPaths.Controls.Add(this.btnOpenPersonalFolderPath);
            this.tpPaths.Controls.Add(this.txtCustomScreenshotsPath);
            this.tpPaths.Controls.Add(this.cbUseCustomScreenshotsPath);
            this.tpPaths.Controls.Add(this.lblSaveImageSubFolderPattern);
            this.tpPaths.Controls.Add(this.lblSaveImageSubFolderPatternPreview);
            this.tpPaths.Controls.Add(this.txtSaveImageSubFolderPattern);
            resources.ApplyResources(this.tpPaths, "tpPaths");
            this.tpPaths.Name = "tpPaths";
            this.tpPaths.UseVisualStyleBackColor = true;
            // 
            // lblNotePersonalFolderPath
            // 
            resources.ApplyResources(this.lblNotePersonalFolderPath, "lblNotePersonalFolderPath");
            this.lblNotePersonalFolderPath.Name = "lblNotePersonalFolderPath";
            // 
            // btnOpenScreenshotsFolder
            // 
            resources.ApplyResources(this.btnOpenScreenshotsFolder, "btnOpenScreenshotsFolder");
            this.btnOpenScreenshotsFolder.Name = "btnOpenScreenshotsFolder";
            this.btnOpenScreenshotsFolder.UseVisualStyleBackColor = true;
            this.btnOpenScreenshotsFolder.Click += new System.EventHandler(this.btnOpenScreenshotsFolder_Click);
            // 
            // lblPreviewPersonalFolderPath
            // 
            resources.ApplyResources(this.lblPreviewPersonalFolderPath, "lblPreviewPersonalFolderPath");
            this.lblPreviewPersonalFolderPath.Name = "lblPreviewPersonalFolderPath";
            // 
            // btnBrowsePersonalFolderPath
            // 
            resources.ApplyResources(this.btnBrowsePersonalFolderPath, "btnBrowsePersonalFolderPath");
            this.btnBrowsePersonalFolderPath.Name = "btnBrowsePersonalFolderPath";
            this.btnBrowsePersonalFolderPath.UseVisualStyleBackColor = true;
            this.btnBrowsePersonalFolderPath.Click += new System.EventHandler(this.btnBrowsePersonalFolderPath_Click);
            // 
            // lblPersonalFolderPath
            // 
            resources.ApplyResources(this.lblPersonalFolderPath, "lblPersonalFolderPath");
            this.lblPersonalFolderPath.Name = "lblPersonalFolderPath";
            // 
            // txtPersonalFolderPath
            // 
            resources.ApplyResources(this.txtPersonalFolderPath, "txtPersonalFolderPath");
            this.txtPersonalFolderPath.Name = "txtPersonalFolderPath";
            this.txtPersonalFolderPath.TextChanged += new System.EventHandler(this.txtPersonalFolderPath_TextChanged);
            // 
            // btnBrowseCustomScreenshotsPath
            // 
            resources.ApplyResources(this.btnBrowseCustomScreenshotsPath, "btnBrowseCustomScreenshotsPath");
            this.btnBrowseCustomScreenshotsPath.Name = "btnBrowseCustomScreenshotsPath";
            this.btnBrowseCustomScreenshotsPath.UseVisualStyleBackColor = true;
            this.btnBrowseCustomScreenshotsPath.Click += new System.EventHandler(this.btnBrowseCustomScreenshotsPath_Click);
            // 
            // btnOpenPersonalFolderPath
            // 
            resources.ApplyResources(this.btnOpenPersonalFolderPath, "btnOpenPersonalFolderPath");
            this.btnOpenPersonalFolderPath.Name = "btnOpenPersonalFolderPath";
            this.btnOpenPersonalFolderPath.UseVisualStyleBackColor = true;
            this.btnOpenPersonalFolderPath.Click += new System.EventHandler(this.btnOpenPersonalFolder_Click);
            // 
            // txtCustomScreenshotsPath
            // 
            resources.ApplyResources(this.txtCustomScreenshotsPath, "txtCustomScreenshotsPath");
            this.txtCustomScreenshotsPath.Name = "txtCustomScreenshotsPath";
            this.txtCustomScreenshotsPath.TextChanged += new System.EventHandler(this.txtCustomScreenshotsPath_TextChanged);
            // 
            // cbUseCustomScreenshotsPath
            // 
            resources.ApplyResources(this.cbUseCustomScreenshotsPath, "cbUseCustomScreenshotsPath");
            this.cbUseCustomScreenshotsPath.Name = "cbUseCustomScreenshotsPath";
            this.cbUseCustomScreenshotsPath.UseVisualStyleBackColor = true;
            this.cbUseCustomScreenshotsPath.CheckedChanged += new System.EventHandler(this.cbUseCustomScreenshotsPath_CheckedChanged);
            // 
            // lblSaveImageSubFolderPattern
            // 
            resources.ApplyResources(this.lblSaveImageSubFolderPattern, "lblSaveImageSubFolderPattern");
            this.lblSaveImageSubFolderPattern.Name = "lblSaveImageSubFolderPattern";
            // 
            // lblSaveImageSubFolderPatternPreview
            // 
            resources.ApplyResources(this.lblSaveImageSubFolderPatternPreview, "lblSaveImageSubFolderPatternPreview");
            this.lblSaveImageSubFolderPatternPreview.Name = "lblSaveImageSubFolderPatternPreview";
            // 
            // txtSaveImageSubFolderPattern
            // 
            resources.ApplyResources(this.txtSaveImageSubFolderPattern, "txtSaveImageSubFolderPattern");
            this.txtSaveImageSubFolderPattern.Name = "txtSaveImageSubFolderPattern";
            this.txtSaveImageSubFolderPattern.TextChanged += new System.EventHandler(this.txtSaveImageSubFolderPattern_TextChanged);
            // 
            // tpExportImport
            // 
            this.tpExportImport.Controls.Add(this.cbExportLogs);
            this.tpExportImport.Controls.Add(this.cbExportHistory);
            this.tpExportImport.Controls.Add(this.cbExportSettings);
            this.tpExportImport.Controls.Add(this.pbExportImport);
            this.tpExportImport.Controls.Add(this.btnExport);
            this.tpExportImport.Controls.Add(this.btnImport);
            resources.ApplyResources(this.tpExportImport, "tpExportImport");
            this.tpExportImport.Name = "tpExportImport";
            this.tpExportImport.UseVisualStyleBackColor = true;
            // 
            // cbExportLogs
            // 
            resources.ApplyResources(this.cbExportLogs, "cbExportLogs");
            this.cbExportLogs.Name = "cbExportLogs";
            this.cbExportLogs.UseVisualStyleBackColor = true;
            this.cbExportLogs.CheckedChanged += new System.EventHandler(this.cbExportLogs_CheckedChanged);
            // 
            // cbExportHistory
            // 
            resources.ApplyResources(this.cbExportHistory, "cbExportHistory");
            this.cbExportHistory.Name = "cbExportHistory";
            this.cbExportHistory.UseVisualStyleBackColor = true;
            this.cbExportHistory.CheckedChanged += new System.EventHandler(this.cbExportHistory_CheckedChanged);
            // 
            // cbExportSettings
            // 
            resources.ApplyResources(this.cbExportSettings, "cbExportSettings");
            this.cbExportSettings.Name = "cbExportSettings";
            this.cbExportSettings.UseVisualStyleBackColor = true;
            this.cbExportSettings.CheckedChanged += new System.EventHandler(this.cbExportSettings_CheckedChanged);
            // 
            // pbExportImport
            // 
            resources.ApplyResources(this.pbExportImport, "pbExportImport");
            this.pbExportImport.MarqueeAnimationSpeed = 50;
            this.pbExportImport.Name = "pbExportImport";
            this.pbExportImport.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Name = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // tpUpload
            // 
            this.tpUpload.Controls.Add(this.tcUpload);
            resources.ApplyResources(this.tpUpload, "tpUpload");
            this.tpUpload.Name = "tpUpload";
            this.tpUpload.UseVisualStyleBackColor = true;
            // 
            // tcUpload
            // 
            this.tcUpload.Controls.Add(this.tpPerformance);
            this.tcUpload.Controls.Add(this.tpUploadResults);
            this.tcUpload.Controls.Add(this.tpUploadRetry);
            resources.ApplyResources(this.tcUpload, "tcUpload");
            this.tcUpload.Name = "tcUpload";
            this.tcUpload.SelectedIndex = 0;
            // 
            // tpPerformance
            // 
            this.tpPerformance.Controls.Add(this.lblUploadLimit);
            this.tpPerformance.Controls.Add(this.nudUploadLimit);
            this.tpPerformance.Controls.Add(this.lblUploadLimitHint);
            this.tpPerformance.Controls.Add(this.cbBufferSize);
            this.tpPerformance.Controls.Add(this.lblBufferSize);
            resources.ApplyResources(this.tpPerformance, "tpPerformance");
            this.tpPerformance.Name = "tpPerformance";
            this.tpPerformance.UseVisualStyleBackColor = true;
            // 
            // lblUploadLimit
            // 
            resources.ApplyResources(this.lblUploadLimit, "lblUploadLimit");
            this.lblUploadLimit.Name = "lblUploadLimit";
            // 
            // nudUploadLimit
            // 
            resources.ApplyResources(this.nudUploadLimit, "nudUploadLimit");
            this.nudUploadLimit.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudUploadLimit.Name = "nudUploadLimit";
            this.nudUploadLimit.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudUploadLimit.ValueChanged += new System.EventHandler(this.nudUploadLimit_ValueChanged);
            // 
            // lblUploadLimitHint
            // 
            resources.ApplyResources(this.lblUploadLimitHint, "lblUploadLimitHint");
            this.lblUploadLimitHint.Name = "lblUploadLimitHint";
            // 
            // cbBufferSize
            // 
            this.cbBufferSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBufferSize.FormattingEnabled = true;
            resources.ApplyResources(this.cbBufferSize, "cbBufferSize");
            this.cbBufferSize.Name = "cbBufferSize";
            this.cbBufferSize.SelectedIndexChanged += new System.EventHandler(this.cbBufferSize_SelectedIndexChanged);
            // 
            // lblBufferSize
            // 
            resources.ApplyResources(this.lblBufferSize, "lblBufferSize");
            this.lblBufferSize.Name = "lblBufferSize";
            // 
            // tpUploadResults
            // 
            this.tpUploadResults.Controls.Add(this.gbClipboardFormats);
            resources.ApplyResources(this.tpUploadResults, "tpUploadResults");
            this.tpUploadResults.Name = "tpUploadResults";
            this.tpUploadResults.UseVisualStyleBackColor = true;
            // 
            // gbClipboardFormats
            // 
            resources.ApplyResources(this.gbClipboardFormats, "gbClipboardFormats");
            this.gbClipboardFormats.Controls.Add(this.btnClipboardFormatEdit);
            this.gbClipboardFormats.Controls.Add(this.btnClipboardFormatRemove);
            this.gbClipboardFormats.Controls.Add(this.btnClipboardFormatAdd);
            this.gbClipboardFormats.Controls.Add(this.lvClipboardFormats);
            this.gbClipboardFormats.Name = "gbClipboardFormats";
            this.gbClipboardFormats.TabStop = false;
            // 
            // btnClipboardFormatEdit
            // 
            resources.ApplyResources(this.btnClipboardFormatEdit, "btnClipboardFormatEdit");
            this.btnClipboardFormatEdit.Name = "btnClipboardFormatEdit";
            this.btnClipboardFormatEdit.UseVisualStyleBackColor = true;
            this.btnClipboardFormatEdit.Click += new System.EventHandler(this.btnClipboardFormatEdit_Click);
            // 
            // btnClipboardFormatRemove
            // 
            resources.ApplyResources(this.btnClipboardFormatRemove, "btnClipboardFormatRemove");
            this.btnClipboardFormatRemove.Name = "btnClipboardFormatRemove";
            this.btnClipboardFormatRemove.UseVisualStyleBackColor = true;
            this.btnClipboardFormatRemove.Click += new System.EventHandler(this.btnClipboardFormatRemove_Click);
            // 
            // btnClipboardFormatAdd
            // 
            resources.ApplyResources(this.btnClipboardFormatAdd, "btnClipboardFormatAdd");
            this.btnClipboardFormatAdd.Name = "btnClipboardFormatAdd";
            this.btnClipboardFormatAdd.UseVisualStyleBackColor = true;
            this.btnClipboardFormatAdd.Click += new System.EventHandler(this.btnAddClipboardFormat_Click);
            // 
            // lvClipboardFormats
            // 
            resources.ApplyResources(this.lvClipboardFormats, "lvClipboardFormats");
            this.lvClipboardFormats.AutoFillColumn = true;
            this.lvClipboardFormats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDescription,
            this.chFormat});
            this.lvClipboardFormats.FullRowSelect = true;
            this.lvClipboardFormats.Name = "lvClipboardFormats";
            this.lvClipboardFormats.UseCompatibleStateImageBehavior = false;
            this.lvClipboardFormats.View = System.Windows.Forms.View.Details;
            this.lvClipboardFormats.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvClipboardFormats_MouseDoubleClick);
            // 
            // chDescription
            // 
            resources.ApplyResources(this.chDescription, "chDescription");
            // 
            // chFormat
            // 
            resources.ApplyResources(this.chFormat, "chFormat");
            // 
            // tpUploadRetry
            // 
            this.tpUploadRetry.Controls.Add(this.chkUseSecondaryUploaders);
            this.tpUploadRetry.Controls.Add(this.tlpBackupDestinations);
            this.tpUploadRetry.Controls.Add(this.cbIfUploadFailRetryOnce);
            this.tpUploadRetry.Controls.Add(this.nudRetryUpload);
            resources.ApplyResources(this.tpUploadRetry, "tpUploadRetry");
            this.tpUploadRetry.Name = "tpUploadRetry";
            this.tpUploadRetry.UseVisualStyleBackColor = true;
            // 
            // chkUseSecondaryUploaders
            // 
            resources.ApplyResources(this.chkUseSecondaryUploaders, "chkUseSecondaryUploaders");
            this.chkUseSecondaryUploaders.Name = "chkUseSecondaryUploaders";
            this.chkUseSecondaryUploaders.UseVisualStyleBackColor = true;
            this.chkUseSecondaryUploaders.CheckedChanged += new System.EventHandler(this.chkUseSecondaryUploaders_CheckedChanged);
            // 
            // tlpBackupDestinations
            // 
            resources.ApplyResources(this.tlpBackupDestinations, "tlpBackupDestinations");
            this.tlpBackupDestinations.Controls.Add(this.gbSecondaryImageUploaders, 0, 0);
            this.tlpBackupDestinations.Controls.Add(this.gbSecondaryFileUploaders, 2, 0);
            this.tlpBackupDestinations.Controls.Add(this.gbSecondaryTextUploaders, 1, 0);
            this.tlpBackupDestinations.Name = "tlpBackupDestinations";
            // 
            // gbSecondaryImageUploaders
            // 
            this.gbSecondaryImageUploaders.Controls.Add(this.lvSecondaryImageUploaders);
            resources.ApplyResources(this.gbSecondaryImageUploaders, "gbSecondaryImageUploaders");
            this.gbSecondaryImageUploaders.Name = "gbSecondaryImageUploaders";
            this.gbSecondaryImageUploaders.TabStop = false;
            // 
            // lvSecondaryImageUploaders
            // 
            this.lvSecondaryImageUploaders.AllowDrop = true;
            this.lvSecondaryImageUploaders.AllowItemDrag = true;
            this.lvSecondaryImageUploaders.AutoFillColumn = true;
            this.lvSecondaryImageUploaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvSecondaryImageUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSecondaryImageUploaders});
            resources.ApplyResources(this.lvSecondaryImageUploaders, "lvSecondaryImageUploaders");
            this.lvSecondaryImageUploaders.FullRowSelect = true;
            this.lvSecondaryImageUploaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvSecondaryImageUploaders.HideSelection = false;
            this.lvSecondaryImageUploaders.MultiSelect = false;
            this.lvSecondaryImageUploaders.Name = "lvSecondaryImageUploaders";
            this.lvSecondaryImageUploaders.UseCompatibleStateImageBehavior = false;
            this.lvSecondaryImageUploaders.View = System.Windows.Forms.View.Details;
            this.lvSecondaryImageUploaders.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSecondaryUploaders_MouseUp);
            // 
            // gbSecondaryFileUploaders
            // 
            this.gbSecondaryFileUploaders.Controls.Add(this.lvSecondaryFileUploaders);
            resources.ApplyResources(this.gbSecondaryFileUploaders, "gbSecondaryFileUploaders");
            this.gbSecondaryFileUploaders.Name = "gbSecondaryFileUploaders";
            this.gbSecondaryFileUploaders.TabStop = false;
            // 
            // lvSecondaryFileUploaders
            // 
            this.lvSecondaryFileUploaders.AllowDrop = true;
            this.lvSecondaryFileUploaders.AllowItemDrag = true;
            this.lvSecondaryFileUploaders.AutoFillColumn = true;
            this.lvSecondaryFileUploaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvSecondaryFileUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSecondaryFileUploaders});
            resources.ApplyResources(this.lvSecondaryFileUploaders, "lvSecondaryFileUploaders");
            this.lvSecondaryFileUploaders.FullRowSelect = true;
            this.lvSecondaryFileUploaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvSecondaryFileUploaders.MultiSelect = false;
            this.lvSecondaryFileUploaders.Name = "lvSecondaryFileUploaders";
            this.lvSecondaryFileUploaders.UseCompatibleStateImageBehavior = false;
            this.lvSecondaryFileUploaders.View = System.Windows.Forms.View.Details;
            this.lvSecondaryFileUploaders.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSecondaryUploaders_MouseUp);
            // 
            // gbSecondaryTextUploaders
            // 
            this.gbSecondaryTextUploaders.Controls.Add(this.lvSecondaryTextUploaders);
            resources.ApplyResources(this.gbSecondaryTextUploaders, "gbSecondaryTextUploaders");
            this.gbSecondaryTextUploaders.Name = "gbSecondaryTextUploaders";
            this.gbSecondaryTextUploaders.TabStop = false;
            // 
            // lvSecondaryTextUploaders
            // 
            this.lvSecondaryTextUploaders.AllowDrop = true;
            this.lvSecondaryTextUploaders.AllowItemDrag = true;
            this.lvSecondaryTextUploaders.AutoFillColumn = true;
            this.lvSecondaryTextUploaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvSecondaryTextUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSecondaryTextUploaders});
            resources.ApplyResources(this.lvSecondaryTextUploaders, "lvSecondaryTextUploaders");
            this.lvSecondaryTextUploaders.FullRowSelect = true;
            this.lvSecondaryTextUploaders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvSecondaryTextUploaders.MultiSelect = false;
            this.lvSecondaryTextUploaders.Name = "lvSecondaryTextUploaders";
            this.lvSecondaryTextUploaders.UseCompatibleStateImageBehavior = false;
            this.lvSecondaryTextUploaders.View = System.Windows.Forms.View.Details;
            this.lvSecondaryTextUploaders.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSecondaryUploaders_MouseUp);
            // 
            // cbIfUploadFailRetryOnce
            // 
            resources.ApplyResources(this.cbIfUploadFailRetryOnce, "cbIfUploadFailRetryOnce");
            this.cbIfUploadFailRetryOnce.Name = "cbIfUploadFailRetryOnce";
            // 
            // nudRetryUpload
            // 
            resources.ApplyResources(this.nudRetryUpload, "nudRetryUpload");
            this.nudRetryUpload.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudRetryUpload.Name = "nudRetryUpload";
            this.nudRetryUpload.ValueChanged += new System.EventHandler(this.nudRetryUpload_ValueChanged);
            // 
            // tpHistory
            // 
            this.tpHistory.Controls.Add(this.gbHistory);
            this.tpHistory.Controls.Add(this.gbRecentLinks);
            resources.ApplyResources(this.tpHistory, "tpHistory");
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.UseVisualStyleBackColor = true;
            // 
            // gbHistory
            // 
            this.gbHistory.Controls.Add(this.cbHistoryCheckURL);
            this.gbHistory.Controls.Add(this.cbHistorySaveTasks);
            resources.ApplyResources(this.gbHistory, "gbHistory");
            this.gbHistory.Name = "gbHistory";
            this.gbHistory.TabStop = false;
            // 
            // cbHistoryCheckURL
            // 
            resources.ApplyResources(this.cbHistoryCheckURL, "cbHistoryCheckURL");
            this.cbHistoryCheckURL.Name = "cbHistoryCheckURL";
            this.cbHistoryCheckURL.UseVisualStyleBackColor = true;
            this.cbHistoryCheckURL.CheckedChanged += new System.EventHandler(this.cbHistoryCheckURL_CheckedChanged);
            // 
            // cbHistorySaveTasks
            // 
            resources.ApplyResources(this.cbHistorySaveTasks, "cbHistorySaveTasks");
            this.cbHistorySaveTasks.Name = "cbHistorySaveTasks";
            this.cbHistorySaveTasks.UseVisualStyleBackColor = true;
            this.cbHistorySaveTasks.CheckedChanged += new System.EventHandler(this.cbHistorySaveTasks_CheckedChanged);
            // 
            // gbRecentLinks
            // 
            this.gbRecentLinks.Controls.Add(this.cbRecentTasksTrayMenuMostRecentFirst);
            this.gbRecentLinks.Controls.Add(this.lblRecentTasksMaxCount);
            this.gbRecentLinks.Controls.Add(this.nudRecentTasksMaxCount);
            this.gbRecentLinks.Controls.Add(this.cbRecentTasksShowInTrayMenu);
            this.gbRecentLinks.Controls.Add(this.cbRecentTasksShowInMainWindow);
            this.gbRecentLinks.Controls.Add(this.cbRecentTasksSave);
            resources.ApplyResources(this.gbRecentLinks, "gbRecentLinks");
            this.gbRecentLinks.Name = "gbRecentLinks";
            this.gbRecentLinks.TabStop = false;
            // 
            // cbRecentTasksTrayMenuMostRecentFirst
            // 
            resources.ApplyResources(this.cbRecentTasksTrayMenuMostRecentFirst, "cbRecentTasksTrayMenuMostRecentFirst");
            this.cbRecentTasksTrayMenuMostRecentFirst.Name = "cbRecentTasksTrayMenuMostRecentFirst";
            this.cbRecentTasksTrayMenuMostRecentFirst.UseVisualStyleBackColor = true;
            this.cbRecentTasksTrayMenuMostRecentFirst.CheckedChanged += new System.EventHandler(this.cbRecentTasksTrayMenuMostRecentFirst_CheckedChanged);
            // 
            // lblRecentTasksMaxCount
            // 
            resources.ApplyResources(this.lblRecentTasksMaxCount, "lblRecentTasksMaxCount");
            this.lblRecentTasksMaxCount.Name = "lblRecentTasksMaxCount";
            // 
            // nudRecentTasksMaxCount
            // 
            resources.ApplyResources(this.nudRecentTasksMaxCount, "nudRecentTasksMaxCount");
            this.nudRecentTasksMaxCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRecentTasksMaxCount.Name = "nudRecentTasksMaxCount";
            this.nudRecentTasksMaxCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRecentTasksMaxCount.ValueChanged += new System.EventHandler(this.nudRecentTasksMaxCount_ValueChanged);
            // 
            // cbRecentTasksShowInTrayMenu
            // 
            resources.ApplyResources(this.cbRecentTasksShowInTrayMenu, "cbRecentTasksShowInTrayMenu");
            this.cbRecentTasksShowInTrayMenu.Name = "cbRecentTasksShowInTrayMenu";
            this.cbRecentTasksShowInTrayMenu.UseVisualStyleBackColor = true;
            this.cbRecentTasksShowInTrayMenu.CheckedChanged += new System.EventHandler(this.cbRecentTasksShowInTrayMenu_CheckedChanged);
            // 
            // cbRecentTasksShowInMainWindow
            // 
            resources.ApplyResources(this.cbRecentTasksShowInMainWindow, "cbRecentTasksShowInMainWindow");
            this.cbRecentTasksShowInMainWindow.Name = "cbRecentTasksShowInMainWindow";
            this.cbRecentTasksShowInMainWindow.UseVisualStyleBackColor = true;
            this.cbRecentTasksShowInMainWindow.CheckedChanged += new System.EventHandler(this.cbRecentTasksShowInMainWindow_CheckedChanged);
            // 
            // cbRecentTasksSave
            // 
            resources.ApplyResources(this.cbRecentTasksSave, "cbRecentTasksSave");
            this.cbRecentTasksSave.Name = "cbRecentTasksSave";
            this.cbRecentTasksSave.UseVisualStyleBackColor = true;
            this.cbRecentTasksSave.CheckedChanged += new System.EventHandler(this.cbRecentTasksSave_CheckedChanged);
            // 
            // tpPrint
            // 
            this.tpPrint.Controls.Add(this.cbPrintDontShowWindowsDialog);
            this.tpPrint.Controls.Add(this.cbDontShowPrintSettingDialog);
            this.tpPrint.Controls.Add(this.btnShowImagePrintSettings);
            resources.ApplyResources(this.tpPrint, "tpPrint");
            this.tpPrint.Name = "tpPrint";
            this.tpPrint.UseVisualStyleBackColor = true;
            // 
            // cbPrintDontShowWindowsDialog
            // 
            resources.ApplyResources(this.cbPrintDontShowWindowsDialog, "cbPrintDontShowWindowsDialog");
            this.cbPrintDontShowWindowsDialog.Name = "cbPrintDontShowWindowsDialog";
            this.cbPrintDontShowWindowsDialog.UseVisualStyleBackColor = true;
            this.cbPrintDontShowWindowsDialog.CheckedChanged += new System.EventHandler(this.cbPrintDontShowWindowsDialog_CheckedChanged);
            // 
            // cbDontShowPrintSettingDialog
            // 
            resources.ApplyResources(this.cbDontShowPrintSettingDialog, "cbDontShowPrintSettingDialog");
            this.cbDontShowPrintSettingDialog.Name = "cbDontShowPrintSettingDialog";
            this.cbDontShowPrintSettingDialog.UseVisualStyleBackColor = true;
            this.cbDontShowPrintSettingDialog.CheckedChanged += new System.EventHandler(this.cbDontShowPrintSettingDialog_CheckedChanged);
            // 
            // btnShowImagePrintSettings
            // 
            resources.ApplyResources(this.btnShowImagePrintSettings, "btnShowImagePrintSettings");
            this.btnShowImagePrintSettings.Name = "btnShowImagePrintSettings";
            this.btnShowImagePrintSettings.UseVisualStyleBackColor = true;
            this.btnShowImagePrintSettings.Click += new System.EventHandler(this.btnShowImagePrintSettings_Click);
            // 
            // tpProxy
            // 
            this.tpProxy.Controls.Add(this.cbProxyMethod);
            this.tpProxy.Controls.Add(this.lblProxyMethod);
            this.tpProxy.Controls.Add(this.lblProxyHost);
            this.tpProxy.Controls.Add(this.txtProxyHost);
            this.tpProxy.Controls.Add(this.nudProxyPort);
            this.tpProxy.Controls.Add(this.lblProxyPort);
            this.tpProxy.Controls.Add(this.lblProxyPassword);
            this.tpProxy.Controls.Add(this.txtProxyPassword);
            this.tpProxy.Controls.Add(this.lblProxyUsername);
            this.tpProxy.Controls.Add(this.txtProxyUsername);
            resources.ApplyResources(this.tpProxy, "tpProxy");
            this.tpProxy.Name = "tpProxy";
            this.tpProxy.UseVisualStyleBackColor = true;
            // 
            // cbProxyMethod
            // 
            this.cbProxyMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProxyMethod.FormattingEnabled = true;
            resources.ApplyResources(this.cbProxyMethod, "cbProxyMethod");
            this.cbProxyMethod.Name = "cbProxyMethod";
            this.cbProxyMethod.SelectedIndexChanged += new System.EventHandler(this.cbProxyMethod_SelectedIndexChanged);
            // 
            // lblProxyMethod
            // 
            resources.ApplyResources(this.lblProxyMethod, "lblProxyMethod");
            this.lblProxyMethod.Name = "lblProxyMethod";
            // 
            // lblProxyHost
            // 
            resources.ApplyResources(this.lblProxyHost, "lblProxyHost");
            this.lblProxyHost.Name = "lblProxyHost";
            // 
            // txtProxyHost
            // 
            resources.ApplyResources(this.txtProxyHost, "txtProxyHost");
            this.txtProxyHost.Name = "txtProxyHost";
            this.txtProxyHost.TextChanged += new System.EventHandler(this.txtProxyHost_TextChanged);
            // 
            // nudProxyPort
            // 
            resources.ApplyResources(this.nudProxyPort, "nudProxyPort");
            this.nudProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudProxyPort.Name = "nudProxyPort";
            this.nudProxyPort.ValueChanged += new System.EventHandler(this.nudProxyPort_ValueChanged);
            // 
            // lblProxyPort
            // 
            resources.ApplyResources(this.lblProxyPort, "lblProxyPort");
            this.lblProxyPort.Name = "lblProxyPort";
            // 
            // lblProxyPassword
            // 
            resources.ApplyResources(this.lblProxyPassword, "lblProxyPassword");
            this.lblProxyPassword.Name = "lblProxyPassword";
            // 
            // txtProxyPassword
            // 
            resources.ApplyResources(this.txtProxyPassword, "txtProxyPassword");
            this.txtProxyPassword.Name = "txtProxyPassword";
            this.txtProxyPassword.UseSystemPasswordChar = true;
            this.txtProxyPassword.TextChanged += new System.EventHandler(this.txtProxyPassword_TextChanged);
            // 
            // lblProxyUsername
            // 
            resources.ApplyResources(this.lblProxyUsername, "lblProxyUsername");
            this.lblProxyUsername.Name = "lblProxyUsername";
            // 
            // txtProxyUsername
            // 
            resources.ApplyResources(this.txtProxyUsername, "txtProxyUsername");
            this.txtProxyUsername.Name = "txtProxyUsername";
            this.txtProxyUsername.TextChanged += new System.EventHandler(this.txtProxyUsername_TextChanged);
            // 
            // tpAdvanced
            // 
            this.tpAdvanced.Controls.Add(this.pgSettings);
            resources.ApplyResources(this.tpAdvanced, "tpAdvanced");
            this.tpAdvanced.Name = "tpAdvanced";
            this.tpAdvanced.UseVisualStyleBackColor = true;
            // 
            // pgSettings
            // 
            this.pgSettings.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            resources.ApplyResources(this.pgSettings, "pgSettings");
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgSettings.ToolbarVisible = false;
            // 
            // tttvMain
            // 
            resources.ApplyResources(this.tttvMain, "tttvMain");
            this.tttvMain.ImageList = null;
            this.tttvMain.MainTabControl = null;
            this.tttvMain.Name = "tttvMain";
            this.tttvMain.TreeViewFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tttvMain.TreeViewSize = 175;
            // 
            // btnChromeEnableSupport
            // 
            resources.ApplyResources(this.btnChromeEnableSupport, "btnChromeEnableSupport");
            this.btnChromeEnableSupport.Name = "btnChromeEnableSupport";
            this.btnChromeEnableSupport.UseVisualStyleBackColor = true;
            this.btnChromeEnableSupport.Click += new System.EventHandler(this.btnChromeEnableSupport_Click);
            // 
            // btnChromeDisableSupport
            // 
            resources.ApplyResources(this.btnChromeDisableSupport, "btnChromeDisableSupport");
            this.btnChromeDisableSupport.Name = "btnChromeDisableSupport";
            this.btnChromeDisableSupport.UseVisualStyleBackColor = true;
            this.btnChromeDisableSupport.Click += new System.EventHandler(this.btnChromeDisableSupport_Click);
            // 
            // btnChromeOpenExtensionPage
            // 
            resources.ApplyResources(this.btnChromeOpenExtensionPage, "btnChromeOpenExtensionPage");
            this.btnChromeOpenExtensionPage.Name = "btnChromeOpenExtensionPage";
            this.btnChromeOpenExtensionPage.UseVisualStyleBackColor = true;
            this.btnChromeOpenExtensionPage.Click += new System.EventHandler(this.btnChromeOpenExtensionPage_Click);
            // 
            // gpFirefox
            // 
            this.gpFirefox.Controls.Add(this.btnFirefoxOpenAddonPage);
            this.gpFirefox.Controls.Add(this.btnFirefoxDisableSupport);
            this.gpFirefox.Controls.Add(this.btnFirefoxEnableSupport);
            resources.ApplyResources(this.gpFirefox, "gpFirefox");
            this.gpFirefox.Name = "gpFirefox";
            this.gpFirefox.TabStop = false;
            // 
            // btnFirefoxOpenAddonPage
            // 
            resources.ApplyResources(this.btnFirefoxOpenAddonPage, "btnFirefoxOpenAddonPage");
            this.btnFirefoxOpenAddonPage.Name = "btnFirefoxOpenAddonPage";
            this.btnFirefoxOpenAddonPage.UseVisualStyleBackColor = true;
            this.btnFirefoxOpenAddonPage.Click += new System.EventHandler(this.btnFirefoxOpenAddonPage_Click);
            // 
            // btnFirefoxDisableSupport
            // 
            resources.ApplyResources(this.btnFirefoxDisableSupport, "btnFirefoxDisableSupport");
            this.btnFirefoxDisableSupport.Name = "btnFirefoxDisableSupport";
            this.btnFirefoxDisableSupport.UseVisualStyleBackColor = true;
            this.btnFirefoxDisableSupport.Click += new System.EventHandler(this.btnFirefoxDisableSupport_Click);
            // 
            // btnFirefoxEnableSupport
            // 
            resources.ApplyResources(this.btnFirefoxEnableSupport, "btnFirefoxEnableSupport");
            this.btnFirefoxEnableSupport.Name = "btnFirefoxEnableSupport";
            this.btnFirefoxEnableSupport.UseVisualStyleBackColor = true;
            this.btnFirefoxEnableSupport.Click += new System.EventHandler(this.btnFirefoxEnableSupport_Click);
            // 
            // ApplicationSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcSettings);
            this.Controls.Add(this.tttvMain);
            this.Name = "ApplicationSettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.Resize += new System.EventHandler(this.SettingsForm_Resize);
            this.tcSettings.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.tpIntegration.ResumeLayout(false);
            this.gbSteam.ResumeLayout(false);
            this.gbSteam.PerformLayout();
            this.gbChrome.ResumeLayout(false);
            this.gbWindows.ResumeLayout(false);
            this.gbWindows.PerformLayout();
            this.tpPaths.ResumeLayout(false);
            this.tpPaths.PerformLayout();
            this.tpExportImport.ResumeLayout(false);
            this.tpExportImport.PerformLayout();
            this.tpUpload.ResumeLayout(false);
            this.tcUpload.ResumeLayout(false);
            this.tpPerformance.ResumeLayout(false);
            this.tpPerformance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUploadLimit)).EndInit();
            this.tpUploadResults.ResumeLayout(false);
            this.gbClipboardFormats.ResumeLayout(false);
            this.tpUploadRetry.ResumeLayout(false);
            this.tpUploadRetry.PerformLayout();
            this.tlpBackupDestinations.ResumeLayout(false);
            this.gbSecondaryImageUploaders.ResumeLayout(false);
            this.gbSecondaryFileUploaders.ResumeLayout(false);
            this.gbSecondaryTextUploaders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryUpload)).EndInit();
            this.tpHistory.ResumeLayout(false);
            this.gbHistory.ResumeLayout(false);
            this.gbHistory.PerformLayout();
            this.gbRecentLinks.ResumeLayout(false);
            this.gbRecentLinks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentTasksMaxCount)).EndInit();
            this.tpPrint.ResumeLayout(false);
            this.tpPrint.PerformLayout();
            this.tpProxy.ResumeLayout(false);
            this.tpProxy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudProxyPort)).EndInit();
            this.tpAdvanced.ResumeLayout(false);
            this.gpFirefox.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox gbClipboardFormats;
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
        private System.Windows.Forms.Label lblNotePersonalFolderPath;
        private System.Windows.Forms.CheckBox cbSilentRun;
        private System.Windows.Forms.NumericUpDown nudRetryUpload;
        private System.Windows.Forms.TableLayoutPanel tlpBackupDestinations;
        private System.Windows.Forms.GroupBox gbSecondaryImageUploaders;
        private MyListView lvSecondaryImageUploaders;
        private System.Windows.Forms.GroupBox gbSecondaryFileUploaders;
        private MyListView lvSecondaryFileUploaders;
        private System.Windows.Forms.GroupBox gbSecondaryTextUploaders;
        private MyListView lvSecondaryTextUploaders;
        private System.Windows.Forms.TabControl tcUpload;
        private System.Windows.Forms.TabPage tpPerformance;
        private System.Windows.Forms.TabPage tpUploadResults;
        private System.Windows.Forms.TabPage tpUploadRetry;
        private System.Windows.Forms.CheckBox chkUseSecondaryUploaders;
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
        private System.Windows.Forms.TabPage tpExportImport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox cbExportLogs;
        private System.Windows.Forms.CheckBox cbExportHistory;
        private System.Windows.Forms.CheckBox cbExportSettings;
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
        private System.Windows.Forms.CheckBox cbCheckPreReleaseUpdates;
        private System.Windows.Forms.Button btnChromeEnableSupport;
        private System.Windows.Forms.Button btnChromeOpenExtensionPage;
        private System.Windows.Forms.Button btnChromeDisableSupport;
        private System.Windows.Forms.GroupBox gpFirefox;
        private System.Windows.Forms.Button btnFirefoxOpenAddonPage;
        private System.Windows.Forms.Button btnFirefoxDisableSupport;
        private System.Windows.Forms.Button btnFirefoxEnableSupport;
    }
}