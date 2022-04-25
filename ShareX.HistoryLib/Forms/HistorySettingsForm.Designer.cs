
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
            this.cbRememberSearchText = new System.Windows.Forms.CheckBox();
            this.cbRememberWindowState = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HistorySettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox cbRememberSearchText;
        private System.Windows.Forms.CheckBox cbRememberWindowState;
    }
}