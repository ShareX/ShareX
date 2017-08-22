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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QRCodeForm));
            this.cmsQR = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.txtQRCode = new System.Windows.Forms.TextBox();
            this.pbQRCode = new System.Windows.Forms.PictureBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpEncode = new System.Windows.Forms.TabPage();
            this.tpDecode = new System.Windows.Forms.TabPage();
            this.txtDecodeResult = new System.Windows.Forms.TextBox();
            this.lblDecodeResult = new System.Windows.Forms.Label();
            this.btnDecodeFromScreen = new System.Windows.Forms.Button();
            this.btnDecodeFromFile = new System.Windows.Forms.Button();
            this.cmsQR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbQRCode)).BeginInit();
            this.tcMain.SuspendLayout();
            this.tpEncode.SuspendLayout();
            this.tpDecode.SuspendLayout();
            this.SuspendLayout();
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
            resources.ApplyResources(this.txtQRCode, "txtQRCode");
            this.txtQRCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQRCode.Name = "txtQRCode";
            this.txtQRCode.TextChanged += new System.EventHandler(this.txtQRCode_TextChanged);
            // 
            // pbQRCode
            // 
            resources.ApplyResources(this.pbQRCode, "pbQRCode");
            this.pbQRCode.ContextMenuStrip = this.cmsQR;
            this.pbQRCode.Name = "pbQRCode";
            this.pbQRCode.TabStop = false;
            this.pbQRCode.Click += new System.EventHandler(this.pbQRCode_Click);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpEncode);
            this.tcMain.Controls.Add(this.tpDecode);
            resources.ApplyResources(this.tcMain, "tcMain");
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            // 
            // tpEncode
            // 
            this.tpEncode.Controls.Add(this.txtQRCode);
            this.tpEncode.Controls.Add(this.pbQRCode);
            resources.ApplyResources(this.tpEncode, "tpEncode");
            this.tpEncode.Name = "tpEncode";
            this.tpEncode.UseVisualStyleBackColor = true;
            // 
            // tpDecode
            // 
            this.tpDecode.Controls.Add(this.btnDecodeFromFile);
            this.tpDecode.Controls.Add(this.txtDecodeResult);
            this.tpDecode.Controls.Add(this.lblDecodeResult);
            this.tpDecode.Controls.Add(this.btnDecodeFromScreen);
            resources.ApplyResources(this.tpDecode, "tpDecode");
            this.tpDecode.Name = "tpDecode";
            this.tpDecode.UseVisualStyleBackColor = true;
            // 
            // txtDecodeResult
            // 
            resources.ApplyResources(this.txtDecodeResult, "txtDecodeResult");
            this.txtDecodeResult.Name = "txtDecodeResult";
            // 
            // lblDecodeResult
            // 
            resources.ApplyResources(this.lblDecodeResult, "lblDecodeResult");
            this.lblDecodeResult.Name = "lblDecodeResult";
            // 
            // btnDecodeFromScreen
            // 
            resources.ApplyResources(this.btnDecodeFromScreen, "btnDecodeFromScreen");
            this.btnDecodeFromScreen.Name = "btnDecodeFromScreen";
            this.btnDecodeFromScreen.UseVisualStyleBackColor = true;
            this.btnDecodeFromScreen.Click += new System.EventHandler(this.btnDecodeFromScreen_Click);
            // 
            // btnDecodeFromFile
            // 
            resources.ApplyResources(this.btnDecodeFromFile, "btnDecodeFromFile");
            this.btnDecodeFromFile.Name = "btnDecodeFromFile";
            this.btnDecodeFromFile.UseVisualStyleBackColor = true;
            this.btnDecodeFromFile.Click += new System.EventHandler(this.btnDecodeFromFile_Click);
            // 
            // QRCodeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcMain);
            this.Name = "QRCodeForm";
            this.Load += new System.EventHandler(this.QRCodeForm_Load);
            this.Resize += new System.EventHandler(this.QRCodeForm_Resize);
            this.cmsQR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbQRCode)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.tpEncode.ResumeLayout(false);
            this.tpEncode.PerformLayout();
            this.tpDecode.ResumeLayout(false);
            this.tpDecode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtQRCode;
        private System.Windows.Forms.ContextMenuStrip cmsQR;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
        private System.Windows.Forms.PictureBox pbQRCode;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpEncode;
        private System.Windows.Forms.TabPage tpDecode;
        private System.Windows.Forms.Button btnDecodeFromScreen;
        private System.Windows.Forms.TextBox txtDecodeResult;
        private System.Windows.Forms.Label lblDecodeResult;
        private System.Windows.Forms.Button btnDecodeFromFile;
    }
}