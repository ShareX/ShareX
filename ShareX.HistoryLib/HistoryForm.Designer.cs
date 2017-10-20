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
            this.btnApplyFilters = new System.Windows.Forms.Button();
            this.scMain = new ShareX.HelpersLib.SplitContainerCustomSplitter();
            this.lvHistory = new ShareX.HistoryLib.HistoryListView();
            this.chIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbThumbnail = new ShareX.HelpersLib.MyPictureBox();
            this.gbFilters = new System.Windows.Forms.GroupBox();
            this.cbHostFilterSelection = new System.Windows.Forms.ComboBox();
            this.btnRemoveFilters = new System.Windows.Forms.Button();
            this.cbTypeFilterSelection = new System.Windows.Forms.ComboBox();
            this.cbHostFilter = new System.Windows.Forms.CheckBox();
            this.cbTypeFilter = new System.Windows.Forms.CheckBox();
            this.dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFilterFrom = new System.Windows.Forms.Label();
            this.cbFilenameFilter = new System.Windows.Forms.CheckBox();
            this.lblFilterTo = new System.Windows.Forms.Label();
            this.cbDateFilter = new System.Windows.Forms.CheckBox();
            this.dtpFilterTo = new System.Windows.Forms.DateTimePicker();
            this.txtFilenameFilter = new System.Windows.Forms.TextBox();
            this.cbFilenameFilterMethod = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.gbFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApplyFilters
            // 
            resources.ApplyResources(this.btnApplyFilters, "btnApplyFilters");
            this.btnApplyFilters.Name = "btnApplyFilters";
            this.btnApplyFilters.UseVisualStyleBackColor = true;
            this.btnApplyFilters.Click += new System.EventHandler(this.btnApplyFilters_Click);
            // 
            // scMain
            // 
            resources.ApplyResources(this.scMain, "scMain");
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.lvHistory);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pbThumbnail);
            this.scMain.Panel2.Controls.Add(this.gbFilters);
            this.scMain.SplitterColor = System.Drawing.Color.DarkGray;
            this.scMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.scMain_SplitterMoved);
            // 
            // lvHistory
            //
            this.lvHistory.AllowItemDrag = true;
            this.lvHistory.AllowColumnSort = true;
            this.lvHistory.AutoFillColumn = true;
            this.lvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chIcon,
            this.chDateTime,
            this.chFilename,
            this.chURL});
            resources.ApplyResources(this.lvHistory, "lvHistory");
            this.lvHistory.FullRowSelect = true;
            this.lvHistory.HideSelection = false;
            this.lvHistory.Name = "lvHistory";
            this.lvHistory.UseCompatibleStateImageBehavior = false;
            this.lvHistory.View = System.Windows.Forms.View.Details;
            this.lvHistory.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvHistory_ItemSelectionChanged);
            this.lvHistory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvHistory_KeyDown);
            this.lvHistory.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvHistory_MouseDoubleClick);
            this.lvHistory.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvHistory_MouseUp);
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
            // pbThumbnail
            // 
            resources.ApplyResources(this.pbThumbnail, "pbThumbnail");
            this.pbThumbnail.BackColor = System.Drawing.SystemColors.Window;
            this.pbThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbThumbnail.DrawCheckeredBackground = true;
            this.pbThumbnail.FullscreenOnClick = true;
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.ShowImageSizeLabel = true;
            // 
            // gbFilters
            // 
            resources.ApplyResources(this.gbFilters, "gbFilters");
            this.gbFilters.Controls.Add(this.cbHostFilterSelection);
            this.gbFilters.Controls.Add(this.btnRemoveFilters);
            this.gbFilters.Controls.Add(this.btnApplyFilters);
            this.gbFilters.Controls.Add(this.cbTypeFilterSelection);
            this.gbFilters.Controls.Add(this.cbHostFilter);
            this.gbFilters.Controls.Add(this.cbTypeFilter);
            this.gbFilters.Controls.Add(this.dtpFilterFrom);
            this.gbFilters.Controls.Add(this.lblFilterFrom);
            this.gbFilters.Controls.Add(this.cbFilenameFilter);
            this.gbFilters.Controls.Add(this.lblFilterTo);
            this.gbFilters.Controls.Add(this.cbDateFilter);
            this.gbFilters.Controls.Add(this.dtpFilterTo);
            this.gbFilters.Controls.Add(this.txtFilenameFilter);
            this.gbFilters.Controls.Add(this.cbFilenameFilterMethod);
            this.gbFilters.Name = "gbFilters";
            this.gbFilters.TabStop = false;
            // 
            // cbHostFilterSelection
            // 
            this.cbHostFilterSelection.FormattingEnabled = true;
            resources.ApplyResources(this.cbHostFilterSelection, "cbHostFilterSelection");
            this.cbHostFilterSelection.Name = "cbHostFilterSelection";
            // 
            // btnRemoveFilters
            // 
            resources.ApplyResources(this.btnRemoveFilters, "btnRemoveFilters");
            this.btnRemoveFilters.Name = "btnRemoveFilters";
            this.btnRemoveFilters.UseVisualStyleBackColor = true;
            this.btnRemoveFilters.Click += new System.EventHandler(this.btnRemoveFilters_Click);
            // 
            // cbTypeFilterSelection
            // 
            this.cbTypeFilterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeFilterSelection.FormattingEnabled = true;
            resources.ApplyResources(this.cbTypeFilterSelection, "cbTypeFilterSelection");
            this.cbTypeFilterSelection.Name = "cbTypeFilterSelection";
            // 
            // cbHostFilter
            // 
            resources.ApplyResources(this.cbHostFilter, "cbHostFilter");
            this.cbHostFilter.Name = "cbHostFilter";
            this.cbHostFilter.UseVisualStyleBackColor = true;
            // 
            // cbTypeFilter
            // 
            resources.ApplyResources(this.cbTypeFilter, "cbTypeFilter");
            this.cbTypeFilter.Name = "cbTypeFilter";
            this.cbTypeFilter.UseVisualStyleBackColor = true;
            // 
            // dtpFilterFrom
            // 
            resources.ApplyResources(this.dtpFilterFrom, "dtpFilterFrom");
            this.dtpFilterFrom.Name = "dtpFilterFrom";
            // 
            // lblFilterFrom
            // 
            resources.ApplyResources(this.lblFilterFrom, "lblFilterFrom");
            this.lblFilterFrom.Name = "lblFilterFrom";
            // 
            // cbFilenameFilter
            // 
            resources.ApplyResources(this.cbFilenameFilter, "cbFilenameFilter");
            this.cbFilenameFilter.Name = "cbFilenameFilter";
            this.cbFilenameFilter.UseVisualStyleBackColor = true;
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
            // 
            // dtpFilterTo
            // 
            resources.ApplyResources(this.dtpFilterTo, "dtpFilterTo");
            this.dtpFilterTo.Name = "dtpFilterTo";
            // 
            // txtFilenameFilter
            // 
            resources.ApplyResources(this.txtFilenameFilter, "txtFilenameFilter");
            this.txtFilenameFilter.Name = "txtFilenameFilter";
            this.txtFilenameFilter.TextChanged += new System.EventHandler(this.txtFilenameFilter_TextChanged);
            // 
            // cbFilenameFilterMethod
            // 
            this.cbFilenameFilterMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilenameFilterMethod.FormattingEnabled = true;
            this.cbFilenameFilterMethod.Items.AddRange(new object[] {
            resources.GetString("cbFilenameFilterMethod.Items"),
            resources.GetString("cbFilenameFilterMethod.Items1"),
            resources.GetString("cbFilenameFilterMethod.Items2"),
            resources.GetString("cbFilenameFilterMethod.Items3")});
            resources.ApplyResources(this.cbFilenameFilterMethod, "cbFilenameFilterMethod");
            this.cbFilenameFilterMethod.Name = "cbFilenameFilterMethod";
            // 
            // HistoryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.scMain);
            this.KeyPreview = true;
            this.Name = "HistoryForm";
            this.Shown += new System.EventHandler(this.HistoryForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HistoryForm_KeyDown);
            this.Resize += new System.EventHandler(this.HistoryForm_Resize);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.gbFilters.ResumeLayout(false);
            this.gbFilters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private ShareX.HistoryLib.HistoryListView lvHistory;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chDateTime;
        private System.Windows.Forms.ColumnHeader chURL;
        private System.Windows.Forms.DateTimePicker dtpFilterFrom;
        private System.Windows.Forms.CheckBox cbDateFilter;
        private System.Windows.Forms.Label lblFilterFrom;
        private System.Windows.Forms.Label lblFilterTo;
        private System.Windows.Forms.DateTimePicker dtpFilterTo;
        private System.Windows.Forms.Button btnApplyFilters;
        private System.Windows.Forms.TextBox txtFilenameFilter;
        private System.Windows.Forms.ComboBox cbFilenameFilterMethod;
        private System.Windows.Forms.CheckBox cbFilenameFilter;
        private ShareX.HelpersLib.MyPictureBox pbThumbnail;
        private System.Windows.Forms.GroupBox gbFilters;
        private System.Windows.Forms.Button btnRemoveFilters;
        private System.Windows.Forms.ComboBox cbTypeFilterSelection;
        private System.Windows.Forms.CheckBox cbHostFilter;
        private System.Windows.Forms.CheckBox cbTypeFilter;
        private System.Windows.Forms.ColumnHeader chIcon;
        private ShareX.HelpersLib.SplitContainerCustomSplitter scMain;
        private System.Windows.Forms.ComboBox cbHostFilterSelection;
    }
}