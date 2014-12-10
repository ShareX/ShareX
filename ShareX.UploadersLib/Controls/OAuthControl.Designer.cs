namespace UploadersLib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OAuthControl));
            this.gbUserAccount = new System.Windows.Forms.GroupBox();
            this.btnClearAuthorization = new System.Windows.Forms.Button();
            this.btnRefreshAuthorization = new System.Windows.Forms.Button();
            this.btnOpenAuthorizePage = new System.Windows.Forms.Button();
            this.lblVerificationCode = new System.Windows.Forms.Label();
            this.btnCompleteAuthorization = new System.Windows.Forms.Button();
            this.txtVerificationCode = new System.Windows.Forms.TextBox();
            this.lblLoginStatus = new System.Windows.Forms.Label();
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
            resources.ApplyResources(this.gbUserAccount, "gbUserAccount");
            this.gbUserAccount.Name = "gbUserAccount";
            this.gbUserAccount.TabStop = false;
            // 
            // btnClearAuthorization
            // 
            resources.ApplyResources(this.btnClearAuthorization, "btnClearAuthorization");
            this.btnClearAuthorization.Name = "btnClearAuthorization";
            this.btnClearAuthorization.UseVisualStyleBackColor = true;
            this.btnClearAuthorization.Click += new System.EventHandler(this.btnClearAuthorization_Click);
            // 
            // btnRefreshAuthorization
            // 
            resources.ApplyResources(this.btnRefreshAuthorization, "btnRefreshAuthorization");
            this.btnRefreshAuthorization.Name = "btnRefreshAuthorization";
            this.btnRefreshAuthorization.UseVisualStyleBackColor = true;
            this.btnRefreshAuthorization.Click += new System.EventHandler(this.btnRefreshAuthorization_Click);
            // 
            // btnOpenAuthorizePage
            // 
            resources.ApplyResources(this.btnOpenAuthorizePage, "btnOpenAuthorizePage");
            this.btnOpenAuthorizePage.Name = "btnOpenAuthorizePage";
            this.btnOpenAuthorizePage.UseVisualStyleBackColor = true;
            this.btnOpenAuthorizePage.Click += new System.EventHandler(this.btnOpenAuthorizePage_Click);
            // 
            // lblVerificationCode
            // 
            resources.ApplyResources(this.lblVerificationCode, "lblVerificationCode");
            this.lblVerificationCode.Name = "lblVerificationCode";
            // 
            // btnCompleteAuthorization
            // 
            resources.ApplyResources(this.btnCompleteAuthorization, "btnCompleteAuthorization");
            this.btnCompleteAuthorization.Name = "btnCompleteAuthorization";
            this.btnCompleteAuthorization.UseVisualStyleBackColor = true;
            this.btnCompleteAuthorization.Click += new System.EventHandler(this.btnCompleteAuthorization_Click);
            // 
            // txtVerificationCode
            // 
            resources.ApplyResources(this.txtVerificationCode, "txtVerificationCode");
            this.txtVerificationCode.Name = "txtVerificationCode";
            this.txtVerificationCode.TextChanged += new System.EventHandler(this.txtVerificationCode_TextChanged);
            // 
            // lblLoginStatus
            // 
            resources.ApplyResources(this.lblLoginStatus, "lblLoginStatus");
            this.lblLoginStatus.Name = "lblLoginStatus";
            // 
            // OAuthControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbUserAccount);
            this.Name = "OAuthControl";
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
