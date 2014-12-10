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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryForm));
            this.dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
            this.cbDateFilter = new System.Windows.Forms.CheckBox();
            this.lblFilterFrom = new System.Windows.Forms.Label();
            this.lblFilterTo = new System.Windows.Forms.Label();
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
            resources.ApplyResources(this.dtpFilterFrom, "dtpFilterFrom");
            this.dtpFilterFrom.Name = "dtpFilterFrom";
            // 
            // cbDateFilter
            // 
            resources.ApplyResources(this.cbDateFilter, "cbDateFilter");
            this.cbDateFilter.Name = "cbDateFilter";
            this.cbDateFilter.UseVisualStyleBackColor = true;
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
            // dtpFilterTo
            // 
            resources.ApplyResources(this.dtpFilterTo, "dtpFilterTo");
            this.dtpFilterTo.Name = "dtpFilterTo";
            // 
            // btnApplyFilters
            // 
            resources.ApplyResources(this.btnApplyFilters, "btnApplyFilters");
            this.btnApplyFilters.Name = "btnApplyFilters";
            this.btnApplyFilters.UseVisualStyleBackColor = true;
            this.btnApplyFilters.Click += new System.EventHandler(this.btnApplyFilters_Click);
            // 
            // txtFilenameFilter
            // 
            resources.ApplyResources(this.txtFilenameFilter, "txtFilenameFilter");
            this.txtFilenameFilter.Name = "txtFilenameFilter";
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
            // cbFilenameFilterCulture
            // 
            this.cbFilenameFilterCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilenameFilterCulture.FormattingEnabled = true;
            this.cbFilenameFilterCulture.Items.AddRange(new object[] {
            resources.GetString("cbFilenameFilterCulture.Items"),
            resources.GetString("cbFilenameFilterCulture.Items1"),
            resources.GetString("cbFilenameFilterCulture.Items2")});
            resources.ApplyResources(this.cbFilenameFilterCulture, "cbFilenameFilterCulture");
            this.cbFilenameFilterCulture.Name = "cbFilenameFilterCulture";
            // 
            // cbFilenameFilter
            // 
            resources.ApplyResources(this.cbFilenameFilter, "cbFilenameFilter");
            this.cbFilenameFilter.Name = "cbFilenameFilter";
            this.cbFilenameFilter.UseVisualStyleBackColor = true;
            // 
            // cbFilenameFilterCase
            // 
            resources.ApplyResources(this.cbFilenameFilterCase, "cbFilenameFilterCase");
            this.cbFilenameFilterCase.Name = "cbFilenameFilterCase";
            this.cbFilenameFilterCase.UseVisualStyleBackColor = true;
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            resources.ApplyResources(this.ssMain, "ssMain");
            this.ssMain.Name = "ssMain";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            resources.ApplyResources(this.tsslStatus, "tsslStatus");
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
            this.gbFilters.Controls.Add(this.lblFilterFrom);
            this.gbFilters.Controls.Add(this.cbFilenameFilter);
            this.gbFilters.Controls.Add(this.lblFilterTo);
            this.gbFilters.Controls.Add(this.cbFilenameFilterCase);
            this.gbFilters.Controls.Add(this.cbDateFilter);
            this.gbFilters.Controls.Add(this.dtpFilterTo);
            this.gbFilters.Controls.Add(this.cbFilenameFilterCulture);
            this.gbFilters.Controls.Add(this.txtFilenameFilter);
            this.gbFilters.Controls.Add(this.cbFilenameFilterMethod);
            resources.ApplyResources(this.gbFilters, "gbFilters");
            this.gbFilters.Name = "gbFilters";
            this.gbFilters.TabStop = false;
            // 
            // btnRemoveFilters
            // 
            resources.ApplyResources(this.btnRemoveFilters, "btnRemoveFilters");
            this.btnRemoveFilters.Name = "btnRemoveFilters";
            this.btnRemoveFilters.UseVisualStyleBackColor = true;
            this.btnRemoveFilters.Click += new System.EventHandler(this.btnRemoveFilters_Click);
            // 
            // txtHostFilter
            // 
            resources.ApplyResources(this.txtHostFilter, "txtHostFilter");
            this.txtHostFilter.Name = "txtHostFilter";
            // 
            // cbTypeFilterSelection
            // 
            this.cbTypeFilterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeFilterSelection.FormattingEnabled = true;
            this.cbTypeFilterSelection.Items.AddRange(new object[] {
            resources.GetString("cbTypeFilterSelection.Items"),
            resources.GetString("cbTypeFilterSelection.Items1"),
            resources.GetString("cbTypeFilterSelection.Items2")});
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
            // lvHistory
            // 
            this.lvHistory.AllowColumnSort = true;
            resources.ApplyResources(this.lvHistory, "lvHistory");
            this.lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDateTime,
            this.chFilename,
            this.chType,
            this.chHost,
            this.chURL});
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
            // chDateTime
            // 
            resources.ApplyResources(this.chDateTime, "chDateTime");
            // 
            // chFilename
            // 
            resources.ApplyResources(this.chFilename, "chFilename");
            // 
            // chType
            // 
            resources.ApplyResources(this.chType, "chType");
            // 
            // chHost
            // 
            resources.ApplyResources(this.chHost, "chHost");
            // 
            // chURL
            // 
            resources.ApplyResources(this.chURL, "chURL");
            // 
            // pbThumbnail
            // 
            resources.ApplyResources(this.pbThumbnail, "pbThumbnail");
            this.pbThumbnail.BackColor = System.Drawing.Color.White;
            this.pbThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbThumbnail.DrawCheckeredBackground = true;
            this.pbThumbnail.FullscreenOnClick = true;
            this.pbThumbnail.Name = "pbThumbnail";
            // 
            // HistoryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvHistory);
            this.Controls.Add(this.pbThumbnail);
            this.Controls.Add(this.gbFilters);
            this.Controls.Add(this.ssMain);
            this.KeyPreview = true;
            this.Name = "HistoryForm";
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
        private System.Windows.Forms.Label lblFilterFrom;
        private System.Windows.Forms.Label lblFilterTo;
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