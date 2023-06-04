namespace ShareX
{
    partial class WatchFolderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WatchFolderForm));
            this.btnPathBrowse = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.lblFilterExample = new System.Windows.Forms.Label();
            this.cbIncludeSubdirectories = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbMoveToScreenshotsFolder = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnPathBrowse
            // 
            resources.ApplyResources(this.btnPathBrowse, "btnPathBrowse");
            this.btnPathBrowse.Name = "btnPathBrowse";
            this.btnPathBrowse.UseVisualStyleBackColor = true;
            this.btnPathBrowse.Click += new System.EventHandler(this.btnPathBrowse_Click);
            // 
            // txtFilter
            // 
            resources.ApplyResources(this.txtFilter, "txtFilter");
            this.txtFilter.Name = "txtFilter";
            // 
            // txtFolderPath
            // 
            resources.ApplyResources(this.txtFolderPath, "txtFolderPath");
            this.txtFolderPath.Name = "txtFolderPath";
            // 
            // lblFilter
            // 
            resources.ApplyResources(this.lblFilter, "lblFilter");
            this.lblFilter.Name = "lblFilter";
            // 
            // lblFolderPath
            // 
            resources.ApplyResources(this.lblFolderPath, "lblFolderPath");
            this.lblFolderPath.Name = "lblFolderPath";
            // 
            // lblFilterExample
            // 
            resources.ApplyResources(this.lblFilterExample, "lblFilterExample");
            this.lblFilterExample.Name = "lblFilterExample";
            // 
            // cbIncludeSubdirectories
            // 
            resources.ApplyResources(this.cbIncludeSubdirectories, "cbIncludeSubdirectories");
            this.cbIncludeSubdirectories.Name = "cbIncludeSubdirectories";
            this.cbIncludeSubdirectories.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbMoveToScreenshotsFolder
            // 
            resources.ApplyResources(this.cbMoveToScreenshotsFolder, "cbMoveToScreenshotsFolder");
            this.cbMoveToScreenshotsFolder.Name = "cbMoveToScreenshotsFolder";
            this.cbMoveToScreenshotsFolder.UseVisualStyleBackColor = true;
            // 
            // WatchFolderForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbMoveToScreenshotsFolder);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbIncludeSubdirectories);
            this.Controls.Add(this.lblFilterExample);
            this.Controls.Add(this.btnPathBrowse);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.lblFolderPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "WatchFolderForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPathBrowse;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.Label lblFilterExample;
        private System.Windows.Forms.CheckBox cbIncludeSubdirectories;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbMoveToScreenshotsFolder;
    }
}