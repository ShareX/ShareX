namespace ShareX.HelpersLib
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this.txtException = new System.Windows.Forms.TextBox();
            this.btnSendBugReport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpenLogFile = new System.Windows.Forms.Button();
            this.flpMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.flpMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtException
            // 
            resources.ApplyResources(this.txtException, "txtException");
            this.txtException.Name = "txtException";
            this.txtException.ReadOnly = true;
            // 
            // btnSendBugReport
            // 
            this.btnSendBugReport.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnSendBugReport, "btnSendBugReport");
            this.btnSendBugReport.Name = "btnSendBugReport";
            this.btnSendBugReport.UseVisualStyleBackColor = false;
            this.btnSendBugReport.Click += new System.EventHandler(this.btnSendBugReport_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpenLogFile
            // 
            this.btnOpenLogFile.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnOpenLogFile, "btnOpenLogFile");
            this.btnOpenLogFile.Name = "btnOpenLogFile";
            this.btnOpenLogFile.UseVisualStyleBackColor = false;
            this.btnOpenLogFile.Click += new System.EventHandler(this.btnOpenLogFile_Click);
            // 
            // flpMenu
            // 
            resources.ApplyResources(this.flpMenu, "flpMenu");
            this.flpMenu.Controls.Add(this.btnSendBugReport);
            this.flpMenu.Controls.Add(this.btnOpenLogFile);
            this.flpMenu.Controls.Add(this.btnContinue);
            this.flpMenu.Controls.Add(this.btnClose);
            this.flpMenu.Controls.Add(this.btnOK);
            this.flpMenu.Name = "flpMenu";
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnContinue, "btnContinue");
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.UseVisualStyleBackColor = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblErrorMessage
            // 
            resources.ApplyResources(this.lblErrorMessage, "lblErrorMessage");
            this.lblErrorMessage.Name = "lblErrorMessage";
            // 
            // ErrorForm
            // 
            this.AcceptButton = this.btnContinue;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.flpMenu);
            this.Controls.Add(this.txtException);
            this.Name = "ErrorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ErrorForm_Shown);
            this.flpMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.TextBox txtException;
        private System.Windows.Forms.Button btnSendBugReport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpenLogFile;
        private System.Windows.Forms.FlowLayoutPanel flpMenu;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Label lblErrorMessage;
        private System.Windows.Forms.Button btnOK;
    }
}