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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scMain = new ShareX.HelpersLib.SplitContainerCustomSplitter();
            this.lvUploads = new ShareX.HelpersLib.MyListView();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chElapsed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chRemaining = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbPreview = new ShareX.HelpersLib.MyPictureBox();
            this.tsMain = new ShareX.HelpersLib.ToolStripBorderRight();
            this.tsddbCapture = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRectangleLight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRectangleTransparent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLastRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenRecordingFFmpeg = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenRecordingGIF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScrollingCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCapture1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiShowCursor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay0 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenshotDelay5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddbUpload = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiUploadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadText = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShortenURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTweetMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddbWorkflows = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPinToScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTools1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageBeautifier = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageCombiner = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageSplitter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTools2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiVideoConverter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVideoThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTools3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOCR = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHashChecker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTools4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClipboardViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBorderlessWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInspectWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDNSChanger = new System.Windows.Forms.ToolStripMenuItem();
            this.tssMain1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsddbAfterCaptureTasks = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbAfterUploadTasks = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbDestinations = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiURLSharingServices = new System.Windows.Forms.ToolStripMenuItem();
            this.tssDestinations1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDestinationSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCustomUploaderSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbApplicationSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbTaskSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbHotkeySettings = new System.Windows.Forms.ToolStripButton();
            this.tssMain2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbScreenshotsFolder = new System.Windows.Forms.ToolStripButton();
            this.tsbHistory = new System.Windows.Forms.ToolStripButton();
            this.tsbImageHistory = new System.Windows.Forms.ToolStripButton();
            this.tssMain3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsddbDebug = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiShowDebugLog = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestImageUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestTextUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestFileUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestURLShortener = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestURLSharing = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbDonate = new System.Windows.Forms.ToolStripButton();
            this.tsbX = new System.Windows.Forms.ToolStripButton();
            this.tsbDiscord = new System.Windows.Forms.ToolStripButton();
            this.tsbAbout = new System.Windows.Forms.ToolStripButton();
            this.cmsTaskInfo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiShowErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStopUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenShortenedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenThumbnailURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenDeletionURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tssOpen1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenThumbnailFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyShortenedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyThumbnailURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyDeletionURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyImageDimensions = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyText = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyThumbnailFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyThumbnailImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyHTMLLink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyHTMLImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyHTMLLinkedImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyForumLink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyForumImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyForumLinkedImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyMarkdownLink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyMarkdownImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyMarkdownLinkedImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyFilePath = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFileNameWithExtension = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiUploadSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDownloadSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBeautifyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPinSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRunAction = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteSelectedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShortenSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShareSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGoogleLens = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBingVisualSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOCRImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCombineImages = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCombineImagesHorizontally = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCombineImagesVertically = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowResponse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearList = new System.Windows.Forms.ToolStripMenuItem();
            this.tssUploadInfo1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSwitchTaskViewMode = new System.Windows.Forms.ToolStripMenuItem();
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiTrayCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRectangleLight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRectangleTransparent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayLastRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenRecordingFFmpeg = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenRecordingGIF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScrollingCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayAutoCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTrayCapture1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayShowCursor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay0 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenshotDelay5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadText = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayShortenURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTweetMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayWorkflows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayPinToScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTrayTools1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageBeautifier = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageCombiner = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageSplitter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTrayTools2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayVideoConverter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayVideoThumbnailer = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTrayTools3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayOCR = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHashChecker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTrayTools4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayClipboardViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayBorderlessWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayInspectWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayDNSChanger = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTray1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayAfterCaptureTasks = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayAfterUploadTasks = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayDestinations = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTextUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayFileUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayURLShorteners = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayURLSharingServices = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTrayDestinations1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayDestinationSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayCustomUploaderSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayApplicationSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTaskSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHotkeySettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayToggleHotkeys = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTray2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiScreenshotsFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTray3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRestartAsAdmin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRecentItems = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenActionsToolbar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerTraySingleClick = new System.Windows.Forms.Timer(this.components);
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.pToolbars = new System.Windows.Forms.Panel();
            this.dgvHotkeys = new System.Windows.Forms.DataGridView();
            this.cHotkeyStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cHotkey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pMain = new System.Windows.Forms.Panel();
            this.pHotkeys = new System.Windows.Forms.Panel();
            this.ucTaskThumbnailView = new ShareX.TaskThumbnailView();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.cmsTaskInfo.SuspendLayout();
            this.cmsTray.SuspendLayout();
            this.pToolbars.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotkeys)).BeginInit();
            this.pMain.SuspendLayout();
            this.pHotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // scMain
            // 
            resources.ApplyResources(this.scMain, "scMain");
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.lvUploads);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pbPreview);
            this.scMain.SplitterColor = System.Drawing.Color.White;
            this.scMain.SplitterLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            this.scMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.scMain_SplitterMoved);
            // 
            // lvUploads
            // 
            this.lvUploads.AutoFillColumn = true;
            this.lvUploads.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvUploads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chStatus,
            this.chProgress,
            this.chSpeed,
            this.chElapsed,
            this.chRemaining,
            this.chURL});
            resources.ApplyResources(this.lvUploads, "lvUploads");
            this.lvUploads.FullRowSelect = true;
            this.lvUploads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUploads.HideSelection = false;
            this.lvUploads.Name = "lvUploads";
            this.lvUploads.ShowItemToolTips = true;
            this.lvUploads.UseCompatibleStateImageBehavior = false;
            this.lvUploads.View = System.Windows.Forms.View.Details;
            this.lvUploads.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.lvUploads_ColumnWidthChanged);
            this.lvUploads.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvUploads_ItemDrag);
            this.lvUploads.SelectedIndexChanged += new System.EventHandler(this.lvUploads_SelectedIndexChanged);
            this.lvUploads.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvUploads_KeyDown);
            this.lvUploads.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvUploads_MouseDoubleClick);
            this.lvUploads.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvUploads_MouseUp);
            // 
            // chFilename
            // 
            resources.ApplyResources(this.chFilename, "chFilename");
            // 
            // chStatus
            // 
            resources.ApplyResources(this.chStatus, "chStatus");
            // 
            // chProgress
            // 
            resources.ApplyResources(this.chProgress, "chProgress");
            // 
            // chSpeed
            // 
            resources.ApplyResources(this.chSpeed, "chSpeed");
            // 
            // chElapsed
            // 
            resources.ApplyResources(this.chElapsed, "chElapsed");
            // 
            // chRemaining
            // 
            resources.ApplyResources(this.chRemaining, "chRemaining");
            // 
            // chURL
            // 
            resources.ApplyResources(this.chURL, "chURL");
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.SystemColors.Window;
            this.pbPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbPreview, "pbPreview");
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.EnableRightClickMenu = true;
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.PictureBoxBackColor = System.Drawing.SystemColors.Control;
            this.pbPreview.ShowImageSizeLabel = true;
            this.pbPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPreview_MouseDown);
            // 
            // tsMain
            // 
            this.tsMain.CanOverflow = false;
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.DrawCustomBorder = true;
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbCapture,
            this.tsddbUpload,
            this.tsddbWorkflows,
            this.tsddbTools,
            this.tssMain1,
            this.tsddbAfterCaptureTasks,
            this.tsddbAfterUploadTasks,
            this.tsddbDestinations,
            this.tsbApplicationSettings,
            this.tsbTaskSettings,
            this.tsbHotkeySettings,
            this.tssMain2,
            this.tsbScreenshotsFolder,
            this.tsbHistory,
            this.tsbImageHistory,
            this.tssMain3,
            this.tsddbDebug,
            this.tsbDonate,
            this.tsbX,
            this.tsbDiscord,
            this.tsbAbout});
            this.tsMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.tsMain.Name = "tsMain";
            this.tsMain.ShowItemToolTips = false;
            this.tsMain.TabStop = true;
            // 
            // tsddbCapture
            // 
            this.tsddbCapture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFullscreen,
            this.tsmiWindow,
            this.tsmiMonitor,
            this.tsmiRectangle,
            this.tsmiRectangleLight,
            this.tsmiRectangleTransparent,
            this.tsmiLastRegion,
            this.tsmiScreenRecordingFFmpeg,
            this.tsmiScreenRecordingGIF,
            this.tsmiScrollingCapture,
            this.tsmiAutoCapture,
            this.tssCapture1,
            this.tsmiShowCursor,
            this.tsmiScreenshotDelay});
            this.tsddbCapture.Image = global::ShareX.Properties.Resources.camera;
            resources.ApplyResources(this.tsddbCapture, "tsddbCapture");
            this.tsddbCapture.Name = "tsddbCapture";
            this.tsddbCapture.DropDownOpening += new System.EventHandler(this.tsddbCapture_DropDownOpening);
            // 
            // tsmiFullscreen
            // 
            this.tsmiFullscreen.Image = global::ShareX.Properties.Resources.layer_fullscreen;
            this.tsmiFullscreen.Name = "tsmiFullscreen";
            resources.ApplyResources(this.tsmiFullscreen, "tsmiFullscreen");
            this.tsmiFullscreen.Click += new System.EventHandler(this.tsmiFullscreen_Click);
            // 
            // tsmiWindow
            // 
            this.tsmiWindow.Image = global::ShareX.Properties.Resources.application_blue;
            this.tsmiWindow.Name = "tsmiWindow";
            resources.ApplyResources(this.tsmiWindow, "tsmiWindow");
            // 
            // tsmiMonitor
            // 
            this.tsmiMonitor.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiMonitor.Name = "tsmiMonitor";
            resources.ApplyResources(this.tsmiMonitor, "tsmiMonitor");
            // 
            // tsmiRectangle
            // 
            this.tsmiRectangle.Image = global::ShareX.Properties.Resources.layer_shape;
            this.tsmiRectangle.Name = "tsmiRectangle";
            resources.ApplyResources(this.tsmiRectangle, "tsmiRectangle");
            this.tsmiRectangle.Click += new System.EventHandler(this.tsmiRectangle_Click);
            // 
            // tsmiRectangleLight
            // 
            this.tsmiRectangleLight.Image = global::ShareX.Properties.Resources.Rectangle;
            this.tsmiRectangleLight.Name = "tsmiRectangleLight";
            resources.ApplyResources(this.tsmiRectangleLight, "tsmiRectangleLight");
            this.tsmiRectangleLight.Click += new System.EventHandler(this.tsmiRectangleLight_Click);
            // 
            // tsmiRectangleTransparent
            // 
            this.tsmiRectangleTransparent.Image = global::ShareX.Properties.Resources.layer_transparent;
            this.tsmiRectangleTransparent.Name = "tsmiRectangleTransparent";
            resources.ApplyResources(this.tsmiRectangleTransparent, "tsmiRectangleTransparent");
            this.tsmiRectangleTransparent.Click += new System.EventHandler(this.tsmiRectangleTransparent_Click);
            // 
            // tsmiLastRegion
            // 
            this.tsmiLastRegion.Image = global::ShareX.Properties.Resources.layers;
            this.tsmiLastRegion.Name = "tsmiLastRegion";
            resources.ApplyResources(this.tsmiLastRegion, "tsmiLastRegion");
            this.tsmiLastRegion.Click += new System.EventHandler(this.tsmiLastRegion_Click);
            // 
            // tsmiScreenRecordingFFmpeg
            // 
            this.tsmiScreenRecordingFFmpeg.Image = global::ShareX.Properties.Resources.camcorder_image;
            this.tsmiScreenRecordingFFmpeg.Name = "tsmiScreenRecordingFFmpeg";
            resources.ApplyResources(this.tsmiScreenRecordingFFmpeg, "tsmiScreenRecordingFFmpeg");
            this.tsmiScreenRecordingFFmpeg.Click += new System.EventHandler(this.tsmiScreenRecordingFFmpeg_Click);
            // 
            // tsmiScreenRecordingGIF
            // 
            this.tsmiScreenRecordingGIF.Image = global::ShareX.Properties.Resources.film;
            this.tsmiScreenRecordingGIF.Name = "tsmiScreenRecordingGIF";
            resources.ApplyResources(this.tsmiScreenRecordingGIF, "tsmiScreenRecordingGIF");
            this.tsmiScreenRecordingGIF.Click += new System.EventHandler(this.tsmiScreenRecordingGIF_Click);
            // 
            // tsmiScrollingCapture
            // 
            this.tsmiScrollingCapture.Image = global::ShareX.Properties.Resources.ui_scroll_pane_image;
            this.tsmiScrollingCapture.Name = "tsmiScrollingCapture";
            resources.ApplyResources(this.tsmiScrollingCapture, "tsmiScrollingCapture");
            this.tsmiScrollingCapture.Click += new System.EventHandler(this.tsmiScrollingCapture_Click);
            // 
            // tsmiAutoCapture
            // 
            this.tsmiAutoCapture.Image = global::ShareX.Properties.Resources.clock;
            this.tsmiAutoCapture.Name = "tsmiAutoCapture";
            resources.ApplyResources(this.tsmiAutoCapture, "tsmiAutoCapture");
            this.tsmiAutoCapture.Click += new System.EventHandler(this.tsmiAutoCapture_Click);
            // 
            // tssCapture1
            // 
            this.tssCapture1.Name = "tssCapture1";
            resources.ApplyResources(this.tssCapture1, "tssCapture1");
            // 
            // tsmiShowCursor
            // 
            this.tsmiShowCursor.CheckOnClick = true;
            this.tsmiShowCursor.Image = global::ShareX.Properties.Resources.cursor;
            this.tsmiShowCursor.Name = "tsmiShowCursor";
            resources.ApplyResources(this.tsmiShowCursor, "tsmiShowCursor");
            this.tsmiShowCursor.Click += new System.EventHandler(this.tsmiShowCursor_Click);
            // 
            // tsmiScreenshotDelay
            // 
            this.tsmiScreenshotDelay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScreenshotDelay0,
            this.tsmiScreenshotDelay1,
            this.tsmiScreenshotDelay2,
            this.tsmiScreenshotDelay3,
            this.tsmiScreenshotDelay4,
            this.tsmiScreenshotDelay5});
            this.tsmiScreenshotDelay.Image = global::ShareX.Properties.Resources.clock_select;
            this.tsmiScreenshotDelay.Name = "tsmiScreenshotDelay";
            resources.ApplyResources(this.tsmiScreenshotDelay, "tsmiScreenshotDelay");
            // 
            // tsmiScreenshotDelay0
            // 
            this.tsmiScreenshotDelay0.Name = "tsmiScreenshotDelay0";
            resources.ApplyResources(this.tsmiScreenshotDelay0, "tsmiScreenshotDelay0");
            this.tsmiScreenshotDelay0.Click += new System.EventHandler(this.tsmiScreenshotDelay0_Click);
            // 
            // tsmiScreenshotDelay1
            // 
            this.tsmiScreenshotDelay1.Name = "tsmiScreenshotDelay1";
            resources.ApplyResources(this.tsmiScreenshotDelay1, "tsmiScreenshotDelay1");
            this.tsmiScreenshotDelay1.Click += new System.EventHandler(this.tsmiScreenshotDelay1_Click);
            // 
            // tsmiScreenshotDelay2
            // 
            this.tsmiScreenshotDelay2.Name = "tsmiScreenshotDelay2";
            resources.ApplyResources(this.tsmiScreenshotDelay2, "tsmiScreenshotDelay2");
            this.tsmiScreenshotDelay2.Click += new System.EventHandler(this.tsmiScreenshotDelay2_Click);
            // 
            // tsmiScreenshotDelay3
            // 
            this.tsmiScreenshotDelay3.Name = "tsmiScreenshotDelay3";
            resources.ApplyResources(this.tsmiScreenshotDelay3, "tsmiScreenshotDelay3");
            this.tsmiScreenshotDelay3.Click += new System.EventHandler(this.tsmiScreenshotDelay3_Click);
            // 
            // tsmiScreenshotDelay4
            // 
            this.tsmiScreenshotDelay4.Name = "tsmiScreenshotDelay4";
            resources.ApplyResources(this.tsmiScreenshotDelay4, "tsmiScreenshotDelay4");
            this.tsmiScreenshotDelay4.Click += new System.EventHandler(this.tsmiScreenshotDelay4_Click);
            // 
            // tsmiScreenshotDelay5
            // 
            this.tsmiScreenshotDelay5.Name = "tsmiScreenshotDelay5";
            resources.ApplyResources(this.tsmiScreenshotDelay5, "tsmiScreenshotDelay5");
            this.tsmiScreenshotDelay5.Click += new System.EventHandler(this.tsmiScreenshotDelay5_Click);
            // 
            // tsddbUpload
            // 
            this.tsddbUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUploadFile,
            this.tsmiUploadFolder,
            this.tsmiUploadClipboard,
            this.tsmiUploadText,
            this.tsmiUploadURL,
            this.tsmiUploadDragDrop,
            this.tsmiShortenURL,
            this.tsmiTweetMessage});
            this.tsddbUpload.Image = global::ShareX.Properties.Resources.arrow_090;
            resources.ApplyResources(this.tsddbUpload, "tsddbUpload");
            this.tsddbUpload.Name = "tsddbUpload";
            // 
            // tsmiUploadFile
            // 
            this.tsmiUploadFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiUploadFile.Name = "tsmiUploadFile";
            resources.ApplyResources(this.tsmiUploadFile, "tsmiUploadFile");
            this.tsmiUploadFile.Click += new System.EventHandler(this.tsbFileUpload_Click);
            // 
            // tsmiUploadFolder
            // 
            this.tsmiUploadFolder.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiUploadFolder.Name = "tsmiUploadFolder";
            resources.ApplyResources(this.tsmiUploadFolder, "tsmiUploadFolder");
            this.tsmiUploadFolder.Click += new System.EventHandler(this.tsmiUploadFolder_Click);
            // 
            // tsmiUploadClipboard
            // 
            this.tsmiUploadClipboard.Image = global::ShareX.Properties.Resources.clipboard;
            this.tsmiUploadClipboard.Name = "tsmiUploadClipboard";
            resources.ApplyResources(this.tsmiUploadClipboard, "tsmiUploadClipboard");
            this.tsmiUploadClipboard.Click += new System.EventHandler(this.tsbClipboardUpload_Click);
            // 
            // tsmiUploadText
            // 
            this.tsmiUploadText.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiUploadText.Name = "tsmiUploadText";
            resources.ApplyResources(this.tsmiUploadText, "tsmiUploadText");
            this.tsmiUploadText.Click += new System.EventHandler(this.tsmiUploadText_Click);
            // 
            // tsmiUploadURL
            // 
            this.tsmiUploadURL.Image = global::ShareX.Properties.Resources.drive;
            this.tsmiUploadURL.Name = "tsmiUploadURL";
            resources.ApplyResources(this.tsmiUploadURL, "tsmiUploadURL");
            this.tsmiUploadURL.Click += new System.EventHandler(this.tsmiUploadURL_Click);
            // 
            // tsmiUploadDragDrop
            // 
            this.tsmiUploadDragDrop.Image = global::ShareX.Properties.Resources.inbox;
            this.tsmiUploadDragDrop.Name = "tsmiUploadDragDrop";
            resources.ApplyResources(this.tsmiUploadDragDrop, "tsmiUploadDragDrop");
            this.tsmiUploadDragDrop.Click += new System.EventHandler(this.tsbDragDropUpload_Click);
            // 
            // tsmiShortenURL
            // 
            this.tsmiShortenURL.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiShortenURL.Name = "tsmiShortenURL";
            resources.ApplyResources(this.tsmiShortenURL, "tsmiShortenURL");
            this.tsmiShortenURL.Click += new System.EventHandler(this.tsmiShortenURL_Click);
            // 
            // tsmiTweetMessage
            // 
            this.tsmiTweetMessage.Image = global::ShareX.Properties.Resources.X_black;
            this.tsmiTweetMessage.Name = "tsmiTweetMessage";
            resources.ApplyResources(this.tsmiTweetMessage, "tsmiTweetMessage");
            this.tsmiTweetMessage.Click += new System.EventHandler(this.tsmiTweetMessage_Click);
            // 
            // tsddbWorkflows
            // 
            this.tsddbWorkflows.Image = global::ShareX.Properties.Resources.categories;
            resources.ApplyResources(this.tsddbWorkflows, "tsddbWorkflows");
            this.tsddbWorkflows.Name = "tsddbWorkflows";
            // 
            // tsddbTools
            // 
            this.tsddbTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiColorPicker,
            this.tsmiScreenColorPicker,
            this.tsmiRuler,
            this.tsmiPinToScreen,
            this.tssTools1,
            this.tsmiImageEditor,
            this.tsmiImageBeautifier,
            this.tsmiImageEffects,
            this.tsmiImageViewer,
            this.tsmiImageCombiner,
            this.tsmiImageSplitter,
            this.tsmiImageThumbnailer,
            this.tssTools2,
            this.tsmiVideoConverter,
            this.tsmiVideoThumbnailer,
            this.tssTools3,
            this.tsmiOCR,
            this.tsmiQRCode,
            this.tsmiHashChecker,
            this.tsmiIndexFolder,
            this.tssTools4,
            this.tsmiClipboardViewer,
            this.tsmiBorderlessWindow,
            this.tsmiInspectWindow,
            this.tsmiMonitorTest,
            this.tsmiDNSChanger});
            this.tsddbTools.Image = global::ShareX.Properties.Resources.toolbox;
            resources.ApplyResources(this.tsddbTools, "tsddbTools");
            this.tsddbTools.Name = "tsddbTools";
            // 
            // tsmiColorPicker
            // 
            this.tsmiColorPicker.Image = global::ShareX.Properties.Resources.color;
            this.tsmiColorPicker.Name = "tsmiColorPicker";
            resources.ApplyResources(this.tsmiColorPicker, "tsmiColorPicker");
            this.tsmiColorPicker.Click += new System.EventHandler(this.tsmiColorPicker_Click);
            // 
            // tsmiScreenColorPicker
            // 
            this.tsmiScreenColorPicker.Image = global::ShareX.Properties.Resources.pipette;
            this.tsmiScreenColorPicker.Name = "tsmiScreenColorPicker";
            resources.ApplyResources(this.tsmiScreenColorPicker, "tsmiScreenColorPicker");
            this.tsmiScreenColorPicker.Click += new System.EventHandler(this.tsmiScreenColorPicker_Click);
            // 
            // tsmiRuler
            // 
            this.tsmiRuler.Image = global::ShareX.Properties.Resources.ruler_triangle;
            this.tsmiRuler.Name = "tsmiRuler";
            resources.ApplyResources(this.tsmiRuler, "tsmiRuler");
            this.tsmiRuler.Click += new System.EventHandler(this.tsmiRuler_Click);
            // 
            // tsmiPinToScreen
            // 
            this.tsmiPinToScreen.Image = global::ShareX.Properties.Resources.pin;
            this.tsmiPinToScreen.Name = "tsmiPinToScreen";
            resources.ApplyResources(this.tsmiPinToScreen, "tsmiPinToScreen");
            this.tsmiPinToScreen.Click += new System.EventHandler(this.tsmiPinToScreen_Click);
            // 
            // tssTools1
            // 
            this.tssTools1.Name = "tssTools1";
            resources.ApplyResources(this.tssTools1, "tssTools1");
            // 
            // tsmiImageEditor
            // 
            this.tsmiImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiImageEditor.Name = "tsmiImageEditor";
            resources.ApplyResources(this.tsmiImageEditor, "tsmiImageEditor");
            this.tsmiImageEditor.Click += new System.EventHandler(this.tsmiImageEditor_Click);
            // 
            // tsmiImageBeautifier
            // 
            this.tsmiImageBeautifier.Image = global::ShareX.Properties.Resources.picture_sunset;
            this.tsmiImageBeautifier.Name = "tsmiImageBeautifier";
            resources.ApplyResources(this.tsmiImageBeautifier, "tsmiImageBeautifier");
            this.tsmiImageBeautifier.Click += new System.EventHandler(this.tsmiImageBeautifier_Click);
            // 
            // tsmiImageEffects
            // 
            this.tsmiImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiImageEffects.Name = "tsmiImageEffects";
            resources.ApplyResources(this.tsmiImageEffects, "tsmiImageEffects");
            this.tsmiImageEffects.Click += new System.EventHandler(this.tsmiImageEffects_Click);
            // 
            // tsmiImageViewer
            // 
            this.tsmiImageViewer.Image = global::ShareX.Properties.Resources.images_flickr;
            this.tsmiImageViewer.Name = "tsmiImageViewer";
            resources.ApplyResources(this.tsmiImageViewer, "tsmiImageViewer");
            this.tsmiImageViewer.Click += new System.EventHandler(this.tsmiImageViewer_Click);
            // 
            // tsmiImageCombiner
            // 
            this.tsmiImageCombiner.Image = global::ShareX.Properties.Resources.document_break;
            this.tsmiImageCombiner.Name = "tsmiImageCombiner";
            resources.ApplyResources(this.tsmiImageCombiner, "tsmiImageCombiner");
            this.tsmiImageCombiner.Click += new System.EventHandler(this.tsmiImageCombiner_Click);
            // 
            // tsmiImageSplitter
            // 
            this.tsmiImageSplitter.Image = global::ShareX.Properties.Resources.image_split;
            this.tsmiImageSplitter.Name = "tsmiImageSplitter";
            resources.ApplyResources(this.tsmiImageSplitter, "tsmiImageSplitter");
            this.tsmiImageSplitter.Click += new System.EventHandler(this.TsmiImageSplitter_Click);
            // 
            // tsmiImageThumbnailer
            // 
            this.tsmiImageThumbnailer.Image = global::ShareX.Properties.Resources.image_resize_actual;
            this.tsmiImageThumbnailer.Name = "tsmiImageThumbnailer";
            resources.ApplyResources(this.tsmiImageThumbnailer, "tsmiImageThumbnailer");
            this.tsmiImageThumbnailer.Click += new System.EventHandler(this.tsmiImageThumbnailer_Click);
            // 
            // tssTools2
            // 
            this.tssTools2.Name = "tssTools2";
            resources.ApplyResources(this.tssTools2, "tssTools2");
            // 
            // tsmiVideoConverter
            // 
            this.tsmiVideoConverter.Image = global::ShareX.Properties.Resources.camcorder_pencil;
            this.tsmiVideoConverter.Name = "tsmiVideoConverter";
            resources.ApplyResources(this.tsmiVideoConverter, "tsmiVideoConverter");
            this.tsmiVideoConverter.Click += new System.EventHandler(this.tsmiVideoConverter_Click);
            // 
            // tsmiVideoThumbnailer
            // 
            this.tsmiVideoThumbnailer.Image = global::ShareX.Properties.Resources.images_stack;
            this.tsmiVideoThumbnailer.Name = "tsmiVideoThumbnailer";
            resources.ApplyResources(this.tsmiVideoThumbnailer, "tsmiVideoThumbnailer");
            this.tsmiVideoThumbnailer.Click += new System.EventHandler(this.tsmiVideoThumbnailer_Click);
            // 
            // tssTools3
            // 
            this.tssTools3.Name = "tssTools3";
            resources.ApplyResources(this.tssTools3, "tssTools3");
            // 
            // tsmiOCR
            // 
            this.tsmiOCR.Image = global::ShareX.Properties.Resources.edit_drop_cap;
            this.tsmiOCR.Name = "tsmiOCR";
            resources.ApplyResources(this.tsmiOCR, "tsmiOCR");
            this.tsmiOCR.Click += new System.EventHandler(this.tsmiOCR_Click);
            // 
            // tsmiQRCode
            // 
            this.tsmiQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiQRCode.Name = "tsmiQRCode";
            resources.ApplyResources(this.tsmiQRCode, "tsmiQRCode");
            this.tsmiQRCode.Click += new System.EventHandler(this.tsmiQRCode_Click);
            // 
            // tsmiHashChecker
            // 
            this.tsmiHashChecker.Image = global::ShareX.Properties.Resources.application_task;
            this.tsmiHashChecker.Name = "tsmiHashChecker";
            resources.ApplyResources(this.tsmiHashChecker, "tsmiHashChecker");
            this.tsmiHashChecker.Click += new System.EventHandler(this.tsmiHashChecker_Click);
            // 
            // tsmiIndexFolder
            // 
            this.tsmiIndexFolder.Image = global::ShareX.Properties.Resources.folder_tree;
            this.tsmiIndexFolder.Name = "tsmiIndexFolder";
            resources.ApplyResources(this.tsmiIndexFolder, "tsmiIndexFolder");
            this.tsmiIndexFolder.Click += new System.EventHandler(this.tsmiIndexFolder_Click);
            // 
            // tssTools4
            // 
            this.tssTools4.Name = "tssTools4";
            resources.ApplyResources(this.tssTools4, "tssTools4");
            // 
            // tsmiClipboardViewer
            // 
            this.tsmiClipboardViewer.Image = global::ShareX.Properties.Resources.clipboard_block;
            this.tsmiClipboardViewer.Name = "tsmiClipboardViewer";
            resources.ApplyResources(this.tsmiClipboardViewer, "tsmiClipboardViewer");
            this.tsmiClipboardViewer.Click += new System.EventHandler(this.tsmiClipboardViewer_Click);
            // 
            // tsmiBorderlessWindow
            // 
            this.tsmiBorderlessWindow.Image = global::ShareX.Properties.Resources.application_resize_full;
            this.tsmiBorderlessWindow.Name = "tsmiBorderlessWindow";
            resources.ApplyResources(this.tsmiBorderlessWindow, "tsmiBorderlessWindow");
            this.tsmiBorderlessWindow.Click += new System.EventHandler(this.tsmiBorderlessWindow_Click);
            // 
            // tsmiInspectWindow
            // 
            this.tsmiInspectWindow.Image = global::ShareX.Properties.Resources.application_search_result;
            this.tsmiInspectWindow.Name = "tsmiInspectWindow";
            resources.ApplyResources(this.tsmiInspectWindow, "tsmiInspectWindow");
            this.tsmiInspectWindow.Click += new System.EventHandler(this.tsmiInspectWindow_Click);
            // 
            // tsmiMonitorTest
            // 
            this.tsmiMonitorTest.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiMonitorTest.Name = "tsmiMonitorTest";
            resources.ApplyResources(this.tsmiMonitorTest, "tsmiMonitorTest");
            this.tsmiMonitorTest.Click += new System.EventHandler(this.tsmiMonitorTest_Click);
            // 
            // tsmiDNSChanger
            // 
            this.tsmiDNSChanger.Image = global::ShareX.Properties.Resources.network_ip;
            this.tsmiDNSChanger.Name = "tsmiDNSChanger";
            resources.ApplyResources(this.tsmiDNSChanger, "tsmiDNSChanger");
            this.tsmiDNSChanger.Click += new System.EventHandler(this.tsmiDNSChanger_Click);
            // 
            // tssMain1
            // 
            this.tssMain1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.tssMain1.Name = "tssMain1";
            resources.ApplyResources(this.tssMain1, "tssMain1");
            // 
            // tsddbAfterCaptureTasks
            // 
            this.tsddbAfterCaptureTasks.Image = global::ShareX.Properties.Resources.image_export;
            resources.ApplyResources(this.tsddbAfterCaptureTasks, "tsddbAfterCaptureTasks");
            this.tsddbAfterCaptureTasks.Name = "tsddbAfterCaptureTasks";
            // 
            // tsddbAfterUploadTasks
            // 
            this.tsddbAfterUploadTasks.Image = global::ShareX.Properties.Resources.upload_cloud;
            resources.ApplyResources(this.tsddbAfterUploadTasks, "tsddbAfterUploadTasks");
            this.tsddbAfterUploadTasks.Name = "tsddbAfterUploadTasks";
            // 
            // tsddbDestinations
            // 
            this.tsddbDestinations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImageUploaders,
            this.tsmiTextUploaders,
            this.tsmiFileUploaders,
            this.tsmiURLShorteners,
            this.tsmiURLSharingServices,
            this.tssDestinations1,
            this.tsmiDestinationSettings,
            this.tsmiCustomUploaderSettings});
            this.tsddbDestinations.Image = global::ShareX.Properties.Resources.drive_globe;
            resources.ApplyResources(this.tsddbDestinations, "tsddbDestinations");
            this.tsddbDestinations.Name = "tsddbDestinations";
            this.tsddbDestinations.DropDownOpened += new System.EventHandler(this.tsddbDestinations_DropDownOpened);
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
            // tssDestinations1
            // 
            this.tssDestinations1.Name = "tssDestinations1";
            resources.ApplyResources(this.tssDestinations1, "tssDestinations1");
            // 
            // tsmiDestinationSettings
            // 
            this.tsmiDestinationSettings.Image = global::ShareX.Properties.Resources.globe_pencil;
            this.tsmiDestinationSettings.Name = "tsmiDestinationSettings";
            resources.ApplyResources(this.tsmiDestinationSettings, "tsmiDestinationSettings");
            this.tsmiDestinationSettings.Click += new System.EventHandler(this.tsmiDestinationSettings_Click);
            // 
            // tsmiCustomUploaderSettings
            // 
            this.tsmiCustomUploaderSettings.Image = global::ShareX.Properties.Resources.network_cloud;
            this.tsmiCustomUploaderSettings.Name = "tsmiCustomUploaderSettings";
            resources.ApplyResources(this.tsmiCustomUploaderSettings, "tsmiCustomUploaderSettings");
            this.tsmiCustomUploaderSettings.Click += new System.EventHandler(this.tsmiCustomUploaderSettings_Click);
            // 
            // tsbApplicationSettings
            // 
            this.tsbApplicationSettings.Image = global::ShareX.Properties.Resources.wrench_screwdriver;
            resources.ApplyResources(this.tsbApplicationSettings, "tsbApplicationSettings");
            this.tsbApplicationSettings.Name = "tsbApplicationSettings";
            this.tsbApplicationSettings.Click += new System.EventHandler(this.tsbApplicationSettings_Click);
            // 
            // tsbTaskSettings
            // 
            this.tsbTaskSettings.Image = global::ShareX.Properties.Resources.gear;
            resources.ApplyResources(this.tsbTaskSettings, "tsbTaskSettings");
            this.tsbTaskSettings.Name = "tsbTaskSettings";
            this.tsbTaskSettings.Click += new System.EventHandler(this.tsbTaskSettings_Click);
            // 
            // tsbHotkeySettings
            // 
            this.tsbHotkeySettings.Image = global::ShareX.Properties.Resources.keyboard;
            resources.ApplyResources(this.tsbHotkeySettings, "tsbHotkeySettings");
            this.tsbHotkeySettings.Name = "tsbHotkeySettings";
            this.tsbHotkeySettings.Click += new System.EventHandler(this.tsbHotkeySettings_Click);
            // 
            // tssMain2
            // 
            this.tssMain2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.tssMain2.Name = "tssMain2";
            resources.ApplyResources(this.tssMain2, "tssMain2");
            // 
            // tsbScreenshotsFolder
            // 
            this.tsbScreenshotsFolder.Image = global::ShareX.Properties.Resources.folder_open_image;
            resources.ApplyResources(this.tsbScreenshotsFolder, "tsbScreenshotsFolder");
            this.tsbScreenshotsFolder.Name = "tsbScreenshotsFolder";
            this.tsbScreenshotsFolder.Click += new System.EventHandler(this.tsbScreenshotsFolder_Click);
            // 
            // tsbHistory
            // 
            this.tsbHistory.Image = global::ShareX.Properties.Resources.application_blog;
            resources.ApplyResources(this.tsbHistory, "tsbHistory");
            this.tsbHistory.Name = "tsbHistory";
            this.tsbHistory.Click += new System.EventHandler(this.tsbHistory_Click);
            // 
            // tsbImageHistory
            // 
            this.tsbImageHistory.Image = global::ShareX.Properties.Resources.application_icon_large;
            resources.ApplyResources(this.tsbImageHistory, "tsbImageHistory");
            this.tsbImageHistory.Name = "tsbImageHistory";
            this.tsbImageHistory.Click += new System.EventHandler(this.tsbImageHistory_Click);
            // 
            // tssMain3
            // 
            this.tssMain3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.tssMain3.Name = "tssMain3";
            resources.ApplyResources(this.tssMain3, "tssMain3");
            // 
            // tsddbDebug
            // 
            this.tsddbDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowDebugLog,
            this.tsmiTestImageUpload,
            this.tsmiTestTextUpload,
            this.tsmiTestFileUpload,
            this.tsmiTestURLShortener,
            this.tsmiTestURLSharing});
            this.tsddbDebug.Image = global::ShareX.Properties.Resources.traffic_cone;
            resources.ApplyResources(this.tsddbDebug, "tsddbDebug");
            this.tsddbDebug.Name = "tsddbDebug";
            // 
            // tsmiShowDebugLog
            // 
            this.tsmiShowDebugLog.Image = global::ShareX.Properties.Resources.application_monitor;
            this.tsmiShowDebugLog.Name = "tsmiShowDebugLog";
            resources.ApplyResources(this.tsmiShowDebugLog, "tsmiShowDebugLog");
            this.tsmiShowDebugLog.Click += new System.EventHandler(this.tsmiShowDebugLog_Click);
            // 
            // tsmiTestImageUpload
            // 
            this.tsmiTestImageUpload.Image = global::ShareX.Properties.Resources.image;
            this.tsmiTestImageUpload.Name = "tsmiTestImageUpload";
            resources.ApplyResources(this.tsmiTestImageUpload, "tsmiTestImageUpload");
            this.tsmiTestImageUpload.Click += new System.EventHandler(this.tsmiTestImageUpload_Click);
            // 
            // tsmiTestTextUpload
            // 
            this.tsmiTestTextUpload.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTestTextUpload.Name = "tsmiTestTextUpload";
            resources.ApplyResources(this.tsmiTestTextUpload, "tsmiTestTextUpload");
            this.tsmiTestTextUpload.Click += new System.EventHandler(this.tsmiTestTextUpload_Click);
            // 
            // tsmiTestFileUpload
            // 
            this.tsmiTestFileUpload.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiTestFileUpload.Name = "tsmiTestFileUpload";
            resources.ApplyResources(this.tsmiTestFileUpload, "tsmiTestFileUpload");
            this.tsmiTestFileUpload.Click += new System.EventHandler(this.tsmiTestFileUpload_Click);
            // 
            // tsmiTestURLShortener
            // 
            this.tsmiTestURLShortener.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTestURLShortener.Name = "tsmiTestURLShortener";
            resources.ApplyResources(this.tsmiTestURLShortener, "tsmiTestURLShortener");
            this.tsmiTestURLShortener.Click += new System.EventHandler(this.tsmiTestURLShortener_Click);
            // 
            // tsmiTestURLSharing
            // 
            this.tsmiTestURLSharing.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiTestURLSharing.Name = "tsmiTestURLSharing";
            resources.ApplyResources(this.tsmiTestURLSharing, "tsmiTestURLSharing");
            this.tsmiTestURLSharing.Click += new System.EventHandler(this.tsmiTestURLSharing_Click);
            // 
            // tsbDonate
            // 
            this.tsbDonate.Image = global::ShareX.Properties.Resources.heart;
            resources.ApplyResources(this.tsbDonate, "tsbDonate");
            this.tsbDonate.Name = "tsbDonate";
            this.tsbDonate.Click += new System.EventHandler(this.tsbDonate_Click);
            // 
            // tsbX
            // 
            this.tsbX.Image = global::ShareX.Properties.Resources.X_black;
            resources.ApplyResources(this.tsbX, "tsbX");
            this.tsbX.Name = "tsbX";
            this.tsbX.Click += new System.EventHandler(this.tsbX_Click);
            // 
            // tsbDiscord
            // 
            this.tsbDiscord.Image = global::ShareX.Properties.Resources.Discord_black;
            resources.ApplyResources(this.tsbDiscord, "tsbDiscord");
            this.tsbDiscord.Name = "tsbDiscord";
            this.tsbDiscord.Click += new System.EventHandler(this.tsbDiscord_Click);
            // 
            // tsbAbout
            // 
            this.tsbAbout.Image = global::ShareX.Properties.Resources.crown;
            resources.ApplyResources(this.tsbAbout, "tsbAbout");
            this.tsbAbout.Name = "tsbAbout";
            this.tsbAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // cmsTaskInfo
            // 
            this.cmsTaskInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowErrors,
            this.tsmiStopUpload,
            this.tsmiOpen,
            this.tsmiCopy,
            this.tsmiUploadSelectedFile,
            this.tsmiDownloadSelectedURL,
            this.tsmiEditSelectedFile,
            this.tsmiBeautifyImage,
            this.tsmiAddImageEffects,
            this.tsmiPinSelectedFile,
            this.tsmiRunAction,
            this.tsmiDeleteSelectedItem,
            this.tsmiDeleteSelectedFile,
            this.tsmiShortenSelectedURL,
            this.tsmiShareSelectedURL,
            this.tsmiGoogleLens,
            this.tsmiBingVisualSearch,
            this.tsmiShowQRCode,
            this.tsmiOCRImage,
            this.tsmiCombineImages,
            this.tsmiShowResponse,
            this.tsmiClearList,
            this.tssUploadInfo1,
            this.tsmiSwitchTaskViewMode});
            this.cmsTaskInfo.Name = "cmsHistory";
            resources.ApplyResources(this.cmsTaskInfo, "cmsTaskInfo");
            this.cmsTaskInfo.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.cmsTaskInfo_Closing);
            this.cmsTaskInfo.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cmsTaskInfo_PreviewKeyDown);
            // 
            // tsmiShowErrors
            // 
            this.tsmiShowErrors.Image = global::ShareX.Properties.Resources.exclamation_button;
            this.tsmiShowErrors.Name = "tsmiShowErrors";
            resources.ApplyResources(this.tsmiShowErrors, "tsmiShowErrors");
            this.tsmiShowErrors.Click += new System.EventHandler(this.tsmiShowErrors_Click);
            // 
            // tsmiStopUpload
            // 
            this.tsmiStopUpload.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiStopUpload.Name = "tsmiStopUpload";
            resources.ApplyResources(this.tsmiStopUpload, "tsmiStopUpload");
            this.tsmiStopUpload.Click += new System.EventHandler(this.tsmiStopUpload_Click);
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenURL,
            this.tsmiOpenShortenedURL,
            this.tsmiOpenThumbnailURL,
            this.tsmiOpenDeletionURL,
            this.tssOpen1,
            this.tsmiOpenFile,
            this.tsmiOpenFolder,
            this.tsmiOpenThumbnailFile});
            this.tsmiOpen.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiOpen.Name = "tsmiOpen";
            resources.ApplyResources(this.tsmiOpen, "tsmiOpen");
            // 
            // tsmiOpenURL
            // 
            this.tsmiOpenURL.Name = "tsmiOpenURL";
            resources.ApplyResources(this.tsmiOpenURL, "tsmiOpenURL");
            this.tsmiOpenURL.Click += new System.EventHandler(this.tsmiOpenURL_Click);
            // 
            // tsmiOpenShortenedURL
            // 
            this.tsmiOpenShortenedURL.Name = "tsmiOpenShortenedURL";
            resources.ApplyResources(this.tsmiOpenShortenedURL, "tsmiOpenShortenedURL");
            this.tsmiOpenShortenedURL.Click += new System.EventHandler(this.tsmiOpenShortenedURL_Click);
            // 
            // tsmiOpenThumbnailURL
            // 
            this.tsmiOpenThumbnailURL.Name = "tsmiOpenThumbnailURL";
            resources.ApplyResources(this.tsmiOpenThumbnailURL, "tsmiOpenThumbnailURL");
            this.tsmiOpenThumbnailURL.Click += new System.EventHandler(this.tsmiOpenThumbnailURL_Click);
            // 
            // tsmiOpenDeletionURL
            // 
            this.tsmiOpenDeletionURL.Name = "tsmiOpenDeletionURL";
            resources.ApplyResources(this.tsmiOpenDeletionURL, "tsmiOpenDeletionURL");
            this.tsmiOpenDeletionURL.Click += new System.EventHandler(this.tsmiOpenDeletionURL_Click);
            // 
            // tssOpen1
            // 
            this.tssOpen1.Name = "tssOpen1";
            resources.ApplyResources(this.tssOpen1, "tssOpen1");
            // 
            // tsmiOpenFile
            // 
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            resources.ApplyResources(this.tsmiOpenFile, "tsmiOpenFile");
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiOpenFolder
            // 
            this.tsmiOpenFolder.Name = "tsmiOpenFolder";
            resources.ApplyResources(this.tsmiOpenFolder, "tsmiOpenFolder");
            this.tsmiOpenFolder.Click += new System.EventHandler(this.tsmiOpenFolder_Click);
            // 
            // tsmiOpenThumbnailFile
            // 
            this.tsmiOpenThumbnailFile.Name = "tsmiOpenThumbnailFile";
            resources.ApplyResources(this.tsmiOpenThumbnailFile, "tsmiOpenThumbnailFile");
            this.tsmiOpenThumbnailFile.Click += new System.EventHandler(this.tsmiOpenThumbnailFile_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyURL,
            this.tsmiCopyShortenedURL,
            this.tsmiCopyThumbnailURL,
            this.tsmiCopyDeletionURL,
            this.tssCopy1,
            this.tsmiCopyFile,
            this.tsmiCopyImage,
            this.tsmiCopyImageDimensions,
            this.tsmiCopyText,
            this.tsmiCopyThumbnailFile,
            this.tsmiCopyThumbnailImage,
            this.tssCopy2,
            this.tsmiCopyHTMLLink,
            this.tsmiCopyHTMLImage,
            this.tsmiCopyHTMLLinkedImage,
            this.tssCopy3,
            this.tsmiCopyForumLink,
            this.tsmiCopyForumImage,
            this.tsmiCopyForumLinkedImage,
            this.tssCopy4,
            this.tsmiCopyMarkdownLink,
            this.tsmiCopyMarkdownImage,
            this.tsmiCopyMarkdownLinkedImage,
            this.tssCopy5,
            this.tsmiCopyFilePath,
            this.tsmiCopyFileName,
            this.tsmiCopyFileNameWithExtension,
            this.tsmiCopyFolder,
            this.tssCopy6});
            this.tsmiCopy.Image = global::ShareX.Properties.Resources.document_copy;
            this.tsmiCopy.Name = "tsmiCopy";
            resources.ApplyResources(this.tsmiCopy, "tsmiCopy");
            // 
            // tsmiCopyURL
            // 
            this.tsmiCopyURL.Name = "tsmiCopyURL";
            resources.ApplyResources(this.tsmiCopyURL, "tsmiCopyURL");
            this.tsmiCopyURL.Click += new System.EventHandler(this.tsmiCopyURL_Click);
            // 
            // tsmiCopyShortenedURL
            // 
            this.tsmiCopyShortenedURL.Name = "tsmiCopyShortenedURL";
            resources.ApplyResources(this.tsmiCopyShortenedURL, "tsmiCopyShortenedURL");
            this.tsmiCopyShortenedURL.Click += new System.EventHandler(this.tsmiCopyShortenedURL_Click);
            // 
            // tsmiCopyThumbnailURL
            // 
            this.tsmiCopyThumbnailURL.Name = "tsmiCopyThumbnailURL";
            resources.ApplyResources(this.tsmiCopyThumbnailURL, "tsmiCopyThumbnailURL");
            this.tsmiCopyThumbnailURL.Click += new System.EventHandler(this.tsmiCopyThumbnailURL_Click);
            // 
            // tsmiCopyDeletionURL
            // 
            this.tsmiCopyDeletionURL.Name = "tsmiCopyDeletionURL";
            resources.ApplyResources(this.tsmiCopyDeletionURL, "tsmiCopyDeletionURL");
            this.tsmiCopyDeletionURL.Click += new System.EventHandler(this.tsmiCopyDeletionURL_Click);
            // 
            // tssCopy1
            // 
            this.tssCopy1.Name = "tssCopy1";
            resources.ApplyResources(this.tssCopy1, "tssCopy1");
            // 
            // tsmiCopyFile
            // 
            this.tsmiCopyFile.Name = "tsmiCopyFile";
            resources.ApplyResources(this.tsmiCopyFile, "tsmiCopyFile");
            this.tsmiCopyFile.Click += new System.EventHandler(this.tsmiCopyFile_Click);
            // 
            // tsmiCopyImage
            // 
            this.tsmiCopyImage.Name = "tsmiCopyImage";
            resources.ApplyResources(this.tsmiCopyImage, "tsmiCopyImage");
            this.tsmiCopyImage.Click += new System.EventHandler(this.tsmiCopyImage_Click);
            // 
            // tsmiCopyImageDimensions
            // 
            this.tsmiCopyImageDimensions.Name = "tsmiCopyImageDimensions";
            resources.ApplyResources(this.tsmiCopyImageDimensions, "tsmiCopyImageDimensions");
            this.tsmiCopyImageDimensions.Click += new System.EventHandler(this.tsmiCopyImageDimensions_Click);
            // 
            // tsmiCopyText
            // 
            this.tsmiCopyText.Name = "tsmiCopyText";
            resources.ApplyResources(this.tsmiCopyText, "tsmiCopyText");
            this.tsmiCopyText.Click += new System.EventHandler(this.tsmiCopyText_Click);
            // 
            // tsmiCopyThumbnailFile
            // 
            this.tsmiCopyThumbnailFile.Name = "tsmiCopyThumbnailFile";
            resources.ApplyResources(this.tsmiCopyThumbnailFile, "tsmiCopyThumbnailFile");
            this.tsmiCopyThumbnailFile.Click += new System.EventHandler(this.tsmiCopyThumbnailFile_Click);
            // 
            // tsmiCopyThumbnailImage
            // 
            this.tsmiCopyThumbnailImage.Name = "tsmiCopyThumbnailImage";
            resources.ApplyResources(this.tsmiCopyThumbnailImage, "tsmiCopyThumbnailImage");
            this.tsmiCopyThumbnailImage.Click += new System.EventHandler(this.tsmiCopyThumbnailImage_Click);
            // 
            // tssCopy2
            // 
            this.tssCopy2.Name = "tssCopy2";
            resources.ApplyResources(this.tssCopy2, "tssCopy2");
            // 
            // tsmiCopyHTMLLink
            // 
            this.tsmiCopyHTMLLink.Name = "tsmiCopyHTMLLink";
            resources.ApplyResources(this.tsmiCopyHTMLLink, "tsmiCopyHTMLLink");
            this.tsmiCopyHTMLLink.Click += new System.EventHandler(this.tsmiCopyHTMLLink_Click);
            // 
            // tsmiCopyHTMLImage
            // 
            this.tsmiCopyHTMLImage.Name = "tsmiCopyHTMLImage";
            resources.ApplyResources(this.tsmiCopyHTMLImage, "tsmiCopyHTMLImage");
            this.tsmiCopyHTMLImage.Click += new System.EventHandler(this.tsmiCopyHTMLImage_Click);
            // 
            // tsmiCopyHTMLLinkedImage
            // 
            this.tsmiCopyHTMLLinkedImage.Name = "tsmiCopyHTMLLinkedImage";
            resources.ApplyResources(this.tsmiCopyHTMLLinkedImage, "tsmiCopyHTMLLinkedImage");
            this.tsmiCopyHTMLLinkedImage.Click += new System.EventHandler(this.tsmiCopyHTMLLinkedImage_Click);
            // 
            // tssCopy3
            // 
            this.tssCopy3.Name = "tssCopy3";
            resources.ApplyResources(this.tssCopy3, "tssCopy3");
            // 
            // tsmiCopyForumLink
            // 
            this.tsmiCopyForumLink.Name = "tsmiCopyForumLink";
            resources.ApplyResources(this.tsmiCopyForumLink, "tsmiCopyForumLink");
            this.tsmiCopyForumLink.Click += new System.EventHandler(this.tsmiCopyForumLink_Click);
            // 
            // tsmiCopyForumImage
            // 
            this.tsmiCopyForumImage.Name = "tsmiCopyForumImage";
            resources.ApplyResources(this.tsmiCopyForumImage, "tsmiCopyForumImage");
            this.tsmiCopyForumImage.Click += new System.EventHandler(this.tsmiCopyForumImage_Click);
            // 
            // tsmiCopyForumLinkedImage
            // 
            this.tsmiCopyForumLinkedImage.Name = "tsmiCopyForumLinkedImage";
            resources.ApplyResources(this.tsmiCopyForumLinkedImage, "tsmiCopyForumLinkedImage");
            this.tsmiCopyForumLinkedImage.Click += new System.EventHandler(this.tsmiCopyForumLinkedImage_Click);
            // 
            // tssCopy4
            // 
            this.tssCopy4.Name = "tssCopy4";
            resources.ApplyResources(this.tssCopy4, "tssCopy4");
            // 
            // tsmiCopyMarkdownLink
            // 
            this.tsmiCopyMarkdownLink.Name = "tsmiCopyMarkdownLink";
            resources.ApplyResources(this.tsmiCopyMarkdownLink, "tsmiCopyMarkdownLink");
            this.tsmiCopyMarkdownLink.Click += new System.EventHandler(this.tsmiCopyMarkdownLink_Click);
            // 
            // tsmiCopyMarkdownImage
            // 
            this.tsmiCopyMarkdownImage.Name = "tsmiCopyMarkdownImage";
            resources.ApplyResources(this.tsmiCopyMarkdownImage, "tsmiCopyMarkdownImage");
            this.tsmiCopyMarkdownImage.Click += new System.EventHandler(this.tsmiCopyMarkdownImage_Click);
            // 
            // tsmiCopyMarkdownLinkedImage
            // 
            this.tsmiCopyMarkdownLinkedImage.Name = "tsmiCopyMarkdownLinkedImage";
            resources.ApplyResources(this.tsmiCopyMarkdownLinkedImage, "tsmiCopyMarkdownLinkedImage");
            this.tsmiCopyMarkdownLinkedImage.Click += new System.EventHandler(this.tsmiCopyMarkdownLinkedImage_Click);
            // 
            // tssCopy5
            // 
            this.tssCopy5.Name = "tssCopy5";
            resources.ApplyResources(this.tssCopy5, "tssCopy5");
            // 
            // tsmiCopyFilePath
            // 
            this.tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            resources.ApplyResources(this.tsmiCopyFilePath, "tsmiCopyFilePath");
            this.tsmiCopyFilePath.Click += new System.EventHandler(this.tsmiCopyFilePath_Click);
            // 
            // tsmiCopyFileName
            // 
            this.tsmiCopyFileName.Name = "tsmiCopyFileName";
            resources.ApplyResources(this.tsmiCopyFileName, "tsmiCopyFileName");
            this.tsmiCopyFileName.Click += new System.EventHandler(this.tsmiCopyFileName_Click);
            // 
            // tsmiCopyFileNameWithExtension
            // 
            this.tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            resources.ApplyResources(this.tsmiCopyFileNameWithExtension, "tsmiCopyFileNameWithExtension");
            this.tsmiCopyFileNameWithExtension.Click += new System.EventHandler(this.tsmiCopyFileNameWithExtension_Click);
            // 
            // tsmiCopyFolder
            // 
            this.tsmiCopyFolder.Name = "tsmiCopyFolder";
            resources.ApplyResources(this.tsmiCopyFolder, "tsmiCopyFolder");
            this.tsmiCopyFolder.Click += new System.EventHandler(this.tsmiCopyFolder_Click);
            // 
            // tssCopy6
            // 
            this.tssCopy6.Name = "tssCopy6";
            resources.ApplyResources(this.tssCopy6, "tssCopy6");
            // 
            // tsmiUploadSelectedFile
            // 
            this.tsmiUploadSelectedFile.Image = global::ShareX.Properties.Resources.drive_upload;
            this.tsmiUploadSelectedFile.Name = "tsmiUploadSelectedFile";
            resources.ApplyResources(this.tsmiUploadSelectedFile, "tsmiUploadSelectedFile");
            this.tsmiUploadSelectedFile.Click += new System.EventHandler(this.tsmiUploadSelectedFile_Click);
            // 
            // tsmiDownloadSelectedURL
            // 
            this.tsmiDownloadSelectedURL.Image = global::ShareX.Properties.Resources.drive_download;
            this.tsmiDownloadSelectedURL.Name = "tsmiDownloadSelectedURL";
            resources.ApplyResources(this.tsmiDownloadSelectedURL, "tsmiDownloadSelectedURL");
            this.tsmiDownloadSelectedURL.Click += new System.EventHandler(this.tsmiDownloadSelectedURL_Click);
            // 
            // tsmiEditSelectedFile
            // 
            this.tsmiEditSelectedFile.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiEditSelectedFile.Name = "tsmiEditSelectedFile";
            resources.ApplyResources(this.tsmiEditSelectedFile, "tsmiEditSelectedFile");
            this.tsmiEditSelectedFile.Click += new System.EventHandler(this.tsmiEditSelectedFile_Click);
            // 
            // tsmiBeautifyImage
            // 
            this.tsmiBeautifyImage.Image = global::ShareX.Properties.Resources.picture_sunset;
            this.tsmiBeautifyImage.Name = "tsmiBeautifyImage";
            resources.ApplyResources(this.tsmiBeautifyImage, "tsmiBeautifyImage");
            this.tsmiBeautifyImage.Click += new System.EventHandler(this.tsmiBeautifyImage_Click);
            // 
            // tsmiAddImageEffects
            // 
            this.tsmiAddImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiAddImageEffects.Name = "tsmiAddImageEffects";
            resources.ApplyResources(this.tsmiAddImageEffects, "tsmiAddImageEffects");
            this.tsmiAddImageEffects.Click += new System.EventHandler(this.tsmiAddImageEffects_Click);
            // 
            // tsmiPinSelectedFile
            // 
            this.tsmiPinSelectedFile.Image = global::ShareX.Properties.Resources.pin;
            this.tsmiPinSelectedFile.Name = "tsmiPinSelectedFile";
            resources.ApplyResources(this.tsmiPinSelectedFile, "tsmiPinSelectedFile");
            this.tsmiPinSelectedFile.Click += new System.EventHandler(this.tsmiPinSelectedFile_Click);
            // 
            // tsmiRunAction
            // 
            this.tsmiRunAction.Image = global::ShareX.Properties.Resources.application_terminal;
            this.tsmiRunAction.Name = "tsmiRunAction";
            resources.ApplyResources(this.tsmiRunAction, "tsmiRunAction");
            // 
            // tsmiDeleteSelectedItem
            // 
            this.tsmiDeleteSelectedItem.Image = global::ShareX.Properties.Resources.script__minus;
            this.tsmiDeleteSelectedItem.Name = "tsmiDeleteSelectedItem";
            resources.ApplyResources(this.tsmiDeleteSelectedItem, "tsmiDeleteSelectedItem");
            this.tsmiDeleteSelectedItem.Click += new System.EventHandler(this.tsmiDeleteSelectedItem_Click);
            // 
            // tsmiDeleteSelectedFile
            // 
            this.tsmiDeleteSelectedFile.Image = global::ShareX.Properties.Resources.bin;
            this.tsmiDeleteSelectedFile.Name = "tsmiDeleteSelectedFile";
            resources.ApplyResources(this.tsmiDeleteSelectedFile, "tsmiDeleteSelectedFile");
            this.tsmiDeleteSelectedFile.Click += new System.EventHandler(this.tsmiDeleteSelectedFile_Click);
            // 
            // tsmiShortenSelectedURL
            // 
            this.tsmiShortenSelectedURL.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiShortenSelectedURL.Name = "tsmiShortenSelectedURL";
            resources.ApplyResources(this.tsmiShortenSelectedURL, "tsmiShortenSelectedURL");
            // 
            // tsmiShareSelectedURL
            // 
            this.tsmiShareSelectedURL.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiShareSelectedURL.Name = "tsmiShareSelectedURL";
            resources.ApplyResources(this.tsmiShareSelectedURL, "tsmiShareSelectedURL");
            // 
            // tsmiGoogleLens
            // 
            this.tsmiGoogleLens.Image = global::ShareX.Properties.Resources.Google_Lens;
            this.tsmiGoogleLens.Name = "tsmiGoogleLens";
            resources.ApplyResources(this.tsmiGoogleLens, "tsmiGoogleLens");
            this.tsmiGoogleLens.Click += new System.EventHandler(this.tsmiGoogleLens_Click);
            // 
            // tsmiBingVisualSearch
            // 
            this.tsmiBingVisualSearch.Image = global::ShareX.Properties.Resources.Bing;
            this.tsmiBingVisualSearch.Name = "tsmiBingVisualSearch";
            resources.ApplyResources(this.tsmiBingVisualSearch, "tsmiBingVisualSearch");
            this.tsmiBingVisualSearch.Click += new System.EventHandler(this.tsmiBingVisualSearch_Click);
            // 
            // tsmiShowQRCode
            // 
            this.tsmiShowQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiShowQRCode.Name = "tsmiShowQRCode";
            resources.ApplyResources(this.tsmiShowQRCode, "tsmiShowQRCode");
            this.tsmiShowQRCode.Click += new System.EventHandler(this.tsmiShowQRCode_Click);
            // 
            // tsmiOCRImage
            // 
            this.tsmiOCRImage.Image = global::ShareX.Properties.Resources.edit_drop_cap;
            this.tsmiOCRImage.Name = "tsmiOCRImage";
            resources.ApplyResources(this.tsmiOCRImage, "tsmiOCRImage");
            this.tsmiOCRImage.Click += new System.EventHandler(this.tsmiOCRImage_Click);
            // 
            // tsmiCombineImages
            // 
            this.tsmiCombineImages.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCombineImagesHorizontally,
            this.tsmiCombineImagesVertically});
            this.tsmiCombineImages.Image = global::ShareX.Properties.Resources.document_break;
            this.tsmiCombineImages.Name = "tsmiCombineImages";
            resources.ApplyResources(this.tsmiCombineImages, "tsmiCombineImages");
            this.tsmiCombineImages.Click += new System.EventHandler(this.tsmiCombineImages_Click);
            // 
            // tsmiCombineImagesHorizontally
            // 
            this.tsmiCombineImagesHorizontally.Image = global::ShareX.Properties.Resources.application_tile_horizontal;
            this.tsmiCombineImagesHorizontally.Name = "tsmiCombineImagesHorizontally";
            resources.ApplyResources(this.tsmiCombineImagesHorizontally, "tsmiCombineImagesHorizontally");
            this.tsmiCombineImagesHorizontally.Click += new System.EventHandler(this.tsmiCombineImagesHorizontally_Click);
            // 
            // tsmiCombineImagesVertically
            // 
            this.tsmiCombineImagesVertically.Image = global::ShareX.Properties.Resources.application_tile_vertical;
            this.tsmiCombineImagesVertically.Name = "tsmiCombineImagesVertically";
            resources.ApplyResources(this.tsmiCombineImagesVertically, "tsmiCombineImagesVertically");
            this.tsmiCombineImagesVertically.Click += new System.EventHandler(this.tsmiCombineImagesVertically_Click);
            // 
            // tsmiShowResponse
            // 
            this.tsmiShowResponse.Image = global::ShareX.Properties.Resources.application_browser;
            this.tsmiShowResponse.Name = "tsmiShowResponse";
            resources.ApplyResources(this.tsmiShowResponse, "tsmiShowResponse");
            this.tsmiShowResponse.Click += new System.EventHandler(this.tsmiShowResponse_Click);
            // 
            // tsmiClearList
            // 
            this.tsmiClearList.Image = global::ShareX.Properties.Resources.eraser;
            this.tsmiClearList.Name = "tsmiClearList";
            resources.ApplyResources(this.tsmiClearList, "tsmiClearList");
            this.tsmiClearList.Click += new System.EventHandler(this.tsmiClearList_Click);
            // 
            // tssUploadInfo1
            // 
            this.tssUploadInfo1.Name = "tssUploadInfo1";
            resources.ApplyResources(this.tssUploadInfo1, "tssUploadInfo1");
            // 
            // tsmiSwitchTaskViewMode
            // 
            this.tsmiSwitchTaskViewMode.Name = "tsmiSwitchTaskViewMode";
            resources.ApplyResources(this.tsmiSwitchTaskViewMode, "tsmiSwitchTaskViewMode");
            this.tsmiSwitchTaskViewMode.Click += new System.EventHandler(this.TsmiSwitchTaskViewMode_Click);
            // 
            // niTray
            // 
            this.niTray.ContextMenuStrip = this.cmsTray;
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.BalloonTipClicked += new System.EventHandler(this.niTray_BalloonTipClicked);
            this.niTray.MouseUp += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseUp);
            // 
            // cmsTray
            // 
            this.cmsTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayCapture,
            this.tsmiTrayUpload,
            this.tsmiTrayWorkflows,
            this.tsmiTrayTools,
            this.tssTray1,
            this.tsmiTrayAfterCaptureTasks,
            this.tsmiTrayAfterUploadTasks,
            this.tsmiTrayDestinations,
            this.tsmiTrayApplicationSettings,
            this.tsmiTrayTaskSettings,
            this.tsmiTrayHotkeySettings,
            this.tsmiTrayToggleHotkeys,
            this.tssTray2,
            this.tsmiScreenshotsFolder,
            this.tsmiTrayHistory,
            this.tsmiTrayImageHistory,
            this.tssTray3,
            this.tsmiRestartAsAdmin,
            this.tsmiTrayRecentItems,
            this.tsmiOpenActionsToolbar,
            this.tsmiTrayShow,
            this.tsmiTrayExit});
            this.cmsTray.Name = "cmsTray";
            resources.ApplyResources(this.cmsTray, "cmsTray");
            this.cmsTray.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.cmsTray_Closed);
            this.cmsTray.Opened += new System.EventHandler(this.cmsTray_Opened);
            // 
            // tsmiTrayCapture
            // 
            this.tsmiTrayCapture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayFullscreen,
            this.tsmiTrayWindow,
            this.tsmiTrayMonitor,
            this.tsmiTrayRectangle,
            this.tsmiTrayRectangleLight,
            this.tsmiTrayRectangleTransparent,
            this.tsmiTrayLastRegion,
            this.tsmiTrayScreenRecordingFFmpeg,
            this.tsmiTrayScreenRecordingGIF,
            this.tsmiTrayScrollingCapture,
            this.tsmiTrayAutoCapture,
            this.tssTrayCapture1,
            this.tsmiTrayShowCursor,
            this.tsmiTrayScreenshotDelay});
            this.tsmiTrayCapture.Image = global::ShareX.Properties.Resources.camera;
            this.tsmiTrayCapture.Name = "tsmiTrayCapture";
            resources.ApplyResources(this.tsmiTrayCapture, "tsmiTrayCapture");
            this.tsmiTrayCapture.DropDownOpening += new System.EventHandler(this.tsmiCapture_DropDownOpening);
            // 
            // tsmiTrayFullscreen
            // 
            this.tsmiTrayFullscreen.Image = global::ShareX.Properties.Resources.layer_fullscreen;
            this.tsmiTrayFullscreen.Name = "tsmiTrayFullscreen";
            resources.ApplyResources(this.tsmiTrayFullscreen, "tsmiTrayFullscreen");
            this.tsmiTrayFullscreen.Click += new System.EventHandler(this.tsmiTrayFullscreen_Click);
            // 
            // tsmiTrayWindow
            // 
            this.tsmiTrayWindow.Image = global::ShareX.Properties.Resources.application_blue;
            this.tsmiTrayWindow.Name = "tsmiTrayWindow";
            resources.ApplyResources(this.tsmiTrayWindow, "tsmiTrayWindow");
            // 
            // tsmiTrayMonitor
            // 
            this.tsmiTrayMonitor.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiTrayMonitor.Name = "tsmiTrayMonitor";
            resources.ApplyResources(this.tsmiTrayMonitor, "tsmiTrayMonitor");
            // 
            // tsmiTrayRectangle
            // 
            this.tsmiTrayRectangle.Image = global::ShareX.Properties.Resources.layer_shape;
            this.tsmiTrayRectangle.Name = "tsmiTrayRectangle";
            resources.ApplyResources(this.tsmiTrayRectangle, "tsmiTrayRectangle");
            this.tsmiTrayRectangle.Click += new System.EventHandler(this.tsmiTrayRectangle_Click);
            // 
            // tsmiTrayRectangleLight
            // 
            this.tsmiTrayRectangleLight.Image = global::ShareX.Properties.Resources.Rectangle;
            this.tsmiTrayRectangleLight.Name = "tsmiTrayRectangleLight";
            resources.ApplyResources(this.tsmiTrayRectangleLight, "tsmiTrayRectangleLight");
            this.tsmiTrayRectangleLight.Click += new System.EventHandler(this.tsmiTrayRectangleLight_Click);
            // 
            // tsmiTrayRectangleTransparent
            // 
            this.tsmiTrayRectangleTransparent.Image = global::ShareX.Properties.Resources.layer_transparent;
            this.tsmiTrayRectangleTransparent.Name = "tsmiTrayRectangleTransparent";
            resources.ApplyResources(this.tsmiTrayRectangleTransparent, "tsmiTrayRectangleTransparent");
            this.tsmiTrayRectangleTransparent.Click += new System.EventHandler(this.tsmiTrayRectangleTransparent_Click);
            // 
            // tsmiTrayLastRegion
            // 
            this.tsmiTrayLastRegion.Image = global::ShareX.Properties.Resources.layers;
            this.tsmiTrayLastRegion.Name = "tsmiTrayLastRegion";
            resources.ApplyResources(this.tsmiTrayLastRegion, "tsmiTrayLastRegion");
            this.tsmiTrayLastRegion.Click += new System.EventHandler(this.tsmiTrayLastRegion_Click);
            // 
            // tsmiTrayScreenRecordingFFmpeg
            // 
            this.tsmiTrayScreenRecordingFFmpeg.Image = global::ShareX.Properties.Resources.camcorder_image;
            this.tsmiTrayScreenRecordingFFmpeg.Name = "tsmiTrayScreenRecordingFFmpeg";
            resources.ApplyResources(this.tsmiTrayScreenRecordingFFmpeg, "tsmiTrayScreenRecordingFFmpeg");
            this.tsmiTrayScreenRecordingFFmpeg.Click += new System.EventHandler(this.tsmiScreenRecordingFFmpeg_Click);
            // 
            // tsmiTrayScreenRecordingGIF
            // 
            this.tsmiTrayScreenRecordingGIF.Image = global::ShareX.Properties.Resources.film;
            this.tsmiTrayScreenRecordingGIF.Name = "tsmiTrayScreenRecordingGIF";
            resources.ApplyResources(this.tsmiTrayScreenRecordingGIF, "tsmiTrayScreenRecordingGIF");
            this.tsmiTrayScreenRecordingGIF.Click += new System.EventHandler(this.tsmiScreenRecordingGIF_Click);
            // 
            // tsmiTrayScrollingCapture
            // 
            this.tsmiTrayScrollingCapture.Image = global::ShareX.Properties.Resources.ui_scroll_pane_image;
            this.tsmiTrayScrollingCapture.Name = "tsmiTrayScrollingCapture";
            resources.ApplyResources(this.tsmiTrayScrollingCapture, "tsmiTrayScrollingCapture");
            this.tsmiTrayScrollingCapture.Click += new System.EventHandler(this.tsmiScrollingCapture_Click);
            // 
            // tsmiTrayAutoCapture
            // 
            this.tsmiTrayAutoCapture.Image = global::ShareX.Properties.Resources.clock;
            this.tsmiTrayAutoCapture.Name = "tsmiTrayAutoCapture";
            resources.ApplyResources(this.tsmiTrayAutoCapture, "tsmiTrayAutoCapture");
            this.tsmiTrayAutoCapture.Click += new System.EventHandler(this.tsmiAutoCapture_Click);
            // 
            // tssTrayCapture1
            // 
            this.tssTrayCapture1.Name = "tssTrayCapture1";
            resources.ApplyResources(this.tssTrayCapture1, "tssTrayCapture1");
            // 
            // tsmiTrayShowCursor
            // 
            this.tsmiTrayShowCursor.CheckOnClick = true;
            this.tsmiTrayShowCursor.Image = global::ShareX.Properties.Resources.cursor;
            this.tsmiTrayShowCursor.Name = "tsmiTrayShowCursor";
            resources.ApplyResources(this.tsmiTrayShowCursor, "tsmiTrayShowCursor");
            this.tsmiTrayShowCursor.Click += new System.EventHandler(this.tsmiShowCursor_Click);
            // 
            // tsmiTrayScreenshotDelay
            // 
            this.tsmiTrayScreenshotDelay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayScreenshotDelay0,
            this.tsmiTrayScreenshotDelay1,
            this.tsmiTrayScreenshotDelay2,
            this.tsmiTrayScreenshotDelay3,
            this.tsmiTrayScreenshotDelay4,
            this.tsmiTrayScreenshotDelay5});
            this.tsmiTrayScreenshotDelay.Image = global::ShareX.Properties.Resources.clock_select;
            this.tsmiTrayScreenshotDelay.Name = "tsmiTrayScreenshotDelay";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay, "tsmiTrayScreenshotDelay");
            // 
            // tsmiTrayScreenshotDelay0
            // 
            this.tsmiTrayScreenshotDelay0.Name = "tsmiTrayScreenshotDelay0";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay0, "tsmiTrayScreenshotDelay0");
            this.tsmiTrayScreenshotDelay0.Click += new System.EventHandler(this.tsmiScreenshotDelay0_Click);
            // 
            // tsmiTrayScreenshotDelay1
            // 
            this.tsmiTrayScreenshotDelay1.Name = "tsmiTrayScreenshotDelay1";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay1, "tsmiTrayScreenshotDelay1");
            this.tsmiTrayScreenshotDelay1.Click += new System.EventHandler(this.tsmiScreenshotDelay1_Click);
            // 
            // tsmiTrayScreenshotDelay2
            // 
            this.tsmiTrayScreenshotDelay2.Name = "tsmiTrayScreenshotDelay2";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay2, "tsmiTrayScreenshotDelay2");
            this.tsmiTrayScreenshotDelay2.Click += new System.EventHandler(this.tsmiScreenshotDelay2_Click);
            // 
            // tsmiTrayScreenshotDelay3
            // 
            this.tsmiTrayScreenshotDelay3.Name = "tsmiTrayScreenshotDelay3";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay3, "tsmiTrayScreenshotDelay3");
            this.tsmiTrayScreenshotDelay3.Click += new System.EventHandler(this.tsmiScreenshotDelay3_Click);
            // 
            // tsmiTrayScreenshotDelay4
            // 
            this.tsmiTrayScreenshotDelay4.Name = "tsmiTrayScreenshotDelay4";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay4, "tsmiTrayScreenshotDelay4");
            this.tsmiTrayScreenshotDelay4.Click += new System.EventHandler(this.tsmiScreenshotDelay4_Click);
            // 
            // tsmiTrayScreenshotDelay5
            // 
            this.tsmiTrayScreenshotDelay5.Name = "tsmiTrayScreenshotDelay5";
            resources.ApplyResources(this.tsmiTrayScreenshotDelay5, "tsmiTrayScreenshotDelay5");
            this.tsmiTrayScreenshotDelay5.Click += new System.EventHandler(this.tsmiScreenshotDelay5_Click);
            // 
            // tsmiTrayUpload
            // 
            this.tsmiTrayUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayUploadFile,
            this.tsmiTrayUploadFolder,
            this.tsmiTrayUploadClipboard,
            this.tsmiTrayUploadText,
            this.tsmiTrayUploadURL,
            this.tsmiTrayUploadDragDrop,
            this.tsmiTrayShortenURL,
            this.tsmiTrayTweetMessage});
            this.tsmiTrayUpload.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsmiTrayUpload.Name = "tsmiTrayUpload";
            resources.ApplyResources(this.tsmiTrayUpload, "tsmiTrayUpload");
            // 
            // tsmiTrayUploadFile
            // 
            this.tsmiTrayUploadFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiTrayUploadFile.Name = "tsmiTrayUploadFile";
            resources.ApplyResources(this.tsmiTrayUploadFile, "tsmiTrayUploadFile");
            this.tsmiTrayUploadFile.Click += new System.EventHandler(this.tsbFileUpload_Click);
            // 
            // tsmiTrayUploadFolder
            // 
            this.tsmiTrayUploadFolder.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiTrayUploadFolder.Name = "tsmiTrayUploadFolder";
            resources.ApplyResources(this.tsmiTrayUploadFolder, "tsmiTrayUploadFolder");
            this.tsmiTrayUploadFolder.Click += new System.EventHandler(this.tsmiUploadFolder_Click);
            // 
            // tsmiTrayUploadClipboard
            // 
            this.tsmiTrayUploadClipboard.Image = global::ShareX.Properties.Resources.clipboard;
            this.tsmiTrayUploadClipboard.Name = "tsmiTrayUploadClipboard";
            resources.ApplyResources(this.tsmiTrayUploadClipboard, "tsmiTrayUploadClipboard");
            this.tsmiTrayUploadClipboard.Click += new System.EventHandler(this.tsbClipboardUpload_Click);
            // 
            // tsmiTrayUploadText
            // 
            this.tsmiTrayUploadText.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTrayUploadText.Name = "tsmiTrayUploadText";
            resources.ApplyResources(this.tsmiTrayUploadText, "tsmiTrayUploadText");
            this.tsmiTrayUploadText.Click += new System.EventHandler(this.tsmiUploadText_Click);
            // 
            // tsmiTrayUploadURL
            // 
            this.tsmiTrayUploadURL.Image = global::ShareX.Properties.Resources.drive;
            this.tsmiTrayUploadURL.Name = "tsmiTrayUploadURL";
            resources.ApplyResources(this.tsmiTrayUploadURL, "tsmiTrayUploadURL");
            this.tsmiTrayUploadURL.Click += new System.EventHandler(this.tsmiUploadURL_Click);
            // 
            // tsmiTrayUploadDragDrop
            // 
            this.tsmiTrayUploadDragDrop.Image = global::ShareX.Properties.Resources.inbox;
            this.tsmiTrayUploadDragDrop.Name = "tsmiTrayUploadDragDrop";
            resources.ApplyResources(this.tsmiTrayUploadDragDrop, "tsmiTrayUploadDragDrop");
            this.tsmiTrayUploadDragDrop.Click += new System.EventHandler(this.tsbDragDropUpload_Click);
            // 
            // tsmiTrayShortenURL
            // 
            this.tsmiTrayShortenURL.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTrayShortenURL.Name = "tsmiTrayShortenURL";
            resources.ApplyResources(this.tsmiTrayShortenURL, "tsmiTrayShortenURL");
            this.tsmiTrayShortenURL.Click += new System.EventHandler(this.tsmiShortenURL_Click);
            // 
            // tsmiTrayTweetMessage
            // 
            this.tsmiTrayTweetMessage.Image = global::ShareX.Properties.Resources.X_black;
            this.tsmiTrayTweetMessage.Name = "tsmiTrayTweetMessage";
            resources.ApplyResources(this.tsmiTrayTweetMessage, "tsmiTrayTweetMessage");
            this.tsmiTrayTweetMessage.Click += new System.EventHandler(this.tsmiTweetMessage_Click);
            // 
            // tsmiTrayWorkflows
            // 
            this.tsmiTrayWorkflows.Image = global::ShareX.Properties.Resources.categories;
            this.tsmiTrayWorkflows.Name = "tsmiTrayWorkflows";
            resources.ApplyResources(this.tsmiTrayWorkflows, "tsmiTrayWorkflows");
            // 
            // tsmiTrayTools
            // 
            this.tsmiTrayTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayColorPicker,
            this.tsmiTrayScreenColorPicker,
            this.tsmiTrayRuler,
            this.tsmiTrayPinToScreen,
            this.tssTrayTools1,
            this.tsmiTrayImageEditor,
            this.tsmiTrayImageBeautifier,
            this.tsmiTrayImageEffects,
            this.tsmiTrayImageViewer,
            this.tsmiTrayImageCombiner,
            this.tsmiTrayImageSplitter,
            this.tsmiTrayImageThumbnailer,
            this.tssTrayTools2,
            this.tsmiTrayVideoConverter,
            this.tsmiTrayVideoThumbnailer,
            this.tssTrayTools3,
            this.tsmiTrayOCR,
            this.tsmiTrayQRCode,
            this.tsmiTrayHashChecker,
            this.tsmiTrayIndexFolder,
            this.tssTrayTools4,
            this.tsmiTrayClipboardViewer,
            this.tsmiTrayBorderlessWindow,
            this.tsmiTrayInspectWindow,
            this.tsmiTrayMonitorTest,
            this.tsmiTrayDNSChanger});
            this.tsmiTrayTools.Image = global::ShareX.Properties.Resources.toolbox;
            this.tsmiTrayTools.Name = "tsmiTrayTools";
            resources.ApplyResources(this.tsmiTrayTools, "tsmiTrayTools");
            // 
            // tsmiTrayColorPicker
            // 
            this.tsmiTrayColorPicker.Image = global::ShareX.Properties.Resources.color;
            this.tsmiTrayColorPicker.Name = "tsmiTrayColorPicker";
            resources.ApplyResources(this.tsmiTrayColorPicker, "tsmiTrayColorPicker");
            this.tsmiTrayColorPicker.Click += new System.EventHandler(this.tsmiColorPicker_Click);
            // 
            // tsmiTrayScreenColorPicker
            // 
            this.tsmiTrayScreenColorPicker.Image = global::ShareX.Properties.Resources.pipette;
            this.tsmiTrayScreenColorPicker.Name = "tsmiTrayScreenColorPicker";
            resources.ApplyResources(this.tsmiTrayScreenColorPicker, "tsmiTrayScreenColorPicker");
            this.tsmiTrayScreenColorPicker.Click += new System.EventHandler(this.tsmiScreenColorPicker_Click);
            // 
            // tsmiTrayRuler
            // 
            this.tsmiTrayRuler.Image = global::ShareX.Properties.Resources.ruler_triangle;
            this.tsmiTrayRuler.Name = "tsmiTrayRuler";
            resources.ApplyResources(this.tsmiTrayRuler, "tsmiTrayRuler");
            this.tsmiTrayRuler.Click += new System.EventHandler(this.tsmiRuler_Click);
            // 
            // tsmiTrayPinToScreen
            // 
            this.tsmiTrayPinToScreen.Image = global::ShareX.Properties.Resources.pin;
            this.tsmiTrayPinToScreen.Name = "tsmiTrayPinToScreen";
            resources.ApplyResources(this.tsmiTrayPinToScreen, "tsmiTrayPinToScreen");
            this.tsmiTrayPinToScreen.Click += new System.EventHandler(this.tsmiPinToScreen_Click);
            // 
            // tssTrayTools1
            // 
            this.tssTrayTools1.Name = "tssTrayTools1";
            resources.ApplyResources(this.tssTrayTools1, "tssTrayTools1");
            // 
            // tsmiTrayImageEditor
            // 
            this.tsmiTrayImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiTrayImageEditor.Name = "tsmiTrayImageEditor";
            resources.ApplyResources(this.tsmiTrayImageEditor, "tsmiTrayImageEditor");
            this.tsmiTrayImageEditor.Click += new System.EventHandler(this.tsmiImageEditor_Click);
            // 
            // tsmiTrayImageBeautifier
            // 
            this.tsmiTrayImageBeautifier.Image = global::ShareX.Properties.Resources.picture_sunset;
            this.tsmiTrayImageBeautifier.Name = "tsmiTrayImageBeautifier";
            resources.ApplyResources(this.tsmiTrayImageBeautifier, "tsmiTrayImageBeautifier");
            this.tsmiTrayImageBeautifier.Click += new System.EventHandler(this.tsmiImageBeautifier_Click);
            // 
            // tsmiTrayImageEffects
            // 
            this.tsmiTrayImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiTrayImageEffects.Name = "tsmiTrayImageEffects";
            resources.ApplyResources(this.tsmiTrayImageEffects, "tsmiTrayImageEffects");
            this.tsmiTrayImageEffects.Click += new System.EventHandler(this.tsmiImageEffects_Click);
            // 
            // tsmiTrayImageViewer
            // 
            this.tsmiTrayImageViewer.Image = global::ShareX.Properties.Resources.images_flickr;
            this.tsmiTrayImageViewer.Name = "tsmiTrayImageViewer";
            resources.ApplyResources(this.tsmiTrayImageViewer, "tsmiTrayImageViewer");
            this.tsmiTrayImageViewer.Click += new System.EventHandler(this.tsmiImageViewer_Click);
            // 
            // tsmiTrayImageCombiner
            // 
            this.tsmiTrayImageCombiner.Image = global::ShareX.Properties.Resources.document_break;
            this.tsmiTrayImageCombiner.Name = "tsmiTrayImageCombiner";
            resources.ApplyResources(this.tsmiTrayImageCombiner, "tsmiTrayImageCombiner");
            this.tsmiTrayImageCombiner.Click += new System.EventHandler(this.tsmiImageCombiner_Click);
            // 
            // tsmiTrayImageSplitter
            // 
            this.tsmiTrayImageSplitter.Image = global::ShareX.Properties.Resources.image_split;
            this.tsmiTrayImageSplitter.Name = "tsmiTrayImageSplitter";
            resources.ApplyResources(this.tsmiTrayImageSplitter, "tsmiTrayImageSplitter");
            this.tsmiTrayImageSplitter.Click += new System.EventHandler(this.TsmiImageSplitter_Click);
            // 
            // tsmiTrayImageThumbnailer
            // 
            this.tsmiTrayImageThumbnailer.Image = global::ShareX.Properties.Resources.image_resize_actual;
            this.tsmiTrayImageThumbnailer.Name = "tsmiTrayImageThumbnailer";
            resources.ApplyResources(this.tsmiTrayImageThumbnailer, "tsmiTrayImageThumbnailer");
            this.tsmiTrayImageThumbnailer.Click += new System.EventHandler(this.tsmiImageThumbnailer_Click);
            // 
            // tssTrayTools2
            // 
            this.tssTrayTools2.Name = "tssTrayTools2";
            resources.ApplyResources(this.tssTrayTools2, "tssTrayTools2");
            // 
            // tsmiTrayVideoConverter
            // 
            this.tsmiTrayVideoConverter.Image = global::ShareX.Properties.Resources.camcorder_pencil;
            this.tsmiTrayVideoConverter.Name = "tsmiTrayVideoConverter";
            resources.ApplyResources(this.tsmiTrayVideoConverter, "tsmiTrayVideoConverter");
            this.tsmiTrayVideoConverter.Click += new System.EventHandler(this.tsmiVideoConverter_Click);
            // 
            // tsmiTrayVideoThumbnailer
            // 
            this.tsmiTrayVideoThumbnailer.Image = global::ShareX.Properties.Resources.images_stack;
            this.tsmiTrayVideoThumbnailer.Name = "tsmiTrayVideoThumbnailer";
            resources.ApplyResources(this.tsmiTrayVideoThumbnailer, "tsmiTrayVideoThumbnailer");
            this.tsmiTrayVideoThumbnailer.Click += new System.EventHandler(this.tsmiVideoThumbnailer_Click);
            // 
            // tssTrayTools3
            // 
            this.tssTrayTools3.Name = "tssTrayTools3";
            resources.ApplyResources(this.tssTrayTools3, "tssTrayTools3");
            // 
            // tsmiTrayOCR
            // 
            this.tsmiTrayOCR.Image = global::ShareX.Properties.Resources.edit_drop_cap;
            this.tsmiTrayOCR.Name = "tsmiTrayOCR";
            resources.ApplyResources(this.tsmiTrayOCR, "tsmiTrayOCR");
            this.tsmiTrayOCR.Click += new System.EventHandler(this.tsmiTrayOCR_Click);
            // 
            // tsmiTrayQRCode
            // 
            this.tsmiTrayQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiTrayQRCode.Name = "tsmiTrayQRCode";
            resources.ApplyResources(this.tsmiTrayQRCode, "tsmiTrayQRCode");
            this.tsmiTrayQRCode.Click += new System.EventHandler(this.tsmiQRCode_Click);
            // 
            // tsmiTrayHashChecker
            // 
            this.tsmiTrayHashChecker.Image = global::ShareX.Properties.Resources.application_task;
            this.tsmiTrayHashChecker.Name = "tsmiTrayHashChecker";
            resources.ApplyResources(this.tsmiTrayHashChecker, "tsmiTrayHashChecker");
            this.tsmiTrayHashChecker.Click += new System.EventHandler(this.tsmiHashChecker_Click);
            // 
            // tsmiTrayIndexFolder
            // 
            this.tsmiTrayIndexFolder.Image = global::ShareX.Properties.Resources.folder_tree;
            this.tsmiTrayIndexFolder.Name = "tsmiTrayIndexFolder";
            resources.ApplyResources(this.tsmiTrayIndexFolder, "tsmiTrayIndexFolder");
            this.tsmiTrayIndexFolder.Click += new System.EventHandler(this.tsmiIndexFolder_Click);
            // 
            // tssTrayTools4
            // 
            this.tssTrayTools4.Name = "tssTrayTools4";
            resources.ApplyResources(this.tssTrayTools4, "tssTrayTools4");
            // 
            // tsmiTrayClipboardViewer
            // 
            this.tsmiTrayClipboardViewer.Image = global::ShareX.Properties.Resources.clipboard_block;
            this.tsmiTrayClipboardViewer.Name = "tsmiTrayClipboardViewer";
            resources.ApplyResources(this.tsmiTrayClipboardViewer, "tsmiTrayClipboardViewer");
            this.tsmiTrayClipboardViewer.Click += new System.EventHandler(this.tsmiClipboardViewer_Click);
            // 
            // tsmiTrayBorderlessWindow
            // 
            this.tsmiTrayBorderlessWindow.Image = global::ShareX.Properties.Resources.application_resize_full;
            this.tsmiTrayBorderlessWindow.Name = "tsmiTrayBorderlessWindow";
            resources.ApplyResources(this.tsmiTrayBorderlessWindow, "tsmiTrayBorderlessWindow");
            this.tsmiTrayBorderlessWindow.Click += new System.EventHandler(this.tsmiBorderlessWindow_Click);
            // 
            // tsmiTrayInspectWindow
            // 
            this.tsmiTrayInspectWindow.Image = global::ShareX.Properties.Resources.application_search_result;
            this.tsmiTrayInspectWindow.Name = "tsmiTrayInspectWindow";
            resources.ApplyResources(this.tsmiTrayInspectWindow, "tsmiTrayInspectWindow");
            this.tsmiTrayInspectWindow.Click += new System.EventHandler(this.tsmiInspectWindow_Click);
            // 
            // tsmiTrayMonitorTest
            // 
            this.tsmiTrayMonitorTest.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiTrayMonitorTest.Name = "tsmiTrayMonitorTest";
            resources.ApplyResources(this.tsmiTrayMonitorTest, "tsmiTrayMonitorTest");
            this.tsmiTrayMonitorTest.Click += new System.EventHandler(this.tsmiMonitorTest_Click);
            // 
            // tsmiTrayDNSChanger
            // 
            this.tsmiTrayDNSChanger.Image = global::ShareX.Properties.Resources.network_ip;
            this.tsmiTrayDNSChanger.Name = "tsmiTrayDNSChanger";
            resources.ApplyResources(this.tsmiTrayDNSChanger, "tsmiTrayDNSChanger");
            this.tsmiTrayDNSChanger.Click += new System.EventHandler(this.tsmiDNSChanger_Click);
            // 
            // tssTray1
            // 
            this.tssTray1.Name = "tssTray1";
            resources.ApplyResources(this.tssTray1, "tssTray1");
            // 
            // tsmiTrayAfterCaptureTasks
            // 
            this.tsmiTrayAfterCaptureTasks.Image = global::ShareX.Properties.Resources.image_export;
            this.tsmiTrayAfterCaptureTasks.Name = "tsmiTrayAfterCaptureTasks";
            resources.ApplyResources(this.tsmiTrayAfterCaptureTasks, "tsmiTrayAfterCaptureTasks");
            // 
            // tsmiTrayAfterUploadTasks
            // 
            this.tsmiTrayAfterUploadTasks.Image = global::ShareX.Properties.Resources.upload_cloud;
            this.tsmiTrayAfterUploadTasks.Name = "tsmiTrayAfterUploadTasks";
            resources.ApplyResources(this.tsmiTrayAfterUploadTasks, "tsmiTrayAfterUploadTasks");
            // 
            // tsmiTrayDestinations
            // 
            this.tsmiTrayDestinations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayImageUploaders,
            this.tsmiTrayTextUploaders,
            this.tsmiTrayFileUploaders,
            this.tsmiTrayURLShorteners,
            this.tsmiTrayURLSharingServices,
            this.tssTrayDestinations1,
            this.tsmiTrayDestinationSettings,
            this.tsmiTrayCustomUploaderSettings});
            this.tsmiTrayDestinations.Image = global::ShareX.Properties.Resources.drive_globe;
            this.tsmiTrayDestinations.Name = "tsmiTrayDestinations";
            resources.ApplyResources(this.tsmiTrayDestinations, "tsmiTrayDestinations");
            this.tsmiTrayDestinations.DropDownOpened += new System.EventHandler(this.tsddbDestinations_DropDownOpened);
            // 
            // tsmiTrayImageUploaders
            // 
            this.tsmiTrayImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiTrayImageUploaders.Name = "tsmiTrayImageUploaders";
            resources.ApplyResources(this.tsmiTrayImageUploaders, "tsmiTrayImageUploaders");
            // 
            // tsmiTrayTextUploaders
            // 
            this.tsmiTrayTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTrayTextUploaders.Name = "tsmiTrayTextUploaders";
            resources.ApplyResources(this.tsmiTrayTextUploaders, "tsmiTrayTextUploaders");
            // 
            // tsmiTrayFileUploaders
            // 
            this.tsmiTrayFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiTrayFileUploaders.Name = "tsmiTrayFileUploaders";
            resources.ApplyResources(this.tsmiTrayFileUploaders, "tsmiTrayFileUploaders");
            // 
            // tsmiTrayURLShorteners
            // 
            this.tsmiTrayURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTrayURLShorteners.Name = "tsmiTrayURLShorteners";
            resources.ApplyResources(this.tsmiTrayURLShorteners, "tsmiTrayURLShorteners");
            // 
            // tsmiTrayURLSharingServices
            // 
            this.tsmiTrayURLSharingServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiTrayURLSharingServices.Name = "tsmiTrayURLSharingServices";
            resources.ApplyResources(this.tsmiTrayURLSharingServices, "tsmiTrayURLSharingServices");
            // 
            // tssTrayDestinations1
            // 
            this.tssTrayDestinations1.Name = "tssTrayDestinations1";
            resources.ApplyResources(this.tssTrayDestinations1, "tssTrayDestinations1");
            // 
            // tsmiTrayDestinationSettings
            // 
            this.tsmiTrayDestinationSettings.Image = global::ShareX.Properties.Resources.globe_pencil;
            this.tsmiTrayDestinationSettings.Name = "tsmiTrayDestinationSettings";
            resources.ApplyResources(this.tsmiTrayDestinationSettings, "tsmiTrayDestinationSettings");
            this.tsmiTrayDestinationSettings.Click += new System.EventHandler(this.tsmiDestinationSettings_Click);
            // 
            // tsmiTrayCustomUploaderSettings
            // 
            this.tsmiTrayCustomUploaderSettings.Image = global::ShareX.Properties.Resources.network_cloud;
            this.tsmiTrayCustomUploaderSettings.Name = "tsmiTrayCustomUploaderSettings";
            resources.ApplyResources(this.tsmiTrayCustomUploaderSettings, "tsmiTrayCustomUploaderSettings");
            this.tsmiTrayCustomUploaderSettings.Click += new System.EventHandler(this.tsmiCustomUploaderSettings_Click);
            // 
            // tsmiTrayApplicationSettings
            // 
            this.tsmiTrayApplicationSettings.Image = global::ShareX.Properties.Resources.wrench_screwdriver;
            this.tsmiTrayApplicationSettings.Name = "tsmiTrayApplicationSettings";
            resources.ApplyResources(this.tsmiTrayApplicationSettings, "tsmiTrayApplicationSettings");
            this.tsmiTrayApplicationSettings.Click += new System.EventHandler(this.tsbApplicationSettings_Click);
            // 
            // tsmiTrayTaskSettings
            // 
            this.tsmiTrayTaskSettings.Image = global::ShareX.Properties.Resources.gear;
            this.tsmiTrayTaskSettings.Name = "tsmiTrayTaskSettings";
            resources.ApplyResources(this.tsmiTrayTaskSettings, "tsmiTrayTaskSettings");
            this.tsmiTrayTaskSettings.Click += new System.EventHandler(this.tsbTaskSettings_Click);
            // 
            // tsmiTrayHotkeySettings
            // 
            this.tsmiTrayHotkeySettings.Image = global::ShareX.Properties.Resources.keyboard;
            this.tsmiTrayHotkeySettings.Name = "tsmiTrayHotkeySettings";
            resources.ApplyResources(this.tsmiTrayHotkeySettings, "tsmiTrayHotkeySettings");
            this.tsmiTrayHotkeySettings.Click += new System.EventHandler(this.tsbHotkeySettings_Click);
            // 
            // tsmiTrayToggleHotkeys
            // 
            this.tsmiTrayToggleHotkeys.Image = global::ShareX.Properties.Resources.keyboard__minus;
            this.tsmiTrayToggleHotkeys.Name = "tsmiTrayToggleHotkeys";
            resources.ApplyResources(this.tsmiTrayToggleHotkeys, "tsmiTrayToggleHotkeys");
            this.tsmiTrayToggleHotkeys.Click += new System.EventHandler(this.tsmiTrayToggleHotkeys_Click);
            // 
            // tssTray2
            // 
            this.tssTray2.Name = "tssTray2";
            resources.ApplyResources(this.tssTray2, "tssTray2");
            // 
            // tsmiScreenshotsFolder
            // 
            this.tsmiScreenshotsFolder.Image = global::ShareX.Properties.Resources.folder_open_image;
            this.tsmiScreenshotsFolder.Name = "tsmiScreenshotsFolder";
            resources.ApplyResources(this.tsmiScreenshotsFolder, "tsmiScreenshotsFolder");
            this.tsmiScreenshotsFolder.Click += new System.EventHandler(this.tsbScreenshotsFolder_Click);
            // 
            // tsmiTrayHistory
            // 
            this.tsmiTrayHistory.Image = global::ShareX.Properties.Resources.application_blog;
            this.tsmiTrayHistory.Name = "tsmiTrayHistory";
            resources.ApplyResources(this.tsmiTrayHistory, "tsmiTrayHistory");
            this.tsmiTrayHistory.Click += new System.EventHandler(this.tsbHistory_Click);
            // 
            // tsmiTrayImageHistory
            // 
            this.tsmiTrayImageHistory.Image = global::ShareX.Properties.Resources.application_icon_large;
            this.tsmiTrayImageHistory.Name = "tsmiTrayImageHistory";
            resources.ApplyResources(this.tsmiTrayImageHistory, "tsmiTrayImageHistory");
            this.tsmiTrayImageHistory.Click += new System.EventHandler(this.tsbImageHistory_Click);
            // 
            // tssTray3
            // 
            this.tssTray3.Name = "tssTray3";
            resources.ApplyResources(this.tssTray3, "tssTray3");
            // 
            // tsmiRestartAsAdmin
            // 
            this.tsmiRestartAsAdmin.Image = global::ShareX.Properties.Resources.uac;
            this.tsmiRestartAsAdmin.Name = "tsmiRestartAsAdmin";
            resources.ApplyResources(this.tsmiRestartAsAdmin, "tsmiRestartAsAdmin");
            this.tsmiRestartAsAdmin.Click += new System.EventHandler(this.tsmiRestartAsAdmin_Click);
            // 
            // tsmiTrayRecentItems
            // 
            this.tsmiTrayRecentItems.Image = global::ShareX.Properties.Resources.clipboard_list;
            this.tsmiTrayRecentItems.Name = "tsmiTrayRecentItems";
            resources.ApplyResources(this.tsmiTrayRecentItems, "tsmiTrayRecentItems");
            // 
            // tsmiOpenActionsToolbar
            // 
            this.tsmiOpenActionsToolbar.Image = global::ShareX.Properties.Resources.ui_toolbar__arrow;
            this.tsmiOpenActionsToolbar.Name = "tsmiOpenActionsToolbar";
            resources.ApplyResources(this.tsmiOpenActionsToolbar, "tsmiOpenActionsToolbar");
            this.tsmiOpenActionsToolbar.Click += new System.EventHandler(this.tsmiOpenActionsToolbar_Click);
            // 
            // tsmiTrayShow
            // 
            this.tsmiTrayShow.Image = global::ShareX.Properties.Resources.tick_button;
            this.tsmiTrayShow.Name = "tsmiTrayShow";
            resources.ApplyResources(this.tsmiTrayShow, "tsmiTrayShow");
            this.tsmiTrayShow.Click += new System.EventHandler(this.tsmiTrayShow_Click);
            // 
            // tsmiTrayExit
            // 
            this.tsmiTrayExit.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiTrayExit.Name = "tsmiTrayExit";
            resources.ApplyResources(this.tsmiTrayExit, "tsmiTrayExit");
            this.tsmiTrayExit.Click += new System.EventHandler(this.tsmiTrayExit_Click);
            this.tsmiTrayExit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsmiTrayExit_MouseDown);
            // 
            // timerTraySingleClick
            // 
            this.timerTraySingleClick.Tick += new System.EventHandler(this.timerTraySingleClick_Tick);
            // 
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 5000;
            this.ttMain.InitialDelay = 200;
            this.ttMain.OwnerDraw = true;
            this.ttMain.ReshowDelay = 100;
            this.ttMain.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.TtMain_Draw);
            // 
            // pToolbars
            // 
            resources.ApplyResources(this.pToolbars, "pToolbars");
            this.pToolbars.Controls.Add(this.tsMain);
            this.pToolbars.Name = "pToolbars";
            // 
            // dgvHotkeys
            // 
            this.dgvHotkeys.AllowUserToAddRows = false;
            this.dgvHotkeys.AllowUserToDeleteRows = false;
            this.dgvHotkeys.AllowUserToResizeColumns = false;
            this.dgvHotkeys.AllowUserToResizeRows = false;
            this.dgvHotkeys.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvHotkeys.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHotkeys.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHotkeys.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvHotkeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHotkeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cHotkeyStatus,
            this.cHotkey,
            this.cDescription});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHotkeys.DefaultCellStyle = dataGridViewCellStyle12;
            resources.ApplyResources(this.dgvHotkeys, "dgvHotkeys");
            this.dgvHotkeys.MultiSelect = false;
            this.dgvHotkeys.Name = "dgvHotkeys";
            this.dgvHotkeys.ReadOnly = true;
            this.dgvHotkeys.RowHeadersVisible = false;
            this.dgvHotkeys.TabStop = false;
            this.dgvHotkeys.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgvHotkeys_MouseDoubleClick);
            this.dgvHotkeys.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvHotkeys_MouseUp);
            // 
            // cHotkeyStatus
            // 
            resources.ApplyResources(this.cHotkeyStatus, "cHotkeyStatus");
            this.cHotkeyStatus.Name = "cHotkeyStatus";
            this.cHotkeyStatus.ReadOnly = true;
            // 
            // cHotkey
            // 
            this.cHotkey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.cHotkey.DefaultCellStyle = dataGridViewCellStyle11;
            resources.ApplyResources(this.cHotkey, "cHotkey");
            this.cHotkey.Name = "cHotkey";
            this.cHotkey.ReadOnly = true;
            this.cHotkey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.cDescription, "cDescription");
            this.cDescription.Name = "cDescription";
            this.cDescription.ReadOnly = true;
            this.cDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pMain
            // 
            this.pMain.Controls.Add(this.pHotkeys);
            this.pMain.Controls.Add(this.ucTaskThumbnailView);
            this.pMain.Controls.Add(this.scMain);
            resources.ApplyResources(this.pMain, "pMain");
            this.pMain.Name = "pMain";
            // 
            // pHotkeys
            // 
            this.pHotkeys.Controls.Add(this.dgvHotkeys);
            resources.ApplyResources(this.pHotkeys, "pHotkeys");
            this.pHotkeys.Name = "pHotkeys";
            // 
            // ucTaskThumbnailView
            // 
            resources.ApplyResources(this.ucTaskThumbnailView, "ucTaskThumbnailView");
            this.ucTaskThumbnailView.BackColor = System.Drawing.SystemColors.Window;
            this.ucTaskThumbnailView.ClickAction = ShareX.ThumbnailViewClickAction.Default;
            this.ucTaskThumbnailView.Name = "ucTaskThumbnailView";
            this.ucTaskThumbnailView.ThumbnailSize = new System.Drawing.Size(200, 150);
            this.ucTaskThumbnailView.TitleLocation = ShareX.ThumbnailTitleLocation.Top;
            this.ucTaskThumbnailView.TitleVisible = true;
            this.ucTaskThumbnailView.ContextMenuRequested += new ShareX.TaskThumbnailView.TaskViewMouseEventHandler(this.UcTaskView_ContextMenuRequested);
            this.ucTaskThumbnailView.SelectedPanelChanged += new System.EventHandler(this.ucTaskThumbnailView_SelectedPanelChanged);
            this.ucTaskThumbnailView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvUploads_KeyDown);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pMain);
            this.Controls.Add(this.pToolbars);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.cmsTaskInfo.ResumeLayout(false);
            this.cmsTray.ResumeLayout(false);
            this.pToolbars.ResumeLayout(false);
            this.pToolbars.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotkeys)).EndInit();
            this.pMain.ResumeLayout(false);
            this.pHotkeys.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.ToolStripMenuItem tsmiDNSChanger;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayDNSChanger;
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
        private System.Windows.Forms.ToolStripSeparator tssDestinations1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDestinationSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadClipboard;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadURL;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadDragDrop;
        private System.Windows.Forms.ToolStripSeparator tssTrayDestinations1;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayDestinationSettings;
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
        private System.Windows.Forms.ToolStripMenuItem tsmiCustomUploaderSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayCustomUploaderSettings;
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
        private System.Windows.Forms.ToolStripMenuItem tsmiTweetMessage;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTweetMessage;
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
    }
}