namespace ShareX
{
    partial class AIOptionsForm
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
            btnAPIKeyHelp = new System.Windows.Forms.Button();
            cbReasoningEffort = new System.Windows.Forms.ComboBox();
            lblReasoningEffort = new System.Windows.Forms.Label();
            txtAPIKey = new System.Windows.Forms.TextBox();
            lblAPIKey = new System.Windows.Forms.Label();
            cbModel = new System.Windows.Forms.ComboBox();
            lblModel = new System.Windows.Forms.Label();
            cbAutoStartRegion = new System.Windows.Forms.CheckBox();
            cbAutoStartAnalyze = new System.Windows.Forms.CheckBox();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            cbAutoCopyResult = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // btnAPIKeyHelp
            // 
            btnAPIKeyHelp.Image = Properties.Resources.question;
            btnAPIKeyHelp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            btnAPIKeyHelp.Location = new System.Drawing.Point(336, 88);
            btnAPIKeyHelp.Margin = new System.Windows.Forms.Padding(4);
            btnAPIKeyHelp.Name = "btnAPIKeyHelp";
            btnAPIKeyHelp.Size = new System.Drawing.Size(32, 32);
            btnAPIKeyHelp.TabIndex = 4;
            btnAPIKeyHelp.UseVisualStyleBackColor = true;
            btnAPIKeyHelp.Click += btnAPIKeyHelp_Click;
            // 
            // cbReasoningEffort
            // 
            cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbReasoningEffort.FormattingEnabled = true;
            cbReasoningEffort.Items.AddRange(new object[] { "minimal", "low", "medium", "high" });
            cbReasoningEffort.Location = new System.Drawing.Point(16, 152);
            cbReasoningEffort.Margin = new System.Windows.Forms.Padding(4);
            cbReasoningEffort.Name = "cbReasoningEffort";
            cbReasoningEffort.Size = new System.Drawing.Size(351, 24);
            cbReasoningEffort.TabIndex = 6;
            // 
            // lblReasoningEffort
            // 
            lblReasoningEffort.AutoSize = true;
            lblReasoningEffort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            lblReasoningEffort.Location = new System.Drawing.Point(16, 128);
            lblReasoningEffort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblReasoningEffort.Name = "lblReasoningEffort";
            lblReasoningEffort.Size = new System.Drawing.Size(108, 16);
            lblReasoningEffort.TabIndex = 5;
            lblReasoningEffort.Text = "Reasoning effort:";
            // 
            // txtAPIKey
            // 
            txtAPIKey.Location = new System.Drawing.Point(16, 96);
            txtAPIKey.Margin = new System.Windows.Forms.Padding(4);
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.Size = new System.Drawing.Size(312, 22);
            txtAPIKey.TabIndex = 3;
            txtAPIKey.UseSystemPasswordChar = true;
            // 
            // lblAPIKey
            // 
            lblAPIKey.AutoSize = true;
            lblAPIKey.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            lblAPIKey.Location = new System.Drawing.Point(16, 72);
            lblAPIKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAPIKey.Name = "lblAPIKey";
            lblAPIKey.Size = new System.Drawing.Size(56, 16);
            lblAPIKey.TabIndex = 2;
            lblAPIKey.Text = "API key:";
            // 
            // cbModel
            // 
            cbModel.FormattingEnabled = true;
            cbModel.Items.AddRange(new object[] { "gpt-5", "gpt-5-mini", "gpt-5-nano" });
            cbModel.Location = new System.Drawing.Point(16, 40);
            cbModel.Margin = new System.Windows.Forms.Padding(4);
            cbModel.Name = "cbModel";
            cbModel.Size = new System.Drawing.Size(351, 24);
            cbModel.TabIndex = 1;
            // 
            // lblModel
            // 
            lblModel.AutoSize = true;
            lblModel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            lblModel.Location = new System.Drawing.Point(16, 16);
            lblModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblModel.Name = "lblModel";
            lblModel.Size = new System.Drawing.Size(48, 16);
            lblModel.TabIndex = 0;
            lblModel.Text = "Model:";
            // 
            // cbAutoStartRegion
            // 
            cbAutoStartRegion.AutoSize = true;
            cbAutoStartRegion.Location = new System.Drawing.Point(16, 192);
            cbAutoStartRegion.Name = "cbAutoStartRegion";
            cbAutoStartRegion.Size = new System.Drawing.Size(122, 20);
            cbAutoStartRegion.TabIndex = 7;
            cbAutoStartRegion.Text = "Auto start region";
            cbAutoStartRegion.UseVisualStyleBackColor = true;
            // 
            // cbAutoStartAnalyze
            // 
            cbAutoStartAnalyze.AutoSize = true;
            cbAutoStartAnalyze.Location = new System.Drawing.Point(16, 224);
            cbAutoStartAnalyze.Name = "cbAutoStartAnalyze";
            cbAutoStartAnalyze.Size = new System.Drawing.Size(131, 20);
            cbAutoStartAnalyze.TabIndex = 8;
            cbAutoStartAnalyze.Text = "Auto start analyze";
            cbAutoStartAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(152, 288);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(104, 32);
            btnOK.TabIndex = 10;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(264, 288);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(104, 32);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // cbAutoCopyResult
            // 
            cbAutoCopyResult.AutoSize = true;
            cbAutoCopyResult.Location = new System.Drawing.Point(16, 256);
            cbAutoCopyResult.Name = "cbAutoCopyResult";
            cbAutoCopyResult.Size = new System.Drawing.Size(121, 20);
            cbAutoCopyResult.TabIndex = 9;
            cbAutoCopyResult.Text = "Auto copy result";
            cbAutoCopyResult.UseVisualStyleBackColor = true;
            // 
            // AIOptionsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(384, 336);
            Controls.Add(cbAutoCopyResult);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(cbAutoStartAnalyze);
            Controls.Add(cbAutoStartRegion);
            Controls.Add(btnAPIKeyHelp);
            Controls.Add(cbReasoningEffort);
            Controls.Add(lblReasoningEffort);
            Controls.Add(txtAPIKey);
            Controls.Add(lblAPIKey);
            Controls.Add(cbModel);
            Controls.Add(lblModel);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            Name = "AIOptionsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ShareX - AI options";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnAPIKeyHelp;
        private System.Windows.Forms.ComboBox cbReasoningEffort;
        private System.Windows.Forms.Label lblReasoningEffort;
        private System.Windows.Forms.TextBox txtAPIKey;
        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.ComboBox cbModel;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.CheckBox cbAutoStartRegion;
        private System.Windows.Forms.CheckBox cbAutoStartAnalyze;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbAutoCopyResult;
    }
}