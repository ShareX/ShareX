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
            this.lblViewMode = new System.Windows.Forms.Label();
            this.lblThumbnailSize = new System.Windows.Forms.Label();
            this.lblMaximumImageLimit = new System.Windows.Forms.Label();
            this.cbViewMode = new System.Windows.Forms.ComboBox();
            this.nudThumbnailSize = new System.Windows.Forms.NumericUpDown();
            this.nudMaximumImageLimit = new System.Windows.Forms.NumericUpDown();
            this.lblThumbnailSizeUnit = new System.Windows.Forms.Label();
            this.cbRememberSearchText = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumImageLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // lblViewMode
            // 
            resources.ApplyResources(this.lblViewMode, "lblViewMode");
            this.lblViewMode.Name = "lblViewMode";
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
            // cbViewMode
            // 
            this.cbViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbViewMode.FormattingEnabled = true;
            resources.ApplyResources(this.cbViewMode, "cbViewMode");
            this.cbViewMode.Name = "cbViewMode";
            this.cbViewMode.SelectedIndexChanged += new System.EventHandler(this.cbViewMode_SelectedIndexChanged);
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
            50,
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
            // ImageHistorySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbRememberSearchText);
            this.Controls.Add(this.lblThumbnailSizeUnit);
            this.Controls.Add(this.nudMaximumImageLimit);
            this.Controls.Add(this.nudThumbnailSize);
            this.Controls.Add(this.cbViewMode);
            this.Controls.Add(this.lblMaximumImageLimit);
            this.Controls.Add(this.lblThumbnailSize);
            this.Controls.Add(this.lblViewMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ImageHistorySettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudThumbnailSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumImageLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblViewMode;
        private System.Windows.Forms.Label lblThumbnailSize;
        private System.Windows.Forms.Label lblMaximumImageLimit;
        private System.Windows.Forms.ComboBox cbViewMode;
        private System.Windows.Forms.NumericUpDown nudThumbnailSize;
        private System.Windows.Forms.NumericUpDown nudMaximumImageLimit;
        private System.Windows.Forms.Label lblThumbnailSizeUnit;
        private System.Windows.Forms.CheckBox cbRememberSearchText;
    }
}