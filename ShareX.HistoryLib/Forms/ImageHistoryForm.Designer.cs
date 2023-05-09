namespace ShareX.HistoryLib
{
    partial class ImageHistoryForm
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
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader ımageListViewColumnHeader1 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Name, "Name", 100, 0, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader ımageListViewColumnHeader2 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FileSize, "Size", 100, 1, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader ımageListViewColumnHeader3 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Dimensions, "Dimensions", 100, 2, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader ımageListViewColumnHeader4 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FilePath, "Path", 100, 3, true);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageHistoryForm));
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.ilvImages = new Manina.Windows.Forms.ImageListView();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tslSearch = new System.Windows.Forms.ToolStripLabel();
            this.tstbSearch = new System.Windows.Forms.ToolStripTextBox();
            this.tsbSearch = new System.Windows.Forms.ToolStripButton();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSettings = new System.Windows.Forms.ToolStripButton();
            this.tscMain.ContentPanel.SuspendLayout();
            this.tscMain.TopToolStripPanel.SuspendLayout();
            this.tscMain.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscMain
            // 
            // 
            // tscMain.ContentPanel
            // 
            this.tscMain.ContentPanel.Controls.Add(this.ilvImages);
            resources.ApplyResources(this.tscMain.ContentPanel, "tscMain.ContentPanel");
            resources.ApplyResources(this.tscMain, "tscMain");
            this.tscMain.Name = "tscMain";
            // 
            // tscMain.TopToolStripPanel
            // 
            this.tscMain.TopToolStripPanel.Controls.Add(this.tsMain);
            // 
            // ilvImages
            // 
            this.ilvImages.AllowDrag = true;
            this.ilvImages.AllowDuplicateFileNames = true;
            this.ilvImages.AllowItemReorder = false;
            this.ilvImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ilvImages.CacheLimit = "100MB";
            ımageListViewColumnHeader1.Comparer = null;
            ımageListViewColumnHeader1.DisplayIndex = 0;
            ımageListViewColumnHeader1.Grouper = null;
            ımageListViewColumnHeader1.Key = "";
            ımageListViewColumnHeader1.Type = Manina.Windows.Forms.ColumnType.Name;
            ımageListViewColumnHeader2.Comparer = null;
            ımageListViewColumnHeader2.DisplayIndex = 1;
            ımageListViewColumnHeader2.Grouper = null;
            ımageListViewColumnHeader2.Key = "";
            ımageListViewColumnHeader2.Type = Manina.Windows.Forms.ColumnType.FileSize;
            ımageListViewColumnHeader3.Comparer = null;
            ımageListViewColumnHeader3.DisplayIndex = 2;
            ımageListViewColumnHeader3.Grouper = null;
            ımageListViewColumnHeader3.Key = "";
            ımageListViewColumnHeader3.Type = Manina.Windows.Forms.ColumnType.Dimensions;
            ımageListViewColumnHeader4.Comparer = null;
            ımageListViewColumnHeader4.DisplayIndex = 3;
            ımageListViewColumnHeader4.Grouper = null;
            ımageListViewColumnHeader4.Key = "";
            ımageListViewColumnHeader4.Type = Manina.Windows.Forms.ColumnType.FilePath;
            this.ilvImages.Columns.AddRange(new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader[] {
            ımageListViewColumnHeader1,
            ımageListViewColumnHeader2,
            ımageListViewColumnHeader3,
            ımageListViewColumnHeader4});
            resources.ApplyResources(this.ilvImages, "ilvImages");
            this.ilvImages.Name = "ilvImages";
            this.ilvImages.PersistentCacheDirectory = "";
            this.ilvImages.PersistentCacheSize = ((long)(100));
            this.ilvImages.ThumbnailSize = new System.Drawing.Size(100, 100);
            this.ilvImages.UseWIC = true;
            this.ilvImages.ItemDoubleClick += new Manina.Windows.Forms.ItemDoubleClickEventHandler(this.ilvImages_ItemDoubleClick);
            this.ilvImages.SelectionChanged += new System.EventHandler(this.ilvImages_SelectionChanged);
            this.ilvImages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ilvImages_KeyDown);
            // 
            // tsMain
            // 
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSearch,
            this.tstbSearch,
            this.tsbSearch,
            this.tss1,
            this.tsbSettings});
            this.tsMain.Name = "tsMain";
            // 
            // tslSearch
            // 
            this.tslSearch.Name = "tslSearch";
            resources.ApplyResources(this.tslSearch, "tslSearch");
            // 
            // tstbSearch
            // 
            this.tstbSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tstbSearch, "tstbSearch");
            this.tstbSearch.Name = "tstbSearch";
            this.tstbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tstbSearch_KeyDown);
            // 
            // tsbSearch
            // 
            this.tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSearch.Image = global::ShareX.HistoryLib.Properties.Resources.magnifier;
            resources.ApplyResources(this.tsbSearch, "tsbSearch");
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Click += new System.EventHandler(this.tsbSearch_Click);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            resources.ApplyResources(this.tss1, "tss1");
            // 
            // tsbSettings
            // 
            this.tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSettings.Image = global::ShareX.HistoryLib.Properties.Resources.gear;
            resources.ApplyResources(this.tsbSettings, "tsbSettings");
            this.tsbSettings.Name = "tsbSettings";
            this.tsbSettings.Click += new System.EventHandler(this.tsbSettings_Click);
            // 
            // ImageHistoryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tscMain);
            this.KeyPreview = true;
            this.Name = "ImageHistoryForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageHistoryForm_FormClosing);
            this.Shown += new System.EventHandler(this.ImageHistoryForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageHistoryForm_KeyDown);
            this.tscMain.ContentPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.PerformLayout();
            this.tscMain.ResumeLayout(false);
            this.tscMain.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Manina.Windows.Forms.ImageListView ilvImages;
        private System.Windows.Forms.ToolStripContainer tscMain;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripLabel tslSearch;
        private System.Windows.Forms.ToolStripTextBox tstbSearch;
        private System.Windows.Forms.ToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripButton tsbSettings;
    }
}