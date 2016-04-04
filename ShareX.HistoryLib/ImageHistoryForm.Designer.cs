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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageHistoryForm));
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.ilvImages = new Manina.Windows.Forms.ImageListView();
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
            this.tsddbMaxImageLimit = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiMaxImageLimit100 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaxImageLimit250 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaxImageLimit1000 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaxImageLimit0 = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ilvImages.AllowDuplicateFileNames = true;
            this.ilvImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ilvImages.CacheLimit = "100MB";
            this.ilvImages.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            resources.ApplyResources(this.ilvImages, "ilvImages");
            this.ilvImages.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ilvImages.Name = "ilvImages";
            this.ilvImages.PersistentCacheDirectory = "";
            this.ilvImages.PersistentCacheSize = ((long)(100));
            this.ilvImages.ThumbnailSize = new System.Drawing.Size(100, 100);
            this.ilvImages.ItemDoubleClick += new Manina.Windows.Forms.ItemDoubleClickEventHandler(this.ilvImages_ItemDoubleClick);
            this.ilvImages.SelectionChanged += new System.EventHandler(this.ilvImages_SelectionChanged);
            this.ilvImages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ilvImages_KeyDown);
            this.ilvImages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ilvImages_MouseUp);
            // 
            // tsMain
            // 
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbViewMode,
            this.tsddbThumbnailSize,
            this.tsddbMaxImageLimit});
            this.tsMain.Name = "tsMain";
            // 
            // tsddbViewMode
            // 
            this.tsddbViewMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiViewModeThumbnails,
            this.tsmiViewModeGallery,
            this.tsmiViewModePane});
            this.tsddbViewMode.Name = "tsddbViewMode";
            resources.ApplyResources(this.tsddbViewMode, "tsddbViewMode");
            // 
            // tsmiViewModeThumbnails
            // 
            this.tsmiViewModeThumbnails.Name = "tsmiViewModeThumbnails";
            resources.ApplyResources(this.tsmiViewModeThumbnails, "tsmiViewModeThumbnails");
            this.tsmiViewModeThumbnails.Click += new System.EventHandler(this.tsmiViewModeThumbnails_Click);
            // 
            // tsmiViewModeGallery
            // 
            this.tsmiViewModeGallery.Name = "tsmiViewModeGallery";
            resources.ApplyResources(this.tsmiViewModeGallery, "tsmiViewModeGallery");
            this.tsmiViewModeGallery.Click += new System.EventHandler(this.tsmiViewModeGallery_Click);
            // 
            // tsmiViewModePane
            // 
            this.tsmiViewModePane.Name = "tsmiViewModePane";
            resources.ApplyResources(this.tsmiViewModePane, "tsmiViewModePane");
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
            this.tsddbThumbnailSize.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.tsddbThumbnailSize.Name = "tsddbThumbnailSize";
            resources.ApplyResources(this.tsddbThumbnailSize, "tsddbThumbnailSize");
            // 
            // tsmiThumbnailSize75
            // 
            this.tsmiThumbnailSize75.Name = "tsmiThumbnailSize75";
            resources.ApplyResources(this.tsmiThumbnailSize75, "tsmiThumbnailSize75");
            this.tsmiThumbnailSize75.Click += new System.EventHandler(this.tsmiThumbnailSize75_Click);
            // 
            // tsmiThumbnailSize100
            // 
            this.tsmiThumbnailSize100.Name = "tsmiThumbnailSize100";
            resources.ApplyResources(this.tsmiThumbnailSize100, "tsmiThumbnailSize100");
            this.tsmiThumbnailSize100.Click += new System.EventHandler(this.tsmiThumbnailSize100_Click);
            // 
            // tsmiThumbnailSize150
            // 
            this.tsmiThumbnailSize150.Name = "tsmiThumbnailSize150";
            resources.ApplyResources(this.tsmiThumbnailSize150, "tsmiThumbnailSize150");
            this.tsmiThumbnailSize150.Click += new System.EventHandler(this.tsmiThumbnailSize150_Click);
            // 
            // tsmiThumbnailSize200
            // 
            this.tsmiThumbnailSize200.Name = "tsmiThumbnailSize200";
            resources.ApplyResources(this.tsmiThumbnailSize200, "tsmiThumbnailSize200");
            this.tsmiThumbnailSize200.Click += new System.EventHandler(this.tsmiThumbnailSize200_Click);
            // 
            // tsmiThumbnailSize250
            // 
            this.tsmiThumbnailSize250.Name = "tsmiThumbnailSize250";
            resources.ApplyResources(this.tsmiThumbnailSize250, "tsmiThumbnailSize250");
            this.tsmiThumbnailSize250.Click += new System.EventHandler(this.tsmiThumbnailSize250_Click);
            // 
            // tsddbMaxImageLimit
            // 
            this.tsddbMaxImageLimit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbMaxImageLimit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMaxImageLimit100,
            this.tsmiMaxImageLimit250,
            this.tsmiMaxImageLimit1000,
            this.tsmiMaxImageLimit0});
            resources.ApplyResources(this.tsddbMaxImageLimit, "tsddbMaxImageLimit");
            this.tsddbMaxImageLimit.Name = "tsddbMaxImageLimit";
            // 
            // tsmiMaxImageLimit100
            // 
            this.tsmiMaxImageLimit100.Name = "tsmiMaxImageLimit100";
            resources.ApplyResources(this.tsmiMaxImageLimit100, "tsmiMaxImageLimit100");
            this.tsmiMaxImageLimit100.Click += new System.EventHandler(this.tsmiMaxImageLimit100_Click);
            // 
            // tsmiMaxImageLimit250
            // 
            this.tsmiMaxImageLimit250.Name = "tsmiMaxImageLimit250";
            resources.ApplyResources(this.tsmiMaxImageLimit250, "tsmiMaxImageLimit250");
            this.tsmiMaxImageLimit250.Click += new System.EventHandler(this.tsmiMaxImageLimit250_Click);
            // 
            // tsmiMaxImageLimit1000
            // 
            this.tsmiMaxImageLimit1000.Name = "tsmiMaxImageLimit1000";
            resources.ApplyResources(this.tsmiMaxImageLimit1000, "tsmiMaxImageLimit1000");
            this.tsmiMaxImageLimit1000.Click += new System.EventHandler(this.tsmiMaxImageLimit1000_Click);
            // 
            // tsmiMaxImageLimit0
            // 
            this.tsmiMaxImageLimit0.Name = "tsmiMaxImageLimit0";
            resources.ApplyResources(this.tsmiMaxImageLimit0, "tsmiMaxImageLimit0");
            this.tsmiMaxImageLimit0.Click += new System.EventHandler(this.tsmiMaxImageLimit0_Click);
            // 
            // ImageHistoryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tscMain);
            this.KeyPreview = true;
            this.Name = "ImageHistoryForm";
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
        private System.Windows.Forms.ToolStripDropDownButton tsddbMaxImageLimit;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaxImageLimit100;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaxImageLimit250;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaxImageLimit1000;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaxImageLimit0;
    }
}