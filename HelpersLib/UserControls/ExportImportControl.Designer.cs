namespace HelpersLib.UserControls
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
            this.cmsExport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExportClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsImport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiImportClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImport = new HelpersLib.MenuButton();
            this.btnExport = new HelpersLib.MenuButton();
            this.tsmiImportURL = new System.Windows.Forms.ToolStripMenuItem();
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
            this.cmsExport.Size = new System.Drawing.Size(145, 70);
            // 
            // tsmiExportClipboard
            // 
            this.tsmiExportClipboard.Name = "tsmiExportClipboard";
            this.tsmiExportClipboard.Size = new System.Drawing.Size(144, 22);
            this.tsmiExportClipboard.Text = "Copy to clipboard";
            this.tsmiExportClipboard.Click += new System.EventHandler(this.tsmiExportClipboard_Click);
            // 
            // tsmiExportFile
            // 
            this.tsmiExportFile.Name = "tsmiExportFile";
            this.tsmiExportFile.Size = new System.Drawing.Size(144, 22);
            this.tsmiExportFile.Text = "Save to file...";
            this.tsmiExportFile.Click += new System.EventHandler(this.tsmiExportFile_Click);
            // 
            // tsmiExportUpload
            // 
            this.tsmiExportUpload.Name = "tsmiExportUpload";
            this.tsmiExportUpload.Size = new System.Drawing.Size(144, 22);
            this.tsmiExportUpload.Text = "Upload as text";
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
            this.cmsImport.Size = new System.Drawing.Size(131, 92);
            // 
            // tsmiImportClipboard
            // 
            this.tsmiImportClipboard.Name = "tsmiImportClipboard";
            this.tsmiImportClipboard.Size = new System.Drawing.Size(130, 22);
            this.tsmiImportClipboard.Text = "From clipboard";
            this.tsmiImportClipboard.Click += new System.EventHandler(this.tsmiImportClipboard_Click);
            // 
            // tsmiImportFile
            // 
            this.tsmiImportFile.Name = "tsmiImportFile";
            this.tsmiImportFile.Size = new System.Drawing.Size(130, 22);
            this.tsmiImportFile.Text = "From file...";
            this.tsmiImportFile.Click += new System.EventHandler(this.tsmiImportFile_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(72, 0);
            this.btnImport.Menu = this.cmsImport;
            this.btnImport.MenuX0 = true;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(64, 24);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = " Import";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(0, 0);
            this.btnExport.Menu = this.cmsExport;
            this.btnExport.MenuX0 = true;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(64, 24);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = " Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // tsmiImportURL
            // 
            this.tsmiImportURL.Name = "tsmiImportURL";
            this.tsmiImportURL.Size = new System.Drawing.Size(130, 22);
            this.tsmiImportURL.Text = "From URL...";
            this.tsmiImportURL.Click += new System.EventHandler(this.tsmiImportURL_Click);
            // 
            // ExportImportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Name = "ExportImportControl";
            this.Size = new System.Drawing.Size(136, 24);
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
