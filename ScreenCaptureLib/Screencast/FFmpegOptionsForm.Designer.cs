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
            this.tpFFmpeg = new System.Windows.Forms.ToolTip(this.components);
            this.nudQscale = new System.Windows.Forms.NumericUpDown();
            this.cbExtension = new System.Windows.Forms.ComboBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.cbCodec = new System.Windows.Forms.ComboBox();
            this.cbPreset = new System.Windows.Forms.ComboBox();
            this.lblPreset = new System.Windows.Forms.Label();
            this.gbH264 = new System.Windows.Forms.GroupBox();
            this.gbH263 = new System.Windows.Forms.GroupBox();
            this.lblQscale = new System.Windows.Forms.Label();
            this.gbFFmpegExe = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnFFmpegBrowse = new System.Windows.Forms.Button();
            this.tbFFmpegPath = new System.Windows.Forms.TextBox();
            this.gbCommandLinePreview = new System.Windows.Forms.GroupBox();
            this.tbCommandLinePreview = new System.Windows.Forms.TextBox();
            this.gbCommandLineArgs = new System.Windows.Forms.GroupBox();
            this.btnFFmpegHelp = new System.Windows.Forms.Button();
            this.tbUserArgs = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudCRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).BeginInit();
            this.gbH264.SuspendLayout();
            this.gbH263.SuspendLayout();
            this.gbFFmpegExe.SuspendLayout();
            this.gbCommandLinePreview.SuspendLayout();
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
            this.lblCRF.Location = new System.Drawing.Point(16, 24);
            this.lblCRF.Name = "lblCRF";
            this.lblCRF.Size = new System.Drawing.Size(31, 13);
            this.lblCRF.TabIndex = 13;
            this.lblCRF.Text = "CRF:";
            // 
            // nudCRF
            // 
            this.nudCRF.Location = new System.Drawing.Point(72, 20);
            this.nudCRF.Maximum = new decimal(new int[] {
            51,
            0,
            0,
            0});
            this.nudCRF.Name = "nudCRF";
            this.nudCRF.Size = new System.Drawing.Size(121, 20);
            this.nudCRF.TabIndex = 14;
            this.tpFFmpeg.SetToolTip(this.nudCRF, resources.GetString("nudCRF.ToolTip"));
            this.nudCRF.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            // 
            // nudQscale
            // 
            this.nudQscale.Location = new System.Drawing.Point(72, 20);
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
            this.tpFFmpeg.SetToolTip(this.nudQscale, "1 being highest quality/largest filesize and 31 being the lowest quality/smallest" +
        " filesize.");
            this.nudQscale.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cbExtension
            // 
            this.cbExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExtension.FormattingEnabled = true;
            this.cbExtension.Items.AddRange(new object[] {
            "mp4",
            "webm",
            "avi"});
            this.cbExtension.Location = new System.Drawing.Point(304, 9);
            this.cbExtension.Name = "cbExtension";
            this.cbExtension.Size = new System.Drawing.Size(152, 21);
            this.cbExtension.TabIndex = 15;
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
            // cbCodec
            // 
            this.cbCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCodec.FormattingEnabled = true;
            this.cbCodec.Location = new System.Drawing.Point(64, 9);
            this.cbCodec.Name = "cbCodec";
            this.cbCodec.Size = new System.Drawing.Size(160, 21);
            this.cbCodec.TabIndex = 17;
            // 
            // cbPreset
            // 
            this.cbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPreset.FormattingEnabled = true;
            this.cbPreset.Location = new System.Drawing.Point(72, 52);
            this.cbPreset.Name = "cbPreset";
            this.cbPreset.Size = new System.Drawing.Size(121, 21);
            this.cbPreset.TabIndex = 19;
            // 
            // lblPreset
            // 
            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(16, 56);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(40, 13);
            this.lblPreset.TabIndex = 18;
            this.lblPreset.Text = "Preset:";
            // 
            // gbH264
            // 
            this.gbH264.Controls.Add(this.nudCRF);
            this.gbH264.Controls.Add(this.lblCRF);
            this.gbH264.Controls.Add(this.lblPreset);
            this.gbH264.Controls.Add(this.cbPreset);
            this.gbH264.Location = new System.Drawing.Point(8, 40);
            this.gbH264.Name = "gbH264";
            this.gbH264.Size = new System.Drawing.Size(448, 88);
            this.gbH264.TabIndex = 22;
            this.gbH264.TabStop = false;
            this.gbH264.Text = "H.264";
            // 
            // gbH263
            // 
            this.gbH263.Controls.Add(this.nudQscale);
            this.gbH263.Controls.Add(this.lblQscale);
            this.gbH263.Location = new System.Drawing.Point(8, 40);
            this.gbH263.Name = "gbH263";
            this.gbH263.Size = new System.Drawing.Size(448, 88);
            this.gbH263.TabIndex = 23;
            this.gbH263.TabStop = false;
            this.gbH263.Text = "XviD";
            // 
            // lblQscale
            // 
            this.lblQscale.AutoSize = true;
            this.lblQscale.Location = new System.Drawing.Point(16, 24);
            this.lblQscale.Name = "lblQscale";
            this.lblQscale.Size = new System.Drawing.Size(41, 13);
            this.lblQscale.TabIndex = 15;
            this.lblQscale.Text = "qscale:";
            // 
            // gbFFmpegExe
            // 
            this.gbFFmpegExe.Controls.Add(this.btnDownload);
            this.gbFFmpegExe.Controls.Add(this.btnFFmpegBrowse);
            this.gbFFmpegExe.Controls.Add(this.tbFFmpegPath);
            this.gbFFmpegExe.Location = new System.Drawing.Point(8, 136);
            this.gbFFmpegExe.Name = "gbFFmpegExe";
            this.gbFFmpegExe.Size = new System.Drawing.Size(448, 56);
            this.gbFFmpegExe.TabIndex = 24;
            this.gbFFmpegExe.TabStop = false;
            this.gbFFmpegExe.Text = "ffmpeg.exe";
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
            // btnFFmpegBrowse
            // 
            this.btnFFmpegBrowse.Location = new System.Drawing.Point(320, 22);
            this.btnFFmpegBrowse.Name = "btnFFmpegBrowse";
            this.btnFFmpegBrowse.Size = new System.Drawing.Size(40, 24);
            this.btnFFmpegBrowse.TabIndex = 1;
            this.btnFFmpegBrowse.Text = "...";
            this.btnFFmpegBrowse.UseVisualStyleBackColor = true;
            this.btnFFmpegBrowse.Click += new System.EventHandler(this.buttonFFmpegBrowse_Click);
            // 
            // tbFFmpegPath
            // 
            this.tbFFmpegPath.Location = new System.Drawing.Point(8, 24);
            this.tbFFmpegPath.Name = "tbFFmpegPath";
            this.tbFFmpegPath.Size = new System.Drawing.Size(304, 20);
            this.tbFFmpegPath.TabIndex = 0;
            // 
            // gbCommandLinePreview
            // 
            this.gbCommandLinePreview.Controls.Add(this.tbCommandLinePreview);
            this.gbCommandLinePreview.Location = new System.Drawing.Point(8, 264);
            this.gbCommandLinePreview.Name = "gbCommandLinePreview";
            this.gbCommandLinePreview.Size = new System.Drawing.Size(448, 104);
            this.gbCommandLinePreview.TabIndex = 25;
            this.gbCommandLinePreview.TabStop = false;
            this.gbCommandLinePreview.Text = "Command line preview";
            // 
            // tbCommandLinePreview
            // 
            this.tbCommandLinePreview.Location = new System.Drawing.Point(8, 24);
            this.tbCommandLinePreview.Multiline = true;
            this.tbCommandLinePreview.Name = "tbCommandLinePreview";
            this.tbCommandLinePreview.ReadOnly = true;
            this.tbCommandLinePreview.Size = new System.Drawing.Size(432, 72);
            this.tbCommandLinePreview.TabIndex = 0;
            // 
            // gbCommandLineArgs
            // 
            this.gbCommandLineArgs.Controls.Add(this.btnFFmpegHelp);
            this.gbCommandLineArgs.Controls.Add(this.tbUserArgs);
            this.gbCommandLineArgs.Location = new System.Drawing.Point(8, 200);
            this.gbCommandLineArgs.Name = "gbCommandLineArgs";
            this.gbCommandLineArgs.Size = new System.Drawing.Size(448, 56);
            this.gbCommandLineArgs.TabIndex = 25;
            this.gbCommandLineArgs.TabStop = false;
            this.gbCommandLineArgs.Text = "Additional command line arguments";
            // 
            // btnFFmpegHelp
            // 
            this.btnFFmpegHelp.Location = new System.Drawing.Point(400, 22);
            this.btnFFmpegHelp.Name = "btnFFmpegHelp";
            this.btnFFmpegHelp.Size = new System.Drawing.Size(40, 24);
            this.btnFFmpegHelp.TabIndex = 1;
            this.btnFFmpegHelp.Text = "?";
            this.btnFFmpegHelp.UseVisualStyleBackColor = true;
            this.btnFFmpegHelp.Click += new System.EventHandler(this.buttonFFmpegHelp_Click);
            // 
            // tbUserArgs
            // 
            this.tbUserArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbUserArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbUserArgs.Location = new System.Drawing.Point(8, 24);
            this.tbUserArgs.Name = "tbUserArgs";
            this.tbUserArgs.Size = new System.Drawing.Size(384, 20);
            this.tbUserArgs.TabIndex = 0;
            // 
            // FFmpegOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 378);
            this.Controls.Add(this.gbH263);
            this.Controls.Add(this.gbCommandLineArgs);
            this.Controls.Add(this.gbCommandLinePreview);
            this.Controls.Add(this.gbH264);
            this.Controls.Add(this.cbCodec);
            this.Controls.Add(this.lblCodec);
            this.Controls.Add(this.cbExtension);
            this.Controls.Add(this.lblExt);
            this.Controls.Add(this.gbFFmpegExe);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 416);
            this.Name = "FFmpegOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFmpegGUI";
            ((System.ComponentModel.ISupportInitialize)(this.nudCRF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).EndInit();
            this.gbH264.ResumeLayout(false);
            this.gbH264.PerformLayout();
            this.gbH263.ResumeLayout(false);
            this.gbH263.PerformLayout();
            this.gbFFmpegExe.ResumeLayout(false);
            this.gbFFmpegExe.PerformLayout();
            this.gbCommandLinePreview.ResumeLayout(false);
            this.gbCommandLinePreview.PerformLayout();
            this.gbCommandLineArgs.ResumeLayout(false);
            this.gbCommandLineArgs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.Label lblCRF;
        private System.Windows.Forms.NumericUpDown nudCRF;
        private System.Windows.Forms.ToolTip tpFFmpeg;
        private System.Windows.Forms.ComboBox cbExtension;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox cbCodec;
        private System.Windows.Forms.ComboBox cbPreset;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.GroupBox gbH264;
        private System.Windows.Forms.GroupBox gbH263;
        private System.Windows.Forms.NumericUpDown nudQscale;
        private System.Windows.Forms.Label lblQscale;
        private System.Windows.Forms.GroupBox gbFFmpegExe;
        private System.Windows.Forms.Button btnFFmpegBrowse;
        private System.Windows.Forms.TextBox tbFFmpegPath;
        private System.Windows.Forms.GroupBox gbCommandLinePreview;
        private System.Windows.Forms.TextBox tbCommandLinePreview;
        private System.Windows.Forms.GroupBox gbCommandLineArgs;
        private System.Windows.Forms.Button btnFFmpegHelp;
        private System.Windows.Forms.TextBox tbUserArgs;
        private System.Windows.Forms.Button btnDownload;
    }
}