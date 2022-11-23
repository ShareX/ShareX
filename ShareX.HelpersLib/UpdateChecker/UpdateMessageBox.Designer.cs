namespace ShareX.HelpersLib
{
    partial class UpdateMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateMessageBox));
            this.lblText = new System.Windows.Forms.Label();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.lblViewChangelog = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblText
            // 
            resources.ApplyResources(this.lblText, "lblText");
            this.lblText.Name = "lblText";
            // 
            // btnNo
            // 
            resources.ApplyResources(this.btnNo, "btnNo");
            this.btnNo.Name = "btnNo";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnNo_MouseClick);
            // 
            // btnYes
            // 
            resources.ApplyResources(this.btnYes, "btnYes");
            this.btnYes.Name = "btnYes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnYes_MouseClick);
            // 
            // lblViewChangelog
            // 
            resources.ApplyResources(this.lblViewChangelog, "lblViewChangelog");
            this.lblViewChangelog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewChangelog.Name = "lblViewChangelog";
            this.lblViewChangelog.Click += new System.EventHandler(this.lblViewChangelog_Click);
            // 
            // UpdateMessageBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblViewChangelog);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lblText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UpdateMessageBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateMessageBox_FormClosing);
            this.Shown += new System.EventHandler(this.UpdateMessageBox_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblViewChangelog;
    }
}