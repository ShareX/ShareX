namespace ShareX
{
    partial class AIForm
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
            lblModel = new System.Windows.Forms.Label();
            cbModel = new System.Windows.Forms.ComboBox();
            lblInput = new System.Windows.Forms.Label();
            lblImage = new System.Windows.Forms.Label();
            txtImage = new System.Windows.Forms.TextBox();
            btnAnalyze = new System.Windows.Forms.Button();
            lblAPIKey = new System.Windows.Forms.Label();
            txtAPIKey = new System.Windows.Forms.TextBox();
            lblResult = new System.Windows.Forms.Label();
            txtResult = new System.Windows.Forms.TextBox();
            btnImageBrowse = new System.Windows.Forms.Button();
            pbImage = new ShareX.HelpersLib.MyPictureBox();
            cbInput = new System.Windows.Forms.ComboBox();
            lblReasoningEffort = new System.Windows.Forms.Label();
            cbReasoningEffort = new System.Windows.Forms.ComboBox();
            lblTimer = new System.Windows.Forms.Label();
            btnCapture = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblModel
            // 
            lblModel.AutoSize = true;
            lblModel.Location = new System.Drawing.Point(13, 16);
            lblModel.Name = "lblModel";
            lblModel.Size = new System.Drawing.Size(48, 16);
            lblModel.TabIndex = 0;
            lblModel.Text = "Model:";
            // 
            // cbModel
            // 
            cbModel.FormattingEnabled = true;
            cbModel.Items.AddRange(new object[] { "gpt-5", "gpt-5-mini", "gpt-5-nano" });
            cbModel.Location = new System.Drawing.Point(16, 40);
            cbModel.Name = "cbModel";
            cbModel.Size = new System.Drawing.Size(264, 24);
            cbModel.TabIndex = 1;
            cbModel.TextChanged += cbModel_TextChanged;
            // 
            // lblInput
            // 
            lblInput.AutoSize = true;
            lblInput.Location = new System.Drawing.Point(13, 184);
            lblInput.Name = "lblInput";
            lblInput.Size = new System.Drawing.Size(53, 16);
            lblInput.TabIndex = 6;
            lblInput.Text = "Prompt:";
            // 
            // lblImage
            // 
            lblImage.AutoSize = true;
            lblImage.Location = new System.Drawing.Point(13, 240);
            lblImage.Name = "lblImage";
            lblImage.Size = new System.Drawing.Size(77, 16);
            lblImage.TabIndex = 8;
            lblImage.Text = "Image path:";
            // 
            // txtImage
            // 
            txtImage.Location = new System.Drawing.Point(16, 264);
            txtImage.Name = "txtImage";
            txtImage.Size = new System.Drawing.Size(224, 22);
            txtImage.TabIndex = 9;
            txtImage.TextChanged += txtImage_TextChanged;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Enabled = false;
            btnAnalyze.Location = new System.Drawing.Point(16, 568);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new System.Drawing.Size(224, 32);
            btnAnalyze.TabIndex = 12;
            btnAnalyze.Text = "Analyze image";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // lblAPIKey
            // 
            lblAPIKey.AutoSize = true;
            lblAPIKey.Location = new System.Drawing.Point(13, 72);
            lblAPIKey.Name = "lblAPIKey";
            lblAPIKey.Size = new System.Drawing.Size(56, 16);
            lblAPIKey.TabIndex = 2;
            lblAPIKey.Text = "API key:";
            // 
            // txtAPIKey
            // 
            txtAPIKey.Location = new System.Drawing.Point(16, 96);
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.Size = new System.Drawing.Size(264, 22);
            txtAPIKey.TabIndex = 3;
            txtAPIKey.UseSystemPasswordChar = true;
            txtAPIKey.TextChanged += txtAPIKey_TextChanged;
            // 
            // lblResult
            // 
            lblResult.AutoSize = true;
            lblResult.Location = new System.Drawing.Point(301, 16);
            lblResult.Name = "lblResult";
            lblResult.Size = new System.Drawing.Size(48, 16);
            lblResult.TabIndex = 13;
            lblResult.Text = "Result:";
            // 
            // txtResult
            // 
            txtResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtResult.Location = new System.Drawing.Point(304, 40);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtResult.Size = new System.Drawing.Size(608, 560);
            txtResult.TabIndex = 14;
            // 
            // btnImageBrowse
            // 
            btnImageBrowse.Location = new System.Drawing.Point(248, 263);
            btnImageBrowse.Name = "btnImageBrowse";
            btnImageBrowse.Size = new System.Drawing.Size(32, 24);
            btnImageBrowse.TabIndex = 10;
            btnImageBrowse.Text = "...";
            btnImageBrowse.UseVisualStyleBackColor = true;
            btnImageBrowse.Click += btnImageBrowse_Click;
            // 
            // pbImage
            // 
            pbImage.BackColor = System.Drawing.SystemColors.Window;
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.DrawCheckeredBackground = true;
            pbImage.FullscreenOnClick = true;
            pbImage.Location = new System.Drawing.Point(16, 296);
            pbImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbImage.Name = "pbImage";
            pbImage.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            pbImage.Size = new System.Drawing.Size(264, 264);
            pbImage.TabIndex = 11;
            // 
            // cbInput
            // 
            cbInput.FormattingEnabled = true;
            cbInput.Items.AddRange(new object[] { "What is in this image?", "Thoroughly describe this image.", "Transcribe the image's text, do not write anything else." });
            cbInput.Location = new System.Drawing.Point(16, 208);
            cbInput.Name = "cbInput";
            cbInput.Size = new System.Drawing.Size(264, 24);
            cbInput.TabIndex = 7;
            cbInput.TextChanged += cbInput_TextChanged;
            // 
            // lblReasoningEffort
            // 
            lblReasoningEffort.AutoSize = true;
            lblReasoningEffort.Location = new System.Drawing.Point(13, 128);
            lblReasoningEffort.Name = "lblReasoningEffort";
            lblReasoningEffort.Size = new System.Drawing.Size(108, 16);
            lblReasoningEffort.TabIndex = 4;
            lblReasoningEffort.Text = "Reasoning effort:";
            // 
            // cbReasoningEffort
            // 
            cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbReasoningEffort.FormattingEnabled = true;
            cbReasoningEffort.Items.AddRange(new object[] { "minimal", "low", "medium", "high" });
            cbReasoningEffort.Location = new System.Drawing.Point(16, 152);
            cbReasoningEffort.Name = "cbReasoningEffort";
            cbReasoningEffort.Size = new System.Drawing.Size(264, 24);
            cbReasoningEffort.TabIndex = 5;
            cbReasoningEffort.SelectedIndexChanged += cbReasoningEffort_SelectedIndexChanged;
            // 
            // lblTimer
            // 
            lblTimer.Location = new System.Drawing.Point(744, 12);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new System.Drawing.Size(168, 24);
            lblTimer.TabIndex = 15;
            lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCapture
            // 
            btnCapture.Image = Properties.Resources.camera;
            btnCapture.Location = new System.Drawing.Point(248, 568);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new System.Drawing.Size(32, 32);
            btnCapture.TabIndex = 16;
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // AIForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(928, 616);
            Controls.Add(btnCapture);
            Controls.Add(txtResult);
            Controls.Add(lblTimer);
            Controls.Add(cbReasoningEffort);
            Controls.Add(lblReasoningEffort);
            Controls.Add(cbInput);
            Controls.Add(pbImage);
            Controls.Add(btnImageBrowse);
            Controls.Add(lblResult);
            Controls.Add(txtAPIKey);
            Controls.Add(lblAPIKey);
            Controls.Add(btnAnalyze);
            Controls.Add(txtImage);
            Controls.Add(lblImage);
            Controls.Add(lblInput);
            Controls.Add(cbModel);
            Controls.Add(lblModel);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "AIForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ShareX - AI";
            Shown += AIForm_Shown;
            DragDrop += AIForm_DragDrop;
            DragEnter += AIForm_DragEnter;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.ComboBox cbModel;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.TextBox txtAPIKey;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnImageBrowse;
        private HelpersLib.MyPictureBox pbImage;
        private System.Windows.Forms.ComboBox cbInput;
        private System.Windows.Forms.Label lblReasoningEffort;
        private System.Windows.Forms.ComboBox cbReasoningEffort;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnCapture;
    }
}