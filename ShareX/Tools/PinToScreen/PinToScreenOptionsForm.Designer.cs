namespace ShareX
{
    partial class PinToScreenOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PinToScreenOptionsForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPlacement = new System.Windows.Forms.Label();
            this.cbPlacement = new System.Windows.Forms.ComboBox();
            this.lblPlacementOffset = new System.Windows.Forms.Label();
            this.nudPlacementOffset = new System.Windows.Forms.NumericUpDown();
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.cbKeepCenterLocation = new System.Windows.Forms.CheckBox();
            this.cbShadow = new System.Windows.Forms.CheckBox();
            this.cbBorder = new System.Windows.Forms.CheckBox();
            this.lblBorderSize = new System.Windows.Forms.Label();
            this.nudBorderSize = new System.Windows.Forms.NumericUpDown();
            this.btnBorderColor = new ShareX.HelpersLib.ColorButton();
            this.lblMinimizeSize = new System.Windows.Forms.Label();
            this.nudMinimizeSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.nudMinimizeSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.lblMinimizeSizeX = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlacementOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBorderSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeHeight)).BeginInit();
            this.SuspendLayout();
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
            // lblPlacement
            // 
            resources.ApplyResources(this.lblPlacement, "lblPlacement");
            this.lblPlacement.Name = "lblPlacement";
            // 
            // cbPlacement
            // 
            this.cbPlacement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPlacement.FormattingEnabled = true;
            resources.ApplyResources(this.cbPlacement, "cbPlacement");
            this.cbPlacement.Name = "cbPlacement";
            // 
            // lblPlacementOffset
            // 
            resources.ApplyResources(this.lblPlacementOffset, "lblPlacementOffset");
            this.lblPlacementOffset.Name = "lblPlacementOffset";
            // 
            // nudPlacementOffset
            // 
            resources.ApplyResources(this.nudPlacementOffset, "nudPlacementOffset");
            this.nudPlacementOffset.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudPlacementOffset.Name = "nudPlacementOffset";
            // 
            // cbTopMost
            // 
            resources.ApplyResources(this.cbTopMost, "cbTopMost");
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.UseVisualStyleBackColor = true;
            // 
            // cbKeepCenterLocation
            // 
            resources.ApplyResources(this.cbKeepCenterLocation, "cbKeepCenterLocation");
            this.cbKeepCenterLocation.Name = "cbKeepCenterLocation";
            this.cbKeepCenterLocation.UseVisualStyleBackColor = true;
            // 
            // cbShadow
            // 
            resources.ApplyResources(this.cbShadow, "cbShadow");
            this.cbShadow.Name = "cbShadow";
            this.cbShadow.UseVisualStyleBackColor = true;
            // 
            // cbBorder
            // 
            resources.ApplyResources(this.cbBorder, "cbBorder");
            this.cbBorder.Name = "cbBorder";
            this.cbBorder.UseVisualStyleBackColor = true;
            // 
            // lblBorderSize
            // 
            resources.ApplyResources(this.lblBorderSize, "lblBorderSize");
            this.lblBorderSize.Name = "lblBorderSize";
            // 
            // nudBorderSize
            // 
            resources.ApplyResources(this.nudBorderSize, "nudBorderSize");
            this.nudBorderSize.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudBorderSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBorderSize.Name = "nudBorderSize";
            this.nudBorderSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnBorderColor
            // 
            this.btnBorderColor.Color = System.Drawing.Color.Empty;
            this.btnBorderColor.ColorPickerOptions = null;
            resources.ApplyResources(this.btnBorderColor, "btnBorderColor");
            this.btnBorderColor.Name = "btnBorderColor";
            this.btnBorderColor.UseVisualStyleBackColor = true;
            // 
            // lblMinimizeSize
            // 
            resources.ApplyResources(this.lblMinimizeSize, "lblMinimizeSize");
            this.lblMinimizeSize.Name = "lblMinimizeSize";
            // 
            // nudMinimizeSizeWidth
            // 
            resources.ApplyResources(this.nudMinimizeSizeWidth, "nudMinimizeSizeWidth");
            this.nudMinimizeSizeWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMinimizeSizeWidth.Name = "nudMinimizeSizeWidth";
            // 
            // nudMinimizeSizeHeight
            // 
            resources.ApplyResources(this.nudMinimizeSizeHeight, "nudMinimizeSizeHeight");
            this.nudMinimizeSizeHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMinimizeSizeHeight.Name = "nudMinimizeSizeHeight";
            // 
            // lblMinimizeSizeX
            // 
            resources.ApplyResources(this.lblMinimizeSizeX, "lblMinimizeSizeX");
            this.lblMinimizeSizeX.Name = "lblMinimizeSizeX";
            // 
            // PinToScreenOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblMinimizeSizeX);
            this.Controls.Add(this.nudMinimizeSizeHeight);
            this.Controls.Add(this.nudMinimizeSizeWidth);
            this.Controls.Add(this.lblMinimizeSize);
            this.Controls.Add(this.btnBorderColor);
            this.Controls.Add(this.nudBorderSize);
            this.Controls.Add(this.lblBorderSize);
            this.Controls.Add(this.cbBorder);
            this.Controls.Add(this.cbShadow);
            this.Controls.Add(this.cbKeepCenterLocation);
            this.Controls.Add(this.cbTopMost);
            this.Controls.Add(this.nudPlacementOffset);
            this.Controls.Add(this.lblPlacementOffset);
            this.Controls.Add(this.cbPlacement);
            this.Controls.Add(this.lblPlacement);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PinToScreenOptionsForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudPlacementOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBorderSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPlacement;
        private System.Windows.Forms.ComboBox cbPlacement;
        private System.Windows.Forms.Label lblPlacementOffset;
        private System.Windows.Forms.NumericUpDown nudPlacementOffset;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.CheckBox cbKeepCenterLocation;
        private System.Windows.Forms.CheckBox cbShadow;
        private System.Windows.Forms.CheckBox cbBorder;
        private System.Windows.Forms.Label lblBorderSize;
        private System.Windows.Forms.NumericUpDown nudBorderSize;
        private HelpersLib.ColorButton btnBorderColor;
        private System.Windows.Forms.Label lblMinimizeSize;
        private System.Windows.Forms.NumericUpDown nudMinimizeSizeWidth;
        private System.Windows.Forms.NumericUpDown nudMinimizeSizeHeight;
        private System.Windows.Forms.Label lblMinimizeSizeX;
    }
}