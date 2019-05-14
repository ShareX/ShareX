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

            ClearThumbnail();

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
            this.pbThumbnail = new System.Windows.Forms.PictureBox();
            this.lblFilename = new ShareX.HelpersLib.BlackStyleLabel();
            this.pThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // pThumbnail
            // 
            this.pThumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pThumbnail.BackColor = System.Drawing.Color.Transparent;
            this.pThumbnail.Controls.Add(this.pbProgress);
            this.pThumbnail.Controls.Add(this.pbThumbnail);
            this.pThumbnail.Location = new System.Drawing.Point(0, 24);
            this.pThumbnail.Name = "pThumbnail";
            this.pThumbnail.Padding = new System.Windows.Forms.Padding(5);
            this.pThumbnail.PanelColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.pThumbnail.Radius = 5F;
            this.pThumbnail.Size = new System.Drawing.Size(256, 256);
            this.pThumbnail.TabIndex = 0;
            this.pThumbnail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseDown);
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(8, 216);
            this.pbProgress.Maximum = 100;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(240, 32);
            this.pbProgress.TabIndex = 1;
            this.pbProgress.Value = 0;
            this.pbProgress.Visible = false;
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.BackColor = System.Drawing.Color.Transparent;
            this.pbThumbnail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbThumbnail.Location = new System.Drawing.Point(5, 5);
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.Size = new System.Drawing.Size(246, 246);
            this.pbThumbnail.TabIndex = 2;
            this.pbThumbnail.TabStop = false;
            this.pbThumbnail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseDown);
            // 
            // lblFilename
            // 
            this.lblFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilename.AutoEllipsis = true;
            this.lblFilename.BackColor = System.Drawing.Color.Transparent;
            this.lblFilename.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilename.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.lblFilename.Location = new System.Drawing.Point(0, 0);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(256, 22);
            this.lblFilename.TabIndex = 1;
            this.lblFilename.Text = "Test.png";
            this.lblFilename.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TaskPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pThumbnail);
            this.Controls.Add(this.lblFilename);
            this.Name = "TaskPanel";
            this.Size = new System.Drawing.Size(256, 280);
            this.pThumbnail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private HelpersLib.RoundedCornerPanel pThumbnail;
        private HelpersLib.BlackStyleLabel lblFilename;
        private HelpersLib.BlackStyleProgressBar pbProgress;
        private System.Windows.Forms.PictureBox pbThumbnail;
    }
}
