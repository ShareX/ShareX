namespace ShareX.ScreenCaptureLib
{
    partial class ScrollingCaptureForm
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
            this.btnSelectHandle = new System.Windows.Forms.Button();
            this.lblControlText = new System.Windows.Forms.Label();
            this.btnCapture = new System.Windows.Forms.Button();
            this.captureTimer = new System.Windows.Forms.Timer(this.components);
            this.nudScrollDelay = new System.Windows.Forms.NumericUpDown();
            this.nudMaximumScrollCount = new System.Windows.Forms.NumericUpDown();
            this.lblScrollDelay = new System.Windows.Forms.Label();
            this.lblMaximumScrollCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectHandle
            // 
            this.btnSelectHandle.Location = new System.Drawing.Point(8, 8);
            this.btnSelectHandle.Name = "btnSelectHandle";
            this.btnSelectHandle.Size = new System.Drawing.Size(304, 23);
            this.btnSelectHandle.TabIndex = 0;
            this.btnSelectHandle.Text = "Select window or control to scroll";
            this.btnSelectHandle.UseVisualStyleBackColor = true;
            this.btnSelectHandle.Click += new System.EventHandler(this.btnSelectHandle_Click);
            // 
            // lblControlText
            // 
            this.lblControlText.Location = new System.Drawing.Point(320, 13);
            this.lblControlText.Name = "lblControlText";
            this.lblControlText.Size = new System.Drawing.Size(240, 19);
            this.lblControlText.TabIndex = 1;
            this.lblControlText.Text = "Text";
            // 
            // btnCapture
            // 
            this.btnCapture.Enabled = false;
            this.btnCapture.Location = new System.Drawing.Point(8, 104);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(152, 23);
            this.btnCapture.TabIndex = 2;
            this.btnCapture.Text = "Start capture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // captureTimer
            // 
            this.captureTimer.Tick += new System.EventHandler(this.captureTimer_Tick);
            // 
            // nudScrollDelay
            // 
            this.nudScrollDelay.Location = new System.Drawing.Point(136, 44);
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.Size = new System.Drawing.Size(80, 20);
            this.nudScrollDelay.TabIndex = 3;
            this.nudScrollDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScrollDelay.ValueChanged += new System.EventHandler(this.nudScrollDelay_ValueChanged);
            // 
            // nudMaximumScrollCount
            // 
            this.nudMaximumScrollCount.Location = new System.Drawing.Point(136, 68);
            this.nudMaximumScrollCount.Name = "nudMaximumScrollCount";
            this.nudMaximumScrollCount.Size = new System.Drawing.Size(80, 20);
            this.nudMaximumScrollCount.TabIndex = 4;
            this.nudMaximumScrollCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudMaximumScrollCount.ValueChanged += new System.EventHandler(this.nudMaximumScrollCount_ValueChanged);
            // 
            // lblScrollDelay
            // 
            this.lblScrollDelay.AutoSize = true;
            this.lblScrollDelay.Location = new System.Drawing.Point(8, 48);
            this.lblScrollDelay.Name = "lblScrollDelay";
            this.lblScrollDelay.Size = new System.Drawing.Size(64, 13);
            this.lblScrollDelay.TabIndex = 5;
            this.lblScrollDelay.Text = "Scroll delay:";
            // 
            // lblMaximumScrollCount
            // 
            this.lblMaximumScrollCount.AutoSize = true;
            this.lblMaximumScrollCount.Location = new System.Drawing.Point(8, 72);
            this.lblMaximumScrollCount.Name = "lblMaximumScrollCount";
            this.lblMaximumScrollCount.Size = new System.Drawing.Size(111, 13);
            this.lblMaximumScrollCount.TabIndex = 6;
            this.lblMaximumScrollCount.Text = "Maximum scroll count:";
            // 
            // ScrollingCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 427);
            this.Controls.Add(this.lblMaximumScrollCount);
            this.Controls.Add(this.lblScrollDelay);
            this.Controls.Add(this.nudMaximumScrollCount);
            this.Controls.Add(this.nudScrollDelay);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.lblControlText);
            this.Controls.Add(this.btnSelectHandle);
            this.Name = "ScrollingCaptureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Scrolling capture";
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectHandle;
        private System.Windows.Forms.Label lblControlText;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Timer captureTimer;
        private System.Windows.Forms.NumericUpDown nudScrollDelay;
        private System.Windows.Forms.NumericUpDown nudMaximumScrollCount;
        private System.Windows.Forms.Label lblScrollDelay;
        private System.Windows.Forms.Label lblMaximumScrollCount;
    }
}