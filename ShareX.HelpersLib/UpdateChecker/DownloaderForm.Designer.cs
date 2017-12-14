namespace ShareX.HelpersLib
{
    partial class DownloaderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloaderForm));
            this.lblProgress = new ShareX.HelpersLib.BlackStyleLabel();
            this.lblStatus = new ShareX.HelpersLib.BlackStyleLabel();
            this.lblFilename = new ShareX.HelpersLib.BlackStyleLabel();
            this.btnAction = new ShareX.HelpersLib.BlackStyleButton();
            this.pbProgress = new ShareX.HelpersLib.BlackStyleProgressBar();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.ForeColor = System.Drawing.Color.White;
            this.lblProgress.Name = "lblProgress";
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Name = "lblStatus";
            // 
            // lblFilename
            // 
            this.lblFilename.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblFilename, "lblFilename");
            this.lblFilename.ForeColor = System.Drawing.Color.White;
            this.lblFilename.Name = "lblFilename";
            // 
            // btnAction
            // 
            this.btnAction.BackColor = System.Drawing.Color.Transparent;
            this.btnAction.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnAction, "btnAction");
            this.btnAction.ForeColor = System.Drawing.Color.White;
            this.btnAction.Name = "btnAction";
            this.btnAction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnAction_MouseClick);
            // 
            // pbProgress
            // 
            this.pbProgress.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.Maximum = 100;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Value = 0;
            // 
            // DownloaderForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.pbProgress);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DownloaderForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdaterForm_FormClosing);
            this.Shown += new System.EventHandler(this.DownloaderForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private BlackStyleProgressBar pbProgress;
        private BlackStyleButton btnAction;
        private BlackStyleLabel lblFilename;
        private BlackStyleLabel lblStatus;
        private BlackStyleLabel lblProgress;
    }
}