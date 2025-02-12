namespace ShareX.HelpersLib
{
    partial class HashCheckerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashCheckerForm));
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnFilePathBrowse = new System.Windows.Forms.Button();
            this.lblHashType = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.btnStartHashCheck = new System.Windows.Forms.Button();
            this.cbHashType = new System.Windows.Forms.ComboBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.lblFilePath2 = new System.Windows.Forms.Label();
            this.txtFilePath2 = new System.Windows.Forms.TextBox();
            this.btnFilePathBrowse2 = new System.Windows.Forms.Button();
            this.cbCompareTwoFiles = new System.Windows.Forms.CheckBox();
            this.pbProgress = new ShareX.HelpersLib.BlackStyleProgressBar();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtFilePath, "txtFilePath");
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.TextChanged += new System.EventHandler(this.txtFilePath_TextChanged);
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
            this.txtTarget.TextChanged += new System.EventHandler(this.txtTarget_TextChanged);
            // 
            // lblFilePath
            // 
            resources.ApplyResources(this.lblFilePath, "lblFilePath");
            this.lblFilePath.Name = "lblFilePath";
            // 
            // lblFilePath2
            // 
            resources.ApplyResources(this.lblFilePath2, "lblFilePath2");
            this.lblFilePath2.Name = "lblFilePath2";
            // 
            // txtFilePath2
            // 
            this.txtFilePath2.AllowDrop = true;
            this.txtFilePath2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtFilePath2, "txtFilePath2");
            this.txtFilePath2.Name = "txtFilePath2";
            this.txtFilePath2.TextChanged += new System.EventHandler(this.txtFilePath2_TextChanged);
            this.txtFilePath2.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtFilePath2_DragDrop);
            this.txtFilePath2.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtFilePath2_DragEnter);
            // 
            // btnFilePathBrowse2
            // 
            resources.ApplyResources(this.btnFilePathBrowse2, "btnFilePathBrowse2");
            this.btnFilePathBrowse2.Name = "btnFilePathBrowse2";
            this.btnFilePathBrowse2.UseVisualStyleBackColor = true;
            this.btnFilePathBrowse2.Click += new System.EventHandler(this.btnFilePathBrowse2_Click);
            // 
            // cbCompareTwoFiles
            // 
            resources.ApplyResources(this.cbCompareTwoFiles, "cbCompareTwoFiles");
            this.cbCompareTwoFiles.Name = "cbCompareTwoFiles";
            this.cbCompareTwoFiles.UseVisualStyleBackColor = true;
            this.cbCompareTwoFiles.CheckedChanged += new System.EventHandler(this.cbCompareTwoFiles_CheckedChanged);
            // 
            // pbProgress
            // 
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.ShowPercentageText = true;
            // 
            // HashCheckerForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lblFilePath2);
            this.Controls.Add(this.txtFilePath2);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.btnFilePathBrowse2);
            this.Controls.Add(this.cbCompareTwoFiles);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.btnStartHashCheck);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.cbHashType);
            this.Controls.Add(this.btnFilePathBrowse);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblHashType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HashCheckerForm";
            this.Shown += new System.EventHandler(this.HashCheckerForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HashCheckerForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HashCheckerForm_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnFilePathBrowse;
        private System.Windows.Forms.Label lblHashType;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Button btnStartHashCheck;
        private System.Windows.Forms.ComboBox cbHashType;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label lblFilePath2;
        private System.Windows.Forms.TextBox txtFilePath2;
        private System.Windows.Forms.Button btnFilePathBrowse2;
        private System.Windows.Forms.CheckBox cbCompareTwoFiles;
        private BlackStyleProgressBar pbProgress;
    }
}