namespace ShareX.UploadersLib
{
    partial class FTPClientForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FTPClientForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lvFTPList = new ShareX.UploadersLib.ListViewEx();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFilesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFiletype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLastModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tssRightClickMenu1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenURL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tssRightClickMenu2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyURL = new System.Windows.Forms.ToolStripMenuItem();
            this.txtRename = new System.Windows.Forms.TextBox();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.cbDirectoryList = new System.Windows.Forms.ComboBox();
            this.pConnecting = new System.Windows.Forms.Panel();
            this.lblConnecting = new System.Windows.Forms.Label();
            this.pbConnecting = new System.Windows.Forms.ProgressBar();
            this.tcFTP = new System.Windows.Forms.TabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.tpAccount = new System.Windows.Forms.TabPage();
            this.pgAccount = new System.Windows.Forms.PropertyGrid();
            this.tpConsole = new System.Windows.Forms.TabPage();
            this.scConsole = new System.Windows.Forms.SplitContainer();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.txtConsoleWrite = new System.Windows.Forms.TextBox();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.ssStatus.SuspendLayout();
            this.cmsRightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.pConnecting.SuspendLayout();
            this.tcFTP.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.tpAccount.SuspendLayout();
            this.tpConsole.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scConsole)).BeginInit();
            this.scConsole.Panel1.SuspendLayout();
            this.scConsole.Panel2.SuspendLayout();
            this.scConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.ssStatus);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lvFTPList);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // ssStatus
            // 
            resources.ApplyResources(this.ssStatus, "ssStatus");
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ssStatus.SizingGrip = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            resources.ApplyResources(this.lblStatus, "lblStatus");
            // 
            // lvFTPList
            // 
            this.lvFTPList.AllowColumnReorder = true;
            this.lvFTPList.AllowDrop = true;
            this.lvFTPList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chFilesize,
            this.chFiletype,
            this.chLastModified});
            this.lvFTPList.ContextMenuStrip = this.cmsRightClickMenu;
            resources.ApplyResources(this.lvFTPList, "lvFTPList");
            this.lvFTPList.DoubleClickActivation = false;
            this.lvFTPList.FullRowSelect = true;
            this.lvFTPList.GridLines = true;
            this.lvFTPList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFTPList.HideSelection = false;
            this.lvFTPList.Name = "lvFTPList";
            this.lvFTPList.UseCompatibleStateImageBehavior = false;
            this.lvFTPList.View = System.Windows.Forms.View.Details;
            this.lvFTPList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvFTPList_ItemDrag);
            this.lvFTPList.SelectedIndexChanged += new System.EventHandler(this.lvFTPList_SelectedIndexChanged);
            this.lvFTPList.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvFTPList_DragDrop);
            this.lvFTPList.DragOver += new System.Windows.Forms.DragEventHandler(this.lvFTPList_DragOver);
            this.lvFTPList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvFTPList_MouseDoubleClick);
            // 
            // chFilename
            // 
            resources.ApplyResources(this.chFilename, "chFilename");
            // 
            // chFilesize
            // 
            resources.ApplyResources(this.chFilesize, "chFilesize");
            // 
            // chFiletype
            // 
            resources.ApplyResources(this.chFiletype, "chFiletype");
            // 
            // chLastModified
            // 
            resources.ApplyResources(this.chLastModified, "chLastModified");
            // 
            // cmsRightClickMenu
            // 
            this.cmsRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiConnect,
            this.tsmiDisconnect,
            this.tssRightClickMenu1,
            this.tsmiDownload,
            this.tsmiOpenURL,
            this.tsmiRename,
            this.tsmiDelete,
            this.tssRightClickMenu2,
            this.tsmiRefresh,
            this.tsmiCreateDirectory,
            this.tsmiCopyURL});
            this.cmsRightClickMenu.Name = "cmsRightClickMenu";
            this.cmsRightClickMenu.ShowImageMargin = false;
            resources.ApplyResources(this.cmsRightClickMenu, "cmsRightClickMenu");
            // 
            // tsmiConnect
            // 
            this.tsmiConnect.Name = "tsmiConnect";
            resources.ApplyResources(this.tsmiConnect, "tsmiConnect");
            this.tsmiConnect.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // tsmiDisconnect
            // 
            this.tsmiDisconnect.Name = "tsmiDisconnect";
            resources.ApplyResources(this.tsmiDisconnect, "tsmiDisconnect");
            this.tsmiDisconnect.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // tssRightClickMenu1
            // 
            this.tssRightClickMenu1.Name = "tssRightClickMenu1";
            resources.ApplyResources(this.tssRightClickMenu1, "tssRightClickMenu1");
            // 
            // tsmiDownload
            // 
            this.tsmiDownload.Name = "tsmiDownload";
            resources.ApplyResources(this.tsmiDownload, "tsmiDownload");
            this.tsmiDownload.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // tsmiOpenURL
            // 
            this.tsmiOpenURL.Name = "tsmiOpenURL";
            resources.ApplyResources(this.tsmiOpenURL, "tsmiOpenURL");
            this.tsmiOpenURL.Click += new System.EventHandler(this.openURLToolStripMenuItem_Click);
            // 
            // tsmiRename
            // 
            this.tsmiRename.Name = "tsmiRename";
            resources.ApplyResources(this.tsmiRename, "tsmiRename");
            this.tsmiRename.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            resources.ApplyResources(this.tsmiDelete, "tsmiDelete");
            this.tsmiDelete.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // tssRightClickMenu2
            // 
            this.tssRightClickMenu2.Name = "tssRightClickMenu2";
            resources.ApplyResources(this.tssRightClickMenu2, "tssRightClickMenu2");
            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            resources.ApplyResources(this.tsmiRefresh, "tsmiRefresh");
            this.tsmiRefresh.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // tsmiCreateDirectory
            // 
            this.tsmiCreateDirectory.Name = "tsmiCreateDirectory";
            resources.ApplyResources(this.tsmiCreateDirectory, "tsmiCreateDirectory");
            this.tsmiCreateDirectory.Click += new System.EventHandler(this.createDirectoryToolStripMenuItem_Click);
            // 
            // tsmiCopyURL
            // 
            this.tsmiCopyURL.Name = "tsmiCopyURL";
            resources.ApplyResources(this.tsmiCopyURL, "tsmiCopyURL");
            this.tsmiCopyURL.Click += new System.EventHandler(this.copyURLsToClipboardToolStripMenuItem_Click);
            // 
            // txtRename
            // 
            resources.ApplyResources(this.txtRename, "txtRename");
            this.txtRename.Name = "txtRename";
            // 
            // scMain
            // 
            resources.ApplyResources(this.scMain, "scMain");
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.cbDirectoryList);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pConnecting);
            this.scMain.Panel2.Controls.Add(this.toolStripContainer1);
            // 
            // cbDirectoryList
            // 
            resources.ApplyResources(this.cbDirectoryList, "cbDirectoryList");
            this.cbDirectoryList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirectoryList.FormattingEnabled = true;
            this.cbDirectoryList.Name = "cbDirectoryList";
            this.cbDirectoryList.SelectedIndexChanged += new System.EventHandler(this.cbDirectoryList_SelectedIndexChanged);
            // 
            // pConnecting
            // 
            this.pConnecting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pConnecting.Controls.Add(this.lblConnecting);
            this.pConnecting.Controls.Add(this.pbConnecting);
            resources.ApplyResources(this.pConnecting, "pConnecting");
            this.pConnecting.Name = "pConnecting";
            // 
            // lblConnecting
            // 
            resources.ApplyResources(this.lblConnecting, "lblConnecting");
            this.lblConnecting.Name = "lblConnecting";
            // 
            // pbConnecting
            // 
            resources.ApplyResources(this.pbConnecting, "pbConnecting");
            this.pbConnecting.Name = "pbConnecting";
            this.pbConnecting.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // tcFTP
            // 
            this.tcFTP.Controls.Add(this.tpMain);
            this.tcFTP.Controls.Add(this.tpAccount);
            this.tcFTP.Controls.Add(this.tpConsole);
            resources.ApplyResources(this.tcFTP, "tcFTP");
            this.tcFTP.Name = "tcFTP";
            this.tcFTP.SelectedIndex = 0;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.scMain);
            resources.ApplyResources(this.tpMain, "tpMain");
            this.tpMain.Name = "tpMain";
            this.tpMain.UseVisualStyleBackColor = true;
            // 
            // tpAccount
            // 
            this.tpAccount.Controls.Add(this.pgAccount);
            resources.ApplyResources(this.tpAccount, "tpAccount");
            this.tpAccount.Name = "tpAccount";
            this.tpAccount.UseVisualStyleBackColor = true;
            // 
            // pgAccount
            // 
            resources.ApplyResources(this.pgAccount, "pgAccount");
            this.pgAccount.Name = "pgAccount";
            this.pgAccount.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgAccount.ToolbarVisible = false;
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.scConsole);
            resources.ApplyResources(this.tpConsole, "tpConsole");
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // scConsole
            // 
            resources.ApplyResources(this.scConsole, "scConsole");
            this.scConsole.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scConsole.Name = "scConsole";
            // 
            // scConsole.Panel1
            // 
            this.scConsole.Panel1.Controls.Add(this.txtDebug);
            // 
            // scConsole.Panel2
            // 
            this.scConsole.Panel2.Controls.Add(this.txtConsoleWrite);
            // 
            // txtDebug
            // 
            resources.ApplyResources(this.txtDebug, "txtDebug");
            this.txtDebug.Name = "txtDebug";
            // 
            // txtConsoleWrite
            // 
            this.txtConsoleWrite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtConsoleWrite, "txtConsoleWrite");
            this.txtConsoleWrite.Name = "txtConsoleWrite";
            this.txtConsoleWrite.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsoleWrite_KeyDown);
            // 
            // FTPClientForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcFTP);
            this.Controls.Add(this.txtRename);
            this.Name = "FTPClientForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FTPClient2_FormClosing);
            this.Resize += new System.EventHandler(this.FTPClient_Resize);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.cmsRightClickMenu.ResumeLayout(false);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.pConnecting.ResumeLayout(false);
            this.tcFTP.ResumeLayout(false);
            this.tpMain.ResumeLayout(false);
            this.tpAccount.ResumeLayout(false);
            this.tpConsole.ResumeLayout(false);
            this.scConsole.Panel1.ResumeLayout(false);
            this.scConsole.Panel1.PerformLayout();
            this.scConsole.Panel2.ResumeLayout(false);
            this.scConsole.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scConsole)).EndInit();
            this.scConsole.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private ListViewEx lvFTPList;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chFilesize;
        private System.Windows.Forms.ColumnHeader chFiletype;
        private System.Windows.Forms.ColumnHeader chLastModified;
        private System.Windows.Forms.ContextMenuStrip cmsRightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiRename;
        private System.Windows.Forms.TextBox txtRename;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiDownload;
        private System.Windows.Forms.TabControl tcFTP;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.TabPage tpConsole;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateDirectory;
        private System.Windows.Forms.ComboBox cbDirectoryList;
        private System.Windows.Forms.ToolStripSeparator tssRightClickMenu2;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyURL;
        private System.Windows.Forms.Panel pConnecting;
        private System.Windows.Forms.Label lblConnecting;
        private System.Windows.Forms.ProgressBar pbConnecting;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenURL;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem tsmiConnect;
        private System.Windows.Forms.ToolStripMenuItem tsmiDisconnect;
        private System.Windows.Forms.ToolStripSeparator tssRightClickMenu1;
        private System.Windows.Forms.TabPage tpAccount;
        private System.Windows.Forms.PropertyGrid pgAccount;
        private System.Windows.Forms.SplitContainer scConsole;
        private System.Windows.Forms.TextBox txtConsoleWrite;
        private System.Windows.Forms.TextBox txtDebug;
    }
}