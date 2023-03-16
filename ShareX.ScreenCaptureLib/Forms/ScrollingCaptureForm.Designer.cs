namespace ShareX.ScreenCaptureLib
{
    partial class ScrollingCaptureForm
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
            this.btnCapture = new System.Windows.Forms.Button();
            this.pOutput = new System.Windows.Forms.Panel();
            this.pbOutput = new System.Windows.Forms.PictureBox();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.lblResultSize = new System.Windows.Forms.Label();
            this.pOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCapture
            // 
            this.btnCapture.Location = new System.Drawing.Point(8, 8);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(160, 32);
            this.btnCapture.TabIndex = 0;
            this.btnCapture.Text = "Capture...";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // pOutput
            // 
            this.pOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pOutput.AutoScroll = true;
            this.pOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pOutput.Controls.Add(this.pbOutput);
            this.pOutput.Location = new System.Drawing.Point(8, 48);
            this.pOutput.Name = "pOutput";
            this.pOutput.Size = new System.Drawing.Size(968, 605);
            this.pOutput.TabIndex = 3;
            // 
            // pbOutput
            // 
            this.pbOutput.Location = new System.Drawing.Point(0, 0);
            this.pbOutput.Name = "pbOutput";
            this.pbOutput.Size = new System.Drawing.Size(100, 100);
            this.pbOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOutput.TabIndex = 0;
            this.pbOutput.TabStop = false;
            this.pbOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbOutput_MouseDown);
            this.pbOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbOutput_MouseMove);
            this.pbOutput.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbOutput_MouseUp);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(344, 8);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(160, 32);
            this.btnOptions.TabIndex = 2;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Enabled = false;
            this.btnUpload.Location = new System.Drawing.Point(176, 8);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(160, 32);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Upload / Save";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // lblResultSize
            // 
            this.lblResultSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultSize.Location = new System.Drawing.Point(872, 18);
            this.lblResultSize.Name = "lblResultSize";
            this.lblResultSize.Size = new System.Drawing.Size(108, 24);
            this.lblResultSize.TabIndex = 4;
            this.lblResultSize.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // ScrollingCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.lblResultSize);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.pOutput);
            this.Controls.Add(this.btnCapture);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "ScrollingCaptureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Scrolling capture";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Activated += new System.EventHandler(this.ScrollingCaptureForm_Activated);
            this.Load += new System.EventHandler(this.ScrollingCaptureForm_Load);
            this.pOutput.ResumeLayout(false);
            this.pOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Panel pOutput;
        private System.Windows.Forms.PictureBox pbOutput;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Label lblResultSize;
    }
}