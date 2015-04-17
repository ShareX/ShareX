namespace ShareX.HelpersLib
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QRCodeForm));
            this.qrMain = new Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeGraphicControl();
            this.cmsQR = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.txtQRCode = new System.Windows.Forms.TextBox();
            this.cmsQR.SuspendLayout();
            this.SuspendLayout();
            // 
            // qrMain
            // 
            resources.ApplyResources(this.qrMain, "qrMain");
            this.qrMain.ContextMenuStrip = this.cmsQR;
            this.qrMain.ErrorCorrectLevel = Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M;
            this.qrMain.Name = "qrMain";
            this.qrMain.QuietZoneModule = Gma.QrCodeNet.Encoding.Windows.Render.QuietZoneModules.Two;
            this.qrMain.Click += new System.EventHandler(this.qrMain_Click);
            // 
            // cmsQR
            // 
            this.cmsQR.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopy,
            this.tsmiSaveAs});
            this.cmsQR.Name = "cmsQR";
            this.cmsQR.ShowImageMargin = false;
            resources.ApplyResources(this.cmsQR, "cmsQR");
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            resources.ApplyResources(this.tsmiCopy, "tsmiCopy");
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            resources.ApplyResources(this.tsmiSaveAs, "tsmiSaveAs");
            this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
            // 
            // txtQRCode
            // 
            this.txtQRCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtQRCode, "txtQRCode");
            this.txtQRCode.Name = "txtQRCode";
            this.txtQRCode.TextChanged += new System.EventHandler(this.txtQRCode_TextChanged);
            // 
            // QRCodeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.txtQRCode);
            this.Controls.Add(this.qrMain);
            this.Name = "QRCodeForm";
            this.TopMost = true;
            this.Resize += new System.EventHandler(this.QRCodeForm_Resize);
            this.cmsQR.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeGraphicControl qrMain;
        private System.Windows.Forms.TextBox txtQRCode;
        private System.Windows.Forms.ContextMenuStrip cmsQR;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
    }
}