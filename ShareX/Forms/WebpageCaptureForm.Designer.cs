namespace ShareX
{
    partial class WebpageCaptureForm
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

                if (webpageCapture != null)
                {
                    webpageCapture.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebpageCaptureForm));
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.btnCapture = new System.Windows.Forms.Button();
            this.pbResult = new System.Windows.Forms.PictureBox();
            this.pResult = new System.Windows.Forms.Panel();
            this.lblWebpageSize = new System.Windows.Forms.Label();
            this.nudWebpageWidth = new System.Windows.Forms.NumericUpDown();
            this.nudWebpageHeight = new System.Windows.Forms.NumericUpDown();
            this.lblWebpageX = new System.Windows.Forms.Label();
            this.lblCaptureDelay = new System.Windows.Forms.Label();
            this.nudCaptureDelay = new System.Windows.Forms.NumericUpDown();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).BeginInit();
            this.pResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebpageWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebpageHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // txtURL
            // 
            resources.ApplyResources(this.txtURL, "txtURL");
            this.txtURL.Name = "txtURL";
            this.txtURL.TextChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // lblURL
            // 
            resources.ApplyResources(this.lblURL, "lblURL");
            this.lblURL.Name = "lblURL";
            // 
            // btnCapture
            // 
            resources.ApplyResources(this.btnCapture, "btnCapture");
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // pbResult
            // 
            this.pbResult.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pbResult, "pbResult");
            this.pbResult.Name = "pbResult";
            this.pbResult.TabStop = false;
            // 
            // pResult
            // 
            resources.ApplyResources(this.pResult, "pResult");
            this.pResult.BackColor = System.Drawing.Color.White;
            this.pResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResult.Controls.Add(this.pbResult);
            this.pResult.Name = "pResult";
            // 
            // lblWebpageSize
            // 
            resources.ApplyResources(this.lblWebpageSize, "lblWebpageSize");
            this.lblWebpageSize.Name = "lblWebpageSize";
            // 
            // nudWebpageWidth
            // 
            resources.ApplyResources(this.nudWebpageWidth, "nudWebpageWidth");
            this.nudWebpageWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWebpageWidth.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWebpageWidth.Name = "nudWebpageWidth";
            this.nudWebpageWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            this.nudWebpageWidth.ValueChanged += new System.EventHandler(this.nudWebpageWidth_ValueChanged);
            // 
            // nudWebpageHeight
            // 
            resources.ApplyResources(this.nudWebpageHeight, "nudWebpageHeight");
            this.nudWebpageHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWebpageHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWebpageHeight.Name = "nudWebpageHeight";
            this.nudWebpageHeight.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            this.nudWebpageHeight.ValueChanged += new System.EventHandler(this.nudWebpageHeight_ValueChanged);
            // 
            // lblWebpageX
            // 
            resources.ApplyResources(this.lblWebpageX, "lblWebpageX");
            this.lblWebpageX.Name = "lblWebpageX";
            // 
            // lblCaptureDelay
            // 
            resources.ApplyResources(this.lblCaptureDelay, "lblCaptureDelay");
            this.lblCaptureDelay.Name = "lblCaptureDelay";
            // 
            // nudCaptureDelay
            // 
            this.nudCaptureDelay.DecimalPlaces = 1;
            this.nudCaptureDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudCaptureDelay, "nudCaptureDelay");
            this.nudCaptureDelay.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCaptureDelay.Name = "nudCaptureDelay";
            this.nudCaptureDelay.ValueChanged += new System.EventHandler(this.nudCaptureDelay_ValueChanged);
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnUpload
            // 
            resources.ApplyResources(this.btnUpload, "btnUpload");
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // WebpageCaptureForm
            // 
            this.AcceptButton = this.btnCapture;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.nudCaptureDelay);
            this.Controls.Add(this.lblCaptureDelay);
            this.Controls.Add(this.nudWebpageHeight);
            this.Controls.Add(this.lblWebpageX);
            this.Controls.Add(this.nudWebpageWidth);
            this.Controls.Add(this.lblWebpageSize);
            this.Controls.Add(this.pResult);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.lblURL);
            this.Controls.Add(this.txtURL);
            this.Name = "WebpageCaptureForm";
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).EndInit();
            this.pResult.ResumeLayout(false);
            this.pResult.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebpageWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebpageHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.PictureBox pbResult;
        private System.Windows.Forms.Panel pResult;
        private System.Windows.Forms.Label lblWebpageSize;
        private System.Windows.Forms.NumericUpDown nudWebpageWidth;
        private System.Windows.Forms.NumericUpDown nudWebpageHeight;
        private System.Windows.Forms.Label lblWebpageX;
        private System.Windows.Forms.Label lblCaptureDelay;
        private System.Windows.Forms.NumericUpDown nudCaptureDelay;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnUpload;
    }
}