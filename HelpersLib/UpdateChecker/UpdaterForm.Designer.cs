namespace HelpersLib
{
    partial class UpdaterForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtChangelog = new System.Windows.Forms.TextBox();
            this.lblProgress = new HelpersLib.MyLabel();
            this.cbShowChangelog = new HelpersLib.MyCheckBox();
            this.lblStatus = new HelpersLib.MyLabel();
            this.lblFilename = new HelpersLib.MyLabel();
            this.btnAction = new HelpersLib.MyButton();
            this.pbProgress = new HelpersLib.MyProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::HelpersLib.Properties.Resources.tick;
            this.pictureBox1.Location = new System.Drawing.Point(352, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // txtChangelog
            // 
            this.txtChangelog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.txtChangelog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChangelog.ForeColor = System.Drawing.Color.White;
            this.txtChangelog.Location = new System.Drawing.Point(8, 208);
            this.txtChangelog.Multiline = true;
            this.txtChangelog.Name = "txtChangelog";
            this.txtChangelog.ReadOnly = true;
            this.txtChangelog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtChangelog.Size = new System.Drawing.Size(472, 201);
            this.txtChangelog.TabIndex = 14;
            // 
            // lblProgress
            // 
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            this.lblProgress.Font = new System.Drawing.Font("Arial", 12F);
            this.lblProgress.ForeColor = System.Drawing.Color.White;
            this.lblProgress.Location = new System.Drawing.Point(8, 72);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(328, 78);
            this.lblProgress.TabIndex = 12;
            this.lblProgress.Text = "Progress:";
            // 
            // cbShowChangelog
            // 
            this.cbShowChangelog.BackColor = System.Drawing.Color.Transparent;
            this.cbShowChangelog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbShowChangelog.Font = new System.Drawing.Font("Arial", 8F);
            this.cbShowChangelog.ForeColor = System.Drawing.Color.White;
            this.cbShowChangelog.Location = new System.Drawing.Point(352, 142);
            this.cbShowChangelog.Name = "cbShowChangelog";
            this.cbShowChangelog.Size = new System.Drawing.Size(128, 16);
            this.cbShowChangelog.SpaceAfterCheckBox = 3;
            this.cbShowChangelog.TabIndex = 15;
            this.cbShowChangelog.Text = "Show update notes";
            this.cbShowChangelog.Visible = false;
            this.cbShowChangelog.CheckedChanged += new System.EventHandler(this.cbShowChangelog_CheckedChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 12F);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(8, 40);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(328, 24);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Status:";
            // 
            // lblFilename
            // 
            this.lblFilename.BackColor = System.Drawing.Color.Transparent;
            this.lblFilename.Font = new System.Drawing.Font("Arial", 12F);
            this.lblFilename.ForeColor = System.Drawing.Color.White;
            this.lblFilename.Location = new System.Drawing.Point(8, 8);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(328, 24);
            this.lblFilename.TabIndex = 10;
            this.lblFilename.Text = "Filename:";
            // 
            // btnAction
            // 
            this.btnAction.BackColor = System.Drawing.Color.Transparent;
            this.btnAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAction.Font = new System.Drawing.Font("Arial", 12F);
            this.btnAction.ForeColor = System.Drawing.Color.White;
            this.btnAction.Location = new System.Drawing.Point(352, 163);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(128, 32);
            this.btnAction.TabIndex = 9;
            this.btnAction.Text = "Download";
            this.btnAction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnAction_MouseClick);
            // 
            // pbProgress
            // 
            this.pbProgress.BackColor = System.Drawing.Color.Transparent;
            this.pbProgress.Location = new System.Drawing.Point(8, 163);
            this.pbProgress.Maximum = 100;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(336, 32);
            this.pbProgress.TabIndex = 8;
            this.pbProgress.Value = 0;
            // 
            // UpdaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.ClientSize = new System.Drawing.Size(488, 417);
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
            this.Name = "UpdaterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdaterForm_FormClosing);
            this.Shown += new System.EventHandler(this.DownloaderForm_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UpdaterForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private MyProgressBar pbProgress;
        private MyButton btnAction;
        private MyLabel lblFilename;
        private MyLabel lblStatus;
        private MyLabel lblProgress;
        private System.Windows.Forms.TextBox txtChangelog;
        private MyCheckBox cbShowChangelog;
    }
}