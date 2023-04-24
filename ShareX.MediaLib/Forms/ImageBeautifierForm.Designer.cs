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
            this.lblMarginValue = new System.Windows.Forms.Label();
            this.lblPaddingValue = new System.Windows.Forms.Label();
            this.lblRoundedCornerValue = new System.Windows.Forms.Label();
            this.lblShadowSizeValue = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pbPreview = new ShareX.HelpersLib.MyPictureBox();
            this.pOptions = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.pbBackground = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPadding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRoundedCorner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowSize)).BeginInit();
            this.tlpMain.SuspendLayout();
            this.pOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMargin
            // 
            this.lblMargin.AutoSize = true;
            this.lblMargin.Location = new System.Drawing.Point(13, 16);
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
            this.tbMargin.Size = new System.Drawing.Size(296, 45);
            this.tbMargin.TabIndex = 1;
            this.tbMargin.TickFrequency = 10;
            this.tbMargin.Scroll += new System.EventHandler(this.tbMargin_Scroll);
            // 
            // lblPadding
            // 
            this.lblPadding.AutoSize = true;
            this.lblPadding.Location = new System.Drawing.Point(13, 88);
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
            this.tbPadding.Size = new System.Drawing.Size(296, 45);
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
            this.lblRoundedCorner.Location = new System.Drawing.Point(13, 192);
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
            this.tbRoundedCorner.Size = new System.Drawing.Size(296, 45);
            this.tbRoundedCorner.TabIndex = 8;
            this.tbRoundedCorner.TickFrequency = 5;
            this.tbRoundedCorner.Scroll += new System.EventHandler(this.tbRoundedCorner_Scroll);
            // 
            // lblShadowSize
            // 
            this.lblShadowSize.AutoSize = true;
            this.lblShadowSize.Location = new System.Drawing.Point(13, 264);
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
            this.tbShadowSize.Size = new System.Drawing.Size(296, 45);
            this.tbShadowSize.TabIndex = 11;
            this.tbShadowSize.TickFrequency = 5;
            this.tbShadowSize.Scroll += new System.EventHandler(this.tbShadowSize_Scroll);
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(13, 336);
            this.lblBackground.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(80, 17);
            this.lblBackground.TabIndex = 13;
            this.lblBackground.Text = "Background:";
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
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 335F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pbPreview, 1, 0);
            this.tlpMain.Controls.Add(this.pOptions, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1384, 761);
            this.tlpMain.TabIndex = 15;
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.SystemColors.Window;
            this.pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.EnableRightClickMenu = true;
            this.pbPreview.FullscreenOnClick = true;
            this.pbPreview.Location = new System.Drawing.Point(335, 0);
            this.pbPreview.Margin = new System.Windows.Forms.Padding(0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbPreview.ShowImageSizeLabel = true;
            this.pbPreview.Size = new System.Drawing.Size(1049, 761);
            this.pbPreview.TabIndex = 12;
            // 
            // pOptions
            // 
            this.pOptions.Controls.Add(this.btnPrint);
            this.pOptions.Controls.Add(this.btnSave);
            this.pOptions.Controls.Add(this.btnUpload);
            this.pOptions.Controls.Add(this.btnSaveAs);
            this.pOptions.Controls.Add(this.btnCopy);
            this.pOptions.Controls.Add(this.pbBackground);
            this.pOptions.Controls.Add(this.lblMargin);
            this.pOptions.Controls.Add(this.lblShadowSizeValue);
            this.pOptions.Controls.Add(this.tbMargin);
            this.pOptions.Controls.Add(this.lblRoundedCornerValue);
            this.pOptions.Controls.Add(this.lblPadding);
            this.pOptions.Controls.Add(this.lblPaddingValue);
            this.pOptions.Controls.Add(this.tbPadding);
            this.pOptions.Controls.Add(this.lblMarginValue);
            this.pOptions.Controls.Add(this.cbSmartPadding);
            this.pOptions.Controls.Add(this.lblRoundedCorner);
            this.pOptions.Controls.Add(this.lblBackground);
            this.pOptions.Controls.Add(this.tbRoundedCorner);
            this.pOptions.Controls.Add(this.tbShadowSize);
            this.pOptions.Controls.Add(this.lblShadowSize);
            this.pOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pOptions.Location = new System.Drawing.Point(3, 3);
            this.pOptions.Name = "pOptions";
            this.pOptions.Size = new System.Drawing.Size(329, 755);
            this.pOptions.TabIndex = 0;
            // 
            // btnPrint
            // 
            this.btnPrint.Image = global::ShareX.MediaLib.Properties.Resources.printer;
            this.btnPrint.Location = new System.Drawing.Point(264, 696);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(56, 48);
            this.btnPrint.TabIndex = 19;
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Image = global::ShareX.MediaLib.Properties.Resources.disk_black;
            this.btnSave.Location = new System.Drawing.Point(72, 696);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 48);
            this.btnSave.TabIndex = 18;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpload.Image = global::ShareX.MediaLib.Properties.Resources.upload_cloud;
            this.btnUpload.Location = new System.Drawing.Point(200, 696);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(56, 48);
            this.btnUpload.TabIndex = 17;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveAs.Image = global::ShareX.MediaLib.Properties.Resources.disks_black;
            this.btnSaveAs.Location = new System.Drawing.Point(136, 696);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(56, 48);
            this.btnSaveAs.TabIndex = 16;
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Image = global::ShareX.MediaLib.Properties.Resources.document_copy;
            this.btnCopy.Location = new System.Drawing.Point(8, 696);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(56, 48);
            this.btnCopy.TabIndex = 15;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // pbBackground
            // 
            this.pbBackground.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbBackground.Location = new System.Drawing.Point(16, 360);
            this.pbBackground.Name = "pbBackground";
            this.pbBackground.Size = new System.Drawing.Size(296, 40);
            this.pbBackground.TabIndex = 14;
            this.pbBackground.TabStop = false;
            this.pbBackground.Click += new System.EventHandler(this.pbBackground_Click);
            // 
            // ImageBeautifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "ImageBeautifierForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image beautifier";
            this.Shown += new System.EventHandler(this.ImageBeautifierForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.tbMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPadding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRoundedCorner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowSize)).EndInit();
            this.tlpMain.ResumeLayout(false);
            this.pOptions.ResumeLayout(false);
            this.pOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Label lblMarginValue;
        private System.Windows.Forms.Label lblPaddingValue;
        private System.Windows.Forms.Label lblRoundedCornerValue;
        private System.Windows.Forms.Label lblShadowSizeValue;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pOptions;
        private HelpersLib.MyPictureBox pbPreview;
        private System.Windows.Forms.PictureBox pbBackground;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPrint;
    }
}