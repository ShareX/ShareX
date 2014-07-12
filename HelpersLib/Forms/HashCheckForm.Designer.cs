namespace HelpersLib
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
            this.txtFilePath.Location = new System.Drawing.Point(72, 12);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(280, 20);
            this.txtFilePath.TabIndex = 1;
            this.txtFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtFilePath_DragDrop);
            this.txtFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtFilePath_DragEnter);
            // 
            // btnFilePathBrowse
            // 
            this.btnFilePathBrowse.Location = new System.Drawing.Point(360, 10);
            this.btnFilePathBrowse.Name = "btnFilePathBrowse";
            this.btnFilePathBrowse.Size = new System.Drawing.Size(72, 24);
            this.btnFilePathBrowse.TabIndex = 2;
            this.btnFilePathBrowse.Text = "Browse...";
            this.btnFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnFilePathBrowse.Click += new System.EventHandler(this.btnFilePathBrowse_Click);
            // 
            // lblHashType
            // 
            this.lblHashType.AutoSize = true;
            this.lblHashType.Location = new System.Drawing.Point(8, 48);
            this.lblHashType.Name = "lblHashType";
            this.lblHashType.Size = new System.Drawing.Size(58, 13);
            this.lblHashType.TabIndex = 3;
            this.lblHashType.Text = "Hash type:";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(8, 80);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(51, 13);
            this.lblProgress.TabIndex = 6;
            this.lblProgress.Text = "Progress:";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(8, 112);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(40, 13);
            this.lblResult.TabIndex = 9;
            this.lblResult.Text = "Result:";
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(8, 144);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(41, 13);
            this.lblTarget.TabIndex = 11;
            this.lblTarget.Text = "Target:";
            // 
            // lblProgressPercentage
            // 
            this.lblProgressPercentage.AutoSize = true;
            this.lblProgressPercentage.Location = new System.Drawing.Point(392, 80);
            this.lblProgressPercentage.Name = "lblProgressPercentage";
            this.lblProgressPercentage.Size = new System.Drawing.Size(21, 13);
            this.lblProgressPercentage.TabIndex = 8;
            this.lblProgressPercentage.Text = "0%";
            // 
            // btnStartHashCheck
            // 
            this.btnStartHashCheck.Location = new System.Drawing.Point(168, 42);
            this.btnStartHashCheck.Name = "btnStartHashCheck";
            this.btnStartHashCheck.Size = new System.Drawing.Size(72, 24);
            this.btnStartHashCheck.TabIndex = 5;
            this.btnStartHashCheck.Text = "Start";
            this.btnStartHashCheck.UseVisualStyleBackColor = true;
            this.btnStartHashCheck.Click += new System.EventHandler(this.btnStartHashCheck_Click);
            // 
            // cbHashType
            // 
            this.cbHashType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHashType.FormattingEnabled = true;
            this.cbHashType.Location = new System.Drawing.Point(72, 44);
            this.cbHashType.Name = "cbHashType";
            this.cbHashType.Size = new System.Drawing.Size(88, 21);
            this.cbHashType.TabIndex = 4;
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(72, 74);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(312, 24);
            this.pbProgress.TabIndex = 7;
            // 
            // txtResult
            // 
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResult.Location = new System.Drawing.Point(72, 108);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(360, 20);
            this.txtResult.TabIndex = 10;
            this.txtResult.TextChanged += new System.EventHandler(this.txtResult_TextChanged);
            // 
            // txtTarget
            // 
            this.txtTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTarget.Location = new System.Drawing.Point(72, 140);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(360, 20);
            this.txtTarget.TabIndex = 12;
            this.txtTarget.TextChanged += new System.EventHandler(this.txtResult_TextChanged);
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(8, 16);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(50, 13);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "File path:";
            // 
            // HashCheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(442, 171);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hash check";
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