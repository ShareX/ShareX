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
            txtInput = new System.Windows.Forms.TextBox();
            lblImage = new System.Windows.Forms.Label();
            txtImage = new System.Windows.Forms.TextBox();
            btnAnalyze = new System.Windows.Forms.Button();
            lblAPIKey = new System.Windows.Forms.Label();
            txtAPIKey = new System.Windows.Forms.TextBox();
            lblResult = new System.Windows.Forms.Label();
            txtResult = new System.Windows.Forms.TextBox();
            btnImageBrowse = new System.Windows.Forms.Button();
            pbImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbImage).BeginInit();
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
            cbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbModel.FormattingEnabled = true;
            cbModel.Items.AddRange(new object[] { "gpt-5", "gpt-5-mini", "gpt-5-nano" });
            cbModel.Location = new System.Drawing.Point(16, 40);
            cbModel.Name = "cbModel";
            cbModel.Size = new System.Drawing.Size(264, 24);
            cbModel.TabIndex = 1;
            cbModel.SelectedIndexChanged += cbModel_SelectedIndexChanged;
            // 
            // lblInput
            // 
            lblInput.AutoSize = true;
            lblInput.Location = new System.Drawing.Point(13, 128);
            lblInput.Name = "lblInput";
            lblInput.Size = new System.Drawing.Size(53, 16);
            lblInput.TabIndex = 4;
            lblInput.Text = "Prompt:";
            // 
            // txtInput
            // 
            txtInput.Location = new System.Drawing.Point(16, 152);
            txtInput.Name = "txtInput";
            txtInput.Size = new System.Drawing.Size(264, 22);
            txtInput.TabIndex = 5;
            txtInput.TextChanged += txtInput_TextChanged;
            // 
            // lblImage
            // 
            lblImage.AutoSize = true;
            lblImage.Location = new System.Drawing.Point(13, 184);
            lblImage.Name = "lblImage";
            lblImage.Size = new System.Drawing.Size(77, 16);
            lblImage.TabIndex = 6;
            lblImage.Text = "Image path:";
            // 
            // txtImage
            // 
            txtImage.Location = new System.Drawing.Point(16, 208);
            txtImage.Name = "txtImage";
            txtImage.Size = new System.Drawing.Size(224, 22);
            txtImage.TabIndex = 7;
            txtImage.TextChanged += txtImage_TextChanged;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Enabled = false;
            btnAnalyze.Location = new System.Drawing.Point(16, 512);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new System.Drawing.Size(264, 32);
            btnAnalyze.TabIndex = 9;
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
            lblResult.TabIndex = 10;
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
            txtResult.Size = new System.Drawing.Size(608, 504);
            txtResult.TabIndex = 11;
            // 
            // btnImageBrowse
            // 
            btnImageBrowse.Location = new System.Drawing.Point(248, 207);
            btnImageBrowse.Name = "btnImageBrowse";
            btnImageBrowse.Size = new System.Drawing.Size(32, 24);
            btnImageBrowse.TabIndex = 8;
            btnImageBrowse.Text = "...";
            btnImageBrowse.UseVisualStyleBackColor = true;
            btnImageBrowse.Click += btnImageBrowse_Click;
            // 
            // pbImage
            // 
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.Location = new System.Drawing.Point(16, 240);
            pbImage.Name = "pbImage";
            pbImage.Size = new System.Drawing.Size(264, 264);
            pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbImage.TabIndex = 12;
            pbImage.TabStop = false;
            // 
            // AIForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(929, 561);
            Controls.Add(pbImage);
            Controls.Add(btnImageBrowse);
            Controls.Add(txtResult);
            Controls.Add(lblResult);
            Controls.Add(txtAPIKey);
            Controls.Add(lblAPIKey);
            Controls.Add(btnAnalyze);
            Controls.Add(txtImage);
            Controls.Add(lblImage);
            Controls.Add(txtInput);
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
            ((System.ComponentModel.ISupportInitialize)pbImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.ComboBox cbModel;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.TextBox txtAPIKey;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnImageBrowse;
        private System.Windows.Forms.PictureBox pbImage;
    }
}