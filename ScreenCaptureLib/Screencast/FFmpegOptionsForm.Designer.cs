namespace ScreenCaptureLib
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
            this.lblExt = new System.Windows.Forms.Label();
            this.lblX264CRF = new System.Windows.Forms.Label();
            this.nudx264CRF = new System.Windows.Forms.NumericUpDown();
            this.ttHelpTip = new System.Windows.Forms.ToolTip(this.components);
            this.nudQscale = new System.Windows.Forms.NumericUpDown();
            this.nudVPxCRF = new System.Windows.Forms.NumericUpDown();
            this.cbPreset = new System.Windows.Forms.ComboBox();
            this.tbVorbis_qscale = new System.Windows.Forms.TrackBar();
            this.tbMP3_qscale = new System.Windows.Forms.TrackBar();
            this.tbAACBitrate = new System.Windows.Forms.TrackBar();
            this.cboExtension = new System.Windows.Forms.ComboBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.cboVideoCodec = new System.Windows.Forms.ComboBox();
            this.lblPreset = new System.Windows.Forms.Label();
            this.lblQscale = new System.Windows.Forms.Label();
            this.gbFFmpegExe = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnFFmpegBrowse = new System.Windows.Forms.Button();
            this.txtFFmpegPath = new System.Windows.Forms.TextBox();
            this.gbCommandLinePreview = new System.Windows.Forms.GroupBox();
            this.cbCustomCommands = new System.Windows.Forms.CheckBox();
            this.txtCommandLinePreview = new System.Windows.Forms.TextBox();
            this.gbCommandLineArgs = new System.Windows.Forms.GroupBox();
            this.btnFFmpegHelp = new System.Windows.Forms.Button();
            this.tbUserArgs = new System.Windows.Forms.TextBox();
            this.tcFFmpegVideoCodecs = new System.Windows.Forms.TabControl();
            this.tpX264 = new System.Windows.Forms.TabPage();
            this.tpVpx = new System.Windows.Forms.TabPage();
            this.lblVpxCRF = new System.Windows.Forms.Label();
            this.tpXvid = new System.Windows.Forms.TabPage();
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
            this.gbCodecs = new System.Windows.Forms.GroupBox();
            this.gbContainer = new System.Windows.Forms.GroupBox();
            this.btnRefreshSources = new System.Windows.Forms.Button();
            this.cbShowError = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVPxCRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVorbis_qscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMP3_qscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAACBitrate)).BeginInit();
            this.gbFFmpegExe.SuspendLayout();
            this.gbCommandLinePreview.SuspendLayout();
            this.gbCommandLineArgs.SuspendLayout();
            this.tcFFmpegVideoCodecs.SuspendLayout();
            this.tpX264.SuspendLayout();
            this.tpVpx.SuspendLayout();
            this.tpXvid.SuspendLayout();
            this.tcFFmpegAudioCodecs.SuspendLayout();
            this.tpAAC.SuspendLayout();
            this.tpVorbis.SuspendLayout();
            this.tpMP3.SuspendLayout();
            this.gbSource.SuspendLayout();
            this.gbCodecs.SuspendLayout();
            this.gbContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(8, 26);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(56, 13);
            this.lblExt.TabIndex = 0;
            this.lblExt.Text = "Extension:";
            // 
            // lblX264CRF
            // 
            this.lblX264CRF.AutoSize = true;
            this.lblX264CRF.Location = new System.Drawing.Point(16, 16);
            this.lblX264CRF.Name = "lblX264CRF";
            this.lblX264CRF.Size = new System.Drawing.Size(31, 13);
            this.lblX264CRF.TabIndex = 0;
            this.lblX264CRF.Text = "CRF:";
            // 
            // nudx264CRF
            // 
            this.nudx264CRF.Location = new System.Drawing.Point(56, 12);
            this.nudx264CRF.Maximum = new decimal(new int[] {
            51,
            0,
            0,
            0});
            this.nudx264CRF.Name = "nudx264CRF";
            this.nudx264CRF.Size = new System.Drawing.Size(48, 20);
            this.nudx264CRF.TabIndex = 1;
            this.nudx264CRF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // nudQscale
            // 
            this.nudQscale.Location = new System.Drawing.Point(104, 12);
            this.nudQscale.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudQscale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudQscale.Name = "nudQscale";
            this.nudQscale.Size = new System.Drawing.Size(48, 20);
            this.nudQscale.TabIndex = 1;
            this.nudQscale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ttHelpTip.SetToolTip(this.nudQscale, "1 being highest quality/largest filesize and 31 being the lowest quality/smallest" +
        " filesize.");
            this.nudQscale.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudQscale.ValueChanged += new System.EventHandler(this.nudQscale_ValueChanged);
            // 
            // nudVPxCRF
            // 
            this.nudVPxCRF.Location = new System.Drawing.Point(56, 12);
            this.nudVPxCRF.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.nudVPxCRF.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudVPxCRF.Name = "nudVPxCRF";
            this.nudVPxCRF.Size = new System.Drawing.Size(48, 20);
            this.nudVPxCRF.TabIndex = 1;
            this.nudVPxCRF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ttHelpTip.SetToolTip(this.nudVPxCRF, "CRF value can be from 4–63, and 10 is a good starting point. Lower values mean be" +
        "tter quality.");
            this.nudVPxCRF.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nudVPxCRF.ValueChanged += new System.EventHandler(this.nudVPxCRF_ValueChanged);
            // 
            // cbPreset
            // 
            this.cbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPreset.FormattingEnabled = true;
            this.cbPreset.Location = new System.Drawing.Point(168, 12);
            this.cbPreset.Name = "cbPreset";
            this.cbPreset.Size = new System.Drawing.Size(121, 21);
            this.cbPreset.TabIndex = 3;
            this.ttHelpTip.SetToolTip(this.cbPreset, resources.GetString("cbPreset.ToolTip"));
            this.cbPreset.SelectedIndexChanged += new System.EventHandler(this.cbPreset_SelectedIndexChanged);
            // 
            // tbVorbis_qscale
            // 
            this.tbVorbis_qscale.BackColor = System.Drawing.Color.White;
            this.tbVorbis_qscale.Dock = System.Windows.Forms.DockStyle.Right;
            this.tbVorbis_qscale.LargeChange = 1;
            this.tbVorbis_qscale.Location = new System.Drawing.Point(88, 3);
            this.tbVorbis_qscale.Name = "tbVorbis_qscale";
            this.tbVorbis_qscale.Size = new System.Drawing.Size(221, 36);
            this.tbVorbis_qscale.TabIndex = 1;
            this.tbVorbis_qscale.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ttHelpTip.SetToolTip(this.tbVorbis_qscale, "Range is 0–10, where 10 is highest quality. 3–6 is a good range to try. Default i" +
        "s 3.");
            this.tbVorbis_qscale.Value = 3;
            this.tbVorbis_qscale.Scroll += new System.EventHandler(this.tbVorbis_qscale_Scroll);
            // 
            // tbMP3_qscale
            // 
            this.tbMP3_qscale.BackColor = System.Drawing.Color.White;
            this.tbMP3_qscale.Dock = System.Windows.Forms.DockStyle.Right;
            this.tbMP3_qscale.LargeChange = 1;
            this.tbMP3_qscale.Location = new System.Drawing.Point(88, 3);
            this.tbMP3_qscale.Maximum = 9;
            this.tbMP3_qscale.Name = "tbMP3_qscale";
            this.tbMP3_qscale.Size = new System.Drawing.Size(221, 36);
            this.tbMP3_qscale.TabIndex = 1;
            this.tbMP3_qscale.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ttHelpTip.SetToolTip(this.tbMP3_qscale, "Range is 0-9 where a lower value is a higher quality. 0-3 will normally produce t" +
        "ransparent results, 4 (default) should be close to perceptual transparency, and " +
        "6 produces an \"acceptable\" quality.");
            this.tbMP3_qscale.Value = 5;
            this.tbMP3_qscale.Scroll += new System.EventHandler(this.tbMP3_qscale_Scroll);
            // 
            // tbAACBitrate
            // 
            this.tbAACBitrate.BackColor = System.Drawing.Color.White;
            this.tbAACBitrate.Dock = System.Windows.Forms.DockStyle.Right;
            this.tbAACBitrate.LargeChange = 1;
            this.tbAACBitrate.Location = new System.Drawing.Point(88, 3);
            this.tbAACBitrate.Maximum = 16;
            this.tbAACBitrate.Minimum = 1;
            this.tbAACBitrate.Name = "tbAACBitrate";
            this.tbAACBitrate.Size = new System.Drawing.Size(221, 36);
            this.tbAACBitrate.TabIndex = 1;
            this.tbAACBitrate.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ttHelpTip.SetToolTip(this.tbAACBitrate, "Default is 128k.");
            this.tbAACBitrate.Value = 4;
            this.tbAACBitrate.Scroll += new System.EventHandler(this.tbAACBitrate_Scroll);
            // 
            // cboExtension
            // 
            this.cboExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExtension.FormattingEnabled = true;
            this.cboExtension.Location = new System.Drawing.Point(72, 22);
            this.cboExtension.Name = "cboExtension";
            this.cboExtension.Size = new System.Drawing.Size(88, 21);
            this.cboExtension.TabIndex = 1;
            this.cboExtension.SelectedIndexChanged += new System.EventHandler(this.cbExtension_SelectedIndexChanged);
            // 
            // lblCodec
            // 
            this.lblCodec.AutoSize = true;
            this.lblCodec.Location = new System.Drawing.Point(8, 26);
            this.lblCodec.Name = "lblCodec";
            this.lblCodec.Size = new System.Drawing.Size(70, 13);
            this.lblCodec.TabIndex = 0;
            this.lblCodec.Text = "Video codec:";
            // 
            // cboVideoCodec
            // 
            this.cboVideoCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoCodec.FormattingEnabled = true;
            this.cboVideoCodec.Location = new System.Drawing.Point(88, 22);
            this.cboVideoCodec.Name = "cboVideoCodec";
            this.cboVideoCodec.Size = new System.Drawing.Size(88, 21);
            this.cboVideoCodec.TabIndex = 1;
            this.cboVideoCodec.SelectedIndexChanged += new System.EventHandler(this.cboVideoCodec_SelectedIndexChanged);
            // 
            // lblPreset
            // 
            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(120, 16);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(40, 13);
            this.lblPreset.TabIndex = 2;
            this.lblPreset.Text = "Preset:";
            // 
            // lblQscale
            // 
            this.lblQscale.AutoSize = true;
            this.lblQscale.Location = new System.Drawing.Point(16, 16);
            this.lblQscale.Name = "lblQscale";
            this.lblQscale.Size = new System.Drawing.Size(83, 13);
            this.lblQscale.TabIndex = 0;
            this.lblQscale.Text = "Variable bit rate:";
            // 
            // gbFFmpegExe
            // 
            this.gbFFmpegExe.Controls.Add(this.btnDownload);
            this.gbFFmpegExe.Controls.Add(this.btnFFmpegBrowse);
            this.gbFFmpegExe.Controls.Add(this.txtFFmpegPath);
            this.gbFFmpegExe.Location = new System.Drawing.Point(8, 8);
            this.gbFFmpegExe.Name = "gbFFmpegExe";
            this.gbFFmpegExe.Size = new System.Drawing.Size(648, 56);
            this.gbFFmpegExe.TabIndex = 6;
            this.gbFFmpegExe.TabStop = false;
            this.gbFFmpegExe.Text = "ffmpeg.exe";
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(568, 22);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(72, 24);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnFFmpegBrowse
            // 
            this.btnFFmpegBrowse.Location = new System.Drawing.Point(520, 22);
            this.btnFFmpegBrowse.Name = "btnFFmpegBrowse";
            this.btnFFmpegBrowse.Size = new System.Drawing.Size(40, 24);
            this.btnFFmpegBrowse.TabIndex = 1;
            this.btnFFmpegBrowse.Text = "...";
            this.btnFFmpegBrowse.UseVisualStyleBackColor = true;
            this.btnFFmpegBrowse.Click += new System.EventHandler(this.buttonFFmpegBrowse_Click);
            // 
            // txtFFmpegPath
            // 
            this.txtFFmpegPath.Location = new System.Drawing.Point(8, 24);
            this.txtFFmpegPath.Name = "txtFFmpegPath";
            this.txtFFmpegPath.Size = new System.Drawing.Size(504, 20);
            this.txtFFmpegPath.TabIndex = 0;
            this.txtFFmpegPath.TextChanged += new System.EventHandler(this.tbFFmpegPath_TextChanged);
            // 
            // gbCommandLinePreview
            // 
            this.gbCommandLinePreview.Controls.Add(this.cbCustomCommands);
            this.gbCommandLinePreview.Controls.Add(this.txtCommandLinePreview);
            this.gbCommandLinePreview.Location = new System.Drawing.Point(8, 309);
            this.gbCommandLinePreview.Name = "gbCommandLinePreview";
            this.gbCommandLinePreview.Padding = new System.Windows.Forms.Padding(8);
            this.gbCommandLinePreview.Size = new System.Drawing.Size(648, 96);
            this.gbCommandLinePreview.TabIndex = 10;
            this.gbCommandLinePreview.TabStop = false;
            this.gbCommandLinePreview.Text = "Command line preview";
            // 
            // cbCustomCommands
            // 
            this.cbCustomCommands.AutoSize = true;
            this.cbCustomCommands.Location = new System.Drawing.Point(280, 0);
            this.cbCustomCommands.Name = "cbCustomCommands";
            this.cbCustomCommands.Size = new System.Drawing.Size(136, 17);
            this.cbCustomCommands.TabIndex = 1;
            this.cbCustomCommands.Text = "Use custom commands";
            this.cbCustomCommands.UseVisualStyleBackColor = true;
            this.cbCustomCommands.CheckedChanged += new System.EventHandler(this.cbCustomCommands_CheckedChanged);
            // 
            // txtCommandLinePreview
            // 
            this.txtCommandLinePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommandLinePreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtCommandLinePreview.Location = new System.Drawing.Point(8, 21);
            this.txtCommandLinePreview.Multiline = true;
            this.txtCommandLinePreview.Name = "txtCommandLinePreview";
            this.txtCommandLinePreview.ReadOnly = true;
            this.txtCommandLinePreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommandLinePreview.Size = new System.Drawing.Size(632, 67);
            this.txtCommandLinePreview.TabIndex = 0;
            this.txtCommandLinePreview.TextChanged += new System.EventHandler(this.txtCommandLinePreview_TextChanged);
            // 
            // gbCommandLineArgs
            // 
            this.gbCommandLineArgs.Controls.Add(this.btnFFmpegHelp);
            this.gbCommandLineArgs.Controls.Add(this.tbUserArgs);
            this.gbCommandLineArgs.Location = new System.Drawing.Point(8, 240);
            this.gbCommandLineArgs.Name = "gbCommandLineArgs";
            this.gbCommandLineArgs.Size = new System.Drawing.Size(648, 56);
            this.gbCommandLineArgs.TabIndex = 7;
            this.gbCommandLineArgs.TabStop = false;
            this.gbCommandLineArgs.Text = "Additional command line arguments";
            // 
            // btnFFmpegHelp
            // 
            this.btnFFmpegHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFFmpegHelp.Location = new System.Drawing.Point(600, 22);
            this.btnFFmpegHelp.Name = "btnFFmpegHelp";
            this.btnFFmpegHelp.Size = new System.Drawing.Size(40, 24);
            this.btnFFmpegHelp.TabIndex = 1;
            this.btnFFmpegHelp.Text = "?";
            this.btnFFmpegHelp.UseVisualStyleBackColor = true;
            this.btnFFmpegHelp.Click += new System.EventHandler(this.buttonFFmpegHelp_Click);
            // 
            // tbUserArgs
            // 
            this.tbUserArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbUserArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbUserArgs.Location = new System.Drawing.Point(8, 24);
            this.tbUserArgs.Name = "tbUserArgs";
            this.tbUserArgs.Size = new System.Drawing.Size(584, 20);
            this.tbUserArgs.TabIndex = 0;
            this.tbUserArgs.TextChanged += new System.EventHandler(this.tbUserArgs_TextChanged);
            // 
            // tcFFmpegVideoCodecs
            // 
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpX264);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpVpx);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpXvid);
            this.tcFFmpegVideoCodecs.Location = new System.Drawing.Point(8, 164);
            this.tcFFmpegVideoCodecs.Name = "tcFFmpegVideoCodecs";
            this.tcFFmpegVideoCodecs.SelectedIndex = 0;
            this.tcFFmpegVideoCodecs.Size = new System.Drawing.Size(320, 68);
            this.tcFFmpegVideoCodecs.TabIndex = 4;
            // 
            // tpX264
            // 
            this.tpX264.Controls.Add(this.nudx264CRF);
            this.tpX264.Controls.Add(this.lblX264CRF);
            this.tpX264.Controls.Add(this.cbPreset);
            this.tpX264.Controls.Add(this.lblPreset);
            this.tpX264.Location = new System.Drawing.Point(4, 22);
            this.tpX264.Name = "tpX264";
            this.tpX264.Padding = new System.Windows.Forms.Padding(3);
            this.tpX264.Size = new System.Drawing.Size(312, 42);
            this.tpX264.TabIndex = 1;
            this.tpX264.Text = "x264";
            this.tpX264.UseVisualStyleBackColor = true;
            // 
            // tpVpx
            // 
            this.tpVpx.Controls.Add(this.nudVPxCRF);
            this.tpVpx.Controls.Add(this.lblVpxCRF);
            this.tpVpx.Location = new System.Drawing.Point(4, 22);
            this.tpVpx.Name = "tpVpx";
            this.tpVpx.Size = new System.Drawing.Size(312, 42);
            this.tpVpx.TabIndex = 2;
            this.tpVpx.Text = "VP8";
            this.tpVpx.UseVisualStyleBackColor = true;
            // 
            // lblVpxCRF
            // 
            this.lblVpxCRF.AutoSize = true;
            this.lblVpxCRF.Location = new System.Drawing.Point(16, 16);
            this.lblVpxCRF.Name = "lblVpxCRF";
            this.lblVpxCRF.Size = new System.Drawing.Size(31, 13);
            this.lblVpxCRF.TabIndex = 0;
            this.lblVpxCRF.Text = "CRF:";
            // 
            // tpXvid
            // 
            this.tpXvid.Controls.Add(this.nudQscale);
            this.tpXvid.Controls.Add(this.lblQscale);
            this.tpXvid.Location = new System.Drawing.Point(4, 22);
            this.tpXvid.Name = "tpXvid";
            this.tpXvid.Size = new System.Drawing.Size(312, 42);
            this.tpXvid.TabIndex = 3;
            this.tpXvid.Text = "XviD";
            this.tpXvid.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(136, 304);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(88, 23);
            this.btnTest.TabIndex = 8;
            this.btnTest.Text = "Test with CMD";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCopyPreview
            // 
            this.btnCopyPreview.Location = new System.Drawing.Point(227, 304);
            this.btnCopyPreview.Name = "btnCopyPreview";
            this.btnCopyPreview.Size = new System.Drawing.Size(53, 23);
            this.btnCopyPreview.TabIndex = 9;
            this.btnCopyPreview.Text = "Copy";
            this.btnCopyPreview.UseVisualStyleBackColor = true;
            this.btnCopyPreview.Click += new System.EventHandler(this.btnCopyPreview_Click);
            // 
            // tcFFmpegAudioCodecs
            // 
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpAAC);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpVorbis);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpMP3);
            this.tcFFmpegAudioCodecs.Location = new System.Drawing.Point(336, 164);
            this.tcFFmpegAudioCodecs.Name = "tcFFmpegAudioCodecs";
            this.tcFFmpegAudioCodecs.SelectedIndex = 0;
            this.tcFFmpegAudioCodecs.Size = new System.Drawing.Size(320, 68);
            this.tcFFmpegAudioCodecs.TabIndex = 5;
            // 
            // tpAAC
            // 
            this.tpAAC.Controls.Add(this.tbAACBitrate);
            this.tpAAC.Controls.Add(this.lblAACQuality);
            this.tpAAC.Location = new System.Drawing.Point(4, 22);
            this.tpAAC.Name = "tpAAC";
            this.tpAAC.Padding = new System.Windows.Forms.Padding(3);
            this.tpAAC.Size = new System.Drawing.Size(312, 42);
            this.tpAAC.TabIndex = 3;
            this.tpAAC.Text = "AAC";
            this.tpAAC.UseVisualStyleBackColor = true;
            // 
            // lblAACQuality
            // 
            this.lblAACQuality.AutoSize = true;
            this.lblAACQuality.Location = new System.Drawing.Point(16, 16);
            this.lblAACQuality.Name = "lblAACQuality";
            this.lblAACQuality.Size = new System.Drawing.Size(40, 13);
            this.lblAACQuality.TabIndex = 0;
            this.lblAACQuality.Text = "Bitrate:";
            // 
            // tpVorbis
            // 
            this.tpVorbis.Controls.Add(this.tbVorbis_qscale);
            this.tpVorbis.Controls.Add(this.lblVorbisQuality);
            this.tpVorbis.Location = new System.Drawing.Point(4, 22);
            this.tpVorbis.Name = "tpVorbis";
            this.tpVorbis.Padding = new System.Windows.Forms.Padding(3);
            this.tpVorbis.Size = new System.Drawing.Size(312, 42);
            this.tpVorbis.TabIndex = 0;
            this.tpVorbis.Text = "Vorbis";
            this.tpVorbis.UseVisualStyleBackColor = true;
            // 
            // lblVorbisQuality
            // 
            this.lblVorbisQuality.AutoSize = true;
            this.lblVorbisQuality.Location = new System.Drawing.Point(16, 16);
            this.lblVorbisQuality.Name = "lblVorbisQuality";
            this.lblVorbisQuality.Size = new System.Drawing.Size(42, 13);
            this.lblVorbisQuality.TabIndex = 0;
            this.lblVorbisQuality.Text = "Quality:";
            // 
            // tpMP3
            // 
            this.tpMP3.Controls.Add(this.tbMP3_qscale);
            this.tpMP3.Controls.Add(this.lblMP3Quality);
            this.tpMP3.Location = new System.Drawing.Point(4, 22);
            this.tpMP3.Name = "tpMP3";
            this.tpMP3.Padding = new System.Windows.Forms.Padding(3);
            this.tpMP3.Size = new System.Drawing.Size(312, 42);
            this.tpMP3.TabIndex = 2;
            this.tpMP3.Text = "MP3";
            this.tpMP3.UseVisualStyleBackColor = true;
            // 
            // lblMP3Quality
            // 
            this.lblMP3Quality.AutoSize = true;
            this.lblMP3Quality.Location = new System.Drawing.Point(16, 16);
            this.lblMP3Quality.Name = "lblMP3Quality";
            this.lblMP3Quality.Size = new System.Drawing.Size(42, 13);
            this.lblMP3Quality.TabIndex = 0;
            this.lblMP3Quality.Text = "Quality:";
            // 
            // cboVideoSource
            // 
            this.cboVideoSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoSource.FormattingEnabled = true;
            this.cboVideoSource.Location = new System.Drawing.Point(88, 22);
            this.cboVideoSource.Name = "cboVideoSource";
            this.cboVideoSource.Size = new System.Drawing.Size(184, 21);
            this.cboVideoSource.TabIndex = 1;
            this.cboVideoSource.SelectedIndexChanged += new System.EventHandler(this.cboVideoSource_SelectedIndexChanged);
            // 
            // lblVideoSource
            // 
            this.lblVideoSource.AutoSize = true;
            this.lblVideoSource.Location = new System.Drawing.Point(8, 26);
            this.lblVideoSource.Name = "lblVideoSource";
            this.lblVideoSource.Size = new System.Drawing.Size(72, 13);
            this.lblVideoSource.TabIndex = 0;
            this.lblVideoSource.Text = "Video source:";
            // 
            // cboAudioSource
            // 
            this.cboAudioSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioSource.FormattingEnabled = true;
            this.cboAudioSource.Location = new System.Drawing.Point(88, 52);
            this.cboAudioSource.Name = "cboAudioSource";
            this.cboAudioSource.Size = new System.Drawing.Size(184, 21);
            this.cboAudioSource.TabIndex = 3;
            this.cboAudioSource.SelectedIndexChanged += new System.EventHandler(this.cboAudioSource_SelectedIndexChanged);
            // 
            // lblAudioSource
            // 
            this.lblAudioSource.AutoSize = true;
            this.lblAudioSource.Location = new System.Drawing.Point(8, 56);
            this.lblAudioSource.Name = "lblAudioSource";
            this.lblAudioSource.Size = new System.Drawing.Size(72, 13);
            this.lblAudioSource.TabIndex = 2;
            this.lblAudioSource.Text = "Audio source:";
            // 
            // cboAudioCodec
            // 
            this.cboAudioCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioCodec.FormattingEnabled = true;
            this.cboAudioCodec.Location = new System.Drawing.Point(88, 52);
            this.cboAudioCodec.Name = "cboAudioCodec";
            this.cboAudioCodec.Size = new System.Drawing.Size(88, 21);
            this.cboAudioCodec.TabIndex = 3;
            this.cboAudioCodec.SelectedIndexChanged += new System.EventHandler(this.cboAudioCodec_SelectedIndexChanged);
            // 
            // lblAudioCodec
            // 
            this.lblAudioCodec.AutoSize = true;
            this.lblAudioCodec.Location = new System.Drawing.Point(8, 56);
            this.lblAudioCodec.Name = "lblAudioCodec";
            this.lblAudioCodec.Size = new System.Drawing.Size(70, 13);
            this.lblAudioCodec.TabIndex = 2;
            this.lblAudioCodec.Text = "Audio codec:";
            // 
            // gbSource
            // 
            this.gbSource.Controls.Add(this.cboVideoSource);
            this.gbSource.Controls.Add(this.lblVideoSource);
            this.gbSource.Controls.Add(this.cboAudioSource);
            this.gbSource.Controls.Add(this.lblAudioSource);
            this.gbSource.Location = new System.Drawing.Point(8, 68);
            this.gbSource.Name = "gbSource";
            this.gbSource.Size = new System.Drawing.Size(280, 88);
            this.gbSource.TabIndex = 1;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "Sources";
            // 
            // gbCodecs
            // 
            this.gbCodecs.Controls.Add(this.cboAudioCodec);
            this.gbCodecs.Controls.Add(this.lblAudioCodec);
            this.gbCodecs.Controls.Add(this.cboVideoCodec);
            this.gbCodecs.Controls.Add(this.lblCodec);
            this.gbCodecs.Location = new System.Drawing.Point(296, 68);
            this.gbCodecs.Name = "gbCodecs";
            this.gbCodecs.Size = new System.Drawing.Size(184, 88);
            this.gbCodecs.TabIndex = 2;
            this.gbCodecs.TabStop = false;
            this.gbCodecs.Text = "Codecs";
            // 
            // gbContainer
            // 
            this.gbContainer.Controls.Add(this.cboExtension);
            this.gbContainer.Controls.Add(this.lblExt);
            this.gbContainer.Location = new System.Drawing.Point(488, 68);
            this.gbContainer.Name = "gbContainer";
            this.gbContainer.Size = new System.Drawing.Size(168, 88);
            this.gbContainer.TabIndex = 3;
            this.gbContainer.TabStop = false;
            this.gbContainer.Text = "Container format";
            // 
            // btnRefreshSources
            // 
            this.btnRefreshSources.Location = new System.Drawing.Point(225, 64);
            this.btnRefreshSources.Name = "btnRefreshSources";
            this.btnRefreshSources.Size = new System.Drawing.Size(56, 23);
            this.btnRefreshSources.TabIndex = 0;
            this.btnRefreshSources.Text = "Refresh";
            this.btnRefreshSources.UseVisualStyleBackColor = true;
            this.btnRefreshSources.Click += new System.EventHandler(this.btnRefreshSources_Click);
            // 
            // cbShowError
            // 
            this.cbShowError.AutoSize = true;
            this.cbShowError.Location = new System.Drawing.Point(9, 416);
            this.cbShowError.Name = "cbShowError";
            this.cbShowError.Size = new System.Drawing.Size(274, 17);
            this.cbShowError.TabIndex = 11;
            this.cbShowError.Text = "If recording or encoding fails then show error window";
            this.cbShowError.UseVisualStyleBackColor = true;
            this.cbShowError.CheckedChanged += new System.EventHandler(this.cbShowError_CheckedChanged);
            // 
            // FFmpegOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(666, 442);
            this.Controls.Add(this.cbShowError);
            this.Controls.Add(this.btnRefreshSources);
            this.Controls.Add(this.gbContainer);
            this.Controls.Add(this.gbCodecs);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.tcFFmpegAudioCodecs);
            this.Controls.Add(this.btnCopyPreview);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.tcFFmpegVideoCodecs);
            this.Controls.Add(this.gbCommandLinePreview);
            this.Controls.Add(this.gbFFmpegExe);
            this.Controls.Add(this.gbCommandLineArgs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FFmpegOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - FFmpeg options";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.FFmpegOptionsForm_HelpButtonClicked);
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVPxCRF)).EndInit();
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
            this.tpXvid.ResumeLayout(false);
            this.tpXvid.PerformLayout();
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
            this.gbContainer.ResumeLayout(false);
            this.gbContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.Label lblX264CRF;
        private System.Windows.Forms.NumericUpDown nudx264CRF;
        private System.Windows.Forms.ToolTip ttHelpTip;
        private System.Windows.Forms.ComboBox cboExtension;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox cboVideoCodec;
        private System.Windows.Forms.ComboBox cbPreset;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.NumericUpDown nudQscale;
        private System.Windows.Forms.Label lblQscale;
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
        private System.Windows.Forms.NumericUpDown nudVPxCRF;
        private System.Windows.Forms.Label lblVpxCRF;
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
        private System.Windows.Forms.GroupBox gbContainer;
        private System.Windows.Forms.Button btnRefreshSources;
        private System.Windows.Forms.TrackBar tbVorbis_qscale;
        private System.Windows.Forms.Label lblVorbisQuality;
        private System.Windows.Forms.TrackBar tbMP3_qscale;
        private System.Windows.Forms.Label lblMP3Quality;
        private System.Windows.Forms.TabPage tpAAC;
        private System.Windows.Forms.TrackBar tbAACBitrate;
        private System.Windows.Forms.Label lblAACQuality;
        private System.Windows.Forms.CheckBox cbShowError;
        private System.Windows.Forms.CheckBox cbCustomCommands;
    }
}