namespace HelpersLib
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
            this.cbAutoRotate.AutoSize = true;
            this.cbAutoRotate.Location = new System.Drawing.Point(16, 48);
            this.cbAutoRotate.Name = "cbAutoRotate";
            this.cbAutoRotate.Size = new System.Drawing.Size(109, 17);
            this.cbAutoRotate.TabIndex = 0;
            this.cbAutoRotate.Text = "Auto rotate image";
            this.cbAutoRotate.UseVisualStyleBackColor = true;
            this.cbAutoRotate.CheckedChanged += new System.EventHandler(this.cbAutoRotate_CheckedChanged);
            // 
            // nudMargin
            // 
            this.nudMargin.Location = new System.Drawing.Point(56, 12);
            this.nudMargin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMargin.Name = "nudMargin";
            this.nudMargin.Size = new System.Drawing.Size(56, 20);
            this.nudMargin.TabIndex = 1;
            this.nudMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudMargin.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMargin.ValueChanged += new System.EventHandler(this.nudMargin_ValueChanged);
            // 
            // cbAutoScale
            // 
            this.cbAutoScale.AutoSize = true;
            this.cbAutoScale.Location = new System.Drawing.Point(16, 72);
            this.cbAutoScale.Name = "cbAutoScale";
            this.cbAutoScale.Size = new System.Drawing.Size(107, 17);
            this.cbAutoScale.TabIndex = 2;
            this.cbAutoScale.Text = "Auto scale image";
            this.cbAutoScale.UseVisualStyleBackColor = true;
            this.cbAutoScale.CheckedChanged += new System.EventHandler(this.cbAutoScale_CheckedChanged);
            // 
            // lblMargin
            // 
            this.lblMargin.AutoSize = true;
            this.lblMargin.Location = new System.Drawing.Point(8, 16);
            this.lblMargin.Name = "lblMargin";
            this.lblMargin.Size = new System.Drawing.Size(42, 13);
            this.lblMargin.TabIndex = 3;
            this.lblMargin.Text = "Margin:";
            // 
            // cbAllowEnlarge
            // 
            this.cbAllowEnlarge.AutoSize = true;
            this.cbAllowEnlarge.Location = new System.Drawing.Point(40, 96);
            this.cbAllowEnlarge.Name = "cbAllowEnlarge";
            this.cbAllowEnlarge.Size = new System.Drawing.Size(132, 17);
            this.cbAllowEnlarge.TabIndex = 4;
            this.cbAllowEnlarge.Text = "Allow image to enlarge";
            this.cbAllowEnlarge.UseVisualStyleBackColor = true;
            this.cbAllowEnlarge.CheckedChanged += new System.EventHandler(this.cbAllowEnlarge_CheckedChanged);
            // 
            // cbCenterImage
            // 
            this.cbCenterImage.AutoSize = true;
            this.cbCenterImage.Location = new System.Drawing.Point(40, 120);
            this.cbCenterImage.Name = "cbCenterImage";
            this.cbCenterImage.Size = new System.Drawing.Size(127, 17);
            this.cbCenterImage.TabIndex = 5;
            this.cbCenterImage.Text = "Center image position";
            this.cbCenterImage.UseVisualStyleBackColor = true;
            this.cbCenterImage.CheckedChanged += new System.EventHandler(this.cbCenterImage_CheckedChanged);
            // 
            // btnShowPreview
            // 
            this.btnShowPreview.Location = new System.Drawing.Point(88, 152);
            this.btnShowPreview.Name = "btnShowPreview";
            this.btnShowPreview.Size = new System.Drawing.Size(75, 23);
            this.btnShowPreview.TabIndex = 6;
            this.btnShowPreview.Text = "Preview...";
            this.btnShowPreview.UseVisualStyleBackColor = true;
            this.btnShowPreview.Click += new System.EventHandler(this.btnShowPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(8, 152);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print...";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(168, 152);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 185);
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
            this.Name = "PrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Print options";
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