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
            this.lblThumbnailSize = new System.Windows.Forms.Label();
            this.lblMaximumImageLimit = new System.Windows.Forms.Label();
            this.nudThumbnailSize = new System.Windows.Forms.NumericUpDown();
            this.nudMaximumImageLimit = new System.Windows.Forms.NumericUpDown();
            this.lblThumbnailSizeUnit = new System.Windows.Forms.Label();
            this.cbRememberSearchText = new System.Windows.Forms.CheckBox();
            this.cbFilterMissingFiles = new System.Windows.Forms.CheckBox();
            this.cbRememberWindowState = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumImageLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // lblThumbnailSize
            // 
            resources.ApplyResources(this.lblThumbnailSize, "lblThumbnailSize");
            this.lblThumbnailSize.Name = "lblThumbnailSize";
            // 
            // lblMaximumImageLimit
            // 
            resources.ApplyResources(this.lblMaximumImageLimit, "lblMaximumImageLimit");
            this.lblMaximumImageLimit.Name = "lblMaximumImageLimit";
            // 
            // nudThumbnailSize
            // 
            resources.ApplyResources(this.nudThumbnailSize, "nudThumbnailSize");
            this.nudThumbnailSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudThumbnailSize.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudThumbnailSize.Name = "nudThumbnailSize";
            this.nudThumbnailSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudThumbnailSize.ValueChanged += new System.EventHandler(this.nudThumbnailSize_ValueChanged);
            // 
            // nudMaximumImageLimit
            // 
            resources.ApplyResources(this.nudMaximumImageLimit, "nudMaximumImageLimit");
            this.nudMaximumImageLimit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaximumImageLimit.Name = "nudMaximumImageLimit";
            this.nudMaximumImageLimit.ValueChanged += new System.EventHandler(this.nudMaximumImageLimit_ValueChanged);
            // 
            // lblThumbnailSizeUnit
            // 
            resources.ApplyResources(this.lblThumbnailSizeUnit, "lblThumbnailSizeUnit");
            this.lblThumbnailSizeUnit.Name = "lblThumbnailSizeUnit";
            // 
            // cbRememberSearchText
            // 
            resources.ApplyResources(this.cbRememberSearchText, "cbRememberSearchText");
            this.cbRememberSearchText.Name = "cbRememberSearchText";
            this.cbRememberSearchText.UseVisualStyleBackColor = true;
            this.cbRememberSearchText.CheckedChanged += new System.EventHandler(this.cbRememberSearchText_CheckedChanged);
            // 
            // cbFilterMissingFiles
            // 
            resources.ApplyResources(this.cbFilterMissingFiles, "cbFilterMissingFiles");
            this.cbFilterMissingFiles.Name = "cbFilterMissingFiles";
            this.cbFilterMissingFiles.UseVisualStyleBackColor = true;
            this.cbFilterMissingFiles.CheckedChanged += new System.EventHandler(this.cbFilterMissingFiles_CheckedChanged);
            // 
            // cbRememberWindowState
            // 
            resources.ApplyResources(this.cbRememberWindowState, "cbRememberWindowState");
            this.cbRememberWindowState.Name = "cbRememberWindowState";
            this.cbRememberWindowState.UseVisualStyleBackColor = true;
            this.cbRememberWindowState.CheckedChanged += new System.EventHandler(this.cbRememberWindowState_CheckedChanged);
            // 
            // ImageHistorySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbRememberWindowState);
            this.Controls.Add(this.cbFilterMissingFiles);
            this.Controls.Add(this.cbRememberSearchText);
            this.Controls.Add(this.lblThumbnailSizeUnit);
            this.Controls.Add(this.nudMaximumImageLimit);
            this.Controls.Add(this.nudThumbnailSize);
            this.Controls.Add(this.lblMaximumImageLimit);
            this.Controls.Add(this.lblThumbnailSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ImageHistorySettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumImageLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}