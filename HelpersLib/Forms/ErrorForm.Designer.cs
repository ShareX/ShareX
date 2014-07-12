namespace HelpersLib
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
            this.txtException = new System.Windows.Forms.TextBox();
            this.btnCopyAll = new System.Windows.Forms.Button();
            this.btnSendBugReport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpenLogFile = new System.Windows.Forms.Button();
            this.flpMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.btnContinue = new System.Windows.Forms.Button();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.flpMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtException
            // 
            this.txtException.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtException.BackColor = System.Drawing.Color.White;
            this.txtException.Location = new System.Drawing.Point(8, 48);
            this.txtException.Multiline = true;
            this.txtException.Name = "txtException";
            this.txtException.ReadOnly = true;
            this.txtException.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtException.Size = new System.Drawing.Size(480, 222);
            this.txtException.TabIndex = 1;
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.BackColor = System.Drawing.Color.Transparent;
            this.btnCopyAll.Location = new System.Drawing.Point(0, 3);
            this.btnCopyAll.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(72, 24);
            this.btnCopyAll.TabIndex = 0;
            this.btnCopyAll.Text = "Copy all";
            this.btnCopyAll.UseVisualStyleBackColor = false;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // btnSendBugReport
            // 
            this.btnSendBugReport.BackColor = System.Drawing.Color.Transparent;
            this.btnSendBugReport.Location = new System.Drawing.Point(173, 3);
            this.btnSendBugReport.Name = "btnSendBugReport";
            this.btnSendBugReport.Size = new System.Drawing.Size(104, 24);
            this.btnSendBugReport.TabIndex = 2;
            this.btnSendBugReport.Text = "Send bug report";
            this.btnSendBugReport.UseVisualStyleBackColor = false;
            this.btnSendBugReport.Click += new System.EventHandler(this.btnSendBugReport_Click);
            // 
            // btnClose
            // 
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Location = new System.Drawing.Point(283, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(106, 24);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Exit application";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpenLogFile
            // 
            this.btnOpenLogFile.BackColor = System.Drawing.Color.Transparent;
            this.btnOpenLogFile.Location = new System.Drawing.Point(78, 3);
            this.btnOpenLogFile.Name = "btnOpenLogFile";
            this.btnOpenLogFile.Size = new System.Drawing.Size(89, 24);
            this.btnOpenLogFile.TabIndex = 1;
            this.btnOpenLogFile.Text = "Open log file";
            this.btnOpenLogFile.UseVisualStyleBackColor = false;
            this.btnOpenLogFile.Click += new System.EventHandler(this.btnOpenLogFile_Click);
            // 
            // flpMenu
            // 
            this.flpMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpMenu.Controls.Add(this.btnCopyAll);
            this.flpMenu.Controls.Add(this.btnOpenLogFile);
            this.flpMenu.Controls.Add(this.btnSendBugReport);
            this.flpMenu.Controls.Add(this.btnClose);
            this.flpMenu.Controls.Add(this.btnContinue);
            this.flpMenu.Location = new System.Drawing.Point(8, 278);
            this.flpMenu.Name = "flpMenu";
            this.flpMenu.Size = new System.Drawing.Size(480, 32);
            this.flpMenu.TabIndex = 2;
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.Transparent;
            this.btnContinue.Location = new System.Drawing.Point(395, 3);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(74, 24);
            this.btnContinue.TabIndex = 4;
            this.btnContinue.Text = "&OK";
            this.btnContinue.UseVisualStyleBackColor = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblErrorMessage.Location = new System.Drawing.Point(7, 8);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(480, 34);
            this.lblErrorMessage.TabIndex = 0;
            this.lblErrorMessage.Text = "Error\r\nError 2";
            // 
            // ErrorForm
            // 
            this.AcceptButton = this.btnContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 313);
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.flpMenu);
            this.Controls.Add(this.txtException);
            this.Name = "ErrorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Error";
            this.Shown += new System.EventHandler(this.ErrorForm_Shown);
            this.flpMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.TextBox txtException;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.Button btnSendBugReport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpenLogFile;
        private System.Windows.Forms.FlowLayoutPanel flpMenu;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Label lblErrorMessage;
    }
}