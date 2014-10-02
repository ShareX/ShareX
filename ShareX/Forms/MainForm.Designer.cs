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
            this.scMain = new HelpersLib.SplitContainerCustomSplitter();
            this.pBackground = new System.Windows.Forms.Panel();
            this.pbLogo = new HelpersLib.MyPictureBox();
            this.lblDragAndDropTip = new System.Windows.Forms.Label();
            this.lblSplitter = new System.Windows.Forms.Label();
            this.lvUploads = new HelpersLib.MyListView();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chElapsed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chRemaining = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbPreview = new HelpersLib.MyPictureBox();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsddbCapture = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWindowRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRectangleAnnotate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRectangleLight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRoundedRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEllipse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTriangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDiamond = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFreeHand = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLastRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenRecordingFFmpeg = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScreenRecordingGIF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAutoCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddbUpload = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiUploadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUploadDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddbWorkflows = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsddbTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiScreenColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHashCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDNSChanger = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFTPClient = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTweetMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tsbApplicationSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbTaskSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbHotkeySettings = new System.Windows.Forms.ToolStripButton();
            this.tssMain2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbScreenshotsFolder = new System.Windows.Forms.ToolStripButton();
            this.tsbHistory = new System.Windows.Forms.ToolStripButton();
            this.tsbImageHistory = new System.Windows.Forms.ToolStripButton();
            this.tsddbDebug = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiShowDebugLog = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestImageUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestTextUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestFileUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestURLShortener = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestURLSharing = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTestUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDonate = new System.Windows.Forms.ToolStripButton();
            this.tsmiAbout = new System.Windows.Forms.ToolStripButton();
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
            this.tsmiCopyFilePath = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFileNameWithExtension = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCopy5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiUploadSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteSelectedFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShortenSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShareSelectedURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowResponse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearList = new System.Windows.Forms.ToolStripMenuItem();
            this.tssUploadInfo1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImagePreview = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImagePreviewShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImagePreviewHide = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImagePreviewAutomatic = new System.Windows.Forms.ToolStripMenuItem();
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiTrayCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayWindowRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRectangleAnnotate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRectangleLight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRoundedRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayEllipse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTriangle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayDiamond = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayFreeHand = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayLastRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.screenRecordingFFmpegToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenRecordingGIF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayAutoCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayUploadDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayWorkflows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayScreenColorPicker = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHashCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayDNSChanger = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayFTPClient = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTweetMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tsmiTrayApplicationSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTaskSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHotkeySettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTray2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiScreenshotsFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayDonate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tssTray3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTrayShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ssToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.pBackground.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.cmsTaskInfo.SuspendLayout();
            this.cmsTray.SuspendLayout();
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
            resources.ApplyResources(this.scMain.Panel1, "scMain.Panel1");
            this.scMain.Panel1.Controls.Add(this.pBackground);
            this.scMain.Panel1.Controls.Add(this.lblSplitter);
            this.scMain.Panel1.Controls.Add(this.lvUploads);
            // 
            // scMain.Panel2
            // 
            resources.ApplyResources(this.scMain.Panel2, "scMain.Panel2");
            this.scMain.Panel2.Controls.Add(this.pbPreview);
            this.scMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.scMain_SplitterMoved);
            // 
            // pBackground
            // 
            resources.ApplyResources(this.pBackground, "pBackground");
            this.pBackground.BackColor = System.Drawing.Color.White;
            this.pBackground.Controls.Add(this.pbLogo);
            this.pBackground.Controls.Add(this.lblDragAndDropTip);
            this.pBackground.Name = "pBackground";
            // 
            // pbLogo
            // 
            resources.ApplyResources(this.pbLogo, "pbLogo");
            this.pbLogo.BackColor = System.Drawing.Color.White;
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.TabStop = false;
            this.pbLogo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDragAndDropTip_MouseUp);
            // 
            // lblDragAndDropTip
            // 
            resources.ApplyResources(this.lblDragAndDropTip, "lblDragAndDropTip");
            this.lblDragAndDropTip.BackColor = System.Drawing.Color.White;
            this.lblDragAndDropTip.ForeColor = System.Drawing.Color.Silver;
            this.lblDragAndDropTip.Name = "lblDragAndDropTip";
            this.lblDragAndDropTip.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDragAndDropTip_MouseUp);
            // 
            // lblSplitter
            // 
            resources.ApplyResources(this.lblSplitter, "lblSplitter");
            this.lblSplitter.BackColor = System.Drawing.Color.Black;
            this.lblSplitter.Name = "lblSplitter";
            // 
            // lvUploads
            // 
            resources.ApplyResources(this.lvUploads, "lvUploads");
            this.lvUploads.AutoFillColumn = true;
            this.lvUploads.BackColor = System.Drawing.Color.White;
            this.lvUploads.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvUploads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chStatus,
            this.chProgress,
            this.chSpeed,
            this.chElapsed,
            this.chRemaining,
            this.chURL});
            this.lvUploads.FullRowSelect = true;
            this.lvUploads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUploads.HideSelection = false;
            this.lvUploads.Name = "lvUploads";
            this.lvUploads.ShowItemToolTips = true;
            this.lvUploads.UseCompatibleStateImageBehavior = false;
            this.lvUploads.View = System.Windows.Forms.View.Details;
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
            resources.ApplyResources(this.pbPreview, "pbPreview");
            this.pbPreview.BackColor = System.Drawing.Color.White;
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.EnableRightClickMenu = true;
            this.pbPreview.FullscreenOnClick = true;
            this.pbPreview.Name = "pbPreview";
            // 
            // tsMain
            // 
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.CanOverflow = false;
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
            this.tsddbDebug,
            this.tsmiDonate,
            this.tsmiAbout});
            this.tsMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.tsMain.Name = "tsMain";
            this.tsMain.ShowItemToolTips = false;
            this.tsMain.TabStop = true;
            // 
            // tsddbCapture
            // 
            resources.ApplyResources(this.tsddbCapture, "tsddbCapture");
            this.tsddbCapture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFullscreen,
            this.tsmiWindow,
            this.tsmiMonitor,
            this.tsmiRectangle,
            this.tsmiWindowRectangle,
            this.tsmiRectangleAnnotate,
            this.tsmiRectangleLight,
            this.tsmiRoundedRectangle,
            this.tsmiEllipse,
            this.tsmiTriangle,
            this.tsmiDiamond,
            this.tsmiPolygon,
            this.tsmiFreeHand,
            this.tsmiLastRegion,
            this.tsmiScreenRecordingFFmpeg,
            this.tsmiScreenRecordingGIF,
            this.tsmiAutoCapture});
            this.tsddbCapture.Image = global::ShareX.Properties.Resources.camera;
            this.tsddbCapture.Name = "tsddbCapture";
            this.tsddbCapture.DropDownOpening += new System.EventHandler(this.tsddbCapture_DropDownOpening);
            // 
            // tsmiFullscreen
            // 
            resources.ApplyResources(this.tsmiFullscreen, "tsmiFullscreen");
            this.tsmiFullscreen.Image = global::ShareX.Properties.Resources.layer;
            this.tsmiFullscreen.Name = "tsmiFullscreen";
            this.tsmiFullscreen.Click += new System.EventHandler(this.tsmiFullscreen_Click);
            // 
            // tsmiWindow
            // 
            resources.ApplyResources(this.tsmiWindow, "tsmiWindow");
            this.tsmiWindow.Image = global::ShareX.Properties.Resources.application_blue;
            this.tsmiWindow.Name = "tsmiWindow";
            // 
            // tsmiMonitor
            // 
            resources.ApplyResources(this.tsmiMonitor, "tsmiMonitor");
            this.tsmiMonitor.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiMonitor.Name = "tsmiMonitor";
            // 
            // tsmiRectangle
            // 
            resources.ApplyResources(this.tsmiRectangle, "tsmiRectangle");
            this.tsmiRectangle.Image = global::ShareX.Properties.Resources.layer_shape;
            this.tsmiRectangle.Name = "tsmiRectangle";
            this.tsmiRectangle.Click += new System.EventHandler(this.tsmiRectangle_Click);
            // 
            // tsmiWindowRectangle
            // 
            resources.ApplyResources(this.tsmiWindowRectangle, "tsmiWindowRectangle");
            this.tsmiWindowRectangle.Image = global::ShareX.Properties.Resources.layers_ungroup;
            this.tsmiWindowRectangle.Name = "tsmiWindowRectangle";
            this.tsmiWindowRectangle.Click += new System.EventHandler(this.tsmiWindowRectangle_Click);
            // 
            // tsmiRectangleAnnotate
            // 
            resources.ApplyResources(this.tsmiRectangleAnnotate, "tsmiRectangleAnnotate");
            this.tsmiRectangleAnnotate.Image = global::ShareX.Properties.Resources.layer_pencil;
            this.tsmiRectangleAnnotate.Name = "tsmiRectangleAnnotate";
            this.tsmiRectangleAnnotate.Click += new System.EventHandler(this.tsmiRectangleAnnotate_Click);
            // 
            // tsmiRectangleLight
            // 
            resources.ApplyResources(this.tsmiRectangleLight, "tsmiRectangleLight");
            this.tsmiRectangleLight.Image = global::ShareX.Properties.Resources.Rectangle;
            this.tsmiRectangleLight.Name = "tsmiRectangleLight";
            this.tsmiRectangleLight.Click += new System.EventHandler(this.tsmiRectangleLight_Click);
            // 
            // tsmiRoundedRectangle
            // 
            resources.ApplyResources(this.tsmiRoundedRectangle, "tsmiRoundedRectangle");
            this.tsmiRoundedRectangle.Image = global::ShareX.Properties.Resources.layer_shape_round;
            this.tsmiRoundedRectangle.Name = "tsmiRoundedRectangle";
            this.tsmiRoundedRectangle.Click += new System.EventHandler(this.tsmiRoundedRectangle_Click);
            // 
            // tsmiEllipse
            // 
            resources.ApplyResources(this.tsmiEllipse, "tsmiEllipse");
            this.tsmiEllipse.Image = global::ShareX.Properties.Resources.layer_shape_ellipse;
            this.tsmiEllipse.Name = "tsmiEllipse";
            this.tsmiEllipse.Click += new System.EventHandler(this.tsmiEllipse_Click);
            // 
            // tsmiTriangle
            // 
            resources.ApplyResources(this.tsmiTriangle, "tsmiTriangle");
            this.tsmiTriangle.Image = global::ShareX.Properties.Resources.Triangle;
            this.tsmiTriangle.Name = "tsmiTriangle";
            this.tsmiTriangle.Click += new System.EventHandler(this.tsmiTriangle_Click);
            // 
            // tsmiDiamond
            // 
            resources.ApplyResources(this.tsmiDiamond, "tsmiDiamond");
            this.tsmiDiamond.Image = global::ShareX.Properties.Resources.Diamond;
            this.tsmiDiamond.Name = "tsmiDiamond";
            this.tsmiDiamond.Click += new System.EventHandler(this.tsmiDiamond_Click);
            // 
            // tsmiPolygon
            // 
            resources.ApplyResources(this.tsmiPolygon, "tsmiPolygon");
            this.tsmiPolygon.Image = global::ShareX.Properties.Resources.layer_shape_polygon;
            this.tsmiPolygon.Name = "tsmiPolygon";
            this.tsmiPolygon.Click += new System.EventHandler(this.tsmiPolygon_Click);
            // 
            // tsmiFreeHand
            // 
            resources.ApplyResources(this.tsmiFreeHand, "tsmiFreeHand");
            this.tsmiFreeHand.Image = global::ShareX.Properties.Resources.layer_shape_curve;
            this.tsmiFreeHand.Name = "tsmiFreeHand";
            this.tsmiFreeHand.Click += new System.EventHandler(this.tsmiFreeHand_Click);
            // 
            // tsmiLastRegion
            // 
            resources.ApplyResources(this.tsmiLastRegion, "tsmiLastRegion");
            this.tsmiLastRegion.Image = global::ShareX.Properties.Resources.layers_arrange;
            this.tsmiLastRegion.Name = "tsmiLastRegion";
            this.tsmiLastRegion.Click += new System.EventHandler(this.tsmiLastRegion_Click);
            // 
            // tsmiScreenRecordingFFmpeg
            // 
            resources.ApplyResources(this.tsmiScreenRecordingFFmpeg, "tsmiScreenRecordingFFmpeg");
            this.tsmiScreenRecordingFFmpeg.Image = global::ShareX.Properties.Resources.camcorder_image;
            this.tsmiScreenRecordingFFmpeg.Name = "tsmiScreenRecordingFFmpeg";
            this.tsmiScreenRecordingFFmpeg.Click += new System.EventHandler(this.tsmiScreenRecordingFFmpeg_Click);
            // 
            // tsmiScreenRecordingGIF
            // 
            resources.ApplyResources(this.tsmiScreenRecordingGIF, "tsmiScreenRecordingGIF");
            this.tsmiScreenRecordingGIF.Image = global::ShareX.Properties.Resources.film;
            this.tsmiScreenRecordingGIF.Name = "tsmiScreenRecordingGIF";
            this.tsmiScreenRecordingGIF.Click += new System.EventHandler(this.tsmiScreenRecordingGIF_Click);
            // 
            // tsmiAutoCapture
            // 
            resources.ApplyResources(this.tsmiAutoCapture, "tsmiAutoCapture");
            this.tsmiAutoCapture.Image = global::ShareX.Properties.Resources.clock;
            this.tsmiAutoCapture.Name = "tsmiAutoCapture";
            this.tsmiAutoCapture.Click += new System.EventHandler(this.tsmiAutoCapture_Click);
            // 
            // tsddbUpload
            // 
            resources.ApplyResources(this.tsddbUpload, "tsddbUpload");
            this.tsddbUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUploadFile,
            this.tsmiUploadFolder,
            this.tsmiUploadClipboard,
            this.tsmiUploadURL,
            this.tsmiUploadDragDrop});
            this.tsddbUpload.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsddbUpload.Name = "tsddbUpload";
            // 
            // tsmiUploadFile
            // 
            resources.ApplyResources(this.tsmiUploadFile, "tsmiUploadFile");
            this.tsmiUploadFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiUploadFile.Name = "tsmiUploadFile";
            this.tsmiUploadFile.Click += new System.EventHandler(this.tsbFileUpload_Click);
            // 
            // tsmiUploadFolder
            // 
            resources.ApplyResources(this.tsmiUploadFolder, "tsmiUploadFolder");
            this.tsmiUploadFolder.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiUploadFolder.Name = "tsmiUploadFolder";
            this.tsmiUploadFolder.Click += new System.EventHandler(this.tsmiUploadFolder_Click);
            // 
            // tsmiUploadClipboard
            // 
            resources.ApplyResources(this.tsmiUploadClipboard, "tsmiUploadClipboard");
            this.tsmiUploadClipboard.Image = global::ShareX.Properties.Resources.clipboard;
            this.tsmiUploadClipboard.Name = "tsmiUploadClipboard";
            this.tsmiUploadClipboard.Click += new System.EventHandler(this.tsbClipboardUpload_Click);
            // 
            // tsmiUploadURL
            // 
            resources.ApplyResources(this.tsmiUploadURL, "tsmiUploadURL");
            this.tsmiUploadURL.Image = global::ShareX.Properties.Resources.drive;
            this.tsmiUploadURL.Name = "tsmiUploadURL";
            this.tsmiUploadURL.Click += new System.EventHandler(this.tsmiUploadURL_Click);
            // 
            // tsmiUploadDragDrop
            // 
            resources.ApplyResources(this.tsmiUploadDragDrop, "tsmiUploadDragDrop");
            this.tsmiUploadDragDrop.Image = global::ShareX.Properties.Resources.inbox;
            this.tsmiUploadDragDrop.Name = "tsmiUploadDragDrop";
            this.tsmiUploadDragDrop.Click += new System.EventHandler(this.tsbDragDropUpload_Click);
            // 
            // tsddbWorkflows
            // 
            resources.ApplyResources(this.tsddbWorkflows, "tsddbWorkflows");
            this.tsddbWorkflows.Image = global::ShareX.Properties.Resources.categories;
            this.tsddbWorkflows.Name = "tsddbWorkflows";
            // 
            // tsddbTools
            // 
            resources.ApplyResources(this.tsddbTools, "tsddbTools");
            this.tsddbTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScreenColorPicker,
            this.tsmiImageEditor,
            this.tsmiImageEffects,
            this.tsmiHashCheck,
            this.tsmiDNSChanger,
            this.tsmiQRCode,
            this.tsmiIndexFolder,
            this.tsmiRuler,
            this.tsmiFTPClient,
            this.tsmiTweetMessage,
            this.tsmiMonitorTest});
            this.tsddbTools.Image = global::ShareX.Properties.Resources.toolbox;
            this.tsddbTools.Name = "tsddbTools";
            // 
            // tsmiScreenColorPicker
            // 
            resources.ApplyResources(this.tsmiScreenColorPicker, "tsmiScreenColorPicker");
            this.tsmiScreenColorPicker.Image = global::ShareX.Properties.Resources.color;
            this.tsmiScreenColorPicker.Name = "tsmiScreenColorPicker";
            this.tsmiScreenColorPicker.Click += new System.EventHandler(this.tsmiCursorHelper_Click);
            // 
            // tsmiImageEditor
            // 
            resources.ApplyResources(this.tsmiImageEditor, "tsmiImageEditor");
            this.tsmiImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiImageEditor.Name = "tsmiImageEditor";
            this.tsmiImageEditor.Click += new System.EventHandler(this.tsmiImageEditor_Click);
            // 
            // tsmiImageEffects
            // 
            resources.ApplyResources(this.tsmiImageEffects, "tsmiImageEffects");
            this.tsmiImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiImageEffects.Name = "tsmiImageEffects";
            this.tsmiImageEffects.Click += new System.EventHandler(this.tsmiImageEffects_Click);
            // 
            // tsmiHashCheck
            // 
            resources.ApplyResources(this.tsmiHashCheck, "tsmiHashCheck");
            this.tsmiHashCheck.Image = global::ShareX.Properties.Resources.application_task;
            this.tsmiHashCheck.Name = "tsmiHashCheck";
            this.tsmiHashCheck.Click += new System.EventHandler(this.tsmiHashCheck_Click);
            // 
            // tsmiDNSChanger
            // 
            resources.ApplyResources(this.tsmiDNSChanger, "tsmiDNSChanger");
            this.tsmiDNSChanger.Image = global::ShareX.Properties.Resources.network_ip;
            this.tsmiDNSChanger.Name = "tsmiDNSChanger";
            this.tsmiDNSChanger.Click += new System.EventHandler(this.tsmiDNSChanger_Click);
            // 
            // tsmiQRCode
            // 
            resources.ApplyResources(this.tsmiQRCode, "tsmiQRCode");
            this.tsmiQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiQRCode.Name = "tsmiQRCode";
            this.tsmiQRCode.Click += new System.EventHandler(this.tsmiQRCode_Click);
            // 
            // tsmiIndexFolder
            // 
            resources.ApplyResources(this.tsmiIndexFolder, "tsmiIndexFolder");
            this.tsmiIndexFolder.Image = global::ShareX.Properties.Resources.folder_tree;
            this.tsmiIndexFolder.Name = "tsmiIndexFolder";
            this.tsmiIndexFolder.Click += new System.EventHandler(this.tsmiIndexFolder_Click);
            // 
            // tsmiRuler
            // 
            resources.ApplyResources(this.tsmiRuler, "tsmiRuler");
            this.tsmiRuler.Image = global::ShareX.Properties.Resources.ruler_triangle;
            this.tsmiRuler.Name = "tsmiRuler";
            this.tsmiRuler.Click += new System.EventHandler(this.tsmiRuler_Click);
            // 
            // tsmiFTPClient
            // 
            resources.ApplyResources(this.tsmiFTPClient, "tsmiFTPClient");
            this.tsmiFTPClient.Image = global::ShareX.Properties.Resources.application_network;
            this.tsmiFTPClient.Name = "tsmiFTPClient";
            this.tsmiFTPClient.Click += new System.EventHandler(this.tsmiFTPClient_Click);
            // 
            // tsmiTweetMessage
            // 
            resources.ApplyResources(this.tsmiTweetMessage, "tsmiTweetMessage");
            this.tsmiTweetMessage.Image = global::ShareX.Properties.Resources.Twitter;
            this.tsmiTweetMessage.Name = "tsmiTweetMessage";
            this.tsmiTweetMessage.Click += new System.EventHandler(this.tsmiTweetMessage_Click);
            // 
            // tsmiMonitorTest
            // 
            resources.ApplyResources(this.tsmiMonitorTest, "tsmiMonitorTest");
            this.tsmiMonitorTest.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiMonitorTest.Name = "tsmiMonitorTest";
            this.tsmiMonitorTest.Click += new System.EventHandler(this.tsmiMonitorTest_Click);
            // 
            // tssMain1
            // 
            resources.ApplyResources(this.tssMain1, "tssMain1");
            this.tssMain1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tssMain1.Name = "tssMain1";
            // 
            // tsddbAfterCaptureTasks
            // 
            resources.ApplyResources(this.tsddbAfterCaptureTasks, "tsddbAfterCaptureTasks");
            this.tsddbAfterCaptureTasks.Image = global::ShareX.Properties.Resources.image_export;
            this.tsddbAfterCaptureTasks.Name = "tsddbAfterCaptureTasks";
            // 
            // tsddbAfterUploadTasks
            // 
            resources.ApplyResources(this.tsddbAfterUploadTasks, "tsddbAfterUploadTasks");
            this.tsddbAfterUploadTasks.Image = global::ShareX.Properties.Resources.upload_cloud;
            this.tsddbAfterUploadTasks.Name = "tsddbAfterUploadTasks";
            // 
            // tsddbDestinations
            // 
            resources.ApplyResources(this.tsddbDestinations, "tsddbDestinations");
            this.tsddbDestinations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImageUploaders,
            this.tsmiTextUploaders,
            this.tsmiFileUploaders,
            this.tsmiURLShorteners,
            this.tsmiURLSharingServices,
            this.tssDestinations1,
            this.tsmiDestinationSettings});
            this.tsddbDestinations.Image = global::ShareX.Properties.Resources.drive_globe;
            this.tsddbDestinations.Name = "tsddbDestinations";
            this.tsddbDestinations.DropDownOpened += new System.EventHandler(this.tsddbDestinations_DropDownOpened);
            // 
            // tsmiImageUploaders
            // 
            resources.ApplyResources(this.tsmiImageUploaders, "tsmiImageUploaders");
            this.tsmiImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiImageUploaders.Name = "tsmiImageUploaders";
            // 
            // tsmiTextUploaders
            // 
            resources.ApplyResources(this.tsmiTextUploaders, "tsmiTextUploaders");
            this.tsmiTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTextUploaders.Name = "tsmiTextUploaders";
            // 
            // tsmiFileUploaders
            // 
            resources.ApplyResources(this.tsmiFileUploaders, "tsmiFileUploaders");
            this.tsmiFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiFileUploaders.Name = "tsmiFileUploaders";
            // 
            // tsmiURLShorteners
            // 
            resources.ApplyResources(this.tsmiURLShorteners, "tsmiURLShorteners");
            this.tsmiURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiURLShorteners.Name = "tsmiURLShorteners";
            // 
            // tsmiURLSharingServices
            // 
            resources.ApplyResources(this.tsmiURLSharingServices, "tsmiURLSharingServices");
            this.tsmiURLSharingServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiURLSharingServices.Name = "tsmiURLSharingServices";
            // 
            // tssDestinations1
            // 
            resources.ApplyResources(this.tssDestinations1, "tssDestinations1");
            this.tssDestinations1.Name = "tssDestinations1";
            // 
            // tsmiDestinationSettings
            // 
            resources.ApplyResources(this.tsmiDestinationSettings, "tsmiDestinationSettings");
            this.tsmiDestinationSettings.Image = global::ShareX.Properties.Resources.globe_pencil;
            this.tsmiDestinationSettings.Name = "tsmiDestinationSettings";
            this.tsmiDestinationSettings.Click += new System.EventHandler(this.tsbDestinationSettings_Click);
            // 
            // tsbApplicationSettings
            // 
            resources.ApplyResources(this.tsbApplicationSettings, "tsbApplicationSettings");
            this.tsbApplicationSettings.Image = global::ShareX.Properties.Resources.wrench_screwdriver;
            this.tsbApplicationSettings.Name = "tsbApplicationSettings";
            this.tsbApplicationSettings.Click += new System.EventHandler(this.tsbApplicationSettings_Click);
            // 
            // tsbTaskSettings
            // 
            resources.ApplyResources(this.tsbTaskSettings, "tsbTaskSettings");
            this.tsbTaskSettings.Image = global::ShareX.Properties.Resources.gear;
            this.tsbTaskSettings.Name = "tsbTaskSettings";
            this.tsbTaskSettings.Click += new System.EventHandler(this.tsbTaskSettings_Click);
            // 
            // tsbHotkeySettings
            // 
            resources.ApplyResources(this.tsbHotkeySettings, "tsbHotkeySettings");
            this.tsbHotkeySettings.Image = global::ShareX.Properties.Resources.keyboard;
            this.tsbHotkeySettings.Name = "tsbHotkeySettings";
            this.tsbHotkeySettings.Click += new System.EventHandler(this.tsbHotkeySettings_Click);
            // 
            // tssMain2
            // 
            resources.ApplyResources(this.tssMain2, "tssMain2");
            this.tssMain2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tssMain2.Name = "tssMain2";
            // 
            // tsbScreenshotsFolder
            // 
            resources.ApplyResources(this.tsbScreenshotsFolder, "tsbScreenshotsFolder");
            this.tsbScreenshotsFolder.Image = global::ShareX.Properties.Resources.folder_open_image;
            this.tsbScreenshotsFolder.Name = "tsbScreenshotsFolder";
            this.tsbScreenshotsFolder.Click += new System.EventHandler(this.tsbScreenshotsFolder_Click);
            // 
            // tsbHistory
            // 
            resources.ApplyResources(this.tsbHistory, "tsbHistory");
            this.tsbHistory.Image = global::ShareX.Properties.Resources.application_blog;
            this.tsbHistory.Name = "tsbHistory";
            this.tsbHistory.Click += new System.EventHandler(this.tsbHistory_Click);
            // 
            // tsbImageHistory
            // 
            resources.ApplyResources(this.tsbImageHistory, "tsbImageHistory");
            this.tsbImageHistory.Image = global::ShareX.Properties.Resources.application_icon_large;
            this.tsbImageHistory.Name = "tsbImageHistory";
            this.tsbImageHistory.Click += new System.EventHandler(this.tsbImageHistory_Click);
            // 
            // tsddbDebug
            // 
            resources.ApplyResources(this.tsddbDebug, "tsddbDebug");
            this.tsddbDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowDebugLog,
            this.tsmiTestImageUpload,
            this.tsmiTestTextUpload,
            this.tsmiTestFileUpload,
            this.tsmiTestURLShortener,
            this.tsmiTestURLSharing,
            this.tsmiTestUploaders});
            this.tsddbDebug.Image = global::ShareX.Properties.Resources.traffic_cone;
            this.tsddbDebug.Name = "tsddbDebug";
            // 
            // tsmiShowDebugLog
            // 
            resources.ApplyResources(this.tsmiShowDebugLog, "tsmiShowDebugLog");
            this.tsmiShowDebugLog.Image = global::ShareX.Properties.Resources.application_monitor;
            this.tsmiShowDebugLog.Name = "tsmiShowDebugLog";
            this.tsmiShowDebugLog.Click += new System.EventHandler(this.tsmiShowDebugLog_Click);
            // 
            // tsmiTestImageUpload
            // 
            resources.ApplyResources(this.tsmiTestImageUpload, "tsmiTestImageUpload");
            this.tsmiTestImageUpload.Image = global::ShareX.Properties.Resources.image;
            this.tsmiTestImageUpload.Name = "tsmiTestImageUpload";
            this.tsmiTestImageUpload.Click += new System.EventHandler(this.tsmiTestImageUpload_Click);
            // 
            // tsmiTestTextUpload
            // 
            resources.ApplyResources(this.tsmiTestTextUpload, "tsmiTestTextUpload");
            this.tsmiTestTextUpload.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTestTextUpload.Name = "tsmiTestTextUpload";
            this.tsmiTestTextUpload.Click += new System.EventHandler(this.tsmiTestTextUpload_Click);
            // 
            // tsmiTestFileUpload
            // 
            resources.ApplyResources(this.tsmiTestFileUpload, "tsmiTestFileUpload");
            this.tsmiTestFileUpload.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiTestFileUpload.Name = "tsmiTestFileUpload";
            this.tsmiTestFileUpload.Click += new System.EventHandler(this.tsmiTestFileUpload_Click);
            // 
            // tsmiTestURLShortener
            // 
            resources.ApplyResources(this.tsmiTestURLShortener, "tsmiTestURLShortener");
            this.tsmiTestURLShortener.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTestURLShortener.Name = "tsmiTestURLShortener";
            this.tsmiTestURLShortener.Click += new System.EventHandler(this.tsmiTestURLShortener_Click);
            // 
            // tsmiTestURLSharing
            // 
            resources.ApplyResources(this.tsmiTestURLSharing, "tsmiTestURLSharing");
            this.tsmiTestURLSharing.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiTestURLSharing.Name = "tsmiTestURLSharing";
            this.tsmiTestURLSharing.Click += new System.EventHandler(this.tsmiTestURLSharing_Click);
            // 
            // tsmiTestUploaders
            // 
            resources.ApplyResources(this.tsmiTestUploaders, "tsmiTestUploaders");
            this.tsmiTestUploaders.Image = global::ShareX.Properties.Resources.application_browser;
            this.tsmiTestUploaders.Name = "tsmiTestUploaders";
            this.tsmiTestUploaders.Click += new System.EventHandler(this.tsmiTestUploaders_Click);
            // 
            // tsmiDonate
            // 
            resources.ApplyResources(this.tsmiDonate, "tsmiDonate");
            this.tsmiDonate.Image = global::ShareX.Properties.Resources.heart;
            this.tsmiDonate.Name = "tsmiDonate";
            this.tsmiDonate.Click += new System.EventHandler(this.tsbDonate_Click);
            // 
            // tsmiAbout
            // 
            resources.ApplyResources(this.tsmiAbout, "tsmiAbout");
            this.tsmiAbout.Image = global::ShareX.Properties.Resources.crown;
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // cmsTaskInfo
            // 
            resources.ApplyResources(this.cmsTaskInfo, "cmsTaskInfo");
            this.cmsTaskInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowErrors,
            this.tsmiStopUpload,
            this.tsmiOpen,
            this.tsmiCopy,
            this.tsmiUploadSelectedFile,
            this.tsmiEditSelectedFile,
            this.tsmiDeleteSelectedFile,
            this.tsmiShortenSelectedURL,
            this.tsmiShareSelectedURL,
            this.tsmiShowQRCode,
            this.tsmiShowResponse,
            this.tsmiClearList,
            this.tssUploadInfo1,
            this.tsmiHideMenu,
            this.tsmiImagePreview});
            this.cmsTaskInfo.Name = "cmsHistory";
            // 
            // tsmiShowErrors
            // 
            resources.ApplyResources(this.tsmiShowErrors, "tsmiShowErrors");
            this.tsmiShowErrors.Image = global::ShareX.Properties.Resources.exclamation_button;
            this.tsmiShowErrors.Name = "tsmiShowErrors";
            this.tsmiShowErrors.Click += new System.EventHandler(this.tsmiShowErrors_Click);
            // 
            // tsmiStopUpload
            // 
            resources.ApplyResources(this.tsmiStopUpload, "tsmiStopUpload");
            this.tsmiStopUpload.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiStopUpload.Name = "tsmiStopUpload";
            this.tsmiStopUpload.Click += new System.EventHandler(this.tsmiStopUpload_Click);
            // 
            // tsmiOpen
            // 
            resources.ApplyResources(this.tsmiOpen, "tsmiOpen");
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
            // 
            // tsmiOpenURL
            // 
            resources.ApplyResources(this.tsmiOpenURL, "tsmiOpenURL");
            this.tsmiOpenURL.Name = "tsmiOpenURL";
            this.tsmiOpenURL.Click += new System.EventHandler(this.tsmiOpenURL_Click);
            // 
            // tsmiOpenShortenedURL
            // 
            resources.ApplyResources(this.tsmiOpenShortenedURL, "tsmiOpenShortenedURL");
            this.tsmiOpenShortenedURL.Name = "tsmiOpenShortenedURL";
            this.tsmiOpenShortenedURL.Click += new System.EventHandler(this.tsmiOpenShortenedURL_Click);
            // 
            // tsmiOpenThumbnailURL
            // 
            resources.ApplyResources(this.tsmiOpenThumbnailURL, "tsmiOpenThumbnailURL");
            this.tsmiOpenThumbnailURL.Name = "tsmiOpenThumbnailURL";
            this.tsmiOpenThumbnailURL.Click += new System.EventHandler(this.tsmiOpenThumbnailURL_Click);
            // 
            // tsmiOpenDeletionURL
            // 
            resources.ApplyResources(this.tsmiOpenDeletionURL, "tsmiOpenDeletionURL");
            this.tsmiOpenDeletionURL.Name = "tsmiOpenDeletionURL";
            this.tsmiOpenDeletionURL.Click += new System.EventHandler(this.tsmiOpenDeletionURL_Click);
            // 
            // tssOpen1
            // 
            resources.ApplyResources(this.tssOpen1, "tssOpen1");
            this.tssOpen1.Name = "tssOpen1";
            // 
            // tsmiOpenFile
            // 
            resources.ApplyResources(this.tsmiOpenFile, "tsmiOpenFile");
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiOpenFolder
            // 
            resources.ApplyResources(this.tsmiOpenFolder, "tsmiOpenFolder");
            this.tsmiOpenFolder.Name = "tsmiOpenFolder";
            this.tsmiOpenFolder.Click += new System.EventHandler(this.tsmiOpenFolder_Click);
            // 
            // tsmiOpenThumbnailFile
            // 
            resources.ApplyResources(this.tsmiOpenThumbnailFile, "tsmiOpenThumbnailFile");
            this.tsmiOpenThumbnailFile.Name = "tsmiOpenThumbnailFile";
            this.tsmiOpenThumbnailFile.Click += new System.EventHandler(this.tsmiOpenThumbnailFile_Click);
            // 
            // tsmiCopy
            // 
            resources.ApplyResources(this.tsmiCopy, "tsmiCopy");
            this.tsmiCopy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyURL,
            this.tsmiCopyShortenedURL,
            this.tsmiCopyThumbnailURL,
            this.tsmiCopyDeletionURL,
            this.tssCopy1,
            this.tsmiCopyFile,
            this.tsmiCopyImage,
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
            this.tsmiCopyFilePath,
            this.tsmiCopyFileName,
            this.tsmiCopyFileNameWithExtension,
            this.tsmiCopyFolder,
            this.tssCopy5});
            this.tsmiCopy.Image = global::ShareX.Properties.Resources.document_copy;
            this.tsmiCopy.Name = "tsmiCopy";
            // 
            // tsmiCopyURL
            // 
            resources.ApplyResources(this.tsmiCopyURL, "tsmiCopyURL");
            this.tsmiCopyURL.Name = "tsmiCopyURL";
            this.tsmiCopyURL.Click += new System.EventHandler(this.tsmiCopyURL_Click);
            // 
            // tsmiCopyShortenedURL
            // 
            resources.ApplyResources(this.tsmiCopyShortenedURL, "tsmiCopyShortenedURL");
            this.tsmiCopyShortenedURL.Name = "tsmiCopyShortenedURL";
            this.tsmiCopyShortenedURL.Click += new System.EventHandler(this.tsmiCopyShortenedURL_Click);
            // 
            // tsmiCopyThumbnailURL
            // 
            resources.ApplyResources(this.tsmiCopyThumbnailURL, "tsmiCopyThumbnailURL");
            this.tsmiCopyThumbnailURL.Name = "tsmiCopyThumbnailURL";
            this.tsmiCopyThumbnailURL.Click += new System.EventHandler(this.tsmiCopyThumbnailURL_Click);
            // 
            // tsmiCopyDeletionURL
            // 
            resources.ApplyResources(this.tsmiCopyDeletionURL, "tsmiCopyDeletionURL");
            this.tsmiCopyDeletionURL.Name = "tsmiCopyDeletionURL";
            this.tsmiCopyDeletionURL.Click += new System.EventHandler(this.tsmiCopyDeletionURL_Click);
            // 
            // tssCopy1
            // 
            resources.ApplyResources(this.tssCopy1, "tssCopy1");
            this.tssCopy1.Name = "tssCopy1";
            // 
            // tsmiCopyFile
            // 
            resources.ApplyResources(this.tsmiCopyFile, "tsmiCopyFile");
            this.tsmiCopyFile.Name = "tsmiCopyFile";
            this.tsmiCopyFile.Click += new System.EventHandler(this.tsmiCopyFile_Click);
            // 
            // tsmiCopyImage
            // 
            resources.ApplyResources(this.tsmiCopyImage, "tsmiCopyImage");
            this.tsmiCopyImage.Name = "tsmiCopyImage";
            this.tsmiCopyImage.Click += new System.EventHandler(this.tsmiCopyImage_Click);
            // 
            // tsmiCopyText
            // 
            resources.ApplyResources(this.tsmiCopyText, "tsmiCopyText");
            this.tsmiCopyText.Name = "tsmiCopyText";
            this.tsmiCopyText.Click += new System.EventHandler(this.tsmiCopyText_Click);
            // 
            // tsmiCopyThumbnailFile
            // 
            resources.ApplyResources(this.tsmiCopyThumbnailFile, "tsmiCopyThumbnailFile");
            this.tsmiCopyThumbnailFile.Name = "tsmiCopyThumbnailFile";
            this.tsmiCopyThumbnailFile.Click += new System.EventHandler(this.tsmiCopyThumbnailFile_Click);
            // 
            // tsmiCopyThumbnailImage
            // 
            resources.ApplyResources(this.tsmiCopyThumbnailImage, "tsmiCopyThumbnailImage");
            this.tsmiCopyThumbnailImage.Name = "tsmiCopyThumbnailImage";
            this.tsmiCopyThumbnailImage.Click += new System.EventHandler(this.tsmiCopyThumbnailImage_Click);
            // 
            // tssCopy2
            // 
            resources.ApplyResources(this.tssCopy2, "tssCopy2");
            this.tssCopy2.Name = "tssCopy2";
            // 
            // tsmiCopyHTMLLink
            // 
            resources.ApplyResources(this.tsmiCopyHTMLLink, "tsmiCopyHTMLLink");
            this.tsmiCopyHTMLLink.Name = "tsmiCopyHTMLLink";
            this.tsmiCopyHTMLLink.Click += new System.EventHandler(this.tsmiCopyHTMLLink_Click);
            // 
            // tsmiCopyHTMLImage
            // 
            resources.ApplyResources(this.tsmiCopyHTMLImage, "tsmiCopyHTMLImage");
            this.tsmiCopyHTMLImage.Name = "tsmiCopyHTMLImage";
            this.tsmiCopyHTMLImage.Click += new System.EventHandler(this.tsmiCopyHTMLImage_Click);
            // 
            // tsmiCopyHTMLLinkedImage
            // 
            resources.ApplyResources(this.tsmiCopyHTMLLinkedImage, "tsmiCopyHTMLLinkedImage");
            this.tsmiCopyHTMLLinkedImage.Name = "tsmiCopyHTMLLinkedImage";
            this.tsmiCopyHTMLLinkedImage.Click += new System.EventHandler(this.tsmiCopyHTMLLinkedImage_Click);
            // 
            // tssCopy3
            // 
            resources.ApplyResources(this.tssCopy3, "tssCopy3");
            this.tssCopy3.Name = "tssCopy3";
            // 
            // tsmiCopyForumLink
            // 
            resources.ApplyResources(this.tsmiCopyForumLink, "tsmiCopyForumLink");
            this.tsmiCopyForumLink.Name = "tsmiCopyForumLink";
            this.tsmiCopyForumLink.Click += new System.EventHandler(this.tsmiCopyForumLink_Click);
            // 
            // tsmiCopyForumImage
            // 
            resources.ApplyResources(this.tsmiCopyForumImage, "tsmiCopyForumImage");
            this.tsmiCopyForumImage.Name = "tsmiCopyForumImage";
            this.tsmiCopyForumImage.Click += new System.EventHandler(this.tsmiCopyForumImage_Click);
            // 
            // tsmiCopyForumLinkedImage
            // 
            resources.ApplyResources(this.tsmiCopyForumLinkedImage, "tsmiCopyForumLinkedImage");
            this.tsmiCopyForumLinkedImage.Name = "tsmiCopyForumLinkedImage";
            this.tsmiCopyForumLinkedImage.Click += new System.EventHandler(this.tsmiCopyForumLinkedImage_Click);
            // 
            // tssCopy4
            // 
            resources.ApplyResources(this.tssCopy4, "tssCopy4");
            this.tssCopy4.Name = "tssCopy4";
            // 
            // tsmiCopyFilePath
            // 
            resources.ApplyResources(this.tsmiCopyFilePath, "tsmiCopyFilePath");
            this.tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            this.tsmiCopyFilePath.Click += new System.EventHandler(this.tsmiCopyFilePath_Click);
            // 
            // tsmiCopyFileName
            // 
            resources.ApplyResources(this.tsmiCopyFileName, "tsmiCopyFileName");
            this.tsmiCopyFileName.Name = "tsmiCopyFileName";
            this.tsmiCopyFileName.Click += new System.EventHandler(this.tsmiCopyFileName_Click);
            // 
            // tsmiCopyFileNameWithExtension
            // 
            resources.ApplyResources(this.tsmiCopyFileNameWithExtension, "tsmiCopyFileNameWithExtension");
            this.tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            this.tsmiCopyFileNameWithExtension.Click += new System.EventHandler(this.tsmiCopyFileNameWithExtension_Click);
            // 
            // tsmiCopyFolder
            // 
            resources.ApplyResources(this.tsmiCopyFolder, "tsmiCopyFolder");
            this.tsmiCopyFolder.Name = "tsmiCopyFolder";
            this.tsmiCopyFolder.Click += new System.EventHandler(this.tsmiCopyFolder_Click);
            // 
            // tssCopy5
            // 
            resources.ApplyResources(this.tssCopy5, "tssCopy5");
            this.tssCopy5.Name = "tssCopy5";
            // 
            // tsmiUploadSelectedFile
            // 
            resources.ApplyResources(this.tsmiUploadSelectedFile, "tsmiUploadSelectedFile");
            this.tsmiUploadSelectedFile.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsmiUploadSelectedFile.Name = "tsmiUploadSelectedFile";
            this.tsmiUploadSelectedFile.Click += new System.EventHandler(this.tsmiUploadSelectedFile_Click);
            // 
            // tsmiEditSelectedFile
            // 
            resources.ApplyResources(this.tsmiEditSelectedFile, "tsmiEditSelectedFile");
            this.tsmiEditSelectedFile.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiEditSelectedFile.Name = "tsmiEditSelectedFile";
            this.tsmiEditSelectedFile.Click += new System.EventHandler(this.tsmiEditSelectedFile_Click);
            // 
            // tsmiDeleteSelectedFile
            // 
            resources.ApplyResources(this.tsmiDeleteSelectedFile, "tsmiDeleteSelectedFile");
            this.tsmiDeleteSelectedFile.Image = global::ShareX.Properties.Resources.bin;
            this.tsmiDeleteSelectedFile.Name = "tsmiDeleteSelectedFile";
            this.tsmiDeleteSelectedFile.Click += new System.EventHandler(this.tsmiDeleteSelectedFile_Click);
            // 
            // tsmiShortenSelectedURL
            // 
            resources.ApplyResources(this.tsmiShortenSelectedURL, "tsmiShortenSelectedURL");
            this.tsmiShortenSelectedURL.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiShortenSelectedURL.Name = "tsmiShortenSelectedURL";
            // 
            // tsmiShareSelectedURL
            // 
            resources.ApplyResources(this.tsmiShareSelectedURL, "tsmiShareSelectedURL");
            this.tsmiShareSelectedURL.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiShareSelectedURL.Name = "tsmiShareSelectedURL";
            // 
            // tsmiShowQRCode
            // 
            resources.ApplyResources(this.tsmiShowQRCode, "tsmiShowQRCode");
            this.tsmiShowQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiShowQRCode.Name = "tsmiShowQRCode";
            this.tsmiShowQRCode.Click += new System.EventHandler(this.tsmiShowQRCode_Click);
            // 
            // tsmiShowResponse
            // 
            resources.ApplyResources(this.tsmiShowResponse, "tsmiShowResponse");
            this.tsmiShowResponse.Image = global::ShareX.Properties.Resources.application_browser;
            this.tsmiShowResponse.Name = "tsmiShowResponse";
            this.tsmiShowResponse.Click += new System.EventHandler(this.tsmiShowResponse_Click);
            // 
            // tsmiClearList
            // 
            resources.ApplyResources(this.tsmiClearList, "tsmiClearList");
            this.tsmiClearList.Image = global::ShareX.Properties.Resources.eraser;
            this.tsmiClearList.Name = "tsmiClearList";
            this.tsmiClearList.Click += new System.EventHandler(this.tsmiClearList_Click);
            // 
            // tssUploadInfo1
            // 
            resources.ApplyResources(this.tssUploadInfo1, "tssUploadInfo1");
            this.tssUploadInfo1.Name = "tssUploadInfo1";
            // 
            // tsmiHideMenu
            // 
            resources.ApplyResources(this.tsmiHideMenu, "tsmiHideMenu");
            this.tsmiHideMenu.Image = global::ShareX.Properties.Resources.layout_select_sidebar;
            this.tsmiHideMenu.Name = "tsmiHideMenu";
            this.tsmiHideMenu.Click += new System.EventHandler(this.tsmiHideMenu_Click);
            // 
            // tsmiImagePreview
            // 
            resources.ApplyResources(this.tsmiImagePreview, "tsmiImagePreview");
            this.tsmiImagePreview.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImagePreviewShow,
            this.tsmiImagePreviewHide,
            this.tsmiImagePreviewAutomatic});
            this.tsmiImagePreview.Image = global::ShareX.Properties.Resources.layout_select_content;
            this.tsmiImagePreview.Name = "tsmiImagePreview";
            // 
            // tsmiImagePreviewShow
            // 
            resources.ApplyResources(this.tsmiImagePreviewShow, "tsmiImagePreviewShow");
            this.tsmiImagePreviewShow.Checked = true;
            this.tsmiImagePreviewShow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiImagePreviewShow.Name = "tsmiImagePreviewShow";
            this.tsmiImagePreviewShow.Click += new System.EventHandler(this.tsmiImagePreviewShow_Click);
            // 
            // tsmiImagePreviewHide
            // 
            resources.ApplyResources(this.tsmiImagePreviewHide, "tsmiImagePreviewHide");
            this.tsmiImagePreviewHide.Name = "tsmiImagePreviewHide";
            this.tsmiImagePreviewHide.Click += new System.EventHandler(this.tsmiImagePreviewHide_Click);
            // 
            // tsmiImagePreviewAutomatic
            // 
            resources.ApplyResources(this.tsmiImagePreviewAutomatic, "tsmiImagePreviewAutomatic");
            this.tsmiImagePreviewAutomatic.Name = "tsmiImagePreviewAutomatic";
            this.tsmiImagePreviewAutomatic.Click += new System.EventHandler(this.tsmiImagePreviewAutomatic_Click);
            // 
            // niTray
            // 
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.ContextMenuStrip = this.cmsTray;
            this.niTray.BalloonTipClicked += new System.EventHandler(this.niTray_BalloonTipClicked);
            this.niTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseDoubleClick);
            this.niTray.MouseUp += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseUp);
            // 
            // cmsTray
            // 
            resources.ApplyResources(this.cmsTray, "cmsTray");
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
            this.tssTray2,
            this.tsmiScreenshotsFolder,
            this.tsmiTrayHistory,
            this.tsmiTrayImageHistory,
            this.tsmiTrayDonate,
            this.tsmiTrayAbout,
            this.tssTray3,
            this.tsmiTrayShow,
            this.tsmiTrayExit});
            this.cmsTray.Name = "cmsTray";
            // 
            // tsmiTrayCapture
            // 
            resources.ApplyResources(this.tsmiTrayCapture, "tsmiTrayCapture");
            this.tsmiTrayCapture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayFullscreen,
            this.tsmiTrayWindow,
            this.tsmiTrayMonitor,
            this.tsmiTrayRectangle,
            this.tsmiTrayWindowRectangle,
            this.tsmiTrayRectangleAnnotate,
            this.tsmiTrayRectangleLight,
            this.tsmiTrayRoundedRectangle,
            this.tsmiTrayEllipse,
            this.tsmiTrayTriangle,
            this.tsmiTrayDiamond,
            this.tsmiTrayPolygon,
            this.tsmiTrayFreeHand,
            this.tsmiTrayLastRegion,
            this.screenRecordingFFmpegToolStripMenuItem,
            this.tsmiTrayScreenRecordingGIF,
            this.tsmiTrayAutoCapture});
            this.tsmiTrayCapture.Image = global::ShareX.Properties.Resources.camera;
            this.tsmiTrayCapture.Name = "tsmiTrayCapture";
            this.tsmiTrayCapture.DropDownOpening += new System.EventHandler(this.tsmiCapture_DropDownOpening);
            // 
            // tsmiTrayFullscreen
            // 
            resources.ApplyResources(this.tsmiTrayFullscreen, "tsmiTrayFullscreen");
            this.tsmiTrayFullscreen.Image = global::ShareX.Properties.Resources.layer;
            this.tsmiTrayFullscreen.Name = "tsmiTrayFullscreen";
            this.tsmiTrayFullscreen.Click += new System.EventHandler(this.tsmiTrayFullscreen_Click);
            // 
            // tsmiTrayWindow
            // 
            resources.ApplyResources(this.tsmiTrayWindow, "tsmiTrayWindow");
            this.tsmiTrayWindow.Image = global::ShareX.Properties.Resources.application_blue;
            this.tsmiTrayWindow.Name = "tsmiTrayWindow";
            // 
            // tsmiTrayMonitor
            // 
            resources.ApplyResources(this.tsmiTrayMonitor, "tsmiTrayMonitor");
            this.tsmiTrayMonitor.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiTrayMonitor.Name = "tsmiTrayMonitor";
            // 
            // tsmiTrayRectangle
            // 
            resources.ApplyResources(this.tsmiTrayRectangle, "tsmiTrayRectangle");
            this.tsmiTrayRectangle.Image = global::ShareX.Properties.Resources.layer_shape;
            this.tsmiTrayRectangle.Name = "tsmiTrayRectangle";
            this.tsmiTrayRectangle.Click += new System.EventHandler(this.tsmiTrayRectangle_Click);
            // 
            // tsmiTrayWindowRectangle
            // 
            resources.ApplyResources(this.tsmiTrayWindowRectangle, "tsmiTrayWindowRectangle");
            this.tsmiTrayWindowRectangle.Image = global::ShareX.Properties.Resources.layers_ungroup;
            this.tsmiTrayWindowRectangle.Name = "tsmiTrayWindowRectangle";
            this.tsmiTrayWindowRectangle.Click += new System.EventHandler(this.tsmiTrayWindowRectangle_Click);
            // 
            // tsmiTrayRectangleAnnotate
            // 
            resources.ApplyResources(this.tsmiTrayRectangleAnnotate, "tsmiTrayRectangleAnnotate");
            this.tsmiTrayRectangleAnnotate.Image = global::ShareX.Properties.Resources.layer_pencil;
            this.tsmiTrayRectangleAnnotate.Name = "tsmiTrayRectangleAnnotate";
            this.tsmiTrayRectangleAnnotate.Click += new System.EventHandler(this.tsmiTrayRectangleAnnotate_Click);
            // 
            // tsmiTrayRectangleLight
            // 
            resources.ApplyResources(this.tsmiTrayRectangleLight, "tsmiTrayRectangleLight");
            this.tsmiTrayRectangleLight.Image = global::ShareX.Properties.Resources.Rectangle;
            this.tsmiTrayRectangleLight.Name = "tsmiTrayRectangleLight";
            this.tsmiTrayRectangleLight.Click += new System.EventHandler(this.tsmiTrayRectangleLight_Click);
            // 
            // tsmiTrayRoundedRectangle
            // 
            resources.ApplyResources(this.tsmiTrayRoundedRectangle, "tsmiTrayRoundedRectangle");
            this.tsmiTrayRoundedRectangle.Image = global::ShareX.Properties.Resources.layer_shape_round;
            this.tsmiTrayRoundedRectangle.Name = "tsmiTrayRoundedRectangle";
            this.tsmiTrayRoundedRectangle.Click += new System.EventHandler(this.tsmiTrayRoundedRectangle_Click);
            // 
            // tsmiTrayEllipse
            // 
            resources.ApplyResources(this.tsmiTrayEllipse, "tsmiTrayEllipse");
            this.tsmiTrayEllipse.Image = global::ShareX.Properties.Resources.layer_shape_ellipse;
            this.tsmiTrayEllipse.Name = "tsmiTrayEllipse";
            this.tsmiTrayEllipse.Click += new System.EventHandler(this.tsmiTrayEllipse_Click);
            // 
            // tsmiTrayTriangle
            // 
            resources.ApplyResources(this.tsmiTrayTriangle, "tsmiTrayTriangle");
            this.tsmiTrayTriangle.Image = global::ShareX.Properties.Resources.Triangle;
            this.tsmiTrayTriangle.Name = "tsmiTrayTriangle";
            this.tsmiTrayTriangle.Click += new System.EventHandler(this.tsmiTrayTriangle_Click);
            // 
            // tsmiTrayDiamond
            // 
            resources.ApplyResources(this.tsmiTrayDiamond, "tsmiTrayDiamond");
            this.tsmiTrayDiamond.Image = global::ShareX.Properties.Resources.Diamond;
            this.tsmiTrayDiamond.Name = "tsmiTrayDiamond";
            this.tsmiTrayDiamond.Click += new System.EventHandler(this.tsmiTrayDiamond_Click);
            // 
            // tsmiTrayPolygon
            // 
            resources.ApplyResources(this.tsmiTrayPolygon, "tsmiTrayPolygon");
            this.tsmiTrayPolygon.Image = global::ShareX.Properties.Resources.layer_shape_polygon;
            this.tsmiTrayPolygon.Name = "tsmiTrayPolygon";
            this.tsmiTrayPolygon.Click += new System.EventHandler(this.tsmiTrayPolygon_Click);
            // 
            // tsmiTrayFreeHand
            // 
            resources.ApplyResources(this.tsmiTrayFreeHand, "tsmiTrayFreeHand");
            this.tsmiTrayFreeHand.Image = global::ShareX.Properties.Resources.layer_shape_curve;
            this.tsmiTrayFreeHand.Name = "tsmiTrayFreeHand";
            this.tsmiTrayFreeHand.Click += new System.EventHandler(this.tsmiTrayFreeHand_Click);
            // 
            // tsmiTrayLastRegion
            // 
            resources.ApplyResources(this.tsmiTrayLastRegion, "tsmiTrayLastRegion");
            this.tsmiTrayLastRegion.Image = global::ShareX.Properties.Resources.layers_arrange;
            this.tsmiTrayLastRegion.Name = "tsmiTrayLastRegion";
            this.tsmiTrayLastRegion.Click += new System.EventHandler(this.tsmiTrayLastRegion_Click);
            // 
            // screenRecordingFFmpegToolStripMenuItem
            // 
            resources.ApplyResources(this.screenRecordingFFmpegToolStripMenuItem, "screenRecordingFFmpegToolStripMenuItem");
            this.screenRecordingFFmpegToolStripMenuItem.Image = global::ShareX.Properties.Resources.camcorder_image;
            this.screenRecordingFFmpegToolStripMenuItem.Name = "screenRecordingFFmpegToolStripMenuItem";
            this.screenRecordingFFmpegToolStripMenuItem.Click += new System.EventHandler(this.tsmiScreenRecordingFFmpeg_Click);
            // 
            // tsmiTrayScreenRecordingGIF
            // 
            resources.ApplyResources(this.tsmiTrayScreenRecordingGIF, "tsmiTrayScreenRecordingGIF");
            this.tsmiTrayScreenRecordingGIF.Image = global::ShareX.Properties.Resources.film;
            this.tsmiTrayScreenRecordingGIF.Name = "tsmiTrayScreenRecordingGIF";
            this.tsmiTrayScreenRecordingGIF.Click += new System.EventHandler(this.tsmiScreenRecordingGIF_Click);
            // 
            // tsmiTrayAutoCapture
            // 
            resources.ApplyResources(this.tsmiTrayAutoCapture, "tsmiTrayAutoCapture");
            this.tsmiTrayAutoCapture.Image = global::ShareX.Properties.Resources.clock;
            this.tsmiTrayAutoCapture.Name = "tsmiTrayAutoCapture";
            this.tsmiTrayAutoCapture.Click += new System.EventHandler(this.tsmiAutoCapture_Click);
            // 
            // tsmiTrayUpload
            // 
            resources.ApplyResources(this.tsmiTrayUpload, "tsmiTrayUpload");
            this.tsmiTrayUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayUploadFile,
            this.tsmiTrayUploadFolder,
            this.tsmiTrayUploadClipboard,
            this.tsmiTrayUploadURL,
            this.tsmiTrayUploadDragDrop});
            this.tsmiTrayUpload.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsmiTrayUpload.Name = "tsmiTrayUpload";
            // 
            // tsmiTrayUploadFile
            // 
            resources.ApplyResources(this.tsmiTrayUploadFile, "tsmiTrayUploadFile");
            this.tsmiTrayUploadFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiTrayUploadFile.Name = "tsmiTrayUploadFile";
            this.tsmiTrayUploadFile.Click += new System.EventHandler(this.tsbFileUpload_Click);
            // 
            // tsmiTrayUploadFolder
            // 
            resources.ApplyResources(this.tsmiTrayUploadFolder, "tsmiTrayUploadFolder");
            this.tsmiTrayUploadFolder.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiTrayUploadFolder.Name = "tsmiTrayUploadFolder";
            this.tsmiTrayUploadFolder.Click += new System.EventHandler(this.tsmiUploadFolder_Click);
            // 
            // tsmiTrayUploadClipboard
            // 
            resources.ApplyResources(this.tsmiTrayUploadClipboard, "tsmiTrayUploadClipboard");
            this.tsmiTrayUploadClipboard.Image = global::ShareX.Properties.Resources.clipboard;
            this.tsmiTrayUploadClipboard.Name = "tsmiTrayUploadClipboard";
            this.tsmiTrayUploadClipboard.Click += new System.EventHandler(this.tsbClipboardUpload_Click);
            // 
            // tsmiTrayUploadURL
            // 
            resources.ApplyResources(this.tsmiTrayUploadURL, "tsmiTrayUploadURL");
            this.tsmiTrayUploadURL.Image = global::ShareX.Properties.Resources.drive;
            this.tsmiTrayUploadURL.Name = "tsmiTrayUploadURL";
            this.tsmiTrayUploadURL.Click += new System.EventHandler(this.tsmiUploadURL_Click);
            // 
            // tsmiTrayUploadDragDrop
            // 
            resources.ApplyResources(this.tsmiTrayUploadDragDrop, "tsmiTrayUploadDragDrop");
            this.tsmiTrayUploadDragDrop.Image = global::ShareX.Properties.Resources.inbox;
            this.tsmiTrayUploadDragDrop.Name = "tsmiTrayUploadDragDrop";
            this.tsmiTrayUploadDragDrop.Click += new System.EventHandler(this.tsbDragDropUpload_Click);
            // 
            // tsmiTrayWorkflows
            // 
            resources.ApplyResources(this.tsmiTrayWorkflows, "tsmiTrayWorkflows");
            this.tsmiTrayWorkflows.Image = global::ShareX.Properties.Resources.categories;
            this.tsmiTrayWorkflows.Name = "tsmiTrayWorkflows";
            // 
            // tsmiTrayTools
            // 
            resources.ApplyResources(this.tsmiTrayTools, "tsmiTrayTools");
            this.tsmiTrayTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayScreenColorPicker,
            this.tsmiTrayImageEditor,
            this.tsmiTrayImageEffects,
            this.tsmiTrayHashCheck,
            this.tsmiTrayDNSChanger,
            this.tsmiTrayQRCode,
            this.tsmiTrayIndexFolder,
            this.tsmiTrayRuler,
            this.tsmiTrayFTPClient,
            this.tsmiTrayTweetMessage,
            this.tsmiTrayMonitorTest});
            this.tsmiTrayTools.Image = global::ShareX.Properties.Resources.toolbox;
            this.tsmiTrayTools.Name = "tsmiTrayTools";
            // 
            // tsmiTrayScreenColorPicker
            // 
            resources.ApplyResources(this.tsmiTrayScreenColorPicker, "tsmiTrayScreenColorPicker");
            this.tsmiTrayScreenColorPicker.Image = global::ShareX.Properties.Resources.color;
            this.tsmiTrayScreenColorPicker.Name = "tsmiTrayScreenColorPicker";
            this.tsmiTrayScreenColorPicker.Click += new System.EventHandler(this.tsmiCursorHelper_Click);
            // 
            // tsmiTrayImageEditor
            // 
            resources.ApplyResources(this.tsmiTrayImageEditor, "tsmiTrayImageEditor");
            this.tsmiTrayImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiTrayImageEditor.Name = "tsmiTrayImageEditor";
            this.tsmiTrayImageEditor.Click += new System.EventHandler(this.tsmiImageEditor_Click);
            // 
            // tsmiTrayImageEffects
            // 
            resources.ApplyResources(this.tsmiTrayImageEffects, "tsmiTrayImageEffects");
            this.tsmiTrayImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiTrayImageEffects.Name = "tsmiTrayImageEffects";
            this.tsmiTrayImageEffects.Click += new System.EventHandler(this.tsmiImageEffects_Click);
            // 
            // tsmiTrayHashCheck
            // 
            resources.ApplyResources(this.tsmiTrayHashCheck, "tsmiTrayHashCheck");
            this.tsmiTrayHashCheck.Image = global::ShareX.Properties.Resources.application_task;
            this.tsmiTrayHashCheck.Name = "tsmiTrayHashCheck";
            this.tsmiTrayHashCheck.Click += new System.EventHandler(this.tsmiHashCheck_Click);
            // 
            // tsmiTrayDNSChanger
            // 
            resources.ApplyResources(this.tsmiTrayDNSChanger, "tsmiTrayDNSChanger");
            this.tsmiTrayDNSChanger.Image = global::ShareX.Properties.Resources.network_ip;
            this.tsmiTrayDNSChanger.Name = "tsmiTrayDNSChanger";
            this.tsmiTrayDNSChanger.Click += new System.EventHandler(this.tsmiDNSChanger_Click);
            // 
            // tsmiTrayQRCode
            // 
            resources.ApplyResources(this.tsmiTrayQRCode, "tsmiTrayQRCode");
            this.tsmiTrayQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiTrayQRCode.Name = "tsmiTrayQRCode";
            this.tsmiTrayQRCode.Click += new System.EventHandler(this.tsmiQRCode_Click);
            // 
            // tsmiTrayIndexFolder
            // 
            resources.ApplyResources(this.tsmiTrayIndexFolder, "tsmiTrayIndexFolder");
            this.tsmiTrayIndexFolder.Image = global::ShareX.Properties.Resources.folder_tree;
            this.tsmiTrayIndexFolder.Name = "tsmiTrayIndexFolder";
            this.tsmiTrayIndexFolder.Click += new System.EventHandler(this.tsmiIndexFolder_Click);
            // 
            // tsmiTrayRuler
            // 
            resources.ApplyResources(this.tsmiTrayRuler, "tsmiTrayRuler");
            this.tsmiTrayRuler.Image = global::ShareX.Properties.Resources.ruler_triangle;
            this.tsmiTrayRuler.Name = "tsmiTrayRuler";
            this.tsmiTrayRuler.Click += new System.EventHandler(this.tsmiRuler_Click);
            // 
            // tsmiTrayFTPClient
            // 
            resources.ApplyResources(this.tsmiTrayFTPClient, "tsmiTrayFTPClient");
            this.tsmiTrayFTPClient.Image = global::ShareX.Properties.Resources.application_network;
            this.tsmiTrayFTPClient.Name = "tsmiTrayFTPClient";
            this.tsmiTrayFTPClient.Click += new System.EventHandler(this.tsmiFTPClient_Click);
            // 
            // tsmiTrayTweetMessage
            // 
            resources.ApplyResources(this.tsmiTrayTweetMessage, "tsmiTrayTweetMessage");
            this.tsmiTrayTweetMessage.Image = global::ShareX.Properties.Resources.Twitter;
            this.tsmiTrayTweetMessage.Name = "tsmiTrayTweetMessage";
            this.tsmiTrayTweetMessage.Click += new System.EventHandler(this.tsmiTweetMessage_Click);
            // 
            // tsmiTrayMonitorTest
            // 
            resources.ApplyResources(this.tsmiTrayMonitorTest, "tsmiTrayMonitorTest");
            this.tsmiTrayMonitorTest.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiTrayMonitorTest.Name = "tsmiTrayMonitorTest";
            this.tsmiTrayMonitorTest.Click += new System.EventHandler(this.tsmiMonitorTest_Click);
            // 
            // tssTray1
            // 
            resources.ApplyResources(this.tssTray1, "tssTray1");
            this.tssTray1.Name = "tssTray1";
            // 
            // tsmiTrayAfterCaptureTasks
            // 
            resources.ApplyResources(this.tsmiTrayAfterCaptureTasks, "tsmiTrayAfterCaptureTasks");
            this.tsmiTrayAfterCaptureTasks.Image = global::ShareX.Properties.Resources.image_export;
            this.tsmiTrayAfterCaptureTasks.Name = "tsmiTrayAfterCaptureTasks";
            // 
            // tsmiTrayAfterUploadTasks
            // 
            resources.ApplyResources(this.tsmiTrayAfterUploadTasks, "tsmiTrayAfterUploadTasks");
            this.tsmiTrayAfterUploadTasks.Image = global::ShareX.Properties.Resources.upload_cloud;
            this.tsmiTrayAfterUploadTasks.Name = "tsmiTrayAfterUploadTasks";
            // 
            // tsmiTrayDestinations
            // 
            resources.ApplyResources(this.tsmiTrayDestinations, "tsmiTrayDestinations");
            this.tsmiTrayDestinations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayImageUploaders,
            this.tsmiTrayTextUploaders,
            this.tsmiTrayFileUploaders,
            this.tsmiTrayURLShorteners,
            this.tsmiTrayURLSharingServices,
            this.tssTrayDestinations1,
            this.tsmiTrayDestinationSettings});
            this.tsmiTrayDestinations.Image = global::ShareX.Properties.Resources.drive_globe;
            this.tsmiTrayDestinations.Name = "tsmiTrayDestinations";
            this.tsmiTrayDestinations.DropDownOpened += new System.EventHandler(this.tsddbDestinations_DropDownOpened);
            // 
            // tsmiTrayImageUploaders
            // 
            resources.ApplyResources(this.tsmiTrayImageUploaders, "tsmiTrayImageUploaders");
            this.tsmiTrayImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiTrayImageUploaders.Name = "tsmiTrayImageUploaders";
            // 
            // tsmiTrayTextUploaders
            // 
            resources.ApplyResources(this.tsmiTrayTextUploaders, "tsmiTrayTextUploaders");
            this.tsmiTrayTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTrayTextUploaders.Name = "tsmiTrayTextUploaders";
            // 
            // tsmiTrayFileUploaders
            // 
            resources.ApplyResources(this.tsmiTrayFileUploaders, "tsmiTrayFileUploaders");
            this.tsmiTrayFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiTrayFileUploaders.Name = "tsmiTrayFileUploaders";
            // 
            // tsmiTrayURLShorteners
            // 
            resources.ApplyResources(this.tsmiTrayURLShorteners, "tsmiTrayURLShorteners");
            this.tsmiTrayURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTrayURLShorteners.Name = "tsmiTrayURLShorteners";
            // 
            // tsmiTrayURLSharingServices
            // 
            resources.ApplyResources(this.tsmiTrayURLSharingServices, "tsmiTrayURLSharingServices");
            this.tsmiTrayURLSharingServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiTrayURLSharingServices.Name = "tsmiTrayURLSharingServices";
            // 
            // tssTrayDestinations1
            // 
            resources.ApplyResources(this.tssTrayDestinations1, "tssTrayDestinations1");
            this.tssTrayDestinations1.Name = "tssTrayDestinations1";
            // 
            // tsmiTrayDestinationSettings
            // 
            resources.ApplyResources(this.tsmiTrayDestinationSettings, "tsmiTrayDestinationSettings");
            this.tsmiTrayDestinationSettings.Image = global::ShareX.Properties.Resources.globe_pencil;
            this.tsmiTrayDestinationSettings.Name = "tsmiTrayDestinationSettings";
            this.tsmiTrayDestinationSettings.Click += new System.EventHandler(this.tsbDestinationSettings_Click);
            // 
            // tsmiTrayApplicationSettings
            // 
            resources.ApplyResources(this.tsmiTrayApplicationSettings, "tsmiTrayApplicationSettings");
            this.tsmiTrayApplicationSettings.Image = global::ShareX.Properties.Resources.wrench_screwdriver;
            this.tsmiTrayApplicationSettings.Name = "tsmiTrayApplicationSettings";
            this.tsmiTrayApplicationSettings.Click += new System.EventHandler(this.tsbApplicationSettings_Click);
            // 
            // tsmiTrayTaskSettings
            // 
            resources.ApplyResources(this.tsmiTrayTaskSettings, "tsmiTrayTaskSettings");
            this.tsmiTrayTaskSettings.Image = global::ShareX.Properties.Resources.gear;
            this.tsmiTrayTaskSettings.Name = "tsmiTrayTaskSettings";
            this.tsmiTrayTaskSettings.Click += new System.EventHandler(this.tsbTaskSettings_Click);
            // 
            // tsmiTrayHotkeySettings
            // 
            resources.ApplyResources(this.tsmiTrayHotkeySettings, "tsmiTrayHotkeySettings");
            this.tsmiTrayHotkeySettings.Image = global::ShareX.Properties.Resources.keyboard;
            this.tsmiTrayHotkeySettings.Name = "tsmiTrayHotkeySettings";
            this.tsmiTrayHotkeySettings.Click += new System.EventHandler(this.tsbHotkeySettings_Click);
            // 
            // tssTray2
            // 
            resources.ApplyResources(this.tssTray2, "tssTray2");
            this.tssTray2.Name = "tssTray2";
            // 
            // tsmiScreenshotsFolder
            // 
            resources.ApplyResources(this.tsmiScreenshotsFolder, "tsmiScreenshotsFolder");
            this.tsmiScreenshotsFolder.Image = global::ShareX.Properties.Resources.folder_open_image;
            this.tsmiScreenshotsFolder.Name = "tsmiScreenshotsFolder";
            this.tsmiScreenshotsFolder.Click += new System.EventHandler(this.tsbScreenshotsFolder_Click);
            // 
            // tsmiTrayHistory
            // 
            resources.ApplyResources(this.tsmiTrayHistory, "tsmiTrayHistory");
            this.tsmiTrayHistory.Image = global::ShareX.Properties.Resources.application_blog;
            this.tsmiTrayHistory.Name = "tsmiTrayHistory";
            this.tsmiTrayHistory.Click += new System.EventHandler(this.tsbHistory_Click);
            // 
            // tsmiTrayImageHistory
            // 
            resources.ApplyResources(this.tsmiTrayImageHistory, "tsmiTrayImageHistory");
            this.tsmiTrayImageHistory.Image = global::ShareX.Properties.Resources.application_icon_large;
            this.tsmiTrayImageHistory.Name = "tsmiTrayImageHistory";
            this.tsmiTrayImageHistory.Click += new System.EventHandler(this.tsbImageHistory_Click);
            // 
            // tsmiTrayDonate
            // 
            resources.ApplyResources(this.tsmiTrayDonate, "tsmiTrayDonate");
            this.tsmiTrayDonate.Image = global::ShareX.Properties.Resources.heart;
            this.tsmiTrayDonate.Name = "tsmiTrayDonate";
            this.tsmiTrayDonate.Click += new System.EventHandler(this.tsbDonate_Click);
            // 
            // tsmiTrayAbout
            // 
            resources.ApplyResources(this.tsmiTrayAbout, "tsmiTrayAbout");
            this.tsmiTrayAbout.Image = global::ShareX.Properties.Resources.crown;
            this.tsmiTrayAbout.Name = "tsmiTrayAbout";
            this.tsmiTrayAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // tssTray3
            // 
            resources.ApplyResources(this.tssTray3, "tssTray3");
            this.tssTray3.Name = "tssTray3";
            // 
            // tsmiTrayShow
            // 
            resources.ApplyResources(this.tsmiTrayShow, "tsmiTrayShow");
            this.tsmiTrayShow.Image = global::ShareX.Properties.Resources.tick_button;
            this.tsmiTrayShow.Name = "tsmiTrayShow";
            this.tsmiTrayShow.Click += new System.EventHandler(this.tsmiTrayShow_Click);
            // 
            // tsmiTrayExit
            // 
            resources.ApplyResources(this.tsmiTrayExit, "tsmiTrayExit");
            this.tsmiTrayExit.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiTrayExit.Name = "tsmiTrayExit";
            this.tsmiTrayExit.Click += new System.EventHandler(this.tsmiTrayExit_Click);
            // 
            // ssToolStripMenuItem
            // 
            resources.ApplyResources(this.ssToolStripMenuItem, "ssToolStripMenuItem");
            this.ssToolStripMenuItem.Name = "ssToolStripMenuItem";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.tsMain);
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
            this.pBackground.ResumeLayout(false);
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.cmsTaskInfo.ResumeLayout(false);
            this.cmsTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private HelpersLib.MyListView lvUploads;
        private System.Windows.Forms.ColumnHeader chStatus;
        private System.Windows.Forms.ColumnHeader chURL;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chProgress;
        private System.Windows.Forms.ToolStrip tsMain;
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
        private System.Windows.Forms.ContextMenuStrip cmsTray;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayExit;
        private System.Windows.Forms.ToolStripSeparator tssTray1;
        public System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.ToolStripDropDownButton tsddbCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiFullscreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiRoundedRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiEllipse;
        private System.Windows.Forms.ToolStripMenuItem tsmiTriangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiDiamond;
        private System.Windows.Forms.ToolStripMenuItem tsmiPolygon;
        private System.Windows.Forms.ToolStripMenuItem tsmiFreeHand;
        private System.Windows.Forms.ToolStripMenuItem tsmiWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiWindowRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayAbout;
        private System.Windows.Forms.ToolStripSeparator tssTray2;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayFullscreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayWindowRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRoundedRectangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTriangle;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayDiamond;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayPolygon;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayFreeHand;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayEllipse;
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
        private System.Windows.Forms.Label lblSplitter;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayAfterUploadTasks;
        private System.Windows.Forms.ToolStripSeparator tssUploadInfo1;
        private System.Windows.Forms.ToolStripMenuItem tsmiImagePreview;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadSelectedFile;
        private System.Windows.Forms.ToolStripButton tsbImageHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTools;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenColorPicker;
        private System.Windows.Forms.ToolStripDropDownButton tsddbTools;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenColorPicker;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearList;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenRecordingGIF;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayScreenRecordingGIF;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayDonate;
        private System.Windows.Forms.ToolStripMenuItem tsmiHashCheck;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayHashCheck;
        private System.Windows.Forms.ToolStripMenuItem tsmiMonitor;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayMonitor;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoCapture;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayAutoCapture;
        private System.Windows.Forms.ToolStripDropDownButton tsddbDebug;
        private System.Windows.Forms.ToolStripMenuItem ssToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestImageUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestTextUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestFileUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestURLShortener;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestUploaders;
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
        private System.Windows.Forms.ToolStripButton tsmiAbout;
        private System.Windows.Forms.ToolStripButton tsmiDonate;
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
        private System.Windows.Forms.ToolStripMenuItem tsmiFTPClient;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayFTPClient;
        private System.Windows.Forms.ToolStripMenuItem tsmiImageEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayImageEditor;
        private System.Windows.Forms.ToolStripDropDownButton tsddbWorkflows;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayWorkflows;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowQRCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiQRCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayQRCode;
        private System.Windows.Forms.ToolStripMenuItem tsmiRectangleLight;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRectangleLight;
        private System.Windows.Forms.ToolStripMenuItem tsmiTweetMessage;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayTweetMessage;
        private System.Windows.Forms.ToolStripMenuItem tsmiImagePreviewShow;
        private System.Windows.Forms.ToolStripMenuItem tsmiImagePreviewHide;
        private System.Windows.Forms.ToolStripMenuItem tsmiImagePreviewAutomatic;
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
        private System.Windows.Forms.ToolStripMenuItem tsmiRectangleAnnotate;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayRectangleAnnotate;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditSelectedFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiTestURLSharing;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteSelectedFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiScreenRecordingFFmpeg;
        private System.Windows.Forms.ToolStripMenuItem screenRecordingFFmpegToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiUploadFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiTrayUploadFolder;
        private HelpersLib.MyPictureBox pbLogo;
        private System.Windows.Forms.Label lblDragAndDropTip;
        internal System.Windows.Forms.Panel pBackground;
    }
}