namespace ShareX.HelpersLib
{
    partial class PrintForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintForm));
            this.cbAutoRotate = new System.Windows.Forms.CheckBox();
            this.nudMargin = new System.Windows.Forms.NumericUpDown();
            this.cbAutoScale = new System.Windows.Forms.CheckBox();
            this.lblMargin = new System.Windows.Forms.Label();
            this.cbAllowEnlarge = new System.Windows.Forms.CheckBox();
            this.cbCenterImage = new System.Windows.Forms.CheckBox();
            this.btnShowPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudMargin)).BeginInit();
            this.SuspendLayout();
            // 
            // cbAutoRotate
            // 
            resources.ApplyResources(this.cbAutoRotate, "cbAutoRotate");
            this.cbAutoRotate.Name = "cbAutoRotate";
            this.cbAutoRotate.UseVisualStyleBackColor = true;
            this.cbAutoRotate.CheckedChanged += new System.EventHandler(this.cbAutoRotate_CheckedChanged);
            // 
            // nudMargin
            // 
            resources.ApplyResources(this.nudMargin, "nudMargin");
            this.nudMargin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMargin.Name = "nudMargin";
            this.nudMargin.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMargin.ValueChanged += new System.EventHandler(this.nudMargin_ValueChanged);
            // 
            // cbAutoScale
            // 
            resources.ApplyResources(this.cbAutoScale, "cbAutoScale");
            this.cbAutoScale.Name = "cbAutoScale";
            this.cbAutoScale.UseVisualStyleBackColor = true;
            this.cbAutoScale.CheckedChanged += new System.EventHandler(this.cbAutoScale_CheckedChanged);
            // 
            // lblMargin
            // 
            resources.ApplyResources(this.lblMargin, "lblMargin");
            this.lblMargin.Name = "lblMargin";
            // 
            // cbAllowEnlarge
            // 
            resources.ApplyResources(this.cbAllowEnlarge, "cbAllowEnlarge");
            this.cbAllowEnlarge.Name = "cbAllowEnlarge";
            this.cbAllowEnlarge.UseVisualStyleBackColor = true;
            this.cbAllowEnlarge.CheckedChanged += new System.EventHandler(this.cbAllowEnlarge_CheckedChanged);
            // 
            // cbCenterImage
            // 
            resources.ApplyResources(this.cbCenterImage, "cbCenterImage");
            this.cbCenterImage.Name = "cbCenterImage";
            this.cbCenterImage.UseVisualStyleBackColor = true;
            this.cbCenterImage.CheckedChanged += new System.EventHandler(this.cbCenterImage_CheckedChanged);
            // 
            // btnShowPreview
            // 
            resources.ApplyResources(this.btnShowPreview, "btnShowPreview");
            this.btnShowPreview.Name = "btnShowPreview";
            this.btnShowPreview.UseVisualStyleBackColor = true;
            this.btnShowPreview.Click += new System.EventHandler(this.btnShowPreview_Click);
            // 
            // btnPrint
            // 
            resources.ApplyResources(this.btnPrint, "btnPrint");
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PrintForm
            // 
            this.AcceptButton = this.btnPrint;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnShowPreview);
            this.Controls.Add(this.cbCenterImage);
            this.Controls.Add(this.cbAllowEnlarge);
            this.Controls.Add(this.lblMargin);
            this.Controls.Add(this.cbAutoScale);
            this.Controls.Add(this.nudMargin);
            this.Controls.Add(this.cbAutoRotate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintForm";
            this.Shown += new System.EventHandler(this.PrintForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudMargin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbAutoRotate;
        private System.Windows.Forms.NumericUpDown nudMargin;
        private System.Windows.Forms.CheckBox cbAutoScale;
        private System.Windows.Forms.Label lblMargin;
        private System.Windows.Forms.CheckBox cbAllowEnlarge;
        private System.Windows.Forms.CheckBox cbCenterImage;
        private System.Windows.Forms.Button btnShowPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnCancel;
    }
}