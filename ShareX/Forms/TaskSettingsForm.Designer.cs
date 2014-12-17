namespace ShareX
{
    partial class TaskSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskSettingsForm));
            this.cmsAfterCapture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsAfterUpload = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cbUseDefaultAfterCaptureSettings = new System.Windows.Forms.CheckBox();
            this.cbUseDefaultAfterUploadSettings = new System.Windows.Forms.CheckBox();
            this.cbUseDefaultDestinationSettings = new System.Windows.Forms.CheckBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.cmsTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tcTaskSettings = new System.Windows.Forms.TabControl();
            this.tpTask = new System.Windows.Forms.TabPage();
            this.cbOverrideCustomUploader = new System.Windows.Forms.ComboBox();
            this.chkOverrideCustomUploader = new System.Windows.Forms.CheckBox();
            this.chkOverrideFTP = new System.Windows.Forms.CheckBox();
            this.cboFTPaccounts = new System.Windows.Forms.ComboBox();
            this.btnAfterCapture = new ShareX.HelpersLib.MenuButton();
            this.btnAfterUpload = new ShareX.HelpersLib.MenuButton();
            this.btnDestinations = new ShareX.HelpersLib.MenuButton();
            this.cmsDestinations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiURLSharingServices = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTask = new ShareX.HelpersLib.MenuButton();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.chkShowBeforeUploadForm = new System.Windows.Forms.CheckBox();
            this.lblAfterTaskNotification = new System.Windows.Forms.Label();
            this.cboPopUpNotification = new System.Windows.Forms.ComboBox();
            this.chkShowAfterUploadForm = new System.Windows.Forms.CheckBox();
            this.cbShowAfterCaptureTasksForm = new System.Windows.Forms.CheckBox();
            this.cbPlaySoundAfterUpload = new System.Windows.Forms.CheckBox();
            this.cbHistorySave = new System.Windows.Forms.CheckBox();
            this.cbPlaySoundAfterCapture = new System.Windows.Forms.CheckBox();
            this.chkUseDefaultGeneralSettings = new System.Windows.Forms.CheckBox();
            this.tpImage = new System.Windows.Forms.TabPage();
            this.tcImage = new System.Windows.Forms.TabControl();
            this.tpQuality = new System.Windows.Forms.TabPage();
            this.pImage = new System.Windows.Forms.Panel();
            this.lblImageFormat = new System.Windows.Forms.Label();
            this.cbImageFileExist = new System.Windows.Forms.ComboBox();
            this.lblUseImageFormat2After = new System.Windows.Forms.Label();
            this.lblImageFileExist = new System.Windows.Forms.Label();
            this.nudUseImageFormat2After = new System.Windows.Forms.NumericUpDown();
            this.lblImageFormat2 = new System.Windows.Forms.Label();
            this.lblUseImageFormat2AfterHint = new System.Windows.Forms.Label();
            this.nudImageJPEGQuality = new System.Windows.Forms.NumericUpDown();
            this.cbImageFormat = new System.Windows.Forms.ComboBox();
            this.cbImageFormat2 = new System.Windows.Forms.ComboBox();
            this.lblImageJPEGQualityHint = new System.Windows.Forms.Label();
            this.lblImageGIFQuality = new System.Windows.Forms.Label();
            this.lblImageJPEGQuality = new System.Windows.Forms.Label();
            this.cbImageGIFQuality = new System.Windows.Forms.ComboBox();
            this.chkUseDefaultImageSettings = new System.Windows.Forms.CheckBox();
            this.tpEffects = new System.Windows.Forms.TabPage();
            this.lblImageEffectsNote = new System.Windows.Forms.Label();
            this.chkShowImageEffectsWindowAfterCapture = new System.Windows.Forms.CheckBox();
            this.cbImageEffectOnlyRegionCapture = new System.Windows.Forms.CheckBox();
            this.btnImageEffects = new System.Windows.Forms.Button();
            this.tpThumbnail = new System.Windows.Forms.TabPage();
            this.cbThumbnailIfSmaller = new System.Windows.Forms.CheckBox();
            this.lblThumbnailNamePreview = new System.Windows.Forms.Label();
            this.lblThumbnailName = new System.Windows.Forms.Label();
            this.txtThumbnailName = new System.Windows.Forms.TextBox();
            this.lblThumbnailHeight = new System.Windows.Forms.Label();
            this.lblThumbnailWidth = new System.Windows.Forms.Label();
            this.nudThumbnailHeight = new System.Windows.Forms.NumericUpDown();
            this.nudThumbnailWidth = new System.Windows.Forms.NumericUpDown();
            this.tpCapture = new System.Windows.Forms.TabPage();
            this.tcCapture = new System.Windows.Forms.TabControl();
            this.tpCaptureGeneral = new System.Windows.Forms.TabPage();
            this.pCapture = new System.Windows.Forms.Panel();
            this.cbShowCursor = new System.Windows.Forms.CheckBox();
            this.lblCaptureShadowOffset = new System.Windows.Forms.Label();
            this.cbCaptureTransparent = new System.Windows.Forms.CheckBox();
            this.cbCaptureAutoHideTaskbar = new System.Windows.Forms.CheckBox();
            this.cbCaptureShadow = new System.Windows.Forms.CheckBox();
            this.lblScreenshotDelayInfo = new System.Windows.Forms.Label();
            this.cbCaptureClientArea = new System.Windows.Forms.CheckBox();
            this.nudScreenshotDelay = new System.Windows.Forms.NumericUpDown();
            this.nudCaptureShadowOffset = new System.Windows.Forms.NumericUpDown();
            this.cbScreenshotDelay = new System.Windows.Forms.CheckBox();
            this.chkUseDefaultCaptureSettings = new System.Windows.Forms.CheckBox();
            this.tpRegionCapture = new System.Windows.Forms.TabPage();
            this.pgRegionCapture = new System.Windows.Forms.PropertyGrid();
            this.tpScreenRecorder = new System.Windows.Forms.TabPage();
            this.lblScreenRecorderStartDelay = new System.Windows.Forms.Label();
            this.chkScreenRecordAutoStart = new System.Windows.Forms.CheckBox();
            this.cbScreenRecordAutoDisableAero = new System.Windows.Forms.CheckBox();
            this.lblScreenRecorderFixedDuration = new System.Windows.Forms.Label();
            this.nudScreenRecordFPS = new System.Windows.Forms.NumericUpDown();
            this.lblScreenRecordFPS = new System.Windows.Forms.Label();
            this.chkRunScreencastCLI = new System.Windows.Forms.CheckBox();
            this.btnScreenRecorderOptions = new System.Windows.Forms.Button();
            this.btnEncoderConfig = new System.Windows.Forms.Button();
            this.cboEncoder = new System.Windows.Forms.ComboBox();
            this.nudScreenRecorderDuration = new System.Windows.Forms.NumericUpDown();
            this.nudScreenRecorderStartDelay = new System.Windows.Forms.NumericUpDown();
            this.cbScreenRecorderOutput = new System.Windows.Forms.ComboBox();
            this.lblScreenRecorderOutput = new System.Windows.Forms.Label();
            this.cbScreenRecorderFixedDuration = new System.Windows.Forms.CheckBox();
            this.nudGIFFPS = new System.Windows.Forms.NumericUpDown();
            this.lblGIFFPS = new System.Windows.Forms.Label();
            this.tpRectangleAnnotate = new System.Windows.Forms.TabPage();
            this.pgRectangleAnnotate = new System.Windows.Forms.PropertyGrid();
            this.tpActions = new System.Windows.Forms.TabPage();
            this.pActions = new System.Windows.Forms.Panel();
            this.btnActionsDuplicate = new System.Windows.Forms.Button();
            this.btnActionsAdd = new System.Windows.Forms.Button();
            this.lvActions = new ShareX.HelpersLib.MyListView();
            this.chActionsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsArgs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsExtensions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnActionsEdit = new System.Windows.Forms.Button();
            this.btnActionsRemove = new System.Windows.Forms.Button();
            this.chkUseDefaultActions = new System.Windows.Forms.CheckBox();
            this.tpWatchFolders = new System.Windows.Forms.TabPage();
            this.cbWatchFolderEnabled = new System.Windows.Forms.CheckBox();
            this.lvWatchFolderList = new System.Windows.Forms.ListView();
            this.chWatchFolderFolderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWatchFolderFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWatchFolderIncludeSubdirectories = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnWatchFolderRemove = new System.Windows.Forms.Button();
            this.btnWatchFolderAdd = new System.Windows.Forms.Button();
            this.tpUpload = new System.Windows.Forms.TabPage();
            this.tcUpload = new System.Windows.Forms.TabControl();
            this.tpUploadNamePattern = new System.Windows.Forms.TabPage();
            this.pUpload = new System.Windows.Forms.Panel();
            this.lblNameFormatPattern = new System.Windows.Forms.Label();
            this.cbFileUploadUseNamePattern = new System.Windows.Forms.CheckBox();
            this.lblNameFormatPatternPreviewActiveWindow = new System.Windows.Forms.Label();
            this.lblNameFormatPatternPreview = new System.Windows.Forms.Label();
            this.txtNameFormatPatternActiveWindow = new System.Windows.Forms.TextBox();
            this.txtNameFormatPattern = new System.Windows.Forms.TextBox();
            this.btnResetAutoIncrementNumber = new System.Windows.Forms.Button();
            this.lblNameFormatPatternActiveWindow = new System.Windows.Forms.Label();
            this.chkUseDefaultUploadSettings = new System.Windows.Forms.CheckBox();
            this.tpUploadClipboard = new System.Windows.Forms.TabPage();
            this.cbClipboardUploadShareURL = new System.Windows.Forms.CheckBox();
            this.chkClipboardUploadURLContents = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadAutoIndexFolder = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadShortenURL = new System.Windows.Forms.CheckBox();
            this.tpIndexer = new System.Windows.Forms.TabPage();
            this.pgIndexerConfig = new System.Windows.Forms.PropertyGrid();
            this.chkUseDefaultIndexerSettings = new System.Windows.Forms.CheckBox();
            this.tpAdvanced = new System.Windows.Forms.TabPage();
            this.pgTaskSettings = new System.Windows.Forms.PropertyGrid();
            this.chkUseDefaultAdvancedSettings = new System.Windows.Forms.CheckBox();
            this.tttvMain = new ShareX.HelpersLib.TabToTreeView();
            this.tcTaskSettings.SuspendLayout();
            this.tpTask.SuspendLayout();
            this.cmsDestinations.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.tpImage.SuspendLayout();
            this.tcImage.SuspendLayout();
            this.tpQuality.SuspendLayout();
            this.pImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUseImageFormat2After)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageJPEGQuality)).BeginInit();
            this.tpEffects.SuspendLayout();
            this.tpThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailWidth)).BeginInit();
            this.tpCapture.SuspendLayout();
            this.tcCapture.SuspendLayout();
            this.tpCaptureGeneral.SuspendLayout();
            this.pCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenshotDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureShadowOffset)).BeginInit();
            this.tpRegionCapture.SuspendLayout();
            this.tpScreenRecorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecordFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGIFFPS)).BeginInit();
            this.tpRectangleAnnotate.SuspendLayout();
            this.tpActions.SuspendLayout();
            this.pActions.SuspendLayout();
            this.tpWatchFolders.SuspendLayout();
            this.tpUpload.SuspendLayout();
            this.tcUpload.SuspendLayout();
            this.tpUploadNamePattern.SuspendLayout();
            this.pUpload.SuspendLayout();
            this.tpUploadClipboard.SuspendLayout();
            this.tpIndexer.SuspendLayout();
            this.tpAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsAfterCapture
            // 
            this.cmsAfterCapture.Name = "cmsAfterCapture";
            resources.ApplyResources(this.cmsAfterCapture, "cmsAfterCapture");
            // 
            // cmsAfterUpload
            // 
            this.cmsAfterUpload.Name = "cmsAfterCapture";
            resources.ApplyResources(this.cmsAfterUpload, "cmsAfterUpload");
            // 
            // cbUseDefaultAfterCaptureSettings
            // 
            resources.ApplyResources(this.cbUseDefaultAfterCaptureSettings, "cbUseDefaultAfterCaptureSettings");
            this.cbUseDefaultAfterCaptureSettings.Name = "cbUseDefaultAfterCaptureSettings";
            this.cbUseDefaultAfterCaptureSettings.UseVisualStyleBackColor = true;
            this.cbUseDefaultAfterCaptureSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAfterCaptureSettings_CheckedChanged);
            // 
            // cbUseDefaultAfterUploadSettings
            // 
            resources.ApplyResources(this.cbUseDefaultAfterUploadSettings, "cbUseDefaultAfterUploadSettings");
            this.cbUseDefaultAfterUploadSettings.Name = "cbUseDefaultAfterUploadSettings";
            this.cbUseDefaultAfterUploadSettings.UseVisualStyleBackColor = true;
            this.cbUseDefaultAfterUploadSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAfterUploadSettings_CheckedChanged);
            // 
            // cbUseDefaultDestinationSettings
            // 
            resources.ApplyResources(this.cbUseDefaultDestinationSettings, "cbUseDefaultDestinationSettings");
            this.cbUseDefaultDestinationSettings.Name = "cbUseDefaultDestinationSettings";
            this.cbUseDefaultDestinationSettings.UseVisualStyleBackColor = true;
            this.cbUseDefaultDestinationSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultDestinationSettings_CheckedChanged);
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.TextChanged += new System.EventHandler(this.tbDescription_TextChanged);
            // 
            // cmsTask
            // 
            this.cmsTask.Name = "cmsAfterCapture";
            resources.ApplyResources(this.cmsTask, "cmsTask");
            // 
            // tcTaskSettings
            // 
            this.tcTaskSettings.Controls.Add(this.tpTask);
            this.tcTaskSettings.Controls.Add(this.tpGeneral);
            this.tcTaskSettings.Controls.Add(this.tpImage);
            this.tcTaskSettings.Controls.Add(this.tpCapture);
            this.tcTaskSettings.Controls.Add(this.tpActions);
            this.tcTaskSettings.Controls.Add(this.tpWatchFolders);
            this.tcTaskSettings.Controls.Add(this.tpUpload);
            this.tcTaskSettings.Controls.Add(this.tpIndexer);
            this.tcTaskSettings.Controls.Add(this.tpAdvanced);
            resources.ApplyResources(this.tcTaskSettings, "tcTaskSettings");
            this.tcTaskSettings.Name = "tcTaskSettings";
            this.tcTaskSettings.SelectedIndex = 0;
            // 
            // tpTask
            // 
            this.tpTask.Controls.Add(this.cbOverrideCustomUploader);
            this.tpTask.Controls.Add(this.chkOverrideCustomUploader);
            this.tpTask.Controls.Add(this.chkOverrideFTP);
            this.tpTask.Controls.Add(this.cboFTPaccounts);
            this.tpTask.Controls.Add(this.tbDescription);
            this.tpTask.Controls.Add(this.btnAfterCapture);
            this.tpTask.Controls.Add(this.btnAfterUpload);
            this.tpTask.Controls.Add(this.btnDestinations);
            this.tpTask.Controls.Add(this.cbUseDefaultAfterCaptureSettings);
            this.tpTask.Controls.Add(this.btnTask);
            this.tpTask.Controls.Add(this.cbUseDefaultAfterUploadSettings);
            this.tpTask.Controls.Add(this.cbUseDefaultDestinationSettings);
            this.tpTask.Controls.Add(this.lblDescription);
            resources.ApplyResources(this.tpTask, "tpTask");
            this.tpTask.Name = "tpTask";
            this.tpTask.UseVisualStyleBackColor = true;
            // 
            // cbOverrideCustomUploader
            // 
            this.cbOverrideCustomUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbOverrideCustomUploader, "cbOverrideCustomUploader");
            this.cbOverrideCustomUploader.FormattingEnabled = true;
            this.cbOverrideCustomUploader.Name = "cbOverrideCustomUploader";
            this.cbOverrideCustomUploader.SelectedIndexChanged += new System.EventHandler(this.cbOverrideCustomUploader_SelectedIndexChanged);
            // 
            // chkOverrideCustomUploader
            // 
            resources.ApplyResources(this.chkOverrideCustomUploader, "chkOverrideCustomUploader");
            this.chkOverrideCustomUploader.Name = "chkOverrideCustomUploader";
            this.chkOverrideCustomUploader.UseVisualStyleBackColor = true;
            this.chkOverrideCustomUploader.CheckedChanged += new System.EventHandler(this.chkOverrideCustomUploader_CheckedChanged);
            // 
            // chkOverrideFTP
            // 
            resources.ApplyResources(this.chkOverrideFTP, "chkOverrideFTP");
            this.chkOverrideFTP.Name = "chkOverrideFTP";
            this.chkOverrideFTP.UseVisualStyleBackColor = true;
            this.chkOverrideFTP.CheckedChanged += new System.EventHandler(this.chkOverrideFTP_CheckedChanged);
            // 
            // cboFTPaccounts
            // 
            this.cboFTPaccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboFTPaccounts, "cboFTPaccounts");
            this.cboFTPaccounts.FormattingEnabled = true;
            this.cboFTPaccounts.Name = "cboFTPaccounts";
            this.cboFTPaccounts.SelectedIndexChanged += new System.EventHandler(this.cboFTPaccounts_SelectedIndexChanged);
            // 
            // btnAfterCapture
            // 
            resources.ApplyResources(this.btnAfterCapture, "btnAfterCapture");
            this.btnAfterCapture.Menu = this.cmsAfterCapture;
            this.btnAfterCapture.Name = "btnAfterCapture";
            this.btnAfterCapture.UseMnemonic = false;
            this.btnAfterCapture.UseVisualStyleBackColor = true;
            // 
            // btnAfterUpload
            // 
            resources.ApplyResources(this.btnAfterUpload, "btnAfterUpload");
            this.btnAfterUpload.Menu = this.cmsAfterUpload;
            this.btnAfterUpload.Name = "btnAfterUpload";
            this.btnAfterUpload.UseMnemonic = false;
            this.btnAfterUpload.UseVisualStyleBackColor = true;
            // 
            // btnDestinations
            // 
            resources.ApplyResources(this.btnDestinations, "btnDestinations");
            this.btnDestinations.Menu = this.cmsDestinations;
            this.btnDestinations.Name = "btnDestinations";
            this.btnDestinations.UseMnemonic = false;
            this.btnDestinations.UseVisualStyleBackColor = true;
            // 
            // cmsDestinations
            // 
            this.cmsDestinations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImageUploaders,
            this.tsmiTextUploaders,
            this.tsmiFileUploaders,
            this.tsmiURLShorteners,
            this.tsmiURLSharingServices});
            this.cmsDestinations.Name = "cmsDestinations";
            resources.ApplyResources(this.cmsDestinations, "cmsDestinations");
            // 
            // tsmiImageUploaders
            // 
            this.tsmiImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiImageUploaders.Name = "tsmiImageUploaders";
            resources.ApplyResources(this.tsmiImageUploaders, "tsmiImageUploaders");
            // 
            // tsmiTextUploaders
            // 
            this.tsmiTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTextUploaders.Name = "tsmiTextUploaders";
            resources.ApplyResources(this.tsmiTextUploaders, "tsmiTextUploaders");
            // 
            // tsmiFileUploaders
            // 
            this.tsmiFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiFileUploaders.Name = "tsmiFileUploaders";
            resources.ApplyResources(this.tsmiFileUploaders, "tsmiFileUploaders");
            // 
            // tsmiURLShorteners
            // 
            this.tsmiURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiURLShorteners.Name = "tsmiURLShorteners";
            resources.ApplyResources(this.tsmiURLShorteners, "tsmiURLShorteners");
            // 
            // tsmiURLSharingServices
            // 
            this.tsmiURLSharingServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiURLSharingServices.Name = "tsmiURLSharingServices";
            resources.ApplyResources(this.tsmiURLSharingServices, "tsmiURLSharingServices");
            // 
            // btnTask
            // 
            resources.ApplyResources(this.btnTask, "btnTask");
            this.btnTask.Menu = this.cmsTask;
            this.btnTask.Name = "btnTask";
            this.btnTask.UseMnemonic = false;
            this.btnTask.UseVisualStyleBackColor = true;
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.panelGeneral);
            this.tpGeneral.Controls.Add(this.chkUseDefaultGeneralSettings);
            resources.ApplyResources(this.tpGeneral, "tpGeneral");
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // panelGeneral
            // 
            this.panelGeneral.Controls.Add(this.chkShowBeforeUploadForm);
            this.panelGeneral.Controls.Add(this.lblAfterTaskNotification);
            this.panelGeneral.Controls.Add(this.cboPopUpNotification);
            this.panelGeneral.Controls.Add(this.chkShowAfterUploadForm);
            this.panelGeneral.Controls.Add(this.cbShowAfterCaptureTasksForm);
            this.panelGeneral.Controls.Add(this.cbPlaySoundAfterUpload);
            this.panelGeneral.Controls.Add(this.cbHistorySave);
            this.panelGeneral.Controls.Add(this.cbPlaySoundAfterCapture);
            resources.ApplyResources(this.panelGeneral, "panelGeneral");
            this.panelGeneral.Name = "panelGeneral";
            // 
            // chkShowBeforeUploadForm
            // 
            resources.ApplyResources(this.chkShowBeforeUploadForm, "chkShowBeforeUploadForm");
            this.chkShowBeforeUploadForm.Name = "chkShowBeforeUploadForm";
            this.chkShowBeforeUploadForm.UseVisualStyleBackColor = true;
            this.chkShowBeforeUploadForm.CheckedChanged += new System.EventHandler(this.chkShowBeforeUploadForm_CheckedChanged);
            // 
            // lblAfterTaskNotification
            // 
            resources.ApplyResources(this.lblAfterTaskNotification, "lblAfterTaskNotification");
            this.lblAfterTaskNotification.Name = "lblAfterTaskNotification";
            // 
            // cboPopUpNotification
            // 
            this.cboPopUpNotification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPopUpNotification.FormattingEnabled = true;
            resources.ApplyResources(this.cboPopUpNotification, "cboPopUpNotification");
            this.cboPopUpNotification.Name = "cboPopUpNotification";
            this.cboPopUpNotification.SelectedIndexChanged += new System.EventHandler(this.cboPopUpNotification_SelectedIndexChanged);
            // 
            // chkShowAfterUploadForm
            // 
            resources.ApplyResources(this.chkShowAfterUploadForm, "chkShowAfterUploadForm");
            this.chkShowAfterUploadForm.Name = "chkShowAfterUploadForm";
            this.chkShowAfterUploadForm.UseVisualStyleBackColor = true;
            this.chkShowAfterUploadForm.CheckedChanged += new System.EventHandler(this.chkShowAfterUploadForm_CheckedChanged);
            // 
            // cbShowAfterCaptureTasksForm
            // 
            resources.ApplyResources(this.cbShowAfterCaptureTasksForm, "cbShowAfterCaptureTasksForm");
            this.cbShowAfterCaptureTasksForm.Name = "cbShowAfterCaptureTasksForm";
            this.cbShowAfterCaptureTasksForm.UseVisualStyleBackColor = true;
            this.cbShowAfterCaptureTasksForm.CheckedChanged += new System.EventHandler(this.cbShowAfterCaptureTasksForm_CheckedChanged);
            // 
            // cbPlaySoundAfterUpload
            // 
            resources.ApplyResources(this.cbPlaySoundAfterUpload, "cbPlaySoundAfterUpload");
            this.cbPlaySoundAfterUpload.Name = "cbPlaySoundAfterUpload";
            this.cbPlaySoundAfterUpload.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterUpload.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterUpload_CheckedChanged);
            // 
            // cbHistorySave
            // 
            resources.ApplyResources(this.cbHistorySave, "cbHistorySave");
            this.cbHistorySave.Name = "cbHistorySave";
            this.cbHistorySave.UseVisualStyleBackColor = true;
            this.cbHistorySave.CheckedChanged += new System.EventHandler(this.cbHistorySave_CheckedChanged);
            // 
            // cbPlaySoundAfterCapture
            // 
            resources.ApplyResources(this.cbPlaySoundAfterCapture, "cbPlaySoundAfterCapture");
            this.cbPlaySoundAfterCapture.Name = "cbPlaySoundAfterCapture";
            this.cbPlaySoundAfterCapture.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterCapture.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterCapture_CheckedChanged);
            // 
            // chkUseDefaultGeneralSettings
            // 
            resources.ApplyResources(this.chkUseDefaultGeneralSettings, "chkUseDefaultGeneralSettings");
            this.chkUseDefaultGeneralSettings.Checked = true;
            this.chkUseDefaultGeneralSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultGeneralSettings.Name = "chkUseDefaultGeneralSettings";
            this.chkUseDefaultGeneralSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultGeneralSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultGeneralSettings_CheckedChanged);
            // 
            // tpImage
            // 
            this.tpImage.Controls.Add(this.tcImage);
            resources.ApplyResources(this.tpImage, "tpImage");
            this.tpImage.Name = "tpImage";
            this.tpImage.UseVisualStyleBackColor = true;
            // 
            // tcImage
            // 
            this.tcImage.Controls.Add(this.tpQuality);
            this.tcImage.Controls.Add(this.tpEffects);
            this.tcImage.Controls.Add(this.tpThumbnail);
            resources.ApplyResources(this.tcImage, "tcImage");
            this.tcImage.Name = "tcImage";
            this.tcImage.SelectedIndex = 0;
            // 
            // tpQuality
            // 
            this.tpQuality.Controls.Add(this.pImage);
            this.tpQuality.Controls.Add(this.chkUseDefaultImageSettings);
            resources.ApplyResources(this.tpQuality, "tpQuality");
            this.tpQuality.Name = "tpQuality";
            this.tpQuality.UseVisualStyleBackColor = true;
            // 
            // pImage
            // 
            this.pImage.Controls.Add(this.lblImageFormat);
            this.pImage.Controls.Add(this.cbImageFileExist);
            this.pImage.Controls.Add(this.lblUseImageFormat2After);
            this.pImage.Controls.Add(this.lblImageFileExist);
            this.pImage.Controls.Add(this.nudUseImageFormat2After);
            this.pImage.Controls.Add(this.lblImageFormat2);
            this.pImage.Controls.Add(this.lblUseImageFormat2AfterHint);
            this.pImage.Controls.Add(this.nudImageJPEGQuality);
            this.pImage.Controls.Add(this.cbImageFormat);
            this.pImage.Controls.Add(this.cbImageFormat2);
            this.pImage.Controls.Add(this.lblImageJPEGQualityHint);
            this.pImage.Controls.Add(this.lblImageGIFQuality);
            this.pImage.Controls.Add(this.lblImageJPEGQuality);
            this.pImage.Controls.Add(this.cbImageGIFQuality);
            resources.ApplyResources(this.pImage, "pImage");
            this.pImage.Name = "pImage";
            // 
            // lblImageFormat
            // 
            resources.ApplyResources(this.lblImageFormat, "lblImageFormat");
            this.lblImageFormat.Name = "lblImageFormat";
            // 
            // cbImageFileExist
            // 
            this.cbImageFileExist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageFileExist.FormattingEnabled = true;
            resources.ApplyResources(this.cbImageFileExist, "cbImageFileExist");
            this.cbImageFileExist.Name = "cbImageFileExist";
            this.cbImageFileExist.SelectedIndexChanged += new System.EventHandler(this.cbImageFileExist_SelectedIndexChanged);
            // 
            // lblUseImageFormat2After
            // 
            resources.ApplyResources(this.lblUseImageFormat2After, "lblUseImageFormat2After");
            this.lblUseImageFormat2After.Name = "lblUseImageFormat2After";
            // 
            // lblImageFileExist
            // 
            resources.ApplyResources(this.lblImageFileExist, "lblImageFileExist");
            this.lblImageFileExist.Name = "lblImageFileExist";
            // 
            // nudUseImageFormat2After
            // 
            resources.ApplyResources(this.nudUseImageFormat2After, "nudUseImageFormat2After");
            this.nudUseImageFormat2After.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudUseImageFormat2After.Name = "nudUseImageFormat2After";
            this.nudUseImageFormat2After.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudUseImageFormat2After.ValueChanged += new System.EventHandler(this.nudUseImageFormat2After_ValueChanged);
            // 
            // lblImageFormat2
            // 
            resources.ApplyResources(this.lblImageFormat2, "lblImageFormat2");
            this.lblImageFormat2.Name = "lblImageFormat2";
            // 
            // lblUseImageFormat2AfterHint
            // 
            resources.ApplyResources(this.lblUseImageFormat2AfterHint, "lblUseImageFormat2AfterHint");
            this.lblUseImageFormat2AfterHint.Name = "lblUseImageFormat2AfterHint";
            // 
            // nudImageJPEGQuality
            // 
            resources.ApplyResources(this.nudImageJPEGQuality, "nudImageJPEGQuality");
            this.nudImageJPEGQuality.Name = "nudImageJPEGQuality";
            this.nudImageJPEGQuality.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudImageJPEGQuality.ValueChanged += new System.EventHandler(this.nudImageJPEGQuality_ValueChanged);
            // 
            // cbImageFormat
            // 
            this.cbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageFormat.FormattingEnabled = true;
            resources.ApplyResources(this.cbImageFormat, "cbImageFormat");
            this.cbImageFormat.Name = "cbImageFormat";
            this.cbImageFormat.SelectedIndexChanged += new System.EventHandler(this.cbImageFormat_SelectedIndexChanged);
            // 
            // cbImageFormat2
            // 
            this.cbImageFormat2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageFormat2.FormattingEnabled = true;
            resources.ApplyResources(this.cbImageFormat2, "cbImageFormat2");
            this.cbImageFormat2.Name = "cbImageFormat2";
            this.cbImageFormat2.SelectedIndexChanged += new System.EventHandler(this.cbImageFormat2_SelectedIndexChanged);
            // 
            // lblImageJPEGQualityHint
            // 
            resources.ApplyResources(this.lblImageJPEGQualityHint, "lblImageJPEGQualityHint");
            this.lblImageJPEGQualityHint.Name = "lblImageJPEGQualityHint";
            // 
            // lblImageGIFQuality
            // 
            resources.ApplyResources(this.lblImageGIFQuality, "lblImageGIFQuality");
            this.lblImageGIFQuality.Name = "lblImageGIFQuality";
            // 
            // lblImageJPEGQuality
            // 
            resources.ApplyResources(this.lblImageJPEGQuality, "lblImageJPEGQuality");
            this.lblImageJPEGQuality.Name = "lblImageJPEGQuality";
            // 
            // cbImageGIFQuality
            // 
            this.cbImageGIFQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageGIFQuality.FormattingEnabled = true;
            resources.ApplyResources(this.cbImageGIFQuality, "cbImageGIFQuality");
            this.cbImageGIFQuality.Name = "cbImageGIFQuality";
            this.cbImageGIFQuality.SelectedIndexChanged += new System.EventHandler(this.cbImageGIFQuality_SelectedIndexChanged);
            // 
            // chkUseDefaultImageSettings
            // 
            resources.ApplyResources(this.chkUseDefaultImageSettings, "chkUseDefaultImageSettings");
            this.chkUseDefaultImageSettings.Checked = true;
            this.chkUseDefaultImageSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultImageSettings.Name = "chkUseDefaultImageSettings";
            this.chkUseDefaultImageSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultImageSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultImageSettings_CheckedChanged);
            // 
            // tpEffects
            // 
            this.tpEffects.Controls.Add(this.lblImageEffectsNote);
            this.tpEffects.Controls.Add(this.chkShowImageEffectsWindowAfterCapture);
            this.tpEffects.Controls.Add(this.cbImageEffectOnlyRegionCapture);
            this.tpEffects.Controls.Add(this.btnImageEffects);
            resources.ApplyResources(this.tpEffects, "tpEffects");
            this.tpEffects.Name = "tpEffects";
            this.tpEffects.UseVisualStyleBackColor = true;
            // 
            // lblImageEffectsNote
            // 
            resources.ApplyResources(this.lblImageEffectsNote, "lblImageEffectsNote");
            this.lblImageEffectsNote.Name = "lblImageEffectsNote";
            // 
            // chkShowImageEffectsWindowAfterCapture
            // 
            resources.ApplyResources(this.chkShowImageEffectsWindowAfterCapture, "chkShowImageEffectsWindowAfterCapture");
            this.chkShowImageEffectsWindowAfterCapture.Name = "chkShowImageEffectsWindowAfterCapture";
            this.chkShowImageEffectsWindowAfterCapture.UseVisualStyleBackColor = true;
            this.chkShowImageEffectsWindowAfterCapture.CheckedChanged += new System.EventHandler(this.chkShowImageEffectsWindowAfterCapture_CheckedChanged);
            // 
            // cbImageEffectOnlyRegionCapture
            // 
            resources.ApplyResources(this.cbImageEffectOnlyRegionCapture, "cbImageEffectOnlyRegionCapture");
            this.cbImageEffectOnlyRegionCapture.Name = "cbImageEffectOnlyRegionCapture";
            this.cbImageEffectOnlyRegionCapture.UseVisualStyleBackColor = true;
            this.cbImageEffectOnlyRegionCapture.CheckedChanged += new System.EventHandler(this.cbImageEffectOnlyRegionCapture_CheckedChanged);
            // 
            // btnImageEffects
            // 
            resources.ApplyResources(this.btnImageEffects, "btnImageEffects");
            this.btnImageEffects.Name = "btnImageEffects";
            this.btnImageEffects.UseVisualStyleBackColor = true;
            this.btnImageEffects.Click += new System.EventHandler(this.btnImageEffects_Click);
            // 
            // tpThumbnail
            // 
            this.tpThumbnail.Controls.Add(this.cbThumbnailIfSmaller);
            this.tpThumbnail.Controls.Add(this.lblThumbnailNamePreview);
            this.tpThumbnail.Controls.Add(this.lblThumbnailName);
            this.tpThumbnail.Controls.Add(this.txtThumbnailName);
            this.tpThumbnail.Controls.Add(this.lblThumbnailHeight);
            this.tpThumbnail.Controls.Add(this.lblThumbnailWidth);
            this.tpThumbnail.Controls.Add(this.nudThumbnailHeight);
            this.tpThumbnail.Controls.Add(this.nudThumbnailWidth);
            resources.ApplyResources(this.tpThumbnail, "tpThumbnail");
            this.tpThumbnail.Name = "tpThumbnail";
            this.tpThumbnail.UseVisualStyleBackColor = true;
            // 
            // cbThumbnailIfSmaller
            // 
            resources.ApplyResources(this.cbThumbnailIfSmaller, "cbThumbnailIfSmaller");
            this.cbThumbnailIfSmaller.Name = "cbThumbnailIfSmaller";
            this.cbThumbnailIfSmaller.UseVisualStyleBackColor = true;
            this.cbThumbnailIfSmaller.CheckedChanged += new System.EventHandler(this.cbThumbnailIfSmaller_CheckedChanged);
            // 
            // lblThumbnailNamePreview
            // 
            resources.ApplyResources(this.lblThumbnailNamePreview, "lblThumbnailNamePreview");
            this.lblThumbnailNamePreview.Name = "lblThumbnailNamePreview";
            // 
            // lblThumbnailName
            // 
            resources.ApplyResources(this.lblThumbnailName, "lblThumbnailName");
            this.lblThumbnailName.Name = "lblThumbnailName";
            // 
            // txtThumbnailName
            // 
            resources.ApplyResources(this.txtThumbnailName, "txtThumbnailName");
            this.txtThumbnailName.Name = "txtThumbnailName";
            this.txtThumbnailName.TextChanged += new System.EventHandler(this.txtThumbnailName_TextChanged);
            // 
            // lblThumbnailHeight
            // 
            resources.ApplyResources(this.lblThumbnailHeight, "lblThumbnailHeight");
            this.lblThumbnailHeight.Name = "lblThumbnailHeight";
            // 
            // lblThumbnailWidth
            // 
            resources.ApplyResources(this.lblThumbnailWidth, "lblThumbnailWidth");
            this.lblThumbnailWidth.Name = "lblThumbnailWidth";
            // 
            // nudThumbnailHeight
            // 
            resources.ApplyResources(this.nudThumbnailHeight, "nudThumbnailHeight");
            this.nudThumbnailHeight.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudThumbnailHeight.Name = "nudThumbnailHeight";
            this.nudThumbnailHeight.ValueChanged += new System.EventHandler(this.nudThumbnailHeight_ValueChanged);
            // 
            // nudThumbnailWidth
            // 
            resources.ApplyResources(this.nudThumbnailWidth, "nudThumbnailWidth");
            this.nudThumbnailWidth.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudThumbnailWidth.Name = "nudThumbnailWidth";
            this.nudThumbnailWidth.ValueChanged += new System.EventHandler(this.nudThumbnailWidth_ValueChanged);
            // 
            // tpCapture
            // 
            this.tpCapture.Controls.Add(this.tcCapture);
            resources.ApplyResources(this.tpCapture, "tpCapture");
            this.tpCapture.Name = "tpCapture";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // tcCapture
            // 
            this.tcCapture.Controls.Add(this.tpCaptureGeneral);
            this.tcCapture.Controls.Add(this.tpRegionCapture);
            this.tcCapture.Controls.Add(this.tpScreenRecorder);
            this.tcCapture.Controls.Add(this.tpRectangleAnnotate);
            resources.ApplyResources(this.tcCapture, "tcCapture");
            this.tcCapture.Name = "tcCapture";
            this.tcCapture.SelectedIndex = 0;
            // 
            // tpCaptureGeneral
            // 
            this.tpCaptureGeneral.Controls.Add(this.pCapture);
            this.tpCaptureGeneral.Controls.Add(this.chkUseDefaultCaptureSettings);
            resources.ApplyResources(this.tpCaptureGeneral, "tpCaptureGeneral");
            this.tpCaptureGeneral.Name = "tpCaptureGeneral";
            this.tpCaptureGeneral.UseVisualStyleBackColor = true;
            // 
            // pCapture
            // 
            this.pCapture.Controls.Add(this.cbShowCursor);
            this.pCapture.Controls.Add(this.lblCaptureShadowOffset);
            this.pCapture.Controls.Add(this.cbCaptureTransparent);
            this.pCapture.Controls.Add(this.cbCaptureAutoHideTaskbar);
            this.pCapture.Controls.Add(this.cbCaptureShadow);
            this.pCapture.Controls.Add(this.lblScreenshotDelayInfo);
            this.pCapture.Controls.Add(this.cbCaptureClientArea);
            this.pCapture.Controls.Add(this.nudScreenshotDelay);
            this.pCapture.Controls.Add(this.nudCaptureShadowOffset);
            this.pCapture.Controls.Add(this.cbScreenshotDelay);
            resources.ApplyResources(this.pCapture, "pCapture");
            this.pCapture.Name = "pCapture";
            // 
            // cbShowCursor
            // 
            resources.ApplyResources(this.cbShowCursor, "cbShowCursor");
            this.cbShowCursor.Name = "cbShowCursor";
            this.cbShowCursor.UseVisualStyleBackColor = true;
            this.cbShowCursor.CheckedChanged += new System.EventHandler(this.cbShowCursor_CheckedChanged);
            // 
            // lblCaptureShadowOffset
            // 
            resources.ApplyResources(this.lblCaptureShadowOffset, "lblCaptureShadowOffset");
            this.lblCaptureShadowOffset.Name = "lblCaptureShadowOffset";
            // 
            // cbCaptureTransparent
            // 
            resources.ApplyResources(this.cbCaptureTransparent, "cbCaptureTransparent");
            this.cbCaptureTransparent.Name = "cbCaptureTransparent";
            this.cbCaptureTransparent.UseVisualStyleBackColor = true;
            this.cbCaptureTransparent.CheckedChanged += new System.EventHandler(this.cbCaptureTransparent_CheckedChanged);
            // 
            // cbCaptureAutoHideTaskbar
            // 
            resources.ApplyResources(this.cbCaptureAutoHideTaskbar, "cbCaptureAutoHideTaskbar");
            this.cbCaptureAutoHideTaskbar.Name = "cbCaptureAutoHideTaskbar";
            this.cbCaptureAutoHideTaskbar.UseVisualStyleBackColor = true;
            this.cbCaptureAutoHideTaskbar.CheckedChanged += new System.EventHandler(this.cbCaptureAutoHideTaskbar_CheckedChanged);
            // 
            // cbCaptureShadow
            // 
            resources.ApplyResources(this.cbCaptureShadow, "cbCaptureShadow");
            this.cbCaptureShadow.Name = "cbCaptureShadow";
            this.cbCaptureShadow.UseVisualStyleBackColor = true;
            this.cbCaptureShadow.CheckedChanged += new System.EventHandler(this.cbCaptureShadow_CheckedChanged);
            // 
            // lblScreenshotDelayInfo
            // 
            resources.ApplyResources(this.lblScreenshotDelayInfo, "lblScreenshotDelayInfo");
            this.lblScreenshotDelayInfo.Name = "lblScreenshotDelayInfo";
            // 
            // cbCaptureClientArea
            // 
            resources.ApplyResources(this.cbCaptureClientArea, "cbCaptureClientArea");
            this.cbCaptureClientArea.Name = "cbCaptureClientArea";
            this.cbCaptureClientArea.UseVisualStyleBackColor = true;
            this.cbCaptureClientArea.CheckedChanged += new System.EventHandler(this.cbCaptureClientArea_CheckedChanged);
            // 
            // nudScreenshotDelay
            // 
            this.nudScreenshotDelay.DecimalPlaces = 1;
            this.nudScreenshotDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudScreenshotDelay, "nudScreenshotDelay");
            this.nudScreenshotDelay.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudScreenshotDelay.Name = "nudScreenshotDelay";
            this.nudScreenshotDelay.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudScreenshotDelay.ValueChanged += new System.EventHandler(this.nudScreenshotDelay_ValueChanged);
            // 
            // nudCaptureShadowOffset
            // 
            resources.ApplyResources(this.nudCaptureShadowOffset, "nudCaptureShadowOffset");
            this.nudCaptureShadowOffset.Name = "nudCaptureShadowOffset";
            this.nudCaptureShadowOffset.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudCaptureShadowOffset.ValueChanged += new System.EventHandler(this.nudCaptureShadowOffset_ValueChanged);
            // 
            // cbScreenshotDelay
            // 
            resources.ApplyResources(this.cbScreenshotDelay, "cbScreenshotDelay");
            this.cbScreenshotDelay.Name = "cbScreenshotDelay";
            this.cbScreenshotDelay.UseVisualStyleBackColor = true;
            this.cbScreenshotDelay.CheckedChanged += new System.EventHandler(this.cbScreenshotDelay_CheckedChanged);
            // 
            // chkUseDefaultCaptureSettings
            // 
            resources.ApplyResources(this.chkUseDefaultCaptureSettings, "chkUseDefaultCaptureSettings");
            this.chkUseDefaultCaptureSettings.Checked = true;
            this.chkUseDefaultCaptureSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultCaptureSettings.Name = "chkUseDefaultCaptureSettings";
            this.chkUseDefaultCaptureSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultCaptureSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultCaptureSettings_CheckedChanged);
            // 
            // tpRegionCapture
            // 
            this.tpRegionCapture.Controls.Add(this.pgRegionCapture);
            resources.ApplyResources(this.tpRegionCapture, "tpRegionCapture");
            this.tpRegionCapture.Name = "tpRegionCapture";
            this.tpRegionCapture.UseVisualStyleBackColor = true;
            // 
            // pgRegionCapture
            // 
            resources.ApplyResources(this.pgRegionCapture, "pgRegionCapture");
            this.pgRegionCapture.Name = "pgRegionCapture";
            this.pgRegionCapture.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgRegionCapture.ToolbarVisible = false;
            // 
            // tpScreenRecorder
            // 
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderStartDelay);
            this.tpScreenRecorder.Controls.Add(this.chkScreenRecordAutoStart);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecordAutoDisableAero);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderFixedDuration);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecordFPS);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecordFPS);
            this.tpScreenRecorder.Controls.Add(this.chkRunScreencastCLI);
            this.tpScreenRecorder.Controls.Add(this.btnScreenRecorderOptions);
            this.tpScreenRecorder.Controls.Add(this.btnEncoderConfig);
            this.tpScreenRecorder.Controls.Add(this.cboEncoder);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderDuration);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderStartDelay);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecorderOutput);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderOutput);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecorderFixedDuration);
            this.tpScreenRecorder.Controls.Add(this.nudGIFFPS);
            this.tpScreenRecorder.Controls.Add(this.lblGIFFPS);
            resources.ApplyResources(this.tpScreenRecorder, "tpScreenRecorder");
            this.tpScreenRecorder.Name = "tpScreenRecorder";
            this.tpScreenRecorder.UseVisualStyleBackColor = true;
            // 
            // lblScreenRecorderStartDelay
            // 
            resources.ApplyResources(this.lblScreenRecorderStartDelay, "lblScreenRecorderStartDelay");
            this.lblScreenRecorderStartDelay.Name = "lblScreenRecorderStartDelay";
            // 
            // chkScreenRecordAutoStart
            // 
            resources.ApplyResources(this.chkScreenRecordAutoStart, "chkScreenRecordAutoStart");
            this.chkScreenRecordAutoStart.Name = "chkScreenRecordAutoStart";
            this.chkScreenRecordAutoStart.UseVisualStyleBackColor = true;
            this.chkScreenRecordAutoStart.CheckedChanged += new System.EventHandler(this.chkScreenRecordAutoStart_CheckedChanged);
            // 
            // cbScreenRecordAutoDisableAero
            // 
            resources.ApplyResources(this.cbScreenRecordAutoDisableAero, "cbScreenRecordAutoDisableAero");
            this.cbScreenRecordAutoDisableAero.Name = "cbScreenRecordAutoDisableAero";
            this.cbScreenRecordAutoDisableAero.UseVisualStyleBackColor = true;
            this.cbScreenRecordAutoDisableAero.CheckedChanged += new System.EventHandler(this.cbScreenRecordAutoDisableAero_CheckedChanged);
            // 
            // lblScreenRecorderFixedDuration
            // 
            resources.ApplyResources(this.lblScreenRecorderFixedDuration, "lblScreenRecorderFixedDuration");
            this.lblScreenRecorderFixedDuration.Name = "lblScreenRecorderFixedDuration";
            // 
            // nudScreenRecordFPS
            // 
            resources.ApplyResources(this.nudScreenRecordFPS, "nudScreenRecordFPS");
            this.nudScreenRecordFPS.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudScreenRecordFPS.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudScreenRecordFPS.Name = "nudScreenRecordFPS";
            this.nudScreenRecordFPS.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudScreenRecordFPS.ValueChanged += new System.EventHandler(this.nudScreenRecordFPS_ValueChanged);
            // 
            // lblScreenRecordFPS
            // 
            resources.ApplyResources(this.lblScreenRecordFPS, "lblScreenRecordFPS");
            this.lblScreenRecordFPS.Name = "lblScreenRecordFPS";
            // 
            // chkRunScreencastCLI
            // 
            resources.ApplyResources(this.chkRunScreencastCLI, "chkRunScreencastCLI");
            this.chkRunScreencastCLI.Name = "chkRunScreencastCLI";
            this.chkRunScreencastCLI.UseVisualStyleBackColor = true;
            this.chkRunScreencastCLI.CheckedChanged += new System.EventHandler(this.chkRunScreencastCLI_CheckedChanged);
            // 
            // btnScreenRecorderOptions
            // 
            resources.ApplyResources(this.btnScreenRecorderOptions, "btnScreenRecorderOptions");
            this.btnScreenRecorderOptions.Name = "btnScreenRecorderOptions";
            this.btnScreenRecorderOptions.UseVisualStyleBackColor = true;
            this.btnScreenRecorderOptions.Click += new System.EventHandler(this.btnScreenRecorderOptions_Click);
            // 
            // btnEncoderConfig
            // 
            resources.ApplyResources(this.btnEncoderConfig, "btnEncoderConfig");
            this.btnEncoderConfig.Name = "btnEncoderConfig";
            this.btnEncoderConfig.UseVisualStyleBackColor = true;
            this.btnEncoderConfig.Click += new System.EventHandler(this.btnEncoderConfig_Click);
            // 
            // cboEncoder
            // 
            this.cboEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEncoder.FormattingEnabled = true;
            resources.ApplyResources(this.cboEncoder, "cboEncoder");
            this.cboEncoder.Name = "cboEncoder";
            this.cboEncoder.SelectedIndexChanged += new System.EventHandler(this.cboEncoder_SelectedIndexChanged);
            // 
            // nudScreenRecorderDuration
            // 
            this.nudScreenRecorderDuration.DecimalPlaces = 1;
            this.nudScreenRecorderDuration.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudScreenRecorderDuration, "nudScreenRecorderDuration");
            this.nudScreenRecorderDuration.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudScreenRecorderDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScreenRecorderDuration.Name = "nudScreenRecorderDuration";
            this.nudScreenRecorderDuration.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudScreenRecorderDuration.ValueChanged += new System.EventHandler(this.nudScreenRecorderDuration_ValueChanged);
            // 
            // nudScreenRecorderStartDelay
            // 
            this.nudScreenRecorderStartDelay.DecimalPlaces = 1;
            this.nudScreenRecorderStartDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudScreenRecorderStartDelay, "nudScreenRecorderStartDelay");
            this.nudScreenRecorderStartDelay.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudScreenRecorderStartDelay.Name = "nudScreenRecorderStartDelay";
            this.nudScreenRecorderStartDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScreenRecorderStartDelay.ValueChanged += new System.EventHandler(this.nudScreenRecorderStartDelay_ValueChanged);
            // 
            // cbScreenRecorderOutput
            // 
            this.cbScreenRecorderOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScreenRecorderOutput.FormattingEnabled = true;
            resources.ApplyResources(this.cbScreenRecorderOutput, "cbScreenRecorderOutput");
            this.cbScreenRecorderOutput.Name = "cbScreenRecorderOutput";
            this.cbScreenRecorderOutput.SelectedIndexChanged += new System.EventHandler(this.cbScreenRecorderOutput_SelectedIndexChanged);
            // 
            // lblScreenRecorderOutput
            // 
            resources.ApplyResources(this.lblScreenRecorderOutput, "lblScreenRecorderOutput");
            this.lblScreenRecorderOutput.Name = "lblScreenRecorderOutput";
            // 
            // cbScreenRecorderFixedDuration
            // 
            resources.ApplyResources(this.cbScreenRecorderFixedDuration, "cbScreenRecorderFixedDuration");
            this.cbScreenRecorderFixedDuration.Name = "cbScreenRecorderFixedDuration";
            this.cbScreenRecorderFixedDuration.UseVisualStyleBackColor = true;
            this.cbScreenRecorderFixedDuration.CheckedChanged += new System.EventHandler(this.cbScreenRecorderFixedDuration_CheckedChanged);
            // 
            // nudGIFFPS
            // 
            resources.ApplyResources(this.nudGIFFPS, "nudGIFFPS");
            this.nudGIFFPS.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudGIFFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGIFFPS.Name = "nudGIFFPS";
            this.nudGIFFPS.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudGIFFPS.ValueChanged += new System.EventHandler(this.nudGIFFPS_ValueChanged);
            // 
            // lblGIFFPS
            // 
            resources.ApplyResources(this.lblGIFFPS, "lblGIFFPS");
            this.lblGIFFPS.Name = "lblGIFFPS";
            // 
            // tpRectangleAnnotate
            // 
            this.tpRectangleAnnotate.Controls.Add(this.pgRectangleAnnotate);
            resources.ApplyResources(this.tpRectangleAnnotate, "tpRectangleAnnotate");
            this.tpRectangleAnnotate.Name = "tpRectangleAnnotate";
            this.tpRectangleAnnotate.UseVisualStyleBackColor = true;
            // 
            // pgRectangleAnnotate
            // 
            resources.ApplyResources(this.pgRectangleAnnotate, "pgRectangleAnnotate");
            this.pgRectangleAnnotate.Name = "pgRectangleAnnotate";
            this.pgRectangleAnnotate.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgRectangleAnnotate.ToolbarVisible = false;
            // 
            // tpActions
            // 
            this.tpActions.Controls.Add(this.pActions);
            this.tpActions.Controls.Add(this.chkUseDefaultActions);
            resources.ApplyResources(this.tpActions, "tpActions");
            this.tpActions.Name = "tpActions";
            this.tpActions.UseVisualStyleBackColor = true;
            // 
            // pActions
            // 
            this.pActions.Controls.Add(this.btnActionsDuplicate);
            this.pActions.Controls.Add(this.btnActionsAdd);
            this.pActions.Controls.Add(this.lvActions);
            this.pActions.Controls.Add(this.btnActionsEdit);
            this.pActions.Controls.Add(this.btnActionsRemove);
            resources.ApplyResources(this.pActions, "pActions");
            this.pActions.Name = "pActions";
            // 
            // btnActionsDuplicate
            // 
            resources.ApplyResources(this.btnActionsDuplicate, "btnActionsDuplicate");
            this.btnActionsDuplicate.Name = "btnActionsDuplicate";
            this.btnActionsDuplicate.UseVisualStyleBackColor = true;
            this.btnActionsDuplicate.Click += new System.EventHandler(this.btnActionsDuplicate_Click);
            // 
            // btnActionsAdd
            // 
            resources.ApplyResources(this.btnActionsAdd, "btnActionsAdd");
            this.btnActionsAdd.Name = "btnActionsAdd";
            this.btnActionsAdd.UseVisualStyleBackColor = true;
            this.btnActionsAdd.Click += new System.EventHandler(this.btnActionsAdd_Click);
            // 
            // lvActions
            // 
            resources.ApplyResources(this.lvActions, "lvActions");
            this.lvActions.AutoFillColumn = true;
            this.lvActions.CheckBoxes = true;
            this.lvActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chActionsName,
            this.chActionsPath,
            this.chActionsArgs,
            this.chActionsExtensions});
            this.lvActions.FullRowSelect = true;
            this.lvActions.MultiSelect = false;
            this.lvActions.Name = "lvActions";
            this.lvActions.UseCompatibleStateImageBehavior = false;
            this.lvActions.View = System.Windows.Forms.View.Details;
            this.lvActions.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvActions_ItemChecked);
            // 
            // chActionsName
            // 
            resources.ApplyResources(this.chActionsName, "chActionsName");
            // 
            // chActionsPath
            // 
            resources.ApplyResources(this.chActionsPath, "chActionsPath");
            // 
            // chActionsArgs
            // 
            resources.ApplyResources(this.chActionsArgs, "chActionsArgs");
            // 
            // chActionsExtensions
            // 
            resources.ApplyResources(this.chActionsExtensions, "chActionsExtensions");
            // 
            // btnActionsEdit
            // 
            resources.ApplyResources(this.btnActionsEdit, "btnActionsEdit");
            this.btnActionsEdit.Name = "btnActionsEdit";
            this.btnActionsEdit.UseVisualStyleBackColor = true;
            this.btnActionsEdit.Click += new System.EventHandler(this.btnActionsEdit_Click);
            // 
            // btnActionsRemove
            // 
            resources.ApplyResources(this.btnActionsRemove, "btnActionsRemove");
            this.btnActionsRemove.Name = "btnActionsRemove";
            this.btnActionsRemove.UseVisualStyleBackColor = true;
            this.btnActionsRemove.Click += new System.EventHandler(this.btnActionsRemove_Click);
            // 
            // chkUseDefaultActions
            // 
            resources.ApplyResources(this.chkUseDefaultActions, "chkUseDefaultActions");
            this.chkUseDefaultActions.Checked = true;
            this.chkUseDefaultActions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultActions.Name = "chkUseDefaultActions";
            this.chkUseDefaultActions.UseVisualStyleBackColor = true;
            this.chkUseDefaultActions.CheckedChanged += new System.EventHandler(this.chkUseDefaultActions_CheckedChanged);
            // 
            // tpWatchFolders
            // 
            this.tpWatchFolders.Controls.Add(this.cbWatchFolderEnabled);
            this.tpWatchFolders.Controls.Add(this.lvWatchFolderList);
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderRemove);
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderAdd);
            resources.ApplyResources(this.tpWatchFolders, "tpWatchFolders");
            this.tpWatchFolders.Name = "tpWatchFolders";
            this.tpWatchFolders.UseVisualStyleBackColor = true;
            // 
            // cbWatchFolderEnabled
            // 
            resources.ApplyResources(this.cbWatchFolderEnabled, "cbWatchFolderEnabled");
            this.cbWatchFolderEnabled.Name = "cbWatchFolderEnabled";
            this.cbWatchFolderEnabled.UseVisualStyleBackColor = true;
            this.cbWatchFolderEnabled.CheckedChanged += new System.EventHandler(this.cbWatchFolderEnabled_CheckedChanged);
            // 
            // lvWatchFolderList
            // 
            resources.ApplyResources(this.lvWatchFolderList, "lvWatchFolderList");
            this.lvWatchFolderList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chWatchFolderFolderPath,
            this.chWatchFolderFilter,
            this.chWatchFolderIncludeSubdirectories});
            this.lvWatchFolderList.FullRowSelect = true;
            this.lvWatchFolderList.Name = "lvWatchFolderList";
            this.lvWatchFolderList.UseCompatibleStateImageBehavior = false;
            this.lvWatchFolderList.View = System.Windows.Forms.View.Details;
            this.lvWatchFolderList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvWatchFolderList_MouseDoubleClick);
            // 
            // chWatchFolderFolderPath
            // 
            resources.ApplyResources(this.chWatchFolderFolderPath, "chWatchFolderFolderPath");
            // 
            // chWatchFolderFilter
            // 
            resources.ApplyResources(this.chWatchFolderFilter, "chWatchFolderFilter");
            // 
            // chWatchFolderIncludeSubdirectories
            // 
            resources.ApplyResources(this.chWatchFolderIncludeSubdirectories, "chWatchFolderIncludeSubdirectories");
            // 
            // btnWatchFolderRemove
            // 
            resources.ApplyResources(this.btnWatchFolderRemove, "btnWatchFolderRemove");
            this.btnWatchFolderRemove.Name = "btnWatchFolderRemove";
            this.btnWatchFolderRemove.UseVisualStyleBackColor = true;
            this.btnWatchFolderRemove.Click += new System.EventHandler(this.btnWatchFolderRemove_Click);
            // 
            // btnWatchFolderAdd
            // 
            resources.ApplyResources(this.btnWatchFolderAdd, "btnWatchFolderAdd");
            this.btnWatchFolderAdd.Name = "btnWatchFolderAdd";
            this.btnWatchFolderAdd.UseVisualStyleBackColor = true;
            this.btnWatchFolderAdd.Click += new System.EventHandler(this.btnWatchFolderAdd_Click);
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
            this.tcUpload.Controls.Add(this.tpUploadNamePattern);
            this.tcUpload.Controls.Add(this.tpUploadClipboard);
            resources.ApplyResources(this.tcUpload, "tcUpload");
            this.tcUpload.Name = "tcUpload";
            this.tcUpload.SelectedIndex = 0;
            // 
            // tpUploadNamePattern
            // 
            this.tpUploadNamePattern.Controls.Add(this.pUpload);
            this.tpUploadNamePattern.Controls.Add(this.chkUseDefaultUploadSettings);
            resources.ApplyResources(this.tpUploadNamePattern, "tpUploadNamePattern");
            this.tpUploadNamePattern.Name = "tpUploadNamePattern";
            this.tpUploadNamePattern.UseVisualStyleBackColor = true;
            // 
            // pUpload
            // 
            this.pUpload.Controls.Add(this.lblNameFormatPattern);
            this.pUpload.Controls.Add(this.cbFileUploadUseNamePattern);
            this.pUpload.Controls.Add(this.lblNameFormatPatternPreviewActiveWindow);
            this.pUpload.Controls.Add(this.lblNameFormatPatternPreview);
            this.pUpload.Controls.Add(this.txtNameFormatPatternActiveWindow);
            this.pUpload.Controls.Add(this.txtNameFormatPattern);
            this.pUpload.Controls.Add(this.btnResetAutoIncrementNumber);
            this.pUpload.Controls.Add(this.lblNameFormatPatternActiveWindow);
            resources.ApplyResources(this.pUpload, "pUpload");
            this.pUpload.Name = "pUpload";
            // 
            // lblNameFormatPattern
            // 
            resources.ApplyResources(this.lblNameFormatPattern, "lblNameFormatPattern");
            this.lblNameFormatPattern.Name = "lblNameFormatPattern";
            // 
            // cbFileUploadUseNamePattern
            // 
            resources.ApplyResources(this.cbFileUploadUseNamePattern, "cbFileUploadUseNamePattern");
            this.cbFileUploadUseNamePattern.Name = "cbFileUploadUseNamePattern";
            this.cbFileUploadUseNamePattern.UseVisualStyleBackColor = true;
            this.cbFileUploadUseNamePattern.CheckedChanged += new System.EventHandler(this.cbFileUploadUseNamePattern_CheckedChanged);
            // 
            // lblNameFormatPatternPreviewActiveWindow
            // 
            resources.ApplyResources(this.lblNameFormatPatternPreviewActiveWindow, "lblNameFormatPatternPreviewActiveWindow");
            this.lblNameFormatPatternPreviewActiveWindow.Name = "lblNameFormatPatternPreviewActiveWindow";
            // 
            // lblNameFormatPatternPreview
            // 
            resources.ApplyResources(this.lblNameFormatPatternPreview, "lblNameFormatPatternPreview");
            this.lblNameFormatPatternPreview.Name = "lblNameFormatPatternPreview";
            // 
            // txtNameFormatPatternActiveWindow
            // 
            resources.ApplyResources(this.txtNameFormatPatternActiveWindow, "txtNameFormatPatternActiveWindow");
            this.txtNameFormatPatternActiveWindow.Name = "txtNameFormatPatternActiveWindow";
            this.txtNameFormatPatternActiveWindow.TextChanged += new System.EventHandler(this.txtNameFormatPatternActiveWindow_TextChanged);
            // 
            // txtNameFormatPattern
            // 
            resources.ApplyResources(this.txtNameFormatPattern, "txtNameFormatPattern");
            this.txtNameFormatPattern.Name = "txtNameFormatPattern";
            this.txtNameFormatPattern.TextChanged += new System.EventHandler(this.txtNameFormatPattern_TextChanged);
            // 
            // btnResetAutoIncrementNumber
            // 
            resources.ApplyResources(this.btnResetAutoIncrementNumber, "btnResetAutoIncrementNumber");
            this.btnResetAutoIncrementNumber.Name = "btnResetAutoIncrementNumber";
            this.btnResetAutoIncrementNumber.UseVisualStyleBackColor = true;
            this.btnResetAutoIncrementNumber.Click += new System.EventHandler(this.btnResetAutoIncrementNumber_Click);
            // 
            // lblNameFormatPatternActiveWindow
            // 
            resources.ApplyResources(this.lblNameFormatPatternActiveWindow, "lblNameFormatPatternActiveWindow");
            this.lblNameFormatPatternActiveWindow.Name = "lblNameFormatPatternActiveWindow";
            // 
            // chkUseDefaultUploadSettings
            // 
            resources.ApplyResources(this.chkUseDefaultUploadSettings, "chkUseDefaultUploadSettings");
            this.chkUseDefaultUploadSettings.Checked = true;
            this.chkUseDefaultUploadSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultUploadSettings.Name = "chkUseDefaultUploadSettings";
            this.chkUseDefaultUploadSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultUploadSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultUploadSettings_CheckedChanged);
            // 
            // tpUploadClipboard
            // 
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadShareURL);
            this.tpUploadClipboard.Controls.Add(this.chkClipboardUploadURLContents);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadAutoIndexFolder);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadShortenURL);
            resources.ApplyResources(this.tpUploadClipboard, "tpUploadClipboard");
            this.tpUploadClipboard.Name = "tpUploadClipboard";
            this.tpUploadClipboard.UseVisualStyleBackColor = true;
            // 
            // cbClipboardUploadShareURL
            // 
            resources.ApplyResources(this.cbClipboardUploadShareURL, "cbClipboardUploadShareURL");
            this.cbClipboardUploadShareURL.Name = "cbClipboardUploadShareURL";
            this.cbClipboardUploadShareURL.UseVisualStyleBackColor = true;
            this.cbClipboardUploadShareURL.CheckedChanged += new System.EventHandler(this.cbClipboardUploadShareURL_CheckedChanged);
            // 
            // chkClipboardUploadURLContents
            // 
            resources.ApplyResources(this.chkClipboardUploadURLContents, "chkClipboardUploadURLContents");
            this.chkClipboardUploadURLContents.Name = "chkClipboardUploadURLContents";
            this.chkClipboardUploadURLContents.UseVisualStyleBackColor = true;
            this.chkClipboardUploadURLContents.CheckedChanged += new System.EventHandler(this.chkClipboardUploadContents_CheckedChanged);
            // 
            // cbClipboardUploadAutoIndexFolder
            // 
            resources.ApplyResources(this.cbClipboardUploadAutoIndexFolder, "cbClipboardUploadAutoIndexFolder");
            this.cbClipboardUploadAutoIndexFolder.Name = "cbClipboardUploadAutoIndexFolder";
            this.cbClipboardUploadAutoIndexFolder.UseVisualStyleBackColor = true;
            this.cbClipboardUploadAutoIndexFolder.CheckedChanged += new System.EventHandler(this.cbClipboardUploadAutoIndexFolder_CheckedChanged);
            // 
            // cbClipboardUploadShortenURL
            // 
            resources.ApplyResources(this.cbClipboardUploadShortenURL, "cbClipboardUploadShortenURL");
            this.cbClipboardUploadShortenURL.Name = "cbClipboardUploadShortenURL";
            this.cbClipboardUploadShortenURL.UseVisualStyleBackColor = true;
            this.cbClipboardUploadShortenURL.CheckedChanged += new System.EventHandler(this.cbClipboardUploadAutoDetectURL_CheckedChanged);
            // 
            // tpIndexer
            // 
            this.tpIndexer.Controls.Add(this.pgIndexerConfig);
            this.tpIndexer.Controls.Add(this.chkUseDefaultIndexerSettings);
            resources.ApplyResources(this.tpIndexer, "tpIndexer");
            this.tpIndexer.Name = "tpIndexer";
            this.tpIndexer.UseVisualStyleBackColor = true;
            // 
            // pgIndexerConfig
            // 
            resources.ApplyResources(this.pgIndexerConfig, "pgIndexerConfig");
            this.pgIndexerConfig.Name = "pgIndexerConfig";
            this.pgIndexerConfig.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgIndexerConfig.ToolbarVisible = false;
            // 
            // chkUseDefaultIndexerSettings
            // 
            resources.ApplyResources(this.chkUseDefaultIndexerSettings, "chkUseDefaultIndexerSettings");
            this.chkUseDefaultIndexerSettings.Checked = true;
            this.chkUseDefaultIndexerSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultIndexerSettings.Name = "chkUseDefaultIndexerSettings";
            this.chkUseDefaultIndexerSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultIndexerSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultIndexerSettings_CheckedChanged);
            // 
            // tpAdvanced
            // 
            this.tpAdvanced.Controls.Add(this.pgTaskSettings);
            this.tpAdvanced.Controls.Add(this.chkUseDefaultAdvancedSettings);
            resources.ApplyResources(this.tpAdvanced, "tpAdvanced");
            this.tpAdvanced.Name = "tpAdvanced";
            this.tpAdvanced.UseVisualStyleBackColor = true;
            // 
            // pgTaskSettings
            // 
            resources.ApplyResources(this.pgTaskSettings, "pgTaskSettings");
            this.pgTaskSettings.Name = "pgTaskSettings";
            this.pgTaskSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgTaskSettings.ToolbarVisible = false;
            // 
            // chkUseDefaultAdvancedSettings
            // 
            resources.ApplyResources(this.chkUseDefaultAdvancedSettings, "chkUseDefaultAdvancedSettings");
            this.chkUseDefaultAdvancedSettings.Checked = true;
            this.chkUseDefaultAdvancedSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultAdvancedSettings.Name = "chkUseDefaultAdvancedSettings";
            this.chkUseDefaultAdvancedSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultAdvancedSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultAdvancedSettings_CheckedChanged);
            // 
            // tttvMain
            // 
            resources.ApplyResources(this.tttvMain, "tttvMain");
            this.tttvMain.ImageList = null;
            this.tttvMain.MainTabControl = null;
            this.tttvMain.Name = "tttvMain";
            this.tttvMain.TreeViewFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tttvMain.TreeViewSize = 150;
            // 
            // TaskSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tcTaskSettings);
            this.Controls.Add(this.tttvMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TaskSettingsForm";
            this.Resize += new System.EventHandler(this.TaskSettingsForm_Resize);
            this.tcTaskSettings.ResumeLayout(false);
            this.tpTask.ResumeLayout(false);
            this.tpTask.PerformLayout();
            this.cmsDestinations.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.panelGeneral.ResumeLayout(false);
            this.panelGeneral.PerformLayout();
            this.tpImage.ResumeLayout(false);
            this.tcImage.ResumeLayout(false);
            this.tpQuality.ResumeLayout(false);
            this.tpQuality.PerformLayout();
            this.pImage.ResumeLayout(false);
            this.pImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUseImageFormat2After)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageJPEGQuality)).EndInit();
            this.tpEffects.ResumeLayout(false);
            this.tpEffects.PerformLayout();
            this.tpThumbnail.ResumeLayout(false);
            this.tpThumbnail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailWidth)).EndInit();
            this.tpCapture.ResumeLayout(false);
            this.tcCapture.ResumeLayout(false);
            this.tpCaptureGeneral.ResumeLayout(false);
            this.tpCaptureGeneral.PerformLayout();
            this.pCapture.ResumeLayout(false);
            this.pCapture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenshotDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureShadowOffset)).EndInit();
            this.tpRegionCapture.ResumeLayout(false);
            this.tpScreenRecorder.ResumeLayout(false);
            this.tpScreenRecorder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecordFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGIFFPS)).EndInit();
            this.tpRectangleAnnotate.ResumeLayout(false);
            this.tpActions.ResumeLayout(false);
            this.tpActions.PerformLayout();
            this.pActions.ResumeLayout(false);
            this.tpWatchFolders.ResumeLayout(false);
            this.tpWatchFolders.PerformLayout();
            this.tpUpload.ResumeLayout(false);
            this.tcUpload.ResumeLayout(false);
            this.tpUploadNamePattern.ResumeLayout(false);
            this.tpUploadNamePattern.PerformLayout();
            this.pUpload.ResumeLayout(false);
            this.pUpload.PerformLayout();
            this.tpUploadClipboard.ResumeLayout(false);
            this.tpUploadClipboard.PerformLayout();
            this.tpIndexer.ResumeLayout(false);
            this.tpIndexer.PerformLayout();
            this.tpAdvanced.ResumeLayout(false);
            this.tpAdvanced.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private HelpersLib.MenuButton btnAfterCapture;
        private System.Windows.Forms.ContextMenuStrip cmsAfterCapture;
        private HelpersLib.MenuButton btnAfterUpload;
        private HelpersLib.MenuButton btnDestinations;
        private System.Windows.Forms.ContextMenuStrip cmsAfterUpload;
        private System.Windows.Forms.CheckBox cbUseDefaultAfterCaptureSettings;
        private System.Windows.Forms.CheckBox cbUseDefaultAfterUploadSettings;
        private System.Windows.Forms.CheckBox cbUseDefaultDestinationSettings;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox tbDescription;
        private HelpersLib.MenuButton btnTask;
        private System.Windows.Forms.ContextMenuStrip cmsTask;
        private System.Windows.Forms.TabControl tcTaskSettings;
        private System.Windows.Forms.TabPage tpImage;
        private System.Windows.Forms.TabPage tpCapture;
        private System.Windows.Forms.TabControl tcImage;
        private System.Windows.Forms.TabPage tpQuality;
        private System.Windows.Forms.Label lblImageFormat;
        private System.Windows.Forms.Label lblUseImageFormat2AfterHint;
        private System.Windows.Forms.ComboBox cbImageFormat;
        private System.Windows.Forms.Label lblImageJPEGQualityHint;
        private System.Windows.Forms.Label lblImageJPEGQuality;
        private System.Windows.Forms.ComboBox cbImageGIFQuality;
        private System.Windows.Forms.Label lblImageGIFQuality;
        private System.Windows.Forms.ComboBox cbImageFormat2;
        private System.Windows.Forms.NumericUpDown nudImageJPEGQuality;
        private System.Windows.Forms.Label lblImageFormat2;
        private System.Windows.Forms.NumericUpDown nudUseImageFormat2After;
        private System.Windows.Forms.Label lblUseImageFormat2After;
        private System.Windows.Forms.TabPage tpEffects;
        private System.Windows.Forms.TabControl tcCapture;
        private System.Windows.Forms.TabPage tpCaptureGeneral;
        private System.Windows.Forms.CheckBox cbCaptureAutoHideTaskbar;
        private System.Windows.Forms.Label lblScreenshotDelayInfo;
        private System.Windows.Forms.NumericUpDown nudScreenshotDelay;
        private System.Windows.Forms.CheckBox cbScreenshotDelay;
        private System.Windows.Forms.NumericUpDown nudCaptureShadowOffset;
        private System.Windows.Forms.CheckBox cbCaptureClientArea;
        private System.Windows.Forms.CheckBox cbCaptureShadow;
        private System.Windows.Forms.CheckBox cbShowCursor;
        private System.Windows.Forms.CheckBox cbCaptureTransparent;
        private System.Windows.Forms.TabPage tpRegionCapture;
        private System.Windows.Forms.TabPage tpScreenRecorder;
        private System.Windows.Forms.TabPage tpTask;
        private System.Windows.Forms.TabPage tpActions;
        private System.Windows.Forms.Button btnActionsEdit;
        private System.Windows.Forms.Button btnActionsRemove;
        private System.Windows.Forms.Button btnActionsAdd;
        private HelpersLib.MyListView lvActions;
        private System.Windows.Forms.ColumnHeader chActionsName;
        private System.Windows.Forms.ColumnHeader chActionsPath;
        private System.Windows.Forms.ColumnHeader chActionsArgs;
        private System.Windows.Forms.TabPage tpUpload;
        private System.Windows.Forms.TabControl tcUpload;
        private System.Windows.Forms.TabPage tpUploadNamePattern;
        private System.Windows.Forms.CheckBox cbFileUploadUseNamePattern;
        private System.Windows.Forms.Label lblNameFormatPattern;
        private System.Windows.Forms.TextBox txtNameFormatPatternActiveWindow;
        private System.Windows.Forms.Button btnResetAutoIncrementNumber;
        private System.Windows.Forms.Label lblNameFormatPatternActiveWindow;
        private System.Windows.Forms.TextBox txtNameFormatPattern;
        private System.Windows.Forms.Label lblNameFormatPatternPreview;
        private System.Windows.Forms.Label lblNameFormatPatternPreviewActiveWindow;
        private System.Windows.Forms.TabPage tpUploadClipboard;
        private System.Windows.Forms.CheckBox cbClipboardUploadShortenURL;
        private System.Windows.Forms.TabPage tpAdvanced;
        private System.Windows.Forms.PropertyGrid pgTaskSettings;
        private System.Windows.Forms.CheckBox chkUseDefaultImageSettings;
        private System.Windows.Forms.CheckBox chkUseDefaultCaptureSettings;
        private System.Windows.Forms.CheckBox chkUseDefaultActions;
        private System.Windows.Forms.CheckBox chkUseDefaultUploadSettings;
        private System.Windows.Forms.Panel pActions;
        private System.Windows.Forms.CheckBox chkUseDefaultAdvancedSettings;
        private System.Windows.Forms.CheckBox cbScreenRecorderFixedDuration;
        private System.Windows.Forms.NumericUpDown nudGIFFPS;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderDuration;
        private System.Windows.Forms.Label lblGIFFPS;
        private System.Windows.Forms.ComboBox cbScreenRecorderOutput;
        private System.Windows.Forms.Label lblScreenRecorderOutput;
        private System.Windows.Forms.TabPage tpWatchFolders;
        private System.Windows.Forms.CheckBox cbWatchFolderEnabled;
        private System.Windows.Forms.ListView lvWatchFolderList;
        private System.Windows.Forms.ColumnHeader chWatchFolderFolderPath;
        private System.Windows.Forms.ColumnHeader chWatchFolderFilter;
        private System.Windows.Forms.ColumnHeader chWatchFolderIncludeSubdirectories;
        private System.Windows.Forms.Button btnWatchFolderRemove;
        private System.Windows.Forms.Button btnWatchFolderAdd;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.CheckBox cbShowAfterCaptureTasksForm;
        private System.Windows.Forms.CheckBox cbHistorySave;
        private System.Windows.Forms.CheckBox cbPlaySoundAfterCapture;
        private System.Windows.Forms.CheckBox cbPlaySoundAfterUpload;
        private System.Windows.Forms.CheckBox chkShowAfterUploadForm;
        private System.Windows.Forms.CheckBox chkUseDefaultGeneralSettings;
        private System.Windows.Forms.Panel panelGeneral;
        private System.Windows.Forms.PropertyGrid pgRegionCapture;
        private System.Windows.Forms.TabPage tpIndexer;
        private System.Windows.Forms.PropertyGrid pgIndexerConfig;
        private System.Windows.Forms.CheckBox chkUseDefaultIndexerSettings;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderStartDelay;
        private System.Windows.Forms.Button btnImageEffects;
        private System.Windows.Forms.CheckBox cbImageEffectOnlyRegionCapture;
        private System.Windows.Forms.CheckBox chkShowImageEffectsWindowAfterCapture;
        private System.Windows.Forms.CheckBox chkOverrideFTP;
        private System.Windows.Forms.ComboBox cboFTPaccounts;
        private System.Windows.Forms.ComboBox cboPopUpNotification;
        private System.Windows.Forms.Label lblAfterTaskNotification;
        private System.Windows.Forms.ContextMenuStrip cmsDestinations;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiTextUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiURLShorteners;
        private System.Windows.Forms.ToolStripMenuItem tsmiURLSharingServices;
        private System.Windows.Forms.ComboBox cbImageFileExist;
        private System.Windows.Forms.Label lblImageFileExist;
        private System.Windows.Forms.ComboBox cboEncoder;
        private System.Windows.Forms.Button btnEncoderConfig;
        private System.Windows.Forms.TabPage tpThumbnail;
        private System.Windows.Forms.Label lblThumbnailHeight;
        private System.Windows.Forms.Label lblThumbnailWidth;
        private System.Windows.Forms.NumericUpDown nudThumbnailHeight;
        private System.Windows.Forms.NumericUpDown nudThumbnailWidth;
        private System.Windows.Forms.Label lblThumbnailName;
        private System.Windows.Forms.TextBox txtThumbnailName;
        private System.Windows.Forms.Label lblThumbnailNamePreview;
        private System.Windows.Forms.CheckBox cbThumbnailIfSmaller;
        private System.Windows.Forms.CheckBox cbClipboardUploadAutoIndexFolder;
        private System.Windows.Forms.Button btnScreenRecorderOptions;
        private System.Windows.Forms.CheckBox chkRunScreencastCLI;
        private System.Windows.Forms.CheckBox chkClipboardUploadURLContents;
        private System.Windows.Forms.NumericUpDown nudScreenRecordFPS;
        private System.Windows.Forms.Label lblScreenRecordFPS;
        private System.Windows.Forms.CheckBox chkShowBeforeUploadForm;
        private System.Windows.Forms.Label lblScreenRecorderFixedDuration;
        private System.Windows.Forms.CheckBox cbScreenRecordAutoDisableAero;
        private System.Windows.Forms.CheckBox cbClipboardUploadShareURL;
        private System.Windows.Forms.TabPage tpRectangleAnnotate;
        private System.Windows.Forms.PropertyGrid pgRectangleAnnotate;
        private System.Windows.Forms.ColumnHeader chActionsExtensions;
        private System.Windows.Forms.Button btnActionsDuplicate;
        private System.Windows.Forms.Label lblImageEffectsNote;
        private System.Windows.Forms.Label lblCaptureShadowOffset;
        private System.Windows.Forms.CheckBox chkScreenRecordAutoStart;
        private System.Windows.Forms.Label lblScreenRecorderStartDelay;
        private HelpersLib.TabToTreeView tttvMain;
        private System.Windows.Forms.Panel pImage;
        private System.Windows.Forms.Panel pCapture;
        private System.Windows.Forms.Panel pUpload;
        private System.Windows.Forms.ComboBox cbOverrideCustomUploader;
        private System.Windows.Forms.CheckBox chkOverrideCustomUploader;



    }
}