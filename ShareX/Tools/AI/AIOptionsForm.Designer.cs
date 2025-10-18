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
            this.btnAPIKeyHelp = new System.Windows.Forms.Button();
            this.cbReasoningEffort = new System.Windows.Forms.ComboBox();
            this.lblReasoningEffort = new System.Windows.Forms.Label();
            this.cbAutoStartRegion = new System.Windows.Forms.CheckBox();
            this.cbAutoStartAnalyze = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbAutoCopyResult = new System.Windows.Forms.CheckBox();
            this.cbVerbosity = new System.Windows.Forms.ComboBox();
            this.lblVerbosity = new System.Windows.Forms.Label();
            this.lblProvider = new System.Windows.Forms.Label();
            this.cbProvider = new System.Windows.Forms.ComboBox();
            this.gbOpenAI = new System.Windows.Forms.GroupBox();
            this.lblOpenAICustomURL = new System.Windows.Forms.Label();
            this.txtOpenAICustomURL = new System.Windows.Forms.TextBox();
            this.lblOpenAIModel = new System.Windows.Forms.Label();
            this.cbOpenAIModel = new System.Windows.Forms.ComboBox();
            this.lblOpenAIAPIKey = new System.Windows.Forms.Label();
            this.txtOpenAIAPIKey = new System.Windows.Forms.TextBox();
            this.gbGemini = new System.Windows.Forms.GroupBox();
            this.lblGeminiModel = new System.Windows.Forms.Label();
            this.cbGeminiModel = new System.Windows.Forms.ComboBox();
            this.lblGeminiAPIKey = new System.Windows.Forms.Label();
            this.txtGeminiAPIKey = new System.Windows.Forms.TextBox();
            this.gbOpenRouter = new System.Windows.Forms.GroupBox();
            this.lblOpenRouterModel = new System.Windows.Forms.Label();
            this.cbOpenRouterModel = new System.Windows.Forms.ComboBox();
            this.lblOpenRouterAPIKey = new System.Windows.Forms.Label();
            this.txtOpenRouterAPIKey = new System.Windows.Forms.TextBox();
            this.gbOpenAI.SuspendLayout();
            this.gbGemini.SuspendLayout();
            this.gbOpenRouter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAPIKeyHelp
            // 
            this.btnAPIKeyHelp.Image = global::ShareX.Properties.Resources.question;
            resources.ApplyResources(this.btnAPIKeyHelp, "btnAPIKeyHelp");
            this.btnAPIKeyHelp.Name = "btnAPIKeyHelp";
            this.btnAPIKeyHelp.UseVisualStyleBackColor = true;
            this.btnAPIKeyHelp.Click += new System.EventHandler(this.btnAPIKeyHelp_Click);
            // 
            // cbReasoningEffort
            // 
            this.cbReasoningEffort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReasoningEffort.FormattingEnabled = true;
            this.cbReasoningEffort.Items.AddRange(new object[] {
            resources.GetString("cbReasoningEffort.Items"),
            resources.GetString("cbReasoningEffort.Items1"),
            resources.GetString("cbReasoningEffort.Items2"),
            resources.GetString("cbReasoningEffort.Items3")});
            resources.ApplyResources(this.cbReasoningEffort, "cbReasoningEffort");
            this.cbReasoningEffort.Name = "cbReasoningEffort";
            // 
            // lblReasoningEffort
            // 
            resources.ApplyResources(this.lblReasoningEffort, "lblReasoningEffort");
            this.lblReasoningEffort.Name = "lblReasoningEffort";
            // 
            // cbAutoStartRegion
            // 
            resources.ApplyResources(this.cbAutoStartRegion, "cbAutoStartRegion");
            this.cbAutoStartRegion.Name = "cbAutoStartRegion";
            this.cbAutoStartRegion.UseVisualStyleBackColor = true;
            // 
            // cbAutoStartAnalyze
            // 
            resources.ApplyResources(this.cbAutoStartAnalyze, "cbAutoStartAnalyze");
            this.cbAutoStartAnalyze.Name = "cbAutoStartAnalyze";
            this.cbAutoStartAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbAutoCopyResult
            // 
            resources.ApplyResources(this.cbAutoCopyResult, "cbAutoCopyResult");
            this.cbAutoCopyResult.Name = "cbAutoCopyResult";
            this.cbAutoCopyResult.UseVisualStyleBackColor = true;
            // 
            // cbVerbosity
            // 
            this.cbVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVerbosity.FormattingEnabled = true;
            this.cbVerbosity.Items.AddRange(new object[] {
            resources.GetString("cbVerbosity.Items"),
            resources.GetString("cbVerbosity.Items1"),
            resources.GetString("cbVerbosity.Items2")});
            resources.ApplyResources(this.cbVerbosity, "cbVerbosity");
            this.cbVerbosity.Name = "cbVerbosity";
            // 
            // lblVerbosity
            // 
            resources.ApplyResources(this.lblVerbosity, "lblVerbosity");
            this.lblVerbosity.Name = "lblVerbosity";
            // 
            // lblProvider
            // 
            resources.ApplyResources(this.lblProvider, "lblProvider");
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Text = "Select Provider:";
            // 
            // cbProvider
            // 
            this.cbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProvider.FormattingEnabled = true;
            resources.ApplyResources(this.cbProvider, "cbProvider");
            this.cbProvider.Name = "cbProvider";
            this.cbProvider.SelectedIndexChanged += new System.EventHandler(this.cbProvider_SelectedIndexChanged);
            // 
            // gbOpenAI
            // 
            this.gbOpenAI.Controls.Add(this.lblOpenAICustomURL);
            this.gbOpenAI.Controls.Add(this.txtOpenAICustomURL);
            this.gbOpenAI.Controls.Add(this.lblOpenAIModel);
            this.gbOpenAI.Controls.Add(this.cbOpenAIModel);
            this.gbOpenAI.Controls.Add(this.lblOpenAIAPIKey);
            this.gbOpenAI.Controls.Add(this.txtOpenAIAPIKey);
            resources.ApplyResources(this.gbOpenAI, "gbOpenAI");
            this.gbOpenAI.Name = "gbOpenAI";
            this.gbOpenAI.TabStop = false;
            this.gbOpenAI.Text = "Configure Provider";
            // 
            // lblOpenAICustomURL
            // 
            resources.ApplyResources(this.lblOpenAICustomURL, "lblOpenAICustomURL");
            this.lblOpenAICustomURL.Name = "lblOpenAICustomURL";
            // 
            // txtOpenAICustomURL
            // 
            resources.ApplyResources(this.txtOpenAICustomURL, "txtOpenAICustomURL");
            this.txtOpenAICustomURL.Name = "txtOpenAICustomURL";
            // 
            // lblOpenAIModel
            // 
            resources.ApplyResources(this.lblOpenAIModel, "lblOpenAIModel");
            this.lblOpenAIModel.Name = "lblOpenAIModel";
            // 
            // cbOpenAIModel
            // 
            this.cbOpenAIModel.FormattingEnabled = true;
            resources.ApplyResources(this.cbOpenAIModel, "cbOpenAIModel");
            this.cbOpenAIModel.Name = "cbOpenAIModel";
            // 
            // lblOpenAIAPIKey
            // 
            resources.ApplyResources(this.lblOpenAIAPIKey, "lblOpenAIAPIKey");
            this.lblOpenAIAPIKey.Name = "lblOpenAIAPIKey";
            // 
            // txtOpenAIAPIKey
            // 
            resources.ApplyResources(this.txtOpenAIAPIKey, "txtOpenAIAPIKey");
            this.txtOpenAIAPIKey.Name = "txtOpenAIAPIKey";
            this.txtOpenAIAPIKey.UseSystemPasswordChar = true;
            // 
            // gbGemini
            // 
            this.gbGemini.Controls.Add(this.lblGeminiModel);
            this.gbGemini.Controls.Add(this.cbGeminiModel);
            this.gbGemini.Controls.Add(this.lblGeminiAPIKey);
            this.gbGemini.Controls.Add(this.txtGeminiAPIKey);
            resources.ApplyResources(this.gbGemini, "gbGemini");
            this.gbGemini.Name = "gbGemini";
            this.gbGemini.TabStop = false;
            this.gbGemini.Text = "Configure Provider";
            // 
            // lblGeminiModel
            // 
            resources.ApplyResources(this.lblGeminiModel, "lblGeminiModel");
            this.lblGeminiModel.Name = "lblGeminiModel";
            // 
            // cbGeminiModel
            // 
            this.cbGeminiModel.FormattingEnabled = true;
            resources.ApplyResources(this.cbGeminiModel, "cbGeminiModel");
            this.cbGeminiModel.Name = "cbGeminiModel";
            // 
            // lblGeminiAPIKey
            // 
            resources.ApplyResources(this.lblGeminiAPIKey, "lblGeminiAPIKey");
            this.lblGeminiAPIKey.Name = "lblGeminiAPIKey";
            // 
            // txtGeminiAPIKey
            // 
            resources.ApplyResources(this.txtGeminiAPIKey, "txtGeminiAPIKey");
            this.txtGeminiAPIKey.Name = "txtGeminiAPIKey";
            this.txtGeminiAPIKey.UseSystemPasswordChar = true;
            // 
            // gbOpenRouter
            // 
            this.gbOpenRouter.Controls.Add(this.lblOpenRouterModel);
            this.gbOpenRouter.Controls.Add(this.cbOpenRouterModel);
            this.gbOpenRouter.Controls.Add(this.lblOpenRouterAPIKey);
            this.gbOpenRouter.Controls.Add(this.txtOpenRouterAPIKey);
            resources.ApplyResources(this.gbOpenRouter, "gbOpenRouter");
            this.gbOpenRouter.Name = "gbOpenRouter";
            this.gbOpenRouter.TabStop = false;
            this.gbOpenRouter.Text = "Configure Provider";
            // 
            // lblOpenRouterModel
            // 
            resources.ApplyResources(this.lblOpenRouterModel, "lblOpenRouterModel");
            this.lblOpenRouterModel.Name = "lblOpenRouterModel";
            // 
            // cbOpenRouterModel
            // 
            this.cbOpenRouterModel.FormattingEnabled = true;
            resources.ApplyResources(this.cbOpenRouterModel, "cbOpenRouterModel");
            this.cbOpenRouterModel.Name = "cbOpenRouterModel";
            // 
            // lblOpenRouterAPIKey
            // 
            resources.ApplyResources(this.lblOpenRouterAPIKey, "lblOpenRouterAPIKey");
            this.lblOpenRouterAPIKey.Name = "lblOpenRouterAPIKey";
            // 
            // txtOpenRouterAPIKey
            // 
            resources.ApplyResources(this.txtOpenRouterAPIKey, "txtOpenRouterAPIKey");
            this.txtOpenRouterAPIKey.Name = "txtOpenRouterAPIKey";
            this.txtOpenRouterAPIKey.UseSystemPasswordChar = true;
            // 
            // AIOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblProvider);
            this.Controls.Add(this.cbProvider);
            this.Controls.Add(this.gbOpenAI);
            this.Controls.Add(this.gbGemini);
            this.Controls.Add(this.gbOpenRouter);
            this.Controls.Add(this.lblVerbosity);
            this.Controls.Add(this.cbVerbosity);
            this.Controls.Add(this.cbAutoCopyResult);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbAutoStartAnalyze);
            this.Controls.Add(this.cbAutoStartRegion);
            this.Controls.Add(this.btnAPIKeyHelp);
            this.Controls.Add(this.cbReasoningEffort);
            this.Controls.Add(this.lblReasoningEffort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AIOptionsForm";
            this.gbOpenAI.ResumeLayout(false);
            this.gbOpenAI.PerformLayout();
            this.gbGemini.ResumeLayout(false);
            this.gbGemini.PerformLayout();
            this.gbOpenRouter.ResumeLayout(false);
            this.gbOpenRouter.PerformLayout();

            // Explicit manual layout overrides to ensure consistent UI regardless of stale .resx
            // Increase form size to accommodate provider configuration group and other options
            this.ClientSize = new System.Drawing.Size(420, 520);

            // Provider selection at the top
            this.lblProvider.AutoSize = true;
            this.lblProvider.Location = new System.Drawing.Point(16, 16);

            this.cbProvider.Location = new System.Drawing.Point(16, 36);
            this.cbProvider.Size = new System.Drawing.Size(this.ClientSize.Width - 56, this.cbProvider.Height);
            this.cbProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            // Help button next to provider selector
            this.btnAPIKeyHelp.Location = new System.Drawing.Point(this.ClientSize.Width - 16 - this.btnAPIKeyHelp.Width, 32);
            this.btnAPIKeyHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // Recompute provider ComboBox width to avoid overlap with help button (leave 8px gap)
            this.cbProvider.Size = new System.Drawing.Size(this.btnAPIKeyHelp.Left - 8 - this.cbProvider.Left, this.cbProvider.Height);

            // Group boxes share the same location/size; visibility toggled by provider selection
            int groupLeft = 16;
            int groupTop = 72;
            int groupWidth = this.ClientSize.Width - 32;
            int groupHeight = 210;

            this.gbOpenAI.Location = new System.Drawing.Point(groupLeft, groupTop);
            this.gbOpenAI.Size = new System.Drawing.Size(groupWidth, groupHeight);
            this.gbOpenAI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            this.gbGemini.Location = new System.Drawing.Point(groupLeft, groupTop);
            this.gbGemini.Size = new System.Drawing.Size(groupWidth, groupHeight);
            this.gbGemini.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            this.gbOpenRouter.Location = new System.Drawing.Point(groupLeft, groupTop);
            this.gbOpenRouter.Size = new System.Drawing.Size(groupWidth, groupHeight);
            this.gbOpenRouter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            // OpenAI group contents
            this.lblOpenAIAPIKey.Location = new System.Drawing.Point(12, 24);
            this.txtOpenAIAPIKey.Location = new System.Drawing.Point(12, 44);
            this.txtOpenAIAPIKey.Size = new System.Drawing.Size(groupWidth - 24, this.txtOpenAIAPIKey.Height);

            this.lblOpenAIModel.Location = new System.Drawing.Point(12, 80);
            this.cbOpenAIModel.Location = new System.Drawing.Point(12, 100);
            this.cbOpenAIModel.Size = new System.Drawing.Size(groupWidth - 24, this.cbOpenAIModel.Height);

            this.lblOpenAICustomURL.Location = new System.Drawing.Point(12, 136);
            this.txtOpenAICustomURL.Location = new System.Drawing.Point(12, 156);
            this.txtOpenAICustomURL.Size = new System.Drawing.Size(groupWidth - 24, this.txtOpenAICustomURL.Height);

            // Ensure labels have text and autosize; inputs expand horizontally
            this.lblOpenAIAPIKey.AutoSize = true;
            this.lblOpenAIAPIKey.Text = "API key:";
            this.txtOpenAIAPIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            this.lblOpenAIModel.AutoSize = true;
            this.lblOpenAIModel.Text = "Model:";
            this.cbOpenAIModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOpenAIModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

            this.lblOpenAICustomURL.AutoSize = true;
            this.lblOpenAICustomURL.Text = "Custom base URL:";
            this.txtOpenAICustomURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            // Gemini group contents
            this.lblGeminiAPIKey.Location = new System.Drawing.Point(12, 24);
            this.txtGeminiAPIKey.Location = new System.Drawing.Point(12, 44);
            this.txtGeminiAPIKey.Size = new System.Drawing.Size(groupWidth - 24, this.txtGeminiAPIKey.Height);

            this.lblGeminiModel.Location = new System.Drawing.Point(12, 80);
            this.cbGeminiModel.Location = new System.Drawing.Point(12, 100);
            this.cbGeminiModel.Size = new System.Drawing.Size(groupWidth - 24, this.cbGeminiModel.Height);

            // Ensure labels have text and autosize; inputs expand horizontally (Gemini)
            this.lblGeminiAPIKey.AutoSize = true;
            this.lblGeminiAPIKey.Text = "API key:";
            this.txtGeminiAPIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            this.lblGeminiModel.AutoSize = true;
            this.lblGeminiModel.Text = "Model:";
            this.cbGeminiModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGeminiModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

            // OpenRouter group contents
            this.lblOpenRouterAPIKey.Location = new System.Drawing.Point(12, 24);
            this.txtOpenRouterAPIKey.Location = new System.Drawing.Point(12, 44);
            this.txtOpenRouterAPIKey.Size = new System.Drawing.Size(groupWidth - 24, this.txtOpenRouterAPIKey.Height);

            this.lblOpenRouterModel.Location = new System.Drawing.Point(12, 80);
            this.cbOpenRouterModel.Location = new System.Drawing.Point(12, 100);
            this.cbOpenRouterModel.Size = new System.Drawing.Size(groupWidth - 24, this.cbOpenRouterModel.Height);

            // Ensure labels have text and autosize; inputs expand horizontally (OpenRouter)
            this.lblOpenRouterAPIKey.AutoSize = true;
            this.lblOpenRouterAPIKey.Text = "API key:";
            this.txtOpenRouterAPIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));

            this.lblOpenRouterModel.AutoSize = true;
            this.lblOpenRouterModel.Text = "Model:";
            this.cbOpenRouterModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOpenRouterModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

            // Remaining options stacked under the provider config group
            // Test connection button and status
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblTestStatus = new System.Windows.Forms.Label();

            this.btnTestConnection.Text = "Test";
            this.btnTestConnection.Size = new System.Drawing.Size(80, 28);
            this.btnTestConnection.Location = new System.Drawing.Point(this.ClientSize.Width - 16 - this.btnTestConnection.Width, groupTop + groupHeight + 8);
            this.btnTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);

            this.lblTestStatus.AutoSize = true;
            this.lblTestStatus.Location = new System.Drawing.Point(16, groupTop + groupHeight + 12);
            this.lblTestStatus.Text = "";
            this.Controls.Add(this.lblTestStatus);
            this.Controls.Add(this.btnTestConnection);

            int y = groupTop + groupHeight + 12 + 36;

            this.lblReasoningEffort.Location = new System.Drawing.Point(16, y);
            this.cbReasoningEffort.Location = new System.Drawing.Point(16, y + 24);
            this.cbReasoningEffort.Size = new System.Drawing.Size(this.ClientSize.Width - 32, this.cbReasoningEffort.Height);
            this.cbReasoningEffort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            y += 24 + 24 + 12;

            this.lblVerbosity.Location = new System.Drawing.Point(16, y);
            this.cbVerbosity.Location = new System.Drawing.Point(16, y + 24);
            this.cbVerbosity.Size = new System.Drawing.Size(this.ClientSize.Width - 32, this.cbVerbosity.Height);
            this.cbVerbosity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            y += 24 + 24 + 12;

            this.cbAutoStartRegion.Location = new System.Drawing.Point(16, y);
            this.cbAutoStartAnalyze.Location = new System.Drawing.Point(16, y + 24);
            this.cbAutoCopyResult.Location = new System.Drawing.Point(16, y + 48);

            // OK/Cancel at bottom-right
            this.btnOK.Location = new System.Drawing.Point(this.ClientSize.Width - 224, this.ClientSize.Height - 48);
            this.btnCancel.Location = new System.Drawing.Point(this.ClientSize.Width - 112, this.ClientSize.Height - 48);
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAPIKeyHelp;
        private System.Windows.Forms.ComboBox cbReasoningEffort;
        private System.Windows.Forms.Label lblReasoningEffort;
        private System.Windows.Forms.CheckBox cbAutoStartRegion;
        private System.Windows.Forms.CheckBox cbAutoStartAnalyze;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbAutoCopyResult;
        private System.Windows.Forms.ComboBox cbVerbosity;
        private System.Windows.Forms.Label lblVerbosity;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.ComboBox cbProvider;
        private System.Windows.Forms.GroupBox gbOpenAI;
        private System.Windows.Forms.Label lblOpenAIModel;
        private System.Windows.Forms.ComboBox cbOpenAIModel;
        private System.Windows.Forms.Label lblOpenAIAPIKey;
        private System.Windows.Forms.TextBox txtOpenAIAPIKey;
        private System.Windows.Forms.GroupBox gbGemini;
        private System.Windows.Forms.Label lblGeminiModel;
        private System.Windows.Forms.ComboBox cbGeminiModel;
        private System.Windows.Forms.Label lblGeminiAPIKey;
        private System.Windows.Forms.TextBox txtGeminiAPIKey;
        private System.Windows.Forms.GroupBox gbOpenRouter;
        private System.Windows.Forms.Label lblOpenRouterModel;
        private System.Windows.Forms.ComboBox cbOpenRouterModel;
        private System.Windows.Forms.Label lblOpenRouterAPIKey;
        private System.Windows.Forms.TextBox txtOpenRouterAPIKey;
        private System.Windows.Forms.Label lblOpenAICustomURL;
        private System.Windows.Forms.TextBox txtOpenAICustomURL;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblTestStatus;
    }
}
