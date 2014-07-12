namespace CodeWorks
{
    partial class MainForm
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
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lvResults = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddLicense = new System.Windows.Forms.Button();
            this.btnAddLicenseAll = new System.Windows.Forms.Button();
            this.scTextBoxes = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbDefaultText = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.tbNewText = new System.Windows.Forms.TextBox();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.btnFindRegionAreas = new System.Windows.Forms.Button();
            this.btnOrderLines = new System.Windows.Forms.Button();
            this.scTextBoxes.Panel1.SuspendLayout();
            this.scTextBoxes.Panel2.SuspendLayout();
            this.scTextBoxes.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(72, 8);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(974, 20);
            this.txtFolderPath.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(152, 40);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(136, 24);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search (Old method)";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lvResults
            // 
            this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvResults.FullRowSelect = true;
            this.lvResults.GridLines = true;
            this.lvResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvResults.Location = new System.Drawing.Point(0, 0);
            this.lvResults.MultiSelect = false;
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(1040, 351);
            this.lvResults.TabIndex = 2;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.SelectedIndexChanged += new System.EventHandler(this.lvResults_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 900;
            // 
            // btnAddLicense
            // 
            this.btnAddLicense.Location = new System.Drawing.Point(296, 40);
            this.btnAddLicense.Name = "btnAddLicense";
            this.btnAddLicense.Size = new System.Drawing.Size(80, 24);
            this.btnAddLicense.TabIndex = 4;
            this.btnAddLicense.Text = "Add license";
            this.btnAddLicense.UseVisualStyleBackColor = true;
            this.btnAddLicense.Click += new System.EventHandler(this.btnAddLicense_Click);
            // 
            // btnAddLicenseAll
            // 
            this.btnAddLicenseAll.Location = new System.Drawing.Point(384, 40);
            this.btnAddLicenseAll.Name = "btnAddLicenseAll";
            this.btnAddLicenseAll.Size = new System.Drawing.Size(112, 24);
            this.btnAddLicenseAll.TabIndex = 5;
            this.btnAddLicenseAll.Text = "Add license to all";
            this.btnAddLicenseAll.UseVisualStyleBackColor = true;
            this.btnAddLicenseAll.Click += new System.EventHandler(this.btnAddLicenseAll_Click);
            // 
            // scTextBoxes
            // 
            this.scTextBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTextBoxes.Location = new System.Drawing.Point(0, 0);
            this.scTextBoxes.Name = "scTextBoxes";
            // 
            // scTextBoxes.Panel1
            // 
            this.scTextBoxes.Panel1.Controls.Add(this.groupBox1);
            // 
            // scTextBoxes.Panel2
            // 
            this.scTextBoxes.Panel2.Controls.Add(this.groupBox2);
            this.scTextBoxes.Size = new System.Drawing.Size(1040, 346);
            this.scTextBoxes.SplitterDistance = 501;
            this.scTextBoxes.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbDefaultText);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(501, 346);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default text";
            // 
            // tbDefaultText
            // 
            this.tbDefaultText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDefaultText.Location = new System.Drawing.Point(3, 16);
            this.tbDefaultText.Multiline = true;
            this.tbDefaultText.Name = "tbDefaultText";
            this.tbDefaultText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbDefaultText.Size = new System.Drawing.Size(495, 327);
            this.tbDefaultText.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnCopyToClipboard);
            this.groupBox2.Controls.Add(this.tbNewText);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(535, 346);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New text";
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Location = new System.Drawing.Point(384, 312);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(115, 23);
            this.btnCopyToClipboard.TabIndex = 2;
            this.btnCopyToClipboard.Text = "Copy to clipboard";
            this.btnCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // tbNewText
            // 
            this.tbNewText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNewText.Location = new System.Drawing.Point(3, 16);
            this.tbNewText.Multiline = true;
            this.tbNewText.Name = "tbNewText";
            this.tbNewText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbNewText.Size = new System.Drawing.Size(529, 327);
            this.tbNewText.TabIndex = 1;
            // 
            // scMain
            // 
            this.scMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scMain.Location = new System.Drawing.Point(8, 72);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.lvResults);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.scTextBoxes);
            this.scMain.Size = new System.Drawing.Size(1040, 704);
            this.scMain.SplitterDistance = 351;
            this.scMain.SplitterWidth = 7;
            this.scMain.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Folder path:";
            // 
            // lblFileCount
            // 
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblFileCount.Location = new System.Drawing.Point(512, 44);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(0, 16);
            this.lblFileCount.TabIndex = 9;
            // 
            // btnFindRegionAreas
            // 
            this.btnFindRegionAreas.Location = new System.Drawing.Point(8, 40);
            this.btnFindRegionAreas.Name = "btnFindRegionAreas";
            this.btnFindRegionAreas.Size = new System.Drawing.Size(136, 24);
            this.btnFindRegionAreas.TabIndex = 10;
            this.btnFindRegionAreas.Text = "Search region areas";
            this.btnFindRegionAreas.UseVisualStyleBackColor = true;
            this.btnFindRegionAreas.Click += new System.EventHandler(this.btnFindRegionAreas_Click);
            // 
            // btnOrderLines
            // 
            this.btnOrderLines.Location = new System.Drawing.Point(912, 40);
            this.btnOrderLines.Name = "btnOrderLines";
            this.btnOrderLines.Size = new System.Drawing.Size(128, 24);
            this.btnOrderLines.TabIndex = 11;
            this.btnOrderLines.Text = "Clipboard lines A-Z";
            this.btnOrderLines.UseVisualStyleBackColor = true;
            this.btnOrderLines.Click += new System.EventHandler(this.btnOrderLines_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 785);
            this.Controls.Add(this.btnOrderLines);
            this.Controls.Add(this.btnFindRegionAreas);
            this.Controls.Add(this.lblFileCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.btnAddLicenseAll);
            this.Controls.Add(this.btnAddLicense);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtFolderPath);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CodeWorks";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.scTextBoxes.Panel1.ResumeLayout(false);
            this.scTextBoxes.Panel2.ResumeLayout(false);
            this.scTextBoxes.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            this.scMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnAddLicense;
        private System.Windows.Forms.Button btnAddLicenseAll;
        private System.Windows.Forms.SplitContainer scTextBoxes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbDefaultText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbNewText;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Button btnFindRegionAreas;
        private System.Windows.Forms.Button btnOrderLines;
        private System.Windows.Forms.Button btnCopyToClipboard;
    }
}

