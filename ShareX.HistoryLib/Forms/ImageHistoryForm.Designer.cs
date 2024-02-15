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
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader1 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Name, "Name", 100, 0, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader2 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FileSize, "Size", 100, 1, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader3 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Dimensions, "Dimensions", 100, 2, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader4 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FilePath, "Path", 100, 3, true);
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
            imageListViewColumnHeader1.Comparer = null;
            imageListViewColumnHeader1.DisplayIndex = 0;
            imageListViewColumnHeader1.Grouper = null;
            imageListViewColumnHeader1.Key = "";
            imageListViewColumnHeader1.Type = Manina.Windows.Forms.ColumnType.Name;
            imageListViewColumnHeader2.Comparer = null;
            imageListViewColumnHeader2.DisplayIndex = 1;
            imageListViewColumnHeader2.Grouper = null;
            imageListViewColumnHeader2.Key = "";
            imageListViewColumnHeader2.Type = Manina.Windows.Forms.ColumnType.FileSize;
            imageListViewColumnHeader3.Comparer = null;
            imageListViewColumnHeader3.DisplayIndex = 2;
            imageListViewColumnHeader3.Grouper = null;
            imageListViewColumnHeader3.Key = "";
            imageListViewColumnHeader3.Type = Manina.Windows.Forms.ColumnType.Dimensions;
            imageListViewColumnHeader4.Comparer = null;
            imageListViewColumnHeader4.DisplayIndex = 3;
            imageListViewColumnHeader4.Grouper = null;
            imageListViewColumnHeader4.Key = "";
            imageListViewColumnHeader4.Type = Manina.Windows.Forms.ColumnType.FilePath;
            this.ilvImages.Columns.AddRange(new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader[] {
            imageListViewColumnHeader1,
            imageListViewColumnHeader2,
            imageListViewColumnHeader3,
            imageListViewColumnHeader4});
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
            this.tstbSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.tstbSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
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