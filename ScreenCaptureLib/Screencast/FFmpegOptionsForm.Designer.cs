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
            this.tpFFmpeg = new System.Windows.Forms.ToolTip(this.components);
            this.nudQscale = new System.Windows.Forms.NumericUpDown();
            this.nudVPxCRF = new System.Windows.Forms.NumericUpDown();
            this.cbExtension = new System.Windows.Forms.ComboBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.cbCodec = new System.Windows.Forms.ComboBox();
            this.cbPreset = new System.Windows.Forms.ComboBox();
            this.lblPreset = new System.Windows.Forms.Label();
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
            this.tcFFmpeg = new System.Windows.Forms.TabControl();
            this.tpX264 = new System.Windows.Forms.TabPage();
            this.tpVpx = new System.Windows.Forms.TabPage();
            this.lblVpxCRF = new System.Windows.Forms.Label();
            this.tpXvid = new System.Windows.Forms.TabPage();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnCopyPreview = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVPxCRF)).BeginInit();
            this.gbFFmpegExe.SuspendLayout();
            this.gbCommandLinePreview.SuspendLayout();
            this.gbCommandLineArgs.SuspendLayout();
            this.tcFFmpeg.SuspendLayout();
            this.tpX264.SuspendLayout();
            this.tpVpx.SuspendLayout();
            this.tpXvid.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(192, 16);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(56, 13);
            this.lblExt.TabIndex = 11;
            this.lblExt.Text = "Extension:";
            // 
            // lblX264CRF
            // 
            this.lblX264CRF.AutoSize = true;
            this.lblX264CRF.Location = new System.Drawing.Point(16, 16);
            this.lblX264CRF.Name = "lblX264CRF";
            this.lblX264CRF.Size = new System.Drawing.Size(31, 13);
            this.lblX264CRF.TabIndex = 13;
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
            this.nudx264CRF.TabIndex = 14;
            this.nudx264CRF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tpFFmpeg.SetToolTip(this.nudx264CRF, resources.GetString("nudx264CRF.ToolTip"));
            this.nudx264CRF.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
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
            this.nudQscale.TabIndex = 16;
            this.nudQscale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tpFFmpeg.SetToolTip(this.nudQscale, "1 being highest quality/largest filesize and 31 being the lowest quality/smallest" +
        " filesize.");
            this.nudQscale.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // nudVPxCRF
            // 
            this.nudVPxCRF.Location = new System.Drawing.Point(56, 12);
            this.nudVPxCRF.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudVPxCRF.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudVPxCRF.Name = "nudVPxCRF";
            this.nudVPxCRF.Size = new System.Drawing.Size(48, 20);
            this.nudVPxCRF.TabIndex = 16;
            this.nudVPxCRF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tpFFmpeg.SetToolTip(this.nudVPxCRF, resources.GetString("nudVPxCRF.ToolTip"));
            this.nudVPxCRF.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // cbExtension
            // 
            this.cbExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExtension.FormattingEnabled = true;
            this.cbExtension.Location = new System.Drawing.Point(256, 12);
            this.cbExtension.Name = "cbExtension";
            this.cbExtension.Size = new System.Drawing.Size(72, 21);
            this.cbExtension.TabIndex = 15;
            // 
            // lblCodec
            // 
            this.lblCodec.AutoSize = true;
            this.lblCodec.Location = new System.Drawing.Point(8, 16);
            this.lblCodec.Name = "lblCodec";
            this.lblCodec.Size = new System.Drawing.Size(41, 13);
            this.lblCodec.TabIndex = 16;
            this.lblCodec.Text = "Codec:";
            // 
            // cbCodec
            // 
            this.cbCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCodec.FormattingEnabled = true;
            this.cbCodec.Location = new System.Drawing.Point(56, 12);
            this.cbCodec.Name = "cbCodec";
            this.cbCodec.Size = new System.Drawing.Size(120, 21);
            this.cbCodec.TabIndex = 17;
            // 
            // cbPreset
            // 
            this.cbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPreset.FormattingEnabled = true;
            this.cbPreset.Location = new System.Drawing.Point(168, 12);
            this.cbPreset.Name = "cbPreset";
            this.cbPreset.Size = new System.Drawing.Size(121, 21);
            this.cbPreset.TabIndex = 19;
            // 
            // lblPreset
            // 
            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(120, 16);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(40, 13);
            this.lblPreset.TabIndex = 18;
            this.lblPreset.Text = "Preset:";
            // 
            // lblQscale
            // 
            this.lblQscale.AutoSize = true;
            this.lblQscale.Location = new System.Drawing.Point(16, 16);
            this.lblQscale.Name = "lblQscale";
            this.lblQscale.Size = new System.Drawing.Size(83, 13);
            this.lblQscale.TabIndex = 15;
            this.lblQscale.Text = "Variable bit rate:";
            // 
            // gbFFmpegExe
            // 
            this.gbFFmpegExe.Controls.Add(this.btnDownload);
            this.gbFFmpegExe.Controls.Add(this.btnFFmpegBrowse);
            this.gbFFmpegExe.Controls.Add(this.tbFFmpegPath);
            this.gbFFmpegExe.Location = new System.Drawing.Point(8, 120);
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
            this.gbCommandLinePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCommandLinePreview.Controls.Add(this.tbCommandLinePreview);
            this.gbCommandLinePreview.Location = new System.Drawing.Point(8, 248);
            this.gbCommandLinePreview.Name = "gbCommandLinePreview";
            this.gbCommandLinePreview.Padding = new System.Windows.Forms.Padding(8);
            this.gbCommandLinePreview.Size = new System.Drawing.Size(448, 104);
            this.gbCommandLinePreview.TabIndex = 25;
            this.gbCommandLinePreview.TabStop = false;
            this.gbCommandLinePreview.Text = "Command line preview";
            // 
            // tbCommandLinePreview
            // 
            this.tbCommandLinePreview.BackColor = System.Drawing.Color.White;
            this.tbCommandLinePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCommandLinePreview.Location = new System.Drawing.Point(8, 21);
            this.tbCommandLinePreview.Multiline = true;
            this.tbCommandLinePreview.Name = "tbCommandLinePreview";
            this.tbCommandLinePreview.ReadOnly = true;
            this.tbCommandLinePreview.Size = new System.Drawing.Size(432, 75);
            this.tbCommandLinePreview.TabIndex = 0;
            // 
            // gbCommandLineArgs
            // 
            this.gbCommandLineArgs.Controls.Add(this.btnFFmpegHelp);
            this.gbCommandLineArgs.Controls.Add(this.tbUserArgs);
            this.gbCommandLineArgs.Location = new System.Drawing.Point(8, 184);
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
            // tcFFmpeg
            // 
            this.tcFFmpeg.Controls.Add(this.tpX264);
            this.tcFFmpeg.Controls.Add(this.tpVpx);
            this.tcFFmpeg.Controls.Add(this.tpXvid);
            this.tcFFmpeg.Location = new System.Drawing.Point(8, 40);
            this.tcFFmpeg.Name = "tcFFmpeg";
            this.tcFFmpeg.SelectedIndex = 0;
            this.tcFFmpeg.Size = new System.Drawing.Size(448, 72);
            this.tcFFmpeg.TabIndex = 26;
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
            this.tpX264.Size = new System.Drawing.Size(440, 46);
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
            this.tpVpx.Size = new System.Drawing.Size(440, 46);
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
            this.lblVpxCRF.TabIndex = 15;
            this.lblVpxCRF.Text = "CRF:";
            // 
            // tpXvid
            // 
            this.tpXvid.Controls.Add(this.nudQscale);
            this.tpXvid.Controls.Add(this.lblQscale);
            this.tpXvid.Location = new System.Drawing.Point(4, 22);
            this.tpXvid.Name = "tpXvid";
            this.tpXvid.Size = new System.Drawing.Size(440, 46);
            this.tpXvid.TabIndex = 3;
            this.tpXvid.Text = "XviD";
            this.tpXvid.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(136, 243);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(88, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test with CMD";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCopyPreview
            // 
            this.btnCopyPreview.Location = new System.Drawing.Point(227, 243);
            this.btnCopyPreview.Name = "btnCopyPreview";
            this.btnCopyPreview.Size = new System.Drawing.Size(53, 23);
            this.btnCopyPreview.TabIndex = 1;
            this.btnCopyPreview.Text = "Copy";
            this.btnCopyPreview.UseVisualStyleBackColor = true;
            this.btnCopyPreview.Click += new System.EventHandler(this.btnCopyPreview_Click);
            // 
            // FFmpegOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(464, 360);
            this.Controls.Add(this.btnCopyPreview);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.cbCodec);
            this.Controls.Add(this.tcFFmpeg);
            this.Controls.Add(this.lblCodec);
            this.Controls.Add(this.cbExtension);
            this.Controls.Add(this.gbCommandLinePreview);
            this.Controls.Add(this.lblExt);
            this.Controls.Add(this.gbFFmpegExe);
            this.Controls.Add(this.gbCommandLineArgs);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FFmpegOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFmpegGUI";
            ((System.ComponentModel.ISupportInitialize)(this.nudx264CRF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVPxCRF)).EndInit();
            this.gbFFmpegExe.ResumeLayout(false);
            this.gbFFmpegExe.PerformLayout();
            this.gbCommandLinePreview.ResumeLayout(false);
            this.gbCommandLinePreview.PerformLayout();
            this.gbCommandLineArgs.ResumeLayout(false);
            this.gbCommandLineArgs.PerformLayout();
            this.tcFFmpeg.ResumeLayout(false);
            this.tpX264.ResumeLayout(false);
            this.tpX264.PerformLayout();
            this.tpVpx.ResumeLayout(false);
            this.tpVpx.PerformLayout();
            this.tpXvid.ResumeLayout(false);
            this.tpXvid.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.Label lblX264CRF;
        private System.Windows.Forms.NumericUpDown nudx264CRF;
        private System.Windows.Forms.ToolTip tpFFmpeg;
        private System.Windows.Forms.ComboBox cbExtension;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox cbCodec;
        private System.Windows.Forms.ComboBox cbPreset;
        private System.Windows.Forms.Label lblPreset;
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
        private System.Windows.Forms.TabControl tcFFmpeg;
        private System.Windows.Forms.TabPage tpX264;
        private System.Windows.Forms.TabPage tpVpx;
        private System.Windows.Forms.TabPage tpXvid;
        private System.Windows.Forms.NumericUpDown nudVPxCRF;
        private System.Windows.Forms.Label lblVpxCRF;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnCopyPreview;
    }
}