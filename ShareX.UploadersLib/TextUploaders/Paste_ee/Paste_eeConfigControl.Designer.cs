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
            this.lblPaste_eeUserAPIKey = new System.Windows.Forms.Label();
            this.txtPaste_eeUserAPIKey = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblPaste_eeUserAPIKey
            // 
            this.lblPaste_eeUserAPIKey.AutoSize = true;
            this.lblPaste_eeUserAPIKey.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPaste_eeUserAPIKey.Location = new System.Drawing.Point(17, 16);
            this.lblPaste_eeUserAPIKey.Name = "lblPaste_eeUserAPIKey";
            this.lblPaste_eeUserAPIKey.Size = new System.Drawing.Size(72, 13);
            this.lblPaste_eeUserAPIKey.TabIndex = 2;
            this.lblPaste_eeUserAPIKey.Text = "User API key:";
            // 
            // txtPaste_eeUserAPIKey
            // 
            this.txtPaste_eeUserAPIKey.Location = new System.Drawing.Point(20, 32);
            this.txtPaste_eeUserAPIKey.Name = "txtPaste_eeUserAPIKey";
            this.txtPaste_eeUserAPIKey.Size = new System.Drawing.Size(296, 20);
            this.txtPaste_eeUserAPIKey.TabIndex = 3;
            this.txtPaste_eeUserAPIKey.UseSystemPasswordChar = true;
            this.txtPaste_eeUserAPIKey.TextChanged += new System.EventHandler(this.txtPaste_eeUserAPIKey_TextChanged);
            // 
            // Paste_eeConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
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
