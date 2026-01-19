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
            lblProvider = new System.Windows.Forms.Label();
            cbProvider = new System.Windows.Forms.ComboBox();
            btnAPIKeyHelp = new System.Windows.Forms.Button();
            gbOpenAI = new System.Windows.Forms.GroupBox();
            lblOpenAIAPIKey = new System.Windows.Forms.Label();
            txtOpenAIAPIKey = new System.Windows.Forms.TextBox();
            lblOpenAIModel = new System.Windows.Forms.Label();
            cbOpenAIModel = new System.Windows.Forms.ComboBox();
            lblOpenAICustomURL = new System.Windows.Forms.Label();
            txtOpenAICustomURL = new System.Windows.Forms.TextBox();
            gbGemini = new System.Windows.Forms.GroupBox();
            lblGeminiAPIKey = new System.Windows.Forms.Label();
            txtGeminiAPIKey = new System.Windows.Forms.TextBox();
            lblGeminiModel = new System.Windows.Forms.Label();
            cbGeminiModel = new System.Windows.Forms.ComboBox();
            gbOpenRouter = new System.Windows.Forms.GroupBox();
            lblOpenRouterAPIKey = new System.Windows.Forms.Label();
            txtOpenRouterAPIKey = new System.Windows.Forms.TextBox();
            lblOpenRouterModel = new System.Windows.Forms.Label();
            cbOpenRouterModel = new System.Windows.Forms.ComboBox();
            btnTestConnection = new System.Windows.Forms.Button();
            lblTestStatus = new System.Windows.Forms.Label();
            lblReasoningEffort = new System.Windows.Forms.Label();
            cbReasoningEffort = new System.Windows.Forms.ComboBox();
            lblVerbosity = new System.Windows.Forms.Label();
            cbVerbosity = new System.Windows.Forms.ComboBox();
            cbAutoStartRegion = new System.Windows.Forms.CheckBox();
            cbAutoStartAnalyze = new System.Windows.Forms.CheckBox();
            cbAutoCopyResult = new System.Windows.Forms.CheckBox();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            gbOpenAI.SuspendLayout();
            gbGemini.SuspendLayout();
            gbOpenRouter.SuspendLayout();
            SuspendLayout();
            // 
            // lblProvider
            // 
            lblProvider.AutoSize = true;
            lblProvider.Location = new System.Drawing.Point(16, 16);
            lblProvider.Name = "lblProvider";
            lblProvider.Size = new System.Drawing.Size(61, 16);
            lblProvider.TabIndex = 0;
            lblProvider.Text = "Provider:";
            // 
            // cbProvider
            // 
            cbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbProvider.FormattingEnabled = true;
            cbProvider.Location = new System.Drawing.Point(16, 36);
            cbProvider.Name = "cbProvider";
            cbProvider.Size = new System.Drawing.Size(312, 24);
            cbProvider.TabIndex = 1;
            cbProvider.SelectedIndexChanged += cbProvider_SelectedIndexChanged;
            // 
            // btnAPIKeyHelp
            // 
            btnAPIKeyHelp.Image = Properties.Resources.question;
            btnAPIKeyHelp.Location = new System.Drawing.Point(336, 32);
            btnAPIKeyHelp.Name = "btnAPIKeyHelp";
            btnAPIKeyHelp.Size = new System.Drawing.Size(32, 32);
            btnAPIKeyHelp.TabIndex = 2;
            btnAPIKeyHelp.UseVisualStyleBackColor = true;
            btnAPIKeyHelp.Click += btnAPIKeyHelp_Click;
            // 
            // gbOpenAI
            // 
            gbOpenAI.Controls.Add(lblOpenAIAPIKey);
            gbOpenAI.Controls.Add(txtOpenAIAPIKey);
            gbOpenAI.Controls.Add(lblOpenAIModel);
            gbOpenAI.Controls.Add(cbOpenAIModel);
            gbOpenAI.Controls.Add(lblOpenAICustomURL);
            gbOpenAI.Controls.Add(txtOpenAICustomURL);
            gbOpenAI.Location = new System.Drawing.Point(16, 72);
            gbOpenAI.Name = "gbOpenAI";
            gbOpenAI.Size = new System.Drawing.Size(352, 180);
            gbOpenAI.TabIndex = 3;
            gbOpenAI.TabStop = false;
            gbOpenAI.Text = "Provider Settings";
            // 
            // lblOpenAIAPIKey
            // 
            lblOpenAIAPIKey.AutoSize = true;
            lblOpenAIAPIKey.Location = new System.Drawing.Point(12, 24);
            lblOpenAIAPIKey.Name = "lblOpenAIAPIKey";
            lblOpenAIAPIKey.Size = new System.Drawing.Size(56, 16);
            lblOpenAIAPIKey.TabIndex = 0;
            lblOpenAIAPIKey.Text = "API key:";
            // 
            // txtOpenAIAPIKey
            // 
            txtOpenAIAPIKey.Location = new System.Drawing.Point(12, 44);
            txtOpenAIAPIKey.Name = "txtOpenAIAPIKey";
            txtOpenAIAPIKey.Size = new System.Drawing.Size(328, 22);
            txtOpenAIAPIKey.TabIndex = 1;
            txtOpenAIAPIKey.UseSystemPasswordChar = true;
            // 
            // lblOpenAIModel
            // 
            lblOpenAIModel.AutoSize = true;
            lblOpenAIModel.Location = new System.Drawing.Point(12, 76);
            lblOpenAIModel.Name = "lblOpenAIModel";
            lblOpenAIModel.Size = new System.Drawing.Size(48, 16);
            lblOpenAIModel.TabIndex = 2;
            lblOpenAIModel.Text = "Model:";
            // 
            // cbOpenAIModel
            // 
            cbOpenAIModel.FormattingEnabled = true;
            cbOpenAIModel.Items.AddRange(new object[] { "gpt-5.2", "gpt-5.1", "gpt-5", "gpt-5-mini", "gpt-5-nano" });
            cbOpenAIModel.Location = new System.Drawing.Point(12, 96);
            cbOpenAIModel.Name = "cbOpenAIModel";
            cbOpenAIModel.Size = new System.Drawing.Size(328, 24);
            cbOpenAIModel.TabIndex = 3;
            // 
            // lblOpenAICustomURL
            // 
            lblOpenAICustomURL.AutoSize = true;
            lblOpenAICustomURL.Location = new System.Drawing.Point(12, 128);
            lblOpenAICustomURL.Name = "lblOpenAICustomURL";
            lblOpenAICustomURL.Size = new System.Drawing.Size(119, 16);
            lblOpenAICustomURL.TabIndex = 4;
            lblOpenAICustomURL.Text = "Custom base URL:";
            // 
            // txtOpenAICustomURL
            // 
            txtOpenAICustomURL.Location = new System.Drawing.Point(12, 148);
            txtOpenAICustomURL.Name = "txtOpenAICustomURL";
            txtOpenAICustomURL.Size = new System.Drawing.Size(328, 22);
            txtOpenAICustomURL.TabIndex = 5;
            // 
            // gbGemini
            // 
            gbGemini.Controls.Add(lblGeminiAPIKey);
            gbGemini.Controls.Add(txtGeminiAPIKey);
            gbGemini.Controls.Add(lblGeminiModel);
            gbGemini.Controls.Add(cbGeminiModel);
            gbGemini.Location = new System.Drawing.Point(16, 72);
            gbGemini.Name = "gbGemini";
            gbGemini.Size = new System.Drawing.Size(352, 180);
            gbGemini.TabIndex = 4;
            gbGemini.TabStop = false;
            gbGemini.Text = "Provider Settings";
            gbGemini.Visible = false;
            // 
            // lblGeminiAPIKey
            // 
            lblGeminiAPIKey.AutoSize = true;
            lblGeminiAPIKey.Location = new System.Drawing.Point(12, 24);
            lblGeminiAPIKey.Name = "lblGeminiAPIKey";
            lblGeminiAPIKey.Size = new System.Drawing.Size(56, 16);
            lblGeminiAPIKey.TabIndex = 0;
            lblGeminiAPIKey.Text = "API key:";
            // 
            // txtGeminiAPIKey
            // 
            txtGeminiAPIKey.Location = new System.Drawing.Point(12, 44);
            txtGeminiAPIKey.Name = "txtGeminiAPIKey";
            txtGeminiAPIKey.Size = new System.Drawing.Size(328, 22);
            txtGeminiAPIKey.TabIndex = 1;
            txtGeminiAPIKey.UseSystemPasswordChar = true;
            // 
            // lblGeminiModel
            // 
            lblGeminiModel.AutoSize = true;
            lblGeminiModel.Location = new System.Drawing.Point(12, 76);
            lblGeminiModel.Name = "lblGeminiModel";
            lblGeminiModel.Size = new System.Drawing.Size(48, 16);
            lblGeminiModel.TabIndex = 2;
            lblGeminiModel.Text = "Model:";
            // 
            // cbGeminiModel
            // 
            cbGeminiModel.FormattingEnabled = true;
            cbGeminiModel.Items.AddRange(new object[] { "gemini-2.0-flash", "gemini-2.0-flash-lite", "gemini-1.5-flash", "gemini-1.5-pro" });
            cbGeminiModel.Location = new System.Drawing.Point(12, 96);
            cbGeminiModel.Name = "cbGeminiModel";
            cbGeminiModel.Size = new System.Drawing.Size(328, 24);
            cbGeminiModel.TabIndex = 3;
            // 
            // gbOpenRouter
            // 
            gbOpenRouter.Controls.Add(lblOpenRouterAPIKey);
            gbOpenRouter.Controls.Add(txtOpenRouterAPIKey);
            gbOpenRouter.Controls.Add(lblOpenRouterModel);
            gbOpenRouter.Controls.Add(cbOpenRouterModel);
            gbOpenRouter.Location = new System.Drawing.Point(16, 72);
            gbOpenRouter.Name = "gbOpenRouter";
            gbOpenRouter.Size = new System.Drawing.Size(352, 180);
            gbOpenRouter.TabIndex = 5;
            gbOpenRouter.TabStop = false;
            gbOpenRouter.Text = "Provider Settings";
            gbOpenRouter.Visible = false;
            // 
            // lblOpenRouterAPIKey
            // 
            lblOpenRouterAPIKey.AutoSize = true;
            lblOpenRouterAPIKey.Location = new System.Drawing.Point(12, 24);
            lblOpenRouterAPIKey.Name = "lblOpenRouterAPIKey";
            lblOpenRouterAPIKey.Size = new System.Drawing.Size(56, 16);
            lblOpenRouterAPIKey.TabIndex = 0;
            lblOpenRouterAPIKey.Text = "API key:";
            // 
            // txtOpenRouterAPIKey
            // 
            txtOpenRouterAPIKey.Location = new System.Drawing.Point(12, 44);
            txtOpenRouterAPIKey.Name = "txtOpenRouterAPIKey";
            txtOpenRouterAPIKey.Size = new System.Drawing.Size(328, 22);
            txtOpenRouterAPIKey.TabIndex = 1;
            txtOpenRouterAPIKey.UseSystemPasswordChar = true;
            // 
            // lblOpenRouterModel
            // 
            lblOpenRouterModel.AutoSize = true;
            lblOpenRouterModel.Location = new System.Drawing.Point(12, 76);
            lblOpenRouterModel.Name = "lblOpenRouterModel";
            lblOpenRouterModel.Size = new System.Drawing.Size(48, 16);
            lblOpenRouterModel.TabIndex = 2;
            lblOpenRouterModel.Text = "Model:";
            // 
            // cbOpenRouterModel
            // 
            cbOpenRouterModel.FormattingEnabled = true;
            cbOpenRouterModel.Items.AddRange(new object[] { "openai/gpt-4o", "anthropic/claude-3.5-sonnet", "google/gemini-pro-1.5" });
            cbOpenRouterModel.Location = new System.Drawing.Point(12, 96);
            cbOpenRouterModel.Name = "cbOpenRouterModel";
            cbOpenRouterModel.Size = new System.Drawing.Size(328, 24);
            cbOpenRouterModel.TabIndex = 3;
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new System.Drawing.Point(288, 260);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new System.Drawing.Size(80, 28);
            btnTestConnection.TabIndex = 6;
            btnTestConnection.Text = "Test";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // lblTestStatus
            // 
            lblTestStatus.AutoSize = true;
            lblTestStatus.Location = new System.Drawing.Point(16, 266);
            lblTestStatus.Name = "lblTestStatus";
            lblTestStatus.Size = new System.Drawing.Size(0, 16);
            lblTestStatus.TabIndex = 7;
            // 
            // lblReasoningEffort
            // 
            lblReasoningEffort.AutoSize = true;
            lblReasoningEffort.Location = new System.Drawing.Point(16, 300);
            lblReasoningEffort.Name = "lblReasoningEffort";
            lblReasoningEffort.Size = new System.Drawing.Size(108, 16);
            lblReasoningEffort.TabIndex = 8;
            lblReasoningEffort.Text = "Reasoning effort:";
            // 
            // cbReasoningEffort
            // 
            cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbReasoningEffort.FormattingEnabled = true;
            cbReasoningEffort.Items.AddRange(new object[] { "minimal", "low", "medium", "high" });
            cbReasoningEffort.Location = new System.Drawing.Point(16, 320);
            cbReasoningEffort.Name = "cbReasoningEffort";
            cbReasoningEffort.Size = new System.Drawing.Size(352, 24);
            cbReasoningEffort.TabIndex = 9;
            // 
            // lblVerbosity
            // 
            lblVerbosity.AutoSize = true;
            lblVerbosity.Location = new System.Drawing.Point(16, 356);
            lblVerbosity.Name = "lblVerbosity";
            lblVerbosity.Size = new System.Drawing.Size(67, 16);
            lblVerbosity.TabIndex = 10;
            lblVerbosity.Text = "Verbosity:";
            // 
            // cbVerbosity
            // 
            cbVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbVerbosity.FormattingEnabled = true;
            cbVerbosity.Items.AddRange(new object[] { "high", "medium", "low" });
            cbVerbosity.Location = new System.Drawing.Point(16, 376);
            cbVerbosity.Name = "cbVerbosity";
            cbVerbosity.Size = new System.Drawing.Size(352, 24);
            cbVerbosity.TabIndex = 11;
            // 
            // cbAutoStartRegion
            // 
            cbAutoStartRegion.AutoSize = true;
            cbAutoStartRegion.Location = new System.Drawing.Point(16, 416);
            cbAutoStartRegion.Name = "cbAutoStartRegion";
            cbAutoStartRegion.Size = new System.Drawing.Size(122, 20);
            cbAutoStartRegion.TabIndex = 12;
            cbAutoStartRegion.Text = "Auto start region";
            cbAutoStartRegion.UseVisualStyleBackColor = true;
            // 
            // cbAutoStartAnalyze
            // 
            cbAutoStartAnalyze.AutoSize = true;
            cbAutoStartAnalyze.Location = new System.Drawing.Point(16, 440);
            cbAutoStartAnalyze.Name = "cbAutoStartAnalyze";
            cbAutoStartAnalyze.Size = new System.Drawing.Size(131, 20);
            cbAutoStartAnalyze.TabIndex = 13;
            cbAutoStartAnalyze.Text = "Auto start analyze";
            cbAutoStartAnalyze.UseVisualStyleBackColor = true;
            // 
            // cbAutoCopyResult
            // 
            cbAutoCopyResult.AutoSize = true;
            cbAutoCopyResult.Location = new System.Drawing.Point(16, 464);
            cbAutoCopyResult.Name = "cbAutoCopyResult";
            cbAutoCopyResult.Size = new System.Drawing.Size(121, 20);
            cbAutoCopyResult.TabIndex = 14;
            cbAutoCopyResult.Text = "Auto copy result";
            cbAutoCopyResult.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(160, 500);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(100, 32);
            btnOK.TabIndex = 15;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(268, 500);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 32);
            btnCancel.TabIndex = 16;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // AIOptionsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(384, 548);
            Controls.Add(lblProvider);
            Controls.Add(cbProvider);
            Controls.Add(btnAPIKeyHelp);
            Controls.Add(gbOpenAI);
            Controls.Add(gbGemini);
            Controls.Add(gbOpenRouter);
            Controls.Add(btnTestConnection);
            Controls.Add(lblTestStatus);
            Controls.Add(lblReasoningEffort);
            Controls.Add(cbReasoningEffort);
            Controls.Add(lblVerbosity);
            Controls.Add(cbVerbosity);
            Controls.Add(cbAutoStartRegion);
            Controls.Add(cbAutoStartAnalyze);
            Controls.Add(cbAutoCopyResult);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "AIOptionsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ShareX - AI options";
            gbOpenAI.ResumeLayout(false);
            gbOpenAI.PerformLayout();
            gbGemini.ResumeLayout(false);
            gbGemini.PerformLayout();
            gbOpenRouter.ResumeLayout(false);
            gbOpenRouter.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.ComboBox cbProvider;
        private System.Windows.Forms.Button btnAPIKeyHelp;
        private System.Windows.Forms.GroupBox gbOpenAI;
        private System.Windows.Forms.Label lblOpenAIAPIKey;
        private System.Windows.Forms.TextBox txtOpenAIAPIKey;
        private System.Windows.Forms.Label lblOpenAIModel;
        private System.Windows.Forms.ComboBox cbOpenAIModel;
        private System.Windows.Forms.Label lblOpenAICustomURL;
        private System.Windows.Forms.TextBox txtOpenAICustomURL;
        private System.Windows.Forms.GroupBox gbGemini;
        private System.Windows.Forms.Label lblGeminiAPIKey;
        private System.Windows.Forms.TextBox txtGeminiAPIKey;
        private System.Windows.Forms.Label lblGeminiModel;
        private System.Windows.Forms.ComboBox cbGeminiModel;
        private System.Windows.Forms.GroupBox gbOpenRouter;
        private System.Windows.Forms.Label lblOpenRouterAPIKey;
        private System.Windows.Forms.TextBox txtOpenRouterAPIKey;
        private System.Windows.Forms.Label lblOpenRouterModel;
        private System.Windows.Forms.ComboBox cbOpenRouterModel;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblTestStatus;
        private System.Windows.Forms.Label lblReasoningEffort;
        private System.Windows.Forms.ComboBox cbReasoningEffort;
        private System.Windows.Forms.Label lblVerbosity;
        private System.Windows.Forms.ComboBox cbVerbosity;
        private System.Windows.Forms.CheckBox cbAutoStartRegion;
        private System.Windows.Forms.CheckBox cbAutoStartAnalyze;
        private System.Windows.Forms.CheckBox cbAutoCopyResult;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
