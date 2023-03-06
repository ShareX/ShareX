namespace ShareX.ScreenCaptureLib
{
    partial class ScrollingCaptureOptionsForm
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
            this.lblStartDelay = new System.Windows.Forms.Label();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.lblScrollDelay = new System.Windows.Forms.Label();
            this.nudScrollDelay = new System.Windows.Forms.NumericUpDown();
            this.cbAutoScrollTop = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStartDelay
            // 
            this.lblStartDelay.AutoSize = true;
            this.lblStartDelay.Location = new System.Drawing.Point(13, 16);
            this.lblStartDelay.Name = "lblStartDelay";
            this.lblStartDelay.Size = new System.Drawing.Size(74, 16);
            this.lblStartDelay.TabIndex = 0;
            this.lblStartDelay.Text = "Start delay:";
            // 
            // nudStartDelay
            // 
            this.nudStartDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudStartDelay.Location = new System.Drawing.Point(16, 40);
            this.nudStartDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudStartDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudStartDelay.Name = "nudStartDelay";
            this.nudStartDelay.Size = new System.Drawing.Size(80, 22);
            this.nudStartDelay.TabIndex = 1;
            this.nudStartDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudStartDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudStartDelay.ValueChanged += new System.EventHandler(this.nudStartDelay_ValueChanged);
            // 
            // lblScrollDelay
            // 
            this.lblScrollDelay.AutoSize = true;
            this.lblScrollDelay.Location = new System.Drawing.Point(13, 112);
            this.lblScrollDelay.Name = "lblScrollDelay";
            this.lblScrollDelay.Size = new System.Drawing.Size(81, 16);
            this.lblScrollDelay.TabIndex = 2;
            this.lblScrollDelay.Text = "Scroll delay:";
            // 
            // nudScrollDelay
            // 
            this.nudScrollDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudScrollDelay.Location = new System.Drawing.Point(16, 136);
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.Size = new System.Drawing.Size(80, 22);
            this.nudScrollDelay.TabIndex = 3;
            this.nudScrollDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScrollDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudScrollDelay.ValueChanged += new System.EventHandler(this.nudScrollDelay_ValueChanged);
            // 
            // cbAutoScrollTop
            // 
            this.cbAutoScrollTop.AutoSize = true;
            this.cbAutoScrollTop.Location = new System.Drawing.Point(16, 80);
            this.cbAutoScrollTop.Name = "cbAutoScrollTop";
            this.cbAutoScrollTop.Size = new System.Drawing.Size(177, 20);
            this.cbAutoScrollTop.TabIndex = 4;
            this.cbAutoScrollTop.Text = "Automatically scroll to top";
            this.cbAutoScrollTop.UseVisualStyleBackColor = true;
            this.cbAutoScrollTop.CheckedChanged += new System.EventHandler(this.cbAutoScrollTop_CheckedChanged);
            // 
            // ScrollingCaptureOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(434, 361);
            this.Controls.Add(this.cbAutoScrollTop);
            this.Controls.Add(this.nudScrollDelay);
            this.Controls.Add(this.lblScrollDelay);
            this.Controls.Add(this.nudStartDelay);
            this.Controls.Add(this.lblStartDelay);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ScrollingCaptureOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Scrolling capture options";
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStartDelay;
        private System.Windows.Forms.NumericUpDown nudStartDelay;
        private System.Windows.Forms.Label lblScrollDelay;
        private System.Windows.Forms.NumericUpDown nudScrollDelay;
        private System.Windows.Forms.CheckBox cbAutoScrollTop;
    }
}