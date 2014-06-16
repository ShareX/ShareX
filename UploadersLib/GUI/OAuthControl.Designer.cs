namespace UploadersLib.GUI
{
    partial class OAuthControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbUserAccount = new System.Windows.Forms.GroupBox();
            this.btnRefreshAuthorization = new System.Windows.Forms.Button();
            this.btnOpenAuthorizePage = new System.Windows.Forms.Button();
            this.lblVerificationCode = new System.Windows.Forms.Label();
            this.btnCompleteAuthorization = new System.Windows.Forms.Button();
            this.txtVerificationCode = new System.Windows.Forms.TextBox();
            this.lblLoginStatus = new System.Windows.Forms.Label();
            this.btnClearAuthorization = new System.Windows.Forms.Button();
            this.gbUserAccount.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbUserAccount
            // 
            this.gbUserAccount.Controls.Add(this.btnClearAuthorization);
            this.gbUserAccount.Controls.Add(this.btnRefreshAuthorization);
            this.gbUserAccount.Controls.Add(this.btnOpenAuthorizePage);
            this.gbUserAccount.Controls.Add(this.lblVerificationCode);
            this.gbUserAccount.Controls.Add(this.btnCompleteAuthorization);
            this.gbUserAccount.Controls.Add(this.txtVerificationCode);
            this.gbUserAccount.Controls.Add(this.lblLoginStatus);
            this.gbUserAccount.Location = new System.Drawing.Point(0, 0);
            this.gbUserAccount.Name = "gbUserAccount";
            this.gbUserAccount.Size = new System.Drawing.Size(320, 230);
            this.gbUserAccount.TabIndex = 5;
            this.gbUserAccount.TabStop = false;
            this.gbUserAccount.Text = "User account";
            // 
            // btnRefreshAuthorization
            // 
            this.btnRefreshAuthorization.Enabled = false;
            this.btnRefreshAuthorization.Location = new System.Drawing.Point(16, 192);
            this.btnRefreshAuthorization.Name = "btnRefreshAuthorization";
            this.btnRefreshAuthorization.Size = new System.Drawing.Size(288, 23);
            this.btnRefreshAuthorization.TabIndex = 5;
            this.btnRefreshAuthorization.Text = "Refresh authorization";
            this.btnRefreshAuthorization.UseVisualStyleBackColor = true;
            this.btnRefreshAuthorization.Click += new System.EventHandler(this.btnRefreshAuthorization_Click);
            // 
            // btnOpenAuthorizePage
            // 
            this.btnOpenAuthorizePage.Location = new System.Drawing.Point(16, 24);
            this.btnOpenAuthorizePage.Name = "btnOpenAuthorizePage";
            this.btnOpenAuthorizePage.Size = new System.Drawing.Size(288, 23);
            this.btnOpenAuthorizePage.TabIndex = 0;
            this.btnOpenAuthorizePage.Text = "1. Open authorize page...";
            this.btnOpenAuthorizePage.UseVisualStyleBackColor = true;
            this.btnOpenAuthorizePage.Click += new System.EventHandler(this.btnOpenAuthorizePage_Click);
            // 
            // lblVerificationCode
            // 
            this.lblVerificationCode.AutoSize = true;
            this.lblVerificationCode.Location = new System.Drawing.Point(16, 56);
            this.lblVerificationCode.Name = "lblVerificationCode";
            this.lblVerificationCode.Size = new System.Drawing.Size(292, 13);
            this.lblVerificationCode.TabIndex = 1;
            this.lblVerificationCode.Text = "Verification code (Get verification code from authorize page):";
            // 
            // btnCompleteAuthorization
            // 
            this.btnCompleteAuthorization.Enabled = false;
            this.btnCompleteAuthorization.Location = new System.Drawing.Point(16, 104);
            this.btnCompleteAuthorization.Name = "btnCompleteAuthorization";
            this.btnCompleteAuthorization.Size = new System.Drawing.Size(288, 23);
            this.btnCompleteAuthorization.TabIndex = 3;
            this.btnCompleteAuthorization.Text = "2. Complete authorization";
            this.btnCompleteAuthorization.UseVisualStyleBackColor = true;
            this.btnCompleteAuthorization.Click += new System.EventHandler(this.btnCompleteAuthorization_Click);
            // 
            // txtVerificationCode
            // 
            this.txtVerificationCode.Location = new System.Drawing.Point(16, 80);
            this.txtVerificationCode.Name = "txtVerificationCode";
            this.txtVerificationCode.Size = new System.Drawing.Size(288, 20);
            this.txtVerificationCode.TabIndex = 2;
            this.txtVerificationCode.TextChanged += new System.EventHandler(this.txtVerificationCode_TextChanged);
            // 
            // lblLoginStatus
            // 
            this.lblLoginStatus.AutoSize = true;
            this.lblLoginStatus.Location = new System.Drawing.Point(16, 136);
            this.lblLoginStatus.Name = "lblLoginStatus";
            this.lblLoginStatus.Size = new System.Drawing.Size(37, 13);
            this.lblLoginStatus.TabIndex = 4;
            this.lblLoginStatus.Text = "Status";
            // 
            // btnClearAuthorization
            // 
            this.btnClearAuthorization.Enabled = false;
            this.btnClearAuthorization.Location = new System.Drawing.Point(16, 160);
            this.btnClearAuthorization.Name = "btnClearAuthorization";
            this.btnClearAuthorization.Size = new System.Drawing.Size(288, 23);
            this.btnClearAuthorization.TabIndex = 6;
            this.btnClearAuthorization.Text = "Clear authorization";
            this.btnClearAuthorization.UseVisualStyleBackColor = true;
            this.btnClearAuthorization.Click += new System.EventHandler(this.btnClearAuthorization_Click);
            // 
            // OAuthControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbUserAccount);
            this.Name = "OAuthControl";
            this.Size = new System.Drawing.Size(326, 238);
            this.gbUserAccount.ResumeLayout(false);
            this.gbUserAccount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbUserAccount;
        private System.Windows.Forms.Button btnRefreshAuthorization;
        private System.Windows.Forms.Button btnOpenAuthorizePage;
        private System.Windows.Forms.Label lblVerificationCode;
        private System.Windows.Forms.Button btnCompleteAuthorization;
        private System.Windows.Forms.TextBox txtVerificationCode;
        private System.Windows.Forms.Label lblLoginStatus;
        private System.Windows.Forms.Button btnClearAuthorization;
    }
}
