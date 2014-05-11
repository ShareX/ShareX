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
            this.lblCRF = new System.Windows.Forms.Label();
            this.nudCRF = new System.Windows.Forms.NumericUpDown();
            this.toolTipFFmpeg = new System.Windows.Forms.ToolTip(this.components);
            this.nudQscale = new System.Windows.Forms.NumericUpDown();
            this.comboBoxExtension = new System.Windows.Forms.ComboBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.comboBoxCodec = new System.Windows.Forms.ComboBox();
            this.comboBoxPreset = new System.Windows.Forms.ComboBox();
            this.lblPreset = new System.Windows.Forms.Label();
            this.groupBoxH264 = new System.Windows.Forms.GroupBox();
            this.groupBoxH263 = new System.Windows.Forms.GroupBox();
            this.labelQscale = new System.Windows.Forms.Label();
            this.groupBoxFFmpegExe = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.buttonFFmpegBrowse = new System.Windows.Forms.Button();
            this.textBoxFFmpegPath = new System.Windows.Forms.TextBox();
            this.groupBoxCommandLinePreview = new System.Windows.Forms.GroupBox();
            this.textBoxCommandLinePreview = new System.Windows.Forms.TextBox();
            this.gbCommandLineArgs = new System.Windows.Forms.GroupBox();
            this.buttonFFmpegHelp = new System.Windows.Forms.Button();
            this.textBoxUserArgs = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudCRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).BeginInit();
            this.groupBoxH264.SuspendLayout();
            this.groupBoxH263.SuspendLayout();
            this.groupBoxFFmpegExe.SuspendLayout();
            this.groupBoxCommandLinePreview.SuspendLayout();
            this.gbCommandLineArgs.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(240, 13);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(56, 13);
            this.lblExt.TabIndex = 11;
            this.lblExt.Text = "Extension:";
            // 
            // lblCRF
            // 
            this.lblCRF.AutoSize = true;
            this.lblCRF.Location = new System.Drawing.Point(8, 29);
            this.lblCRF.Name = "lblCRF";
            this.lblCRF.Size = new System.Drawing.Size(31, 13);
            this.lblCRF.TabIndex = 13;
            this.lblCRF.Text = "CRF:";
            // 
            // nudCRF
            // 
            this.nudCRF.Location = new System.Drawing.Point(80, 24);
            this.nudCRF.Maximum = new decimal(new int[] {
            51,
            0,
            0,
            0});
            this.nudCRF.Name = "nudCRF";
            this.nudCRF.Size = new System.Drawing.Size(121, 20);
            this.nudCRF.TabIndex = 14;
            this.toolTipFFmpeg.SetToolTip(this.nudCRF, resources.GetString("nudCRF.ToolTip"));
            this.nudCRF.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            // 
            // nudQscale
            // 
            this.nudQscale.Location = new System.Drawing.Point(88, 24);
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
            this.nudQscale.Size = new System.Drawing.Size(121, 20);
            this.nudQscale.TabIndex = 16;
            this.toolTipFFmpeg.SetToolTip(this.nudQscale, "1 being highest quality/largest filesize and 31 being the lowest quality/smallest" +
        " filesize.");
            this.nudQscale.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // comboBoxExtension
            // 
            this.comboBoxExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExtension.FormattingEnabled = true;
            this.comboBoxExtension.Items.AddRange(new object[] {
            "mp4",
            "webm",
            "avi"});
            this.comboBoxExtension.Location = new System.Drawing.Point(304, 9);
            this.comboBoxExtension.Name = "comboBoxExtension";
            this.comboBoxExtension.Size = new System.Drawing.Size(152, 21);
            this.comboBoxExtension.TabIndex = 15;
            // 
            // lblCodec
            // 
            this.lblCodec.AutoSize = true;
            this.lblCodec.Location = new System.Drawing.Point(16, 13);
            this.lblCodec.Name = "lblCodec";
            this.lblCodec.Size = new System.Drawing.Size(41, 13);
            this.lblCodec.TabIndex = 16;
            this.lblCodec.Text = "Codec:";
            // 
            // comboBoxCodec
            // 
            this.comboBoxCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCodec.FormattingEnabled = true;
            this.comboBoxCodec.Location = new System.Drawing.Point(64, 9);
            this.comboBoxCodec.Name = "comboBoxCodec";
            this.comboBoxCodec.Size = new System.Drawing.Size(160, 21);
            this.comboBoxCodec.TabIndex = 17;
            // 
            // comboBoxPreset
            // 
            this.comboBoxPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreset.FormattingEnabled = true;
            this.comboBoxPreset.Location = new System.Drawing.Point(80, 56);
            this.comboBoxPreset.Name = "comboBoxPreset";
            this.comboBoxPreset.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPreset.TabIndex = 19;
            // 
            // lblPreset
            // 
            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(8, 61);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(40, 13);
            this.lblPreset.TabIndex = 18;
            this.lblPreset.Text = "Preset:";
            // 
            // groupBoxH264
            // 
            this.groupBoxH264.Controls.Add(this.nudCRF);
            this.groupBoxH264.Controls.Add(this.lblCRF);
            this.groupBoxH264.Controls.Add(this.lblPreset);
            this.groupBoxH264.Controls.Add(this.comboBoxPreset);
            this.groupBoxH264.Location = new System.Drawing.Point(8, 40);
            this.groupBoxH264.Name = "groupBoxH264";
            this.groupBoxH264.Size = new System.Drawing.Size(216, 96);
            this.groupBoxH264.TabIndex = 22;
            this.groupBoxH264.TabStop = false;
            this.groupBoxH264.Text = "H.264";
            // 
            // groupBoxH263
            // 
            this.groupBoxH263.Controls.Add(this.nudQscale);
            this.groupBoxH263.Controls.Add(this.labelQscale);
            this.groupBoxH263.Location = new System.Drawing.Point(240, 40);
            this.groupBoxH263.Name = "groupBoxH263";
            this.groupBoxH263.Size = new System.Drawing.Size(216, 96);
            this.groupBoxH263.TabIndex = 23;
            this.groupBoxH263.TabStop = false;
            this.groupBoxH263.Text = "XviD";
            // 
            // labelQscale
            // 
            this.labelQscale.AutoSize = true;
            this.labelQscale.Location = new System.Drawing.Point(16, 29);
            this.labelQscale.Name = "labelQscale";
            this.labelQscale.Size = new System.Drawing.Size(41, 13);
            this.labelQscale.TabIndex = 15;
            this.labelQscale.Text = "qscale:";
            // 
            // groupBoxFFmpegExe
            // 
            this.groupBoxFFmpegExe.Controls.Add(this.btnDownload);
            this.groupBoxFFmpegExe.Controls.Add(this.buttonFFmpegBrowse);
            this.groupBoxFFmpegExe.Controls.Add(this.textBoxFFmpegPath);
            this.groupBoxFFmpegExe.Location = new System.Drawing.Point(8, 144);
            this.groupBoxFFmpegExe.Name = "groupBoxFFmpegExe";
            this.groupBoxFFmpegExe.Size = new System.Drawing.Size(448, 56);
            this.groupBoxFFmpegExe.TabIndex = 24;
            this.groupBoxFFmpegExe.TabStop = false;
            this.groupBoxFFmpegExe.Text = "ffmpeg.exe";
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(368, 22);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(72, 24);
            this.btnDownload.TabIndex = 26;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // buttonFFmpegBrowse
            // 
            this.buttonFFmpegBrowse.Location = new System.Drawing.Point(320, 22);
            this.buttonFFmpegBrowse.Name = "buttonFFmpegBrowse";
            this.buttonFFmpegBrowse.Size = new System.Drawing.Size(40, 24);
            this.buttonFFmpegBrowse.TabIndex = 1;
            this.buttonFFmpegBrowse.Text = "...";
            this.buttonFFmpegBrowse.UseVisualStyleBackColor = true;
            this.buttonFFmpegBrowse.Click += new System.EventHandler(this.buttonFFmpegBrowse_Click);
            // 
            // textBoxFFmpegPath
            // 
            this.textBoxFFmpegPath.Location = new System.Drawing.Point(8, 24);
            this.textBoxFFmpegPath.Name = "textBoxFFmpegPath";
            this.textBoxFFmpegPath.Size = new System.Drawing.Size(304, 20);
            this.textBoxFFmpegPath.TabIndex = 0;
            // 
            // groupBoxCommandLinePreview
            // 
            this.groupBoxCommandLinePreview.Controls.Add(this.textBoxCommandLinePreview);
            this.groupBoxCommandLinePreview.Location = new System.Drawing.Point(8, 272);
            this.groupBoxCommandLinePreview.Name = "groupBoxCommandLinePreview";
            this.groupBoxCommandLinePreview.Size = new System.Drawing.Size(448, 96);
            this.groupBoxCommandLinePreview.TabIndex = 25;
            this.groupBoxCommandLinePreview.TabStop = false;
            this.groupBoxCommandLinePreview.Text = "Command line preview";
            // 
            // textBoxCommandLinePreview
            // 
            this.textBoxCommandLinePreview.Location = new System.Drawing.Point(8, 24);
            this.textBoxCommandLinePreview.Multiline = true;
            this.textBoxCommandLinePreview.Name = "textBoxCommandLinePreview";
            this.textBoxCommandLinePreview.ReadOnly = true;
            this.textBoxCommandLinePreview.Size = new System.Drawing.Size(432, 64);
            this.textBoxCommandLinePreview.TabIndex = 0;
            // 
            // gbCommandLineArgs
            // 
            this.gbCommandLineArgs.Controls.Add(this.buttonFFmpegHelp);
            this.gbCommandLineArgs.Controls.Add(this.textBoxUserArgs);
            this.gbCommandLineArgs.Location = new System.Drawing.Point(8, 208);
            this.gbCommandLineArgs.Name = "gbCommandLineArgs";
            this.gbCommandLineArgs.Size = new System.Drawing.Size(448, 56);
            this.gbCommandLineArgs.TabIndex = 25;
            this.gbCommandLineArgs.TabStop = false;
            this.gbCommandLineArgs.Text = "Additional command line arguments";
            // 
            // buttonFFmpegHelp
            // 
            this.buttonFFmpegHelp.Location = new System.Drawing.Point(400, 22);
            this.buttonFFmpegHelp.Name = "buttonFFmpegHelp";
            this.buttonFFmpegHelp.Size = new System.Drawing.Size(40, 24);
            this.buttonFFmpegHelp.TabIndex = 1;
            this.buttonFFmpegHelp.Text = "?";
            this.buttonFFmpegHelp.UseVisualStyleBackColor = true;
            this.buttonFFmpegHelp.Click += new System.EventHandler(this.buttonFFmpegHelp_Click);
            // 
            // textBoxUserArgs
            // 
            this.textBoxUserArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxUserArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxUserArgs.Location = new System.Drawing.Point(8, 24);
            this.textBoxUserArgs.Name = "textBoxUserArgs";
            this.textBoxUserArgs.Size = new System.Drawing.Size(384, 20);
            this.textBoxUserArgs.TabIndex = 0;
            // 
            // FFmpegCLIOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 378);
            this.Controls.Add(this.gbCommandLineArgs);
            this.Controls.Add(this.groupBoxCommandLinePreview);
            this.Controls.Add(this.groupBoxH263);
            this.Controls.Add(this.groupBoxH264);
            this.Controls.Add(this.comboBoxCodec);
            this.Controls.Add(this.lblCodec);
            this.Controls.Add(this.comboBoxExtension);
            this.Controls.Add(this.lblExt);
            this.Controls.Add(this.groupBoxFFmpegExe);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 416);
            this.Name = "FFmpegCLIOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFmpegGUI";
            ((System.ComponentModel.ISupportInitialize)(this.nudCRF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).EndInit();
            this.groupBoxH264.ResumeLayout(false);
            this.groupBoxH264.PerformLayout();
            this.groupBoxH263.ResumeLayout(false);
            this.groupBoxH263.PerformLayout();
            this.groupBoxFFmpegExe.ResumeLayout(false);
            this.groupBoxFFmpegExe.PerformLayout();
            this.groupBoxCommandLinePreview.ResumeLayout(false);
            this.groupBoxCommandLinePreview.PerformLayout();
            this.gbCommandLineArgs.ResumeLayout(false);
            this.gbCommandLineArgs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.Label lblCRF;
        private System.Windows.Forms.NumericUpDown nudCRF;
        private System.Windows.Forms.ToolTip toolTipFFmpeg;
        private System.Windows.Forms.ComboBox comboBoxExtension;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox comboBoxCodec;
        private System.Windows.Forms.ComboBox comboBoxPreset;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.GroupBox groupBoxH264;
        private System.Windows.Forms.GroupBox groupBoxH263;
        private System.Windows.Forms.NumericUpDown nudQscale;
        private System.Windows.Forms.Label labelQscale;
        private System.Windows.Forms.GroupBox groupBoxFFmpegExe;
        private System.Windows.Forms.Button buttonFFmpegBrowse;
        private System.Windows.Forms.TextBox textBoxFFmpegPath;
        private System.Windows.Forms.GroupBox groupBoxCommandLinePreview;
        private System.Windows.Forms.TextBox textBoxCommandLinePreview;
        private System.Windows.Forms.GroupBox gbCommandLineArgs;
        private System.Windows.Forms.Button buttonFFmpegHelp;
        private System.Windows.Forms.TextBox textBoxUserArgs;
        private System.Windows.Forms.Button btnDownload;
    }
}