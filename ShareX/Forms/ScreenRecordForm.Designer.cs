namespace ShareX
{
    partial class ScreenRecordForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenRecordForm));
            this.btnStart = new ShareX.HelpersLib.BlackStyleButton();
            this.lblTimer = new ShareX.HelpersLib.BlackStyleLabel();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.btnAbort = new ShareX.HelpersLib.BlackStyleButton();
            this.pInfo = new System.Windows.Forms.Panel();
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbort = new System.Windows.Forms.ToolStripMenuItem();
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.pInfo.SuspendLayout();
            this.cmsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Name = "btnStart";
            this.btnStart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnStart_MouseClick);
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.Color.DimGray;
            this.lblTimer.DrawBorder = true;
            resources.ApplyResources(this.lblTimer, "lblTimer");
            this.lblTimer.ForeColor = System.Drawing.Color.White;
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // btnAbort
            // 
            resources.ApplyResources(this.btnAbort, "btnAbort");
            this.btnAbort.ForeColor = System.Drawing.Color.White;
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnAbort_MouseClick);
            // 
            // pInfo
            // 
            resources.ApplyResources(this.pInfo, "pInfo");
            this.pInfo.Controls.Add(this.btnAbort);
            this.pInfo.Controls.Add(this.btnStart);
            this.pInfo.Controls.Add(this.lblTimer);
            this.pInfo.Name = "pInfo";
            // 
            // cmsMain
            // 
            this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiStart,
            this.tsmiAbort});
            this.cmsMain.Name = "cmsMain";
            resources.ApplyResources(this.cmsMain, "cmsMain");
            // 
            // tsmiStart
            // 
            this.tsmiStart.Image = global::ShareX.Properties.Resources.control_record;
            this.tsmiStart.Name = "tsmiStart";
            resources.ApplyResources(this.tsmiStart, "tsmiStart");
            this.tsmiStart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnStart_MouseClick);
            // 
            // tsmiAbort
            // 
            this.tsmiAbort.Image = global::ShareX.Properties.Resources.cross;
            this.tsmiAbort.Name = "tsmiAbort";
            resources.ApplyResources(this.tsmiAbort, "tsmiAbort");
            this.tsmiAbort.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAbort_MouseClick);
            // 
            // niTray
            // 
            this.niTray.ContextMenuStrip = this.cmsMain;
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnStart_MouseClick);
            // 
            // ScreenRecordForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScreenRecordForm";
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScreenRecordForm_FormClosed);
            this.Shown += new System.EventHandler(this.ScreenRegionForm_Shown);
            this.pInfo.ResumeLayout(false);
            this.cmsMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelpersLib.BlackStyleButton btnStart;
        private System.Windows.Forms.Timer timerRefresh;
        private HelpersLib.BlackStyleLabel lblTimer;
        private HelpersLib.BlackStyleButton btnAbort;
        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbort;
        private System.Windows.Forms.NotifyIcon niTray;
    }
}