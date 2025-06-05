namespace ShareX.HistoryLib.Forms
{
    partial class MediaImporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaImporter));
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseFolderPath = new System.Windows.Forms.Button();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.pbImportProgress = new System.Windows.Forms.ProgressBar();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtFolderPath
            // 
            resources.ApplyResources(this.txtFolderPath, "txtFolderPath");
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.ReadOnly = true;
            // 
            // btnBrowseFolderPath
            // 
            resources.ApplyResources(this.btnBrowseFolderPath, "btnBrowseFolderPath");
            this.btnBrowseFolderPath.Name = "btnBrowseFolderPath";
            this.btnBrowseFolderPath.UseVisualStyleBackColor = true;
            this.btnBrowseFolderPath.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblFolderPath
            // 
            resources.ApplyResources(this.lblFolderPath, "lblFolderPath");
            this.lblFolderPath.Name = "lblFolderPath";
            // 
            // pbImportProgress
            // 
            resources.ApplyResources(this.pbImportProgress, "pbImportProgress");
            this.pbImportProgress.Name = "pbImportProgress";
            this.pbImportProgress.UseWaitCursor = true;
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Name = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbStatus
            // 
            resources.ApplyResources(this.lbStatus, "lbStatus");
            this.lbStatus.Name = "lbStatus";
            // 
            // MediaImporter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.pbImportProgress);
            this.Controls.Add(this.lblFolderPath);
            this.Controls.Add(this.btnBrowseFolderPath);
            this.Controls.Add(this.txtFolderPath);
            this.Name = "MediaImporter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MediaImporter_FormClosing);
            this.Load += new System.EventHandler(this.ImageImporter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowseFolderPath;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.ProgressBar pbImportProgress;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lbStatus;
    }
}