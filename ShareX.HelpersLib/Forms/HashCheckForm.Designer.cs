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
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.btnFilePathBrowse = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblHashType = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.cbHashType = new System.Windows.Forms.ComboBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnStartHashCheck = new System.Windows.Forms.Button();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblProgressPercentage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFile
            // 
            resources.ApplyResources(this.lblFile, "lblFile");
            this.lblFile.Name = "lblFile";
            // 
            // txtFilePath
            // 
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtFilePath, "txtFilePath");
            this.txtFilePath.Name = "txtFilePath";
            // 
            // txtTarget
            // 
            this.txtTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtTarget, "txtTarget");
            this.txtTarget.Name = "txtTarget";
            // 
            // btnFilePathBrowse
            // 
            resources.ApplyResources(this.btnFilePathBrowse, "btnFilePathBrowse");
            this.btnFilePathBrowse.Name = "btnFilePathBrowse";
            this.btnFilePathBrowse.UseVisualStyleBackColor = true;
            // 
            // txtResult
            // 
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtResult, "txtResult");
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            // 
            // lblHashType
            // 
            resources.ApplyResources(this.lblHashType, "lblHashType");
            this.lblHashType.Name = "lblHashType";
            // 
            // pbProgress
            // 
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.Name = "pbProgress";
            // 
            // cbHashType
            // 
            this.cbHashType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHashType.FormattingEnabled = true;
            resources.ApplyResources(this.cbHashType, "cbHashType");
            this.cbHashType.Name = "cbHashType";
            // 
            // lblResult
            // 
            resources.ApplyResources(this.lblResult, "lblResult");
            this.lblResult.Name = "lblResult";
            // 
            // btnStartHashCheck
            // 
            resources.ApplyResources(this.btnStartHashCheck, "btnStartHashCheck");
            this.btnStartHashCheck.Name = "btnStartHashCheck";
            this.btnStartHashCheck.UseVisualStyleBackColor = true;
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
            // HashCheckForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.btnFilePathBrowse);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblHashType);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.cbHashType);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnStartHashCheck);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblProgressPercentage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HashCheckForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Button btnFilePathBrowse;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblHashType;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.ComboBox cbHashType;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnStartHashCheck;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblProgressPercentage;
    }
}