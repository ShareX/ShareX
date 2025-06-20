namespace ShareX.ScreenCaptureLib
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenRecordForm));
            btnStart = new ShareX.HelpersLib.NoFocusBorderButton();
            lblTimer = new System.Windows.Forms.Label();
            timerRefresh = new System.Windows.Forms.Timer(components);
            btnAbort = new ShareX.HelpersLib.NoFocusBorderButton();
            pInfo = new System.Windows.Forms.Panel();
            btnPause = new ShareX.HelpersLib.NoFocusBorderButton();
            cmsMain = new System.Windows.Forms.ContextMenuStrip(components);
            tsmiStart = new System.Windows.Forms.ToolStripMenuItem();
            tsmiPause = new System.Windows.Forms.ToolStripMenuItem();
            tsmiAbort = new System.Windows.Forms.ToolStripMenuItem();
            niTray = new System.Windows.Forms.NotifyIcon(components);
            pInfo.SuspendLayout();
            cmsMain.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            resources.ApplyResources(btnStart, "btnStart");
            btnStart.Name = "btnStart";
            btnStart.MouseClick += btnStart_MouseClick;
            // 
            // lblTimer
            // 
            resources.ApplyResources(lblTimer, "lblTimer");
            lblTimer.Name = "lblTimer";
            lblTimer.MouseDown += lblTimer_MouseDown;
            lblTimer.MouseMove += lblTimer_MouseMove;
            lblTimer.MouseUp += lblTimer_MouseUp;
            // 
            // timerRefresh
            // 
            timerRefresh.Tick += timerRefresh_Tick;
            // 
            // btnAbort
            // 
            resources.ApplyResources(btnAbort, "btnAbort");
            btnAbort.Name = "btnAbort";
            btnAbort.MouseClick += btnAbort_MouseClick;
            // 
            // pInfo
            // 
            resources.ApplyResources(pInfo, "pInfo");
            pInfo.Controls.Add(btnPause);
            pInfo.Controls.Add(btnAbort);
            pInfo.Controls.Add(btnStart);
            pInfo.Controls.Add(lblTimer);
            pInfo.Name = "pInfo";
            // 
            // btnPause
            // 
            resources.ApplyResources(btnPause, "btnPause");
            btnPause.Name = "btnPause";
            btnPause.MouseClick += btnPause_MouseClick;
            // 
            // cmsMain
            // 
            cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiStart, tsmiPause, tsmiAbort });
            cmsMain.Name = "cmsMain";
            resources.ApplyResources(cmsMain, "cmsMain");
            // 
            // tsmiStart
            // 
            tsmiStart.Image = Properties.Resources.control_record;
            tsmiStart.Name = "tsmiStart";
            resources.ApplyResources(tsmiStart, "tsmiStart");
            tsmiStart.MouseUp += btnStart_MouseClick;
            // 
            // tsmiPause
            // 
            tsmiPause.Image = Properties.Resources.control_pause;
            tsmiPause.Name = "tsmiPause";
            resources.ApplyResources(tsmiPause, "tsmiPause");
            tsmiPause.MouseUp += btnPause_MouseClick;
            // 
            // tsmiAbort
            // 
            tsmiAbort.Image = Properties.Resources.cross;
            tsmiAbort.Name = "tsmiAbort";
            resources.ApplyResources(tsmiAbort, "tsmiAbort");
            tsmiAbort.MouseUp += btnAbort_MouseClick;
            // 
            // niTray
            // 
            niTray.ContextMenuStrip = cmsMain;
            resources.ApplyResources(niTray, "niTray");
            niTray.MouseClick += btnStart_MouseClick;
            // 
            // ScreenRecordForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(pInfo);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "ScreenRecordForm";
            ShowInTaskbar = false;
            FormClosed += ScreenRecordForm_FormClosed;
            Shown += ScreenRegionForm_Shown;
            pInfo.ResumeLayout(false);
            cmsMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private ShareX.HelpersLib.NoFocusBorderButton btnStart;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Label lblTimer;
        private ShareX.HelpersLib.NoFocusBorderButton btnAbort;
        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbort;
        private System.Windows.Forms.NotifyIcon niTray;
        private HelpersLib.NoFocusBorderButton btnPause;
        private System.Windows.Forms.ToolStripMenuItem tsmiPause;
    }
}