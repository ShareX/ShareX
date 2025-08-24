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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AIForm));
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
            btnAPIKeyHelp = new System.Windows.Forms.Button();
            btnResultCopy = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblModel
            // 
            resources.ApplyResources(lblModel, "lblModel");
            lblModel.Name = "lblModel";
            // 
            // cbModel
            // 
            cbModel.FormattingEnabled = true;
            cbModel.Items.AddRange(new object[] { resources.GetString("cbModel.Items"), resources.GetString("cbModel.Items1"), resources.GetString("cbModel.Items2") });
            resources.ApplyResources(cbModel, "cbModel");
            cbModel.Name = "cbModel";
            cbModel.TextChanged += cbModel_TextChanged;
            // 
            // lblInput
            // 
            resources.ApplyResources(lblInput, "lblInput");
            lblInput.Name = "lblInput";
            // 
            // lblImage
            // 
            resources.ApplyResources(lblImage, "lblImage");
            lblImage.Name = "lblImage";
            // 
            // txtImage
            // 
            resources.ApplyResources(txtImage, "txtImage");
            txtImage.Name = "txtImage";
            txtImage.TextChanged += txtImage_TextChanged;
            // 
            // btnAnalyze
            // 
            resources.ApplyResources(btnAnalyze, "btnAnalyze");
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // lblAPIKey
            // 
            resources.ApplyResources(lblAPIKey, "lblAPIKey");
            lblAPIKey.Name = "lblAPIKey";
            // 
            // txtAPIKey
            // 
            resources.ApplyResources(txtAPIKey, "txtAPIKey");
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.UseSystemPasswordChar = true;
            txtAPIKey.TextChanged += txtAPIKey_TextChanged;
            // 
            // lblResult
            // 
            resources.ApplyResources(lblResult, "lblResult");
            lblResult.Name = "lblResult";
            // 
            // txtResult
            // 
            resources.ApplyResources(txtResult, "txtResult");
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            // 
            // btnImageBrowse
            // 
            btnImageBrowse.Image = Properties.Resources.folder_open_image;
            resources.ApplyResources(btnImageBrowse, "btnImageBrowse");
            btnImageBrowse.Name = "btnImageBrowse";
            btnImageBrowse.UseVisualStyleBackColor = true;
            btnImageBrowse.Click += btnImageBrowse_Click;
            // 
            // pbImage
            // 
            pbImage.BackColor = System.Drawing.SystemColors.Window;
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.DrawCheckeredBackground = true;
            pbImage.FullscreenOnClick = true;
            resources.ApplyResources(pbImage, "pbImage");
            pbImage.Name = "pbImage";
            pbImage.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            // 
            // cbInput
            // 
            cbInput.FormattingEnabled = true;
            cbInput.Items.AddRange(new object[] { resources.GetString("cbInput.Items"), resources.GetString("cbInput.Items1"), resources.GetString("cbInput.Items2"), resources.GetString("cbInput.Items3") });
            resources.ApplyResources(cbInput, "cbInput");
            cbInput.Name = "cbInput";
            cbInput.TextChanged += cbInput_TextChanged;
            // 
            // lblReasoningEffort
            // 
            resources.ApplyResources(lblReasoningEffort, "lblReasoningEffort");
            lblReasoningEffort.Name = "lblReasoningEffort";
            // 
            // cbReasoningEffort
            // 
            cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbReasoningEffort.FormattingEnabled = true;
            cbReasoningEffort.Items.AddRange(new object[] { resources.GetString("cbReasoningEffort.Items"), resources.GetString("cbReasoningEffort.Items1"), resources.GetString("cbReasoningEffort.Items2"), resources.GetString("cbReasoningEffort.Items3") });
            resources.ApplyResources(cbReasoningEffort, "cbReasoningEffort");
            cbReasoningEffort.Name = "cbReasoningEffort";
            cbReasoningEffort.SelectedIndexChanged += cbReasoningEffort_SelectedIndexChanged;
            // 
            // lblTimer
            // 
            resources.ApplyResources(lblTimer, "lblTimer");
            lblTimer.Name = "lblTimer";
            // 
            // btnCapture
            // 
            btnCapture.Image = Properties.Resources.camera;
            resources.ApplyResources(btnCapture, "btnCapture");
            btnCapture.Name = "btnCapture";
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // btnAPIKeyHelp
            // 
            btnAPIKeyHelp.Image = Properties.Resources.question;
            resources.ApplyResources(btnAPIKeyHelp, "btnAPIKeyHelp");
            btnAPIKeyHelp.Name = "btnAPIKeyHelp";
            btnAPIKeyHelp.UseVisualStyleBackColor = true;
            btnAPIKeyHelp.Click += btnAPIKeyHelp_Click;
            // 
            // btnResultCopy
            // 
            resources.ApplyResources(btnResultCopy, "btnResultCopy");
            btnResultCopy.Image = Properties.Resources.document_copy;
            btnResultCopy.Name = "btnResultCopy";
            btnResultCopy.UseVisualStyleBackColor = true;
            btnResultCopy.Click += btnResultCopy_Click;
            // 
            // AIForm
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(btnResultCopy);
            Controls.Add(btnAPIKeyHelp);
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
            Name = "AIForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
        private System.Windows.Forms.Button btnAPIKeyHelp;
        private System.Windows.Forms.Button btnResultCopy;
    }
}