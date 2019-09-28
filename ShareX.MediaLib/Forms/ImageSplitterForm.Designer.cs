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
            ((System.ComponentModel.ISupportInitialize)(this.nudRowCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumnCount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblImageFilePath
            // 
            this.lblImageFilePath.AutoSize = true;
            this.lblImageFilePath.Location = new System.Drawing.Point(13, 16);
            this.lblImageFilePath.Name = "lblImageFilePath";
            this.lblImageFilePath.Size = new System.Drawing.Size(79, 13);
            this.lblImageFilePath.TabIndex = 0;
            this.lblImageFilePath.Text = "Image file path:";
            // 
            // lblRowCount
            // 
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Location = new System.Drawing.Point(13, 40);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(62, 13);
            this.lblRowCount.TabIndex = 1;
            this.lblRowCount.Text = "Row count:";
            // 
            // lblColumnCount
            // 
            this.lblColumnCount.AutoSize = true;
            this.lblColumnCount.Location = new System.Drawing.Point(13, 64);
            this.lblColumnCount.Name = "lblColumnCount";
            this.lblColumnCount.Size = new System.Drawing.Size(75, 13);
            this.lblColumnCount.TabIndex = 2;
            this.lblColumnCount.Text = "Column count:";
            // 
            // nudRowCount
            // 
            this.nudRowCount.Location = new System.Drawing.Point(120, 36);
            this.nudRowCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRowCount.Name = "nudRowCount";
            this.nudRowCount.Size = new System.Drawing.Size(56, 20);
            this.nudRowCount.TabIndex = 3;
            this.nudRowCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudRowCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudColumnCount
            // 
            this.nudColumnCount.Location = new System.Drawing.Point(120, 60);
            this.nudColumnCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudColumnCount.Name = "nudColumnCount";
            this.nudColumnCount.Size = new System.Drawing.Size(56, 20);
            this.nudColumnCount.TabIndex = 4;
            this.nudColumnCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudColumnCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtImageFilePath
            // 
            this.txtImageFilePath.Location = new System.Drawing.Point(120, 12);
            this.txtImageFilePath.Name = "txtImageFilePath";
            this.txtImageFilePath.Size = new System.Drawing.Size(280, 20);
            this.txtImageFilePath.TabIndex = 5;
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(13, 88);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(71, 13);
            this.lblOutputFolder.TabIndex = 6;
            this.lblOutputFolder.Text = "Output folder:";
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(120, 84);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(280, 20);
            this.txtOutputFolder.TabIndex = 7;
            // 
            // btnImageFilePathBrowse
            // 
            this.btnImageFilePathBrowse.Location = new System.Drawing.Point(408, 11);
            this.btnImageFilePathBrowse.Name = "btnImageFilePathBrowse";
            this.btnImageFilePathBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnImageFilePathBrowse.TabIndex = 8;
            this.btnImageFilePathBrowse.Text = "...";
            this.btnImageFilePathBrowse.UseVisualStyleBackColor = true;
            // 
            // btnOutputFolderBrowse
            // 
            this.btnOutputFolderBrowse.Location = new System.Drawing.Point(408, 83);
            this.btnOutputFolderBrowse.Name = "btnOutputFolderBrowse";
            this.btnOutputFolderBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnOutputFolderBrowse.TabIndex = 9;
            this.btnOutputFolderBrowse.Text = "...";
            this.btnOutputFolderBrowse.UseVisualStyleBackColor = true;
            // 
            // btnSplitImage
            // 
            this.btnSplitImage.Location = new System.Drawing.Point(16, 112);
            this.btnSplitImage.Name = "btnSplitImage";
            this.btnSplitImage.Size = new System.Drawing.Size(424, 24);
            this.btnSplitImage.TabIndex = 10;
            this.btnSplitImage.Text = "Split image";
            this.btnSplitImage.UseVisualStyleBackColor = true;
            this.btnSplitImage.Click += new System.EventHandler(this.BtnSplitImage_Click);
            // 
            // ImageSplitterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(449, 145);
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
            this.Name = "ImageSplitterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image splitter";
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
    }
}