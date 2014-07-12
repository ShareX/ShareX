namespace UploadersLib.GUI
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
            this.lblIssueId.AutoSize = true;
            this.lblIssueId.Location = new System.Drawing.Point(19, 9);
            this.lblIssueId.Name = "lblIssueId";
            this.lblIssueId.Size = new System.Drawing.Size(47, 13);
            this.lblIssueId.TabIndex = 0;
            this.lblIssueId.Text = "Issue Id:";
            this.lblIssueId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtIssueId
            // 
            this.txtIssueId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIssueId.Location = new System.Drawing.Point(72, 6);
            this.txtIssueId.Name = "txtIssueId";
            this.txtIssueId.Size = new System.Drawing.Size(153, 20);
            this.txtIssueId.TabIndex = 1;
            this.txtIssueId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtIssueId.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // lblSummary
            // 
            this.lblSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSummary.Location = new System.Drawing.Point(3, 16);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(207, 41);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "lblSummary";
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpload.Location = new System.Drawing.Point(147, 98);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(75, 23);
            this.btnUpload.TabIndex = 4;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(18, 98);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gpSummary
            // 
            this.gpSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpSummary.Controls.Add(this.lblSummary);
            this.gpSummary.Location = new System.Drawing.Point(12, 32);
            this.gpSummary.Name = "gpSummary";
            this.gpSummary.Size = new System.Drawing.Size(213, 60);
            this.gpSummary.TabIndex = 2;
            this.gpSummary.TabStop = false;
            this.gpSummary.Text = "Summary";
            // 
            // JiraUpload
            // 
            this.AcceptButton = this.btnUpload;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(237, 127);
            this.Controls.Add(this.txtIssueId);
            this.Controls.Add(this.lblIssueId);
            this.Controls.Add(this.gpSummary);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpload);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JiraUpload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jira file upload";
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