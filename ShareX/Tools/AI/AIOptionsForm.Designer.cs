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
            this.lblProvider = new System.Windows.Forms.Label();
            this.cbProvider = new System.Windows.Forms.ComboBox();
            this.btnAPIKeyHelp = new System.Windows.Forms.Button();
            this.gbOpenAI = new System.Windows.Forms.GroupBox();
            this.lblOpenAIAPIKey = new System.Windows.Forms.Label();
            this.txtOpenAIAPIKey = new System.Windows.Forms.TextBox();
            this.lblOpenAIModel = new System.Windows.Forms.Label();
            this.cbOpenAIModel = new System.Windows.Forms.ComboBox();
            this.lblOpenAICustomURL = new System.Windows.Forms.Label();
            this.txtOpenAICustomURL = new System.Windows.Forms.TextBox();
            this.gbGemini = new System.Windows.Forms.GroupBox();
            this.lblGeminiAPIKey = new System.Windows.Forms.Label();
            this.txtGeminiAPIKey = new System.Windows.Forms.TextBox();
            this.lblGeminiModel = new System.Windows.Forms.Label();
            this.cbGeminiModel = new System.Windows.Forms.ComboBox();
            this.gbOpenRouter = new System.Windows.Forms.GroupBox();
            this.lblOpenRouterAPIKey = new System.Windows.Forms.Label();
            this.txtOpenRouterAPIKey = new System.Windows.Forms.TextBox();
            this.lblOpenRouterModel = new System.Windows.Forms.Label();
            this.cbOpenRouterModel = new System.Windows.Forms.ComboBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblTestStatus = new System.Windows.Forms.Label();
            this.lblReasoningEffort = new System.Windows.Forms.Label();
            this.cbReasoningEffort = new System.Windows.Forms.ComboBox();
            this.lblVerbosity = new System.Windows.Forms.Label();
            this.cbVerbosity = new System.Windows.Forms.ComboBox();
            this.cbAutoStartRegion = new System.Windows.Forms.CheckBox();
            this.cbAutoStartAnalyze = new System.Windows.Forms.CheckBox();
            this.cbAutoCopyResult = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbOpenAI.SuspendLayout();
            this.gbGemini.SuspendLayout();
            this.gbOpenRouter.SuspendLayout();
            this.SuspendLayout();
            //
            // lblProvider
            //
            this.lblProvider.AutoSize = true;
            this.lblProvider.Location = new System.Drawing.Point(16, 16);
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Size = new System.Drawing.Size(61, 16);
            this.lblProvider.TabIndex = 0;
            this.lblProvider.Text = "Provider:";
            //
            // cbProvider
            //
            this.cbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProvider.FormattingEnabled = true;
            this.cbProvider.Location = new System.Drawing.Point(16, 36);
            this.cbProvider.Name = "cbProvider";
            this.cbProvider.Size = new System.Drawing.Size(312, 24);
            this.cbProvider.TabIndex = 1;
            this.cbProvider.SelectedIndexChanged += new System.EventHandler(this.cbProvider_SelectedIndexChanged);
            //
            // btnAPIKeyHelp
            //
            this.btnAPIKeyHelp.Image = global::ShareX.Properties.Resources.question;
            this.btnAPIKeyHelp.Location = new System.Drawing.Point(336, 32);
            this.btnAPIKeyHelp.Name = "btnAPIKeyHelp";
            this.btnAPIKeyHelp.Size = new System.Drawing.Size(32, 32);
            this.btnAPIKeyHelp.TabIndex = 2;
            this.btnAPIKeyHelp.UseVisualStyleBackColor = true;
            this.btnAPIKeyHelp.Click += new System.EventHandler(this.btnAPIKeyHelp_Click);
            //
            // gbOpenAI
            //
            this.gbOpenAI.Controls.Add(this.lblOpenAIAPIKey);
            this.gbOpenAI.Controls.Add(this.txtOpenAIAPIKey);
            this.gbOpenAI.Controls.Add(this.lblOpenAIModel);
            this.gbOpenAI.Controls.Add(this.cbOpenAIModel);
            this.gbOpenAI.Controls.Add(this.lblOpenAICustomURL);
            this.gbOpenAI.Controls.Add(this.txtOpenAICustomURL);
            this.gbOpenAI.Location = new System.Drawing.Point(16, 72);
            this.gbOpenAI.Name = "gbOpenAI";
            this.gbOpenAI.Size = new System.Drawing.Size(352, 180);
            this.gbOpenAI.TabIndex = 3;
            this.gbOpenAI.TabStop = false;
            this.gbOpenAI.Text = "Provider Settings";
            //
            // lblOpenAIAPIKey
            //
            this.lblOpenAIAPIKey.AutoSize = true;
            this.lblOpenAIAPIKey.Location = new System.Drawing.Point(12, 24);
            this.lblOpenAIAPIKey.Name = "lblOpenAIAPIKey";
            this.lblOpenAIAPIKey.Size = new System.Drawing.Size(56, 16);
            this.lblOpenAIAPIKey.TabIndex = 0;
            this.lblOpenAIAPIKey.Text = "API key:";
            //
            // txtOpenAIAPIKey
            //
            this.txtOpenAIAPIKey.Location = new System.Drawing.Point(12, 44);
            this.txtOpenAIAPIKey.Name = "txtOpenAIAPIKey";
            this.txtOpenAIAPIKey.Size = new System.Drawing.Size(328, 22);
            this.txtOpenAIAPIKey.TabIndex = 1;
            this.txtOpenAIAPIKey.UseSystemPasswordChar = true;
            //
            // lblOpenAIModel
            //
            this.lblOpenAIModel.AutoSize = true;
            this.lblOpenAIModel.Location = new System.Drawing.Point(12, 76);
            this.lblOpenAIModel.Name = "lblOpenAIModel";
            this.lblOpenAIModel.Size = new System.Drawing.Size(48, 16);
            this.lblOpenAIModel.TabIndex = 2;
            this.lblOpenAIModel.Text = "Model:";
            //
            // cbOpenAIModel
            //
            this.cbOpenAIModel.FormattingEnabled = true;
            this.cbOpenAIModel.Items.AddRange(new object[] {
            "gpt-4o",
            "gpt-4o-mini",
            "gpt-4-turbo",
            "gpt-4",
            "o1",
            "o1-mini",
            "o3-mini"});
            this.cbOpenAIModel.Location = new System.Drawing.Point(12, 96);
            this.cbOpenAIModel.Name = "cbOpenAIModel";
            this.cbOpenAIModel.Size = new System.Drawing.Size(328, 24);
            this.cbOpenAIModel.TabIndex = 3;
            //
            // lblOpenAICustomURL
            //
            this.lblOpenAICustomURL.AutoSize = true;
            this.lblOpenAICustomURL.Location = new System.Drawing.Point(12, 128);
            this.lblOpenAICustomURL.Name = "lblOpenAICustomURL";
            this.lblOpenAICustomURL.Size = new System.Drawing.Size(113, 16);
            this.lblOpenAICustomURL.TabIndex = 4;
            this.lblOpenAICustomURL.Text = "Custom base URL:";
            //
            // txtOpenAICustomURL
            //
            this.txtOpenAICustomURL.Location = new System.Drawing.Point(12, 148);
            this.txtOpenAICustomURL.Name = "txtOpenAICustomURL";
            this.txtOpenAICustomURL.Size = new System.Drawing.Size(328, 22);
            this.txtOpenAICustomURL.TabIndex = 5;
            //
            // gbGemini
            //
            this.gbGemini.Controls.Add(this.lblGeminiAPIKey);
            this.gbGemini.Controls.Add(this.txtGeminiAPIKey);
            this.gbGemini.Controls.Add(this.lblGeminiModel);
            this.gbGemini.Controls.Add(this.cbGeminiModel);
            this.gbGemini.Location = new System.Drawing.Point(16, 72);
            this.gbGemini.Name = "gbGemini";
            this.gbGemini.Size = new System.Drawing.Size(352, 180);
            this.gbGemini.TabIndex = 4;
            this.gbGemini.TabStop = false;
            this.gbGemini.Text = "Provider Settings";
            this.gbGemini.Visible = false;
            //
            // lblGeminiAPIKey
            //
            this.lblGeminiAPIKey.AutoSize = true;
            this.lblGeminiAPIKey.Location = new System.Drawing.Point(12, 24);
            this.lblGeminiAPIKey.Name = "lblGeminiAPIKey";
            this.lblGeminiAPIKey.Size = new System.Drawing.Size(56, 16);
            this.lblGeminiAPIKey.TabIndex = 0;
            this.lblGeminiAPIKey.Text = "API key:";
            //
            // txtGeminiAPIKey
            //
            this.txtGeminiAPIKey.Location = new System.Drawing.Point(12, 44);
            this.txtGeminiAPIKey.Name = "txtGeminiAPIKey";
            this.txtGeminiAPIKey.Size = new System.Drawing.Size(328, 22);
            this.txtGeminiAPIKey.TabIndex = 1;
            this.txtGeminiAPIKey.UseSystemPasswordChar = true;
            //
            // lblGeminiModel
            //
            this.lblGeminiModel.AutoSize = true;
            this.lblGeminiModel.Location = new System.Drawing.Point(12, 76);
            this.lblGeminiModel.Name = "lblGeminiModel";
            this.lblGeminiModel.Size = new System.Drawing.Size(48, 16);
            this.lblGeminiModel.TabIndex = 2;
            this.lblGeminiModel.Text = "Model:";
            //
            // cbGeminiModel
            //
            this.cbGeminiModel.FormattingEnabled = true;
            this.cbGeminiModel.Items.AddRange(new object[] {
            "gemini-2.0-flash",
            "gemini-2.0-flash-lite",
            "gemini-1.5-flash",
            "gemini-1.5-pro"});
            this.cbGeminiModel.Location = new System.Drawing.Point(12, 96);
            this.cbGeminiModel.Name = "cbGeminiModel";
            this.cbGeminiModel.Size = new System.Drawing.Size(328, 24);
            this.cbGeminiModel.TabIndex = 3;
            //
            // gbOpenRouter
            //
            this.gbOpenRouter.Controls.Add(this.lblOpenRouterAPIKey);
            this.gbOpenRouter.Controls.Add(this.txtOpenRouterAPIKey);
            this.gbOpenRouter.Controls.Add(this.lblOpenRouterModel);
            this.gbOpenRouter.Controls.Add(this.cbOpenRouterModel);
            this.gbOpenRouter.Location = new System.Drawing.Point(16, 72);
            this.gbOpenRouter.Name = "gbOpenRouter";
            this.gbOpenRouter.Size = new System.Drawing.Size(352, 180);
            this.gbOpenRouter.TabIndex = 5;
            this.gbOpenRouter.TabStop = false;
            this.gbOpenRouter.Text = "Provider Settings";
            this.gbOpenRouter.Visible = false;
            //
            // lblOpenRouterAPIKey
            //
            this.lblOpenRouterAPIKey.AutoSize = true;
            this.lblOpenRouterAPIKey.Location = new System.Drawing.Point(12, 24);
            this.lblOpenRouterAPIKey.Name = "lblOpenRouterAPIKey";
            this.lblOpenRouterAPIKey.Size = new System.Drawing.Size(56, 16);
            this.lblOpenRouterAPIKey.TabIndex = 0;
            this.lblOpenRouterAPIKey.Text = "API key:";
            //
            // txtOpenRouterAPIKey
            //
            this.txtOpenRouterAPIKey.Location = new System.Drawing.Point(12, 44);
            this.txtOpenRouterAPIKey.Name = "txtOpenRouterAPIKey";
            this.txtOpenRouterAPIKey.Size = new System.Drawing.Size(328, 22);
            this.txtOpenRouterAPIKey.TabIndex = 1;
            this.txtOpenRouterAPIKey.UseSystemPasswordChar = true;
            //
            // lblOpenRouterModel
            //
            this.lblOpenRouterModel.AutoSize = true;
            this.lblOpenRouterModel.Location = new System.Drawing.Point(12, 76);
            this.lblOpenRouterModel.Name = "lblOpenRouterModel";
            this.lblOpenRouterModel.Size = new System.Drawing.Size(48, 16);
            this.lblOpenRouterModel.TabIndex = 2;
            this.lblOpenRouterModel.Text = "Model:";
            //
            // cbOpenRouterModel
            //
            this.cbOpenRouterModel.FormattingEnabled = true;
            this.cbOpenRouterModel.Items.AddRange(new object[] {
            "openai/gpt-4o",
            "anthropic/claude-3.5-sonnet",
            "google/gemini-pro-1.5"});
            this.cbOpenRouterModel.Location = new System.Drawing.Point(12, 96);
            this.cbOpenRouterModel.Name = "cbOpenRouterModel";
            this.cbOpenRouterModel.Size = new System.Drawing.Size(328, 24);
            this.cbOpenRouterModel.TabIndex = 3;
            //
            // btnTestConnection
            //
            this.btnTestConnection.Location = new System.Drawing.Point(288, 260);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(80, 28);
            this.btnTestConnection.TabIndex = 6;
            this.btnTestConnection.Text = "Test";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            //
            // lblTestStatus
            //
            this.lblTestStatus.AutoSize = true;
            this.lblTestStatus.Location = new System.Drawing.Point(16, 266);
            this.lblTestStatus.Name = "lblTestStatus";
            this.lblTestStatus.Size = new System.Drawing.Size(0, 16);
            this.lblTestStatus.TabIndex = 7;
            //
            // lblReasoningEffort
            //
            this.lblReasoningEffort.AutoSize = true;
            this.lblReasoningEffort.Location = new System.Drawing.Point(16, 300);
            this.lblReasoningEffort.Name = "lblReasoningEffort";
            this.lblReasoningEffort.Size = new System.Drawing.Size(108, 16);
            this.lblReasoningEffort.TabIndex = 8;
            this.lblReasoningEffort.Text = "Reasoning effort:";
            //
            // cbReasoningEffort
            //
            this.cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReasoningEffort.FormattingEnabled = true;
            this.cbReasoningEffort.Items.AddRange(new object[] {
            "minimal",
            "low",
            "medium",
            "high"});
            this.cbReasoningEffort.Location = new System.Drawing.Point(16, 320);
            this.cbReasoningEffort.Name = "cbReasoningEffort";
            this.cbReasoningEffort.Size = new System.Drawing.Size(352, 24);
            this.cbReasoningEffort.TabIndex = 9;
            //
            // lblVerbosity
            //
            this.lblVerbosity.AutoSize = true;
            this.lblVerbosity.Location = new System.Drawing.Point(16, 356);
            this.lblVerbosity.Name = "lblVerbosity";
            this.lblVerbosity.Size = new System.Drawing.Size(67, 16);
            this.lblVerbosity.TabIndex = 10;
            this.lblVerbosity.Text = "Verbosity:";
            //
            // cbVerbosity
            //
            this.cbVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVerbosity.FormattingEnabled = true;
            this.cbVerbosity.Items.AddRange(new object[] {
            "high",
            "medium",
            "low"});
            this.cbVerbosity.Location = new System.Drawing.Point(16, 376);
            this.cbVerbosity.Name = "cbVerbosity";
            this.cbVerbosity.Size = new System.Drawing.Size(352, 24);
            this.cbVerbosity.TabIndex = 11;
            //
            // cbAutoStartRegion
            //
            this.cbAutoStartRegion.AutoSize = true;
            this.cbAutoStartRegion.Location = new System.Drawing.Point(16, 416);
            this.cbAutoStartRegion.Name = "cbAutoStartRegion";
            this.cbAutoStartRegion.Size = new System.Drawing.Size(122, 20);
            this.cbAutoStartRegion.TabIndex = 12;
            this.cbAutoStartRegion.Text = "Auto start region";
            this.cbAutoStartRegion.UseVisualStyleBackColor = true;
            //
            // cbAutoStartAnalyze
            //
            this.cbAutoStartAnalyze.AutoSize = true;
            this.cbAutoStartAnalyze.Location = new System.Drawing.Point(16, 440);
            this.cbAutoStartAnalyze.Name = "cbAutoStartAnalyze";
            this.cbAutoStartAnalyze.Size = new System.Drawing.Size(131, 20);
            this.cbAutoStartAnalyze.TabIndex = 13;
            this.cbAutoStartAnalyze.Text = "Auto start analyze";
            this.cbAutoStartAnalyze.UseVisualStyleBackColor = true;
            //
            // cbAutoCopyResult
            //
            this.cbAutoCopyResult.AutoSize = true;
            this.cbAutoCopyResult.Location = new System.Drawing.Point(16, 464);
            this.cbAutoCopyResult.Name = "cbAutoCopyResult";
            this.cbAutoCopyResult.Size = new System.Drawing.Size(121, 20);
            this.cbAutoCopyResult.TabIndex = 14;
            this.cbAutoCopyResult.Text = "Auto copy result";
            this.cbAutoCopyResult.UseVisualStyleBackColor = true;
            //
            // btnOK
            //
            this.btnOK.Location = new System.Drawing.Point(160, 500);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 32);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(268, 500);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // AIOptionsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 548);
            this.Controls.Add(this.lblProvider);
            this.Controls.Add(this.cbProvider);
            this.Controls.Add(this.btnAPIKeyHelp);
            this.Controls.Add(this.gbOpenAI);
            this.Controls.Add(this.gbGemini);
            this.Controls.Add(this.gbOpenRouter);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.lblTestStatus);
            this.Controls.Add(this.lblReasoningEffort);
            this.Controls.Add(this.cbReasoningEffort);
            this.Controls.Add(this.lblVerbosity);
            this.Controls.Add(this.cbVerbosity);
            this.Controls.Add(this.cbAutoStartRegion);
            this.Controls.Add(this.cbAutoStartAnalyze);
            this.Controls.Add(this.cbAutoCopyResult);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AIOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - AI options";
            this.gbOpenAI.ResumeLayout(false);
            this.gbOpenAI.PerformLayout();
            this.gbGemini.ResumeLayout(false);
            this.gbGemini.PerformLayout();
            this.gbOpenRouter.ResumeLayout(false);
            this.gbOpenRouter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
