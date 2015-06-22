namespace ShareX
{
    partial class ScreenRegionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenRegionForm));
            this.btnStop = new ShareX.HelpersLib.BlackStyleButton();
            this.lblTimer = new ShareX.HelpersLib.BlackStyleLabel();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.btnAbort = new ShareX.HelpersLib.BlackStyleButton();
            this.pInfo = new System.Windows.Forms.Panel();
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Name = "btnStop";
            this.btnStop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnStop_MouseClick);
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
            this.pInfo.BackColor = System.Drawing.Color.White;
            this.pInfo.Controls.Add(this.btnAbort);
            this.pInfo.Controls.Add(this.btnStop);
            this.pInfo.Controls.Add(this.lblTimer);
            this.pInfo.Name = "pInfo";
            // 
            // ScreenRegionForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScreenRegionForm";
            this.ShowInTaskbar = false;
            this.Shown += new System.EventHandler(this.ScreenRegionForm_Shown);
            this.pInfo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelpersLib.BlackStyleButton btnStop;
        private System.Windows.Forms.Timer timerRefresh;
        private HelpersLib.BlackStyleLabel lblTimer;
        private HelpersLib.BlackStyleButton btnAbort;
        private System.Windows.Forms.Panel pInfo;

    }
}