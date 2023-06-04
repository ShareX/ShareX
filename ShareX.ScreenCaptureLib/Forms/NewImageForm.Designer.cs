namespace ShareX.ScreenCaptureLib
{
    partial class NewImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewImageForm));
            this.lblHeightPixels = new System.Windows.Forms.Label();
            this.lblWidthPixels = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.cbTransparent = new System.Windows.Forms.CheckBox();
            this.btnChangeColor = new ShareX.HelpersLib.ColorButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbBackgroundColor = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            this.gbBackgroundColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeightPixels
            // 
            resources.ApplyResources(this.lblHeightPixels, "lblHeightPixels");
            this.lblHeightPixels.Name = "lblHeightPixels";
            // 
            // lblWidthPixels
            // 
            resources.ApplyResources(this.lblWidthPixels, "lblWidthPixels");
            this.lblWidthPixels.Name = "lblWidthPixels";
            // 
            // lblHeight
            // 
            resources.ApplyResources(this.lblHeight, "lblHeight");
            this.lblHeight.Name = "lblHeight";
            // 
            // lblWidth
            // 
            resources.ApplyResources(this.lblWidth, "lblWidth");
            this.lblWidth.Name = "lblWidth";
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
            this.nudHeight.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
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
            this.nudWidth.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // cbTransparent
            // 
            resources.ApplyResources(this.cbTransparent, "cbTransparent");
            this.cbTransparent.Name = "cbTransparent";
            this.cbTransparent.UseVisualStyleBackColor = true;
            this.cbTransparent.CheckedChanged += new System.EventHandler(this.cbTransparent_CheckedChanged);
            // 
            // btnChangeColor
            // 
            this.btnChangeColor.Color = System.Drawing.Color.Empty;
            this.btnChangeColor.ColorPickerOptions = null;
            resources.ApplyResources(this.btnChangeColor, "btnChangeColor");
            this.btnChangeColor.Name = "btnChangeColor";
            this.btnChangeColor.UseVisualStyleBackColor = true;
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
            // gbBackgroundColor
            // 
            this.gbBackgroundColor.Controls.Add(this.cbTransparent);
            this.gbBackgroundColor.Controls.Add(this.btnChangeColor);
            resources.ApplyResources(this.gbBackgroundColor, "gbBackgroundColor");
            this.gbBackgroundColor.Name = "gbBackgroundColor";
            this.gbBackgroundColor.TabStop = false;
            // 
            // NewImageForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.gbBackgroundColor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblHeightPixels);
            this.Controls.Add(this.lblWidthPixels);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.nudWidth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewImageForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            this.gbBackgroundColor.ResumeLayout(false);
            this.gbBackgroundColor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeightPixels;
        private System.Windows.Forms.Label lblWidthPixels;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.CheckBox cbTransparent;
        private HelpersLib.ColorButton btnChangeColor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbBackgroundColor;
    }
}