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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lvFTPList = new ShareX.UploadersLib.ListViewEx();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFilesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFiletype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLastModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyURLsToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtRename = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbDirectoryList = new System.Windows.Forms.ComboBox();
            this.pConnecting = new System.Windows.Forms.Panel();
            this.lblConnecting = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
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
            this.statusStrip1.SuspendLayout();
            this.cmsRightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lvFTPList);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.SizingGrip = false;
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
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.toolStripSeparator2,
            this.downloadToolStripMenuItem,
            this.openURLToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.refreshToolStripMenuItem,
            this.createDirectoryToolStripMenuItem,
            this.copyURLsToClipboardToolStripMenuItem});
            this.cmsRightClickMenu.Name = "cmsRightClickMenu";
            resources.ApplyResources(this.cmsRightClickMenu, "cmsRightClickMenu");
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            resources.ApplyResources(this.connectToolStripMenuItem, "connectToolStripMenuItem");
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            resources.ApplyResources(this.disconnectToolStripMenuItem, "disconnectToolStripMenuItem");
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            resources.ApplyResources(this.downloadToolStripMenuItem, "downloadToolStripMenuItem");
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // openURLToolStripMenuItem
            // 
            this.openURLToolStripMenuItem.Name = "openURLToolStripMenuItem";
            resources.ApplyResources(this.openURLToolStripMenuItem, "openURLToolStripMenuItem");
            this.openURLToolStripMenuItem.Click += new System.EventHandler(this.openURLToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            resources.ApplyResources(this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // createDirectoryToolStripMenuItem
            // 
            this.createDirectoryToolStripMenuItem.Name = "createDirectoryToolStripMenuItem";
            resources.ApplyResources(this.createDirectoryToolStripMenuItem, "createDirectoryToolStripMenuItem");
            this.createDirectoryToolStripMenuItem.Click += new System.EventHandler(this.createDirectoryToolStripMenuItem_Click);
            // 
            // copyURLsToClipboardToolStripMenuItem
            // 
            this.copyURLsToClipboardToolStripMenuItem.Name = "copyURLsToClipboardToolStripMenuItem";
            resources.ApplyResources(this.copyURLsToClipboardToolStripMenuItem, "copyURLsToClipboardToolStripMenuItem");
            this.copyURLsToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyURLsToClipboardToolStripMenuItem_Click);
            // 
            // txtRename
            // 
            resources.ApplyResources(this.txtRename, "txtRename");
            this.txtRename.Name = "txtRename";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbDirectoryList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pConnecting);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripContainer1);
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
            this.pConnecting.Controls.Add(this.progressBar1);
            resources.ApplyResources(this.pConnecting, "pConnecting");
            this.pConnecting.Name = "pConnecting";
            // 
            // lblConnecting
            // 
            resources.ApplyResources(this.lblConnecting, "lblConnecting");
            this.lblConnecting.Name = "lblConnecting";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
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
            this.tpMain.Controls.Add(this.splitContainer1);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmsRightClickMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.TextBox txtRename;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.TabControl tcFTP;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.TabPage tpConsole;
        private System.Windows.Forms.ToolStripMenuItem createDirectoryToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbDirectoryList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyURLsToClipboardToolStripMenuItem;
        private System.Windows.Forms.Panel pConnecting;
        private System.Windows.Forms.Label lblConnecting;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolStripMenuItem openURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TabPage tpAccount;
        private System.Windows.Forms.PropertyGrid pgAccount;
        private System.Windows.Forms.SplitContainer scConsole;
        private System.Windows.Forms.TextBox txtConsoleWrite;
        private System.Windows.Forms.TextBox txtDebug;
    }
}