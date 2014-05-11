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
            this.cmsAfterCapture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsAfterUpload = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cbUseDefaultAfterCaptureSettings = new System.Windows.Forms.CheckBox();
            this.cbUseDefaultAfterUploadSettings = new System.Windows.Forms.CheckBox();
            this.cbUseDefaultDestinationSettings = new System.Windows.Forms.CheckBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.cmsTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tcHotkeySettings = new System.Windows.Forms.TabControl();
            this.tpTask = new System.Windows.Forms.TabPage();
            this.chkOverrideFTP = new System.Windows.Forms.CheckBox();
            this.cboFTPaccounts = new System.Windows.Forms.ComboBox();
            this.btnAfterCapture = new HelpersLib.MenuButton();
            this.btnAfterUpload = new HelpersLib.MenuButton();
            this.btnDestinations = new HelpersLib.MenuButton();
            this.cmsDestinations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSocialServices = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTask = new HelpersLib.MenuButton();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.panelGeneral = new System.Windows.Forms.Panel();
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
            this.cbImageFileExist = new System.Windows.Forms.ComboBox();
            this.lblImageFileExist = new System.Windows.Forms.Label();
            this.lblImageFormat = new System.Windows.Forms.Label();
            this.lblUseImageFormat2AfterHint = new System.Windows.Forms.Label();
            this.cbImageFormat = new System.Windows.Forms.ComboBox();
            this.lblImageJPEGQualityHint = new System.Windows.Forms.Label();
            this.lblImageJPEGQuality = new System.Windows.Forms.Label();
            this.cbImageGIFQuality = new System.Windows.Forms.ComboBox();
            this.lblImageGIFQuality = new System.Windows.Forms.Label();
            this.cbImageFormat2 = new System.Windows.Forms.ComboBox();
            this.nudImageJPEGQuality = new System.Windows.Forms.NumericUpDown();
            this.lblImageFormat2 = new System.Windows.Forms.Label();
            this.nudUseImageFormat2After = new System.Windows.Forms.NumericUpDown();
            this.lblUseImageFormat2After = new System.Windows.Forms.Label();
            this.tpEffects = new System.Windows.Forms.TabPage();
            this.gbImageEffects = new System.Windows.Forms.GroupBox();
            this.chkShowImageEffectsWindowAfterCapture = new System.Windows.Forms.CheckBox();
            this.cbImageEffectOnlyRegionCapture = new System.Windows.Forms.CheckBox();
            this.btnImageEffects = new System.Windows.Forms.Button();
            this.btnWatermarkSettings = new System.Windows.Forms.Button();
            this.tpThumbnail = new System.Windows.Forms.TabPage();
            this.cbThumbnailIfSmaller = new System.Windows.Forms.CheckBox();
            this.lblThumbnailNamePreview = new System.Windows.Forms.Label();
            this.lblThumbnailName = new System.Windows.Forms.Label();
            this.txtThumbnailName = new System.Windows.Forms.TextBox();
            this.lblThumbnailHeight = new System.Windows.Forms.Label();
            this.lblThumbnailWidth = new System.Windows.Forms.Label();
            this.nudThumbnailHeight = new System.Windows.Forms.NumericUpDown();
            this.nudThumbnailWidth = new System.Windows.Forms.NumericUpDown();
            this.chkUseDefaultImageSettings = new System.Windows.Forms.CheckBox();
            this.tpCapture = new System.Windows.Forms.TabPage();
            this.tcCapture = new System.Windows.Forms.TabControl();
            this.tpCaptureGeneral = new System.Windows.Forms.TabPage();
            this.cbCaptureAutoHideTaskbar = new System.Windows.Forms.CheckBox();
            this.lblScreenshotDelayInfo = new System.Windows.Forms.Label();
            this.nudScreenshotDelay = new System.Windows.Forms.NumericUpDown();
            this.cbScreenshotDelay = new System.Windows.Forms.CheckBox();
            this.nudCaptureShadowOffset = new System.Windows.Forms.NumericUpDown();
            this.cbCaptureClientArea = new System.Windows.Forms.CheckBox();
            this.cbCaptureShadow = new System.Windows.Forms.CheckBox();
            this.cbShowCursor = new System.Windows.Forms.CheckBox();
            this.cbCaptureTransparent = new System.Windows.Forms.CheckBox();
            this.tpCaptureShape = new System.Windows.Forms.TabPage();
            this.pgShapesCapture = new System.Windows.Forms.PropertyGrid();
            this.tpScreenRecorder = new System.Windows.Forms.TabPage();
            this.chkRunScreencastCLI = new System.Windows.Forms.CheckBox();
            this.lblScreenRecorderCLI = new System.Windows.Forms.Label();
            this.btnScreenRecorderOptions = new System.Windows.Forms.Button();
            this.btnEncoderConfig = new System.Windows.Forms.Button();
            this.cboEncoder = new System.Windows.Forms.ComboBox();
            this.nudScreenRecorderDuration = new System.Windows.Forms.NumericUpDown();
            this.lblScreenRecorderStartDelay = new System.Windows.Forms.Label();
            this.nudScreenRecorderStartDelay = new System.Windows.Forms.NumericUpDown();
            this.cbScreenRecorderOutput = new System.Windows.Forms.ComboBox();
            this.lblScreenRecorderOutput = new System.Windows.Forms.Label();
            this.cbScreenRecorderFixedDuration = new System.Windows.Forms.CheckBox();
            this.nudScreenRecorderFPS = new System.Windows.Forms.NumericUpDown();
            this.lblScreenRecorderFPS = new System.Windows.Forms.Label();
            this.chkUseDefaultCaptureSettings = new System.Windows.Forms.CheckBox();
            this.tpActions = new System.Windows.Forms.TabPage();
            this.pActions = new System.Windows.Forms.Panel();
            this.btnActionsAdd = new System.Windows.Forms.Button();
            this.lvActions = new HelpersLib.MyListView();
            this.chActionsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsArgs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.cbFileUploadUseNamePattern = new System.Windows.Forms.CheckBox();
            this.lblNameFormatPattern = new System.Windows.Forms.Label();
            this.txtNameFormatPatternActiveWindow = new System.Windows.Forms.TextBox();
            this.btnResetAutoIncrementNumber = new System.Windows.Forms.Button();
            this.lblNameFormatPatternActiveWindow = new System.Windows.Forms.Label();
            this.txtNameFormatPattern = new System.Windows.Forms.TextBox();
            this.lblNameFormatPatternPreview = new System.Windows.Forms.Label();
            this.lblNameFormatPatternPreviewActiveWindow = new System.Windows.Forms.Label();
            this.tpUploadClipboard = new System.Windows.Forms.TabPage();
            this.chkClipboardUploadContents = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadAutoIndexFolder = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadAutoDetectURL = new System.Windows.Forms.CheckBox();
            this.chkUseDefaultUploadSettings = new System.Windows.Forms.CheckBox();
            this.tpIndexer = new System.Windows.Forms.TabPage();
            this.pgIndexerConfig = new System.Windows.Forms.PropertyGrid();
            this.chkUseDefaultIndexerSettings = new System.Windows.Forms.CheckBox();
            this.tpAdvanced = new System.Windows.Forms.TabPage();
            this.pgTaskSettings = new System.Windows.Forms.PropertyGrid();
            this.chkUseDefaultAdvancedSettings = new System.Windows.Forms.CheckBox();
            this.ttTaskSettings = new System.Windows.Forms.ToolTip(this.components);
            this.tcHotkeySettings.SuspendLayout();
            this.tpTask.SuspendLayout();
            this.cmsDestinations.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.tpImage.SuspendLayout();
            this.tcImage.SuspendLayout();
            this.tpQuality.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageJPEGQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUseImageFormat2After)).BeginInit();
            this.tpEffects.SuspendLayout();
            this.gbImageEffects.SuspendLayout();
            this.tpThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailWidth)).BeginInit();
            this.tpCapture.SuspendLayout();
            this.tcCapture.SuspendLayout();
            this.tpCaptureGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenshotDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureShadowOffset)).BeginInit();
            this.tpCaptureShape.SuspendLayout();
            this.tpScreenRecorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderFPS)).BeginInit();
            this.tpActions.SuspendLayout();
            this.pActions.SuspendLayout();
            this.tpWatchFolders.SuspendLayout();
            this.tpUpload.SuspendLayout();
            this.tcUpload.SuspendLayout();
            this.tpUploadNamePattern.SuspendLayout();
            this.tpUploadClipboard.SuspendLayout();
            this.tpIndexer.SuspendLayout();
            this.tpAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsAfterCapture
            // 
            this.cmsAfterCapture.Name = "cmsAfterCapture";
            this.cmsAfterCapture.Size = new System.Drawing.Size(61, 4);
            // 
            // cmsAfterUpload
            // 
            this.cmsAfterUpload.Name = "cmsAfterCapture";
            this.cmsAfterUpload.Size = new System.Drawing.Size(61, 4);
            // 
            // cbUseDefaultAfterCaptureSettings
            // 
            this.cbUseDefaultAfterCaptureSettings.AutoSize = true;
            this.cbUseDefaultAfterCaptureSettings.Location = new System.Drawing.Point(6, 70);
            this.cbUseDefaultAfterCaptureSettings.Name = "cbUseDefaultAfterCaptureSettings";
            this.cbUseDefaultAfterCaptureSettings.Size = new System.Drawing.Size(222, 17);
            this.cbUseDefaultAfterCaptureSettings.TabIndex = 3;
            this.cbUseDefaultAfterCaptureSettings.Text = "Use main window \"After capture\" settings";
            this.cbUseDefaultAfterCaptureSettings.UseVisualStyleBackColor = true;
            this.cbUseDefaultAfterCaptureSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAfterCaptureSettings_CheckedChanged);
            // 
            // cbUseDefaultAfterUploadSettings
            // 
            this.cbUseDefaultAfterUploadSettings.AutoSize = true;
            this.cbUseDefaultAfterUploadSettings.Location = new System.Drawing.Point(6, 126);
            this.cbUseDefaultAfterUploadSettings.Name = "cbUseDefaultAfterUploadSettings";
            this.cbUseDefaultAfterUploadSettings.Size = new System.Drawing.Size(218, 17);
            this.cbUseDefaultAfterUploadSettings.TabIndex = 5;
            this.cbUseDefaultAfterUploadSettings.Text = "Use main window \"After upload\" settings";
            this.cbUseDefaultAfterUploadSettings.UseVisualStyleBackColor = true;
            this.cbUseDefaultAfterUploadSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAfterUploadSettings_CheckedChanged);
            // 
            // cbUseDefaultDestinationSettings
            // 
            this.cbUseDefaultDestinationSettings.AutoSize = true;
            this.cbUseDefaultDestinationSettings.Location = new System.Drawing.Point(6, 182);
            this.cbUseDefaultDestinationSettings.Name = "cbUseDefaultDestinationSettings";
            this.cbUseDefaultDestinationSettings.Size = new System.Drawing.Size(214, 17);
            this.cbUseDefaultDestinationSettings.TabIndex = 7;
            this.cbUseDefaultDestinationSettings.Text = "Use main window \"Destination\" settings";
            this.cbUseDefaultDestinationSettings.UseVisualStyleBackColor = true;
            this.cbUseDefaultDestinationSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultDestinationSettings_CheckedChanged);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 13);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Description:";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(78, 9);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(434, 20);
            this.tbDescription.TabIndex = 1;
            this.tbDescription.TextChanged += new System.EventHandler(this.tbDescription_TextChanged);
            // 
            // cmsTask
            // 
            this.cmsTask.Name = "cmsAfterCapture";
            this.cmsTask.Size = new System.Drawing.Size(61, 4);
            // 
            // tcHotkeySettings
            // 
            this.tcHotkeySettings.Controls.Add(this.tpTask);
            this.tcHotkeySettings.Controls.Add(this.tpGeneral);
            this.tcHotkeySettings.Controls.Add(this.tpImage);
            this.tcHotkeySettings.Controls.Add(this.tpCapture);
            this.tcHotkeySettings.Controls.Add(this.tpActions);
            this.tcHotkeySettings.Controls.Add(this.tpWatchFolders);
            this.tcHotkeySettings.Controls.Add(this.tpUpload);
            this.tcHotkeySettings.Controls.Add(this.tpIndexer);
            this.tcHotkeySettings.Controls.Add(this.tpAdvanced);
            this.tcHotkeySettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcHotkeySettings.Location = new System.Drawing.Point(3, 3);
            this.tcHotkeySettings.Name = "tcHotkeySettings";
            this.tcHotkeySettings.SelectedIndex = 0;
            this.tcHotkeySettings.Size = new System.Drawing.Size(558, 386);
            this.tcHotkeySettings.TabIndex = 0;
            // 
            // tpTask
            // 
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
            this.tpTask.Location = new System.Drawing.Point(4, 22);
            this.tpTask.Name = "tpTask";
            this.tpTask.Padding = new System.Windows.Forms.Padding(3);
            this.tpTask.Size = new System.Drawing.Size(550, 360);
            this.tpTask.TabIndex = 0;
            this.tpTask.Text = "Task";
            this.tpTask.UseVisualStyleBackColor = true;
            // 
            // chkOverrideFTP
            // 
            this.chkOverrideFTP.AutoSize = true;
            this.chkOverrideFTP.Location = new System.Drawing.Point(6, 240);
            this.chkOverrideFTP.Name = "chkOverrideFTP";
            this.chkOverrideFTP.Size = new System.Drawing.Size(169, 17);
            this.chkOverrideFTP.TabIndex = 14;
            this.chkOverrideFTP.Text = "Override default FTP account:";
            this.chkOverrideFTP.UseVisualStyleBackColor = true;
            this.chkOverrideFTP.CheckedChanged += new System.EventHandler(this.chkOverrideFTP_CheckedChanged);
            // 
            // cboFTPaccounts
            // 
            this.cboFTPaccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFTPaccounts.Enabled = false;
            this.cboFTPaccounts.FormattingEnabled = true;
            this.cboFTPaccounts.Location = new System.Drawing.Point(181, 238);
            this.cboFTPaccounts.Name = "cboFTPaccounts";
            this.cboFTPaccounts.Size = new System.Drawing.Size(330, 21);
            this.cboFTPaccounts.TabIndex = 13;
            this.cboFTPaccounts.SelectedIndexChanged += new System.EventHandler(this.cboFTPaccounts_SelectedIndexChanged);
            // 
            // btnAfterCapture
            // 
            this.btnAfterCapture.Location = new System.Drawing.Point(6, 93);
            this.btnAfterCapture.Menu = this.cmsAfterCapture;
            this.btnAfterCapture.Name = "btnAfterCapture";
            this.btnAfterCapture.Size = new System.Drawing.Size(506, 23);
            this.btnAfterCapture.TabIndex = 4;
            this.btnAfterCapture.Text = "After capture...";
            this.btnAfterCapture.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAfterCapture.UseMnemonic = false;
            this.btnAfterCapture.UseVisualStyleBackColor = true;
            // 
            // btnAfterUpload
            // 
            this.btnAfterUpload.Location = new System.Drawing.Point(6, 149);
            this.btnAfterUpload.Menu = this.cmsAfterUpload;
            this.btnAfterUpload.Name = "btnAfterUpload";
            this.btnAfterUpload.Size = new System.Drawing.Size(506, 23);
            this.btnAfterUpload.TabIndex = 6;
            this.btnAfterUpload.Text = "After upload...";
            this.btnAfterUpload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAfterUpload.UseMnemonic = false;
            this.btnAfterUpload.UseVisualStyleBackColor = true;
            // 
            // btnDestinations
            // 
            this.btnDestinations.Location = new System.Drawing.Point(6, 205);
            this.btnDestinations.Menu = this.cmsDestinations;
            this.btnDestinations.Name = "btnDestinations";
            this.btnDestinations.Size = new System.Drawing.Size(506, 23);
            this.btnDestinations.TabIndex = 8;
            this.btnDestinations.Text = "Destinations...";
            this.btnDestinations.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.tsmiSocialServices});
            this.cmsDestinations.Name = "cmsDestinations";
            this.cmsDestinations.Size = new System.Drawing.Size(213, 114);
            // 
            // tsmiImageUploaders
            // 
            this.tsmiImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiImageUploaders.Name = "tsmiImageUploaders";
            this.tsmiImageUploaders.Size = new System.Drawing.Size(212, 22);
            this.tsmiImageUploaders.Text = "Image uploaders";
            // 
            // tsmiTextUploaders
            // 
            this.tsmiTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTextUploaders.Name = "tsmiTextUploaders";
            this.tsmiTextUploaders.Size = new System.Drawing.Size(212, 22);
            this.tsmiTextUploaders.Text = "Text uploaders";
            // 
            // tsmiFileUploaders
            // 
            this.tsmiFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiFileUploaders.Name = "tsmiFileUploaders";
            this.tsmiFileUploaders.Size = new System.Drawing.Size(212, 22);
            this.tsmiFileUploaders.Text = "File uploaders";
            // 
            // tsmiURLShorteners
            // 
            this.tsmiURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiURLShorteners.Name = "tsmiURLShorteners";
            this.tsmiURLShorteners.Size = new System.Drawing.Size(212, 22);
            this.tsmiURLShorteners.Text = "URL shorteners";
            // 
            // tsmiSocialServices
            // 
            this.tsmiSocialServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiSocialServices.Name = "tsmiSocialServices";
            this.tsmiSocialServices.Size = new System.Drawing.Size(212, 22);
            this.tsmiSocialServices.Text = "Social networking services";
            // 
            // btnTask
            // 
            this.btnTask.Location = new System.Drawing.Point(6, 37);
            this.btnTask.Menu = this.cmsTask;
            this.btnTask.Name = "btnTask";
            this.btnTask.Size = new System.Drawing.Size(506, 23);
            this.btnTask.TabIndex = 2;
            this.btnTask.Text = "Task...";
            this.btnTask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTask.UseMnemonic = false;
            this.btnTask.UseVisualStyleBackColor = true;
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.panelGeneral);
            this.tpGeneral.Controls.Add(this.chkUseDefaultGeneralSettings);
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpGeneral.Size = new System.Drawing.Size(550, 360);
            this.tpGeneral.TabIndex = 7;
            this.tpGeneral.Text = "General";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // panelGeneral
            // 
            this.panelGeneral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGeneral.Controls.Add(this.lblAfterTaskNotification);
            this.panelGeneral.Controls.Add(this.cboPopUpNotification);
            this.panelGeneral.Controls.Add(this.chkShowAfterUploadForm);
            this.panelGeneral.Controls.Add(this.cbShowAfterCaptureTasksForm);
            this.panelGeneral.Controls.Add(this.cbPlaySoundAfterUpload);
            this.panelGeneral.Controls.Add(this.cbHistorySave);
            this.panelGeneral.Controls.Add(this.cbPlaySoundAfterCapture);
            this.panelGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGeneral.Location = new System.Drawing.Point(3, 30);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Size = new System.Drawing.Size(544, 327);
            this.panelGeneral.TabIndex = 19;
            // 
            // lblAfterTaskNotification
            // 
            this.lblAfterTaskNotification.AutoSize = true;
            this.lblAfterTaskNotification.Location = new System.Drawing.Point(8, 84);
            this.lblAfterTaskNotification.Name = "lblAfterTaskNotification";
            this.lblAfterTaskNotification.Size = new System.Drawing.Size(117, 13);
            this.lblAfterTaskNotification.TabIndex = 21;
            this.lblAfterTaskNotification.Text = "After task is completed:";
            // 
            // cboPopUpNotification
            // 
            this.cboPopUpNotification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPopUpNotification.FormattingEnabled = true;
            this.cboPopUpNotification.Location = new System.Drawing.Point(128, 80);
            this.cboPopUpNotification.Name = "cboPopUpNotification";
            this.cboPopUpNotification.Size = new System.Drawing.Size(224, 21);
            this.cboPopUpNotification.TabIndex = 20;
            this.cboPopUpNotification.SelectedIndexChanged += new System.EventHandler(this.cboPopUpNotification_SelectedIndexChanged);
            // 
            // chkShowAfterUploadForm
            // 
            this.chkShowAfterUploadForm.AutoSize = true;
            this.chkShowAfterUploadForm.Location = new System.Drawing.Point(8, 112);
            this.chkShowAfterUploadForm.Name = "chkShowAfterUploadForm";
            this.chkShowAfterUploadForm.Size = new System.Drawing.Size(271, 17);
            this.chkShowAfterUploadForm.TabIndex = 18;
            this.chkShowAfterUploadForm.Text = "Show \"After upload\" window after task is completed";
            this.chkShowAfterUploadForm.UseVisualStyleBackColor = true;
            this.chkShowAfterUploadForm.CheckedChanged += new System.EventHandler(this.chkShowAfterUploadForm_CheckedChanged);
            // 
            // cbShowAfterCaptureTasksForm
            // 
            this.cbShowAfterCaptureTasksForm.AutoSize = true;
            this.cbShowAfterCaptureTasksForm.Location = new System.Drawing.Point(8, 32);
            this.cbShowAfterCaptureTasksForm.Name = "cbShowAfterCaptureTasksForm";
            this.cbShowAfterCaptureTasksForm.Size = new System.Drawing.Size(295, 17);
            this.cbShowAfterCaptureTasksForm.TabIndex = 13;
            this.cbShowAfterCaptureTasksForm.Text = "Show \"Post capture tasks\" window after capture is made";
            this.cbShowAfterCaptureTasksForm.UseVisualStyleBackColor = true;
            this.cbShowAfterCaptureTasksForm.CheckedChanged += new System.EventHandler(this.cbShowAfterCaptureTasksForm_CheckedChanged);
            // 
            // cbPlaySoundAfterUpload
            // 
            this.cbPlaySoundAfterUpload.AutoSize = true;
            this.cbPlaySoundAfterUpload.Location = new System.Drawing.Point(8, 56);
            this.cbPlaySoundAfterUpload.Name = "cbPlaySoundAfterUpload";
            this.cbPlaySoundAfterUpload.Size = new System.Drawing.Size(187, 17);
            this.cbPlaySoundAfterUpload.TabIndex = 15;
            this.cbPlaySoundAfterUpload.Text = "Play sound after task is completed";
            this.cbPlaySoundAfterUpload.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterUpload.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterUpload_CheckedChanged);
            // 
            // cbHistorySave
            // 
            this.cbHistorySave.AutoSize = true;
            this.cbHistorySave.Location = new System.Drawing.Point(8, 136);
            this.cbHistorySave.Name = "cbHistorySave";
            this.cbHistorySave.Size = new System.Drawing.Size(139, 17);
            this.cbHistorySave.TabIndex = 17;
            this.cbHistorySave.Text = "Save task info to history";
            this.cbHistorySave.UseVisualStyleBackColor = true;
            this.cbHistorySave.CheckedChanged += new System.EventHandler(this.cbHistorySave_CheckedChanged);
            // 
            // cbPlaySoundAfterCapture
            // 
            this.cbPlaySoundAfterCapture.AutoSize = true;
            this.cbPlaySoundAfterCapture.Location = new System.Drawing.Point(8, 8);
            this.cbPlaySoundAfterCapture.Name = "cbPlaySoundAfterCapture";
            this.cbPlaySoundAfterCapture.Size = new System.Drawing.Size(180, 17);
            this.cbPlaySoundAfterCapture.TabIndex = 14;
            this.cbPlaySoundAfterCapture.Text = "Play sound after capture is made";
            this.cbPlaySoundAfterCapture.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterCapture.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterCapture_CheckedChanged);
            // 
            // chkUseDefaultGeneralSettings
            // 
            this.chkUseDefaultGeneralSettings.AutoSize = true;
            this.chkUseDefaultGeneralSettings.Checked = true;
            this.chkUseDefaultGeneralSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultGeneralSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultGeneralSettings.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultGeneralSettings.Name = "chkUseDefaultGeneralSettings";
            this.chkUseDefaultGeneralSettings.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultGeneralSettings.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultGeneralSettings.TabIndex = 20;
            this.chkUseDefaultGeneralSettings.Text = "Use general settings in main window task settings";
            this.chkUseDefaultGeneralSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultGeneralSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultGeneralSettings_CheckedChanged);
            // 
            // tpImage
            // 
            this.tpImage.Controls.Add(this.tcImage);
            this.tpImage.Controls.Add(this.chkUseDefaultImageSettings);
            this.tpImage.Location = new System.Drawing.Point(4, 22);
            this.tpImage.Name = "tpImage";
            this.tpImage.Padding = new System.Windows.Forms.Padding(3);
            this.tpImage.Size = new System.Drawing.Size(550, 360);
            this.tpImage.TabIndex = 1;
            this.tpImage.Text = "Image";
            this.tpImage.UseVisualStyleBackColor = true;
            // 
            // tcImage
            // 
            this.tcImage.Controls.Add(this.tpQuality);
            this.tcImage.Controls.Add(this.tpEffects);
            this.tcImage.Controls.Add(this.tpThumbnail);
            this.tcImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcImage.Location = new System.Drawing.Point(3, 30);
            this.tcImage.Name = "tcImage";
            this.tcImage.SelectedIndex = 0;
            this.tcImage.Size = new System.Drawing.Size(544, 327);
            this.tcImage.TabIndex = 1;
            // 
            // tpQuality
            // 
            this.tpQuality.Controls.Add(this.cbImageFileExist);
            this.tpQuality.Controls.Add(this.lblImageFileExist);
            this.tpQuality.Controls.Add(this.lblImageFormat);
            this.tpQuality.Controls.Add(this.lblUseImageFormat2AfterHint);
            this.tpQuality.Controls.Add(this.cbImageFormat);
            this.tpQuality.Controls.Add(this.lblImageJPEGQualityHint);
            this.tpQuality.Controls.Add(this.lblImageJPEGQuality);
            this.tpQuality.Controls.Add(this.cbImageGIFQuality);
            this.tpQuality.Controls.Add(this.lblImageGIFQuality);
            this.tpQuality.Controls.Add(this.cbImageFormat2);
            this.tpQuality.Controls.Add(this.nudImageJPEGQuality);
            this.tpQuality.Controls.Add(this.lblImageFormat2);
            this.tpQuality.Controls.Add(this.nudUseImageFormat2After);
            this.tpQuality.Controls.Add(this.lblUseImageFormat2After);
            this.tpQuality.Location = new System.Drawing.Point(4, 22);
            this.tpQuality.Name = "tpQuality";
            this.tpQuality.Padding = new System.Windows.Forms.Padding(3);
            this.tpQuality.Size = new System.Drawing.Size(536, 301);
            this.tpQuality.TabIndex = 0;
            this.tpQuality.Text = "General";
            this.tpQuality.UseVisualStyleBackColor = true;
            // 
            // cbImageFileExist
            // 
            this.cbImageFileExist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageFileExist.FormattingEnabled = true;
            this.cbImageFileExist.Location = new System.Drawing.Point(104, 172);
            this.cbImageFileExist.Name = "cbImageFileExist";
            this.cbImageFileExist.Size = new System.Drawing.Size(176, 21);
            this.cbImageFileExist.TabIndex = 13;
            this.cbImageFileExist.SelectedIndexChanged += new System.EventHandler(this.cbImageFileExist_SelectedIndexChanged);
            // 
            // lblImageFileExist
            // 
            this.lblImageFileExist.AutoSize = true;
            this.lblImageFileExist.Location = new System.Drawing.Point(16, 176);
            this.lblImageFileExist.Name = "lblImageFileExist";
            this.lblImageFileExist.Size = new System.Drawing.Size(56, 13);
            this.lblImageFileExist.TabIndex = 12;
            this.lblImageFileExist.Text = "If file exist:";
            // 
            // lblImageFormat
            // 
            this.lblImageFormat.AutoSize = true;
            this.lblImageFormat.Location = new System.Drawing.Point(16, 16);
            this.lblImageFormat.Name = "lblImageFormat";
            this.lblImageFormat.Size = new System.Drawing.Size(71, 13);
            this.lblImageFormat.TabIndex = 0;
            this.lblImageFormat.Text = "Image format:";
            // 
            // lblUseImageFormat2AfterHint
            // 
            this.lblUseImageFormat2AfterHint.AutoSize = true;
            this.lblUseImageFormat2AfterHint.Location = new System.Drawing.Point(288, 112);
            this.lblUseImageFormat2AfterHint.Name = "lblUseImageFormat2AfterHint";
            this.lblUseImageFormat2AfterHint.Size = new System.Drawing.Size(121, 13);
            this.lblUseImageFormat2AfterHint.TabIndex = 9;
            this.lblUseImageFormat2AfterHint.Text = "kB  0 - 5000 (0 disables)";
            // 
            // cbImageFormat
            // 
            this.cbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageFormat.FormattingEnabled = true;
            this.cbImageFormat.Items.AddRange(new object[] {
            "PNG",
            "JPEG",
            "GIF",
            "BMP",
            "TIFF"});
            this.cbImageFormat.Location = new System.Drawing.Point(104, 12);
            this.cbImageFormat.Name = "cbImageFormat";
            this.cbImageFormat.Size = new System.Drawing.Size(56, 21);
            this.cbImageFormat.TabIndex = 1;
            this.cbImageFormat.SelectedIndexChanged += new System.EventHandler(this.cbImageFormat_SelectedIndexChanged);
            // 
            // lblImageJPEGQualityHint
            // 
            this.lblImageJPEGQualityHint.AutoSize = true;
            this.lblImageJPEGQualityHint.Location = new System.Drawing.Point(168, 48);
            this.lblImageJPEGQualityHint.Name = "lblImageJPEGQualityHint";
            this.lblImageJPEGQualityHint.Size = new System.Drawing.Size(40, 13);
            this.lblImageJPEGQualityHint.TabIndex = 4;
            this.lblImageJPEGQualityHint.Text = "0 - 100";
            // 
            // lblImageJPEGQuality
            // 
            this.lblImageJPEGQuality.AutoSize = true;
            this.lblImageJPEGQuality.Location = new System.Drawing.Point(16, 48);
            this.lblImageJPEGQuality.Name = "lblImageJPEGQuality";
            this.lblImageJPEGQuality.Size = new System.Drawing.Size(70, 13);
            this.lblImageJPEGQuality.TabIndex = 2;
            this.lblImageJPEGQuality.Text = "JPEG quality:";
            // 
            // cbImageGIFQuality
            // 
            this.cbImageGIFQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageGIFQuality.FormattingEnabled = true;
            this.cbImageGIFQuality.Items.AddRange(new object[] {
            "Default (Fast)",
            "256 colors (8 bit)",
            "16 colors (4 bit)",
            "Grayscale"});
            this.cbImageGIFQuality.Location = new System.Drawing.Point(104, 76);
            this.cbImageGIFQuality.Name = "cbImageGIFQuality";
            this.cbImageGIFQuality.Size = new System.Drawing.Size(120, 21);
            this.cbImageGIFQuality.TabIndex = 6;
            this.cbImageGIFQuality.SelectedIndexChanged += new System.EventHandler(this.cbImageGIFQuality_SelectedIndexChanged);
            // 
            // lblImageGIFQuality
            // 
            this.lblImageGIFQuality.AutoSize = true;
            this.lblImageGIFQuality.Location = new System.Drawing.Point(16, 80);
            this.lblImageGIFQuality.Name = "lblImageGIFQuality";
            this.lblImageGIFQuality.Size = new System.Drawing.Size(60, 13);
            this.lblImageGIFQuality.TabIndex = 5;
            this.lblImageGIFQuality.Text = "GIF quality:";
            // 
            // cbImageFormat2
            // 
            this.cbImageFormat2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageFormat2.FormattingEnabled = true;
            this.cbImageFormat2.Items.AddRange(new object[] {
            "PNG",
            "JPEG",
            "GIF",
            "BMP",
            "TIFF"});
            this.cbImageFormat2.Location = new System.Drawing.Point(104, 140);
            this.cbImageFormat2.Name = "cbImageFormat2";
            this.cbImageFormat2.Size = new System.Drawing.Size(56, 21);
            this.cbImageFormat2.TabIndex = 11;
            this.cbImageFormat2.SelectedIndexChanged += new System.EventHandler(this.cbImageFormat2_SelectedIndexChanged);
            // 
            // nudImageJPEGQuality
            // 
            this.nudImageJPEGQuality.Location = new System.Drawing.Point(104, 44);
            this.nudImageJPEGQuality.Name = "nudImageJPEGQuality";
            this.nudImageJPEGQuality.Size = new System.Drawing.Size(56, 20);
            this.nudImageJPEGQuality.TabIndex = 3;
            this.nudImageJPEGQuality.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudImageJPEGQuality.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudImageJPEGQuality.ValueChanged += new System.EventHandler(this.nudImageJPEGQuality_ValueChanged);
            // 
            // lblImageFormat2
            // 
            this.lblImageFormat2.AutoSize = true;
            this.lblImageFormat2.Location = new System.Drawing.Point(16, 144);
            this.lblImageFormat2.Name = "lblImageFormat2";
            this.lblImageFormat2.Size = new System.Drawing.Size(80, 13);
            this.lblImageFormat2.TabIndex = 10;
            this.lblImageFormat2.Text = "Image format 2:";
            // 
            // nudUseImageFormat2After
            // 
            this.nudUseImageFormat2After.Location = new System.Drawing.Point(224, 108);
            this.nudUseImageFormat2After.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudUseImageFormat2After.Name = "nudUseImageFormat2After";
            this.nudUseImageFormat2After.Size = new System.Drawing.Size(56, 20);
            this.nudUseImageFormat2After.TabIndex = 8;
            this.nudUseImageFormat2After.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudUseImageFormat2After.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudUseImageFormat2After.ValueChanged += new System.EventHandler(this.nudUseImageFormat2After_ValueChanged);
            // 
            // lblUseImageFormat2After
            // 
            this.lblUseImageFormat2After.AutoSize = true;
            this.lblUseImageFormat2After.Location = new System.Drawing.Point(16, 112);
            this.lblUseImageFormat2After.Name = "lblUseImageFormat2After";
            this.lblUseImageFormat2After.Size = new System.Drawing.Size(198, 13);
            this.lblUseImageFormat2After.TabIndex = 7;
            this.lblUseImageFormat2After.Text = "Image size limit for use \"Image format 2\":";
            // 
            // tpEffects
            // 
            this.tpEffects.Controls.Add(this.gbImageEffects);
            this.tpEffects.Controls.Add(this.btnWatermarkSettings);
            this.tpEffects.Location = new System.Drawing.Point(4, 22);
            this.tpEffects.Name = "tpEffects";
            this.tpEffects.Padding = new System.Windows.Forms.Padding(3);
            this.tpEffects.Size = new System.Drawing.Size(536, 301);
            this.tpEffects.TabIndex = 2;
            this.tpEffects.Text = "Effects";
            this.tpEffects.UseVisualStyleBackColor = true;
            // 
            // gbImageEffects
            // 
            this.gbImageEffects.Controls.Add(this.chkShowImageEffectsWindowAfterCapture);
            this.gbImageEffects.Controls.Add(this.cbImageEffectOnlyRegionCapture);
            this.gbImageEffects.Controls.Add(this.btnImageEffects);
            this.gbImageEffects.Location = new System.Drawing.Point(16, 16);
            this.gbImageEffects.Name = "gbImageEffects";
            this.gbImageEffects.Size = new System.Drawing.Size(304, 112);
            this.gbImageEffects.TabIndex = 5;
            this.gbImageEffects.TabStop = false;
            this.gbImageEffects.Text = "Image effects";
            // 
            // chkShowImageEffectsWindowAfterCapture
            // 
            this.chkShowImageEffectsWindowAfterCapture.AutoSize = true;
            this.chkShowImageEffectsWindowAfterCapture.Location = new System.Drawing.Point(16, 56);
            this.chkShowImageEffectsWindowAfterCapture.Name = "chkShowImageEffectsWindowAfterCapture";
            this.chkShowImageEffectsWindowAfterCapture.Size = new System.Drawing.Size(221, 17);
            this.chkShowImageEffectsWindowAfterCapture.TabIndex = 4;
            this.chkShowImageEffectsWindowAfterCapture.Text = "Show image effects window after capture";
            this.chkShowImageEffectsWindowAfterCapture.UseVisualStyleBackColor = true;
            this.chkShowImageEffectsWindowAfterCapture.CheckedChanged += new System.EventHandler(this.chkShowImageEffectsWindowAfterCapture_CheckedChanged);
            // 
            // cbImageEffectOnlyRegionCapture
            // 
            this.cbImageEffectOnlyRegionCapture.AutoSize = true;
            this.cbImageEffectOnlyRegionCapture.Location = new System.Drawing.Point(16, 80);
            this.cbImageEffectOnlyRegionCapture.Name = "cbImageEffectOnlyRegionCapture";
            this.cbImageEffectOnlyRegionCapture.Size = new System.Drawing.Size(193, 17);
            this.cbImageEffectOnlyRegionCapture.TabIndex = 3;
            this.cbImageEffectOnlyRegionCapture.Text = "Only apply effects to region capture";
            this.cbImageEffectOnlyRegionCapture.UseVisualStyleBackColor = true;
            this.cbImageEffectOnlyRegionCapture.CheckedChanged += new System.EventHandler(this.cbImageEffectOnlyRegionCapture_CheckedChanged);
            // 
            // btnImageEffects
            // 
            this.btnImageEffects.Location = new System.Drawing.Point(16, 24);
            this.btnImageEffects.Name = "btnImageEffects";
            this.btnImageEffects.Size = new System.Drawing.Size(208, 23);
            this.btnImageEffects.TabIndex = 2;
            this.btnImageEffects.Text = "Image effects configuration...";
            this.btnImageEffects.UseVisualStyleBackColor = true;
            this.btnImageEffects.Click += new System.EventHandler(this.btnImageEffects_Click);
            // 
            // btnWatermarkSettings
            // 
            this.btnWatermarkSettings.Location = new System.Drawing.Point(16, 144);
            this.btnWatermarkSettings.Name = "btnWatermarkSettings";
            this.btnWatermarkSettings.Size = new System.Drawing.Size(208, 23);
            this.btnWatermarkSettings.TabIndex = 0;
            this.btnWatermarkSettings.Text = "Watermark configuration...";
            this.btnWatermarkSettings.UseVisualStyleBackColor = true;
            this.btnWatermarkSettings.Click += new System.EventHandler(this.btnWatermarkSettings_Click);
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
            this.tpThumbnail.Location = new System.Drawing.Point(4, 22);
            this.tpThumbnail.Name = "tpThumbnail";
            this.tpThumbnail.Padding = new System.Windows.Forms.Padding(3);
            this.tpThumbnail.Size = new System.Drawing.Size(536, 301);
            this.tpThumbnail.TabIndex = 3;
            this.tpThumbnail.Text = "Thumbnail";
            this.tpThumbnail.UseVisualStyleBackColor = true;
            // 
            // cbThumbnailIfSmaller
            // 
            this.cbThumbnailIfSmaller.AutoSize = true;
            this.cbThumbnailIfSmaller.Location = new System.Drawing.Point(20, 104);
            this.cbThumbnailIfSmaller.Name = "cbThumbnailIfSmaller";
            this.cbThumbnailIfSmaller.Size = new System.Drawing.Size(322, 17);
            this.cbThumbnailIfSmaller.TabIndex = 7;
            this.cbThumbnailIfSmaller.Text = "Create thumbnail only if image size is bigger than thumbnail size";
            this.cbThumbnailIfSmaller.UseVisualStyleBackColor = true;
            this.cbThumbnailIfSmaller.CheckedChanged += new System.EventHandler(this.cbThumbnailIfSmaller_CheckedChanged);
            // 
            // lblThumbnailNamePreview
            // 
            this.lblThumbnailNamePreview.AutoSize = true;
            this.lblThumbnailNamePreview.Location = new System.Drawing.Point(248, 76);
            this.lblThumbnailNamePreview.Name = "lblThumbnailNamePreview";
            this.lblThumbnailNamePreview.Size = new System.Drawing.Size(45, 13);
            this.lblThumbnailNamePreview.TabIndex = 6;
            this.lblThumbnailNamePreview.Text = "Preview";
            // 
            // lblThumbnailName
            // 
            this.lblThumbnailName.AutoSize = true;
            this.lblThumbnailName.Location = new System.Drawing.Point(16, 76);
            this.lblThumbnailName.Name = "lblThumbnailName";
            this.lblThumbnailName.Size = new System.Drawing.Size(88, 13);
            this.lblThumbnailName.TabIndex = 5;
            this.lblThumbnailName.Text = "Thumbnail name:";
            // 
            // txtThumbnailName
            // 
            this.txtThumbnailName.Location = new System.Drawing.Point(112, 72);
            this.txtThumbnailName.Name = "txtThumbnailName";
            this.txtThumbnailName.Size = new System.Drawing.Size(128, 20);
            this.txtThumbnailName.TabIndex = 4;
            this.txtThumbnailName.TextChanged += new System.EventHandler(this.txtThumbnailName_TextChanged);
            // 
            // lblThumbnailHeight
            // 
            this.lblThumbnailHeight.AutoSize = true;
            this.lblThumbnailHeight.Location = new System.Drawing.Point(16, 44);
            this.lblThumbnailHeight.Name = "lblThumbnailHeight";
            this.lblThumbnailHeight.Size = new System.Drawing.Size(41, 13);
            this.lblThumbnailHeight.TabIndex = 3;
            this.lblThumbnailHeight.Text = "Height:";
            // 
            // lblThumbnailWidth
            // 
            this.lblThumbnailWidth.AutoSize = true;
            this.lblThumbnailWidth.Location = new System.Drawing.Point(16, 16);
            this.lblThumbnailWidth.Name = "lblThumbnailWidth";
            this.lblThumbnailWidth.Size = new System.Drawing.Size(38, 13);
            this.lblThumbnailWidth.TabIndex = 2;
            this.lblThumbnailWidth.Text = "Width:";
            // 
            // nudThumbnailHeight
            // 
            this.nudThumbnailHeight.Location = new System.Drawing.Point(64, 40);
            this.nudThumbnailHeight.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudThumbnailHeight.Name = "nudThumbnailHeight";
            this.nudThumbnailHeight.Size = new System.Drawing.Size(64, 20);
            this.nudThumbnailHeight.TabIndex = 1;
            this.nudThumbnailHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudThumbnailHeight.ValueChanged += new System.EventHandler(this.nudThumbnailHeight_ValueChanged);
            // 
            // nudThumbnailWidth
            // 
            this.nudThumbnailWidth.Location = new System.Drawing.Point(64, 12);
            this.nudThumbnailWidth.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudThumbnailWidth.Name = "nudThumbnailWidth";
            this.nudThumbnailWidth.Size = new System.Drawing.Size(64, 20);
            this.nudThumbnailWidth.TabIndex = 0;
            this.nudThumbnailWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudThumbnailWidth.ValueChanged += new System.EventHandler(this.nudThumbnailWidth_ValueChanged);
            // 
            // chkUseDefaultImageSettings
            // 
            this.chkUseDefaultImageSettings.AutoSize = true;
            this.chkUseDefaultImageSettings.Checked = true;
            this.chkUseDefaultImageSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultImageSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultImageSettings.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultImageSettings.Name = "chkUseDefaultImageSettings";
            this.chkUseDefaultImageSettings.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultImageSettings.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultImageSettings.TabIndex = 0;
            this.chkUseDefaultImageSettings.Text = "Use image settings in main window task settings";
            this.chkUseDefaultImageSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultImageSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultImageSettings_CheckedChanged);
            // 
            // tpCapture
            // 
            this.tpCapture.Controls.Add(this.tcCapture);
            this.tpCapture.Controls.Add(this.chkUseDefaultCaptureSettings);
            this.tpCapture.Location = new System.Drawing.Point(4, 22);
            this.tpCapture.Name = "tpCapture";
            this.tpCapture.Padding = new System.Windows.Forms.Padding(3);
            this.tpCapture.Size = new System.Drawing.Size(550, 360);
            this.tpCapture.TabIndex = 2;
            this.tpCapture.Text = "Capture";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // tcCapture
            // 
            this.tcCapture.Controls.Add(this.tpCaptureGeneral);
            this.tcCapture.Controls.Add(this.tpCaptureShape);
            this.tcCapture.Controls.Add(this.tpScreenRecorder);
            this.tcCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCapture.Location = new System.Drawing.Point(3, 30);
            this.tcCapture.Name = "tcCapture";
            this.tcCapture.SelectedIndex = 0;
            this.tcCapture.Size = new System.Drawing.Size(544, 327);
            this.tcCapture.TabIndex = 1;
            // 
            // tpCaptureGeneral
            // 
            this.tpCaptureGeneral.Controls.Add(this.cbCaptureAutoHideTaskbar);
            this.tpCaptureGeneral.Controls.Add(this.lblScreenshotDelayInfo);
            this.tpCaptureGeneral.Controls.Add(this.nudScreenshotDelay);
            this.tpCaptureGeneral.Controls.Add(this.cbScreenshotDelay);
            this.tpCaptureGeneral.Controls.Add(this.nudCaptureShadowOffset);
            this.tpCaptureGeneral.Controls.Add(this.cbCaptureClientArea);
            this.tpCaptureGeneral.Controls.Add(this.cbCaptureShadow);
            this.tpCaptureGeneral.Controls.Add(this.cbShowCursor);
            this.tpCaptureGeneral.Controls.Add(this.cbCaptureTransparent);
            this.tpCaptureGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpCaptureGeneral.Name = "tpCaptureGeneral";
            this.tpCaptureGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpCaptureGeneral.Size = new System.Drawing.Size(536, 301);
            this.tpCaptureGeneral.TabIndex = 0;
            this.tpCaptureGeneral.Text = "General";
            this.tpCaptureGeneral.UseVisualStyleBackColor = true;
            // 
            // cbCaptureAutoHideTaskbar
            // 
            this.cbCaptureAutoHideTaskbar.AutoSize = true;
            this.cbCaptureAutoHideTaskbar.Location = new System.Drawing.Point(16, 136);
            this.cbCaptureAutoHideTaskbar.Name = "cbCaptureAutoHideTaskbar";
            this.cbCaptureAutoHideTaskbar.Size = new System.Drawing.Size(402, 17);
            this.cbCaptureAutoHideTaskbar.TabIndex = 8;
            this.cbCaptureAutoHideTaskbar.Text = "When doing window capture if window intersects with taskbar then hide taskbar";
            this.cbCaptureAutoHideTaskbar.UseVisualStyleBackColor = true;
            this.cbCaptureAutoHideTaskbar.CheckedChanged += new System.EventHandler(this.cbCaptureAutoHideTaskbar_CheckedChanged);
            // 
            // lblScreenshotDelayInfo
            // 
            this.lblScreenshotDelayInfo.AutoSize = true;
            this.lblScreenshotDelayInfo.Location = new System.Drawing.Point(192, 114);
            this.lblScreenshotDelayInfo.Name = "lblScreenshotDelayInfo";
            this.lblScreenshotDelayInfo.Size = new System.Drawing.Size(47, 13);
            this.lblScreenshotDelayInfo.TabIndex = 7;
            this.lblScreenshotDelayInfo.Text = "seconds";
            // 
            // nudScreenshotDelay
            // 
            this.nudScreenshotDelay.DecimalPlaces = 1;
            this.nudScreenshotDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudScreenshotDelay.Location = new System.Drawing.Point(128, 110);
            this.nudScreenshotDelay.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudScreenshotDelay.Name = "nudScreenshotDelay";
            this.nudScreenshotDelay.Size = new System.Drawing.Size(56, 20);
            this.nudScreenshotDelay.TabIndex = 6;
            this.nudScreenshotDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScreenshotDelay.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudScreenshotDelay.ValueChanged += new System.EventHandler(this.nudScreenshotDelay_ValueChanged);
            // 
            // cbScreenshotDelay
            // 
            this.cbScreenshotDelay.AutoSize = true;
            this.cbScreenshotDelay.Location = new System.Drawing.Point(16, 112);
            this.cbScreenshotDelay.Name = "cbScreenshotDelay";
            this.cbScreenshotDelay.Size = new System.Drawing.Size(111, 17);
            this.cbScreenshotDelay.TabIndex = 5;
            this.cbScreenshotDelay.Text = "Screenshot delay:";
            this.cbScreenshotDelay.UseVisualStyleBackColor = true;
            this.cbScreenshotDelay.CheckedChanged += new System.EventHandler(this.cbScreenshotDelay_CheckedChanged);
            // 
            // nudCaptureShadowOffset
            // 
            this.nudCaptureShadowOffset.Location = new System.Drawing.Point(368, 62);
            this.nudCaptureShadowOffset.Name = "nudCaptureShadowOffset";
            this.nudCaptureShadowOffset.Size = new System.Drawing.Size(48, 20);
            this.nudCaptureShadowOffset.TabIndex = 3;
            this.nudCaptureShadowOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCaptureShadowOffset.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudCaptureShadowOffset.ValueChanged += new System.EventHandler(this.nudCaptureShadowOffset_ValueChanged);
            // 
            // cbCaptureClientArea
            // 
            this.cbCaptureClientArea.AutoSize = true;
            this.cbCaptureClientArea.Location = new System.Drawing.Point(16, 88);
            this.cbCaptureClientArea.Name = "cbCaptureClientArea";
            this.cbCaptureClientArea.Size = new System.Drawing.Size(334, 17);
            this.cbCaptureClientArea.TabIndex = 4;
            this.cbCaptureClientArea.Text = "Capture client area when doing window or active window capture";
            this.cbCaptureClientArea.UseVisualStyleBackColor = true;
            this.cbCaptureClientArea.CheckedChanged += new System.EventHandler(this.cbCaptureClientArea_CheckedChanged);
            // 
            // cbCaptureShadow
            // 
            this.cbCaptureShadow.AutoSize = true;
            this.cbCaptureShadow.Location = new System.Drawing.Point(16, 64);
            this.cbCaptureShadow.Name = "cbCaptureShadow";
            this.cbCaptureShadow.Size = new System.Drawing.Size(351, 17);
            this.cbCaptureShadow.TabIndex = 2;
            this.cbCaptureShadow.Text = "Capture window with shadow (requires transparency)  Shadow offset:";
            this.cbCaptureShadow.UseVisualStyleBackColor = true;
            this.cbCaptureShadow.CheckedChanged += new System.EventHandler(this.cbCaptureShadow_CheckedChanged);
            // 
            // cbShowCursor
            // 
            this.cbShowCursor.AutoSize = true;
            this.cbShowCursor.Location = new System.Drawing.Point(16, 16);
            this.cbShowCursor.Name = "cbShowCursor";
            this.cbShowCursor.Size = new System.Drawing.Size(156, 17);
            this.cbShowCursor.TabIndex = 0;
            this.cbShowCursor.Text = "Show cursor in screenshots";
            this.cbShowCursor.UseVisualStyleBackColor = true;
            this.cbShowCursor.CheckedChanged += new System.EventHandler(this.cbShowCursor_CheckedChanged);
            // 
            // cbCaptureTransparent
            // 
            this.cbCaptureTransparent.AutoSize = true;
            this.cbCaptureTransparent.Location = new System.Drawing.Point(16, 40);
            this.cbCaptureTransparent.Name = "cbCaptureTransparent";
            this.cbCaptureTransparent.Size = new System.Drawing.Size(188, 17);
            this.cbCaptureTransparent.TabIndex = 1;
            this.cbCaptureTransparent.Text = "Capture window with transparency";
            this.cbCaptureTransparent.UseVisualStyleBackColor = true;
            this.cbCaptureTransparent.CheckedChanged += new System.EventHandler(this.cbCaptureTransparent_CheckedChanged);
            // 
            // tpCaptureShape
            // 
            this.tpCaptureShape.Controls.Add(this.pgShapesCapture);
            this.tpCaptureShape.Location = new System.Drawing.Point(4, 22);
            this.tpCaptureShape.Name = "tpCaptureShape";
            this.tpCaptureShape.Padding = new System.Windows.Forms.Padding(3);
            this.tpCaptureShape.Size = new System.Drawing.Size(536, 301);
            this.tpCaptureShape.TabIndex = 1;
            this.tpCaptureShape.Text = "Shape capture";
            this.tpCaptureShape.UseVisualStyleBackColor = true;
            // 
            // pgShapesCapture
            // 
            this.pgShapesCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgShapesCapture.Location = new System.Drawing.Point(3, 3);
            this.pgShapesCapture.Name = "pgShapesCapture";
            this.pgShapesCapture.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgShapesCapture.Size = new System.Drawing.Size(530, 295);
            this.pgShapesCapture.TabIndex = 11;
            this.pgShapesCapture.ToolbarVisible = false;
            // 
            // tpScreenRecorder
            // 
            this.tpScreenRecorder.Controls.Add(this.chkRunScreencastCLI);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderCLI);
            this.tpScreenRecorder.Controls.Add(this.btnScreenRecorderOptions);
            this.tpScreenRecorder.Controls.Add(this.btnEncoderConfig);
            this.tpScreenRecorder.Controls.Add(this.cboEncoder);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderDuration);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderStartDelay);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderStartDelay);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecorderOutput);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderOutput);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecorderFixedDuration);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderFPS);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderFPS);
            this.tpScreenRecorder.Location = new System.Drawing.Point(4, 22);
            this.tpScreenRecorder.Name = "tpScreenRecorder";
            this.tpScreenRecorder.Padding = new System.Windows.Forms.Padding(3);
            this.tpScreenRecorder.Size = new System.Drawing.Size(536, 301);
            this.tpScreenRecorder.TabIndex = 2;
            this.tpScreenRecorder.Text = "Screen recorder";
            this.tpScreenRecorder.UseVisualStyleBackColor = true;
            // 
            // chkRunScreencastCLI
            // 
            this.chkRunScreencastCLI.AutoSize = true;
            this.chkRunScreencastCLI.Location = new System.Drawing.Point(392, 14);
            this.chkRunScreencastCLI.Name = "chkRunScreencastCLI";
            this.chkRunScreencastCLI.Size = new System.Drawing.Size(117, 17);
            this.chkRunScreencastCLI.TabIndex = 14;
            this.chkRunScreencastCLI.Text = "Run CLI afterwards";
            this.chkRunScreencastCLI.UseVisualStyleBackColor = true;
            this.chkRunScreencastCLI.CheckedChanged += new System.EventHandler(this.chkRunScreencastCLI_CheckedChanged);
            // 
            // lblScreenRecorderCLI
            // 
            this.lblScreenRecorderCLI.AutoSize = true;
            this.lblScreenRecorderCLI.Location = new System.Drawing.Point(16, 48);
            this.lblScreenRecorderCLI.Name = "lblScreenRecorderCLI";
            this.lblScreenRecorderCLI.Size = new System.Drawing.Size(26, 13);
            this.lblScreenRecorderCLI.TabIndex = 13;
            this.lblScreenRecorderCLI.Text = "CLI:";
            // 
            // btnScreenRecorderOptions
            // 
            this.btnScreenRecorderOptions.Location = new System.Drawing.Point(320, 11);
            this.btnScreenRecorderOptions.Name = "btnScreenRecorderOptions";
            this.btnScreenRecorderOptions.Size = new System.Drawing.Size(64, 23);
            this.btnScreenRecorderOptions.TabIndex = 12;
            this.btnScreenRecorderOptions.Text = "Options...";
            this.btnScreenRecorderOptions.UseVisualStyleBackColor = true;
            this.btnScreenRecorderOptions.Click += new System.EventHandler(this.btnScreenRecorderOptions_Click);
            // 
            // btnEncoderConfig
            // 
            this.btnEncoderConfig.Location = new System.Drawing.Point(344, 43);
            this.btnEncoderConfig.Name = "btnEncoderConfig";
            this.btnEncoderConfig.Size = new System.Drawing.Size(40, 23);
            this.btnEncoderConfig.TabIndex = 11;
            this.btnEncoderConfig.Text = "...";
            this.btnEncoderConfig.UseVisualStyleBackColor = true;
            this.btnEncoderConfig.Click += new System.EventHandler(this.btnEncoderConfig_Click);
            // 
            // cboEncoder
            // 
            this.cboEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEncoder.FormattingEnabled = true;
            this.cboEncoder.Location = new System.Drawing.Point(48, 44);
            this.cboEncoder.Name = "cboEncoder";
            this.cboEncoder.Size = new System.Drawing.Size(288, 21);
            this.cboEncoder.TabIndex = 10;
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
            this.nudScreenRecorderDuration.Location = new System.Drawing.Point(163, 100);
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
            this.nudScreenRecorderDuration.Size = new System.Drawing.Size(56, 20);
            this.nudScreenRecorderDuration.TabIndex = 6;
            this.nudScreenRecorderDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScreenRecorderDuration.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudScreenRecorderDuration.ValueChanged += new System.EventHandler(this.nudScreenRecorderDuration_ValueChanged);
            // 
            // lblScreenRecorderStartDelay
            // 
            this.lblScreenRecorderStartDelay.AutoSize = true;
            this.lblScreenRecorderStartDelay.Location = new System.Drawing.Point(16, 134);
            this.lblScreenRecorderStartDelay.Name = "lblScreenRecorderStartDelay";
            this.lblScreenRecorderStartDelay.Size = new System.Drawing.Size(60, 13);
            this.lblScreenRecorderStartDelay.TabIndex = 9;
            this.lblScreenRecorderStartDelay.Text = "Start delay:";
            // 
            // nudScreenRecorderStartDelay
            // 
            this.nudScreenRecorderStartDelay.DecimalPlaces = 1;
            this.nudScreenRecorderStartDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudScreenRecorderStartDelay.Location = new System.Drawing.Point(80, 130);
            this.nudScreenRecorderStartDelay.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudScreenRecorderStartDelay.Name = "nudScreenRecorderStartDelay";
            this.nudScreenRecorderStartDelay.Size = new System.Drawing.Size(56, 20);
            this.nudScreenRecorderStartDelay.TabIndex = 8;
            this.nudScreenRecorderStartDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            this.cbScreenRecorderOutput.Location = new System.Drawing.Point(64, 12);
            this.cbScreenRecorderOutput.Name = "cbScreenRecorderOutput";
            this.cbScreenRecorderOutput.Size = new System.Drawing.Size(248, 21);
            this.cbScreenRecorderOutput.TabIndex = 1;
            this.cbScreenRecorderOutput.SelectedIndexChanged += new System.EventHandler(this.cbScreenRecorderOutput_SelectedIndexChanged);
            // 
            // lblScreenRecorderOutput
            // 
            this.lblScreenRecorderOutput.AutoSize = true;
            this.lblScreenRecorderOutput.Location = new System.Drawing.Point(16, 16);
            this.lblScreenRecorderOutput.Name = "lblScreenRecorderOutput";
            this.lblScreenRecorderOutput.Size = new System.Drawing.Size(42, 13);
            this.lblScreenRecorderOutput.TabIndex = 0;
            this.lblScreenRecorderOutput.Text = "Output:";
            // 
            // cbScreenRecorderFixedDuration
            // 
            this.cbScreenRecorderFixedDuration.AutoSize = true;
            this.cbScreenRecorderFixedDuration.Location = new System.Drawing.Point(19, 102);
            this.cbScreenRecorderFixedDuration.Name = "cbScreenRecorderFixedDuration";
            this.cbScreenRecorderFixedDuration.Size = new System.Drawing.Size(144, 17);
            this.cbScreenRecorderFixedDuration.TabIndex = 4;
            this.cbScreenRecorderFixedDuration.Text = "Fixed duration (seconds):";
            this.cbScreenRecorderFixedDuration.UseVisualStyleBackColor = true;
            this.cbScreenRecorderFixedDuration.CheckedChanged += new System.EventHandler(this.cbScreenRecorderFixedDuration_CheckedChanged);
            // 
            // nudScreenRecorderFPS
            // 
            this.nudScreenRecorderFPS.Location = new System.Drawing.Point(48, 74);
            this.nudScreenRecorderFPS.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudScreenRecorderFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScreenRecorderFPS.Name = "nudScreenRecorderFPS";
            this.nudScreenRecorderFPS.Size = new System.Drawing.Size(64, 20);
            this.nudScreenRecorderFPS.TabIndex = 3;
            this.nudScreenRecorderFPS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScreenRecorderFPS.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudScreenRecorderFPS.ValueChanged += new System.EventHandler(this.nudScreenRecorderFPS_ValueChanged);
            // 
            // lblScreenRecorderFPS
            // 
            this.lblScreenRecorderFPS.AutoSize = true;
            this.lblScreenRecorderFPS.Location = new System.Drawing.Point(16, 78);
            this.lblScreenRecorderFPS.Name = "lblScreenRecorderFPS";
            this.lblScreenRecorderFPS.Size = new System.Drawing.Size(30, 13);
            this.lblScreenRecorderFPS.TabIndex = 2;
            this.lblScreenRecorderFPS.Text = "FPS:";
            // 
            // chkUseDefaultCaptureSettings
            // 
            this.chkUseDefaultCaptureSettings.AutoSize = true;
            this.chkUseDefaultCaptureSettings.Checked = true;
            this.chkUseDefaultCaptureSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultCaptureSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultCaptureSettings.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultCaptureSettings.Name = "chkUseDefaultCaptureSettings";
            this.chkUseDefaultCaptureSettings.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultCaptureSettings.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultCaptureSettings.TabIndex = 0;
            this.chkUseDefaultCaptureSettings.Text = "Use capture settings in main window task settings";
            this.chkUseDefaultCaptureSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultCaptureSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultCaptureSettings_CheckedChanged);
            // 
            // tpActions
            // 
            this.tpActions.Controls.Add(this.pActions);
            this.tpActions.Controls.Add(this.chkUseDefaultActions);
            this.tpActions.Location = new System.Drawing.Point(4, 22);
            this.tpActions.Name = "tpActions";
            this.tpActions.Padding = new System.Windows.Forms.Padding(3);
            this.tpActions.Size = new System.Drawing.Size(550, 360);
            this.tpActions.TabIndex = 3;
            this.tpActions.Text = "Actions";
            this.tpActions.UseVisualStyleBackColor = true;
            // 
            // pActions
            // 
            this.pActions.Controls.Add(this.btnActionsAdd);
            this.pActions.Controls.Add(this.lvActions);
            this.pActions.Controls.Add(this.btnActionsEdit);
            this.pActions.Controls.Add(this.btnActionsRemove);
            this.pActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pActions.Location = new System.Drawing.Point(3, 30);
            this.pActions.Margin = new System.Windows.Forms.Padding(0);
            this.pActions.Name = "pActions";
            this.pActions.Size = new System.Drawing.Size(544, 327);
            this.pActions.TabIndex = 1;
            // 
            // btnActionsAdd
            // 
            this.btnActionsAdd.Location = new System.Drawing.Point(8, 8);
            this.btnActionsAdd.Name = "btnActionsAdd";
            this.btnActionsAdd.Size = new System.Drawing.Size(75, 23);
            this.btnActionsAdd.TabIndex = 0;
            this.btnActionsAdd.Text = "Add";
            this.btnActionsAdd.UseVisualStyleBackColor = true;
            this.btnActionsAdd.Click += new System.EventHandler(this.btnActionsAdd_Click);
            // 
            // lvActions
            // 
            this.lvActions.CheckBoxes = true;
            this.lvActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chActionsName,
            this.chActionsPath,
            this.chActionsArgs});
            this.lvActions.FullRowSelect = true;
            this.lvActions.Location = new System.Drawing.Point(8, 40);
            this.lvActions.MultiSelect = false;
            this.lvActions.Name = "lvActions";
            this.lvActions.Size = new System.Drawing.Size(496, 280);
            this.lvActions.TabIndex = 3;
            this.lvActions.UseCompatibleStateImageBehavior = false;
            this.lvActions.View = System.Windows.Forms.View.Details;
            this.lvActions.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvActions_ItemChecked);
            // 
            // chActionsName
            // 
            this.chActionsName.Text = "Name";
            this.chActionsName.Width = 100;
            // 
            // chActionsPath
            // 
            this.chActionsPath.Text = "Path";
            this.chActionsPath.Width = 250;
            // 
            // chActionsArgs
            // 
            this.chActionsArgs.Text = "Args";
            this.chActionsArgs.Width = 134;
            // 
            // btnActionsEdit
            // 
            this.btnActionsEdit.Location = new System.Drawing.Point(88, 8);
            this.btnActionsEdit.Name = "btnActionsEdit";
            this.btnActionsEdit.Size = new System.Drawing.Size(75, 23);
            this.btnActionsEdit.TabIndex = 1;
            this.btnActionsEdit.Text = "Edit";
            this.btnActionsEdit.UseVisualStyleBackColor = true;
            this.btnActionsEdit.Click += new System.EventHandler(this.btnActionsEdit_Click);
            // 
            // btnActionsRemove
            // 
            this.btnActionsRemove.Location = new System.Drawing.Point(168, 8);
            this.btnActionsRemove.Name = "btnActionsRemove";
            this.btnActionsRemove.Size = new System.Drawing.Size(75, 23);
            this.btnActionsRemove.TabIndex = 2;
            this.btnActionsRemove.Text = "Remove";
            this.btnActionsRemove.UseVisualStyleBackColor = true;
            this.btnActionsRemove.Click += new System.EventHandler(this.btnActionsRemove_Click);
            // 
            // chkUseDefaultActions
            // 
            this.chkUseDefaultActions.AutoSize = true;
            this.chkUseDefaultActions.Checked = true;
            this.chkUseDefaultActions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultActions.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultActions.Name = "chkUseDefaultActions";
            this.chkUseDefaultActions.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultActions.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultActions.TabIndex = 0;
            this.chkUseDefaultActions.Text = "Use actions in main window task settings";
            this.chkUseDefaultActions.UseVisualStyleBackColor = true;
            this.chkUseDefaultActions.CheckedChanged += new System.EventHandler(this.chkUseDefaultActions_CheckedChanged);
            // 
            // tpWatchFolders
            // 
            this.tpWatchFolders.Controls.Add(this.cbWatchFolderEnabled);
            this.tpWatchFolders.Controls.Add(this.lvWatchFolderList);
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderRemove);
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderAdd);
            this.tpWatchFolders.Location = new System.Drawing.Point(4, 22);
            this.tpWatchFolders.Name = "tpWatchFolders";
            this.tpWatchFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tpWatchFolders.Size = new System.Drawing.Size(550, 360);
            this.tpWatchFolders.TabIndex = 5;
            this.tpWatchFolders.Text = "Watch folders";
            this.tpWatchFolders.UseVisualStyleBackColor = true;
            // 
            // cbWatchFolderEnabled
            // 
            this.cbWatchFolderEnabled.AutoSize = true;
            this.cbWatchFolderEnabled.Location = new System.Drawing.Point(8, 8);
            this.cbWatchFolderEnabled.Name = "cbWatchFolderEnabled";
            this.cbWatchFolderEnabled.Size = new System.Drawing.Size(266, 17);
            this.cbWatchFolderEnabled.TabIndex = 0;
            this.cbWatchFolderEnabled.Text = "Watch folders and if new file created then upload it";
            this.cbWatchFolderEnabled.UseVisualStyleBackColor = true;
            this.cbWatchFolderEnabled.CheckedChanged += new System.EventHandler(this.cbWatchFolderEnabled_CheckedChanged);
            // 
            // lvWatchFolderList
            // 
            this.lvWatchFolderList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chWatchFolderFolderPath,
            this.chWatchFolderFilter,
            this.chWatchFolderIncludeSubdirectories});
            this.lvWatchFolderList.FullRowSelect = true;
            this.lvWatchFolderList.Location = new System.Drawing.Point(8, 64);
            this.lvWatchFolderList.Name = "lvWatchFolderList";
            this.lvWatchFolderList.Size = new System.Drawing.Size(528, 288);
            this.lvWatchFolderList.TabIndex = 3;
            this.lvWatchFolderList.UseCompatibleStateImageBehavior = false;
            this.lvWatchFolderList.View = System.Windows.Forms.View.Details;
            this.lvWatchFolderList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvWatchFolderList_MouseDoubleClick);
            // 
            // chWatchFolderFolderPath
            // 
            this.chWatchFolderFolderPath.Text = "Folder path";
            this.chWatchFolderFolderPath.Width = 323;
            // 
            // chWatchFolderFilter
            // 
            this.chWatchFolderFilter.Text = "Filter";
            this.chWatchFolderFilter.Width = 43;
            // 
            // chWatchFolderIncludeSubdirectories
            // 
            this.chWatchFolderIncludeSubdirectories.Text = "Include subdirectories";
            this.chWatchFolderIncludeSubdirectories.Width = 124;
            // 
            // btnWatchFolderRemove
            // 
            this.btnWatchFolderRemove.Location = new System.Drawing.Point(88, 32);
            this.btnWatchFolderRemove.Name = "btnWatchFolderRemove";
            this.btnWatchFolderRemove.Size = new System.Drawing.Size(75, 23);
            this.btnWatchFolderRemove.TabIndex = 2;
            this.btnWatchFolderRemove.Text = "Remove";
            this.btnWatchFolderRemove.UseVisualStyleBackColor = true;
            this.btnWatchFolderRemove.Click += new System.EventHandler(this.btnWatchFolderRemove_Click);
            // 
            // btnWatchFolderAdd
            // 
            this.btnWatchFolderAdd.Location = new System.Drawing.Point(8, 32);
            this.btnWatchFolderAdd.Name = "btnWatchFolderAdd";
            this.btnWatchFolderAdd.Size = new System.Drawing.Size(75, 23);
            this.btnWatchFolderAdd.TabIndex = 1;
            this.btnWatchFolderAdd.Text = "Add...";
            this.btnWatchFolderAdd.UseVisualStyleBackColor = true;
            this.btnWatchFolderAdd.Click += new System.EventHandler(this.btnWatchFolderAdd_Click);
            // 
            // tpUpload
            // 
            this.tpUpload.Controls.Add(this.tcUpload);
            this.tpUpload.Controls.Add(this.chkUseDefaultUploadSettings);
            this.tpUpload.Location = new System.Drawing.Point(4, 22);
            this.tpUpload.Name = "tpUpload";
            this.tpUpload.Padding = new System.Windows.Forms.Padding(3);
            this.tpUpload.Size = new System.Drawing.Size(550, 360);
            this.tpUpload.TabIndex = 4;
            this.tpUpload.Text = "Upload";
            this.tpUpload.UseVisualStyleBackColor = true;
            // 
            // tcUpload
            // 
            this.tcUpload.Controls.Add(this.tpUploadNamePattern);
            this.tcUpload.Controls.Add(this.tpUploadClipboard);
            this.tcUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcUpload.Location = new System.Drawing.Point(3, 30);
            this.tcUpload.Name = "tcUpload";
            this.tcUpload.SelectedIndex = 0;
            this.tcUpload.Size = new System.Drawing.Size(544, 327);
            this.tcUpload.TabIndex = 1;
            // 
            // tpUploadNamePattern
            // 
            this.tpUploadNamePattern.Controls.Add(this.cbFileUploadUseNamePattern);
            this.tpUploadNamePattern.Controls.Add(this.lblNameFormatPattern);
            this.tpUploadNamePattern.Controls.Add(this.txtNameFormatPatternActiveWindow);
            this.tpUploadNamePattern.Controls.Add(this.btnResetAutoIncrementNumber);
            this.tpUploadNamePattern.Controls.Add(this.lblNameFormatPatternActiveWindow);
            this.tpUploadNamePattern.Controls.Add(this.txtNameFormatPattern);
            this.tpUploadNamePattern.Controls.Add(this.lblNameFormatPatternPreview);
            this.tpUploadNamePattern.Controls.Add(this.lblNameFormatPatternPreviewActiveWindow);
            this.tpUploadNamePattern.Location = new System.Drawing.Point(4, 22);
            this.tpUploadNamePattern.Name = "tpUploadNamePattern";
            this.tpUploadNamePattern.Padding = new System.Windows.Forms.Padding(3);
            this.tpUploadNamePattern.Size = new System.Drawing.Size(536, 301);
            this.tpUploadNamePattern.TabIndex = 0;
            this.tpUploadNamePattern.Text = "Name pattern";
            this.tpUploadNamePattern.UseVisualStyleBackColor = true;
            // 
            // cbFileUploadUseNamePattern
            // 
            this.cbFileUploadUseNamePattern.AutoSize = true;
            this.cbFileUploadUseNamePattern.Location = new System.Drawing.Point(16, 192);
            this.cbFileUploadUseNamePattern.Name = "cbFileUploadUseNamePattern";
            this.cbFileUploadUseNamePattern.Size = new System.Drawing.Size(313, 17);
            this.cbFileUploadUseNamePattern.TabIndex = 7;
            this.cbFileUploadUseNamePattern.Text = "Use name pattern for file uploads too instead actual file name";
            this.cbFileUploadUseNamePattern.UseVisualStyleBackColor = true;
            this.cbFileUploadUseNamePattern.CheckedChanged += new System.EventHandler(this.cbFileUploadUseNamePattern_CheckedChanged);
            // 
            // lblNameFormatPattern
            // 
            this.lblNameFormatPattern.AutoSize = true;
            this.lblNameFormatPattern.Location = new System.Drawing.Point(16, 20);
            this.lblNameFormatPattern.Name = "lblNameFormatPattern";
            this.lblNameFormatPattern.Size = new System.Drawing.Size(221, 13);
            this.lblNameFormatPattern.TabIndex = 0;
            this.lblNameFormatPattern.Text = "Name pattern for capture or clipboard upload:";
            // 
            // txtNameFormatPatternActiveWindow
            // 
            this.txtNameFormatPatternActiveWindow.Location = new System.Drawing.Point(16, 129);
            this.txtNameFormatPatternActiveWindow.Name = "txtNameFormatPatternActiveWindow";
            this.txtNameFormatPatternActiveWindow.Size = new System.Drawing.Size(448, 20);
            this.txtNameFormatPatternActiveWindow.TabIndex = 5;
            this.txtNameFormatPatternActiveWindow.TextChanged += new System.EventHandler(this.txtNameFormatPatternActiveWindow_TextChanged);
            // 
            // btnResetAutoIncrementNumber
            // 
            this.btnResetAutoIncrementNumber.Location = new System.Drawing.Point(296, 17);
            this.btnResetAutoIncrementNumber.Name = "btnResetAutoIncrementNumber";
            this.btnResetAutoIncrementNumber.Size = new System.Drawing.Size(168, 23);
            this.btnResetAutoIncrementNumber.TabIndex = 1;
            this.btnResetAutoIncrementNumber.Text = "Reset auto increment number";
            this.btnResetAutoIncrementNumber.UseVisualStyleBackColor = true;
            this.btnResetAutoIncrementNumber.Click += new System.EventHandler(this.btnResetAutoIncrementNumber_Click);
            // 
            // lblNameFormatPatternActiveWindow
            // 
            this.lblNameFormatPatternActiveWindow.AutoSize = true;
            this.lblNameFormatPatternActiveWindow.Location = new System.Drawing.Point(16, 105);
            this.lblNameFormatPatternActiveWindow.Name = "lblNameFormatPatternActiveWindow";
            this.lblNameFormatPatternActiveWindow.Size = new System.Drawing.Size(199, 13);
            this.lblNameFormatPatternActiveWindow.TabIndex = 4;
            this.lblNameFormatPatternActiveWindow.Text = "Name pattern for active window capture:";
            // 
            // txtNameFormatPattern
            // 
            this.txtNameFormatPattern.Location = new System.Drawing.Point(16, 44);
            this.txtNameFormatPattern.Name = "txtNameFormatPattern";
            this.txtNameFormatPattern.Size = new System.Drawing.Size(448, 20);
            this.txtNameFormatPattern.TabIndex = 2;
            this.txtNameFormatPattern.TextChanged += new System.EventHandler(this.txtNameFormatPattern_TextChanged);
            // 
            // lblNameFormatPatternPreview
            // 
            this.lblNameFormatPatternPreview.AutoSize = true;
            this.lblNameFormatPatternPreview.Location = new System.Drawing.Point(16, 76);
            this.lblNameFormatPatternPreview.Name = "lblNameFormatPatternPreview";
            this.lblNameFormatPatternPreview.Size = new System.Drawing.Size(48, 13);
            this.lblNameFormatPatternPreview.TabIndex = 3;
            this.lblNameFormatPatternPreview.Text = "Preview:";
            // 
            // lblNameFormatPatternPreviewActiveWindow
            // 
            this.lblNameFormatPatternPreviewActiveWindow.AutoSize = true;
            this.lblNameFormatPatternPreviewActiveWindow.Location = new System.Drawing.Point(16, 161);
            this.lblNameFormatPatternPreviewActiveWindow.Name = "lblNameFormatPatternPreviewActiveWindow";
            this.lblNameFormatPatternPreviewActiveWindow.Size = new System.Drawing.Size(48, 13);
            this.lblNameFormatPatternPreviewActiveWindow.TabIndex = 6;
            this.lblNameFormatPatternPreviewActiveWindow.Text = "Preview:";
            // 
            // tpUploadClipboard
            // 
            this.tpUploadClipboard.Controls.Add(this.chkClipboardUploadContents);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadAutoIndexFolder);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadAutoDetectURL);
            this.tpUploadClipboard.Location = new System.Drawing.Point(4, 22);
            this.tpUploadClipboard.Name = "tpUploadClipboard";
            this.tpUploadClipboard.Padding = new System.Windows.Forms.Padding(3);
            this.tpUploadClipboard.Size = new System.Drawing.Size(536, 301);
            this.tpUploadClipboard.TabIndex = 1;
            this.tpUploadClipboard.Text = "Clipboard upload";
            this.tpUploadClipboard.UseVisualStyleBackColor = true;
            // 
            // chkClipboardUploadContents
            // 
            this.chkClipboardUploadContents.AutoSize = true;
            this.chkClipboardUploadContents.Location = new System.Drawing.Point(16, 16);
            this.chkClipboardUploadContents.Name = "chkClipboardUploadContents";
            this.chkClipboardUploadContents.Size = new System.Drawing.Size(308, 17);
            this.chkClipboardUploadContents.TabIndex = 3;
            this.chkClipboardUploadContents.Text = "If clipboard contains a file URL then download it and upload";
            this.chkClipboardUploadContents.UseVisualStyleBackColor = true;
            this.chkClipboardUploadContents.CheckedChanged += new System.EventHandler(this.chkClipboardUploadContents_CheckedChanged);
            // 
            // cbClipboardUploadAutoIndexFolder
            // 
            this.cbClipboardUploadAutoIndexFolder.AutoSize = true;
            this.cbClipboardUploadAutoIndexFolder.Location = new System.Drawing.Point(16, 64);
            this.cbClipboardUploadAutoIndexFolder.Name = "cbClipboardUploadAutoIndexFolder";
            this.cbClipboardUploadAutoIndexFolder.Size = new System.Drawing.Size(387, 17);
            this.cbClipboardUploadAutoIndexFolder.TabIndex = 2;
            this.cbClipboardUploadAutoIndexFolder.Text = "If clipboard contains a folder path then index that folder and upload the index";
            this.cbClipboardUploadAutoIndexFolder.UseVisualStyleBackColor = true;
            this.cbClipboardUploadAutoIndexFolder.CheckedChanged += new System.EventHandler(this.cbClipboardUploadAutoIndexFolder_CheckedChanged);
            // 
            // cbClipboardUploadAutoDetectURL
            // 
            this.cbClipboardUploadAutoDetectURL.AutoSize = true;
            this.cbClipboardUploadAutoDetectURL.Location = new System.Drawing.Point(16, 40);
            this.cbClipboardUploadAutoDetectURL.Name = "cbClipboardUploadAutoDetectURL";
            this.cbClipboardUploadAutoDetectURL.Size = new System.Drawing.Size(271, 17);
            this.cbClipboardUploadAutoDetectURL.TabIndex = 1;
            this.cbClipboardUploadAutoDetectURL.Text = "If clipboard contains a URL then use URL shortener";
            this.cbClipboardUploadAutoDetectURL.UseVisualStyleBackColor = true;
            this.cbClipboardUploadAutoDetectURL.CheckedChanged += new System.EventHandler(this.cbClipboardUploadAutoDetectURL_CheckedChanged);
            // 
            // chkUseDefaultUploadSettings
            // 
            this.chkUseDefaultUploadSettings.AutoSize = true;
            this.chkUseDefaultUploadSettings.Checked = true;
            this.chkUseDefaultUploadSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultUploadSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultUploadSettings.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultUploadSettings.Name = "chkUseDefaultUploadSettings";
            this.chkUseDefaultUploadSettings.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultUploadSettings.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultUploadSettings.TabIndex = 0;
            this.chkUseDefaultUploadSettings.Text = "Use upload settings in main window task settings";
            this.chkUseDefaultUploadSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultUploadSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultUploadSettings_CheckedChanged);
            // 
            // tpIndexer
            // 
            this.tpIndexer.Controls.Add(this.pgIndexerConfig);
            this.tpIndexer.Controls.Add(this.chkUseDefaultIndexerSettings);
            this.tpIndexer.Location = new System.Drawing.Point(4, 22);
            this.tpIndexer.Name = "tpIndexer";
            this.tpIndexer.Padding = new System.Windows.Forms.Padding(3);
            this.tpIndexer.Size = new System.Drawing.Size(550, 360);
            this.tpIndexer.TabIndex = 8;
            this.tpIndexer.Text = "Indexer";
            this.tpIndexer.UseVisualStyleBackColor = true;
            // 
            // pgIndexerConfig
            // 
            this.pgIndexerConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgIndexerConfig.Location = new System.Drawing.Point(3, 30);
            this.pgIndexerConfig.Name = "pgIndexerConfig";
            this.pgIndexerConfig.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgIndexerConfig.Size = new System.Drawing.Size(544, 327);
            this.pgIndexerConfig.TabIndex = 0;
            this.pgIndexerConfig.ToolbarVisible = false;
            // 
            // chkUseDefaultIndexerSettings
            // 
            this.chkUseDefaultIndexerSettings.AutoSize = true;
            this.chkUseDefaultIndexerSettings.Checked = true;
            this.chkUseDefaultIndexerSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultIndexerSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultIndexerSettings.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultIndexerSettings.Name = "chkUseDefaultIndexerSettings";
            this.chkUseDefaultIndexerSettings.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultIndexerSettings.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultIndexerSettings.TabIndex = 1;
            this.chkUseDefaultIndexerSettings.Text = "Use indexer settings in main window task settings";
            this.chkUseDefaultIndexerSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultIndexerSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultIndexerSettings_CheckedChanged);
            // 
            // tpAdvanced
            // 
            this.tpAdvanced.Controls.Add(this.pgTaskSettings);
            this.tpAdvanced.Controls.Add(this.chkUseDefaultAdvancedSettings);
            this.tpAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tpAdvanced.Name = "tpAdvanced";
            this.tpAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tpAdvanced.Size = new System.Drawing.Size(550, 360);
            this.tpAdvanced.TabIndex = 6;
            this.tpAdvanced.Text = "Advanced";
            this.tpAdvanced.UseVisualStyleBackColor = true;
            // 
            // pgTaskSettings
            // 
            this.pgTaskSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgTaskSettings.Location = new System.Drawing.Point(3, 30);
            this.pgTaskSettings.Name = "pgTaskSettings";
            this.pgTaskSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgTaskSettings.Size = new System.Drawing.Size(544, 327);
            this.pgTaskSettings.TabIndex = 1;
            this.pgTaskSettings.ToolbarVisible = false;
            // 
            // chkUseDefaultAdvancedSettings
            // 
            this.chkUseDefaultAdvancedSettings.AutoSize = true;
            this.chkUseDefaultAdvancedSettings.Checked = true;
            this.chkUseDefaultAdvancedSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultAdvancedSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUseDefaultAdvancedSettings.Location = new System.Drawing.Point(3, 3);
            this.chkUseDefaultAdvancedSettings.Name = "chkUseDefaultAdvancedSettings";
            this.chkUseDefaultAdvancedSettings.Padding = new System.Windows.Forms.Padding(5);
            this.chkUseDefaultAdvancedSettings.Size = new System.Drawing.Size(544, 27);
            this.chkUseDefaultAdvancedSettings.TabIndex = 0;
            this.chkUseDefaultAdvancedSettings.Text = "Use advanced settings in main window task settings";
            this.chkUseDefaultAdvancedSettings.UseVisualStyleBackColor = true;
            this.chkUseDefaultAdvancedSettings.CheckedChanged += new System.EventHandler(this.chkUseDefaultAdvancedSettings_CheckedChanged);
            // 
            // TaskSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(564, 392);
            this.Controls.Add(this.tcHotkeySettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(580, 430);
            this.Name = "TaskSettingsForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Task settings";
            this.Resize += new System.EventHandler(this.TaskSettingsForm_Resize);
            this.tcHotkeySettings.ResumeLayout(false);
            this.tpTask.ResumeLayout(false);
            this.tpTask.PerformLayout();
            this.cmsDestinations.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.panelGeneral.ResumeLayout(false);
            this.panelGeneral.PerformLayout();
            this.tpImage.ResumeLayout(false);
            this.tpImage.PerformLayout();
            this.tcImage.ResumeLayout(false);
            this.tpQuality.ResumeLayout(false);
            this.tpQuality.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageJPEGQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUseImageFormat2After)).EndInit();
            this.tpEffects.ResumeLayout(false);
            this.gbImageEffects.ResumeLayout(false);
            this.gbImageEffects.PerformLayout();
            this.tpThumbnail.ResumeLayout(false);
            this.tpThumbnail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailWidth)).EndInit();
            this.tpCapture.ResumeLayout(false);
            this.tpCapture.PerformLayout();
            this.tcCapture.ResumeLayout(false);
            this.tpCaptureGeneral.ResumeLayout(false);
            this.tpCaptureGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenshotDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureShadowOffset)).EndInit();
            this.tpCaptureShape.ResumeLayout(false);
            this.tpScreenRecorder.ResumeLayout(false);
            this.tpScreenRecorder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderFPS)).EndInit();
            this.tpActions.ResumeLayout(false);
            this.tpActions.PerformLayout();
            this.pActions.ResumeLayout(false);
            this.tpWatchFolders.ResumeLayout(false);
            this.tpWatchFolders.PerformLayout();
            this.tpUpload.ResumeLayout(false);
            this.tpUpload.PerformLayout();
            this.tcUpload.ResumeLayout(false);
            this.tpUploadNamePattern.ResumeLayout(false);
            this.tpUploadNamePattern.PerformLayout();
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
        private System.Windows.Forms.TabControl tcHotkeySettings;
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
        private System.Windows.Forms.Button btnWatermarkSettings;
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
        private System.Windows.Forms.TabPage tpCaptureShape;
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
        private System.Windows.Forms.CheckBox cbClipboardUploadAutoDetectURL;
        private System.Windows.Forms.TabPage tpAdvanced;
        private System.Windows.Forms.PropertyGrid pgTaskSettings;
        private System.Windows.Forms.CheckBox chkUseDefaultImageSettings;
        private System.Windows.Forms.CheckBox chkUseDefaultCaptureSettings;
        private System.Windows.Forms.CheckBox chkUseDefaultActions;
        private System.Windows.Forms.CheckBox chkUseDefaultUploadSettings;
        private System.Windows.Forms.Panel pActions;
        private System.Windows.Forms.CheckBox chkUseDefaultAdvancedSettings;
        private System.Windows.Forms.CheckBox cbScreenRecorderFixedDuration;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderFPS;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderDuration;
        private System.Windows.Forms.Label lblScreenRecorderFPS;
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
        private System.Windows.Forms.PropertyGrid pgShapesCapture;
        private System.Windows.Forms.TabPage tpIndexer;
        private System.Windows.Forms.PropertyGrid pgIndexerConfig;
        private System.Windows.Forms.CheckBox chkUseDefaultIndexerSettings;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderStartDelay;
        private System.Windows.Forms.Label lblScreenRecorderStartDelay;
        private System.Windows.Forms.Button btnImageEffects;
        private System.Windows.Forms.CheckBox cbImageEffectOnlyRegionCapture;
        private System.Windows.Forms.GroupBox gbImageEffects;
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
        private System.Windows.Forms.ToolStripMenuItem tsmiSocialServices;
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
        private System.Windows.Forms.Label lblScreenRecorderCLI;
        private System.Windows.Forms.CheckBox chkRunScreencastCLI;
        private System.Windows.Forms.CheckBox chkClipboardUploadContents;
        private System.Windows.Forms.ToolTip ttTaskSettings;



    }
}