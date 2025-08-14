
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
            cbRememberSearchText = new System.Windows.Forms.CheckBox();
            cbRememberWindowState = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // cbRememberSearchText
            // 
            resources.ApplyResources(cbRememberSearchText, "cbRememberSearchText");
            cbRememberSearchText.Name = "cbRememberSearchText";
            cbRememberSearchText.UseVisualStyleBackColor = true;
            cbRememberSearchText.CheckedChanged += cbRememberSearchText_CheckedChanged;
            // 
            // cbRememberWindowState
            // 
            resources.ApplyResources(cbRememberWindowState, "cbRememberWindowState");
            cbRememberWindowState.Name = "cbRememberWindowState";
            cbRememberWindowState.UseVisualStyleBackColor = true;
            cbRememberWindowState.CheckedChanged += cbRememberWindowState_CheckedChanged;
            // 
            // HistorySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(cbRememberWindowState);
            Controls.Add(cbRememberSearchText);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "HistorySettingsForm";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox cbRememberSearchText;
        private System.Windows.Forms.CheckBox cbRememberWindowState;
    }
}