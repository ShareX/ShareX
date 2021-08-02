
namespace ShareX.HistoryLib
{
    partial class HistorySettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistorySettingsForm));
            this.lblMaximumItemLimit = new System.Windows.Forms.Label();
            this.nudMaximumItemLimit = new System.Windows.Forms.NumericUpDown();
            this.cbRememberSearchText = new System.Windows.Forms.CheckBox();
            this.cbRememberWindowState = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumItemLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMaximumItemLimit
            // 
            resources.ApplyResources(this.lblMaximumItemLimit, "lblMaximumItemLimit");
            this.lblMaximumItemLimit.Name = "lblMaximumItemLimit";
            // 
            // nudMaximumItemLimit
            // 
            resources.ApplyResources(this.nudMaximumItemLimit, "nudMaximumItemLimit");
            this.nudMaximumItemLimit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaximumItemLimit.Name = "nudMaximumItemLimit";
            this.nudMaximumItemLimit.ValueChanged += new System.EventHandler(this.nudMaximumItemLimit_ValueChanged);
            // 
            // cbRememberSearchText
            // 
            resources.ApplyResources(this.cbRememberSearchText, "cbRememberSearchText");
            this.cbRememberSearchText.Name = "cbRememberSearchText";
            this.cbRememberSearchText.UseVisualStyleBackColor = true;
            this.cbRememberSearchText.CheckedChanged += new System.EventHandler(this.cbRememberSearchText_CheckedChanged);
            // 
            // cbRememberWindowState
            // 
            resources.ApplyResources(this.cbRememberWindowState, "cbRememberWindowState");
            this.cbRememberWindowState.Name = "cbRememberWindowState";
            this.cbRememberWindowState.UseVisualStyleBackColor = true;
            this.cbRememberWindowState.CheckedChanged += new System.EventHandler(this.cbRememberWindowState_CheckedChanged);
            // 
            // HistorySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbRememberWindowState);
            this.Controls.Add(this.cbRememberSearchText);
            this.Controls.Add(this.nudMaximumItemLimit);
            this.Controls.Add(this.lblMaximumItemLimit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HistorySettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumItemLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMaximumItemLimit;
        private System.Windows.Forms.NumericUpDown nudMaximumItemLimit;
        private System.Windows.Forms.CheckBox cbRememberSearchText;
        private System.Windows.Forms.CheckBox cbRememberWindowState;
    }
}