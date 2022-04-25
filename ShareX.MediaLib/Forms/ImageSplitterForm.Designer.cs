namespace ShareX.MediaLib
{
    partial class ImageSplitterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageSplitterForm));
            this.lblImageFilePath = new System.Windows.Forms.Label();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.lblColumnCount = new System.Windows.Forms.Label();
            this.nudRowCount = new System.Windows.Forms.NumericUpDown();
            this.nudColumnCount = new System.Windows.Forms.NumericUpDown();
            this.txtImageFilePath = new System.Windows.Forms.TextBox();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnImageFilePathBrowse = new System.Windows.Forms.Button();
            this.btnOutputFolderBrowse = new System.Windows.Forms.Button();
            this.btnSplitImage = new System.Windows.Forms.Button();
            this.btnCopyChatEmoji = new System.Windows.Forms.Button();
            this.lblColumnRow = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudRowCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumnCount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblImageFilePath
            // 
            resources.ApplyResources(this.lblImageFilePath, "lblImageFilePath");
            this.lblImageFilePath.Name = "lblImageFilePath";
            // 
            // lblRowCount
            // 
            resources.ApplyResources(this.lblRowCount, "lblRowCount");
            this.lblRowCount.Name = "lblRowCount";
            // 
            // lblColumnCount
            // 
            resources.ApplyResources(this.lblColumnCount, "lblColumnCount");
            this.lblColumnCount.Name = "lblColumnCount";
            // 
            // nudRowCount
            // 
            resources.ApplyResources(this.nudRowCount, "nudRowCount");
            this.nudRowCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRowCount.Name = "nudRowCount";
            this.nudRowCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRowCount.ValueChanged += new System.EventHandler(this.nudRowCount_ValueChanged);
            // 
            // nudColumnCount
            // 
            resources.ApplyResources(this.nudColumnCount, "nudColumnCount");
            this.nudColumnCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudColumnCount.Name = "nudColumnCount";
            this.nudColumnCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudColumnCount.ValueChanged += new System.EventHandler(this.nudColumnCount_ValueChanged);
            // 
            // txtImageFilePath
            // 
            resources.ApplyResources(this.txtImageFilePath, "txtImageFilePath");
            this.txtImageFilePath.Name = "txtImageFilePath";
            this.txtImageFilePath.TextChanged += new System.EventHandler(this.txtImageFilePath_TextChanged);
            // 
            // lblOutputFolder
            // 
            resources.ApplyResources(this.lblOutputFolder, "lblOutputFolder");
            this.lblOutputFolder.Name = "lblOutputFolder";
            // 
            // txtOutputFolder
            // 
            resources.ApplyResources(this.txtOutputFolder, "txtOutputFolder");
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.TextChanged += new System.EventHandler(this.txtOutputFolder_TextChanged);
            // 
            // btnImageFilePathBrowse
            // 
            resources.ApplyResources(this.btnImageFilePathBrowse, "btnImageFilePathBrowse");
            this.btnImageFilePathBrowse.Name = "btnImageFilePathBrowse";
            this.btnImageFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnImageFilePathBrowse.Click += new System.EventHandler(this.BtnImageFilePathBrowse_Click);
            // 
            // btnOutputFolderBrowse
            // 
            resources.ApplyResources(this.btnOutputFolderBrowse, "btnOutputFolderBrowse");
            this.btnOutputFolderBrowse.Name = "btnOutputFolderBrowse";
            this.btnOutputFolderBrowse.UseVisualStyleBackColor = true;
            this.btnOutputFolderBrowse.Click += new System.EventHandler(this.BtnOutputFolderBrowse_Click);
            // 
            // btnSplitImage
            // 
            resources.ApplyResources(this.btnSplitImage, "btnSplitImage");
            this.btnSplitImage.Name = "btnSplitImage";
            this.btnSplitImage.UseVisualStyleBackColor = true;
            this.btnSplitImage.Click += new System.EventHandler(this.BtnSplitImage_Click);
            // 
            // btnCopyChatEmoji
            // 
            resources.ApplyResources(this.btnCopyChatEmoji, "btnCopyChatEmoji");
            this.btnCopyChatEmoji.Name = "btnCopyChatEmoji";
            this.btnCopyChatEmoji.UseVisualStyleBackColor = true;
            this.btnCopyChatEmoji.Click += new System.EventHandler(this.btnCopyChatEmoji_Click);
            // 
            // lblColumnRow
            // 
            resources.ApplyResources(this.lblColumnRow, "lblColumnRow");
            this.lblColumnRow.Name = "lblColumnRow";
            // 
            // ImageSplitterForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblColumnRow);
            this.Controls.Add(this.btnCopyChatEmoji);
            this.Controls.Add(this.btnSplitImage);
            this.Controls.Add(this.btnOutputFolderBrowse);
            this.Controls.Add(this.btnImageFilePathBrowse);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.lblOutputFolder);
            this.Controls.Add(this.txtImageFilePath);
            this.Controls.Add(this.nudColumnCount);
            this.Controls.Add(this.nudRowCount);
            this.Controls.Add(this.lblColumnCount);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.lblImageFilePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ImageSplitterForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudRowCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumnCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblImageFilePath;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label lblColumnCount;
        private System.Windows.Forms.NumericUpDown nudRowCount;
        private System.Windows.Forms.NumericUpDown nudColumnCount;
        private System.Windows.Forms.TextBox txtImageFilePath;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnImageFilePathBrowse;
        private System.Windows.Forms.Button btnOutputFolderBrowse;
        private System.Windows.Forms.Button btnSplitImage;
        private System.Windows.Forms.Button btnCopyChatEmoji;
        private System.Windows.Forms.Label lblColumnRow;
    }
}