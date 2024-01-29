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
            this.tscHistory = new System.Windows.Forms.ToolStripContainer();
            this.lvHistory = new ShareX.HelpersLib.MyListView();
            this.chIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tsHistory = new System.Windows.Forms.ToolStrip();
            this.tslSearch = new System.Windows.Forms.ToolStripLabel();
            this.tstbSearch = new System.Windows.Forms.ToolStripTextBox();
            this.tsbSearch = new System.Windows.Forms.ToolStripButton();
            this.tsbAdvancedSearch = new System.Windows.Forms.ToolStripButton();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToggleMoreInfo = new System.Windows.Forms.ToolStripButton();
            this.tsbShowStats = new System.Windows.Forms.ToolStripButton();
            this.tss2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSettings = new System.Windows.Forms.ToolStripButton();
            this.gbAdvancedSearch = new System.Windows.Forms.GroupBox();
            this.btnAdvancedSearchClose = new System.Windows.Forms.Button();
            this.btnAdvancedSearchReset = new System.Windows.Forms.Button();
            this.lblURLFilter = new System.Windows.Forms.Label();
            this.txtURLFilter = new System.Windows.Forms.TextBox();
            this.lblFilenameFilter = new System.Windows.Forms.Label();
            this.cbHostFilterSelection = new System.Windows.Forms.ComboBox();
            this.cbTypeFilterSelection = new System.Windows.Forms.ComboBox();
            this.cbHostFilter = new System.Windows.Forms.CheckBox();
            this.cbTypeFilter = new System.Windows.Forms.CheckBox();
            this.dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFilterFrom = new System.Windows.Forms.Label();
            this.lblFilterTo = new System.Windows.Forms.Label();
            this.cbDateFilter = new System.Windows.Forms.CheckBox();
            this.dtpFilterTo = new System.Windows.Forms.DateTimePicker();
            this.txtFilenameFilter = new System.Windows.Forms.TextBox();
            this.scMain = new ShareX.HelpersLib.SplitContainerCustomSplitter();
            this.scHistoryItemInfo = new ShareX.HelpersLib.SplitContainerCustomSplitter();
            this.pbThumbnail = new ShareX.HelpersLib.MyPictureBox();
            this.pgHistoryItemInfo = new System.Windows.Forms.PropertyGrid();
            this.tscHistory.ContentPanel.SuspendLayout();
            this.tscHistory.TopToolStripPanel.SuspendLayout();
            this.tscHistory.SuspendLayout();
            this.tsHistory.SuspendLayout();
            this.gbAdvancedSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scHistoryItemInfo)).BeginInit();
            this.scHistoryItemInfo.Panel1.SuspendLayout();
            this.scHistoryItemInfo.Panel2.SuspendLayout();
            this.scHistoryItemInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscHistory
            // 
            // 
            // tscHistory.ContentPanel
            // 
            this.tscHistory.ContentPanel.Controls.Add(this.lvHistory);
            resources.ApplyResources(this.tscHistory.ContentPanel, "tscHistory.ContentPanel");
            resources.ApplyResources(this.tscHistory, "tscHistory");
            this.tscHistory.Name = "tscHistory";
            // 
            // tscHistory.TopToolStripPanel
            // 
            this.tscHistory.TopToolStripPanel.Controls.Add(this.tsHistory);
            // 
            // lvHistory
            // 
            this.lvHistory.AllowSelectAll = false;
            this.lvHistory.AutoFillColumn = true;
            this.lvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chIcon,
            this.chDateTime,
            this.chFilename,
            this.chURL});
            resources.ApplyResources(this.lvHistory, "lvHistory");
            this.lvHistory.FullRowSelect = true;
            this.lvHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvHistory.HideSelection = false;
            this.lvHistory.Name = "lvHistory";
            this.lvHistory.UseCompatibleStateImageBehavior = false;
            this.lvHistory.View = System.Windows.Forms.View.Details;
            this.lvHistory.VirtualMode = true;
            this.lvHistory.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.lvHistory_CacheVirtualItems);
            this.lvHistory.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvHistory_ItemDrag);
            this.lvHistory.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvHistory_ItemSelectionChanged);
            this.lvHistory.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvHistory_RetrieveVirtualItem);
            this.lvHistory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvHistory_KeyDown);
            this.lvHistory.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvHistory_MouseDoubleClick);
            // 
            // chIcon
            // 
            resources.ApplyResources(this.chIcon, "chIcon");
            // 
            // chDateTime
            // 
            resources.ApplyResources(this.chDateTime, "chDateTime");
            // 
            // chFilename
            // 
            resources.ApplyResources(this.chFilename, "chFilename");
            // 
            // chURL
            // 
            resources.ApplyResources(this.chURL, "chURL");
            // 
            // tsHistory
            // 
            resources.ApplyResources(this.tsHistory, "tsHistory");
            this.tsHistory.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSearch,
            this.tstbSearch,
            this.tsbSearch,
            this.tsbAdvancedSearch,
            this.tss1,
            this.tsbToggleMoreInfo,
            this.tsbShowStats,
            this.tss2,
            this.tsbSettings});
            this.tsHistory.Name = "tsHistory";
            // 
            // tslSearch
            // 
            this.tslSearch.Name = "tslSearch";
            resources.ApplyResources(this.tslSearch, "tslSearch");
            // 
            // tstbSearch
            // 
            this.tstbSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.tstbSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tstbSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tstbSearch, "tstbSearch");
            this.tstbSearch.Name = "tstbSearch";
            this.tstbSearch.TextChanged += new System.EventHandler(this.tstbSearch_TextChanged);
            // 
            // tsbSearch
            // 
            this.tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSearch.Image = global::ShareX.HistoryLib.Properties.Resources.magnifier;
            resources.ApplyResources(this.tsbSearch, "tsbSearch");
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Click += new System.EventHandler(this.tsbSearch_Click);
            // 
            // tsbAdvancedSearch
            // 
            this.tsbAdvancedSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdvancedSearch.Image = global::ShareX.HistoryLib.Properties.Resources.magnifier__plus;
            resources.ApplyResources(this.tsbAdvancedSearch, "tsbAdvancedSearch");
            this.tsbAdvancedSearch.Name = "tsbAdvancedSearch";
            this.tsbAdvancedSearch.Click += new System.EventHandler(this.tsbAdvancedSearch_Click);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            resources.ApplyResources(this.tss1, "tss1");
            // 
            // tsbToggleMoreInfo
            // 
            this.tsbToggleMoreInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToggleMoreInfo.Image = global::ShareX.HistoryLib.Properties.Resources.layout_header_3_mix;
            resources.ApplyResources(this.tsbToggleMoreInfo, "tsbToggleMoreInfo");
            this.tsbToggleMoreInfo.Name = "tsbToggleMoreInfo";
            this.tsbToggleMoreInfo.Click += new System.EventHandler(this.tsbToggleMoreInfo_Click);
            // 
            // tsbShowStats
            // 
            this.tsbShowStats.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowStats.Image = global::ShareX.HistoryLib.Properties.Resources.chart;
            resources.ApplyResources(this.tsbShowStats, "tsbShowStats");
            this.tsbShowStats.Name = "tsbShowStats";
            this.tsbShowStats.Click += new System.EventHandler(this.tsbShowStats_Click);
            // 
            // tss2
            // 
            this.tss2.Name = "tss2";
            resources.ApplyResources(this.tss2, "tss2");
            // 
            // tsbSettings
            // 
            this.tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSettings.Image = global::ShareX.HistoryLib.Properties.Resources.gear;
            resources.ApplyResources(this.tsbSettings, "tsbSettings");
            this.tsbSettings.Name = "tsbSettings";
            this.tsbSettings.Click += new System.EventHandler(this.tsbSettings_Click);
            // 
            // gbAdvancedSearch
            // 
            this.gbAdvancedSearch.Controls.Add(this.btnAdvancedSearchClose);
            this.gbAdvancedSearch.Controls.Add(this.btnAdvancedSearchReset);
            this.gbAdvancedSearch.Controls.Add(this.lblURLFilter);
            this.gbAdvancedSearch.Controls.Add(this.txtURLFilter);
            this.gbAdvancedSearch.Controls.Add(this.lblFilenameFilter);
            this.gbAdvancedSearch.Controls.Add(this.cbHostFilterSelection);
            this.gbAdvancedSearch.Controls.Add(this.cbTypeFilterSelection);
            this.gbAdvancedSearch.Controls.Add(this.cbHostFilter);
            this.gbAdvancedSearch.Controls.Add(this.cbTypeFilter);
            this.gbAdvancedSearch.Controls.Add(this.dtpFilterFrom);
            this.gbAdvancedSearch.Controls.Add(this.lblFilterFrom);
            this.gbAdvancedSearch.Controls.Add(this.lblFilterTo);
            this.gbAdvancedSearch.Controls.Add(this.cbDateFilter);
            this.gbAdvancedSearch.Controls.Add(this.dtpFilterTo);
            this.gbAdvancedSearch.Controls.Add(this.txtFilenameFilter);
            resources.ApplyResources(this.gbAdvancedSearch, "gbAdvancedSearch");
            this.gbAdvancedSearch.Name = "gbAdvancedSearch";
            this.gbAdvancedSearch.TabStop = false;
            // 
            // btnAdvancedSearchClose
            // 
            resources.ApplyResources(this.btnAdvancedSearchClose, "btnAdvancedSearchClose");
            this.btnAdvancedSearchClose.Name = "btnAdvancedSearchClose";
            this.btnAdvancedSearchClose.UseVisualStyleBackColor = true;
            this.btnAdvancedSearchClose.Click += new System.EventHandler(this.btnAdvancedSearchClose_Click);
            // 
            // btnAdvancedSearchReset
            // 
            resources.ApplyResources(this.btnAdvancedSearchReset, "btnAdvancedSearchReset");
            this.btnAdvancedSearchReset.Name = "btnAdvancedSearchReset";
            this.btnAdvancedSearchReset.UseVisualStyleBackColor = true;
            this.btnAdvancedSearchReset.Click += new System.EventHandler(this.btnAdvancedSearchReset_Click);
            // 
            // lblURLFilter
            // 
            resources.ApplyResources(this.lblURLFilter, "lblURLFilter");
            this.lblURLFilter.Name = "lblURLFilter";
            // 
            // txtURLFilter
            // 
            resources.ApplyResources(this.txtURLFilter, "txtURLFilter");
            this.txtURLFilter.Name = "txtURLFilter";
            this.txtURLFilter.TextChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // lblFilenameFilter
            // 
            resources.ApplyResources(this.lblFilenameFilter, "lblFilenameFilter");
            this.lblFilenameFilter.Name = "lblFilenameFilter";
            // 
            // cbHostFilterSelection
            // 
            this.cbHostFilterSelection.FormattingEnabled = true;
            resources.ApplyResources(this.cbHostFilterSelection, "cbHostFilterSelection");
            this.cbHostFilterSelection.Name = "cbHostFilterSelection";
            this.cbHostFilterSelection.SelectedIndexChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // cbTypeFilterSelection
            // 
            this.cbTypeFilterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeFilterSelection.FormattingEnabled = true;
            resources.ApplyResources(this.cbTypeFilterSelection, "cbTypeFilterSelection");
            this.cbTypeFilterSelection.Name = "cbTypeFilterSelection";
            this.cbTypeFilterSelection.SelectedIndexChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // cbHostFilter
            // 
            resources.ApplyResources(this.cbHostFilter, "cbHostFilter");
            this.cbHostFilter.Name = "cbHostFilter";
            this.cbHostFilter.UseVisualStyleBackColor = true;
            this.cbHostFilter.CheckedChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // cbTypeFilter
            // 
            resources.ApplyResources(this.cbTypeFilter, "cbTypeFilter");
            this.cbTypeFilter.Name = "cbTypeFilter";
            this.cbTypeFilter.UseVisualStyleBackColor = true;
            this.cbTypeFilter.CheckedChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // dtpFilterFrom
            // 
            resources.ApplyResources(this.dtpFilterFrom, "dtpFilterFrom");
            this.dtpFilterFrom.Name = "dtpFilterFrom";
            this.dtpFilterFrom.ValueChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // lblFilterFrom
            // 
            resources.ApplyResources(this.lblFilterFrom, "lblFilterFrom");
            this.lblFilterFrom.Name = "lblFilterFrom";
            // 
            // lblFilterTo
            // 
            resources.ApplyResources(this.lblFilterTo, "lblFilterTo");
            this.lblFilterTo.Name = "lblFilterTo";
            // 
            // cbDateFilter
            // 
            resources.ApplyResources(this.cbDateFilter, "cbDateFilter");
            this.cbDateFilter.Name = "cbDateFilter";
            this.cbDateFilter.UseVisualStyleBackColor = true;
            this.cbDateFilter.CheckedChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // dtpFilterTo
            // 
            resources.ApplyResources(this.dtpFilterTo, "dtpFilterTo");
            this.dtpFilterTo.Name = "dtpFilterTo";
            this.dtpFilterTo.ValueChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // txtFilenameFilter
            // 
            resources.ApplyResources(this.txtFilenameFilter, "txtFilenameFilter");
            this.txtFilenameFilter.Name = "txtFilenameFilter";
            this.txtFilenameFilter.TextChanged += new System.EventHandler(this.AdvancedFilter_ValueChanged);
            // 
            // scMain
            // 
            resources.ApplyResources(this.scMain, "scMain");
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.tscHistory);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.scHistoryItemInfo);
            this.scMain.SplitterColor = System.Drawing.Color.White;
            this.scMain.SplitterLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            // 
            // scHistoryItemInfo
            // 
            resources.ApplyResources(this.scHistoryItemInfo, "scHistoryItemInfo");
            this.scHistoryItemInfo.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scHistoryItemInfo.Name = "scHistoryItemInfo";
            // 
            // scHistoryItemInfo.Panel1
            // 
            this.scHistoryItemInfo.Panel1.Controls.Add(this.gbAdvancedSearch);
            this.scHistoryItemInfo.Panel1.Controls.Add(this.pbThumbnail);
            // 
            // scHistoryItemInfo.Panel2
            // 
            this.scHistoryItemInfo.Panel2.Controls.Add(this.pgHistoryItemInfo);
            this.scHistoryItemInfo.SplitterColor = System.Drawing.Color.White;
            this.scHistoryItemInfo.SplitterLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.BackColor = System.Drawing.SystemColors.Window;
            this.pbThumbnail.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbThumbnail, "pbThumbnail");
            this.pbThumbnail.DrawCheckeredBackground = true;
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.PictureBoxBackColor = System.Drawing.SystemColors.Control;
            this.pbThumbnail.ShowImageSizeLabel = true;
            this.pbThumbnail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbThumbnail_MouseDown);
            // 
            // pgHistoryItemInfo
            // 
            resources.ApplyResources(this.pgHistoryItemInfo, "pgHistoryItemInfo");
            this.pgHistoryItemInfo.Name = "pgHistoryItemInfo";
            this.pgHistoryItemInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgHistoryItemInfo.ToolbarVisible = false;
            // 
            // HistoryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.scMain);
            this.Name = "HistoryForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HistoryForm_FormClosing);
            this.Shown += new System.EventHandler(this.HistoryForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HistoryForm_KeyDown);
            this.Resize += new System.EventHandler(this.HistoryForm_Resize);
            this.tscHistory.ContentPanel.ResumeLayout(false);
            this.tscHistory.TopToolStripPanel.ResumeLayout(false);
            this.tscHistory.TopToolStripPanel.PerformLayout();
            this.tscHistory.ResumeLayout(false);
            this.tscHistory.PerformLayout();
            this.tsHistory.ResumeLayout(false);
            this.tsHistory.PerformLayout();
            this.gbAdvancedSearch.ResumeLayout(false);
            this.gbAdvancedSearch.PerformLayout();
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.scHistoryItemInfo.Panel1.ResumeLayout(false);
            this.scHistoryItemInfo.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scHistoryItemInfo)).EndInit();
            this.scHistoryItemInfo.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.PropertyGrid pgHistoryItemInfo;
        private HelpersLib.SplitContainerCustomSplitter scHistoryItemInfo;
        private System.Windows.Forms.ToolStripButton tsbToggleMoreInfo;
        private System.Windows.Forms.ToolStripButton tsbShowStats;
        private System.Windows.Forms.Button btnAdvancedSearchClose;
    }
}