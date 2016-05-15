namespace ShareX.UploadersLib
{
    partial class OCRSpaceForm
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
            this.cbLanguages = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.llAttribution = new System.Windows.Forms.LinkLabel();
            this.btnStartOCR = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // cbLanguages
            // 
            this.cbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguages.FormattingEnabled = true;
            this.cbLanguages.Location = new System.Drawing.Point(8, 24);
            this.cbLanguages.Name = "cbLanguages";
            this.cbLanguages.Size = new System.Drawing.Size(152, 21);
            this.cbLanguages.TabIndex = 0;
            this.cbLanguages.SelectedIndexChanged += new System.EventHandler(this.cbLanguages_SelectedIndexChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(5, 8);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(58, 13);
            this.lblLanguage.TabIndex = 1;
            this.lblLanguage.Text = "Language:";
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(8, 72);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(544, 368);
            this.txtResult.TabIndex = 2;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(5, 56);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(40, 13);
            this.lblResult.TabIndex = 3;
            this.lblResult.Text = "Result:";
            // 
            // llAttribution
            // 
            this.llAttribution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llAttribution.AutoSize = true;
            this.llAttribution.Location = new System.Drawing.Point(440, 8);
            this.llAttribution.Name = "llAttribution";
            this.llAttribution.Size = new System.Drawing.Size(114, 13);
            this.llAttribution.TabIndex = 4;
            this.llAttribution.TabStop = true;
            this.llAttribution.Text = "Using OCR.Space API";
            this.llAttribution.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAttribution_LinkClicked);
            // 
            // btnStartOCR
            // 
            this.btnStartOCR.Location = new System.Drawing.Point(168, 22);
            this.btnStartOCR.Name = "btnStartOCR";
            this.btnStartOCR.Size = new System.Drawing.Size(136, 24);
            this.btnStartOCR.TabIndex = 5;
            this.btnStartOCR.Text = "Start OCR";
            this.btnStartOCR.UseVisualStyleBackColor = true;
            this.btnStartOCR.Click += new System.EventHandler(this.btnStartOCR_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(168, 22);
            this.pbProgress.MarqueeAnimationSpeed = 50;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(136, 24);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbProgress.TabIndex = 6;
            this.pbProgress.Visible = false;
            // 
            // OCRSpaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 448);
            this.Controls.Add(this.llAttribution);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.cbLanguages);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnStartOCR);
            this.Name = "OCRSpaceForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Optical character recognition";
            this.Shown += new System.EventHandler(this.OCRSpaceResultForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbLanguages;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.LinkLabel llAttribution;
        private System.Windows.Forms.Button btnStartOCR;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}