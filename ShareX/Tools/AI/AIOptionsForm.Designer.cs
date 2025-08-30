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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AIOptionsForm));
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
            cbVerbosity = new System.Windows.Forms.ComboBox();
            lblVerbosity = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnAPIKeyHelp
            // 
            btnAPIKeyHelp.Image = Properties.Resources.question;
            resources.ApplyResources(btnAPIKeyHelp, "btnAPIKeyHelp");
            btnAPIKeyHelp.Name = "btnAPIKeyHelp";
            btnAPIKeyHelp.UseVisualStyleBackColor = true;
            btnAPIKeyHelp.Click += btnAPIKeyHelp_Click;
            // 
            // cbReasoningEffort
            // 
            cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbReasoningEffort.FormattingEnabled = true;
            cbReasoningEffort.Items.AddRange(new object[] { resources.GetString("cbReasoningEffort.Items"), resources.GetString("cbReasoningEffort.Items1"), resources.GetString("cbReasoningEffort.Items2"), resources.GetString("cbReasoningEffort.Items3") });
            resources.ApplyResources(cbReasoningEffort, "cbReasoningEffort");
            cbReasoningEffort.Name = "cbReasoningEffort";
            // 
            // lblReasoningEffort
            // 
            resources.ApplyResources(lblReasoningEffort, "lblReasoningEffort");
            lblReasoningEffort.Name = "lblReasoningEffort";
            // 
            // txtAPIKey
            // 
            resources.ApplyResources(txtAPIKey, "txtAPIKey");
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.UseSystemPasswordChar = true;
            // 
            // lblAPIKey
            // 
            resources.ApplyResources(lblAPIKey, "lblAPIKey");
            lblAPIKey.Name = "lblAPIKey";
            // 
            // cbModel
            // 
            cbModel.FormattingEnabled = true;
            cbModel.Items.AddRange(new object[] { resources.GetString("cbModel.Items"), resources.GetString("cbModel.Items1"), resources.GetString("cbModel.Items2") });
            resources.ApplyResources(cbModel, "cbModel");
            cbModel.Name = "cbModel";
            // 
            // lblModel
            // 
            resources.ApplyResources(lblModel, "lblModel");
            lblModel.Name = "lblModel";
            // 
            // cbAutoStartRegion
            // 
            resources.ApplyResources(cbAutoStartRegion, "cbAutoStartRegion");
            cbAutoStartRegion.Name = "cbAutoStartRegion";
            cbAutoStartRegion.UseVisualStyleBackColor = true;
            // 
            // cbAutoStartAnalyze
            // 
            resources.ApplyResources(cbAutoStartAnalyze, "cbAutoStartAnalyze");
            cbAutoStartAnalyze.Name = "cbAutoStartAnalyze";
            cbAutoStartAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(btnOK, "btnOK");
            btnOK.Name = "btnOK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // cbAutoCopyResult
            // 
            resources.ApplyResources(cbAutoCopyResult, "cbAutoCopyResult");
            cbAutoCopyResult.Name = "cbAutoCopyResult";
            cbAutoCopyResult.UseVisualStyleBackColor = true;
            // 
            // cbVerbosity
            // 
            cbVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbVerbosity.FormattingEnabled = true;
            cbVerbosity.Items.AddRange(new object[] { resources.GetString("cbVerbosity.Items"), resources.GetString("cbVerbosity.Items1"), resources.GetString("cbVerbosity.Items2") });
            resources.ApplyResources(cbVerbosity, "cbVerbosity");
            cbVerbosity.Name = "cbVerbosity";
            // 
            // lblVerbosity
            // 
            resources.ApplyResources(lblVerbosity, "lblVerbosity");
            lblVerbosity.Name = "lblVerbosity";
            // 
            // AIOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(lblVerbosity);
            Controls.Add(cbVerbosity);
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
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "AIOptionsForm";
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
        private System.Windows.Forms.ComboBox cbVerbosity;
        private System.Windows.Forms.Label lblVerbosity;
    }
}