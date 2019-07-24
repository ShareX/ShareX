namespace ShareX
{
    partial class TaskThumbnailPanel
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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new ShareX.HelpersLib.BlackStyleLabel();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.pThumbnail = new ShareX.TaskRoundedCornerPanel();
            this.pbProgress = new ShareX.HelpersLib.BlackStyleProgressBar();
            this.pbThumbnail = new System.Windows.Forms.PictureBox();
            this.pThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(256, 22);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Test.png";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LblTitle_MouseClick);
            // 
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 5000;
            this.ttMain.InitialDelay = 200;
            this.ttMain.OwnerDraw = true;
            this.ttMain.ReshowDelay = 100;
            this.ttMain.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.TtMain_Draw);
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
            this.pThumbnail.Size = new System.Drawing.Size(256, 256);
            this.pThumbnail.TabIndex = 0;
            this.pThumbnail.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseClick);
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbProgress.ForeColor = System.Drawing.Color.White;
            this.pbProgress.Location = new System.Drawing.Point(8, 216);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.ShowPercentageText = true;
            this.pbProgress.Size = new System.Drawing.Size(240, 32);
            this.pbProgress.TabIndex = 1;
            this.pbProgress.Text = null;
            this.pbProgress.Visible = false;
            this.pbProgress.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseClick);
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
            this.pbThumbnail.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseClick);
            this.pbThumbnail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseDown);
            this.pbThumbnail.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseMove);
            this.pbThumbnail.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseUp);
            // 
            // TaskThumbnailPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pThumbnail);
            this.Controls.Add(this.lblTitle);
            this.Name = "TaskThumbnailPanel";
            this.Size = new System.Drawing.Size(256, 280);
            this.pThumbnail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ShareX.TaskRoundedCornerPanel pThumbnail;
        private HelpersLib.BlackStyleLabel lblTitle;
        private HelpersLib.BlackStyleProgressBar pbProgress;
        private System.Windows.Forms.PictureBox pbThumbnail;
        private System.Windows.Forms.ToolTip ttMain;
    }
}
