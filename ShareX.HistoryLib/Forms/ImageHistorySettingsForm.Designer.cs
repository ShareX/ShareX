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
            nudThumbnailSize = new System.Windows.Forms.NumericUpDown();
            nudMaximumImageLimit = new System.Windows.Forms.NumericUpDown();
            lblThumbnailSizeUnit = new System.Windows.Forms.Label();
            cbRememberSearchText = new System.Windows.Forms.CheckBox();
            cbFilterMissingFiles = new System.Windows.Forms.CheckBox();
            cbRememberWindowState = new System.Windows.Forms.CheckBox();
            cbImageOnly = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaximumImageLimit).BeginInit();
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
            // nudThumbnailSize
            // 
            resources.ApplyResources(nudThumbnailSize, "nudThumbnailSize");
            nudThumbnailSize.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudThumbnailSize.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudThumbnailSize.Name = "nudThumbnailSize";
            nudThumbnailSize.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudThumbnailSize.ValueChanged += nudThumbnailSize_ValueChanged;
            // 
            // nudMaximumImageLimit
            // 
            resources.ApplyResources(nudMaximumImageLimit, "nudMaximumImageLimit");
            nudMaximumImageLimit.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudMaximumImageLimit.Name = "nudMaximumImageLimit";
            nudMaximumImageLimit.ValueChanged += nudMaximumImageLimit_ValueChanged;
            // 
            // lblThumbnailSizeUnit
            // 
            resources.ApplyResources(lblThumbnailSizeUnit, "lblThumbnailSizeUnit");
            lblThumbnailSizeUnit.Name = "lblThumbnailSizeUnit";
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
            // ImageHistorySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(cbImageOnly);
            Controls.Add(cbRememberWindowState);
            Controls.Add(cbFilterMissingFiles);
            Controls.Add(cbRememberSearchText);
            Controls.Add(lblThumbnailSizeUnit);
            Controls.Add(nudMaximumImageLimit);
            Controls.Add(nudThumbnailSize);
            Controls.Add(lblMaximumImageLimit);
            Controls.Add(lblThumbnailSize);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ImageHistorySettingsForm";
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaximumImageLimit).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblThumbnailSize;
        private System.Windows.Forms.Label lblMaximumImageLimit;
        private System.Windows.Forms.NumericUpDown nudThumbnailSize;
        private System.Windows.Forms.NumericUpDown nudMaximumImageLimit;
        private System.Windows.Forms.Label lblThumbnailSizeUnit;
        private System.Windows.Forms.CheckBox cbRememberSearchText;
        private System.Windows.Forms.CheckBox cbFilterMissingFiles;
        private System.Windows.Forms.CheckBox cbRememberWindowState;
        private System.Windows.Forms.CheckBox cbImageOnly;
    }
}