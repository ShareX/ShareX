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
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader13 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Name, "Name", 100, 0, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader14 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FileSize, "Size", 100, 1, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader15 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Dimensions, "Dimensions", 100, 2, true);
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader16 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FilePath, "Path", 100, 3, true);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageHistoryForm));
            tscMain = new System.Windows.Forms.ToolStripContainer();
            ilvImages = new Manina.Windows.Forms.ImageListView();
            tsMain = new System.Windows.Forms.ToolStrip();
            tslSearch = new System.Windows.Forms.ToolStripLabel();
            tstbSearch = new System.Windows.Forms.ToolStripTextBox();
            tsbSearch = new System.Windows.Forms.ToolStripButton();
            tsbFavorites = new System.Windows.Forms.ToolStripButton();
            tss1 = new System.Windows.Forms.ToolStripSeparator();
            tsbSettings = new System.Windows.Forms.ToolStripButton();
            tss2 = new System.Windows.Forms.ToolStripSeparator();
            tsbShowStats = new System.Windows.Forms.ToolStripButton();
            tscMain.ContentPanel.SuspendLayout();
            tscMain.TopToolStripPanel.SuspendLayout();
            tscMain.SuspendLayout();
            tsMain.SuspendLayout();
            SuspendLayout();
            // 
            // tscMain
            // 
            // 
            // tscMain.ContentPanel
            // 
            tscMain.ContentPanel.Controls.Add(ilvImages);
            resources.ApplyResources(tscMain.ContentPanel, "tscMain.ContentPanel");
            resources.ApplyResources(tscMain, "tscMain");
            tscMain.Name = "tscMain";
            // 
            // tscMain.TopToolStripPanel
            // 
            tscMain.TopToolStripPanel.Controls.Add(tsMain);
            // 
            // ilvImages
            // 
            ilvImages.AllowDrag = true;
            ilvImages.AllowDuplicateFileNames = true;
            ilvImages.AllowItemReorder = false;
            ilvImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ilvImages.CacheLimit = "100MB";
            imageListViewColumnHeader13.Comparer = null;
            imageListViewColumnHeader13.DisplayIndex = 0;
            imageListViewColumnHeader13.Grouper = null;
            imageListViewColumnHeader13.Key = "";
            imageListViewColumnHeader13.Type = Manina.Windows.Forms.ColumnType.Name;
            imageListViewColumnHeader14.Comparer = null;
            imageListViewColumnHeader14.DisplayIndex = 1;
            imageListViewColumnHeader14.Grouper = null;
            imageListViewColumnHeader14.Key = "";
            imageListViewColumnHeader14.Type = Manina.Windows.Forms.ColumnType.FileSize;
            imageListViewColumnHeader15.Comparer = null;
            imageListViewColumnHeader15.DisplayIndex = 2;
            imageListViewColumnHeader15.Grouper = null;
            imageListViewColumnHeader15.Key = "";
            imageListViewColumnHeader15.Type = Manina.Windows.Forms.ColumnType.Dimensions;
            imageListViewColumnHeader16.Comparer = null;
            imageListViewColumnHeader16.DisplayIndex = 3;
            imageListViewColumnHeader16.Grouper = null;
            imageListViewColumnHeader16.Key = "";
            imageListViewColumnHeader16.Type = Manina.Windows.Forms.ColumnType.FilePath;
            ilvImages.Columns.AddRange(new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader[] { imageListViewColumnHeader13, imageListViewColumnHeader14, imageListViewColumnHeader15, imageListViewColumnHeader16 });
            resources.ApplyResources(ilvImages, "ilvImages");
            ilvImages.Name = "ilvImages";
            ilvImages.ThumbnailSize = new System.Drawing.Size(100, 100);
            ilvImages.UseWIC = true;
            ilvImages.ItemDoubleClick += ilvImages_ItemDoubleClick;
            ilvImages.SelectionChanged += ilvImages_SelectionChanged;
            ilvImages.KeyDown += ilvImages_KeyDown;
            // 
            // tsMain
            // 
            resources.ApplyResources(tsMain, "tsMain");
            tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tslSearch, tstbSearch, tsbSearch, tss1, tsbFavorites, tsbShowStats, tss2, tsbSettings });
            tsMain.Name = "tsMain";
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
            tstbSearch.KeyDown += tstbSearch_KeyDown;
            // 
            // tsbSearch
            // 
            tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSearch.Image = Properties.Resources.magnifier;
            resources.ApplyResources(tsbSearch, "tsbSearch");
            tsbSearch.Name = "tsbSearch";
            tsbSearch.Click += tsbSearch_Click;
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
            // tss1
            // 
            tss1.Name = "tss1";
            resources.ApplyResources(tss1, "tss1");
            // 
            // tsbSettings
            // 
            tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSettings.Image = Properties.Resources.gear;
            resources.ApplyResources(tsbSettings, "tsbSettings");
            tsbSettings.Name = "tsbSettings";
            tsbSettings.Click += tsbSettings_Click;
            // 
            // tss2
            // 
            tss2.Name = "tss2";
            resources.ApplyResources(tss2, "tss2");
            // 
            // tsbShowStats
            // 
            tsbShowStats.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbShowStats.Image = Properties.Resources.chart;
            resources.ApplyResources(tsbShowStats, "tsbShowStats");
            tsbShowStats.Name = "tsbShowStats";
            tsbShowStats.Click += tsbShowStats_Click;
            // 
            // ImageHistoryForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(tscMain);
            Name = "ImageHistoryForm";
            FormClosing += ImageHistoryForm_FormClosing;
            Shown += ImageHistoryForm_Shown;
            KeyDown += ImageHistoryForm_KeyDown;
            tscMain.ContentPanel.ResumeLayout(false);
            tscMain.TopToolStripPanel.ResumeLayout(false);
            tscMain.TopToolStripPanel.PerformLayout();
            tscMain.ResumeLayout(false);
            tscMain.PerformLayout();
            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            ResumeLayout(false);

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
        private System.Windows.Forms.ToolStripButton tsbFavorites;
        private System.Windows.Forms.ToolStripSeparator tss2;
        private System.Windows.Forms.ToolStripButton tsbShowStats;
    }
}