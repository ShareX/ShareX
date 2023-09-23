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
            this.ttHelpTip = new System.Windows.Forms.ToolTip(this.components);
            this.pbx264PresetWarning = new System.Windows.Forms.PictureBox();
            this.cbx264Preset = new System.Windows.Forms.ComboBox();
            this.nudx264CRF = new System.Windows.Forms.NumericUpDown();
            this.nudXvidQscale = new System.Windows.Forms.NumericUpDown();
            this.nudGIFBayerScale = new System.Windows.Forms.NumericUpDown();
            this.cbGIFDither = new System.Windows.Forms.ComboBox();
            this.cbGIFStatsMode = new System.Windows.Forms.ComboBox();
            this.cbVideoCodec = new System.Windows.Forms.ComboBox();
            this.btnFFmpegBrowse = new System.Windows.Forms.Button();
            this.txtFFmpegPath = new System.Windows.Forms.TextBox();
            this.cbCustomCommands = new System.Windows.Forms.CheckBox();
            this.txtCommandLinePreview = new System.Windows.Forms.TextBox();
            this.txtUserArgs = new System.Windows.Forms.TextBox();
            this.cbVideoSource = new System.Windows.Forms.ComboBox();
            this.lblVideoSource = new System.Windows.Forms.Label();
            this.cbAudioSource = new System.Windows.Forms.ComboBox();
            this.lblAudioSource = new System.Windows.Forms.Label();
            this.cbAudioCodec = new System.Windows.Forms.ComboBox();
            this.btnHelperDevicesHelp = new System.Windows.Forms.Button();
            this.lblHelperDevices = new System.Windows.Forms.Label();
            this.btnInstallHelperDevices = new System.Windows.Forms.Button();
            this.lblCommandLineArgs = new System.Windows.Forms.Label();
            this.cbUseCustomFFmpegPath = new System.Windows.Forms.CheckBox();
            this.lblVideoEncoder = new System.Windows.Forms.Label();
            this.lblAudioEncoder = new System.Windows.Forms.Label();
            this.tcFFmpegAudioCodecs = new ShareX.HelpersLib.TablessControl();
            this.tpAAC = new System.Windows.Forms.TabPage();
            this.lblAACBitrateK = new System.Windows.Forms.Label();
            this.cbAACBitrate = new System.Windows.Forms.ComboBox();
            this.lblAACBitrate = new System.Windows.Forms.Label();
            this.tpOpus = new System.Windows.Forms.TabPage();
            this.lblOpusBitrateK = new System.Windows.Forms.Label();
            this.cbOpusBitrate = new System.Windows.Forms.ComboBox();
            this.lblOpusBitrate = new System.Windows.Forms.Label();
            this.tpVorbis = new System.Windows.Forms.TabPage();
            this.cbVorbisQuality = new System.Windows.Forms.ComboBox();
            this.lblVorbisQuality = new System.Windows.Forms.Label();
            this.tpMP3 = new System.Windows.Forms.TabPage();
            this.lblMP3Quality = new System.Windows.Forms.Label();
            this.tcFFmpegVideoCodecs = new ShareX.HelpersLib.TablessControl();
            this.tpX264 = new System.Windows.Forms.TabPage();
            this.lblx264BitrateK = new System.Windows.Forms.Label();
            this.cbx264UseBitrate = new System.Windows.Forms.CheckBox();
            this.lblx264CRF = new System.Windows.Forms.Label();
            this.lblx264Preset = new System.Windows.Forms.Label();
            this.nudx264Bitrate = new System.Windows.Forms.NumericUpDown();
            this.tpVpx = new System.Windows.Forms.TabPage();
            this.lblVP8BitrateK = new System.Windows.Forms.Label();
            this.nudVP8Bitrate = new System.Windows.Forms.NumericUpDown();
            this.lblVP8Bitrate = new System.Windows.Forms.Label();
            this.tpXvid = new System.Windows.Forms.TabPage();
            this.lblXvidQscale = new System.Windows.Forms.Label();
            this.tpNVENC = new System.Windows.Forms.TabPage();
            this.cbNVENCTune = new System.Windows.Forms.ComboBox();
            this.lblNVENCTune = new System.Windows.Forms.Label();
            this.lblNVENCBitrateK = new System.Windows.Forms.Label();
            this.cbNVENCPreset = new System.Windows.Forms.ComboBox();
            this.lblNVENCPreset = new System.Windows.Forms.Label();
            this.nudNVENCBitrate = new System.Windows.Forms.NumericUpDown();
            this.lblNVENCBitrate = new System.Windows.Forms.Label();
            this.tpGIF = new System.Windows.Forms.TabPage();
            this.lblGIFDither = new System.Windows.Forms.Label();
            this.lblGIFStatsMode = new System.Windows.Forms.Label();
            this.tpAMF = new System.Windows.Forms.TabPage();
            this.lblAMFBitrateK = new System.Windows.Forms.Label();
            this.nudAMFBitrate = new System.Windows.Forms.NumericUpDown();
            this.lblAMFBitrate = new System.Windows.Forms.Label();
            this.cbAMFQuality = new System.Windows.Forms.ComboBox();
            this.lblAMFQuality = new System.Windows.Forms.Label();
            this.cbAMFUsage = new System.Windows.Forms.ComboBox();
            this.lblAMFUsage = new System.Windows.Forms.Label();
            this.tpQSV = new System.Windows.Forms.TabPage();
            this.lblQSVBitrateK = new System.Windows.Forms.Label();
            this.cbQSVPreset = new System.Windows.Forms.ComboBox();
            this.lblQSVPreset = new System.Windows.Forms.Label();
            this.nudQSVBitrate = new System.Windows.Forms.NumericUpDown();
            this.lblQSVBitrate = new System.Windows.Forms.Label();
            this.btnResetOptions = new System.Windows.Forms.Button();
            this.cbMP3Quality = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbx264PresetWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXvidQscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGIFBayerScale)).BeginInit();
            this.tcFFmpegAudioCodecs.SuspendLayout();
            this.tpAAC.SuspendLayout();
            this.tpOpus.SuspendLayout();
            this.tpVorbis.SuspendLayout();
            this.tpMP3.SuspendLayout();
            this.tcFFmpegVideoCodecs.SuspendLayout();
            this.tpX264.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264Bitrate)).BeginInit();
            this.tpVpx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVP8Bitrate)).BeginInit();
            this.tpXvid.SuspendLayout();
            this.tpNVENC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNVENCBitrate)).BeginInit();
            this.tpGIF.SuspendLayout();
            this.tpAMF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAMFBitrate)).BeginInit();
            this.tpQSV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudQSVBitrate)).BeginInit();
            this.SuspendLayout();
            // 
            // ttHelpTip
            // 
            this.ttHelpTip.AutomaticDelay = 0;
            this.ttHelpTip.AutoPopDelay = 30000;
            this.ttHelpTip.BackColor = System.Drawing.SystemColors.Window;
            this.ttHelpTip.InitialDelay = 500;
            this.ttHelpTip.ReshowDelay = 100;
            this.ttHelpTip.UseAnimation = false;
            this.ttHelpTip.UseFading = false;
            // 
            // pbx264PresetWarning
            // 
            this.pbx264PresetWarning.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.exclamation_button;
            resources.ApplyResources(this.pbx264PresetWarning, "pbx264PresetWarning");
            this.pbx264PresetWarning.Name = "pbx264PresetWarning";
            this.pbx264PresetWarning.TabStop = false;
            this.ttHelpTip.SetToolTip(this.pbx264PresetWarning, resources.GetString("pbx264PresetWarning.ToolTip"));
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
            // nudGIFBayerScale
            // 
            resources.ApplyResources(this.nudGIFBayerScale, "nudGIFBayerScale");
            this.nudGIFBayerScale.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudGIFBayerScale.Name = "nudGIFBayerScale";
            this.ttHelpTip.SetToolTip(this.nudGIFBayerScale, resources.GetString("nudGIFBayerScale.ToolTip"));
            this.nudGIFBayerScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGIFBayerScale.ValueChanged += new System.EventHandler(this.nudGIFBayerScale_SelectedIndexChanged);
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
            // cbGIFStatsMode
            // 
            this.cbGIFStatsMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGIFStatsMode.FormattingEnabled = true;
            resources.ApplyResources(this.cbGIFStatsMode, "cbGIFStatsMode");
            this.cbGIFStatsMode.Name = "cbGIFStatsMode";
            this.ttHelpTip.SetToolTip(this.cbGIFStatsMode, resources.GetString("cbGIFStatsMode.ToolTip"));
            this.cbGIFStatsMode.SelectedIndexChanged += new System.EventHandler(this.cbGIFStatsMode_SelectedIndexChanged);
            // 
            // cbVideoCodec
            // 
            this.cbVideoCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoCodec.FormattingEnabled = true;
            resources.ApplyResources(this.cbVideoCodec, "cbVideoCodec");
            this.cbVideoCodec.Name = "cbVideoCodec";
            this.cbVideoCodec.SelectedIndexChanged += new System.EventHandler(this.cbVideoCodec_SelectedIndexChanged);
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
            // txtUserArgs
            // 
            this.txtUserArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtUserArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            resources.ApplyResources(this.txtUserArgs, "txtUserArgs");
            this.txtUserArgs.Name = "txtUserArgs";
            this.txtUserArgs.TextChanged += new System.EventHandler(this.txtUserArgs_TextChanged);
            // 
            // cbVideoSource
            // 
            this.cbVideoSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoSource.FormattingEnabled = true;
            resources.ApplyResources(this.cbVideoSource, "cbVideoSource");
            this.cbVideoSource.Name = "cbVideoSource";
            this.cbVideoSource.SelectedIndexChanged += new System.EventHandler(this.cbVideoSource_SelectedIndexChanged);
            // 
            // lblVideoSource
            // 
            resources.ApplyResources(this.lblVideoSource, "lblVideoSource");
            this.lblVideoSource.Name = "lblVideoSource";
            // 
            // cbAudioSource
            // 
            this.cbAudioSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioSource.FormattingEnabled = true;
            resources.ApplyResources(this.cbAudioSource, "cbAudioSource");
            this.cbAudioSource.Name = "cbAudioSource";
            this.cbAudioSource.SelectedIndexChanged += new System.EventHandler(this.cbAudioSource_SelectedIndexChanged);
            // 
            // lblAudioSource
            // 
            resources.ApplyResources(this.lblAudioSource, "lblAudioSource");
            this.lblAudioSource.Name = "lblAudioSource";
            // 
            // cbAudioCodec
            // 
            this.cbAudioCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioCodec.FormattingEnabled = true;
            resources.ApplyResources(this.cbAudioCodec, "cbAudioCodec");
            this.cbAudioCodec.Name = "cbAudioCodec";
            this.cbAudioCodec.SelectedIndexChanged += new System.EventHandler(this.cbAudioCodec_SelectedIndexChanged);
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
            // lblCommandLineArgs
            // 
            resources.ApplyResources(this.lblCommandLineArgs, "lblCommandLineArgs");
            this.lblCommandLineArgs.Name = "lblCommandLineArgs";
            // 
            // cbUseCustomFFmpegPath
            // 
            resources.ApplyResources(this.cbUseCustomFFmpegPath, "cbUseCustomFFmpegPath");
            this.cbUseCustomFFmpegPath.Name = "cbUseCustomFFmpegPath";
            this.cbUseCustomFFmpegPath.UseVisualStyleBackColor = true;
            this.cbUseCustomFFmpegPath.CheckedChanged += new System.EventHandler(this.cbUseCustomFFmpegPath_CheckedChanged);
            // 
            // lblVideoEncoder
            // 
            resources.ApplyResources(this.lblVideoEncoder, "lblVideoEncoder");
            this.lblVideoEncoder.Name = "lblVideoEncoder";
            // 
            // lblAudioEncoder
            // 
            resources.ApplyResources(this.lblAudioEncoder, "lblAudioEncoder");
            this.lblAudioEncoder.Name = "lblAudioEncoder";
            // 
            // tcFFmpegAudioCodecs
            // 
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpAAC);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpOpus);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpVorbis);
            this.tcFFmpegAudioCodecs.Controls.Add(this.tpMP3);
            resources.ApplyResources(this.tcFFmpegAudioCodecs, "tcFFmpegAudioCodecs");
            this.tcFFmpegAudioCodecs.Multiline = true;
            this.tcFFmpegAudioCodecs.Name = "tcFFmpegAudioCodecs";
            this.tcFFmpegAudioCodecs.SelectedIndex = 0;
            // 
            // tpAAC
            // 
            this.tpAAC.BackColor = System.Drawing.SystemColors.Window;
            this.tpAAC.Controls.Add(this.lblAACBitrateK);
            this.tpAAC.Controls.Add(this.cbAACBitrate);
            this.tpAAC.Controls.Add(this.lblAACBitrate);
            resources.ApplyResources(this.tpAAC, "tpAAC");
            this.tpAAC.Name = "tpAAC";
            // 
            // lblAACBitrateK
            // 
            resources.ApplyResources(this.lblAACBitrateK, "lblAACBitrateK");
            this.lblAACBitrateK.Name = "lblAACBitrateK";
            // 
            // cbAACBitrate
            // 
            this.cbAACBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAACBitrate.FormattingEnabled = true;
            resources.ApplyResources(this.cbAACBitrate, "cbAACBitrate");
            this.cbAACBitrate.Name = "cbAACBitrate";
            this.cbAACBitrate.SelectedIndexChanged += new System.EventHandler(this.cbAACBitrate_SelectedIndexChanged);
            // 
            // lblAACBitrate
            // 
            resources.ApplyResources(this.lblAACBitrate, "lblAACBitrate");
            this.lblAACBitrate.Name = "lblAACBitrate";
            // 
            // tpOpus
            // 
            this.tpOpus.Controls.Add(this.lblOpusBitrateK);
            this.tpOpus.Controls.Add(this.cbOpusBitrate);
            this.tpOpus.Controls.Add(this.lblOpusBitrate);
            resources.ApplyResources(this.tpOpus, "tpOpus");
            this.tpOpus.Name = "tpOpus";
            this.tpOpus.UseVisualStyleBackColor = true;
            // 
            // lblOpusBitrateK
            // 
            resources.ApplyResources(this.lblOpusBitrateK, "lblOpusBitrateK");
            this.lblOpusBitrateK.Name = "lblOpusBitrateK";
            // 
            // cbOpusBitrate
            // 
            this.cbOpusBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOpusBitrate.FormattingEnabled = true;
            resources.ApplyResources(this.cbOpusBitrate, "cbOpusBitrate");
            this.cbOpusBitrate.Name = "cbOpusBitrate";
            this.cbOpusBitrate.SelectedIndexChanged += new System.EventHandler(this.cbOpusBitrate_SelectedIndexChanged);
            // 
            // lblOpusBitrate
            // 
            resources.ApplyResources(this.lblOpusBitrate, "lblOpusBitrate");
            this.lblOpusBitrate.Name = "lblOpusBitrate";
            // 
            // tpVorbis
            // 
            this.tpVorbis.BackColor = System.Drawing.SystemColors.Window;
            this.tpVorbis.Controls.Add(this.cbVorbisQuality);
            this.tpVorbis.Controls.Add(this.lblVorbisQuality);
            resources.ApplyResources(this.tpVorbis, "tpVorbis");
            this.tpVorbis.Name = "tpVorbis";
            // 
            // cbVorbisQuality
            // 
            this.cbVorbisQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVorbisQuality.FormattingEnabled = true;
            resources.ApplyResources(this.cbVorbisQuality, "cbVorbisQuality");
            this.cbVorbisQuality.Name = "cbVorbisQuality";
            this.cbVorbisQuality.SelectedIndexChanged += new System.EventHandler(this.cbVorbisQuality_SelectedIndexChanged);
            // 
            // lblVorbisQuality
            // 
            resources.ApplyResources(this.lblVorbisQuality, "lblVorbisQuality");
            this.lblVorbisQuality.Name = "lblVorbisQuality";
            // 
            // tpMP3
            // 
            this.tpMP3.BackColor = System.Drawing.SystemColors.Window;
            this.tpMP3.Controls.Add(this.cbMP3Quality);
            this.tpMP3.Controls.Add(this.lblMP3Quality);
            resources.ApplyResources(this.tpMP3, "tpMP3");
            this.tpMP3.Name = "tpMP3";
            // 
            // lblMP3Quality
            // 
            resources.ApplyResources(this.lblMP3Quality, "lblMP3Quality");
            this.lblMP3Quality.Name = "lblMP3Quality";
            // 
            // tcFFmpegVideoCodecs
            // 
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpX264);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpVpx);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpXvid);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpNVENC);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpGIF);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpAMF);
            this.tcFFmpegVideoCodecs.Controls.Add(this.tpQSV);
            resources.ApplyResources(this.tcFFmpegVideoCodecs, "tcFFmpegVideoCodecs");
            this.tcFFmpegVideoCodecs.Multiline = true;
            this.tcFFmpegVideoCodecs.Name = "tcFFmpegVideoCodecs";
            this.tcFFmpegVideoCodecs.SelectedIndex = 0;
            // 
            // tpX264
            // 
            this.tpX264.BackColor = System.Drawing.SystemColors.Window;
            this.tpX264.Controls.Add(this.lblx264BitrateK);
            this.tpX264.Controls.Add(this.cbx264UseBitrate);
            this.tpX264.Controls.Add(this.pbx264PresetWarning);
            this.tpX264.Controls.Add(this.lblx264CRF);
            this.tpX264.Controls.Add(this.cbx264Preset);
            this.tpX264.Controls.Add(this.lblx264Preset);
            this.tpX264.Controls.Add(this.nudx264CRF);
            this.tpX264.Controls.Add(this.nudx264Bitrate);
            resources.ApplyResources(this.tpX264, "tpX264");
            this.tpX264.Name = "tpX264";
            // 
            // lblx264BitrateK
            // 
            resources.ApplyResources(this.lblx264BitrateK, "lblx264BitrateK");
            this.lblx264BitrateK.Name = "lblx264BitrateK";
            // 
            // cbx264UseBitrate
            // 
            resources.ApplyResources(this.cbx264UseBitrate, "cbx264UseBitrate");
            this.cbx264UseBitrate.Name = "cbx264UseBitrate";
            this.cbx264UseBitrate.UseVisualStyleBackColor = true;
            this.cbx264UseBitrate.CheckedChanged += new System.EventHandler(this.cbx264UseBitrate_CheckedChanged);
            // 
            // lblx264CRF
            // 
            resources.ApplyResources(this.lblx264CRF, "lblx264CRF");
            this.lblx264CRF.Name = "lblx264CRF";
            // 
            // lblx264Preset
            // 
            resources.ApplyResources(this.lblx264Preset, "lblx264Preset");
            this.lblx264Preset.Name = "lblx264Preset";
            // 
            // nudx264Bitrate
            // 
            resources.ApplyResources(this.nudx264Bitrate, "nudx264Bitrate");
            this.nudx264Bitrate.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudx264Bitrate.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudx264Bitrate.Name = "nudx264Bitrate";
            this.nudx264Bitrate.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudx264Bitrate.ValueChanged += new System.EventHandler(this.nudx264Bitrate_ValueChanged);
            // 
            // tpVpx
            // 
            this.tpVpx.BackColor = System.Drawing.SystemColors.Window;
            this.tpVpx.Controls.Add(this.lblVP8BitrateK);
            this.tpVpx.Controls.Add(this.nudVP8Bitrate);
            this.tpVpx.Controls.Add(this.lblVP8Bitrate);
            resources.ApplyResources(this.tpVpx, "tpVpx");
            this.tpVpx.Name = "tpVpx";
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
            100000,
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
            3000,
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
            this.tpXvid.BackColor = System.Drawing.SystemColors.Window;
            this.tpXvid.Controls.Add(this.nudXvidQscale);
            this.tpXvid.Controls.Add(this.lblXvidQscale);
            resources.ApplyResources(this.tpXvid, "tpXvid");
            this.tpXvid.Name = "tpXvid";
            // 
            // lblXvidQscale
            // 
            resources.ApplyResources(this.lblXvidQscale, "lblXvidQscale");
            this.lblXvidQscale.Name = "lblXvidQscale";
            // 
            // tpNVENC
            // 
            this.tpNVENC.BackColor = System.Drawing.SystemColors.Window;
            this.tpNVENC.Controls.Add(this.cbNVENCTune);
            this.tpNVENC.Controls.Add(this.lblNVENCTune);
            this.tpNVENC.Controls.Add(this.lblNVENCBitrateK);
            this.tpNVENC.Controls.Add(this.cbNVENCPreset);
            this.tpNVENC.Controls.Add(this.lblNVENCPreset);
            this.tpNVENC.Controls.Add(this.nudNVENCBitrate);
            this.tpNVENC.Controls.Add(this.lblNVENCBitrate);
            resources.ApplyResources(this.tpNVENC, "tpNVENC");
            this.tpNVENC.Name = "tpNVENC";
            // 
            // cbNVENCTune
            // 
            this.cbNVENCTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNVENCTune.FormattingEnabled = true;
            resources.ApplyResources(this.cbNVENCTune, "cbNVENCTune");
            this.cbNVENCTune.Name = "cbNVENCTune";
            this.cbNVENCTune.SelectedIndexChanged += new System.EventHandler(this.cbNVENCTune_SelectedIndexChanged);
            // 
            // lblNVENCTune
            // 
            resources.ApplyResources(this.lblNVENCTune, "lblNVENCTune");
            this.lblNVENCTune.Name = "lblNVENCTune";
            // 
            // lblNVENCBitrateK
            // 
            resources.ApplyResources(this.lblNVENCBitrateK, "lblNVENCBitrateK");
            this.lblNVENCBitrateK.Name = "lblNVENCBitrateK";
            // 
            // cbNVENCPreset
            // 
            this.cbNVENCPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNVENCPreset.FormattingEnabled = true;
            resources.ApplyResources(this.cbNVENCPreset, "cbNVENCPreset");
            this.cbNVENCPreset.Name = "cbNVENCPreset";
            this.cbNVENCPreset.SelectedIndexChanged += new System.EventHandler(this.cbNVENCPreset_SelectedIndexChanged);
            // 
            // lblNVENCPreset
            // 
            resources.ApplyResources(this.lblNVENCPreset, "lblNVENCPreset");
            this.lblNVENCPreset.Name = "lblNVENCPreset";
            // 
            // nudNVENCBitrate
            // 
            resources.ApplyResources(this.nudNVENCBitrate, "nudNVENCBitrate");
            this.nudNVENCBitrate.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudNVENCBitrate.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudNVENCBitrate.Name = "nudNVENCBitrate";
            this.nudNVENCBitrate.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudNVENCBitrate.ValueChanged += new System.EventHandler(this.nudNVENCBitrate_ValueChanged);
            // 
            // lblNVENCBitrate
            // 
            resources.ApplyResources(this.lblNVENCBitrate, "lblNVENCBitrate");
            this.lblNVENCBitrate.Name = "lblNVENCBitrate";
            // 
            // tpGIF
            // 
            this.tpGIF.BackColor = System.Drawing.SystemColors.Window;
            this.tpGIF.Controls.Add(this.nudGIFBayerScale);
            this.tpGIF.Controls.Add(this.cbGIFDither);
            this.tpGIF.Controls.Add(this.lblGIFDither);
            this.tpGIF.Controls.Add(this.cbGIFStatsMode);
            this.tpGIF.Controls.Add(this.lblGIFStatsMode);
            resources.ApplyResources(this.tpGIF, "tpGIF");
            this.tpGIF.Name = "tpGIF";
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
            // tpAMF
            // 
            this.tpAMF.Controls.Add(this.lblAMFBitrateK);
            this.tpAMF.Controls.Add(this.nudAMFBitrate);
            this.tpAMF.Controls.Add(this.lblAMFBitrate);
            this.tpAMF.Controls.Add(this.cbAMFQuality);
            this.tpAMF.Controls.Add(this.lblAMFQuality);
            this.tpAMF.Controls.Add(this.cbAMFUsage);
            this.tpAMF.Controls.Add(this.lblAMFUsage);
            resources.ApplyResources(this.tpAMF, "tpAMF");
            this.tpAMF.Name = "tpAMF";
            this.tpAMF.UseVisualStyleBackColor = true;
            // 
            // lblAMFBitrateK
            // 
            resources.ApplyResources(this.lblAMFBitrateK, "lblAMFBitrateK");
            this.lblAMFBitrateK.Name = "lblAMFBitrateK";
            // 
            // nudAMFBitrate
            // 
            resources.ApplyResources(this.nudAMFBitrate, "nudAMFBitrate");
            this.nudAMFBitrate.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudAMFBitrate.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudAMFBitrate.Name = "nudAMFBitrate";
            this.nudAMFBitrate.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudAMFBitrate.ValueChanged += new System.EventHandler(this.nudAMFBitrate_ValueChanged);
            // 
            // lblAMFBitrate
            // 
            resources.ApplyResources(this.lblAMFBitrate, "lblAMFBitrate");
            this.lblAMFBitrate.Name = "lblAMFBitrate";
            // 
            // cbAMFQuality
            // 
            this.cbAMFQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAMFQuality.FormattingEnabled = true;
            resources.ApplyResources(this.cbAMFQuality, "cbAMFQuality");
            this.cbAMFQuality.Name = "cbAMFQuality";
            this.cbAMFQuality.SelectedIndexChanged += new System.EventHandler(this.cbAMFQuality_SelectedIndexChanged);
            // 
            // lblAMFQuality
            // 
            resources.ApplyResources(this.lblAMFQuality, "lblAMFQuality");
            this.lblAMFQuality.Name = "lblAMFQuality";
            // 
            // cbAMFUsage
            // 
            this.cbAMFUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAMFUsage.FormattingEnabled = true;
            resources.ApplyResources(this.cbAMFUsage, "cbAMFUsage");
            this.cbAMFUsage.Name = "cbAMFUsage";
            this.cbAMFUsage.SelectedIndexChanged += new System.EventHandler(this.cbAMFUsage_SelectedIndexChanged);
            // 
            // lblAMFUsage
            // 
            resources.ApplyResources(this.lblAMFUsage, "lblAMFUsage");
            this.lblAMFUsage.Name = "lblAMFUsage";
            // 
            // tpQSV
            // 
            this.tpQSV.Controls.Add(this.lblQSVBitrateK);
            this.tpQSV.Controls.Add(this.cbQSVPreset);
            this.tpQSV.Controls.Add(this.lblQSVPreset);
            this.tpQSV.Controls.Add(this.nudQSVBitrate);
            this.tpQSV.Controls.Add(this.lblQSVBitrate);
            resources.ApplyResources(this.tpQSV, "tpQSV");
            this.tpQSV.Name = "tpQSV";
            this.tpQSV.UseVisualStyleBackColor = true;
            // 
            // lblQSVBitrateK
            // 
            resources.ApplyResources(this.lblQSVBitrateK, "lblQSVBitrateK");
            this.lblQSVBitrateK.Name = "lblQSVBitrateK";
            // 
            // cbQSVPreset
            // 
            this.cbQSVPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbQSVPreset.FormattingEnabled = true;
            resources.ApplyResources(this.cbQSVPreset, "cbQSVPreset");
            this.cbQSVPreset.Name = "cbQSVPreset";
            this.cbQSVPreset.SelectedIndexChanged += new System.EventHandler(this.cbQSVPreset_SelectedIndexChanged);
            // 
            // lblQSVPreset
            // 
            resources.ApplyResources(this.lblQSVPreset, "lblQSVPreset");
            this.lblQSVPreset.Name = "lblQSVPreset";
            // 
            // nudQSVBitrate
            // 
            resources.ApplyResources(this.nudQSVBitrate, "nudQSVBitrate");
            this.nudQSVBitrate.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudQSVBitrate.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudQSVBitrate.Name = "nudQSVBitrate";
            this.nudQSVBitrate.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudQSVBitrate.ValueChanged += new System.EventHandler(this.nudQSVBitrate_ValueChanged);
            // 
            // lblQSVBitrate
            // 
            resources.ApplyResources(this.lblQSVBitrate, "lblQSVBitrate");
            this.lblQSVBitrate.Name = "lblQSVBitrate";
            // 
            // btnResetOptions
            // 
            resources.ApplyResources(this.btnResetOptions, "btnResetOptions");
            this.btnResetOptions.Name = "btnResetOptions";
            this.btnResetOptions.UseVisualStyleBackColor = true;
            this.btnResetOptions.Click += new System.EventHandler(this.btnResetOptions_Click);
            // 
            // cbMP3Quality
            // 
            this.cbMP3Quality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMP3Quality.FormattingEnabled = true;
            resources.ApplyResources(this.cbMP3Quality, "cbMP3Quality");
            this.cbMP3Quality.Name = "cbMP3Quality";
            this.cbMP3Quality.SelectedIndexChanged += new System.EventHandler(this.cbMP3Quality_SelectedIndexChanged);
            // 
            // FFmpegOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnResetOptions);
            this.Controls.Add(this.tcFFmpegAudioCodecs);
            this.Controls.Add(this.tcFFmpegVideoCodecs);
            this.Controls.Add(this.lblAudioEncoder);
            this.Controls.Add(this.lblVideoEncoder);
            this.Controls.Add(this.cbUseCustomFFmpegPath);
            this.Controls.Add(this.cbCustomCommands);
            this.Controls.Add(this.txtCommandLinePreview);
            this.Controls.Add(this.txtUserArgs);
            this.Controls.Add(this.lblCommandLineArgs);
            this.Controls.Add(this.btnHelperDevicesHelp);
            this.Controls.Add(this.cbAudioCodec);
            this.Controls.Add(this.lblHelperDevices);
            this.Controls.Add(this.cbVideoCodec);
            this.Controls.Add(this.btnInstallHelperDevices);
            this.Controls.Add(this.cbVideoSource);
            this.Controls.Add(this.btnFFmpegBrowse);
            this.Controls.Add(this.lblVideoSource);
            this.Controls.Add(this.cbAudioSource);
            this.Controls.Add(this.txtFFmpegPath);
            this.Controls.Add(this.lblAudioSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FFmpegOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.FFmpegOptionsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbx264PresetWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXvidQscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGIFBayerScale)).EndInit();
            this.tcFFmpegAudioCodecs.ResumeLayout(false);
            this.tpAAC.ResumeLayout(false);
            this.tpAAC.PerformLayout();
            this.tpOpus.ResumeLayout(false);
            this.tpOpus.PerformLayout();
            this.tpVorbis.ResumeLayout(false);
            this.tpVorbis.PerformLayout();
            this.tpMP3.ResumeLayout(false);
            this.tpMP3.PerformLayout();
            this.tcFFmpegVideoCodecs.ResumeLayout(false);
            this.tpX264.ResumeLayout(false);
            this.tpX264.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264Bitrate)).EndInit();
            this.tpVpx.ResumeLayout(false);
            this.tpVpx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVP8Bitrate)).EndInit();
            this.tpXvid.ResumeLayout(false);
            this.tpXvid.PerformLayout();
            this.tpNVENC.ResumeLayout(false);
            this.tpNVENC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNVENCBitrate)).EndInit();
            this.tpGIF.ResumeLayout(false);
            this.tpGIF.PerformLayout();
            this.tpAMF.ResumeLayout(false);
            this.tpAMF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAMFBitrate)).EndInit();
            this.tpQSV.ResumeLayout(false);
            this.tpQSV.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudQSVBitrate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblx264CRF;
        private System.Windows.Forms.NumericUpDown nudx264CRF;
        private System.Windows.Forms.ToolTip ttHelpTip;
        private System.Windows.Forms.ComboBox cbVideoCodec;
        private System.Windows.Forms.ComboBox cbx264Preset;
        private System.Windows.Forms.Label lblx264Preset;
        private System.Windows.Forms.NumericUpDown nudXvidQscale;
        private System.Windows.Forms.Label lblXvidQscale;
        private System.Windows.Forms.Button btnFFmpegBrowse;
        private System.Windows.Forms.TextBox txtFFmpegPath;
        private System.Windows.Forms.TextBox txtCommandLinePreview;
        private System.Windows.Forms.TextBox txtUserArgs;
        private HelpersLib.TablessControl tcFFmpegVideoCodecs;
        private System.Windows.Forms.TabPage tpX264;
        private System.Windows.Forms.TabPage tpVpx;
        private System.Windows.Forms.TabPage tpXvid;
        private HelpersLib.TablessControl tcFFmpegAudioCodecs;
        private System.Windows.Forms.TabPage tpVorbis;
        private System.Windows.Forms.TabPage tpMP3;
        private System.Windows.Forms.ComboBox cbVideoSource;
        private System.Windows.Forms.Label lblVideoSource;
        private System.Windows.Forms.ComboBox cbAudioSource;
        private System.Windows.Forms.Label lblAudioSource;
        private System.Windows.Forms.ComboBox cbAudioCodec;
        private System.Windows.Forms.Label lblVorbisQuality;
        private System.Windows.Forms.Label lblMP3Quality;
        private System.Windows.Forms.TabPage tpAAC;
        private System.Windows.Forms.Label lblAACBitrate;
        private System.Windows.Forms.CheckBox cbCustomCommands;
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
        private System.Windows.Forms.PictureBox pbx264PresetWarning;
        private System.Windows.Forms.TabPage tpNVENC;
        private System.Windows.Forms.ComboBox cbNVENCPreset;
        private System.Windows.Forms.Label lblNVENCPreset;
        private System.Windows.Forms.NumericUpDown nudNVENCBitrate;
        private System.Windows.Forms.Label lblNVENCBitrate;
        private System.Windows.Forms.TabPage tpAMF;
        private System.Windows.Forms.ComboBox cbAMFUsage;
        private System.Windows.Forms.Label lblAMFUsage;
        private System.Windows.Forms.ComboBox cbAMFQuality;
        private System.Windows.Forms.Label lblAMFQuality;
        private System.Windows.Forms.TabPage tpQSV;
        private System.Windows.Forms.ComboBox cbQSVPreset;
        private System.Windows.Forms.Label lblQSVPreset;
        private System.Windows.Forms.NumericUpDown nudQSVBitrate;
        private System.Windows.Forms.Label lblQSVBitrate;
        private System.Windows.Forms.TabPage tpOpus;
        private System.Windows.Forms.Label lblOpusBitrate;
        private System.Windows.Forms.NumericUpDown nudGIFBayerScale;
        private System.Windows.Forms.NumericUpDown nudx264Bitrate;
        private System.Windows.Forms.CheckBox cbx264UseBitrate;
        private System.Windows.Forms.Label lblx264BitrateK;
        private System.Windows.Forms.Label lblNVENCBitrateK;
        private System.Windows.Forms.Label lblQSVBitrateK;
        private System.Windows.Forms.Label lblCommandLineArgs;
        private System.Windows.Forms.CheckBox cbUseCustomFFmpegPath;
        private System.Windows.Forms.Label lblVideoEncoder;
        private System.Windows.Forms.Label lblAudioEncoder;
        private System.Windows.Forms.Label lblAMFBitrateK;
        private System.Windows.Forms.NumericUpDown nudAMFBitrate;
        private System.Windows.Forms.Label lblAMFBitrate;
        private System.Windows.Forms.ComboBox cbNVENCTune;
        private System.Windows.Forms.Label lblNVENCTune;
        private System.Windows.Forms.Button btnResetOptions;
        private System.Windows.Forms.ComboBox cbAACBitrate;
        private System.Windows.Forms.Label lblAACBitrateK;
        private System.Windows.Forms.ComboBox cbOpusBitrate;
        private System.Windows.Forms.Label lblOpusBitrateK;
        private System.Windows.Forms.ComboBox cbVorbisQuality;
        private System.Windows.Forms.ComboBox cbMP3Quality;
    }
}