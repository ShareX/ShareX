namespace ShareX.HelpersLib
{
    partial class HashCheckForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashCheckForm));
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnFilePathBrowse = new System.Windows.Forms.Button();
            this.lblHashType = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblProgressPercentage = new System.Windows.Forms.Label();
            this.btnStartHashCheck = new System.Windows.Forms.Button();
            this.cbHashType = new System.Windows.Forms.ComboBox();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.lblFile = new System.Windows.Forms.Label();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpFileHashCheck = new System.Windows.Forms.TabPage();
            this.tpTextConversions = new System.Windows.Forms.TabPage();
            this.btnHashCheckCopyAll = new System.Windows.Forms.Button();
            this.txtHashCheckHash = new System.Windows.Forms.TextBox();
            this.lblHashCheckHash = new System.Windows.Forms.Label();
            this.btnHashCheckDecodeBase64 = new System.Windows.Forms.Button();
            this.txtHashCheckBase64 = new System.Windows.Forms.TextBox();
            this.lblHashCheckBase64 = new System.Windows.Forms.Label();
            this.btnHashCheckDecodeASCII = new System.Windows.Forms.Button();
            this.txtHashCheckASCII = new System.Windows.Forms.TextBox();
            this.lblHashCheckASCII = new System.Windows.Forms.Label();
            this.btnHashCheckDecodeHex = new System.Windows.Forms.Button();
            this.txtHashCheckHex = new System.Windows.Forms.TextBox();
            this.lblHashCheckHex = new System.Windows.Forms.Label();
            this.btnHashCheckDecodeBinary = new System.Windows.Forms.Button();
            this.txtHashCheckBinary = new System.Windows.Forms.TextBox();
            this.lblHashCheckBinary = new System.Windows.Forms.Label();
            this.btnHashCheckEncodeText = new System.Windows.Forms.Button();
            this.txtHashCheckText = new System.Windows.Forms.TextBox();
            this.lblHashCheckText = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tpFileHashCheck.SuspendLayout();
            this.tpTextConversions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.AllowDrop = true;
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtFilePath, "txtFilePath");
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtFilePath_DragDrop);
            this.txtFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtFilePath_DragEnter);
            // 
            // btnFilePathBrowse
            // 
            resources.ApplyResources(this.btnFilePathBrowse, "btnFilePathBrowse");
            this.btnFilePathBrowse.Name = "btnFilePathBrowse";
            this.btnFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnFilePathBrowse.Click += new System.EventHandler(this.btnFilePathBrowse_Click);
            // 
            // lblHashType
            // 
            resources.ApplyResources(this.lblHashType, "lblHashType");
            this.lblHashType.Name = "lblHashType";
            // 
            // lblResult
            // 
            resources.ApplyResources(this.lblResult, "lblResult");
            this.lblResult.Name = "lblResult";
            // 
            // lblTarget
            // 
            resources.ApplyResources(this.lblTarget, "lblTarget");
            this.lblTarget.Name = "lblTarget";
            // 
            // lblProgressPercentage
            // 
            resources.ApplyResources(this.lblProgressPercentage, "lblProgressPercentage");
            this.lblProgressPercentage.Name = "lblProgressPercentage";
            // 
            // btnStartHashCheck
            // 
            resources.ApplyResources(this.btnStartHashCheck, "btnStartHashCheck");
            this.btnStartHashCheck.Name = "btnStartHashCheck";
            this.btnStartHashCheck.UseVisualStyleBackColor = true;
            this.btnStartHashCheck.Click += new System.EventHandler(this.btnStartHashCheck_Click);
            // 
            // cbHashType
            // 
            this.cbHashType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHashType.FormattingEnabled = true;
            resources.ApplyResources(this.cbHashType, "cbHashType");
            this.cbHashType.Name = "cbHashType";
            // 
            // pbProgress
            // 
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.Name = "pbProgress";
            // 
            // txtResult
            // 
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtResult, "txtResult");
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.TextChanged += new System.EventHandler(this.txtResult_TextChanged);
            // 
            // txtTarget
            // 
            this.txtTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtTarget, "txtTarget");
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.TextChanged += new System.EventHandler(this.txtResult_TextChanged);
            // 
            // lblFile
            // 
            resources.ApplyResources(this.lblFile, "lblFile");
            this.lblFile.Name = "lblFile";
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpFileHashCheck);
            this.tcMain.Controls.Add(this.tpTextConversions);
            resources.ApplyResources(this.tcMain, "tcMain");
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            // 
            // tpFileHashCheck
            // 
            this.tpFileHashCheck.Controls.Add(this.lblFile);
            this.tpFileHashCheck.Controls.Add(this.txtFilePath);
            this.tpFileHashCheck.Controls.Add(this.txtTarget);
            this.tpFileHashCheck.Controls.Add(this.btnFilePathBrowse);
            this.tpFileHashCheck.Controls.Add(this.txtResult);
            this.tpFileHashCheck.Controls.Add(this.lblHashType);
            this.tpFileHashCheck.Controls.Add(this.pbProgress);
            this.tpFileHashCheck.Controls.Add(this.cbHashType);
            this.tpFileHashCheck.Controls.Add(this.lblResult);
            this.tpFileHashCheck.Controls.Add(this.btnStartHashCheck);
            this.tpFileHashCheck.Controls.Add(this.lblTarget);
            this.tpFileHashCheck.Controls.Add(this.lblProgressPercentage);
            resources.ApplyResources(this.tpFileHashCheck, "tpFileHashCheck");
            this.tpFileHashCheck.Name = "tpFileHashCheck";
            this.tpFileHashCheck.UseVisualStyleBackColor = true;
            // 
            // tpTextConversions
            // 
            this.tpTextConversions.Controls.Add(this.btnHashCheckCopyAll);
            this.tpTextConversions.Controls.Add(this.txtHashCheckHash);
            this.tpTextConversions.Controls.Add(this.lblHashCheckHash);
            this.tpTextConversions.Controls.Add(this.btnHashCheckDecodeBase64);
            this.tpTextConversions.Controls.Add(this.txtHashCheckBase64);
            this.tpTextConversions.Controls.Add(this.lblHashCheckBase64);
            this.tpTextConversions.Controls.Add(this.btnHashCheckDecodeASCII);
            this.tpTextConversions.Controls.Add(this.txtHashCheckASCII);
            this.tpTextConversions.Controls.Add(this.lblHashCheckASCII);
            this.tpTextConversions.Controls.Add(this.btnHashCheckDecodeHex);
            this.tpTextConversions.Controls.Add(this.txtHashCheckHex);
            this.tpTextConversions.Controls.Add(this.lblHashCheckHex);
            this.tpTextConversions.Controls.Add(this.btnHashCheckDecodeBinary);
            this.tpTextConversions.Controls.Add(this.txtHashCheckBinary);
            this.tpTextConversions.Controls.Add(this.lblHashCheckBinary);
            this.tpTextConversions.Controls.Add(this.btnHashCheckEncodeText);
            this.tpTextConversions.Controls.Add(this.txtHashCheckText);
            this.tpTextConversions.Controls.Add(this.lblHashCheckText);
            resources.ApplyResources(this.tpTextConversions, "tpTextConversions");
            this.tpTextConversions.Name = "tpTextConversions";
            this.tpTextConversions.UseVisualStyleBackColor = true;
            // 
            // btnHashCheckCopyAll
            // 
            resources.ApplyResources(this.btnHashCheckCopyAll, "btnHashCheckCopyAll");
            this.btnHashCheckCopyAll.Name = "btnHashCheckCopyAll";
            this.btnHashCheckCopyAll.UseVisualStyleBackColor = true;
            this.btnHashCheckCopyAll.Click += new System.EventHandler(this.btnHashCheckCopyAll_Click);
            // 
            // txtHashCheckHash
            // 
            resources.ApplyResources(this.txtHashCheckHash, "txtHashCheckHash");
            this.txtHashCheckHash.Name = "txtHashCheckHash";
            // 
            // lblHashCheckHash
            // 
            resources.ApplyResources(this.lblHashCheckHash, "lblHashCheckHash");
            this.lblHashCheckHash.Name = "lblHashCheckHash";
            // 
            // btnHashCheckDecodeBase64
            // 
            resources.ApplyResources(this.btnHashCheckDecodeBase64, "btnHashCheckDecodeBase64");
            this.btnHashCheckDecodeBase64.Name = "btnHashCheckDecodeBase64";
            this.btnHashCheckDecodeBase64.UseVisualStyleBackColor = true;
            this.btnHashCheckDecodeBase64.Click += new System.EventHandler(this.btnHashCheckDecodeBase64_Click);
            // 
            // txtHashCheckBase64
            // 
            resources.ApplyResources(this.txtHashCheckBase64, "txtHashCheckBase64");
            this.txtHashCheckBase64.Name = "txtHashCheckBase64";
            // 
            // lblHashCheckBase64
            // 
            resources.ApplyResources(this.lblHashCheckBase64, "lblHashCheckBase64");
            this.lblHashCheckBase64.Name = "lblHashCheckBase64";
            // 
            // btnHashCheckDecodeASCII
            // 
            resources.ApplyResources(this.btnHashCheckDecodeASCII, "btnHashCheckDecodeASCII");
            this.btnHashCheckDecodeASCII.Name = "btnHashCheckDecodeASCII";
            this.btnHashCheckDecodeASCII.UseVisualStyleBackColor = true;
            this.btnHashCheckDecodeASCII.Click += new System.EventHandler(this.btnHashCheckDecodeASCII_Click);
            // 
            // txtHashCheckASCII
            // 
            resources.ApplyResources(this.txtHashCheckASCII, "txtHashCheckASCII");
            this.txtHashCheckASCII.Name = "txtHashCheckASCII";
            // 
            // lblHashCheckASCII
            // 
            resources.ApplyResources(this.lblHashCheckASCII, "lblHashCheckASCII");
            this.lblHashCheckASCII.Name = "lblHashCheckASCII";
            // 
            // btnHashCheckDecodeHex
            // 
            resources.ApplyResources(this.btnHashCheckDecodeHex, "btnHashCheckDecodeHex");
            this.btnHashCheckDecodeHex.Name = "btnHashCheckDecodeHex";
            this.btnHashCheckDecodeHex.UseVisualStyleBackColor = true;
            this.btnHashCheckDecodeHex.Click += new System.EventHandler(this.btnHashCheckDecodeHex_Click);
            // 
            // txtHashCheckHex
            // 
            resources.ApplyResources(this.txtHashCheckHex, "txtHashCheckHex");
            this.txtHashCheckHex.Name = "txtHashCheckHex";
            // 
            // lblHashCheckHex
            // 
            resources.ApplyResources(this.lblHashCheckHex, "lblHashCheckHex");
            this.lblHashCheckHex.Name = "lblHashCheckHex";
            // 
            // btnHashCheckDecodeBinary
            // 
            resources.ApplyResources(this.btnHashCheckDecodeBinary, "btnHashCheckDecodeBinary");
            this.btnHashCheckDecodeBinary.Name = "btnHashCheckDecodeBinary";
            this.btnHashCheckDecodeBinary.UseVisualStyleBackColor = true;
            this.btnHashCheckDecodeBinary.Click += new System.EventHandler(this.btnHashCheckDecodeBinary_Click);
            // 
            // txtHashCheckBinary
            // 
            resources.ApplyResources(this.txtHashCheckBinary, "txtHashCheckBinary");
            this.txtHashCheckBinary.Name = "txtHashCheckBinary";
            // 
            // lblHashCheckBinary
            // 
            resources.ApplyResources(this.lblHashCheckBinary, "lblHashCheckBinary");
            this.lblHashCheckBinary.Name = "lblHashCheckBinary";
            // 
            // btnHashCheckEncodeText
            // 
            resources.ApplyResources(this.btnHashCheckEncodeText, "btnHashCheckEncodeText");
            this.btnHashCheckEncodeText.Name = "btnHashCheckEncodeText";
            this.btnHashCheckEncodeText.UseVisualStyleBackColor = true;
            this.btnHashCheckEncodeText.Click += new System.EventHandler(this.btnHashCheckEncodeText_Click);
            // 
            // txtHashCheckText
            // 
            resources.ApplyResources(this.txtHashCheckText, "txtHashCheckText");
            this.txtHashCheckText.Name = "txtHashCheckText";
            // 
            // lblHashCheckText
            // 
            resources.ApplyResources(this.lblHashCheckText, "lblHashCheckText");
            this.lblHashCheckText.Name = "lblHashCheckText";
            // 
            // HashCheckForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.tcMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HashCheckForm";
            this.tcMain.ResumeLayout(false);
            this.tpFileHashCheck.ResumeLayout(false);
            this.tpFileHashCheck.PerformLayout();
            this.tpTextConversions.ResumeLayout(false);
            this.tpTextConversions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnFilePathBrowse;
        private System.Windows.Forms.Label lblHashType;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblProgressPercentage;
        private System.Windows.Forms.Button btnStartHashCheck;
        private System.Windows.Forms.ComboBox cbHashType;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpFileHashCheck;
        private System.Windows.Forms.TabPage tpTextConversions;
        private System.Windows.Forms.Button btnHashCheckEncodeText;
        private System.Windows.Forms.TextBox txtHashCheckText;
        private System.Windows.Forms.Label lblHashCheckText;
        private System.Windows.Forms.Button btnHashCheckCopyAll;
        private System.Windows.Forms.TextBox txtHashCheckHash;
        private System.Windows.Forms.Label lblHashCheckHash;
        private System.Windows.Forms.Button btnHashCheckDecodeBase64;
        private System.Windows.Forms.TextBox txtHashCheckBase64;
        private System.Windows.Forms.Label lblHashCheckBase64;
        private System.Windows.Forms.Button btnHashCheckDecodeASCII;
        private System.Windows.Forms.TextBox txtHashCheckASCII;
        private System.Windows.Forms.Label lblHashCheckASCII;
        private System.Windows.Forms.Button btnHashCheckDecodeHex;
        private System.Windows.Forms.TextBox txtHashCheckHex;
        private System.Windows.Forms.Label lblHashCheckHex;
        private System.Windows.Forms.Button btnHashCheckDecodeBinary;
        private System.Windows.Forms.TextBox txtHashCheckBinary;
        private System.Windows.Forms.Label lblHashCheckBinary;
    }
}