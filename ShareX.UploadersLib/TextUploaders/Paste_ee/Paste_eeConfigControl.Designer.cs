namespace ShareX.UploadersLib.TextUploaders.Paste_ee
{
    partial class Paste_eeConfigControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Paste_eeConfigControl));
            this.lblPaste_eeUserAPIKey = new System.Windows.Forms.Label();
            this.txtPaste_eeUserAPIKey = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblPaste_eeUserAPIKey
            // 
            resources.ApplyResources(this.lblPaste_eeUserAPIKey, "lblPaste_eeUserAPIKey");
            this.lblPaste_eeUserAPIKey.Name = "lblPaste_eeUserAPIKey";
            // 
            // txtPaste_eeUserAPIKey
            // 
            resources.ApplyResources(this.txtPaste_eeUserAPIKey, "txtPaste_eeUserAPIKey");
            this.txtPaste_eeUserAPIKey.Name = "txtPaste_eeUserAPIKey";
            this.txtPaste_eeUserAPIKey.UseSystemPasswordChar = true;
            this.txtPaste_eeUserAPIKey.TextChanged += new System.EventHandler(this.txtPaste_eeUserAPIKey_TextChanged);
            // 
            // Paste_eeConfigControl
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lblPaste_eeUserAPIKey);
            this.Controls.Add(this.txtPaste_eeUserAPIKey);
            this.Name = "Paste_eeConfigControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPaste_eeUserAPIKey;
        private System.Windows.Forms.TextBox txtPaste_eeUserAPIKey;
    }
}
