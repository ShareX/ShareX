namespace ShareX.HelpersLib
{
    partial class MyPictureBox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyPictureBox));
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbMain = new System.Windows.Forms.PictureBox();
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.lblImageSize = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Name = "lblStatus";
            // 
            // pbMain
            // 
            resources.ApplyResources(this.pbMain, "pbMain");
            this.pbMain.Name = "pbMain";
            this.pbMain.TabStop = false;
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyImage});
            this.cmsMenu.Name = "cmsMenu";
            this.cmsMenu.ShowImageMargin = false;
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            // 
            // tsmiCopyImage
            // 
            this.tsmiCopyImage.Name = "tsmiCopyImage";
            resources.ApplyResources(this.tsmiCopyImage, "tsmiCopyImage");
            this.tsmiCopyImage.Click += new System.EventHandler(this.tsmiCopyImage_Click);
            // 
            // lblImageSize
            // 
            resources.ApplyResources(this.lblImageSize, "lblImageSize");
            this.lblImageSize.BackColor = System.Drawing.SystemColors.Window;
            this.lblImageSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblImageSize.Name = "lblImageSize";
            // 
            // MyPictureBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblImageSize);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbMain);
            this.Name = "MyPictureBox";
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Component Designer generated code

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox pbMain;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyImage;
        private System.Windows.Forms.Label lblImageSize;
    }
}