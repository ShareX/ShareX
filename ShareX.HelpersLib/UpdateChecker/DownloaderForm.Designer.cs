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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtChangelog = new System.Windows.Forms.TextBox();
            this.lblProgress = new ShareX.HelpersLib.BlackStyleLabel();
            this.cbShowChangelog = new ShareX.HelpersLib.BlackStyleCheckBox();
            this.lblStatus = new ShareX.HelpersLib.BlackStyleLabel();
            this.lblFilename = new ShareX.HelpersLib.BlackStyleLabel();
            this.btnAction = new ShareX.HelpersLib.BlackStyleButton();
            this.pbProgress = new ShareX.HelpersLib.BlackStyleProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::ShareX.HelpersLib.Properties.Resources.tick;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // txtChangelog
            // 
            this.txtChangelog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.txtChangelog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChangelog.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.txtChangelog, "txtChangelog");
            this.txtChangelog.Name = "txtChangelog";
            this.txtChangelog.ReadOnly = true;
            // 
            // lblProgress
            // 
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.ForeColor = System.Drawing.Color.White;
            this.lblProgress.Name = "lblProgress";
            // 
            // cbShowChangelog
            // 
            this.cbShowChangelog.BackColor = System.Drawing.Color.Transparent;
            this.cbShowChangelog.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.cbShowChangelog, "cbShowChangelog");
            this.cbShowChangelog.ForeColor = System.Drawing.Color.White;
            this.cbShowChangelog.Name = "cbShowChangelog";
            this.cbShowChangelog.SpaceAfterCheckBox = 3;
            this.cbShowChangelog.CheckedChanged += new System.EventHandler(this.cbShowChangelog_CheckedChanged);
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
            this.Controls.Add(this.txtChangelog);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.cbShowChangelog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pbProgress);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DownloaderForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdaterForm_FormClosing);
            this.Shown += new System.EventHandler(this.DownloaderForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private BlackStyleProgressBar pbProgress;
        private BlackStyleButton btnAction;
        private BlackStyleLabel lblFilename;
        private BlackStyleLabel lblStatus;
        private BlackStyleLabel lblProgress;
        private System.Windows.Forms.TextBox txtChangelog;
        private BlackStyleCheckBox cbShowChangelog;
    }
}