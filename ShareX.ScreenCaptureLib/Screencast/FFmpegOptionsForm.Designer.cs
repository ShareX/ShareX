namespace ShareX.ScreenCaptureLib
{
    partial class FFmpegOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FFmpegOptionsForm));
            this.lblx264CRF = new System.Windows.Forms.Label();
            this.nudx264CRF = new System.Windows.Forms.NumericUpDown();
            this.ttHelpTip = new System.Windows.Forms.ToolTip(this.components);
            this.nudXvidQscale = new System.Windows.Forms.NumericUpDown();
            this.cbx264Preset = new System.Windows.Forms.ComboBox();
            this.tbVorbis_qscale = new System.Windows.Forms.TrackBar();
            this.tbMP3_qscale = new System.Windows.Forms.TrackBar();
            this.tbAACBitrate = new System.Windows.Forms.TrackBar();
            this.cbGIFStatsMode = new System.Windows.Forms.ComboBox();
            this.cbGIFDither = new System.Windows.Forms.ComboBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.cboVideoCodec = new System.Windows.Forms.ComboBox();
            this.lblx264Preset = new System.Windows.Forms.Label();
            this.lblXvidQscale = new System.Windows.Forms.Label();
            this.gbFFmpegExe = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnFFmpegBrowse = new System.Windows.Forms.Button();
            this.txtFFmpegPath = new System.Windows.Forms.TextBox();
            this.cbOverrideFFmpegPath = new System.Windows.Forms.CheckBox();
            this.gbCommandLinePreview = new System.Windows.Forms.GroupBox();
            this.cbCustomCommands = new System.Windows.Forms.CheckBox();
            this.txtCommandLinePreview = new System.Windows.Forms.TextBox();
            this.gbCommandLineArgs = new System.Windows.Forms.GroupBox();
            this.btnFFmpegHelp = new System.Windows.Forms.Button();
            this.tbUserArgs = new System.Windows.Forms.TextBox();
            this.tcFFmpegVideoCodecs = new System.Windows.Forms.TabControl();
            this.tpX264 = new System.Windows.Forms.TabPage();
            this.tpVpx = new System.Windows.Forms.TabPage();
            this.lblVP8BitrateK = new System.Windows.Forms.Label();
            this.nudVP8Bitrate = new System.Windows.Forms.NumericUpDown();
            this.lblVP8Bitrate = new System.Windows.Forms.Label();
            this.tpXvid = new System.Windows.Forms.TabPage();
            this.tpGIF = new System.Windows.Forms.TabPage();
            this.lblGIFDither = new System.Windows.Forms.Label();
            this.lblGIFStatsMode = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnCopyPreview = new System.Windows.Forms.Button();
            this.tcFFmpegAudioCodecs = new System.Windows.Forms.TabControl();
            this.tpAAC = new System.Windows.Forms.TabPage();
            this.lblAACQuality = new System.Windows.Forms.Label();
            this.tpVorbis = new System.Windows.Forms.TabPage();
            this.lblVorbisQuality = new System.Windows.Forms.Label();
            this.tpMP3 = new System.Windows.Forms.TabPage();
            this.lblMP3Quality = new System.Windows.Forms.Label();
            this.cboVideoSource = new System.Windows.Forms.ComboBox();
            this.lblVideoSource = new System.Windows.Forms.Label();
            this.cboAudioSource = new System.Windows.Forms.ComboBox();
            this.lblAudioSource = new System.Windows.Forms.Label();
            this.cboAudioCodec = new System.Windows.Forms.ComboBox();
            this.lblAudioCodec = new System.Windows.Forms.Label();
            this.gbSource = new System.Windows.Forms.GroupBox();
            this.btnHelperDevicesHelp = new System.Windows.Forms.Button();
            this.lblHelperDevices = new System.Windows.Forms.Label();
            this.btnInstallHelperDevices = new System.Windows.Forms.Button();
            this.btnRefreshSources = new System.Windows.Forms.Button();
            this.gbCodecs = new System.Windows.Forms.GroupBox();
            this.pbAudioCodecWarning = new System.Windows.Forms.PictureBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.eiFFmpeg = new ShareX.HelpersLib.ExportImportControl();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXvidQscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVorbis_qscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMP3_qscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAACBitrate)).BeginInit();
            this.gbFFmpegExe.SuspendLayout();
            this.gbCommandLinePreview.SuspendLayout();
            this.gbCommandLineArgs.SuspendLayout();
            this.tcFFmpegVideoCodecs.SuspendLayout();
            this.tpX264.SuspendLayout();
            this.tpVpx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVP8Bitrate)).BeginInit();
            this.tpXvid.SuspendLayout();
            this.tpGIF.SuspendLayout();
            this.tcFFmpegAudioCodecs.SuspendLayout();
            this.tpAAC.SuspendLayout();
            this.tpVorbis.SuspendLayout();
            this.tpMP3.SuspendLayout();
            this.gbSource.SuspendLayout();
            this.gbCodecs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAudioCodecWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // lblx264CRF
            // 
            resources.ApplyResources(this.lblx264CRF, "lblx264CRF");
            this.lblx264CRF.Name = "lblx264CRF";
            // 
            // nudx264CRF
            // 
            resources.ApplyResources(this.nudx264CRF, "nudx264CRF");
            this.nudx264CRF.Maximum = new decimal(new int[] {
            51,
            0,
            0,
            0});
            this.nudx264CRF.Name = "nudx264CRF";
            this.ttHelpTip.SetToolTip(this.nudx264CRF, resources.GetString("nudx264CRF.ToolTip"));
            this.nudx264CRF.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nudx264CRF.ValueChanged += new System.EventHandler(this.nudx264CRF_ValueChanged);
            // 
            // ttHelpTip
            // 
            this.ttHelpTip.AutomaticDelay = 0;
            this.ttHelpTip.AutoPopDelay = 30000;
            this.ttHelpTip.BackColor = System.Drawing.Color.White;
            this.ttHelpTip.InitialDelay = 500;
            this.ttHelpTip.IsBalloon = true;
            this.ttHelpTip.ReshowDelay = 100;
            this.ttHelpTip.UseAnimation = false;
            this.ttHelpTip.UseFading = false;
            // 
            // nudXvidQscale
            // 
            resources.ApplyResources(this.nudXvidQscale, "nudXvidQscale");
            this.nudXvidQscale.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudXvidQscale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudXvidQscale.Name = "nudXvidQscale";
            this.ttHelpTip.SetToolTip(this.nudXvidQscale, resources.GetString("nudXvidQscale.ToolTip"));
            this.nudXvidQscale.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudXvidQscale.ValueChanged += new System.EventHandler(this.nudQscale_ValueChanged);
            // 
            // cbx264Preset
            // 
            this.cbx264Preset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx264Preset.FormattingEnabled = true;
            resources.ApplyResources(this.cbx264Preset, "cbx264Preset");
            this.cbx264Preset.Name = "cbx264Preset";
            this.ttHelpTip.SetToolTip(this.cbx264Preset, resources.GetString("cbx264Preset.ToolTip"));
            this.cbx264Preset.SelectedIndexChanged += new System.EventHandler(this.cbPreset_SelectedIndexChanged);
            // 
            // tbVorbis_qscale
            // 
            this.tbVorbis_qscale.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.tbVorbis_qscale, "tbVorbis_qscale");
            this.tbVorbis_qscale.LargeChange = 1;
            this.tbVorbis_qscale.Name = "tbVorbis_qscale";
            this.tbVorbis_qscale.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ttHelpTip.SetToolTip(this.tbVorbis_qscale, resources.GetString("tbVorbis_qscale.ToolTip"));
            this.tbVorbis_qscale.Value = 3;
            this.tbVorbis_qscale.ValueChanged += new System.EventHandler(this.tbVorbis_qscale_ValueChanged);
            // 
            // tbMP3_qscale
            // 
            this.tbMP3_qscale.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.tbMP3_qscale, "tbMP3_qscale");
            this.tbMP3_qscale.LargeChange = 1;
            this.tbMP3_qscale.Maximum = 9;
            this.tbMP3_qscale.Name = "tbMP3_qscale";
            this.tbMP3_qscale.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ttHelpTip.SetToolTip(this.tbMP3_qscale, resources.GetString("tbMP3_qscale.ToolTip"));
            this.tbMP3_qscale.Value = 5;
            this.tbMP3_qscale.ValueChanged += new System.EventHandler(this.tbMP3_qscale_ValueChanged);
            // 
            // tbAACBitrate
            // 
            this.tbAACBitrate.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.tbAACBitrate, "tbAACBitrate");
            this.tbAACBitrate.LargeChange = 1;
            this.tbAACBitrate.Maximum = 16;
            this.tbAACBitrate.Minimum = 1;
            this.tbAACBitrate.Name = "tbAACBitrate";
            this.tbAACBitrate.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ttHelpTip.SetToolTip(this.tbAACBitrate, resources.GetString("tbAACBitrate.ToolTip"));
            this.tbAACBitrate.Value = 4;
            this.tbAACBitrate.ValueChanged += new System.EventHandler(this.tbAACBitrate_ValueChanged);
            // 
            // cbGIFStatsMode
            // 
            this.cbGIFStatsMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGIFStatsMode.FormattingEnabled = true;
            resources.ApplyResources(this.cbGIFStatsMode, "cbGIFStatsMode");
            this.cbGIFStatsMode.Name = "cbGIFStatsMode";
            this.ttHelpTip.SetToolTip(this.cbGIFStatsMode, resources.GetString("cbGIFStatsMode.ToolTip"));
            this.cbGIFStatsMode.SelectedIndexChanged += new System.EventHandler(this.cbGIFStatsMode_SelectedIndexChanged);
            // 
            // cbGIFDither
            // 
            this.cbGIFDither.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGIFDither.FormattingEnabled = true;
            resources.ApplyResources(this.cbGIFDither, "cbGIFDither");
            this.cbGIFDither.Name = "cbGIFDither";
            this.ttHelpTip.SetToolTip(this.cbGIFDither, resources.GetString("cbGIFDither.ToolTip"));
            this.cbGIFDither.SelectedIndexChanged += new System.EventHandler(this.cbGIFDither_SelectedIndexChanged);
            // 
            // lblCodec
            // 
            resources.ApplyResources(this.lblCodec, "lblCodec");
            this.lblCodec.Name = "lblCodec";
            // 
            // cboVideoCodec
            // 
            this.cboVideoCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoCodec.FormattingEnabled = true;
            resources.ApplyResources(this.cboVideoCodec, "cboVideoCodec");
            this.cboVideoCodec.Name = "cboVideoCodec";
            this.cboVideoCodec.SelectedIndexChanged += new System.EventHandler(this.cboVideoCodec_SelectedIndexChanged);
            // 
            // lblx264Preset
            // 
            resources.ApplyResources(this.lblx264Preset, "lblx264Preset");
            this.lblx264Preset.Name = "lblx264Preset";
            // 
            // lblXvidQscale
            // 
            resources.ApplyResources(this.lblXvidQscale, "lblXvidQscale");
            this.lblXvidQscale.Name = "lblXvidQscale";
            // 
            // gbFFmpegExe
            // 
            this.gbFFmpegExe.Controls.Add(this.btnDownload);
            this.gbFFmpegExe.Controls.Add(this.btnFFmpegBrowse);
            this.gbFFmpegExe.Controls.Add(this.txtFFmpegPath);
            resources.ApplyResources(this.gbFFmpegExe, "gbFFmpegExe");
            this.gbFFmpegExe.Name = "gbFFmpegExe";
            this.gbFFmpegExe.TabStop = false;
            // 
            // btnDownload
            // 
            resources.ApplyResources(this.btnDownload, "btnDownload");
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnFFmpegBrowse
            // 
            resources.ApplyResources(this.btnFFmpegBrowse, "btnFFmpegBrowse");
            this.btnFFmpegBrowse.Name = "btnFFmpegBrowse";
            this.btnFFmpegBrowse.UseVisualStyleBackColor = true;
            this.btnFFmpegBrowse.Click += new System.EventHandler(this.buttonFFmpegBrowse_Click);
            // 
            // txtFFmpegPath
            // 
            resources.ApplyResources(this.txtFFmpegPath, "txtFFmpegPath");
            this.txtFFmpegPath.Name = "txtFFmpegPath";
            this.txtFFmpegPath.TextChanged += new System.EventHandler(this.txtFFmpegPath_TextChanged);
            // 
            // cbOverrideFFmpegPath
            // 
            resources.ApplyResources(this.cbOverrideFFmpegPath, "cbOverrideFFmpegPath");
            this.cbOverrideFFmpegPath.Name = "cbOverrideFFmpegPath";
            this.cbOverrideFFmpegPath.UseVisualStyleBackColor = true;
            this.cbOverrideFFmpegPath.CheckedChanged += new System.EventHandler(this.cbOverrideFFmpegPath_CheckedChanged);
            // 
            // gbCommandLinePreview
            // 
            this.gbCommandLinePreview.Controls.Add(this.cbCustomCommands);
            this.gbCommandLinePreview.Controls.Add(this.txtCommandLinePreview);
            resources.ApplyResources(this.gbCommandLinePreview, "gbCommandLinePreview");
            this.gbCommandLinePreview.Name = "gbCommandLinePreview";
            this.gbCommandLinePreview.TabStop = false;
            // 
            // cbCustomCommands
            // 
            resources.ApplyResources(this.cbCustomCommands, "cbCustomCommands");
            this.cbCustomCommands.Name = "cbCustomCommands";
            this.cbCustomCommands.UseVisualStyleBackColor = true;
            this.cbCustomCommands.CheckedChanged += new System.EventHandler(this.cbCustomCommands_CheckedChanged);
            // 
            // txtCommandLinePreview
            // 
            resources.ApplyResources(this.txtCommandLinePreview, "txtCommandLinePreview");
            this.txtCommandLinePreview.Name = "txtCommandLinePreview";
            this.txtCommandLinePreview.ReadOnly = true;
            this.txtCommandLinePreview.TextChanged += new System.EventHandler(this.txtCommandLinePreview_TextChanged);
            // 
            // gbCommandLineArgs
            // 
            this.gbCommandLineArgs.Controls.Add(this.btnFFmpegHelp);
            this.gbCommandLineArgs.Controls.Add(this.tbUserArgs);
            resources.ApplyResources(this.gbCommandLineArgs, "gbCommandLineArgs");
            this.gbCommandLineArgs.Name = "gbCommandLineArgs";
            this.gbCommandLineArgs.TabStop = false;
            // 
            // btnFFmpegHelp
            // 
            resources.ApplyResources(this.btnFFmpegHelp, "btnFFmpegHelp");
            this.btnFFmpegHelp.Name = "btnFFmpegHelp";
            this.btnFFmpegHelp.UseVisualStyleBackColor = true;
            this.btnFFmpegHelp.Click += new System.EventHandler(this.buttonFFmpegHelp_Click);
            // 
            // tbUserArgs
            // 
            resources.ApplyResources(this.tbUserArgs, "tbUserArgs");
            this.tbUserArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbUserArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbUserArgs.Name = "tbUserArgs";
            this.tbUserArgs.TextChanged += new System.EventHandler(this.tbUserArgs_TextChanged);
            // 
            // tcFFmpegVideoCodecs
            // 
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpX264);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpVpx);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpXvid);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpGIF);
            resources.ApplyResources(this.tcFFmpegVideoCodecs, "tcFFmpegVideoCodecs");
            this.tcFFmpegVideoCodecs.Name = "tcFFmpegVideoCodecs";
            this.tcFFmpegVideoCodecs.SelectedIndex = 0;
            // 
            // tpX264
            // 
            this.tpX264.Controls.Add(this.nudx264CRF);
            this.tpX264.Controls.Add(this.lblx264CRF);
            this.tpX264.Controls.Add(this.cbx264Preset);
            this.tpX264.Controls.Add(this.lblx264Preset);
            resources.ApplyResources(this.tpX264, "tpX264");
            this.tpX264.Name = "tpX264";
            this.tpX264.UseVisualStyleBackColor = true;
            // 
            // tpVpx
            // 
            this.tpVpx.Controls.Add(this.lblVP8BitrateK);
            this.tpVpx.Controls.Add(this.nudVP8Bitrate);
            this.tpVpx.Controls.Add(this.lblVP8Bitrate);
            resources.ApplyResources(this.tpVpx, "tpVpx");
            this.tpVpx.Name = "tpVpx";
            this.tpVpx.UseVisualStyleBackColor = true;
            // 
            // lblVP8BitrateK
            // 
            resources.ApplyResources(this.lblVP8BitrateK, "lblVP8BitrateK");
            this.lblVP8BitrateK.Name = "lblVP8BitrateK";
            // 
            // nudVP8Bitrate
            // 
            this.nudVP8Bitrate.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudVP8Bitrate, "nudVP8Bitrate");
            this.nudVP8Bitrate.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudVP8Bitrate.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudVP8Bitrate.Name = "nudVP8Bitrate";
            this.nudVP8Bitrate.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudVP8Bitrate.ValueChanged += new System.EventHandler(this.nudVP8Bitrate_ValueChanged);
            // 
            // lblVP8Bitrate
            // 
            resources.ApplyResources(this.lblVP8Bitrate, "lblVP8Bitrate");
            this.lblVP8Bitrate.Name = "lblVP8Bitrate";
            // 
            // tpXvid
            // 
            this.tpXvid.Controls.Add(this.nudXvidQscale);
            this.tpXvid.Controls.Add(this.lblXvidQscale);
            resources.ApplyResources(this.tpXvid, "tpXvid");
            this.tpXvid.Name = "tpXvid";
            this.tpXvid.UseVisualStyleBackColor = true;
            // 
            // tpGIF
            // 
            this.tpGIF.Controls.Add(this.cbGIFDither);
            this.tpGIF.Controls.Add(this.lblGIFDither);
            this.tpGIF.Controls.Add(this.cbGIFStatsMode);
            this.tpGIF.Controls.Add(this.lblGIFStatsMode);
            resources.ApplyResources(this.tpGIF, "tpGIF");
            this.tpGIF.Name = "tpGIF";
            this.tpGIF.UseVisualStyleBackColor = true;
            // 
            // lblGIFDither
            // 
            resources.ApplyResources(this.lblGIFDither, "lblGIFDither");
            this.lblGIFDither.Name = "lblGIFDither";
            // 
            // lblGIFStatsMode
            // 
            resources.ApplyResources(this.lblGIFStatsMode, "lblGIFStatsMode");
            this.lblGIFStatsMode.Name = "lblGIFStatsMode";
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCopyPreview
            // 
            resources.ApplyResources(this.btnCopyPreview, "btnCopyPreview");
            this.btnCopyPreview.Name = "btnCopyPreview";
            this.btnCopyPreview.UseVisualStyleBackColor = true;
            this.btnCopyPreview.Click += new System.EventHandler(this.btnCopyPreview_Click);
            // 
            // tcFFmpegAudioCodecs
            // 
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpAAC);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpVorbis);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpMP3);
            resources.ApplyResources(this.tcFFmpegAudioCodecs, "tcFFmpegAudioCodecs");
            this.tcFFmpegAudioCodecs.Name = "tcFFmpegAudioCodecs";
            this.tcFFmpegAudioCodecs.SelectedIndex = 0;
            // 
            // tpAAC
            // 
            this.tpAAC.Controls.Add(this.tbAACBitrate);
            this.tpAAC.Controls.Add(this.lblAACQuality);
            resources.ApplyResources(this.tpAAC, "tpAAC");
            this.tpAAC.Name = "tpAAC";
            this.tpAAC.UseVisualStyleBackColor = true;
            // 
            // lblAACQuality
            // 
            resources.ApplyResources(this.lblAACQuality, "lblAACQuality");
            this.lblAACQuality.Name = "lblAACQuality";
            // 
            // tpVorbis
            // 
            this.tpVorbis.Controls.Add(this.tbVorbis_qscale);
            this.tpVorbis.Controls.Add(this.lblVorbisQuality);
            resources.ApplyResources(this.tpVorbis, "tpVorbis");
            this.tpVorbis.Name = "tpVorbis";
            this.tpVorbis.UseVisualStyleBackColor = true;
            // 
            // lblVorbisQuality
            // 
            resources.ApplyResources(this.lblVorbisQuality, "lblVorbisQuality");
            this.lblVorbisQuality.Name = "lblVorbisQuality";
            // 
            // tpMP3
            // 
            this.tpMP3.Controls.Add(this.tbMP3_qscale);
            this.tpMP3.Controls.Add(this.lblMP3Quality);
            resources.ApplyResources(this.tpMP3, "tpMP3");
            this.tpMP3.Name = "tpMP3";
            this.tpMP3.UseVisualStyleBackColor = true;
            // 
            // lblMP3Quality
            // 
            resources.ApplyResources(this.lblMP3Quality, "lblMP3Quality");
            this.lblMP3Quality.Name = "lblMP3Quality";
            // 
            // cboVideoSource
            // 
            this.cboVideoSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoSource.FormattingEnabled = true;
            resources.ApplyResources(this.cboVideoSource, "cboVideoSource");
            this.cboVideoSource.Name = "cboVideoSource";
            this.cboVideoSource.SelectedIndexChanged += new System.EventHandler(this.cboVideoSource_SelectedIndexChanged);
            // 
            // lblVideoSource
            // 
            resources.ApplyResources(this.lblVideoSource, "lblVideoSource");
            this.lblVideoSource.Name = "lblVideoSource";
            // 
            // cboAudioSource
            // 
            this.cboAudioSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioSource.FormattingEnabled = true;
            resources.ApplyResources(this.cboAudioSource, "cboAudioSource");
            this.cboAudioSource.Name = "cboAudioSource";
            this.cboAudioSource.SelectedIndexChanged += new System.EventHandler(this.cboAudioSource_SelectedIndexChanged);
            // 
            // lblAudioSource
            // 
            resources.ApplyResources(this.lblAudioSource, "lblAudioSource");
            this.lblAudioSource.Name = "lblAudioSource";
            // 
            // cboAudioCodec
            // 
            this.cboAudioCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioCodec.FormattingEnabled = true;
            resources.ApplyResources(this.cboAudioCodec, "cboAudioCodec");
            this.cboAudioCodec.Name = "cboAudioCodec";
            this.cboAudioCodec.SelectedIndexChanged += new System.EventHandler(this.cboAudioCodec_SelectedIndexChanged);
            // 
            // lblAudioCodec
            // 
            resources.ApplyResources(this.lblAudioCodec, "lblAudioCodec");
            this.lblAudioCodec.Name = "lblAudioCodec";
            // 
            // gbSource
            // 
            this.gbSource.Controls.Add(this.btnHelperDevicesHelp);
            this.gbSource.Controls.Add(this.lblHelperDevices);
            this.gbSource.Controls.Add(this.btnInstallHelperDevices);
            this.gbSource.Controls.Add(this.cboVideoSource);
            this.gbSource.Controls.Add(this.lblVideoSource);
            this.gbSource.Controls.Add(this.cboAudioSource);
            this.gbSource.Controls.Add(this.lblAudioSource);
            this.gbSource.Controls.Add(this.btnRefreshSources);
            resources.ApplyResources(this.gbSource, "gbSource");
            this.gbSource.Name = "gbSource";
            this.gbSource.TabStop = false;
            // 
            // btnHelperDevicesHelp
            // 
            resources.ApplyResources(this.btnHelperDevicesHelp, "btnHelperDevicesHelp");
            this.btnHelperDevicesHelp.Name = "btnHelperDevicesHelp";
            this.btnHelperDevicesHelp.UseVisualStyleBackColor = true;
            this.btnHelperDevicesHelp.Click += new System.EventHandler(this.btnHelperDevicesHelp_Click);
            // 
            // lblHelperDevices
            // 
            resources.ApplyResources(this.lblHelperDevices, "lblHelperDevices");
            this.lblHelperDevices.Name = "lblHelperDevices";
            // 
            // btnInstallHelperDevices
            // 
            resources.ApplyResources(this.btnInstallHelperDevices, "btnInstallHelperDevices");
            this.btnInstallHelperDevices.Name = "btnInstallHelperDevices";
            this.btnInstallHelperDevices.UseVisualStyleBackColor = true;
            this.btnInstallHelperDevices.Click += new System.EventHandler(this.btnInstallHelperDevices_Click);
            // 
            // btnRefreshSources
            // 
            resources.ApplyResources(this.btnRefreshSources, "btnRefreshSources");
            this.btnRefreshSources.Name = "btnRefreshSources";
            this.btnRefreshSources.UseVisualStyleBackColor = true;
            this.btnRefreshSources.Click += new System.EventHandler(this.btnRefreshSources_Click);
            // 
            // gbCodecs
            // 
            this.gbCodecs.Controls.Add(this.pbAudioCodecWarning);
            this.gbCodecs.Controls.Add(this.cboAudioCodec);
            this.gbCodecs.Controls.Add(this.lblAudioCodec);
            this.gbCodecs.Controls.Add(this.cboVideoCodec);
            this.gbCodecs.Controls.Add(this.lblCodec);
            this.gbCodecs.Controls.Add(this.tcFFmpegAudioCodecs);
            this.gbCodecs.Controls.Add(this.tcFFmpegVideoCodecs);
            resources.ApplyResources(this.gbCodecs, "gbCodecs");
            this.gbCodecs.Name = "gbCodecs";
            this.gbCodecs.TabStop = false;
            // 
            // pbAudioCodecWarning
            // 
            this.pbAudioCodecWarning.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.exclamation_button;
            resources.ApplyResources(this.pbAudioCodecWarning, "pbAudioCodecWarning");
            this.pbAudioCodecWarning.Name = "pbAudioCodecWarning";
            this.pbAudioCodecWarning.TabStop = false;
            this.ttHelpTip.SetToolTip(this.pbAudioCodecWarning, resources.GetString("pbAudioCodecWarning.ToolTip"));
            // 
            // btnHelp
            // 
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // eiFFmpeg
            // 
            resources.ApplyResources(this.eiFFmpeg, "eiFFmpeg");
            this.eiFFmpeg.Name = "eiFFmpeg";
            this.eiFFmpeg.ObjectType = null;
            this.eiFFmpeg.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.eiFFmpeg_ExportRequested);
            this.eiFFmpeg.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.eiFFmpeg_ImportRequested);
            // 
            // FFmpegOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cbOverrideFFmpegPath);
            this.Controls.Add(this.eiFFmpeg);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.gbCodecs);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.btnCopyPreview);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.gbCommandLinePreview);
            this.Controls.Add(this.gbFFmpegExe);
            this.Controls.Add(this.gbCommandLineArgs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FFmpegOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXvidQscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVorbis_qscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMP3_qscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAACBitrate)).EndInit();
            this.gbFFmpegExe.ResumeLayout(false);
            this.gbFFmpegExe.PerformLayout();
            this.gbCommandLinePreview.ResumeLayout(false);
            this.gbCommandLinePreview.PerformLayout();
            this.gbCommandLineArgs.ResumeLayout(false);
            this.gbCommandLineArgs.PerformLayout();
            this.tcFFmpegVideoCodecs.ResumeLayout(false);
            this.tpX264.ResumeLayout(false);
            this.tpX264.PerformLayout();
            this.tpVpx.ResumeLayout(false);
            this.tpVpx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVP8Bitrate)).EndInit();
            this.tpXvid.ResumeLayout(false);
            this.tpXvid.PerformLayout();
            this.tpGIF.ResumeLayout(false);
            this.tpGIF.PerformLayout();
            this.tcFFmpegAudioCodecs.ResumeLayout(false);
            this.tpAAC.ResumeLayout(false);
            this.tpAAC.PerformLayout();
            this.tpVorbis.ResumeLayout(false);
            this.tpVorbis.PerformLayout();
            this.tpMP3.ResumeLayout(false);
            this.tpMP3.PerformLayout();
            this.gbSource.ResumeLayout(false);
            this.gbSource.PerformLayout();
            this.gbCodecs.ResumeLayout(false);
            this.gbCodecs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAudioCodecWarning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblx264CRF;
        private System.Windows.Forms.NumericUpDown nudx264CRF;
        private System.Windows.Forms.ToolTip ttHelpTip;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox cboVideoCodec;
        private System.Windows.Forms.ComboBox cbx264Preset;
        private System.Windows.Forms.Label lblx264Preset;
        private System.Windows.Forms.NumericUpDown nudXvidQscale;
        private System.Windows.Forms.Label lblXvidQscale;
        private System.Windows.Forms.GroupBox gbFFmpegExe;
        private System.Windows.Forms.Button btnFFmpegBrowse;
        private System.Windows.Forms.TextBox txtFFmpegPath;
        private System.Windows.Forms.GroupBox gbCommandLinePreview;
        private System.Windows.Forms.TextBox txtCommandLinePreview;
        private System.Windows.Forms.GroupBox gbCommandLineArgs;
        private System.Windows.Forms.Button btnFFmpegHelp;
        private System.Windows.Forms.TextBox tbUserArgs;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TabControl tcFFmpegVideoCodecs;
        private System.Windows.Forms.TabPage tpX264;
        private System.Windows.Forms.TabPage tpVpx;
        private System.Windows.Forms.TabPage tpXvid;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnCopyPreview;
        private System.Windows.Forms.TabControl tcFFmpegAudioCodecs;
        private System.Windows.Forms.TabPage tpVorbis;
        private System.Windows.Forms.TabPage tpMP3;
        private System.Windows.Forms.ComboBox cboVideoSource;
        private System.Windows.Forms.Label lblVideoSource;
        private System.Windows.Forms.ComboBox cboAudioSource;
        private System.Windows.Forms.Label lblAudioSource;
        private System.Windows.Forms.ComboBox cboAudioCodec;
        private System.Windows.Forms.Label lblAudioCodec;
        private System.Windows.Forms.GroupBox gbSource;
        private System.Windows.Forms.GroupBox gbCodecs;
        private System.Windows.Forms.Button btnRefreshSources;
        private System.Windows.Forms.TrackBar tbVorbis_qscale;
        private System.Windows.Forms.Label lblVorbisQuality;
        private System.Windows.Forms.TrackBar tbMP3_qscale;
        private System.Windows.Forms.Label lblMP3Quality;
        private System.Windows.Forms.TabPage tpAAC;
        private System.Windows.Forms.TrackBar tbAACBitrate;
        private System.Windows.Forms.Label lblAACQuality;
        private System.Windows.Forms.CheckBox cbCustomCommands;
        private System.Windows.Forms.Button btnHelp;
        private ShareX.HelpersLib.ExportImportControl eiFFmpeg;
        private System.Windows.Forms.Label lblVP8BitrateK;
        private System.Windows.Forms.NumericUpDown nudVP8Bitrate;
        private System.Windows.Forms.Label lblVP8Bitrate;
        private System.Windows.Forms.TabPage tpGIF;
        private System.Windows.Forms.ComboBox cbGIFDither;
        private System.Windows.Forms.Label lblGIFDither;
        private System.Windows.Forms.ComboBox cbGIFStatsMode;
        private System.Windows.Forms.Label lblGIFStatsMode;
        private System.Windows.Forms.Button btnHelperDevicesHelp;
        private System.Windows.Forms.Label lblHelperDevices;
        private System.Windows.Forms.Button btnInstallHelperDevices;
        private System.Windows.Forms.CheckBox cbOverrideFFmpegPath;
        private System.Windows.Forms.PictureBox pbAudioCodecWarning;
    }
}