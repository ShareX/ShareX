namespace ShareX.UploadersLib.Forms
{
    partial class DropboxFilesForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropboxFilesForm));
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.tsbSelectFolder = new System.Windows.Forms.ToolStripButton();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lvDropboxFiles = new ShareX.HelpersLib.MyListView();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsDropbox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyPublicLink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDownloadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenu.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.cmsDropbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMenu
            // 
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSelectFolder});
            resources.ApplyResources(this.tsMenu, "tsMenu");
            this.tsMenu.Name = "tsMenu";
            // 
            // tsbSelectFolder
            // 
            this.tsbSelectFolder.Image = global::ShareX.UploadersLib.Properties.Resources.folder;
            resources.ApplyResources(this.tsbSelectFolder, "tsbSelectFolder");
            this.tsbSelectFolder.Name = "tsbSelectFolder";
            this.tsbSelectFolder.Click += new System.EventHandler(this.tsbSelectFolder_Click);
            // 
            // tlpMain
            // 
            resources.ApplyResources(this.tlpMain, "tlpMain");
            this.tlpMain.Controls.Add(this.tsMenu, 0, 0);
            this.tlpMain.Controls.Add(this.lvDropboxFiles, 0, 1);
            this.tlpMain.Name = "tlpMain";
            // 
            // lvDropboxFiles
            // 
            this.lvDropboxFiles.AutoFillColumn = true;
            this.lvDropboxFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chSize,
            this.chModified});
            this.lvDropboxFiles.ContextMenuStrip = this.cmsDropbox;
            resources.ApplyResources(this.lvDropboxFiles, "lvDropboxFiles");
            this.lvDropboxFiles.FullRowSelect = true;
            this.lvDropboxFiles.GridLines = true;
            this.lvDropboxFiles.Name = "lvDropboxFiles";
            this.lvDropboxFiles.UseCompatibleStateImageBehavior = false;
            this.lvDropboxFiles.View = System.Windows.Forms.View.Details;
            this.lvDropboxFiles.SelectedIndexChanged += new System.EventHandler(this.lvDropboxFiles_SelectedIndexChanged);
            this.lvDropboxFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvDropboxFiles_MouseDoubleClick);
            // 
            // chFilename
            // 
            resources.ApplyResources(this.chFilename, "chFilename");
            // 
            // chSize
            // 
            resources.ApplyResources(this.chSize, "chSize");
            // 
            // chModified
            // 
            resources.ApplyResources(this.chModified, "chModified");
            // 
            // cmsDropbox
            // 
            this.cmsDropbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyPublicLink,
            this.tsmiDownloadFile,
            this.tsmiDelete,
            this.tsmiRefresh,
            this.tsmiCreateDirectory});
            this.cmsDropbox.Name = "cmsDropbox";
            this.cmsDropbox.ShowImageMargin = false;
            resources.ApplyResources(this.cmsDropbox, "cmsDropbox");
            this.cmsDropbox.Opening += new System.ComponentModel.CancelEventHandler(this.cmsDropbox_Opening);
            // 
            // tsmiCopyPublicLink
            // 
            this.tsmiCopyPublicLink.Name = "tsmiCopyPublicLink";
            resources.ApplyResources(this.tsmiCopyPublicLink, "tsmiCopyPublicLink");
            this.tsmiCopyPublicLink.Click += new System.EventHandler(this.tsmiCopyPublicLink_Click);
            // 
            // tsmiDownloadFile
            // 
            this.tsmiDownloadFile.Name = "tsmiDownloadFile";
            resources.ApplyResources(this.tsmiDownloadFile, "tsmiDownloadFile");
            this.tsmiDownloadFile.Click += new System.EventHandler(this.tsmiDownloadFile_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            resources.ApplyResources(this.tsmiDelete, "tsmiDelete");
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            resources.ApplyResources(this.tsmiRefresh, "tsmiRefresh");
            this.tsmiRefresh.Click += new System.EventHandler(this.tsmiRefresh_Click);
            // 
            // tsmiCreateDirectory
            // 
            this.tsmiCreateDirectory.Name = "tsmiCreateDirectory";
            resources.ApplyResources(this.tsmiCreateDirectory, "tsmiCreateDirectory");
            this.tsmiCreateDirectory.Click += new System.EventHandler(this.tsmiCreateDirectory_Click);
            // 
            // DropboxFilesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tlpMain);
            this.Name = "DropboxFilesForm";
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.cmsDropbox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ShareX.HelpersLib.MyListView lvDropboxFiles;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chModified;
        private System.Windows.Forms.ToolStrip tsMenu;
        private System.Windows.Forms.ToolStripButton tsbSelectFolder;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.ContextMenuStrip cmsDropbox;
        private System.Windows.Forms.ToolStripMenuItem tsmiDownloadFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyPublicLink;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateDirectory;
    }
}