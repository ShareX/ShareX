namespace ScreenCaptureLib
{
    partial class FFmpegCLIOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FFmpegCLIOptionsForm));
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBoxH264 = new System.Windows.Forms.GroupBox();
            this.groupBoxH263 = new System.Windows.Forms.GroupBox();
            this.labelQscale = new System.Windows.Forms.Label();
            this.groupBoxFFmpegExe = new System.Windows.Forms.GroupBox();
            this.textBoxFFmpegPath = new System.Windows.Forms.TextBox();
            this.buttonFFmpegBrowse = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudCRF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQscale)).BeginInit();
            this.groupBoxH264.SuspendLayout();
            this.groupBoxH263.SuspendLayout();
            this.groupBoxFFmpegExe.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(264, 13);
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
            this.nudCRF.ValueChanged += new System.EventHandler(this.nudCRF_ValueChanged);
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
            this.nudQscale.ValueChanged += new System.EventHandler(this.nudQscale_ValueChanged);
            // 
            // comboBoxExtension
            // 
            this.comboBoxExtension.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxExtension.FormattingEnabled = true;
            this.comboBoxExtension.Items.AddRange(new object[] {
            "mp4",
            "webm",
            "avi"});
            this.comboBoxExtension.Location = new System.Drawing.Point(336, 8);
            this.comboBoxExtension.Name = "comboBoxExtension";
            this.comboBoxExtension.Size = new System.Drawing.Size(121, 21);
            this.comboBoxExtension.TabIndex = 15;
            this.comboBoxExtension.Text = "mp4";
            this.comboBoxExtension.SelectedValueChanged += new System.EventHandler(this.comboBoxExtension_SelectedValueChanged);
            // 
            // lblCodec
            // 
            this.lblCodec.AutoSize = true;
            this.lblCodec.Location = new System.Drawing.Point(24, 13);
            this.lblCodec.Name = "lblCodec";
            this.lblCodec.Size = new System.Drawing.Size(41, 13);
            this.lblCodec.TabIndex = 16;
            this.lblCodec.Text = "Codec:";
            // 
            // comboBoxCodec
            // 
            this.comboBoxCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCodec.FormattingEnabled = true;
            this.comboBoxCodec.Location = new System.Drawing.Point(96, 8);
            this.comboBoxCodec.Name = "comboBoxCodec";
            this.comboBoxCodec.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCodec.TabIndex = 17;
            this.comboBoxCodec.SelectedIndexChanged += new System.EventHandler(this.comboBoxCodec_SelectedIndexChanged);
            // 
            // comboBoxPreset
            // 
            this.comboBoxPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreset.FormattingEnabled = true;
            this.comboBoxPreset.Location = new System.Drawing.Point(80, 56);
            this.comboBoxPreset.Name = "comboBoxPreset";
            this.comboBoxPreset.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPreset.TabIndex = 19;
            this.comboBoxPreset.SelectedIndexChanged += new System.EventHandler(this.comboBoxPreset_SelectedIndexChanged);
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
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(392, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(312, 224);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBoxH264
            // 
            this.groupBoxH264.Controls.Add(this.nudCRF);
            this.groupBoxH264.Controls.Add(this.lblCRF);
            this.groupBoxH264.Controls.Add(this.lblPreset);
            this.groupBoxH264.Controls.Add(this.comboBoxPreset);
            this.groupBoxH264.Location = new System.Drawing.Point(16, 40);
            this.groupBoxH264.Name = "groupBoxH264";
            this.groupBoxH264.Size = new System.Drawing.Size(216, 96);
            this.groupBoxH264.TabIndex = 22;
            this.groupBoxH264.TabStop = false;
            this.groupBoxH264.Text = "H.264 (x264, VP8 etc.)";
            // 
            // groupBoxH263
            // 
            this.groupBoxH263.Controls.Add(this.nudQscale);
            this.groupBoxH263.Controls.Add(this.labelQscale);
            this.groupBoxH263.Location = new System.Drawing.Point(248, 40);
            this.groupBoxH263.Name = "groupBoxH263";
            this.groupBoxH263.Size = new System.Drawing.Size(216, 96);
            this.groupBoxH263.TabIndex = 23;
            this.groupBoxH263.TabStop = false;
            this.groupBoxH263.Text = "H.263 (DivX, XviD etc.)";
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
            this.groupBoxFFmpegExe.Controls.Add(this.buttonFFmpegBrowse);
            this.groupBoxFFmpegExe.Controls.Add(this.textBoxFFmpegPath);
            this.groupBoxFFmpegExe.Location = new System.Drawing.Point(16, 144);
            this.groupBoxFFmpegExe.Name = "groupBoxFFmpegExe";
            this.groupBoxFFmpegExe.Size = new System.Drawing.Size(448, 56);
            this.groupBoxFFmpegExe.TabIndex = 24;
            this.groupBoxFFmpegExe.TabStop = false;
            this.groupBoxFFmpegExe.Text = "ffmpeg.exe";
            // 
            // textBoxFFmpegPath
            // 
            this.textBoxFFmpegPath.Location = new System.Drawing.Point(8, 24);
            this.textBoxFFmpegPath.Name = "textBoxFFmpegPath";
            this.textBoxFFmpegPath.Size = new System.Drawing.Size(392, 20);
            this.textBoxFFmpegPath.TabIndex = 0;
            // 
            // buttonFFmpegBrowse
            // 
            this.buttonFFmpegBrowse.Location = new System.Drawing.Point(408, 24);
            this.buttonFFmpegBrowse.Name = "buttonFFmpegBrowse";
            this.buttonFFmpegBrowse.Size = new System.Drawing.Size(35, 20);
            this.buttonFFmpegBrowse.TabIndex = 1;
            this.buttonFFmpegBrowse.Text = "...";
            this.buttonFFmpegBrowse.UseVisualStyleBackColor = true;
            this.buttonFFmpegBrowse.Click += new System.EventHandler(this.buttonFFmpegBrowse_Click);
            // 
            // FFmpegCLIOptionsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 262);
            this.Controls.Add(this.groupBoxFFmpegExe);
            this.Controls.Add(this.groupBoxH263);
            this.Controls.Add(this.groupBoxH264);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboBoxCodec);
            this.Controls.Add(this.lblCodec);
            this.Controls.Add(this.comboBoxExtension);
            this.Controls.Add(this.lblExt);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(496, 300);
            this.Name = "FFmpegCLIOptionsForm";
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBoxH264;
        private System.Windows.Forms.GroupBox groupBoxH263;
        private System.Windows.Forms.NumericUpDown nudQscale;
        private System.Windows.Forms.Label labelQscale;
        private System.Windows.Forms.GroupBox groupBoxFFmpegExe;
        private System.Windows.Forms.Button buttonFFmpegBrowse;
        private System.Windows.Forms.TextBox textBoxFFmpegPath;
    }
}