namespace ShareX
{
    partial class PinToScreenForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PinToScreenForm));
            this.tsMain = new ShareX.HelpersLib.ToolStripEx();
            this.tsbCopy = new System.Windows.Forms.ToolStripButton();
            this.tslScale = new System.Windows.Forms.ToolStripLabel();
            this.tsbOptions = new System.Windows.Forms.ToolStripButton();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMain
            // 
            this.tsMain.ClickThrough = true;
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCopy,
            this.tslScale,
            this.tsbOptions,
            this.tsbClose});
            this.tsMain.Name = "tsMain";
            this.tsMain.MouseLeave += new System.EventHandler(this.tsMain_MouseLeave);
            // 
            // tsbCopy
            // 
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopy.Image = global::ShareX.Properties.Resources.document_copy;
            resources.ApplyResources(this.tsbCopy, "tsbCopy");
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Padding = new System.Windows.Forms.Padding(4);
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
            // 
            // tslScale
            // 
            resources.ApplyResources(this.tslScale, "tslScale");
            this.tslScale.Name = "tslScale";
            this.tslScale.Padding = new System.Windows.Forms.Padding(4);
            this.tslScale.Click += new System.EventHandler(this.tslScale_Click);
            // 
            // tsbOptions
            // 
            this.tsbOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOptions.Image = global::ShareX.Properties.Resources.gear;
            resources.ApplyResources(this.tsbOptions, "tsbOptions");
            this.tsbOptions.Name = "tsbOptions";
            this.tsbOptions.Padding = new System.Windows.Forms.Padding(4);
            this.tsbOptions.Click += new System.EventHandler(this.tsbOptions_Click);
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = global::ShareX.Properties.Resources.cross_button;
            resources.ApplyResources(this.tsbClose, "tsbClose");
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Padding = new System.Windows.Forms.Padding(4);
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // PinToScreenForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tsMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PinToScreenForm";
            this.ShowInTaskbar = false;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PinToScreenForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseDown);
            this.MouseEnter += new System.EventHandler(this.PinToScreenForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.PinToScreenForm_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseWheel);
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private HelpersLib.ToolStripEx tsMain;
        private System.Windows.Forms.ToolStripButton tsbOptions;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripLabel tslScale;
        private System.Windows.Forms.ToolStripButton tsbCopy;
    }
}