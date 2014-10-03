namespace ShareX
{
    partial class UploadTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadTestForm));
            this.tscTesters = new System.Windows.Forms.ToolStripContainer();
            this.lvUploaders = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsUploaders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testSelectedUploadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnTestSelected = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnTestAll = new System.Windows.Forms.ToolStripButton();
            this.tcTesters = new System.Windows.Forms.TabControl();
            this.tpTestUploaders = new System.Windows.Forms.TabPage();
            this.tpConsole = new System.Windows.Forms.TabPage();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.tscTesters.ContentPanel.SuspendLayout();
            this.tscTesters.TopToolStripPanel.SuspendLayout();
            this.tscTesters.SuspendLayout();
            this.cmsUploaders.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tcTesters.SuspendLayout();
            this.tpTestUploaders.SuspendLayout();
            this.tpConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscTesters
            // 
            // 
            // tscTesters.ContentPanel
            // 
            this.tscTesters.ContentPanel.Controls.Add(this.lvUploaders);
            resources.ApplyResources(this.tscTesters.ContentPanel, "tscTesters.ContentPanel");
            resources.ApplyResources(this.tscTesters, "tscTesters");
            this.tscTesters.Name = "tscTesters";
            // 
            // tscTesters.TopToolStripPanel
            // 
            this.tscTesters.TopToolStripPanel.BackColor = System.Drawing.Color.White;
            this.tscTesters.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // lvUploaders
            // 
            this.lvUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvUploaders.ContextMenuStrip = this.cmsUploaders;
            resources.ApplyResources(this.lvUploaders, "lvUploaders");
            this.lvUploaders.FullRowSelect = true;
            this.lvUploaders.GridLines = true;
            this.lvUploaders.Name = "lvUploaders";
            this.lvUploaders.UseCompatibleStateImageBehavior = false;
            this.lvUploaders.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // cmsUploaders
            // 
            this.cmsUploaders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testSelectedUploadersToolStripMenuItem,
            this.openURLToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.cmsUploaders.Name = "cmsUploaders";
            resources.ApplyResources(this.cmsUploaders, "cmsUploaders");
            // 
            // testSelectedUploadersToolStripMenuItem
            // 
            this.testSelectedUploadersToolStripMenuItem.Name = "testSelectedUploadersToolStripMenuItem";
            resources.ApplyResources(this.testSelectedUploadersToolStripMenuItem, "testSelectedUploadersToolStripMenuItem");
            this.testSelectedUploadersToolStripMenuItem.Click += new System.EventHandler(this.btnTestSelected_Click);
            // 
            // openURLToolStripMenuItem
            // 
            this.openURLToolStripMenuItem.Name = "openURLToolStripMenuItem";
            resources.ApplyResources(this.openURLToolStripMenuItem, "openURLToolStripMenuItem");
            this.openURLToolStripMenuItem.Click += new System.EventHandler(this.openURLToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTestSelected,
            this.toolStripSeparator1,
            this.btnTestAll});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // btnTestSelected
            // 
            this.btnTestSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.btnTestSelected, "btnTestSelected");
            this.btnTestSelected.Name = "btnTestSelected";
            this.btnTestSelected.Click += new System.EventHandler(this.btnTestSelected_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // btnTestAll
            // 
            this.btnTestAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.btnTestAll, "btnTestAll");
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.Click += new System.EventHandler(this.btnTestAll_Click);
            // 
            // tcTesters
            // 
            this.tcTesters.Controls.Add(this.tpTestUploaders);
            this.tcTesters.Controls.Add(this.tpConsole);
            resources.ApplyResources(this.tcTesters, "tcTesters");
            this.tcTesters.Name = "tcTesters";
            this.tcTesters.SelectedIndex = 0;
            // 
            // tpTestUploaders
            // 
            this.tpTestUploaders.Controls.Add(this.tscTesters);
            resources.ApplyResources(this.tpTestUploaders, "tpTestUploaders");
            this.tpTestUploaders.Name = "tpTestUploaders";
            this.tpTestUploaders.UseVisualStyleBackColor = true;
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.txtConsole);
            resources.ApplyResources(this.tpConsole, "tpConsole");
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // txtConsole
            // 
            resources.ApplyResources(this.txtConsole, "txtConsole");
            this.txtConsole.Name = "txtConsole";
            // 
            // UploadTestForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcTesters);
            this.Name = "UploadTestForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TesterGUI_FormClosing);
            this.tscTesters.ContentPanel.ResumeLayout(false);
            this.tscTesters.TopToolStripPanel.ResumeLayout(false);
            this.tscTesters.TopToolStripPanel.PerformLayout();
            this.tscTesters.ResumeLayout(false);
            this.tscTesters.PerformLayout();
            this.cmsUploaders.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tcTesters.ResumeLayout(false);
            this.tpTestUploaders.ResumeLayout(false);
            this.tpConsole.ResumeLayout(false);
            this.tpConsole.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.ListView lvUploaders;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabControl tcTesters;
        private System.Windows.Forms.TabPage tpTestUploaders;
        private System.Windows.Forms.TabPage tpConsole;
        private System.Windows.Forms.ContextMenuStrip cmsUploaders;
        private System.Windows.Forms.ToolStripMenuItem openURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.ToolStripMenuItem testSelectedUploadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer tscTesters;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnTestAll;
        private System.Windows.Forms.ToolStripButton btnTestSelected;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}