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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (hashCheck != null)
                {
                    hashCheck.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashCheckForm));
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnFilePathBrowse = new System.Windows.Forms.Button();
            this.lblHashType = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblProgressPercentage = new System.Windows.Forms.Label();
            this.btnStartHashCheck = new System.Windows.Forms.Button();
            this.cbHashType = new System.Windows.Forms.ComboBox();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.lblFile = new System.Windows.Forms.Label();
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
            // lblProgress
            // 
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.Name = "lblProgress";
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
            // HashCheckForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.cbHashType);
            this.Controls.Add(this.btnStartHashCheck);
            this.Controls.Add(this.lblProgressPercentage);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblHashType);
            this.Controls.Add(this.btnFilePathBrowse);
            this.Controls.Add(this.txtFilePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HashCheckForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnFilePathBrowse;
        private System.Windows.Forms.Label lblHashType;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblProgressPercentage;
        private System.Windows.Forms.Button btnStartHashCheck;
        private System.Windows.Forms.ComboBox cbHashType;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label lblFile;
    }
}