namespace ShareX.ImageEffectsLib
{
    partial class ImageEffectPackagerForm
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
            this.btnPackage = new System.Windows.Forms.Button();
            this.lblAssetsFolderPath = new System.Windows.Forms.Label();
            this.lblPackageFilePath = new System.Windows.Forms.Label();
            this.txtPackageFilePath = new System.Windows.Forms.TextBox();
            this.btnPackageFilePathBrowse = new System.Windows.Forms.Button();
            this.txtAssetsFolderPath = new System.Windows.Forms.TextBox();
            this.btnAssetsFolderPathBrowse = new System.Windows.Forms.Button();
            this.btnOpenImageEffectsFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPackage
            // 
            this.btnPackage.Location = new System.Drawing.Point(16, 144);
            this.btnPackage.Name = "btnPackage";
            this.btnPackage.Size = new System.Drawing.Size(384, 24);
            this.btnPackage.TabIndex = 0;
            this.btnPackage.Text = "Package";
            this.btnPackage.UseVisualStyleBackColor = true;
            this.btnPackage.Click += new System.EventHandler(this.btnPackage_Click);
            // 
            // lblAssetsFolderPath
            // 
            this.lblAssetsFolderPath.AutoSize = true;
            this.lblAssetsFolderPath.Location = new System.Drawing.Point(13, 48);
            this.lblAssetsFolderPath.Name = "lblAssetsFolderPath";
            this.lblAssetsFolderPath.Size = new System.Drawing.Size(94, 13);
            this.lblAssetsFolderPath.TabIndex = 2;
            this.lblAssetsFolderPath.Text = "Assets folder path:";
            // 
            // lblPackageFilePath
            // 
            this.lblPackageFilePath.AutoSize = true;
            this.lblPackageFilePath.Location = new System.Drawing.Point(13, 96);
            this.lblPackageFilePath.Name = "lblPackageFilePath";
            this.lblPackageFilePath.Size = new System.Drawing.Size(93, 13);
            this.lblPackageFilePath.TabIndex = 5;
            this.lblPackageFilePath.Text = "Package file path:";
            // 
            // txtPackageFilePath
            // 
            this.txtPackageFilePath.Location = new System.Drawing.Point(16, 112);
            this.txtPackageFilePath.Name = "txtPackageFilePath";
            this.txtPackageFilePath.Size = new System.Drawing.Size(344, 20);
            this.txtPackageFilePath.TabIndex = 6;
            this.txtPackageFilePath.TextChanged += new System.EventHandler(this.txtPackageFilePath_TextChanged);
            // 
            // btnPackageFilePathBrowse
            // 
            this.btnPackageFilePathBrowse.Location = new System.Drawing.Point(368, 111);
            this.btnPackageFilePathBrowse.Name = "btnPackageFilePathBrowse";
            this.btnPackageFilePathBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnPackageFilePathBrowse.TabIndex = 7;
            this.btnPackageFilePathBrowse.Text = "...";
            this.btnPackageFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnPackageFilePathBrowse.Click += new System.EventHandler(this.btnPackageFilePathBrowse_Click);
            // 
            // txtAssetsFolderPath
            // 
            this.txtAssetsFolderPath.Location = new System.Drawing.Point(16, 64);
            this.txtAssetsFolderPath.Name = "txtAssetsFolderPath";
            this.txtAssetsFolderPath.Size = new System.Drawing.Size(344, 20);
            this.txtAssetsFolderPath.TabIndex = 3;
            this.txtAssetsFolderPath.TextChanged += new System.EventHandler(this.txtAssetsFolderPath_TextChanged);
            // 
            // btnAssetsFolderPathBrowse
            // 
            this.btnAssetsFolderPathBrowse.Location = new System.Drawing.Point(368, 63);
            this.btnAssetsFolderPathBrowse.Name = "btnAssetsFolderPathBrowse";
            this.btnAssetsFolderPathBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnAssetsFolderPathBrowse.TabIndex = 4;
            this.btnAssetsFolderPathBrowse.Text = "...";
            this.btnAssetsFolderPathBrowse.UseVisualStyleBackColor = true;
            this.btnAssetsFolderPathBrowse.Click += new System.EventHandler(this.btnAssetsFolderPathBrowse_Click);
            // 
            // btnOpenImageEffectsFolder
            // 
            this.btnOpenImageEffectsFolder.Location = new System.Drawing.Point(16, 16);
            this.btnOpenImageEffectsFolder.Name = "btnOpenImageEffectsFolder";
            this.btnOpenImageEffectsFolder.Size = new System.Drawing.Size(384, 24);
            this.btnOpenImageEffectsFolder.TabIndex = 1;
            this.btnOpenImageEffectsFolder.Text = "Open image effects folder...";
            this.btnOpenImageEffectsFolder.UseVisualStyleBackColor = true;
            this.btnOpenImageEffectsFolder.Click += new System.EventHandler(this.btnOpenImageEffectsFolder_Click);
            // 
            // ImageEffectPackagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(416, 184);
            this.Controls.Add(this.btnOpenImageEffectsFolder);
            this.Controls.Add(this.btnAssetsFolderPathBrowse);
            this.Controls.Add(this.txtAssetsFolderPath);
            this.Controls.Add(this.btnPackageFilePathBrowse);
            this.Controls.Add(this.txtPackageFilePath);
            this.Controls.Add(this.lblPackageFilePath);
            this.Controls.Add(this.lblAssetsFolderPath);
            this.Controls.Add(this.btnPackage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ImageEffectPackagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image effect packager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnPackage;
        private System.Windows.Forms.Label lblAssetsFolderPath;
        private System.Windows.Forms.Label lblPackageFilePath;
        private System.Windows.Forms.TextBox txtPackageFilePath;
        private System.Windows.Forms.Button btnPackageFilePathBrowse;
        private System.Windows.Forms.TextBox txtAssetsFolderPath;
        private System.Windows.Forms.Button btnAssetsFolderPathBrowse;
        private System.Windows.Forms.Button btnOpenImageEffectsFolder;
    }
}