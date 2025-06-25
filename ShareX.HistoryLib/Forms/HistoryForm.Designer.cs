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
            this.tsbMediaImporter = new System.Windows.Forms.ToolStripButton();
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

            this.lvHistory.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
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
            this.tsbSettings,
            this.tsbMediaImporter});
            this.tsHistory.Name = "tsHistory";

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
            // tsbToggleMoreInfo
            // 
            tsbToggleMoreInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbToggleMoreInfo.Image = Properties.Resources.layout_header_3_mix;
            resources.ApplyResources(tsbToggleMoreInfo, "tsbToggleMoreInfo");
            tsbToggleMoreInfo.Name = "tsbToggleMoreInfo";
            tsbToggleMoreInfo.Click += tsbToggleMoreInfo_Click;
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
            // tsbSettings
            // 
            tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSettings.Image = Properties.Resources.gear;
            resources.ApplyResources(tsbSettings, "tsbSettings");
            tsbSettings.Name = "tsbSettings";
            tsbSettings.Click += tsbSettings_Click;
            // 
            // tsbMediaImporter
            // 
            this.tsbMediaImporter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMediaImporter.Image = global::ShareX.HistoryLib.Properties.Resources.ic_fluent_image_add_24_regular;
            resources.ApplyResources(this.tsbMediaImporter, "tsbMediaImporter");
            this.tsbMediaImporter.Name = "tsbMediaImporter";
            this.tsbMediaImporter.Click += new System.EventHandler(this.tsbMediaImporter_Click);
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
            scMain.Panel2.Controls.Add(scHistoryItemInfo);
            scMain.SplitterColor = System.Drawing.Color.White;
            scMain.SplitterLineColor = System.Drawing.Color.FromArgb(189, 189, 189);
            // 
            // scHistoryItemInfo
            // 
            resources.ApplyResources(scHistoryItemInfo, "scHistoryItemInfo");
            scHistoryItemInfo.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            scHistoryItemInfo.Name = "scHistoryItemInfo";
            // 
            // scHistoryItemInfo.Panel1
            // 
            scHistoryItemInfo.Panel1.Controls.Add(gbAdvancedSearch);
            scHistoryItemInfo.Panel1.Controls.Add(pbThumbnail);
            // 
            // scHistoryItemInfo.Panel2
            // 
            scHistoryItemInfo.Panel2.Controls.Add(pgHistoryItemInfo);
            scHistoryItemInfo.SplitterColor = System.Drawing.Color.White;
            scHistoryItemInfo.SplitterLineColor = System.Drawing.Color.FromArgb(189, 189, 189);
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
            // pgHistoryItemInfo
            // 
            pgHistoryItemInfo.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(pgHistoryItemInfo, "pgHistoryItemInfo");
            pgHistoryItemInfo.Name = "pgHistoryItemInfo";
            pgHistoryItemInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            pgHistoryItemInfo.ToolbarVisible = false;
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
            scHistoryItemInfo.Panel1.ResumeLayout(false);
            scHistoryItemInfo.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scHistoryItemInfo).EndInit();
            scHistoryItemInfo.ResumeLayout(false);
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
        private System.Windows.Forms.PropertyGrid pgHistoryItemInfo;
        private HelpersLib.SplitContainerCustomSplitter scHistoryItemInfo;
        private System.Windows.Forms.ToolStripButton tsbToggleMoreInfo;
        private System.Windows.Forms.ToolStripButton tsbShowStats;
        private System.Windows.Forms.Button btnAdvancedSearchClose;
        private System.Windows.Forms.ToolStripButton tsbMediaImporter;
    }
}