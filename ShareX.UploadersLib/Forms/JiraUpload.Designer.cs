namespace ShareX.UploadersLib
{
    partial class JiraUpload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JiraUpload));
            this.lblIssueId = new System.Windows.Forms.Label();
            this.txtIssueId = new System.Windows.Forms.TextBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gpSummary = new System.Windows.Forms.GroupBox();
            this.gpSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblIssueId
            // 
            resources.ApplyResources(this.lblIssueId, "lblIssueId");
            this.lblIssueId.Name = "lblIssueId";
            // 
            // txtIssueId
            // 
            resources.ApplyResources(this.txtIssueId, "txtIssueId");
            this.txtIssueId.Name = "txtIssueId";
            this.txtIssueId.TextChanged += new System.EventHandler(this.txtIssueId_TextChanged);
            // 
            // lblSummary
            // 
            resources.ApplyResources(this.lblSummary, "lblSummary");
            this.lblSummary.Name = "lblSummary";
            // 
            // btnUpload
            // 
            resources.ApplyResources(this.btnUpload, "btnUpload");
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gpSummary
            // 
            resources.ApplyResources(this.gpSummary, "gpSummary");
            this.gpSummary.Controls.Add(this.lblSummary);
            this.gpSummary.Name = "gpSummary";
            this.gpSummary.TabStop = false;
            // 
            // JiraUpload
            // 
            this.AcceptButton = this.btnUpload;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.txtIssueId);
            this.Controls.Add(this.lblIssueId);
            this.Controls.Add(this.gpSummary);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpload);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JiraUpload";
            this.Load += new System.EventHandler(this.JiraUpload_Load);
            this.gpSummary.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIssueId;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TextBox txtIssueId;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gpSummary;
    }
}