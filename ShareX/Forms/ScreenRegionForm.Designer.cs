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
            this.pInfo = new System.Windows.Forms.Panel();
            this.btnStop = new HelpersLib.BlackStyleButton();
            this.lblTimer = new HelpersLib.BlackStyleLabel();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pInfo
            // 
            this.pInfo.BackColor = System.Drawing.Color.White;
            this.pInfo.Controls.Add(this.btnStop);
            this.pInfo.Controls.Add(this.lblTimer);
            resources.ApplyResources(this.pInfo, "pInfo");
            this.pInfo.Name = "pInfo";
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
            resources.ApplyResources(this.lblTimer, "lblTimer");
            this.lblTimer.DrawBorder = true;
            this.lblTimer.ForeColor = System.Drawing.Color.White;
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 200;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // ScreenRegionForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScreenRegionForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.pInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pInfo;
        private HelpersLib.BlackStyleButton btnStop;
        private System.Windows.Forms.Timer timerRefresh;
        private HelpersLib.BlackStyleLabel lblTimer;

    }
}