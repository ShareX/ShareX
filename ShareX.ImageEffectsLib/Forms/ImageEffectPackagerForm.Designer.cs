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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEffectPackagerForm));
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
            resources.ApplyResources(this.btnPackage, "btnPackage");
            this.btnPackage.Name = "btnPackage";
            this.btnPackage.UseVisualStyleBackColor = true;
            this.btnPackage.Click += new System.EventHandler(this.btnPackage_Click);
            // 
            // lblAssetsFolderPath
            // 
            resources.ApplyResources(this.lblAssetsFolderPath, "lblAssetsFolderPath");
            this.lblAssetsFolderPath.Name = "lblAssetsFolderPath";
            // 
            // lblPackageFilePath
            // 
            resources.ApplyResources(this.lblPackageFilePath, "lblPackageFilePath");
            this.lblPackageFilePath.Name = "lblPackageFilePath";
            // 
            // txtPackageFilePath
            // 
            resources.ApplyResources(this.txtPackageFilePath, "txtPackageFilePath");
            this.txtPackageFilePath.Name = "txtPackageFilePath";
            this.txtPackageFilePath.TextChanged += new System.EventHandler(this.txtPackageFilePath_TextChanged);
            // 
            // btnPackageFilePathBrowse
            // 
            resources.ApplyResources(this.btnPackageFilePathBrowse, "btnPackageFilePathBrowse");
            this.btnPackageFilePathBrowse.Name = "btnPackageFilePathBrowse";
            this.btnPackageFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnPackageFilePathBrowse.Click += new System.EventHandler(this.btnPackageFilePathBrowse_Click);
            // 
            // txtAssetsFolderPath
            // 
            resources.ApplyResources(this.txtAssetsFolderPath, "txtAssetsFolderPath");
            this.txtAssetsFolderPath.Name = "txtAssetsFolderPath";
            this.txtAssetsFolderPath.TextChanged += new System.EventHandler(this.txtAssetsFolderPath_TextChanged);
            // 
            // btnAssetsFolderPathBrowse
            // 
            resources.ApplyResources(this.btnAssetsFolderPathBrowse, "btnAssetsFolderPathBrowse");
            this.btnAssetsFolderPathBrowse.Name = "btnAssetsFolderPathBrowse";
            this.btnAssetsFolderPathBrowse.UseVisualStyleBackColor = true;
            this.btnAssetsFolderPathBrowse.Click += new System.EventHandler(this.btnAssetsFolderPathBrowse_Click);
            // 
            // btnOpenImageEffectsFolder
            // 
            resources.ApplyResources(this.btnOpenImageEffectsFolder, "btnOpenImageEffectsFolder");
            this.btnOpenImageEffectsFolder.Name = "btnOpenImageEffectsFolder";
            this.btnOpenImageEffectsFolder.UseVisualStyleBackColor = true;
            this.btnOpenImageEffectsFolder.Click += new System.EventHandler(this.btnOpenImageEffectsFolder_Click);
            // 
            // ImageEffectPackagerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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