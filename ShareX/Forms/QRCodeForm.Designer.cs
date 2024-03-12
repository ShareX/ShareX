namespace ShareX
{
    partial class QRCodeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QRCodeForm));
            this.txtText = new System.Windows.Forms.TextBox();
            this.lblQRCodeSizeHint = new System.Windows.Forms.Label();
            this.lblQRCodeSize = new System.Windows.Forms.Label();
            this.nudQRCodeSize = new System.Windows.Forms.NumericUpDown();
            this.pbQRCode = new ShareX.HelpersLib.MyPictureBox();
            this.lblQRCode = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.btnCopyImage = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.btnUploadImage = new System.Windows.Forms.Button();
            this.btnScanQRCodeFromScreen = new System.Windows.Forms.Button();
            this.btnScanQRCodeFromImageFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudQRCodeSize)).BeginInit();
            this.SuspendLayout();
            // 
            // txtText
            // 
            resources.ApplyResources(this.txtText, "txtText");
            this.txtText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtText.Name = "txtText";
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            // 
            // lblQRCodeSizeHint
            // 
            resources.ApplyResources(this.lblQRCodeSizeHint, "lblQRCodeSizeHint");
            this.lblQRCodeSizeHint.Name = "lblQRCodeSizeHint";
            // 
            // lblQRCodeSize
            // 
            resources.ApplyResources(this.lblQRCodeSize, "lblQRCodeSize");
            this.lblQRCodeSize.Name = "lblQRCodeSize";
            // 
            // nudQRCodeSize
            // 
            resources.ApplyResources(this.nudQRCodeSize, "nudQRCodeSize");
            this.nudQRCodeSize.Increment = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudQRCodeSize.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.nudQRCodeSize.Name = "nudQRCodeSize";
            this.nudQRCodeSize.ValueChanged += new System.EventHandler(this.nudQRCodeSize_ValueChanged);
            // 
            // pbQRCode
            // 
            resources.ApplyResources(this.pbQRCode, "pbQRCode");
            this.pbQRCode.BackColor = System.Drawing.SystemColors.Window;
            this.pbQRCode.FullscreenOnClick = true;
            this.pbQRCode.Name = "pbQRCode";
            this.pbQRCode.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            // 
            // lblQRCode
            // 
            resources.ApplyResources(this.lblQRCode, "lblQRCode");
            this.lblQRCode.Name = "lblQRCode";
            // 
            // lblText
            // 
            resources.ApplyResources(this.lblText, "lblText");
            this.lblText.Name = "lblText";
            // 
            // btnCopyImage
            // 
            resources.ApplyResources(this.btnCopyImage, "btnCopyImage");
            this.btnCopyImage.Name = "btnCopyImage";
            this.btnCopyImage.UseVisualStyleBackColor = true;
            this.btnCopyImage.Click += new System.EventHandler(this.btnCopyImage_Click);
            // 
            // btnSaveImage
            // 
            resources.ApplyResources(this.btnSaveImage, "btnSaveImage");
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // btnUploadImage
            // 
            resources.ApplyResources(this.btnUploadImage, "btnUploadImage");
            this.btnUploadImage.Name = "btnUploadImage";
            this.btnUploadImage.UseVisualStyleBackColor = true;
            this.btnUploadImage.Click += new System.EventHandler(this.btnUploadImage_Click);
            // 
            // btnScanQRCodeFromScreen
            // 
            resources.ApplyResources(this.btnScanQRCodeFromScreen, "btnScanQRCodeFromScreen");
            this.btnScanQRCodeFromScreen.Name = "btnScanQRCodeFromScreen";
            this.btnScanQRCodeFromScreen.UseVisualStyleBackColor = true;
            this.btnScanQRCodeFromScreen.Click += new System.EventHandler(this.btnScanQRCodeFromScreen_Click);
            // 
            // btnScanQRCodeFromImageFile
            // 
            resources.ApplyResources(this.btnScanQRCodeFromImageFile, "btnScanQRCodeFromImageFile");
            this.btnScanQRCodeFromImageFile.Name = "btnScanQRCodeFromImageFile";
            this.btnScanQRCodeFromImageFile.UseVisualStyleBackColor = true;
            this.btnScanQRCodeFromImageFile.Click += new System.EventHandler(this.btnScanQRCodeFromImageFile_Click);
            // 
            // QRCodeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnScanQRCodeFromImageFile);
            this.Controls.Add(this.btnScanQRCodeFromScreen);
            this.Controls.Add(this.btnUploadImage);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnCopyImage);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.lblQRCode);
            this.Controls.Add(this.pbQRCode);
            this.Controls.Add(this.lblQRCodeSizeHint);
            this.Controls.Add(this.lblQRCodeSize);
            this.Controls.Add(this.nudQRCodeSize);
            this.Controls.Add(this.txtText);
            this.Name = "QRCodeForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.QRCodeForm_Shown);
            this.Resize += new System.EventHandler(this.QRCodeForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.nudQRCodeSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.NumericUpDown nudQRCodeSize;
        private HelpersLib.MyPictureBox pbQRCode;
        private System.Windows.Forms.Label lblQRCodeSize;
        private System.Windows.Forms.Label lblQRCodeSizeHint;
        private System.Windows.Forms.Label lblQRCode;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Button btnCopyImage;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.Button btnUploadImage;
        private System.Windows.Forms.Button btnScanQRCodeFromScreen;
        private System.Windows.Forms.Button btnScanQRCodeFromImageFile;
    }
}