namespace ShareX.ImageEffectsLib
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
            this.lblWatermarkOffsetPixel = new System.Windows.Forms.Label();
            this.cboWatermarkType = new System.Windows.Forms.ComboBox();
            this.cbWatermarkAutoHide = new System.Windows.Forms.CheckBox();
            this.lblWatermarkType = new System.Windows.Forms.Label();
            this.chkWatermarkPosition = new System.Windows.Forms.ComboBox();
            this.lblWatermarkPosition = new System.Windows.Forms.Label();
            this.nudWatermarkOffset = new System.Windows.Forms.NumericUpDown();
            this.lblWatermarkOffset = new System.Windows.Forms.Label();
            this.gbWatermarkBackground = new System.Windows.Forms.GroupBox();
            this.btnBackgroundColor2 = new ShareX.HelpersLib.ColorButton();
            this.btnBackgroundColor = new ShareX.HelpersLib.ColorButton();
            this.btnBorderColor = new ShareX.HelpersLib.ColorButton();
            this.cbWatermarkUseGradient = new System.Windows.Forms.CheckBox();
            this.cbWatermarkDrawBackground = new System.Windows.Forms.CheckBox();
            this.lblRectangleCornerRadius = new System.Windows.Forms.Label();
            this.nudWatermarkCornerRadius = new System.Windows.Forms.NumericUpDown();
            this.lblWatermarkCornerRadiusTip = new System.Windows.Forms.Label();
            this.cbWatermarkGradientType = new System.Windows.Forms.ComboBox();
            this.gbWatermarkText = new System.Windows.Forms.GroupBox();
            this.btnTextColor = new ShareX.HelpersLib.ColorButton();
            this.lblWatermarkText = new System.Windows.Forms.Label();
            this.lblWatermarkFont = new System.Windows.Forms.Label();
            this.btnWatermarkFont = new System.Windows.Forms.Button();
            this.txtWatermarkText = new System.Windows.Forms.TextBox();
            this.btwWatermarkBrowseImage = new System.Windows.Forms.Button();
            this.txtWatermarkImageLocation = new System.Windows.Forms.TextBox();
            this.gbImageWatermark = new System.Windows.Forms.GroupBox();
            this.lblImageLocation = new System.Windows.Forms.Label();
            this.pbPreview = new ShareX.HelpersLib.MyPictureBox();
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
            this.lblWatermarkOffsetPixel.AutoSize = true;
            this.lblWatermarkOffsetPixel.Location = new System.Drawing.Point(152, 80);
            this.lblWatermarkOffsetPixel.Name = "lblWatermarkOffsetPixel";
            this.lblWatermarkOffsetPixel.Size = new System.Drawing.Size(18, 13);
            this.lblWatermarkOffsetPixel.TabIndex = 6;
            this.lblWatermarkOffsetPixel.Text = "px";
            // 
            // cboWatermarkType
            // 
            this.cboWatermarkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWatermarkType.FormattingEnabled = true;
            this.cboWatermarkType.Location = new System.Drawing.Point(88, 12);
            this.cboWatermarkType.Name = "cboWatermarkType";
            this.cboWatermarkType.Size = new System.Drawing.Size(120, 21);
            this.cboWatermarkType.TabIndex = 1;
            this.cboWatermarkType.SelectedIndexChanged += new System.EventHandler(this.cboWatermarkType_SelectedIndexChanged);
            // 
            // cbWatermarkAutoHide
            // 
            this.cbWatermarkAutoHide.AutoSize = true;
            this.cbWatermarkAutoHide.Location = new System.Drawing.Point(17, 112);
            this.cbWatermarkAutoHide.Name = "cbWatermarkAutoHide";
            this.cbWatermarkAutoHide.Size = new System.Drawing.Size(260, 17);
            this.cbWatermarkAutoHide.TabIndex = 7;
            this.cbWatermarkAutoHide.Text = "Hide watermark if image is smaller than watermark";
            this.cbWatermarkAutoHide.UseVisualStyleBackColor = true;
            this.cbWatermarkAutoHide.CheckedChanged += new System.EventHandler(this.cbWatermarkAutoHide_CheckedChanged);
            // 
            // lblWatermarkType
            // 
            this.lblWatermarkType.AutoSize = true;
            this.lblWatermarkType.Location = new System.Drawing.Point(16, 16);
            this.lblWatermarkType.Name = "lblWatermarkType";
            this.lblWatermarkType.Size = new System.Drawing.Size(34, 13);
            this.lblWatermarkType.TabIndex = 0;
            this.lblWatermarkType.Text = "Type:";
            // 
            // chkWatermarkPosition
            // 
            this.chkWatermarkPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chkWatermarkPosition.FormattingEnabled = true;
            this.chkWatermarkPosition.Location = new System.Drawing.Point(88, 44);
            this.chkWatermarkPosition.Name = "chkWatermarkPosition";
            this.chkWatermarkPosition.Size = new System.Drawing.Size(120, 21);
            this.chkWatermarkPosition.TabIndex = 3;
            this.chkWatermarkPosition.SelectedIndexChanged += new System.EventHandler(this.cbWatermarkPosition_SelectedIndexChanged);
            // 
            // lblWatermarkPosition
            // 
            this.lblWatermarkPosition.AutoSize = true;
            this.lblWatermarkPosition.Location = new System.Drawing.Point(16, 48);
            this.lblWatermarkPosition.Name = "lblWatermarkPosition";
            this.lblWatermarkPosition.Size = new System.Drawing.Size(60, 13);
            this.lblWatermarkPosition.TabIndex = 2;
            this.lblWatermarkPosition.Text = "Placement:";
            // 
            // nudWatermarkOffset
            // 
            this.nudWatermarkOffset.Location = new System.Drawing.Point(88, 76);
            this.nudWatermarkOffset.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudWatermarkOffset.Name = "nudWatermarkOffset";
            this.nudWatermarkOffset.Size = new System.Drawing.Size(56, 20);
            this.nudWatermarkOffset.TabIndex = 5;
            this.nudWatermarkOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudWatermarkOffset.ValueChanged += new System.EventHandler(this.nudWatermarkOffset_ValueChanged);
            // 
            // lblWatermarkOffset
            // 
            this.lblWatermarkOffset.AutoSize = true;
            this.lblWatermarkOffset.Location = new System.Drawing.Point(16, 80);
            this.lblWatermarkOffset.Name = "lblWatermarkOffset";
            this.lblWatermarkOffset.Size = new System.Drawing.Size(38, 13);
            this.lblWatermarkOffset.TabIndex = 4;
            this.lblWatermarkOffset.Text = "Offset:";
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
            this.gbWatermarkBackground.Location = new System.Drawing.Point(304, 126);
            this.gbWatermarkBackground.Name = "gbWatermarkBackground";
            this.gbWatermarkBackground.Size = new System.Drawing.Size(448, 210);
            this.gbWatermarkBackground.TabIndex = 9;
            this.gbWatermarkBackground.TabStop = false;
            this.gbWatermarkBackground.Text = "Text background settings";
            // 
            // btnBackgroundColor2
            // 
            this.btnBackgroundColor2.Color = System.Drawing.Color.Empty;
            this.btnBackgroundColor2.Location = new System.Drawing.Point(16, 176);
            this.btnBackgroundColor2.Name = "btnBackgroundColor2";
            this.btnBackgroundColor2.Size = new System.Drawing.Size(144, 23);
            this.btnBackgroundColor2.TabIndex = 8;
            this.btnBackgroundColor2.Text = "Background color 2...";
            this.btnBackgroundColor2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackgroundColor2.UseVisualStyleBackColor = true;
            this.btnBackgroundColor2.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnBackgroundColor2_ColorChanged);
            // 
            // btnBackgroundColor
            // 
            this.btnBackgroundColor.Color = System.Drawing.Color.Empty;
            this.btnBackgroundColor.Location = new System.Drawing.Point(16, 112);
            this.btnBackgroundColor.Name = "btnBackgroundColor";
            this.btnBackgroundColor.Size = new System.Drawing.Size(144, 23);
            this.btnBackgroundColor.TabIndex = 5;
            this.btnBackgroundColor.Text = "Background color...";
            this.btnBackgroundColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackgroundColor.UseVisualStyleBackColor = true;
            this.btnBackgroundColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnBackgroundColor_ColorChanged);
            // 
            // btnBorderColor
            // 
            this.btnBorderColor.Color = System.Drawing.Color.Empty;
            this.btnBorderColor.Location = new System.Drawing.Point(16, 50);
            this.btnBorderColor.Name = "btnBorderColor";
            this.btnBorderColor.Size = new System.Drawing.Size(144, 23);
            this.btnBorderColor.TabIndex = 1;
            this.btnBorderColor.Text = "Border color...";
            this.btnBorderColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorderColor.UseVisualStyleBackColor = true;
            this.btnBorderColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnBorderColor_ColorChanged);
            // 
            // cbWatermarkUseGradient
            // 
            this.cbWatermarkUseGradient.AutoSize = true;
            this.cbWatermarkUseGradient.Checked = true;
            this.cbWatermarkUseGradient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWatermarkUseGradient.Location = new System.Drawing.Point(19, 149);
            this.cbWatermarkUseGradient.Name = "cbWatermarkUseGradient";
            this.cbWatermarkUseGradient.Size = new System.Drawing.Size(92, 17);
            this.cbWatermarkUseGradient.TabIndex = 6;
            this.cbWatermarkUseGradient.Text = "Gradient type:";
            this.cbWatermarkUseGradient.UseVisualStyleBackColor = true;
            this.cbWatermarkUseGradient.CheckedChanged += new System.EventHandler(this.cbWatermarkBackColor2_CheckedChanged);
            // 
            // cbWatermarkDrawBackground
            // 
            this.cbWatermarkDrawBackground.AutoSize = true;
            this.cbWatermarkDrawBackground.Location = new System.Drawing.Point(19, 24);
            this.cbWatermarkDrawBackground.Name = "cbWatermarkDrawBackground";
            this.cbWatermarkDrawBackground.Size = new System.Drawing.Size(111, 17);
            this.cbWatermarkDrawBackground.TabIndex = 0;
            this.cbWatermarkDrawBackground.Text = "Draw background";
            this.cbWatermarkDrawBackground.UseVisualStyleBackColor = true;
            this.cbWatermarkDrawBackground.CheckedChanged += new System.EventHandler(this.cbWatermarkDrawBackground_CheckedChanged);
            // 
            // lblRectangleCornerRadius
            // 
            this.lblRectangleCornerRadius.AutoSize = true;
            this.lblRectangleCornerRadius.Location = new System.Drawing.Point(16, 88);
            this.lblRectangleCornerRadius.Name = "lblRectangleCornerRadius";
            this.lblRectangleCornerRadius.Size = new System.Drawing.Size(123, 13);
            this.lblRectangleCornerRadius.TabIndex = 2;
            this.lblRectangleCornerRadius.Text = "Rectangle corner radius:";
            // 
            // nudWatermarkCornerRadius
            // 
            this.nudWatermarkCornerRadius.Location = new System.Drawing.Point(144, 84);
            this.nudWatermarkCornerRadius.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudWatermarkCornerRadius.Name = "nudWatermarkCornerRadius";
            this.nudWatermarkCornerRadius.Size = new System.Drawing.Size(48, 20);
            this.nudWatermarkCornerRadius.TabIndex = 3;
            this.nudWatermarkCornerRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudWatermarkCornerRadius.ValueChanged += new System.EventHandler(this.nudWatermarkCornerRadius_ValueChanged);
            // 
            // lblWatermarkCornerRadiusTip
            // 
            this.lblWatermarkCornerRadiusTip.AutoSize = true;
            this.lblWatermarkCornerRadiusTip.Location = new System.Drawing.Point(200, 88);
            this.lblWatermarkCornerRadiusTip.Name = "lblWatermarkCornerRadiusTip";
            this.lblWatermarkCornerRadiusTip.Size = new System.Drawing.Size(105, 13);
            this.lblWatermarkCornerRadiusTip.TabIndex = 4;
            this.lblWatermarkCornerRadiusTip.Text = "0 = Normal rectangle";
            // 
            // cbWatermarkGradientType
            // 
            this.cbWatermarkGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWatermarkGradientType.FormattingEnabled = true;
            this.cbWatermarkGradientType.Location = new System.Drawing.Point(120, 147);
            this.cbWatermarkGradientType.Name = "cbWatermarkGradientType";
            this.cbWatermarkGradientType.Size = new System.Drawing.Size(121, 21);
            this.cbWatermarkGradientType.TabIndex = 7;
            this.cbWatermarkGradientType.SelectedIndexChanged += new System.EventHandler(this.cbWatermarkGradientType_SelectedIndexChanged);
            // 
            // gbWatermarkText
            // 
            this.gbWatermarkText.Controls.Add(this.btnTextColor);
            this.gbWatermarkText.Controls.Add(this.lblWatermarkText);
            this.gbWatermarkText.Controls.Add(this.lblWatermarkFont);
            this.gbWatermarkText.Controls.Add(this.btnWatermarkFont);
            this.gbWatermarkText.Controls.Add(this.txtWatermarkText);
            this.gbWatermarkText.Location = new System.Drawing.Point(304, 8);
            this.gbWatermarkText.Name = "gbWatermarkText";
            this.gbWatermarkText.Size = new System.Drawing.Size(448, 112);
            this.gbWatermarkText.TabIndex = 8;
            this.gbWatermarkText.TabStop = false;
            this.gbWatermarkText.Text = "Text settings";
            // 
            // btnTextColor
            // 
            this.btnTextColor.Color = System.Drawing.Color.Empty;
            this.btnTextColor.Location = new System.Drawing.Point(16, 80);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Size = new System.Drawing.Size(144, 23);
            this.btnTextColor.TabIndex = 4;
            this.btnTextColor.Text = "Text color...";
            this.btnTextColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTextColor.UseVisualStyleBackColor = true;
            this.btnTextColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnTextColor_ColorChanged);
            // 
            // lblWatermarkText
            // 
            this.lblWatermarkText.AutoSize = true;
            this.lblWatermarkText.Location = new System.Drawing.Point(16, 24);
            this.lblWatermarkText.Name = "lblWatermarkText";
            this.lblWatermarkText.Size = new System.Drawing.Size(31, 13);
            this.lblWatermarkText.TabIndex = 0;
            this.lblWatermarkText.Text = "Text:";
            // 
            // lblWatermarkFont
            // 
            this.lblWatermarkFont.AutoSize = true;
            this.lblWatermarkFont.Location = new System.Drawing.Point(168, 54);
            this.lblWatermarkFont.Name = "lblWatermarkFont";
            this.lblWatermarkFont.Size = new System.Drawing.Size(83, 13);
            this.lblWatermarkFont.TabIndex = 3;
            this.lblWatermarkFont.Text = "Font Information";
            // 
            // btnWatermarkFont
            // 
            this.btnWatermarkFont.Location = new System.Drawing.Point(16, 48);
            this.btnWatermarkFont.Name = "btnWatermarkFont";
            this.btnWatermarkFont.Size = new System.Drawing.Size(144, 24);
            this.btnWatermarkFont.TabIndex = 2;
            this.btnWatermarkFont.Text = "Text font...";
            this.btnWatermarkFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWatermarkFont.UseVisualStyleBackColor = true;
            this.btnWatermarkFont.Click += new System.EventHandler(this.btnWatermarkFont_Click);
            // 
            // txtWatermarkText
            // 
            this.txtWatermarkText.Location = new System.Drawing.Point(56, 20);
            this.txtWatermarkText.Name = "txtWatermarkText";
            this.txtWatermarkText.Size = new System.Drawing.Size(384, 20);
            this.txtWatermarkText.TabIndex = 1;
            this.txtWatermarkText.TextChanged += new System.EventHandler(this.txtWatermarkText_TextChanged);
            // 
            // btwWatermarkBrowseImage
            // 
            this.btwWatermarkBrowseImage.Location = new System.Drawing.Point(376, 22);
            this.btwWatermarkBrowseImage.Name = "btwWatermarkBrowseImage";
            this.btwWatermarkBrowseImage.Size = new System.Drawing.Size(64, 24);
            this.btwWatermarkBrowseImage.TabIndex = 2;
            this.btwWatermarkBrowseImage.Tag = "Browse for a Watermark Image";
            this.btwWatermarkBrowseImage.Text = "Browse...";
            this.btwWatermarkBrowseImage.UseVisualStyleBackColor = true;
            this.btwWatermarkBrowseImage.Click += new System.EventHandler(this.btwWatermarkBrowseImage_Click);
            // 
            // txtWatermarkImageLocation
            // 
            this.txtWatermarkImageLocation.Location = new System.Drawing.Point(64, 24);
            this.txtWatermarkImageLocation.Name = "txtWatermarkImageLocation";
            this.txtWatermarkImageLocation.Size = new System.Drawing.Size(304, 20);
            this.txtWatermarkImageLocation.TabIndex = 1;
            this.txtWatermarkImageLocation.TextChanged += new System.EventHandler(this.txtWatermarkImageLocation_TextChanged);
            // 
            // gbImageWatermark
            // 
            this.gbImageWatermark.Controls.Add(this.lblImageLocation);
            this.gbImageWatermark.Controls.Add(this.txtWatermarkImageLocation);
            this.gbImageWatermark.Controls.Add(this.btwWatermarkBrowseImage);
            this.gbImageWatermark.Location = new System.Drawing.Point(304, 344);
            this.gbImageWatermark.Name = "gbImageWatermark";
            this.gbImageWatermark.Size = new System.Drawing.Size(448, 56);
            this.gbImageWatermark.TabIndex = 10;
            this.gbImageWatermark.TabStop = false;
            this.gbImageWatermark.Text = "Image settings";
            // 
            // lblImageLocation
            // 
            this.lblImageLocation.AutoSize = true;
            this.lblImageLocation.Location = new System.Drawing.Point(16, 28);
            this.lblImageLocation.Name = "lblImageLocation";
            this.lblImageLocation.Size = new System.Drawing.Size(39, 13);
            this.lblImageLocation.TabIndex = 0;
            this.lblImageLocation.Text = "Image:";
            // 
            // pbPreview
            // 
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.Location = new System.Drawing.Point(8, 160);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(288, 240);
            this.pbPreview.TabIndex = 12;
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(8, 144);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(102, 13);
            this.lblPreview.TabIndex = 11;
            this.lblPreview.Text = "Watermark preview:";
            // 
            // WatermarkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(761, 409);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Watermark settings";
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
        private ShareX.HelpersLib.MyPictureBox pbPreview;
        private System.Windows.Forms.GroupBox gbImageWatermark;
        private System.Windows.Forms.CheckBox cbWatermarkDrawBackground;
        private System.Windows.Forms.CheckBox cbWatermarkUseGradient;
        private ShareX.HelpersLib.ColorButton btnTextColor;
        private ShareX.HelpersLib.ColorButton btnBackgroundColor2;
        private ShareX.HelpersLib.ColorButton btnBackgroundColor;
        private ShareX.HelpersLib.ColorButton btnBorderColor;
        internal System.Windows.Forms.Label lblImageLocation;
        private System.Windows.Forms.Label lblPreview;
    }
}