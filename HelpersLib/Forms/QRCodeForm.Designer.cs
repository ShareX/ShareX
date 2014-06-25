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
            this.components = new System.ComponentModel.Container();
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
            this.qrMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qrMain.ContextMenuStrip = this.cmsQR;
            this.qrMain.ErrorCorrectLevel = Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M;
            this.qrMain.Location = new System.Drawing.Point(2, 26);
            this.qrMain.Name = "qrMain";
            this.qrMain.QuietZoneModule = Gma.QrCodeNet.Encoding.Windows.Render.QuietZoneModules.Two;
            this.qrMain.Size = new System.Drawing.Size(380, 334);
            this.qrMain.TabIndex = 1;
            this.qrMain.Click += new System.EventHandler(this.qrMain_Click);
            // 
            // cmsQR
            // 
            this.cmsQR.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopy,
            this.tsmiSaveAs});
            this.cmsQR.Name = "cmsQR";
            this.cmsQR.Size = new System.Drawing.Size(212, 48);
            this.cmsQR.Text = "QR";
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopy.Size = new System.Drawing.Size(211, 22);
            this.tsmiCopy.Text = "Copy to clipboard";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.Size = new System.Drawing.Size(211, 22);
            this.tsmiSaveAs.Text = "Save image as PNG...";
            this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
            // 
            // txtQRCode
            // 
            this.txtQRCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQRCode.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtQRCode.Location = new System.Drawing.Point(2, 2);
            this.txtQRCode.Name = "txtQRCode";
            this.txtQRCode.Size = new System.Drawing.Size(380, 20);
            this.txtQRCode.TabIndex = 0;
            this.txtQRCode.Visible = false;
            this.txtQRCode.TextChanged += new System.EventHandler(this.txtQRCode_TextChanged);
            // 
            // QRCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(384, 362);
            this.Controls.Add(this.txtQRCode);
            this.Controls.Add(this.qrMain);
            this.Name = "QRCodeForm";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - QR code";
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