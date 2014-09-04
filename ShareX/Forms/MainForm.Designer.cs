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
            this.tsmiRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFTPClient = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHashCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDNSChanger = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTweetMessage = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tsmiTrayRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayFTPClient = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayHashCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayIndexFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayImageEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayMonitorTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayDNSChanger = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayQRCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTrayTweetMessage = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.pBackground.SuspendLayout();
            this.cmsTaskInfo.SuspendLayout();
            this.cmsTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMain
            // 
            this.tsMain.AutoSize = false;
            this.tsMain.CanOverflow = false;
            this.tsMain.Dock = System.Windows.Forms.DockStyle.Left;
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
            this.tsMain.Location = new System.Drawing.Point(0, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Padding = new System.Windows.Forms.Padding(6);
            this.tsMain.ShowItemToolTips = false;
            this.tsMain.Size = new System.Drawing.Size(170, 407);
            this.tsMain.TabIndex = 1;
            this.tsMain.TabStop = true;
            // 
            // tsddbCapture
            // 
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
            this.tsddbCapture.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbCapture.Name = "tsddbCapture";
            this.tsddbCapture.Size = new System.Drawing.Size(157, 20);
            this.tsddbCapture.Text = "Capture";
            this.tsddbCapture.DropDownOpening += new System.EventHandler(this.tsddbCapture_DropDownOpening);
            // 
            // tsmiFullscreen
            // 
            this.tsmiFullscreen.Image = global::ShareX.Properties.Resources.layer;
            this.tsmiFullscreen.Name = "tsmiFullscreen";
            this.tsmiFullscreen.Size = new System.Drawing.Size(217, 22);
            this.tsmiFullscreen.Text = "Fullscreen";
            this.tsmiFullscreen.Click += new System.EventHandler(this.tsmiFullscreen_Click);
            // 
            // tsmiWindow
            // 
            this.tsmiWindow.Image = global::ShareX.Properties.Resources.application_blue;
            this.tsmiWindow.Name = "tsmiWindow";
            this.tsmiWindow.Size = new System.Drawing.Size(217, 22);
            this.tsmiWindow.Text = "Window";
            // 
            // tsmiMonitor
            // 
            this.tsmiMonitor.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiMonitor.Name = "tsmiMonitor";
            this.tsmiMonitor.Size = new System.Drawing.Size(217, 22);
            this.tsmiMonitor.Text = "Monitor";
            // 
            // tsmiRectangle
            // 
            this.tsmiRectangle.Image = global::ShareX.Properties.Resources.layer_shape;
            this.tsmiRectangle.Name = "tsmiRectangle";
            this.tsmiRectangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiRectangle.Text = "Rectangle";
            this.tsmiRectangle.Click += new System.EventHandler(this.tsmiRectangle_Click);
            // 
            // tsmiWindowRectangle
            // 
            this.tsmiWindowRectangle.Image = global::ShareX.Properties.Resources.layers_ungroup;
            this.tsmiWindowRectangle.Name = "tsmiWindowRectangle";
            this.tsmiWindowRectangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiWindowRectangle.Text = "Rectangle (Objects)";
            this.tsmiWindowRectangle.Click += new System.EventHandler(this.tsmiWindowRectangle_Click);
            // 
            // tsmiRectangleAnnotate
            // 
            this.tsmiRectangleAnnotate.Image = global::ShareX.Properties.Resources.layer_pencil;
            this.tsmiRectangleAnnotate.Name = "tsmiRectangleAnnotate";
            this.tsmiRectangleAnnotate.Size = new System.Drawing.Size(217, 22);
            this.tsmiRectangleAnnotate.Text = "Rectangle (Annotate)";
            this.tsmiRectangleAnnotate.Click += new System.EventHandler(this.tsmiRectangleAnnotate_Click);
            // 
            // tsmiRectangleLight
            // 
            this.tsmiRectangleLight.Image = global::ShareX.Properties.Resources.Rectangle;
            this.tsmiRectangleLight.Name = "tsmiRectangleLight";
            this.tsmiRectangleLight.Size = new System.Drawing.Size(217, 22);
            this.tsmiRectangleLight.Text = "Rectangle (Light)";
            this.tsmiRectangleLight.Click += new System.EventHandler(this.tsmiRectangleLight_Click);
            // 
            // tsmiRoundedRectangle
            // 
            this.tsmiRoundedRectangle.Image = global::ShareX.Properties.Resources.layer_shape_round;
            this.tsmiRoundedRectangle.Name = "tsmiRoundedRectangle";
            this.tsmiRoundedRectangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiRoundedRectangle.Text = "Rounded rectangle";
            this.tsmiRoundedRectangle.Click += new System.EventHandler(this.tsmiRoundedRectangle_Click);
            // 
            // tsmiEllipse
            // 
            this.tsmiEllipse.Image = global::ShareX.Properties.Resources.layer_shape_ellipse;
            this.tsmiEllipse.Name = "tsmiEllipse";
            this.tsmiEllipse.Size = new System.Drawing.Size(217, 22);
            this.tsmiEllipse.Text = "Ellipse";
            this.tsmiEllipse.Click += new System.EventHandler(this.tsmiEllipse_Click);
            // 
            // tsmiTriangle
            // 
            this.tsmiTriangle.Image = global::ShareX.Properties.Resources.Triangle;
            this.tsmiTriangle.Name = "tsmiTriangle";
            this.tsmiTriangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiTriangle.Text = "Triangle";
            this.tsmiTriangle.Click += new System.EventHandler(this.tsmiTriangle_Click);
            // 
            // tsmiDiamond
            // 
            this.tsmiDiamond.Image = global::ShareX.Properties.Resources.Diamond;
            this.tsmiDiamond.Name = "tsmiDiamond";
            this.tsmiDiamond.Size = new System.Drawing.Size(217, 22);
            this.tsmiDiamond.Text = "Diamond";
            this.tsmiDiamond.Click += new System.EventHandler(this.tsmiDiamond_Click);
            // 
            // tsmiPolygon
            // 
            this.tsmiPolygon.Image = global::ShareX.Properties.Resources.layer_shape_polygon;
            this.tsmiPolygon.Name = "tsmiPolygon";
            this.tsmiPolygon.Size = new System.Drawing.Size(217, 22);
            this.tsmiPolygon.Text = "Polygon";
            this.tsmiPolygon.Click += new System.EventHandler(this.tsmiPolygon_Click);
            // 
            // tsmiFreeHand
            // 
            this.tsmiFreeHand.Image = global::ShareX.Properties.Resources.layer_shape_curve;
            this.tsmiFreeHand.Name = "tsmiFreeHand";
            this.tsmiFreeHand.Size = new System.Drawing.Size(217, 22);
            this.tsmiFreeHand.Text = "Freehand";
            this.tsmiFreeHand.Click += new System.EventHandler(this.tsmiFreeHand_Click);
            // 
            // tsmiLastRegion
            // 
            this.tsmiLastRegion.Image = global::ShareX.Properties.Resources.layers_arrange;
            this.tsmiLastRegion.Name = "tsmiLastRegion";
            this.tsmiLastRegion.Size = new System.Drawing.Size(217, 22);
            this.tsmiLastRegion.Text = "Last region";
            this.tsmiLastRegion.Click += new System.EventHandler(this.tsmiLastRegion_Click);
            // 
            // tsmiScreenRecordingFFmpeg
            // 
            this.tsmiScreenRecordingFFmpeg.Image = global::ShareX.Properties.Resources.film;
            this.tsmiScreenRecordingFFmpeg.Name = "tsmiScreenRecordingFFmpeg";
            this.tsmiScreenRecordingFFmpeg.Size = new System.Drawing.Size(217, 22);
            this.tsmiScreenRecordingFFmpeg.Text = "Screen recording (FFmpeg)";
            this.tsmiScreenRecordingFFmpeg.Click += new System.EventHandler(this.tsmiScreenRecordingFFmpeg_Click);
            // 
            // tsmiScreenRecordingGIF
            // 
            this.tsmiScreenRecordingGIF.Image = global::ShareX.Properties.Resources.camcorder_image;
            this.tsmiScreenRecordingGIF.Name = "tsmiScreenRecordingGIF";
            this.tsmiScreenRecordingGIF.Size = new System.Drawing.Size(217, 22);
            this.tsmiScreenRecordingGIF.Text = "Screen recording (GIF)";
            this.tsmiScreenRecordingGIF.Click += new System.EventHandler(this.tsmiScreenRecordingGIF_Click);
            // 
            // tsmiAutoCapture
            // 
            this.tsmiAutoCapture.Image = global::ShareX.Properties.Resources.clock;
            this.tsmiAutoCapture.Name = "tsmiAutoCapture";
            this.tsmiAutoCapture.Size = new System.Drawing.Size(217, 22);
            this.tsmiAutoCapture.Text = "Auto capture...";
            this.tsmiAutoCapture.Click += new System.EventHandler(this.tsmiAutoCapture_Click);
            // 
            // tsddbUpload
            // 
            this.tsddbUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUploadFile,
            this.tsmiUploadFolder,
            this.tsmiUploadClipboard,
            this.tsmiUploadURL,
            this.tsmiUploadDragDrop});
            this.tsddbUpload.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsddbUpload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbUpload.Name = "tsddbUpload";
            this.tsddbUpload.Size = new System.Drawing.Size(157, 20);
            this.tsddbUpload.Text = "Upload";
            // 
            // tsmiUploadFile
            // 
            this.tsmiUploadFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiUploadFile.Name = "tsmiUploadFile";
            this.tsmiUploadFile.Size = new System.Drawing.Size(203, 22);
            this.tsmiUploadFile.Text = "Upload file...";
            this.tsmiUploadFile.Click += new System.EventHandler(this.tsbFileUpload_Click);
            // 
            // tsmiUploadFolder
            // 
            this.tsmiUploadFolder.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiUploadFolder.Name = "tsmiUploadFolder";
            this.tsmiUploadFolder.Size = new System.Drawing.Size(203, 22);
            this.tsmiUploadFolder.Text = "Upload folder...";
            this.tsmiUploadFolder.Click += new System.EventHandler(this.tsmiUploadFolder_Click);
            // 
            // tsmiUploadClipboard
            // 
            this.tsmiUploadClipboard.Image = global::ShareX.Properties.Resources.clipboard;
            this.tsmiUploadClipboard.Name = "tsmiUploadClipboard";
            this.tsmiUploadClipboard.Size = new System.Drawing.Size(203, 22);
            this.tsmiUploadClipboard.Text = "Upload from clipboard...";
            this.tsmiUploadClipboard.Click += new System.EventHandler(this.tsbClipboardUpload_Click);
            // 
            // tsmiUploadURL
            // 
            this.tsmiUploadURL.Image = global::ShareX.Properties.Resources.drive;
            this.tsmiUploadURL.Name = "tsmiUploadURL";
            this.tsmiUploadURL.Size = new System.Drawing.Size(203, 22);
            this.tsmiUploadURL.Text = "Upload from URL...";
            this.tsmiUploadURL.Click += new System.EventHandler(this.tsmiUploadURL_Click);
            // 
            // tsmiUploadDragDrop
            // 
            this.tsmiUploadDragDrop.Image = global::ShareX.Properties.Resources.inbox;
            this.tsmiUploadDragDrop.Name = "tsmiUploadDragDrop";
            this.tsmiUploadDragDrop.Size = new System.Drawing.Size(203, 22);
            this.tsmiUploadDragDrop.Text = "Drag and drop upload...";
            this.tsmiUploadDragDrop.Click += new System.EventHandler(this.tsbDragDropUpload_Click);
            // 
            // tsddbWorkflows
            // 
            this.tsddbWorkflows.Image = global::ShareX.Properties.Resources.categories;
            this.tsddbWorkflows.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.tsddbWorkflows.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbWorkflows.Name = "tsddbWorkflows";
            this.tsddbWorkflows.Size = new System.Drawing.Size(157, 20);
            this.tsddbWorkflows.Text = "Workflows";
            // 
            // tsddbTools
            // 
            this.tsddbTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScreenColorPicker,
            this.tsmiRuler,
            this.tsmiFTPClient,
            this.tsmiHashCheck,
            this.tsmiIndexFolder,
            this.tsmiImageEditor,
            this.tsmiImageEffects,
            this.tsmiMonitorTest,
            this.tsmiDNSChanger,
            this.tsmiQRCode,
            this.tsmiTweetMessage});
            this.tsddbTools.Image = global::ShareX.Properties.Resources.toolbox;
            this.tsddbTools.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbTools.Name = "tsddbTools";
            this.tsddbTools.Size = new System.Drawing.Size(157, 20);
            this.tsddbTools.Text = "Tools";
            // 
            // tsmiScreenColorPicker
            // 
            this.tsmiScreenColorPicker.Image = global::ShareX.Properties.Resources.color;
            this.tsmiScreenColorPicker.Name = "tsmiScreenColorPicker";
            this.tsmiScreenColorPicker.Size = new System.Drawing.Size(183, 22);
            this.tsmiScreenColorPicker.Text = "Screen color picker...";
            this.tsmiScreenColorPicker.Click += new System.EventHandler(this.tsmiCursorHelper_Click);
            // 
            // tsmiRuler
            // 
            this.tsmiRuler.Image = global::ShareX.Properties.Resources.ruler_triangle;
            this.tsmiRuler.Name = "tsmiRuler";
            this.tsmiRuler.Size = new System.Drawing.Size(183, 22);
            this.tsmiRuler.Text = "Ruler...";
            this.tsmiRuler.Click += new System.EventHandler(this.tsmiRuler_Click);
            // 
            // tsmiFTPClient
            // 
            this.tsmiFTPClient.Image = global::ShareX.Properties.Resources.application_network;
            this.tsmiFTPClient.Name = "tsmiFTPClient";
            this.tsmiFTPClient.Size = new System.Drawing.Size(183, 22);
            this.tsmiFTPClient.Text = "FTP client...";
            this.tsmiFTPClient.Click += new System.EventHandler(this.tsmiFTPClient_Click);
            // 
            // tsmiHashCheck
            // 
            this.tsmiHashCheck.Image = global::ShareX.Properties.Resources.application_task;
            this.tsmiHashCheck.Name = "tsmiHashCheck";
            this.tsmiHashCheck.Size = new System.Drawing.Size(183, 22);
            this.tsmiHashCheck.Text = "Hash check...";
            this.tsmiHashCheck.Click += new System.EventHandler(this.tsmiHashCheck_Click);
            // 
            // tsmiIndexFolder
            // 
            this.tsmiIndexFolder.Image = global::ShareX.Properties.Resources.folder_tree;
            this.tsmiIndexFolder.Name = "tsmiIndexFolder";
            this.tsmiIndexFolder.Size = new System.Drawing.Size(183, 22);
            this.tsmiIndexFolder.Text = "Index folder...";
            this.tsmiIndexFolder.Click += new System.EventHandler(this.tsmiIndexFolder_Click);
            // 
            // tsmiImageEditor
            // 
            this.tsmiImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiImageEditor.Name = "tsmiImageEditor";
            this.tsmiImageEditor.Size = new System.Drawing.Size(183, 22);
            this.tsmiImageEditor.Text = "Image editor...";
            this.tsmiImageEditor.Click += new System.EventHandler(this.tsmiImageEditor_Click);
            // 
            // tsmiImageEffects
            // 
            this.tsmiImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiImageEffects.Name = "tsmiImageEffects";
            this.tsmiImageEffects.Size = new System.Drawing.Size(183, 22);
            this.tsmiImageEffects.Text = "Image effects...";
            this.tsmiImageEffects.Click += new System.EventHandler(this.tsmiImageEffects_Click);
            // 
            // tsmiMonitorTest
            // 
            this.tsmiMonitorTest.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiMonitorTest.Name = "tsmiMonitorTest";
            this.tsmiMonitorTest.Size = new System.Drawing.Size(183, 22);
            this.tsmiMonitorTest.Text = "Monitor test...";
            this.tsmiMonitorTest.Click += new System.EventHandler(this.tsmiMonitorTest_Click);
            // 
            // tsmiDNSChanger
            // 
            this.tsmiDNSChanger.Image = global::ShareX.Properties.Resources.network_ip;
            this.tsmiDNSChanger.Name = "tsmiDNSChanger";
            this.tsmiDNSChanger.Size = new System.Drawing.Size(183, 22);
            this.tsmiDNSChanger.Text = "DNS changer...";
            this.tsmiDNSChanger.Click += new System.EventHandler(this.tsmiDNSChanger_Click);
            // 
            // tsmiQRCode
            // 
            this.tsmiQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiQRCode.Name = "tsmiQRCode";
            this.tsmiQRCode.Size = new System.Drawing.Size(183, 22);
            this.tsmiQRCode.Text = "QR code...";
            this.tsmiQRCode.Click += new System.EventHandler(this.tsmiQRCode_Click);
            // 
            // tsmiTweetMessage
            // 
            this.tsmiTweetMessage.Image = global::ShareX.Properties.Resources.Twitter;
            this.tsmiTweetMessage.Name = "tsmiTweetMessage";
            this.tsmiTweetMessage.Size = new System.Drawing.Size(183, 22);
            this.tsmiTweetMessage.Text = "Tweet message...";
            this.tsmiTweetMessage.Click += new System.EventHandler(this.tsmiTweetMessage_Click);
            // 
            // tssMain1
            // 
            this.tssMain1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tssMain1.Name = "tssMain1";
            this.tssMain1.Size = new System.Drawing.Size(157, 6);
            // 
            // tsddbAfterCaptureTasks
            // 
            this.tsddbAfterCaptureTasks.Image = global::ShareX.Properties.Resources.image_export;
            this.tsddbAfterCaptureTasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbAfterCaptureTasks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbAfterCaptureTasks.Name = "tsddbAfterCaptureTasks";
            this.tsddbAfterCaptureTasks.Size = new System.Drawing.Size(157, 20);
            this.tsddbAfterCaptureTasks.Text = "After capture tasks";
            // 
            // tsddbAfterUploadTasks
            // 
            this.tsddbAfterUploadTasks.Image = global::ShareX.Properties.Resources.upload_cloud;
            this.tsddbAfterUploadTasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbAfterUploadTasks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbAfterUploadTasks.Name = "tsddbAfterUploadTasks";
            this.tsddbAfterUploadTasks.Size = new System.Drawing.Size(157, 20);
            this.tsddbAfterUploadTasks.Text = "After upload tasks";
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
            this.tsmiDestinationSettings});
            this.tsddbDestinations.Image = global::ShareX.Properties.Resources.drive_globe;
            this.tsddbDestinations.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbDestinations.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbDestinations.Name = "tsddbDestinations";
            this.tsddbDestinations.Size = new System.Drawing.Size(157, 20);
            this.tsddbDestinations.Text = "Destinations";
            this.tsddbDestinations.DropDownOpened += new System.EventHandler(this.tsddbDestinations_DropDownOpened);
            // 
            // tsmiImageUploaders
            // 
            this.tsmiImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiImageUploaders.Name = "tsmiImageUploaders";
            this.tsmiImageUploaders.Size = new System.Drawing.Size(187, 22);
            this.tsmiImageUploaders.Text = "Image uploaders";
            // 
            // tsmiTextUploaders
            // 
            this.tsmiTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTextUploaders.Name = "tsmiTextUploaders";
            this.tsmiTextUploaders.Size = new System.Drawing.Size(187, 22);
            this.tsmiTextUploaders.Text = "Text uploaders";
            // 
            // tsmiFileUploaders
            // 
            this.tsmiFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiFileUploaders.Name = "tsmiFileUploaders";
            this.tsmiFileUploaders.Size = new System.Drawing.Size(187, 22);
            this.tsmiFileUploaders.Text = "File uploaders";
            // 
            // tsmiURLShorteners
            // 
            this.tsmiURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiURLShorteners.Name = "tsmiURLShorteners";
            this.tsmiURLShorteners.Size = new System.Drawing.Size(187, 22);
            this.tsmiURLShorteners.Text = "URL shorteners";
            // 
            // tsmiURLSharingServices
            // 
            this.tsmiURLSharingServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiURLSharingServices.Name = "tsmiURLSharingServices";
            this.tsmiURLSharingServices.Size = new System.Drawing.Size(187, 22);
            this.tsmiURLSharingServices.Text = "URL sharing services";
            // 
            // tssDestinations1
            // 
            this.tssDestinations1.Name = "tssDestinations1";
            this.tssDestinations1.Size = new System.Drawing.Size(184, 6);
            // 
            // tsmiDestinationSettings
            // 
            this.tsmiDestinationSettings.Image = global::ShareX.Properties.Resources.globe_pencil;
            this.tsmiDestinationSettings.Name = "tsmiDestinationSettings";
            this.tsmiDestinationSettings.Size = new System.Drawing.Size(187, 22);
            this.tsmiDestinationSettings.Text = "Destination settings...";
            this.tsmiDestinationSettings.Click += new System.EventHandler(this.tsbDestinationSettings_Click);
            // 
            // tsbApplicationSettings
            // 
            this.tsbApplicationSettings.Image = global::ShareX.Properties.Resources.wrench_screwdriver;
            this.tsbApplicationSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbApplicationSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbApplicationSettings.Name = "tsbApplicationSettings";
            this.tsbApplicationSettings.Size = new System.Drawing.Size(157, 20);
            this.tsbApplicationSettings.Text = "Application settings...";
            this.tsbApplicationSettings.Click += new System.EventHandler(this.tsbApplicationSettings_Click);
            // 
            // tsbTaskSettings
            // 
            this.tsbTaskSettings.Image = global::ShareX.Properties.Resources.gear;
            this.tsbTaskSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbTaskSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTaskSettings.Name = "tsbTaskSettings";
            this.tsbTaskSettings.Size = new System.Drawing.Size(157, 20);
            this.tsbTaskSettings.Text = "Task settings...";
            this.tsbTaskSettings.Click += new System.EventHandler(this.tsbTaskSettings_Click);
            // 
            // tsbHotkeySettings
            // 
            this.tsbHotkeySettings.Image = global::ShareX.Properties.Resources.keyboard;
            this.tsbHotkeySettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbHotkeySettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHotkeySettings.Name = "tsbHotkeySettings";
            this.tsbHotkeySettings.Size = new System.Drawing.Size(157, 20);
            this.tsbHotkeySettings.Text = "Hotkey settings...";
            this.tsbHotkeySettings.Click += new System.EventHandler(this.tsbHotkeySettings_Click);
            // 
            // tssMain2
            // 
            this.tssMain2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tssMain2.Name = "tssMain2";
            this.tssMain2.Size = new System.Drawing.Size(157, 6);
            // 
            // tsbScreenshotsFolder
            // 
            this.tsbScreenshotsFolder.Image = global::ShareX.Properties.Resources.folder_open_image;
            this.tsbScreenshotsFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbScreenshotsFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScreenshotsFolder.Name = "tsbScreenshotsFolder";
            this.tsbScreenshotsFolder.Size = new System.Drawing.Size(157, 20);
            this.tsbScreenshotsFolder.Text = "Screenshots folder...";
            this.tsbScreenshotsFolder.Click += new System.EventHandler(this.tsbScreenshotsFolder_Click);
            // 
            // tsbHistory
            // 
            this.tsbHistory.Image = global::ShareX.Properties.Resources.application_blog;
            this.tsbHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbHistory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHistory.Name = "tsbHistory";
            this.tsbHistory.Size = new System.Drawing.Size(157, 20);
            this.tsbHistory.Text = "History...";
            this.tsbHistory.Click += new System.EventHandler(this.tsbHistory_Click);
            // 
            // tsbImageHistory
            // 
            this.tsbImageHistory.Image = global::ShareX.Properties.Resources.application_icon_large;
            this.tsbImageHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbImageHistory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImageHistory.Name = "tsbImageHistory";
            this.tsbImageHistory.Size = new System.Drawing.Size(157, 20);
            this.tsbImageHistory.Text = "Image history...";
            this.tsbImageHistory.Click += new System.EventHandler(this.tsbImageHistory_Click);
            // 
            // tsddbDebug
            // 
            this.tsddbDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowDebugLog,
            this.tsmiTestImageUpload,
            this.tsmiTestTextUpload,
            this.tsmiTestFileUpload,
            this.tsmiTestURLShortener,
            this.tsmiTestURLSharing,
            this.tsmiTestUploaders});
            this.tsddbDebug.Image = global::ShareX.Properties.Resources.traffic_cone;
            this.tsddbDebug.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsddbDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbDebug.Name = "tsddbDebug";
            this.tsddbDebug.Size = new System.Drawing.Size(157, 20);
            this.tsddbDebug.Text = "Debug";
            // 
            // tsmiShowDebugLog
            // 
            this.tsmiShowDebugLog.Image = global::ShareX.Properties.Resources.application_monitor;
            this.tsmiShowDebugLog.Name = "tsmiShowDebugLog";
            this.tsmiShowDebugLog.Size = new System.Drawing.Size(173, 22);
            this.tsmiShowDebugLog.Text = "Debug log...";
            this.tsmiShowDebugLog.Click += new System.EventHandler(this.tsmiShowDebugLog_Click);
            // 
            // tsmiTestImageUpload
            // 
            this.tsmiTestImageUpload.Image = global::ShareX.Properties.Resources.image;
            this.tsmiTestImageUpload.Name = "tsmiTestImageUpload";
            this.tsmiTestImageUpload.Size = new System.Drawing.Size(173, 22);
            this.tsmiTestImageUpload.Text = "Test image upload";
            this.tsmiTestImageUpload.Click += new System.EventHandler(this.tsmiTestImageUpload_Click);
            // 
            // tsmiTestTextUpload
            // 
            this.tsmiTestTextUpload.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTestTextUpload.Name = "tsmiTestTextUpload";
            this.tsmiTestTextUpload.Size = new System.Drawing.Size(173, 22);
            this.tsmiTestTextUpload.Text = "Test text upload";
            this.tsmiTestTextUpload.Click += new System.EventHandler(this.tsmiTestTextUpload_Click);
            // 
            // tsmiTestFileUpload
            // 
            this.tsmiTestFileUpload.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiTestFileUpload.Name = "tsmiTestFileUpload";
            this.tsmiTestFileUpload.Size = new System.Drawing.Size(173, 22);
            this.tsmiTestFileUpload.Text = "Test file upload";
            this.tsmiTestFileUpload.Click += new System.EventHandler(this.tsmiTestFileUpload_Click);
            // 
            // tsmiTestURLShortener
            // 
            this.tsmiTestURLShortener.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTestURLShortener.Name = "tsmiTestURLShortener";
            this.tsmiTestURLShortener.Size = new System.Drawing.Size(173, 22);
            this.tsmiTestURLShortener.Text = "Test URL shortener";
            this.tsmiTestURLShortener.Click += new System.EventHandler(this.tsmiTestURLShortener_Click);
            // 
            // tsmiTestURLSharing
            // 
            this.tsmiTestURLSharing.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiTestURLSharing.Name = "tsmiTestURLSharing";
            this.tsmiTestURLSharing.Size = new System.Drawing.Size(173, 22);
            this.tsmiTestURLSharing.Text = "Test URL sharing";
            this.tsmiTestURLSharing.Click += new System.EventHandler(this.tsmiTestURLSharing_Click);
            // 
            // tsmiTestUploaders
            // 
            this.tsmiTestUploaders.Image = global::ShareX.Properties.Resources.application_browser;
            this.tsmiTestUploaders.Name = "tsmiTestUploaders";
            this.tsmiTestUploaders.Size = new System.Drawing.Size(173, 22);
            this.tsmiTestUploaders.Text = "Test uploaders...";
            this.tsmiTestUploaders.Visible = false;
            this.tsmiTestUploaders.Click += new System.EventHandler(this.tsmiTestUploaders_Click);
            // 
            // tsmiDonate
            // 
            this.tsmiDonate.Image = global::ShareX.Properties.Resources.heart;
            this.tsmiDonate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsmiDonate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiDonate.Name = "tsmiDonate";
            this.tsmiDonate.Size = new System.Drawing.Size(157, 20);
            this.tsmiDonate.Text = "Donate...";
            this.tsmiDonate.Click += new System.EventHandler(this.tsbDonate_Click);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Image = global::ShareX.Properties.Resources.crown;
            this.tsmiAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsmiAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(157, 20);
            this.tsmiAbout.Text = "About...";
            this.tsmiAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Location = new System.Drawing.Point(170, 0);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.pBackground);
            this.scMain.Panel1.Controls.Add(this.lblSplitter);
            this.scMain.Panel1.Controls.Add(this.lvUploads);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pbPreview);
            this.scMain.Size = new System.Drawing.Size(664, 407);
            this.scMain.SplitterDistance = 335;
            this.scMain.SplitterWidth = 6;
            this.scMain.TabIndex = 1;
            this.scMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.scMain_SplitterMoved);
            // 
            // pBackground
            // 
            this.pBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBackground.BackColor = System.Drawing.Color.White;
            this.pBackground.Controls.Add(this.pbLogo);
            this.pBackground.Controls.Add(this.lblDragAndDropTip);
            this.pBackground.Location = new System.Drawing.Point(8, 40);
            this.pBackground.Name = "pBackground";
            this.pBackground.Size = new System.Drawing.Size(320, 344);
            this.pBackground.TabIndex = 3;
            // 
            // pbLogo
            // 
            this.pbLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLogo.BackColor = System.Drawing.Color.White;
            this.pbLogo.Location = new System.Drawing.Point(0, 0);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(320, 280);
            this.pbLogo.TabIndex = 2;
            this.pbLogo.TabStop = false;
            this.pbLogo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDragAndDropTip_MouseUp);
            // 
            // lblDragAndDropTip
            // 
            this.lblDragAndDropTip.BackColor = System.Drawing.Color.White;
            this.lblDragAndDropTip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDragAndDropTip.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDragAndDropTip.ForeColor = System.Drawing.Color.DarkGray;
            this.lblDragAndDropTip.Location = new System.Drawing.Point(0, 280);
            this.lblDragAndDropTip.Name = "lblDragAndDropTip";
            this.lblDragAndDropTip.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lblDragAndDropTip.Size = new System.Drawing.Size(320, 64);
            this.lblDragAndDropTip.TabIndex = 1;
            this.lblDragAndDropTip.Text = "You can drag and drop files to this window";
            this.lblDragAndDropTip.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDragAndDropTip.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDragAndDropTip_MouseUp);
            // 
            // lblSplitter
            // 
            this.lblSplitter.BackColor = System.Drawing.Color.Black;
            this.lblSplitter.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblSplitter.Location = new System.Drawing.Point(0, 0);
            this.lblSplitter.Name = "lblSplitter";
            this.lblSplitter.Size = new System.Drawing.Size(1, 407);
            this.lblSplitter.TabIndex = 0;
            // 
            // lvUploads
            // 
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
            this.lvUploads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvUploads.FullRowSelect = true;
            this.lvUploads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUploads.HideSelection = false;
            this.lvUploads.Location = new System.Drawing.Point(0, 0);
            this.lvUploads.Name = "lvUploads";
            this.lvUploads.ShowItemToolTips = true;
            this.lvUploads.Size = new System.Drawing.Size(335, 407);
            this.lvUploads.TabIndex = 2;
            this.lvUploads.UseCompatibleStateImageBehavior = false;
            this.lvUploads.View = System.Windows.Forms.View.Details;
            this.lvUploads.SelectedIndexChanged += new System.EventHandler(this.lvUploads_SelectedIndexChanged);
            this.lvUploads.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvUploads_KeyDown);
            this.lvUploads.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvUploads_MouseDoubleClick);
            this.lvUploads.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvUploads_MouseUp);
            // 
            // chFilename
            // 
            this.chFilename.Text = "Filename";
            this.chFilename.Width = 150;
            // 
            // chStatus
            // 
            this.chStatus.Text = "Status";
            // 
            // chProgress
            // 
            this.chProgress.Text = "Progress";
            this.chProgress.Width = 125;
            // 
            // chSpeed
            // 
            this.chSpeed.Text = "Speed";
            this.chSpeed.Width = 75;
            // 
            // chElapsed
            // 
            this.chElapsed.Text = "Elapsed";
            this.chElapsed.Width = 42;
            // 
            // chRemaining
            // 
            this.chRemaining.Text = "Remaining";
            this.chRemaining.Width = 42;
            // 
            // chURL
            // 
            this.chURL.Text = "URL";
            this.chURL.Width = 150;
            // 
            // pbPreview
            // 
            this.pbPreview.AccessibleDescription = "";
            this.pbPreview.AccessibleName = "";
            this.pbPreview.BackColor = System.Drawing.Color.White;
            this.pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.EnableRightClickMenu = true;
            this.pbPreview.FullscreenOnClick = true;
            this.pbPreview.Location = new System.Drawing.Point(0, 0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(323, 407);
            this.pbPreview.TabIndex = 0;
            // 
            // cmsTaskInfo
            // 
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
            this.cmsTaskInfo.Size = new System.Drawing.Size(164, 318);
            // 
            // tsmiShowErrors
            // 
            this.tsmiShowErrors.Image = global::ShareX.Properties.Resources.exclamation_button;
            this.tsmiShowErrors.Name = "tsmiShowErrors";
            this.tsmiShowErrors.Size = new System.Drawing.Size(163, 22);
            this.tsmiShowErrors.Text = "Show errors";
            this.tsmiShowErrors.Click += new System.EventHandler(this.tsmiShowErrors_Click);
            // 
            // tsmiStopUpload
            // 
            this.tsmiStopUpload.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiStopUpload.Name = "tsmiStopUpload";
            this.tsmiStopUpload.Size = new System.Drawing.Size(163, 22);
            this.tsmiStopUpload.Text = "Stop upload";
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
            this.tsmiOpen.Size = new System.Drawing.Size(163, 22);
            this.tsmiOpen.Text = "Open";
            // 
            // tsmiOpenURL
            // 
            this.tsmiOpenURL.Name = "tsmiOpenURL";
            this.tsmiOpenURL.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenURL.Text = "URL";
            this.tsmiOpenURL.Click += new System.EventHandler(this.tsmiOpenURL_Click);
            // 
            // tsmiOpenShortenedURL
            // 
            this.tsmiOpenShortenedURL.Name = "tsmiOpenShortenedURL";
            this.tsmiOpenShortenedURL.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenShortenedURL.Text = "Shortened URL";
            this.tsmiOpenShortenedURL.Click += new System.EventHandler(this.tsmiOpenShortenedURL_Click);
            // 
            // tsmiOpenThumbnailURL
            // 
            this.tsmiOpenThumbnailURL.Name = "tsmiOpenThumbnailURL";
            this.tsmiOpenThumbnailURL.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenThumbnailURL.Text = "Thumbnail URL";
            this.tsmiOpenThumbnailURL.Click += new System.EventHandler(this.tsmiOpenThumbnailURL_Click);
            // 
            // tsmiOpenDeletionURL
            // 
            this.tsmiOpenDeletionURL.Name = "tsmiOpenDeletionURL";
            this.tsmiOpenDeletionURL.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenDeletionURL.Text = "Deletion URL";
            this.tsmiOpenDeletionURL.Click += new System.EventHandler(this.tsmiOpenDeletionURL_Click);
            // 
            // tssOpen1
            // 
            this.tssOpen1.Name = "tssOpen1";
            this.tssOpen1.Size = new System.Drawing.Size(153, 6);
            // 
            // tsmiOpenFile
            // 
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            this.tsmiOpenFile.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenFile.Text = "File";
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiOpenFolder
            // 
            this.tsmiOpenFolder.Name = "tsmiOpenFolder";
            this.tsmiOpenFolder.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenFolder.Text = "Folder";
            this.tsmiOpenFolder.Click += new System.EventHandler(this.tsmiOpenFolder_Click);
            // 
            // tsmiOpenThumbnailFile
            // 
            this.tsmiOpenThumbnailFile.Name = "tsmiOpenThumbnailFile";
            this.tsmiOpenThumbnailFile.Size = new System.Drawing.Size(156, 22);
            this.tsmiOpenThumbnailFile.Text = "Thumbnail file";
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
            this.tsmiCopy.Size = new System.Drawing.Size(163, 22);
            this.tsmiCopy.Text = "Copy";
            // 
            // tsmiCopyURL
            // 
            this.tsmiCopyURL.Name = "tsmiCopyURL";
            this.tsmiCopyURL.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyURL.Text = "URL";
            this.tsmiCopyURL.Click += new System.EventHandler(this.tsmiCopyURL_Click);
            // 
            // tsmiCopyShortenedURL
            // 
            this.tsmiCopyShortenedURL.Name = "tsmiCopyShortenedURL";
            this.tsmiCopyShortenedURL.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyShortenedURL.Text = "Shortened URL";
            this.tsmiCopyShortenedURL.Click += new System.EventHandler(this.tsmiCopyShortenedURL_Click);
            // 
            // tsmiCopyThumbnailURL
            // 
            this.tsmiCopyThumbnailURL.Name = "tsmiCopyThumbnailURL";
            this.tsmiCopyThumbnailURL.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyThumbnailURL.Text = "Thumbnail URL";
            this.tsmiCopyThumbnailURL.Click += new System.EventHandler(this.tsmiCopyThumbnailURL_Click);
            // 
            // tsmiCopyDeletionURL
            // 
            this.tsmiCopyDeletionURL.Name = "tsmiCopyDeletionURL";
            this.tsmiCopyDeletionURL.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyDeletionURL.Text = "Deletion URL";
            this.tsmiCopyDeletionURL.Click += new System.EventHandler(this.tsmiCopyDeletionURL_Click);
            // 
            // tssCopy1
            // 
            this.tssCopy1.Name = "tssCopy1";
            this.tssCopy1.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiCopyFile
            // 
            this.tsmiCopyFile.Name = "tsmiCopyFile";
            this.tsmiCopyFile.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyFile.Text = "File";
            this.tsmiCopyFile.Click += new System.EventHandler(this.tsmiCopyFile_Click);
            // 
            // tsmiCopyImage
            // 
            this.tsmiCopyImage.Name = "tsmiCopyImage";
            this.tsmiCopyImage.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyImage.Text = "Image";
            this.tsmiCopyImage.Click += new System.EventHandler(this.tsmiCopyImage_Click);
            // 
            // tsmiCopyText
            // 
            this.tsmiCopyText.Name = "tsmiCopyText";
            this.tsmiCopyText.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyText.Text = "Text";
            this.tsmiCopyText.Click += new System.EventHandler(this.tsmiCopyText_Click);
            // 
            // tsmiCopyThumbnailFile
            // 
            this.tsmiCopyThumbnailFile.Name = "tsmiCopyThumbnailFile";
            this.tsmiCopyThumbnailFile.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyThumbnailFile.Text = "Thumbnail file";
            this.tsmiCopyThumbnailFile.Click += new System.EventHandler(this.tsmiCopyThumbnailFile_Click);
            // 
            // tsmiCopyThumbnailImage
            // 
            this.tsmiCopyThumbnailImage.Name = "tsmiCopyThumbnailImage";
            this.tsmiCopyThumbnailImage.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyThumbnailImage.Text = "Thumbnail image";
            this.tsmiCopyThumbnailImage.Click += new System.EventHandler(this.tsmiCopyThumbnailImage_Click);
            // 
            // tssCopy2
            // 
            this.tssCopy2.Name = "tssCopy2";
            this.tssCopy2.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiCopyHTMLLink
            // 
            this.tsmiCopyHTMLLink.Name = "tsmiCopyHTMLLink";
            this.tsmiCopyHTMLLink.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyHTMLLink.Text = "HTML link";
            this.tsmiCopyHTMLLink.Click += new System.EventHandler(this.tsmiCopyHTMLLink_Click);
            // 
            // tsmiCopyHTMLImage
            // 
            this.tsmiCopyHTMLImage.Name = "tsmiCopyHTMLImage";
            this.tsmiCopyHTMLImage.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyHTMLImage.Text = "HTML image";
            this.tsmiCopyHTMLImage.Click += new System.EventHandler(this.tsmiCopyHTMLImage_Click);
            // 
            // tsmiCopyHTMLLinkedImage
            // 
            this.tsmiCopyHTMLLinkedImage.Name = "tsmiCopyHTMLLinkedImage";
            this.tsmiCopyHTMLLinkedImage.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyHTMLLinkedImage.Text = "HTML linked image";
            this.tsmiCopyHTMLLinkedImage.Click += new System.EventHandler(this.tsmiCopyHTMLLinkedImage_Click);
            // 
            // tssCopy3
            // 
            this.tssCopy3.Name = "tssCopy3";
            this.tssCopy3.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiCopyForumLink
            // 
            this.tsmiCopyForumLink.Name = "tsmiCopyForumLink";
            this.tsmiCopyForumLink.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyForumLink.Text = "Forum (BBCode) link";
            this.tsmiCopyForumLink.Click += new System.EventHandler(this.tsmiCopyForumLink_Click);
            // 
            // tsmiCopyForumImage
            // 
            this.tsmiCopyForumImage.Name = "tsmiCopyForumImage";
            this.tsmiCopyForumImage.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyForumImage.Text = "Forum (BBCode) image";
            this.tsmiCopyForumImage.Click += new System.EventHandler(this.tsmiCopyForumImage_Click);
            // 
            // tsmiCopyForumLinkedImage
            // 
            this.tsmiCopyForumLinkedImage.Name = "tsmiCopyForumLinkedImage";
            this.tsmiCopyForumLinkedImage.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyForumLinkedImage.Text = "Forum (BBCode) linked image";
            this.tsmiCopyForumLinkedImage.Click += new System.EventHandler(this.tsmiCopyForumLinkedImage_Click);
            // 
            // tssCopy4
            // 
            this.tssCopy4.Name = "tssCopy4";
            this.tssCopy4.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiCopyFilePath
            // 
            this.tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            this.tsmiCopyFilePath.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyFilePath.Text = "File path";
            this.tsmiCopyFilePath.Click += new System.EventHandler(this.tsmiCopyFilePath_Click);
            // 
            // tsmiCopyFileName
            // 
            this.tsmiCopyFileName.Name = "tsmiCopyFileName";
            this.tsmiCopyFileName.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyFileName.Text = "File name";
            this.tsmiCopyFileName.Click += new System.EventHandler(this.tsmiCopyFileName_Click);
            // 
            // tsmiCopyFileNameWithExtension
            // 
            this.tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            this.tsmiCopyFileNameWithExtension.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyFileNameWithExtension.Text = "File name with extension";
            this.tsmiCopyFileNameWithExtension.Click += new System.EventHandler(this.tsmiCopyFileNameWithExtension_Click);
            // 
            // tsmiCopyFolder
            // 
            this.tsmiCopyFolder.Name = "tsmiCopyFolder";
            this.tsmiCopyFolder.Size = new System.Drawing.Size(233, 22);
            this.tsmiCopyFolder.Text = "Folder";
            this.tsmiCopyFolder.Click += new System.EventHandler(this.tsmiCopyFolder_Click);
            // 
            // tssCopy5
            // 
            this.tssCopy5.Name = "tssCopy5";
            this.tssCopy5.Size = new System.Drawing.Size(230, 6);
            this.tssCopy5.Visible = false;
            // 
            // tsmiUploadSelectedFile
            // 
            this.tsmiUploadSelectedFile.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsmiUploadSelectedFile.Name = "tsmiUploadSelectedFile";
            this.tsmiUploadSelectedFile.Size = new System.Drawing.Size(163, 22);
            this.tsmiUploadSelectedFile.Text = "Upload";
            this.tsmiUploadSelectedFile.Click += new System.EventHandler(this.tsmiUploadSelectedFile_Click);
            // 
            // tsmiEditSelectedFile
            // 
            this.tsmiEditSelectedFile.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiEditSelectedFile.Name = "tsmiEditSelectedFile";
            this.tsmiEditSelectedFile.Size = new System.Drawing.Size(163, 22);
            this.tsmiEditSelectedFile.Text = "Edit image...";
            this.tsmiEditSelectedFile.Click += new System.EventHandler(this.tsmiEditSelectedFile_Click);
            // 
            // tsmiDeleteSelectedFile
            // 
            this.tsmiDeleteSelectedFile.Image = global::ShareX.Properties.Resources.bin;
            this.tsmiDeleteSelectedFile.Name = "tsmiDeleteSelectedFile";
            this.tsmiDeleteSelectedFile.Size = new System.Drawing.Size(163, 22);
            this.tsmiDeleteSelectedFile.Text = "Delete file locally";
            this.tsmiDeleteSelectedFile.Click += new System.EventHandler(this.tsmiDeleteSelectedFile_Click);
            // 
            // tsmiShortenSelectedURL
            // 
            this.tsmiShortenSelectedURL.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiShortenSelectedURL.Name = "tsmiShortenSelectedURL";
            this.tsmiShortenSelectedURL.Size = new System.Drawing.Size(163, 22);
            this.tsmiShortenSelectedURL.Text = "Shorten URL";
            // 
            // tsmiShareSelectedURL
            // 
            this.tsmiShareSelectedURL.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiShareSelectedURL.Name = "tsmiShareSelectedURL";
            this.tsmiShareSelectedURL.Size = new System.Drawing.Size(163, 22);
            this.tsmiShareSelectedURL.Text = "Share URL";
            // 
            // tsmiShowQRCode
            // 
            this.tsmiShowQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiShowQRCode.Name = "tsmiShowQRCode";
            this.tsmiShowQRCode.Size = new System.Drawing.Size(163, 22);
            this.tsmiShowQRCode.Text = "Show QR code...";
            this.tsmiShowQRCode.Click += new System.EventHandler(this.tsmiShowQRCode_Click);
            // 
            // tsmiShowResponse
            // 
            this.tsmiShowResponse.Image = global::ShareX.Properties.Resources.application_browser;
            this.tsmiShowResponse.Name = "tsmiShowResponse";
            this.tsmiShowResponse.Size = new System.Drawing.Size(163, 22);
            this.tsmiShowResponse.Text = "Show response...";
            this.tsmiShowResponse.Click += new System.EventHandler(this.tsmiShowResponse_Click);
            // 
            // tsmiClearList
            // 
            this.tsmiClearList.Image = global::ShareX.Properties.Resources.eraser;
            this.tsmiClearList.Name = "tsmiClearList";
            this.tsmiClearList.Size = new System.Drawing.Size(163, 22);
            this.tsmiClearList.Text = "Clear list";
            this.tsmiClearList.Click += new System.EventHandler(this.tsmiClearList_Click);
            // 
            // tssUploadInfo1
            // 
            this.tssUploadInfo1.Name = "tssUploadInfo1";
            this.tssUploadInfo1.Size = new System.Drawing.Size(160, 6);
            // 
            // tsmiHideMenu
            // 
            this.tsmiHideMenu.Image = global::ShareX.Properties.Resources.layout_select_sidebar;
            this.tsmiHideMenu.Name = "tsmiHideMenu";
            this.tsmiHideMenu.Size = new System.Drawing.Size(163, 22);
            this.tsmiHideMenu.Text = "Hide menu";
            this.tsmiHideMenu.Click += new System.EventHandler(this.tsmiHideMenu_Click);
            // 
            // tsmiImagePreview
            // 
            this.tsmiImagePreview.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImagePreviewShow,
            this.tsmiImagePreviewHide,
            this.tsmiImagePreviewAutomatic});
            this.tsmiImagePreview.Image = global::ShareX.Properties.Resources.layout_select_content;
            this.tsmiImagePreview.Name = "tsmiImagePreview";
            this.tsmiImagePreview.Size = new System.Drawing.Size(163, 22);
            this.tsmiImagePreview.Text = "Image preview";
            // 
            // tsmiImagePreviewShow
            // 
            this.tsmiImagePreviewShow.Checked = true;
            this.tsmiImagePreviewShow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiImagePreviewShow.Name = "tsmiImagePreviewShow";
            this.tsmiImagePreviewShow.Size = new System.Drawing.Size(130, 22);
            this.tsmiImagePreviewShow.Text = "Show";
            this.tsmiImagePreviewShow.Click += new System.EventHandler(this.tsmiImagePreviewShow_Click);
            // 
            // tsmiImagePreviewHide
            // 
            this.tsmiImagePreviewHide.Name = "tsmiImagePreviewHide";
            this.tsmiImagePreviewHide.Size = new System.Drawing.Size(130, 22);
            this.tsmiImagePreviewHide.Text = "Hide";
            this.tsmiImagePreviewHide.Click += new System.EventHandler(this.tsmiImagePreviewHide_Click);
            // 
            // tsmiImagePreviewAutomatic
            // 
            this.tsmiImagePreviewAutomatic.Name = "tsmiImagePreviewAutomatic";
            this.tsmiImagePreviewAutomatic.Size = new System.Drawing.Size(130, 22);
            this.tsmiImagePreviewAutomatic.Text = "Automatic";
            this.tsmiImagePreviewAutomatic.Click += new System.EventHandler(this.tsmiImagePreviewAutomatic_Click);
            // 
            // niTray
            // 
            this.niTray.ContextMenuStrip = this.cmsTray;
            this.niTray.Text = "ShareX";
            this.niTray.BalloonTipClicked += new System.EventHandler(this.niTray_BalloonTipClicked);
            this.niTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseDoubleClick);
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
            this.cmsTray.Size = new System.Drawing.Size(189, 396);
            // 
            // tsmiTrayCapture
            // 
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
            this.tsmiTrayCapture.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayCapture.Text = "Capture";
            this.tsmiTrayCapture.DropDownOpening += new System.EventHandler(this.tsmiCapture_DropDownOpening);
            // 
            // tsmiTrayFullscreen
            // 
            this.tsmiTrayFullscreen.Image = global::ShareX.Properties.Resources.layer;
            this.tsmiTrayFullscreen.Name = "tsmiTrayFullscreen";
            this.tsmiTrayFullscreen.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayFullscreen.Text = "Fullscreen";
            this.tsmiTrayFullscreen.Click += new System.EventHandler(this.tsmiTrayFullscreen_Click);
            // 
            // tsmiTrayWindow
            // 
            this.tsmiTrayWindow.Image = global::ShareX.Properties.Resources.application_blue;
            this.tsmiTrayWindow.Name = "tsmiTrayWindow";
            this.tsmiTrayWindow.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayWindow.Text = "Window";
            // 
            // tsmiTrayMonitor
            // 
            this.tsmiTrayMonitor.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiTrayMonitor.Name = "tsmiTrayMonitor";
            this.tsmiTrayMonitor.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayMonitor.Text = "Monitor";
            // 
            // tsmiTrayRectangle
            // 
            this.tsmiTrayRectangle.Image = global::ShareX.Properties.Resources.layer_shape;
            this.tsmiTrayRectangle.Name = "tsmiTrayRectangle";
            this.tsmiTrayRectangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayRectangle.Text = "Rectangle";
            this.tsmiTrayRectangle.Click += new System.EventHandler(this.tsmiTrayRectangle_Click);
            // 
            // tsmiTrayWindowRectangle
            // 
            this.tsmiTrayWindowRectangle.Image = global::ShareX.Properties.Resources.layers_ungroup;
            this.tsmiTrayWindowRectangle.Name = "tsmiTrayWindowRectangle";
            this.tsmiTrayWindowRectangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayWindowRectangle.Text = "Rectangle (Objects)";
            this.tsmiTrayWindowRectangle.Click += new System.EventHandler(this.tsmiTrayWindowRectangle_Click);
            // 
            // tsmiTrayRectangleAnnotate
            // 
            this.tsmiTrayRectangleAnnotate.Image = global::ShareX.Properties.Resources.layer_pencil;
            this.tsmiTrayRectangleAnnotate.Name = "tsmiTrayRectangleAnnotate";
            this.tsmiTrayRectangleAnnotate.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayRectangleAnnotate.Text = "Rectangle (Annotate)";
            this.tsmiTrayRectangleAnnotate.Click += new System.EventHandler(this.tsmiTrayRectangleAnnotate_Click);
            // 
            // tsmiTrayRectangleLight
            // 
            this.tsmiTrayRectangleLight.Image = global::ShareX.Properties.Resources.Rectangle;
            this.tsmiTrayRectangleLight.Name = "tsmiTrayRectangleLight";
            this.tsmiTrayRectangleLight.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayRectangleLight.Text = "Rectangle (Light)";
            this.tsmiTrayRectangleLight.Click += new System.EventHandler(this.tsmiTrayRectangleLight_Click);
            // 
            // tsmiTrayRoundedRectangle
            // 
            this.tsmiTrayRoundedRectangle.Image = global::ShareX.Properties.Resources.layer_shape_round;
            this.tsmiTrayRoundedRectangle.Name = "tsmiTrayRoundedRectangle";
            this.tsmiTrayRoundedRectangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayRoundedRectangle.Text = "Rounded rectangle";
            this.tsmiTrayRoundedRectangle.Click += new System.EventHandler(this.tsmiTrayRoundedRectangle_Click);
            // 
            // tsmiTrayEllipse
            // 
            this.tsmiTrayEllipse.Image = global::ShareX.Properties.Resources.layer_shape_ellipse;
            this.tsmiTrayEllipse.Name = "tsmiTrayEllipse";
            this.tsmiTrayEllipse.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayEllipse.Text = "Ellipse";
            this.tsmiTrayEllipse.Click += new System.EventHandler(this.tsmiTrayEllipse_Click);
            // 
            // tsmiTrayTriangle
            // 
            this.tsmiTrayTriangle.Image = global::ShareX.Properties.Resources.Triangle;
            this.tsmiTrayTriangle.Name = "tsmiTrayTriangle";
            this.tsmiTrayTriangle.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayTriangle.Text = "Triangle";
            this.tsmiTrayTriangle.Click += new System.EventHandler(this.tsmiTrayTriangle_Click);
            // 
            // tsmiTrayDiamond
            // 
            this.tsmiTrayDiamond.Image = global::ShareX.Properties.Resources.Diamond;
            this.tsmiTrayDiamond.Name = "tsmiTrayDiamond";
            this.tsmiTrayDiamond.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayDiamond.Text = "Diamond";
            this.tsmiTrayDiamond.Click += new System.EventHandler(this.tsmiTrayDiamond_Click);
            // 
            // tsmiTrayPolygon
            // 
            this.tsmiTrayPolygon.Image = global::ShareX.Properties.Resources.layer_shape_polygon;
            this.tsmiTrayPolygon.Name = "tsmiTrayPolygon";
            this.tsmiTrayPolygon.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayPolygon.Text = "Polygon";
            this.tsmiTrayPolygon.Click += new System.EventHandler(this.tsmiTrayPolygon_Click);
            // 
            // tsmiTrayFreeHand
            // 
            this.tsmiTrayFreeHand.Image = global::ShareX.Properties.Resources.layer_shape_curve;
            this.tsmiTrayFreeHand.Name = "tsmiTrayFreeHand";
            this.tsmiTrayFreeHand.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayFreeHand.Text = "Freehand";
            this.tsmiTrayFreeHand.Click += new System.EventHandler(this.tsmiTrayFreeHand_Click);
            // 
            // tsmiTrayLastRegion
            // 
            this.tsmiTrayLastRegion.Image = global::ShareX.Properties.Resources.layers_arrange;
            this.tsmiTrayLastRegion.Name = "tsmiTrayLastRegion";
            this.tsmiTrayLastRegion.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayLastRegion.Text = "Last region";
            this.tsmiTrayLastRegion.Click += new System.EventHandler(this.tsmiTrayLastRegion_Click);
            // 
            // screenRecordingFFmpegToolStripMenuItem
            // 
            this.screenRecordingFFmpegToolStripMenuItem.Image = global::ShareX.Properties.Resources.film;
            this.screenRecordingFFmpegToolStripMenuItem.Name = "screenRecordingFFmpegToolStripMenuItem";
            this.screenRecordingFFmpegToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.screenRecordingFFmpegToolStripMenuItem.Text = "Screen recording (FFmpeg)";
            this.screenRecordingFFmpegToolStripMenuItem.Click += new System.EventHandler(this.tsmiScreenRecordingFFmpeg_Click);
            // 
            // tsmiTrayScreenRecordingGIF
            // 
            this.tsmiTrayScreenRecordingGIF.Image = global::ShareX.Properties.Resources.camcorder_image;
            this.tsmiTrayScreenRecordingGIF.Name = "tsmiTrayScreenRecordingGIF";
            this.tsmiTrayScreenRecordingGIF.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayScreenRecordingGIF.Text = "Screen recording (GIF)";
            this.tsmiTrayScreenRecordingGIF.Click += new System.EventHandler(this.tsmiScreenRecordingGIF_Click);
            // 
            // tsmiTrayAutoCapture
            // 
            this.tsmiTrayAutoCapture.Image = global::ShareX.Properties.Resources.clock;
            this.tsmiTrayAutoCapture.Name = "tsmiTrayAutoCapture";
            this.tsmiTrayAutoCapture.Size = new System.Drawing.Size(217, 22);
            this.tsmiTrayAutoCapture.Text = "Auto capture...";
            this.tsmiTrayAutoCapture.Click += new System.EventHandler(this.tsmiAutoCapture_Click);
            // 
            // tsmiTrayUpload
            // 
            this.tsmiTrayUpload.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayUploadFile,
            this.tsmiTrayUploadFolder,
            this.tsmiTrayUploadClipboard,
            this.tsmiTrayUploadURL,
            this.tsmiTrayUploadDragDrop});
            this.tsmiTrayUpload.Image = global::ShareX.Properties.Resources.arrow_090;
            this.tsmiTrayUpload.Name = "tsmiTrayUpload";
            this.tsmiTrayUpload.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayUpload.Text = "Upload";
            // 
            // tsmiTrayUploadFile
            // 
            this.tsmiTrayUploadFile.Image = global::ShareX.Properties.Resources.folder_open_document;
            this.tsmiTrayUploadFile.Name = "tsmiTrayUploadFile";
            this.tsmiTrayUploadFile.Size = new System.Drawing.Size(203, 22);
            this.tsmiTrayUploadFile.Text = "Upload file...";
            this.tsmiTrayUploadFile.Click += new System.EventHandler(this.tsbFileUpload_Click);
            // 
            // tsmiTrayUploadFolder
            // 
            this.tsmiTrayUploadFolder.Image = global::ShareX.Properties.Resources.folder;
            this.tsmiTrayUploadFolder.Name = "tsmiTrayUploadFolder";
            this.tsmiTrayUploadFolder.Size = new System.Drawing.Size(203, 22);
            this.tsmiTrayUploadFolder.Text = "Upload folder...";
            this.tsmiTrayUploadFolder.Click += new System.EventHandler(this.tsmiUploadFolder_Click);
            // 
            // tsmiTrayUploadClipboard
            // 
            this.tsmiTrayUploadClipboard.Image = global::ShareX.Properties.Resources.clipboard;
            this.tsmiTrayUploadClipboard.Name = "tsmiTrayUploadClipboard";
            this.tsmiTrayUploadClipboard.Size = new System.Drawing.Size(203, 22);
            this.tsmiTrayUploadClipboard.Text = "Upload from clipboard...";
            this.tsmiTrayUploadClipboard.Click += new System.EventHandler(this.tsbClipboardUpload_Click);
            // 
            // tsmiTrayUploadURL
            // 
            this.tsmiTrayUploadURL.Image = global::ShareX.Properties.Resources.drive;
            this.tsmiTrayUploadURL.Name = "tsmiTrayUploadURL";
            this.tsmiTrayUploadURL.Size = new System.Drawing.Size(203, 22);
            this.tsmiTrayUploadURL.Text = "Upload from URL...";
            this.tsmiTrayUploadURL.Click += new System.EventHandler(this.tsmiUploadURL_Click);
            // 
            // tsmiTrayUploadDragDrop
            // 
            this.tsmiTrayUploadDragDrop.Image = global::ShareX.Properties.Resources.inbox;
            this.tsmiTrayUploadDragDrop.Name = "tsmiTrayUploadDragDrop";
            this.tsmiTrayUploadDragDrop.Size = new System.Drawing.Size(203, 22);
            this.tsmiTrayUploadDragDrop.Text = "Drag and drop upload...";
            this.tsmiTrayUploadDragDrop.Click += new System.EventHandler(this.tsbDragDropUpload_Click);
            // 
            // tsmiTrayWorkflows
            // 
            this.tsmiTrayWorkflows.Image = global::ShareX.Properties.Resources.categories;
            this.tsmiTrayWorkflows.Name = "tsmiTrayWorkflows";
            this.tsmiTrayWorkflows.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayWorkflows.Text = "Workflows";
            // 
            // tsmiTrayTools
            // 
            this.tsmiTrayTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTrayScreenColorPicker,
            this.tsmiTrayRuler,
            this.tsmiTrayFTPClient,
            this.tsmiTrayHashCheck,
            this.tsmiTrayIndexFolder,
            this.tsmiTrayImageEditor,
            this.tsmiTrayImageEffects,
            this.tsmiTrayMonitorTest,
            this.tsmiTrayDNSChanger,
            this.tsmiTrayQRCode,
            this.tsmiTrayTweetMessage});
            this.tsmiTrayTools.Image = global::ShareX.Properties.Resources.toolbox;
            this.tsmiTrayTools.Name = "tsmiTrayTools";
            this.tsmiTrayTools.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayTools.Text = "Tools";
            // 
            // tsmiTrayScreenColorPicker
            // 
            this.tsmiTrayScreenColorPicker.Image = global::ShareX.Properties.Resources.color;
            this.tsmiTrayScreenColorPicker.Name = "tsmiTrayScreenColorPicker";
            this.tsmiTrayScreenColorPicker.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayScreenColorPicker.Text = "Screen color picker...";
            this.tsmiTrayScreenColorPicker.Click += new System.EventHandler(this.tsmiCursorHelper_Click);
            // 
            // tsmiTrayRuler
            // 
            this.tsmiTrayRuler.Image = global::ShareX.Properties.Resources.ruler_triangle;
            this.tsmiTrayRuler.Name = "tsmiTrayRuler";
            this.tsmiTrayRuler.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayRuler.Text = "Ruler...";
            this.tsmiTrayRuler.Click += new System.EventHandler(this.tsmiRuler_Click);
            // 
            // tsmiTrayFTPClient
            // 
            this.tsmiTrayFTPClient.Image = global::ShareX.Properties.Resources.application_network;
            this.tsmiTrayFTPClient.Name = "tsmiTrayFTPClient";
            this.tsmiTrayFTPClient.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayFTPClient.Text = "FTP client...";
            this.tsmiTrayFTPClient.Click += new System.EventHandler(this.tsmiFTPClient_Click);
            // 
            // tsmiTrayHashCheck
            // 
            this.tsmiTrayHashCheck.Image = global::ShareX.Properties.Resources.application_task;
            this.tsmiTrayHashCheck.Name = "tsmiTrayHashCheck";
            this.tsmiTrayHashCheck.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayHashCheck.Text = "Hash check...";
            this.tsmiTrayHashCheck.Click += new System.EventHandler(this.tsmiHashCheck_Click);
            // 
            // tsmiTrayIndexFolder
            // 
            this.tsmiTrayIndexFolder.Image = global::ShareX.Properties.Resources.folder_tree;
            this.tsmiTrayIndexFolder.Name = "tsmiTrayIndexFolder";
            this.tsmiTrayIndexFolder.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayIndexFolder.Text = "Index folder...";
            this.tsmiTrayIndexFolder.Click += new System.EventHandler(this.tsmiIndexFolder_Click);
            // 
            // tsmiTrayImageEditor
            // 
            this.tsmiTrayImageEditor.Image = global::ShareX.Properties.Resources.image_pencil;
            this.tsmiTrayImageEditor.Name = "tsmiTrayImageEditor";
            this.tsmiTrayImageEditor.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayImageEditor.Text = "Image editor...";
            this.tsmiTrayImageEditor.Click += new System.EventHandler(this.tsmiImageEditor_Click);
            // 
            // tsmiTrayImageEffects
            // 
            this.tsmiTrayImageEffects.Image = global::ShareX.Properties.Resources.image_saturation;
            this.tsmiTrayImageEffects.Name = "tsmiTrayImageEffects";
            this.tsmiTrayImageEffects.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayImageEffects.Text = "Image effects...";
            this.tsmiTrayImageEffects.Click += new System.EventHandler(this.tsmiImageEffects_Click);
            // 
            // tsmiTrayMonitorTest
            // 
            this.tsmiTrayMonitorTest.Image = global::ShareX.Properties.Resources.monitor;
            this.tsmiTrayMonitorTest.Name = "tsmiTrayMonitorTest";
            this.tsmiTrayMonitorTest.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayMonitorTest.Text = "Monitor test...";
            this.tsmiTrayMonitorTest.Click += new System.EventHandler(this.tsmiMonitorTest_Click);
            // 
            // tsmiTrayDNSChanger
            // 
            this.tsmiTrayDNSChanger.Image = global::ShareX.Properties.Resources.network_ip;
            this.tsmiTrayDNSChanger.Name = "tsmiTrayDNSChanger";
            this.tsmiTrayDNSChanger.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayDNSChanger.Text = "DNS changer...";
            this.tsmiTrayDNSChanger.Click += new System.EventHandler(this.tsmiDNSChanger_Click);
            // 
            // tsmiTrayQRCode
            // 
            this.tsmiTrayQRCode.Image = global::ShareX.Properties.Resources.barcode_2d;
            this.tsmiTrayQRCode.Name = "tsmiTrayQRCode";
            this.tsmiTrayQRCode.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayQRCode.Text = "QR code...";
            this.tsmiTrayQRCode.Click += new System.EventHandler(this.tsmiQRCode_Click);
            // 
            // tsmiTrayTweetMessage
            // 
            this.tsmiTrayTweetMessage.Image = global::ShareX.Properties.Resources.Twitter;
            this.tsmiTrayTweetMessage.Name = "tsmiTrayTweetMessage";
            this.tsmiTrayTweetMessage.Size = new System.Drawing.Size(183, 22);
            this.tsmiTrayTweetMessage.Text = "Tweet message...";
            this.tsmiTrayTweetMessage.Click += new System.EventHandler(this.tsmiTweetMessage_Click);
            // 
            // tssTray1
            // 
            this.tssTray1.Name = "tssTray1";
            this.tssTray1.Size = new System.Drawing.Size(185, 6);
            // 
            // tsmiTrayAfterCaptureTasks
            // 
            this.tsmiTrayAfterCaptureTasks.Image = global::ShareX.Properties.Resources.image_export;
            this.tsmiTrayAfterCaptureTasks.Name = "tsmiTrayAfterCaptureTasks";
            this.tsmiTrayAfterCaptureTasks.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayAfterCaptureTasks.Text = "After capture";
            // 
            // tsmiTrayAfterUploadTasks
            // 
            this.tsmiTrayAfterUploadTasks.Image = global::ShareX.Properties.Resources.upload_cloud;
            this.tsmiTrayAfterUploadTasks.Name = "tsmiTrayAfterUploadTasks";
            this.tsmiTrayAfterUploadTasks.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayAfterUploadTasks.Text = "After upload";
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
            this.tsmiTrayDestinationSettings});
            this.tsmiTrayDestinations.Image = global::ShareX.Properties.Resources.drive_globe;
            this.tsmiTrayDestinations.Name = "tsmiTrayDestinations";
            this.tsmiTrayDestinations.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayDestinations.Text = "Destinations";
            this.tsmiTrayDestinations.DropDownOpened += new System.EventHandler(this.tsddbDestinations_DropDownOpened);
            // 
            // tsmiTrayImageUploaders
            // 
            this.tsmiTrayImageUploaders.Image = global::ShareX.Properties.Resources.image;
            this.tsmiTrayImageUploaders.Name = "tsmiTrayImageUploaders";
            this.tsmiTrayImageUploaders.Size = new System.Drawing.Size(187, 22);
            this.tsmiTrayImageUploaders.Text = "Image uploaders";
            // 
            // tsmiTrayTextUploaders
            // 
            this.tsmiTrayTextUploaders.Image = global::ShareX.Properties.Resources.notebook;
            this.tsmiTrayTextUploaders.Name = "tsmiTrayTextUploaders";
            this.tsmiTrayTextUploaders.Size = new System.Drawing.Size(187, 22);
            this.tsmiTrayTextUploaders.Text = "Text uploaders";
            // 
            // tsmiTrayFileUploaders
            // 
            this.tsmiTrayFileUploaders.Image = global::ShareX.Properties.Resources.application_block;
            this.tsmiTrayFileUploaders.Name = "tsmiTrayFileUploaders";
            this.tsmiTrayFileUploaders.Size = new System.Drawing.Size(187, 22);
            this.tsmiTrayFileUploaders.Text = "File uploaders";
            // 
            // tsmiTrayURLShorteners
            // 
            this.tsmiTrayURLShorteners.Image = global::ShareX.Properties.Resources.edit_scale;
            this.tsmiTrayURLShorteners.Name = "tsmiTrayURLShorteners";
            this.tsmiTrayURLShorteners.Size = new System.Drawing.Size(187, 22);
            this.tsmiTrayURLShorteners.Text = "URL shorteners";
            // 
            // tsmiTrayURLSharingServices
            // 
            this.tsmiTrayURLSharingServices.Image = global::ShareX.Properties.Resources.globe_share;
            this.tsmiTrayURLSharingServices.Name = "tsmiTrayURLSharingServices";
            this.tsmiTrayURLSharingServices.Size = new System.Drawing.Size(187, 22);
            this.tsmiTrayURLSharingServices.Text = "URL sharing services";
            // 
            // tssTrayDestinations1
            // 
            this.tssTrayDestinations1.Name = "tssTrayDestinations1";
            this.tssTrayDestinations1.Size = new System.Drawing.Size(184, 6);
            // 
            // tsmiTrayDestinationSettings
            // 
            this.tsmiTrayDestinationSettings.Image = global::ShareX.Properties.Resources.globe_pencil;
            this.tsmiTrayDestinationSettings.Name = "tsmiTrayDestinationSettings";
            this.tsmiTrayDestinationSettings.Size = new System.Drawing.Size(187, 22);
            this.tsmiTrayDestinationSettings.Text = "Destination settings...";
            this.tsmiTrayDestinationSettings.Click += new System.EventHandler(this.tsbDestinationSettings_Click);
            // 
            // tsmiTrayApplicationSettings
            // 
            this.tsmiTrayApplicationSettings.Image = global::ShareX.Properties.Resources.wrench_screwdriver;
            this.tsmiTrayApplicationSettings.Name = "tsmiTrayApplicationSettings";
            this.tsmiTrayApplicationSettings.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayApplicationSettings.Text = "Application settings...";
            this.tsmiTrayApplicationSettings.Click += new System.EventHandler(this.tsbApplicationSettings_Click);
            // 
            // tsmiTrayTaskSettings
            // 
            this.tsmiTrayTaskSettings.Image = global::ShareX.Properties.Resources.gear;
            this.tsmiTrayTaskSettings.Name = "tsmiTrayTaskSettings";
            this.tsmiTrayTaskSettings.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayTaskSettings.Text = "Task settings...";
            this.tsmiTrayTaskSettings.Click += new System.EventHandler(this.tsbTaskSettings_Click);
            // 
            // tsmiTrayHotkeySettings
            // 
            this.tsmiTrayHotkeySettings.Image = global::ShareX.Properties.Resources.keyboard;
            this.tsmiTrayHotkeySettings.Name = "tsmiTrayHotkeySettings";
            this.tsmiTrayHotkeySettings.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayHotkeySettings.Text = "Hotkey settings...";
            this.tsmiTrayHotkeySettings.Click += new System.EventHandler(this.tsbHotkeySettings_Click);
            // 
            // tssTray2
            // 
            this.tssTray2.Name = "tssTray2";
            this.tssTray2.Size = new System.Drawing.Size(185, 6);
            // 
            // tsmiScreenshotsFolder
            // 
            this.tsmiScreenshotsFolder.Image = global::ShareX.Properties.Resources.folder_open_image;
            this.tsmiScreenshotsFolder.Name = "tsmiScreenshotsFolder";
            this.tsmiScreenshotsFolder.Size = new System.Drawing.Size(188, 22);
            this.tsmiScreenshotsFolder.Text = "Screenshots folder...";
            this.tsmiScreenshotsFolder.Click += new System.EventHandler(this.tsbScreenshotsFolder_Click);
            // 
            // tsmiTrayHistory
            // 
            this.tsmiTrayHistory.Image = global::ShareX.Properties.Resources.application_blog;
            this.tsmiTrayHistory.Name = "tsmiTrayHistory";
            this.tsmiTrayHistory.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayHistory.Text = "History...";
            this.tsmiTrayHistory.Click += new System.EventHandler(this.tsbHistory_Click);
            // 
            // tsmiTrayImageHistory
            // 
            this.tsmiTrayImageHistory.Image = global::ShareX.Properties.Resources.application_icon_large;
            this.tsmiTrayImageHistory.Name = "tsmiTrayImageHistory";
            this.tsmiTrayImageHistory.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayImageHistory.Text = "Image history...";
            this.tsmiTrayImageHistory.Click += new System.EventHandler(this.tsbImageHistory_Click);
            // 
            // tsmiTrayDonate
            // 
            this.tsmiTrayDonate.Image = global::ShareX.Properties.Resources.heart;
            this.tsmiTrayDonate.Name = "tsmiTrayDonate";
            this.tsmiTrayDonate.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayDonate.Text = "Donate...";
            this.tsmiTrayDonate.Click += new System.EventHandler(this.tsbDonate_Click);
            // 
            // tsmiTrayAbout
            // 
            this.tsmiTrayAbout.Image = global::ShareX.Properties.Resources.crown;
            this.tsmiTrayAbout.Name = "tsmiTrayAbout";
            this.tsmiTrayAbout.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayAbout.Text = "About...";
            this.tsmiTrayAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // tssTray3
            // 
            this.tssTray3.Name = "tssTray3";
            this.tssTray3.Size = new System.Drawing.Size(185, 6);
            // 
            // tsmiTrayShow
            // 
            this.tsmiTrayShow.Image = global::ShareX.Properties.Resources.tick_button;
            this.tsmiTrayShow.Name = "tsmiTrayShow";
            this.tsmiTrayShow.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayShow.Text = "Show ShareX window";
            this.tsmiTrayShow.Click += new System.EventHandler(this.tsmiTrayShow_Click);
            // 
            // tsmiTrayExit
            // 
            this.tsmiTrayExit.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsmiTrayExit.Name = "tsmiTrayExit";
            this.tsmiTrayExit.Size = new System.Drawing.Size(188, 22);
            this.tsmiTrayExit.Text = "Exit";
            this.tsmiTrayExit.Click += new System.EventHandler(this.tsmiTrayExit_Click);
            // 
            // ssToolStripMenuItem
            // 
            this.ssToolStripMenuItem.Name = "ssToolStripMenuItem";
            this.ssToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ssToolStripMenuItem.Text = "ss";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 407);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.tsMain);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.pBackground.ResumeLayout(false);
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