namespace HistoryLib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageHistoryForm));
            this.ilvImages = new Manina.Windows.Forms.ImageListView();
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsddbViewMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiViewModeThumbnails = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiViewModeGallery = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiViewModePane = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddbThumbnailSize = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiThumbnailSize75 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiThumbnailSize100 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiThumbnailSize150 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiThumbnailSize200 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiThumbnailSize250 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbQuickList = new System.Windows.Forms.ToolStripButton();
            this.tscMain.ContentPanel.SuspendLayout();
            this.tscMain.TopToolStripPanel.SuspendLayout();
            this.tscMain.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilvImages
            // 
            this.ilvImages.AllowDuplicateFileNames = true;
            this.ilvImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ilvImages.CacheLimit = "100MB";
            this.ilvImages.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ilvImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ilvImages.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ilvImages.Location = new System.Drawing.Point(0, 0);
            this.ilvImages.Name = "ilvImages";
            this.ilvImages.PersistentCacheDirectory = "";
            this.ilvImages.PersistentCacheSize = ((long)(100));
            this.ilvImages.Size = new System.Drawing.Size(804, 587);
            this.ilvImages.TabIndex = 0;
            this.ilvImages.ThumbnailSize = new System.Drawing.Size(100, 100);
            this.ilvImages.ItemDoubleClick += new Manina.Windows.Forms.ItemDoubleClickEventHandler(this.ilvImages_ItemDoubleClick);
            this.ilvImages.SelectionChanged += new System.EventHandler(this.ilvImages_SelectionChanged);
            this.ilvImages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ilvImages_KeyDown);
            this.ilvImages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ilvImages_MouseUp);
            // 
            // tscMain
            // 
            // 
            // tscMain.ContentPanel
            // 
            this.tscMain.ContentPanel.Controls.Add(this.ilvImages);
            this.tscMain.ContentPanel.Size = new System.Drawing.Size(804, 587);
            this.tscMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscMain.Location = new System.Drawing.Point(0, 0);
            this.tscMain.Name = "tscMain";
            this.tscMain.Size = new System.Drawing.Size(804, 612);
            this.tscMain.TabIndex = 1;
            this.tscMain.Text = "toolStripContainer1";
            // 
            // tscMain.TopToolStripPanel
            // 
            this.tscMain.TopToolStripPanel.Controls.Add(this.tsMain);
            // 
            // tsMain
            // 
            this.tsMain.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbViewMode,
            this.tsddbThumbnailSize,
            this.tsbQuickList});
            this.tsMain.Location = new System.Drawing.Point(3, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Padding = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.tsMain.Size = new System.Drawing.Size(396, 25);
            this.tsMain.TabIndex = 0;
            // 
            // tsddbViewMode
            // 
            this.tsddbViewMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiViewModeThumbnails,
            this.tsmiViewModeGallery,
            this.tsmiViewModePane});
            this.tsddbViewMode.Image = ((System.Drawing.Image)(resources.GetObject("tsddbViewMode.Image")));
            this.tsddbViewMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbViewMode.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.tsddbViewMode.Name = "tsddbViewMode";
            this.tsddbViewMode.Size = new System.Drawing.Size(79, 19);
            this.tsddbViewMode.Text = "View mode";
            // 
            // tsmiViewModeThumbnails
            // 
            this.tsmiViewModeThumbnails.Name = "tsmiViewModeThumbnails";
            this.tsmiViewModeThumbnails.Size = new System.Drawing.Size(152, 22);
            this.tsmiViewModeThumbnails.Text = "Thumbnails";
            this.tsmiViewModeThumbnails.Click += new System.EventHandler(this.tsmiViewModeThumbnails_Click);
            // 
            // tsmiViewModeGallery
            // 
            this.tsmiViewModeGallery.Name = "tsmiViewModeGallery";
            this.tsmiViewModeGallery.Size = new System.Drawing.Size(152, 22);
            this.tsmiViewModeGallery.Text = "Gallery";
            this.tsmiViewModeGallery.Click += new System.EventHandler(this.tsmiViewModeGallery_Click);
            // 
            // tsmiViewModePane
            // 
            this.tsmiViewModePane.Name = "tsmiViewModePane";
            this.tsmiViewModePane.Size = new System.Drawing.Size(152, 22);
            this.tsmiViewModePane.Text = "Pane";
            this.tsmiViewModePane.Click += new System.EventHandler(this.tsmiViewModePane_Click);
            // 
            // tsddbThumbnailSize
            // 
            this.tsddbThumbnailSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbThumbnailSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiThumbnailSize75,
            this.tsmiThumbnailSize100,
            this.tsmiThumbnailSize150,
            this.tsmiThumbnailSize200,
            this.tsmiThumbnailSize250});
            this.tsddbThumbnailSize.Image = ((System.Drawing.Image)(resources.GetObject("tsddbThumbnailSize.Image")));
            this.tsddbThumbnailSize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbThumbnailSize.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.tsddbThumbnailSize.Name = "tsddbThumbnailSize";
            this.tsddbThumbnailSize.Size = new System.Drawing.Size(100, 19);
            this.tsddbThumbnailSize.Text = "Thumbnail size";
            // 
            // tsmiThumbnailSize75
            // 
            this.tsmiThumbnailSize75.Name = "tsmiThumbnailSize75";
            this.tsmiThumbnailSize75.Size = new System.Drawing.Size(152, 22);
            this.tsmiThumbnailSize75.Text = "75 x 75";
            this.tsmiThumbnailSize75.Click += new System.EventHandler(this.tsmiThumbnailSize75_Click);
            // 
            // tsmiThumbnailSize100
            // 
            this.tsmiThumbnailSize100.Name = "tsmiThumbnailSize100";
            this.tsmiThumbnailSize100.Size = new System.Drawing.Size(152, 22);
            this.tsmiThumbnailSize100.Text = "100 x 100";
            this.tsmiThumbnailSize100.Click += new System.EventHandler(this.tsmiThumbnailSize100_Click);
            // 
            // tsmiThumbnailSize150
            // 
            this.tsmiThumbnailSize150.Name = "tsmiThumbnailSize150";
            this.tsmiThumbnailSize150.Size = new System.Drawing.Size(152, 22);
            this.tsmiThumbnailSize150.Text = "150 x 150";
            this.tsmiThumbnailSize150.Click += new System.EventHandler(this.tsmiThumbnailSize150_Click);
            // 
            // tsmiThumbnailSize200
            // 
            this.tsmiThumbnailSize200.Name = "tsmiThumbnailSize200";
            this.tsmiThumbnailSize200.Size = new System.Drawing.Size(152, 22);
            this.tsmiThumbnailSize200.Text = "200 x 200";
            this.tsmiThumbnailSize200.Click += new System.EventHandler(this.tsmiThumbnailSize200_Click);
            // 
            // tsmiThumbnailSize250
            // 
            this.tsmiThumbnailSize250.Name = "tsmiThumbnailSize250";
            this.tsmiThumbnailSize250.Size = new System.Drawing.Size(152, 22);
            this.tsmiThumbnailSize250.Text = "250 x 250";
            this.tsmiThumbnailSize250.Click += new System.EventHandler(this.tsmiThumbnailSize250_Click);
            // 
            // tsbQuickList
            // 
            this.tsbQuickList.CheckOnClick = true;
            this.tsbQuickList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbQuickList.Image = ((System.Drawing.Image)(resources.GetObject("tsbQuickList.Image")));
            this.tsbQuickList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbQuickList.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.tsbQuickList.Name = "tsbQuickList";
            this.tsbQuickList.Size = new System.Drawing.Size(174, 19);
            this.tsbQuickList.Text = "Only show last 100 screenshots";
            this.tsbQuickList.Click += new System.EventHandler(this.tsbQuickList_Click);
            // 
            // ImageHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 612);
            this.Controls.Add(this.tscMain);
            this.KeyPreview = true;
            this.Name = "ImageHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image history";
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
        private System.Windows.Forms.ToolStripDropDownButton tsddbThumbnailSize;
        private System.Windows.Forms.ToolStripMenuItem tsmiThumbnailSize75;
        private System.Windows.Forms.ToolStripMenuItem tsmiThumbnailSize100;
        private System.Windows.Forms.ToolStripMenuItem tsmiThumbnailSize150;
        private System.Windows.Forms.ToolStripMenuItem tsmiThumbnailSize200;
        private System.Windows.Forms.ToolStripMenuItem tsmiThumbnailSize250;
        private System.Windows.Forms.ToolStripDropDownButton tsddbViewMode;
        private System.Windows.Forms.ToolStripMenuItem tsmiViewModeThumbnails;
        private System.Windows.Forms.ToolStripMenuItem tsmiViewModeGallery;
        private System.Windows.Forms.ToolStripMenuItem tsmiViewModePane;
        private System.Windows.Forms.ToolStripButton tsbQuickList;
    }
}