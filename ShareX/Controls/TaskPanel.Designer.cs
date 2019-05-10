namespace ShareX
{
    partial class TaskPanel
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
            this.pThumbnail = new ShareX.HelpersLib.RoundedCornerPanel();
            this.pbProgress = new ShareX.HelpersLib.BlackStyleProgressBar();
            this.pbThumbnail = new ShareX.HelpersLib.MyPictureBox();
            this.lblFilename = new ShareX.HelpersLib.BlackStyleLabel();
            this.pThumbnail.SuspendLayout();
            this.SuspendLayout();
            // 
            // pThumbnail
            // 
            this.pThumbnail.BackColor = System.Drawing.Color.Transparent;
            this.pThumbnail.Controls.Add(this.pbProgress);
            this.pThumbnail.Controls.Add(this.pbThumbnail);
            this.pThumbnail.Location = new System.Drawing.Point(0, 24);
            this.pThumbnail.Name = "pThumbnail";
            this.pThumbnail.Padding = new System.Windows.Forms.Padding(5);
            this.pThumbnail.PanelColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.pThumbnail.Radius = 5F;
            this.pThumbnail.Size = new System.Drawing.Size(256, 222);
            this.pThumbnail.TabIndex = 0;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(8, 183);
            this.pbProgress.Maximum = 100;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(240, 31);
            this.pbProgress.TabIndex = 1;
            this.pbProgress.Text = "blackStyleProgressBar1";
            this.pbProgress.Value = 50;
            this.pbProgress.Visible = false;
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.BackColor = System.Drawing.SystemColors.Window;
            this.pbThumbnail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbThumbnail.DrawCheckeredBackground = true;
            this.pbThumbnail.FullscreenOnClick = true;
            this.pbThumbnail.Location = new System.Drawing.Point(5, 5);
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.Size = new System.Drawing.Size(246, 212);
            this.pbThumbnail.TabIndex = 0;
            // 
            // lblFilename
            // 
            this.lblFilename.AutoEllipsis = true;
            this.lblFilename.BackColor = System.Drawing.Color.Transparent;
            this.lblFilename.Font = new System.Drawing.Font("Arial", 12F);
            this.lblFilename.ForeColor = System.Drawing.Color.White;
            this.lblFilename.Location = new System.Drawing.Point(0, 0);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(256, 24);
            this.lblFilename.TabIndex = 1;
            this.lblFilename.Text = "Test.png";
            this.lblFilename.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TaskPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pThumbnail);
            this.Controls.Add(this.lblFilename);
            this.Name = "TaskPanel";
            this.Size = new System.Drawing.Size(259, 249);
            this.pThumbnail.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private HelpersLib.RoundedCornerPanel pThumbnail;
        private HelpersLib.MyPictureBox pbThumbnail;
        private HelpersLib.BlackStyleLabel lblFilename;
        private HelpersLib.BlackStyleProgressBar pbProgress;
    }
}
