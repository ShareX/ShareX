namespace ShareX.ScreenCaptureLib
{
    partial class StickerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StickerForm));
            this.ilvStickers = new Manina.Windows.Forms.ImageListView();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tslSearch = new System.Windows.Forms.ToolStripLabel();
            this.tstbSearch = new System.Windows.Forms.ToolStripTextBox();
            this.tslStickers = new System.Windows.Forms.ToolStripLabel();
            this.tscbStickers = new System.Windows.Forms.ToolStripComboBox();
            this.tsbEditStickers = new System.Windows.Forms.ToolStripButton();
            this.tslSize = new System.Windows.Forms.ToolStripLabel();
            this.tsnudSize = new ShareX.HelpersLib.ToolStripNumericUpDown();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilvStickers
            // 
            this.ilvStickers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ilvStickers.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            resources.ApplyResources(this.ilvStickers, "ilvStickers");
            this.ilvStickers.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ilvStickers.MultiSelect = false;
            this.ilvStickers.Name = "ilvStickers";
            this.ilvStickers.PersistentCacheDirectory = "";
            this.ilvStickers.PersistentCacheSize = ((long)(100));
            this.ilvStickers.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.ilvStickers_ItemClick);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.ilvStickers);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsMain);
            // 
            // tsMain
            // 
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSearch,
            this.tstbSearch,
            this.tslStickers,
            this.tscbStickers,
            this.tsbEditStickers,
            this.tslSize,
            this.tsnudSize});
            this.tsMain.Name = "tsMain";
            this.tsMain.Stretch = true;
            // 
            // tslSearch
            // 
            this.tslSearch.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.tslSearch.Name = "tslSearch";
            resources.ApplyResources(this.tslSearch, "tslSearch");
            // 
            // tstbSearch
            // 
            this.tstbSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstbSearch.Name = "tstbSearch";
            resources.ApplyResources(this.tstbSearch, "tstbSearch");
            this.tstbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tstbSearch_KeyDown);
            this.tstbSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tstbSearch_KeyUp);
            this.tstbSearch.TextChanged += new System.EventHandler(this.tstbSearch_TextChanged);
            // 
            // tslStickers
            // 
            this.tslStickers.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.tslStickers.Name = "tslStickers";
            resources.ApplyResources(this.tslStickers, "tslStickers");
            // 
            // tscbStickers
            // 
            this.tscbStickers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.tscbStickers, "tscbStickers");
            this.tscbStickers.Name = "tscbStickers";
            this.tscbStickers.SelectedIndexChanged += new System.EventHandler(this.tscbStickers_SelectedIndexChanged);
            // 
            // tsbEditStickers
            // 
            this.tsbEditStickers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditStickers.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.gear;
            resources.ApplyResources(this.tsbEditStickers, "tsbEditStickers");
            this.tsbEditStickers.Name = "tsbEditStickers";
            this.tsbEditStickers.Click += new System.EventHandler(this.tsbEditStickers_Click);
            // 
            // tslSize
            // 
            this.tslSize.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.tslSize.Name = "tslSize";
            resources.ApplyResources(this.tslSize, "tslSize");
            // 
            // tsnudSize
            // 
            this.tsnudSize.Name = "tsnudSize";
            resources.ApplyResources(this.tsnudSize, "tsnudSize");
            this.tsnudSize.ValueChanged += new System.EventHandler(this.tsnudSize_ValueChanged);
            // 
            // StickerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "StickerForm";
            this.Shown += new System.EventHandler(this.StickerForm_Shown);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Manina.Windows.Forms.ImageListView ilvStickers;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripLabel tslSearch;
        private System.Windows.Forms.ToolStripTextBox tstbSearch;
        private System.Windows.Forms.ToolStripLabel tslStickers;
        private System.Windows.Forms.ToolStripLabel tslSize;
        private System.Windows.Forms.ToolStripComboBox tscbStickers;
        private HelpersLib.ToolStripNumericUpDown tsnudSize;
        private System.Windows.Forms.ToolStripButton tsbEditStickers;
    }
}