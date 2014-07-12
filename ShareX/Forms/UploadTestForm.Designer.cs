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
            this.lvUploaders = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsUploaders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testSelectedUploadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tcTesters = new System.Windows.Forms.TabControl();
            this.tpTestUploaders = new System.Windows.Forms.TabPage();
            this.tscTesters = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnTestSelected = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnTestAll = new System.Windows.Forms.ToolStripButton();
            this.tpConsole = new System.Windows.Forms.TabPage();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.cmsUploaders.SuspendLayout();
            this.tcTesters.SuspendLayout();
            this.tpTestUploaders.SuspendLayout();
            this.tscTesters.ContentPanel.SuspendLayout();
            this.tscTesters.TopToolStripPanel.SuspendLayout();
            this.tscTesters.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tpConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvUploaders
            // 
            this.lvUploaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvUploaders.ContextMenuStrip = this.cmsUploaders;
            this.lvUploaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvUploaders.FullRowSelect = true;
            this.lvUploaders.GridLines = true;
            this.lvUploaders.Location = new System.Drawing.Point(0, 0);
            this.lvUploaders.Name = "lvUploaders";
            this.lvUploaders.Size = new System.Drawing.Size(621, 608);
            this.lvUploaders.TabIndex = 0;
            this.lvUploaders.UseCompatibleStateImageBehavior = false;
            this.lvUploaders.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Status";
            this.columnHeader2.Width = 333;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Time";
            this.columnHeader3.Width = 86;
            // 
            // cmsUploaders
            // 
            this.cmsUploaders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testSelectedUploadersToolStripMenuItem,
            this.openURLToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.cmsUploaders.Name = "cmsUploaders";
            this.cmsUploaders.Size = new System.Drawing.Size(207, 70);
            // 
            // testSelectedUploadersToolStripMenuItem
            // 
            this.testSelectedUploadersToolStripMenuItem.Name = "testSelectedUploadersToolStripMenuItem";
            this.testSelectedUploadersToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.testSelectedUploadersToolStripMenuItem.Text = "Test selected uploaders";
            this.testSelectedUploadersToolStripMenuItem.Click += new System.EventHandler(this.btnTestSelected_Click);
            // 
            // openURLToolStripMenuItem
            // 
            this.openURLToolStripMenuItem.Name = "openURLToolStripMenuItem";
            this.openURLToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openURLToolStripMenuItem.Text = "Open URL";
            this.openURLToolStripMenuItem.Click += new System.EventHandler(this.openURLToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.copyToolStripMenuItem.Text = "Copy URL(s) to clipboard";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // tcTesters
            // 
            this.tcTesters.Controls.Add(this.tpTestUploaders);
            this.tcTesters.Controls.Add(this.tpConsole);
            this.tcTesters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTesters.Location = new System.Drawing.Point(3, 3);
            this.tcTesters.Name = "tcTesters";
            this.tcTesters.SelectedIndex = 0;
            this.tcTesters.Size = new System.Drawing.Size(635, 663);
            this.tcTesters.TabIndex = 0;
            // 
            // tpTestUploaders
            // 
            this.tpTestUploaders.Controls.Add(this.tscTesters);
            this.tpTestUploaders.Location = new System.Drawing.Point(4, 22);
            this.tpTestUploaders.Name = "tpTestUploaders";
            this.tpTestUploaders.Padding = new System.Windows.Forms.Padding(3);
            this.tpTestUploaders.Size = new System.Drawing.Size(627, 637);
            this.tpTestUploaders.TabIndex = 0;
            this.tpTestUploaders.Text = "Test uploaders";
            this.tpTestUploaders.UseVisualStyleBackColor = true;
            // 
            // tscTesters
            // 
            // 
            // tscTesters.ContentPanel
            // 
            this.tscTesters.ContentPanel.Controls.Add(this.lvUploaders);
            this.tscTesters.ContentPanel.Size = new System.Drawing.Size(621, 608);
            this.tscTesters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscTesters.Location = new System.Drawing.Point(3, 3);
            this.tscTesters.Name = "tscTesters";
            this.tscTesters.Size = new System.Drawing.Size(621, 631);
            this.tscTesters.TabIndex = 0;
            this.tscTesters.Text = "toolStripContainer1";
            // 
            // tscTesters.TopToolStripPanel
            // 
            this.tscTesters.TopToolStripPanel.BackColor = System.Drawing.Color.White;
            this.tscTesters.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTestSelected,
            this.toolStripSeparator1,
            this.btnTestAll});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(134, 23);
            this.toolStrip1.TabIndex = 0;
            // 
            // btnTestSelected
            // 
            this.btnTestSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTestSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTestSelected.Name = "btnTestSelected";
            this.btnTestSelected.Size = new System.Drawing.Size(79, 19);
            this.btnTestSelected.Text = "Test selected";
            this.btnTestSelected.Click += new System.EventHandler(this.btnTestSelected_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // btnTestAll
            // 
            this.btnTestAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTestAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.Size = new System.Drawing.Size(48, 19);
            this.btnTestAll.Text = "Test all";
            this.btnTestAll.Click += new System.EventHandler(this.btnTestAll_Click);
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.txtConsole);
            this.tpConsole.Location = new System.Drawing.Point(4, 22);
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tpConsole.Size = new System.Drawing.Size(627, 637);
            this.tpConsole.TabIndex = 1;
            this.tpConsole.Text = "Console";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // txtConsole
            // 
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.Location = new System.Drawing.Point(3, 3);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.Size = new System.Drawing.Size(621, 631);
            this.txtConsole.TabIndex = 0;
            // 
            // UploadTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 669);
            this.Controls.Add(this.tcTesters);
            this.Name = "UploadTestForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "ShareX - Test uploaders";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TesterGUI_FormClosing);
            this.cmsUploaders.ResumeLayout(false);
            this.tcTesters.ResumeLayout(false);
            this.tpTestUploaders.ResumeLayout(false);
            this.tscTesters.ContentPanel.ResumeLayout(false);
            this.tscTesters.TopToolStripPanel.ResumeLayout(false);
            this.tscTesters.TopToolStripPanel.PerformLayout();
            this.tscTesters.ResumeLayout(false);
            this.tscTesters.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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