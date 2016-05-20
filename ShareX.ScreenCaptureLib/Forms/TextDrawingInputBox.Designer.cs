namespace ShareX.ScreenCaptureLib
{
    partial class TextDrawingInputBox
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
            this.components = new System.ComponentModel.Container();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnTextColor = new ShareX.HelpersLib.ColorButton();
            this.flpProperties = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFont = new System.Windows.Forms.Label();
            this.cbFonts = new System.Windows.Forms.ComboBox();
            this.lblTextSize = new System.Windows.Forms.Label();
            this.nudTextSize = new System.Windows.Forms.NumericUpDown();
            this.cbBold = new System.Windows.Forms.CheckBox();
            this.cbItalic = new System.Windows.Forms.CheckBox();
            this.cbUnderline = new System.Windows.Forms.CheckBox();
            this.btnAlignmentHorizontal = new System.Windows.Forms.Button();
            this.btnAlignmentVertical = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmsAlignmentHorizontal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAlignmentLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlignmentCenter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlignmentRight = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAlignmentVertical = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAlignmentTop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlignmentMiddle = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlignmentBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.flpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTextSize)).BeginInit();
            this.cmsAlignmentHorizontal.SuspendLayout();
            this.cmsAlignmentVertical.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Location = new System.Drawing.Point(8, 40);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(518, 281);
            this.txtInput.TabIndex = 0;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // btnTextColor
            // 
            this.btnTextColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnTextColor.Color = System.Drawing.Color.Empty;
            this.btnTextColor.Location = new System.Drawing.Point(319, 3);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Size = new System.Drawing.Size(24, 24);
            this.btnTextColor.TabIndex = 1;
            this.btnTextColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTextColor.UseVisualStyleBackColor = true;
            this.btnTextColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnTextColor_ColorChanged);
            // 
            // flpProperties
            // 
            this.flpProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpProperties.Controls.Add(this.lblFont);
            this.flpProperties.Controls.Add(this.cbFonts);
            this.flpProperties.Controls.Add(this.lblTextSize);
            this.flpProperties.Controls.Add(this.nudTextSize);
            this.flpProperties.Controls.Add(this.btnTextColor);
            this.flpProperties.Controls.Add(this.cbBold);
            this.flpProperties.Controls.Add(this.cbItalic);
            this.flpProperties.Controls.Add(this.cbUnderline);
            this.flpProperties.Controls.Add(this.btnAlignmentHorizontal);
            this.flpProperties.Controls.Add(this.btnAlignmentVertical);
            this.flpProperties.Location = new System.Drawing.Point(8, 5);
            this.flpProperties.Name = "flpProperties";
            this.flpProperties.Size = new System.Drawing.Size(518, 32);
            this.flpProperties.TabIndex = 2;
            this.flpProperties.WrapContents = false;
            // 
            // lblFont
            // 
            this.lblFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(3, 8);
            this.lblFont.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(31, 13);
            this.lblFont.TabIndex = 7;
            this.lblFont.Text = "Font:";
            // 
            // cbFonts
            // 
            this.cbFonts.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFonts.FormattingEnabled = true;
            this.cbFonts.Location = new System.Drawing.Point(37, 4);
            this.cbFonts.Name = "cbFonts";
            this.cbFonts.Size = new System.Drawing.Size(184, 21);
            this.cbFonts.TabIndex = 8;
            this.cbFonts.SelectedIndexChanged += new System.EventHandler(this.cbFonts_SelectedIndexChanged);
            // 
            // lblTextSize
            // 
            this.lblTextSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTextSize.AutoSize = true;
            this.lblTextSize.Location = new System.Drawing.Point(227, 8);
            this.lblTextSize.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblTextSize.Name = "lblTextSize";
            this.lblTextSize.Size = new System.Drawing.Size(30, 13);
            this.lblTextSize.TabIndex = 2;
            this.lblTextSize.Text = "Size:";
            // 
            // nudTextSize
            // 
            this.nudTextSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudTextSize.Location = new System.Drawing.Point(260, 5);
            this.nudTextSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudTextSize.Name = "nudTextSize";
            this.nudTextSize.Size = new System.Drawing.Size(53, 20);
            this.nudTextSize.TabIndex = 3;
            this.nudTextSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTextSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTextSize.ValueChanged += new System.EventHandler(this.nudTextSize_ValueChanged);
            // 
            // cbBold
            // 
            this.cbBold.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbBold.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbBold.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_bold;
            this.cbBold.Location = new System.Drawing.Point(349, 3);
            this.cbBold.Name = "cbBold";
            this.cbBold.Size = new System.Drawing.Size(24, 24);
            this.cbBold.TabIndex = 4;
            this.cbBold.UseVisualStyleBackColor = true;
            this.cbBold.CheckedChanged += new System.EventHandler(this.cbBold_CheckedChanged);
            // 
            // cbItalic
            // 
            this.cbItalic.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbItalic.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbItalic.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_italic;
            this.cbItalic.Location = new System.Drawing.Point(379, 3);
            this.cbItalic.Name = "cbItalic";
            this.cbItalic.Size = new System.Drawing.Size(24, 24);
            this.cbItalic.TabIndex = 5;
            this.cbItalic.UseVisualStyleBackColor = true;
            this.cbItalic.CheckedChanged += new System.EventHandler(this.cbItalic_CheckedChanged);
            // 
            // cbUnderline
            // 
            this.cbUnderline.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbUnderline.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbUnderline.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_underline;
            this.cbUnderline.Location = new System.Drawing.Point(409, 3);
            this.cbUnderline.Name = "cbUnderline";
            this.cbUnderline.Size = new System.Drawing.Size(24, 24);
            this.cbUnderline.TabIndex = 6;
            this.cbUnderline.UseVisualStyleBackColor = true;
            this.cbUnderline.CheckedChanged += new System.EventHandler(this.cbUnderline_CheckedChanged);
            // 
            // btnAlignmentHorizontal
            // 
            this.btnAlignmentHorizontal.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAlignmentHorizontal.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_alignment_center;
            this.btnAlignmentHorizontal.Location = new System.Drawing.Point(439, 3);
            this.btnAlignmentHorizontal.Name = "btnAlignmentHorizontal";
            this.btnAlignmentHorizontal.Size = new System.Drawing.Size(24, 24);
            this.btnAlignmentHorizontal.TabIndex = 9;
            this.btnAlignmentHorizontal.UseVisualStyleBackColor = true;
            this.btnAlignmentHorizontal.Click += new System.EventHandler(this.btnAlignmentHorizontal_Click);
            // 
            // btnAlignmentVertical
            // 
            this.btnAlignmentVertical.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAlignmentVertical.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_vertical_alignment_middle;
            this.btnAlignmentVertical.Location = new System.Drawing.Point(469, 3);
            this.btnAlignmentVertical.Name = "btnAlignmentVertical";
            this.btnAlignmentVertical.Size = new System.Drawing.Size(24, 24);
            this.btnAlignmentVertical.TabIndex = 10;
            this.btnAlignmentVertical.UseVisualStyleBackColor = true;
            this.btnAlignmentVertical.Click += new System.EventHandler(this.btnAlignmentVertical_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(310, 329);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(104, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(422, 329);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmsAlignmentHorizontal
            // 
            this.cmsAlignmentHorizontal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAlignmentLeft,
            this.tsmiAlignmentCenter,
            this.tsmiAlignmentRight});
            this.cmsAlignmentHorizontal.Name = "cmsAlignmentHorizontal";
            this.cmsAlignmentHorizontal.Size = new System.Drawing.Size(110, 70);
            // 
            // tsmiAlignmentLeft
            // 
            this.tsmiAlignmentLeft.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_alignment;
            this.tsmiAlignmentLeft.Name = "tsmiAlignmentLeft";
            this.tsmiAlignmentLeft.Size = new System.Drawing.Size(109, 22);
            this.tsmiAlignmentLeft.Text = "Left";
            this.tsmiAlignmentLeft.Click += new System.EventHandler(this.tsmiAlignmentLeft_Click);
            // 
            // tsmiAlignmentCenter
            // 
            this.tsmiAlignmentCenter.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_alignment_center;
            this.tsmiAlignmentCenter.Name = "tsmiAlignmentCenter";
            this.tsmiAlignmentCenter.Size = new System.Drawing.Size(109, 22);
            this.tsmiAlignmentCenter.Text = "Center";
            this.tsmiAlignmentCenter.Click += new System.EventHandler(this.tsmiAlignmentCenter_Click);
            // 
            // tsmiAlignmentRight
            // 
            this.tsmiAlignmentRight.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_alignment_right;
            this.tsmiAlignmentRight.Name = "tsmiAlignmentRight";
            this.tsmiAlignmentRight.Size = new System.Drawing.Size(109, 22);
            this.tsmiAlignmentRight.Text = "Right";
            this.tsmiAlignmentRight.Click += new System.EventHandler(this.tsmiAlignmentRight_Click);
            // 
            // cmsAlignmentVertical
            // 
            this.cmsAlignmentVertical.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAlignmentTop,
            this.tsmiAlignmentMiddle,
            this.tsmiAlignmentBottom});
            this.cmsAlignmentVertical.Name = "cmsAlignmentVertical";
            this.cmsAlignmentVertical.Size = new System.Drawing.Size(115, 70);
            // 
            // tsmiAlignmentTop
            // 
            this.tsmiAlignmentTop.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_vertical_alignment_top;
            this.tsmiAlignmentTop.Name = "tsmiAlignmentTop";
            this.tsmiAlignmentTop.Size = new System.Drawing.Size(152, 22);
            this.tsmiAlignmentTop.Text = "Top";
            this.tsmiAlignmentTop.Click += new System.EventHandler(this.tsmiAlignmentTop_Click);
            // 
            // tsmiAlignmentMiddle
            // 
            this.tsmiAlignmentMiddle.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_vertical_alignment_middle;
            this.tsmiAlignmentMiddle.Name = "tsmiAlignmentMiddle";
            this.tsmiAlignmentMiddle.Size = new System.Drawing.Size(152, 22);
            this.tsmiAlignmentMiddle.Text = "Middle";
            this.tsmiAlignmentMiddle.Click += new System.EventHandler(this.tsmiAlignmentMiddle_Click);
            // 
            // tsmiAlignmentBottom
            // 
            this.tsmiAlignmentBottom.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.edit_vertical_alignment;
            this.tsmiAlignmentBottom.Name = "tsmiAlignmentBottom";
            this.tsmiAlignmentBottom.Size = new System.Drawing.Size(152, 22);
            this.tsmiAlignmentBottom.Text = "Bottom";
            this.tsmiAlignmentBottom.Click += new System.EventHandler(this.tsmiAlignmentBottom_Click);
            // 
            // TextDrawingInputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 361);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.flpProperties);
            this.Controls.Add(this.txtInput);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextDrawingInputBox";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Text input";
            this.TopMost = true;
            this.flpProperties.ResumeLayout(false);
            this.flpProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTextSize)).EndInit();
            this.cmsAlignmentHorizontal.ResumeLayout(false);
            this.cmsAlignmentVertical.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private HelpersLib.ColorButton btnTextColor;
        private System.Windows.Forms.FlowLayoutPanel flpProperties;
        private System.Windows.Forms.Label lblTextSize;
        private System.Windows.Forms.NumericUpDown nudTextSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbBold;
        private System.Windows.Forms.CheckBox cbItalic;
        private System.Windows.Forms.CheckBox cbUnderline;
        private System.Windows.Forms.Label lblFont;
        private System.Windows.Forms.ComboBox cbFonts;
        private System.Windows.Forms.Button btnAlignmentHorizontal;
        private System.Windows.Forms.Button btnAlignmentVertical;
        private System.Windows.Forms.ContextMenuStrip cmsAlignmentHorizontal;
        private System.Windows.Forms.ContextMenuStrip cmsAlignmentVertical;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlignmentLeft;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlignmentCenter;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlignmentRight;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlignmentTop;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlignmentMiddle;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlignmentBottom;
    }
}