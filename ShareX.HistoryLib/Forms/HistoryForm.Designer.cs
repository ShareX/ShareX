namespace ShareX.HistoryLib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryForm));
            tscHistory = new System.Windows.Forms.ToolStripContainer();
            lvHistory = new ShareX.HelpersLib.MyListView();
            chIcon = new System.Windows.Forms.ColumnHeader();
            chDateTime = new System.Windows.Forms.ColumnHeader();
            chFilename = new System.Windows.Forms.ColumnHeader();
            chURL = new System.Windows.Forms.ColumnHeader();
            tsHistory = new System.Windows.Forms.ToolStrip();
            tslSearch = new System.Windows.Forms.ToolStripLabel();
            tstbSearch = new System.Windows.Forms.ToolStripTextBox();
            tsbSearch = new System.Windows.Forms.ToolStripButton();
            tsbAdvancedSearch = new System.Windows.Forms.ToolStripButton();
            tss1 = new System.Windows.Forms.ToolStripSeparator();
            tsbFavorites = new System.Windows.Forms.ToolStripButton();
            tsbShowStats = new System.Windows.Forms.ToolStripButton();
            tss2 = new System.Windows.Forms.ToolStripSeparator();
            tsbImportFolder = new System.Windows.Forms.ToolStripButton();
            tsbSettings = new System.Windows.Forms.ToolStripButton();
            gbAdvancedSearch = new System.Windows.Forms.GroupBox();
            btnAdvancedSearchClose = new System.Windows.Forms.Button();
            btnAdvancedSearchReset = new System.Windows.Forms.Button();
            lblURLFilter = new System.Windows.Forms.Label();
            txtURLFilter = new System.Windows.Forms.TextBox();
            lblFilenameFilter = new System.Windows.Forms.Label();
            cbHostFilterSelection = new System.Windows.Forms.ComboBox();
            cbTypeFilterSelection = new System.Windows.Forms.ComboBox();
            cbHostFilter = new System.Windows.Forms.CheckBox();
            cbTypeFilter = new System.Windows.Forms.CheckBox();
            dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
            lblFilterFrom = new System.Windows.Forms.Label();
            lblFilterTo = new System.Windows.Forms.Label();
            cbDateFilter = new System.Windows.Forms.CheckBox();
            dtpFilterTo = new System.Windows.Forms.DateTimePicker();
            txtFilenameFilter = new System.Windows.Forms.TextBox();
            scMain = new ShareX.HelpersLib.SplitContainerCustomSplitter();
            pbThumbnail = new ShareX.HelpersLib.MyPictureBox();
            tscHistory.ContentPanel.SuspendLayout();
            tscHistory.TopToolStripPanel.SuspendLayout();
            tscHistory.SuspendLayout();
            tsHistory.SuspendLayout();
            gbAdvancedSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scMain).BeginInit();
            scMain.Panel1.SuspendLayout();
            scMain.Panel2.SuspendLayout();
            scMain.SuspendLayout();
            SuspendLayout();
            // 
            // tscHistory
            // 
            // 
            // tscHistory.ContentPanel
            // 
            tscHistory.ContentPanel.Controls.Add(lvHistory);
            resources.ApplyResources(tscHistory.ContentPanel, "tscHistory.ContentPanel");
            resources.ApplyResources(tscHistory, "tscHistory");
            tscHistory.Name = "tscHistory";
            // 
            // tscHistory.TopToolStripPanel
            // 
            tscHistory.TopToolStripPanel.Controls.Add(tsHistory);
            // 
            // lvHistory
            // 
            lvHistory.AllowSelectAll = false;
            lvHistory.AutoFillColumn = true;
            lvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chIcon, chDateTime, chFilename, chURL });
            resources.ApplyResources(lvHistory, "lvHistory");
            lvHistory.FullRowSelect = true;
            lvHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvHistory.Name = "lvHistory";
            lvHistory.UseCompatibleStateImageBehavior = false;
            lvHistory.View = System.Windows.Forms.View.Details;
            lvHistory.VirtualMode = true;
            lvHistory.CacheVirtualItems += lvHistory_CacheVirtualItems;
            lvHistory.ItemDrag += lvHistory_ItemDrag;
            lvHistory.ItemSelectionChanged += lvHistory_ItemSelectionChanged;
            lvHistory.RetrieveVirtualItem += lvHistory_RetrieveVirtualItem;
            lvHistory.KeyDown += lvHistory_KeyDown;
            lvHistory.MouseDoubleClick += lvHistory_MouseDoubleClick;
            // 
            // chIcon
            // 
            resources.ApplyResources(chIcon, "chIcon");
            // 
            // chDateTime
            // 
            resources.ApplyResources(chDateTime, "chDateTime");
            // 
            // chFilename
            // 
            resources.ApplyResources(chFilename, "chFilename");
            // 
            // chURL
            // 
            resources.ApplyResources(chURL, "chURL");
            // 
            // tsHistory
            // 
            resources.ApplyResources(tsHistory, "tsHistory");
            tsHistory.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            tsHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tslSearch, tstbSearch, tsbSearch, tsbAdvancedSearch, tss1, tsbFavorites, tsbShowStats, tsbImportFolder, tss2, tsbSettings });
            tsHistory.Name = "tsHistory";
            // 
            // tslSearch
            // 
            tslSearch.Name = "tslSearch";
            resources.ApplyResources(tslSearch, "tslSearch");
            // 
            // tstbSearch
            // 
            tstbSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            tstbSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            tstbSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tstbSearch.Name = "tstbSearch";
            resources.ApplyResources(tstbSearch, "tstbSearch");
            tstbSearch.TextChanged += tstbSearch_TextChanged;
            // 
            // tsbSearch
            // 
            tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSearch.Image = Properties.Resources.magnifier;
            resources.ApplyResources(tsbSearch, "tsbSearch");
            tsbSearch.Name = "tsbSearch";
            tsbSearch.Click += tsbSearch_Click;
            // 
            // tsbAdvancedSearch
            // 
            tsbAdvancedSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbAdvancedSearch.Image = Properties.Resources.magnifier__plus;
            resources.ApplyResources(tsbAdvancedSearch, "tsbAdvancedSearch");
            tsbAdvancedSearch.Name = "tsbAdvancedSearch";
            tsbAdvancedSearch.Click += tsbAdvancedSearch_Click;
            // 
            // tss1
            // 
            tss1.Name = "tss1";
            resources.ApplyResources(tss1, "tss1");
            // 
            // tsbFavorites
            // 
            tsbFavorites.CheckOnClick = true;
            tsbFavorites.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbFavorites.Image = Properties.Resources.star;
            resources.ApplyResources(tsbFavorites, "tsbFavorites");
            tsbFavorites.Name = "tsbFavorites";
            tsbFavorites.Click += tsbFavorites_Click;
            // 
            // tsbShowStats
            // 
            tsbShowStats.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbShowStats.Image = Properties.Resources.chart;
            resources.ApplyResources(tsbShowStats, "tsbShowStats");
            tsbShowStats.Name = "tsbShowStats";
            tsbShowStats.Click += tsbShowStats_Click;
            // 
            // tss2
            // 
            tss2.Name = "tss2";
            resources.ApplyResources(tss2, "tss2");
            // 
            // tsbImportFolder
            // 
            tsbImportFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbImportFolder.Image = Properties.Resources.folder_search_result;
            resources.ApplyResources(tsbImportFolder, "tsbImportFolder");
            tsbImportFolder.Name = "tsbImportFolder";
            tsbImportFolder.Click += tsbImportFolder_Click;
            // 
            // tsbSettings
            // 
            tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSettings.Image = Properties.Resources.gear;
            resources.ApplyResources(tsbSettings, "tsbSettings");
            tsbSettings.Name = "tsbSettings";
            tsbSettings.Click += tsbSettings_Click;
            // 
            // gbAdvancedSearch
            // 
            gbAdvancedSearch.Controls.Add(btnAdvancedSearchClose);
            gbAdvancedSearch.Controls.Add(btnAdvancedSearchReset);
            gbAdvancedSearch.Controls.Add(lblURLFilter);
            gbAdvancedSearch.Controls.Add(txtURLFilter);
            gbAdvancedSearch.Controls.Add(lblFilenameFilter);
            gbAdvancedSearch.Controls.Add(cbHostFilterSelection);
            gbAdvancedSearch.Controls.Add(cbTypeFilterSelection);
            gbAdvancedSearch.Controls.Add(cbHostFilter);
            gbAdvancedSearch.Controls.Add(cbTypeFilter);
            gbAdvancedSearch.Controls.Add(dtpFilterFrom);
            gbAdvancedSearch.Controls.Add(lblFilterFrom);
            gbAdvancedSearch.Controls.Add(lblFilterTo);
            gbAdvancedSearch.Controls.Add(cbDateFilter);
            gbAdvancedSearch.Controls.Add(dtpFilterTo);
            gbAdvancedSearch.Controls.Add(txtFilenameFilter);
            resources.ApplyResources(gbAdvancedSearch, "gbAdvancedSearch");
            gbAdvancedSearch.Name = "gbAdvancedSearch";
            gbAdvancedSearch.TabStop = false;
            // 
            // btnAdvancedSearchClose
            // 
            resources.ApplyResources(btnAdvancedSearchClose, "btnAdvancedSearchClose");
            btnAdvancedSearchClose.Name = "btnAdvancedSearchClose";
            btnAdvancedSearchClose.UseVisualStyleBackColor = true;
            btnAdvancedSearchClose.Click += btnAdvancedSearchClose_Click;
            // 
            // btnAdvancedSearchReset
            // 
            resources.ApplyResources(btnAdvancedSearchReset, "btnAdvancedSearchReset");
            btnAdvancedSearchReset.Name = "btnAdvancedSearchReset";
            btnAdvancedSearchReset.UseVisualStyleBackColor = true;
            btnAdvancedSearchReset.Click += btnAdvancedSearchReset_Click;
            // 
            // lblURLFilter
            // 
            resources.ApplyResources(lblURLFilter, "lblURLFilter");
            lblURLFilter.Name = "lblURLFilter";
            // 
            // txtURLFilter
            // 
            resources.ApplyResources(txtURLFilter, "txtURLFilter");
            txtURLFilter.Name = "txtURLFilter";
            txtURLFilter.TextChanged += AdvancedFilter_ValueChanged;
            // 
            // lblFilenameFilter
            // 
            resources.ApplyResources(lblFilenameFilter, "lblFilenameFilter");
            lblFilenameFilter.Name = "lblFilenameFilter";
            // 
            // cbHostFilterSelection
            // 
            cbHostFilterSelection.FormattingEnabled = true;
            resources.ApplyResources(cbHostFilterSelection, "cbHostFilterSelection");
            cbHostFilterSelection.Name = "cbHostFilterSelection";
            cbHostFilterSelection.SelectedIndexChanged += AdvancedFilter_ValueChanged;
            // 
            // cbTypeFilterSelection
            // 
            cbTypeFilterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTypeFilterSelection.FormattingEnabled = true;
            resources.ApplyResources(cbTypeFilterSelection, "cbTypeFilterSelection");
            cbTypeFilterSelection.Name = "cbTypeFilterSelection";
            cbTypeFilterSelection.SelectedIndexChanged += AdvancedFilter_ValueChanged;
            // 
            // cbHostFilter
            // 
            resources.ApplyResources(cbHostFilter, "cbHostFilter");
            cbHostFilter.Name = "cbHostFilter";
            cbHostFilter.UseVisualStyleBackColor = true;
            cbHostFilter.CheckedChanged += AdvancedFilter_ValueChanged;
            // 
            // cbTypeFilter
            // 
            resources.ApplyResources(cbTypeFilter, "cbTypeFilter");
            cbTypeFilter.Name = "cbTypeFilter";
            cbTypeFilter.UseVisualStyleBackColor = true;
            cbTypeFilter.CheckedChanged += AdvancedFilter_ValueChanged;
            // 
            // dtpFilterFrom
            // 
            resources.ApplyResources(dtpFilterFrom, "dtpFilterFrom");
            dtpFilterFrom.Name = "dtpFilterFrom";
            dtpFilterFrom.ValueChanged += AdvancedFilter_ValueChanged;
            // 
            // lblFilterFrom
            // 
            resources.ApplyResources(lblFilterFrom, "lblFilterFrom");
            lblFilterFrom.Name = "lblFilterFrom";
            // 
            // lblFilterTo
            // 
            resources.ApplyResources(lblFilterTo, "lblFilterTo");
            lblFilterTo.Name = "lblFilterTo";
            // 
            // cbDateFilter
            // 
            resources.ApplyResources(cbDateFilter, "cbDateFilter");
            cbDateFilter.Name = "cbDateFilter";
            cbDateFilter.UseVisualStyleBackColor = true;
            cbDateFilter.CheckedChanged += AdvancedFilter_ValueChanged;
            // 
            // dtpFilterTo
            // 
            resources.ApplyResources(dtpFilterTo, "dtpFilterTo");
            dtpFilterTo.Name = "dtpFilterTo";
            dtpFilterTo.ValueChanged += AdvancedFilter_ValueChanged;
            // 
            // txtFilenameFilter
            // 
            resources.ApplyResources(txtFilenameFilter, "txtFilenameFilter");
            txtFilenameFilter.Name = "txtFilenameFilter";
            txtFilenameFilter.TextChanged += AdvancedFilter_ValueChanged;
            // 
            // scMain
            // 
            resources.ApplyResources(scMain, "scMain");
            scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            scMain.Panel1.Controls.Add(tscHistory);
            // 
            // scMain.Panel2
            // 
            scMain.Panel2.Controls.Add(gbAdvancedSearch);
            scMain.Panel2.Controls.Add(pbThumbnail);
            scMain.SplitterColor = System.Drawing.Color.White;
            scMain.SplitterLineColor = System.Drawing.Color.FromArgb(189, 189, 189);
            // 
            // pbThumbnail
            // 
            pbThumbnail.BackColor = System.Drawing.SystemColors.Window;
            pbThumbnail.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(pbThumbnail, "pbThumbnail");
            pbThumbnail.DrawCheckeredBackground = true;
            pbThumbnail.Name = "pbThumbnail";
            pbThumbnail.PictureBoxBackColor = System.Drawing.SystemColors.Control;
            pbThumbnail.ShowImageSizeLabel = true;
            pbThumbnail.MouseDown += pbThumbnail_MouseDown;
            // 
            // HistoryForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(scMain);
            Name = "HistoryForm";
            FormClosing += HistoryForm_FormClosing;
            Shown += HistoryForm_Shown;
            KeyDown += HistoryForm_KeyDown;
            Resize += HistoryForm_Resize;
            tscHistory.ContentPanel.ResumeLayout(false);
            tscHistory.TopToolStripPanel.ResumeLayout(false);
            tscHistory.TopToolStripPanel.PerformLayout();
            tscHistory.ResumeLayout(false);
            tscHistory.PerformLayout();
            tsHistory.ResumeLayout(false);
            tsHistory.PerformLayout();
            gbAdvancedSearch.ResumeLayout(false);
            gbAdvancedSearch.PerformLayout();
            scMain.Panel1.ResumeLayout(false);
            scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scMain).EndInit();
            scMain.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private ShareX.HelpersLib.MyListView lvHistory;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chDateTime;
        private System.Windows.Forms.ColumnHeader chURL;
        private System.Windows.Forms.ColumnHeader chIcon;
        private ShareX.HelpersLib.SplitContainerCustomSplitter scMain;
        private HelpersLib.MyPictureBox pbThumbnail;
        private System.Windows.Forms.GroupBox gbAdvancedSearch;
        private System.Windows.Forms.ComboBox cbHostFilterSelection;
        private System.Windows.Forms.ComboBox cbTypeFilterSelection;
        private System.Windows.Forms.CheckBox cbHostFilter;
        private System.Windows.Forms.CheckBox cbTypeFilter;
        private System.Windows.Forms.DateTimePicker dtpFilterFrom;
        private System.Windows.Forms.Label lblFilterFrom;
        private System.Windows.Forms.Label lblFilterTo;
        private System.Windows.Forms.CheckBox cbDateFilter;
        private System.Windows.Forms.DateTimePicker dtpFilterTo;
        private System.Windows.Forms.TextBox txtFilenameFilter;
        private System.Windows.Forms.Label lblFilenameFilter;
        private System.Windows.Forms.Label lblURLFilter;
        private System.Windows.Forms.TextBox txtURLFilter;
        private System.Windows.Forms.ToolStripContainer tscHistory;
        private System.Windows.Forms.ToolStrip tsHistory;
        private System.Windows.Forms.ToolStripLabel tslSearch;
        private System.Windows.Forms.ToolStripTextBox tstbSearch;
        private System.Windows.Forms.ToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripButton tsbSettings;
        private System.Windows.Forms.ToolStripButton tsbAdvancedSearch;
        private System.Windows.Forms.Button btnAdvancedSearchReset;
        private System.Windows.Forms.ToolStripSeparator tss2;
        private System.Windows.Forms.ToolStripButton tsbShowStats;
        private System.Windows.Forms.Button btnAdvancedSearchClose;
        private System.Windows.Forms.ToolStripButton tsbFavorites;
        private System.Windows.Forms.ToolStripButton tsbImportFolder;
    }
}