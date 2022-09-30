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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskThumbnailPanel));
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.lblTitle = new ShareX.HelpersLib.BlackStyleLabel();
            this.pThumbnail = new ShareX.TaskRoundedCornerPanel();
            this.lblCombineVertical = new ShareX.HelpersLib.BlackStyleLabel();
            this.lblError = new ShareX.HelpersLib.BlackStyleLabel();
            this.lblCombineHorizontal = new ShareX.HelpersLib.BlackStyleLabel();
            this.pbProgress = new ShareX.HelpersLib.BlackStyleProgressBar();
            this.pbThumbnail = new System.Windows.Forms.PictureBox();
            this.pThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 5000;
            this.ttMain.InitialDelay = 200;
            this.ttMain.OwnerDraw = true;
            this.ttMain.ReshowDelay = 100;
            this.ttMain.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.TtMain_Draw);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LblTitle_MouseClick);
            // 
            // pThumbnail
            // 
            this.pThumbnail.AllowDrop = true;
            this.pThumbnail.BackColor = System.Drawing.Color.Transparent;
            this.pThumbnail.Controls.Add(this.lblCombineVertical);
            this.pThumbnail.Controls.Add(this.lblError);
            this.pThumbnail.Controls.Add(this.lblCombineHorizontal);
            this.pThumbnail.Controls.Add(this.pbProgress);
            this.pThumbnail.Controls.Add(this.pbThumbnail);
            resources.ApplyResources(this.pThumbnail, "pThumbnail");
            this.pThumbnail.Name = "pThumbnail";
            this.pThumbnail.PanelColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.pThumbnail.Radius = 5F;
            this.pThumbnail.Selected = false;
            this.pThumbnail.StatusLocation = ShareX.ThumbnailTitleLocation.Top;
            this.pThumbnail.DragDrop += new System.Windows.Forms.DragEventHandler(this.pThumbnail_DragDrop);
            this.pThumbnail.DragEnter += new System.Windows.Forms.DragEventHandler(this.pThumbnail_DragEnter);
            this.pThumbnail.DragLeave += new System.EventHandler(this.pThumbnail_DragLeave);
            // 
            // lblCombineVertical
            // 
            this.lblCombineVertical.BackColor = System.Drawing.Color.Transparent;
            this.lblCombineVertical.BorderColor = System.Drawing.Color.Empty;
            this.lblCombineVertical.DrawBorder = true;
            resources.ApplyResources(this.lblCombineVertical, "lblCombineVertical");
            this.lblCombineVertical.ForeColor = System.Drawing.Color.White;
            this.lblCombineVertical.Name = "lblCombineVertical";
            this.lblCombineVertical.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblError
            // 
            resources.ApplyResources(this.lblError, "lblError");
            this.lblError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblError.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblError.ForeColor = System.Drawing.Color.White;
            this.lblError.Name = "lblError";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblError.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblError_MouseClick);
            // 
            // lblCombineHorizontal
            // 
            this.lblCombineHorizontal.BackColor = System.Drawing.Color.Transparent;
            this.lblCombineHorizontal.BorderColor = System.Drawing.Color.Empty;
            this.lblCombineHorizontal.DrawBorder = true;
            resources.ApplyResources(this.lblCombineHorizontal, "lblCombineHorizontal");
            this.lblCombineHorizontal.ForeColor = System.Drawing.Color.White;
            this.lblCombineHorizontal.Name = "lblCombineHorizontal";
            this.lblCombineHorizontal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbProgress
            // 
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.ForeColor = System.Drawing.Color.White;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.ShowPercentageText = true;
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pbThumbnail, "pbThumbnail");
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.TabStop = false;
            this.pbThumbnail.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseClick);
            this.pbThumbnail.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbThumbnail_MouseDoubleClick);
            this.pbThumbnail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseDown);
            this.pbThumbnail.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseMove);
            this.pbThumbnail.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PbThumbnail_MouseUp);
            // 
            // TaskThumbnailPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pThumbnail);
            this.Controls.Add(this.lblTitle);
            this.Name = "TaskThumbnailPanel";
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
        private HelpersLib.BlackStyleLabel lblError;
        private HelpersLib.BlackStyleLabel lblCombineHorizontal;
        private HelpersLib.BlackStyleLabel lblCombineVertical;
    }
}
