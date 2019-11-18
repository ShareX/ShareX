namespace ShareX.MediaLib
{
    partial class VideoConverterForm
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
            this.lblInputFilePath = new System.Windows.Forms.Label();
            this.txtInputFilePath = new System.Windows.Forms.TextBox();
            this.btnInputFilePathBrowse = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.btnOutputFolderBrowse = new System.Windows.Forms.Button();
            this.lblOutputFileName = new System.Windows.Forms.Label();
            this.txtOutputFileName = new System.Windows.Forms.TextBox();
            this.lblVideoCodec = new System.Windows.Forms.Label();
            this.cbVideoCodec = new System.Windows.Forms.ComboBox();
            this.lblVideoQuality = new System.Windows.Forms.Label();
            this.nudVideoQuality = new System.Windows.Forms.NumericUpDown();
            this.btnEncode = new System.Windows.Forms.Button();
            this.lblArguments = new System.Windows.Forms.Label();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInputFilePath
            // 
            this.lblInputFilePath.AutoSize = true;
            this.lblInputFilePath.Location = new System.Drawing.Point(13, 16);
            this.lblInputFilePath.Name = "lblInputFilePath";
            this.lblInputFilePath.Size = new System.Drawing.Size(74, 13);
            this.lblInputFilePath.TabIndex = 0;
            this.lblInputFilePath.Text = "Input file path:";
            // 
            // txtInputFilePath
            // 
            this.txtInputFilePath.Location = new System.Drawing.Point(120, 12);
            this.txtInputFilePath.Name = "txtInputFilePath";
            this.txtInputFilePath.Size = new System.Drawing.Size(280, 20);
            this.txtInputFilePath.TabIndex = 1;
            this.txtInputFilePath.TextChanged += new System.EventHandler(this.txtInputFilePath_TextChanged);
            // 
            // btnInputFilePathBrowse
            // 
            this.btnInputFilePathBrowse.Location = new System.Drawing.Point(408, 11);
            this.btnInputFilePathBrowse.Name = "btnInputFilePathBrowse";
            this.btnInputFilePathBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnInputFilePathBrowse.TabIndex = 2;
            this.btnInputFilePathBrowse.Text = "...";
            this.btnInputFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnInputFilePathBrowse.Click += new System.EventHandler(this.btnInputFilePathBrowse_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(120, 36);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(280, 20);
            this.txtOutputFolder.TabIndex = 4;
            this.txtOutputFolder.TextChanged += new System.EventHandler(this.txtOutputFolder_TextChanged);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(13, 40);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(71, 13);
            this.lblOutputFolder.TabIndex = 3;
            this.lblOutputFolder.Text = "Output folder:";
            // 
            // btnOutputFolderBrowse
            // 
            this.btnOutputFolderBrowse.Location = new System.Drawing.Point(408, 35);
            this.btnOutputFolderBrowse.Name = "btnOutputFolderBrowse";
            this.btnOutputFolderBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnOutputFolderBrowse.TabIndex = 5;
            this.btnOutputFolderBrowse.Text = "...";
            this.btnOutputFolderBrowse.UseVisualStyleBackColor = true;
            // 
            // lblOutputFileName
            // 
            this.lblOutputFileName.AutoSize = true;
            this.lblOutputFileName.Location = new System.Drawing.Point(13, 64);
            this.lblOutputFileName.Name = "lblOutputFileName";
            this.lblOutputFileName.Size = new System.Drawing.Size(87, 13);
            this.lblOutputFileName.TabIndex = 6;
            this.lblOutputFileName.Text = "Output file name:";
            // 
            // txtOutputFileName
            // 
            this.txtOutputFileName.Location = new System.Drawing.Point(120, 60);
            this.txtOutputFileName.Name = "txtOutputFileName";
            this.txtOutputFileName.Size = new System.Drawing.Size(280, 20);
            this.txtOutputFileName.TabIndex = 7;
            this.txtOutputFileName.TextChanged += new System.EventHandler(this.txtOutputFileName_TextChanged);
            // 
            // lblVideoCodec
            // 
            this.lblVideoCodec.AutoSize = true;
            this.lblVideoCodec.Location = new System.Drawing.Point(13, 88);
            this.lblVideoCodec.Name = "lblVideoCodec";
            this.lblVideoCodec.Size = new System.Drawing.Size(70, 13);
            this.lblVideoCodec.TabIndex = 8;
            this.lblVideoCodec.Text = "Video codec:";
            // 
            // cbVideoCodec
            // 
            this.cbVideoCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoCodec.FormattingEnabled = true;
            this.cbVideoCodec.Location = new System.Drawing.Point(120, 84);
            this.cbVideoCodec.Name = "cbVideoCodec";
            this.cbVideoCodec.Size = new System.Drawing.Size(120, 21);
            this.cbVideoCodec.TabIndex = 9;
            this.cbVideoCodec.SelectedIndexChanged += new System.EventHandler(this.cbVideoCodec_SelectedIndexChanged);
            // 
            // lblVideoQuality
            // 
            this.lblVideoQuality.AutoSize = true;
            this.lblVideoQuality.Location = new System.Drawing.Point(13, 112);
            this.lblVideoQuality.Name = "lblVideoQuality";
            this.lblVideoQuality.Size = new System.Drawing.Size(70, 13);
            this.lblVideoQuality.TabIndex = 10;
            this.lblVideoQuality.Text = "Video quality:";
            // 
            // nudVideoQuality
            // 
            this.nudVideoQuality.Location = new System.Drawing.Point(120, 108);
            this.nudVideoQuality.Name = "nudVideoQuality";
            this.nudVideoQuality.Size = new System.Drawing.Size(72, 20);
            this.nudVideoQuality.TabIndex = 11;
            this.nudVideoQuality.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudVideoQuality.ValueChanged += new System.EventHandler(this.nudVideoQuality_ValueChanged);
            // 
            // btnEncode
            // 
            this.btnEncode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEncode.Location = new System.Drawing.Point(16, 264);
            this.btnEncode.Name = "btnEncode";
            this.btnEncode.Size = new System.Drawing.Size(424, 32);
            this.btnEncode.TabIndex = 14;
            this.btnEncode.Text = "Start encoding";
            this.btnEncode.UseVisualStyleBackColor = true;
            this.btnEncode.Click += new System.EventHandler(this.btnEncode_Click);
            // 
            // lblArguments
            // 
            this.lblArguments.AutoSize = true;
            this.lblArguments.Location = new System.Drawing.Point(13, 136);
            this.lblArguments.Name = "lblArguments";
            this.lblArguments.Size = new System.Drawing.Size(60, 13);
            this.lblArguments.TabIndex = 12;
            this.lblArguments.Text = "Arguments:";
            // 
            // txtArguments
            // 
            this.txtArguments.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtArguments.Location = new System.Drawing.Point(16, 152);
            this.txtArguments.Multiline = true;
            this.txtArguments.Name = "txtArguments";
            this.txtArguments.ReadOnly = true;
            this.txtArguments.Size = new System.Drawing.Size(424, 104);
            this.txtArguments.TabIndex = 13;
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(16, 264);
            this.pbProgress.MarqueeAnimationSpeed = 50;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(424, 32);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbProgress.TabIndex = 16;
            // 
            // VideoConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(456, 311);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.lblArguments);
            this.Controls.Add(this.nudVideoQuality);
            this.Controls.Add(this.lblVideoQuality);
            this.Controls.Add(this.cbVideoCodec);
            this.Controls.Add(this.lblVideoCodec);
            this.Controls.Add(this.txtOutputFileName);
            this.Controls.Add(this.lblOutputFileName);
            this.Controls.Add(this.btnOutputFolderBrowse);
            this.Controls.Add(this.lblOutputFolder);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.btnInputFilePathBrowse);
            this.Controls.Add(this.txtInputFilePath);
            this.Controls.Add(this.lblInputFilePath);
            this.Controls.Add(this.btnEncode);
            this.Controls.Add(this.pbProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "VideoConverterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Video converter";
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInputFilePath;
        private System.Windows.Forms.TextBox txtInputFilePath;
        private System.Windows.Forms.Button btnInputFilePathBrowse;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.Button btnOutputFolderBrowse;
        private System.Windows.Forms.Label lblOutputFileName;
        private System.Windows.Forms.TextBox txtOutputFileName;
        private System.Windows.Forms.Label lblVideoCodec;
        private System.Windows.Forms.ComboBox cbVideoCodec;
        private System.Windows.Forms.Label lblVideoQuality;
        private System.Windows.Forms.NumericUpDown nudVideoQuality;
        private System.Windows.Forms.Button btnEncode;
        private System.Windows.Forms.Label lblArguments;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}