namespace ShareX.IndexerLib
{
    partial class DirectoryIndexerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectoryIndexerForm));
            txtFolderPath = new System.Windows.Forms.TextBox();
            btnBrowseFolder = new System.Windows.Forms.Button();
            btnIndexFolder = new System.Windows.Forms.Button();
            btnUpload = new System.Windows.Forms.Button();
            tcMain = new System.Windows.Forms.TabControl();
            tpPreview = new System.Windows.Forms.TabPage();
            txtPreview = new System.Windows.Forms.TextBox();
            wbPreview = new System.Windows.Forms.WebBrowser();
            tpSettings = new System.Windows.Forms.TabPage();
            pgSettings = new System.Windows.Forms.PropertyGrid();
            btnSaveAs = new System.Windows.Forms.Button();
            tcMain.SuspendLayout();
            tpPreview.SuspendLayout();
            tpSettings.SuspendLayout();
            SuspendLayout();
            // 
            // txtFolderPath
            // 
            resources.ApplyResources(txtFolderPath, "txtFolderPath");
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.TextChanged += txtFolderPath_TextChanged;
            // 
            // btnBrowseFolder
            // 
            resources.ApplyResources(btnBrowseFolder, "btnBrowseFolder");
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // btnIndexFolder
            // 
            resources.ApplyResources(btnIndexFolder, "btnIndexFolder");
            btnIndexFolder.Name = "btnIndexFolder";
            btnIndexFolder.UseVisualStyleBackColor = true;
            btnIndexFolder.Click += btnIndexFolder_Click;
            // 
            // btnUpload
            // 
            resources.ApplyResources(btnUpload, "btnUpload");
            btnUpload.Name = "btnUpload";
            btnUpload.UseVisualStyleBackColor = true;
            btnUpload.Click += btnUpload_Click;
            // 
            // tcMain
            // 
            resources.ApplyResources(tcMain, "tcMain");
            tcMain.Controls.Add(tpPreview);
            tcMain.Controls.Add(tpSettings);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            // 
            // tpPreview
            // 
            tpPreview.BackColor = System.Drawing.SystemColors.Window;
            tpPreview.Controls.Add(txtPreview);
            tpPreview.Controls.Add(wbPreview);
            resources.ApplyResources(tpPreview, "tpPreview");
            tpPreview.Name = "tpPreview";
            // 
            // txtPreview
            // 
            txtPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(txtPreview, "txtPreview");
            txtPreview.Name = "txtPreview";
            // 
            // wbPreview
            // 
            resources.ApplyResources(wbPreview, "wbPreview");
            wbPreview.Name = "wbPreview";
            // 
            // tpSettings
            // 
            tpSettings.BackColor = System.Drawing.SystemColors.Window;
            tpSettings.Controls.Add(pgSettings);
            resources.ApplyResources(tpSettings, "tpSettings");
            tpSettings.Name = "tpSettings";
            // 
            // pgSettings
            // 
            pgSettings.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(pgSettings, "pgSettings");
            pgSettings.Name = "pgSettings";
            pgSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            pgSettings.ToolbarVisible = false;
            // 
            // btnSaveAs
            // 
            resources.ApplyResources(btnSaveAs, "btnSaveAs");
            btnSaveAs.Name = "btnSaveAs";
            btnSaveAs.UseVisualStyleBackColor = true;
            btnSaveAs.Click += btnSaveAs_Click;
            // 
            // DirectoryIndexerForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(btnSaveAs);
            Controls.Add(tcMain);
            Controls.Add(btnUpload);
            Controls.Add(btnIndexFolder);
            Controls.Add(btnBrowseFolder);
            Controls.Add(txtFolderPath);
            Name = "DirectoryIndexerForm";
            Load += DirectoryIndexerForm_Load;
            tcMain.ResumeLayout(false);
            tpPreview.ResumeLayout(false);
            tpPreview.PerformLayout();
            tpSettings.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbPreview;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.Button btnIndexFolder;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.TabPage tpPreview;
        private System.Windows.Forms.TextBox txtPreview;
        private System.Windows.Forms.Button btnSaveAs;
    }
}