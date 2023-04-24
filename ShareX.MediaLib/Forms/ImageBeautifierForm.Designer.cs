namespace ShareX.MediaLib
{
    partial class ImageBeautifierForm
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
            this.lblMargin = new System.Windows.Forms.Label();
            this.tbMargin = new System.Windows.Forms.TrackBar();
            this.lblPadding = new System.Windows.Forms.Label();
            this.tbPadding = new System.Windows.Forms.TrackBar();
            this.cbSmartPadding = new System.Windows.Forms.CheckBox();
            this.lblRoundedCorner = new System.Windows.Forms.Label();
            this.tbRoundedCorner = new System.Windows.Forms.TrackBar();
            this.lblShadowSize = new System.Windows.Forms.Label();
            this.tbShadowSize = new System.Windows.Forms.TrackBar();
            this.lblBackground = new System.Windows.Forms.Label();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.pPreview = new System.Windows.Forms.Panel();
            this.lblMarginValue = new System.Windows.Forms.Label();
            this.lblPaddingValue = new System.Windows.Forms.Label();
            this.lblRoundedCornerValue = new System.Windows.Forms.Label();
            this.lblShadowSizeValue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPadding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRoundedCorner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.pPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMargin
            // 
            this.lblMargin.AutoSize = true;
            this.lblMargin.Location = new System.Drawing.Point(16, 16);
            this.lblMargin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMargin.Name = "lblMargin";
            this.lblMargin.Size = new System.Drawing.Size(53, 17);
            this.lblMargin.TabIndex = 0;
            this.lblMargin.Text = "Margin:";
            // 
            // tbMargin
            // 
            this.tbMargin.Location = new System.Drawing.Point(16, 40);
            this.tbMargin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbMargin.Maximum = 300;
            this.tbMargin.Name = "tbMargin";
            this.tbMargin.Size = new System.Drawing.Size(299, 45);
            this.tbMargin.TabIndex = 1;
            this.tbMargin.TickFrequency = 10;
            this.tbMargin.Scroll += new System.EventHandler(this.tbMargin_Scroll);
            // 
            // lblPadding
            // 
            this.lblPadding.AutoSize = true;
            this.lblPadding.Location = new System.Drawing.Point(16, 88);
            this.lblPadding.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPadding.Name = "lblPadding";
            this.lblPadding.Size = new System.Drawing.Size(59, 17);
            this.lblPadding.TabIndex = 3;
            this.lblPadding.Text = "Padding:";
            // 
            // tbPadding
            // 
            this.tbPadding.Location = new System.Drawing.Point(16, 112);
            this.tbPadding.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbPadding.Maximum = 200;
            this.tbPadding.Name = "tbPadding";
            this.tbPadding.Size = new System.Drawing.Size(299, 45);
            this.tbPadding.TabIndex = 4;
            this.tbPadding.TickFrequency = 10;
            this.tbPadding.Scroll += new System.EventHandler(this.tbPadding_Scroll);
            // 
            // cbSmartPadding
            // 
            this.cbSmartPadding.AutoSize = true;
            this.cbSmartPadding.Location = new System.Drawing.Point(16, 160);
            this.cbSmartPadding.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbSmartPadding.Name = "cbSmartPadding";
            this.cbSmartPadding.Size = new System.Drawing.Size(114, 21);
            this.cbSmartPadding.TabIndex = 6;
            this.cbSmartPadding.Text = "Smart padding";
            this.cbSmartPadding.UseVisualStyleBackColor = true;
            this.cbSmartPadding.CheckedChanged += new System.EventHandler(this.cbSmartPadding_CheckedChanged);
            // 
            // lblRoundedCorner
            // 
            this.lblRoundedCorner.AutoSize = true;
            this.lblRoundedCorner.Location = new System.Drawing.Point(16, 192);
            this.lblRoundedCorner.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRoundedCorner.Name = "lblRoundedCorner";
            this.lblRoundedCorner.Size = new System.Drawing.Size(106, 17);
            this.lblRoundedCorner.TabIndex = 7;
            this.lblRoundedCorner.Text = "Rounded corner:";
            // 
            // tbRoundedCorner
            // 
            this.tbRoundedCorner.Location = new System.Drawing.Point(16, 216);
            this.tbRoundedCorner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbRoundedCorner.Maximum = 50;
            this.tbRoundedCorner.Name = "tbRoundedCorner";
            this.tbRoundedCorner.Size = new System.Drawing.Size(299, 45);
            this.tbRoundedCorner.TabIndex = 8;
            this.tbRoundedCorner.TickFrequency = 5;
            this.tbRoundedCorner.Scroll += new System.EventHandler(this.tbRoundedCorner_Scroll);
            // 
            // lblShadowSize
            // 
            this.lblShadowSize.AutoSize = true;
            this.lblShadowSize.Location = new System.Drawing.Point(16, 264);
            this.lblShadowSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowSize.Name = "lblShadowSize";
            this.lblShadowSize.Size = new System.Drawing.Size(83, 17);
            this.lblShadowSize.TabIndex = 10;
            this.lblShadowSize.Text = "Shadow size:";
            // 
            // tbShadowSize
            // 
            this.tbShadowSize.Location = new System.Drawing.Point(16, 288);
            this.tbShadowSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbShadowSize.Maximum = 100;
            this.tbShadowSize.Name = "tbShadowSize";
            this.tbShadowSize.Size = new System.Drawing.Size(299, 45);
            this.tbShadowSize.TabIndex = 11;
            this.tbShadowSize.TickFrequency = 5;
            this.tbShadowSize.Scroll += new System.EventHandler(this.tbShadowSize_Scroll);
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(16, 336);
            this.lblBackground.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(80, 17);
            this.lblBackground.TabIndex = 13;
            this.lblBackground.Text = "Background:";
            // 
            // pbPreview
            // 
            this.pbPreview.Location = new System.Drawing.Point(0, 0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(100, 100);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbPreview.TabIndex = 11;
            this.pbPreview.TabStop = false;
            // 
            // pPreview
            // 
            this.pPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pPreview.AutoScroll = true;
            this.pPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pPreview.Controls.Add(this.pbPreview);
            this.pPreview.Location = new System.Drawing.Point(328, 8);
            this.pPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pPreview.Name = "pPreview";
            this.pPreview.Size = new System.Drawing.Size(1048, 745);
            this.pPreview.TabIndex = 14;
            // 
            // lblMarginValue
            // 
            this.lblMarginValue.Location = new System.Drawing.Point(272, 16);
            this.lblMarginValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMarginValue.Name = "lblMarginValue";
            this.lblMarginValue.Size = new System.Drawing.Size(40, 24);
            this.lblMarginValue.TabIndex = 2;
            this.lblMarginValue.Text = "0";
            this.lblMarginValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPaddingValue
            // 
            this.lblPaddingValue.Location = new System.Drawing.Point(272, 88);
            this.lblPaddingValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPaddingValue.Name = "lblPaddingValue";
            this.lblPaddingValue.Size = new System.Drawing.Size(40, 24);
            this.lblPaddingValue.TabIndex = 5;
            this.lblPaddingValue.Text = "0";
            this.lblPaddingValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRoundedCornerValue
            // 
            this.lblRoundedCornerValue.Location = new System.Drawing.Point(272, 192);
            this.lblRoundedCornerValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRoundedCornerValue.Name = "lblRoundedCornerValue";
            this.lblRoundedCornerValue.Size = new System.Drawing.Size(40, 24);
            this.lblRoundedCornerValue.TabIndex = 9;
            this.lblRoundedCornerValue.Text = "0";
            this.lblRoundedCornerValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblShadowSizeValue
            // 
            this.lblShadowSizeValue.Location = new System.Drawing.Point(272, 264);
            this.lblShadowSizeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowSizeValue.Name = "lblShadowSizeValue";
            this.lblShadowSizeValue.Size = new System.Drawing.Size(40, 24);
            this.lblShadowSizeValue.TabIndex = 12;
            this.lblShadowSizeValue.Text = "0";
            this.lblShadowSizeValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ImageBeautifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.lblShadowSizeValue);
            this.Controls.Add(this.lblRoundedCornerValue);
            this.Controls.Add(this.lblPaddingValue);
            this.Controls.Add(this.lblMarginValue);
            this.Controls.Add(this.pPreview);
            this.Controls.Add(this.lblBackground);
            this.Controls.Add(this.tbShadowSize);
            this.Controls.Add(this.lblShadowSize);
            this.Controls.Add(this.tbRoundedCorner);
            this.Controls.Add(this.lblRoundedCorner);
            this.Controls.Add(this.cbSmartPadding);
            this.Controls.Add(this.tbPadding);
            this.Controls.Add(this.lblPadding);
            this.Controls.Add(this.tbMargin);
            this.Controls.Add(this.lblMargin);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ImageBeautifierForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image beautifier";
            this.Shown += new System.EventHandler(this.ImageBeautifierForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.tbMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPadding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRoundedCorner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.pPreview.ResumeLayout(false);
            this.pPreview.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblMargin;
        private System.Windows.Forms.TrackBar tbMargin;
        private System.Windows.Forms.Label lblPadding;
        private System.Windows.Forms.TrackBar tbPadding;
        private System.Windows.Forms.CheckBox cbSmartPadding;
        private System.Windows.Forms.Label lblRoundedCorner;
        private System.Windows.Forms.TrackBar tbRoundedCorner;
        private System.Windows.Forms.Label lblShadowSize;
        private System.Windows.Forms.TrackBar tbShadowSize;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.Panel pPreview;
        private System.Windows.Forms.Label lblMarginValue;
        private System.Windows.Forms.Label lblPaddingValue;
        private System.Windows.Forms.Label lblRoundedCornerValue;
        private System.Windows.Forms.Label lblShadowSizeValue;
    }
}