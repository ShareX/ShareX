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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskSettingsForm));
            cmsAfterCapture = new System.Windows.Forms.ContextMenuStrip(components);
            cmsAfterUpload = new System.Windows.Forms.ContextMenuStrip(components);
            cbOverrideAfterCaptureSettings = new System.Windows.Forms.CheckBox();
            cbOverrideAfterUploadSettings = new System.Windows.Forms.CheckBox();
            cbOverrideDestinationSettings = new System.Windows.Forms.CheckBox();
            lblDescription = new System.Windows.Forms.Label();
            tbDescription = new System.Windows.Forms.TextBox();
            cmsTask = new System.Windows.Forms.ContextMenuStrip(components);
            tcTaskSettings = new System.Windows.Forms.TabControl();
            tpTask = new System.Windows.Forms.TabPage();
            lblTask = new System.Windows.Forms.Label();
            btnScreenshotsFolderBrowse = new System.Windows.Forms.Button();
            txtScreenshotsFolder = new System.Windows.Forms.TextBox();
            cbOverrideScreenshotsFolder = new System.Windows.Forms.CheckBox();
            cbCustomUploaders = new System.Windows.Forms.ComboBox();
            cbOverrideCustomUploader = new System.Windows.Forms.CheckBox();
            cbOverrideFTPAccount = new System.Windows.Forms.CheckBox();
            cbFTPAccounts = new System.Windows.Forms.ComboBox();
            btnAfterCapture = new ShareX.HelpersLib.MenuButton();
            btnAfterUpload = new ShareX.HelpersLib.MenuButton();
            btnDestinations = new ShareX.HelpersLib.MenuButton();
            cmsDestinations = new System.Windows.Forms.ContextMenuStrip(components);
            tsmiImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            tsmiURLSharingServices = new System.Windows.Forms.ToolStripMenuItem();
            btnTask = new ShareX.HelpersLib.MenuButton();
            tpGeneral = new System.Windows.Forms.TabPage();
            tcGeneral = new System.Windows.Forms.TabControl();
            tpGeneralMain = new System.Windows.Forms.TabPage();
            cbOverrideGeneralSettings = new System.Windows.Forms.CheckBox();
            tpNotifications = new System.Windows.Forms.TabPage();
            btnCustomActionCompletedSoundPath = new System.Windows.Forms.Button();
            txtCustomActionCompletedSoundPath = new System.Windows.Forms.TextBox();
            cbUseCustomActionCompletedSound = new System.Windows.Forms.CheckBox();
            cbPlaySoundAfterAction = new System.Windows.Forms.CheckBox();
            cbShowToastNotificationAfterTaskCompleted = new System.Windows.Forms.CheckBox();
            btnCustomErrorSoundPath = new System.Windows.Forms.Button();
            btnCustomTaskCompletedSoundPath = new System.Windows.Forms.Button();
            btnCustomCaptureSoundPath = new System.Windows.Forms.Button();
            txtCustomErrorSoundPath = new System.Windows.Forms.TextBox();
            txtCustomTaskCompletedSoundPath = new System.Windows.Forms.TextBox();
            txtCustomCaptureSoundPath = new System.Windows.Forms.TextBox();
            cbUseCustomErrorSound = new System.Windows.Forms.CheckBox();
            cbUseCustomTaskCompletedSound = new System.Windows.Forms.CheckBox();
            cbUseCustomCaptureSound = new System.Windows.Forms.CheckBox();
            gbToastWindow = new System.Windows.Forms.GroupBox();
            cbToastWindowAutoHide = new System.Windows.Forms.CheckBox();
            lblToastWindowFadeDurationSeconds = new System.Windows.Forms.Label();
            lblToastWindowDurationSeconds = new System.Windows.Forms.Label();
            lblToastWindowSizeX = new System.Windows.Forms.Label();
            cbToastWindowMiddleClickAction = new System.Windows.Forms.ComboBox();
            cbToastWindowRightClickAction = new System.Windows.Forms.ComboBox();
            cbToastWindowLeftClickAction = new System.Windows.Forms.ComboBox();
            nudToastWindowSizeHeight = new System.Windows.Forms.NumericUpDown();
            nudToastWindowSizeWidth = new System.Windows.Forms.NumericUpDown();
            cbToastWindowPlacement = new System.Windows.Forms.ComboBox();
            nudToastWindowFadeDuration = new System.Windows.Forms.NumericUpDown();
            cbDisableNotificationsOnFullscreen = new System.Windows.Forms.CheckBox();
            nudToastWindowDuration = new System.Windows.Forms.NumericUpDown();
            lblToastWindowMiddleClickAction = new System.Windows.Forms.Label();
            lblToastWindowRightClickAction = new System.Windows.Forms.Label();
            lblToastWindowLeftClickAction = new System.Windows.Forms.Label();
            lblToastWindowSize = new System.Windows.Forms.Label();
            lblToastWindowPlacement = new System.Windows.Forms.Label();
            lblToastWindowFadeDuration = new System.Windows.Forms.Label();
            lblToastWindowDuration = new System.Windows.Forms.Label();
            cbPlaySoundAfterCapture = new System.Windows.Forms.CheckBox();
            cbPlaySoundAfterUpload = new System.Windows.Forms.CheckBox();
            tpImage = new System.Windows.Forms.TabPage();
            tcImage = new System.Windows.Forms.TabControl();
            tpQuality = new System.Windows.Forms.TabPage();
            pImage = new System.Windows.Forms.Panel();
            cbImageAutoJPEGQuality = new System.Windows.Forms.CheckBox();
            cbImagePNGBitDepth = new System.Windows.Forms.ComboBox();
            lblImagePNGBitDepth = new System.Windows.Forms.Label();
            cbImageAutoUseJPEG = new System.Windows.Forms.CheckBox();
            lblImageFormat = new System.Windows.Forms.Label();
            cbImageFileExist = new System.Windows.Forms.ComboBox();
            lblImageFileExist = new System.Windows.Forms.Label();
            nudImageAutoUseJPEGSize = new System.Windows.Forms.NumericUpDown();
            lblImageSizeLimitHint = new System.Windows.Forms.Label();
            nudImageJPEGQuality = new System.Windows.Forms.NumericUpDown();
            cbImageFormat = new System.Windows.Forms.ComboBox();
            lblImageJPEGQualityHint = new System.Windows.Forms.Label();
            lblImageGIFQuality = new System.Windows.Forms.Label();
            lblImageJPEGQuality = new System.Windows.Forms.Label();
            cbImageGIFQuality = new System.Windows.Forms.ComboBox();
            cbOverrideImageSettings = new System.Windows.Forms.CheckBox();
            tpEffects = new System.Windows.Forms.TabPage();
            cbUseRandomImageEffect = new System.Windows.Forms.CheckBox();
            lblImageEffectsNote = new System.Windows.Forms.Label();
            cbShowImageEffectsWindowAfterCapture = new System.Windows.Forms.CheckBox();
            cbImageEffectOnlyRegionCapture = new System.Windows.Forms.CheckBox();
            btnImageEffects = new System.Windows.Forms.Button();
            tpThumbnail = new System.Windows.Forms.TabPage();
            cbThumbnailIfSmaller = new System.Windows.Forms.CheckBox();
            lblThumbnailNamePreview = new System.Windows.Forms.Label();
            lblThumbnailName = new System.Windows.Forms.Label();
            txtThumbnailName = new System.Windows.Forms.TextBox();
            lblThumbnailHeight = new System.Windows.Forms.Label();
            lblThumbnailWidth = new System.Windows.Forms.Label();
            nudThumbnailHeight = new System.Windows.Forms.NumericUpDown();
            nudThumbnailWidth = new System.Windows.Forms.NumericUpDown();
            tpCapture = new System.Windows.Forms.TabPage();
            tcCapture = new System.Windows.Forms.TabControl();
            tpCaptureGeneral = new System.Windows.Forms.TabPage();
            pCapture = new System.Windows.Forms.Panel();
            cbCaptureAutoHideDesktopIcons = new System.Windows.Forms.CheckBox();
            txtCaptureCustomWindow = new System.Windows.Forms.TextBox();
            lblCaptureCustomWindow = new System.Windows.Forms.Label();
            lblScreenshotDelay = new System.Windows.Forms.Label();
            btnCaptureCustomRegionSelectRectangle = new System.Windows.Forms.Button();
            lblCaptureCustomRegion = new System.Windows.Forms.Label();
            lblCaptureCustomRegionWidth = new System.Windows.Forms.Label();
            lblCaptureCustomRegionHeight = new System.Windows.Forms.Label();
            lblCaptureCustomRegionY = new System.Windows.Forms.Label();
            lblCaptureCustomRegionX = new System.Windows.Forms.Label();
            nudCaptureCustomRegionHeight = new System.Windows.Forms.NumericUpDown();
            nudCaptureCustomRegionWidth = new System.Windows.Forms.NumericUpDown();
            nudCaptureCustomRegionY = new System.Windows.Forms.NumericUpDown();
            nudCaptureCustomRegionX = new System.Windows.Forms.NumericUpDown();
            cbShowCursor = new System.Windows.Forms.CheckBox();
            lblCaptureShadowOffset = new System.Windows.Forms.Label();
            cbCaptureTransparent = new System.Windows.Forms.CheckBox();
            cbCaptureAutoHideTaskbar = new System.Windows.Forms.CheckBox();
            cbCaptureShadow = new System.Windows.Forms.CheckBox();
            lblScreenshotDelayInfo = new System.Windows.Forms.Label();
            cbCaptureClientArea = new System.Windows.Forms.CheckBox();
            nudScreenshotDelay = new System.Windows.Forms.NumericUpDown();
            nudCaptureShadowOffset = new System.Windows.Forms.NumericUpDown();
            cbOverrideCaptureSettings = new System.Windows.Forms.CheckBox();
            tpRegionCapture = new System.Windows.Forms.TabPage();
            lblRegionCaptureBackgroundDimStrengthHint = new System.Windows.Forms.Label();
            nudRegionCaptureBackgroundDimStrength = new System.Windows.Forms.NumericUpDown();
            lblRegionCaptureBackgroundDimStrength = new System.Windows.Forms.Label();
            cbRegionCaptureActiveMonitorMode = new System.Windows.Forms.CheckBox();
            nudRegionCaptureFPSLimit = new System.Windows.Forms.NumericUpDown();
            lblRegionCaptureFPSLimit = new System.Windows.Forms.Label();
            cbRegionCaptureShowFPS = new System.Windows.Forms.CheckBox();
            flpRegionCaptureFixedSize = new System.Windows.Forms.FlowLayoutPanel();
            lblRegionCaptureFixedSizeWidth = new System.Windows.Forms.Label();
            nudRegionCaptureFixedSizeWidth = new System.Windows.Forms.NumericUpDown();
            lblRegionCaptureFixedSizeHeight = new System.Windows.Forms.Label();
            nudRegionCaptureFixedSizeHeight = new System.Windows.Forms.NumericUpDown();
            cbRegionCaptureIsFixedSize = new System.Windows.Forms.CheckBox();
            cbRegionCaptureShowCrosshair = new System.Windows.Forms.CheckBox();
            lblRegionCaptureMagnifierPixelSize = new System.Windows.Forms.Label();
            lblRegionCaptureMagnifierPixelCount = new System.Windows.Forms.Label();
            cbRegionCaptureUseSquareMagnifier = new System.Windows.Forms.CheckBox();
            cbRegionCaptureShowMagnifier = new System.Windows.Forms.CheckBox();
            cbRegionCaptureShowInfo = new System.Windows.Forms.CheckBox();
            btnRegionCaptureSnapSizesRemove = new System.Windows.Forms.Button();
            btnRegionCaptureSnapSizesAdd = new System.Windows.Forms.Button();
            cbRegionCaptureSnapSizes = new System.Windows.Forms.ComboBox();
            lblRegionCaptureSnapSizes = new System.Windows.Forms.Label();
            cbRegionCaptureUseCustomInfoText = new System.Windows.Forms.CheckBox();
            cbRegionCaptureDetectControls = new System.Windows.Forms.CheckBox();
            cbRegionCaptureDetectWindows = new System.Windows.Forms.CheckBox();
            cbRegionCaptureMouse5ClickAction = new System.Windows.Forms.ComboBox();
            lblRegionCaptureMouse5ClickAction = new System.Windows.Forms.Label();
            cbRegionCaptureMouse4ClickAction = new System.Windows.Forms.ComboBox();
            lblRegionCaptureMouse4ClickAction = new System.Windows.Forms.Label();
            cbRegionCaptureMouseMiddleClickAction = new System.Windows.Forms.ComboBox();
            lblRegionCaptureMouseMiddleClickAction = new System.Windows.Forms.Label();
            cbRegionCaptureMouseRightClickAction = new System.Windows.Forms.ComboBox();
            lblRegionCaptureMouseRightClickAction = new System.Windows.Forms.Label();
            cbRegionCaptureMultiRegionMode = new System.Windows.Forms.CheckBox();
            pRegionCaptureSnapSizes = new System.Windows.Forms.Panel();
            btnRegionCaptureSnapSizesDialogCancel = new System.Windows.Forms.Button();
            btnRegionCaptureSnapSizesDialogAdd = new System.Windows.Forms.Button();
            nudRegionCaptureSnapSizesHeight = new System.Windows.Forms.NumericUpDown();
            RegionCaptureSnapSizesHeight = new System.Windows.Forms.Label();
            nudRegionCaptureSnapSizesWidth = new System.Windows.Forms.NumericUpDown();
            lblRegionCaptureSnapSizesWidth = new System.Windows.Forms.Label();
            txtRegionCaptureCustomInfoText = new System.Windows.Forms.TextBox();
            nudRegionCaptureMagnifierPixelCount = new System.Windows.Forms.NumericUpDown();
            nudRegionCaptureMagnifierPixelSize = new System.Windows.Forms.NumericUpDown();
            tpScreenRecorder = new System.Windows.Forms.TabPage();
            cbScreenRecordTransparentRegion = new System.Windows.Forms.CheckBox();
            cbScreenRecordTwoPassEncoding = new System.Windows.Forms.CheckBox();
            cbScreenRecordConfirmAbort = new System.Windows.Forms.CheckBox();
            cbScreenRecorderShowCursor = new System.Windows.Forms.CheckBox();
            btnScreenRecorderFFmpegOptions = new System.Windows.Forms.Button();
            lblScreenRecorderStartDelay = new System.Windows.Forms.Label();
            cbScreenRecordAutoStart = new System.Windows.Forms.CheckBox();
            lblScreenRecorderFixedDuration = new System.Windows.Forms.Label();
            nudScreenRecordFPS = new System.Windows.Forms.NumericUpDown();
            lblScreenRecordFPS = new System.Windows.Forms.Label();
            nudScreenRecorderDuration = new System.Windows.Forms.NumericUpDown();
            nudScreenRecorderStartDelay = new System.Windows.Forms.NumericUpDown();
            cbScreenRecorderFixedDuration = new System.Windows.Forms.CheckBox();
            nudGIFFPS = new System.Windows.Forms.NumericUpDown();
            lblGIFFPS = new System.Windows.Forms.Label();
            tpOCR = new System.Windows.Forms.TabPage();
            btnCaptureOCRHelp = new System.Windows.Forms.Button();
            cbCaptureOCRAutoCopy = new System.Windows.Forms.CheckBox();
            cbCloseWindowAfterOpenServiceLink = new System.Windows.Forms.CheckBox();
            cbCaptureOCRSilent = new System.Windows.Forms.CheckBox();
            lblOCRDefaultLanguage = new System.Windows.Forms.Label();
            cbCaptureOCRDefaultLanguage = new System.Windows.Forms.ComboBox();
            tpUpload = new System.Windows.Forms.TabPage();
            tcUpload = new System.Windows.Forms.TabControl();
            tpUploadMain = new System.Windows.Forms.TabPage();
            cbOverrideUploadSettings = new System.Windows.Forms.CheckBox();
            tpFileNaming = new System.Windows.Forms.TabPage();
            txtURLRegexReplaceReplacement = new System.Windows.Forms.TextBox();
            lblURLRegexReplaceReplacement = new System.Windows.Forms.Label();
            txtURLRegexReplacePattern = new System.Windows.Forms.TextBox();
            lblURLRegexReplacePattern = new System.Windows.Forms.Label();
            cbURLRegexReplace = new System.Windows.Forms.CheckBox();
            btnAutoIncrementNumber = new System.Windows.Forms.Button();
            lblAutoIncrementNumber = new System.Windows.Forms.Label();
            nudAutoIncrementNumber = new System.Windows.Forms.NumericUpDown();
            cbFileUploadReplaceProblematicCharacters = new System.Windows.Forms.CheckBox();
            cbNameFormatCustomTimeZone = new System.Windows.Forms.CheckBox();
            lblNameFormatPatternPreview = new System.Windows.Forms.Label();
            lblNameFormatPatternActiveWindow = new System.Windows.Forms.Label();
            lblNameFormatPatternPreviewActiveWindow = new System.Windows.Forms.Label();
            cbNameFormatTimeZone = new System.Windows.Forms.ComboBox();
            txtNameFormatPatternActiveWindow = new System.Windows.Forms.TextBox();
            cbFileUploadUseNamePattern = new System.Windows.Forms.CheckBox();
            lblNameFormatPattern = new System.Windows.Forms.Label();
            txtNameFormatPattern = new System.Windows.Forms.TextBox();
            tpUploadClipboard = new System.Windows.Forms.TabPage();
            cbClipboardUploadShareURL = new System.Windows.Forms.CheckBox();
            cbClipboardUploadURLContents = new System.Windows.Forms.CheckBox();
            cbClipboardUploadAutoIndexFolder = new System.Windows.Forms.CheckBox();
            cbClipboardUploadShortenURL = new System.Windows.Forms.CheckBox();
            tpUploaderFilters = new System.Windows.Forms.TabPage();
            lvUploaderFiltersList = new ShareX.HelpersLib.MyListView();
            chUploaderFiltersName = new System.Windows.Forms.ColumnHeader();
            chUploaderFiltersExtension = new System.Windows.Forms.ColumnHeader();
            btnUploaderFiltersRemove = new System.Windows.Forms.Button();
            btnUploaderFiltersUpdate = new System.Windows.Forms.Button();
            btnUploaderFiltersAdd = new System.Windows.Forms.Button();
            lblUploaderFiltersDestination = new System.Windows.Forms.Label();
            cbUploaderFiltersDestination = new System.Windows.Forms.ComboBox();
            lblUploaderFiltersExtensionsExample = new System.Windows.Forms.Label();
            lblUploaderFiltersExtensions = new System.Windows.Forms.Label();
            txtUploaderFiltersExtensions = new System.Windows.Forms.TextBox();
            tpActions = new System.Windows.Forms.TabPage();
            pActions = new System.Windows.Forms.Panel();
            btnActions = new System.Windows.Forms.Button();
            lblActionsNote = new System.Windows.Forms.Label();
            btnActionsDuplicate = new System.Windows.Forms.Button();
            btnActionsAdd = new System.Windows.Forms.Button();
            lvActions = new ShareX.HelpersLib.MyListView();
            chActionsName = new System.Windows.Forms.ColumnHeader();
            chActionsPath = new System.Windows.Forms.ColumnHeader();
            chActionsArgs = new System.Windows.Forms.ColumnHeader();
            chActionsExtensions = new System.Windows.Forms.ColumnHeader();
            btnActionsEdit = new System.Windows.Forms.Button();
            btnActionsRemove = new System.Windows.Forms.Button();
            cbOverrideActions = new System.Windows.Forms.CheckBox();
            tpWatchFolders = new System.Windows.Forms.TabPage();
            btnWatchFolderEdit = new System.Windows.Forms.Button();
            cbWatchFolderEnabled = new System.Windows.Forms.CheckBox();
            lvWatchFolderList = new ShareX.HelpersLib.MyListView();
            chWatchFolderFolderPath = new System.Windows.Forms.ColumnHeader();
            chWatchFolderFilter = new System.Windows.Forms.ColumnHeader();
            chWatchFolderIncludeSubdirectories = new System.Windows.Forms.ColumnHeader();
            btnWatchFolderRemove = new System.Windows.Forms.Button();
            btnWatchFolderAdd = new System.Windows.Forms.Button();
            tpTools = new System.Windows.Forms.TabPage();
            pTools = new System.Windows.Forms.Panel();
            txtToolsScreenColorPickerFormatCtrl = new System.Windows.Forms.TextBox();
            lblToolsScreenColorPickerFormatCtrl = new System.Windows.Forms.Label();
            txtToolsScreenColorPickerInfoText = new System.Windows.Forms.TextBox();
            lblToolsScreenColorPickerInfoText = new System.Windows.Forms.Label();
            txtToolsScreenColorPickerFormat = new System.Windows.Forms.TextBox();
            lblToolsScreenColorPickerFormat = new System.Windows.Forms.Label();
            cbOverrideToolsSettings = new System.Windows.Forms.CheckBox();
            tpAdvanced = new System.Windows.Forms.TabPage();
            pgTaskSettings = new System.Windows.Forms.PropertyGrid();
            cbOverrideAdvancedSettings = new System.Windows.Forms.CheckBox();
            tttvMain = new ShareX.HelpersLib.TabToTreeView();
            tcTaskSettings.SuspendLayout();
            tpTask.SuspendLayout();
            cmsDestinations.SuspendLayout();
            tpGeneral.SuspendLayout();
            tcGeneral.SuspendLayout();
            tpGeneralMain.SuspendLayout();
            tpNotifications.SuspendLayout();
            gbToastWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowSizeHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowSizeWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowFadeDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowDuration).BeginInit();
            tpImage.SuspendLayout();
            tcImage.SuspendLayout();
            tpQuality.SuspendLayout();
            pImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudImageAutoUseJPEGSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudImageJPEGQuality).BeginInit();
            tpEffects.SuspendLayout();
            tpThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailWidth).BeginInit();
            tpCapture.SuspendLayout();
            tcCapture.SuspendLayout();
            tpCaptureGeneral.SuspendLayout();
            pCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudScreenshotDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureShadowOffset).BeginInit();
            tpRegionCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureBackgroundDimStrength).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureFPSLimit).BeginInit();
            flpRegionCaptureFixedSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureFixedSizeWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureFixedSizeHeight).BeginInit();
            pRegionCaptureSnapSizes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureSnapSizesHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureSnapSizesWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureMagnifierPixelCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureMagnifierPixelSize).BeginInit();
            tpScreenRecorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudScreenRecordFPS).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudScreenRecorderDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudScreenRecorderStartDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGIFFPS).BeginInit();
            tpOCR.SuspendLayout();
            tpUpload.SuspendLayout();
            tcUpload.SuspendLayout();
            tpUploadMain.SuspendLayout();
            tpFileNaming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAutoIncrementNumber).BeginInit();
            tpUploadClipboard.SuspendLayout();
            tpUploaderFilters.SuspendLayout();
            tpActions.SuspendLayout();
            pActions.SuspendLayout();
            tpWatchFolders.SuspendLayout();
            tpTools.SuspendLayout();
            pTools.SuspendLayout();
            tpAdvanced.SuspendLayout();
            SuspendLayout();
            // 
            // cmsAfterCapture
            // 
            cmsAfterCapture.Name = "cmsAfterCapture";
            resources.ApplyResources(cmsAfterCapture, "cmsAfterCapture");
            // 
            // cmsAfterUpload
            // 
            cmsAfterUpload.Name = "cmsAfterCapture";
            resources.ApplyResources(cmsAfterUpload, "cmsAfterUpload");
            // 
            // cbOverrideAfterCaptureSettings
            // 
            resources.ApplyResources(cbOverrideAfterCaptureSettings, "cbOverrideAfterCaptureSettings");
            cbOverrideAfterCaptureSettings.Name = "cbOverrideAfterCaptureSettings";
            cbOverrideAfterCaptureSettings.UseVisualStyleBackColor = true;
            cbOverrideAfterCaptureSettings.CheckedChanged += cbUseDefaultAfterCaptureSettings_CheckedChanged;
            // 
            // cbOverrideAfterUploadSettings
            // 
            resources.ApplyResources(cbOverrideAfterUploadSettings, "cbOverrideAfterUploadSettings");
            cbOverrideAfterUploadSettings.Name = "cbOverrideAfterUploadSettings";
            cbOverrideAfterUploadSettings.UseVisualStyleBackColor = true;
            cbOverrideAfterUploadSettings.CheckedChanged += cbUseDefaultAfterUploadSettings_CheckedChanged;
            // 
            // cbOverrideDestinationSettings
            // 
            resources.ApplyResources(cbOverrideDestinationSettings, "cbOverrideDestinationSettings");
            cbOverrideDestinationSettings.Name = "cbOverrideDestinationSettings";
            cbOverrideDestinationSettings.UseVisualStyleBackColor = true;
            cbOverrideDestinationSettings.CheckedChanged += cbUseDefaultDestinationSettings_CheckedChanged;
            // 
            // lblDescription
            // 
            resources.ApplyResources(lblDescription, "lblDescription");
            lblDescription.Name = "lblDescription";
            // 
            // tbDescription
            // 
            resources.ApplyResources(tbDescription, "tbDescription");
            tbDescription.Name = "tbDescription";
            tbDescription.TextChanged += tbDescription_TextChanged;
            // 
            // cmsTask
            // 
            cmsTask.Name = "cmsAfterCapture";
            resources.ApplyResources(cmsTask, "cmsTask");
            // 
            // tcTaskSettings
            // 
            tcTaskSettings.Controls.Add(tpTask);
            tcTaskSettings.Controls.Add(tpGeneral);
            tcTaskSettings.Controls.Add(tpImage);
            tcTaskSettings.Controls.Add(tpCapture);
            tcTaskSettings.Controls.Add(tpUpload);
            tcTaskSettings.Controls.Add(tpActions);
            tcTaskSettings.Controls.Add(tpWatchFolders);
            tcTaskSettings.Controls.Add(tpTools);
            tcTaskSettings.Controls.Add(tpAdvanced);
            resources.ApplyResources(tcTaskSettings, "tcTaskSettings");
            tcTaskSettings.Name = "tcTaskSettings";
            tcTaskSettings.SelectedIndex = 0;
            // 
            // tpTask
            // 
            tpTask.BackColor = System.Drawing.SystemColors.Window;
            tpTask.Controls.Add(lblTask);
            tpTask.Controls.Add(btnScreenshotsFolderBrowse);
            tpTask.Controls.Add(txtScreenshotsFolder);
            tpTask.Controls.Add(cbOverrideScreenshotsFolder);
            tpTask.Controls.Add(cbCustomUploaders);
            tpTask.Controls.Add(cbOverrideCustomUploader);
            tpTask.Controls.Add(cbOverrideFTPAccount);
            tpTask.Controls.Add(cbFTPAccounts);
            tpTask.Controls.Add(tbDescription);
            tpTask.Controls.Add(btnAfterCapture);
            tpTask.Controls.Add(btnAfterUpload);
            tpTask.Controls.Add(btnDestinations);
            tpTask.Controls.Add(cbOverrideAfterCaptureSettings);
            tpTask.Controls.Add(btnTask);
            tpTask.Controls.Add(cbOverrideAfterUploadSettings);
            tpTask.Controls.Add(cbOverrideDestinationSettings);
            tpTask.Controls.Add(lblDescription);
            resources.ApplyResources(tpTask, "tpTask");
            tpTask.Name = "tpTask";
            // 
            // lblTask
            // 
            resources.ApplyResources(lblTask, "lblTask");
            lblTask.Name = "lblTask";
            // 
            // btnScreenshotsFolderBrowse
            // 
            resources.ApplyResources(btnScreenshotsFolderBrowse, "btnScreenshotsFolderBrowse");
            btnScreenshotsFolderBrowse.Name = "btnScreenshotsFolderBrowse";
            btnScreenshotsFolderBrowse.UseVisualStyleBackColor = true;
            btnScreenshotsFolderBrowse.Click += btnScreenshotsFolderBrowse_Click;
            // 
            // txtScreenshotsFolder
            // 
            resources.ApplyResources(txtScreenshotsFolder, "txtScreenshotsFolder");
            txtScreenshotsFolder.Name = "txtScreenshotsFolder";
            txtScreenshotsFolder.TextChanged += txtScreenshotsFolder_TextChanged;
            // 
            // cbOverrideScreenshotsFolder
            // 
            resources.ApplyResources(cbOverrideScreenshotsFolder, "cbOverrideScreenshotsFolder");
            cbOverrideScreenshotsFolder.Name = "cbOverrideScreenshotsFolder";
            cbOverrideScreenshotsFolder.UseVisualStyleBackColor = true;
            cbOverrideScreenshotsFolder.CheckedChanged += cbOverrideScreenshotsFolder_CheckedChanged;
            // 
            // cbCustomUploaders
            // 
            cbCustomUploaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbCustomUploaders.FormattingEnabled = true;
            resources.ApplyResources(cbCustomUploaders, "cbCustomUploaders");
            cbCustomUploaders.Name = "cbCustomUploaders";
            cbCustomUploaders.SelectedIndexChanged += cbCustomUploaders_SelectedIndexChanged;
            // 
            // cbOverrideCustomUploader
            // 
            resources.ApplyResources(cbOverrideCustomUploader, "cbOverrideCustomUploader");
            cbOverrideCustomUploader.Name = "cbOverrideCustomUploader";
            cbOverrideCustomUploader.UseVisualStyleBackColor = true;
            cbOverrideCustomUploader.CheckedChanged += cbOverrideCustomUploader_CheckedChanged;
            // 
            // cbOverrideFTPAccount
            // 
            resources.ApplyResources(cbOverrideFTPAccount, "cbOverrideFTPAccount");
            cbOverrideFTPAccount.Name = "cbOverrideFTPAccount";
            cbOverrideFTPAccount.UseVisualStyleBackColor = true;
            cbOverrideFTPAccount.CheckedChanged += cbOverrideFTPAccount_CheckedChanged;
            // 
            // cbFTPAccounts
            // 
            cbFTPAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFTPAccounts.FormattingEnabled = true;
            resources.ApplyResources(cbFTPAccounts, "cbFTPAccounts");
            cbFTPAccounts.Name = "cbFTPAccounts";
            cbFTPAccounts.SelectedIndexChanged += cbFTPAccounts_SelectedIndexChanged;
            // 
            // btnAfterCapture
            // 
            resources.ApplyResources(btnAfterCapture, "btnAfterCapture");
            btnAfterCapture.Menu = cmsAfterCapture;
            btnAfterCapture.Name = "btnAfterCapture";
            btnAfterCapture.UseMnemonic = false;
            btnAfterCapture.UseVisualStyleBackColor = true;
            // 
            // btnAfterUpload
            // 
            resources.ApplyResources(btnAfterUpload, "btnAfterUpload");
            btnAfterUpload.Menu = cmsAfterUpload;
            btnAfterUpload.Name = "btnAfterUpload";
            btnAfterUpload.UseMnemonic = false;
            btnAfterUpload.UseVisualStyleBackColor = true;
            // 
            // btnDestinations
            // 
            resources.ApplyResources(btnDestinations, "btnDestinations");
            btnDestinations.Menu = cmsDestinations;
            btnDestinations.Name = "btnDestinations";
            btnDestinations.UseMnemonic = false;
            btnDestinations.UseVisualStyleBackColor = true;
            // 
            // cmsDestinations
            // 
            cmsDestinations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiImageUploaders, tsmiTextUploaders, tsmiFileUploaders, tsmiURLShorteners, tsmiURLSharingServices });
            cmsDestinations.Name = "cmsDestinations";
            resources.ApplyResources(cmsDestinations, "cmsDestinations");
            // 
            // tsmiImageUploaders
            // 
            tsmiImageUploaders.Image = Properties.Resources.image;
            tsmiImageUploaders.Name = "tsmiImageUploaders";
            resources.ApplyResources(tsmiImageUploaders, "tsmiImageUploaders");
            // 
            // tsmiTextUploaders
            // 
            tsmiTextUploaders.Image = Properties.Resources.notebook;
            tsmiTextUploaders.Name = "tsmiTextUploaders";
            resources.ApplyResources(tsmiTextUploaders, "tsmiTextUploaders");
            // 
            // tsmiFileUploaders
            // 
            tsmiFileUploaders.Image = Properties.Resources.application_block;
            tsmiFileUploaders.Name = "tsmiFileUploaders";
            resources.ApplyResources(tsmiFileUploaders, "tsmiFileUploaders");
            // 
            // tsmiURLShorteners
            // 
            tsmiURLShorteners.Image = Properties.Resources.edit_scale;
            tsmiURLShorteners.Name = "tsmiURLShorteners";
            resources.ApplyResources(tsmiURLShorteners, "tsmiURLShorteners");
            // 
            // tsmiURLSharingServices
            // 
            tsmiURLSharingServices.Image = Properties.Resources.globe_share;
            tsmiURLSharingServices.Name = "tsmiURLSharingServices";
            resources.ApplyResources(tsmiURLSharingServices, "tsmiURLSharingServices");
            // 
            // btnTask
            // 
            btnTask.Image = Properties.Resources.gear;
            resources.ApplyResources(btnTask, "btnTask");
            btnTask.Menu = cmsTask;
            btnTask.Name = "btnTask";
            btnTask.UseMnemonic = false;
            btnTask.UseVisualStyleBackColor = true;
            // 
            // tpGeneral
            // 
            tpGeneral.BackColor = System.Drawing.SystemColors.Window;
            tpGeneral.Controls.Add(tcGeneral);
            resources.ApplyResources(tpGeneral, "tpGeneral");
            tpGeneral.Name = "tpGeneral";
            // 
            // tcGeneral
            // 
            tcGeneral.Controls.Add(tpGeneralMain);
            tcGeneral.Controls.Add(tpNotifications);
            resources.ApplyResources(tcGeneral, "tcGeneral");
            tcGeneral.Name = "tcGeneral";
            tcGeneral.SelectedIndex = 0;
            // 
            // tpGeneralMain
            // 
            tpGeneralMain.Controls.Add(cbOverrideGeneralSettings);
            resources.ApplyResources(tpGeneralMain, "tpGeneralMain");
            tpGeneralMain.Name = "tpGeneralMain";
            tpGeneralMain.UseVisualStyleBackColor = true;
            // 
            // cbOverrideGeneralSettings
            // 
            resources.ApplyResources(cbOverrideGeneralSettings, "cbOverrideGeneralSettings");
            cbOverrideGeneralSettings.Checked = true;
            cbOverrideGeneralSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideGeneralSettings.Name = "cbOverrideGeneralSettings";
            cbOverrideGeneralSettings.UseVisualStyleBackColor = true;
            cbOverrideGeneralSettings.CheckedChanged += cbUseDefaultGeneralSettings_CheckedChanged;
            // 
            // tpNotifications
            // 
            tpNotifications.Controls.Add(btnCustomActionCompletedSoundPath);
            tpNotifications.Controls.Add(txtCustomActionCompletedSoundPath);
            tpNotifications.Controls.Add(cbUseCustomActionCompletedSound);
            tpNotifications.Controls.Add(cbPlaySoundAfterAction);
            tpNotifications.Controls.Add(cbShowToastNotificationAfterTaskCompleted);
            tpNotifications.Controls.Add(btnCustomErrorSoundPath);
            tpNotifications.Controls.Add(btnCustomTaskCompletedSoundPath);
            tpNotifications.Controls.Add(btnCustomCaptureSoundPath);
            tpNotifications.Controls.Add(txtCustomErrorSoundPath);
            tpNotifications.Controls.Add(txtCustomTaskCompletedSoundPath);
            tpNotifications.Controls.Add(txtCustomCaptureSoundPath);
            tpNotifications.Controls.Add(cbUseCustomErrorSound);
            tpNotifications.Controls.Add(cbUseCustomTaskCompletedSound);
            tpNotifications.Controls.Add(cbUseCustomCaptureSound);
            tpNotifications.Controls.Add(gbToastWindow);
            tpNotifications.Controls.Add(cbPlaySoundAfterCapture);
            tpNotifications.Controls.Add(cbPlaySoundAfterUpload);
            resources.ApplyResources(tpNotifications, "tpNotifications");
            tpNotifications.Name = "tpNotifications";
            tpNotifications.UseVisualStyleBackColor = true;
            // 
            // btnCustomActionCompletedSoundPath
            // 
            resources.ApplyResources(btnCustomActionCompletedSoundPath, "btnCustomActionCompletedSoundPath");
            btnCustomActionCompletedSoundPath.Name = "btnCustomActionCompletedSoundPath";
            btnCustomActionCompletedSoundPath.UseVisualStyleBackColor = true;
            btnCustomActionCompletedSoundPath.Click += btnCustomActionCompletedSoundPath_Click;
            // 
            // txtCustomActionCompletedSoundPath
            // 
            resources.ApplyResources(txtCustomActionCompletedSoundPath, "txtCustomActionCompletedSoundPath");
            txtCustomActionCompletedSoundPath.Name = "txtCustomActionCompletedSoundPath";
            txtCustomActionCompletedSoundPath.TextChanged += txtCustomActionCompletedSoundPath_TextChanged;
            // 
            // cbUseCustomActionCompletedSound
            // 
            resources.ApplyResources(cbUseCustomActionCompletedSound, "cbUseCustomActionCompletedSound");
            cbUseCustomActionCompletedSound.Name = "cbUseCustomActionCompletedSound";
            cbUseCustomActionCompletedSound.UseVisualStyleBackColor = true;
            cbUseCustomActionCompletedSound.CheckedChanged += cbUseCustomActionCompletedSound_CheckedChanged;
            // 
            // cbPlaySoundAfterAction
            // 
            resources.ApplyResources(cbPlaySoundAfterAction, "cbPlaySoundAfterAction");
            cbPlaySoundAfterAction.Name = "cbPlaySoundAfterAction";
            cbPlaySoundAfterAction.UseVisualStyleBackColor = true;
            cbPlaySoundAfterAction.CheckedChanged += cbPlaySoundAfterAction_CheckedChanged;
            // 
            // cbShowToastNotificationAfterTaskCompleted
            // 
            resources.ApplyResources(cbShowToastNotificationAfterTaskCompleted, "cbShowToastNotificationAfterTaskCompleted");
            cbShowToastNotificationAfterTaskCompleted.Name = "cbShowToastNotificationAfterTaskCompleted";
            cbShowToastNotificationAfterTaskCompleted.UseVisualStyleBackColor = true;
            cbShowToastNotificationAfterTaskCompleted.CheckedChanged += cbShowToastNotificationAfterTaskCompleted_CheckedChanged;
            // 
            // btnCustomErrorSoundPath
            // 
            resources.ApplyResources(btnCustomErrorSoundPath, "btnCustomErrorSoundPath");
            btnCustomErrorSoundPath.Name = "btnCustomErrorSoundPath";
            btnCustomErrorSoundPath.UseVisualStyleBackColor = true;
            btnCustomErrorSoundPath.Click += btnCustomErrorSoundPath_Click;
            // 
            // btnCustomTaskCompletedSoundPath
            // 
            resources.ApplyResources(btnCustomTaskCompletedSoundPath, "btnCustomTaskCompletedSoundPath");
            btnCustomTaskCompletedSoundPath.Name = "btnCustomTaskCompletedSoundPath";
            btnCustomTaskCompletedSoundPath.UseVisualStyleBackColor = true;
            btnCustomTaskCompletedSoundPath.Click += btnCustomTaskCompletedSoundPath_Click;
            // 
            // btnCustomCaptureSoundPath
            // 
            resources.ApplyResources(btnCustomCaptureSoundPath, "btnCustomCaptureSoundPath");
            btnCustomCaptureSoundPath.Name = "btnCustomCaptureSoundPath";
            btnCustomCaptureSoundPath.UseVisualStyleBackColor = true;
            btnCustomCaptureSoundPath.Click += btnCustomCaptureSoundPath_Click;
            // 
            // txtCustomErrorSoundPath
            // 
            resources.ApplyResources(txtCustomErrorSoundPath, "txtCustomErrorSoundPath");
            txtCustomErrorSoundPath.Name = "txtCustomErrorSoundPath";
            txtCustomErrorSoundPath.TextChanged += txtCustomErrorSoundPath_TextChanged;
            // 
            // txtCustomTaskCompletedSoundPath
            // 
            resources.ApplyResources(txtCustomTaskCompletedSoundPath, "txtCustomTaskCompletedSoundPath");
            txtCustomTaskCompletedSoundPath.Name = "txtCustomTaskCompletedSoundPath";
            txtCustomTaskCompletedSoundPath.TextChanged += txtCustomTaskCompletedSoundPath_TextChanged;
            // 
            // txtCustomCaptureSoundPath
            // 
            resources.ApplyResources(txtCustomCaptureSoundPath, "txtCustomCaptureSoundPath");
            txtCustomCaptureSoundPath.Name = "txtCustomCaptureSoundPath";
            txtCustomCaptureSoundPath.TextChanged += txtCustomCaptureSoundPath_TextChanged;
            // 
            // cbUseCustomErrorSound
            // 
            resources.ApplyResources(cbUseCustomErrorSound, "cbUseCustomErrorSound");
            cbUseCustomErrorSound.Name = "cbUseCustomErrorSound";
            cbUseCustomErrorSound.UseVisualStyleBackColor = true;
            cbUseCustomErrorSound.CheckedChanged += cbUseCustomErrorSound_CheckedChanged;
            // 
            // cbUseCustomTaskCompletedSound
            // 
            resources.ApplyResources(cbUseCustomTaskCompletedSound, "cbUseCustomTaskCompletedSound");
            cbUseCustomTaskCompletedSound.Name = "cbUseCustomTaskCompletedSound";
            cbUseCustomTaskCompletedSound.UseVisualStyleBackColor = true;
            cbUseCustomTaskCompletedSound.CheckedChanged += cbUseCustomTaskCompletedSound_CheckedChanged;
            // 
            // cbUseCustomCaptureSound
            // 
            resources.ApplyResources(cbUseCustomCaptureSound, "cbUseCustomCaptureSound");
            cbUseCustomCaptureSound.Name = "cbUseCustomCaptureSound";
            cbUseCustomCaptureSound.UseVisualStyleBackColor = true;
            cbUseCustomCaptureSound.CheckedChanged += cbUseCustomCaptureSound_CheckedChanged;
            // 
            // gbToastWindow
            // 
            gbToastWindow.Controls.Add(cbToastWindowAutoHide);
            gbToastWindow.Controls.Add(lblToastWindowFadeDurationSeconds);
            gbToastWindow.Controls.Add(lblToastWindowDurationSeconds);
            gbToastWindow.Controls.Add(lblToastWindowSizeX);
            gbToastWindow.Controls.Add(cbToastWindowMiddleClickAction);
            gbToastWindow.Controls.Add(cbToastWindowRightClickAction);
            gbToastWindow.Controls.Add(cbToastWindowLeftClickAction);
            gbToastWindow.Controls.Add(nudToastWindowSizeHeight);
            gbToastWindow.Controls.Add(nudToastWindowSizeWidth);
            gbToastWindow.Controls.Add(cbToastWindowPlacement);
            gbToastWindow.Controls.Add(nudToastWindowFadeDuration);
            gbToastWindow.Controls.Add(cbDisableNotificationsOnFullscreen);
            gbToastWindow.Controls.Add(nudToastWindowDuration);
            gbToastWindow.Controls.Add(lblToastWindowMiddleClickAction);
            gbToastWindow.Controls.Add(lblToastWindowRightClickAction);
            gbToastWindow.Controls.Add(lblToastWindowLeftClickAction);
            gbToastWindow.Controls.Add(lblToastWindowSize);
            gbToastWindow.Controls.Add(lblToastWindowPlacement);
            gbToastWindow.Controls.Add(lblToastWindowFadeDuration);
            gbToastWindow.Controls.Add(lblToastWindowDuration);
            resources.ApplyResources(gbToastWindow, "gbToastWindow");
            gbToastWindow.Name = "gbToastWindow";
            gbToastWindow.TabStop = false;
            // 
            // cbToastWindowAutoHide
            // 
            resources.ApplyResources(cbToastWindowAutoHide, "cbToastWindowAutoHide");
            cbToastWindowAutoHide.Name = "cbToastWindowAutoHide";
            cbToastWindowAutoHide.UseVisualStyleBackColor = true;
            cbToastWindowAutoHide.CheckedChanged += cbToastWindowAutoHide_CheckedChanged;
            // 
            // lblToastWindowFadeDurationSeconds
            // 
            resources.ApplyResources(lblToastWindowFadeDurationSeconds, "lblToastWindowFadeDurationSeconds");
            lblToastWindowFadeDurationSeconds.Name = "lblToastWindowFadeDurationSeconds";
            // 
            // lblToastWindowDurationSeconds
            // 
            resources.ApplyResources(lblToastWindowDurationSeconds, "lblToastWindowDurationSeconds");
            lblToastWindowDurationSeconds.Name = "lblToastWindowDurationSeconds";
            // 
            // lblToastWindowSizeX
            // 
            resources.ApplyResources(lblToastWindowSizeX, "lblToastWindowSizeX");
            lblToastWindowSizeX.Name = "lblToastWindowSizeX";
            // 
            // cbToastWindowMiddleClickAction
            // 
            cbToastWindowMiddleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbToastWindowMiddleClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbToastWindowMiddleClickAction, "cbToastWindowMiddleClickAction");
            cbToastWindowMiddleClickAction.Name = "cbToastWindowMiddleClickAction";
            cbToastWindowMiddleClickAction.SelectedIndexChanged += cbToastWindowMiddleClickAction_SelectedIndexChanged;
            // 
            // cbToastWindowRightClickAction
            // 
            cbToastWindowRightClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbToastWindowRightClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbToastWindowRightClickAction, "cbToastWindowRightClickAction");
            cbToastWindowRightClickAction.Name = "cbToastWindowRightClickAction";
            cbToastWindowRightClickAction.SelectedIndexChanged += cbToastWindowRightClickAction_SelectedIndexChanged;
            // 
            // cbToastWindowLeftClickAction
            // 
            cbToastWindowLeftClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbToastWindowLeftClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbToastWindowLeftClickAction, "cbToastWindowLeftClickAction");
            cbToastWindowLeftClickAction.Name = "cbToastWindowLeftClickAction";
            cbToastWindowLeftClickAction.SelectedIndexChanged += cbToastWindowLeftClickAction_SelectedIndexChanged;
            // 
            // nudToastWindowSizeHeight
            // 
            resources.ApplyResources(nudToastWindowSizeHeight, "nudToastWindowSizeHeight");
            nudToastWindowSizeHeight.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudToastWindowSizeHeight.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudToastWindowSizeHeight.Name = "nudToastWindowSizeHeight";
            nudToastWindowSizeHeight.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudToastWindowSizeHeight.ValueChanged += nudToastWindowSizeHeight_ValueChanged;
            // 
            // nudToastWindowSizeWidth
            // 
            resources.ApplyResources(nudToastWindowSizeWidth, "nudToastWindowSizeWidth");
            nudToastWindowSizeWidth.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudToastWindowSizeWidth.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudToastWindowSizeWidth.Name = "nudToastWindowSizeWidth";
            nudToastWindowSizeWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudToastWindowSizeWidth.ValueChanged += nudToastWindowSizeWidth_ValueChanged;
            // 
            // cbToastWindowPlacement
            // 
            cbToastWindowPlacement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbToastWindowPlacement.FormattingEnabled = true;
            resources.ApplyResources(cbToastWindowPlacement, "cbToastWindowPlacement");
            cbToastWindowPlacement.Name = "cbToastWindowPlacement";
            cbToastWindowPlacement.SelectedIndexChanged += cbToastWindowPlacement_SelectedIndexChanged;
            // 
            // nudToastWindowFadeDuration
            // 
            nudToastWindowFadeDuration.DecimalPlaces = 1;
            resources.ApplyResources(nudToastWindowFadeDuration, "nudToastWindowFadeDuration");
            nudToastWindowFadeDuration.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            nudToastWindowFadeDuration.Name = "nudToastWindowFadeDuration";
            nudToastWindowFadeDuration.ValueChanged += nudToastWindowFadeDuration_ValueChanged;
            // 
            // cbDisableNotificationsOnFullscreen
            // 
            resources.ApplyResources(cbDisableNotificationsOnFullscreen, "cbDisableNotificationsOnFullscreen");
            cbDisableNotificationsOnFullscreen.Name = "cbDisableNotificationsOnFullscreen";
            cbDisableNotificationsOnFullscreen.UseVisualStyleBackColor = true;
            cbDisableNotificationsOnFullscreen.CheckedChanged += cbDisableNotificationsOnFullscreen_CheckedChanged;
            // 
            // nudToastWindowDuration
            // 
            nudToastWindowDuration.DecimalPlaces = 1;
            resources.ApplyResources(nudToastWindowDuration, "nudToastWindowDuration");
            nudToastWindowDuration.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            nudToastWindowDuration.Name = "nudToastWindowDuration";
            nudToastWindowDuration.ValueChanged += nudToastWindowDuration_ValueChanged;
            // 
            // lblToastWindowMiddleClickAction
            // 
            resources.ApplyResources(lblToastWindowMiddleClickAction, "lblToastWindowMiddleClickAction");
            lblToastWindowMiddleClickAction.Name = "lblToastWindowMiddleClickAction";
            // 
            // lblToastWindowRightClickAction
            // 
            resources.ApplyResources(lblToastWindowRightClickAction, "lblToastWindowRightClickAction");
            lblToastWindowRightClickAction.Name = "lblToastWindowRightClickAction";
            // 
            // lblToastWindowLeftClickAction
            // 
            resources.ApplyResources(lblToastWindowLeftClickAction, "lblToastWindowLeftClickAction");
            lblToastWindowLeftClickAction.Name = "lblToastWindowLeftClickAction";
            // 
            // lblToastWindowSize
            // 
            resources.ApplyResources(lblToastWindowSize, "lblToastWindowSize");
            lblToastWindowSize.Name = "lblToastWindowSize";
            // 
            // lblToastWindowPlacement
            // 
            resources.ApplyResources(lblToastWindowPlacement, "lblToastWindowPlacement");
            lblToastWindowPlacement.Name = "lblToastWindowPlacement";
            // 
            // lblToastWindowFadeDuration
            // 
            resources.ApplyResources(lblToastWindowFadeDuration, "lblToastWindowFadeDuration");
            lblToastWindowFadeDuration.Name = "lblToastWindowFadeDuration";
            // 
            // lblToastWindowDuration
            // 
            resources.ApplyResources(lblToastWindowDuration, "lblToastWindowDuration");
            lblToastWindowDuration.Name = "lblToastWindowDuration";
            // 
            // cbPlaySoundAfterCapture
            // 
            resources.ApplyResources(cbPlaySoundAfterCapture, "cbPlaySoundAfterCapture");
            cbPlaySoundAfterCapture.Name = "cbPlaySoundAfterCapture";
            cbPlaySoundAfterCapture.UseVisualStyleBackColor = true;
            cbPlaySoundAfterCapture.CheckedChanged += cbPlaySoundAfterCapture_CheckedChanged;
            // 
            // cbPlaySoundAfterUpload
            // 
            resources.ApplyResources(cbPlaySoundAfterUpload, "cbPlaySoundAfterUpload");
            cbPlaySoundAfterUpload.Name = "cbPlaySoundAfterUpload";
            cbPlaySoundAfterUpload.UseVisualStyleBackColor = true;
            cbPlaySoundAfterUpload.CheckedChanged += cbPlaySoundAfterUpload_CheckedChanged;
            // 
            // tpImage
            // 
            tpImage.BackColor = System.Drawing.SystemColors.Window;
            tpImage.Controls.Add(tcImage);
            resources.ApplyResources(tpImage, "tpImage");
            tpImage.Name = "tpImage";
            // 
            // tcImage
            // 
            tcImage.Controls.Add(tpQuality);
            tcImage.Controls.Add(tpEffects);
            tcImage.Controls.Add(tpThumbnail);
            resources.ApplyResources(tcImage, "tcImage");
            tcImage.Name = "tcImage";
            tcImage.SelectedIndex = 0;
            // 
            // tpQuality
            // 
            tpQuality.BackColor = System.Drawing.SystemColors.Window;
            tpQuality.Controls.Add(pImage);
            tpQuality.Controls.Add(cbOverrideImageSettings);
            resources.ApplyResources(tpQuality, "tpQuality");
            tpQuality.Name = "tpQuality";
            // 
            // pImage
            // 
            pImage.Controls.Add(cbImageAutoJPEGQuality);
            pImage.Controls.Add(cbImagePNGBitDepth);
            pImage.Controls.Add(lblImagePNGBitDepth);
            pImage.Controls.Add(cbImageAutoUseJPEG);
            pImage.Controls.Add(lblImageFormat);
            pImage.Controls.Add(cbImageFileExist);
            pImage.Controls.Add(lblImageFileExist);
            pImage.Controls.Add(nudImageAutoUseJPEGSize);
            pImage.Controls.Add(lblImageSizeLimitHint);
            pImage.Controls.Add(nudImageJPEGQuality);
            pImage.Controls.Add(cbImageFormat);
            pImage.Controls.Add(lblImageJPEGQualityHint);
            pImage.Controls.Add(lblImageGIFQuality);
            pImage.Controls.Add(lblImageJPEGQuality);
            pImage.Controls.Add(cbImageGIFQuality);
            resources.ApplyResources(pImage, "pImage");
            pImage.Name = "pImage";
            // 
            // cbImageAutoJPEGQuality
            // 
            resources.ApplyResources(cbImageAutoJPEGQuality, "cbImageAutoJPEGQuality");
            cbImageAutoJPEGQuality.Name = "cbImageAutoJPEGQuality";
            cbImageAutoJPEGQuality.UseVisualStyleBackColor = true;
            cbImageAutoJPEGQuality.CheckedChanged += cbImageAutoJPEGQuality_CheckedChanged;
            // 
            // cbImagePNGBitDepth
            // 
            cbImagePNGBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbImagePNGBitDepth.FormattingEnabled = true;
            resources.ApplyResources(cbImagePNGBitDepth, "cbImagePNGBitDepth");
            cbImagePNGBitDepth.Name = "cbImagePNGBitDepth";
            cbImagePNGBitDepth.SelectedIndexChanged += cbImagePNGBitDepth_SelectedIndexChanged;
            // 
            // lblImagePNGBitDepth
            // 
            resources.ApplyResources(lblImagePNGBitDepth, "lblImagePNGBitDepth");
            lblImagePNGBitDepth.Name = "lblImagePNGBitDepth";
            // 
            // cbImageAutoUseJPEG
            // 
            resources.ApplyResources(cbImageAutoUseJPEG, "cbImageAutoUseJPEG");
            cbImageAutoUseJPEG.Name = "cbImageAutoUseJPEG";
            cbImageAutoUseJPEG.UseVisualStyleBackColor = true;
            cbImageAutoUseJPEG.CheckedChanged += cbImageAutoUseJPEG_CheckedChanged;
            // 
            // lblImageFormat
            // 
            resources.ApplyResources(lblImageFormat, "lblImageFormat");
            lblImageFormat.Name = "lblImageFormat";
            // 
            // cbImageFileExist
            // 
            cbImageFileExist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbImageFileExist.FormattingEnabled = true;
            resources.ApplyResources(cbImageFileExist, "cbImageFileExist");
            cbImageFileExist.Name = "cbImageFileExist";
            cbImageFileExist.SelectedIndexChanged += cbImageFileExist_SelectedIndexChanged;
            // 
            // lblImageFileExist
            // 
            resources.ApplyResources(lblImageFileExist, "lblImageFileExist");
            lblImageFileExist.Name = "lblImageFileExist";
            // 
            // nudImageAutoUseJPEGSize
            // 
            resources.ApplyResources(nudImageAutoUseJPEGSize, "nudImageAutoUseJPEGSize");
            nudImageAutoUseJPEGSize.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudImageAutoUseJPEGSize.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudImageAutoUseJPEGSize.Name = "nudImageAutoUseJPEGSize";
            nudImageAutoUseJPEGSize.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            nudImageAutoUseJPEGSize.ValueChanged += nudImageAutoUseJPEGSize_ValueChanged;
            // 
            // lblImageSizeLimitHint
            // 
            resources.ApplyResources(lblImageSizeLimitHint, "lblImageSizeLimitHint");
            lblImageSizeLimitHint.Name = "lblImageSizeLimitHint";
            // 
            // nudImageJPEGQuality
            // 
            resources.ApplyResources(nudImageJPEGQuality, "nudImageJPEGQuality");
            nudImageJPEGQuality.Name = "nudImageJPEGQuality";
            nudImageJPEGQuality.Value = new decimal(new int[] { 90, 0, 0, 0 });
            nudImageJPEGQuality.ValueChanged += nudImageJPEGQuality_ValueChanged;
            // 
            // cbImageFormat
            // 
            cbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbImageFormat.FormattingEnabled = true;
            resources.ApplyResources(cbImageFormat, "cbImageFormat");
            cbImageFormat.Name = "cbImageFormat";
            cbImageFormat.SelectedIndexChanged += cbImageFormat_SelectedIndexChanged;
            // 
            // lblImageJPEGQualityHint
            // 
            resources.ApplyResources(lblImageJPEGQualityHint, "lblImageJPEGQualityHint");
            lblImageJPEGQualityHint.Name = "lblImageJPEGQualityHint";
            // 
            // lblImageGIFQuality
            // 
            resources.ApplyResources(lblImageGIFQuality, "lblImageGIFQuality");
            lblImageGIFQuality.Name = "lblImageGIFQuality";
            // 
            // lblImageJPEGQuality
            // 
            resources.ApplyResources(lblImageJPEGQuality, "lblImageJPEGQuality");
            lblImageJPEGQuality.Name = "lblImageJPEGQuality";
            // 
            // cbImageGIFQuality
            // 
            cbImageGIFQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbImageGIFQuality.FormattingEnabled = true;
            resources.ApplyResources(cbImageGIFQuality, "cbImageGIFQuality");
            cbImageGIFQuality.Name = "cbImageGIFQuality";
            cbImageGIFQuality.SelectedIndexChanged += cbImageGIFQuality_SelectedIndexChanged;
            // 
            // cbOverrideImageSettings
            // 
            resources.ApplyResources(cbOverrideImageSettings, "cbOverrideImageSettings");
            cbOverrideImageSettings.Checked = true;
            cbOverrideImageSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideImageSettings.Name = "cbOverrideImageSettings";
            cbOverrideImageSettings.UseVisualStyleBackColor = true;
            cbOverrideImageSettings.CheckedChanged += cbUseDefaultImageSettings_CheckedChanged;
            // 
            // tpEffects
            // 
            tpEffects.BackColor = System.Drawing.SystemColors.Window;
            tpEffects.Controls.Add(cbUseRandomImageEffect);
            tpEffects.Controls.Add(lblImageEffectsNote);
            tpEffects.Controls.Add(cbShowImageEffectsWindowAfterCapture);
            tpEffects.Controls.Add(cbImageEffectOnlyRegionCapture);
            tpEffects.Controls.Add(btnImageEffects);
            resources.ApplyResources(tpEffects, "tpEffects");
            tpEffects.Name = "tpEffects";
            // 
            // cbUseRandomImageEffect
            // 
            resources.ApplyResources(cbUseRandomImageEffect, "cbUseRandomImageEffect");
            cbUseRandomImageEffect.Name = "cbUseRandomImageEffect";
            cbUseRandomImageEffect.UseVisualStyleBackColor = true;
            cbUseRandomImageEffect.CheckedChanged += cbUseRandomImageEffect_CheckedChanged;
            // 
            // lblImageEffectsNote
            // 
            resources.ApplyResources(lblImageEffectsNote, "lblImageEffectsNote");
            lblImageEffectsNote.Name = "lblImageEffectsNote";
            // 
            // cbShowImageEffectsWindowAfterCapture
            // 
            resources.ApplyResources(cbShowImageEffectsWindowAfterCapture, "cbShowImageEffectsWindowAfterCapture");
            cbShowImageEffectsWindowAfterCapture.Name = "cbShowImageEffectsWindowAfterCapture";
            cbShowImageEffectsWindowAfterCapture.UseVisualStyleBackColor = true;
            cbShowImageEffectsWindowAfterCapture.CheckedChanged += cbShowImageEffectsWindowAfterCapture_CheckedChanged;
            // 
            // cbImageEffectOnlyRegionCapture
            // 
            resources.ApplyResources(cbImageEffectOnlyRegionCapture, "cbImageEffectOnlyRegionCapture");
            cbImageEffectOnlyRegionCapture.Name = "cbImageEffectOnlyRegionCapture";
            cbImageEffectOnlyRegionCapture.UseVisualStyleBackColor = true;
            cbImageEffectOnlyRegionCapture.CheckedChanged += cbImageEffectOnlyRegionCapture_CheckedChanged;
            // 
            // btnImageEffects
            // 
            resources.ApplyResources(btnImageEffects, "btnImageEffects");
            btnImageEffects.Name = "btnImageEffects";
            btnImageEffects.UseVisualStyleBackColor = true;
            btnImageEffects.Click += btnImageEffects_Click;
            // 
            // tpThumbnail
            // 
            tpThumbnail.BackColor = System.Drawing.SystemColors.Window;
            tpThumbnail.Controls.Add(cbThumbnailIfSmaller);
            tpThumbnail.Controls.Add(lblThumbnailNamePreview);
            tpThumbnail.Controls.Add(lblThumbnailName);
            tpThumbnail.Controls.Add(txtThumbnailName);
            tpThumbnail.Controls.Add(lblThumbnailHeight);
            tpThumbnail.Controls.Add(lblThumbnailWidth);
            tpThumbnail.Controls.Add(nudThumbnailHeight);
            tpThumbnail.Controls.Add(nudThumbnailWidth);
            resources.ApplyResources(tpThumbnail, "tpThumbnail");
            tpThumbnail.Name = "tpThumbnail";
            // 
            // cbThumbnailIfSmaller
            // 
            resources.ApplyResources(cbThumbnailIfSmaller, "cbThumbnailIfSmaller");
            cbThumbnailIfSmaller.Name = "cbThumbnailIfSmaller";
            cbThumbnailIfSmaller.UseVisualStyleBackColor = true;
            cbThumbnailIfSmaller.CheckedChanged += cbThumbnailIfSmaller_CheckedChanged;
            // 
            // lblThumbnailNamePreview
            // 
            resources.ApplyResources(lblThumbnailNamePreview, "lblThumbnailNamePreview");
            lblThumbnailNamePreview.Name = "lblThumbnailNamePreview";
            // 
            // lblThumbnailName
            // 
            resources.ApplyResources(lblThumbnailName, "lblThumbnailName");
            lblThumbnailName.Name = "lblThumbnailName";
            // 
            // txtThumbnailName
            // 
            resources.ApplyResources(txtThumbnailName, "txtThumbnailName");
            txtThumbnailName.Name = "txtThumbnailName";
            txtThumbnailName.TextChanged += txtThumbnailName_TextChanged;
            // 
            // lblThumbnailHeight
            // 
            resources.ApplyResources(lblThumbnailHeight, "lblThumbnailHeight");
            lblThumbnailHeight.Name = "lblThumbnailHeight";
            // 
            // lblThumbnailWidth
            // 
            resources.ApplyResources(lblThumbnailWidth, "lblThumbnailWidth");
            lblThumbnailWidth.Name = "lblThumbnailWidth";
            // 
            // nudThumbnailHeight
            // 
            resources.ApplyResources(nudThumbnailHeight, "nudThumbnailHeight");
            nudThumbnailHeight.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            nudThumbnailHeight.Name = "nudThumbnailHeight";
            nudThumbnailHeight.ValueChanged += nudThumbnailHeight_ValueChanged;
            // 
            // nudThumbnailWidth
            // 
            resources.ApplyResources(nudThumbnailWidth, "nudThumbnailWidth");
            nudThumbnailWidth.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            nudThumbnailWidth.Name = "nudThumbnailWidth";
            nudThumbnailWidth.ValueChanged += nudThumbnailWidth_ValueChanged;
            // 
            // tpCapture
            // 
            tpCapture.BackColor = System.Drawing.SystemColors.Window;
            tpCapture.Controls.Add(tcCapture);
            resources.ApplyResources(tpCapture, "tpCapture");
            tpCapture.Name = "tpCapture";
            // 
            // tcCapture
            // 
            tcCapture.Controls.Add(tpCaptureGeneral);
            tcCapture.Controls.Add(tpRegionCapture);
            tcCapture.Controls.Add(tpScreenRecorder);
            tcCapture.Controls.Add(tpOCR);
            resources.ApplyResources(tcCapture, "tcCapture");
            tcCapture.Name = "tcCapture";
            tcCapture.SelectedIndex = 0;
            // 
            // tpCaptureGeneral
            // 
            tpCaptureGeneral.BackColor = System.Drawing.SystemColors.Window;
            tpCaptureGeneral.Controls.Add(pCapture);
            tpCaptureGeneral.Controls.Add(cbOverrideCaptureSettings);
            resources.ApplyResources(tpCaptureGeneral, "tpCaptureGeneral");
            tpCaptureGeneral.Name = "tpCaptureGeneral";
            // 
            // pCapture
            // 
            pCapture.Controls.Add(cbCaptureAutoHideDesktopIcons);
            pCapture.Controls.Add(txtCaptureCustomWindow);
            pCapture.Controls.Add(lblCaptureCustomWindow);
            pCapture.Controls.Add(lblScreenshotDelay);
            pCapture.Controls.Add(btnCaptureCustomRegionSelectRectangle);
            pCapture.Controls.Add(lblCaptureCustomRegion);
            pCapture.Controls.Add(lblCaptureCustomRegionWidth);
            pCapture.Controls.Add(lblCaptureCustomRegionHeight);
            pCapture.Controls.Add(lblCaptureCustomRegionY);
            pCapture.Controls.Add(lblCaptureCustomRegionX);
            pCapture.Controls.Add(nudCaptureCustomRegionHeight);
            pCapture.Controls.Add(nudCaptureCustomRegionWidth);
            pCapture.Controls.Add(nudCaptureCustomRegionY);
            pCapture.Controls.Add(nudCaptureCustomRegionX);
            pCapture.Controls.Add(cbShowCursor);
            pCapture.Controls.Add(lblCaptureShadowOffset);
            pCapture.Controls.Add(cbCaptureTransparent);
            pCapture.Controls.Add(cbCaptureAutoHideTaskbar);
            pCapture.Controls.Add(cbCaptureShadow);
            pCapture.Controls.Add(lblScreenshotDelayInfo);
            pCapture.Controls.Add(cbCaptureClientArea);
            pCapture.Controls.Add(nudScreenshotDelay);
            pCapture.Controls.Add(nudCaptureShadowOffset);
            resources.ApplyResources(pCapture, "pCapture");
            pCapture.Name = "pCapture";
            // 
            // cbCaptureAutoHideDesktopIcons
            // 
            resources.ApplyResources(cbCaptureAutoHideDesktopIcons, "cbCaptureAutoHideDesktopIcons");
            cbCaptureAutoHideDesktopIcons.Name = "cbCaptureAutoHideDesktopIcons";
            cbCaptureAutoHideDesktopIcons.UseVisualStyleBackColor = true;
            cbCaptureAutoHideDesktopIcons.CheckedChanged += cbCaptureAutoHideDesktopIcons_CheckedChanged;
            // 
            // txtCaptureCustomWindow
            // 
            resources.ApplyResources(txtCaptureCustomWindow, "txtCaptureCustomWindow");
            txtCaptureCustomWindow.Name = "txtCaptureCustomWindow";
            txtCaptureCustomWindow.TextChanged += txtCaptureCustomWindow_TextChanged;
            // 
            // lblCaptureCustomWindow
            // 
            resources.ApplyResources(lblCaptureCustomWindow, "lblCaptureCustomWindow");
            lblCaptureCustomWindow.Name = "lblCaptureCustomWindow";
            // 
            // lblScreenshotDelay
            // 
            resources.ApplyResources(lblScreenshotDelay, "lblScreenshotDelay");
            lblScreenshotDelay.Name = "lblScreenshotDelay";
            // 
            // btnCaptureCustomRegionSelectRectangle
            // 
            resources.ApplyResources(btnCaptureCustomRegionSelectRectangle, "btnCaptureCustomRegionSelectRectangle");
            btnCaptureCustomRegionSelectRectangle.Name = "btnCaptureCustomRegionSelectRectangle";
            btnCaptureCustomRegionSelectRectangle.UseVisualStyleBackColor = true;
            btnCaptureCustomRegionSelectRectangle.Click += btnCaptureCustomRegionSelectRectangle_Click;
            // 
            // lblCaptureCustomRegion
            // 
            resources.ApplyResources(lblCaptureCustomRegion, "lblCaptureCustomRegion");
            lblCaptureCustomRegion.Name = "lblCaptureCustomRegion";
            // 
            // lblCaptureCustomRegionWidth
            // 
            resources.ApplyResources(lblCaptureCustomRegionWidth, "lblCaptureCustomRegionWidth");
            lblCaptureCustomRegionWidth.Name = "lblCaptureCustomRegionWidth";
            // 
            // lblCaptureCustomRegionHeight
            // 
            resources.ApplyResources(lblCaptureCustomRegionHeight, "lblCaptureCustomRegionHeight");
            lblCaptureCustomRegionHeight.Name = "lblCaptureCustomRegionHeight";
            // 
            // lblCaptureCustomRegionY
            // 
            resources.ApplyResources(lblCaptureCustomRegionY, "lblCaptureCustomRegionY");
            lblCaptureCustomRegionY.Name = "lblCaptureCustomRegionY";
            // 
            // lblCaptureCustomRegionX
            // 
            resources.ApplyResources(lblCaptureCustomRegionX, "lblCaptureCustomRegionX");
            lblCaptureCustomRegionX.Name = "lblCaptureCustomRegionX";
            // 
            // nudCaptureCustomRegionHeight
            // 
            resources.ApplyResources(nudCaptureCustomRegionHeight, "nudCaptureCustomRegionHeight");
            nudCaptureCustomRegionHeight.Maximum = new decimal(new int[] { int.MinValue, 0, 0, 0 });
            nudCaptureCustomRegionHeight.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            nudCaptureCustomRegionHeight.Name = "nudCaptureCustomRegionHeight";
            nudCaptureCustomRegionHeight.ValueChanged += nudScreenRegionHeight_ValueChanged;
            // 
            // nudCaptureCustomRegionWidth
            // 
            resources.ApplyResources(nudCaptureCustomRegionWidth, "nudCaptureCustomRegionWidth");
            nudCaptureCustomRegionWidth.Maximum = new decimal(new int[] { int.MinValue, 0, 0, 0 });
            nudCaptureCustomRegionWidth.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            nudCaptureCustomRegionWidth.Name = "nudCaptureCustomRegionWidth";
            nudCaptureCustomRegionWidth.ValueChanged += nudScreenRegionWidth_ValueChanged;
            // 
            // nudCaptureCustomRegionY
            // 
            resources.ApplyResources(nudCaptureCustomRegionY, "nudCaptureCustomRegionY");
            nudCaptureCustomRegionY.Maximum = new decimal(new int[] { int.MinValue, 0, 0, 0 });
            nudCaptureCustomRegionY.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            nudCaptureCustomRegionY.Name = "nudCaptureCustomRegionY";
            nudCaptureCustomRegionY.ValueChanged += nudScreenRegionY_ValueChanged;
            // 
            // nudCaptureCustomRegionX
            // 
            resources.ApplyResources(nudCaptureCustomRegionX, "nudCaptureCustomRegionX");
            nudCaptureCustomRegionX.Maximum = new decimal(new int[] { int.MinValue, 0, 0, 0 });
            nudCaptureCustomRegionX.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            nudCaptureCustomRegionX.Name = "nudCaptureCustomRegionX";
            nudCaptureCustomRegionX.ValueChanged += nudScreenRegionX_ValueChanged;
            // 
            // cbShowCursor
            // 
            resources.ApplyResources(cbShowCursor, "cbShowCursor");
            cbShowCursor.Name = "cbShowCursor";
            cbShowCursor.UseVisualStyleBackColor = true;
            cbShowCursor.CheckedChanged += cbShowCursor_CheckedChanged;
            // 
            // lblCaptureShadowOffset
            // 
            resources.ApplyResources(lblCaptureShadowOffset, "lblCaptureShadowOffset");
            lblCaptureShadowOffset.Name = "lblCaptureShadowOffset";
            // 
            // cbCaptureTransparent
            // 
            resources.ApplyResources(cbCaptureTransparent, "cbCaptureTransparent");
            cbCaptureTransparent.Name = "cbCaptureTransparent";
            cbCaptureTransparent.UseVisualStyleBackColor = true;
            cbCaptureTransparent.CheckedChanged += cbCaptureTransparent_CheckedChanged;
            // 
            // cbCaptureAutoHideTaskbar
            // 
            resources.ApplyResources(cbCaptureAutoHideTaskbar, "cbCaptureAutoHideTaskbar");
            cbCaptureAutoHideTaskbar.Name = "cbCaptureAutoHideTaskbar";
            cbCaptureAutoHideTaskbar.UseVisualStyleBackColor = true;
            cbCaptureAutoHideTaskbar.CheckedChanged += cbCaptureAutoHideTaskbar_CheckedChanged;
            // 
            // cbCaptureShadow
            // 
            resources.ApplyResources(cbCaptureShadow, "cbCaptureShadow");
            cbCaptureShadow.Name = "cbCaptureShadow";
            cbCaptureShadow.UseVisualStyleBackColor = true;
            cbCaptureShadow.CheckedChanged += cbCaptureShadow_CheckedChanged;
            // 
            // lblScreenshotDelayInfo
            // 
            resources.ApplyResources(lblScreenshotDelayInfo, "lblScreenshotDelayInfo");
            lblScreenshotDelayInfo.Name = "lblScreenshotDelayInfo";
            // 
            // cbCaptureClientArea
            // 
            resources.ApplyResources(cbCaptureClientArea, "cbCaptureClientArea");
            cbCaptureClientArea.Name = "cbCaptureClientArea";
            cbCaptureClientArea.UseVisualStyleBackColor = true;
            cbCaptureClientArea.CheckedChanged += cbCaptureClientArea_CheckedChanged;
            // 
            // nudScreenshotDelay
            // 
            nudScreenshotDelay.DecimalPlaces = 1;
            resources.ApplyResources(nudScreenshotDelay, "nudScreenshotDelay");
            nudScreenshotDelay.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudScreenshotDelay.Name = "nudScreenshotDelay";
            nudScreenshotDelay.ValueChanged += nudScreenshotDelay_ValueChanged;
            // 
            // nudCaptureShadowOffset
            // 
            resources.ApplyResources(nudCaptureShadowOffset, "nudCaptureShadowOffset");
            nudCaptureShadowOffset.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudCaptureShadowOffset.Name = "nudCaptureShadowOffset";
            nudCaptureShadowOffset.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudCaptureShadowOffset.ValueChanged += nudCaptureShadowOffset_ValueChanged;
            // 
            // cbOverrideCaptureSettings
            // 
            resources.ApplyResources(cbOverrideCaptureSettings, "cbOverrideCaptureSettings");
            cbOverrideCaptureSettings.Checked = true;
            cbOverrideCaptureSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideCaptureSettings.Name = "cbOverrideCaptureSettings";
            cbOverrideCaptureSettings.UseVisualStyleBackColor = true;
            cbOverrideCaptureSettings.CheckedChanged += cbUseDefaultCaptureSettings_CheckedChanged;
            // 
            // tpRegionCapture
            // 
            tpRegionCapture.BackColor = System.Drawing.SystemColors.Window;
            tpRegionCapture.Controls.Add(lblRegionCaptureBackgroundDimStrengthHint);
            tpRegionCapture.Controls.Add(nudRegionCaptureBackgroundDimStrength);
            tpRegionCapture.Controls.Add(lblRegionCaptureBackgroundDimStrength);
            tpRegionCapture.Controls.Add(cbRegionCaptureActiveMonitorMode);
            tpRegionCapture.Controls.Add(nudRegionCaptureFPSLimit);
            tpRegionCapture.Controls.Add(lblRegionCaptureFPSLimit);
            tpRegionCapture.Controls.Add(cbRegionCaptureShowFPS);
            tpRegionCapture.Controls.Add(flpRegionCaptureFixedSize);
            tpRegionCapture.Controls.Add(cbRegionCaptureIsFixedSize);
            tpRegionCapture.Controls.Add(cbRegionCaptureShowCrosshair);
            tpRegionCapture.Controls.Add(lblRegionCaptureMagnifierPixelSize);
            tpRegionCapture.Controls.Add(lblRegionCaptureMagnifierPixelCount);
            tpRegionCapture.Controls.Add(cbRegionCaptureUseSquareMagnifier);
            tpRegionCapture.Controls.Add(cbRegionCaptureShowMagnifier);
            tpRegionCapture.Controls.Add(cbRegionCaptureShowInfo);
            tpRegionCapture.Controls.Add(btnRegionCaptureSnapSizesRemove);
            tpRegionCapture.Controls.Add(btnRegionCaptureSnapSizesAdd);
            tpRegionCapture.Controls.Add(cbRegionCaptureSnapSizes);
            tpRegionCapture.Controls.Add(lblRegionCaptureSnapSizes);
            tpRegionCapture.Controls.Add(cbRegionCaptureUseCustomInfoText);
            tpRegionCapture.Controls.Add(cbRegionCaptureDetectControls);
            tpRegionCapture.Controls.Add(cbRegionCaptureDetectWindows);
            tpRegionCapture.Controls.Add(cbRegionCaptureMouse5ClickAction);
            tpRegionCapture.Controls.Add(lblRegionCaptureMouse5ClickAction);
            tpRegionCapture.Controls.Add(cbRegionCaptureMouse4ClickAction);
            tpRegionCapture.Controls.Add(lblRegionCaptureMouse4ClickAction);
            tpRegionCapture.Controls.Add(cbRegionCaptureMouseMiddleClickAction);
            tpRegionCapture.Controls.Add(lblRegionCaptureMouseMiddleClickAction);
            tpRegionCapture.Controls.Add(cbRegionCaptureMouseRightClickAction);
            tpRegionCapture.Controls.Add(lblRegionCaptureMouseRightClickAction);
            tpRegionCapture.Controls.Add(cbRegionCaptureMultiRegionMode);
            tpRegionCapture.Controls.Add(pRegionCaptureSnapSizes);
            tpRegionCapture.Controls.Add(txtRegionCaptureCustomInfoText);
            tpRegionCapture.Controls.Add(nudRegionCaptureMagnifierPixelCount);
            tpRegionCapture.Controls.Add(nudRegionCaptureMagnifierPixelSize);
            resources.ApplyResources(tpRegionCapture, "tpRegionCapture");
            tpRegionCapture.Name = "tpRegionCapture";
            // 
            // lblRegionCaptureBackgroundDimStrengthHint
            // 
            resources.ApplyResources(lblRegionCaptureBackgroundDimStrengthHint, "lblRegionCaptureBackgroundDimStrengthHint");
            lblRegionCaptureBackgroundDimStrengthHint.Name = "lblRegionCaptureBackgroundDimStrengthHint";
            // 
            // nudRegionCaptureBackgroundDimStrength
            // 
            resources.ApplyResources(nudRegionCaptureBackgroundDimStrength, "nudRegionCaptureBackgroundDimStrength");
            nudRegionCaptureBackgroundDimStrength.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            nudRegionCaptureBackgroundDimStrength.Name = "nudRegionCaptureBackgroundDimStrength";
            nudRegionCaptureBackgroundDimStrength.ValueChanged += nudRegionCaptureBackgroundDimStrength_ValueChanged;
            // 
            // lblRegionCaptureBackgroundDimStrength
            // 
            resources.ApplyResources(lblRegionCaptureBackgroundDimStrength, "lblRegionCaptureBackgroundDimStrength");
            lblRegionCaptureBackgroundDimStrength.Name = "lblRegionCaptureBackgroundDimStrength";
            // 
            // cbRegionCaptureActiveMonitorMode
            // 
            resources.ApplyResources(cbRegionCaptureActiveMonitorMode, "cbRegionCaptureActiveMonitorMode");
            cbRegionCaptureActiveMonitorMode.Name = "cbRegionCaptureActiveMonitorMode";
            cbRegionCaptureActiveMonitorMode.UseVisualStyleBackColor = true;
            cbRegionCaptureActiveMonitorMode.CheckedChanged += cbRegionCaptureActiveMonitorMode_CheckedChanged;
            // 
            // nudRegionCaptureFPSLimit
            // 
            resources.ApplyResources(nudRegionCaptureFPSLimit, "nudRegionCaptureFPSLimit");
            nudRegionCaptureFPSLimit.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudRegionCaptureFPSLimit.Name = "nudRegionCaptureFPSLimit";
            nudRegionCaptureFPSLimit.ValueChanged += nudRegionCaptureFPSLimit_ValueChanged;
            // 
            // lblRegionCaptureFPSLimit
            // 
            resources.ApplyResources(lblRegionCaptureFPSLimit, "lblRegionCaptureFPSLimit");
            lblRegionCaptureFPSLimit.Name = "lblRegionCaptureFPSLimit";
            // 
            // cbRegionCaptureShowFPS
            // 
            resources.ApplyResources(cbRegionCaptureShowFPS, "cbRegionCaptureShowFPS");
            cbRegionCaptureShowFPS.Name = "cbRegionCaptureShowFPS";
            cbRegionCaptureShowFPS.UseVisualStyleBackColor = true;
            cbRegionCaptureShowFPS.CheckedChanged += cbRegionCaptureShowFPS_CheckedChanged;
            // 
            // flpRegionCaptureFixedSize
            // 
            resources.ApplyResources(flpRegionCaptureFixedSize, "flpRegionCaptureFixedSize");
            flpRegionCaptureFixedSize.Controls.Add(lblRegionCaptureFixedSizeWidth);
            flpRegionCaptureFixedSize.Controls.Add(nudRegionCaptureFixedSizeWidth);
            flpRegionCaptureFixedSize.Controls.Add(lblRegionCaptureFixedSizeHeight);
            flpRegionCaptureFixedSize.Controls.Add(nudRegionCaptureFixedSizeHeight);
            flpRegionCaptureFixedSize.Name = "flpRegionCaptureFixedSize";
            // 
            // lblRegionCaptureFixedSizeWidth
            // 
            resources.ApplyResources(lblRegionCaptureFixedSizeWidth, "lblRegionCaptureFixedSizeWidth");
            lblRegionCaptureFixedSizeWidth.Name = "lblRegionCaptureFixedSizeWidth";
            // 
            // nudRegionCaptureFixedSizeWidth
            // 
            nudRegionCaptureFixedSizeWidth.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            resources.ApplyResources(nudRegionCaptureFixedSizeWidth, "nudRegionCaptureFixedSizeWidth");
            nudRegionCaptureFixedSizeWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudRegionCaptureFixedSizeWidth.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudRegionCaptureFixedSizeWidth.Name = "nudRegionCaptureFixedSizeWidth";
            nudRegionCaptureFixedSizeWidth.Value = new decimal(new int[] { 10, 0, 0, 0 });
            nudRegionCaptureFixedSizeWidth.ValueChanged += nudRegionCaptureFixedSizeWidth_ValueChanged;
            // 
            // lblRegionCaptureFixedSizeHeight
            // 
            resources.ApplyResources(lblRegionCaptureFixedSizeHeight, "lblRegionCaptureFixedSizeHeight");
            lblRegionCaptureFixedSizeHeight.Name = "lblRegionCaptureFixedSizeHeight";
            // 
            // nudRegionCaptureFixedSizeHeight
            // 
            nudRegionCaptureFixedSizeHeight.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            resources.ApplyResources(nudRegionCaptureFixedSizeHeight, "nudRegionCaptureFixedSizeHeight");
            nudRegionCaptureFixedSizeHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudRegionCaptureFixedSizeHeight.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudRegionCaptureFixedSizeHeight.Name = "nudRegionCaptureFixedSizeHeight";
            nudRegionCaptureFixedSizeHeight.Value = new decimal(new int[] { 10, 0, 0, 0 });
            nudRegionCaptureFixedSizeHeight.ValueChanged += nudRegionCaptureFixedSizeHeight_ValueChanged;
            // 
            // cbRegionCaptureIsFixedSize
            // 
            resources.ApplyResources(cbRegionCaptureIsFixedSize, "cbRegionCaptureIsFixedSize");
            cbRegionCaptureIsFixedSize.Name = "cbRegionCaptureIsFixedSize";
            cbRegionCaptureIsFixedSize.UseVisualStyleBackColor = true;
            cbRegionCaptureIsFixedSize.CheckedChanged += cbRegionCaptureIsFixedSize_CheckedChanged;
            // 
            // cbRegionCaptureShowCrosshair
            // 
            resources.ApplyResources(cbRegionCaptureShowCrosshair, "cbRegionCaptureShowCrosshair");
            cbRegionCaptureShowCrosshair.Name = "cbRegionCaptureShowCrosshair";
            cbRegionCaptureShowCrosshair.UseVisualStyleBackColor = true;
            cbRegionCaptureShowCrosshair.CheckedChanged += cbRegionCaptureShowCrosshair_CheckedChanged;
            // 
            // lblRegionCaptureMagnifierPixelSize
            // 
            resources.ApplyResources(lblRegionCaptureMagnifierPixelSize, "lblRegionCaptureMagnifierPixelSize");
            lblRegionCaptureMagnifierPixelSize.Name = "lblRegionCaptureMagnifierPixelSize";
            // 
            // lblRegionCaptureMagnifierPixelCount
            // 
            resources.ApplyResources(lblRegionCaptureMagnifierPixelCount, "lblRegionCaptureMagnifierPixelCount");
            lblRegionCaptureMagnifierPixelCount.Name = "lblRegionCaptureMagnifierPixelCount";
            // 
            // cbRegionCaptureUseSquareMagnifier
            // 
            resources.ApplyResources(cbRegionCaptureUseSquareMagnifier, "cbRegionCaptureUseSquareMagnifier");
            cbRegionCaptureUseSquareMagnifier.Name = "cbRegionCaptureUseSquareMagnifier";
            cbRegionCaptureUseSquareMagnifier.UseVisualStyleBackColor = true;
            cbRegionCaptureUseSquareMagnifier.CheckedChanged += cbRegionCaptureUseSquareMagnifier_CheckedChanged;
            // 
            // cbRegionCaptureShowMagnifier
            // 
            resources.ApplyResources(cbRegionCaptureShowMagnifier, "cbRegionCaptureShowMagnifier");
            cbRegionCaptureShowMagnifier.Name = "cbRegionCaptureShowMagnifier";
            cbRegionCaptureShowMagnifier.UseVisualStyleBackColor = true;
            cbRegionCaptureShowMagnifier.CheckedChanged += cbRegionCaptureShowMagnifier_CheckedChanged;
            // 
            // cbRegionCaptureShowInfo
            // 
            resources.ApplyResources(cbRegionCaptureShowInfo, "cbRegionCaptureShowInfo");
            cbRegionCaptureShowInfo.Name = "cbRegionCaptureShowInfo";
            cbRegionCaptureShowInfo.UseVisualStyleBackColor = true;
            cbRegionCaptureShowInfo.CheckedChanged += cbRegionCaptureShowInfo_CheckedChanged;
            // 
            // btnRegionCaptureSnapSizesRemove
            // 
            resources.ApplyResources(btnRegionCaptureSnapSizesRemove, "btnRegionCaptureSnapSizesRemove");
            btnRegionCaptureSnapSizesRemove.Name = "btnRegionCaptureSnapSizesRemove";
            btnRegionCaptureSnapSizesRemove.UseVisualStyleBackColor = true;
            btnRegionCaptureSnapSizesRemove.Click += btnRegionCaptureSnapSizesRemove_Click;
            // 
            // btnRegionCaptureSnapSizesAdd
            // 
            resources.ApplyResources(btnRegionCaptureSnapSizesAdd, "btnRegionCaptureSnapSizesAdd");
            btnRegionCaptureSnapSizesAdd.Name = "btnRegionCaptureSnapSizesAdd";
            btnRegionCaptureSnapSizesAdd.UseVisualStyleBackColor = true;
            btnRegionCaptureSnapSizesAdd.Click += btnRegionCaptureSnapSizesAdd_Click;
            // 
            // cbRegionCaptureSnapSizes
            // 
            cbRegionCaptureSnapSizes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbRegionCaptureSnapSizes.FormattingEnabled = true;
            resources.ApplyResources(cbRegionCaptureSnapSizes, "cbRegionCaptureSnapSizes");
            cbRegionCaptureSnapSizes.Name = "cbRegionCaptureSnapSizes";
            // 
            // lblRegionCaptureSnapSizes
            // 
            resources.ApplyResources(lblRegionCaptureSnapSizes, "lblRegionCaptureSnapSizes");
            lblRegionCaptureSnapSizes.Name = "lblRegionCaptureSnapSizes";
            // 
            // cbRegionCaptureUseCustomInfoText
            // 
            resources.ApplyResources(cbRegionCaptureUseCustomInfoText, "cbRegionCaptureUseCustomInfoText");
            cbRegionCaptureUseCustomInfoText.Name = "cbRegionCaptureUseCustomInfoText";
            cbRegionCaptureUseCustomInfoText.UseVisualStyleBackColor = true;
            cbRegionCaptureUseCustomInfoText.CheckedChanged += cbRegionCaptureUseCustomInfoText_CheckedChanged;
            // 
            // cbRegionCaptureDetectControls
            // 
            resources.ApplyResources(cbRegionCaptureDetectControls, "cbRegionCaptureDetectControls");
            cbRegionCaptureDetectControls.Name = "cbRegionCaptureDetectControls";
            cbRegionCaptureDetectControls.UseVisualStyleBackColor = true;
            cbRegionCaptureDetectControls.CheckedChanged += cbRegionCaptureDetectControls_CheckedChanged;
            // 
            // cbRegionCaptureDetectWindows
            // 
            resources.ApplyResources(cbRegionCaptureDetectWindows, "cbRegionCaptureDetectWindows");
            cbRegionCaptureDetectWindows.Name = "cbRegionCaptureDetectWindows";
            cbRegionCaptureDetectWindows.UseVisualStyleBackColor = true;
            cbRegionCaptureDetectWindows.CheckedChanged += cbRegionCaptureDetectWindows_CheckedChanged;
            // 
            // cbRegionCaptureMouse5ClickAction
            // 
            cbRegionCaptureMouse5ClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbRegionCaptureMouse5ClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbRegionCaptureMouse5ClickAction, "cbRegionCaptureMouse5ClickAction");
            cbRegionCaptureMouse5ClickAction.Name = "cbRegionCaptureMouse5ClickAction";
            cbRegionCaptureMouse5ClickAction.SelectedIndexChanged += cbRegionCaptureMouse5ClickAction_SelectedIndexChanged;
            // 
            // lblRegionCaptureMouse5ClickAction
            // 
            resources.ApplyResources(lblRegionCaptureMouse5ClickAction, "lblRegionCaptureMouse5ClickAction");
            lblRegionCaptureMouse5ClickAction.Name = "lblRegionCaptureMouse5ClickAction";
            // 
            // cbRegionCaptureMouse4ClickAction
            // 
            cbRegionCaptureMouse4ClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbRegionCaptureMouse4ClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbRegionCaptureMouse4ClickAction, "cbRegionCaptureMouse4ClickAction");
            cbRegionCaptureMouse4ClickAction.Name = "cbRegionCaptureMouse4ClickAction";
            cbRegionCaptureMouse4ClickAction.SelectedIndexChanged += cbRegionCaptureMouse4ClickAction_SelectedIndexChanged;
            // 
            // lblRegionCaptureMouse4ClickAction
            // 
            resources.ApplyResources(lblRegionCaptureMouse4ClickAction, "lblRegionCaptureMouse4ClickAction");
            lblRegionCaptureMouse4ClickAction.Name = "lblRegionCaptureMouse4ClickAction";
            // 
            // cbRegionCaptureMouseMiddleClickAction
            // 
            cbRegionCaptureMouseMiddleClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbRegionCaptureMouseMiddleClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbRegionCaptureMouseMiddleClickAction, "cbRegionCaptureMouseMiddleClickAction");
            cbRegionCaptureMouseMiddleClickAction.Name = "cbRegionCaptureMouseMiddleClickAction";
            cbRegionCaptureMouseMiddleClickAction.SelectedIndexChanged += cbRegionCaptureMouseMiddleClickAction_SelectedIndexChanged;
            // 
            // lblRegionCaptureMouseMiddleClickAction
            // 
            resources.ApplyResources(lblRegionCaptureMouseMiddleClickAction, "lblRegionCaptureMouseMiddleClickAction");
            lblRegionCaptureMouseMiddleClickAction.Name = "lblRegionCaptureMouseMiddleClickAction";
            // 
            // cbRegionCaptureMouseRightClickAction
            // 
            cbRegionCaptureMouseRightClickAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbRegionCaptureMouseRightClickAction.FormattingEnabled = true;
            resources.ApplyResources(cbRegionCaptureMouseRightClickAction, "cbRegionCaptureMouseRightClickAction");
            cbRegionCaptureMouseRightClickAction.Name = "cbRegionCaptureMouseRightClickAction";
            cbRegionCaptureMouseRightClickAction.SelectedIndexChanged += cbRegionCaptureMouseRightClickAction_SelectedIndexChanged;
            // 
            // lblRegionCaptureMouseRightClickAction
            // 
            resources.ApplyResources(lblRegionCaptureMouseRightClickAction, "lblRegionCaptureMouseRightClickAction");
            lblRegionCaptureMouseRightClickAction.Name = "lblRegionCaptureMouseRightClickAction";
            // 
            // cbRegionCaptureMultiRegionMode
            // 
            resources.ApplyResources(cbRegionCaptureMultiRegionMode, "cbRegionCaptureMultiRegionMode");
            cbRegionCaptureMultiRegionMode.Name = "cbRegionCaptureMultiRegionMode";
            cbRegionCaptureMultiRegionMode.UseVisualStyleBackColor = true;
            cbRegionCaptureMultiRegionMode.CheckedChanged += cbRegionCaptureMultiRegionMode_CheckedChanged;
            // 
            // pRegionCaptureSnapSizes
            // 
            pRegionCaptureSnapSizes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pRegionCaptureSnapSizes.Controls.Add(btnRegionCaptureSnapSizesDialogCancel);
            pRegionCaptureSnapSizes.Controls.Add(btnRegionCaptureSnapSizesDialogAdd);
            pRegionCaptureSnapSizes.Controls.Add(nudRegionCaptureSnapSizesHeight);
            pRegionCaptureSnapSizes.Controls.Add(RegionCaptureSnapSizesHeight);
            pRegionCaptureSnapSizes.Controls.Add(nudRegionCaptureSnapSizesWidth);
            pRegionCaptureSnapSizes.Controls.Add(lblRegionCaptureSnapSizesWidth);
            resources.ApplyResources(pRegionCaptureSnapSizes, "pRegionCaptureSnapSizes");
            pRegionCaptureSnapSizes.Name = "pRegionCaptureSnapSizes";
            // 
            // btnRegionCaptureSnapSizesDialogCancel
            // 
            resources.ApplyResources(btnRegionCaptureSnapSizesDialogCancel, "btnRegionCaptureSnapSizesDialogCancel");
            btnRegionCaptureSnapSizesDialogCancel.Name = "btnRegionCaptureSnapSizesDialogCancel";
            btnRegionCaptureSnapSizesDialogCancel.UseVisualStyleBackColor = true;
            btnRegionCaptureSnapSizesDialogCancel.Click += btnRegionCaptureSnapSizesDialogCancel_Click;
            // 
            // btnRegionCaptureSnapSizesDialogAdd
            // 
            resources.ApplyResources(btnRegionCaptureSnapSizesDialogAdd, "btnRegionCaptureSnapSizesDialogAdd");
            btnRegionCaptureSnapSizesDialogAdd.Name = "btnRegionCaptureSnapSizesDialogAdd";
            btnRegionCaptureSnapSizesDialogAdd.UseVisualStyleBackColor = true;
            btnRegionCaptureSnapSizesDialogAdd.Click += btnRegionCaptureSnapSizesDialogAdd_Click;
            // 
            // nudRegionCaptureSnapSizesHeight
            // 
            resources.ApplyResources(nudRegionCaptureSnapSizesHeight, "nudRegionCaptureSnapSizesHeight");
            nudRegionCaptureSnapSizesHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudRegionCaptureSnapSizesHeight.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nudRegionCaptureSnapSizesHeight.Name = "nudRegionCaptureSnapSizesHeight";
            nudRegionCaptureSnapSizesHeight.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // RegionCaptureSnapSizesHeight
            // 
            resources.ApplyResources(RegionCaptureSnapSizesHeight, "RegionCaptureSnapSizesHeight");
            RegionCaptureSnapSizesHeight.Name = "RegionCaptureSnapSizesHeight";
            // 
            // nudRegionCaptureSnapSizesWidth
            // 
            resources.ApplyResources(nudRegionCaptureSnapSizesWidth, "nudRegionCaptureSnapSizesWidth");
            nudRegionCaptureSnapSizesWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudRegionCaptureSnapSizesWidth.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nudRegionCaptureSnapSizesWidth.Name = "nudRegionCaptureSnapSizesWidth";
            nudRegionCaptureSnapSizesWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblRegionCaptureSnapSizesWidth
            // 
            resources.ApplyResources(lblRegionCaptureSnapSizesWidth, "lblRegionCaptureSnapSizesWidth");
            lblRegionCaptureSnapSizesWidth.Name = "lblRegionCaptureSnapSizesWidth";
            // 
            // txtRegionCaptureCustomInfoText
            // 
            resources.ApplyResources(txtRegionCaptureCustomInfoText, "txtRegionCaptureCustomInfoText");
            txtRegionCaptureCustomInfoText.Name = "txtRegionCaptureCustomInfoText";
            txtRegionCaptureCustomInfoText.TextChanged += txtRegionCaptureCustomInfoText_TextChanged;
            // 
            // nudRegionCaptureMagnifierPixelCount
            // 
            nudRegionCaptureMagnifierPixelCount.Increment = new decimal(new int[] { 2, 0, 0, 0 });
            resources.ApplyResources(nudRegionCaptureMagnifierPixelCount, "nudRegionCaptureMagnifierPixelCount");
            nudRegionCaptureMagnifierPixelCount.Name = "nudRegionCaptureMagnifierPixelCount";
            nudRegionCaptureMagnifierPixelCount.ValueChanged += nudRegionCaptureMagnifierPixelCount_ValueChanged;
            // 
            // nudRegionCaptureMagnifierPixelSize
            // 
            resources.ApplyResources(nudRegionCaptureMagnifierPixelSize, "nudRegionCaptureMagnifierPixelSize");
            nudRegionCaptureMagnifierPixelSize.Name = "nudRegionCaptureMagnifierPixelSize";
            nudRegionCaptureMagnifierPixelSize.ValueChanged += nudRegionCaptureMagnifierPixelSize_ValueChanged;
            // 
            // tpScreenRecorder
            // 
            tpScreenRecorder.BackColor = System.Drawing.SystemColors.Window;
            tpScreenRecorder.Controls.Add(cbScreenRecordTransparentRegion);
            tpScreenRecorder.Controls.Add(cbScreenRecordTwoPassEncoding);
            tpScreenRecorder.Controls.Add(cbScreenRecordConfirmAbort);
            tpScreenRecorder.Controls.Add(cbScreenRecorderShowCursor);
            tpScreenRecorder.Controls.Add(btnScreenRecorderFFmpegOptions);
            tpScreenRecorder.Controls.Add(lblScreenRecorderStartDelay);
            tpScreenRecorder.Controls.Add(cbScreenRecordAutoStart);
            tpScreenRecorder.Controls.Add(lblScreenRecorderFixedDuration);
            tpScreenRecorder.Controls.Add(nudScreenRecordFPS);
            tpScreenRecorder.Controls.Add(lblScreenRecordFPS);
            tpScreenRecorder.Controls.Add(nudScreenRecorderDuration);
            tpScreenRecorder.Controls.Add(nudScreenRecorderStartDelay);
            tpScreenRecorder.Controls.Add(cbScreenRecorderFixedDuration);
            tpScreenRecorder.Controls.Add(nudGIFFPS);
            tpScreenRecorder.Controls.Add(lblGIFFPS);
            resources.ApplyResources(tpScreenRecorder, "tpScreenRecorder");
            tpScreenRecorder.Name = "tpScreenRecorder";
            // 
            // cbScreenRecordTransparentRegion
            // 
            resources.ApplyResources(cbScreenRecordTransparentRegion, "cbScreenRecordTransparentRegion");
            cbScreenRecordTransparentRegion.Name = "cbScreenRecordTransparentRegion";
            cbScreenRecordTransparentRegion.UseVisualStyleBackColor = true;
            cbScreenRecordTransparentRegion.CheckedChanged += cbScreenRecordTransparentRegion_CheckedChanged;
            // 
            // cbScreenRecordTwoPassEncoding
            // 
            resources.ApplyResources(cbScreenRecordTwoPassEncoding, "cbScreenRecordTwoPassEncoding");
            cbScreenRecordTwoPassEncoding.Name = "cbScreenRecordTwoPassEncoding";
            cbScreenRecordTwoPassEncoding.UseVisualStyleBackColor = true;
            cbScreenRecordTwoPassEncoding.CheckedChanged += cbScreenRecordTwoPassEncoding_CheckedChanged;
            // 
            // cbScreenRecordConfirmAbort
            // 
            resources.ApplyResources(cbScreenRecordConfirmAbort, "cbScreenRecordConfirmAbort");
            cbScreenRecordConfirmAbort.Name = "cbScreenRecordConfirmAbort";
            cbScreenRecordConfirmAbort.UseVisualStyleBackColor = true;
            cbScreenRecordConfirmAbort.CheckedChanged += cbScreenRecordConfirmAbort_CheckedChanged;
            // 
            // cbScreenRecorderShowCursor
            // 
            resources.ApplyResources(cbScreenRecorderShowCursor, "cbScreenRecorderShowCursor");
            cbScreenRecorderShowCursor.Name = "cbScreenRecorderShowCursor";
            cbScreenRecorderShowCursor.UseVisualStyleBackColor = true;
            cbScreenRecorderShowCursor.CheckedChanged += cbScreenRecorderShowCursor_CheckedChanged;
            // 
            // btnScreenRecorderFFmpegOptions
            // 
            resources.ApplyResources(btnScreenRecorderFFmpegOptions, "btnScreenRecorderFFmpegOptions");
            btnScreenRecorderFFmpegOptions.Name = "btnScreenRecorderFFmpegOptions";
            btnScreenRecorderFFmpegOptions.UseVisualStyleBackColor = true;
            btnScreenRecorderFFmpegOptions.Click += btnScreenRecorderFFmpegOptions_Click;
            // 
            // lblScreenRecorderStartDelay
            // 
            resources.ApplyResources(lblScreenRecorderStartDelay, "lblScreenRecorderStartDelay");
            lblScreenRecorderStartDelay.Name = "lblScreenRecorderStartDelay";
            // 
            // cbScreenRecordAutoStart
            // 
            resources.ApplyResources(cbScreenRecordAutoStart, "cbScreenRecordAutoStart");
            cbScreenRecordAutoStart.Name = "cbScreenRecordAutoStart";
            cbScreenRecordAutoStart.UseVisualStyleBackColor = true;
            cbScreenRecordAutoStart.CheckedChanged += cbScreenRecordAutoStart_CheckedChanged;
            // 
            // lblScreenRecorderFixedDuration
            // 
            resources.ApplyResources(lblScreenRecorderFixedDuration, "lblScreenRecorderFixedDuration");
            lblScreenRecorderFixedDuration.Name = "lblScreenRecorderFixedDuration";
            // 
            // nudScreenRecordFPS
            // 
            resources.ApplyResources(nudScreenRecordFPS, "nudScreenRecordFPS");
            nudScreenRecordFPS.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudScreenRecordFPS.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudScreenRecordFPS.Name = "nudScreenRecordFPS";
            nudScreenRecordFPS.Value = new decimal(new int[] { 20, 0, 0, 0 });
            nudScreenRecordFPS.ValueChanged += nudScreenRecordFPS_ValueChanged;
            // 
            // lblScreenRecordFPS
            // 
            resources.ApplyResources(lblScreenRecordFPS, "lblScreenRecordFPS");
            lblScreenRecordFPS.Name = "lblScreenRecordFPS";
            // 
            // nudScreenRecorderDuration
            // 
            nudScreenRecorderDuration.DecimalPlaces = 1;
            nudScreenRecorderDuration.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            resources.ApplyResources(nudScreenRecorderDuration, "nudScreenRecorderDuration");
            nudScreenRecorderDuration.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudScreenRecorderDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudScreenRecorderDuration.Name = "nudScreenRecorderDuration";
            nudScreenRecorderDuration.Value = new decimal(new int[] { 3, 0, 0, 0 });
            nudScreenRecorderDuration.ValueChanged += nudScreenRecorderDuration_ValueChanged;
            // 
            // nudScreenRecorderStartDelay
            // 
            nudScreenRecorderStartDelay.DecimalPlaces = 1;
            nudScreenRecorderStartDelay.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            resources.ApplyResources(nudScreenRecorderStartDelay, "nudScreenRecorderStartDelay");
            nudScreenRecorderStartDelay.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudScreenRecorderStartDelay.Name = "nudScreenRecorderStartDelay";
            nudScreenRecorderStartDelay.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudScreenRecorderStartDelay.ValueChanged += nudScreenRecorderStartDelay_ValueChanged;
            // 
            // cbScreenRecorderFixedDuration
            // 
            resources.ApplyResources(cbScreenRecorderFixedDuration, "cbScreenRecorderFixedDuration");
            cbScreenRecorderFixedDuration.Name = "cbScreenRecorderFixedDuration";
            cbScreenRecorderFixedDuration.UseVisualStyleBackColor = true;
            cbScreenRecorderFixedDuration.CheckedChanged += cbScreenRecorderFixedDuration_CheckedChanged;
            // 
            // nudGIFFPS
            // 
            resources.ApplyResources(nudGIFFPS, "nudGIFFPS");
            nudGIFFPS.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            nudGIFFPS.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudGIFFPS.Name = "nudGIFFPS";
            nudGIFFPS.Value = new decimal(new int[] { 5, 0, 0, 0 });
            nudGIFFPS.ValueChanged += nudGIFFPS_ValueChanged;
            // 
            // lblGIFFPS
            // 
            resources.ApplyResources(lblGIFFPS, "lblGIFFPS");
            lblGIFFPS.Name = "lblGIFFPS";
            // 
            // tpOCR
            // 
            tpOCR.Controls.Add(btnCaptureOCRHelp);
            tpOCR.Controls.Add(cbCaptureOCRAutoCopy);
            tpOCR.Controls.Add(cbCloseWindowAfterOpenServiceLink);
            tpOCR.Controls.Add(cbCaptureOCRSilent);
            tpOCR.Controls.Add(lblOCRDefaultLanguage);
            tpOCR.Controls.Add(cbCaptureOCRDefaultLanguage);
            resources.ApplyResources(tpOCR, "tpOCR");
            tpOCR.Name = "tpOCR";
            tpOCR.UseVisualStyleBackColor = true;
            // 
            // btnCaptureOCRHelp
            // 
            btnCaptureOCRHelp.Image = Properties.Resources.question;
            resources.ApplyResources(btnCaptureOCRHelp, "btnCaptureOCRHelp");
            btnCaptureOCRHelp.Name = "btnCaptureOCRHelp";
            btnCaptureOCRHelp.UseVisualStyleBackColor = true;
            btnCaptureOCRHelp.Click += btnCaptureOCRHelp_Click;
            // 
            // cbCaptureOCRAutoCopy
            // 
            resources.ApplyResources(cbCaptureOCRAutoCopy, "cbCaptureOCRAutoCopy");
            cbCaptureOCRAutoCopy.Name = "cbCaptureOCRAutoCopy";
            cbCaptureOCRAutoCopy.UseVisualStyleBackColor = true;
            cbCaptureOCRAutoCopy.CheckedChanged += cbCaptureOCRAutoCopy_CheckedChanged;
            // 
            // cbCloseWindowAfterOpenServiceLink
            // 
            resources.ApplyResources(cbCloseWindowAfterOpenServiceLink, "cbCloseWindowAfterOpenServiceLink");
            cbCloseWindowAfterOpenServiceLink.Name = "cbCloseWindowAfterOpenServiceLink";
            cbCloseWindowAfterOpenServiceLink.UseVisualStyleBackColor = true;
            cbCloseWindowAfterOpenServiceLink.CheckedChanged += cbCloseWindowAfterOpenServiceLink_CheckedChanged;
            // 
            // cbCaptureOCRSilent
            // 
            resources.ApplyResources(cbCaptureOCRSilent, "cbCaptureOCRSilent");
            cbCaptureOCRSilent.Name = "cbCaptureOCRSilent";
            cbCaptureOCRSilent.UseVisualStyleBackColor = true;
            cbCaptureOCRSilent.CheckedChanged += cbCaptureOCRSilent_CheckedChanged;
            // 
            // lblOCRDefaultLanguage
            // 
            resources.ApplyResources(lblOCRDefaultLanguage, "lblOCRDefaultLanguage");
            lblOCRDefaultLanguage.Name = "lblOCRDefaultLanguage";
            // 
            // cbCaptureOCRDefaultLanguage
            // 
            cbCaptureOCRDefaultLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbCaptureOCRDefaultLanguage.FormattingEnabled = true;
            resources.ApplyResources(cbCaptureOCRDefaultLanguage, "cbCaptureOCRDefaultLanguage");
            cbCaptureOCRDefaultLanguage.Name = "cbCaptureOCRDefaultLanguage";
            cbCaptureOCRDefaultLanguage.SelectedIndexChanged += cbCaptureOCRDefaultLanguage_SelectedIndexChanged;
            // 
            // tpUpload
            // 
            tpUpload.BackColor = System.Drawing.SystemColors.Window;
            tpUpload.Controls.Add(tcUpload);
            resources.ApplyResources(tpUpload, "tpUpload");
            tpUpload.Name = "tpUpload";
            // 
            // tcUpload
            // 
            tcUpload.Controls.Add(tpUploadMain);
            tcUpload.Controls.Add(tpFileNaming);
            tcUpload.Controls.Add(tpUploadClipboard);
            tcUpload.Controls.Add(tpUploaderFilters);
            resources.ApplyResources(tcUpload, "tcUpload");
            tcUpload.Name = "tcUpload";
            tcUpload.SelectedIndex = 0;
            // 
            // tpUploadMain
            // 
            tpUploadMain.BackColor = System.Drawing.SystemColors.Window;
            tpUploadMain.Controls.Add(cbOverrideUploadSettings);
            resources.ApplyResources(tpUploadMain, "tpUploadMain");
            tpUploadMain.Name = "tpUploadMain";
            // 
            // cbOverrideUploadSettings
            // 
            resources.ApplyResources(cbOverrideUploadSettings, "cbOverrideUploadSettings");
            cbOverrideUploadSettings.Checked = true;
            cbOverrideUploadSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideUploadSettings.Name = "cbOverrideUploadSettings";
            cbOverrideUploadSettings.UseVisualStyleBackColor = true;
            cbOverrideUploadSettings.CheckedChanged += cbUseDefaultUploadSettings_CheckedChanged;
            // 
            // tpFileNaming
            // 
            tpFileNaming.BackColor = System.Drawing.SystemColors.Window;
            tpFileNaming.Controls.Add(txtURLRegexReplaceReplacement);
            tpFileNaming.Controls.Add(lblURLRegexReplaceReplacement);
            tpFileNaming.Controls.Add(txtURLRegexReplacePattern);
            tpFileNaming.Controls.Add(lblURLRegexReplacePattern);
            tpFileNaming.Controls.Add(cbURLRegexReplace);
            tpFileNaming.Controls.Add(btnAutoIncrementNumber);
            tpFileNaming.Controls.Add(lblAutoIncrementNumber);
            tpFileNaming.Controls.Add(nudAutoIncrementNumber);
            tpFileNaming.Controls.Add(cbFileUploadReplaceProblematicCharacters);
            tpFileNaming.Controls.Add(cbNameFormatCustomTimeZone);
            tpFileNaming.Controls.Add(lblNameFormatPatternPreview);
            tpFileNaming.Controls.Add(lblNameFormatPatternActiveWindow);
            tpFileNaming.Controls.Add(lblNameFormatPatternPreviewActiveWindow);
            tpFileNaming.Controls.Add(cbNameFormatTimeZone);
            tpFileNaming.Controls.Add(txtNameFormatPatternActiveWindow);
            tpFileNaming.Controls.Add(cbFileUploadUseNamePattern);
            tpFileNaming.Controls.Add(lblNameFormatPattern);
            tpFileNaming.Controls.Add(txtNameFormatPattern);
            resources.ApplyResources(tpFileNaming, "tpFileNaming");
            tpFileNaming.Name = "tpFileNaming";
            // 
            // txtURLRegexReplaceReplacement
            // 
            resources.ApplyResources(txtURLRegexReplaceReplacement, "txtURLRegexReplaceReplacement");
            txtURLRegexReplaceReplacement.Name = "txtURLRegexReplaceReplacement";
            txtURLRegexReplaceReplacement.TextChanged += txtURLRegexReplaceReplacement_TextChanged;
            // 
            // lblURLRegexReplaceReplacement
            // 
            resources.ApplyResources(lblURLRegexReplaceReplacement, "lblURLRegexReplaceReplacement");
            lblURLRegexReplaceReplacement.Name = "lblURLRegexReplaceReplacement";
            // 
            // txtURLRegexReplacePattern
            // 
            resources.ApplyResources(txtURLRegexReplacePattern, "txtURLRegexReplacePattern");
            txtURLRegexReplacePattern.Name = "txtURLRegexReplacePattern";
            txtURLRegexReplacePattern.TextChanged += txtURLRegexReplacePattern_TextChanged;
            // 
            // lblURLRegexReplacePattern
            // 
            resources.ApplyResources(lblURLRegexReplacePattern, "lblURLRegexReplacePattern");
            lblURLRegexReplacePattern.Name = "lblURLRegexReplacePattern";
            // 
            // cbURLRegexReplace
            // 
            resources.ApplyResources(cbURLRegexReplace, "cbURLRegexReplace");
            cbURLRegexReplace.Name = "cbURLRegexReplace";
            cbURLRegexReplace.UseVisualStyleBackColor = true;
            cbURLRegexReplace.CheckedChanged += cbURLRegexReplace_CheckedChanged;
            // 
            // btnAutoIncrementNumber
            // 
            resources.ApplyResources(btnAutoIncrementNumber, "btnAutoIncrementNumber");
            btnAutoIncrementNumber.Name = "btnAutoIncrementNumber";
            btnAutoIncrementNumber.UseVisualStyleBackColor = true;
            btnAutoIncrementNumber.Click += btnAutoIncrementNumber_Click;
            // 
            // lblAutoIncrementNumber
            // 
            resources.ApplyResources(lblAutoIncrementNumber, "lblAutoIncrementNumber");
            lblAutoIncrementNumber.Name = "lblAutoIncrementNumber";
            // 
            // nudAutoIncrementNumber
            // 
            resources.ApplyResources(nudAutoIncrementNumber, "nudAutoIncrementNumber");
            nudAutoIncrementNumber.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            nudAutoIncrementNumber.Name = "nudAutoIncrementNumber";
            // 
            // cbFileUploadReplaceProblematicCharacters
            // 
            resources.ApplyResources(cbFileUploadReplaceProblematicCharacters, "cbFileUploadReplaceProblematicCharacters");
            cbFileUploadReplaceProblematicCharacters.Name = "cbFileUploadReplaceProblematicCharacters";
            cbFileUploadReplaceProblematicCharacters.UseVisualStyleBackColor = true;
            cbFileUploadReplaceProblematicCharacters.CheckedChanged += cbFileUploadReplaceProblematicCharacters_CheckedChanged;
            // 
            // cbNameFormatCustomTimeZone
            // 
            resources.ApplyResources(cbNameFormatCustomTimeZone, "cbNameFormatCustomTimeZone");
            cbNameFormatCustomTimeZone.Name = "cbNameFormatCustomTimeZone";
            cbNameFormatCustomTimeZone.UseVisualStyleBackColor = true;
            cbNameFormatCustomTimeZone.CheckedChanged += cbNameFormatCustomTimeZone_CheckedChanged;
            // 
            // lblNameFormatPatternPreview
            // 
            resources.ApplyResources(lblNameFormatPatternPreview, "lblNameFormatPatternPreview");
            lblNameFormatPatternPreview.Name = "lblNameFormatPatternPreview";
            // 
            // lblNameFormatPatternActiveWindow
            // 
            resources.ApplyResources(lblNameFormatPatternActiveWindow, "lblNameFormatPatternActiveWindow");
            lblNameFormatPatternActiveWindow.Name = "lblNameFormatPatternActiveWindow";
            // 
            // lblNameFormatPatternPreviewActiveWindow
            // 
            resources.ApplyResources(lblNameFormatPatternPreviewActiveWindow, "lblNameFormatPatternPreviewActiveWindow");
            lblNameFormatPatternPreviewActiveWindow.Name = "lblNameFormatPatternPreviewActiveWindow";
            // 
            // cbNameFormatTimeZone
            // 
            cbNameFormatTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbNameFormatTimeZone.FormattingEnabled = true;
            resources.ApplyResources(cbNameFormatTimeZone, "cbNameFormatTimeZone");
            cbNameFormatTimeZone.Name = "cbNameFormatTimeZone";
            cbNameFormatTimeZone.SelectedIndexChanged += cbNameFormatTimeZone_SelectedIndexChanged;
            // 
            // txtNameFormatPatternActiveWindow
            // 
            resources.ApplyResources(txtNameFormatPatternActiveWindow, "txtNameFormatPatternActiveWindow");
            txtNameFormatPatternActiveWindow.Name = "txtNameFormatPatternActiveWindow";
            txtNameFormatPatternActiveWindow.TextChanged += txtNameFormatPatternActiveWindow_TextChanged;
            // 
            // cbFileUploadUseNamePattern
            // 
            resources.ApplyResources(cbFileUploadUseNamePattern, "cbFileUploadUseNamePattern");
            cbFileUploadUseNamePattern.Name = "cbFileUploadUseNamePattern";
            cbFileUploadUseNamePattern.UseVisualStyleBackColor = true;
            cbFileUploadUseNamePattern.CheckedChanged += cbFileUploadUseNamePattern_CheckedChanged;
            // 
            // lblNameFormatPattern
            // 
            resources.ApplyResources(lblNameFormatPattern, "lblNameFormatPattern");
            lblNameFormatPattern.Name = "lblNameFormatPattern";
            // 
            // txtNameFormatPattern
            // 
            resources.ApplyResources(txtNameFormatPattern, "txtNameFormatPattern");
            txtNameFormatPattern.Name = "txtNameFormatPattern";
            txtNameFormatPattern.TextChanged += txtNameFormatPattern_TextChanged;
            // 
            // tpUploadClipboard
            // 
            tpUploadClipboard.BackColor = System.Drawing.SystemColors.Window;
            tpUploadClipboard.Controls.Add(cbClipboardUploadShareURL);
            tpUploadClipboard.Controls.Add(cbClipboardUploadURLContents);
            tpUploadClipboard.Controls.Add(cbClipboardUploadAutoIndexFolder);
            tpUploadClipboard.Controls.Add(cbClipboardUploadShortenURL);
            resources.ApplyResources(tpUploadClipboard, "tpUploadClipboard");
            tpUploadClipboard.Name = "tpUploadClipboard";
            // 
            // cbClipboardUploadShareURL
            // 
            resources.ApplyResources(cbClipboardUploadShareURL, "cbClipboardUploadShareURL");
            cbClipboardUploadShareURL.Name = "cbClipboardUploadShareURL";
            cbClipboardUploadShareURL.UseVisualStyleBackColor = true;
            cbClipboardUploadShareURL.CheckedChanged += cbClipboardUploadShareURL_CheckedChanged;
            // 
            // cbClipboardUploadURLContents
            // 
            resources.ApplyResources(cbClipboardUploadURLContents, "cbClipboardUploadURLContents");
            cbClipboardUploadURLContents.Name = "cbClipboardUploadURLContents";
            cbClipboardUploadURLContents.UseVisualStyleBackColor = true;
            cbClipboardUploadURLContents.CheckedChanged += cbClipboardUploadContents_CheckedChanged;
            // 
            // cbClipboardUploadAutoIndexFolder
            // 
            resources.ApplyResources(cbClipboardUploadAutoIndexFolder, "cbClipboardUploadAutoIndexFolder");
            cbClipboardUploadAutoIndexFolder.Name = "cbClipboardUploadAutoIndexFolder";
            cbClipboardUploadAutoIndexFolder.UseVisualStyleBackColor = true;
            cbClipboardUploadAutoIndexFolder.CheckedChanged += cbClipboardUploadAutoIndexFolder_CheckedChanged;
            // 
            // cbClipboardUploadShortenURL
            // 
            resources.ApplyResources(cbClipboardUploadShortenURL, "cbClipboardUploadShortenURL");
            cbClipboardUploadShortenURL.Name = "cbClipboardUploadShortenURL";
            cbClipboardUploadShortenURL.UseVisualStyleBackColor = true;
            cbClipboardUploadShortenURL.CheckedChanged += cbClipboardUploadAutoDetectURL_CheckedChanged;
            // 
            // tpUploaderFilters
            // 
            tpUploaderFilters.BackColor = System.Drawing.SystemColors.Window;
            tpUploaderFilters.Controls.Add(lvUploaderFiltersList);
            tpUploaderFilters.Controls.Add(btnUploaderFiltersRemove);
            tpUploaderFilters.Controls.Add(btnUploaderFiltersUpdate);
            tpUploaderFilters.Controls.Add(btnUploaderFiltersAdd);
            tpUploaderFilters.Controls.Add(lblUploaderFiltersDestination);
            tpUploaderFilters.Controls.Add(cbUploaderFiltersDestination);
            tpUploaderFilters.Controls.Add(lblUploaderFiltersExtensionsExample);
            tpUploaderFilters.Controls.Add(lblUploaderFiltersExtensions);
            tpUploaderFilters.Controls.Add(txtUploaderFiltersExtensions);
            resources.ApplyResources(tpUploaderFilters, "tpUploaderFilters");
            tpUploaderFilters.Name = "tpUploaderFilters";
            // 
            // lvUploaderFiltersList
            // 
            resources.ApplyResources(lvUploaderFiltersList, "lvUploaderFiltersList");
            lvUploaderFiltersList.AutoFillColumn = true;
            lvUploaderFiltersList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chUploaderFiltersName, chUploaderFiltersExtension });
            lvUploaderFiltersList.FullRowSelect = true;
            lvUploaderFiltersList.Name = "lvUploaderFiltersList";
            lvUploaderFiltersList.UseCompatibleStateImageBehavior = false;
            lvUploaderFiltersList.View = System.Windows.Forms.View.Details;
            lvUploaderFiltersList.SelectedIndexChanged += lvUploaderFiltersList_SelectedIndexChanged;
            // 
            // chUploaderFiltersName
            // 
            resources.ApplyResources(chUploaderFiltersName, "chUploaderFiltersName");
            // 
            // chUploaderFiltersExtension
            // 
            resources.ApplyResources(chUploaderFiltersExtension, "chUploaderFiltersExtension");
            // 
            // btnUploaderFiltersRemove
            // 
            resources.ApplyResources(btnUploaderFiltersRemove, "btnUploaderFiltersRemove");
            btnUploaderFiltersRemove.Name = "btnUploaderFiltersRemove";
            btnUploaderFiltersRemove.UseVisualStyleBackColor = true;
            btnUploaderFiltersRemove.Click += btnUploaderFiltersRemove_Click;
            // 
            // btnUploaderFiltersUpdate
            // 
            resources.ApplyResources(btnUploaderFiltersUpdate, "btnUploaderFiltersUpdate");
            btnUploaderFiltersUpdate.Name = "btnUploaderFiltersUpdate";
            btnUploaderFiltersUpdate.UseVisualStyleBackColor = true;
            btnUploaderFiltersUpdate.Click += btnUploaderFiltersUpdate_Click;
            // 
            // btnUploaderFiltersAdd
            // 
            resources.ApplyResources(btnUploaderFiltersAdd, "btnUploaderFiltersAdd");
            btnUploaderFiltersAdd.Name = "btnUploaderFiltersAdd";
            btnUploaderFiltersAdd.UseVisualStyleBackColor = true;
            btnUploaderFiltersAdd.Click += btnUploaderFiltersAdd_Click;
            // 
            // lblUploaderFiltersDestination
            // 
            resources.ApplyResources(lblUploaderFiltersDestination, "lblUploaderFiltersDestination");
            lblUploaderFiltersDestination.Name = "lblUploaderFiltersDestination";
            // 
            // cbUploaderFiltersDestination
            // 
            cbUploaderFiltersDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbUploaderFiltersDestination.FormattingEnabled = true;
            resources.ApplyResources(cbUploaderFiltersDestination, "cbUploaderFiltersDestination");
            cbUploaderFiltersDestination.Name = "cbUploaderFiltersDestination";
            // 
            // lblUploaderFiltersExtensionsExample
            // 
            resources.ApplyResources(lblUploaderFiltersExtensionsExample, "lblUploaderFiltersExtensionsExample");
            lblUploaderFiltersExtensionsExample.Name = "lblUploaderFiltersExtensionsExample";
            // 
            // lblUploaderFiltersExtensions
            // 
            resources.ApplyResources(lblUploaderFiltersExtensions, "lblUploaderFiltersExtensions");
            lblUploaderFiltersExtensions.Name = "lblUploaderFiltersExtensions";
            // 
            // txtUploaderFiltersExtensions
            // 
            resources.ApplyResources(txtUploaderFiltersExtensions, "txtUploaderFiltersExtensions");
            txtUploaderFiltersExtensions.Name = "txtUploaderFiltersExtensions";
            // 
            // tpActions
            // 
            tpActions.BackColor = System.Drawing.SystemColors.Window;
            tpActions.Controls.Add(pActions);
            tpActions.Controls.Add(cbOverrideActions);
            resources.ApplyResources(tpActions, "tpActions");
            tpActions.Name = "tpActions";
            // 
            // pActions
            // 
            pActions.Controls.Add(btnActions);
            pActions.Controls.Add(lblActionsNote);
            pActions.Controls.Add(btnActionsDuplicate);
            pActions.Controls.Add(btnActionsAdd);
            pActions.Controls.Add(lvActions);
            pActions.Controls.Add(btnActionsEdit);
            pActions.Controls.Add(btnActionsRemove);
            resources.ApplyResources(pActions, "pActions");
            pActions.Name = "pActions";
            // 
            // btnActions
            // 
            resources.ApplyResources(btnActions, "btnActions");
            btnActions.Name = "btnActions";
            btnActions.UseVisualStyleBackColor = true;
            btnActions.Click += btnActions_Click;
            // 
            // lblActionsNote
            // 
            resources.ApplyResources(lblActionsNote, "lblActionsNote");
            lblActionsNote.Name = "lblActionsNote";
            // 
            // btnActionsDuplicate
            // 
            resources.ApplyResources(btnActionsDuplicate, "btnActionsDuplicate");
            btnActionsDuplicate.Name = "btnActionsDuplicate";
            btnActionsDuplicate.UseVisualStyleBackColor = true;
            btnActionsDuplicate.Click += btnActionsDuplicate_Click;
            // 
            // btnActionsAdd
            // 
            resources.ApplyResources(btnActionsAdd, "btnActionsAdd");
            btnActionsAdd.Name = "btnActionsAdd";
            btnActionsAdd.UseVisualStyleBackColor = true;
            btnActionsAdd.Click += btnActionsAdd_Click;
            // 
            // lvActions
            // 
            lvActions.AllowDrop = true;
            lvActions.AllowItemDrag = true;
            resources.ApplyResources(lvActions, "lvActions");
            lvActions.AutoFillColumn = true;
            lvActions.AutoFillColumnIndex = 2;
            lvActions.CheckBoxes = true;
            lvActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chActionsName, chActionsPath, chActionsArgs, chActionsExtensions });
            lvActions.FullRowSelect = true;
            lvActions.MultiSelect = false;
            lvActions.Name = "lvActions";
            lvActions.UseCompatibleStateImageBehavior = false;
            lvActions.View = System.Windows.Forms.View.Details;
            lvActions.ItemMoved += lvActions_ItemMoved;
            lvActions.ItemChecked += lvActions_ItemChecked;
            lvActions.SelectedIndexChanged += lvActions_SelectedIndexChanged;
            // 
            // chActionsName
            // 
            resources.ApplyResources(chActionsName, "chActionsName");
            // 
            // chActionsPath
            // 
            resources.ApplyResources(chActionsPath, "chActionsPath");
            // 
            // chActionsArgs
            // 
            resources.ApplyResources(chActionsArgs, "chActionsArgs");
            // 
            // chActionsExtensions
            // 
            resources.ApplyResources(chActionsExtensions, "chActionsExtensions");
            // 
            // btnActionsEdit
            // 
            resources.ApplyResources(btnActionsEdit, "btnActionsEdit");
            btnActionsEdit.Name = "btnActionsEdit";
            btnActionsEdit.UseVisualStyleBackColor = true;
            btnActionsEdit.Click += btnActionsEdit_Click;
            // 
            // btnActionsRemove
            // 
            resources.ApplyResources(btnActionsRemove, "btnActionsRemove");
            btnActionsRemove.Name = "btnActionsRemove";
            btnActionsRemove.UseVisualStyleBackColor = true;
            btnActionsRemove.Click += btnActionsRemove_Click;
            // 
            // cbOverrideActions
            // 
            resources.ApplyResources(cbOverrideActions, "cbOverrideActions");
            cbOverrideActions.Checked = true;
            cbOverrideActions.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideActions.Name = "cbOverrideActions";
            cbOverrideActions.UseVisualStyleBackColor = true;
            cbOverrideActions.CheckedChanged += cbUseDefaultActions_CheckedChanged;
            // 
            // tpWatchFolders
            // 
            tpWatchFolders.BackColor = System.Drawing.SystemColors.Window;
            tpWatchFolders.Controls.Add(btnWatchFolderEdit);
            tpWatchFolders.Controls.Add(cbWatchFolderEnabled);
            tpWatchFolders.Controls.Add(lvWatchFolderList);
            tpWatchFolders.Controls.Add(btnWatchFolderRemove);
            tpWatchFolders.Controls.Add(btnWatchFolderAdd);
            resources.ApplyResources(tpWatchFolders, "tpWatchFolders");
            tpWatchFolders.Name = "tpWatchFolders";
            // 
            // btnWatchFolderEdit
            // 
            resources.ApplyResources(btnWatchFolderEdit, "btnWatchFolderEdit");
            btnWatchFolderEdit.Name = "btnWatchFolderEdit";
            btnWatchFolderEdit.UseVisualStyleBackColor = true;
            btnWatchFolderEdit.Click += btnWatchFolderEdit_Click;
            // 
            // cbWatchFolderEnabled
            // 
            resources.ApplyResources(cbWatchFolderEnabled, "cbWatchFolderEnabled");
            cbWatchFolderEnabled.Name = "cbWatchFolderEnabled";
            cbWatchFolderEnabled.UseVisualStyleBackColor = true;
            cbWatchFolderEnabled.CheckedChanged += cbWatchFolderEnabled_CheckedChanged;
            // 
            // lvWatchFolderList
            // 
            resources.ApplyResources(lvWatchFolderList, "lvWatchFolderList");
            lvWatchFolderList.AutoFillColumn = true;
            lvWatchFolderList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chWatchFolderFolderPath, chWatchFolderFilter, chWatchFolderIncludeSubdirectories });
            lvWatchFolderList.FullRowSelect = true;
            lvWatchFolderList.Name = "lvWatchFolderList";
            lvWatchFolderList.UseCompatibleStateImageBehavior = false;
            lvWatchFolderList.View = System.Windows.Forms.View.Details;
            lvWatchFolderList.MouseDoubleClick += lvWatchFolderList_MouseDoubleClick;
            // 
            // chWatchFolderFolderPath
            // 
            resources.ApplyResources(chWatchFolderFolderPath, "chWatchFolderFolderPath");
            // 
            // chWatchFolderFilter
            // 
            resources.ApplyResources(chWatchFolderFilter, "chWatchFolderFilter");
            // 
            // chWatchFolderIncludeSubdirectories
            // 
            resources.ApplyResources(chWatchFolderIncludeSubdirectories, "chWatchFolderIncludeSubdirectories");
            // 
            // btnWatchFolderRemove
            // 
            resources.ApplyResources(btnWatchFolderRemove, "btnWatchFolderRemove");
            btnWatchFolderRemove.Name = "btnWatchFolderRemove";
            btnWatchFolderRemove.UseVisualStyleBackColor = true;
            btnWatchFolderRemove.Click += btnWatchFolderRemove_Click;
            // 
            // btnWatchFolderAdd
            // 
            resources.ApplyResources(btnWatchFolderAdd, "btnWatchFolderAdd");
            btnWatchFolderAdd.Name = "btnWatchFolderAdd";
            btnWatchFolderAdd.UseVisualStyleBackColor = true;
            btnWatchFolderAdd.Click += btnWatchFolderAdd_Click;
            // 
            // tpTools
            // 
            tpTools.BackColor = System.Drawing.SystemColors.Window;
            tpTools.Controls.Add(pTools);
            tpTools.Controls.Add(cbOverrideToolsSettings);
            resources.ApplyResources(tpTools, "tpTools");
            tpTools.Name = "tpTools";
            // 
            // pTools
            // 
            pTools.Controls.Add(txtToolsScreenColorPickerFormatCtrl);
            pTools.Controls.Add(lblToolsScreenColorPickerFormatCtrl);
            pTools.Controls.Add(txtToolsScreenColorPickerInfoText);
            pTools.Controls.Add(lblToolsScreenColorPickerInfoText);
            pTools.Controls.Add(txtToolsScreenColorPickerFormat);
            pTools.Controls.Add(lblToolsScreenColorPickerFormat);
            resources.ApplyResources(pTools, "pTools");
            pTools.Name = "pTools";
            // 
            // txtToolsScreenColorPickerFormatCtrl
            // 
            resources.ApplyResources(txtToolsScreenColorPickerFormatCtrl, "txtToolsScreenColorPickerFormatCtrl");
            txtToolsScreenColorPickerFormatCtrl.Name = "txtToolsScreenColorPickerFormatCtrl";
            txtToolsScreenColorPickerFormatCtrl.TextChanged += txtToolsScreenColorPickerFormatCtrl_TextChanged;
            // 
            // lblToolsScreenColorPickerFormatCtrl
            // 
            resources.ApplyResources(lblToolsScreenColorPickerFormatCtrl, "lblToolsScreenColorPickerFormatCtrl");
            lblToolsScreenColorPickerFormatCtrl.Name = "lblToolsScreenColorPickerFormatCtrl";
            // 
            // txtToolsScreenColorPickerInfoText
            // 
            resources.ApplyResources(txtToolsScreenColorPickerInfoText, "txtToolsScreenColorPickerInfoText");
            txtToolsScreenColorPickerInfoText.Name = "txtToolsScreenColorPickerInfoText";
            txtToolsScreenColorPickerInfoText.TextChanged += txtToolsScreenColorPickerInfoText_TextChanged;
            // 
            // lblToolsScreenColorPickerInfoText
            // 
            resources.ApplyResources(lblToolsScreenColorPickerInfoText, "lblToolsScreenColorPickerInfoText");
            lblToolsScreenColorPickerInfoText.Name = "lblToolsScreenColorPickerInfoText";
            // 
            // txtToolsScreenColorPickerFormat
            // 
            resources.ApplyResources(txtToolsScreenColorPickerFormat, "txtToolsScreenColorPickerFormat");
            txtToolsScreenColorPickerFormat.Name = "txtToolsScreenColorPickerFormat";
            txtToolsScreenColorPickerFormat.TextChanged += txtToolsScreenColorPickerFormat_TextChanged;
            // 
            // lblToolsScreenColorPickerFormat
            // 
            resources.ApplyResources(lblToolsScreenColorPickerFormat, "lblToolsScreenColorPickerFormat");
            lblToolsScreenColorPickerFormat.Name = "lblToolsScreenColorPickerFormat";
            // 
            // cbOverrideToolsSettings
            // 
            resources.ApplyResources(cbOverrideToolsSettings, "cbOverrideToolsSettings");
            cbOverrideToolsSettings.Checked = true;
            cbOverrideToolsSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideToolsSettings.Name = "cbOverrideToolsSettings";
            cbOverrideToolsSettings.UseVisualStyleBackColor = true;
            cbOverrideToolsSettings.CheckedChanged += cbUseDefaultToolsSettings_CheckedChanged;
            // 
            // tpAdvanced
            // 
            tpAdvanced.BackColor = System.Drawing.SystemColors.Window;
            tpAdvanced.Controls.Add(pgTaskSettings);
            tpAdvanced.Controls.Add(cbOverrideAdvancedSettings);
            resources.ApplyResources(tpAdvanced, "tpAdvanced");
            tpAdvanced.Name = "tpAdvanced";
            // 
            // pgTaskSettings
            // 
            pgTaskSettings.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(pgTaskSettings, "pgTaskSettings");
            pgTaskSettings.Name = "pgTaskSettings";
            pgTaskSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            pgTaskSettings.ToolbarVisible = false;
            // 
            // cbOverrideAdvancedSettings
            // 
            resources.ApplyResources(cbOverrideAdvancedSettings, "cbOverrideAdvancedSettings");
            cbOverrideAdvancedSettings.Checked = true;
            cbOverrideAdvancedSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            cbOverrideAdvancedSettings.Name = "cbOverrideAdvancedSettings";
            cbOverrideAdvancedSettings.UseVisualStyleBackColor = true;
            cbOverrideAdvancedSettings.CheckedChanged += cbUseDefaultAdvancedSettings_CheckedChanged;
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
            tttvMain.TreeViewSize = 190;
            tttvMain.TabChanged += tttvMain_TabChanged;
            // 
            // TaskSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(tcTaskSettings);
            Controls.Add(tttvMain);
            Name = "TaskSettingsForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Resize += TaskSettingsForm_Resize;
            tcTaskSettings.ResumeLayout(false);
            tpTask.ResumeLayout(false);
            tpTask.PerformLayout();
            cmsDestinations.ResumeLayout(false);
            tpGeneral.ResumeLayout(false);
            tcGeneral.ResumeLayout(false);
            tpGeneralMain.ResumeLayout(false);
            tpGeneralMain.PerformLayout();
            tpNotifications.ResumeLayout(false);
            tpNotifications.PerformLayout();
            gbToastWindow.ResumeLayout(false);
            gbToastWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowSizeHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowSizeWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowFadeDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudToastWindowDuration).EndInit();
            tpImage.ResumeLayout(false);
            tcImage.ResumeLayout(false);
            tpQuality.ResumeLayout(false);
            tpQuality.PerformLayout();
            pImage.ResumeLayout(false);
            pImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudImageAutoUseJPEGSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudImageJPEGQuality).EndInit();
            tpEffects.ResumeLayout(false);
            tpEffects.PerformLayout();
            tpThumbnail.ResumeLayout(false);
            tpThumbnail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailWidth).EndInit();
            tpCapture.ResumeLayout(false);
            tcCapture.ResumeLayout(false);
            tpCaptureGeneral.ResumeLayout(false);
            tpCaptureGeneral.PerformLayout();
            pCapture.ResumeLayout(false);
            pCapture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureCustomRegionX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudScreenshotDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCaptureShadowOffset).EndInit();
            tpRegionCapture.ResumeLayout(false);
            tpRegionCapture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureBackgroundDimStrength).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureFPSLimit).EndInit();
            flpRegionCaptureFixedSize.ResumeLayout(false);
            flpRegionCaptureFixedSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureFixedSizeWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureFixedSizeHeight).EndInit();
            pRegionCaptureSnapSizes.ResumeLayout(false);
            pRegionCaptureSnapSizes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureSnapSizesHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureSnapSizesWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureMagnifierPixelCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRegionCaptureMagnifierPixelSize).EndInit();
            tpScreenRecorder.ResumeLayout(false);
            tpScreenRecorder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudScreenRecordFPS).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudScreenRecorderDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudScreenRecorderStartDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGIFFPS).EndInit();
            tpOCR.ResumeLayout(false);
            tpOCR.PerformLayout();
            tpUpload.ResumeLayout(false);
            tcUpload.ResumeLayout(false);
            tpUploadMain.ResumeLayout(false);
            tpUploadMain.PerformLayout();
            tpFileNaming.ResumeLayout(false);
            tpFileNaming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAutoIncrementNumber).EndInit();
            tpUploadClipboard.ResumeLayout(false);
            tpUploadClipboard.PerformLayout();
            tpUploaderFilters.ResumeLayout(false);
            tpUploaderFilters.PerformLayout();
            tpActions.ResumeLayout(false);
            tpActions.PerformLayout();
            pActions.ResumeLayout(false);
            pActions.PerformLayout();
            tpWatchFolders.ResumeLayout(false);
            tpWatchFolders.PerformLayout();
            tpTools.ResumeLayout(false);
            tpTools.PerformLayout();
            pTools.ResumeLayout(false);
            pTools.PerformLayout();
            tpAdvanced.ResumeLayout(false);
            tpAdvanced.PerformLayout();
            ResumeLayout(false);

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
        private System.Windows.Forms.CheckBox cbCaptureAutoHideDesktopIcons;
    }
}