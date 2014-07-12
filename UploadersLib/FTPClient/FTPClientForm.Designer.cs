namespace UploadersLib
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lvFTPList = new UploadersLib.ListViewEx();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFilesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFiletype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLastModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tcFTP = new System.Windows.Forms.TabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.tpAccount = new System.Windows.Forms.TabPage();
            this.pgAccount = new System.Windows.Forms.PropertyGrid();
            this.tpConsole = new System.Windows.Forms.TabPage();
            this.scConsole = new System.Windows.Forms.SplitContainer();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.txtConsoleWrite = new System.Windows.Forms.TextBox();
            this.cmsRightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pConnecting.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.cmsRightClickMenu.Size = new System.Drawing.Size(207, 214);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(203, 6);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // openURLToolStripMenuItem
            // 
            this.openURLToolStripMenuItem.Name = "openURLToolStripMenuItem";
            this.openURLToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openURLToolStripMenuItem.Text = "Open URL";
            this.openURLToolStripMenuItem.Click += new System.EventHandler(this.openURLToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // createDirectoryToolStripMenuItem
            // 
            this.createDirectoryToolStripMenuItem.Name = "createDirectoryToolStripMenuItem";
            this.createDirectoryToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.createDirectoryToolStripMenuItem.Text = "Create directory";
            this.createDirectoryToolStripMenuItem.Click += new System.EventHandler(this.createDirectoryToolStripMenuItem_Click);
            // 
            // copyURLsToClipboardToolStripMenuItem
            // 
            this.copyURLsToClipboardToolStripMenuItem.Name = "copyURLsToClipboardToolStripMenuItem";
            this.copyURLsToClipboardToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.copyURLsToClipboardToolStripMenuItem.Text = "Copy URL(s) to clipboard";
            this.copyURLsToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyURLsToClipboardToolStripMenuItem_Click);
            // 
            // txtRename
            // 
            this.txtRename.Location = new System.Drawing.Point(8, 8);
            this.txtRename.Name = "txtRename";
            this.txtRename.Size = new System.Drawing.Size(100, 20);
            this.txtRename.TabIndex = 1;
            this.txtRename.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbDirectoryList);
            this.splitContainer1.Panel1MinSize = 20;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pConnecting);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripContainer1);
            this.splitContainer1.Size = new System.Drawing.Size(952, 557);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 0;
            // 
            // cbDirectoryList
            // 
            this.cbDirectoryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbDirectoryList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirectoryList.FormattingEnabled = true;
            this.cbDirectoryList.Location = new System.Drawing.Point(0, 0);
            this.cbDirectoryList.Name = "cbDirectoryList";
            this.cbDirectoryList.Size = new System.Drawing.Size(952, 21);
            this.cbDirectoryList.TabIndex = 0;
            this.cbDirectoryList.SelectedIndexChanged += new System.EventHandler(this.cbDirectoryList_SelectedIndexChanged);
            // 
            // pConnecting
            // 
            this.pConnecting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pConnecting.Controls.Add(this.lblConnecting);
            this.pConnecting.Controls.Add(this.progressBar1);
            this.pConnecting.Location = new System.Drawing.Point(350, 208);
            this.pConnecting.Name = "pConnecting";
            this.pConnecting.Padding = new System.Windows.Forms.Padding(5);
            this.pConnecting.Size = new System.Drawing.Size(252, 64);
            this.pConnecting.TabIndex = 1;
            // 
            // lblConnecting
            // 
            this.lblConnecting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConnecting.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnecting.Location = new System.Drawing.Point(5, 5);
            this.lblConnecting.Name = "lblConnecting";
            this.lblConnecting.Size = new System.Drawing.Size(238, 26);
            this.lblConnecting.TabIndex = 0;
            this.lblConnecting.Text = "Connecting to FTP server...";
            this.lblConnecting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(5, 31);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(238, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(952, 481);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(952, 528);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(952, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 17);
            this.lblStatus.Text = "status";
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
            this.lvFTPList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFTPList.DoubleClickActivation = false;
            this.lvFTPList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvFTPList.FullRowSelect = true;
            this.lvFTPList.GridLines = true;
            this.lvFTPList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFTPList.HideSelection = false;
            this.lvFTPList.Location = new System.Drawing.Point(0, 0);
            this.lvFTPList.Name = "lvFTPList";
            this.lvFTPList.Size = new System.Drawing.Size(952, 481);
            this.lvFTPList.TabIndex = 0;
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
            this.chFilename.Text = "Filename";
            this.chFilename.Width = 493;
            // 
            // chFilesize
            // 
            this.chFilesize.Text = "Filesize";
            this.chFilesize.Width = 100;
            // 
            // chFiletype
            // 
            this.chFiletype.Text = "Filetype";
            this.chFiletype.Width = 200;
            // 
            // chLastModified
            // 
            this.chLastModified.Text = "Last modified";
            this.chLastModified.Width = 150;
            // 
            // tcFTP
            // 
            this.tcFTP.Controls.Add(this.tpMain);
            this.tcFTP.Controls.Add(this.tpAccount);
            this.tcFTP.Controls.Add(this.tpConsole);
            this.tcFTP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcFTP.Location = new System.Drawing.Point(3, 3);
            this.tcFTP.Name = "tcFTP";
            this.tcFTP.SelectedIndex = 0;
            this.tcFTP.Size = new System.Drawing.Size(966, 589);
            this.tcFTP.TabIndex = 0;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.splitContainer1);
            this.tpMain.Location = new System.Drawing.Point(4, 22);
            this.tpMain.Name = "tpMain";
            this.tpMain.Padding = new System.Windows.Forms.Padding(3);
            this.tpMain.Size = new System.Drawing.Size(958, 563);
            this.tpMain.TabIndex = 0;
            this.tpMain.Text = "Files";
            this.tpMain.UseVisualStyleBackColor = true;
            // 
            // tpAccount
            // 
            this.tpAccount.Controls.Add(this.pgAccount);
            this.tpAccount.Location = new System.Drawing.Point(4, 22);
            this.tpAccount.Name = "tpAccount";
            this.tpAccount.Padding = new System.Windows.Forms.Padding(3);
            this.tpAccount.Size = new System.Drawing.Size(958, 563);
            this.tpAccount.TabIndex = 1;
            this.tpAccount.Text = "Account";
            this.tpAccount.UseVisualStyleBackColor = true;
            // 
            // pgAccount
            // 
            this.pgAccount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgAccount.Location = new System.Drawing.Point(3, 3);
            this.pgAccount.Name = "pgAccount";
            this.pgAccount.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgAccount.Size = new System.Drawing.Size(952, 557);
            this.pgAccount.TabIndex = 0;
            this.pgAccount.ToolbarVisible = false;
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.scConsole);
            this.tpConsole.Location = new System.Drawing.Point(4, 22);
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tpConsole.Size = new System.Drawing.Size(958, 563);
            this.tpConsole.TabIndex = 2;
            this.tpConsole.Text = "Console";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // scConsole
            // 
            this.scConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scConsole.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scConsole.IsSplitterFixed = true;
            this.scConsole.Location = new System.Drawing.Point(3, 3);
            this.scConsole.Name = "scConsole";
            this.scConsole.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scConsole.Panel1
            // 
            this.scConsole.Panel1.Controls.Add(this.txtDebug);
            // 
            // scConsole.Panel2
            // 
            this.scConsole.Panel2.Controls.Add(this.txtConsoleWrite);
            this.scConsole.Panel2MinSize = 20;
            this.scConsole.Size = new System.Drawing.Size(952, 557);
            this.scConsole.SplitterDistance = 531;
            this.scConsole.SplitterWidth = 1;
            this.scConsole.TabIndex = 0;
            // 
            // txtDebug
            // 
            this.txtDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDebug.Location = new System.Drawing.Point(0, 0);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDebug.Size = new System.Drawing.Size(952, 531);
            this.txtDebug.TabIndex = 0;
            // 
            // txtConsoleWrite
            // 
            this.txtConsoleWrite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConsoleWrite.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtConsoleWrite.Location = new System.Drawing.Point(0, 5);
            this.txtConsoleWrite.Name = "txtConsoleWrite";
            this.txtConsoleWrite.Size = new System.Drawing.Size(952, 20);
            this.txtConsoleWrite.TabIndex = 0;
            this.txtConsoleWrite.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsoleWrite_KeyDown);
            // 
            // FTPClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 595);
            this.Controls.Add(this.tcFTP);
            this.Controls.Add(this.txtRename);
            this.Name = "FTPClientForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX FTP client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FTPClient2_FormClosing);
            this.Resize += new System.EventHandler(this.FTPClient_Resize);
            this.cmsRightClickMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pConnecting.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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