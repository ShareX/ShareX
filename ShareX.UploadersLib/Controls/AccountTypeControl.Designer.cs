namespace ShareX.UploadersLib
{
    partial class AccountTypeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountTypeControl));
            this.lblAccountType = new System.Windows.Forms.Label();
            this.cbAccountType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblAccountType
            // 
            resources.ApplyResources(this.lblAccountType, "lblAccountType");
            this.lblAccountType.Name = "lblAccountType";
            // 
            // cbAccountType
            // 
            this.cbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAccountType.FormattingEnabled = true;
            this.cbAccountType.Items.AddRange(new object[] {
            resources.GetString("cbAccountType.Items"),
            resources.GetString("cbAccountType.Items1")});
            resources.ApplyResources(this.cbAccountType, "cbAccountType");
            this.cbAccountType.Name = "cbAccountType";
            // 
            // AccountTypeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbAccountType);
            this.Controls.Add(this.lblAccountType);
            this.Name = "AccountTypeControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAccountType;
        private System.Windows.Forms.ComboBox cbAccountType;
    }
}
