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
            components = new System.ComponentModel.Container();
            lblInput = new System.Windows.Forms.Label();
            txtInput = new System.Windows.Forms.TextBox();
            cmsPresets = new System.Windows.Forms.ContextMenuStrip(components);
            lblImage = new System.Windows.Forms.Label();
            txtImage = new System.Windows.Forms.TextBox();
            btnImageBrowse = new System.Windows.Forms.Button();
            pbImage = new ShareX.HelpersLib.MyPictureBox();
            btnAnalyze = new System.Windows.Forms.Button();
            btnCapture = new System.Windows.Forms.Button();
            lblResult = new System.Windows.Forms.Label();
            lblTimer = new System.Windows.Forms.Label();
            txtResult = new System.Windows.Forms.TextBox();
            btnResultCopy = new System.Windows.Forms.Button();
            btnOptions = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblInput
            // 
            lblInput.AutoSize = true;
            lblInput.Location = new System.Drawing.Point(16, 51);
            lblInput.Name = "lblInput";
            lblInput.Size = new System.Drawing.Size(53, 16);
            lblInput.TabIndex = 1;
            lblInput.Text = "Prompt:";
            // 
            // txtInput
            // 
            txtInput.ContextMenuStrip = cmsPresets;
            txtInput.Location = new System.Drawing.Point(16, 70);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtInput.Size = new System.Drawing.Size(264, 80);
            txtInput.TabIndex = 2;
            txtInput.TextChanged += txtInput_TextChanged;
            // 
            // cmsPresets
            // 
            cmsPresets.Name = "cmsPresets";
            cmsPresets.Size = new System.Drawing.Size(61, 4);
            // 
            // lblImage
            // 
            lblImage.AutoSize = true;
            lblImage.Location = new System.Drawing.Point(16, 153);
            lblImage.Name = "lblImage";
            lblImage.Size = new System.Drawing.Size(77, 16);
            lblImage.TabIndex = 3;
            lblImage.Text = "Image path:";
            // 
            // txtImage
            // 
            txtImage.Location = new System.Drawing.Point(16, 172);
            txtImage.Name = "txtImage";
            txtImage.Size = new System.Drawing.Size(224, 22);
            txtImage.TabIndex = 4;
            txtImage.TextChanged += txtImage_TextChanged;
            // 
            // btnImageBrowse
            // 
            btnImageBrowse.Image = Properties.Resources.folder_open_image;
            btnImageBrowse.Location = new System.Drawing.Point(248, 172);
            btnImageBrowse.Name = "btnImageBrowse";
            btnImageBrowse.Size = new System.Drawing.Size(32, 22);
            btnImageBrowse.TabIndex = 5;
            btnImageBrowse.UseVisualStyleBackColor = true;
            btnImageBrowse.Click += btnImageBrowse_Click;
            // 
            // pbImage
            // 
            pbImage.BackColor = System.Drawing.SystemColors.Window;
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.DrawCheckeredBackground = true;
            pbImage.FullscreenOnClick = true;
            pbImage.Location = new System.Drawing.Point(16, 200);
            pbImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbImage.Name = "pbImage";
            pbImage.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            pbImage.Size = new System.Drawing.Size(264, 234);
            pbImage.TabIndex = 6;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Enabled = false;
            btnAnalyze.Location = new System.Drawing.Point(16, 440);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new System.Drawing.Size(224, 32);
            btnAnalyze.TabIndex = 7;
            btnAnalyze.Text = "Analyze image";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // btnCapture
            // 
            btnCapture.Image = Properties.Resources.camera;
            btnCapture.Location = new System.Drawing.Point(248, 440);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new System.Drawing.Size(32, 32);
            btnCapture.TabIndex = 8;
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // lblResult
            // 
            lblResult.AutoSize = true;
            lblResult.Location = new System.Drawing.Point(301, 16);
            lblResult.Name = "lblResult";
            lblResult.Size = new System.Drawing.Size(48, 16);
            lblResult.TabIndex = 9;
            lblResult.Text = "Result:";
            // 
            // lblTimer
            // 
            lblTimer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblTimer.Location = new System.Drawing.Point(744, 12);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new System.Drawing.Size(168, 24);
            lblTimer.TabIndex = 10;
            lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtResult
            // 
            txtResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            txtResult.Location = new System.Drawing.Point(304, 40);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtResult.Size = new System.Drawing.Size(608, 432);
            txtResult.TabIndex = 11;
            // 
            // btnResultCopy
            // 
            btnResultCopy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnResultCopy.Enabled = false;
            btnResultCopy.Image = Properties.Resources.document_copy;
            btnResultCopy.Location = new System.Drawing.Point(858, 440);
            btnResultCopy.Name = "btnResultCopy";
            btnResultCopy.Size = new System.Drawing.Size(32, 32);
            btnResultCopy.TabIndex = 12;
            btnResultCopy.UseVisualStyleBackColor = true;
            btnResultCopy.Click += btnResultCopy_Click;
            // 
            // btnOptions
            // 
            btnOptions.Location = new System.Drawing.Point(16, 16);
            btnOptions.Name = "btnOptions";
            btnOptions.Size = new System.Drawing.Size(264, 32);
            btnOptions.TabIndex = 0;
            btnOptions.Text = "Options...";
            btnOptions.UseVisualStyleBackColor = true;
            btnOptions.Click += btnOptions_Click;
            // 
            // AIForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(928, 488);
            Controls.Add(btnOptions);
            Controls.Add(btnResultCopy);
            Controls.Add(txtResult);
            Controls.Add(lblTimer);
            Controls.Add(lblResult);
            Controls.Add(btnCapture);
            Controls.Add(btnAnalyze);
            Controls.Add(pbImage);
            Controls.Add(btnImageBrowse);
            Controls.Add(txtImage);
            Controls.Add(lblImage);
            Controls.Add(txtInput);
            Controls.Add(lblInput);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "AIForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ShareX - AI";
            Load += AIForm_Load;
            Shown += AIForm_Shown;
            DragDrop += AIForm_DragDrop;
            DragEnter += AIForm_DragEnter;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ContextMenuStrip cmsPresets;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.Button btnImageBrowse;
        private HelpersLib.MyPictureBox pbImage;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnResultCopy;
        private System.Windows.Forms.Button btnOptions;
    }
}
