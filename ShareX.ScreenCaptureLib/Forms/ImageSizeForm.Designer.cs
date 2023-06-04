namespace ShareX.ScreenCaptureLib
{
    partial class ImageSizeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageSizeForm));
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.cbAspectRatio = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblWidthPixels = new System.Windows.Forms.Label();
            this.lblHeightPixels = new System.Windows.Forms.Label();
            this.lblResampling = new System.Windows.Forms.Label();
            this.cbResampling = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // nudWidth
            // 
            resources.ApplyResources(this.nudWidth, "nudWidth");
            this.nudWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            // 
            // nudHeight
            // 
            resources.ApplyResources(this.nudHeight, "nudHeight");
            this.nudHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            // 
            // lblWidth
            // 
            resources.ApplyResources(this.lblWidth, "lblWidth");
            this.lblWidth.Name = "lblWidth";
            // 
            // lblHeight
            // 
            resources.ApplyResources(this.lblHeight, "lblHeight");
            this.lblHeight.Name = "lblHeight";
            // 
            // cbAspectRatio
            // 
            resources.ApplyResources(this.cbAspectRatio, "cbAspectRatio");
            this.cbAspectRatio.Checked = true;
            this.cbAspectRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAspectRatio.Name = "cbAspectRatio";
            this.cbAspectRatio.UseVisualStyleBackColor = true;
            this.cbAspectRatio.CheckedChanged += new System.EventHandler(this.cbAspectRatio_CheckedChanged);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblWidthPixels
            // 
            resources.ApplyResources(this.lblWidthPixels, "lblWidthPixels");
            this.lblWidthPixels.Name = "lblWidthPixels";
            // 
            // lblHeightPixels
            // 
            resources.ApplyResources(this.lblHeightPixels, "lblHeightPixels");
            this.lblHeightPixels.Name = "lblHeightPixels";
            // 
            // lblResampling
            // 
            resources.ApplyResources(this.lblResampling, "lblResampling");
            this.lblResampling.Name = "lblResampling";
            // 
            // cbResampling
            // 
            this.cbResampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbResampling.FormattingEnabled = true;
            resources.ApplyResources(this.cbResampling, "cbResampling");
            this.cbResampling.Name = "cbResampling";
            this.cbResampling.SelectedIndexChanged += new System.EventHandler(this.cbResampling_SelectedIndexChanged);
            // 
            // ImageSizeForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbResampling);
            this.Controls.Add(this.lblResampling);
            this.Controls.Add(this.lblHeightPixels);
            this.Controls.Add(this.lblWidthPixels);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbAspectRatio);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.nudWidth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageSizeForm";
            this.Shown += new System.EventHandler(this.ResizeSizeForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.CheckBox cbAspectRatio;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblWidthPixels;
        private System.Windows.Forms.Label lblHeightPixels;
        private System.Windows.Forms.Label lblResampling;
        private System.Windows.Forms.ComboBox cbResampling;
    }
}