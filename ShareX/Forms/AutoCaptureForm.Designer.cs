namespace ShareX
{
    partial class AutoCaptureForm
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

            if (screenshotTimer != null)
            {
                screenshotTimer.Dispose();
            }

            if (statusTimer != null)
            {
                statusTimer.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoCaptureForm));
            this.ssBar = new System.Windows.Forms.StatusStrip();
            this.tspbBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnExecute = new System.Windows.Forms.Button();
            this.cbWaitUploads = new System.Windows.Forms.CheckBox();
            this.cbAutoMinimize = new System.Windows.Forms.CheckBox();
            this.lblRegion = new System.Windows.Forms.Label();
            this.btnRegion = new System.Windows.Forms.Button();
            this.nudRepeatTime = new System.Windows.Forms.NumericUpDown();
            this.lblDuration = new System.Windows.Forms.Label();
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblDurationSeconds = new System.Windows.Forms.Label();
            this.gbRegion = new System.Windows.Forms.GroupBox();
            this.rbFullscreen = new System.Windows.Forms.RadioButton();
            this.rbCustomRegion = new System.Windows.Forms.RadioButton();
            this.ssBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRepeatTime)).BeginInit();
            this.gbRegion.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssBar
            // 
            this.ssBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspbBar,
            this.tsslStatus});
            resources.ApplyResources(this.ssBar, "ssBar");
            this.ssBar.Name = "ssBar";
            this.ssBar.SizingGrip = false;
            // 
            // tspbBar
            // 
            this.tspbBar.Name = "tspbBar";
            resources.ApplyResources(this.tspbBar, "tspbBar");
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            resources.ApplyResources(this.tsslStatus, "tsslStatus");
            // 
            // btnExecute
            // 
            resources.ApplyResources(this.btnExecute, "btnExecute");
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // cbWaitUploads
            // 
            resources.ApplyResources(this.cbWaitUploads, "cbWaitUploads");
            this.cbWaitUploads.Name = "cbWaitUploads";
            this.cbWaitUploads.UseVisualStyleBackColor = true;
            this.cbWaitUploads.CheckedChanged += new System.EventHandler(this.cbWaitUploads_CheckedChanged);
            // 
            // cbAutoMinimize
            // 
            resources.ApplyResources(this.cbAutoMinimize, "cbAutoMinimize");
            this.cbAutoMinimize.Name = "cbAutoMinimize";
            this.cbAutoMinimize.UseVisualStyleBackColor = true;
            this.cbAutoMinimize.CheckedChanged += new System.EventHandler(this.cbAutoMinimize_CheckedChanged);
            // 
            // lblRegion
            // 
            resources.ApplyResources(this.lblRegion, "lblRegion");
            this.lblRegion.Name = "lblRegion";
            // 
            // btnRegion
            // 
            resources.ApplyResources(this.btnRegion, "btnRegion");
            this.btnRegion.Name = "btnRegion";
            this.btnRegion.UseVisualStyleBackColor = true;
            this.btnRegion.Click += new System.EventHandler(this.btnRegion_Click);
            // 
            // nudRepeatTime
            // 
            this.nudRepeatTime.DecimalPlaces = 1;
            this.nudRepeatTime.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudRepeatTime, "nudRepeatTime");
            this.nudRepeatTime.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.nudRepeatTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRepeatTime.Name = "nudRepeatTime";
            this.nudRepeatTime.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudRepeatTime.ValueChanged += new System.EventHandler(this.nudDuration_ValueChanged);
            // 
            // lblDuration
            // 
            resources.ApplyResources(this.lblDuration, "lblDuration");
            this.lblDuration.Name = "lblDuration";
            // 
            // niTray
            // 
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseClick);
            // 
            // lblDurationSeconds
            // 
            resources.ApplyResources(this.lblDurationSeconds, "lblDurationSeconds");
            this.lblDurationSeconds.Name = "lblDurationSeconds";
            // 
            // gbRegion
            // 
            this.gbRegion.Controls.Add(this.rbFullscreen);
            this.gbRegion.Controls.Add(this.rbCustomRegion);
            this.gbRegion.Controls.Add(this.btnRegion);
            this.gbRegion.Controls.Add(this.lblRegion);
            resources.ApplyResources(this.gbRegion, "gbRegion");
            this.gbRegion.Name = "gbRegion";
            this.gbRegion.TabStop = false;
            // 
            // rbFullscreen
            // 
            resources.ApplyResources(this.rbFullscreen, "rbFullscreen");
            this.rbFullscreen.Name = "rbFullscreen";
            this.rbFullscreen.UseVisualStyleBackColor = true;
            this.rbFullscreen.CheckedChanged += new System.EventHandler(this.rbFullscreen_CheckedChanged);
            // 
            // rbCustomRegion
            // 
            resources.ApplyResources(this.rbCustomRegion, "rbCustomRegion");
            this.rbCustomRegion.Checked = true;
            this.rbCustomRegion.Name = "rbCustomRegion";
            this.rbCustomRegion.TabStop = true;
            this.rbCustomRegion.UseVisualStyleBackColor = true;
            this.rbCustomRegion.CheckedChanged += new System.EventHandler(this.rbCustomRegion_CheckedChanged);
            // 
            // AutoCaptureForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.gbRegion);
            this.Controls.Add(this.lblDurationSeconds);
            this.Controls.Add(this.nudRepeatTime);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.cbAutoMinimize);
            this.Controls.Add(this.cbWaitUploads);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.ssBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AutoCaptureForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoCapture_FormClosing);
            this.Resize += new System.EventHandler(this.AutoCapture_Resize);
            this.ssBar.ResumeLayout(false);
            this.ssBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRepeatTime)).EndInit();
            this.gbRegion.ResumeLayout(false);
            this.gbRegion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.StatusStrip ssBar;
        private System.Windows.Forms.ToolStripProgressBar tspbBar;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.CheckBox cbWaitUploads;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.CheckBox cbAutoMinimize;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.Button btnRegion;
        private System.Windows.Forms.NumericUpDown nudRepeatTime;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.Label lblDurationSeconds;
        private System.Windows.Forms.GroupBox gbRegion;
        private System.Windows.Forms.RadioButton rbFullscreen;
        private System.Windows.Forms.RadioButton rbCustomRegion;
    }
}