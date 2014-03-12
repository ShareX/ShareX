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
            this.btnFullscreen = new System.Windows.Forms.Button();
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblDurationSeconds = new System.Windows.Forms.Label();
            this.ssBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRepeatTime)).BeginInit();
            this.SuspendLayout();
            // 
            // ssBar
            // 
            this.ssBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspbBar,
            this.tsslStatus});
            this.ssBar.Location = new System.Drawing.Point(0, 158);
            this.ssBar.Name = "ssBar";
            this.ssBar.Size = new System.Drawing.Size(319, 22);
            this.ssBar.SizingGrip = false;
            this.ssBar.TabIndex = 6;
            this.ssBar.Text = "statusStrip1";
            // 
            // tspbBar
            // 
            this.tspbBar.Name = "tspbBar";
            this.tspbBar.Size = new System.Drawing.Size(75, 16);
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // btnExecute
            // 
            this.btnExecute.Enabled = false;
            this.btnExecute.Location = new System.Drawing.Point(192, 104);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(112, 40);
            this.btnExecute.TabIndex = 4;
            this.btnExecute.Text = "Start";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // cbWaitUploads
            // 
            this.cbWaitUploads.AutoSize = true;
            this.cbWaitUploads.Location = new System.Drawing.Point(16, 128);
            this.cbWaitUploads.Name = "cbWaitUploads";
            this.cbWaitUploads.Size = new System.Drawing.Size(144, 17);
            this.cbWaitUploads.TabIndex = 5;
            this.cbWaitUploads.Text = "Wait until tasks complete";
            this.cbWaitUploads.UseVisualStyleBackColor = true;
            this.cbWaitUploads.CheckedChanged += new System.EventHandler(this.cbWaitUploads_CheckedChanged);
            // 
            // cbAutoMinimize
            // 
            this.cbAutoMinimize.AutoSize = true;
            this.cbAutoMinimize.Location = new System.Drawing.Point(16, 104);
            this.cbAutoMinimize.Name = "cbAutoMinimize";
            this.cbAutoMinimize.Size = new System.Drawing.Size(122, 17);
            this.cbAutoMinimize.TabIndex = 3;
            this.cbAutoMinimize.Text = "Auto minimize to tray";
            this.cbAutoMinimize.UseVisualStyleBackColor = true;
            this.cbAutoMinimize.CheckedChanged += new System.EventHandler(this.cbAutoMinimize_CheckedChanged);
            // 
            // lblRegion
            // 
            this.lblRegion.AutoSize = true;
            this.lblRegion.Location = new System.Drawing.Point(16, 48);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(41, 13);
            this.lblRegion.TabIndex = 9;
            this.lblRegion.Text = "Region";
            // 
            // btnRegion
            // 
            this.btnRegion.Location = new System.Drawing.Point(16, 16);
            this.btnRegion.Name = "btnRegion";
            this.btnRegion.Size = new System.Drawing.Size(104, 24);
            this.btnRegion.TabIndex = 8;
            this.btnRegion.Text = "Select region";
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
            this.nudRepeatTime.Location = new System.Drawing.Point(88, 72);
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
            this.nudRepeatTime.Size = new System.Drawing.Size(64, 20);
            this.nudRepeatTime.TabIndex = 11;
            this.nudRepeatTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudRepeatTime.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudRepeatTime.ValueChanged += new System.EventHandler(this.nudDuration_ValueChanged);
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(16, 76);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(67, 13);
            this.lblDuration.TabIndex = 10;
            this.lblDuration.Text = "Repeat time:";
            // 
            // btnFullscreen
            // 
            this.btnFullscreen.Location = new System.Drawing.Point(128, 16);
            this.btnFullscreen.Name = "btnFullscreen";
            this.btnFullscreen.Size = new System.Drawing.Size(104, 23);
            this.btnFullscreen.TabIndex = 12;
            this.btnFullscreen.Text = "Fullscreen";
            this.btnFullscreen.UseVisualStyleBackColor = true;
            this.btnFullscreen.Click += new System.EventHandler(this.btnFullscreen_Click);
            // 
            // niTray
            // 
            this.niTray.Text = "ShareX - Auto capture";
            this.niTray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseClick);
            // 
            // lblDurationSeconds
            // 
            this.lblDurationSeconds.AutoSize = true;
            this.lblDurationSeconds.Location = new System.Drawing.Point(160, 76);
            this.lblDurationSeconds.Name = "lblDurationSeconds";
            this.lblDurationSeconds.Size = new System.Drawing.Size(47, 13);
            this.lblDurationSeconds.TabIndex = 13;
            this.lblDurationSeconds.Text = "seconds";
            // 
            // AutoCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 180);
            this.Controls.Add(this.lblDurationSeconds);
            this.Controls.Add(this.btnFullscreen);
            this.Controls.Add(this.nudRepeatTime);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblRegion);
            this.Controls.Add(this.btnRegion);
            this.Controls.Add(this.cbAutoMinimize);
            this.Controls.Add(this.cbWaitUploads);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.ssBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AutoCaptureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Auto capture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoCapture_FormClosing);
            this.Resize += new System.EventHandler(this.AutoCapture_Resize);
            this.ssBar.ResumeLayout(false);
            this.ssBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRepeatTime)).EndInit();
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
        private System.Windows.Forms.Button btnFullscreen;
        private System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.Label lblDurationSeconds;
    }
}