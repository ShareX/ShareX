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
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tslScale = new System.Windows.Forms.ToolStripLabel();
            this.tsbOptions = new System.Windows.Forms.ToolStripButton();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tsbCopy = new System.Windows.Forms.ToolStripButton();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMain
            // 
            this.tsMain.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCopy,
            this.tslScale,
            this.tsbOptions,
            this.tsbClose});
            this.tsMain.Location = new System.Drawing.Point(8, 8);
            this.tsMain.Name = "tsMain";
            this.tsMain.Padding = new System.Windows.Forms.Padding(3, 2, 1, 1);
            this.tsMain.Size = new System.Drawing.Size(174, 34);
            this.tsMain.TabIndex = 0;
            // 
            // tslScale
            // 
            this.tslScale.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tslScale.Name = "tslScale";
            this.tslScale.Padding = new System.Windows.Forms.Padding(4);
            this.tslScale.Size = new System.Drawing.Size(53, 28);
            this.tslScale.Text = "100%";
            this.tslScale.ToolTipText = "Scale";
            this.tslScale.Click += new System.EventHandler(this.tslScale_Click);
            // 
            // tsbOptions
            // 
            this.tsbOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOptions.Image = global::ShareX.Properties.Resources.gear;
            this.tsbOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOptions.Name = "tsbOptions";
            this.tsbOptions.Padding = new System.Windows.Forms.Padding(4);
            this.tsbOptions.Size = new System.Drawing.Size(28, 28);
            this.tsbOptions.Text = "Options...";
            this.tsbOptions.Click += new System.EventHandler(this.tsbOptions_Click);
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = global::ShareX.Properties.Resources.cross_button;
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Padding = new System.Windows.Forms.Padding(4);
            this.tsbClose.Size = new System.Drawing.Size(28, 28);
            this.tsbClose.Text = "Close";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tsbCopy
            // 
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopy.Image = global::ShareX.Properties.Resources.document_copy;
            this.tsbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Padding = new System.Windows.Forms.Padding(4);
            this.tsbCopy.Size = new System.Drawing.Size(28, 28);
            this.tsbCopy.Text = "Copy";
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
            // 
            // PinToScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.tsMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PinToScreenForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ShareX - Pin to screen";
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
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripButton tsbOptions;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripLabel tslScale;
        private System.Windows.Forms.ToolStripButton tsbCopy;
    }
}