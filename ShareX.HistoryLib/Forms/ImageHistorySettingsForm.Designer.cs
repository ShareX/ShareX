namespace ShareX.HistoryLib
{
    partial class ImageHistorySettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageHistorySettingsForm));
            lblThumbnailSize = new System.Windows.Forms.Label();
            lblMaximumImageLimit = new System.Windows.Forms.Label();
            nudThumbnailSizeWidth = new System.Windows.Forms.NumericUpDown();
            nudMaximumImageLimit = new System.Windows.Forms.NumericUpDown();
            lblThumbnailSizeWidthUnit = new System.Windows.Forms.Label();
            cbRememberSearchText = new System.Windows.Forms.CheckBox();
            cbFilterMissingFiles = new System.Windows.Forms.CheckBox();
            cbRememberWindowState = new System.Windows.Forms.CheckBox();
            cbImageOnly = new System.Windows.Forms.CheckBox();
            cbAutoLoadMoreItems = new System.Windows.Forms.CheckBox();
            lblThumbnailSizeHeightUnit = new System.Windows.Forms.Label();
            nudThumbnailSizeHeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSizeWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaximumImageLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSizeHeight).BeginInit();
            SuspendLayout();
            // 
            // lblThumbnailSize
            // 
            resources.ApplyResources(lblThumbnailSize, "lblThumbnailSize");
            lblThumbnailSize.Name = "lblThumbnailSize";
            // 
            // lblMaximumImageLimit
            // 
            resources.ApplyResources(lblMaximumImageLimit, "lblMaximumImageLimit");
            lblMaximumImageLimit.Name = "lblMaximumImageLimit";
            // 
            // nudThumbnailSizeWidth
            // 
            resources.ApplyResources(nudThumbnailSizeWidth, "nudThumbnailSizeWidth");
            nudThumbnailSizeWidth.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudThumbnailSizeWidth.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudThumbnailSizeWidth.Name = "nudThumbnailSizeWidth";
            nudThumbnailSizeWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudThumbnailSizeWidth.ValueChanged += nudThumbnailSizeWidth_ValueChanged;
            // 
            // nudMaximumImageLimit
            // 
            resources.ApplyResources(nudMaximumImageLimit, "nudMaximumImageLimit");
            nudMaximumImageLimit.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudMaximumImageLimit.Name = "nudMaximumImageLimit";
            nudMaximumImageLimit.ValueChanged += nudMaximumImageLimit_ValueChanged;
            // 
            // lblThumbnailSizeWidthUnit
            // 
            resources.ApplyResources(lblThumbnailSizeWidthUnit, "lblThumbnailSizeWidthUnit");
            lblThumbnailSizeWidthUnit.Name = "lblThumbnailSizeWidthUnit";
            // 
            // cbRememberSearchText
            // 
            resources.ApplyResources(cbRememberSearchText, "cbRememberSearchText");
            cbRememberSearchText.Name = "cbRememberSearchText";
            cbRememberSearchText.UseVisualStyleBackColor = true;
            cbRememberSearchText.CheckedChanged += cbRememberSearchText_CheckedChanged;
            // 
            // cbFilterMissingFiles
            // 
            resources.ApplyResources(cbFilterMissingFiles, "cbFilterMissingFiles");
            cbFilterMissingFiles.Name = "cbFilterMissingFiles";
            cbFilterMissingFiles.UseVisualStyleBackColor = true;
            cbFilterMissingFiles.CheckedChanged += cbFilterMissingFiles_CheckedChanged;
            // 
            // cbRememberWindowState
            // 
            resources.ApplyResources(cbRememberWindowState, "cbRememberWindowState");
            cbRememberWindowState.Name = "cbRememberWindowState";
            cbRememberWindowState.UseVisualStyleBackColor = true;
            cbRememberWindowState.CheckedChanged += cbRememberWindowState_CheckedChanged;
            // 
            // cbImageOnly
            // 
            resources.ApplyResources(cbImageOnly, "cbImageOnly");
            cbImageOnly.Name = "cbImageOnly";
            cbImageOnly.UseVisualStyleBackColor = true;
            cbImageOnly.CheckedChanged += cbImageOnly_CheckedChanged;
            // 
            // cbAutoLoadMoreItems
            // 
            resources.ApplyResources(cbAutoLoadMoreItems, "cbAutoLoadMoreItems");
            cbAutoLoadMoreItems.Name = "cbAutoLoadMoreItems";
            cbAutoLoadMoreItems.UseVisualStyleBackColor = true;
            cbAutoLoadMoreItems.CheckedChanged += cbAutoLoadMoreItems_CheckedChanged;
            // 
            // lblThumbnailSizeHeightUnit
            // 
            resources.ApplyResources(lblThumbnailSizeHeightUnit, "lblThumbnailSizeHeightUnit");
            lblThumbnailSizeHeightUnit.Name = "lblThumbnailSizeHeightUnit";
            // 
            // nudThumbnailSizeHeight
            // 
            resources.ApplyResources(nudThumbnailSizeHeight, "nudThumbnailSizeHeight");
            nudThumbnailSizeHeight.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudThumbnailSizeHeight.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudThumbnailSizeHeight.Name = "nudThumbnailSizeHeight";
            nudThumbnailSizeHeight.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudThumbnailSizeHeight.ValueChanged += nudThumbnailSizeHeight_ValueChanged;
            // 
            // ImageHistorySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(lblThumbnailSizeHeightUnit);
            Controls.Add(nudThumbnailSizeHeight);
            Controls.Add(cbAutoLoadMoreItems);
            Controls.Add(cbImageOnly);
            Controls.Add(cbRememberWindowState);
            Controls.Add(cbFilterMissingFiles);
            Controls.Add(cbRememberSearchText);
            Controls.Add(lblThumbnailSizeWidthUnit);
            Controls.Add(nudMaximumImageLimit);
            Controls.Add(nudThumbnailSizeWidth);
            Controls.Add(lblMaximumImageLimit);
            Controls.Add(lblThumbnailSize);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ImageHistorySettingsForm";
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSizeWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaximumImageLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSizeHeight).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblThumbnailSize;
        private System.Windows.Forms.Label lblMaximumImageLimit;
        private System.Windows.Forms.NumericUpDown nudThumbnailSizeWidth;
        private System.Windows.Forms.NumericUpDown nudMaximumImageLimit;
        private System.Windows.Forms.Label lblThumbnailSizeWidthUnit;
        private System.Windows.Forms.CheckBox cbRememberSearchText;
        private System.Windows.Forms.CheckBox cbFilterMissingFiles;
        private System.Windows.Forms.CheckBox cbRememberWindowState;
        private System.Windows.Forms.CheckBox cbImageOnly;
        private System.Windows.Forms.CheckBox cbAutoLoadMoreItems;
        private System.Windows.Forms.Label lblThumbnailSizeHeightUnit;
        private System.Windows.Forms.NumericUpDown nudThumbnailSizeHeight;
    }
}