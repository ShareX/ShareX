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
            this.cbUpdateChannel = new System.Windows.Forms.ComboBox();
            this.lblUpdateChannel = new System.Windows.Forms.Label();
            this.cbAutoCheckUpdate = new System.Windows.Forms.CheckBox();
            this.cbUseWhiteShareXIcon = new System.Windows.Forms.CheckBox();
            this.btnCheckDevBuild = new System.Windows.Forms.Button();
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
            this.tpTheme = new System.Windows.Forms.TabPage();
            this.btnThemeReset = new System.Windows.Forms.Button();
            this.btnThemeRemove = new System.Windows.Forms.Button();
            this.btnThemeAdd = new System.Windows.Forms.Button();
            this.cbThemes = new System.Windows.Forms.ComboBox();
            this.pgTheme = new System.Windows.Forms.PropertyGrid();
            this.cbUseCustomTheme = new System.Windows.Forms.CheckBox();
            this.eiTheme = new ShareX.HelpersLib.ExportImportControl();
            this.tpIntegration = new System.Windows.Forms.TabPage();
            this.gbFirefox = new System.Windows.Forms.GroupBox();
            this.cbFirefoxAddonSupport = new System.Windows.Forms.CheckBox();
            this.btnFirefoxOpenAddonPage = new System.Windows.Forms.Button();
            this.gbSteam = new System.Windows.Forms.GroupBox();
            this.cbSteamShowInApp = new System.Windows.Forms.CheckBox();
            this.gbChrome = new System.Windows.Forms.GroupBox();
            this.cbChromeExtensionSupport = new System.Windows.Forms.CheckBox();
            this.btnChromeOpenExtensionPage = new System.Windows.Forms.Button();
            this.gbWindows = new System.Windows.Forms.GroupBox();
            this.cbEditWithShareX = new System.Windows.Forms.CheckBox();
            this.cbStartWithWindows = new System.Windows.Forms.CheckBox();
            this.cbSendToMenu = new System.Windows.Forms.CheckBox();
            this.cbShellContextMenu = new System.Windows.Forms.CheckBox();
            this.tpPaths = new System.Windows.Forms.TabPage();
            this.txtSaveImageSubFolderPatternWindow = new System.Windows.Forms.TextBox();
            this.lblSaveImageSubFolderPatternWindow = new System.Windows.Forms.Label();
            this.btnPersonalFolderPathApply = new System.Windows.Forms.Button();
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
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.cbAutomaticallyCleanupLogFiles = new System.Windows.Forms.CheckBox();
            this.nudCleanupKeepFileCount = new System.Windows.Forms.NumericUpDown();
            this.lblCleanupKeepFileCount = new System.Windows.Forms.Label();
            this.cbAutomaticallyCleanupBackupFiles = new System.Windows.Forms.CheckBox();
            this.pbExportImportNote = new System.Windows.Forms.PictureBox();
            this.cbExportHistory = new System.Windows.Forms.CheckBox();
            this.cbExportSettings = new System.Windows.Forms.CheckBox();
            this.lblExportImportNote = new System.Windows.Forms.Label();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.pbExportImport = new System.Windows.Forms.ProgressBar();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.tpMainWindow = new System.Windows.Forms.TabPage();
            this.gbListView = new System.Windows.Forms.GroupBox();
            this.cbListViewImagePreviewLocation = new System.Windows.Forms.ComboBox();
            this.lblListViewImagePreviewLocation = new System.Windows.Forms.Label();
            this.cbListViewImagePreviewVisibility = new System.Windows.Forms.ComboBox();
            this.lblListViewImagePreviewVisibility = new System.Windows.Forms.Label();
            this.cbListViewShowColumns = new System.Windows.Forms.CheckBox();
            this.gbThumbnailView = new System.Windows.Forms.GroupBox();
            this.btnThumbnailViewThumbnailSizeReset = new System.Windows.Forms.Button();
            this.lblThumbnailViewThumbnailSizeX = new System.Windows.Forms.Label();
            this.nudThumbnailViewThumbnailSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.nudThumbnailViewThumbnailSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.cbThumbnailViewThumbnailClickAction = new System.Windows.Forms.ComboBox();
            this.lblThumbnailViewThumbnailClickAction = new System.Windows.Forms.Label();
            this.lblThumbnailViewThumbnailSize = new System.Windows.Forms.Label();
            this.cbThumbnailViewTitleLocation = new System.Windows.Forms.ComboBox();
            this.lblThumbnailViewTitleLocation = new System.Windows.Forms.Label();
            this.cbThumbnailViewShowTitle = new System.Windows.Forms.CheckBox();
            this.cbMainWindowShowMenu = new System.Windows.Forms.CheckBox();
            this.cbMainWindowTaskViewMode = new System.Windows.Forms.ComboBox();
            this.lblMainWindowTaskViewMode = new System.Windows.Forms.Label();
            this.tpClipboardFormats = new System.Windows.Forms.TabPage();
            this.lblClipboardFormatsTip = new System.Windows.Forms.Label();
            this.btnClipboardFormatEdit = new System.Windows.Forms.Button();
            this.btnClipboardFormatRemove = new System.Windows.Forms.Button();
            this.btnClipboardFormatAdd = new System.Windows.Forms.Button();
            this.lvClipboardFormats = new ShareX.HelpersLib.MyListView();
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpUpload = new System.Windows.Forms.TabPage();
            this.gbSecondaryFileUploaders = new System.Windows.Forms.GroupBox();
            this.lvSecondaryFileUploaders = new ShareX.HelpersLib.MyListView();
            this.chSecondaryFileUploaders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblUploadLimit = new System.Windows.Forms.Label();
            this.gbSecondaryImageUploaders = new System.Windows.Forms.GroupBox();
            this.lvSecondaryImageUploaders = new ShareX.HelpersLib.MyListView();
            this.chSecondaryImageUploaders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSecondaryTextUploaders = new System.Windows.Forms.GroupBox();
            this.lvSecondaryTextUploaders = new ShareX.HelpersLib.MyListView();
            this.chSecondaryTextUploaders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nudUploadLimit = new System.Windows.Forms.NumericUpDown();
            this.cbUseSecondaryUploaders = new System.Windows.Forms.CheckBox();
            this.lblUploadLimitHint = new System.Windows.Forms.Label();
            this.cbIfUploadFailRetryOnce = new System.Windows.Forms.Label();
            this.lblBufferSize = new System.Windows.Forms.Label();
            this.nudRetryUpload = new System.Windows.Forms.NumericUpDown();
            this.cbBufferSize = new System.Windows.Forms.ComboBox();
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
            this.lblDefaultPrinterOverride = new System.Windows.Forms.Label();
            this.txtDefaultPrinterOverride = new System.Windows.Forms.TextBox();
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
            this.tcSettings.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.tpTheme.SuspendLayout();
            this.tpIntegration.SuspendLayout();
            this.gbFirefox.SuspendLayout();
            this.gbSteam.SuspendLayout();
            this.gbChrome.SuspendLayout();
            this.gbWindows.SuspendLayout();
            this.tpPaths.SuspendLayout();
            this.tpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCleanupKeepFileCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExportImportNote)).BeginInit();
            this.tpMainWindow.SuspendLayout();
            this.gbListView.SuspendLayout();
            this.gbThumbnailView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailViewThumbnailSizeHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailViewThumbnailSizeWidth)).BeginInit();
            this.tpClipboardFormats.SuspendLayout();
            this.tpUpload.SuspendLayout();
            this.gbSecondaryFileUploaders.SuspendLayout();
            this.gbSecondaryImageUploaders.SuspendLayout();
            this.gbSecondaryTextUploaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUploadLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryUpload)).BeginInit();
            this.tpHistory.SuspendLayout();
            this.gbHistory.SuspendLayout();
            this.gbRecentLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentTasksMaxCount)).BeginInit();
            this.tpPrint.SuspendLayout();
            this.tpProxy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudProxyPort)).BeginInit();
            this.tpAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcSettings
            // 
            resources.ApplyResources(this.tcSettings, "tcSettings");
            this.tcSettings.Controls.Add(this.tpGeneral);
            this.tcSettings.Controls.Add(this.tpTheme);
            this.tcSettings.Controls.Add(this.tpIntegration);
            this.tcSettings.Controls.Add(this.tpPaths);
            this.tcSettings.Controls.Add(this.tpSettings);
            this.tcSettings.Controls.Add(this.tpMainWindow);
            this.tcSettings.Controls.Add(this.tpClipboardFormats);
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
            this.tpGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.tpGeneral.Controls.Add(this.cbUpdateChannel);
            this.tpGeneral.Controls.Add(this.lblUpdateChannel);
            this.tpGeneral.Controls.Add(this.cbAutoCheckUpdate);
            this.tpGeneral.Controls.Add(this.cbUseWhiteShareXIcon);
            this.tpGeneral.Controls.Add(this.btnCheckDevBuild);
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
            // 
            // cbUpdateChannel
            // 
            this.cbUpdateChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUpdateChannel.FormattingEnabled = true;
            resources.ApplyResources(this.cbUpdateChannel, "cbUpdateChannel");
            this.cbUpdateChannel.Name = "cbUpdateChannel";
            this.cbUpdateChannel.SelectedIndexChanged += new System.EventHandler(this.cbUpdateChannel_SelectedIndexChanged);
            // 
            // lblUpdateChannel
            // 
            resources.ApplyResources(this.lblUpdateChannel, "lblUpdateChannel");
            this.lblUpdateChannel.Name = "lblUpdateChannel";
            // 
            // cbAutoCheckUpdate
            // 
            resources.ApplyResources(this.cbAutoCheckUpdate, "cbAutoCheckUpdate");
            this.cbAutoCheckUpdate.Name = "cbAutoCheckUpdate";
            this.cbAutoCheckUpdate.UseVisualStyleBackColor = true;
            this.cbAutoCheckUpdate.CheckedChanged += new System.EventHandler(this.cbAutoCheckUpdate_CheckedChanged);
            // 
            // cbUseWhiteShareXIcon
            // 
            resources.ApplyResources(this.cbUseWhiteShareXIcon, "cbUseWhiteShareXIcon");
            this.cbUseWhiteShareXIcon.Name = "cbUseWhiteShareXIcon";
            this.cbUseWhiteShareXIcon.UseVisualStyleBackColor = true;
            this.cbUseWhiteShareXIcon.CheckedChanged += new System.EventHandler(this.CbUseWhiteShareXIcon_CheckedChanged);
            // 
            // btnCheckDevBuild
            // 
            resources.ApplyResources(this.btnCheckDevBuild, "btnCheckDevBuild");
            this.btnCheckDevBuild.Name = "btnCheckDevBuild";
            this.btnCheckDevBuild.UseVisualStyleBackColor = true;
            this.btnCheckDevBuild.Click += new System.EventHandler(this.btnCheckDevBuild_Click);
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
            // tpTheme
            // 
            this.tpTheme.Controls.Add(this.btnThemeReset);
            this.tpTheme.Controls.Add(this.btnThemeRemove);
            this.tpTheme.Controls.Add(this.btnThemeAdd);
            this.tpTheme.Controls.Add(this.cbThemes);
            this.tpTheme.Controls.Add(this.pgTheme);
            this.tpTheme.Controls.Add(this.cbUseCustomTheme);
            this.tpTheme.Controls.Add(this.eiTheme);
            resources.ApplyResources(this.tpTheme, "tpTheme");
            this.tpTheme.Name = "tpTheme";
            this.tpTheme.UseVisualStyleBackColor = true;
            // 
            // btnThemeReset
            // 
            resources.ApplyResources(this.btnThemeReset, "btnThemeReset");
            this.btnThemeReset.Name = "btnThemeReset";
            this.btnThemeReset.UseVisualStyleBackColor = true;
            this.btnThemeReset.Click += new System.EventHandler(this.BtnThemeReset_Click);
            // 
            // btnThemeRemove
            // 
            resources.ApplyResources(this.btnThemeRemove, "btnThemeRemove");
            this.btnThemeRemove.Name = "btnThemeRemove";
            this.btnThemeRemove.UseVisualStyleBackColor = true;
            this.btnThemeRemove.Click += new System.EventHandler(this.BtnThemeRemove_Click);
            // 
            // btnThemeAdd
            // 
            resources.ApplyResources(this.btnThemeAdd, "btnThemeAdd");
            this.btnThemeAdd.Name = "btnThemeAdd";
            this.btnThemeAdd.UseVisualStyleBackColor = true;
            this.btnThemeAdd.Click += new System.EventHandler(this.BtnThemeAdd_Click);
            // 
            // cbThemes
            // 
            this.cbThemes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThemes.FormattingEnabled = true;
            resources.ApplyResources(this.cbThemes, "cbThemes");
            this.cbThemes.Name = "cbThemes";
            this.cbThemes.SelectedIndexChanged += new System.EventHandler(this.CbThemes_SelectedIndexChanged);
            // 
            // pgTheme
            // 
            resources.ApplyResources(this.pgTheme, "pgTheme");
            this.pgTheme.Name = "pgTheme";
            this.pgTheme.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgTheme.ToolbarVisible = false;
            this.pgTheme.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgTheme_PropertyValueChanged);
            // 
            // cbUseCustomTheme
            // 
            resources.ApplyResources(this.cbUseCustomTheme, "cbUseCustomTheme");
            this.cbUseCustomTheme.Name = "cbUseCustomTheme";
            this.cbUseCustomTheme.UseVisualStyleBackColor = true;
            this.cbUseCustomTheme.CheckedChanged += new System.EventHandler(this.CbUseCustomTheme_CheckedChanged);
            // 
            // eiTheme
            // 
            this.eiTheme.DefaultFileName = null;
            resources.ApplyResources(this.eiTheme, "eiTheme");
            this.eiTheme.Name = "eiTheme";
            this.eiTheme.ObjectType = null;
            this.eiTheme.SerializationBinder = null;
            this.eiTheme.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.EiTheme_ExportRequested);
            this.eiTheme.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.EiTheme_ImportRequested);
            // 
            // tpIntegration
            // 
            this.tpIntegration.BackColor = System.Drawing.SystemColors.Window;
            this.tpIntegration.Controls.Add(this.gbFirefox);
            this.tpIntegration.Controls.Add(this.gbSteam);
            this.tpIntegration.Controls.Add(this.gbChrome);
            this.tpIntegration.Controls.Add(this.gbWindows);
            resources.ApplyResources(this.tpIntegration, "tpIntegration");
            this.tpIntegration.Name = "tpIntegration";
            // 
            // gbFirefox
            // 
            this.gbFirefox.Controls.Add(this.cbFirefoxAddonSupport);
            this.gbFirefox.Controls.Add(this.btnFirefoxOpenAddonPage);
            resources.ApplyResources(this.gbFirefox, "gbFirefox");
            this.gbFirefox.Name = "gbFirefox";
            this.gbFirefox.TabStop = false;
            // 
            // cbFirefoxAddonSupport
            // 
            resources.ApplyResources(this.cbFirefoxAddonSupport, "cbFirefoxAddonSupport");
            this.cbFirefoxAddonSupport.Name = "cbFirefoxAddonSupport";
            this.cbFirefoxAddonSupport.UseVisualStyleBackColor = true;
            this.cbFirefoxAddonSupport.CheckedChanged += new System.EventHandler(this.cbFirefoxAddonSupport_CheckedChanged);
            // 
            // btnFirefoxOpenAddonPage
            // 
            resources.ApplyResources(this.btnFirefoxOpenAddonPage, "btnFirefoxOpenAddonPage");
            this.btnFirefoxOpenAddonPage.Name = "btnFirefoxOpenAddonPage";
            this.btnFirefoxOpenAddonPage.UseVisualStyleBackColor = true;
            this.btnFirefoxOpenAddonPage.Click += new System.EventHandler(this.btnFirefoxOpenAddonPage_Click);
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
            this.gbChrome.Controls.Add(this.cbChromeExtensionSupport);
            this.gbChrome.Controls.Add(this.btnChromeOpenExtensionPage);
            resources.ApplyResources(this.gbChrome, "gbChrome");
            this.gbChrome.Name = "gbChrome";
            this.gbChrome.TabStop = false;
            // 
            // cbChromeExtensionSupport
            // 
            resources.ApplyResources(this.cbChromeExtensionSupport, "cbChromeExtensionSupport");
            this.cbChromeExtensionSupport.Name = "cbChromeExtensionSupport";
            this.cbChromeExtensionSupport.UseVisualStyleBackColor = true;
            this.cbChromeExtensionSupport.CheckedChanged += new System.EventHandler(this.cbChromeExtensionSupport_CheckedChanged);
            // 
            // btnChromeOpenExtensionPage
            // 
            resources.ApplyResources(this.btnChromeOpenExtensionPage, "btnChromeOpenExtensionPage");
            this.btnChromeOpenExtensionPage.Name = "btnChromeOpenExtensionPage";
            this.btnChromeOpenExtensionPage.UseVisualStyleBackColor = true;
            this.btnChromeOpenExtensionPage.Click += new System.EventHandler(this.btnChromeOpenExtensionPage_Click);
            // 
            // gbWindows
            // 
            this.gbWindows.Controls.Add(this.cbEditWithShareX);
            this.gbWindows.Controls.Add(this.cbStartWithWindows);
            this.gbWindows.Controls.Add(this.cbSendToMenu);
            this.gbWindows.Controls.Add(this.cbShellContextMenu);
            resources.ApplyResources(this.gbWindows, "gbWindows");
            this.gbWindows.Name = "gbWindows";
            this.gbWindows.TabStop = false;
            // 
            // cbEditWithShareX
            // 
            resources.ApplyResources(this.cbEditWithShareX, "cbEditWithShareX");
            this.cbEditWithShareX.Name = "cbEditWithShareX";
            this.cbEditWithShareX.UseVisualStyleBackColor = true;
            this.cbEditWithShareX.CheckedChanged += new System.EventHandler(this.cbEditWithShareX_CheckedChanged);
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
            this.tpPaths.BackColor = System.Drawing.SystemColors.Window;
            this.tpPaths.Controls.Add(this.txtSaveImageSubFolderPatternWindow);
            this.tpPaths.Controls.Add(this.lblSaveImageSubFolderPatternWindow);
            this.tpPaths.Controls.Add(this.btnPersonalFolderPathApply);
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
            // 
            // txtSaveImageSubFolderPatternWindow
            // 
            resources.ApplyResources(this.txtSaveImageSubFolderPatternWindow, "txtSaveImageSubFolderPatternWindow");
            this.txtSaveImageSubFolderPatternWindow.Name = "txtSaveImageSubFolderPatternWindow";
            this.txtSaveImageSubFolderPatternWindow.TextChanged += new System.EventHandler(this.txtSaveImageSubFolderPatternWindow_TextChanged);
            // 
            // lblSaveImageSubFolderPatternWindow
            // 
            resources.ApplyResources(this.lblSaveImageSubFolderPatternWindow, "lblSaveImageSubFolderPatternWindow");
            this.lblSaveImageSubFolderPatternWindow.Name = "lblSaveImageSubFolderPatternWindow";
            // 
            // btnPersonalFolderPathApply
            // 
            resources.ApplyResources(this.btnPersonalFolderPathApply, "btnPersonalFolderPathApply");
            this.btnPersonalFolderPathApply.Name = "btnPersonalFolderPathApply";
            this.btnPersonalFolderPathApply.UseVisualStyleBackColor = true;
            this.btnPersonalFolderPathApply.Click += new System.EventHandler(this.btnPersonalFolderPathApply_Click);
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
            // tpSettings
            // 
            this.tpSettings.BackColor = System.Drawing.SystemColors.Window;
            this.tpSettings.Controls.Add(this.cbAutomaticallyCleanupLogFiles);
            this.tpSettings.Controls.Add(this.nudCleanupKeepFileCount);
            this.tpSettings.Controls.Add(this.lblCleanupKeepFileCount);
            this.tpSettings.Controls.Add(this.cbAutomaticallyCleanupBackupFiles);
            this.tpSettings.Controls.Add(this.pbExportImportNote);
            this.tpSettings.Controls.Add(this.cbExportHistory);
            this.tpSettings.Controls.Add(this.cbExportSettings);
            this.tpSettings.Controls.Add(this.lblExportImportNote);
            this.tpSettings.Controls.Add(this.btnResetSettings);
            this.tpSettings.Controls.Add(this.pbExportImport);
            this.tpSettings.Controls.Add(this.btnExport);
            this.tpSettings.Controls.Add(this.btnImport);
            resources.ApplyResources(this.tpSettings, "tpSettings");
            this.tpSettings.Name = "tpSettings";
            // 
            // cbAutomaticallyCleanupLogFiles
            // 
            resources.ApplyResources(this.cbAutomaticallyCleanupLogFiles, "cbAutomaticallyCleanupLogFiles");
            this.cbAutomaticallyCleanupLogFiles.Name = "cbAutomaticallyCleanupLogFiles";
            this.cbAutomaticallyCleanupLogFiles.UseVisualStyleBackColor = true;
            this.cbAutomaticallyCleanupLogFiles.CheckedChanged += new System.EventHandler(this.cbAutomaticallyCleanupLogFiles_CheckedChanged);
            // 
            // nudCleanupKeepFileCount
            // 
            resources.ApplyResources(this.nudCleanupKeepFileCount, "nudCleanupKeepFileCount");
            this.nudCleanupKeepFileCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCleanupKeepFileCount.Name = "nudCleanupKeepFileCount";
            this.nudCleanupKeepFileCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCleanupKeepFileCount.ValueChanged += new System.EventHandler(this.nudCleanupKeepFileCount_ValueChanged);
            // 
            // lblCleanupKeepFileCount
            // 
            resources.ApplyResources(this.lblCleanupKeepFileCount, "lblCleanupKeepFileCount");
            this.lblCleanupKeepFileCount.Name = "lblCleanupKeepFileCount";
            // 
            // cbAutomaticallyCleanupBackupFiles
            // 
            resources.ApplyResources(this.cbAutomaticallyCleanupBackupFiles, "cbAutomaticallyCleanupBackupFiles");
            this.cbAutomaticallyCleanupBackupFiles.Name = "cbAutomaticallyCleanupBackupFiles";
            this.cbAutomaticallyCleanupBackupFiles.UseVisualStyleBackColor = true;
            this.cbAutomaticallyCleanupBackupFiles.CheckedChanged += new System.EventHandler(this.cbAutomaticallyCleanupBackupFiles_CheckedChanged);
            // 
            // pbExportImportNote
            // 
            this.pbExportImportNote.Image = global::ShareX.Properties.Resources.exclamation;
            resources.ApplyResources(this.pbExportImportNote, "pbExportImportNote");
            this.pbExportImportNote.Name = "pbExportImportNote";
            this.pbExportImportNote.TabStop = false;
            // 
            // cbExportHistory
            // 
            resources.ApplyResources(this.cbExportHistory, "cbExportHistory");
            this.cbExportHistory.Checked = true;
            this.cbExportHistory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbExportHistory.Name = "cbExportHistory";
            this.cbExportHistory.UseVisualStyleBackColor = true;
            this.cbExportHistory.CheckedChanged += new System.EventHandler(this.cbExportHistory_CheckedChanged);
            // 
            // cbExportSettings
            // 
            resources.ApplyResources(this.cbExportSettings, "cbExportSettings");
            this.cbExportSettings.Checked = true;
            this.cbExportSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbExportSettings.Name = "cbExportSettings";
            this.cbExportSettings.UseVisualStyleBackColor = true;
            this.cbExportSettings.CheckedChanged += new System.EventHandler(this.cbExportSettings_CheckedChanged);
            // 
            // lblExportImportNote
            // 
            resources.ApplyResources(this.lblExportImportNote, "lblExportImportNote");
            this.lblExportImportNote.Name = "lblExportImportNote";
            // 
            // btnResetSettings
            // 
            resources.ApplyResources(this.btnResetSettings, "btnResetSettings");
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.btnResetSettings_Click);
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
            // tpMainWindow
            // 
            this.tpMainWindow.Controls.Add(this.gbListView);
            this.tpMainWindow.Controls.Add(this.gbThumbnailView);
            this.tpMainWindow.Controls.Add(this.cbMainWindowShowMenu);
            this.tpMainWindow.Controls.Add(this.cbMainWindowTaskViewMode);
            this.tpMainWindow.Controls.Add(this.lblMainWindowTaskViewMode);
            resources.ApplyResources(this.tpMainWindow, "tpMainWindow");
            this.tpMainWindow.Name = "tpMainWindow";
            this.tpMainWindow.UseVisualStyleBackColor = true;
            // 
            // gbListView
            // 
            this.gbListView.Controls.Add(this.cbListViewImagePreviewLocation);
            this.gbListView.Controls.Add(this.lblListViewImagePreviewLocation);
            this.gbListView.Controls.Add(this.cbListViewImagePreviewVisibility);
            this.gbListView.Controls.Add(this.lblListViewImagePreviewVisibility);
            this.gbListView.Controls.Add(this.cbListViewShowColumns);
            resources.ApplyResources(this.gbListView, "gbListView");
            this.gbListView.Name = "gbListView";
            this.gbListView.TabStop = false;
            // 
            // cbListViewImagePreviewLocation
            // 
            this.cbListViewImagePreviewLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbListViewImagePreviewLocation.FormattingEnabled = true;
            resources.ApplyResources(this.cbListViewImagePreviewLocation, "cbListViewImagePreviewLocation");
            this.cbListViewImagePreviewLocation.Name = "cbListViewImagePreviewLocation";
            this.cbListViewImagePreviewLocation.SelectedIndexChanged += new System.EventHandler(this.cbListViewImagePreviewLocation_SelectedIndexChanged);
            // 
            // lblListViewImagePreviewLocation
            // 
            resources.ApplyResources(this.lblListViewImagePreviewLocation, "lblListViewImagePreviewLocation");
            this.lblListViewImagePreviewLocation.Name = "lblListViewImagePreviewLocation";
            // 
            // cbListViewImagePreviewVisibility
            // 
            this.cbListViewImagePreviewVisibility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbListViewImagePreviewVisibility.FormattingEnabled = true;
            resources.ApplyResources(this.cbListViewImagePreviewVisibility, "cbListViewImagePreviewVisibility");
            this.cbListViewImagePreviewVisibility.Name = "cbListViewImagePreviewVisibility";
            this.cbListViewImagePreviewVisibility.SelectedIndexChanged += new System.EventHandler(this.cbListViewImagePreviewVisibility_SelectedIndexChanged);
            // 
            // lblListViewImagePreviewVisibility
            // 
            resources.ApplyResources(this.lblListViewImagePreviewVisibility, "lblListViewImagePreviewVisibility");
            this.lblListViewImagePreviewVisibility.Name = "lblListViewImagePreviewVisibility";
            // 
            // cbListViewShowColumns
            // 
            resources.ApplyResources(this.cbListViewShowColumns, "cbListViewShowColumns");
            this.cbListViewShowColumns.Name = "cbListViewShowColumns";
            this.cbListViewShowColumns.UseVisualStyleBackColor = true;
            this.cbListViewShowColumns.CheckedChanged += new System.EventHandler(this.cbListViewShowColumns_CheckedChanged);
            // 
            // gbThumbnailView
            // 
            this.gbThumbnailView.Controls.Add(this.btnThumbnailViewThumbnailSizeReset);
            this.gbThumbnailView.Controls.Add(this.lblThumbnailViewThumbnailSizeX);
            this.gbThumbnailView.Controls.Add(this.nudThumbnailViewThumbnailSizeHeight);
            this.gbThumbnailView.Controls.Add(this.nudThumbnailViewThumbnailSizeWidth);
            this.gbThumbnailView.Controls.Add(this.cbThumbnailViewThumbnailClickAction);
            this.gbThumbnailView.Controls.Add(this.lblThumbnailViewThumbnailClickAction);
            this.gbThumbnailView.Controls.Add(this.lblThumbnailViewThumbnailSize);
            this.gbThumbnailView.Controls.Add(this.cbThumbnailViewTitleLocation);
            this.gbThumbnailView.Controls.Add(this.lblThumbnailViewTitleLocation);
            this.gbThumbnailView.Controls.Add(this.cbThumbnailViewShowTitle);
            resources.ApplyResources(this.gbThumbnailView, "gbThumbnailView");
            this.gbThumbnailView.Name = "gbThumbnailView";
            this.gbThumbnailView.TabStop = false;
            // 
            // btnThumbnailViewThumbnailSizeReset
            // 
            resources.ApplyResources(this.btnThumbnailViewThumbnailSizeReset, "btnThumbnailViewThumbnailSizeReset");
            this.btnThumbnailViewThumbnailSizeReset.Name = "btnThumbnailViewThumbnailSizeReset";
            this.btnThumbnailViewThumbnailSizeReset.UseVisualStyleBackColor = true;
            this.btnThumbnailViewThumbnailSizeReset.Click += new System.EventHandler(this.btnThumbnailViewThumbnailSizeReset_Click);
            // 
            // lblThumbnailViewThumbnailSizeX
            // 
            resources.ApplyResources(this.lblThumbnailViewThumbnailSizeX, "lblThumbnailViewThumbnailSizeX");
            this.lblThumbnailViewThumbnailSizeX.Name = "lblThumbnailViewThumbnailSizeX";
            // 
            // nudThumbnailViewThumbnailSizeHeight
            // 
            resources.ApplyResources(this.nudThumbnailViewThumbnailSizeHeight, "nudThumbnailViewThumbnailSizeHeight");
            this.nudThumbnailViewThumbnailSizeHeight.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudThumbnailViewThumbnailSizeHeight.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudThumbnailViewThumbnailSizeHeight.Name = "nudThumbnailViewThumbnailSizeHeight";
            this.nudThumbnailViewThumbnailSizeHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudThumbnailViewThumbnailSizeHeight.ValueChanged += new System.EventHandler(this.nudThumbnailViewThumbnailSizeHeight_ValueChanged);
            // 
            // nudThumbnailViewThumbnailSizeWidth
            // 
            resources.ApplyResources(this.nudThumbnailViewThumbnailSizeWidth, "nudThumbnailViewThumbnailSizeWidth");
            this.nudThumbnailViewThumbnailSizeWidth.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudThumbnailViewThumbnailSizeWidth.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudThumbnailViewThumbnailSizeWidth.Name = "nudThumbnailViewThumbnailSizeWidth";
            this.nudThumbnailViewThumbnailSizeWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudThumbnailViewThumbnailSizeWidth.ValueChanged += new System.EventHandler(this.nudThumbnailViewThumbnailSizeWidth_ValueChanged);
            // 
            // cbThumbnailViewThumbnailClickAction
            // 
            this.cbThumbnailViewThumbnailClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThumbnailViewThumbnailClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbThumbnailViewThumbnailClickAction, "cbThumbnailViewThumbnailClickAction");
            this.cbThumbnailViewThumbnailClickAction.Name = "cbThumbnailViewThumbnailClickAction";
            this.cbThumbnailViewThumbnailClickAction.SelectedIndexChanged += new System.EventHandler(this.cbThumbnailViewThumbnailClickAction_SelectedIndexChanged);
            // 
            // lblThumbnailViewThumbnailClickAction
            // 
            resources.ApplyResources(this.lblThumbnailViewThumbnailClickAction, "lblThumbnailViewThumbnailClickAction");
            this.lblThumbnailViewThumbnailClickAction.Name = "lblThumbnailViewThumbnailClickAction";
            // 
            // lblThumbnailViewThumbnailSize
            // 
            resources.ApplyResources(this.lblThumbnailViewThumbnailSize, "lblThumbnailViewThumbnailSize");
            this.lblThumbnailViewThumbnailSize.Name = "lblThumbnailViewThumbnailSize";
            // 
            // cbThumbnailViewTitleLocation
            // 
            this.cbThumbnailViewTitleLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThumbnailViewTitleLocation.FormattingEnabled = true;
            resources.ApplyResources(this.cbThumbnailViewTitleLocation, "cbThumbnailViewTitleLocation");
            this.cbThumbnailViewTitleLocation.Name = "cbThumbnailViewTitleLocation";
            this.cbThumbnailViewTitleLocation.SelectedIndexChanged += new System.EventHandler(this.cbThumbnailViewTitleLocation_SelectedIndexChanged);
            // 
            // lblThumbnailViewTitleLocation
            // 
            resources.ApplyResources(this.lblThumbnailViewTitleLocation, "lblThumbnailViewTitleLocation");
            this.lblThumbnailViewTitleLocation.Name = "lblThumbnailViewTitleLocation";
            // 
            // cbThumbnailViewShowTitle
            // 
            resources.ApplyResources(this.cbThumbnailViewShowTitle, "cbThumbnailViewShowTitle");
            this.cbThumbnailViewShowTitle.Name = "cbThumbnailViewShowTitle";
            this.cbThumbnailViewShowTitle.UseVisualStyleBackColor = true;
            this.cbThumbnailViewShowTitle.CheckedChanged += new System.EventHandler(this.cbThumbnailViewShowTitle_CheckedChanged);
            // 
            // cbMainWindowShowMenu
            // 
            resources.ApplyResources(this.cbMainWindowShowMenu, "cbMainWindowShowMenu");
            this.cbMainWindowShowMenu.Name = "cbMainWindowShowMenu";
            this.cbMainWindowShowMenu.UseVisualStyleBackColor = true;
            this.cbMainWindowShowMenu.CheckedChanged += new System.EventHandler(this.cbMainWindowShowMenu_CheckedChanged);
            // 
            // cbMainWindowTaskViewMode
            // 
            this.cbMainWindowTaskViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMainWindowTaskViewMode.FormattingEnabled = true;
            resources.ApplyResources(this.cbMainWindowTaskViewMode, "cbMainWindowTaskViewMode");
            this.cbMainWindowTaskViewMode.Name = "cbMainWindowTaskViewMode";
            this.cbMainWindowTaskViewMode.SelectedIndexChanged += new System.EventHandler(this.cbMainWindowTaskViewMode_SelectedIndexChanged);
            // 
            // lblMainWindowTaskViewMode
            // 
            resources.ApplyResources(this.lblMainWindowTaskViewMode, "lblMainWindowTaskViewMode");
            this.lblMainWindowTaskViewMode.Name = "lblMainWindowTaskViewMode";
            // 
            // tpClipboardFormats
            // 
            this.tpClipboardFormats.Controls.Add(this.lblClipboardFormatsTip);
            this.tpClipboardFormats.Controls.Add(this.btnClipboardFormatEdit);
            this.tpClipboardFormats.Controls.Add(this.btnClipboardFormatRemove);
            this.tpClipboardFormats.Controls.Add(this.btnClipboardFormatAdd);
            this.tpClipboardFormats.Controls.Add(this.lvClipboardFormats);
            resources.ApplyResources(this.tpClipboardFormats, "tpClipboardFormats");
            this.tpClipboardFormats.Name = "tpClipboardFormats";
            this.tpClipboardFormats.UseVisualStyleBackColor = true;
            // 
            // lblClipboardFormatsTip
            // 
            resources.ApplyResources(this.lblClipboardFormatsTip, "lblClipboardFormatsTip");
            this.lblClipboardFormatsTip.Name = "lblClipboardFormatsTip";
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
            this.lvClipboardFormats.HideSelection = false;
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
            // tpUpload
            // 
            this.tpUpload.BackColor = System.Drawing.SystemColors.Window;
            this.tpUpload.Controls.Add(this.gbSecondaryFileUploaders);
            this.tpUpload.Controls.Add(this.lblUploadLimit);
            this.tpUpload.Controls.Add(this.gbSecondaryImageUploaders);
            this.tpUpload.Controls.Add(this.gbSecondaryTextUploaders);
            this.tpUpload.Controls.Add(this.nudUploadLimit);
            this.tpUpload.Controls.Add(this.cbUseSecondaryUploaders);
            this.tpUpload.Controls.Add(this.lblUploadLimitHint);
            this.tpUpload.Controls.Add(this.cbIfUploadFailRetryOnce);
            this.tpUpload.Controls.Add(this.lblBufferSize);
            this.tpUpload.Controls.Add(this.nudRetryUpload);
            this.tpUpload.Controls.Add(this.cbBufferSize);
            resources.ApplyResources(this.tpUpload, "tpUpload");
            this.tpUpload.Name = "tpUpload";
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
            this.lvSecondaryFileUploaders.HideSelection = false;
            this.lvSecondaryFileUploaders.MultiSelect = false;
            this.lvSecondaryFileUploaders.Name = "lvSecondaryFileUploaders";
            this.lvSecondaryFileUploaders.UseCompatibleStateImageBehavior = false;
            this.lvSecondaryFileUploaders.View = System.Windows.Forms.View.Details;
            this.lvSecondaryFileUploaders.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSecondaryUploaders_MouseUp);
            // 
            // lblUploadLimit
            // 
            resources.ApplyResources(this.lblUploadLimit, "lblUploadLimit");
            this.lblUploadLimit.Name = "lblUploadLimit";
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
            this.lvSecondaryTextUploaders.HideSelection = false;
            this.lvSecondaryTextUploaders.MultiSelect = false;
            this.lvSecondaryTextUploaders.Name = "lvSecondaryTextUploaders";
            this.lvSecondaryTextUploaders.UseCompatibleStateImageBehavior = false;
            this.lvSecondaryTextUploaders.View = System.Windows.Forms.View.Details;
            this.lvSecondaryTextUploaders.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSecondaryUploaders_MouseUp);
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
            // cbUseSecondaryUploaders
            // 
            resources.ApplyResources(this.cbUseSecondaryUploaders, "cbUseSecondaryUploaders");
            this.cbUseSecondaryUploaders.Name = "cbUseSecondaryUploaders";
            this.cbUseSecondaryUploaders.UseVisualStyleBackColor = true;
            this.cbUseSecondaryUploaders.CheckedChanged += new System.EventHandler(this.cbUseSecondaryUploaders_CheckedChanged);
            // 
            // lblUploadLimitHint
            // 
            resources.ApplyResources(this.lblUploadLimitHint, "lblUploadLimitHint");
            this.lblUploadLimitHint.Name = "lblUploadLimitHint";
            // 
            // cbIfUploadFailRetryOnce
            // 
            resources.ApplyResources(this.cbIfUploadFailRetryOnce, "cbIfUploadFailRetryOnce");
            this.cbIfUploadFailRetryOnce.Name = "cbIfUploadFailRetryOnce";
            // 
            // lblBufferSize
            // 
            resources.ApplyResources(this.lblBufferSize, "lblBufferSize");
            this.lblBufferSize.Name = "lblBufferSize";
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
            // cbBufferSize
            // 
            this.cbBufferSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBufferSize.FormattingEnabled = true;
            resources.ApplyResources(this.cbBufferSize, "cbBufferSize");
            this.cbBufferSize.Name = "cbBufferSize";
            this.cbBufferSize.SelectedIndexChanged += new System.EventHandler(this.cbBufferSize_SelectedIndexChanged);
            // 
            // tpHistory
            // 
            this.tpHistory.BackColor = System.Drawing.SystemColors.Window;
            this.tpHistory.Controls.Add(this.gbHistory);
            this.tpHistory.Controls.Add(this.gbRecentLinks);
            resources.ApplyResources(this.tpHistory, "tpHistory");
            this.tpHistory.Name = "tpHistory";
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
            this.tpPrint.BackColor = System.Drawing.SystemColors.Window;
            this.tpPrint.Controls.Add(this.lblDefaultPrinterOverride);
            this.tpPrint.Controls.Add(this.txtDefaultPrinterOverride);
            this.tpPrint.Controls.Add(this.cbPrintDontShowWindowsDialog);
            this.tpPrint.Controls.Add(this.cbDontShowPrintSettingDialog);
            this.tpPrint.Controls.Add(this.btnShowImagePrintSettings);
            resources.ApplyResources(this.tpPrint, "tpPrint");
            this.tpPrint.Name = "tpPrint";
            // 
            // lblDefaultPrinterOverride
            // 
            resources.ApplyResources(this.lblDefaultPrinterOverride, "lblDefaultPrinterOverride");
            this.lblDefaultPrinterOverride.Name = "lblDefaultPrinterOverride";
            // 
            // txtDefaultPrinterOverride
            // 
            resources.ApplyResources(this.txtDefaultPrinterOverride, "txtDefaultPrinterOverride");
            this.txtDefaultPrinterOverride.Name = "txtDefaultPrinterOverride";
            this.txtDefaultPrinterOverride.TextChanged += new System.EventHandler(this.txtDefaultPrinterOverride_TextChanged);
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
            this.tpProxy.BackColor = System.Drawing.SystemColors.Window;
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
            this.tpAdvanced.BackColor = System.Drawing.SystemColors.Window;
            this.tpAdvanced.Controls.Add(this.pgSettings);
            resources.ApplyResources(this.tpAdvanced, "tpAdvanced");
            this.tpAdvanced.Name = "tpAdvanced";
            // 
            // pgSettings
            // 
            resources.ApplyResources(this.pgSettings, "pgSettings");
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgSettings.ToolbarVisible = false;
            // 
            // tttvMain
            // 
            resources.ApplyResources(this.tttvMain, "tttvMain");
            this.tttvMain.ImageList = null;
            this.tttvMain.LeftPanelBackColor = System.Drawing.SystemColors.Window;
            this.tttvMain.MainTabControl = null;
            this.tttvMain.Name = "tttvMain";
            this.tttvMain.SeparatorColor = System.Drawing.SystemColors.ControlDark;
            this.tttvMain.TreeViewFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tttvMain.TreeViewSize = 175;
            this.tttvMain.TabChanged += new ShareX.HelpersLib.TabToTreeView.TabChangedEventHandler(this.tttvMain_TabChanged);
            // 
            // ApplicationSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcSettings);
            this.Controls.Add(this.tttvMain);
            this.Name = "ApplicationSettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.Resize += new System.EventHandler(this.SettingsForm_Resize);
            this.tcSettings.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.tpTheme.ResumeLayout(false);
            this.tpTheme.PerformLayout();
            this.tpIntegration.ResumeLayout(false);
            this.gbFirefox.ResumeLayout(false);
            this.gbFirefox.PerformLayout();
            this.gbSteam.ResumeLayout(false);
            this.gbSteam.PerformLayout();
            this.gbChrome.ResumeLayout(false);
            this.gbChrome.PerformLayout();
            this.gbWindows.ResumeLayout(false);
            this.gbWindows.PerformLayout();
            this.tpPaths.ResumeLayout(false);
            this.tpPaths.PerformLayout();
            this.tpSettings.ResumeLayout(false);
            this.tpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCleanupKeepFileCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExportImportNote)).EndInit();
            this.tpMainWindow.ResumeLayout(false);
            this.tpMainWindow.PerformLayout();
            this.gbListView.ResumeLayout(false);
            this.gbListView.PerformLayout();
            this.gbThumbnailView.ResumeLayout(false);
            this.gbThumbnailView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailViewThumbnailSizeHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailViewThumbnailSizeWidth)).EndInit();
            this.tpClipboardFormats.ResumeLayout(false);
            this.tpClipboardFormats.PerformLayout();
            this.tpUpload.ResumeLayout(false);
            this.tpUpload.PerformLayout();
            this.gbSecondaryFileUploaders.ResumeLayout(false);
            this.gbSecondaryImageUploaders.ResumeLayout(false);
            this.gbSecondaryTextUploaders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudUploadLimit)).EndInit();
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
        private System.Windows.Forms.CheckBox cbUseCustomTheme;
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
        private System.Windows.Forms.Label lblMainWindowTaskViewMode;
        private System.Windows.Forms.ComboBox cbMainWindowTaskViewMode;
        private System.Windows.Forms.CheckBox cbMainWindowShowMenu;
        private System.Windows.Forms.GroupBox gbThumbnailView;
        private System.Windows.Forms.CheckBox cbThumbnailViewShowTitle;
        private System.Windows.Forms.ComboBox cbThumbnailViewTitleLocation;
        private System.Windows.Forms.Label lblThumbnailViewTitleLocation;
        private System.Windows.Forms.Label lblThumbnailViewThumbnailSize;
        private System.Windows.Forms.Label lblThumbnailViewThumbnailClickAction;
        private System.Windows.Forms.GroupBox gbListView;
        private System.Windows.Forms.CheckBox cbListViewShowColumns;
        private System.Windows.Forms.ComboBox cbListViewImagePreviewVisibility;
        private System.Windows.Forms.Label lblListViewImagePreviewVisibility;
        private System.Windows.Forms.Label lblListViewImagePreviewLocation;
        private System.Windows.Forms.ComboBox cbListViewImagePreviewLocation;
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