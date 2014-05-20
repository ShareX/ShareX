namespace HelpersLib
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
            this.qrMain = new Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeGraphicControl();
            this.SuspendLayout();
            // 
            // qrMain
            // 
            this.qrMain.Cursor = System.Windows.Forms.Cursors.Hand;
            this.qrMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.qrMain.ErrorCorrectLevel = Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M;
            this.qrMain.Location = new System.Drawing.Point(0, 0);
            this.qrMain.Name = "qrMain";
            this.qrMain.QuietZoneModule = Gma.QrCodeNet.Encoding.Windows.Render.QuietZoneModules.Two;
            this.qrMain.Size = new System.Drawing.Size(384, 362);
            this.qrMain.TabIndex = 0;
            this.qrMain.Click += new System.EventHandler(this.qrMain_Click);
            // 
            // QRCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(384, 362);
            this.Controls.Add(this.qrMain);
            this.Name = "QRCodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - QR code";
            this.TopMost = true;
            this.Resize += new System.EventHandler(this.QRCodeForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeGraphicControl qrMain;
    }
}