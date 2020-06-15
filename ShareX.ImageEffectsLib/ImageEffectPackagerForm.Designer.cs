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
            this.lblAssetsFolder = new System.Windows.Forms.Label();
            this.txtAssetsFolder = new System.Windows.Forms.TextBox();
            this.btnPackage = new System.Windows.Forms.Button();
            this.btnAssetsFolderBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAssetsFolder
            // 
            this.lblAssetsFolder.AutoSize = true;
            this.lblAssetsFolder.Location = new System.Drawing.Point(13, 16);
            this.lblAssetsFolder.Name = "lblAssetsFolder";
            this.lblAssetsFolder.Size = new System.Drawing.Size(118, 13);
            this.lblAssetsFolder.TabIndex = 0;
            this.lblAssetsFolder.Text = "Assets folder (Optional):";
            // 
            // txtAssetsFolder
            // 
            this.txtAssetsFolder.Location = new System.Drawing.Point(16, 32);
            this.txtAssetsFolder.Name = "txtAssetsFolder";
            this.txtAssetsFolder.Size = new System.Drawing.Size(336, 20);
            this.txtAssetsFolder.TabIndex = 1;
            this.txtAssetsFolder.TextChanged += new System.EventHandler(this.txtAssetsFolder_TextChanged);
            // 
            // btnPackage
            // 
            this.btnPackage.Location = new System.Drawing.Point(16, 64);
            this.btnPackage.Name = "btnPackage";
            this.btnPackage.Size = new System.Drawing.Size(128, 23);
            this.btnPackage.TabIndex = 2;
            this.btnPackage.Text = "Package...";
            this.btnPackage.UseVisualStyleBackColor = true;
            this.btnPackage.Click += new System.EventHandler(this.btnPackage_Click);
            // 
            // btnAssetsFolderBrowse
            // 
            this.btnAssetsFolderBrowse.Location = new System.Drawing.Point(360, 31);
            this.btnAssetsFolderBrowse.Name = "btnAssetsFolderBrowse";
            this.btnAssetsFolderBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnAssetsFolderBrowse.TabIndex = 3;
            this.btnAssetsFolderBrowse.Text = "...";
            this.btnAssetsFolderBrowse.UseVisualStyleBackColor = true;
            this.btnAssetsFolderBrowse.Click += new System.EventHandler(this.btnAssetsFolderBrowse_Click);
            // 
            // ImageEffectPackagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 98);
            this.Controls.Add(this.btnAssetsFolderBrowse);
            this.Controls.Add(this.btnPackage);
            this.Controls.Add(this.txtAssetsFolder);
            this.Controls.Add(this.lblAssetsFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ImageEffectPackagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image effect packager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAssetsFolder;
        private System.Windows.Forms.TextBox txtAssetsFolder;
        private System.Windows.Forms.Button btnPackage;
        private System.Windows.Forms.Button btnAssetsFolderBrowse;
    }
}