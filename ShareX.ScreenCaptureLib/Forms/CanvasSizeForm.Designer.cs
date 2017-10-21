namespace ShareX.ScreenCaptureLib
{
    partial class CanvasSizeForm
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
            this.nudLeft = new System.Windows.Forms.NumericUpDown();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.nudRight = new System.Windows.Forms.NumericUpDown();
            this.lblTop = new System.Windows.Forms.Label();
            this.nudTop = new System.Windows.Forms.NumericUpDown();
            this.lblBottom = new System.Windows.Forms.Label();
            this.nudBottom = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottom)).BeginInit();
            this.SuspendLayout();
            // 
            // nudLeft
            // 
            this.nudLeft.Location = new System.Drawing.Point(16, 80);
            this.nudLeft.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLeft.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudLeft.Name = "nudLeft";
            this.nudLeft.Size = new System.Drawing.Size(64, 20);
            this.nudLeft.TabIndex = 0;
            this.nudLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(13, 64);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(28, 13);
            this.lblLeft.TabIndex = 1;
            this.lblLeft.Text = "Left:";
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(157, 64);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(35, 13);
            this.lblRight.TabIndex = 3;
            this.lblRight.Text = "Right:";
            // 
            // nudRight
            // 
            this.nudRight.Location = new System.Drawing.Point(160, 80);
            this.nudRight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudRight.Name = "nudRight";
            this.nudRight.Size = new System.Drawing.Size(64, 20);
            this.nudRight.TabIndex = 2;
            this.nudRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTop
            // 
            this.lblTop.AutoSize = true;
            this.lblTop.Location = new System.Drawing.Point(85, 8);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(29, 13);
            this.lblTop.TabIndex = 5;
            this.lblTop.Text = "Top:";
            // 
            // nudTop
            // 
            this.nudTop.Location = new System.Drawing.Point(88, 24);
            this.nudTop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTop.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudTop.Name = "nudTop";
            this.nudTop.Size = new System.Drawing.Size(64, 20);
            this.nudTop.TabIndex = 4;
            this.nudTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblBottom
            // 
            this.lblBottom.AutoSize = true;
            this.lblBottom.Location = new System.Drawing.Point(85, 120);
            this.lblBottom.Name = "lblBottom";
            this.lblBottom.Size = new System.Drawing.Size(43, 13);
            this.lblBottom.TabIndex = 7;
            this.lblBottom.Text = "Bottom:";
            // 
            // nudBottom
            // 
            this.nudBottom.Location = new System.Drawing.Point(88, 136);
            this.nudBottom.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudBottom.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudBottom.Name = "nudBottom";
            this.nudBottom.Size = new System.Drawing.Size(64, 20);
            this.nudBottom.TabIndex = 6;
            this.nudBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 176);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(108, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(124, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CanvasSizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 208);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblBottom);
            this.Controls.Add(this.nudBottom);
            this.Controls.Add(this.lblTop);
            this.Controls.Add(this.nudTop);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.nudRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.nudLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CanvasSizeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Canvas size";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.CanvasSizeForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudLeft;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.NumericUpDown nudRight;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.NumericUpDown nudTop;
        private System.Windows.Forms.Label lblBottom;
        private System.Windows.Forms.NumericUpDown nudBottom;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}