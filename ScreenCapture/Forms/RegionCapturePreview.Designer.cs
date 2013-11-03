namespace ScreenCapture
{
    partial class RegionCapturePreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegionCapturePreview));
            this.tsRegionTools = new System.Windows.Forms.ToolStrip();
            this.tsbFullscreen = new System.Windows.Forms.ToolStripButton();
            this.tsbWindowRectangle = new System.Windows.Forms.ToolStripButton();
            this.tsbRectangle = new System.Windows.Forms.ToolStripButton();
            this.tsbRoundedRectangle = new System.Windows.Forms.ToolStripButton();
            this.tsbEllipse = new System.Windows.Forms.ToolStripButton();
            this.tsbTriangle = new System.Windows.Forms.ToolStripButton();
            this.tsbDiamond = new System.Windows.Forms.ToolStripButton();
            this.tsbPolygon = new System.Windows.Forms.ToolStripButton();
            this.tsbFreeHand = new System.Windows.Forms.ToolStripButton();
            this.tsbLastRegion = new System.Windows.Forms.ToolStripButton();
            this.pbResult = new System.Windows.Forms.PictureBox();
            this.pImage = new System.Windows.Forms.Panel();
            this.pgSurfaceConfig = new System.Windows.Forms.PropertyGrid();
            this.btnClipboardCopy = new System.Windows.Forms.Button();
            this.tsRegionTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).BeginInit();
            this.pImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsRegionTools
            // 
            this.tsRegionTools.AutoSize = false;
            this.tsRegionTools.Dock = System.Windows.Forms.DockStyle.None;
            this.tsRegionTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFullscreen,
            this.tsbWindowRectangle,
            this.tsbRectangle,
            this.tsbRoundedRectangle,
            this.tsbEllipse,
            this.tsbTriangle,
            this.tsbDiamond,
            this.tsbPolygon,
            this.tsbFreeHand,
            this.tsbLastRegion});
            this.tsRegionTools.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.tsRegionTools.Location = new System.Drawing.Point(8, 8);
            this.tsRegionTools.Name = "tsRegionTools";
            this.tsRegionTools.Padding = new System.Windows.Forms.Padding(10, 10, 1, 0);
            this.tsRegionTools.ShowItemToolTips = false;
            this.tsRegionTools.Size = new System.Drawing.Size(168, 248);
            this.tsRegionTools.TabIndex = 0;
            this.tsRegionTools.Text = "toolStrip1";
            // 
            // tsbFullscreen
            // 
            this.tsbFullscreen.Image = ((System.Drawing.Image)(resources.GetObject("tsbFullscreen.Image")));
            this.tsbFullscreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFullscreen.Name = "tsbFullscreen";
            this.tsbFullscreen.Size = new System.Drawing.Size(80, 20);
            this.tsbFullscreen.Text = "Fullscreen";
            this.tsbFullscreen.Click += new System.EventHandler(this.tsbFullscreen_Click);
            // 
            // tsbWindowRectangle
            // 
            this.tsbWindowRectangle.Image = ((System.Drawing.Image)(resources.GetObject("tsbWindowRectangle.Image")));
            this.tsbWindowRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWindowRectangle.Name = "tsbWindowRectangle";
            this.tsbWindowRectangle.Size = new System.Drawing.Size(139, 20);
            this.tsbWindowRectangle.Text = "Window && Rectangle";
            this.tsbWindowRectangle.Click += new System.EventHandler(this.tsbWindowRectangle_Click);
            // 
            // tsbRectangle
            // 
            this.tsbRectangle.Image = ((System.Drawing.Image)(resources.GetObject("tsbRectangle.Image")));
            this.tsbRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRectangle.Name = "tsbRectangle";
            this.tsbRectangle.Size = new System.Drawing.Size(79, 20);
            this.tsbRectangle.Text = "Rectangle";
            this.tsbRectangle.Click += new System.EventHandler(this.tsbRectangle_Click);
            // 
            // tsbRoundedRectangle
            // 
            this.tsbRoundedRectangle.Image = ((System.Drawing.Image)(resources.GetObject("tsbRoundedRectangle.Image")));
            this.tsbRoundedRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRoundedRectangle.Name = "tsbRoundedRectangle";
            this.tsbRoundedRectangle.Size = new System.Drawing.Size(130, 20);
            this.tsbRoundedRectangle.Text = "Rounded Rectangle";
            this.tsbRoundedRectangle.Click += new System.EventHandler(this.tsbRoundedRectangle_Click);
            // 
            // tsbEllipse
            // 
            this.tsbEllipse.Image = ((System.Drawing.Image)(resources.GetObject("tsbEllipse.Image")));
            this.tsbEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEllipse.Name = "tsbEllipse";
            this.tsbEllipse.Size = new System.Drawing.Size(60, 20);
            this.tsbEllipse.Text = "Ellipse";
            this.tsbEllipse.Click += new System.EventHandler(this.tsbEllipse_Click);
            // 
            // tsbTriangle
            // 
            this.tsbTriangle.Image = ((System.Drawing.Image)(resources.GetObject("tsbTriangle.Image")));
            this.tsbTriangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTriangle.Name = "tsbTriangle";
            this.tsbTriangle.Size = new System.Drawing.Size(70, 20);
            this.tsbTriangle.Text = "Triangle";
            this.tsbTriangle.Click += new System.EventHandler(this.tsbTriangle_Click);
            // 
            // tsbDiamond
            // 
            this.tsbDiamond.Image = ((System.Drawing.Image)(resources.GetObject("tsbDiamond.Image")));
            this.tsbDiamond.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDiamond.Name = "tsbDiamond";
            this.tsbDiamond.Size = new System.Drawing.Size(76, 20);
            this.tsbDiamond.Text = "Diamond";
            this.tsbDiamond.Click += new System.EventHandler(this.tsbDiamond_Click);
            // 
            // tsbPolygon
            // 
            this.tsbPolygon.Image = ((System.Drawing.Image)(resources.GetObject("tsbPolygon.Image")));
            this.tsbPolygon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPolygon.Name = "tsbPolygon";
            this.tsbPolygon.Size = new System.Drawing.Size(71, 20);
            this.tsbPolygon.Text = "Polygon";
            this.tsbPolygon.Click += new System.EventHandler(this.tsbPolygon_Click);
            // 
            // tsbFreeHand
            // 
            this.tsbFreeHand.Image = ((System.Drawing.Image)(resources.GetObject("tsbFreeHand.Image")));
            this.tsbFreeHand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFreeHand.Name = "tsbFreeHand";
            this.tsbFreeHand.Size = new System.Drawing.Size(81, 20);
            this.tsbFreeHand.Text = "Free Hand";
            this.tsbFreeHand.Click += new System.EventHandler(this.tsbFreeHand_Click);
            // 
            // tsbLastRegion
            // 
            this.tsbLastRegion.Image = ((System.Drawing.Image)(resources.GetObject("tsbLastRegion.Image")));
            this.tsbLastRegion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLastRegion.Name = "tsbLastRegion";
            this.tsbLastRegion.Size = new System.Drawing.Size(88, 20);
            this.tsbLastRegion.Text = "Last Region";
            this.tsbLastRegion.Click += new System.EventHandler(this.tsbLastRegion_Click);
            // 
            // pbResult
            // 
            this.pbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbResult.Location = new System.Drawing.Point(0, 0);
            this.pbResult.Name = "pbResult";
            this.pbResult.Size = new System.Drawing.Size(500, 500);
            this.pbResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbResult.TabIndex = 1;
            this.pbResult.TabStop = false;
            // 
            // pImage
            // 
            this.pImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pImage.AutoScroll = true;
            this.pImage.Controls.Add(this.pbResult);
            this.pImage.Location = new System.Drawing.Point(304, 8);
            this.pImage.Name = "pImage";
            this.pImage.Size = new System.Drawing.Size(536, 632);
            this.pImage.TabIndex = 4;
            // 
            // pgSurfaceConfig
            // 
            this.pgSurfaceConfig.Location = new System.Drawing.Point(8, 264);
            this.pgSurfaceConfig.Name = "pgSurfaceConfig";
            this.pgSurfaceConfig.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSurfaceConfig.Size = new System.Drawing.Size(288, 376);
            this.pgSurfaceConfig.TabIndex = 2;
            this.pgSurfaceConfig.ToolbarVisible = false;
            // 
            // btnClipboardCopy
            // 
            this.btnClipboardCopy.Location = new System.Drawing.Point(184, 8);
            this.btnClipboardCopy.Name = "btnClipboardCopy";
            this.btnClipboardCopy.Size = new System.Drawing.Size(112, 40);
            this.btnClipboardCopy.TabIndex = 2;
            this.btnClipboardCopy.Text = "Copy image to clipboard";
            this.btnClipboardCopy.UseVisualStyleBackColor = true;
            this.btnClipboardCopy.Click += new System.EventHandler(this.btnClipboardCopy_Click);
            // 
            // RegionCapturePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 649);
            this.Controls.Add(this.btnClipboardCopy);
            this.Controls.Add(this.pImage);
            this.Controls.Add(this.pgSurfaceConfig);
            this.Controls.Add(this.tsRegionTools);
            this.Name = "RegionCapturePreview";
            this.Text = "Region Capture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegionCapturePreview_FormClosing);
            this.tsRegionTools.ResumeLayout(false);
            this.tsRegionTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).EndInit();
            this.pImage.ResumeLayout(false);
            this.pImage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsRegionTools;
        private System.Windows.Forms.ToolStripButton tsbFreeHand;
        private System.Windows.Forms.ToolStripButton tsbEllipse;
        private System.Windows.Forms.ToolStripButton tsbRectangle;
        private System.Windows.Forms.ToolStripButton tsbRoundedRectangle;
        private System.Windows.Forms.ToolStripButton tsbTriangle;
        private System.Windows.Forms.ToolStripButton tsbPolygon;
        private System.Windows.Forms.PictureBox pbResult;
        private System.Windows.Forms.ToolStripButton tsbDiamond;
        private System.Windows.Forms.ToolStripButton tsbFullscreen;
        private System.Windows.Forms.Panel pImage;
        private System.Windows.Forms.Button btnClipboardCopy;
        private System.Windows.Forms.ToolStripButton tsbWindowRectangle;
        private System.Windows.Forms.ToolStripButton tsbLastRegion;
        private System.Windows.Forms.PropertyGrid pgSurfaceConfig;
    }
}