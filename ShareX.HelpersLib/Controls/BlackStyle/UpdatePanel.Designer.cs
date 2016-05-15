namespace ShareX.HelpersLib
{
    partial class UpdatePanel
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
            this.btnChangelog = new ShareX.HelpersLib.BlackStyleButton();
            this.btnDownload = new ShareX.HelpersLib.BlackStyleButton();
            this.SuspendLayout();
            // 
            // btnChangelog
            // 
            this.btnChangelog.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnChangelog.Font = new System.Drawing.Font("Arial", 12F);
            this.btnChangelog.ForeColor = System.Drawing.Color.White;
            this.btnChangelog.Location = new System.Drawing.Point(186, 4);
            this.btnChangelog.Name = "btnChangelog";
            this.btnChangelog.Size = new System.Drawing.Size(154, 32);
            this.btnChangelog.TabIndex = 1;
            this.btnChangelog.Text = "Changelog";
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDownload.Font = new System.Drawing.Font("Arial", 12F);
            this.btnDownload.ForeColor = System.Drawing.Color.White;
            this.btnDownload.Location = new System.Drawing.Point(344, 4);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(154, 32);
            this.btnDownload.TabIndex = 0;
            this.btnDownload.Text = "Download";
            // 
            // UpdatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnChangelog);
            this.Controls.Add(this.btnDownload);
            this.Name = "UpdatePanel";
            this.Size = new System.Drawing.Size(502, 39);
            this.ResumeLayout(false);

        }

        #endregion

        private BlackStyleButton btnDownload;
        private BlackStyleButton btnChangelog;
    }
}
