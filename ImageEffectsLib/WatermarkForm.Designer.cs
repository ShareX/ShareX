namespace ImageEffectsLib
{
    partial class WatermarkForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WatermarkForm));
            this.lblWatermarkOffsetPixel = new System.Windows.Forms.Label();
            this.cboWatermarkType = new System.Windows.Forms.ComboBox();
            this.cbWatermarkAutoHide = new System.Windows.Forms.CheckBox();
            this.lblWatermarkType = new System.Windows.Forms.Label();
            this.chkWatermarkPosition = new System.Windows.Forms.ComboBox();
            this.lblWatermarkPosition = new System.Windows.Forms.Label();
            this.nudWatermarkOffset = new System.Windows.Forms.NumericUpDown();
            this.lblWatermarkOffset = new System.Windows.Forms.Label();
            this.gbWatermarkBackground = new System.Windows.Forms.GroupBox();
            this.btnBackgroundColor2 = new HelpersLib.ColorButton();
            this.btnBackgroundColor = new HelpersLib.ColorButton();
            this.btnBorderColor = new HelpersLib.ColorButton();
            this.cbWatermarkUseGradient = new System.Windows.Forms.CheckBox();
            this.cbWatermarkDrawBackground = new System.Windows.Forms.CheckBox();
            this.lblRectangleCornerRadius = new System.Windows.Forms.Label();
            this.nudWatermarkCornerRadius = new System.Windows.Forms.NumericUpDown();
            this.lblWatermarkCornerRadiusTip = new System.Windows.Forms.Label();
            this.cbWatermarkGradientType = new System.Windows.Forms.ComboBox();
            this.gbWatermarkText = new System.Windows.Forms.GroupBox();
            this.btnTextColor = new HelpersLib.ColorButton();
            this.lblWatermarkText = new System.Windows.Forms.Label();
            this.lblWatermarkFont = new System.Windows.Forms.Label();
            this.btnWatermarkFont = new System.Windows.Forms.Button();
            this.txtWatermarkText = new System.Windows.Forms.TextBox();
            this.btwWatermarkBrowseImage = new System.Windows.Forms.Button();
            this.txtWatermarkImageLocation = new System.Windows.Forms.TextBox();
            this.gbImageWatermark = new System.Windows.Forms.GroupBox();
            this.lblImageLocation = new System.Windows.Forms.Label();
            this.pbPreview = new HelpersLib.MyPictureBox();
            this.lblPreview = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatermarkOffset)).BeginInit();
            this.gbWatermarkBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatermarkCornerRadius)).BeginInit();
            this.gbWatermarkText.SuspendLayout();
            this.gbImageWatermark.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWatermarkOffsetPixel
            // 
            resources.ApplyResources(this.lblWatermarkOffsetPixel, "lblWatermarkOffsetPixel");
            this.lblWatermarkOffsetPixel.Name = "lblWatermarkOffsetPixel";
            // 
            // cboWatermarkType
            // 
            this.cboWatermarkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWatermarkType.FormattingEnabled = true;
            resources.ApplyResources(this.cboWatermarkType, "cboWatermarkType");
            this.cboWatermarkType.Name = "cboWatermarkType";
            this.cboWatermarkType.SelectedIndexChanged += new System.EventHandler(this.cboWatermarkType_SelectedIndexChanged);
            // 
            // cbWatermarkAutoHide
            // 
            resources.ApplyResources(this.cbWatermarkAutoHide, "cbWatermarkAutoHide");
            this.cbWatermarkAutoHide.Name = "cbWatermarkAutoHide";
            this.cbWatermarkAutoHide.UseVisualStyleBackColor = true;
            this.cbWatermarkAutoHide.CheckedChanged += new System.EventHandler(this.cbWatermarkAutoHide_CheckedChanged);
            // 
            // lblWatermarkType
            // 
            resources.ApplyResources(this.lblWatermarkType, "lblWatermarkType");
            this.lblWatermarkType.Name = "lblWatermarkType";
            // 
            // chkWatermarkPosition
            // 
            this.chkWatermarkPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chkWatermarkPosition.FormattingEnabled = true;
            resources.ApplyResources(this.chkWatermarkPosition, "chkWatermarkPosition");
            this.chkWatermarkPosition.Name = "chkWatermarkPosition";
            this.chkWatermarkPosition.SelectedIndexChanged += new System.EventHandler(this.cbWatermarkPosition_SelectedIndexChanged);
            // 
            // lblWatermarkPosition
            // 
            resources.ApplyResources(this.lblWatermarkPosition, "lblWatermarkPosition");
            this.lblWatermarkPosition.Name = "lblWatermarkPosition";
            // 
            // nudWatermarkOffset
            // 
            resources.ApplyResources(this.nudWatermarkOffset, "nudWatermarkOffset");
            this.nudWatermarkOffset.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudWatermarkOffset.Name = "nudWatermarkOffset";
            this.nudWatermarkOffset.ValueChanged += new System.EventHandler(this.nudWatermarkOffset_ValueChanged);
            // 
            // lblWatermarkOffset
            // 
            resources.ApplyResources(this.lblWatermarkOffset, "lblWatermarkOffset");
            this.lblWatermarkOffset.Name = "lblWatermarkOffset";
            // 
            // gbWatermarkBackground
            // 
            this.gbWatermarkBackground.Controls.Add(this.btnBackgroundColor2);
            this.gbWatermarkBackground.Controls.Add(this.btnBackgroundColor);
            this.gbWatermarkBackground.Controls.Add(this.btnBorderColor);
            this.gbWatermarkBackground.Controls.Add(this.cbWatermarkUseGradient);
            this.gbWatermarkBackground.Controls.Add(this.cbWatermarkDrawBackground);
            this.gbWatermarkBackground.Controls.Add(this.lblRectangleCornerRadius);
            this.gbWatermarkBackground.Controls.Add(this.nudWatermarkCornerRadius);
            this.gbWatermarkBackground.Controls.Add(this.lblWatermarkCornerRadiusTip);
            this.gbWatermarkBackground.Controls.Add(this.cbWatermarkGradientType);
            resources.ApplyResources(this.gbWatermarkBackground, "gbWatermarkBackground");
            this.gbWatermarkBackground.Name = "gbWatermarkBackground";
            this.gbWatermarkBackground.TabStop = false;
            // 
            // btnBackgroundColor2
            // 
            this.btnBackgroundColor2.Color = System.Drawing.Color.Empty;
            resources.ApplyResources(this.btnBackgroundColor2, "btnBackgroundColor2");
            this.btnBackgroundColor2.Name = "btnBackgroundColor2";
            this.btnBackgroundColor2.UseVisualStyleBackColor = true;
            this.btnBackgroundColor2.ColorChanged += new HelpersLib.ColorButton.ColorChangedEventHandler(this.btnBackgroundColor2_ColorChanged);
            // 
            // btnBackgroundColor
            // 
            this.btnBackgroundColor.Color = System.Drawing.Color.Empty;
            resources.ApplyResources(this.btnBackgroundColor, "btnBackgroundColor");
            this.btnBackgroundColor.Name = "btnBackgroundColor";
            this.btnBackgroundColor.UseVisualStyleBackColor = true;
            this.btnBackgroundColor.ColorChanged += new HelpersLib.ColorButton.ColorChangedEventHandler(this.btnBackgroundColor_ColorChanged);
            // 
            // btnBorderColor
            // 
            this.btnBorderColor.Color = System.Drawing.Color.Empty;
            resources.ApplyResources(this.btnBorderColor, "btnBorderColor");
            this.btnBorderColor.Name = "btnBorderColor";
            this.btnBorderColor.UseVisualStyleBackColor = true;
            this.btnBorderColor.ColorChanged += new HelpersLib.ColorButton.ColorChangedEventHandler(this.btnBorderColor_ColorChanged);
            // 
            // cbWatermarkUseGradient
            // 
            resources.ApplyResources(this.cbWatermarkUseGradient, "cbWatermarkUseGradient");
            this.cbWatermarkUseGradient.Checked = true;
            this.cbWatermarkUseGradient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWatermarkUseGradient.Name = "cbWatermarkUseGradient";
            this.cbWatermarkUseGradient.UseVisualStyleBackColor = true;
            this.cbWatermarkUseGradient.CheckedChanged += new System.EventHandler(this.cbWatermarkBackColor2_CheckedChanged);
            // 
            // cbWatermarkDrawBackground
            // 
            resources.ApplyResources(this.cbWatermarkDrawBackground, "cbWatermarkDrawBackground");
            this.cbWatermarkDrawBackground.Name = "cbWatermarkDrawBackground";
            this.cbWatermarkDrawBackground.UseVisualStyleBackColor = true;
            this.cbWatermarkDrawBackground.CheckedChanged += new System.EventHandler(this.cbWatermarkDrawBackground_CheckedChanged);
            // 
            // lblRectangleCornerRadius
            // 
            resources.ApplyResources(this.lblRectangleCornerRadius, "lblRectangleCornerRadius");
            this.lblRectangleCornerRadius.Name = "lblRectangleCornerRadius";
            // 
            // nudWatermarkCornerRadius
            // 
            resources.ApplyResources(this.nudWatermarkCornerRadius, "nudWatermarkCornerRadius");
            this.nudWatermarkCornerRadius.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudWatermarkCornerRadius.Name = "nudWatermarkCornerRadius";
            this.nudWatermarkCornerRadius.ValueChanged += new System.EventHandler(this.nudWatermarkCornerRadius_ValueChanged);
            // 
            // lblWatermarkCornerRadiusTip
            // 
            resources.ApplyResources(this.lblWatermarkCornerRadiusTip, "lblWatermarkCornerRadiusTip");
            this.lblWatermarkCornerRadiusTip.Name = "lblWatermarkCornerRadiusTip";
            // 
            // cbWatermarkGradientType
            // 
            this.cbWatermarkGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWatermarkGradientType.FormattingEnabled = true;
            resources.ApplyResources(this.cbWatermarkGradientType, "cbWatermarkGradientType");
            this.cbWatermarkGradientType.Name = "cbWatermarkGradientType";
            this.cbWatermarkGradientType.SelectedIndexChanged += new System.EventHandler(this.cbWatermarkGradientType_SelectedIndexChanged);
            // 
            // gbWatermarkText
            // 
            this.gbWatermarkText.Controls.Add(this.btnTextColor);
            this.gbWatermarkText.Controls.Add(this.lblWatermarkText);
            this.gbWatermarkText.Controls.Add(this.lblWatermarkFont);
            this.gbWatermarkText.Controls.Add(this.btnWatermarkFont);
            this.gbWatermarkText.Controls.Add(this.txtWatermarkText);
            resources.ApplyResources(this.gbWatermarkText, "gbWatermarkText");
            this.gbWatermarkText.Name = "gbWatermarkText";
            this.gbWatermarkText.TabStop = false;
            // 
            // btnTextColor
            // 
            this.btnTextColor.Color = System.Drawing.Color.Empty;
            resources.ApplyResources(this.btnTextColor, "btnTextColor");
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.UseVisualStyleBackColor = true;
            this.btnTextColor.ColorChanged += new HelpersLib.ColorButton.ColorChangedEventHandler(this.btnTextColor_ColorChanged);
            // 
            // lblWatermarkText
            // 
            resources.ApplyResources(this.lblWatermarkText, "lblWatermarkText");
            this.lblWatermarkText.Name = "lblWatermarkText";
            // 
            // lblWatermarkFont
            // 
            resources.ApplyResources(this.lblWatermarkFont, "lblWatermarkFont");
            this.lblWatermarkFont.Name = "lblWatermarkFont";
            // 
            // btnWatermarkFont
            // 
            resources.ApplyResources(this.btnWatermarkFont, "btnWatermarkFont");
            this.btnWatermarkFont.Name = "btnWatermarkFont";
            this.btnWatermarkFont.UseVisualStyleBackColor = true;
            this.btnWatermarkFont.Click += new System.EventHandler(this.btnWatermarkFont_Click);
            // 
            // txtWatermarkText
            // 
            resources.ApplyResources(this.txtWatermarkText, "txtWatermarkText");
            this.txtWatermarkText.Name = "txtWatermarkText";
            this.txtWatermarkText.TextChanged += new System.EventHandler(this.txtWatermarkText_TextChanged);
            // 
            // btwWatermarkBrowseImage
            // 
            resources.ApplyResources(this.btwWatermarkBrowseImage, "btwWatermarkBrowseImage");
            this.btwWatermarkBrowseImage.Name = "btwWatermarkBrowseImage";
            this.btwWatermarkBrowseImage.Tag = "Browse for a Watermark Image";
            this.btwWatermarkBrowseImage.UseVisualStyleBackColor = true;
            this.btwWatermarkBrowseImage.Click += new System.EventHandler(this.btwWatermarkBrowseImage_Click);
            // 
            // txtWatermarkImageLocation
            // 
            resources.ApplyResources(this.txtWatermarkImageLocation, "txtWatermarkImageLocation");
            this.txtWatermarkImageLocation.Name = "txtWatermarkImageLocation";
            this.txtWatermarkImageLocation.TextChanged += new System.EventHandler(this.txtWatermarkImageLocation_TextChanged);
            // 
            // gbImageWatermark
            // 
            this.gbImageWatermark.Controls.Add(this.lblImageLocation);
            this.gbImageWatermark.Controls.Add(this.txtWatermarkImageLocation);
            this.gbImageWatermark.Controls.Add(this.btwWatermarkBrowseImage);
            resources.ApplyResources(this.gbImageWatermark, "gbImageWatermark");
            this.gbImageWatermark.Name = "gbImageWatermark";
            this.gbImageWatermark.TabStop = false;
            // 
            // lblImageLocation
            // 
            resources.ApplyResources(this.lblImageLocation, "lblImageLocation");
            this.lblImageLocation.Name = "lblImageLocation";
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.Color.White;
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPreview.DrawCheckeredBackground = true;
            resources.ApplyResources(this.pbPreview, "pbPreview");
            this.pbPreview.Name = "pbPreview";
            // 
            // lblPreview
            // 
            resources.ApplyResources(this.lblPreview, "lblPreview");
            this.lblPreview.Name = "lblPreview";
            // 
            // WatermarkForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblWatermarkOffsetPixel);
            this.Controls.Add(this.gbImageWatermark);
            this.Controls.Add(this.cboWatermarkType);
            this.Controls.Add(this.pbPreview);
            this.Controls.Add(this.cbWatermarkAutoHide);
            this.Controls.Add(this.gbWatermarkBackground);
            this.Controls.Add(this.lblWatermarkType);
            this.Controls.Add(this.chkWatermarkPosition);
            this.Controls.Add(this.gbWatermarkText);
            this.Controls.Add(this.lblWatermarkPosition);
            this.Controls.Add(this.lblWatermarkOffset);
            this.Controls.Add(this.nudWatermarkOffset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WatermarkForm";
            this.Load += new System.EventHandler(this.WatermarkUI_Load);
            this.Resize += new System.EventHandler(this.WatermarkUI_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.nudWatermarkOffset)).EndInit();
            this.gbWatermarkBackground.ResumeLayout(false);
            this.gbWatermarkBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatermarkCornerRadius)).EndInit();
            this.gbWatermarkText.ResumeLayout(false);
            this.gbWatermarkText.PerformLayout();
            this.gbImageWatermark.ResumeLayout(false);
            this.gbImageWatermark.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWatermarkOffsetPixel;
        internal System.Windows.Forms.ComboBox cboWatermarkType;
        internal System.Windows.Forms.CheckBox cbWatermarkAutoHide;
        internal System.Windows.Forms.Label lblWatermarkType;
        internal System.Windows.Forms.ComboBox chkWatermarkPosition;
        internal System.Windows.Forms.Label lblWatermarkPosition;
        internal System.Windows.Forms.NumericUpDown nudWatermarkOffset;
        internal System.Windows.Forms.Label lblWatermarkOffset;
        internal System.Windows.Forms.GroupBox gbWatermarkBackground;
        internal System.Windows.Forms.Label lblRectangleCornerRadius;
        internal System.Windows.Forms.ComboBox cbWatermarkGradientType;
        internal System.Windows.Forms.NumericUpDown nudWatermarkCornerRadius;
        internal System.Windows.Forms.Label lblWatermarkCornerRadiusTip;
        internal System.Windows.Forms.GroupBox gbWatermarkText;
        internal System.Windows.Forms.Label lblWatermarkText;
        internal System.Windows.Forms.Label lblWatermarkFont;
        internal System.Windows.Forms.Button btnWatermarkFont;
        internal System.Windows.Forms.TextBox txtWatermarkText;
        internal System.Windows.Forms.Button btwWatermarkBrowseImage;
        internal System.Windows.Forms.TextBox txtWatermarkImageLocation;
        private HelpersLib.MyPictureBox pbPreview;
        private System.Windows.Forms.GroupBox gbImageWatermark;
        private System.Windows.Forms.CheckBox cbWatermarkDrawBackground;
        private System.Windows.Forms.CheckBox cbWatermarkUseGradient;
        private HelpersLib.ColorButton btnTextColor;
        private HelpersLib.ColorButton btnBackgroundColor2;
        private HelpersLib.ColorButton btnBackgroundColor;
        private HelpersLib.ColorButton btnBorderColor;
        internal System.Windows.Forms.Label lblImageLocation;
        private System.Windows.Forms.Label lblPreview;
    }
}