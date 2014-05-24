namespace HistoryLib
{
    partial class HistoryForm
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
            this.dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
            this.cbDateFilter = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFilterTo = new System.Windows.Forms.DateTimePicker();
            this.btnApplyFilters = new System.Windows.Forms.Button();
            this.txtFilenameFilter = new System.Windows.Forms.TextBox();
            this.cbFilenameFilterMethod = new System.Windows.Forms.ComboBox();
            this.cbFilenameFilterCulture = new System.Windows.Forms.ComboBox();
            this.cbFilenameFilter = new System.Windows.Forms.CheckBox();
            this.cbFilenameFilterCase = new System.Windows.Forms.CheckBox();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbFilters = new System.Windows.Forms.GroupBox();
            this.btnRemoveFilters = new System.Windows.Forms.Button();
            this.txtHostFilter = new System.Windows.Forms.TextBox();
            this.cbTypeFilterSelection = new System.Windows.Forms.ComboBox();
            this.cbHostFilter = new System.Windows.Forms.CheckBox();
            this.cbTypeFilter = new System.Windows.Forms.CheckBox();
            this.lvHistory = new HelpersLib.MyListView();
            this.chDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chHost = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbThumbnail = new HelpersLib.MyPictureBox();
            this.ssMain.SuspendLayout();
            this.gbFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpFilterFrom
            // 
            this.dtpFilterFrom.Location = new System.Drawing.Point(56, 46);
            this.dtpFilterFrom.Name = "dtpFilterFrom";
            this.dtpFilterFrom.Size = new System.Drawing.Size(230, 20);
            this.dtpFilterFrom.TabIndex = 2;
            // 
            // cbDateFilter
            // 
            this.cbDateFilter.AutoSize = true;
            this.cbDateFilter.Location = new System.Drawing.Point(16, 24);
            this.cbDateFilter.Name = "cbDateFilter";
            this.cbDateFilter.Size = new System.Drawing.Size(74, 17);
            this.cbDateFilter.TabIndex = 0;
            this.cbDateFilter.Text = "Date filter:";
            this.cbDateFilter.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "From:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "To:";
            // 
            // dtpFilterTo
            // 
            this.dtpFilterTo.Location = new System.Drawing.Point(56, 70);
            this.dtpFilterTo.Name = "dtpFilterTo";
            this.dtpFilterTo.Size = new System.Drawing.Size(230, 20);
            this.dtpFilterTo.TabIndex = 4;
            // 
            // btnApplyFilters
            // 
            this.btnApplyFilters.Location = new System.Drawing.Point(16, 232);
            this.btnApplyFilters.Name = "btnApplyFilters";
            this.btnApplyFilters.Size = new System.Drawing.Size(136, 24);
            this.btnApplyFilters.TabIndex = 1;
            this.btnApplyFilters.Text = "Apply filters";
            this.btnApplyFilters.UseVisualStyleBackColor = true;
            this.btnApplyFilters.Click += new System.EventHandler(this.btnApplyFilters_Click);
            // 
            // txtFilenameFilter
            // 
            this.txtFilenameFilter.Location = new System.Drawing.Point(16, 119);
            this.txtFilenameFilter.Name = "txtFilenameFilter";
            this.txtFilenameFilter.Size = new System.Drawing.Size(170, 20);
            this.txtFilenameFilter.TabIndex = 6;
            // 
            // cbFilenameFilterMethod
            // 
            this.cbFilenameFilterMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilenameFilterMethod.FormattingEnabled = true;
            this.cbFilenameFilterMethod.Items.AddRange(new object[] {
            "Contains",
            "Starts with",
            "Exact match"});
            this.cbFilenameFilterMethod.Location = new System.Drawing.Point(192, 119);
            this.cbFilenameFilterMethod.Name = "cbFilenameFilterMethod";
            this.cbFilenameFilterMethod.Size = new System.Drawing.Size(96, 21);
            this.cbFilenameFilterMethod.TabIndex = 7;
            // 
            // cbFilenameFilterCulture
            // 
            this.cbFilenameFilterCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilenameFilterCulture.FormattingEnabled = true;
            this.cbFilenameFilterCulture.Items.AddRange(new object[] {
            "Current culture",
            "Invariant culture (English)",
            "Ordinal (English)"});
            this.cbFilenameFilterCulture.Location = new System.Drawing.Point(16, 145);
            this.cbFilenameFilterCulture.Name = "cbFilenameFilterCulture";
            this.cbFilenameFilterCulture.Size = new System.Drawing.Size(170, 21);
            this.cbFilenameFilterCulture.TabIndex = 8;
            // 
            // cbFilenameFilter
            // 
            this.cbFilenameFilter.AutoSize = true;
            this.cbFilenameFilter.Location = new System.Drawing.Point(16, 96);
            this.cbFilenameFilter.Name = "cbFilenameFilter";
            this.cbFilenameFilter.Size = new System.Drawing.Size(93, 17);
            this.cbFilenameFilter.TabIndex = 5;
            this.cbFilenameFilter.Text = "Filename filter:";
            this.cbFilenameFilter.UseVisualStyleBackColor = true;
            // 
            // cbFilenameFilterCase
            // 
            this.cbFilenameFilterCase.AutoSize = true;
            this.cbFilenameFilterCase.Location = new System.Drawing.Point(192, 146);
            this.cbFilenameFilterCase.Name = "cbFilenameFilterCase";
            this.cbFilenameFilterCase.Size = new System.Drawing.Size(94, 17);
            this.cbFilenameFilterCase.TabIndex = 9;
            this.cbFilenameFilterCase.Text = "Case sensitive";
            this.cbFilenameFilterCase.UseVisualStyleBackColor = true;
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.ssMain.Location = new System.Drawing.Point(0, 665);
            this.ssMain.Name = "ssMain";
            this.ssMain.Size = new System.Drawing.Size(920, 22);
            this.ssMain.TabIndex = 1;
            this.ssMain.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(39, 17);
            this.tsslStatus.Text = "Status";
            // 
            // gbFilters
            // 
            this.gbFilters.Controls.Add(this.btnRemoveFilters);
            this.gbFilters.Controls.Add(this.btnApplyFilters);
            this.gbFilters.Controls.Add(this.txtHostFilter);
            this.gbFilters.Controls.Add(this.cbTypeFilterSelection);
            this.gbFilters.Controls.Add(this.cbHostFilter);
            this.gbFilters.Controls.Add(this.cbTypeFilter);
            this.gbFilters.Controls.Add(this.dtpFilterFrom);
            this.gbFilters.Controls.Add(this.label1);
            this.gbFilters.Controls.Add(this.cbFilenameFilter);
            this.gbFilters.Controls.Add(this.label2);
            this.gbFilters.Controls.Add(this.cbFilenameFilterCase);
            this.gbFilters.Controls.Add(this.cbDateFilter);
            this.gbFilters.Controls.Add(this.dtpFilterTo);
            this.gbFilters.Controls.Add(this.cbFilenameFilterCulture);
            this.gbFilters.Controls.Add(this.txtFilenameFilter);
            this.gbFilters.Controls.Add(this.cbFilenameFilterMethod);
            this.gbFilters.Location = new System.Drawing.Point(8, 8);
            this.gbFilters.Name = "gbFilters";
            this.gbFilters.Size = new System.Drawing.Size(312, 272);
            this.gbFilters.TabIndex = 2;
            this.gbFilters.TabStop = false;
            this.gbFilters.Text = "Filters";
            // 
            // btnRemoveFilters
            // 
            this.btnRemoveFilters.Location = new System.Drawing.Point(160, 232);
            this.btnRemoveFilters.Name = "btnRemoveFilters";
            this.btnRemoveFilters.Size = new System.Drawing.Size(136, 24);
            this.btnRemoveFilters.TabIndex = 2;
            this.btnRemoveFilters.Text = "Remove filters";
            this.btnRemoveFilters.UseVisualStyleBackColor = true;
            this.btnRemoveFilters.Click += new System.EventHandler(this.btnRemoveFilters_Click);
            // 
            // txtHostFilter
            // 
            this.txtHostFilter.Location = new System.Drawing.Point(112, 199);
            this.txtHostFilter.Name = "txtHostFilter";
            this.txtHostFilter.Size = new System.Drawing.Size(128, 20);
            this.txtHostFilter.TabIndex = 13;
            // 
            // cbTypeFilterSelection
            // 
            this.cbTypeFilterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeFilterSelection.FormattingEnabled = true;
            this.cbTypeFilterSelection.Items.AddRange(new object[] {
            "Image",
            "File",
            "Text"});
            this.cbTypeFilterSelection.Location = new System.Drawing.Point(112, 172);
            this.cbTypeFilterSelection.Name = "cbTypeFilterSelection";
            this.cbTypeFilterSelection.Size = new System.Drawing.Size(128, 21);
            this.cbTypeFilterSelection.TabIndex = 11;
            // 
            // cbHostFilter
            // 
            this.cbHostFilter.AutoSize = true;
            this.cbHostFilter.Location = new System.Drawing.Point(16, 201);
            this.cbHostFilter.Name = "cbHostFilter";
            this.cbHostFilter.Size = new System.Drawing.Size(73, 17);
            this.cbHostFilter.TabIndex = 12;
            this.cbHostFilter.Text = "Host filter:";
            this.cbHostFilter.UseVisualStyleBackColor = true;
            // 
            // cbTypeFilter
            // 
            this.cbTypeFilter.AutoSize = true;
            this.cbTypeFilter.Location = new System.Drawing.Point(16, 174);
            this.cbTypeFilter.Name = "cbTypeFilter";
            this.cbTypeFilter.Size = new System.Drawing.Size(90, 17);
            this.cbTypeFilter.TabIndex = 10;
            this.cbTypeFilter.Text = "File type filter:";
            this.cbTypeFilter.UseVisualStyleBackColor = true;
            // 
            // lvHistory
            // 
            this.lvHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDateTime,
            this.chFilename,
            this.chType,
            this.chHost,
            this.chURL});
            this.lvHistory.FullRowSelect = true;
            this.lvHistory.HideSelection = false;
            this.lvHistory.Location = new System.Drawing.Point(8, 288);
            this.lvHistory.Name = "lvHistory";
            this.lvHistory.Size = new System.Drawing.Size(904, 368);
            this.lvHistory.TabIndex = 1;
            this.lvHistory.UseCompatibleStateImageBehavior = false;
            this.lvHistory.View = System.Windows.Forms.View.Details;
            this.lvHistory.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvHistory_ColumnClick);
            this.lvHistory.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvHistory_ItemSelectionChanged);
            this.lvHistory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvHistory_KeyDown);
            this.lvHistory.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvHistory_MouseDoubleClick);
            this.lvHistory.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvHistory_MouseUp);
            // 
            // chDateTime
            // 
            this.chDateTime.Text = "Date & time";
            this.chDateTime.Width = 122;
            // 
            // chFilename
            // 
            this.chFilename.Text = "Filename";
            this.chFilename.Width = 172;
            // 
            // chType
            // 
            this.chType.Text = "Type";
            this.chType.Width = 56;
            // 
            // chHost
            // 
            this.chHost.Text = "Host";
            this.chHost.Width = 95;
            // 
            // chURL
            // 
            this.chURL.Text = "URL";
            this.chURL.Width = 330;
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbThumbnail.BackColor = System.Drawing.Color.White;
            this.pbThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbThumbnail.DrawCheckeredBackground = true;
            this.pbThumbnail.FullscreenOnClick = true;
            this.pbThumbnail.Location = new System.Drawing.Point(328, 8);
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.Size = new System.Drawing.Size(584, 272);
            this.pbThumbnail.TabIndex = 1;
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 687);
            this.Controls.Add(this.lvHistory);
            this.Controls.Add(this.pbThumbnail);
            this.Controls.Add(this.gbFilters);
            this.Controls.Add(this.ssMain);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(925, 725);
            this.Name = "HistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "History";
            this.Shown += new System.EventHandler(this.HistoryForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HistoryForm_KeyDown);
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.gbFilters.ResumeLayout(false);
            this.gbFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private HelpersLib.MyListView lvHistory;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chDateTime;
        private System.Windows.Forms.ColumnHeader chType;
        private System.Windows.Forms.ColumnHeader chHost;
        private System.Windows.Forms.ColumnHeader chURL;
        private System.Windows.Forms.DateTimePicker dtpFilterFrom;
        private System.Windows.Forms.CheckBox cbDateFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFilterTo;
        private System.Windows.Forms.Button btnApplyFilters;
        private System.Windows.Forms.TextBox txtFilenameFilter;
        private System.Windows.Forms.ComboBox cbFilenameFilterMethod;
        private System.Windows.Forms.ComboBox cbFilenameFilterCulture;
        private System.Windows.Forms.CheckBox cbFilenameFilter;
        private System.Windows.Forms.CheckBox cbFilenameFilterCase;
        private HelpersLib.MyPictureBox pbThumbnail;
        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.GroupBox gbFilters;
        private System.Windows.Forms.Button btnRemoveFilters;
        private System.Windows.Forms.ComboBox cbTypeFilterSelection;
        private System.Windows.Forms.CheckBox cbHostFilter;
        private System.Windows.Forms.CheckBox cbTypeFilter;
        private System.Windows.Forms.TextBox txtHostFilter;
    }
}