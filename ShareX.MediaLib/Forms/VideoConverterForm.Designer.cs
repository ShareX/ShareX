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
            this.lblCLI = new System.Windows.Forms.Label();
            this.txtCLI = new System.Windows.Forms.TextBox();
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
            // 
            // btnEncode
            // 
            this.btnEncode.Location = new System.Drawing.Point(16, 264);
            this.btnEncode.Name = "btnEncode";
            this.btnEncode.Size = new System.Drawing.Size(424, 23);
            this.btnEncode.TabIndex = 14;
            this.btnEncode.Text = "Start encoding";
            this.btnEncode.UseVisualStyleBackColor = true;
            this.btnEncode.Click += new System.EventHandler(this.btnEncode_Click);
            // 
            // lblCLI
            // 
            this.lblCLI.AutoSize = true;
            this.lblCLI.Location = new System.Drawing.Point(13, 136);
            this.lblCLI.Name = "lblCLI";
            this.lblCLI.Size = new System.Drawing.Size(26, 13);
            this.lblCLI.TabIndex = 12;
            this.lblCLI.Text = "CLI:";
            // 
            // txtCLI
            // 
            this.txtCLI.Location = new System.Drawing.Point(16, 152);
            this.txtCLI.Multiline = true;
            this.txtCLI.Name = "txtCLI";
            this.txtCLI.Size = new System.Drawing.Size(424, 104);
            this.txtCLI.TabIndex = 13;
            // 
            // VideoConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(456, 298);
            this.Controls.Add(this.txtCLI);
            this.Controls.Add(this.lblCLI);
            this.Controls.Add(this.btnEncode);
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
        private System.Windows.Forms.Label lblCLI;
        private System.Windows.Forms.TextBox txtCLI;
    }
}