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
            cbOpenAIReasoningEffort = new System.Windows.Forms.ComboBox();
            lblReasoningEffort = new System.Windows.Forms.Label();
            lblVerbosity = new System.Windows.Forms.Label();
            cbOpenAIVerbosity = new System.Windows.Forms.ComboBox();
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
            resources.ApplyResources(lblProvider, "lblProvider");
            lblProvider.Name = "lblProvider";
            // 
            // cbProvider
            // 
            cbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbProvider.FormattingEnabled = true;
            resources.ApplyResources(cbProvider, "cbProvider");
            cbProvider.Name = "cbProvider";
            cbProvider.SelectedIndexChanged += cbProvider_SelectedIndexChanged;
            // 
            // btnAPIKeyHelp
            // 
            btnAPIKeyHelp.Image = Properties.Resources.question;
            resources.ApplyResources(btnAPIKeyHelp, "btnAPIKeyHelp");
            btnAPIKeyHelp.Name = "btnAPIKeyHelp";
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
            gbOpenAI.Controls.Add(cbOpenAIReasoningEffort);
            gbOpenAI.Controls.Add(lblReasoningEffort);
            gbOpenAI.Controls.Add(lblVerbosity);
            gbOpenAI.Controls.Add(cbOpenAIVerbosity);
            resources.ApplyResources(gbOpenAI, "gbOpenAI");
            gbOpenAI.Name = "gbOpenAI";
            gbOpenAI.TabStop = false;
            // 
            // lblOpenAIAPIKey
            // 
            resources.ApplyResources(lblOpenAIAPIKey, "lblOpenAIAPIKey");
            lblOpenAIAPIKey.Name = "lblOpenAIAPIKey";
            // 
            // txtOpenAIAPIKey
            // 
            resources.ApplyResources(txtOpenAIAPIKey, "txtOpenAIAPIKey");
            txtOpenAIAPIKey.Name = "txtOpenAIAPIKey";
            txtOpenAIAPIKey.UseSystemPasswordChar = true;
            // 
            // lblOpenAIModel
            // 
            resources.ApplyResources(lblOpenAIModel, "lblOpenAIModel");
            lblOpenAIModel.Name = "lblOpenAIModel";
            // 
            // cbOpenAIModel
            // 
            cbOpenAIModel.FormattingEnabled = true;
            cbOpenAIModel.Items.AddRange(new object[] { resources.GetString("cbOpenAIModel.Items"), resources.GetString("cbOpenAIModel.Items1"), resources.GetString("cbOpenAIModel.Items2"), resources.GetString("cbOpenAIModel.Items3"), resources.GetString("cbOpenAIModel.Items4") });
            resources.ApplyResources(cbOpenAIModel, "cbOpenAIModel");
            cbOpenAIModel.Name = "cbOpenAIModel";
            // 
            // lblOpenAICustomURL
            // 
            resources.ApplyResources(lblOpenAICustomURL, "lblOpenAICustomURL");
            lblOpenAICustomURL.Name = "lblOpenAICustomURL";
            // 
            // txtOpenAICustomURL
            // 
            resources.ApplyResources(txtOpenAICustomURL, "txtOpenAICustomURL");
            txtOpenAICustomURL.Name = "txtOpenAICustomURL";
            // 
            // cbOpenAIReasoningEffort
            // 
            cbOpenAIReasoningEffort.FormattingEnabled = true;
            cbOpenAIReasoningEffort.Items.AddRange(new object[] { resources.GetString("cbOpenAIReasoningEffort.Items"), resources.GetString("cbOpenAIReasoningEffort.Items1"), resources.GetString("cbOpenAIReasoningEffort.Items2"), resources.GetString("cbOpenAIReasoningEffort.Items3") });
            resources.ApplyResources(cbOpenAIReasoningEffort, "cbOpenAIReasoningEffort");
            cbOpenAIReasoningEffort.Name = "cbOpenAIReasoningEffort";
            // 
            // lblReasoningEffort
            // 
            resources.ApplyResources(lblReasoningEffort, "lblReasoningEffort");
            lblReasoningEffort.Name = "lblReasoningEffort";
            // 
            // lblVerbosity
            // 
            resources.ApplyResources(lblVerbosity, "lblVerbosity");
            lblVerbosity.Name = "lblVerbosity";
            // 
            // cbOpenAIVerbosity
            // 
            cbOpenAIVerbosity.FormattingEnabled = true;
            cbOpenAIVerbosity.Items.AddRange(new object[] { resources.GetString("cbOpenAIVerbosity.Items"), resources.GetString("cbOpenAIVerbosity.Items1"), resources.GetString("cbOpenAIVerbosity.Items2") });
            resources.ApplyResources(cbOpenAIVerbosity, "cbOpenAIVerbosity");
            cbOpenAIVerbosity.Name = "cbOpenAIVerbosity";
            // 
            // gbGemini
            // 
            gbGemini.Controls.Add(lblGeminiAPIKey);
            gbGemini.Controls.Add(txtGeminiAPIKey);
            gbGemini.Controls.Add(lblGeminiModel);
            gbGemini.Controls.Add(cbGeminiModel);
            resources.ApplyResources(gbGemini, "gbGemini");
            gbGemini.Name = "gbGemini";
            gbGemini.TabStop = false;
            // 
            // lblGeminiAPIKey
            // 
            resources.ApplyResources(lblGeminiAPIKey, "lblGeminiAPIKey");
            lblGeminiAPIKey.Name = "lblGeminiAPIKey";
            // 
            // txtGeminiAPIKey
            // 
            resources.ApplyResources(txtGeminiAPIKey, "txtGeminiAPIKey");
            txtGeminiAPIKey.Name = "txtGeminiAPIKey";
            txtGeminiAPIKey.UseSystemPasswordChar = true;
            // 
            // lblGeminiModel
            // 
            resources.ApplyResources(lblGeminiModel, "lblGeminiModel");
            lblGeminiModel.Name = "lblGeminiModel";
            // 
            // cbGeminiModel
            // 
            cbGeminiModel.FormattingEnabled = true;
            cbGeminiModel.Items.AddRange(new object[] { resources.GetString("cbGeminiModel.Items"), resources.GetString("cbGeminiModel.Items1"), resources.GetString("cbGeminiModel.Items2"), resources.GetString("cbGeminiModel.Items3") });
            resources.ApplyResources(cbGeminiModel, "cbGeminiModel");
            cbGeminiModel.Name = "cbGeminiModel";
            // 
            // gbOpenRouter
            // 
            gbOpenRouter.Controls.Add(lblOpenRouterAPIKey);
            gbOpenRouter.Controls.Add(txtOpenRouterAPIKey);
            gbOpenRouter.Controls.Add(lblOpenRouterModel);
            gbOpenRouter.Controls.Add(cbOpenRouterModel);
            resources.ApplyResources(gbOpenRouter, "gbOpenRouter");
            gbOpenRouter.Name = "gbOpenRouter";
            gbOpenRouter.TabStop = false;
            // 
            // lblOpenRouterAPIKey
            // 
            resources.ApplyResources(lblOpenRouterAPIKey, "lblOpenRouterAPIKey");
            lblOpenRouterAPIKey.Name = "lblOpenRouterAPIKey";
            // 
            // txtOpenRouterAPIKey
            // 
            resources.ApplyResources(txtOpenRouterAPIKey, "txtOpenRouterAPIKey");
            txtOpenRouterAPIKey.Name = "txtOpenRouterAPIKey";
            txtOpenRouterAPIKey.UseSystemPasswordChar = true;
            // 
            // lblOpenRouterModel
            // 
            resources.ApplyResources(lblOpenRouterModel, "lblOpenRouterModel");
            lblOpenRouterModel.Name = "lblOpenRouterModel";
            // 
            // cbOpenRouterModel
            // 
            cbOpenRouterModel.FormattingEnabled = true;
            cbOpenRouterModel.Items.AddRange(new object[] { resources.GetString("cbOpenRouterModel.Items"), resources.GetString("cbOpenRouterModel.Items1"), resources.GetString("cbOpenRouterModel.Items2") });
            resources.ApplyResources(cbOpenRouterModel, "cbOpenRouterModel");
            cbOpenRouterModel.Name = "cbOpenRouterModel";
            // 
            // btnTestConnection
            // 
            resources.ApplyResources(btnTestConnection, "btnTestConnection");
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // lblTestStatus
            // 
            resources.ApplyResources(lblTestStatus, "lblTestStatus");
            lblTestStatus.Name = "lblTestStatus";
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
            // cbAutoCopyResult
            // 
            resources.ApplyResources(cbAutoCopyResult, "cbAutoCopyResult");
            cbAutoCopyResult.Name = "cbAutoCopyResult";
            cbAutoCopyResult.UseVisualStyleBackColor = true;
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
            // AIOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(lblProvider);
            Controls.Add(cbProvider);
            Controls.Add(btnAPIKeyHelp);
            Controls.Add(gbOpenAI);
            Controls.Add(gbGemini);
            Controls.Add(gbOpenRouter);
            Controls.Add(btnTestConnection);
            Controls.Add(lblTestStatus);
            Controls.Add(cbAutoStartRegion);
            Controls.Add(cbAutoStartAnalyze);
            Controls.Add(cbAutoCopyResult);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "AIOptionsForm";
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
        private System.Windows.Forms.ComboBox cbOpenAIReasoningEffort;
        private System.Windows.Forms.Label lblVerbosity;
        private System.Windows.Forms.ComboBox cbOpenAIVerbosity;
        private System.Windows.Forms.CheckBox cbAutoStartRegion;
        private System.Windows.Forms.CheckBox cbAutoStartAnalyze;
        private System.Windows.Forms.CheckBox cbAutoCopyResult;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
