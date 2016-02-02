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
            this.wbMain = new System.Windows.Forms.WebBrowser();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.btnIndexFolder = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.tpPreview = new System.Windows.Forms.TabPage();
            this.tcMain.SuspendLayout();
            this.tpSettings.SuspendLayout();
            this.tpPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // wbMain
            // 
            this.wbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbMain.Location = new System.Drawing.Point(3, 3);
            this.wbMain.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbMain.Name = "wbMain";
            this.wbMain.Size = new System.Drawing.Size(854, 558);
            this.wbMain.TabIndex = 0;
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderPath.Location = new System.Drawing.Point(192, 8);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(682, 20);
            this.txtFolderPath.TabIndex = 1;
            this.txtFolderPath.TextChanged += new System.EventHandler(this.txtFolderPath_TextChanged);
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(8, 7);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(176, 23);
            this.btnBrowseFolder.TabIndex = 0;
            this.btnBrowseFolder.Text = "Browse folder...";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // btnIndexFolder
            // 
            this.btnIndexFolder.Enabled = false;
            this.btnIndexFolder.Location = new System.Drawing.Point(8, 32);
            this.btnIndexFolder.Name = "btnIndexFolder";
            this.btnIndexFolder.Size = new System.Drawing.Size(344, 23);
            this.btnIndexFolder.TabIndex = 2;
            this.btnIndexFolder.Text = "Index selected folder";
            this.btnIndexFolder.UseVisualStyleBackColor = true;
            this.btnIndexFolder.Click += new System.EventHandler(this.btnIndexFolder_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Enabled = false;
            this.btnUpload.Location = new System.Drawing.Point(360, 32);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(344, 23);
            this.btnUpload.TabIndex = 3;
            this.btnUpload.Text = "Upload and close this window";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpPreview);
            this.tcMain.Controls.Add(this.tpSettings);
            this.tcMain.Location = new System.Drawing.Point(8, 64);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(868, 590);
            this.tcMain.TabIndex = 5;
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.pgSettings);
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpSettings.Size = new System.Drawing.Size(860, 564);
            this.tpSettings.TabIndex = 0;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // pgSettings
            // 
            this.pgSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSettings.Location = new System.Drawing.Point(3, 3);
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.Size = new System.Drawing.Size(854, 558);
            this.pgSettings.TabIndex = 0;
            this.pgSettings.ToolbarVisible = false;
            // 
            // tpPreview
            // 
            this.tpPreview.Controls.Add(this.wbMain);
            this.tpPreview.Location = new System.Drawing.Point(4, 22);
            this.tpPreview.Name = "tpPreview";
            this.tpPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tpPreview.Size = new System.Drawing.Size(860, 564);
            this.tpPreview.TabIndex = 1;
            this.tpPreview.Text = "Preview";
            this.tpPreview.UseVisualStyleBackColor = true;
            // 
            // DirectoryIndexerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnIndexFolder);
            this.Controls.Add(this.btnBrowseFolder);
            this.Controls.Add(this.txtFolderPath);
            this.Name = "DirectoryIndexerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Directory indexer";
            this.tcMain.ResumeLayout(false);
            this.tpSettings.ResumeLayout(false);
            this.tpPreview.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbMain;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.Button btnIndexFolder;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.TabPage tpPreview;
    }
}