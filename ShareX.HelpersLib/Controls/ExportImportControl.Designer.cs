namespace ShareX.HelpersLib
{
    partial class ExportImportControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportImportControl));
            this.cmsExport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExportClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsImport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiImportClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportURL = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImport = new HelpersLib.MenuButton();
            this.btnExport = new HelpersLib.MenuButton();
            this.cmsExport.SuspendLayout();
            this.cmsImport.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsExport
            // 
            this.cmsExport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExportClipboard,
            this.tsmiExportFile,
            this.tsmiExportUpload});
            this.cmsExport.Name = "cmsExport";
            this.cmsExport.ShowImageMargin = false;
            resources.ApplyResources(this.cmsExport, "cmsExport");
            // 
            // tsmiExportClipboard
            // 
            this.tsmiExportClipboard.Name = "tsmiExportClipboard";
            resources.ApplyResources(this.tsmiExportClipboard, "tsmiExportClipboard");
            this.tsmiExportClipboard.Click += new System.EventHandler(this.tsmiExportClipboard_Click);
            // 
            // tsmiExportFile
            // 
            this.tsmiExportFile.Name = "tsmiExportFile";
            resources.ApplyResources(this.tsmiExportFile, "tsmiExportFile");
            this.tsmiExportFile.Click += new System.EventHandler(this.tsmiExportFile_Click);
            // 
            // tsmiExportUpload
            // 
            this.tsmiExportUpload.Name = "tsmiExportUpload";
            resources.ApplyResources(this.tsmiExportUpload, "tsmiExportUpload");
            this.tsmiExportUpload.Click += new System.EventHandler(this.tsmiExportUpload_Click);
            // 
            // cmsImport
            // 
            this.cmsImport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImportClipboard,
            this.tsmiImportFile,
            this.tsmiImportURL});
            this.cmsImport.Name = "cmsImport";
            this.cmsImport.ShowImageMargin = false;
            resources.ApplyResources(this.cmsImport, "cmsImport");
            // 
            // tsmiImportClipboard
            // 
            this.tsmiImportClipboard.Name = "tsmiImportClipboard";
            resources.ApplyResources(this.tsmiImportClipboard, "tsmiImportClipboard");
            this.tsmiImportClipboard.Click += new System.EventHandler(this.tsmiImportClipboard_Click);
            // 
            // tsmiImportFile
            // 
            this.tsmiImportFile.Name = "tsmiImportFile";
            resources.ApplyResources(this.tsmiImportFile, "tsmiImportFile");
            this.tsmiImportFile.Click += new System.EventHandler(this.tsmiImportFile_Click);
            // 
            // tsmiImportURL
            // 
            this.tsmiImportURL.Name = "tsmiImportURL";
            resources.ApplyResources(this.tsmiImportURL, "tsmiImportURL");
            this.tsmiImportURL.Click += new System.EventHandler(this.tsmiImportURL_Click);
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Menu = this.cmsImport;
            this.btnImport.MenuX0 = true;
            this.btnImport.Name = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Menu = this.cmsExport;
            this.btnExport.MenuX0 = true;
            this.btnExport.Name = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // ExportImportControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Name = "ExportImportControl";
            this.cmsExport.ResumeLayout(false);
            this.cmsImport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmsExport;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportClipboard;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportFile;
        private System.Windows.Forms.ContextMenuStrip cmsImport;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportClipboard;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportFile;
        private MenuButton btnExport;
        private MenuButton btnImport;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportURL;
    }
}
