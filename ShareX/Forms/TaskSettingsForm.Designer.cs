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
            this.cbOverrideAfterCaptureSettings = new System.Windows.Forms.CheckBox();
            this.cbOverrideAfterUploadSettings = new System.Windows.Forms.CheckBox();
            this.cbOverrideDestinationSettings = new System.Windows.Forms.CheckBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.cmsTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tcTaskSettings = new System.Windows.Forms.TabControl();
            this.tpTask = new System.Windows.Forms.TabPage();
            this.lblTask = new System.Windows.Forms.Label();
            this.btnScreenshotsFolderBrowse = new System.Windows.Forms.Button();
            this.txtScreenshotsFolder = new System.Windows.Forms.TextBox();
            this.cbOverrideScreenshotsFolder = new System.Windows.Forms.CheckBox();
            this.cbCustomUploaders = new System.Windows.Forms.ComboBox();
            this.cbOverrideCustomUploader = new System.Windows.Forms.CheckBox();
            this.cbOverrideFTPAccount = new System.Windows.Forms.CheckBox();
            this.cbFTPAccounts = new System.Windows.Forms.ComboBox();
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
            this.tcGeneral = new System.Windows.Forms.TabControl();
            this.tpGeneralMain = new System.Windows.Forms.TabPage();
            this.cbOverrideGeneralSettings = new System.Windows.Forms.CheckBox();
            this.tpNotifications = new System.Windows.Forms.TabPage();
            this.btnCustomActionCompletedSoundPath = new System.Windows.Forms.Button();
            this.txtCustomActionCompletedSoundPath = new System.Windows.Forms.TextBox();
            this.cbUseCustomActionCompletedSound = new System.Windows.Forms.CheckBox();
            this.cbPlaySoundAfterAction = new System.Windows.Forms.CheckBox();
            this.cbShowToastNotificationAfterTaskCompleted = new System.Windows.Forms.CheckBox();
            this.btnCustomErrorSoundPath = new System.Windows.Forms.Button();
            this.btnCustomTaskCompletedSoundPath = new System.Windows.Forms.Button();
            this.btnCustomCaptureSoundPath = new System.Windows.Forms.Button();
            this.txtCustomErrorSoundPath = new System.Windows.Forms.TextBox();
            this.txtCustomTaskCompletedSoundPath = new System.Windows.Forms.TextBox();
            this.txtCustomCaptureSoundPath = new System.Windows.Forms.TextBox();
            this.cbUseCustomErrorSound = new System.Windows.Forms.CheckBox();
            this.cbUseCustomTaskCompletedSound = new System.Windows.Forms.CheckBox();
            this.cbUseCustomCaptureSound = new System.Windows.Forms.CheckBox();
            this.gbToastWindow = new System.Windows.Forms.GroupBox();
            this.cbToastWindowAutoHide = new System.Windows.Forms.CheckBox();
            this.lblToastWindowFadeDurationSeconds = new System.Windows.Forms.Label();
            this.lblToastWindowDurationSeconds = new System.Windows.Forms.Label();
            this.lblToastWindowSizeX = new System.Windows.Forms.Label();
            this.cbToastWindowMiddleClickAction = new System.Windows.Forms.ComboBox();
            this.cbToastWindowRightClickAction = new System.Windows.Forms.ComboBox();
            this.cbToastWindowLeftClickAction = new System.Windows.Forms.ComboBox();
            this.nudToastWindowSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.nudToastWindowSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.cbToastWindowPlacement = new System.Windows.Forms.ComboBox();
            this.nudToastWindowFadeDuration = new System.Windows.Forms.NumericUpDown();
            this.cbDisableNotificationsOnFullscreen = new System.Windows.Forms.CheckBox();
            this.nudToastWindowDuration = new System.Windows.Forms.NumericUpDown();
            this.lblToastWindowMiddleClickAction = new System.Windows.Forms.Label();
            this.lblToastWindowRightClickAction = new System.Windows.Forms.Label();
            this.lblToastWindowLeftClickAction = new System.Windows.Forms.Label();
            this.lblToastWindowSize = new System.Windows.Forms.Label();
            this.lblToastWindowPlacement = new System.Windows.Forms.Label();
            this.lblToastWindowFadeDuration = new System.Windows.Forms.Label();
            this.lblToastWindowDuration = new System.Windows.Forms.Label();
            this.cbPlaySoundAfterCapture = new System.Windows.Forms.CheckBox();
            this.cbPlaySoundAfterUpload = new System.Windows.Forms.CheckBox();
            this.tpImage = new System.Windows.Forms.TabPage();
            this.tcImage = new System.Windows.Forms.TabControl();
            this.tpQuality = new System.Windows.Forms.TabPage();
            this.pImage = new System.Windows.Forms.Panel();
            this.cbImageAutoJPEGQuality = new System.Windows.Forms.CheckBox();
            this.cbImagePNGBitDepth = new System.Windows.Forms.ComboBox();
            this.lblImagePNGBitDepth = new System.Windows.Forms.Label();
            this.cbImageAutoUseJPEG = new System.Windows.Forms.CheckBox();
            this.lblImageFormat = new System.Windows.Forms.Label();
            this.cbImageFileExist = new System.Windows.Forms.ComboBox();
            this.lblImageFileExist = new System.Windows.Forms.Label();
            this.nudImageAutoUseJPEGSize = new System.Windows.Forms.NumericUpDown();
            this.lblImageSizeLimitHint = new System.Windows.Forms.Label();
            this.nudImageJPEGQuality = new System.Windows.Forms.NumericUpDown();
            this.cbImageFormat = new System.Windows.Forms.ComboBox();
            this.lblImageJPEGQualityHint = new System.Windows.Forms.Label();
            this.lblImageGIFQuality = new System.Windows.Forms.Label();
            this.lblImageJPEGQuality = new System.Windows.Forms.Label();
            this.cbImageGIFQuality = new System.Windows.Forms.ComboBox();
            this.cbOverrideImageSettings = new System.Windows.Forms.CheckBox();
            this.tpEffects = new System.Windows.Forms.TabPage();
            this.cbUseRandomImageEffect = new System.Windows.Forms.CheckBox();
            this.lblImageEffectsNote = new System.Windows.Forms.Label();
            this.cbShowImageEffectsWindowAfterCapture = new System.Windows.Forms.CheckBox();
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
            this.txtCaptureCustomWindow = new System.Windows.Forms.TextBox();
            this.lblCaptureCustomWindow = new System.Windows.Forms.Label();
            this.lblScreenshotDelay = new System.Windows.Forms.Label();
            this.btnCaptureCustomRegionSelectRectangle = new System.Windows.Forms.Button();
            this.lblCaptureCustomRegion = new System.Windows.Forms.Label();
            this.lblCaptureCustomRegionWidth = new System.Windows.Forms.Label();
            this.lblCaptureCustomRegionHeight = new System.Windows.Forms.Label();
            this.lblCaptureCustomRegionY = new System.Windows.Forms.Label();
            this.lblCaptureCustomRegionX = new System.Windows.Forms.Label();
            this.nudCaptureCustomRegionHeight = new System.Windows.Forms.NumericUpDown();
            this.nudCaptureCustomRegionWidth = new System.Windows.Forms.NumericUpDown();
            this.nudCaptureCustomRegionY = new System.Windows.Forms.NumericUpDown();
            this.nudCaptureCustomRegionX = new System.Windows.Forms.NumericUpDown();
            this.cbShowCursor = new System.Windows.Forms.CheckBox();
            this.lblCaptureShadowOffset = new System.Windows.Forms.Label();
            this.cbCaptureTransparent = new System.Windows.Forms.CheckBox();
            this.cbCaptureAutoHideTaskbar = new System.Windows.Forms.CheckBox();
            this.cbCaptureShadow = new System.Windows.Forms.CheckBox();
            this.lblScreenshotDelayInfo = new System.Windows.Forms.Label();
            this.cbCaptureClientArea = new System.Windows.Forms.CheckBox();
            this.nudScreenshotDelay = new System.Windows.Forms.NumericUpDown();
            this.nudCaptureShadowOffset = new System.Windows.Forms.NumericUpDown();
            this.cbOverrideCaptureSettings = new System.Windows.Forms.CheckBox();
            this.tpRegionCapture = new System.Windows.Forms.TabPage();
            this.lblRegionCaptureBackgroundDimStrengthHint = new System.Windows.Forms.Label();
            this.nudRegionCaptureBackgroundDimStrength = new System.Windows.Forms.NumericUpDown();
            this.lblRegionCaptureBackgroundDimStrength = new System.Windows.Forms.Label();
            this.cbRegionCaptureActiveMonitorMode = new System.Windows.Forms.CheckBox();
            this.nudRegionCaptureFPSLimit = new System.Windows.Forms.NumericUpDown();
            this.lblRegionCaptureFPSLimit = new System.Windows.Forms.Label();
            this.cbRegionCaptureShowFPS = new System.Windows.Forms.CheckBox();
            this.flpRegionCaptureFixedSize = new System.Windows.Forms.FlowLayoutPanel();
            this.lblRegionCaptureFixedSizeWidth = new System.Windows.Forms.Label();
            this.nudRegionCaptureFixedSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.lblRegionCaptureFixedSizeHeight = new System.Windows.Forms.Label();
            this.nudRegionCaptureFixedSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.cbRegionCaptureIsFixedSize = new System.Windows.Forms.CheckBox();
            this.cbRegionCaptureShowCrosshair = new System.Windows.Forms.CheckBox();
            this.lblRegionCaptureMagnifierPixelSize = new System.Windows.Forms.Label();
            this.lblRegionCaptureMagnifierPixelCount = new System.Windows.Forms.Label();
            this.cbRegionCaptureUseSquareMagnifier = new System.Windows.Forms.CheckBox();
            this.cbRegionCaptureShowMagnifier = new System.Windows.Forms.CheckBox();
            this.cbRegionCaptureShowInfo = new System.Windows.Forms.CheckBox();
            this.btnRegionCaptureSnapSizesRemove = new System.Windows.Forms.Button();
            this.btnRegionCaptureSnapSizesAdd = new System.Windows.Forms.Button();
            this.cbRegionCaptureSnapSizes = new System.Windows.Forms.ComboBox();
            this.lblRegionCaptureSnapSizes = new System.Windows.Forms.Label();
            this.cbRegionCaptureUseCustomInfoText = new System.Windows.Forms.CheckBox();
            this.cbRegionCaptureDetectControls = new System.Windows.Forms.CheckBox();
            this.cbRegionCaptureDetectWindows = new System.Windows.Forms.CheckBox();
            this.cbRegionCaptureMouse5ClickAction = new System.Windows.Forms.ComboBox();
            this.lblRegionCaptureMouse5ClickAction = new System.Windows.Forms.Label();
            this.cbRegionCaptureMouse4ClickAction = new System.Windows.Forms.ComboBox();
            this.lblRegionCaptureMouse4ClickAction = new System.Windows.Forms.Label();
            this.cbRegionCaptureMouseMiddleClickAction = new System.Windows.Forms.ComboBox();
            this.lblRegionCaptureMouseMiddleClickAction = new System.Windows.Forms.Label();
            this.cbRegionCaptureMouseRightClickAction = new System.Windows.Forms.ComboBox();
            this.lblRegionCaptureMouseRightClickAction = new System.Windows.Forms.Label();
            this.cbRegionCaptureMultiRegionMode = new System.Windows.Forms.CheckBox();
            this.pRegionCaptureSnapSizes = new System.Windows.Forms.Panel();
            this.btnRegionCaptureSnapSizesDialogCancel = new System.Windows.Forms.Button();
            this.btnRegionCaptureSnapSizesDialogAdd = new System.Windows.Forms.Button();
            this.nudRegionCaptureSnapSizesHeight = new System.Windows.Forms.NumericUpDown();
            this.RegionCaptureSnapSizesHeight = new System.Windows.Forms.Label();
            this.nudRegionCaptureSnapSizesWidth = new System.Windows.Forms.NumericUpDown();
            this.lblRegionCaptureSnapSizesWidth = new System.Windows.Forms.Label();
            this.txtRegionCaptureCustomInfoText = new System.Windows.Forms.TextBox();
            this.nudRegionCaptureMagnifierPixelCount = new System.Windows.Forms.NumericUpDown();
            this.nudRegionCaptureMagnifierPixelSize = new System.Windows.Forms.NumericUpDown();
            this.tpScreenRecorder = new System.Windows.Forms.TabPage();
            this.cbScreenRecordTransparentRegion = new System.Windows.Forms.CheckBox();
            this.cbScreenRecordTwoPassEncoding = new System.Windows.Forms.CheckBox();
            this.cbScreenRecordConfirmAbort = new System.Windows.Forms.CheckBox();
            this.cbScreenRecorderShowCursor = new System.Windows.Forms.CheckBox();
            this.btnScreenRecorderFFmpegOptions = new System.Windows.Forms.Button();
            this.lblScreenRecorderStartDelay = new System.Windows.Forms.Label();
            this.cbScreenRecordAutoStart = new System.Windows.Forms.CheckBox();
            this.lblScreenRecorderFixedDuration = new System.Windows.Forms.Label();
            this.nudScreenRecordFPS = new System.Windows.Forms.NumericUpDown();
            this.lblScreenRecordFPS = new System.Windows.Forms.Label();
            this.nudScreenRecorderDuration = new System.Windows.Forms.NumericUpDown();
            this.nudScreenRecorderStartDelay = new System.Windows.Forms.NumericUpDown();
            this.cbScreenRecorderFixedDuration = new System.Windows.Forms.CheckBox();
            this.nudGIFFPS = new System.Windows.Forms.NumericUpDown();
            this.lblGIFFPS = new System.Windows.Forms.Label();
            this.tpOCR = new System.Windows.Forms.TabPage();
            this.btnCaptureOCRHelp = new System.Windows.Forms.Button();
            this.cbCaptureOCRAutoCopy = new System.Windows.Forms.CheckBox();
            this.cbCloseWindowAfterOpenServiceLink = new System.Windows.Forms.CheckBox();
            this.cbCaptureOCRSilent = new System.Windows.Forms.CheckBox();
            this.lblOCRDefaultLanguage = new System.Windows.Forms.Label();
            this.cbCaptureOCRDefaultLanguage = new System.Windows.Forms.ComboBox();
            this.tpUpload = new System.Windows.Forms.TabPage();
            this.tcUpload = new System.Windows.Forms.TabControl();
            this.tpUploadMain = new System.Windows.Forms.TabPage();
            this.cbOverrideUploadSettings = new System.Windows.Forms.CheckBox();
            this.tpFileNaming = new System.Windows.Forms.TabPage();
            this.txtURLRegexReplaceReplacement = new System.Windows.Forms.TextBox();
            this.lblURLRegexReplaceReplacement = new System.Windows.Forms.Label();
            this.txtURLRegexReplacePattern = new System.Windows.Forms.TextBox();
            this.lblURLRegexReplacePattern = new System.Windows.Forms.Label();
            this.cbURLRegexReplace = new System.Windows.Forms.CheckBox();
            this.btnAutoIncrementNumber = new System.Windows.Forms.Button();
            this.lblAutoIncrementNumber = new System.Windows.Forms.Label();
            this.nudAutoIncrementNumber = new System.Windows.Forms.NumericUpDown();
            this.cbFileUploadReplaceProblematicCharacters = new System.Windows.Forms.CheckBox();
            this.cbNameFormatCustomTimeZone = new System.Windows.Forms.CheckBox();
            this.lblNameFormatPatternPreview = new System.Windows.Forms.Label();
            this.lblNameFormatPatternActiveWindow = new System.Windows.Forms.Label();
            this.lblNameFormatPatternPreviewActiveWindow = new System.Windows.Forms.Label();
            this.cbNameFormatTimeZone = new System.Windows.Forms.ComboBox();
            this.txtNameFormatPatternActiveWindow = new System.Windows.Forms.TextBox();
            this.cbFileUploadUseNamePattern = new System.Windows.Forms.CheckBox();
            this.lblNameFormatPattern = new System.Windows.Forms.Label();
            this.txtNameFormatPattern = new System.Windows.Forms.TextBox();
            this.tpUploadClipboard = new System.Windows.Forms.TabPage();
            this.cbClipboardUploadShareURL = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadURLContents = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadAutoIndexFolder = new System.Windows.Forms.CheckBox();
            this.cbClipboardUploadShortenURL = new System.Windows.Forms.CheckBox();
            this.tpUploaderFilters = new System.Windows.Forms.TabPage();
            this.lvUploaderFiltersList = new ShareX.HelpersLib.MyListView();
            this.chUploaderFiltersName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chUploaderFiltersExtension = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnUploaderFiltersRemove = new System.Windows.Forms.Button();
            this.btnUploaderFiltersUpdate = new System.Windows.Forms.Button();
            this.btnUploaderFiltersAdd = new System.Windows.Forms.Button();
            this.lblUploaderFiltersDestination = new System.Windows.Forms.Label();
            this.cbUploaderFiltersDestination = new System.Windows.Forms.ComboBox();
            this.lblUploaderFiltersExtensionsExample = new System.Windows.Forms.Label();
            this.lblUploaderFiltersExtensions = new System.Windows.Forms.Label();
            this.txtUploaderFiltersExtensions = new System.Windows.Forms.TextBox();
            this.tpActions = new System.Windows.Forms.TabPage();
            this.pActions = new System.Windows.Forms.Panel();
            this.btnActions = new System.Windows.Forms.Button();
            this.lblActionsNote = new System.Windows.Forms.Label();
            this.btnActionsDuplicate = new System.Windows.Forms.Button();
            this.btnActionsAdd = new System.Windows.Forms.Button();
            this.lvActions = new ShareX.HelpersLib.MyListView();
            this.chActionsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsArgs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chActionsExtensions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnActionsEdit = new System.Windows.Forms.Button();
            this.btnActionsRemove = new System.Windows.Forms.Button();
            this.cbOverrideActions = new System.Windows.Forms.CheckBox();
            this.tpWatchFolders = new System.Windows.Forms.TabPage();
            this.btnWatchFolderEdit = new System.Windows.Forms.Button();
            this.cbWatchFolderEnabled = new System.Windows.Forms.CheckBox();
            this.lvWatchFolderList = new ShareX.HelpersLib.MyListView();
            this.chWatchFolderFolderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWatchFolderFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWatchFolderIncludeSubdirectories = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnWatchFolderRemove = new System.Windows.Forms.Button();
            this.btnWatchFolderAdd = new System.Windows.Forms.Button();
            this.tpTools = new System.Windows.Forms.TabPage();
            this.pTools = new System.Windows.Forms.Panel();
            this.txtToolsScreenColorPickerFormatCtrl = new System.Windows.Forms.TextBox();
            this.lblToolsScreenColorPickerFormatCtrl = new System.Windows.Forms.Label();
            this.txtToolsScreenColorPickerInfoText = new System.Windows.Forms.TextBox();
            this.lblToolsScreenColorPickerInfoText = new System.Windows.Forms.Label();
            this.txtToolsScreenColorPickerFormat = new System.Windows.Forms.TextBox();
            this.lblToolsScreenColorPickerFormat = new System.Windows.Forms.Label();
            this.cbOverrideToolsSettings = new System.Windows.Forms.CheckBox();
            this.tpAdvanced = new System.Windows.Forms.TabPage();
            this.pgTaskSettings = new System.Windows.Forms.PropertyGrid();
            this.cbOverrideAdvancedSettings = new System.Windows.Forms.CheckBox();
            this.tttvMain = new ShareX.HelpersLib.TabToTreeView();
            this.tcTaskSettings.SuspendLayout();
            this.tpTask.SuspendLayout();
            this.cmsDestinations.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.tcGeneral.SuspendLayout();
            this.tpGeneralMain.SuspendLayout();
            this.tpNotifications.SuspendLayout();
            this.gbToastWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowSizeHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowSizeWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowFadeDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowDuration)).BeginInit();
            this.tpImage.SuspendLayout();
            this.tcImage.SuspendLayout();
            this.tpQuality.SuspendLayout();
            this.pImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageAutoUseJPEGSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageJPEGQuality)).BeginInit();
            this.tpEffects.SuspendLayout();
            this.tpThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailWidth)).BeginInit();
            this.tpCapture.SuspendLayout();
            this.tcCapture.SuspendLayout();
            this.tpCaptureGeneral.SuspendLayout();
            this.pCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenshotDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureShadowOffset)).BeginInit();
            this.tpRegionCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureBackgroundDimStrength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureFPSLimit)).BeginInit();
            this.flpRegionCaptureFixedSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureFixedSizeWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureFixedSizeHeight)).BeginInit();
            this.pRegionCaptureSnapSizes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureSnapSizesHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureSnapSizesWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureMagnifierPixelCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureMagnifierPixelSize)).BeginInit();
            this.tpScreenRecorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecordFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGIFFPS)).BeginInit();
            this.tpOCR.SuspendLayout();
            this.tpUpload.SuspendLayout();
            this.tcUpload.SuspendLayout();
            this.tpUploadMain.SuspendLayout();
            this.tpFileNaming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoIncrementNumber)).BeginInit();
            this.tpUploadClipboard.SuspendLayout();
            this.tpUploaderFilters.SuspendLayout();
            this.tpActions.SuspendLayout();
            this.pActions.SuspendLayout();
            this.tpWatchFolders.SuspendLayout();
            this.tpTools.SuspendLayout();
            this.pTools.SuspendLayout();
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
            // cbOverrideAfterCaptureSettings
            // 
            resources.ApplyResources(this.cbOverrideAfterCaptureSettings, "cbOverrideAfterCaptureSettings");
            this.cbOverrideAfterCaptureSettings.Name = "cbOverrideAfterCaptureSettings";
            this.cbOverrideAfterCaptureSettings.UseVisualStyleBackColor = true;
            this.cbOverrideAfterCaptureSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAfterCaptureSettings_CheckedChanged);
            // 
            // cbOverrideAfterUploadSettings
            // 
            resources.ApplyResources(this.cbOverrideAfterUploadSettings, "cbOverrideAfterUploadSettings");
            this.cbOverrideAfterUploadSettings.Name = "cbOverrideAfterUploadSettings";
            this.cbOverrideAfterUploadSettings.UseVisualStyleBackColor = true;
            this.cbOverrideAfterUploadSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAfterUploadSettings_CheckedChanged);
            // 
            // cbOverrideDestinationSettings
            // 
            resources.ApplyResources(this.cbOverrideDestinationSettings, "cbOverrideDestinationSettings");
            this.cbOverrideDestinationSettings.Name = "cbOverrideDestinationSettings";
            this.cbOverrideDestinationSettings.UseVisualStyleBackColor = true;
            this.cbOverrideDestinationSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultDestinationSettings_CheckedChanged);
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
            this.tcTaskSettings.Controls.Add(this.tpUpload);
            this.tcTaskSettings.Controls.Add(this.tpActions);
            this.tcTaskSettings.Controls.Add(this.tpWatchFolders);
            this.tcTaskSettings.Controls.Add(this.tpTools);
            this.tcTaskSettings.Controls.Add(this.tpAdvanced);
            resources.ApplyResources(this.tcTaskSettings, "tcTaskSettings");
            this.tcTaskSettings.Name = "tcTaskSettings";
            this.tcTaskSettings.SelectedIndex = 0;
            // 
            // tpTask
            // 
            this.tpTask.BackColor = System.Drawing.SystemColors.Window;
            this.tpTask.Controls.Add(this.lblTask);
            this.tpTask.Controls.Add(this.btnScreenshotsFolderBrowse);
            this.tpTask.Controls.Add(this.txtScreenshotsFolder);
            this.tpTask.Controls.Add(this.cbOverrideScreenshotsFolder);
            this.tpTask.Controls.Add(this.cbCustomUploaders);
            this.tpTask.Controls.Add(this.cbOverrideCustomUploader);
            this.tpTask.Controls.Add(this.cbOverrideFTPAccount);
            this.tpTask.Controls.Add(this.cbFTPAccounts);
            this.tpTask.Controls.Add(this.tbDescription);
            this.tpTask.Controls.Add(this.btnAfterCapture);
            this.tpTask.Controls.Add(this.btnAfterUpload);
            this.tpTask.Controls.Add(this.btnDestinations);
            this.tpTask.Controls.Add(this.cbOverrideAfterCaptureSettings);
            this.tpTask.Controls.Add(this.btnTask);
            this.tpTask.Controls.Add(this.cbOverrideAfterUploadSettings);
            this.tpTask.Controls.Add(this.cbOverrideDestinationSettings);
            this.tpTask.Controls.Add(this.lblDescription);
            resources.ApplyResources(this.tpTask, "tpTask");
            this.tpTask.Name = "tpTask";
            // 
            // lblTask
            // 
            resources.ApplyResources(this.lblTask, "lblTask");
            this.lblTask.Name = "lblTask";
            // 
            // btnScreenshotsFolderBrowse
            // 
            resources.ApplyResources(this.btnScreenshotsFolderBrowse, "btnScreenshotsFolderBrowse");
            this.btnScreenshotsFolderBrowse.Name = "btnScreenshotsFolderBrowse";
            this.btnScreenshotsFolderBrowse.UseVisualStyleBackColor = true;
            this.btnScreenshotsFolderBrowse.Click += new System.EventHandler(this.btnScreenshotsFolderBrowse_Click);
            // 
            // txtScreenshotsFolder
            // 
            resources.ApplyResources(this.txtScreenshotsFolder, "txtScreenshotsFolder");
            this.txtScreenshotsFolder.Name = "txtScreenshotsFolder";
            this.txtScreenshotsFolder.TextChanged += new System.EventHandler(this.txtScreenshotsFolder_TextChanged);
            // 
            // cbOverrideScreenshotsFolder
            // 
            resources.ApplyResources(this.cbOverrideScreenshotsFolder, "cbOverrideScreenshotsFolder");
            this.cbOverrideScreenshotsFolder.Name = "cbOverrideScreenshotsFolder";
            this.cbOverrideScreenshotsFolder.UseVisualStyleBackColor = true;
            this.cbOverrideScreenshotsFolder.CheckedChanged += new System.EventHandler(this.cbOverrideScreenshotsFolder_CheckedChanged);
            // 
            // cbCustomUploaders
            // 
            this.cbCustomUploaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomUploaders.FormattingEnabled = true;
            resources.ApplyResources(this.cbCustomUploaders, "cbCustomUploaders");
            this.cbCustomUploaders.Name = "cbCustomUploaders";
            this.cbCustomUploaders.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaders_SelectedIndexChanged);
            // 
            // cbOverrideCustomUploader
            // 
            resources.ApplyResources(this.cbOverrideCustomUploader, "cbOverrideCustomUploader");
            this.cbOverrideCustomUploader.Name = "cbOverrideCustomUploader";
            this.cbOverrideCustomUploader.UseVisualStyleBackColor = true;
            this.cbOverrideCustomUploader.CheckedChanged += new System.EventHandler(this.cbOverrideCustomUploader_CheckedChanged);
            // 
            // cbOverrideFTPAccount
            // 
            resources.ApplyResources(this.cbOverrideFTPAccount, "cbOverrideFTPAccount");
            this.cbOverrideFTPAccount.Name = "cbOverrideFTPAccount";
            this.cbOverrideFTPAccount.UseVisualStyleBackColor = true;
            this.cbOverrideFTPAccount.CheckedChanged += new System.EventHandler(this.cbOverrideFTPAccount_CheckedChanged);
            // 
            // cbFTPAccounts
            // 
            this.cbFTPAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFTPAccounts.FormattingEnabled = true;
            resources.ApplyResources(this.cbFTPAccounts, "cbFTPAccounts");
            this.cbFTPAccounts.Name = "cbFTPAccounts";
            this.cbFTPAccounts.SelectedIndexChanged += new System.EventHandler(this.cbFTPAccounts_SelectedIndexChanged);
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
            this.btnTask.Image = global::ShareX.Properties.Resources.gear;
            resources.ApplyResources(this.btnTask, "btnTask");
            this.btnTask.Menu = this.cmsTask;
            this.btnTask.Name = "btnTask";
            this.btnTask.UseMnemonic = false;
            this.btnTask.UseVisualStyleBackColor = true;
            // 
            // tpGeneral
            // 
            this.tpGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.tpGeneral.Controls.Add(this.tcGeneral);
            resources.ApplyResources(this.tpGeneral, "tpGeneral");
            this.tpGeneral.Name = "tpGeneral";
            // 
            // tcGeneral
            // 
            this.tcGeneral.Controls.Add(this.tpGeneralMain);
            this.tcGeneral.Controls.Add(this.tpNotifications);
            resources.ApplyResources(this.tcGeneral, "tcGeneral");
            this.tcGeneral.Name = "tcGeneral";
            this.tcGeneral.SelectedIndex = 0;
            // 
            // tpGeneralMain
            // 
            this.tpGeneralMain.Controls.Add(this.cbOverrideGeneralSettings);
            resources.ApplyResources(this.tpGeneralMain, "tpGeneralMain");
            this.tpGeneralMain.Name = "tpGeneralMain";
            this.tpGeneralMain.UseVisualStyleBackColor = true;
            // 
            // cbOverrideGeneralSettings
            // 
            resources.ApplyResources(this.cbOverrideGeneralSettings, "cbOverrideGeneralSettings");
            this.cbOverrideGeneralSettings.Checked = true;
            this.cbOverrideGeneralSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideGeneralSettings.Name = "cbOverrideGeneralSettings";
            this.cbOverrideGeneralSettings.UseVisualStyleBackColor = true;
            this.cbOverrideGeneralSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultGeneralSettings_CheckedChanged);
            // 
            // tpNotifications
            // 
            this.tpNotifications.Controls.Add(this.btnCustomActionCompletedSoundPath);
            this.tpNotifications.Controls.Add(this.txtCustomActionCompletedSoundPath);
            this.tpNotifications.Controls.Add(this.cbUseCustomActionCompletedSound);
            this.tpNotifications.Controls.Add(this.cbPlaySoundAfterAction);
            this.tpNotifications.Controls.Add(this.cbShowToastNotificationAfterTaskCompleted);
            this.tpNotifications.Controls.Add(this.btnCustomErrorSoundPath);
            this.tpNotifications.Controls.Add(this.btnCustomTaskCompletedSoundPath);
            this.tpNotifications.Controls.Add(this.btnCustomCaptureSoundPath);
            this.tpNotifications.Controls.Add(this.txtCustomErrorSoundPath);
            this.tpNotifications.Controls.Add(this.txtCustomTaskCompletedSoundPath);
            this.tpNotifications.Controls.Add(this.txtCustomCaptureSoundPath);
            this.tpNotifications.Controls.Add(this.cbUseCustomErrorSound);
            this.tpNotifications.Controls.Add(this.cbUseCustomTaskCompletedSound);
            this.tpNotifications.Controls.Add(this.cbUseCustomCaptureSound);
            this.tpNotifications.Controls.Add(this.gbToastWindow);
            this.tpNotifications.Controls.Add(this.cbPlaySoundAfterCapture);
            this.tpNotifications.Controls.Add(this.cbPlaySoundAfterUpload);
            resources.ApplyResources(this.tpNotifications, "tpNotifications");
            this.tpNotifications.Name = "tpNotifications";
            this.tpNotifications.UseVisualStyleBackColor = true;
            // 
            // btnCustomActionCompletedSoundPath
            // 
            resources.ApplyResources(this.btnCustomActionCompletedSoundPath, "btnCustomActionCompletedSoundPath");
            this.btnCustomActionCompletedSoundPath.Name = "btnCustomActionCompletedSoundPath";
            this.btnCustomActionCompletedSoundPath.UseVisualStyleBackColor = true;
            this.btnCustomActionCompletedSoundPath.Click += new System.EventHandler(this.btnCustomActionCompletedSoundPath_Click);
            // 
            // txtCustomActionCompletedSoundPath
            // 
            resources.ApplyResources(this.txtCustomActionCompletedSoundPath, "txtCustomActionCompletedSoundPath");
            this.txtCustomActionCompletedSoundPath.Name = "txtCustomActionCompletedSoundPath";
            this.txtCustomActionCompletedSoundPath.TextChanged += new System.EventHandler(this.txtCustomActionCompletedSoundPath_TextChanged);
            // 
            // cbUseCustomActionCompletedSound
            // 
            resources.ApplyResources(this.cbUseCustomActionCompletedSound, "cbUseCustomActionCompletedSound");
            this.cbUseCustomActionCompletedSound.Name = "cbUseCustomActionCompletedSound";
            this.cbUseCustomActionCompletedSound.UseVisualStyleBackColor = true;
            this.cbUseCustomActionCompletedSound.CheckedChanged += new System.EventHandler(this.cbUseCustomActionCompletedSound_CheckedChanged);
            // 
            // cbPlaySoundAfterAction
            // 
            resources.ApplyResources(this.cbPlaySoundAfterAction, "cbPlaySoundAfterAction");
            this.cbPlaySoundAfterAction.Name = "cbPlaySoundAfterAction";
            this.cbPlaySoundAfterAction.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterAction.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterAction_CheckedChanged);
            // 
            // cbShowToastNotificationAfterTaskCompleted
            // 
            resources.ApplyResources(this.cbShowToastNotificationAfterTaskCompleted, "cbShowToastNotificationAfterTaskCompleted");
            this.cbShowToastNotificationAfterTaskCompleted.Name = "cbShowToastNotificationAfterTaskCompleted";
            this.cbShowToastNotificationAfterTaskCompleted.UseVisualStyleBackColor = true;
            this.cbShowToastNotificationAfterTaskCompleted.CheckedChanged += new System.EventHandler(this.cbShowToastNotificationAfterTaskCompleted_CheckedChanged);
            // 
            // btnCustomErrorSoundPath
            // 
            resources.ApplyResources(this.btnCustomErrorSoundPath, "btnCustomErrorSoundPath");
            this.btnCustomErrorSoundPath.Name = "btnCustomErrorSoundPath";
            this.btnCustomErrorSoundPath.UseVisualStyleBackColor = true;
            this.btnCustomErrorSoundPath.Click += new System.EventHandler(this.btnCustomErrorSoundPath_Click);
            // 
            // btnCustomTaskCompletedSoundPath
            // 
            resources.ApplyResources(this.btnCustomTaskCompletedSoundPath, "btnCustomTaskCompletedSoundPath");
            this.btnCustomTaskCompletedSoundPath.Name = "btnCustomTaskCompletedSoundPath";
            this.btnCustomTaskCompletedSoundPath.UseVisualStyleBackColor = true;
            this.btnCustomTaskCompletedSoundPath.Click += new System.EventHandler(this.btnCustomTaskCompletedSoundPath_Click);
            // 
            // btnCustomCaptureSoundPath
            // 
            resources.ApplyResources(this.btnCustomCaptureSoundPath, "btnCustomCaptureSoundPath");
            this.btnCustomCaptureSoundPath.Name = "btnCustomCaptureSoundPath";
            this.btnCustomCaptureSoundPath.UseVisualStyleBackColor = true;
            this.btnCustomCaptureSoundPath.Click += new System.EventHandler(this.btnCustomCaptureSoundPath_Click);
            // 
            // txtCustomErrorSoundPath
            // 
            resources.ApplyResources(this.txtCustomErrorSoundPath, "txtCustomErrorSoundPath");
            this.txtCustomErrorSoundPath.Name = "txtCustomErrorSoundPath";
            this.txtCustomErrorSoundPath.TextChanged += new System.EventHandler(this.txtCustomErrorSoundPath_TextChanged);
            // 
            // txtCustomTaskCompletedSoundPath
            // 
            resources.ApplyResources(this.txtCustomTaskCompletedSoundPath, "txtCustomTaskCompletedSoundPath");
            this.txtCustomTaskCompletedSoundPath.Name = "txtCustomTaskCompletedSoundPath";
            this.txtCustomTaskCompletedSoundPath.TextChanged += new System.EventHandler(this.txtCustomTaskCompletedSoundPath_TextChanged);
            // 
            // txtCustomCaptureSoundPath
            // 
            resources.ApplyResources(this.txtCustomCaptureSoundPath, "txtCustomCaptureSoundPath");
            this.txtCustomCaptureSoundPath.Name = "txtCustomCaptureSoundPath";
            this.txtCustomCaptureSoundPath.TextChanged += new System.EventHandler(this.txtCustomCaptureSoundPath_TextChanged);
            // 
            // cbUseCustomErrorSound
            // 
            resources.ApplyResources(this.cbUseCustomErrorSound, "cbUseCustomErrorSound");
            this.cbUseCustomErrorSound.Name = "cbUseCustomErrorSound";
            this.cbUseCustomErrorSound.UseVisualStyleBackColor = true;
            this.cbUseCustomErrorSound.CheckedChanged += new System.EventHandler(this.cbUseCustomErrorSound_CheckedChanged);
            // 
            // cbUseCustomTaskCompletedSound
            // 
            resources.ApplyResources(this.cbUseCustomTaskCompletedSound, "cbUseCustomTaskCompletedSound");
            this.cbUseCustomTaskCompletedSound.Name = "cbUseCustomTaskCompletedSound";
            this.cbUseCustomTaskCompletedSound.UseVisualStyleBackColor = true;
            this.cbUseCustomTaskCompletedSound.CheckedChanged += new System.EventHandler(this.cbUseCustomTaskCompletedSound_CheckedChanged);
            // 
            // cbUseCustomCaptureSound
            // 
            resources.ApplyResources(this.cbUseCustomCaptureSound, "cbUseCustomCaptureSound");
            this.cbUseCustomCaptureSound.Name = "cbUseCustomCaptureSound";
            this.cbUseCustomCaptureSound.UseVisualStyleBackColor = true;
            this.cbUseCustomCaptureSound.CheckedChanged += new System.EventHandler(this.cbUseCustomCaptureSound_CheckedChanged);
            // 
            // gbToastWindow
            // 
            this.gbToastWindow.Controls.Add(this.cbToastWindowAutoHide);
            this.gbToastWindow.Controls.Add(this.lblToastWindowFadeDurationSeconds);
            this.gbToastWindow.Controls.Add(this.lblToastWindowDurationSeconds);
            this.gbToastWindow.Controls.Add(this.lblToastWindowSizeX);
            this.gbToastWindow.Controls.Add(this.cbToastWindowMiddleClickAction);
            this.gbToastWindow.Controls.Add(this.cbToastWindowRightClickAction);
            this.gbToastWindow.Controls.Add(this.cbToastWindowLeftClickAction);
            this.gbToastWindow.Controls.Add(this.nudToastWindowSizeHeight);
            this.gbToastWindow.Controls.Add(this.nudToastWindowSizeWidth);
            this.gbToastWindow.Controls.Add(this.cbToastWindowPlacement);
            this.gbToastWindow.Controls.Add(this.nudToastWindowFadeDuration);
            this.gbToastWindow.Controls.Add(this.cbDisableNotificationsOnFullscreen);
            this.gbToastWindow.Controls.Add(this.nudToastWindowDuration);
            this.gbToastWindow.Controls.Add(this.lblToastWindowMiddleClickAction);
            this.gbToastWindow.Controls.Add(this.lblToastWindowRightClickAction);
            this.gbToastWindow.Controls.Add(this.lblToastWindowLeftClickAction);
            this.gbToastWindow.Controls.Add(this.lblToastWindowSize);
            this.gbToastWindow.Controls.Add(this.lblToastWindowPlacement);
            this.gbToastWindow.Controls.Add(this.lblToastWindowFadeDuration);
            this.gbToastWindow.Controls.Add(this.lblToastWindowDuration);
            resources.ApplyResources(this.gbToastWindow, "gbToastWindow");
            this.gbToastWindow.Name = "gbToastWindow";
            this.gbToastWindow.TabStop = false;
            // 
            // cbToastWindowAutoHide
            // 
            resources.ApplyResources(this.cbToastWindowAutoHide, "cbToastWindowAutoHide");
            this.cbToastWindowAutoHide.Name = "cbToastWindowAutoHide";
            this.cbToastWindowAutoHide.UseVisualStyleBackColor = true;
            this.cbToastWindowAutoHide.CheckedChanged += new System.EventHandler(this.cbToastWindowAutoHide_CheckedChanged);
            // 
            // lblToastWindowFadeDurationSeconds
            // 
            resources.ApplyResources(this.lblToastWindowFadeDurationSeconds, "lblToastWindowFadeDurationSeconds");
            this.lblToastWindowFadeDurationSeconds.Name = "lblToastWindowFadeDurationSeconds";
            // 
            // lblToastWindowDurationSeconds
            // 
            resources.ApplyResources(this.lblToastWindowDurationSeconds, "lblToastWindowDurationSeconds");
            this.lblToastWindowDurationSeconds.Name = "lblToastWindowDurationSeconds";
            // 
            // lblToastWindowSizeX
            // 
            resources.ApplyResources(this.lblToastWindowSizeX, "lblToastWindowSizeX");
            this.lblToastWindowSizeX.Name = "lblToastWindowSizeX";
            // 
            // cbToastWindowMiddleClickAction
            // 
            this.cbToastWindowMiddleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToastWindowMiddleClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbToastWindowMiddleClickAction, "cbToastWindowMiddleClickAction");
            this.cbToastWindowMiddleClickAction.Name = "cbToastWindowMiddleClickAction";
            this.cbToastWindowMiddleClickAction.SelectedIndexChanged += new System.EventHandler(this.cbToastWindowMiddleClickAction_SelectedIndexChanged);
            // 
            // cbToastWindowRightClickAction
            // 
            this.cbToastWindowRightClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToastWindowRightClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbToastWindowRightClickAction, "cbToastWindowRightClickAction");
            this.cbToastWindowRightClickAction.Name = "cbToastWindowRightClickAction";
            this.cbToastWindowRightClickAction.SelectedIndexChanged += new System.EventHandler(this.cbToastWindowRightClickAction_SelectedIndexChanged);
            // 
            // cbToastWindowLeftClickAction
            // 
            this.cbToastWindowLeftClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToastWindowLeftClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbToastWindowLeftClickAction, "cbToastWindowLeftClickAction");
            this.cbToastWindowLeftClickAction.Name = "cbToastWindowLeftClickAction";
            this.cbToastWindowLeftClickAction.SelectedIndexChanged += new System.EventHandler(this.cbToastWindowLeftClickAction_SelectedIndexChanged);
            // 
            // nudToastWindowSizeHeight
            // 
            resources.ApplyResources(this.nudToastWindowSizeHeight, "nudToastWindowSizeHeight");
            this.nudToastWindowSizeHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudToastWindowSizeHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudToastWindowSizeHeight.Name = "nudToastWindowSizeHeight";
            this.nudToastWindowSizeHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudToastWindowSizeHeight.ValueChanged += new System.EventHandler(this.nudToastWindowSizeHeight_ValueChanged);
            // 
            // nudToastWindowSizeWidth
            // 
            resources.ApplyResources(this.nudToastWindowSizeWidth, "nudToastWindowSizeWidth");
            this.nudToastWindowSizeWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudToastWindowSizeWidth.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudToastWindowSizeWidth.Name = "nudToastWindowSizeWidth";
            this.nudToastWindowSizeWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudToastWindowSizeWidth.ValueChanged += new System.EventHandler(this.nudToastWindowSizeWidth_ValueChanged);
            // 
            // cbToastWindowPlacement
            // 
            this.cbToastWindowPlacement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToastWindowPlacement.FormattingEnabled = true;
            resources.ApplyResources(this.cbToastWindowPlacement, "cbToastWindowPlacement");
            this.cbToastWindowPlacement.Name = "cbToastWindowPlacement";
            this.cbToastWindowPlacement.SelectedIndexChanged += new System.EventHandler(this.cbToastWindowPlacement_SelectedIndexChanged);
            // 
            // nudToastWindowFadeDuration
            // 
            this.nudToastWindowFadeDuration.DecimalPlaces = 1;
            resources.ApplyResources(this.nudToastWindowFadeDuration, "nudToastWindowFadeDuration");
            this.nudToastWindowFadeDuration.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudToastWindowFadeDuration.Name = "nudToastWindowFadeDuration";
            this.nudToastWindowFadeDuration.ValueChanged += new System.EventHandler(this.nudToastWindowFadeDuration_ValueChanged);
            // 
            // cbDisableNotificationsOnFullscreen
            // 
            resources.ApplyResources(this.cbDisableNotificationsOnFullscreen, "cbDisableNotificationsOnFullscreen");
            this.cbDisableNotificationsOnFullscreen.Name = "cbDisableNotificationsOnFullscreen";
            this.cbDisableNotificationsOnFullscreen.UseVisualStyleBackColor = true;
            this.cbDisableNotificationsOnFullscreen.CheckedChanged += new System.EventHandler(this.cbDisableNotificationsOnFullscreen_CheckedChanged);
            // 
            // nudToastWindowDuration
            // 
            this.nudToastWindowDuration.DecimalPlaces = 1;
            resources.ApplyResources(this.nudToastWindowDuration, "nudToastWindowDuration");
            this.nudToastWindowDuration.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudToastWindowDuration.Name = "nudToastWindowDuration";
            this.nudToastWindowDuration.ValueChanged += new System.EventHandler(this.nudToastWindowDuration_ValueChanged);
            // 
            // lblToastWindowMiddleClickAction
            // 
            resources.ApplyResources(this.lblToastWindowMiddleClickAction, "lblToastWindowMiddleClickAction");
            this.lblToastWindowMiddleClickAction.Name = "lblToastWindowMiddleClickAction";
            // 
            // lblToastWindowRightClickAction
            // 
            resources.ApplyResources(this.lblToastWindowRightClickAction, "lblToastWindowRightClickAction");
            this.lblToastWindowRightClickAction.Name = "lblToastWindowRightClickAction";
            // 
            // lblToastWindowLeftClickAction
            // 
            resources.ApplyResources(this.lblToastWindowLeftClickAction, "lblToastWindowLeftClickAction");
            this.lblToastWindowLeftClickAction.Name = "lblToastWindowLeftClickAction";
            // 
            // lblToastWindowSize
            // 
            resources.ApplyResources(this.lblToastWindowSize, "lblToastWindowSize");
            this.lblToastWindowSize.Name = "lblToastWindowSize";
            // 
            // lblToastWindowPlacement
            // 
            resources.ApplyResources(this.lblToastWindowPlacement, "lblToastWindowPlacement");
            this.lblToastWindowPlacement.Name = "lblToastWindowPlacement";
            // 
            // lblToastWindowFadeDuration
            // 
            resources.ApplyResources(this.lblToastWindowFadeDuration, "lblToastWindowFadeDuration");
            this.lblToastWindowFadeDuration.Name = "lblToastWindowFadeDuration";
            // 
            // lblToastWindowDuration
            // 
            resources.ApplyResources(this.lblToastWindowDuration, "lblToastWindowDuration");
            this.lblToastWindowDuration.Name = "lblToastWindowDuration";
            // 
            // cbPlaySoundAfterCapture
            // 
            resources.ApplyResources(this.cbPlaySoundAfterCapture, "cbPlaySoundAfterCapture");
            this.cbPlaySoundAfterCapture.Name = "cbPlaySoundAfterCapture";
            this.cbPlaySoundAfterCapture.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterCapture.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterCapture_CheckedChanged);
            // 
            // cbPlaySoundAfterUpload
            // 
            resources.ApplyResources(this.cbPlaySoundAfterUpload, "cbPlaySoundAfterUpload");
            this.cbPlaySoundAfterUpload.Name = "cbPlaySoundAfterUpload";
            this.cbPlaySoundAfterUpload.UseVisualStyleBackColor = true;
            this.cbPlaySoundAfterUpload.CheckedChanged += new System.EventHandler(this.cbPlaySoundAfterUpload_CheckedChanged);
            // 
            // tpImage
            // 
            this.tpImage.BackColor = System.Drawing.SystemColors.Window;
            this.tpImage.Controls.Add(this.tcImage);
            resources.ApplyResources(this.tpImage, "tpImage");
            this.tpImage.Name = "tpImage";
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
            this.tpQuality.BackColor = System.Drawing.SystemColors.Window;
            this.tpQuality.Controls.Add(this.pImage);
            this.tpQuality.Controls.Add(this.cbOverrideImageSettings);
            resources.ApplyResources(this.tpQuality, "tpQuality");
            this.tpQuality.Name = "tpQuality";
            // 
            // pImage
            // 
            this.pImage.Controls.Add(this.cbImageAutoJPEGQuality);
            this.pImage.Controls.Add(this.cbImagePNGBitDepth);
            this.pImage.Controls.Add(this.lblImagePNGBitDepth);
            this.pImage.Controls.Add(this.cbImageAutoUseJPEG);
            this.pImage.Controls.Add(this.lblImageFormat);
            this.pImage.Controls.Add(this.cbImageFileExist);
            this.pImage.Controls.Add(this.lblImageFileExist);
            this.pImage.Controls.Add(this.nudImageAutoUseJPEGSize);
            this.pImage.Controls.Add(this.lblImageSizeLimitHint);
            this.pImage.Controls.Add(this.nudImageJPEGQuality);
            this.pImage.Controls.Add(this.cbImageFormat);
            this.pImage.Controls.Add(this.lblImageJPEGQualityHint);
            this.pImage.Controls.Add(this.lblImageGIFQuality);
            this.pImage.Controls.Add(this.lblImageJPEGQuality);
            this.pImage.Controls.Add(this.cbImageGIFQuality);
            resources.ApplyResources(this.pImage, "pImage");
            this.pImage.Name = "pImage";
            // 
            // cbImageAutoJPEGQuality
            // 
            resources.ApplyResources(this.cbImageAutoJPEGQuality, "cbImageAutoJPEGQuality");
            this.cbImageAutoJPEGQuality.Name = "cbImageAutoJPEGQuality";
            this.cbImageAutoJPEGQuality.UseVisualStyleBackColor = true;
            this.cbImageAutoJPEGQuality.CheckedChanged += new System.EventHandler(this.cbImageAutoJPEGQuality_CheckedChanged);
            // 
            // cbImagePNGBitDepth
            // 
            this.cbImagePNGBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImagePNGBitDepth.FormattingEnabled = true;
            resources.ApplyResources(this.cbImagePNGBitDepth, "cbImagePNGBitDepth");
            this.cbImagePNGBitDepth.Name = "cbImagePNGBitDepth";
            this.cbImagePNGBitDepth.SelectedIndexChanged += new System.EventHandler(this.cbImagePNGBitDepth_SelectedIndexChanged);
            // 
            // lblImagePNGBitDepth
            // 
            resources.ApplyResources(this.lblImagePNGBitDepth, "lblImagePNGBitDepth");
            this.lblImagePNGBitDepth.Name = "lblImagePNGBitDepth";
            // 
            // cbImageAutoUseJPEG
            // 
            resources.ApplyResources(this.cbImageAutoUseJPEG, "cbImageAutoUseJPEG");
            this.cbImageAutoUseJPEG.Name = "cbImageAutoUseJPEG";
            this.cbImageAutoUseJPEG.UseVisualStyleBackColor = true;
            this.cbImageAutoUseJPEG.CheckedChanged += new System.EventHandler(this.cbImageAutoUseJPEG_CheckedChanged);
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
            // lblImageFileExist
            // 
            resources.ApplyResources(this.lblImageFileExist, "lblImageFileExist");
            this.lblImageFileExist.Name = "lblImageFileExist";
            // 
            // nudImageAutoUseJPEGSize
            // 
            resources.ApplyResources(this.nudImageAutoUseJPEGSize, "nudImageAutoUseJPEGSize");
            this.nudImageAutoUseJPEGSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudImageAutoUseJPEGSize.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudImageAutoUseJPEGSize.Name = "nudImageAutoUseJPEGSize";
            this.nudImageAutoUseJPEGSize.Value = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.nudImageAutoUseJPEGSize.ValueChanged += new System.EventHandler(this.nudImageAutoUseJPEGSize_ValueChanged);
            // 
            // lblImageSizeLimitHint
            // 
            resources.ApplyResources(this.lblImageSizeLimitHint, "lblImageSizeLimitHint");
            this.lblImageSizeLimitHint.Name = "lblImageSizeLimitHint";
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
            // cbOverrideImageSettings
            // 
            resources.ApplyResources(this.cbOverrideImageSettings, "cbOverrideImageSettings");
            this.cbOverrideImageSettings.Checked = true;
            this.cbOverrideImageSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideImageSettings.Name = "cbOverrideImageSettings";
            this.cbOverrideImageSettings.UseVisualStyleBackColor = true;
            this.cbOverrideImageSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultImageSettings_CheckedChanged);
            // 
            // tpEffects
            // 
            this.tpEffects.BackColor = System.Drawing.SystemColors.Window;
            this.tpEffects.Controls.Add(this.cbUseRandomImageEffect);
            this.tpEffects.Controls.Add(this.lblImageEffectsNote);
            this.tpEffects.Controls.Add(this.cbShowImageEffectsWindowAfterCapture);
            this.tpEffects.Controls.Add(this.cbImageEffectOnlyRegionCapture);
            this.tpEffects.Controls.Add(this.btnImageEffects);
            resources.ApplyResources(this.tpEffects, "tpEffects");
            this.tpEffects.Name = "tpEffects";
            // 
            // cbUseRandomImageEffect
            // 
            resources.ApplyResources(this.cbUseRandomImageEffect, "cbUseRandomImageEffect");
            this.cbUseRandomImageEffect.Name = "cbUseRandomImageEffect";
            this.cbUseRandomImageEffect.UseVisualStyleBackColor = true;
            this.cbUseRandomImageEffect.CheckedChanged += new System.EventHandler(this.cbUseRandomImageEffect_CheckedChanged);
            // 
            // lblImageEffectsNote
            // 
            resources.ApplyResources(this.lblImageEffectsNote, "lblImageEffectsNote");
            this.lblImageEffectsNote.Name = "lblImageEffectsNote";
            // 
            // cbShowImageEffectsWindowAfterCapture
            // 
            resources.ApplyResources(this.cbShowImageEffectsWindowAfterCapture, "cbShowImageEffectsWindowAfterCapture");
            this.cbShowImageEffectsWindowAfterCapture.Name = "cbShowImageEffectsWindowAfterCapture";
            this.cbShowImageEffectsWindowAfterCapture.UseVisualStyleBackColor = true;
            this.cbShowImageEffectsWindowAfterCapture.CheckedChanged += new System.EventHandler(this.cbShowImageEffectsWindowAfterCapture_CheckedChanged);
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
            this.tpThumbnail.BackColor = System.Drawing.SystemColors.Window;
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
            this.tpCapture.BackColor = System.Drawing.SystemColors.Window;
            this.tpCapture.Controls.Add(this.tcCapture);
            resources.ApplyResources(this.tpCapture, "tpCapture");
            this.tpCapture.Name = "tpCapture";
            // 
            // tcCapture
            // 
            this.tcCapture.Controls.Add(this.tpCaptureGeneral);
            this.tcCapture.Controls.Add(this.tpRegionCapture);
            this.tcCapture.Controls.Add(this.tpScreenRecorder);
            this.tcCapture.Controls.Add(this.tpOCR);
            resources.ApplyResources(this.tcCapture, "tcCapture");
            this.tcCapture.Name = "tcCapture";
            this.tcCapture.SelectedIndex = 0;
            // 
            // tpCaptureGeneral
            // 
            this.tpCaptureGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.tpCaptureGeneral.Controls.Add(this.pCapture);
            this.tpCaptureGeneral.Controls.Add(this.cbOverrideCaptureSettings);
            resources.ApplyResources(this.tpCaptureGeneral, "tpCaptureGeneral");
            this.tpCaptureGeneral.Name = "tpCaptureGeneral";
            // 
            // pCapture
            // 
            this.pCapture.Controls.Add(this.txtCaptureCustomWindow);
            this.pCapture.Controls.Add(this.lblCaptureCustomWindow);
            this.pCapture.Controls.Add(this.lblScreenshotDelay);
            this.pCapture.Controls.Add(this.btnCaptureCustomRegionSelectRectangle);
            this.pCapture.Controls.Add(this.lblCaptureCustomRegion);
            this.pCapture.Controls.Add(this.lblCaptureCustomRegionWidth);
            this.pCapture.Controls.Add(this.lblCaptureCustomRegionHeight);
            this.pCapture.Controls.Add(this.lblCaptureCustomRegionY);
            this.pCapture.Controls.Add(this.lblCaptureCustomRegionX);
            this.pCapture.Controls.Add(this.nudCaptureCustomRegionHeight);
            this.pCapture.Controls.Add(this.nudCaptureCustomRegionWidth);
            this.pCapture.Controls.Add(this.nudCaptureCustomRegionY);
            this.pCapture.Controls.Add(this.nudCaptureCustomRegionX);
            this.pCapture.Controls.Add(this.cbShowCursor);
            this.pCapture.Controls.Add(this.lblCaptureShadowOffset);
            this.pCapture.Controls.Add(this.cbCaptureTransparent);
            this.pCapture.Controls.Add(this.cbCaptureAutoHideTaskbar);
            this.pCapture.Controls.Add(this.cbCaptureShadow);
            this.pCapture.Controls.Add(this.lblScreenshotDelayInfo);
            this.pCapture.Controls.Add(this.cbCaptureClientArea);
            this.pCapture.Controls.Add(this.nudScreenshotDelay);
            this.pCapture.Controls.Add(this.nudCaptureShadowOffset);
            resources.ApplyResources(this.pCapture, "pCapture");
            this.pCapture.Name = "pCapture";
            // 
            // txtCaptureCustomWindow
            // 
            resources.ApplyResources(this.txtCaptureCustomWindow, "txtCaptureCustomWindow");
            this.txtCaptureCustomWindow.Name = "txtCaptureCustomWindow";
            this.txtCaptureCustomWindow.TextChanged += new System.EventHandler(this.txtCaptureCustomWindow_TextChanged);
            // 
            // lblCaptureCustomWindow
            // 
            resources.ApplyResources(this.lblCaptureCustomWindow, "lblCaptureCustomWindow");
            this.lblCaptureCustomWindow.Name = "lblCaptureCustomWindow";
            // 
            // lblScreenshotDelay
            // 
            resources.ApplyResources(this.lblScreenshotDelay, "lblScreenshotDelay");
            this.lblScreenshotDelay.Name = "lblScreenshotDelay";
            // 
            // btnCaptureCustomRegionSelectRectangle
            // 
            resources.ApplyResources(this.btnCaptureCustomRegionSelectRectangle, "btnCaptureCustomRegionSelectRectangle");
            this.btnCaptureCustomRegionSelectRectangle.Name = "btnCaptureCustomRegionSelectRectangle";
            this.btnCaptureCustomRegionSelectRectangle.UseVisualStyleBackColor = true;
            this.btnCaptureCustomRegionSelectRectangle.Click += new System.EventHandler(this.btnCaptureCustomRegionSelectRectangle_Click);
            // 
            // lblCaptureCustomRegion
            // 
            resources.ApplyResources(this.lblCaptureCustomRegion, "lblCaptureCustomRegion");
            this.lblCaptureCustomRegion.Name = "lblCaptureCustomRegion";
            // 
            // lblCaptureCustomRegionWidth
            // 
            resources.ApplyResources(this.lblCaptureCustomRegionWidth, "lblCaptureCustomRegionWidth");
            this.lblCaptureCustomRegionWidth.Name = "lblCaptureCustomRegionWidth";
            // 
            // lblCaptureCustomRegionHeight
            // 
            resources.ApplyResources(this.lblCaptureCustomRegionHeight, "lblCaptureCustomRegionHeight");
            this.lblCaptureCustomRegionHeight.Name = "lblCaptureCustomRegionHeight";
            // 
            // lblCaptureCustomRegionY
            // 
            resources.ApplyResources(this.lblCaptureCustomRegionY, "lblCaptureCustomRegionY");
            this.lblCaptureCustomRegionY.Name = "lblCaptureCustomRegionY";
            // 
            // lblCaptureCustomRegionX
            // 
            resources.ApplyResources(this.lblCaptureCustomRegionX, "lblCaptureCustomRegionX");
            this.lblCaptureCustomRegionX.Name = "lblCaptureCustomRegionX";
            // 
            // nudCaptureCustomRegionHeight
            // 
            resources.ApplyResources(this.nudCaptureCustomRegionHeight, "nudCaptureCustomRegionHeight");
            this.nudCaptureCustomRegionHeight.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.nudCaptureCustomRegionHeight.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudCaptureCustomRegionHeight.Name = "nudCaptureCustomRegionHeight";
            this.nudCaptureCustomRegionHeight.ValueChanged += new System.EventHandler(this.nudScreenRegionHeight_ValueChanged);
            // 
            // nudCaptureCustomRegionWidth
            // 
            resources.ApplyResources(this.nudCaptureCustomRegionWidth, "nudCaptureCustomRegionWidth");
            this.nudCaptureCustomRegionWidth.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.nudCaptureCustomRegionWidth.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudCaptureCustomRegionWidth.Name = "nudCaptureCustomRegionWidth";
            this.nudCaptureCustomRegionWidth.ValueChanged += new System.EventHandler(this.nudScreenRegionWidth_ValueChanged);
            // 
            // nudCaptureCustomRegionY
            // 
            resources.ApplyResources(this.nudCaptureCustomRegionY, "nudCaptureCustomRegionY");
            this.nudCaptureCustomRegionY.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.nudCaptureCustomRegionY.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudCaptureCustomRegionY.Name = "nudCaptureCustomRegionY";
            this.nudCaptureCustomRegionY.ValueChanged += new System.EventHandler(this.nudScreenRegionY_ValueChanged);
            // 
            // nudCaptureCustomRegionX
            // 
            resources.ApplyResources(this.nudCaptureCustomRegionX, "nudCaptureCustomRegionX");
            this.nudCaptureCustomRegionX.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.nudCaptureCustomRegionX.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudCaptureCustomRegionX.Name = "nudCaptureCustomRegionX";
            this.nudCaptureCustomRegionX.ValueChanged += new System.EventHandler(this.nudScreenRegionX_ValueChanged);
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
            resources.ApplyResources(this.nudScreenshotDelay, "nudScreenshotDelay");
            this.nudScreenshotDelay.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudScreenshotDelay.Name = "nudScreenshotDelay";
            this.nudScreenshotDelay.ValueChanged += new System.EventHandler(this.nudScreenshotDelay_ValueChanged);
            // 
            // nudCaptureShadowOffset
            // 
            resources.ApplyResources(this.nudCaptureShadowOffset, "nudCaptureShadowOffset");
            this.nudCaptureShadowOffset.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudCaptureShadowOffset.Name = "nudCaptureShadowOffset";
            this.nudCaptureShadowOffset.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudCaptureShadowOffset.ValueChanged += new System.EventHandler(this.nudCaptureShadowOffset_ValueChanged);
            // 
            // cbOverrideCaptureSettings
            // 
            resources.ApplyResources(this.cbOverrideCaptureSettings, "cbOverrideCaptureSettings");
            this.cbOverrideCaptureSettings.Checked = true;
            this.cbOverrideCaptureSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideCaptureSettings.Name = "cbOverrideCaptureSettings";
            this.cbOverrideCaptureSettings.UseVisualStyleBackColor = true;
            this.cbOverrideCaptureSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultCaptureSettings_CheckedChanged);
            // 
            // tpRegionCapture
            // 
            this.tpRegionCapture.BackColor = System.Drawing.SystemColors.Window;
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureBackgroundDimStrengthHint);
            this.tpRegionCapture.Controls.Add(this.nudRegionCaptureBackgroundDimStrength);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureBackgroundDimStrength);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureActiveMonitorMode);
            this.tpRegionCapture.Controls.Add(this.nudRegionCaptureFPSLimit);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureFPSLimit);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureShowFPS);
            this.tpRegionCapture.Controls.Add(this.flpRegionCaptureFixedSize);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureIsFixedSize);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureShowCrosshair);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureMagnifierPixelSize);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureMagnifierPixelCount);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureUseSquareMagnifier);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureShowMagnifier);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureShowInfo);
            this.tpRegionCapture.Controls.Add(this.btnRegionCaptureSnapSizesRemove);
            this.tpRegionCapture.Controls.Add(this.btnRegionCaptureSnapSizesAdd);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureSnapSizes);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureSnapSizes);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureUseCustomInfoText);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureDetectControls);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureDetectWindows);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureMouse5ClickAction);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureMouse5ClickAction);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureMouse4ClickAction);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureMouse4ClickAction);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureMouseMiddleClickAction);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureMouseMiddleClickAction);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureMouseRightClickAction);
            this.tpRegionCapture.Controls.Add(this.lblRegionCaptureMouseRightClickAction);
            this.tpRegionCapture.Controls.Add(this.cbRegionCaptureMultiRegionMode);
            this.tpRegionCapture.Controls.Add(this.pRegionCaptureSnapSizes);
            this.tpRegionCapture.Controls.Add(this.txtRegionCaptureCustomInfoText);
            this.tpRegionCapture.Controls.Add(this.nudRegionCaptureMagnifierPixelCount);
            this.tpRegionCapture.Controls.Add(this.nudRegionCaptureMagnifierPixelSize);
            resources.ApplyResources(this.tpRegionCapture, "tpRegionCapture");
            this.tpRegionCapture.Name = "tpRegionCapture";
            // 
            // lblRegionCaptureBackgroundDimStrengthHint
            // 
            resources.ApplyResources(this.lblRegionCaptureBackgroundDimStrengthHint, "lblRegionCaptureBackgroundDimStrengthHint");
            this.lblRegionCaptureBackgroundDimStrengthHint.Name = "lblRegionCaptureBackgroundDimStrengthHint";
            // 
            // nudRegionCaptureBackgroundDimStrength
            // 
            resources.ApplyResources(this.nudRegionCaptureBackgroundDimStrength, "nudRegionCaptureBackgroundDimStrength");
            this.nudRegionCaptureBackgroundDimStrength.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudRegionCaptureBackgroundDimStrength.Name = "nudRegionCaptureBackgroundDimStrength";
            this.nudRegionCaptureBackgroundDimStrength.ValueChanged += new System.EventHandler(this.nudRegionCaptureBackgroundDimStrength_ValueChanged);
            // 
            // lblRegionCaptureBackgroundDimStrength
            // 
            resources.ApplyResources(this.lblRegionCaptureBackgroundDimStrength, "lblRegionCaptureBackgroundDimStrength");
            this.lblRegionCaptureBackgroundDimStrength.Name = "lblRegionCaptureBackgroundDimStrength";
            // 
            // cbRegionCaptureActiveMonitorMode
            // 
            resources.ApplyResources(this.cbRegionCaptureActiveMonitorMode, "cbRegionCaptureActiveMonitorMode");
            this.cbRegionCaptureActiveMonitorMode.Name = "cbRegionCaptureActiveMonitorMode";
            this.cbRegionCaptureActiveMonitorMode.UseVisualStyleBackColor = true;
            this.cbRegionCaptureActiveMonitorMode.CheckedChanged += new System.EventHandler(this.cbRegionCaptureActiveMonitorMode_CheckedChanged);
            // 
            // nudRegionCaptureFPSLimit
            // 
            resources.ApplyResources(this.nudRegionCaptureFPSLimit, "nudRegionCaptureFPSLimit");
            this.nudRegionCaptureFPSLimit.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudRegionCaptureFPSLimit.Name = "nudRegionCaptureFPSLimit";
            this.nudRegionCaptureFPSLimit.ValueChanged += new System.EventHandler(this.nudRegionCaptureFPSLimit_ValueChanged);
            // 
            // lblRegionCaptureFPSLimit
            // 
            resources.ApplyResources(this.lblRegionCaptureFPSLimit, "lblRegionCaptureFPSLimit");
            this.lblRegionCaptureFPSLimit.Name = "lblRegionCaptureFPSLimit";
            // 
            // cbRegionCaptureShowFPS
            // 
            resources.ApplyResources(this.cbRegionCaptureShowFPS, "cbRegionCaptureShowFPS");
            this.cbRegionCaptureShowFPS.Name = "cbRegionCaptureShowFPS";
            this.cbRegionCaptureShowFPS.UseVisualStyleBackColor = true;
            this.cbRegionCaptureShowFPS.CheckedChanged += new System.EventHandler(this.cbRegionCaptureShowFPS_CheckedChanged);
            // 
            // flpRegionCaptureFixedSize
            // 
            resources.ApplyResources(this.flpRegionCaptureFixedSize, "flpRegionCaptureFixedSize");
            this.flpRegionCaptureFixedSize.Controls.Add(this.lblRegionCaptureFixedSizeWidth);
            this.flpRegionCaptureFixedSize.Controls.Add(this.nudRegionCaptureFixedSizeWidth);
            this.flpRegionCaptureFixedSize.Controls.Add(this.lblRegionCaptureFixedSizeHeight);
            this.flpRegionCaptureFixedSize.Controls.Add(this.nudRegionCaptureFixedSizeHeight);
            this.flpRegionCaptureFixedSize.Name = "flpRegionCaptureFixedSize";
            // 
            // lblRegionCaptureFixedSizeWidth
            // 
            resources.ApplyResources(this.lblRegionCaptureFixedSizeWidth, "lblRegionCaptureFixedSizeWidth");
            this.lblRegionCaptureFixedSizeWidth.Name = "lblRegionCaptureFixedSizeWidth";
            // 
            // nudRegionCaptureFixedSizeWidth
            // 
            this.nudRegionCaptureFixedSizeWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nudRegionCaptureFixedSizeWidth, "nudRegionCaptureFixedSizeWidth");
            this.nudRegionCaptureFixedSizeWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRegionCaptureFixedSizeWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRegionCaptureFixedSizeWidth.Name = "nudRegionCaptureFixedSizeWidth";
            this.nudRegionCaptureFixedSizeWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRegionCaptureFixedSizeWidth.ValueChanged += new System.EventHandler(this.nudRegionCaptureFixedSizeWidth_ValueChanged);
            // 
            // lblRegionCaptureFixedSizeHeight
            // 
            resources.ApplyResources(this.lblRegionCaptureFixedSizeHeight, "lblRegionCaptureFixedSizeHeight");
            this.lblRegionCaptureFixedSizeHeight.Name = "lblRegionCaptureFixedSizeHeight";
            // 
            // nudRegionCaptureFixedSizeHeight
            // 
            this.nudRegionCaptureFixedSizeHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nudRegionCaptureFixedSizeHeight, "nudRegionCaptureFixedSizeHeight");
            this.nudRegionCaptureFixedSizeHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRegionCaptureFixedSizeHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRegionCaptureFixedSizeHeight.Name = "nudRegionCaptureFixedSizeHeight";
            this.nudRegionCaptureFixedSizeHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRegionCaptureFixedSizeHeight.ValueChanged += new System.EventHandler(this.nudRegionCaptureFixedSizeHeight_ValueChanged);
            // 
            // cbRegionCaptureIsFixedSize
            // 
            resources.ApplyResources(this.cbRegionCaptureIsFixedSize, "cbRegionCaptureIsFixedSize");
            this.cbRegionCaptureIsFixedSize.Name = "cbRegionCaptureIsFixedSize";
            this.cbRegionCaptureIsFixedSize.UseVisualStyleBackColor = true;
            this.cbRegionCaptureIsFixedSize.CheckedChanged += new System.EventHandler(this.cbRegionCaptureIsFixedSize_CheckedChanged);
            // 
            // cbRegionCaptureShowCrosshair
            // 
            resources.ApplyResources(this.cbRegionCaptureShowCrosshair, "cbRegionCaptureShowCrosshair");
            this.cbRegionCaptureShowCrosshair.Name = "cbRegionCaptureShowCrosshair";
            this.cbRegionCaptureShowCrosshair.UseVisualStyleBackColor = true;
            this.cbRegionCaptureShowCrosshair.CheckedChanged += new System.EventHandler(this.cbRegionCaptureShowCrosshair_CheckedChanged);
            // 
            // lblRegionCaptureMagnifierPixelSize
            // 
            resources.ApplyResources(this.lblRegionCaptureMagnifierPixelSize, "lblRegionCaptureMagnifierPixelSize");
            this.lblRegionCaptureMagnifierPixelSize.Name = "lblRegionCaptureMagnifierPixelSize";
            // 
            // lblRegionCaptureMagnifierPixelCount
            // 
            resources.ApplyResources(this.lblRegionCaptureMagnifierPixelCount, "lblRegionCaptureMagnifierPixelCount");
            this.lblRegionCaptureMagnifierPixelCount.Name = "lblRegionCaptureMagnifierPixelCount";
            // 
            // cbRegionCaptureUseSquareMagnifier
            // 
            resources.ApplyResources(this.cbRegionCaptureUseSquareMagnifier, "cbRegionCaptureUseSquareMagnifier");
            this.cbRegionCaptureUseSquareMagnifier.Name = "cbRegionCaptureUseSquareMagnifier";
            this.cbRegionCaptureUseSquareMagnifier.UseVisualStyleBackColor = true;
            this.cbRegionCaptureUseSquareMagnifier.CheckedChanged += new System.EventHandler(this.cbRegionCaptureUseSquareMagnifier_CheckedChanged);
            // 
            // cbRegionCaptureShowMagnifier
            // 
            resources.ApplyResources(this.cbRegionCaptureShowMagnifier, "cbRegionCaptureShowMagnifier");
            this.cbRegionCaptureShowMagnifier.Name = "cbRegionCaptureShowMagnifier";
            this.cbRegionCaptureShowMagnifier.UseVisualStyleBackColor = true;
            this.cbRegionCaptureShowMagnifier.CheckedChanged += new System.EventHandler(this.cbRegionCaptureShowMagnifier_CheckedChanged);
            // 
            // cbRegionCaptureShowInfo
            // 
            resources.ApplyResources(this.cbRegionCaptureShowInfo, "cbRegionCaptureShowInfo");
            this.cbRegionCaptureShowInfo.Name = "cbRegionCaptureShowInfo";
            this.cbRegionCaptureShowInfo.UseVisualStyleBackColor = true;
            this.cbRegionCaptureShowInfo.CheckedChanged += new System.EventHandler(this.cbRegionCaptureShowInfo_CheckedChanged);
            // 
            // btnRegionCaptureSnapSizesRemove
            // 
            resources.ApplyResources(this.btnRegionCaptureSnapSizesRemove, "btnRegionCaptureSnapSizesRemove");
            this.btnRegionCaptureSnapSizesRemove.Name = "btnRegionCaptureSnapSizesRemove";
            this.btnRegionCaptureSnapSizesRemove.UseVisualStyleBackColor = true;
            this.btnRegionCaptureSnapSizesRemove.Click += new System.EventHandler(this.btnRegionCaptureSnapSizesRemove_Click);
            // 
            // btnRegionCaptureSnapSizesAdd
            // 
            resources.ApplyResources(this.btnRegionCaptureSnapSizesAdd, "btnRegionCaptureSnapSizesAdd");
            this.btnRegionCaptureSnapSizesAdd.Name = "btnRegionCaptureSnapSizesAdd";
            this.btnRegionCaptureSnapSizesAdd.UseVisualStyleBackColor = true;
            this.btnRegionCaptureSnapSizesAdd.Click += new System.EventHandler(this.btnRegionCaptureSnapSizesAdd_Click);
            // 
            // cbRegionCaptureSnapSizes
            // 
            this.cbRegionCaptureSnapSizes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegionCaptureSnapSizes.FormattingEnabled = true;
            resources.ApplyResources(this.cbRegionCaptureSnapSizes, "cbRegionCaptureSnapSizes");
            this.cbRegionCaptureSnapSizes.Name = "cbRegionCaptureSnapSizes";
            // 
            // lblRegionCaptureSnapSizes
            // 
            resources.ApplyResources(this.lblRegionCaptureSnapSizes, "lblRegionCaptureSnapSizes");
            this.lblRegionCaptureSnapSizes.Name = "lblRegionCaptureSnapSizes";
            // 
            // cbRegionCaptureUseCustomInfoText
            // 
            resources.ApplyResources(this.cbRegionCaptureUseCustomInfoText, "cbRegionCaptureUseCustomInfoText");
            this.cbRegionCaptureUseCustomInfoText.Name = "cbRegionCaptureUseCustomInfoText";
            this.cbRegionCaptureUseCustomInfoText.UseVisualStyleBackColor = true;
            this.cbRegionCaptureUseCustomInfoText.CheckedChanged += new System.EventHandler(this.cbRegionCaptureUseCustomInfoText_CheckedChanged);
            // 
            // cbRegionCaptureDetectControls
            // 
            resources.ApplyResources(this.cbRegionCaptureDetectControls, "cbRegionCaptureDetectControls");
            this.cbRegionCaptureDetectControls.Name = "cbRegionCaptureDetectControls";
            this.cbRegionCaptureDetectControls.UseVisualStyleBackColor = true;
            this.cbRegionCaptureDetectControls.CheckedChanged += new System.EventHandler(this.cbRegionCaptureDetectControls_CheckedChanged);
            // 
            // cbRegionCaptureDetectWindows
            // 
            resources.ApplyResources(this.cbRegionCaptureDetectWindows, "cbRegionCaptureDetectWindows");
            this.cbRegionCaptureDetectWindows.Name = "cbRegionCaptureDetectWindows";
            this.cbRegionCaptureDetectWindows.UseVisualStyleBackColor = true;
            this.cbRegionCaptureDetectWindows.CheckedChanged += new System.EventHandler(this.cbRegionCaptureDetectWindows_CheckedChanged);
            // 
            // cbRegionCaptureMouse5ClickAction
            // 
            this.cbRegionCaptureMouse5ClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegionCaptureMouse5ClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbRegionCaptureMouse5ClickAction, "cbRegionCaptureMouse5ClickAction");
            this.cbRegionCaptureMouse5ClickAction.Name = "cbRegionCaptureMouse5ClickAction";
            this.cbRegionCaptureMouse5ClickAction.SelectedIndexChanged += new System.EventHandler(this.cbRegionCaptureMouse5ClickAction_SelectedIndexChanged);
            // 
            // lblRegionCaptureMouse5ClickAction
            // 
            resources.ApplyResources(this.lblRegionCaptureMouse5ClickAction, "lblRegionCaptureMouse5ClickAction");
            this.lblRegionCaptureMouse5ClickAction.Name = "lblRegionCaptureMouse5ClickAction";
            // 
            // cbRegionCaptureMouse4ClickAction
            // 
            this.cbRegionCaptureMouse4ClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegionCaptureMouse4ClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbRegionCaptureMouse4ClickAction, "cbRegionCaptureMouse4ClickAction");
            this.cbRegionCaptureMouse4ClickAction.Name = "cbRegionCaptureMouse4ClickAction";
            this.cbRegionCaptureMouse4ClickAction.SelectedIndexChanged += new System.EventHandler(this.cbRegionCaptureMouse4ClickAction_SelectedIndexChanged);
            // 
            // lblRegionCaptureMouse4ClickAction
            // 
            resources.ApplyResources(this.lblRegionCaptureMouse4ClickAction, "lblRegionCaptureMouse4ClickAction");
            this.lblRegionCaptureMouse4ClickAction.Name = "lblRegionCaptureMouse4ClickAction";
            // 
            // cbRegionCaptureMouseMiddleClickAction
            // 
            this.cbRegionCaptureMouseMiddleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegionCaptureMouseMiddleClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbRegionCaptureMouseMiddleClickAction, "cbRegionCaptureMouseMiddleClickAction");
            this.cbRegionCaptureMouseMiddleClickAction.Name = "cbRegionCaptureMouseMiddleClickAction";
            this.cbRegionCaptureMouseMiddleClickAction.SelectedIndexChanged += new System.EventHandler(this.cbRegionCaptureMouseMiddleClickAction_SelectedIndexChanged);
            // 
            // lblRegionCaptureMouseMiddleClickAction
            // 
            resources.ApplyResources(this.lblRegionCaptureMouseMiddleClickAction, "lblRegionCaptureMouseMiddleClickAction");
            this.lblRegionCaptureMouseMiddleClickAction.Name = "lblRegionCaptureMouseMiddleClickAction";
            // 
            // cbRegionCaptureMouseRightClickAction
            // 
            this.cbRegionCaptureMouseRightClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegionCaptureMouseRightClickAction.FormattingEnabled = true;
            resources.ApplyResources(this.cbRegionCaptureMouseRightClickAction, "cbRegionCaptureMouseRightClickAction");
            this.cbRegionCaptureMouseRightClickAction.Name = "cbRegionCaptureMouseRightClickAction";
            this.cbRegionCaptureMouseRightClickAction.SelectedIndexChanged += new System.EventHandler(this.cbRegionCaptureMouseRightClickAction_SelectedIndexChanged);
            // 
            // lblRegionCaptureMouseRightClickAction
            // 
            resources.ApplyResources(this.lblRegionCaptureMouseRightClickAction, "lblRegionCaptureMouseRightClickAction");
            this.lblRegionCaptureMouseRightClickAction.Name = "lblRegionCaptureMouseRightClickAction";
            // 
            // cbRegionCaptureMultiRegionMode
            // 
            resources.ApplyResources(this.cbRegionCaptureMultiRegionMode, "cbRegionCaptureMultiRegionMode");
            this.cbRegionCaptureMultiRegionMode.Name = "cbRegionCaptureMultiRegionMode";
            this.cbRegionCaptureMultiRegionMode.UseVisualStyleBackColor = true;
            this.cbRegionCaptureMultiRegionMode.CheckedChanged += new System.EventHandler(this.cbRegionCaptureMultiRegionMode_CheckedChanged);
            // 
            // pRegionCaptureSnapSizes
            // 
            this.pRegionCaptureSnapSizes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pRegionCaptureSnapSizes.Controls.Add(this.btnRegionCaptureSnapSizesDialogCancel);
            this.pRegionCaptureSnapSizes.Controls.Add(this.btnRegionCaptureSnapSizesDialogAdd);
            this.pRegionCaptureSnapSizes.Controls.Add(this.nudRegionCaptureSnapSizesHeight);
            this.pRegionCaptureSnapSizes.Controls.Add(this.RegionCaptureSnapSizesHeight);
            this.pRegionCaptureSnapSizes.Controls.Add(this.nudRegionCaptureSnapSizesWidth);
            this.pRegionCaptureSnapSizes.Controls.Add(this.lblRegionCaptureSnapSizesWidth);
            resources.ApplyResources(this.pRegionCaptureSnapSizes, "pRegionCaptureSnapSizes");
            this.pRegionCaptureSnapSizes.Name = "pRegionCaptureSnapSizes";
            // 
            // btnRegionCaptureSnapSizesDialogCancel
            // 
            resources.ApplyResources(this.btnRegionCaptureSnapSizesDialogCancel, "btnRegionCaptureSnapSizesDialogCancel");
            this.btnRegionCaptureSnapSizesDialogCancel.Name = "btnRegionCaptureSnapSizesDialogCancel";
            this.btnRegionCaptureSnapSizesDialogCancel.UseVisualStyleBackColor = true;
            this.btnRegionCaptureSnapSizesDialogCancel.Click += new System.EventHandler(this.btnRegionCaptureSnapSizesDialogCancel_Click);
            // 
            // btnRegionCaptureSnapSizesDialogAdd
            // 
            resources.ApplyResources(this.btnRegionCaptureSnapSizesDialogAdd, "btnRegionCaptureSnapSizesDialogAdd");
            this.btnRegionCaptureSnapSizesDialogAdd.Name = "btnRegionCaptureSnapSizesDialogAdd";
            this.btnRegionCaptureSnapSizesDialogAdd.UseVisualStyleBackColor = true;
            this.btnRegionCaptureSnapSizesDialogAdd.Click += new System.EventHandler(this.btnRegionCaptureSnapSizesDialogAdd_Click);
            // 
            // nudRegionCaptureSnapSizesHeight
            // 
            resources.ApplyResources(this.nudRegionCaptureSnapSizesHeight, "nudRegionCaptureSnapSizesHeight");
            this.nudRegionCaptureSnapSizesHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRegionCaptureSnapSizesHeight.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudRegionCaptureSnapSizesHeight.Name = "nudRegionCaptureSnapSizesHeight";
            this.nudRegionCaptureSnapSizesHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // RegionCaptureSnapSizesHeight
            // 
            resources.ApplyResources(this.RegionCaptureSnapSizesHeight, "RegionCaptureSnapSizesHeight");
            this.RegionCaptureSnapSizesHeight.Name = "RegionCaptureSnapSizesHeight";
            // 
            // nudRegionCaptureSnapSizesWidth
            // 
            resources.ApplyResources(this.nudRegionCaptureSnapSizesWidth, "nudRegionCaptureSnapSizesWidth");
            this.nudRegionCaptureSnapSizesWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRegionCaptureSnapSizesWidth.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudRegionCaptureSnapSizesWidth.Name = "nudRegionCaptureSnapSizesWidth";
            this.nudRegionCaptureSnapSizesWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblRegionCaptureSnapSizesWidth
            // 
            resources.ApplyResources(this.lblRegionCaptureSnapSizesWidth, "lblRegionCaptureSnapSizesWidth");
            this.lblRegionCaptureSnapSizesWidth.Name = "lblRegionCaptureSnapSizesWidth";
            // 
            // txtRegionCaptureCustomInfoText
            // 
            resources.ApplyResources(this.txtRegionCaptureCustomInfoText, "txtRegionCaptureCustomInfoText");
            this.txtRegionCaptureCustomInfoText.Name = "txtRegionCaptureCustomInfoText";
            this.txtRegionCaptureCustomInfoText.TextChanged += new System.EventHandler(this.txtRegionCaptureCustomInfoText_TextChanged);
            // 
            // nudRegionCaptureMagnifierPixelCount
            // 
            this.nudRegionCaptureMagnifierPixelCount.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            resources.ApplyResources(this.nudRegionCaptureMagnifierPixelCount, "nudRegionCaptureMagnifierPixelCount");
            this.nudRegionCaptureMagnifierPixelCount.Name = "nudRegionCaptureMagnifierPixelCount";
            this.nudRegionCaptureMagnifierPixelCount.ValueChanged += new System.EventHandler(this.nudRegionCaptureMagnifierPixelCount_ValueChanged);
            // 
            // nudRegionCaptureMagnifierPixelSize
            // 
            resources.ApplyResources(this.nudRegionCaptureMagnifierPixelSize, "nudRegionCaptureMagnifierPixelSize");
            this.nudRegionCaptureMagnifierPixelSize.Name = "nudRegionCaptureMagnifierPixelSize";
            this.nudRegionCaptureMagnifierPixelSize.ValueChanged += new System.EventHandler(this.nudRegionCaptureMagnifierPixelSize_ValueChanged);
            // 
            // tpScreenRecorder
            // 
            this.tpScreenRecorder.BackColor = System.Drawing.SystemColors.Window;
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecordTransparentRegion);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecordTwoPassEncoding);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecordConfirmAbort);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecorderShowCursor);
            this.tpScreenRecorder.Controls.Add(this.btnScreenRecorderFFmpegOptions);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderStartDelay);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecordAutoStart);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecorderFixedDuration);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecordFPS);
            this.tpScreenRecorder.Controls.Add(this.lblScreenRecordFPS);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderDuration);
            this.tpScreenRecorder.Controls.Add(this.nudScreenRecorderStartDelay);
            this.tpScreenRecorder.Controls.Add(this.cbScreenRecorderFixedDuration);
            this.tpScreenRecorder.Controls.Add(this.nudGIFFPS);
            this.tpScreenRecorder.Controls.Add(this.lblGIFFPS);
            resources.ApplyResources(this.tpScreenRecorder, "tpScreenRecorder");
            this.tpScreenRecorder.Name = "tpScreenRecorder";
            // 
            // cbScreenRecordTransparentRegion
            // 
            resources.ApplyResources(this.cbScreenRecordTransparentRegion, "cbScreenRecordTransparentRegion");
            this.cbScreenRecordTransparentRegion.Name = "cbScreenRecordTransparentRegion";
            this.cbScreenRecordTransparentRegion.UseVisualStyleBackColor = true;
            this.cbScreenRecordTransparentRegion.CheckedChanged += new System.EventHandler(this.cbScreenRecordTransparentRegion_CheckedChanged);
            // 
            // cbScreenRecordTwoPassEncoding
            // 
            resources.ApplyResources(this.cbScreenRecordTwoPassEncoding, "cbScreenRecordTwoPassEncoding");
            this.cbScreenRecordTwoPassEncoding.Name = "cbScreenRecordTwoPassEncoding";
            this.cbScreenRecordTwoPassEncoding.UseVisualStyleBackColor = true;
            this.cbScreenRecordTwoPassEncoding.CheckedChanged += new System.EventHandler(this.cbScreenRecordTwoPassEncoding_CheckedChanged);
            // 
            // cbScreenRecordConfirmAbort
            // 
            resources.ApplyResources(this.cbScreenRecordConfirmAbort, "cbScreenRecordConfirmAbort");
            this.cbScreenRecordConfirmAbort.Name = "cbScreenRecordConfirmAbort";
            this.cbScreenRecordConfirmAbort.UseVisualStyleBackColor = true;
            this.cbScreenRecordConfirmAbort.CheckedChanged += new System.EventHandler(this.cbScreenRecordConfirmAbort_CheckedChanged);
            // 
            // cbScreenRecorderShowCursor
            // 
            resources.ApplyResources(this.cbScreenRecorderShowCursor, "cbScreenRecorderShowCursor");
            this.cbScreenRecorderShowCursor.Name = "cbScreenRecorderShowCursor";
            this.cbScreenRecorderShowCursor.UseVisualStyleBackColor = true;
            this.cbScreenRecorderShowCursor.CheckedChanged += new System.EventHandler(this.cbScreenRecorderShowCursor_CheckedChanged);
            // 
            // btnScreenRecorderFFmpegOptions
            // 
            resources.ApplyResources(this.btnScreenRecorderFFmpegOptions, "btnScreenRecorderFFmpegOptions");
            this.btnScreenRecorderFFmpegOptions.Name = "btnScreenRecorderFFmpegOptions";
            this.btnScreenRecorderFFmpegOptions.UseVisualStyleBackColor = true;
            this.btnScreenRecorderFFmpegOptions.Click += new System.EventHandler(this.btnScreenRecorderFFmpegOptions_Click);
            // 
            // lblScreenRecorderStartDelay
            // 
            resources.ApplyResources(this.lblScreenRecorderStartDelay, "lblScreenRecorderStartDelay");
            this.lblScreenRecorderStartDelay.Name = "lblScreenRecorderStartDelay";
            // 
            // cbScreenRecordAutoStart
            // 
            resources.ApplyResources(this.cbScreenRecordAutoStart, "cbScreenRecordAutoStart");
            this.cbScreenRecordAutoStart.Name = "cbScreenRecordAutoStart";
            this.cbScreenRecordAutoStart.UseVisualStyleBackColor = true;
            this.cbScreenRecordAutoStart.CheckedChanged += new System.EventHandler(this.cbScreenRecordAutoStart_CheckedChanged);
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
            60,
            0,
            0,
            0});
            this.nudScreenRecordFPS.Minimum = new decimal(new int[] {
            1,
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
            30,
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
            // tpOCR
            // 
            this.tpOCR.Controls.Add(this.btnCaptureOCRHelp);
            this.tpOCR.Controls.Add(this.cbCaptureOCRAutoCopy);
            this.tpOCR.Controls.Add(this.cbCloseWindowAfterOpenServiceLink);
            this.tpOCR.Controls.Add(this.cbCaptureOCRSilent);
            this.tpOCR.Controls.Add(this.lblOCRDefaultLanguage);
            this.tpOCR.Controls.Add(this.cbCaptureOCRDefaultLanguage);
            resources.ApplyResources(this.tpOCR, "tpOCR");
            this.tpOCR.Name = "tpOCR";
            this.tpOCR.UseVisualStyleBackColor = true;
            // 
            // btnCaptureOCRHelp
            // 
            this.btnCaptureOCRHelp.Image = global::ShareX.Properties.Resources.question;
            resources.ApplyResources(this.btnCaptureOCRHelp, "btnCaptureOCRHelp");
            this.btnCaptureOCRHelp.Name = "btnCaptureOCRHelp";
            this.btnCaptureOCRHelp.UseVisualStyleBackColor = true;
            this.btnCaptureOCRHelp.Click += new System.EventHandler(this.btnCaptureOCRHelp_Click);
            // 
            // cbCaptureOCRAutoCopy
            // 
            resources.ApplyResources(this.cbCaptureOCRAutoCopy, "cbCaptureOCRAutoCopy");
            this.cbCaptureOCRAutoCopy.Name = "cbCaptureOCRAutoCopy";
            this.cbCaptureOCRAutoCopy.UseVisualStyleBackColor = true;
            this.cbCaptureOCRAutoCopy.CheckedChanged += new System.EventHandler(this.cbCaptureOCRAutoCopy_CheckedChanged);
            // 
            // cbCloseWindowAfterOpenServiceLink
            // 
            resources.ApplyResources(this.cbCloseWindowAfterOpenServiceLink, "cbCloseWindowAfterOpenServiceLink");
            this.cbCloseWindowAfterOpenServiceLink.Name = "cbCloseWindowAfterOpenServiceLink";
            this.cbCloseWindowAfterOpenServiceLink.UseVisualStyleBackColor = true;
            this.cbCloseWindowAfterOpenServiceLink.CheckedChanged += new System.EventHandler(this.cbCloseWindowAfterOpenServiceLink_CheckedChanged);
            // 
            // cbCaptureOCRSilent
            // 
            resources.ApplyResources(this.cbCaptureOCRSilent, "cbCaptureOCRSilent");
            this.cbCaptureOCRSilent.Name = "cbCaptureOCRSilent";
            this.cbCaptureOCRSilent.UseVisualStyleBackColor = true;
            this.cbCaptureOCRSilent.CheckedChanged += new System.EventHandler(this.cbCaptureOCRSilent_CheckedChanged);
            // 
            // lblOCRDefaultLanguage
            // 
            resources.ApplyResources(this.lblOCRDefaultLanguage, "lblOCRDefaultLanguage");
            this.lblOCRDefaultLanguage.Name = "lblOCRDefaultLanguage";
            // 
            // cbCaptureOCRDefaultLanguage
            // 
            this.cbCaptureOCRDefaultLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCaptureOCRDefaultLanguage.FormattingEnabled = true;
            resources.ApplyResources(this.cbCaptureOCRDefaultLanguage, "cbCaptureOCRDefaultLanguage");
            this.cbCaptureOCRDefaultLanguage.Name = "cbCaptureOCRDefaultLanguage";
            this.cbCaptureOCRDefaultLanguage.SelectedIndexChanged += new System.EventHandler(this.cbCaptureOCRDefaultLanguage_SelectedIndexChanged);
            // 
            // tpUpload
            // 
            this.tpUpload.BackColor = System.Drawing.SystemColors.Window;
            this.tpUpload.Controls.Add(this.tcUpload);
            resources.ApplyResources(this.tpUpload, "tpUpload");
            this.tpUpload.Name = "tpUpload";
            // 
            // tcUpload
            // 
            this.tcUpload.Controls.Add(this.tpUploadMain);
            this.tcUpload.Controls.Add(this.tpFileNaming);
            this.tcUpload.Controls.Add(this.tpUploadClipboard);
            this.tcUpload.Controls.Add(this.tpUploaderFilters);
            resources.ApplyResources(this.tcUpload, "tcUpload");
            this.tcUpload.Name = "tcUpload";
            this.tcUpload.SelectedIndex = 0;
            // 
            // tpUploadMain
            // 
            this.tpUploadMain.BackColor = System.Drawing.SystemColors.Window;
            this.tpUploadMain.Controls.Add(this.cbOverrideUploadSettings);
            resources.ApplyResources(this.tpUploadMain, "tpUploadMain");
            this.tpUploadMain.Name = "tpUploadMain";
            // 
            // cbOverrideUploadSettings
            // 
            resources.ApplyResources(this.cbOverrideUploadSettings, "cbOverrideUploadSettings");
            this.cbOverrideUploadSettings.Checked = true;
            this.cbOverrideUploadSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideUploadSettings.Name = "cbOverrideUploadSettings";
            this.cbOverrideUploadSettings.UseVisualStyleBackColor = true;
            this.cbOverrideUploadSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultUploadSettings_CheckedChanged);
            // 
            // tpFileNaming
            // 
            this.tpFileNaming.BackColor = System.Drawing.SystemColors.Window;
            this.tpFileNaming.Controls.Add(this.txtURLRegexReplaceReplacement);
            this.tpFileNaming.Controls.Add(this.lblURLRegexReplaceReplacement);
            this.tpFileNaming.Controls.Add(this.txtURLRegexReplacePattern);
            this.tpFileNaming.Controls.Add(this.lblURLRegexReplacePattern);
            this.tpFileNaming.Controls.Add(this.cbURLRegexReplace);
            this.tpFileNaming.Controls.Add(this.btnAutoIncrementNumber);
            this.tpFileNaming.Controls.Add(this.lblAutoIncrementNumber);
            this.tpFileNaming.Controls.Add(this.nudAutoIncrementNumber);
            this.tpFileNaming.Controls.Add(this.cbFileUploadReplaceProblematicCharacters);
            this.tpFileNaming.Controls.Add(this.cbNameFormatCustomTimeZone);
            this.tpFileNaming.Controls.Add(this.lblNameFormatPatternPreview);
            this.tpFileNaming.Controls.Add(this.lblNameFormatPatternActiveWindow);
            this.tpFileNaming.Controls.Add(this.lblNameFormatPatternPreviewActiveWindow);
            this.tpFileNaming.Controls.Add(this.cbNameFormatTimeZone);
            this.tpFileNaming.Controls.Add(this.txtNameFormatPatternActiveWindow);
            this.tpFileNaming.Controls.Add(this.cbFileUploadUseNamePattern);
            this.tpFileNaming.Controls.Add(this.lblNameFormatPattern);
            this.tpFileNaming.Controls.Add(this.txtNameFormatPattern);
            resources.ApplyResources(this.tpFileNaming, "tpFileNaming");
            this.tpFileNaming.Name = "tpFileNaming";
            // 
            // txtURLRegexReplaceReplacement
            // 
            resources.ApplyResources(this.txtURLRegexReplaceReplacement, "txtURLRegexReplaceReplacement");
            this.txtURLRegexReplaceReplacement.Name = "txtURLRegexReplaceReplacement";
            this.txtURLRegexReplaceReplacement.TextChanged += new System.EventHandler(this.txtURLRegexReplaceReplacement_TextChanged);
            // 
            // lblURLRegexReplaceReplacement
            // 
            resources.ApplyResources(this.lblURLRegexReplaceReplacement, "lblURLRegexReplaceReplacement");
            this.lblURLRegexReplaceReplacement.Name = "lblURLRegexReplaceReplacement";
            // 
            // txtURLRegexReplacePattern
            // 
            resources.ApplyResources(this.txtURLRegexReplacePattern, "txtURLRegexReplacePattern");
            this.txtURLRegexReplacePattern.Name = "txtURLRegexReplacePattern";
            this.txtURLRegexReplacePattern.TextChanged += new System.EventHandler(this.txtURLRegexReplacePattern_TextChanged);
            // 
            // lblURLRegexReplacePattern
            // 
            resources.ApplyResources(this.lblURLRegexReplacePattern, "lblURLRegexReplacePattern");
            this.lblURLRegexReplacePattern.Name = "lblURLRegexReplacePattern";
            // 
            // cbURLRegexReplace
            // 
            resources.ApplyResources(this.cbURLRegexReplace, "cbURLRegexReplace");
            this.cbURLRegexReplace.Name = "cbURLRegexReplace";
            this.cbURLRegexReplace.UseVisualStyleBackColor = true;
            this.cbURLRegexReplace.CheckedChanged += new System.EventHandler(this.cbURLRegexReplace_CheckedChanged);
            // 
            // btnAutoIncrementNumber
            // 
            resources.ApplyResources(this.btnAutoIncrementNumber, "btnAutoIncrementNumber");
            this.btnAutoIncrementNumber.Name = "btnAutoIncrementNumber";
            this.btnAutoIncrementNumber.UseVisualStyleBackColor = true;
            this.btnAutoIncrementNumber.Click += new System.EventHandler(this.btnAutoIncrementNumber_Click);
            // 
            // lblAutoIncrementNumber
            // 
            resources.ApplyResources(this.lblAutoIncrementNumber, "lblAutoIncrementNumber");
            this.lblAutoIncrementNumber.Name = "lblAutoIncrementNumber";
            // 
            // nudAutoIncrementNumber
            // 
            resources.ApplyResources(this.nudAutoIncrementNumber, "nudAutoIncrementNumber");
            this.nudAutoIncrementNumber.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudAutoIncrementNumber.Name = "nudAutoIncrementNumber";
            // 
            // cbFileUploadReplaceProblematicCharacters
            // 
            resources.ApplyResources(this.cbFileUploadReplaceProblematicCharacters, "cbFileUploadReplaceProblematicCharacters");
            this.cbFileUploadReplaceProblematicCharacters.Name = "cbFileUploadReplaceProblematicCharacters";
            this.cbFileUploadReplaceProblematicCharacters.UseVisualStyleBackColor = true;
            this.cbFileUploadReplaceProblematicCharacters.CheckedChanged += new System.EventHandler(this.cbFileUploadReplaceProblematicCharacters_CheckedChanged);
            // 
            // cbNameFormatCustomTimeZone
            // 
            resources.ApplyResources(this.cbNameFormatCustomTimeZone, "cbNameFormatCustomTimeZone");
            this.cbNameFormatCustomTimeZone.Name = "cbNameFormatCustomTimeZone";
            this.cbNameFormatCustomTimeZone.UseVisualStyleBackColor = true;
            this.cbNameFormatCustomTimeZone.CheckedChanged += new System.EventHandler(this.cbNameFormatCustomTimeZone_CheckedChanged);
            // 
            // lblNameFormatPatternPreview
            // 
            resources.ApplyResources(this.lblNameFormatPatternPreview, "lblNameFormatPatternPreview");
            this.lblNameFormatPatternPreview.Name = "lblNameFormatPatternPreview";
            // 
            // lblNameFormatPatternActiveWindow
            // 
            resources.ApplyResources(this.lblNameFormatPatternActiveWindow, "lblNameFormatPatternActiveWindow");
            this.lblNameFormatPatternActiveWindow.Name = "lblNameFormatPatternActiveWindow";
            // 
            // lblNameFormatPatternPreviewActiveWindow
            // 
            resources.ApplyResources(this.lblNameFormatPatternPreviewActiveWindow, "lblNameFormatPatternPreviewActiveWindow");
            this.lblNameFormatPatternPreviewActiveWindow.Name = "lblNameFormatPatternPreviewActiveWindow";
            // 
            // cbNameFormatTimeZone
            // 
            this.cbNameFormatTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNameFormatTimeZone.FormattingEnabled = true;
            resources.ApplyResources(this.cbNameFormatTimeZone, "cbNameFormatTimeZone");
            this.cbNameFormatTimeZone.Name = "cbNameFormatTimeZone";
            this.cbNameFormatTimeZone.SelectedIndexChanged += new System.EventHandler(this.cbNameFormatTimeZone_SelectedIndexChanged);
            // 
            // txtNameFormatPatternActiveWindow
            // 
            resources.ApplyResources(this.txtNameFormatPatternActiveWindow, "txtNameFormatPatternActiveWindow");
            this.txtNameFormatPatternActiveWindow.Name = "txtNameFormatPatternActiveWindow";
            this.txtNameFormatPatternActiveWindow.TextChanged += new System.EventHandler(this.txtNameFormatPatternActiveWindow_TextChanged);
            // 
            // cbFileUploadUseNamePattern
            // 
            resources.ApplyResources(this.cbFileUploadUseNamePattern, "cbFileUploadUseNamePattern");
            this.cbFileUploadUseNamePattern.Name = "cbFileUploadUseNamePattern";
            this.cbFileUploadUseNamePattern.UseVisualStyleBackColor = true;
            this.cbFileUploadUseNamePattern.CheckedChanged += new System.EventHandler(this.cbFileUploadUseNamePattern_CheckedChanged);
            // 
            // lblNameFormatPattern
            // 
            resources.ApplyResources(this.lblNameFormatPattern, "lblNameFormatPattern");
            this.lblNameFormatPattern.Name = "lblNameFormatPattern";
            // 
            // txtNameFormatPattern
            // 
            resources.ApplyResources(this.txtNameFormatPattern, "txtNameFormatPattern");
            this.txtNameFormatPattern.Name = "txtNameFormatPattern";
            this.txtNameFormatPattern.TextChanged += new System.EventHandler(this.txtNameFormatPattern_TextChanged);
            // 
            // tpUploadClipboard
            // 
            this.tpUploadClipboard.BackColor = System.Drawing.SystemColors.Window;
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadShareURL);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadURLContents);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadAutoIndexFolder);
            this.tpUploadClipboard.Controls.Add(this.cbClipboardUploadShortenURL);
            resources.ApplyResources(this.tpUploadClipboard, "tpUploadClipboard");
            this.tpUploadClipboard.Name = "tpUploadClipboard";
            // 
            // cbClipboardUploadShareURL
            // 
            resources.ApplyResources(this.cbClipboardUploadShareURL, "cbClipboardUploadShareURL");
            this.cbClipboardUploadShareURL.Name = "cbClipboardUploadShareURL";
            this.cbClipboardUploadShareURL.UseVisualStyleBackColor = true;
            this.cbClipboardUploadShareURL.CheckedChanged += new System.EventHandler(this.cbClipboardUploadShareURL_CheckedChanged);
            // 
            // cbClipboardUploadURLContents
            // 
            resources.ApplyResources(this.cbClipboardUploadURLContents, "cbClipboardUploadURLContents");
            this.cbClipboardUploadURLContents.Name = "cbClipboardUploadURLContents";
            this.cbClipboardUploadURLContents.UseVisualStyleBackColor = true;
            this.cbClipboardUploadURLContents.CheckedChanged += new System.EventHandler(this.cbClipboardUploadContents_CheckedChanged);
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
            // tpUploaderFilters
            // 
            this.tpUploaderFilters.BackColor = System.Drawing.SystemColors.Window;
            this.tpUploaderFilters.Controls.Add(this.lvUploaderFiltersList);
            this.tpUploaderFilters.Controls.Add(this.btnUploaderFiltersRemove);
            this.tpUploaderFilters.Controls.Add(this.btnUploaderFiltersUpdate);
            this.tpUploaderFilters.Controls.Add(this.btnUploaderFiltersAdd);
            this.tpUploaderFilters.Controls.Add(this.lblUploaderFiltersDestination);
            this.tpUploaderFilters.Controls.Add(this.cbUploaderFiltersDestination);
            this.tpUploaderFilters.Controls.Add(this.lblUploaderFiltersExtensionsExample);
            this.tpUploaderFilters.Controls.Add(this.lblUploaderFiltersExtensions);
            this.tpUploaderFilters.Controls.Add(this.txtUploaderFiltersExtensions);
            resources.ApplyResources(this.tpUploaderFilters, "tpUploaderFilters");
            this.tpUploaderFilters.Name = "tpUploaderFilters";
            // 
            // lvUploaderFiltersList
            // 
            resources.ApplyResources(this.lvUploaderFiltersList, "lvUploaderFiltersList");
            this.lvUploaderFiltersList.AutoFillColumn = true;
            this.lvUploaderFiltersList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chUploaderFiltersName,
            this.chUploaderFiltersExtension});
            this.lvUploaderFiltersList.FullRowSelect = true;
            this.lvUploaderFiltersList.HideSelection = false;
            this.lvUploaderFiltersList.Name = "lvUploaderFiltersList";
            this.lvUploaderFiltersList.UseCompatibleStateImageBehavior = false;
            this.lvUploaderFiltersList.View = System.Windows.Forms.View.Details;
            this.lvUploaderFiltersList.SelectedIndexChanged += new System.EventHandler(this.lvUploaderFiltersList_SelectedIndexChanged);
            // 
            // chUploaderFiltersName
            // 
            resources.ApplyResources(this.chUploaderFiltersName, "chUploaderFiltersName");
            // 
            // chUploaderFiltersExtension
            // 
            resources.ApplyResources(this.chUploaderFiltersExtension, "chUploaderFiltersExtension");
            // 
            // btnUploaderFiltersRemove
            // 
            resources.ApplyResources(this.btnUploaderFiltersRemove, "btnUploaderFiltersRemove");
            this.btnUploaderFiltersRemove.Name = "btnUploaderFiltersRemove";
            this.btnUploaderFiltersRemove.UseVisualStyleBackColor = true;
            this.btnUploaderFiltersRemove.Click += new System.EventHandler(this.btnUploaderFiltersRemove_Click);
            // 
            // btnUploaderFiltersUpdate
            // 
            resources.ApplyResources(this.btnUploaderFiltersUpdate, "btnUploaderFiltersUpdate");
            this.btnUploaderFiltersUpdate.Name = "btnUploaderFiltersUpdate";
            this.btnUploaderFiltersUpdate.UseVisualStyleBackColor = true;
            this.btnUploaderFiltersUpdate.Click += new System.EventHandler(this.btnUploaderFiltersUpdate_Click);
            // 
            // btnUploaderFiltersAdd
            // 
            resources.ApplyResources(this.btnUploaderFiltersAdd, "btnUploaderFiltersAdd");
            this.btnUploaderFiltersAdd.Name = "btnUploaderFiltersAdd";
            this.btnUploaderFiltersAdd.UseVisualStyleBackColor = true;
            this.btnUploaderFiltersAdd.Click += new System.EventHandler(this.btnUploaderFiltersAdd_Click);
            // 
            // lblUploaderFiltersDestination
            // 
            resources.ApplyResources(this.lblUploaderFiltersDestination, "lblUploaderFiltersDestination");
            this.lblUploaderFiltersDestination.Name = "lblUploaderFiltersDestination";
            // 
            // cbUploaderFiltersDestination
            // 
            this.cbUploaderFiltersDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUploaderFiltersDestination.FormattingEnabled = true;
            resources.ApplyResources(this.cbUploaderFiltersDestination, "cbUploaderFiltersDestination");
            this.cbUploaderFiltersDestination.Name = "cbUploaderFiltersDestination";
            // 
            // lblUploaderFiltersExtensionsExample
            // 
            resources.ApplyResources(this.lblUploaderFiltersExtensionsExample, "lblUploaderFiltersExtensionsExample");
            this.lblUploaderFiltersExtensionsExample.Name = "lblUploaderFiltersExtensionsExample";
            // 
            // lblUploaderFiltersExtensions
            // 
            resources.ApplyResources(this.lblUploaderFiltersExtensions, "lblUploaderFiltersExtensions");
            this.lblUploaderFiltersExtensions.Name = "lblUploaderFiltersExtensions";
            // 
            // txtUploaderFiltersExtensions
            // 
            resources.ApplyResources(this.txtUploaderFiltersExtensions, "txtUploaderFiltersExtensions");
            this.txtUploaderFiltersExtensions.Name = "txtUploaderFiltersExtensions";
            // 
            // tpActions
            // 
            this.tpActions.BackColor = System.Drawing.SystemColors.Window;
            this.tpActions.Controls.Add(this.pActions);
            this.tpActions.Controls.Add(this.cbOverrideActions);
            resources.ApplyResources(this.tpActions, "tpActions");
            this.tpActions.Name = "tpActions";
            // 
            // pActions
            // 
            this.pActions.Controls.Add(this.btnActions);
            this.pActions.Controls.Add(this.lblActionsNote);
            this.pActions.Controls.Add(this.btnActionsDuplicate);
            this.pActions.Controls.Add(this.btnActionsAdd);
            this.pActions.Controls.Add(this.lvActions);
            this.pActions.Controls.Add(this.btnActionsEdit);
            this.pActions.Controls.Add(this.btnActionsRemove);
            resources.ApplyResources(this.pActions, "pActions");
            this.pActions.Name = "pActions";
            // 
            // btnActions
            // 
            resources.ApplyResources(this.btnActions, "btnActions");
            this.btnActions.Name = "btnActions";
            this.btnActions.UseVisualStyleBackColor = true;
            this.btnActions.Click += new System.EventHandler(this.btnActions_Click);
            // 
            // lblActionsNote
            // 
            resources.ApplyResources(this.lblActionsNote, "lblActionsNote");
            this.lblActionsNote.Name = "lblActionsNote";
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
            this.lvActions.AllowDrop = true;
            this.lvActions.AllowItemDrag = true;
            resources.ApplyResources(this.lvActions, "lvActions");
            this.lvActions.AutoFillColumn = true;
            this.lvActions.AutoFillColumnIndex = 2;
            this.lvActions.CheckBoxes = true;
            this.lvActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chActionsName,
            this.chActionsPath,
            this.chActionsArgs,
            this.chActionsExtensions});
            this.lvActions.FullRowSelect = true;
            this.lvActions.HideSelection = false;
            this.lvActions.MultiSelect = false;
            this.lvActions.Name = "lvActions";
            this.lvActions.UseCompatibleStateImageBehavior = false;
            this.lvActions.View = System.Windows.Forms.View.Details;
            this.lvActions.ItemMoved += new ShareX.HelpersLib.MyListView.ListViewItemMovedEventHandler(this.lvActions_ItemMoved);
            this.lvActions.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvActions_ItemChecked);
            this.lvActions.SelectedIndexChanged += new System.EventHandler(this.lvActions_SelectedIndexChanged);
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
            // cbOverrideActions
            // 
            resources.ApplyResources(this.cbOverrideActions, "cbOverrideActions");
            this.cbOverrideActions.Checked = true;
            this.cbOverrideActions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideActions.Name = "cbOverrideActions";
            this.cbOverrideActions.UseVisualStyleBackColor = true;
            this.cbOverrideActions.CheckedChanged += new System.EventHandler(this.cbUseDefaultActions_CheckedChanged);
            // 
            // tpWatchFolders
            // 
            this.tpWatchFolders.BackColor = System.Drawing.SystemColors.Window;
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderEdit);
            this.tpWatchFolders.Controls.Add(this.cbWatchFolderEnabled);
            this.tpWatchFolders.Controls.Add(this.lvWatchFolderList);
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderRemove);
            this.tpWatchFolders.Controls.Add(this.btnWatchFolderAdd);
            resources.ApplyResources(this.tpWatchFolders, "tpWatchFolders");
            this.tpWatchFolders.Name = "tpWatchFolders";
            // 
            // btnWatchFolderEdit
            // 
            resources.ApplyResources(this.btnWatchFolderEdit, "btnWatchFolderEdit");
            this.btnWatchFolderEdit.Name = "btnWatchFolderEdit";
            this.btnWatchFolderEdit.UseVisualStyleBackColor = true;
            this.btnWatchFolderEdit.Click += new System.EventHandler(this.btnWatchFolderEdit_Click);
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
            this.lvWatchFolderList.AutoFillColumn = true;
            this.lvWatchFolderList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chWatchFolderFolderPath,
            this.chWatchFolderFilter,
            this.chWatchFolderIncludeSubdirectories});
            this.lvWatchFolderList.FullRowSelect = true;
            this.lvWatchFolderList.HideSelection = false;
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
            // tpTools
            // 
            this.tpTools.BackColor = System.Drawing.SystemColors.Window;
            this.tpTools.Controls.Add(this.pTools);
            this.tpTools.Controls.Add(this.cbOverrideToolsSettings);
            resources.ApplyResources(this.tpTools, "tpTools");
            this.tpTools.Name = "tpTools";
            // 
            // pTools
            // 
            this.pTools.Controls.Add(this.txtToolsScreenColorPickerFormatCtrl);
            this.pTools.Controls.Add(this.lblToolsScreenColorPickerFormatCtrl);
            this.pTools.Controls.Add(this.txtToolsScreenColorPickerInfoText);
            this.pTools.Controls.Add(this.lblToolsScreenColorPickerInfoText);
            this.pTools.Controls.Add(this.txtToolsScreenColorPickerFormat);
            this.pTools.Controls.Add(this.lblToolsScreenColorPickerFormat);
            resources.ApplyResources(this.pTools, "pTools");
            this.pTools.Name = "pTools";
            // 
            // txtToolsScreenColorPickerFormatCtrl
            // 
            resources.ApplyResources(this.txtToolsScreenColorPickerFormatCtrl, "txtToolsScreenColorPickerFormatCtrl");
            this.txtToolsScreenColorPickerFormatCtrl.Name = "txtToolsScreenColorPickerFormatCtrl";
            this.txtToolsScreenColorPickerFormatCtrl.TextChanged += new System.EventHandler(this.txtToolsScreenColorPickerFormatCtrl_TextChanged);
            // 
            // lblToolsScreenColorPickerFormatCtrl
            // 
            resources.ApplyResources(this.lblToolsScreenColorPickerFormatCtrl, "lblToolsScreenColorPickerFormatCtrl");
            this.lblToolsScreenColorPickerFormatCtrl.Name = "lblToolsScreenColorPickerFormatCtrl";
            // 
            // txtToolsScreenColorPickerInfoText
            // 
            resources.ApplyResources(this.txtToolsScreenColorPickerInfoText, "txtToolsScreenColorPickerInfoText");
            this.txtToolsScreenColorPickerInfoText.Name = "txtToolsScreenColorPickerInfoText";
            this.txtToolsScreenColorPickerInfoText.TextChanged += new System.EventHandler(this.txtToolsScreenColorPickerInfoText_TextChanged);
            // 
            // lblToolsScreenColorPickerInfoText
            // 
            resources.ApplyResources(this.lblToolsScreenColorPickerInfoText, "lblToolsScreenColorPickerInfoText");
            this.lblToolsScreenColorPickerInfoText.Name = "lblToolsScreenColorPickerInfoText";
            // 
            // txtToolsScreenColorPickerFormat
            // 
            resources.ApplyResources(this.txtToolsScreenColorPickerFormat, "txtToolsScreenColorPickerFormat");
            this.txtToolsScreenColorPickerFormat.Name = "txtToolsScreenColorPickerFormat";
            this.txtToolsScreenColorPickerFormat.TextChanged += new System.EventHandler(this.txtToolsScreenColorPickerFormat_TextChanged);
            // 
            // lblToolsScreenColorPickerFormat
            // 
            resources.ApplyResources(this.lblToolsScreenColorPickerFormat, "lblToolsScreenColorPickerFormat");
            this.lblToolsScreenColorPickerFormat.Name = "lblToolsScreenColorPickerFormat";
            // 
            // cbOverrideToolsSettings
            // 
            resources.ApplyResources(this.cbOverrideToolsSettings, "cbOverrideToolsSettings");
            this.cbOverrideToolsSettings.Checked = true;
            this.cbOverrideToolsSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideToolsSettings.Name = "cbOverrideToolsSettings";
            this.cbOverrideToolsSettings.UseVisualStyleBackColor = true;
            this.cbOverrideToolsSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultToolsSettings_CheckedChanged);
            // 
            // tpAdvanced
            // 
            this.tpAdvanced.BackColor = System.Drawing.SystemColors.Window;
            this.tpAdvanced.Controls.Add(this.pgTaskSettings);
            this.tpAdvanced.Controls.Add(this.cbOverrideAdvancedSettings);
            resources.ApplyResources(this.tpAdvanced, "tpAdvanced");
            this.tpAdvanced.Name = "tpAdvanced";
            // 
            // pgTaskSettings
            // 
            resources.ApplyResources(this.pgTaskSettings, "pgTaskSettings");
            this.pgTaskSettings.Name = "pgTaskSettings";
            this.pgTaskSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgTaskSettings.ToolbarVisible = false;
            // 
            // cbOverrideAdvancedSettings
            // 
            resources.ApplyResources(this.cbOverrideAdvancedSettings, "cbOverrideAdvancedSettings");
            this.cbOverrideAdvancedSettings.Checked = true;
            this.cbOverrideAdvancedSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverrideAdvancedSettings.Name = "cbOverrideAdvancedSettings";
            this.cbOverrideAdvancedSettings.UseVisualStyleBackColor = true;
            this.cbOverrideAdvancedSettings.CheckedChanged += new System.EventHandler(this.cbUseDefaultAdvancedSettings_CheckedChanged);
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
            this.tttvMain.TreeViewSize = 190;
            this.tttvMain.TabChanged += new ShareX.HelpersLib.TabToTreeView.TabChangedEventHandler(this.tttvMain_TabChanged);
            // 
            // TaskSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcTaskSettings);
            this.Controls.Add(this.tttvMain);
            this.Name = "TaskSettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Resize += new System.EventHandler(this.TaskSettingsForm_Resize);
            this.tcTaskSettings.ResumeLayout(false);
            this.tpTask.ResumeLayout(false);
            this.tpTask.PerformLayout();
            this.cmsDestinations.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tcGeneral.ResumeLayout(false);
            this.tpGeneralMain.ResumeLayout(false);
            this.tpGeneralMain.PerformLayout();
            this.tpNotifications.ResumeLayout(false);
            this.tpNotifications.PerformLayout();
            this.gbToastWindow.ResumeLayout(false);
            this.gbToastWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowSizeHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowSizeWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowFadeDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToastWindowDuration)).EndInit();
            this.tpImage.ResumeLayout(false);
            this.tcImage.ResumeLayout(false);
            this.tpQuality.ResumeLayout(false);
            this.tpQuality.PerformLayout();
            this.pImage.ResumeLayout(false);
            this.pImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageAutoUseJPEGSize)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCustomRegionX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenshotDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureShadowOffset)).EndInit();
            this.tpRegionCapture.ResumeLayout(false);
            this.tpRegionCapture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureBackgroundDimStrength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureFPSLimit)).EndInit();
            this.flpRegionCaptureFixedSize.ResumeLayout(false);
            this.flpRegionCaptureFixedSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureFixedSizeWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureFixedSizeHeight)).EndInit();
            this.pRegionCaptureSnapSizes.ResumeLayout(false);
            this.pRegionCaptureSnapSizes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureSnapSizesHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureSnapSizesWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureMagnifierPixelCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRegionCaptureMagnifierPixelSize)).EndInit();
            this.tpScreenRecorder.ResumeLayout(false);
            this.tpScreenRecorder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecordFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreenRecorderStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGIFFPS)).EndInit();
            this.tpOCR.ResumeLayout(false);
            this.tpOCR.PerformLayout();
            this.tpUpload.ResumeLayout(false);
            this.tcUpload.ResumeLayout(false);
            this.tpUploadMain.ResumeLayout(false);
            this.tpUploadMain.PerformLayout();
            this.tpFileNaming.ResumeLayout(false);
            this.tpFileNaming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoIncrementNumber)).EndInit();
            this.tpUploadClipboard.ResumeLayout(false);
            this.tpUploadClipboard.PerformLayout();
            this.tpUploaderFilters.ResumeLayout(false);
            this.tpUploaderFilters.PerformLayout();
            this.tpActions.ResumeLayout(false);
            this.tpActions.PerformLayout();
            this.pActions.ResumeLayout(false);
            this.pActions.PerformLayout();
            this.tpWatchFolders.ResumeLayout(false);
            this.tpWatchFolders.PerformLayout();
            this.tpTools.ResumeLayout(false);
            this.tpTools.PerformLayout();
            this.pTools.ResumeLayout(false);
            this.pTools.PerformLayout();
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
        private System.Windows.Forms.CheckBox cbOverrideAfterCaptureSettings;
        private System.Windows.Forms.CheckBox cbOverrideAfterUploadSettings;
        private System.Windows.Forms.CheckBox cbOverrideDestinationSettings;
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
        private System.Windows.Forms.Label lblImageSizeLimitHint;
        private System.Windows.Forms.ComboBox cbImageFormat;
        private System.Windows.Forms.Label lblImageJPEGQualityHint;
        private System.Windows.Forms.Label lblImageJPEGQuality;
        private System.Windows.Forms.ComboBox cbImageGIFQuality;
        private System.Windows.Forms.Label lblImageGIFQuality;
        private System.Windows.Forms.NumericUpDown nudImageJPEGQuality;
        private System.Windows.Forms.NumericUpDown nudImageAutoUseJPEGSize;
        private System.Windows.Forms.TabPage tpEffects;
        private System.Windows.Forms.TabControl tcCapture;
        private System.Windows.Forms.TabPage tpCaptureGeneral;
        private System.Windows.Forms.CheckBox cbCaptureAutoHideTaskbar;
        private System.Windows.Forms.Label lblScreenshotDelayInfo;
        private System.Windows.Forms.NumericUpDown nudScreenshotDelay;
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
        private System.Windows.Forms.TabPage tpUploadMain;
        private System.Windows.Forms.CheckBox cbFileUploadUseNamePattern;
        private System.Windows.Forms.Label lblNameFormatPattern;
        private System.Windows.Forms.TextBox txtNameFormatPatternActiveWindow;
        private System.Windows.Forms.Label lblNameFormatPatternActiveWindow;
        private System.Windows.Forms.TextBox txtNameFormatPattern;
        private System.Windows.Forms.Label lblNameFormatPatternPreview;
        private System.Windows.Forms.Label lblNameFormatPatternPreviewActiveWindow;
        private System.Windows.Forms.TabPage tpUploadClipboard;
        private System.Windows.Forms.CheckBox cbClipboardUploadShortenURL;
        private System.Windows.Forms.TabPage tpAdvanced;
        private System.Windows.Forms.PropertyGrid pgTaskSettings;
        private System.Windows.Forms.CheckBox cbOverrideImageSettings;
        private System.Windows.Forms.CheckBox cbOverrideCaptureSettings;
        private System.Windows.Forms.CheckBox cbOverrideActions;
        private System.Windows.Forms.CheckBox cbOverrideUploadSettings;
        private System.Windows.Forms.Panel pActions;
        private System.Windows.Forms.CheckBox cbOverrideAdvancedSettings;
        private System.Windows.Forms.CheckBox cbScreenRecorderFixedDuration;
        private System.Windows.Forms.NumericUpDown nudGIFFPS;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderDuration;
        private System.Windows.Forms.Label lblGIFFPS;
        private System.Windows.Forms.TabPage tpWatchFolders;
        private System.Windows.Forms.CheckBox cbWatchFolderEnabled;
        private HelpersLib.MyListView lvWatchFolderList;
        private System.Windows.Forms.ColumnHeader chWatchFolderFolderPath;
        private System.Windows.Forms.ColumnHeader chWatchFolderFilter;
        private System.Windows.Forms.ColumnHeader chWatchFolderIncludeSubdirectories;
        private System.Windows.Forms.Button btnWatchFolderRemove;
        private System.Windows.Forms.Button btnWatchFolderAdd;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.CheckBox cbPlaySoundAfterCapture;
        private System.Windows.Forms.CheckBox cbPlaySoundAfterUpload;
        private System.Windows.Forms.CheckBox cbOverrideGeneralSettings;
        private System.Windows.Forms.TabPage tpTools;
        private System.Windows.Forms.NumericUpDown nudScreenRecorderStartDelay;
        private System.Windows.Forms.Button btnImageEffects;
        private System.Windows.Forms.CheckBox cbImageEffectOnlyRegionCapture;
        private System.Windows.Forms.CheckBox cbShowImageEffectsWindowAfterCapture;
        private System.Windows.Forms.CheckBox cbOverrideFTPAccount;
        private System.Windows.Forms.ComboBox cbFTPAccounts;
        private System.Windows.Forms.ContextMenuStrip cmsDestinations;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiTextUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiURLShorteners;
        private System.Windows.Forms.ToolStripMenuItem tsmiURLSharingServices;
        private System.Windows.Forms.ComboBox cbImageFileExist;
        private System.Windows.Forms.Label lblImageFileExist;
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
        private System.Windows.Forms.CheckBox cbClipboardUploadURLContents;
        private System.Windows.Forms.NumericUpDown nudScreenRecordFPS;
        private System.Windows.Forms.Label lblScreenRecordFPS;
        private System.Windows.Forms.Label lblScreenRecorderFixedDuration;
        private System.Windows.Forms.CheckBox cbClipboardUploadShareURL;
        private System.Windows.Forms.ColumnHeader chActionsExtensions;
        private System.Windows.Forms.Button btnActionsDuplicate;
        private System.Windows.Forms.Label lblImageEffectsNote;
        private System.Windows.Forms.Label lblCaptureShadowOffset;
        private System.Windows.Forms.CheckBox cbScreenRecordAutoStart;
        private System.Windows.Forms.Label lblScreenRecorderStartDelay;
        private HelpersLib.TabToTreeView tttvMain;
        private System.Windows.Forms.Panel pImage;
        private System.Windows.Forms.Panel pCapture;
        private System.Windows.Forms.ComboBox cbCustomUploaders;
        private System.Windows.Forms.CheckBox cbOverrideCustomUploader;
        private System.Windows.Forms.Button btnScreenRecorderFFmpegOptions;
        private System.Windows.Forms.ComboBox cbNameFormatTimeZone;
        private System.Windows.Forms.CheckBox cbNameFormatCustomTimeZone;
        private System.Windows.Forms.Label lblCaptureCustomRegionWidth;
        private System.Windows.Forms.Label lblCaptureCustomRegionHeight;
        private System.Windows.Forms.Label lblCaptureCustomRegionY;
        private System.Windows.Forms.Label lblCaptureCustomRegionX;
        private System.Windows.Forms.NumericUpDown nudCaptureCustomRegionHeight;
        private System.Windows.Forms.NumericUpDown nudCaptureCustomRegionWidth;
        private System.Windows.Forms.NumericUpDown nudCaptureCustomRegionY;
        private System.Windows.Forms.NumericUpDown nudCaptureCustomRegionX;
        private System.Windows.Forms.CheckBox cbScreenRecorderShowCursor;
        private System.Windows.Forms.CheckBox cbOverrideToolsSettings;
        private System.Windows.Forms.TabPage tpFileNaming;
        private System.Windows.Forms.Label lblCaptureCustomRegion;
        private System.Windows.Forms.Button btnCaptureCustomRegionSelectRectangle;
        private System.Windows.Forms.CheckBox cbRegionCaptureMultiRegionMode;
        private System.Windows.Forms.Label lblRegionCaptureMouseRightClickAction;
        private System.Windows.Forms.ComboBox cbRegionCaptureMouse5ClickAction;
        private System.Windows.Forms.Label lblRegionCaptureMouse5ClickAction;
        private System.Windows.Forms.ComboBox cbRegionCaptureMouse4ClickAction;
        private System.Windows.Forms.Label lblRegionCaptureMouse4ClickAction;
        private System.Windows.Forms.ComboBox cbRegionCaptureMouseMiddleClickAction;
        private System.Windows.Forms.Label lblRegionCaptureMouseMiddleClickAction;
        private System.Windows.Forms.ComboBox cbRegionCaptureMouseRightClickAction;
        private System.Windows.Forms.CheckBox cbRegionCaptureDetectWindows;
        private System.Windows.Forms.CheckBox cbRegionCaptureDetectControls;
        private System.Windows.Forms.CheckBox cbRegionCaptureUseCustomInfoText;
        private System.Windows.Forms.TextBox txtRegionCaptureCustomInfoText;
        private System.Windows.Forms.Label lblRegionCaptureSnapSizes;
        private System.Windows.Forms.ComboBox cbRegionCaptureSnapSizes;
        private System.Windows.Forms.Button btnRegionCaptureSnapSizesRemove;
        private System.Windows.Forms.Button btnRegionCaptureSnapSizesAdd;
        private System.Windows.Forms.Panel pRegionCaptureSnapSizes;
        private System.Windows.Forms.Button btnRegionCaptureSnapSizesDialogCancel;
        private System.Windows.Forms.Button btnRegionCaptureSnapSizesDialogAdd;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureSnapSizesHeight;
        private System.Windows.Forms.Label RegionCaptureSnapSizesHeight;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureSnapSizesWidth;
        private System.Windows.Forms.Label lblRegionCaptureSnapSizesWidth;
        private System.Windows.Forms.CheckBox cbRegionCaptureShowInfo;
        private System.Windows.Forms.CheckBox cbRegionCaptureShowMagnifier;
        private System.Windows.Forms.Label lblRegionCaptureMagnifierPixelCount;
        private System.Windows.Forms.CheckBox cbRegionCaptureUseSquareMagnifier;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureMagnifierPixelCount;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureMagnifierPixelSize;
        private System.Windows.Forms.Label lblRegionCaptureMagnifierPixelSize;
        private System.Windows.Forms.CheckBox cbRegionCaptureShowCrosshair;
        private System.Windows.Forms.FlowLayoutPanel flpRegionCaptureFixedSize;
        private System.Windows.Forms.Label lblRegionCaptureFixedSizeWidth;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureFixedSizeWidth;
        private System.Windows.Forms.Label lblRegionCaptureFixedSizeHeight;
        private System.Windows.Forms.CheckBox cbRegionCaptureIsFixedSize;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureFixedSizeHeight;
        private System.Windows.Forms.CheckBox cbRegionCaptureShowFPS;
        private System.Windows.Forms.CheckBox cbImageAutoUseJPEG;
        private System.Windows.Forms.Panel pTools;
        private System.Windows.Forms.TextBox txtToolsScreenColorPickerFormat;
        private System.Windows.Forms.Label lblToolsScreenColorPickerFormat;
        private System.Windows.Forms.TabPage tpUploaderFilters;
        private HelpersLib.MyListView lvUploaderFiltersList;
        private System.Windows.Forms.ColumnHeader chUploaderFiltersName;
        private System.Windows.Forms.ColumnHeader chUploaderFiltersExtension;
        private System.Windows.Forms.Button btnUploaderFiltersRemove;
        private System.Windows.Forms.Button btnUploaderFiltersUpdate;
        private System.Windows.Forms.Button btnUploaderFiltersAdd;
        private System.Windows.Forms.Label lblUploaderFiltersDestination;
        private System.Windows.Forms.ComboBox cbUploaderFiltersDestination;
        private System.Windows.Forms.Label lblUploaderFiltersExtensionsExample;
        private System.Windows.Forms.Label lblUploaderFiltersExtensions;
        private System.Windows.Forms.TextBox txtUploaderFiltersExtensions;
        private System.Windows.Forms.ComboBox cbImagePNGBitDepth;
        private System.Windows.Forms.Label lblImagePNGBitDepth;
        private System.Windows.Forms.Button btnWatchFolderEdit;
        private System.Windows.Forms.CheckBox cbScreenRecordConfirmAbort;
        private System.Windows.Forms.CheckBox cbFileUploadReplaceProblematicCharacters;
        private System.Windows.Forms.CheckBox cbScreenRecordTwoPassEncoding;
        private System.Windows.Forms.TabPage tpOCR;
        private System.Windows.Forms.Label lblOCRDefaultLanguage;
        private System.Windows.Forms.ComboBox cbCaptureOCRDefaultLanguage;
        private System.Windows.Forms.CheckBox cbCaptureOCRSilent;
        private System.Windows.Forms.CheckBox cbCaptureOCRAutoCopy;
        private System.Windows.Forms.CheckBox cbCloseWindowAfterOpenServiceLink;
        private System.Windows.Forms.Label lblScreenshotDelay;
        private System.Windows.Forms.Label lblAutoIncrementNumber;
        private System.Windows.Forms.NumericUpDown nudAutoIncrementNumber;
        private System.Windows.Forms.Button btnAutoIncrementNumber;
        private System.Windows.Forms.Label lblActionsNote;
        private System.Windows.Forms.CheckBox cbScreenRecordTransparentRegion;
        private System.Windows.Forms.CheckBox cbOverrideScreenshotsFolder;
        private System.Windows.Forms.Button btnScreenshotsFolderBrowse;
        private System.Windows.Forms.TextBox txtScreenshotsFolder;
        private System.Windows.Forms.CheckBox cbURLRegexReplace;
        private System.Windows.Forms.Label lblURLRegexReplacePattern;
        private System.Windows.Forms.Label lblURLRegexReplaceReplacement;
        private System.Windows.Forms.TextBox txtURLRegexReplacePattern;
        private System.Windows.Forms.TextBox txtURLRegexReplaceReplacement;
        private System.Windows.Forms.TextBox txtToolsScreenColorPickerInfoText;
        private System.Windows.Forms.Label lblToolsScreenColorPickerInfoText;
        private System.Windows.Forms.TextBox txtToolsScreenColorPickerFormatCtrl;
        private System.Windows.Forms.Label lblToolsScreenColorPickerFormatCtrl;
        private System.Windows.Forms.TabControl tcGeneral;
        private System.Windows.Forms.TabPage tpGeneralMain;
        private System.Windows.Forms.TabPage tpNotifications;
        private System.Windows.Forms.CheckBox cbDisableNotificationsOnFullscreen;
        private System.Windows.Forms.GroupBox gbToastWindow;
        private System.Windows.Forms.Label lblToastWindowLeftClickAction;
        private System.Windows.Forms.Label lblToastWindowSize;
        private System.Windows.Forms.Label lblToastWindowPlacement;
        private System.Windows.Forms.Label lblToastWindowFadeDuration;
        private System.Windows.Forms.Label lblToastWindowDuration;
        private System.Windows.Forms.Label lblToastWindowMiddleClickAction;
        private System.Windows.Forms.Label lblToastWindowRightClickAction;
        private System.Windows.Forms.Label lblToastWindowSizeX;
        private System.Windows.Forms.ComboBox cbToastWindowMiddleClickAction;
        private System.Windows.Forms.ComboBox cbToastWindowRightClickAction;
        private System.Windows.Forms.ComboBox cbToastWindowLeftClickAction;
        private System.Windows.Forms.NumericUpDown nudToastWindowSizeHeight;
        private System.Windows.Forms.NumericUpDown nudToastWindowSizeWidth;
        private System.Windows.Forms.ComboBox cbToastWindowPlacement;
        private System.Windows.Forms.NumericUpDown nudToastWindowFadeDuration;
        private System.Windows.Forms.NumericUpDown nudToastWindowDuration;
        private System.Windows.Forms.TextBox txtCustomErrorSoundPath;
        private System.Windows.Forms.TextBox txtCustomTaskCompletedSoundPath;
        private System.Windows.Forms.TextBox txtCustomCaptureSoundPath;
        private System.Windows.Forms.CheckBox cbUseCustomErrorSound;
        private System.Windows.Forms.CheckBox cbUseCustomTaskCompletedSound;
        private System.Windows.Forms.CheckBox cbUseCustomCaptureSound;
        private System.Windows.Forms.Button btnCustomErrorSoundPath;
        private System.Windows.Forms.Button btnCustomTaskCompletedSoundPath;
        private System.Windows.Forms.Button btnCustomCaptureSoundPath;
        private System.Windows.Forms.CheckBox cbShowToastNotificationAfterTaskCompleted;
        private System.Windows.Forms.Label lblToastWindowFadeDurationSeconds;
        private System.Windows.Forms.Label lblToastWindowDurationSeconds;
        private System.Windows.Forms.Button btnActions;
        private System.Windows.Forms.CheckBox cbImageAutoJPEGQuality;
        private System.Windows.Forms.Label lblTask;
        private System.Windows.Forms.CheckBox cbToastWindowAutoHide;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureFPSLimit;
        private System.Windows.Forms.Label lblRegionCaptureFPSLimit;
        private System.Windows.Forms.CheckBox cbRegionCaptureActiveMonitorMode;
        private System.Windows.Forms.Button btnCaptureOCRHelp;
        private System.Windows.Forms.CheckBox cbUseRandomImageEffect;
        private System.Windows.Forms.Label lblCaptureCustomWindow;
        private System.Windows.Forms.TextBox txtCaptureCustomWindow;
        private System.Windows.Forms.NumericUpDown nudRegionCaptureBackgroundDimStrength;
        private System.Windows.Forms.Label lblRegionCaptureBackgroundDimStrength;
        private System.Windows.Forms.Label lblRegionCaptureBackgroundDimStrengthHint;
        private System.Windows.Forms.CheckBox cbPlaySoundAfterAction;
        private System.Windows.Forms.Button btnCustomActionCompletedSoundPath;
        private System.Windows.Forms.TextBox txtCustomActionCompletedSoundPath;
        private System.Windows.Forms.CheckBox cbUseCustomActionCompletedSound;
    }
}