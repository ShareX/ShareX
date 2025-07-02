namespace ShareX
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            scMain = new ShareX.HelpersLib.SplitContainerCustomSplitter();
            lvUploads = new ShareX.HelpersLib.MyListView();
            chFilename = new System.Windows.Forms.ColumnHeader();
            chStatus = new System.Windows.Forms.ColumnHeader();
            chProgress = new System.Windows.Forms.ColumnHeader();
            chSpeed = new System.Windows.Forms.ColumnHeader();
            chElapsed = new System.Windows.Forms.ColumnHeader();
            chRemaining = new System.Windows.Forms.ColumnHeader();
            chURL = new System.Windows.Forms.ColumnHeader();
            pbPreview = new ShareX.HelpersLib.MyPictureBox();
            tsMain = new ShareX.HelpersLib.ToolStripBorderRight();
            tsddbCapture = new System.Windows.Forms.ToolStripDropDownButton();
            tsmiFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            tsmiWindow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiMonitor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiRectangle = new System.Windows.Forms.ToolStripMenuItem();
            tsmiRectangleLight = new System.Windows.Forms.ToolStripMenuItem();
            tsmiRectangleTransparent = new System.Windows.Forms.ToolStripMenuItem();
            tsmiLastRegion = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenRecordingFFmpeg = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenRecordingGIF = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScrollingCapture = new System.Windows.Forms.ToolStripMenuItem();
            tsmiAutoCapture = new System.Windows.Forms.ToolStripMenuItem();
            tssCapture1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiShowCursor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay0 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay1 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay2 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay3 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay4 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenshotDelay5 = new System.Windows.Forms.ToolStripMenuItem();
            tsddbUpload = new System.Windows.Forms.ToolStripDropDownButton();
            tsmiUploadFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiUploadFolder = new System.Windows.Forms.ToolStripMenuItem();
            tsmiUploadClipboard = new System.Windows.Forms.ToolStripMenuItem();
            tsmiUploadText = new System.Windows.Forms.ToolStripMenuItem();
            tsmiUploadURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiUploadDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            tsmiShortenURL = new System.Windows.Forms.ToolStripMenuItem();
            tsddbWorkflows = new System.Windows.Forms.ToolStripDropDownButton();
            tsddbTools = new System.Windows.Forms.ToolStripDropDownButton();
            tsmiColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            tsmiScreenColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            tsmiRuler = new System.Windows.Forms.ToolStripMenuItem();
            tsmiPinToScreen = new System.Windows.Forms.ToolStripMenuItem();
            tssTools1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiImageBeautifier = new System.Windows.Forms.ToolStripMenuItem();
            tsmiImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            tsmiImageViewer = new System.Windows.Forms.ToolStripMenuItem();
            tsmiImageCombiner = new System.Windows.Forms.ToolStripMenuItem();
            tsmiImageSplitter = new System.Windows.Forms.ToolStripMenuItem();
            tsmiImageThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            tssTools2 = new System.Windows.Forms.ToolStripSeparator();
            tsmiVideoConverter = new System.Windows.Forms.ToolStripMenuItem();
            tsmiVideoThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            tssTools3 = new System.Windows.Forms.ToolStripSeparator();
            tsmiOCR = new System.Windows.Forms.ToolStripMenuItem();
            tsmiQRCode = new System.Windows.Forms.ToolStripMenuItem();
            tsmiHashChecker = new System.Windows.Forms.ToolStripMenuItem();
            tsmiMetadata = new System.Windows.Forms.ToolStripMenuItem();
            tsmiIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            tssTools4 = new System.Windows.Forms.ToolStripSeparator();
            tsmiClipboardViewer = new System.Windows.Forms.ToolStripMenuItem();
            tsmiBorderlessWindow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiInspectWindow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
            tssMain1 = new System.Windows.Forms.ToolStripSeparator();
            tsddbAfterCaptureTasks = new System.Windows.Forms.ToolStripDropDownButton();
            tsddbAfterUploadTasks = new System.Windows.Forms.ToolStripDropDownButton();
            tsddbDestinations = new System.Windows.Forms.ToolStripDropDownButton();
            tsmiImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            tsmiURLSharingServices = new System.Windows.Forms.ToolStripMenuItem();
            tssMain2 = new System.Windows.Forms.ToolStripSeparator();
            tsbApplicationSettings = new System.Windows.Forms.ToolStripButton();
            tsbTaskSettings = new System.Windows.Forms.ToolStripButton();
            tsbHotkeySettings = new System.Windows.Forms.ToolStripButton();
            tsbDestinationSettings = new System.Windows.Forms.ToolStripButton();
            tsbCustomUploaderSettings = new System.Windows.Forms.ToolStripButton();
            tssMain3 = new System.Windows.Forms.ToolStripSeparator();
            tsbScreenshotsFolder = new System.Windows.Forms.ToolStripButton();
            tsbHistory = new System.Windows.Forms.ToolStripButton();
            tsbImageHistory = new System.Windows.Forms.ToolStripButton();
            tssMain4 = new System.Windows.Forms.ToolStripSeparator();
            tsddbDebug = new System.Windows.Forms.ToolStripDropDownButton();
            tsmiShowDebugLog = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTestImageUpload = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTestTextUpload = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTestFileUpload = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTestURLShortener = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTestURLSharing = new System.Windows.Forms.ToolStripMenuItem();
            tsbDonate = new System.Windows.Forms.ToolStripButton();
            tsbX = new System.Windows.Forms.ToolStripButton();
            tsbDiscord = new System.Windows.Forms.ToolStripButton();
            tsbAbout = new System.Windows.Forms.ToolStripButton();
            cmsTaskInfo = new System.Windows.Forms.ContextMenuStrip(components);
            tsmiShowErrors = new System.Windows.Forms.ToolStripMenuItem();
            tsmiStopUpload = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenShortenedURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenThumbnailURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenDeletionURL = new System.Windows.Forms.ToolStripMenuItem();
            tssOpen1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenThumbnailFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyShortenedURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyThumbnailURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyDeletionURL = new System.Windows.Forms.ToolStripMenuItem();
            tssCopy1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiCopyFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyImageDimensions = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyText = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyThumbnailFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyThumbnailImage = new System.Windows.Forms.ToolStripMenuItem();
            tssCopy2 = new System.Windows.Forms.ToolStripSeparator();
            tsmiCopyHTMLLink = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyHTMLImage = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyHTMLLinkedImage = new System.Windows.Forms.ToolStripMenuItem();
            tssCopy3 = new System.Windows.Forms.ToolStripSeparator();
            tsmiCopyForumLink = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyForumImage = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyForumLinkedImage = new System.Windows.Forms.ToolStripMenuItem();
            tssCopy4 = new System.Windows.Forms.ToolStripSeparator();
            tsmiCopyMarkdownLink = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyMarkdownImage = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyMarkdownLinkedImage = new System.Windows.Forms.ToolStripMenuItem();
            tssCopy5 = new System.Windows.Forms.ToolStripSeparator();
            tsmiCopyFilePath = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyFileName = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyFileNameWithExtension = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCopyFolder = new System.Windows.Forms.ToolStripMenuItem();
            tssCopy6 = new System.Windows.Forms.ToolStripSeparator();
            tsmiUploadSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiDownloadSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiBeautifyImage = new System.Windows.Forms.ToolStripMenuItem();
            tsmiAddImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            tsmiPinSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiRunAction = new System.Windows.Forms.ToolStripMenuItem();
            tsmiDeleteSelectedItem = new System.Windows.Forms.ToolStripMenuItem();
            tsmiDeleteSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiShortenSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiShareSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiGoogleLens = new System.Windows.Forms.ToolStripMenuItem();
            tsmiBingVisualSearch = new System.Windows.Forms.ToolStripMenuItem();
            tsmiShowQRCode = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOCRImage = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCombineImages = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCombineImagesHorizontally = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCombineImagesVertically = new System.Windows.Forms.ToolStripMenuItem();
            tsmiShowResponse = new System.Windows.Forms.ToolStripMenuItem();
            tsmiClearList = new System.Windows.Forms.ToolStripMenuItem();
            tssUploadInfo1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiSwitchTaskViewMode = new System.Windows.Forms.ToolStripMenuItem();
            niTray = new System.Windows.Forms.NotifyIcon(components);
            cmsTray = new System.Windows.Forms.ContextMenuStrip(components);
            tsmiTrayCapture = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayWindow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayMonitor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayRectangle = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayRectangleLight = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayRectangleTransparent = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayLastRegion = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenRecordingFFmpeg = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenRecordingGIF = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScrollingCapture = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayAutoCapture = new System.Windows.Forms.ToolStripMenuItem();
            tssTrayCapture1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayShowCursor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay0 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay1 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay2 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay3 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay4 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenshotDelay5 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUpload = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUploadFile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUploadFolder = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUploadClipboard = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUploadText = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUploadURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayUploadDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayShortenURL = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayWorkflows = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayTools = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayScreenColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayRuler = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayPinToScreen = new System.Windows.Forms.ToolStripMenuItem();
            tssTrayTools1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageBeautifier = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageViewer = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageCombiner = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageSplitter = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            tssTrayTools2 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayVideoConverter = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayVideoThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            tssTrayTools3 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayOCR = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayQRCode = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayHashChecker = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayMetadata = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            tssTrayTools4 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayClipboardViewer = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayBorderlessWindow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayInspectWindow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
            tssTray1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayAfterCaptureTasks = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayAfterUploadTasks = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayDestinations = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayURLSharingServices = new System.Windows.Forms.ToolStripMenuItem();
            tssTray2 = new System.Windows.Forms.ToolStripSeparator();
            tsmiTrayApplicationSettings = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayTaskSettings = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayHotkeySettings = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayToggleHotkeys = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayDestinationSettings = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayCustomUploaderSettings = new System.Windows.Forms.ToolStripMenuItem();
            tssTray3 = new System.Windows.Forms.ToolStripSeparator();
            tsmiScreenshotsFolder = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayHistory = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayImageHistory = new System.Windows.Forms.ToolStripMenuItem();
            tssTray4 = new System.Windows.Forms.ToolStripSeparator();
            tsmiRestartAsAdmin = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayRecentItems = new System.Windows.Forms.ToolStripMenuItem();
            tsmiOpenActionsToolbar = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayShow = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            timerTraySingleClick = new System.Windows.Forms.Timer(components);
            ttMain = new System.Windows.Forms.ToolTip(components);
            pToolbars = new System.Windows.Forms.Panel();
            dgvHotkeys = new System.Windows.Forms.DataGridView();
            cHotkeyStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cHotkey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            pMain = new System.Windows.Forms.Panel();
            pHotkeys = new System.Windows.Forms.Panel();
            ucTaskThumbnailView = new TaskThumbnailView();
            ((System.ComponentModel.ISupportInitialize)scMain).BeginInit();
            scMain.Panel1.SuspendLayout();
            scMain.Panel2.SuspendLayout();
            scMain.SuspendLayout();
            tsMain.SuspendLayout();
            cmsTaskInfo.SuspendLayout();
            cmsTray.SuspendLayout();
            pToolbars.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHotkeys).BeginInit();
            pMain.SuspendLayout();
            pHotkeys.SuspendLayout();
            SuspendLayout();
            // 
            // scMain
            // 
            resources.ApplyResources(scMain, "scMain");
            scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            scMain.Panel1.Controls.Add(lvUploads);
            // 
            // scMain.Panel2
            // 
            scMain.Panel2.Controls.Add(pbPreview);
            scMain.SplitterColor = System.Drawing.Color.White;
            scMain.SplitterLineColor = System.Drawing.Color.FromArgb(189, 189, 189);
            scMain.SplitterMoved += scMain_SplitterMoved;
            // 
            // lvUploads
            // 
            lvUploads.AutoFillColumn = true;
            lvUploads.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvUploads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chFilename, chStatus, chProgress, chSpeed, chElapsed, chRemaining, chURL });
            resources.ApplyResources(lvUploads, "lvUploads");
            lvUploads.FullRowSelect = true;
            lvUploads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvUploads.Name = "lvUploads";
            lvUploads.ShowItemToolTips = true;
            lvUploads.UseCompatibleStateImageBehavior = false;
            lvUploads.View = System.Windows.Forms.View.Details;
            lvUploads.ColumnWidthChanged += lvUploads_ColumnWidthChanged;
            lvUploads.ItemDrag += lvUploads_ItemDrag;
            lvUploads.SelectedIndexChanged += lvUploads_SelectedIndexChanged;
            lvUploads.KeyDown += lvUploads_KeyDown;
            lvUploads.MouseDoubleClick += lvUploads_MouseDoubleClick;
            lvUploads.MouseUp += lvUploads_MouseUp;
            // 
            // chFilename
            // 
            resources.ApplyResources(chFilename, "chFilename");
            // 
            // chStatus
            // 
            resources.ApplyResources(chStatus, "chStatus");
            // 
            // chProgress
            // 
            resources.ApplyResources(chProgress, "chProgress");
            // 
            // chSpeed
            // 
            resources.ApplyResources(chSpeed, "chSpeed");
            // 
            // chElapsed
            // 
            resources.ApplyResources(chElapsed, "chElapsed");
            // 
            // chRemaining
            // 
            resources.ApplyResources(chRemaining, "chRemaining");
            // 
            // chURL
            // 
            resources.ApplyResources(chURL, "chURL");
            // 
            // pbPreview
            // 
            pbPreview.BackColor = System.Drawing.SystemColors.Window;
            pbPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(pbPreview, "pbPreview");
            pbPreview.DrawCheckeredBackground = true;
            pbPreview.EnableRightClickMenu = true;
            pbPreview.Name = "pbPreview";
            pbPreview.PictureBoxBackColor = System.Drawing.SystemColors.Control;
            pbPreview.ShowImageSizeLabel = true;
            pbPreview.MouseDown += pbPreview_MouseDown;
            // 
            // tsMain
            // 
            tsMain.CanOverflow = false;
            resources.ApplyResources(tsMain, "tsMain");
            tsMain.DrawCustomBorder = true;
            tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsddbCapture, tsddbUpload, tsddbWorkflows, tsddbTools, tssMain1, tsddbAfterCaptureTasks, tsddbAfterUploadTasks, tsddbDestinations, tssMain2, tsbApplicationSettings, tsbTaskSettings, tsbHotkeySettings, tsbDestinationSettings, tsbCustomUploaderSettings, tssMain3, tsbScreenshotsFolder, tsbHistory, tsbImageHistory, tssMain4, tsddbDebug, tsbDonate, tsbX, tsbDiscord, tsbAbout });
            tsMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            tsMain.Name = "tsMain";
            tsMain.ShowItemToolTips = false;
            tsMain.TabStop = true;
            // 
            // tsddbCapture
            // 
            tsddbCapture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiFullscreen, tsmiWindow, tsmiMonitor, tsmiRectangle, tsmiRectangleLight, tsmiRectangleTransparent, tsmiLastRegion, tsmiScreenRecordingFFmpeg, tsmiScreenRecordingGIF, tsmiScrollingCapture, tsmiAutoCapture, tssCapture1, tsmiShowCursor, tsmiScreenshotDelay });
            tsddbCapture.Image = Properties.Resources.camera;
            resources.ApplyResources(tsddbCapture, "tsddbCapture");
            tsddbCapture.Name = "tsddbCapture";
            tsddbCapture.DropDownOpening += tsddbCapture_DropDownOpening;
            // 
            // tsmiFullscreen
            // 
            tsmiFullscreen.Image = Properties.Resources.layer_fullscreen;
            tsmiFullscreen.Name = "tsmiFullscreen";
            resources.ApplyResources(tsmiFullscreen, "tsmiFullscreen");
            tsmiFullscreen.Click += tsmiFullscreen_Click;
            // 
            // tsmiWindow
            // 
            tsmiWindow.Image = Properties.Resources.application_blue;
            tsmiWindow.Name = "tsmiWindow";
            resources.ApplyResources(tsmiWindow, "tsmiWindow");
            // 
            // tsmiMonitor
            // 
            tsmiMonitor.Image = Properties.Resources.monitor;
            tsmiMonitor.Name = "tsmiMonitor";
            resources.ApplyResources(tsmiMonitor, "tsmiMonitor");
            // 
            // tsmiRectangle
            // 
            tsmiRectangle.Image = Properties.Resources.layer_shape;
            tsmiRectangle.Name = "tsmiRectangle";
            resources.ApplyResources(tsmiRectangle, "tsmiRectangle");
            tsmiRectangle.Click += tsmiRectangle_Click;
            // 
            // tsmiRectangleLight
            // 
            tsmiRectangleLight.Image = Properties.Resources.Rectangle;
            tsmiRectangleLight.Name = "tsmiRectangleLight";
            resources.ApplyResources(tsmiRectangleLight, "tsmiRectangleLight");
            tsmiRectangleLight.Click += tsmiRectangleLight_Click;
            // 
            // tsmiRectangleTransparent
            // 
            tsmiRectangleTransparent.Image = Properties.Resources.layer_transparent;
            tsmiRectangleTransparent.Name = "tsmiRectangleTransparent";
            resources.ApplyResources(tsmiRectangleTransparent, "tsmiRectangleTransparent");
            tsmiRectangleTransparent.Click += tsmiRectangleTransparent_Click;
            // 
            // tsmiLastRegion
            // 
            tsmiLastRegion.Image = Properties.Resources.layers;
            tsmiLastRegion.Name = "tsmiLastRegion";
            resources.ApplyResources(tsmiLastRegion, "tsmiLastRegion");
            tsmiLastRegion.Click += tsmiLastRegion_Click;
            // 
            // tsmiScreenRecordingFFmpeg
            // 
            tsmiScreenRecordingFFmpeg.Image = Properties.Resources.camcorder_image;
            tsmiScreenRecordingFFmpeg.Name = "tsmiScreenRecordingFFmpeg";
            resources.ApplyResources(tsmiScreenRecordingFFmpeg, "tsmiScreenRecordingFFmpeg");
            tsmiScreenRecordingFFmpeg.Click += tsmiScreenRecordingFFmpeg_Click;
            // 
            // tsmiScreenRecordingGIF
            // 
            tsmiScreenRecordingGIF.Image = Properties.Resources.film;
            tsmiScreenRecordingGIF.Name = "tsmiScreenRecordingGIF";
            resources.ApplyResources(tsmiScreenRecordingGIF, "tsmiScreenRecordingGIF");
            tsmiScreenRecordingGIF.Click += tsmiScreenRecordingGIF_Click;
            // 
            // tsmiScrollingCapture
            // 
            tsmiScrollingCapture.Image = Properties.Resources.ui_scroll_pane_image;
            tsmiScrollingCapture.Name = "tsmiScrollingCapture";
            resources.ApplyResources(tsmiScrollingCapture, "tsmiScrollingCapture");
            tsmiScrollingCapture.Click += tsmiScrollingCapture_Click;
            // 
            // tsmiAutoCapture
            // 
            tsmiAutoCapture.Image = Properties.Resources.clock;
            tsmiAutoCapture.Name = "tsmiAutoCapture";
            resources.ApplyResources(tsmiAutoCapture, "tsmiAutoCapture");
            tsmiAutoCapture.Click += tsmiAutoCapture_Click;
            // 
            // tssCapture1
            // 
            tssCapture1.Name = "tssCapture1";
            resources.ApplyResources(tssCapture1, "tssCapture1");
            // 
            // tsmiShowCursor
            // 
            tsmiShowCursor.CheckOnClick = true;
            tsmiShowCursor.Image = Properties.Resources.cursor;
            tsmiShowCursor.Name = "tsmiShowCursor";
            resources.ApplyResources(tsmiShowCursor, "tsmiShowCursor");
            tsmiShowCursor.Click += tsmiShowCursor_Click;
            // 
            // tsmiScreenshotDelay
            // 
            tsmiScreenshotDelay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiScreenshotDelay0, tsmiScreenshotDelay1, tsmiScreenshotDelay2, tsmiScreenshotDelay3, tsmiScreenshotDelay4, tsmiScreenshotDelay5 });
            tsmiScreenshotDelay.Image = Properties.Resources.clock_select;
            tsmiScreenshotDelay.Name = "tsmiScreenshotDelay";
            resources.ApplyResources(tsmiScreenshotDelay, "tsmiScreenshotDelay");
            // 
            // tsmiScreenshotDelay0
            // 
            tsmiScreenshotDelay0.Name = "tsmiScreenshotDelay0";
            resources.ApplyResources(tsmiScreenshotDelay0, "tsmiScreenshotDelay0");
            tsmiScreenshotDelay0.Click += tsmiScreenshotDelay0_Click;
            // 
            // tsmiScreenshotDelay1
            // 
            tsmiScreenshotDelay1.Name = "tsmiScreenshotDelay1";
            resources.ApplyResources(tsmiScreenshotDelay1, "tsmiScreenshotDelay1");
            tsmiScreenshotDelay1.Click += tsmiScreenshotDelay1_Click;
            // 
            // tsmiScreenshotDelay2
            // 
            tsmiScreenshotDelay2.Name = "tsmiScreenshotDelay2";
            resources.ApplyResources(tsmiScreenshotDelay2, "tsmiScreenshotDelay2");
            tsmiScreenshotDelay2.Click += tsmiScreenshotDelay2_Click;
            // 
            // tsmiScreenshotDelay3
            // 
            tsmiScreenshotDelay3.Name = "tsmiScreenshotDelay3";
            resources.ApplyResources(tsmiScreenshotDelay3, "tsmiScreenshotDelay3");
            tsmiScreenshotDelay3.Click += tsmiScreenshotDelay3_Click;
            // 
            // tsmiScreenshotDelay4
            // 
            tsmiScreenshotDelay4.Name = "tsmiScreenshotDelay4";
            resources.ApplyResources(tsmiScreenshotDelay4, "tsmiScreenshotDelay4");
            tsmiScreenshotDelay4.Click += tsmiScreenshotDelay4_Click;
            // 
            // tsmiScreenshotDelay5
            // 
            tsmiScreenshotDelay5.Name = "tsmiScreenshotDelay5";
            resources.ApplyResources(tsmiScreenshotDelay5, "tsmiScreenshotDelay5");
            tsmiScreenshotDelay5.Click += tsmiScreenshotDelay5_Click;
            // 
            // tsddbUpload
            // 
            tsddbUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiUploadFile, tsmiUploadFolder, tsmiUploadClipboard, tsmiUploadText, tsmiUploadURL, tsmiUploadDragDrop, tsmiShortenURL });
            tsddbUpload.Image = Properties.Resources.arrow_090;
            resources.ApplyResources(tsddbUpload, "tsddbUpload");
            tsddbUpload.Name = "tsddbUpload";
            // 
            // tsmiUploadFile
            // 
            tsmiUploadFile.Image = Properties.Resources.folder_open_document;
            tsmiUploadFile.Name = "tsmiUploadFile";
            resources.ApplyResources(tsmiUploadFile, "tsmiUploadFile");
            tsmiUploadFile.Click += tsbFileUpload_Click;
            // 
            // tsmiUploadFolder
            // 
            tsmiUploadFolder.Image = Properties.Resources.folder;
            tsmiUploadFolder.Name = "tsmiUploadFolder";
            resources.ApplyResources(tsmiUploadFolder, "tsmiUploadFolder");
            tsmiUploadFolder.Click += tsmiUploadFolder_Click;
            // 
            // tsmiUploadClipboard
            // 
            tsmiUploadClipboard.Image = Properties.Resources.clipboard;
            tsmiUploadClipboard.Name = "tsmiUploadClipboard";
            resources.ApplyResources(tsmiUploadClipboard, "tsmiUploadClipboard");
            tsmiUploadClipboard.Click += tsbClipboardUpload_Click;
            // 
            // tsmiUploadText
            // 
            tsmiUploadText.Image = Properties.Resources.notebook;
            tsmiUploadText.Name = "tsmiUploadText";
            resources.ApplyResources(tsmiUploadText, "tsmiUploadText");
            tsmiUploadText.Click += tsmiUploadText_Click;
            // 
            // tsmiUploadURL
            // 
            tsmiUploadURL.Image = Properties.Resources.drive;
            tsmiUploadURL.Name = "tsmiUploadURL";
            resources.ApplyResources(tsmiUploadURL, "tsmiUploadURL");
            tsmiUploadURL.Click += tsmiUploadURL_Click;
            // 
            // tsmiUploadDragDrop
            // 
            tsmiUploadDragDrop.Image = Properties.Resources.inbox;
            tsmiUploadDragDrop.Name = "tsmiUploadDragDrop";
            resources.ApplyResources(tsmiUploadDragDrop, "tsmiUploadDragDrop");
            tsmiUploadDragDrop.Click += tsbDragDropUpload_Click;
            // 
            // tsmiShortenURL
            // 
            tsmiShortenURL.Image = Properties.Resources.edit_scale;
            tsmiShortenURL.Name = "tsmiShortenURL";
            resources.ApplyResources(tsmiShortenURL, "tsmiShortenURL");
            tsmiShortenURL.Click += tsmiShortenURL_Click;
            // 
            // tsddbWorkflows
            // 
            tsddbWorkflows.Image = Properties.Resources.categories;
            resources.ApplyResources(tsddbWorkflows, "tsddbWorkflows");
            tsddbWorkflows.Name = "tsddbWorkflows";
            // 
            // tsddbTools
            // 
            tsddbTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiColorPicker, tsmiScreenColorPicker, tsmiRuler, tsmiPinToScreen, tssTools1, tsmiImageEditor, tsmiImageBeautifier, tsmiImageEffects, tsmiImageViewer, tsmiImageCombiner, tsmiImageSplitter, tsmiImageThumbnailer, tssTools2, tsmiVideoConverter, tsmiVideoThumbnailer, tssTools3, tsmiOCR, tsmiQRCode, tsmiHashChecker, tsmiMetadata, tsmiIndexFolder, tssTools4, tsmiClipboardViewer, tsmiBorderlessWindow, tsmiInspectWindow, tsmiMonitorTest });
            tsddbTools.Image = Properties.Resources.toolbox;
            resources.ApplyResources(tsddbTools, "tsddbTools");
            tsddbTools.Name = "tsddbTools";
            // 
            // tsmiColorPicker
            // 
            tsmiColorPicker.Image = Properties.Resources.color;
            tsmiColorPicker.Name = "tsmiColorPicker";
            resources.ApplyResources(tsmiColorPicker, "tsmiColorPicker");
            tsmiColorPicker.Click += tsmiColorPicker_Click;
            // 
            // tsmiScreenColorPicker
            // 
            tsmiScreenColorPicker.Image = Properties.Resources.pipette;
            tsmiScreenColorPicker.Name = "tsmiScreenColorPicker";
            resources.ApplyResources(tsmiScreenColorPicker, "tsmiScreenColorPicker");
            tsmiScreenColorPicker.Click += tsmiScreenColorPicker_Click;
            // 
            // tsmiRuler
            // 
            tsmiRuler.Image = Properties.Resources.ruler_triangle;
            tsmiRuler.Name = "tsmiRuler";
            resources.ApplyResources(tsmiRuler, "tsmiRuler");
            tsmiRuler.Click += tsmiRuler_Click;
            // 
            // tsmiPinToScreen
            // 
            tsmiPinToScreen.Image = Properties.Resources.pin;
            tsmiPinToScreen.Name = "tsmiPinToScreen";
            resources.ApplyResources(tsmiPinToScreen, "tsmiPinToScreen");
            tsmiPinToScreen.Click += tsmiPinToScreen_Click;
            // 
            // tssTools1
            // 
            tssTools1.Name = "tssTools1";
            resources.ApplyResources(tssTools1, "tssTools1");
            // 
            // tsmiImageEditor
            // 
            tsmiImageEditor.Image = Properties.Resources.image_pencil;
            tsmiImageEditor.Name = "tsmiImageEditor";
            resources.ApplyResources(tsmiImageEditor, "tsmiImageEditor");
            tsmiImageEditor.Click += tsmiImageEditor_Click;
            // 
            // tsmiImageBeautifier
            // 
            tsmiImageBeautifier.Image = Properties.Resources.picture_sunset;
            tsmiImageBeautifier.Name = "tsmiImageBeautifier";
            resources.ApplyResources(tsmiImageBeautifier, "tsmiImageBeautifier");
            tsmiImageBeautifier.Click += tsmiImageBeautifier_Click;
            // 
            // tsmiImageEffects
            // 
            tsmiImageEffects.Image = Properties.Resources.image_saturation;
            tsmiImageEffects.Name = "tsmiImageEffects";
            resources.ApplyResources(tsmiImageEffects, "tsmiImageEffects");
            tsmiImageEffects.Click += tsmiImageEffects_Click;
            // 
            // tsmiImageViewer
            // 
            tsmiImageViewer.Image = Properties.Resources.images_flickr;
            tsmiImageViewer.Name = "tsmiImageViewer";
            resources.ApplyResources(tsmiImageViewer, "tsmiImageViewer");
            tsmiImageViewer.Click += tsmiImageViewer_Click;
            // 
            // tsmiImageCombiner
            // 
            tsmiImageCombiner.Image = Properties.Resources.document_break;
            tsmiImageCombiner.Name = "tsmiImageCombiner";
            resources.ApplyResources(tsmiImageCombiner, "tsmiImageCombiner");
            tsmiImageCombiner.Click += tsmiImageCombiner_Click;
            // 
            // tsmiImageSplitter
            // 
            tsmiImageSplitter.Image = Properties.Resources.image_split;
            tsmiImageSplitter.Name = "tsmiImageSplitter";
            resources.ApplyResources(tsmiImageSplitter, "tsmiImageSplitter");
            tsmiImageSplitter.Click += tsmiImageSplitter_Click;
            // 
            // tsmiImageThumbnailer
            // 
            tsmiImageThumbnailer.Image = Properties.Resources.image_resize_actual;
            tsmiImageThumbnailer.Name = "tsmiImageThumbnailer";
            resources.ApplyResources(tsmiImageThumbnailer, "tsmiImageThumbnailer");
            tsmiImageThumbnailer.Click += tsmiImageThumbnailer_Click;
            // 
            // tssTools2
            // 
            tssTools2.Name = "tssTools2";
            resources.ApplyResources(tssTools2, "tssTools2");
            // 
            // tsmiVideoConverter
            // 
            tsmiVideoConverter.Image = Properties.Resources.camcorder_pencil;
            tsmiVideoConverter.Name = "tsmiVideoConverter";
            resources.ApplyResources(tsmiVideoConverter, "tsmiVideoConverter");
            tsmiVideoConverter.Click += tsmiVideoConverter_Click;
            // 
            // tsmiVideoThumbnailer
            // 
            tsmiVideoThumbnailer.Image = Properties.Resources.images_stack;
            tsmiVideoThumbnailer.Name = "tsmiVideoThumbnailer";
            resources.ApplyResources(tsmiVideoThumbnailer, "tsmiVideoThumbnailer");
            tsmiVideoThumbnailer.Click += tsmiVideoThumbnailer_Click;
            // 
            // tssTools3
            // 
            tssTools3.Name = "tssTools3";
            resources.ApplyResources(tssTools3, "tssTools3");
            // 
            // tsmiOCR
            // 
            tsmiOCR.Image = Properties.Resources.edit_drop_cap;
            tsmiOCR.Name = "tsmiOCR";
            resources.ApplyResources(tsmiOCR, "tsmiOCR");
            tsmiOCR.Click += tsmiOCR_Click;
            // 
            // tsmiQRCode
            // 
            tsmiQRCode.Image = Properties.Resources.barcode_2d;
            tsmiQRCode.Name = "tsmiQRCode";
            resources.ApplyResources(tsmiQRCode, "tsmiQRCode");
            tsmiQRCode.Click += tsmiQRCode_Click;
            // 
            // tsmiHashChecker
            // 
            tsmiHashChecker.Image = Properties.Resources.application_task;
            tsmiHashChecker.Name = "tsmiHashChecker";
            resources.ApplyResources(tsmiHashChecker, "tsmiHashChecker");
            tsmiHashChecker.Click += tsmiHashChecker_Click;
            // 
            // tsmiMetadata
            // 
            tsmiMetadata.Image = Properties.Resources.tag_hash;
            tsmiMetadata.Name = "tsmiMetadata";
            resources.ApplyResources(tsmiMetadata, "tsmiMetadata");
            tsmiMetadata.Click += tsmiMetadata_Click;
            // 
            // tsmiIndexFolder
            // 
            tsmiIndexFolder.Image = Properties.Resources.folder_tree;
            tsmiIndexFolder.Name = "tsmiIndexFolder";
            resources.ApplyResources(tsmiIndexFolder, "tsmiIndexFolder");
            tsmiIndexFolder.Click += tsmiIndexFolder_Click;
            // 
            // tssTools4
            // 
            tssTools4.Name = "tssTools4";
            resources.ApplyResources(tssTools4, "tssTools4");
            // 
            // tsmiClipboardViewer
            // 
            tsmiClipboardViewer.Image = Properties.Resources.clipboard_block;
            tsmiClipboardViewer.Name = "tsmiClipboardViewer";
            resources.ApplyResources(tsmiClipboardViewer, "tsmiClipboardViewer");
            tsmiClipboardViewer.Click += tsmiClipboardViewer_Click;
            // 
            // tsmiBorderlessWindow
            // 
            tsmiBorderlessWindow.Image = Properties.Resources.application_resize_full;
            tsmiBorderlessWindow.Name = "tsmiBorderlessWindow";
            resources.ApplyResources(tsmiBorderlessWindow, "tsmiBorderlessWindow");
            tsmiBorderlessWindow.Click += tsmiBorderlessWindow_Click;
            // 
            // tsmiInspectWindow
            // 
            tsmiInspectWindow.Image = Properties.Resources.application_search_result;
            tsmiInspectWindow.Name = "tsmiInspectWindow";
            resources.ApplyResources(tsmiInspectWindow, "tsmiInspectWindow");
            tsmiInspectWindow.Click += tsmiInspectWindow_Click;
            // 
            // tsmiMonitorTest
            // 
            tsmiMonitorTest.Image = Properties.Resources.monitor;
            tsmiMonitorTest.Name = "tsmiMonitorTest";
            resources.ApplyResources(tsmiMonitorTest, "tsmiMonitorTest");
            tsmiMonitorTest.Click += tsmiMonitorTest_Click;
            // 
            // tssMain1
            // 
            tssMain1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            tssMain1.Name = "tssMain1";
            resources.ApplyResources(tssMain1, "tssMain1");
            // 
            // tsddbAfterCaptureTasks
            // 
            tsddbAfterCaptureTasks.Image = Properties.Resources.image_export;
            resources.ApplyResources(tsddbAfterCaptureTasks, "tsddbAfterCaptureTasks");
            tsddbAfterCaptureTasks.Name = "tsddbAfterCaptureTasks";
            // 
            // tsddbAfterUploadTasks
            // 
            tsddbAfterUploadTasks.Image = Properties.Resources.upload_cloud;
            resources.ApplyResources(tsddbAfterUploadTasks, "tsddbAfterUploadTasks");
            tsddbAfterUploadTasks.Name = "tsddbAfterUploadTasks";
            // 
            // tsddbDestinations
            // 
            tsddbDestinations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiImageUploaders, tsmiTextUploaders, tsmiFileUploaders, tsmiURLShorteners, tsmiURLSharingServices });
            tsddbDestinations.Image = Properties.Resources.drive_globe;
            resources.ApplyResources(tsddbDestinations, "tsddbDestinations");
            tsddbDestinations.Name = "tsddbDestinations";
            tsddbDestinations.DropDownOpened += tsddbDestinations_DropDownOpened;
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
            // tssMain2
            // 
            tssMain2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            tssMain2.Name = "tssMain2";
            resources.ApplyResources(tssMain2, "tssMain2");
            // 
            // tsbApplicationSettings
            // 
            tsbApplicationSettings.Image = Properties.Resources.wrench_screwdriver;
            resources.ApplyResources(tsbApplicationSettings, "tsbApplicationSettings");
            tsbApplicationSettings.Name = "tsbApplicationSettings";
            tsbApplicationSettings.Click += tsbApplicationSettings_Click;
            // 
            // tsbTaskSettings
            // 
            tsbTaskSettings.Image = Properties.Resources.gear;
            resources.ApplyResources(tsbTaskSettings, "tsbTaskSettings");
            tsbTaskSettings.Name = "tsbTaskSettings";
            tsbTaskSettings.Click += tsbTaskSettings_Click;
            // 
            // tsbHotkeySettings
            // 
            tsbHotkeySettings.Image = Properties.Resources.keyboard;
            resources.ApplyResources(tsbHotkeySettings, "tsbHotkeySettings");
            tsbHotkeySettings.Name = "tsbHotkeySettings";
            tsbHotkeySettings.Click += tsbHotkeySettings_Click;
            // 
            // tsbDestinationSettings
            // 
            tsbDestinationSettings.Image = Properties.Resources.globe_pencil;
            resources.ApplyResources(tsbDestinationSettings, "tsbDestinationSettings");
            tsbDestinationSettings.Name = "tsbDestinationSettings";
            tsbDestinationSettings.Click += tsbDestinationSettings_Click;
            // 
            // tsbCustomUploaderSettings
            // 
            tsbCustomUploaderSettings.Image = Properties.Resources.network_cloud;
            resources.ApplyResources(tsbCustomUploaderSettings, "tsbCustomUploaderSettings");
            tsbCustomUploaderSettings.Name = "tsbCustomUploaderSettings";
            tsbCustomUploaderSettings.Click += tsbCustomUploaderSettings_Click;
            // 
            // tssMain3
            // 
            tssMain3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            tssMain3.Name = "tssMain3";
            resources.ApplyResources(tssMain3, "tssMain3");
            // 
            // tsbScreenshotsFolder
            // 
            tsbScreenshotsFolder.Image = Properties.Resources.folder_open_image;
            resources.ApplyResources(tsbScreenshotsFolder, "tsbScreenshotsFolder");
            tsbScreenshotsFolder.Name = "tsbScreenshotsFolder";
            tsbScreenshotsFolder.Click += tsbScreenshotsFolder_Click;
            // 
            // tsbHistory
            // 
            tsbHistory.Image = Properties.Resources.application_blog;
            resources.ApplyResources(tsbHistory, "tsbHistory");
            tsbHistory.Name = "tsbHistory";
            tsbHistory.Click += tsbHistory_Click;
            // 
            // tsbImageHistory
            // 
            tsbImageHistory.Image = Properties.Resources.application_icon_large;
            resources.ApplyResources(tsbImageHistory, "tsbImageHistory");
            tsbImageHistory.Name = "tsbImageHistory";
            tsbImageHistory.Click += tsbImageHistory_Click;
            // 
            // tssMain4
            // 
            tssMain4.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            tssMain4.Name = "tssMain4";
            resources.ApplyResources(tssMain4, "tssMain4");
            // 
            // tsddbDebug
            // 
            tsddbDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiShowDebugLog, tsmiTestImageUpload, tsmiTestTextUpload, tsmiTestFileUpload, tsmiTestURLShortener, tsmiTestURLSharing });
            tsddbDebug.Image = Properties.Resources.traffic_cone;
            resources.ApplyResources(tsddbDebug, "tsddbDebug");
            tsddbDebug.Name = "tsddbDebug";
            // 
            // tsmiShowDebugLog
            // 
            tsmiShowDebugLog.Image = Properties.Resources.application_monitor;
            tsmiShowDebugLog.Name = "tsmiShowDebugLog";
            resources.ApplyResources(tsmiShowDebugLog, "tsmiShowDebugLog");
            tsmiShowDebugLog.Click += tsmiShowDebugLog_Click;
            // 
            // tsmiTestImageUpload
            // 
            tsmiTestImageUpload.Image = Properties.Resources.image;
            tsmiTestImageUpload.Name = "tsmiTestImageUpload";
            resources.ApplyResources(tsmiTestImageUpload, "tsmiTestImageUpload");
            tsmiTestImageUpload.Click += tsmiTestImageUpload_Click;
            // 
            // tsmiTestTextUpload
            // 
            tsmiTestTextUpload.Image = Properties.Resources.notebook;
            tsmiTestTextUpload.Name = "tsmiTestTextUpload";
            resources.ApplyResources(tsmiTestTextUpload, "tsmiTestTextUpload");
            tsmiTestTextUpload.Click += tsmiTestTextUpload_Click;
            // 
            // tsmiTestFileUpload
            // 
            tsmiTestFileUpload.Image = Properties.Resources.application_block;
            tsmiTestFileUpload.Name = "tsmiTestFileUpload";
            resources.ApplyResources(tsmiTestFileUpload, "tsmiTestFileUpload");
            tsmiTestFileUpload.Click += tsmiTestFileUpload_Click;
            // 
            // tsmiTestURLShortener
            // 
            tsmiTestURLShortener.Image = Properties.Resources.edit_scale;
            tsmiTestURLShortener.Name = "tsmiTestURLShortener";
            resources.ApplyResources(tsmiTestURLShortener, "tsmiTestURLShortener");
            tsmiTestURLShortener.Click += tsmiTestURLShortener_Click;
            // 
            // tsmiTestURLSharing
            // 
            tsmiTestURLSharing.Image = Properties.Resources.globe_share;
            tsmiTestURLSharing.Name = "tsmiTestURLSharing";
            resources.ApplyResources(tsmiTestURLSharing, "tsmiTestURLSharing");
            tsmiTestURLSharing.Click += tsmiTestURLSharing_Click;
            // 
            // tsbDonate
            // 
            tsbDonate.Image = Properties.Resources.heart;
            resources.ApplyResources(tsbDonate, "tsbDonate");
            tsbDonate.Name = "tsbDonate";
            tsbDonate.Click += tsbDonate_Click;
            // 
            // tsbX
            // 
            tsbX.Image = Properties.Resources.X_black;
            resources.ApplyResources(tsbX, "tsbX");
            tsbX.Name = "tsbX";
            tsbX.Click += tsbX_Click;
            // 
            // tsbDiscord
            // 
            tsbDiscord.Image = Properties.Resources.Discord_black;
            resources.ApplyResources(tsbDiscord, "tsbDiscord");
            tsbDiscord.Name = "tsbDiscord";
            tsbDiscord.Click += tsbDiscord_Click;
            // 
            // tsbAbout
            // 
            tsbAbout.Image = Properties.Resources.crown;
            resources.ApplyResources(tsbAbout, "tsbAbout");
            tsbAbout.Name = "tsbAbout";
            tsbAbout.Click += tsbAbout_Click;
            // 
            // cmsTaskInfo
            // 
            cmsTaskInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiShowErrors, tsmiStopUpload, tsmiOpen, tsmiCopy, tsmiUploadSelectedFile, tsmiDownloadSelectedURL, tsmiEditSelectedFile, tsmiBeautifyImage, tsmiAddImageEffects, tsmiPinSelectedFile, tsmiRunAction, tsmiDeleteSelectedItem, tsmiDeleteSelectedFile, tsmiShortenSelectedURL, tsmiShareSelectedURL, tsmiGoogleLens, tsmiBingVisualSearch, tsmiShowQRCode, tsmiOCRImage, tsmiCombineImages, tsmiShowResponse, tsmiClearList, tssUploadInfo1, tsmiSwitchTaskViewMode });
            cmsTaskInfo.Name = "cmsHistory";
            resources.ApplyResources(cmsTaskInfo, "cmsTaskInfo");
            cmsTaskInfo.Closing += cmsTaskInfo_Closing;
            cmsTaskInfo.PreviewKeyDown += cmsTaskInfo_PreviewKeyDown;
            // 
            // tsmiShowErrors
            // 
            tsmiShowErrors.Image = Properties.Resources.exclamation_button;
            tsmiShowErrors.Name = "tsmiShowErrors";
            resources.ApplyResources(tsmiShowErrors, "tsmiShowErrors");
            tsmiShowErrors.Click += tsmiShowErrors_Click;
            // 
            // tsmiStopUpload
            // 
            tsmiStopUpload.Image = Properties.Resources.cross_button;
            tsmiStopUpload.Name = "tsmiStopUpload";
            resources.ApplyResources(tsmiStopUpload, "tsmiStopUpload");
            tsmiStopUpload.Click += tsmiStopUpload_Click;
            // 
            // tsmiOpen
            // 
            tsmiOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiOpenURL, tsmiOpenShortenedURL, tsmiOpenThumbnailURL, tsmiOpenDeletionURL, tssOpen1, tsmiOpenFile, tsmiOpenFolder, tsmiOpenThumbnailFile });
            tsmiOpen.Image = Properties.Resources.folder_open_document;
            tsmiOpen.Name = "tsmiOpen";
            resources.ApplyResources(tsmiOpen, "tsmiOpen");
            // 
            // tsmiOpenURL
            // 
            tsmiOpenURL.Name = "tsmiOpenURL";
            resources.ApplyResources(tsmiOpenURL, "tsmiOpenURL");
            tsmiOpenURL.Click += tsmiOpenURL_Click;
            // 
            // tsmiOpenShortenedURL
            // 
            tsmiOpenShortenedURL.Name = "tsmiOpenShortenedURL";
            resources.ApplyResources(tsmiOpenShortenedURL, "tsmiOpenShortenedURL");
            tsmiOpenShortenedURL.Click += tsmiOpenShortenedURL_Click;
            // 
            // tsmiOpenThumbnailURL
            // 
            tsmiOpenThumbnailURL.Name = "tsmiOpenThumbnailURL";
            resources.ApplyResources(tsmiOpenThumbnailURL, "tsmiOpenThumbnailURL");
            tsmiOpenThumbnailURL.Click += tsmiOpenThumbnailURL_Click;
            // 
            // tsmiOpenDeletionURL
            // 
            tsmiOpenDeletionURL.Name = "tsmiOpenDeletionURL";
            resources.ApplyResources(tsmiOpenDeletionURL, "tsmiOpenDeletionURL");
            tsmiOpenDeletionURL.Click += tsmiOpenDeletionURL_Click;
            // 
            // tssOpen1
            // 
            tssOpen1.Name = "tssOpen1";
            resources.ApplyResources(tssOpen1, "tssOpen1");
            // 
            // tsmiOpenFile
            // 
            tsmiOpenFile.Name = "tsmiOpenFile";
            resources.ApplyResources(tsmiOpenFile, "tsmiOpenFile");
            tsmiOpenFile.Click += tsmiOpenFile_Click;
            // 
            // tsmiOpenFolder
            // 
            tsmiOpenFolder.Name = "tsmiOpenFolder";
            resources.ApplyResources(tsmiOpenFolder, "tsmiOpenFolder");
            tsmiOpenFolder.Click += tsmiOpenFolder_Click;
            // 
            // tsmiOpenThumbnailFile
            // 
            tsmiOpenThumbnailFile.Name = "tsmiOpenThumbnailFile";
            resources.ApplyResources(tsmiOpenThumbnailFile, "tsmiOpenThumbnailFile");
            tsmiOpenThumbnailFile.Click += tsmiOpenThumbnailFile_Click;
            // 
            // tsmiCopy
            // 
            tsmiCopy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiCopyURL, tsmiCopyShortenedURL, tsmiCopyThumbnailURL, tsmiCopyDeletionURL, tssCopy1, tsmiCopyFile, tsmiCopyImage, tsmiCopyImageDimensions, tsmiCopyText, tsmiCopyThumbnailFile, tsmiCopyThumbnailImage, tssCopy2, tsmiCopyHTMLLink, tsmiCopyHTMLImage, tsmiCopyHTMLLinkedImage, tssCopy3, tsmiCopyForumLink, tsmiCopyForumImage, tsmiCopyForumLinkedImage, tssCopy4, tsmiCopyMarkdownLink, tsmiCopyMarkdownImage, tsmiCopyMarkdownLinkedImage, tssCopy5, tsmiCopyFilePath, tsmiCopyFileName, tsmiCopyFileNameWithExtension, tsmiCopyFolder, tssCopy6 });
            tsmiCopy.Image = Properties.Resources.document_copy;
            tsmiCopy.Name = "tsmiCopy";
            resources.ApplyResources(tsmiCopy, "tsmiCopy");
            // 
            // tsmiCopyURL
            // 
            tsmiCopyURL.Name = "tsmiCopyURL";
            resources.ApplyResources(tsmiCopyURL, "tsmiCopyURL");
            tsmiCopyURL.Click += tsmiCopyURL_Click;
            // 
            // tsmiCopyShortenedURL
            // 
            tsmiCopyShortenedURL.Name = "tsmiCopyShortenedURL";
            resources.ApplyResources(tsmiCopyShortenedURL, "tsmiCopyShortenedURL");
            tsmiCopyShortenedURL.Click += tsmiCopyShortenedURL_Click;
            // 
            // tsmiCopyThumbnailURL
            // 
            tsmiCopyThumbnailURL.Name = "tsmiCopyThumbnailURL";
            resources.ApplyResources(tsmiCopyThumbnailURL, "tsmiCopyThumbnailURL");
            tsmiCopyThumbnailURL.Click += tsmiCopyThumbnailURL_Click;
            // 
            // tsmiCopyDeletionURL
            // 
            tsmiCopyDeletionURL.Name = "tsmiCopyDeletionURL";
            resources.ApplyResources(tsmiCopyDeletionURL, "tsmiCopyDeletionURL");
            tsmiCopyDeletionURL.Click += tsmiCopyDeletionURL_Click;
            // 
            // tssCopy1
            // 
            tssCopy1.Name = "tssCopy1";
            resources.ApplyResources(tssCopy1, "tssCopy1");
            // 
            // tsmiCopyFile
            // 
            tsmiCopyFile.Name = "tsmiCopyFile";
            resources.ApplyResources(tsmiCopyFile, "tsmiCopyFile");
            tsmiCopyFile.Click += tsmiCopyFile_Click;
            // 
            // tsmiCopyImage
            // 
            tsmiCopyImage.Name = "tsmiCopyImage";
            resources.ApplyResources(tsmiCopyImage, "tsmiCopyImage");
            tsmiCopyImage.Click += tsmiCopyImage_Click;
            // 
            // tsmiCopyImageDimensions
            // 
            tsmiCopyImageDimensions.Name = "tsmiCopyImageDimensions";
            resources.ApplyResources(tsmiCopyImageDimensions, "tsmiCopyImageDimensions");
            tsmiCopyImageDimensions.Click += tsmiCopyImageDimensions_Click;
            // 
            // tsmiCopyText
            // 
            tsmiCopyText.Name = "tsmiCopyText";
            resources.ApplyResources(tsmiCopyText, "tsmiCopyText");
            tsmiCopyText.Click += tsmiCopyText_Click;
            // 
            // tsmiCopyThumbnailFile
            // 
            tsmiCopyThumbnailFile.Name = "tsmiCopyThumbnailFile";
            resources.ApplyResources(tsmiCopyThumbnailFile, "tsmiCopyThumbnailFile");
            tsmiCopyThumbnailFile.Click += tsmiCopyThumbnailFile_Click;
            // 
            // tsmiCopyThumbnailImage
            // 
            tsmiCopyThumbnailImage.Name = "tsmiCopyThumbnailImage";
            resources.ApplyResources(tsmiCopyThumbnailImage, "tsmiCopyThumbnailImage");
            tsmiCopyThumbnailImage.Click += tsmiCopyThumbnailImage_Click;
            // 
            // tssCopy2
            // 
            tssCopy2.Name = "tssCopy2";
            resources.ApplyResources(tssCopy2, "tssCopy2");
            // 
            // tsmiCopyHTMLLink
            // 
            tsmiCopyHTMLLink.Name = "tsmiCopyHTMLLink";
            resources.ApplyResources(tsmiCopyHTMLLink, "tsmiCopyHTMLLink");
            tsmiCopyHTMLLink.Click += tsmiCopyHTMLLink_Click;
            // 
            // tsmiCopyHTMLImage
            // 
            tsmiCopyHTMLImage.Name = "tsmiCopyHTMLImage";
            resources.ApplyResources(tsmiCopyHTMLImage, "tsmiCopyHTMLImage");
            tsmiCopyHTMLImage.Click += tsmiCopyHTMLImage_Click;
            // 
            // tsmiCopyHTMLLinkedImage
            // 
            tsmiCopyHTMLLinkedImage.Name = "tsmiCopyHTMLLinkedImage";
            resources.ApplyResources(tsmiCopyHTMLLinkedImage, "tsmiCopyHTMLLinkedImage");
            tsmiCopyHTMLLinkedImage.Click += tsmiCopyHTMLLinkedImage_Click;
            // 
            // tssCopy3
            // 
            tssCopy3.Name = "tssCopy3";
            resources.ApplyResources(tssCopy3, "tssCopy3");
            // 
            // tsmiCopyForumLink
            // 
            tsmiCopyForumLink.Name = "tsmiCopyForumLink";
            resources.ApplyResources(tsmiCopyForumLink, "tsmiCopyForumLink");
            tsmiCopyForumLink.Click += tsmiCopyForumLink_Click;
            // 
            // tsmiCopyForumImage
            // 
            tsmiCopyForumImage.Name = "tsmiCopyForumImage";
            resources.ApplyResources(tsmiCopyForumImage, "tsmiCopyForumImage");
            tsmiCopyForumImage.Click += tsmiCopyForumImage_Click;
            // 
            // tsmiCopyForumLinkedImage
            // 
            tsmiCopyForumLinkedImage.Name = "tsmiCopyForumLinkedImage";
            resources.ApplyResources(tsmiCopyForumLinkedImage, "tsmiCopyForumLinkedImage");
            tsmiCopyForumLinkedImage.Click += tsmiCopyForumLinkedImage_Click;
            // 
            // tssCopy4
            // 
            tssCopy4.Name = "tssCopy4";
            resources.ApplyResources(tssCopy4, "tssCopy4");
            // 
            // tsmiCopyMarkdownLink
            // 
            tsmiCopyMarkdownLink.Name = "tsmiCopyMarkdownLink";
            resources.ApplyResources(tsmiCopyMarkdownLink, "tsmiCopyMarkdownLink");
            tsmiCopyMarkdownLink.Click += tsmiCopyMarkdownLink_Click;
            // 
            // tsmiCopyMarkdownImage
            // 
            tsmiCopyMarkdownImage.Name = "tsmiCopyMarkdownImage";
            resources.ApplyResources(tsmiCopyMarkdownImage, "tsmiCopyMarkdownImage");
            tsmiCopyMarkdownImage.Click += tsmiCopyMarkdownImage_Click;
            // 
            // tsmiCopyMarkdownLinkedImage
            // 
            tsmiCopyMarkdownLinkedImage.Name = "tsmiCopyMarkdownLinkedImage";
            resources.ApplyResources(tsmiCopyMarkdownLinkedImage, "tsmiCopyMarkdownLinkedImage");
            tsmiCopyMarkdownLinkedImage.Click += tsmiCopyMarkdownLinkedImage_Click;
            // 
            // tssCopy5
            // 
            tssCopy5.Name = "tssCopy5";
            resources.ApplyResources(tssCopy5, "tssCopy5");
            // 
            // tsmiCopyFilePath
            // 
            tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            resources.ApplyResources(tsmiCopyFilePath, "tsmiCopyFilePath");
            tsmiCopyFilePath.Click += tsmiCopyFilePath_Click;
            // 
            // tsmiCopyFileName
            // 
            tsmiCopyFileName.Name = "tsmiCopyFileName";
            resources.ApplyResources(tsmiCopyFileName, "tsmiCopyFileName");
            tsmiCopyFileName.Click += tsmiCopyFileName_Click;
            // 
            // tsmiCopyFileNameWithExtension
            // 
            tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            resources.ApplyResources(tsmiCopyFileNameWithExtension, "tsmiCopyFileNameWithExtension");
            tsmiCopyFileNameWithExtension.Click += tsmiCopyFileNameWithExtension_Click;
            // 
            // tsmiCopyFolder
            // 
            tsmiCopyFolder.Name = "tsmiCopyFolder";
            resources.ApplyResources(tsmiCopyFolder, "tsmiCopyFolder");
            tsmiCopyFolder.Click += tsmiCopyFolder_Click;
            // 
            // tssCopy6
            // 
            tssCopy6.Name = "tssCopy6";
            resources.ApplyResources(tssCopy6, "tssCopy6");
            // 
            // tsmiUploadSelectedFile
            // 
            tsmiUploadSelectedFile.Image = Properties.Resources.drive_upload;
            tsmiUploadSelectedFile.Name = "tsmiUploadSelectedFile";
            resources.ApplyResources(tsmiUploadSelectedFile, "tsmiUploadSelectedFile");
            tsmiUploadSelectedFile.Click += tsmiUploadSelectedFile_Click;
            // 
            // tsmiDownloadSelectedURL
            // 
            tsmiDownloadSelectedURL.Image = Properties.Resources.drive_download;
            tsmiDownloadSelectedURL.Name = "tsmiDownloadSelectedURL";
            resources.ApplyResources(tsmiDownloadSelectedURL, "tsmiDownloadSelectedURL");
            tsmiDownloadSelectedURL.Click += tsmiDownloadSelectedURL_Click;
            // 
            // tsmiEditSelectedFile
            // 
            tsmiEditSelectedFile.Image = Properties.Resources.image_pencil;
            tsmiEditSelectedFile.Name = "tsmiEditSelectedFile";
            resources.ApplyResources(tsmiEditSelectedFile, "tsmiEditSelectedFile");
            tsmiEditSelectedFile.Click += tsmiEditSelectedFile_Click;
            // 
            // tsmiBeautifyImage
            // 
            tsmiBeautifyImage.Image = Properties.Resources.picture_sunset;
            tsmiBeautifyImage.Name = "tsmiBeautifyImage";
            resources.ApplyResources(tsmiBeautifyImage, "tsmiBeautifyImage");
            tsmiBeautifyImage.Click += tsmiBeautifyImage_Click;
            // 
            // tsmiAddImageEffects
            // 
            tsmiAddImageEffects.Image = Properties.Resources.image_saturation;
            tsmiAddImageEffects.Name = "tsmiAddImageEffects";
            resources.ApplyResources(tsmiAddImageEffects, "tsmiAddImageEffects");
            tsmiAddImageEffects.Click += tsmiAddImageEffects_Click;
            // 
            // tsmiPinSelectedFile
            // 
            tsmiPinSelectedFile.Image = Properties.Resources.pin;
            tsmiPinSelectedFile.Name = "tsmiPinSelectedFile";
            resources.ApplyResources(tsmiPinSelectedFile, "tsmiPinSelectedFile");
            tsmiPinSelectedFile.Click += tsmiPinSelectedFile_Click;
            // 
            // tsmiRunAction
            // 
            tsmiRunAction.Image = Properties.Resources.application_terminal;
            tsmiRunAction.Name = "tsmiRunAction";
            resources.ApplyResources(tsmiRunAction, "tsmiRunAction");
            // 
            // tsmiDeleteSelectedItem
            // 
            tsmiDeleteSelectedItem.Image = Properties.Resources.script__minus;
            tsmiDeleteSelectedItem.Name = "tsmiDeleteSelectedItem";
            resources.ApplyResources(tsmiDeleteSelectedItem, "tsmiDeleteSelectedItem");
            tsmiDeleteSelectedItem.Click += tsmiDeleteSelectedItem_Click;
            // 
            // tsmiDeleteSelectedFile
            // 
            tsmiDeleteSelectedFile.Image = Properties.Resources.bin;
            tsmiDeleteSelectedFile.Name = "tsmiDeleteSelectedFile";
            resources.ApplyResources(tsmiDeleteSelectedFile, "tsmiDeleteSelectedFile");
            tsmiDeleteSelectedFile.Click += tsmiDeleteSelectedFile_Click;
            // 
            // tsmiShortenSelectedURL
            // 
            tsmiShortenSelectedURL.Image = Properties.Resources.edit_scale;
            tsmiShortenSelectedURL.Name = "tsmiShortenSelectedURL";
            resources.ApplyResources(tsmiShortenSelectedURL, "tsmiShortenSelectedURL");
            // 
            // tsmiShareSelectedURL
            // 
            tsmiShareSelectedURL.Image = Properties.Resources.globe_share;
            tsmiShareSelectedURL.Name = "tsmiShareSelectedURL";
            resources.ApplyResources(tsmiShareSelectedURL, "tsmiShareSelectedURL");
            // 
            // tsmiGoogleLens
            // 
            tsmiGoogleLens.Image = Properties.Resources.Google_Lens;
            tsmiGoogleLens.Name = "tsmiGoogleLens";
            resources.ApplyResources(tsmiGoogleLens, "tsmiGoogleLens");
            tsmiGoogleLens.Click += tsmiGoogleLens_Click;
            // 
            // tsmiBingVisualSearch
            // 
            tsmiBingVisualSearch.Image = Properties.Resources.Bing;
            tsmiBingVisualSearch.Name = "tsmiBingVisualSearch";
            resources.ApplyResources(tsmiBingVisualSearch, "tsmiBingVisualSearch");
            tsmiBingVisualSearch.Click += tsmiBingVisualSearch_Click;
            // 
            // tsmiShowQRCode
            // 
            tsmiShowQRCode.Image = Properties.Resources.barcode_2d;
            tsmiShowQRCode.Name = "tsmiShowQRCode";
            resources.ApplyResources(tsmiShowQRCode, "tsmiShowQRCode");
            tsmiShowQRCode.Click += tsmiShowQRCode_Click;
            // 
            // tsmiOCRImage
            // 
            tsmiOCRImage.Image = Properties.Resources.edit_drop_cap;
            tsmiOCRImage.Name = "tsmiOCRImage";
            resources.ApplyResources(tsmiOCRImage, "tsmiOCRImage");
            tsmiOCRImage.Click += tsmiOCRImage_Click;
            // 
            // tsmiCombineImages
            // 
            tsmiCombineImages.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiCombineImagesHorizontally, tsmiCombineImagesVertically });
            tsmiCombineImages.Image = Properties.Resources.document_break;
            tsmiCombineImages.Name = "tsmiCombineImages";
            resources.ApplyResources(tsmiCombineImages, "tsmiCombineImages");
            tsmiCombineImages.Click += tsmiCombineImages_Click;
            // 
            // tsmiCombineImagesHorizontally
            // 
            tsmiCombineImagesHorizontally.Image = Properties.Resources.application_tile_horizontal;
            tsmiCombineImagesHorizontally.Name = "tsmiCombineImagesHorizontally";
            resources.ApplyResources(tsmiCombineImagesHorizontally, "tsmiCombineImagesHorizontally");
            tsmiCombineImagesHorizontally.Click += tsmiCombineImagesHorizontally_Click;
            // 
            // tsmiCombineImagesVertically
            // 
            tsmiCombineImagesVertically.Image = Properties.Resources.application_tile_vertical;
            tsmiCombineImagesVertically.Name = "tsmiCombineImagesVertically";
            resources.ApplyResources(tsmiCombineImagesVertically, "tsmiCombineImagesVertically");
            tsmiCombineImagesVertically.Click += tsmiCombineImagesVertically_Click;
            // 
            // tsmiShowResponse
            // 
            tsmiShowResponse.Image = Properties.Resources.application_browser;
            tsmiShowResponse.Name = "tsmiShowResponse";
            resources.ApplyResources(tsmiShowResponse, "tsmiShowResponse");
            tsmiShowResponse.Click += tsmiShowResponse_Click;
            // 
            // tsmiClearList
            // 
            tsmiClearList.Image = Properties.Resources.eraser;
            tsmiClearList.Name = "tsmiClearList";
            resources.ApplyResources(tsmiClearList, "tsmiClearList");
            tsmiClearList.Click += tsmiClearList_Click;
            // 
            // tssUploadInfo1
            // 
            tssUploadInfo1.Name = "tssUploadInfo1";
            resources.ApplyResources(tssUploadInfo1, "tssUploadInfo1");
            // 
            // tsmiSwitchTaskViewMode
            // 
            tsmiSwitchTaskViewMode.Name = "tsmiSwitchTaskViewMode";
            resources.ApplyResources(tsmiSwitchTaskViewMode, "tsmiSwitchTaskViewMode");
            tsmiSwitchTaskViewMode.Click += TsmiSwitchTaskViewMode_Click;
            // 
            // niTray
            // 
            niTray.ContextMenuStrip = cmsTray;
            resources.ApplyResources(niTray, "niTray");
            niTray.BalloonTipClicked += niTray_BalloonTipClicked;
            niTray.MouseUp += niTray_MouseUp;
            // 
            // cmsTray
            // 
            cmsTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiTrayCapture, tsmiTrayUpload, tsmiTrayWorkflows, tsmiTrayTools, tssTray1, tsmiTrayAfterCaptureTasks, tsmiTrayAfterUploadTasks, tsmiTrayDestinations, tssTray2, tsmiTrayApplicationSettings, tsmiTrayTaskSettings, tsmiTrayHotkeySettings, tsmiTrayToggleHotkeys, tsmiTrayDestinationSettings, tsmiTrayCustomUploaderSettings, tssTray3, tsmiScreenshotsFolder, tsmiTrayHistory, tsmiTrayImageHistory, tssTray4, tsmiRestartAsAdmin, tsmiTrayRecentItems, tsmiOpenActionsToolbar, tsmiTrayShow, tsmiTrayExit });
            cmsTray.Name = "cmsTray";
            resources.ApplyResources(cmsTray, "cmsTray");
            cmsTray.Closed += cmsTray_Closed;
            cmsTray.Opened += cmsTray_Opened;
            // 
            // tsmiTrayCapture
            // 
            tsmiTrayCapture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiTrayFullscreen, tsmiTrayWindow, tsmiTrayMonitor, tsmiTrayRectangle, tsmiTrayRectangleLight, tsmiTrayRectangleTransparent, tsmiTrayLastRegion, tsmiTrayScreenRecordingFFmpeg, tsmiTrayScreenRecordingGIF, tsmiTrayScrollingCapture, tsmiTrayAutoCapture, tssTrayCapture1, tsmiTrayShowCursor, tsmiTrayScreenshotDelay });
            tsmiTrayCapture.Image = Properties.Resources.camera;
            tsmiTrayCapture.Name = "tsmiTrayCapture";
            resources.ApplyResources(tsmiTrayCapture, "tsmiTrayCapture");
            tsmiTrayCapture.DropDownOpening += tsmiCapture_DropDownOpening;
            // 
            // tsmiTrayFullscreen
            // 
            tsmiTrayFullscreen.Image = Properties.Resources.layer_fullscreen;
            tsmiTrayFullscreen.Name = "tsmiTrayFullscreen";
            resources.ApplyResources(tsmiTrayFullscreen, "tsmiTrayFullscreen");
            tsmiTrayFullscreen.Click += tsmiTrayFullscreen_Click;
            // 
            // tsmiTrayWindow
            // 
            tsmiTrayWindow.Image = Properties.Resources.application_blue;
            tsmiTrayWindow.Name = "tsmiTrayWindow";
            resources.ApplyResources(tsmiTrayWindow, "tsmiTrayWindow");
            // 
            // tsmiTrayMonitor
            // 
            tsmiTrayMonitor.Image = Properties.Resources.monitor;
            tsmiTrayMonitor.Name = "tsmiTrayMonitor";
            resources.ApplyResources(tsmiTrayMonitor, "tsmiTrayMonitor");
            // 
            // tsmiTrayRectangle
            // 
            tsmiTrayRectangle.Image = Properties.Resources.layer_shape;
            tsmiTrayRectangle.Name = "tsmiTrayRectangle";
            resources.ApplyResources(tsmiTrayRectangle, "tsmiTrayRectangle");
            tsmiTrayRectangle.Click += tsmiTrayRectangle_Click;
            // 
            // tsmiTrayRectangleLight
            // 
            tsmiTrayRectangleLight.Image = Properties.Resources.Rectangle;
            tsmiTrayRectangleLight.Name = "tsmiTrayRectangleLight";
            resources.ApplyResources(tsmiTrayRectangleLight, "tsmiTrayRectangleLight");
            tsmiTrayRectangleLight.Click += tsmiTrayRectangleLight_Click;
            // 
            // tsmiTrayRectangleTransparent
            // 
            tsmiTrayRectangleTransparent.Image = Properties.Resources.layer_transparent;
            tsmiTrayRectangleTransparent.Name = "tsmiTrayRectangleTransparent";
            resources.ApplyResources(tsmiTrayRectangleTransparent, "tsmiTrayRectangleTransparent");
            tsmiTrayRectangleTransparent.Click += tsmiTrayRectangleTransparent_Click;
            // 
            // tsmiTrayLastRegion
            // 
            tsmiTrayLastRegion.Image = Properties.Resources.layers;
            tsmiTrayLastRegion.Name = "tsmiTrayLastRegion";
            resources.ApplyResources(tsmiTrayLastRegion, "tsmiTrayLastRegion");
            tsmiTrayLastRegion.Click += tsmiTrayLastRegion_Click;
            // 
            // tsmiTrayScreenRecordingFFmpeg
            // 
            tsmiTrayScreenRecordingFFmpeg.Image = Properties.Resources.camcorder_image;
            tsmiTrayScreenRecordingFFmpeg.Name = "tsmiTrayScreenRecordingFFmpeg";
            resources.ApplyResources(tsmiTrayScreenRecordingFFmpeg, "tsmiTrayScreenRecordingFFmpeg");
            tsmiTrayScreenRecordingFFmpeg.Click += tsmiScreenRecordingFFmpeg_Click;
            // 
            // tsmiTrayScreenRecordingGIF
            // 
            tsmiTrayScreenRecordingGIF.Image = Properties.Resources.film;
            tsmiTrayScreenRecordingGIF.Name = "tsmiTrayScreenRecordingGIF";
            resources.ApplyResources(tsmiTrayScreenRecordingGIF, "tsmiTrayScreenRecordingGIF");
            tsmiTrayScreenRecordingGIF.Click += tsmiScreenRecordingGIF_Click;
            // 
            // tsmiTrayScrollingCapture
            // 
            tsmiTrayScrollingCapture.Image = Properties.Resources.ui_scroll_pane_image;
            tsmiTrayScrollingCapture.Name = "tsmiTrayScrollingCapture";
            resources.ApplyResources(tsmiTrayScrollingCapture, "tsmiTrayScrollingCapture");
            tsmiTrayScrollingCapture.Click += tsmiScrollingCapture_Click;
            // 
            // tsmiTrayAutoCapture
            // 
            tsmiTrayAutoCapture.Image = Properties.Resources.clock;
            tsmiTrayAutoCapture.Name = "tsmiTrayAutoCapture";
            resources.ApplyResources(tsmiTrayAutoCapture, "tsmiTrayAutoCapture");
            tsmiTrayAutoCapture.Click += tsmiAutoCapture_Click;
            // 
            // tssTrayCapture1
            // 
            tssTrayCapture1.Name = "tssTrayCapture1";
            resources.ApplyResources(tssTrayCapture1, "tssTrayCapture1");
            // 
            // tsmiTrayShowCursor
            // 
            tsmiTrayShowCursor.CheckOnClick = true;
            tsmiTrayShowCursor.Image = Properties.Resources.cursor;
            tsmiTrayShowCursor.Name = "tsmiTrayShowCursor";
            resources.ApplyResources(tsmiTrayShowCursor, "tsmiTrayShowCursor");
            tsmiTrayShowCursor.Click += tsmiShowCursor_Click;
            // 
            // tsmiTrayScreenshotDelay
            // 
            tsmiTrayScreenshotDelay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiTrayScreenshotDelay0, tsmiTrayScreenshotDelay1, tsmiTrayScreenshotDelay2, tsmiTrayScreenshotDelay3, tsmiTrayScreenshotDelay4, tsmiTrayScreenshotDelay5 });
            tsmiTrayScreenshotDelay.Image = Properties.Resources.clock_select;
            tsmiTrayScreenshotDelay.Name = "tsmiTrayScreenshotDelay";
            resources.ApplyResources(tsmiTrayScreenshotDelay, "tsmiTrayScreenshotDelay");
            // 
            // tsmiTrayScreenshotDelay0
            // 
            tsmiTrayScreenshotDelay0.Name = "tsmiTrayScreenshotDelay0";
            resources.ApplyResources(tsmiTrayScreenshotDelay0, "tsmiTrayScreenshotDelay0");
            tsmiTrayScreenshotDelay0.Click += tsmiScreenshotDelay0_Click;
            // 
            // tsmiTrayScreenshotDelay1
            // 
            tsmiTrayScreenshotDelay1.Name = "tsmiTrayScreenshotDelay1";
            resources.ApplyResources(tsmiTrayScreenshotDelay1, "tsmiTrayScreenshotDelay1");
            tsmiTrayScreenshotDelay1.Click += tsmiScreenshotDelay1_Click;
            // 
            // tsmiTrayScreenshotDelay2
            // 
            tsmiTrayScreenshotDelay2.Name = "tsmiTrayScreenshotDelay2";
            resources.ApplyResources(tsmiTrayScreenshotDelay2, "tsmiTrayScreenshotDelay2");
            tsmiTrayScreenshotDelay2.Click += tsmiScreenshotDelay2_Click;
            // 
            // tsmiTrayScreenshotDelay3
            // 
            tsmiTrayScreenshotDelay3.Name = "tsmiTrayScreenshotDelay3";
            resources.ApplyResources(tsmiTrayScreenshotDelay3, "tsmiTrayScreenshotDelay3");
            tsmiTrayScreenshotDelay3.Click += tsmiScreenshotDelay3_Click;
            // 
            // tsmiTrayScreenshotDelay4
            // 
            tsmiTrayScreenshotDelay4.Name = "tsmiTrayScreenshotDelay4";
            resources.ApplyResources(tsmiTrayScreenshotDelay4, "tsmiTrayScreenshotDelay4");
            tsmiTrayScreenshotDelay4.Click += tsmiScreenshotDelay4_Click;
            // 
            // tsmiTrayScreenshotDelay5
            // 
            tsmiTrayScreenshotDelay5.Name = "tsmiTrayScreenshotDelay5";
            resources.ApplyResources(tsmiTrayScreenshotDelay5, "tsmiTrayScreenshotDelay5");
            tsmiTrayScreenshotDelay5.Click += tsmiScreenshotDelay5_Click;
            // 
            // tsmiTrayUpload
            // 
            tsmiTrayUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiTrayUploadFile, tsmiTrayUploadFolder, tsmiTrayUploadClipboard, tsmiTrayUploadText, tsmiTrayUploadURL, tsmiTrayUploadDragDrop, tsmiTrayShortenURL });
            tsmiTrayUpload.Image = Properties.Resources.arrow_090;
            tsmiTrayUpload.Name = "tsmiTrayUpload";
            resources.ApplyResources(tsmiTrayUpload, "tsmiTrayUpload");
            // 
            // tsmiTrayUploadFile
            // 
            tsmiTrayUploadFile.Image = Properties.Resources.folder_open_document;
            tsmiTrayUploadFile.Name = "tsmiTrayUploadFile";
            resources.ApplyResources(tsmiTrayUploadFile, "tsmiTrayUploadFile");
            tsmiTrayUploadFile.Click += tsbFileUpload_Click;
            // 
            // tsmiTrayUploadFolder
            // 
            tsmiTrayUploadFolder.Image = Properties.Resources.folder;
            tsmiTrayUploadFolder.Name = "tsmiTrayUploadFolder";
            resources.ApplyResources(tsmiTrayUploadFolder, "tsmiTrayUploadFolder");
            tsmiTrayUploadFolder.Click += tsmiUploadFolder_Click;
            // 
            // tsmiTrayUploadClipboard
            // 
            tsmiTrayUploadClipboard.Image = Properties.Resources.clipboard;
            tsmiTrayUploadClipboard.Name = "tsmiTrayUploadClipboard";
            resources.ApplyResources(tsmiTrayUploadClipboard, "tsmiTrayUploadClipboard");
            tsmiTrayUploadClipboard.Click += tsbClipboardUpload_Click;
            // 
            // tsmiTrayUploadText
            // 
            tsmiTrayUploadText.Image = Properties.Resources.notebook;
            tsmiTrayUploadText.Name = "tsmiTrayUploadText";
            resources.ApplyResources(tsmiTrayUploadText, "tsmiTrayUploadText");
            tsmiTrayUploadText.Click += tsmiUploadText_Click;
            // 
            // tsmiTrayUploadURL
            // 
            tsmiTrayUploadURL.Image = Properties.Resources.drive;
            tsmiTrayUploadURL.Name = "tsmiTrayUploadURL";
            resources.ApplyResources(tsmiTrayUploadURL, "tsmiTrayUploadURL");
            tsmiTrayUploadURL.Click += tsmiUploadURL_Click;
            // 
            // tsmiTrayUploadDragDrop
            // 
            tsmiTrayUploadDragDrop.Image = Properties.Resources.inbox;
            tsmiTrayUploadDragDrop.Name = "tsmiTrayUploadDragDrop";
            resources.ApplyResources(tsmiTrayUploadDragDrop, "tsmiTrayUploadDragDrop");
            tsmiTrayUploadDragDrop.Click += tsbDragDropUpload_Click;
            // 
            // tsmiTrayShortenURL
            // 
            tsmiTrayShortenURL.Image = Properties.Resources.edit_scale;
            tsmiTrayShortenURL.Name = "tsmiTrayShortenURL";
            resources.ApplyResources(tsmiTrayShortenURL, "tsmiTrayShortenURL");
            tsmiTrayShortenURL.Click += tsmiShortenURL_Click;
            // 
            // tsmiTrayWorkflows
            // 
            tsmiTrayWorkflows.Image = Properties.Resources.categories;
            tsmiTrayWorkflows.Name = "tsmiTrayWorkflows";
            resources.ApplyResources(tsmiTrayWorkflows, "tsmiTrayWorkflows");
            // 
            // tsmiTrayTools
            // 
            tsmiTrayTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiTrayColorPicker, tsmiTrayScreenColorPicker, tsmiTrayRuler, tsmiTrayPinToScreen, tssTrayTools1, tsmiTrayImageEditor, tsmiTrayImageBeautifier, tsmiTrayImageEffects, tsmiTrayImageViewer, tsmiTrayImageCombiner, tsmiTrayImageSplitter, tsmiTrayImageThumbnailer, tssTrayTools2, tsmiTrayVideoConverter, tsmiTrayVideoThumbnailer, tssTrayTools3, tsmiTrayOCR, tsmiTrayQRCode, tsmiTrayHashChecker, tsmiTrayMetadata, tsmiTrayIndexFolder, tssTrayTools4, tsmiTrayClipboardViewer, tsmiTrayBorderlessWindow, tsmiTrayInspectWindow, tsmiTrayMonitorTest });
            tsmiTrayTools.Image = Properties.Resources.toolbox;
            tsmiTrayTools.Name = "tsmiTrayTools";
            resources.ApplyResources(tsmiTrayTools, "tsmiTrayTools");
            // 
            // tsmiTrayColorPicker
            // 
            tsmiTrayColorPicker.Image = Properties.Resources.color;
            tsmiTrayColorPicker.Name = "tsmiTrayColorPicker";
            resources.ApplyResources(tsmiTrayColorPicker, "tsmiTrayColorPicker");
            tsmiTrayColorPicker.Click += tsmiColorPicker_Click;
            // 
            // tsmiTrayScreenColorPicker
            // 
            tsmiTrayScreenColorPicker.Image = Properties.Resources.pipette;
            tsmiTrayScreenColorPicker.Name = "tsmiTrayScreenColorPicker";
            resources.ApplyResources(tsmiTrayScreenColorPicker, "tsmiTrayScreenColorPicker");
            tsmiTrayScreenColorPicker.Click += tsmiScreenColorPicker_Click;
            // 
            // tsmiTrayRuler
            // 
            tsmiTrayRuler.Image = Properties.Resources.ruler_triangle;
            tsmiTrayRuler.Name = "tsmiTrayRuler";
            resources.ApplyResources(tsmiTrayRuler, "tsmiTrayRuler");
            tsmiTrayRuler.Click += tsmiRuler_Click;
            // 
            // tsmiTrayPinToScreen
            // 
            tsmiTrayPinToScreen.Image = Properties.Resources.pin;
            tsmiTrayPinToScreen.Name = "tsmiTrayPinToScreen";
            resources.ApplyResources(tsmiTrayPinToScreen, "tsmiTrayPinToScreen");
            tsmiTrayPinToScreen.Click += tsmiPinToScreen_Click;
            // 
            // tssTrayTools1
            // 
            tssTrayTools1.Name = "tssTrayTools1";
            resources.ApplyResources(tssTrayTools1, "tssTrayTools1");
            // 
            // tsmiTrayImageEditor
            // 
            tsmiTrayImageEditor.Image = Properties.Resources.image_pencil;
            tsmiTrayImageEditor.Name = "tsmiTrayImageEditor";
            resources.ApplyResources(tsmiTrayImageEditor, "tsmiTrayImageEditor");
            tsmiTrayImageEditor.Click += tsmiImageEditor_Click;
            // 
            // tsmiTrayImageBeautifier
            // 
            tsmiTrayImageBeautifier.Image = Properties.Resources.picture_sunset;
            tsmiTrayImageBeautifier.Name = "tsmiTrayImageBeautifier";
            resources.ApplyResources(tsmiTrayImageBeautifier, "tsmiTrayImageBeautifier");
            tsmiTrayImageBeautifier.Click += tsmiImageBeautifier_Click;
            // 
            // tsmiTrayImageEffects
            // 
            tsmiTrayImageEffects.Image = Properties.Resources.image_saturation;
            tsmiTrayImageEffects.Name = "tsmiTrayImageEffects";
            resources.ApplyResources(tsmiTrayImageEffects, "tsmiTrayImageEffects");
            tsmiTrayImageEffects.Click += tsmiImageEffects_Click;
            // 
            // tsmiTrayImageViewer
            // 
            tsmiTrayImageViewer.Image = Properties.Resources.images_flickr;
            tsmiTrayImageViewer.Name = "tsmiTrayImageViewer";
            resources.ApplyResources(tsmiTrayImageViewer, "tsmiTrayImageViewer");
            tsmiTrayImageViewer.Click += tsmiImageViewer_Click;
            // 
            // tsmiTrayImageCombiner
            // 
            tsmiTrayImageCombiner.Image = Properties.Resources.document_break;
            tsmiTrayImageCombiner.Name = "tsmiTrayImageCombiner";
            resources.ApplyResources(tsmiTrayImageCombiner, "tsmiTrayImageCombiner");
            tsmiTrayImageCombiner.Click += tsmiImageCombiner_Click;
            // 
            // tsmiTrayImageSplitter
            // 
            tsmiTrayImageSplitter.Image = Properties.Resources.image_split;
            tsmiTrayImageSplitter.Name = "tsmiTrayImageSplitter";
            resources.ApplyResources(tsmiTrayImageSplitter, "tsmiTrayImageSplitter");
            tsmiTrayImageSplitter.Click += tsmiImageSplitter_Click;
            // 
            // tsmiTrayImageThumbnailer
            // 
            tsmiTrayImageThumbnailer.Image = Properties.Resources.image_resize_actual;
            tsmiTrayImageThumbnailer.Name = "tsmiTrayImageThumbnailer";
            resources.ApplyResources(tsmiTrayImageThumbnailer, "tsmiTrayImageThumbnailer");
            tsmiTrayImageThumbnailer.Click += tsmiImageThumbnailer_Click;
            // 
            // tssTrayTools2
            // 
            tssTrayTools2.Name = "tssTrayTools2";
            resources.ApplyResources(tssTrayTools2, "tssTrayTools2");
            // 
            // tsmiTrayVideoConverter
            // 
            tsmiTrayVideoConverter.Image = Properties.Resources.camcorder_pencil;
            tsmiTrayVideoConverter.Name = "tsmiTrayVideoConverter";
            resources.ApplyResources(tsmiTrayVideoConverter, "tsmiTrayVideoConverter");
            tsmiTrayVideoConverter.Click += tsmiVideoConverter_Click;
            // 
            // tsmiTrayVideoThumbnailer
            // 
            tsmiTrayVideoThumbnailer.Image = Properties.Resources.images_stack;
            tsmiTrayVideoThumbnailer.Name = "tsmiTrayVideoThumbnailer";
            resources.ApplyResources(tsmiTrayVideoThumbnailer, "tsmiTrayVideoThumbnailer");
            tsmiTrayVideoThumbnailer.Click += tsmiVideoThumbnailer_Click;
            // 
            // tssTrayTools3
            // 
            tssTrayTools3.Name = "tssTrayTools3";
            resources.ApplyResources(tssTrayTools3, "tssTrayTools3");
            // 
            // tsmiTrayOCR
            // 
            tsmiTrayOCR.Image = Properties.Resources.edit_drop_cap;
            tsmiTrayOCR.Name = "tsmiTrayOCR";
            resources.ApplyResources(tsmiTrayOCR, "tsmiTrayOCR");
            tsmiTrayOCR.Click += tsmiTrayOCR_Click;
            // 
            // tsmiTrayQRCode
            // 
            tsmiTrayQRCode.Image = Properties.Resources.barcode_2d;
            tsmiTrayQRCode.Name = "tsmiTrayQRCode";
            resources.ApplyResources(tsmiTrayQRCode, "tsmiTrayQRCode");
            tsmiTrayQRCode.Click += tsmiQRCode_Click;
            // 
            // tsmiTrayHashChecker
            // 
            tsmiTrayHashChecker.Image = Properties.Resources.application_task;
            tsmiTrayHashChecker.Name = "tsmiTrayHashChecker";
            resources.ApplyResources(tsmiTrayHashChecker, "tsmiTrayHashChecker");
            tsmiTrayHashChecker.Click += tsmiHashChecker_Click;
            // 
            // tsmiTrayMetadata
            // 
            tsmiTrayMetadata.Image = Properties.Resources.tag_hash;
            tsmiTrayMetadata.Name = "tsmiTrayMetadata";
            resources.ApplyResources(tsmiTrayMetadata, "tsmiTrayMetadata");
            tsmiTrayMetadata.Click += tsmiMetadata_Click;
            // 
            // tsmiTrayIndexFolder
            // 
            tsmiTrayIndexFolder.Image = Properties.Resources.folder_tree;
            tsmiTrayIndexFolder.Name = "tsmiTrayIndexFolder";
            resources.ApplyResources(tsmiTrayIndexFolder, "tsmiTrayIndexFolder");
            tsmiTrayIndexFolder.Click += tsmiIndexFolder_Click;
            // 
            // tssTrayTools4
            // 
            tssTrayTools4.Name = "tssTrayTools4";
            resources.ApplyResources(tssTrayTools4, "tssTrayTools4");
            // 
            // tsmiTrayClipboardViewer
            // 
            tsmiTrayClipboardViewer.Image = Properties.Resources.clipboard_block;
            tsmiTrayClipboardViewer.Name = "tsmiTrayClipboardViewer";
            resources.ApplyResources(tsmiTrayClipboardViewer, "tsmiTrayClipboardViewer");
            tsmiTrayClipboardViewer.Click += tsmiClipboardViewer_Click;
            // 
            // tsmiTrayBorderlessWindow
            // 
            tsmiTrayBorderlessWindow.Image = Properties.Resources.application_resize_full;
            tsmiTrayBorderlessWindow.Name = "tsmiTrayBorderlessWindow";
            resources.ApplyResources(tsmiTrayBorderlessWindow, "tsmiTrayBorderlessWindow");
            tsmiTrayBorderlessWindow.Click += tsmiBorderlessWindow_Click;
            // 
            // tsmiTrayInspectWindow
            // 
            tsmiTrayInspectWindow.Image = Properties.Resources.application_search_result;
            tsmiTrayInspectWindow.Name = "tsmiTrayInspectWindow";
            resources.ApplyResources(tsmiTrayInspectWindow, "tsmiTrayInspectWindow");
            tsmiTrayInspectWindow.Click += tsmiInspectWindow_Click;
            // 
            // tsmiTrayMonitorTest
            // 
            tsmiTrayMonitorTest.Image = Properties.Resources.monitor;
            tsmiTrayMonitorTest.Name = "tsmiTrayMonitorTest";
            resources.ApplyResources(tsmiTrayMonitorTest, "tsmiTrayMonitorTest");
            tsmiTrayMonitorTest.Click += tsmiMonitorTest_Click;
            // 
            // tssTray1
            // 
            tssTray1.Name = "tssTray1";
            resources.ApplyResources(tssTray1, "tssTray1");
            // 
            // tsmiTrayAfterCaptureTasks
            // 
            tsmiTrayAfterCaptureTasks.Image = Properties.Resources.image_export;
            tsmiTrayAfterCaptureTasks.Name = "tsmiTrayAfterCaptureTasks";
            resources.ApplyResources(tsmiTrayAfterCaptureTasks, "tsmiTrayAfterCaptureTasks");
            // 
            // tsmiTrayAfterUploadTasks
            // 
            tsmiTrayAfterUploadTasks.Image = Properties.Resources.upload_cloud;
            tsmiTrayAfterUploadTasks.Name = "tsmiTrayAfterUploadTasks";
            resources.ApplyResources(tsmiTrayAfterUploadTasks, "tsmiTrayAfterUploadTasks");
            // 
            // tsmiTrayDestinations
            // 
            tsmiTrayDestinations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiTrayImageUploaders, tsmiTrayTextUploaders, tsmiTrayFileUploaders, tsmiTrayURLShorteners, tsmiTrayURLSharingServices });
            tsmiTrayDestinations.Image = Properties.Resources.drive_globe;
            tsmiTrayDestinations.Name = "tsmiTrayDestinations";
            resources.ApplyResources(tsmiTrayDestinations, "tsmiTrayDestinations");
            tsmiTrayDestinations.DropDownOpened += tsddbDestinations_DropDownOpened;
            // 
            // tsmiTrayImageUploaders
            // 
            tsmiTrayImageUploaders.Image = Properties.Resources.image;
            tsmiTrayImageUploaders.Name = "tsmiTrayImageUploaders";
            resources.ApplyResources(tsmiTrayImageUploaders, "tsmiTrayImageUploaders");
            // 
            // tsmiTrayTextUploaders
            // 
            tsmiTrayTextUploaders.Image = Properties.Resources.notebook;
            tsmiTrayTextUploaders.Name = "tsmiTrayTextUploaders";
            resources.ApplyResources(tsmiTrayTextUploaders, "tsmiTrayTextUploaders");
            // 
            // tsmiTrayFileUploaders
            // 
            tsmiTrayFileUploaders.Image = Properties.Resources.application_block;
            tsmiTrayFileUploaders.Name = "tsmiTrayFileUploaders";
            resources.ApplyResources(tsmiTrayFileUploaders, "tsmiTrayFileUploaders");
            // 
            // tsmiTrayURLShorteners
            // 
            tsmiTrayURLShorteners.Image = Properties.Resources.edit_scale;
            tsmiTrayURLShorteners.Name = "tsmiTrayURLShorteners";
            resources.ApplyResources(tsmiTrayURLShorteners, "tsmiTrayURLShorteners");
            // 
            // tsmiTrayURLSharingServices
            // 
            tsmiTrayURLSharingServices.Image = Properties.Resources.globe_share;
            tsmiTrayURLSharingServices.Name = "tsmiTrayURLSharingServices";
            resources.ApplyResources(tsmiTrayURLSharingServices, "tsmiTrayURLSharingServices");
            // 
            // tssTray2
            // 
            tssTray2.Name = "tssTray2";
            resources.ApplyResources(tssTray2, "tssTray2");
            // 
            // tsmiTrayApplicationSettings
            // 
            tsmiTrayApplicationSettings.Image = Properties.Resources.wrench_screwdriver;
            tsmiTrayApplicationSettings.Name = "tsmiTrayApplicationSettings";
            resources.ApplyResources(tsmiTrayApplicationSettings, "tsmiTrayApplicationSettings");
            tsmiTrayApplicationSettings.Click += tsbApplicationSettings_Click;
            // 
            // tsmiTrayTaskSettings
            // 
            tsmiTrayTaskSettings.Image = Properties.Resources.gear;
            tsmiTrayTaskSettings.Name = "tsmiTrayTaskSettings";
            resources.ApplyResources(tsmiTrayTaskSettings, "tsmiTrayTaskSettings");
            tsmiTrayTaskSettings.Click += tsbTaskSettings_Click;
            // 
            // tsmiTrayHotkeySettings
            // 
            tsmiTrayHotkeySettings.Image = Properties.Resources.keyboard;
            tsmiTrayHotkeySettings.Name = "tsmiTrayHotkeySettings";
            resources.ApplyResources(tsmiTrayHotkeySettings, "tsmiTrayHotkeySettings");
            tsmiTrayHotkeySettings.Click += tsbHotkeySettings_Click;
            // 
            // tsmiTrayToggleHotkeys
            // 
            tsmiTrayToggleHotkeys.Image = Properties.Resources.keyboard__minus;
            tsmiTrayToggleHotkeys.Name = "tsmiTrayToggleHotkeys";
            resources.ApplyResources(tsmiTrayToggleHotkeys, "tsmiTrayToggleHotkeys");
            tsmiTrayToggleHotkeys.Click += tsmiTrayToggleHotkeys_Click;
            // 
            // tsmiTrayDestinationSettings
            // 
            tsmiTrayDestinationSettings.Image = Properties.Resources.globe_pencil;
            tsmiTrayDestinationSettings.Name = "tsmiTrayDestinationSettings";
            resources.ApplyResources(tsmiTrayDestinationSettings, "tsmiTrayDestinationSettings");
            tsmiTrayDestinationSettings.Click += tsbDestinationSettings_Click;
            // 
            // tsmiTrayCustomUploaderSettings
            // 
            tsmiTrayCustomUploaderSettings.Image = Properties.Resources.network_cloud;
            tsmiTrayCustomUploaderSettings.Name = "tsmiTrayCustomUploaderSettings";
            resources.ApplyResources(tsmiTrayCustomUploaderSettings, "tsmiTrayCustomUploaderSettings");
            tsmiTrayCustomUploaderSettings.Click += tsbCustomUploaderSettings_Click;
            // 
            // tssTray3
            // 
            tssTray3.Name = "tssTray3";
            resources.ApplyResources(tssTray3, "tssTray3");
            // 
            // tsmiScreenshotsFolder
            // 
            tsmiScreenshotsFolder.Image = Properties.Resources.folder_open_image;
            tsmiScreenshotsFolder.Name = "tsmiScreenshotsFolder";
            resources.ApplyResources(tsmiScreenshotsFolder, "tsmiScreenshotsFolder");
            tsmiScreenshotsFolder.Click += tsbScreenshotsFolder_Click;
            // 
            // tsmiTrayHistory
            // 
            tsmiTrayHistory.Image = Properties.Resources.application_blog;
            tsmiTrayHistory.Name = "tsmiTrayHistory";
            resources.ApplyResources(tsmiTrayHistory, "tsmiTrayHistory");
            tsmiTrayHistory.Click += tsbHistory_Click;
            // 
            // tsmiTrayImageHistory
            // 
            tsmiTrayImageHistory.Image = Properties.Resources.application_icon_large;
            tsmiTrayImageHistory.Name = "tsmiTrayImageHistory";
            resources.ApplyResources(tsmiTrayImageHistory, "tsmiTrayImageHistory");
            tsmiTrayImageHistory.Click += tsbImageHistory_Click;
            // 
            // tssTray4
            // 
            tssTray4.Name = "tssTray4";
            resources.ApplyResources(tssTray4, "tssTray4");
            // 
            // tsmiRestartAsAdmin
            // 
            tsmiRestartAsAdmin.Image = Properties.Resources.uac;
            tsmiRestartAsAdmin.Name = "tsmiRestartAsAdmin";
            resources.ApplyResources(tsmiRestartAsAdmin, "tsmiRestartAsAdmin");
            tsmiRestartAsAdmin.Click += tsmiRestartAsAdmin_Click;
            // 
            // tsmiTrayRecentItems
            // 
            tsmiTrayRecentItems.Image = Properties.Resources.clipboard_list;
            tsmiTrayRecentItems.Name = "tsmiTrayRecentItems";
            resources.ApplyResources(tsmiTrayRecentItems, "tsmiTrayRecentItems");
            // 
            // tsmiOpenActionsToolbar
            // 
            tsmiOpenActionsToolbar.Image = Properties.Resources.ui_toolbar__arrow;
            tsmiOpenActionsToolbar.Name = "tsmiOpenActionsToolbar";
            resources.ApplyResources(tsmiOpenActionsToolbar, "tsmiOpenActionsToolbar");
            tsmiOpenActionsToolbar.Click += tsmiOpenActionsToolbar_Click;
            // 
            // tsmiTrayShow
            // 
            tsmiTrayShow.Image = Properties.Resources.tick_button;
            tsmiTrayShow.Name = "tsmiTrayShow";
            resources.ApplyResources(tsmiTrayShow, "tsmiTrayShow");
            tsmiTrayShow.Click += tsmiTrayShow_Click;
            // 
            // tsmiTrayExit
            // 
            tsmiTrayExit.Image = Properties.Resources.cross_button;
            tsmiTrayExit.Name = "tsmiTrayExit";
            resources.ApplyResources(tsmiTrayExit, "tsmiTrayExit");
            tsmiTrayExit.Click += tsmiTrayExit_Click;
            tsmiTrayExit.MouseDown += tsmiTrayExit_MouseDown;
            // 
            // timerTraySingleClick
            // 
            timerTraySingleClick.Tick += timerTraySingleClick_Tick;
            // 
            // ttMain
            // 
            ttMain.AutoPopDelay = 5000;
            ttMain.InitialDelay = 200;
            ttMain.OwnerDraw = true;
            ttMain.ReshowDelay = 100;
            ttMain.Draw += TtMain_Draw;
            // 
            // pToolbars
            // 
            resources.ApplyResources(pToolbars, "pToolbars");
            pToolbars.Controls.Add(tsMain);
            pToolbars.Name = "pToolbars";
            // 
            // dgvHotkeys
            // 
            dgvHotkeys.AllowUserToAddRows = false;
            dgvHotkeys.AllowUserToDeleteRows = false;
            dgvHotkeys.AllowUserToResizeColumns = false;
            dgvHotkeys.AllowUserToResizeRows = false;
            dgvHotkeys.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgvHotkeys.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvHotkeys.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvHotkeys.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvHotkeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHotkeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { cHotkeyStatus, cHotkey, cDescription });
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvHotkeys.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(dgvHotkeys, "dgvHotkeys");
            dgvHotkeys.MultiSelect = false;
            dgvHotkeys.Name = "dgvHotkeys";
            dgvHotkeys.ReadOnly = true;
            dgvHotkeys.RowHeadersVisible = false;
            dgvHotkeys.TabStop = false;
            dgvHotkeys.MouseDoubleClick += dgvHotkeys_MouseDoubleClick;
            dgvHotkeys.MouseUp += dgvHotkeys_MouseUp;
            // 
            // cHotkeyStatus
            // 
            resources.ApplyResources(cHotkeyStatus, "cHotkeyStatus");
            cHotkeyStatus.Name = "cHotkeyStatus";
            cHotkeyStatus.ReadOnly = true;
            // 
            // cHotkey
            // 
            cHotkey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            cHotkey.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(cHotkey, "cHotkey");
            cHotkey.Name = "cHotkey";
            cHotkey.ReadOnly = true;
            cHotkey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cDescription
            // 
            cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(cDescription, "cDescription");
            cDescription.Name = "cDescription";
            cDescription.ReadOnly = true;
            cDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pMain
            // 
            pMain.Controls.Add(pHotkeys);
            pMain.Controls.Add(ucTaskThumbnailView);
            pMain.Controls.Add(scMain);
            resources.ApplyResources(pMain, "pMain");
            pMain.Name = "pMain";
            // 
            // pHotkeys
            // 
            pHotkeys.Controls.Add(dgvHotkeys);
            resources.ApplyResources(pHotkeys, "pHotkeys");
            pHotkeys.Name = "pHotkeys";
            // 
            // ucTaskThumbnailView
            // 
            resources.ApplyResources(ucTaskThumbnailView, "ucTaskThumbnailView");
            ucTaskThumbnailView.BackColor = System.Drawing.SystemColors.Window;
            ucTaskThumbnailView.ClickAction = ThumbnailViewClickAction.Default;
            ucTaskThumbnailView.Name = "ucTaskThumbnailView";
            ucTaskThumbnailView.ThumbnailSize = new System.Drawing.Size(200, 150);
            ucTaskThumbnailView.TitleLocation = ThumbnailTitleLocation.Top;
            ucTaskThumbnailView.TitleVisible = true;
            ucTaskThumbnailView.ContextMenuRequested += UcTaskView_ContextMenuRequested;
            ucTaskThumbnailView.SelectedPanelChanged += ucTaskThumbnailView_SelectedPanelChanged;
            ucTaskThumbnailView.KeyDown += lvUploads_KeyDown;
            // 
            // MainForm
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(pMain);
            Controls.Add(pToolbars);
            DoubleBuffered = true;
            Name = "MainForm";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Shown += MainForm_Shown;
            LocationChanged += MainForm_LocationChanged;
            SizeChanged += MainForm_SizeChanged;
            VisibleChanged += MainForm_VisibleChanged;
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            Resize += MainForm_Resize;
            scMain.Panel1.ResumeLayout(false);
            scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scMain).EndInit();
            scMain.ResumeLayout(false);
            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            cmsTaskInfo.ResumeLayout(false);
            cmsTray.ResumeLayout(false);
            pToolbars.ResumeLayout(false);
            pToolbars.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHotkeys).EndInit();
            pMain.ResumeLayout(false);
            pHotkeys.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion Windows Form Designer generated code
        private System.Windows.Forms.ColumnHeader chStatus;
        private System.Windows.Forms.ColumnHeader chURL;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chProgress;
        private ShareX.HelpersLib.ToolStripBorderRight tsMain;
        private System.Windows.Forms.ToolStripSeparator tssMain1;
        private System.Windows.Forms.ColumnHeader chSpeed;
        private System.Windows.Forms.ColumnHeader chRemaining;
        private System.Windows.Forms.ColumnHeader chElapsed;
        private System.Windows.Forms.ToolStripButton tsbHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiTextUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiURLShorteners;
        private System.Windows.Forms.ToolStripDropDownButton tsddbDestinations;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayExit;
        private System.Windows.Forms.ToolStripSeparator tssTray1;
        public System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.ToolStripDropDownButton tsddbCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiFullscreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayHistory;
        private System.Windows.Forms.ToolStripSeparator tssTray2;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayFullscreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiLastRegion;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayLastRegion;
        private HelpersLib.SplitContainerCustomSplitter scMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayDestinations;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTextUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayFileUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayURLShorteners;
        private System.Windows.Forms.ContextMenuStrip cmsTaskInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenShortenedURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenThumbnailURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenDeletionURL;
        private System.Windows.Forms.ToolStripSeparator tssOpen1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyShortenedURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyThumbnailURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyDeletionURL;
        private System.Windows.Forms.ToolStripSeparator tssCopy1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyText;
        private System.Windows.Forms.ToolStripSeparator tssCopy2;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHTMLLink;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHTMLImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHTMLLinkedImage;
        private System.Windows.Forms.ToolStripSeparator tssCopy3;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyForumLink;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyForumImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyForumLinkedImage;
        private System.Windows.Forms.ToolStripSeparator tssCopy4;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyFilePath;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyFileName;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyFileNameWithExtension;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiStopUpload;
        private HelpersLib.MyPictureBox pbPreview;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowErrors;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowResponse;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotsFolder;
        private System.Windows.Forms.ToolStripDropDownButton tsddbAfterCaptureTasks;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayAfterCaptureTasks;
        private System.Windows.Forms.ToolStripMenuItem tsmiURLSharingServices;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayURLSharingServices;
        private System.Windows.Forms.ToolStripDropDownButton tsddbAfterUploadTasks;
        private System.Windows.Forms.ToolStripButton tsbScreenshotsFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayAfterUploadTasks;
        private System.Windows.Forms.ToolStripSeparator tssUploadInfo1;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadSelectedFile;
        private System.Windows.Forms.ToolStripButton tsbImageHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTools;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayColorPicker;
        private System.Windows.Forms.ToolStripDropDownButton tsddbTools;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorPicker;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearList;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenRecordingGIF;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenRecordingGIF;
        private System.Windows.Forms.ToolStripMenuItem tsmiHashChecker;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayHashChecker;
        private System.Windows.Forms.ToolStripMenuItem tsmiMonitor;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayMonitor;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayAutoCapture;
        private System.Windows.Forms.ToolStripDropDownButton tsddbDebug;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestImageUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestTextUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestFileUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestURLShortener;
        private System.Windows.Forms.ToolStripSeparator tssCopy5;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowDebugLog;
        private System.Windows.Forms.ToolStripButton tsbApplicationSettings;
        private System.Windows.Forms.ToolStripButton tsbTaskSettings;
        private System.Windows.Forms.ToolStripButton tsbHotkeySettings;
        private System.Windows.Forms.ToolStripSeparator tssMain2;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayApplicationSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTaskSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayHotkeySettings;
        private System.Windows.Forms.ToolStripSeparator tssTray3;
        private System.Windows.Forms.ToolStripMenuItem tsmiIndexFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayIndexFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageEffects;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageEffects;
        private System.Windows.Forms.ToolStripButton tsbAbout;
        private System.Windows.Forms.ToolStripMenuItem tsmiMonitorTest;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayMonitorTest;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayShow;
        private System.Windows.Forms.ToolStripMenuItem tsmiRuler;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRuler;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenThumbnailFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyThumbnailFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyThumbnailImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageEditor;
        private System.Windows.Forms.ToolStripDropDownButton tsddbWorkflows;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayWorkflows;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowQRCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiQRCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayQRCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiRectangleLight;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRectangleLight;
        private System.Windows.Forms.ToolStripDropDownButton tsddbUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadClipboard;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadDragDrop;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadClipboard;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadDragDrop;
        private System.Windows.Forms.ToolStripMenuItem tsmiShareSelectedURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiShortenSelectedURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditSelectedFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestURLSharing;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteSelectedFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenRecordingFFmpeg;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenRecordingFFmpeg;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenColorPicker;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenColorPicker;
        public System.Windows.Forms.ToolStripMenuItem tsmiTrayRecentItems;
        private System.Windows.Forms.ContextMenuStrip cmsTray;
        private System.Windows.Forms.ToolStripMenuItem tsmiRectangleTransparent;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRectangleTransparent;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayToggleHotkeys;
        private System.Windows.Forms.ToolStripMenuItem tsmiVideoThumbnailer;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayVideoThumbnailer;
        private System.Windows.Forms.Timer timerTraySingleClick;
        private System.Windows.Forms.ToolStripMenuItem tsmiScrollingCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScrollingCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageCombiner;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageCombiner;
        private System.Windows.Forms.ToolStripMenuItem tsmiDownloadSelectedURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiOCRImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiCombineImages;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenActionsToolbar;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteSelectedItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageThumbnailer;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageThumbnailer;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadText;
        private System.Windows.Forms.ToolStripMenuItem tsmiShortenURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadText;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayShortenURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyMarkdownLink;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyMarkdownImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyMarkdownLinkedImage;
        private System.Windows.Forms.ToolStripSeparator tssCopy6;
        private System.Windows.Forms.ToolStripSeparator tssCapture1;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowCursor;
        private System.Windows.Forms.ToolStripSeparator tssTrayCapture1;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayShowCursor;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyImageDimensions;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay0;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay1;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay2;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay3;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay4;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenshotDelay5;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay0;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay1;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay2;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay3;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay4;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenshotDelay5;
        private TaskThumbnailView ucTaskThumbnailView;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwitchTaskViewMode;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiRunAction;
        private System.Windows.Forms.Panel pToolbars;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageSplitter;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageSplitter;
        private System.Windows.Forms.ToolStripMenuItem tsmiVideoConverter;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayVideoConverter;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddImageEffects;
        private System.Windows.Forms.ToolStripMenuItem tsmiClipboardViewer;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayClipboardViewer;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestartAsAdmin;
        private System.Windows.Forms.ToolStripMenuItem tsmiInspectWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayInspectWindow;
        private System.Windows.Forms.ToolStripSeparator tssTools1;
        private System.Windows.Forms.ToolStripSeparator tssTools2;
        private System.Windows.Forms.ToolStripSeparator tssTools3;
        private System.Windows.Forms.ToolStripSeparator tssTools4;
        private System.Windows.Forms.ToolStripSeparator tssTrayTools1;
        private System.Windows.Forms.ToolStripSeparator tssTrayTools2;
        private System.Windows.Forms.ToolStripSeparator tssTrayTools3;
        private System.Windows.Forms.ToolStripSeparator tssTrayTools4;
        private System.Windows.Forms.ToolStripMenuItem tsmiCombineImagesHorizontally;
        private System.Windows.Forms.ToolStripMenuItem tsmiCombineImagesVertically;
        private System.Windows.Forms.ToolStripMenuItem tsmiBingVisualSearch;
        private System.Windows.Forms.ToolStripButton tsbDiscord;
        private System.Windows.Forms.ToolStripSeparator tssMain3;
        private System.Windows.Forms.ToolStripButton tsbDonate;
        private System.Windows.Forms.ToolStripMenuItem tsmiBorderlessWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayBorderlessWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageViewer;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageViewer;
        private System.Windows.Forms.ToolStripMenuItem tsmiOCR;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayOCR;
        private System.Windows.Forms.ToolStripMenuItem tsmiPinSelectedFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiPinToScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayPinToScreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageBeautifier;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageBeautifier;
        private System.Windows.Forms.ToolStripMenuItem tsmiBeautifyImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn cHotkeyStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn cHotkey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
        private System.Windows.Forms.Panel pMain;
        private System.Windows.Forms.DataGridView dgvHotkeys;
        internal System.Windows.Forms.Panel pHotkeys;
        private HelpersLib.MyListView lvUploads;
        private System.Windows.Forms.ToolStripButton tsbX;
        private System.Windows.Forms.ToolStripMenuItem tsmiGoogleLens;
        private System.Windows.Forms.ToolStripMenuItem tsmiMetadata;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayMetadata;
        private System.Windows.Forms.ToolStripButton tsbDestinationSettings;
        private System.Windows.Forms.ToolStripButton tsbCustomUploaderSettings;
        private System.Windows.Forms.ToolStripSeparator tssMain4;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayDestinationSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayCustomUploaderSettings;
        private System.Windows.Forms.ToolStripSeparator tssTray4;
    }
}